# Maya WarTex (BLP Image Loader)

This is a small Autodesk Maya plugin written using the .NET API.
It adds support for loading BLP (a legacy texture format used by Warcraft III and World of Warcraft) image files via `MPxImageFile`, allowing them to be opened as regular image files inside Maya.

<p align="center">
    <img src="images/ImagePlane.png" width="400"/>
    <img src="images/Model.png" width="400"/>
</p>

There's no direct GPU loading (the `glLoad()` method) here, only the `MImage` instance is initialized. Mipmaps are ignored.

## Dirty MEL hack
The project also demonstrates how image file extensions can be registered in Maya’s Image Types list at runtime using MEL from managed code.

![](./images/OpenFileDialog.png)

Maya does not provide a public API for this in `MPxImageFile`, so a MEL-based workaround is required.

## Target DCC
Made for .NET 8 compatible versions of Maya (Maya 2025 or later), Windows only.

## Dependencies 
- [War3Net.Drawing.Blp](https://github.com/Drake53/War3Net/tree/master/src/War3Net.Drawing.Blp) by Drake53. Used as a NuGet package for reading and decoding BLP textures.
- `openmayacs.dll`. Maya .NET API assembly provided with Autodesk Maya.