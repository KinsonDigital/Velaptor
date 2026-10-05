// Fragment shader for textured quads (and font atlas rendering).
//
// Tint color is supplied as 0-255 floats per vertex and normalised to [0, 1].
// UV V=0 is the top of the texture, V=1 is the bottom (WebGPU convention —
// no Y-flip needed).
//
// UV:   u ∈ [0, 1] left→right, v ∈ [0, 1] top→bottom

struct VertexOutput {
    @builtin(position) position:  vec4<f32>,
    @location(0)       uv:        vec2<f32>,
    @location(1)       tintColor: vec4<f32>,
};

@group(0) @binding(0) var t_diffuse: texture_2d<f32>;
@group(0) @binding(1) var s_diffuse: sampler;

fn mapValue(value: f32, fromStart: f32, fromStop: f32, toStart: f32, toStop: f32) -> f32 {
    return toStart + ((toStop - toStart) * ((value - fromStart) / (fromStop - fromStart)));
}

// Converts a single sRGB component [0, 1] to linear light using the IEC 61966-2-1 formula.
// Needed because the swap-chain surface format is sRGB: the GPU applies linear→sRGB encoding
// on every fragment output, so colours must be in linear space before they are written.
fn srgbToLinear(c: f32) -> f32 {
    if c <= 0.04045 {
        return c / 12.92;
    }
    return pow((c + 0.055) / 1.055, 2.4);
}

fn toNDCColor(pixelColor: vec4<f32>) -> vec4<f32> {
    // RGB: convert from sRGB 0-255 → linear [0, 1].
    // Alpha: linear pass-through (alpha is never sRGB-encoded).
    return vec4<f32>(
        srgbToLinear(mapValue(pixelColor.r, 0.0, 255.0, 0.0, 1.0)),
        srgbToLinear(mapValue(pixelColor.g, 0.0, 255.0, 0.0, 1.0)),
        srgbToLinear(mapValue(pixelColor.b, 0.0, 255.0, 0.0, 1.0)),
        mapValue(pixelColor.a, 0.0, 255.0, 0.0, 1.0));
}

@fragment
fn fs_main(fin: VertexOutput) -> @location(0) vec4<f32> {
    let ndcColor = toNDCColor(fin.tintColor);
    return textureSample(t_diffuse, s_diffuse, fin.uv) * ndcColor;
}
