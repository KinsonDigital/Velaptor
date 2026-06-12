// Vertex shader for textured quads.
//
// Positions arrive pre-converted to NDC from screen pixel coordinates (top-left
// origin, Y-down). Tint color is supplied as 0-255 floats per vertex and
// normalised to [0, 1] in the fragment shader.
//
// NDC:  x ∈ [-1, +1] left→right, y ∈ [-1, +1] bottom→top

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

@vertex
fn vs_main(vin: VertexInput) -> VertexOutput {
    var vout: VertexOutput;
    vout.position  = vec4<f32>(vin.position, 0.0, 1.0);
    vout.uv        = vin.uv;
    vout.tintColor = vin.tintColor;
    return vout;
}
