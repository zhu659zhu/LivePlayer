using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LivePlayer
{
    public partial class M3u8Player : Form
    {
        /// <summary>
        /// 全屏按钮的Click事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; 
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized; 
            this.TopMost = true;
            panel1.Width = this.Width;
            panel1.Height = this.Height;
            panel1.BringToFront();
        }

        #region 导出函数

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern IntPtr libvlc_new(int argc, string[] argv);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern IntPtr libvlc_media_new_path(IntPtr pInstance, string path);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern IntPtr libvlc_media_player_new_from_media(IntPtr pMedia);

        // 设置图像输出的窗口
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern void libvlc_media_player_set_hwnd(IntPtr pPlayer, Int32 hWnd);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern int libvlc_media_player_play(IntPtr pPlayer);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern void libvlc_media_player_stop(IntPtr pPlayer);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern void libvlc_release(IntPtr pInstace);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void libvlc_media_release(IntPtr libvlc_media_inst);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern void libvlc_media_player_pause(IntPtr libvlc_mediaplayer);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void libvlc_media_player_release(IntPtr libvlc_mediaplayer);

        // 解析视频资源的媒体信息(如时长等)
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void libvlc_media_parse(IntPtr libvlc_media);

        // 返回视频的时长(必须先调用libvlc_media_parse之后，该函数才会生效)
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int64 libvlc_media_get_duration(IntPtr libvlc_media);

        // 当前播放的时间
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int64 libvlc_media_player_get_time(IntPtr libvlc_mediaplayer);

        // 设置播放位置(拖动)
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void libvlc_media_player_set_time(IntPtr libvlc_mediaplayer, Int64 time);

        // 获取和设置音量
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int libvlc_audio_get_volume(IntPtr libvlc_media_player);
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void libvlc_audio_set_volume(IntPtr libvlc_media_player, int volume);

        // 设置全屏
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void libvlc_set_fullscreen(IntPtr libvlc_media_player, bool isFullScreen);

        //获取和改变播放速率  流媒体可用 1为正常速度
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float libvlc_media_player_get_rate(IntPtr libvlc_media_player);
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int libvlc_media_player_set_rate(IntPtr libvlc_media_player, float rate);

        //获取和改变播放进度  流媒体可用
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void libvlc_media_player_set_position(IntPtr libvlc_media_player, float f_pos);
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float libvlc_media_player_get_position(IntPtr libvlc_media_player);

        //获取播放状态
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int libvlc_media_player_get_state(IntPtr libvlc_media_player);

        //截屏
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int libvlc_video_take_snapshot(IntPtr libvlc_media_player, int num, string path, int width, int height);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int64 libvlc_media_player_get_chapter(IntPtr libvlc_media_player);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern IntPtr libvlc_media_new_location(IntPtr pInstance, string path);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern void libvlc_toggle_fullscreen(IntPtr pInstance, int b_fullscreen);

        #endregion

        #region 播放器变量

        private IntPtr pInstance;
        private IntPtr pMedia;
        private IntPtr pPlayer;
        private bool bPlaying;

        double TotalTime = 0;

        #endregion

        public M3u8Player()
        {
            InitializeComponent();
        }

        private void M3u8Player_Load(object sender, EventArgs e)
        {
            //PlayM3u8();
        }

        public void PlayM3u8()
        {
            
            if (bPlaying == true)
            {
                libvlc_media_player_stop(pPlayer);
            }
            //http://vzb.tc.qq.com:1863/100209403.flv?vkey=25FAE94735A94170C0CDF426A0046A4E386A0B4056D8C72BBEF0FD6C04F6B96A9B805675870E7731B47A924A6386AF23BFEB070C4C2ED9F35B735FF98436C470D1302EB7DF908B123B79878EF593DDBF&guid=20E9E815257F6F5E10C23AFE3DD4836182C7FC20
            string[] argv = new string[] { 
                "--ignore-config"
            };
            pInstance = libvlc_new(argv.Length, argv);
            //pMedia = libvlc_media_new_path(pInstance, "temp.m3u8");
            pMedia = libvlc_media_new_location(pInstance,System.Web.HttpUtility.UrlDecode(textBox2.Text) );

            pPlayer = libvlc_media_player_new_from_media(pMedia);
            libvlc_media_player_set_hwnd(pPlayer, (Int32)panel1.Handle);

            libvlc_media_player_play(pPlayer);
            bPlaying = true;

        }//播放

        private void button4_Click(object sender, EventArgs e)
        {
            if (bPlaying)
                libvlc_video_take_snapshot(pPlayer, 0, "temp.jpg", 600, 480);
        }

        private void btn_play_Click(object sender, EventArgs e)
        {
            if (bPlaying)
            {
                libvlc_media_player_stop(pPlayer);
                libvlc_media_release(pMedia);
                libvlc_release(pPlayer);
                bPlaying = false;
                this.Dispose();
            }
        }

        private void M3u8Player_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bPlaying)
            {
                libvlc_media_player_stop(pPlayer);
                libvlc_media_release(pMedia);
                bPlaying = false;
                this.Dispose();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PlayM3u8(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            this.TopMost = false;
            panel1.Width = this.Width;
            panel1.Height = this.Height-80;
        }

        public void RegexUrl(string url)
        {
            MessageBox.Show(url);
            textBox2.Text = url;
            PlayM3u8();
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
                    request.ProtocolVersion = HttpVersion.Version10;    //http版本，默认是1.1,这里设置为1.0
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                }

                request.Method = "GET";
                StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.UTF8);
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
        }

        private void M3u8Player_SizeChanged(object sender, EventArgs e)
        {
            panel1.Width = this.Width;
            panel1.Height = this.Height - 80;
            panel2.Top = this.Height - 80;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }//https验证函数


    }


}
