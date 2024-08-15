using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BraketsEngine
{
    public class BridgeClient
    {
        private TcpClient _client;
        private CancellationTokenSource cancel;

        public Action<string> OnReceive;

        public BridgeClient()
        {
            _client = new TcpClient();
            cancel = new CancellationTokenSource();
        }

        public async Task ConnectAsync(string hostname, int port)
        {
            try
            {
                await _client.ConnectAsync(hostname, port, cancel.Token);
                Debug.Log("Connected to bridge server");

                await ListenForMessagesAsync(cancel.Token);
            }
            catch (Exception ex)
            {
                Debug.Error("Error connecting to bridge server: " + ex.Message, this);
            }
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                NetworkStream stream = _client.GetStream();
                byte[] data = Encoding.ASCII.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length, cancel.Token);
            }
            catch (Exception ex)
            {
                Debug.Error("Error sending message to bridge server: " + ex.Message, this);
                Disconnect();
            }
        }

        private async Task ListenForMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                NetworkStream stream = _client.GetStream();
                byte[] buffer = new byte[1024];

                while (!cancellationToken.IsCancellationRequested)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    OnReceive?.Invoke(message);
                }
            }
            catch (Exception ex)
            {
                Debug.Error("Error receiving message from bridge server: " + ex.Message, this);
            }
            finally
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            _client.Close();
            cancel.Cancel();
            Debug.Log("Disconnected from bridge server.");
        }
    }
}
