using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoomOffline.Entity;
using BoomOffline.Event;
using BoomOffline.Resource;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.UI
{
    class ConfirmationDialog : GameUI
    {
        private string notificationText;
        private BasicEntity dialog;
        private TextEntity notification;
        private Button yes;
        private Button no;

        public ConfirmationDialog(string notificationText)
        {
            this.notificationText = notificationText;
            dialog = new BasicEntity();
            notification = new TextEntity();
            yes = new Button();
            no = new Button();
        }

        public override void Load()
        {
            var viewPort = Global.Instance.Graphics.Viewport;
            var unit = Global.Instance.Unit;
            var res = ResManager.Instance;
            notification.Load(notificationText, res.Font_1, new Vector2(viewPort.Width / 2, viewPort.Height / 3), Color.Red, true);
            var containerRect = notification.Rect;
            containerRect.Inflate(170, 100);
            dialog.Load(res.Dialog, containerRect, Color.White);
            var buttonPos = containerRect.Location;
            buttonPos.X += (containerRect.Width - unit * 5) / 2;
            buttonPos.Y = containerRect.Bottom - unit - unit / 2;
            var buttonYesPos = buttonPos;
            var buttonNoPos = buttonPos;
            buttonYesPos.X = containerRect.Left + unit;
            buttonNoPos.X = containerRect.Right - unit - unit*5;
            yes.Load("YES", buttonYesPos.X, buttonYesPos.Y, new GameEvent(GameEvent.Type.DismissDialog,1,"yes"));
            no.Load("NO", buttonNoPos.X, buttonNoPos.Y, new GameEvent(GameEvent.Type.DismissDialog,1,"no"));


        }

        public override void HandleEvent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            yes.Update(gameTime);
            no.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            dialog.Draw(spriteBatch);
            notification.Draw(spriteBatch);
            yes.Draw(spriteBatch);
            no.Draw(spriteBatch);
        }
    }
}
