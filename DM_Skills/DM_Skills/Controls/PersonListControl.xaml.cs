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
    /// Interaction logic for PersonListControl.xaml
    /// </summary>
    public partial class PersonListControl : UserControl
    {
        public static readonly DependencyProperty PersonsProperty =
            DependencyProperty.Register(
                "Persons", 
                typeof(ObservableCollection<Models.PersonModel>), 
                typeof(PersonListControl), 
                new PropertyMetadata(
                    new ObservableCollection<Models.PersonModel>()
                )
            );

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(PersonListControl), new PropertyMetadata("Title"));



        public ObservableCollection<Models.PersonModel> Persons
        {
            get { return (ObservableCollection<Models.PersonModel>)GetValue(PersonsProperty); }
            set { SetValue(PersonsProperty, value); }
        }
        
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        
        
        
        private Style styleTxtInput;
        private Style styleBtnInput;
        private ControlTemplate templatePerson;


        //private static void PersonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as PersonListControl).InsertNewInputBox();
            
        //}


        public PersonListControl()
        {
            InitializeComponent();
            styleTxtInput = (Style)FindResource("TextBox_Style_Default");
            styleBtnInput = (Style)FindResource("Button_Style_Default");
            templatePerson = (ControlTemplate)FindResource("person");

            Persons.CollectionChanged += Persons_CollectionChanged;

            if (Persons.Count == 0)
                Persons.Add(new Models.PersonModel() { Name = "Debug 1" });

            for (int i = 2; i < 5; i++)
            {

                Persons.Add(new Models.PersonModel() { Name = "Debug "+i });
            }


        }

        private void Persons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    int maxID = Persons.Count -1;
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        var item = e.NewItems[i] as Models.PersonModel;
                        int oldPos = e.NewStartingIndex + i;
                        int newPos = Persons.Count-2;
                        int currentID = e.NewStartingIndex + i;

                        Console.WriteLine("Add: {0}", item.Name);
                        Console.WriteLine("{0} - {1} - {2} - {3}", oldPos, newPos, currentID, maxID);

                        InsertNewInputBox(item);

                        if (currentID == maxID && maxID > 0)
                        {
                            Persons.Move(currentID, maxID - 1);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Console.WriteLine("Remove");
                    int lastID = Persons.Count + e.OldItems.Count -1;

                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        var item = e.OldItems[i] as Models.PersonModel;
                        DeleteInputBox(item);

                        Console.WriteLine("{0} - {1}", lastID, e.OldStartingIndex + i);

                        if (lastID == e.OldStartingIndex + i)
                        {
                            Console.WriteLine("R Add");
                            Persons.Add(item);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Console.WriteLine("Replace");
                    break;
                case NotifyCollectionChangedAction.Move:
                    Console.WriteLine("Move");

                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        var item = e.NewItems[i] as Models.PersonModel;
                        DeleteInputBox(item);
                        InsertNewInputBox(item, e.NewStartingIndex + i);
                    }

                    //stackPanel1.Children.Remove(item);
                    //stackPanel1.Children.Insert(newIndex, item);


                    //listOfPersons.Children.RemoveAt(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Console.WriteLine("Reset");
                    break;
                default:
                    break;
            }
        }

        private void InsertNewInputBox(Models.PersonModel model, int index = -1)
        {
            var item = new Label();
            item.Template = templatePerson;
            item.SetBinding(Label.ContentProperty, new Binding()
            {
                Source = model
            });
            

            if (index == -1)
                listOfPersons.Children.Add(item);
            else
                listOfPersons.Children.Insert(index, item);


            //var txtName = new TextBox();
            //var btnDelete = new Button();

            ////Sæt placering
            //int rowID = gridListOfPlayers.RowDefinitions.Count;
            //gridListOfPlayers.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            ////gridListOfPlayers.RowDefinitions.Add(new RowDefinition());

            //Grid.SetRow(txtName, rowID);
            //Grid.SetRow(btnDelete, rowID);
            //Grid.SetColumn(txtName, 0);
            //Grid.SetColumn(btnDelete, 1);


            ////Styling
            //txtName.Style = styleTxtInput;
            //txtName.ToolTip = "Navn";
            //txtName.Margin = new Thickness(0, 5, 0, 0);
            //txtName.Height = 30;

            //btnDelete.Style = styleBtnInput;
            //btnDelete.Content = "X";
            //btnDelete.FontWeight = FontWeights.Bold;
            //btnDelete.Foreground = (Brush)FindResource("Color_Foreground_Light_1");
            //btnDelete.Background = (Brush)FindResource("Color_Background_Button_LightRed");
            //btnDelete.Margin = new Thickness(6, 5, 0, 0);
            //btnDelete.Padding = new Thickness(0);
            //btnDelete.Width = 30;
            //btnDelete.Height = 30;


            ////Event
            //txtName.TextChanged += (o, e) =>
            //{
            //    if ((o as TextBox).Text == null || (o as TextBox).Text == "")
            //        return;
            //    if (IsInputElementLast((UIElement)o))
            //        InsertNewInputBox();
            //};

            //btnDelete.Click += (o, e) =>
            //{
            //    if (!IsInputElementLast((UIElement)o))
            //        DeleteInputBoxByRowID(Grid.GetRow((UIElement)o));
            //};


            ////Add to view
            //gridListOfPlayers.Children.Add(txtName);
            //gridListOfPlayers.Children.Add(btnDelete);


            // < TextBox Grid.Column = "0" ToolTip = "Navn" Style = "{StaticResource TextBox_Style_Default}" VerticalAlignment = "Top" />
            //       < Button Grid.Column = "1" Content = "X" 
            //FontWeight = "Bold" Foreground = "White" Background = "Red" 
            //    Style = "{StaticResource Button_Style_Default}" Width = "30" Height = "30" Margin = "6 0 0 0" Padding = "0" />


        }

        private void DeleteInputBox(Models.PersonModel model)
        {
            foreach (var deleteMe in FindInputBox(model))
            {
                listOfPersons.Children.Remove(deleteMe);
            }
        }

        private bool IsInputElementLast(Models.PersonModel model)
        {
            return Persons.IndexOf(model) == Persons.Count - 1;
        }

        private UIElement[] FindInputBox(Models.PersonModel model)
        {
            return listOfPersons.Children.Cast<UIElement>().Where(o => o is Label && ((Label)o).Content == model).ToArray();
        }

        private void dropdownBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Border Size {0}", (sender as Border).ActualWidth);
        }

        private void debug_MouseEnter(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Label Size {0}", (sender as Label).ActualWidth);

        }

        private void Button_DeleteInput_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).Tag as Models.PersonModel;
            if (!IsInputElementLast(item))
                Persons.Remove(item);
        }

        private void TextBox_Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = (TextBox)sender;
            if (IsInputElementLast(txt.Tag as Models.PersonModel) && txt.Text != null && txt.Text != "" )
            {
                Persons.Add(new Models.PersonModel());
            }
        }
    }
}
