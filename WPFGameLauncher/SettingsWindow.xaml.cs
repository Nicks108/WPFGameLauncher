using System.Windows;
using WPFGameLauncher;

namespace WpfCarouselDemo
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Settings SettingsRef;
        public SettingsWindow(ref Settings settingsRef)
        {
            InitializeComponent();
            SettingsRef = settingsRef;
        }

        private void BTNOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.None;
            System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();

            while (result != System.Windows.Forms.DialogResult.OK)
            {
                result = openFileDlg.ShowDialog();
            }

            SettingsRef.GameLibRootDir = openFileDlg.SelectedPath;
            txtFolderLocation.Text = openFileDlg.SelectedPath;
        }
    }
}
