using fNbt;

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
                else if (item.Value.Contains("@" + name))
                {
                    item.Value = item.Value.Replace("@" + name, maker().StringValue);
                }
            }
        }
        return compound;
    }
}
