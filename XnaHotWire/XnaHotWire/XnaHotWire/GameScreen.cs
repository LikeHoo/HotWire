using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaHotWire
{
    public abstract class GameScreen : DrawableGameComponent
    {
        private readonly List<GameComponent> _components = new List<GameComponent>();
        protected Game _game;
        protected SpriteBatch SpriteBatch;

        public List<GameComponent> Components
        {
            get { return _components; }
        }

        // The sub-rectangle of the drawable area which should be visible on all TVs
        protected Rectangle SafeBounds;

        // Percentage of the screen on every side is the safe area
        private const float SafeAreaPortion = 0.00f;//old value 0.05f

        public GameScreen(Game game, SpriteBatch spriteBatch, HotWire parent)
            : base(game)
        {
            _game = game;
            SpriteBatch = spriteBatch;
            Parent = parent;

            // Calculate safe bounds based on current resolution
            Viewport viewport = Game.GraphicsDevice.Viewport;
            SafeBounds = new Rectangle(
                (int)(viewport.Width * SafeAreaPortion),
                (int)(viewport.Height * SafeAreaPortion),
                (int)(viewport.Width * (1 - 2 * SafeAreaPortion)),
                (int)(viewport.Height * (1 - 2 * SafeAreaPortion)));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (GameComponent component in _components)
                if (component.Enabled)
                    component.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            foreach (GameComponent component in _components)
                if (component is DrawableGameComponent && ((DrawableGameComponent) component).Visible)
                    ((DrawableGameComponent) component).Draw(gameTime);
        }

        public virtual void Show()
        {
            Visible = true;
            Enabled = true;
            foreach (GameComponent component in _components)
            {
                component.Enabled = true;
                if (component is DrawableGameComponent)
                    ((DrawableGameComponent) component).Visible = true;
            }
        }

        public virtual void Hide()
        {
            Visible = false;
            Enabled = false;
            foreach (GameComponent component in _components)
            {
                component.Enabled = false;
                if (component is DrawableGameComponent)
                    ((DrawableGameComponent) component).Visible = false;
            }
        }

        public HotWire Parent { get; set; }
    }
}
