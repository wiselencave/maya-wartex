/*
 * At runtime, modify the global MEL variable $gFileDialogFilterTypes[4]
 * to add the extension to the Image Types list.
 * 
 * Maya2xxx\scripts\others\fileDialogFilterTypes.mel:
 * $gFileDialogFilterTypes[4] = ($ImageFiles + " (*.map *.pix *.als *.ALS *.jpeg *.JPEG *.jpg *.JPG *.pntg *.PNTG *.ps *.PS *.png *.PNG *.psd *.PSD *.pict *.PICT *.tx *.TX *.tex *.TEX");
		$gFileDialogFilterTypes[4] = ($gFileDialogFilterTypes[4] + " *.ptx");
		$gFileDialogFilterTypes[4] = ($gFileDialogFilterTypes[4] + " *.qt *.QT *.qtif *.QTIF *.sgi *.SGI *.tga *.TGA *.tif *.TIF *.bmp *.BMP *.tiff *.TIFF *.iff *.IFF *.rgb *.RGB *.tdi *.TDI *.gif *.GIF *.exr *.EXR *.xpm *.XPM *.hdr *.HDR *.dds *.DDS)");
 */

using Autodesk.Maya.OpenMaya;

namespace WarTex
{
    public static class FileDialog
    {
        private const string ArrayName = "$gFileDialogFilterTypes[]";
        private const string CellName = "$gFileDialogFilterTypes[4]";

        public static bool RegisterExtensionAsImageType(string ext)
        {
            if (string.IsNullOrWhiteSpace(ext) || ext.Any(char.IsWhiteSpace))
                return false;

            var str = GetRawImageTypes();
            var (lower, upper) = NormalizeExtension(ext);

            str = InsertExtension(str, lower);
            str = InsertExtension(str, upper);

            return TryUpdateImageTypes(str);
        }

        private static string InsertExtension(string target, string ext)
        {
            if (target.Contains($" *.{ext}", StringComparison.Ordinal))
                return target;
            
            var str = target.Replace(")", $" *.{ext})");
            return str;
        }

        public static bool UnregisterExtensionAsImageType(string ext)
        {
            if (string.IsNullOrWhiteSpace(ext))
                return false;

            var str = GetRawImageTypes();
            var (lower, upper) = NormalizeExtension(ext);

            str = RemoveExtension(str, lower);
            str = RemoveExtension(str, upper);

            return TryUpdateImageTypes(str);
        }

        private static string RemoveExtension(string target, string ext)
        {
            return target.Replace($" *.{ext}", "");
        }

        private static (string, string) NormalizeExtension(string ext)
        {
            var normExt = ext.Replace(".", "");
            return (normExt.ToLower(), normExt.ToUpper());
        }

        private static string GetRawImageTypes()
        {
            // Internal MEL procedure buildImageFileFilterList() appends "All Files (*.*)",
            // so we construct our own MEL proc to return raw image types

            var procName = "wt_getRawImageTypes()";
            var command = $"proc string {procName} {{global string {ArrayName}; return {CellName};}}; {procName};";
            return MGlobal.executeCommandStringResult(command);
        }

        private static bool TryUpdateImageTypes(string imageTypes)
        {
            try
            {
                MGlobal.executeCommand($"{CellName} = \"{imageTypes}\";");
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
