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

using Krypton;
using Krypton.Lights;


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
        public List<Bullet> enemyBulletList;
        public List<Enemy> enemyList;


        //Lighting
        Light2D playerLight;
        Light2D startLight;
        Light2D endLight;

        //New Level constructore
        public Level(Game1 game)
        {
            this.game = game;
            Vector2 startPos = new Vector2(0,0);

            //startPos = LoadArena();

            
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
            LoadLights();
            //CreateLights(game.mLightTexture, 10);
            //CreateHulls(5, 5);
            LoadHulls();

            this.playerBulletList = new List<Bullet>();
            this.enemyBulletList = new List<Bullet>();
            this.enemyList = new List<Enemy>();
            SpawnEnemies();


        }

     
        private void LoadLights()
        {
            game.krypton.Lights.Clear();
            
            playerLight = new Light2D()
            {
                Texture = game.mLightTexture,
                Range =500,
                Color = Color.White,
              
                Intensity = 1f,
                Angle = MathHelper.TwoPi,
                X = 0,
                Y = 0,
                Fov = MathHelper.TwoPi,
                
                
            };
            
            game.krypton.Lights.Add(playerLight);

            startLight = new Light2D()
            {
                Texture = game.mLightTexture,
                Range = 100,
                Color = Color.Orange,

                Intensity = 1f,
                Angle = MathHelper.TwoPi,
                X = getLevelPos(TileType.StairDown).X,
                Y = getLevelPos(TileType.StairDown).Y,
                Fov = MathHelper.TwoPi,

            };
            game.krypton.Lights.Add(startLight);

            endLight = new Light2D()
            {
                Texture = game.mLightTexture,
                Range = 100,
                Color = Color.Purple,

                Intensity = 1f,
                Angle = MathHelper.TwoPi,
                X = getLevelPos(TileType.StairUp).X,
                Y = getLevelPos(TileType.StairUp).Y,
                Fov = MathHelper.TwoPi,

            };
            game.krypton.Lights.Add(endLight);

           
        }

        //iterate over the entire level adding tiles to the hull set?
        private void LoadHulls()
        {
            game.krypton.Hulls.Clear();

            for (int i = 0; i < tileArray.GetLength(0); i++)
            {
                for (int j = 0; j < tileArray.GetLength(1); j++)
                {
                    if (tileArray[i, j].isSolid)
                    {
                        var hull =  ShadowHull.CreateRectangle(new Vector2(GameConstants.TileWidth, GameConstants.TileHeight));
                        hull.Position = TileHelper.GetWorldPosition(i, j) + new Vector2(GameConstants.TileWidth/2,GameConstants.TileHeight/2);
                        game.krypton.Hulls.Add(hull);
                        
                    }
                   
                }
            }
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

        private Vector2 LoadArena()
        {

            int height = 20;
            int width = 20;

            Vector2 startPos = new Vector2(width / 2 * GameConstants.TileWidth, height / 2 * GameConstants.TileHeight);

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
                    else if (game.r.Next(100) < 10)
                    {
                        isSolid = true;
                    }

                    else
                    {
                        floorList.Add(new Point(i, j));
                        isSolid = false;
                    }


                    tileArray[i, j] = new Tile(isSolid, tileType);
                }
            }

            return startPos;
        }

       
        private Vector2 LoadCave()
        {

            Vector2 startPos = new Vector2(1000, 1000);

            int height = GameConstants.LevelHeight;
            int width = GameConstants.LevelWidth;
            bool hasStart = false;
            bool hasEnd = false;

            CellAutoCave CACave = null;
            while (!hasStart && !hasEnd)
            {
                CACave = new CellAutoCave(width, height);
                hasStart = CACave.hasStart;
                hasEnd = CACave.hasEnd;

            }

            if (CACave != null)
            {
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

            Generation berryGen = null;
            bool hasStart = false;
            bool hasEnd = false;

            while (!hasStart && !hasEnd)
            {
                 berryGen = new Generation();
                berryGen.Generate(width, height);
                hasStart = berryGen.hasStart;
                hasEnd = berryGen.hasEnd;
            }

            if (berryGen != null)
            {
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
            }

            return startPos;

        }

        

        public void Update(GameTime gameTime)
        {
            player.Update(gameTime);
            UpdateLights(gameTime);

            UpdateBullets(gameTime);
            UpdateEnemies(gameTime);

            
        }

        private void UpdateLights(GameTime gametime)
        {
            /*
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                foreach (Light2D light in game.krypton.Lights)
                {
                    light.Position = new Vector2(light.Position.X, light.Position.Y + 10);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                foreach (Light2D light in game.krypton.Lights)
                {
                    light.Position = new Vector2(light.Position.X, light.Position.Y - 10);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                foreach (Light2D light in game.krypton.Lights)
                {
                    light.Position = new Vector2(light.Position.X - 10, light.Position.Y);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                foreach (Light2D light in game.krypton.Lights)
                {
                    light.Position = new Vector2(light.Position.X + 10, light.Position.Y);
                }
            }
             * */

            playerLight.Position = player.Position;

            /*
            foreach (Light2D light in game.krypton.Lights)
            {
                light.Position = player.Position;

            }
             * */
        }

        public void UpdateBullets(GameTime gameTime)
        {
            for (int i = playerBulletList.Count - 1; i >= 0; i--)
            {
                if (isOnScreen(playerBulletList[i].BoundingRectangle))
                {
                    playerBulletList[i].Update(gameTime);
                }
                else
                {
                    DespawnBullet(playerBulletList[i]);
                }
            }

            for (int i = enemyBulletList.Count - 1; i >= 0; i--)
            {
                if (isOnScreen(enemyBulletList[i].BoundingRectangle))
                {
                    enemyBulletList[i].Update(gameTime);
                }
                else
                {
                    DespawnBullet(enemyBulletList[i]);
                }
            }
        }

        public void UpdateEnemies(GameTime gameTime)
        {
            for (int i = enemyList.Count - 1; i >= 0; i--)
            {
                if (isOnScreen(enemyList[i].BoundingRectangle))
                {
                    enemyList[i].Update(gameTime);
                }
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


        public bool isOnScreen(Rectangle rec)
        {
            int left = (int)Math.Floor(cameraPosition);
            int right = left + viewWidth;

            int top = (int)Math.Floor(cameraPositionYAxis);
            int bottom = top + viewHeight;

            if (((rec.Left) >= left && (rec.Right) <= right) &&
                ((rec.Top) >= top && (rec.Bottom) <= bottom))
                return true;
            else
                return false;
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

                enemyList.Add(EnemyFactory.getRandomEnemy(game.r, this, popRandomFloorPoint()));
                //SpawnEnemy(popRandomFloorPoint(), EnemyMoveType.SeekPlayer, EnemyShootType.Shotgun);
            }
        }

        //gets a random floor point (used for spawning stuff) and removes it from the list
        private Vector2 popRandomFloorPoint()
        {
            Point p = floorList[game.r.Next(floorList.Count - 1)];
            floorList.Remove(p);

            return TileHelper.GetWorldPosition(p.X, p.Y);
           

        }


        

        public void SpawnEnemy(Vector2 pos, EnemyMoveType moveType, EnemyShootType shootType)
        {
            Enemy e = new Enemy(this, pos, moveType, shootType, 10, 10);
            enemyList.Add(e);
        }

        public void DespawnEnemy(Enemy e)
        {
            enemyList.Remove(e);
        }

        public void SpawnBullet(Vector2 pos, Vector2 direction, BulletType type, bool isPlayer)
        {
            if (isPlayer)
            {
                Bullet b = new Bullet(this, pos, direction, 10, 750, BulletType.Red, isPlayer);
                playerBulletList.Add(b);
            }
            else
            {
                Bullet b = new Bullet(this, pos, direction, 10, 250, BulletType.Red, isPlayer);
                enemyBulletList.Add(b);
            }
        }

        public void DespawnBullet(Bullet b)
        {
     

            if (b.isPlayers)
            {
                playerBulletList.Remove(b);
            }
            else
            {
                enemyBulletList.Remove(b);
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            game.krypton.Matrix = cameraTransform;
            game.krypton.SpriteBatchCompatablityEnabled = true;
            game.krypton.CullMode = CullMode.None;

            game.krypton.Bluriness = 10;
            game.krypton.LightMapPrepare();
            Color transBlack = Color.Lerp(Color.Gray, Color.Black, .90f);


            game.krypton.AmbientColor = transBlack;

           
            // Make sure we clear the backbuffer *after* Krypton is done pre-rendering
            game.GraphicsDevice.Clear(Color.White);

            // ----- DRAW STUFF HERE ----- //
            // By drawing here, you ensure that your scene is properly lit by krypton.
            // Drawing after KryptonEngine.Draw will cause you objects to be drawn on top of the lightmap (can be useful, fyi)
            // ----- DRAW STUFF HERE ----- //

            DrawLevel(gameTime, spriteBatch);

            // Draw krypton (This can be omited if krypton is in the Component list. It will simply draw krypton when base.Draw is called
            game.krypton.Draw(gameTime);
            this.DebugDraw();

            // Draw the shadow hulls as-is (no shadow stretching) in pure white on top of the shadows
            // You can omit this line if you want to see what the light-map looks like :)
          
            DrawHUD(gameTime, spriteBatch);

        }

        private void DebugDraw()
        {
            game.krypton.RenderHelper.Effect.CurrentTechnique = game.krypton.RenderHelper.Effect.Techniques["DebugDraw"];
            game.GraphicsDevice.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.WireFrame,
            };
            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                // Clear the helpers vertices
                game.krypton.RenderHelper.ShadowHullVertices.Clear();
                game.krypton.RenderHelper.ShadowHullIndicies.Clear();

                foreach (var hull in game.krypton.Hulls)
                {
                    game.krypton.RenderHelper.BufferAddShadowHull(hull);
                }


                foreach (var effectPass in game.krypton.RenderHelper.Effect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    game.krypton.RenderHelper.BufferDraw();
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                game.krypton.RenderHelper.ShadowHullVertices.Clear();
                game.krypton.RenderHelper.ShadowHullIndicies.Clear();

                foreach (Light2D light in game.krypton.Lights)
                {
                    game.krypton.RenderHelper.BufferAddBoundOutline(light.Bounds);
                }

                foreach (var effectPass in game.krypton.RenderHelper.Effect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    game.krypton.RenderHelper.BufferDraw();
                }
            }
        }
        

        private void DrawLevel(GameTime gameTime, SpriteBatch spriteBatch)
        {
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

            foreach (Bullet b in enemyBulletList)
            {
                b.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
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
                    DrawPrimitives.DrawRectangle(rec, game.WhitePixel, Color.Black, spriteBatch, false, 1);
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

            //Player HP
            Rectangle playerHealthRec = new Rectangle(50,650,1180,25);
            Color healthRed = new Color(200, 0, 0, 100);
            DrawPrimitives.DrawHealthBar(spriteBatch, game.WhitePixel, playerHealthRec, healthRed, true, true, player.HP, player.totalHP);
            string hpText = string.Format("{0} / {1}", player.HP, player.totalHP);
            spriteBatch.DrawString(game.font, hpText, new Vector2(playerHealthRec.Center.X, playerHealthRec.Center.Y), Color.Black);


            //player XP Level
            Rectangle playerXPRec = new Rectangle(50, 675, 1180, 25);
            Color xpPurple = new Color(100, 0, 100, 100);
            DrawPrimitives.DrawHealthBar(spriteBatch, game.WhitePixel, playerXPRec, xpPurple, true, true, player.XPRelative, player.XPNeededRelative);
            string xpText = string.Format("{0} / {1}", player.xp, player.XPNeeded);
            spriteBatch.DrawString(game.font, xpText, new Vector2(playerXPRec.Center.X, playerXPRec.Center.Y), Color.Black);

            spriteBatch.DrawString(game.font, player.xpLevel.ToString(), new Vector2(50, 675), Color.Black, 0, new Vector2(0, 0), 3, SpriteEffects.None, 1);


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
