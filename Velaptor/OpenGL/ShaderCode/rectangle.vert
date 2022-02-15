#version 450 core

layout(location = 0) in vec2 a_vertexPos;
/*
    pass_shape can represent either a rectangle or an ellipse.
    The center of either shape is the position represented by
    vec4.x and vec4.y.  The width is represented by vec4.z
    and the height is represented by vec4.w
*/
layout(location = 1) in vec4 a_shape;
layout(location = 2) in vec4 a_color;

// This represents a boolean value with 0.0 == false and 1.0 == true
layout(location = 3) in float a_isFilled;
layout(location = 4) in float a_borderThickness;
layout(location = 5) in float a_topLeftCornerRadius;
layout(location = 6) in float a_bottomLeftCornerRadius;
layout(location = 7) in float a_bottomRightCornerRadius;
layout(location = 8) in float a_topRightCornerRadius;

out vec4 pass_shape;
out vec4 pass_color;
out float pass_isFilled;
out float pass_borderThickness;
out float pass_topLeftCornerRadius;
out float pass_bottomLeftCornerRadius;
out float pass_bottomRightCornerRadius;
out float pass_topRightCornerRadius;

void main()
{
    // Pass all of the shape information to the fragment shader
    pass_shape = a_shape;
    pass_color = a_color;
    pass_isFilled = a_isFilled;
    pass_borderThickness = a_borderThickness;
    pass_topLeftCornerRadius = a_topLeftCornerRadius;
    pass_bottomLeftCornerRadius = a_bottomLeftCornerRadius;
    pass_bottomRightCornerRadius = a_bottomRightCornerRadius;
    pass_topRightCornerRadius = a_topRightCornerRadius;

    gl_Position = vec4(a_vertexPos, 1.0, 1.0);
}
