using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

// From http://noelberry.ca/2013/06/procedural-generation-the-dungeons
namespace SevenDayRogue
{


    public class Generation 
    {
        // map tile types and their a* pathfinding cost
        // so, for example, if it wanted to move through a Wall, it'd have to be better than moving through 4 Floor tiles
        public enum Type
        {
            Floor = 1,
            Start=2,
            End=3,
            Wall = 4,
            Stone = 20,
            None = 0
        }

        public Random r;

        // map data
        public Type[,] Map;
        public int RoomCount;
        public int MinRoomSize;
        public int MaxRoomSize;
        public int Grid = 16;
        public int Padding = 4;

        // list of active rooms and the room xml templates
        public List<Rectangle> Rooms;
        //public List<XmlElement> RoomTemplates;

        // map size
        public new int Width { get { return Map.GetLength(0); } }
        public new int Height { get { return Map.GetLength(1); } }

        // pathfinding cost thing
        private int[,] cost;
        private List<Point> currentPath;

        private bool hasStart = false;
        private bool hasEnd = false;


        public Generation()
        {
            r = new Random();
          
        }

        #region map generation

        /// <summary>
        /// Generates the room of the given width and height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="callback">Called on generation-complete</param>
        public void Generate(int width, int height)
        {
            // create the empty map and an empty list of rooms
            Map = new Type[width, height];
            Rooms = new List<Rectangle>();

            // fill with Wall tiles
            FillMap(Type.Wall);


            // set the min/max room size and the room count. Play around with these values
            MinRoomSize = 6;
            MaxRoomSize = (int)(Math.Min(width, height) / 4f);
            RoomCount = (int)(Math.Sqrt(width * height) / 2f);

            // Start the coroutine of generating the segments
            GenerateSegments();
        }

        /// <summary>
        /// Generates everything
        /// </summary>
        private void GenerateSegments()
        {
            // dig stuff
            PlaceRooms();
            
            
            PlaceTunnels();

        
        }

        /// <summary>
        /// Fills the entire map with the given type
        /// </summary>
        /// <param name="type"></param>
        public void FillMap(Type type)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                    Map[i, j] = type;
            }
        }

        #endregion

        #region Room generation

        /// <summary>
        /// Places all the rooms we're supposed to place
        /// </summary>
        public void PlaceRooms()
        {
            // place rooms
            int placed = 0;
            int count = 0;
            
            while (placed < RoomCount)
            {
               

                if (PlaceRectRoom())
                {
                    placed++;
                }
                        

                // this is for debug stuff - shouldn't ever happen
                count++;
                if (count > 1000)
                {
                    
                    //Add the end to a room randomly near the end
                    

                    break;
                }
            }

            AddStart(Rooms[0]);
            AddEnd(Rooms[r.Next(Rooms.Count - Rooms.Count / 4, Rooms.Count - 1)]);


        }

        /// <summary>
        /// Digs out a rectangular room
        /// </summary>
        /// <returns></returns>
        public bool PlaceRectRoom()
        {
            int width = r.Next(MinRoomSize, MaxRoomSize);
            int height = r.Next(MinRoomSize, MaxRoomSize);
            Rectangle room = new Rectangle(r.Next(Padding, Width - width - Padding * 2), r.Next(Padding, Height - height - Padding * 2), width, height);

            // check room
            if (Overlaps(room))
            {
                Rooms.Add(room);
                DigRoom(room);

                return true;
            }
            return false;
        }

        private void AddStart(Rectangle room)
        {
            if (!hasStart)
            {
                Set(r.Next(room.X + 1, room.X + room.Width - 1), r.Next(room.Y + 1, room.Y + room.Height - 1), Type.Start);
                hasStart = true;
            }
        }

        private void AddEnd(Rectangle room)
        {
            if (!hasEnd)
            {
                Set(r.Next(room.X + 1, room.X + room.Width - 1), r.Next(room.Y + 1, room.Y + room.Height - 1), Type.End);
                hasEnd = true;
            }
        }

       

        /// <summary>
        /// Digs out the rectangular room
        /// </summary>
        /// <param name="room"></param>
        public void DigRoom(Rectangle room)
        {
            // place floors
            for (int i = 0; i < room.Width; i++)
            {
                for (int j = 0; j < room.Height; j++)
                {
                    Set(room.X + i, room.Y + j, Type.Floor);
                }
            }

            // place stone around the entire thing
            for (int i = 0; i < room.Width; i++)
            {
                Set(room.X + i, room.Y - 1, Type.Stone);
                Set(room.X + i, room.Y + room.Height, Type.Stone);
            }

            for (int i = 0; i < room.Height; i++)
            {
                Set(room.X - 1, room.Y + i, Type.Stone);
                Set(room.X + room.Width, room.Y + i, Type.Stone);
            }

            // make some doors
            int doors = 2;
            for (int i = 0; i < doors; i++)
            {
                if (r.Next(0, 2) == 0)
                {
                    Set(room.X + r.Next(0, room.Width), room.Y + RandomChoose(-1, room.Height), Type.Wall);
                }
                else
                {
                    Set(room.X + RandomChoose(-1, room.Width), room.Y + r.Next(0, room.Height), Type.Wall);
                }
            }
        }

       

        #endregion

        #region generate tunnels

        /// <summary>
        /// Places and digs out the tunnels from room-to-room
        /// </summary>
        public void PlaceTunnels()
        {
            int count = 0;

            // pathfind tunnels
            Rectangle prev = Rooms[Rooms.Count - 1];
            foreach (Rectangle next in Rooms)
            {
                count++;

                // pathfind from the center of the previous room to the center of the next room
                Pathfind(prev.X + prev.Width / 2, prev.Y + prev.Height / 2, next.X + next.Width / 2, next.Y + next.Height / 2);

                // dig out the tunnel we just found
                int size = RandomChoose(1, 2);
                foreach (Point point in currentPath)
                {
                    Set(point.X - size / 2, point.Y - size / 2, size, size, Type.Floor, Type.Stone);
                }

                prev = next;
                Console.WriteLine("Pathfound {0}/{1}", count, Rooms.Count);
            }
        }

        /// <summary>
        /// Finds a path from the first poisition to the 2nd position and stores it in the currentPath variable
        /// NOTE: This is probably super horrible A* Pathfinding algorithm. I'm sure there's WAY better ways of writing this
        /// </summary>
        public void Pathfind(int x, int y, int x2, int y2)
        {
            cost = new int[Width, Height];
            cost[x, y] = (int)Type.Floor;

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
                    cost[point.X - 1, point.Y] = currentCost + (int)Map[point.X - 1, point.Y];
                }
                if (point.X + 1 < Width && cost[point.X + 1, point.Y] == 0)
                {
                    active.Add(new Point(point.X + 1, point.Y));
                    cost[point.X + 1, point.Y] = currentCost + (int)Map[point.X + 1, point.Y];
                }
                if (point.Y - 1 >= 0 && cost[point.X, point.Y - 1] == 0)
                {
                    active.Add(new Point(point.X, point.Y - 1));
                    cost[point.X, point.Y - 1] = currentCost + (int)Map[point.X, point.Y - 1];
                }
                if (point.Y + 1 < Height && cost[point.X, point.Y + 1] == 0)
                {
                    active.Add(new Point(point.X, point.Y + 1));
                    cost[point.X, point.Y + 1] = currentCost + (int)Map[point.X, point.Y + 1];
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
            currentPath = points;
        }

        #endregion

        #region map data - set/check

        /// <summary>
        /// Sets the given cell to the given type, if it's not already set to the untype
        /// </summary>
        public void Set(int x, int y, Type type, Type untype = Type.None)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return;
            if (Map[x, y] != untype)
                Map[x, y] = type;
        }

        /// <summary>
        /// Sets the given rectangle of cells to the type
        /// </summary>
        public void Set(int x, int y, int w, int h, Type type, Type untype = Type.None)
        {
            for (int i = x; i < x + w; i++)
            {
                for (int j = y; j < y + h; j++)
                {
                    Set(i, j, type);
                }
            }
        }

        /// <summary>
        /// Makes sure the area doesn't overlap any floors
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public bool Overlaps(Rectangle area)
        {
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    if (Map[area.X + i, area.Y + j] != Type.Wall && Map[area.X + i, area.Y + j] != Type.Stone)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns false if the position is outside the map
        /// </summary>
        /// <returns></returns>
        public bool CellOutside(int x, int y)
        {
            return x < 0 || y < 0 || x >= Width || y >= Height;
        }

        /// <summary>
        /// Gets the type of the given cell
        /// </summary>
        /// <returns></returns>
        public Type GetCell(int x, int y)
        {
            if (!CellOutside(x, y))
                return Map[x, y];
            else return Type.None;
        }

        #endregion

        /// <summary>
        /// Returns a 2D bool array used for checking collisions in entities
        /// </summary>
        /// <returns></returns>
        public bool[,] GetCollideData()
        {
            bool[,] data = new bool[Width, Height];
            for (int i = 1; i < Width - 1; i++)
            {
                for (int j = 1; j < Height - 1; j++)
                {
                    // this looks a bit weird, but it basically just only places "Edge" walls (it doesn't fill the insides)
                    // so it just checks to make sure each tile is adjacent to a floor tile
                    if (Map[i, j] != Type.Floor)
                    {
                        if (Map[i - 1, j] == Type.Floor || Map[i - 1, j - 1] == Type.Floor || Map[i, j - 1] == Type.Floor || Map[i + 1, j - 1] == Type.Floor
                            || Map[i + 1, j] == Type.Floor || Map[i + 1, j + 1] == Type.Floor || Map[i, j + 1] == Type.Floor || Map[i - 1, j + 1] == Type.Floor)
                        {
                            data[i, j] = true;
                        }
                    }

                }
            }
            return data;
        }

        /// <summary>
        /// Gets a random position in the map that is a Floor (not a wall)
        /// </summary>
        /// <returns></returns>
        public Vector2 GetRandomPosition()
        {
            int room = r.Next(0, Rooms.Count - 1);
            Vector2 point = Vector2.Zero;
            while (point == Vector2.Zero || Map[(int)point.X, (int)point.Y] != Type.Floor)
            {
                point = new Vector2(
                    r.Next(Rooms[room].X, Rooms[room].X + Rooms[room].Width),
                    r.Next(Rooms[room].Y, Rooms[room].Y + Rooms[room].Height)
                );
            }

            return point * Grid;
        }


        private int RandomChoose(int val1, int val2)
        {
            if (r.Next(2) == 0)
            {
                return val1;
            }
            else
            {
                return val2;
            }
        }

        private int GetMin(int val1, int val2, int val3)
        {
            return Math.Min(val1, Math.Min(val2, val3));
        }
    }
}