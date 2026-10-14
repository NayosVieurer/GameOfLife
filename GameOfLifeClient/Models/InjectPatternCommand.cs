using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeClient.Models
{
    public sealed class InjectPatternCommand : BaseCommand
    {
        public override CommandType OpCode => CommandType.InjectPattern;
        public int Row { get; init; }
        public int Col { get; init; }
        public PatternType Pattern { get; init; }

        public override byte DataSize => 12;

        protected override void SerializeData(Span<byte> buffer)
        {
            BinaryPrimitives.WriteInt32LittleEndian(buffer[0..4], Row);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[4..8], Col);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[8..12], (int)Pattern);
        }
    }

    public enum PatternType
    {
        Glider,
        SmallExploder,
        Exploder,
        TenCellRow,
        LightweightSpaceship,
        Tumbler,
        GosperGliderGun
    }
}
