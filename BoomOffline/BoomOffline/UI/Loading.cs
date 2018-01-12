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
            if (MatchStorage.Instance.NeedToLoadDataHere)
            {
                var room = RoomSetting.Instance;
                var mapData = MatchStorage.Instance.MapData;

                room.MapName = mapData[0];
                room.MapSize = Int32.Parse(mapData[1]);
                room.PlayerType = Int32.Parse(mapData[2]);
                room.NumOfBot = Int32.Parse(mapData[3]);
                if (mapData[4] != "No poscam")
                {
                    var data = mapData[4].Split(' ').Select(curData => Double.Parse(curData)).ToArray();
                    room.PosCam = new Vector2((float) data[0],(float) data[1]);
                }
                if (mapData[5] != "No plus poscam")
                {
                    var data = mapData[5].Split(' ').Select(curData => Double.Parse(curData)).ToArray();
                    room.PlusPosCam = new Vector2((float)data[0], (float)data[1]);
                }

            }
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
