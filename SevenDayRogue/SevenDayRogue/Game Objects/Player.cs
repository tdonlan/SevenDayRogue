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

        public Rectangle CurBoundingRec(Vector2 pos)
        {
            return new Rectangle((int)pos.X - 13, (int)pos.Y - 13, 25, 25);
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
           
            //check X
            Vector2 newPosX = new Vector2(Position.X + Velocity.X * dx, Position.Y);
      
          
            if (!Collision.CheckCollision(level, CurBoundingRec(newPosX)))
            {
                
                Position = newPosX;
            }
          
            //check y
            Vector2 newPosY = new Vector2(Position.X, Position.Y + Velocity.Y * dx);
          
        
            if (!Collision.CheckCollision(level,CurBoundingRec(newPosY)))
            {
              
                Position = newPosY;
            }
           
             
        }

        private void HandleInput(GameTime gametime)
        {
            Vector2 moveVector = new Vector2(level.game.gameInput.HMovement, level.game.gameInput.VMovement);
            if (moveVector.X != 0 && moveVector.Y != 0)
                moveVector.Normalize();
            this.Velocity = moveVector * GameConstants.playerSpeed;


            
        }

     

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawPrimitives.DrawRectangle(BoundingRectangle, level.game.WhitePixel, Color.Pink, spriteBatch, true, 1);
            DrawPrimitives.DrawRectangle(new Rectangle((int)Position.X,(int)Position.Y,5,5) , level.game.WhitePixel, Color.Red, spriteBatch, true, 1);

            spriteBatch.DrawString(level.game.font, Position.X + "," + Position.Y, new Vector2(BoundingRectangle.Center.X, BoundingRectangle.Center.Y), Color.Black);
        }

    }
}
