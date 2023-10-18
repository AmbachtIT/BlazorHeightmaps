using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Blazor.Dialogs
{
    public interface ISelectionDialog<T>
    {

        IEnumerable<T> AvailableOptions { get; set; }

        Type ComponentType { get; set; }

    }

    public interface IMultipleSelectionDialog<T> : ISelectionDialog<T>
    {
        List<T> SelectedOptions { get; set; }
    }

    public interface ISingleSelectionDialog<T> : ISelectionDialog<T>
    {
        T SelectedOption { get; set; }
    }

}
