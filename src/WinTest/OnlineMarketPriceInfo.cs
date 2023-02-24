using DotNet.Standard.NParsing.ComponentModel;
using DotNet.Standard.NParsing.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WinTest
{
    [ObModel(Name = "Bas_OnlineMarketPrices")]
    public class OnlineMarketPriceInfo : ObModelBase
    {
        [ObProperty(Name = "Name", Length = 50, Nullable = false)]
        public virtual string Name { get; set; }

        [ObProperty(Name = "Exchange", Length = 50, Nullable = false)]
        public virtual string Exchange { get; set; }

        [ObProperty(Name = "Price", Length = 15, Precision = 4, Nullable = true)]
        public virtual decimal? Price { get; set; }

        /// <summary>
        /// 涨跌
        /// </summary>
        [ObProperty(Name = "Change", Length = 15, Precision = 4, Nullable = true)]
        public virtual decimal? Change { get; set; }

        /// <summary>
        /// 持仓量
        /// </summary>
        [ObProperty(Name = "Position", Length = 15, Precision = 4, Nullable = true)]
        public virtual decimal? Position { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        [ObProperty(Name = "Volume", Length = 15, Precision = 4, Nullable = true)]
        public virtual decimal? Volume { get; set; }

        /// <summary>
        /// 昨结算价
        /// </summary>
        [ObProperty(Name = "SettlePrice", Length = 15, Precision = 4, Nullable = true)]
        public virtual decimal? SettlePrice { get; set; }

        /// <summary>
        /// 今开盘价
        /// </summary>
        [ObProperty(Name = "OpenPrice", Length = 15, Precision = 4, Nullable = true)]
        public virtual decimal? OpenPrice { get; set; }

        /// <summary>
        /// 最低价
        /// </summary>
        [ObProperty(Name = "LowPrice", Length = 15, Precision = 4, Nullable = true)]
        public virtual decimal? LowPrice { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        [ObProperty(Name = "HighPrice", Length = 15, Precision = 4, Nullable = true)]
        public virtual decimal? HighPrice { get; set; }
    }
}
