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
    public class ContentLoader
    {
        public ContentManager content;

        public Dictionary<string, Texture2D> textureDictionary = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> spriteDictionary = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> menuDictionary = new Dictionary<string, Texture2D>();

        public Dictionary<string, SpriteFont> fontDictionary = new Dictionary<string, SpriteFont>();



        public ContentLoader(ContentManager content)
        {
            this.content = content;

            LoadTextures();
            LoadSprites();
            LoadFonts();
            LoadMenu();
          

        }

        private void LoadTextures()
        {
            textureDictionary.Add("TileGray",content.Load<Texture2D>("Textures/TilesetGray2"));
        }

        private void LoadSprites()
        {
            spriteDictionary.Add("EnemyGuard", content.Load<Texture2D>("Sprites/Guard2"));
            spriteDictionary.Add("Player", content.Load<Texture2D>("Sprites/player"));
            spriteDictionary.Add("Nanite", content.Load<Texture2D>("Sprites/nanite"));
            spriteDictionary.Add("WhitePixel", content.Load<Texture2D>("Test/WhitePixel"));
        }

        private void LoadFonts()
        {
            fontDictionary.Add("FontSmall", content.Load<SpriteFont>("Fonts/font1"));
            fontDictionary.Add("FontMed", content.Load<SpriteFont>("Fonts/fontMed"));
        }

        private void LoadSounds()
        { }

        private void LoadMenu()
        {
           menuDictionary.Add("AgilityIcon", content.Load<Texture2D>("Menu/AgilIcon"));
           menuDictionary.Add("AttuneIcon", content.Load<Texture2D>("Menu/AttuneIcon"));
           menuDictionary.Add("StrengthIcon", content.Load<Texture2D>("Menu/StrengthIcon"));

           menuDictionary.Add("Gun1", content.Load<Texture2D>("Menu/Gun1"));
           menuDictionary.Add("Gun2", content.Load<Texture2D>("Menu/Gun2"));
           menuDictionary.Add("Knife1", content.Load<Texture2D>("Menu/Knife1"));
           menuDictionary.Add("Shotgun1", content.Load<Texture2D>("Menu/Shotgun1"));

           menuDictionary.Add("RogueMenu", content.Load<Texture2D>("Menu/RogueMenu"));
        }


        public SpriteFont getFont(string name)
        {
            if (fontDictionary.ContainsKey(name))
            {
                return fontDictionary[name];
            }
            else
            {
                return null;
            }
        }


        public Texture2D getTexture(string name)
        {
            if (textureDictionary.ContainsKey(name))
            {
                return textureDictionary[name];
            }
            else
            {
                return null;
            }
        }

        public Texture2D getSprite(string name)
        {
            if (spriteDictionary.ContainsKey(name))
            {
                return spriteDictionary[name];
            }
            else
            {
                return null;
            }
        }

        public Texture2D getMenu(string name)
        {
            if (menuDictionary.ContainsKey(name))
            {
                return menuDictionary[name];
            }
            else
            {
                return null;
            }
        }


    }
}
