using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoomOffline.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.UI
{
    abstract class GameUI: IModel
    {
        public enum ViewType
        {
            Menu = 0,
            Room = 1,
            Match = 2,
            Setting = 3,
            Loading = 4,
            Result = 5,
        }
        public abstract void Load();
        public abstract void HandleEvent();

        public virtual void Update(GameTime gameTime)
        {
            HandleEvent();
        }

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
