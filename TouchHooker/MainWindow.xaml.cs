using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace TouchHooker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private readonly HookProc _mouseHook;
        //private IntPtr _hMouseHook;

        public MainWindow()
        {
            InitializeComponent();

            hook = new DisableTouchConversionToMouse();
        }

        private DisableTouchConversionToMouse hook;
}
