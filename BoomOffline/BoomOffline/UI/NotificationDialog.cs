using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using BoomOffline.Entity;
using BoomOffline.Event;
using BoomOffline.Resource;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.UI
{
    class NotificationDialog : GameUI
    {
        private string notificationText;
        private BasicEntity dialog;
        private TextEntity notification;
        private Button ok;

        public NotificationDialog(string notificationText)
        {
            this.notificationText = notificationText;
            dialog = new BasicEntity();
            notification = new TextEntity();
            ok = new Button();
        }

        public override void Load()
        {
            var viewPort = Global.Instance.Graphics.Viewport;
            var unit = Global.Instance.Unit;
            var res = ResManager.Instance;
            notification.Load(notificationText, res.Font_1, new Vector2(viewPort.Width / 2, viewPort.Height / 3), Color.Red, true);
            var containerRect = notification.Rect;
            containerRect.Inflate(100,100);
            dialog.Load(res.Dialog, containerRect, Color.White);
            var buttonPos = containerRect.Location;
            buttonPos.X += (containerRect.Width - unit*5)/2;
            buttonPos.Y = containerRect.Bottom - unit - unit / 2;
            ok.Load("OK",buttonPos.X, buttonPos.Y, new GameEvent(GameEvent.Type.DismissDialog,0));
        }

        public override void HandleEvent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            ok.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            dialog.Draw(spriteBatch);
            notification.Draw(spriteBatch);
            ok.Draw(spriteBatch);
        }
    }
}
