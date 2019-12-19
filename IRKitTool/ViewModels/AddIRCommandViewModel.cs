using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IRKitTool.ViewModels
{
	public class AddIRCommandViewModel : BindableBase, IDialogAware
	{
        public AddIRCommandViewModel()
        {

        }

        public string Title => "コマンド追加";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
    }
}
