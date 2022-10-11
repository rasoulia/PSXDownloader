using PSXDLL;
using System.IO;
using System.Text.Json;
using WinForm = System.Windows.Forms;

namespace PSXDownloader.MVVM.Data
{
    public class SettingRepository
    {
        public string? LocalFilePath()
        {
            string? path = null;
            WinForm.FolderBrowserDialog fbd = new();
            if (fbd.ShowDialog() == WinForm.DialogResult.OK)
            {
                path = fbd.SelectedPath;
            }
            return path;
        }

        public void SaveSetting(AppConfig? config)
        {
            if (!Directory.Exists("Settings"))
            {
                Directory.CreateDirectory("Settings");
            }

            string? fileName = "Settings\\Settings.json";
            JsonSerializerOptions? options = new() { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(config, options);
            File.WriteAllText(fileName, jsonString);
        }

        public AppConfig? LoadSetting(AppConfig? config)
        {
            string? fileName = "Settings\\Settings.json";
            if (!File.Exists(fileName))
            {
                config!.IsAutoFindFile = true;
                config.Rule = "*.pkg|*.pup|*.xml";
                config.LocalFileDirectory = "D:\\";
                SaveSetting(config);
            }
            string json = File.ReadAllText(fileName);
            AppConfig? settings = JsonSerializer.Deserialize<AppConfig>(json);
            return settings;
        }
    }
}
