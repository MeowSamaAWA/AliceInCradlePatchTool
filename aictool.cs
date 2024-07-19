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
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Security.Policy;
using Microsoft.VisualBasic;
using Sunny.UI;
using System.IO.Compression;
using System.Runtime.ConstrainedExecution;

namespace AliceInCradle
{
    public partial class aictool : Form
    {
        //定义一个空字符串 dfP
        string dfP = string.Empty;

        bool isInstall = false;


        public aictool()
        {
            InitializeComponent();
            gameVersion();
            downloadServer();
        }

        //游戏版本
        public void gameVersion() 
        {
            comboBox1.Items.Add("0.25f");
            comboBox1.Items.Add("0.24g");
        }
        //获取游戏版本
        public void getGameVer()
        {
            try
            {
                string sFilePath = textBox1.Text + "\\" + "AliceInCradle_Data" + "\\" + "Managed" + "\\" + "Assembly-CSharp.dll";
                FileInfo fileInfo = new FileInfo(sFilePath); //获取源Assembly-CSharp.dll文件大小
                long mSize = fileInfo.Length;
                switch (mSize)
                {
                    case 3278848:
                        MessageBox.Show("已为您自动选择好版本，如有错误，请手动选择。");
                        comboBox1.Text = "0.25f";
                        break;
                    case 2904064:
                        MessageBox.Show("已为您自动选择好版本，如有错误，请手动选择。");
                        comboBox1.Text = "0.24g";
                        break;
                    case 3273216:
                        comboBox1.Text = "0.25f";
                        isInstall = true;
                        break;
                    case 2903552:
                        comboBox1.Text = "0.24g";
                        isInstall = true;
                        break;
                    default:
                        MessageBox.Show("错误，请手动选择");
                        break;
                }
            }
            catch (Exception e)
            {
                {
                    MessageBox.Show(e.Message);
                    selecteGameRoot();
                }
            }

        }
        public void selecteGameRoot()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "选择游戏文件夹";
            if (dfP != string.Empty)
            {
                //判断dfP是否为空，如否，直接赋值到SelectedPath
                fbd.SelectedPath = dfP;
            }
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                dfP = fbd.SelectedPath;
                //MessageBox.Show(dfP); //调试
                textBox1.Text = dfP;
            }
        }
        //选择游戏根目录
        public void button1_Click(object sender, EventArgs e)
        {
            selecteGameRoot();
        }

        public void DownLoadFixes(string url, string filePath)
        {
            string fileName = "Assembly-CSharp.dll"; //定义文件名
            if (textBox1.Text == "")
            {
                MessageBox.Show("笨蛋，还没选择游戏根目录");
            }
            else
            {
                if (isInstall)
                {
                    MessageBox.Show("笨蛋，你已经安装了补丁");
                }
                else
                {
                    //MessageBox.Show(fileInfo.Length.ToString());
                    if (comboBox1.Text == "0.25f") //0.25f版本补丁
                    {
                        WebRequest request = WebRequest.Create(url);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        int code = Convert.ToInt32(response.StatusCode);
                        //MessageBox.Show("StatusCode" + code.ToString());
                        try
                        {
                            if (code == 200) //判断Url可否访问
                            {
                                //是，开始下载, 并判断文件是否存在
                                if (!File.Exists(filePath + "\\" + fileName))
                                {
                                    MessageBox.Show("下载中......");
                                    WebClient client = new WebClient();
                                    Uri uri = new Uri(url);
                                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DFCallBack);
                                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(ProgressCallBack);
                                    client.Credentials = CredentialCache.DefaultCredentials;
                                    client.DownloadFileAsync(uri, filePath + "\\" + fileName);
                                }
                                else
                                {
                                    MessageBox.Show("文件已存在");
                                }
                            }//否
                            else
                            {
                                MessageBox.Show("网络错误");
                            }
                        }
                        catch (Exception a)
                        {
                            MessageBox.Show(a.Message);
                        }
                    }
                    //else if (comboBox1.Text == "0.24g") //0.24g版本补丁, 同上
                    //{

                    //    WebRequest request = WebRequest.Create(url);
                    //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    //    int code = Convert.ToInt32(response.StatusCode);
                    //    //MessageBox.Show("StatusCode" + code.ToString());
                    //    try
                    //    {
                    //        if (code == 200)
                    //        {
                    //            if (!File.Exists(filePath + "\\" + fileName))
                    //            {
                    //                MessageBox.Show("下载中......");
                    //                WebClient client = new WebClient();
                    //                client.Credentials = CredentialCache.DefaultCredentials;
                    //                client.DownloadFile(url, filePath + "\\" + fileName);
                    //                MessageBox.Show("下载完成");
                    //            }
                    //            else
                    //            {
                    //                MessageBox.Show("文件已存在");
                    //            }
                    //        }
                    //        else
                    //        {
                    //            MessageBox.Show("网络错误");
                    //        }
                    //    }
                    //    catch (Exception a)
                    //    {
                    //        MessageBox.Show(a.Message);
                    //    }
                    //}
                }
            }
        }

        //下载服务器
        public void downloadServer()
        {
            comboBox2.Items.Add("154.40.45.38");

            comboBox2.Text = "154.40.45.38";
        }
        //游戏下载
        public async void downloadVer(string url, string ver)
        {
            uiProcessBar1.Value = 0;
            //MessageBox.Show(url); //调试
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "选择下载文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show(fbd.SelectedPath); //调试
                string filePath = fbd.SelectedPath; //将用户的游戏根目录赋值到 filePath
                string fileName = string.Empty; //定义文件名
                
                switch (ver)
                {
                    case "0.25f":
                        fileName = "Win+ver025f.zip";
                        break;
                    case "0.24g":
                        fileName = "Win+ver024g.zip";
                        break;
                }
                WebRequest request = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                int code = Convert.ToInt32(response.StatusCode);
                //MessageBox.Show("StatusCode" + code.ToString());
                try
                {
                    if (code == 200 && fileName != string.Empty) //判断Url可否访问
                    {
                        //是，开始下载, 并判断文件是否存在
                        if (!File.Exists(filePath + "\\" + fileName))
                        {
                            MessageBox.Show("\a 下载中......");
                            WebClient client = new WebClient();
                            Uri uri = new Uri(url);
                            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DFCallBack);
                            client.DownloadFileCompleted += new AsyncCompletedEventHandler(ProgressCallBack);
                            client.Credentials = CredentialCache.DefaultCredentials;
                            client.DownloadFileAsync(uri, filePath + "\\" + fileName);

                            isFi = false;
                            isFinish(filePath, fileName, ver);
                        }
                        else
                        {
                            MessageBox.Show("文件已存在");
                        }
                    }//否
                    else
                    {
                        MessageBox.Show("网络错误");
                    }
                }
                catch (Exception a)
                {

                    MessageBox.Show(a.Message);

                }
            }
        }
        //游戏安装

        public string ifilePath;
        public string ifileName;
        public string iver;
        public void installGame(string filePath, string fileName, string ver)
        {
                ifilePath = filePath;
                ifileName = fileName;
                iver = ver;
                //调试
                //filePath = "D:\\Users\\MeowLoli\\Downloads";
                //fileName = "Win+ver025f.zip";
                
                Thread thread = new Thread(new ThreadStart(openSelectFolder));
                thread.SetApartmentState(ApartmentState.STA); //重点
                thread.Start();
                
                
                


        }

        public void openSelectFolder()
        {
            
            //MessageBox.Show("ok");
            string zipFilePath = string.Empty;
            uiProcessBar1.Value = 0;
            FolderBrowserDialog igP = new FolderBrowserDialog();
            igP.Description = "选择游戏安装文件夹";
            if (igP.ShowDialog() == DialogResult.OK)
            {
                zipFilePath = igP.SelectedPath;
                MessageBox.Show("\a 解压中......");
                try
                {
                    ZipFile.ExtractToDirectory(ifilePath + "\\" + ifileName, zipFilePath);
                    MessageBox.Show("\a 解压完成");
                    switch (iver)
                    {
                        case "0.25f":
                            textBox1.Text = zipFilePath + "\\Win ver025\\AliceInCradle_ver025";
                            break;
                        case "0.24g":
                            textBox1.Text = zipFilePath + "\\Win ver024\\AliceInCradle_ver024";
                            break;
                    }
                    //textBox1.Text = zipFilePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Err");
            }
        }
        //下载补丁文件
        private void setFixesDownloadAdress()
        {
            try
            {
                string ver = comboBox1.Text;
                string fixesUrl = string.Empty;
                if (comboBox2.Text == "154.40.45.38")
                {
                    switch (ver)
                    {
                        case "0.25f":
                            fixesUrl = "http://154.40.45.38:8889/down/nWWw1wqp6wIB.dll";
                            break;
                        case "0.24g":
                            fixesUrl = "http://154.40.45.38:8889/down/exjP6av1weWk.dll";
                            break;
                    }
                }
                DownLoadFixes(fixesUrl, textBox1.Text);
            }
            catch (Exception b)
            {
                MessageBox.Show(b.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            setFixesDownloadAdress();
        }
        //安装补丁
        private void installFixes()
        {
                        if (textBox1.Text == "")
            {
                MessageBox.Show("笨蛋，还没选择游戏根目录");
            }
            else
            {
                if (isInstall)
                {
                    MessageBox.Show("笨蛋，你已经安装了补丁");
                }
                else
                {
                    string filePath = textBox1.Text + "\\" + "Assembly-CSharp.dll";
                    string sFilePath = textBox1.Text + "\\" + "AliceInCradle_Data" + "\\" + "Managed" + "\\" + "Assembly-CSharp.dll";
                    //MessageBox.Show(textBox1.Text + "\\" + "Managed" + "\\" + "Assembly-CSharp.dll");

                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show("文件不存在，正在重新下载......");
                        try
                        {
                            if (comboBox1.Text == "0.25f") //0.25f版本补丁
                            {
                                string url = "http://154.40.45.38:8889/down/nWWw1wqp6wIB.dll";
                                string filePath1 = textBox1.Text; //将用户的游戏根目录赋值到 filePath
                                string fileName = "Assembly-CSharp.dll"; //定义文件名
                                WebRequest request = WebRequest.Create(url);
                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                int code = Convert.ToInt32(response.StatusCode);
                                //MessageBox.Show("StatusCode" + code.ToString());
                                try
                                {
                                    if (code == 200) //判断Url可否访问
                                    {
                                        //是，开始下载, 并判断文件是否存在
                                        if (!File.Exists(filePath1 + "\\" + fileName))
                                        {
                                            MessageBox.Show("下载中......");
                                            WebClient client = new WebClient();
                                            client.Credentials = CredentialCache.DefaultCredentials;
                                            client.DownloadFile(url, filePath1 + "\\" + fileName);
                                            MessageBox.Show("下载完成");
                                        }
                                        else
                                        {
                                            MessageBox.Show("文件已存在");
                                        }
                                    }//否
                                    else
                                    {
                                        MessageBox.Show("网络错误");
                                    }
                                }
                                catch (Exception a)
                                {
                                    MessageBox.Show(a.Message);
                                }
                            }
                            else if (comboBox1.Text == "0.24g") //0.24g版本补丁, 同上
                            {
                                string url = "http://154.40.45.38:8889/down/exjP6av1weWk.dll";
                                string filePath2 = textBox1.Text;
                                string fileName = "Assembly-CSharp.dll";
                                WebRequest request = WebRequest.Create(url);
                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                int code = Convert.ToInt32(response.StatusCode);
                                //MessageBox.Show("StatusCode" + code.ToString());
                                try
                                {
                                    if (code == 200) //判断Url可否访问
                                    {
                                        if (!File.Exists(filePath2 + "\\" + fileName))
                                        {
                                            WebClient client = new WebClient();
                                            client.Credentials = CredentialCache.DefaultCredentials;
                                            client.DownloadFile(url, filePath2 + "\\" + fileName);
                                            MessageBox.Show("下载完成");
                                        }
                                        else
                                        {
                                            MessageBox.Show("文件已存在");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("网络错误");
                                    }
                                }
                                catch (Exception a)
                                {
                                    MessageBox.Show(a.Message);
                                }
                            }
                        }
                        catch (Exception a)
                        {
                            MessageBox.Show(a.Message);
                        }
                    }
                    else
                    {
                        string ver = comboBox1.Text;
                        FileInfo fileInfo = new FileInfo(sFilePath);
                        long ltInfo = fileInfo.Length;
                        switch (ltInfo)
                        {
                            case 3273216:
                                isInstall = true;
                                break;
                            case 2904064:
                                isInstall = true;
                                break;
                        }
                        if (isInstall == false)
                        {
                            try
                            {
                                //检测版本                    
                                switch (ver)
                                {
                                    case "0.25f":
                                        File.Delete(sFilePath);
                                        Thread.Sleep(100);
                                        File.Move(filePath, sFilePath);
                                        MessageBox.Show(ver + "补丁安装成功");
                                        //Application.Exit();
                                        isInstall = true;
                                        break;
                                    case "0.24g":
                                        File.Delete(sFilePath);
                                        Thread.Sleep(100);
                                        File.Move(filePath, sFilePath);
                                        MessageBox.Show(ver + "补丁安装成功");
                                        //Application.Exit();
                                        isInstall = true;
                                        break;
                                    default:
                                        MessageBox.Show("未知版本");
                                        break;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Assembly-CSharp.dll 不存在");
                            }
                        }
                        else
                        {
                            switch (ltInfo)
                            {
                                case 3273216:
                                    MessageBox.Show("您已安装" + ver + "补丁");
                                    break;
                                case 2903552:
                                    MessageBox.Show("您已安装" + ver + "补丁");
                                    break;
                            }
                        }
                    }
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            installFixes();
        }

        private void DFCallBack(object sender, DownloadProgressChangedEventArgs d)
        {
            this.BeginInvoke(new Action(() =>
            {
                uiProcessBar1.Value = d.ProgressPercentage;
            }));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        public bool isFi = false;

        public async void isFinish(string filePath,string fileName, string ver)
        {
            await Task.Run(() => {
                //installGame(filePath, fileName, ver);
                for(; isFi == false; ){}
                installGame(filePath, fileName, ver);
            });
        }
        public void ProgressCallBack(object sender, AsyncCompletedEventArgs a)
        {
            this.BeginInvoke(new Action(() =>
            {          
                MessageBox.Show("下载完成");
                isFi = true;     
            }));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void uiButton3_Click(object sender, EventArgs e)
        {
            string url;
            string ver = comboBox1.Text;
            
            switch (ver) {
                case "0.25f":
                    url = "https://dl.aliceincradle.dev/Win%20ver025f.zip";
                    downloadVer(url, "0.25f");
                    break;
                case "0.24g":
                    url = "https://shigure.shiro.dev/f/ePxiP/Win%20ver024g.zip";
                    downloadVer(url, "0.24g");
                    break;
                default:
                    MessageBox.Show("未知版本");
                    break;
            }          

        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            getGameVer();
        }

    }
}
