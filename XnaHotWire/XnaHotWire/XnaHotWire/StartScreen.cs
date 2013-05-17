using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaHotWire
{
    class StartScreen : GameScreen
    {
        readonly MenuComponent _menuComponent;
        readonly Texture2D _image;
        readonly Rectangle _imageRectangle;

        public int SelectedIndex
        {
            get { return _menuComponent.SelectedIndex; }
            set { _menuComponent.SelectedIndex = value; }
        }

        public StartScreen(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont, Texture2D image, HotWire parent)
            : base(game, spriteBatch, parent)
        {
             string[] menuItems = { "Start Game", "Calibration Dialog", "Lost Game Dialog", "End Game" };
            _menuComponent = new MenuComponent(game, spriteBatch, spriteFont, menuItems);
             Components.Add(_menuComponent);
            // _image = image;
            // _imageRectangle = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
        }

        protected override void LoadContent()
        {
            //string[] menuItems = { "Start Game", "High Scores", "End Game" };

            //_spriteBatch = new SpriteBatch(GraphicsDevice);

            //_menuComponent = new MenuComponent(Game, _spriteBatch, Game.Content.Load<SpriteFont>("SpriteFont"), menuItems);
            //Components.Add(_menuComponent);
        }

        public override void Update(GameTime gameTime)
        {
            _menuComponent.Update(gameTime);

            if (Parent.CheckKey(Keys.Enter))
            {
                if (SelectedIndex == 0)
                {
                    if (Parent.SerialController.IsCalibrated())
                    {
                        Parent.GotoScreen(ScreenType.Action);
                    }
                    else
                    {
                        Parent.GotoScreen(ScreenType.Calibration);
                    }
                }
                if (SelectedIndex == 1)
                {
                    Parent.GotoScreen(ScreenType.Calibration);
                }
                if (SelectedIndex == 2)
                {
                    
                  Parent.GotoScreen(ScreenType.Lost);
                }
                if (SelectedIndex == 3)
                {
                    Parent.Exit();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null);
             // _spriteBatch.Draw(_image, _imageRectangle, Color.White);
            // _menuComponent.Draw(gameTime);
            //_spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

