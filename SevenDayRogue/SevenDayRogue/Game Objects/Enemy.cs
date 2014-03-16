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

      
        public EnemyMoveType moveType;
        public EnemyShootType shootType;

        public int dmg; //dmg inflicted by crashing into player


        public TimeSpan HitTimer;
        public float HitTime = .1f;

        Color c;

        public bool isWaypointLooping; //whether we go back to the first waypoint if we reached the last
        public int waypointIndex=0;
        public List<Point> waypointList = new List<Point>();

        private bool isActive = false;

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)(Position.X - origin.X), (int)(Position.Y - origin.Y), 25, 25);
             
            }
        }

        public Enemy(Level level, Vector2 position, EnemyMoveType moveType, EnemyShootType shootType, int hp, int dmg)
        {
            this.level = level;
            this.Position = position;
            this.moveType = moveType;
            this.shootType = shootType;
           
            this.TotalHP = hp;
            this.hp = TotalHP;
            this.dmg = dmg;

            HitTimer = TimeSpan.FromSeconds(1); //brief invincibility when spawning

            score = 1;

            LoadContent();

            c = Color.Red;

            //setWaypointSquare();
            //setWaypointSeekPlayer();
            //setWaypointSmallPatrol();
            setWaypointLargePatrol();
        }

        private void LoadContent()
        {
          
            texture = level.game.guardTexture;

            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                UpdateWaypoint();
                //UpdateSeekPlayer();
                //UpdateSeekPlayerAggressive();

                HitTimer -= gameTime.ElapsedGameTime;

                float dx = (float)gameTime.ElapsedGameTime.TotalSeconds;

                Position += Velocity * dx;

                //Collision.HandleCollisions(level,BoundingRectangle,ref Position);

            }
            else
            {
                Vector2 losVector = new Vector2();
                if (Collision.getLOS(level, Position, level.player.Position, ref losVector))
                {
                    isActive = true;
                }
            }

        }

        //make a simple square waypoint list
        private void setWaypointSquare()
        {
            isWaypointLooping = true;
            //starting tile
             Vector2 curTileVector = TileHelper.GetTilePosition(Position);
             waypointList.Add(new Point((int)curTileVector.X, (int)curTileVector.Y));
             waypointList.Add(new Point((int)curTileVector.X, (int)curTileVector.Y - 2));
             waypointList.Add(new Point((int)curTileVector.X+2, (int)curTileVector.Y - 2));
             waypointList.Add(new Point((int)curTileVector.X+2, (int)curTileVector.Y));
        }


        private void setWaypointSmallPatrol()
        {
            //pick a random space nearby, use A* to get there and back.
            Vector2 tilePos = TileHelper.GetTilePosition(Position);
            List<Point> floorList = TileHelper.getFloorList(level, new Rectangle((int)tilePos.X - 5, (int)tilePos.Y - 5, 10, 10));

            Point randPt = floorList[level.game.r.Next(floorList.Count - 1)];

            List<Point> ptList1 = PathFinder.Pathfind(level, (int)tilePos.X, (int)tilePos.Y, randPt.X, randPt.Y);

            waypointIndex = 0;
            isWaypointLooping = true;
            waypointList.AddRange(ptList1);
            ptList1.Reverse();
            waypointList.AddRange(ptList1);
        }

        private void setWaypointLargePatrol()
        {

            waypointIndex = 0;
            isWaypointLooping = true;

            //pick 2-5 points medium distance, use A* to traverse them.
            Vector2 tilePos = TileHelper.GetTilePosition(Position);
            List<Point> floorList = TileHelper.getFloorList(level, new Rectangle((int)tilePos.X - 15, (int)tilePos.Y - 15, 30, 30));

            Point endPt = new Point((int)tilePos.X, (int)tilePos.Y);
            Point curPt = endPt;
            for (int i = 0; i < level.game.r.Next(5); i++)
            {
                Point newPt = floorList[level.game.r.Next(floorList.Count - 1)];
                List<Point> ptList1 = PathFinder.Pathfind(level, curPt.X, curPt.Y, newPt.X, newPt.Y);
                waypointList.AddRange(ptList1);
                curPt = newPt;
            }

            List<Point> ptList2 = PathFinder.Pathfind(level, curPt.X, curPt.Y, endPt.X, endPt.Y);
            waypointList.AddRange(ptList2);

        }

        //use A* to go after the player
        //need to keep calling during update
        private void setWaypointSeekPlayer()
        {
            isWaypointLooping = false;
            Vector2 enemyTilePos = TileHelper.GetTilePosition(Position);
            Vector2 playertilePos = TileHelper.GetTilePosition(level.player.Position);
            waypointList = PathFinder.Pathfind(level, (int)enemyTilePos.X, (int)enemyTilePos.Y, (int)playertilePos.X, (int)playertilePos.Y);

            waypointIndex = 0;
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
                else if (waypointIndex >= waypointList.Count)
                {
                    waypointIndex--;
                    Direction = new Vector2(0, 0);
                    Velocity = new Vector2(0, 0);
                    return;
                }
            }

            Direction = new Vector2(waypointList[waypointIndex].X,waypointList[waypointIndex].Y) - curTileVector;
            if (Direction.X != 0 && Direction.Y != 0)
            {
                Direction.Normalize();
            }
            Velocity = Direction * speed;

        }


        //follow waypoints.  When we get to end, seek the player again.
        private void UpdateSeekPlayer()
        {
            //check if we are at a waypoint
            curTileVector = TileHelper.GetTilePosition(Position);
            Point curTile = new Point((int)curTileVector.X, (int)curTileVector.Y);

            if (curTile == waypointList[waypointIndex])
            {
                waypointIndex++;
                if ( waypointIndex >= waypointList.Count)
                {
                    waypointIndex = 0;
                    setWaypointSeekPlayer();
                    return;
                }
            }

            Direction = new Vector2(waypointList[waypointIndex].X, waypointList[waypointIndex].Y) - curTileVector;
            Direction.Normalize();
            Velocity = Direction * speed;
        }

        private void UpdateSeekPlayerAggressive()
        {
            //check if we are at a waypoint
            curTileVector = TileHelper.GetTilePosition(Position);
            Point curTile = new Point((int)curTileVector.X, (int)curTileVector.Y);

            Vector2 playertilePos = TileHelper.GetTilePosition(level.player.Position);
            Point playertilePoint = new Point((int)playertilePos.X, (int)playertilePos.Y);
            if (playertilePoint != waypointList[waypointList.Count-1])
            {
                waypointIndex = 0;
                setWaypointSeekPlayer();
                return;
            }

            //Vector2 waypointTileCenterPos = TileHelper.GetWorldPosition(waypointList[waypointIndex].X, waypointList[waypointIndex].Y);
            //if(DrawPrimitives.getDistance(waypointTileCenterPos,Position) < GameConstants.TileWidth /2)

            if (curTile == waypointList[waypointIndex])
            {
                waypointIndex++;
                if (waypointIndex >= waypointList.Count)
                {
                    waypointIndex = 0;
                    setWaypointSeekPlayer();
                    return;
                }
            }

            //do direction basedon actual positions (not tile centers)
          // Direction = TileHelper.GetWorldPosition(waypointList[waypointIndex].X, waypointList[waypointIndex].Y) - Position;

            Direction = new Vector2(waypointList[waypointIndex].X, waypointList[waypointIndex].Y) - curTileVector;
            Direction.Normalize();
            Velocity = Direction * speed;
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
            Rectangle destRect = new Rectangle((int)dest.X, (int)dest.Y, 25, 25);
            DrawPrimitives.DrawRectangle(destRect, level.game.WhitePixel, Color.Yellow, spriteBatch, true, 1);


            //spriteBatch.Draw(texture, Position, null, c, rotation, origin, 1f, SpriteEffects.None, 1);
            DrawPrimitives.DrawRectangle(BoundingRectangle, level.game.WhitePixel, Color.Red, spriteBatch, true, 1);
         

            //spriteBatch.Draw(level.game.guardTexture, Position, Color.White);

            spriteBatch.Draw(texture, Position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);

            //position location
            DrawPrimitives.DrawRectangle(new Rectangle((int)Position.X-2, (int)Position.Y-2, 4, 4), level.game.WhitePixel, Color.White, spriteBatch, true, 1);

        }


    }
}
