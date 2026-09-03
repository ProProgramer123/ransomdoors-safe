using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;

namespace rans0m
{
    public static class GoldCoinManager
    {
        private const string RegistryValueName = "GoldCoins";
        private const string GoldFolderName = "RANS0M Gold Coins";

        /// <summary>
        /// Gets the predictable folder where the current ransom's .gold files are created.
        /// Keeping the location predictable makes the safe recreation much easier to use.
        /// </summary>
        public static string GetGoldCoinDirectory()
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (string.IsNullOrWhiteSpace(desktop))
                desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (string.IsNullOrWhiteSpace(desktop))
                desktop = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            return Path.Combine(desktop, GoldFolderName);
        }

        /// <summary>
        /// Creates the requested number of .gold files in a predictable folder on the user's Desktop.
        /// Each file contains an encrypted JSON object: {"RANSOM_COIN":"randomString"}.
        /// Paths are stored in the registry for later deletion.
        /// </summary>
        public static int CreateRandomCoins(int count)
        {
            if (count <= 0)
                return 0;

            string targetDir = GetGoldCoinDirectory();
            Directory.CreateDirectory(targetDir);

            List<string> createdPaths = new List<string>();

            for (int i = 0; i < count; i++)
            {
                try
                {
                    string randomString = Guid.NewGuid().ToString("N");
                    Dictionary<string, string> payload = new Dictionary<string, string>
                    {
                        { "RANSOM_COIN", randomString }
                    };

                    string json = JsonSerializer.Serialize(payload);
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                    byte[] encrypted = ProtectedData.Protect(
                        jsonBytes,
                        null,
                        DataProtectionScope.CurrentUser);

                    string fileName = $"{Guid.NewGuid():N}.gold";
                    string fullPath = Path.Combine(targetDir, fileName);

                    File.WriteAllBytes(fullPath, encrypted);
                    createdPaths.Add(fullPath);
                }
                catch
                {
                    // Keep creating the remaining coins if one file fails.
                }
            }

            if (createdPaths.Count > 0)
                AppendToRegistryList(createdPaths);

            return createdPaths.Count;
        }

        /// <summary>
        /// Deletes all .gold files created by this app and removes their registry entries.
        /// </summary>
        public static void DeleteAllCoins()
        {
            List<string>? paths = GetRegistryFileList();

            if (paths != null)
            {
                foreach (string path in paths)
                {
                    try { File.Delete(path); }
                    catch { }
                }
            }

            // Also clean up any .gold files left in the predictable game folder.
            try
            {
                string folder = GetGoldCoinDirectory();
                if (Directory.Exists(folder))
                {
                    foreach (string file in Directory.EnumerateFiles(folder, "*.gold"))
                    {
                        try { File.Delete(file); }
                        catch { }
                    }
                }
            }
            catch { }

            using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\RANSOM"))
            {
                key?.DeleteValue(RegistryValueName, false);
            }
        }

        /// <summary>
        /// Decrypts a .gold file and returns the dictionary inside.
        /// </summary>
        public static Dictionary<string, string> DecryptCoinFile(string filePath)
        {
            byte[] encrypted = File.ReadAllBytes(filePath);
            byte[] decryptedBytes = ProtectedData.Unprotect(
                encrypted,
                null,
                DataProtectionScope.CurrentUser);
            string json = Encoding.UTF8.GetString(decryptedBytes);

            return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                ?? throw new InvalidDataException("Invalid .gold file.");
        }

        private static void AppendToRegistryList(IEnumerable<string> newPaths)
        {
            List<string> existing = GetRegistryFileList() ?? new List<string>();
            existing.AddRange(newPaths);
            SetRegistryFileList(existing);
        }

        private static List<string>? GetRegistryFileList()
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\RANSOM"))
            {
                if (key == null) return null;

                object? value = key.GetValue(RegistryValueName);
                if (value is string[] multiString)
                    return new List<string>(multiString);

                return null;
            }
        }

        private static void SetRegistryFileList(List<string> paths)
        {
            using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\RANSOM"))
            {
                key?.SetValue(
                    RegistryValueName,
                    paths.ToArray(),
                    RegistryValueKind.MultiString);
            }
        }
    }
}