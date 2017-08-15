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


        public ObservableCollection<Models.LocationModel> Locations {
            get
            {
                var result = Models.LocationModel.GetAll();
                result.Insert(0, new Models.LocationModel() { Name = "Vælg lokation" });

                return result;
            }
        }


        private int Print_ = 0;
        

        public sog_pa_tider()
        {
            InitializeComponent();
            ItemSourceSearch = new ObservableCollection<Models.TableModelN>();
            for (int i = 0; i < 100; i++)
            {
                ItemSourceSearch.Add(new Models.TableModelN()
                {
                    School = new Models.SchoolModel() { Name = "Skole "+(i +1) },
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

            if(((CheckBox)sender).IsChecked ?? false)
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


        //private bool codeReSizeColumn = false;
        //private void results_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    if (!((GridViewColumnHeader)sender).IsLoaded)
        //        return;

        //    if (codeReSizeColumn)
        //        return;

        //    codeReSizeColumn = true;
        //    ((GridViewColumnHeader)sender).Width = e.PreviousSize.Width;
        //    codeReSizeColumn = false;
        //    //((GridViewColumnHeader)sender).SizeChanged += results_SizeChanged;
        //    //Console.WriteLine("Size! {0} == {1} == {2}", e.NewSize, e.PreviousSize, sender.GetType());
        //}

        private  void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var scrollviewer = (ScrollViewer)Scripts.Helper.FindAncestor(this, typeof(ScrollViewer));

            scrollviewer.ScrollChanged += (o, ee) =>
            {
                var margin = ((Grid)sender).Margin;
                margin.Top = ((ScrollViewer)o).VerticalOffset;
                ((Grid)sender).Margin = margin;
            };
        }

        private async void Button_Print_Click(object sender, RoutedEventArgs e)
        {
            var items = ItemSourceSearch;

            Console.WriteLine("Start");
            var createPDF = Task.Run(() => {
                var print = new Scripts.Print();
                print.CreatePDF(@"C:\Users\shsk\Desktop\Debug\PDFF.pdf", items);
                Console.WriteLine("Done1");
            });

            Console.WriteLine("Middle");
            var printd = new PrintDialog();
            var result = printd.ShowDialog();

            if(result ?? false)
            {
                //FlowDocument
               // printd.PrintDocument()
                printd.PrintVisual(searchList, "RR");
                //printd.do
                Console.WriteLine("Print nu");
            }


            Console.WriteLine("Done");

            //var wPrint = new Views.Udskriv();
            //wPrint.Owner = App.Current.MainWindow;
            //wPrint.ShowInTaskbar = false;
            //wPrint.ResizeMode = ResizeMode.NoResize;
            //wPrint.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            //wPrint.ShowDialog();
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {

            var school = txtSchoolName.Text;
            var person = txtDeltager.Text;
            var from = dtFrom.SelectedDate == null ? "" : dtFrom.SelectedDate.Value.ToShortDateString();
            var to = dtTo.SelectedDate == null ? "" : dtTo.SelectedDate.Value.ToShortDateString();
            var location = cbLocation.SelectedIndex == 0 ? null : cbLocation.SelectedItem as Models.LocationModel;

            var items = Models.TableModelN.GetTables(null, school, person, location, from, to);
            searchList.ItemsSource = items;
        }
    }
}
