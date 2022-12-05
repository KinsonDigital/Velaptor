#version 450 core

in vec4 pass_color;
out vec4 finalColor;

/*
    @summary Maps the given value from one range to another.

    @param float - value The value to map.
    @param float - fromStart The from starting range value.
    @param float - fromStop The from ending range value.
    @param float - toStart The to starting range value.
    @param float - toStop The to ending range value.
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
    finalColor = toNDCColor(pass_color);
}
