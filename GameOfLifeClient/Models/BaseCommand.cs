using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeClient.Models
{
    public abstract class BaseCommand
    {
        public abstract CommandType OpCode { get; }
        public abstract byte DataSize { get; }

        public void Serialize(byte[] buffer)
        {
            if(buffer.Length < DataSize + 2)
                throw new ArgumentException("Buffer is too small for the command data.");

            buffer[0] = (byte)OpCode;
            buffer[1] = DataSize;

            Span<byte> dataBuffer = buffer.AsSpan().Slice(2, DataSize);

            SerializeData(dataBuffer);
        }

        protected abstract void SerializeData(Span<byte> buffer);
    }

    public enum CommandType : byte
    {
        StartSimulation = 0x01,
        InjectPattern = 0x02,
        ACK = 0x06
    }
}
