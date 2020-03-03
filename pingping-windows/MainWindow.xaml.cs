using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace PingPing___Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Socket _server;
        private Thread _t;

        public MainWindow()
        {
            InitializeComponent();
            UserIp.Text = GetUserIp();
        }

        private static string GetUserIp()
        {
            var userIp = "주소를 불러올 수 없습니다. 네트워크 상태를 확인해주세요!";
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var i in host.AddressList)
            {
                if (i.AddressFamily != AddressFamily.InterNetwork) continue;
                userIp = i.ToString();
                break;
            }

            return userIp;
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            Switch.Content = "종료";
            MessageBox.Show("작동이 시작되었습니다", "시작");

            _t = new Thread(new ThreadStart(StartSocketConnection));
            _t.Start();
        }

        private void StartSocketConnection()
        {
            try
            {
                //소켓 생성
                _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                //바인딩
                _server.Bind(new IPEndPoint(IPAddress.Any, 7777));
               

                while (true)
                {
                    //리슨
                    _server.Listen(10);
                    //연결을 받아서 새 소켓 생성
                    var client = _server.Accept();
                    
                    var buff = new byte[1024];
                    
                    //리시브
                    var data = client.Receive(buff);
                    var msg = Encoding.UTF8.GetString(buff, 0, data);

                    DoSomething(msg);
                    
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("작동이 종료되었습니다.", "종료");
            }
        }

        private static void DoSomething(string msg)
        {
            switch (msg)
            {
                case "test":
                    MessageBox.Show("테스트 메세지 전달 성공", "Test");
                    break;
                case "start":
                    keybd_event(VK_F5, 0, WM_KEYDOWN, 0);
                    keybd_event(VK_F5, 0, WM_KEYUP, 0);
                    break;
                case "prev":
                    keybd_event(VK_LEFT, 0, WM_KEYDOWN, 0);
                    keybd_event(VK_LEFT, 0, WM_KEYUP, 0);
                    break;
                case "next":
                    keybd_event(VK_RIGHT, 0, WM_KEYDOWN, 0);
                    keybd_event(VK_RIGHT, 0, WM_KEYUP, 0);
                    break;
                case "end":
                    keybd_event(VK_ESC, 0, WM_KEYDOWN, 0);
                    keybd_event(VK_ESC, 0, WM_KEYUP, 0);
                    break;
            }
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            Switch.Content = "시작";
            StopSocketConnection();
        }

        private void StopSocketConnection()
        {
            _server.Close();
        }
        
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int VK_LEFT = 0x25;
        private const int VK_RIGHT = 0x27;
        private const int VK_ESC = 0x1B;
        private const int VK_F5 = 0x74;
    }
}