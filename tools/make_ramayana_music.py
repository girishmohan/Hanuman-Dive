"""Synthesizes an original devotional-style background loop (pure Python, no deps).

Raag Bhupali melody (Sa Re Ga Pa Dha) on a bansuri-like flute over a tanpura
drone, tabla in keherwa taal and manjira. Written for this game, so it is free
of third-party copyright. The loop is seamless (tails wrap to the start).

    python3 tools/make_ramayana_music.py
"""
import math
import os
import random
import struct
import wave

RATE = 22050
BPM = 96
BEAT = 60.0 / BPM
SA = 277.18  # C#4, flute's Sa; tanpura and tabla are tuned to the same Sa
TAU = 2 * math.pi

# Melody as (semitones from Sa, beats). Four 4-bar phrases = 16 bars.
MELODY = [
    # A
    (4, 1), (2, .5), (0, .5), (-3, 1), (0, 1),
    (2, 1), (4, 2), (7, 1),
    (9, 1), (7, .5), (4, .5), (2, 1), (4, 1),
    (2, 1), (0, 3),
    # B
    (7, 1), (9, 1), (12, 2),
    (12, .5), (14, .5), (12, 1), (9, 1), (7, 1),
    (9, 1), (7, 1), (4, 1), (7, 1),
    (4, 1), (2, 1), (4, 2),
    # A'
    (4, 1), (2, .5), (0, .5), (-3, 1), (0, 1),
    (2, 1), (4, 2), (7, 1),
    (9, 2), (12, 1), (9, 1),
    (7, 4),
    # C (resolves to Sa so the loop restarts cleanly)
    (12, 1), (14, 1), (16, 2),
    (14, 1), (12, 1), (9, 2),
    (7, 1), (9, 1), (7, 1), (4, 1),
    (2, 1), (0, 3),
]
BARS = 16
LOOP_SECONDS = BARS * 4 * BEAT
TAIL_SECONDS = 3.0


def freq(semi, base=SA):
    return base * 2 ** (semi / 12)


class Mix:
    def __init__(self, seconds):
        self.buf = [0.0] * int(seconds * RATE)

    def add(self, start_s, samples, gain=1.0):
        i0 = int(start_s * RATE)
        buf = self.buf
        for k, v in enumerate(samples):
            i = i0 + k
            if i >= len(buf):
                break
            buf[i] += v * gain


def flute(mix):
    """One continuous voice with glides between notes, vibrato and breath."""
    rnd = random.Random(7)
    t = 0.0
    events = []
    for semi, beats in MELODY:
        events.append((t, beats * BEAT, freq(semi)))
        t += beats * BEAT
    total = int(LOOP_SECONDS * RATE)
    out = [0.0] * total
    phase = 0.0
    cur = events[-1][2]  # start from the last note so the loop glides seamlessly
    ei = 0
    lp = 0.0
    for i in range(total):
        ts = i / RATE
        while ei + 1 < len(events) and ts >= events[ei + 1][0]:
            ei += 1
        start, dur, target = events[ei]
        local = ts - start
        # Meend: glide 70 ms into the new note.
        cur += (target - cur) * min(1.0, 1.0 / (0.07 * RATE) * 4)
        vib = 1 + 0.004 * math.sin(TAU * 5.3 * ts) * min(1.0, max(0.0, (local - 0.15) / 0.2))
        phase += TAU * cur * vib / RATE
        # Soft re-articulation at each note start, gentle fade at the end.
        env = min(1.0, local / 0.04) * min(1.0, (dur - local) / 0.06 + 0.35)
        tone = math.sin(phase) + 0.22 * math.sin(2 * phase) + 0.07 * math.sin(3 * phase)
        noise = rnd.uniform(-1, 1)
        lp += 0.08 * (noise - lp)
        out[i] = env * (tone + 0.35 * lp)
    mix.add(0, out, 0.30)


def pluck(f, seconds, harmonics, decay):
    n = int(seconds * RATE)
    out = [0.0] * n
    for h in range(1, harmonics + 1):
        amp = 1.0 / h
        w = TAU * f * h * (1 + 0.0007 * h) / RATE
        d = decay / (1 + 0.25 * h)
        for i in range(n):
            t = i / RATE
            # Jawari-like swell of the upper harmonics.
            swell = 1 + (0.6 * math.sin(math.pi * min(1.0, t / seconds)) if h > 3 else 0)
            out[i] += amp * swell * math.sin(w * i) * math.exp(-t / d)
    return out


def tanpura(mix):
    pa_low = freq(-5, SA / 2)
    sa = SA / 2
    sa_low = SA / 4
    pattern = [pa_low, sa, sa, sa_low]
    step = 2 * BEAT / len(pattern) * 2  # one cycle every 4 beats
    cache = {}
    t = 0.0
    k = 0
    while t < LOOP_SECONDS:
        f = pattern[k % 4]
        if f not in cache:
            cache[f] = pluck(f, 3.0, 7, 1.4)
        mix.add(t, cache[f], 0.07)
        t += step
        k += 1


def drum(f0, f1, seconds, decay, partials=(1.0,), noise=0.0, seed=0):
    rnd = random.Random(seed)
    n = int(seconds * RATE)
    out = [0.0] * n
    phase = [0.0] * len(partials)
    for i in range(n):
        t = i / RATE
        f = f1 + (f0 - f1) * math.exp(-t / 0.03)
        e = math.exp(-t / decay)
        v = 0.0
        for j, p in enumerate(partials):
            phase[j] += TAU * f * p / RATE
            v += math.sin(phase[j]) / (j + 1)
        if noise:
            v += noise * rnd.uniform(-1, 1) * math.exp(-t / 0.01)
        out[i] = v * e
    return out


def tabla(mix):
    ge = drum(115, 72, 0.6, 0.22)
    na = drum(SA * 1.02, SA, 0.35, 0.12, partials=(1.0, 2.0, 3.0, 4.0), noise=0.6, seed=1)
    tin = drum(SA * 1.02, SA, 0.6, 0.30, partials=(1.0, 2.0, 3.0), noise=0.3, seed=2)
    ti = drum(900, 700, 0.08, 0.02, noise=1.0, seed=3)
    bols = {"Dha": (ge, na), "Ge": (ge,), "Na": (na,), "Ti": (ti,), "Ka": (ti,), "Dhi": (ge, tin)}
    keherwa = ["Dha", "Ge", "Na", "Ti", "Na", "Ka", "Dhi", "Na"]
    eighth = BEAT / 2
    for bar in range(BARS):
        for j, bol in enumerate(keherwa):
            t = (bar * 8 + j) * eighth
            accent = 1.0 if j in (0, 6) else 0.7
            for s in bols[bol]:
                mix.add(t, s, 0.22 * accent)


def manjira(mix):
    n = int(0.9 * RATE)
    ring = [0.0] * n
    for p, a in ((3150, 1.0), (4720, 0.6), (6300, 0.4), (7900, 0.25)):
        w = TAU * p / RATE
        for i in range(n):
            ring[i] += a * math.sin(w * i) * math.exp(-i / RATE / 0.35)
    for bar in range(BARS):
        for beat in (0, 2):
            mix.add((bar * 4 + beat) * BEAT, ring, 0.05)


def main():
    mix = Mix(LOOP_SECONDS + TAIL_SECONDS)
    tanpura(mix)
    tabla(mix)
    manjira(mix)
    flute(mix)

    loop_n = int(LOOP_SECONDS * RATE)
    buf = mix.buf
    for i in range(loop_n, len(buf)):  # wrap the tails for a seamless loop
        buf[i - loop_n] += buf[i]
    buf = buf[:loop_n]
    peak = max(abs(v) for v in buf) or 1.0
    scale = 0.89 / peak

    out = os.path.join(os.path.dirname(__file__), "..", "hyper dive", "Assets",
                       "A-Z_Materials", "bgMusic", "ramayana_theme_loop.wav")
    with wave.open(out, "wb") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(RATE)
        w.writeframes(b"".join(struct.pack("<h", int(v * scale * 32767)) for v in buf))
    print("wrote", os.path.normpath(out), f"{LOOP_SECONDS:.1f}s")


if __name__ == "__main__":
    main()
