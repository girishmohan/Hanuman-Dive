// Repaints the price on each AVATAR shop card (storUI/Group 36-43.png) in Baloo 2,
// keeping the original card art. Prices must match shopManager.cs.
//   swift tools/render_shop_prices.swift
import AppKit
import CoreText

let root = URL(fileURLWithPath: CommandLine.arguments[0]).deletingLastPathComponent().deletingLastPathComponent()
let mat = root.appendingPathComponent("hyper dive/Assets/A-Z_Materials")
let fontURL = mat.appendingPathComponent("fonts/Baloo2/Baloo2-Variable.ttf")

// Card image -> new price (card for the 2500 skin is Group 37, and so on).
let cards: [(String, String)] = [
    ("Group 37", "500"), ("Group 41", "1,500"), ("Group 42", "3,000"), ("Group 43", "6,000"),
    ("Group 36", "12,000"), ("Group 39", "25,000"), ("Group 38", "50,000"), ("Group 40", "100,000"),
]

let descs = CTFontManagerCreateFontDescriptorsFromURL(fontURL as CFURL) as! [CTFontDescriptor]
let font = CTFontCreateWithFontDescriptor(
    CTFontDescriptorCreateCopyWithAttributes(descs[0], [kCTFontVariationAttribute: [NSNumber(value: 0x7767_6874): 700]] as CFDictionary),
    170, nil)
let gem = NSImage(contentsOf: mat.deletingLastPathComponent().appendingPathComponent("prefab/gemPrefab/icons8-diamond.png"))!

for (file, price) in cards {
    let url = mat.appendingPathComponent("storUI/\(file).png")
    let src = NSBitmapImageRep(data: try! Data(contentsOf: url))!
    let w = src.pixelsWide, h = src.pixelsHigh
    let rep = NSBitmapImageRep(bitmapDataPlanes: nil, pixelsWide: w, pixelsHigh: h, bitsPerSample: 8, samplesPerPixel: 4,
                               hasAlpha: true, isPlanar: false, colorSpaceName: .deviceRGB, bytesPerRow: 0, bitsPerPixel: 0)!
    NSGraphicsContext.current = NSGraphicsContext(bitmapImageRep: rep)
    let cg = NSGraphicsContext.current!.cgContext
    cg.clear(CGRect(x: 0, y: 0, width: w, height: h))
    var q = [Int](repeating: 0, count: 4)
    for y in 0..<h { for x in 0..<w { src.getPixel(&q, atX: x, y: y); rep.setPixel(&q, atX: x, y: y) } }

    // Paint over the old price strip with the card's own gradient, row by row
    // (sampled from a clean column near the left edge).
    let stripTop = Int(Double(h) * 0.80), stripBottom = Int(Double(h) * 0.955)
    let x0 = Int(Double(w) * 0.12), x1 = Int(Double(w) * 0.88)
    // Copy raw pixels (no colour-space conversion) from a clean column at x = 20% across the strip.
    let sampleX = Int(Double(w) * 0.2)
    var px = [Int](repeating: 0, count: 4)
    for y in stripTop..<stripBottom {
        src.getPixel(&px, atX: sampleX, y: y)
        for x in x0..<x1 {
            var p = px
            rep.setPixel(&p, atX: x, y: y)
        }
    }

    // Gem icon + price, centred in the strip.
    let line = CTLineCreateWithAttributedString(NSAttributedString(string: price, attributes: [
        .font: font, .foregroundColor: NSColor(calibratedWhite: 0.16, alpha: 1)]))
    let textW = CGFloat(CTLineGetTypographicBounds(line, nil, nil, nil))
    let gemSize: CGFloat = 150, gapPx: CGFloat = 24
    let total = gemSize + gapPx + textW
    let midY = CGFloat(h) - CGFloat(stripTop + stripBottom) / 2
    let startX = (CGFloat(w) - total) / 2
    gem.draw(in: NSRect(x: startX, y: midY - gemSize / 2, width: gemSize, height: gemSize))
    cg.textPosition = CGPoint(x: startX + gemSize + gapPx, y: midY - CTFontGetCapHeight(font) / 2)
    CTLineDraw(line, cg)

    NSGraphicsContext.current!.flushGraphics()
    try! rep.representation(using: .png, properties: [:])!.write(to: url)
    print("card", file, "->", price)
}
