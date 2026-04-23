using System.Data;
using System.Reflection.PortableExecutable;
using Microsoft.Xna.Framework.Input;
using TheCure.Managers;

namespace TheCure
{
    public class InputManager : Manager<InputManager>
    {
        public KeyboardState LastKeyboardState { get; private set; }
        public KeyboardState CurrentKeyboardState { get; private set; }
        public MouseState LastMouseState { get; private set; }
        public MouseState CurrentMouseState { get; private set; }

        public void Load()
        {
            LastKeyboardState = Keyboard.GetState();
            CurrentKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();
            LastMouseState = Mouse.GetState();
        }

        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return CurrentKeyboardState.IsKeyUp(key);
        }

        public bool IsKeyPress(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyUp(key);
        }

        public bool LeftMousePress()
        {
            return CurrentMouseState.LeftButton == ButtonState.Pressed &&
                   LastMouseState.LeftButton == ButtonState.Released;
        }

        public bool RightMousePress()
        {
            return CurrentMouseState.RightButton == ButtonState.Pressed &&
                   LastMouseState.RightButton == ButtonState.Released;
        }
    }
}