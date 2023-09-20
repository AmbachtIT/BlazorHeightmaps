using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Blazor.Svg
{
    public abstract class SvgLength
    {

        public static SvgPixelLength[] GetSizes(float available, SvgLength[] sizes)
        {
            var left = available;
            foreach (var size in sizes.OfType<SvgPixelLength>())
            {
                left -= size.Amount;
            }

            var relative = left / sizes.OfType<SvgRelativeLength>().Sum(s => s.Amount);
            var result = new SvgPixelLength[sizes.Length];
            for (var s = 0; s < sizes.Length; s++)
            {
                var size = sizes[s];
                if (size is SvgPixelLength ps)
                {
                    result[s] = ps;
                }
                else if (size is SvgRelativeLength rs)
                {
                    result[s] = new SvgPixelLength(rs.Amount * relative);
                }
                else
                {
                    throw new NotImplementedException("You'll need to convert auto sizes to pixel sizes first");
                }
            }

            return result;
        }


        public static SvgLength Px(float px) => new SvgPixelLength(px);

        public static SvgLength Relative(float amount) => new SvgRelativeLength(amount);

        public static SvgLength Auto() => new SvgAutoLength();

    }

    public class SvgPixelLength : SvgLength
    {
        public SvgPixelLength(float amount)
        {
            Amount = amount;
        }

        public float Amount { get; }
    }

    public class SvgRelativeLength : SvgLength
    {
        public SvgRelativeLength(float amount)
        {
            Amount = amount;
        }

        public float Amount { get; }
    }

    /// <summary>
    /// The size is determined by the size of the children.
    /// </summary>
    public class SvgAutoLength : SvgLength
    {

    }
}
