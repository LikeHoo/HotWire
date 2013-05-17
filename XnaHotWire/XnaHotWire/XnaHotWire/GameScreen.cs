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

        public GameScreen(Game game, SpriteBatch spriteBatch, HotWire2 parent)
            : base(game)
        {
            _game = game;
            SpriteBatch = spriteBatch;
            Parent = parent;
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
                if (component is DrawableGameComponent &&
                    ((DrawableGameComponent) component).Visible)
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

        public KeyboardState NewKeyboardState { get; set; }

        public KeyboardState OldKeyboardState { get; set; }

        protected bool CheckKey(Keys theKey)
        {
            return NewKeyboardState.IsKeyUp(theKey) &&
                OldKeyboardState.IsKeyDown(theKey);
        }

        public HotWire2 Parent { get; set; }
    }
}
