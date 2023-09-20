using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{
    public record struct Rectangle
    {

        public Rectangle(float left, float top, float width, float height)
        {
            if (width < 0 || height < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
        }

        public float Left { get; init; }
        public float Top { get; init; }

        public float Width { get; init; }

        public float Height { get; init; }
        public float Right => Left + Width;
        public float Bottom => Top + Height;
        public Vector2 Size => new Vector2(Width, Height);
        public bool HasArea => Width > 0 && Height > 0;

        public static readonly Rectangle Empty = new Rectangle();

        /// <summary>
        /// Returns a copy of this rectangle that is reduced to fit within the specified boundaries
        /// </summary>
        /// <param name="boundaries"></param>
        /// <returns></returns>
        public Rectangle Clamp(Rectangle boundaries)
        {
            var x1 = Math.Max(Left, boundaries.Left);
            var x2 = Math.Min(Right, boundaries.Right);
            var width = Math.Max(x2 - x1, 0);

            var y1 = Math.Max(Top, boundaries.Left);
            var y2 = Math.Min(Bottom, boundaries.Right);
            var height = Math.Max(y2 - y1, 0);

            return new Rectangle(x1, y1, width, height);
        }


        public static Rectangle Cover(IEnumerable<Vector2> points)
        {
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;

            foreach (var point in points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);

                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }

            if (minX == float.MaxValue)
            {
                return Empty;
            }

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public static Rectangle Cover(IEnumerable<Rectangle> rects)
        {
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;

            foreach (var rect in rects)
            {
                minX = Math.Min(minX, rect.Left);
                minY = Math.Min(minY, rect.Top);

                maxX = Math.Max(maxX, rect.Right);
                maxY = Math.Max(maxY, rect.Bottom);
            }

            if (minX == float.MaxValue)
            {
                return Empty;
            }

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }


        public Rectangle Expand(float amount)
        {
            return new Rectangle(Left - amount, Top - amount, Width + amount * 2, Height + amount * 2);
        }

        public Rectangle ExpandPercentage(float percentage)
        {
            var size = Math.Max(Width, Height);
            return Expand(percentage * size / 100f);
        }


        public Vector2 Center()
        {
            return new Vector2(Left + Width / 2, Top + Height / 2);
        }

        public bool Contains(Vector2 v) => v.X >= Left
                                           && v.X <= Right
                                           && v.Y >= Top
                                           && v.Y <= Bottom;


        public bool Overlaps(Rectangle r) => !DoesNotOverlap(r);

        private bool DoesNotOverlap(Rectangle r) => Left > r.Right
                                                    || Right < r.Left
                                                    || Top > r.Bottom
                                                    || Bottom < r.Top;



        public IEnumerable<Vector2> Corners()
        {
            yield return new Vector2(Left, Top);
            yield return new Vector2(Right, Top);
            yield return new Vector2(Right, Bottom);
            yield return new Vector2(Left, Bottom);
        }

        public Rectangle Translate(Vector2 v) => new(Left + v.X, Top + v.Y, Width, Height);

        #region Dragging

        /// <summary>
        /// Drags the left side of this rectangle, keeping the right side constant. Will clamp amount if it would lead to a negative width
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Rectangle DragLeft(float amount)
        {
            amount = Math.Min(amount, Width);
            return this with
            {
                Left = Left + amount,
                Width = Width - amount
            };
        }

        /// <summary>
        /// Drags the right side of this rectangle, keeping the left side constant. Will clamp amount if it would lead to a negative width
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Rectangle DragRight(float amount)
        {
            amount = Math.Max(amount, -Width);
            return this with
            {
                Width = Width + amount
            };
        }


        /// <summary>
        /// Drags the top side of this rectangle, keeping the bottom side constant. Will clamp amount if it would lead to a negative height
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Rectangle DragTop(float amount)
        {
            amount = Math.Max(amount, -Height);
            return this with
            {
                Top = Top + amount,
                Height = Height + amount
            };
        }

        /// <summary>
        /// Drags the bottom side of this rectangle, keeping the top side constant. Will clamp amount if it would lead to a negative height
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Rectangle DragBottom(float amount)
        {
            amount = Math.Min(amount, Height);
            return this with
            {
                Height = Height - amount
            };
        }

        #endregion
    }
}
