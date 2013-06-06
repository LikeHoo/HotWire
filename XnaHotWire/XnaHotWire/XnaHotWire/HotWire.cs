using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaHotWire
{
    public class HotWire : Game
    {
        private SpriteBatch _spriteBatch;

        private GameScreen _activeScreen;
        private StartScreen _startScreen;
        private LevelSelectScreen _levelSelectScreen;
        private ActionScreen _actionScreen;
        private CalibrationScreen _calibrationScreen;
        private MessageScreen _messageScreen;

        private readonly GraphicsDeviceManager _graphics;
        private readonly SerialInput _serialInput;

        public HotWire(SerialInput serialInput)
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

        public SpriteFont HeaderFont
        {
            get { return Content.Load<SpriteFont>("HeaderFont"); }
        }

        public Texture2D DefaultBackground
        {
            get { return Content.Load<Texture2D>("BG_Cloudy"); }
        }

        public KeyboardState NewKeyboardState { get; set; }

        public KeyboardState OldKeyboardState { get; set; }

        public bool CheckKey(Keys theKey)
        {
            return NewKeyboardState.IsKeyUp(theKey) && OldKeyboardState.IsKeyDown(theKey);
        }


        protected override void LoadContent()
        {
            // Create a sprite batch to draw those textures
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _startScreen = new StartScreen(this, _spriteBatch, DefaultFont, DefaultBackground, this); 
            Components.Add(_startScreen);
            _startScreen.Hide();

            _levelSelectScreen = new LevelSelectScreen(this, _spriteBatch, DefaultFont, DefaultBackground, this);
            Components.Add(_levelSelectScreen);
            _levelSelectScreen.Hide();

            _messageScreen = new MessageScreen(this, _spriteBatch, DefaultFont, DefaultBackground, this);
            Components.Add(_messageScreen);
            _messageScreen.Hide();

            _actionScreen = new ActionScreen(this, _spriteBatch, DefaultBackground, this);
            Components.Add(_actionScreen);
            _actionScreen.Hide();

            _calibrationScreen = new CalibrationScreen(this, _spriteBatch, DefaultFont, DefaultBackground, this);
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
            NewKeyboardState = Keyboard.GetState();
            _activeScreen.Update(gameTime);
            OldKeyboardState = NewKeyboardState;
        }

        public void GotoScreen(ScreenType screenType)
        {
            _activeScreen.Hide();
            // GraphicsDevice.Clear(Color.Beige);

            switch (screenType)
            {
                case ScreenType.Action:
                    _actionScreen.ResetPosition();
                    _actionScreen.GameLost = false;
                    _actionScreen.LoopHit = true;
                    _activeScreen = _actionScreen;
                    break;
                case ScreenType.Calibration:
                    _actionScreen.ResetPosition();
                    _actionScreen.GameLost = false;
                    _actionScreen.LoopHit = true;
                    _calibrationScreen.CalibratedSince = DateTime.MinValue;
                    _activeScreen = _calibrationScreen;
                    break;
                case ScreenType.Lost:
                    _messageScreen.Message = "Verloren! Neues Spiel?";
                    _messageScreen.ScreenOnYes = ScreenType.LevelSelect;
                    _messageScreen.ScreenOnNo = ScreenType.Start;
                    _activeScreen = _messageScreen;
                    break;
                case ScreenType.Won:
                    _messageScreen.Message = "Gewonnen! Neues Spiel?";
                    _messageScreen.ScreenOnYes = ScreenType.LevelSelect;
                    _messageScreen.ScreenOnNo = ScreenType.Start;
                    _activeScreen = _messageScreen;
                    break;
                case ScreenType.Start:
                    _activeScreen = _startScreen;
                    break;
                case ScreenType.LevelSelect:
                    _activeScreen = _levelSelectScreen;
                    break;

            }
            _activeScreen.Show();
        }

        public void SetLevel(string level)
        {
            _actionScreen.WireTexture = Content.Load<Texture2D>(level);
        }
    }
}
