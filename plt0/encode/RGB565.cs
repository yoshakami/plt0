using System.Collections.Generic;
using System.Linq;

class RGB565_class
{
    Parse_args_class _plt0;
    public RGB565_class(Parse_args_class Parse_args_class)
    {
        _plt0 = Parse_args_class;
    }
    public void RGB565(List<byte[]> index_list, byte[] bmp_image, byte[] index)
    {
        int j = 0;
        byte red;
        byte green;
        byte blue;
        switch (_plt0.algorithm)
        {
            case 2:  // custom  RRRR RGGG GGGB BBBB
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
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
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
    }
}