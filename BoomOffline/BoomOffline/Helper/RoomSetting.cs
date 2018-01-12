using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BoomOffline.Helper
{
    class RoomSetting
    {
        private static RoomSetting instance;

        private bool multiplayerMode;
        public static RoomSetting Instance
        {
            get
            {
                if (instance == null)
                    instance = new RoomSetting();
                return instance;
            }
        }

        public int MapSize
        {
            get { return mapSize; }
            set { mapSize = value; }
        }

        public List<int[]>MyBot
        {
            get {
                if (myBot == null)
                    myBot = new List<int[]>();
                return myBot; 
            }
            set { myBot = value; }
        }

        public string MapName
        {
            get { return mapName; }
            set 
            { 
                mapName = value; 
                switch(mapName)
                {
                    case "map01.txt":
                        mapSize = 21;
                        break;
                    case "map02.txt": case "random_map":
                        mapSize = 31;
                        break;
                    case "map03.txt":
                        mapSize = 51;
                        break;
                    case "map04.txt":
                        mapSize = 51;
                        break;

                }
                plusPosCam.X = 0;
                plusPosCam.Y = 0;
            }
        }

        public int NumOfBot
        {
            get { return numOfBots; }
            set { numOfBots = value; }
        }

        public int PlayerType
        {
            get { return playerType; }
            set { playerType = value; }
        }

        public Vector2 PosCam
        {
            get { return posCam; }
            set { posCam = value; }
        }

        public Vector2 PlusPosCam
        {
            get { return plusPosCam; }
            set { plusPosCam = value; }
        }

        public bool MultiplayerMode
        {
            get { return multiplayerMode; }
            set { multiplayerMode = value; }
        }

        public int Player2Type
        {
            get { return player2Type; }
            set { player2Type = value; }
        }

        private RoomSetting()
        {
            mapName = "map01.txt";
            numOfBots = 2;
            PlayerType = 0;
            mapSize = 21;
        }

        private string mapName;
        private int numOfBots;
        private int mapSize;
        private int playerType;
        private int player2Type;
        private List<int[]> myBot;
        private Vector2 posCam;
        private Vector2 plusPosCam;
    }
}
