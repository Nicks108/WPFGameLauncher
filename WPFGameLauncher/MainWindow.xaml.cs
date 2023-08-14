using System.Runtime.InteropServices;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WpfCarouselDemo;
using WPFGameLauncher.Model;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

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
                        gameInfoCollection.Add(System.Text.Json.JsonSerializer.Deserialize<GameInfo>(FS));
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

        }

        private void OnButtonKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                        MessageBox.Show(e.Key.ToString());
                        var viewModel = DataContext as ViewModel.MainViewModel;
                        Process.Start(settingsManager.settings.GameLibRootDir + "\\" + viewModel.SelectedGameInfo.RelativeEXELocation);
                        //Process.Start(@"C:\Windows\notepad.exe");
                    break;
                case Key.Left:
                    _carouselDABRadioStations.RotateLeft();
                    break;
                case Key.Right:
                    _carouselDABRadioStations.RotateRight();
                    break;
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

        private void _buttonLeftManyArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselDABRadioStations.RotateIncrement(-5);
        }

        private void _buttonRightManyArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselDABRadioStations.RotateIncrement(5);
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

        //Modifiers:
        //private const uint MOD_NONE = 0x0000; //(none)
        //private const uint MOD_ALT = 0x0001; //ALT
        //private const uint MOD_CONTROL = 0x0002; //CTRL
        private const uint MOD_SHIFT = 0x0004; //SHIFT
        //private const uint MOD_WIN = 0x0008; //WINDOWS
        //private const uint VK_CAPITAL = 0x14; //CAPS LOCK:
        private const uint VK_ESCAPE = 0x1B; //ESC key
        private IntPtr _windowHandle;
        private HwndSource _source;

        //set up global hotkey 
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

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
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_ESCAPE)
                            {
                                MessageBox.Show("shift esc was pressed" + Environment.NewLine);
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
