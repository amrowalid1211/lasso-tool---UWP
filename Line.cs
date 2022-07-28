using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lasso_tool
{
    public class Line
    {
        public Point p1, p2;

        public Line(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public static SortedDictionary<int, List<int>> getPointsFromDrawing(HashSet<Point> drawnPoints)
        {
            SortedDictionary<int, List<int>> points = new SortedDictionary<int, List<int>>();
            for (int i = 0; i < drawnPoints.Count-1; i++)
            {
                Point a = drawnPoints.ElementAt(i);
                Point b = drawnPoints.ElementAt(i+1);
                SortedDictionary<int, List<int>> keyValuePairs = getPoints(a, b);
                foreach(KeyValuePair<int, List<int>> pair in keyValuePairs)
                {
                    if (points.ContainsKey(pair.Key))
                    {
                        points[pair.Key].AddRange(pair.Value);
                        points[pair.Key].Sort();
                    }else
                    {
                        points.Add(pair.Key, pair.Value);
                    }
                }
            }
            return points;
        }


        public static SortedDictionary<int, List<int>> getPoints(Point p1,Point p2)
        {
            //var points = new Point[Math.Max(p2.X-p1.X,p2.Y-p1.X) + 10];
            int quantity = (int) Math.Max(p2.X - p1.X, p2.Y - p1.X) + 10;
            SortedDictionary<int,List<int>> points = new SortedDictionary<int, List<int>>();
            
            int ydiff = p2.Y - p1.Y, xdiff = p2.X - p1.X;
            double slope = (double)(p2.Y - p1.Y) / (p2.X - p1.X);
            double x, y;

            for (double i = 0; i < quantity; i++)
            {
                y = slope == 0 ? 0 : ydiff * (i / quantity);
                x = slope == 0 ? xdiff * (i / quantity) : y / slope;
                addPointOnLineDictionary(new Point((int)Math.Round(x) + p1.X, (int)Math.Round(y) + p1.Y),points);
                
            }
            addPointOnLineDictionary(new Point(p2.X,p2.Y), points);
            return points;
        }

        private static void addPointOnLineDictionary(Point point, SortedDictionary<int, List<int>> points)
        {
            if (points.ContainsKey(point.X))
            {
                points[point.X].Add(point.Y);
                points[point.X].Sort();
            } else points.Add(point.X, new List<int> { point.Y });
        }
    }
}
