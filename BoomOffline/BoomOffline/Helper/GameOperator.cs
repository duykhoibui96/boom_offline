using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoomOffline.Entity;
using BoomOffline.Input;
using BoomOffline.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BoomOffline.Helper
{
    class GameOperator
    {
        private MapGenerator mapGenerator;
        private Character player;
        private List<Bomb> bombs;

        private Character bot; // Tạm thời 1 con trước đi

        public BasicEntity[,] Map
        {
            get { return mapGenerator.Map; }
        }

        public Character Player
        {
            get { return player; }
        }

        public List<Bomb> Bombs
        {
            get { return bombs; }
        }

        public Character Bot
        {
            get { return bot; }
            set { bot = value; }
        }

        public GameOperator()
        {
            player = new Character();
            Bot = new Character();
            mapGenerator = new MapGenerator();
            bombs = new List<Bomb>();
        }

        public void Init()
        {
            mapGenerator.GenerateMap(25, 25);
            player.Load(0, mapGenerator.Map[1, 1].Rect, 1, 1);
            bot.Load(1, mapGenerator.Map[1, 19].Rect, 1, 19);
        }

        public void Update(GameTime gameTime)
        {
            CheckPlayerMoving(player); //Kiểm tra xem nhân vật có lệnh di chuyển hay không
            CheckSetBomb(player); //Kiểm tra xem nhân vật có lệnh đặt bom hay không


            foreach (var bomb in bombs) //Cập nhật trạng thái của quả bom
            {
                bomb.Update(gameTime);
            }
            

            if (player.IsAlive)
            {
                foreach (var bomb in bombs) // Kiểm tra xem bom có nổ chết thằng nào ko
                {
                    if (bomb.State == Bomb.BombState.Explosion)
                    {    
                        if (bot.IsAlive && bomb.IsDeathByBomb(bot.CurRect))
                        {
                            bot.IsAlive = false;
                        }
                        if (player.IsAlive && bomb.IsDeathByBomb(player.CurRect))
                        {
                            player.IsAlive = false;
                        }
                    }
                   
                }
            }

            player.Update(gameTime);//Cập nhật trạng thái nhân vật
            bot.Update(gameTime); //Cập nhật trạng thái bot

            bombs.Remove(bombs.Find(bomb => bomb.State == Bomb.BombState.End)); //Xóa các quả bom đã nổ
        }

        private void CheckPlayerMoving(Character character, bool isBot = false)
        {
            if (player.IsAlive && !player.IsMoving) //Nếu nhân vật đang trong quá trình di chuyển ( 1 lần 1 ô ) thì sẽ skip các lệnh di chuyển
            {
                var keyboard = KeyboardEvent.Instance;
                int movementIndex = Character.IDLE; //Hướng di chuyển của nhân vật (mặc định là đứng yên)

                int characterI = character.I;
                int characterJ = character.J;

                if (!isBot) //Đây là người chơi
                {
                    //Cập nhất hướng di chuyển và tọa độ mới trên map cho người chơi
                    if (keyboard.IsKeyDown(Keys.Up))
                    {
                        characterI--;
                        movementIndex = Character.MOVE_UP;
                    }
                    else if (keyboard.IsKeyDown(Keys.Down))
                    {
                        characterI++;
                        movementIndex = Character.MOVE_DOWN;
                    }
                    else if (keyboard.IsKeyDown(Keys.Left))
                    {
                        characterJ--;
                        movementIndex = Character.MOVE_LEFT;
                    }
                    else if (keyboard.IsKeyDown(Keys.Right))
                    {
                        characterJ++;
                        movementIndex = Character.MOVE_RIGHT;
                    }
                }
                else //Đây là bot
                {
                    // AI điều khiển hướng di chuyển ở đây, cập nhật 2 tham số movementIndex và characterI, characterJ tương tự trên
                }



                if (movementIndex != -1 && mapGenerator.IsValidLocation(characterI, characterJ)) //Nếu nhân vật có lệnh di chuyển và vị trí mới hợp lệ
                {
                    character.Move(movementIndex); //Di chuyển nhân vật
                }
            }

        }

        private void CheckSetBomb(Character character) //Kiểm tra xem nhân vật character có đặt bom k, nếu có thì thêm vào danh sách bom
        {
            var keyboard = KeyboardEvent.Instance;
            if (keyboard.IsPressed(Keys.Space))
            {
                int currentCoordI = character.I;
                int currentCoordJ = character.J;

                //Các giới hạn cho phạm vi nổ của bom ---------------------------------------------------
                var leftLimit = Rectangle.Empty;
                var rightLimit = Rectangle.Empty;
                var topLimit = Rectangle.Empty;
                var bottomLimit = Rectangle.Empty;
                //---------------------------------------------------------------------------------------


                //Cập nhật phạm vi nổ -----------------------------------------------------------------
                for (int i = currentCoordI; i < Map.GetLength(0); i++)
                {
                    if (!mapGenerator.IsValidLocation(i, currentCoordJ))
                    {
                        bottomLimit = Map[i, currentCoordJ].Rect;
                        break;
                    }
                }
                for (int i = currentCoordI; i >= 0; i--)
                {
                    if (!mapGenerator.IsValidLocation(i, currentCoordJ))
                    {
                        topLimit = Map[i, currentCoordJ].Rect;
                        break;
                    }
                }
                for (int j = currentCoordJ; j < Map.GetLength(1); j++)
                {
                    if (!mapGenerator.IsValidLocation(currentCoordI, j))
                    {
                        rightLimit = Map[currentCoordI, j].Rect;
                        break;
                    }
                }
                for (int j = currentCoordJ; j >= 0; j--)
                {
                    if (!mapGenerator.IsValidLocation(currentCoordI, j))
                    {
                        leftLimit = Map[currentCoordI, j].Rect;
                        break;
                    }
                }
                //------------------------------------------------------------------------------


                //Thêm bom mới
                bombs.Add(new Bomb(Map[currentCoordI, currentCoordJ].Rect, leftLimit, rightLimit, topLimit, bottomLimit));
            }
        }


    }
}
