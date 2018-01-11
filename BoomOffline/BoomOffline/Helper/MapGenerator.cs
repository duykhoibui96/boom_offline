using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoomOffline.Entity;
using BoomOffline.Resource;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.Helper
{
    class MapGenerator
    {

        private static MapGenerator instance;

        private const int TYPE_OBSTACLE = 1;
        private const int TYPE_EMPTY = 0;
        private int count;
        Random rand = new Random();

        public BasicEntity[,] map;
        private int[][] logicMap;

        public BasicEntity[,] Map
        {
            get { return map; }
        }

        public int[][] LogicMap
        {
            get { return logicMap; }
            set { logicMap = value; }
        }

        public static MapGenerator Instance
        {
            get
            {
                if (instance == null)
                    instance = new MapGenerator();
                return instance;
            }
            set { instance = value; }
        }

        private MapGenerator()
        {

        }

        public Point GetRealPosition(int i, int j)
        {
            return map[i, j].Rect.Location;
        }

        private void GenerateLogicMap(string map)
        {
            if (map == "random_map")
            {
                generateRandomMap();
            }
            else
            {
                List<int[]> grid = new List<int[]>();
                string[] lines = System.IO.File.ReadAllLines(@map);
                foreach (string line in lines)
                {
                    string[] tokens = line.Split('\t');
                    grid.Add(tokens.Select(token => Int32.Parse(token)).ToArray());
                }

                LogicMap = grid.ToArray();
            }

        }

        public void generateRandomMap()
        {
            var mapSize = RoomSetting.Instance.MapSize;
            int[] temp = new int[3];
            int w, h, type, i;
            int divide = RoomSetting.Instance.MapSize / RoomSetting.Instance.NumOfBot;
            //Đọc dữ liệu từ UserSetting.Instance.NumOfBots để biết số con bot
            for (i = 0; i < RoomSetting.Instance.NumOfBot; i++)
            {
                w = rand.Next(3, RoomSetting.Instance.MapSize - 3);
                h = rand.Next(divide * i, divide * i + divide);
                type = rand.Next(0, 3);
                temp[0] = h;
                temp[1] = w;
                do
                {
                    type = rand.Next(0, 3);
                } while (type == RoomSetting.Instance.PlayerType);
                temp[2] = type;
                RoomSetting.Instance.MyBot.Add(temp);
            }

            logicMap = new int[mapSize][];
            for (i = 0; i < RoomSetting.Instance.MapSize; i++)
            {
                logicMap[i] = new int[mapSize];
                for (int j = 0; j < RoomSetting.Instance.MapSize; j++)
                    LogicMap[i][j] = TYPE_OBSTACLE;
            }

            for (i = 0; i < RoomSetting.Instance.MyBot.Count; i++)
            {
                count = 0;
                recursion(RoomSetting.Instance.MyBot[i][0], RoomSetting.Instance.MyBot[i][1]);
            }

        }



        public void recursion(int r, int c)
        {
            Console.WriteLine(count);
            while (true)
            {
                if ((LogicMap[r][c] == TYPE_EMPTY && LogicMap[1][1] == TYPE_EMPTY) || count >= 10000)
                    return;
                count++;
                bool key = false;
                // 4 random directions
                //int[] randDirs = generateRandomDirections();
                // Examine each direction
                //for (int i = 0; i < randDirs.Length; i++)
                {
                    int direction = rand.Next(1, 5);

                    switch (direction)
                    {
                        case 1: // Up
                            //　Whether 2 cells up is out or not
                            if (r - 2 <= 0)
                            {
                                key = true;
                                break;
                            }
                            //if (LogicMap[r - 2][c] != TYPE_EMPTY)
                            {
                                LogicMap[r - 2][c] = TYPE_EMPTY;
                                LogicMap[r - 1][c] = TYPE_EMPTY;
                                r = r - 2;
                            }
                            break;
                        case 2: // Right
                            // Whether 2 cells to the right is out or not
                            if (c + 2 >= RoomSetting.Instance.MapSize - 1)
                            {
                                key = true;
                                break;
                            }
                            //if (LogicMap[r][c + 2] != TYPE_EMPTY)
                            {
                                LogicMap[r][c + 2] = TYPE_EMPTY;
                                LogicMap[r][c + 1] = TYPE_EMPTY;
                                c = c + 2;
                            }
                            break;
                        case 3: // Down
                            // Whether 2 cells down is out or not
                            if (r + 2 >= RoomSetting.Instance.MapSize - 1)
                            {
                                key = true;
                                break;
                            }
                            //if (LogicMap[r + 2][c] != TYPE_EMPTY)
                            {
                                LogicMap[r + 2][c] = TYPE_EMPTY;
                                LogicMap[r + 1][c] = TYPE_EMPTY;
                                r = r + 1;
                            }
                            break;
                        case 4: // Left
                            // Whether 2 cells to the left is out or not
                            if (c - 2 <= 0)
                            {
                                key = true;
                                break;
                            }
                            //if (LogicMap[r][c - 2] != TYPE_EMPTY)
                            {
                                LogicMap[r][c - 2] = TYPE_EMPTY;
                                LogicMap[r][c - 1] = TYPE_EMPTY;
                                c = c - 2;
                            }
                            break;
                    }

                }
            }
        }

        /**
        * Generate an array with random directions 1-4
        * @return Array containing 4 directions in random order
        */
        public int[] generateRandomDirections()
        {
            Random rand = new Random();
            List<int> randoms = new List<int>();
            for (int i = 0; i < 4; i++)
                randoms.Add(i + 1);

            return randoms.OrderBy(x => rand.Next()).ToArray();
        }

        public void GenerateMap()
        {
            GenerateLogicMap(RoomSetting.Instance.MapName);
            map = new BasicEntity[LogicMap.Length, LogicMap[0].Length];
            var unit = Global.Instance.GameUnit;
            var viewPort = Global.Instance.Graphics.Viewport;
            int startMapX = 10;
            int startMapY = 10;

            int mapSize = RoomSetting.Instance.MapSize;

            if (mapSize * unit < viewPort.Width)
            {
                startMapX = (viewPort.Width - unit * mapSize) / 2;
            }

            Texture2D obstacle = new Texture2D(Global.Instance.Graphics, 1, 1);
            obstacle.SetData(new Color[] { Color.Gray });
            Texture2D empty = new Texture2D(Global.Instance.Graphics, 1, 1);
            empty.SetData(new Color[] { Color.Green });

            for (int i = 0; i < LogicMap.Length; i++)
            {
                for (int j = 0; j < LogicMap[i].Length; j++)
                {
                    map[i, j] = new BasicEntity();
                    map[i, j].Load(LogicMap[i][j] == TYPE_OBSTACLE ? obstacle : empty, new Rectangle(startMapX + unit * j, startMapY + unit * i, unit, unit), Color.White);
                }
            }
            Global.Instance.map_size = new Vector2(map.GetLength(0) * unit, map.GetLength(1) * unit);
        }

        public bool IsValidLocation(int i, int j)
        {
            return LogicMap[i][j] == TYPE_EMPTY;
        }

    }
}
