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

namespace BoomOffline.UI
{
    class Room : GameUI
    {
        private class MapInfo
        {
            private string mapName;
            private Texture2D mapTexture;

            public string MapName
            {
                get { return mapName; }
                set { mapName = value; }
            }

            public Texture2D MapTexture
            {
                get { return mapTexture; }
                set { mapTexture = value; }
            }

            public MapInfo(string mapName, Texture2D mapTexture)
            {
                this.mapName = mapName;
                this.mapTexture = mapTexture;
            }
        }

        private MapInfo[] mapInfo;
        private Character[] playerInfo;

        private BasicEntity background;

        private TextEntity playerTypeTitle;
        private Character player;
        private Button fight;
        private Button returnToMenu;

        private TextEntity numOfBotsTitle;
        private TextEntity numOfBotsEntity;
        private int numOfBots;
        private BasicEntity decreaseNumOfBots;
        private BasicEntity increaseNumOfBots;

        private BasicEntity map;
        private BasicEntity previousMap;
        private BasicEntity nextMap;

        private BasicEntity previousPlayer;
        private BasicEntity nextPlayer;

        private int selectedMap;
        private int selectedPlayer;

        public Room()
        {
            var res = ResManager.Instance;

            playerInfo = new Character[]
            {
                new Character(), 
                new Character(), 
                new Character(), 
                new Character()
            };

            background = new BasicEntity();
            
            playerTypeTitle = new TextEntity();
            player = new Character();
            fight = new Button();
            returnToMenu = new Button();

            numOfBotsTitle = new TextEntity();
            numOfBotsEntity = new TextEntity();
            decreaseNumOfBots = new BasicEntity();
            increaseNumOfBots = new BasicEntity();

            map = new BasicEntity();

            previousPlayer = new BasicEntity();
            nextPlayer = new BasicEntity();

            previousMap = new BasicEntity();
            nextMap = new BasicEntity();

        }

        public override void Load()
        {
            var unit = Global.Instance.Unit;
            var viewPort = Global.Instance.Graphics.Viewport;
            var res = ResManager.Instance;
            var roomSetting = RoomSetting.Instance;

            mapInfo = new MapInfo[]
            {
                new MapInfo("map01.txt", res.Map_1),
                new MapInfo("map02.txt", res.Map_2),
                new MapInfo("map03.txt", res.Map_3),
                new MapInfo("map04.txt", res.Map_4)
            };

            int leftSectionX = unit;
            int leftSectionY = unit;

            background.Load(res.RoomBackground,new Rectangle(0,0,viewPort.Width,viewPort.Height),Color.White);

            selectedPlayer = roomSetting.PlayerType;

            playerTypeTitle.Load("Player type", res.Font_1, new Vector2(leftSectionX, leftSectionY + unit / 2), Color.Yellow);
            playerInfo[0].Load(0, new Rectangle(leftSectionX + unit * 5, leftSectionY, unit * 2, unit * 2), 0, 0);
            playerInfo[1].Load(1, new Rectangle(leftSectionX + unit * 5, leftSectionY, unit * 2, unit * 2), 0, 0);
            playerInfo[2].Load(2, new Rectangle(leftSectionX + unit * 5, leftSectionY, unit * 2, unit * 2), 0, 0);
            playerInfo[3].Load(3, new Rectangle(leftSectionX + unit * 5, leftSectionY, unit * 2, unit * 2), 0, 0);

            previousPlayer.Load(res.LeftArrow, new Rectangle(leftSectionX + unit * 4, leftSectionY + unit / 2, unit, unit), Color.Green);
            nextPlayer.Load(res.RightArrow, new Rectangle(leftSectionX + unit * 7, leftSectionY + unit / 2, unit, unit), Color.Green);
            player = playerInfo[selectedPlayer];

            numOfBotsTitle.Load("Number of bots", res.Font_1, new Vector2(leftSectionX, leftSectionY + unit * 3), Color.Yellow);
            numOfBots = roomSetting.NumOfBot;
            numOfBotsEntity.Load(roomSetting.NumOfBot.ToString(), res.Font_2, new Vector2(leftSectionX + unit * 6, leftSectionY + unit * 3), Color.Red);
            decreaseNumOfBots.Load(res.Minus, new Rectangle(leftSectionX + unit * 5, leftSectionY + unit * 3 + unit / 4, unit / 2, unit / 2), Color.White);
            increaseNumOfBots.Load(res.Plus, new Rectangle(leftSectionX + unit * 7, leftSectionY + unit * 3 + unit / 4, unit / 2, unit / 2), Color.White);

            int rightSectionX = viewPort.Width - unit * 9;
            int rightSectionY = unit;

            selectedMap = mapInfo.ToList().FindIndex(curMap => curMap.MapName == roomSetting.MapName);
            map.Load(mapInfo[selectedMap].MapTexture, new Rectangle(rightSectionX + unit, rightSectionY, unit * 6, unit * 6), Color.White);
            previousMap.Load(res.LeftArrow, new Rectangle(rightSectionX, rightSectionY + unit * 2 + unit / 2, unit, unit), Color.White);
            nextMap.Load(res.RightArrow, new Rectangle(rightSectionX + unit * 7, rightSectionY + unit * 2 + unit / 2, unit, unit), Color.White);

            fight.Load("FIGHT", rightSectionX + unit + unit / 2, rightSectionY + unit * 7, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Match));
            returnToMenu.Load("RETURN TO MENU", rightSectionX + unit + unit / 2, rightSectionY + unit * 8 + unit / 2, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Menu));

        }

        public override void HandleEvent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            fight.Update(gameTime);
            returnToMenu.Update(gameTime);

            var mouseEvent = MouseEvent.Instance;

            bool isLeftClick = mouseEvent.IsLeftClick;
            if (mouseEvent.IsHover(previousMap.Rect) && isLeftClick)
            {
                selectedMap--;
                if (selectedMap == -1)
                    selectedMap = 3;
                RoomSetting.Instance.MapName = mapInfo[selectedMap].MapName;
                map.CurTexture = mapInfo[selectedMap].MapTexture;

            }
            else if (mouseEvent.IsHover(nextMap.Rect) && isLeftClick)
            {
                selectedMap++;
                if (selectedMap == 4)
                    selectedMap = 0;
                RoomSetting.Instance.MapName = mapInfo[selectedMap].MapName;
                map.CurTexture = mapInfo[selectedMap].MapTexture;
            }
            else if (mouseEvent.IsHover(previousPlayer.Rect) && isLeftClick)
            {
                selectedPlayer--;
                if (selectedPlayer < 0)
                    selectedPlayer = 3;
                RoomSetting.Instance.PlayerType = selectedPlayer;
                player = playerInfo[selectedPlayer];
            }
            else if (mouseEvent.IsHover(nextPlayer.Rect) && isLeftClick)
            {
                selectedPlayer++;
                if (selectedPlayer == 4)
                    selectedPlayer = 0;
                RoomSetting.Instance.PlayerType = selectedPlayer;
                player = playerInfo[selectedPlayer];
            }
            else if (mouseEvent.IsHover(decreaseNumOfBots.Rect) && isLeftClick)
            {
                numOfBots--;
                if (numOfBots == 0)
                    numOfBots = 1;
                numOfBotsEntity.Text = numOfBots.ToString();
                RoomSetting.Instance.NumOfBot = numOfBots;
            }
            else if (mouseEvent.IsHover(increaseNumOfBots.Rect) && isLeftClick)
            {
                numOfBots++;
                if (numOfBots == 4)
                    numOfBots = 3;
                numOfBotsEntity.Text = numOfBots.ToString();
                RoomSetting.Instance.NumOfBot = numOfBots;
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);

            player.Draw(spriteBatch);
            previousPlayer.Draw(spriteBatch);
            nextPlayer.Draw(spriteBatch);

            playerTypeTitle.Draw(spriteBatch);
            numOfBotsTitle.Draw(spriteBatch);

            numOfBotsEntity.Draw(spriteBatch);
            decreaseNumOfBots.Draw(spriteBatch);
            increaseNumOfBots.Draw(spriteBatch);

            map.Draw(spriteBatch);
            previousMap.Draw(spriteBatch);
            nextMap.Draw(spriteBatch);

            fight.Draw(spriteBatch);
            returnToMenu.Draw(spriteBatch);

        }
    }
}
