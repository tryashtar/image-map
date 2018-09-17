

# <img src="https://i.imgur.com/E6BqiuN.png" width=48> Image Map
### [Download link](https://github.com/tryashtar/image-map/releases)

This is a simple Windows application that converts images to Minecraft maps using the latest colors. It works for Java Edition 1.13+ and Bedrock 1.5/1.6+ (inferior Windows 10 Edition).  

## How to Use
First, click the button depending on the edition you're using. For Java, you'll need to browse to your saves folder. By default, it's located at `%appdata%\.minecraft\saves`. Choose your world and click **Select Folder**.

### Import Maps
This tab shows maps that you want to import. Click **Open** to browse for images to convert into maps. For each image, you'll be able to change some import settings.
* **Scaling Mode** (defaults to "Automatic"): You usually don't want to touch this, but if your images are looking blurry or pixelated, try changing it.
* **Rotate Button**: Press this to rotate the image by 90 degrees clockwise.
* **Map Split**: Increasing these will split the images into multiple maps, and will show you how it will look when arranged in a grid.
* **Apply to All**: If this is checked, pressing confirm will skip this settings menu for all other images you imported and use the current settings for them. Pressing cancel will discard all other images you imported.

Once your images are imported, they'll appear as they will in-game as maps. Hover over an image to see the original. You can right-click maps to discard them, and they will not be sent to the world. When you're ready, click **Send All to World**.  

If the **Add new maps to inventory** checkbox is checked, a chest will be added to your inventory containing all the maps you just imported.

### Existing Maps
This tab shows maps that are currently in your world. You can click a map to select it, then click again to deselect. The three buttons at the bottom apply to all selected maps.
* **Export Image**: Saves the map image itself as a 128x128 PNG image file. If you have one map selected, it will be saved directly. If you have multiple selected, they will all be saved in a folder.
* **Add to Inventory**: Adds a chest to your inventory containing all the selected maps.
* **Delete**: Removes the maps from the world entirely. All copies will be made blank.

## Java Color Showcase
The following is a comparison of this program's map output compared to the most commonly used image-to-map program, [ImageToMapX](http://www.minecraftforum.net/forums/mapping-and-modding/minecraft-tools/1261738), which still uses pre-1.12 colors.

#### This program
<img src="http://i.imgur.com/2hLXneF.png" width="480" height="270"/>

#### ImageToMapX
<img src="http://i.imgur.com/UBN7uGL.png" width="480" height="270"/>
