using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
    /// Interaction logic for sog_pa_tider.xaml
    /// </summary>
    public partial class sog_pa_tider : UserControl, INotifyPropertyChanged
    {

        Models.SettingsModel Settings;

        private int? searchID = null;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public Models.TableModelN debug_item
        {
            get { return (Models.TableModelN)GetValue(debug_itemProperty); }
            set { SetValue(debug_itemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for debug_item.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty debug_itemProperty =
            DependencyProperty.Register("debug_item", typeof(Models.TableModelN), typeof(sog_pa_tider), new PropertyMetadata(null));

        public Scripts.Order Order
        {
            get
            {
                return _Order;
            }
            set
            {
                _Order = value;
                OnPropertyChanged();
            }
        }
        private Scripts.Order _Order = Scripts.Order.NyesteTider;


        public ObservableCollection<Models.TableModelN> ItemSourceSearch
        {
            get { return (ObservableCollection<Models.TableModelN>)GetValue(ItemSourceSearchProperty); }
            set { SetValue(ItemSourceSearchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSourceSearch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSourceSearchProperty =
            DependencyProperty.Register(
                "ItemSourceSearch",
                typeof(ObservableCollection<Models.TableModelN>),
                typeof(sog_pa_tider), new PropertyMetadata(null));




        private int Print_ = 0;


        public sog_pa_tider()
        {
            InitializeComponent();
            Order = Scripts.Order.NyesteTider;
            ItemSourceSearch = new ObservableCollection<Models.TableModelN>();
            for (int i = 0; i < 100; i++)
            {
                ItemSourceSearch.Add(new Models.TableModelN()
                {
                    School = new Models.SchoolModel() { Name = "Skole " + (i + 1) },
                    Location = new Models.LocationModel() { Name = "Frederiksberg" },
                    Persons = new ObservableCollection<Models.PersonModel>() {
                        new Models.PersonModel() { Name = "Person 1 "},
                        new Models.PersonModel() { Name = "Person 2 "},
                        new Models.PersonModel() { Name = "Person 2 "},
                        new Models.PersonModel() { Name = "Person 2 "}
                    },
                    Team = new Models.TeamModel()
                    {
                        Date = "01-08-2017 ",
                        Class = "7A ",
                        Time = "14:00:12 ",
                        ID = 1
                    }

                });
                Settings = (Models.SettingsModel)FindResource("Settings");
            }

            //debug_grid.DataContext = debug_item;
            Loaded += Sog_pa_tider_Loaded;
        }

        private void Sog_pa_tider_Loaded(object sender, RoutedEventArgs e)
        {
            //BindingOperations.GetBindingExpressionBase(results, ListView.ItemsSourceProperty);
        }

        private void results_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        private void SearchList_CheckAll(object sender, RoutedEventArgs e)
        {
            if (Print_ == 1) return;

            if (((CheckBox)sender).IsChecked ?? false)
                searchList.SelectAll();
            else
                searchList.SelectedIndex = -1;
        }


        private void SearchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Print_ = 1;
            printCheckAll.IsChecked = searchList.SelectedItems.Count == searchList.Items.Count;
            Print_ = 0;
        }


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var scrollviewer = (ScrollViewer)Scripts.Helper.FindAncestor(this, typeof(ScrollViewer));

            scrollviewer.ScrollChanged += (o, ee) =>
            {
                var margin = ((Grid)sender).Margin;
                margin.Top = ((ScrollViewer)o).VerticalOffset;
                ((Grid)sender).Margin = margin;
            };
        }

        private void Button_Print_Click(object sender, RoutedEventArgs e)
        {

            if (searchList.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Du har ikke valgt nogen skoler som skal udskrives", "Ingen valgte", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            var print = new Scripts.Print();

            dlg.Filter = "(.pdf)|*.pdf";
            if (dlg.ShowDialog() == true)
            {
                var models = new ObservableCollection<Models.TableModelN>();
                foreach (var i in searchList.SelectedItems)
                {
                    models.Add(i as Models.TableModelN);
                }
                print.CreatePDF(dlg.FileName, models);
            }



        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            

            Console.WriteLine(Settings.IsServer + "  " + Settings.IsClient);

            if (!Settings.IsServer && !Settings.IsClient)
            {
                MessageBox.Show("Du er ikke tilsluttet til en database", "Ingen database", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            var school = txtSchoolName.Text;
            var person = txtDeltager.Text;
            var from = dtFrom.SelectedDate == null ? "" : dtFrom.SelectedDate.Value.ToShortDateString();
            var to = dtTo.SelectedDate == null ? "" : dtTo.SelectedDate.Value.ToShortDateString();
            var location = cbLocation.SelectedIndex == 0 ? null : cbLocation.SelectedItem as Models.LocationModel;
            Console.WriteLine(searchList.Items.Count + " 2");


            if (searchID.HasValue)
            {
                return;
            }

            searchID = Models.TableModelN.GetTables(
                Order,
                school,
                person,
                location,
                from, to,
                (o) =>
                {
                    Application.Current.Dispatcher.Invoke(delegate ()
                    {
                        searchList.Items.Clear();
                    });
                    foreach (var item in o)
                    {
                        Application.Current.Dispatcher.Invoke(delegate ()
                        {
                            searchList.Visibility = Visibility.Visible;
                            Print.Visibility = Visibility.Visible;
                            header.Visibility = Visibility.Visible;
                            ReplacedHeaderTxt.Visibility = Visibility.Collapsed;
                            searchList.Items.Add(item);
                            
                        });
                        Thread.Sleep(30);
                    }


                    Application.Current.Dispatcher.Invoke(delegate ()
                    {
                        btnA.Visibility = Visibility.Collapsed;
                        btnS.Visibility = Visibility.Visible;
                        if (searchList.Items.Count <= 0)
                        {
                            searchList.Visibility = Visibility.Collapsed;
                            Print.Visibility = Visibility.Collapsed;
                            header.Visibility = Visibility.Collapsed;
                            ReplacedHeaderTxt.Visibility = Visibility.Visible;
                        }
                    });

                    searchID = null;

                });
            btnS.Visibility = Visibility.Collapsed;
            btnA.Visibility = Visibility.Visible;

            


            //searchList.ItemsSource = items;
        }

        private void ComboBox_Location_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if ((sender as ComboBox).Items.Count > 0)
            {
                (sender as ComboBox).SelectedIndex = 0;
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (searchID.HasValue)
            {
                Models.TableModelN.CancelThread(searchID);
                searchID = null;
            }
            btnA.Visibility = Visibility.Collapsed;
            btnS.Visibility = Visibility.Visible;


        }
    }
}
