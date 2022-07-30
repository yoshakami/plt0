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

BTI (File Format)
```
colourenc = ['I4', 'I8', 'IA4', 'IA8', 'RGB565', 'RGB5A3', 'RGBA8', 0, 'CI4', 'CI8', 'CI14x2', 0, 0, 0, 'CMPR']
paletteenc = ["IA8", "RGB565", "RGB5A3"]
wrap = ["Clamp", "Repeat", "Mirror"]
filter = ["Nearest", "Linear", "NearestMipmapNearest", "NearestMipmapLinear", "LinearMipmapNearest", "LinearMipmapLinear"]
File Header
Offset Size  Description
0x00    1    Image format (see colourenc)
0x01    1    Enable alpha (0 = disabled, 1 = 1-bit alpha, 2 = 8-bit alpha, 204 = unknown and only seen in bmd files)
0x02    2    Width of the image in pixels.
0x04    2    Height of the image in pixels.
0x06    1    WrapS ( 0 = clamp, 01 = repeat, 02 = mirror)
0x07    1    WrapT (vertical wrap)
0x08    1    0 if the bti has no palette, and 1 if the bti has a palette (no bmd in mkdd use a palette, only UI bti does)
0x09    1    Palette format [0 = IA8, 1 = RGB565, 2 = RGB5A3]
0x0A    2    Colour number
0x0C    4    Offset to palette data else 0 or [in BMD files] offset to the end of all bti headers, relative to the start of the file header
0x10    4    0 if no mipmap and 01 00 00 00 if the bti uses a mipmap, 0 if the bti with mipmap uses "1=Linear" for filters (Linear doesn't use mipmaps)*
0x14    1    Minification filter type (see filter).
0x15    1    Magnification filter type. (never > 1 because you won't use a mipmap for upscaling would you)
0x16    1    MinLOD  (always 0 in mkdd)
0x17    1    MaxLOD  (equal to (image_count-1)*8)
0x18    1    Total number of images, thus number of mipmaps + 1. (substract 1 and multiply by 8 to have the value at 0x17)
0x19    1    Unknown. always 0 outside bmd files. has a lot of different values in mkdd (safe to freely change)
0x1A    2    LODBias (apparently it's the distance from which the game should switch to mipmaps)  <------------------- I don't know how that works so I've made it to be always zero in the write_BTI function
0x1C    4    Offset to image data, relative to the start of the file header.
* according to SuperBMD, 0x11  = EdgeLOD (bool),  0x12 = BiasClamp (bool), 0x13 =  MaxAniso, but they're always 0 in mkdd

v-- BTI Data --v
0x20   -    Data Starts
```
 
 BMP (file format) - the windows one (also System.Drawing.Imaging.ImageFormat.Bmp)
https://en.wikipedia.org/wiki/BMP_file_format
```
v--- File Header ---v
Offset Size  Description
0x00    2    BM
0x02    4    File Size (for example 255 bytes is FF 00 00 00)
0x06    2    Reserved (= whatever you want)
0x08    2    Reserved (= whatever you want)
0x0A    4    pixel data start offset (usually 36 + colour palette length = 36 + number of colours * 4)

v--- DIB header ---v
0x0E    4    bytes remaining before the end of this f*cking header (28 00 00 00)
0x12    4    Width
0x16    4    Height
0x1A    2    number of color planes (01 00)
0x1C    2    Depth (bits per pixel) Typical values are 1, 4, 8, 16 (0x10), 24 (0x18) and 32 (0x20).
0x1E    4    Compression (00 00 00 00, please use another image format to compress)
0x22    4    size of the raw pixel data (wikipedia says 0 can be put if bmp is not compressed)
0x26    4    can be 0 (the horizontal resolution of the image. pixel per metre, signed integer)
0x2A    4    can be 0 (the vertical resolution of the image. pixel per metre, signed integer)
0x2E    4    number of colors in the color palette (remember bytes are swapped out)
0x32    4    number of colors in the color palette (remember bytes are swapped out)
// I'm not joking this thing has really to be pasted twice

v--- Colour Palette ---v
always has a size of a power of 2.
contains each colour next to each other
each colour is 4 bytes long : B G R A

v--- Pixel data ---v
(always starting by bottom left pixel then going by rows)
padding can be whatever value you want

depth 32: BGRA next to each other 
depth 24: BGR but each row has a padding to be a multiple of 4. 
depth 16: 4 bit Alpha, 4-bit Blue, 4-bit Green, 4-bit Red next to each other + row padding 
depth 8: each pixel is an byte index in the colour palette + row padding 
depth 4/2/1: each pixel is 4/2/1-bit index in the colour palette + row padding


Note: everything is in little-endian : ALL BYTES ARE SWAPPED OUT
12 34 56 78 -> 78 56 34 12
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
            main_class.check_exit();
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
    public class UshortArrayComparer : IComparer<ushort[]>
    {
        public int Compare(ushort[] ba, ushort[] bb)
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
        bool ask_exit = false;
        bool bmd = false;
        bool bmd_file = false;
        bool bmp = false;
        bool bti = false;
        bool exit = false;
        bool fill_height = false;
        bool fill_width = false;
        bool FORCE_ALPHA = false;
        bool funky = false;
        bool gif = false;
        bool grey = true;
        bool has_palette = false;
        bool ico = false;
        bool jpeg = false;
        bool jpg = false;
        bool png = false;
        bool random_palette = false;
        bool safe_mode = false;
        bool success = false;
        bool tif = false;
        bool tiff = false;
        bool tpl = false;
        bool user_palette = false;
        bool warn = false;
        byte WrapS = 1; // 0 = Clamp   1 = Repeat   2 = Mirror
        byte WrapT = 1; // 0 = Clamp   1 = Repeat   2 = Mirror
        byte algorithm = 0;  // 0 = CIE 601    1 = CIE 709     2 = custom RGBA
        byte alpha = 9;  // 0 = no alpha - 1 = alpha - 2 = mix 
        byte color;
        byte cmpr_alpha_threshold = 100;
        byte diversity = 10;
        byte diversity2 = 0;
        byte magnification_filter = 1;  // 0 = Nearest Neighbour   1 = Linear
        byte minificaction_filter = 1;  // 0 = Nearest Neighbour   1 = Linear
        byte mipmaps_number = 0;
        byte round3 = 16;
        byte round4 = 8;
        byte round5 = 4;
        byte round6 = 2;
        byte[] colour_palette;
        byte[] palette_format_int32 = { 0, 0, 0, 9 };  // 0 = AI8   1 = RGB565  2 = RGB5A3
        byte[] texture_format_int32 = { 0, 0, 0, 7 };  // 8 = CI4   9 = CI8    10 = CI14x2
        byte[] block_width_array = { 4, 8, 8, 8, 8, 8, 16, 4, 8, 8, 4 }; // altered to match bit-per pixel size.
        byte[] block_height_array = { 8, 4, 4, 4, 4, 4, 4, 8, 4, 4, 8 };
        double format_ratio = 1;  // bit par pixel * format_ratio = 8
        double percentage = 0;
        double percentage2 = 0;
        float[] custom_rgba = { 1, 1, 1, 1 };
        int colour_number_x2;
        int colour_number_x4;
        int fill_palette_start_offset = 0;
        int pass = 0;
        int pixel_count;
        int y = 0;
        sbyte block_height = -1;
        sbyte block_width = -1;
        string input_file = "";
        string input_fil = "";
        string input_ext = "";
        string input_file2 = "";
        string output_file = "";
        string swap = "";
        ushort bitmap_height;
        ushort bitmap_width;
        ushort colour_number = 0;
        ushort max_colours = 0;
        ushort z;
        List<byte> BGRA = new List<byte>();
        public void parse_args()
        {
            string[] args = Environment.GetCommandLineArgs();
            for (ushort i = 1; i < args.Length; i++)
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
                            palette_format_int32[3] = 0;  // will be set to texture format if texture format is unset
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
                    case "I4":
                        {
                            texture_format_int32[3] = 0;
                            block_width = 8;
                            block_height = 8;
                            format_ratio = 2;
                            has_palette = false;
                            break;
                        }
                    case "I8":
                        {
                            texture_format_int32[3] = 1;
                            block_width = 8;
                            block_height = 4;
                            format_ratio = 1;
                            has_palette = false;
                            break;
                        }
                    case "IA4":
                    case "AI4":
                        {
                            texture_format_int32[3] = 2;
                            block_width = 8;
                            block_height = 4;
                            format_ratio = 1;
                            has_palette = false;
                            break;
                        }
                    case "FORCE_ALPHA":
                        {
                            FORCE_ALPHA = true;
                            break;
                        }
                    case "FORCE":
                        {
                            FORCE_ALPHA = true;
                            if (args[i + 1].ToUpper() == "ALPHA")
                            {
                                pass = 1;
                            }
                            break;
                        }
                    case "RGBA32":
                    case "RGBA8":
                    case "RGBA64":
                        {
                            texture_format_int32[3] = 6;
                            block_width = 4;
                            block_height = 4;
                            format_ratio = 0.25;
                            has_palette = false;
                            break;
                        }
                    case "CMPR":
                        {
                            texture_format_int32[3] = 0xE;
                            block_width = 8;
                            block_height = 8;
                            format_ratio = 2;
                            has_palette = false;
                            break;
                        }
                    case "C4":
                    case "CI4":
                        {
                            max_colours = 16;
                            block_width = 8;
                            block_height = 8;
                            format_ratio = 2;
                            texture_format_int32[3] = 8;
                            has_palette = true;
                            break;
                        }
                    case "C8":
                    case "CI8":
                        {
                            max_colours = 256;
                            block_width = 8;
                            block_height = 4;
                            texture_format_int32[3] = 9;
                            has_palette = true;
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
                            has_palette = true;
                            break;
                        }
                    case "G2":
                        {
                            algorithm = 1;
                            break;
                        }
                    case "CC":
                        {
                            if (args.Length > i + 5)
                            {
                                algorithm = 2;
                                float.TryParse(args[i + 1], out custom_rgba[0]);
                                float.TryParse(args[i + 2], out custom_rgba[1]);
                                float.TryParse(args[i + 3], out custom_rgba[2]);
                                success = float.TryParse(args[i + 4], out custom_rgba[3]);
                                if (success)
                                {
                                    pass = 4;
                                }
                            }
                            break;
                        }
                    case "ROUND3":
                        {
                            success = byte.TryParse(args[i + 1], out round3);
                            if (success)
                            {
                                pass = 1;
                                if (round3 > 31)
                                {
                                    Console.WriteLine("um, so you would like to round up the 3rd bit if the value of the truncated bytes is greater than their max value, which means always round down (a.k.a BrawlCrate method), sure but be aware that the accuracy of the image is lowered");
                                }
                            }
                            break;
                        }
                    case "ROUND4":
                        {
                            success = byte.TryParse(args[i + 1], out round4);
                            if (success)
                            {
                                pass = 1;
                                if (round4 > 15)
                                {
                                    Console.WriteLine("um, so you would like to round up the 4th bit if the value of the truncated bytes is greater than their max value, which means always round down (a.k.a BrawlCrate method), sure but be aware that the accuracy of the image is lowered");
                                }
                            }
                            break;
                        }
                    case "ROUND5":
                        {
                            success = byte.TryParse(args[i + 1], out round5);
                            if (success)
                            {
                                pass = 1;
                                if (round5 > 7)
                                {
                                    Console.WriteLine("um, so you would like to round up the 5th bit if the value of the truncated bytes is greater than their max value, which means always round down (a.k.a BrawlCrate method), sure but be aware that the accuracy of the image is lowered");
                                }
                            }
                            break;
                        }
                    case "ROUND6":
                        {
                            success = byte.TryParse(args[i + 1], out round6);
                            if (success)
                            {
                                pass = 1;
                                if (round6 > 3)
                                {
                                    Console.WriteLine("um, so you would like to round up the 6th bit if the value of the truncated bytes is greater than their max value, which means always round down (a.k.a BrawlCrate method), sure but be aware that the accuracy of the image is lowered");
                                }
                            }
                            break;
                        }
                    case "FUNKY":
                        {
                            funky = true;
                            break;
                        }
                    case "ALPHA":
                    case "1BIT":
                    case "1-BIT":
                        {
                            if (args.Length == i + 1)
                            {
                                alpha = 1;
                            }
                            else
                            {
                                success = byte.TryParse(args[i + 1], out alpha);
                                if (success)
                                {
                                    pass = 1;
                                }
                                else
                                {
                                    alpha = 1;
                                }
                            }
                            break;
                        }
                    case "BMP":
                        {
                            bmp = true;
                            break;
                        }
                    case "BMD":
                        {
                            bmd = true;
                            break;
                        }
                    case "BTI":
                        {
                            bti = true;
                            break;
                        }
                    case "PNG":
                        {
                            png = true;
                            break;
                        }
                    case "TIF":
                        {
                            tif = true;
                            break;
                        }
                    case "TIFF":
                        {
                            tiff = true;
                            break;
                        }
                    case "ICO":
                        {
                            ico = true;
                            break;
                        }
                    case "JPG":
                        {
                            jpg = true;
                            break;
                        }
                    case "JPEG":
                        {
                            jpeg = true;
                            break;
                        }
                    case "GIF":
                        {
                            gif = true;
                            break;
                        }
                    case "C":
                        {
                            if (args.Length > i + 1)
                            {
                                success = ushort.TryParse(args[i + 1], out colour_number); // colour_number is now a number 

                                if (success)
                                {
                                    colour_number_x2 = colour_number << 1;
                                    colour_number_x4 = colour_number << 2;
                                    pass = 1;
                                }
                            }
                            break;
                        }
                    case "D":
                    case "DIVERSITY":
                        {
                            // default_diversity = false;
                            if (args.Length > i + 1)
                            {
                                success = byte.TryParse(args[i + 1], out diversity); // diversity is now a number 

                                if (success)
                                {
                                    pass = 1;
                                }
                            }
                            break;
                        }
                    case "D2":
                    case "DIVERSITY2":
                        {
                            // default_diversity2 = false;
                            if (args.Length > i + 1)
                            {
                                success = byte.TryParse(args[i + 1], out diversity2); // diversity2 is now a number 
                                if (success)
                                {
                                    pass = 1;
                                }
                            }
                            break;
                        }
                    case "M":
                    case "N-MIPMAPS":
                    case "N-MM":
                        {
                            if (args.Length > i + 1)
                            {
                                success = byte.TryParse(args[i + 1], out mipmaps_number); // mipmaps_number is now a number 
                                if (success)
                                {
                                    pass = 1;
                                }
                            }
                            break;
                        }
                    case "MIN":
                        {

                            if (args.Length < i + 2)
                            {
                                break;
                            }
                            pass = 1;
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
                                default:
                                    {
                                        pass = 0;
                                        break;
                                    }
                            }
                            break;
                        }
                    case "MAG":
                        {
                            if (args.Length < i + 2)
                            {
                                break;
                            }
                            pass = 1;
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
                                default:
                                    {
                                        pass = 0;
                                        break;
                                    }
                            }
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
                            if (args.Length < i + 2)
                            {
                                break;
                            }
                            if (args[i + 1].ToUpper() == "ALPHA")
                            {
                                alpha = 0;
                                pass = 1;
                            }
                            break;
                        }
                    case "PAL":
                        {
                            z = i;
                            pass = -1;
                            while (z < args.Length)
                            {
                                z++;
                                pass++;
                                if (args[z][0] == '#' && args[z].Length > 6)  // #RRGGBB
                                {
                                    byte.TryParse(args[z].Substring(5, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                    BGRA.Add(color);
                                    byte.TryParse(args[z].Substring(3, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                    BGRA.Add(color);
                                    byte.TryParse(args[z].Substring(1, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                    BGRA.Add(color);  // for god's sake, 0 is the hashtag, NOT A F-CKING INDEX START
                                    if (args[z].Length > 8)  // #RRGGBBAA
                                    {
                                        byte.TryParse(args[z].Substring(7, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                        BGRA.Add(color);
                                    }
                                    else
                                    {
                                        BGRA.Add(0xff); // no alpha
                                    }

                                }
                                else if (args[z][0] == '#' && args[z].Length > 3)  // #RGB
                                {
                                    byte.TryParse(args[z].Substring(3, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                    BGRA.Add((byte)(color << 4));
                                    byte.TryParse(args[z].Substring(2, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                    BGRA.Add((byte)(color << 4));
                                    byte.TryParse(args[z].Substring(1, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                    BGRA.Add((byte)(color << 4));
                                    if (args[z].Length > 4)  // #RGBA
                                    {
                                        byte.TryParse(args[z].Substring(4, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                        BGRA.Add((byte)(color << 4));
                                    }
                                    else
                                    {
                                        BGRA.Add(0xff); // no alpha
                                    }
                                }
                                else  // no # means there are other arguments
                                {
                                    break;
                                }
                            }
                            break;
                        }
                    case "P":
                    case "PERCENTAGE":
                        {
                            if (args.Length < i + 2)
                            {
                                break;
                            }
                            success = double.TryParse(args[i + 1], out percentage);  // percentage is now a double

                            if (success)
                            {
                                pass = 1;
                            }
                            break;
                        }
                    case "P2":
                    case "PERCENTAGE2":
                        {
                            if (args.Length < i + 2)
                            {
                                break;
                            }
                            success = double.TryParse(args[i + 1], out percentage2);  // percentage2 is now a double
                            if (success)
                            {
                                pass = 1;
                            }
                            break;
                        }
                    case "RANDOM":
                    case "RAND":
                        {
                            random_palette = true;
                            user_palette = true;  // palette doesn't need some complex indexing to be created
                            break;
                        }
                    case "CMPR_ALPHA_THRESHOLD":
                    case "CMPR_ALPHA_TRESHOLD":
                    case "CMPR_ALPHA_TRESOLD":
                    case "THRESHOLD":
                    case "TRESHOLD": // common typo I do a lot
                    case "TRESOLD":
                        {
                            if (args.Length > i + 1)
                            {
                                success = byte.TryParse(args[i + 1], out cmpr_alpha_threshold);
                                if (success)
                                {
                                    pass = 1;
                                }
                            }
                            break;
                        }
                    case "TPL":
                        {
                            tpl = true;
                            break;
                        }
                    case "WARN":
                    case "W":
                    case "VERBOSE":
                        {
                            warn = true;
                            break;
                        }
                    case "EXIT":
                    case "ASK":
                        {
                            ask_exit = true;
                            break;
                        }
                    case "NOERROR":
                    case "NO-ERROR":
                    case "NOERR":
                    case "NO-ERR":
                    case "SAFE":
                        {
                            safe_mode = true;
                            break;
                        }
                    case "WRAP":
                        {
                            if (args.Length < i + 3)
                            {
                                break;
                            }
                            pass = 2;
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
                                default:
                                    {
                                        pass = 0;
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
                                default:
                                    {
                                        pass = 0;
                                        break;
                                    }
                            }
                            break;
                        }

                    default:
                        {
                            if (System.IO.File.Exists(args[i]) && input_file == "")
                            {
                                input_file = args[i];
                                input_fil = args[i].Substring(0, args[i].Length - args[i].Split('.')[args[i].Split('.').Length - 1].Length - 1);  // removes the text after the extension dot.
                                input_ext = args[i].Substring(args[i].Length - args[i].Split('.')[args[i].Split('.').Length - 1].Length - 1, args[i].Length - input_fil.Length);  // removes the text before the extension dot.
                            }
                            else if (System.IO.File.Exists(swap) && input_file2 == "")  // swap out args[i] and output file.
                            {
                                input_file2 = swap;
                                if (args[i].Contains(".") && args[i].Length > 1)
                                {
                                    output_file = args[i].Substring(0, args[i].Length - args[i].Split('.')[args[i].Split('.').Length - 1].Length - 1);  // same as the os.path.splitext(args[i])[0] function in python
                                }
                                else
                                {
                                    output_file = args[i];
                                }

                            }
                            else
                            {
                                if (args[i].Contains(".") && args[i].Length > 1)
                                {
                                    output_file = args[i].Substring(0, args[i].Length - args[i].Split('.')[args[i].Split('.').Length - 1].Length - 1);  // removes the text after the extension dot.
                                    swap = args[i];
                                }
                                else
                                {
                                    output_file = args[i];
                                    swap = args[i];
                                }
                            }
                            break;
                        }
                }

            }
            if (input_file == "")
            {
                Console.WriteLine("no input file specified\nusage: PLT0 <file> [pal custom palette|bmd file|plt0 file|bmp file|png file|jpeg file|tiff file|ico file|gif file|rle file] [dest] [tpl|bti|bmd|bmp|png|gif|jpeg|ico|tiff] [Encoding Format] [Palette Format] [alpha|no alpha|mix] [c colour number] [d diversity] [d2 diversity2] [m mipmaps] [p percentage] [p2 percentage2] [g2|cc 0.7 0.369 0.4 1.0] [warn|w] [exit|ask] [safe|noerror] [random] [min nearest neighbour] [mag linear] [Wrap Clamp Clamp] [funky] [round3] [round4] [round5] [round6] [force alpha]\nthis is the usage format for parameters : [optional] <mandatory>\n\nthe order of the parameters doesn't matter, but the image to encode must be put before the second input file (bmd/image used as palette)\nif you don't specify tpl, this program will output by default a tex0 and a plt0 file.\nAvailable Encoding Formats: C4, CI4, C8, CI8, CI14x2, C14x2, default = CI8\nAvailable Palette Formats : IA8 (Black and White), RGB565 (RGB), RGB5A3 (RGBA), default = (auto)\n\nif the palette chosen is RGB5A3, you can force the image to have no alpha for all colours (so the RGB values will be stored on 5 bits each), or force all colours to use an alpha channel, by default it's set on 'mix'\n\ndashes are not needed before each parameter, it will still work if you add some as you please lol, this cli parsing function should be unbreakable\nthe number of colours is stored on 2 bytes, but the index bit length of the colour encoding format limits the max number of colours to these:\nCI4 : from 0 to 16\nCI8 : from 0 to 256\nCI14x2 : from 0 to 16384\n\nbmd = inject directly the bti file in a bmd (if the bti filename isn't in the bmd or you haven't entered a bmd second input file, this will notice you of the failure).\n\npal #RRGGBB #RRGGBB #RRGGBB ... is used to directly input your colour palette to a tex0 so it'll encode it with this palette. (example creating a 2 colour palette:plt0 pal #000000 #FFFFFF)\nif you input two existing images, the second one will be taken as the input palette.\n\nd | diversity = a minimum amount of difference for at least one colour channel in the colour palette\n(prevents a palette from having colours close to each other) default : 16 colours -> diversity = 60 | 256 colours -> diversity = 20\nvalue between 0 and 255, if the program heaven't filled the colour table with this parameter on, it'll fill it again with diversity2, then it'll let you know and fill it with the most used colours\n\n d2 | diversity2 = the diversity parameter for the second loop that fills the colour table if it's not full after the first loop.\n\n-m | --n-mm | m | --n-mipmaps = mipmaps number (number of duplicates versions of lower resolution of the input image stored in the output texture) default = 0\n\n p | percentage a double (64 bit float) between zero and 100. Alongside the diversity setting, if a colour counts for less than p % of the whole number of pixels, then it won't be added to the palette. default = 0 (0%)\n-p2 | p2 | percentage2 = the minimum percentage for the second loop that fills the colour table. default = 0 (0%)\nadd w or warn to make the program ask to you before overwriting the output file or tell you which parameters the program used.\n\nexit or ask is used to always make the tool make you press enter before exit.\nsafe or noerror is used to make the program never throw execution error code (useful for using subsystem.check_output in python without crash)\nrandom = random colour palette\n\ntype g2 to use grayscale recommendation CIE 709, the default one used is the recommendation CIE 601.\ntype cc then your float RGBA factors to multiply every pixel of the input image by this factor\n the order of parameters doesn't matter, though you need to add a number after commands like 'm', 'c' or 'd',\nmin or mag filters are used to choose the algorithm for downscaling/upscaling textures in a tpl file : (nearest neighbour, linear, NearestMipmapNearest, NearestMipmapLinear, LinearMipmapNearest, LinearMipmapLinear) you can just type nn or nearest instead of nearest neighbour or the filter number.\nwrap X Y is the syntax to define the wrap mode for both horizontal and vertical within the same option. X and Y can have these values (clamp, repeat, mirror) or just type the first letter. \nplease note that if a parameter corresponds to nothing described above and doesn't exists, it'll be taken as the output file name.\n\nRound3 : threshold for 3-bit values (alpha in RGB5A3). every value of the trimmed bits above this threshold will round up the last bit. Default: 16.\nRound4 : Default: 8 (it's the middle between 0 and 16)\nRound5 : Default: 4 (brawlcrate uses to put 0 everywhere)\nRound6 : Default: 2 (wimgt has every default value here +1 eg here it would be 3)\n\nFORCE ALPHA: by default only 32 bits RGBA Images will have alpha enabled automatically or not (if the image has transparent pixels) on RGBA32 and RGB5A3 and CMPR textures.\n1-bit, 4-bit, 8-bit and 24-bit depth images won't have alpha enabled unless you FORCE ALPHA (which will likely result in a fully transparent image)\nthreshold / cmpr_alpha_threshold : every pixel that has an alpha below this value will be fully transparent (only for cmpr format, value between 0 and 255). default = 100\nfunky : gives funky results if your output image is a bmp/png/gif etc. because it paste the raw encoded format in the bmp who's supposed to have a GBRA byte array 🤣🤣🤣\n\n Examples: (using wimgt synthax can work as seen below)\nplt0 rosalina.png -d 0 -x ci8 rgb5a3 --n-mipmaps 5 -w -c 256 (output name will be '-x.plt0' and '-x.tex0', as this is not an existing option)\nplt0 tpl ci4 rosalina.jpeg AI8 c 4 d 16 g2 m 1 warn texture.tex0");
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
            if (texture_format_int32[3] == 7 && palette_format_int32[3] != 9)  // if user has chosen not to convert to AI8, RGB565, or RGB5A3
            {
                texture_format_int32[3] = (byte)(palette_format_int32[3] + 3);
                block_width = 4;
                block_height = 4;
                format_ratio = 0.5;
                has_palette = false;
            }
            if (max_colours == 0 && texture_format_int32[3] == 7 && palette_format_int32[3] != 9)  // if user haven't chosen a texture format
            {
                max_colours = 256;  // let's set CI8 as default
                block_width = 8;
                block_height = 4;
                texture_format_int32[3] = 9;
                has_palette = true;
            }
            if (colour_number == 0)
            {
                colour_number = max_colours;
                colour_number_x2 = colour_number << 1;
                colour_number_x4 = colour_number << 2;
            }
            //try
            //{
            using (System.IO.FileStream file = System.IO.File.Open(input_file, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                byte[] id = new byte[4];
                file.Read(id, 0, 4);
                // msm tools handles these. the user can also make scripts to launch the program for every file he wants.
                //case "bres":
                //case .arc file from wii games
                //case bmd
                //case bti ??? how do I write that lol

                if (id.ToString() == "TEX0")
                {
                    file.Read(id, 0x23, 1);
                    if (id[0] == 8 || id[0] == 9 || id[0] == 10)  // CI4, CI8, CI14x2
                    {
                        if (input_file2 == "")
                        {
                            // get the palette file from the same directory or exit
                        }
                        else
                        {
                            // check if second file is the palette or exit
                        }
                    }
                    else
                    {
                        // decode the image
                    }
                }
            }
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
            if ((palette_format_int32[3] == 9 && texture_format_int32[3] > 7 && texture_format_int32[3] < 11) || (texture_format_int32[3] == 5 && alpha == 9)) // if a colour palette hasn't been selected by the user, this program will set it automatically to the most fitting one
            {                                                                                                                                                   // or if alpha hasn't been selected by the user
                                                                                                                                                                //y = 0;
                for (int i = 0; i < bitmap_width; i += 4)  // first row - or last one because it's a bmp lol
                {
                    //y += bmp_image[pixel_data_start_offset + i + 3];
                    if (bmp_image[pixel_data_start_offset + i + 3] != 0xff && alpha == 9)
                    {
                        alpha = 2;
                    }
                    if (bmp_image[pixel_data_start_offset + i] != bmp_image[pixel_data_start_offset + i + 1] || bmp_image[pixel_data_start_offset + i] == bmp_image[pixel_data_start_offset + i + 2])
                    {
                        grey = false;
                    }
                    if (alpha < 3 && !grey && y != 0)
                    {
                        break;
                    }
                }
                for (int i = bitmap_width * (bitmap_height >> 1); i % bitmap_width != 0; i += 4)  // middle row
                {
                    //y += bmp_image[pixel_data_start_offset + i + 3];
                    if (bmp_image[pixel_data_start_offset + i + 3] != 0xff && alpha == 9)
                    {
                        alpha = 2;
                    }
                    if (bmp_image[pixel_data_start_offset + i] != bmp_image[pixel_data_start_offset + i + 1] || bmp_image[pixel_data_start_offset + i] == bmp_image[pixel_data_start_offset + i + 2])
                    {
                        grey = false;
                    }
                    if (alpha < 3 && !grey && y != 0)
                    {
                        break;
                    }
                }
                //if (y == 0)
                //{
                //not_32bits;
                //}
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
            // I hope there could be a keyboard shortcut to format every tabs :')
            // I'm too lazy to do that manually, + that's just visual lol
            //edit: It's Ctrl + E
            ushort[] vanilla_size = { bitmap_width, bitmap_height };
            Array.Resize(ref colour_palette, colour_number_x2);
            if (BGRA.Count != 0)
            {
                byte[] colors = BGRA.ToArray();
                if (colors.Length > colour_number_x4)
                {
                    Console.WriteLine((int)(colors.Length / 4) + " colours sent in the palette but colour number is set to " + colour_number + " trimming the palette to " + colour_number);
                    Array.Resize(ref colors, colour_number_x4);
                }
                fill_palette(colors, 0, colors.Length);
                fill_palette_start_offset = colors.Length >> 1; // divides by two because PLT0 is two bytes per colour
                user_palette = true;
            }
            if (random_palette) // fill the palette randomly :')
            {
                Random rnd = new Random();
                rnd.NextBytes(colour_palette);
            }
            try  // try to see what the second file is
            {
                if (input_file2 != "")
                {
                    using (System.IO.FileStream file2 = System.IO.File.Open(input_file2, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        byte[] id2 = new byte[8];
                        file2.Read(id2, 0, 8);
                        if (id2.ToString() == "J3D2bmd3")
                        {
                            bmd_file = true;
                        }
                        else if (id2.ToString().Substring(0, 4) == "PLT0")
                        {
                            byte[] data = new byte[0x20];
                            file2.Read(data, 0, 0x20);
                            colour_number = (ushort)((data[0x1C] << 8) + data[0x1D]);
                            colour_number_x2 = colour_number << 1;
                            colour_number_x4 = colour_number << 2;
                            palette_format_int32[3] = data[0x1B];
                            file2.Read(colour_palette, 0x40, colour_number_x2);
                            user_palette = true;
                        }
                        else
                        {
                            byte[] bmp_palette = Convert_to_bmp((Bitmap)Bitmap.FromFile(input_file2));  // will fail if this isn't a supported image
                            user_palette = true;
                            int array_size = bmp_palette[2] | bmp_palette[3] << 8 | bmp_palette[4] << 16 | bmp_palette[5] << 24;
                            int pixel_start_offset = bmp_palette[10] | bmp_palette[11] << 8 | bmp_palette[12] << 16 | bmp_palette[13] << 24;
                            ushort bitmap_w = (ushort)(bmp_palette[0x13] << 8 | bmp_palette[0x12]);
                            ushort bitmap_h = (ushort)(bmp_palette[0x17] << 8 | bmp_palette[0x16]);
                            ushort pixel = (ushort)(bitmap_w * bitmap_h);
                            if (pixel != colour_number)
                            {
                                Console.WriteLine("Second image input has " + pixel + " pixels while there are " + colour_number + " max colours in the palette.");
                                return;
                            }
                            fill_palette(bmp_palette, pixel_start_offset, array_size);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Out of memory." && ex.Source == "System.Drawing")
                {
                    Console.WriteLine("Second image input format not supported (convert it to jpeg or png)");
                }
                else if (safe_mode)
                {
                    Console.WriteLine("error on " + input_file2 + "\nsafe mode is enabled, this program will exit silently");
                }
                else
                {
                    throw ex;
                }
                return;
            }
            List<List<byte[]>> index_list = new List<List<byte[]>>();
            object v = create_PLT0(bmp_image, bmp_filesize, pixel_data_start_offset);
            index_list.Add((List<byte[]>)v);

            if (warn)
            {
                Console.WriteLine("v-- bool --v\nbmd=" + bmd + " bmd_file=" + bmd_file + " bmp=" + bmp + " bti=" + bti + " fill_height=" + fill_height + " fill_width=" + fill_width + " gif=" + gif + " grey=" + grey + " ico=" + ico + " jpeg=" + jpeg + " jpg=" + jpg + " png=" + png + " success=" + success + " tif=" + tif + " tiff=" + tiff + " tpl=" + tpl + " user_palette=" + user_palette + " warn=" + warn + "\n\nv-- byte --v\nWrapS=" + WrapS + " WrapT=" + WrapT + " algorithm=" + algorithm + " alpha=" + alpha + " color=" + color + " diversity=" + diversity + " diversity2=" + diversity2 + " magnification_filter=" + magnification_filter + " minificaction_filter=" + minificaction_filter + " mipmaps_number=" + mipmaps_number + "\n\nv-- byte[] --v\ncolour_palette=" + colour_palette + " palette_format_int32=" + palette_format_int32 + " texture_format_int32=" + texture_format_int32 + "\n\nv-- double --v\nformat_ratio=" + format_ratio + " percentage=" + percentage + " percentage2=" + percentage2 + "\n\nv-- float[] --v\ncustom_rgba=" + custom_rgba + "\n\nv-- int --v\ncolour_number_x2=" + colour_number_x2 + " colour_number_x4=" + colour_number_x4 + " pass=" + pass + " pixel_count=" + pixel_count + "\n\nv-- signed byte --v\nblock_height=" + block_height + " block_width=" + block_width + "\n\nv-- string --v\ninput_file=" + input_file + " input_file2=" + input_file2 + " output_file=" + output_file + " swap=" + swap + "\n\nv-- unsigned short --v\nbitmap_height=" + bitmap_height + " bitmap_width=" + bitmap_width + " colour_number=" + colour_number + " max_colours=" + max_colours + " z=" + z + "\n\nv-- List<byte> --v\nBGRA=" + BGRA);
            }
            for (z = 1; z <= mipmaps_number; z++)
            {
                if (warn)
                {
                    Console.WriteLine("processing mipmap " + z);
                }
                if (System.IO.File.Exists(input_fil + ".mm" + z + input_ext))  // image with mipmap: input.png -> input.mm1.png -> input.mm2.png
                {
                    byte[] bmp_mipmap = Convert_to_bmp((Bitmap)Bitmap.FromFile(input_fil + ".mm" + z + input_ext));
                    if (bmp_mipmap[0x15] != 0 || bmp_mipmap[0x14] != 0 || bmp_mipmap[0x19] != 0 || bmp_mipmap[0x18] != 0)
                    {
                        Console.WriteLine("Textures Dimensions are too high for a TEX0. Maximum dimensions are the values of a 2 bytes integer (65535 x 65535)");
                        return;
                    }
                    /***** BMP File Process *****/
                    // process the bmp file
                    int bmp_size = bmp_mipmap[2] | bmp_mipmap[3] << 8 | bmp_mipmap[4] << 16 | bmp_mipmap[5] << 24;
                    int pixel_start_offset = bmp_mipmap[10] | bmp_mipmap[11] << 8 | bmp_mipmap[12] << 16 | bmp_mipmap[13] << 24;
                    bitmap_width = (ushort)(bmp_mipmap[0x13] << 8 | bmp_mipmap[0x12]);
                    bitmap_height = (ushort)(bmp_mipmap[0x17] << 8 | bmp_mipmap[0x16]);
                    pixel_count = bitmap_width * bitmap_height;
                    user_palette = true; // won't edit palette with mipmaps
                    object w = create_PLT0(bmp_mipmap, bmp_size, pixel_start_offset);
                    index_list.Add((List<byte[]>)w);
                }
                else
                {
                    bitmap_width >>= 1; // divides by 2
                    bitmap_height >>= 1; // divides by 2   - also YES 1 DIVIDED BY TWO IS ZERO
                    if (bitmap_width == 0 || bitmap_height == 0)
                    {
                        Console.WriteLine("Too many mipmaps. " + (z - 1) + " is the maximum for this file");
                        exit = true;
                        return;
                    }
                    byte[] bmp_mipmap = Convert_to_bmp(ResizeImage((Bitmap)Bitmap.FromFile(input_file), bitmap_width, bitmap_height));


                    if (bmp_mipmap[0x15] != 0 || bmp_mipmap[0x14] != 0 || bmp_mipmap[0x19] != 0 || bmp_mipmap[0x18] != 0)
                    {
                        Console.WriteLine("Textures Dimensions are too high for a TEX0. Maximum dimensions are the values of a 2 bytes integer (65535 x 65535)");
                        return;
                    }
                    /***** BMP File Process *****/
                    // process the bmp file
                    int bmp_size = bmp_mipmap[2] | bmp_mipmap[3] << 8 | bmp_mipmap[4] << 16 | bmp_mipmap[5] << 24;
                    int pixel_start_offset = bmp_mipmap[10] | bmp_mipmap[11] << 8 | bmp_mipmap[12] << 16 | bmp_mipmap[13] << 24;
                    bitmap_width = (ushort)(bmp_mipmap[0x13] << 8 | bmp_mipmap[0x12]);
                    bitmap_height = (ushort)(bmp_mipmap[0x17] << 8 | bmp_mipmap[0x16]);
                    pixel_count = bitmap_width * bitmap_height;
                    user_palette = true; // won't edit palette with mipmaps
                    object w = create_PLT0(bmp_mipmap, bmp_size, pixel_start_offset);
                    index_list.Add((List<byte[]>)w);
                }
            }
            if (exit)
            {
                return;
            }
            bitmap_width = vanilla_size[0];
            bitmap_height = vanilla_size[1];
            if (bti || bmd)
            {
                if (bmd && !bmd_file)
                {
                    Console.WriteLine("specified bmd output but no bmd file given");
                    return;
                }
                write_BTI(index_list);
            }
            else if (tpl)
            {
                write_TPL(index_list);
            }
            else if (bmp || png || tif || tiff || ico || jpg || jpeg || gif)
            {
                write_BMP(index_list);
            }
            else
            {
                if (has_palette)
                {
                    write_PLT0();
                }
                write_TEX0(index_list);
            }

            /* catch (Exception ex)  // remove this when debugging else it'll tell you every error were at this line lol
            {
                if (ex.Message == "Out of memory." && ex.Source == "System.Drawing")
                    Console.WriteLine("Image input format not supported (convert it to jpeg or png)");
                else if (safe_mode)
                {
                    Console.WriteLine("error on " + input_file + "\nsafe mode is enabled, this program will exit silently");
                }
                else
                {
                    Console.WriteLine("\n\nPlease report this error on github https://github.com/yoshi2999/plt0/issues or to yosh#0304 on discord. Preferably with the file you used and the full command causing this.\n\nThe command you used is : \n" + args.ToString());
                    throw ex;
                }
                return;
            }*/

        }
        public void check_exit()
        {
            if (ask_exit)
            {
                Console.WriteLine("\nPress enter to exit...");
                Console.ReadLine();
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

        public void fill_palette(byte[] BGRA_array, int pixel_start_offset, int array_size)
        {
            byte red, green, blue, a;
            switch (palette_format_int32[3])  // fills the colour table
            {
                case 0: // AI8
                    {
                        switch (algorithm)
                        {
                            case 0: // cie_601
                                {
                                    for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
                                    {
                                        colour_palette[j] = (BGRA_array[i + 3]);  // alpha value
                                        if (BGRA_array[i + 3] != 0)
                                        {
                                            colour_palette[j + 1] = (byte)(BGRA_array[i] * 0.114 + BGRA_array[i + 1] * 0.587 + BGRA_array[i + 2] * 0.299);
                                        }
                                    }
                                    break;
                                }
                            case 1: // cie_709
                                {
                                    for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
                                    {
                                        colour_palette[j] = (BGRA_array[i + 3]);  // alpha value
                                        if (BGRA_array[i + 3] != 0)
                                        {
                                            colour_palette[j + 1] = (byte)(BGRA_array[i] * 0.0721 + BGRA_array[i + 1] * 0.7154 + BGRA_array[i + 2] * 0.2125);
                                        }
                                    }
                                    break;
                                }
                            case 2:  // custom
                                {
                                    for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
                                    {
                                        colour_palette[j] = (BGRA_array[i + 3]);  // alpha value
                                        if (BGRA_array[i + 3] != 0)
                                        {
                                            colour_palette[j + 1] = (byte)(BGRA_array[i] * custom_rgba[2] + BGRA_array[i + 1] * custom_rgba[1] + BGRA_array[i + 2] * custom_rgba[0]);
                                        }
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
                                    for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                    {
                                        red = (byte)(BGRA_array[i + 2] * custom_rgba[0]);
                                        green = (byte)(BGRA_array[i + 1] * custom_rgba[1]);
                                        blue = (byte)(BGRA_array[i] * custom_rgba[2]);
                                        if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                        {
                                            red += 8;
                                        }
                                        if ((green & round6) == round6 && green < 252)  // 6-bit max value on a trimmed byte
                                        {
                                            green += 4;
                                        }
                                        if ((blue & 7) > round5 && blue < 248)
                                        {
                                            blue += 8;
                                        }
                                        // pixel = (ushort)((((byte)(red) >> 3) << 11) + (((byte)(green) >> 2) << 5) + (byte)(blue) >> 3);
                                        colour_palette[j] = (byte)((red & 0xf8) | (green >> 5));
                                        colour_palette[j + 1] = (byte)(((green << 3) & 0xe0) | (blue >> 3));
                                    }
                                    break;
                                }
                            default: // RRRR RGGG GGGB BBBB
                                {
                                    for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                    {
                                        red = BGRA_array[i + 2];
                                        green = BGRA_array[i + 1];
                                        blue = BGRA_array[i];
                                        if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                        {
                                            red += 8;
                                        }
                                        if ((green & round6) == round6 && green < 252)  // 6-bit max value on a trimmed byte
                                        {
                                            green += 4;
                                        }
                                        if ((blue & 7) > round5 && blue < 248)
                                        {
                                            blue += 8;
                                        }
                                        // pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3));
                                        colour_palette[j] = (byte)((red & 0xf8) | (green >> 5));
                                        colour_palette[j + 1] = (byte)(((green << 3) & 0xe0) | (blue >> 3));
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
                                        for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                        {
                                            a = (byte)(BGRA_array[i + 3] * custom_rgba[3]);
                                            red = (byte)(BGRA_array[i + 2] * custom_rgba[0]);
                                            green = (byte)(BGRA_array[i + 1] * custom_rgba[1]);
                                            blue = (byte)(BGRA_array[i] * custom_rgba[2]);
                                            if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                            {
                                                a += 32;
                                            }
                                            if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                            {
                                                red += 16;
                                            }
                                            if ((green & 15) > round4 && green < 240)
                                            {
                                                green += 16;
                                            }
                                            if ((blue & 15) > round4 && blue < 240)
                                            {
                                                blue += 16;
                                            }
                                            // pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                            colour_palette[j] = (byte)(((a >> 1) & 0x70) | (red >> 4));
                                            colour_palette[j + 1] = (byte)((green & 0xf0) | (blue >> 4));
                                        }
                                    }
                                    else if (alpha == 0)  // 1RRR RRGG GGGB BBBB
                                    {
                                        for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                        {
                                            red = (byte)(BGRA_array[i + 2] * custom_rgba[0]);
                                            green = (byte)(BGRA_array[i + 1] * custom_rgba[1]);
                                            blue = (byte)(BGRA_array[i] * custom_rgba[2]);
                                            if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                            {
                                                red += 8;
                                            }
                                            if ((green & 7) > round5 && green < 248)
                                            {
                                                green += 8;
                                            }
                                            if ((blue & 7) > round5 && blue < 248)
                                            {
                                                blue += 8;
                                            }
                                            // pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                            colour_palette[j] = (byte)(0x80 | ((red >> 1) & 0x7C) | (green >> 6));
                                            colour_palette[j + 1] = (byte)(((green & 0x38) << 2) | (blue >> 3));
                                        }
                                    }
                                    else  // check for each colour if alpha trimmed to 3 bits is 255
                                    {
                                        for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                        {
                                            a = (byte)(BGRA_array[i + 3] * custom_rgba[3]);
                                            red = (byte)(BGRA_array[i + 2] * custom_rgba[0]);
                                            green = (byte)(BGRA_array[i + 1] * custom_rgba[1]);
                                            blue = (byte)(BGRA_array[i] * custom_rgba[2]);
                                            if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                            {
                                                a += 32;
                                            }
                                            if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                            {
                                                red += 16;
                                            }
                                            if ((green & 15) > round4 && green < 240)
                                            {
                                                green += 16;
                                            }
                                            if ((blue & 15) > round4 && blue < 240)
                                            {
                                                blue += 16;
                                            }
                                            if (a > 223)  // no alpha
                                            {
                                                colour_palette[j] = (byte)(0x80 | ((red >> 1) & 0x7C) | (green >> 6));
                                                colour_palette[j + 1] = (byte)(((green & 0x38) << 2) | (blue >> 3));
                                            }
                                            else
                                            {
                                                colour_palette[j] = (byte)(((a >> 1) & 0x70) | (red >> 4));
                                                colour_palette[j + 1] = (byte)((green & 0xf0) | (blue >> 4));
                                            }
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    if (alpha == 1)  // 0AAA RRRR GGGG BBBB
                                    {
                                        for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                        {
                                            a = BGRA_array[i + 3];
                                            red = BGRA_array[i + 2];
                                            green = BGRA_array[i + 1];
                                            blue = BGRA_array[i];
                                            if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                            {
                                                a += 32;
                                            }
                                            if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                            {
                                                red += 16;
                                            }
                                            if ((green & 15) > round4 && green < 240)
                                            {
                                                green += 16;
                                            }
                                            if ((blue & 15) > round4 && blue < 240)
                                            {
                                                blue += 16;
                                            }
                                            // pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                            colour_palette[j] = (byte)(((a >> 1) & 0x70) | (red >> 4));
                                            colour_palette[j + 1] = (byte)((green & 0xf0) | (blue >> 4));
                                        }
                                    }
                                    else if (alpha == 0)  // 1RRR RRGG GGGB BBBB
                                    {
                                        for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                        {
                                            red = BGRA_array[i + 2];
                                            green = BGRA_array[i + 1];
                                            blue = BGRA_array[i];
                                            if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                            {
                                                red += 8;
                                            }
                                            if ((green & 7) > round5 && green < 248)
                                            {
                                                green += 8;
                                            }
                                            if ((blue & 7) > round5 && blue < 248)
                                            {
                                                blue += 8;
                                            }
                                            // pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                            colour_palette[j] = (byte)(0x80 | ((red >> 1) & 0x7C) | (green >> 6));
                                            colour_palette[j + 1] = (byte)(((green & 0x38) << 2) | (blue >> 3));
                                        }
                                    }
                                    else  // mix up alpha and no alpha
                                    {
                                        for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                        {
                                            a = BGRA_array[i + 3];
                                            red = BGRA_array[i + 2];
                                            green = BGRA_array[i + 1];
                                            blue = BGRA_array[i];
                                            if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                            {
                                                a += 32;
                                            }
                                            if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                            {
                                                red += 16;
                                            }
                                            if ((green & 15) > round4 && green < 240)
                                            {
                                                green += 16;
                                            }
                                            if ((blue & 15) > round4 && blue < 240)
                                            {
                                                blue += 16;
                                            }
                                            if (a > 223)  // no alpha
                                            {
                                                colour_palette[j] = (byte)(0x80 | ((red >> 1) & 0x7C) | (green >> 6));
                                                colour_palette[j + 1] = (byte)(((green & 0x38) << 2) | (blue >> 3));
                                            }
                                            else
                                            {
                                                colour_palette[j] = (byte)(((a >> 1) & 0x70) | (red >> 4));
                                                colour_palette[j + 1] = (byte)((green & 0xf0) | (blue >> 4));
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        public void write_BMP(List<List<byte[]>> index_list)  // index_list contains all mipmaps.
        {
            byte padding = (byte)(4 - (bitmap_width % 4));
            if (padding == 4)
            {
                padding = 0;
            }
            ushort ref_width = bitmap_width;
            switch (texture_format_int32[3])
            {
                case 0:
                case 0xE:
                case 8: // CI4
                    {
                        ref_width >>= 1;  // 4 bits per pixel
                        break;
                    }
                case 3:
                case 4:
                case 5:
                case 10:
                    {
                        ref_width <<= 1;  // 16 bits per pixel
                        break;
                    }
                case 6:
                    {
                        ref_width <<= 2; // 32 bits per pixel
                        break;
                    }
            }
            int image_size = ((ref_width + padding) * bitmap_height);
            int pixel_start_offset = 0x36 + colour_number_x4;
            int size = pixel_start_offset + image_size;  // fixed size at 1 image
            // int size2 = pixel_start_offset + image_size;  // plus the header?????? added it twice lol
            ushort width = 0;  // will change when equal to 4 or 16 bit because of bypass lol
            ushort header_width = 0; // width written in the header
            ushort height = 0;
            int index;
            byte[] data = new byte[54];  // header data
            byte[] palette = new byte[colour_number_x4];
            string end = ".bmp";
            bool done = false;
            for (z = 0; z < mipmaps_number + 1; z++)
            {
                byte[] pixel = new byte[image_size];
                if (z == 0)
                {
                    width = ref_width;
                    header_width = bitmap_width;
                    height = bitmap_height;
                }
                else
                {
                    width >>= 1;  // divides by 2
                    header_width >>= 1;  // divides by 2
                    height >>= 1; // divides by 2
                }
                index = 0;
                // fill header
                data[0] = 0x42;  // B
                data[1] = 0x4D;  // M
                data[2] = (byte)(size);
                data[3] = (byte)(size >> 8);
                data[4] = (byte)(size >> 16);
                data[5] = (byte)(size >> 24);
                data[6] = 0x79; // y
                data[7] = 0x6F; // o
                data[8] = 0x73; // s
                data[9] = 0x68; // h
                data[10] = (byte)(pixel_start_offset);
                data[11] = (byte)(pixel_start_offset >> 8);
                data[12] = (byte)(pixel_start_offset >> 16);
                data[13] = (byte)(pixel_start_offset >> 24);
                data[14] = 0x28;  // DIB header size
                data[15] = 0;
                data[16] = 0;
                data[17] = 0;
                data[18] = (byte)(header_width);
                data[19] = (byte)(header_width >> 8);
                data[20] = 0;  // width can't exceed 65535 because it's stored as ushort in tex0/tpl/bti
                data[21] = 0;  // trust me, you don't want to have an image that is 24GB decompressed, neither in your ram lol
                data[22] = (byte)(height);
                data[23] = (byte)(height >> 8);
                data[24] = 0;
                data[25] = 0; // fourth byte of height
                data[26] = 1; // always 1
                data[27] = 0; // always 0
                switch (texture_format_int32[3])
                {
                    case 0:  //I4
                    case 0xE:  // CMPR
                    case 8: // CI4
                        {
                            data[28] = 4;  // 4-bit per pixel
                            break;
                        }
                    case 9:  // CI8
                    case 1: // I8
                    case 2: // AI4
                        {
                            data[28] = 8;  // 8-bit per pixel
                            break;
                        }
                    case 3:  // IA8
                    case 4:  // RGB565
                    case 5:  // RGB5A3
                    case 10: // CI14x2
                        {
                            data[28] = 16;  // 16-bit per pixel
                            break;
                        }
                    case 6:  // RGBA32
                        {
                            data[28] = 32; // 32 bits per pixel
                            break;
                        }
                }
                data[29] = 0; // second bit of depth
                data[30] = 0; // compression
                data[31] = 0; // compression
                data[32] = 0; // compression
                data[33] = 0; // compression
                data[34] = (byte)(image_size);
                data[35] = (byte)(image_size >> 8);
                data[36] = (byte)(image_size >> 16);
                data[37] = (byte)(image_size >> 24);
                data[38] = 0;
                data[39] = 0;
                data[40] = 0;
                data[41] = 0;  // some resolution unused setting
                data[42] = 0;
                data[43] = 0;
                data[44] = 0;
                data[45] = 0;  // some resolution unused setting
                data[47] = 0;
                data[48] = 0;
                data[49] = 0;
                data[50] = 0;
                data[51] = 0;
                data[52] = 0;
                data[53] = 0;
                if (has_palette)
                {
                    data[46] = (byte)(colour_number);
                    data[47] = (byte)(colour_number >> 8);
                    data[48] = 0; // colour_number is stored on 2 bytes
                    data[49] = 0;
                    data[50] = (byte)(colour_number);
                    data[51] = (byte)(colour_number >> 8);
                    data[52] = 0; // colour_number is stored on 2 bytes
                    data[53] = 0;
                    // fill palette data
                    // plt0 palettes are 2 bytes per colour, while bmp palette is 4 bytes per colour.
                    switch (palette_format_int32[3])
                    {
                        case 0: // AI8
                            {
                                for (int j = 0, k = 0; j < colour_number_x2; j += 2, k += 4)
                                {
                                    palette[k] = colour_palette[j + 1];    // this is the formula for black and white lol
                                    palette[k + 1] = colour_palette[j + 1];
                                    palette[k + 2] = colour_palette[j + 1];
                                    palette[k + 3] = colour_palette[j];  // Alpha
                                }
                                break;
                            }
                        case 1: // RGB565
                            {
                                for (int j = 0, k = 0; j < colour_number_x2; j += 2, k += 4)
                                {
                                    palette[k] = (byte)(colour_palette[j + 1] << 3);  // Blue
                                    palette[k + 1] = (byte)(((colour_palette[j] << 5) | (colour_palette[j + 1] >> 3)) & 0xfc);  // Green
                                    palette[k + 2] = (byte)(colour_palette[j] & 0xf8);  // Red
                                    palette[k + 3] = 0xff;  // No Alpha
                                }
                                break;
                            }
                        case 2:  // RGB5A3
                            {
                                if (alpha == 1)  // alpha
                                {

                                    for (int j = 0, k = 0; j < colour_number_x2; j += 2, k += 4)
                                    {  // 0AAA RRRR   GGGG BBBB
                                        palette[k] = (byte)(colour_palette[j + 1] << 4);  // Blue
                                        palette[k + 1] = (byte)(colour_palette[j + 1] & 0xf0);  // Green
                                        palette[k + 2] = (byte)((colour_palette[j] << 4) & 0xf0);  // Red
                                        palette[k + 3] = (byte)((colour_palette[j] << 1) & 0xe0);  // Alpha
                                    }
                                }
                                else if (alpha == 0)  // no alpha
                                {
                                    for (int j = 0, k = 0; j < colour_number_x2; j += 2, k += 4)
                                    {  // 1RRR RRGG   GGGB BBBB
                                        palette[k] = (byte)(colour_palette[j + 1] << 3);  // Blue
                                        palette[k + 1] = (byte)(((colour_palette[j] << 6) | (colour_palette[j + 1] >> 2)) & 0xf8);  // Green
                                        palette[k + 2] = (byte)((colour_palette[j] << 1) & 0xf8);  // Red
                                        palette[k + 3] = 0xff;  // No Alpha
                                    }
                                }
                                else  // mix
                                {
                                    for (int j = 0, k = 0; j < colour_number_x2; j += 2, k += 4)
                                    {
                                        if (colour_palette[j] >> 7 == 0)  // if it has alpha
                                        {  // 0AAA RRRR   GGGG BBBB
                                            palette[k] = (byte)(colour_palette[j + 1] << 4);  // Blue
                                            palette[k + 1] = (byte)(colour_palette[j + 1] & 0xf0);  // Green
                                            palette[k + 2] = (byte)((colour_palette[j] << 4) & 0xf0);  // Red
                                            palette[k + 3] = (byte)((colour_palette[j] << 1) & 0xe0);  // Alpha
                                        }
                                        else  // no alpha
                                        {  // 1RRR RRGG   GGGB BBBB
                                            palette[k] = (byte)(colour_palette[j + 1] << 3);  // Blue
                                            palette[k + 1] = (byte)(((colour_palette[j] << 6) | (colour_palette[j + 1] >> 2)) & 0xf8);  // Green
                                            palette[k + 2] = (byte)((colour_palette[j] << 1) & 0xf8);  // Red
                                            palette[k + 3] = 0xff;  // No Alpha
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
                if (has_palette || funky) // && (texture_format_int32[3] > 2 || texture_format_int32[3] != 0xe)))
                {
                    // fill pixel data
                    for (int j = 0; j < height; j++)
                    {
                        for (int k = 0; k < width; k++, index++)
                        {
                            pixel[index] = index_list[z][j][k];
                        }
                        for (int k = 0; k < padding; k++, index++)
                        {
                            pixel[index] = 0x69;  // my signature XDDDD 
                        }
                    }
                }
                else
                {
                    // fill pixel data
                    for (int j = 0; j < height; j++)
                    {
                        for (int k = 0; k < width; k++, index++)
                        {
                            pixel[index] = index_list[z][j][k];
                        }
                        for (int k = 0; k < padding; k++, index++)
                        {
                            pixel[index] = 0x69;  // my signature XDDDD 
                        }
                    }
                }
                if (z != 0)
                {
                    end = ".mm" + z + ".bmp";
                }
                index = 0;
                done = false;
                while (!done)
                {

                    try
                    {
                        FileMode mode = System.IO.FileMode.CreateNew;
                        if (System.IO.File.Exists(output_file + end))
                        {
                            mode = System.IO.FileMode.Truncate;
                            if (warn)
                            {
                                Console.WriteLine("Press enter to overwrite " + output_file + end);
                                Console.ReadLine();
                            }
                        }
                        using (System.IO.FileStream file = System.IO.File.Open(output_file + end, mode, System.IO.FileAccess.Write))
                        {
                            file.Write(data, 0, data.Length);
                            file.Write(palette, 0, colour_number_x4);
                            file.Write(pixel, 0, pixel.Length);
                            file.Close();
                            Console.WriteLine(output_file + end);
                        }
                        if (!bmp)
                        {

                            ConvertAndSave((Bitmap)Bitmap.FromFile(output_file + end), z);
                        }
                        done = true;
                    }
                    catch (Exception ex)
                    {
                        index += 1;
                        if (ex.Message.Substring(0, 34) == "The process cannot access the file")  // because it is being used by another process
                        {
                            if (index > 1)
                            {
                                output_file = output_file.Substring(output_file.Length - 2) + "-" + index;
                            }
                            else
                            {
                                output_file += "-" + index;
                            }
                        }
                        continue;
                    }
                }
                size = size >> 2;  // for next loop - divides by 4 because it's half the width size and half the height size
            }
        }

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
                    temp = bitmap_width / Math.Pow(2, i);
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
                    temp = bitmap_height / Math.Pow(2, i);
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
            data[2] = (byte)(bitmap_width >> 8);  // unsigned short width
            data[3] = (byte)bitmap_width; // second byte of width
            data[4] = (byte)(bitmap_height >> 8);  // height
            data[5] = (byte)bitmap_height;
            data[6] = WrapS; // sideways wrap
            data[7] = WrapT; // vertical wrap
            if (has_palette)
            {
                data[8] = 1; // well, 1 means palette enabled, and the whole purpose of this tool is to make images with palette LMAO
                data[9] = palette_format_int32[3]; // pretty straightforward again
                data[10] = (byte)(colour_number >> 8);
                data[11] = (byte)(colour_number);  // number of colours
                data[12] = 0;
                data[13] = 0;
                data[14] = 0;
                data[15] = 32; // palette data address
            }
            else
            {

                data[8] = 0;  // well, I've changed my mind, I wanna make it better than wimgt 
                data[9] = 0; // pretty straightforward again
                data[10] = 0;
                data[11] = 0;  // number of colours
                data[12] = 0;
                data[13] = 0;
                data[14] = 0;
                data[15] = 0; // palette data address
            }
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
            data[23] = (byte)(mipmaps_number << 3);  // MaxLOD   << 3 is faster than * 8
            data[24] = (byte)(mipmaps_number + 1);  // number of images
            data[25] = 0x69; // my signature XDDDD, I couldn't figure out what this setting does, but it doesn't affect the gameplay at all, it looks like it is used as a version number.
            data[26] = 0; // how do I calculate LODBIAS
            data[27] = 0; // how do I calculate LODBIAS
            data[28] = 0;  // won't exceed the max number of colours in the colour table which is stored on 2 bytes + header size 0x20
            data[29] = (byte)((0x20 + colour_palette.Length) >> 16); // >> 16 is better than / 65536
            data[30] = (byte)((0x20 + colour_palette.Length) >> 8);  // >> 8 is better than / 256
            data[31] = (byte)(0x20 + colour_palette.Length);  // offset to image0 header, (byte) acts as %256
            // now's palette data, but it's already stored in colour_palette, so let's jump onto image data.
            byte[] tex_data = new byte[size - 0x20 - colour_palette.Length];
            int count = 0;
            int height;
            int width;
            block_width = (sbyte)(block_width / format_ratio);

            if (texture_format_int32[3] == 6)  // RGBA32 has a f-cking byte order
            {
                for (int i = 0; i < settings.Count; i++)  // mipmaps
                {
                    height = settings[i][1] * block_height - 1;
                    width = 0;
                    for (int j = settings[i][1]; j > 0; j--)  // number of height blocks  (remember that the last line was added first into index_list)
                    {
                        for (int k = 0; k < settings[i][0]; k++)  // number of width blocks
                        {
                            for (int h = 0; h < 4; h++)  // height in the block
                            {
                                for (int w = 0; w < 8; w++)  // width in the block
                                {
                                    tex_data[count] = index_list[i][height - h][width + w];  // adds the 8 ARAR ARAR formatted bytes for each line
                                    count++;
                                }
                            }
                            for (int h = 0; h < 4; h++)  // height in the block
                            {
                                for (int w = 8; w < 16; w++)  // width in the block
                                {
                                    tex_data[count] = index_list[i][height - h][width + w];  // adds the 8 GBGB GBGB formatted bytes for each line
                                    count++;
                                }
                            }
                            width += block_width;
                        } // end of the loop to go through number of horizontal blocks
                        height -= block_height;
                        width = 0;
                    }   // go through vertical blocks
                }      // go through mipmaps
            }
            else if (texture_format_int32[3] == 0xe) // yeah, I ordered CMPR by sub-blocks
            {
                for (int i = 0; i < settings.Count; i++)  // mipmaps
                {
                    height = settings[i][1] * block_height - 1;
                    width = 0;
                    for (int j = settings[i][1]; j > 0; j--)  // number of height blocks  (remember that the last line was added first into index_list)
                    {
                        for (int k = 0; k < settings[i][0]; k++)  // number of width blocks
                        {
                            for (int h = 0; h < 4; h++)  // number of sub-blocks
                            {
                                for (int w = 0; w < 8; w++)  // width in the sub-block
                                {
                                    tex_data[count] = index_list[i][height - h][width + w];  // adds the 8 CMPR formatted bytes for each line
                                    count++;
                                }
                            }
                            width += block_width;
                        } // end of the loop to go through number of horizontal blocks
                        height -= block_height;
                        width = 0;
                    }   // go through vertical blocks
                }      // go through mipmaps
            }
            else
                {
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
                }
            
            // finished to get everything of the bti
            // now we'll check if that bti needs to be injected in a bmd
            // I made extract-bti.py first, so I'm using it as a template
            // 0x14/20 bytes long
            /*
            TEX1 Header description
            0x00
            char[4] Magic_TEX1; //'TEX1'
            0x04
            int ChunkSize; //Total bytes of chunk
            0x08
            short TextureCount; //Number of textures
            0x0A
            short Padding; (Usually 0xFFFF)
            0x0C
            int TextureHeaderOffset; // (always 0x20) TextureCount bti image headers are stored here. Relative to TEX1Header start.
            0x10
            int StringTableOffset; //Stores one filename for each texture. Relative to TEX1Header start.
            */
            if (bmd_file)
            {
                if (has_palette)
                {
                    Console.WriteLine("Sorry man, I have not met any bmd file with a palette amongst all games I've reviewed so far. I'll be glad to add this feature if you find some files I could look into\n for now you can't use CI4, CI8 and CI14x2 in a bmd\n");
                    Console.WriteLine("Press enter to exit...");
                    Console.ReadLine();
                    return;
                }
                using (System.IO.FileStream file = System.IO.File.Open(input_file2, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    byte[] j3d = new byte[file.Length];
                    file.Read(j3d, 0, j3d.Length);
                    file.Close();
                    y = 0x20; // next section offset
                    while (!(j3d[y] == 'T' && j3d[y + 1] == 'E' && j3d[y + 2] == 'X' && j3d[y + 3] == '1'))  // section identifier
                    {
                        if (y + 9 > j3d.Length)
                        {
                            Console.WriteLine("Couldn't find the TEX1 Section. This bmd file is corrupted. the program will exit");
                            return;
                        }
                        y += (j3d[y + 4] << 24) + (j3d[y + 5] << 16) + (j3d[y + 6] << 8) + j3d[y + 7];  // size of current section
                    }
                    // now that we found TEX1 section address, we can find string pool table address, then get all texture names
                    int x = y + (j3d[y + 16] << 24) + (j3d[y + 17] << 16) + (j3d[y + 18] << 8) + j3d[y + 19];  // string pool table start offset
                    int string_pool_table_start_offset = x;
                    ushort string_count = (ushort)((j3d[x] << 8) + j3d[x + 1]);
                    int string_count_5 = string_count << 5;
                    if (string_count == 0)  // ARE YOU KIDDING ME
                    {
                        Console.WriteLine("I could have sworn you were smart enough to take a bmd with bti textures in it, but this one contains none.");
                    }
                    string name = "";
                    x += 4 + (string_count << 2); // 4 is the string pool table header size
                    // string count is multiplied by 4 because after the header there is a ushort hash and a ushort offset for each string
                    success = false;
                    ushort[] score = new ushort[string_count];
                    int f = 0;
                    ushort current_string = 0;
                    ushort best_score = 0;  // length of the biggest substring in common
                    ushort current = 0;
                    List<string> name_list = new List<string>();
                    while (current_string < string_count) // used to compare each texture name with the input file
                    {
                        while (j3d[x] != 0)  // 0 is supposed to be the byte separating strings. never seen another value in mkdd
                        {
                            name += j3d[x];
                            x++;
                        }
                        name_list.Add(name);
                        if (name == input_fil)
                        {
                            success = true;
                            break;
                        }
                        else  // use an algorithm to determine the MOST SIMILAR texture by file name
                        {
                            for (z = 0; z < name.Length - 1; z++)
                            {
                                for (pass = 0; pass < input_fil.Length; pass++)
                                {
                                    if (name[z] == input_fil[pass] && name[z + 1] == input_fil[pass])
                                    {
                                        break;
                                    }
                                }  // used to find the start of the similar substring
                                // no, don't say 1 byte length files won't work here. They're supposed to fall in the first if
                                for (f = z, current = 0; f < input_fil.Length && f < name.Length; f++, current++)
                                {
                                    if (input_fil[pass] != name[f])
                                    {
                                        break;
                                    }
                                }
                                // get the length of the common substring 
                                if (current > best_score)
                                {
                                    best_score = current;
                                }
                            }
                            score[current_string] = best_score;
                        }
                        current_string++;
                    }
                    if (!success)  // get the most fitting texture and ask for confirmation
                    {
                        best_score = 0;
                        f = 0;
                        for (z = 0; z < score.Length; z++)
                        {
                            if (score[z] < best_score)
                            {
                                best_score = score[z];
                                f = z;
                            }
                        }
                        name = name_list[f];  // most probable fitting texture by the file name
                        if (warn)
                        {
                            Console.WriteLine("File names doesn't exactly match. Press Enter to replace " + name + " inside " + input_file2);
                            Console.ReadLine();
                        }
                    }
                    List<int> data_offset_array = new List<int>();
                    List<int> data_size_array = new List<int>();
                    int blocks_wide;
                    int blocks_tall;
                    int curr_mipmap_size;
                    // f = bti header of the image to replace >> 5 (divided by 32, and relative to y)
                    // y = TEX1 section start offset
                    // z = current bti header offset (relative to y)
                    // 
                    for (z = 0; z < string_count_5; z += 32)
                    {
                        if ((z >> 5) == f)
                        {
                            data[12] = (byte)(((string_count_5 - z)) << 24);
                            data[13] = (byte)(((string_count_5 - z)) << 16);
                            data[14] = (byte)(((string_count_5 - z)) << 8);
                            data[15] = (byte)(((string_count_5 - z)));
                            x = y + (z + 1 * 0x20);
                        }
                        int data_offset = (j3d[y + z + 0x3C] << 24) + (j3d[y + z + 0x3D] << 16) + (j3d[y + z + 0x3E] << 8) + (j3d[y + z + 0x3F]);
                        data_offset_array.Add(data_offset);
                        blocks_wide = (j3d[y + 0x22 + z] + j3d[y + 0x23 + z] + block_width_array[j3d[y + 0x20 + z]]) / block_width_array[j3d[y + 0x20 + z]];
                        blocks_tall = (j3d[y + 0x24 + z] + j3d[y + 0x25 + z] + block_width_array[j3d[y + 0x20 + z]]) / block_height_array[j3d[y + 0x20 + z]];
                        int data_size = (blocks_wide * blocks_tall);
                        if (j3d[y + 0x20 + z] == 6)
                        {
                            data_size <<= 6;
                        }
                        else
                        {
                            data_size <<= 5;
                        }
                        curr_mipmap_size = data_size;
                        for (int i = 1; i < j3d[y + 0x38 + z]; i++)
                        {
                            curr_mipmap_size >>= 2;
                            data_size += curr_mipmap_size;
                        }
                        data_size_array.Add(data_size);
                        /*blocks_wide = (self.width + (self.block_width-1)) // self.block_width
                          blocks_tall = (self.height + (self.block_height-1)) // self.block_height
                          image_data_size = blocks_wide*blocks_tall*self.block_data_size
                          remaining_mipmaps = self.mipmap_count-1
                          curr_mipmap_size = image_data_size
                          while remaining_mipmaps > 0:
                          # Each mipmap is a quarter the size of the last (half the width and half the height).
                              curr_mipmap_size = curr_mipmap_size//4
                              image_data_size += curr_mipmap_size
                              remaining_mipmaps -= 1*/

                        // fix palette_offset inside bmd which is the offset to the end of all bti headers
                        j3d[y + 0x20 + (0x20 * z) + 12] = (byte)(((string_count_5 - z)) << 24);
                        j3d[y + 0x20 + (0x20 * z) + 13] = (byte)(((string_count_5 - z)) << 16);
                        j3d[y + 0x20 + (0x20 * z) + 14] = (byte)(((string_count_5 - z)) << 8);
                        j3d[y + 0x20 + (0x20 * z) + 15] = (byte)(((string_count_5 - z)));
                    }
                    // now let's remove the image data that we're replacing if it's unused
                    // no, I won't check for unused texture data apart from this one. these kind of things were generated by other softwares!
                    int start = y + string_count_5 + 0x20;  // bti headers end offset
                    int middle = data_offset_array[f] + ((string_count - f) << 5); // relative to start
                    int start_2 = y + string_count_5 + data_offset_array[f + 1] - ((string_count - f + 1) << 5);
                    for (int z = 0; z < string_count_5; z += 32)
                    {
                        if (data_offset_array[z >> 5] < middle)
                        {
                            if ((data_offset_array[z >> 5] + data_size_array[z >> 5]) > middle)
                            {
                                middle = (data_offset_array[z >> 5] + data_size_array[z >> 5]);
                                if (middle > start_2)
                                {
                                    start_2 = middle;
                                }
                            }
                        }
                    }
                    FileMode mode = System.IO.FileMode.CreateNew;
                    if (System.IO.File.Exists(output_file + ".bmd"))
                    {
                        mode = System.IO.FileMode.Truncate;
                        if (warn)
                        {
                            Console.WriteLine("Press enter to overwrite " + output_file + ".bmd");
                            Console.ReadLine();
                        }
                    }
                    using (System.IO.FileStream ofile = System.IO.File.Open(output_file + ".bmd", mode, System.IO.FileAccess.Write))
                    {
                        ofile.Write(j3d, 0, y);
                        // write texture headers
                        if (f != 0)
                        {
                            ofile.Write(j3d, y, f << 5);
                        }
                        ofile.Write(data, 0, 32);
                        if (f != string_count - 1)
                        {
                            ofile.Write(j3d, x, (string_count - f - 1) << 5);
                        }
                        if (middle == start_2)
                        {
                            ofile.Write(j3d, start, j3d.Length - start);
                        }
                        else
                        {
                            ofile.Write(j3d, start, middle);
                            ofile.Write(j3d, start_2, j3d.Length - start_2);
                        }
                        ofile.Close();
                        Console.WriteLine(output_file + ".bmd");
                    }
                }
            }
            else  // single bti
            {
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
        }

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
                    temp = bitmap_width / Math.Pow(2, i);
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
                    temp = bitmap_height / Math.Pow(2, i);
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
            data[13] = (byte)((0x20 + colour_palette.Length) >> 16);
            data[14] = (byte)((0x20 + colour_palette.Length) >> 8);
            data[15] = (byte)(0x20 + colour_palette.Length);  // offset to image0 header, (byte) acts as %256
            data[16] = 0;
            data[17] = 0;
            data[18] = 0;
            if (has_palette)
            {
                data[19] = 20;  // offset to palette0 header
                data[20] = (byte)(colour_number >> 8);
                data[21] = (byte)(colour_number);  // number of colours

                data[27] = palette_format_int32[3]; // palette format

                data[31] = 32; // palette data address
            }
            else
            {
                data[19] = 0;
                data[20] = 0;
                data[21] = 0;  // number of colours

                data[27] = 0; // palette format

                data[31] = 0; // palette data address

            }
            data[22] = 0;  // unpacked (0 I guess)
            data[23] = 0;  // padding
            data[24] = palette_format_int32[0];
            data[25] = palette_format_int32[1];
            data[26] = palette_format_int32[2]; // palette format
            data[28] = 0;
            data[29] = 0;
            data[30] = 0;
            // now's palette data, but it's already stored in colour_palette, so let's jump onto image header.
            header[0] = (byte)(bitmap_width >> 8);  // unsigned short width
            header[1] = (byte)bitmap_width; // second byte of width
            header[2] = (byte)(bitmap_height >> 8);  // height
            header[3] = (byte)bitmap_height;
            header[4] = texture_format_int32[0];
            header[5] = texture_format_int32[1];
            header[6] = texture_format_int32[2];
            header[7] = texture_format_int32[3];
            header[8] = 0; // won't exceed the max number of colours in the colour table which is stored on 2 bytes + header size 0x20
            header[9] = (byte)((0x60 + colour_palette.Length) >> 16);
            header[10] = (byte)((0x60 + colour_palette.Length) >> 8);
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

            if (texture_format_int32[3] == 6)  // RGBA32 has a f-cking byte order
            {
                for (int i = 0; i < settings.Count; i++)  // mipmaps
                {
                    height = settings[i][1] * block_height - 1;
                    width = 0;
                    for (int j = settings[i][1]; j > 0; j--)  // number of height blocks  (remember that the last line was added first into index_list)
                    {
                        for (int k = 0; k < settings[i][0]; k++)  // number of width blocks
                        {
                            for (int h = 0; h < 4; h++)  // height in the block
                            {
                                for (int w = 0; w < 8; w++)  // width in the block
                                {
                                    tex_data[count] = index_list[i][height - h][width + w];  // adds the 8 ARAR ARAR formatted bytes for each line
                                    count++;
                                }
                            }
                            for (int h = 0; h < 4; h++)  // height in the block
                            {
                                for (int w = 8; w < 16; w++)  // width in the block
                                {
                                    tex_data[count] = index_list[i][height - h][width + w];  // adds the 8 GBGB GBGB formatted bytes for each line
                                    count++;
                                }
                            }
                            width += block_width;
                        } // end of the loop to go through number of horizontal blocks
                        height -= block_height;
                        width = 0;
                    }   // go through vertical blocks
                }      // go through mipmaps
            }
            else if (texture_format_int32[3] == 0xe) // yeah, I ordered CMPR by sub-blocks
            {
                for (int i = 0; i < settings.Count; i++)  // mipmaps
                {
                    height = settings[i][1] * block_height - 1;
                    width = 0;
                    for (int j = settings[i][1]; j > 0; j--)  // number of height blocks  (remember that the last line was added first into index_list)
                    {
                        for (int k = 0; k < settings[i][0]; k++)  // number of width blocks
                        {
                            for (int h = 0; h < 4; h++)  // number of sub-blocks
                            {
                                for (int w = 0; w < 8; w++)  // width in the sub-block
                                {
                                    tex_data[count] = index_list[i][height - h][width + w];  // adds the 8 CMPR formatted bytes for each line
                                    count++;
                                }
                            }
                            width += block_width;
                        } // end of the loop to go through number of horizontal blocks
                        height -= block_height;
                        width = 0;
                    }   // go through vertical blocks
                }      // go through mipmaps
            }
            else
            {
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
            }
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
                file.Write(colour_palette, 0, colour_palette.Length);  // can I write nothing?
                file.Write(header, 0, 64);
                file.Write(tex_data, 0, tex_data.Length);
                file.Write(data2, 0, data2.Length);
                file.Close();
                Console.WriteLine(output_file + ".tpl");
            }
        }

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


        /// <summary>
        /// writes a TEX0 file from parameters of plt0_class and the list given in argument
        /// </summary>
        /// <param name="index_list">a List of mipmaps, first one being the highest quality one. <br/>
        /// each mipmap contains a list of every row of the image (starting by the bottom one). <br/>
        /// each row is actually a byte array of every pixel encoded in a specific format.</param>
        /// <returns>nothing. but writes the file into the output name given in CLI argument</returns>
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
                    temp = bitmap_width / Math.Pow(2, i);
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
                    temp = bitmap_height / Math.Pow(2, i);
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
            if (has_palette)
            {
                data[27] = 1;  // image has a palette
            }
            else
            {
                data[27] = 0; // image don't have a palette
            }
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
            for (int i = 48; i < 64; i++)  // undocumented bytes
            {
                data[i] = 0;
            }
            byte[] tex_data = new byte[size - 64];
            int count = 0;
            int height;
            int width;
            block_width = (sbyte)(block_width / format_ratio);
            if (texture_format_int32[3] == 6)  // RGBA32 has a f-cking byte order
            {
                for (int i = 0; i < settings.Count; i++)  // mipmaps
                {
                    height = settings[i][1] * block_height - 1;
                    width = 0;
                    for (int j = settings[i][1]; j > 0; j--)  // number of height blocks  (remember that the last line was added first into index_list)
                    {
                        for (int k = 0; k < settings[i][0]; k++)  // number of width blocks
                        {
                            for (int h = 0; h < 4; h++)  // height in the block
                            {
                                for (int w = 0; w < 8; w++)  // width in the block
                                {
                                    tex_data[count] = index_list[i][height - h][width + w];  // adds the 8 ARAR ARAR formatted bytes for each line
                                    count++;
                                }
                            }
                            for (int h = 0; h < 4; h++)  // height in the block
                            {
                                for (int w = 8; w < 16; w++)  // width in the block
                                {
                                    tex_data[count] = index_list[i][height - h][width + w];  // adds the 8 GBGB GBGB formatted bytes for each line
                                    count++;
                                }
                            }
                            width += block_width;
                        } // end of the loop to go through number of horizontal blocks
                        height -= block_height;
                        width = 0;
                    }   // go through vertical blocks
                }      // go through mipmaps
            }
            else if (texture_format_int32[3] == 0xe) // yeah, I ordered CMPR by sub-blocks
            {
                for (int i = 0; i < settings.Count; i++)  // mipmaps
                {
                    height = (settings[i][1] << 3) - 1;
                    for (int j = settings[i][1]; j > 0; j--)  // number of height blocks  (remember that the last line was added first into index_list)
                    {
                        for (int k = 0; k < settings[i][0]; k++)  // number of width blocks
                        {
                            for (int h = 0; h < 4; h++)  // number of sub-blocks
                            {
                                for (int w = 0; w < 8; w++)  // width in the sub-block
                                {
                                    tex_data[count] = index_list[i][height - h][w];  // adds the 8 CMPR formatted bytes for each line
                                    count++;
                                }
                            }
                        } // end of the loop to go through number of horizontal blocks
                        height -= 4;
                    }   // go through vertical blocks
                }      // go through mipmaps
            }
            else
            {
                for (int i = 0; i < settings.Count; i++)  // mipmaps
                {
                    height = settings[i][1] * block_height - 1;
                    width = 0;
                    for (int j = settings[i][1]; j > 0; j--)  // number of height blocks  (remember that the last line was added first into index_list)
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
            }
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

        /// <summary>
        /// Fills the colour palette and process every pixel of the image to be an index of the colour palette
        /// </summary>
        /// <param name="bmp_image">the raw bmp file as a byte array</param>
        /// <param name="bmp_filesize">the size of the file, it can be read from the array itself, it's also the length of the array</param>
        /// <param name="pixel_data_start_offset">read from the array itself</param>
        /// <returns>a list of each row of the image (starting by the bottom one) and each row is a byte array which contains every pixel of a row.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public List<byte[]> create_PLT0(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {
            ushort pixel = bitmap_width;
            if (bitmap_height % block_height != 0)
            {
                fill_height = true;
                if (bitmap_width % block_width != 0)
                {
                    Console.WriteLine("Height is not a multiple of " + block_height + " (this value changes for each format), just be aware that the file size would be the same with an image of dimensions " + (bitmap_width + (block_width - (bitmap_width % block_width))) + "x" + ((bitmap_height + (block_height - (bitmap_height % block_height)))));
                }
                else
                {
                    Console.WriteLine("Height is not a multiple of " + block_height + " (this value changes for each format), just be aware that the file size would be the same with an image of dimensions " + bitmap_width + "x" + ((bitmap_height + (block_height - (bitmap_height % block_height)))));
                }
            }
            if (bitmap_width % block_width != 0)
            {
                fill_width = true;
                if (fill_height)
                {
                    Console.WriteLine("Width is not a multiple of " + block_width + " (this value changes for each format), just be aware that the file size would be the same with an image of dimensions " + (bitmap_width + (block_width - (bitmap_width % block_width))) + "x" + ((bitmap_height + (block_height - (bitmap_height % block_height)))));
                }
                else
                {
                    Console.WriteLine("Width is not a multiple of " + block_width + " (this value changes for each format), just be aware that the file size would be the same with an image of dimensions " + (bitmap_width + (block_width - (bitmap_width % block_width))) + "x" + bitmap_height);
                }
            }
            // pixel = (ushort)(((ushort)(bitmap_width / block_width) + 1) * block_width);
            pixel = (ushort)(bitmap_width + ((block_width - (bitmap_width % block_width)) % block_width));
            switch (texture_format_int32[3])  // pixel *= reverse_format ratio XD
            {
                case 0:
                case 0xE:
                case 8: // CI4
                    {
                        pixel >>= 1;  // 4 bits per pixel
                        break;
                    }
                case 3:
                case 4:
                case 5:
                case 10:
                    {
                        pixel <<= 1;  // 16 bits per pixel
                        break;
                    }
                case 6:
                    {
                        pixel <<= 2; // 32 bits per pixel
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
            int j = fill_palette_start_offset;
            byte red;
            byte green;
            byte blue;
            byte a;
            byte grey;
            bool not_similar;
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            if (bmp_image[0x1C] != 32)
            {
                Console.WriteLine("HOLY SHIT (colour depth of the converted bmp image is " + bmp_image[0x1C] + ")");
                Environment.Exit(0);
            }
            // colour depth
            // 32-bit BGRA bmp image
            if (has_palette)
            {
                List<ushort> Colours = new List<ushort>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
                List<int[]> Colour_Table = new List<int[]>();  // {occurence, every possible salues of a short} used to find the most used colours, and then build a colour_palette from these
                int[] colour = { 0, 0 };
                for (int i = 0; i < 65536; i++)
                {
                    colour[1] = i;
                    Colour_Table.Add(colour.ToArray());  // adds a copy of the current array, so it won't be linked after changes on next loop
                }
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
                            if (!user_palette || fill_palette_start_offset != 0)  // no input palette / partial user input palette = fill it with these colours
                            {
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
                                            not_similar = true;
                                            for (int k = 0; k < j; k += 2)
                                            {
                                                if ((colour_palette[k] != (byte)(Colour_Table[i][1] >> 8)) || colour_palette[k + 1] != (byte)(Colour_Table[i][1]))
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
                                    }
                                }
                            }
                            if (has_palette)
                            {
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
                                                            diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            diff = (short)(Math.Abs(colour_palette[i] - (byte)(Colours[j] >> 8)) + Math.Abs(colour_palette[i + 1] - (byte)Colours[j]));
                                                            if (diff < diff_min)
                                                            {
                                                                diff_min = diff;
                                                                diff_min_index = (byte)(i >> 1);
                                                            }
                                                        }
                                                    }
                                                    if (w % 2 == 0)  // stores the index on the upper 4 bits
                                                    {
                                                        index[w >> 1] = (byte)(diff_min_index << 4);
                                                    }
                                                    else  // stores the index on the lower 4 bits
                                                    {
                                                        index[w >> 1] += diff_min_index;
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
                                                        index[pixel >> 1] = 0;
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
                                                            diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            diff = (short)(Math.Abs(colour_palette[i] - (byte)(Colours[j] >> 8)) + Math.Abs(colour_palette[i + 1] - (byte)Colours[j]));
                                                            if (diff < diff_min)
                                                            {
                                                                diff_min = diff;
                                                                diff_min_index = (byte)(i >> 1);
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
                                                for (int w = 0; w < bitmap_width << 1; w += 2)  // multiplied by two because each index is a 14 bytes integer
                                                {
                                                    diff_min = 500;
                                                    for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                    {
                                                        if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                        {
                                                            diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            diff = (short)(Math.Abs(colour_palette[i] - (byte)(Colours[j] >> 8)) + Math.Abs(colour_palette[i + 1] - (byte)Colours[j]));
                                                            if (diff < diff_min)
                                                            {
                                                                diff_min = diff;
                                                                diff_min_index = (byte)(i >> 1);
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
                                                        index[pixel << 1] = 0;
                                                        index[pixel << 1 + 1] = 0;
                                                    }
                                                }
                                                index_list.Add(index.ToArray());
                                            }
                                            break;
                                        }
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
                                            if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                            {
                                                red += 8;
                                            }
                                            if ((green & round6) == round6 && green < 252)  // 6-bit max value on a trimmed byte
                                            {
                                                green += 4;
                                            }
                                            if ((blue & 7) > round5 && blue < 248)
                                            {
                                                blue += 8;
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
                                            if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                            {
                                                red += 8;
                                            }
                                            if ((green & round6) == round6 && green < 252)  // 6-bit max value on a trimmed byte
                                            {
                                                green += 4;
                                            }
                                            if ((blue & 7) > round5 && blue < 248)
                                            {
                                                blue += 8;
                                            }
                                            pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3));
                                            Colours.Add(pixel);
                                            Colour_Table[pixel][0] += 1;
                                        }
                                        break;
                                    }
                            }
                            if (!user_palette || fill_palette_start_offset != 0)  // no input palette / partial user input palette = fill it with these colours
                            {
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
                                            not_similar = true;
                                            for (int k = 0; k < j; k += 2)
                                            {
                                                if ((colour_palette[k] != (byte)(Colour_Table[i][1] >> 8)) || colour_palette[k + 1] != (byte)(Colour_Table[i][1]))
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
                                    }
                                }
                            }
                            if (has_palette)
                            {
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
                                                            diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                            break;
                                                        }
                                                        else  // calculate difference between each separate colour channel and store the sum
                                                        {
                                                            diff = (short)(Math.Abs((colour_palette[i] & 248) - ((Colours[j] >> 8) & 248)) + Math.Abs(((colour_palette[i] & 7) << 5) + ((colour_palette[i + 1] >> 3) & 28) - ((Colours[j] >> 3) & 252)) + Math.Abs(((colour_palette[i + 1] << 3) & 248) - ((Colours[j] << 3) & 248)));
                                                            if (diff < diff_min)
                                                            {
                                                                diff_min = diff;
                                                                diff_min_index = (byte)(i >> 1);
                                                            }
                                                        }
                                                    }
                                                    if (w % 2 == 0)  // stores the index on the upper 4 bits
                                                    {
                                                        index[w >> 1] = (byte)(diff_min_index << 4);
                                                    }
                                                    else  // stores the index on the lower 4 bits
                                                    {
                                                        index[w >> 1] += diff_min_index;
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
                                                        index[pixel >> 1] = 0;
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
                                                            diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                            break;
                                                        }
                                                        else  // calculate difference between each separate colour channel and store the sum
                                                        {

                                                            diff = (short)(Math.Abs((colour_palette[i] & 248) - ((Colours[j] >> 8) & 248)) + Math.Abs(((colour_palette[i] & 7) << 5) + ((colour_palette[i + 1] >> 3) & 28) - ((Colours[j] >> 3) & 252)) + Math.Abs(((colour_palette[i + 1] << 3) & 248) - ((Colours[j] << 3) & 248))); if (diff < diff_min)
                                                            {
                                                                diff_min = diff;
                                                                diff_min_index = (byte)(i >> 1);
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
                                                for (int w = 0; w < bitmap_width << 1; w += 2)  // multiplied by two because each index is a 14 bytes integer
                                                {
                                                    diff_min = 500;
                                                    for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                    {
                                                        if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                        {
                                                            diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                            break;
                                                        }
                                                        else  // calculate difference between each separate colour channel and store the sum
                                                        {

                                                            diff = (short)(Math.Abs((colour_palette[i] & 248) - ((Colours[j] >> 8) & 248)) + Math.Abs(((colour_palette[i] & 7) << 5) + ((colour_palette[i + 1] >> 3) & 28) - ((Colours[j] >> 3) & 252)) + Math.Abs(((colour_palette[i + 1] << 3) & 248) - ((Colours[j] << 3) & 248))); if (diff < diff_min)
                                                            {
                                                                diff_min = diff;
                                                                diff_min_index = (byte)(i >> 1);
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
                                                        index[pixel << 1] = 0;
                                                        index[pixel << 1 + 1] = 0;
                                                    }
                                                }
                                                index_list.Add(index.ToArray());
                                            }
                                            break;
                                        }
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
                                                if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                                {
                                                    a += 32;
                                                }
                                                if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                                {
                                                    red += 16;
                                                }
                                                if ((green & 15) > round4 && green < 240)
                                                {
                                                    green += 16;
                                                }
                                                if ((blue & 15) > round4 && blue < 240)
                                                {
                                                    blue += 16;
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
                                                if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                                {
                                                    red += 8;
                                                }
                                                if ((green & 7) > round5 && green < 248)
                                                {
                                                    green += 8;
                                                }
                                                if ((blue & 7) > round5 && blue < 248)
                                                {
                                                    blue += 8;
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
                                                if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                                {
                                                    a += 32;
                                                }
                                                if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                                {
                                                    red += 16;
                                                }
                                                if ((green & 15) > round4 && green < 240)
                                                {
                                                    green += 16;
                                                }
                                                if ((blue & 15) > round4 && blue < 240)
                                                {
                                                    blue += 16;
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
                                                if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                                {
                                                    a += 32;
                                                }
                                                if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                                {
                                                    red += 16;
                                                }
                                                if ((green & 15) > round4 && green < 240)
                                                {
                                                    green += 16;
                                                }
                                                if ((blue & 15) > round4 && blue < 240)
                                                {
                                                    blue += 16;
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
                                                if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                                {
                                                    red += 8;
                                                }
                                                if ((green & 7) > round5 && green < 248)
                                                {
                                                    green += 8;
                                                }
                                                if ((blue & 7) > round5 && blue < 248)
                                                {
                                                    blue += 8;
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
                                                if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                                {
                                                    a += 32;
                                                }
                                                if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                                {
                                                    red += 16;
                                                }
                                                if ((green & 15) > round4 && green < 240)
                                                {
                                                    green += 16;
                                                }
                                                if ((blue & 15) > round4 && blue < 240)
                                                {
                                                    blue += 16;
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
                            byte a2;
                            byte red2;
                            byte green2;
                            byte blue2;
                            if (!user_palette || fill_palette_start_offset != 0)  // no input palette / partial user input palette = fill it with these colours
                            {

                                Colour_Table.Sort(new IntArrayComparer());  // sorts the table by the most used colour first
                                Console.WriteLine("findind most used Colours");
                                if (alpha == 1)
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
                                                not_similar = true;
                                                for (int k = 0; k < j; k += 2)
                                                {
                                                    if ((colour_palette[k] != (byte)(Colour_Table[i][1] >> 8)) || colour_palette[k + 1] != (byte)(Colour_Table[i][1]))
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
                                                not_similar = true;
                                                for (int k = 0; k < j; k += 2)
                                                {
                                                    if ((colour_palette[k] != (byte)(Colour_Table[i][1] >> 8)) || colour_palette[k + 1] != (byte)(Colour_Table[i][1]))
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
                                                not_similar = true;
                                                for (int k = 0; k < j; k += 2)
                                                {
                                                    if ((colour_palette[k] != (byte)(Colour_Table[i][1] >> 8)) || colour_palette[k + 1] != (byte)(Colour_Table[i][1]))
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
                                        }
                                    }
                                }
                            }
                            if (has_palette)
                            {
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
                                                            diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
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
                                                                diff_min_index = (byte)(i >> 1);
                                                            }
                                                        }
                                                    }
                                                    if (w % 2 == 0)  // stores the index on the upper 4 bits
                                                    {
                                                        index[w >> 1] = (byte)(diff_min_index << 4);
                                                    }
                                                    else  // stores the index on the lower 4 bits
                                                    {
                                                        index[w >> 1] += diff_min_index;
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
                                                        index[pixel >> 1] = 0;
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
                                                            diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
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
                                                                diff_min_index = (byte)(i >> 1);
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
                                                for (int w = 0; w < bitmap_width << 1; w += 2)  // multiplied by two because each index is a 14 bytes integer
                                                {
                                                    diff_min = 500;
                                                    for (int i = 0; i < colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                    {
                                                        if (colour_palette[i] == (byte)(Colours[j] >> 8) && colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                        {
                                                            diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
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
                                                                diff_min_index = (byte)(i >> 1);
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
                                                        index[pixel << 1] = 0;
                                                        index[pixel << 1 + 1] = 0;
                                                    }
                                                }
                                                index_list.Add(index.ToArray());
                                            }
                                            break;
                                        }  // end of case 10 CI14x2
                                } // end of switch texture format
                            }  // end of if (has_palette)
                            break;
                        }  // end of case 2 palette RGB5A3
                }  // end of switch palette format
            }
            else
            {
                j = 0;
                if (fill_width && (texture_format_int32[3] == 0 || texture_format_int32[3] == 6 || texture_format_int32[3] == 0xe))  // adds a lot of checks thorough the way of creating each pixel, which is definitely slower than having an image that fullfill each block :P
                {
                    switch (texture_format_int32[3])
                    {
                        case 0: // I4  - works well
                            {
                                switch (algorithm)
                                {
                                    case 0: // cie_601
                                        {
                                            for (y = pixel_data_start_offset; y < bmp_filesize; y += 8)  // process every pixel by groups of two to fit the AAAA BBBB  profile
                                            {
                                                a = (byte)(bmp_image[y] * 0.114 + bmp_image[y + 1] * 0.587 + bmp_image[y + 2] * 0.299);  // grey colour trimmed to 4 bit
                                                if ((a & 0xf) > round4 && a < 240)
                                                {
                                                    a += 16;
                                                }
                                                if (y + 6 < bmp_filesize)
                                                {
                                                    grey = (byte)(bmp_image[y + 4] * 0.114 + bmp_image[y + 5] * 0.587 + bmp_image[y + 6] * 0.299);
                                                    if ((grey & 0xf) > round4 && grey < 240)
                                                    {
                                                        grey += 16;
                                                    }
                                                    index[j] = (byte)((a & 0xf0) + (grey >> 4));
                                                    j++;
                                                    if (j << 1 == bitmap_width)
                                                    {
                                                        j = 0;
                                                        index_list.Add(index.ToArray());
                                                    }
                                                    else if (j << 1 > bitmap_width)
                                                    {
                                                        j = 0;
                                                        index_list.Add(index.ToArray());
                                                        y -= 4;
                                                    }
                                                }
                                                else
                                                {
                                                    index[j] = a;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                    case 1: // cie_709
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 8)
                                            {
                                                a = (byte)(bmp_image[i] * 0.0721 + bmp_image[i + 1] * 0.7154 + bmp_image[i + 2] * 0.2125);
                                                if ((a & 0xf) > round4 && a < 240)
                                                {
                                                    a += 16;
                                                }

                                                if (i + 6 < bmp_filesize)
                                                {
                                                    grey = (byte)(bmp_image[i + 4] * 0.0721 + bmp_image[i + 5] * 0.7154 + bmp_image[i + 6] * 0.2125);
                                                    if ((grey & 0xf) > round4 && grey < 240)
                                                    {
                                                        grey += 16;
                                                    }
                                                    index[j] = (byte)((a & 0xf0) + (grey >> 4));
                                                    j++;
                                                    if (j == (bitmap_width >> 1))
                                                    {
                                                        j = 0;
                                                        index_list.Add(index.ToArray());
                                                    }
                                                    else if (j > (bitmap_width >> 1))
                                                    {
                                                        j = 0;
                                                        index_list.Add(index.ToArray());
                                                        y -= 4;
                                                    }
                                                }
                                                else
                                                {
                                                    index[j] = a;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                    case 2:  // custom
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 8)
                                            {
                                                a = (byte)(bmp_image[i] * custom_rgba[2] + bmp_image[i + 1] * custom_rgba[1] + bmp_image[i + 2] * custom_rgba[0]);
                                                if ((a & 0xf) > round4 && a < 240)
                                                {
                                                    a += 16;
                                                }

                                                if (i + 6 < bmp_filesize)
                                                {
                                                    grey = (byte)(bmp_image[i + 4] * custom_rgba[2] + bmp_image[i + 5] * custom_rgba[1] + bmp_image[i + 6] * custom_rgba[0]);
                                                    if ((grey & 0xf) > round4 && grey < 240)
                                                    {
                                                        grey += 16;
                                                    }
                                                    index[j] = (byte)((a & 0xf0) + (grey >> 4));
                                                    j++;
                                                    if (j == (bitmap_width >> 1))
                                                    {
                                                        j = 0;
                                                        index_list.Add(index.ToArray());
                                                    }
                                                    else if (j > (bitmap_width >> 1))
                                                    {
                                                        j = 0;
                                                        index_list.Add(index.ToArray());
                                                        y -= 4;
                                                    }
                                                }
                                                else
                                                {
                                                    index[j] = a;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        // i += 4 for all deleted ones lol
                        case 6: // RGBA32   - works well.
                            {
                                /* 4x4 pixel block
                                 * warning: THESE ARE BYTES
                                 * I4M SERIOUS ALL OTHERS ABOVE ARE BITS BUT THIS ONE IS BYTES
                                 * I'll name the first pixel 1234 and the last 5678
                                   12AR ARAR ARAR ARAR
                                   ARAR ARAR ARAR AR56
                                   34GB GBGB GBGB GBGB
                                   GBGB GBGB GBGB GB78

                                 but I'm going to encode each row in this order:
                                   ARAR ARAR GBGB GBGB
                                */
                                switch (algorithm)
                                {
                                    case 2:  // custom
                                        {
                                            for (y = pixel_data_start_offset; y < bmp_filesize; y += 16)
                                            {
                                                // alpha and red
                                                // Green and Blue
                                                index[j] = (byte)(bmp_image[y + 3] * custom_rgba[3]);       // A
                                                index[j + 1] = (byte)(bmp_image[y + 2] * custom_rgba[0]);   // R
                                                index[j + 8] = (byte)(bmp_image[y + 1] * custom_rgba[1]);   // G
                                                index[j + 9] = (byte)(bmp_image[y] * custom_rgba[2]);       // B
                                                if (y + 7 < bmp_filesize)
                                                {
                                                    index[j + 2] = (byte)(bmp_image[y + 7] * custom_rgba[3]);   // A
                                                    index[j + 3] = (byte)(bmp_image[y + 6] * custom_rgba[0]);   // R
                                                    index[j + 10] = (byte)(bmp_image[y + 5] * custom_rgba[1]);  // G
                                                    index[j + 11] = (byte)(bmp_image[y + 4] * custom_rgba[2]);  // B
                                                    if (y + 11 < bmp_filesize)
                                                    {
                                                        index[j + 4] = (byte)(bmp_image[y + 11] * custom_rgba[3]);  // A
                                                        index[j + 5] = (byte)(bmp_image[y + 10] * custom_rgba[0]);  // R
                                                        index[j + 12] = (byte)(bmp_image[y + 9] * custom_rgba[1]);  // G
                                                        index[j + 13] = (byte)(bmp_image[y + 8] * custom_rgba[2]);  // B
                                                        if (y + 15 < bmp_filesize)
                                                        {
                                                            index[j + 6] = (byte)(bmp_image[y + 15] * custom_rgba[3]);  // A
                                                            index[j + 7] = (byte)(bmp_image[y + 14] * custom_rgba[0]);  // R
                                                            index[j + 14] = (byte)(bmp_image[y + 13] * custom_rgba[1]); // G
                                                            index[j + 15] = (byte)(bmp_image[y + 12] * custom_rgba[2]); // B
                                                        }
                                                    }
                                                }
                                                j += 16;
                                                if (j == (bitmap_width << 2))
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                                else if (j > (bitmap_width << 2))
                                                {
                                                    for (; j > bitmap_width << 2; j -= 4)
                                                    {
                                                        y -= 4;
                                                    }
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            for (y = pixel_data_start_offset; y < bmp_filesize; y += 16)
                                            {
                                                // alpha and red
                                                // Green and Blue
                                                index[j] = (byte)(bmp_image[y + 3]);       // A
                                                index[j + 1] = (byte)(bmp_image[y + 2]);   // R
                                                index[j + 8] = (byte)(bmp_image[y + 1]);   // G
                                                index[j + 9] = (byte)(bmp_image[y]);       // B

                                                if (y + 7 < bmp_filesize)
                                                {
                                                    index[j + 2] = (byte)(bmp_image[y + 7]);   // A
                                                    index[j + 3] = (byte)(bmp_image[y + 6]);   // R
                                                    index[j + 10] = (byte)(bmp_image[y + 5]);  // G
                                                    index[j + 11] = (byte)(bmp_image[y + 4]);  // B

                                                    if (y + 11 < bmp_filesize)
                                                    {
                                                        index[j + 4] = (byte)(bmp_image[y + 11]);  // A
                                                        index[j + 5] = (byte)(bmp_image[y + 10]);  // R
                                                        index[j + 12] = (byte)(bmp_image[y + 9]);  // G
                                                        index[j + 13] = (byte)(bmp_image[y + 8]);  // B

                                                        if (y + 15 < bmp_filesize)
                                                        {
                                                            index[j + 6] = (byte)(bmp_image[y + 15]);  // A
                                                            index[j + 7] = (byte)(bmp_image[y + 14]);  // R
                                                            index[j + 14] = (byte)(bmp_image[y + 13]); // G
                                                            index[j + 15] = (byte)(bmp_image[y + 12]); // B

                                                        }
                                                    }
                                                }
                                                j += 16;
                                                if (j == (bitmap_width << 2))
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                                else if (j > (bitmap_width << 2))
                                                {
                                                    for (; j > bitmap_width << 2; j -= 4)
                                                    {
                                                        y -= 4;
                                                    }
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                }
                                /*
                                // 32 bits depth
                                pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                if (pixel != 0) // fills the block width data by adding zeros to the width
                                {
                                    for (; pixel < block_width; pixel++)
                                    {
                                        index[pixel << 2] = 0;
                                        index[pixel << 2 + 1] = 0;
                                        index[pixel << 2 + 2] = 0;
                                        index[pixel << 2 + 3] = 0;
                                    }
                                }
                                index_list.Add(index.ToArray()); */
                                break;
                            }
                        case 0xE: // CMPR
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
                                                if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                                {
                                                    red += 8;
                                                }
                                                if ((green & round6) == round6 && green < 252)  // 6-bit max value on a trimmed byte
                                                {
                                                    green += 4;
                                                }
                                                if ((blue & 7) > round5 && blue < 248)
                                                {
                                                    blue += 8;
                                                }
                                                pixel = (ushort)((((byte)(red) >> 3) << 11) + (((byte)(green) >> 2) << 5) + (byte)(blue) >> 3);
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
                                                if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                                {
                                                    red += 8;
                                                }
                                                if ((green & round6) == round6 && green < 252)  // 6-bit max value on a trimmed byte
                                                {
                                                    green += 4;
                                                }
                                                if ((blue & 7) > round5 && blue < 248)
                                                {
                                                    blue += 8;
                                                }
                                                pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3));
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }
                else
                {
                    switch (texture_format_int32[3])
                    {
                        case 0: // I4
                            {
                                switch (algorithm)
                                {
                                    case 0: // cie_601
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 8)  // process every pixel by groups of two to fit the AAAA BBBB  profile
                                            {
                                                a = (byte)(bmp_image[i] * 0.114 + bmp_image[i + 1] * 0.587 + bmp_image[i + 2] * 0.299);  // grey colour trimmed to 4 bit
                                                if ((a & 0xf) > round4 && a < 240)
                                                {
                                                    a += 16;
                                                }
                                                grey = (byte)(bmp_image[i + 4] * 0.114 + bmp_image[i + 5] * 0.587 + bmp_image[i + 6] * 0.299);
                                                if ((grey & 0xf) > round4 && grey < 240)
                                                {
                                                    grey += 16;
                                                }
                                                index[j] = (byte)((a & 0xf0) + (grey >> 4));
                                                j++;
                                                if (j == index.Length)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                    case 1: // cie_709
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 8)
                                            {
                                                a = (byte)(bmp_image[i] * 0.0721 + bmp_image[i + 1] * 0.7154 + bmp_image[i + 2] * 0.2125);
                                                if ((a & 0xf) > round4 && a < 240)
                                                {
                                                    a += 16;
                                                }
                                                grey = (byte)(bmp_image[i + 4] * 0.0721 + bmp_image[i + 5] * 0.7154 + bmp_image[i + 6] * 0.2125);
                                                if ((grey & 0xf) > round4 && grey < 240)
                                                {
                                                    grey += 16;
                                                }
                                                index[j] = (byte)((a & 0xf0) + (grey >> 4));
                                                j++;
                                                if (j == index.Length)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                    case 2:  // custom
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 8)
                                            {
                                                a = (byte)(bmp_image[i] * custom_rgba[2] + bmp_image[i + 1] * custom_rgba[1] + bmp_image[i + 2] * custom_rgba[0]);
                                                if ((a & 0xf) > round4 && a < 240)
                                                {
                                                    a += 16;
                                                }
                                                grey = (byte)(bmp_image[i + 4] * custom_rgba[2] + bmp_image[i + 5] * custom_rgba[1] + bmp_image[i + 6] * custom_rgba[0]);
                                                if ((grey & 0xf) > round4 && grey < 240)
                                                {
                                                    grey += 16;
                                                }
                                                index[j] = (byte)((a & 0xf0) + (grey >> 4));
                                                j++;
                                                if (j == index.Length)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case 1: // I8
                            {
                                switch (algorithm)
                                {
                                    case 0: // cie_601
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)  // process every pixel to fit the CCCC CCCC profile
                                            {
                                                index[j] = (byte)(bmp_image[i] * 0.114 + bmp_image[i + 1] * 0.587 + bmp_image[i + 2] * 0.299);
                                                j++;
                                                if (j == bitmap_width)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                    case 1: // cie_709
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                            {
                                                index[j] = (byte)(bmp_image[i] * 0.0721 + bmp_image[i + 1] * 0.7154 + bmp_image[i + 2] * 0.2125);
                                                j++;
                                                if (j == bitmap_width)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                    case 2:  // custom
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                            {
                                                index[j] = (byte)(bmp_image[i] * custom_rgba[2] + bmp_image[i + 1] * custom_rgba[1] + bmp_image[i + 2] * custom_rgba[0]);
                                                j++;
                                                if (j == bitmap_width)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case 2: // IA4
                            {
                                switch (algorithm)
                                {
                                    case 0: // cie_601
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)  // process every pixel to fit the AAAA CCCC profile
                                            {
                                                a = (bmp_image[i + 3]);  // alpha value
                                                if ((a & 0xf) > round4 && a < 240)
                                                {
                                                    a += 16;
                                                }
                                                grey = (byte)(bmp_image[i] * 0.114 + bmp_image[i + 1] * 0.587 + bmp_image[i + 2] * 0.299);
                                                if ((grey & 0xf) > round4 && grey < 240)
                                                {
                                                    grey += 16;
                                                }
                                                index[j] = (byte)((a & 0xf0) + (grey >> 4));
                                                j++;
                                                if (j == bitmap_width)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                    case 1: // cie_709
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                            {
                                                a = (bmp_image[i + 3]);  // alpha value
                                                if ((a & 0xf) > round4 && a < 240)
                                                {
                                                    a += 16;
                                                }
                                                grey = (byte)(bmp_image[i] * 0.0721 + bmp_image[i + 1] * 0.7154 + bmp_image[i + 2] * 0.2125);
                                                if ((grey & 0xf) > round4 && grey < 240)
                                                {
                                                    grey += 16;
                                                }
                                                index[j] = (byte)((a & 0xf0) + (grey >> 4));
                                                j++;
                                                if (j == bitmap_width)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                    case 2:  // custom
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                            {
                                                a = (byte)(bmp_image[i + 3] * custom_rgba[3]);  // alpha value
                                                if ((a & 0xf) > round4 && a < 240)
                                                {
                                                    a += 16;
                                                }
                                                grey = (byte)(bmp_image[i] * custom_rgba[2] + bmp_image[i + 1] * custom_rgba[1] + bmp_image[i + 2] * custom_rgba[0]);
                                                if ((grey & 0xf) > round4 && grey < 240)
                                                {
                                                    grey += 16;
                                                }
                                                index[j] = (byte)((a & 0xf0) + (grey >> 4));
                                                j++;
                                                if (j == bitmap_width)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case 3:  // AI8
                            {

                                switch (algorithm)
                                {
                                    case 0: // cie_601
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
                                            {
                                                index[j] = bmp_image[i + 3];  // alpha value
                                                index[j + 1] = (byte)(bmp_image[i] * 0.114 + bmp_image[i + 1] * 0.587 + bmp_image[i + 2] * 0.299);  // Grey Value
                                                j += 2;
                                                if (j == bitmap_width << 1)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                    case 1: // cie_709
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                            {
                                                index[j] = bmp_image[i + 3];  // alpha value
                                                index[j + 1] = (byte)(bmp_image[i] * 0.0721 + bmp_image[i + 1] * 0.7154 + bmp_image[i + 2] * 0.2125);  // Grey Value
                                                j += 2;
                                                if (j == bitmap_width << 1)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                    case 2:  // custom
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                            {
                                                index[j] = (byte)(bmp_image[i + 3] * custom_rgba[3]);  // alpha value
                                                index[j + 1] = (byte)(bmp_image[i] * custom_rgba[2] + bmp_image[i + 1] * custom_rgba[1] + bmp_image[i + 2] * custom_rgba[0]);  // Grey Value
                                                j += 2;
                                                if (j == bitmap_width << 1)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case 4:  // RGB565
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
                                                if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                                {
                                                    red += 8;
                                                }
                                                if ((green & round6) == round6 && green < 252)  // 6-bit max value on a trimmed byte
                                                {
                                                    green += 4;
                                                }
                                                if ((blue & 7) > round5 && blue < 248)
                                                {
                                                    blue += 8;
                                                }
                                                index[j] = (byte)((red & 0xf8) + (green >> 5));
                                                index[j + 1] = (byte)(((green << 3) & 224) + (blue >> 3));
                                                j += 2;
                                                if (j == bitmap_width << 1)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }

                                            break;
                                        }
                                    default: // RRRR RGGG GGGB BBBB
                                        {
                                            for (y = pixel_data_start_offset; y < bmp_filesize; y += 4)
                                            {
                                                red = bmp_image[y + 2];
                                                green = bmp_image[y + 1];
                                                blue = bmp_image[y];
                                                if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                                {
                                                    red += 8;
                                                }
                                                if ((green & round6) == round6 && green < 252)  // 6-bit max value on a trimmed byte
                                                {
                                                    green += 4;
                                                }
                                                if ((blue & 7) > round5 && blue < 248)
                                                {
                                                    blue += 8;
                                                }
                                                index[j] = (byte)((red & 0xf8) + (green >> 5));
                                                index[j + 1] = (byte)(((green << 3) & 224) + (blue >> 3));
                                                j += 2;
                                                if (j == bitmap_width << 1)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case 5:  // RGB5A3
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
                                                    if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                                    {
                                                        a += 32;
                                                    }
                                                    if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                                    {
                                                        red += 16;
                                                    }
                                                    if ((green & 15) > round4 && green < 240)
                                                    {
                                                        green += 16;
                                                    }
                                                    if ((blue & 15) > round4 && blue < 240)
                                                    {
                                                        blue += 16;
                                                    }
                                                    index[j] = (byte)(((a >> 1) & 0x70) + (red >> 4));
                                                    index[j + 1] = (byte)((green & 0xf0) + (blue >> 4));
                                                    j += 2;
                                                    if (j == bitmap_width << 1)
                                                    {
                                                        j = 0;
                                                        index_list.Add(index.ToArray());
                                                    }
                                                }
                                            }
                                            else if (alpha == 0)  // 1RRR RRGG GGGB BBBB
                                            {
                                                for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                {
                                                    red = (byte)(bmp_image[i + 2] * custom_rgba[0]);
                                                    green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                                    blue = (byte)(bmp_image[i] * custom_rgba[2]);
                                                    if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                                    {
                                                        red += 8;
                                                    }
                                                    if ((green & 7) > round5 && green < 248)
                                                    {
                                                        green += 8;
                                                    }
                                                    if ((blue & 7) > round5 && blue < 248)
                                                    {
                                                        blue += 8;
                                                    }
                                                    index[j] = (byte)(0x80 + ((red >> 1) & 0x7c) + (green >> 6));
                                                    index[j + 1] = (byte)(((green << 2) & 0xe0) + (blue >> 3));
                                                    j += 2;
                                                    if (j == bitmap_width << 1)
                                                    {
                                                        j = 0;
                                                        index_list.Add(index.ToArray());
                                                    }
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
                                                    if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                                    {
                                                        a += 32;
                                                    }
                                                    if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                                    {
                                                        red += 16;
                                                    }
                                                    if ((green & 15) > round4 && green < 240)
                                                    {
                                                        green += 16;
                                                    }
                                                    if ((blue & 15) > round4 && blue < 240)
                                                    {
                                                        blue += 16;
                                                    }
                                                    if (a > 223)  // 0AAA RRRR GGGG BBBB
                                                    {

                                                        index[j] = (byte)(0x80 + ((red >> 1) & 0x7c) + (green >> 6));
                                                        index[j + 1] = (byte)(((green << 2) & 0xe0) + (blue >> 3));
                                                    }
                                                    else  // 1RRR RRGG GGGB BBBB
                                                    {

                                                        index[j] = (byte)(((a >> 1) & 0x70) + (red >> 4));
                                                        index[j + 1] = (byte)((green & 0xf0) + (blue >> 4));

                                                    }
                                                    j += 2;
                                                    if (j == bitmap_width << 1)
                                                    {
                                                        j = 0;
                                                        index_list.Add(index.ToArray());
                                                    }
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
                                                    if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                                    {
                                                        a += 32;
                                                    }
                                                    if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                                    {
                                                        red += 16;
                                                    }
                                                    if ((green & 15) > round4 && green < 240)
                                                    {
                                                        green += 16;
                                                    }
                                                    if ((blue & 15) > round4 && blue < 240)
                                                    {
                                                        blue += 16;
                                                    }
                                                    j += 2;
                                                    if (j == bitmap_width << 1)
                                                    {
                                                        j = 0;
                                                        index_list.Add(index.ToArray());
                                                    }
                                                }
                                            }
                                            else if (alpha == 0)  // 1RRR RRGG GGGB BBBB
                                            {
                                                for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                                {
                                                    red = bmp_image[i + 2];
                                                    green = bmp_image[i + 1];
                                                    blue = bmp_image[i];
                                                    if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                                    {
                                                        red += 8;
                                                    }
                                                    if ((green & 7) > round5 && green < 248)
                                                    {
                                                        green += 8;
                                                    }
                                                    if ((blue & 7) > round5 && blue < 248)
                                                    {
                                                        blue += 8;
                                                    }

                                                    index[j] = (byte)(0x80 + ((red >> 1) & 0x7c) + (green >> 6));
                                                    index[j + 1] = (byte)(((green << 2) & 0xe0) + (blue >> 3));
                                                    j += 2;
                                                    if (j == bitmap_width << 1)
                                                    {
                                                        j = 0;
                                                        index_list.Add(index.ToArray());
                                                    }
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
                                                    if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                                    {
                                                        a += 32;
                                                    }
                                                    if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                                    {
                                                        red += 16;
                                                    }
                                                    if ((green & 15) > round4 && green < 240)
                                                    {
                                                        green += 16;
                                                    }
                                                    if ((blue & 15) > round4 && blue < 240)
                                                    {
                                                        blue += 16;
                                                    }
                                                    if (a > 223)  // 1RRR RRGG GGGB BBBB
                                                    {
                                                        index[j] = (byte)(0x80 + ((red >> 1) & 0x7c) + (green >> 6));
                                                        index[j + 1] = (byte)(((green << 2) & 0xe0) + (blue >> 3));
                                                    }
                                                    else  // 0AAA RRRR GGGG BBBB
                                                    {
                                                        index[j] = (byte)(((a >> 1) & 0x70) + (red >> 4));
                                                        index[j + 1] = (byte)((green & 0xf0) + (blue >> 4));
                                                    }
                                                    j += 2;
                                                    if (j == bitmap_width << 1)
                                                    {
                                                        j = 0;
                                                        index_list.Add(index.ToArray());
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case 6: // RGBA32
                            {
                                /* 4x4 pixel block
                                 * warning: THESE ARE BYTES
                                 * I4M SERIOUS ALL OTHERS ABOVE ARE BITS BUT THIS ONE IS BYTES
                                 * I'll name the first pixel 1234 and the last 5678
                                   12AR ARAR ARAR ARAR
                                   ARAR ARAR ARAR AR56
                                   34GB GBGB GBGB GBGB
                                   GBGB GBGB GBGB GB78

                                 but I'm going to encode each row in this order:
                                   ARAR ARAR GBGB GBGB
                                */
                                switch (algorithm)
                                {
                                    case 2:  // custom
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 16)
                                            {
                                                // alpha and red
                                                index[j] = (byte)(bmp_image[i + 3] * custom_rgba[3]);       // A
                                                index[j + 1] = (byte)(bmp_image[i + 2] * custom_rgba[0]);   // R
                                                index[j + 2] = (byte)(bmp_image[i + 7] * custom_rgba[3]);   // A
                                                index[j + 3] = (byte)(bmp_image[i + 6] * custom_rgba[0]);   // R
                                                index[j + 4] = (byte)(bmp_image[i + 11] * custom_rgba[3]);  // A
                                                index[j + 5] = (byte)(bmp_image[i + 10] * custom_rgba[0]);  // R
                                                index[j + 6] = (byte)(bmp_image[i + 15] * custom_rgba[3]);  // A
                                                index[j + 7] = (byte)(bmp_image[i + 14] * custom_rgba[0]);  // R
                                                                                                            // Green and Blue
                                                index[j + 8] = (byte)(bmp_image[i + 1] * custom_rgba[1]);   // G
                                                index[j + 9] = (byte)(bmp_image[i] * custom_rgba[2]);       // B
                                                index[j + 10] = (byte)(bmp_image[i + 5] * custom_rgba[1]);  // G
                                                index[j + 11] = (byte)(bmp_image[i + 4] * custom_rgba[2]);  // B
                                                index[j + 12] = (byte)(bmp_image[i + 9] * custom_rgba[1]);  // G
                                                index[j + 13] = (byte)(bmp_image[i + 8] * custom_rgba[2]);  // B
                                                index[j + 14] = (byte)(bmp_image[i + 13] * custom_rgba[1]); // G
                                                index[j + 15] = (byte)(bmp_image[i + 12] * custom_rgba[2]); // B
                                                j += 16;
                                                if (j == index.Length)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }

                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 16)
                                            {
                                                // alpha and red
                                                index[j] = (byte)(bmp_image[i + 3]);       // A
                                                index[j + 1] = (byte)(bmp_image[i + 2]);   // R
                                                index[j + 2] = (byte)(bmp_image[i + 7]);   // A
                                                index[j + 3] = (byte)(bmp_image[i + 6]);   // R
                                                index[j + 4] = (byte)(bmp_image[i + 11]);  // A
                                                index[j + 5] = (byte)(bmp_image[i + 10]);  // R
                                                index[j + 6] = (byte)(bmp_image[i + 15]);  // A
                                                index[j + 7] = (byte)(bmp_image[i + 14]);  // R
                                                                                           // Green and Blue
                                                index[j + 8] = (byte)(bmp_image[i + 1]);   // G
                                                index[j + 9] = (byte)(bmp_image[i]);       // B
                                                index[j + 10] = (byte)(bmp_image[i + 5]);  // G
                                                index[j + 11] = (byte)(bmp_image[i + 4]);  // B
                                                index[j + 12] = (byte)(bmp_image[i + 9]);  // G
                                                index[j + 13] = (byte)(bmp_image[i + 8]);  // B
                                                index[j + 14] = (byte)(bmp_image[i + 13]); // G
                                                index[j + 15] = (byte)(bmp_image[i + 12]); // B
                                                j += 16;
                                                if (j == index.Length)
                                                {
                                                    j = 0;
                                                    index_list.Add(index.ToArray());
                                                }
                                            }
                                            break;
                                        }
                                }
                                /*
                                // 32 bits depth
                                pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                if (pixel != 0) // fills the block width data by adding zeros to the width
                                {
                                    for (; pixel < block_width; pixel++)
                                    {
                                        index[pixel << 2] = 0;
                                        index[pixel << 2 + 1] = 0;
                                        index[pixel << 2 + 2] = 0;
                                        index[pixel << 2 + 3] = 0;
                                    }
                                }
                                index_list.Add(index.ToArray()); */
                                break;
                            }
                        case 0xE: // CMPR
                            {

                                /* hmm, well. let's be honest. this is the harderest encoding to write, and the most efficient one
                                 * I'll be directly storing sub-blocks here because the rgb565 values can't be added like that lol 
                                 
                                 each block is 4 sub blocks
                                this is a sub-block structure. with 4x4 pixel and 2 rgb565 colours
                                RRRR  RGGG    GGGB  BBBB
                                RRRR  RGGG    GGGB  BBBB
                                II II II II   II II II II  - 2 bit index per pixel
                                II II II II   II II II II
                                II II II II   II II II II
                                II II II II   II II II II

                                 */
                                List<ushort> Colour_rgb565 = new List<ushort>();  // won't be sorted
                                List<ushort[]> Colour_list = new List<ushort[]>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
                                // byte[] Colour_count = {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };  // 16 pixels, because a block is 4x4
                                j = 0;
                                z = 0;
                                int x = 0;
                                int c;
                                ushort alpha_bitfield = 0;
                                ushort[] Colour_pixel = { 1, 0 };
                                List<ushort> Colour_palette = new List<ushort>();
                                bool use_alpha = false;
                                Array.Resize(ref index, 8);  // sub block length
                                switch (algorithm)
                                {
                                    case 2:  // custom  RRRR RGGG GGGB BBBB
                                        {
                                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                            {
                                                red = (byte)(bmp_image[i + 2] * custom_rgba[0]);
                                                green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                                blue = (byte)(bmp_image[i] * custom_rgba[2]);
                                                if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                                {
                                                    red += 8;
                                                }
                                                if ((green & round6) == round6 && green < 252)  // 6-bit max value on a trimmed byte
                                                {
                                                    green += 4;
                                                }
                                                if ((blue & 7) > round5 && blue < 248)
                                                {
                                                    blue += 8;
                                                }
                                                pixel = (ushort)((((byte)(red) >> 3) << 11) + (((byte)(green) >> 2) << 5) + (byte)(blue) >> 3);
                                            }

                                            break;
                                        }
                                    default: // RRRR RGGG GGGB BBBB
                                        {
                                            for (y = pixel_data_start_offset; y < bmp_filesize; y += 4)
                                            {
                                                red = bmp_image[y + 2];
                                                green = bmp_image[y + 1];
                                                blue = bmp_image[y];
                                                if (alpha > 0 && bmp_image[y + 3] < cmpr_alpha_threshold)
                                                {
                                                    use_alpha = true;
                                                    alpha_bitfield += (ushort)(1 << (j + (z * 4)));
                                                }
                                                if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                                {
                                                    red += 8;
                                                }
                                                if ((green & round6) == round6 && green < 252)  // 6-bit max value on a trimmed byte
                                                {
                                                    green += 4;
                                                }
                                                if ((blue & 7) > round5 && blue < 248)
                                                {
                                                    blue += 8;
                                                }
                                                // Colour_pixel[0] = // the number of occurences, though it stays to 1 so that's not really a problem lol
                                                pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                                                Colour_pixel[1] = pixel;
                                                Colour_list.Add(Colour_pixel);
                                                Colour_rgb565.Add(pixel);
                                                j++;
                                                if (j == 4)
                                                {
                                                    j = 0;
                                                    z++;
                                                    x++;
                                                    y += (bitmap_width << 2) - 16; // returns to the start of the next line (I hope)  - bitmap width << 2 because it's a 32-bit BGRA bmp file
                                                    if (z == 4)
                                                    {
                                                        alpha_bitfield = 0;
                                                        use_alpha = false;
                                                        z = 0;
                                                        if (x == bitmap_width)
                                                        {
                                                            // y += (bitmap_width << 4) - 4; // adds 4 lines and put the cursor back to the first block in width (I hope)
                                                            // y += 16; // hmm, it looks like the cursor warped horizontally to the first block in width 4 lines above
                                                            // EDIT: YA DEFINITELY NEED TO CANCEL THE Y OPERATION ABOVE, IT WARPS NORMALLY LIKE IT4S THE PIXEL AFTER
                                                            y -= (bitmap_width << 2) - 16;  // this has been driving me nuts
                                                            x = 0;
                                                        }
                                                        else if (x > bitmap_width)
                                                        {
                                                            Console.WriteLine("HOLY SH*T");
                                                        }
                                                        else
                                                        {
                                                            y -= (bitmap_width << 4) - 16; // on retire 4 lignes et on passe le 1er block héhé
                                                                                           // substract 4 lines and jumps over the first block
                                                        }
                                                        // now let's just try to take the most two used colours and use diversity I guess
                                                        // implementing my own way to find most used colours:
                                                        // let's count the number of exact same colours in Colour_list
                                                        for (int i = 0; i < 15; i++)  // useless to set it to 16 because of the condition k > i.
                                                        {
                                                            for (int k = 0; k < 16; k++)
                                                            {
                                                                if (k == i)
                                                                {
                                                                    continue;
                                                                }
                                                                if (Colour_list[k] == Colour_list[i] && k > i && ((alpha_bitfield >> k) & 1) == 0 && ((alpha_bitfield >> i) & 1) == 0)  // k > i prevents colours occurences from being added twice.
                                                                {
                                                                    Colour_list[k][0]++;
                                                                    Colour_list[i][0]--; // should set it to zero.
                                                                }
                                                            }
                                                        }
                                                        Colour_list.Sort(new UshortArrayComparer());  // sorts the table by the most used colour first
                                                        c = 0;
                                                        for (int i = 0; i < 16 && c < 4; i++)  // build the colour table with the two most used colours and diversityfffffffffffff
                                                        {
                                                            not_similar = true;
                                                            if (Colour_list[i][0] / 16 < percentage / 100)
                                                            {
                                                                break;
                                                            }
                                                            if (c == 2)  // checks for diversity before adding the second colour ^^
                                                            {
                                                                if (Math.Abs((index[0] & 248) - ((Colour_list[i][1] >> 8) & 248)) < diversity && Math.Abs(((index[0] & 7) << 5) + ((index[1] >> 3) & 28) - ((Colour_list[i][1] >> 3) & 252)) < diversity && Math.Abs(((index[1] << 3) & 248) - (Colour_list[i][1] << 3) & 248) < diversity)
                                                                {
                                                                    not_similar = false;
                                                                    break;
                                                                }
                                                            }
                                                            if (not_similar)
                                                            {
                                                                index[c] = (byte)(Colour_list[i][1] >> 8);  // adds the RRRR RGGG value
                                                                index[c + 1] = (byte)(Colour_list[i][1]);  // adds the GGGB BBBB value
                                                                c += 2;
                                                            }
                                                        }
                                                        if (c < 4) // if the colour palette is not full
                                                        {
                                                            Console.WriteLine("The colour palette was not full, starting second loop...\n");

                                                            for (int i = 0; i < 16 && c < 4; i++)
                                                            {
                                                                not_similar = true;
                                                                if (Colour_list[i][0] / 16 < percentage2 / 100)
                                                                {
                                                                    break;
                                                                }
                                                                if (c == 2)  // checks for diversity before adding the second colour ^^
                                                                {
                                                                    if (Math.Abs((index[0] & 248) - ((Colour_list[i][1] >> 8) & 248)) < diversity2 && Math.Abs(((index[0] & 7) << 5) + ((index[1] >> 3) & 28) - ((Colour_list[i][1] >> 3) & 252)) < diversity2 && Math.Abs(((index[1] << 3) & 248) - (Colour_list[i][1] << 3) & 248) < diversity2)
                                                                    {
                                                                        not_similar = false;
                                                                        break;
                                                                    }
                                                                }
                                                                if (not_similar)
                                                                {
                                                                    index[c] = (byte)(Colour_list[i][1] >> 8);  // adds the RRRR RGGG value
                                                                    index[c + 1] = (byte)(Colour_list[i][1]);  // adds the GGGB BBBB value
                                                                    c += 2;
                                                                }
                                                            }
                                                            if (c < 4) // if the colour palette is still not full
                                                            {
                                                                Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                                                for (int i = 0; i < 16 && c < 4; i++)
                                                                {
                                                                    not_similar = true;
                                                                    if (c == 2)
                                                                    {
                                                                        if ((index[0] != (byte)(Colour_list[i][1] >> 8)) || index[1] != (byte)(Colour_list[i][1]))
                                                                        {
                                                                            not_similar = false;
                                                                            break;
                                                                        }
                                                                    }
                                                                    if (not_similar)
                                                                    {
                                                                        index[c] = (byte)(Colour_list[i][1] >> 8);  // adds the RRRR RGGG value
                                                                        index[c + 1] = (byte)(Colour_list[i][1]);  // adds the GGGB BBBB value
                                                                        c += 2;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        if (use_alpha)  // put the biggest ushort in second place
                                                        {
                                                            if (index[0] > index[2] || (index[0] == index[2] && index[1] > index[3]))  // swap
                                                            {
                                                                index[4] = index[0];
                                                                index[5] = index[1];
                                                                index[0] = index[2];
                                                                index[1] = index[3];
                                                                index[0] = index[4];
                                                                index[1] = index[5];
                                                            }
                                                            Colour_palette.Add((ushort)((index[0] << 8) + index[1]));
                                                            Colour_palette.Add((ushort)((index[2] << 8) + index[3]));
                                                            red = (byte)(((index[0] & 248) + (index[2] & 248)) / 2);
                                                            green = (byte)(((((index[0] & 7) << 5) + ((index[1] >> 3) & 28)) + (((index[2] & 7) << 5) + ((index[3] >> 3) & 28))) / 2);
                                                            blue = (byte)((((index[1] << 3) & 248) + ((index[3] << 3) & 248)) / 2);
                                                            Colour_palette.Add((ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)));  // the RGB565 third colour
                                                                                                                                                   // last colour isn't in the palette, it's in alpha_bitfield
                                                        }
                                                        else
                                                        {
                                                            // of course, that's the exact opposite!
                                                            if (index[0] < index[2] || (index[0] == index[2] && index[1] < index[3]))  // swap
                                                            {
                                                                index[4] = index[0];
                                                                index[5] = index[1];
                                                                index[0] = index[2];
                                                                index[1] = index[3];
                                                                index[0] = index[4];
                                                                index[1] = index[5];
                                                            }
                                                            Colour_palette.Add((ushort)((index[0] << 8) + index[1]));
                                                            Colour_palette.Add((ushort)((index[2] << 8) + index[3]));

                                                            red = (byte)(((index[0] & 248) + (index[2] & 248)) * (1 / 3));
                                                            green = (byte)(((((index[0] & 7) << 5) + ((index[1] >> 3) & 28)) + (((index[2] & 7) << 5) + ((index[3] >> 3) & 28))) * (1 / 3));
                                                            blue = (byte)((((index[1] << 3) & 248) + ((index[3] << 3) & 248)) * (1 / 3));
                                                            Colour_palette.Add((ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)));  // the RGB565 third colour

                                                            red = (byte)(((index[0] & 248) + (index[2] & 248)) * (2 / 3));
                                                            green = (byte)(((((index[0] & 7) << 5) + ((index[1] >> 3) & 28)) + (((index[2] & 7) << 5) + ((index[3] >> 3) & 28))) * (2 / 3));
                                                            blue = (byte)((((index[1] << 3) & 248) + ((index[3] << 3) & 248)) * (2 / 3));
                                                            Colour_palette.Add((ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)));  // the RGB565 fourth colour
                                                                                                                                                   // last colour isn't in the palette, it's in alpha_bitfield
                                                        }
                                                        // time to get the "linear interpolation to add third and fourth colour
                                                        Console.WriteLine("creating indexes");
                                                        // CI2 if that's a name lol
                                                        for (int h = 0; h < 4; h++)
                                                        {
                                                            for (int w = 0; w < 4; w++)  // index_size = number of pixels
                                                            {
                                                                Console.WriteLine(Colour_palette.Count + " " + ((h * 4) + w));
                                                                if (((alpha_bitfield >> (h * 4) + w) & 1) == 1)
                                                                {
                                                                    index[2 + h] += (byte)(3 << (6 - (w << 1)));
                                                                    continue;
                                                                }
                                                                diff_min = 500;
                                                                for (byte i = 0; i < Colour_palette.Count; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                                {
                                                                    if (Colour_palette[i] == Colour_rgb565[(h * 4) + w])  // if it's the exact same colour
                                                                    {
                                                                        diff_min_index = i;  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                                        break;
                                                                    }
                                                                    else  // calculate difference between each separate colour channel and store the sum
                                                                    {
                                                                        diff = (short)(Math.Abs(((Colour_palette[i] >> 8) & 248) - ((Colour_rgb565[(h * 4) + w] >> 8) & 248)) + Math.Abs(((Colour_palette[i] >> 3) & 252) - ((Colour_rgb565[(h * 4) + w] >> 3) & 252)) + Math.Abs(((Colour_palette[i] << 3) & 248) - ((Colour_rgb565[(h * 4) + w] << 3) & 248)));
                                                                        if (diff < diff_min)
                                                                        {
                                                                            diff_min = diff;
                                                                            diff_min_index = (byte)(i >> 1);
                                                                        }
                                                                    }
                                                                }
                                                                index[2 + h] += (byte)(diff_min_index << (6 - (w << 1)));
                                                            }
                                                            /*
                                                            pixel = (ushort)(Math.Abs(block_width - bitmap_width) % block_width);
                                                            if (pixel != 0) // fills the block width data by adding zeros to the width
                                                            {
                                                                if (pixel % 2 != 0)
                                                                {
                                                                    pixel++;
                                                                }
                                                                for (; pixel < block_width; pixel++)
                                                                {
                                                                    index[pixel >> 1] = 0;
                                                                }
                                                            }*/
                                                        }
                                                        index_list.Add(index.ToArray());
                                                        // index is overwritten each time
                                                        // the lists need to be cleaned
                                                        Colour_list.Clear();
                                                        Colour_palette.Clear();
                                                        Colour_rgb565.Clear();
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                }
                                break;
                            }
                    }
                }
            }

            return index_list;
        }
        public byte[] Convert_to_bmp(System.Drawing.Bitmap imageIn)
        {
            Console.WriteLine(imageIn.PixelFormat.ToString());
            if (!FORCE_ALPHA)
            {
                switch (imageIn.PixelFormat.ToString())
                {
                    case "Format32bppRgb":
                    case "Format24bppRgb":
                    case "Format1bppIndexed":
                    case "Format4bppIndexed":
                    case "Format8bppIndexed":
                        {
                            alpha = 0; // I don't care if the user set alpha to 1 or 2. the input image has no alpha. I won't let him trick my tool unless he uses the parameter "FORCE ALPHA"
                            break;
                        }
                        // case "Format32bppArgb"
                }
            }
            var bmp = new Bitmap(imageIn.Width, imageIn.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);  // makes it 32 bit in depth
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(imageIn, new Rectangle(0, 0, imageIn.Width, imageIn.Height));
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Convert an image to specified format.
        /// </summary>
        /// <param name="imageIn">The image to convert.</param>
        /// <param name="current_mipmap">The actual numnber of mipmap (used to add .mm1 at the end).</param>
        /// <returns>The converted image written in a free-to-edit file.</returns>
        public bool ConvertAndSave(System.Drawing.Bitmap imageIn, int current_mipmap)
        {
            string end = ".bmp";
            using (var ms = new MemoryStream())
            {
                if (png)
                {
                    imageIn.Save(ms, ImageFormat.Png);
                    end = ".png";
                }
                else if (tif)
                {
                    imageIn.Save(ms, ImageFormat.Tiff);
                    end = ".tif";
                }
                else if (tiff)
                {
                    imageIn.Save(ms, ImageFormat.Tiff);
                    end = ".tiff";
                }
                else if (ico)
                {
                    imageIn.Save(ms, ImageFormat.Icon);
                    end = ".ico";
                }
                else if (jpg)
                {
                    imageIn.Save(ms, ImageFormat.Jpeg);
                    end = ".jpg";
                }
                else if (jpeg)
                {
                    imageIn.Save(ms, ImageFormat.Jpeg);
                    end = ".jpeg";
                }
                else if (gif)
                {
                    imageIn.Save(ms, ImageFormat.Gif);
                    end = ".gif";
                }
                if (current_mipmap != 0)
                {
                    end = ".mm" + current_mipmap + end;
                }
                FileMode mode = System.IO.FileMode.CreateNew;
                if (System.IO.File.Exists(output_file + end))
                {
                    mode = System.IO.FileMode.Truncate;
                    if (warn)
                    {
                        Console.WriteLine("Press enter to overwrite " + output_file + end);
                        Console.ReadLine();
                    }
                }
                using (System.IO.FileStream file = System.IO.File.Open(output_file + end, mode, System.IO.FileAccess.Write))
                {
                    file.Write(ms.ToArray(), 0, (int)ms.Length);
                    file.Close();
                    Console.WriteLine(output_file + end);
                }
                return true;
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
