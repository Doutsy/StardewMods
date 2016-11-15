﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.Menus
{
    public class ClickableHeartsComponent : BaseInteractiveMenuComponent
    {
        protected readonly static Rectangle HeartFull = new Rectangle(211, 428, 7, 6);
        protected readonly static Rectangle HeartEmpty = new Rectangle(218, 428, 7, 6);
        public int Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = Math.Min(Math.Max(0, value), MaxValue);
            }
        }
        public event ValueChanged<int> Handler;
        protected int _Value;
        protected int OldValue;
        protected int MaxValue;
        protected bool Hovered = false;
        public ClickableHeartsComponent(Point position, int value, int maxValue, ValueChanged<int> handler=null)
        {
            if (maxValue % 2 != 0)
                maxValue++;
            SetScaledArea(new Rectangle(position.X, position.Y, 8 * (maxValue / 2), HeartEmpty.Height));
            MaxValue = maxValue;
            Value = value;
            OldValue = Value;
            if (handler != null)
                Handler += handler;
        }
        public override void HoverIn(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Hovered = true;
        }
        public override void HoverOut(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Hovered = false;
        }
        public override void LeftUp(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Value = (int)Math.Round((p.X - (Area.X + o.X)) / 4D / Game1.pixelZoom);
            if (OldValue == Value)
                return;
            OldValue = Value;
            Handler?.Invoke(this, c, m, Value);
        }
        protected double zoom025 = Game1.pixelZoom / 4;
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!Visible)
                return;
            for (int c = 0; c < MaxValue / 2; c++)
                b.Draw(Game1.mouseCursors, new Vector2(o.X + Area.X + Game1.pixelZoom + c * zoom8, o.Y + Area.Y), new Rectangle(HeartEmpty.X, HeartEmpty.Y, 7, 6), Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
            for (int c = 0; c < Value; c++)
                b.Draw(Game1.mouseCursors, new Vector2(o.X + Area.X + Game1.pixelZoom + c * zoom4, o.Y + Area.Y), new Rectangle(HeartFull.X + (c % 2 == 0 ? 0 : 4), HeartFull.Y, (c % 2 == 0 ? 4 : 3), 6), Color.White * (Hovered?0.5f:1), 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
            if (!Hovered)
                return;
            int value = Math.Min(MaxValue, (int)Math.Round((Game1.getMouseX() - (Area.X + o.X)) / zoom025));
            for (int c = 0; c < value; c++)
                b.Draw(Game1.mouseCursors, new Vector2(o.X + Area.X + Game1.pixelZoom + c * zoom4, o.Y + Area.Y), new Rectangle(HeartFull.X + (c % 2 == 0 ? 0 : 4), HeartFull.Y, (c % 2 == 0 ? 4 : 3), 6), Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
        }
    }
}