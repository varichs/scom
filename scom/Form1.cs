using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Runtime.InteropServices;
namespace scom
{
    public partial class Form1 : Form
    {
        //参数存储功能
        [DllImport("kernel32")]
        //系统dll导入ini写函数
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
       [DllImport("kernel32")]
        //系统dll导入ini读函数
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        //ini文件名
        string FileName = System.AppDomain.CurrentDomain.BaseDirectory + "scom_data.ini";
        //存储读出ini内容的变量
        StringBuilder temp = new StringBuilder(255);
        //StringBuilder CycleTemp = new StringBuilder(255);
       
        //所需记忆的变量
        string CurrentPortName;
        string CurrentCycle;
        string CurrentBaudRate;
        string CurrentDataBits;
        string CurrentStopBits;
        string CurrentParity;
        string[] CMDData = new string[10];
       
        //循环命令发送所需的按钮及输入框
        Button[] My_Button = new Button[10];
        TextBox[] My_TextBox = new TextBox[10];


        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            serialPort1.Encoding = Encoding.GetEncoding("GB2312");
            SearchAnAddAerialToComboBox(serialPort1, comboBox1);
            //窗口关闭时的记忆时间
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form1_FormClosing);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //throw new NotImplementedException();
            
            //窗口关闭时需要保存的数据
            WritePrivateProfileString("PortData", "PortName", CurrentPortName, FileName);
            WritePrivateProfileString("CycleData", "Interval", CurrentCycle, FileName);
            WritePrivateProfileString("BaudRateData", "BaudRate", CurrentBaudRate, FileName);
            WritePrivateProfileString("DataBitsData", "DataBits", CurrentDataBits, FileName);
            WritePrivateProfileString("StopBitsData", "StopBits", CurrentStopBits, FileName);
            WritePrivateProfileString("ParityData", "Parity", CurrentParity, FileName);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;      //选择端口号
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text, 10);     //将波特率转换成十进制
                serialPort1.DataBits = Convert.ToInt32(comboBox3.Text.Trim(), 10);     //数据位选择

                float   f = Convert.ToSingle (comboBox4.Text.Trim());     //停止位选择
                if (f == 1)
                {
                    serialPort1.StopBits = StopBits.One;
                }
                
                else if (f == 1.5)
                {
                    serialPort1.StopBits = StopBits.OnePointFive;
                }
                else if (f == 2)
                {
                    serialPort1.StopBits = StopBits.Two;
                }
                else
                {
                    serialPort1.StopBits = StopBits.One;
                }

                string s = comboBox5.Text.Trim();       //奇偶校验选择
                if (s.CompareTo("无") == 0)
                {
                    serialPort1.Parity = Parity.None;
                }
                else if (s.CompareTo("奇校验") == 0)
                {
                    serialPort1.Parity = Parity.Odd;
                }
                else if (s.CompareTo("偶校验") == 0)
                {
                    serialPort1.Parity = Parity.Even;
                }
                else
                {
                    serialPort1.Parity = Parity.None;
                }

                serialPort1.Open();
                CurrentPortName = comboBox1.Text;
                CurrentBaudRate = comboBox2.Text;
                CurrentDataBits = comboBox3.Text;
                CurrentStopBits = comboBox4.Text;
                CurrentParity = comboBox5.Text;
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;
                button2.ForeColor = Color.Lime;
                button1.Enabled = false;        //成功打开串口后，“打开串口”按钮不可用
                button2.Enabled = true;         //“关闭端口按钮可用”
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
            }
            catch
            {
                MessageBox.Show("打开串口错误","错误提示");
                button1.ForeColor = Color.Red;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.MaximizeBox = false;
            for (int i=1;i<20;i++)
            {
                comboBox1.Items.Add("COM"+i.ToString());
            }



            //列出常用波特率
            comboBox2.Items.Add("1200");
            comboBox2.Items.Add("2400");
            comboBox2.Items.Add("4800");
            comboBox2.Items.Add("9600");
            comboBox2.Items.Add("19200");
            comboBox2.Items.Add("38400");
            comboBox2.Items.Add("43000");
            comboBox2.Items.Add("56000");
            comboBox2.Items.Add("57600");
            comboBox2.Items.Add("115200");
            comboBox2.SelectedIndex = 9;


            //comboBox2.Text = "115200";      //默认波特率115200

            //数据位选项
            comboBox3.Items.Add("8");
            comboBox3.Items.Add("7");
            comboBox3.Items.Add("6");
            comboBox3.Items.Add("5");
            //comboBox3.SelectedIndex = 0;

            //列出停止位
            
            comboBox4.Items.Add("1");
            comboBox4.Items.Add("1.5");
            comboBox4.Items.Add("2");
            //comboBox4.SelectedIndex = 0;

            //列出奇偶校验位
            comboBox5.Items.Add("无");
            comboBox5.Items.Add("奇校验");
            comboBox5.Items.Add("偶校验");
            //comboBox5.SelectedIndex = 0;

            Button[] My_Button = new Button[10];
            TextBox[] My_TextBox = new TextBox[10];
            
            My_Button[0] = button7;
            My_Button[1] = button8;
            My_Button[2] = button9;
            My_Button[3] = button10;
            My_Button[4] = button11;
            My_Button[5] = button12;
            My_Button[6] = button13;
            My_Button[7] = button14;
            My_Button[8] = button15;
            My_Button[9] = button16;

            //注册按钮点击的触发事件
            for(int i=0;i<10;i++)
            {
                My_Button[i].Click += new EventHandler(My_Button_Click);
            }

            My_TextBox[0] = textBox4;
            My_TextBox[1] = textBox5;
            My_TextBox[2] = textBox6;
            My_TextBox[3] = textBox7;
            My_TextBox[4] = textBox8;
            My_TextBox[5] = textBox9;
            My_TextBox[6] = textBox10;
            My_TextBox[7] = textBox11;
            My_TextBox[8] = textBox12;
            My_TextBox[9] = textBox13;
            My_TextBox[0].Text = textBox4.Text;
            
            //配置初始化
            GetPrivateProfileString("CycleData", "Interval", "1000", temp, 256, FileName);
            textBox3.Text = temp.ToString();
            GetPrivateProfileString("PortData", "PortName", "COM1", temp, 256, FileName);//读取ini值，默认是COM1
            comboBox1.Text = temp.ToString();//初始化
            GetPrivateProfileString("BaudRateData", "BaudRate", "115200", temp, 256, FileName);
            comboBox2.Text = temp.ToString();
            GetPrivateProfileString("DataBitsData", "DataBits", "8", temp, 256, FileName);
            comboBox3.Text = temp.ToString();
            GetPrivateProfileString("StopBitsData", "StopBits", "1", temp, 256, FileName);
            comboBox4.Text = temp.ToString();
            GetPrivateProfileString("ParityData", "Parity", "无", temp, 256, FileName);
            comboBox5.Text = temp.ToString();
            //命令记忆部分
            GetPrivateProfileString("CMDData", "CMDData[0]", "", temp, 256, FileName);
            textBox4.Text = temp.ToString();
            GetPrivateProfileString("CMDData", "CMDData[1]", "", temp, 256, FileName);
            textBox5.Text = temp.ToString();
            GetPrivateProfileString("CMDData", "CMDData[2]", "", temp, 256, FileName);
            textBox6.Text = temp.ToString();
            GetPrivateProfileString("CMDData", "CMDData[3]", "", temp, 256, FileName);
            textBox7.Text = temp.ToString();
            GetPrivateProfileString("CMDData", "CMDData[4]", "", temp, 256, FileName);
            textBox8.Text = temp.ToString();
            GetPrivateProfileString("CMDData", "CMDData[5]", "", temp, 256, FileName);
            textBox9.Text = temp.ToString();
            GetPrivateProfileString("CMDData", "CMDData[6]", "", temp, 256, FileName);
            textBox10.Text = temp.ToString();
            GetPrivateProfileString("CMDData", "CMDData[7]", "", temp, 256, FileName);
            textBox11.Text = temp.ToString();
            GetPrivateProfileString("CMDData", "CMDData[8]", "", temp, 256, FileName);
            textBox12.Text = temp.ToString();
            GetPrivateProfileString("CMDData", "CMDData[9]", "", temp, 256, FileName);
            textBox13.Text = temp.ToString();


            /*------------必须手动添加的接收事件-------------*/
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }

        private void port_DataReceived(object sender,SerialDataReceivedEventArgs e)
        {
            //如果接收模式为字符模式
            if(!radioButton3.Checked)
            {
                //字符串方式读
                string str = serialPort1.ReadExisting();
                //接收框添加内容
                textBox2.AppendText(str);
            }
            else
            {
                //读串口接收数据缓存区
                Byte[] data = new byte[serialPort1.BytesToRead];
                serialPort1.Read(data, 0, data.Length);
                foreach(byte member in data )
                {
                    string str = Convert.ToString(member, 16).ToUpper();
                    textBox2.AppendText("0x" + (str.Length == 1 ? "0" + str : str) + "-");

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //关闭串口
                checkBox1.Checked = false;
                serialPort1.Close();
                button1.ForeColor = Color.Red;
                button1.Enabled = true;
                button2.Enabled = false;
                comboBox1.Enabled = true ;
                comboBox2.Enabled = true ;
                comboBox3.Enabled = true ;
                comboBox4.Enabled = true ;
                comboBox5.Enabled = true ;
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;


            }
            catch
            {
                MessageBox.Show("关闭串口错误", "错误提示");
                button2.ForeColor = Color.Lime;
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[1];
            //判断串口是否打开，如果打开则继续执行
            if(serialPort1.IsOpen)
            {
                if(textBox1.Text !="")
                {
                    if(!radioButton1.Checked )
                    {
                        try
                        {
                            serialPort1.WriteLine(textBox1.Text);
                        }
                        catch
                        {
                            MessageBox.Show("串口写入数据错误", "错误提示");
                            serialPort1.Close();
                            button1.Enabled = true;
                            button2.Enabled = false;
                        }
                    }
                    else
                    {
                        try
                        {
                            for (int i = 0; i < (textBox1.Text.Length - textBox1.Text.Length % 2) / 2; i++)
                            {
                                data[0] = Convert.ToByte(textBox1.Text.Substring(i * 2, 2), 16);
                                serialPort1.Write(data, 0, 1);//循环发送（如果输入字符为0A0BB,则只发送0A,0B）
                            }
                            if (textBox1.Text.Length % 2 != 0)
                            {
                                data[0] = Convert.ToByte(textBox1.Text.Substring(textBox1.Text.Length - 1, 1), 16);//单独发送B（0B）
                                serialPort1.Write(data, 0, 1);//发送
                            }
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确格式的数据或选择正确的发送接收方式", "错误提示！");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请输入要发送的数据", "错误提示");
                }

            }
            else
            {
                MessageBox.Show("请打开串口", "错误提示");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            textBox2.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SearchAnAddAerialToComboBox(serialPort1, comboBox1);
            if (comboBox1.Items.Count==0)
            {
                MessageBox.Show("无可用串口", "错误提示");
            }
            else
            {
                comboBox1.SelectedIndex = 0;
            }
            
        }
        private void SearchAnAddAerialToComboBox(SerialPort MyPort,ComboBox MyBox)
        {
            //将可用端口号添加到ComboBox
            //string[] MyString = new string[20];                         //最多容纳20个，太多会影响调试效率
            string Buffer;                                              //缓存
            MyBox.Items.Clear();                                        //清空ComboBox内容
           
            for (int i = 1; i < 20; i++)                                //循环
            {
                try                                                     //核心原理是依靠try和catch完成遍历
                {
                    Buffer = "COM" + i.ToString();
                    MyPort.PortName = Buffer;
                    MyPort.Open();                                      //如果失败，后面的代码不会执行
                                                                        // MyString[count] = Buffer;
                    MyBox.Items.Add(Buffer);                            //打开成功，添加至下俩列表
                    MyPort.Close();                                     //关闭
                    
                }
                catch
                {
                   
                }
            }
        }

        private void  send_data(TextBox TextBox_Num)
        {
            byte[] data = new byte[1];
            //判断串口是否打开，如果打开则继续执行
            if (serialPort1.IsOpen)
            {
                if (TextBox_Num.Text != "")
                {
                    if (!radioButton1.Checked)
                    {
                        try
                        {
                            serialPort1.WriteLine(TextBox_Num.Text);
                        }
                        catch
                        {
                            MessageBox.Show("串口写入数据错误", "错误提示");
                            serialPort1.Close();
                            button1.Enabled = true;
                            button2.Enabled = false;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < (TextBox_Num.Text.Length - TextBox_Num.Text.Length % 2) / 2; i++)
                        {
                            data[0] = Convert.ToByte(TextBox_Num.Text.Substring(i * 2, 2), 16);
                            serialPort1.Write(data, 0, 1);//循环发送（如果输入字符为0A0BB,则只发送0A,0B）
                        }
                        if (TextBox_Num.Text.Length % 2 != 0)
                        {
                            data[0] = Convert.ToByte(TextBox_Num.Text.Substring(TextBox_Num.Text.Length - 1, 1), 16);//单独发送B（0B）
                            serialPort1.Write(data, 0, 1);//发送
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请输入要发送的数据", "错误提示");
                }

            }
            else
            {
                MessageBox.Show("请打开串口", "错误提示");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
          send_data(textBox1);            //发送主输入框的数据
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)  //检查发送框是否被选中
            {
                if (serialPort1.IsOpen==true)   //检查串口是否打开
                {
                   if(textBox1.Text != "")  //检测主输入框是否有待发送的数据
                    {
                        try
                        {
                            timer1.Interval = Convert.ToInt32(textBox3.Text.Trim());//定时器赋初值单位毫秒
                            CurrentCycle = textBox3.Text;
                            timer1.Start();
                            textBox3.Enabled = false;
                        }
                        catch
                        {
                            checkBox1.Checked = false;
                            MessageBox.Show("请输入计时周期", "错误提示");
                        }

                    }

                    else/* if (checkBox1.Checked == false)*/
                    {
                        MessageBox.Show("请输入数据", "错误提示");
                        checkBox1.Checked = false;
                    }
                    return;
                }
                else
                {
                    MessageBox.Show("请打开串口", "错误提示");
                    checkBox1.Checked = false;
                    return;
                }
            }
            else
            {
                timer1.Stop();
                textBox3.Enabled = true;                               
            }
        }
        //循环发送函数，循环命令按钮的事件响应函数
        protected  void My_Button_Click(object sender, EventArgs e)
        {
            Button My_Button = (Button)sender;
            int s = Convert.ToInt32(My_Button.Text);
            //CMDData[s] = My_TextBox[s].Text;
            //MessageBox.Show("事件响应"+s+My_Button, "系统提示！");
            switch (s)
            {
                default:MessageBox.Show("未知错误", "系统错误!"); break;
                case 1:send_data(textBox4);     break;
                case 2:send_data(textBox5);     break;
                case 3: send_data(textBox6);    break;
                case 4: send_data(textBox7);    break;
                case 5: send_data(textBox8);    break;
                case 6: send_data(textBox9);    break;
                case 7: send_data(textBox10);   break;
                case 8: send_data(textBox11);   break;
                case 9: send_data(textBox12);   break;
                case 10: send_data(textBox13);  break;
            }

        }
        //命令记忆按钮
        private void button17_Click(object sender, EventArgs e)
        {

            CMDData[0] = textBox4.Text;
            CMDData[1] = textBox5.Text;
            CMDData[2] = textBox6.Text;
            CMDData[3] = textBox7.Text;
            CMDData[4] = textBox8.Text;
            CMDData[5] = textBox9.Text;
            CMDData[6] = textBox10.Text;
            CMDData[7] = textBox11.Text;
            CMDData[8] = textBox12.Text;
            CMDData[9] = textBox13.Text;

           
          //保存命令数据
           WritePrivateProfileString("CMDData", "CMDData[0]", CMDData[0], FileName);
           WritePrivateProfileString("CMDData", "CMDData[1]", CMDData[1], FileName);
           WritePrivateProfileString("CMDData", "CMDData[2]", CMDData[2], FileName);
           WritePrivateProfileString("CMDData", "CMDData[3]", CMDData[3], FileName);
           WritePrivateProfileString("CMDData", "CMDData[4]", CMDData[4], FileName);
           WritePrivateProfileString("CMDData", "CMDData[5]", CMDData[5], FileName);
           WritePrivateProfileString("CMDData", "CMDData[6]", CMDData[6], FileName);
           WritePrivateProfileString("CMDData", "CMDData[7]", CMDData[7], FileName);
           WritePrivateProfileString("CMDData", "CMDData[8]", CMDData[8], FileName);
           WritePrivateProfileString("CMDData", "CMDData[9]", CMDData[9], FileName);
    
        }
        //取消记忆按钮
        private void button18_Click(object sender, EventArgs e)
        {
            //取消记忆命令
            CMDData[0] = "";
            CMDData[1] = "";
            CMDData[2] = "";
            CMDData[3] = "";
            CMDData[4] = "";
            CMDData[5] = "";
            CMDData[6] = "";
            CMDData[7] = "";
            CMDData[8] = "";
            CMDData[9] = "";
            //保存命令数据
            WritePrivateProfileString("CMDData", "CMDData[0]", CMDData[0], FileName);
            WritePrivateProfileString("CMDData", "CMDData[1]", CMDData[1], FileName);
            WritePrivateProfileString("CMDData", "CMDData[2]", CMDData[2], FileName);
            WritePrivateProfileString("CMDData", "CMDData[3]", CMDData[3], FileName);
            WritePrivateProfileString("CMDData", "CMDData[4]", CMDData[4], FileName);
            WritePrivateProfileString("CMDData", "CMDData[5]", CMDData[5], FileName);
            WritePrivateProfileString("CMDData", "CMDData[6]", CMDData[6], FileName);
            WritePrivateProfileString("CMDData", "CMDData[7]", CMDData[7], FileName);
            WritePrivateProfileString("CMDData", "CMDData[8]", CMDData[8], FileName);
            WritePrivateProfileString("CMDData", "CMDData[9]", CMDData[9], FileName);
        }
    }
}
