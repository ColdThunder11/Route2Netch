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
using System.Net;

namespace Route2Netch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty) return;
            if (!IsFileNameValid(textBox1.Text))
            {
                MessageBox.Show("游戏名存在不合法文件名，请修改");
                return;
            }
            var gameName = textBox1.Text;
            label2.Text = "正在抓取路由表，可能需要较长时间";
            CatchRoute();
            label2.Text = "正在创建模式文件";
            var rule= Route2Rule(Path.Combine(Environment.CurrentDirectory, "route.txt"));
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "mode")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "mode"));
            string firstLine = "# " + gameName + " (TUN/TAP)" + ", 1, 0\r\n";
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "mode", gameName + " (TUN-TAP).txt"),
                firstLine + rule);
            File.Delete(Path.Combine(Environment.CurrentDirectory, "route.txt"));
            label2.Text = "抓取完成";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty) return;
            if (!IsFileNameValid(textBox1.Text))
            {
                MessageBox.Show("游戏名存在不合法文件名，请修改");
                return;
            }
            var gameName = textBox1.Text;
            label2.Text = "正在抓取路由表，可能需要较长时间";
            CatchRoute();
            label2.Text = "正在创建模式文件";
            var rule = Route2Rule(Path.Combine(Environment.CurrentDirectory, "route.txt"));
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "rules")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "rules"));
            string firstLine = "#" + gameName + "," + gameName + ",0,0,1,0,1,0,By-Route2Netch" + Environment.NewLine;
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "rules", gameName + ".rules"),
                firstLine + rule);
            File.Delete(Path.Combine(Environment.CurrentDirectory, "route.txt"));
            label2.Text = "抓取完成";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty) return;
            if (!IsFileNameValid(textBox1.Text))
            {
                MessageBox.Show("游戏名存在不合法文件名，请修改");
                return;
            }
            var gameName = textBox1.Text;
            label2.Text = "正在抓取路由表，可能需要较长时间";
            CatchRoute();
            label2.Text = "正在创建模式文件";
            var rule = Route2Rule(Path.Combine(Environment.CurrentDirectory, "route.txt"));
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "mode")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "mode"));
            string firstLine = "# " + gameName + " (TUN/TAP)" + ", 1, 0\r\n";
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "mode", gameName + " (TUN-TAP).txt"),
                firstLine + rule);
            rule = Route2Rule(Path.Combine(Environment.CurrentDirectory, "route.txt"));
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "rules")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "rules"));
            firstLine = "#" + gameName + "," + gameName + ",0,0,1,0,1,0,By-Route2Netch" + Environment.NewLine;
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "rules", gameName + ".rules"),
                firstLine + rule);
            File.Delete(Path.Combine(Environment.CurrentDirectory, "route.txt"));
            label2.Text = "抓取完成";
        }
        private bool IsFileNameValid(string name)
        {
            bool isFilename = true;
            string[] errorStr = new string[] { "/", "\\", ":", ",", "*", "?", "\"", "<", ">", "|" };

            if (string.IsNullOrEmpty(name))
            {
                isFilename = false;
            }
            else
            {
                for (int i = 0; i < errorStr.Length; i++)
                {
                    if (name.Contains(errorStr[i]))
                    {
                        isFilename = false;
                        break;
                    }
                }
            }
            return isFilename;
        }
        void CatchRoute()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序
            p.StandardInput.WriteLine("route print -4 >> " + Path.Combine(Environment.CurrentDirectory, "route.txt"));
            p.StandardInput.AutoFlush = true;
            p.StandardInput.WriteLine("exit");
            p.WaitForExit();
            p.Close();
        }
        string Route2Rule(string routePath)
        {
            var routeLines = File.ReadAllLines(routePath);
            int partFlag = 0;
            bool startFlag = false;
            string metric = string.Empty;
            var sb = new StringBuilder();
            for(int i=0; i < routeLines.Length; i++)
            {
                if (routeLines[i].StartsWith("="))
                {
                    partFlag++;
                    continue;
                }
                if (partFlag == 3 && !startFlag)
                {
                    i += 1;
                    startFlag = true;
                    continue;
                }
                if (startFlag && partFlag == 3)
                {
                    List<string> avaArr = new List<string>();
                    var splitParts = routeLines[i].Split(' ');
                    foreach(var str in splitParts)
                    {
                        if (str != "")
                        {
                            avaArr.Add(str);
                        }
                    }
                    if (avaArr[0].Split('.')[0] == "10") continue;
                    if (avaArr[0].Split('.')[0] == "172" &&
                        (int.Parse(avaArr[0].Split('.')[1]) >= 16 && int.Parse(avaArr[0].Split('.')[1]) < 32))
                        continue;
                    if (avaArr[0].Split('.')[0] == "192"&& avaArr[0].Split('.')[1] == "168") continue;
                    if (avaArr[0] == "0.0.0.0") continue;
                    if (avaArr[0].StartsWith("127.0.0.")) continue;
                    if (avaArr[0].StartsWith("224.0.0.")) continue;
                    if (avaArr[0]=="255.255.255.255") continue;
                    sb.Append(avaArr[0]);
                    int netmask = 0;
                    var nm = avaArr[1].Split('.');
                    for(int k = 0; k < 4; k++)
                    {
                        int t = int.Parse(nm[k]);
                        if (t != 0)
                        {
                            var dexstr = Convert.ToString(t, 2);
                            netmask += dexstr.Length;
                        }
                    }
                    sb.Append("/" + netmask.ToString());sb.Append(Environment.NewLine);
                }
                if (partFlag > 3)
                {
                    break;
                }
            }
            return sb.ToString();
        }

    }
}
