using System.Runtime.InteropServices;
using System.Text.Json;

namespace cs2typescript
{
    internal class Program
    {
        public static bool MinifyJS { get; set; } = false;
        
        public class Config
        {
            public string? PathToContentAddon { get; set; }
            public string? MinifyJS { get; set; } = "false";
        }

        public static FileSystemWatcher FileWatcher(string filePath)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(filePath)!);
            watcher.Filter = Path.GetFileName(filePath);
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += (sender, e) =>
            {
                if (e.FullPath == filePath)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} File {Path.GetFileName(filePath)} has been changed");
                    string newPath = filePath.Replace("\\content\\", "\\game\\");
                    string pathDir = Path.GetDirectoryName(newPath)!;
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
            EnableAnsiSupport();
            PrintCon($"\x1b[38;2;91;206;250mCS2TypeScript \x1b[38;2;100;100;100m{DateTime.Now:HH:mm:ss}\n\x1b[38;2;91;206;250mVersion: \x1b[38;2;100;100;100m{typeof(Program).Assembly.GetName().Version!.ToString(3)}.");
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            string? pathToContentAddon = null;

            if (File.Exists(configFilePath))
            {
                try
                {
                    string configContent = File.ReadAllText(configFilePath);
                    Config config = JsonSerializer.Deserialize<Config>(configContent)!;
                    if (config != null && !string.IsNullOrEmpty(config.PathToContentAddon))
                    {
                        pathToContentAddon = config.PathToContentAddon;
                    }

                    if (config != null && !string.IsNullOrEmpty(config.MinifyJS))
                    {
                        MinifyJS = bool.Parse(config.MinifyJS);
                    }
                }
                catch (Exception ex)
                {
                    PrintCon($"Failed to read or parse config.json: {ex.Message}");
                }
            }

            if (string.IsNullOrEmpty(pathToContentAddon))
            {
                PrintCon("Enter path to content addon \n(example F:\\SteamLibrary\\steamapps\\common\\Counter-Strike Global Offensive\\content\\csgo_addons\\example_addon\\): ");
                pathToContentAddon = args.Length > 0 ? args[0] : Console.ReadLine()!;
            }

            if (string.IsNullOrEmpty(pathToContentAddon)) return;
            if (pathToContentAddon[pathToContentAddon.Length - 1] != '\\') pathToContentAddon += '\\';

            if (!Directory.Exists(pathToContentAddon))
            {
                PrintCon("Wrong path! Check config.json!");
                Thread.Sleep(1000);
                Environment.Exit(1);
            }

            pathToContentAddon = pathToContentAddon.Replace("\\game\\csgo_addons", "\\content\\csgo_addons");
            string pathToAddon = pathToContentAddon.Replace("\\content\\csgo_addons", "\\game\\csgo_addons");
            string pathToContentScriptsFolder = pathToContentAddon + "scripts\\";
            string pathToContentVScriptsFolder = pathToContentAddon + "scripts\\vscripts";
            string pathToScriptsFolder = pathToAddon + "scripts\\";
            string pathToVScriptsFolder = pathToAddon + "scripts\\vscripts";

            if (!Directory.Exists(pathToContentScriptsFolder)) Directory.CreateDirectory(pathToContentScriptsFolder);
            if (!Directory.Exists(pathToContentVScriptsFolder)) Directory.CreateDirectory(pathToContentVScriptsFolder);
            if (!Directory.Exists(pathToScriptsFolder)) Directory.CreateDirectory(pathToScriptsFolder);
            if (!Directory.Exists(pathToVScriptsFolder)) Directory.CreateDirectory(pathToVScriptsFolder);

            PrintCon($"Move the .vts files to the folder:\n{pathToContentVScriptsFolder}\n");

            foreach (var file in GetFiles(pathToContentScriptsFolder))
            {
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
            string[] tsFiles = Directory.GetFiles(path, "*.ts", SearchOption.AllDirectories);
            string[] jsFiles = Directory.GetFiles(path, "*.js", SearchOption.AllDirectories);
            string[] vtsFiles = Directory.GetFiles(path, "*.vts", SearchOption.AllDirectories);

            string[] allFiles = tsFiles.Concat(jsFiles).Concat(vtsFiles).ToArray();

            return allFiles;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        private const int STD_OUTPUT_HANDLE = -11;
        private const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        private static void EnableAnsiSupport()
        {
            IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (!GetConsoleMode(handle, out int mode))
            {
                return;
            }
            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            SetConsoleMode(handle, mode);
        }

        public static void PrintCon(string msg, int type = 0)
        {
            string prefix = type == 0 ? "\x1b[38;2;100;100;100m" : $"\x1b[38;2;91;206;250m[CS2TypeScript {DateTime.Now:HH:mm:ss}] \x1b[38;2;245;169;184m";
            Console.WriteLine($"{prefix}{msg}");
        }
    }
}