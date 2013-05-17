﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaHotWire
{
    class ActionScreen : GameScreen
    {
        KeyboardState _keyboardState;
        readonly Texture2D _image;
        readonly Rectangle _imageRectangle;

        // The images we will draw
        private readonly Texture2D _loopTextureLeft;
        private readonly Texture2D _loopTextureRight;
        private readonly Texture2D _wireTexture;
        private readonly Texture2D _collisionTexture;
        private readonly Texture2D _backGroundTexture;
        private readonly Texture2D _backGroundTextureWarning;

        // The color data for the images; used for per pixel collision
        private readonly Color[] _loopTextureData;
        private readonly Color[] _wireTextureData;
        private readonly Color[] _collisionTextureData;

        // positions 
        private Vector2 _loopPosition;
        private const int LoopMoveSpeed = 2;

        // direction
        private Vector2 _loopDirection;
        private readonly Vector2 _loopOrigin;
        private float _loopAngle;
        private Vector2 _previousPosition;
        private Vector2 _currentPosition;

        // Blocks
        private readonly Vector2 _blockPosition = new Vector2();

        // The sub-rectangle of the drawable area which should be visible on all TVs
        private Rectangle _safeBounds;

        readonly SpriteFont _font;

        // Percentage of the screen on every side is the safe area
        private const float SafeAreaPortion = 0.00f;//old value 0.05f

        public bool GameLost { get; set; }

        public bool LoopHit { get; set; }

        public ActionScreen(Game game, SpriteBatch spriteBatch, Texture2D image, HotWire parent)
            : base(game, spriteBatch, parent)
        {

            // Calculate safe bounds based on current resolution
            Viewport viewport = Game.GraphicsDevice.Viewport;
            _safeBounds = new Rectangle(
                (int)(viewport.Width * SafeAreaPortion),
                (int)(viewport.Height * SafeAreaPortion),
                (int)(viewport.Width * (1 - 2 * SafeAreaPortion)),
                (int)(viewport.Height * (1 - 2 * SafeAreaPortion)));


            _image = image;
           
            _imageRectangle = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);

            // Load textures
            _wireTexture = game.Content.Load<Texture2D>("Wire003");
            _loopTextureLeft = game.Content.Load<Texture2D>("Loop003_links");
            _loopTextureRight = game.Content.Load<Texture2D>("Loop003_rechts");
            _collisionTexture = game.Content.Load<Texture2D>("Collision001");

            _backGroundTexture = game.Content.Load<Texture2D>("BG_Cloudy");
            _backGroundTextureWarning = game.Content.Load<Texture2D>("BG_Cloudy_Red");

            // Extract collision data
            _wireTextureData = new Color[_wireTexture.Width * _wireTexture.Height];
            _wireTexture.GetData(_wireTextureData);

            _loopTextureData = new Color[_loopTextureLeft.Width * _loopTextureLeft.Height];
            _loopTextureLeft.GetData(_loopTextureData);

            _collisionTextureData = new Color[_collisionTexture.Width * _collisionTexture.Height];
            _collisionTexture.GetData(_collisionTextureData);

            _font = game.Content.Load<SpriteFont>("SpriteFont");


            ResetPosition();

            // Set rotation center
            _loopOrigin = new Vector2(_loopTextureLeft.Width / 2.0f, _loopTextureLeft.Height / 2.0f);
        }

        public void ResetPosition()
        {
            // Start loop at outer left and middle height.
            _loopPosition.X = 0;
            _loopPosition.Y = _safeBounds.Height / 2 - _loopTextureLeft.Height;

            // Set initial direction
            _previousPosition = _loopPosition;
            _currentPosition = _loopPosition;
        }

        public override void Update(GameTime gameTime)
        {
            // Get input
            // KeyboardState keyboard = Keyboard.GetState();
            // GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            //// Allows the Game to exit
            if (Parent.CheckKey(Keys.Escape))
            {
                Parent.GotoScreen(ScreenType.Start);
            }

            //// Move the loop left and right with arrow keys or d-pad
            //if (CheckKey(Keys.Left))
            //{
            //    _loopPosition.X -= LoopMoveSpeed;
            //}


            ////Constant movement by always increasing position?
            //if (CheckKey(Keys.Right))
            //{
            //    _loopPosition.X += LoopMoveSpeed;
            //}

            //// Move the loop up and down with arrow keys or d-pad
            //if (CheckKey(Keys.Up))
            //{
            //    _loopPosition.Y -= LoopMoveSpeed;
            //}

            //if (CheckKey(Keys.Down))
            //{
            //    _loopPosition.Y += LoopMoveSpeed;
            //}

            // Analoge input for xbox360 controller
            //_loopPosition.X += gamePad.ThumbSticks.Left.X;
            //_loopPosition.Y -= gamePad.ThumbSticks.Left.Y;

            // Get input from serial controller
            //debug
            //float xx = _serialInput.GetPositionX();
            //float yy = _serialInput.GetPositionY();
            //Console.Write("X:{0}  ", xx);
            //Console.WriteLine("Y:{0}", yy);

            _loopPosition.X += Parent.SerialController.GetPositionX();
            _loopPosition.Y -= Parent.SerialController.GetPositionY();

            // Prevent the loop from moving off of the screen
            _loopPosition.X = MathHelper.Clamp(_loopPosition.X, _safeBounds.Left, _safeBounds.Right - _loopTextureLeft.Width);
            _loopPosition.Y = MathHelper.Clamp(_loopPosition.Y, _safeBounds.Top, _safeBounds.Bottom - _loopTextureLeft.Height);

            // Goal reached?
            if (_loopPosition.X > (_wireTexture.Width - 50))
            {
                //TargetReached();
            }

            //Get the bounding rectangle of the collison
            Rectangle collisionRectangle = new Rectangle((int)_loopPosition.X + (_loopTextureLeft.Width / 2) - (_collisionTexture.Width / 2), (int)_loopPosition.Y + (_loopTextureLeft.Height / 2) - (_collisionTexture.Height / 2),
                _collisionTexture.Width, _collisionTexture.Height);

            // Get the bounding rectangle of this block
            Rectangle wireRectangle = new Rectangle(0, 0, _wireTexture.Width, _wireTexture.Height);

            // Update each block
            LoopHit = false;

            // Update angle
            //TODO: calculate angle
            _currentPosition = _loopPosition;
            _loopDirection = Vector2.Subtract(_currentPosition, _previousPosition);
            _loopAngle = (float)(0.5 * Math.PI) - (float)Math.Atan2(_loopDirection.X, _loopDirection.Y);

            // Set new previous position, subtracting 0.001 to avoid zero-vector
            _previousPosition.X = _currentPosition.X - 0.001f;
            _previousPosition.Y = _currentPosition.Y;

            //  Check collision with the wire
            //if (IntersectPixels(wireRectangle, _wireTextureData, loopRectangle, _loopTextureData))
            if (IntersectPixels(wireRectangle, _wireTextureData, collisionRectangle, _collisionTextureData))
            {
                LoopHit = true;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Change the background to red when the wire was hit by a block
            if (LoopHit && !GameLost)
            {
                SpriteBatch.Draw(_backGroundTexture, _blockPosition, Color.White);
            }
            else
            {
                SpriteBatch.Draw(_backGroundTextureWarning, _blockPosition, Color.White);

                if (!GameLost)
                {
                    Parent.GotoScreen(ScreenType.Lost);

                    GameLost = true;
                }
            }

            // Draw loop
            //_spriteBatch.Draw(_loopTexture, _loopPosition, Color.White);
            //TODO: rotate   

            //temp
            Vector2 rotateLoopPosition = new Vector2(0, 0);
            rotateLoopPosition.X = _loopPosition.X + _loopTextureLeft.Width / 2.0f;
            rotateLoopPosition.Y = _loopPosition.Y + _loopTextureLeft.Height / 2.0f;

            SpriteBatch.Draw(_loopTextureLeft, rotateLoopPosition, null, Color.White, _loopAngle, _loopOrigin, 1.0f, SpriteEffects.None, 0);
            SpriteBatch.Draw(_wireTexture, _blockPosition, Color.White);
            SpriteBatch.Draw(_loopTextureRight, rotateLoopPosition, null, Color.White, _loopAngle, _loopOrigin, 1.0f, SpriteEffects.None, 0);

            SpriteBatch.DrawString(_font, "X: " + Parent.SerialController.GetPositionX(), new Vector2(10, 10), Color.Black);
            SpriteBatch.DrawString(_font, "Y: " + Parent.SerialController.GetPositionY(), new Vector2(10, 35), Color.Black);
            SpriteBatch.DrawString(_font, "Angle: " + _loopAngle, new Vector2(10, 60), Color.Black);
            SpriteBatch.DrawString(_font, "Direction: " + _loopDirection, new Vector2(10, 85), Color.Black);

            // GraphicsDevice.Clear(Color.CornflowerBlue);
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the first sprite</param>
        /// <param name="dataA">Pixel data of the first sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the second sprite</param>
        /// <param name="dataB">Pixel data of the second sprite</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                    Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }
    }
}

