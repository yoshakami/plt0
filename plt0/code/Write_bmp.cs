using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

class Write_bmp_class
{
    static public void Write_bmp(List<List<byte[]>> index_list, List<ushort[]> canvas_dim, byte[] colour_palette, byte[] texture_format_int32, byte[] palette_format_int32, ushort colour_number, string output_file, bool bmp_32, bool funky, bool has_palette, bool warn, bool stfu, bool no_warning, bool safe_mode, bool png, bool gif, bool jpeg, bool jpg, bool ico, bool tiff, bool tif, byte mipmaps_number, byte alpha, int colour_number_x2, int colour_number_x4)  // index_list contains all mipmaps.
    {
        byte padding = (byte)(4 - (canvas_dim[0][2] % 4));
        if (padding == 4)
        {
            padding = 0;
        }
        byte alpha_header_size = 0;

        int image_size; // = ((ref_width + padding) * canvas_height);
        int pixel_start_offset; // = 0x36 + colour_number_x4 + alpha_header_size;
        int size; // = pixel_start_offset + image_size;  // fixed size at 1 image
                  // int size2 = pixel_start_offset + image_size;  // plus the header?????? added it twice lol
        int width;  // will change when equal to 4 or 16 bit because of bypass lol
        ushort header_width; // width written in the header
        ushort height;
        int index;
        byte pixel_color;
        byte pixel_alpha;
        byte[] data = new byte[54];  // header data
        byte[] pixel = new byte[0];
        string end = ".bmp";
        bool done = false;
        // vanilla BITMAPV4HEADER
        // byte[] alpha_header = { 00, 00, 0xFF, 00, 00, 0xFF, 00, 00, 0xFF, 00, 00, 00, 00, 00, 00, 0xFF, 0x20, 0x6E, 0x69, 0x57, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 };
        switch (texture_format_int32[3])
        {
            case 0:  // I4
                {
                    colour_number_x4 = 64; // 16 * 4
                    break;
                }
            /* case 8: // CI4
                {
                    width >>= 1;  // 4-bit per pixel
                    break;
                }*/
            case 1: // I8
                {
                    colour_number_x4 = 1024; // 256 * 4
                    break;
                }
            /* 
              case 9:  // CI8
                nothing happens
           
            case 4:  // RGB565
                {
                    width *= 3; // converted to 24bpp to prevent loss
                    break;
                } */
            case 2: // AI4
            case 3:  // IA8
            case 5:  // RGB5A3
            case 6:  // RGBA32
            case 10: // CI14x2
            case 0xE:  // CMPR
                {
                    colour_number_x4 = 0;
                    alpha_header_size = 0x44;
                    break;
                }
        }
        if ((has_palette && alpha > 0) || bmp_32)  // AI8 Palette with alpha // RGB565 Palette with alpha // RGB5A3 Palette with alpha
        {
            colour_number_x4 = 0;
            alpha_header_size = 0x44;
        }
        byte[] palette = new byte[colour_number_x4];
        // custom padding - the funni one  - it is mandatory to add these padding bytes, else paint.net won't open the picture. (it's fine with explorer.exe's preview) - these bytes can be whatever you want. so I've made it write some fun text 
        byte[] alpha_header = { 0, 0, 255, 0, 0, 255, 0, 0, 255, 0, 0, 0, 0, 0, 0, 255, 32, 110, 105, 87, 0, 104, 116, 116, 112, 115, 58, 47, 47, 100, 105, 115, 99, 111, 114, 100, 46, 103, 103, 47, 118, 57, 86, 112, 68, 90, 57, 0, 116, 104, 105, 115, 32, 105, 115, 32, 112, 97, 100, 100, 105, 110, 103, 32, 100, 97, 116, 97 };
        for (int z = 0; z < mipmaps_number + 1; z++)
        {
            width = canvas_dim[z][2];
            header_width = canvas_dim[z][2];
            height = canvas_dim[z][3];
            switch (texture_format_int32[3])
            {
                case 0:  // I4
                    {
                        width >>= 1;  // 4-bit per pixel
                        break;
                    }
                case 8: // CI4
                    {
                        width >>= 1;  // 4-bit per pixel
                        break;
                    }
                /* case 1: // I8
                    {
                        colour_number_x4 = 1024; // 256 * 4
                        break;
                    }
                /* 
                  case 9:  // CI8
                    nothing happens
               */
                case 4:  // RGB565
                    {
                        width *= 3; // converted to 24bpp to prevent loss
                        break;
                    }
                case 2: // AI4
                case 3:  // IA8
                case 5:  // RGB5A3
                case 6:  // RGBA32
                case 10: // CI14x2
                case 0xE:  // CMPR
                    {
                        width <<= 2; // 32 bits per pixel
                        break;
                    }
            }
            if ((has_palette && alpha > 0) || bmp_32)  // AI8 Palette with alpha // RGB565 Palette with alpha // RGB5A3 Palette with alpha
            {
                width = canvas_dim[z][2] << 2; // 32 bits per pixel
            }
            image_size = ((width + padding) * height);
            pixel_start_offset = 0x36 + colour_number_x4 + alpha_header_size;
            size = pixel_start_offset + image_size;  // fixed size at 1 image

            /* that doesn't work for images with dimensions not a power of two
            image_size >>= 2; // for next loop - divides by 4 because it's half the width size and half the height size
            width >>= 1;  // divides by 2
            header_width >>= 1;  // divides by 2
            height >>= 1; // divides by 2 */

            //size = pixel_start_offset + image_size;

            Array.Resize(ref pixel, image_size);

            index = 0;
            // fill header
            data[0] = 0x42;  // B
            data[1] = 0x4D;  // M
            data[2] = (byte)(size);
            data[3] = (byte)(size >> 8);
            data[4] = (byte)(size >> 16);
            data[5] = (byte)(size >> 24);
            data[6] = 0x79; // y
            data[7] = 0x6F; // o
            data[8] = 0x73; // s
            data[9] = 0x68; // h
            data[10] = (byte)(pixel_start_offset);
            data[11] = (byte)(pixel_start_offset >> 8);
            data[12] = (byte)(pixel_start_offset >> 16);
            data[13] = (byte)(pixel_start_offset >> 24);
            data[14] = 0x28;  // DIB header size
            data[15] = 0;
            data[16] = 0;
            data[17] = 0;
            data[18] = (byte)(header_width);
            data[19] = (byte)(header_width >> 8);
            data[20] = 0;  // width can't exceed 65535 because it's stored as ushort in tex0/tpl/bti
            data[21] = 0;  // trust me, you don't want to have an image that is 24GB decompressed, neither in your ram lol
            data[22] = (byte)(height);
            data[23] = (byte)(height >> 8);
            data[24] = 0;
            data[25] = 0; // fourth byte of height
            data[26] = 1; // always 1
            data[27] = 0; // always 0
            switch (texture_format_int32[3])
            {
                case 0:  // I4
                case 8: // CI4
                    {
                        data[28] = 4;  // 4-bit per pixel
                        break;
                    }
                case 1: // I8
                case 9:  // CI8
                    {
                        data[28] = 8;  // 8-bit per pixel
                        break;
                    }
                /* technically RGB5A3 with no alpha would fit as it's XRRR RRGG GGGB BBBB, but lots of softwares doesn't like these
                {
                    data[28] = 16;  // 16-bit per pixel
                    break;
                }
                */

                case 4:  // RGB565
                    {
                        data[28] = 24; // converted to 24bpp to prevent loss
                        break;
                    }
                case 2: // AI4
                case 3:  // IA8
                case 6:  // RGBA32
                case 5:  // RGB5A3
                case 10: // CI14x2
                case 0xE:  // CMPR
                    {
                        data[28] = 32; // 32 bits per pixel
                        break;
                    }
            }
            data[29] = 0; // second bit of depth
            data[30] = 0; // compression
            data[31] = 0; // compression
            data[32] = 0; // compression
            data[33] = 0; // compression
            data[34] = (byte)(image_size);
            data[35] = (byte)(image_size >> 8);
            data[36] = (byte)(image_size >> 16);
            data[37] = (byte)(image_size >> 24);
            data[38] = 0;
            data[39] = 0;
            data[40] = 0;
            data[41] = 0;  // some resolution unused setting
            data[42] = 0;
            data[43] = 0;
            data[44] = 0;
            data[45] = 0;  // some resolution unused setting
            data[46] = 0;
            data[47] = 0;
            data[48] = 0;
            data[49] = 0;
            data[50] = 0;
            data[51] = 0;
            data[52] = 0;
            data[53] = 0;
            if (alpha_header_size != 0)  // 32 bit depth images without that update store an useless alpha byte for each pixel
            {
                // Windows DIB Header BITMAPV4HEADER
                data[30] = 3; // compression set to BI_BITFIELDS - basically it's the update to the bmp file format that supports alpha channel lol
                data[14] = 0x6C;  // DIB header size
            }
            if (has_palette)
            {
                data[46] = (byte)(colour_number);
                data[47] = (byte)(colour_number >> 8); // colour_number is stored on 2 bytes
                data[50] = (byte)(colour_number);
                data[51] = (byte)(colour_number >> 8); // colour_number is stored on 2 bytes
                                                       // fill palette data
                                                       // plt0 palettes are 2 bytes per colour, while bmp palette is 4 bytes per colour.
                switch (palette_format_int32[3])
                {
                    case 0: // AI8
                        {
                            if (alpha == 0 && texture_format_int32[3] != 10 && !bmp_32) // ci14x2 is converted to 32-bit depth because bmp files can't support palettes > 256 colours
                            {
                                for (int j = 0, k = 0; j < colour_number_x2; j += 2, k += 4)
                                {
                                    palette[k] = colour_palette[j + 1];    // this is the formula for black and white lol
                                    palette[k + 1] = colour_palette[j + 1];
                                    palette[k + 2] = colour_palette[j + 1];
                                    palette[k + 3] = colour_palette[j];  // Alpha
                                }
                            }
                            else
                            {
                                data[46] = 0;
                                data[47] = 0;
                                data[48] = 0;
                                data[49] = 0;
                                data[50] = 0;
                                data[51] = 0;
                                data[28] = 32;  // converts it to 32-bit depth, as 8-bit depth bmp files don't have alpha despite STORING A F*CKING ALPHA BYTE FOR. EACH. PIXEL.
                                switch (texture_format_int32[3])
                                {
                                    case 8: // CI4
                                        {
                                            for (int j = 0; j < index_list[z].Count; j++)
                                            {
                                                for (int k = 0; k < index_list[z][0].Length; k++, index += 8)
                                                {
                                                    pixel[index] = colour_palette[((index_list[z][j][k] >> 4) << 1) + 1];
                                                    pixel[index + 1] = colour_palette[((index_list[z][j][k] >> 4) << 1) + 1];
                                                    pixel[index + 2] = colour_palette[((index_list[z][j][k] >> 4) << 1) + 1];
                                                    pixel[index + 3] = colour_palette[((index_list[z][j][k] >> 4) << 1)];
                                                    pixel[index + 4] = colour_palette[((index_list[z][j][k] & 15) << 1) + 1];
                                                    pixel[index + 5] = colour_palette[((index_list[z][j][k] & 15) << 1) + 1];
                                                    pixel[index + 6] = colour_palette[((index_list[z][j][k] & 15) << 1) + 1];
                                                    pixel[index + 7] = colour_palette[((index_list[z][j][k] & 15) << 1)];
                                                }
                                                for (int k = 0; k < padding; k++, index++)
                                                {
                                                    pixel[index] = 0x69;  // my signature XDDDD 
                                                }
                                            }
                                            break;
                                        }
                                    case 9: // CI8
                                        {
                                            for (int j = 0; j < index_list[z].Count; j++)
                                            {
                                                for (int k = 0; k < index_list[z][0].Length; k++, index += 4)
                                                {
                                                    pixel[index] = colour_palette[(index_list[z][j][k] << 1) + 1];
                                                    pixel[index + 1] = colour_palette[(index_list[z][j][k] << 1) + 1];
                                                    pixel[index + 2] = colour_palette[(index_list[z][j][k] << 1) + 1];
                                                    pixel[index + 3] = colour_palette[(index_list[z][j][k] << 1)];
                                                }
                                                for (int k = 0; k < padding; k++, index++)
                                                {
                                                    pixel[index] = 0x69;  // my signature XDDDD 
                                                }
                                            }
                                            break;
                                        }
                                    case 10: // CI14x2
                                        {

                                            for (int j = 0; j < index_list[z].Count; j++)
                                            {
                                                for (int k = 0; k < index_list[z][0].Length; k += 2, index += 4)
                                                {
                                                    pixel[index] = colour_palette[((index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)) + 1];
                                                    pixel[index + 1] = colour_palette[((index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)) + 1];
                                                    pixel[index + 2] = colour_palette[((index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)) + 1];
                                                    pixel[index + 3] = colour_palette[((index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1))];
                                                }
                                                for (int k = 0; k < padding; k++, index++)
                                                {
                                                    pixel[index] = 0x69;  // my signature XDDDD 
                                                }
                                            }
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    case 1: // RGB565
                        {
                            if (alpha == 0 && texture_format_int32[3] != 10 && !bmp_32) // ci14x2 is converted to 32-bit depth because bmp files can't support palettes > 256 colours
                            {
                                for (int j = 0, k = 0; j < colour_number_x2; j += 2, k += 4)
                                {
                                    palette[k] = (byte)(colour_palette[j + 1] << 3);  // Blue
                                    if (palette[k] == 248)
                                    {
                                        palette[k] = 255;
                                    }
                                    palette[k + 1] = (byte)(((colour_palette[j] << 5) | (colour_palette[j + 1] >> 3)) & 0xfc);  // Green
                                    if (palette[k + 1] == 252)
                                    {
                                        palette[k + 1] = 255;
                                    }
                                    palette[k + 2] = (byte)(colour_palette[j] & 0xf8);  // Red
                                    if (palette[k + 2] == 248)
                                    {
                                        palette[k + 2] = 255;
                                    }
                                    palette[k + 3] = 0xff;  // No Alpha
                                }
                            }
                            else
                            {
                                data[46] = 0;
                                data[47] = 0;
                                data[48] = 0;
                                data[49] = 0;
                                data[50] = 0;
                                data[51] = 0;
                                data[28] = 32;  // converts it to 32-bit depth
                                switch (texture_format_int32[3])
                                {
                                    case 8: // CI4
                                        {
                                            for (int j = 0; j < index_list[z].Count; j++)
                                            {
                                                for (int k = 0; k < index_list[z][0].Length; k++, index += 8)
                                                {
                                                    pixel[index] = (byte)(colour_palette[((index_list[z][j][k] >> 4) << 1) + 1] << 3);  // Blue
                                                    if (pixel[index] == 248)
                                                    {
                                                        pixel[index] = 255;
                                                    }
                                                    pixel[index + 1] = (byte)(((colour_palette[(index_list[z][j][k] >> 4) << 1] << 5) | (colour_palette[((index_list[z][j][k] >> 4) << 1) + 1] >> 3)) & 0xfc);  // Green
                                                    if (pixel[index + 1] == 252)
                                                    {
                                                        pixel[index + 1] = 255;
                                                    }
                                                    pixel[index + 2] = (byte)(colour_palette[(index_list[z][j][k] >> 4) << 1] & 0xf8);  // Red
                                                    if (pixel[index + 2] == 248)
                                                    {
                                                        pixel[index + 2] = 255;
                                                    }
                                                    pixel[index + 3] = 0xff;  // No Alpha

                                                    pixel[index + 4] = (byte)(colour_palette[((index_list[z][j][k] >> 4) << 1) + 1] << 3);  // Blue
                                                    if (pixel[index + 4] == 248)
                                                    {
                                                        pixel[index + 4] = 255;
                                                    }
                                                    pixel[index + 5] = (byte)(((colour_palette[(index_list[z][j][k] >> 4) << 1] << 5) | (colour_palette[((index_list[z][j][k] >> 4) << 1) + 1] >> 3)) & 0xfc);  // Green
                                                    if (pixel[index + 5] == 252)
                                                    {
                                                        pixel[index + 5] = 255;
                                                    }
                                                    pixel[index + 6] = (byte)(colour_palette[(index_list[z][j][k] >> 4) << 1] & 0xf8);  // Red
                                                    if (pixel[index + 6] == 248)
                                                    {
                                                        pixel[index + 6] = 255;
                                                    }
                                                    pixel[index + 7] = 0xff;  // No Alpha
                                                }
                                                for (int k = 0; k < padding; k++, index++)
                                                {
                                                    pixel[index] = 0x69;  // my signature XDDDD 
                                                }
                                            }
                                            break;
                                        }
                                    case 9: // CI8
                                        {
                                            for (int j = 0; j < index_list[z].Count; j++)
                                            {
                                                for (int k = 0; k < index_list[z][0].Length; k++, index += 4)
                                                {
                                                    pixel[index] = (byte)(colour_palette[(index_list[z][j][k] << 1) + 1] << 3);  // Blue
                                                    if (pixel[index] == 248)
                                                    {
                                                        pixel[index] = 255;
                                                    }
                                                    pixel[index + 1] = (byte)(((colour_palette[index_list[z][j][k] << 1] << 5) | (colour_palette[(index_list[z][j][k] << 1) + 1] >> 3)) & 0xfc);  // Green
                                                    if (pixel[index + 1] == 252)
                                                    {
                                                        pixel[index + 1] = 255;
                                                    }
                                                    pixel[index + 2] = (byte)(colour_palette[index_list[z][j][k] << 1] & 0xf8);  // Red
                                                    if (pixel[index + 2] == 248)
                                                    {
                                                        pixel[index + 2] = 255;
                                                    }
                                                    pixel[index + 3] = 0xff;  // No Alpha

                                                }
                                                for (int k = 0; k < padding; k++, index++)
                                                {
                                                    pixel[index] = 0x69;  // my signature XDDDD 
                                                }
                                            }
                                            break;
                                        }
                                    case 10: // CI14x2
                                        {

                                            for (int j = 0; j < index_list[z].Count; j++)
                                            {
                                                for (int k = 0; k < index_list[z][0].Length; k += 2, index += 4)
                                                {
                                                    pixel[index] = (byte)(colour_palette[(index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1) + 1] << 3);  // Blue
                                                    if (pixel[index] == 248)
                                                    {
                                                        pixel[index] = 255;
                                                    }
                                                    pixel[index + 1] = (byte)(((colour_palette[(index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)] << 5) | (colour_palette[(index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1) + 1] >> 3)) & 0xfc);  // Green
                                                    if (pixel[index + 1] == 252)
                                                    {
                                                        pixel[index + 1] = 255;
                                                    }
                                                    pixel[index + 2] = (byte)(colour_palette[(index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)] & 0xf8);  // Red
                                                    if (pixel[index + 2] == 248)
                                                    {
                                                        pixel[index + 2] = 255;
                                                    }
                                                    pixel[index + 3] = 0xff;  // No Alpha

                                                }
                                                for (int k = 0; k < padding; k++, index++)
                                                {
                                                    pixel[index] = 0x69;  // my signature XDDDD 
                                                }
                                            }
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    case 2: // RGB5A3
                        {
                            if (alpha == 0 && texture_format_int32[3] != 10 && !bmp_32) // ci14x2 is converted to 32-bit depth because bmp files can't support palettes > 256 colours
                            {
                                for (int j = 0, k = 0; j < colour_number_x2; j += 2, k += 4)
                                {  // 1RRR RRGG   GGGB BBBB
                                    palette[k] = (byte)(colour_palette[j + 1] << 3);  // Blue
                                    if (palette[k] == 248)
                                    {
                                        palette[k] = 255;
                                    }
                                    palette[k + 1] = (byte)(((colour_palette[j] << 6) | (colour_palette[j + 1] >> 2)) & 0xf8);  // Green
                                    if (palette[k + 1] == 248)
                                    {
                                        palette[k + 1] = 255;
                                    }
                                    palette[k + 2] = (byte)((colour_palette[j] << 1) & 0xf8);  // Red
                                    if (palette[k + 2] == 248)
                                    {
                                        palette[k + 2] = 255;
                                    }
                                    palette[k + 3] = 0xff;  // No Alpha
                                }
                            }
                            else if (alpha == 1)  // alpha
                            {
                                data[46] = 0;
                                data[47] = 0;
                                data[48] = 0;
                                data[49] = 0;
                                data[50] = 0;
                                data[51] = 0;
                                data[28] = 32;  // converts it to 32-bit depth, as 8-bit depth bmp files don't have alpha despite STORING A F*CKING ALPHA BYTE FOR. EACH. PIXEL.


                                switch (texture_format_int32[3])
                                {
                                    case 8: // CI4
                                        {
                                            for (int j = 0; j < index_list[z].Count; j++)
                                            {
                                                for (int k = 0; k < index_list[z][0].Length; k++, index += 8)
                                                {
                                                    pixel_alpha = (byte)((colour_palette[(index_list[z][j][k] >> 4) << 1] << 1) & 0xe0);
                                                    if (pixel_alpha == 0xe0)
                                                    {
                                                        pixel_alpha = 0xff;
                                                    }
                                                    pixel[index + 3] = pixel_alpha;  // alpha
                                                    pixel[index + 2] = (byte)((colour_palette[(index_list[z][j][k] >> 4) << 1] & 0x0f) << 4);  // red
                                                    if (pixel[index + 2] == 0xf0)
                                                    {
                                                        pixel[index + 2] = 0xff;
                                                    }
                                                    pixel[index + 1] = (byte)(colour_palette[((index_list[z][j][k] >> 4) << 1) + 1] & 0xf0);  // green
                                                    if (pixel[index + 1] == 0xf0)
                                                    {
                                                        pixel[index + 1] = 0xff;
                                                    }
                                                    pixel[index] = (byte)((colour_palette[((index_list[z][j][k] >> 4) << 1) + 1] & 0x0f) << 4);  // blue
                                                    if (pixel[index] == 0xf0)
                                                    {
                                                        pixel[index] = 0xff;
                                                    }
                                                    pixel_alpha = (byte)((colour_palette[(index_list[z][j][k] >> 4) << 1] << 1) & 0xe0);
                                                    if (pixel_alpha == 0xe0)
                                                    {
                                                        pixel_alpha = 0xff;
                                                    }

                                                    // adds the second part of the byte
                                                    pixel[index + 7] = pixel_alpha;  // alpha
                                                    pixel[index + 6] = (byte)((colour_palette[(index_list[z][j][k] & 15) << 1] & 0x0f) << 4);  // red
                                                    if (pixel[index + 6] == 0xf0)
                                                    {
                                                        pixel[index + 6] = 0xff;
                                                    }
                                                    pixel[index + 5] = (byte)(colour_palette[((index_list[z][j][k] & 15) << 1) + 1] & 0xf0);  // green
                                                    if (pixel[index + 5] == 0xf0)
                                                    {
                                                        pixel[index + 5] = 0xff;
                                                    }
                                                    pixel[index + 4] = (byte)((colour_palette[((index_list[z][j][k] & 15) << 1) + 1] & 0x0f) << 4);  // blue
                                                    if (pixel[index + 4] == 0xf0)
                                                    {
                                                        pixel[index + 4] = 0xff;
                                                    }

                                                }
                                                for (int k = 0; k < padding; k++, index++)
                                                {
                                                    pixel[index] = 0x69;  // my signature XDDDD 
                                                }
                                            }
                                            break;
                                        }
                                    case 9: // CI8
                                        {
                                            for (int j = 0; j < index_list[z].Count; j++)
                                            {
                                                for (int k = 0; k < index_list[z][0].Length; k++, index += 4)
                                                {
                                                    pixel_alpha = (byte)((colour_palette[index_list[z][j][k] << 1] << 1) & 0xe0);
                                                    if (pixel_alpha == 0xe0)
                                                    {
                                                        pixel_alpha = 0xff;
                                                    }
                                                    pixel[index + 3] = pixel_alpha;  // alpha
                                                    pixel[index + 2] = (byte)((colour_palette[index_list[z][j][k] << 1] & 0x0f) << 4);  // red
                                                    if (pixel[index + 2] == 0xf0)
                                                    {
                                                        pixel[index + 2] = 0xff;
                                                    }
                                                    pixel[index + 1] = (byte)(colour_palette[(index_list[z][j][k] << 1) + 1] & 0xf0);  // green
                                                    if (pixel[index + 1] == 0xf0)
                                                    {
                                                        pixel[index + 1] = 0xff;
                                                    }
                                                    pixel[index] = (byte)((colour_palette[(index_list[z][j][k] << 1) + 1] & 0x0f) << 4);  // blue
                                                    if (pixel[index] == 0xf0)
                                                    {
                                                        pixel[index] = 0xff;
                                                    }


                                                }
                                                for (int k = 0; k < padding; k++, index++)
                                                {
                                                    pixel[index] = 0x69;  // my signature XDDDD 
                                                }
                                            }
                                            break;
                                        }
                                    case 10: // CI14x2
                                        {

                                            for (int j = 0; j < index_list[z].Count; j++)
                                            {
                                                for (int k = 0; k < index_list[z][0].Length; k += 2, index += 4)
                                                {
                                                    pixel_alpha = (byte)((colour_palette[(index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)] << 1) & 0xe0);
                                                    if (pixel_alpha == 0xe0)
                                                    {
                                                        pixel_alpha = 0xff;
                                                    }
                                                    pixel[index + 3] = pixel_alpha;  // alpha
                                                    pixel[index + 2] = (byte)((colour_palette[(index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)] & 0x0f) << 4);  // red
                                                    if (pixel[index + 2] == 0xf0)
                                                    {
                                                        pixel[index + 2] = 0xff;
                                                    }
                                                    pixel[index + 1] = (byte)(colour_palette[((index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)) + 1] & 0xf0);  // green
                                                    if (pixel[index + 1] == 0xf0)
                                                    {
                                                        pixel[index + 1] = 0xff;
                                                    }
                                                    pixel[index] = (byte)((colour_palette[((index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)) + 1] & 0x0f) << 4);  // blue
                                                    if (pixel[index] == 0xf0)
                                                    {
                                                        pixel[index] = 0xff;
                                                    }


                                                }
                                                for (int k = 0; k < padding; k++, index++)
                                                {
                                                    pixel[index] = 0x69;  // my signature XDDDD 
                                                }
                                            }
                                            break;
                                        }
                                }
                            }
                            else  // mix
                            {
                                data[46] = 0;
                                data[47] = 0;
                                data[48] = 0;
                                data[49] = 0;
                                data[50] = 0;
                                data[51] = 0;
                                data[28] = 32;  // converts it to 32-bit depth, as it's either rgb555 or rgba4443
                                switch (texture_format_int32[3])
                                {
                                    case 8: // CI4
                                        {
                                            for (int j = 0; j < index_list[z].Count; j++)
                                            {
                                                for (int k = 0; k < index_list[z][0].Length; k++, index += 8)
                                                {
                                                    if (colour_palette[(index_list[z][j][k] >> 4) << 1] >> 7 == 0)  // alpha - 
                                                    {
                                                        pixel_alpha = (byte)((colour_palette[(index_list[z][j][k] >> 4) << 1] << 1) & 0xe0);
                                                        if (pixel_alpha == 0xe0)
                                                        {
                                                            pixel_alpha = 0xff;
                                                        }
                                                        pixel[index + 3] = pixel_alpha;  // alpha
                                                        pixel[index + 2] = (byte)((colour_palette[(index_list[z][j][k] >> 4) << 1] & 0x0f) << 4);  // red
                                                        if (pixel[index + 2] == 0xf0)
                                                        {
                                                            pixel[index + 2] = 0xff;
                                                        }
                                                        pixel[index + 1] = (byte)(colour_palette[((index_list[z][j][k] >> 4) << 1) + 1] & 0xf0);  // green
                                                        if (pixel[index + 1] == 0xf0)
                                                        {
                                                            pixel[index + 1] = 0xff;
                                                        }
                                                        pixel[index] = (byte)((colour_palette[((index_list[z][j][k] >> 4) << 1) + 1] & 0x0f) << 4);  // blue
                                                        if (pixel[index] == 0xf0)
                                                        {
                                                            pixel[index] = 0xff;
                                                        }
                                                        pixel_alpha = (byte)((colour_palette[(index_list[z][j][k] >> 4) << 1] << 1) & 0xe0);
                                                        if (pixel_alpha == 0xe0)
                                                        {
                                                            pixel_alpha = 0xff;
                                                        }

                                                        // adds the second part of the byte
                                                        pixel[index + 7] = pixel_alpha;  // alpha
                                                        pixel[index + 6] = (byte)((colour_palette[(index_list[z][j][k] & 15) << 1] & 0x0f) << 4);  // red
                                                        if (pixel[index + 6] == 0xf0)
                                                        {
                                                            pixel[index + 6] = 0xff;
                                                        }
                                                        pixel[index + 5] = (byte)(colour_palette[((index_list[z][j][k] & 15) << 1) + 1] & 0xf0);  // green
                                                        if (pixel[index + 5] == 0xf0)
                                                        {
                                                            pixel[index + 5] = 0xff;
                                                        }
                                                        pixel[index + 4] = (byte)((colour_palette[((index_list[z][j][k] & 15) << 1) + 1] & 0x0f) << 4);  // blue
                                                        if (pixel[index + 4] == 0xf0)
                                                        {
                                                            pixel[index + 4] = 0xff;
                                                        }
                                                    }
                                                    else  // reads 0RRR RRGG GGGB BBBB
                                                    {
                                                        pixel[index + 2] = (byte)((colour_palette[(index_list[z][j][k] >> 4) << 1] << 1) & 248);  // red
                                                        if (pixel[index + 2] == 248)
                                                        {
                                                            pixel[index + 2] = 255;
                                                        }
                                                        pixel[index + 1] = (byte)((colour_palette[(index_list[z][j][k] >> 4) << 1] << 6) + (colour_palette[((index_list[z][j][k] >> 4) << 1) + 1] >> 2));  // green
                                                        if (pixel[index + 1] == 248)
                                                        {
                                                            pixel[index + 1] = 0xff;
                                                        }
                                                        pixel[index] = (byte)(colour_palette[((index_list[z][j][k] >> 4) << 1) + 1] << 3);  // blue
                                                        if (pixel[index] == 248)
                                                        {
                                                            pixel[index] = 0xff;
                                                        }
                                                        pixel[index + 3] = 0xff; // no alpha

                                                        //adds the second part of the byte
                                                        pixel[index + 6] = (byte)((colour_palette[(index_list[z][j][k] & 15) << 1] << 1) & 248);  // red
                                                        if (pixel[index + 6] == 248)
                                                        {
                                                            pixel[index + 6] = 255;
                                                        }
                                                        pixel[index + 5] = (byte)((colour_palette[(index_list[z][j][k] & 15) << 1] << 6) + (colour_palette[((index_list[z][j][k] & 15) << 1) + 1] >> 2));  // green
                                                        if (pixel[index + 5] == 248)
                                                        {
                                                            pixel[index + 5] = 0xff;
                                                        }
                                                        pixel[index + 4] = (byte)(colour_palette[((index_list[z][j][k] & 15) << 1) + 1] << 3);  // blue
                                                        if (pixel[index + 4] == 248)
                                                        {
                                                            pixel[index + 4] = 0xff;
                                                        }
                                                        pixel[index + 7] = 0xff; // no alpha
                                                    }

                                                }
                                                for (int k = 0; k < padding; k++, index++)
                                                {
                                                    pixel[index] = 0x69;  // my signature XDDDD 
                                                }
                                            }
                                            break;
                                        }
                                    case 9: // CI8
                                        {
                                            for (int j = 0; j < index_list[z].Count; j++)
                                            {
                                                for (int k = 0; k < index_list[z][0].Length; k++, index += 4)
                                                {
                                                    if (colour_palette[index_list[z][j][k] << 1] >> 7 == 0)  // alpha - 
                                                    {
                                                        pixel_alpha = (byte)((colour_palette[index_list[z][j][k] << 1] << 1) & 0xe0);
                                                        if (pixel_alpha == 0xe0)
                                                        {
                                                            pixel_alpha = 0xff;
                                                        }
                                                        pixel[index + 3] = pixel_alpha;  // alpha
                                                        pixel[index + 2] = (byte)((colour_palette[index_list[z][j][k] << 1] & 0x0f) << 4);  // red
                                                        if (pixel[index + 2] == 0xf0)
                                                        {
                                                            pixel[index + 2] = 0xff;
                                                        }
                                                        pixel[index + 1] = (byte)(colour_palette[(index_list[z][j][k] << 1) + 1] & 0xf0);  // green
                                                        if (pixel[index + 1] == 0xf0)
                                                        {
                                                            pixel[index + 1] = 0xff;
                                                        }
                                                        pixel[index] = (byte)((colour_palette[(index_list[z][j][k] << 1) + 1] & 0x0f) << 4);  // blue
                                                        if (pixel[index] == 0xf0)
                                                        {
                                                            pixel[index] = 0xff;
                                                        }
                                                    }
                                                    else  // reads 0RRR RRGG GGGB BBBB
                                                    {
                                                        pixel[index + 2] = (byte)((colour_palette[index_list[z][j][k] << 1] << 1) & 248);  // red
                                                        if (pixel[index + 2] == 248)
                                                        {
                                                            pixel[index + 2] = 255;
                                                        }
                                                        pixel[index + 1] = (byte)((colour_palette[index_list[z][j][k] << 1] << 6) + (colour_palette[(index_list[z][j][k] << 1) + 1] >> 2));  // green
                                                        if (pixel[index + 1] == 248)
                                                        {
                                                            pixel[index + 1] = 0xff;
                                                        }
                                                        pixel[index] = (byte)(colour_palette[(index_list[z][j][k] << 1) + 1] << 3);  // blue
                                                        if (pixel[index] == 248)
                                                        {
                                                            pixel[index] = 0xff;
                                                        }
                                                        pixel[index + 3] = 0xff; // no alpha
                                                    }

                                                }
                                                for (int k = 0; k < padding; k++, index++)
                                                {
                                                    pixel[index] = 0x69;  // my signature XDDDD 
                                                }
                                            }
                                            break;
                                        }
                                    case 10: // CI14x2
                                        {

                                            for (int j = 0; j < index_list[z].Count; j++)
                                            {
                                                for (int k = 0; k < index_list[z][0].Length; k += 2, index += 4)
                                                {
                                                    if (colour_palette[(index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)] >> 7 == 0)  // alpha - 
                                                    {
                                                        pixel_alpha = (byte)((colour_palette[(index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)] << 1) & 0xe0);
                                                        if (pixel_alpha == 0xe0)
                                                        {
                                                            pixel_alpha = 0xff;
                                                        }
                                                        pixel[index + 3] = pixel_alpha;  // alpha
                                                        pixel[index + 2] = (byte)((colour_palette[(index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)] & 0x0f) << 4);  // red
                                                        if (pixel[index + 2] == 0xf0)
                                                        {
                                                            pixel[index + 2] = 0xff;
                                                        }
                                                        pixel[index + 1] = (byte)(colour_palette[((index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)) + 1] & 0xf0);  // green
                                                        if (pixel[index + 1] == 0xf0)
                                                        {
                                                            pixel[index + 1] = 0xff;
                                                        }
                                                        pixel[index] = (byte)((colour_palette[((index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)) + 1] & 0x0f) << 4);  // blue
                                                        if (pixel[index] == 0xf0)
                                                        {
                                                            pixel[index] = 0xff;
                                                        }
                                                    }
                                                    else  // reads 0RRR RRGG GGGB BBBB
                                                    {
                                                        pixel[index + 2] = (byte)((colour_palette[(index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)] << 1) & 248);  // red
                                                        if (pixel[index + 2] == 248)
                                                        {
                                                            pixel[index + 2] = 255;
                                                        }
                                                        pixel[index + 1] = (byte)((colour_palette[(index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)] << 6) + (colour_palette[((index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)) + 1] >> 2));  // green
                                                        if (pixel[index + 1] == 248)
                                                        {
                                                            pixel[index + 1] = 0xff;
                                                        }
                                                        pixel[index] = (byte)(colour_palette[((index_list[z][j][k] << 9) + (index_list[z][j][k + 1] << 1)) + 1] << 3);  // blue
                                                        if (pixel[index] == 248)
                                                        {
                                                            pixel[index] = 0xff;
                                                        }
                                                        pixel[index + 3] = 0xff; // no alpha
                                                    }

                                                }
                                                for (int k = 0; k < padding; k++, index++)
                                                {
                                                    pixel[index] = 0x69;  // my signature XDDDD 
                                                }
                                            }
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                }
            }
            if ((has_palette || funky) && texture_format_int32[3] != 0 && texture_format_int32[3] != 10 && !bmp_32 && alpha < 1) // && (texture_format_int32[3] > 2 || texture_format_int32[3] != 0xe)))
            {
                if (texture_format_int32[3] == 3 || texture_format_int32[3] == 4 || texture_format_int32[3] == 5)
                {
                    data[28] = 16;  // 16-bit per pixel
                }
                // fill pixel data
                for (int j = 0; j < index_list[z].Count; j++)
                {
                    for (int k = 0; k < index_list[z][0].Length; k++, index++)
                    {
                        pixel[index] = index_list[z][j][k];
                    }
                    for (int k = 0; k < padding; k++, index++)
                    {
                        pixel[index] = 0x69;  // my signature XDDDD 
                    }
                }
            }
            // fill pixel data
            else
            {
                switch (texture_format_int32[3])
                {
                    case 0:  // reverse I4
                        {
                            if (bmp_32)  // 32-bit
                            {
                                for (int j = 0; j < index_list[z].Count; j++)
                                {
                                    for (int k = 0; k < index_list[z][0].Length; k++, index += 8)
                                    {
                                        pixel_color = (byte)(index_list[z][j][k] & 240);
                                        if (pixel_color == 240)
                                        {
                                            pixel_color = 255;
                                        }
                                        pixel[index] = pixel_color;
                                        pixel[index + 1] = pixel_color;
                                        pixel[index + 2] = pixel_color;
                                        pixel[index + 3] = 0xff;
                                        pixel_color = (byte)((index_list[z][j][k] & 15) << 4);
                                        if (pixel_color == 240)
                                        {
                                            pixel_color = 255;
                                        }
                                        pixel[index + 4] = pixel_color;
                                        pixel[index + 5] = pixel_color;
                                        pixel[index + 6] = pixel_color;
                                        pixel[index + 7] = 0xff;
                                    }
                                }
                            }
                            else  // 4-bit
                            {
                                // fill palette data
                                if (funky)
                                {
                                    Random rnd = new Random();
                                    rnd.NextBytes(palette);
                                }
                                else
                                {
                                    for (byte j = 0, k = 0; j < 16; k += 4, j++)  // builds EVERY POSSIBLE I4 COLOUR
                                    {
                                        palette[k] = (byte)(j << 4);  // Blue
                                        palette[k + 1] = (byte)(j << 4);  // Green
                                        palette[k + 2] = (byte)(j << 4);  // Red
                                        palette[k + 3] = 0xff;  // Alpha - unused
                                        if (j == 15)
                                        {
                                            palette[k] = 0xff;  // Blue
                                            palette[k + 1] = 0xff;  // Green
                                            palette[k + 2] = 0xff;  // Red
                                            palette[k + 3] = 0xff;  // Alpha - unused
                                        }
                                    }
                                }

                                // fill pixel data
                                for (int j = 0; j < index_list[z].Count; j++)
                                {
                                    for (int k = 0; k < index_list[z][0].Length; k++, index++)
                                    {
                                        pixel[index] = index_list[z][j][k];
                                    }
                                    for (int k = 0; k < padding; k++, index++)
                                    {
                                        pixel[index] = 0x69;  // my signature XDDDD 
                                    }
                                }
                            }

                            break;
                        }
                    case 1:  // reverse I8
                        {
                            if (bmp_32) // 32-bit
                            {
                                for (int j = 0; j < index_list[z].Count; j++)
                                {
                                    for (int k = 0; k < index_list[z][0].Length; k++, index += 4)
                                    {
                                        pixel_color = index_list[z][j][k];
                                        if (pixel_color == 240)
                                        {
                                            pixel_color = 255;
                                        }
                                        pixel[index] = pixel_color;
                                        pixel[index + 1] = pixel_color;
                                        pixel[index + 2] = pixel_color;
                                        pixel[index + 3] = 0xff;
                                    }
                                }
                            }
                            else  // 8-bit
                            {
                                // fill palette data
                                for (int j = 0, k = 0; j < 256; k += 4, j++)  // builds EVERY POSSIBLE I4 COLOUR
                                {
                                    palette[k] = (byte)j;  // Blue
                                    palette[k + 1] = (byte)j;  // Green
                                    palette[k + 2] = (byte)j;  // Red
                                    palette[k + 3] = 0xff;  // Alpha - unused
                                }
                                // fill pixel data
                                for (int j = 0; j < index_list[z].Count; j++)
                                {
                                    for (int k = 0; k < index_list[z][0].Length; k++, index++)
                                    {
                                        pixel[index] = index_list[z][j][k];
                                    }
                                    for (int k = 0; k < padding; k++, index++)
                                    {
                                        pixel[index] = 0x69;  // my signature XDDDD 
                                    }
                                }
                            }
                            break;
                        }
                    case 2:  // reverse AI4
                        {
                            for (int j = 0; j < index_list[z].Count; j++)
                            {
                                for (int k = 0; k < index_list[z][0].Length; k++, index += 4)
                                {
                                    pixel_alpha = (byte)(index_list[z][j][k] & 240);
                                    if (pixel_alpha == 240)
                                    {
                                        pixel_alpha = 255;
                                    }
                                    pixel_color = (byte)((index_list[z][j][k] & 15) << 4);
                                    if (pixel_color == 240)
                                    {
                                        pixel_color = 255;
                                    }
                                    pixel[index] = pixel_color;
                                    pixel[index + 1] = pixel_color;
                                    pixel[index + 2] = pixel_color;
                                    pixel[index + 3] = pixel_alpha;
                                }
                            }
                            break;
                        }
                    case 3:  // reverse AI8
                        {
                            for (int j = 0; j < index_list[z].Count; j++)
                            {
                                for (int k = 0; k < index_list[z][0].Length; k += 2, index += 4)
                                {
                                    pixel[index] = index_list[z][j][k + 1];
                                    pixel[index + 1] = index_list[z][j][k + 1];
                                    pixel[index + 2] = index_list[z][j][k + 1];
                                    pixel[index + 3] = index_list[z][j][k];
                                }
                            }
                            break;
                        }
                    case 4:  // reverse RGB565
                        {
                            for (int j = 0; j < index_list[z].Count; j++)
                            {
                                for (int k = 0; k < index_list[z][0].Length; k += 2, index += 3)
                                {

                                    pixel[index] = (byte)(index_list[z][j][k + 1] << 3); // blue
                                    if (pixel[index] == 248)
                                    {
                                        pixel[index] = 255;
                                    }
                                    pixel[index + 1] = (byte)(((index_list[z][j][k] << 5) | (index_list[z][j][k + 1] >> 3)) & 0xfc);  // green
                                    if (pixel[index + 1] == 248)
                                    {
                                        pixel[index + 1] = 255;
                                    }
                                    pixel[index + 2] = (byte)(index_list[z][j][k] & 0xf8);  // red
                                    if (pixel[index + 2] == 248)
                                    {
                                        pixel[index + 2] = 255;
                                    }
                                }
                                for (int k = 0; k < padding; k++, index++)
                                {
                                    pixel[index] = 0x69;  // my signature XDDDD 
                                }
                            }
                            break;
                        }
                    case 5:  // reverse RGB5A3
                        {
                            if (alpha == 0)  // no alpha
                            {
                                for (int j = 0; j < index_list[z].Count; j++)
                                {
                                    for (int k = 0; k < index_list[z][0].Length; k += 2, index += 4)
                                    {
                                        pixel[index + 2] = (byte)((index_list[z][j][k] << 1) & 248);  // red
                                        if (pixel[index + 2] == 248)
                                        {
                                            pixel[index + 2] = 255;
                                        }
                                        pixel[index + 1] = (byte)((index_list[z][j][k] << 6) + (index_list[z][j][k + 1] >> 2));  // green
                                        if (pixel[index + 1] == 248)
                                        {
                                            pixel[index + 1] = 0xff;
                                        }
                                        pixel[index] = (byte)(index_list[z][j][k + 1] << 3);  // blue
                                        if (pixel[index] == 248)
                                        {
                                            pixel[index] = 0xff;
                                        }
                                        pixel[index + 3] = 0xff; // no alpha
                                    }
                                }
                            }
                            else if (alpha == 1)  // alpha - 0AAA RRRR GGGG BBBB 
                            {
                                for (int j = 0; j < index_list[z].Count; j++)
                                {
                                    for (int k = 0; k < index_list[z][0].Length; k+=2, index += 4)
                                    {
                                        pixel_alpha = (byte)((index_list[z][j][k] << 1) & 0xe0);
                                        if (pixel_alpha == 0xe0)
                                        {
                                            pixel_alpha = 0xff;
                                        }
                                        pixel[index + 3] = pixel_alpha;  // alpha
                                        pixel[index + 2] = (byte)((index_list[z][j][k] & 0x0f) << 4);  // red
                                        if (pixel[index + 2] == 0xf0)
                                        {
                                            pixel[index + 2] = 0xff;
                                        }
                                        pixel[index + 1] = (byte)(index_list[z][j][k + 1] & 0xf0);  // green
                                        if (pixel[index + 1] == 0xf0)
                                        {
                                            pixel[index + 1] = 0xff;
                                        }
                                        pixel[index] = (byte)((index_list[z][j][k + 1] & 0x0f) << 4);  // blue
                                        if (pixel[index] == 0xf0)
                                        {
                                            pixel[index] = 0xff;
                                        }
                                    }
                                }
                            }
                            else  // mix
                            {
                                for (int j = 0; j < index_list[z].Count; j++)
                                {
                                    for (int k = 0; k < index_list[z][0].Length; k += 2, index += 4)
                                    {
                                        if (index_list[z][j][k] >> 7 == 0)  // alpha - 0AAA RRRR GGGG BBBB
                                        {
                                            pixel_alpha = (byte)((index_list[z][j][k] << 1) & 0xe0);
                                            if (pixel_alpha == 0xe0)
                                            {
                                                pixel_alpha = 0xff;
                                            }
                                            pixel[index + 3] = pixel_alpha;  // alpha
                                            pixel[index + 2] = (byte)((index_list[z][j][k] & 0x0f) << 4);  // red
                                            if (pixel[index + 2] == 0xf0)
                                            {
                                                pixel[index + 2] = 0xff;
                                            }
                                            pixel[index + 1] = (byte)(index_list[z][j][k + 1] & 0xf0);  // green
                                            if (pixel[index + 1] == 0xf0)
                                            {
                                                pixel[index + 1] = 0xff;
                                            }
                                            pixel[index] = (byte)((index_list[z][j][k + 1] & 0x0f) << 4);  // blue
                                            if (pixel[index] == 0xf0)
                                            {
                                                pixel[index] = 0xff;
                                            }
                                        }
                                        else  // reads 1RRR RRGG GGGB BBBB
                                        {
                                            pixel[index + 2] = (byte)((index_list[z][j][k] << 1) & 248);  // red
                                            if (pixel[index + 2] == 248)
                                            {
                                                pixel[index + 2] = 255;
                                            }
                                            pixel[index + 1] = (byte)((index_list[z][j][k] << 6) + (index_list[z][j][k + 1] >> 2));  // green
                                            if (pixel[index + 1] == 248)
                                            {
                                                pixel[index + 1] = 0xff;
                                            }
                                            pixel[index] = (byte)(index_list[z][j][k + 1] << 3);  // blue
                                            if (pixel[index] == 248)
                                            {
                                                pixel[index] = 0xff;
                                            }
                                            pixel[index + 3] = 0xff; // no alpha
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case 6:  // reverse RGBA8
                        {  // remember I encoded each group of 16 bytes ARAR ARAR GBGB GBGB
                            for (int j = 0; j < index_list[z].Count; j++)
                            {
                                for (int k = 0; k < index_list[z][0].Length; k += 16, index += 16)
                                {
                                    pixel[index] = index_list[z][j][k + 9];  // blue
                                    pixel[index + 1] = index_list[z][j][k + 8];  // green
                                    pixel[index + 2] = index_list[z][j][k + 1];  // red 1
                                    pixel[index + 3] = index_list[z][j][k];  // alpha
                                    pixel[index + 4] = index_list[z][j][k + 11];  // blue
                                    pixel[index + 5] = index_list[z][j][k + 10];  // green
                                    pixel[index + 6] = index_list[z][j][k + 3];  // red 2
                                    pixel[index + 7] = index_list[z][j][k + 2];  // alpha
                                    pixel[index + 8] = index_list[z][j][k + 13];  // blue
                                    pixel[index + 9] = index_list[z][j][k + 12];  // green
                                    pixel[index + 10] = index_list[z][j][k + 5];  // red 3
                                    pixel[index + 11] = index_list[z][j][k + 4];  // alpha
                                    pixel[index + 12] = index_list[z][j][k + 15];  // blue
                                    pixel[index + 13] = index_list[z][j][k + 14];  // green
                                    pixel[index + 14] = index_list[z][j][k + 7];  // red 4
                                    pixel[index + 15] = index_list[z][j][k + 6];  // alpha
                                }
                            }
                            break;
                        }
                    case 0xe:  // reverse CMPR
                        {
                            // annoying af
                            List<byte[]> cmpr_palette = new List<byte[]>();
                            byte[] color_rgba = { 0, 0, 0, 0 };
                            ushort color1;
                            ushort color2;
                            index = width - 16;
                            byte block = 0;  // also known as x in the encoding code part
                            ushort x = 0;  // also known as width, but here it's the ref_width of the main image
                            for (int j = 0; j < index_list[z].Count; j++)
                            {
                                color1 = (ushort)((index_list[z][j][0] << 8) + index_list[z][j][1]);
                                color2 = (ushort)((index_list[z][j][2] << 8) + index_list[z][j][3]);

                                color_rgba[0] = (byte)(index_list[z][j][0] & 248);  // red
                                if (color_rgba[0] == 248)
                                {
                                    color_rgba[0] = 255;
                                }
                                color_rgba[1] = (byte)(((index_list[z][j][0] << 5) + (index_list[z][j][1] >> 3)) & 0xfc);  // green
                                if (color_rgba[1] == 252)
                                {
                                    color_rgba[1] = 255;
                                }
                                color_rgba[2] = (byte)((index_list[z][j][1] << 3) & 248);  // blue
                                if (color_rgba[2] == 248)
                                {
                                    color_rgba[2] = 255;
                                }
                                color_rgba[3] = 0xff; // rgb565 value has no alpha
                                cmpr_palette.Add(color_rgba.ToArray());
                                color_rgba[0] = (byte)(index_list[z][j][2] & 248);  // red
                                if (color_rgba[0] == 248)
                                {
                                    color_rgba[0] = 255;
                                }
                                color_rgba[1] = (byte)(((index_list[z][j][2] << 5) + (index_list[z][j][3] >> 3)) & 0xfc);  // green
                                if (color_rgba[1] == 252)
                                {
                                    color_rgba[1] = 255;
                                }
                                color_rgba[2] = (byte)((index_list[z][j][3] << 3) & 248);  // blue
                                if (color_rgba[2] == 248)
                                {
                                    color_rgba[2] = 255;
                                }
                                // color_rgba[3] = 0xff; // rgb565 value has no alpha
                                cmpr_palette.Add(color_rgba.ToArray());
                                if (color1 > color2)
                                {
                                    color_rgba[0] = (byte)((cmpr_palette[0][0] * 2 / 3) + (cmpr_palette[1][0] / 3));
                                    color_rgba[1] = (byte)((cmpr_palette[0][1] * 2 / 3) + (cmpr_palette[1][1] / 3));
                                    color_rgba[2] = (byte)((cmpr_palette[0][2] * 2 / 3) + (cmpr_palette[1][2] / 3));
                                    cmpr_palette.Add(color_rgba.ToArray());
                                    color_rgba[0] = (byte)((cmpr_palette[0][0] / 3) + (cmpr_palette[1][0] * 2 / 3));
                                    color_rgba[1] = (byte)((cmpr_palette[0][1] / 3) + (cmpr_palette[1][1] * 2 / 3));
                                    color_rgba[2] = (byte)((cmpr_palette[0][2] / 3) + (cmpr_palette[1][2] * 2 / 3));
                                    cmpr_palette.Add(color_rgba.ToArray());
                                }
                                else
                                {
                                    color_rgba[0] = (byte)((cmpr_palette[0][0] + cmpr_palette[1][0]) >> 1);
                                    color_rgba[1] = (byte)((cmpr_palette[0][1] + cmpr_palette[1][1]) >> 1);
                                    color_rgba[2] = (byte)((cmpr_palette[0][2] + cmpr_palette[1][2]) >> 1);
                                    // color_rgba[3] = 0xff; // rgb565 value has no alpha
                                    cmpr_palette.Add(color_rgba.ToArray());
                                    color_rgba[0] = 0;
                                    color_rgba[1] = 0;
                                    color_rgba[2] = 0;
                                    color_rgba[3] = 0;
                                    cmpr_palette.Add(color_rgba.ToArray());
                                }
                                //Console.WriteLine(index);
                                for (sbyte h = 0; h < 4; h++, index += width - 16)
                                {
                                    for (byte w = 0; w < 4; w++, index += 4)
                                    {
                                        pixel[index] = cmpr_palette[(index_list[z][j][7 - h] >> (6 - (w << 1))) & 3][2];  // blue
                                        pixel[index + 1] = cmpr_palette[(index_list[z][j][7 - h] >> (6 - (w << 1))) & 3][1];  // green
                                        pixel[index + 2] = cmpr_palette[(index_list[z][j][7 - h] >> (6 - (w << 1))) & 3][0];  // red
                                        pixel[index + 3] = cmpr_palette[(index_list[z][j][7 - h] >> (6 - (w << 1))) & 3][3];  // alpha
                                    }
                                }
                                //Console.WriteLine(index);
                                block++;
                                x += 8;  // basically the same width +=2 but here ref_width is 4 times canvas_width because of 32-bit depth bmp
                                if (x == width)
                                {
                                    x = 0;
                                    index += width - 16;
                                    block = 0;
                                }

                                else if (block == 2)
                                {
                                    index += 16;  // point to the top right sub-block 
                                }
                                else if (block == 4)
                                {
                                    index -= (width << 3) + 16;  // minus 8 lines and point to the bottom right sub-block of the next block
                                    block = 0;
                                }
                                else
                                {
                                    index -= (width << 2) + 16; // removes 4 lines and goes to the left sub-block
                                }
                                cmpr_palette.Clear();  // removes the 4 colours of the previous sub-block
                            }
                            break;
                        }
                }
            }
            if (z != 0)
            {
                end = ".mm" + z + ".bmp";
            }
            index = 0;
            done = false;
            while (!done)  // makes sure it writes the file.
            {

                try
                {
                    FileMode mode = System.IO.FileMode.CreateNew;
                    if (System.IO.File.Exists(output_file + end))
                    {
                        mode = System.IO.FileMode.Truncate;
                        if (warn)
                        {
                            Console.WriteLine("Press enter to overwrite " + output_file + end);
                            Console.ReadLine();
                        }
                    }
                    using (System.IO.FileStream file = System.IO.File.Open(output_file + end, mode, System.IO.FileAccess.Write))
                    {
                        file.Write(data, 0, data.Length); // byte[54]
                        file.Write(palette, 0, colour_number_x4);
                        // Console.WriteLine(alpha_header_size + " " + alpha_header.Length);
                        file.Write(alpha_header, 0, alpha_header_size);
                        file.Write(pixel, 0, image_size);
                        file.Close();
                        if (!stfu)
                            Console.WriteLine(output_file + end);
                        // done = true;  // fun fact, this statement IS executed. I DON4T F4CKING KNOW WHY I HAD TO PASTE IT THRICE
                    }
                    if (png || gif || jpeg || jpg || ico || tiff || tif)
                    {
                        // LMAO THE NUMBER OF ARGS, that's how you do dependancy injection without renaming EACH VARIABLE
                        using (Bitmap output_bmp = (Bitmap)Bitmap.FromFile(output_file + end))
                        {
                            Convert_from_bmp_class.Convert_from_bmp(output_bmp, z, output_file, canvas_dim[z][2], canvas_dim[z][3], png, tif, tiff, jpg, jpeg, gif, ico, no_warning, warn, stfu);  // the problem was in this func.....
                        }
                        // done = true;  // fun fact, this statement is never executed.
                    }
                    done = true;
                }
                catch (Exception ex)
                {
                    index += 1;
                    if (ex.Message.Substring(0, 34) == "The process cannot access the file")  // because it is being used by another process
                    {
                        if (index > 1)
                        {
                            output_file = output_file.Substring(output_file.Length - 2) + "-" + index;
                        }
                        else
                        {
                            output_file += "-" + index;
                        }
                    }
                    else if (safe_mode)
                    {
                        if (!no_warning)
                            Console.WriteLine("an error occured while trying to write the output file");
                        continue;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            //size = size >> 2;  // for next loop - divides by 4 because it's half the width size and half the height size
            //Array.Resize(ref pixel, pixel.Length >> 1);
        }
    }

}