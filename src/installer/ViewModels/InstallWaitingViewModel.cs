﻿using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Windows.Input;

namespace PicoTorrentBootstrapper.ViewModels
{
    public sealed class InstallWaitingViewModel : PropertyNotifyBase
    {
        private readonly BootstrapperApplication _bootstrapper;

        private bool _addWindowsFirewallException;
        private bool _downloadDependencies;
        private bool _registerFileProtocolHandlers;
        private string _installLocation;
        private ICommand _changeInstallLocationCommand;

        public InstallWaitingViewModel(BootstrapperApplication bootstrapper)
        {
            _bootstrapper = bootstrapper ?? throw new ArgumentNullException(nameof(bootstrapper));

            AddWindowsFirewallException = true;
            RegisterFileProtocolHandlers = true;
            ShouldDownloadDependencies = true;

            Refresh();
        }

        public ICommand ChangeInstallLocationCommand
        {
            get
            {
                if (_changeInstallLocationCommand == null)
                {
                    _changeInstallLocationCommand = new RelayCommand(param => ChangeInstallLocation());
                }

                return _changeInstallLocationCommand;
            }
        }

        public string InstallLocation
        {
            get { return _installLocation; }
            set
            {
                _bootstrapper.Engine.StringVariables["InstallFolder"] = value;
                _installLocation = value;
                OnPropertyChanged(nameof(InstallLocation));
            }
        }

        public bool AddWindowsFirewallException
        {
            get { return _addWindowsFirewallException; }
            set
            {
                _bootstrapper.Engine.StringVariables["SKIP_FIREWALL_EXCEPTION"] = value ? "0" : "1";
                _addWindowsFirewallException = value;
                OnPropertyChanged(nameof(AddWindowsFirewallException));
            }
        }

        public bool RegisterFileProtocolHandlers
        {
            get { return _registerFileProtocolHandlers; }
            set
            {
                _bootstrapper.Engine.StringVariables["SKIP_ASSOCIATE_FILES"] = value ? "0" : "1";
                _registerFileProtocolHandlers = value;
                OnPropertyChanged(nameof(RegisterFileProtocolHandlers));
            }
        }

        public bool ShouldDownloadDependencies
        {
            get { return _downloadDependencies; }
            set
            {
                _bootstrapper.Engine.StringVariables["INSTALL_VCREDIST"] = value ? "1" : "0";
                _downloadDependencies = value;
                OnPropertyChanged(nameof(ShouldDownloadDependencies));
            }
        }

        public void Refresh()
        {
            if (_bootstrapper.Engine.StringVariables.Contains("InstallFolder"))
            {
                InstallLocation = _bootstrapper.Engine.FormatString(_bootstrapper.Engine.StringVariables["InstallFolder"]).TrimEnd('\\');
            }
        }

        private void ChangeInstallLocation()
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = InstallLocation
            };

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            InstallLocation = dlg.SelectedPath;
        }
    }
}
