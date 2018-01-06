using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoomOffline.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.Resource
{
    class Global
    {
        private static Global instance;

        public static Global Instance
        {
            get
            {
                if (instance == null)
                    instance = new Global();
                return instance;
            }
        }

        public GraphicsDevice Graphics;
        public int Unit;
        public int GameUnit;

        private Global()
        {
            Unit = 50;
            GameUnit = 30;
        }

    }
}
