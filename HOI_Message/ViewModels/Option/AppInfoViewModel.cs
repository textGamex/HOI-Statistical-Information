using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HOI_Message.ViewModels.Option;

[ObservableObject]
internal partial class AppInfoViewModel
{
    [ObservableProperty]
    private string lastCompileTime = string.Empty;

    [ObservableProperty]
    private string frameworkDescription = string.Empty;

    [ObservableProperty]
    private string hoiParseLibrary = string.Empty;

    public AppInfoViewModel()
    {
        LastCompileTime = File.GetLastWriteTime(this.GetType().Assembly.Location).ToString("yyyy-M-dd HH:mm:ss");
        FrameworkDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        HoiParseLibrary = "CWTools v0.3.0";
    }
}