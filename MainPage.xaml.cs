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
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Graphics.Canvas;
using Windows.Graphics.Imaging;
using Windows.Graphics.DirectX;

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
        SoftwareBitmap softwareBitmap;
        int minX = int.MaxValue,minY = int.MaxValue, maxX = int.MinValue, maxY= int.MinValue;
        HashSet<System.Drawing.Point> visited = new HashSet<System.Drawing.Point>();
        public MainPage()
        {
            this.InitializeComponent();
            //WriteableBitmap writeableBmp = BitmapFactory.New(512, 512);
            this.Loaded += MainPage_Loaded;
            
           
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            imageWriteableBitmap =  await BitmapFactory.FromContent(new Uri(this.BaseUri, "/Assets/x.png"));// BitmapFactory.New((int)lassoImg.ActualWidth, (int)lassoImg.ActualHeight);//.FromByteArray(ImageUriToBytes(@"C:\Users\amrow\OneDrive\الصور\Cyberpunk 2077\photomode_01082021_192634.png").Result);
            lassoImg.Source = imageWriteableBitmap;
           
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
                minX = (int)Math.Min(minX, point.X);
                minY = (int)Math.Min(minY, point.Y);
                maxX = (int)Math.Max(maxX, point.X);
                maxY = (int)Math.Max(maxY, point.Y);
                visited.Add(new System.Drawing.Point((int)point.X,(int)point.Y));
                if (a.X!=-1)
                {
                    imageWriteableBitmap.DrawLine((int)a.X, (int)a.Y, (int)point.X, (int)point.Y, Colors.White);
                    Debug.WriteLine("Moving " + point.X + " " + point.Y);
                    lassoImg.Source = imageWriteableBitmap;
                }
                a = point;
            }
        }

        private void Image_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isPressed = false;
            a.X = -1;
            
            BitmapImage image = new BitmapImage();

            WriteableBitmap c = BitmapFactory.New(maxX - minX, maxY - minY); //await BitmapFactory.FromContent(new Uri(this.BaseUri, "/Assets/x.png"));  //BitmapFactory.New((int)lassoImg.ActualWidth, (int)lassoImg.ActualHeight);
            
            c.Blit(new Rect(new Point(0, 0), new Point(maxX - minX, maxY - minY)), imageWriteableBitmap, new Rect(new Point(minX, minY), new Point(maxX, maxY)));
            
            clearLassoOnDrawnPoints(visited, c);
          
            minX = int.MaxValue; minY = int.MaxValue; maxX = int.MinValue; maxY = int.MinValue;
            visited.Clear();
        }

        private async void clearLassoOnDrawnPoints(HashSet<System.Drawing.Point> visited, WriteableBitmap c)
        {
            List<int> selectedPoints = new List<int>();
            int pointC = 0;
            for (int i = 0; i < visited.Count; i++)
            {
                selectedPoints.Add(visited.ElementAt(i).X-minX);
                selectedPoints.Add(visited.ElementAt(i).Y-minY);
              
            }
            
             selectedPoints.Add(selectedPoints.ElementAt(0));
             selectedPoints.Add(selectedPoints.ElementAt(1));
            c.Clear(Colors.White);

            c.FillPolygon(selectedPoints.ToArray(), Colors.Transparent);
           // WriteableBitmap inverted = c.Invert();
            WriteableBitmap masked = BitmapFactory.New(c.PixelWidth, c.PixelHeight);
            masked.Blit(new Rect(new Point(0, 0), new Point(maxX - minX, maxY - minY)), imageWriteableBitmap, new Rect(new Point(minX, minY), new Point(maxX, maxY)), WriteableBitmapExtensions.BlendMode.Alpha);

            masked.Blit(new Rect(new Point(0, 0), new Point(maxX - minX, maxY - minY)), c, new Rect(new Point(0, 0), new Point(maxX - minX, maxY - minY)),WriteableBitmapExtensions.BlendMode.Alpha);
            masked.ForEach((x, y, color) =>
            {
                if (color == Colors.White)
                    return Colors.Transparent;
                return color;
            });
            

            var stream = new InMemoryRandomAccessStream();

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            Stream pixelStream = masked.PixelBuffer.AsStream();
            byte[] pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

            //Software Bitmap
            softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, masked.PixelWidth,masked.PixelHeight);
            encoder.SetSoftwareBitmap(softwareBitmap);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)masked.PixelWidth, (uint)masked.PixelHeight, 96.0, 96.0, pixels);

            await encoder.FlushAsync();
            var streamRef = RandomAccessStreamReference.CreateFromStream(stream);
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;

            dataPackage.SetBitmap(streamRef);

            Clipboard.SetContent(dataPackage);
            lassoImg.Source = masked;


            /*
            Byte[] pixelData = masked.PixelBuffer.ToArray();
            using (Stream stream = masked.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(pixelData, 0, pixelData.Length);
                
            }
            var streamInMemory = new InMemoryRandomAccessStream();
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

            var str = RandomAccessStreamReference.CreateFromStream(masked.PixelBuffer.AsStream().AsRandomAccessStream());
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;

            dataPackage.SetBitmap(str);

            Clipboard.SetContent(dataPackage);
            */
        }


    }
}
