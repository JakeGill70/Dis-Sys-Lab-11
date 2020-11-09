using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net;

namespace LabWeek11App
{
    public partial class AppForm : Form
    {
        ServerNode _localServer;
        RemoteNode _remoteServer;

        public AppForm()
        {
            InitializeComponent();
        }

        private void CmdBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ProcessCommand(CmdBox.Text);
                CmdBox.Text = "";
            }
        }

        private void ProcessCommand(string commandText)
        {
            // commandText ::= <command> <parameters>
            var tokens = commandText.Split(' ');
            switch (tokens[0])
            {
                case "set": // e.g. set 5001
                    ProcessSet(tokens[1]);
                    break;
                case "connect":
                    ProcessConnect(tokens[1]);
                    break;
            }
        }

        private void ProcessConnect(string portString) {
            IPAddress localAddr = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            int port = int.Parse(portString);
            _remoteServer = new RemoteNode(_localServer);
            _remoteServer.ConnectToRemoteEndPoint(localAddr, port);
        }

        private void ProcessSet(string parameters)
        {
            // parameters ::= <port>
            var port = Int32.Parse(parameters);
            OutputBox.Text += "Port: " + port;

            _localServer = new ServerNode(port);
            _localServer.Subscribe(new StringObserver(OutputBox));
            _localServer.SetupLocalEndPoint();
            OutputBox.Text += "\n" + _localServer.IPAddress.ToString(); // I like to be explicit :P
            _localServer.StartListening();
            // Changed this after rejoining...
            //Task.Factory.StartNew(() =>
            //     _localServer.WaitForConnection()
            //    );
            Task.Run(() => _localServer.WaitForConnection());
        }
    }
}
