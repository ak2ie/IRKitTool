using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;

namespace IRKitTool.ViewModels
{
    public class AddIRCommandDialogViewModel : BindableBase, IDialogAware
    {
        /// <summary>
        /// ダイアログタイトル
        /// </summary>
        public string Title => "コマンド追加";

        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// 赤外線コマンド名称
        /// </summary>
        [Required(ErrorMessage = "必須")]
        public ReactiveProperty<string> Name { get; private set; }

        /// <summary>
        /// IRKitから取得した赤外線信号
        /// </summary>
        [Required(ErrorMessage ="必須")]
        public ReactiveProperty<string> IRCommandStr { get; private set; }

        /// <summary>
        /// 赤外線コマンドを登録する
        /// </summary>
        public ReactiveCommand<object> RegisterIRCommand { get; }

        /// <summary>
        /// IRKitから赤外線信号を取得する
        /// </summary>
        public AsyncReactiveCommand<object> RegisterIRString { get; }

        /// <summary>
        /// 赤外線コマンド登録状況
        /// </summary>
        public ReactiveProperty<string> CommandRegistStatus { get; }

        private readonly System.Reactive.Disposables.CompositeDisposable disposables = new System.Reactive.Disposables.CompositeDisposable();

        private readonly Models.IRKit iRKit;

        public AddIRCommandDialogViewModel(Models.IRKit iRKit)
        {
            Name = new ReactiveProperty<string>("")
                .SetValidateAttribute(() => this.Name)
                .AddTo(disposables);

            IRCommandStr = new ReactiveProperty<string>("")
                .SetValidateAttribute(() => this.IRCommandStr)
                .AddTo(disposables);

            RegisterIRCommand = new[]
            {
                this.Name.ObserveHasErrors,
                IRCommandStr.ObserveHasErrors
            }
            .CombineLatest(x => x.All(y => !y))
            .ToReactiveCommand()
            .WithSubscribe(_ => RegistIRCommand())
            .AddTo(disposables);

            RegisterIRString = new AsyncReactiveCommand<object>()
                .WithSubscribe(async _ => await GetIRStringAsync())
                .AddTo(disposables);

            CommandRegistStatus = new ReactiveProperty<string>("未登録").AddTo(disposables);

            this.iRKit = iRKit;
        }

        /// <summary>
        /// IRKitから赤外線信号を取得する
        /// </summary>
        private async System.Threading.Tasks.Task GetIRStringAsync()
        {
            var signalStr = await iRKit.GetMessagesAsync();
            if (signalStr != string.Empty)
            {
                IRCommandStr.Value = signalStr;
                this.CommandRegistStatus.Value = "登録済";
            } else
            {
                IRCommandStr.Value = "";
                this.CommandRegistStatus.Value = "未登録";
            }
        }

        private void RegistIRCommand()
        {
            iRKit.IRCommandList.Add(new Models.IRCommand(Name.Value, IRCommandStr.Value));
        }

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

        private void Dispose()
        {
            this.disposables.Dispose();
        }
    }
}
