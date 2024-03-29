﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Bricks
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        // All Entities present within this spectrum
        GameContent gameContent;
        private Wall wall;
        private Paddle paddle;
        private Ball ball;
        // private Ball staticBall;
        private GameBorder border;
        private int screenWidth = 0;
        private int screenHeight = 0;

        // Mouse and Keyboard controls
        private MouseState oldmouse;
        private KeyboardState oldkeyboard;

        // Components for the Ball
        private bool ReadytoserveBall = true;
        private int ballRemaining = 3;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameContent = new GameContent(Content);
            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            // set game to 502 * 700 or screen max if smaller
            if (screenWidth >= 510)
            {
                screenWidth = 510;
            }
            if (screenWidth >= 710)
            {
                screenWidth = 710;
            }
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();

            // create game objects
            int paddleX = (screenWidth - gameContent.Paddle.Width) / 2;
            // we'll center the paddle on the screen to start
            int paddleY = screenHeight - 100;
            paddle = new Paddle(paddleX, paddleY, screenWidth, spriteBatch, gameContent);
            wall = new Wall(1, 50, spriteBatch, gameContent);
            border = new GameBorder(screenWidth, screenHeight, spriteBatch, gameContent);
            ball = new Ball(screenWidth, screenHeight, spriteBatch, gameContent);
            // ball.X = 25;
            // ball.Y = 25;

            // For the icon
            //staticBall = new Ball(screenWidth, screenHeight, spriteBatch, gameContent);
            // staticBall.X = 25;
            // staticBall.Y = 25;
            //staticBall.Visible = true;
            //staticBall.UseRotation = false;
        }
        private void ServeBall()
        {
            if (ballRemaining < 1)
            {
                ballRemaining = 3;
                ball.Score = 0;
                wall = new Wall(1, 50, spriteBatch, gameContent);
            }
            ReadytoserveBall = false;
            float ballX = paddle.X + (paddle.Width) / 2;
            float ballY = paddle.Y - ball.Height;
            ball.Lauch(ballX, ballY, -3, -3);
            Console.WriteLine("BallServed");
            Console.WriteLine(ball.Visible);
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            //This If statement prevents actions from happening in game if the screen is not active
            if (IsActive == false)
            {
                return;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState newKeyBoard = Keyboard.GetState();
            MouseState newMouse = Mouse.GetState();
            //Moves mouse function here
            if (oldmouse.X != newMouse.X)
            {
                if (newMouse.X >= 0 || newMouse.X < screenWidth)
                {
                    paddle.MoveTo(newMouse.X); // Depending on which direction your mouse is going, it will move the paddle in that direction
                }
            }
            if (newMouse.LeftButton == ButtonState.Released && oldmouse.LeftButton == ButtonState.Pressed && oldmouse.X == newMouse.X && oldmouse.Y == newMouse.Y && ReadytoserveBall)
            {
                ServeBall();
            }
            if (newKeyBoard.IsKeyDown(Keys.Left))
            {
                paddle.MoveLeft();
            }
            if (newKeyBoard.IsKeyDown(Keys.Right))
            {
                paddle.MoveRight();
            }

            // Serves Ball
            if (oldkeyboard.IsKeyUp(Keys.Space) && newKeyBoard.IsKeyDown(Keys.Space) && ReadytoserveBall)
            {
                ServeBall();
            }
            oldmouse = newMouse;
            oldkeyboard = newKeyBoard;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            paddle.Draw();
            wall.Draw();

            if (ball.Visible)
            {
                bool inPlay = ball.Move(wall, paddle);
                if (inPlay)
                {
                    ball.Draw();
                }
                else
                {
                    ballRemaining--;
                    ReadytoserveBall = true;
                }
            }

            border.Draw();

            // staticBall.Draw();
            string scoreMsg = "Score: " + ball.Score.ToString("00000");
            Vector2 space = gameContent.Helvetica.MeasureString(scoreMsg);
            spriteBatch.DrawString(gameContent.Helvetica, scoreMsg, new Vector2((screenWidth - space.X) / 2, screenHeight - 40), Color.White);
            if (ball.BricksCleared >= 70)
            {
                ball.Visible = false;
                ball.BricksCleared = 0;
                wall = new Wall(1, 50, spriteBatch, gameContent);
                ReadytoserveBall = true;
            }
            if (ReadytoserveBall)
            {
                if (ballRemaining > 0)
                {
                    string startMsg = "Press <Space> or Click Mouse to Start";
                    Vector2 startSpace = gameContent.Helvetica.MeasureString(startMsg);
                    spriteBatch.DrawString(gameContent.Helvetica, startMsg, new Vector2((screenWidth - startSpace.X) / 2, screenHeight / 2), Color.White);
                }
                else
                {
                    string endMsg = "Game Over";
                    Vector2 endSpace = gameContent.Helvetica.MeasureString(endMsg);
                    spriteBatch.DrawString(gameContent.Helvetica, endMsg, new Vector2((screenWidth - endSpace.X) / 2, screenHeight / 2), Color.White);
                }
                spriteBatch.DrawString(gameContent.Helvetica, ballRemaining.ToString(), new Vector2(40, 10), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
