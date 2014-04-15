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
        public Vector2 Acceleration;

        public TimeSpan shootTimer;
        public float shootTime = .1f;

        public int totalHP;
        public int HP;

        public int nanites=100;

        public int xpLevel;
        public int xp;
        public List<int> levelXPCurve = new List<int>();

        public TimeSpan hitTimer;
        public float hitTime = 1f;

        public Texture2D texture;
        public Vector2 origin;
        public float rotation;

        public float moveSpeed;

        public int shootSpeed;
        public int shootDmg;

        public int selectedGun = 0;
        public List<Gun> gunList = new List<Gun>();


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

        public int XPRelative
        {
            get
            {
                return xp - levelXPCurve[xpLevel - 1];
            }
        }

        public int XPNeededRelative
        {
            get
            {
                return levelXPCurve[xpLevel] - levelXPCurve[xpLevel-1];
            }
        }

        public int XPNeeded
        {
            get { return levelXPCurve[xpLevel]; }
        }

        public Player(Level level, Vector2 position)
        {
            this.level = level;
            this.Position = position;
            this.totalHP = 100;
            this.HP = this.totalHP; 

            this.xp = 50;
            this.xpLevel = 1;
            setXPCurveList();

            this.moveSpeed = GameConstants.playerSpeed;

            gunList.Add(Gun.getMiniGun());
            setGunStats();

            LoadContent();
        }

        public void LoadContent()
        {
            texture = level.game.playerTexture;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            rotation = 0;
        }

        private void setGunStats()
        {
            if (gunList.Count > selectedGun)
            {
                Gun g = gunList[selectedGun];
                this.shootDmg = g.damage;
                this.shootSpeed = g.speed;
                this.shootTime = g.fireRate;

            }

        }

        private void setXPCurveList()
        {
            levelXPCurve.Add(0);
            int xpAmt = 100;
            for(int i=1;i<100;i++)
            {
                levelXPCurve.Add(xpAmt);
                xpAmt += (int)Math.Round(xpAmt * 1.1);
            }
        }

        public void Update(GameTime gameTime)
        {

            //timers
            shootTimer -= gameTime.ElapsedGameTime;
            hitTimer -= gameTime.ElapsedGameTime;


            Vector2 previousPosition = this.Position;

            float dx = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleInput(gameTime);

             Position += Velocity * dx;
             Collision.HandleCollisions(level, BoundingRectangle, ref Position);


             if (Position.X == previousPosition.X)
             {
                 Acceleration.X = 0;
                 Velocity.X = 0;
             }

             if (Position.Y == previousPosition.Y)
             {
                 Acceleration.Y = 0;
                 Velocity.Y = 0;

             }
           

            //Smoother collision, but doesn't allow going all the way to wall
            /*
           
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
           */
             
        }

        private void HandleInput(GameTime gametime)
        {
            if (level.game.gameInput.IsNewKeyPress(Keys.Space))
            {
                if (level.CheckAtEnd(BoundingRectangle))
                {
                    level.game.LevelUp();
                }
                else if (level.CheckAtStart(BoundingRectangle))
                {
                    level.game.LevelDown();
                }
            }

            if (level.game.gameInput.isMouseButtonPress(MouseButton.LeftButton))
            {
                Shoot();
            }


            Vector2 moveVector = new Vector2(level.game.gameInput.HMovement, level.game.gameInput.VMovement);
            if (moveVector.X != 0 && moveVector.Y != 0)
                moveVector.Normalize();


            float dx = (float)gametime.ElapsedGameTime.TotalSeconds;

            this.Acceleration = moveVector;
            
            this.Velocity += this.Acceleration * moveSpeed * dx;

            this.Velocity *= .8f;

            this.rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X);
        }

        private void Shoot()
        {
            if (shootTimer < TimeSpan.Zero)
            {

                if (nanites > gunList[selectedGun].fireCost)
                {
                    nanites -= gunList[selectedGun].fireCost;

                    shootTimer = TimeSpan.FromSeconds(shootTime);

                    Vector2 dir = (level.game.gameInput.mousePos + new Vector2(level.cameraPosition, level.cameraPositionYAxis)) - Position;
                    dir.Normalize();

                    level.SpawnBullet(Position, dir, shootSpeed, shootDmg, BulletType.Red, true);
                }

            }
        }

        public void Hit(int dmg)
        {
            if (hitTimer <= TimeSpan.Zero)
            {
                hitTimer = TimeSpan.FromSeconds(hitTime);

                HP -= dmg;
                if (HP <= 0)
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            level.game.Die();
        }

        public void GetNanites(int amt)
        {
            nanites += amt;
        }

        public void getXP(int xp)
        {
            this.xp += xp;
            setXPLevel();
        }

        public void setXPLevel()
        {
            int index = xpLevel;
            while (levelXPCurve[index] < xp)
            {
                index++;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            xpLevel++;
            totalHP += 10;
            HP = totalHP;

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Bounding Rectangle
            //DrawPrimitives.DrawRectangle(BoundingRectangle, level.game.WhitePixel, Color.Pink, spriteBatch, true, 1);
            
            //Text position
            //spriteBatch.DrawString(level.game.font, Position.X + "," + Position.Y, new Vector2(BoundingRectangle.Center.X, BoundingRectangle.Center.Y), Color.Black);

            //spriteBatch.Draw(level.game.playerTexture, Position, Color.White);
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);

            //Position Rect
            //DrawPrimitives.DrawRectangle(new Rectangle((int)Position.X - 2, (int)Position.Y - 2, 5, 5), level.game.WhitePixel, Color.Red, spriteBatch, true, 1);
        }

    }
}
