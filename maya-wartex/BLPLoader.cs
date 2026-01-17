using Autodesk.Maya.OpenMaya;
using War3Net.Drawing.Blp;

[assembly: MPxImageFileClass(typeof(WarTex.BLPLoader), "BLPLoader")]

namespace WarTex
{
    [MPxImageExtension("blp")]
    class BLPLoader : MPxImageFile
    {
        private const uint ChannelsNumber = 4;
        private const bool HasAlpha = true;
        private const MImage.MPixelType PixelType = MImage.MPixelType.kByte;

        private string? _path;
        public override void open(string pathname, MImageFileInfo info)
        {
            if (string.IsNullOrEmpty(pathname) || info == null)
                return;

            try
            {
                FillMetaData(pathname, info);
                _path = pathname;
            }
            catch (Exception ex)
            {
                MGlobal.displayError($"Error while opening BLP file {pathname}\r\n\t{ex}");
            }
        }

        public override void load(MImage image, uint imageNumber)
        {
            if (image == null || imageNumber > 0) 
                return;

            try
            {
                CreateImage(image);
            }
            catch( Exception ex )
            {
                MGlobal.displayError($"Error while loading BLP file {_path}\r\n\t{ex}");
            }
        }

        public override void close()
        {
            _path = null;
            base.close();
        }

        private static void FillMetaData(string pathname, MImageFileInfo info)
        {
            using var fs = File.OpenRead(pathname);
            var blpFile = new BlpFile(fs);

            info.width = (uint)blpFile.Width;
            info.height = (uint)blpFile.Height;
            info.channels = ChannelsNumber;
            info.hasAlpha = HasAlpha;

            info.pixelType = PixelType;
            info.imageType = MImageFileInfo.MImageType.kImageTypeColor;
            info.numberOfImages = 1;

            // BLP typically has mipmaps, but for Maya it is GPU loading (glLoad()) feature only
            // info.hasMipMaps = blp.MipMapCount > 1;
        }

        private void CreateImage(MImage image)
        {
            if (String.IsNullOrEmpty(_path))
                return;

            using var fs = File.OpenRead(_path);
            var blpFile = new BlpFile(fs);
            
            // BGRA
            byte[] pixels = blpFile.GetPixels(0, out int w, out int h, true);
            
            long expectedSize = (long)w * h * ChannelsNumber;
            if (pixels.Length != expectedSize)
                throw new InvalidDataException($"Unexpected pixel buffer size: {pixels.Length}, expected {expectedSize}");

            image.create((uint)w, (uint)h, ChannelsNumber, PixelType);

            unsafe
            {
                byte* dst = image.pixels();
                fixed (byte* src = pixels)
                {
                    Buffer.MemoryCopy(src, dst, expectedSize, expectedSize);
                }
                
                // Flip Y
                image.verticalFlip();
            }
        }
    };
}
