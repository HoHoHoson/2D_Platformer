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
                    GameState.score = 0;
                    isLoaded = false;
                }
            }
            oldState = newState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "GAME OVER", new Vector2(200, 200), Color.OrangeRed);
            spriteBatch.DrawString(font, "Score : " + GameState.score.ToString() + "/10", new Vector2(200, 240), Color.OrangeRed);
            if (GameState.score == 10)
                spriteBatch.DrawString(font, "Perfect! You were born to be a hero! :D", new Vector2(200, 270), Color.OrangeRed);
            if (GameState.score < 10 && GameState.lives > 0)
                spriteBatch.DrawString(font, "Congrats on the loot! The revives were expensive though...", new Vector2(200, 270), Color.OrangeRed);
            if (GameState.lives <= 0)
                spriteBatch.DrawString(font, "Rest in Pieces x_x", new Vector2(200, 270), Color.OrangeRed);
            spriteBatch.DrawString(font, "Retry (Enter)", new Vector2(200, 460), Color.OrangeRed);
            spriteBatch.DrawString(font, "Quit (Esc)", new Vector2(450, 460), Color.OrangeRed);
            spriteBatch.End();
        }

        public override void CleanUp()
        {
            font = null;
            isLoaded = false;
        }
    }
}
