using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DM_Skills.Controls
{
    /// <summary>
    /// Klassen arver fra UserControls
    /// Klassen har DependencyProperty så vi kan lave data bindings
    /// udover det styre den også hvad der skal ske i bord oplysninger
    /// </summary>
    public partial class TablesControl : UserControl
    {
        

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TablesControl), new PropertyMetadata(""));
        
        public static readonly DependencyProperty SchoolProperty =
            DependencyProperty.Register("School", typeof(string), typeof(TablesControl), new PropertyMetadata(""));

        public static readonly DependencyProperty ClassIDProperty =
            DependencyProperty.Register("ClassID", typeof(string), typeof(TablesControl), new PropertyMetadata(""));

        public static readonly DependencyProperty TeamIDProperty =
            DependencyProperty.Register("TeamID", typeof(int), typeof(TablesControl));

        public static readonly DependencyProperty PlayersProperty =
            DependencyProperty.Register("Players", typeof(string), typeof(TablesControl), new PropertyMetadata(""));

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(TimeSpan), typeof(TablesControl));

        public static readonly DependencyProperty SchoolListProperty =
            DependencyProperty.Register("SchoolList", typeof(List<string>), typeof(TablesControl), new PropertyMetadata(new List<string>()));


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string School
        {
            get { return (string)GetValue(SchoolProperty); }
            set { SetValue(SchoolProperty, value); }
        }

        public string ClassID
        {
            get { return (string)GetValue(ClassIDProperty); }
            set { SetValue(ClassIDProperty, value); }
        }

        public int TeamID
        {
            get { return (int)GetValue(TeamIDProperty); }
            set { SetValue(TeamIDProperty, value); }
        }

        public string Players
        {
            get { return (string)GetValue(PlayersProperty); }
            set { SetValue(PlayersProperty, value); }
        }

        public TimeSpan Time
        {
            get { return (TimeSpan)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public List<string> SchoolList
        {
            get { return (List<string>)GetValue(SchoolListProperty); }
            set { SetValue(SchoolListProperty, value); }
        }


        /// <summary>
        /// Opretter de elementer der er i xaml
        /// </summary>
        public TablesControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Nulstil der der står i vores textboxes 
        /// som man har mulighed for at ændre
        /// </summary>
        public void Reset()
        {
            School = null;
            Time = new TimeSpan();
            ClassID = null;
            Players = null;

            BorderThickness = new Thickness(0);
        }

        private void Label_Drop(object sender, DragEventArgs e)
        {
            Console.WriteLine("Item Dropped");
        }
    }
}
