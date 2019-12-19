using IRKitTool.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace IRKitTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // コマンド追加画面をダイアログとして登録
            containerRegistry.RegisterDialog<AddIRCommandDialog, ViewModels.AddIRCommandDialogViewModel>();
            // IRKitオブジェクトをDiコンテナに登録
            containerRegistry.RegisterInstance(new Models.IRKit());
        }
    }
}
