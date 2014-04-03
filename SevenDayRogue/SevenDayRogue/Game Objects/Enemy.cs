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

    public class EnemyFactory
    {
        public static Enemy getRandomEnemy(Random r, Level level, Vector2 pos)
        {
            int levelIndex = level.game.levelIndex;
            int difficulty = r.Next(levelIndex / 2, levelIndex * 2);
            int movePts = (int)Math.Round(difficulty * r.NextDouble());
            int shootPts = difficulty - movePts;

            int hp = r.Next(10) * levelIndex;
            int dmg = r.Next(levelIndex);
           

            return new Enemy(level, pos, getMoveType(r,movePts), getShootType(r,shootPts), hp, dmg);
        }

        private static EnemyMoveType getMoveType(Random r, int pts)
        {
            return (EnemyMoveType)r.Next(5);
        }

        private static EnemyShootType getShootType(Random r, int pts)
        {
            return (EnemyShootType)r.Next(6);
        }
    }

    public class Enemy
    {
        public Level level;

        public Vector2 Position;

        public Vector2 Direction;
        public float moveSpeed = 100;
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
        public int shootSpeed;

        public TimeSpan HitTimer;
        public float HitTime = .1f;

        Color c;

        public bool isWaypointLooping; //whether we go back to the first waypoint if we reached the last
        public int waypointIndex=0;
        public List<Point> waypointList = new List<Point>();

        private bool isActive = false;

        public TimeSpan shootTimer;
        public float shootTime = 1f;

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

            this.score = hp;

            LoadContent();

            c = Color.Red;

            InitMovement();
            InitShooting();
            setWaypoints();
            
        }

        private void LoadContent()
        {
          
            texture = level.game.guardTexture;

            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        private void InitMovement()
        {
            switch (moveType)
            {
                case EnemyMoveType.LargePatrol:
                    moveSpeed = 75;
                    break; 
                case EnemyMoveType.LineOfSight:
                    moveSpeed = 100;
                    break;
                case EnemyMoveType.SeekPlayer:
                    moveSpeed = 150; 
                    break;
                case EnemyMoveType.SmallPatrol:
                    moveSpeed = 50;
                    break;
                case EnemyMoveType.Static:

                    moveSpeed = 0;
                    break;
                default:
                    moveSpeed = 0;
                    break;
            }
        }

        private void InitShooting()
        {
            switch (shootType)
            {
                case EnemyShootType.Random:
                    shootTime = 1f + (float)level.game.r.NextDouble();
                    shootSpeed = GameConstants.slowEnemyShootSpeed;
                    break;
                case EnemyShootType.Shooter:
                    shootTime = 1f;
                    shootSpeed = GameConstants.slowEnemyShootSpeed;
                    break;
                case EnemyShootType.Shotgun:
                    shootTime = 2f;
                    shootSpeed = GameConstants.medEnemyShootSpeed;
                    break;
                case EnemyShootType.Sniper:
                    shootTime = 3f;
                    shootSpeed = GameConstants.snipeEnemyShootSpeed;
                    break;
                case EnemyShootType.Spray:
                    shootTime = .1f;
                    shootSpeed = GameConstants.medEnemyShootSpeed;
                    break;
                case EnemyShootType.Turret:
                    shootTime = .05f;
                    shootSpeed = GameConstants.fastEnemyShootSpeed;
                    break;
                      
                     
            }
        }

        private void setWaypoints()
        {
            switch (moveType)
            {
                case EnemyMoveType.Static:
                    break;
                case EnemyMoveType.SmallPatrol:
                    setWaypointSmallPatrol();
                    break;
                case EnemyMoveType.SeekPlayer:
                    setWaypointSeekPlayer();
                    break;
                case EnemyMoveType.LineOfSight:
                    setWaypointSeekPlayer();
                    break;
                case EnemyMoveType.LargePatrol:
                    setWaypointLargePatrol();
                    break;
                default:
                    break;
            }
        }


        //make a simple square waypoint list
        //TESTING
        private void setWaypointSquare()
        {
            isWaypointLooping = true;
            //starting tile
            Vector2 curTileVector = TileHelper.GetTilePosition(Position);
            waypointList.Add(new Point((int)curTileVector.X, (int)curTileVector.Y));
            waypointList.Add(new Point((int)curTileVector.X, (int)curTileVector.Y - 2));
            waypointList.Add(new Point((int)curTileVector.X + 2, (int)curTileVector.Y - 2));
            waypointList.Add(new Point((int)curTileVector.X + 2, (int)curTileVector.Y));
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

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                UpdateMovement(gameTime);

                UpdateShooting(gameTime);

                HitTimer -= gameTime.ElapsedGameTime;

                float dx = (float)gameTime.ElapsedGameTime.TotalSeconds;

                this.rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X);

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


        private void UpdateMovement(GameTime gameTime)
        {
         
            switch (moveType)
            {
                case EnemyMoveType.Static:
                    break;
                case EnemyMoveType.SmallPatrol:
                    UpdateWaypoint();
                    break;
                case EnemyMoveType.SeekPlayer:
                    UpdateSeekPlayerAggressive();
                    break;
                case EnemyMoveType.LineOfSight:
                    UpdateSeekPlayer();
                    break;
                case EnemyMoveType.LargePatrol:
                    UpdateWaypoint();
                    break;
                default:
                    break;
            }
            
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
            Velocity = Direction * moveSpeed;

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
            Velocity = Direction * moveSpeed;
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
            Velocity = Direction * moveSpeed;
        }

        private void UpdateShooting(GameTime gameTime)
        {
            shootTimer -= gameTime.ElapsedGameTime;
            if (shootTimer < TimeSpan.Zero)
            {
                switch (shootType)
                {
                    case EnemyShootType.Random:
                        //random direction, random timer
                        level.SpawnBullet(Position, getRandomVector(),shootSpeed,dmg, BulletType.Red, false);
                        shootTimer = TimeSpan.FromSeconds(shootTime);
                        break;
                    case EnemyShootType.Shooter:
                        level.SpawnBullet(Position, getPlayerVector(), shootSpeed, dmg, BulletType.Red, false);
                        shootTimer = TimeSpan.FromSeconds(shootTime);
                        break;
                    case EnemyShootType.Shotgun:
                        Vector2 direction = getPlayerVector();
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 newDir = new Vector2(direction.X + level.game.r.Next(-10, 10), direction.Y + level.game.r.Next(-10, 10));
                            newDir.Normalize();
                            level.SpawnBullet(Position, newDir, shootSpeed, dmg, BulletType.Red, false);
                        }
                        shootTimer = TimeSpan.FromSeconds(shootTime);
                        break;
                    case EnemyShootType.Sniper:
                        level.SpawnBullet(Position, getPlayerVector(), shootSpeed, dmg, BulletType.Red, false);
                        shootTimer = TimeSpan.FromSeconds(shootTime);
                        break;
                    case EnemyShootType.Spray:
                        Vector2 dir2 = getPlayerVector();
                        Vector2 newDir2 = new Vector2(dir2.X + level.game.r.Next(-10, 10), dir2.Y + level.game.r.Next(-10, 10));
                        newDir2.Normalize();
                        level.SpawnBullet(Position, newDir2, shootSpeed, dmg, BulletType.Red, false);
                        shootTimer = TimeSpan.FromSeconds(shootTime);
                        break;
                    case EnemyShootType.Turret:
                        level.SpawnBullet(Position, getPlayerVector(), shootSpeed, dmg, BulletType.Red, false);
                        shootTimer = TimeSpan.FromSeconds(shootTime);

                        break;
                    default:
                        break;
                }
            }
        }

        //get vector to shoot at player
        private Vector2 getPlayerVector()
        {
            Vector2 dif = level.player.Position - Position;
            if (dif.X != 0 && dif.Y != 0)
            {
                dif.Normalize();
            }
            return dif;
        }

        private Vector2 getRandomVector()
        {
            double radian = level.game.r.NextDouble() * Math.PI;
            return new Vector2((float)Math.Cos(radian), (float)Math.Sin(radian));
        }

        private void Kill(BulletType bulletType, bool isExplosion)
        {      
            level.player.getXP(score);
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
            /*
            if (waypointList.Count > 0)
            {
                Vector2 dest = TileHelper.GetWorldPosition(waypointList[waypointIndex].X, waypointList[waypointIndex].Y);
                Rectangle destRect = new Rectangle((int)dest.X, (int)dest.Y, 25, 25);
                DrawPrimitives.DrawRectangle(destRect, level.game.WhitePixel, Color.Yellow, spriteBatch, true, 1);
            }
             * */


            //Enemy Bounding Rectangle
            //DrawPrimitives.DrawRectangle(BoundingRectangle, level.game.WhitePixel, Color.Red, spriteBatch, true, 1);
         

            //spriteBatch.Draw(level.game.guardTexture, Position, Color.White);

            spriteBatch.Draw(texture, Position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);

            //position location
            //DrawPrimitives.DrawRectangle(new Rectangle((int)Position.X-2, (int)Position.Y-2, 4, 4), level.game.WhitePixel, Color.White, spriteBatch, true, 1);

            //healthbar
            Rectangle enemyHealthRec = new Rectangle((int)Position.X - BoundingRectangle.Width/2,(int)Position.Y-BoundingRectangle.Height,BoundingRectangle.Width,5);
            DrawPrimitives.DrawHealthBar(spriteBatch, level.game.WhitePixel, enemyHealthRec, Color.Red, false, false, hp, TotalHP);

        }


    }
}
