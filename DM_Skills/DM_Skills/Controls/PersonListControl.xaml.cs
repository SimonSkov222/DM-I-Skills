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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Animation;
using System.Collections;

namespace DM_Skills.Controls
{
    /// <summary>
    /// Interaction logic for PersonListControl.xaml
    /// </summary>
    public partial class PersonListControl : UserControl, INotifyPropertyChanged
    {


        public bool WindowIsFocused
        {
            get { return (bool)GetValue(WindowIsFocusedProperty); }
            set { SetValue(WindowIsFocusedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WindowIsFocused.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowIsFocusedProperty =
            DependencyProperty.Register("WindowIsFocused", typeof(bool), typeof(PersonListControl), new PropertyMetadata(false));


        public bool Error
        {
            get { return (bool)GetValue(ErrorProperty); }
            set { SetValue(ErrorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Error.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorProperty =
            DependencyProperty.Register("Error", typeof(bool), typeof(PersonListControl), new PropertyMetadata(false, new PropertyChangedCallback(CallBackProperty)));

        private Brush OldBrush;

        public static void CallBackProperty(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (PersonListControl)sender;
            if ((bool)e.NewValue)
            {
                control.OldBrush = control.BorderBrush;
                control.BorderBrush = Brushes.Red;
            }
            else if (control.OldBrush != null)
            {
                control.BorderBrush = control.OldBrush;
            }
        }


        public static readonly DependencyProperty PersonsProperty =
            DependencyProperty.Register(
                "Persons",
                typeof(ObservableCollection<Models.PersonModel>),
                typeof(PersonListControl),
                new PropertyMetadata(null)
            );



        public ObservableCollection<Models.PersonModel> Persons
        {
            get { return (ObservableCollection<Models.PersonModel>)GetValue(PersonsProperty); }
            set
            {
                SetValue(PersonsProperty, value);

                value.CollectionChanged += Persons_CollectionChanged;
                Persons_CollectionChanged(value, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
                NotifyPropertyChanged(nameof(Persons));
                listOfPersons.Children.Clear();
            }
        }


        public bool IsPopupOpen
        {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPopupOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register("IsPopupOpen", typeof(bool), typeof(PersonListControl), new PropertyMetadata(false));





        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placeholder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(PersonListControl), new PropertyMetadata(""));


        public bool HasPersons
        {
            get { return Persons != null && Persons.Count > 0; }
        }

        public ObservableCollection<Models.PersonModel> PersonsS = new ObservableCollection<Models.PersonModel>();



        private Style styleTxtInput;
        private Style styleBtnInput;
        private IMultiValueConverter errorConverter;
        private bool HasLoaded = false;

        public PersonListControl()
        {
            InitializeComponent();
            styleTxtInput = (Style)FindResource("TextBox_Style_Default");
            styleBtnInput = (Style)FindResource("Button_Style_Default");
            errorConverter = (IMultiValueConverter)FindResource("ErrorConvert");

            if (Persons == null)
                Persons = new ObservableCollection<Models.PersonModel>();
            


            InsertNewInputBox();

            Loaded += PersonListControl_Loaded;
            Loaded += (oo, ee) =>
            {
                if (!HasLoaded)
                {
                    var win = Window.GetWindow(this);
                    win.Activated += (o, e) => WindowIsFocused = true;
                    win.Deactivated += (o, e) => WindowIsFocused = false;
                    WindowIsFocused = win.IsActive;
                    HasLoaded = true;
                }
            };

            GotFocus += (o,e) => { Console.WriteLine("GotFocus"); };
        }
        

        private void PersonListControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Persons == null)
                return;

            Persons.CollectionChanged += Persons_CollectionChanged;
            Persons_CollectionChanged(Persons, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Persons));

        }

        private void Txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender as TextBox).IsLoaded) return;
            (sender as TextBox).TextChanged -= Txt_TextChanged;

            var input = (Grid)newPerson.Children[0];
            var txt = (TextBox)input.Children[0];
            var model = (Models.PersonModel)input.DataContext;
            Persons.Add(model);
        }
        


        private void Persons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        var item = e.NewItems[i];
                        

                        if (newPerson.Children.Count == 0) {
                            //Opret input element og find  fjern knappen
                            var input = BuildInputElement();
                            var btn = (Button)input.Children[1];

                            //Sæt textboxen bindingen
                            input.DataContext = item;
                            //Tilføj event
                            btn.Click += Btn_Click;
                            
                            
                            //Tilføj til view
                            listOfPersons.Children.Add(input);
                        }
                        else
                        {
                            //Find nuværnde input element/textbox og button
                            var input = (Grid)newPerson.Children[0];
                            var txt = (PlaceholderBox)input.Children[0];
                            var btn = (Button)input.Children[1];

                            //Fjern event
                            txt.TextChanged -= Txt_TextChanged;

                            //Tilføj event
                            btn.Click += Btn_Click;

                            //Går at bindingen på textbox vil skifte til det nye item der er tilføjet
                            input.DataContext = item;

                            txt.SetBinding(PlaceholderBox.ErrorProperty, new MultiBinding()
                            {
                                Bindings =
                                {
                                    new Binding() {
                                        Source = this,
                                        Path = new PropertyPath("Error")
                                    },
                                    new Binding() {
                                        Source = item,
                                        Path = new PropertyPath("CanUpload")
                                    }
                                },
                                Converter = errorConverter
                            });





                            //Skift parent for input element
                            newPerson.Children.Clear();
                            listOfPersons.Children.Add(input);

                            //Lav et nyt input element
                            InsertNewInputBox();
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:

                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        var item = e.OldItems[i];
                        var input = listOfPersons.Children.Cast<UIElement>().Where(o => o is Grid && ((Grid)o).DataContext == item).First();
                        listOfPersons.Children.Remove(input);

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
            NotifyPropertyChanged("Persons");
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var input = (Grid)(sender as Button).Parent;
            Persons.Remove((Models.PersonModel)input.DataContext);
            PersonsS.Remove((Models.PersonModel)input.DataContext);
        }

        private Grid BuildInputElement()
        {
            var input = new Grid();
            var txt = new PlaceholderBox();
            var btn = new Button();

            //Placering
            input.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            input.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            Grid.SetColumn(txt, 0);
            Grid.SetColumn(btn, 1);

            //Model
            input.DataContext = new Models.PersonModel();
            txt.SetBinding(PlaceholderBox.TextProperty, new Binding()
            {
                Path = new PropertyPath("Name"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            //Events
            txt.TextChanged += (o,e) => NotifyPropertyChanged("Persons");

            //Styling
            //txt.Style = styleTxtInput;
            txt.Placeholder = "Navn";
            txt.Margin = new Thickness(0, 5, 0, 0);
            txt.Height = 30;
            txt.TabIndex = 2000 + Persons.Count;

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
            var txt = (PlaceholderBox)input.Children[0];

            //Events
            txt.TextChanged += Txt_TextChanged;

            //Add til view
            newPerson.Children.Add(input);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (!this.IsLoaded)
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == nameof(Persons))
            {
                BindingOperations.GetBindingExpressionBase(display.Content as TextBlock, TextBlock.TextProperty).UpdateTarget();
                NotifyPropertyChanged(nameof(HasPersons));
            }
        }

        public void Reset()
        {
            Persons = new ObservableCollection<Models.PersonModel>();

        }


        private void Animation_Slide(object sender, MouseEventArgs e) {
            int speed = 300;//msec

            DoubleAnimation animation = null;
            if ((sender as UIElement).IsMouseOver)
            {
                IsPopupOpen = true;
                int msec = (int)Math.Round((1 - dropdownBorder.MaxHeight / 150) * speed);
                animation = new DoubleAnimation()
                {
                    Duration = new TimeSpan(0, 0, 0, 0, msec),
                    From = dropdownBorder.MaxHeight,
                    To = 150,
                };
            }
            else
            {
                int msec = (int)Math.Round((dropdownBorder.MaxHeight / 150) * speed);
                animation = new DoubleAnimation()
                {
                    Duration = new TimeSpan(0, 0, 0, 0, msec),
                    From = dropdownBorder.MaxHeight,
                    To = 0
                };

                animation.Completed += (o, ee) => { IsPopupOpen = IsMouseOver || popup.IsMouseOver; };
            }

            dropdownBorder.BeginAnimation(Border.MaxHeightProperty, null);
            dropdownBorder.BeginAnimation(Border.MaxHeightProperty, animation);
        }

        private delegate void SimpleDelegate();
        private void display_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var input = newPerson.Children[0] as Grid;
            var txt = (PlaceholderBox)input.Children[0];

            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.ApplicationIdle ,
                new SimpleDelegate(delegate() { txt.Focus(); })
            );
            //txt.Focusable = true;
            //FocusManager.SetIsFocusScope(txt, true);
            //FocusManager.SetFocusedElement(this, txt);
            //Keyboard.Focus(txt);
            //txt.SelectionStart = txt.Text.Length;



            //FocusManager.SetFocusedElement(this, txt);

            //Key key = Key.Enter;                    // Key to send
            //var target = Keyboard.FocusedElement;    // Target element
            //RoutedEvent routedEvent = Keyboard.KeyDownEvent; // Event to send

            //target.RaiseEvent(
            //    new KeyEventArgs(
            //        Keyboard.PrimaryDevice,
            //        PresentationSource.FromVisual(txt),
            //        0,
            //        key)
            //    { RoutedEvent = routedEvent }
            //);
        }
    }
}
