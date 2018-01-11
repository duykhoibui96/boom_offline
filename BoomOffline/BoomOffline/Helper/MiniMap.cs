using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using BoomOffline.Entity;
using BoomOffline.Model;
using BoomOffline.Resource;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.Helper
{
    class MiniMap : IModel
    {
        private BasicEntity[] map;
        private BasicEntity playerCharacter;
        private BasicEntity[] bots;
        private BasicEntity[] bombs;

        private Texture2D wall;
        private Texture2D empty;
        private Texture2D player;
        private Texture2D bot;
        private Texture2D bomb;

        private int cellUnit;
        private int startX;
        private int startY;

        private bool isEnabled;

        public MiniMap()
        {
            cellUnit = 5;
            var graphics = Global.Instance.Graphics;
            wall = new Texture2D(graphics, 1, 1);
            empty = new Texture2D(graphics, 1, 1);
            player = new Texture2D(graphics, 1, 1);
            bot = new Texture2D(graphics, 1, 1);
            bomb = new Texture2D(graphics, 1, 1);

            wall.SetData(new Color[] { Color.Gray });
            empty.SetData(new Color[] { Color.Green });
            player.SetData(new Color[] { Color.Blue });
            bot.SetData(new Color[] { Color.Red });
            bomb.SetData(new Color[] { Color.Black });

            playerCharacter = new BasicEntity();


        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        public void Load(int[][] logicMap, int mapSize)
        {
            isEnabled = true;
            playerCharacter.Load(player, Rectangle.Empty, Color.White);
            var viewPort = Global.Instance.Graphics.Viewport;
            startX = viewPort.Width - cellUnit * mapSize;
            startY = viewPort.Height - cellUnit * mapSize;
            var map = new List<BasicEntity>();

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    var entity = new BasicEntity();
                    entity.Load(logicMap[i][j] == MapGenerator.TYPE_OBSTACLE ? wall : empty, Refractor(i, j), Color.White);
                    map.Add(entity);
                }
            }
            this.map = map.ToArray();
        }

        private Rectangle Refractor(int i, int j)
        {
            return new Rectangle(startX + cellUnit * j, startY + cellUnit * i, cellUnit, cellUnit);
        }


        public void Update(GameTime gameTime)
        {

        }

        public void ApplyEntity(Character player, List<Character> bots, List<Bomb> bombs)
        {
            playerCharacter.Rect = Refractor(player.NewI, player.NewJ);
            this.bots = bots.Select(curBot =>
            {
                var botEntity = new BasicEntity();
                botEntity.Load(bot, Refractor(curBot.NewI, curBot.NewJ), Color.White);
                return botEntity;

            }).ToArray();
            this.bombs = bombs.Select(curBom =>
            {
                var botEntity = new BasicEntity();
                botEntity.Load(bomb, Refractor(curBom.I, curBom.J), Color.White);
                return botEntity;

            }).ToArray();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var mapCell in map)
            {
                mapCell.Draw(spriteBatch);
            }

            playerCharacter.Draw(spriteBatch);

            foreach (var bot in bots)
            {
                bot.Draw(spriteBatch);
            }

            foreach (var bomb in bombs)
            {
                bomb.Draw(spriteBatch);
            }
        }
    }
}
