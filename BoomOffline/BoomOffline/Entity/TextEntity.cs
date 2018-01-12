using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoomOffline.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.Entity
{
    class TextEntity : IModel
    {
        private string text;
        private SpriteFont font;
        private Vector2 vector;
        private Color color;
        private Rectangle rect;

        public Color TextColor
        {
            set { color = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }

        public void Load(string text, SpriteFont font, Vector2 vector, Color color, bool startFromMiddle = false)
        {
            this.Text = text;
            this.font = font;
            this.color = color;

            var textSize = font.MeasureString(text);
            if (startFromMiddle)
            {
                vector.X -= textSize.X / 2;
            }
            Rect = new Rectangle((int) vector.X, (int) vector.Y, (int) textSize.X, (int) textSize.Y);
            this.vector = vector;
        }

        public void Offset(int x, int y)
        {
            vector.X += x;
            vector.Y += y;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, Text, vector, color);
        }
    }
}
