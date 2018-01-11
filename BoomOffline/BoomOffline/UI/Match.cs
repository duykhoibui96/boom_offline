using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using BoomOffline.Entity;
using BoomOffline.Event;
using BoomOffline.Helper;
using BoomOffline.Input;
using BoomOffline.Resource;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoomOffline.UI
{
    class Match : GameUI
    {
        private GameOperator gameOperator;

        private BasicEntity pauseScreen;
        private SpriteBatch spriteBatch;
        private Button returnToMenu;
        private Button continueGame;
        private Camera camera;
        private bool isPause;

        public Match()
        {
            gameOperator = new GameOperator();
            spriteBatch = new SpriteBatch(Global.Instance.Graphics);
            returnToMenu = new Button();
            continueGame = new Button();
            pauseScreen = new BasicEntity();
        }

        public override void Load()
        {
            var viewPort = Global.Instance.Graphics.Viewport;
            var unit = Global.Instance.Unit;
            camera = new Camera();
            Global.Instance.currentCamera = camera;
            returnToMenu.Load("Main menu", (viewPort.Width - unit * 5) / 2, unit * 9, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Menu));
            continueGame.Load("Continue", (viewPort.Width - unit * 5) / 2, unit * 10 + unit / 2, new GameEvent(GameEvent.Type.Resume));
            //var viewPort = Global.Instance.Graphics.Viewport;
            pauseScreen.Load(ResManager.Instance.PauseBackground,new Rectangle(0,0,viewPort.Width,viewPort.Height),Color.White);
            
            gameOperator.Init();
        }

        public override void HandleEvent()
        {
            var ev = EventQueue.Instance.CheckCurrentEvent();

            if (ev != null && ev.EventType == GameEvent.Type.Resume)
            {
                isPause = false;
                EventQueue.Instance.Next();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (KeyboardEvent.Instance.IsPressed(Keys.Escape))
            {
                isPause = !isPause;
            }
            camera.Update(gameTime);
            if (isPause)
            {
                returnToMenu.Update(gameTime);
                continueGame.Update(gameTime);
            }      
            else
                gameOperator.Update(gameTime);
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            if (isPause)
            {
                spriteBatch.Begin();
                pauseScreen.Draw(spriteBatch);
                continueGame.Draw(spriteBatch);
                returnToMenu.Draw(spriteBatch);
                spriteBatch.End();
            }
            else
            {
                if (RoomSetting.Instance.MapSize == 21)
                    spriteBatch.Begin();
                else
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                    null,
                    null,
                    null,
                    null,
                    camera.GetTransformation());

                var map = gameOperator.Map;
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        map[i, j].Draw(spriteBatch);
                    }
                }

                foreach (var bomb in gameOperator.Bombs)
                {
                    bomb.Draw(spriteBatch);
                }

                gameOperator.Player.Draw(spriteBatch);
                foreach (var bot in gameOperator.Bots)
                {
                    bot.Draw(spriteBatch);
                }

                spriteBatch.End();
            }

        }
    }
}
