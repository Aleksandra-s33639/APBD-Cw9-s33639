using System;
using System.Collections.Generic;

namespace APBD_Cw9_s33639.Models;

public partial class ComponentManufacturer
{
    public int Id { get; set; }

    public string Abbreviation { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateOnly FoundationDate { get; set; }

    public virtual ICollection<Component> Components { get; set; } = new List<Component>();
}
