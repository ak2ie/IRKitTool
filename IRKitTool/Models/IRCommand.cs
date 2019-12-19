using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRKitTool.Models
{
    /// <summary>
    /// 赤外線操作
    /// </summary>
    public class IRCommand
    {
        /// <summary>
        /// リモコン操作名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 赤外線信号
        /// </summary>
        public string Signal { get; private set; }

        public IRCommand(string name, string signal)
        {
            Name = name;

            if (signal == string.Empty)
            {
                throw new ArgumentException("赤外線信号が空文字列");
            }
            Signal = signal;
        }
    }
}
