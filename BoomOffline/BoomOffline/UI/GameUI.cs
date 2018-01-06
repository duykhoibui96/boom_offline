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
        public abstract void Load();
        public abstract void HandleEvent();

        public virtual void Update(GameTime gameTime)
        {
            HandleEvent();
        }

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
