﻿using System;
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

        private List<Character> bots; // Tạm thời 1 con trước đi

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

        public List<Character> Bots
        {
            get { return bots; }
            set { bots = value; }
        }

        public GameOperator()
        {
            player = new Character();
            Bots = new List<Character>();
            mapGenerator = new MapGenerator();
            bombs = new List<Bomb>();
        }

        public void Init()
        {
            mapGenerator.GenerateMap(25, 25);
            player.Load(0, mapGenerator.Map[1, 1].Rect, 1, 1);
            bots.Add(new Character());
            bots[0].Load(1, mapGenerator.Map[19, 19].Rect, 19, 19);
            bots.Add(new Character());
            bots[1].Load(1, mapGenerator.Map[19, 1].Rect, 19, 1);
            bots.Add(new Character());
            bots[2].Load(1, mapGenerator.Map[1, 19].Rect, 1, 19);
        }

        public void Update(GameTime gameTime)
        {
            CheckPlayerMoving(player); //Kiểm tra xem nhân vật có lệnh di chuyển hay không
            CheckSetBomb(player); //Kiểm tra xem nhân vật có lệnh đặt bom hay không

            foreach (var b in bots)
                CheckPlayerMoving(b, true);

            foreach (var bomb in bombs) //Cập nhật trạng thái của quả bom
            {
                bomb.Update(gameTime);
            }


            //Kiểm tra nổ lan
            foreach (var bomb in bombs) // Kiểm tra xem bom có nổ chết thằng nào ko
            {
                if (bomb.State == Bomb.BombState.Explosion)
                {
                    bombs.ForEach(otherBom =>
                    {
                        if (!otherBom.Equals(bomb) && otherBom.State == Bomb.BombState.CountDown && bomb.IsInExplosionArea(otherBom.Rect))
                        {
                            otherBom.State = Bomb.BombState.Explosion;
                        }
                    });
                }

            }


            if (player.IsAlive)
            {
                foreach (var bomb in bombs) // Kiểm tra xem bom có nổ chết thằng nào ko
                {
                    if (bomb.State == Bomb.BombState.Explosion)
                    {
                        for (int i = 0; i < bots.Count; i++ )
                            if (bots[i].IsAlive && bomb.IsInExplosionArea(bots[i].CurRect))
                            {
                                bots[i].IsAlive = false;
                                bots.Remove(bots[i]);
                                i--;
                            }
                        if (player.IsAlive && bomb.IsInExplosionArea(player.CurRect))
                        {
                            player.IsAlive = false;
                        }
                    }

                }
            }

            player.Update(gameTime);//Cập nhật trạng thái nhân vật
            foreach (var b in bots)
                b.Update(gameTime); //Cập nhật trạng thái bot

            bombs.Remove(bombs.Find(bomb => bomb.State == Bomb.BombState.End)); //Xóa các quả bom đã nổ
        }

        private void CheckPlayerMoving(Character character, bool isBot = false)
        {
            if (character.IsAlive && !character.IsMoving) //Nếu nhân vật đang trong quá trình di chuyển ( 1 lần 1 ô ) thì sẽ skip các lệnh di chuyển
            {
                var keyboard = KeyboardEvent.Instance;
                int movementIndex = Character.IDLE; //Hướng di chuyển của nhân vật (mặc định là đứng yên)
                int move;

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
                    move = AImove(character);
                    switch (move)
                    {
                        case 0:
                            characterI--;
                            movementIndex = Character.MOVE_UP;
                            break;
                        case 1:
                            characterI++;
                            movementIndex = Character.MOVE_DOWN;
                            break;
                        case 2:
                            characterJ--;
                            movementIndex = Character.MOVE_LEFT;
                            break;
                        case 3:
                            characterJ++;
                            movementIndex = Character.MOVE_RIGHT;
                            break;

                    }
                }



                if (movementIndex != -1 && mapGenerator.IsValidLocation(characterI, characterJ) && !bombs.Any(bomb => bomb.State != Bomb.BombState.End && bomb.I == characterI && bomb.J == characterJ)) //Nếu nhân vật có lệnh di chuyển và vị trí mới hợp lệ
                {
                    if (!isBot || CheckValidBotPosition(character,characterI, characterJ))
                        character.Move(movementIndex); //Di chuyển nhân vật
                }
            }

        }

        private bool CheckValidBotPosition(Character curBot,int newI, int newJ)
        {
            return !bots.Any(bot => !bot.Equals(curBot) && bot.NewI == newI && bot.NewJ == newJ);
            //return true;
        }

        private int AImove(Character c)
        {
            int dir = -1;
            int distance = calcDistance(c.I, c.J, player.I, player.J);

            //kiểm tra có nằm trên đường đạn không
            if (checkInBombsArea(c.I, c.J))
            {
                //tìm điểm an toàn trong phạm vi 3 nước tính từ vị trí hiện tại
                return findSafePlace(c, 3);
            }

            //khi khoảng cách 2 bên quá xa thì thu hẹp khoảng cách
            if (distance > 5)
            {
                int[] possibleMove = findPossibleMove(c);
                dir = findNextMove(possibleMove, c, player.I, player.J);
            }
            //khi gần rồi thì tấn công
            else
            {
                if (player.I == c.I || player.J == c.J)
                {
                    CheckSetBomb(c, true);
                }
            }

            return dir;
        }

        private int findSafePlace(Character c, int length)
        {
            int i, j, dir, tempI, tempJ, distance, shortest;
            distance = 0;
            dir = tempI = tempJ = -1;
            shortest = 99999;
            for (i = c.I - length; i < c.I + length; i++)
            {
                if (i < 0 || i >= mapGenerator.LogicMap.Length)
                    continue;
                for (j = c.J - length; j < c.J + length; j++)
                {
                    if (j < 0 || j >= mapGenerator.LogicMap[0].Length)
                        continue;
                    if (mapGenerator.IsValidLocation(i, j) && !checkInBombsArea(i, j))
                    {
                        distance = calcDistance(i, j, c.I, c.J);
                        if (distance < shortest)
                        {
                            shortest = distance;
                            tempI = i;
                            tempJ = j;
                        }
                    }
                }
            }

            if (tempI != -1 || tempJ != -1)
            {
                return findNextMove(findPossibleMove(c, true), c, tempI, tempJ);
            }
            return 0;
        }

        private int findNextMove(int[] possibleMove, Character c, int destI, int destJ)
        {
            int shortest, tempI, tempJ;
            shortest = 99999;

            int dir = 0, distance = 0;
            for (int i = 1; i <= possibleMove.Length; i++)
            {
                switch (possibleMove[i - 1])
                {
                    case 0:
                        tempI = c.I - 1;
                        tempJ = c.J;
                        distance = calcDistance(tempI, tempJ, destI, destJ);
                        break;
                    case 1:
                        tempI = c.I + 1;
                        tempJ = c.J;
                        distance = calcDistance(tempI, tempJ, destI, destJ);
                        break;
                    case 2:
                        tempI = c.I;
                        tempJ = c.J - 1;
                        distance = calcDistance(tempI, tempJ, destI, destJ);
                        break;
                    case 3:
                        tempI = c.I;
                        tempJ = c.J + 1;
                        distance = calcDistance(tempI, tempJ, destI, destJ);
                        break;

                }
                if (distance < shortest)
                {
                    dir = possibleMove[i - 1];
                    shortest = distance;
                }
            }

            return dir;
        }

        private int[] findPossibleMove(Character c, bool type = false)
        {
            List<int> p = new List<int>();
            int tempI, tempJ;
            if (!type)
            {
                tempI = c.I - 1;
                tempJ = c.J;
                if (mapGenerator.IsValidLocation(tempI, tempJ) && !checkInBombsArea(tempI, tempJ))
                {
                    p.Add(0);
                }

                tempI = c.I + 1;
                tempJ = c.J;
                if (mapGenerator.IsValidLocation(tempI, tempJ) && !checkInBombsArea(tempI, tempJ))
                {
                    p.Add(1);
                }

                tempI = c.I;
                tempJ = c.J - 1;
                if (mapGenerator.IsValidLocation(tempI, tempJ) && !checkInBombsArea(tempI, tempJ))
                {
                    p.Add(2);
                }

                tempI = c.I;
                tempJ = c.J + 1;
                if (mapGenerator.IsValidLocation(tempI, tempJ) && !checkInBombsArea(tempI, tempJ))
                {
                    p.Add(3);
                }
            }
            else
            {
                tempI = c.I - 1;
                tempJ = c.J;
                if (mapGenerator.IsValidLocation(tempI, tempJ))
                {
                    p.Add(0);
                }

                tempI = c.I + 1;
                tempJ = c.J;
                if (mapGenerator.IsValidLocation(tempI, tempJ))
                {
                    p.Add(1);
                }

                tempI = c.I;
                tempJ = c.J - 1;
                if (mapGenerator.IsValidLocation(tempI, tempJ))
                {
                    p.Add(2);
                }

                tempI = c.I;
                tempJ = c.J + 1;
                if (mapGenerator.IsValidLocation(tempI, tempJ))
                {
                    p.Add(3);
                }
            }

            return p.ToArray();
        }

        private bool checkInBombsArea(int i, int j)
        {
            return bombs.Any(bomb => bomb.ExplosionArea.Any(area => area.X == i && area.Y == j)) == true;
        }

        private int calcDistance(int i1, int j1, int i2, int j2)
        {
            return (int)Math.Sqrt(Math.Pow(i1 - i2, 2) + Math.Pow(j1 - j2, 2));
        }

        private void CheckSetBomb(Character character, bool AIbomb = false) //Kiểm tra xem nhân vật character có đặt bom k, nếu có thì thêm vào danh sách bom
        {
            var keyboard = KeyboardEvent.Instance;
            if (AIbomb || keyboard.IsPressed(Keys.Space))
            {
                int currentCoordI = character.I;
                int currentCoordJ = character.J;

                //Các giới hạn cho phạm vi nổ của bom ---------------------------------------------------
                var leftLimit = Rectangle.Empty;
                var rightLimit = Rectangle.Empty;
                var topLimit = Rectangle.Empty;
                var bottomLimit = Rectangle.Empty;

                var explosionArea = new List<Point>();
                //---------------------------------------------------------------------------------------


                //Cập nhật phạm vi nổ -----------------------------------------------------------------
                for (int i = currentCoordI; i < Map.GetLength(0); i++)
                {
                    if (!mapGenerator.IsValidLocation(i, currentCoordJ))
                    {
                        bottomLimit = Map[i, currentCoordJ].Rect;
                        break;
                    }
                    else
                    {
                        explosionArea.Add(new Point(i, currentCoordJ));
                    }
                }
                for (int i = currentCoordI; i >= 0; i--)
                {
                    if (!mapGenerator.IsValidLocation(i, currentCoordJ))
                    {
                        topLimit = Map[i, currentCoordJ].Rect;
                        break;
                    }
                    else
                    {
                        explosionArea.Add(new Point(i, currentCoordJ));
                    }
                }
                for (int j = currentCoordJ; j < Map.GetLength(1); j++)
                {
                    if (!mapGenerator.IsValidLocation(currentCoordI, j))
                    {
                        rightLimit = Map[currentCoordI, j].Rect;
                        break;
                    }
                    else
                    {
                        explosionArea.Add(new Point(currentCoordI, j));
                    }
                }
                for (int j = currentCoordJ; j >= 0; j--)
                {
                    if (!mapGenerator.IsValidLocation(currentCoordI, j))
                    {
                        leftLimit = Map[currentCoordI, j].Rect;
                        break;
                    }
                    else
                    {
                        explosionArea.Add(new Point(currentCoordI, j));
                    }
                }
                //------------------------------------------------------------------------------



                //Thêm bom mới
                bombs.Add(new Bomb(currentCoordI, currentCoordJ, Map[currentCoordI, currentCoordJ].Rect, leftLimit, rightLimit, topLimit, bottomLimit, explosionArea.ToArray()));
            }
        }


    }
}
