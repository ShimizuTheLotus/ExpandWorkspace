using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;

namespace ExpandWorkspace
{
    public partial class MainWindow
    {
        public bool IsHideLoop = false;
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string? lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

            public int X
            {
                get { return Left; }
                set { Right -= (Left - value); Left = value; }
            }

            public int Y
            {
                get { return Top; }
                set { Bottom -= (Top - value); Top = value; }
            }

            public int Height
            {
                get { return Bottom - Top; }
                set { Bottom = value + Top; }
            }

            public int Width
            {
                get { return Right - Left; }
                set { Right = value + Left; }
            }

            public System.Drawing.Point Location
            {
                get { return new System.Drawing.Point(Left, Top); }
                set { X = value.X; Y = value.Y; }
            }

            public System.Drawing.Size Size
            {
                get { return new System.Drawing.Size(Width, Height); }
                set { Width = value.Width; Height = value.Height; }
            }

            public static implicit operator System.Drawing.Rectangle(RECT r)
            {
                return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator RECT(System.Drawing.Rectangle r)
            {
                return new RECT(r);
            }

            public static bool operator ==(RECT r1, RECT r2)
            {
                return r1.Equals(r2);
            }

            public static bool operator !=(RECT r1, RECT r2)
            {
                return !r1.Equals(r2);
            }

            public bool Equals(RECT r)
            {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object? obj)
            {
                if(obj == null) return false;
                if (obj is RECT)
                    return Equals((RECT)obj);
                else if (obj is System.Drawing.Rectangle)
                    return Equals(new RECT((System.Drawing.Rectangle)obj));
                return false;
            }

            public override int GetHashCode()
            {
                return ((System.Drawing.Rectangle)this).GetHashCode();
            }

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize; // initialize this field using: Marshal.SizeOf(typeof(APPBARDATA));
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public int lParam;
        }
        public enum AppBarStates
        {
            AlwaysOnTop = 0x00,
            AutoHide = 0x01
        }
        public enum AppBarMessages
        {
            New = 0x00,
            Remove = 0x01,
            QueryPos = 0x02,
            SetPos = 0x03,
            GetState = 0x04,
            GetTaskBarPos = 0x05,
            Activate = 0x06,
            GetAutoHideBar = 0x07,
            SetAutoHideBar = 0x08,
            WindowPosChanged = 0x09,
            SetState = 0x0a
        }
        [DllImport("shell32.dll")]
        public static extern UInt32 SHAppBarMessage(UInt32 dwMessage, ref APPBARDATA pData);
        public static void SetTaskbarState(AppBarStates option)
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);
            msgData.hWnd = FindWindow("System_TrayWnd", null);
            msgData.lParam = (int)option;
            SHAppBarMessage((UInt32)AppBarMessages.SetState, ref msgData);
        }
        // 定义常量
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        /// <summary>
        /// 隐藏任务栏
        /// </summary>
        public async void HideTaskbar()
        {
            var handle = FindWindow("Shell_TrayWnd", null);
            if (handle != IntPtr.Zero)
            {
                ShowWindow(handle, SW_HIDE); // 隐藏任务栏
                SetTaskbarState(AppBarStates.AutoHide);
                ShowWindow(handle, SW_HIDE); // 隐藏任务栏
                IsHideLoop = true;
                HideBarLoop();
            }
        }

        /// <summary>
        /// 显示任务栏
        /// </summary>
        public void ShowTaskbar()
        {
            var handle = FindWindow("Shell_TrayWnd", null);
            if (handle != IntPtr.Zero)
            {
                SetTaskbarState(AppBarStates.AlwaysOnTop);
                ShowWindow(handle, SW_SHOW); // 显示任务栏
                IsHideLoop = false;
            }
        }

        public async void HideBarLoop()
        {
            while (IsHideLoop)
            {
                var handle = FindWindow("Shell_TrayWnd", null);
                ShowWindow(handle, SW_HIDE); // 隐藏任务栏
                await Task.Delay(100);
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (BarToggle.IsChecked == true)
            {
                BarToggle.Content = "显示任务栏";
                HideTaskbar();
            }
            else
            {
                BarToggle.Content = "隐藏任务栏";
                ShowTaskbar();
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BarToggle.FontSize = 27;
        }
    }
}