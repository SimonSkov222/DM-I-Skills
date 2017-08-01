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
            /// <summary>
            /// Vi søger i databasen efter alle skolerne 
            /// og laver det om til en list af strings
            /// </summary>
            public List<string> SchoolAutoComplete
            {
                get
                {
                    List<string> result = new List<string>();
                    if (Database.IsConnected)
                    {
                        var data = Database.GetRows<int>("Schools", new string[] { "Name" });
                        if (data != null)
                            foreach (var item in data)
                                result.Add((string)item[0]);
                    }
                    return result;
                }
            }

            private int _Round = 1;
            public int Round
            {
                get { return _Round; }
                set
                {
                    _Round = value;
                    NotifyPropertyChanged();
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
            

                Loaded += MainWindow_Loaded;
            }

            /// <summary>
            /// Når vinduet er loaded vil vi opdatere hold nummere 
            /// og skole autocomplete liste for bordene
            /// </summary>
            private void MainWindow_Loaded(object sender, RoutedEventArgs e)
            {
                UpdateTableTeam();
                UpdateSchoolList();
            }
        
            /// <summary>
            /// Giver alle bordene det rigtige hold nummer alt efter runde
            /// (Vi har lavet denne function fordi vores
            /// data binding ikke ville gøre det for os)
            /// </summary>
            private void UpdateTableTeam()
            {
                //cTable1.TeamID = (Round - 1) * 10 + 1;
                //cTable2.TeamID = (Round - 1) * 10 + 2;
                //cTable3.TeamID = (Round - 1) * 10 + 3;
                //cTable4.TeamID = (Round - 1) * 10 + 4;
                //cTable5.TeamID = (Round - 1) * 10 + 5;
            }

            /// <summary>
            /// Opdatere autocomplete listen med skolerne for bordene
            /// (Vi har lavet denne function fordi vores
            /// data binding ikke ville gøre det for os)
            /// </summary>
            private void UpdateSchoolList()
            {
                ////Nulstil bordene
                //cTable1.schoolList.Children.Clear();
                //cTable2.schoolList.Children.Clear();
                //cTable3.schoolList.Children.Clear();
                //cTable4.schoolList.Children.Clear();
                //cTable5.schoolList.Children.Clear();

                ////Tilføj skolerne
                //foreach (var item in SchoolAutoComplete)
                //{
                //    cTable1.schoolList.Children.Add(item);
                //    cTable2.schoolList.Children.Add(item);
                //    cTable3.schoolList.Children.Add(item);
                //    cTable4.schoolList.Children.Add(item);
                //    cTable5.schoolList.Children.Add(item);
                //}
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

                    //Gå til næste runde
                    Round++;
                    UpdateTableTeam();
                    UpdateSchoolList();
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
