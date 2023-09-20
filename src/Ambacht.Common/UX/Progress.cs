using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;

namespace Ambacht.Common.UX
{
    public class Progress
    {

        public string Task { get; set; } = "Working";

        public Percentage<float> Value { get; set; } = Percentage<float>.FromFraction(float.NaN);

        public override string ToString()
        {
            if (float.IsNaN(Value.FractionValue))
            {
                return Task;
            }
            else
            {
                return $"{Task} ({Value:0})";
            }
        } 
    }
}
