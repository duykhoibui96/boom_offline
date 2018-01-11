using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using BoomOffline.Entity;
using BoomOffline.Resource;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.UI
{
    class Setting : GameUI
    {
        private BasicEntity soundControl;
        private BasicEntity musicControl;

        public Setting()
        {
            soundControl = new BasicEntity();
            musicControl = new BasicEntity();
        }

        public override void Load()
        {
            soundControl.Load(ResManager.Instance.Control, new Rectangle(200, 100, 50, 50), Color.White);
            musicControl.Load(ResManager.Instance.Control, new Rectangle(200, 200, 50, 50), Color.White);
        }

        public override void HandleEvent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            soundControl.Draw(spriteBatch);
            musicControl.Draw(spriteBatch);
        }
    }
}
