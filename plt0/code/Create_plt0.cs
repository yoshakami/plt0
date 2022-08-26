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
    /// <param name="bmp_filesize">the size of the file, it can be read from the array itself, it's also the length of the array</param>
    /// <param name="pixel_data_start_offset">read from the array itself</param>
    /// <returns>a list of each row of the image (starting by the bottom one) and each row is a byte array which contains every pixel of a row.</returns>
    // [MethodImpl(MethodImplOptions.NoOptimization)]
    public List<byte[]> Create_plt0(byte[] bmp_image, int bmp_filesize, int pixel_data_start_offset)
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
        for (ushort i = 0; i < index.Length; i++)  // fills in the row with zeros
        {
            index[i] = 0;
        }
        List<byte[]> index_list = new List<byte[]>(); // will contains each row of index
        int j = _plt0.fill_palette_start_offset;
        byte red;
        byte green;
        byte blue;
        byte a;
        byte grey;
        bool not_similar;
        short diff_min = 500;
        short diff = 0;
        byte diff_min_index = 0;
        ushort diff_min_ind14x2 = 0;
        if (bmp_image[0x1C] != 32)
        {
            if (!_plt0.no_warning)
                Console.WriteLine("HOLY SHIT (colour depth of the converted bmp image is " + bmp_image[0x1C] + ")");
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
                    {

                        switch (_plt0.algorithm)
                        {
                            case 0: // cie_601
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
                                    {
                                        pixel = (ushort)(bmp_image[i + _plt0.rgba_channel[3]] << 8);  // _plt0.alpha value
                                        if (bmp_image[i + _plt0.rgba_channel[3]] != 0)
                                        {
                                            pixel += (ushort)((byte)(bmp_image[i + _plt0.rgba_channel[0]] * 0.299) + bmp_image[i + _plt0.rgba_channel[1]] * 0.587 + bmp_image[i + _plt0.rgba_channel[2]] * 0.114);
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
                                        pixel = (ushort)(bmp_image[i + _plt0.rgba_channel[3]] << 8);  // _plt0.alpha value
                                        if (bmp_image[i + _plt0.rgba_channel[3]] != 0)
                                        {
                                            pixel += (ushort)((byte)(bmp_image[i + _plt0.rgba_channel[2]] * 0.0721 + bmp_image[i + _plt0.rgba_channel[1]] * 0.7154 + bmp_image[i + _plt0.rgba_channel[0]] * 0.2125));
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
                                        pixel = (ushort)((byte)(bmp_image[i + _plt0.rgba_channel[3]] * _plt0.custom_rgba[3]) << 8);  // _plt0.alpha value
                                        if (pixel != 0)
                                        {
                                            pixel += (ushort)(byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2] + bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1] + bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                        }
                                        Colours.Add(pixel);
                                        Colour_Table[pixel][0] += 1;
                                    }
                                    break;
                                }
                        }
                        if (!_plt0.user_palette || _plt0.fill_palette_start_offset != 0)  // no input palette / partial user input palette = fill it with these colours
                        {
                            Colour_Table.Sort(new IntArrayComparer());  // sorts the table by the most used colour first
                            if (!_plt0.stfu)
                                Console.WriteLine("findind most used Colours");
                            for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                            {
                                not_similar = true;
                                if (Colour_Table[i][0] / _plt0.pixel_count < _plt0.percentage / 100)
                                {
                                    break;
                                }
                                for (int k = 0; k < j; k += 2)
                                {
                                    if (Math.Abs(_plt0.colour_palette[k] - (byte)(Colour_Table[i][1] >> 8)) < _plt0.diversity && Math.Abs(_plt0.colour_palette[k + 1] - (byte)(Colour_Table[i][1])) < _plt0.diversity)
                                    {
                                        not_similar = false;
                                        break;
                                    }
                                }
                                if (not_similar)
                                {
                                    _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the _plt0.alpha value
                                    _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                                    j += 2;
                                }
                            }
                            if (j < _plt0.colour_number_x2) // if the colour palette is not full
                            {
                                if (!_plt0.stfu)
                                    Console.WriteLine("The colour palette was not full, starting second loop...\n");
                                for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                {
                                    not_similar = true;
                                    if (Colour_Table[i][0] / _plt0.pixel_count < _plt0.percentage2 / 100)
                                    {
                                        break;
                                    }
                                    for (int k = 0; k < j; k += 2)
                                    {
                                        if (Math.Abs(_plt0.colour_palette[k] - (byte)(Colour_Table[i][1] >> 8)) < _plt0.diversity2 && Math.Abs(_plt0.colour_palette[k + 1] - (byte)(Colour_Table[i][1])) < _plt0.diversity2)
                                        {
                                            not_similar = false;
                                            break;
                                        }
                                    }
                                    if (not_similar)
                                    {
                                        _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the _plt0.alpha value
                                        _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                                        j += 2;
                                    }
                                }
                                if (j < _plt0.colour_number_x2) // if the colour palette is still not full
                                {
                                    if (!_plt0.stfu)
                                        Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                    for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                    {
                                        not_similar = true;
                                        for (int k = 0; k < j; k += 2)
                                        {
                                            if ((_plt0.colour_palette[k] == (byte)(Colour_Table[i][1] >> 8)) && _plt0.colour_palette[k + 1] == (byte)(Colour_Table[i][1]))
                                            {
                                                not_similar = false;
                                                break;
                                            }
                                        }
                                        if (not_similar)
                                        {
                                            _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the _plt0.alpha value
                                            _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                                            j += 2;
                                        }
                                    }
                                }
                            }
                        }
                        if (!_plt0.stfu)
                            Console.WriteLine("creating indexes");
                        j = 0;
                        switch (_plt0.texture_format_int32[3])
                        {
                            case 8: // CI4
                                {
                                    for (int h = 0; h < _plt0.canvas_height; h++)
                                    {
                                        for (int w = 0; w < _plt0.canvas_width; w++)  // index_size = number of pixels
                                        {
                                            diff_min = 500;
                                            for (int i = 0; i < _plt0.colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                            {
                                                if (_plt0.colour_palette[i] == (byte)(Colours[j] >> 8) && _plt0.colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                {
                                                    diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                    break;
                                                }
                                                else
                                                {
                                                    diff = (short)(Math.Abs(_plt0.colour_palette[i] - (byte)(Colours[j] >> 8)) + Math.Abs(_plt0.colour_palette[i + 1] - (byte)Colours[j]));
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
                                        index_list.Add(index.ToArray());
                                    }
                                    break;
                                }
                            case 9: // CI8
                                {
                                    for (int h = 0; h < _plt0.canvas_height; h++)
                                    {
                                        for (int w = 0; w < _plt0.canvas_width; w++)  // index_size = number of pixels
                                        {
                                            diff_min = 500;
                                            for (int i = 0; i < _plt0.colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                            {
                                                if (_plt0.colour_palette[i] == (byte)(Colours[j] >> 8) && _plt0.colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                {
                                                    diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                    break;
                                                }
                                                else
                                                {
                                                    diff = (short)(Math.Abs(_plt0.colour_palette[i] - (byte)(Colours[j] >> 8)) + Math.Abs(_plt0.colour_palette[i + 1] - (byte)Colours[j]));
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
                                        index_list.Add(index.ToArray());
                                    }
                                    break;
                                }
                            case 10:  // CI14x2
                                {
                                    for (int h = 0; h < _plt0.canvas_height; h++)
                                    {
                                        for (int w = 0; w < _plt0.canvas_width << 1; w += 2)  // multiplied by two because each index is a 14 bytes integer
                                        {
                                            diff_min = 500;
                                            for (int i = 0; i < _plt0.colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                            {
                                                if (_plt0.colour_palette[i] == (byte)(Colours[j] >> 8) && _plt0.colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                {
                                                    diff_min_ind14x2 = (ushort)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                    break;
                                                }
                                                else
                                                {
                                                    diff = (short)(Math.Abs(_plt0.colour_palette[i] - (byte)(Colours[j] >> 8)) + Math.Abs(_plt0.colour_palette[i + 1] - (byte)Colours[j]));
                                                    if (diff < diff_min)
                                                    {
                                                        diff_min = diff;
                                                        diff_min_ind14x2 = (ushort)(i >> 1);
                                                    }
                                                }
                                            }
                                            index[w] = (byte)(diff_min_ind14x2 >> 8);  // adding a short at each iteration
                                            index[w + 1] = (byte)diff_min_ind14x2;  // casting to byte acts as a % 0xff
                                            j += 1;
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

                        switch (_plt0.algorithm)
                        {
                            case 2:  // custom  RRRR RGGG GGGB BBBB
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        red = (byte)(bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                        green = (byte)(bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);
                                        blue = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);
                                        if ((red & 7) > _plt0.round5 && red < 248)  // 5-bit max value on a trimmed byte
                                        {
                                            red += 8;
                                        }
                                        if ((green & _plt0.round6) == _plt0.round6 && green < 252)  // 6-bit max value on a trimmed byte
                                        {
                                            green += 4;
                                        }
                                        if ((blue & 7) > _plt0.round5 && blue < 248)
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
                                        red = bmp_image[i + _plt0.rgba_channel[0]];
                                        green = bmp_image[i + _plt0.rgba_channel[1]];
                                        blue = bmp_image[i + _plt0.rgba_channel[2]];
                                        if ((red & 7) > _plt0.round5 && red < 248)  // 5-bit max value on a trimmed byte
                                        {
                                            red += 8;
                                        }
                                        if ((green & _plt0.round6) == _plt0.round6 && green < 252)  // 6-bit max value on a trimmed byte
                                        {
                                            green += 4;
                                        }
                                        if ((blue & 7) > _plt0.round5 && blue < 248)
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
                        if (!_plt0.user_palette || _plt0.fill_palette_start_offset != 0)  // no input palette / partial user input palette = fill it with these colours
                        {
                            Colour_Table.Sort(new IntArrayComparer());  // sorts the table by the most used colour first
                            if (!_plt0.stfu)
                                Console.WriteLine("findind most used Colours");
                            for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                            {
                                not_similar = true;
                                if (Colour_Table[i][0] / _plt0.pixel_count < _plt0.percentage / 100)
                                {
                                    break;
                                }
                                for (int k = 0; k < j; k += 2)
                                {
                                    if (Math.Abs((_plt0.colour_palette[k] & 248) - ((Colour_Table[i][1] >> 8) & 248)) < _plt0.diversity && Math.Abs(((_plt0.colour_palette[k] & 7) << 5) + ((_plt0.colour_palette[k + 1] >> 3) & 28) - ((Colour_Table[i][1] >> 3) & 252)) < _plt0.diversity && Math.Abs(((_plt0.colour_palette[k + 1] << 3) & 248) - (Colour_Table[i][1] << 3) & 248) < _plt0.diversity)
                                    {
                                        not_similar = false;
                                        break;
                                    }
                                }
                                if (not_similar)
                                {
                                    _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the RRRR RGGG value
                                    _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the GGGB BBBB value
                                    j += 2;
                                }
                            }
                            if (j < _plt0.colour_number_x2) // if the colour palette is not full
                            {
                                if (!_plt0.stfu)
                                    Console.WriteLine("The colour palette was not full, starting second loop...\n");
                                for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                {
                                    not_similar = true;
                                    if (Colour_Table[i][0] / _plt0.pixel_count < _plt0.percentage2 / 100)
                                    {
                                        break;
                                    }
                                    for (int k = 0; k < j; k += 2)
                                    {
                                        if (Math.Abs((_plt0.colour_palette[k] & 248) - ((Colour_Table[i][1] >> 8) & 248)) < _plt0.diversity2 && Math.Abs(((_plt0.colour_palette[k] & 7) << 5) + ((_plt0.colour_palette[k + 1] >> 3) & 28) - ((Colour_Table[i][1] >> 3) & 252)) < _plt0.diversity2 && Math.Abs(((_plt0.colour_palette[k + 1] << 3) & 248) - (Colour_Table[i][1] << 3) & 248) < _plt0.diversity2)
                                        {
                                            not_similar = false;
                                            break;
                                        }
                                    }
                                    if (not_similar)
                                    {
                                        _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the Red and green value
                                        _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Green and blue value
                                        j += 2;
                                    }
                                }
                                if (j < _plt0.colour_number_x2) // if the colour palette is still not full
                                {
                                    if (!_plt0.stfu)
                                        Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                    for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                    {
                                        not_similar = true;
                                        for (int k = 0; k < j; k += 2)
                                        {
                                            if ((_plt0.colour_palette[k] == (byte)(Colour_Table[i][1] >> 8)) && _plt0.colour_palette[k + 1] == (byte)(Colour_Table[i][1]))
                                            {
                                                not_similar = false;
                                                break;
                                            }
                                        }
                                        if (not_similar)
                                        {
                                            _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the RRRR RGGG value
                                            _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the GGGB BBBB value
                                            j += 2;
                                        }
                                    }
                                }
                            }
                        }
                        if (!_plt0.stfu)
                            Console.WriteLine("creating indexes");
                        j = 0;
                        switch (_plt0.texture_format_int32[3])
                        {
                            case 8: // CI4
                                {
                                    for (int h = 0; h < _plt0.canvas_height; h++)
                                    {
                                        for (int w = 0; w < _plt0.canvas_width; w++)  // index_size = number of pixels
                                        {
                                            diff_min = 500;
                                            for (int i = 0; i < _plt0.colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                            {
                                                if (_plt0.colour_palette[i] == (byte)(Colours[j] >> 8) && _plt0.colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                {
                                                    diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                    break;
                                                }
                                                else  // calculate difference between each separate colour channel and store the sum
                                                {
                                                    diff = (short)(Math.Abs((_plt0.colour_palette[i] & 248) - ((Colours[j] >> 8) & 248)) + Math.Abs(((_plt0.colour_palette[i] & 7) << 5) + ((_plt0.colour_palette[i + 1] >> 3) & 28) - ((Colours[j] >> 3) & 252)) + Math.Abs(((_plt0.colour_palette[i + 1] << 3) & 248) - ((Colours[j] << 3) & 248)));
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
                                        index_list.Add(index.ToArray());
                                    }
                                    break;
                                }
                            case 9: // CI8
                                {
                                    for (int h = 0; h < _plt0.canvas_height; h++)
                                    {
                                        for (int w = 0; w < _plt0.canvas_width; w++)  // index_size = number of pixels
                                        {
                                            diff_min = 500;
                                            for (int i = 0; i < _plt0.colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                            {
                                                if (_plt0.colour_palette[i] == (byte)(Colours[j] >> 8) && _plt0.colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                {
                                                    diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                    break;
                                                }
                                                else  // calculate difference between each separate colour channel and store the sum
                                                {

                                                    diff = (short)(Math.Abs((_plt0.colour_palette[i] & 248) - ((Colours[j] >> 8) & 248)) + Math.Abs(((_plt0.colour_palette[i] & 7) << 5) + ((_plt0.colour_palette[i + 1] >> 3) & 28) - ((Colours[j] >> 3) & 252)) + Math.Abs(((_plt0.colour_palette[i + 1] << 3) & 248) - ((Colours[j] << 3) & 248))); if (diff < diff_min)
                                                    {
                                                        diff_min = diff;
                                                        diff_min_index = (byte)(i >> 1);
                                                    }
                                                }
                                            }
                                            index[w] = diff_min_index;
                                            j += 1;
                                        }
                                        index_list.Add(index.ToArray());
                                    }
                                    break;
                                }
                            case 10:  // CI14x2
                                {
                                    for (int h = 0; h < _plt0.canvas_height; h++)
                                    {
                                        for (int w = 0; w < _plt0.canvas_width << 1; w += 2)  // multiplied by two because each index is a 14 bytes integer
                                        {
                                            diff_min = 500;
                                            for (int i = 0; i < _plt0.colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                            {
                                                if (_plt0.colour_palette[i] == (byte)(Colours[j] >> 8) && _plt0.colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                {
                                                    diff_min_ind14x2 = (ushort)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                    break;
                                                }
                                                else  // calculate difference between each separate colour channel and store the sum
                                                {

                                                    diff = (short)(Math.Abs((_plt0.colour_palette[i] & 248) - ((Colours[j] >> 8) & 248)) + Math.Abs(((_plt0.colour_palette[i] & 7) << 5) + ((_plt0.colour_palette[i + 1] >> 3) & 28) - ((Colours[j] >> 3) & 252)) + Math.Abs(((_plt0.colour_palette[i + 1] << 3) & 248) - ((Colours[j] << 3) & 248)));
                                                    if (diff < diff_min)
                                                    {
                                                        diff_min = diff;
                                                        diff_min_ind14x2 = (ushort)(i >> 1);
                                                    }
                                                }
                                            }
                                            index[w] = (byte)(diff_min_ind14x2 >> 8);  // adding a short at each iteration
                                            index[w + 1] = (byte)diff_min_ind14x2;  // casting to byte acts as a % 0xff
                                            j += 1;
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
                        switch (_plt0.algorithm)
                        {
                            case 2:  // custom
                                {
                                    if (_plt0.alpha == 1)  // 0AAA RRRR GGGG BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            a = (byte)(bmp_image[i + _plt0.rgba_channel[3]] * _plt0.custom_rgba[3]);
                                            red = (byte)(bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                            green = (byte)(bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);
                                            blue = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);
                                            if ((a & 31) > _plt0.round3 && a < 224)  // 3-bit max value on a trimmed byte
                                            {
                                                a += 32;
                                            }
                                            if ((red & 15) > _plt0.round4 && red < 240)  // 4-bit max value on a trimmed byte
                                            {
                                                red += 16;
                                            }
                                            if ((green & 15) > _plt0.round4 && green < 240)
                                            {
                                                green += 16;
                                            }
                                            if ((blue & 15) > _plt0.round4 && blue < 240)
                                            {
                                                blue += 16;
                                            }
                                            pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                            Colours.Add(pixel);
                                            Colour_Table[pixel][0] += 1;
                                        }
                                    }
                                    else if (_plt0.alpha == 0)  // 1RRR RRGG GGGB BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            red = (byte)(bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                            green = (byte)(bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);
                                            blue = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);
                                            if ((red & 7) > _plt0.round5 && red < 248)  // 5-bit max value on a trimmed byte
                                            {
                                                red += 8;
                                            }
                                            if ((green & 7) > _plt0.round5 && green < 248)
                                            {
                                                green += 8;
                                            }
                                            if ((blue & 7) > _plt0.round5 && blue < 248)
                                            {
                                                blue += 8;
                                            }
                                            pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                            Colours.Add(pixel);
                                            Colour_Table[pixel][0] += 1;
                                        }
                                    }
                                    else  // check for each colour if _plt0.alpha trimmed to 3 bits is 255
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            a = (byte)(bmp_image[i + _plt0.rgba_channel[3]] * _plt0.custom_rgba[3]);
                                            red = (byte)(bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                            green = (byte)(bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);
                                            blue = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);
                                            if ((a & 31) > _plt0.round3 && a < 224)  // 3-bit max value on a trimmed byte
                                            {
                                                a += 32;
                                            }
                                            if ((red & 15) > _plt0.round4 && red < 240)  // 4-bit max value on a trimmed byte
                                            {
                                                red += 16;
                                            }
                                            if ((green & 15) > _plt0.round4 && green < 240)
                                            {
                                                green += 16;
                                            }
                                            if ((blue & 15) > _plt0.round4 && blue < 240)
                                            {
                                                blue += 16;
                                            }
                                            if (a > 223)  // no _plt0.alpha
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
                                    if (_plt0.alpha == 1)  // 0AAA RRRR GGGG BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            a = bmp_image[i + _plt0.rgba_channel[3]];
                                            red = bmp_image[i + _plt0.rgba_channel[0]];
                                            green = bmp_image[i + _plt0.rgba_channel[1]];
                                            blue = bmp_image[i + _plt0.rgba_channel[2]];
                                            if ((a & 31) > _plt0.round3 && a < 224)  // 3-bit max value on a trimmed byte
                                            {
                                                a += 32;
                                            }
                                            if ((red & 15) > _plt0.round4 && red < 240)  // 4-bit max value on a trimmed byte
                                            {
                                                red += 16;
                                            }
                                            if ((green & 15) > _plt0.round4 && green < 240)
                                            {
                                                green += 16;
                                            }
                                            if ((blue & 15) > _plt0.round4 && blue < 240)
                                            {
                                                blue += 16;
                                            }
                                            pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                            Colours.Add(pixel);
                                            Colour_Table[pixel][0] += 1;
                                        }
                                    }
                                    else if (_plt0.alpha == 0)  // 1RRR RRGG GGGB BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            red = bmp_image[i + _plt0.rgba_channel[0]];
                                            green = bmp_image[i + _plt0.rgba_channel[1]];
                                            blue = bmp_image[i + _plt0.rgba_channel[2]];
                                            if ((red & 7) > _plt0.round5 && red < 248)  // 5-bit max value on a trimmed byte
                                            {
                                                red += 8;
                                            }
                                            if ((green & 7) > _plt0.round5 && green < 248)
                                            {
                                                green += 8;
                                            }
                                            if ((blue & 7) > _plt0.round5 && blue < 248)
                                            {
                                                blue += 8;
                                            }
                                            pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                            Colours.Add(pixel);
                                            Colour_Table[pixel][0] += 1;
                                        }
                                    }
                                    else  // mix up _plt0.alpha and no _plt0.alpha
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            a = bmp_image[i + _plt0.rgba_channel[3]];
                                            red = bmp_image[i + _plt0.rgba_channel[0]];
                                            green = bmp_image[i + _plt0.rgba_channel[1]];
                                            blue = bmp_image[i + _plt0.rgba_channel[2]];
                                            if ((a & 31) > _plt0.round3 && a < 224)  // 3-bit max value on a trimmed byte
                                            {
                                                a += 32;
                                            }
                                            if ((red & 15) > _plt0.round4 && red < 240)  // 4-bit max value on a trimmed byte
                                            {
                                                red += 16;
                                            }
                                            if ((green & 15) > _plt0.round4 && green < 240)
                                            {
                                                green += 16;
                                            }
                                            if ((blue & 15) > _plt0.round4 && blue < 240)
                                            {
                                                blue += 16;
                                            }
                                            if (a > 223)  // no _plt0.alpha
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
                        if (!_plt0.user_palette || _plt0.fill_palette_start_offset != 0)  // no input palette / partial user input palette = fill it with these colours
                        {

                            Colour_Table.Sort(new IntArrayComparer());  // sorts the table by the most used colour first
                            if (!_plt0.stfu)
                                Console.WriteLine("findind most used Colours");
                            if (_plt0.alpha == 1)
                            {
                                for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                {
                                    not_similar = true;
                                    if (Colour_Table[i][0] / _plt0.pixel_count < _plt0.percentage / 100)
                                    {
                                        break;
                                    }
                                    for (int k = 0; k < j; k += 2)
                                    {
                                        if (Math.Abs((_plt0.colour_palette[k] & 112) - (Colour_Table[i][1] >> 8) & 112) < _plt0.diversity && Math.Abs(((_plt0.colour_palette[k] << 4) & 240) - ((Colour_Table[i][1] >> 4) & 240)) < _plt0.diversity && Math.Abs((_plt0.colour_palette[k + 1] & 240) - ((Colour_Table[i][1]) & 240)) < _plt0.diversity && Math.Abs(((_plt0.colour_palette[k + 1] << 4) & 240) - ((Colour_Table[i][1] << 4) & 240)) < _plt0.diversity)
                                        {
                                            not_similar = false;
                                            break;
                                        }
                                    }
                                    if (not_similar)
                                    {
                                        _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the 0AAA RRRR value
                                        _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the GGGG BBBB value
                                        j += 2;
                                    }
                                }
                                if (j < _plt0.colour_number_x2) // if the colour palette is not full
                                {
                                    if (!_plt0.stfu)
                                        Console.WriteLine("The colour palette was not full, starting second loop...\n");
                                    for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                    {
                                        not_similar = true;
                                        if (Colour_Table[i][0] / _plt0.pixel_count < _plt0.percentage2 / 100)
                                        {
                                            break;
                                        }
                                        for (int k = 0; k < j; k += 2)
                                        {
                                            if (Math.Abs((_plt0.colour_palette[k] & 112) - (Colour_Table[i][1] >> 8) & 112) < _plt0.diversity2 && Math.Abs(((_plt0.colour_palette[k] << 4) & 240) - ((Colour_Table[i][1] >> 4) & 240)) < _plt0.diversity2 && Math.Abs((_plt0.colour_palette[k + 1] & 240) - ((Colour_Table[i][1]) & 240)) < _plt0.diversity2 && Math.Abs(((_plt0.colour_palette[k + 1] << 4) & 240) - ((Colour_Table[i][1] << 4) & 240)) < _plt0.diversity2)
                                            {
                                                not_similar = false;
                                                break;
                                            }
                                        }
                                        if (not_similar)
                                        {
                                            _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the 0AAA RRRR value
                                            _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the GGGG BBBB value
                                            j += 2;
                                        }
                                    }
                                    if (j < _plt0.colour_number_x2) // if the colour palette is still not full
                                    {
                                        if (!_plt0.stfu)
                                            Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                        for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                        {
                                            not_similar = true;
                                            for (int k = 0; k < j; k += 2)
                                            {
                                                if ((_plt0.colour_palette[k] == (byte)(Colour_Table[i][1] >> 8)) && _plt0.colour_palette[k + 1] == (byte)(Colour_Table[i][1]))
                                                {
                                                    not_similar = false;
                                                    break;
                                                }
                                            }
                                            if (not_similar)
                                            {
                                                _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the 0AAA RRRR value
                                                _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the GGGG BBBB value
                                                j += 2;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (_plt0.alpha == 0)  // 1RRR RRGG GGGB BBBB
                            {
                                for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                {
                                    not_similar = true;
                                    if (Colour_Table[i][0] / _plt0.pixel_count < _plt0.percentage / 100)
                                    {
                                        break;
                                    }
                                    for (int k = 0; k < j; k += 2)
                                    {
                                        if (Math.Abs(((_plt0.colour_palette[k] << 1) & 248) - ((Colour_Table[i][1] >> 7) & 248)) < _plt0.diversity && Math.Abs(((_plt0.colour_palette[k] & 3) << 6) + ((_plt0.colour_palette[k + 1] >> 2) & 56) - ((Colour_Table[i][1] >> 2) & 248)) < _plt0.diversity && Math.Abs(((_plt0.colour_palette[k + 1] << 3) & 248) - (Colour_Table[i][1] << 3) & 248) < _plt0.diversity)
                                        {
                                            not_similar = false;
                                            break;
                                        }
                                    }
                                    if (not_similar)
                                    {
                                        _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the 1RRR RRGG value
                                        _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the GGGB BBBB value
                                        j += 2;
                                    }
                                }
                                if (j < _plt0.colour_number_x2) // if the colour palette is not full
                                {
                                    if (!_plt0.stfu)
                                        Console.WriteLine("The colour palette was not full, starting second loop...\n");
                                    for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                    {
                                        not_similar = true;
                                        if (Colour_Table[i][0] / _plt0.pixel_count < _plt0.percentage2 / 100)
                                        {
                                            break;
                                        }
                                        for (int k = 0; k < j; k += 2)
                                        {
                                            if (Math.Abs(((_plt0.colour_palette[k] << 1) & 248) - ((Colour_Table[i][1] >> 7) & 248)) < _plt0.diversity2 && Math.Abs(((_plt0.colour_palette[k] & 3) << 6) + ((_plt0.colour_palette[k + 1] >> 2) & 56) - ((Colour_Table[i][1] >> 2) & 248)) < _plt0.diversity2 && Math.Abs(((_plt0.colour_palette[k + 1] << 3) & 248) - (Colour_Table[i][1] << 3) & 248) < _plt0.diversity2)
                                            {
                                                not_similar = false;
                                                break;
                                            }
                                        }
                                        if (not_similar)
                                        {
                                            _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the _plt0.alpha and red value
                                            _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Gren and blue value
                                            j += 2;
                                        }
                                    }
                                    if (j < _plt0.colour_number_x2) // if the colour palette is still not full
                                    {
                                        if (!_plt0.stfu)
                                            Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                        for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                        {
                                            not_similar = true;
                                            for (int k = 0; k < j; k += 2)
                                            {
                                                if ((_plt0.colour_palette[k] == (byte)(Colour_Table[i][1] >> 8)) && _plt0.colour_palette[k + 1] == (byte)(Colour_Table[i][1]))
                                                {
                                                    not_similar = false;
                                                    break;
                                                }
                                            }
                                            if (not_similar)
                                            {
                                                _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the _plt0.alpha and red value
                                                _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Green and blue value
                                                j += 2;
                                            }
                                        }
                                    }
                                }
                            }
                            else  // mix
                            {

                                for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                {
                                    not_similar = true;
                                    if (Colour_Table[i][0] / _plt0.pixel_count < _plt0.percentage / 100)
                                    {
                                        break;
                                    }
                                    for (int k = 0; k < j; k += 2)
                                    {
                                        if ((_plt0.colour_palette[k] >> 7) == 1)  // no _plt0.alpha - 1RRR RRGG GGGB BBBB
                                        {
                                            a = 255;
                                            red = (byte)((_plt0.colour_palette[k] << 1) & 248);
                                            green = (byte)(((_plt0.colour_palette[k] & 3) << 6) + ((_plt0.colour_palette[k + 1] >> 2) & 56));
                                            blue = (byte)((_plt0.colour_palette[k + 1] << 3) & 248);
                                        }
                                        else  // _plt0.alpha - 0AAA RRRR GGGG BBBB
                                        {
                                            a = (byte)(_plt0.colour_palette[k] & 112);
                                            red = (byte)((_plt0.colour_palette[k] << 4) & 240);
                                            green = (byte)(_plt0.colour_palette[k + 1] & 240);
                                            blue = (byte)((_plt0.colour_palette[k + 1] << 4) & 240);
                                        }
                                        if (Colour_Table[i][1] >> 15 == 1)  // no _plt0.alpha - 1RRR RRGG GGGB BBBB
                                        {
                                            a2 = 255;
                                            red2 = (byte)((Colour_Table[i][1] >> 7) & 248);
                                            green2 = (byte)((Colour_Table[i][1] >> 2) & 248);
                                            blue2 = (byte)((Colour_Table[i][1] << 3) & 248);
                                        }
                                        else  // _plt0.alpha - 0AAA RRRR GGGG BBBB
                                        {
                                            a2 = (byte)((Colour_Table[i][1] >> 8) & 112);
                                            red2 = (byte)((Colour_Table[i][1] >> 4) & 240);
                                            green2 = (byte)((Colour_Table[i][1]) & 240);
                                            blue2 = (byte)((Colour_Table[i][1] << 4) & 240);
                                        }
                                        if (Math.Abs(a - a2) < _plt0.diversity && Math.Abs(red - red2) < _plt0.diversity && Math.Abs(green - green2) < _plt0.diversity && Math.Abs(blue - blue2) < _plt0.diversity)
                                        {
                                            not_similar = false;
                                            break;
                                        }
                                    }
                                    if (not_similar)
                                    {
                                        _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the _plt0.alpha and red value
                                        _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Green and blue value
                                        j += 2;
                                    }
                                }
                                if (j < _plt0.colour_number_x2) // if the colour palette is not full
                                {
                                    if (!_plt0.stfu)
                                        Console.WriteLine("The colour palette was not full, starting second loop...\n");
                                    for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                    {
                                        not_similar = true;
                                        if (Colour_Table[i][0] / _plt0.pixel_count < _plt0.percentage2 / 100)
                                        {
                                            break;
                                        }
                                        for (int k = 0; k < j; k += 2)
                                        {
                                            if ((_plt0.colour_palette[k] >> 7) == 1)  // no _plt0.alpha - 1RRR RRGG GGGB BBBB
                                            {
                                                a = 255;
                                                red = (byte)((_plt0.colour_palette[k] << 1) & 248);
                                                green = (byte)(((_plt0.colour_palette[k] & 3) << 6) + ((_plt0.colour_palette[k + 1] >> 2) & 56));
                                                blue = (byte)((_plt0.colour_palette[k + 1] << 3) & 248);
                                            }
                                            else  // _plt0.alpha - 0AAA RRRR GGGG BBBB
                                            {
                                                a = (byte)(_plt0.colour_palette[k] & 112);
                                                red = (byte)((_plt0.colour_palette[k] << 4) & 240);
                                                green = (byte)(_plt0.colour_palette[k + 1] & 240);
                                                blue = (byte)((_plt0.colour_palette[k + 1] << 4) & 240);
                                            }
                                            if (Colour_Table[i][1] >> 15 == 1)  // no _plt0.alpha - 1RRR RRGG GGGB BBBB
                                            {
                                                a2 = 255;
                                                red2 = (byte)((Colour_Table[i][1] >> 7) & 248);
                                                green2 = (byte)((Colour_Table[i][1] >> 2) & 248);
                                                blue2 = (byte)((Colour_Table[i][1] << 3) & 248);
                                            }
                                            else  // _plt0.alpha - 0AAA RRRR GGGG BBBB
                                            {
                                                a2 = (byte)((Colour_Table[i][1] >> 8) & 112);
                                                red2 = (byte)((Colour_Table[i][1] >> 4) & 240);
                                                green2 = (byte)((Colour_Table[i][1]) & 240);
                                                blue2 = (byte)((Colour_Table[i][1] << 4) & 240);
                                            }
                                            if (Math.Abs(a - a2) < _plt0.diversity2 && Math.Abs(red - red2) < _plt0.diversity2 && Math.Abs(green - green2) < _plt0.diversity2 && Math.Abs(blue - blue2) < _plt0.diversity2)
                                            {
                                                not_similar = false;
                                                break;
                                            }
                                        }
                                        if (not_similar)
                                        {
                                            _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the _plt0.alpha value
                                            _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                                            j += 2;
                                        }
                                    }
                                    if (j < _plt0.colour_number_x2) // if the colour palette is still not full
                                    {
                                        if (!_plt0.stfu)
                                            Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                        for (int i = 0; i < Colour_Table.Count && j < _plt0.colour_number_x2; i++)
                                        {
                                            not_similar = true;
                                            for (int k = 0; k < j; k += 2)
                                            {
                                                if ((_plt0.colour_palette[k] == (byte)(Colour_Table[i][1] >> 8)) && _plt0.colour_palette[k + 1] == (byte)(Colour_Table[i][1]))
                                                {
                                                    not_similar = false;
                                                    break;
                                                }
                                            }
                                            if (not_similar)
                                            {
                                                _plt0.colour_palette[j] = (byte)(Colour_Table[i][1] >> 8);  // adds the _plt0.alpha value
                                                _plt0.colour_palette[j + 1] = (byte)(Colour_Table[i][1]);  // adds the Grey value
                                                j += 2;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!_plt0.stfu)
                            Console.WriteLine("creating indexes");
                        j = 0;
                        switch (_plt0.texture_format_int32[3])
                        {
                            case 8: // CI4
                                {
                                    for (int h = 0; h < _plt0.canvas_height; h++)
                                    {
                                        for (int w = 0; w < _plt0.canvas_width; w++)  // index_size = number of pixels
                                        {
                                            diff_min = 500;
                                            for (int i = 0; i < _plt0.colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                            {
                                                if (_plt0.colour_palette[i] == (byte)(Colours[j] >> 8) && _plt0.colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                {
                                                    diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                    break;
                                                }
                                                else  // calculate difference between each separate colour channel and store the sum
                                                {
                                                    if ((_plt0.colour_palette[i] >> 7) == 1)  // no _plt0.alpha - 1RRR RRGG GGGB BBBB
                                                    {
                                                        a = 255;
                                                        red = (byte)((_plt0.colour_palette[i] << 1) & 248);
                                                        green = (byte)(((_plt0.colour_palette[i] & 3) << 6) + ((_plt0.colour_palette[i + 1] >> 2) & 56));
                                                        blue = (byte)((_plt0.colour_palette[i + 1] << 3) & 248);
                                                    }
                                                    else  // _plt0.alpha - 0AAA RRRR GGGG BBBB
                                                    {
                                                        a = (byte)(_plt0.colour_palette[i] & 112);
                                                        red = (byte)((_plt0.colour_palette[i] << 4) & 240);
                                                        green = (byte)(_plt0.colour_palette[i + 1] & 240);
                                                        blue = (byte)((_plt0.colour_palette[i + 1] << 4) & 240);
                                                    }
                                                    if (Colours[j] >> 15 == 1)  // no _plt0.alpha - 1RRR RRGG GGGB BBBB
                                                    {
                                                        a2 = 255;
                                                        red2 = (byte)((Colours[j] >> 7) & 248);
                                                        green2 = (byte)((Colours[j] >> 2) & 248);
                                                        blue2 = (byte)((Colours[j] << 3) & 248);
                                                    }
                                                    else  // _plt0.alpha - 0AAA RRRR GGGG BBBB
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
                                        index_list.Add(index.ToArray());
                                    }
                                    break;
                                }
                            case 9: // CI8
                                {
                                    for (int h = 0; h < _plt0.canvas_height; h++)
                                    {
                                        for (int w = 0; w < _plt0.canvas_width; w++)  // index_size = number of pixels
                                        {
                                            diff_min = 500;
                                            for (int i = 0; i < _plt0.colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                            {
                                                if (_plt0.colour_palette[i] == (byte)(Colours[j] >> 8) && _plt0.colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                {
                                                    diff_min_index = (byte)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                    break;
                                                }
                                                else  // calculate difference between each separate colour channel and store the sum
                                                {
                                                    if ((_plt0.colour_palette[i] >> 7) == 1)  // no _plt0.alpha - 1RRR RRGG GGGB BBBB
                                                    {
                                                        a = 255;
                                                        red = (byte)((_plt0.colour_palette[i] << 1) & 248);
                                                        green = (byte)(((_plt0.colour_palette[i] & 3) << 6) + ((_plt0.colour_palette[i + 1] >> 2) & 56));
                                                        blue = (byte)((_plt0.colour_palette[i + 1] << 3) & 248);
                                                    }
                                                    else  // _plt0.alpha - 0AAA RRRR GGGG BBBB
                                                    {
                                                        a = (byte)(_plt0.colour_palette[i] & 112);
                                                        red = (byte)((_plt0.colour_palette[i] << 4) & 240);
                                                        green = (byte)(_plt0.colour_palette[i + 1] & 240);
                                                        blue = (byte)((_plt0.colour_palette[i + 1] << 4) & 240);
                                                    }
                                                    if (Colours[j] >> 15 == 1)  // no _plt0.alpha - 1RRR RRGG GGGB BBBB
                                                    {
                                                        a2 = 255;
                                                        red2 = (byte)((Colours[j] >> 7) & 248);
                                                        green2 = (byte)((Colours[j] >> 2) & 248);
                                                        blue2 = (byte)((Colours[j] << 3) & 248);
                                                    }
                                                    else  // _plt0.alpha - 0AAA RRRR GGGG BBBB
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
                                        index_list.Add(index.ToArray());
                                    }
                                    break;
                                }
                            case 10:  // CI14x2
                                {
                                    for (int h = 0; h < _plt0.canvas_height; h++)
                                    {
                                        for (int w = 0; w < _plt0.canvas_width << 1; w += 2)  // multiplied by two because each index is a 14 bits integer
                                        {
                                            diff_min = 500;
                                            for (int i = 0; i < _plt0.colour_number_x2; i += 2)  // process the colour palette to find the closest colour corresponding to the current pixel
                                            {
                                                if (_plt0.colour_palette[i] == (byte)(Colours[j] >> 8) && _plt0.colour_palette[i + 1] == (byte)Colours[j])  // if it's the exact same colour
                                                {
                                                    diff_min_ind14x2 = (ushort)(i >> 1);  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                    break;
                                                }
                                                else  // calculate difference between each separate colour channel and store the sum
                                                {
                                                    if ((_plt0.colour_palette[i] >> 7) == 1)  // no _plt0.alpha - 1RRR RRGG GGGB BBBB
                                                    {
                                                        a = 255;
                                                        red = (byte)((_plt0.colour_palette[i] << 1) & 248);
                                                        green = (byte)(((_plt0.colour_palette[i] & 3) << 6) + ((_plt0.colour_palette[i + 1] >> 2) & 56));
                                                        blue = (byte)((_plt0.colour_palette[i + 1] << 3) & 248);
                                                    }
                                                    else  // _plt0.alpha - 0AAA RRRR GGGG BBBB
                                                    {
                                                        a = (byte)(_plt0.colour_palette[i] & 112);
                                                        red = (byte)((_plt0.colour_palette[i] << 4) & 240);
                                                        green = (byte)(_plt0.colour_palette[i + 1] & 240);
                                                        blue = (byte)((_plt0.colour_palette[i + 1] << 4) & 240);
                                                    }
                                                    if (Colours[j] >> 15 == 1)  // no _plt0.alpha - 1RRR RRGG GGGB BBBB
                                                    {
                                                        a2 = 255;
                                                        red2 = (byte)((Colours[j] >> 7) & 248);
                                                        green2 = (byte)((Colours[j] >> 2) & 248);
                                                        blue2 = (byte)((Colours[j] << 3) & 248);
                                                    }
                                                    else  // _plt0.alpha - 0AAA RRRR GGGG BBBB
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
                                                        diff_min_ind14x2 = (ushort)(i >> 1);
                                                    }
                                                }
                                            }
                                            index[w] = (byte)(diff_min_ind14x2 >> 8);  // adding a short at each iteration
                                            index[w + 1] = (byte)diff_min_ind14x2;  // casting to byte acts as a % 0xff
                                            j += 1;
                                        }
                                        index_list.Add(index.ToArray());
                                    }
                                    break;
                                }  // end of case 10 CI14x2
                        } // end of switch texture format
                        break;
                    }  // end of case 2 palette RGB5A3
            }  // end of switch palette format
        }  // end of if (has palette)
        else
        {
            j = 0;
            switch (_plt0.texture_format_int32[3])
            {
                case 0: // I4
                    {
                        switch (_plt0.algorithm)
                        {
                            default: // cie_601
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 8)  // process every pixel by groups of two to fit the AAAA BBBB  profile
                                    {
                                        a = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * 0.114 + bmp_image[i + _plt0.rgba_channel[1]] * 0.587 + bmp_image[i + _plt0.rgba_channel[0]] * 0.299);  // grey colour trimmed to 4 bit
                                        if ((a & 0xf) > _plt0.round4 && a < 240)
                                        {
                                            a += 16;
                                        }
                                        grey = (byte)(bmp_image[i + 4 + _plt0.rgba_channel[2]] * 0.114 + bmp_image[i + 4 + _plt0.rgba_channel[1]] * 0.587 + bmp_image[i + 4 + _plt0.rgba_channel[0]] * 0.299);
                                        if ((grey & 0xf) > _plt0.round4 && grey < 240)
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
                                        a = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * 0.0721 + bmp_image[i + _plt0.rgba_channel[1]] * 0.7154 + bmp_image[i + _plt0.rgba_channel[0]] * 0.2125);
                                        if ((a & 0xf) > _plt0.round4 && a < 240)
                                        {
                                            a += 16;
                                        }
                                        grey = (byte)(bmp_image[i + 4 + _plt0.rgba_channel[2]] * 0.0721 + bmp_image[i + 4 + _plt0.rgba_channel[1]] * 0.7154 + bmp_image[i + 4 + _plt0.rgba_channel[0]] * 0.2125);
                                        if ((grey & 0xf) > _plt0.round4 && grey < 240)
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
                                        a = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2] + bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1] + bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                        if ((a & 0xf) > _plt0.round4 && a < 240)
                                        {
                                            a += 16;
                                        }
                                        grey = (byte)(bmp_image[i + 4 + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2] + bmp_image[i + 4 + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1] + bmp_image[i + 4 + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                        if ((grey & 0xf) > _plt0.round4 && grey < 240)
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
                        switch (_plt0.algorithm)
                        {
                            default: // cie_601
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)  // process every pixel to fit the CCCC CCCC profile
                                    {
                                        index[j] = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * 0.114 + bmp_image[i + _plt0.rgba_channel[1]] * 0.587 + bmp_image[i + _plt0.rgba_channel[0]] * 0.299);
                                        j++;
                                        if (j == _plt0.canvas_width)
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
                                        index[j] = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * 0.0721 + bmp_image[i + _plt0.rgba_channel[1]] * 0.7154 + bmp_image[i + _plt0.rgba_channel[0]] * 0.2125);
                                        j++;
                                        if (j == _plt0.canvas_width)
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
                                        index[j] = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2] + bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1] + bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                        j++;
                                        if (j == _plt0.canvas_width)
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
                        switch (_plt0.algorithm)
                        {
                            default: // cie_601
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)  // process every pixel to fit the AAAA CCCC profile
                                    {
                                        a = (bmp_image[i + _plt0.rgba_channel[3]]);  // _plt0.alpha value
                                        if ((a & 0xf) > _plt0.round4 && a < 240)
                                        {
                                            a += 16;
                                        }
                                        grey = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * 0.114 + bmp_image[i + _plt0.rgba_channel[1]] * 0.587 + bmp_image[i + _plt0.rgba_channel[0]] * 0.299);
                                        if ((grey & 0xf) > _plt0.round4 && grey < 240)
                                        {
                                            grey += 16;
                                        }
                                        index[j] = (byte)((a & 0xf0) + (grey >> 4));
                                        j++;
                                        if (j == _plt0.canvas_width)
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
                                        a = (bmp_image[i + _plt0.rgba_channel[3]]);  // _plt0.alpha value
                                        if ((a & 0xf) > _plt0.round4 && a < 240)
                                        {
                                            a += 16;
                                        }
                                        grey = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * 0.0721 + bmp_image[i + _plt0.rgba_channel[1]] * 0.7154 + bmp_image[i + _plt0.rgba_channel[0]] * 0.2125);
                                        if ((grey & 0xf) > _plt0.round4 && grey < 240)
                                        {
                                            grey += 16;
                                        }
                                        index[j] = (byte)((a & 0xf0) + (grey >> 4));
                                        j++;
                                        if (j == _plt0.canvas_width)
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
                                        a = (byte)(bmp_image[i + _plt0.rgba_channel[3]] * _plt0.custom_rgba[3]);  // _plt0.alpha value
                                        if ((a & 0xf) > _plt0.round4 && a < 240)
                                        {
                                            a += 16;
                                        }
                                        grey = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2] + bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1] + bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                        if ((grey & 0xf) > _plt0.round4 && grey < 240)
                                        {
                                            grey += 16;
                                        }
                                        index[j] = (byte)((a & 0xf0) + (grey >> 4));
                                        j++;
                                        if (j == _plt0.canvas_width)
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

                        switch (_plt0.algorithm)
                        {
                            default: // cie_601
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
                                    {
                                        index[j] = bmp_image[i + _plt0.rgba_channel[3]];  // _plt0.alpha value
                                        index[j + 1] = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * 0.114 + bmp_image[i + _plt0.rgba_channel[1]] * 0.587 + bmp_image[i + _plt0.rgba_channel[0]] * 0.299);  // Grey Value
                                        j += 2;
                                        if (j == _plt0.canvas_width << 1)
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
                                        index[j] = bmp_image[i + _plt0.rgba_channel[3]];  // _plt0.alpha value
                                        index[j + 1] = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * 0.0721 + bmp_image[i + _plt0.rgba_channel[1]] * 0.7154 + bmp_image[i + _plt0.rgba_channel[0]] * 0.2125);  // Grey Value
                                        j += 2;
                                        if (j == _plt0.canvas_width << 1)
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
                                        index[j] = (byte)(bmp_image[i + _plt0.rgba_channel[3]] * _plt0.custom_rgba[3]);  // _plt0.alpha value
                                        index[j + 1] = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2] + bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1] + bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);  // Grey Value
                                        j += 2;
                                        if (j == _plt0.canvas_width << 1)
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

                        switch (_plt0.algorithm)
                        {
                            case 2:  // custom  RRRR RGGG GGGB BBBB
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        red = (byte)(bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                        green = (byte)(bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);
                                        blue = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);
                                        if ((red & 7) > _plt0.round5 && red < 248)  // 5-bit max value on a trimmed byte
                                        {
                                            red += 8;
                                        }
                                        if ((green & _plt0.round6) == _plt0.round6 && green < 252)  // 6-bit max value on a trimmed byte
                                        {
                                            green += 4;
                                        }
                                        if ((blue & 7) > _plt0.round5 && blue < 248)
                                        {
                                            blue += 8;
                                        }
                                        index[j] = (byte)((red & 0xf8) + (green >> 5));
                                        index[j + 1] = (byte)(((green << 3) & 224) + (blue >> 3));
                                        j += 2;
                                        if (j == _plt0.canvas_width << 1)
                                        {
                                            j = 0;
                                            index_list.Add(index.ToArray());
                                        }
                                    }

                                    break;
                                }
                            default: // RRRR RGGG GGGB BBBB
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                    {
                                        red = bmp_image[i + _plt0.rgba_channel[0]];
                                        green = bmp_image[i + _plt0.rgba_channel[1]];
                                        blue = bmp_image[i + _plt0.rgba_channel[2]];
                                        if ((red & 7) > _plt0.round5 && red < 248)  // 5-bit max value on a trimmed byte
                                        {
                                            red += 8;
                                        }
                                        if ((green & _plt0.round6) == _plt0.round6 && green < 252)  // 6-bit max value on a trimmed byte
                                        {
                                            green += 4;
                                        }
                                        if ((blue & 7) > _plt0.round5 && blue < 248)
                                        {
                                            blue += 8;
                                        }
                                        index[j] = (byte)((red & 0xf8) + (green >> 5));
                                        index[j + 1] = (byte)(((green << 3) & 224) + (blue >> 3));
                                        j += 2;
                                        if (j == _plt0.canvas_width << 1)
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
                        switch (_plt0.algorithm)
                        {
                            case 2:  // custom
                                {
                                    if (_plt0.alpha == 1)  // 0AAA RRRR GGGG BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            a = (byte)(bmp_image[i + _plt0.rgba_channel[3]] * _plt0.custom_rgba[3]);
                                            red = (byte)(bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                            green = (byte)(bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);
                                            blue = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);
                                            if ((a & 31) > _plt0.round3 && a < 224)  // 3-bit max value on a trimmed byte
                                            {
                                                a += 32;
                                            }
                                            if ((red & 15) > _plt0.round4 && red < 240)  // 4-bit max value on a trimmed byte
                                            {
                                                red += 16;
                                            }
                                            if ((green & 15) > _plt0.round4 && green < 240)
                                            {
                                                green += 16;
                                            }
                                            if ((blue & 15) > _plt0.round4 && blue < 240)
                                            {
                                                blue += 16;
                                            }
                                            index[j] = (byte)(((a >> 1) & 0x70) + (red >> 4));
                                            index[j + 1] = (byte)((green & 0xf0) + (blue >> 4));
                                            j += 2;
                                            if (j == _plt0.canvas_width << 1)
                                            {
                                                j = 0;
                                                index_list.Add(index.ToArray());
                                            }
                                        }
                                    }
                                    else if (_plt0.alpha == 0)  // 1RRR RRGG GGGB BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            red = (byte)(bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                            green = (byte)(bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);
                                            blue = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);
                                            if ((red & 7) > _plt0.round5 && red < 248)  // 5-bit max value on a trimmed byte
                                            {
                                                red += 8;
                                            }
                                            if ((green & 7) > _plt0.round5 && green < 248)
                                            {
                                                green += 8;
                                            }
                                            if ((blue & 7) > _plt0.round5 && blue < 248)
                                            {
                                                blue += 8;
                                            }
                                            index[j] = (byte)(0x80 + ((red >> 1) & 0x7c) + (green >> 6));
                                            index[j + 1] = (byte)(((green << 2) & 0xe0) + (blue >> 3));
                                            j += 2;
                                            if (j == _plt0.canvas_width << 1)
                                            {
                                                j = 0;
                                                index_list.Add(index.ToArray());
                                            }
                                        }
                                    }
                                    else  // check for each colour if _plt0.alpha trimmed to 3 bits is 255
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            a = (byte)(bmp_image[i + _plt0.rgba_channel[3]] * _plt0.custom_rgba[3]);
                                            red = (byte)(bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                            green = (byte)(bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);
                                            blue = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);
                                            if ((a & 31) > _plt0.round3 && a < 224)  // 3-bit max value on a trimmed byte
                                            {
                                                a += 32;
                                            }
                                            if ((red & 15) > _plt0.round4 && red < 240)  // 4-bit max value on a trimmed byte
                                            {
                                                red += 16;
                                            }
                                            if ((green & 15) > _plt0.round4 && green < 240)
                                            {
                                                green += 16;
                                            }
                                            if ((blue & 15) > _plt0.round4 && blue < 240)
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
                                            if (j == _plt0.canvas_width << 1)
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
                                    if (_plt0.alpha == 1)  // 0AAA RRRR GGGG BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            a = bmp_image[i + _plt0.rgba_channel[3]];
                                            red = bmp_image[i + _plt0.rgba_channel[0]];
                                            green = bmp_image[i + _plt0.rgba_channel[1]];
                                            blue = bmp_image[i + _plt0.rgba_channel[2]];
                                            if ((a & 31) > _plt0.round3 && a < 224)  // 3-bit max value on a trimmed byte
                                            {
                                                a += 32;
                                            }
                                            if ((red & 15) > _plt0.round4 && red < 240)  // 4-bit max value on a trimmed byte
                                            {
                                                red += 16;
                                            }
                                            if ((green & 15) > _plt0.round4 && green < 240)
                                            {
                                                green += 16;
                                            }
                                            if ((blue & 15) > _plt0.round4 && blue < 240)
                                            {
                                                blue += 16;
                                            }
                                            index[j] = (byte)(((a >> 1) & 0x70) + (red >> 4));
                                            index[j + 1] = (byte)((green & 0xf0) + (blue >> 4));
                                            j += 2;
                                            if (j == _plt0.canvas_width << 1)
                                            {
                                                j = 0;
                                                index_list.Add(index.ToArray());
                                            }
                                        }
                                    }
                                    else if (_plt0.alpha == 0)  // 1RRR RRGG GGGB BBBB
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            red = bmp_image[i + _plt0.rgba_channel[0]];
                                            green = bmp_image[i + _plt0.rgba_channel[1]];
                                            blue = bmp_image[i + _plt0.rgba_channel[2]];
                                            if ((red & 7) > _plt0.round5 && red < 248)  // 5-bit max value on a trimmed byte
                                            {
                                                red += 8;
                                            }
                                            if ((green & 7) > _plt0.round5 && green < 248)
                                            {
                                                green += 8;
                                            }
                                            if ((blue & 7) > _plt0.round5 && blue < 248)
                                            {
                                                blue += 8;
                                            }
                                            index[j] = (byte)(0x80 + ((red >> 1) & 0x7c) + (green >> 6));
                                            index[j + 1] = (byte)(((green << 2) & 0xe0) + (blue >> 3));
                                            j += 2;
                                            if (j == _plt0.canvas_width << 1)
                                            {
                                                j = 0;
                                                index_list.Add(index.ToArray());
                                            }
                                        }
                                    }
                                    else  // mix up _plt0.alpha and no _plt0.alpha
                                    {
                                        for (int i = pixel_data_start_offset; i < bmp_filesize; i += 4)
                                        {
                                            a = bmp_image[i + _plt0.rgba_channel[3]];
                                            red = bmp_image[i + _plt0.rgba_channel[0]];
                                            green = bmp_image[i + _plt0.rgba_channel[1]];
                                            blue = bmp_image[i + _plt0.rgba_channel[2]];
                                            if ((a & 31) > _plt0.round3 && a < 224)  // 3-bit max value on a trimmed byte
                                            {
                                                a += 32;
                                            }
                                            if ((red & 15) > _plt0.round4 && red < 240)  // 4-bit max value on a trimmed byte
                                            {
                                                red += 16;
                                            }
                                            if ((green & 15) > _plt0.round4 && green < 240)
                                            {
                                                green += 16;
                                            }
                                            if ((blue & 15) > _plt0.round4 && blue < 240)
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
                                            if (j == _plt0.canvas_width << 1)
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
                        switch (_plt0.algorithm)
                        {
                            case 2:  // custom
                                {
                                    for (int i = pixel_data_start_offset; i < bmp_filesize; i += 16)
                                    {
                                        // _plt0.alpha and red
                                        index[j] = (byte)(bmp_image[i + _plt0.rgba_channel[3]] * _plt0.custom_rgba[3]);       // A
                                        index[j + 1] = (byte)(bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);   // R
                                        index[j + 2] = (byte)(bmp_image[i + 4 + _plt0.rgba_channel[3]] * _plt0.custom_rgba[3]);   // A
                                        index[j + 3] = (byte)(bmp_image[i + 4 + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);   // R
                                        index[j + 4] = (byte)(bmp_image[i + 8 + _plt0.rgba_channel[3]] * _plt0.custom_rgba[3]);  // A
                                        index[j + 5] = (byte)(bmp_image[i + 8 + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);  // R
                                        index[j + 6] = (byte)(bmp_image[i + 12 + _plt0.rgba_channel[3]] * _plt0.custom_rgba[3]);  // A
                                        index[j + 7] = (byte)(bmp_image[i + 12 + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);  // R
                                                                                                                                  // Green and Blue
                                        index[j + 8] = (byte)(bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);   // G
                                        index[j + 9] = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);       // B
                                        index[j + 10] = (byte)(bmp_image[i + 4 + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);  // G
                                        index[j + 11] = (byte)(bmp_image[i + 4 + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);  // B
                                        index[j + 12] = (byte)(bmp_image[i + 8 + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);  // G
                                        index[j + 13] = (byte)(bmp_image[i + 8 + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);  // B
                                        index[j + 14] = (byte)(bmp_image[i + 12 + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]); // G
                                        index[j + 15] = (byte)(bmp_image[i + 12 + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]); // B
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
                                        // _plt0.alpha and red
                                        index[j] = (byte)(bmp_image[i + _plt0.rgba_channel[3]]);       // A
                                        index[j + 1] = (byte)(bmp_image[i + _plt0.rgba_channel[0]]);   // R
                                        index[j + 2] = (byte)(bmp_image[i + 4 + _plt0.rgba_channel[3]]);   // A
                                        index[j + 3] = (byte)(bmp_image[i + 4 + _plt0.rgba_channel[0]]);   // R
                                        index[j + 4] = (byte)(bmp_image[i + 8 + _plt0.rgba_channel[3]]);  // A
                                        index[j + 5] = (byte)(bmp_image[i + 8 + _plt0.rgba_channel[0]]);  // R
                                        index[j + 6] = (byte)(bmp_image[i + 12 + _plt0.rgba_channel[3]]);  // A
                                        index[j + 7] = (byte)(bmp_image[i + 12 + _plt0.rgba_channel[0]]);  // R
                                                                                                           // Green and Blue
                                        index[j + 8] = (byte)(bmp_image[i + _plt0.rgba_channel[1]]);   // G
                                        index[j + 9] = (byte)(bmp_image[i + _plt0.rgba_channel[2]]);       // B
                                        index[j + 10] = (byte)(bmp_image[i + 4 + _plt0.rgba_channel[1]]);  // G
                                        index[j + 11] = (byte)(bmp_image[i + 4 + _plt0.rgba_channel[2]]);  // B
                                        index[j + 12] = (byte)(bmp_image[i + 8 + _plt0.rgba_channel[1]]);  // G
                                        index[j + 13] = (byte)(bmp_image[i + 8 + _plt0.rgba_channel[2]]);  // B
                                        index[j + 14] = (byte)(bmp_image[i + 12 + _plt0.rgba_channel[1]]); // G
                                        index[j + 15] = (byte)(bmp_image[i + 12 + _plt0.rgba_channel[2]]); // B
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
                        index_list.Clear();  // removes the "fill height" lines, because UH YOUVE GUESSED IT, I4M NOT STORING THESE IN LINE ORDER BUT IN SUB-BLOCK ORDER
                                             // I swear this is a nightmare
                        List<ushort> Colour_rgb565 = new List<ushort>();  // won't be sorted
                        List<ushort[]> Colour_list = new List<ushort[]>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
                                                                            // byte[] Colour_count = {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };  // 16 pixels, because a block is 4x4
                        j = 0;
                        int x = 0;
                        byte c;
                        ushort alpha_bitfield = 0;
                        byte red2;
                        byte green2;
                        byte blue2;
                        // byte f;
                        // int[] total_diff = {0, 0, 0};  // total_diff, e, f
                        // List<int[]> diff_array = new List<int[]>();
                        // List<byte> ef = new List<byte>();
                        ushort[] Colour_pixel = { 1, 0 };  // case 3
                        ushort width = 0;
                        ushort[] Colour_array = { 1, 0, 0 };  // default
                        ushort diff_max;
                        byte diff_max_index = 0;
                        List<ushort> colour_palette = new List<ushort>();
                        // bool use__plt0.alpha = false;
                        // bool done = false;
                        Array.Resize(ref index, 8);  // sub block length
                        int y = 0;
                        ushort z = 0;
                        switch (_plt0.algorithm)
                        {
                            case 2: // custom
                                {
                                    for (y = pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < bmp_filesize; y += 4)
                                    {
                                        red = (byte)(bmp_image[y + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);
                                        green = (byte)(bmp_image[y + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);
                                        blue = (byte)(bmp_image[y + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);
                                        if (_plt0.alpha > 0 && bmp_image[y + 3] < _plt0.cmpr_alpha_threshold)
                                        {
                                            alpha_bitfield += (ushort)(1 << (j + (z * 4)));
                                        }
                                        if ((red & 7) > _plt0.round5 && red < 248)  // 5-bit max value on a trimmed byte
                                        {
                                            red += 8;
                                        }
                                        if ((green & _plt0.round6) == _plt0.round6 && green < 252)  // 6-bit max value on a trimmed byte
                                        {
                                            green += 4;
                                        }
                                        if ((blue & 7) > _plt0.round5 && blue < 248)
                                        {
                                            blue += 8;
                                        }
                                        // Colour_pixel[0] = // the number of occurences, though it stays to 1 so that's not really a problem lol
                                        pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                                        Colour_array[1] = pixel;
                                        Colour_array[2] = (ushort)(red + green + blue); // best way to find darkest colour :D
                                        Colour_list.Add(Colour_array.ToArray());
                                        Colour_rgb565.Add(pixel);
                                        j++;
                                        if (j != 4)
                                        {
                                            continue;
                                        }
                                        j = 0;
                                        z++;
                                        y += (_plt0.canvas_width << 2) - 16; // returns to the start of the next line  - bitmap width << 2 because it's a 32-bit BGRA bmp file
                                        if (z != 4)
                                        {
                                            continue;  // Still within the same 4x4 block
                                        }
                                        x++;
                                        z = 0;
                                        width += 2;  // triggered 4 times per block
                                        if (width == _plt0.canvas_width)
                                        {
                                            width = 0;
                                            y += (_plt0.canvas_width << 2) - 16;
                                            x = 0;
                                        }
                                        else if (x == 2)
                                        {
                                            y += 16;  // you just need to add 32 everywhere
                                        }
                                        else if (x == 4)
                                        {
                                            y -= (_plt0.canvas_width << 5) + 16;// minus 8 lines + point to next block
                                            x = 0;
                                        }
                                        else
                                        {
                                            y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
                                        }
                                        // now let's just try to take the most two used colours and use _plt0.diversity I guess
                                        // implementing my own way to find most used colours:
                                        // let's count the number of exact same colours in Colour_list
                                        for (byte i = 0; i < 15; i++)  // useless to set it to 16 because of the condition k > i.
                                        {
                                            for (byte k = 0; k < 16; k++)
                                            {
                                                if (k == i)
                                                {
                                                    continue;
                                                }
                                                if (Colour_list[k][1] == Colour_list[i][1] && k > i && ((alpha_bitfield >> k) & 1) == 0 && ((alpha_bitfield >> i) & 1) == 0)  // k > i prevents colours occurences from being added twice.
                                                {
                                                    Colour_list[k][0]++;
                                                    Colour_list[i][0] = 0; // should set it to zero.
                                                }
                                            }
                                        }
                                        Colour_list.Sort(new UshortArrayComparer());  // sorts the table by the most used colour first
                                                                                      //now let's take the darkest and the brightest colour from the cmpr_max most used ones
                                        diff_min = 1024;
                                        diff_max = 0;
                                        for (byte i = 0; i < _plt0.cmpr_max && Colour_list[i][0] != 0; i++)
                                        {
                                            if (Colour_list[i][2] < diff_min)
                                            {
                                                diff_min = (short)(Colour_list[i][2]);
                                                diff_min_index = i;
                                            }
                                            if (Colour_list[i][2] > diff_max)
                                            {
                                                diff_max = Colour_list[i][2];
                                                diff_max_index = i;
                                            }
                                        }

                                        if (alpha_bitfield == 0)  // put the biggest ushort in second place
                                        {
                                            colour_palette.Add(Colour_list[diff_min_index][1]);
                                            colour_palette.Add(Colour_list[diff_max_index][1]);
                                            index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                                            index[1] = (byte)(Colour_list[diff_min_index][1]);
                                            index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                                            index[3] = (byte)(Colour_list[diff_max_index][1]);
                                            red = (byte)(((index[0] & 248) + (index[2] & 248)) / 2);
                                            green = (byte)(((((index[0] & 7) << 5) + ((index[1] >> 3) & 28)) + (((index[2] & 7) << 5) + ((index[3] >> 3) & 28))) / 2);
                                            blue = (byte)((((index[1] << 3) & 248) + ((index[3] << 3) & 248)) / 2);
                                            colour_palette.Add((ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)));  // the RGB565 third colour
                                                                                                                                   // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                                        }
                                        else
                                        {
                                            // of course, that's the exact opposite!
                                            colour_palette.Add(Colour_list[diff_max_index][1]);
                                            colour_palette.Add(Colour_list[diff_min_index][1]);
                                            index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                                            index[1] = (byte)(Colour_list[diff_max_index][1]);
                                            index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                                            index[3] = (byte)(Colour_list[diff_min_index][1]);

                                            red = (byte)(index[0] & 248);
                                            green = (byte)(((index[0] & 7) << 5) + ((index[1] >> 3) & 28));
                                            blue = (byte)((index[1] << 3) & 248);

                                            red2 = (byte)(index[2] & 248);
                                            green2 = (byte)(((index[2] & 7) << 5) + ((index[3] >> 3) & 28));
                                            blue2 = (byte)((index[3] << 3) & 248);

                                            pixel = (ushort)(((((red * 2 / 3) + (red2 / 3)) >> 3) << 11) + ((((green * 2 / 3) + (green2 / 3)) >> 2) << 5) + (((blue * 2 / 3) + (blue2 / 3)) >> 3));
                                            colour_palette.Add(pixel);  // the RGB565 third colour
                                            pixel = (ushort)(((((red / 3) + (red2 * 2 / 3)) >> 3) << 11) + ((((green / 3) + (green2 * 2 / 3)) >> 2) << 5) + (((blue / 3) + (blue2 * 2 / 3)) >> 3));
                                            colour_palette.Add(pixel);  // the RGB565 fourth colour
                                        }
                                        /*
                                         * t = (pixel_posN - pixel_pos1) / (pixel_pos2 - pixel_pos1)
pixelN_red = (t-1)*pixel1_red + (t)*pixel2_red
same for blue + green*/
                                        for (byte i = 4; i < 8; i++)
                                        {
                                            index[i] = 0;
                                        }
                                        // time to get the "linear interpolation to add third and fourth colour
                                        // CI2 if that's a name lol
                                        if (_plt0.reverse_x)
                                        {
                                            for (sbyte h = 3; h >= 0; h--)
                                            {
                                                for (sbyte w = 3; w >= 0; w--)  // index_size = number of pixels
                                                {
                                                    if (((alpha_bitfield >> (h * 4) + w) & 1) == 1)
                                                    {
                                                        index[7 - h] += (byte)(3 << (w << 1));
                                                        continue;
                                                    }
                                                    diff_min = 500;
                                                    // diff_min_index = w;
                                                    for (byte i = 0; i < colour_palette.Count; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                    {
                                                        if (colour_palette[i] == Colour_rgb565[(h * 4) + w])  // if it's the exact same colour
                                                        {
                                                            diff_min_index = i;  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                            break;
                                                        }
                                                        else  // calculate difference between each separate colour channel and store the sum
                                                        {
                                                            diff = (short)(Math.Abs(((colour_palette[i] >> 8) & 248) - ((Colour_rgb565[(h * 4) + w] >> 8) & 248)) + Math.Abs(((colour_palette[i] >> 3) & 252) - ((Colour_rgb565[(h * 4) + w] >> 3) & 252)) + Math.Abs(((colour_palette[i] << 3) & 248) - ((Colour_rgb565[(h * 4) + w] << 3) & 248)));
                                                            if (diff < diff_min)
                                                            {
                                                                diff_min = diff;
                                                                diff_min_index = i;
                                                            }
                                                        }
                                                    }
                                                    index[7 - h] += (byte)(diff_min_index << (w << 1));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (sbyte h = 3; h >= 0; h--)
                                            //for (byte h = 0; h < 4; h++)
                                            {
                                                for (byte w = 0; w < 4; w++)  // index_size = number of pixels
                                                {
                                                    if (((alpha_bitfield >> (h * 4) + w) & 1) == 1)
                                                    {
                                                        index[7 - h] += (byte)(3 << (6 - (w << 1)));
                                                        continue;
                                                    }
                                                    diff_min = 500;
                                                    // diff_min_index = w;
                                                    for (byte i = 0; i < colour_palette.Count; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                    {
                                                        if (colour_palette[i] == Colour_rgb565[(h * 4) + w])  // if it's the exact same colour
                                                        {
                                                            diff_min_index = i;  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                            break;
                                                        }
                                                        else  // calculate difference between each separate colour channel and store the sum
                                                        {
                                                            diff = (short)(Math.Abs(((colour_palette[i] >> 8) & 248) - ((Colour_rgb565[(h * 4) + w] >> 8) & 248)) + Math.Abs(((colour_palette[i] >> 3) & 252) - ((Colour_rgb565[(h * 4) + w] >> 3) & 252)) + Math.Abs(((colour_palette[i] << 3) & 248) - ((Colour_rgb565[(h * 4) + w] << 3) & 248)));
                                                            if (diff < diff_min)
                                                            {
                                                                diff_min = diff;
                                                                diff_min_index = i;
                                                            }
                                                        }
                                                    }
                                                    index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                                                    // Console.WriteLine(index[4 + h]);
                                                }
                                            }
                                        }
                                        index_list.Add(index.ToArray());
                                        // index is overwritten each time
                                        // the lists need to be cleaned
                                        Colour_list.Clear();
                                        colour_palette.Clear();
                                        Colour_rgb565.Clear();
                                        alpha_bitfield = 0;
                                        // THAT INDEX ARRAY THAT I CAN4T SEE CONTENTS IN THE DEBUGGER ALSO NEEDS TO BE CLEANED
                                        // edit: moved it after the swap function THAT FREAKING DOES CHANGE ARRAY CONTENTS
                                    }
                                }
                                break;
                            case 3:  // most used colours with _plt0.diversity - no gradient - similar - looks pixelated
                                {
                                    for (y = pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < bmp_filesize; y += 4)
                                    {
                                        red = bmp_image[y + _plt0.rgba_channel[0]];
                                        green = bmp_image[y + _plt0.rgba_channel[1]];
                                        blue = bmp_image[y + _plt0.rgba_channel[2]];
                                        if (_plt0.alpha > 0 && bmp_image[y + 3] < _plt0.cmpr_alpha_threshold)
                                        {
                                            alpha_bitfield += (ushort)(1 << (j + (z * 4)));
                                        }
                                        if ((red & 7) > _plt0.round5 && red < 248)  // 5-bit max value on a trimmed byte
                                        {
                                            red += 8;
                                        }
                                        if ((green & _plt0.round6) == _plt0.round6 && green < 252)  // 6-bit max value on a trimmed byte
                                        {
                                            green += 4;
                                        }
                                        if ((blue & 7) > _plt0.round5 && blue < 248)
                                        {
                                            blue += 8;
                                        }
                                        // Colour_pixel[0] = // the number of occurences, though it stays to 1 so that's not really a problem lol
                                        pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                                        Colour_pixel[1] = pixel;
                                        Colour_list.Add(Colour_pixel.ToArray());
                                        Colour_rgb565.Add(pixel);
                                        j++;
                                        if (j != 4)
                                        {
                                            continue;
                                        }
                                        j = 0;
                                        z++;
                                        y += (_plt0.canvas_width << 2) - 16; // returns to the start of the next line  - bitmap width << 2 because it's a 32-bit BGRA bmp file
                                        if (z != 4)
                                        {
                                            continue;  // Still within the same 4x4 block
                                        }
                                        x++;
                                        z = 0;
                                        width += 2;  // triggered 4 times per block
                                        if (width == _plt0.canvas_width)
                                        {
                                            width = 0;
                                            y += (_plt0.canvas_width << 2) - 16; // this has been driving me nuts
                                            x = 0;
                                        }
                                        else if (x == 2)
                                        {
                                            // y += (_plt0.bitmap_width << 4) - 4; // adds 4 lines and put the cursor back to the first block in width (I hope)
                                            // y += 16; // hmm, it looks like the cursor warped horizontally to the first block in width 4 lines above
                                            // EDIT: YA DEFINITELY NEED TO CANCEL THE Y OPERATION ABOVE, IT WARPS NORMALLY LIKE IT4S THE PIXEL AFTER
                                            //y -= (_plt0.bitmap_width << 2) - 16;  // this has been driving me nuts
                                            y += 16;  // I can't believe this is right in the mirror and mirrorred mode lol
                                                      // edit: you just need to add 32 everywhere
                                        }
                                        else if (x == 4)
                                        {
                                            y -= (_plt0.canvas_width << 5) + 16; // minus 8 lines + point to next block
                                            x = 0;
                                        }
                                        else
                                        {


                                            y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
                                        }
                                        // now let's just try to take the most two used colours and use _plt0.diversity I guess
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
                                                if (Colour_list[k][1] == Colour_list[i][1] && k > i && ((alpha_bitfield >> k) & 1) == 0 && ((alpha_bitfield >> i) & 1) == 0)  // k > i prevents colours occurences from being added twice.
                                                {
                                                    Colour_list[k][0]++;
                                                    Colour_list[i][0] = 0; // should set it to zero.
                                                }
                                            }
                                        }
                                        Colour_list.Sort(new UshortArrayComparer());  // sorts the table by the most used colour first
                                        c = 0;
                                        for (int i = 0; i < 16 && c < 4; i++)  // build the colour table with the two most used colours and _plt0.diversityfffffffffffff
                                        {
                                            not_similar = true;
                                            if (Colour_list[i][0] / 16 < _plt0.percentage / 100)
                                            {
                                                // break;  // STOP BREAKING THE F*CKING LOOP
                                                continue;
                                            }
                                            if (c == 2)  // checks for _plt0.diversity before adding the second colour ^^
                                            {
                                                if (Math.Abs((index[0] & 248) - ((Colour_list[i][1] >> 8) & 248)) < _plt0.diversity && Math.Abs(((index[0] & 7) << 5) + ((index[1] >> 3) & 28) - ((Colour_list[i][1] >> 3) & 252)) < _plt0.diversity && Math.Abs(((index[1] << 3) & 248) - (Colour_list[i][1] << 3) & 248) < _plt0.diversity)
                                                {
                                                    not_similar = false;
                                                    // break;  // HOLY SHIT YOU4VE BROKEN THE LOOP
                                                    continue;
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
                                            // Console.WriteLine("The colour palette was not full, starting second loop...\n");

                                            for (int i = 0; i < 16 && c < 4; i++)
                                            {
                                                not_similar = true;
                                                if (Colour_list[i][0] / 16 < _plt0.percentage2 / 100)
                                                {
                                                    continue;
                                                }
                                                if (c == 2)  // checks for _plt0.diversity before adding the second colour ^^
                                                {
                                                    if (Math.Abs((index[0] & 248) - ((Colour_list[i][1] >> 8) & 248)) < _plt0.diversity2 && Math.Abs(((index[0] & 7) << 5) + ((index[1] >> 3) & 28) - ((Colour_list[i][1] >> 3) & 252)) < _plt0.diversity2 && Math.Abs(((index[1] << 3) & 248) - (Colour_list[i][1] << 3) & 248) < _plt0.diversity2)
                                                    {
                                                        not_similar = false;
                                                        continue;
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
                                                // Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                                for (int i = 0; i < 16 && c < 4; i++)
                                                {
                                                    not_similar = true;
                                                    if (c == 2)
                                                    {
                                                        if ((index[0] == (byte)(Colour_list[i][1] >> 8)) && index[1] == (byte)(Colour_list[i][1]))
                                                        {
                                                            not_similar = false;
                                                            continue;
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
                                        if (alpha_bitfield == 0)  // put the biggest ushort in second place
                                        {
                                            if (index[0] > index[2] || (index[0] == index[2] && index[1] > index[3]))  // swap
                                            {
                                                index[4] = index[0];
                                                index[5] = index[1];
                                                index[0] = index[2];
                                                index[1] = index[3];
                                                index[2] = index[4];
                                                index[3] = index[5];
                                            }
                                            colour_palette.Add((ushort)((index[0] << 8) + index[1]));
                                            colour_palette.Add((ushort)((index[2] << 8) + index[3]));
                                            red = (byte)(((index[0] & 248) + (index[2] & 248)) / 2);
                                            green = (byte)(((((index[0] & 7) << 5) + ((index[1] >> 3) & 28)) + (((index[2] & 7) << 5) + ((index[3] >> 3) & 28))) / 2);
                                            blue = (byte)((((index[1] << 3) & 248) + ((index[3] << 3) & 248)) / 2);
                                            colour_palette.Add((ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)));  // the RGB565 third colour
                                                                                                                                   // last colour isn't in the palette, it's in _plt0.alpha_bitfield
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
                                                index[2] = index[4];
                                                index[3] = index[5];  // this is confusing
                                            }
                                            colour_palette.Add((ushort)((index[0] << 8) + index[1]));
                                            colour_palette.Add((ushort)((index[2] << 8) + index[3]));

                                            red = (byte)(index[0] & 248);
                                            green = (byte)(((index[0] & 7) << 5) + ((index[1] >> 3) & 28));
                                            blue = (byte)((index[1] << 3) & 248);

                                            red2 = (byte)(index[2] & 248);
                                            green2 = (byte)(((index[2] & 7) << 5) + ((index[3] >> 3) & 28));
                                            blue2 = (byte)((index[3] << 3) & 248);

                                            pixel = (ushort)(((((red * 2 / 3) + (red2 / 3)) >> 3) << 11) + ((((green * 2 / 3) + (green2 / 3)) >> 2) << 5) + (((blue * 2 / 3) + (blue2 / 3)) >> 3));
                                            colour_palette.Add(pixel);  // the RGB565 third colour
                                            pixel = (ushort)(((((red / 3) + (red2 * 2 / 3)) >> 3) << 11) + ((((green / 3) + (green2 * 2 / 3)) >> 2) << 5) + (((blue / 3) + (blue2 * 2 / 3)) >> 3));
                                            colour_palette.Add(pixel);  // the RGB565 fourth colour
                                        }
                                        for (byte i = 4; i < 8; i++)
                                        {
                                            index[i] = 0;
                                        }
                                        // time to get the "linear interpolation to add third and fourth colour
                                        // CI2 if that's a name lol

                                        if (_plt0.reverse_x)
                                        {
                                            for (sbyte h = 3; h >= 0; h--)
                                            {
                                                for (sbyte w = 3; w >= 0; w--)  // index_size = number of pixels
                                                {
                                                    if (((alpha_bitfield >> (h * 4) + w) & 1) == 1)
                                                    {
                                                        index[7 - h] += (byte)(3 << (w << 1));
                                                        continue;
                                                    }
                                                    diff_min = 500;
                                                    // diff_min_index = w;
                                                    for (byte i = 0; i < colour_palette.Count; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                    {
                                                        if (colour_palette[i] == Colour_rgb565[(h * 4) + w])  // if it's the exact same colour
                                                        {
                                                            diff_min_index = i;  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                            break;
                                                        }
                                                        else  // calculate difference between each separate colour channel and store the sum
                                                        {
                                                            diff = (short)(Math.Abs(((colour_palette[i] >> 8) & 248) - ((Colour_rgb565[(h * 4) + w] >> 8) & 248)) + Math.Abs(((colour_palette[i] >> 3) & 252) - ((Colour_rgb565[(h * 4) + w] >> 3) & 252)) + Math.Abs(((colour_palette[i] << 3) & 248) - ((Colour_rgb565[(h * 4) + w] << 3) & 248)));
                                                            if (diff < diff_min)
                                                            {
                                                                diff_min = diff;
                                                                diff_min_index = i;
                                                            }
                                                        }
                                                    }
                                                    index[7 - h] += (byte)(diff_min_index << (w << 1));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (sbyte h = 3; h >= 0; h--)
                                            //for (byte h = 0; h < 4; h++)
                                            {
                                                for (byte w = 0; w < 4; w++)  // index_size = number of pixels
                                                {
                                                    if (((alpha_bitfield >> (h * 4) + w) & 1) == 1)
                                                    {
                                                        index[7 - h] += (byte)(3 << (6 - (w << 1)));
                                                        continue;
                                                    }
                                                    diff_min = 500;
                                                    // diff_min_index = w;
                                                    for (byte i = 0; i < colour_palette.Count; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                    {
                                                        if (colour_palette[i] == Colour_rgb565[(h * 4) + w])  // if it's the exact same colour
                                                        {
                                                            diff_min_index = i;  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                            break;
                                                        }
                                                        else  // calculate difference between each separate colour channel and store the sum
                                                        {
                                                            diff = (short)(Math.Abs(((colour_palette[i] >> 8) & 248) - ((Colour_rgb565[(h * 4) + w] >> 8) & 248)) + Math.Abs(((colour_palette[i] >> 3) & 252) - ((Colour_rgb565[(h * 4) + w] >> 3) & 252)) + Math.Abs(((colour_palette[i] << 3) & 248) - ((Colour_rgb565[(h * 4) + w] << 3) & 248)));
                                                            if (diff < diff_min)
                                                            {
                                                                diff_min = diff;
                                                                diff_min_index = i;
                                                            }
                                                        }
                                                    }
                                                    index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                                                }
                                            }
                                        }
                                        index_list.Add(index.ToArray());
                                        // index is overwritten each time
                                        // the lists need to be cleaned
                                        Colour_list.Clear();
                                        colour_palette.Clear();
                                        Colour_rgb565.Clear();
                                        alpha_bitfield = 0;
                                        // THAT INDEX ARRAY THAT I CAN4T SEE CONTENTS IN THE DEBUGGER ALSO NEEDS TO BE CLEANED
                                    }
                                }
                                break;

                            default: // linear
                                {
                                    for (y = pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < bmp_filesize; y += 4)
                                    {
                                        red = bmp_image[y + _plt0.rgba_channel[0]];
                                        green = bmp_image[y + _plt0.rgba_channel[1]];
                                        blue = bmp_image[y + _plt0.rgba_channel[2]];
                                        if (_plt0.alpha > 0 && bmp_image[y + 3] < _plt0.cmpr_alpha_threshold)
                                        {
                                            alpha_bitfield += (ushort)(1 << (j + (z * 4)));
                                        }
                                        if ((red & 7) > _plt0.round5 && red < 248)  // 5-bit max value on a trimmed byte
                                        {
                                            red += 8;
                                        }
                                        if ((green & _plt0.round6) == _plt0.round6 && green < 252)  // 6-bit max value on a trimmed byte
                                        {
                                            green += 4;
                                        }
                                        if ((blue & 7) > _plt0.round5 && blue < 248)
                                        {
                                            blue += 8;
                                        }
                                        // Colour_pixel[0] = // the number of occurences, though it stays to 1 so that's not really a problem lol
                                        pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                                        Colour_array[1] = pixel;
                                        Colour_array[2] = (ushort)(red + green + blue); // best way to find darkest colour :D
                                        Colour_list.Add(Colour_array.ToArray());
                                        Colour_rgb565.Add(pixel);
                                        j++;
                                        if (j != 4)
                                        {
                                            continue;
                                        }
                                        j = 0;
                                        z++;
                                        y += (_plt0.canvas_width << 2) - 16; // returns to the start of the next line  - bitmap width << 2 because it's a 32-bit BGRA bmp file
                                        if (z != 4)
                                        {
                                            continue;  // Still within the same 4x4 block
                                        }
                                        x++;
                                        z = 0;
                                        width += 2;  // triggered 4 times per block
                                        if (width == _plt0.canvas_width)
                                        {
                                            width = 0;
                                            // y -= (_plt0.bitmap_width << 1) - 16;  // this has been driving me nuts
                                            y += (_plt0.canvas_width << 2) - 16;
                                            x = 0;
                                        }
                                        else if (x == 2)
                                        {
                                            // y += (_plt0.bitmap_width << 4) - 4; // adds 4 lines and put the cursor back to the first block in width (I hope)
                                            // y += 16; // hmm, it looks like the cursor warped horizontally to the first block in width 4 lines above
                                            // EDIT: YA DEFINITELY NEED TO CANCEL THE Y OPERATION ABOVE, IT WARPS NORMALLY LIKE IT4S THE PIXEL AFTER
                                            //y -= (_plt0.bitmap_width << 2) - 16;  // this has been driving me nuts
                                            y += 16;  // I can't believe this is right in the mirror and mirrorred mode lol
                                                      // edit: you just need to add 32 everywhere
                                        }
                                        else if (x == 4)
                                        {
                                            //y -= (_plt0.bitmap_width << 5) - 16; // minus 8 lines + point to next block
                                            y -= (_plt0.canvas_width << 5) + 16;
                                            x = 0;
                                        }
                                        else
                                        {
                                            /* y -= (_plt0.bitmap_width << 4) - 16; // on retire 4 lignes et on passe le 1er block héhé
                                             substract 4 lines and jumps over the first block */


                                            y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
                                        }
                                        // now let's just try to take the most two used colours and use _plt0.diversity I guess
                                        // implementing my own way to find most used colours:
                                        // let's count the number of exact same colours in Colour_list
                                        for (byte i = 0; i < 15; i++)  // useless to set it to 16 because of the condition k > i.
                                        {
                                            for (byte k = 0; k < 16; k++)
                                            {
                                                if (k == i)
                                                {
                                                    continue;
                                                }
                                                if (Colour_list[k][1] == Colour_list[i][1] && k > i && ((alpha_bitfield >> k) & 1) == 0 && ((alpha_bitfield >> i) & 1) == 0)  // k > i prevents colours occurences from being added twice.
                                                {
                                                    Colour_list[k][0]++;
                                                    Colour_list[i][0] = 0; // should set it to zero.
                                                }
                                            }
                                        }
                                        Colour_list.Sort(new UshortArrayComparer());  // sorts the table by the most used colour first
                                                                                      //now let's take the darkest and the brightest colour from the cmpr_max most used ones
                                        diff_min = 1024;
                                        diff_max = 0;
                                        for (byte i = 0; i < _plt0.cmpr_max && Colour_list[i][0] != 0; i++)
                                        {
                                            if (Colour_list[i][2] < diff_min)
                                            {
                                                diff_min = (short)(Colour_list[i][2]);
                                                diff_min_index = i;
                                            }
                                            if (Colour_list[i][2] > diff_max)
                                            {
                                                diff_max = Colour_list[i][2];
                                                diff_max_index = i;
                                            }
                                        }

                                        if (alpha_bitfield == 0)  // put the biggest ushort in second place
                                        {
                                            colour_palette.Add(Colour_list[diff_min_index][1]);
                                            colour_palette.Add(Colour_list[diff_max_index][1]);
                                            index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                                            index[1] = (byte)(Colour_list[diff_min_index][1]);
                                            index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                                            index[3] = (byte)(Colour_list[diff_max_index][1]);
                                            red = (byte)(((index[0] & 248) + (index[2] & 248)) / 2);
                                            green = (byte)(((((index[0] & 7) << 5) + ((index[1] >> 3) & 28)) + (((index[2] & 7) << 5) + ((index[3] >> 3) & 28))) / 2);
                                            blue = (byte)((((index[1] << 3) & 248) + ((index[3] << 3) & 248)) / 2);
                                            colour_palette.Add((ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)));  // the RGB565 third colour
                                                                                                                                   // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                                        }
                                        else
                                        {
                                            // of course, that's the exact opposite!
                                            colour_palette.Add(Colour_list[diff_max_index][1]);
                                            colour_palette.Add(Colour_list[diff_min_index][1]);
                                            index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                                            index[1] = (byte)(Colour_list[diff_max_index][1]);
                                            index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                                            index[3] = (byte)(Colour_list[diff_min_index][1]);

                                            red = (byte)(index[0] & 248);
                                            green = (byte)(((index[0] & 7) << 5) + ((index[1] >> 3) & 28));
                                            blue = (byte)((index[1] << 3) & 248);

                                            red2 = (byte)(index[2] & 248);
                                            green2 = (byte)(((index[2] & 7) << 5) + ((index[3] >> 3) & 28));
                                            blue2 = (byte)((index[3] << 3) & 248);

                                            pixel = (ushort)(((((red * 2 / 3) + (red2 / 3)) >> 3) << 11) + ((((green * 2 / 3) + (green2 / 3)) >> 2) << 5) + (((blue * 2 / 3) + (blue2 / 3)) >> 3));
                                            colour_palette.Add(pixel);  // the RGB565 third colour
                                            pixel = (ushort)(((((red / 3) + (red2 * 2 / 3)) >> 3) << 11) + ((((green / 3) + (green2 * 2 / 3)) >> 2) << 5) + (((blue / 3) + (blue2 * 2 / 3)) >> 3));
                                            colour_palette.Add(pixel);  // the RGB565 fourth colour
                                        }
                                        /*
                                         * t = (pixel_posN - pixel_pos1) / (pixel_pos2 - pixel_pos1)
pixelN_red = (t-1)*pixel1_red + (t)*pixel2_red
same for blue + green*/
                                        for (byte i = 4; i < 8; i++)
                                        {
                                            index[i] = 0;
                                        }
                                        // time to get the "linear interpolation to add third and fourth colour
                                        // CI2 if that's a name lol
                                        if (_plt0.reverse_x)
                                        {
                                            for (sbyte h = 3; h >= 0; h--)
                                            {
                                                for (sbyte w = 3; w >= 0; w--)  // index_size = number of pixels
                                                {
                                                    if (((alpha_bitfield >> (h * 4) + w) & 1) == 1)
                                                    {
                                                        index[7 - h] += (byte)(3 << (w << 1));
                                                        continue;
                                                    }
                                                    diff_min = 500;
                                                    // diff_min_index = w;
                                                    for (byte i = 0; i < colour_palette.Count; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                    {
                                                        if (colour_palette[i] == Colour_rgb565[(h * 4) + w])  // if it's the exact same colour
                                                        {
                                                            diff_min_index = i;  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                            break;
                                                        }
                                                        else  // calculate difference between each separate colour channel and store the sum
                                                        {
                                                            diff = (short)(Math.Abs(((colour_palette[i] >> 8) & 248) - ((Colour_rgb565[(h * 4) + w] >> 8) & 248)) + Math.Abs(((colour_palette[i] >> 3) & 252) - ((Colour_rgb565[(h * 4) + w] >> 3) & 252)) + Math.Abs(((colour_palette[i] << 3) & 248) - ((Colour_rgb565[(h * 4) + w] << 3) & 248)));
                                                            if (diff < diff_min)
                                                            {
                                                                diff_min = diff;
                                                                diff_min_index = i;
                                                            }
                                                        }
                                                    }
                                                    index[7 - h] += (byte)(diff_min_index << (w << 1));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (sbyte h = 3; h >= 0; h--)
                                            //for (byte h = 0; h < 4; h++)
                                            {
                                                for (byte w = 0; w < 4; w++)  // index_size = number of pixels
                                                {
                                                    if (((alpha_bitfield >> (h * 4) + w) & 1) == 1)
                                                    {
                                                        index[7 - h] += (byte)(3 << (6 - (w << 1)));
                                                        continue;
                                                    }
                                                    diff_min = 500;
                                                    // diff_min_index = w;
                                                    for (byte i = 0; i < colour_palette.Count; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                                                    {
                                                        if (colour_palette[i] == Colour_rgb565[(h * 4) + w])  // if it's the exact same colour
                                                        {
                                                            diff_min_index = i;  // index is stored on 1 byte, while each colour is stored on 2 bytes
                                                            break;
                                                        }
                                                        else  // calculate difference between each separate colour channel and store the sum
                                                        {
                                                            diff = (short)(Math.Abs(((colour_palette[i] >> 8) & 248) - ((Colour_rgb565[(h * 4) + w] >> 8) & 248)) + Math.Abs(((colour_palette[i] >> 3) & 252) - ((Colour_rgb565[(h * 4) + w] >> 3) & 252)) + Math.Abs(((colour_palette[i] << 3) & 248) - ((Colour_rgb565[(h * 4) + w] << 3) & 248)));
                                                            if (diff < diff_min)
                                                            {
                                                                diff_min = diff;
                                                                diff_min_index = i;
                                                            }
                                                        }
                                                    }
                                                    index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                                                    // Console.WriteLine(index[4 + h]);
                                                }
                                            }
                                        }

                                        index_list.Add(index.ToArray());
                                        // index is overwritten each time
                                        // the lists need to be cleaned
                                        Colour_list.Clear();
                                        colour_palette.Clear();
                                        Colour_rgb565.Clear();
                                        alpha_bitfield = 0;
                                    }
                                }
                                break;

                        }
                        break;
                    }

            }
        }
        if (_plt0.reverse_y)
            index_list.Reverse();
        if (_plt0.reverse_x)
        {
            if (_plt0.texture_format_int32[3] == 0xE)
            {
                int blocks_wide = _plt0.canvas_width >> 2;
                int blocks_tall = _plt0.canvas_height >> 3;
                //byte[][] index_reversed = new byte[blocks_wide][]; // I guess byte[][] sucks nowadays that List<byte[]> exists, can't get it to work
                List<byte[]> index_reversed = new List<byte[]>();
                int h = ((blocks_wide>>1) - 1) << 2;
                for (int d = 0; d < blocks_tall; d++)
                {
                    for (int e = 0; e < blocks_wide >> 1; e++)
                    {
                        for (int i = blocks_wide - 3; i >= 0; i -= 4)
                        {
                            index_reversed.Add(index_list[h + i]);
                            index_reversed.Add(index_list[h + i - 1]);
                        }
                        for (int i = blocks_wide - 1; i >= 0; i -= 4)
                        {
                            index_reversed.Add(index_list[h + i]);
                            index_reversed.Add(index_list[h + i - 1]);
                        }
                        h -= 4;
                    }
                    h += (blocks_wide << 2);
                }
                return index_reversed;
            }
            else
            {
                for (int i = 0; i < index_list.Count; i++)
                {
                    index_list[i] = index_list[i].Reverse().ToArray();
                }
            }
        }
        return index_list;
    }
}