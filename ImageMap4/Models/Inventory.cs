using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap4;

public abstract class Inventory
{
    public abstract string Name { get; }
    public abstract void AddItem(NbtCompound item);
}

public class NoInventory : Inventory
{
    public override string Name => "None";
    public override void AddItem(NbtCompound item) { }
}
