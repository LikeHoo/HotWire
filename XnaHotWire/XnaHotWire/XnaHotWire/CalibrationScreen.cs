using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaHotWire
{
    class CalibrationScreen : GameScreen
    {
        private DateTime _calibratedSince = DateTime.MinValue;

        public CalibrationScreen(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont, Texture2D image, HotWire parent)
            : base(game, spriteBatch, parent)
        { }

        public DateTime CalibratedSince
        {
            get { return _calibratedSince; }
            set { _calibratedSince = value; }
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
                if (CalibratedSince == DateTime.MinValue)
                    CalibratedSince = DateTime.Now;

                TimeSpan span = DateTime.Now.Subtract(CalibratedSince);

                int secondsToWait = 3 - span.Seconds;

                SpriteBatch.DrawString(Parent.DefaultFont, "Calibrated! Game starts in " + secondsToWait + " Seconds", new Vector2(100, 250), Color.White);

                if (secondsToWait == 0)
                {
                    Parent.GotoScreen(ScreenType.Action);
                }
            }
            else
            {
                CalibratedSince = DateTime.MinValue;
            }
          
            base.Draw(gameTime);
        }
    }
}

