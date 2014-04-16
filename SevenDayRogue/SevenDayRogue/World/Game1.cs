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
