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
        private Button setting;
        private Button save;
        private Camera camera;
        private TextEntity playerOneInstruction;
        private TextEntity playerTwoInstruction;
        private bool isPause;

        public Match()
        {
            gameOperator = new GameOperator();
            spriteBatch = new SpriteBatch(Global.Instance.Graphics);
            returnToMenu = new Button();
            continueGame = new Button();
            setting = new Button();
            save = new Button();
            pauseScreen = new BasicEntity();
            playerOneInstruction = new TextEntity();
            playerTwoInstruction = new TextEntity();
        }

        public override void Load()
        {
            var viewPort = Global.Instance.Graphics.Viewport;
            var unit = Global.Instance.Unit;
            camera = new Camera();
            if (MatchStorage.Instance.NeedToLoadDataHere && MatchStorage.Instance.CameraData != null)
            {
                var cameraData = MatchStorage.Instance.CameraData;
                var data = cameraData.Split(' ').Select(curData => Double.Parse(curData)).ToArray();
                camera._pos = new Vector2((float) data[0],(float) data[1]);
            }
            Global.Instance.currentCamera = camera;
            returnToMenu.Load("MAIN MENU", (viewPort.Width - unit * 5) / 2, unit * 9, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Menu));
            continueGame.Load("CONTINUE", (viewPort.Width - unit * 5) / 2, unit * 5, new GameEvent(GameEvent.Type.Resume));
            //var viewPort = Global.Instance.Graphics.Viewport;
            pauseScreen.Load(ResManager.Instance.PauseBackground,new Rectangle(0,0,viewPort.Width,viewPort.Height),Color.White);
            setting.Load("SETTING", (viewPort.Width - unit * 5) / 2, unit * 7, new GameEvent(GameEvent.Type.SwitchView,(int)GameUI.ViewType.Setting));
            save.Load("SAVE", (viewPort.Width - unit * 5) / 2, unit * 3, new GameEvent(GameEvent.Type.Save));
            playerOneInstruction.Load("Player 2:\nW,S,A,D + LShift",ResManager.Instance.Font_1,new Vector2(10,200),Color.Red);
            playerTwoInstruction.Load("Player 1:\nArrows + RShift", ResManager.Instance.Font_1, new Vector2(viewPort.Width - 185, 200), Color.Red);
            gameOperator.Init();
        }

        public override void HandleEvent()
        {
            var ev = EventQueue.Instance.CheckCurrentEvent();

            if (ev != null)
            {
                if (ev.EventType == GameEvent.Type.Resume)
                {
                    isPause = false;
                    EventQueue.Instance.Next();
                }
                else if (ev.EventType == GameEvent.Type.Save)
                {
                    MatchStorage.Instance.Save(gameOperator.Player, gameOperator.Bots, gameOperator.Bombs);
                    EventQueue.Instance.Next();
                    EventQueue.Instance.AddEvent(new GameEvent(GameEvent.Type.OpenDialog, 0, "You saved the match!"));
                }
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
                setting.Update(gameTime);
                if (!gameOperator.IsMultiplayer)
                    save.Update(gameTime);
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
                setting.Draw(spriteBatch);
                if (!gameOperator.IsMultiplayer)
                    save.Draw(spriteBatch);
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

                if (gameOperator.IsMultiplayer)
                {
                    gameOperator.Player2.Draw(spriteBatch);
                }
                else
                {
                    foreach (var bot in gameOperator.Bots)
                    {
                        bot.Draw(spriteBatch);
                    }
                }
                
                spriteBatch.End();

                if (gameOperator.MiniMap.IsEnabled)
                { 
                    spriteBatch.Begin();
                    gameOperator.MiniMap.Draw(spriteBatch);
                    spriteBatch.End();
                }

                if (gameOperator.IsMultiplayer)
                {
                    spriteBatch.Begin();
                    playerOneInstruction.Draw(spriteBatch);
                    playerTwoInstruction.Draw(spriteBatch);
                    spriteBatch.End();
                }
                
            }

            
         

        }
    }
}
