using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Concurrent;

namespace LabWeek11App
{
    public partial class AppForm : Form
    {
        ServerNode _localServer;
        ConcurrentDictionary<int, RemoteNode> _remoteServers;

        public AppForm()
        {
            _remoteServers = new ConcurrentDictionary<int, RemoteNode>();
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
            var tokens = commandText.Split(" ", 2);
            switch (tokens[0])
            {
                case "set": // e.g. set 5001
                    ProcessSet(tokens);
                    break;
                case "connect":
                    ProcessConnect(tokens);
                    break;
                case "create_clock":
                    ProcessCreateClock(tokens);
                    break;
                case "clock":
                    ProcessClock(tokens);
                    break;
                case "send":
                    ProcessSend(tokens);
                    break;
            }
        }

        private void ProcessSend(string[] parameters) {
            parameters = parameters[1].Split("|");
            int port = int.Parse(parameters[0]);
            string message = parameters[1];
            message = $"Clock:{_localServer.Clock.Counter}:" + message;
            if (_remoteServers.ContainsKey(port))
            {
                _remoteServers[port].SendRequest(message);
            }
            else {
                _localServer.ReportMessage("Cannot send message to target. Not connected to node at port " + port);
            }
        }

        private void ProcessClock(string[] parameters) {
            if (_localServer.Clock != null)
            {
                _localServer.ReportMessage("Clock: " + _localServer.Clock.Counter);
            }
            else {
                _localServer.ReportMessage("Clock has not started yet.");
            }
        }

        private void ProcessCreateClock(string[] parameters) {
            parameters = parameters[1].Split("|");
            int interval = int.Parse(parameters[0]);
            int step = int.Parse(parameters[1]);
            _localServer.Clock = new LogicalClock(interval, step);
            _localServer.Clock.Start();
            _localServer.ReportMessage($"The clock ({interval}|{step}) has started...");
        }

        private void ProcessConnect(string[] parameters) {
            IPAddress localAddr = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            int port = int.Parse(parameters[1]);
            _remoteServers[port] = new RemoteNode(_localServer);
            _remoteServers[port].ConnectToRemoteEndPoint(localAddr, port);
        }

        private void ProcessSet(string[] parameters)
        {
            // parameters ::= <port>
            var port = Int32.Parse(parameters[1]);
            OutputBox.Text += "Port: " + port;

            _localServer = new ServerNode(port);
            _localServer.Subscribe(new StringObserver(OutputBox));
            _localServer.SetupLocalEndPoint();
            OutputBox.Text += "\n" + _localServer.IPAddress.ToString(); // I like to be explicit :P
            _localServer.StartListening();
            Task.Run(() => _localServer.WaitForConnection());
        }
    }
}
