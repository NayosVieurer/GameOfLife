using GameOfLifeClient.Models;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeClient.Services
{
    public class PipeComm : IDisposable
    {
        private int gridSize = 512 * 512;
        private const string serverName = ".";
        private const string streamPipeName = "GameOfLifeStream";
        private const string commandPipeName = "GameOfLifeCommands";

        private NamedPipeClientStream streamingClient;
        private NamedPipeClientStream commandClient;
        private byte[] buffer;

        private CancellationTokenSource cts;

        public EventHandler<byte[]> OnFrameReceived;

        public int GridSize 
        { 
            get { return gridSize; }
            set
            {
                if (gridSize != value)
                {
                    gridSize = value;
                }   
                buffer = new byte[gridSize];
            }
        } 

        public PipeComm()
        {
        }

        public void Connect()
        {
            cts = new CancellationTokenSource();

            Task.Run(async () => commandClient = await ConnectToPipe(commandPipeName, PipeDirection.Out));
            _ = ConnectToStreamingPipe();
        }

        public async Task SendCommandAsync(BaseCommand command)
        {
            if (commandClient == null || !commandClient.IsConnected)
                throw new InvalidOperationException("Command pipe is not connected.");

            int commandSize = 2 + command.DataSize; // OpCode (1 byte) + DataSize (1 byte) + Data

            byte[] commandBuffer = ArrayPool<byte>.Shared.Rent(commandSize);

            command.Serialize(commandBuffer);

            ReadOnlyMemory<byte> data = commandBuffer.AsMemory(0, commandSize);

            await commandClient.WriteAsync(data, cts.Token);
            await commandClient.FlushAsync(cts.Token);
        }

        private async Task<NamedPipeClientStream> ConnectToPipe(string pipeName, PipeDirection direction)
        {
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    var pipeToConnect = new NamedPipeClientStream(".", pipeName, direction);

                    await pipeToConnect.ConnectAsync(cts.Token);

                    return pipeToConnect;
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

            return null;
        }

        private async Task ConnectToStreamingPipe()
        {
            while(!cts.Token.IsCancellationRequested && (streamingClient == null || !streamingClient.IsConnected))
            {
                streamingClient = await ConnectToPipe(streamPipeName, PipeDirection.In);

                if (streamingClient.IsConnected)
                {
                    // Start reading from the pipe
                    await Task.Run(ReadFromPipe);
                }
            }
        }

        private async Task ReadFromPipe()
        {
            while (!cts.Token.IsCancellationRequested && streamingClient.IsConnected)
            {
                try
                {
                    int bytesRead = 0;


                    while (bytesRead < GridSize)
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
