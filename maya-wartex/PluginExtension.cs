using Autodesk.Maya.OpenMaya;
using WarTex;

[assembly: ExtensionPlugin(typeof(PluginExtension), "Any")]
namespace WarTex
{
    class PluginExtension : IExtensionPlugin
    {
        public bool InitializePlugin()
        {
            if (FileDialog.RegisterExtensionAsImageType("blp"))
            {
                MGlobal.displayInfo("BLP extension is registered as image type");
                return true;
            }
            MGlobal.displayError("Error while registering BLP extension");
            return false;
        }

        public bool UninitializePlugin()
        {
            if (FileDialog.UnregisterExtensionAsImageType("blp"))
            {
                MGlobal.displayInfo("BLP extension is unregistered as image type");
                return true;
            }
            MGlobal.displayError("Error while unregistering BLP extension");
            return false;
        }

        public string GetMayaDotNetSdkBuildVersion()
        {
            // no idea, got from API usage examples
            return "201353";
        }
    }
}
