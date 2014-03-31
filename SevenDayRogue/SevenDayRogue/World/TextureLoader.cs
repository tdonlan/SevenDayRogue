using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace SevenDayRogue
{
    public class TextureLoader
    {
        Game1 game;
        public Texture2D tileset;

        public TextureLoader(Game1 game, Texture2D tileset)
        {
            this.game = game;
            this.tileset = tileset;
        }

        public static Rectangle getTilesetRec(int x, int y)
        {
            return new Rectangle(x * GameConstants.TileWidth, y * GameConstants.TileHeight,GameConstants.TileWidth,GameConstants.TileHeight);
        }


     


       
    }
}
