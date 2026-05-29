using System;
using System.Collections.Generic;

namespace APBD_Cw9_s33639.Models;

public partial class Component
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int ComponentManufacturerId { get; set; }

    public int ComponentTypeId { get; set; }

    public virtual ComponentManufacturer ComponentManufacturer { get; set; } = null!;

    public virtual ComponentType ComponentType { get; set; } = null!;

    public virtual ICollection<Pccomponent> Pccomponents { get; set; } = new List<Pccomponent>();
}
