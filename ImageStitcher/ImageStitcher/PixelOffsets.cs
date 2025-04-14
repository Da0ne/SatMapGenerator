using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStitcher
{
    public enum PixelOffsets
    {
        Chernarus_Livonia = 32, //16 if tile size is 256x256, 32 if tile size is 512x512
        Namalsk = 112, //112 - weird idk this map is weird man
        Default = 32 //assumes tiles are 512x512
    }
}
