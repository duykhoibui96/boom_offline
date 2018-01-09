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

        public string MapName
        {
            get { return mapName; }
            set { mapName = value; }
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
        }

        private string mapName;
        private int numOfBots;
        private int playerType;

    }
}
