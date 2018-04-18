using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;



namespace _2D_Platformer
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }



        protected override void Initialize()
        {
            base.Initialize();
        }



        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _2D_Platformer.StateManager.CreateState("SPLASH", new SplashState());
            _2D_Platformer.StateManager.CreateState("GAME", new GameState(this));
            _2D_Platformer.StateManager.CreateState("GAMEOVER", new GameOverState());

            _2D_Platformer.StateManager.PushState("SPLASH");
        }



        protected override void UnloadContent()
        {
            
        }



        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _2D_Platformer.StateManager.Update(Content, gameTime);

            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.GhostWhite);

            _2D_Platformer.StateManager.Draw(spriteBatch);                        spriteBatch.Begin();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
