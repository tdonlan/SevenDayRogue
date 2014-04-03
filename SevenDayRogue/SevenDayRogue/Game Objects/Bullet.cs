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

using Krypton;
using Krypton.Lights;

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

        public Light2D light;

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


            if (isPlayers)
            {
                LoadLight();
            }


            LoadContent();
        }

        private void LoadContent()
        {
            texture = level.game.WhitePixel;

            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        private void LoadLight()
        {
            light = new Light2D()
            {
                Texture = level.game.mLightTexture,
                Range = 100,
                Color = Color.White,

                Intensity = .5f,
                Angle = MathHelper.TwoPi,
                X = Position.X,
                Y = Position.Y,
                Fov = MathHelper.TwoPi,

            };

            level.game.krypton.Lights.Add(light);
        }

        public void Update(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += new Vector2(Direction.X * speed * elapsed, Direction.Y * speed * elapsed);
            

            //check if we shoot enemy
            if (isPlayers)
            {
                light.Position = Position;

                for (int i = level.enemyList.Count - 1; i >= 0; i--)
                {
                    if (level.enemyList[i].BoundingRectangle.Intersects(BoundingRectangle))
                    {

                        level.enemyList[i].Hit(type, damage, false);
                        level.DespawnBullet(this);
                    }
                }
            }
            else
            {
                if (level.player.BoundingRectangle.Intersects(BoundingRectangle))
                {
                    level.player.Hit(damage);
                    level.DespawnBullet(this);
                }
            }

            //check if we've gone off the screen

            Vector2 tilePos =  TileHelper.GetTilePosition(Position);
            if (level.GetCollision((int)tilePos.X, (int)tilePos.Y))
            {
                level.DespawnBullet(this);
            }
          
        }

        
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (isPlayers)
            {
                DrawPrimitives.DrawRectangle(BoundingRectangle, level.game.WhitePixel, Color.Green, spriteBatch, true, 1);
                //spriteBatch.Draw(texture, Position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
            }
            else
            {
                DrawPrimitives.DrawRectangle(BoundingRectangle, level.game.WhitePixel, Color.Red, spriteBatch, true, 1);
            }
          
        }

    }
}
