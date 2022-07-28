using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics; 
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.UI;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace lasso_tool
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool isPressed = false;
        Point a = new Point(-1,-1);
        List<Point> points = new List<Point>();
        WriteableBitmap imageWriteableBitmap;
        Double minX = Double.MaxValue,minY = Double.MaxValue, maxX = Double.MinValue, maxY= Double.MinValue;

        public MainPage()
        {
            this.InitializeComponent();
            //WriteableBitmap writeableBmp = BitmapFactory.New(512, 512);
            this.Loaded += MainPage_Loaded;
            
           
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //imageWriteableBitmap = new WriteableBitmap((int)lassoImg.ActualWidth, (int)lassoImg.ActualHeight);
            imageWriteableBitmap =  await BitmapFactory.FromContent(new Uri(this.BaseUri, "/Assets/x.png"));// BitmapFactory.New((int)lassoImg.ActualWidth, (int)lassoImg.ActualHeight);//.FromByteArray(ImageUriToBytes(@"C:\Users\amrow\OneDrive\الصور\Cyberpunk 2077\photomode_01082021_192634.png").Result);
            //imageWriteableBitmap.Clear(Colors.Beige);
            //imageWriteableBitmap.FillRectangle(150, 150, 250, 250, Colors.White);
            lassoImg.Source = imageWriteableBitmap;
            //writeableBmp.Blit()
            //lassoImg.Source = imageWriteableBitmap;
        }

        private void Image_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("Inside");
            isPressed = true;
           
        }

        private void Image_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Image).Position;
            

            if (isPressed)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);

                if (a.X!=-1)
                {
                    imageWriteableBitmap.DrawLine((int)a.X, (int)a.Y, (int)point.X, (int)point.Y, Colors.Black);
                    Debug.WriteLine("Moving " + point.X + " " + point.Y);
                }
                a = point;
            }
        }

        private async void Image_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isPressed = false;
            a.X = -1;
            //imageWriteableBitmap.FillRectangle(150, 150, 250, 250, Colors.White);
            BitmapImage image = new BitmapImage();

            WriteableBitmap c = BitmapFactory.New((int)lassoImg.ActualWidth, (int)lassoImg.ActualHeight);// await BitmapFactory.FromContent(new Uri(this.BaseUri,"/Assets/x.png"));
            c.Clear(Colors.White);
            c.Blit(new Rect(new Point(minX, minY), new Point(maxX,maxY)), imageWriteableBitmap, new Rect(new Point(minX, minY), new Point(maxX, maxY)));
            lassoImg.Source = c;
            //c.Clear(Colors.AliceBlue);
            //c.Blit(new Rect(new Point(300,300),new Size(500,500)), imageWriteableBitmap, new Rect(100, 100, 300, 300));
            //lassoImg.Source = c;
            //imageWriteableBitmap.
            minX = Double.MaxValue; minY = Double.MaxValue; maxX = Double.MinValue; maxY = Double.MinValue;

        }


        
    }
}
