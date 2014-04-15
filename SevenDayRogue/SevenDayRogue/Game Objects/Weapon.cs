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
    public class Weapon
    {
        public Level level;
        public Vector2 position;

        public Texture2D texture;

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            }
        }


        //spawns, swings, then despawns
        public Weapon(Level level)
        {

        }
             
             
    }
}
