using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;            //INotifyPropertyChanged
using System.Runtime.CompilerServices;  //CallerMemberName for PropertyChanged
using System.Windows.Documents;
using System.Windows.Data;

namespace DM_Skills
{
    /// <summary>
    /// Klassen arver fra Window og bruger interface`et INotifyPropertyChanged
    /// Klassen styre det der skal ske i tidtagning fanen
    /// </summary>
    public partial class MainWindow : Window
    {

        public Views.Projektor projek;
        private Models.SettingsModel Settings;




        /// <summary>
        /// Her connecter vi til databasen og opretter den
        /// vi tilføjet også en load event
        /// </summary>
        /// 
        public MainWindow()
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(DateTime.Now.ToShortDateString(), @"\d{2}-\d{2}-\d{4}"))
            {
                MessageBox.Show("Din kalender format på computeren skal være sat til dansk!\n\nProgrammet lukkes", "Forkert Format", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }
            

            //Scripts.Database.CreateDatabase();
            Scripts.Database.CreateLocalDatabase();
            InitializeComponent();

            Settings = (Models.SettingsModel)FindResource("Settings");

            while (string.IsNullOrEmpty(Settings.Index))
            {
                Settings.Index = IPrompt.IInputBox.Show(
                    "Indtast et unik ID for dette program",
                    "Indtast ID", MessageBoxImage.None, "");
            }

            Settings.OnConnection += Settings_OnConnection;
            Settings.OnDisconnection += Settings_OnDisconnection;
            Loaded += (o, e) => { Menu_Indstillinger.IsChecked = false; Menu_Indstillinger.IsChecked = true; };

        }

        private void Settings_OnDisconnection(bool disconnectByUser)
        {
            //Menu_Indstillinger.IsChecked = true;
        }

        private void Settings_OnConnection()
        {
            Settings.NotifyPropertyChanged(nameof(Settings.AllLocations));
            Settings.InvokeSchoolsChanged();
            if (Settings.IsClient)
            {
                if (true)
                {
                    Settings.Client.Send((int)Scripts.JsonCommandIDs.GetLocation);
                }

                if (true)
                {
                    Settings.Client.Send((int)Scripts.JsonCommandIDs.GetTime);
                }
            }
        }
        

        private void Menu_Projektor_Checked(object sender, RoutedEventArgs e)
        {
            if (Menu_Projektor.IsChecked ?? false)
            {
                projek = new Views.Projektor(view_forside.stopwatch, this);
                
                projek.Show();
            }
            else
            {
                projek.Close();
            }
        }

        
        

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            var margin = warningDB.Margin;
            margin.Top = ((ScrollViewer)sender).VerticalOffset;
            warningDB.Margin = margin;
        }

        private void Button_KlikHer_Click(object sender, RoutedEventArgs e)
        {
            Menu_Indstillinger.IsChecked = true;
        }

        private void WarningDB_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            BindingOperations.GetBindingExpressionBase(gridForside, Grid.HeightProperty).UpdateTarget();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var newID = IPrompt.IInputBox.Show(
                    "Indtast et unik ID for dette program",
                    "Indtast ID", MessageBoxImage.None, Settings.Index);
            if (!string.IsNullOrEmpty(newID))
            {
                Settings.Index = newID;
            }
        }
    }
}
