using System;
using System.Collections.Generic;
using System.Text.Json;

namespace TheCure
{
    public abstract record SettingKey(string Group, string Name);

    public sealed record SettingKey<T>(string Group, string Name) : SettingKey(Group, Name);

    public static class Settings
    {
        private static Dictionary<string, Dictionary<string, JsonElement>> SettingsDictionary { get; set; } = new();

        public static void Load()
        {
            var savedSettings = JsonHelper.Load<Dictionary<string, Dictionary<string, JsonElement>>>();

            SettingsDictionary = savedSettings ?? new();
        }

        public static void Save<T>(SettingKey<T> settingKey, T value)
        {
            if (!SettingsDictionary.TryGetValue(settingKey.Group, out var groupSettings))
            {
                groupSettings = new Dictionary<string, JsonElement>();
                SettingsDictionary[settingKey.Group] = groupSettings;
            }

            groupSettings[settingKey.Name] = JsonSerializer.SerializeToElement(value);

            JsonHelper.Save(SettingsDictionary);
        }

        public static T GetValue<T>(SettingKey<T> settingKey, bool useDefault = false)
        {
            
            if (SettingsDictionary.TryGetValue(settingKey.Group, out var groupSettings) && !useDefault)
            {
                if (groupSettings.TryGetValue(settingKey.Name, out var value))
                    return value.Deserialize<T>()!;
            }

            foreach (var kvp in DefaultSettings.Values)
            {
                if (kvp.Key.Group == settingKey.Group && kvp.Key.Name == settingKey.Name)
                    return (T)kvp.Value;
            }

            throw new Exception($"No value or default found for {settingKey.Group}.{settingKey.Name}");
        }
    }
}