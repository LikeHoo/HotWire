using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaHotWire
{
    public class MenuComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        readonly string[] _menuItems;
        int _selectedIndex;

        readonly Color _normal = Color.White;
        readonly Color _hilite = Color.Yellow;

        KeyboardState _keyboardState;
        KeyboardState _oldKeyboardState;

        readonly SpriteBatch _spriteBatch;
        readonly SpriteFont _spriteFont;

        Vector2 _position;
        float _width = 0f;
        float _height = 0f;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float Width
        {
            get { return _width; }
        }

        public float Height
        {
            get { return _height; }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                if (_selectedIndex < 0)
                    _selectedIndex = 0;
                if (_selectedIndex >= _menuItems.Length)
                    _selectedIndex = _menuItems.Length - 1;
            }
        }

        public MenuComponent(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont, string[] menuItems)
            : base(game)
        {
            _spriteBatch = spriteBatch;
            _spriteFont = spriteFont;
            _menuItems = menuItems;
            MeasureMenu();
        }

        private void MeasureMenu()
        {
            _height = 0;
            _width = 0;

            foreach (string item in _menuItems)
            {
                Vector2 size = _spriteFont.MeasureString(item);
                if (size.X > _width)
                    _width = size.X;
                _height += _spriteFont.LineSpacing + 5;
            }

            _position = new Vector2((Game.Window.ClientBounds.Width - _width) / 2, (Game.Window.ClientBounds.Height - _height) / 2);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        private bool CheckKey(Keys theKey)
        {
            return _keyboardState.IsKeyUp(theKey) &&
                _oldKeyboardState.IsKeyDown(theKey);
        }

        public override void Update(GameTime gameTime)
        {
            _keyboardState = Keyboard.GetState();

            if (CheckKey(Keys.Down))
            {
                _selectedIndex++;
                if (_selectedIndex == _menuItems.Length)
                    _selectedIndex = 0;
            }

            if (CheckKey(Keys.Up))
            {
                _selectedIndex--;
                if (_selectedIndex < 0)
                    _selectedIndex = _menuItems.Length - 1;
            }

            base.Update(gameTime);

            _oldKeyboardState = _keyboardState;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Vector2 location = _position;

            for (int i = 0; i < _menuItems.Length; i++)
            {
                Color tint;
                if (i == _selectedIndex)
                    tint = _hilite;
                else
                    tint = _normal;

                _spriteBatch.DrawString(_spriteFont, _menuItems[i], location, tint);
                location.Y += _spriteFont.LineSpacing + 5;
            }
        }
    }
}

