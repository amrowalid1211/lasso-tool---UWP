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

        private async void Image_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isPressed = false;
            a.X = -1;
            //imageWriteableBitmap.FillRectangle(150, 150, 250, 250, Colors.White);
            BitmapImage image = new BitmapImage();

            WriteableBitmap c = await BitmapFactory.FromContent(new Uri(this.BaseUri, "/Assets/x.png"));  //BitmapFactory.New((int)lassoImg.ActualWidth, (int)lassoImg.ActualHeight);// await BitmapFactory.FromContent(new Uri(this.BaseUri,"/Assets/x.png"));
            //c.Clear(Colors.White);
            //c.Blit(new Rect(new Point(minX, minY), new Point(maxX,maxY)), imageWriteableBitmap, new Rect(new Point(minX, minY), new Point(maxX, maxY)));
            //clearLasso(visited, c);
            clearLassoOnDrawnPoints(visited, c);
            lassoImg.Source = c;
            //c.Clear(Colors.AliceBlue);
            //c.Blit(new Rect(new Point(300,300),new Size(500,500)), imageWriteableBitmap, new Rect(100, 100, 300, 300));
            //lassoImg.Source = c;
            //imageWriteableBitmap.
            minX = int.MaxValue; minY = int.MaxValue; maxX = int.MinValue; maxY = int.MinValue;
            visited.Clear();
        }

        private async void clearLassoOnDrawnPoints(HashSet<System.Drawing.Point> visited, WriteableBitmap c)
        {
            List<int> selectedPoints = new List<int>();
            int pointC = 0;
            for (int i = 0; i < visited.Count; i++)
            {
                selectedPoints.Add(visited.ElementAt(i).X);
                selectedPoints.Add(visited.ElementAt(i).Y);
              
            }

             selectedPoints.Add(selectedPoints.ElementAt(0));
             selectedPoints.Add(selectedPoints.ElementAt(1));

            c.FillPolygon(selectedPoints.ToArray(), Colors.Transparent);
            //for(int i = minX; i < maxX; i++)
            //{
            //    for(int j = minY; j < maxY; j++)
            //    {

            //        c.SetPixel(i,j,c.GetPixel(i,j).A == 0 ? imageWriteableBitmap.GetPixel(i,j) : Colors.Red);
            //    }
            //}
            //imageWriteableBitmap.
            //DataPackage dp = new DataPackage();
            //var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
            //await stream.WriteAsync(bytes.AsBuffer());
            //stream.Seek(0);
            //await image.SetSourceAsync(stream);
            //dp.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));

            //Clipboard.SetContent(new DataPackage());
        }

        private void clearLasso(HashSet<System.Drawing.Point> visited, WriteableBitmap imageWriteableBitmap)
        {
            SortedDictionary<int, List<int>> map = Line.getPointsFromDrawing(visited);
            int[] points = new int[map.Count*2+6];

            int pointC = 0;
            for (int i = 0; i < map.Count; i++)
            {
                Point a = new Point(map.Keys.ElementAt(i), map[map.Keys.ElementAt(i)][0]);
                //Point b = new Point(map.Keys.ElementAt(i), map[map.Keys.ElementAt(i)][map[map.Keys.ElementAt(i)].Count - 1]);
               
                points[pointC++] = (int)a.X;
                points[pointC++] = (int)a.Y;
                


            }
            points[pointC++] = maxX;
            points[pointC++] = minY;
            points[pointC++] = minX;
            points[pointC++] = minY; 
            points[pointC++] = points[0];
            points[pointC++] = points[1];
            imageWriteableBitmap.FillPolygon(points,Colors.Blue);

            ////////////////////////////////////////////////////////
            ///
            points = new int[map.Count * 2 + 6];
            List<int> selectedPoints = new List<int>();
            pointC = 0;
            for (int i = 0; i < map.Count; i++)
            {
                Point aX = new Point(map.Keys.ElementAt(i), map[map.Keys.ElementAt(i)][0]);
                Point a = new Point(map.Keys.ElementAt(i), map[map.Keys.ElementAt(i)][map[map.Keys.ElementAt(i)].Count - 1]);
                int x = (int)a.X;
                int y = (int)a.Y;
                if (aX.Y!=a.Y)
                {
                    selectedPoints.Add(x);
                    selectedPoints.Add(y);
                }
               


            }
            selectedPoints.Add(maxX);
            selectedPoints.Add(maxY);
            selectedPoints.Add(minX);
            selectedPoints.Add(maxY);
            selectedPoints.Add(selectedPoints.ElementAt(0));
            selectedPoints.Add(selectedPoints.ElementAt(1));
          
            imageWriteableBitmap.FillPolygon(selectedPoints.ToArray(), Colors.Red);
        }
    }
}
