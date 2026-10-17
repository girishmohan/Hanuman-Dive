// Builds the 8 shop avatars from hanuman_player.png: same Hanuman, different aura colour,
// writing them over the old diver skin images (same files, so the shop and the
// in-game skins both pick them up). Also replaces the diver on the default sprite.
//
//   swift tools/make_hanuman_forms.swift
import AppKit
import CoreImage
import Foundation

let root = URL(fileURLWithPath: CommandLine.arguments[0]).deletingLastPathComponent().deletingLastPathComponent()
let mat = root.appendingPathComponent("hyper dive/Assets/A-Z_Materials")
let source = CIImage(contentsOf: mat.appendingPathComponent("hanuman/hanuman_player.png"))!
let ctx = CIContext()

struct Form { let file: String; let name: String; let aura: CIColor; var strength: Double = 1 }

// Every avatar is the same refined Hanuman; each form is a different aura colour.
// Skin price -> image file mapping comes from the player objects in SampleScene.
let forms: [Form] = [
    Form(file: "buyPlayer/Group 30.png", name: "Golden (2500)", aura: CIColor(red: 1.0, green: 0.80, blue: 0.20)),
    Form(file: "buyPlayer/Group 28.png", name: "Sindoor (5000)", aura: CIColor(red: 0.95, green: 0.25, blue: 0.15)),
    Form(file: "buyPlayer/Group 31.png", name: "Forest (7500)", aura: CIColor(red: 0.30, green: 0.85, blue: 0.35)),
    Form(file: "buyPlayer/Group 29.png", name: "Ocean (15000)", aura: CIColor(red: 0.20, green: 0.65, blue: 1.0)),
    Form(file: "buyPlayer/Group 32.png", name: "Moon (50000)", aura: CIColor(red: 0.92, green: 0.95, blue: 1.0)),
    Form(file: "buyPlayer/Group 27.png", name: "Twilight (75000)", aura: CIColor(red: 0.70, green: 0.40, blue: 1.0)),
    Form(file: "buyPlayer/Group 34.png", name: "Lanka Night (100000)", aura: CIColor(red: 0.25, green: 0.15, blue: 0.60), strength: 1.3),
    Form(file: "buyPlayer/Group 33.png", name: "Radiant (150000)", aura: CIColor(red: 1.0, green: 0.88, blue: 0.35), strength: 1.8),
    // Default player sprite that the diver used (start menu, extra life): no aura.
    Form(file: "playerFall/midAstro.png", name: "Classic", aura: CIColor(red: 0, green: 0, blue: 0), strength: 0),
]

func write(_ image: CIImage, to url: URL) {
    let extent = source.extent
    let cg = ctx.createCGImage(image.cropped(to: extent), from: extent)!
    let rep = NSBitmapImageRep(cgImage: cg)
    try! rep.representation(using: .png, properties: [:])!.write(to: url)
}

for f in forms {
    var img = source
    if f.strength > 0 {
        // Soft glow behind the figure, built from its own silhouette.
        let silhouette = source.applyingFilter("CIColorMatrix", parameters: [
            "inputRVector": CIVector(x: 0, y: 0, z: 0, w: 0), "inputGVector": CIVector(x: 0, y: 0, z: 0, w: 0),
            "inputBVector": CIVector(x: 0, y: 0, z: 0, w: 0), "inputAVector": CIVector(x: 0, y: 0, z: 0, w: CGFloat(f.strength)),
            "inputBiasVector": CIVector(x: f.aura.red, y: f.aura.green, z: f.aura.blue, w: 0)])
        let glow = silhouette.clampedToExtent().applyingFilter("CIGaussianBlur", parameters: [kCIInputRadiusKey: 16])
            .cropped(to: source.extent)
        img = img.composited(over: glow.composited(over: glow.composited(over: glow)))
    }
    write(img, to: mat.appendingPathComponent(f.file))
    print("form", f.name, "->", f.file)
}
