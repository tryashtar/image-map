# <img src="https://i.imgur.com/E6BqiuN.png" width=48> Image Map

### [Download link](https://github.com/tryashtar/image-map/releases)

This is a simple Windows application that converts images to Minecraft maps using the latest colors. It works for both Java Edition and Bedrock Edition (also known as the inferior Windows 10 version).

## How to Use

Firstly: download, unzip, and run the application. Click the button that corresponds to the edition you're using, or drag the world folder right onto the form.

### Existing Maps

This tab shows maps that are currently in your world. Click on a map to select it, and again to deselect. Right-click to open the options menu:  
<img src="https://i.imgur.com/vuEaZPn.png" width=200>

If your world has tons of maps, not all of them are loaded at once, for performance reasons. You can press these buttons to continue loading in maps, if you wish to preview them:  
<img src="https://i.imgur.com/OG4vZjN.png" width=140> <img src="https://i.imgur.com/C23TcQQ.png" width=140>&nbsp;  
Note that even if some maps aren't loaded, the app will still detect and warn about transfer conflicts if you try to change the ID of a map to one that's already taken.

### Import Maps

This tab shows maps that you want to import into your world. Click **Open** to browse for images to convert into maps, or drag files right onto the form. You can also use `CTRL-V` to paste if there is image data on your clipboard, such as a screenshot.

You'll be able to control a few settings about your imported images:

---

<img src="https://i.imgur.com/8yb14cK.png" height=25>&nbsp;  
Press this to rotate the image by 90 degrees clockwise.  
<img src="https://i.imgur.com/vRi1KIU.png" width=200> <img src="https://i.imgur.com/1OmSXUn.png" width=200>

---

If your image isn't exactly 128x128 pixels, it needs to be resized.  
<img src="https://i.imgur.com/hsNEeDv.png" height=25>&nbsp; Use this scaling (nearest neighbor) to keep pixels looking sharp.  
<img src="https://i.imgur.com/POslryd.png" width=200> <img src="https://i.imgur.com/fE2TUjg.png" width=200>&nbsp;  
<img src="https://i.imgur.com/2NR4FQS.png" height=25>&nbsp; Use this scaling to keep lines looking smooth.  
<img src="https://i.imgur.com/6zviveK.png" width=200> <img src="https://i.imgur.com/oiCUFvq.png" width=200>

---

<img src="https://i.imgur.com/7ATc95p.png" height=25>&nbsp;  
You can split your image into a grid of multiple maps, to put into item frames.  
<img src="https://i.imgur.com/h0KyxtQ.png" width=400>&nbsp;  
Check <img src="https://i.imgur.com/JeocVvl.png" height=16> to fill the entire space with your image. Leave it unchecked to maintain your aspect ratio.

---
Java Edition has a limited color palette for maps, so there are a few options to help your images look accurate. I use the Good Fast Algorithm with dithering most of the time, but feel free to try other algorithms to look for a better result. Dithering improves color accuracy, but introduces noise.

<img src="https://i.imgur.com/IDyH7uJ.png" height=20>&nbsp;  
<img src="https://i.imgur.com/HZAsWVt.png" width=170> <img src="https://i.imgur.com/pY5AaCA.png" width=170> <img src="https://i.imgur.com/GF0vBp3.png" width=170>&nbsp;  
<img src="https://i.imgur.com/VhtTdZX.png" width=170> <img src="https://i.imgur.com/2DjitsV.png" width=170> <img src="https://i.imgur.com/84vOGax.png" width=170>&nbsp;  
<img src="https://i.imgur.com/s9rrx7n.png" width=170> <img src="https://i.imgur.com/T1dZbPb.png" width=170>&nbsp;  
<img src="https://i.imgur.com/A4xj78s.png" width=170> <img src="https://i.imgur.com/4bqfzfp.png" width=170>

<img src="https://i.imgur.com/H7SlmFG.png" height=20>&nbsp;  
<img src="https://i.imgur.com/HZAsWVt.png" width=170> <img src="https://i.imgur.com/pY5AaCA.png" width=170> <img src="https://i.imgur.com/GF0vBp3.png" width=170>&nbsp;  
<img src="https://i.imgur.com/sDo7gXu.png" width=170> <img src="https://i.imgur.com/NpeDOzM.png" width=170> <img src="https://i.imgur.com/ovNTrnj.png" width=170>&nbsp;  
<img src="https://i.imgur.com/s9rrx7n.png" width=170> <img src="https://i.imgur.com/T1dZbPb.png" width=170>&nbsp;  
<img src="https://i.imgur.com/vEBDbwL.png" width=170> <img src="https://i.imgur.com/h5vnEzT.png" width=170>

---

Large item frames and slow algorithms may take some time to process. Maps that are still loading will appear with this icon:  
<img src="https://i.imgur.com/E6BqiuN.png" width=80>

You can hover over any map to see the original image. Click on a map to select it, and again to deselect. Right-click to open the options menu:  
<img src="https://i.imgur.com/xerSSiz.png" width=200>

You can also send all of your maps to the world at once with this button:  
<img src="https://i.imgur.com/EjrbLtG.png" width=200>

If <img src="https://i.imgur.com/Km59iGD.png" height=20> is checked, a chest will be added to your singleplayer inventory containing all the maps you sent. Ensure your world isn't open in Minecraft, or this won't work!
