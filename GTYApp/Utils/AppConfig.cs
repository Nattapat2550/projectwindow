using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace GTYApp.Utils
{
    public static class AppConfig
    {
        public static JsonObject JsonRoot { get; private set; } = [];

        public static void Load()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(path)) { JsonRoot = []; return; }
            var json = File.ReadAllText(path);
            JsonRoot = (JsonNode.Parse(json) as JsonObject) ?? [];
        }

        public static void LoadOrThrow()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(path))
                throw new FileNotFoundException("appsettings.json not found in " + AppContext.BaseDirectory);
            var json = File.ReadAllText(path);
            var obj = JsonNode.Parse(json) as JsonObject ?? throw new InvalidDataException("appsettings.json cannot be parsed.");
            JsonRoot = obj;
        }

        public static string? GetString(params string[] path)
        {
            JsonNode? node = JsonRoot;
            foreach (var p in path)
            {
                if (node is null) return null;
                node = (node as JsonObject)?[p];
            }
            return node?.GetValue<string>();
        }
    }
}
