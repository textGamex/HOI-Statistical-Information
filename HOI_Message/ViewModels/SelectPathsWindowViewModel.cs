using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HOI_Message.Logic;
using HOI_Message.Logic.Country;
using HOI_Message.Logic.Localisation;
using HOI_Message.Logic.State;
using static HOI_Message.Logic.Localisation.LocalisationData;
using MessageBox = HandyControl.Controls.MessageBox;
using NLog;

namespace HOI_Message.ViewModels
{
    public partial class SelectPathsWindowViewModel : ObservableObject
    {
        private string? _modDescriptorPath = null;
        private const string DescriptorName = "descriptor.mod";

        public SelectPathsWindowViewModel()
        {
            PropertyChanged += SelectPathsWindowViewModel_PropertyChanged;
#if DEBUG
            GameRootPath = @"D:\STEAM\steamapps\common\Hearts of Iron IV";
            LocalisationPath = @"D:\STEAM\steamapps\workshop\content\394360\698748356\localisation\replace";
#endif            
        }

        private void SelectPathsWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GameRootPath))
            {
                _dataPathMap[DataPaths.States] = Path.Combine(GameRootPath, "history", "states");       
                LocalisationPath = Path.Combine(GameRootPath, "localisation");

                // 尝试查找mod配置文件
                var descriptorPath = Path.Combine(GameRootPath, DescriptorName);
                if (File.Exists(descriptorPath))
                {
                    var descriptor = new Descriptor(descriptorPath);

                }
            }
            if (e.PropertyName == nameof(LocalisationPath))
            {
                _dataPathMap[DataPaths.Localisation] = LocalisationPath;
            }
        }

        [ObservableProperty]
        public string gameRootPath = string.Empty;

        [ObservableProperty]
        public string localisationPath = string.Empty;

        [ObservableProperty]
        public string parseItemNumberLabel = string.Empty;

        private readonly Dictionary<DataPaths, string> _dataPathMap = new(8);

        private List<StateInfo>? _stateMessages = null;

        private GameLocalisation _localisation = GameLocalisation.Empty;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [RelayCommand]
        void StartParseButtonClick()
        {
            if (GameRootPath == string.Empty)
            {
                MessageBox.Warning("请选择游戏文件根目录");
                return;
            }

            if (LocalisationPath == string.Empty &&
                MessageBox.Show("您还没有选择本地化文件, 这样显示出来的文本将会是游戏原始名称, 您确定吗?",
                "提示",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            WeakReferenceMessenger.Default.Send(string.Empty, EventId.StartParseData);

            AddStateData(_dataPathMap[DataPaths.States]);
            AddLocalisationData(_dataPathMap[DataPaths.Localisation]);

            GameModels._countries = NationalInfo.ByStates(_stateMessages!);
            GameModels.Localisation = _localisation;
            WeakReferenceMessenger.Default.Send(string.Empty, EventId.ParseDataSuccess);
        }

        private void AddStateData(string folderPath)
        {
            var dir = new DirectoryInfo(folderPath);
            var files = dir.GetFiles();
            _stateMessages = new List<StateInfo>(files.Length);

            foreach (var file in files)
            {
                _stateMessages.Add(new StateInfo(file.FullName));

                //刷新进度条
                double progressBarValue = (_stateMessages.Count / (double)files.Length) * 100;
                WeakReferenceMessenger.Default.Send(Tuple.Create(progressBarValue, file), EventId.UpdateParseProgressBar);
                ParseItemNumberLabel = $"{_stateMessages.Count} / {files.Length}";
            }
        }

        private void AddLocalisationData(string folderPath)
        {
            var files = new DirectoryInfo(folderPath).GetFiles(".", SearchOption.AllDirectories);
            var map = new Dictionary<string, Data>();
            uint count = 0;

            foreach (var file in files)
            {
                GameLocalisation.AddDataToMap(map, file.FullName);
                ++count;

                //刷新进度条
                double progressBarValue = (count / (double)files.Length) * 100;
                WeakReferenceMessenger.Default.Send(Tuple.Create(progressBarValue, file), EventId.UpdateParseProgressBar);
                ParseItemNumberLabel = $"{count} / {files.Length}";
            }
            _localisation = new GameLocalisation(map);
        }


        [RelayCommand]
        private void SelectGameRootFolderButtonClick()
        {
            FolderBrowserDialog dialog = new()
            {
                Description = "选择游戏文件夹"
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            GameRootPath = dialog.SelectedPath;
        }

        [RelayCommand]
        private void SelectLocalisationButtonClick()
        {
            FolderBrowserDialog dialog = new()
            {
                Description = "选择本地化文件文件夹"
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            LocalisationPath = dialog.SelectedPath;
        }

        private enum DataPaths : byte
        {
            States,
            Localisation
        }
    }
}
