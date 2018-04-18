using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace _2D_Platformer
{
    public class SplashState : _2D_Platformer.State
    {
        SpriteFont font = null;
        float timer = 5;

        public SplashState() : base()
        {

        }

        public override void Update(ContentManager Content, GameTime gameTime)
        {
            if (font == null)
            {
                font = Content.Load<SpriteFont>("Arial");
            }

            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer <= 0 || Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                _2D_Platformer.StateManager.ChangeState("GAME");
                timer = 5;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Splash", new Vector2(200, 200), Color.OrangeRed);
            spriteBatch.End();
        }

        public override void CleanUp()
        {
            font = null;
            timer = 3;
        }
    }
}