namespace cs2typescript
{
    internal class Program
    {
        public static FileSystemWatcher FileWatcher(string filePath)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(filePath));
            watcher.Filter = Path.GetFileName(filePath);
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += (sender, e) =>
            {
                if (e.FullPath == filePath)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} File {Path.GetFileName(filePath)} has been changed");
                    string newPath = filePath.Replace("\\content\\", "\\game\\");
                    string pathDir = Path.GetDirectoryName(newPath);
                    if (!Directory.Exists(pathDir)) Directory.CreateDirectory(pathDir);
                    CS2TypeScript fakeScript = new CS2TypeScript(filePath, newPath);
                }
            };

            watcher.EnableRaisingEvents = true;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} Tracking file changes {Path.GetFileName(filePath)}.");
            return watcher;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Enter path to content addon \n(example F:\\SteamLibrary\\steamapps\\common\\Counter-Strike Global Offensive\\content\\csgo_addons\\example_addon\\): ");
            string pathToContentAddon;
            if (args.Length > 0)
            {
                pathToContentAddon = args[0];
            }
            else
            {
                pathToContentAddon = Console.ReadLine();
            }
            if (pathToContentAddon == null) return;
            if (pathToContentAddon.Length == 0) pathToContentAddon = "F:\\SteamLibrary\\steamapps\\common\\Counter-Strike Global Offensive\\content\\csgo_addons\\squid_game\\";
            if (pathToContentAddon[pathToContentAddon.Length - 1] != '\\') pathToContentAddon += '\\';
            string pathToAddon = pathToContentAddon.Replace("\\content\\", "\\game\\");
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