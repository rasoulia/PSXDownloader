using PSXDownloader.MVVM.Commands;
using PSXDownloader.MVVM.Data;
using PSXDownloader.MVVM.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace PSXDownloader.MVVM.ViewModels
{
    public class PSXViewModel : ViewModelBase
    {
        private PSXDatabase? _game;

        private ICommand? _addGame;
        private ICommand? _editGame;
        private ICommand? _deleteGame;
        private ICommand? _bulkGame;
        private ICommand? _localDirectory;
        private ICommand? _backup;
        private ICommand? _restore;

        private readonly PSXRepository _repository;

        public PSXViewModel()
        {
            _game = new();
            _repository = new();
        }

        public ObservableCollection<PSXDatabase> GameList => new(_repository.GetAll().Result);

        public PSXDatabase? Game
        {
            get => _game;
            set
            {
                _game = value;
                OnPropertyChanged();
            }
        }

        public ICommand? AddGame
        {
            get
            {
                _addGame ??= new RelayCommand(AddGameCommand, CanAddGameCommand);
                return _addGame;
            }
        }

        private bool CanAddGameCommand(object? obj)
        {
            return Game?.ID < 1 && Game.TitleID?.Length > 0 && Game.Title?.Length > 0 && Directory.Exists(Game.LocalPath);
        }

        private async void AddGameCommand(object? obj)
        {
            await _repository.Create(Game!);
            Game = new();
            OnPropertyChanged(nameof(GameList));
        }

        public ICommand? EditGame
        {
            get
            {
                _editGame ??= new RelayCommand(EditGameCommand, CanEditGameCommand);
                return _editGame;
            }
        }

        private bool CanEditGameCommand(object? obj)
        {
            return Game?.ID > 0 && Game.TitleID?.Length > 0 && Game.Title?.Length > 0 && Directory.Exists(Game.LocalPath);
        }

        private async void EditGameCommand(object? obj)
        {
            MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                await _repository.Update(Game!);
                OnPropertyChanged(nameof(GameList));
            }
            Game = new();
        }

        public ICommand? DeleteGame
        {
            get
            {
                _deleteGame ??= new RelayCommand(DeleteGameCommand, CanDeleteGameCommand);
                return _deleteGame;
            }
        }

        private bool CanDeleteGameCommand(object? obj)
        {
            return Game?.ID > 0;
        }

        private async void DeleteGameCommand(object? obj)
        {
            MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                await _repository.Delete(Game!);
                OnPropertyChanged(nameof(GameList));
            }
            Game = new();
        }

        public ICommand? BulkGame
        {
            get
            {
                _bulkGame ??= new RelayCommand(BulkGameCommand);
                return _bulkGame;
            }
        }

        private async void BulkGameCommand(object? obj)
        {
            string? path = _repository.LocalFilePath();
            if (Directory.Exists(path))
            {
                await _repository.BulkAdd(path);
                OnPropertyChanged(nameof(GameList));
            }
        }

        public ICommand? LocalDirectory
        {
            get
            {
                _localDirectory ??= new RelayCommand(LocalDirectoryCommand);
                return _localDirectory;
            }
        }

        private async void LocalDirectoryCommand(object? obj)
        {
            Game = await _repository.SingleAdd();
            OnPropertyChanged(nameof(Game));
        }

        public ICommand? Backup
        {
            get
            {
                _backup ??= new RelayCommand(BackupCommand);
                return _backup;
            }
        }

        private async void BackupCommand(object? obj)
        {
            await _repository.Backup();
        }

        public ICommand? Restore
        {
            get
            {
                _restore ??= new RelayCommand(RestoreCommand);
                return _restore;
            }
        }

        private async void RestoreCommand(object? obj)
        {
            await _repository.Restore();
        }
    }
}
