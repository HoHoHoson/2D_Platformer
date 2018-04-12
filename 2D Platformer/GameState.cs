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
    public class GameState : _2D_Platformer.State
    {
        bool isLoaded = false;
        SpriteFont font = null;

        public GameState() : base()
        {

        }

        public override void Update(ContentManager Content, GameTime gameTime)
        {
            if (isLoaded == false)
            {
                isLoaded = true;
                font = Content.Load<SpriteFont>("Arial");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                _2D_Platformer.StateManager.ChangeState("GAMEOVER");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Game State", new Vector2(200, 200), Color.White);
            spriteBatch.End();
        }

        public override void CleanUp()
        {
            font = null;
            isLoaded = false;
        }
    }
}
