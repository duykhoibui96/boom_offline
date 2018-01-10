using System;
using System.Collections.Generic;
using System.Linq;
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
        private Button returnToMenu;

        private SpriteBatch spriteBatch;

        private Camera camera;

        public Match()
        {
            gameOperator = new GameOperator();
            returnToMenu = new Button();
            spriteBatch = new SpriteBatch(Global.Instance.Graphics);
        }

        public override void Load()
        {
            camera = new Camera();
            Global.Instance.currentCamera = camera;
            //var viewPort = Global.Instance.Graphics.Viewport;
            var unit = Global.Instance.Unit;
            gameOperator.Init();
            //returnToMenu.Load("RETURN TO MENU", viewPort.Width - unit * 5, unit, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Menu));
            returnToMenu.Load("RETURN TO MENU", 20, 20, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Menu));
        }

        public override void HandleEvent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            camera.Update(gameTime);
            gameOperator.Update(gameTime);
            returnToMenu.Update(gameTime);
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
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
            returnToMenu.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
