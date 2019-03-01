using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jidian
{
    public partial class Form2 : Form
    {
        bool ad = false;
        string[] ads = new string[12];
        int adnum = 0, shunxu = 0;
        string[] neirong = new string[2];
        string link = "https://blog.ymefg.cn/";

        public Form2()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string txt = "";
            listView1.Items.Clear();
            try
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData.GetDataPresent(DataFormats.Text))
                {
                   txt = (string)iData.GetData(DataFormats.UnicodeText);
                   //MessageBox.Show(txt);
                }
                else
                {
                    MessageBox.Show("目前剪贴板中数据不可转换为文本", "错误");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }

            int num = txt.IndexOf("课程性质");
            if (num == -1)
            {
                MessageBox.Show("请复制正确内容");
                return;
            }
            txt = txt.Substring(num + 6);
            //MessageBox.Show(txt);
            //txt = txt.Trim(new char[] { '\r' });
            string[] sptxt = txt.Split(new char[] { '\n' });
            //MessageBox.Show(sptxt[1]);

            int j = 0;

            try
            {
                //edge
                if (sptxt[1].Length == 12 && sptxt.Length >= 10)
                {
                    int itemnum = (sptxt.Length - 2) / 10;
                    int termnum = 1;

                    string[] term = new string[itemnum];
                    for (int i = 0; i < itemnum; i++)
                    {
                        term[i] = sptxt[1 + i * 10];
                    }

                    string s = term[0];
                    for (int i = 0; i < term.Length; i++)
                    {
                        if (!s.Equals(term[i]))
                        {
                            termnum++;
                            s = term[i];
                        }
                    }

                    string[] finalterm = new string[termnum];
                    float[] termachievement = new float[termnum];
                    float[] termcredit = new float[termnum];
                    float[] termpoint = new float[termnum];
                    int calitem = 0;
                    float psum = 0, asum = 0, csum = 0;
                    //finalterm[0] = term[0];
                    //s = term[0];
                    termnum = 0;
                    for (int i = 0; i < term.Length; i++)
                    {
                        if (!s.Equals(term[i]))
                        {
                            finalterm[termnum] = term[i];
                            s = term[i];
                            if (calitem != 0)
                            {
                                termachievement[termnum - 1] = asum / calitem;
                                termcredit[termnum - 1] = csum;
                                termpoint[termnum - 1] = psum / termcredit[termnum - 1];
                                calitem = 0;
                                csum = asum = psum = 0;
                            }
                            termnum++;
                        }
                        calitem++;
                        sptxt[4 + i * 10] = sptxt[4 + i * 10].Replace("优秀\r", "95");
                        sptxt[4 + i * 10] = sptxt[4 + i * 10].Replace("良好\r", "85");
                        sptxt[4 + i * 10] = sptxt[4 + i * 10].Replace("中等\r", "75");
                        sptxt[4 + i * 10] = sptxt[4 + i * 10].Replace("及格\r", "60");
                        sptxt[4 + i * 10] = sptxt[4 + i * 10].Replace("不及格\r", "0");
                        asum += Convert.ToSingle(sptxt[4 + i * 10]);
                        csum += Convert.ToSingle(sptxt[5 + i * 10]);
                        psum += getpoint(Convert.ToInt32(sptxt[4 + i * 10]), Convert.ToSingle(sptxt[5 + i * 10]));
                    }
                    termachievement[termnum - 1] = asum / calitem;
                    termcredit[termnum - 1] = csum;
                    termpoint[termnum - 1] = psum / termcredit[termnum - 1];
                    calitem = 0;
                    csum = asum = psum = 0;

                    for (int i = 0; i < termnum; i++)
                    {
                        ListViewItem li = new ListViewItem("-");
                        li.SubItems.Add(finalterm[i] + " 学期平均");
                        li.SubItems.Add(termachievement[i].ToString()); //成绩
                        li.SubItems.Add("(总)" + termcredit[i].ToString()); //学分
                        li.SubItems.Add(termpoint[i].ToString());  //绩点
                        listView1.Items.Add(li);
                    }
                    for (int i = 0; i < itemnum; i++)
                    {
                        ListViewItem li = new ListViewItem(sptxt[0 + i * 10]);
                        li.SubItems.Add(sptxt[3 + i * 10]);
                        li.SubItems.Add(sptxt[4 + i * 10]); //成绩
                        li.SubItems.Add(sptxt[5 + i * 10]); //学分
                        li.SubItems.Add(getpoint(Convert.ToInt32(sptxt[4 + i * 10]), Convert.ToSingle(sptxt[5 + i * 10])).ToString());
                        listView1.Items.Add(li);
                        asum += Convert.ToSingle(sptxt[4 + i * 10]);
                        csum += Convert.ToSingle(sptxt[5 + i * 10]);
                        psum += getpoint(Convert.ToInt32(sptxt[4 + i * 10]), Convert.ToSingle(sptxt[5 + i * 10]));
                    }
                    asum = asum / itemnum;
                    psum = psum / csum;
                    label1.Text = "总学分：" + csum.ToString() + "    平均成绩：" + asum.ToString() + "    平均学分绩点：" + psum.ToString(); ;
                }
                //chrome
                else
                {
                    string[] subjectlist = new string[10];
                    int itemnum = (sptxt.Length - 2);
                    int termnum = 1, ie = 0;

                    if (sptxt[0] == "") ie = 1;

                    string[] term = new string[itemnum];
                    if (ie == 1)
                    {
                        subjectlist = sptxt[1].Split(new char[] { ' ', '\t' });
                        term[0] = subjectlist[1];
                    }
                    for (int i = ie; i < itemnum; i++)
                    {
                        subjectlist = sptxt[i].Split(new char[] { ' ', '\t' });
                        term[i] = subjectlist[1];
                    }

                    string s = term[0];
                    for (int i = ie; i < term.Length; i++)
                    {
                        if (!s.Equals(term[i]))
                        {
                            termnum++;
                            s = term[i];
                        }
                    }

                    string[] finalterm = new string[termnum];
                    float[] termachievement = new float[termnum];
                    float[] termcredit = new float[termnum];
                    float[] termpoint = new float[termnum];
                    int calitem = 0;
                    float psum = 0, asum = 0, csum = 0;
                    //finalterm[0] = term[0];
                    //s = term[0];
                    termnum = 0;

                    /*
                    if (ie == 1)
                    {
                        calitem++;
                        sptxt[1] = sptxt[1].Replace("优秀", "95");
                        sptxt[1] = sptxt[1].Replace("良好", "85");
                        sptxt[1] = sptxt[1].Replace("中等", "75");
                        sptxt[1] = sptxt[1].Replace("及格", "60");
                        sptxt[1] = sptxt[1].Replace("不及格", "0");
                        subjectlist = sptxt[].Split(new char[] { ' ', '\t' });
                        asum += Convert.ToSingle(subjectlist[4]);
                        csum += Convert.ToSingle(subjectlist[5]);
                        psum += getpoint(Convert.ToInt32(subjectlist[4]), Convert.ToSingle(subjectlist[5]));
                    }*/
                    for (int i = ie; i < term.Length; i++)
                    {
                        if (!s.Equals(term[i]))
                        {
                            finalterm[termnum] = term[i];
                            s = term[i];
                            if (calitem != 0)
                            {
                                termachievement[termnum - 1] = asum / calitem;
                                termcredit[termnum - 1] = csum;
                                termpoint[termnum - 1] = psum / termcredit[termnum - 1];
                                calitem = 0;
                                csum = asum = psum = 0;
                            }
                            termnum++;
                        }
                        calitem++;
                        sptxt[i] = sptxt[i].Replace("优秀", "95");
                        sptxt[i] = sptxt[i].Replace("良好", "85");
                        sptxt[i] = sptxt[i].Replace("中等", "75");
                        sptxt[i] = sptxt[i].Replace("及格", "60");
                        sptxt[i] = sptxt[i].Replace("不及格", "0");
                        subjectlist = sptxt[i].Split(new char[] { ' ', '\t' });
                        asum += Convert.ToSingle(subjectlist[4]);
                        csum += Convert.ToSingle(subjectlist[5]);
                        psum += getpoint(Convert.ToInt32(subjectlist[4]), Convert.ToSingle(subjectlist[5]));
                    }
                    if (termnum == 0) termnum++;
                    termachievement[termnum - 1] = asum / calitem;
                    termcredit[termnum - 1] = csum;
                    termpoint[termnum - 1] = psum / termcredit[termnum - 1];
                    calitem = 0;
                    csum = asum = psum = 0;

                    for (int i = 0; i < termnum; i++)
                    {
                        ListViewItem li = new ListViewItem("-");
                        li.SubItems.Add(finalterm[i] + " 学期平均");
                        li.SubItems.Add(termachievement[i].ToString()); //成绩
                        li.SubItems.Add("(总)" + termcredit[i].ToString()); //学分
                        li.SubItems.Add(termpoint[i].ToString());  //绩点
                        listView1.Items.Add(li);
                    }
                    for (int i = ie; i < itemnum; i++)
                    {
                        subjectlist = sptxt[i].Split(new char[] { ' ', '\t' });
                        ListViewItem li = new ListViewItem(subjectlist[0]);
                        li.SubItems.Add(subjectlist[3]);
                        li.SubItems.Add(subjectlist[4]); //成绩
                        li.SubItems.Add(subjectlist[5]); //学分
                        li.SubItems.Add(getpoint(Convert.ToInt32(subjectlist[4]), Convert.ToSingle(subjectlist[5])).ToString());
                        listView1.Items.Add(li);
                        asum += Convert.ToSingle(subjectlist[4]);
                        csum += Convert.ToSingle(subjectlist[5]);
                        psum += getpoint(Convert.ToInt32(subjectlist[4]), Convert.ToSingle(subjectlist[5]));
                    }
                    asum = asum / (itemnum - ie);
                    psum = psum / csum;
                    label1.Text = "总学分：" + csum.ToString() + "    平均成绩：" + asum.ToString() + "    平均学分绩点：" + psum.ToString(); ;
                }
            }
            catch
            {
                MessageBox.Show("计算出现错误，请：\n● 使用Internet Explorer 或 Edge 或 Chrome 浏览器进行复制。\n● 等待网页完全加载完毕后全选复制完整内容" , "错误");
            }
            
        }

        private float getpoint(int achievement, float credit)
        {
            float fachievement = achievement;
            return (fachievement - 50) / 10 * credit;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                System.Net.WebClient getversion = new System.Net.WebClient();
                getversion.DownloadStringAsync(new Uri(@"http://ymefg.cn/jidian/version.txt"));
                getversion.DownloadStringCompleted += Getversion_DownloadStringCompleted;

                System.Net.WebClient getad = new System.Net.WebClient();
                getad.DownloadStringAsync(new Uri(@"http://ymefg.cn/jidian/ad.txt"));
                getad.DownloadStringCompleted += Getad_DownloadStringCompleted;
            }
            catch
            {

            }
        }

        private void Getad_DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                ads = e.Result.Split('\n');
                for (int i = 0; i < ads.Length; i++)
                {
                    if (ads[i] != null) adnum++;
                }
                ad = true;
                timer1.Enabled = true;
            }
            catch
            {
                ad = false;
                timer1.Enabled = false;
            }
        }

        private void Getversion_DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            try
            {
                //throw new NotImplementedException();
                if (Properties.Settings.Default.version < Convert.ToInt32(e.Result))
                {
                    button2.Text = "有新版本";
                }
            }
            catch
            {

            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(link);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (shunxu >= adnum) shunxu = 0;
            string[] wenzi = ads[shunxu].Split('^');
            linkLabel1.Text = wenzi[0];
            link = wenzi[1];
            shunxu++;
        }
    }
}
