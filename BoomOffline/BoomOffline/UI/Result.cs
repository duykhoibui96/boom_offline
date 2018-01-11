using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using BoomOffline.Entity;
using BoomOffline.Event;
using BoomOffline.Helper;
using BoomOffline.Resource;
using BoomOffline.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.UI
{
    class Result : GameUI
    {
        private Button playAgain;
        private Button returnRoom;
        private Button mainMenu;
        private TextEntity notification;


        public Result()
        {
            notification = new TextEntity();
            playAgain = new Button();
            returnRoom = new Button();
            mainMenu = new Button();
        }


        public override void Load()
        {
            var viewPort = Global.Instance.Graphics.Viewport;
            var unit = Global.Instance.Unit;
            bool isWin = GameResult.Instance.IsWin;

            notification.Load(isWin ? "YOU WIN" : "GAME OVER!", ResManager.Instance.Font_2, new Vector2(viewPort.Width / 2, unit), Color.Yellow, true);
            playAgain.Load("Play again", (viewPort.Width - unit * 5) / 2, unit * 3, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Match));
            returnRoom.Load("Return room", (viewPort.Width - unit * 5) / 2, unit * 4 + unit / 2, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Room));
            mainMenu.Load("Main menu", (viewPort.Width - unit * 5) / 2, unit * 6, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Menu));

            if (isWin)
            {
                SoundManager.Instance.PlaySound(SoundManager.SoundType.Win);
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundManager.SoundType.Lose);
            }
        
        }

        public override void HandleEvent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            playAgain.Update(gameTime);
            returnRoom.Update(gameTime);
            mainMenu.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            notification.Draw(spriteBatch);
            playAgain.Draw(spriteBatch);
            returnRoom.Draw(spriteBatch);
            mainMenu.Draw(spriteBatch);
        }
    }
}
