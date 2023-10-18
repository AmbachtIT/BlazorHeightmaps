using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{

    public record struct Rectangle<T> : IFormattable where T : IFloatingPoint<T>
    {

      public static readonly T Two = T.One + T.One;

        public Rectangle(T left, T top, T width, T height)
        {
            if (width < T.Zero || height < T.Zero)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
        }

        public T Left { get; init; }
        public T Top { get; init; }

        public T Width { get; init; }

        public T Height { get; init; }
        public T Right => Left + Width;
        public T Bottom => Top + Height;
        public Vector2<T> Size => new Vector2<T>(Width, Height);
        public bool HasArea => Width > T.Zero && Height > T.Zero;

        public static readonly Rectangle<T> Empty = new();

        /// <summary>
        /// Returns a copy of this rectangle that is reduced to fit within the specified boundaries
        /// </summary>
        /// <param name="boundaries"></param>
        /// <returns></returns>
        public Rectangle<T> Clamp(Rectangle<T> boundaries)
        {
            var x1 = T.Max(Left, boundaries.Left);
            var x2 = T.Min(Right, boundaries.Right);
            var width = T.Max(x2 - x1, T.Zero);

            var y1 = T.Max(Top, boundaries.Left);
            var y2 = T.Min(Bottom, boundaries.Right);
            var height = T.Max(y2 - y1, T.Zero);

            return new Rectangle<T>(x1, y1, width, height);
        }


        


        public Rectangle<T> Expand(T amount)
        {
            return new Rectangle<T>(Left - amount, Top - amount, Width + (amount + amount), Height + (amount + amount));
        }

        public Rectangle<T> Expand(T amountX, T amountY)
        {
          return new Rectangle<T>(Left - amountX, Top - amountY, Width + (amountX + amountX), Height + (amountY + amountY));
        }


        public Rectangle<T> ExpandFraction(T fraction) => Expand(fraction * Width, fraction * Height);


    public Vector2<T> Center()
        {
            return new Vector2<T>(Left + Width / Two, Top + Height / Two);
        }

        public bool Contains(Vector2<T> v) => v.X >= Left
                                           && v.X <= Right
                                           && v.Y >= Top
                                           && v.Y <= Bottom;


        public bool Overlaps(Rectangle<T> r) => !DoesNotOverlap(r);

        private bool DoesNotOverlap(Rectangle<T> r) => Left > r.Right
                                                       || Right < r.Left
                                                       || Top > r.Bottom
                                                       || Bottom < r.Top;



        public IEnumerable<Vector2<T>> Corners()
        {
            yield return new Vector2<T>(Left, Top);
            yield return new Vector2<T>(Right, Top);
            yield return new Vector2<T>(Right, Bottom);
            yield return new Vector2<T>(Left, Bottom);
        }

        public Vector2<T> TopLeft() => new(Left, Top);
        public Vector2<T> BottomRight() => new(Right, Bottom);

		public Rectangle<T> Translate(Vector2<T> v) => new(Left + v.X, Top + v.Y, Width, Height);

        #region Dragging

        /// <summary>
        /// Drags the left side of this rectangle, keeping the right side constant. Will clamp amount if it would lead to a negative width
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Rectangle<T> DragLeft(T amount)
        {
            amount = T.Min(amount, Width);
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
        public Rectangle<T> DragRight(T amount)
        {
            amount = T.Max(amount, -Width);
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
        public Rectangle<T> DragTop(T amount)
        {
            amount = T.Max(amount, -Height);
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
        public Rectangle<T> DragBottom(T amount)
        {
            amount = T.Min(amount, Height);
            return this with
            {
                Height = Height - amount
            };
        }

        #endregion

        public string ToString(string format, IFormatProvider formatProvider)
        {
	        if (string.IsNullOrEmpty(format))
	        {
		        return base.ToString();
	        }

            var builder = new StringBuilder();
            foreach (var c in format)
            {
	            builder.Append(c switch
	            {
		            'l' or 'L' => Left.ToString(null, formatProvider),
		            'r' or 'R' => Right.ToString(null, formatProvider),
		            't' or 'T' => Top.ToString(null, formatProvider),
		            'b' or 'B' => Bottom.ToString(null, formatProvider),
		            'w' or 'W' => Width.ToString(null, formatProvider),
		            'h' or 'H' => Height.ToString(null, formatProvider),
		            _ => c.ToString()
	            });
            } 
            return builder.ToString();
        }

        public string ToString(string format) => ToString(format, CultureInfo.InvariantCulture);

		public override string ToString() => ToString("(L, T)-(R, B)");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
		public Rectangle<T> ExpandToMatchRatio(Vector2<T> size)
        {
	        var scaleX = Width / size.X;
            var scaleY = Height / size.Y;
            var scale = T.Min(scaleX, scaleY);

            var newSize = size * scale;
            return Around(Center(), newSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Rectangle<T> ShrinkToMatchRatio(Vector2<T> size)
        {
	        var scaleX = Width / size.X;
	        var scaleY = Height / size.Y;
	        var scale = T.Max(scaleX, scaleY);

	        var newSize = size * scale;
	        return Around(Center(), newSize);
        }



		public static Rectangle<T> Around(Vector2<T> center, Vector2<T> size)
        {
	        var half = size / Two;
	        return new Rectangle<T>(center.X - half.X, center.Y - half.Y, size.X, size.Y);
        }
    }


    
}
