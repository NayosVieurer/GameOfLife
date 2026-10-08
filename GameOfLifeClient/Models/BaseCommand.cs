using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeClient.Models
{
    public abstract class BaseCommand
    {
        public abstract byte OpCode { get; }
        public abstract byte[] Serialize();
    }
}
