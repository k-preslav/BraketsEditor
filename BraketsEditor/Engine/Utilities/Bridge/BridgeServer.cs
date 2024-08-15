using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BraketsEngine;

public class BridgeServer
{
    public Action<string> OnRecieve;

    private TcpListener _listener;
    private static CancellationTokenSource cancel;

    private static TcpClient connectedClient;

    public BridgeServer(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        cancel = new CancellationTokenSource();
    }

    public void Start()
    {
        _listener.Start();
        Debug.Log("Bridge Server started!");

        Task.Run(async () =>
        {
            while (!cancel.IsCancellationRequested)
            {
                TcpClient client = _listener.AcceptTcpClient();
                Debug.Log("Bridge Client connected! Listening...");

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

                Thread.Sleep(30);
            }
        }
        catch (Exception ex)
        {
            Debug.Error("Error handling bridge client: " + ex.Message, this);
        }
        finally
        {
            Debug.Log("Client disconnected.");
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
            Debug.Error("Error sending message to client: " + ex.Message);
        }
    }

    public void Stop()
    {
        cancel.Cancel();
        connectedClient?.Close();
        _listener.Stop();
    }
}