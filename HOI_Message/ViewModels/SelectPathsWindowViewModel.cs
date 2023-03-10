using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HOI_Message.Logic;
using HOI_Message.Logic.Country;
using HOI_Message.Logic.CustomException;
using HOI_Message.Logic.Localisation;
using HOI_Message.Logic.State;
using HOI_Message.Logic.Util.CWTool;
using NLog;
using MessageBox = HandyControl.Controls.MessageBox;

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

        [ObservableProperty]
        private double parseProgressBarValue;

        private readonly Dictionary<DataPaths, string> _dataPathMap = new(8);

        private List<StateInfo>? _statesInfo;

        private readonly GameLocalisation _localisation = new();

        /// <summary>
        /// Key是国家Tag, Value是国家信息
        /// </summary>
        private readonly Dictionary<string, NationalInfo> _nationalInfoMap = new();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
                if (!File.Exists(descriptorPath))
                {
                    return;
                }

                try
                {
                    var descriptor = new Descriptor(descriptorPath);
                    SetModInfoOnView(descriptor);
                }
                catch (ParseException ex)
                {
                    _logger.Warn(ex);
                    MessageBox.Warning("MOD描述文件解析错误, 未解析出信息.");
                    return;
                }
            }
            else if (e.PropertyName == nameof(LocalisationPath))
            {
                _dataPathMap[DataPaths.Localisation] = LocalisationPath;
            }
        }

        private void SetModInfoOnView(Descriptor descriptor)
        {
            var picturePath = Path.Combine(GameRootPath, descriptor.PictureName);
            if (File.Exists(picturePath))
            {
                ImageSource = new BitmapImage(new Uri(picturePath, UriKind.Absolute));
            }
            ModName = descriptor.Name;
            ModVersion = descriptor.SupportedVersion;
            ModTags = string.Join(", ", descriptor.Tags);
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
            AddCountriesColor();

            // 配置全局资源
            GameModels._countries = new List<NationalInfo>(_nationalInfoMap.Values);
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
                try
                {
                    var state = new StateInfo(file.FullName);
                    _statesInfo.Add(state);
                }
                catch (ParseException ex)
                {
                    _logger.Warn(ex);
                }

                //刷新进度条                               
                double progressBarValue = (_statesInfo.Count / (double)files.Length) * 100;
                WeakReferenceMessenger.Default.Send(Tuple.Create(progressBarValue, file.FullName), EventId.UpdateParseProgressBar);
                ParseItemNumberLabel = $"{_statesInfo.Count} / {files.Length}";
            }
        }

        private void AddLocalisationData(string folderPath)
        {
            var files = new DirectoryInfo(folderPath).GetFiles(".", SearchOption.AllDirectories);
            uint count = 0;

            foreach (var file in files)
            {
                _localisation.AddForFilePath(file.FullName);
                ++count;

                //刷新进度条
                double progressBarValue = (count / (double)files.Length) * 100;
                WeakReferenceMessenger.Default.Send(Tuple.Create(progressBarValue, file.FullName), EventId.UpdateParseProgressBar);
                ParseItemNumberLabel = $"{count} / {files.Length}";
            }
            _localisation.TrimExcess();
        }

        private void AddCountriesInfo(string gameRootPath)
        {
            var countriesTagsMap = NationalInfo.GetAllCountriesTag(gameRootPath);
            var nationalStatesMap = NationalInfo.ClassifyStates(_statesInfo ?? throw new ArgumentNullException(nameof(_statesInfo)));
            uint count = 0;

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
                    _nationalInfoMap.Add(item.Key, NationalInfo.Empty);
                    continue;
                }

                NationalInfo nationalInfo;
                if (nationalStatesMap.TryGetValue(item.Key, out var countryOwnStates))
                {
                    nationalInfo = new NationalInfo(parser!, countryOwnStates, item.Key);
                }
                else
                {
                    nationalInfo = new NationalInfo(parser!, emptyStatesList, item.Key);
                }

                _nationalInfoMap.Add(item.Key, nationalInfo);
            }
        }

        private void AddOOB()
        {
            uint count = 0;
            foreach (var item in _nationalInfoMap.Values)
            {
                count++;
                var oobFilePath = Path.Combine(GameRootPath, "history", "units", $"{item.OOBName}.txt");

                if (!File.Exists(oobFilePath))
                {
                    _logger.Warn("文件 '{0}' 不存在", oobFilePath);
                    continue;
                }
                try
                {
                    item.SetUnitInfo(oobFilePath);
                }
                catch (ParseException ex)
                {
                    _logger.Warn(ex);
                }
                var value = ((double)count / _nationalInfoMap.Count) * 100;
                WeakReferenceMessenger.Default.Send(Tuple.Create(value, oobFilePath), EventId.UpdateParseProgressBar);
                ParseItemNumberLabel = $"{count} / {_nationalInfoMap.Count}";
            }
        }

        private void AddCountriesColor()
        {
            var map = NationalInfo.GetCountriesColorFilePath(GameRootPath);

            foreach (var item in map)
            {
                if (!File.Exists(item.Value))
                {
                    _logger.Warn($"已定义颜色但未找到对应文件, path: '{item.Value}'");
                    continue;
                }
                var parser = new CWToolsAdapter(item.Value);
                if (!parser.IsSuccess)
                {
                    _logger.Warn($"{item.Value} 解析失败");
                    continue;
                }

                var colorData = parser.Root.Child("color").Value.LeafValues.ToList();
                try
                {
                    byte red = byte.Parse(colorData[0].Value.ToRawString());
                    byte green = byte.Parse(colorData[1].Value.ToRawString());
                    byte blue = byte.Parse(colorData[2].Value.ToRawString());
                    if (_nationalInfoMap.TryGetValue(item.Key, out var country))
                    {
                        country.SetMapColor(red, green, blue);
                    }
                    else
                    {
                        _logger.Warn($"未找到 '{item.Key}', path:{item.Value}");
                    }
                }
                catch (FormatException)
                {
                    // 有可能是 hsv 格式而不是 rgb 格式, 现在先不管了
                }
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
