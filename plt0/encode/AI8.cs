using System.Collections.Generic;
using System.Linq;

class AI8_class
{
    Parse_args_class _plt0;
    public AI8_class(Parse_args_class Parse_args_class)
    {
        _plt0 = Parse_args_class;
    }
    public void AI8(List<byte[]> index_list, byte[] bmp_image, byte[] index)
    {
        int j = 0;
        switch (_plt0.algorithm)
        {
            default: // cie_601
                {
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
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
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
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
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
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
            case 3:  // inverse of the gamma function
                Preceptual_Brightness_class gray_class = new Preceptual_Brightness_class();
                for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 4)
                {
                    index[j] = (byte)(bmp_image[i + _plt0.rgba_channel[3]]);  // _plt0.alpha value
                    index[j + 1] = (byte)gray_class.Preceptual_Brightness(bmp_image[i + _plt0.rgba_channel[0]], bmp_image[i + _plt0.rgba_channel[1]], bmp_image[i + _plt0.rgba_channel[2]]);  // Grey Value
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