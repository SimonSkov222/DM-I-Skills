using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DM_Skills.Scripts
{
    class Helper
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {

            T parent = VisualTreeHelper.GetParent(child) as T;

            if (parent != null)
                return parent;
            else
                return FindParent<T>(parent);
        }

        public static Visual FindAncestor(Visual child, Type typeAncestor)

        {

            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null && !typeAncestor.IsInstanceOfType(parent))

            {

                parent = VisualTreeHelper.GetParent(parent);

            }

            return (parent as Visual);
        }
    }
}
