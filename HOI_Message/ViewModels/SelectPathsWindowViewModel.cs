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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using HOI_Message.Logic.Unit;

namespace HOI_Message.ViewModels
{

    public partial class SelectPathsWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string gameRootPath = string.Empty;

        [ObservableProperty]
        private string localisationPath = string.Empty;

        [ObservableProperty]
        private string parseItemNumberLabel = string.Empty;

        [ObservableProperty]
        private ImageSource? imageSource;

        [ObservableProperty]
        private string modName = string.Empty;

        [ObservableProperty]
        private string modVersion = string.Empty;

        [ObservableProperty]
        private string modTags = string.Empty;

        private readonly Dictionary<DataPaths, string> _dataPathMap = new(8);

        private List<StateInfo>? _statesInfo = null;

        private GameLocalisation _localisation = GameLocalisation.Empty;

        private readonly List<NationalInfo> _nationalInfoList = new();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();


        //private string? _modDescriptorPath = null;

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
                    var picturePath = Path.Combine(GameRootPath, descriptor.PictureName);
                    if (File.Exists(picturePath))
                    {
                        ImageSource = new BitmapImage(new Uri(picturePath, UriKind.Absolute));
                    }
                    
                    ModName = descriptor.Name;
                    ModVersion = descriptor.Version;
                    ModTags = string.Join(", ", descriptor.Tags);
                }
            }
            if (e.PropertyName == nameof(LocalisationPath))
            {
                _dataPathMap[DataPaths.Localisation] = LocalisationPath;
            }
        }



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
            AddCountriesInfo(GameRootPath);
            AddOOB();

            // 配置全局资源
            GameModels._countries = _nationalInfoList!;
            GameModels.Localisation = _localisation;
            WeakReferenceMessenger.Default.Send(string.Empty, EventId.ParseDataSuccess);
        }

        private void AddStateData(string folderPath)
        {
            var dir = new DirectoryInfo(folderPath);
            var files = dir.GetFiles();
            _statesInfo = new List<StateInfo>(files.Length);

            foreach (var file in files)
            {
                _statesInfo.Add(new StateInfo(file.FullName));

                //刷新进度条
                double progressBarValue = (_statesInfo.Count / (double)files.Length) * 100;
                WeakReferenceMessenger.Default.Send(Tuple.Create(progressBarValue, file.FullName), EventId.UpdateParseProgressBar);
                ParseItemNumberLabel = $"{_statesInfo.Count} / {files.Length}";
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
                WeakReferenceMessenger.Default.Send(Tuple.Create(progressBarValue, file.FullName), EventId.UpdateParseProgressBar);
                ParseItemNumberLabel = $"{count} / {files.Length}";
            }
            _localisation = new GameLocalisation(map);
        }

        private void AddCountriesInfo(string gameRootPath)
        {
            var countriesTagsMap = NationalInfo.GetAllCountriesTag(gameRootPath);
            var nationalStatesMap = NationalInfo.ClassifyStates(_statesInfo ?? throw new Exception());
            uint count = 0;

            NationalInfo nationalInfo;
            var emptyStatesList = new List<StateInfo>();
            foreach (var item in countriesTagsMap)
            {
                ++count;
                double progress = ((double)count / countriesTagsMap.Count) * 100;
                WeakReferenceMessenger.Default.Send(Tuple.Create(progress, item.Value), EventId.UpdateParseProgressBar);
                ParseItemNumberLabel = $"{count} / {countriesTagsMap.Count}";

                if (!CountryFileParser.TryParseFile(item.Value, out var parser, out var errorMessage))
                {
                    _logger.Warn("{0} 文件出现问题, 错误信息: {1}", item.Value, errorMessage);
                    continue;
                }
                
                if (nationalStatesMap.TryGetValue(item.Key, out var countryOwnStates))
                {
                    nationalInfo = new NationalInfo(parser!, countryOwnStates, item.Key);
                }
                else
                {
                    nationalInfo = new NationalInfo(parser!, emptyStatesList, item.Key);
                }

                _nationalInfoList.Add(nationalInfo);
            }
        }

        private void AddOOB()
        {
            foreach (var item in _nationalInfoList)
            {
                var oobFilePath = Path.Combine(GameRootPath, "history", "units", item.OOBName);

                if (!File.Exists(oobFilePath))
                {
                    item.UnitInfo = UnitInfo.Empty;
                    continue;
                }
                item.UnitInfo = new UnitInfo(oobFilePath);
            }
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
