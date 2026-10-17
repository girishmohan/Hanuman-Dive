// Draws the rakshasa enemies and the Ramayana backdrop silhouettes.
//   swift tools/make_ramayana_sprites.swift
// Rakshasas: 512x512, bold outline, matching the Muse Hanuman's cartoon style.
// Backdrop: white silhouettes, tinted in Unity per chapter (ramayanaBackdrop.cs).
import AppKit
import Foundation

let root = URL(fileURLWithPath: CommandLine.arguments[0]).deletingLastPathComponent().deletingLastPathComponent()
let outDir = root.appendingPathComponent("hyper dive/Assets/A-Z_Materials/ramayana")
try? FileManager.default.createDirectory(at: outDir, withIntermediateDirectories: true)

let INK = NSColor(red: 0.16, green: 0.07, blue: 0.03, alpha: 1)

func render(_ name: String, _ w: Int, _ h: Int, _ draw: () -> Void) {
    let rep = NSBitmapImageRep(bitmapDataPlanes: nil, pixelsWide: w, pixelsHigh: h, bitsPerSample: 8, samplesPerPixel: 4,
                               hasAlpha: true, isPlanar: false, colorSpaceName: .deviceRGB, bytesPerRow: 0, bitsPerPixel: 0)!
    NSGraphicsContext.current = NSGraphicsContext(bitmapImageRep: rep)
    NSGraphicsContext.current!.cgContext.clear(CGRect(x: 0, y: 0, width: w, height: h))
    draw()
    NSGraphicsContext.current!.flushGraphics()
    try! rep.representation(using: .png, properties: [:])!.write(to: outDir.appendingPathComponent(name))
    print("wrote", name)
}

func shape(_ p: NSBezierPath, _ fill: NSColor, line: CGFloat = 10) {
    fill.setFill(); p.fill()
    if line > 0 { INK.setStroke(); p.lineWidth = line; p.lineJoinStyle = .round; p.lineCapStyle = .round; p.stroke() }
}
func oval(_ x: CGFloat, _ y: CGFloat, _ w: CGFloat, _ h: CGFloat) -> NSBezierPath { NSBezierPath(ovalIn: NSRect(x: x - w / 2, y: y - h / 2, width: w, height: h)) }
func poly(_ pts: [(CGFloat, CGFloat)]) -> NSBezierPath {
    let p = NSBezierPath(); p.move(to: NSPoint(x: pts[0].0, y: pts[0].1))
    for q in pts.dropFirst() { p.line(to: NSPoint(x: q.0, y: q.1)) }
    p.close(); return p
}
func curve(_ a: (CGFloat, CGFloat), _ c1: (CGFloat, CGFloat), _ c2: (CGFloat, CGFloat), _ b: (CGFloat, CGFloat)) -> NSBezierPath {
    let p = NSBezierPath(); p.move(to: NSPoint(x: a.0, y: a.1))
    p.curve(to: NSPoint(x: b.0, y: b.1), controlPoint1: NSPoint(x: c1.0, y: c1.1), controlPoint2: NSPoint(x: c2.0, y: c2.1))
    return p
}
func limb(_ a: (CGFloat, CGFloat), _ b: (CGFloat, CGFloat), _ r: CGFloat, _ c: NSColor) {
    let p = NSBezierPath(); p.move(to: NSPoint(x: a.0, y: a.1)); p.line(to: NSPoint(x: b.0, y: b.1))
    p.lineCapStyle = .round
    INK.setStroke(); p.lineWidth = r * 2 + 10; p.stroke()
    c.setStroke(); p.lineWidth = r * 2; p.stroke()
}

// MARK: Rakshasa
func rakshasa(_ name: String, skin: NSColor, skinDark: NSColor, cloth: NSColor) {
    render(name, 512, 512) {
        let gold = NSColor(red: 0.98, green: 0.76, blue: 0.18, alpha: 1)
        let ivory = NSColor(red: 0.96, green: 0.91, blue: 0.78, alpha: 1)
        // Spiked club in the right hand (behind the arm).
        limb((372, 330), (452, 470), 9, NSColor(red: 0.45, green: 0.28, blue: 0.14, alpha: 1))
        shape(oval(455, 462, 70, 82), NSColor(red: 0.42, green: 0.40, blue: 0.40, alpha: 1))
        for (dx, dy) in [(-38, 0), (38, 0), (0, 44), (-28, 30), (28, 30)] as [(CGFloat, CGFloat)] {
            shape(poly([(455 + dx * 0.7, 462 + dy * 0.7 - 6), (455 + dx * 1.25, 462 + dy * 1.25), (455 + dx * 0.7 + 6, 462 + dy * 0.7)]), ivory, line: 5)
        }
        // Legs and feet.
        limb((222, 150), (196, 52), 26, skinDark)
        limb((290, 150), (318, 52), 26, skinDark)
        shape(oval(186, 44, 64, 30), skinDark)
        shape(oval(328, 44, 64, 30), skinDark)
        // Arms raised, menacing.
        limb((178, 300), (78, 380), 26, skin)
        limb((334, 300), (380, 332), 26, skin)
        // Clawed hands.
        shape(oval(70, 392, 58, 58), skin)
        for k in 0..<3 { shape(poly([(48 + CGFloat(k) * 18, 414), (52 + CGFloat(k) * 18, 446), (62 + CGFloat(k) * 18, 416)]), ivory, line: 5) }
        shape(oval(384, 338, 56, 56), skin)
        // Torso, belly, dhoti, belt.
        shape(poly([(160, 320), (352, 320), (330, 170), (182, 170)]), skin)
        shape(oval(256, 232, 120, 110), skinDark, line: 0)
        shape(poly([(176, 190), (336, 190), (360, 112), (256, 128), (152, 112)]), cloth)
        shape(poly([(174, 186), (338, 186), (336, 206), (176, 206)]), gold, line: 6)
        shape(oval(256, 196, 26, 26), NSColor(red: 0.85, green: 0.1, blue: 0.1, alpha: 1), line: 5)
        // Skull necklace.
        for k in 0..<5 {
            let x = 196 + CGFloat(k) * 30, y = 300 - abs(CGFloat(k) - 2) * 8
            shape(oval(x, y, 22, 22), ivory, line: 5)
        }
        // Wild hair behind the head.
        shape(poly([(150, 380), (130, 470), (190, 440), (210, 500), (256, 455), (302, 500), (322, 440), (382, 470), (362, 380)]),
              NSColor(red: 0.12, green: 0.08, blue: 0.10, alpha: 1))
        // Horns.
        let lh = curve((196, 432), (150, 470), (132, 500), (150, 506)); lh.line(to: NSPoint(x: 222, y: 446)); lh.close()
        let rh = curve((316, 432), (362, 470), (380, 500), (362, 506)); rh.line(to: NSPoint(x: 290, y: 446)); rh.close()
        shape(lh, ivory); shape(rh, ivory)
        // Head and ears.
        shape(oval(160, 368, 40, 56), skin)
        shape(oval(352, 368, 40, 56), skin)
        shape(oval(256, 368, 196, 180), skin)
        // Crown.
        shape(poly([(190, 432), (322, 432), (330, 470), (300, 452), (280, 490), (256, 458), (232, 490), (212, 452), (182, 470)]), gold, line: 7)
        shape(oval(256, 444, 20, 20), NSColor(red: 0.2, green: 0.75, blue: 0.4, alpha: 1), line: 4)
        // Angry eyes and brows.
        for side in [-1.0, 1.0] as [CGFloat] {
            shape(oval(256 + side * 44, 384, 56, 40), NSColor.white, line: 7)
            shape(oval(256 + side * 40, 382, 24, 24), NSColor(red: 0.85, green: 0.08, blue: 0.08, alpha: 1), line: 0)
            shape(oval(256 + side * 40, 382, 10, 12), INK, line: 0)
            shape(poly([(256 + side * 14, 404), (256 + side * 78, 424), (256 + side * 80, 410), (256 + side * 16, 392)]), INK, line: 0)
        }
        // Nose, moustache, grin with fangs.
        shape(oval(256, 352, 38, 26), skinDark, line: 6)
        shape(oval(256, 300, 120, 50), NSColor(red: 0.45, green: 0.05, blue: 0.08, alpha: 1), line: 8)
        shape(poly([(212, 296), (226, 330), (238, 298)]), ivory, line: 5)
        shape(poly([(274, 298), (286, 330), (300, 296)]), ivory, line: 5)
        let must = curve((256, 334), (220, 350), (180, 330), (176, 304)); must.curve(to: NSPoint(x: 256, y: 326), controlPoint1: NSPoint(x: 196, y: 330), controlPoint2: NSPoint(x: 226, y: 338))
        must.curve(to: NSPoint(x: 336, y: 304), controlPoint1: NSPoint(x: 286, y: 338), controlPoint2: NSPoint(x: 316, y: 330))
        must.curve(to: NSPoint(x: 256, y: 334), controlPoint1: NSPoint(x: 332, y: 330), controlPoint2: NSPoint(x: 292, y: 350))
        shape(must, NSColor(red: 0.10, green: 0.07, blue: 0.08, alpha: 1), line: 4)
    }
}

rakshasa("rakshasa_blue.png", skin: NSColor(red: 0.27, green: 0.47, blue: 0.86, alpha: 1),
         skinDark: NSColor(red: 0.20, green: 0.34, blue: 0.68, alpha: 1), cloth: NSColor(red: 0.55, green: 0.10, blue: 0.12, alpha: 1))
rakshasa("rakshasa_green.png", skin: NSColor(red: 0.30, green: 0.62, blue: 0.36, alpha: 1),
         skinDark: NSColor(red: 0.22, green: 0.46, blue: 0.27, alpha: 1), cloth: NSColor(red: 0.35, green: 0.12, blue: 0.45, alpha: 1))

// MARK: Backdrop silhouettes (white, soft-edged look comes from Unity alpha)
let W = NSColor.white
func silhouette(_ name: String, _ w: Int, _ h: Int, _ body: () -> Void) {
    render(name, w, h) { W.setFill(); body() }
}

silhouette("bd_gopuram.png", 256, 512) {
    // Stepped temple tower with a kalasha finial.
    var y: CGFloat = 0, half: CGFloat = 118
    for tier in 0..<7 {
        let hgt: CGFloat = tier == 0 ? 90 : 52
        poly([(128 - half, y), (128 + half, y), (128 + half - 10, y + hgt), (128 - half + 10, y + hgt)]).fill()
        y += hgt; half -= 13
    }
    NSBezierPath(roundedRect: NSRect(x: 128 - 60, y: y, width: 120, height: 34), xRadius: 16, yRadius: 16).fill()
    oval(128, y + 52, 30, 34).fill(); poly([(122, y + 66), (134, y + 66), (128, y + 96)]).fill()
}
silhouette("bd_lanka_tower.png", 256, 512) {
    NSBezierPath(rect: NSRect(x: 70, y: 0, width: 116, height: 300)).fill()
    for k in 0..<4 { NSBezierPath(rect: NSRect(x: 62 + CGFloat(k) * 36, y: 300, width: 22, height: 26)).fill() }
    NSBezierPath(rect: NSRect(x: 92, y: 320, width: 72, height: 80)).fill()
    oval(128, 420, 96, 80).fill()
    poly([(122, 452), (134, 452), (128, 500)]).fill()
    poly([(128, 500), (128, 470), (170, 488)]).fill()
}
silhouette("bd_palm.png", 256, 512) {
    let trunk = curve((120, 0), (110, 160), (160, 300), (140, 390)); trunk.line(to: NSPoint(x: 156, y: 392))
    trunk.curve(to: NSPoint(x: 140, y: 0), controlPoint1: NSPoint(x: 180, y: 300), controlPoint2: NSPoint(x: 130, y: 160)); trunk.close(); trunk.fill()
    for a in stride(from: 0.0, to: 360.0, by: 52.0) {
        let r = a * .pi / 180
        let tip = (148 + CGFloat(cos(r)) * 120, 396 + CGFloat(sin(r)) * 60 - 30)
        let leaf = curve((148, 396), (148 + CGFloat(cos(r)) * 60, 440), (tip.0, tip.1 + 40), tip)
        leaf.curve(to: NSPoint(x: 148, y: 396), controlPoint1: NSPoint(x: tip.0, y: tip.1 + 10), controlPoint2: NSPoint(x: 148 + CGFloat(cos(r)) * 50, y: 410))
        leaf.fill()
    }
}
silhouette("bd_cloud.png", 512, 256) {
    for (x, y, r) in [(120, 90, 70), (200, 130, 90), (300, 120, 80), (380, 90, 66), (250, 80, 80)] as [(CGFloat, CGFloat, CGFloat)] { oval(x, y, r * 2, r * 2).fill() }
    NSBezierPath(rect: NSRect(x: 60, y: 30, width: 380, height: 70)).fill()
}
silhouette("bd_lotus.png", 256, 256) {
    for a in stride(from: -60.0, through: 60.0, by: 30.0) {
        let t = NSAffineTransform(); t.translateX(by: 128, yBy: 60); t.rotate(byDegrees: CGFloat(a))
        let petal = oval(0, 70, 54, 150); petal.transform(using: t as AffineTransform); petal.fill()
    }
    oval(128, 50, 200, 50).fill()
}
silhouette("bd_mountain.png", 512, 256) {
    poly([(0, 0), (120, 150), (180, 110), (290, 240), (380, 140), (430, 170), (512, 60), (512, 0)]).fill()
}

// MARK: Chapter scenery (added for chapter-themed backdrops)
silhouette("bd_wave.png", 512, 256) {
    let p = NSBezierPath(); p.move(to: NSPoint(x: 0, y: 0)); p.line(to: NSPoint(x: 0, y: 90))
    for k in 0..<4 {
        let x0 = CGFloat(k) * 128
        p.curve(to: NSPoint(x: x0 + 128, y: 90), controlPoint1: NSPoint(x: x0 + 30, y: 190), controlPoint2: NSPoint(x: x0 + 110, y: 170))
    }
    p.line(to: NSPoint(x: 512, y: 0)); p.close(); p.fill()
}
silhouette("bd_ashoka_tree.png", 384, 512) {
    poly([(176, 0), (208, 0), (204, 220), (180, 220)]).fill()
    for (x, y, r) in [(192, 300, 110), (110, 260, 80), (274, 260, 80), (150, 380, 85), (240, 390, 85), (192, 450, 70)] as [(CGFloat, CGFloat, CGFloat)] { oval(x, y, r * 2, r * 1.7).fill() }
}
silhouette("bd_flame.png", 256, 512) {
    let f = curve((128, 0), (20, 60), (40, 260), (110, 330)); f.curve(to: NSPoint(x: 128, y: 500), controlPoint1: NSPoint(x: 90, y: 400), controlPoint2: NSPoint(x: 120, y: 450))
    f.curve(to: NSPoint(x: 190, y: 300), controlPoint1: NSPoint(x: 160, y: 420), controlPoint2: NSPoint(x: 210, y: 380))
    f.curve(to: NSPoint(x: 128, y: 0), controlPoint1: NSPoint(x: 240, y: 200), controlPoint2: NSPoint(x: 230, y: 40)); f.close(); f.fill()
}
silhouette("bd_herb.png", 384, 512) {
    poly([(0, 0), (90, 220), (150, 170), (230, 360), (300, 230), (384, 300), (384, 0)]).fill()
    for (x, y) in [(150, 250), (230, 420), (300, 300)] as [(CGFloat, CGFloat)] {
        for a in [-40.0, 0.0, 40.0] {
            let t = NSAffineTransform(); t.translateX(by: x, yBy: y); t.rotate(byDegrees: CGFloat(a))
            let leaf = oval(0, 36, 26, 70); leaf.transform(using: t as AffineTransform); leaf.fill()
        }
    }
}
silhouette("bd_diya.png", 256, 256) {
    let bowl = curve((30, 90), (60, 10), (196, 10), (226, 90)); bowl.close(); bowl.fill()
    let flame = curve((128, 90), (98, 140), (118, 190), (128, 230)); flame.curve(to: NSPoint(x: 128, y: 90), controlPoint1: NSPoint(x: 140, y: 190), controlPoint2: NSPoint(x: 158, y: 140)); flame.fill()
}
silhouette("bd_flag.png", 256, 512) {
    NSBezierPath(rect: NSRect(x: 60, y: 0, width: 14, height: 500)).fill()
    poly([(74, 500), (230, 450), (74, 380)]).fill()
    poly([(74, 420), (200, 380), (74, 330)]).fill()
}
silhouette("bd_bell.png", 256, 384) {
    NSBezierPath(rect: NSRect(x: 120, y: 260, width: 16, height: 120)).fill()
    let b = curve((40, 60), (60, 200), (90, 270), (128, 270)); b.curve(to: NSPoint(x: 216, y: 60), controlPoint1: NSPoint(x: 166, y: 270), controlPoint2: NSPoint(x: 196, y: 200)); b.close(); b.fill()
    NSBezierPath(rect: NSRect(x: 26, y: 40, width: 204, height: 24)).fill()
    oval(128, 26, 34, 34).fill()
}

// MARK: Milestone icons (256x256, coloured, outlined)
func icon(_ name: String, _ body: () -> Void) {
    render(name, 256, 256) {
        // Soft golden halo so pickups read as special.
        let halo = NSGradient(colors: [NSColor(red: 1, green: 0.9, blue: 0.5, alpha: 0.85), NSColor(red: 1, green: 0.8, blue: 0.3, alpha: 0)])!
        halo.draw(in: NSBezierPath(ovalIn: NSRect(x: 4, y: 4, width: 248, height: 248)), relativeCenterPosition: .zero)
        body()
    }
}
let GOLD = NSColor(red: 0.98, green: 0.78, blue: 0.20, alpha: 1)
let GOLD_D = NSColor(red: 0.80, green: 0.55, blue: 0.10, alpha: 1)
icon("ms_ring.png") {
    let ring = NSBezierPath(ovalIn: NSRect(x: 68, y: 50, width: 120, height: 120))
    ring.appendOval(in: NSRect(x: 92, y: 74, width: 72, height: 72)); ring.windingRule = .evenOdd
    shape(ring, GOLD, line: 8)
    shape(poly([(98, 168), (158, 168), (174, 196), (128, 230), (82, 196)]), NSColor(red: 0.4, green: 0.85, blue: 1, alpha: 1), line: 7)
    shape(poly([(98, 168), (158, 168), (128, 140)]), GOLD_D, line: 6)
}
icon("ms_mainaka.png") {
    shape(poly([(40, 60), (110, 200), (140, 160), (216, 60)]), NSColor(red: 0.35, green: 0.70, blue: 0.40, alpha: 1))
    shape(poly([(110, 200), (90, 160), (128, 168)]), NSColor.white, line: 5)
    shape(oval(128, 214, 30, 30), GOLD, line: 6)
}
icon("ms_gate.png") {
    let arch = NSBezierPath(); arch.move(to: NSPoint(x: 50, y: 40)); arch.line(to: NSPoint(x: 50, y: 150))
    arch.curve(to: NSPoint(x: 206, y: 150), controlPoint1: NSPoint(x: 60, y: 230), controlPoint2: NSPoint(x: 196, y: 230))
    arch.line(to: NSPoint(x: 206, y: 40)); arch.close()
    arch.appendRect(NSRect(x: 92, y: 40, width: 72, height: 100)); arch.windingRule = .evenOdd
    shape(arch, GOLD, line: 8)
    shape(oval(128, 196, 22, 22), NSColor(red: 0.85, green: 0.1, blue: 0.1, alpha: 1), line: 5)
}
icon("ms_ring_to_sita.png") {
    for a in stride(from: -60.0, through: 60.0, by: 30.0) {
        let t = NSAffineTransform(); t.translateX(by: 128, yBy: 60); t.rotate(byDegrees: CGFloat(a))
        let petal = oval(0, 56, 46, 110); petal.transform(using: t as AffineTransform)
        shape(petal, NSColor(red: 1, green: 0.62, blue: 0.75, alpha: 1), line: 6)
    }
    let ring = NSBezierPath(ovalIn: NSRect(x: 92, y: 120, width: 72, height: 72))
    ring.appendOval(in: NSRect(x: 108, y: 136, width: 40, height: 40)); ring.windingRule = .evenOdd
    shape(ring, GOLD, line: 6)
}
icon("ms_gada.png") {
    limb((70, 50), (150, 150), 10, NSColor(red: 0.55, green: 0.35, blue: 0.15, alpha: 1))
    shape(oval(170, 172, 90, 100), GOLD, line: 8)
    for k in 0..<3 { shape(oval(150 + CGFloat(k) * 20, 172 + CGFloat(k % 2) * 14, 10, 10), GOLD_D, line: 0) }
    shape(poly([(160, 218), (182, 218), (171, 244)]), GOLD, line: 6)
}
icon("ms_fire.png") {
    limb((128, 30), (128, 110), 12, NSColor(red: 0.55, green: 0.35, blue: 0.15, alpha: 1))
    let f = curve((128, 110), (60, 140), (80, 200), (128, 236)); f.curve(to: NSPoint(x: 128, y: 110), controlPoint1: NSPoint(x: 176, y: 200), controlPoint2: NSPoint(x: 196, y: 140))
    shape(f, NSColor(red: 1, green: 0.45, blue: 0.1, alpha: 1), line: 7)
    let inner = curve((128, 118), (100, 140), (106, 180), (128, 200)); inner.curve(to: NSPoint(x: 128, y: 118), controlPoint1: NSPoint(x: 150, y: 180), controlPoint2: NSPoint(x: 156, y: 140))
    shape(inner, NSColor(red: 1, green: 0.85, blue: 0.3, alpha: 1), line: 0)
}
icon("ms_sanjeevani.png") {
    shape(poly([(40, 50), (100, 170), (150, 120), (216, 50)]), NSColor(red: 0.45, green: 0.40, blue: 0.35, alpha: 1))
    for (x, y, a) in [(110, 190, -30.0), (140, 205, 0.0), (170, 190, 30.0)] as [(CGFloat, CGFloat, Double)] {
        let t = NSAffineTransform(); t.translateX(by: x, yBy: y - 30); t.rotate(byDegrees: CGFloat(a))
        let leaf = oval(0, 30, 30, 70); leaf.transform(using: t as AffineTransform)
        shape(leaf, NSColor(red: 0.35, green: 0.95, blue: 0.45, alpha: 1), line: 6)
    }
}
icon("ms_bell.png") {
    limb((128, 200), (128, 240), 6, GOLD_D)
    let b = curve((56, 70), (70, 170), (96, 210), (128, 210)); b.curve(to: NSPoint(x: 200, y: 70), controlPoint1: NSPoint(x: 160, y: 210), controlPoint2: NSPoint(x: 186, y: 170)); b.close()
    shape(b, GOLD, line: 8)
    shape(NSBezierPath(roundedRect: NSRect(x: 44, y: 56, width: 168, height: 22), xRadius: 8, yRadius: 8), GOLD_D, line: 6)
    shape(oval(128, 42, 30, 30), GOLD, line: 6)
}
