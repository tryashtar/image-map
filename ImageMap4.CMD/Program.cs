using ImageMap4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Dithering;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.Diagnostics;
using System.Text;
using TryashtarUtils.Utility;

Console.OutputEncoding = Encoding.UTF8;
if (args.Length == 0)
{
    Console.WriteLine(
@$"Usage:
  {Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName)} <world folder> <actions...>

Actions:
  --inventory <name>
  --import <image file/folder> <options...>
    Options:
     --size <width>,<height>
     --scaling pixel|bicubic
     --fill uniform|stretch|crop
     --algorithm good|euclidean|ciede2000|cie76|cmc
     --dithering none|floyd|burks
     --background <color>
     --id <id>
  --delete <ids...>
  --change-id <<from>,<to>...>
");
    return;
}

string folder = Path.GetFullPath(args[0]);
World world;
IInventory? inventory = null;
if (!Directory.Exists(folder))
{
    Console.Error.WriteLine($"World folder '{folder}' not found");
    return;
}
if (!File.Exists(Path.Combine(folder, "level.dat")))
{
    Console.Error.WriteLine($"World folder '{folder}' does not appear to be a Minecraft world, no level.dat found");
    return;
}
if (Directory.Exists(Path.Combine(folder, "db")))
{
    Console.WriteLine($"Opening Bedrock world at {folder}");
    world = new BedrockWorld(folder);
}
else
{
    Console.WriteLine($"Opening Java world at {folder}");
    world = new JavaWorld(folder);
}
for (int i = 1; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--inventory":
            i++;
            if (i >= args.Length)
            {
                Console.Error.WriteLine("Expected an inventory name after --inventory");
                break;
            }
            var inventories = world.GetInventories();
            var selected = inventories.FirstOrDefault(x => x.Name == args[i]);
            if (selected == null)
                Console.Error.WriteLine($"No inventory named {args[i]} found: ({String.Join(", ", inventories.Select(x => x.Name))})");
            else
            {
                inventory = selected;
                Console.WriteLine($"Set inventory to {inventory.Name}");
            }
            break;
        case "--delete":
            var ids = new List<long>();
            if (i >= args.Length - 1)
            {
                Console.Error.WriteLine("Expected map IDs after --delete");
                break;
            }
            while (i < args.Length - 1 && !args[i + 1].StartsWith("--"))
            {
                i++;
                if (long.TryParse(args[i], out long id))
                    ids.Add(id);
                else
                    Console.Error.WriteLine($"Expected a numeric map ID, instead got '{args[i]}'");
            }
            Console.WriteLine($"Removing {ids.Count} maps");
            world.RemoveMaps(ids);
            break;
        case "--change-id":
            Console.WriteLine("Fetching maps from world");
            var maps = world.GetMapsAsync().ToListAsync().AsTask().Result;
            var remove = new List<long>();
            var add = new List<Map>();
            if (i >= args.Length - 1)
            {
                Console.Error.WriteLine("Expected map IDs after --change-id");
                break;
            }
            while (i < args.Length - 1 && !args[i + 1].StartsWith("--"))
            {
                i++;
                var split = args[i].Split(',');
                if (split.Length != 2 || !long.TryParse(split[0], out long from) || !long.TryParse(split[1], out long to))
                    Console.Error.WriteLine($"Unexpected two map IDs separated by a comma, instead got '{args[i]}'");
                else
                {
                    var map = maps.FirstOrDefault(x => x.ID == from);
                    if (map == null)
                        Console.Error.WriteLine($"No map with ID {from} found");
                    else
                    {
                        remove.Add(from);
                        add.Add(map);
                        map.ID = to;
                    }
                }
            }
            Console.WriteLine($"Changing IDs of {add.Count} maps");
            world.RemoveMaps(remove);
            world.AddMaps(add);
            break;
        case "--import":
            i++;
            if (i >= args.Length)
            {
                Console.Error.WriteLine("Expected an file path after --import");
                break;
            }
            string import = Path.GetFullPath(args[i]);
            var size = (width: 1, height: 1);
            IResampler scaling = KnownResamplers.NearestNeighbor;
            ResizeMode fill = ResizeMode.Max;
            IColorAlgorithm algorithm = new SimpleAlgorithm();
            IDither? dither = null;
            Rgba32 background = Color.Transparent;
            long? starting_id = null;
            while (i < args.Length - 1)
            {
                i++;
                switch (args[i])
                {
                    case "--size":
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --size");
                            break;
                        }
                        var split = args[i].Split(',');
                        if (split.Length != 2 || !int.TryParse(split[0], out int width) || !int.TryParse(split[1], out int height))
                            Console.Error.WriteLine($"Unexpected width and height separated by a comma, instead got '{args[i]}'");
                        else
                            size = (width, height);
                        break;
                    case "--scaling":
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --scaling");
                            break;
                        }
                        if (args[i] == "pixel")
                            scaling = KnownResamplers.NearestNeighbor;
                        else if (args[i] == "bicubic")
                            scaling = KnownResamplers.Bicubic;
                        else
                            Console.Error.WriteLine($"Unexpected scaling '{args[i]}', expected pixel or bicubic");
                        break;
                    case "--fill":
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --fill");
                            break;
                        }
                        if (args[i] == "uniform")
                            fill = ResizeMode.Max;
                        else if (args[i] == "stretch")
                            fill = ResizeMode.Stretch;
                        else if (args[i] == "crop")
                            fill = ResizeMode.Crop;
                        else
                            Console.Error.WriteLine($"Unexpected fill '{args[i]}', expected uniform, stretch, or crop");
                        break;
                    case "--algorithm":
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --algorithm");
                            break;
                        }
                        if (args[i] == "good")
                            algorithm = new SimpleAlgorithm();
                        else if (args[i] == "euclidean")
                            algorithm = new EuclideanAlgorithm();
                        else if (args[i] == "ciede2000")
                            algorithm = new Ciede2000Algorithm();
                        else if (args[i] == "cie76")
                            algorithm = new Cie76Algorithm();
                        else if (args[i] == "cmc")
                            algorithm = new CmcAlgorithm();
                        else
                            Console.Error.WriteLine($"Unexpected algorithm '{args[i]}', expected good, euclidean, ciede2000, cie76, or cmc");
                        break;
                    case "--dithering":
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --dithering");
                            break;
                        }
                        if (args[i] == "none")
                            dither = null;
                        else if (args[i] == "floyd")
                            dither = KnownDitherings.FloydSteinberg;
                        else if (args[i] == "burks")
                            dither = KnownDitherings.Burks;
                        else
                            Console.Error.WriteLine($"Unexpected dithering '{args[i]}', expected none, floyd, or burks");
                        break;
                    case "--background":
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --background");
                            break;
                        }
                        if (Color.TryParse(args[i], out var color))
                            background = color;
                        else
                            Console.Error.WriteLine($"Couldn't determine a color from '{args[i]}', try using hex");
                        break;
                    case "--id":
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --id");
                            break;
                        }
                        if (long.TryParse(args[i], out var id))
                            starting_id = id;
                        else
                            Console.Error.WriteLine($"Expected a numeric map ID, instead got '{args[i]}'");
                        break;
                    default:
                        goto process;
                }
            }
            process:
            var files = new List<string>();
            if (File.Exists(import))
                files.Add(import);
            else if (Directory.Exists(import))
            {
                var children = Directory.GetFiles(import);
                Console.WriteLine($"Found {children.Length} files in folder");
                files.AddRange(children);
            }
            else
                Console.WriteLine($"Image file '{import}' not found");
            foreach (var file in files)
            {
                var settings = new ImportSettings(
                    new PreviewImage(PendingSource.FromPath(file)),
                    size.width, size.height,
                    new(scaling), fill, background,
                    new ProcessSettings(dither, algorithm)
                );
                if (starting_id == null)
                {
                    starting_id = 0;
                    while (world.IsIdTaken(starting_id.Value))
                        starting_id++;
                    Console.WriteLine($"Selected automatic safe ID {starting_id}");
                }
                Console.WriteLine($"Generating {settings.Width * settings.Height} maps from image");
                var batch = world.MakeMaps(settings);
                var data = ListUtils.Map2D(batch, x => new Map(starting_id++.Value, x));
                var structure = new StructureGrid("imagemap:" + Path.GetFileNameWithoutExtension(file), data);
                Console.WriteLine("Adding maps to world");
                world.AddMaps(ListUtils.Flatten(data));
                if (inventory != null)
                {
                    Console.WriteLine($"Adding structure {structure.Identifier} to inventory");
                    world.AddStructures(new[] { structure }, inventory);
                }
            }
            break;
        default:
            Console.Error.WriteLine($"Unexpected action '{args[i]}', expected --inventory, --import, --delete, or --change-id");
            break;
    }
}
