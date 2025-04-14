using ImageStitcher;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

string[] maps = { "Chernarus (#1)", "Livonia (#2)", "Sakhal (#3)", "Namalsk (#4)", "generic-any (#5)" };
for (int i = 0; i < maps.Count(); i++)
{
    Console.WriteLine(maps[i]);
}

int number;
do
{
    Console.WriteLine("\nEnter map # from above: ");
} while (!int.TryParse(Console.ReadLine(), out number) || number <= 0 || number > maps.Count());

DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
Console.WriteLine("Searching in...." + directory.FullName);

if (directory != null)
{
    FileInfo[] files = directory.GetFiles();

    int tileCount = GetPngCount();
    int tilesPerRow = 32;
    int tileSize = 512;

    int w = (tileSize * tilesPerRow) - tileCount;
    int h = (tileSize * tilesPerRow) - tileCount;

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

        //componsate 4x4 tiles, these are scaled down if an entire tile is one material
        if (img.Width == (int)4 || img.Height == (int)4)
        {
            Bitmap original = new Bitmap(file.FullName);
            Bitmap paddedImage = new Bitmap(tileSize, tileSize);

            using (Graphics g2 = Graphics.FromImage(paddedImage))
            {
                g2.Clear(Color.Transparent);
                g2.DrawImage(original, new Point(0, 0));
            }
            img = paddedImage;
        }

        int x = 0;
        int y = 0;

        switch (number)
        {
            //Chernarus + Livonia
            case 1:
            case 2:
            case 3:
                x = idxH * (img.Width - (int)PixelOffsets.Chernarus_Livonia);
                y = idxW * (img.Height - (int)PixelOffsets.Chernarus_Livonia);
                break;

            //Namalsk
            case 4:
                x = idxH * (img.Width - (int)PixelOffsets.Namalsk);
                y = idxW * (img.Height - (int)PixelOffsets.Namalsk);
                break;

            //default any
            case 5:
                x = idxH * (img.Width - (int)PixelOffsets.Default);
                y = idxW * (img.Height - (int)PixelOffsets.Default);
                break;
        }

       
        //Draw the image at the calculated position
        g.DrawImage(img, new Point(x, y));
        idxW++;

        // If we've filled a row, reset idxH and move to the next row
        if (idxW == tilesPerRow) // There are 32 tiles per row (8192 / 256 = 32 tiles per row)
        {
            idxW = 0;
            idxH++;
        }

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

int GetPngCount()
{
    DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
    string[] pngFiles = Directory.GetFiles(directory.FullName, "*.png");
    return pngFiles.Length;
}