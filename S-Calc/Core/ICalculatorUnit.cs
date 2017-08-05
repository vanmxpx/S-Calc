using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S_Calc.Core
{
    interface ICalculatorUnit
    {
        string Name { get; }

        string Description { get; }

        UInt32 Rating { get; }

        bool IsRemovable { get; }

        void ResetRating();
    }
}
