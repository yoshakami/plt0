using System;
/*
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices; */
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
0x00    2    Height  // height is indeed written before width
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
0x1A    2    LODBias (apparently it's the distance from which the game should switch to mipmaps)  <------------------- I can't know how that works so I've made it to be always zero in the write_BTI function
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
            Parse_args_class main_class = new Parse_args_class();
            main_class.Parse_args();
            main_class.Check_exit();
        }
    }
}
