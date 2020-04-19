
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace IP_CHATv2
{
        public partial class Form1 : Form
        {
            Socket mySocket;
            EndPoint epLocal, epRemote;
            byte[] buffer;
            public Form1()
            {
                InitializeComponent();
            }

            private void Form1_Load(object sender, EventArgs e)
            {
                mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //create socket since it needs to go from tcp to udp
                mySocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //reuse same ip adress for transportation
                txtLocalIP.Text = GetLocalIP();
                txtRemoteIP.Text = GetLocalIP();
                groupBox1.Text = Class1.i;
            }
            private string GetLocalIP()
            {
                IPHostEntry myHost;
                myHost = Dns.GetHostEntry(Dns.GetHostName());//get ip automatically when connected
                foreach (IPAddress ip in myHost.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        return ip.ToString();
                }
                return "192.168.0.1";
            }
            private void Button1_Click(object sender, EventArgs e)//code of the connect button
            {
                try
                {
                    if (txtLocalPort.Text != "" && txtRemotePort.Text != "")
                    {

                        epLocal = new IPEndPoint(IPAddress.Parse(txtLocalIP.Text), Convert.ToInt32(txtLocalPort.Text));
                        mySocket.Bind(epLocal);//connection to our ip address
                        epRemote = new IPEndPoint(IPAddress.Parse(txtRemoteIP.Text), Convert.ToInt32(txtRemotePort.Text));
                        mySocket.Connect(epRemote);//listen to remote ip address
                        buffer = new byte[1500];
                        mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
                        button1.Text = "Connected";//changing the button
                        button1.Enabled = false;
                        btnSend.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Some Fields Are missing before connecting");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            private void BtnSend_Click(object sender, EventArgs e)
        { 
                try
                {
                    if (key.Text != "")
                    {
                    //recieved message is sent for encrytion
                        string x = encrypt(groupBox1.Text.ToString() + ":  " + txtMessage.Text, key.Text.ToString());
                        int l = System.Text.ASCIIEncoding.Unicode.GetByteCount(x);
                        Class1.j = l;
                        byte[] SendingMessage = new byte[l];
                        ASCIIEncoding aEncoding = new ASCIIEncoding();
                        SendingMessage = aEncoding.GetBytes(x);
                        mySocket.Send(SendingMessage);
                        ListMessages.Items.Add(groupBox1.Text.ToString() + ": " + txtMessage.Text);//display the message
                        txtMessage.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("Key Required....");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            public string encrypt(string x, string keyai)
            {
                try
                {

                    string y = x;
                    byte[] etext = UTF8Encoding.UTF8.GetBytes(y);
                    string key = keyai;
                    MD5CryptoServiceProvider mdhash = new MD5CryptoServiceProvider();
                    byte[] keyarray = mdhash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    TripleDESCryptoServiceProvider tds = new TripleDESCryptoServiceProvider();
                    tds.Key = keyarray;
                    tds.Mode = CipherMode.ECB;
                    tds.Padding = PaddingMode.PKCS7;
                    ICryptoTransform itransform = tds.CreateEncryptor();
                    byte[] result = itransform.TransformFinalBlock(etext, 0, etext.Length);
                    string encryptresult = Convert.ToBase64String(result);
                    return encryptresult.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message + "Possible Reason,Key/Pin problem";
                }
            }
            public string dencrypt(string x, string keyai)
            {
                try
                {
                    string y = x.Replace("\0", null);
                    byte[] etext = Convert.FromBase64String(y);
                    string key = keyai;
                    MD5CryptoServiceProvider mdhash = new MD5CryptoServiceProvider();
                    byte[] keyarray = mdhash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    TripleDESCryptoServiceProvider tds = new TripleDESCryptoServiceProvider();
                    tds.Key = keyarray;
                    tds.Mode = CipherMode.ECB;
                    tds.Padding = PaddingMode.PKCS7;
                    ICryptoTransform itransform = tds.CreateDecryptor();
                    byte[] result = itransform.TransformFinalBlock(etext, 0, etext.Length);
                    string dencryptresult = UTF8Encoding.UTF8.GetString(result);
                    return dencryptresult.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message + "Possible Reason,Key problem";
                }
            }
            private void exitToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Application.Exit();
            }
            private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Application.Restart();

            }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void MessageCallBack(IAsyncResult aResult)//recieving function for messages works in back
            {
                try
                {
                    byte[] RecivedData = new byte[Class1.j];
                    RecivedData = (byte[])aResult.AsyncState;
                    //converting byte[] into string
                    ASCIIEncoding aEncoding = new ASCIIEncoding();
                    string RecivedMessage = aEncoding.GetString(RecivedData);
                    string decmessage = dencrypt(RecivedMessage, key.Text.ToString());
                    //Adding this message to listbox
                    ListMessages.Items.Add(decmessage);
                    buffer = new byte[1500];
                    mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }


