using System.Collections.Generic;
using System.Linq;

class RGB5A3_class
{
    Parse_args_class _plt0;
    public RGB5A3_class(Parse_args_class Parse_args_class)
    {
        _plt0 = Parse_args_class;
    }
    public void RGB5A3(List<byte[]> index_list, byte[] bmp_image, byte[] index)
    {
        int j = 0;
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
                            if (a > 223) // 1RRR RRGG GGGB BBBB
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
    }
}