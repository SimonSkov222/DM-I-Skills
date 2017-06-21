using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;   // ObservableCollection.
using System.Collections.Specialized;   // NotifyCollectionChangedEventHandler.
using System.Windows.Markup;            // [ContentProperty()]

namespace DM_Skills.Controls {
    /// <summary>
    /// Klassen arver fra Control
    /// Klassen bruge til at give en textbox en dropdown liste
    /// med mulighed for autoudfyldelse
    /// </summary>
	[ContentProperty("Children")] //Gør at vi i xaml kan bruge <a>item</a> i stedet for <a><a.item>item</a.item></a>
    [TemplatePart(Name="PART_CONTENT", Type= typeof(StackPanel))] //Gør hvis man opretter flere template skal den have dette element med navnet
	public class Autocomplete : Control {

        public static readonly DependencyProperty ChildrenProperty          = DependencyProperty.Register("Children", typeof(ObservableCollection<string>), typeof(Autocomplete), new PropertyMetadata(new ObservableCollection<string>()));
        public static readonly DependencyProperty TargetProperty			= DependencyProperty.Register("Target",			typeof(TextBox),	typeof(Autocomplete));
		public static readonly DependencyProperty BorderRadiusProperty		= DependencyProperty.Register("BorderRadius",	typeof(CornerRadius),	typeof(Autocomplete));
		public static readonly DependencyProperty IsOpenProperty			= DependencyProperty.Register("IsOpen",			typeof(bool),	typeof(Autocomplete), new PropertyMetadata(false));
		
		public TextBox Target				{ get { return (TextBox)GetValue(TargetProperty); }					set { SetValue(TargetProperty, value); } }
		public CornerRadius BorderRadius	{ get { return (CornerRadius)GetValue(BorderRadiusProperty); }		set { SetValue(BorderRadiusProperty, value); } }
		public bool IsOpen					{ get { return (bool)GetValue(IsOpenProperty); }					set { SetValue(IsOpenProperty, value); } }
        

        public ObservableCollection<string> Children
        {
            get { return (ObservableCollection<string>)GetValue(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }
        
        //public ObservableCollection<string> Children	{ get; set; }
		
		public StackPanel childContainer;
		private int LastIndex = -1;
		private TextBlock LastSelected = null;
        private bool TextHasChange = false;
        private IMultiValueConverter VisibilityConvert = new Converters.AutocomplateConvert();


        /// <summary>
        /// Giver vores element en default style
        /// </summary>
        static Autocomplete() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Autocomplete),new FrameworkPropertyMetadata(typeof(Autocomplete)));
		}

        /// <summary>
        /// Opretter this.Children element
        /// </summary>
        public Autocomplete() {
			Children = new ObservableCollection<string>();
		}
		
        /// <summary>
        /// Tildeler forskellige event metoder samt opdater de this.Children
        /// der er blevet tilføjet så vi kan se dem
        /// </summary>
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			childContainer           = GetTemplateChild("PART_CONTENT") as StackPanel;

            //Textbox events
			Target.PreviewKeyDown   += Target_KeyDown;
			Target.TextChanged      += Target_TextChanged;
			Target.LostFocus        += Target_LostFocus;
			Target.MouseDoubleClick += Target_MouseDoubleClick;

            //List 
			Children.CollectionChanged += Children_CollectionChanged;

			for(int i = 0;i < Children.Count;i++)
				AddChild(Children[i]);
		}

        /// <summary>
        /// Dobbelt klik åbner/lukket listen med tekst
        /// </summary>
		private void Target_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
			IsOpen = !IsOpen;
		}

        /// <summary>
        /// Denne function kan fortælle om en tekst i
        /// autocomplete listen er synlig eller ej
        /// </summary>
		private bool TextblockIsVisible(TextBlock sender) {
			object[] values = { sender.Text, sender.Tag };
            
            if (values.Length != 2) return false; 
            if (values[0] == null || values[1] == null) return false;

            //samlign ikke med mellemrum
            string str1 = values[0].ToString().ToLower().Replace(" ", "");
            string str2 = values[1].ToString().ToLower().Replace(" ", ""); ;

            return str1.Contains(str2);
        }

        /// <summary>
        /// Luk autocomplete listen når 
        /// textboxen mister focus
        /// </summary>
		private void Target_LostFocus(object sender,RoutedEventArgs e) {
			IsOpen = false;
			SetHighlight(-1);
		}

        /// <summary>
        /// Når man skriver en textboxen skal den 
        /// kun vise listen hvis der er noget at vise
        /// </summary>
        private void Target_TextChanged(object sender,TextChangedEventArgs e) {
            
			int visibleChildCnt = childContainer.Children.Cast<TextBlock>().Count(x => x.Visibility == Visibility.Visible);
            IsOpen = visibleChildCnt > 0;

            SetHighlight(-1);			
		}

        /// <summary>
        /// Gør at vi kan styre listen med keys 
        /// når textboxen er i focus
        /// 
        /// enter og tab vælger den man er ved
        /// piletaster op og ned gør at man kan gå op og ned i forslag
        /// esc lukker listen
        /// </summary>
        private void Target_KeyDown(object sender,KeyEventArgs e) {
			
			if(e.Key == Key.Enter || e.Key == Key.Tab) {

				if(LastSelected == null)
					SetHighlight(0);
				
				if(LastSelected != null) {
					
					TextHasChange = (sender as TextBox).Text != LastSelected.Text;;

					(sender as TextBox).Text = LastSelected.Text;
					(sender as TextBox).CaretIndex = (sender as TextBox).Text.Length;
				}


				//Gå til næste element "TAB"
				if(e.Key == Key.Tab) {
					e.Handled = true;
					int visibleChildCnt = childContainer.Children.Cast<TextBlock>().Where(x => x.IsVisible).ToArray().Length;

					// Gå vindre
					if(!TextHasChange) {
						TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
						request.Wrapped = true;
						(sender as TextBox).MoveFocus(request);
					}
				}
			} 
			
			else if(e.Key == Key.Up) {
				if(LastIndex == -1) {
						
					int visibleChildCnt = childContainer.Children.Cast<TextBlock>().Where(x => x.IsVisible).ToArray().Length;
					LastIndex = visibleChildCnt;
				}
				SetHighlight(--LastIndex);
			} 
			
			else if(e.Key == Key.Down) {				
				SetHighlight(++LastIndex);
			}

			
			else if(e.Key == Key.Escape) {
					IsOpen = false;
			}
		}

        /// <summary>
        /// Opret et textblock element der skal ind 
        /// i vores autocomplete liste
        /// </summary>
		private void AddChild(string text) {
			TextBlock childTB = new TextBlock();//AutocompleteTextBlock
            new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
			//childTB.Style = (Style)FindResource("AutocompleteTextBlock");
			childTB.Text = text;
			childTB.MouseLeftButtonDown +=ChildTB_MouseLeftButtonDown;


            childTB.SetBinding(TextBlock.VisibilityProperty, new MultiBinding()
            {
                Bindings = {
                    new Binding()
                    {
                        Mode = BindingMode.OneWay,
                        Path = new PropertyPath("Text"),
                        Source = childTB
                    },
                    new Binding()
                    {
                        Mode = BindingMode.OneWay,
                        Path = new PropertyPath("Text"),
                        Source = Target
                    }
                },
                Converter = VisibilityConvert
            });


   //         childTB.SetBinding(TextBlock.TagProperty, new Binding() {
			//	Mode = BindingMode.OneWay,
			//	Path = new PropertyPath("Text"),
			//	Source = Target
			//});

			childTB.MouseLeftButtonUp +=ChildTB_MouseLeftButtonUp;

			childContainer.Children.Add(childTB);
		}

        /// <summary>
        /// Når musen er trykket nede på et item i autocomplete
        /// vil textboxen få denne tekst
        /// </summary>
		private void ChildTB_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			Target.Text = (sender as TextBlock).Text;
		}

        /// <summary>
        /// Når musen efter være trykket givet slip
        /// vil vi sæt highlight på den vi har trykket på
        /// </summary>
		private void ChildTB_MouseLeftButtonUp(object sender,MouseButtonEventArgs e) {
			
			var visibleChilds = childContainer.Children.Cast<TextBlock>().Where(x => x.IsVisible).ToList();
			int index = visibleChilds.IndexOf(sender as TextBlock);
			SetHighlight(index);
		}

        /// <summary>
        /// Når der blivet tilføjet et nyt item til this.Children
        /// Gør denne function at vi vil kunne se det uden at loop
        /// igennem dem vi allerede kan se
        /// </summary>
		private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (string currentElement in e.NewItems) {
						AddChild(currentElement);
					}
					break;
   
				case NotifyCollectionChangedAction.Remove:        
					//foreach (NavigationChild currentElement in e.OldItems)
					//	childContainer.Children.Remove(currentElement);            
					break;

				case NotifyCollectionChangedAction.Move:	break;    
				case NotifyCollectionChangedAction.Replace:	break;
				case NotifyCollectionChangedAction.Reset: childContainer.Children.Clear();	break;
				default:  break;        
			}
		}
		
        /// <summary>
        /// Gør at vi kan se forskel på den vi har valgt
        /// </summary>
		private void SetHighlight(int index) {

			var visibleChilds = childContainer.Children.Cast<TextBlock>().Where(x => x.IsVisible).ToArray();

			if(LastSelected != null) {
				//LastSelected.Style = (Style)FindResource("AutocompleteTextBlock");
				LastSelected = null;
				LastIndex = -1;
			}

			//Må ike komme ud af scope
			if(index >= 0 && index < visibleChilds.Length) {
				//visibleChilds[index].Style = (Style)FindResource("AutocompleteTextBlockSelected");
				LastSelected = visibleChilds[index];
				LastSelected.BringIntoView();
				LastIndex = index;
			}
		}
	}
}
