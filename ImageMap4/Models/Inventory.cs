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
