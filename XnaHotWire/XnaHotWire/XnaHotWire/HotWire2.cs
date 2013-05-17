using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaHotWire
{
    public class HotWire2 : Game
    {
        private SpriteBatch _spriteBatch;

        GameScreen _activeScreen;
        StartScreen _startScreen;

        private ActionScreen _actionScreen;
        private CalibrationScreen _calibrationScreen;

        private readonly GraphicsDeviceManager _graphics;
        private readonly SerialInput _serialInput;

        public HotWire2(SerialInput serialInput)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 600;
            _serialInput = serialInput;
        }

        public SerialInput SerialController
        {
            get { return _serialInput;  }
        }

        public SpriteFont DefaultFont
        {
            get { return Content.Load<SpriteFont>("SpriteFont"); }
        }

        protected override void LoadContent()
        {
            // Create a sprite batch to draw those textures
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _startScreen = new StartScreen(this, _spriteBatch, DefaultFont, Content.Load<Texture2D>("BG_Cloudy"), this); 
            
            Components.Add(_startScreen);
            _startScreen.Hide();

            _actionScreen = new ActionScreen(this, _spriteBatch, Content.Load<Texture2D>("BG_Cloudy"), this);
            Components.Add(_actionScreen);
            _actionScreen.Hide();

            _calibrationScreen = new CalibrationScreen(this, _spriteBatch, DefaultFont, Content.Load<Texture2D>("BG_Cloudy"), this);
            Components.Add(_calibrationScreen);
            _calibrationScreen.Hide();

            _activeScreen = _startScreen;
           
            _activeScreen.Show();
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            base.Draw(gameTime);
            _activeScreen.Draw(gameTime);

            _spriteBatch.End();
        }

        protected override void Update(GameTime gameTime)
        {
            _actionScreen.NewKeyboardState = Keyboard.GetState();
            _activeScreen.Update(gameTime);
            _activeScreen.OldKeyboardState = _actionScreen.NewKeyboardState;
        }

        public void GotoScreen(ScreenType screenType)
        {
            _activeScreen.Hide();
            GraphicsDevice.Clear(Color.Beige);

            switch (screenType)
            {
                case ScreenType.Action:
                    _actionScreen.GameLost = false;
                    _actionScreen.LoopHit = true;
                    _activeScreen = _actionScreen;
                    break;
                case ScreenType.Calibration:
                    _activeScreen = _calibrationScreen;
                    break;

            }
            _activeScreen.Show();

        }
    }
}
