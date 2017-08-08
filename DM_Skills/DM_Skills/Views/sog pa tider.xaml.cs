using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DM_Skills.Views
{
    /// <summary>
    /// Interaction logic for sog_pa_tider.xaml
    /// </summary>
    public partial class sog_pa_tider : UserControl
    {


        public Models.TableModelN debug_item
        {
            get { return (Models.TableModelN)GetValue(debug_itemProperty); }
            set { SetValue(debug_itemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for debug_item.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty debug_itemProperty =
            DependencyProperty.Register("debug_item", typeof(Models.TableModelN), typeof(sog_pa_tider), new PropertyMetadata(null));





        public List<Models.TableModelN> Itemsss
        {
            get { return (List<Models.TableModelN>)GetValue(ItemsssProperty); }
            set { SetValue(ItemsssProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Itemsss.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsssProperty =
            DependencyProperty.Register("Itemsss", typeof(List<Models.TableModelN>), typeof(sog_pa_tider), new PropertyMetadata(null));




        private int Print_ = 0;

        public List<Models.TableModelN> Itemss { get; set; }
        //public Models.TableModelN debug_item { get; set; }

        public sog_pa_tider()
        {
            InitializeComponent();

            debug_item = new Models.TableModelN()
            {
                School = new Models.SchoolModel() { Name = "Skole" },
                Location = new Models.LocationModel() { Name = "Frederiksberg" },
                Persons = new ObservableCollection<Models.PersonModel>() {
                    new Models.PersonModel() { Name = "Person 1"},
                    new Models.PersonModel() { Name = "Person 2"}
                },
                Team = new Models.TeamModel()
                {
                    Date = "01-08-2017",
                    Class = "7A",
                    Time = "14:00:12",
                    ID = 1
                }

            };


            Itemsss = new List<Models.TableModelN>();

            Itemsss.Add(new Models.TableModelN()
            {
                School = new Models.SchoolModel() { Name = "Skole " },
                Location = new Models.LocationModel() { Name = "Frederiksberg" },
                Persons = new ObservableCollection<Models.PersonModel>() {
                    new Models.PersonModel() { Name = "Person 1 "},
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

            Itemsss.Add(new Models.TableModelN()
            {
                School = new Models.SchoolModel() { Name = "Skole 2 " },
                Location = new Models.LocationModel() { Name = "Lokation 2 " },
                Persons = new ObservableCollection<Models.PersonModel>() {
                    new Models.PersonModel() { Name = "Person 2 1 "},
                    new Models.PersonModel() { Name = "Person 2 2 "}
                },
                Team = new Models.TeamModel()
                {
                    Date = "02-08-2017 ",
                    Class = "8A ",
                    Time = "13:00:12 ",
                    ID = 2
                }

            });

            Itemsss.Add(new Models.TableModelN()
            {
                School = new Models.SchoolModel() { Name = "Skole 3 " },
                Location = new Models.LocationModel() { Name = "Lokation 3 " },
                Persons = new ObservableCollection<Models.PersonModel>() {
                    new Models.PersonModel() { Name = "Person 3 1 "},
                    new Models.PersonModel() { Name = "Person 3 2 "}
                },
                Team = new Models.TeamModel()
                {
                    Date = "03-08-2017 ",
                    Class = "8b ",
                    Time = "15:00:12 ",
                    ID = 3
                }

            });

            Itemsss.Add(new Models.TableModelN()
            {
                School = new Models.SchoolModel() { Name = "Skole 4 " },
                Location = new Models.LocationModel() { Name = "Lokation 4 " },
                Persons = new ObservableCollection<Models.PersonModel>() {
                    new Models.PersonModel() { Name = "Person 4 1 "},
                    new Models.PersonModel() { Name = "Person 4 2 "}
                },
                Team = new Models.TeamModel()
                {
                    Date = "04-08-2017 ",
                    Class = "8c ",
                    Time = "11:00:12 ",
                    ID = 4
                }

            });

            Itemsss.Add(new Models.TableModelN()
            {
                School = new Models.SchoolModel() { Name = "Skole 5 " },
                Location = new Models.LocationModel() { Name = "Lokation 5 " },
                Persons = new ObservableCollection<Models.PersonModel>() {
                    new Models.PersonModel() { Name = "Person 5 1 "},
                    new Models.PersonModel() { Name = "Person 5 2 "}
                },
                Team = new Models.TeamModel()
                {
                    Date = "02-08-2017 ",
                    Class = "9A ",
                    Time = "13:00:12 ",
                    ID = 5
                }

            });

            Itemsss.Add(new Models.TableModelN()
            {
                School = new Models.SchoolModel() { Name = "Skole 6 " },
                Location = new Models.LocationModel() { Name = "Lokation 6 " },
                Persons = new ObservableCollection<Models.PersonModel>() {
                    new Models.PersonModel() { Name = "Person 6 1 "},
                    new Models.PersonModel() { Name = "Person 6 2 "}
                },
                Team = new Models.TeamModel()
                {
                    Date = "02-06-2017 ",
                    Class = "8x ",
                    Time = "14:00:12 ",
                    ID = 6
                }

            });


            //debug_grid.DataContext = debug_item;
            Loaded += Sog_pa_tider_Loaded;
        }

        private void Sog_pa_tider_Loaded(object sender, RoutedEventArgs e)
        {
            BindingOperations.GetBindingExpressionBase(results, ListView.ItemsSourceProperty);
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

        private void Result_CheckAll(object sender, RoutedEventArgs e)
        {
            if (Print_ == 1) return;

            if(((CheckBox)sender).IsChecked ?? false)
                results.SelectAll();
            else
                results.SelectedIndex = -1;
        }

        
        private void Results_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Print_ = 1;
            printCheckAll.IsChecked = results.SelectedItems.Count == results.Items.Count;
            Print_ = 0;
        }


        private bool codeReSizeColumn = false;
        private void results_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!((GridViewColumnHeader)sender).IsLoaded)
                return;

            if (codeReSizeColumn)
                return;

            codeReSizeColumn = true;
            ((GridViewColumnHeader)sender).Width = e.PreviousSize.Width;
            codeReSizeColumn = false;
            //((GridViewColumnHeader)sender).SizeChanged += results_SizeChanged;
            //Console.WriteLine("Size! {0} == {1} == {2}", e.NewSize, e.PreviousSize, sender.GetType());
        }
    }
}
