using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoomOffline.Entity;
using BoomOffline.Event;
using BoomOffline.Helper;
using BoomOffline.Model;
using BoomOffline.Resource;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.UI
{
    class Menu: GameUI
    {
        private bool isContinueDisplay;
        //private TextEntity title;
        private BasicEntity background;
        private Button newGame;
        private Button setting;
        private Button exit;
        private Button continueGame;

        public Menu()
        {
            //title = new TextEntity();
            background = new BasicEntity();
            newGame = new Button();
            setting = new Button();
            exit = new Button();
            continueGame = new Button();
        }

        public override void Load()
        {
            MatchStorage.Instance.Load();
            isContinueDisplay = MatchStorage.Instance.DataAvailable;
            int unit = Global.Instance.Unit;
            var viewPort = Global.Instance.Graphics.Viewport;
            continueGame.Load("CONTINUE", unit,unit * 3,new GameEvent(GameEvent.Type.Continue));
            //title.Load("BOOM OFFLINE",ResManager.Instance.Font_2,new Vector2(viewPort.Width / 2, unit),Color.Yellow,true);
            background.Load(ResManager.Instance.MenuBackground, new Rectangle(0, 0, viewPort.Width, viewPort.Height), Color.White);
            newGame.Load("NEW GAME", unit,unit * 5,new GameEvent(GameEvent.Type.NewGame));
            setting.Load("SETTING", unit, unit * 7, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Setting));
            exit.Load("EXIT", unit, unit * 9, new GameEvent(GameEvent.Type.WantToExit));
            
        }

        public override void HandleEvent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (isContinueDisplay)
                continueGame.Update(gameTime);
            newGame.Update(gameTime);
            setting.Update(gameTime);
            exit.Update(gameTime);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
            //title.Draw(spriteBatch);
            if (isContinueDisplay)
                continueGame.Draw(spriteBatch);
            newGame.Draw(spriteBatch);
            setting.Draw(spriteBatch);
            exit.Draw(spriteBatch);
        }
    }
}
