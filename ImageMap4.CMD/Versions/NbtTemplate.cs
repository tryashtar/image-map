using fNbt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ImageMap4;

public class NbtTemplate
{
    private readonly NbtCompound Template;
    public NbtTemplate(NbtCompound compound)
    {
        Template = compound;
    }

    public NbtCompound Create(params (string name, Func<NbtTag> maker)[] variables)
    {
        var compound = (NbtCompound)Template.Clone();
        foreach (var item in compound.GetAllTags().OfType<NbtString>())
        {
            foreach (var (name, maker) in variables)
            {
                if (item.Value == "@" + name)
                    item.Parent[item.Name] = maker();
            }
        }
        return compound;
    }
}
