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

        public Tile(bool isSolid, TileType type)
        {
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
