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

        public Level(Game1 game)
        {
            this.game = game;
           
            
           
            LoadCave();
            //LoadMaze();
            //LoadBerryDungeon();

            this.player = new Player(this, new Vector2(1000, 1000));

            LoadContent();
        }

        private void LoadContent()
        {
       
        }

        private void LoadLevel()
        {
            tileArray = new Tile[50, 50];
            for (int i = 0; i < tileArray.GetLength(0); i++)
            {
                for (int j = 0; j < tileArray.GetLength(1); j++)
                {
                    bool isSolid = false;
                    if (i == 0 || i == tileArray.GetLength(0) - 1 || j == 0 || j == tileArray.GetLength(1) - 1)
                        isSolid = true;

                    if (i % 4 == 0 && j % 4 == 0)
                        isSolid = true;

                    tileArray[i, j] = new Tile(isSolid, TileType.Stone);
                }
            }
        }

        private void LoadCave()
        {
            int height = 100;
            int width = 100;
            CellAutoCave CACave = new CellAutoCave(width, height);

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

                        if (CACave.grid[i, j] == 1)
                        {
                            isSolid = true;
                        }
                       
                    }


                    tileArray[i, j] = new Tile(isSolid, TileType.Stone);
                }
            }

        }



        private void LoadMaze()
        {
            int height = 100;
            int width = 100;

          
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

                    }

                    tileArray[i, j] = new Tile(isSolid, TileType.Stone);
                }
            }

        }


        private void LoadBerryDungeon()
        {
         
            Generation berryGen = new Generation();
            berryGen.Generate(100, 100, null);

            tileArray = new Tile[berryGen.Width, berryGen.Height];

            for (int i = 0; i < berryGen.Height; i++)
            {
                for (int j = 0; j < berryGen.Width; j++)
                {
                    bool isSolid = false;
                    if (i == 0 || i == tileArray.GetLength(0) - 1 || j == tileArray.GetLength(1) - 1)
                    {
                        isSolid = true;
                    }
                    else
                    {

                        if (berryGen.Map[i, j] == Generation.Type.Wall || berryGen.Map[i, j] == Generation.Type.Stone)
                        {
                            isSolid = true;
                        }

                    }

                    tileArray[i, j] = new Tile(isSolid, TileType.Stone);
                }
            }

        }

        public void Update(GameTime gameTime)
        {
            player.Update(gameTime);
          
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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color transBlack = Color.Lerp(Color.Black, Color.Transparent, .5f);
            ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
            cameraTransform = Matrix.CreateTranslation(-cameraPosition, -cameraPositionYAxis, 0.0f);

            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);

            DrawTiles(spriteBatch);
            player.Draw(gameTime, spriteBatch);

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
                DrawPrimitives.DrawRectangle(rec, game.WhitePixel, Color.Red, spriteBatch, true, 1);
                //spriteBatch.Draw(game.wallTexture, rec, Color.White);
            }
            else
            {
                DrawPrimitives.DrawRectangle(rec, game.WhitePixel, Color.White, spriteBatch, true, 1);
                //spriteBatch.Draw(game.floorTexture, rec, Color.White);
            }
          
        }


        private void DrawHUD(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            spriteBatch.Begin();

            Color transBlack = Color.Lerp(Color.Black, Color.Transparent, .1f);


            Vector2 hudPOS = new Vector2(0,600);
            Rectangle HUDRec = new Rectangle((int)hudPOS.X,(int)hudPOS.Y,1280,200);
            DrawPrimitives.DrawRectangle(HUDRec, game.WhitePixel, transBlack, spriteBatch, true, 1);
            


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
