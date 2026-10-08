using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeClient.Models
{
    public sealed class InjectPatternCommand : BaseCommand
    {
        public override byte OpCode => 0x02;
        public int Row { get; init; }
        public int Col { get; init; }
        public PatternType Pattern { get; init; }
        public override byte[] Serialize()
        {
            var buffer = new byte[13]; // 1 byte for OpCode + 4 bytes for Row + 4 bytes for Col + Pattern length
            buffer[0] = OpCode;
            BitConverter.GetBytes(Row).CopyTo(buffer, 1);
            BitConverter.GetBytes(Col).CopyTo(buffer, 5);
            BitConverter.GetBytes((int)Pattern).CopyTo(buffer, 9);
            return buffer;
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
