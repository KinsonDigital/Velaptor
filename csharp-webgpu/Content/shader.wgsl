// Vertex/fragment shader for textured quads.
//
// Positions arrive pre-converted to NDC from screen pixel coordinates (top-left
// origin, Y-down). Tint color is supplied as 0-255 floats per vertex and
// normalised to [0, 1] in the fragment shader. UV V=0 is the top of the
// texture, V=1 is the bottom (WebGPU convention — no Y-flip needed).
//
// NDC:  x ∈ [-1, +1] left→right, y ∈ [-1, +1] bottom→top
// UV:   u ∈ [0,   1] left→right, v ∈ [0,   1] top→bottom

struct VertexInput {
    @location(0) position:  vec2<f32>,  // NDC vertex position
    @location(1) uv:        vec2<f32>,  // texture UV [0, 1]
    @location(2) tintColor: vec4<f32>,  // RGBA as 0-255 floats
};

struct VertexOutput {
    @builtin(position) position:  vec4<f32>,
    @location(0)       uv:        vec2<f32>,
    @location(1)       tintColor: vec4<f32>,
};

@group(0) @binding(0) var t_diffuse: texture_2d<f32>;
@group(0) @binding(1) var s_diffuse: sampler;

@vertex
fn vs_main(vin: VertexInput) -> VertexOutput {
    var vout: VertexOutput;
    vout.position  = vec4<f32>(vin.position, 0.0, 1.0);
    vout.uv        = vin.uv;
    vout.tintColor = vin.tintColor;
    return vout;
}

fn mapValue(value: f32, fromStart: f32, fromStop: f32, toStart: f32, toStop: f32) -> f32 {
    return toStart + ((toStop - toStart) * ((value - fromStart) / (fromStop - fromStart)));
}

fn toNDCColor(pixelColor: vec4<f32>) -> vec4<f32> {
    return vec4<f32>(
        mapValue(pixelColor.r, 0.0, 255.0, 0.0, 1.0),
        mapValue(pixelColor.g, 0.0, 255.0, 0.0, 1.0),
        mapValue(pixelColor.b, 0.0, 255.0, 0.0, 1.0),
        mapValue(pixelColor.a, 0.0, 255.0, 0.0, 1.0));
}

@fragment
fn fs_main(fin: VertexOutput) -> @location(0) vec4<f32> {
    let ndcColor = toNDCColor(fin.tintColor);
    return textureSample(t_diffuse, s_diffuse, fin.uv) * ndcColor;
}

