using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DM_Skills.Views
{
    /// <summary>
    /// Interaction logic for Connection.xaml
    /// </summary>
    public partial class Connection : UserControl
    {
        public Connection()
        {
            InitializeComponent();
            txtIP.Text = ServerIP;
            var p = new PasswordBox();
            Console.WriteLine(p.Password.GetType());

            var r = new RichTextBox();
            
        }
        public string ServerIP { get { return Scripts.Helper.GetLocalIPv4(); } }

        private string clientIP = "";
        private string clientPort = "";
        private string serverPort = "";

        private void rClient_Checked(object sender, RoutedEventArgs e)
        {

            if (!this.IsLoaded)
            {
                return;
            }
            if (((RadioButton)sender).IsChecked ?? false)
            {
                serverPort = txtPort.Text;
                txtPort.Text = clientPort;
                txtIP.Text = clientIP;
            }
            else
            {
                clientPort = txtPort.Text;
                txtPort.Text = serverPort;
                
                clientIP = txtIP.Text;
                txtIP.Text = ServerIP;

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int id = Grid.GetColumn(sender as Button);

            var dlg = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            dlg.ShowDialog();

            OpenFileDialog dlgOpen = new OpenFileDialog();
            SaveFileDialog dlgSave = new SaveFileDialog();

            dlgOpen.DefaultExt = ".db";
            dlgOpen.Filter = "Text documents (.db)|*.db";

            bool? openResult = dlgOpen.ShowDialog();
            bool? saveResult = dlgSave.ShowDialog();

            string filename = "";

            if (openResult == true)
            {
                txtDB.Text = dlgOpen.FileName;
                filename = dlgOpen.FileName;
            }
            if (saveResult == true)
            {
                File.Create(filename);
            }





        }
    }
}
