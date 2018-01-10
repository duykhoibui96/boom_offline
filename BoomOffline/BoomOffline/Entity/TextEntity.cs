﻿using System;
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

        public Color TextColor
        {
            set { color = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public void Load(string text, SpriteFont font, Vector2 vector, Color color, bool startFromMiddle = false)
        {
            this.Text = text;
            this.font = font;
            this.color = color;

            if (startFromMiddle)
            {
                var textSize = font.MeasureString(text);
                vector.X -= textSize.X / 2;
            }
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
