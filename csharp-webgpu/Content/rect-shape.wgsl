// ── Vertex / fragment shader for rounded-rectangle rendering ──────────────
//
// All shape properties arrive per-vertex via the vertex buffer.  The vertex
// shader is a pass-through: positions are already in NDC and the fragment
// shader receives the pixel-space bounding box and shape attributes, then
// decides per-pixel whether it falls inside the rounded rectangle (filled
// mode) or inside the border ring (border mode).
//
// NDC:          x ∈ [-1, +1] left→right,  y ∈ [-1, +1] bottom→top
// PIXEL SPACE:  origin at window top-left, Y increases downward
//               (matches @builtin(position) in the fragment shader)

// ── Vertex input layout ──────────────────────────────────────────────────
// stride = 64 bytes (16 × f32)
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

// ── Fragment shader ──────────────────────────────────────────────────────

const TOP_LEFT_CORNER     = 1u;
const TOP_RIGHT_CORNER    = 2u;
const BOTTOM_RIGHT_CORNER = 3u;
const BOTTOM_LEFT_CORNER  = 4u;

struct Rectangle {
    centerX: f32,
    centerY: f32,
    width:   f32,
    height:  f32,
}

struct Circle {
    cx: f32,
    cy: f32,
    radius: f32,
}

// ── Helper: linear map from one range to another ─────────────────────────
fn mapValue(value: f32, fromStart: f32, fromStop: f32, toStart: f32, toStop: f32) -> f32 {
    return toStart + ((toStop - toStart) * ((value - fromStart) / (fromStop - fromStart)));
}

// ── Helper: pixel 0‑255 color to normalized 0‑1 ──────────────────────────
fn toNDCColor(pixelColor: vec4<f32>) -> vec4<f32> {
    let r = mapValue(pixelColor.r, 0.0, 255.0, 0.0, 1.0);
    let g = mapValue(pixelColor.g, 0.0, 255.0, 0.0, 1.0);
    let b = mapValue(pixelColor.b, 0.0, 255.0, 0.0, 1.0);
    let a = mapValue(pixelColor.a, 0.0, 255.0, 0.0, 1.0);
    return vec4<f32>(r, g, b, a);
}

// ── Create a circle in a specific corner of the rectangle ────────────────
fn createCornerCircle(rect: Rectangle, cornerType: u32, radii: vec4<f32>) -> Circle {
    let halfW = rect.width / 2.0;
    let halfH = rect.height / 2.0;
    let maxR = min(halfW, halfH);

    var result: Circle;
    result.radius = 0.0;
    result.cx = 0.0;
    result.cy = 0.0;

    switch (cornerType) {
        case TOP_LEFT_CORNER: {
            result.radius = clamp(radii.x, 0.0, maxR);
            result.cx = (rect.centerX - halfW) + result.radius;
            result.cy = (rect.centerY - halfH) + result.radius;
        }
        case TOP_RIGHT_CORNER: {
            result.radius = clamp(radii.y, 0.0, maxR);
            result.cx = (rect.centerX + halfW) - result.radius;
            result.cy = (rect.centerY - halfH) + result.radius;
        }
        case BOTTOM_RIGHT_CORNER: {
            result.radius = clamp(radii.z, 0.0, maxR);
            result.cx = (rect.centerX + halfW) - result.radius;
            result.cy = (rect.centerY + halfH) - result.radius;
        }
        case BOTTOM_LEFT_CORNER: {
            result.radius = clamp(radii.w, 0.0, maxR);
            result.cx = (rect.centerX - halfW) + result.radius;
            result.cy = (rect.centerY + halfH) - result.radius;
        }
        default {}
    }
    return result;
}

// ── Is the fragment inside the corner circle? ────────────────────────────
fn containedByCircle(circle: Circle, cornerType: u32, radii: vec4<f32>, fragPos: vec2<f32>) -> bool {
    // If the radius is zero, no rounding → everything is "contained" (no clipping)
    switch (cornerType) {
        case TOP_LEFT_CORNER:     { if (radii.x == 0.0) { return true; } }
        case TOP_RIGHT_CORNER:    { if (radii.y == 0.0) { return true; } }
        case BOTTOM_RIGHT_CORNER: { if (radii.z == 0.0) { return true; } }
        case BOTTOM_LEFT_CORNER:  { if (radii.w == 0.0) { return true; } }
        default {}
    }

    // Ellipse equation: (x - cx)² / r² + (y - cy)² / r² <= 1.0
    let dx = fragPos.x - circle.cx;
    let dy = fragPos.y - circle.cy;
    return (dx * dx) / (circle.radius * circle.radius) +
           (dy * dy) / (circle.radius * circle.radius) <= 1.0;
}

// ── Is the fragment in the correct quadrant of the corner circle? ────────
fn inCorrectCircleQuadrant(circle: Circle, cornerType: u32, radii: vec4<f32>, fragPos: vec2<f32>) -> bool {
    switch (cornerType) {
        case TOP_LEFT_CORNER: {
            return radii.x == 0.0 ||
                fragPos.x < circle.cx && fragPos.y < circle.cy;
        }
        case TOP_RIGHT_CORNER: {
            return radii.y == 0.0 ||
                fragPos.x > circle.cx && fragPos.y < circle.cy;
        }
        case BOTTOM_RIGHT_CORNER: {
            return radii.z == 0.0 ||
                fragPos.x > circle.cx && fragPos.y > circle.cy;
        }
        case BOTTOM_LEFT_CORNER: {
            return radii.w == 0.0 ||
                fragPos.x < circle.cx && fragPos.y > circle.cy;
        }
        default { return false; }
    }
}

// ── Is the fragment inside the corner (circle + quadrant check)? ─────────
fn inRectCorner(circle: Circle, cornerType: u32, radii: vec4<f32>, fragPos: vec2<f32>) -> bool {
    return containedByCircle(circle, cornerType, radii, fragPos) &&
           inCorrectCircleQuadrant(circle, cornerType, radii, fragPos);
}

// ── Is the fragment in the corner tip (outside circle but in quadrant)? ───
fn inRectCornerTip(circle: Circle, cornerType: u32, radii: vec4<f32>, fragPos: vec2<f32>) -> bool {
    return !containedByCircle(circle, cornerType, radii, fragPos) &&
           inCorrectCircleQuadrant(circle, cornerType, radii, fragPos);
}

// ── Is the fragment inside the rounded rectangle? ────────────────────────
fn containedByRect(rect: Rectangle, radii: vec4<f32>, fragPos: vec2<f32>) -> bool {
    let tlCircle = createCornerCircle(rect, TOP_LEFT_CORNER, radii);
    let trCircle = createCornerCircle(rect, TOP_RIGHT_CORNER, radii);
    let brCircle = createCornerCircle(rect, BOTTOM_RIGHT_CORNER, radii);
    let blCircle = createCornerCircle(rect, BOTTOM_LEFT_CORNER, radii);

    let inTL = inRectCorner(tlCircle, TOP_LEFT_CORNER, radii, fragPos);
    let inTR = inRectCorner(trCircle, TOP_RIGHT_CORNER, radii, fragPos);
    let inBR = inRectCorner(brCircle, BOTTOM_RIGHT_CORNER, radii, fragPos);
    let inBL = inRectCorner(blCircle, BOTTOM_LEFT_CORNER, radii, fragPos);

    let inTLTip = inRectCornerTip(tlCircle, TOP_LEFT_CORNER, radii, fragPos);
    let inTRTip = inRectCornerTip(trCircle, TOP_RIGHT_CORNER, radii, fragPos);
    let inBRTip = inRectCornerTip(brCircle, BOTTOM_RIGHT_CORNER, radii, fragPos);
    let inBLTip = inRectCornerTip(blCircle, BOTTOM_LEFT_CORNER, radii, fragPos);

    let inAnyCorners = inTL || inTR || inBR || inBL;
    let notInAnyTips = !(inTLTip || inTRTip || inBRTip || inBLTip);

    let halfW = rect.width / 2.0;
    let halfH = rect.height / 2.0;
    let left   = rect.centerX - halfW;
    let right  = rect.centerX + halfW;
    let top    = rect.centerY - halfH;
    let bottom = rect.centerY + halfH;

    let inRect = fragPos.x >= left &&
                 fragPos.x <= right &&
                 fragPos.y >= top &&
                 fragPos.y <= bottom;

    return (inRect || inAnyCorners) && notInAnyTips;
}

@fragment
fn fs_main(fin: VertexOutput) -> @location(0) vec4<f32> {
    let shape = fin.shape;
    let halfW = max(shape.z / 2.0, 0.0);
    let halfH = max(shape.w / 2.0, 0.0);

    let isFilled = fin.isFilled > 0.0;

    let bClamped = clamp(fin.borderThickness, 1.0, min(halfW, halfH));

    let ndcColor = toNDCColor(fin.color);

    let outerRect = Rectangle(shape.x, shape.y, shape.z, shape.w);

    // Pack the four radii into a vec4 for convenient passing.
    let radii = vec4<f32>(fin.topLeftRadius, fin.topRightRadius, fin.bottomRightRadius, fin.bottomLeftRadius);

    let fragPos = fin.position.xy;

    let inOuterRect = containedByRect(outerRect, radii, fragPos);

    if (isFilled) {
        return select(vec4<f32>(0.0), ndcColor, inOuterRect);
    }

    // Border mode: inner rect creates the hollow centre.
    var innerW = shape.z - (bClamped * 2.0);
    var innerH = shape.w - (bClamped * 2.0);

    if (innerW % 2.0 != 0.0) { innerW -= 1.0; }
    if (innerH % 2.0 != 0.0) { innerH -= 1.0; }

    let innerRect = Rectangle(shape.x, shape.y, innerW, innerH);
    let notInInnerRect = !containedByRect(innerRect, radii, fragPos);

    return select(vec4<f32>(0.0), ndcColor, inOuterRect && notInInnerRect);
}
