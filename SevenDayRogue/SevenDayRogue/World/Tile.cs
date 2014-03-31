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
    public class Tile
    {
        public bool isSolid;
        public TileType type;
        public WallTileType wallTileType;

        public Tile(bool isSolid, TileType type)
        {
            this.wallTileType = WallTileType.None;
            this.isSolid = isSolid;
            this.type = type;
        }
    }

    public class TileHelper
    {
        //get the world position (center) of the tile
        public static Vector2 GetWorldPosition(int x, int y)
        {
            //return new Vector2(x * GameConstants.TileWidth + (GameConstants.TileWidth/2), y * GameConstants.TileHeight + (GameConstants.TileHeight/2));
            return new Vector2(x * GameConstants.TileWidth, y * GameConstants.TileHeight );
        }

        //get the tile position that overlaps the world position
        public static Vector2 GetTilePosition(Vector2 position)
        {
            int x = (int)Math.Floor(position.X / GameConstants.TileWidth);
            int y = (int)Math.Floor(position.Y / GameConstants.TileHeight);
            return new Vector2(x, y);
        }

        public static Rectangle GetTileRectangle(int x, int y)
        {
            return new Rectangle(x * GameConstants.TileWidth, y * GameConstants.TileHeight, GameConstants.TileWidth, GameConstants.TileHeight);
        }

        public static Rectangle getRecForWallType(WallTileType type)
        {
            switch (type)
            {
                case WallTileType.None:
                    return GetTileRectangle(6, 0);
                case WallTileType.Top:
                    return GetTileRectangle(3, 0);
                case WallTileType.TopRight:
                    return GetTileRectangle(3, 2);
                case WallTileType.InnerTopRight:
                    return GetTileRectangle(3, 0);
                case WallTileType.Right:
                    return GetTileRectangle(1, 1);
                case WallTileType.InnerBottomRight:
                    return GetTileRectangle(3, 0);
                case WallTileType.BottomRight:
                    return GetTileRectangle(3, 3);
                case WallTileType.Bottom:
                    return GetTileRectangle(3, 1);
                case WallTileType.BottomLeft:
                    return GetTileRectangle(2, 3);
                case WallTileType.InnerBottomLeft:
                    return GetTileRectangle(3, 0);
                case WallTileType.Left:
                    return GetTileRectangle(0, 1);
                case WallTileType.InnerTopLeft:
                    return GetTileRectangle(3, 0);
                case WallTileType.TopLeft:
                    return GetTileRectangle(2, 3);
                case WallTileType.Center:
                    return GetTileRectangle(3, 0);

                default:
                    return GetTileRectangle(6, 0);
                  

            }
        }

        public static Rectangle getRecForWallType2(WallTileType type)
        {
            switch (type)
            {
                case WallTileType.None:
                    return GetTileRectangle(2, 2);
                case WallTileType.Top:
                    return GetTileRectangle(2, 0);
                case WallTileType.TopRight:
                    return GetTileRectangle(3, 0);
                case WallTileType.InnerTopRight:
                    return GetTileRectangle(3, 1);
                case WallTileType.Right:
                    return GetTileRectangle(4, 2);
                case WallTileType.InnerBottomRight:
                    return GetTileRectangle(3, 3);
                case WallTileType.BottomRight:
                    return GetTileRectangle(4, 3);
                case WallTileType.Bottom:
                    return GetTileRectangle(2, 4);
                case WallTileType.BottomLeft:
                    return GetTileRectangle(1, 4);
                case WallTileType.InnerBottomLeft:
                    return GetTileRectangle(1, 3);
                case WallTileType.Left:
                    return GetTileRectangle(0, 2);
                case WallTileType.InnerTopLeft:
                    return GetTileRectangle(1, 1);
                case WallTileType.TopLeft:
                    return GetTileRectangle(1, 0);
                case WallTileType.Center:
                    return GetTileRectangle(2, 1);

                default:
                    return GetTileRectangle(2, 2);


            }
        }


        public static WallTileType getWallTileType(Level level, int x, int y)
        {
            bool[] e = getEdgeTileArray(level.tileArray, x, y);

            if (!e[0] && !e[2] && e[6])
                return WallTileType.TopRight;

            else if (!e[0] && !e[6] && e[2])

                return WallTileType.TopLeft;
            else if (!e[0])

                return WallTileType.Top;
            else if (e[0] && !e[2] && !e[4])

                return WallTileType.BottomRight;
            else if (e[0] && !e[6] && !e[4])

                return WallTileType.BottomLeft;
            else if (!e[4])

                return WallTileType.Bottom;
            else if (e[0] && !e[2] && e[4])

                return WallTileType.Right;
            else if (e[0] && !e[6] && e[4])
                return WallTileType.Left;
            else
                return WallTileType.Center;

        }

        //fill out a bool array on the type of the surrounding tiles (impassible vs passible)
        //goes around clockwise from top
        private static bool[] getEdgeTileArray(Tile[,] map, int x, int y)
        {
            bool[] retval = new bool[8] { false, false, false, false, false, false, false, false };

            retval[0] = getTileType(map, x, y - 1); //n
            retval[1] = getTileType(map, x + 1, y - 1); //ne
            retval[2] = getTileType(map, x + 1, y); //e
            retval[3] = getTileType(map, x + 1, y + 1); //se
            retval[4] = getTileType(map, x, y + 1); //s
            retval[5] = getTileType(map, x - 1, y + 1); //sw
            retval[6] = getTileType(map, x - 1, y); //w
            retval[7] = getTileType(map, x - 1, y - 1); //nw

            return retval;
        }

        //get the tile type.
        //if tile is off the map, return true
        private static bool getTileType(Tile[,] map, int x, int y)
        {
            if (x < 0 || x >= map.GetLength(0))
            {
                return true;
            }
            else if (y < 0 || y >= map.GetLength(1))
            {

                return true;
            }
            else
            {
                if (map[x, y].isSolid)
                    return true;
                else
                    return false;
            }

        }

        //used to check if a player is on stairs, etc
        public static bool CheckCollisionWithType(Rectangle boundingRec, Tile[,] tileArray, TileType type)
        {
            bool retval = false;

            Rectangle bounds = boundingRec;
            int leftTile = (int)Math.Floor((float)bounds.Left / GameConstants.TileWidth);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / GameConstants.TileWidth)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / GameConstants.TileHeight);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / GameConstants.TileHeight)) - 1;


            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = rightTile; x >= leftTile; --x)
                {
                    // If this tile is collidable,
                    if (tileArray[x, y].type == type)
                    {
                        return true;
                    }
                }
            }

            return retval;
        }

        //return a list of all floor tiles within the rectangle of the level
        //rectangle is in tile coordinates
        public static List<Point> getFloorList(Level level, Rectangle rec)
        {
            List<Point> pointList = new List<Point>();
            for(int x=rec.X;x<rec.X+rec.Width;x++)
            {
                for (int y = rec.Y; y < rec.Y + rec.Height; y++)
                {
                    if (!level.GetCollision(x, y))
                    {
                        pointList.Add(new Point(x, y));
                    }
                }

            }

            return pointList;

        }


        //DEPRECIATED?  
        public static bool CheckCollision(Rectangle boundingRec, Tile[,] tileArray, Direction dir)
        {
            bool retval = false;

            int leftTile = (int)Math.Floor((float)boundingRec.Left / GameConstants.TileWidth);
            int rightTile = (int)Math.Ceiling(((float)boundingRec.Right / GameConstants.TileWidth)) -1;
            int topTile = (int)Math.Floor((float)boundingRec.Top / GameConstants.TileHeight);
            int bottomTile = (int)Math.Ceiling(((float)boundingRec.Bottom  / GameConstants.TileHeight)) ;

            switch (dir)
            {
                case Direction.Left:
                    for (int i = topTile; i <= bottomTile; i++)
                    {
                        if (tileArray[leftTile, i].isSolid) retval = true;
                    }
                    break;
                case Direction.Right:
                    for (int i = topTile; i <= bottomTile; i++)
                    {
                        if (tileArray[rightTile, i].isSolid) retval = true;
                    }
                    break;
                case Direction.Up:
                    for (int i = leftTile; i <= rightTile; i++)
                    {
                        if (tileArray[i, topTile].isSolid) retval = true;
                    }

                    break;
                case Direction.Down:
                    for (int i = leftTile; i <= rightTile; i++)
                    {
                        if (tileArray[i, bottomTile].isSolid) retval = true;
                    }
                    break;
                default:
                    break;
            }

            return retval;

        }
    }

    //put a level factory here?
}
