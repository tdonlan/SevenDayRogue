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
    public class Collision
    {

        //check collision - returns true if the bounding box overlaps an impassable tile
        //used primarily for grapple hook or other things that move the player position directly (instead of by clamped vector)
        public static bool CheckCollision(Level level, Rectangle BoundingRectangle)
        {
            bool retval = false;

            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / GameConstants.TileWidth);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / GameConstants.TileWidth)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / GameConstants.TileHeight);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / GameConstants.TileHeight)) - 1;


            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = rightTile; x >= leftTile; --x)
                {
                    // If this tile is collidable,
                    if (level.tileArray[x, y].isSolid)
                    {
                        return true;
                    }
                }
            }

            return retval;
        }


        //Need to make sure the calculated bounds are within the array bounds
        public static void HandleCollisions(Level level, Rectangle BoundingRectangle, ref Vector2 Position)
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / GameConstants.TileWidth);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / GameConstants.TileWidth)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / GameConstants.TileHeight);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / GameConstants.TileHeight)) - 1;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = rightTile; x >= leftTile; --x)
                {
                    // If this tile is collidable,
                    if (level.tileArray[x, y].isSolid)
                    {
                        Rectangle tileBounds = TileHelper.GetTileRectangle(x, y);
                        // Determine collision depth (with direction) and magnitude.
                        bounds = HandleCollision(BoundingRectangle, bounds, true, tileBounds, ref Position);
                    }
                }
            }


        }

        private static Rectangle HandleCollision(Rectangle BoundingRectangle, Rectangle bounds, bool isSolid, Rectangle tileBounds, ref Vector2 Position)
        {
            Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
            if (depth != Vector2.Zero)
            {
                float absDepthX = Math.Abs(depth.X);
                float absDepthY = Math.Abs(depth.Y);

                // Resolve the collision along the shallow axis.  
                if (absDepthY < absDepthX)
                {
                    // Ignore platforms, unless we are on the ground.  
                    if (isSolid)
                    {
                        // Resolve the collision along the Y axis.  
                        Position = new Vector2(Position.X, Position.Y + depth.Y);

                        // Perform further collisions with the new bounds.  Need to include change in position
                        
                        bounds = new Rectangle(BoundingRectangle.X, (int)Math.Round(BoundingRectangle.Y + depth.Y), BoundingRectangle.Width, BoundingRectangle.Height);
                    }
                }
                else if (isSolid) // Ignore platforms.  
                {
                    // Resolve the collision along the X axis.  
                    Position = new Vector2(Position.X + depth.X, Position.Y);

                    // Perform further collisions with the new bounds.  
                    bounds = new Rectangle((int)Math.Round(BoundingRectangle.X + depth.X), BoundingRectangle.Y, BoundingRectangle.Width, BoundingRectangle.Height);
                    
                }
            }
            return bounds;
        }

        public static bool getLOS(Level level, Vector2 vec1, Vector2 vec2, ref Vector2 vecLOS)
        {
            bool retval = true;
            //check if both vecs are on screen?

            //get resultant vector
            vecLOS = vec2 - vec1;
            //reduce by greatest common denom
            vecLOS.Normalize();
            Vector2 pos = vec1;

            //get a list of tile coords.  traverse the list and check for collision

            while (incrementVector(ref pos, vecLOS, vec2, GameConstants.TileHeight / 2) == true)
            {
                int leftTile = (int)Math.Floor(pos.X / GameConstants.TileWidth);
                int rightTile = (int)Math.Ceiling((pos.X / GameConstants.TileWidth)) - 1;
                int topTile = (int)Math.Floor(pos.Y / GameConstants.TileHeight);
                int bottomTile = (int)Math.Ceiling((pos.Y / GameConstants.TileHeight)) - 1;

                for (int y = topTile; y <= bottomTile; ++y)
                {
                    for (int x = leftTile; x <= rightTile; ++x)
                    {
                        // If this tile is collidable,

                        if(level.GetCollision(x, y))
                        {
                            retval = false;
                        }
                    }
                }

            }

            return retval;

        }

        private static bool incrementVector(ref Vector2 position, Vector2 dir, Vector2 dest, int amount)
        {
            //quick and dirty vector (slope) reduction
            //dir = Helper.reduceVector2(dir);

            
            float startDist = DrawPrimitives.getDistance(position, dest);

            position.X += (dir.X * amount);
            position.Y += (dir.Y * amount);


            float endDist = DrawPrimitives.getDistance(position, dest);
            if (endDist > startDist) //we've passed our destination
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }

}