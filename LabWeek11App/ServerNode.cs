using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LabWeek11App
{
    public class ServerNode : IObservable<string>
    {
        private readonly ConcurrentBag<IObserver<string>> _observers;

        public int PortNumber { get; set; }
        public IPAddress IPAddress { get; set; }
        private Socket _listener;
        private IPEndPoint _localEndPoint;
        public LogicalClock Clock { get; set; }

        public ServerNode(int portNumber)
        {
            _observers = new ConcurrentBag<IObserver<string>>();
            PortNumber = portNumber;
        }

        public void SetupLocalEndPoint() {
            IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            _localEndPoint = new IPEndPoint(IPAddress, PortNumber);
        }

        public void StartListening() {
            _listener = new Socket(IPAddress.AddressFamily,
               SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(_localEndPoint);
            _listener.Listen(10);
        }

        private void ProcessRequests(Socket handler) {
            // Left over code from pre-part 4
            //ReportMessage("Client connected: " + handler.RemoteEndPoint);
            //handler.Shutdown(SocketShutdown.Both);
            //handler.Close();

            byte[] buffer = new byte[1024];
            string data;
            string request;
            do
            {
                data = "";
                while (true)
                {
                    int bytesRec = handler.Receive(buffer);
                    data += Encoding.ASCII.GetString(buffer, 0, bytesRec);
                    int index = data.IndexOf("<EOF>");
                    if (index > -1)
                    {
                        request = data.Substring(0, index);
                        break;
                    }
                }
                ReportMessage($"RECEIVED:{request} at {Clock.Counter}");
            } while (request != "Exit");



            
        }

        public void WaitForConnection() {

            while (true) {
                ReportMessage("\nWaiting for a connection...");
                Socket handler = _listener.Accept();

                Task.Factory.StartNew(() =>
                     ProcessRequests(handler)
                    );
            }
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);

            return new MessageUnsubscriber(_observers, observer);
        }

        public void ReportMessage(string message)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(message);
            }
        }
    }
}
