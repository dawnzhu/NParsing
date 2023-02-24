using DotNet.Standard.Common.Utilities;
using DotNet.Standard.NParsing.Factory;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace WinTest
{
    public partial class FrmMain : Form
    {
        private WebBrowser _webBrowser;
        private string _dceUrl = "http://www.dce.com.cn//webquote/futures_quote_ajax?varietyid=";
        private string _shfeUrl = "https://www.shfe.com.cn/data/delayed_market_data.dat?rnd=";
        private string _czceUrl = "http://www.czce.com.cn/cn/DFSStaticFiles/Future/Quotation/ChineseFutureQuotation.htm";
        private System.Windows.Forms.Timer _timerDce;
        private System.Windows.Forms.Timer _timerShfe;
        private System.Windows.Forms.Timer _timerCzce;
        private string[] _dceCommodities = new[] { "a", "b", "m", "y", "p", "c", "cs", "jd", "rr", "lh", "fb", "bb", "l", "v", "pp", "eb", "j", "jm", "i", "eg", "pg" };
        private int _dceIndex = 0;
        public FrmMain()
        {
            InitializeComponent();
            InitializeWebBrowser();
            _timerCzce = new System.Windows.Forms.Timer { Interval = 1 };
            _timerShfe = new System.Windows.Forms.Timer { Interval = 5000 };
            _timerDce = new System.Windows.Forms.Timer { Interval = 10000 };
            _timerCzce.Tick += async (sender, e) =>
            {
                try
                {
                    _timerCzce.Interval = 30000;
                    var htmlString = "";
                    using (var client = new HttpClient())
                    {
                        htmlString = await client.GetStringAsync(_czceUrl);
                    }
                    var datas = Regex.Matches(htmlString, @"<tr>[^<]*<td><a[^>]*>([^<]*)</a></td>[^<]*<td>([^<]*)</td>[^<]*<td>([^<]*)</td>[^<]*<td>([^<]*)</td>[^<]*<td>([^<]*)</td>[^<]*<td>([\d,\.]*)</td>[^<]*<td>([\d,\.]*).*</td>[^<]*<td>([\d,\.]*)</td>[^<]*<td>([\d,\.]*)</td>[^<]*<td>([\d,\.]*)</td>[^<]*<td>([\d,\.]*)</td>");
                    var list = new List<OnlineMarketPriceInfo>();
                    foreach (Match d in datas)
                    {
                        try
                        {
                            var model = ObModel.Create<OnlineMarketPriceInfo>();
                            model.Exchange = "CZCE";
                            model.Name = d.Groups[1].Value;
                            if (decimal.TryParse(d.Groups[6].Value, out var price))
                            {
                                model.Price = price;
                            }
                            if (decimal.TryParse(d.Groups[11].Value, out var position))
                            {
                                model.Position = position;
                            }
                            if (decimal.TryParse(d.Groups[10].Value, out var volume))
                            {
                                model.Volume = volume;
                            }
                            if (decimal.TryParse(d.Groups[2].Value, out var settlePrice))
                            {
                                model.SettlePrice = settlePrice;
                            }
                            if (decimal.TryParse(d.Groups[3].Value, out var openPrice))
                            {
                                model.OpenPrice = openPrice;
                            }
                            if (decimal.TryParse(d.Groups[5].Value, out var lowPrice))
                            {
                                model.LowPrice = lowPrice;
                            }
                            if (decimal.TryParse(d.Groups[4].Value, out var highPrice))
                            {
                                model.HighPrice = highPrice;
                            }
                            if (model.Price.HasValue || model.SettlePrice.HasValue)
                            {
                                model.Change = model.Price - model.SettlePrice;
                            }
                            list.Add(model);
                        }
                        catch (Exception er)
                        {
                            LogUtil.WriteLog("CZCE", er);
                        }
                    }
                    //_ = _onlineMarketPriceService.UpdateAsync(list);
                    lbCzce.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("CZCE2", er);
                }
            };
            _timerShfe.Tick += async (sender, e) =>
            {
                try
                {
                    _timerShfe.Interval = 30000;
                    var csvString = "";
                    using (var client = new HttpClient())
                    {
                        csvString = await client.GetStringAsync(_shfeUrl + DateTime.Now.Ticks);
                    }
                    var datas = Regex.Matches(csvString, @"[^,]*,[^,]*,([^,]*),([^,]*),[^,]*,([^,]*),[^,]*,[^,]*,[^,]*,[^,]*,([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,]*,[^,\n]*\n");
                    var list = new List<OnlineMarketPriceInfo>();
                    foreach (Match d in datas)
                    {
                        try
                        {
                            var model = ObModel.Create<OnlineMarketPriceInfo>();
                            model.Exchange = "SHFE";
                            model.Name = d.Groups[1].Value;
                            if (decimal.TryParse(d.Groups[4].Value, out var price))
                            {
                                model.Price = price;
                            }
                            if (decimal.TryParse(d.Groups[8].Value, out var position))
                            {
                                model.Position = position;
                            }
                            if (decimal.TryParse(d.Groups[7].Value, out var volume))
                            {
                                model.Volume = volume;
                            }
                            if (decimal.TryParse(d.Groups[2].Value, out var settlePrice))
                            {
                                model.SettlePrice = settlePrice;
                            }
                            if (decimal.TryParse(d.Groups[3].Value, out var openPrice))
                            {
                                model.OpenPrice = openPrice;
                            }
                            if (decimal.TryParse(d.Groups[6].Value, out var lowPrice))
                            {
                                model.LowPrice = lowPrice;
                            }
                            if (decimal.TryParse(d.Groups[5].Value, out var highPrice))
                            {
                                model.HighPrice = highPrice;
                            }
                            if (model.Price.HasValue || model.SettlePrice.HasValue)
                            {
                                model.Change = model.Price - model.SettlePrice;
                            }
                            list.Add(model);
                        }
                        catch (Exception er)
                        {
                            LogUtil.WriteLog("SHFE", er);
                        }
                    }
                    //_ = _onlineMarketPriceService.UpdateAsync(list);
                    lbShfe.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("SHFE2", er);
                }
            };
            _timerDce.Tick += (sender, e) =>
            {
                try
                {
                    _timerDce.Interval = 30000;
                    _dceIndex = 0;
                    _webBrowser.Url = new Uri(_dceUrl + _dceCommodities[_dceIndex]);
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("DCE", er);
                    InitializeWebBrowser();
                }
            };
        }

        private void InitializeWebBrowser()
        {
            if (_webBrowser != null)
            {
                try
                {
                    _webBrowser.Dispose();
                }
                catch (Exception)
                { }
            }
            _webBrowser = new WebBrowser();
            _webBrowser.DocumentCompleted += (sender, e) =>
            {
                try
                {
                    var text = ((WebBrowser)sender).DocumentText;
                    if (!text.StartsWith("{") || !text.EndsWith("}"))
                    {
                        //LogUtil.WriteLog("DCE3", text);
                        return;
                    }
                    var list = new List<OnlineMarketPriceInfo>();
                    var jo = JToken.Parse(text);
                    var contractIds = jo.Value<string>("contractIds").TrimEnd(',').Split(',');
                    var contractQuote = jo["contractQuote"];
                    foreach (var contractId in contractIds)
                    {
                        var model = ObModel.Create<OnlineMarketPriceInfo>();
                        model.Exchange = "DCE";
                        model.Name = contractId;
                        if (decimal.TryParse(contractQuote[contractId].Value<string>("lastPrice"), out var price))
                        {
                            model.Price = price;
                        }
                        if (decimal.TryParse(contractQuote[contractId].Value<string>("openInterest"), out var position))
                        {
                            model.Position = position;
                        }
                        if (decimal.TryParse(contractQuote[contractId].Value<string>("matchTotQty"), out var volume))
                        {
                            model.Volume = volume;
                        }
                        if (decimal.TryParse(contractQuote[contractId].Value<string>("lastClearPrice"), out var settlePrice))
                        {
                            model.SettlePrice = settlePrice;
                        }
                        if (decimal.TryParse(contractQuote[contractId].Value<string>("openPrice"), out var openPrice))
                        {
                            model.OpenPrice = openPrice;
                        }
                        if (decimal.TryParse(contractQuote[contractId].Value<string>("lowPrice"), out var lowPrice))
                        {
                            model.LowPrice = lowPrice;
                        }
                        if (decimal.TryParse(contractQuote[contractId].Value<string>("highPrice"), out var highPrice))
                        {
                            model.HighPrice = highPrice;
                        }
                        if (model.Price.HasValue || model.SettlePrice.HasValue)
                        {
                            model.Change = model.Price - model.SettlePrice;
                        }
                        list.Add(model);
                    }
                    //_onlineMarketPriceService.UpdateAsync(list);
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("DCE1", er);
                }
                try
                {
                    _dceIndex++;
                    if (_dceIndex >= _dceCommodities.Length)
                    {
                        lbDce.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        return;
                    }
                    _webBrowser.Url = new Uri(_dceUrl + _dceCommodities[_dceIndex]);
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("DCE2", er);
                }
            };
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                while (DateTime.Now.Second > 0)
                {
                    Thread.Sleep(1000);
                }
                Invoke(new Action(() =>
                {
                    _timerCzce.Start();
                    _timerShfe.Start();
                    _timerDce.Start();
                }));
            })
            { IsBackground = true }.Start();
        }
    }
}