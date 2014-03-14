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

        public Vector2 Direction;
        public float speed = 100;
           
        public Vector2 Velocity;

        public Texture2D texture; //sprite?
        public Vector2 origin;
        public float rotation;

        public Vector2 curTileVector;
   

        public int TotalHP;
        public int hp;
        public int score;

        public EnemyType enemyType;

        public int dmg; //dmg inflicted by crashing into player


        public TimeSpan HitTimer;
        public float HitTime = .1f;

        Color c;

        public bool isWaypointLooping; //whether we go back to the first waypoint if we reached the last
        public int waypointIndex=0;
        public List<Point> waypointList = new List<Point>();

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

            LoadContent();

            c = Color.Red;

            setWaypointList();
        }

        private void LoadContent()
        {
          
            texture = level.game.guardTexture;

            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Update(GameTime gameTime)
        {

            UpdateWaypoint();

            HitTimer -= gameTime.ElapsedGameTime;

            float dx = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * dx;

            //Collision.HandleCollisions(level,BoundingRectangle,ref Position);

            //check if we hit player

            //check if we are 

        }

        //make a simple square waypoint list
        private void setWaypointList()
        {
            isWaypointLooping = true;
            //starting tile
             Vector2 curTileVector = TileHelper.GetTilePosition(Position);
             waypointList.Add(new Point((int)curTileVector.X, (int)curTileVector.Y));
             waypointList.Add(new Point((int)curTileVector.X, (int)curTileVector.Y - 2));
             waypointList.Add(new Point((int)curTileVector.X+2, (int)curTileVector.Y - 2));
             waypointList.Add(new Point((int)curTileVector.X+2, (int)curTileVector.Y));
        }

        private void UpdateWaypoint()
        {
            //check if we are at a waypoint
            curTileVector = TileHelper.GetTilePosition(Position);
            Point curTile = new Point((int)curTileVector.X, (int)curTileVector.Y);

            if (curTile == waypointList[waypointIndex])
            {
                waypointIndex++;
                if (isWaypointLooping && waypointIndex >= waypointList.Count)
                {
                    waypointIndex = 0;
                }
                if (waypointIndex > waypointList.Count)
                {
                    Direction = new Vector2(0, 0);
                    Velocity = new Vector2(0, 0);
                    return;
                }
            }

            Direction = new Vector2(waypointList[waypointIndex].X,waypointList[waypointIndex].Y) - curTileVector;
            Direction.Normalize();
            Velocity = Direction * speed;

            /*

            //direction = current position - world vector of waypoint tile
            Vector2 dest = TileHelper.GetWorldPosition(waypointList[waypointIndex].X, waypointList[waypointIndex].Y);

            Direction = dest - (Position);
            Direction.Normalize();
            Velocity = Direction * speed;
             * */
          

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
            //destination
            Vector2 dest = TileHelper.GetWorldPosition(waypointList[waypointIndex].X,waypointList[waypointIndex].Y);
            Rectangle destRect = new Rectangle((int)dest.X-13, (int)dest.Y-13, 25, 25);
            DrawPrimitives.DrawRectangle(destRect, level.game.WhitePixel, Color.Yellow, spriteBatch, true, 1);


            //spriteBatch.Draw(texture, Position, null, c, rotation, origin, 1f, SpriteEffects.None, 1);
            DrawPrimitives.DrawRectangle(BoundingRectangle, level.game.WhitePixel, Color.Red, spriteBatch, true, 1);
         

            //spriteBatch.Draw(level.game.guardTexture, Position, Color.White);

            spriteBatch.Draw(texture, Position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);

            //position location
            DrawPrimitives.DrawRectangle(new Rectangle((int)Position.X, (int)Position.Y, 2, 2), level.game.WhitePixel, Color.White, spriteBatch, true, 1);
        }


    }
}
