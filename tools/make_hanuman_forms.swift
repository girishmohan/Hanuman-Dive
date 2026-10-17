// Builds the 8 shop "forms" of Hanuman from hanuman_player.png by recolouring it,
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

struct Form { let file: String; let name: String; let hue: Double; let sat: Double; let bright: Double; let contrast: Double; var aura: CIColor? = nil }

// Skin price -> image file mapping comes from the player objects in SampleScene.
let forms: [Form] = [
    Form(file: "buyPlayer/Group 30.png", name: "Golden (2500)", hue: 0.10, sat: 1.25, bright: 0.06, contrast: 1.0),
    Form(file: "buyPlayer/Group 28.png", name: "Sindoor (5000)", hue: -0.28, sat: 1.35, bright: -0.02, contrast: 1.05),
    Form(file: "buyPlayer/Group 31.png", name: "Forest (7500)", hue: 1.35, sat: 0.95, bright: 0.0, contrast: 1.0),
    Form(file: "buyPlayer/Group 29.png", name: "Ocean (15000)", hue: 2.9, sat: 0.95, bright: 0.02, contrast: 1.0),
    Form(file: "buyPlayer/Group 32.png", name: "Moon (50000)", hue: 0.0, sat: 0.12, bright: 0.12, contrast: 1.05),
    Form(file: "buyPlayer/Group 27.png", name: "Twilight (75000)", hue: -1.9, sat: 0.95, bright: 0.0, contrast: 1.0),
    Form(file: "buyPlayer/Group 34.png", name: "Lanka Night (100000)", hue: 3.4, sat: 0.7, bright: -0.18, contrast: 1.15),
    Form(file: "buyPlayer/Group 33.png", name: "Radiant (150000)", hue: 0.05, sat: 1.3, bright: 0.08, contrast: 1.05,
         aura: CIColor(red: 1.0, green: 0.82, blue: 0.25, alpha: 1)),
    // Default player sprite that the diver used (start menu, extra life).
    Form(file: "playerFall/midAstro.png", name: "Classic", hue: 0, sat: 1, bright: 0, contrast: 1),
]

func write(_ image: CIImage, to url: URL) {
    let extent = source.extent
    let cg = ctx.createCGImage(image.cropped(to: extent), from: extent)!
    let rep = NSBitmapImageRep(cgImage: cg)
    try! rep.representation(using: .png, properties: [:])!.write(to: url)
}

for f in forms {
    var img = source
        .applyingFilter("CIHueAdjust", parameters: [kCIInputAngleKey: f.hue])
        .applyingFilter("CIColorControls", parameters: [
            kCIInputSaturationKey: f.sat, kCIInputBrightnessKey: f.bright, kCIInputContrastKey: f.contrast])
    if let aura = f.aura {
        // Soft glow behind the figure, built from its own silhouette.
        let silhouette = source.applyingFilter("CIColorMatrix", parameters: [
            "inputRVector": CIVector(x: 0, y: 0, z: 0, w: 0), "inputGVector": CIVector(x: 0, y: 0, z: 0, w: 0),
            "inputBVector": CIVector(x: 0, y: 0, z: 0, w: 0), "inputAVector": CIVector(x: 0, y: 0, z: 0, w: 1),
            "inputBiasVector": CIVector(x: aura.red, y: aura.green, z: aura.blue, w: 0)])
        let glow = silhouette.clampedToExtent().applyingFilter("CIGaussianBlur", parameters: [kCIInputRadiusKey: 14])
            .cropped(to: source.extent)
        img = img.composited(over: glow.composited(over: glow))
    }
    write(img, to: mat.appendingPathComponent(f.file))
    print("form", f.name, "->", f.file)
}
