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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SQLite_DB_LIB;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DM_Skills.Views
{
    /// <summary>
    /// Interaction logic for Forside.xaml
    /// </summary>
    public partial class Forside : UserControl, INotifyPropertyChanged
    {
        public int NumbOfTables
        {
            get { return (int)GetValue(NumbOfTablesProperty); }
            set { SetValue(NumbOfTablesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumbOfTables.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NumbOfTablesProperty =
            DependencyProperty.Register(
                "NumbOfTables", 
                typeof(int), 
                typeof(Forside), 
                new PropertyMetadata(
                    3, 
                    new PropertyChangedCallback(CallBackProperty)
                    )
                );

        private Models.SettingsModel Settings;

        public static void CallBackProperty(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Forside element = sender as Forside;

            if (e.Property == NumbOfTablesProperty)
            {
                element.UpdateTableLayout((int)e.NewValue);
            }
        }

        private void UpdateTableLayout(int numb) {
            var visibleCnt = listOfTables.Children.Cast<UIElement>().Count(o => o.IsVisible);

            //Fjern sidste bord

            for (int i = visibleCnt; i > numb; i--)
            {
                listOfTables.Children[i].Visibility = Visibility.Collapsed;
            }
            //while (visibleCnt > numb)
            //{
            //    listOfTables.Children.RemoveAt(listOfTables.Children.Count - 1);
            //}

            //Tilføj nye bordre
            while (listOfTables.Children.Count < numb)
            {
                Controls.TablesControl table = new Controls.TablesControl();
                table.Title = "Bord " + (listOfTables.Children.Count +1);
                table.Margin = new Thickness(0, 0, 0, 3);

                table.SetBinding(Controls.TablesControl.ShowDropLocationProperty, new Binding() {
                    Path = new PropertyPath("IsDraggingItem"),
                    Source = LapList
                });
                table.SetBinding(Controls.TablesControl.SchoolsProperty, new Binding() {
                    Source = Settings,
                    Path = new PropertyPath("AllSchools")
                });

                listOfTables.Children.Add(table);
            }

            visibleCnt = listOfTables.Children.Cast<UIElement>().Count(o => o.IsVisible);
            for (int i = visibleCnt; i < numb; i++)
            {
                listOfTables.Children[i].Visibility = Visibility.Visible;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Fortæller at en property har ændret sig
            /// </summary>
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                //Tjekker om PropertyChanged er sat og kalder den hvis den er
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        

        /// <summary>
        /// Her connecter vi til databasen og opretter den
        /// vi tilføjet også en load event
        /// </summary>
        public Forside()
        {
            InitializeComponent();
            Settings = (Models.SettingsModel)FindResource("Settings");
            Loaded += (o,e) => UpdateTableLayout(NumbOfTables);
        }
        
       

            /// <summary>
            /// StopUr omgang klik.
            /// Tilføj omgang tiden til vores drag and drop list 
            /// </summary>
            private void TimerControl_OnLap(TimeSpan obj)
            {
                LapList.Add(obj);
            }

            /// <summary>
            /// StopUr Nulstil klik.
            /// Gør at vores drag and drop list også bliver resat
            /// </summary>
            private void TimerControl_OnReset()
            {
                LapList.Reset();
            }

            /// <summary>
            /// Indsend klik
            /// Her tjekker vi om bordene er udfyldt rigtigt
            /// og uploader dem hvis de er
            /// 
            /// Brugeren får også besked om man kunne
            /// uploade eller ej
            /// </summary>
            private void Button_Upload_Click(object sender, RoutedEventArgs e)
            {

            foreach (var item in listOfTables.Children)
            {
                ((Controls.TablesControl)item).Model.FailedUpload = true;
            }
            return;

                //Gør vi kan loop igennem bordene med et loop
                UserControl[] values =
                {
                //cTable1,
                //cTable2,
                //cTable3,
                //cTable4,
                //cTable5
            };

                //Fortæller om vi kan oploade eller ej
                bool canUpload = true;

                foreach (var table in values)
                {
                    //Hent model og tjek om vi overholder 
                    //reglerne for at kunne uploade
                    var model = table.DataContext as Models.TableModel;
                    if (model.HasData() && !model.CanUpload())
                    {
                        //Highlight det bord der er en fejl på
                        table.BorderThickness = new Thickness(2);
                        table.BorderBrush = Brushes.Red;
                        canUpload = false;
                    }
                    else
                    {
                        //Fjern highlight
                        table.BorderThickness = new Thickness(0);
                    }
                }


                //Tjek om må uploade
                if (canUpload)
                {
                    //Start upload for de bordere der har data
                    foreach (var table in values)
                    {
                        var model = table.DataContext as Models.TableModel;
                        if (model.HasData())
                            model.Uplaod();
                    }


                    MessageBox.Show("Er nu uploadet til databasen.");

                    
                    Button_Reset_Click(null, null);
                }
                else
                {
                    MessageBox.Show("Kan ikke uploade. Ret fejlne.");
                }
            }


            /// <summary>
            /// Nulstiling af runde klik.
            /// </summary>
            private void Button_Reset_Click(object sender, RoutedEventArgs e)
            {
                //cTable1.Reset();
                //cTable2.Reset();
                //cTable3.Reset();
                //cTable4.Reset();
                //cTable5.Reset();

                stopwatch.Reset();
                LapList.Reset();
            }

        private void TablesControl_Drop(object sender, DragEventArgs e)
        {
            Console.WriteLine("Drop");
        }

        private void TablesControl_DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine("drag enter");

        }
    }



}
