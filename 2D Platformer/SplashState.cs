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
        KeyboardState oldState;
        bool isLoaded = false;



        public SplashState() : base()
        {

        }


        public override void Update(ContentManager Content, GameTime gameTime)
        {
            if (isLoaded == false)
            {
                oldState = Keyboard.GetState();
                font = Content.Load<SpriteFont>("Arial");
                isLoaded = true;
            }
            KeyboardState newState = Keyboard.GetState();
            
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer <= 0 || newState.IsKeyDown(Keys.Enter) == true)
            {
                if (timer <=0 || oldState.IsKeyDown(Keys.Enter) == false)
                _2D_Platformer.StateManager.ChangeState("GAME");
                timer = 5;
                isLoaded = false;
            }
            newState = oldState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Splash", new Vector2(200, 200), Color.OrangeRed);
            spriteBatch.DrawString(font, "[INSERT A SOON TO BE AWESOME COMPANY'S LOGO HERE]", new Vector2(200, 240), Color.OrangeRed);
            spriteBatch.DrawString(font, "Skip (Enter)", new Vector2(200, 460), Color.OrangeRed);
            spriteBatch.DrawString(font, "Quit (Esc)", new Vector2(450, 460), Color.OrangeRed);
            spriteBatch.End();
        }

        public override void CleanUp()
        {
            font = null;
            timer = 3;
        }
    }
}