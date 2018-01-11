using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BoomOffline.Event;
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

        private GameUI gameUI;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 650
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
            gameUI.Update(gameTime);

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
                            gameUI = new Room();
                            break;
                        case (int)GameUI.ViewType.Match:
                            gameUI = new Match();
                            break;
                        case (int)GameUI.ViewType.Loading:
                            gameUI = new Loading();
                            break;
                        case (int)GameUI.ViewType.Setting:
                            gameUI = new Setting();
                            break;
                    }
                    gameUI.Load();
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
            MouseEvent.Instance.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
