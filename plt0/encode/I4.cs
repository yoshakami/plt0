using System.Collections.Generic;
using System.Linq;

class I4_class
{
    Parse_args_class _plt0;
    public I4_class(Parse_args_class Parse_args_class)
    {
        _plt0 = Parse_args_class;
    }
    public void I4(List<byte[]> index_list, byte[] bmp_image, byte[] index)
    {
        int j = 0;
        byte a;
        byte grey;
        switch (_plt0.algorithm)
        {
            default: // cie_601
                {
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 8)  // process every pixel by groups of two to fit the AAAA BBBB  profile
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
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 8)
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
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 8)
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
            case 3:  // inverse of the gamma function
                Preceptual_Brightness_class gray_class = new Preceptual_Brightness_class();
                for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 8)
                {
                    a = (byte)(gray_class.Preceptual_Brightness(bmp_image[i + _plt0.rgba_channel[0]], bmp_image[i + _plt0.rgba_channel[1]], bmp_image[i + _plt0.rgba_channel[2]]));
                    if ((a & 0xf) > _plt0.round4 && a < 240)
                    {
                        a += 16;
                    }
                    grey = (byte)(gray_class.Preceptual_Brightness(bmp_image[i + 4 + _plt0.rgba_channel[0]], bmp_image[i + 4 + _plt0.rgba_channel[1]], bmp_image[i + 4 + _plt0.rgba_channel[2]]));
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
}