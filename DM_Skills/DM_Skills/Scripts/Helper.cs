using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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



        // Convert an object to a byte array
        public static byte[] ObjectToByteArray(object obj)
        {

            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        // Convert a byte array to an Object
        public static object ByteArrayToObject(byte[] arrBytes)
        {

            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Position = 0;
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = (object)binForm.Deserialize(memStream);
                return obj;
            }
        }

        
    }
}