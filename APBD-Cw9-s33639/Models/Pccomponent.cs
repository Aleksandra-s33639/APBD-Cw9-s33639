using System;
using System.Collections.Generic;

namespace APBD_Cw9_s33639.Models;

public partial class Pccomponent
{
    public int PcId { get; set; }

    public string ComponentCode { get; set; } = null!;

    public int Amount { get; set; }

    public virtual Component ComponentCodeNavigation { get; set; } = null!;

    public virtual Pc Pc { get; set; } = null!;
}
