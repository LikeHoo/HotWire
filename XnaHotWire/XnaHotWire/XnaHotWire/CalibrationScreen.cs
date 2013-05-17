using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaHotWire
{
    class CalibrationScreen : GameScreen
    {
      
        readonly Texture2D _image;
        readonly Rectangle _imageRectangle;
        
        public CalibrationScreen(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont, Texture2D image, HotWire2 parent)
            : base(game, spriteBatch, parent)
        {
          
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            SpriteBatch.DrawString(Parent.DefaultFont, "Calibrate Controller to zero", new Vector2(100, 50), Color.Black);

            float x = Parent.SerialController.GetPositionX();
            float y = Parent.SerialController.GetPositionY();



            SpriteBatch.DrawString(Parent.DefaultFont, "X:" + x, new Vector2(100, 125), Color.Black);
            SpriteBatch.DrawString(Parent.DefaultFont, "Y:" + y, new Vector2(100, 150), Color.Black);

            if (Parent.SerialController.IsCalibrated())
            {
                SpriteBatch.DrawString(Parent.DefaultFont, "Calibrated!" + y, new Vector2(100, 175), Color.Black);
                
                Parent.GotoScreen(ScreenType.Action);
            }
          
        
            base.Draw(gameTime);
        }
    }
}

