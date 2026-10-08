using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeClient.Models
{
    public sealed class StartSimCommand : BaseCommand
    {
        public override byte OpCode => 0x01;

        public int Rows { get; init; }
        public int Cols { get; init; }
        public int ChanceForCell { get; init; }

        public override byte[] Serialize()
        {
            var buffer = new byte[13]; // 1 byte for OpCode + 4 bytes for Rows + 4 bytes for Cols + 4 bytes for ChanceForCell

            buffer[0] = OpCode;
            BitConverter.GetBytes(Rows).CopyTo(buffer, 1);
            BitConverter.GetBytes(Cols).CopyTo(buffer, 5);
            BitConverter.GetBytes(ChanceForCell).CopyTo(buffer, 9);

            return buffer;
        }
    }
}
