using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoomOffline.Entity;
using BoomOffline.Event;
using BoomOffline.Input;
using BoomOffline.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BoomOffline.Helper
{
    class GameOperator
    {
        private MapGenerator mapGenerator;
        private MiniMap miniMap;
        private Character player;
        private Character player_2;
        private List<Bomb> bombs;
        private Astar astar;
        private bool isMultiplayer;
        Random rand = new Random();

        private List<Character> bots;

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

        public MiniMap MiniMap
        {
            get { return miniMap; }
            set { miniMap = value; }
        }

        public Character Player2
        {
            get { return player_2; }
            set { player_2 = value; }
        }

        public bool IsMultiplayer
        {
            get { return isMultiplayer; }
            set { isMultiplayer = value; }
        }

        public GameOperator()
        {
            player = new Character();
            Player2 = new Character();
            Bots = new List<Character>();
            mapGenerator = MapGenerator.Instance;
            bombs = new List<Bomb>();
            MiniMap = new MiniMap();
        }

        public void Init()
        {
            int i, j, flag = 0;
            int w, h, type;

            isMultiplayer = RoomSetting.Instance.MultiplayerMode;

            if (isMultiplayer)
            {
                player.Load(RoomSetting.Instance.PlayerType, mapGenerator.Map[19, 19].Rect, 19, 19, "PLAYER 1");
                Player2.Load(RoomSetting.Instance.Player2Type, mapGenerator.Map[1, 1].Rect, 1, 1, "PLAYER 2");
            }
            else
            {
                //Đọc dữ liệu từ UserSetting.Instance.NumOfBots để biết số con bot
                if (RoomSetting.Instance.MapName != "random_map")
                {
                    if (MatchStorage.Instance.NeedToLoadDataHere)
                    {
                        var characterData = MatchStorage.Instance.CharacterData;
                        player.LoadData(characterData[0]);
                        for (int index = 1; index < characterData.Length; index++)
                        {
                            var bot = new Character();
                            bot.LoadData(characterData[index]);
                            bots.Add(bot);
                        }
                        MatchStorage.Instance.NeedToLoadDataHere = false;
                    }
                    else
                    {
                        player.Load(RoomSetting.Instance.PlayerType, mapGenerator.Map[1, 1].Rect, 1, 1, "PLAYER");

                        int divide = RoomSetting.Instance.MapSize / RoomSetting.Instance.NumOfBot;
                        for (i = 0; i < RoomSetting.Instance.NumOfBot; i++)
                        {
                            //random position
                            do
                            {
                                w = rand.Next(3, RoomSetting.Instance.MapSize - 3);
                                h = rand.Next(divide * i, divide * i + divide);
                            } while (!mapGenerator.IsValidLocation(h, w));

                            do
                            {
                                type = rand.Next(0, 3);
                            } while (type == RoomSetting.Instance.PlayerType);

                            bots.Add(new Character());
                            bots[i].Load(type, mapGenerator.Map[h, w].Rect, h, w);
                        }
                    }   
                }
                else
                {
                    for (i = 0; i < RoomSetting.Instance.MapSize; i++)
                    {
                        for (j = 0; j < RoomSetting.Instance.MapSize; j++)
                            if (mapGenerator.IsValidLocation(i, j))
                            {
                                player.Load(RoomSetting.Instance.PlayerType, mapGenerator.Map[i, j].Rect, i, j,"YOU");
                                flag = 1;
                                break;
                            }
                        if (flag == 1)
                            break;
                    }
                    bots = RoomSetting.Instance.MyBot.Select(bot =>
                    {
                        var newBot = new Character();
                        newBot.Load(bot[2], mapGenerator.Map[bot[0], bot[1]].Rect, bot[0], bot[1]);
                        return newBot;
                    }).ToList();
                }

                astar = new Astar(this);
                if (RoomSetting.Instance.MapSize > 21)
                    miniMap.Load(MapGenerator.Instance.LogicMap, RoomSetting.Instance.MapSize);
            }

        }

        public void Update(GameTime gameTime)
        {
            if (isMultiplayer)
            {
                if (!player.IsAlive)
                {
                    if (player.TimeForAgony <= 0)
                    {
                        GameResult.Instance.IsWin = true;
                        GameResult.Instance.WinnerPlayer = 2;
                        EventQueue.Instance.AddEvent(new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Result));
                        return;
                    }
                    else
                    {
                        player_2.IsCongratulate = true;
                    }
                    
                }
                else if (!player_2.IsAlive)
                {
                    if (player_2.TimeForAgony <= 0)
                    {
                        GameResult.Instance.IsWin = true;
                        GameResult.Instance.WinnerPlayer = 1;
                        EventQueue.Instance.AddEvent(new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Result));
                        return;
                    }
                    else
                    {
                        player.IsCongratulate = true;
                    }
                }
                else
                {
                    CheckPlayerMoving(player);
                    CheckPlayerMoving(player_2, 2);

                    CheckSetBomb(player);
                    CheckSetBomb(player_2, 2);


                    foreach (var bomb in bombs) // Kiểm tra xem bom có nổ chết ai ko
                    {
                        if (bomb.State == Bomb.BombState.Explosion)
                        {
                            if (player.IsAlive && bomb.IsInExplosionArea(player.CurRect))
                            {
                                player.IsAlive = false;
                            }
                            if (player_2.IsAlive && bomb.IsInExplosionArea(player_2.CurRect))
                            {
                                player_2.IsAlive = false;
                            }
                        }

                    }
                }

                player.Update(gameTime);
                player_2.Update(gameTime);

            }
            else
            {
                bool isBotDeadAll = false;
                bool isPlayerDead = false;
                if (player.IsAlive) //Giết hết bot
                {
                    if (bots.Count == 0)
                    {
                        GameResult.Instance.IsWin = true;
                        EventQueue.Instance.AddEvent(new GameEvent(GameEvent.Type.SwitchView,
                            (int)GameUI.ViewType.Result));
                        return;
                    }
                    else if (bots.TrueForAll(bot => !bot.IsAlive))
                    {
                        isBotDeadAll = true;
                    }

                }
                else
                {
                    isPlayerDead = true;
                }

                if (isBotDeadAll)
                {
                    player.IsCongratulate = true;
                }
                else if (isPlayerDead)
                {
                    if (player.TimeForAgony <= 0)
                    {
                        GameResult.Instance.IsWin = false;
                        EventQueue.Instance.AddEvent(new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Result));
                        return;
                    }
                }
                else
                {
                    CheckSetBomb(player); //Kiểm tra xem nhân vật có lệnh đặt bom hay không

                    foreach (var b in bots)
                        CheckBotMoving(b);


                    //Kiểm tra nổ lan
                    foreach (var bomb in bombs) // Kiểm tra xem bom có nổ chết ai ko
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

                    CheckPlayerMoving(player); //Kiểm tra xem nhân vật có lệnh di chuyển hay không

                    foreach (var bomb in bombs) // Kiểm tra xem bom có nổ chết ai ko
                    {
                        if (bomb.State == Bomb.BombState.Explosion)
                        {
                            bots.ForEach(bot =>
                            {
                                if (bot.IsAlive && bomb.IsInExplosionArea(bot.CurRect)) //Bot bị dính bom
                                {
                                    bot.IsAlive = false; //Bắt đầu hấp hối
                                }
                            });
                            if (player.IsAlive && bomb.IsInExplosionArea(player.CurRect))
                            {
                                player.IsAlive = false;
                                bots.ForEach(bot =>
                                {
                                    if (bot.IsAlive)
                                    {
                                        bot.IsCongratulate = true;
                                    }
                                });
                            }
                        }

                    }

                    if (miniMap.IsEnabled)
                        miniMap.ApplyEntity(player, bots, bombs);
                }

                player.Update(gameTime);//Cập nhật trạng thái nhân vật
                var removeBots = new List<Character>(); //Danh sách hỏa thiêu
                foreach (var b in bots)
                {
                    b.Update(gameTime); //Cập nhật trạng thái bot
                    if (!b.IsAlive && b.TimeForAgony <= 0) //Hết thời gian hấp hối, cho vào danh sách cần hỏa thiêu
                    {
                        removeBots.Add(b);
                    }
                }

                foreach (var removeBot in removeBots) //Hỏa thiêu những nhân vật đã hấp hối xong
                {
                    bots.Remove(removeBot);
                }

            }

            foreach (var bomb in bombs) //Cập nhật trạng thái của quả bom
            {
                bomb.Update(gameTime);
            }

            bombs.Remove(bombs.Find(bomb => bomb.State == Bomb.BombState.End));

            //Kiểm tra nổ lan
            foreach (var bomb in bombs) // Kiểm tra xem bom có nổ chết ai ko
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

        }

        private void CheckBotMoving(Character bot)
        {
            if (bot.IsAlive && !bot.IsMoving)
            {
                int movementIndex = Character.IDLE; //Hướng di chuyển của nhân vật (mặc định là đứng yên)
                int move;

                int characterI = bot.I;
                int characterJ = bot.J;

                move = AImove(bot);
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

                if (movementIndex != -1 && mapGenerator.IsValidLocation(characterI, characterJ) && !bombs.Any(bomb => bomb.State != Bomb.BombState.End && bomb.I == characterI && bomb.J == characterJ)) //Nếu nhân vật có lệnh di chuyển và vị trí mới hợp lệ
                {
                    if (CheckValidBotPosition(bot, characterI, characterJ))
                        bot.Move(movementIndex); //Di chuyển nhân vật
                }
            }
        }

        private void CheckPlayerMoving(Character character, int playerIndex = 1)
        {
            if (character.IsAlive && !character.IsMoving) //Nếu nhân vật đang trong quá trình di chuyển ( 1 lần 1 ô ) thì sẽ skip các lệnh di chuyển
            {
                var keyboard = KeyboardEvent.Instance;
                int movementIndex = Character.IDLE; //Hướng di chuyển của nhân vật (mặc định là đứng yên)
                int move;

                int characterI = character.I;
                int characterJ = character.J;

                if (playerIndex == 1) //Đây là người chơi 1
                {

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
                else //Đây là người chơi 2
                {
                    if (keyboard.IsKeyDown(Keys.W))
                    {
                        characterI--;
                        movementIndex = Character.MOVE_UP;
                    }
                    else if (keyboard.IsKeyDown(Keys.S))
                    {
                        characterI++;
                        movementIndex = Character.MOVE_DOWN;
                    }
                    else if (keyboard.IsKeyDown(Keys.A))
                    {
                        characterJ--;
                        movementIndex = Character.MOVE_LEFT;
                    }
                    else if (keyboard.IsKeyDown(Keys.D))
                    {
                        characterJ++;
                        movementIndex = Character.MOVE_RIGHT;
                    }

                }


                if (movementIndex != -1 && mapGenerator.IsValidLocation(characterI, characterJ) && !bombs.Any(bomb => bomb.State != Bomb.BombState.End && bomb.I == characterI && bomb.J == characterJ)) //Nếu nhân vật có lệnh di chuyển và vị trí mới hợp lệ
                {
                    character.Move(movementIndex); //Di chuyển nhân vật
                }
            }

        }

        public bool IsPossibleMove(int i, int j)
        {
            return mapGenerator.IsValidLocation(i, j) &&
                   !bombs.Any(bomb => bomb.CountDownTime < 1000 && bomb.ExplosionArea.Any(area => area.X == i && area.Y == j));
        }

        public bool IsValidMove(int i, int j)
        {
            return mapGenerator.IsValidLocation(i, j);
        }

        private bool CheckValidBotPosition(Character curBot, int newI, int newJ)
        {
            return !bots.Any(bot => !bot.Equals(curBot) && bot.IsAlive && bot.NewI == newI && bot.NewJ == newJ);
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
                switch (RoomSetting.Instance.MapSize)
                {
                    case 21:
                        return findSafePlace(c, 10);
                    case 31:
                        return findSafePlace(c, 11);
                    case 51:
                        return findSafePlace(c, 12);
                }

            }

            //khi khoảng cách 2 bên quá xa thì thu hẹp khoảng cách
            int d = rand.Next(1, 3);
            if (distance > d)
            {
                if (RoomSetting.Instance.MapSize <= 31)
                {
                    ANode pos = astar.FindPath(this, new Vector2(c.J, c.I), new Vector2(player.J, player.I));
                    if (pos != null)
                        dir = nextCell(pos, c);
                    else
                    {
                        int[] possibleMove = findPossibleMove(c);
                        dir = findNextMove(possibleMove, c, player.I, player.J);
                    }
                }
                else
                {
                    int[] possibleMove = findPossibleMove(c);
                    dir = findNextMove(possibleMove, c, player.I, player.J);
                }
            }
            //khi gần rồi thì tấn công
            else
            {
                if (player.I == c.I || player.J == c.J)
                {
                    BotSetBomb(c);
                }
            }

            return dir;
        }

        public int nextCell(ANode pos, Character c)
        {
            if (pos.Position.X - c.J > 0)
                return 3;
            else if (pos.Position.X - c.J < 0)
                return 2;
            else if (pos.Position.Y - c.I > 0)
                return 1;
            else if (pos.Position.Y - c.I < 0)
                return 0;
            else
                return -1;
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
                if (RoomSetting.Instance.MapSize <= 31)
                {
                    ANode pos = astar.FindPath(this, new Vector2(c.J, c.I), new Vector2(tempJ, tempI));
                    if (pos != null)
                        return nextCell(pos, c);
                    return findNextMove(findPossibleMove(c, true), c, tempI, tempJ);
                }
                return findNextMove(findPossibleMove(c, true), c, tempI, tempJ);
            }
            return -1;
        }

        private int findNextMove(int[] possibleMove, Character c, int destI, int destJ)
        {
            int shortest, tempI, tempJ;
            shortest = 99999;

            int dir = -1, distance = 0;
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

        private void CheckSetBomb(Character character, int playerIndex = 1) //Kiểm tra xem nhân vật character có đặt bom k, nếu có thì thêm vào danh sách bom
        {
            var keyboard = KeyboardEvent.Instance;
            if ((!isMultiplayer && keyboard.IsPressed(Keys.Space)) ||
                (isMultiplayer && ((playerIndex == 1 && keyboard.IsPressed(Keys.RightShift)) || (playerIndex == 2 && keyboard.IsPressed(Keys.LeftShift)))))
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
                explosionArea.Add(new Point(currentCoordI, currentCoordJ));

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

        public void BotSetBomb(Character bot)
        {
            int currentCoordI = bot.I;
            int currentCoordJ = bot.J;

            //Các giới hạn cho phạm vi nổ của bom ---------------------------------------------------
            var leftLimit = Rectangle.Empty;
            var rightLimit = Rectangle.Empty;
            var topLimit = Rectangle.Empty;
            var bottomLimit = Rectangle.Empty;

            var explosionArea = new List<Point>();
            //---------------------------------------------------------------------------------------
            explosionArea.Add(new Point(currentCoordI, currentCoordJ));

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
