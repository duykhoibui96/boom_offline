using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.Resource
{
    class ResManager
    {
        private static ResManager instance;

        public Texture2D MenuBackground;
        public Texture2D ButtonContainer;

        public SpriteFont Font_1;
        public SpriteFont Font_2;

        public Texture2D Character_1;
        public Texture2D Character_2;

        public Texture2D Bomb;

        public Texture2D Explosion;

        public static ResManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ResManager();
                return instance;
            }
        }

        private ResManager()
        {
            
        }

        public void Load(ContentManager contentManager)
        {
            MenuBackground = contentManager.Load<Texture2D>("menu_background");
            ButtonContainer = contentManager.Load<Texture2D>("button_container");

            Font_1 = contentManager.Load<SpriteFont>("font_1");
            Font_2 = contentManager.Load<SpriteFont>("font_2");

            Character_1 = contentManager.Load<Texture2D>("character_1");
            Character_2 = contentManager.Load<Texture2D>("character_2");

            Bomb = contentManager.Load<Texture2D>("bomb");

            Explosion = contentManager.Load<Texture2D>("explosion");
        }

    }
}
