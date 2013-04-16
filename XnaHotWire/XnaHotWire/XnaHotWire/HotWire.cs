using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaHotWire
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class HotWire : Game
    {
        readonly GraphicsDeviceManager _graphics;

        // The images we will draw
        private Texture2D _loopTextureLeft;
        private Texture2D _loopTextureRight;
        private Texture2D _wireTexture;
        private Texture2D _collisionTexture;
        private Texture2D _backGroundTexture;
        private Texture2D _backGroundTextureWarning;

        // The color data for the images; used for per pixel collision
        private Color[] _loopTextureData;
        private Color[] _wireTextureData;
        private Color[] _collisionTextureData;

        // The images will be drawn with this SpriteBatch
        private SpriteBatch _spriteBatch;

        // positions 
        private Vector2 _loopPosition;
        private const int LoopMoveSpeed = 2;

        // direction
        private Vector2 _loopDirection;
        private Vector2 _loopOrigin;
        private float _loopAngle;
        private Vector2 _previousPosition;
        private Vector2 _currentPosition;

        // Blocks
        private readonly Vector2 _blockPosition = new Vector2();

        // For when a collision is detected
        private bool _loopHit;
        private bool _gameLost;

        // The sub-rectangle of the drawable area which should be visible on all TVs
        private Rectangle _safeBounds;

        // Percentage of the screen on every side is the safe area
        private const float SafeAreaPortion = 0.00f;//old value 0.05f

        public HotWire()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 600;

            // wird für die MessageBox benötigt!
            Components.Add(new GamerServicesComponent(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to
        /// run. This is where it can query for any required services and load any
        /// non-graphic related content.  Calling base.Initialize will enumerate through
        /// any components and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Calculate safe bounds based on current resolution
            Viewport viewport = _graphics.GraphicsDevice.Viewport;
            _safeBounds = new Rectangle(
                (int)(viewport.Width * SafeAreaPortion),
                (int)(viewport.Height * SafeAreaPortion),
                (int)(viewport.Width * (1 - 2 * SafeAreaPortion)),
                (int)(viewport.Height * (1 - 2 * SafeAreaPortion)));

            // Start loop at outer left and middle height.
            _loopPosition.X = 0;
            _loopPosition.Y = _safeBounds.Height / 2 - _loopTextureLeft.Height;

            // Set initial direction
            _previousPosition = _loopPosition;
            _currentPosition = _loopPosition;

        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load textures
            _wireTexture = Content.Load<Texture2D>("Wire002");
            _loopTextureLeft = Content.Load<Texture2D>("Loop003_links");
            _loopTextureRight = Content.Load<Texture2D>("Loop003_rechts");
            _collisionTexture = Content.Load<Texture2D>("Collision001");

            _backGroundTexture = Content.Load<Texture2D>("BG_Cloudy");
            _backGroundTextureWarning = Content.Load<Texture2D>("BG_Cloudy_Red");

            // Extract collision data
            _wireTextureData = new Color[_wireTexture.Width * _wireTexture.Height];
            _wireTexture.GetData(_wireTextureData);

            _loopTextureData = new Color[_loopTextureLeft.Width * _loopTextureLeft.Height];
            _loopTextureLeft.GetData(_loopTextureData);

            _collisionTextureData = new Color[_collisionTexture.Width*_collisionTexture.Height];
            _collisionTexture.GetData(_collisionTextureData);

            // Create a sprite batch to draw those textures
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);

            // Set rotation center
            _loopOrigin = new Vector2(_loopTextureLeft.Width / 2.0f, _loopTextureLeft.Height / 2.0f);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Get input
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            // Allows the game to exit
            if (gamePad.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Move the loop left and right with arrow keys or d-pad
            if (keyboard.IsKeyDown(Keys.Left) || gamePad.DPad.Left == ButtonState.Pressed)
            {
                _loopPosition.X -= LoopMoveSpeed;
            }


            //Constant movement by always increasing position?
            if (keyboard.IsKeyDown(Keys.Right) || gamePad.DPad.Right == ButtonState.Pressed)
            {
                _loopPosition.X += LoopMoveSpeed;
            }

            // Move the loop up and down with arrow keys or d-pad
            if (keyboard.IsKeyDown(Keys.Up) || gamePad.DPad.Up == ButtonState.Pressed)
            {
                _loopPosition.Y -= LoopMoveSpeed;
            }

            if (keyboard.IsKeyDown(Keys.Down) || gamePad.DPad.Down == ButtonState.Pressed)
            {
                _loopPosition.Y += LoopMoveSpeed;
            }

            // Analoge input for xbox360 controller
            _loopPosition.X += gamePad.ThumbSticks.Left.X;
            _loopPosition.Y -= gamePad.ThumbSticks.Left.Y;

            // Prevent the loop from moving off of the screen
            _loopPosition.X = MathHelper.Clamp(_loopPosition.X, _safeBounds.Left, _safeBounds.Right - _loopTextureLeft.Width);
            _loopPosition.Y = MathHelper.Clamp(_loopPosition.Y, _safeBounds.Top, _safeBounds.Bottom - _loopTextureLeft.Height);

            // Goal reached?
            if (_loopPosition.X > (_wireTexture.Width - 50))
            {
                TargetReached();
            }

            //Get the bounding rectangle of the collison
            Rectangle collisionRectangle = new Rectangle((int)_loopPosition.X +(_loopTextureLeft.Width/2)-(_collisionTexture.Width/2), (int)_loopPosition.Y +(_loopTextureLeft.Height/2) -(_collisionTexture.Height/2),
                _collisionTexture.Width, _collisionTexture.Height);

            // Get the bounding rectangle of this block
            Rectangle wireRectangle = new Rectangle(0, 0, _wireTexture.Width, _wireTexture.Height);

            // Update each block
            _loopHit = false;

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
                _loopHit = true;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            // Change the background to red when the wire was hit by a block
            if (_loopHit && !_gameLost)
            {
                _spriteBatch.Draw(_backGroundTexture, _blockPosition, Color.White);
            }
            else
            {
                _spriteBatch.Draw(_backGroundTextureWarning, _blockPosition, Color.White);

                if (!_gameLost)
                {
                    SimpleMessageBox.ShowMessageBox("LOOOOOSER", "Drehe die Potentiomenter zurück um neu zu starten!",
                                                    new[] {"OK"}, 0, MessageBoxIcon.None);
                    _gameLost = true;
                }
            }
           
            // Draw loop
            //_spriteBatch.Draw(_loopTexture, _loopPosition, Color.White);
            //TODO: rotate   
 
            //temp
            Vector2 rotateLoopPosition = new Vector2(0, 0);
            rotateLoopPosition.X = _loopPosition.X + _loopTextureLeft.Width/2.0f;
            rotateLoopPosition.Y = _loopPosition.Y + _loopTextureLeft.Height/2.0f;

            _spriteBatch.Draw(_loopTextureLeft, rotateLoopPosition, null, Color.White, _loopAngle, _loopOrigin, 1.0f, SpriteEffects.None, 0);
            _spriteBatch.Draw(_wireTexture, _blockPosition, Color.White);
            _spriteBatch.Draw(_loopTextureRight, rotateLoopPosition, null, Color.White, _loopAngle, _loopOrigin, 1.0f, SpriteEffects.None, 0);
           
            _spriteBatch.End();

            base.Draw(gameTime);
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

        private void TargetReached()
        {
            SimpleMessageBox.ShowMessageBox("Winner!", "Drehe die Potentiomenter zurück um neu zu starten!",
                                                    new[] { "OK" }, 0, MessageBoxIcon.None);
        }
    }
}
