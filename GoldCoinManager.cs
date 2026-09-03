using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;

namespace rans0m
{
    public static class GoldCoinManager
    {
        private const string RegistryValueName = "GoldCoins";

        /// <summary>
        /// Creates .gold files in randomly selected user folders/subfolders.
        /// The safe version never writes outside the current user's profile.
        /// </summary>
        public static int CreateRandomCoins(int count)
        {
            if (count <= 0)
                return 0;

            List<string> baseDirs = GetUserDirectories();
            if (baseDirs.Count == 0)
                return 0;

            List<string> createdPaths = new List<string>();

            for (int i = 0; i < count; i++)
            {
                bool created = false;

                for (int attempt = 0; attempt < baseDirs.Count * 3 && !created; attempt++)
                {
                    try
                    {
                        string baseDir = baseDirs[Global.rng.Next(baseDirs.Count)];
                        string targetDir = GetRandomSubfolder(baseDir);
                        Directory.CreateDirectory(targetDir);

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
                        created = true;
                    }
                    catch
                    {
                        // Try another random user location.
                    }
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

        private static List<string> GetUserDirectories()
        {
            var dirs = new List<string>();

            Environment.SpecialFolder[] specialFolders =
            {
                Environment.SpecialFolder.Desktop,
                Environment.SpecialFolder.MyDocuments,
                Environment.SpecialFolder.MyPictures,
                Environment.SpecialFolder.MyMusic,
                Environment.SpecialFolder.MyVideos,
                Environment.SpecialFolder.UserProfile
            };

            foreach (Environment.SpecialFolder sf in specialFolders)
            {
                try
                {
                    string path = Environment.GetFolderPath(sf);
                    if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
                        dirs.Add(path);
                }
                catch { }
            }

            try
            {
                string downloads = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads");

                if (Directory.Exists(downloads))
                    dirs.Add(downloads);
            }
            catch { }

            return dirs.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static string GetRandomSubfolder(string root)
        {
            try
            {
                string[] subDirs = Directory.GetDirectories(root);
                if (subDirs.Length > 0)
                    return subDirs[Global.rng.Next(subDirs.Length)];
            }
            catch { }

            return root;
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