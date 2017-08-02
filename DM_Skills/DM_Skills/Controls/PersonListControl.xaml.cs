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

namespace DM_Skills.Controls
{
    /// <summary>
    /// Interaction logic for PersonListControl.xaml
    /// </summary>
    public partial class PersonListControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty PersonsProperty =
            DependencyProperty.Register(
                "Persons", 
                typeof(ObservableCollection<Models.PersonModel>), 
                typeof(PersonListControl), 
                new PropertyMetadata(new ObservableCollection<Models.PersonModel>())
            );

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(PersonListControl), new PropertyMetadata("Title"));



        public ObservableCollection<Models.PersonModel> Persons
        {
            get { return (ObservableCollection<Models.PersonModel>)GetValue(PersonsProperty); }
            set { SetValue(PersonsProperty, value); NotifyPropertyChanged(nameof(Persons)); }
        }
        
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); NotifyPropertyChanged(nameof(Title)); }
        }



        public bool HasPersons
        {
            get { return Persons != null && Persons.Count > 0; }
        }




        private Style styleTxtInput;
        private Style styleBtnInput;



        public PersonListControl()
        {
            InitializeComponent();
            styleTxtInput = (Style)FindResource("TextBox_Style_Default");
            styleBtnInput = (Style)FindResource("Button_Style_Default");

            if (Persons == null)
                Persons = new ObservableCollection<Models.PersonModel>();

            Persons.CollectionChanged += Persons_CollectionChanged;

            InsertNewInputBox();
        }

        
        private void Txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender as TextBox).IsLoaded) return;

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
                            var txt = (TextBox)input.Children[0];
                            var btn = (Button)input.Children[1];

                            //Fjern event
                            txt.TextChanged -= Txt_TextChanged;

                            //Tilføj event
                            btn.Click += Btn_Click;

                            //Går at bindingen på textbox vil skifte til det nye item der er tilføjet
                            input.DataContext = item;

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
            txt.TextChanged += (o,e) => NotifyPropertyChanged("Persons");

            //Styling
            txt.Style = styleTxtInput;
            txt.ToolTip = "Navn";
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
            var txt = (TextBox)input.Children[0];

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == nameof(Persons))
            {
                BindingOperations.GetBindingExpressionBase((TextBlock)display.Content, TextBlock.TextProperty).UpdateTarget();
                NotifyPropertyChanged(nameof(HasPersons));
            }
        }


        private void Animation_Slide(object sender, MouseEventArgs e) {
            int speed = 1000;//msec


            DoubleAnimation animation = null;
            if ((sender as UIElement).IsMouseOver)
            {
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
                    To = 0,
                };
            }

            dropdownBorder.BeginAnimation(Border.MaxHeightProperty, null);
            dropdownBorder.BeginAnimation(Border.MaxHeightProperty, animation);
        }


        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Move");
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Leave");
        }
    }
}
