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
        delegate object palette_function(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset);
        delegate object index_function(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset);
        byte[] texture_format_int32 = {0, 0, 0, 8};
        byte[] palette_format_int32 = { 0, 0, 0, 0 };
        byte[] colour_palette;
        byte mipmaps_number = 0;
        sbyte block_width = -1;
        sbyte block_height = -1;
        bool alpha = false;
        bool grey = false;
        bool tpl = false;
        // int index_size;
        ushort bitmap_width;
        ushort bitmap_height;
        short colour_number = -1;
        short colour_number_x2;
        short max_colours = -1;
        string input_file = "";
        string output_file = "";
        byte pass = 0;
        float[] custom_rgba = { 1, 1, 1, 1 };
        byte algorithm = 0;  // 0 = CIE 601    1 = CIE 709     2 = custom RGBA

        [MethodImpl(MethodImplOptions.NoOptimization)]
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
                        case "TPL":
                            {
                                tpl = true;
                                break;
                            }
                        default:
                            {
                                bool result = short.TryParse(args[i], out colour_number); //colour_number is now a number 
                                colour_number_x2 = (short)(colour_number * 2);
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
                    colour_number_x2 = (short)(colour_number * 2);
                    if (System.IO.File.Exists(args[i]) && input_file == "")
                    {
                        input_file = args[i];
                    }
                    else if (!result)
                    {
                        output_file = args[i].Split('\\')[args[i].Split('\\').Length].Split('.')[0];
                    }
                    break;
                }
            }
            bool stay = true;
            if (max_colours == -1)  // if user haven't chosen a texture format
            {
                max_colours = 256;  // let's set CI8 as default
                block_width = 8;
                block_height = 4;
            }
            if (colour_number == -1)
            {
                colour_number = max_colours;
                colour_number_x2 = (short)(colour_number * 2);
            }
            if (input_file == "")
            {
                Console.WriteLine("no input file specified\nusage: PLT0 [tpl] [Encoding Format] [Palette Format] [c colour number] [m mipmaps number] [g2|cc 0.7 0.369 0.4 1.0] <file> [dest]\nthis is the usage format for parameters : [optional] <mandatory>\nif you don't specify tpl, this program will output by default a tex0 and a plt0 file.\navailable palette formats : IA8 (Black and White), RGB565 (RGB), RGB5A3 (RGBA), default = (auto)\nAvailable Encoding Formats: C4, CI4, C8, CI8, CI14x2, C14x2, default = CI8\nthe number of colours is stored on 2 bytes, but the index bit length of the colour encoding format limits the max number of colours to these:\nCI4 : from 0 to 16\nCI8 : from 0 to 256\nCI14x2 : from 0 to 16384\n\ntype g2 to use grayscale recommendation CIE 709, the default one used is the recommendation CIE 601.\ntype cc then your float RGBA factors to multiply every pixel of the input image by this factor\n-m | --n-mm | m | --n-mipmaps = mipmaps number (number of duplicates versions of lower resolution of the input image stored int he output texture)\n\nExamples:\nplt0 rosalina.png -x ci8 rgb5a3 --n-mipmaps 5 -c 256\nplt0 ci4 rosalina.jpeg AI8 c 15 g2 m 1 texture.tex0");
                stay = false;
            }
            if (output_file == "")
            {
                output_file = input_file.Split('\\')[input_file.Split('\\').Length - 1].Split('.')[0];
            }
            if (colour_number > max_colours && max_colours == 16)
            {
                Console.WriteLine("CI4 can only supports up to 16 colours as each pixel index is stored on 4 bits");
                stay = false;
            }
            if (colour_number > max_colours && max_colours == 256)
            {
                Console.WriteLine("CI8 can only supports up to 256 colours as each pixel index is stored on 8 bits");
                stay = false;
            }
            if (colour_number > max_colours && max_colours == 16385)
            {
                Console.WriteLine("CI14x2 can only supports up to 16385 colours as each pixel index is stored on 14 bits");
                stay = false;
            }
            if (bmp_image[0x15] != 0 || bmp_image[0x14] != 0 || bmp_image[0x19] != 0 || bmp_image[0x18] != 0)
            {
                Console.WriteLine("Textures Dimensions are too high for a TEX0. Maximum dimensions are the values of a 2 bytes integer (65535 x 65535)");
                stay = false;

            }

            /***** BMP File Process *****/
            if (stay)
            {
                // process the bmp file
                int bmp_filesize = bmp_image[2] | bmp_image[3] << 8 | bmp_image[4] << 16 | bmp_image[5] << 24;
                int pixel_data_start_offset = bmp_image[10] | bmp_image[11] << 8 | bmp_image[12] << 16 | bmp_image[13] << 24;
                bitmap_width = (ushort)(bmp_image[0x13] << 8 | bmp_image[0x12]);
                bitmap_height = (ushort)(bmp_image[0x17] << 8 | bmp_image[0x16]);
                // index_size = bitmap_width * bitmap_height;
                if (selected_palette == null)
                {
                    if (bmp_image[pixel_data_start_offset] == bmp_image[pixel_data_start_offset + 1] && bmp_image[pixel_data_start_offset] == bmp_image[pixel_data_start_offset + 2] && bmp_image[pixel_data_start_offset + ((bitmap_width - 1) * 4)] == bmp_image[pixel_data_start_offset + ((bitmap_width - 1) * 4) + 1] && bmp_image[pixel_data_start_offset + ((bitmap_width - 1) * 4)] == bmp_image[pixel_data_start_offset + ((bitmap_width - 1) * 4) + 2] && bmp_image[pixel_data_start_offset + ((bitmap_height - 1) * 4)] == bmp_image[pixel_data_start_offset + ((bitmap_height - 1) * 4) + 1] && bmp_image[pixel_data_start_offset + ((bitmap_height - 1) * 4)] == bmp_image[pixel_data_start_offset + ((bitmap_height - 1) * 4) + 2])
                    {
                        grey = true;
                        selected_palette = create_PLT0_IA8;
                        selected_index = build_mipmaps_IA8;
                    }
                    else if (bmp_image[pixel_data_start_offset + 3] != 0xff || bmp_image[pixel_data_start_offset + ((bitmap_width - 1) * 4) + 3] != 0xff || bmp_image[pixel_data_start_offset + ((bitmap_height - 1) * 4) + 3] != 0xff)
                    {
                        alpha = true;
                        selected_palette = create_PLT0_RGB5A3;
                        selected_index = build_mipmaps_RGB5A3;
                        palette_format_int32[3] = 2;
                    }
                    else
                    {
                        selected_palette = create_PLT0_RGB565;
                        selected_index = build_mipmaps_RGB565;
                        palette_format_int32[3] = 1;
                    }
                }
                ushort[] vanilla_size = {bitmap_width, bitmap_height};
                Array.Resize(ref colour_palette, colour_number_x2);
                List<List<byte[]>> index_list = new List<List<byte[]>>();
                object v = selected_palette(bmp_image, bmp_filesize, pixel_data_start_offset);
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
                    v = selected_index(bmp_image, bmp_filesize, pixel_data_start_offset);
                    index_list.Add((List<byte[]>)v);
                }
                bitmap_width = vanilla_size[0];
                bitmap_height = vanilla_size[1];
                if (tpl)
                {
                    write_TPL(index_list);
                }
                else
                {
                    write_PLT0();
                    write_TEX0(index_list);
                }
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
        public void write_TPL(List<List<byte[]>> index_list)  // index_list contains all mipmaps.
        {

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
                data2[i+size2] = (byte)file_name[i];
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
            using (System.IO.FileStream file = System.IO.File.Open(output_file + ".plt0", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
            {
                file.Write(data, 0, 64);
                file.Write(colour_palette, 0, colour_palette.Length);
                file.Write(data2, 0, data2.Length);
                file.Close();
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
                    param[2] = bitmap_width;
                    param[3] = bitmap_height;
                }
                else
                {
                    temp = bitmap_width / Math.Pow(i, 2);
                    if (temp % 1 != 0)
                    {
                        param[2] = (int)temp + 1;
                    }
                    else
                    {
                        param[2] = (int)temp;
                    }
                    temp = bitmap_height / Math.Pow(i, 2);
                    if (temp % 1 != 0)
                    {
                        param[3] = (int)temp + 1;
                    }
                    else
                    {
                        param[3] = (int)temp;
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
                size += param[0] * block_width * param[1] * block_height;
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
            byte[] tex_data = new byte[size];
            int count = 0;
            int height;
            int width;
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

            using (System.IO.FileStream file = System.IO.File.Open(output_file + ".tex0", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
            {
                file.Write(data, 0, 64);
                file.Write(tex_data, 0, size);
                file.Write(data2, 0, data2.Length);
                file.Close();
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public List<byte[]> build_mipmaps_IA8(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {  // no colour table with mipmaps, as colour_palette is already filled
            List<short> Colours = new List<short>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
            short pixel = (short)((bitmap_width / block_width) + 1);
            if (pixel % 1 != 0)
            {
                pixel = (short)((bitmap_width / block_width) + 1);
            }
            byte[] index = new byte[pixel]; // will contains a row of pixel index starting by the bottom one because bmp_image starts by the bottom one
            List<byte[]> index_list = new List<byte[]>(); // will contains each row of index
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            switch (bmp_image[0x1C])  // colour depth
            {
                case 32:  // 32-bit BGRA bmp image
                    {
                        switch (algorithm)
                        {
                            case 0: // cie_601
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
                                    {
                                        pixel = (short)(bmp_image[i + 3] << 8);  // alpha value
                                        if (bmp_image[i + 3] != 0)
                                        {
                                            pixel += (short)((byte)(bmp_image[i] * 0.114 + bmp_image[i + 1] * 0.587 + bmp_image[i + 2] * 0.299));
                                        }
                                        Colours.Add(pixel);
                                    }
                                    break;
                                }
                            case 1: // cie_709
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        pixel = (short)(bmp_image[i + 3] << 8);  // alpha value
                                        if (bmp_image[i + 3] != 0)
                                        {
                                            pixel += (short)((byte)(bmp_image[i] * 0.0721 + bmp_image[i + 1] * 0.7154 + bmp_image[i + 2] * 0.2125));
                                        }
                                        Colours.Add(pixel);
                                    }
                                    break;
                                }
                            case 2:  // custom
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        pixel = (short)((byte)(bmp_image[i + 3] * custom_rgba[3]) << 8);  // alpha value
                                        if (pixel != 0)
                                        {
                                            pixel += (short)(byte)(bmp_image[i] * custom_rgba[2] + bmp_image[i + 1] * custom_rgba[1] + bmp_image[i + 2] * custom_rgba[0]);
                                        }
                                        Colours.Add(pixel);
                                    }
                                    break;
                                }
                        }
                        short j = 0;
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
                            pixel = (short)(bitmap_width % block_width);
                            if (pixel != 0) // fills in the lacking block data by adding zeros to the width
                            {
                                for (; pixel < block_width; pixel++)
                                {
                                    index[pixel] = 0;
                                }
                            }
                            index_list.Add(index.ToArray());
                        }
                        pixel = (short)(bitmap_height % block_height);
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
        public List<byte[]> create_PLT0_IA8(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
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
            if ((bitmap_width / block_width) % 1 != 0)
            {
                pixel = (ushort)(((ushort)(bitmap_width / block_width) + 1) * block_width);
            }
            byte[] index = new byte[pixel]; // will contains a row of pixel index starting by the bottom one because bmp_image starts by the bottom one
            List<byte[]> index_list = new List<byte[]>(); // will contains each row of index
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            switch (bmp_image[0x1C])  // colour depth
            {
                case 32:  // 32-bit BGRA bmp image
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
                        int j = 0;
                        Console.WriteLine("findind most used Colours");
                        for (int i = 0; i < colour_number; i++)
                        {
                            colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Alpha value
                            colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                            j += 2;
                        }
                        Console.WriteLine("creating indexes");
                        j = 0;
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
                            pixel = (ushort)(bitmap_width % block_width);
                            if (pixel != 0) // fills in the lacking block data by adding zeros to the width
                            {
                                for (; pixel < block_width; pixel++)
                                {
                                    index[pixel] = 0;
                                }
                            }
                            index_list.Add(index.ToArray());
                        }
                        pixel = (ushort)(bitmap_height % block_height);
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
        public List<byte[]> build_mipmaps_RGB565(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {  // mipmaps
            List<short> Colours = new List<short>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
            short pixel = (short)((bitmap_width / block_width) + 1);
            if (pixel % 1 != 0)
            {
                pixel = (short)((bitmap_width / block_width) + 1);
            }
            byte[] index = new byte[pixel]; // will contains a row of pixel index starting by the bottom one because bmp_image starts by the bottom one
            List<byte[]> index_list = new List<byte[]>(); // will contains each row of index
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            switch (bmp_image[0x1C])  // colour depth
            {
                case 32:  // 32-bit BGRA bmp image
                    {
                        switch (algorithm)
                        {
                            case 2:  // custom  RRRR RGGG GGGB BBBB
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        pixel = (short)((((byte)(bmp_image[i + 2] * custom_rgba[0]) >> 3) << 11) + (((byte)(bmp_image[i + 1] * custom_rgba[1]) >> 2) << 5) + (byte)(bmp_image[i] * custom_rgba[2]) >> 3);
                                        Colours.Add(pixel);
                                    }

                                    break;
                                }
                            default: // RRRR RGGG GGGB BBBB
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        pixel = (short)(((bmp_image[i + 2] >> 3) << 11) + ((bmp_image[i + 1] >> 2) << 5) + (bmp_image[i] >> 3));
                                        Colours.Add(pixel);
                                    }
                                    break;
                                }
                        }
                        short j = 0;
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
                            pixel = (short)(bitmap_width % block_width);
                            if (pixel != 0) // fills in the lacking block data by adding zeros to the width
                            {
                                for (; pixel < block_width; pixel++)
                                {
                                    index[pixel] = 0;
                                }
                            }
                            index_list.Add(index.ToArray());
                        }
                        pixel = (short)(bitmap_height % block_height);
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
        public List<byte[]> create_PLT0_RGB565(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {
            List<short> Colours = new List<short>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
            List<int[]> Colour_Table = new List<int[]>();  // {occurence, every possible salues of a short} used to find the most used colours, and then build a colour_palette from these
            int[] colour = { 0, 0 };
            for (int i = 0; i < 65536; i++)
            {
                colour[1] = i;
                Colour_Table.Add(colour.ToArray());  // adds a copy of the current array, so it won't be linked after changes on next loop
            }
            short pixel = (short)((bitmap_width / block_width) + 1);
            if (pixel % 1 != 0)
            {
                pixel = (short)((bitmap_width / block_width) + 1);
            }
            byte[] index = new byte[pixel]; // will contains a row of pixel index starting by the bottom one because bmp_image starts by the bottom one
            List<byte[]> index_list = new List<byte[]>(); // will contains each row of index
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            switch (bmp_image[0x1C])  // colour depth
            {
                case 32:  // 32-bit BGRA bmp image
                    {
                        switch (algorithm)
                        {
                            case 2:  // custom  RRRR RGGG GGGB BBBB
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        pixel = (short)((((byte)(bmp_image[i + 2] * custom_rgba[0]) >> 3) << 11) + (((byte)(bmp_image[i + 1] * custom_rgba[1]) >> 2) << 5) + (byte)(bmp_image[i] * custom_rgba[2]) >> 3);
                                        Colours.Add(pixel);
                                        Colour_Table[pixel][0] += 1;
                                    }

                                    break;
                                }
                            default: // RRRR RGGG GGGB BBBB
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        pixel = (short)(((bmp_image[i + 2] >> 3) << 11) + ((bmp_image[i + 1] >> 2) << 5) + (bmp_image[i] >> 3));
                                        Colours.Add(pixel);
                                        Colour_Table[pixel][0] += 1;
                                    }
                                    break;
                                }
                        }
                        Colour_Table.Sort(new IntArrayComparer());  // sorts the table by the most used colour first
                        short j = 0;
                        Console.WriteLine("findind most used Colours");
                        for (int i = 0; i < colour_number; i++)
                        {
                            colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Alpha value
                            colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                            j += 2;
                        }
                        Console.WriteLine("creating indexes");
                        j = 0;
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
                            pixel = (short)(bitmap_width % block_width);
                            if (pixel != 0) // fills in the lacking block data by adding zeros to the width
                            {
                                for (; pixel < block_width; pixel++)
                                {
                                    index[pixel] = 0;
                                }
                            }
                            index_list.Add(index.ToArray());
                        }
                        pixel = (short)(bitmap_height % block_height);
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
                        Console.WriteLine("hello");
                        break;
                    } // end of case depth = 32-bit BMP
            } // end of switch (colour depth)
            return index_list;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public List<byte[]> build_mipmaps_RGB5A3(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {  // mipmaps
            List<short> Colours = new List<short>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
            short pixel = (short)((bitmap_width / block_width) + 1);
            if (pixel % 1 != 0)
            {
                pixel = (short)((bitmap_width / block_width) + 1);
            }
            byte[] index = new byte[pixel]; // will contains a row of pixel index starting by the bottom one because bmp_image starts by the bottom one
            List<byte[]> index_list = new List<byte[]>(); // will contains each row of index
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            switch (bmp_image[0x1C])  // colour depth
            {
                case 32:  // 32-bit BGRA bmp image
                    {
                        switch (algorithm)
                        {
                            case 2:  // custom
                                {
                                    if (alpha)  // 0AAA RRRR GGGG BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            pixel = (short)((((byte)(bmp_image[i + 3] * custom_rgba[3]) >> 5) << 12) + (((byte)(bmp_image[i + 2] * custom_rgba[0]) >> 4) << 8) + (((byte)(bmp_image[i + 1] * custom_rgba[1]) >> 4) << 4) + (byte)(bmp_image[i] * custom_rgba[2]) >> 4);
                                            Colours.Add(pixel);
                                        }
                                    }
                                    else  // 1RRR RRGG GGGB BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            pixel = (short)((1 << 15) + (((byte)(bmp_image[i + 2] * custom_rgba[0]) >> 3) << 10) + (((byte)(bmp_image[i + 1] * custom_rgba[1]) >> 3) << 5) + (byte)(bmp_image[i] * custom_rgba[2]) >> 3);
                                            Colours.Add(pixel);
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    if (alpha)  // 0AAA RRRR GGGG BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            pixel = (short)(((bmp_image[i + 3] >> 5) << 12) + ((bmp_image[i + 2] >> 4) << 8) + ((bmp_image[i + 1] >> 4) << 4) + (bmp_image[i] >> 4));
                                            Colours.Add(pixel);
                                        }
                                    }
                                    else  // 1RRR RRGG GGGB BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            pixel = (short)((1 << 15) + ((bmp_image[i + 2] >> 3) << 10) + ((bmp_image[i + 1] >> 3) << 5) + (bmp_image[i] >> 3));
                                            Colours.Add(pixel);
                                        }
                                    }
                                    break;
                                }
                        }
                        short j = 0;
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
                            pixel = (short)(bitmap_width % block_width);
                            if (pixel != 0) // fills in the lacking block data by adding zeros to the width
                            {
                                for (; pixel < block_width; pixel++)
                                {
                                    index[pixel] = 0;
                                }
                            }
                            index_list.Add(index.ToArray());
                        }
                        pixel = (short)(bitmap_height % block_height);
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
                        Console.WriteLine("hello");
                        break;
                    } // end of case depth = 32-bit BMP
            } // end of switch (colour depth)
            return index_list;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public List<byte[]> create_PLT0_RGB5A3(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
        {
            List<short> Colours = new List<short>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
            List<int[]> Colour_Table = new List<int[]>();  // {occurence, every possible salues of a short} used to find the most used colours, and then build a colour_palette from these
            int[] colour = { 0, 0 };
            for (int i = 0; i < 65536; i++)
            {
                colour[1] = i;
                Colour_Table.Add(colour.ToArray());  // adds a copy of the current array, so it won't be linked after changes on next loop
            }
            short pixel = (short)((bitmap_width / block_width) + 1);
            if (pixel % 1 != 0)
            {
                pixel = (short)((bitmap_width / block_width) + 1);
            }
            byte[] index = new byte[pixel]; // will contains a row of pixel index starting by the bottom one because bmp_image starts by the bottom one
            List<byte[]> index_list = new List<byte[]>(); // will contains each row of index
            short diff_min = 500;
            short diff = 0;
            byte diff_min_index = 0;
            switch (bmp_image[0x1C])  // colour depth
            {
                case 32:  // 32-bit BGRA bmp image
                    {
                        if (bmp_image[pixel_data_start_offset + 3] != 0xff || bmp_image[pixel_data_start_offset + ((bitmap_width - 1) * 4) + 3] != 0xff || bmp_image[pixel_data_start_offset + ((bitmap_height - 1) * 4) + 3] != 0xff)
                        {
                            alpha = true;
                        }
                        switch (algorithm)
                        {
                            case 2:  // custom
                                {
                                    if (alpha)  // 0AAA RRRR GGGG BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            pixel = (short)((((byte)(bmp_image[i + 3] * custom_rgba[3]) >> 5) << 12) + (((byte)(bmp_image[i + 2] * custom_rgba[0]) >> 4) << 8) + (((byte)(bmp_image[i + 1] * custom_rgba[1]) >> 4) << 4) + (byte)(bmp_image[i] * custom_rgba[2]) >> 4);
                                            Colours.Add(pixel);
                                            Colour_Table[pixel][0] += 1;
                                        }
                                    }
                                    else  // 1RRR RRGG GGGB BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            pixel = (short)((1 << 15) + (((byte)(bmp_image[i + 2] * custom_rgba[0]) >> 3) << 10) + (((byte)(bmp_image[i + 1] * custom_rgba[1]) >> 3) << 5) + (byte)(bmp_image[i] * custom_rgba[2]) >> 3);
                                            Colours.Add(pixel);
                                            Colour_Table[pixel][0] += 1;
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    if (alpha)  // 0AAA RRRR GGGG BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            pixel = (short)(((bmp_image[i + 3] >> 5) << 12) + ((bmp_image[i + 2] >> 4) << 8) + ((bmp_image[i + 1] >> 4) << 4) + (bmp_image[i] >> 4));
                                            Colours.Add(pixel);
                                            Colour_Table[pixel][0] += 1;
                                        }
                                    }
                                    else  // 1RRR RRGG GGGB BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            pixel = (short)((1 << 15) + ((bmp_image[i + 2] >> 3) << 10) + ((bmp_image[i + 1] >> 3) << 5) + (bmp_image[i] >> 3));
                                            Colours.Add(pixel);
                                            Colour_Table[pixel][0] += 1;
                                        }
                                    }
                                    break;
                                }
                        }
                        Colour_Table.Sort(new IntArrayComparer());  // sorts the table by the most used colour first
                        short j = 0;
                        Console.WriteLine("findind most used Colours");
                        for (int i = 0; i < colour_number; i++)
                        {
                            colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Alpha value
                            colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                            j += 2;
                        }
                        Console.WriteLine("creating indexes");
                        j = 0;
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
                            pixel = (short)(bitmap_width % block_width);  
                            if (pixel != 0) // fills in the lacking block data by adding zeros to the width
                            {
                                for (; pixel < block_width ; pixel++)
                                {
                                    index[pixel] = 0;
                                }
                            }
                            index_list.Add(index.ToArray());
                        }
                        pixel = (short)(bitmap_height % block_height);
                        if (pixel != 0)  // fills in the missing block data by adding rows full of zeros
                        {
                            for (int i = 0; i < index.Length ; i++)  // fills in the row with zeros
                            {
                                index[i] = 0;
                            }
                            for (; pixel < block_height; pixel++)  // adds them
                            {
                                index_list.Add(index.ToArray());
                            }
                        }
                        Console.WriteLine("hello");
                        break;
                    } // end of case depth = 32-bit BMP
            } // end of switch (colour depth)
            return index_list;
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
