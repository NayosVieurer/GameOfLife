using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeClient.Models
{
    public sealed class StartSimCommand : BaseCommand
    {
        public override CommandType OpCode => CommandType.StartSimulation;

        public int Rows { get; init; }
        public int Cols { get; init; }
        public int ChanceForCell { get; init; }

        public override byte DataSize => 12;

        protected override void SerializeData(Span<byte> buffer)
        {
            BinaryPrimitives.WriteInt32LittleEndian(buffer[0..4], Rows);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[4..8], Cols);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[8..12], ChanceForCell);
        }
    }
}
