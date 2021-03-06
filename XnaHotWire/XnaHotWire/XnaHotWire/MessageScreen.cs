﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaHotWire
{
    class MessageScreen : GameScreen
    {
        readonly MenuComponent _menuComponent;
        readonly Texture2D _image;
        readonly Rectangle _imageRectangle;

        public int SelectedIndex
        {
            get { return _menuComponent.SelectedIndex; }
            set { _menuComponent.SelectedIndex = value; }
        }

        public MessageScreen(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont, Texture2D image, HotWire parent)
            : base(game, spriteBatch, parent)
        {
            //string[] menuItems = { "Start Game" };
            //_menuComponent = new MenuComponent(game, spriteBatch, spriteFont, menuItems);
            //Components.Add(_menuComponent);

            string[] menuItems = { "Yes", "No" };
            _menuComponent = new MenuComponent(game, spriteBatch, spriteFont, menuItems);
            Components.Add(_menuComponent);
            //_image = image;

            //_imageRectangle = new Rectangle((Game.Window.ClientBounds.Width - _image.Width) / 2, (Game.Window.ClientBounds.Height - _image.Height) / 2, _image.Width, _image.Height);

            //_menuComponent.Position = new Vector2((_imageRectangle.Width - _menuComponent.Width) / 2,_imageRectangle.Bottom - _menuComponent.Height - 10);

        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // _menuComponent.Update(gameTime);
            base.Update(gameTime);

            // _actionScreen.NewKeyboardState = Keyboard.GetState();
            // _activeScreen.Update(gameTime);

            if (Parent.CheckKey(Keys.Enter))
            {
                if (SelectedIndex == 0)
                {
                    Parent.GotoScreen(ScreenOnYes);
                }
                else
                {
                    Parent.GotoScreen(ScreenOnNo);
                }
            }

            if (Parent.CheckKey(Keys.Escape))
            {
                Parent.GotoScreen(ScreenOnNo);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.DrawString(Parent.DefaultFont, Message, new Vector2(100, 100), Color.White);
            // SpriteBatch.Draw(_image, _imageRectangle, Color.White);
            // _menuComponent.Draw(gameTime);
            base.Draw(gameTime);
        }

        public string Message { get; set; }

        public ScreenType ScreenOnYes { get; set; }

        public ScreenType ScreenOnNo { get; set; }
    }
}

