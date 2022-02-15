#version 450 core

/* NOTES:
    The 'pass_' prefix means the variable was passed into this
    fragment shader from the vertex shader.
*/

const uint TOP_LEFT_CORNER = 1;
const uint BOTTOM_LEFT_CORNER = 2;
const uint BOTTOM_RIGHT_CORNER = 3;
const uint TOP_RIGHT_CORNER = 4;

/*
    pass_shape can represent either a rectangle or an ellipse.
    The center of either shape is the position represented by
    vec4.x and vec4.y.  The width is represented by vec4.z
    and the height is represented by vec4.w
*/
in vec4 pass_shape;

/*
    The color of the shape.
    vec4.x = red
    vec4.y = green
    vec4.z = blue
    vec4.w = alpha
*/
in vec4 pass_color;

// This represents a boolean value with 0.0 == false and 1.0 == true
in float pass_isFilled;
in float pass_borderThickness;
in float pass_topLeftCornerRadius;
in float pass_bottomLeftCornerRadius;
in float pass_bottomRightCornerRadius;
in float pass_topRightCornerRadius;

// NOTE: The frag coord is in pixel units.
// The line below sets the frag coordinate to be relative
// to the origin being at the upper left corner of the window
layout(origin_upper_left) in vec4 gl_FragCoord;

// The final color of the current pixel depending on where in the shape it is at
out vec4 finalColor;

/*
    Represents a rectangle to be rendered to the screen.
*/
struct Rectangle
{
    vec2 Position;
    float Width;
    float Height;
};

/*
    Represents an ellipse to be rendered to the screen.
*/
struct Ellipse
{
    vec2 Position;
    float RadiusX;
    float RadiusY;
};

float borderThickness = 1.0; // The clamped border thickness

/*
    Returns the value squared.
*/
float squared(float value)
{
    return pow(value, 2.0);
}

/*
    Creates a circle a given corner of the given rectangle
    to represent the radius of of each corner.
*/
Ellipse createCornerCircle(Rectangle rect, uint cornerType)
{
    float halfWidth = rect.Width / 2.0;
    float halfHeight = rect.Height / 2.0;
    float topLeftRadius = clamp(pass_topLeftCornerRadius, 0.0, halfWidth <= halfHeight ? halfWidth : halfHeight);
    float bottomLeftRadius = clamp(pass_bottomLeftCornerRadius, 0.0, halfWidth <= halfHeight ? halfWidth : halfHeight);
    float bottomRightRadius = clamp(pass_bottomRightCornerRadius, 0.0, halfWidth <= halfHeight ? halfWidth : halfHeight);
    float topRightRadius = clamp(pass_topRightCornerRadius, 0.0, halfWidth <= halfHeight ? halfWidth : halfHeight);

    Ellipse result;

    switch (cornerType)
    {
        case TOP_LEFT_CORNER:
            result.RadiusX = topLeftRadius;
            result.RadiusY = topLeftRadius;
            result.Position.x = (rect.Position.x - halfWidth) + topLeftRadius;
            result.Position.y = (rect.Position.y - halfHeight) + topLeftRadius;
            break;
        case BOTTOM_LEFT_CORNER:
            result.RadiusX = bottomLeftRadius;
            result.RadiusY = bottomLeftRadius;
            result.Position.x = (rect.Position.x - halfWidth) + bottomLeftRadius;
            result.Position.y = (rect.Position.y + halfHeight) - bottomLeftRadius;
            break;
        case BOTTOM_RIGHT_CORNER:
            result.RadiusX = bottomRightRadius;
            result.RadiusY = bottomRightRadius;
            result.Position.x = (rect.Position.x + halfWidth) - bottomRightRadius;
            result.Position.y = (rect.Position.y + halfHeight) - bottomRightRadius;
            break;
        case TOP_RIGHT_CORNER:
            result.RadiusX = topRightRadius;
            result.RadiusY = topRightRadius;
            result.Position.x = (rect.Position.x + halfWidth) - topRightRadius;
            result.Position.y = (rect.Position.y - halfHeight) + topRightRadius;
            break;
    }

    return result;
}

/*
    Returns a value indicating if the given ellipse contains the current pixel/fragment.
*/
bool containedByEllipse(Ellipse ellipse, uint cornerType)
{
    switch (cornerType)
    {
        case TOP_LEFT_CORNER:
            if (pass_topLeftCornerRadius < 0.0)
            {
                return true;
            }
            break;
        case BOTTOM_LEFT_CORNER:
            if (pass_bottomLeftCornerRadius < 0.0)
            {
                return true;
            }
            break;
        case BOTTOM_RIGHT_CORNER:
            if (pass_bottomRightCornerRadius < 0.0)
            {
                return true;
            }
            break;
        case TOP_RIGHT_CORNER:
            if (pass_topRightCornerRadius < 0.0)
            {
                return true;
            }
        break;
    }

    // Refer to link for more info
    // https://www.geeksforgeeks.org/check-if-a-point-is-inside-outside-or-on-the-ellipse/
    return squared(gl_FragCoord.x - ellipse.Position.x) /
        squared(ellipse.RadiusX) +
        squared(gl_FragCoord.y - ellipse.Position.y) /
        squared(ellipse.RadiusY) <= 1.0;
}

/*
    Returns a value indicating if the current pixel/fragment is
    in the given corner ellipse based on the type of corner.
*/
bool inCorrectEllipseQuadrant(Ellipse cornerEllipse, uint cornerType)
{
    /* NOTE:
        No need to check for containment if radius is 0
        This improves performance if there are many rectangles with no corners
    */

    bool result;

    switch(cornerType)
    {
        case TOP_LEFT_CORNER:
            result = pass_topLeftCornerRadius < 0.0 ||
                gl_FragCoord.x < cornerEllipse.Position.x && gl_FragCoord.y < cornerEllipse.Position.y;
            break;
        case BOTTOM_LEFT_CORNER:
            result = pass_bottomLeftCornerRadius < 0.0 ||
                gl_FragCoord.x < cornerEllipse.Position.x && gl_FragCoord.y > cornerEllipse.Position.y;
            break;
        case BOTTOM_RIGHT_CORNER:
            result = pass_bottomRightCornerRadius < 0.0 ||
                gl_FragCoord.x > cornerEllipse.Position.x && gl_FragCoord.y > cornerEllipse.Position.y;
            break;
        case TOP_RIGHT_CORNER:
            result = pass_topRightCornerRadius < 0.0 ||
                gl_FragCoord.x > cornerEllipse.Position.x && gl_FragCoord.y < cornerEllipse.Position.y;
            break;
    }

    return result;
}

/*
    Returns a value indicating if the current pixel/fragment is
    in the corner ellipse based on the type of corner.
*/
bool inRectCorner(Ellipse cornerEllipse, uint cornerType)
{
    return containedByEllipse(cornerEllipse, cornerType) && inCorrectEllipseQuadrant(cornerEllipse, cornerType);
}

/*
    Returns a value indicating if the current pixel/fragment is
    in the tip of the rectangle's corner outside of the given
    corner ellipse based on the type of corner.
*/
bool inRectCornerTip(Ellipse cornerEllipse, uint cornerType)
{
    return !containedByEllipse(cornerEllipse, cornerType) && inCorrectEllipseQuadrant(cornerEllipse, cornerType);
}

/*
    Returns value indicating if the given point is contained
    by the given shape.
*/
bool containedByRect(Rectangle rect)
{
    bool calculateCorners = pass_topLeftCornerRadius > 0;

    bool inTopLeftCorner;
    bool inBottomLeftCorner;
    bool inBottomRightCorner;
    bool inTopRightCorner;

    bool inTopLeftRectTip;
    bool inBottomLeftRectTip;
    bool inBottomRightRectTip;
    bool inTopRightRectTip;

    bool inAnyCorners;
    bool notInAnyCornerTips;

    Ellipse topLeftEllipse = createCornerCircle(rect, TOP_LEFT_CORNER);
    Ellipse bottomLeftEllipse = createCornerCircle(rect, BOTTOM_LEFT_CORNER);
    Ellipse bottomRightEllipse = createCornerCircle(rect, BOTTOM_RIGHT_CORNER);
    Ellipse topRightEllipse = createCornerCircle(rect, TOP_RIGHT_CORNER);

    inTopLeftCorner = inRectCorner(topLeftEllipse, TOP_LEFT_CORNER);
    inBottomLeftCorner = inRectCorner(bottomLeftEllipse, BOTTOM_LEFT_CORNER);
    inBottomRightCorner = inRectCorner(bottomRightEllipse, BOTTOM_RIGHT_CORNER);
    inTopRightCorner = inRectCorner(topRightEllipse, TOP_RIGHT_CORNER);

    inTopLeftRectTip = inRectCornerTip(topLeftEllipse, TOP_LEFT_CORNER);
    inBottomLeftRectTip = inRectCornerTip(bottomLeftEllipse, BOTTOM_LEFT_CORNER);
    inBottomRightRectTip = inRectCornerTip(bottomRightEllipse, BOTTOM_RIGHT_CORNER);
    inTopRightRectTip = inRectCornerTip(topRightEllipse, TOP_RIGHT_CORNER);

    inAnyCorners = inTopLeftCorner || inBottomLeftCorner || inBottomRightCorner || inTopRightCorner;
    notInAnyCornerTips = !inTopLeftRectTip && !inBottomLeftRectTip && !inBottomRightRectTip && !inTopRightRectTip;

    bool inTopLeft = inTopLeftCorner && !inTopLeftRectTip;
    bool inBottomLeft = inBottomLeftCorner && !inBottomLeftRectTip;
    bool inBottomRight = inBottomRightCorner && !inBottomRightRectTip;
    bool inTopRight = inTopRightCorner && !inTopRightRectTip;

    float halfWidth = rect.Width / 2.0;
    float halfHeight = rect.Height / 2.0;
    float left = rect.Position.x - halfWidth;
    float right = rect.Position.x + halfWidth;
    float top = rect.Position.y - halfHeight;
    float bottom = rect.Position.y + halfHeight;

    bool inNonRoundedCornerRect = gl_FragCoord.x >= left &&
    gl_FragCoord.x <= right &&
    gl_FragCoord.y >= top &&
    gl_FragCoord.y <= bottom;

    return (inNonRoundedCornerRect || inAnyCorners) && notInAnyCornerTips;
}

/*
    Maps the given value from one range to another.

    @param value The value to map.
    @param fromStart The from starting range value.
    @param fromStop The from ending range value.
    @param toStart The to starting range value.
    @param toStop The to ending range value.
*/
float mapValue(float value, float fromStart, float fromStop, float toStart, float toStop)
{
    return toStart + ((toStop - toStart) * ((value - fromStart) / (fromStop - fromStart)));
}

/*
    Converts the given color in pixel units to a color with
    NDC(Normalized Device Coordinate) units.
*/
vec4 toNDCColor(vec4 pixelColor)
{
    float red = mapValue(pixelColor.r, 0.0, 255.0, 0.0, 1.0);
    float green = mapValue(pixelColor.g, 0.0, 255.0, 0.0, 1.0);
    float blue = mapValue(pixelColor.b, 0.0, 255.0, 0.0, 1.0);
    float alpha = mapValue(pixelColor.a, 0.0, 255.0, 0.0, 1.0);

    return vec4(red, green, blue, alpha);
}

void main()
{
    // Set the width and height with a limit of 0.0
    vec4 shape = pass_shape;
    shape.z = pass_shape.z < 0.0 ? 0.0 : pass_shape.z;
    shape.w = pass_shape.w < 0.0 ? 0.0 : pass_shape.w;

    float halfWidth = shape.z / 2.0;
    float halfHeight = shape.w / 2.0;
    bool isFilled = pass_isFilled > 0.0;

    // Clamp the corner radius
    borderThickness = clamp(pass_borderThickness, 1.0, halfWidth <= halfHeight ? halfWidth : halfHeight);

    vec4 ndcColor = toNDCColor(pass_color);

    Rectangle outerRect;
    outerRect.Position = shape.xy;
    outerRect.Width = shape.z;
    outerRect.Height = shape.w;

    bool isContainedByOuterRect = containedByRect(outerRect);

    if (isFilled)
    {
        finalColor = isContainedByOuterRect
            ? ndcColor
            : vec4(0.0);
    }
    else
    {
        // Construct the inner shape where pixels will be transparent
        // This is what pulls off the empty shape effect with a border
        Rectangle innerRect;
        innerRect.Position = shape.xy;
        innerRect.Width = shape.z - (borderThickness * 2.0);
        innerRect.Height = shape.w - (borderThickness * 2.0);

        bool widthIsEven = mod(innerRect.Width, 2.0) == 0.0;
        bool heightIsEven = mod(innerRect.Height, 2.0) == 0.0;

        // If the width or height is an odd number, factor in rounding
        // issues to make sure the inner shape does not bleed over the
        // edges of the outer shape
        innerRect.Width = widthIsEven ? innerRect.Width : innerRect.Width - 1.0;
        innerRect.Height = heightIsEven ? innerRect.Height : innerRect.Height - 1.0;

        bool isNotContainedByInnerRect = !containedByRect(innerRect);

        finalColor = isContainedByOuterRect && isNotContainedByInnerRect
            ? ndcColor
            : vec4(0.0);
    }
}
