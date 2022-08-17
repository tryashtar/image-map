using fNbt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ImageMap4;

public interface IJavaVersion
{
    IMapColors Colors { get; }
    IJavaNbtFormat NbtFormat { get; }
}

public class JavaB1p8Version : IJavaVersion
{
    public override string ToString() => "Beta 1.8+";
    public IMapColors Colors => JavaMapColors.Beta1p8;
    public IJavaNbtFormat NbtFormat => new Beta1p8NbtFormat();
}

public class Java1p7SnapshotVersion : IJavaVersion
{
    public override string ToString() => "13w42a+";
    public IMapColors Colors => JavaMapColors.Snapshot13w42a;
    public IJavaNbtFormat NbtFormat => new Beta1p8NbtFormat();
}

// 13w43a+
public class Java1p7Version : IJavaVersion
{
    public override string ToString() => "1.7+";
    public IMapColors Colors => JavaMapColors.Snapshot13w43a;
    public IJavaNbtFormat NbtFormat => new Beta1p8NbtFormat();
}

// 1.8.1-pre1+
public class Java1p8Version : IJavaVersion
{
    public override string ToString() => "1.8+";
    public IMapColors Colors => JavaMapColors.Release1p8;
    public IJavaNbtFormat NbtFormat => new Release1p8NbtFormat();
}

//16w21a+
public class Java1p10Version : IJavaVersion
{
    public override string ToString() => "1.10+";
    public IMapColors Colors => JavaMapColors.Release1p8;
    public IJavaNbtFormat NbtFormat => new Release1p10NbtFormat();
}

//16w32a+
public class Java1p11Version : IJavaVersion
{
    public override string ToString() => "1.11+";
    public IMapColors Colors => JavaMapColors.Release1p8;
    public IJavaNbtFormat NbtFormat => new Snapshot16w32aNbtFormat();
}

// 17w17a+
public class Java1p12Version : IJavaVersion
{
    public override string ToString() => "1.12+";
    public IMapColors Colors => JavaMapColors.Release1p12;
    public IJavaNbtFormat NbtFormat => new Release1p10NbtFormat();
}

// 17w47a+
public class Java1p13Version : IJavaVersion
{
    public override string ToString() => "1.13+";
    public IMapColors Colors => JavaMapColors.Release1p12;
    public IJavaNbtFormat NbtFormat => new Release1p10NbtFormat();
}

// 19w02a+
public class Java1p14Version : IJavaVersion
{
    public override string ToString() => "1.14+";
    public IMapColors Colors => JavaMapColors.Release1p12;
    public IJavaNbtFormat NbtFormat => new Release1p10NbtFormat();
}

// 1.16 pre-6+
public class Java1p16Version : IJavaVersion
{
    public override string ToString() => "1.16+";
    public IMapColors Colors => JavaMapColors.Release1p16;
    public IJavaNbtFormat NbtFormat => new Release1p16NbtFormat();
}

// 21w15a+
public class Java1p17SnapshotVersion : IJavaVersion
{
    public override string ToString() => "21w15a";
    public IMapColors Colors => JavaMapColors.Snapshot21w15a;
    public IJavaNbtFormat NbtFormat => new Release1p16NbtFormat();
}

// 21w16a+
public class Java1p17Version : IJavaVersion
{
    public override string ToString() => "1.17+";
    public IMapColors Colors => JavaMapColors.Snapshot21w16a;
    public IJavaNbtFormat NbtFormat => new Release1p16NbtFormat();
}
