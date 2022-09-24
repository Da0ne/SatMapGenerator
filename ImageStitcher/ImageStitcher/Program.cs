using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

Console.WriteLine("\t\t[[Make sure your PNGs are 256x256]]\n\n");
DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
Console.WriteLine("Searching in...." + directory.FullName);
if (directory != null)
{
    FileInfo[] files = directory.GetFiles();
    
    int w = 8192; //8K
    int h = 8192; //8K

    //Calculate offset (each map tile is 512x512 orignally)
    w = w - 512;
    h = h - 512;

    Bitmap img3 = new Bitmap(w, h, PixelFormat.Format32bppArgb);
    Graphics g = Graphics.FromImage(img3);
    g.Clear(SystemColors.AppWorkspace);

    int idxW = 0;
    int idxH = 0;
    int i = 0;
    foreach (FileInfo file in files)
    {
        if (file.Extension != ".png")
            continue;

        Console.WriteLine("Processing ►►►► " + file.Name);
        Image img = Image.FromFile(file.FullName);
        if (idxW == 32) //32x32 tiled map (1024 paa's for the sat map)
        {
            idxW = 0;
            idxH++;
        }

        if (idxH == 0 && idxW == 0)
        {
            g.DrawImage(img, new Point(0, 0));
        }
        else
        {
            //Map tile image must be converted to 256x256
            //Livonia,Chernarus  offset: (has 16 pixels (256 - 16 = 240))
            g.DrawImage(img, new Point((240 * idxH), (240 * idxW)));
        }
        idxW++;
        i++;
        img.Dispose();
    }

    if (i > 0)
    {
        g.Dispose();
        string outputPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\FullMap.bmp";
        if (File.Exists(outputPath))
        {
            try
            {
                File.Delete(outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save output...unable to override/access file (Make sure FullMap.bmp is not used by another application!)");
                Console.WriteLine(ex.Message);
            }
        }

        img3.Save(outputPath, ImageFormat.Bmp);
        img3.Dispose();

        Console.Write("Full map successfully complete! Saved to: " + outputPath);
        Console.WriteLine("\nDo you wish to delete the .PNGs? (Y/N)");
        string ln = Console.ReadLine().ToLower();
        if (ln == "y")
        {
            foreach (FileInfo file in files)
            {
                if (file.Extension != ".png")
                    continue;

                if (File.Exists(file.FullName))
                    File.Delete(file.FullName);
            }
        }
    }
    else
    {
        Console.WriteLine("Couldn't find any PNGs in this directory to merge into a full map....\n");
        Console.ReadKey();
    }
}