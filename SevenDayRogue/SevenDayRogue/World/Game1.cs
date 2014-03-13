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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Texture2D WhitePixel;
        public Texture2D wallTexture;
        public Texture2D floorTexture;
        public Texture2D playerTexture;
        public Texture2D guardTexture;

        public GameInput gameInput;

        public SpriteFont font;

        public Level currentLevel;
        public List<Tile[,]> levelTileList; //list of tilesets for all levels
        public int levelIndex;

        public Random r = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";

            gameInput = new GameInput(this);

            levelTileList = new List<Tile[,]>();

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            WhitePixel = Content.Load<Texture2D>("WhitePixel") ;
            wallTexture = Content.Load<Texture2D>("wallTile3");
            floorTexture = Content.Load<Texture2D>("floorTile3");
            font = Content.Load<SpriteFont>("font1");

            playerTexture = Content.Load<Texture2D>("player");
            guardTexture = Content.Load<Texture2D>("Guard1");

            levelIndex = 1;
            currentLevel = new Level(this);
            levelTileList.Add(currentLevel.tileArray);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            gameInput.getInput(gameTime);
            // TODO: Add your update logic here
            currentLevel.Update(gameTime);

            base.Update(gameTime);
        }

        public void LevelUp()
        {
            levelIndex++;
            if (levelTileList.Count < levelIndex)
            {
                currentLevel = new Level(this);
                levelTileList.Add(currentLevel.tileArray);
            }
            else
            {
                currentLevel = new Level(this, levelTileList[levelIndex - 1], true);
            }
        }

        public void LevelDown()
        {
            if (levelIndex > 1)
            {
                levelIndex--;
                currentLevel = new Level(this, levelTileList[levelIndex - 1], false);
            }
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //spriteBatch.Begin();

            currentLevel.Draw(gameTime, spriteBatch);

            //spriteBatch.End();
                

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
