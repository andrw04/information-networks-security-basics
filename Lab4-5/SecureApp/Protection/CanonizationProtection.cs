using System.IO;

namespace SecureApp.Protection;

public static class CanonizationProtection
{
    public static bool IsValidFilePath(string filePath)
    {
        string[] components = filePath.Split(Path.DirectorySeparatorChar);
        foreach (var component in components)
        {
            if (component.Contains(".."))
            {
                return false;
            }
        }

        return true;
    }
}