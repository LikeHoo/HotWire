using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaHotWire
{
    class PopupnScreen : GameScreen
    {
        readonly MenuComponent _menuComponent;
        readonly Texture2D _image;
        readonly Rectangle _imageRectangle;

        public int SelectedIndex
        {
            get { return _menuComponent.SelectedIndex; }
            set { _menuComponent.SelectedIndex = value; }
        }

        public PopupnScreen(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont, Texture2D image, HotWire2 parent)
            : base(game, spriteBatch, parent)
        {
            string[] menuItems = { "Yes", "No" };
            _menuComponent = new MenuComponent(game, spriteBatch, spriteFont, menuItems);
            Components.Add(_menuComponent);
            this._image = image;

            _imageRectangle = new Rectangle((Game.Window.ClientBounds.Width - this._image.Width) / 2, (Game.Window.ClientBounds.Height - this._image.Height) / 2, this._image.Width, this._image.Height);

            _menuComponent.Position = new Vector2((_imageRectangle.Width - _menuComponent.Width) / 2, _imageRectangle.Bottom - _menuComponent.Height - 10);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Draw(_image, _imageRectangle, Color.White);
            base.Draw(gameTime);
        }
    }
}

