// Redraws the game's text images in Baloo 2 (smooth, rounded) at their original
// pixel sizes, so they drop into the existing UI layout unchanged. Also draws the
// "RAM" badge that replaces the oxygen tank.
//
//   swift tools/render_ui_text.swift
//
// Edit the LABELS table below to change wording or colours.
import AppKit
import CoreText
import Foundation

let root = URL(fileURLWithPath: CommandLine.arguments[0]).deletingLastPathComponent().deletingLastPathComponent()
let assets = root.appendingPathComponent("hyper dive/Assets")
let fontURL = assets.appendingPathComponent("A-Z_Materials/fonts/Baloo2/Baloo2-Variable.ttf")

struct Label {
    let file: String
    let text: String
    let top: NSColor      // gradient top
    let bottom: NSColor   // gradient bottom
    var fill: CGFloat = 0.86 // fraction of the canvas the text may use
}

let white = (NSColor(white: 1, alpha: 1), NSColor(red: 0.86, green: 0.84, blue: 0.80, alpha: 1))
let red = (NSColor(red: 0.98, green: 0.30, blue: 0.22, alpha: 1), NSColor(red: 0.70, green: 0.08, blue: 0.06, alpha: 1))
let saffron = (NSColor(red: 1.0, green: 0.68, blue: 0.20, alpha: 1), NSColor(red: 0.93, green: 0.40, blue: 0.08, alpha: 1))
let gold = (NSColor(red: 1.0, green: 0.88, blue: 0.40, alpha: 1), NSColor(red: 0.90, green: 0.62, blue: 0.10, alpha: 1))

let LABELS: [Label] = [
    Label(file: "startStuff/meters.png", text: "METERS", top: white.0, bottom: white.1),
    Label(file: "startStuff/gameOver 1.png", text: "GAME\nOVER", top: red.0, bottom: red.1),
    Label(file: "startStuff/resume.png", text: "RESUME", top: red.0, bottom: red.1),
    Label(file: "startStuff/resumeByImage.png", text: "RESUME", top: red.0, bottom: red.1),
    Label(file: "startStuff/playagain.png", text: "PLAY AGAIN", top: white.0, bottom: white.1),
    Label(file: "startStuff/quit.png", text: "QUIT", top: white.0, bottom: white.1),
    Label(file: "startStuff/restart.png", text: "RESTART", top: white.0, bottom: white.1),
    Label(file: "startStuff/start.png", text: "START", top: white.0, bottom: white.1),
    Label(file: "startStuff/jumpsuitsImgText.png", text: "AVATAR", top: saffron.0, bottom: saffron.1),
    Label(file: "startStuff/lastbest.png", text: "LAST BEST", top: white.0, bottom: white.1),
    Label(file: "start3Anim/Last Best 2.png", text: "LAST BEST", top: white.0, bottom: white.1),
    Label(file: "startStuff/loading.png", text: "LOADING", top: white.0, bottom: white.1),
    Label(file: "startStuff/levels.png", text: "LEVELS", top: white.0, bottom: white.1),
    Label(file: "startStuff/expert.png", text: "EXPERT", top: white.0, bottom: white.1),
    Label(file: "startStuff/610 - Crust.png", text: "610 · CRUST", top: white.0, bottom: white.1),
    Label(file: "startStuff/1520 - Seismic.png", text: "1520 · SEISMIC", top: white.0, bottom: white.1),
    Label(file: "startStuff/3330 - core.png", text: "3330 · CORE", top: white.0, bottom: white.1),
    Label(file: "startStuff/5110 - Abyss.png", text: "5110 · ABYSS", top: white.0, bottom: white.1),
    Label(file: "startStuff/6000 + INFINITY.png", text: "6000 + INFINITY", top: white.0, bottom: white.1),
    Label(file: "startStuff/3.png", text: "3", top: saffron.0, bottom: saffron.1, fill: 0.9),
    Label(file: "startStuff/2.png", text: "2", top: saffron.0, bottom: saffron.1, fill: 0.9),
    Label(file: "startStuff/1.png", text: "1", top: saffron.0, bottom: saffron.1, fill: 0.9),
    Label(file: "startStuff/go.png", text: "JAI\nSHRI RAM", top: gold.0, bottom: gold.1, fill: 0.9),
]

func baloo(_ size: CGFloat, weight: CGFloat = 800) -> CTFont {
    let descs = CTFontManagerCreateFontDescriptorsFromURL(fontURL as CFURL) as! [CTFontDescriptor]
    let wght = NSNumber(value: 0x7767_6874) // 'wght'
    let desc = CTFontDescriptorCreateCopyWithAttributes(descs[0],
        [kCTFontVariationAttribute: [wght: weight]] as CFDictionary)
    return CTFontCreateWithFontDescriptor(desc, size, nil)
}

func line(_ s: String, _ font: CTFont) -> CTLine {
    CTLineCreateWithAttributedString(NSAttributedString(string: s, attributes: [.font: font]))
}

func canvas(_ w: Int, _ h: Int, _ draw: (CGContext) -> Void) -> Data {
    let rep = NSBitmapImageRep(bitmapDataPlanes: nil, pixelsWide: w, pixelsHigh: h, bitsPerSample: 8,
        samplesPerPixel: 4, hasAlpha: true, isPlanar: false, colorSpaceName: .deviceRGB, bytesPerRow: 0, bitsPerPixel: 0)!
    let ctx = NSGraphicsContext(bitmapImageRep: rep)!
    NSGraphicsContext.current = ctx
    let cg = ctx.cgContext
    cg.clear(CGRect(x: 0, y: 0, width: w, height: h))
    cg.setShouldAntialias(true)
    draw(cg)
    ctx.flushGraphics()
    return rep.representation(using: .png, properties: [:])!
}

/// Outlined, gradient-filled, shadowed text centred in the canvas.
func drawText(_ cg: CGContext, _ text: String, w: CGFloat, h: CGFloat, top: NSColor, bottom: NSColor, fill: CGFloat) {
    let lines = text.components(separatedBy: "\n")
    // Size so the widest line and the stacked height both fit.
    var size: CGFloat = h
    func metrics(_ s: CGFloat) -> (CGFloat, CGFloat) {
        let f = baloo(s)
        let widest = lines.map { CGFloat(CTLineGetTypographicBounds(line($0, f), nil, nil, nil)) }.max()!
        let lh = (CTFontGetAscent(f) + CTFontGetDescent(f)) * 0.82
        return (widest + s * 0.16, lh * CGFloat(lines.count) + s * 0.16)
    }
    while true {
        let (mw, mh) = metrics(size)
        if mw <= w * fill && mh <= h * fill { break }
        size *= 0.96
    }
    let font = baloo(size)
    let lh = (CTFontGetAscent(font) + CTFontGetDescent(font)) * 0.82
    let blockH = lh * CGFloat(lines.count)
    let stroke = size * 0.13

    for pass in 0..<2 {
        for (i, s) in lines.enumerated() {
            let l = line(s, font)
            let lw = CGFloat(CTLineGetTypographicBounds(l, nil, nil, nil))
            let x = (w - lw) / 2
            let baseline = (h + blockH) / 2 - lh * CGFloat(i + 1) + (lh - CTFontGetAscent(font) * 0.82) / 2 + CTFontGetDescent(font) * 0.35
            cg.saveGState()
            cg.textPosition = CGPoint(x: x, y: baseline)
            if pass == 0 {
                // Dark rounded outline with a soft drop shadow.
                cg.setShadow(offset: CGSize(width: 0, height: -stroke * 0.6), blur: stroke * 0.8,
                             color: NSColor(white: 0, alpha: 0.55).cgColor)
                cg.setLineJoin(.round)
                cg.setLineWidth(stroke * 2)
                cg.setStrokeColor(NSColor(red: 0.13, green: 0.06, blue: 0.03, alpha: 1).cgColor)
                cg.setTextDrawingMode(.stroke)
                CTLineDraw(l, cg)
            } else {
                // Gradient fill clipped to the glyphs.
                cg.setTextDrawingMode(.clip)
                CTLineDraw(l, cg)
                let g = CGGradient(colorsSpace: CGColorSpaceCreateDeviceRGB(),
                                   colors: [top.cgColor, bottom.cgColor] as CFArray, locations: [0, 1])!
                cg.drawLinearGradient(g, start: CGPoint(x: 0, y: baseline + size * 0.75),
                                      end: CGPoint(x: 0, y: baseline - size * 0.05), options: [.drawsBeforeStartLocation, .drawsAfterEndLocation])
            }
            cg.restoreGState()
        }
    }
}

func pixelSize(_ url: URL) -> (Int, Int) {
    let rep = NSImageRep(contentsOf: url)!
    return (rep.pixelsWide, rep.pixelsHigh)
}

// --- Text labels ---
for label in LABELS {
    let url = assets.appendingPathComponent("A-Z_Materials/" + label.file)
    let (w, h) = pixelSize(url)
    let png = canvas(w, h) { cg in
        drawText(cg, label.text, w: CGFloat(w), h: CGFloat(h), top: label.top, bottom: label.bottom, fill: label.fill)
    }
    try! png.write(to: url)
    print("label", label.file, "\(w)x\(h)")
}

// --- RAM badge (replaces the oxygen tank, same 873x1974 footprint) ---
do {
    let url = assets.appendingPathComponent("A-Z_Materials/o2/02_5.png")
    let (w, h) = pixelSize(url)
    let W = CGFloat(w), H = CGFloat(h)
    let png = canvas(w, h) { cg in
        let inset = W * 0.06
        let rect = CGRect(x: inset, y: inset, width: W - inset * 2, height: H - inset * 2)
        let path = CGPath(roundedRect: rect, cornerWidth: W * 0.32, cornerHeight: W * 0.32, transform: nil)
        // Dark rim, gold border, saffron body.
        cg.addPath(path); cg.setFillColor(NSColor(red: 0.30, green: 0.12, blue: 0.03, alpha: 1).cgColor); cg.fillPath()
        let goldPath = CGPath(roundedRect: rect.insetBy(dx: W * 0.05, dy: W * 0.05), cornerWidth: W * 0.28, cornerHeight: W * 0.28, transform: nil)
        cg.addPath(goldPath); cg.setFillColor(NSColor(red: 0.98, green: 0.78, blue: 0.22, alpha: 1).cgColor); cg.fillPath()
        let body = rect.insetBy(dx: W * 0.11, dy: W * 0.11)
        let bodyPath = CGPath(roundedRect: body, cornerWidth: W * 0.22, cornerHeight: W * 0.22, transform: nil)
        cg.saveGState()
        cg.addPath(bodyPath); cg.clip()
        let g = CGGradient(colorsSpace: CGColorSpaceCreateDeviceRGB(),
            colors: [NSColor(red: 1.0, green: 0.55, blue: 0.12, alpha: 1).cgColor,
                     NSColor(red: 0.85, green: 0.25, blue: 0.05, alpha: 1).cgColor] as CFArray, locations: [0, 1])!
        cg.drawLinearGradient(g, start: CGPoint(x: 0, y: body.maxY), end: CGPoint(x: 0, y: body.minY), options: [])
        cg.restoreGState()
        // R, A, M stacked.
        let letters = ["R", "A", "M"]
        let cell = body.height / 3
        for (i, s) in letters.enumerated() {
            let cellRect = CGRect(x: body.minX, y: body.maxY - cell * CGFloat(i + 1), width: body.width, height: cell)
            cg.saveGState()
            cg.translateBy(x: cellRect.minX, y: cellRect.minY)
            drawText(cg, s, w: cellRect.width, h: cellRect.height, top: gold.0, bottom: NSColor.white, fill: 0.92)
            cg.restoreGState()
        }
    }
    try! png.write(to: url)
    print("badge o2/02_5.png \(w)x\(h)")
}
