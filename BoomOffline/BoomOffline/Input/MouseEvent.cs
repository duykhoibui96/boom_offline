using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoomOffline.Input
{
    class MouseEvent
    {
        private static MouseEvent instance;
        public MouseState previousState;
        public MouseState currentMouseState;

        private Rectangle mouseRect;

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

            mouseRect.X = currentMouseState.X;
            mouseRect.Y = currentMouseState.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Resource.ResManager.Instance.Cursor, mouseRect, Color.White);
        }

        private MouseEvent()
        {
            mouseRect = new Rectangle(0, 0, 50, 50);
        }
    }
}
