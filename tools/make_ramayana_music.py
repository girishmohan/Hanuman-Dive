"""Gameplay theme for Hanuman Dive (pure Python, no deps).

Faster and more captivating, still devotional: a catchy bansuri hook in Raag
Hamsadhwani (S R G P N, bright and auspicious) over a sa-pa tanpura, a soft
harmonium (shruti) pad, tabla in keherwa taal, manjira, and swarmandal sweeps.
No vocals. 100 BPM, 32 tanpura cycles = 76.8 s.

Loop rules from theme-brief.pdf still hold: the loop point is on a drone-cycle
boundary, all tails (drone ring, reverb) wrap to the start so the join is
inaudible, the loudest part is in the first half and the end is softer.
Output: 44.1 kHz stereo 16-bit WAV, about -16 LUFS.

    python3 tools/make_ramayana_music.py
"""
import math
import os
import random
import struct
import wave

RATE = 44100
BPM = 100
BEAT = 60.0 / BPM
CYCLE = 4                     # beats per tanpura cycle / bar
BARS = 32
LOOP_S = BARS * CYCLE * BEAT  # 76.8 s
TAIL_S = 6.0
SA = 261.63                   # bansuri Sa (C4); tanpura and tabla tuned to it
TAU = 2 * math.pi

# Raag Hamsadhwani: S R G P N (no Ma, no Dha)
S, R, G, P, N = 0, 2, 4, 7, 11
N_, P_ = -1, -5               # lower octave
S2, R2, G2 = 12, 14, 16       # upper octave
REST = None

# Sections (bars): intro 0-3, groove 4-15, peak 16-23, settle 24-31.
INTRO, GROOVE, PEAK, SETTLE = range(0, 4), range(4, 16), range(16, 24), range(24, 32)


def section_of(bar):
    if bar in INTRO: return "intro"
    if bar in GROOVE: return "groove"
    if bar in PEAK: return "peak"
    return "settle"


# ---- Melody -----------------------------------------------------------------
# Each bar is a list of (swara or REST, beats); every bar sums to 4 beats.
# '~' prefix on a tuple's third item = grace note (kan) from that swara.
HOOK = [
    [(G, .5), (R, .5), (S, .5), (N_, .5), (S, 1), (R, .5), (G, .5)],
    [(P, 1.5), (G, .5), (R, 1), (S, 1)],
    [(G, .5), (P, .5), (N, .5), (S2, .5), (N, 1), (P, .5), (G, .5)],
    [(P, 1), (G, .5), (R, .5), (S, 2)],
]
ANSWER = [
    [(S2, .5), (N, .5), (P, .5), (G, .5), (P, 1), (N, 1)],
    [(S2, 2), (R2, .5), (S2, .5), (N, 1)],
    [(P, .5), (N, .5), (P, .5), (G, .5), (R, 1), (G, 1)],
    [(S, 2.5), (REST, .5), (R, .5), (G, .5)],
]
RUNS = [
    [(G, .25), (P, .25), (N, .25), (S2, .25), (R2, .5), (S2, .5), (N, .5), (P, .5), (G, 1)],
    [(N, .25), (S2, .25), (R2, .25), (G2, .25), (R2, 1), (S2, .5), (N, .5), (S2, 1)],
    [(S2, .5), (N, .25), (P, .25), (G, .5), (P, .5), (N, .5), (S2, .5), (R2, 1)],
    [(S2, 1.5), (N, .5), (P, .5), (G, .5), (R, .5), (G, .5)],
]
ALAAP = [
    [(REST, 2), (N_, 1), (S, 1)],
    [(R, 1.5), (G, 2.5)],
    [(P, 1), (G, .5), (R, .5), (S, 2)],
    [(REST, 2), (R, .5), (G, .5), (P, 1)],
]
SETTLE_PHRASES = [
    [(G, 1), (R, .5), (S, .5), (N_, 1), (S, 1)],
    [(R, 1), (G, 1), (P, 2)],
    [(G, 1), (R, 1), (S, 2)],
    [(REST, 1), (N_, 1), (S, 2)],
]


def melody_bars():
    bars = []
    bars += ALAAP                       # 0-3   intro, free and calm
    bars += HOOK + ANSWER + HOOK        # 4-15  groove: hook, answer, hook
    bars += RUNS + HOOK                 # 16-23 peak: faster runs, hook again
    bars += SETTLE_PHRASES + [          # 24-31 settle, softer, back toward Sa
        [(G, .5), (R, .5), (S, .5), (N_, .5), (S, 2)],
        [(REST, 1), (R, 1), (S, 2)],
        [(REST, 2), (N_, 1), (S, 1)],
        [(REST, 4)],
    ]
    assert len(bars) == BARS
    for b in bars:
        assert abs(sum(d for _, d in b) - 4) < 1e-6, b
    return bars


def hz(semi, base=SA):
    return base * 2 ** (semi / 12)


class Mix:
    def __init__(self, seconds):
        n = int(seconds * RATE)
        self.l = [0.0] * n
        self.r = [0.0] * n

    def add(self, start_s, mono, gain, pan=0.0):
        i0 = int(start_s * RATE)
        gl = gain * math.cos((pan + 1) * math.pi / 4)
        gr = gain * math.sin((pan + 1) * math.pi / 4)
        l, r = self.l, self.r
        end = min(len(l), i0 + len(mono))
        for k in range(max(0, -i0), end - i0):
            v = mono[k]
            l[i0 + k] += v * gl
            r[i0 + k] += v * gr


# ---- Tanpura ----------------------------------------------------------------
def tanpura_string(f, seconds):
    n = int(seconds * RATE)
    out = [0.0] * n
    rnd = random.Random(int(f))
    for h in range(1, 11):
        amp = 1.0 / (h ** 1.1)
        bloom_peak = 0.5 + 0.18 * h
        for detune in (-0.12, 0.12):
            w = TAU * (f * h + detune) / RATE
            ph = rnd.random() * TAU
            for i in range(n):
                t = i / RATE
                env = (1 - math.exp(-t / 0.01)) * math.exp(-t / 2.4)
                bloom = 1 + (h / 6) * math.exp(-((t - bloom_peak) ** 2) / 0.5)
                out[i] += 0.5 * amp * env * bloom * math.sin(w * i + ph)
    return out


def tanpura(mix):
    strings = [hz(P_, SA / 2), hz(0, SA), hz(0, SA), hz(0, SA / 2)]
    pans = [-0.35, -0.1, 0.1, 0.35]
    cache = {}
    for bar in range(BARS):
        for k, f in enumerate(strings):
            key = (k, f)
            if key not in cache:
                cache[key] = tanpura_string(f, 3.6)
            mix.add((bar * CYCLE + k) * BEAT, cache[key], 0.045, pans[k])


# ---- Harmonium / shruti pad (wavetable reed tone) -------------------------------
def harmonium(mix):
    table_n = 2048
    table = [sum(math.sin(TAU * h * i / table_n) / h * (0.85 ** h) for h in range(1, 12)) for i in range(table_n)]
    peak = max(abs(v) for v in table)
    table = [v / peak for v in table]
    start, end = 4 * CYCLE * BEAT, 28 * CYCLE * BEAT          # bars 4-27
    n = int((end - start + 2.0) * RATE)
    out = [0.0] * n
    for f, amp in ((hz(0, SA / 2), 0.5), (hz(P_ + 12, SA / 2), 0.35), (hz(0, SA), 0.3)):
        ph = random.random() * table_n
        inc = f * table_n / RATE
        for i in range(n):
            ph += inc * (1 + 0.0015 * math.sin(TAU * 0.3 * i / RATE))   # gentle bellows wobble
            out[i] += amp * table[int(ph) % table_n]
    for i in range(n):
        t = i / RATE
        swell = min(1.0, t / 2.0) * min(1.0, max(0.0, (end - start + 2.0 - t) / 3.0))
        out[i] *= swell * (0.85 + 0.15 * math.sin(TAU * t / (CYCLE * BEAT * 2)))
    mix.add(start, out, 0.05, -0.15)


# ---- Bansuri ----------------------------------------------------------------
def bandpass(signal, f, q):
    w0 = TAU * f / RATE
    alpha = math.sin(w0) / (2 * q)
    a0, a1, a2 = 1 + alpha, -2 * math.cos(w0), 1 - alpha
    x1 = x2 = y1 = y2 = 0.0
    out = []
    for x in signal:
        y = (alpha * x - alpha * x2 - a1 * y1 - a2 * y2) / a0
        x2, x1, y2, y1 = x1, x, y1, y
        out.append(y)
    return out


def bansuri(mix):
    rnd = random.Random(3)
    notes = []   # (start_s, dur_s, freq, vel)
    for bar, phrase in enumerate(melody_bars()):
        sec = section_of(bar)
        vel = {"intro": .62, "groove": .85, "peak": 1.0, "settle": .55}[sec]
        if sec == "settle":
            vel *= 1 - (bar - 24) / 12          # gently softer toward the loop point
        t = bar * CYCLE * BEAT
        for swara, beats in phrase:
            if swara is not None:
                octave_up = 12 if sec == "peak" and swara < S2 and beats <= .5 else 0
                notes.append((t, beats * BEAT, hz(swara + octave_up), vel * rnd.uniform(.92, 1.0)))
            t += beats * BEAT

    n = int((LOOP_S + TAIL_S) * RATE)
    tone = [0.0] * n
    breath_env = [0.0] * n
    phase = 0.0
    cur = notes[-1][2]
    idx = 0
    for i in range(n):
        t = i / RATE
        while idx + 1 < len(notes) and t >= notes[idx + 1][0]:
            idx += 1
        start, dur, target, vel = notes[idx]
        local = t - start
        if local < 0:
            continue
        # Kan (grace note): a quick touch of the note above on longer notes.
        grace = target * 2 ** (2 / 12) if dur > 0.5 and local < 0.045 else None
        goal = grace if grace else target
        glide = 1 - math.exp(-local / 0.03)
        f = cur + (goal - cur) * glide
        if not grace and glide > 0.999:
            cur = target
        vib_depth = 0.004 * min(1.0, max(0.0, (local - 0.25) / 0.3))
        vib = 1 + vib_depth * math.sin(TAU * 5.4 * t)
        phase += TAU * f * vib / RATE
        attack = 1 - math.exp(-local / 0.025)
        release = min(1.0, max(0.0, (dur + 0.12 - local) / 0.12))
        env = attack * release * vel
        tone[i] = env * (math.sin(phase) + 0.18 * math.sin(2 * phase) + 0.06 * math.sin(3 * phase))
        breath_env[i] = env * (0.08 + 0.5 * math.exp(-local / 0.035))
    noise = [rnd.uniform(-1, 1) for _ in range(n)]
    air = bandpass(noise, 2200, 0.8)
    flute = [tone[i] + 0.5 * air[i] * breath_env[i] for i in range(n)]
    mix.add(0, flute, 0.20, 0.05)


# ---- Tabla (keherwa) --------------------------------------------------------
def tabla_stroke(kind, seed):
    rnd = random.Random(seed)
    if kind == "ge":       # bayan: deep, with a little upward gamak
        n = int(0.55 * RATE)
        out, ph = [0.0] * n, 0.0
        for i in range(n):
            t = i / RATE
            f = 88 + 22 * (1 - math.exp(-t / 0.08))
            ph += TAU * f / RATE
            out[i] = (math.sin(ph) + 0.3 * math.sin(2 * ph)) * math.exp(-t / 0.22)
        return out
    if kind in ("na", "tin"):  # dayan, tuned to Sa
        n = int((0.5 if kind == "na" else 0.8) * RATE)
        out = [0.0] * n
        partials = [(1, 1.0, .20), (2, .55, .14), (3, .38, .10), (4, .22, .07), (5, .12, .05)]
        if kind == "tin":
            partials = [(1, 1.0, .40), (2, .45, .22), (3, .25, .12)]
        for mult, amp, dec in partials:
            w = TAU * SA * mult / RATE
            for i in range(n):
                out[i] += amp * math.sin(w * i) * math.exp(-(i / RATE) / dec)
        for i in range(int(0.004 * RATE)):
            out[i] += rnd.uniform(-1, 1) * 0.6 * (1 - i / (0.004 * RATE))
        return out
    # ti / ka: closed, dry strokes
    n = int(0.09 * RATE)
    out, lp = [0.0] * n, 0.0
    for i in range(n):
        t = i / RATE
        lp += 0.35 * (rnd.uniform(-1, 1) - lp)
        out[i] = (lp * 0.8 + 0.4 * math.sin(TAU * SA * 2 * t)) * math.exp(-t / 0.018)
    return out


def tabla(mix):
    strokes = {k: tabla_stroke(k, s) for s, k in enumerate(["ge", "na", "tin", "ti"])}
    bols = {"Dha": ["ge", "na"], "Ge": ["ge"], "Na": ["na"], "Ti": ["ti"], "Ka": ["ti"], "Dhi": ["ge", "tin"],
            "Tin": ["tin"], "-": []}
    keherwa = ["Dha", "Ge", "Na", "Ti", "Na", "Ka", "Dhi", "Na"]
    fill = ["Dha", "Ti", "Dha", "Ti", "Dha", "-", "Dha", "Ti", "Dha", "Ti", "Dha", "-", "Dha", "Ti", "Dha", "Ti"]
    rnd = random.Random(9)
    eighth = BEAT / 2
    for bar in range(BARS):
        sec = section_of(bar)
        if sec == "intro":
            continue
        level = {"groove": .8, "peak": 1.0, "settle": .55}[sec]
        if sec == "settle" and bar >= 30:
            level = 0.3                        # almost gone before the loop point
        t0 = bar * CYCLE * BEAT
        if sec == "peak" and bar in (19, 23) or bar == 15:
            for j, bol in enumerate(fill):     # tihai-style fill, sixteenths
                for s in bols[bol]:
                    mix.add(t0 + j * eighth / 2 + rnd.uniform(-.004, .004), strokes[s], 0.16 * level, -0.1)
            continue
        for j, bol in enumerate(keherwa):
            accent = 1.0 if j in (0, 6) else 0.75
            for s in bols[bol]:
                mix.add(t0 + j * eighth + rnd.uniform(-.004, .004), strokes[s],
                        0.16 * level * accent * rnd.uniform(.9, 1.05), -0.1)


# ---- Manjira and swarmandal ---------------------------------------------------
def manjira(mix):
    n = int(0.8 * RATE)
    ring = [0.0] * n
    for p, a in ((3150, 1.0), (4720, .55), (6300, .35), (8100, .2)):
        w = TAU * p / RATE
        for i in range(n):
            ring[i] += a * math.sin(w * i) * math.exp(-i / RATE / 0.3)
    chik = [ring[i] * math.exp(-i / RATE / 0.04) for i in range(int(0.1 * RATE))]
    for bar in range(4, BARS):
        sec = section_of(bar)
        level = {"groove": .8, "peak": 1.0, "settle": .5}[sec]
        if bar >= 30:
            level = 0.25
        for beat in range(CYCLE):
            t = (bar * CYCLE + beat) * BEAT
            mix.add(t + BEAT / 2, chik, 0.025 * level, 0.4)          # light offbeat
            if beat in (0, 2):
                mix.add(t, ring, 0.035 * level, 0.35)


def swarmandal(mix):
    """Bright harp sweeps up the Hamsadhwani scale at section starts."""
    def pluck(f):
        n = int(1.2 * RATE)
        return [sum(math.sin(TAU * f * h * i / RATE) / h for h in (1, 2, 3)) * math.exp(-(i / RATE) / 0.45)
                for i in range(n)]
    scale = [S, R, G, P, N, S2, R2, G2, P + 12, N + 12, 24]
    cache = {s: pluck(hz(s)) for s in scale}
    for bar, gain in ((0, .035), (4, .04), (16, .05), (24, .03)):
        t = bar * CYCLE * BEAT - 0.35
        for k, s in enumerate(scale):
            mix.add(max(0.0, t + k * 0.035), cache[s], gain, 0.3 - 0.06 * k)


# ---- Reverb -----------------------------------------------------------------
def reverb(l, r, wet=0.24):
    def channel(x, spread):
        out = [0.0] * len(x)
        for d in (int(RATE * d) + spread for d in (0.0297, 0.0371, 0.0411, 0.0437)):
            buf, j, lp = [0.0] * d, 0, 0.0
            for i, v in enumerate(x):
                y = buf[j]
                lp = y * 0.7 + lp * 0.3
                buf[j] = v + lp * 0.8
                j = (j + 1) % d
                out[i] += y
        for d in (int(RATE * 0.005) + spread, int(RATE * 0.0017) + spread):
            buf, j = [0.0] * d, 0
            for i, v in enumerate(out):
                b = buf[j]
                y = -0.7 * v + b
                buf[j] = v + 0.7 * y
                j = (j + 1) % d
                out[i] = y
        return out
    wl, wr = channel(l, 0), channel(r, 23)
    return ([a * (1 - wet) + b * wet * 0.25 for a, b in zip(l, wl)],
            [a * (1 - wet) + b * wet * 0.25 for a, b in zip(r, wr)])


def main():
    random.seed(1)
    mix = Mix(LOOP_S + TAIL_S)
    tanpura(mix)
    harmonium(mix)
    tabla(mix)
    manjira(mix)
    swarmandal(mix)
    bansuri(mix)
    l, r = reverb(mix.l, mix.r)

    n = int(round(LOOP_S * RATE))
    for i in range(n, len(l)):            # wrap tails: seamless loop
        l[i - n] += l[i]
        r[i - n] += r[i]
    l, r = l[:n], r[:n]

    rms = math.sqrt(sum(a * a + b * b for a, b in zip(l, r)) / (2 * n))
    gain = 10 ** ((-16 + 0.7) / 20) / rms

    def limit(v):
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
