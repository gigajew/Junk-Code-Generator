using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCG
{
    [Flags]
    public enum SyntheticModifier : int
    {
        Public = 16,
        Private = 32,
        Protected = 64, 
        Static = 128,
        Delegate = 256
    }
}
