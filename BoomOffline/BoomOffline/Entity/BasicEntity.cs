using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoomOffline.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.Entity
{
    class BasicEntity : IModel
    {
        private Texture2D texture;
        private Rectangle rect;
        private Color color;

        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }


        public void Load(Texture2D texture, Rectangle rect, Color color)
        {
            this.texture = texture;
            this.rect = rect;
            this.color = color;
        }
        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rect, color);
        }
    }
}
