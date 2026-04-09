using System;
using System.IO;
using System.Text.Json;

namespace TheCure
{
    public static class JsonHelper
    {
        private const string DefaultFileName = "Settings.json";
        
        public static void Save<T>(T data, string fileName = DefaultFileName)
        {
            string path = GetPath(fileName);
            string? directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }

        public static T? Load<T>(string fileName = DefaultFileName)
        {
            string path = GetPath(fileName);

            if (!File.Exists(path))
            {
                return default;
            }

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json);
        }

        private static string GetPath(string fileName)
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../Settings", fileName));
        }
    }
}