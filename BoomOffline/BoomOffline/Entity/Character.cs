using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using BoomOffline.Helper;
using BoomOffline.Model;
using BoomOffline.Resource;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoomOffline.Entity
{
    class Character : IModel
    {
        public const int IDLE = -1;
        public const int MOVE_UP = 0;
        public const int MOVE_DOWN = 1;
        public const int MOVE_LEFT = 2;
        public const int MOVE_RIGHT = 3;
        public const int CONGRATULATION = 4;
        public const int DEATH = 5;

        public bool isPlayer;
        public bool isMoving;
        private bool isAlive;
        private Texture2D sprite;
        private Rectangle curRect;
        private Rectangle newRect;
        private TextEntity playerName;

        private AnimationSprite[] sprites;
        private int currentSprite;
        private int elapsedTime;

        private int timeForAgony;

        private int i, j;
        private int newI, newJ;

        public bool IsMoving
        {
            get { return isMoving; }
            set { isMoving = value; }
        }

        public int I
        {
            get { return i; }
            set { i = value; }
        }

        public int J
        {
            get { return j; }
            set { j = value; }
        }

        public Rectangle CurRect
        {
            get { return curRect; }
        }

        public bool IsAlive
        {
            get { return isAlive; }
            set
            {
                isAlive = value;
                if (!isAlive)
                {
                    elapsedTime = 0;
                    currentSprite = DEATH;
                }

            }
        }

        public int NewI
        {
            get { return newI; }
            set { newI = value; }
        }

        public int NewJ
        {
            get { return newJ; }
            set { newJ = value; }
        }

        public int TimeForAgony
        {
            get { return timeForAgony; }
            set { timeForAgony = value; }
        }

        public Character()
        {
            sprites = new AnimationSprite[]
            {
                new AnimationSprite(), 
                new AnimationSprite(), 
                new AnimationSprite(), 
                new AnimationSprite(), 
                new AnimationSprite(), 
                new AnimationSprite(), 
            };
            newRect = Rectangle.Empty;

        }


        public void Load(int playerType, Rectangle rect, int i, int j, bool isPlayer = false)
        {
            this.isPlayer = isPlayer;
            if (isPlayer)
            {
                var unit = Global.Instance.GameUnit;
                playerName = new TextEntity();
                playerName.Load("YOU", ResManager.Instance.Font_3, new Vector2(rect.X + unit / 2, rect.Y - unit / 2), Color.White, true);
            }
            isAlive = true;
            this.curRect = rect;
            currentSprite = 1;
            this.i = this.newI = i;
            this.j = this.newJ = j;
            TimeForAgony = 1000;
            var characterSprite = ResManager.Instance.Character_1;

            switch (playerType)
            {
                case 1:
                    characterSprite = ResManager.Instance.Character_2;
                    break;
                case 2:
                    characterSprite = ResManager.Instance.Character_3;
                    break;
                case 3:
                    characterSprite = ResManager.Instance.Character_4;
                    break;
            }

            sprite = characterSprite;
            int frameWidth = characterSprite.Width / 3;
            int frameHeight = characterSprite.Height / 6;

            sprites[0].Load(characterSprite, frameWidth, frameHeight, 3, 1);
            sprites[1].Load(characterSprite, frameWidth, frameHeight, 3, 0);
            sprites[2].Load(characterSprite, frameWidth, frameHeight, 3, 2);
            sprites[3].Load(characterSprite, frameWidth, frameHeight, 3, 5);
            sprites[4].Load(characterSprite, frameWidth, frameHeight, 3, 3);
            sprites[5].Load(characterSprite, frameWidth, frameHeight, 3, 4);

        }

        public void Move(int movementIndex)
        {
            var unit = Global.Instance.GameUnit;
            this.currentSprite = movementIndex;
            newRect = curRect;
            switch (movementIndex)
            {
                case MOVE_UP:
                    newRect.Offset(0, -unit);
                    newI = i - 1;
                    break;
                case MOVE_DOWN:
                    newRect.Offset(0, unit);
                    newI = i + 1;
                    break;
                case MOVE_LEFT:
                    newRect.Offset(-unit, 0);
                    newJ = j - 1;
                    break;
                case MOVE_RIGHT:
                    newRect.Offset(unit, 0);
                    newJ = j + 1;
                    break;
            }
            isMoving = true;
        }

        public void Update(GameTime gameTime)
        {
            if (isAlive)
            {
                if (isMoving)
                {
                    if (!newRect.Equals(curRect))
                    {
                        elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                        if (elapsedTime >= 100)
                        {
                            elapsedTime = 0;
                            sprites[currentSprite].Next();
                        }
                        switch (currentSprite)
                        {
                            case MOVE_UP:
                                curRect.Offset(0, -2);
                                if (isPlayer)
                                {
                                    playerName.Offset(0, -2);
                                }
                                break;
                            case MOVE_DOWN:
                                curRect.Offset(0, 2);
                                if (isPlayer)
                                {
                                    playerName.Offset(0, 2);
                                }
                                break;
                            case MOVE_LEFT:
                                curRect.Offset(-2, 0);
                                if (isPlayer)
                                {
                                    playerName.Offset(-2, 0);
                                }
                                break;
                            case MOVE_RIGHT:
                                curRect.Offset(2, 0);
                                if (isPlayer)
                                {
                                    playerName.Offset(2, 0);
                                }
                                break;
                        }

                    }
                    else
                    {
                        isMoving = false;
                        i = newI;
                        j = newJ;
                    }

                }
                else
                {
                    elapsedTime = 0;
                    sprites[currentSprite].BackToDefault();
                }
            }
            else
            {
                elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timeForAgony > 0)
                    timeForAgony -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedTime >= 200)
                {
                    elapsedTime = 0;
                    sprites[currentSprite].Next();
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isPlayer)
                playerName.Draw(spriteBatch);
            spriteBatch.Draw(sprite, curRect, sprites[currentSprite].Frame, Color.White);
        }
    }
}
