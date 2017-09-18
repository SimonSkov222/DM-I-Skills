using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DM_Skills.Scripts
{
    class Helper
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {

            DependencyObject parent = VisualTreeHelper.GetParent(child);

            if (parent != null)
                return parent as T;
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

        public static string GetLocalIPv4(NetworkInterfaceType _type = NetworkInterfaceType.Ethernet)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties adapterProperties = item.GetIPProperties();

                    if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                    {
                        foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                output = ip.Address.ToString();
                            }
                        }
                    }
                }
            }

            return output;
        }

        public static void CreateBitmapFromVisual(Visual target, string fileName)
        {
            if (target == null || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((Int32)bounds.Width, (Int32)bounds.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush visualBrush = new VisualBrush(target);
                context.DrawRectangle(visualBrush, null, new Rect(new Point(), bounds.Size));
            }

            renderTarget.Render(visual);
            PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTarget));
            using (System.IO.Stream stm = System.IO.File.Create(fileName))
            {
                bitmapEncoder.Save(stm);
            }
        }

        public static void BackupBitmap(Visual target)
        {
            string folder = Directory.GetCurrentDirectory() + @"\Backup\";


            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string filename = $"{DateTime.Now.ToShortDateString()} " +
                $"{DateTime.Now.ToLongTimeString().Replace(":", ".")} ";
            filename = filename.Replace("/", "-");
            string number = "";

            int i = 1;

            while (File.Exists(folder + filename + number + ".jpg"))
            {
                number = $"({i})";
                i++;
            }

            string fullFilename = folder + filename + number + ".jpg";

            CreateBitmapFromVisual(target, fullFilename);


        }


        public static void BackupPDF(StackPanel target)
        {
            string folder = Directory.GetCurrentDirectory() + @"\Backup\";


            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string filename = $"{DateTime.Now.ToShortDateString()} " +
                $"{DateTime.Now.ToLongTimeString().Replace(":", ".")} ";
            filename = filename.Replace("/", "-");
            string number = "";

            int i = 1;

            while (File.Exists(folder + filename + number + ".pdf"))
            {
                number = $"({i})";
                i++;
            }

            string fullFilename = folder + filename + number + ".pdf";
            ObservableCollection<Models.TableModelN> models = new ObservableCollection<Models.TableModelN>();
            foreach (var item in target.Children)
            {
                if (item is Controls.TablesControl)
                {
                    if ((item as Controls.TablesControl).Model.HasData)
                    {
                        models.Add((item as Controls.TablesControl).Model);
                    }
                }
            }

            if (models.Count > 0)
            {
                var print = new Scripts.Print();
                print.CreatePDF(fullFilename, models);
            }

            //CreateBitmapFromVisual(target, fullFilename);


        }

        public static void ProcessUITasks()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (object parameter) {
                frame.Continue = false;
                return null;
            }), null);
            Dispatcher.PushFrame(frame);
        }
    }
}