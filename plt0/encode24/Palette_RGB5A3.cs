using System;
using System.Collections.Generic;
using System.Linq;

class Palette_RGB5A3_class
{
    Parse_args_class _plt0;
    public Palette_RGB5A3_class(Parse_args_class Parse_args_class)
    {
        _plt0 = Parse_args_class;
    }
    public void Palette_RGB5A3(List<byte[]> index_list, List<int[]> Colour_Table, byte[] bmp_image, byte[] index)
    {
        List<ushort> Colours = new List<ushort>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format

        int j = _plt0.fill_palette_start_offset;
        bool not_similar;
        short diff_min = 500;
        short diff = 0;
        byte diff_min_index = 0;
        ushort diff_min_ind14x2 = 0;
        ushort pixel;
        byte red;
        byte green;
        byte blue;
        byte a;
        switch (_plt0.algorithm)
        {
            case 2:  // custom
                {
                    if (_plt0.alpha == 1)  // 0AAA RRRR GGGG BBBB
                    {
                        for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
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
                        for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
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
                        for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
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
                        for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
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
                        for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
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
                        for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
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
    }  // end of Palette_RGB5A3
} // end of Palette_RGB5A3_class