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

namespace DM_Skills.Controls
{
    /// <summary>
    /// Interaction logic for TablesControl.xaml
    /// </summary>
    public partial class TablesControl : UserControl
    {


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TablesControl), new PropertyMetadata(""));




        public string School
        {
            get { return (string)GetValue(SchoolProperty); }
            set { SetValue(SchoolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SchoolProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SchoolProperty =
            DependencyProperty.Register("School", typeof(string), typeof(TablesControl), new PropertyMetadata(""));

        



        public string ClassID
        {
            get { return (string)GetValue(ClassIDProperty); }
            set { SetValue(ClassIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClassID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClassIDProperty =
            DependencyProperty.Register("ClassID", typeof(string), typeof(TablesControl), new PropertyMetadata(""));




        public int TeamID
        {
            get { return (int)GetValue(TeamIDProperty); }
            set { SetValue(TeamIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TeamID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TeamIDProperty =
            DependencyProperty.Register("TeamID", typeof(int), typeof(TablesControl));



        public string Players
        {
            get { return (string)GetValue(PlayersProperty); }
            set { SetValue(PlayersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Players.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayersProperty =
            DependencyProperty.Register("Players", typeof(string), typeof(TablesControl), new PropertyMetadata(""));



        public TimeSpan Time
        {
            get { return (TimeSpan)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Time.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(TimeSpan), typeof(TablesControl));







        public TablesControl()
        {
            InitializeComponent();
            
        }
    }
}
