using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Zeroconf;

namespace IRKitTool.Models
{
    /// <summary>
    /// IRKit
    /// </summary>
    public class IRKit
    {
        private static HttpClient httpClient;

        /// <summary>
        /// IRKit IPアドレス取得時の最大リトライ回数
        /// </summary>
        private readonly int MAX_RETRY_COUNT = 5;

        public ObservableCollection<IRCommand> IRCommandList { get; set; }

        public IRKit()
        {
            httpClient = new HttpClient();

            IRCommandList = new ObservableCollection<IRCommand>() {new IRCommand("コマンド1", "1"), new IRCommand("コマンド2", "2") };
        }

        /// <summary>
        /// リモコン操作を実行する
        /// </summary>
        public async Task ExecCommandAsync(IRCommand command)
        {
            var content = new StringContent(command.Signal, Encoding.UTF8);

            var ip = await GetIPAddressAsync();
            var request = new HttpRequestMessage(HttpMethod.Post, string.Format("http://{0}/messages", ip));
            request.Headers.Add("X-Requested-With", "curl");
            request.Content = content;

            await httpClient.SendAsync(request);
        }

        /// <summary>
        /// 赤外線信号を取得する
        /// </summary>
        /// <returns>赤外線信号（取得不可の場合は空文字列）</returns>
        public async Task<string> GetMessagesAsync()
        {
            var ip = await GetIPAddressAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format("http://{0}/messages", ip));
            request.Headers.Add("X-Requested-With", "curl");

            var response = await httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// IRKitのIPアドレスを取得する
        /// </summary>
        /// <returns>IPアドレス（取得失敗の場合は空文字列）</returns>
        private async Task<string> GetIPAddressAsync()
        {
            Console.WriteLine("===== IPアドレス探索開始 =====");
            //var sd = new ServiceDiscovery();
            //sd.ServiceInstanceDiscovered += (s, e) => { Console.WriteLine(e); };
            string ip = "";

            for (int i = 0; i < MAX_RETRY_COUNT; i++)
            {
                ILookup<string, string> domains = await ZeroconfResolver.BrowseDomainsAsync();
                var responses = await ZeroconfResolver.ResolveAsync(domains.Select(g => g.Key));
                var response = responses.Where(x => x.DisplayName.ToUpper().Contains("IRKIT")).FirstOrDefault();
                
                if (response != null && response.IPAddress != string.Empty)
                {
                    ip = response.IPAddress;
                    Console.WriteLine(ip);
                    break;
                }
            }

            return ip;
        }
    }
}
