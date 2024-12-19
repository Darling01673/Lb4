using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        Dictionary<char, byte> iso8859_51 = new Dictionary<char, byte>
        {
            {'А', 0xB0}, {'Б', 0xB1}, {'В', 0xB2}, {'Г', 0xB3}, {'Д', 0xB4},
            {'Е', 0xB5}, {'Ж', 0xB6}, {'З', 0xB7}, {'И', 0xB8}, {'Й', 0xB9},
            {'К', 0xBA}, {'Л', 0xBB}, {'М', 0xBC}, {'Н', 0xBD}, {'О', 0xBE},
            {'П', 0xBF}, {'Р', 0xC0}, {'С', 0xC1}, {'Т', 0xC2}, {'У', 0xC3},
            {'Ф', 0xC4}, {'Х', 0xC5}, {'Ц', 0xC6}, {'Ч', 0xC7}, {'Ш', 0xC8},
            {'Щ', 0xC9}, {'Ъ', 0xCA}, {'Ы', 0xCB}, {'Ь', 0xCC}, {'Э', 0xCD},
            {'Ю', 0xCE}, {'Я', 0xCF}, {'а', 0xD0}, {'б', 0xD1}, {'в', 0xD2},
            {'г', 0xD3}, {'д', 0xD4}, {'е', 0xD5}, {'ж', 0xD6}, {'з', 0xD7},
            {'и', 0xD8}, {'й', 0xD9}, {'к', 0xDA}, {'л', 0xDB}, {'м', 0xDC},
            {'н', 0xDD}, {'о', 0xDE}, {'п', 0xDF}, {'р', 0xE0}, {'с', 0xE1},
            {'т', 0xE2}, {'у', 0xE3}, {'ф', 0xE4}, {'х', 0xE5}, {'ц', 0xE6},
            {'ч', 0xE7}, {'ш', 0xE8}, {'щ', 0xE9}, {'ъ', 0xEA}, {'ы', 0xEB},
            {'ь', 0xEC}, {'э', 0xED}, {'ю', 0xEE}, {'я', 0xEF}, {' ', 0xFF}
        };
        Dictionary<string, char> iso8859_52 = new Dictionary<string, char>
        {
                { "0xB0", 'А' }, { "0xB1", 'Б' }, { "0xB2", 'В' }, { "0xB3", 'Г' }, { "0xB4", 'Д' },
                { "0xB5", 'Е' }, { "0xB6", 'Ж' }, { "0xB7", 'З' }, { "0xB8", 'И' }, { "0xB9", 'Й' },
                { "0xBA", 'К' }, { "0xBB", 'Л' }, { "0xBC", 'М' }, { "0xBD", 'Н' }, { "0xBE", 'О' },
                { "0xBF", 'П' }, { "0xC0", 'Р' }, { "0xC1", 'С' }, { "0xC2", 'Т' }, { "0xC3", 'У' },
                { "0xC4", 'Ф' }, { "0xC5", 'Х' }, { "0xC6", 'Ц' }, { "0xC7", 'Ч' }, { "0xC8", 'Ш' },
                { "0xC9", 'Щ' }, { "0xCA", 'Ъ' }, { "0xCB", 'Ы' }, { "0xCC", 'Ь' }, { "0xCD", 'Э' },
                { "0xCE", 'Ю' }, { "0xCF", 'Я' }, { "0xD0", 'а' }, { "0xD1", 'б' }, { "0xD2", 'в' },
                { "0xD3", 'г' }, { "0xD4", 'д' }, { "0xD5", 'е' }, { "0xD6", 'ж' }, { "0xD7", 'з' },
                { "0xD8", 'и' }, { "0xD9", 'й' }, { "0xDA", 'к' }, { "0xDB", 'л' }, { "0xDC", 'м' },
                { "0xDD", 'н' }, { "0xDE", 'о' }, { "0xDF", 'п' }, { "0xE0", 'р' }, { "0xE1", 'с' },
                { "0xE2", 'т' }, { "0xE3", 'у' }, { "0xE4", 'ф' }, { "0xE5", 'х' }, { "0xE6", 'ц' },
                { "0xE7", 'ч' }, { "0xE8", 'ш' }, { "0xE9", 'щ' }, { "0xEA", 'ъ' }, { "0xEB", 'ы' },
                { "0xEC", 'ь' }, { "0xED", 'э' }, { "0xEE", 'ю' }, { "0xEF", 'я' }, { "0xFF", ' ' }
        };
        int port;
        string localIp;
        string remoteIp;
        int remoteport;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread sputnik = new Thread(new ThreadStart(ReceiveMessages));
            
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show(
            "Введите IP-адрессы и порты",
            "Сообщение",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.DefaultDesktopOnly);
            }
            else {
                localIp = textBox1.Text;
                remoteIp = textBox2.Text;
                port = Int32.Parse(textBox3.Text);
                remoteport = Int32.Parse(textBox4.Text);
                sputnik.Start();
                button1.Enabled = false;
                button2.Enabled = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
            } 
        }
        private void CheckConnection()
        {
            using (UdpClient udpClient = new UdpClient())
            {
                UDPMessage checkMessage = new UDPMessage(true, "Проверка соединения");
                byte[] dataToSend = Serialize(checkMessage);
                udpClient.Send(dataToSend, dataToSend.Length, remoteIp, remoteport);
            }
        }
        private void ReceiveMessages()
        {
            UdpClient udpClient = new UdpClient(port);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                UDPMessage receivedMessage = Deserialize(data);

                this.Invoke((MethodInvoker)(() =>
                {
                    if (receivedMessage.IsCheck)
                    {
                        UDPMessage responseMessage = new UDPMessage(false, "Проверка пройдена");
                        byte[] responseData = Serialize(responseMessage);
                        udpClient.Send(responseData, responseData.Length, remoteEP);
                    }
                    else
                    {
                        string text = receivedMessage.GetDecodedMessage();
                        textBox5.Text += $"{remoteEP.Address}: {text}{Environment.NewLine}";
                    }
                }));
            }

        }
        private byte[] Serialize(UDPMessage message)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, message);
                return ms.ToArray();
            }
        }

        private UDPMessage Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (UDPMessage)formatter.Deserialize(ms);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (UdpClient udpClient = new UdpClient())
            if (udpClient != null)
            {   
                IPEndPoint server = new IPEndPoint(IPAddress.Parse(remoteIp), remoteport);
                udpClient.Connect(server);
                int count = 0;
                StringBuilder encodedMessage = new StringBuilder();
                string predlog;
                string dvoich;
                predlog = textBox6.Text;
                while (count < textBox6.Text.Length)
                {
                    if (iso8859_51.ContainsKey(textBox6.Text[count]))
                    {
                        string hexValue = "0x" + iso8859_51[textBox6.Text[count]].ToString("X2") + ' ';
                        encodedMessage.Append(hexValue);
                    }
                    count++;
                }
                textBox6.Text= encodedMessage.ToString();
                count = 0;
                StringBuilder binaryMessage = new StringBuilder();
                while (count < predlog.Length)
                {
                    if (iso8859_51.ContainsKey(predlog[count]))
                    {
                        string binaryValue = Convert.ToString(iso8859_51[predlog[count]], 2).PadLeft(8, '0') + " ";
                        binaryMessage.Append(binaryValue);
                    }
                    count++;
                }
                dvoich = binaryMessage.ToString();
                textBox6.Text = "";
                textBox6.Text= remoteIp.ToString() + ": " +encodedMessage.ToString()+ " - " + dvoich + " - " + predlog + Environment.NewLine;
                    string messageToSend = textBox6.Text;
                    UDPMessage message = new UDPMessage(false, messageToSend);
                    byte[] dataToSend = Serialize(message);
                    udpClient.Send(dataToSend, dataToSend.Length);

                    textBox6.Clear();
                    textBox5.Text += localIp.ToString()+": "+ encodedMessage.ToString()+ " - " + dvoich + " - " + predlog + Environment.NewLine;
                string fileName = DateTime.UtcNow.ToString("dd.MM.yyyy_HH-mm-ss") + ".txt";
                using (StreamWriter writer = new StreamWriter(fileName, true))
                {
                    writer.WriteLine("Отправлено: " + encodedMessage.ToString());
                    writer.WriteLine("Получено: " + encodedMessage.ToString());
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
    [Serializable]
    public class UDPMessage
    {
        public bool IsCheck { get; set; }
        public int Length { get; set; }
        public byte[] Message { get; set; }

        public UDPMessage(bool isCheck, string message)
        {
            IsCheck = isCheck;
            Message = Encoding.UTF8.GetBytes(message);
            Length = Message.Length;
        }

        public string GetDecodedMessage()
        {
            return Encoding.UTF8.GetString(Message);
        }
    }

}
