using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFGameLauncher;

namespace WpfCarouselDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private Settings SettingsRef;
        public Window1(ref Settings settingsRef)
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
        }
    }
}
