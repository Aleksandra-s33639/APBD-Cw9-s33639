using System;
using System.Collections.Generic;

namespace APBD_Cw9_s33639.Models;

public partial class ComponentType
{
    public int Id { get; set; }

    public string Abbreviation { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Component> Components { get; set; } = new List<Component>();
}
