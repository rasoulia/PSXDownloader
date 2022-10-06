using PSXDLL;
using PSXDLL.BLL;
using PSXDLL.Model;
using PSXDownloader.MVVM.Commands;
using PSXDownloader.MVVM.Data;
using PSXDownloader.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSXDownloader.MVVM.ViewModels
{
    public class ConnectViewModel : ViewModelBase
    {
        private IPAddress? _address;
        private int _port;
        private string? _psnUrl;

        private ICommand? _connect;
        private ICommand? _clearLog;

        private readonly ConnectRepository _repository;

        public ConnectViewModel()
        {
            _repository = new();

            Port = 8080;
        }

        public ObservableCollection<PsnUrlLog> PsnUrlList => new(_repository.GetAllUrl());

        public List<IPAddress> AddressList => ProxyServer.GetHostIp().ToList();

        public IPAddress? Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }

        public string? PsnUrl
        {
            get => _psnUrl;
            set
            {
                _psnUrl = value;
                OnPropertyChanged();
            }
        }

        public string? LocalFileDirectory
        {
            get => AppConfig.Instance().LocalFileDirectory;
            set
            {
                AppConfig.Instance().LocalFileDirectory = value;
                OnPropertyChanged();
            }
        }

        public ICommand? Connect
        {
            get
            {
                _connect ??= new RelayCommands(ConnectMethod);
                return _connect;
            }
        }

        private void AddUrl(UrlInfo ui)
        {
            PsnUrl = ui.PsnUrl?.Split('?')[0];
            _repository.AddLog(PsnUrl!);
            OnPropertyChanged(nameof(PsnUrlList));
            if (PsnUrl!.EndsWith("pkg"))
            {
                Uri? uri = new(PsnUrl);
                string titleID = Path.GetFileName(uri.LocalPath).Split('-')[1].Replace("_00", "");
                LocalFileDirectory = Task.Run(async () => await _repository.GetLocalPath(titleID)).Result;
            }
        }

        private void ConnectMethod(object obj)
        {
            _repository.Connect(Address!, Port, AddUrl);
        }

        public ICommand? ClearLog
        {
            get
            {
                _clearLog ??= new RelayCommands(ClearLogMethod);
                return _clearLog;
            }
        }

        private void ClearLogMethod(object obj)
        {
            _repository.ClearLog();
            OnPropertyChanged(nameof(PsnUrlList));
        }
    }
}
