// Fragment shader for line rendering.
//
// Color is supplied as 0-255 floats per vertex and must be normalised to [0, 1].
//
// sRGB→linear conversion is applied because the swap-chain surface is sRGB.

struct VertexOutput {
    @builtin(position) position: vec4<f32>,
    @location(0)       color:    vec4<f32>,
};

fn mapValue(value: f32, fromStart: f32, fromStop: f32, toStart: f32, toStop: f32) -> f32 {
    return toStart + ((toStop - toStart) * ((value - fromStart) / (fromStop - fromStart)));
}

fn srgbToLinear(c: f32) -> f32 {
    if c <= 0.04045 {
        return c / 12.92;
    }
    return pow((c + 0.055) / 1.055, 2.4);
}

fn toNDCColor(pixelColor: vec4<f32>) -> vec4<f32> {
    return vec4<f32>(
        srgbToLinear(mapValue(pixelColor.r, 0.0, 255.0, 0.0, 1.0)),
        srgbToLinear(mapValue(pixelColor.g, 0.0, 255.0, 0.0, 1.0)),
        srgbToLinear(mapValue(pixelColor.b, 0.0, 255.0, 0.0, 1.0)),
        mapValue(pixelColor.a, 0.0, 255.0, 0.0, 1.0));
}

@fragment
fn fs_main(fin: VertexOutput) -> @location(0) vec4<f32> {
    return toNDCColor(fin.color);
}
