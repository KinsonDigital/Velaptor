## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.23.1</span> - <span style="color:indianred;font-weight:bold">(Hot Fix)</span>

### **Bug Fixes** 🐛

1. Fixed a bug where only the max of 2 textures could be rendered in a batch on the same texture
   * The max quad buffer data amount (batch size) of 2 was all that was getting alloted on the GPU.  This was throwing an exception when attempting to render more than 2 quads worth of buffer data

### **New** 🎉

1. Added the ability to the <span style='font-weight: bold; color: khaki'>SpriteBatch </span> class to be able to set a custom batch size amount
   * Doing this will dispose of the GPU buffers and shader programs and re-create them
