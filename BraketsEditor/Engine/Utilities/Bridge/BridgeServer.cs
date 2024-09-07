using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BraketsEngine
{
    public class BridgeServer
    {
        public string NAME;
        public int PORT;

        public bool showRecieved = false;

        public Action<string> OnRecieve;

        private TcpListener _listener;
        private CancellationTokenSource cancel;

        private static TcpClient connectedClient;

        public BridgeServer(string name)
        {
            NAME = name;
            PORT = FindAvailablePort();

            _listener = new TcpListener(IPAddress.Any, PORT);
            cancel = new CancellationTokenSource();
        }

        public void Start()
        {
            _listener.Start();
            Debug.Log($"Bridge Server started: {NAME}, port:{PORT}", this);

            Task.Run(async () =>
            {
                while (!cancel.IsCancellationRequested)
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    Debug.Log($"[{NAME}] Bridge Client connected! Listening...");

                    await HandleClientAsync(client, cancel);
                }
            }, cancel.Token);
        }

        private async Task HandleClientAsync(TcpClient client, CancellationTokenSource cancellationToken)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] data = new byte[1024];

                connectedClient = client;

                while (!cancellationToken.IsCancellationRequested)
                {
                    int bytes = await stream.ReadAsync(data, 0, data.Length, cancellationToken.Token);
                    if (bytes == 0)
                    {
                        // Client disconnected
                        break;
                    }

                    string incomingData = Encoding.ASCII.GetString(data, 0, bytes);
                    OnRecieve.Invoke(incomingData);

                    if (showRecieved)
                    {
                        Debug.Log($"[{NAME}] Recieved from client: {incomingData}");
                    }

                    Thread.Sleep(30);
                }
            }
            catch (Exception ex)
            {
                Debug.Error($"[{NAME}] Error handling bridge client: " + ex.Message, this);
            }
            finally
            {
                Debug.Log($"[{NAME}] Client disconnected.");
                client.Close();
            }
        }

        public async Task SendMessageToClient(string msg)
        {
            if (connectedClient is null)
                return;

            try
            {
                if (connectedClient.Connected)
                {
                    byte[] data = Encoding.ASCII.GetBytes(msg);
                    NetworkStream stream = connectedClient.GetStream();
                    await stream.WriteAsync(data, 0, data.Length, cancel.Token);
                }
            }
            catch (Exception ex)
            {
                Debug.Error($"{NAME} Error sending message to client: " + ex.Message);
            }
        }

        public void Stop()
        {
            cancel.Cancel();
            connectedClient?.Close();
            _listener.Stop();
        }

        public int FindAvailablePort(int startPort = 1024, int endPort = 65535)
        {
            for (int port = startPort; port <= endPort; port++)
            {
                try
                {
                    TcpListener testListener = new TcpListener(IPAddress.Any, port);
                    testListener.Start();
                    testListener.Stop();
                    return port;
                }
                catch (SocketException) { }
            }
            throw new InvalidOperationException($"[{NAME}] No available ports found in the specified range.");
        }
    }
}
