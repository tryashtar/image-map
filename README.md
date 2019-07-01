
# <img src="https://i.imgur.com/E6BqiuN.png" width=48> Image Map
### [Download link](https://github.com/tryashtar/image-map/releases)

This is a simple Windows application that converts images to Minecraft maps using the latest colors. It works for Java Edition 1.12 - 1.14+ and Bedrock 1.5 - 1.12+ (inferior Windows 10 Edition).  

## How to Use
First, click the button depending on the edition you're using. For Java Edition, you'll need to browse to your saves folder. By default, it's located at `%appdata%\.minecraft\saves`. Choose your world and click **Select Folder**.

### Import Maps
This tab shows maps that you want to import. Click **Open** to browse for images to convert into maps. For each image, you'll be able to change some import settings.
* **Scaling Mode**: Change this if the result comes out blurry or pixelated.
* **Rotate Button**: Press this to rotate the image by 90 degrees clockwise.
* **Map Split**: Increasing these will split the images into multiple maps, and will show you how they'll look when arranged in a grid.
* **Dither**: For Java Edition, this improves color accuracy but introduces noise.
* **Apply to All**: If this is checked, pressing confirm will skip this settings menu for all other images you imported and use the current settings for them. Pressing cancel will discard all other images you imported.

Once your images are imported, they'll appear as they will in-game as maps. Hover over an image to see the original. You can right-click maps to discard them, and they will not be sent to the world. When you're ready, click **Send All to World**.  

If the **Add new maps to inventory** checkbox is checked, a chest will be added to your inventory containing all the maps you just imported.

### Existing Maps
This tab shows maps that are currently in your world. You can click a map to select it, then click again to deselect. The three buttons at the bottom apply to all selected maps.
* **Export Image**: Saves the map image itself as a 128x128 PNG image file. If you have one map selected, it will be saved directly. If you have multiple selected, they will all be saved in a folder.
* **Add to Inventory**: Adds a chest to your inventory containing all the selected maps.
* **Delete**: Removes the maps from the world entirely. All copies will be made blank.

## Java Color Showcase
The following is a comparison of this program's map output in-game compared to some other image-to-map applications.

#### This program
<img src="https://i.imgur.com/VDY27Qk.png" width="480" height="270"/>

---
#### [ImageToMapX](http://www.mediafire.com/file/2celv1aoye62o3t/ImageToMapX1.4-Windows.zip)
<img src="https://i.imgur.com/Rv3zpCK.png" width="480" height="270"/>

---
### [DJFun](https://mc-map.djfun.de/)
<img src="https://i.imgur.com/IA9Y2vx.png" width="480" height="270"/>
