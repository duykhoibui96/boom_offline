using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using BoomOffline.Entity;
using BoomOffline.Resource;
using Microsoft.Xna.Framework;

namespace BoomOffline.Helper
{
    internal class MatchStorage
    {
        private static MatchStorage instance;

        private string[] mapData;
        private string[] characterData;
        private string cameraData;

        private bool dataAvailable;
        private bool needToLoadDataHere;

        public static MatchStorage Instance
        {
            get
            {
                if (instance == null)
                    instance = new MatchStorage();
                return instance;
            }
            set { instance = value; }
        }

        public bool NeedToLoadDataHere
        {
            get { return needToLoadDataHere; }
            set { needToLoadDataHere = value; }
        }

        public bool DataAvailable
        {
            get
            {
                return dataAvailable;
            }
            set
            {
                dataAvailable = value;
            }
        }

        public string[] MapData
        {
            get { return mapData; }
            set { mapData = value; }
        }

        public string[] CharacterData
        {
            get { return characterData; }
            set { characterData = value; }
        }

        public string CameraData
        {
            get { return cameraData; }
            set { cameraData = value; }
        }

        private MatchStorage()
        {
        }

        public void Save(Character player, List<Character> bots, List<Bomb> bombs)
        {
            var room = RoomSetting.Instance;
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"match.txt"))
            {
                file.WriteLine("MAP");
                file.WriteLine(room.MapName);
                file.WriteLine(room.MapSize);
                file.WriteLine(room.PlayerType);
                file.WriteLine(room.NumOfBot);
                if (!room.PosCam.Equals(Vector2.Zero))
                {
                    var _pos = room.PosCam;
                    file.WriteLine(_pos.X + " " + _pos.Y);
                }
                else
                {
                    file.WriteLine("No poscam");
                }
                if (!room.PosCam.Equals(Vector2.Zero))
                {
                    var _pos = room.PlusPosCam;
                    file.WriteLine(_pos.X + " " + _pos.Y);
                }
                else
                {
                    file.WriteLine("No plus poscam");
                }
                file.WriteLine("CAMERA");
                if (Global.Instance.currentCamera != null)
                {
                    var _pos = Global.Instance.currentCamera._pos;
                    file.WriteLine(_pos.X + " " + _pos.Y);
                }
                else
                {
                    file.WriteLine("NO CAMERA");
                }
                file.WriteLine("CHARACTERS");
                player.Save(file);
                foreach (var bot in bots)
                {
                    if (bot.IsAlive)
                    {
                        bot.Save(file);
                    }
                }
                //file.WriteLine("BOMBS");
                //foreach (var bomb in bombs)
                //{
                //    if (bomb.State == Bomb.BombState.CountDown)
                //    {
                //        bomb.Save(file);
                //    }
                //}
            }
        }

        public void Load()
        {
            string[] lines = System.IO.File.ReadAllLines(@"match.txt");
            if (lines.Length == 0)
            {
                dataAvailable = false;
            }
            else
            {
                dataAvailable = true;
                var mapData = new List<String>();
                var characterData = new List<String>();

                int lineIndex = 1;
                while (lines[lineIndex] != "CAMERA")
                {
                    mapData.Add(lines[lineIndex++]);
                }
                lineIndex++;
                if (lines[lineIndex] == "NO CAMERA")
                {
                    cameraData = null;
                }
                else
                {
                    cameraData = lines[lineIndex];
                }
                lineIndex+=2;
                while (lineIndex < lines.Length)
                {
                    characterData.Add(lines[lineIndex++]);
                }

                this.mapData = mapData.ToArray();
                this.characterData = characterData.ToArray();

            }
        }

        public void Clear()
        {
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"match.txt"))
            {
                file.Write("");
                mapData = characterData = null;
                cameraData = null;
            }
        }
    }
}
