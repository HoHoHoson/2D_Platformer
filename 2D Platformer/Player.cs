using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace _2D_Platformer
{
    class Player
    {
        Sprite playerSprite = new Sprite();

        Game1 game = null;
        bool isFalling = true;
        bool isJumping = false;
        bool autoJump = false;

        Vector2 velocity = Vector2.Zero;
        Vector2 position = Vector2.Zero;

        SoundEffect jumpSound;
        SoundEffectInstance jumpSoundInstance;
        SoundEffect playerDeathSound;
        SoundEffectInstance playerDeathSoundInstance;

        public Vector2 Position
        {
            get
            {
                return playerSprite.position;
            }
        }

        public Vector2 Respawn
        {
            get
            {
                return playerSprite.position = new Vector2(70, 950);
            }
        }

        public Player(Game1 game)
        {
            this.game = game;
            isFalling = true;
            isJumping = false;
            velocity = Vector2.Zero;
            position = Vector2.Zero;
        }

        public void Load(ContentManager content)
        {
            AnimatedTexture animation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
            animation.Load(content, "walk", 12, 20);

            jumpSound = content.Load<SoundEffect>("Jump");
            jumpSoundInstance = jumpSound.CreateInstance();
            playerDeathSound = content.Load<SoundEffect>("player_death");
            playerDeathSoundInstance = playerDeathSound.CreateInstance();

            playerSprite.Add(animation, 0, -5);

            playerSprite.position = Respawn;
        }

        public void Update(float deltaTime)
        {
            playerSprite.Update(deltaTime);
            UpdateInput(deltaTime);
        }

        private void UpdateInput (float deltaTime)
        {
            bool wasMovingLeft = velocity.X < 0;
            bool wasMovingRight = velocity.X > 0;
            bool falling = isFalling;

            Vector2 acceleration = new Vector2(0, Game1.gravity);

            if (Keyboard.GetState().IsKeyDown(Keys.A) == true)
            {
                acceleration.X -= Game1.acceleration;
                playerSprite.SetFlipped(true);
                playerSprite.Play();
            }
            else if (wasMovingLeft == true)
            {
                acceleration.X += Game1.friction;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D) == true)
            {
                acceleration.X += Game1.acceleration;
                playerSprite.SetFlipped(false);
                playerSprite.Play();
            }
            else if (wasMovingRight == true)
            {
                acceleration.X -= Game1.friction;
            }

            if ((Keyboard.GetState().IsKeyDown(Keys.Space) == true && this.isJumping == false && falling == false) || autoJump == true)
            {
                acceleration.Y -= Game1.jumpImpulse;
                this.isJumping = true;
                autoJump = false;
                jumpSoundInstance.Play();
            }

            velocity += acceleration * deltaTime;

            velocity.X = MathHelper.Clamp(velocity.X, -Game1.maxVelocity.X, Game1.maxVelocity.X);
            velocity.Y = MathHelper.Clamp(velocity.Y, -Game1.maxVelocity.Y, Game1.maxVelocity.Y);

            playerSprite.position += velocity * deltaTime;

            if (velocity.X == 0)
            {
                playerSprite.Stop();
            }

            if ((wasMovingLeft && (velocity.X > 0)) || (wasMovingRight && (velocity.X < 0)))
            {
                velocity.X = 0;
            }

            int tx = game.PixelToTile(playerSprite.position.X);
            int ty = game.PixelToTile(playerSprite.position.Y);

            bool nx = (playerSprite.position.X) % Game1.tile != 0;
            bool ny = (playerSprite.position.Y) % Game1.tile != 0;
            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;
            bool spike = game.SpikeAtTileCoord(tx, ty) != 0;
            bool spikeright = game.SpikeAtTileCoord(tx + 1, ty) != 0;
            bool spikedown = game.SpikeAtTileCoord(tx, ty + 1) != 0;
            bool spikediag = game.SpikeAtTileCoord(tx + 1, ty + 1) != 0;

            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    playerSprite.position.Y = game.TileToPixel(ty);
                    this.velocity.Y = 0;
                    this.isFalling = false;
                    this.isJumping = false;
                    ny = false;
                }
                if ((spikedown && !spike) || (spikediag && !spikeright && nx))
                {
                    playerSprite.position = Respawn;
                    Game1.lives -= 1;
                    playerDeathSoundInstance.Play();
                }
            }
            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    playerSprite.position.Y = game.TileToPixel(ty + 1);
                    this.velocity.Y = 0;
                    cell = celldown;
                    cellright = celldiag;
                    ny = false;
                }
                if ((spike && !spikedown) || (spikeright && !spikediag && nx))
                {
                    playerSprite.position = Respawn;
                    Game1.lives -= 1;
                    playerDeathSoundInstance.Play();
                }
            }

            if (this.velocity.X > 0)
            {
                if ((cellright && !cell) || (celldiag && !celldown && ny))
                {
                    playerSprite.position.X = game.TileToPixel(tx);
                    this.velocity.X = 0;
                    playerSprite.Pause();
                }
                if ((spikeright && !spike) || (spikediag && !spikedown && ny))
                {
                    playerSprite.position = Respawn;
                    Game1.lives -= 1;
                    playerDeathSoundInstance.Play();
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    playerSprite.position.X = game.TileToPixel(tx + 1);
                    this.velocity.X = 0;
                    playerSprite.Pause();
                }
                if ((spike && !spikeright) || (spikedown && !spikediag && ny))
                {
                    playerSprite.position = Respawn;
                    Game1.lives -= 1;
                    playerDeathSoundInstance.Play();
                }
            }

            this.isFalling = !(celldown || (nx && celldiag));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            playerSprite.Draw(spriteBatch);
        }

        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return playerSprite.Bounds;
            }
        }

        public bool IsFalling
        {
            get
            {
                return isFalling;
            }
        }

        public void JumpOnCollision()
        {
            autoJump = true;
        }

        public SoundEffectInstance PlayerDeath
        {
            get
            {
                return playerDeathSoundInstance;
            }
        }
    }
}
