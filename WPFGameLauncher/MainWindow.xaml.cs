using System.Runtime.InteropServices;
using System;
using System.Windows;
using System.Windows.Interop;

namespace WPFGameLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ViewModel.MainViewModel();

            _carouselDABRadioStations.SelectionChanged += _carouselDABRadioStations_SelectionChanged;



        }

        private void _carouselDABRadioStations_SelectionChanged(FrameworkElement selectedElement)
        {
            var viewModel = DataContext as ViewModel.MainViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.SelectedRadioStationDAB = selectedElement.DataContext as Model.RadioStation;
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
            _carouselDABRadioStations.VerticalOrientation = _checkBoxVerticalCarousel.IsChecked.HasValue ? _checkBoxVerticalCarousel.IsChecked.Value : false;
        }

        private void _buttonLeftManyArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselDABRadioStations.RotateIncrement(-5);
        }

        private void _buttonRightManyArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselDABRadioStations.RotateIncrement(5);
        }

        private void _buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ViewModel.MainViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.Delete();
        }




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

            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_SHIFT, VK_ESCAPE); //CTRL + CAPS_LOCK
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
