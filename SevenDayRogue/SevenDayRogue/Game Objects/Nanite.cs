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
    public class Nanite
    {
        public Level level;
        public Vector2 position;
        public Vector2 origin;
        public int amt;

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)position.X - 13, (int)position.Y - 13, 25, 25);
            }
        }

        public Nanite(Level level, Vector2 pos, int amt)
        {
            this.level = level;
            this.position = pos;
            this.amt = amt;

            
        }

        public void Update(GameTime gameTime)
        {
            if (level.player.BoundingRectangle.Intersects(BoundingRectangle))
            {
                level.player.GetNanites(amt);
                level.DespawnNanite(this);

            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(level.game.gameContent.getSprite("Nanite"), position, Color.White);
        }

    }
}
