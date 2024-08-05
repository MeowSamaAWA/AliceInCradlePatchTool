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
using System.Diagnostics;
using AutoUpdaterDotNET;


namespace AliceInCradle
{
    public partial class aictool : Form
    {
        //定义一个空字符串 dfP
        string dfP = string.Empty;
        bool isInstall = false;
        bool isPathNull = true;

        public aictool()
        {
            InitializeComponent();
            gameVersion();
            downloadServer();
            showTips();
            getUpdate();
        }

        //游戏版本
        public void gameVersion()
        {
            comboBox1.Items.Add("0.25f");
            comboBox1.Items.Add("0.24g");
            comboBox1.Items.Add("0.23e_2");
            comboBox1.Items.Add("0.22q");
            comboBox1.Items.Add("0.21r");
            //comboBox1.Items.Add("0.20s");
        }
        //获取游戏版本
        private void getGameVer()
        {
            //判断textBox1是否为空，是提示为空，否根据Assembly-CSharp.dll文件大小检测游戏版本
            //判断游戏isPathNull 是否为true 同时 comboBox1.Text为空
            if (isPathNull)
            {
                MessageBox.Show("笨蛋，还没选择游戏根目录");
            }
            else
            {
                try
                {
                    string sFilePath = textBox1.Text + "\\" + "AliceInCradle_Data" + "\\" + "Managed" + "\\" + "Assembly-CSharp.dll";
                    //获取源Assembly-CSharp.dll文件大小
                    FileInfo fileInfo = new FileInfo(sFilePath);
                    long mSize = fileInfo.Length;
                    //根据Assembly-CSharp.dll文件大小选择版本
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
                        case 2618368:
                            MessageBox.Show("已为您自动选择好版本，如有错误，请手动选择。");
                            comboBox1.Text = "0.23e_2";
                            break;
                        case 2454016:
                            MessageBox.Show("已为您自动选择好版本，如有错误，请手动选择。");
                            comboBox1.Text = "0.22q";
                            break;
                        case 2125824:
                            MessageBox.Show("已为您自动选择好版本，如有错误，请手动选择。");
                            comboBox1.Text = "0.21r";
                            break;
                        //case 1911808:
                        //    MessageBox.Show("已为您自动选择好版本，如有错误，请手动选择。");
                        //    comboBox1.Text = "0.20s";
                        //    break;
                        case 3273216:
                            comboBox1.Text = "0.25f";
                            isInstall = true;
                            break;
                        case 2903552:
                            comboBox1.Text = "0.24g";
                            isInstall = true;
                            break;
                        case 2613248:
                            comboBox1.Text = "0.23e_2";
                            isInstall = true;
                            break;
                        case 2449408:
                            comboBox1.Text = "0.22q";
                            isInstall = true;
                            break;
                        case 2121216:
                            comboBox1.Text = "0.21r";
                            isInstall = true;
                            break;
                        default:
                            MessageBox.Show("错误，请手动选择");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }
        private void selecteGameRoot()
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
                isPathNull = false;
            }
        }
        //选择游戏根目录
        public void button1_Click(object sender, EventArgs e)
        {
            isInstall = false;
            selecteGameRoot();
        }

        private void DownLoadFixes(string url, string filePath, ref bool isDownloaded)
        {
            string fileName = "Assembly-CSharp.dll"; //定义文件名
            if (textBox1.Text == "")
            {
                MessageBox.Show("笨蛋，还没选择游戏根目录");
            }
            else
            {
                //判断是否安装，为否开始下载补丁
                if (isInstall)
                {
                    MessageBox.Show("笨蛋，你已经安装了补丁");
                    isDownloaded = true;
                    return;
                }
                else
                {
                    //MessageBox.Show(fileInfo.Length.ToString());
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

                                try
                                {
                                    client.DownloadFileAsync(uri, filePath + "\\" + fileName);
                                }
                                catch
                                {
                                    if(File.Exists(filePath + "\\" + fileName))
                                    {
                                        File.Delete(filePath + "\\" + fileName );
                                    }
                                }
                                isDownloaded = true;
                                return;
                            }
                            else
                            {
                                MessageBox.Show("文件已存在");
                                isDownloaded = true;
                                return;
                            }
                        }//否
                        else
                        {
                            MessageBox.Show("网络错误");
                            isDownloaded = false;
                            return;
                        }
                    }
                    catch (Exception a)
                    {
                        MessageBox.Show(a.Message);
                    }
                }
            }
        }

        //下载服务器
        private void downloadServer()
        {
            comboBox2.Items.Add("aic.meow.ink");


            comboBox2.Text = "aic.meow.ink";
        }
        //游戏下载
        private async void downloadVer(string url, string ver)
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
                    case "0.23e_2":
                        fileName = "Win+ver023e_2.zip";
                        break;
                    case "0.22q":
                        fileName = "Win+ver022q.zip";
                        break;
                    case "0.21r":
                        fileName = "Win+ver021r.zip";
                        break;
                        //case "0.20s":
                        //    fileName = "Win+ver020s.zip";
                        //    break;
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
                            MessageBox.Show("下载中......");
                            WebClient client = new WebClient();
                            Uri uri = new Uri(url);
                            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DFCallBack);
                            client.DownloadFileCompleted += new AsyncCompletedEventHandler(gameProgressCallBack);
                            client.Credentials = CredentialCache.DefaultCredentials;
                            try
                            {
                                client.DownloadFileAsync(uri, filePath + "\\" + fileName);
                            }
                            catch
                            {
                                if(File.Exists(filePath + "\\" + fileName))
                                {
                                    File.Delete(filePath + "\\" + fileName);
                                }
                            }

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

        string ifilePath;
        string ifileName;
        string iver;
        string fixesUrl;
        public void installGame(string filePath, string fileName, string ver)
        {
            ifilePath = filePath;
            ifileName = fileName;
            iver = ver;
            //调试
            //filePath = "D:\\Users\\MeowLoli\\Downloads";
            //fileName = "Win+ver025f.zip";

            Thread thread = new Thread(new ThreadStart(openSelectFolder));
            thread.SetApartmentState(ApartmentState.STA);
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
                        case "0.23e_2":
                            textBox1.Text = zipFilePath + "\\Win ver023\\AliceInCradle_ver023";
                            break;
                        case "0.22q":
                            textBox1.Text = zipFilePath + "\\Win ver022\\AliceInCradle_ver022";
                            break;
                        case "0.21r":
                            textBox1.Text = zipFilePath + "\\Win ver021\\AliceInCradle_ver021r";
                            break;
                            //case "0.20s":
                            //    textBox1.Text = zipFilePath + "\\Win ver020\\AliceInCradle_ver020s";
                            //    break;
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
                MessageBox.Show("未选择安装目录");
            }
        }
        //下载补丁文件
        private void setFixesDownloadAdress()
        {
            try
            {
                string ver = comboBox1.Text;
                string Url = string.Empty;
                if (comboBox2.Text == "aic.meow.ink")
                {
                    switch (ver)
                    {
                        case "0.25f":
                            Url = "https://aic.meow.ink/0.25f/Assembly-CSharp.dll";
                            break;
                        case "0.24g":
                            Url = "https://aic.meow.ink/0.24g/Assembly-CSharp.dll";
                            break;
                        case "0.23e_2":
                            Url = "https://aic.meow.ink/0.23e_2/Assembly-CSharp.dll";
                            break;
                        case "0.22q":
                            Url = "https://aic.meow.ink/0.22q/Assembly-CSharp.dll";
                            break;
                        case "0.21r":
                            Url = "https://aic.meow.ink/0.21r/Assembly-CSharp.dll";
                            break;
                            //case "0.20s":
                            //    Url = "http://aic.meow.ink/0.20s/Assembly-CSharp.dll";
                            //    break;
                    }
                }
                fixesUrl = Url;
                bool isDownloaded = false;
                DownLoadFixes(Url, textBox1.Text, ref isDownloaded);
            }
            catch (Exception b)
            {
                MessageBox.Show(b.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {

                if (File.Exists(textBox1.Text + "\\" + "AliceInCradle_Data" + "\\" + "Managed" + "\\" + "Assembly-CSharp.dll"))
                {
                    getGameVer();
                    setFixesDownloadAdress();
                }
                else
                {
                    MessageBox.Show("游戏原Assembly-CSharp.dll不存在,请检查游戏根目录");
                }

            
            //getGameVer();

        }
        //安装补丁


        private void installFixes()
        {
            string filePath = textBox1.Text + "\\" + "Assembly-CSharp.dll";
            string sFilePath = textBox1.Text + "\\" + "AliceInCradle_Data" + "\\" + "Managed" + "\\" + "Assembly-CSharp.dll";

            string ver = comboBox1.Text;
            if (isInstall == false)
            {
                try
                {
                    File.Delete(sFilePath);
                    Thread.Sleep(100);
                    File.Move(filePath, sFilePath);
                    MessageBox.Show(ver + "补丁安装成功");
                    //Application.Exit();
                    isInstall = true;


                    MessageBox.Show("正在打开游戏");
                    runGame();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("安装失败" + ex.Message);
                    isInstall = false;
                }
            }
            else
            {
                MessageBox.Show("您已安装" + ver + "补丁");
                isInstall = true;
            }


            //if (isInstall)
            //{
            //    MessageBox.Show("笨蛋，你已经安装了补丁");
            //    isInstall = true;
            //    isInsd = true;
            //    return;
            //}
            //else
            //{

            //    //MessageBox.Show(textBox1.Text + "\\" + "Managed" + "\\" + "Assembly-CSharp.dll");
            //    if (isInstall == false)
            //    {
            //        if (!File.Exists(filePath))
            //        {
            //            MessageBox.Show("文件不存在，正在重新下载......");
            //            try
            //            {
            //                bool isDownloaded = false;
            //                DownLoadFixes(fixesUrl, filePath, ref isDownloaded);

            //                //判断是否下载好
            //                if (isDownloaded)
            //                {


            //                }
            //            }
            //            catch (Exception a)
            //            {
            //                MessageBox.Show(a.Message);
            //            }
            //        }
            //        else
            //        {

            //        }
            //    }

            //}

        }
        private void button3_Click(object sender, EventArgs e)
        {

                if (File.Exists(textBox1.Text + "\\" + "AliceInCradle_Data" + "\\" + "Managed" + "\\" + "Assembly-CSharp.dll"))
                {
                    getGameVer();
                    if (isInstall)
                    {
                        runGame();
                    }
                    else
                    {
                        installFixes();
                        //MessageBox.Show(isInsd.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("游戏原Assembly-CSharp.dll不存在,请检查游戏根目录");
                }
        }
        private void runGame()
        {
            MessageBox.Show("正在打开游戏......");
            try
            {
                string gameName = "AliceInCradle.exe";
                string gamePath = textBox1.Text + "\\" + gameName;
                Process p = Process.Start(gamePath);
            }
            catch (Exception ex)
            {

                MessageBox.Show("打开失败，错误信息：\n" + ex.Message);
            }

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
        private void showTips()
        {
            tips tips = new tips();
            tips.Show();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        bool isFi = false;
        bool isFinishDownload = false;

        private async void isFinish(string filePath, string fileName, string ver)
        {
            await Task.Run(() =>
            {
                //installGame(filePath, fileName, ver);
                for (; isFi == false;) { }
                installGame(filePath, fileName, ver);
            });
        }


        private void ProgressCallBack(object sender, AsyncCompletedEventArgs a)
        {
            this.BeginInvoke(new Action(() =>
            {
                MessageBox.Show("下载完成");
                isFinishDownload = true;
            }));
        }
        private void gameProgressCallBack(object sender, AsyncCompletedEventArgs a)
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
            string gameUrl;
            string ver = comboBox1.Text;

            switch (ver)
            {
                case "0.25f":
                    gameUrl = "https://dl.aliceincradle.dev/Win%20ver025f.zip";
                    downloadVer(gameUrl, "0.25f");
                    break;
                case "0.24g":
                    gameUrl = "https://shigure.shiro.dev/f/ePxiP/Win%20ver024g.zip";
                    downloadVer(gameUrl, "0.24g");
                    break;
                case "0.23e_2":
                    gameUrl = "https://shigure.shiro.dev/f/rxKCl/Win%20ver023e_2.zip";
                    downloadVer(gameUrl, "0.23e_2");
                    break;
                case "0.22q":
                    gameUrl = "https://shigure.shiro.dev/f/DxQcw/Win%28Full%29%20ver022q.zip";
                    downloadVer(gameUrl, "0.22q");
                    break;
                case "0.21r":
                    gameUrl = "https://shigure.shiro.dev/f/glwfB/Win%20ver021r.zip";
                    downloadVer(gameUrl, "0.21r");
                    break;
                //case "0.20s":
                //    gameUrl = "https://shigure.shiro.dev/f/AGGtv/Win_ver020s.zip";
                //    downloadVer(gameUrl, "0.20s");
                //    break;
                default:
                    MessageBox.Show("未知版本");
                    break;
            }

        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text + "\\" + "AliceInCradle_Data" + "\\" + "Managed" + "\\" + "Assembly-CSharp.dll"))
            {
                getGameVer();
                isInstall = false;
            }
            else
            {
                MessageBox.Show("游戏原Assembly-CSharp.dll不存在,请检查游戏根目录");
            }
            
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            getUpdate();
        }
        private void getUpdate()
        {
            UpdateInfoEventArgs args = new UpdateInfoEventArgs();
            if (args.IsUpdateAvailable)
            {
                AutoUpdater.Start("https://aic.meow.ink/updates/AutoUpdaterStarter.xml");
            }
            else
            {
                MessageBox.Show("未找到新版本");
            }
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            Process.Start("https://cn.aliceincradle.dev/");
        }



        private async void uiButton6_Click(object sender, EventArgs e)
        {
            bool isOpen = false;

            Process[] processes = Process.GetProcessesByName("AliceInCradle");
            foreach (Process myProcess in processes)
            {
                if (myProcess.ProcessName == "AliceInCradle")
                {
                    isOpen = true;
                    isInstall = false;
                    //MessageBox.Show("AliceInCradle已打开");
                    DialogResult dr = MessageBox.Show("AliceInCradle已打开，是否开始安装", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    string processPath = myProcess.MainModule.FileName;
                    string path = Path.GetDirectoryName(processPath);

                    if (dr != DialogResult.OK)
                    {
                        MessageBox.Show("您已取消");
                        break;
                    }
                    else
                    {
                        try
                        {
                            textBox1.Text = path;
                            isPathNull = false;
                            getGameVer();
                            string ver = comboBox1.Text;

                            MessageBox.Show("自动获取版本为：" + ver, "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            DialogResult dr1 = MessageBox.Show("是否开始安装补丁", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            if (dr1 != DialogResult.OK)
                            {
                                break;
                            }
                            else
                            {
                                setFixesDownloadAdress();
                                
                                await Task.Run(() =>
                                {
                                    for (; isFinishDownload == false;)
                                    {}
                                    myProcess.Kill();
                                    installFixes();
                                });
                            }


                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        //MessageBox.Show(path);
                    }
                }
            }
            if (isOpen == false)
            {
                MessageBox.Show("AliceInCradle 未打开");
            }
        }
    }
}
