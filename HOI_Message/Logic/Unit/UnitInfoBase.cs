using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI_Message.Logic.Unit;

public abstract class UnitInfoBase
{
    public abstract string OwnCountryTag { get; }
    public abstract int UnitSum { get; }
}
