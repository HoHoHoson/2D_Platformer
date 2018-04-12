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
    public class GameOverState : _2D_Platformer.State
    {
        bool isLoaded = false;
        SpriteFont font = null;

        KeyboardState oldState;

        public GameOverState() : base()
        {

        }

        public override void Update(ContentManager Content, GameTime gameTime)
        {
            if (isLoaded == false)
            {
                isLoaded = true;
                font = Content.Load<SpriteFont>("Arial");
                oldState = Keyboard.GetState();
            }

            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Enter) == true)
            {
                if (oldState.IsKeyDown(Keys.Enter) == false)
                {
                    _2D_Platformer.StateManager.ChangeState("SPLASH");
                }
            }
            oldState = newState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Game Over...", new Vector2(200, 200), Color.White);
            spriteBatch.End();
        }

        public override void CleanUp()
        {
            font = null;
            isLoaded = false;
        }
    }
}
