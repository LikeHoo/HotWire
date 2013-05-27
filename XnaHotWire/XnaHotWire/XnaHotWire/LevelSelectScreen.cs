using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaHotWire
{
    class LevelSelectScreen : GameScreen
    {
        readonly MenuComponent _menuComponent;
        readonly Texture2D _image;
        readonly Rectangle _imageRectangle;

        public int SelectedIndex
        {
            get { return _menuComponent.SelectedIndex; }
            set { _menuComponent.SelectedIndex = value; }
        }

        public LevelSelectScreen(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont, Texture2D image, HotWire parent)
            : base(game, spriteBatch, parent)
        {
             string[] menuItems = { "Level 1", "Level 2", "Level 3", "Level 4" };
            _menuComponent = new MenuComponent(game, spriteBatch, spriteFont, menuItems);
             Components.Add(_menuComponent);            
        }

        protected override void LoadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            _menuComponent.Update(gameTime);

            if (Parent.CheckKey(Keys.Escape))
            {
                Parent.GotoScreen(ScreenType.Start);
            }

            if (Parent.CheckKey(Keys.Enter))
            {
                string level = "Wire004";

                switch (SelectedIndex)
                {
                    case 0:
                        level = "Wire004";
                        break;
                    case 1:
                        level = "Wire005";
                        break;
                    case 2:
                        level = "Wire003";
                        break;
                    case 3:
                        level = "Wire004";
                        break;
                }
                Parent.SetLevel(level);                
                Parent.GotoScreen(ScreenType.Calibration);                
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}

