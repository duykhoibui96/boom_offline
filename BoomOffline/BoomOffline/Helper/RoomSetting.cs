using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoomOffline.Helper
{
    class RoomSetting
    {
        private static RoomSetting instance;

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
                    case "map02.txt":
                        mapSize = 31;
                        break;
                    case "map03.txt":
                        mapSize = 51;
                        break;
                    case "map04.txt":
                        mapSize = 51;
                        break;
                }
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

    }
}
