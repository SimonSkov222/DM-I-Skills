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
using System.Windows.Controls.Primitives;

namespace DM_Skills.Controls
{
    /// <summary>
    /// Interaction logic for AutocompleteControl.xaml
    /// </summary>
    public partial class AutocompleteControl : UserControl
    {



        public bool IsPopupOpen
        {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPopupOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register("IsPopupOpen", typeof(bool), typeof(AutocompleteControl), new PropertyMetadata(false));




        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placeholder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(AutocompleteControl), new PropertyMetadata(""));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        



        public ObservableCollection<Models.SchoolModel> ItemsSource
        {
            get { return (ObservableCollection<Models.SchoolModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value);}
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource", 
                typeof(ObservableCollection<Models.SchoolModel>), 
                typeof(AutocompleteControl), 
                new PropertyMetadata(null)
            );

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AutocompleteControl), new PropertyMetadata(""));
        

        private IMultiValueConverter ConvertVisibility;
        


        public AutocompleteControl()
        {

            ConvertVisibility = (IMultiValueConverter)FindResource("AutocompleteConvert");

            if (ItemsSource == null)
                ItemsSource = new ObservableCollection<Models.SchoolModel>();

            InitializeComponent();
            Loaded += AutocompleteControl_Loaded;
        }

        private void AutocompleteControl_Loaded(object sender, RoutedEventArgs e)
        {
            ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;
            ItemsSource_CollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, ItemsSource));
         
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        var item = ((Models.SchoolModel)e.NewItems[i]).Name;
                        var opt = new ListViewItem();

                        opt.SetBinding(ListViewItem.ContentProperty, new Binding()
                        {
                            Source = item
                        });

                        opt.SetBinding(ListViewItem.VisibilityProperty, new MultiBinding()
                        {
                            Bindings = {
                                new Binding() {
                                    Path= new PropertyPath("Text"),
                                    Source = input
                                },
                                new Binding() {
                                    Path= new PropertyPath("Content"),
                                    Source = opt
                                }
                            },
                            Converter = ConvertVisibility,
                            ConverterParameter = Converters.AutocompleteConverter.PARAMS_OPTION
                        });


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
            var visibleItems = options.Items.Cast<ListViewItem>().Where(o => o.Visibility == Visibility.Visible).ToArray();
            var cSelectedItem = visibleItems.Where(o => o.IsSelected).ToArray();
            int cID = cSelectedItem.Length == 0 ? -1 : Array.IndexOf(visibleItems, cSelectedItem[0]);
            switch (e.Key)
            {
                case Key.Down:
                    if(visibleItems.Length > 0 && cID < visibleItems.Length -1)
                        options.SelectedItem = visibleItems[cID+1];
                    break;
                case Key.Up:
                    if(visibleItems.Length > 0 && cID > 0)
                        options.SelectedItem = visibleItems[cID - 1];
                    break;

                case Key.Enter:
                    if (cSelectedItem.Length > 0)
                    {
                        input.Text = cSelectedItem[0].Content.ToString();
                        input.CaretIndex = input.Text.Length;
                    }
                    else if (visibleItems.Length > 0)
                    {
                        input.Text = visibleItems[0].Content.ToString();
                        input.CaretIndex = input.Text.Length;
                    }
                    break;
                case Key.Tab:
                    if (cSelectedItem.Length > 0)
                    {
                        input.Text = cSelectedItem[0].Content.ToString();
                        input.CaretIndex = input.Text.Length;
                    }
                    else if (visibleItems.Length > 0)
                    {
                        input.Text = visibleItems[0].Content.ToString();
                        input.CaretIndex = input.Text.Length;
                    }
                    break;
                case Key.Escape:
                    IsPopupOpen = false;
                    break;
            }



            NotifyPropertyChange("IsOpen");
        }


        private void NotifyPropertyChange(string propname) {
            switch (propname.ToLower())
            {
                case "IsOpen":
                    BindingOperations.GetBindingExpressionBase(popup, Popup.IsOpenProperty).UpdateTarget();
                    break;
            }
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsPopupOpen = true;
            options.SelectedIndex = -1;
            NotifyPropertyChange("IsOpen");
        }
        private void Options_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            input.Text = ((ListViewItem)sender).Content.ToString();
            input.CaretIndex = input.Text.Length;
        }
        
        private void Input_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IsPopupOpen = !IsPopupOpen;
        }

        private void Controller_LostFocus(object sender, RoutedEventArgs e)
        {
            IsPopupOpen = false;
        }
    }
}
