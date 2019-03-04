using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace jidian
{
    public partial class Form3 : Form
    {
        string version = "";
        string exename = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "newjidian.exe";
        string exefile = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        bool wanchengxiazai = false;

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            label2.Text = "当前版本：" + Properties.Settings.Default.version + "    最新版本：正在检查";
            try
            {
                System.Net.WebClient getversion = new System.Net.WebClient();
                getversion.DownloadDataAsync(new Uri(@"https://github.com/ymefg/jidian/releases/latest"));
                getversion.DownloadDataCompleted += Getversion_DownloadDataCompleted;
            }
            catch(Exception ex)
            {
                label1.Text = "更新失败，请检查你的网络。\n\n" + ex.Message;
                label2.Text = "当前版本：" + Properties.Settings.Default.version;
                progressBar1.Style = ProgressBarStyle.Blocks;
            }
        }

        private void Getversion_DownloadDataCompleted(object sender, System.Net.DownloadDataCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            //MessageBox.Show(e.Result);
            try
            {
                //MessageBox.Show(Encoding.UTF8.GetString(e.Result));
                
                string result = Encoding.UTF8.GetString(e.Result).Substring(Encoding.UTF8.GetString(e.Result).IndexOf("<a href=\"/ymefg/jidian/releases/tag/") + "<a href=\"/ymefg/jidian/releases/tag/".Length);
                string[] rspv = result.Split('"');
                version = rspv[0];
                label2.Text = "当前版本：" + Properties.Settings.Default.version + "    最新版本：" + version;
                result = Encoding.UTF8.GetString(e.Result).Substring(Encoding.UTF8.GetString(e.Result).IndexOf("<div class=\"markdown-body\">") + "<div class=\"markdown-body\">".Length + 8);
                result = result.Substring(0, result.IndexOf(@"</div>"));
                //result = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(result));
                string rspi = getinfo(result);
                //rspi[0] = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(rspi[0]));
                label1.Text = version + " 更新信息：\n\n" + rspi;
                label1.Text = label1.Text + "\n\n更新时将会在程序运行文件夹内产生 newjidian.exe 和 update.bat 两个文件，请确保程序文件夹内无同名文件，否则上述文件会遭到覆盖并删除。";

                progressBar1.Style = ProgressBarStyle.Blocks;
                if (!Properties.Settings.Default.version.Equals(version))
                {
                    button1.Enabled = true;
                }
            }
            catch(Exception ex)
            {
                label1.Text = "由于未知原因更新失败。\n\n" + ex.Message;
                label2.Text = "当前版本：" + Properties.Settings.Default.version;
                progressBar1.Style = ProgressBarStyle.Blocks;
            }
        }

        private string getinfo(string s)
        {
            
            s = s.Replace("<br>", "");
            s = s.Replace("<ol>", "");
            s = s.Replace("<ul>", "");
            s = s.Replace("</ol>", "");
            s = s.Replace("</ul>", "");
            s = s.Replace("<li>", "● ");
            s = s.Replace("</li>", "");
            s = s.Replace("<p>", "");
            s = s.Replace("</p>", "");
            

            return s;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (wanchengxiazai)
            {
                //byte[] Save = global::jidian.Properties.Resources.update;
                byte[] Save = System.Text.Encoding.Default.GetBytes(global::jidian.Properties.Resources.update);
                FileStream fsObj = new FileStream(exefile + @"update.bat", FileMode.CreateNew);
                fsObj.Write(Save, 0, Save.Length);
                fsObj.Close();

                System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "update.bat");
                System.Environment.Exit(0);
            }
            else
            {
                try
                {
                    button1.Enabled = false;
                    button1.Text = "正在下载";
                    label2.Text = "下载中 (0%)";
                    System.Net.WebClient download = new System.Net.WebClient();
                    //MessageBox.Show(@"https://github.com/ymefg/jidian/releases/download/" + version + @"/jidian.exe");
                    download.DownloadFileAsync(new Uri(@"https://github.com/ymefg/jidian/releases/download/" + version + @"/jidian.exe"), exename);
                    download.DownloadProgressChanged += Download_DownloadProgressChanged;
                    download.DownloadFileCompleted += Download_DownloadFileCompleted;
                }
                catch (Exception ex)
                {
                    label1.Text = "更新失败，请检查你的网络。\n\n" + ex.Message;
                    label2.Text = "当前版本：" + Properties.Settings.Default.version;
                }

            }
        }

        private void Download_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            wanchengxiazai = true;
            button1.Text = "重启";
            label2.Text = "下载完成，请重启以完成更新。";
            button1.Enabled = true;
        }

        private void Download_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
            progressBar1.Value = e.ProgressPercentage;
            label2.Text = "下载中 (" + e.ProgressPercentage.ToString() + "%)";
        }
    }
}
