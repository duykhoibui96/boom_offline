using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BoomOffline.Input
{
    class KeyboardEvent
    {
        private static KeyboardEvent instance;

        public static KeyboardEvent Instance
        {
            get
            {
                if (instance == null)
                    instance = new KeyboardEvent();
                return instance;
            }
        }

        private KeyboardState previousState;
        private KeyboardState currentState;

        public void Update()
        {
            previousState = currentState;
            currentState = Keyboard.GetState();
        }

        public bool IsPressed(Keys key)
        {
            return previousState.IsKeyDown(key) && currentState.IsKeyUp(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return currentState.IsKeyDown(key);
        }
    }
}
