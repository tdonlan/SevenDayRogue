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

using System.Xml;



namespace SevenDayRogue
{
    public class Level
    {

        public Game1 game;

        public Tile[,] tileArray;

        private List<Point> floorList = new List<Point>();

        public int Width
        {
            get { return tileArray.GetLength(0); }
        }

        public int Height
        {
            get { return tileArray.GetLength(1); }
        }

        public Matrix cameraTransform;
        public float cameraPosition;
        public float cameraPositionYAxis;
        private int viewHeight;
        private int viewWidth;

        public Player player;

        public List<Bullet> playerBulletList;
        public List<Enemy> enemyList;

        //New Level constructore
        public Level(Game1 game)
        {
            this.game = game;
            Vector2 startPos = new Vector2(0,0);
            if (game.r.Next(2) == 0)
            {
                startPos = LoadCave();
                //LoadMaze();
            }
            else
            {
               startPos =  LoadBerryDungeon();
            }

             this.player = new Player(this, startPos);
           
             LoadContent();
        }

        //Existing level constructor
        //isUp = We went up to get to this level
        public Level(Game1 game, Tile[,] tileArray, bool isUp)
        {
            this.game = game;
            this.tileArray = tileArray;

            Vector2 startPos;

            if(isUp)
            {
                startPos = getLevelPos(TileType.StairDown);
            }
            else
            {
                startPos = getLevelPos(TileType.StairUp);
            }

            this.player = new Player(this, startPos);

           

            LoadContent();
        }

        private void LoadContent()
        {
            this.playerBulletList = new List<Bullet>();
            this.enemyList = new List<Enemy>();
            SpawnEnemies();
        }

        
        //return where a certain tile type is (start / end)
        private Vector2 getLevelPos(TileType type)
        {

            Vector2 pos = new Vector2(0,0);
             for (int i = 0; i < tileArray.GetLength(0); i++)
            {
                for (int j = 0; j < tileArray.GetLength(1); j++)
                {
                   if( tileArray[i, j].type == type)
                   {
                       pos = TileHelper.GetWorldPosition(i,j);
                   }
                }
            }
            return pos;
        }

       
        private Vector2 LoadCave()
        {

            Vector2 startPos = new Vector2(1000, 1000);

            int height = GameConstants.LevelHeight;
            int width = GameConstants.LevelWidth;
            CellAutoCave CACave = new CellAutoCave(width, height);

            tileArray = new Tile[width, height];

            for (int i = 0; i < tileArray.GetLength(0); i++)
            {
                for (int j = 0; j < tileArray.GetLength(1); j++)
                {
                    bool isSolid = false;
                    TileType tileType = TileType.Stone;

                    if (i == 0 || j == 0 || i == tileArray.GetLength(0) - 1 || j == tileArray.GetLength(1) - 1)
                    {
                        isSolid = true;
                    }
                    else
                    {

                        if (CACave.grid[i, j] == 1)
                        {
                            isSolid = true;
                        }
                        else if (CACave.grid[i, j] == 2)
                        {
                            startPos = TileHelper.GetWorldPosition(i, j);
                            tileType = TileType.StairDown;
                        }
                        else if (CACave.grid[i, j] == 3)
                        {
                            tileType = TileType.StairUp;
                        }
                        else
                        {
                            floorList.Add(new Point(i, j));
                        }
                       
                    }


                    tileArray[i, j] = new Tile(isSolid, tileType);
                }
            }

            return startPos;

        }

        private void LoadMaze()
        {
            int height = GameConstants.LevelHeight;
            int width = GameConstants.LevelWidth;

          
            MazeBuilder mb = new MazeBuilder(height, width,true, 0);

            tileArray = new Tile[width, height];

            for (int i = 0; i < tileArray.GetLength(0); i++)
            {
                for (int j = 0; j < tileArray.GetLength(1); j++)
                {
                    bool isSolid = false;
                    if (i == 0 || i == tileArray.GetLength(0) - 1 || j == tileArray.GetLength(1) - 1)
                    {
                        isSolid = true;
                    }
                    else
                    {

                        if (mb.maze[i, j] == '#')
                        {
                            isSolid = true;
                        }
                        else
                        {
                            floorList.Add(new Point(i, j));
                        }

                    }

                    tileArray[i, j] = new Tile(isSolid, TileType.Stone);
                }
            }

        }

        private Vector2 LoadBerryDungeon()
        {

            Vector2 startPos = new Vector2(1000,1000);

            int height = GameConstants.LevelHeight;
            int width = GameConstants.LevelWidth;

            Generation berryGen = new Generation();
            berryGen.Generate(width, height);

            tileArray = new Tile[berryGen.Width, berryGen.Height];

            for (int i = 0; i < berryGen.Height; i++)
            {
                for (int j = 0; j < berryGen.Width; j++)
                {
                    bool isSolid = false;
                    TileType tileType = TileType.Stone;

                    if (i == 0 || i == tileArray.GetLength(0) - 1 || j == tileArray.GetLength(1) - 1 || j == 0)
                    {
                        isSolid = true;
                    }
                    else
                    {

                        if (berryGen.Map[i, j] == Generation.Type.Wall || berryGen.Map[i, j] == Generation.Type.Stone)
                        {
                            isSolid = true;
                        }
                        else if (berryGen.Map[i, j] == Generation.Type.Start)
                        {
                            startPos = TileHelper.GetWorldPosition(i, j);
                            tileType = TileType.StairDown;
                        }
                        else if (berryGen.Map[i, j] == Generation.Type.End)
                        {
                            tileType = TileType.StairUp;
                        }
                        else
                        {
                            floorList.Add(new Point(i, j));
                        }
                        
                    }

                    tileArray[i, j] = new Tile(isSolid, tileType);
                }
            }

            return startPos;

        }

        

        public void Update(GameTime gameTime)
        {
            player.Update(gameTime);

            UpdateBullets(gameTime);
        }

        public void UpdateBullets(GameTime gameTime)
        {
            for (int i = playerBulletList.Count - 1; i >= 0; i--)
            {
                playerBulletList[i].Update(gameTime);
            }
        }

        public void UpdateEnemies(GameTime gameTime)
        {
            for (int i = enemyList.Count - 1; i >= 0; i--)
            {
                enemyList[i].Update(gameTime);
            }
        }

        public bool CheckAtStart(Rectangle boundingRec)
        {
            return TileHelper.CheckCollisionWithType(boundingRec, tileArray, TileType.StairDown);
                
        }

        public bool CheckAtEnd(Rectangle boundingRec)
        {
            return TileHelper.CheckCollisionWithType(boundingRec, tileArray, TileType.StairUp);
        }

        public bool GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= tileArray.GetLength(0))
                return true;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= tileArray.GetLength(1))
                return true;

            if (tileArray[x, y].isSolid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Initial spawning of enemies in a level
        public void SpawnEnemies()
        {
            int enemyCount = (int)Math.Round(floorList.Count * GameConstants.enemyPercent);
          
            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy(popRandomFloorPoint(), EnemyType.Grunt);
            }
        }

        //gets a random floor point (used for spawning stuff) and removes it from the list
        private Vector2 popRandomFloorPoint()
        {
            Point p = floorList[game.r.Next(floorList.Count - 1)];
            floorList.Remove(p);

            return TileHelper.GetWorldPosition(p.X, p.Y);
           

        }

        public void SpawnEnemy(Vector2 pos, EnemyType type)
        {
            Enemy e = new Enemy(this, pos, type, 10, 10);
            enemyList.Add(e);
        }

        public void DespawnEnemy(Enemy e)
        {
            enemyList.Remove(e);
        }

        public void SpawnBullet(Vector2 pos, Vector2 direction, BulletType type, bool isPlayer)
        {
            Bullet b = new Bullet(this, pos, direction, 1, 750, BulletType.Red, isPlayer);
            playerBulletList.Add(b);
        }

        public void DespawnBullet(Bullet b)
        {
            if (b.isPlayers)
            {
                playerBulletList.Remove(b);
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color transBlack = Color.Lerp(Color.Black, Color.Transparent, .5f);
            ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
            cameraTransform = Matrix.CreateTranslation(-cameraPosition, -cameraPositionYAxis, 0.0f);

            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);

            DrawTiles(spriteBatch);
            player.Draw(gameTime, spriteBatch);

            foreach (Enemy e in enemyList)
            {
                e.Draw(gameTime, spriteBatch);
            }

            foreach (Bullet b in playerBulletList)
            {
                b.Draw(gameTime, spriteBatch);
            }
            

            spriteBatch.End();


            DrawHUD(gameTime, spriteBatch);

        }

        private void DrawTiles(SpriteBatch spriteBatch)
        {
            // Calculate the visible range of tiles.
            int left = (int)Math.Floor(cameraPosition / GameConstants.TileWidth);
            int right = left + (spriteBatch.GraphicsDevice.Viewport.Width / GameConstants.TileWidth) + 1;
            right = Math.Min(right, Width - 1);

            int top = (int)Math.Floor(cameraPositionYAxis / GameConstants.TileHeight);
            int bottom = top + (spriteBatch.GraphicsDevice.Viewport.Height / GameConstants.TileHeight) + 1;
            bottom = Math.Min(bottom, Height - 1);

            // For each tile position
            for (int y =top; y <= bottom; ++y)
            {
                for (int x = left; x <= right; ++x)
                {
                    DrawTile(spriteBatch, tileArray[x, y], x, y);
               
                }
            }
        }


        private void DrawTile(SpriteBatch spriteBatch, Tile t, int x, int y)
        {
            Rectangle rec = TileHelper.GetTileRectangle(x, y);
            if (t.isSolid)
            {
                DrawPrimitives.DrawRectangle(rec, game.WhitePixel, Color.Gray, spriteBatch, true, 1);
                //spriteBatch.Draw(game.wallTexture, rec, Color.White);
            }
            else
            {
                if (t.type == TileType.StairDown)
                {
                    DrawPrimitives.DrawRectangle(rec, game.WhitePixel, Color.Orange, spriteBatch, true, 1);
                }
                else if (t.type == TileType.StairUp)
                {
                    DrawPrimitives.DrawRectangle(rec, game.WhitePixel, Color.Purple, spriteBatch, true, 1);
                }
                else
                {
                    DrawPrimitives.DrawRectangle(rec, game.WhitePixel, Color.White, spriteBatch, true, 1);
                }
                  
            }
          
        }


        private void DrawHUD(GameTime gameTime, SpriteBatch spriteBatch)
        {


            spriteBatch.Begin();

            spriteBatch.DrawString(game.font, game.levelIndex.ToString(), new Vector2(20, 20), Color.Black);

            Color transBlack = Color.Lerp(Color.Black, Color.Transparent, .1f);


            Vector2 hudPOS = new Vector2(0,600);
            Rectangle HUDRec = new Rectangle((int)hudPOS.X,(int)hudPOS.Y,1280,200);
            //DrawPrimitives.DrawRectangle(HUDRec, game.WhitePixel, transBlack, spriteBatch, true, 1);


            //crosshairs
            DrawPrimitives.DrawCrossHair(spriteBatch, game.WhitePixel, game.gameInput.mousePos, Color.HotPink);


            spriteBatch.End();
          
            
        }

        public void ScrollCamera(Viewport viewport)
        {

            const float ViewMargin = 0.5f;

            // Calculate the edges of the screen.
            float marginWidth = viewport.Width * ViewMargin;
            float marginLeft = cameraPosition + marginWidth;
            float marginRight = cameraPosition + viewport.Width - marginWidth;

            const float TopMargin = 0.5f;
           const float BottomMargin = 0.5f;


            float marginTop = cameraPositionYAxis + viewport.Height * TopMargin;
            float marginBottom = cameraPositionYAxis + viewport.Height - viewport.Height * BottomMargin;


            viewHeight = viewport.Height;
            viewWidth = viewport.Width;

            // Calculate how far to scroll when the player is near the edges of the screen.
            float cameraMovement = 0.0f;
            if (player.Position.X < marginLeft)
                cameraMovement = player.Position.X - marginLeft;
            else if (player.Position.X > marginRight)
                cameraMovement = player.Position.X - marginRight;

            // Calculate how far to vertically scroll when the player is near the top or bottom of the screen.  
            float cameraMovementY = 0.0f;
            if (player.Position.Y < marginTop) //above the top margin  
                cameraMovementY = player.Position.Y - marginTop;
            else if (player.Position.Y > marginBottom) //below the bottom margin  
                cameraMovementY = player.Position.Y - marginBottom;

            // Update the camera position, but prevent scrolling off the ends of the level.
            float maxCameraPosition = GameConstants.TileWidth * tileArray.GetLength(0) - viewport.Width;
            float maxCameraPositionYOffset = GameConstants.TileHeight * tileArray.GetLength(1) - viewport.Height;

           
            cameraPosition = MathHelper.Clamp(cameraPosition + cameraMovement, 0.0f, maxCameraPosition);
            cameraPositionYAxis = MathHelper.Clamp(cameraPositionYAxis + cameraMovementY, 0.0f, maxCameraPositionYOffset);

            //cameraPosition = cameraPosition + cameraMovement;
            //cameraPositionYAxis = cameraPositionYAxis + cameraMovementY;
        }

                  
               
    }
}
