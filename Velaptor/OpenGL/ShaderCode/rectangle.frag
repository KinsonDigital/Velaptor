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
    pass_shape can represent either a rectangle or an circle.
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

// NOTE: The fragment coordinate is in pixel units.
// The line below sets the fragment coordinate to be relative
// to the origin being at the upper left corner of the window
layout(origin_upper_left) in vec4 gl_FragCoord;

// The final color of the current pixel depending on where in the shape it is located.
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
    Represents an circle to be rendered to the screen.
*/
struct Circle
{
    vec2 Position;
    float Radius;
};

float borderThickness = 1.0; // The clamped border thickness

/*
    @summary - Returns the value squared.

    @param(float value) - The value to square.

    @returns(float) - The original value squared.
*/
float squared(float value)
{
    return pow(value, 2.0);
}

/*
    @summary - Creates a circle in a given corner of the given rectangle
    to represent the radius of a corner.  The corner is chosen by the 'cornerType'
    parameter.

    @param(Rectangle rect) - The rectangle that contains the corner circle.
    @param(uint cornerType) - The corner of where to put the circle.

    @returns(Circle) - A circle in a particular corner of the rectangle.
*/
Circle createCornerCircle(Rectangle rect, uint cornerType)
{
    float halfWidth = rect.Width / 2.0;
    float halfHeight = rect.Height / 2.0;
    float topLeftRadius = clamp(pass_topLeftCornerRadius, 0.0, halfWidth <= halfHeight ? halfWidth : halfHeight);
    float bottomLeftRadius = clamp(pass_bottomLeftCornerRadius, 0.0, halfWidth <= halfHeight ? halfWidth : halfHeight);
    float bottomRightRadius = clamp(pass_bottomRightCornerRadius, 0.0, halfWidth <= halfHeight ? halfWidth : halfHeight);
    float topRightRadius = clamp(pass_topRightCornerRadius, 0.0, halfWidth <= halfHeight ? halfWidth : halfHeight);

    Circle result;

    switch (cornerType)
    {
        case TOP_LEFT_CORNER:
            result.Radius = topLeftRadius;
            result.Position.x = (rect.Position.x - halfWidth) + topLeftRadius;
            result.Position.y = (rect.Position.y - halfHeight) + topLeftRadius;
            break;
        case BOTTOM_LEFT_CORNER:
            result.Radius = bottomLeftRadius;
            result.Position.x = (rect.Position.x - halfWidth) + bottomLeftRadius;
            result.Position.y = (rect.Position.y + halfHeight) - bottomLeftRadius;
            break;
        case BOTTOM_RIGHT_CORNER:
            result.Radius = bottomRightRadius;
            result.Position.x = (rect.Position.x + halfWidth) - bottomRightRadius;
            result.Position.y = (rect.Position.y + halfHeight) - bottomRightRadius;
            break;
        case TOP_RIGHT_CORNER:
            result.Radius = topRightRadius;
            result.Position.x = (rect.Position.x + halfWidth) - topRightRadius;
            result.Position.y = (rect.Position.y - halfHeight) + topRightRadius;
            break;
    }

    return result;
}

/*
    @summary - Gets a value indicating whether or not the given 'circle' contains
    the current pixel in a corner of the rectangle that matches the given 'cornerType'.

    @param(Circle circle) - The circle that may or may not contain the current pixel.
    @param(uint cornerType) - The type of corner where the 'circle' exists.

    @returns(bool) - True if the pixel is contained by the corner 'circle'.
*/
bool containedByCircle(Circle circle, uint cornerType)
{
    // If the corner does not have a radius, then it cannot be contained.
    // This is an optimization.  There is no point in doing the expensive
    // calcs at the bottom of this function if a circle does not exist.
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

    // Refer to link below for more information
    // https://www.geeksforgeeks.org/check-if-a-point-is-inside-outside-or-on-the-ellipse/
    return squared(gl_FragCoord.x - circle.Position.x) /
        squared(circle.Radius) +
        squared(gl_FragCoord.y - circle.Position.y) /
        squared(circle.Radius) <= 1.0;
}

/*
    @summary - Returns a value indicating whether or not the current pixel is in the
    given quadrant in the given 'cornerCircle' based on the 'cornerType'.

    @param(Circle cornerCircle) - The circle that may or may not contain the current pixel in its quadrant.
    @param(uint cornerType) - The type of corner that the 'cornerCircle' exists in.

    @returns(bool) - True if the pixel is in the correct quadrant of the given 'cornerCircle'.
*/
bool inCorrectCircleQuadrant(Circle cornerCircle, uint cornerType)
{
    /* NOTE:
        No need to check for containment if the radius is zero
        This improves performance if there are many rectangles with no corners.
    */

    bool result;

    switch(cornerType)
    {
        case TOP_LEFT_CORNER:
            result = pass_topLeftCornerRadius < 0.0 ||
                gl_FragCoord.x < cornerCircle.Position.x && gl_FragCoord.y < cornerCircle.Position.y;
            break;
        case BOTTOM_LEFT_CORNER:
            result = pass_bottomLeftCornerRadius < 0.0 ||
                gl_FragCoord.x < cornerCircle.Position.x && gl_FragCoord.y > cornerCircle.Position.y;
            break;
        case BOTTOM_RIGHT_CORNER:
            result = pass_bottomRightCornerRadius < 0.0 ||
                gl_FragCoord.x > cornerCircle.Position.x && gl_FragCoord.y > cornerCircle.Position.y;
            break;
        case TOP_RIGHT_CORNER:
            result = pass_topRightCornerRadius < 0.0 ||
                gl_FragCoord.x > cornerCircle.Position.x && gl_FragCoord.y < cornerCircle.Position.y;
            break;
    }

    return result;
}

/*
    @summary - Returns a value indicating whether or not the current pixel is
    in the given 'cornerCircle' based on the 'cornerType'.

    @param(Circle cornerCircle) - The circle that is in a corner that matches the given 'cornerType'.
    @param(uint cornerType) - The type of corner that the 'cornerCircle' exists in.

    @returns(bool) - True if the pixel is in the correct corner of the rectangle corner.
*/
bool inRectCorner(Circle cornerCircle, uint cornerType)
{
    return containedByCircle(cornerCircle, cornerType) && inCorrectCircleQuadrant(cornerCircle, cornerType);
}

/*
    @summary - Returns a value indicating whether or not the current pixel is
    in the tip of the rectangle's corner outside of the given
    'cornerCircle' based on the given 'cornerType'.

    @param(Circle cornerCircle) - The circle that is in a corner that matches the given 'cornerType'.
    @param(uint cornerType) - The type of corner that the 'cornerCircle' exists in.

    @returns(bool) - True if the pixel is in the correct corner tip of the rectangle corner.
*/
bool inRectCornerTip(Circle cornerCircle, uint cornerType)
{
    return !containedByCircle(cornerCircle, cornerType) && inCorrectCircleQuadrant(cornerCircle, cornerType);
}

/*
    @summary - Returns value indicating whether or not
    the current pixel is contained by the given 'rect'.

    @param(Rectangle rect) - The rectangle that may or may not contain the current pixel.

    @returns(bool) - True if the current pixel is contained in the rectangle.

    @remarks - The containment is based on the pixel being in the main rectangular area
               which includes the rectangle, any of the corner circles, but not in the tip
               of the rectangle corners outside of the corner circles.
*/
bool containedByRect(Rectangle rect)
{
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

    Circle topLeftCircle = createCornerCircle(rect, TOP_LEFT_CORNER);
    Circle bottomLeftCircle = createCornerCircle(rect, BOTTOM_LEFT_CORNER);
    Circle bottomRightCircle = createCornerCircle(rect, BOTTOM_RIGHT_CORNER);
    Circle topRightCircle = createCornerCircle(rect, TOP_RIGHT_CORNER);

    inTopLeftCorner = inRectCorner(topLeftCircle, TOP_LEFT_CORNER);
    inBottomLeftCorner = inRectCorner(bottomLeftCircle, BOTTOM_LEFT_CORNER);
    inBottomRightCorner = inRectCorner(bottomRightCircle, BOTTOM_RIGHT_CORNER);
    inTopRightCorner = inRectCorner(topRightCircle, TOP_RIGHT_CORNER);

    inTopLeftRectTip = inRectCornerTip(topLeftCircle, TOP_LEFT_CORNER);
    inBottomLeftRectTip = inRectCornerTip(bottomLeftCircle, BOTTOM_LEFT_CORNER);
    inBottomRightRectTip = inRectCornerTip(bottomRightCircle, BOTTOM_RIGHT_CORNER);
    inTopRightRectTip = inRectCornerTip(topRightCircle, TOP_RIGHT_CORNER);

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
    @summary - Maps the given value from the range dictated by the 'fromStart' and 'fromStop'
               values to the new range dictaged by the 'toStart' and 'toStop' values.

    @param(float value) - The value to map.
    @param(float fromStart) - The from starting range value.
    @param(float fromStop) - The from ending range value.
    @param(float toStart) - The to starting range value.
    @param(float toStop) - The to ending range value.

    @returns(float) - The given 'value' mapped to the enw range.
*/
float mapValue(float value, float fromStart, float fromStop, float toStart, float toStop)
{
    return toStart + ((toStop - toStart) * ((value - fromStart) / (fromStop - fromStart)));
}

/*
    @summary - Converts the given color in pixel units to a color with
    NDC(Normalized Device Coordinate) units.

    @param(vec4 pixelColor) - The pixel color with the components in the unit range of 0-255.

    @returns(vec4) - A four component vector of all the color components in NDC units.

    @remarks - NDC = (N)ormalized (D)evice (C)oordinate
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
    // Sets the width and height with a limit of 0.0
    vec4 shape = pass_shape;
    shape.z = pass_shape.z < 0.0 ? 0.0 : pass_shape.z;
    shape.w = pass_shape.w < 0.0 ? 0.0 : pass_shape.w;

    float halfWidth = shape.z / 2.0;
    float halfHeight = shape.w / 2.0;
    bool isFilled = pass_isFilled > 0.0;

    // Clamps the border thickness
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
            : vec4(0.0); // RGB of 0,0,0,0 which is transparent.  Example: Non filled rects or in the rect corner tip.
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
            : vec4(0.0); // RGB of 0,0,0,0 which is transparent.  Example: Non filled rects or in the rect corner tip.
    }
}
