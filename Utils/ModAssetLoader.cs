using System.IO;
using System.Reflection;

namespace MoreNPCs.Utils
{
    internal static class ModAssetLoader
    {
        internal static byte[]? LoadBytes(string embeddedResourceName, string relativePathFromModDir)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(embeddedResourceName))
            {
                if (stream != null)
                {
                    using var memory = new MemoryStream();
                    stream.CopyTo(memory);
                    return memory.ToArray();
                }
            }

            var modDir = Path.GetDirectoryName(assembly.Location);
            if (string.IsNullOrEmpty(modDir))
                return null;

            var filePath = Path.Combine(modDir, relativePathFromModDir.Replace('/', Path.DirectorySeparatorChar));
            return File.Exists(filePath) ? File.ReadAllBytes(filePath) : null;
        }
    }
}
