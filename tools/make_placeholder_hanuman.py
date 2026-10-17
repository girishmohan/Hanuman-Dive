"""Draws a flat placeholder Hanuman sprite (no dependencies, pure Python).

Same pose and footprint as the original diver pictogram (arms raised, falling),
so it fits the existing player collider. Replace with real art before launch.

    python3 tools/make_placeholder_hanuman.py
"""
import math
import os
import struct
import zlib

W = H = 600
SS = 3  # supersampling per axis

OUTLINE = (74, 37, 17)
FUR = (240, 138, 36)
FUR_DARK = (214, 102, 31)
FACE = (245, 198, 138)
MUZZLE = (233, 168, 106)
GOLD = (232, 178, 28)
GOLD_DARK = (176, 128, 12)
RED = (198, 40, 40)
BLACK = (30, 20, 15)
MACE_HANDLE = (150, 100, 30)


def capsule(a, b, r):
    (ax, ay), (bx, by) = a, b
    dx, dy = bx - ax, by - ay
    ll = dx * dx + dy * dy or 1e-9

    def inside(x, y, g):
        t = max(0.0, min(1.0, ((x - ax) * dx + (y - ay) * dy) / ll))
        px, py = ax + t * dx - x, ay + t * dy - y
        return px * px + py * py <= (r + g) ** 2

    bbox = (min(ax, bx) - r, min(ay, by) - r, max(ax, bx) + r, max(ay, by) + r)
    return inside, bbox


def circle(c, r):
    return capsule(c, c, r)


def ellipse(c, rx, ry):
    cx, cy = c

    def inside(x, y, g):
        return ((x - cx) / (rx + g)) ** 2 + ((y - cy) / (ry + g)) ** 2 <= 1

    return inside, (cx - rx, cy - ry, cx + rx, cy + ry)


def polygon(pts):
    n = len(pts)
    edges = [(pts[i], pts[(i + 1) % n]) for i in range(n)]

    def seg_dist2(x, y, a, b):
        (ax, ay), (bx, by) = a, b
        dx, dy = bx - ax, by - ay
        ll = dx * dx + dy * dy or 1e-9
        t = max(0.0, min(1.0, ((x - ax) * dx + (y - ay) * dy) / ll))
        px, py = ax + t * dx - x, ay + t * dy - y
        return px * px + py * py

    def inside(x, y, g):
        c = False
        for (ax, ay), (bx, by) in edges:
            if (ay > y) != (by > y) and x < (bx - ax) * (y - ay) / (by - ay) + ax:
                c = not c
        if c or g == 0:
            return c
        return any(seg_dist2(x, y, a, b) <= g * g for a, b in edges)

    xs, ys = [p[0] for p in pts], [p[1] for p in pts]
    return inside, (min(xs), min(ys), max(xs), max(ys))


def bezier(p0, p1, p2, steps=16):
    return [((1 - t) ** 2 * p0[0] + 2 * (1 - t) * t * p1[0] + t * t * p2[0],
             (1 - t) ** 2 * p0[1] + 2 * (1 - t) * t * p1[1] + t * t * p2[1])
            for t in (i / steps for i in range(steps + 1))]


def figure():
    """List of (shape, colour) back to front."""
    s = []
    # Tail: curls up and out to the right behind the body.
    tail = bezier((310, 400), (560, 430), (500, 230)) + bezier((500, 230), (470, 170), (530, 160))[1:]
    for a, b in zip(tail, tail[1:]):
        s.append((capsule(a, b, 13), FUR_DARK))
    # Arms raised like the diver.
    s.append((capsule((225, 220), (95, 85), 30), FUR))
    s.append((capsule((375, 220), (505, 85), 30), FUR))
    # Gada (mace) gripped in the right hand, head above it.
    s.append((capsule((496, 150), (516, 40), 9), MACE_HANDLE))
    s.append((circle((518, 36), 32), GOLD))
    s.append((circle((518, 36), 19), GOLD_DARK))
    s.append((circle((518, 36), 9), GOLD))
    s.append((circle((92, 80), 32), FACE))
    s.append((circle((506, 96), 30), FACE))
    # Legs.
    s.append((capsule((272, 430), (228, 555), 30), FUR))
    s.append((capsule((328, 430), (372, 555), 30), FUR))
    s.append((ellipse((222, 565), 34, 20), FACE))
    s.append((ellipse((378, 565), 34, 20), FACE))
    # Torso, dhoti, waist band.
    s.append((polygon([(205, 200), (395, 200), (355, 385), (245, 385)]), FUR))
    s.append((ellipse((300, 285), 40, 50), FACE))  # chest
    s.append((polygon([(242, 370), (358, 370), (390, 460), (300, 440), (210, 460)]), RED))
    s.append((polygon([(238, 360), (362, 360), (360, 385), (240, 385)]), GOLD))
    # Head: fur, ears, face, muzzle, eyes, tilak.
    s.append((circle((228, 140), 20), FACE))
    s.append((circle((372, 140), 20), FACE))
    s.append((circle((300, 132), 72), FUR))
    s.append((ellipse((300, 148), 50, 46), FACE))
    s.append((ellipse((300, 176), 34, 22), MUZZLE))
    s.append((circle((281, 136), 7), BLACK))
    s.append((circle((319, 136), 7), BLACK))
    s.append((capsule((292, 180), (308, 180), 3), BLACK))
    s.append((capsule((300, 104), (300, 120), 3), RED))
    # Crown (mukut).
    s.append((polygon([(238, 92), (248, 42), (274, 70), (300, 6), (326, 70), (352, 42), (362, 92)]), GOLD))
    s.append((circle((300, 72), 9), RED))
    return s


def render(shapes, outline_px=5):
    img = [[(0.0, 0.0, 0.0, 0.0)] * W for _ in range(H)]
    offs = [(i + 0.5) / SS for i in range(SS)]
    layers = [((inside, bbox), OUTLINE, outline_px) for (inside, bbox), _ in shapes]
    layers += [(shape, col, 0) for shape, col in shapes]
    for (inside, (x0, y0, x1, y1)), col, g in layers:
        for y in range(max(0, int(y0 - g - 1)), min(H, int(y1 + g + 2))):
            row = img[y]
            for x in range(max(0, int(x0 - g - 1)), min(W, int(x1 + g + 2))):
                hits = sum(inside(x + ox, y + oy, g) for oy in offs for ox in offs)
                if not hits:
                    continue
                a = hits / (SS * SS)
                r, gg, b, da = row[x]
                na = a + da * (1 - a)
                mix = lambda c, d: (c * a + d * da * (1 - a)) / na
                row[x] = (mix(col[0], r), mix(col[1], gg), mix(col[2], b), na)
    return img


def write_png(path, img):
    raw = bytearray()
    for row in img:
        raw.append(0)
        for r, g, b, a in row:
            raw += bytes((int(r + .5), int(g + .5), int(b + .5), int(a * 255 + .5)))

    def chunk(tag, data):
        c = tag + data
        return struct.pack(">I", len(data)) + c + struct.pack(">I", zlib.crc32(c) & 0xffffffff)

    with open(path, "wb") as f:
        f.write(b"\x89PNG\r\n\x1a\n")
        f.write(chunk(b"IHDR", struct.pack(">IIBBBBB", W, H, 8, 6, 0, 0, 0)))
        f.write(chunk(b"IDAT", zlib.compress(bytes(raw), 9)))
        f.write(chunk(b"IEND", b""))


if __name__ == "__main__":
    out = os.path.join(os.path.dirname(__file__), "..", "hyper dive", "Assets",
                       "A-Z_Materials", "hanuman", "hanuman_placeholder.png")
    os.makedirs(os.path.dirname(out), exist_ok=True)
    write_png(out, render(figure()))
    print("wrote", os.path.normpath(out))
