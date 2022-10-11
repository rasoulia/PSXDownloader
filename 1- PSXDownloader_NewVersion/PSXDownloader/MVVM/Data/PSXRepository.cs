using Microsoft.EntityFrameworkCore;
using PSXDownloader.MVVM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WinForm = System.Windows.Forms;

namespace PSXDownloader.MVVM.Data
{
    public class PSXRepository
    {
        public async Task BulkAdd(string? path)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            IEnumerable<string> folders = Directory.EnumerateDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string folder in folders)
            {
                try
                {
                    IEnumerable<string> files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith("_0.pkg", StringComparison.OrdinalIgnoreCase));
                    foreach (string file in files)
                    {
                        string? titleID = Path.GetFileNameWithoutExtension(file).Split('-')[1].Replace("_00", "");
                        string? title = Directory.GetParent(file)?.Name;
                        string? localPath = Directory.GetParent(file)?.FullName;
                        PSXDatabase? db = new()
                        {
                            Title = title,
                            TitleID = titleID,
                            LocalPath = localPath,
                        };
                        if (!IsExist(titleID))
                        {
                            await Create(db);
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        public async Task Create(PSXDatabase Entity)
        {
            using PSXDataContext dataContext = new();
            await dataContext.Set<PSXDatabase>().AddAsync(Entity);
            await dataContext.SaveChangesAsync();
        }

        public async Task Update(PSXDatabase Entity)
        {
            using PSXDataContext dataContext = new();
            dataContext.Set<PSXDatabase>().Update(Entity);
            await dataContext.SaveChangesAsync();
        }

        public async Task Delete(PSXDatabase Entity)
        {
            using PSXDataContext dataContext = new();
            dataContext.Set<PSXDatabase>().Remove(Entity);
            await dataContext.SaveChangesAsync();
        }

        public async Task<List<PSXDatabase>> GetAll()
        {
            using PSXDataContext dataContext = new();
            List<PSXDatabase> entities = await dataContext.Set<PSXDatabase>().ToListAsync();
            return await Task.FromResult(entities);
        }

        public bool IsExist(string? titleID)
        {
            using PSXDataContext dataContext = new();
            PSXDatabase? local = dataContext.Set<PSXDatabase>().FirstOrDefault(s => s.TitleID == titleID);
            return local != null;
        }

        public async Task<string> GetLocalPath(string? titleID)
        {
            using PSXDataContext dataContext = new();
            PSXDatabase? local = dataContext.Set<PSXDatabase>().FirstOrDefault(s => s.TitleID == titleID);
            if (local != null)
            {
                return await Task.FromResult(local.LocalPath!);
            }
            return await Task.FromResult(string.Empty);
        }

        public async Task<string> LocalDirectory(string url)
        {
            Uri uri = new(url);
            string titleID = Path.GetFileName(uri.LocalPath).Split('-')[1].Replace("_00", "");

            return await GetLocalPath(titleID);

        }

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

        public async Task Backup()
        {
            try
            {
                if (!Directory.Exists("Backup"))
                {
                    Directory.CreateDirectory("Backup");
                }
                using PSXDataContext dataContext = new();
                List<PSXDatabase> entities = await dataContext.Set<PSXDatabase>().ToListAsync();
                JsonSerializerOptions? options = new() { WriteIndented = true };
                string? json = JsonSerializer.Serialize(entities, options);
                string time = TimeOnly.FromDateTime(DateTime.Now).ToString().Replace(":", "-");
                string backup = $"Backup\\{time}.json";
                using StreamWriter sw = new(backup);
                sw.WriteLine(json);
                MessageBox.Show("Done");
            }
            catch
            {
                MessageBox.Show("Faild");
            }
        }

        public async Task Restore()
        {
            if (!Directory.Exists("Backup"))
            {
                Directory.CreateDirectory("Backup");
            }
            WinForm.OpenFileDialog ofd = new()
            {
                Filter = "Json files (*.*)|*.json",
                InitialDirectory = "Backup\\Game",
            };
            if (ofd.ShowDialog() == WinForm.DialogResult.OK)
            {
                string? json = File.ReadAllText(ofd.FileName);
                try
                {
                    List<PSXDatabase>? entities = JsonSerializer.Deserialize<List<PSXDatabase>>(json);
                    foreach (PSXDatabase? entity in entities!)
                    {
                        if (!IsExist(entity.TitleID))
                        {
                            await Create(entity);
                        }
                    }
                    MessageBox.Show("Done");
                }
                catch (Exception)
                {
                    MessageBox.Show("Faild");
                }
            }
        }
    }
}
