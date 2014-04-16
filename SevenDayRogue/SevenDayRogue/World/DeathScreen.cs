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
    public class DeathScreen
    {

        public Game1 game;

        public TimeSpan deathTimer;
        public float deathTime;

        private bool canRestart = false;

        Color bgColor;


        public DeathScreen(Game1 game)
        {
            this.game = game;
            this.deathTimer = TimeSpan.FromSeconds(deathTime);

            bgColor = Color.Transparent;
        }

        public void Update(GameTime gameTime)
        {
            deathTimer -= gameTime.ElapsedGameTime;
            if (deathTimer < TimeSpan.Zero)
            {
                canRestart = true;
            }

            float ratio = (float)((deathTime - deathTimer.TotalSeconds) / deathTime);

            bgColor = Color.Lerp(bgColor, Color.Black, ratio);

            if (canRestart)
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            if (game.gameInput.IsNewMouseButtonPress(MouseButton.LeftButton))
            {
                game.Restart();
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            Rectangle screenRec = new Rectangle(0, 0, 1280, 720);
            DrawPrimitives.DrawRectangle(screenRec, game.gameContent.getSprite("WhitePixel"), bgColor, spriteBatch, true, 1);

            Vector2 center = new Vector2(screenRec.Center.X,screenRec.Center.Y);

            spriteBatch.DrawString(game.gameContent.getFont("FontMed"), "You Died", center, Color.Red, 0f, new Vector2(0, 0), 5, SpriteEffects.None, 1);

            if (canRestart)
            {
                spriteBatch.DrawString(game.gameContent.getFont("FontMed"), "Press any key to restart", center + new Vector2(0, 100), Color.White, 0f, new Vector2(0, 0), 2, SpriteEffects.None, 1);
            }

            spriteBatch.End();
        }
       
    }
}
