using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SevenDayRogue
{
    public class PathFinder
    {

        public static List<Point> Pathfind(Level level, int x, int y, int x2, int y2)
        {

            int Width = level.Width;
            int Height = level.Height;
            int[,] cost = new int[Width, Height];
          
            cost[x, y] = 1; //floor type

            List<Point> active = new List<Point>();
            active.Add(new Point(x, y));
            // pathfind
            while (true)
            {
                // get lowest cost point in active list
                Point point = active[0];
                for (int i = 1; i < active.Count; i++)
                {
                    Point p = active[i];
                    if (cost[p.X, p.Y] < cost[point.X, point.Y])
                        point = p;
                }

                // if end point
                if (point.X == x2 && point.Y == y2)
                    break;

                // move in directions
                int currentCost = cost[point.X, point.Y];
                if (point.X - 1 >= 0 && cost[point.X - 1, point.Y] == 0)
                {
                    active.Add(new Point(point.X - 1, point.Y));

                    cost[point.X - 1, point.Y] = currentCost + getCost(level.tileArray[point.X - 1, point.Y]);
                }
                if (point.X + 1 < Width && cost[point.X + 1, point.Y] == 0)
                {
                    active.Add(new Point(point.X + 1, point.Y));
                    cost[point.X + 1, point.Y] = currentCost + getCost(level.tileArray[point.X + 1, point.Y]);
                }
                if (point.Y - 1 >= 0 && cost[point.X, point.Y - 1] == 0)
                {
                    active.Add(new Point(point.X, point.Y - 1));
                    cost[point.X, point.Y - 1] = currentCost + getCost(level.tileArray[point.X, point.Y - 1]);
                }
                if (point.Y + 1 < Height && cost[point.X, point.Y + 1] == 0)
                {
                    active.Add(new Point(point.X, point.Y + 1));
                    cost[point.X, point.Y + 1] = currentCost + getCost(level.tileArray[point.X, point.Y + 1]);
                }

                active.Remove(point);
            }

            // work backwards and find path
            List<Point> points = new List<Point>();
            Point current = new Point(x2, y2);
            points.Add(current);

            while (current.X != x || current.Y != y)
            {
                int highest = cost[current.X, current.Y];
                int left = highest, right = highest, up = highest, down = highest;

                // get cost of each direction
                if (current.X - 1 >= 0 && cost[current.X - 1, current.Y] != 0)
                {
                    left = cost[current.X - 1, current.Y];
                }
                if (current.X + 1 < Width && cost[current.X + 1, current.Y] != 0)
                {
                    right = cost[current.X + 1, current.Y];
                }
                if (current.Y - 1 >= 0 && cost[current.X, current.Y - 1] != 0)
                {
                    up = cost[current.X, current.Y - 1];
                }
                if (current.Y + 1 < Height && cost[current.X, current.Y + 1] != 0)
                {
                    down = cost[current.X, current.Y + 1];
                }

                // move in the lowest direction
                if (left <= GetMin(up, down, right))
                {
                    points.Add(current = new Point(current.X - 1, current.Y));
                }
                else if (right <= GetMin(left, down, up))
                {
                    points.Add(current = new Point(current.X + 1, current.Y));
                }
                else if (up <= GetMin(left, right, down))
                {
                    points.Add(current = new Point(current.X, current.Y - 1));
                }
                else
                {
                    points.Add(current = new Point(current.X, current.Y + 1));
                }
            }

            points.Reverse();

            return points;
        }

        private static int getCost(Tile t)
        {
            if (t.isSolid)
            {
                return 99;
            }
            else
            {
                return 1;
            }
        }

      
        private static int GetMin(int val1, int val2, int val3)
        {
            return Math.Min(val1, Math.Min(val2, val3));
        }
    }
}
