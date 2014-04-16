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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public KryptonEngine krypton;
        public Texture2D mLightTexture;

        public ContentLoader gameContent;
     
        /*

        public Texture2D WhitePixel;
        public Texture2D wallTexture;
        public Texture2D floorTexture;
        public Texture2D playerTexture;
        public Texture2D guardTexture;
        public Texture2D tileset;
        public Texture2D tileset2;
        public Texture2D tileset3;
        public Texture2D nanite;
         * */

        public GameInput gameInput;

        //public SpriteFont font;
        //public SpriteFont fontMed;

        public Level currentLevel;
        public List<Tile[,]> levelTileList; //list of tilesets for all levels
        public int levelIndex;

        public Random r = new Random();

        private bool isDead = false;
        public DeathScreen deathScreen = null;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";

            gameInput = new GameInput(this);

            levelTileList = new List<Tile[,]>();

            this.krypton = new KryptonEngine(this, "Effects/KryptonEffect");

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
            this.krypton.Initialize();

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

            gameContent = new ContentLoader(Content);

            // TODO: use this.Content to load your game content here
            /*
            WhitePixel = Content.Load<Texture2D>("Test/WhitePixel") ;
            wallTexture = Content.Load<Texture2D>("Textures/wallTile3");
            floorTexture = Content.Load<Texture2D>("Textures/floorTileB");
            tileset = Content.Load<Texture2D>("Textures/TilesetGray");
            tileset3 = Content.Load<Texture2D>("Textures/TilesetGray2");
            tileset2 = Content.Load<Texture2D>("Textures/BioTile");
            nanite = Content.Load<Texture2D>("Sprites/Nanite");

            font = Content.Load<SpriteFont>("Fonts/font1");
            fontMed = Content.Load<SpriteFont>("Fonts/fontMed");

            playerTexture = Content.Load<Texture2D>("Sprites/player");
            guardTexture = Content.Load<Texture2D>("Sprites/Guard2");
             * */

        
            this.mLightTexture = LightTextureBuilder.CreatePointLight(this.GraphicsDevice, 512);

            Restart();

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
            gameInput.getInput(gameTime);

           
            if (isDead)
            {
                deathScreen.Update(gameTime);
            }
            else
            {
                currentLevel.Update(gameTime);
            }

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

        public void Die()
        {
            isDead = true;
            deathScreen = new DeathScreen(this);
        }

        public void Restart()
        {
            isDead = false;
            levelIndex = 1;
            currentLevel = new Level(this);
            levelTileList.Add(currentLevel.tileArray);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
         
            currentLevel.Draw(gameTime, spriteBatch);

            if (isDead)
            {
                deathScreen.Draw(gameTime, spriteBatch);
            }

            base.Draw(gameTime);
        }
    }
}
