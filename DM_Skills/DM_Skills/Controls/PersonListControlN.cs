using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DM_Skills.Controls
{
    public class PersonListControlN : ListView
    {
        Grid PART_ROOT;
        StackPanel PART_LIST;
        Grid PART_NEW_INPUT;
        Popup PART_POPUP;


        private Style styleTxtInput;
        private Style styleBtnInput;

        


        static PersonListControlN()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PersonListControlN), new FrameworkPropertyMetadata(typeof(PersonListControlN)));

        }

        public PersonListControlN() {

            
           
            ((INotifyCollectionChanged)Items).CollectionChanged += PersonListControlN_CollectionChanged;
            //((INotifyCollectionChanged)ItemsSource).CollectionChanged += PersonListControlN_CollectionChanged;

            styleTxtInput = (Style)FindResource("TextBox_Style_Default");
            styleBtnInput = (Style)FindResource("Button_Style_Default");

            Loaded += (o, e) =>
            {
                Persons = new ObservableCollection<Models.PersonModel>();
                Persons.CollectionChanged += PersonListControlN_CollectionChanged;
                foreach (var ls in Items)
                {
                    Persons.Add((Models.PersonModel)ls);
                }
                ItemsSource = Persons;

                Persons.Add(new Models.PersonModel() { Name = "dd" });
                BindingOperations.GetBindingExpressionBase(this, ItemsSourceProperty).UpdateTarget();
                Console.Write("Person!");
                Console.WriteLine(Items.Count);

                
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ROOT = (Grid)GetTemplateChild("PART_ROOT");
            PART_LIST = (StackPanel)GetTemplateChild("PART_LIST");
            PART_NEW_INPUT = (Grid)GetTemplateChild("PART_NEW_INPUT");
            PART_POPUP = (Popup)GetTemplateChild("PART_POPUP");


            //PART_ROOT.MouseEnter += Animation_Slide;
            //PART_ROOT.MouseLeave += Animation_Slide;
            //PART_POPUP.MouseEnter += Animation_Slide;
            //PART_POPUP.MouseLeave += Animation_Slide;



            InsertNewInputBox();
        }
        private ObservableCollection<Models.PersonModel> Persons;

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

           
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            Console.WriteLine("Change");
        }


        private void PersonListControlN_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Item Change");
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        var item = e.NewItems[i];


                        if (PART_NEW_INPUT.Children.Count == 0)
                        {
                            //Opret input element og find  fjern knappen
                            var input = BuildInputElement();
                            var btn = (Button)input.Children[1];

                            //Sæt textboxen bindingen
                            input.DataContext = item;
                            //Tilføj event
                            btn.Click += Btn_Click;

                            //Tilføj til view
                            PART_LIST.Children.Add(input);
                        }
                        else
                        {
                            //Find nuværnde input element/textbox og button
                            var input = (Grid)PART_NEW_INPUT.Children[0];
                            var txt = (TextBox)input.Children[0];
                            var btn = (Button)input.Children[1];

                            //Fjern event
                            txt.TextChanged -= Txt_TextChanged;

                            //Tilføj event
                            btn.Click += Btn_Click;

                            //Går at bindingen på textbox vil skifte til det nye item der er tilføjet
                            input.DataContext = item;

                            //Skift parent for input element
                            PART_NEW_INPUT.Children.Clear();
                            PART_LIST.Children.Add(input);

                            //Lav et nyt input element
                            InsertNewInputBox();
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:

                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        var item = e.OldItems[i];
                        var input = PART_LIST.Children.Cast<UIElement>().Where(o => o is Grid && ((Grid)o).DataContext == item).First();
                        PART_LIST.Children.Remove(input);

                    }
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
            //NotifyPropertyChanged("Persons");
        }




        private Grid BuildInputElement()
        {
            var input = new Grid();
            var txt = new TextBox();
            var btn = new Button();

            //Placering
            input.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            input.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            Grid.SetColumn(txt, 0);
            Grid.SetColumn(btn, 1);

            //Model
            input.DataContext = new Models.PersonModel();
            txt.SetBinding(TextBox.TextProperty, new Binding()
            {
                Path = new PropertyPath("Name"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            //Events
            //txt.TextChanged += (o, e) => NotifyPropertyChanged("Persons");

            //Styling
            txt.Style = styleTxtInput;
            txt.ToolTip = "Navn";
            txt.Margin = new Thickness(0, 5, 0, 0);
            txt.Height = 30;
            txt.TabIndex = 2000 + Items.Count;

            btn.Style = styleBtnInput;
            btn.Content = "X";
            btn.FontWeight = FontWeights.Bold;
            btn.Foreground = (Brush)FindResource("Color_Foreground_Light_1");
            btn.Background = (Brush)FindResource("Color_Background_Button_LightRed");
            btn.Margin = new Thickness(6, 5, 0, 0);
            btn.Padding = new Thickness(0);
            btn.Width = 30;
            btn.Height = 30;

            //Fjern tab
            FocusManager.SetIsFocusScope(btn, false);
            KeyboardNavigation.SetIsTabStop(btn, false);


            //view
            input.Children.Add(txt);
            input.Children.Add(btn);

            return input;
        }



        private void InsertNewInputBox()
        {
            var input = BuildInputElement();
            var txt = (TextBox)input.Children[0];

            //Events
            txt.TextChanged += Txt_TextChanged;

            //Add til view
            PART_NEW_INPUT.Children.Add(input);
        }

        private void Animation_Slide(object sender, MouseEventArgs e)
        {
            int speed = 1000;//msec
            double maxHeight = (PART_POPUP.Child as Border).MaxHeight;


            DoubleAnimation animation = null;
            if ((sender as UIElement).IsMouseOver)
            {
                int msec = (int)Math.Round((1 - maxHeight / 150) * speed);
                animation = new DoubleAnimation()
                {
                    Duration = new TimeSpan(0, 0, 0, 0, msec),
                    From = maxHeight,
                    To = 150,
                };
            }
            else
            {
                int msec = (int)Math.Round((maxHeight / 150) * speed);
                animation = new DoubleAnimation()
                {
                    Duration = new TimeSpan(0, 0, 0, 0, msec),
                    From = maxHeight,
                    To = 0,
                };
            }

            (PART_POPUP.Child as Border).BeginAnimation(Border.MaxHeightProperty, null);
            (PART_POPUP.Child as Border).BeginAnimation(Border.MaxHeightProperty, animation);
        }



        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var input = (Grid)(sender as Button).Parent;
            Items.Remove((Models.PersonModel)input.DataContext);
        }


        private void Txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Console.Write("Txt");
            Console.Write((sender as TextBox).IsLoaded);


            if (!(sender as TextBox).IsLoaded) return;

            var input = (Grid)PART_NEW_INPUT.Children[0];
            var txt = (TextBox)input.Children[0];
            var model = (Models.PersonModel)input.DataContext;
            Console.WriteLine("Hej");
            //Items.Add(model);
            ((IList<Models.PersonModel>)ItemsSource).Add(model);
            //AddChild(model);
            Console.WriteLine("Done");
        }
    }
}
