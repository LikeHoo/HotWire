using System;
using Microsoft.Xna.Framework;
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
        Texture2D _loopTexture;
        Texture2D _wireTexture;

        // The color data for the images; used for per pixel collision
        Color[] _loopTextureData;
        Color[] _wireTextureData;

        // The images will be drawn with this SpriteBatch
        SpriteBatch _spriteBatch;

        // positions 
        Vector2 _loopPosition;
        const int LoopMoveSpeed = 2;

        // direction
        Vector2 _loopDirection;
        Vector2 _loopOrigin;
        float _loopAngle;

        // Blocks
        readonly Vector2 _blockPosition = new Vector2();

        // For when a collision is detected
        bool _loopHit;

        // The sub-rectangle of the drawable area which should be visible on all TVs
        Rectangle _safeBounds;

        // Percentage of the screen on every side is the safe area
        private const float SafeAreaPortion = 0.00f;//old value 0.05f

        public HotWire()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 600;
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

            // Start the loop in the center along the bottom of the screen
            // ReSharper disable PossibleLossOfFraction
            //_loopPosition.X = (_safeBounds.Width - _loopTexture.Width) / 2;            
            // ReSharper restore PossibleLossOfFraction
            //_loopPosition.Y = _safeBounds.Height - _loopTexture.Height;

            // Start loop at outer left and middle height, add +50 to match wire
            _loopPosition.X = 0;
            _loopPosition.Y = _safeBounds.Height / 2 - _loopTexture.Height + 50;
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load textures
            _wireTexture = Content.Load<Texture2D>("Wire001");
            _loopTexture = Content.Load<Texture2D>("Loop001");

            // Extract collision data
            _wireTextureData = new Color[_wireTexture.Width * _wireTexture.Height];
            _wireTexture.GetData(_wireTextureData);

            _loopTextureData = new Color[_loopTexture.Width * _loopTexture.Height];
            _loopTexture.GetData(_loopTextureData);

            // Create a sprite batch to draw those textures
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
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

            // Prevent the loop from moving off of the screen
            _loopPosition.X = MathHelper.Clamp(_loopPosition.X, _safeBounds.Left, _safeBounds.Right - _loopTexture.Width);

            // Get the bounding rectangle of the loop
            Rectangle loopRectangle = new Rectangle((int)_loopPosition.X, (int)_loopPosition.Y,
                _loopTexture.Width, _loopTexture.Height);

            // Get the bounding rectangle of this block
            Rectangle wireRectangle = new Rectangle(0, 0, _wireTexture.Width, _wireTexture.Height);

            // Update each block
            _loopHit = false;

            // Update angle
            //TODO: calculate angle
            //_loopAngle;
            //_loopDirection;

            //  Check collision with the wire
            if (IntersectPixels(wireRectangle, _wireTextureData,
                                    loopRectangle, _loopTextureData))
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
            GraphicsDevice device = _graphics.GraphicsDevice;

            // Change the background to red when the wire was hit by a block
            if (_loopHit)
            {
                device.Clear(Color.CornflowerBlue);
            }
            else
            {
                device.Clear(Color.Red);
            }

            _spriteBatch.Begin();

            // Draw loop
            _spriteBatch.Draw(_loopTexture, _loopPosition, Color.White);
            //TODO: rotate
            //_spriteBatch.Draw(_loopTexture, _loopPosition, null, Color.White, _loopAngle, _loopOrigin, 1.0f, SpriteEffects.None, 0);

            // Draw wire 
            _spriteBatch.Draw(_wireTexture, _blockPosition, Color.White);

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
    }
}
