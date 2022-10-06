using PSXDLL;
using PSXDownloader.MVVM.Commands;
using PSXDownloader.MVVM.Data;
using PSXDownloader.MVVM.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace PSXDownloader.MVVM.ViewModels
{
    public class CSViewModel : ViewModelBase
    {
        #region Comment
        //private List<string>? _link;
        //private ConnectModel? _connectModel;
        //private LogModel? _logModel;
        //private PSXDatabase? _game;
        //private ICommand? _connect;
        //private ICommand? _add;
        //private ICommand? _delete;
        //private ICommand? _filePath;
        //private ICommand? _saveSetting;
        //private ICommand? _createDatabase;
        //private ICommand? _updateDatabase;
        //private ICommand? _deleteDatabase;
        //private ICommand? _localPkg;
        //private readonly CSRepository _repository;
        //private readonly PSXRepository<PSXDatabase> _psx;
        //public CSViewModel()
        //{
        //    _connectModel = new();
        //    _logModel = new();
        //    _game = new();
        //    _repository = new();
        //    _psx = new();
        //    AppConfig? settings = _repository.LoadSetting(AppConfig.Instance());
        //    IsAutoFind = settings!.IsAutoFindFile;
        //    Rules = settings!.Rule;
        //    LocalDirectory = settings!.LocalFileDirectory;
        //    AllUrl.CollectionChanged += AllUrl_CollectionChanged;
        //    AllIPList.CollectionChanged += AllIPList_CollectionChanged;
        //    AllGame.CollectionChanged += AllGame_CollectionChanged;
        //}

        //public ObservableCollection<LogModel> AllUrl => new(_repository.GetAll());
        //public ObservableCollection<IPAddress> AllIPList => new(_repository.AllIP());
        //public ObservableCollection<PSXDatabase> AllGame => new(_psx.GetAll().Result);

        //private void AllGame_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    OnPropertyChanged(nameof(AllGame));
        //}

        //private void AllIPList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    OnPropertyChanged(nameof(AllIPList));
        //}

        //private void AllUrl_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    OnPropertyChanged(nameof(AllUrl));
        //}

        //public ConnectModel? ConnectModel
        //{
        //    get => _connectModel;
        //    set
        //    {
        //        _connectModel = value;
        //        OnPropertyChanged(nameof(ConnectModel));
        //    }
        //}

        //public LogModel? LogModel
        //{
        //    get => _logModel;
        //    set
        //    {
        //        _logModel = value;
        //        OnPropertyChanged(nameof(LogModel));
        //    }
        //}

        //public PSXDatabase? Game
        //{
        //    get => _game;
        //    set
        //    {
        //        _game = value;
        //        OnPropertyChanged(nameof(Game));
        //    }
        //}

        //public string? LocalDirectory
        //{
        //    get => AppConfig.Instance().LocalFileDirectory;
        //    set
        //    {
        //        AppConfig.Instance().LocalFileDirectory = value;
        //        OnPropertyChanged(nameof(LocalDirectory));
        //    }
        //}

        //public string? Rules
        //{
        //    get => AppConfig.Instance().Rule;
        //    set
        //    {
        //        AppConfig.Instance().Rule = value;
        //        OnPropertyChanged(nameof(Rules));
        //    }
        //}

        //public bool IsAutoFind
        //{
        //    get => AppConfig.Instance().IsAutoFindFile;
        //    set
        //    {
        //        AppConfig.Instance().IsAutoFindFile = value;
        //        OnPropertyChanged(nameof(IsAutoFind));
        //    }
        //}

        //public ICommand? Connect
        //{
        //    get
        //    {
        //        _connect ??= new RelayCommands(ConnectMethod);
        //        return _connect;
        //    }
        //}

        //private void AddUrl(UrlInfo urlinfo)
        //{
        //    string? url = urlinfo.PsnUrl?.Split("?")[0];
        //    if (url!.ToLower().Contains(".pkg"))
        //    {
        //        _repository.Add(url);
        //        if (!_link!.Contains(url))
        //        {
        //            _link.Add(url);
        //            LocalDirectory = Task.Run(async () => await _psx.LocalDirectory(url)).Result;
        //            OnPropertyChanged(nameof(LocalDirectory));
        //        }
        //        OnPropertyChanged(nameof(AllUrl));
        //    }
        //}

        //private void ConnectMethod(object obj)
        //{
        //    _link = new();
        //    _repository.Connect(ConnectModel!.IP!, ConnectModel.Port, AddUrl);
        //}

        //public ICommand? Add
        //{
        //    get
        //    {
        //        _add ??= new RelayCommands(AddMethod);
        //        return _add;
        //    }
        //}

        //private void AddMethod(object obj)
        //{
        //    _repository.Add(LogModel!.PsnUrl!);
        //    OnPropertyChanged(nameof(AllUrl));
        //}

        //public ICommand? Delete
        //{
        //    get
        //    {
        //        _delete ??= new RelayCommands(DeleteMethod);
        //        return _delete;
        //    }
        //}

        //private void DeleteMethod(object obj)
        //{
        //    _repository.Delete();
        //    OnPropertyChanged(nameof(AllUrl));
        //}

        //public ICommand? FilePath
        //{
        //    get
        //    {
        //        _filePath ??= new RelayCommands(FilePathMethod);
        //        return _filePath;
        //    }
        //}

        //private void FilePathMethod(object obj)
        //{
        //    string? filePath = _repository.FilePath();
        //    if (filePath != null)
        //    {
        //        LocalDirectory = filePath;
        //        OnPropertyChanged(nameof(LocalDirectory));
        //    }
        //}

        //public ICommand? SaveSetting
        //{
        //    get
        //    {
        //        _saveSetting ??= new RelayCommands(SaveSettingMethod);
        //        return _saveSetting;
        //    }
        //}

        //private void SaveSettingMethod(object obj)
        //{
        //    _repository.SaveSettins(AppConfig.Instance());
        //}

        //public ICommand? CreateDatabase
        //{
        //    get
        //    {
        //        _createDatabase ??= new RelayCommands(CreateDatabaseMethod, CanCreateDatabaseMethod);
        //        return _createDatabase;
        //    }
        //}

        //private bool CanCreateDatabaseMethod(object obj)
        //{
        //    return Game?.ID < 1;
        //}

        //private async void CreateDatabaseMethod(object obj)
        //{
        //    await _psx.Create(Game!);
        //    OnPropertyChanged(nameof(AllGame));
        //    Game = new();
        //}

        //public ICommand? UpdateDatabase
        //{
        //    get
        //    {
        //        _updateDatabase ??= new RelayCommands(UpdateDatabaseMethod, CanUpdateDatabaseMethod);
        //        return _updateDatabase;
        //    }
        //}

        //private bool CanUpdateDatabaseMethod(object obj)
        //{
        //    return Game?.ID > 0;
        //}

        //private async void UpdateDatabaseMethod(object obj)
        //{
        //    MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning", MessageBoxButton.YesNo);
        //    if (result == MessageBoxResult.Yes)
        //    {
        //        await _psx.Update(Game!);
        //        OnPropertyChanged(nameof(AllGame));
        //    }
        //    Game = new();
        //}

        //public ICommand? DeleteDatabase
        //{
        //    get
        //    {
        //        _deleteDatabase ??= new RelayCommands(DeleteDatabaseMethod, CanDeleteDatabaseMethod);
        //        return _deleteDatabase;
        //    }
        //}

        //private bool CanDeleteDatabaseMethod(object obj)
        //{
        //    return Game?.ID > 0;
        //}

        //private async void DeleteDatabaseMethod(object obj)
        //{
        //    MessageBoxResult result = MessageBox.Show("Are You Sure?", "Warning", MessageBoxButton.YesNo);
        //    if (result == MessageBoxResult.Yes)
        //    {
        //        await _psx.Delete(Game!);
        //        OnPropertyChanged(nameof(AllGame));
        //    }
        //    Game = new();
        //}

        //public ICommand? LocalPkg
        //{
        //    get
        //    {
        //        _localPkg ??= new RelayCommands(LocalPkgMethod);
        //        return _localPkg;
        //    }
        //}

        //private async void LocalPkgMethod(object obj)
        //{
        //    string? path = null;
        //    WinForm.FolderBrowserDialog fbd = new();
        //    if (fbd.ShowDialog() == WinForm.DialogResult.OK)
        //    {
        //        path = fbd.SelectedPath;
        //        await _psx.Add(path);
        //        OnPropertyChanged(nameof(AllGame));
        //    }
        //}
        #endregion
        
    }
}
