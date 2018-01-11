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
        public SpriteFont Font_3;

        public Texture2D Character_1;
        public Texture2D Character_2;
        public Texture2D Character_3;
        public Texture2D Character_4;

        public Texture2D Map;

        public Texture2D Map_1;
        public Texture2D Map_2;
        public Texture2D Map_3;
        public Texture2D Map_4;
        public Texture2D RandomMap;

        public Texture2D LeftArrow;
        public Texture2D RightArrow;
        public Texture2D Plus;
        public Texture2D Minus;

        public Texture2D Bomb;
        public Texture2D RoomBackground;

        public Texture2D Explosion;

        public Texture2D Cursor;

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
            ButtonContainer = contentManager.Load<Texture2D>("button_container_2");

            Font_1 = contentManager.Load<SpriteFont>("font_1");
            Font_2 = contentManager.Load<SpriteFont>("font_2");
            Font_3 = contentManager.Load<SpriteFont>("font_3");

            Character_1 = contentManager.Load<Texture2D>("character_1");
            Character_2 = contentManager.Load<Texture2D>("character_2");
            Character_3 = contentManager.Load<Texture2D>("character_3");
            Character_4 = contentManager.Load<Texture2D>("character_4");

            Bomb = contentManager.Load<Texture2D>("bomb");
            RoomBackground = contentManager.Load<Texture2D>("room_background");

            Explosion = contentManager.Load<Texture2D>("explosion");

            LeftArrow = contentManager.Load<Texture2D>("left_arrow");
            RightArrow = contentManager.Load<Texture2D>("right_arrow");
            Plus = contentManager.Load<Texture2D>("plus");
            Minus = contentManager.Load<Texture2D>("minus");
            Map = contentManager.Load<Texture2D>("map");

            Map_1 = contentManager.Load<Texture2D>("map_01");
            Map_2 = contentManager.Load<Texture2D>("map_02");
            Map_3 = contentManager.Load<Texture2D>("map_03");
            Map_4 = contentManager.Load<Texture2D>("map_04");
            RandomMap = contentManager.Load<Texture2D>("random_map");

            Cursor = contentManager.Load<Texture2D>("cursor");

        }

    }
}
