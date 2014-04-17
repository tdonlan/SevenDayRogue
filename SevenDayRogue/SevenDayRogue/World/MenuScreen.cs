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
    public class MenuScreen
    {

        public Game1 game;
        
        public MenuScreen(Game1 game)
        {
            this.game = game;

        }

        public void Update(GameTime gameTime)
        {
            //if we escape, close menu

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(game.gameContent.getMenu("RogueMenu"), new Vector2(0, 0), Color.White);

            //player level
            spriteBatch.DrawString(game.gameContent.getFont("FontMed"), game.currentLevel.player.xpLevel.ToString(), new Vector2(150, 150), Color.White);


            //player HP bars
            Rectangle playerHealthRec = new Rectangle(240, 125, 900, 25);
            Color healthRed = new Color(200, 0, 0, 100);
            DrawPrimitives.DrawHealthBar(spriteBatch, game.gameContent.getSprite("WhitePixel"), playerHealthRec, healthRed, true, true, game.currentLevel.player.HP, game.currentLevel.player.totalHP);
            string hpText = string.Format("{0} / {1}", game.currentLevel.player.HP, game.currentLevel.player.totalHP);
            spriteBatch.DrawString(game.gameContent.getFont("FontMed"), hpText, new Vector2(playerHealthRec.Center.X, playerHealthRec.Center.Y), Color.White);

            Rectangle playerXPRec = new Rectangle(240, 155, 900, 25);
            Color xpPurple = new Color(100, 0, 100, 100);
            DrawPrimitives.DrawHealthBar(spriteBatch, game.gameContent.getSprite("WhitePixel"), playerXPRec, xpPurple, true, true, game.currentLevel.player.XPRelative, game.currentLevel.player.XPNeededRelative);
            string xpText = string.Format("{0} / {1}", game.currentLevel.player.xp, game.currentLevel.player.XPNeeded);
            spriteBatch.DrawString(game.gameContent.getFont("FontMed"), xpText, new Vector2(playerXPRec.Center.X, playerXPRec.Center.Y), Color.White);

            Rectangle playerNaniteRec = new Rectangle(240, 180, 900, 25);
            Color naniteBlue = Color.LightBlue;
            DrawPrimitives.DrawHealthBar(spriteBatch, game.gameContent.getSprite("WhitePixel"), playerNaniteRec, naniteBlue, true, true, game.currentLevel.player.nanites, 1000);
            string naniteText = string.Format("{0} / {1}", game.currentLevel.player.nanites, 1000);
            spriteBatch.DrawString(game.gameContent.getFont("FontMed"), naniteText, new Vector2(playerNaniteRec.Center.X, playerNaniteRec.Center.Y), Color.White);


            //player skill points


            //player skill icons

            //player equipped weapons

            //player inventory


            spriteBatch.End();
        }

        //if you hover over an icon
        private void DrawWeaponPopup(SpriteBatch spriteBatch)
        {

        }
             
    }
}
