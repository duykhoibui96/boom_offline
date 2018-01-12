using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BoomOffline.Event;
using BoomOffline.Helper;
using BoomOffline.Input;
using BoomOffline.Resource;
using BoomOffline.Sound;
using BoomOffline.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BoomOffline
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private GameUI disabledUI;
        private GameUI dialog;
        private GameUI gameUI;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 670
            };
            //IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Global.Instance.Graphics = GraphicsDevice;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ResManager.Instance.Load(Content);
            SoundManager.Instance.PlayBackgroundMusic();
            gameUI = new Menu();
            gameUI.Load();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            HandledEvent();
            // TODO: Add your update logic here
            MouseEvent.Instance.Update();
            KeyboardEvent.Instance.Update();
            if (dialog == null)
                gameUI.Update(gameTime);
            else
                dialog.Update(gameTime);
            base.Update(gameTime);
        }

        private void HandledEvent()
        {
            var ev = EventQueue.Instance.CheckCurrentEvent();

            if (ev != null)
            {
                if (ev.EventType == GameEvent.Type.Exit)
                {
                    this.Exit();
                }
                else if (ev.EventType == GameEvent.Type.SwitchView)
                {
                    switch (ev.Param)
                    {
                        case (int)GameUI.ViewType.Menu:
                            gameUI = new Menu();
                            break;
                        case (int)GameUI.ViewType.Room:
                            MatchStorage.Instance.NeedToLoadDataHere = false;
                            MatchStorage.Instance.Clear();
                            gameUI = new Room();
                            break;
                        case (int)GameUI.ViewType.Match:
                            gameUI = new Match();
                            break;
                        case (int)GameUI.ViewType.Loading:
                            gameUI = new Loading();
                            break;
                        case (int)GameUI.ViewType.Setting:
                            disabledUI = gameUI;
                            gameUI = new Setting();
                            break;
                        case (int)GameUI.ViewType.Result:
                            gameUI = new Result();
                            break;
                    }
                    gameUI.Load();
                    EventQueue.Instance.Next();
                }
                else if (ev.EventType == GameEvent.Type.NewGame)
                {
                    if (MatchStorage.Instance.DataAvailable)
                    {
                        EventQueue.Instance.AddEvent(new GameEvent(GameEvent.Type.OpenDialog, 1,
                            "You will lose your saved data! Continue?"));
                        EventQueue.Instance.NextEvent = new GameEvent(GameEvent.Type.SwitchView,
                            (int)GameUI.ViewType.Room);
                    }
                    else
                    {
                        EventQueue.Instance.AddEvent(new GameEvent(GameEvent.Type.SwitchView,
                            (int)GameUI.ViewType.Room));
                    }

                    EventQueue.Instance.Next();
                }
                else if (ev.EventType == GameEvent.Type.WantToExit)
                {
                    EventQueue.Instance.AddEvent(new GameEvent(GameEvent.Type.OpenDialog, 1,
                        "Would you like to exit?"));
                    EventQueue.Instance.NextEvent = new GameEvent(GameEvent.Type.Exit);
                    EventQueue.Instance.Next();
                }
                else if (ev.EventType == GameEvent.Type.ResumeView)
                {
                    gameUI = disabledUI;
                    EventQueue.Instance.Next();
                }
                else if (ev.EventType == GameEvent.Type.Continue)
                {
                    MatchStorage.Instance.NeedToLoadDataHere = true;
                    EventQueue.Instance.Next();
                    EventQueue.Instance.AddEvent(new GameEvent(GameEvent.Type.SwitchView, (int)GameUI.ViewType.Loading));
                }
                else if (ev.EventType == GameEvent.Type.OpenDialog)
                {
                    if (ev.Param == 0)
                        dialog = new NotificationDialog(ev.Param2);
                    else
                        dialog = new ConfirmationDialog(ev.Param2);
                    dialog.Load();
                    EventQueue.Instance.Next();
                }
                else if (ev.EventType == GameEvent.Type.DismissDialog)
                {
                    dialog = null;
                    if (ev.Param == 0)
                    {

                    }
                    else
                    {
                        if (ev.Param2 == "yes")
                        {
                            EventQueue.Instance.AddEvent(EventQueue.Instance.NextEvent);
                        }
                        else
                        {
                            EventQueue.Instance.NextEvent = null;
                        }
                    }
                    EventQueue.Instance.Next();
                }
            }

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            gameUI.Draw(spriteBatch);
            if (dialog != null)
            {
                dialog.Draw(spriteBatch);
            }
            MouseEvent.Instance.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
