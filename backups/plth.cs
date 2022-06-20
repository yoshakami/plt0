using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
/*
PLT0 (File Format) Made by me
```
Offset Size  Description
0x00    4    The magic "PLT0" to identify the sub file. (50 4C 54 30)
0x04    4    Length of the sub file. (internal name/ other string at the end not counted)
0x08    4    Sub file version number. ( 00 00 00 03 else crash)
0x0C    4    Offset to outer BRRES File (negative value) -> (00 00 00 00).
0x10    4    header size. (00 00 00 40)
0x14    4    String offset to the name of this sub file. relative to PLT0 start (that means if it's 0 it will point to 'PLT0'. one byte before the string is its length, so it would read the byte before 'PLT0' if the pointer was 0 )

0x18    4    Palette Format. (0 = IA8, 1 = RGB565, 2 = RGB5A3)
0x1C    2    Number of colors (can't be set to more than 256 in BrawlCrate 🤔, usually 01 00)
0x1E    34   Padding (all 00) - 34 is 0x22

v-- PLT0 Data --v
0x40   -    Data Starts
each colour is 2 bytes in length (A = Alpha, R = Red, G = Green, B = Blue)

for the palette RGB5A3, it's 0AAA RRRR   GGGG BBBB  (colour values can only be multiple of 16)
or, alternatively it can be  1RRR RRGG   GGGB BBBB
A: 15 R:255 G:3 B:227 is     0000 1111   0000 1101
A: 0  R:255 G:0 B:221 is what Brawlcrate encodes (such a bad encoding to be honest 15 is nearest from 16 than from 0)

for the palette IA8, it's  AAAA AAAA   CCCC CCCC (Alpha and Colour can be any value between 0 and 255)
A: 15 R:127 G:127 B:127 is 0000 0111   0111 1111

for the palette RGB565, it's RRRR RGGG   GGGB BBBB (alpha always 255, and yes there are 6 G lol)
R: 200 G:60 B:69 would be    1100 0001   1110 1000
R: 197 G:61 B:66 is displayed by BrawlCrate and R:192 G:60 B:64 is what's really encoded (by Brawlcrate). it looks like Brawlcrate is faulty lol
```



TEX0 (File Format) written by me again, the header on wiki tockdom is false. The texture data is correct.
```
Offset Size  Description
0x00    4    The magic "TEX0" to identify the sub file. (54 45 58 30)
0x04    4    Length of the sub file. (internal name/ other string at the end not counted)
0x08    4    Sub file version number. ( 00 00 00 03 else crash)
0x0C    4    Offset to outer BRRES File (negative value) -> (00 00 00 00).
0x10    4    header size. (00 00 00 40)
0x14    4    String offset to the name of this sub file. relative to PLT0 start (that means if it's 0 it will point to 'PLT0'. one byte before the string is its length, so it would read the byte before 'PLT0' if the pointer was 0 )
0x18    4    00 00 00 01 if the image has a palette (a PLT0 file) else 00 00 00 00
0x1C    2    texture width
0x1E    2    texture height
0x20    4    texture format
0x24    4    Number of images (0 mipmap = 1 image, 1 mipmap = 2 images)
0x28    4    unknown
0x2C    4    number of mipmaps as hex float
0x30    16   Padding (all 00) - 16 is 0x10

v-- TEX0 Data --v
0x40   -    Data Starts
https://wiki.tockdom.com/wiki/TEX0_(File_Format)
```
 

TPL (File Format)
```
Offset Size  Description
0x00    4    The magic "TPL0" to identify the sub file. (00 20 AF 30)
0x04    4    Number of images
0x08    4    Offset of the Image Table. (00 00 00 0C)

v-- Image Table --v
0x0C    4    Offset of image#0 header
0x10    4    Offset of palette#0 header
0x14    4    Offset of image#1 header
0x18    4    Offset of palette#1 header
...

v-- Palette Header --v
0x00    2    Entry Count (number of colors)
0x02    1    Unpacked  ( 00 I guess)
0x03    1    1 byte of padding
0x04    4    Palette Format
0x08    4    Palette Data Address

v-- Palette Data --v
this is the same palette data as a PLT0

v-- Image Header --v
0x00    2    Height
0x02    2    Width
0x04    4    Texture Format
0x08    4    Image Data Address
0x0C    4    WrapS ( 0 = clamp, 01 = repeat, 02 = mirror)
0x10    4    WrapT (vertical wrap)
0x14    4    MinFilter (0 = nearest neighbour, 1 = linear)
0x18    4    MagFilter (magnification, used when upscaling)
0x1C    4    LODBias (hex float) (00 00 00 00)
0x20    1    EdgeLODEnable (00)
0x21    1    MinLOD (00)
0x22    1    MaxLOD (number of mipmaps)
0x23    1    Unpacked (00)
https://wiki.tockdom.com/wiki/TPL_(File_Format)
```

 
 
 
 */

namespace plt0
{
    internal static class plt0
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            plt0_class main_class = new plt0_class();
            main_class.parse_args();
        }
    }
    public class IntArrayComparer : IComparer<int[]>
    {
        public int Compare(int[] ba, int[] bb)
        {
            int n = ba.Length;  //fetch the length of the first array
            int ci = n.CompareTo(bb.Length); //compare to the second
            if (ci != 0)
            { //if not equal return the compare result
                return ci;
            }
            else
            { //else elementwise comparer
                for (int i = 0; i < n; i++)
                {
                    if (ba[i] != bb[i])
                    { //if not equal element, return compare result
                        return bb[i].CompareTo(ba[i]);
                    }
                }
                return 0; //if all equal, return 0
            }
        }
    }
    class plt0_class
    {
        byte[] texture_format_int32 = { 0, 0, 0, 8 };  // 8 = CI4   9 = CI8    10 = CI14x2
        byte[] palette_format_int32 = { 0, 0, 0, 9 };  // 0 = AI8   1 = RGB565  2 = RGB5A3
        byte[] colour_palette;
        byte mipmaps_number = 0;
        byte minificaction_filter = 1;  // 0 = Nearest Neighbour   1 = Linear
        byte magnification_filter = 1;  // 0 = Nearest Neighbour   1 = Linear
        byte WrapS = 1; // 0 = Clamp   1 = Repeat   2 = Mirror
        byte WrapT = 1; // 0 = Clamp   1 = Repeat   2 = Mirror
        byte alpha = 9;  // 0 = no alpha - 1 = alpha - 2 = mix 
        byte diversity = 10;
        // bool default_diversity = true;
        byte diversity2 = 0;
        // bool default_diversity2 = true;
        byte algorithm = 0;  // 0 = CIE 601    1 = CIE 709     2 = custom RGBA
        byte pass = 0;
        byte bmd = 0;
        sbyte block_width = -1;
        sbyte block_height = -1;
        bool warn = false;
        bool grey = true;
        bool bti = false;
        bool tpl = false;
        bool success;
        bool fill_height = false;
        bool fill_width = false;
        int pixel_count;
        ushort bitmap_width;
        ushort bitmap_height;
        ushort colour_number = 0;
        ushort colour_number_x2;
        ushort max_colours = 0;
        string input_file = "";
        string output_file = "";
        float[] custom_rgba = { 1, 1, 1, 1 };
        double percentage = 0;
        double percentage2 = 0;
        double format_ratio = 1;

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void parse_args()
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i++)
            {
                if (pass > 0)
                {
                    pass--;
                    continue;
                }
                for (int j = 0; j < 10;)
                {
                    if (args[i][j] == '-')
                    {
                        args[i] = args[i].Substring(1, args[i].Length - 1);
                    }
                    else
                    {
                        break;
                    }
                }  // who had the stupid idea to add -- before each argument. I'm removing them all lol
                switch (args[i].ToUpper())
                {
                    case "IA8":
                    case "AI8":
                        {
                            palette_format_int32[3] = 0;
                            break;
                        }
                    case "RGB565":
                    case "RGB":
                        {
                            palette_format_int32[3] = 1;
                            break;
                        }
                    case "RGB5A3":
                    case "RGBA":
                        {
                            palette_format_int32[3] = 2;
                            break;
                        }
                    case "C4":
                    case "CI4":
                        {
                            max_colours = 16;
                            block_width = 8;
                            block_height = 8;
                            format_ratio = 2;
                            break;
                        }
                    case "C8":
                    case "CI8":
                        {
                            max_colours = 256;
                            block_width = 8;
                            block_height = 4;
                            texture_format_int32[3] = 9;
                            break;
                        }
                    case "CI14X2":
                    case "C14X2":
                        {
                            max_colours = 16385;  // the user can still bypass this number by manually specifiying the number of colours
                            // max_colours = 65535;
                            block_width = 4;
                            block_height = 4;
                            texture_format_int32[3] = 10;
                            format_ratio = 0.5;
                            break;
                        }
                    case "G2":
                        {
                            algorithm = 1;
                            break;
                        }
                    case "CC":
                        {
                            algorithm = 2;
                            float.TryParse(args[i + 1], out custom_rgba[0]);
                            float.TryParse(args[i + 2], out custom_rgba[1]);
                            float.TryParse(args[i + 3], out custom_rgba[2]);
                            float.TryParse(args[i + 4], out custom_rgba[3]);
                            pass = 4;
                            break;
                        }
                    case "ALPHA":
                    case "1BIT":
                    case "1-BIT":
                        {
                            success = byte.TryParse(args[i + 1], out alpha);
                            if (!success)
                            {
                                alpha = 1;
                            }
                            break;
                        }
                    case "BMD":
                        {
                            bmd = 1;
                            break;
                        }
                    case "BTI":
                        {
                            bti = true;
                            break;
                        }
                    case "C":
                        {
                            ushort.TryParse(args[i + 1], out colour_number); // colour_number is now a number 
                            colour_number_x2 = (ushort)(colour_number * 2);
                            pass = 1;
                            break;
                        }
                    case "D":
                    case "DIVERSITY":
                        {
                            // default_diversity = false;
                            byte.TryParse(args[i + 1], out diversity); // diversity is now a number 
                            pass = 1;
                            break;
                        }
                    case "D2":
                    case "DIVERSITY2":
                        {
                            // default_diversity2 = false;
                            byte.TryParse(args[i + 1], out diversity2); // diversity2 is now a number 
                            pass = 1;
                            break;
                        }
                    case "M":
                    case "N-MIPMAPS":
                    case "N-MM":
                        {
                            byte.TryParse(args[i + 1], out mipmaps_number); // mipmaps_number is now a number 
                            pass = 1;
                            break;
                        }
                    case "MIN":
                        {
                            switch (args[i + 1].ToLower())
                            {
                                case "NN":
                                case "NEAREST":
                                case "0":
                                    {
                                        minificaction_filter = 0;
                                        break;
                                    }
                                case "LINEAR":
                                case "1":
                                    {
                                        minificaction_filter = 1;
                                        break;
                                    }
                                case "nearestmipmapnearest":
                                case "2":

                                    {
                                        minificaction_filter = 2;
                                        break;
                                    }
                                case "nearestmipmaplinear":
                                case "3":
                                    {
                                        minificaction_filter = 3;
                                        break;
                                    }
                                case "linearmipmapnearest":
                                case "4":
                                    {
                                        minificaction_filter = 4;
                                        break;
                                    }
                                case "linearmipmaplinear":
                                case "5":
                                    {
                                        minificaction_filter = 5;
                                        break;
                                    }
                            }
                            pass = 1;
                            break;
                        }
                    case "MAG":
                        {
                            switch (args[i + 1].ToLower())
                            {
                                case "NN":
                                case "NEAREST":
                                case "0":
                                    {
                                        magnification_filter = 0;
                                        break;
                                    }
                                case "LINEAR":
                                case "1":
                                    {
                                        magnification_filter = 1;
                                        break;
                                    }
                                case "nearestmipmapnearest":
                                case "2":

                                    {
                                        magnification_filter = 2;
                                        break;
                                    }
                                case "nearestmipmaplinear":
                                case "3":
                                    {
                                        magnification_filter = 3;
                                        break;
                                    }
                                case "linearmipmapnearest":
                                case "4":
                                    {
                                        magnification_filter = 4;
                                        break;
                                    }
                                case "linearmipmaplinear":
                                case "5":
                                    {
                                        magnification_filter = 5;
                                        break;
                                    }
                            }
                            pass = 1;
                            break;
                        }
                    case "MIX":
                    case "MIXED":
                    case "BOTH":
                    case "8-BIT":
                    case "8BIT":
                        {
                            alpha = 2;
                            break;
                        }
                    case "NN":
                    case "NEAREST":
                    case "NEIGHBOUR": // bri'ish
                    case "NEIGHBOR": // ricain
                        {
                            minificaction_filter = 0;
                            magnification_filter = 0;
                            break;
                        }
                    case "LINEAR":
                        {
                            minificaction_filter = 1;
                            magnification_filter = 1;
                            break;
                        }
                    case "NEARESTMIPMAPNEAREST":
                        {
                            minificaction_filter = 2;
                            magnification_filter = 2;
                            break;
                        }
                    case "NEARESTMIPMAPLINEAR":
                        {
                            minificaction_filter = 3;
                            magnification_filter = 3;
                            break;
                        }
                    case "LINEARMIPMAPNEAREST":
                        {
                            minificaction_filter = 4;
                            magnification_filter = 4;
                            break;
                        }
                    case "LINEARMIPMAPLINEAR":
                        {
                            minificaction_filter = 5;
                            magnification_filter = 5;
                            break;
                        }
                    case "NA":
                    case "NOALPHA":
                    case "NO ALPHA":
                        {
                            alpha = 0;
                            break;
                        }
                    case "NO":
                        {
                            if (args[i + 1].ToUpper() == "ALPHA")
                            {
                                alpha = 0;
                            }
                            pass = 1;
                            break;
                        }
                    case "P":
                    case "PERCENTAGE":
                    case "THRESHOLD":
                        {
                            double.TryParse(args[i + 1], out percentage);  // percentage is now a double
                            pass = 1;
                            break;
                        }
                    case "P2":
                    case "PERCENTAGE2":
                    case "THRESHOLD2":
                        {
                            double.TryParse(args[i + 1], out percentage2);  // percentage2 is now a double
                            pass = 1;
                            break;
                        }
                    case "TPL":
                        {
                            tpl = true;
                            break;
                        }
                    case "WARN":
                    case "W":
                        {
                            warn = true;
                            break;
                        }
                    case "WRAP":
                        {
                            switch (args[i + 1].ToUpper().Substring(0, 1))
                            {
                                case "C": // clamp
                                    {
                                        WrapS = 0;
                                        break;
                                    }
                                case "R": // repeat
                                    {
                                        WrapS = 1;
                                        break;
                                    }
                                case "M": // mirror
                                    {
                                        WrapS = 2;
                                        break;
                                    }
                            }
                            switch (args[i + 2].ToUpper().Substring(0, 1))
                            {
                                case "C": // clamp
                                    {
                                        WrapT = 0;
                                        break;
                                    }
                                case "R": // repeat
                                    {
                                        WrapT = 1;
                                        break;
                                    }
                                case "M": // mirror
                                    {
                                        WrapT = 2;
                                        break;
                                    }
                            }
                            pass = 2;
                            break;
                        }

                    default:
                        {
                            if (System.IO.File.Exists(args[i]) && input_file == "")
                            {
                                input_file = args[i];
                            }
                            else
                            {
                                output_file = args[i].Substring(0, args[i].Length - args[i].Split('.')[args[i].Split('.').Length - 1].Length - 1);
                            }
                            break;
                        }
                }

            }
            /*
            if (default_diversity)  // change this and add default diversity 2
            {
                pass = 0;
                for (int i = 0; i < 400; i += 50)
                {
                    if (colour_number < i)
                    {
                        break;
                    }
                    diversity = (byte)(60 - pass);
                    pass += 10;
                }
            }*/
            if (input_file == "")
            {
                Console.WriteLine("no input file specified\nusage: PLT0 [tpl|bti|bmd|bmp|png] [Encoding Format] [Palette Format] [alpha|no alpha|mix] [c colour number] [d diversity] [d2 diversity2] [m mipmaps] [p percentage] [p2 percentage2] [g2|cc 0.7 0.369 0.4 1.0] [warn|w] [min nearest neighbour] [mag linear] [Wrap Clamp Clamp] <file> [dest]\nthis is the usage format for parameters : [optional] <mandatory>\n\nif you don't specify tpl, this program will output by default a tex0 and a plt0 file.\nAvailable Encoding Formats: C4, CI4, C8, CI8, CI14x2, C14x2, default = CI8\nAvailable Palette Formats : IA8 (Black and White), RGB565 (RGB), RGB5A3 (RGBA), default = (auto)\n\nif the palette chosen is RGB5A3, you can force the image to have no alpha for all colours (so the RGB values will be stored on 5 bits each), or force all colours to use an alpha channel, by default it's set on 'mix'\n\nthe number of colours is stored on 2 bytes, but the index bit length of the colour encoding format limits the max number of colours to these:\nCI4 : from 0 to 16\nCI8 : from 0 to 256\nCI14x2 : from 0 to 16384\n\n-d | -diversity | d | diversity = a minimum amount of difference for at least one colour channel in the colour palette\n(prevents a palette from having colours close to each other) default : 16 colours -> diversity = 60 | 256 colours -> diversity = 20\nmust be between 0 and 255, if the program heaven't filled the colour table with this parameter on, it'll fill it again with diversity2, then it'll let you know and fill it with the most used colours\n\n-d2 | -diversity2 | d2 | diversity2 = the diversity parameter for the second loop that fills the colour table if it's not full after the first loop.\n\n-m | --n-mm | m | --n-mipmaps = mipmaps number (number of duplicates versions of lower resolution of the input image stored in the output texture) default = 0\n\n-p | p | percentage | threshold = a double (64 bit float) between zero and 100. Alongside the diversity setting, if a colour counts for less than p % of the whole number of pixels, then it won't be added to the palette. default = 0 (0%)\n-p2 | p2 | percentage2 | threshold2 = the minimum percentage for the second loop that fills the colour table. default = 0 (0%)\nadd -w or warn to make the program ask to you before overwriting the output file\n\ntype g2 to use grayscale recommendation CIE 709, the default one used is the recommendation CIE 601.\ntype cc then your float RGBA factors to multiply every pixel of the input image by this factor\n the order of parameters doesn't matter, though you must add a number after '-m', '-c' or '-d',\nmin or mag filters are used to choose the algorithm for downscaling/upscaling textures in a tpl file : (nearest neighbour, linear, NearestMipmapNearest, NearestMipmapLinear, LinearMipmapNearest, LinearMipmapLinear) you can just type nn or nearest instead of nearest neighbour or the filter number.\nwrap X Y is the syntax to define the wrap mode for both horizontal and vertical within the same option. X and Y can have these values (clamp, repeat, mirror) or just type the first letter. \nplease note that if a parameter corresponds to nothing described above and doesn't exists, it'll be taken as the output file name.\n\nExamples:\nplt0 rosalina.png -d 0 -x ci8 rgb5a3 --n-mipmaps 5 -w -c 256 (output name will be '-x.plt0' and '-x.tex0', as this is not an existing option)\nplt0 tpl ci4 rosalina.jpeg AI8 c 4 d 16 g2 m 1 warn texture.tex0");
                return;
            }
            if (output_file == "")
            {
                output_file = input_file.Substring(0, input_file.Length - input_file.Split('.')[input_file.Split('.').Length - 1].Length - 1);
            }
            if (colour_number > max_colours && max_colours == 16)
            {
                Console.WriteLine("CI4 can only supports up to 16 colours as each pixel index is stored on 4 bits");
                return;
            }
            if (colour_number > max_colours && max_colours == 256)
            {
                Console.WriteLine("CI8 can only supports up to 256 colours as each pixel index is stored on 8 bits");
                return;
            }
            if (colour_number > 65535 && max_colours == 16385)
            {
                Console.WriteLine("Colour number is stored on 2 bytes for CI14x2");
                return;
            }
            if (minificaction_filter == 0 && mipmaps_number > 0)
            {
                minificaction_filter = 2; // Nearest becomes "NearestMipmapNearest", if you don't want that then why the heck are you adding mipmaps to your file if you're not going to use them
            }
            if (minificaction_filter == 1 && mipmaps_number > 0)
            {
                minificaction_filter = 5; // Linear becomes "LinearMipmapLinear"
            }
            if (magnification_filter == 0 && mipmaps_number > 0)
            {
                magnification_filter = 2;
            }
            if (magnification_filter == 1 && mipmaps_number > 0)
            {
                magnification_filter = 5;
            }
            if (max_colours == 0)  // if user haven't chosen a texture format
            {
                max_colours = 256;  // let's set CI8 as default
                block_width = 8;
                block_height = 4;
                texture_format_int32[3] = 9;
            }
            if (colour_number == 0)
            {
                colour_number = max_colours;
                colour_number_x2 = (ushort)(colour_number * 2);
            }
            try
            {
                byte[] bmp_image = Convert_to_bmp((Bitmap)Bitmap.FromFile(input_file));
                /* if (colour_number > max_colours && max_colours == 16385)
            {
                Console.WriteLine("CI14x2 can only supports up to 16385 colours as each pixel index is stored on 14 bits");
                stay = false;
            }*/

                if (bmp_image[0x15] != 0 || bmp_image[0x14] != 0 || bmp_image[0x19] != 0 || bmp_image[0x18] != 0)
                {
                    Console.WriteLine("Textures Dimensions are too high for a TEX0. Maximum dimensions are the values of a 2 bytes integer (65535 x 65535)");
                    return;
                }
                /***** BMP File Process *****/
                // process the bmp file
                int bmp_filesize = bmp_image[2] | bmp_image[3] << 8 | bmp_image[4] << 16 | bmp_image[5] << 24;
                int pixel_data_start_offset = bmp_image[10] | bmp_image[11] << 8 | bmp_image[12] << 16 | bmp_image[13] << 24;
                bitmap_width = (ushort)(bmp_image[0x13] << 8 | bmp_image[0x12]);
                bitmap_height = (ushort)(bmp_image[0x17] << 8 | bmp_image[0x16]);
                pixel_count = bitmap_width * bitmap_height;
                if (palette_format_int32[3] == 9)  // if a colour palette hasn't been selected by the user, this program will set it automatically to the most fitting one
                {
                    for (int i = 0; i < bitmap_width; i += 4)
                    {
                        if (bmp_image[pixel_data_start_offset + i + 3] != 0xff && alpha > 1)
                        {
                            alpha = 2;
                        }
                        if (bmp_image[pixel_data_start_offset + i] != bmp_image[pixel_data_start_offset + i + 1] || bmp_image[pixel_data_start_offset + i] == bmp_image[pixel_data_start_offset + i + 2])
                        {
                            grey = false;
                        }
                        if (alpha < 3 && !grey)
                        {
                            break;
                        }
                    }
                    for (int i = bitmap_width * (bitmap_height / 2) + 1; i % bitmap_width != 0; i += 4)
                    {
                        if (bmp_image[pixel_data_start_offset + i + 3] != 0xff && alpha > 1)
                        {
                            alpha = 2;
                        }
                        if (bmp_image[pixel_data_start_offset + i] != bmp_image[pixel_data_start_offset + i + 1] || bmp_image[pixel_data_start_offset + i] == bmp_image[pixel_data_start_offset + i + 2])
                        {
                            grey = false;
                        }
                        if (alpha < 3 && !grey)
                        {
                            break;
                        }
                    }
                    if (grey)
                    {
                        palette_format_int32[3] = 0;  // AI8
                    }
                    else if (alpha != 9)
                    {
                        palette_format_int32[3] = 2;  // RGB5A3
                    }
                    else
                    {
                        palette_format_int32[3] = 1;  // RGB565
                    }
                }
                ushort[] vanilla_size = { bitmap_width, bitmap_height };
                Array.Resize(ref colour_palette, colour_number_x2);
                List<List<byte[]>> index_list = new List<List<byte[]>>();
                object v = create_PLT0(bmp_image, bmp_filesize, pixel_data_start_offset);
                index_list.Add((List<byte[]>)v);
                for (int i = 1; i < mipmaps_number; i++)
                {

                    using (var ms = new MemoryStream())
                    {
                        ms.Read(bmp_image, 0, bmp_image.Length);
                        Bitmap image = (Bitmap)Bitmap.FromStream(ms);
                        image.Save(ms, ImageFormat.Bmp);
                        bitmap_width = (ushort)(bitmap_width / Math.Pow(i, 2));
                        bitmap_height = (ushort)(bitmap_height / Math.Pow(i, 2));
                        image = ResizeImage(image, bitmap_width, bitmap_height);
                        bmp_image = Convert_to_bmp((Bitmap)image);
                    }
                    v = build_mipmap(bmp_image, bmp_filesize, pixel_data_start_offset);
                    index_list.Add((List<byte[]>)v);
                }
                bitmap_width = vanilla_size[0];
                bitmap_height = vanilla_size[1];
                if (bti)
                {
                    write_BTI(index_list);
                }
                else if (tpl)
                {
                    write_TPL(index_list);
                }
                else
                {
                    write_PLT0();
                    write_TEX0(index_list);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Out of memory." && ex.Source == "System.Drawing")
                    Console.WriteLine("Image input format not supported (convert it to jpeg or png)");
                else
                    throw ex;
                return;
            }
            
        }
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void write_BTI(List<List<byte[]>> index_list)  // index_list contains all mipmaps.
        {
            int size = 0x20 + colour_palette.Length + 0x40; // fixed size at 1 image
            double temp;
            int[] param = new int[4];
            List<int[]> settings = new List<int[]>();
            for (int i = 0; i < mipmaps_number + 1; i++)
            {
                if (i == 0)
                {
                    param[2] = (int)(index_list[i][0].Length * format_ratio);
                    param[3] = index_list[i].Count;
                }
                else
                {
                    temp = bitmap_width / Math.Pow(i, 2);
                    if (temp % 1 != 0)
                    {
                        param[2] = (int)(index_list[i][0].Length * format_ratio) + 1;
                        // param[2] = (int)temp + 1;
                    }
                    else
                    {
                        // param[2] = (int)temp;
                        param[2] = (int)(index_list[i][0].Length * format_ratio);
                    }
                    temp = bitmap_height / Math.Pow(i, 2);
                    if (temp % 1 != 0)
                    {
                        // param[3] = (int)temp + 1;
                        param[3] = index_list[i].Count;
                    }
                    else
                    {
                        // param[3] = (int)temp;
                        param[3] = index_list[i].Count;
                    }
                }
                temp = param[2] / block_width;
                if (temp % 1 != 0)
                {
                    param[0] = (int)temp + 1;
                }
                else
                {
                    param[0] = (int)temp;
                }
                temp = param[3] / block_height;
                if (temp % 1 != 0)
                {
                    param[1] = (int)temp + 1;
                }
                else
                {
                    param[1] = (int)temp;
                }
                settings.Add(param.ToArray());
                // size += param[0] * block_width * param[1] * block_height;
                size += index_list[i][0].Length * index_list[i].Count;
            }
            byte size2 = (byte)(4 + Math.Abs(16 - size) % 16);
            byte len = (byte)output_file.Split('\\').Length;
            string file_name = (output_file.Split('\\')[len - 1]);
            byte[] data2 = new byte[size2 + len + (16 - len) % 16];
            byte[] data = new byte[32];  // header data
            for (int i = 0; i < size2; i++)
            {
                data2[i] = 0;
            }
            for (int i = 0; i < file_name.Length; i++)
            {
                data2[i + size2] = (byte)file_name[i];
            }
            for (int i = size2 + file_name.Length; i < data2.Length; i++)
            {
                data2[i] = 0;
            }
            data[0] = texture_format_int32[3];  // image format, pretty straightforward it isn't an int lol
            data[1] = alpha;
            data[2] = (byte)(settings[0][2] >> 8);  // unsigned short width
            data[3] = (byte)settings[0][2]; // second byte of width
            data[4] = (byte)(settings[0][3] >> 8);  // height
            data[5] = (byte)settings[0][3];
            data[6] = WrapS; // sideways wrap
            data[7] = WrapT; // vertical wrap
            data[8] = 1; // well, 1 means palette enabled, and the whole purpose of this tool is to make images with palette LMAO
            data[9] = palette_format_int32[3]; // pretty straightforward again
            data[10] = (byte)(colour_number / 256);
            data[11] = (byte)(colour_number);  // number of colours
            data[12] = 0;
            data[13] = 0;
            data[14] = 0;
            data[15] = 32; // palette data address
            if (mipmaps_number > 0)
            {
                data[16] = 1;
            }
            else
            {
                data[16] = 0;
            }
            data[17] = 0;  // EdgeLOD (bool)
            data[18] = 0;  // BiasClamp (bool)
            data[19] = 0;  // MaxAniso (byte)
            data[20] = minificaction_filter;
            data[21] = magnification_filter;
            data[22] = 0; // MinLOD
            data[23] = (byte)(mipmaps_number * 8);  // MaxLOD
            data[24] = (byte)(mipmaps_number + 1);  // number of images
            data[25] = 0x69; // my signature XDDDD, I couldn't figure out what this setting does, but it doesn't affect the gameplay at all, it looks like it is used as a version number.
            data[26] = 0; // how do I calculate LODBIAS
            data[27] = 0; // how do I calculate LODBIAS
            data[28] = 0;  // won't exceed the max number of colours in the colour table which is stored on 2 bytes + header size 0x20
            data[29] = (byte)((0x20 + colour_palette.Length) / 65536);
            data[30] = (byte)((0x20 + colour_palette.Length) / 256);
            data[31] = (byte)(0x20 + colour_palette.Length);  // offset to image0 header, (byte) acts as %256
            // now's palette data, but it's already stored in colour_palette, so let's jump onto image data.
            byte[] tex_data = new byte[size - 0x20 - colour_palette.Length];
            int count = 0;
            int height;
            int width;
            block_width = (sbyte)(block_width / format_ratio);
            for (int i = 0; i < settings.Count; i++)  // mipmaps
            {
                height = settings[i][1] * block_height - 1;
                width = 0;
                for (int j = settings[i][1]; j > 0; j--)  // number of height blocks  (remember that the last line was added first)
                {
                    for (int k = 0; k < settings[i][0]; k++)  // number of width blocks
                    {
                        for (int h = 0; h < block_height; h++)  // height in the block
                        {
                            for (int w = 0; w < block_width; w++)  // width in the block
                            {
                                tex_data[count] = index_list[i][height - h][width + w];
                                count++;
                            }
                        }
                        width += block_width;
                    } // end of the loop to go through number of horizontal blocks
                    height -= block_height;
                    width = 0;
                }   // go through vertical blocks
            }      // go through mipmaps
            FileMode mode = System.IO.FileMode.CreateNew;
            if (System.IO.File.Exists(output_file + ".bti"))
            {
                mode = System.IO.FileMode.Truncate;
                if (warn)
                {
                    Console.WriteLine("Press enter to overwrite " + output_file + ".bti");
                    Console.ReadLine();
                }
            }
            using (System.IO.FileStream file = System.IO.File.Open(output_file + ".bti", mode, System.IO.FileAccess.Write))
            {
                file.Write(data, 0, 32);
                file.Write(colour_palette, 0, colour_palette.Length);
                file.Write(tex_data, 0, tex_data.Length);
                file.Write(data2, 0, data2.Length);
                file.Close();
                Console.WriteLine(output_file + ".bti");
            }
        }
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void write_TPL(List<List<byte[]>> index_list)  // index_list contains all mipmaps.
        {
            int size = 0x20 + colour_palette.Length + 0x40; // fixed size at 1 image
            double temp;
            int[] param = new int[4];
            List<int[]> settings = new List<int[]>();
            for (int i = 0; i < mipmaps_number + 1; i++)
            {
                if (i == 0)
                {
                    param[2] = (int)(index_list[i][0].Length * format_ratio);
                    param[3] = index_list[i].Count;
                }
                else
                {
                    temp = bitmap_width / Math.Pow(i, 2);
                    if (temp % 1 != 0)
                    {
                        param[2] = (int)(index_list[i][0].Length * format_ratio) + 1;
                        // param[2] = (int)temp + 1;
                    }
                    else
                    {
                        // param[2] = (int)temp;
                        param[2] = (int)(index_list[i][0].Length * format_ratio);
                    }
                    temp = bitmap_height / Math.Pow(i, 2);
                    if (temp % 1 != 0)
                    {
                        // param[3] = (int)temp + 1;
                        param[3] = index_list[i].Count;
                    }
                    else
                    {
                        // param[3] = (int)temp;
                        param[3] = index_list[i].Count;
                    }
                }
                temp = param[2] / block_width;
                if (temp % 1 != 0)
                {
                    param[0] = (int)temp + 1;
                }
                else
                {
                    param[0] = (int)temp;
                }
                temp = param[3] / block_height;
                if (temp % 1 != 0)
                {
                    param[1] = (int)temp + 1;
                }
                else
                {
                    param[1] = (int)temp;
                }
                settings.Add(param.ToArray());
                // size += param[0] * block_width * param[1] * block_height;
                size += index_list[i][0].Length * index_list[i].Count;
            }
            byte size2 = (byte)(4 + Math.Abs(16 - size) % 16);
            byte len = (byte)output_file.Split('\\').Length;
            string file_name = (output_file.Split('\\')[len - 1]);
            byte[] data2 = new byte[size2 + len + (16 - len) % 16];
            byte[] data = new byte[32];  // header data
            byte[] header = new byte[64];  // image header data
            for (int i = 0; i < size2; i++)
            {
                data2[i] = 0;
            }
            for (int i = 0; i < file_name.Length; i++)
            {
                data2[i + size2] = (byte)file_name[i];
            }
            for (int i = size2 + file_name.Length; i < data2.Length; i++)
            {
                data2[i] = 0;
            }
            data[0] = 0;
            data[1] = 0x20;
            data[2] = 0xAF;
            data[3] = 0x30;  // file identifier
            data[4] = 0;
            data[5] = 0;
            data[6] = 0;
            data[7] = 1;  // number of images
            data[8] = 0;
            data[9] = 0;
            data[10] = 0;
            data[11] = 0x0C; // offset to image table
            data[12] = 0;  // won't exceed the max number of colours in the colour table which is stored on 2 bytes + header size 0x20
            data[13] = (byte)((0x20 + colour_palette.Length) / 65536);
            data[14] = (byte)((0x20 + colour_palette.Length) / 256);
            data[15] = (byte)(0x20 + colour_palette.Length);  // offset to image0 header, (byte) acts as %256
            data[16] = 0;
            data[17] = 0;
            data[18] = 0;
            data[19] = 20;  // offset to palette0 header
            data[20] = (byte)(colour_number / 256);
            data[21] = (byte)(colour_number);  // number of colours
            data[22] = 0;  // unpacked (0 I guess)
            data[23] = 0;  // padding
            data[24] = palette_format_int32[0];
            data[25] = palette_format_int32[1];
            data[26] = palette_format_int32[2];
            data[27] = palette_format_int32[3]; // palette format
            data[28] = 0;
            data[29] = 0;
            data[30] = 0;
            data[31] = 32; // palette data address
            // now's palette data, but it's already stored in colour_palette, so let's jump onto image header.
            header[0] = (byte)(settings[0][2] >> 8);  // unsigned short width
            header[1] = (byte)settings[0][2]; // second byte of width
            header[2] = (byte)(settings[0][3] >> 8);  // height
            header[3] = (byte)settings[0][3];
            header[4] = texture_format_int32[0];
            header[5] = texture_format_int32[1];
            header[6] = texture_format_int32[2];
            header[7] = texture_format_int32[3];
            header[8] = 0; // won't exceed the max number of colours in the colour table which is stored on 2 bytes + header size 0x20
            header[9] = (byte)((0x60 + colour_palette.Length) / 65536);
            header[10] = (byte)((0x60 + colour_palette.Length) / 256);
            header[11] = (byte)(0x60 + colour_palette.Length);
            header[12] = 0;
            header[13] = 0;
            header[14] = 0;
            header[15] = WrapS;
            header[16] = 0;
            header[17] = 0;
            header[18] = 0;
            header[19] = WrapT;
            header[20] = 0;
            header[21] = 0;
            header[22] = 0;
            header[23] = minificaction_filter;
            header[24] = 0;
            header[25] = 0;
            header[26] = 0;
            header[27] = magnification_filter;
            header[28] = 0;
            header[29] = 0;
            header[30] = 0;
            header[31] = 0; // LODBias
            header[32] = 0; // EdgeLODEnable
            header[33] = 0; // MinLOD
            header[34] = mipmaps_number; // MaxLOD
            header[35] = 0; // unpacked
            for (int i = 36; i < 64; i++)  // nintendo does this on their tpl. I guess it's for a good reason
            {
                header[i] = 0;
            }
            byte[] tex_data = new byte[size - 0x60 - colour_palette.Length];
            int count = 0;
            int height;
            int width;
            block_width = (sbyte)(block_width / format_ratio);
            for (int i = 0; i < settings.Count; i++)  // mipmaps
            {
                height = settings[i][1] * block_height - 1;
                width = 0;
                for (int j = settings[i][1]; j > 0; j--)  // number of height blocks  (remember that the last line was added first)
                {
                    for (int k = 0; k < settings[i][0]; k++)  // number of width blocks
                    {
                        for (int h = 0; h < block_height; h++)  // height in the block
                        {
                            for (int w = 0; w < block_width; w++)  // width in the block
                            {
                                tex_data[count] = index_list[i][height - h][width + w];
                                count++;
                            }
                        }
                        width += block_width;
                    } // end of the loop to go through number of horizontal blocks
                    height -= block_height;
                    width = 0;
                }   // go through vertical blocks
            }      // go through mipmaps
            FileMode mode = System.IO.FileMode.CreateNew;
            if (System.IO.File.Exists(output_file + ".tpl"))
            {
                mode = System.IO.FileMode.Truncate;
                if (warn)
                {
                    Console.WriteLine("Press enter to overwrite " + output_file + ".tpl");
                    Console.ReadLine();
                }
            }
            using (System.IO.FileStream file = System.IO.File.Open(output_file + ".tpl", mode, System.IO.FileAccess.Write))
            {
                file.Write(data, 0, 32);
                file.Write(colour_palette, 0, colour_palette.Length);
                file.Write(header, 0, 64);
                file.Write(tex_data, 0, tex_data.Length);
                file.Write(data2, 0, data2.Length);
                file.Close();
                Console.WriteLine(output_file + ".tpl");
            }
        }
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void write_PLT0()
        {
            int size = 0x40 + colour_palette.Length;
            byte size2 = (byte)(4 + Math.Abs(16 - size) % 16);
            byte len = (byte)output_file.Split('\\').Length;
            string file_name = (output_file.Split('\\')[len - 1]);
            byte[] data2 = new byte[size2 + len + ((16 - len) % 16)];
            byte[] data = new byte[64];  // header data
            for (int i = 0; i < size2; i++)
            {
                data2[i] = 0;
            }
            for (int i = 0; i < file_name.Length; i++)
            {
                data2[i + size2] = (byte)file_name[i];
            }
            for (int i = size2 + file_name.Length; i < data2.Length; i++)
            {
                data2[i] = 0;
            }
            data[0] = (byte)'P';
            data[1] = (byte)'L';
            data[2] = (byte)'T';
            data[3] = (byte)'0';  // file identifier
            data[4] = (byte)(size >> 24);
            data[5] = (byte)(size >> 16);
            data[6] = (byte)(size >> 8);
            data[7] = (byte)(size);  // file size
            data[8] = 0;
            data[9] = 0;
            data[10] = 0;
            data[11] = 3; // version
            data[12] = 0;
            data[13] = 0;
            data[14] = 0;
            data[15] = 0; // offset to outer brres
            data[16] = 0;
            data[17] = 0;
            data[18] = 0;
            data[19] = 64; // header size
            data[20] = (byte)((size + size2) >> 24);
            data[21] = (byte)((size + size2) >> 16);
            data[22] = (byte)((size + size2) >> 8);
            data[23] = (byte)(size + size2);  // name location
            data[24] = palette_format_int32[0];
            data[25] = palette_format_int32[1];
            data[26] = palette_format_int32[2];
            data[27] = palette_format_int32[3];
            data[28] = (byte)(colour_number >> 8);
            data[29] = (byte)colour_number;
            for (int i = 30; i < 64; i++)
            {
                data[i] = 0;
            }
            FileMode mode = System.IO.FileMode.CreateNew;
            if (System.IO.File.Exists(output_file + ".plt0"))
            {
                mode = System.IO.FileMode.Truncate;
                if (warn)
                {
                    Console.WriteLine("Press enter to overwrite " + output_file + ".plt0");
                    Console.ReadLine();
                }
            }
            using (System.IO.FileStream file = System.IO.File.Open(output_file + ".plt0", mode, System.IO.FileAccess.Write))
            {
                file.Write(data, 0, 64);
                file.Write(colour_palette, 0, colour_palette.Length);
                file.Write(data2, 0, data2.Length);
                file.Close();
                Console.WriteLine(output_file + ".plt0");
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void write_TEX0(List<List<byte[]>> index_list)  // index_list contains all mipmaps.
        {
            int size = 0x40;
            double temp;
            int[] param = new int[4];
            List<int[]> settings = new List<int[]>();
            for (int i = 0; i < mipmaps_number + 1; i++)
            {
                if (i == 0)
                {
                    param[2] = (int)(index_list[i][0].Length * format_ratio);
                    param[3] = index_list[i].Count;
                    // param[2] = bitmap_width;
                    // param[3] = bitmap_height;
                }
                else
                {
                    temp = bitmap_width / Math.Pow(i, 2);
                    if (temp % 1 != 0)
                    {
                        param[2] = (int)(index_list[i][0].Length * format_ratio) + 1;
                        // param[2] = (int)temp + 1;
                    }
                    else
                    {
                        // param[2] = (int)temp;
                        param[2] = (int)(index_list[i][0].Length * format_ratio);
                    }
                    temp = bitmap_height / Math.Pow(i, 2);
                    if (temp % 1 != 0)
                    {
                        // param[3] = (int)temp + 1;
                        param[3] = index_list[i].Count;
                    }
                    else
                    {
                        // param[3] = (int)temp;
                        param[3] = index_list[i].Count;
                    }
                }
                temp = param[2] / block_width;
                if (temp % 1 != 0)
                {
                    param[0] = (int)temp + 1;
                }
                else
                {
                    param[0] = (int)temp;
                }
                temp = param[3] / block_height;
                if (temp % 1 != 0)
                {
                    param[1] = (int)temp + 1;
                }
                else
                {
                    param[1] = (int)temp;
                }
                settings.Add(param.ToArray());
                // size += param[0] * block_width * param[1] * block_height;
                size += index_list[i][0].Length * index_list[i].Count;
            }
            byte size2 = (byte)(4 + Math.Abs(16 - size) % 16);
            byte len = (byte)output_file.Split('\\').Length;
            string file_name = (output_file.Split('\\')[len - 1]);
            byte[] data2 = new byte[size2 + len + (16 - len) % 16];
            byte[] data = new byte[64];  // header data
            float mipmaps = mipmaps_number;
            byte[] mipmap_float = BitConverter.GetBytes(mipmaps);
            for (int i = 0; i < size2; i++)
            {
                data2[i] = 0;
            }
            for (int i = 0; i < file_name.Length; i++)
            {
                data2[i + size2] = (byte)file_name[i];
            }
            for (int i = size2 + file_name.Length; i < data2.Length; i++)
            {
                data2[i] = 0;
            }
            data[0] = (byte)'T';
            data[1] = (byte)'E';
            data[2] = (byte)'X';
            data[3] = (byte)'0';  // file identifier
            data[4] = (byte)(size >> 24);
            data[5] = (byte)(size >> 16);
            data[6] = (byte)(size >> 8);
            data[7] = (byte)(size);  // file size
            data[8] = 0;
            data[9] = 0;
            data[10] = 0;
            data[11] = 3; // version
            data[12] = 0;
            data[13] = 0;
            data[14] = 0;
            data[15] = 0;  // offset to outer brres
            data[16] = 0;
            data[17] = 0;
            data[18] = 0;
            data[19] = 64;  // header size
            data[20] = (byte)((size + size2) >> 24);
            data[21] = (byte)((size + size2) >> 16);
            data[22] = (byte)((size + size2) >> 8);
            data[23] = (byte)(size + size2);  // name location
            data[24] = 0;
            data[25] = 0;
            data[26] = 0;
            data[27] = 1;  // image has a palette
            data[28] = (byte)(bitmap_width >> 8);  // unsigned short width
            data[29] = (byte)bitmap_width; // second byte of width
            data[30] = (byte)(bitmap_height >> 8);  // height
            data[31] = (byte)bitmap_height;
            data[32] = texture_format_int32[0];
            data[33] = texture_format_int32[1];
            data[34] = texture_format_int32[2];
            data[35] = texture_format_int32[3];
            data[36] = 0;
            data[37] = 0;
            data[38] = 0;
            data[39] = (byte)(mipmaps_number + 1);
            data[40] = 0;
            data[41] = 0;
            data[42] = 0;
            data[43] = 0;  // always zero
            data[44] = mipmap_float[0];
            data[45] = mipmap_float[1];
            data[46] = mipmap_float[2];
            data[47] = mipmap_float[3];
            for (int i = 48; i < 64; i++)
            {
                data[i] = 0;
            }
            byte[] tex_data = new byte[size - 64];
            int count = 0;
            int height;
            int width;
            /*
            switch (texture_format_int32[3])
            {
                case 8: // CI4
                    {
                        block_width = 4;  // 4 bits per pixel  -  it's not really the block width, the real one is 8 but as each pixel is stored on 4 bit, I'm dividing it by two for my algorithm to work haha, isn't it a genius idea lol
                        break;
                    }
                case 10: // CI14x2
                    {
                        block_width = 8;  // 16 bits per pixel
                        break;
                    }
            }*/
            block_width = (sbyte)(block_width / format_ratio);
            for (int i = 0; i < settings.Count; i++)  // mipmaps
            {
                height = settings[i][1] * block_height - 1;
                width = 0;
                for (int j = settings[i][1]; j > 0; j--)  // number of height blocks  (remember that the last line was added first)
                {
                    for (int k = 0; k < settings[i][0]; k++)  // number of width blocks
                    {
                        for (int h = 0; h < block_height; h++)  // height in the block
                        {
                            for (int w = 0; w < block_width; w++)  // width in the block
                            {
                                tex_data[count] = index_list[i][height - h][width + w];
                                count++;
                            }
                        }
                        width += block_width;
                    } // end of the loop to go through number of horizontal blocks
                    height -= block_height;
                    width = 0;
                }   // go through vertical blocks
            }      // go through mipmaps
            FileMode mode = System.IO.FileMode.CreateNew;
            if (System.IO.File.Exists(output_file + ".tex0"))
            {
                mode = System.IO.FileMode.Truncate;
                if (warn)
                {
                    Console.WriteLine("Press enter to overwrite " + output_file + ".tex0");
                    Console.ReadLine();
                }
            }
            using (System.IO.FileStream file = System.IO.File.Open(output_file + ".tex0", mode, System.IO.FileAccess.Write))
            {
                file.Write(data, 0, 64);
                file.Write(tex_data, 0, size - 64);
                file.Write(data2, 0, data2.Length);
                file.Close();
                Console.WriteLine(output_file + ".tex0");
                // Console.ReadLine();
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public List<byte[]> build_mipmap(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {  // no colour table with mipmaps, as colour_palette is already filled

            List<ushort> Colours = new List<ushort>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
            ushort pixel = bitmap_width;
            if ((bitmap_width / block_width) % 1 != 0)
            {
                pixel = (ushort)(((ushort)(bitmap_width / block_width) + 1) * block_width);
            }
            switch (texture_format_int32[3])
            {
                case 8: // CI4
                    {
                        pixel /= 2;  // 4 bits per pixel
                        break;
                    }
                case 10:
                    {
                        pixel *= 2;  // 16 bits per pixel
                        break;
                    }
            }
            byte[] index = new byte[pixel]; // will contains a row of pixel index starting by the bottom one because bmp_image starts by the bottom one
            List<byte[]> index_list = new List<byte[]>(); // will contains each row of index
            int j = 0;
            byte red;
            byte green;
            byte blue;
            byte a;
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            if (bmp_image[0x1C] != 32)
            {
                Console.WriteLine("HOLY SHIT (colour depth of the converted bmp image is " + bmp_image[0x1C] + ")");
                Environment.Exit(0);
            }
            switch (bmp_image[0x1C])  // colour depth
            {
                case 32:  // 32-bit BGRA bmp image
                    {
                        switch (palette_format_int32[3])
                        {
                            case 0:  // AI8
                                {

                                    switch (algorithm)
                                    {
                                        case 0: // cie_601
                                            {
                                                for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
                                                {
                                                    pixel = (ushort)(bmp_image[i + 3] << 8);  // alpha value
                                                    if (bmp_image[i + 3] != 0)
                                                    {
                                                        pixel += (ushort)((byte)(bmp_image[i] * 0.114 + bmp_image[i + 1] * 0.587 + bmp_image[i + 2] * 0.299));
                                                    }
                                                    Colours.Add(pixel);
                                                }
                                                break;
                                            }
                                        case 1: // cie_709
                                            {
                                                for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                {
                                                    pixel = (ushort)(bmp_image[i + 3] << 8);  // alpha value
                                                    if (bmp_image[i + 3] != 0)
                                                    {
                                                        pixel += (ushort)((byte)(bmp_image[i] * 0.0721 + bmp_image[i + 1] * 0.7154 + bmp_image[i + 2] * 0.2125));
                                                    }
                                                    Colours.Add(pixel);
                                                }
                                                break;
                                            }
                                        case 2:  // custom
                                            {
                                                for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                {
                                                    pixel = (ushort)((byte)(bmp_image[i + 3] * custom_rgba[3]) << 8);  // alpha value
                                                    if (pixel != 0)
                                                    {
                                                        pixel += (ushort)(byte)(bmp_image[i] * custom_rgba[2] + bmp_image[i + 1] * custom_rgba[1] + bmp_image[i + 2] * custom_rgba[0]);
                                                    }
                                                    Colours.Add(pixel);
                                                }
                                                break;
                                            }
                                    }
                                    Console.WriteLine("creating indexes");
                                    j = 0;
                                    switch (texture_format_int32[3])
                                    {
                                        case 8: // CI4
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width; w++)  // index_size = number of pixels
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                diff = (short)(Math.Abs(colour_palette[i] - (byte)(Colours[j] >> 8)) + Math.Abs(colour_palette[i + 1] - (byte)Colours[j]));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        if (w % 2 == 0)  // stores the index on the upper 4 bits
                                                        {
                                                            index[w / 2] = (byte)(diff_min_index << 4);
                                                        }
                                                        else  // stores the index on the lower 4 bits
                                                        {
                                                            index[w / 2] += diff_min_index;
                                                        }
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        if (pixel % 2 != 0)
                                                        {
                                                            pixel++;
                                                        }
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel / 2] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                        case 9: // CI8
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width; w++)  // index_size = number of pixels
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                diff = (short)(Math.Abs(colour_palette[i] - (byte)(Colours[j] >> 8)) + Math.Abs(colour_palette[i + 1] - (byte)Colours[j]));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        index[w] = diff_min_index;
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                        case 10:  // CI14x2
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width * 2; w += 2)  // multiplied by two because each index is a 14 bytes integer
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                diff = (short)(Math.Abs(colour_palette[i] - (byte)(Colours[j] >> 8)) + Math.Abs(colour_palette[i + 1] - (byte)Colours[j]));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        index[w] = (byte)(diff_min_index >> 8);  // adding a short at each iteration
                                                        index[w + 1] = (byte)diff_min_index;  // casting to byte acts as a % 0xff
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel * 2] = 0;
                                                            index[pixel * 2 + 1] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 1:  // RGB565
                                {

                                    switch (algorithm)
                                    {
                                        case 2:  // custom  RRRR RGGG GGGB BBBB
                                            {
                                                for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                {
                                                    red = (byte)(bmp_image[i + 2] * custom_rgba[0]);
                                                    green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                                    blue = (byte)(bmp_image[i] * custom_rgba[2]);
                                                    if ((red & 4) == 4 && red < 248)  // 5-bit max value on a trimmed byte
                                                    {
                                                        red += 4;
                                                    }
                                                    if ((green & 2) == 2 && green < 252)  // 6-bit max value on a trimmed byte
                                                    {
                                                        green += 2;
                                                    }
                                                    if ((blue & 4) == 4 && blue < 248)
                                                    {
                                                        blue += 4;
                                                    }
                                                    pixel = (ushort)((((byte)(red) >> 3) << 11) + (((byte)(green) >> 2) << 5) + (byte)(blue) >> 3);
                                                    Colours.Add(pixel);
                                                }

                                                break;
                                            }
                                        default: // RRRR RGGG GGGB BBBB
                                            {
                                                for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                {
                                                    red = bmp_image[i + 2];
                                                    green = bmp_image[i + 1];
                                                    blue = bmp_image[i];
                                                    if ((red & 4) == 4 && red < 248)  // 5-bit max value on a trimmed byte
                                                    {
                                                        red += 4;
                                                    }
                                                    if ((green & 2) == 2 && green < 252)  // 6-bit max value on a trimmed byte
                                                    {
                                                        green += 2;
                                                    }
                                                    if ((blue & 4) == 4 && blue < 248)
                                                    {
                                                        blue += 4;
                                                    }
                                                    pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3));
                                                    Colours.Add(pixel);
                                                }
                                                break;
                                            }
                                    }
                                    Console.WriteLine("creating indexes");
                                    j = 0;

                                    switch (texture_format_int32[3])
                                    {
                                        case 8: // CI4
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width; w++)  // index_size = number of pixels
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else  // calculate difference between each separate colour channel and store the sum
                                                            {

                                                                diff = (short)(Math.Abs((colour_palette[i] & 248) - ((Colours[j] >> 8) & 248)) + Math.Abs(((colour_palette[i] & 7) << 5) + ((colour_palette[i + 1] >> 3) & 28) - ((Colours[j] >> 3) & 252)) + Math.Abs(((colour_palette[i + 1] << 3) & 248) - ((Colours[j] << 3) & 248))); if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        if (w % 2 == 0)  // stores the index on the upper 4 bits
                                                        {
                                                            index[w / 2] = (byte)(diff_min_index << 4);
                                                        }
                                                        else  // stores the index on the lower 4 bits
                                                        {
                                                            index[w / 2] += diff_min_index;
                                                        }
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        if (pixel % 2 != 0)
                                                        {
                                                            pixel++;
                                                        }
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel / 2] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                        case 9: // CI8
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width; w++)  // index_size = number of pixels
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else  // calculate difference between each separate colour channel and store the sum
                                                            {

                                                                diff = (short)(Math.Abs((colour_palette[i] & 248) - ((Colours[j] >> 8) & 248)) + Math.Abs(((colour_palette[i] & 7) << 5) + ((colour_palette[i + 1] >> 3) & 28) - ((Colours[j] >> 3) & 252)) + Math.Abs(((colour_palette[i + 1] << 3) & 248) - ((Colours[j] << 3) & 248))); 
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        index[w] = diff_min_index;
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                        case 10:  // CI14x2
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width * 2; w += 2)  // multiplied by two because each index is a 14 bytes integer
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else  // calculate difference between each separate colour channel and store the sum
                                                            {

                                                                diff = (short)(Math.Abs((colour_palette[i] & 248) - ((Colours[j] >> 8) & 248)) + Math.Abs(((colour_palette[i] & 7) << 5) + ((colour_palette[i + 1] >> 3) & 28) - ((Colours[j] >> 3) & 252)) + Math.Abs(((colour_palette[i + 1] << 3) & 248) - ((Colours[j] << 3) & 248))); 
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        index[w] = (byte)(diff_min_index >> 8);  // adding a short at each iteration
                                                        index[w + 1] = (byte)diff_min_index;  // casting to byte acts as a % 0xff
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel * 2] = 0;
                                                            index[pixel * 2 + 1] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 2:  // RGB5A3
                                {
                                    switch (algorithm)
                                    {
                                        case 2:  // custom
                                            {
                                                if (alpha == 1)  // 0AAA RRRR GGGG BBBB
                                                {
                                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                    {
                                                        a = (byte)(bmp_image[i + 3] * custom_rgba[3]);
                                                        red = (byte)(bmp_image[i + 2] * custom_rgba[0]);
                                                        green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                                        blue = (byte)(bmp_image[i] * custom_rgba[2]);
                                                        if ((a & 16) == 16 && a < 224)  // 3-bit max value on a trimmed byte
                                                        {
                                                            a += 16;
                                                        }
                                                        if ((red & 8) == 8 && red < 240)  // 4-bit max value on a trimmed byte
                                                        {
                                                            red += 8;
                                                        }
                                                        if ((green & 8) == 8 && green < 240)
                                                        {
                                                            green += 8;
                                                        }
                                                        if ((blue & 8) == 8 && blue < 240)
                                                        {
                                                            blue += 8;
                                                        }
                                                        pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                                        Colours.Add(pixel);
                                                    }
                                                }
                                                else if (alpha == 0)  // 1RRR RRGG GGGB BBBB
                                                {
                                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                    {
                                                        red = (byte)(bmp_image[i + 2] * custom_rgba[0]);
                                                        green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                                        blue = (byte)(bmp_image[i] * custom_rgba[2]);
                                                        if ((red & 4) == 4 && red < 248)  // 5-bit max value on a trimmed byte
                                                        {
                                                            red += 4;
                                                        }
                                                        if ((green & 4) == 4 && green < 248)
                                                        {
                                                            green += 4;
                                                        }
                                                        if ((blue & 4) == 4 && blue < 248)
                                                        {
                                                            blue += 4;
                                                        }
                                                        pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                                        Colours.Add(pixel);
                                                    }
                                                }
                                                else  // check for each colour if alpha trimmed to 3 bits is 255
                                                {
                                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                    {
                                                        a = (byte)(bmp_image[i + 3] * custom_rgba[3]);
                                                        red = (byte)(bmp_image[i + 2] * custom_rgba[0]);
                                                        green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                                        blue = (byte)(bmp_image[i] * custom_rgba[2]);
                                                        if ((a & 16) == 16 && a < 224)  // 3-bit max value on a trimmed byte
                                                        {
                                                            a += 16;
                                                        }
                                                        if ((red & 8) == 8 && red < 240)  // 4-bit max value on a trimmed byte
                                                        {
                                                            red += 8;
                                                        }
                                                        if ((green & 8) == 8 && green < 240)
                                                        {
                                                            green += 8;
                                                        }
                                                        if ((blue & 8) == 8 && blue < 240)
                                                        {
                                                            blue += 8;
                                                        }
                                                        if (a > 223)  // no alpha
                                                        {
                                                            pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                                        }
                                                        else
                                                        {
                                                            pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                                        }
                                                        Colours.Add(pixel);
                                                    }
                                                }
                                                break;
                                            }
                                        default:
                                            {
                                                if (alpha == 1)  // 0AAA RRRR GGGG BBBB
                                                {
                                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                    {
                                                        a = bmp_image[i + 3];
                                                        red = bmp_image[i + 2];
                                                        green = bmp_image[i + 1];
                                                        blue = bmp_image[i];
                                                        if ((a & 16) == 16 && a < 224)  // 3-bit max value on a trimmed byte
                                                        {
                                                            a += 16;
                                                        }
                                                        if ((red & 8) == 8 && red < 240)  // 4-bit max value on a trimmed byte
                                                        {
                                                            red += 8;
                                                        }
                                                        if ((green & 8) == 8 && green < 240)
                                                        {
                                                            green += 8;
                                                        }
                                                        if ((blue & 8) == 8 && blue < 240)
                                                        {
                                                            blue += 8;
                                                        }
                                                        pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                                        Colours.Add(pixel);
                                                    }
                                                }
                                                else if (alpha == 0)  // 1RRR RRGG GGGB BBBB
                                                {
                                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                    {
                                                        red = bmp_image[i + 2];
                                                        green = bmp_image[i + 1];
                                                        blue = bmp_image[i];
                                                        if ((red & 4) == 4 && red < 248)  // 5-bit max value on a trimmed byte
                                                        {
                                                            red += 4;
                                                        }
                                                        if ((green & 4) == 4 && green < 248)
                                                        {
                                                            green += 4;
                                                        }
                                                        if ((blue & 4) == 4 && blue < 248)
                                                        {
                                                            blue += 4;
                                                        }
                                                        pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                                        Colours.Add(pixel);
                                                    }
                                                }
                                                else  // mix up alpha and no alpha
                                                {
                                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                    {
                                                        a = bmp_image[i + 3];
                                                        red = bmp_image[i + 2];
                                                        green = bmp_image[i + 1];
                                                        blue = bmp_image[i];
                                                        if ((a & 16) == 16 && a < 224)  // 3-bit max value on a trimmed byte
                                                        {
                                                            a += 16;
                                                        }
                                                        if ((red & 8) == 8 && red < 240)  // 4-bit max value on a trimmed byte
                                                        {
                                                            red += 8;
                                                        }
                                                        if ((green & 8) == 8 && green < 240)
                                                        {
                                                            green += 8;
                                                        }
                                                        if ((blue & 8) == 8 && blue < 240)
                                                        {
                                                            blue += 8;
                                                        }
                                                        if (a > 223)  // no alpha
                                                        {
                                                            pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                                        }
                                                        else
                                                        {
                                                            pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                                        }
                                                        Colours.Add(pixel);
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    byte a2;
                                    byte red2;
                                    byte green2;
                                    byte blue2;
                                    Console.WriteLine("creating indexes");
                                    j = 0;
                                    switch (texture_format_int32[3])
                                    {
                                        case 8: // CI4
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width; w++)  // index_size = number of pixels
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else  // calculate difference between each separate colour channel and store the sum
                                                            {
                                                                if ((colour_palette[i] >> 7) == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                                {
                                                                    a = 255;
                                                                    red = (byte)((colour_palette[i] << 1) & 248);
                                                                    green = (byte)(((colour_palette[i] & 3) << 6) + ((colour_palette[i + 1] >> 2) & 56));
                                                                    blue = (byte)((colour_palette[i + 1] << 3) & 248);
                                                                }
                                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                                {
                                                                    a = (byte)(colour_palette[i] & 112);
                                                                    red = (byte)((colour_palette[i] << 4) & 240);
                                                                    green = (byte)(colour_palette[i + 1] & 240);
                                                                    blue = (byte)((colour_palette[i + 1] << 4) & 240);
                                                                }
                                                                if (Colours[j] >> 15 == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                                {
                                                                    a2 = 255;
                                                                    red2 = (byte)((Colours[j] >> 7) & 248);
                                                                    green2 = (byte)((Colours[j] >> 2) & 248);
                                                                    blue2 = (byte)((Colours[j] << 3) & 248);
                                                                }
                                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                                {
                                                                    a2 = (byte)((Colours[j] >> 8) & 112);
                                                                    red2 = (byte)((Colours[j] >> 4) & 240);
                                                                    green2 = (byte)((Colours[j]) & 240);
                                                                    blue2 = (byte)((Colours[j] << 4) & 240);
                                                                }
                                                                diff = (short)(Math.Abs(a - a2) + Math.Abs(red - red2) + Math.Abs(green - green2) + Math.Abs(blue - blue2));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        if (w % 2 == 0)  // stores the index on the upper 4 bits
                                                        {
                                                            index[w / 2] = (byte)(diff_min_index << 4);
                                                        }
                                                        else  // stores the index on the lower 4 bits
                                                        {
                                                            index[w / 2] += diff_min_index;
                                                        }
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        if (pixel % 2 != 0)
                                                        {
                                                            pixel++;
                                                        }
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel / 2] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                        case 9: // CI8
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width; w++)  // index_size = number of pixels
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else  // calculate difference between each separate colour channel and store the sum
                                                            {
                                                                if ((colour_palette[i] >> 7) == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                                {
                                                                    a = 255;
                                                                    red = (byte)((colour_palette[i] << 1) & 248);
                                                                    green = (byte)(((colour_palette[i] & 3) << 6) + ((colour_palette[i + 1] >> 2) & 56));
                                                                    blue = (byte)((colour_palette[i + 1] << 3) & 248);
                                                                }
                                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                                {
                                                                    a = (byte)(colour_palette[i] & 112);
                                                                    red = (byte)((colour_palette[i] << 4) & 240);
                                                                    green = (byte)(colour_palette[i + 1] & 240);
                                                                    blue = (byte)((colour_palette[i + 1] << 4) & 240);
                                                                }
                                                                if (Colours[j] >> 15 == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                                {
                                                                    a2 = 255;
                                                                    red2 = (byte)((Colours[j] >> 7) & 248);
                                                                    green2 = (byte)((Colours[j] >> 2) & 248);
                                                                    blue2 = (byte)((Colours[j] << 3) & 248);
                                                                }
                                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                                {
                                                                    a2 = (byte)((Colours[j] >> 8) & 112);
                                                                    red2 = (byte)((Colours[j] >> 4) & 240);
                                                                    green2 = (byte)((Colours[j]) & 240);
                                                                    blue2 = (byte)((Colours[j] << 4) & 240);
                                                                }
                                                                diff = (short)(Math.Abs(a - a2) + Math.Abs(red - red2) + Math.Abs(green - green2) + Math.Abs(blue - blue2));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        index[w] = diff_min_index;
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                        case 10:  // CI14x2
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width * 2; w += 2)  // multiplied by two because each index is a 14 bytes integer
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else  // calculate difference between each separate colour channel and store the sum
                                                            {
                                                                if ((colour_palette[i] >> 7) == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                                {
                                                                    a = 255;
                                                                    red = (byte)((colour_palette[i] << 1) & 248);
                                                                    green = (byte)(((colour_palette[i] & 3) << 6) + ((colour_palette[i + 1] >> 2) & 56));
                                                                    blue = (byte)((colour_palette[i + 1] << 3) & 248);
                                                                }
                                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                                {
                                                                    a = (byte)(colour_palette[i] & 112);
                                                                    red = (byte)((colour_palette[i] << 4) & 240);
                                                                    green = (byte)(colour_palette[i + 1] & 240);
                                                                    blue = (byte)((colour_palette[i + 1] << 4) & 240);
                                                                }
                                                                if (Colours[j] >> 15 == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                                {
                                                                    a2 = 255;
                                                                    red2 = (byte)((Colours[j] >> 7) & 248);
                                                                    green2 = (byte)((Colours[j] >> 2) & 248);
                                                                    blue2 = (byte)((Colours[j] << 3) & 248);
                                                                }
                                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                                {
                                                                    a2 = (byte)((Colours[j] >> 8) & 112);
                                                                    red2 = (byte)((Colours[j] >> 4) & 240);
                                                                    green2 = (byte)((Colours[j]) & 240);
                                                                    blue2 = (byte)((Colours[j] << 4) & 240);
                                                                }
                                                                diff = (short)(Math.Abs(a - a2) + Math.Abs(red - red2) + Math.Abs(green - green2) + Math.Abs(blue - blue2));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        index[w] = (byte)(diff_min_index >> 8);  // adding a short at each iteration
                                                        index[w + 1] = (byte)diff_min_index;  // casting to byte acts as a % 0xff
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel * 2] = 0;
                                                            index[pixel * 2 + 1] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }

                        pixel = (ushort)(Math.Abs(block_height - bitmap_height) % block_height);  // fills missing block height
                        if (pixel != 0)  // fills in the missing block data by adding rows full of zeros
                        {
                            for (int i = 0; i < index.Length; i++)  // fills in the row with zeros
                            {
                                index[i] = 0;
                            }
                            for (; pixel < block_height; pixel++)  // adds them
                            {
                                index_list.Add(index.ToArray());
                            }
                        }
                        break;
                    } // end of case depth = 32-bit BMP
            } // end of switch (colour depth)
            return index_list;
        }


        [MethodImpl(MethodImplOptions.NoOptimization)]
        public List<byte[]> create_PLT0(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {
            List<ushort> Colours = new List<ushort>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
            List<int[]> Colour_Table = new List<int[]>();  // {occurence, every possible salues of a short} used to find the most used colours, and then build a colour_palette from these
            int[] colour = { 0, 0 };
            for (int i = 0; i < 65536; i++)
            {
                colour[1] = i;
                Colour_Table.Add(colour.ToArray());  // adds a copy of the current array, so it won't be linked after changes on next loop
            }
            ushort pixel = bitmap_width;
            if (bitmap_height % block_height != 0)
            {
                fill_height = true;
                Console.WriteLine("Height is not a multiple of " + block_height + " (this value changes for each format), just be aware that the file size would be the same with an image of dimensions " + (bitmap_width + (bitmap_width % block_width)) + "x" + (bitmap_height + (bitmap_height % block_height)));
            }
            if (bitmap_width % block_width != 0)
            {
                fill_width = true;

                Console.WriteLine("Width is not a multiple of " + block_width + " (this value changes for each format), just be aware that the file size would be the same with an image of dimensions " + (bitmap_width + (bitmap_width % block_width)) + "x" + (bitmap_height + (bitmap_height % block_height)));
                pixel = (ushort)(((ushort)(bitmap_width / block_width) + 1) * block_width);
            }
            switch (texture_format_int32[3])
            {
                case 8: // CI4
                    {
                        pixel /= 2;  // 4 bits per pixel
                        break;
                    }
                case 10:
                    {
                        pixel *= 2;  // 16 bits per pixel
                        break;
                    }
            }
            byte[] index = new byte[pixel]; // will contains a row of pixel index starting by the bottom one because bmp_image starts by the bottom one
            List<byte[]> index_list = new List<byte[]>(); // will contains each row of index
            if (fill_height)  // fills in the missing block data by adding rows full of zeros (if you wonder why I'm filling before all lines, it's because I'm dealing with a bmp, this naughty file format starts with the bottom line)
            {
                pixel = (ushort)(bitmap_height % block_height);  // fills missing block height
                for (int i = 0; i < index.Length; i++)  // fills in the row with zeros
                {
                    index[i] = 0;
                }
                for (; pixel < block_height; pixel++)  // adds them
                {
                    index_list.Add(index.ToArray());
                }
            }
            int j = 0;
            byte red;
            byte green;
            byte blue;
            byte a;
            bool not_similar;
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            if (bmp_image[0x1C] != 32)
            {
                Console.WriteLine("HOLY SHIT (colour depth of the converted bmp image is " + bmp_image[0x1C] + ")");
                Environment.Exit(0);
            }
            switch (bmp_image[0x1C])  // colour depth
            {
                case 32:  // 32-bit BGRA bmp image
                    {
                        switch (palette_format_int32[3])
                        {
                            case 0:  // AI8
                                {

                                    switch (algorithm)
                                    {
                                        case 0: // cie_601
                                            {
                                                for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
                                                {
                                                    pixel = (ushort)(bmp_image[i + 3] << 8);  // alpha value
                                                    if (bmp_image[i + 3] != 0)
                                                    {
                                                        pixel += (ushort)((byte)(bmp_image[i] * 0.114 + bmp_image[i + 1] * 0.587 + bmp_image[i + 2] * 0.299));
                                                    }
                                                    Colours.Add(pixel);
                                                    Colour_Table[pixel][0] += 1;
                                                }
                                                break;
                                            }
                                        case 1: // cie_709
                                            {
                                                for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                {
                                                    pixel = (ushort)(bmp_image[i + 3] << 8);  // alpha value
                                                    if (bmp_image[i + 3] != 0)
                                                    {
                                                        pixel += (ushort)((byte)(bmp_image[i] * 0.0721 + bmp_image[i + 1] * 0.7154 + bmp_image[i + 2] * 0.2125));
                                                    }
                                                    Colours.Add(pixel);
                                                    Colour_Table[pixel][0] += 1;
                                                }
                                                break;
                                            }
                                        case 2:  // custom
                                            {
                                                for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                {
                                                    pixel = (ushort)((byte)(bmp_image[i + 3] * custom_rgba[3]) << 8);  // alpha value
                                                    if (pixel != 0)
                                                    {
                                                        pixel += (ushort)(byte)(bmp_image[i] * custom_rgba[2] + bmp_image[i + 1] * custom_rgba[1] + bmp_image[i + 2] * custom_rgba[0]);
                                                    }
                                                    Colours.Add(pixel);
                                                    Colour_Table[pixel][0] += 1;
                                                }
                                                break;
                                            }
                                    }
                                    Colour_Table.Sort(new IntArrayComparer());  // sorts the table by the most used colour first
                                    Console.WriteLine("findind most used Colours");
                                    for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                    {
                                        not_similar = true;
                                        if (Colour_Table[i][0] / pixel_count < percentage / 100)
                                        {
                                            break;
                                        }
                                        for (int k = 0; k < j; k += 2)
                                        {
                                            if (Math.Abs(colour_palette[k] - (byte)(Colour_Table[i][1] >> 8)) < diversity && Math.Abs(colour_palette[k + 1] - (byte)(Colour_Table[i][1])) < diversity)
                                            {
                                                not_similar = false;
                                                break;
                                            }
                                        }
                                        if (not_similar)
                                        {
                                            colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Alpha value
                                            colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                                            j += 2;
                                        }
                                    }
                                    if (j < colour_number_x2) // if the colour palette is not full
                                    {
                                        Console.WriteLine("The colour palette was not full, starting second loop...\n");
                                        for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                        {
                                            not_similar = true;
                                            if (Colour_Table[i][0] / pixel_count < percentage2 / 100)
                                            {
                                                break;
                                            }
                                            for (int k = 0; k < j; k += 2)
                                            {
                                                if (Math.Abs(colour_palette[k] - (byte)(Colour_Table[i][1] >> 8)) < diversity2 && Math.Abs(colour_palette[k + 1] - (byte)(Colour_Table[i][1])) < diversity2)
                                                {
                                                    not_similar = false;
                                                    break;
                                                }
                                            }
                                            if (not_similar)
                                            {
                                                colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Alpha value
                                                colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                                                j += 2;
                                            }
                                        }
                                        if (j < colour_number_x2) // if the colour palette is still not full
                                        {
                                            Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                            for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                            {
                                                colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Alpha value
                                                colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                                                j += 2;
                                            }
                                        }
                                    }
                                    Console.WriteLine("creating indexes");
                                    j = 0;
                                    switch (texture_format_int32[3])
                                    {
                                        case 8: // CI4
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width; w++)  // index_size = number of pixels
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                diff = (short)(Math.Abs(colour_palette[i] - (byte)(Colours[j] >> 8)) + Math.Abs(colour_palette[i + 1] - (byte)Colours[j]));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        if (w % 2 == 0)  // stores the index on the upper 4 bits
                                                        {
                                                            index[w / 2] = (byte)(diff_min_index << 4);
                                                        }
                                                        else  // stores the index on the lower 4 bits
                                                        {
                                                            index[w / 2] += diff_min_index;
                                                        }
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        if (pixel % 2 != 0)
                                                        {
                                                            pixel++;
                                                        }
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel / 2] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                        case 9: // CI8
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width; w++)  // index_size = number of pixels
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                diff = (short)(Math.Abs(colour_palette[i] - (byte)(Colours[j] >> 8)) + Math.Abs(colour_palette[i + 1] - (byte)Colours[j]));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        index[w] = diff_min_index;
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                        case 10:  // CI14x2
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width * 2; w += 2)  // multiplied by two because each index is a 14 bytes integer
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                diff = (short)(Math.Abs(colour_palette[i] - (byte)(Colours[j] >> 8)) + Math.Abs(colour_palette[i + 1] - (byte)Colours[j]));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        index[w] = (byte)(diff_min_index >> 8);  // adding a short at each iteration
                                                        index[w + 1] = (byte)diff_min_index;  // casting to byte acts as a % 0xff
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel * 2] = 0;
                                                            index[pixel * 2 + 1] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 1:  // RGB565
                                {

                                    switch (algorithm)
                                    {
                                        case 2:  // custom  RRRR RGGG GGGB BBBB
                                            {
                                                for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                {
                                                    red = (byte)(bmp_image[i + 2] * custom_rgba[0]);
                                                    green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                                    blue = (byte)(bmp_image[i] * custom_rgba[2]);
                                                    if ((red & 4) == 4 && red < 248)  // 5-bit max value on a trimmed byte
                                                    {
                                                        red += 4;
                                                    }
                                                    if ((green & 2) == 2 && green < 252)  // 6-bit max value on a trimmed byte
                                                    {
                                                        green += 2;
                                                    }
                                                    if ((blue & 4) == 4 && blue < 248)
                                                    {
                                                        blue += 4;
                                                    }
                                                    pixel = (ushort)((((byte)(red) >> 3) << 11) + (((byte)(green) >> 2) << 5) + (byte)(blue) >> 3);
                                                    Colours.Add(pixel);
                                                    Colour_Table[pixel][0] += 1;
                                                }

                                                break;
                                            }
                                        default: // RRRR RGGG GGGB BBBB
                                            {
                                                for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                {
                                                    red = bmp_image[i + 2];
                                                    green = bmp_image[i + 1];
                                                    blue = bmp_image[i];
                                                    if ((red & 4) == 4 && red < 248)  // 5-bit max value on a trimmed byte
                                                    {
                                                        red += 4;
                                                    }
                                                    if ((green & 2) == 2 && green < 252)  // 6-bit max value on a trimmed byte
                                                    {
                                                        green += 2;
                                                    }
                                                    if ((blue & 4) == 4 && blue < 248)
                                                    {
                                                        blue += 4;
                                                    }
                                                    pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3));
                                                    Colours.Add(pixel);
                                                    Colour_Table[pixel][0] += 1;
                                                }
                                                break;
                                            }
                                    }
                                    Colour_Table.Sort(new IntArrayComparer());  // sorts the table by the most used colour first
                                    Console.WriteLine("findind most used Colours");
                                    for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                    {
                                        not_similar = true;
                                        if (Colour_Table[i][0] / pixel_count < percentage / 100)
                                        {
                                            break;
                                        }
                                        for (int k = 0; k < j; k += 2)
                                        {
                                            if (Math.Abs((colour_palette[k] & 248) - ((Colour_Table[i][1] >> 8) & 248)) < diversity && Math.Abs(((colour_palette[k] & 7) << 5) + ((colour_palette[k + 1] >> 3) & 28) - ((Colour_Table[i][1] >> 3) & 252)) < diversity && Math.Abs(((colour_palette[k + 1] << 3) & 248) - (Colour_Table[i][1] << 3) & 248) < diversity)
                                            {
                                                not_similar = false;
                                                break;
                                            }
                                        }
                                        if (not_similar)
                                        {
                                            colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the RRRR RGGG value
                                            colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the GGGB BBBB value
                                            j += 2;
                                        }
                                    }
                                    if (j < colour_number_x2) // if the colour palette is not full
                                    {
                                        Console.WriteLine("The colour palette was not full, starting second loop...\n");
                                        for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                        {
                                            not_similar = true;
                                            if (Colour_Table[i][0] / pixel_count < percentage2 / 100)
                                            {
                                                break;
                                            }
                                            for (int k = 0; k < j; k += 2)
                                            {
                                                if (Math.Abs((colour_palette[k] & 248) - ((Colour_Table[i][1] >> 8) & 248)) < diversity2 && Math.Abs(((colour_palette[k] & 7) << 5) + ((colour_palette[k + 1] >> 3) & 28) - ((Colour_Table[i][1] >> 3) & 252)) < diversity2 && Math.Abs(((colour_palette[k + 1] << 3) & 248) - (Colour_Table[i][1] << 3) & 248) < diversity2)
                                                {
                                                    not_similar = false;
                                                    break;
                                                }
                                            }
                                            if (not_similar)
                                            {
                                                colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Red and green value
                                                colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Green and blue value
                                                j += 2;
                                            }
                                        }
                                        if (j < colour_number_x2) // if the colour palette is still not full
                                        {
                                            Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                            for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                            {
                                                colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the RRRR RGGG value
                                                colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the GGGB BBBB value
                                                j += 2;
                                            }
                                        }
                                    }

                                    Console.WriteLine("creating indexes");
                                    j = 0;
                                    switch (texture_format_int32[3])
                                    {
                                        case 8: // CI4
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width; w++)  // index_size = number of pixels
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else  // calculate difference between each separate colour channel and store the sum
                                                            {
                                                                diff = (short)(Math.Abs((colour_palette[i] & 248) - ((Colours[j] >> 8) & 248)) + Math.Abs(((colour_palette[i] & 7) << 5) + ((colour_palette[i + 1] >> 3) & 28) - ((Colours[j] >> 3) & 252)) + Math.Abs(((colour_palette[i + 1] << 3) & 248) - ((Colours[j] << 3) & 248)));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        if (w % 2 == 0)  // stores the index on the upper 4 bits
                                                        {
                                                            index[w / 2] = (byte)(diff_min_index << 4);
                                                        }
                                                        else  // stores the index on the lower 4 bits
                                                        {
                                                            index[w / 2] += diff_min_index;
                                                        }
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        if (pixel % 2 != 0)
                                                        {
                                                            pixel++;
                                                        }
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel / 2] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                        case 9: // CI8
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width; w++)  // index_size = number of pixels
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else  // calculate difference between each separate colour channel and store the sum
                                                            {

                                                                diff = (short)(Math.Abs((colour_palette[i] & 248) - ((Colours[j] >> 8) & 248)) + Math.Abs(((colour_palette[i] & 7) << 5) + ((colour_palette[i + 1] >> 3) & 28) - ((Colours[j] >> 3) & 252)) + Math.Abs(((colour_palette[i + 1] << 3) & 248) - ((Colours[j] << 3) & 248))); if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        index[w] = diff_min_index;
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                        case 10:  // CI14x2
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width * 2; w += 2)  // multiplied by two because each index is a 14 bytes integer
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else  // calculate difference between each separate colour channel and store the sum
                                                            {

                                                                diff = (short)(Math.Abs((colour_palette[i] & 248) - ((Colours[j] >> 8) & 248)) + Math.Abs(((colour_palette[i] & 7) << 5) + ((colour_palette[i + 1] >> 3) & 28) - ((Colours[j] >> 3) & 252)) + Math.Abs(((colour_palette[i + 1] << 3) & 248) - ((Colours[j] << 3) & 248))); if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        index[w] = (byte)(diff_min_index >> 8);  // adding a short at each iteration
                                                        index[w + 1] = (byte)diff_min_index;  // casting to byte acts as a % 0xff
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel * 2] = 0;
                                                            index[pixel * 2 + 1] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 2:  // RGB5A3
                                {
                                    switch (algorithm)
                                    {
                                        case 2:  // custom
                                            {
                                                if (alpha == 1)  // 0AAA RRRR GGGG BBBB
                                                {
                                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                    {
                                                        a = (byte)(bmp_image[i + 3] * custom_rgba[3]);
                                                        red = (byte)(bmp_image[i + 2] * custom_rgba[0]);
                                                        green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                                        blue = (byte)(bmp_image[i] * custom_rgba[2]);
                                                        if ((a & 16) == 16 && a < 224)  // 3-bit max value on a trimmed byte
                                                        {
                                                            a += 16;
                                                        }
                                                        if ((red & 8) == 8 && red < 240)  // 4-bit max value on a trimmed byte
                                                        {
                                                            red += 8;
                                                        }
                                                        if ((green & 8) == 8 && green < 240)
                                                        {
                                                            green += 8;
                                                        }
                                                        if ((blue & 8) == 8 && blue < 240)
                                                        {
                                                            blue += 8;
                                                        }
                                                        pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                                        Colours.Add(pixel);
                                                        Colour_Table[pixel][0] += 1;
                                                    }
                                                }
                                                else if (alpha == 0)  // 1RRR RRGG GGGB BBBB
                                                {
                                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                    {
                                                        red = (byte)(bmp_image[i + 2] * custom_rgba[0]);
                                                        green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                                        blue = (byte)(bmp_image[i] * custom_rgba[2]);
                                                        if ((red & 4) == 4 && red < 248)  // 5-bit max value on a trimmed byte
                                                        {
                                                            red += 4;
                                                        }
                                                        if ((green & 4) == 4 && green < 248)
                                                        {
                                                            green += 4;
                                                        }
                                                        if ((blue & 4) == 4 && blue < 248)
                                                        {
                                                            blue += 4;
                                                        }
                                                        pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                                        Colours.Add(pixel);
                                                        Colour_Table[pixel][0] += 1;
                                                    }
                                                }
                                                else  // check for each colour if alpha trimmed to 3 bits is 255
                                                {
                                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                    {
                                                        a = (byte)(bmp_image[i + 3] * custom_rgba[3]);
                                                        red = (byte)(bmp_image[i + 2] * custom_rgba[0]);
                                                        green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                                        blue = (byte)(bmp_image[i] * custom_rgba[2]);
                                                        if ((a & 16) == 16 && a < 224)  // 3-bit max value on a trimmed byte
                                                        {
                                                            a += 16;
                                                        }
                                                        if ((red & 8) == 8 && red < 240)  // 4-bit max value on a trimmed byte
                                                        {
                                                            red += 8;
                                                        }
                                                        if ((green & 8) == 8 && green < 240)
                                                        {
                                                            green += 8;
                                                        }
                                                        if ((blue & 8) == 8 && blue < 240)
                                                        {
                                                            blue += 8;
                                                        }
                                                        if (a > 223)  // no alpha
                                                        {
                                                            pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                                        }
                                                        else
                                                        {
                                                            pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                                        }
                                                        Colours.Add(pixel);
                                                        Colour_Table[pixel][0] += 1;
                                                    }
                                                }
                                                break;
                                            }
                                        default:
                                            {
                                                if (alpha == 1)  // 0AAA RRRR GGGG BBBB
                                                {
                                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                    {
                                                        a = bmp_image[i + 3];
                                                        red = bmp_image[i + 2];
                                                        green = bmp_image[i + 1];
                                                        blue = bmp_image[i];
                                                        if ((a & 16) == 16 && a < 224)  // 3-bit max value on a trimmed byte
                                                        {
                                                            a += 16;
                                                        }
                                                        if ((red & 8) == 8 && red < 240)  // 4-bit max value on a trimmed byte
                                                        {
                                                            red += 8;
                                                        }
                                                        if ((green & 8) == 8 && green < 240)
                                                        {
                                                            green += 8;
                                                        }
                                                        if ((blue & 8) == 8 && blue < 240)
                                                        {
                                                            blue += 8;
                                                        }
                                                        pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                                        Colours.Add(pixel);
                                                        Colour_Table[pixel][0] += 1;
                                                    }
                                                }
                                                else if (alpha == 0)  // 1RRR RRGG GGGB BBBB
                                                {
                                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                    {
                                                        red = bmp_image[i + 2];
                                                        green = bmp_image[i + 1];
                                                        blue = bmp_image[i];
                                                        if ((red & 4) == 4 && red < 248)  // 5-bit max value on a trimmed byte
                                                        {
                                                            red += 4;
                                                        }
                                                        if ((green & 4) == 4 && green < 248)
                                                        {
                                                            green += 4;
                                                        }
                                                        if ((blue & 4) == 4 && blue < 248)
                                                        {
                                                            blue += 4;
                                                        }
                                                        pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                                        Colours.Add(pixel);
                                                        Colour_Table[pixel][0] += 1;
                                                    }
                                                }
                                                else  // mix up alpha and no alpha
                                                {
                                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                    {
                                                        a = bmp_image[i + 3];
                                                        red = bmp_image[i + 2];
                                                        green = bmp_image[i + 1];
                                                        blue = bmp_image[i];
                                                        if ((a & 16) == 16 && a < 224)  // 3-bit max value on a trimmed byte
                                                        {
                                                            a += 16;
                                                        }
                                                        if ((red & 8) == 8 && red < 240)  // 4-bit max value on a trimmed byte
                                                        {
                                                            red += 8;
                                                        }
                                                        if ((green & 8) == 8 && green < 240)
                                                        {
                                                            green += 8;
                                                        }
                                                        if ((blue & 8) == 8 && blue < 240)
                                                        {
                                                            blue += 8;
                                                        }
                                                        if (a > 223)  // no alpha
                                                        {
                                                            pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                                        }
                                                        else
                                                        {
                                                            pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                                        }
                                                        Colours.Add(pixel);
                                                        Colour_Table[pixel][0] += 1;
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    Colour_Table.Sort(new IntArrayComparer());  // sorts the table by the most used colour first
                                    Console.WriteLine("findind most used Colours");
                                    byte a2;
                                    byte red2;
                                    byte green2;
                                    byte blue2;
                                    if (alpha == 1)
                                    {
                                        for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                        {
                                            not_similar = true;
                                            if (Colour_Table[i][0] / pixel_count < percentage/ 100)
                                            {
                                                break;
                                            }
                                            for (int k = 0; k < j; k += 2)
                                            {
                                                if (Math.Abs((colour_palette[k] & 112) - (Colour_Table[i][1] >> 8) & 112) < diversity && Math.Abs(((colour_palette[k] << 4) & 240) - ((Colour_Table[i][1] >> 4) & 240)) < diversity && Math.Abs((colour_palette[k + 1] & 240) - ((Colour_Table[i][1]) & 240)) < diversity && Math.Abs(((colour_palette[k + 1] << 4) & 240) - ((Colour_Table[i][1] << 4) & 240)) < diversity)
                                                {
                                                    not_similar = false;
                                                    break;
                                                }
                                            }
                                            if (not_similar)
                                            {
                                                colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the 0AAA RRRR value
                                                colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the GGGG BBBB value
                                                j += 2;
                                            }
                                        }
                                        if (j < colour_number_x2) // if the colour palette is not full
                                        {
                                            Console.WriteLine("The colour palette was not full, starting second loop...\n");
                                            for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                            {
                                                not_similar = true;
                                                if (Colour_Table[i][0] / pixel_count < percentage2 / 100)
                                                {
                                                    break;
                                                }
                                                for (int k = 0; k < j; k += 2)
                                                {
                                                    if (Math.Abs((colour_palette[k] & 112) - (Colour_Table[i][1] >> 8) & 112) < diversity2 && Math.Abs(((colour_palette[k] << 4) & 240) - ((Colour_Table[i][1] >> 4) & 240)) < diversity2 && Math.Abs((colour_palette[k + 1] & 240) - ((Colour_Table[i][1]) & 240)) < diversity2 && Math.Abs(((colour_palette[k + 1] << 4) & 240) - ((Colour_Table[i][1] << 4) & 240)) < diversity2)
                                                    {
                                                        not_similar = false;
                                                        break;
                                                    }
                                                }
                                                if (not_similar)
                                                {
                                                    colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the 0AAA RRRR value
                                                    colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the GGGG BBBB value
                                                    j += 2;
                                                }
                                            }
                                            if (j < colour_number_x2) // if the colour palette is still not full
                                            {
                                                Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                                for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                                {
                                                    colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the 0AAA RRRR value
                                                    colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the GGGG BBBB value
                                                    j += 2;
                                                }
                                            }
                                        }
                                    }
                                    else if (alpha == 0)  // 1RRR RRGG GGGB BBBB
                                    {
                                        for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                        {
                                            not_similar = true;
                                            if (Colour_Table[i][0] / pixel_count < percentage / 100)
                                            {
                                                break;
                                            }
                                            for (int k = 0; k < j; k += 2)
                                            {
                                                if (Math.Abs(((colour_palette[k] << 1) & 248) - ((Colour_Table[i][1] >> 7) & 248)) < diversity && Math.Abs(((colour_palette[k] & 3) << 6) + ((colour_palette[k + 1] >> 2) & 56) - ((Colour_Table[i][1] >> 2) & 248)) < diversity && Math.Abs(((colour_palette[k + 1] << 3) & 248) - (Colour_Table[i][1] << 3) & 248) < diversity)
                                                {
                                                    not_similar = false;
                                                    break;
                                                }
                                            }
                                            if (not_similar)
                                            {
                                                colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the 1RRR RRGG value
                                                colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the GGGB BBBB value
                                                j += 2;
                                            }
                                        }
                                        if (j < colour_number_x2) // if the colour palette is not full
                                        {
                                            Console.WriteLine("The colour palette was not full, starting second loop...\n");
                                            for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                            {
                                                not_similar = true;
                                                if (Colour_Table[i][0] / pixel_count < percentage2 / 100)
                                                {
                                                    break;
                                                }
                                                for (int k = 0; k < j; k += 2)
                                                {
                                                    if (Math.Abs(((colour_palette[k] << 1) & 248) - ((Colour_Table[i][1] >> 7) & 248)) < diversity2 && Math.Abs(((colour_palette[k] & 3) << 6) + ((colour_palette[k + 1] >> 2) & 56) - ((Colour_Table[i][1] >> 2) & 248)) < diversity2 && Math.Abs(((colour_palette[k + 1] << 3) & 248) - (Colour_Table[i][1] << 3) & 248) < diversity2)
                                                    {
                                                        not_similar = false;
                                                        break;
                                                    }
                                                }
                                                if (not_similar)
                                                {
                                                    colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Alpha and red value
                                                    colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Gren and blue value
                                                    j += 2;
                                                }
                                            }
                                            if (j < colour_number_x2) // if the colour palette is still not full
                                            {
                                                Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                                for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                                {
                                                    colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Alpha and red value
                                                    colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Green and blue value
                                                    j += 2;
                                                }
                                            }
                                        }
                                    }
                                    else  // mix
                                    {

                                        for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                        {
                                            not_similar = true;
                                            if (Colour_Table[i][0] / pixel_count < percentage / 100)
                                            {
                                                break;
                                            }
                                            for (int k = 0; k < j; k += 2)
                                            {
                                                if ((colour_palette[k] >> 7) == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                {
                                                    a = 255;
                                                    red = (byte)((colour_palette[k] << 1) & 248);
                                                    green = (byte)(((colour_palette[k] & 3) << 6) + ((colour_palette[k + 1] >> 2) & 56));
                                                    blue = (byte)((colour_palette[k + 1] << 3) & 248);
                                                }
                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                {
                                                    a = (byte)(colour_palette[k] & 112);
                                                    red = (byte)((colour_palette[k] << 4) & 240);
                                                    green = (byte)(colour_palette[k + 1] & 240);
                                                    blue = (byte)((colour_palette[k + 1] << 4) & 240);
                                                }
                                                if (Colour_Table[i][1] >> 15 == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                {
                                                    a2 = 255;
                                                    red2 = (byte)((Colour_Table[i][1] >> 7) & 248);
                                                    green2 = (byte)((Colour_Table[i][1] >> 2) & 248);
                                                    blue2 = (byte)((Colour_Table[i][1] << 3) & 248);
                                                }
                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                {
                                                    a2 = (byte)((Colour_Table[i][1] >> 8) & 112);
                                                    red2 = (byte)((Colour_Table[i][1] >> 4) & 240);
                                                    green2 = (byte)((Colour_Table[i][1]) & 240);
                                                    blue2 = (byte)((Colour_Table[i][1] << 4) & 240);
                                                }
                                                if (Math.Abs(a - a2) < diversity && Math.Abs(red - red2) < diversity && Math.Abs(green - green2) < diversity && Math.Abs(blue - blue2) < diversity)
                                                {
                                                    not_similar = false;
                                                    break;
                                                }
                                            }
                                            if (not_similar)
                                            {
                                                colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Alpha and red value
                                                colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Green and blue value
                                                j += 2;
                                            }
                                        }
                                        if (j < colour_number_x2) // if the colour palette is not full
                                        {
                                            Console.WriteLine("The colour palette was not full, starting second loop...\n");
                                            for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                            {
                                                not_similar = true;
                                                if (Colour_Table[i][0] / pixel_count < percentage2 / 100)
                                                {
                                                    break;
                                                }
                                                for (int k = 0; k < j; k += 2)
                                                {
                                                    if ((colour_palette[k] >> 7) == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                    {
                                                        a = 255;
                                                        red = (byte)((colour_palette[k] << 1) & 248);
                                                        green = (byte)(((colour_palette[k] & 3) << 6) + ((colour_palette[k + 1] >> 2) & 56));
                                                        blue = (byte)((colour_palette[k + 1] << 3) & 248);
                                                    }
                                                    else  // alpha - 0AAA RRRR GGGG BBBB
                                                    {
                                                        a = (byte)(colour_palette[k] & 112);
                                                        red = (byte)((colour_palette[k] << 4) & 240);
                                                        green = (byte)(colour_palette[k + 1] & 240);
                                                        blue = (byte)((colour_palette[k + 1] << 4) & 240);
                                                    }
                                                    if (Colour_Table[i][1] >> 15 == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                    {
                                                        a2 = 255;
                                                        red2 = (byte)((Colour_Table[i][1] >> 7) & 248);
                                                        green2 = (byte)((Colour_Table[i][1] >> 2) & 248);
                                                        blue2 = (byte)((Colour_Table[i][1] << 3) & 248);
                                                    }
                                                    else  // alpha - 0AAA RRRR GGGG BBBB
                                                    {
                                                        a2 = (byte)((Colour_Table[i][1] >> 8) & 112);
                                                        red2 = (byte)((Colour_Table[i][1] >> 4) & 240);
                                                        green2 = (byte)((Colour_Table[i][1]) & 240);
                                                        blue2 = (byte)((Colour_Table[i][1] << 4) & 240);
                                                    }
                                                    if (Math.Abs(a - a2) < diversity2 && Math.Abs(red - red2) < diversity2 && Math.Abs(green - green2) < diversity2 && Math.Abs(blue - blue2) < diversity2)
                                                    {
                                                        not_similar = false;
                                                        break;
                                                    }
                                                }
                                                if (not_similar)
                                                {
                                                    colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Alpha value
                                                    colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                                                    j += 2;
                                                }
                                            }
                                            if (j < colour_number_x2) // if the colour palette is still not full
                                            {
                                                Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                                for (int i = 0; i < Colour_Table.Count && j < colour_number_x2; i++)
                                                {
                                                    colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Alpha value
                                                    colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                                                    j += 2;
                                                }
                                            }
                                        }
                                    }
                                    Console.WriteLine("creating indexes");
                                    j = 0;
                                    switch (texture_format_int32[3])
                                    {
                                        case 8: // CI4
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width; w++)  // index_size = number of pixels
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else  // calculate difference between each separate colour channel and store the sum
                                                            {
                                                                if ((colour_palette[i] >> 7) == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                                {
                                                                    a = 255;
                                                                    red = (byte)((colour_palette[i] << 1) & 248);
                                                                    green = (byte)(((colour_palette[i] & 3) << 6) + ((colour_palette[i + 1] >> 2) & 56));
                                                                    blue = (byte)((colour_palette[i + 1] << 3) & 248);
                                                                }
                                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                                {
                                                                    a = (byte)(colour_palette[i] & 112);
                                                                    red = (byte)((colour_palette[i] << 4) & 240);
                                                                    green = (byte)(colour_palette[i + 1] & 240);
                                                                    blue = (byte)((colour_palette[i + 1] << 4) & 240);
                                                                }
                                                                if (Colours[j] >> 15 == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                                {
                                                                    a2 = 255;
                                                                    red2 = (byte)((Colours[j] >> 7) & 248);
                                                                    green2 = (byte)((Colours[j] >> 2) & 248);
                                                                    blue2 = (byte)((Colours[j] << 3) & 248);
                                                                }
                                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                                {
                                                                    a2 = (byte)((Colours[j] >> 8) & 112);
                                                                    red2 = (byte)((Colours[j] >> 4) & 240);
                                                                    green2 = (byte)((Colours[j]) & 240);
                                                                    blue2 = (byte)((Colours[j] << 4) & 240);
                                                                }
                                                                diff = (short)(Math.Abs(a - a2) + Math.Abs(red - red2) + Math.Abs(green - green2) + Math.Abs(blue - blue2));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        if (w % 2 == 0)  // stores the index on the upper 4 bits
                                                        {
                                                            index[w / 2] = (byte)(diff_min_index << 4);
                                                        }
                                                        else  // stores the index on the lower 4 bits
                                                        {
                                                            index[w / 2] += diff_min_index;
                                                        }
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        if (pixel % 2 != 0)
                                                        {
                                                            pixel++;
                                                        }
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel / 2] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                        case 9: // CI8
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width; w++)  // index_size = number of pixels
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else  // calculate difference between each separate colour channel and store the sum
                                                            {
                                                                if ((colour_palette[i] >> 7) == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                                {
                                                                    a = 255;
                                                                    red = (byte)((colour_palette[i] << 1) & 248);
                                                                    green = (byte)(((colour_palette[i] & 3) << 6) + ((colour_palette[i + 1] >> 2) & 56));
                                                                    blue = (byte)((colour_palette[i + 1] << 3) & 248);
                                                                }
                                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                                {
                                                                    a = (byte)(colour_palette[i] & 112);
                                                                    red = (byte)((colour_palette[i] << 4) & 240);
                                                                    green = (byte)(colour_palette[i + 1] & 240);
                                                                    blue = (byte)((colour_palette[i + 1] << 4) & 240);
                                                                }
                                                                if (Colours[j] >> 15 == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                                {
                                                                    a2 = 255;
                                                                    red2 = (byte)((Colours[j] >> 7) & 248);
                                                                    green2 = (byte)((Colours[j] >> 2) & 248);
                                                                    blue2 = (byte)((Colours[j] << 3) & 248);
                                                                }
                                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                                {
                                                                    a2 = (byte)((Colours[j] >> 8) & 112);
                                                                    red2 = (byte)((Colours[j] >> 4) & 240);
                                                                    green2 = (byte)((Colours[j]) & 240);
                                                                    blue2 = (byte)((Colours[j] << 4) & 240);
                                                                }
                                                                diff = (short)(Math.Abs(a - a2) + Math.Abs(red - red2) + Math.Abs(green - green2) + Math.Abs(blue - blue2));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        index[w] = diff_min_index;
                                                        j += 1;
                                                    }
                                                    if (fill_width) // fills the block width data by adding zeros to the width
                                                    {
                                                        pixel = (ushort)(bitmap_width % block_width);
                                                        for (int i = bitmap_width; pixel < block_width; pixel++, i++)
                                                        {
                                                            index[i] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                        case 10:  // CI14x2
                                            {
                                                for (int h = 0; h < bitmap_height; h++)
                                                {
                                                    for (int w = 0; w < bitmap_width * 2; w += 2)  // multiplied by two because each index is a 14 bytes integer
                                                    {
                                                        diff_min = 500;
                                                        for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                        {
                                                            if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                            {
                                                                diff_min_index = (byte)(i / 2);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                break;
                                                            }
                                                            else  // calculate difference between each separate colour channel and store the sum
                                                            {
                                                                if ((colour_palette[i] >> 7) == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                                {
                                                                    a = 255;
                                                                    red = (byte)((colour_palette[i] << 1) & 248);
                                                                    green = (byte)(((colour_palette[i] & 3) << 6) + ((colour_palette[i + 1] >> 2) & 56));
                                                                    blue = (byte)((colour_palette[i + 1] << 3) & 248);
                                                                }
                                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                                {
                                                                    a = (byte)(colour_palette[i] & 112);
                                                                    red = (byte)((colour_palette[i] << 4) & 240);
                                                                    green = (byte)(colour_palette[i + 1] & 240);
                                                                    blue = (byte)((colour_palette[i + 1] << 4) & 240);
                                                                }
                                                                if (Colours[j] >> 15 == 1)  // no alpha - 1RRR RRGG GGGB BBBB
                                                                {
                                                                    a2 = 255;
                                                                    red2 = (byte)((Colours[j] >> 7) & 248);
                                                                    green2 = (byte)((Colours[j] >> 2) & 248);
                                                                    blue2 = (byte)((Colours[j] << 3) & 248);
                                                                }
                                                                else  // alpha - 0AAA RRRR GGGG BBBB
                                                                {
                                                                    a2 = (byte)((Colours[j] >> 8) & 112);
                                                                    red2 = (byte)((Colours[j] >> 4) & 240);
                                                                    green2 = (byte)((Colours[j]) & 240);
                                                                    blue2 = (byte)((Colours[j] << 4) & 240);
                                                                }
                                                                diff = (short)(Math.Abs(a - a2) + Math.Abs(red - red2) + Math.Abs(green - green2) + Math.Abs(blue - blue2));
                                                                if (diff < diff_min)
                                                                {
                                                                    diff_min = diff;
                                                                    diff_min_index = (byte)(i / 2);
                                                                }
                                                            }
                                                        }
                                                        index[w] = (byte)(diff_min_index >> 8);  // adding a short at each iteration
                                                        index[w + 1] = (byte)diff_min_index;  // casting to byte acts as a % 0xff
                                                        j += 1;
                                                    }
                                                    pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                    if (pixel != 0) // fills the block width data by adding zeros to the width
                                                    {
                                                        for (; pixel < block_width; pixel++)
                                                        {
                                                            index[pixel * 2] = 0;
                                                            index[pixel * 2 + 1] = 0;
                                                        }
                                                    }
                                                    index_list.Add(index.ToArray());
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }
                        break;
                    } // end of case depth = 32-bit BMP
            } // end of switch (colour depth)
            return index_list;
        }
        public byte[] Convert_to_bmp(System.Drawing.Bitmap imageIn)
        {
            var bmp = new Bitmap(imageIn.Width, imageIn.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);  // makes it 32 bit in depth
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(imageIn, new Rectangle(0, 0, imageIn.Width, imageIn.Height));
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }
        /*
        public static Bitmap ConvertTo32bpp(Image img)
        {
            var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }*/

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

    }

}
