using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended.ViewportAdapters;


namespace _2D_Platformer
{
    public class GameState : _2D_Platformer.State
    {
        Game1 game1 = null;
        bool isLoaded = false;
        SpriteFont font = null;
        Player player = null;
        List<Enemy> enemies = new List<Enemy>();
        Sprite goal = null;
        Sprite keys = null;

        SoundEffect gameMusic;
        SoundEffectInstance gameMusicInstance;
        SoundEffect zombieDeath;
        SoundEffectInstance zombieDeathInstance;
        SoundEffect keyJingle;
        SoundEffectInstance keyJingleInstance;
        SoundEffect chestOpen;
        SoundEffectInstance chestOpenInstance;

        Camera2D camera = null;
        TiledMap map = null;
        TiledMapRenderer mapRenderer = null;
        TiledMapTileLayer collisionLayer;
        TiledMapTileLayer spikeLayer;

        SpriteFont arialFont;
        public static int score = 0;
        Texture2D heart = null;
        public static int lives = 3;
        Texture2D keyIcon = null;

        public static int tile = 64;
        public static float meter = tile;
        public static float gravity = meter * 9.8f * 7.0f;
        public static Vector2 maxVelocity = new Vector2(meter * 10, meter * 15);
        public static float acceleration = maxVelocity.X * 2;
        public static float friction = maxVelocity.X * 6;
        public static float jumpImpulse = meter * 1500;

        public static bool keyCollected = false;
        public static bool chestInteracted = false;
        public static bool keyLost = false;
        float timer = 0f;



        public GameState(Game1 game1) : base()
        {
            this.game1 = game1;
            player = new Player(this);
        }



        public override void Update(ContentManager Content, GameTime gameTime)
        {
            if (isLoaded == false)
            {
                font = Content.Load<SpriteFont>("Arial");

                var viewportAdapter = new BoxingViewportAdapter(game1.Window, game1.GraphicsDevice, ScreenWidth, ScreenHeight);
                camera = new Camera2D(viewportAdapter);
                camera.Position = new Vector2(0, ScreenHeight);

                map = Content.Load<TiledMap>("Project_Map");
                mapRenderer = new TiledMapRenderer(game1.GraphicsDevice);

                player.Load(Content);

                foreach (TiledMapTileLayer layer in map.TileLayers)
                {
                    if (layer.Name == "Collision")
                        collisionLayer = layer;
                    if (layer.Name == "Spikes")
                        spikeLayer = layer;
                }

                foreach (TiledMapObjectLayer layer in map.ObjectLayers)
                {
                    if (layer.Name == "Enemies")
                    {
                        foreach (TiledMapObject obj in layer.Objects)
                        {
                            Enemy enemy = new Enemy(this);
                            enemy.Load(Content);
                            enemy.Position = new Vector2(obj.Position.X, obj.Position.Y);
                            enemies.Add(enemy);
                        }
                    }
                    if (layer.Name == "Goal")
                    {
                        TiledMapObject obj = layer.Objects[0];
                        if (obj != null)
                        {
                            AnimatedTexture anim = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
                            anim.Load(Content, "chest", 1, 1);
                            goal = new Sprite();
                            goal.Add(anim, 0, 5);
                            goal.position = new Vector2(obj.Position.X, obj.Position.Y);
                        }
                    }
                    if (layer.Name == "Key")
                    {
                        TiledMapObject key = layer.Objects[0];
                        if (key != null)
                        {
                            AnimatedTexture keyAnim = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
                            keyAnim.Load(Content, "Key", 1, 1);
                            keys = new Sprite();
                            keys.Add(keyAnim, 0, 5);
                            keys.position = new Vector2(key.Position.X, key.Position.Y);
                        }
                    }
                }

                gameMusic = Content.Load<SoundEffect>("Superhero_violin_no_intro");
                gameMusicInstance = gameMusic.CreateInstance();
                gameMusicInstance.IsLooped = true;
                gameMusicInstance.Play();

                zombieDeath = Content.Load<SoundEffect>("zombie_death");
                zombieDeathInstance = zombieDeath.CreateInstance();
                keyJingle = Content.Load<SoundEffect>("key_collect");
                keyJingleInstance = keyJingle.CreateInstance();
                chestOpen = Content.Load<SoundEffect>("chest_opened");
                chestOpenInstance = chestOpen.CreateInstance();

                arialFont = Content.Load<SpriteFont>("Arial");
                heart = Content.Load<Texture2D>("heart");
                keyIcon = Content.Load<Texture2D>("Key - Icon");
                lives = 3;

                isLoaded = true;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.Update(deltaTime);
            camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);

            if (keyLost == true)
            {
                 timer += deltaTime;
                if (timer >= 3.0f)
                {
                    keyLost = false;
                    timer = 0f;
                }
            }

            if (score <= 0)
                score = 0;

            foreach (Enemy e in enemies)
            {
                e.Update(deltaTime);
            }

            CheckCollisions();

            if (lives <= 0 || keyCollected == true && chestInteracted == true)
            {
                if (keyCollected == true && chestInteracted == true)
                {
                    score += 3;
                    chestOpen.Play();
                }   
                _2D_Platformer.StateManager.ChangeState("GAMEOVER");
                enemies.Clear();
                gameMusicInstance.Stop();
                keyCollected = false;

                isLoaded = false;
            }
        }



        public override void Draw(SpriteBatch spriteBatch)
        {
            var viewMatrix = camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, ScreenWidth, ScreenHeight, 0, 0f, -1f);

            spriteBatch.Begin(transformMatrix: viewMatrix);

            mapRenderer.Draw(map, ref viewMatrix, ref projectionMatrix);
            player.Draw(spriteBatch);
            foreach (Enemy e in enemies)
            {
                e.Draw(spriteBatch);
            }
            goal.Draw(spriteBatch);
            if (keyCollected == false)
                keys.Draw(spriteBatch);
            if (keyCollected == false && chestInteracted == true)
            {
                spriteBatch.DrawString(arialFont, "Locked", new Vector2(goal.position.X - 10, goal.position.Y - 30), Color.OrangeRed);
            }
            if (keyLost == true)
            {
                spriteBatch.DrawString(arialFont, "Key Lost   :(", new Vector2(player.Position.X + 5, player.Position.Y - 30), Color.OrangeRed);
            }

            spriteBatch.End();

            spriteBatch.Begin();

            spriteBatch.DrawString(arialFont, "Score : " + score.ToString() + "/10", new Vector2(20, 20), Color.OrangeRed);
            for (int i = 0; i < lives; i++)
            {
                spriteBatch.Draw(heart, new Vector2(ScreenWidth / 2 + i * 20, 20), Color.White);
            }
            if (keyCollected == true)
                spriteBatch.Draw(keyIcon, new Vector2(20, 45), Color.White);

            spriteBatch.End();
        }



        public int PixelToTile(float pixelCoord)
        {
            return (int)Math.Floor(pixelCoord / tile);
        }



        public int TileToPixel(int tileCoord)
        {
            return tile * tileCoord;
        }



        public int CellAtPixelCoord(Vector2 pixelCoords)
        {
            if (pixelCoords.X < 0 || pixelCoords.X > map.WidthInPixels || pixelCoords.Y < 0)
                return 1;
            if (pixelCoords.Y > map.HeightInPixels)
                return 0;
            return CellAtTileCoord(PixelToTile(pixelCoords.X), PixelToTile(pixelCoords.Y));
        }



        public int CellAtTileCoord(int tx, int ty)
        {
            if (tx < 0 || tx >= map.Width || ty < 0)
                return 1;
            if (ty >= map.Height)
                return 0;

            TiledMapTile? tile;
            collisionLayer.TryGetTile(tx, ty, out tile);
            return tile.Value.GlobalIdentifier;
        }



        public int SpikeAtTileCoord(int tx, int ty)
        {
            TiledMapTile? tile;
            spikeLayer.TryGetTile(tx, ty, out tile);
            return tile.Value.GlobalIdentifier;
        }



        private void CheckCollisions()
        {
            foreach (Enemy e in enemies)
            {
                if (IsColliding(player.Bounds, e.Bounds) == true)
                {
                    if (player.IsFalling && player.Velocity.Y > 0)
                    {
                        player.JumpOnCollision();
                        enemies.Remove(e);
                        score += 1;
                        zombieDeathInstance.Play();
                        break;
                    }
                    else
                    {
                        lives -= 1;
                        GameState.score -= 2;
                        player.PlayerDeath.Play();
                        Vector2 playerRespawn = player.Respawn;
                        if (keyCollected == true)
                        {
                            keyCollected = false;
                            keyLost = true;
                        }
                    }
                }
            }
            if (IsColliding(player.Bounds, goal.Bounds) == true)
            {
                chestInteracted = true;
            }
            else
            {
                chestInteracted = false;
            }
            if (IsColliding(player.Bounds, keys.Bounds) == true && !keyCollected)
            {
                keyJingleInstance.Play();
                keyCollected = true;
            }
        }



        private bool IsColliding(Rectangle rect1, Rectangle rect2)
        {
            if (rect1.X + rect1.Width < rect2.X ||
            rect1.X > rect2.X + rect2.Width ||
            rect1.Y + rect1.Height < rect2.Y ||
            rect1.Y > rect2.Y + rect2.Height)
            {
                return false;
            }
            return true;
        }



        public int ScreenWidth
        {
            get
            {
                return game1.GraphicsDevice.Viewport.Width;
            }
        }



        public int ScreenHeight
        {
            get
            {
                return game1.GraphicsDevice.Viewport.Height;
            }
        }



        public override void CleanUp()
        {
            font = null;
            isLoaded = false;
            gameMusicInstance.Stop();
            score = 0;
        }
    }
}
