using System;
using System.Collections.Generic;
using System.Linq;

// I used dependancy injection here because there are too many variables from the main class, AND they're edited for some of them, so passing as function's argument won't work.
// it looks ugly at some parts, sorry about that. I used _plt0 instead of _Parse_args_class to improve readability for a bit.

//also, yes, this is what the peeps call "Encode", but it's not really encoded because you can read it, so I'd rather call it Create.
// technically the "Decode" function is both Decode and Write_bmp
class Create_plt0_class
{
    Parse_args_class _plt0;
    public Create_plt0_class(Parse_args_class Parse_args_class)
    {
        _plt0 = Parse_args_class;
    }
    /// <summary>
    /// Fills the colour palette and process every pixel of the image to be an index of the colour palette
    /// </summary>
    /// <param name="bmp_image">the raw bmp file as a byte array</param>
    /// <param name="_plt0.bmp_filesize">the size of the file, it can be read from the array itself, it's also the length of the array</param>
    /// <param name="_plt0.pixel_data_start_offset">read from the array itself</param>
    /// <returns>a list of each row of the image (starting by the bottom one) and each row is a byte array which contains every pixel of a row.</returns>
    // [MethodImpl(MethodImplOptions.NoOptimization)]
    public List<byte[]> Create_plt0(byte[] bmp_image)
    {
        ushort pixel = _plt0.bitmap_width;
        if (_plt0.bitmap_height % _plt0.block_height != 0)
        {
            _plt0.fill_height = true;
            if (_plt0.bitmap_width % _plt0.block_width != 0)
            {
                if (!_plt0.no_warning)
                    Console.WriteLine("Height is not a multiple of " + _plt0.block_height + " (this value changes for each format), just be aware that the file size would be the same with an image of dimensions " + (_plt0.bitmap_width + (_plt0.block_width - (_plt0.bitmap_width % _plt0.block_width))) + "x" + ((_plt0.bitmap_height + (_plt0.block_height - (_plt0.bitmap_height % _plt0.block_height)))));
            }
            else
            {
                if (!_plt0.no_warning)
                    Console.WriteLine("Height is not a multiple of " + _plt0.block_height + " (this value changes for each format), just be aware that the file size would be the same with an image of dimensions " + _plt0.bitmap_width + "x" + ((_plt0.bitmap_height + (_plt0.block_height - (_plt0.bitmap_height % _plt0.block_height)))));
            }
        }
        if (_plt0.bitmap_width % _plt0.block_width != 0)
        {
            _plt0.fill_width = true;
            if (!_plt0.fill_height)
            {
                if (!_plt0.no_warning)
                    Console.WriteLine("Width is not a multiple of " + _plt0.block_width + " (this value changes for each format), just be aware that the file size would be the same with an image of dimensions " + (_plt0.bitmap_width + (_plt0.block_width - (_plt0.bitmap_width % _plt0.block_width))) + "x" + _plt0.bitmap_height);
            }
            else
            {
                Console.WriteLine("Width is not a multiple of " + _plt0.block_width);
            }
        }
        // pixel = (ushort)(_plt0.bitmap_width + ((_plt0.block_width - (_plt0.bitmap_width % _plt0.block_width)) % _plt0.block_width));  // replaced by _plt0.canvas_width
        pixel = _plt0.canvas_width;
        switch (_plt0.texture_format_int32[3])  // pixel *= reverse_format ratio XD
        {
            case 0:
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
        /* for (ushort i = 0; i < index.Length; i++)  // fills in the row with zeros - not needed since it's overwritten
        {
            index[i] = 0;
        }*/
        List<byte[]> index_list = new List<byte[]>(); // will contains each row of index
        if (bmp_image[0x1C] != 32)
        {
            if (bmp_image[0x1C] == 24)
            {
                // colour depth
                // 24-bit BGRA bmp image
                if (_plt0.has_palette)
                {
                    List<ushort> Colours = new List<ushort>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
                    List<int[]> Colour_Table = new List<int[]>();  // {occurence, every possible salues of a short} used to find the most used colours, and then build a _plt0.colour_palette from these
                    int[] colour = { 0, 0 };
                    for (int i = 0; i < 65536; i++)
                    {
                        colour[1] = i;
                        Colour_Table.Add(colour.ToArray());  // adds a copy of the current array, so it won't be linked after changes on next loop
                    }
                    switch (_plt0.palette_format_int32[3])
                    {
                        case 0:  // AI8
                            Palette_AI8_class palette_ai8 = new Palette_AI8_class(_plt0);
                            palette_ai8.Palette_AI8(index_list, Colour_Table, bmp_image, index);
                            break;
                        case 1:  // RGB565
                            Palette_RGB565_class palette_rgb565 = new Palette_RGB565_class(_plt0);
                            palette_rgb565.Palette_RGB565(index_list, Colour_Table, bmp_image, index);
                            break;
                        case 2:  // RGB5A3
                            Palette_RGB5A3_class palette_rgb5a3 = new Palette_RGB5A3_class(_plt0);
                            palette_rgb5a3.Palette_RGB5A3(index_list, Colour_Table, bmp_image, index);
                            break;
                            // end of case 2 palette RGB5A3
                    }  // end of switch palette format
                }  // end of if (has palette)
                else  // image doesn't have a palette
                {
                    switch (_plt0.texture_format_int32[3])  // splitted into different files in the encode folder, because I'm going to add so many algorithms for CMPR
                    {
                        case 0: // I4
                            I4_class i4_class = new I4_class(_plt0);
                            i4_class.I4(index_list, bmp_image, index);
                            break;

                        case 1: // I8
                            I8_class i8_class = new I8_class(_plt0);
                            i8_class.I8(index_list, bmp_image, index);
                            break;

                        case 2: // IA4
                            AI4_class ai4_class = new AI4_class(_plt0);
                            ai4_class.AI4(index_list, bmp_image, index);
                            break;

                        case 3:  // AI8
                            AI8_class ai8_class = new AI8_class(_plt0);
                            ai8_class.AI8(index_list, bmp_image, index);
                            break;

                        case 4:  // RGB565
                            RGB565_class rgb565_class = new RGB565_class(_plt0);
                            rgb565_class.RGB565(index_list, bmp_image, index);
                            break;

                        case 5:  // RGB5A3
                            RGB5A3_class rgb5a3_class = new RGB5A3_class(_plt0);
                            rgb5a3_class.RGB5A3(index_list, bmp_image, index);
                            break;

                        case 6: // RGBA32
                            RGBA32_class rgba32_class = new RGBA32_class(_plt0);
                            rgba32_class.RGBA32(index_list, bmp_image, index);
                            break;

                        case 0xE: // CMPR
                            CMPR_class cmpr_class = new CMPR_class(_plt0);
                            cmpr_class.CMPR(index_list, bmp_image);
                            break;
                    }
                }
                if (_plt0.reverse_y)
                {
                    List<byte[]> index_reversed = new List<byte[]>();
                    if (_plt0.texture_format_int32[3] == 0xE)
                    {
                        int blocks_wide = _plt0.canvas_width >> 2;
                        int blocks_tall = _plt0.canvas_height >> 3;
                        int h = index_list.Count - (blocks_wide << 1);
                        for (int d = 0; d < blocks_tall; d++)
                        {
                            for (int i = 0, e = 0; e < blocks_wide; i += 4, e += 2)
                            {
                                index_reversed.Add(index_list[h + i + 2]);
                                index_reversed.Add(index_list[h + i + 3]);
                                index_reversed.Add(index_list[h + i]);
                                index_reversed.Add(index_list[h + i + 1]);
                            }
                            h -= blocks_wide << 1;
                        }
                        if (_plt0.reverse_x)
                        {
                            return Reverse_x_class.Reverse_x(_plt0.canvas_width, _plt0.canvas_height, index_reversed, _plt0.texture_format_int32[3]);
                        }
                        return index_reversed;
                    }
                    index_list.Reverse();
                }
                if (_plt0.reverse_x)
                {
                    return Reverse_x_class.Reverse_x(_plt0.canvas_width, _plt0.canvas_height, index_list, _plt0.texture_format_int32[3]);
                }
                return index_list;
            }
            if (!_plt0.no_warning)
                Console.WriteLine("EGAD (colour depth of the converted bmp image is " + bmp_image[0x1C] + ")");
            Environment.Exit(0);
        }
        // colour depth
        // 32-bit BGRA bmp image
        if (_plt0.has_palette)
        {
            List<ushort> Colours = new List<ushort>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
            List<int[]> Colour_Table = new List<int[]>();  // {occurence, every possible salues of a short} used to find the most used colours, and then build a _plt0.colour_palette from these
            int[] colour = { 0, 0 };
            for (int i = 0; i < 65536; i++)
            {
                colour[1] = i;
                Colour_Table.Add(colour.ToArray());  // adds a copy of the current array, so it won't be linked after changes on next loop
            }
            switch (_plt0.palette_format_int32[3])
            {
                case 0:  // AI8
                    Palette_AI8_class palette_ai8 = new Palette_AI8_class(_plt0);
                    palette_ai8.Palette_AI8(index_list, Colour_Table, bmp_image, index);
                    break;
                case 1:  // RGB565
                    Palette_RGB565_class palette_rgb565 = new Palette_RGB565_class(_plt0);
                    palette_rgb565.Palette_RGB565(index_list, Colour_Table, bmp_image, index);
                    break;
                case 2:  // RGB5A3
                    Palette_RGB5A3_class palette_rgb5a3 = new Palette_RGB5A3_class(_plt0);
                    palette_rgb5a3.Palette_RGB5A3(index_list, Colour_Table, bmp_image, index);
                    break;
                    // end of case 2 palette RGB5A3
            }  // end of switch palette format
        }  // end of if (has palette)
        else  // image doesn't have a palette
        {
            switch (_plt0.texture_format_int32[3])  // splitted into different files in the encode folder, because I'm going to add so many algorithms for CMPR
            {
                case 0: // I4
                    I4_class i4_class = new I4_class(_plt0);
                    i4_class.I4(index_list, bmp_image, index);
                    break;

                case 1: // I8
                    I8_class i8_class = new I8_class(_plt0);
                    i8_class.I8(index_list, bmp_image, index);
                    break;

                case 2: // IA4
                    AI4_class ai4_class = new AI4_class(_plt0);
                    ai4_class.AI4(index_list, bmp_image, index);
                    break;

                case 3:  // AI8
                    AI8_class ai8_class = new AI8_class(_plt0);
                    ai8_class.AI8(index_list, bmp_image, index);
                    break;

                case 4:  // RGB565
                    RGB565_class rgb565_class = new RGB565_class(_plt0);
                    rgb565_class.RGB565(index_list, bmp_image, index);
                    break;

                case 5:  // RGB5A3
                    RGB5A3_class rgb5a3_class = new RGB5A3_class(_plt0);
                    rgb5a3_class.RGB5A3(index_list, bmp_image, index);
                    break;

                case 6: // RGBA32
                    RGBA32_class rgba32_class = new RGBA32_class(_plt0);
                    rgba32_class.RGBA32(index_list, bmp_image, index);
                    break;

                case 0xE: // CMPR
                    CMPR_class cmpr_class = new CMPR_class(_plt0);
                    cmpr_class.CMPR(index_list, bmp_image);
                    break;
            }
        }
        if (_plt0.reverse_y)
        {
            List<byte[]> index_reversed = new List<byte[]>();
            if (_plt0.texture_format_int32[3] == 0xE)
            {
                int blocks_wide = _plt0.canvas_width >> 2;
                int blocks_tall = _plt0.canvas_height >> 3;
                int h = index_list.Count - (blocks_wide << 1);
                for (int d = 0; d < blocks_tall; d++)
                {
                    for (int i = 0, e = 0; e < blocks_wide; i += 4, e += 2)
                    {
                        index_reversed.Add(index_list[h + i + 2]);
                        index_reversed.Add(index_list[h + i + 3]);
                        index_reversed.Add(index_list[h + i]);
                        index_reversed.Add(index_list[h + i + 1]);
                    }
                    h -= blocks_wide << 1;
                }
                if (_plt0.reverse_x)
                {
                    return Reverse_x_class.Reverse_x(_plt0.canvas_width, _plt0.canvas_height, index_reversed, _plt0.texture_format_int32[3]);
                }
                return index_reversed;
            }
            index_list.Reverse();
        }
        if (_plt0.reverse_x)
        {
            return Reverse_x_class.Reverse_x(_plt0.canvas_width, _plt0.canvas_height, index_list, _plt0.texture_format_int32[3]);
        }
        return index_list;
    }
}