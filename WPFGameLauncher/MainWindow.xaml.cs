﻿using System.Runtime.InteropServices;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Remoting.Channels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using WpfCarouselDemo;
using WPFGameLauncher.Model;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Media;
using System.Windows.Controls;


namespace WPFGameLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public class SettingsManager
    {
        readonly string CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        const string SettingsFile = "Settings.json";

        public Settings settings = new Settings();

        string SettingsFileAndLocation
        {
            get { return CurrentDirectory + "//" + SettingsFile; }
        }
        public SettingsManager()
        {
            
            //check for existing settings file
            if (!File.Exists(SettingsFileAndLocation)) // if no settings
            {
                //show first time setup
                //setting
                using (StreamWriter sw = File.CreateText(SettingsFileAndLocation))
                {
                    //get initial settigns
                    SettingsWindow SettingsWindow = new SettingsWindow(ref settings);
                    SettingsWindow.ShowDialog();

                    //save settings
                    string settingToJson = System.Text.Json.JsonSerializer.Serialize(settings);
                    sw.Write(settingToJson);
                }

            }
            else
            {
                //if settings exists, read settings from file.
                using (StreamReader SR = File.OpenText(SettingsFileAndLocation))
                {
                    settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(SR.ReadToEnd());
                }

            }
        }

    }

    public class Settings
    {
        public string GameLibRootDir { get; set; }
    }


    public partial class MainWindow : Window
    {
        SettingsManager settingsManager = new SettingsManager();
        
        public MainWindow()
        {

            InitializeComponent();


            //make new settigns manager


            string[] GameDirs = GetAllSubDirsFromPath(settingsManager.settings.GameLibRootDir);
            System.Collections.ObjectModel.ObservableCollection<Model.GameInfo> gameInfoCollection = new ObservableCollection<GameInfo>();
            foreach (string gameDir in GameDirs)
            {
                try
                {
                    using (FileStream FS = new FileStream(gameDir +"\\"+ "GameInfo.json", FileMode.Open))
                    {
                        GameInfo GI = System.Text.Json.JsonSerializer.Deserialize<GameInfo>(FS);
                        GI.GameFolderLocation = gameDir;
                        gameInfoCollection.Add(GI);
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }



                
            }


            DataContext = new ViewModel.MainViewModel(gameInfoCollection);
            ViewModel.MainViewModel viewModel = DataContext as ViewModel.MainViewModel;
            



            _carouselDABRadioStations.SelectionChanged += _carouselDABRadioStations_SelectionChanged;

            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;

            this.PreviewKeyDown += new KeyEventHandler(OnButtonKeyDown);



            //var videoPath = Directory.GetCurrentDirectory();
            //mediaElement.Source = new Uri(videoPath + @"\Resources\Background_Vid.wmv", UriKind.Relative);
            //mediaElement.Play();
        }


        private Process GameProcess = null;

        //https://www.ultimarc.com/control-interfaces/i-pacs/i-pac4-board/  (maybe not)

        //p1 = 1 
        //p2 = 2

        //player 1:

        //up = up arrow
        //down = down arrow
        //left = left arrow
        //right = right arrow

        //top row
        //  q
        //  w
        //  e

        //bottom row
        //  a
        //  s
        //  d

        //player 2:

        //  up = t
        //  down = g
        //  left = f
        //  right = h

        //top row:
        //  u
        //  i
        //  o

        //bottom row
        //  j
        //  k
        //  l
        private void OnButtonKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (GameProcess == null)
            {
                switch (e.Key)
                {
                    case Key.Q:
                    case Key.Space:
                        //MessageBox.Show(e.Key.ToString());
                        
                        var viewModel = DataContext as ViewModel.MainViewModel;
                        string ExeLoc = viewModel.SelectedGameInfo.GameFolderLocation + "/" +
                                        viewModel.SelectedGameInfo.RelativeEXELocation;
                        GameProcess = Process.Start(ExeLoc);
                        LoadingIndicator.SetCurrentValue(LoadingIndicators.WPF.LoadingIndicator.IsActiveProperty, true);
                        //Process.Start(@"C:\Windows\notepad.exe");
                        break;
                    case Key.Left:
                        //_carouselDABRadioStations.RotateLeft();
                        _buttonLeftArrow.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        break;
                    case Key.Right:
                        //_carouselDABRadioStations.RotateRight();
                        _buttonRightArrow.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        break;
                }
            }
        }

        string[] GetAllSubDirsFromPath(string Path)
        {
            return Directory.GetDirectories(Path, "*", SearchOption.TopDirectoryOnly);
        }

        private void _carouselDABRadioStations_SelectionChanged(FrameworkElement selectedElement)
        {
            var viewModel = DataContext as ViewModel.MainViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.SelectedGameInfo = selectedElement.DataContext as Model.GameInfo;
        }

        private void _buttonLeftArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselDABRadioStations.RotateRight();
        }

        private void _buttonRightArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselDABRadioStations.RotateLeft();
        }

        private void _checkBoxVerticalCarousel_Click(object sender, RoutedEventArgs e)
        {
            //_carouselDABRadioStations.VerticalOrientation = _checkBoxVerticalCarousel.IsChecked.HasValue ? _checkBoxVerticalCarousel.IsChecked.Value : false;
        }


        //private void _buttonDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    var viewModel = DataContext as ViewModel.MainViewModel;
        //    if (viewModel == null)
        //    {
        //        return;
        //    }

        //    viewModel.Delete();
        //}




        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;
        private const int HOTKEY_ID_X = 9001;

        //Modifiers:
        //private const uint MOD_NONE = 0x0000; //(none)
        //private const uint MOD_ALT = 0x0001; //ALT
        //private const uint MOD_CONTROL = 0x0002; //CTRL
        private const uint MOD_SHIFT = 0x0004; //SHIFT
        //private const uint MOD_WIN = 0x0008; //WINDOWS
        //private const uint VK_CAPITAL = 0x14; //CAPS LOCK:
        private const uint VK_ESCAPE = 0x1B; //ESC key
        private const uint VK_x = 0x58; //x key
        private IntPtr _windowHandle;
        private HwndSource _source;

        //set up global hotkey 
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, HOTKEY_ID_X, 0, VK_x); //SHIFT + ESC
            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_SHIFT, VK_ESCAPE); //SHIFT + ESC
        }

        //check to see which msg was passed and which key was pressed ( if key was pressed)
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID_X:
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_ESCAPE || vkey == VK_x)

                {
                                if (GameProcess != null)
                                {
                                    //MessageBox.Show(" esc/x was pressed" + Environment.NewLine);
                                    GameProcess.Kill();
                                    GameProcess = null;
                                    //LoadingIndicator.IsActive = false;
                                    LoadingIndicator.SetCurrentValue(LoadingIndicators.WPF.LoadingIndicator.IsActiveProperty, false);
                                }
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        //clean up
        protected override void OnClosed(EventArgs e)
        {
         
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            base.OnClosed(e);
        }


    }
}
