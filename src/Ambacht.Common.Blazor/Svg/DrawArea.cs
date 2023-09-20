using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Blazor.Svg
{
    public record class DrawArea
    {

        public DrawArea()
        {
        }

        public DrawArea(Vector2 size)
        {
            Size = size;
        }

        public DrawArea(float width, float height)
        {
            Size = new(width, height);
        }

        public Vector2 Size { get; private set; }

        public DrawArea SetOrientation(DrawAxis mainAxis, DrawDirection mainDirection, DrawDirection crossDirection)
        {
            return SetOrientation(new DrawOrientation(mainAxis, mainDirection),
                new DrawOrientation(mainAxis.GetPerpendicularAxis(), crossDirection));
        }


        public DrawArea SetOrientation(DrawOrientation primary, DrawOrientation secondary)
        {
            if (!primary.IsOrthogonal(secondary))
            {
                throw new ArgumentException("Axes should be orthogonal");
            }

            return this with
            {
                MainOrientation = primary,
                CrossOrientation = secondary
            };
        }

        public DrawArea SetSize(float with, float height) => this with
        {
            Size = new Vector2(with, height)
        };

        /// <summary>
        /// Pixel size of the containing SVG element.
        /// </summary>
        public Vector2 SvgPixelSize { get; set; }

        public DrawOrientation MainOrientation { get; private set; } = DrawOrientation.UnitX;
        public DrawOrientation CrossOrientation { get; private set; } = DrawOrientation.UnitY;

        public string SvgTranslate(float x, float y) => SvgTranslate(new(x, y));

        public string SvgTranslate(Vector2 v)
        {
            v = Transform(v);
            return $"translate({v.X}, {v.Y})";
        }

        public Vector2 Transform(float x, float y) => Transform(new(x, y));

        public Vector2 Transform(Vector2 v)
        {
            return MainOrientation.Transform(v.X)
                   + CrossOrientation.Transform(v.Y);
        }

        public float MainLength => MainOrientation.Axis == DrawAxis.X ? Size.X : Size.Y;
        public float CrossLength => CrossOrientation.Axis == DrawAxis.X ? Size.X : Size.Y;

        public Vector2 PrimaryEnd() => Transform(new(MainLength, 0));

        public Vector2 SecondaryEnd() => Transform(new(0, CrossLength));

        public bool IsEmpty() => Size.X == 0 || Size.Y == 0;
    }

    public enum DrawAxis
    {
        X,
        Y
    }

    public enum DrawDirection
    {
        /// <summary>
        /// Start at 0 and increase
        /// </summary>
        Normal,

        /// <summary>
        /// Start at extent and decrease
        /// </summary>
        Inverted
    }

    public record DrawOrientation(DrawAxis Axis, DrawDirection Direction)
    {
        public bool IsOrthogonal(DrawOrientation other) => IsOrthogonal(other.Axis);

        public bool IsOrthogonal(DrawAxis other)
        {
            return Axis != other;
        }

        public Vector2 Transform(float pos)
        {
            var transformed = Direction.Transform(pos);
            return Axis.ToVector(transformed);
        }

        public static readonly DrawOrientation UnitX = new(DrawAxis.X, DrawDirection.Normal);
        public static readonly DrawOrientation UnitXInverted = new(DrawAxis.X, DrawDirection.Inverted);
        public static readonly DrawOrientation UnitY = new(DrawAxis.Y, DrawDirection.Normal);
        public static readonly DrawOrientation UnitYInverted = new(DrawAxis.Y, DrawDirection.Inverted);

    }

    public static class DrawExtensions
    {
        public static float GetValue(this DrawAxis axis, Vector2 v) => axis == DrawAxis.X ? v.X : v.Y;

        public static Vector2 ToVector(this DrawAxis axis, float value) =>
            axis == DrawAxis.X ? new Vector2(value, 0) : new Vector2(0, value);


        public static float Transform(this DrawDirection direction, float value) =>
            direction == DrawDirection.Normal ? value : -value;

        public static Vector2 Transform(this DrawAxis axis, float x, float y) =>
            axis == DrawAxis.X ? new(x, y) : new(y, x);


        public static Vector2 Transform(this DrawAxis axis, Vector2 v) =>
            axis == DrawAxis.X ? v : new(v.Y, v.X);

        public static DrawAxis GetPerpendicularAxis(this DrawAxis axis) => axis == DrawAxis.X ? DrawAxis.Y : DrawAxis.X;
    }
}
