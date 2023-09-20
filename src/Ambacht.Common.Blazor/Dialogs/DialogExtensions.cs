using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Ambacht.Common.Blazor.Dialogs
{
    public static class DialogExtensions
    {



        /// <summary>
        /// Shows a dialog to pick one of the specified variables. Returns null if cancelled.
        /// </summary>
        /// <returns></returns>
        public static async Task<TItem> ShowSingleSelectionDialog<TItem, TDialog, TComponent>(this IDialogService dialogService, string title, TItem selected, IEnumerable<TItem> options, Dictionary<string, object> additionalOptions = null) where TDialog: ComponentBase, ISingleSelectionDialog<TItem>
        {
            var parameters = new DialogParameters
            {
                { nameof(ISelectionDialog<TItem>.AvailableOptions), options },
                { nameof(ISelectionDialog<TItem>.ComponentType), typeof(TComponent) },
                { nameof(ISingleSelectionDialog<TItem>.SelectedOption), selected },
            };
            var dialogOptions = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Large
            };
            PopulateSelectionDialogOptions(parameters, ref dialogOptions, additionalOptions);
            var dialog = await dialogService.ShowAsync<TDialog>(title, parameters, dialogOptions);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return default;
            }

            return (TItem) result.Data;
        }



        /// <summary>
        /// Shows a dialog to pick one of the specified variables. Returns null if cancelled.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<TItem>> ShowMultipleSelectionDialog<TItem, TDialog, TComponent>(this IDialogService dialogService, string title, List<TItem> selected, IEnumerable<TItem> options, Dictionary<string, object> additionalOptions = null) where TDialog : ComponentBase, IMultipleSelectionDialog<TItem>
        {
            var parameters = new DialogParameters
            {
                { nameof(ISelectionDialog<TItem>.AvailableOptions), options },
                { nameof(ISelectionDialog<TItem>.ComponentType), typeof(TComponent) },
                { nameof(IMultipleSelectionDialog<TItem>.SelectedOptions), selected },
            };
            var dialogOptions = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Large
            };
            PopulateSelectionDialogOptions(parameters, ref dialogOptions, additionalOptions);
            var dialog = await dialogService.ShowAsync<TDialog>(title, parameters, dialogOptions);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return default;
            }

            return (List<TItem>)result.Data;
        }

        private static void PopulateSelectionDialogOptions(DialogParameters parameters, ref DialogOptions dialogOptions, Dictionary<string, object> additionalOptions)
        {
            if (additionalOptions != null)
            {
                foreach (var kv in additionalOptions)
                {
                    if (kv.Key == nameof(DialogOptions))
                    {
                        dialogOptions = (DialogOptions)kv.Value;
                    }
                    else
                    {
                        parameters.Add(kv.Key, kv.Value);
                    }
                }
            }
        }
        

    }
}
