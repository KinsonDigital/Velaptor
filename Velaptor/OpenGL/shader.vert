#version 450 core

//These are vars that take in data from the vertex data buffer
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec4 aTintColor;
layout(location = 3) in float aTransformIndex;

//Uniforms are vars that can dynamically take in data
//from the CPU side at will
uniform mat4 uTransform[1];//$REPLACE_INDEX

//These are vars that send data out and can be
//used as input into the fragment shader
out vec2 v_TexCoord;
out vec4 v_TintClr;


void main()
{
    v_TintClr = aTintColor;
    v_TexCoord = aTexCoord;

    int index = int(aTransformIndex);

    gl_Position = vec4(aPosition, 1.0) * uTransform[index];
}