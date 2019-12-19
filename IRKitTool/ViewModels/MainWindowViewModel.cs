using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using Makaretu.Dns;
using System.Threading;
using Zeroconf;
using System.Net.Http;
using System.Text;
using System.Collections.ObjectModel;
using Prism.Services.Dialogs;
using Reactive.Bindings.Extensions;

namespace IRKitTool.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ReactiveCommand GetIPAddress { get; private set; } = new ReactiveCommand();

        public ReactiveCommand AddList { get; private set; }

        public ReactiveCommand<object> OpenDialog { get; private set; }

        private static HttpClient httpClient;

        public ReadOnlyReactiveCollection<Models.IRCommand> IRCommandList { get; set; }

        private readonly Models.IRKit iRKit;

        /// <summary>
        /// 赤外線コマンド実行
        /// </summary>
        public ReactiveCommand<object> ExecIRCommand { get; }

        /// <summary>
        /// 選択中の赤外線コマンド
        /// </summary>
        public Models.IRCommand SelectedIRCommand { get; set; }

        private readonly System.Reactive.Disposables.CompositeDisposable disposables = new System.Reactive.Disposables.CompositeDisposable();

        public MainWindowViewModel(IDialogService dialogService, Models.IRKit iRKit)
        {
            GetIPAddress.Subscribe(async _ => await SendMessageAsync());
            httpClient = new HttpClient();
            this.iRKit = iRKit;

            IRCommandList = iRKit.IRCommandList.ToReadOnlyReactiveCollection();

            AddList = new ReactiveCommand();
            AddList.Subscribe(_ => iRKit.IRCommandList.Add(new Models.IRCommand("コマンド3", "3")));

            IDialogResult result = null;
            OpenDialog = new ReactiveCommand<object>()
                .WithSubscribe(_ => dialogService.ShowDialog("AddIRCommandDialog", null, ret => result = ret))
                .AddTo(disposables);

            ExecIRCommand = new ReactiveCommand<object>()
                .WithSubscribe(async _ => await ExecIRCommandAsync())
                .AddTo(disposables);

            SelectedIRCommand = null;
        }

        private async System.Threading.Tasks.Task GetIPAddressFromBonjourAsync()
        {
            Console.WriteLine("===== IPアドレス探索開始 =====");
            //var sd = new ServiceDiscovery();
            //sd.ServiceInstanceDiscovered += (s, e) => { Console.WriteLine(e); };

            ILookup<string, string> domains = await ZeroconfResolver.BrowseDomainsAsync();
            var responses = await ZeroconfResolver.ResolveAsync(domains.Select(g => g.Key));
            foreach (var resp in responses)
            {
                Console.WriteLine(resp.DisplayName);
                Console.WriteLine(resp.IPAddress);
            }

            Console.WriteLine("=============================");
        }

        private async System.Threading.Tasks.Task GetMessagesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://192.168.0.4/messages");
            request.Headers.Add("X-Requested-With", "curl");

            var response = await httpClient.SendAsync(request);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        private async System.Threading.Tasks.Task SendMessageAsync()
        {
            var data = "{\"format\":\"raw\",\"freq\":38,\"data\":[4713,1190,1190,1190,1190,1190,1037,1319,1150,1150,1150,1150,1150,1150,1150,1150,2368,1150,1150,1150,1150,1150,1150,1150,1150,54214,4554,1190,1190,1190,1037,1319,1150,1150,1150,1319,1150,1319,1150,1319,1150,1319,2288,1190,1190,1190,1037,1319,1037,1319,1037,54214,4554,1232,1111,1232,1111,1232,1111,1232,1111,1232,1111,1232,1111,1232,1111,1232,2288,1319,1002,1319,1111,1232,1111,1232,1111,54214,4554,1232,1111,1232,1111,1366,1111,1111,1111,1232,1037,1232,1037,1232,1037,1232,2288,1232,1232,1232,1111,1275,1111,1275,1111,54214,4554,1232,1037,1232,1232,1232,1037,1232,1037,1232,1037,1232,1037,1232,1037,1232,2211,1319,1111,1232,1111,1111,1111,1232,1111,54214,4713,1232,1111,1232,1111,1232,1002,1366,1111,1232,1111,1232,1111,1232,1111,1232,2211,1319,1150,1150,1150,1150,1150,1150,1150]}";
            //var content = new StringContent(data, Encoding.UTF8);

            //var request = new HttpRequestMessage(HttpMethod.Post, "http://192.168.0.4/messages");
            //request.Headers.Add("X-Requested-With", "curl");
            //request.Content = content;

            //var response = await httpClient.SendAsync(request);

            Models.IRCommand command = new Models.IRCommand("地デジNHK", data);

            Models.IRKit iRKit = new Models.IRKit();
            await iRKit.ExecCommandAsync(command);
        }

        private async System.Threading.Tasks.Task ExecIRCommandAsync()
        {
            if (SelectedIRCommand != null)
            {
                await iRKit.ExecCommandAsync(SelectedIRCommand);
            }
        }

        private void Dispose()
        {
            this.disposables.Dispose();
        }
    }
}
