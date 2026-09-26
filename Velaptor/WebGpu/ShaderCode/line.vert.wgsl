// Vertex shader for line rendering.
//
// Positions arrive pre-converted to NDC. Color is per-vertex as 0-255 floats.
//
// NDC:  x ∈ [-1, +1] left→right,  y ∈ [-1, +1] bottom→top

struct VertexInput {
    @location(0) position: vec2<f32>,  // NDC vertex position
    @location(1) color:    vec4<f32>,  // RGBA as 0-255 floats
};

struct VertexOutput {
    @builtin(position) position: vec4<f32>,
    @location(0)       color:    vec4<f32>,
};

@vertex
fn vs_main(vin: VertexInput) -> VertexOutput {
    var vout: VertexOutput;
    vout.position = vec4<f32>(vin.position, 0.0, 1.0);
    vout.color = vin.color;
    return vout;
}
