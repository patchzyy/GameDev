using Microsoft.Xna.Framework.Input;

namespace TheCure
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    public class InputBinding
    {
        public Keys? Key { get; private set; }
        public MouseButton? Mouse { get; private set; }

        private InputBinding() { }

        public static InputBinding FromKey(Keys key)
            => new InputBinding { Key = key };

        public static InputBinding FromMouse(MouseButton mouse)
            => new InputBinding { Mouse = mouse };

        public bool IsKeyboard => Key.HasValue;
        public bool IsMouse => Mouse.HasValue;

        public string ToDisplayString()
        {
            if (IsMouse)
                return Mouse switch
                {
                    MouseButton.Left => "M1",
                    MouseButton.Right => "M2",
                    MouseButton.Middle => "M3",
                    _ => "M?"
                };

            var tmp = Key.Value.ToString();
            return tmp.StartsWith("D") ? tmp[1..] : tmp;
        }
    }
}
