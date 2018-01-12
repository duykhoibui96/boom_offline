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

            playAgain.Load("PLAY AGAIN", (viewPort.Width - unit * 5) / 2, unit * 3, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Match));
            returnRoom.Load("RETURN ROOM", (viewPort.Width - unit * 5) / 2, unit * 4 + unit / 2, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Room));
            mainMenu.Load("MAIN MENU", (viewPort.Width - unit * 5) / 2, unit * 6, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Menu));

            string text = "";

            if (isWin)
            {
                SoundManager.Instance.PlaySound(SoundManager.SoundType.Win);
                if (RoomSetting.Instance.MultiplayerMode)
                {
                    int player = GameResult.Instance.WinnerPlayer;

                    if (player == 1)
                    {
                        text = "PLAYER 1 WIN";
                    }
                    else
                    {
                        text = "PLAYER 2 WIN";
                    }
                }
                else
                {
                    text = "YOU WIN";
                }
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundManager.SoundType.Lose);
                text = "GAME OVER";
            }

            notification.Load(text, ResManager.Instance.Font_2, new Vector2(viewPort.Width / 2, unit), Color.Yellow, true);
        
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
