using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DM_Skills.Controls
{
    /// <summary>
    /// Klassen arver fra UserControls
    /// Klassen har DependencyProperty så vi kan lave data bindings
    /// udover det styre den også hvad der skal ske i bord oplysninger
    /// </summary>
    public partial class TablesControl : UserControl
    {


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TablesControl), new PropertyMetadata(""));




        public static readonly DependencyProperty ShowDropLocationProperty =
            DependencyProperty.Register("ShowDropLocation", typeof(bool), typeof(TablesControl), new PropertyMetadata(false));



        public Models.TableModelN Model
        {
            get { return (Models.TableModelN)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Model.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(Models.TableModelN), typeof(TablesControl), new PropertyMetadata(null));




        public ObservableCollection<Models.SchoolModel> Schools
        {
            get { return (ObservableCollection<Models.SchoolModel>)GetValue(SchoolsProperty); }
            set
            {
                Console.WriteLine("#######\n\n###\n\n");
                SetValue(SchoolsProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Schools.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SchoolsProperty =
            DependencyProperty.Register("Schools", typeof(ObservableCollection<Models.SchoolModel>), typeof(TablesControl), new PropertyMetadata(null));




        
        /// <summary>
        /// Opretter de elementer der er i xaml
        /// </summary>
        public TablesControl()
        {

            Console.WriteLine("N---N");

            if (Model == null)
                Model = new Models.TableModelN();
            if (Schools == null)
                Schools = new ObservableCollection<Models.SchoolModel>()
                {
                    new Models.SchoolModel() {Name = "Hej"}
                };

            Schools.CollectionChanged += (o, e) => BindingOperations.GetBindingExpressionBase(autoSchools, AutocompleteControl.ItemsSourceProperty).UpdateSource();


            InitializeComponent();
        }

        /// <summary>
        /// Nulstil der der står i vores textboxes 
        /// som man har mulighed for at ændre
        /// </summary>
        public void Reset()
        {
            Model = new Models.TableModelN();
            Personer.Reset();
        }

        private void Label_Drop(object sender, DragEventArgs e)
        {
            txt_time.Text = ((Label)e.Data.GetData(typeof(Label))).Content.ToString();
            BindingOperations.GetBindingExpressionBase(txt_time, PlaceholderBox.TextProperty).UpdateSource();
            //Model.Team.Time = ((Label)e.Data.GetData(typeof(Label))).Content.ToString(); ;


        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Schools.Add(new Models.SchoolModel() { Name = "HH" });
            Model.Persons.Add(new Models.PersonModel() { Name = "HH" });
            Schools.Add(new Models.SchoolModel() { Name = "HH" });
        }
    }
}
