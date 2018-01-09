using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoomOffline.Event;
using BoomOffline.Input;
using BoomOffline.Model;
using BoomOffline.Resource;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoomOffline.Entity
{
    class Button : IModel
    {

        private BasicEntity container;
        private TextEntity text;
        private Texture2D light;
        private GameEvent ev;

        public void Load(string text, int x, int y, GameEvent ev)
        {
            int unit = Global.Instance.Unit;
            int width = unit*5;
            int height = unit;
            container = new BasicEntity();
            container.Load(ResManager.Instance.ButtonContainer, new Rectangle(x, y, width, height), Color.White);
            this.text = new TextEntity();
            this.text.Load(text, ResManager.Instance.Font_1, new Vector2(x + width / 2, y + 10), Color.Yellow, true);
            this.ev = ev;
        }

        public void Update(GameTime gameTime)
        {
            bool isHover = MouseEvent.Instance.IsHover(container.Rect);

            if (isHover)
            {
                text.TextColor = Color.White;
                if (MouseEvent.Instance.IsLeftClick)
                {
                    EventQueue.Instance.AddEvent(ev);
                }

            }
            else
            {
                text.TextColor = Color.Yellow;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            container.Draw(spriteBatch);
            text.Draw(spriteBatch);
        }
    }
}
