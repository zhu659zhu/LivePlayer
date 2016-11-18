using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LivePlayer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshSource();
            //M3u8Player f1 = new M3u8Player();

            //f1.RegexUrl("http://qqlive.dnion.com:1863/103020102.flv?vkey=8DFC9D8C1B4B1EADE1B9DAD2F5481D13F7D9BC28EE421096C8CC43AE1943093F309F8BF5E53416EFABB99AB4F024ECE6C94AA63FE5AAD7D7E08E1DA2A6269C27563DC39BC2EB462C8D5355764E10BD759446473673D5F0E8");
            //f1.ShowDialog();
        }

        public void RefreshSource()
        {
            this.listView1.Clear();

            this.listView1.Columns.Add("场次", 130, HorizontalAlignment.Left); //一步添加
            this.listView1.Columns.Add("地址", 320, HorizontalAlignment.Left); //一步添加

            int i = 0;
            string res = GetHtml("http://www.jrszhibo.com/d/js/js/1458573304.js");
            Regex pattern = new Regex(@"<div class=""tit"">([\w\W]+?</script>[\w\W]+?)</div>");
            MatchCollection matchsMade = pattern.Matches(res);
            if (matchsMade.Count == 0)
            {
                MessageBox.Show("error...");
            }
            //http://nba.tmiaoo.com/n/100204102/p.html  //选择清晰度
            //http://nba.tmiaoo.com/n/live.html?id=100204102  //选择之后直播地址
            //http://nba.tmiaoo.com/po/100204102.html



            //http://nba.tmiaoo.com/po/100204102.php
            foreach (Match pat in matchsMade)
            {
                //MessageBox.Show(pat.Groups[1].Value);
                Regex pattern2 = new Regex(@"href=""[\w\W]+?(http://[\w\W]+?m3u8[\w\W]*?)""");
                Match matchMode = pattern2.Match(pat.Groups[1].Value);
                if (matchMode.Success)
                {
                    //listView1.Items.Add(matchMode.Groups[1].Value);
                    //<span>[\w\W]+?</span>([\w\W]+?)</div> 
                    Regex pattern3 = new Regex(@"<span>[\w\W]+?</span>([\w\W]+?)</div>");
                    Match matchMode3 = pattern3.Match(pat.Groups[1].Value);
                    if (matchMode3.Success)
                    {
                        this.listView1.BeginUpdate();
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = matchMode3.Groups[1].Value.Replace(" ", "");
                        lvi.SubItems.Add(matchMode.Groups[1].Value);
                        this.listView1.Items.Add(lvi);
                        this.listView1.EndUpdate();
                    }
                }
            }
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("暂无可播放的直播源...");
            }
        }

        private string GetHtml(string url)
        {
            try
            {
                string ret = string.Empty;

                HttpWebRequest request = null;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    //对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version11;    //http版本，默认是1.1,这里设置为1.0
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                }
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"; 

                request.Method = "GET";
                StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                return ret;
            }
            catch (Exception e)
            {
                return "";
            }
        }//获取网页内容

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }//https验证函数


        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.FocusedItem!=null)
            {
                M3u8Player f2 = new M3u8Player();
                f2.Show();
                f2.RegexUrl(listView1.FocusedItem.SubItems[1].Text);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //RefreshSource();
        }
    }
}
