// Vertex shader for rounded-rectangle rendering.
//
// All shape properties arrive per-vertex via the vertex buffer.  The vertex
// shader is a pass-through: positions are already in NDC and the fragment
// shader receives the pixel-space bounding box and shape attributes.
//
// NDC:  x ∈ [-1, +1] left→right,  y ∈ [-1, +1] bottom→top

// Vertex input layout — stride = 64 bytes (16 × f32)
struct VertexInput {
    @location(0) position:          vec2<f32>,   // NDC vertex position
    @location(1) shape:             vec4<f32>,   // (centerX, centerY, width, height) in pixel coords
    @location(2) color:             vec4<f32>,   // RGBA as 0‑255 floats
    @location(3) isFilled:          f32,         // 0.0 = hollow border, 1.0 = solid
    @location(4) borderThickness:   f32,
    @location(5) topLeftRadius:     f32,
    @location(6) topRightRadius:    f32,
    @location(7) bottomRightRadius: f32,
    @location(8) bottomLeftRadius:  f32,
};

struct VertexOutput {
    @builtin(position) position:          vec4<f32>,
    @location(0)       shape:             vec4<f32>,
    @location(1)       color:             vec4<f32>,
    @location(2)       isFilled:          f32,
    @location(3)       borderThickness:   f32,
    @location(4)       topLeftRadius:     f32,
    @location(5)       topRightRadius:    f32,
    @location(6)       bottomRightRadius: f32,
    @location(7)       bottomLeftRadius:  f32,
};

@vertex
fn vs_main(vin: VertexInput) -> VertexOutput {
    var vout: VertexOutput;
    vout.position = vec4<f32>(vin.position, 0.0, 1.0);
    vout.shape = vin.shape;
    vout.color = vin.color;
    vout.isFilled = vin.isFilled;
    vout.borderThickness = vin.borderThickness;
    vout.topLeftRadius = vin.topLeftRadius;
    vout.topRightRadius = vin.topRightRadius;
    vout.bottomRightRadius = vin.bottomRightRadius;
    vout.bottomLeftRadius = vin.bottomLeftRadius;
    return vout;
}
