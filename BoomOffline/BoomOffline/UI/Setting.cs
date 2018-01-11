using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;
using BoomOffline.Entity;
using BoomOffline.Event;
using BoomOffline.Input;
using BoomOffline.Model;
using BoomOffline.Resource;
using BoomOffline.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.UI
{
    class Setting : GameUI
    {
        private class VolumnControl : IModel
        {
            public enum Type
            {
                Music = 0,
                Sound = 1
            }

            private Type type;
            private float vol;
            private TextEntity title;
            private BasicEntity progress;
            private BasicEntity control;
            private Point startPos;
            private Point endPos;

            public VolumnControl()
            {
                title = new TextEntity();
                progress = new BasicEntity();
                control = new BasicEntity();
            }

            public void Load(int startX, int startY, Type type)
            {
                this.type = type;
                var unit = Global.Instance.Unit;
                var graphics = Global.Instance.Graphics;
                startPos = new Point(startX + unit * 5, startY);
                var progressTexture = new Texture2D(graphics, 1, 1);
                endPos = new Point(startPos.X + unit * 10, startPos.Y);
                progressTexture.SetData(new Color[]
                {
                    Color.Red
                });
                string titleText = "";
                switch (type)
                {
                    case Type.Music:
                        vol = SoundManager.Instance.MusicVolumn;
                        titleText = "Music volumn";
                        break;
                    case Type.Sound:
                        vol = SoundManager.Instance.SoundVolumn;
                        titleText = "Sound volumn";
                        break;
                }
                var progressLength = (int)(vol * unit * 10);
                title.Load(titleText, ResManager.Instance.Font_1, new Vector2(startX, startY), Color.Blue);
                progress.Load(progressTexture, new Rectangle(this.startPos.X, this.startPos.Y + unit / 4, progressLength, unit / 2), Color.White);
                control.Load(ResManager.Instance.Control, new Rectangle(this.startPos.X + progressLength, this.startPos.Y, unit, unit), Color.White);
            }

            public void Update(GameTime gameTime)
            {
                var mouse = MouseEvent.Instance;
                if (mouse.IsHoldLeftMouse && control.Rect.Contains(mouse.currentMouseState.X, mouse.currentMouseState.Y))
                {
                    var unit = Global.Instance.Unit;
                    var currentMousePosX = mouse.currentMouseState.X;
                    var previousMousePosX = mouse.previousState.X;
                    var offset = currentMousePosX - previousMousePosX;
                    var controlRect = control.Rect;
                    var progressRect = progress.Rect;
                    controlRect.X += offset;
                    if (controlRect.X < startPos.X || controlRect.X > endPos.X) return;
                    progressRect.Width = controlRect.X - startPos.X;
                    control.Rect = controlRect;
                    progress.Rect = progressRect;
                    vol = (float)progressRect.Width / (unit * 10);
                    Synchronize();
                }

            }

            private void Synchronize()
            {
                switch (type)
                {
                    case Type.Music:
                        SoundManager.Instance.MusicVolumn = vol;
                        break;
                    case Type.Sound:
                        SoundManager.Instance.SoundVolumn = vol;
                        break;
                }
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                title.Draw(spriteBatch);
                progress.Draw(spriteBatch);
                control.Draw(spriteBatch);
            }

        }

        private VolumnControl musicControl;
        private VolumnControl soundControl;
        private Button apply;
        private Button cancel;


        public Setting()
        {
            musicControl = new VolumnControl();
            soundControl = new VolumnControl();
            apply = new Button();
            cancel = new Button();
        }

        public override void Load()
        {
            SoundManager.Instance.Backup();
            musicControl.Load(100, 100, VolumnControl.Type.Music);
            soundControl.Load(100, 200, VolumnControl.Type.Sound);
            var viewPort = Global.Instance.Graphics.Viewport;
            var unit = Global.Instance.Unit;
            apply.Load("APPLY", (viewPort.Width - unit * 11) / 2, 300, new GameEvent(GameEvent.Type.ApplySetting));
            cancel.Load("CANCEL", (viewPort.Width - unit * 11) / 2 + unit * 6, 300, new GameEvent(GameEvent.Type.CancelSetting));
        }

        public override void HandleEvent()
        {
            var ev = EventQueue.Instance.CheckCurrentEvent();

            if (ev != null)
            {
                bool isEventTriggered = true;
                switch (ev.EventType)
                {
                    case GameEvent.Type.ApplySetting:
                        break;
                    case GameEvent.Type.CancelSetting:
                        SoundManager.Instance.Recover();
                        break;
                    default:
                        isEventTriggered = false;
                        break;
                }

                if (isEventTriggered)
                {
                    EventQueue.Instance.AddEvent(new GameEvent(GameEvent.Type.ResumeView));
                    EventQueue.Instance.Next();
                }
                    
            }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            musicControl.Update(gameTime);
            soundControl.Update(gameTime);
            apply.Update(gameTime);
            cancel.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            musicControl.Draw(spriteBatch);
            soundControl.Draw(spriteBatch);
            apply.Draw(spriteBatch);
            cancel.Draw(spriteBatch);
        }
    }
}
