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
        public int Unit = 50;
        public int GameUnit = 30;

        private Global()
        {
        }

    }
}
