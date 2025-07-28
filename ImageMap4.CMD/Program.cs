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
    var filename = Process.GetCurrentProcess().MainModule?.FileName ?? "ImageMap-cmd";
    Console.WriteLine(
        @$"Usage:
  {Path.GetFileName(filename)} <world folder> <actions...>

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
  --export [<<id>,<path>...>]
  --list
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
        {
            i++;
            if (i >= args.Length)
            {
                Console.Error.WriteLine("Expected an inventory name after --inventory");
                break;
            }

            var inventories = world.GetInventories().ToList();
            var selected = inventories.FirstOrDefault(x => x.Name == args[i]);
            if (selected == null)
            {
                Console.Error.WriteLine(
                    $"No inventory named {args[i]} found: ({String.Join(", ", inventories.Select(x => x.Name))})");
                break;
            }

            inventory = selected;
            break;
        }
        case "--export":
        {
            if (i >= args.Length - 1)
            {
                Console.Error.WriteLine("Expected map IDs after --export");
                break;
            }
            Console.WriteLine("Fetching maps from world");
            var maps = world.GetMapsAsync().ToListAsync().AsTask().Result;
            while (i < args.Length - 1 && !args[i + 1].StartsWith("--"))
            {
                i++;
                var split = args[i].Split(',', 2);
                if (split.Length != 2 || !long.TryParse(split[0], out long id))
                {
                    Console.Error.WriteLine($"Expected map ID and file path separated by a comma, instead got '{args[i]}'");
                    continue;
                }

                var map = maps.FirstOrDefault(x => x.ID == id);
                if (map == null)
                {
                    Console.Error.WriteLine($"No map with ID {id} found");
                    continue;
                }
                
                map.Data.Image.Save(split[1]);
            }

            break;
        }
        case "--list":
        {
            Console.WriteLine("Fetching maps from world");
            var maps = world.GetMapsAsync().ToListAsync().AsTask().Result;
            Console.WriteLine($"Taken map IDs: [{String.Join(", ", maps.Select(x => x.ID))}]");
            break;
        }
        case "--delete":
        {
            if (i >= args.Length - 1)
            {
                Console.Error.WriteLine("Expected map IDs after --delete");
                break;
            }

            var ids = new List<long>();
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
        }
        case "--change-id":
        {
            if (i >= args.Length - 1)
            {
                Console.Error.WriteLine("Expected map IDs after --change-id");
                break;
            }

            Console.WriteLine("Fetching maps from world");
            var maps = world.GetMapsAsync().ToListAsync().AsTask().Result;
            var remove = new List<long>();
            var add = new List<Map>();
            while (i < args.Length - 1 && !args[i + 1].StartsWith("--"))
            {
                i++;
                var split = args[i].Split(',', 2);
                if (split.Length != 2 || !long.TryParse(split[0], out long from) ||
                    !long.TryParse(split[1], out long to))
                {
                    Console.Error.WriteLine($"Expected two map IDs separated by a comma, instead got '{args[i]}'");
                    continue;
                }

                var map = maps.FirstOrDefault(x => x.ID == from);
                if (map == null)
                {
                    Console.Error.WriteLine($"No map with ID {from} found");
                    continue;
                }

                remove.Add(from);
                add.Add(map);
                map.ID = to;
            }

            Console.WriteLine($"Changing IDs of {add.Count} maps");
            world.RemoveMaps(remove);
            world.AddMaps(add);
            break;
        }
        case "--import":
        {
            i++;
            if (i >= args.Length)
            {
                Console.Error.WriteLine("Expected an image file path or folder after --import");
                break;
            }

            string import = Path.GetFullPath(args[i]);
            var size = (width: 1, height: 1);
            IResampler scaling = KnownResamplers.NearestNeighbor;
            ResizeMode fill = ResizeMode.Max;
            IColorAlgorithm algorithm = new SimpleAlgorithm();
            IDither? dither = null;
            Rgba32 background = Color.Transparent;
            long? startingId = null;
            while (i < args.Length - 1)
            {
                i++;
                switch (args[i])
                {
                    case "--size":
                    {
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --size");
                            break;
                        }

                        var split = args[i].Split(',', 2);
                        if (split.Length != 2 || !int.TryParse(split[0], out int width) ||
                            !int.TryParse(split[1], out int height))
                        {
                            Console.Error.WriteLine(
                                $"Expected a width and height separated by a comma, instead got '{args[i]}'");
                            break;
                        }

                        size = (width, height);
                        break;
                    }
                    case "--scaling":
                    {
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --scaling");
                            break;
                        }

                        switch (args[i])
                        {
                            case "pixel":
                            {
                                scaling = KnownResamplers.NearestNeighbor;
                                break;
                            }
                            case "bicubic":
                            {
                                scaling = KnownResamplers.Bicubic;
                                break;
                            }
                            default:
                            {
                                Console.Error.WriteLine($"Expected either pixel or bicubic, instead got '{args[i]}'");
                                break;
                            }
                        }

                        break;
                    }
                    case "--fill":
                    {
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --fill");
                            break;
                        }

                        switch (args[i])
                        {
                            case "uniform":
                            {
                                fill = ResizeMode.Max;
                                break;
                            }
                            case "stretch":
                            {
                                fill = ResizeMode.Stretch;
                                break;
                            }
                            case "crop":
                            {
                                fill = ResizeMode.Crop;
                                break;
                            }
                            default:
                            {
                                Console.Error.WriteLine(
                                    $"Expected one of uniform, stretch, or crop, instead got '{args[i]}'");
                                break;
                            }
                        }

                        break;
                    }
                    case "--algorithm":
                    {
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --algorithm");
                            break;
                        }

                        switch (args[i])
                        {
                            case "good":
                            {
                                algorithm = new SimpleAlgorithm();
                                break;
                            }
                            case "euclidean":
                            {
                                algorithm = new EuclideanAlgorithm();
                                break;
                            }
                            case "ciede2000":
                            {
                                algorithm = new Ciede2000Algorithm();
                                break;
                            }
                            case "cie76":
                            {
                                algorithm = new Cie76Algorithm();
                                break;
                            }
                            case "cmc":
                            {
                                algorithm = new CmcAlgorithm();
                                break;
                            }
                            case "oklab":
                            {
                                algorithm = new OkLabAlgorithm();
                                break;
                            }
                            default:
                            {
                                Console.Error.WriteLine(
                                    $"Expected one of good, euclidean, ciede2000, cie76, cmc, or oklab, instead got '{args[i]}'");
                                break;
                            }
                        }

                        break;
                    }
                    case "--dithering":
                    {
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --dithering");
                            break;
                        }

                        switch (args[i])
                        {
                            case "none":
                            {
                                dither = null;
                                break;
                            }
                            case "floyd":
                            {
                                dither = KnownDitherings.FloydSteinberg;
                                break;
                            }
                            case "burks":
                            {
                                dither = KnownDitherings.Burks;
                                break;
                            }
                            default:
                            {
                                Console.Error.WriteLine(
                                    $"Expected one of none, floyd, or burks, instead got '{args[i]}'");
                                break;
                            }
                        }

                        break;
                    }
                    case "--background":
                    {
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --background");
                            break;
                        }

                        if (!Color.TryParse(args[i], out var color))
                        {
                            Console.Error.WriteLine($"Expected a color but got '{args[i]}', try using hex");
                            break;
                        }

                        background = color;
                        break;
                    }
                    case "--id":
                    {
                        i++;
                        if (i >= args.Length)
                        {
                            Console.Error.WriteLine("Expected a value after --id");
                            break;
                        }

                        if (!long.TryParse(args[i], out var id))
                        {
                            Console.Error.WriteLine($"Expected a numeric map ID, instead got '{args[i]}'");
                            break;
                        }

                        startingId = id;
                        break;
                    }
                    default:
                        goto process;
                }
            }

            process:
            var files = new List<string>();
            if (File.Exists(import))
            {
                files.Add(import);
            }
            else if (Directory.Exists(import))
            {
                var children = Directory.GetFiles(import);
                Console.WriteLine($"Found {children.Length} files in folder");
                files.AddRange(children);
            }
            else
            {
                Console.WriteLine($"Image file '{import}' not found");
            }

            foreach (var file in files)
            {
                Console.WriteLine($"Converting image {file}");
                var settings = new ImportSettings(
                    new PreviewImage(PendingSource.FromPath(file)),
                    size.width, size.height,
                    new(scaling), fill, background,
                    new ProcessSettings(dither, algorithm)
                );
                if (startingId == null)
                {
                    startingId = 0;
                    while (world.IsIdTaken(startingId.Value))
                    {
                        startingId++;
                    }

                    Console.WriteLine($"Selected automatic safe ID {startingId}");
                }

                Console.WriteLine($"Generating {settings.Width * settings.Height} maps from image");
                var batch = world.MakeMaps(settings);
                var data = ListUtils.Map2D(batch, x => new Map(startingId++.Value, x));
                var maps = ListUtils.Flatten(data).ToList();
                Console.WriteLine($"Generated map IDs: [{String.Join(", ", maps.Select(x => x.ID))}]");
                Console.WriteLine("Adding maps to world");
                world.AddMaps(maps);
                if (inventory != null)
                {
                    var structure = new StructureGrid("imagemap:" + Path.GetFileNameWithoutExtension(file), data);
                    Console.WriteLine($"Adding structure {structure.Identifier} to inventory {inventory.Name}");
                    world.AddStructures(new[] { structure }, inventory);
                }
            }

            break;
        }
        default:
        {
            Console.Error.WriteLine(
                $"Expected one of --inventory, --import, --export, --list, --delete, or --change-id, instead got '{args[i]}'");
            break;
        }
    }
}