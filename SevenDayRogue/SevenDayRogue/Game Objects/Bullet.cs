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
    public class Bullet
    {
        public Level level;

        public Vector2 Position;
        public Vector2 Direction;

        public float speed;

        public Texture2D texture;
        public Vector2 origin;


        public bool isPlayers;

        public int damage;

        public BulletType type;

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X - 4, (int)Position.Y - 4, 8, 8);
            }
        }

        public Bullet(Level level, Vector2 position, Vector2 direction, int dmg, int speed, BulletType type, bool isPlayers)
        {
            this.level = level;
            this.Position = position;
            this.Direction = direction;
            this.damage = dmg;
            this.speed = speed;

            this.isPlayers = isPlayers;
            this.type = type;


            LoadContent();
        }

        private void LoadContent()
        {
            texture = level.game.WhitePixel;

            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Update(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += new Vector2(Direction.X * speed * elapsed, Direction.Y * speed * elapsed);

            //check if we've gone off the screen

            Vector2 tilePos =  TileHelper.GetTilePosition(Position);
            if (level.GetCollision((int)tilePos.X, (int)tilePos.Y))
            {
                level.DespawnBullet(this);
            }
          
        }

        
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawPrimitives.DrawRectangle(BoundingRectangle, level.game.WhitePixel, Color.Green, spriteBatch, true, 1);
            //spriteBatch.Draw(texture, Position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
          
        }

    }
}
