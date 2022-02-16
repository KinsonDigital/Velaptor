#version 450 core //Using version GLSL version 4.5.0
layout (location = 0) in vec4 vPos;

void main()
{
    gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
}
