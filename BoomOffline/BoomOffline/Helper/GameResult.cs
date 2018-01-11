using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BoomOffline.Helper
{
    class GameResult
    {
        private static GameResult instance;

        private bool isWin;

        public static GameResult Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameResult();
                return instance;
            }
            set { instance = value; }
        }

        public bool IsWin
        {
            get { return isWin; }
            set { isWin = value; }
        }

        private GameResult()
        {
            
        }


    }
}
