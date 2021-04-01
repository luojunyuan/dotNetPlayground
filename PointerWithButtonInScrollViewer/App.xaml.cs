using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PointerWithButtonInScrollViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            //AppContext.SetSwitch("Switch.System.Windows.Input.Stylus.DisableStylusAndTouchSupport", true);
            AppContext.SetSwitch("Switch.System.Windows.Input.Stylus.EnablePointerSupport", true);

            var a = new AdjustScreenByWmi();
            Trace.WriteLine(a.IsSupported);
        }
    }
}
