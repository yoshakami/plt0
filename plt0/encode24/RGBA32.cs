using System.Collections.Generic;
using System.Linq;

class RGBA32_class24  // 24 edit
{
    Parse_args_class _plt0;
    public RGBA32_class24(Parse_args_class Parse_args_class)  // 24 edit
    {
        _plt0 = Parse_args_class;
    }
    public void RGBA32(List<byte[]> index_list, byte[] bmp_image, byte[] index)
    {
        int j = 0;
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
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 16)
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
                            i += _plt0.bitmap_width % 4;  // 24 edit
                            index_list.Add(index.ToArray());
                        }

                    }
                    break;
                }
            default:
                {
                    for (int i = _plt0.pixel_data_start_offset; i < _plt0.bmp_filesize; i += 16)
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
                            i += _plt0.canvas_width % 4;  // 24 edit
                            index_list.Add(index.ToArray());
                        }
                    }
                    break;
                }
        }
    }
}