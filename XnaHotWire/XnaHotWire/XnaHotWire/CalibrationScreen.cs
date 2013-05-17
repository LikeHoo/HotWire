using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaHotWire
{
    class CalibrationScreen : GameScreen
    {
      
        readonly Texture2D _image;
        readonly Rectangle _imageRectangle;
        
        public CalibrationScreen(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont, Texture2D image, HotWire parent)
            : base(game, spriteBatch, parent)
        {
          
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Parent.CheckKey(Keys.Escape))
            {
                Parent.GotoScreen(ScreenType.Start);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.DrawString(Parent.DefaultFont, "Calibrate Controller to zero", new Vector2(100, 50), Color.White);

            float x = Parent.SerialController.GetPositionX();
            float y = Parent.SerialController.GetPositionY();

            SpriteBatch.DrawString(Parent.DefaultFont, "X:" + x, new Vector2(100, 125), Color.White);
            SpriteBatch.DrawString(Parent.DefaultFont, "Y:" + y, new Vector2(100, 150), Color.White);

            if (Parent.SerialController.IsCalibrated())
            {
                SpriteBatch.DrawString(Parent.DefaultFont, "Calibrated!" + y, new Vector2(100, 175), Color.White);

                Parent.GotoScreen(ScreenType.Action);
            }
          
            base.Draw(gameTime);
        }
    }
}

