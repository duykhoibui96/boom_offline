using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BoomOffline.Entity;
using BoomOffline.Event;
using BoomOffline.Helper;
using BoomOffline.Resource;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.UI
{
    class Loading: GameUI
    {
         private BasicEntity background;
        private TextEntity loadingTitle;
        private BackgroundWorker worker;

        public Loading()
        {
            background = new BasicEntity();
            loadingTitle = new TextEntity();
            worker = new BackgroundWorker();
        }
        public override void Load()
        {
            var viewPort = Global.Instance.Graphics.Viewport;
            background.Load(ResManager.Instance.LoadingBackground, new Rectangle(0, 0, viewPort.Width, viewPort.Height), Color.White);
            loadingTitle.Load("Loading...", ResManager.Instance.Font_2, new Vector2(25, 25), Color.White);
            worker.DoWork+= new DoWorkEventHandler(myWorker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(myWorker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        public override void HandleEvent()
        {

        }

        protected void myWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var mapGenerator = MapGenerator.Instance;
            mapGenerator.GenerateMap();
            System.Threading.Thread.Sleep(1000);
        }

        protected void myWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EventQueue.Instance.AddEvent(new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Match));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
            loadingTitle.Draw(spriteBatch);
        }
    }
}
