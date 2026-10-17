"""Gameplay theme per theme-brief.pdf (pure Python, no deps).

Serene, devotional loop: bansuri lead in Raag Yaman over a steady sa-pa tanpura,
no percussion, no vocals. 66 BPM, 80 beats = 20 tanpura cycles = 72.7 s.
Loudest in the first half, softer at the end, and the loop point falls on a
drone-cycle boundary with all tails (drone ring + reverb) wrapped to the start,
so the join is inaudible. Output: 44.1 kHz stereo 16-bit WAV, about -16 LUFS.

    python3 tools/make_ramayana_music.py
"""
import math
import os
import random
import struct
import wave

RATE = 44100
BPM = 66
BEAT = 60.0 / BPM
CYCLE_BEATS = 4                      # one tanpura cycle: Pa, Sa', Sa', Sa
CYCLES = 20
LOOP_BEATS = CYCLE_BEATS * CYCLES    # 80 beats
LOOP_S = LOOP_BEATS * BEAT
TAIL_S = 7.0
SA = 220.0                           # bansuri Sa (A3); tanpura an octave below
TAU = 2 * math.pi

# Raag Yaman (all shuddh, tivra Ma): S R G M# P D N
S, R, G, M, P, D, N = 0, 2, 4, 6, 7, 9, 11
LOW_N, LOW_D = -1, -3
HI_S, HI_R = 12, 14

# (start beat, swara, length in beats, loudness 0..1). Simple, repetitive, aarti-like.
PHRASES = [
    # Opening, sparse: drone alone for one cycle, then the flute speaks softly.
    (4, LOW_N, 1.5, .55), (5.5, R, 1.5, .6), (7, G, 4, .65),
    (12, R, 1, .55), (13, G, 1, .6), (14, R, 1, .55), (15, S, 4, .55),
    # Main theme, the loudest part (first half).
    (20, LOW_N, 1, .8), (21, R, 1, .85), (22, G, 1, .9), (23, M, 1, .9), (24, P, 4, 1.0),
    (28, M, 1, .85), (29, G, 1, .85), (30, R, 1.5, .8), (31.5, G, 2.5, .85),
    (34, M, 1, .9), (35, D, 1, .95), (36, N, 1, .95), (37, HI_S, 4, 1.0),
    (41, N, 1, .85), (42, D, 1, .85), (43, P, 3, .9),
    (46, M, 1, .8), (47, G, 1, .8), (48, R, 1.5, .75), (49.5, S, 2.5, .75),
    # Gentle answer, a little softer.
    (52, G, 1.5, .7), (53.5, M, 1.5, .7), (55, P, 3, .72),
    (58, M, 1, .65), (59, G, 1, .65), (60, R, 3, .62),
    (64, LOW_N, 1, .55), (65, R, 1, .55), (66, G, 3, .55),
    # Settling: long, quiet tones back towards Sa; no final cadence.
    (70, R, 2.5, .42), (72.5, S, 4.5, .38),
    # Last 3 beats: drone alone, so the restart arrives softly.
]


def hz(semi, base=SA):
    return base * 2 ** (semi / 12)


class Stereo:
    def __init__(self, seconds):
        n = int(seconds * RATE)
        self.l = [0.0] * n
        self.r = [0.0] * n

    def add(self, start_s, mono, gain, pan):
        i0 = int(start_s * RATE)
        gl = gain * math.cos((pan + 1) * math.pi / 4)
        gr = gain * math.sin((pan + 1) * math.pi / 4)
        l, r = self.l, self.r
        end = min(len(l), i0 + len(mono))
        for k in range(max(0, -i0), end - i0):
            v = mono[k]
            l[i0 + k] += v * gl
            r[i0 + k] += v * gr


def tanpura_string(f, seconds):
    """One plucked string with the slow 'jawari' bloom of upper harmonics."""
    n = int(seconds * RATE)
    out = [0.0] * n
    for h in range(1, 13):
        amp = 1.0 / (h ** 1.1)
        bloom_peak = 0.8 + 0.25 * h          # higher harmonics bloom later
        for detune in (-0.12, 0.12):         # two slightly detuned 'strings' -> warmth
            w = TAU * (f * h + detune) / RATE
            ph = random.random() * TAU
            for i in range(0, n):
                t = i / RATE
                env = (1 - math.exp(-t / 0.012)) * math.exp(-t / 3.2)
                bloom = 1 + (h / 6) * math.exp(-((t - bloom_peak) ** 2) / 0.9)
                out[i] += 0.5 * amp * env * bloom * math.sin(w * i + ph)
    return out


def tanpura(mix):
    random.seed(11)
    strings = [hz(-5, SA / 2), hz(0, SA), hz(0, SA), hz(0, SA / 2)]   # Pa, Sa', Sa', Sa
    pans = [-0.35, -0.1, 0.1, 0.35]
    rendered = {}
    for c in range(CYCLES):
        for k, f in enumerate(strings):
            key = (k, f)
            if key not in rendered:
                rendered[key] = tanpura_string(f, 5.0)
            mix.add((c * CYCLE_BEATS + k) * BEAT, rendered[key], 0.055, pans[k])


def bandpass(signal, f, q):
    w0 = TAU * f / RATE
    alpha = math.sin(w0) / (2 * q)
    b0, b2 = alpha, -alpha
    a0, a1, a2 = 1 + alpha, -2 * math.cos(w0), 1 - alpha
    x1 = x2 = y1 = y2 = 0.0
    out = []
    for x in signal:
        y = (b0 * x + b2 * x2 - a1 * y1 - a2 * y2) / a0
        x2, x1, y2, y1 = x1, x, y1, y
        out.append(y)
    return out


def bansuri(mix):
    rnd = random.Random(5)
    n = int((LOOP_S + TAIL_S) * RATE)
    tone = [0.0] * n
    breath_env = [0.0] * n
    # Rubato: small, fixed timing nudges per note; the drone keeps strict time.
    notes = []
    for start, semi, length, vel in PHRASES:
        nudge = rnd.uniform(-0.05, 0.08)
        notes.append(((start * BEAT) + nudge, length * BEAT, hz(semi), vel))
    phase = 0.0
    cur = notes[0][2]
    idx = 0
    for i in range(n):
        t = i / RATE
        while idx + 1 < len(notes) and t >= notes[idx + 1][0]:
            idx += 1
        start, dur, target, vel = notes[idx]
        local = t - start
        if local < 0:
            continue
        # Meend: unhurried glide into each note, slight scoop from below.
        glide = 1 - math.exp(-local / 0.07)
        f = cur + (target - cur) * glide
        if glide > 0.999:
            cur = target
        scoop = 1 - 0.012 * math.exp(-local / 0.05)
        vib_depth = 0.0045 * min(1.0, max(0.0, (local - 0.45) / 0.6))
        vib = 1 + vib_depth * math.sin(TAU * 4.8 * t)
        phase += TAU * f * scoop * vib / RATE
        attack = 1 - math.exp(-local / 0.09)
        release = min(1.0, max(0.0, (dur + 0.35 - local) / 0.35))
        swell = 1 + 0.12 * math.sin(math.pi * min(1.0, local / max(dur, 0.1)))
        env = attack * release * swell * vel
        tone[i] = env * (math.sin(phase) + 0.16 * math.sin(2 * phase) + 0.05 * math.sin(3 * phase))
        # Breath: strong 'chiff' at the attack, a soft airy layer while it sounds.
        breath_env[i] = env * (0.10 + 0.55 * math.exp(-local / 0.06))
    noise = [rnd.uniform(-1, 1) for _ in range(n)]
    air = bandpass(noise, 1800, 0.7)
    flute = [tone[i] + 0.55 * air[i] * breath_env[i] for i in range(n)]
    mix.add(0, flute, 0.22, 0.05)


def reverb(l, r, wet=0.28):
    """Small stereo Schroeder hall: 4 combs + 2 allpasses per channel."""
    def channel(x, spread):
        combs = [int(RATE * d) + spread for d in (0.0297, 0.0371, 0.0411, 0.0437)]
        out = [0.0] * len(x)
        for d in combs:
            buf = [0.0] * d
            j = 0
            g = 0.83
            lp = 0.0
            for i, v in enumerate(x):
                y = buf[j]
                lp = y * 0.7 + lp * 0.3            # damping
                buf[j] = v + lp * g
                j = (j + 1) % d
                out[i] += y
        for d, g in ((int(RATE * 0.005) + spread, 0.7), (int(RATE * 0.0017) + spread, 0.7)):
            buf = [0.0] * d
            j = 0
            for i, v in enumerate(out):
                b = buf[j]
                y = -g * v + b
                buf[j] = v + g * y
                j = (j + 1) % d
                out[i] = y
        return out
    wl, wr = channel(l, 0), channel(r, 23)
    return ([a * (1 - wet) + b * wet * 0.25 for a, b in zip(l, wl)],
            [a * (1 - wet) + b * wet * 0.25 for a, b in zip(r, wr)])


def main():
    mix = Stereo(LOOP_S + TAIL_S)
    tanpura(mix)
    bansuri(mix)
    l, r = reverb(mix.l, mix.r)

    # Seamless loop: wrap everything past the loop point back onto the start.
    n = int(round(LOOP_S * RATE))
    for i in range(n, len(l)):
        l[i - n] += l[i]
        r[i - n] += r[i]
    l, r = l[:n], r[:n]

    # Loudness: aim for ~-16 LUFS (approximated from RMS), soft-limit peaks.
    rms = math.sqrt(sum(a * a + b * b for a, b in zip(l, r)) / (2 * n))
    gain = 10 ** ((-16 + 0.7) / 20) / rms
    def limit(v):
        # Transparent below 0.6, smooth soft knee above it (ceiling ~0.95).
        v *= gain
        a = abs(v)
        if a <= 0.6:
            return v
        return math.copysign(0.6 + 0.35 * math.tanh((a - 0.6) / 0.35), v)

    out = os.path.join(os.path.dirname(__file__), "..", "hyper dive", "Assets",
                       "A-Z_Materials", "bgMusic", "ramayana_theme_loop.wav")
    with wave.open(out, "wb") as w:
        w.setnchannels(2)
        w.setsampwidth(2)
        w.setframerate(RATE)
        frames = bytearray()
        for a, b in zip(l, r):
            frames += struct.pack("<hh", int(max(-1, min(1, limit(a))) * 32767),
                                  int(max(-1, min(1, limit(b))) * 32767))
        w.writeframes(bytes(frames))
    print("wrote", os.path.normpath(out), f"{LOOP_S:.1f}s")


if __name__ == "__main__":
    main()
