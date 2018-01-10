using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using BoomOffline.Helper;
using BoomOffline.Model;
using BoomOffline.Resource;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.Entity
{
    class Bomb : IModel
    {
        public enum BombState
        {
            CountDown,
            Explosion,
            End
        }


        private Point[] explosionLogicArea;
        private BombState state;
        private int i, j;
        private Rectangle rect;
        private Texture2D texture;

        private Texture2D explosionTexture;
        private List<Rectangle> explosionArea;
        private int explosionLevel;

        private AnimationSprite sprite;
        private int elapsedGameTime;
        private int countDownTime;
        private int explosionLevelIncreaseTime;

        private bool canExplosionToLeft;
        private bool canExplosionToRight;
        private bool canExplosionToTop;
        private bool canExplosionToBottom;

        private Rectangle leftLimit;
        private Rectangle rightLimit;
        private Rectangle topLimit;
        private Rectangle bottomLimit;

        public Bomb(int i, int j, Rectangle rect, Rectangle leftLimit, Rectangle rightLimit, Rectangle topLimit, Rectangle bottomLimit, Point[] explosionLogicArea)
        {
            this.ExplosionArea = explosionLogicArea;
            this.i = i;
            this.j = j;
            canExplosionToTop = canExplosionToBottom = canExplosionToLeft = canExplosionToRight = true;
            explosionArea = new List<Rectangle>();
            var unit = Global.Instance.GameUnit;
            var explosionOffset = unit / 2;
            explosionTexture = ResManager.Instance.Explosion;
            explosionLevelIncreaseTime = 25;
            explosionLevel = 1;
            countDownTime = 3000;
            this.state = BombState.CountDown;
            this.Rect = rect;
            this.leftLimit = leftLimit;
            this.rightLimit = rightLimit;
            this.topLimit = topLimit;
            this.bottomLimit = bottomLimit;
            this.texture = ResManager.Instance.Bomb;
            sprite = new AnimationSprite();
            sprite.Load(this.texture, this.texture.Width / 8, this.texture.Height, 8, 0);



        }

        public BombState State
        {
            get { return state; }
            set
            {
                this.state = value;
                if (value == BombState.Explosion)
                {
                    countDownTime = 0;
                    elapsedGameTime = 0;
                    state = BombState.Explosion;
                    Explose();
                }
            }
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

        public Point[] ExplosionArea
        {
            get { return explosionLogicArea; }
            set { explosionLogicArea = value; }
        }

        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }

        public bool IsInExplosionArea(Rectangle rect)
        {
            rect.Inflate(-5, -5);
            return rect.Intersects(this.Rect) || explosionArea.Any(explosion => explosion.Intersects(rect));
        }

        public void Update(GameTime gameTime)
        {
            switch (state)
            {
                case BombState.CountDown:
                    elapsedGameTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                    countDownTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (elapsedGameTime > 25)
                    {
                        elapsedGameTime = 0;
                        sprite.Next();
                    }
                    if (countDownTime <= 0)
                    {
                        elapsedGameTime = 0;
                        state = BombState.Explosion;
                        Explose();
                    }
                    break;
                case BombState.Explosion:
                    if (explosionLevel == 10)
                    {
                        state = BombState.End;
                        break;
                    }
                    explosionLevelIncreaseTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (explosionLevelIncreaseTime <= 0)
                    {
                        explosionLevelIncreaseTime = 25;
                        explosionLevel++;
                        Explose();
                    }

                    break;
                case BombState.End:
                    break;
            }

        }

        private void Explose()
        {
            var unit = Global.Instance.GameUnit;
            var explosionOffset = unit / 2;
            if (canExplosionToRight)
            {
                var newExplosion = Rect;
                newExplosion.Offset(explosionLevel * explosionOffset, 0);
                if (!newExplosion.Intersects(rightLimit))
                    explosionArea.Add(newExplosion);
                else
                    canExplosionToRight = false;
            }
            if (canExplosionToLeft)
            {
                var newExplosion = Rect;
                newExplosion.Offset(-explosionLevel * explosionOffset, 0);
                if (!newExplosion.Intersects(leftLimit))
                    explosionArea.Add(newExplosion);
                else
                    canExplosionToLeft = false;
            }
            if (canExplosionToTop)
            {
                var newExplosion = Rect;
                newExplosion.Offset(0, -explosionLevel * explosionOffset);
                if (!newExplosion.Intersects(topLimit))
                    explosionArea.Add(newExplosion);
                else
                    canExplosionToTop = false;
            }
            if (canExplosionToBottom)
            {
                var newExplosion = Rect;
                newExplosion.Offset(0, explosionLevel * explosionOffset);
                if (!newExplosion.Intersects(bottomLimit))
                    explosionArea.Add(newExplosion);
                else
                    canExplosionToBottom = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (state)
            {
                case BombState.CountDown:
                    spriteBatch.Draw(texture, Rect, sprite.Frame, Color.White);
                    break;
                case BombState.Explosion:
                    spriteBatch.Draw(explosionTexture, Rect, Color.White);
                    foreach (var explosionGameUnit in explosionArea)
                    {
                        spriteBatch.Draw(explosionTexture, explosionGameUnit, Color.White);
                    }
                    break;
            }

        }
    }
}
