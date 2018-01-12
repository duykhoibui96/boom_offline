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
        private Character[] player2Info;

        private BasicEntity background;

        private TextEntity multiplayerTitle;
        private TextEntity multiplayerEnabled;
        private TextEntity multiplayerDisabled;

        private TextEntity playerTypeTitle;
        private Character player;
        private BasicEntity previousPlayer;
        private BasicEntity nextPlayer;

        private TextEntity player2TypeTitle;
        private Character player2;
        private BasicEntity previousPlayer2;
        private BasicEntity nextPlayer2;


        private Button fight;
        private Button returnToMenu;

        private TextEntity title;
        private TextEntity numOfBotsTitle;
        private TextEntity numOfBotsEntity;
        private int numOfBots;
        private BasicEntity decreaseNumOfBots;
        private BasicEntity increaseNumOfBots;

        private BasicEntity map;
        private BasicEntity previousMap;
        private BasicEntity nextMap;

        private int selectedMap;
        private int selectedPlayer;
        private int selectedPlayer2;

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

            player2Info = new Character[]
            {
                new Character(), 
                new Character(), 
                new Character(), 
                new Character()
            };

            background = new BasicEntity();

            multiplayerTitle = new TextEntity();
            multiplayerEnabled = new TextEntity();
            multiplayerDisabled = new TextEntity();

            title = new TextEntity();
            playerTypeTitle = new TextEntity();
            player = new Character();
            previousPlayer = new BasicEntity();
            nextPlayer = new BasicEntity();

            player2TypeTitle = new TextEntity();
            player2 = new Character();
            previousPlayer2 = new BasicEntity();
            nextPlayer2 = new BasicEntity();


            fight = new Button();
            returnToMenu = new Button();

            numOfBotsTitle = new TextEntity();
            numOfBotsEntity = new TextEntity();
            decreaseNumOfBots = new BasicEntity();
            increaseNumOfBots = new BasicEntity();

            map = new BasicEntity();



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
                new MapInfo("map04.txt", res.Map_4),
                new MapInfo("random_map", res.RandomMap), 
            };

            multiplayerTitle.Load("Multiplayer", res.Font_1, new Vector2(unit, unit * 3), Color.Yellow);
            multiplayerEnabled.Load("Yes", res.Font_1, new Vector2(unit * 5, unit * 3), Color.Black);
            multiplayerDisabled.Load("No", res.Font_1, new Vector2(unit * 7, unit * 3), Color.White);

            int leftSectionX = unit;
            int leftSectionY = unit * 4;

            background.Load(res.RoomBackground, new Rectangle(0, 0, viewPort.Width, viewPort.Height), Color.White);

            selectedPlayer = roomSetting.PlayerType;

            title.Load("WAITING ROOM", res.Font_2, new Vector2(viewPort.Width / 2, unit), Color.White, true);
            playerTypeTitle.Load("Player type", res.Font_1, new Vector2(leftSectionX, leftSectionY + unit / 2), Color.Yellow);
            playerInfo[0].Load(0, new Rectangle(leftSectionX + unit * 5, leftSectionY, unit * 2, unit * 2), 0, 0);
            playerInfo[1].Load(1, new Rectangle(leftSectionX + unit * 5, leftSectionY, unit * 2, unit * 2), 0, 0);
            playerInfo[2].Load(2, new Rectangle(leftSectionX + unit * 5, leftSectionY, unit * 2, unit * 2), 0, 0);
            playerInfo[3].Load(3, new Rectangle(leftSectionX + unit * 5, leftSectionY, unit * 2, unit * 2), 0, 0);

            previousPlayer.Load(res.LeftArrow, new Rectangle(leftSectionX + unit * 4, leftSectionY + unit / 2, unit, unit), Color.Green);
            nextPlayer.Load(res.RightArrow, new Rectangle(leftSectionX + unit * 7, leftSectionY + unit / 2, unit, unit), Color.Green);
            player = playerInfo[selectedPlayer];

            player2TypeTitle.Load("Player 2 type", res.Font_1, new Vector2(leftSectionX, leftSectionY + unit * 2 + unit / 2), Color.Yellow);
            player2Info[0].Load(0, new Rectangle(leftSectionX + unit * 5, leftSectionY + unit * 2, unit * 2, unit * 2), 0, 0);
            player2Info[1].Load(1, new Rectangle(leftSectionX + unit * 5, leftSectionY + unit * 2, unit * 2, unit * 2), 0, 0);
            player2Info[2].Load(2, new Rectangle(leftSectionX + unit * 5, leftSectionY + unit * 2, unit * 2, unit * 2), 0, 0);
            player2Info[3].Load(3, new Rectangle(leftSectionX + unit * 5, leftSectionY + unit * 2, unit * 2, unit * 2), 0, 0);
            previousPlayer2.Load(res.LeftArrow, new Rectangle(leftSectionX + unit * 4, leftSectionY + +unit * 2 + unit / 2, unit, unit), Color.Green);
            nextPlayer2.Load(res.RightArrow, new Rectangle(leftSectionX + unit * 7, leftSectionY + +unit * 2 + unit / 2, unit, unit), Color.Green);
            player2 = player2Info[selectedPlayer2];


            numOfBotsTitle.Load("Number of bots", res.Font_1, new Vector2(leftSectionX, leftSectionY + unit * 2), Color.Yellow);
            numOfBots = roomSetting.NumOfBot;
            numOfBotsEntity.Load(roomSetting.NumOfBot.ToString(), res.Font_2, new Vector2(leftSectionX + unit * 6, leftSectionY + unit * 2), Color.White);
            decreaseNumOfBots.Load(res.Minus, new Rectangle(leftSectionX + unit * 5, leftSectionY + unit * 2 + unit / 4, unit / 2, unit / 2), Color.White);
            increaseNumOfBots.Load(res.Plus, new Rectangle(leftSectionX + unit * 7, leftSectionY + unit * 2 + unit / 4, unit / 2, unit / 2), Color.White);

            int rightSectionX = viewPort.Width - unit * 9;
            int rightSectionY = unit * 2 + unit / 2;

            selectedMap = mapInfo.ToList().FindIndex(curMap => curMap.MapName == roomSetting.MapName);
            map.Load(mapInfo[selectedMap].MapTexture, new Rectangle(rightSectionX + unit, rightSectionY, unit * 6, unit * 6), Color.White);
            previousMap.Load(res.LeftArrow, new Rectangle(rightSectionX - 5, rightSectionY + unit * 2 + unit / 2, unit, unit), Color.White);
            nextMap.Load(res.RightArrow, new Rectangle(rightSectionX + unit * 7 + 5, rightSectionY + unit * 2 + unit / 2, unit, unit), Color.White);

            fight.Load("FIGHT", rightSectionX + unit + unit / 2, rightSectionY + unit * 7, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Loading));
            returnToMenu.Load("RETURN TO MENU", rightSectionX + unit + unit / 2, rightSectionY + unit * 8 + unit / 2, new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Menu));


            if (RoomSetting.Instance.MultiplayerMode)
            {
                multiplayerEnabled.TextColor = Color.White;
                multiplayerDisabled.TextColor = Color.Black;
                selectedMap = 0;
                roomSetting.MapName = mapInfo[selectedMap].MapName;
                map.CurTexture = mapInfo[selectedMap].MapTexture;
            }

        }

        public override void HandleEvent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var roomSetting = RoomSetting.Instance;

            fight.Update(gameTime);
            returnToMenu.Update(gameTime);

            var mouseEvent = MouseEvent.Instance;

            bool isLeftClick = mouseEvent.IsLeftClick;

            if (mouseEvent.IsHover(multiplayerEnabled.Rect) && isLeftClick)
            {
                multiplayerEnabled.TextColor = Color.White;
                multiplayerDisabled.TextColor = Color.Black;
                roomSetting.MultiplayerMode = true;
                selectedMap = 0;
                roomSetting.MapName = mapInfo[selectedMap].MapName;
                map.CurTexture = mapInfo[selectedMap].MapTexture;

            }
            else if (mouseEvent.IsHover(multiplayerDisabled.Rect) && isLeftClick)
            {
                multiplayerEnabled.TextColor = Color.Black;
                multiplayerDisabled.TextColor = Color.White;
                roomSetting.MultiplayerMode = false;
            }
            else if (mouseEvent.IsHover(previousPlayer.Rect) && isLeftClick)
            {
                selectedPlayer--;
                if (selectedPlayer < 0)
                    selectedPlayer = 3;
                roomSetting.PlayerType = selectedPlayer;
                player = playerInfo[selectedPlayer];
            }
            else if (mouseEvent.IsHover(nextPlayer.Rect) && isLeftClick)
            {
                selectedPlayer++;
                if (selectedPlayer == 4)
                    selectedPlayer = 0;
                roomSetting.PlayerType = selectedPlayer;
                player = playerInfo[selectedPlayer];
            }
            else
            {
                if (RoomSetting.Instance.MultiplayerMode)
                {
                    if (mouseEvent.IsHover(previousPlayer2.Rect) && isLeftClick)
                    {
                        selectedPlayer2--;
                        if (selectedPlayer2 < 0)
                            selectedPlayer2 = 3;
                        roomSetting.Player2Type = selectedPlayer2;
                        player2 = player2Info[selectedPlayer2];
                    }
                    else if (mouseEvent.IsHover(nextPlayer2.Rect) && isLeftClick)
                    {
                        selectedPlayer2++;
                        if (selectedPlayer2 == 4)
                            selectedPlayer2 = 0;
                        roomSetting.Player2Type = selectedPlayer2;
                        player2 = player2Info[selectedPlayer2];
                    }
                }
                else
                {
                    if (mouseEvent.IsHover(decreaseNumOfBots.Rect) && isLeftClick)
                    {
                        numOfBots--;
                        if (numOfBots == 0)
                            numOfBots = 1;
                        numOfBotsEntity.Text = numOfBots.ToString();
                        roomSetting.NumOfBot = numOfBots;
                    }
                    else if (mouseEvent.IsHover(increaseNumOfBots.Rect) && isLeftClick)
                    {
                        numOfBots++;
                        if (numOfBots == 7)
                            numOfBots = 6;
                        numOfBotsEntity.Text = numOfBots.ToString();
                        roomSetting.NumOfBot = numOfBots;
                    }
                    else if (mouseEvent.IsHover(previousMap.Rect) && isLeftClick)
                    {
                        selectedMap--;
                        if (selectedMap == -1)
                            selectedMap = 4;
                        roomSetting.MapName = mapInfo[selectedMap].MapName;
                        map.CurTexture = mapInfo[selectedMap].MapTexture;

                    }
                    else if (mouseEvent.IsHover(nextMap.Rect) && isLeftClick)
                    {
                        selectedMap++;
                        if (selectedMap == 5)
                            selectedMap = 0;
                        roomSetting.MapName = mapInfo[selectedMap].MapName;
                        map.CurTexture = mapInfo[selectedMap].MapTexture;
                    }
                }
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);

            title.Draw(spriteBatch);

            multiplayerTitle.Draw(spriteBatch);
            multiplayerEnabled.Draw(spriteBatch);
            multiplayerDisabled.Draw(spriteBatch);

            player.Draw(spriteBatch);
            previousPlayer.Draw(spriteBatch);
            nextPlayer.Draw(spriteBatch);

            playerTypeTitle.Draw(spriteBatch);


            if (RoomSetting.Instance.MultiplayerMode)
            {
                player2.Draw(spriteBatch);
                previousPlayer2.Draw(spriteBatch);
                nextPlayer2.Draw(spriteBatch);
                player2TypeTitle.Draw(spriteBatch);
            }
            else
            {
                numOfBotsTitle.Draw(spriteBatch);
                numOfBotsEntity.Draw(spriteBatch);
                decreaseNumOfBots.Draw(spriteBatch);
                increaseNumOfBots.Draw(spriteBatch);
                previousMap.Draw(spriteBatch);
                nextMap.Draw(spriteBatch);
            }

            map.Draw(spriteBatch);

            fight.Draw(spriteBatch);
            returnToMenu.Draw(spriteBatch);

        }
    }
}
