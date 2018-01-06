using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BoomOffline.Input
{
    class MouseEvent
    {
        private static MouseEvent instance;
        private MouseState previousState;
        private MouseState currentMouseState;

        public bool IsHover(Rectangle rect)
        {
            return rect.Contains(currentMouseState.X, currentMouseState.Y);
        }

        public bool IsLeftClick
        {
            get
            {
                return previousState.LeftButton == ButtonState.Pressed &&
                       currentMouseState.LeftButton == ButtonState.Released;
            }
        }

        public static MouseEvent Instance
        {
            get
            {
                if (instance == null)
                    instance = new MouseEvent();
                return instance;
            }
        }

        public void Update()
        {
            previousState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        private MouseEvent()
        {

        }
    }
}
