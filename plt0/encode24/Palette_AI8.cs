using System;
using System.Collections.Generic;
using System.Linq;

class Palette_AI8_class24
{
    Parse_args_class _plt0;
    public Palette_AI8_class24(Parse_args_class Parse_args_class)
    {
        _plt0 = Parse_args_class;
    }
    public void Palette_AI8(List<byte[]> index_list, List<int[]> Colour_Table, byte[] bmp_image, byte[] index)
    {
        List<ushort> Colours = new List<ushort>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format

        int j = _plt0.fill_palette_start_offset;
        bool not_similar;
        short diff_min = 500;
        short diff = 0;
        byte diff_min_index = 0;
        ushort diff_min_ind14x2 = 0;
        ushort pixel;

        switch (_plt0.algorithm)
        {
            default: // cie_601
                {
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
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
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
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
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
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
            case 3:  // inverse of the gamma function
                Preceptual_Brightness_class gray_class = new Preceptual_Brightness_class();
                for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
                {
                    pixel = (ushort)(bmp_image[i + _plt0.rgba_channel[3]] << 8);  // _plt0.alpha value
                    if (pixel != 0)
                    {
                        pixel += (byte)gray_class.Preceptual_Brightness(bmp_image[i + _plt0.rgba_channel[0]], bmp_image[i + _plt0.rgba_channel[1]], bmp_image[i + _plt0.rgba_channel[2]]);  // Grey Value
                    }
                    Colours.Add(pixel);
                    Colour_Table[pixel][0] += 1;
                }
                break;
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
    }
}