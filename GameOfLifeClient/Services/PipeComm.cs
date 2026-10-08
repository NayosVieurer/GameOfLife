using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeClient.Services
{
    public class PipeComm : IDisposable
    {
        private int GridSize;
        private const string serverName = ".";
        private const string streamPipeName = "GameOfLifeStream";

        private NamedPipeClientStream streamingClient;
        private NamedPipeClientStream commandClient;
        private readonly byte[] buffer;

        private CancellationTokenSource cts;

        public EventHandler<byte[]> OnFrameReceived;

        public PipeComm()
        {
        }

        public void Connect()
        {
            cts = new CancellationTokenSource();

            Task.Run(ConnectToPipe);
        }

        private async Task ConnectToPipe()
        {
            while (!cts.Token.IsCancellationRequested && (streamingClient == null || !streamingClient.IsConnected))
            {
                try
                {
                    streamingClient = new NamedPipeClientStream(".", streamPipeName, PipeDirection.In);
                    
                    await streamingClient.ConnectAsync(cts.Token);         
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation
                    break;
                }
                catch (Exception ex)
                {
                    // Handle other exceptions (e.g., pipe not available)
                    Console.WriteLine($"Error: {ex.Message}");
                    await Task.Delay(1000); // Wait before retrying
                }
            }

            if(streamingClient.IsConnected)
            {
                // Start reading from the pipe
                await ReadFromPipe();
            }
        }

        private async Task ReadFromPipe()
        {
            while (!cts.Token.IsCancellationRequested && streamingClient.IsConnected)
            {
                try
                {
                    int bytesRead = 0;

                    while(bytesRead < GridSize)
                    {
                        int read = await streamingClient.ReadAsync(buffer, bytesRead, GridSize - bytesRead, cts.Token);
                        if (read == 0)
                        {
                            // Pipe has been closed
                            break;
                        }
                        bytesRead += read;
                    }

                    if (bytesRead == GridSize)
                    {
                        // Successfully read the entire grid
                        // Process the buffer as needed
                        OnFrameReceived?.Invoke(this, buffer);

                        Console.WriteLine("Received grid data from pipe.");
                    }

                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation
                    break;
                }
                catch (Exception ex)
                {
                    // Handle other exceptions (e.g., pipe disconnected)
                    Console.WriteLine($"Error: {ex.Message}");
                    await Task.Delay(1000); // Wait before retrying
                }
            }
        }

        public void Dispose()
        {
            cts?.Cancel();
            streamingClient?.Dispose();
        }
    }
}
