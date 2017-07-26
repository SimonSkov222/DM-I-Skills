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


        //private static void PersonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as PersonListControl).InsertNewInputBox();
            
        //}


        public PersonListControl()
        {
            InitializeComponent();
            styleTxtInput = (Style)FindResource("TextBox_Style_Default");
            styleBtnInput = (Style)FindResource("Button_Style_Default");

            Persons.CollectionChanged += Persons_CollectionChanged;

            if (Persons.Count == 0)
                Persons.Add(new Models.PersonModel());


        }

        private void Persons_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InsertNewInputBox();
        }

        private void InsertNewInputBox()
        {
            var txtName = new TextBox();
            var btnDelete = new Button();

            //Sæt placering
            int rowID = gridListOfPlayers.RowDefinitions.Count;
            gridListOfPlayers.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            //gridListOfPlayers.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(txtName, rowID);
            Grid.SetRow(btnDelete, rowID);
            Grid.SetColumn(txtName, 0);
            Grid.SetColumn(btnDelete, 1);


            //Styling
            txtName.Style = styleTxtInput;
            txtName.ToolTip = "Navn";
            txtName.Margin = new Thickness(0, 5, 0, 0);
            txtName.Height = 30;

            btnDelete.Style = styleBtnInput;
            btnDelete.Content = "X";
            btnDelete.FontWeight = FontWeights.Bold;
            btnDelete.Foreground = (Brush)FindResource("Color_Foreground_Light_1");
            btnDelete.Background = (Brush)FindResource("Color_Background_Button_LightRed");
            btnDelete.Margin = new Thickness(6, 5, 0, 0);
            btnDelete.Padding = new Thickness(0);
            btnDelete.Width = 30;
            btnDelete.Height = 30;


            //Event
            txtName.TextChanged += (o, e) =>
            {
                if ((o as TextBox).Text == null || (o as TextBox).Text == "")
                    return;
                if (IsInputElementLast((UIElement)o))
                    InsertNewInputBox();
            };

            btnDelete.Click += (o, e) =>
            {
                if (!IsInputElementLast((UIElement)o))
                    DeleteInputBoxByRowID(Grid.GetRow((UIElement)o));
            };


            //Add to view
            gridListOfPlayers.Children.Add(txtName);
            gridListOfPlayers.Children.Add(btnDelete);


            // < TextBox Grid.Column = "0" ToolTip = "Navn" Style = "{StaticResource TextBox_Style_Default}" VerticalAlignment = "Top" />
            //       < Button Grid.Column = "1" Content = "X" 
            //FontWeight = "Bold" Foreground = "White" Background = "Red" 
            //    Style = "{StaticResource Button_Style_Default}" Width = "30" Height = "30" Margin = "6 0 0 0" Padding = "0" />


        }
        
        private void DeleteInputBoxByRowID(int id)
        {
            var inputs = gridListOfPlayers.Children.Cast<UIElement>().Where(o => Grid.GetRow(o) == id).ToArray();
            foreach (var item in inputs)
            {
                gridListOfPlayers.Children.Remove(item);
            }

            if (gridListOfPlayers.Children.Count == 0)
                InsertNewInputBox();

        }

        private bool IsInputElementLast(UIElement sender)
        {
            int rID = Grid.GetRow(sender);
            return gridListOfPlayers.Children.Cast<UIElement>().Count(i => Grid.GetRow(i) > rID) == 0;
        }

        private void dropdownBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Border Size {0}", (sender as Border).ActualWidth);
        }

        private void debug_MouseEnter(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Label Size {0}", (sender as Label).ActualWidth);

        }
    }
}
