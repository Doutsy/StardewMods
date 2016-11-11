﻿using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Menus
{
    public class PlusMinusFormComponent : BaseFormComponent
    {
        protected static Rectangle PlusButton = new Rectangle(185, 345, 6, 8);
        protected static Rectangle MinusButton = new Rectangle(177, 345, 6, 8);
        protected static Rectangle Background = new Rectangle(227, 425, 9, 9);
        public int Value {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }
        protected int _Value;
        protected int MinValue;
        protected int MaxValue;
        protected ValueChanged<int> Handler;
        protected Rectangle PlusArea;
        protected Rectangle MinusArea;
        protected int Counter = 0;
        protected int Limiter = 10;
        protected int OptionKey;
        protected int OldValue;
        public PlusMinusFormComponent(Point position, int minValue, int maxValue, int optionKey, ValueChanged<int> handler)
        {
            int width = Math.Max(GetStringWidth(minValue.ToString(), Game1.smallFont), GetStringWidth(maxValue.ToString(), Game1.smallFont)) + 2;
            SetScaledArea(new Rectangle(position.X, position.Y, 16 + width, 8));
            MinusArea = new Rectangle(Area.X, Area.Y, 7 * Game1.pixelZoom, Area.Height);
            PlusArea = new Rectangle(Area.X + Area.Width - 7 * Game1.pixelZoom, Area.Y, 7 * Game1.pixelZoom, Area.Height);
            MinValue = minValue;
            MaxValue = maxValue;
            Value = MinValue;
            Handler = handler;
            OptionKey = optionKey;
            OldValue = Value;
        }
        private void Resolve(Point p, Point o)
        {
            Rectangle PlusAreaOffset = new Rectangle(PlusArea.X + o.X, PlusArea.Y + o.Y, PlusArea.Height, PlusArea.Width);
            if (PlusAreaOffset.Contains(p) && Value < MaxValue)
            {
                Value++;
                Game1.playSound("drumkit6");
                return;
            }
            Rectangle MinusAreaOffset = new Rectangle(MinusArea.X + o.X, MinusArea.Y + o.Y, MinusArea.Height, MinusArea.Width);
            if (MinusAreaOffset.Contains(p) && Value > MinValue)
            {
                Game1.playSound("drumkit6");
                Value--;
                return;
            }
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            if (Disabled)
                return;
            Counter = 0;
            Limiter = 10;
            Resolve(p, o);
        }
        public override void LeftHeld(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Counter++;
            if (Disabled || Counter % Limiter != 0)
                return;
            Counter = 0;
            Limiter = Math.Max(2, Limiter - 1);
            Resolve(p, o);
        }
        public override void LeftUp(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Counter = 0;
            Limiter = 10;
            if (OldValue == Value)
                return;
            OldValue = Value;
            Handler(OptionKey, Value);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            // Minus button on the left
            b.Draw(Game1.mouseCursors, new Vector2(o.X + Area.X, o.Y + Area.Y), MinusButton, Color.White * (Disabled || Value <= MinValue ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            // Plus button on the right
            b.Draw(Game1.mouseCursors, new Vector2(o.X + Area.X + (Area.Width - Game1.pixelZoom * 6), o.Y + Area.Y), PlusButton, Color.White * (Disabled || Value >= MaxValue ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            // Box in the center
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, Background, o.X + Area.X + 6 * Game1.pixelZoom, o.Y + Area.Y, Area.Width - 12 * Game1.pixelZoom, Area.Height, Color.White, Game1.pixelZoom, false);
            // Text label in the center
            Utility.drawTextWithShadow(b, Value.ToString(), Game1.smallFont, new Vector2(o.X + Area.X + 8 * Game1.pixelZoom, o.Y + Area.Y + Game1.pixelZoom), Game1.textColor * (Disabled ? 0.33f : 1f));
        }
    }
}