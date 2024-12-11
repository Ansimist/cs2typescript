using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading;

namespace cs2typescript
{
    internal class Program
    {
        public class Config
        {
            public string PathToContentAddon { get; set; }
        }

        public static FileSystemWatcher FileWatcher(string filePath)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(filePath));
            watcher.Filter = Path.GetFileName(filePath);
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += (sender, e) =>
            {
                if (e.FullPath == filePath)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} File {Path.GetFileName(filePath)} has been changed");
                    string newPath = filePath.Replace("\\content\\", "\\game\\");
                    string pathDir = Path.GetDirectoryName(newPath);
                    if (!Directory.Exists(pathDir)) Directory.CreateDirectory(pathDir);
                    CS2TypeScript fakeScript = new CS2TypeScript(filePath, newPath);
                }
            };

            watcher.EnableRaisingEvents = true;
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Tracking file changes {Path.GetFileName(filePath)}.");
            return watcher;
        }

        static void Main(string[] args)
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            string pathToContentAddon = null;

            if (File.Exists(configFilePath))
            {
                try
                {
                    string configContent = File.ReadAllText(configFilePath);
                    Config config = JsonSerializer.Deserialize<Config>(configContent);
                    if (config != null && !string.IsNullOrEmpty(config.PathToContentAddon))
                    {
                        pathToContentAddon = config.PathToContentAddon;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to read or parse config.json: {ex.Message}");
                }
            }

            if (string.IsNullOrEmpty(pathToContentAddon))
            {
                Console.WriteLine("Enter path to content addon \n(example F:\\SteamLibrary\\steamapps\\common\\Counter-Strike Global Offensive\\content\\csgo_addons\\example_addon\\): ");
                pathToContentAddon = args.Length > 0 ? args[0] : Console.ReadLine();
            }

            if (string.IsNullOrEmpty(pathToContentAddon)) return;
            if (pathToContentAddon[pathToContentAddon.Length - 1] != '\\') pathToContentAddon += '\\';

            if (!Directory.Exists(pathToContentAddon))
            {
                Console.WriteLine("Wrong path!");
                Main(args);
                return;
            }

            pathToContentAddon = pathToContentAddon.Replace("\\game\\csgo_addons", "\\content\\csgo_addons");
            string pathToAddon = pathToContentAddon.Replace("\\content\\csgo_addons", "\\game\\csgo_addons");
            Console.WriteLine($"Path to content addon: {pathToContentAddon}");
            Console.WriteLine($"Path to game addon: {pathToAddon}");
            string pathToContentScriptsFolder = pathToContentAddon + "scripts\\";
            string pathToContentVScriptsFolder = pathToContentAddon + "scripts\\vscripts";
            string pathToScriptsFolder = pathToAddon + "scripts\\";
            string pathToVScriptsFolder = pathToAddon + "scripts\\vscripts";

            if (!Directory.Exists(pathToContentScriptsFolder)) Directory.CreateDirectory(pathToContentScriptsFolder);
            if (!Directory.Exists(pathToContentVScriptsFolder)) Directory.CreateDirectory(pathToContentVScriptsFolder);
            if (!Directory.Exists(pathToScriptsFolder)) Directory.CreateDirectory(pathToScriptsFolder);
            if (!Directory.Exists(pathToVScriptsFolder)) Directory.CreateDirectory(pathToVScriptsFolder);

            Console.WriteLine($"Move the .vts files to the folder: {pathToContentVScriptsFolder}\n");

            foreach (var file in GetFiles(pathToContentScriptsFolder))
            {
                Console.WriteLine(file);
                new CS2TypeScript(file, file.Replace("\\content\\", "\\game\\"));
            }

            Dictionary<string, FileSystemWatcher> pathWatchers = new Dictionary<string, FileSystemWatcher>();
            while (true)
            {
                foreach (var file in GetFiles(pathToContentScriptsFolder))
                {
                    if (!pathWatchers.ContainsKey(file))
                    {
                        pathWatchers.Add(file, FileWatcher(file));
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public static string[] GetFiles(string path)
        {
            string[] files = Directory.GetFiles(path, "*.vts", SearchOption.AllDirectories);
            return files;
        }
    }
}
