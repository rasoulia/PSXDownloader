using PSXDownloader.MVVM.Commands;
using PSXDownloader.MVVM.Data;
using PSXDownloader.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using PSXDLL;

namespace PSXDownloader.MVVM.ViewModels
{
    public class PSXViewModel:ViewModelBase
    {
        private PSXDatabase? _game;



        private ICommand? _addGame;
        private ICommand? _editGame;
        private ICommand? _deleteGame;
        private ICommand? _bulkGame;

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
                _addGame ??= new RelayCommands(AddGameMethod, CanAddGameMethod);
                return _addGame;
            }
        }

        private bool CanAddGameMethod(object obj)
        {
            return Game?.ID < 1 && Game.TitleID?.Length > 0 && Game.Title?.Length > 0 && Directory.Exists(Game.LocalPath);
        }

        private async void AddGameMethod(object obj)
        {
            await _repository.Create(Game!);
            Game = new();
            OnPropertyChanged(nameof(GameList));

        }

        public ICommand? EditGame
        {
            get
            {
                _editGame ??= new RelayCommands(EditGameMethod, CanEditGameMethod);
                return _editGame;
            }
        }

        private bool CanEditGameMethod(object obj)
        {
            return Game?.ID > 0 && Game.TitleID?.Length > 0 && Game.Title?.Length > 0 && Directory.Exists(Game.LocalPath);
        }

        private async void EditGameMethod(object obj)
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
                _deleteGame ??= new RelayCommands(DeleteGameMethod, CanDeleteGameMethod);
                return _deleteGame;
            }
        }

        private bool CanDeleteGameMethod(object obj)
        {
            return Game?.ID > 0;
        }

        private async void DeleteGameMethod(object obj)
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
                _bulkGame ??= new RelayCommands(BulkGameMethod);
                return _bulkGame;
            }
        }

        private async void BulkGameMethod(object obj)
        {
            string? path = _repository.LocalFilePath();
            if (Directory.Exists(path))
            {
                await _repository.BulkAdd(path);
                OnPropertyChanged(nameof(GameList));
            }
        }
    }
}
