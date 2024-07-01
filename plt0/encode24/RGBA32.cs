using System.Collections.Generic;
using System.Linq;
/*
 * this file is completely edited. so no   // 24 edit here, since it's everywhere, too verbose
*/
class RGBA32_class24
{
    Parse_args_class _plt0;
    public RGBA32_class24(Parse_args_class Parse_args_class)
    {
        _plt0 = Parse_args_class;
    }
    public void RGBA32(List<byte[]> index_list, byte[] bmp_image, byte[] index)
    {
        int j = 0;
        int diff = (_plt0.canvas_width - _plt0.bitmap_width) * 3;
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
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 12)
                    {
                        // _plt0.alpha and red
                        index[j] = (byte)(255 * _plt0.custom_rgba[3]);       // A
                        index[j + 1] = (byte)(bmp_image[i + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);   // R
                        index[j + 8] = (byte)(bmp_image[i + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);   // G
                        index[j + 9] = (byte)(bmp_image[i + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);       // B
                        if (i + 5 < _plt0.bmp_filesize)
                        {
                            index[j + 2] = (byte)(255 * _plt0.custom_rgba[3]);   // A
                            index[j + 3] = (byte)(bmp_image[i + 3 + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);   // R
                            index[j + 10] = (byte)(bmp_image[i + 3 + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);  // G
                            index[j + 11] = (byte)(bmp_image[i + 3 + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);  // B
                            if (i + 8 < _plt0.bmp_filesize)
                            {
                                index[j + 4] = (byte)(255 * _plt0.custom_rgba[3]);  // A
                                index[j + 5] = (byte)(bmp_image[i + 6 + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);  // R
                                index[j + 12] = (byte)(bmp_image[i + 6 + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]);  // G
                                index[j + 13] = (byte)(bmp_image[i + 6 + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]);  // B
                                if (i + 11 < _plt0.bmp_filesize)
                                {
                                    index[j + 6] = (byte)(255 * _plt0.custom_rgba[3]);  // A
                                    index[j + 7] = (byte)(bmp_image[i + 9 + _plt0.rgba_channel[0]] * _plt0.custom_rgba[0]);  // R
                                    index[j + 14] = (byte)(bmp_image[i + 9 + _plt0.rgba_channel[1]] * _plt0.custom_rgba[1]); // G
                                    index[j + 15] = (byte)(bmp_image[i + 9 + _plt0.rgba_channel[2]] * _plt0.custom_rgba[2]); // B
                                }
                            }
                        }
                        j += 16;
                        if (j == index.Length)
                        {
                            j = 0;
                            i += _plt0.bitmap_width % 4;
                            i -= diff;
                            index_list.Add(index.ToArray());
                        }

                    }
                    break;
                }
            default:
                {
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 12)
                    {
                        // _plt0.alpha and red
                        index[j] = (byte)(255);       // A
                        index[j + 1] = (byte)(bmp_image[i + _plt0.rgba_channel[0]]);   // R
                        index[j + 8] = (byte)(bmp_image[i + _plt0.rgba_channel[1]]);   // G
                        index[j + 9] = (byte)(bmp_image[i + _plt0.rgba_channel[2]]);       // B
                        if (i + 5 < _plt0.bmp_filesize)
                        {
                            index[j + 2] = (byte)(255);   // A
                            index[j + 3] = (byte)(bmp_image[i + 3 + _plt0.rgba_channel[0]]);   // R
                            index[j + 10] = (byte)(bmp_image[i + 3 + _plt0.rgba_channel[1]]);  // G
                            index[j + 11] = (byte)(bmp_image[i + 3 + _plt0.rgba_channel[2]]);  // B
                            if (i + 8 < _plt0.bmp_filesize)
                            {
                                index[j + 4] = (byte)(255);  // A
                                index[j + 5] = (byte)(bmp_image[i + 6 + _plt0.rgba_channel[0]]);  // R
                                index[j + 12] = (byte)(bmp_image[i + 6 + _plt0.rgba_channel[1]]);  // G
                                index[j + 13] = (byte)(bmp_image[i + 6 + _plt0.rgba_channel[2]]);  // B
                                if (i + 11 < _plt0.bmp_filesize)
                                {
                                    index[j + 6] = (byte)(255);  // A
                                    index[j + 7] = (byte)(bmp_image[i + 9 + _plt0.rgba_channel[0]]);  // R
                                    index[j + 14] = (byte)(bmp_image[i + 9 + _plt0.rgba_channel[1]]); // G
                                    index[j + 15] = (byte)(bmp_image[i + 9 + _plt0.rgba_channel[2]]); // B
                                }
                            }
                        }
                        j += 16;
                        if (j == index.Length)
                        {
                            j = 0;
                            i += _plt0.bitmap_width % 4;
                            i -= diff;
                            index_list.Add(index.ToArray());
                        }
                    }
                    break;
                }
        }
    }
}