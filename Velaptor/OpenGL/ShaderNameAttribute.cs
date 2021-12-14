namespace Velaptor.OpenGL
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    internal class ShaderNameAttribute : Attribute
    {
        public ShaderNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
