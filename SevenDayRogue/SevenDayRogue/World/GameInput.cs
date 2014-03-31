using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


//class that contains both the most  keyboard input (can be used to record fighting game style combos), 
//and the mappings of keys to game "buttons" - abstract away the keyboard vs gamepad
namespace SevenDayRogue
{
    public class GameInput
    {
        public Game1 game;

        public PlayerIndex gamePlayerIndex { get; set; }
        public string playerName;


        public GamePadState currentGamePadState;
        private const float MoveStickScale = 1.0f;


        public KeyboardState currentKeyboardState;

        KeyboardState LastKeyboardStates;
        GamePadState LastGamePadStates;

        public MouseState currentMouseState;
        MouseState LastMouseState;

        public Vector2 mousePos;

        public float HMovement;
        public float VMovement;


        public GameInput(Game1 game)
        {
            this.game = game;
          
        }

        //Standard Controls
        public void MapButtons1()
        {
         
        }


        public void getInput(GameTime GT)
        {

            LastKeyboardStates = currentKeyboardState;
            LastGamePadStates = currentGamePadState;
            LastMouseState = currentMouseState;

            currentGamePadState = GamePad.GetState(gamePlayerIndex);
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            HMovement = currentGamePadState.ThumbSticks.Left.X * MoveStickScale;


            VMovement = currentGamePadState.ThumbSticks.Left.Y * -MoveStickScale;

            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S))
                VMovement = 1.0f;

            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W))
                VMovement = -1.0f;

            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A))
                HMovement = -1.0f;

            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D))
                HMovement = 1.0f;

            mousePos = new Vector2(currentMouseState.X, currentMouseState.Y);

        }



        public void updateButton(ButtonMap butt, GameTime GT)
        {
            if (IsNewButtonPress(butt.B) || IsNewKeyPress(butt.K) )
            {
                butt.NewPress(GT);
            }
            else if (currentGamePadState.IsButtonDown(butt.B) || currentKeyboardState.IsKeyDown(butt.K) )
            {
                butt.Press(GT);
            }
            else
            {
                butt.Release(GT);
            }
        }

        public bool IsNewKeyPress(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key) &&
                        LastKeyboardStates.IsKeyUp(key));
        }

        public bool IsNewButtonPress(Buttons button)
        {
            return (currentGamePadState.IsButtonDown(button) &&
                    LastGamePadStates.IsButtonUp(button));

        }

        public bool IsNewMouseButtonPress(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.None:
                    return false;
                case MouseButton.LeftButton:
                    return (currentMouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released);
                case MouseButton.RightButton:
                    return (currentMouseState.RightButton == ButtonState.Pressed && LastMouseState.RightButton == ButtonState.Released);
                default:
                    return false;

            }
        }

        public bool isMouseButtonPress(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.None:
                    return false;
                case MouseButton.LeftButton:
                   return (currentMouseState.LeftButton == ButtonState.Pressed);

                case MouseButton.RightButton:
                      return (currentMouseState.RightButton == ButtonState.Pressed);
                default:
                    return false;

            }
        }



    }

    //stored what a game button is mapped to, if it is currently pressed, and the gametime in ticks that it was last pressed.
    public class ButtonMap
    {
        string name;
        public Keys K;
        public Buttons B;
   


        public bool isPressed;
        public bool isNewPress;

        TimeSpan timePressed;

        public ButtonMap(string name, Keys k, Buttons b)
        {
            this.name = name;
            K = k;
            B = b;
          
        }



        public void NewPress(GameTime GT)
        {
            isNewPress = true;
            isPressed = true;
            timePressed = GT.ElapsedGameTime;
        }

        public void Press(GameTime GT)
        {
            isPressed = true;
            isNewPress = false;
            timePressed += GT.ElapsedGameTime;

        }

        public void Release(GameTime gt)
        {
            isNewPress = false;
            isPressed = false;
            timePressed = TimeSpan.Zero;
        }


    }
}
