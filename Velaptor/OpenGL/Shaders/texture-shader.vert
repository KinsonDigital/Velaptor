#version 450 core

//These are vars that take in data from the vertex data buffer
layout(location = 0) in vec3 a_vertexPos;
layout(location = 1) in vec2 a_textureCoord;
layout(location = 2) in vec4 a_tintColor;

//These are vars that send data out and can be
//used as input into the fragment shader
out vec2 pass_textureCoord;
out vec4 pass_tintColor;


void main()
{
    pass_tintColor = a_tintColor;
    pass_textureCoord = a_textureCoord;

    gl_Position = vec4(a_vertexPos, 1.0);
}
