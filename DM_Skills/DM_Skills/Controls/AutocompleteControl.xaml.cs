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
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DM_Skills.Controls
{
    /// <summary>
    /// Interaction logic for AutocompleteControl.xaml
    /// </summary>
    public partial class AutocompleteControl : UserControl
    {


        public ObservableCollection<string> ItemsSource
        {
            get { return (ObservableCollection<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<string>), typeof(AutocompleteControl), new PropertyMetadata(null));


        private IValueConverter ConvertVisibility;



        public AutocompleteControl()
        {
            InitializeComponent();

            ConvertVisibility = (IValueConverter)FindResource("BoolToVisibilityConvert");

            if (ItemsSource == null)
                ItemsSource = new ObservableCollection<string>();

            ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;


            ItemsSource.Add("Option 1");
            ItemsSource.Add("Option 2");
            ItemsSource.Add("Option 3");
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        var item = (string)e.NewItems[i];
                        var opt = new TextBlock();

                        opt.SetBinding(TextBlock.TextProperty, new Binding()
                        {
                            Source = item
                        });

                        //opt.SetBinding(TextBlock.VisibilityProperty, new Binding()
                        //{
                        //    Source = item,
                        //     Converter = ConvertVisibility
                        //});


                        options.Items.Add(opt);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                default:
                    break;
            }


        }
    }
}
