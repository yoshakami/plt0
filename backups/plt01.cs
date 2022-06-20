using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Windows.Input;
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

for the palette RGB5A3, it's AAAA RRRR   GGGG BBBB  (colour values can only be multiple of 16)
A: 15 R:255 G:3 B:227 is     0000 1111   0000 1101
A: 0  R:255 G:0 B:221 is what Brawlcrate encodes (such a bad encoding to be honest 15 is nearest from 16 than from 0)

for the palette IA8, it's  AAAA AAAA   CCCC CCCC (Alpha and Colour can be any value between 0 and 255)
A: 15 R:127 G:127 B:127 is 0000 0111   0111 1111

for the palette RGB565, it's RRRR RGGG   GGGB BBBB (alpha always 255, and yes there are 6 G lol)
R: 200 G:60 B:69 would be    1100 0001   1110 1000
R: 197 G:61 B:66 is displayed by BrawlCrate and R:192 G:60 B:64 is what's really encoded (by Brawlcrate). it looks like Brawlcrate is faulty lol
```*/
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
    class plt0_class
    {
        delegate object palette_function(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset);
        delegate object index_function(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset);
        byte[] texture_format_int32 = {0, 0, 0, 8};
        byte[] palette_format_int32 = { 0, 0, 0, 0 };
        byte[] colour_palette;
        byte mipmaps_number = 0;
        sbyte block_width = -1;
        sbyte block_height = -1;
        int index_size;
        short bitmap_width;
        short bitmap_height;
        short colour_number = -1;
        short colour_number_x2;
        short max_colours = -1;
        string input_file = "";
        string output_file = "";
        byte pass = 0;
        float[] custom_rgba = { 1, 1, 1, 1 };
        byte algorithm = 0;  // 0 = CIE 601    1 = CIE 709     2 = custom RGBA
        public void parse_args()
        {
            string[] args = Environment.GetCommandLineArgs();
            byte[] bmp_image = Convert_to_bmp((Bitmap)Bitmap.FromFile(args[1]));
            palette_function selected_palette = null;
            index_function selected_index = null;

            for (int i = 1; i < args.Length; i++)
            {
                if (pass > 0)
                {
                    pass--;
                    continue;
                }
                if (selected_palette == null || block_height == -1)
                {
                    switch (args[i].ToUpper())
                    {
                        case "IA8":
                        case "AI8":
                            {
                                selected_palette = create_PLT0_IA8;
                                selected_index = build_mipmaps_IA8;
                                break;
                            }
                        case "RGB565":
                        case "RGB":
                            {
                                selected_palette = create_PLT0_RGB565;
                                selected_index = build_mipmaps_RGB565;
                                palette_format_int32[3] = 1;
                                break;
                            }
                        case "RGB5A3":
                        case "RGBA":
                            {
                                selected_palette = create_PLT0_RGB5A3;
                                selected_index = build_mipmaps_RGB5A3;
                                palette_format_int32[3] = 2;
                                break;
                            }
                        case "C4":
                        case "CI4":
                            {
                                max_colours = 16;
                                block_width = 8;
                                block_height = 8;
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
                                max_colours = 16385;
                                block_width = 4;
                                block_height = 4;
                                texture_format_int32[3] = 10;
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
                                float.TryParse(args[i+1], out custom_rgba[0]);
                                float.TryParse(args[i+2], out custom_rgba[1]);
                                float.TryParse(args[i+3], out custom_rgba[2]);
                                float.TryParse(args[i+4], out custom_rgba[3]);
                                pass = 4;
                                break;
                            }
                        case "-C":
                        case "C":
                            {
                                short.TryParse(args[i], out colour_number); //colour_number is now a number 
                                colour_number_x2 = (short)(colour_number * 2);
                                break;
                            }
                        case "M":
                        case "--N-MIPMAPS":
                        case "--N-MM":
                        case "-M":
                            {
                                byte.TryParse(args[i], out mipmaps_number); //mipmaps_number is now a number 
                                break;
                            }
                        default:
                            {
                                bool result = short.TryParse(args[i], out colour_number); //colour_number is now a number 
                                if (System.IO.File.Exists(args[i]) && input_file == "")
                                {
                                    input_file = args[i];
                                }
                                else if (!result)
                                {
                                    output_file = args[i].Split('.')[0];
                                }
                                break;
                            }
                    }
                }
                else
                {
                    bool result = short.TryParse(args[i], out colour_number); //max_colours is now a number 
                    if (System.IO.File.Exists(args[i]) && input_file == "")
                    {
                        input_file = args[i];
                    }
                    else if (!result)
                    {
                        output_file = args[i].Split('.')[0];
                    }
                    break;
                }
            }
            if (max_colours == -1)  // if user haven't chosen a texture format
            {
                max_colours = 256;  // let's set CI8 as default
                block_width = 8;
                block_height = 4;
            }
            if (selected_palette == null)
            {
                selected_palette = create_PLT0_auto;
            }
            if (input_file == "")
            {
                Console.WriteLine("no input file specified\nusage: PLT0 [Encoding Format] [Palette Format] [c colour number] [m mipmaps number] [g2|cc 0.7 0.369 0.4 1.0] <file> [dest]\nthis is the usage format for parameters : [optional] <mandatory>\navailable palette formats : IA8 (Black and White), RGB565 (RGB), RGB5A3 (RGBA), default = (auto)\nAvailable Encoding Formats: C4, CI4, C8, CI8, CI14x2, C14x2, default = CI8\nthe number of colours is stored on 2 bytes, but the index bit length of the colour encoding format limits the max number of colours to these:\nCI4 : from 0 to 16\nCI8 : from 0 to 256\nCI14x2 : from 0 to 16384\n\ntype g2 to use grayscale recommendation CIE 709, the default one used is the recommendation CIE 601.\ntype cc then your float RGBA factors to multiply every pixel of the input image by this factor\n-m | --n-mm | m | --n-mipmaps = mipmaps number (number of duplicates versions of lower resolution of the input image stored int he output texture)\n\nExamples:\nplt0 rosalina.png -x ci8 rgb5a3 --n-mipmaps 5 -c 256\nplt0 ci4 rosalina.jpeg AI8 c 15 g2 m 1 texture.tex0");
            }
            if (colour_number > max_colours && max_colours == 16)
            {
                Console.WriteLine("CI4 can only supports up to 16 colours as each pixel index is stored on 4 bits");
            }
            if (colour_number > max_colours && max_colours == 256)
            {
                Console.WriteLine("CI8 can only supports up to 256 colours as each pixel index is stored on 8 bits");
            }
            if (colour_number > max_colours && max_colours == 16385)
            {
                Console.WriteLine("CI14x2 can only supports up to 16385 colours as each pixel index is stored on 14 bits");
            }
            if (bmp_image[0x15] != 0 || bmp_image[0x14] != 0 || bmp_image[0x19] != 0 || bmp_image[0x18] != 0)
            {
                Console.WriteLine("Textures Dimensions are too high for a TEX0. Maximum dimensions are the values of a 2 bytes integer (65535 x 65535)");
            }

            /***** BMP File Process *****/

            // process the bmp file
            int bmp_filesize = bmp_image[2]| bmp_image[3] << 8 | bmp_image[4]<< 16 | bmp_image[5] << 24;
            int pixel_data_start_offset = bmp_image[10] | bmp_image[11] << 8 | bmp_image[12] << 16 | bmp_image[13] << 24;
            bitmap_width = (short)(bmp_image[0x13] << 8 | bmp_image[0x12]);
            bitmap_height = (short)(bmp_image[0x17] << 8 | bmp_image[0x16]);
            index_size = bitmap_width * bitmap_height * 2;
            // byte[] sorted_pixel_bytes = new byte[bmp_filesize - pixel_data_start_offset];
            /* switch (bmp_image[0x1C])
            {
                case 24:
                    {
                        int i = pixel_data_start_offset;
                        int j = 0;
                        int row_length = (bitmap_width * 3)  # the row length in bytes
                        int row_padding = Math.Abs(4 - row_length) % 4;
                        while (i < bmp_filesize)
                        {
                            if (j + row_padding == row_length)
                            {
                                i += row_padding;
                                j = 0;
                                continue;
                            }
                            sorted_pixel_bytes[i] = (bmp_image[i]);  // B
                            sorted_pixel_bytes[i + 1] = (bmp_image[i + 1]);  // G
                            sorted_pixel_bytes[i + 2] = (bmp_image[i + 2]);  // R
                            sorted_pixel_bytes[i+3] = (255);  // A
                            j += 3;
                            i += 3;
                        }
                    }
            }*/
            Array.Resize(ref colour_palette, colour_number_x2);
            List<byte[]> index_list = new List<byte[]>();
            object v = selected_palette(bmp_image, bmp_filesize, pixel_data_start_offset);
            index_list.Add((byte[])v);
            for (int i = 1; i < mipmaps_number; i++)
            {

                using (var ms = new MemoryStream())
                {
                    ms.Read(bmp_image, 0, bmp_image.Length);
                    Bitmap image = (Bitmap)Bitmap.FromStream(ms);
                    image.Save(ms, ImageFormat.Bmp);
                    image = ResizeImage(image, (int)(bitmap_width / Math.Pow(i, 2)), (int)(bitmap_height / Math.Pow(i, 2)));
                    bmp_image = Convert_to_bmp((Bitmap)image);
                }
                v = selected_index(bmp_image, bmp_filesize, pixel_data_start_offset);
                index_list.Add((byte[])v);
            }
            write_PLT0();
            write_TEX0(index_list);
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

        public void write_PLT0()
        {
            int size = 0x40 + colour_palette.Length;
            byte size2 = (byte)(4 + Math.Abs(16 - size) % 16);
            byte len = (byte)output_file.Split('\\').Length;
            string file_name = (output_file.Split('\\')[len - 1]);
            byte[] data2 = new byte[size2 + len + (16 - len) % 16];
            byte[] data = new byte[64];  // header data
            for (int i = 0; i < size2; i++)
            {
                data2[i] = 0;
            }
            for (int i = 0; i < len; i++)
            {
                data2[i+size2] = (byte)file_name[i];
            }
            for (int i = size2+len; i < data2.Length; i++)
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
            using (System.IO.FileStream file = System.IO.File.Open(output_file + ".plt0", System.IO.FileMode.Open, System.IO.FileAccess.Write))
            {
                file.Write(data, 0, 40);
                file.Write(colour_palette, 0, colour_palette.Length);
                file.Write(data2, 0, data2.Length);
                file.Close();
            }
        }
        public void write_TEX0(List<byte[]> index_list)
        {
            int size = 0x40;
            for (int i = 0; i < index_list.Count; i++)
            {
                size += index_list[i].Length;
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
            for (int i = 0; i < len; i++)
            {
                data2[i + size2] = (byte)file_name[i];
            }
            for (int i = size2 + len; i < data2.Length; i++)
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
            data[28] = (byte)(bitmap_width >> 8);
            data[29] = (byte)bitmap_width;
            data[30] = (byte)(bitmap_height >> 8);
            data[31] = (byte)bitmap_height;
            data[32] = texture_format_int32[0];
            data[33] = texture_format_int32[1];
            data[34] = texture_format_int32[2];
            data[35] = texture_format_int32[3];
            data[36] = (byte)(colour_number >> 8);
            data[37] = (byte)(colour_number);
            data[38] = 0;
            data[39] = 0;
            data[40] = 0;
            data[41] = (byte)(mipmaps_number + 1);
            data[42] = 0;
            data[43] = 0;
            data[44] = 0;
            data[45] = 0;  // always zero
            data[46] = mipmap_float[0];
            data[47] = mipmap_float[1];
            data[48] = mipmap_float[2];
            data[49] = mipmap_float[3];
            for (int i = 50; i < 64; i++)
            {
                data[i] = 0;
            }
            using (System.IO.FileStream file = System.IO.File.Open(output_file + ".plt0", System.IO.FileMode.Open, System.IO.FileAccess.Write))
            {
                file.Write(data, 0, 40);
                file.Write(colour_palette, 0, colour_palette.Length);
                file.Write(data2, 0, data2.Length);
                file.Close();
            }
        }
        public byte[] build_mipmaps_IA8(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {

            IDictionary<byte[], int> Colours = new Dictionary<byte[], int>();
            byte[] index = new byte[index_size];
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            switch (bmp_image[0x1C])
            {
                case 32:
                    {
                        switch (algorithm)
                        {
                            case 0: // cie_601
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        byte[] pixel = { 0, 0 };
                                        pixel[0] = bmp_image[i + 3];
                                        if (bmp_image[i + 3] == 0)
                                        {
                                            pixel[1] = 0;
                                        }
                                        else
                                        {
                                            pixel[1] = (byte)(bmp_image[i] * 0.114 + bmp_image[i + 1] * 0.587 + bmp_image[i + 2] * 0.299);
                                        }
                                        if (Colours.ContainsKey(pixel))
                                        {
                                            Colours[pixel]++;
                                        }
                                        else
                                        {
                                            Colours.Add(pixel, 0);
                                        }
                                    }
                                    break;
                                }
                            case 1: // cie_709
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        byte[] pixel = { 0, 0 };
                                        pixel[0] = bmp_image[i + 3];
                                        if (bmp_image[i + 3] == 0)
                                        {
                                            pixel[1] = 0;
                                        }
                                        else
                                        {
                                            pixel[1] = (byte)(bmp_image[i] * 0.0721 + bmp_image[i + 1] * 0.7154 + bmp_image[i + 2] * 0.2125);
                                        }
                                        if (Colours.ContainsKey(pixel))
                                        {
                                            Colours[pixel]++;
                                        }
                                        else
                                        {
                                            Colours.Add(pixel, 0);
                                        }
                                    }
                                    break;
                                }
                            case 2:  // custom
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        byte[] pixel = { 0, 0 };
                                        pixel[0] = (byte)(bmp_image[i + 3] * custom_rgba[3]);
                                        if (pixel[0] == 0)
                                        {
                                            pixel[1] = 0;
                                        }
                                        else
                                        {
                                            pixel[1] = (byte)(bmp_image[i] * custom_rgba[2] + bmp_image[i + 1] * custom_rgba[1] + bmp_image[i + 2] * custom_rgba[0]);
                                        }
                                        if (Colours.ContainsKey(pixel))
                                        {
                                            Colours[pixel]++;
                                        }
                                        else
                                        {
                                            Colours.Add(pixel, 0);
                                        }
                                    }
                                    break;
                                }

                        }
                        var ordered = Colours.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        short j = 0;
                        foreach (var colour in Colours)
                        {

                            for (int i = 0; i < colour_palette.Length; i += 2)
                            {
                                if (colour_palette[i] == colour.Key[0] && colour_palette[i + 1] == colour.Key[1])
                                {
                                    diff_min_index = (byte)(i / 2);
                                    break;
                                }
                                else
                                {
                                    diff = (short)(Math.Abs(colour_palette[i] - colour.Key[0]) + Math.Abs(colour_palette[i + 1] - colour.Key[1]));
                                    if (diff < diff_min)
                                    {
                                        diff_min = diff;
                                        diff_min_index = (byte)(i / 2);
                                    }
                                }
                            }
                            index[j] = diff_min_index;
                            j += 1;
                        }
                        break;
                    } // end of case depth = 32-bit BMP
            } // end of switch
            return index;
        }
        public byte[] create_PLT0_IA8(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {
            // Dictionary<short, int> Colours = new Dictionary<short, int>();
            // System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<short, int>> Colours;
            short pixel;
            byte[] index = new byte[index_size];
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            switch (bmp_image[0x1C])
            {
                case 32:
                    {
                        switch(algorithm)
                        {
                            case 0: // cie_601
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        pixel = bmp_image[i + 3];
                                        if (bmp_image[i + 3] != 0)
                                        {
                                            pixel += (short)(((byte)(bmp_image[i] * 0.114 + bmp_image[i + 1] * 0.587 + bmp_image[i + 2] * 0.299)) << 4);
                                        }
                                        if (Colours.
                                        {
                                            Colours[pixel]++;
                                        }
                                        else
                                        {
                                            Colours.Add((pixel, 0));
                                        }
                                    }
                                    break;
                                }
                            case 1: // cie_709
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        pixel = bmp_image[i + 3];
                                        if (bmp_image[i + 3] != 0)
                                        {
                                            pixel += (short)(((byte)(bmp_image[i] * 0.0721 + bmp_image[i + 1] * 0.7154 + bmp_image[i + 2] * 0.2125)) << 4);
                                        }
                                        if (Colours.ContainsKey(pixel))
                                        {
                                            Colours[pixel]++;
                                        }
                                        else
                                        {
                                            Colours.Add(pixel, 0);
                                        }
                                    }
                                    break;
                                }
                            case 2:  // custom
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        pixel = (byte)(bmp_image[i + 3] * custom_rgba[3]);
                                        if (pixel != 0)
                                        {
                                            pixel += (short)((byte)(bmp_image[i] * custom_rgba[2] + bmp_image[i + 1] * custom_rgba[1] + bmp_image[i + 2] * custom_rgba[0]) << 4);
                                        }
                                        if (Colours.ContainsKey(pixel))
                                        {
                                            Colours[pixel]++;
                                        }
                                        else
                                        {
                                            Colours.Add(pixel, 0);
                                        }
                                    }
                                    break;
                                }

                        }
                        // Dictionary<short, int> ordered = Colours.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        // var ordered = Colours.OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                        // var ordered = from entry in Colours orderby entry.Value ascending select entry;
                        //Colours = Colours.ToList();
                        Colours.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
                        // Dictionary<short, int> ordered = Colours.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        // var ordered = Colours.OrderByDescending(pair => pair.Value).Take(colour_number);
                        // Dictionary<short, int> ordered = Colours.OrderByDescending(pair => pair.Value).Take(colour_number).ToDictionary(pair => pair.Key, pair => pair.Value);
                        short j = 0;
                        Console.WriteLine("findind most used colours");
                        foreach (var colour in ordered)
                        {
                            colour_palette[j] = (byte)colour.Key ;
                            colour_palette[j+1] = (byte)(colour.Key >> 8);
                            j += 2;
                            if (j > colour_number_x2 - 2)
                            {
                                break;
                            }
                        }
                        j = 0;
                        Console.WriteLine("creating indexes");
                        foreach (var colour in Colours)
                        {

                            for (int i = 0; i < colour_palette.Length; i += 2)
                            {
                                if (colour_palette[i] == (byte)colour.Key && colour_palette[i + 1] == colour.Key >> 8)
                                {
                                    diff_min_index = (byte)(i / 2);
                                    break;
                                }
                                else
                                {
                                    diff = (short)(Math.Abs(colour_palette[i] - (byte)colour.Key) + Math.Abs(colour_palette[i + 1] - colour.Key >> 4));
                                    if (diff < diff_min)
                                    {
                                        diff_min = diff;
                                        diff_min_index = (byte)(i / 2);
                                    }
                                }
                            }
                            index[j] = diff_min_index;
                            j += 1;
                        }
                        Console.WriteLine("hello");
                        break;
                    } // end of case depth = 32-bit BMP
            } // end of switch
            return index;
            //return pixel;
        }
        public byte[] build_mipmaps_RGB565(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {
            IDictionary<byte[], int> Colours = new Dictionary<byte[], int>();
            byte[] index = new byte[index_size];
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            byte red;
            byte green;
            byte blue;
            switch (bmp_image[0x1C])
            {
                case 32:
                    {
                        if (algorithm == 2)
                        {
                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                            {
                                byte[] pixel = { 0, 0 };
                                red = (byte)(bmp_image[i] * custom_rgba[0]);
                                green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                blue = (byte)(bmp_image[i + 2] * custom_rgba[2]);
                                if ((red & 4) == 4)
                                {
                                    red += 8;  // rounds to the closest colour
                                }
                                if ((green & 2) == 2)
                                {
                                    green += 4;  // rounds to the closest colour
                                }
                                if ((blue & 4) == 4)
                                {
                                    blue += 8;  // rounds to the closest colour
                                }
                                pixel[0] = (byte)(red & 248 + bmp_image[i + 1] >> 5);
                                pixel[1] = (byte)(((green & 28) << 5) + blue >> 3);
                                if (Colours.ContainsKey(pixel))
                                {
                                    Colours[pixel]++;
                                }
                                else
                                {
                                    Colours.Add(pixel, 0);
                                }
                            }
                        }
                        else
                        {
                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                            {
                                byte[] pixel = { 0, 0 };
                                red = bmp_image[i];
                                green = bmp_image[i + 1];
                            blue = bmp_image[i + 2];
                                if ((red & 4) == 4)
                                {
                                    red += 8;  // rounds to the closest colour
                                }
                                if ((green & 2) == 2)
                                {
                                    green += 4;  // rounds to the closest colour
                                }
                                if ((blue & 4) == 4)
                                {
                                    blue += 8;  // rounds to the closest colour
                                }
                                pixel[0] = (byte)(red & 248 + bmp_image[i + 1] >> 5);
                                pixel[1] = (byte)(((green & 28) << 5) + blue >> 3);
                                if (Colours.ContainsKey(pixel))
                                {
                                    Colours[pixel]++;
                                }
                                else
                                {
                                    Colours.Add(pixel, 0);
                                }
                            }
                        }
                        var ordered = Colours.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        short j = 0;
                        foreach (var colour in Colours)
                        {

                            for (int i = 0; i < colour_palette.Length; i += 2)
                            {
                                if (colour_palette[i] == colour.Key[0] && colour_palette[i + 1] == colour.Key[1])
                                {
                                    diff_min_index = (byte)(i / 2);
                                    break;
                                }
                                else
                                {
                                    diff = (short)(Math.Abs(colour_palette[i] - colour.Key[0]) + Math.Abs(colour_palette[i + 1] - colour.Key[1]));
                                    if (diff < diff_min)
                                    {
                                        diff_min = diff;
                                        diff_min_index = (byte)(i / 2);
                                    }
                                }
                            }
                            index[j] = diff_min_index;
                            j += 1;
                        }
                        break;
                    } // end of case depth = 32-bit BMP
            } // end of switch
            return index;
        }
        public byte[] create_PLT0_RGB565(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {
            IDictionary<byte[], int> Colours = new Dictionary<byte[], int>();
            byte[] index = new byte[index_size];
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            byte red;
            byte green;
            byte blue;
            switch (bmp_image[0x1C])
            {
                case 32:
                    {
                        if (algorithm == 2)
                        {
                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                            {
                                byte[] pixel = { 0, 0 };
                                red = (byte)(bmp_image[i] * custom_rgba[0]);
                                green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                blue = (byte)(bmp_image[i + 2] * custom_rgba[2]);
                                if ((red & 4) == 4)
                                {
                                    red += 8;  // rounds to the closest colour
                                }
                                if ((green & 2) == 2)
                                {
                                    green += 4;  // rounds to the closest colour
                                }
                                if ((blue & 4) == 4)
                                {
                                    blue += 8;  // rounds to the closest colour
                                }
                                pixel[0] = (byte)(red & 248 + bmp_image[i + 1] >> 5);
                                pixel[1] = (byte)(((green & 28) << 5) + blue >> 3);
                                if (Colours.ContainsKey(pixel))
                                {
                                    Colours[pixel]++;
                                }
                                else
                                {
                                    Colours.Add(pixel, 0);
                                }
                            }
                        }
                        else
                        {
                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                            {
                                byte[] pixel = { 0, 0 };
                                red = bmp_image[i];
                                green = bmp_image[i + 1];
                                blue = bmp_image[i + 2];
                                if ((red & 4) == 4)
                                {
                                    red += 8;  // rounds to the closest colour
                                }
                                if ((green & 2) == 2)
                                {
                                    green += 4;  // rounds to the closest colour
                                }
                                if ((blue & 4) == 4)
                                {
                                    blue += 8;  // rounds to the closest colour
                                }
                                pixel[0] = (byte)(red & 248 + bmp_image[i + 1] >> 5);
                                pixel[1] = (byte)(((green & 28) << 5) + blue >> 3);
                                if (Colours.ContainsKey(pixel))
                                {
                                    Colours[pixel]++;
                                }
                                else
                                {
                                    Colours.Add(pixel, 0);
                                }
                            }
                        }
                        var ordered = Colours.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        short j = 0;
                        foreach (var colour in ordered)
                        {
                            colour_palette[j] = colour.Key[0];
                            colour_palette[j + 1] = colour.Key[1];
                            j += 2;
                            if (j > colour_number_x2 - 2)
                            {
                                break;
                            }
                        }
                        j = 0;
                        foreach (var colour in Colours)
                        {

                            for (int i = 0; i < colour_palette.Length; i += 2)
                            {
                                if (colour_palette[i] == colour.Key[0] && colour_palette[i + 1] == colour.Key[1])
                                {
                                    diff_min_index = (byte)(i / 2);
                                    break;
                                }
                                else
                                {
                                    diff = (short)(Math.Abs(colour_palette[i] - colour.Key[0]) + Math.Abs(colour_palette[i + 1] - colour.Key[1]));
                                    if (diff < diff_min)
                                    {
                                        diff_min = diff;
                                        diff_min_index = (byte)(i / 2);
                                    }
                                }
                            }
                            index[j] = diff_min_index;
                            j += 1;
                        }
                        break;
                    } // end of case depth = 32-bit BMP
            } // end of switch
            return index;
        }
        public byte[] build_mipmaps_RGB5A3(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {
            IDictionary<byte[], int> Colours = new Dictionary<byte[], int>();
            byte[] index = new byte[index_size];
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            byte red;
            byte green;
            byte blue;
            byte alpha;
            switch (bmp_image[0x1C])
            {
                case 32:
                    {
                        if (algorithm == 2)
                        {
                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                            {
                                byte[] pixel = { 0, 0 };
                                alpha = (byte)(bmp_image[i + 3] * custom_rgba[3]);
                                if (alpha == 0)
                                {
                                    pixel[0] = 0;
                                    pixel[1] = 0;
                                }
                                else
                                {
                                    red = (byte)(bmp_image[i] * custom_rgba[0]);
                                    green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                    blue = (byte)(bmp_image[i + 2] * custom_rgba[2]);
                                    if ((red & 8) == 8)
                                    {
                                        red += 16;  // rounds to the closest colour
                                    }
                                    if ((green & 8) == 8)
                                    {
                                        green += 16;  // rounds to the closest colour
                                    }
                                    if ((blue & 8) == 8)
                                    {
                                        blue += 16;  // rounds to the closest colour
                                    }
                                    if ((alpha & 8) == 8)
                                    {
                                        alpha += 16;  // rounds to the closest colour
                                    }
                                    pixel[0] = (byte)(alpha & 240 + red >> 4);
                                    pixel[1] = (byte)(green & 240 + blue >> 4);
                                }
                                if (Colours.ContainsKey(pixel))
                                {
                                    Colours[pixel]++;
                                }
                                else
                                {
                                    Colours.Add(pixel, 0);
                                }
                            }
                        }
                        else
                        {
                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                            {
                                byte[] pixel = { 0, 0 };
                                alpha = bmp_image[i + 3];
                                if (alpha == 0)
                                {
                                    pixel[0] = 0;
                                    pixel[1] = 0;
                                }
                                else
                                {
                                    red = bmp_image[i];
                                    green = bmp_image[i + 1];
                                    blue = bmp_image[i + 2];
                                    if ((red & 8) == 8)
                                    {
                                        red += 16;  // rounds to the closest colour
                                    }
                                    if ((green & 8) == 8)
                                    {
                                        green += 16;  // rounds to the closest colour
                                    }
                                    if ((blue & 8) == 8)
                                    {
                                        blue += 16;  // rounds to the closest colour
                                    }
                                    if ((alpha & 8) == 8)
                                    {
                                        alpha += 16;  // rounds to the closest colour
                                    }
                                    pixel[0] = (byte)(alpha & 240 + red >> 4);
                                    pixel[1] = (byte)(green & 240 + blue >> 4);
                                }
                                if (Colours.ContainsKey(pixel))
                                {
                                    Colours[pixel]++;
                                }
                                else
                                {
                                    Colours.Add(pixel, 0);
                                }
                            }
                        }
                        var ordered = Colours.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        short j = 0;
                        foreach (var colour in Colours)
                        {

                            for (int i = 0; i < colour_palette.Length; i += 2)
                            {
                                if (colour_palette[i] == colour.Key[0] && colour_palette[i + 1] == colour.Key[1])
                                {
                                    diff_min_index = (byte)(i / 2);
                                    break;
                                }
                                else
                                {
                                    diff = (short)(Math.Abs(colour_palette[i] - colour.Key[0]) + Math.Abs(colour_palette[i + 1] - colour.Key[1]));
                                    if (diff < diff_min)
                                    {
                                        diff_min = diff;
                                        diff_min_index = (byte)(i / 2);
                                    }
                                }
                            }
                            index[j] = diff_min_index;
                            j += 1;
                        }
                        break;
                    } // end of case depth = 32-bit BMP
            } // end of switch
            return index;
        }
        public byte[] create_PLT0_RGB5A3(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {
            IDictionary<byte[], int> Colours = new Dictionary<byte[], int>();
            byte[] index = new byte[index_size];
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            byte red;
            byte green;
            byte blue;
            byte alpha;
            switch (bmp_image[0x1C])
            {
                case 32:
                    {
                        if (algorithm == 2)
                        {
                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                            {
                                byte[] pixel = { 0, 0 };
                                alpha = (byte)(bmp_image[i + 3] * custom_rgba[3]);
                                if (alpha == 0)
                                {
                                    pixel[0] = 0;
                                    pixel[1] = 0;
                                }
                                else
                                {
                                    red = (byte)(bmp_image[i] * custom_rgba[0]);
                                    green = (byte)(bmp_image[i + 1] * custom_rgba[1]);
                                    blue = (byte)(bmp_image[i + 2] * custom_rgba[2]);
                                    if ((red & 8) == 8)
                                    {
                                        red += 16;  // rounds to the closest colour
                                    }
                                    if ((green & 8) == 8)
                                    {
                                        green += 16;  // rounds to the closest colour
                                    }
                                    if ((blue & 8) == 8)
                                    {
                                        blue += 16;  // rounds to the closest colour
                                    }
                                    if ((alpha & 8) == 8)
                                    {
                                        alpha += 16;  // rounds to the closest colour
                                    }
                                    pixel[0] = (byte)(alpha & 240 + red >> 4);
                                    pixel[1] = (byte)(green & 240 + blue >> 4);
                                }
                                if (Colours.ContainsKey(pixel))
                                {
                                    Colours[pixel]++;
                                }
                                else
                                {
                                    Colours.Add(pixel, 0);
                                }
                            }
                        }
                        else
                        {
                            for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                            {
                                byte[] pixel = { 0, 0 };
                                alpha = bmp_image[i + 3];
                                if (alpha == 0)
                                {
                                    pixel[0] = 0;
                                    pixel[1] = 0;
                                }
                                else
                                {
                                    red = bmp_image[i];
                                    green = bmp_image[i + 1];
                                    blue = bmp_image[i + 2];
                                    if ((red & 8) == 8)
                                    {
                                        red += 16;  // rounds to the closest colour
                                    }
                                    if ((green & 8) == 8)
                                    {
                                        green += 16;  // rounds to the closest colour
                                    }
                                    if ((blue & 8) == 8)
                                    {
                                        blue += 16;  // rounds to the closest colour
                                    }
                                    if ((alpha & 8) == 8)
                                    {
                                        alpha += 16;  // rounds to the closest colour
                                    }
                                    pixel[0] = (byte)(alpha & 240 + red >> 4);
                                    pixel[1] = (byte)(green & 240 + blue >> 4);
                                }
                                if (Colours.ContainsKey(pixel))
                                {
                                    Colours[pixel]++;
                                }
                                else
                                {
                                    Colours.Add(pixel, 0);
                                }
                            }
                        }
                        var ordered = Colours.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        short j = 0;
                        foreach (var colour in ordered)
                        {
                            colour_palette[j] = colour.Key[0];
                            colour_palette[j + 1] = colour.Key[1];
                            j += 2;
                            if (j > colour_number_x2 - 2)
                            {
                                break;
                            }
                        }
                        j = 0;
                        foreach (var colour in Colours)
                        {

                            for (int i = 0; i < colour_palette.Length; i += 2)
                            {
                                if (colour_palette[i] == colour.Key[0] && colour_palette[i + 1] == colour.Key[1])
                                {
                                    diff_min_index = (byte)(i / 2);
                                    break;
                                }
                                else
                                {
                                    diff = (short)(Math.Abs(colour_palette[i] - colour.Key[0]) + Math.Abs(colour_palette[i + 1] - colour.Key[1]));
                                    if (diff < diff_min)
                                    {
                                        diff_min = diff;
                                        diff_min_index = (byte)(i / 2);
                                    }
                                }
                            }
                            index[j] = diff_min_index;
                            j += 1;
                        }
                        break;
                    } // end of case depth = 32-bit BMP
            } // end of switch
            return index;
        }
        public byte[] create_PLT0_auto(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {
            return bmp_image;
        }
        public byte[] Convert_to_bmp(System.Drawing.Bitmap imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

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
