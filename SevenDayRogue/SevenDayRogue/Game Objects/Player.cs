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
    public class Player
    {
        public Level level;

        public Vector2 Position;
        public Vector2 Velocity;

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X - 13, (int)Position.Y - 13, 25, 25);
            }
        }

        public Player(Level level, Vector2 position)
        {
            this.level = level;
            this.Position = position;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 previousPosition = this.Position;

            float dx = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleInput(gameTime);
            this.Position += this.Velocity * dx;

            Collision.HandleCollisions(level, BoundingRectangle, ref Position);

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                Velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                Velocity.Y = 0;


        }

        private void HandleInput(GameTime gametime)
        {
            Vector2 moveVector = new Vector2(level.game.gameInput.HMovement, level.game.gameInput.VMovement);
            if (moveVector.X != 0 && moveVector.Y != 0)
                moveVector.Normalize();
            this.Velocity = moveVector * GameConstants.playerSpeed;


            
        }

        private Vector2 CheckCollision(Vector2 newPosition)
        {

            Vector2 retvalPosition = this.Position;
            /*
            if (newPosition.X < 0)
                newPosition.X = 0;
            if (newPosition.Y < 0)
                newPosition.Y = 0;
            if (newPosition.X + 50 > level.Width * GameConstants.TileWidth)
                newPosition.X = level.Width * GameConstants.TileWidth - 50;
            if (newPosition.Y + 50 > level.Height * GameConstants.TileHeight)
                newPosition.Y = level.Height * GameConstants.TileHeight - 50;
            */

            if (newPosition.X > Position.X && !TileHelper.CheckCollision(BoundingRectangle, level.tileArray, Direction.Right))
            {
                this.Position.X = newPosition.X;
            }
            else if (newPosition.X < Position.X && !TileHelper.CheckCollision(BoundingRectangle, level.tileArray, Direction.Left))
            {
                this.Position.X = newPosition.X;
            }
            else if (newPosition.Y < Position.Y && !TileHelper.CheckCollision(BoundingRectangle, level.tileArray, Direction.Up))
            {
                this.Position.Y = newPosition.X;
            }
            else if (newPosition.Y > Position.Y && !TileHelper.CheckCollision(BoundingRectangle, level.tileArray, Direction.Down))
            {
                this.Position.Y = newPosition.Y;
            }

            return Position;
        
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawPrimitives.DrawRectangle(BoundingRectangle, level.game.WhitePixel, Color.Pink, spriteBatch, true, 1);
            DrawPrimitives.DrawRectangle(new Rectangle((int)Position.X,(int)Position.Y,5,5) , level.game.WhitePixel, Color.Red, spriteBatch, true, 1);

            spriteBatch.DrawString(level.game.font, Position.X + "," + Position.Y, new Vector2(BoundingRectangle.Center.X, BoundingRectangle.Center.Y), Color.Black);
        }

    }
}
