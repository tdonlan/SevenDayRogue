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
    public class Enemy
    {
        public Level level;

        public Vector2 Position;
        public Vector2 Velocity;

        public Texture2D texture; //sprite?
        public Vector2 origin;

        public int TotalHP;
        public int hp;
        public int score;

        public EnemyType enemyType;

        public int dmg; //dmg inflicted by crashing into player

        public float rotationAmt;
        public float rotation = 0f;

        public TimeSpan HitTimer;
        public float HitTime = .1f;

        Color c;

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X - 13, (int)Position.Y - 13, 25, 25);
                //return new Rectangle((int)(Position.X - origin.X), (int)(Position.Y - origin.X), (int)texture.Width, (int)texture.Height);
            }
        }

        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, texture.Width / 3);
            }
        }

        public Enemy(Level level, Vector2 position, EnemyType type, int hp, int dmg)
        {
            this.level = level;
            this.Position = position;
            this.enemyType = type;
           
            this.TotalHP = hp;
            this.hp = TotalHP;
            this.dmg = dmg;

            HitTimer = TimeSpan.FromSeconds(1); //brief invincibility when spawning

            score = 1;

            rotationAmt = (float)level.game.r.NextDouble() - .5f;

            LoadContent();

            c = Color.Red;
        }

        private void LoadContent()
        {
          
            texture = level.game.WhitePixel;

            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Update(GameTime gameTime)
        {
            rotation += rotationAmt * (float)gameTime.ElapsedGameTime.TotalSeconds;
            HitTimer -= gameTime.ElapsedGameTime;

            float dx = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * dx;

            //Collision.HandleCollisions(level,BoundingRectangle,ref Position);

            //check if we hit player

            //check if we are 

        }



        private void Kill(BulletType bulletType, bool isExplosion)
        {
            

            level.DespawnEnemy(this);

        }


        public void Hit(BulletType bulletType, int dmg, bool isExplosion)
        {
                if (HitTimer <= TimeSpan.Zero)
                {
                    HitTimer = TimeSpan.FromSeconds(HitTime);
                    this.hp -= dmg;
        
                    if (hp <= 0)
                    {
                        Kill(bulletType, isExplosion);
                    }
                }
            
        }

        private bool CheckCollision()
        {
            if (BoundingRectangle.Intersects(level.player.BoundingRectangle))
            {
                return true;
            }
            return false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

   
            //spriteBatch.Draw(texture, Position, null, c, rotation, origin, 1f, SpriteEffects.None, 1);
            DrawPrimitives.DrawRectangle(BoundingRectangle, level.game.WhitePixel, Color.Red, spriteBatch, true, 1);
            // DrawPrimitives.DrawCircle(BoundingCircle, level.game.WhitePixel, Color.Indigo, spriteBatch);
        }


    }
}
