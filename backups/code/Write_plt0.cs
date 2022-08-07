using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class output_class
{
    static public void write_BMP(List<List<byte[]>> index_list)  // index_list contains all mipmaps.
    {
        byte padding = (byte)(4 - (canvas_width % 4));
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
        byte[] palette = new byte[colour_number_x4];
        byte[] pixel = new byte[0];
        string end = ".bmp";
        bool done = false;
        // vanilla BITMAPV4HEADER
        // byte[] alpha_header = { 00, 00, 0xFF, 00, 00, 0xFF, 00, 00, 0xFF, 00, 00, 00, 00, 00, 00, 0xFF, 0x20, 0x6E, 0x69, 0x57, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 };

        // custom padding - the funni one  - it is mandatory to add these padding bytes, else paint.net won't open the picture. (it's fine with explorer.exe's preview) - these bytes can be whatever you want. so I've made it write some fun text 
        byte[] alpha_header = { 0, 0, 255, 0, 0, 255, 0, 0, 255, 0, 0, 0, 0, 0, 0, 255, 32, 110, 105, 87, 0, 104, 116, 116, 112, 115, 58, 47, 47, 100, 105, 115, 99, 111, 114, 100, 46, 103, 103, 47, 118, 57, 86, 112, 68, 90, 57, 0, 116, 104, 105, 115, 32, 105, 115, 32, 112, 97, 100, 100, 105, 110, 103, 32, 100, 97, 116, 97 };
        for (z = 0; z < mipmaps_number + 1; z++)
        {
            width = canvas_dim[z][2];
            header_width = canvas_dim[z][2];
            height = canvas_dim[z][3];
            switch (texture_format_int32[3])
            {
                case 0:  // I4
                    {
                        width >>= 1;  // 4-bit per pixel
                        colour_number_x4 = 64; // 16 * 4
                        break;
                    }
                case 8: // CI4
                    {
                        width >>= 1;  // 4-bit per pixel
                        break;
                    }
                case 1: // I8
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
                        colour_number_x4 = 0;
                        alpha_header_size = 0x44;
                        width <<= 2; // 32 bits per pixel
                        break;
                    }
            }
            if ((palette_format_int32[3] == 2 && alpha > 0) || bmp_32 || (palette_format_int32[3] == 0 && alpha > 0))  // AI8 Palette with alpha  // RGB5A3 Palette with alpha
            {
                colour_number_x4 = 0;
                alpha_header_size = 0x44;
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
            if ((has_palette && texture_format_int32[3] != 10 && !bmp_32 && (palette_format_int32[3] != 0 || alpha < 1) && (palette_format_int32[3] != 2 || alpha < 1)) || funky) // && (texture_format_int32[3] > 2 || texture_format_int32[3] != 0xe)))
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
                            // fill palette data
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

                            break;
                        }
                    case 1:  // reverse I8
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
                                    for (int k = 0; k < index_list[z][0].Length; k++, index += 4)
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
                            List<byte[]> colour_palette = new List<byte[]>();
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
                                colour_palette.Add(color_rgba.ToArray());
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
                                colour_palette.Add(color_rgba.ToArray());
                                if (color1 > color2)
                                {
                                    color_rgba[0] = (byte)((colour_palette[0][0] * 2 / 3) + (colour_palette[1][0] / 3));
                                    color_rgba[1] = (byte)((colour_palette[0][1] * 2 / 3) + (colour_palette[1][1] / 3));
                                    color_rgba[2] = (byte)((colour_palette[0][2] * 2 / 3) + (colour_palette[1][2] / 3));
                                    colour_palette.Add(color_rgba.ToArray());
                                    color_rgba[0] = (byte)((colour_palette[0][0] / 3) + (colour_palette[1][0] * 2 / 3));
                                    color_rgba[1] = (byte)((colour_palette[0][1] / 3) + (colour_palette[1][1] * 2 / 3));
                                    color_rgba[2] = (byte)((colour_palette[0][2] / 3) + (colour_palette[1][2] * 2 / 3));
                                    colour_palette.Add(color_rgba.ToArray());
                                }
                                else
                                {
                                    color_rgba[0] = (byte)((colour_palette[0][0] + colour_palette[1][0]) >> 1);
                                    color_rgba[1] = (byte)((colour_palette[0][1] + colour_palette[1][1]) >> 1);
                                    color_rgba[2] = (byte)((colour_palette[0][2] + colour_palette[1][2]) >> 1);
                                    // color_rgba[3] = 0xff; // rgb565 value has no alpha
                                    colour_palette.Add(color_rgba.ToArray());
                                    color_rgba[0] = 0;
                                    color_rgba[1] = 0;
                                    color_rgba[2] = 0;
                                    color_rgba[3] = 0;
                                    colour_palette.Add(color_rgba.ToArray());
                                }
                                //Console.WriteLine(index);
                                for (sbyte h = 0; h < 4; h++, index += width - 16)
                                {
                                    for (byte w = 0; w < 4; w++, index += 4)
                                    {
                                        pixel[index] = colour_palette[(index_list[z][j][7 - h] >> (6 - (w << 1))) & 3][2];  // blue
                                        pixel[index + 1] = colour_palette[(index_list[z][j][7 - h] >> (6 - (w << 1))) & 3][1];  // green
                                        pixel[index + 2] = colour_palette[(index_list[z][j][7 - h] >> (6 - (w << 1))) & 3][0];  // red
                                        pixel[index + 3] = colour_palette[(index_list[z][j][7 - h] >> (6 - (w << 1))) & 3][3];  // alpha
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
                                colour_palette.Clear();  // removes the 4 colours of the previous sub-block
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
                    if (png || gif || jpeg || ico || tiff || tif)
                    {
                        ConvertAndSave((Bitmap)Bitmap.FromFile(output_file + end), z);  // the problem was in this func.....
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

    static public void write_BTI(List<List<byte[]>> index_list)  // index_list contains all mipmaps.
    {
        int size = 0x20 + colour_palette.Length + 0x40; // fixed size at 1 image
        double temp;
        int[] param = new int[4];
        List<int[]> settings = new List<int[]>();
        for (int i = 0; i < mipmaps_number + 1; i++)
        {
            if (i == 0)
            {
                param[2] = (int)(index_list[i][0].Length * format_ratio);
                param[3] = index_list[i].Count;
            }
            else
            {
                temp = bitmap_width / Math.Pow(2, i);
                if (temp % 1 != 0)
                {
                    param[2] = (int)(index_list[i][0].Length * format_ratio) + 1;
                    // param[2] = (int)temp + 1;
                }
                else
                {
                    // param[2] = (int)temp;
                    param[2] = (int)(index_list[i][0].Length * format_ratio);
                }
                temp = bitmap_height / Math.Pow(2, i);
                if (temp % 1 != 0)
                {
                    // param[3] = (int)temp + 1;
                    param[3] = index_list[i].Count;
                }
                else
                {
                    // param[3] = (int)temp;
                    param[3] = index_list[i].Count;
                }
            }
            temp = param[2] / block_width;
            if (temp % 1 != 0)
            {
                param[0] = (int)temp + 1;
            }
            else
            {
                param[0] = (int)temp;
            }
            temp = param[3] / block_height;
            if (temp % 1 != 0)
            {
                param[1] = (int)temp + 1;
            }
            else
            {
                param[1] = (int)temp;
            }
            settings.Add(param.ToArray());
            // size += param[0] * block_width * param[1] * block_height;
            size += index_list[i][0].Length * index_list[i].Count;
        }
        byte size2 = (byte)(4 + Math.Abs(16 - size) % 16);
        byte len = (byte)output_file.Split('\\').Length;
        string file_name = (output_file.Split('\\')[len - 1]);
        byte[] data2 = new byte[size2 + len + (16 - len) % 16];
        byte[] data = new byte[32];  // header data
        for (int i = 0; i < size2; i++)
        {
            data2[i] = 0;
        }
        for (int i = 0; i < file_name.Length; i++)
        {
            data2[i + size2] = (byte)file_name[i];
        }
        for (int i = size2 + file_name.Length; i < data2.Length; i++)
        {
            data2[i] = 0;
        }
        data[0] = texture_format_int32[3];  // image format, pretty straightforward it isn't an int lol
        data[1] = alpha;
        data[2] = (byte)(bitmap_width >> 8);  // unsigned short width
        data[3] = (byte)bitmap_width; // second byte of width
        data[4] = (byte)(bitmap_height >> 8);  // height
        data[5] = (byte)bitmap_height;
        data[6] = WrapS; // sideways wrap
        data[7] = WrapT; // vertical wrap
        if (has_palette)
        {
            data[8] = 1; // well, 1 means palette enabled, and the whole purpose of this tool is to make images with palette LMAO
            data[9] = palette_format_int32[3]; // pretty straightforward again
            data[10] = (byte)(colour_number >> 8);
            data[11] = (byte)(colour_number);  // number of colours
            data[12] = 0;
            data[13] = 0;
            data[14] = 0;
            data[15] = 32; // palette data address
        }
        else
        {

            data[8] = 0;  // well, I've changed my mind, I wanna make it better than wimgt 
            data[9] = 0; // pretty straightforward again
            data[10] = 0;
            data[11] = 0;  // number of colours
            data[12] = 0;
            data[13] = 0;
            data[14] = 0;
            data[15] = 0; // palette data address
        }
        if (mipmaps_number > 0)
        {
            data[16] = 1;
        }
        else
        {
            data[16] = 0;
        }
        data[17] = 0;  // EdgeLOD (bool)
        data[18] = 0;  // BiasClamp (bool)
        data[19] = 0;  // MaxAniso (byte)
        data[20] = minificaction_filter;
        data[21] = magnification_filter;
        data[22] = 0; // MinLOD
        data[23] = (byte)(mipmaps_number << 3);  // MaxLOD   << 3 is faster than * 8
        data[24] = (byte)(mipmaps_number + 1);  // number of images
        data[25] = 0x69; // my signature XDDDD, I couldn't figure out what this setting does, but it doesn't affect the gameplay at all, it looks like it is used as a version number.
        data[26] = 0; // how do I calculate LODBIAS
        data[27] = 0; // how do I calculate LODBIAS
        data[28] = 0;  // won't exceed the max number of colours in the colour table which is stored on 2 bytes + header size 0x20
        data[29] = (byte)((0x20 + colour_palette.Length) >> 16); // >> 16 is better than / 65536
        data[30] = (byte)((0x20 + colour_palette.Length) >> 8);  // >> 8 is better than / 256
        data[31] = (byte)(0x20 + colour_palette.Length);  // offset to image0 header, (byte) acts as %256
                                                          // now's palette data, but it's already stored in colour_palette, so let's jump onto image data.
        byte[] tex_data = new byte[size - 0x20 - colour_palette.Length];
        create_blocks(tex_data, settings, index_list);

        // finished to get everything of the bti
        // now we'll check if that bti needs to be injected in a bmd
        // I made extract-bti.py first, so I'm using it as a template
        // 0x14/20 bytes long
        /*
        TEX1 Header description
        0x00
        char[4] Magic_TEX1; //'TEX1'
        0x04
        int ChunkSize; //Total bytes of chunk
        0x08
        short TextureCount; //Number of textures
        0x0A
        short Padding; (Usually 0xFFFF)
        0x0C
        int TextureHeaderOffset; // (always 0x20) TextureCount bti image headers are stored here. Relative to TEX1Header start.
        0x10
        int StringTableOffset; //Stores one filename for each texture. Relative to TEX1Header start.
        */
        if (bmd_file)
        {
            if (has_palette)
            {
                Console.WriteLine("Sorry man, I have not met any bmd file with a palette amongst all games I've reviewed so far. I'll be glad to add this feature if you find some files I could look into\n for now you can't use CI4, CI8 and CI14x2 in a bmd\n");
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
                return;
            }
            using (System.IO.FileStream file = System.IO.File.Open(input_file2, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                byte[] j3d = new byte[file.Length];
                file.Read(j3d, 0, j3d.Length);
                file.Close();
                y = 0x20; // next section offset
                while (!(j3d[y] == 'T' && j3d[y + 1] == 'E' && j3d[y + 2] == 'X' && j3d[y + 3] == '1'))  // section identifier
                {
                    if (y + 9 > j3d.Length)
                    {
                        if (!no_warning)
                            Console.WriteLine("Couldn't find the TEX1 Section. This bmd file is corrupted. the program will exit");
                        return;
                    }
                    y += (j3d[y + 4] << 24) + (j3d[y + 5] << 16) + (j3d[y + 6] << 8) + j3d[y + 7];  // size of current section
                }
                // now that we found TEX1 section address, we can find string pool table address, then get all texture names
                int x = y + (j3d[y + 16] << 24) + (j3d[y + 17] << 16) + (j3d[y + 18] << 8) + j3d[y + 19];  // string pool table start offset
                int string_pool_table_start_offset = x;
                ushort string_count = (ushort)((j3d[x] << 8) + j3d[x + 1]);
                int string_count_5 = string_count << 5;
                if (string_count == 0)  // ARE YOU KIDDING ME
                {
                    if (!no_warning)
                        Console.WriteLine("I could have sworn you were smart enough to take a bmd with bti textures in it, but this one contains none.");
                    return;
                }
                string name = "";
                x += 4 + (string_count << 2); // 4 is the string pool table header size
                                              // string count is multiplied by 4 because after the header there is a ushort hash and a ushort offset for each string
                success = false;
                ushort[] score = new ushort[string_count];
                int f = 0;
                ushort current_string = 0;
                ushort best_score = 0;  // length of the biggest substring in common
                ushort current = 0;
                List<string> name_list = new List<string>();
                while (current_string < string_count) // used to compare each texture name with the input file
                {
                    while (j3d[x] != 0)  // 0 is supposed to be the byte separating strings. never seen another value in mkdd
                    {
                        name += j3d[x];
                        x++;
                    }
                    name_list.Add(name);
                    if (name == input_fil)
                    {
                        success = true;
                        break;
                    }
                    else  // use an algorithm to determine the MOST SIMILAR texture by file name
                    {
                        for (z = 0; z < name.Length - 1; z++)
                        {
                            for (pass = 0; pass < input_fil.Length; pass++)
                            {
                                if (name[z] == input_fil[pass] && name[z + 1] == input_fil[pass])
                                {
                                    break;
                                }
                            }  // used to find the start of the similar substring
                               // no, don't say 1 byte length files won't work here. They're supposed to fall in the first if
                            for (f = z, current = 0; f < input_fil.Length && f < name.Length; f++, current++)
                            {
                                if (input_fil[pass] != name[f])
                                {
                                    break;
                                }
                            }
                            // get the length of the common substring 
                            if (current > best_score)
                            {
                                best_score = current;
                            }
                        }
                        score[current_string] = best_score;
                    }
                    current_string++;
                }
                if (!success)  // get the most fitting texture and ask for confirmation
                {
                    best_score = 0;
                    f = 0;
                    for (z = 0; z < score.Length; z++)
                    {
                        if (score[z] < best_score)
                        {
                            best_score = score[z];
                            f = z;
                        }
                    }
                    name = name_list[f];  // most probable fitting texture by the file name
                    if (warn)
                    {
                        Console.WriteLine("File names doesn't exactly match. Press Enter to replace " + name + " inside " + input_file2);
                        Console.ReadLine();
                    }
                }
                List<int> data_offset_array = new List<int>();
                List<int> data_size_array = new List<int>();
                int blocks_wide;
                int blocks_tall;
                int curr_mipmap_size;
                // f = bti header of the image to replace >> 5 (divided by 32, and relative to y)
                // y = TEX1 section start offset
                // z = current bti header offset (relative to y)
                // 
                for (z = 0; z < string_count_5; z += 32)
                {
                    if ((z >> 5) == f)
                    {
                        data[12] = (byte)(((string_count_5 - z)) << 24);
                        data[13] = (byte)(((string_count_5 - z)) << 16);
                        data[14] = (byte)(((string_count_5 - z)) << 8);
                        data[15] = (byte)(((string_count_5 - z)));
                        x = y + (z + 1 * 0x20);
                    }
                    int data_offset = (j3d[y + z + 0x3C] << 24) + (j3d[y + z + 0x3D] << 16) + (j3d[y + z + 0x3E] << 8) + (j3d[y + z + 0x3F]);
                    data_offset_array.Add(data_offset);
                    blocks_wide = (j3d[y + 0x22 + z] + j3d[y + 0x23 + z] + block_width_array[j3d[y + 0x20 + z]]) / block_width_array[j3d[y + 0x20 + z]];
                    blocks_tall = (j3d[y + 0x24 + z] + j3d[y + 0x25 + z] + block_width_array[j3d[y + 0x20 + z]]) / block_height_array[j3d[y + 0x20 + z]];
                    int data_size = (blocks_wide * blocks_tall);
                    if (j3d[y + 0x20 + z] == 6)
                    {
                        data_size <<= 6;
                    }
                    else
                    {
                        data_size <<= 5;
                    }
                    curr_mipmap_size = data_size;
                    for (int i = 1; i < j3d[y + 0x38 + z]; i++)
                    {
                        curr_mipmap_size >>= 2;
                        data_size += curr_mipmap_size;
                    }
                    data_size_array.Add(data_size);
                    /*blocks_wide = (self.width + (self.block_width-1)) // self.block_width
                      blocks_tall = (self.height + (self.block_height-1)) // self.block_height
                      image_data_size = blocks_wide*blocks_tall*self.block_data_size
                      remaining_mipmaps = self.mipmap_count-1
                      curr_mipmap_size = image_data_size
                      while remaining_mipmaps > 0:
                      # Each mipmap is a quarter the size of the last (half the width and half the height).
                          curr_mipmap_size = curr_mipmap_size//4
                          image_data_size += curr_mipmap_size
                          remaining_mipmaps -= 1*/

                    // fix palette_offset inside bmd which is the offset to the end of all bti headers
                    j3d[y + 0x20 + (0x20 * z) + 12] = (byte)(((string_count_5 - z)) << 24);
                    j3d[y + 0x20 + (0x20 * z) + 13] = (byte)(((string_count_5 - z)) << 16);
                    j3d[y + 0x20 + (0x20 * z) + 14] = (byte)(((string_count_5 - z)) << 8);
                    j3d[y + 0x20 + (0x20 * z) + 15] = (byte)(((string_count_5 - z)));
                }
                // now let's remove the image data that we're replacing if it's unused
                // no, I won't check for unused texture data apart from this one. these kind of things were generated by other softwares!
                int start = y + string_count_5 + 0x20;  // bti headers end offset
                int middle = data_offset_array[f] + ((string_count - f) << 5); // relative to start
                int start_2 = y + string_count_5 + data_offset_array[f + 1] - ((string_count - f + 1) << 5);
                for (int z = 0; z < string_count_5; z += 32)
                {
                    if (data_offset_array[z >> 5] < middle)
                    {
                        if ((data_offset_array[z >> 5] + data_size_array[z >> 5]) > middle)
                        {
                            middle = (data_offset_array[z >> 5] + data_size_array[z >> 5]);
                            if (middle > start_2)
                            {
                                start_2 = middle;
                            }
                        }
                    }
                }
                FileMode mode = System.IO.FileMode.CreateNew;
                if (System.IO.File.Exists(output_file + ".bmd"))
                {
                    mode = System.IO.FileMode.Truncate;
                    if (warn)
                    {
                        Console.WriteLine("Press enter to overwrite " + output_file + ".bmd");
                        Console.ReadLine();
                    }
                }
                using (System.IO.FileStream ofile = System.IO.File.Open(output_file + ".bmd", mode, System.IO.FileAccess.Write))
                {
                    ofile.Write(j3d, 0, y);
                    // write texture headers
                    if (f != 0)
                    {
                        ofile.Write(j3d, y, f << 5);
                    }
                    ofile.Write(data, 0, 32);
                    if (f != string_count - 1)
                    {
                        ofile.Write(j3d, x, (string_count - f - 1) << 5);
                    }
                    if (middle == start_2)
                    {
                        ofile.Write(j3d, start, j3d.Length - start);
                    }
                    else
                    {
                        ofile.Write(j3d, start, middle);
                        ofile.Write(j3d, start_2, j3d.Length - start_2);
                    }
                    ofile.Close();
                    if (!stfu)
                        Console.WriteLine(output_file + ".bmd");
                }
            }
        }
        else  // single bti
        {
            FileMode mode = System.IO.FileMode.CreateNew;
            if (System.IO.File.Exists(output_file + ".bti"))
            {
                mode = System.IO.FileMode.Truncate;
                if (warn)
                {
                    Console.WriteLine("Press enter to overwrite " + output_file + ".bti");
                    Console.ReadLine();
                }
            }
            using (System.IO.FileStream file = System.IO.File.Open(output_file + ".bti", mode, System.IO.FileAccess.Write))
            {
                file.Write(data, 0, 32);
                file.Write(colour_palette, 0, colour_palette.Length);
                file.Write(tex_data, 0, tex_data.Length);
                file.Write(data2, 0, data2.Length);
                file.Close();
                if (!stfu)
                    Console.WriteLine(output_file + ".bti");
            }
        }
    }

    static public void write_TPL(List<List<byte[]>> index_list)  // index_list contains all mipmaps.
    {
        int size = 0x20 + colour_palette.Length + 0x40; // fixed size at 1 image
        double temp;
        int[] param = new int[4];
        List<int[]> settings = new List<int[]>();
        for (int i = 0; i < mipmaps_number + 1; i++)
        {
            if (i == 0)
            {
                param[2] = (int)(index_list[i][0].Length * format_ratio);
                param[3] = index_list[i].Count;
            }
            else
            {
                temp = bitmap_width / Math.Pow(2, i);
                if (temp % 1 != 0)
                {
                    param[2] = (int)(index_list[i][0].Length * format_ratio) + 1;
                    // param[2] = (int)temp + 1;
                }
                else
                {
                    // param[2] = (int)temp;
                    param[2] = (int)(index_list[i][0].Length * format_ratio);
                }
                temp = bitmap_height / Math.Pow(2, i);
                if (temp % 1 != 0)
                {
                    // param[3] = (int)temp + 1;
                    param[3] = index_list[i].Count;
                }
                else
                {
                    // param[3] = (int)temp;
                    param[3] = index_list[i].Count;
                }
            }
            temp = param[2] / block_width;
            if (temp % 1 != 0)
            {
                param[0] = (int)temp + 1;
            }
            else
            {
                param[0] = (int)temp;
            }
            temp = param[3] / block_height;
            if (temp % 1 != 0)
            {
                param[1] = (int)temp + 1;
            }
            else
            {
                param[1] = (int)temp;
            }
            settings.Add(param.ToArray());
            // size += param[0] * block_width * param[1] * block_height;
            size += index_list[i][0].Length * index_list[i].Count;
        }
        byte size2 = (byte)(4 + Math.Abs(16 - size) % 16);
        byte len = (byte)output_file.Split('\\').Length;
        string file_name = (output_file.Split('\\')[len - 1]);
        byte[] data2 = new byte[size2 + len + (16 - len) % 16];
        byte[] data = new byte[32];  // header data
        byte[] header = new byte[64];  // image header data
        for (int i = 0; i < size2; i++)
        {
            data2[i] = 0;
        }
        for (int i = 0; i < file_name.Length; i++)
        {
            data2[i + size2] = (byte)file_name[i];
        }
        for (int i = size2 + file_name.Length; i < data2.Length; i++)
        {
            data2[i] = 0;
        }
        data[0] = 0;
        data[1] = 0x20;
        data[2] = 0xAF;
        data[3] = 0x30;  // file identifier
        data[4] = 0;
        data[5] = 0;
        data[6] = 0;
        data[7] = 1;  // number of images
        data[8] = 0;
        data[9] = 0;
        data[10] = 0;
        data[11] = 0x0C; // offset to image table
        data[12] = 0;  // won't exceed the max number of colours in the colour table which is stored on 2 bytes + header size 0x20
        data[13] = (byte)((0x20 + colour_palette.Length) >> 16);
        data[14] = (byte)((0x20 + colour_palette.Length) >> 8);
        data[15] = (byte)(0x20 + colour_palette.Length);  // offset to image0 header, (byte) acts as %256
        data[16] = 0;
        data[17] = 0;
        data[18] = 0;
        if (has_palette)
        {
            data[19] = 20;  // offset to palette0 header
            data[20] = (byte)(colour_number >> 8);
            data[21] = (byte)(colour_number);  // number of colours

            data[27] = palette_format_int32[3]; // palette format

            data[31] = 32; // palette data address
        }
        else
        {
            data[19] = 0;
            data[20] = 0;
            data[21] = 0;  // number of colours

            data[27] = 0; // palette format

            data[31] = 0; // palette data address

        }
        data[22] = 0;  // unpacked (0 I guess)
        data[23] = 0;  // padding
        data[24] = palette_format_int32[0];
        data[25] = palette_format_int32[1];
        data[26] = palette_format_int32[2]; // palette format
        data[28] = 0;
        data[29] = 0;
        data[30] = 0;
        // now's palette data, but it's already stored in colour_palette, so let's jump onto image header.
        header[0] = (byte)(bitmap_width >> 8);  // unsigned short width
        header[1] = (byte)bitmap_width; // second byte of width
        header[2] = (byte)(bitmap_height >> 8);  // height
        header[3] = (byte)bitmap_height;
        header[4] = texture_format_int32[0];
        header[5] = texture_format_int32[1];
        header[6] = texture_format_int32[2];
        header[7] = texture_format_int32[3];
        header[8] = 0; // won't exceed the max number of colours in the colour table which is stored on 2 bytes + header size 0x20
        header[9] = (byte)((0x60 + colour_palette.Length) >> 16);
        header[10] = (byte)((0x60 + colour_palette.Length) >> 8);
        header[11] = (byte)(0x60 + colour_palette.Length);
        header[12] = 0;
        header[13] = 0;
        header[14] = 0;
        header[15] = WrapS;
        header[16] = 0;
        header[17] = 0;
        header[18] = 0;
        header[19] = WrapT;
        header[20] = 0;
        header[21] = 0;
        header[22] = 0;
        header[23] = minificaction_filter;
        header[24] = 0;
        header[25] = 0;
        header[26] = 0;
        header[27] = magnification_filter;
        header[28] = 0;
        header[29] = 0;
        header[30] = 0;
        header[31] = 0; // LODBias
        header[32] = 0; // EdgeLODEnable
        header[33] = 0; // MinLOD
        header[34] = mipmaps_number; // MaxLOD
        header[35] = 0; // unpacked
        for (int i = 36; i < 64; i++)  // nintendo does this on their tpl. I guess it's for a good reason
        {
            header[i] = 0;
        }
        byte[] tex_data = new byte[size - 0x60 - colour_palette.Length];
        create_blocks(tex_data, settings, index_list);
        FileMode mode = System.IO.FileMode.CreateNew;
        if (System.IO.File.Exists(output_file + ".tpl"))
        {
            mode = System.IO.FileMode.Truncate;
            if (warn)
            {
                Console.WriteLine("Press enter to overwrite " + output_file + ".tpl");
                Console.ReadLine();
            }
        }
        using (System.IO.FileStream file = System.IO.File.Open(output_file + ".tpl", mode, System.IO.FileAccess.Write))
        {
            file.Write(data, 0, 32);
            file.Write(colour_palette, 0, colour_palette.Length);  // can I write nothing?
            file.Write(header, 0, 64);
            file.Write(tex_data, 0, tex_data.Length);
            file.Write(data2, 0, data2.Length);
            file.Close();
            if (!stfu)
                Console.WriteLine(output_file + ".tpl");
        }
    }

    static public void write_PLT0()
    {
        int size = 0x40 + colour_palette.Length;
        byte size2 = (byte)(4 + Math.Abs(16 - size) % 16);
        byte len = (byte)output_file.Split('\\').Length;
        string file_name = (output_file.Split('\\')[len - 1]);
        byte[] data2 = new byte[size2 + len + ((16 - len) % 16)];
        byte[] data = new byte[64];  // header data
        for (int i = 0; i < size2; i++)
        {
            data2[i] = 0;
        }
        for (int i = 0; i < file_name.Length; i++)
        {
            data2[i + size2] = (byte)file_name[i];
        }
        for (int i = size2 + file_name.Length; i < data2.Length; i++)
        {
            data2[i] = 0;
        }
        data[0] = (byte)'P';
        data[1] = (byte)'L';
        data[2] = (byte)'T';
        data[3] = (byte)'0';  // file identifier
        data[4] = (byte)(size >> 24);
        data[5] = (byte)(size >> 16);
        data[6] = (byte)(size >> 8);
        data[7] = (byte)(size);  // file size
        data[8] = 0;
        data[9] = 0;
        data[10] = 0;
        data[11] = 3; // version
        data[12] = 0;
        data[13] = 0;
        data[14] = 0;
        data[15] = 0; // offset to outer brres
        data[16] = 0;
        data[17] = 0;
        data[18] = 0;
        data[19] = 64; // header size
        data[20] = (byte)((size + size2) >> 24);
        data[21] = (byte)((size + size2) >> 16);
        data[22] = (byte)((size + size2) >> 8);
        data[23] = (byte)(size + size2);  // name location
        data[24] = palette_format_int32[0];
        data[25] = palette_format_int32[1];
        data[26] = palette_format_int32[2];
        data[27] = palette_format_int32[3];
        data[28] = (byte)(colour_number >> 8);
        data[29] = (byte)colour_number;
        for (int i = 30; i < 64; i++)
        {
            data[i] = 0;
        }
        FileMode mode = System.IO.FileMode.CreateNew;
        if (System.IO.File.Exists(output_file + ".plt0"))
        {
            mode = System.IO.FileMode.Truncate;
            if (warn)
            {
                Console.WriteLine("Press enter to overwrite " + output_file + ".plt0");
                Console.ReadLine();
            }
        }
        using (System.IO.FileStream file = System.IO.File.Open(output_file + ".plt0", mode, System.IO.FileAccess.Write))
        {
            file.Write(data, 0, 64);
            file.Write(colour_palette, 0, colour_palette.Length);
            file.Write(data2, 0, data2.Length);
            file.Close();
            if (!stfu)
                Console.WriteLine(output_file + ".plt0");
        }
    }

    //[MethodImpl(MethodImplOptions.NoOptimization)]
    /// <summary>
    /// writes a TEX0 file from parameters of plt0_class and the list given in argument
    /// </summary>
    /// <param name="index_list">a List of mipmaps, first one being the highest quality one. <br/>
    /// each mipmap contains a list of every row of the image (starting by the bottom one). <br/>
    /// each row is actually a byte array of every pixel encoded in a specific format.</param>
    /// <returns>nothing. but writes the file into the output name given in CLI argument</returns>
    static public void write_TEX0(List<List<byte[]>> index_list)  // index_list contains all mipmaps.
    {
        int size = 0x40;
        double temp;
        int[] param = new int[4];
        List<int[]> settings = new List<int[]>();
        for (int i = 0; i < mipmaps_number + 1; i++)
        {
            if (i == 0)
            {
                param[2] = (int)(index_list[i][0].Length * format_ratio);
                param[3] = index_list[i].Count;
                // param[2] = bitmap_width;
                // param[3] = bitmap_height;
            }
            else
            {
                temp = bitmap_width / Math.Pow(2, i);
                if (temp % 1 != 0)
                {
                    param[2] = (int)(index_list[i][0].Length * format_ratio) + 1;
                    // param[2] = (int)temp + 1;
                }
                else
                {
                    // param[2] = (int)temp;
                    param[2] = (int)(index_list[i][0].Length * format_ratio);
                }
                temp = bitmap_height / Math.Pow(2, i);
                if (temp % 1 != 0)
                {
                    // param[3] = (int)temp + 1;
                    param[3] = index_list[i].Count;
                }
                else
                {
                    // param[3] = (int)temp;
                    param[3] = index_list[i].Count;
                }
            }
            temp = param[2] / block_width;
            if (temp % 1 != 0)
            {
                param[0] = (int)temp + 1;
            }
            else
            {
                param[0] = (int)temp;
            }
            temp = param[3] / block_height;
            if (temp % 1 != 0)
            {
                param[1] = (int)temp + 1;
            }
            else
            {
                param[1] = (int)temp;
            }
            settings.Add(param.ToArray());
            // size += param[0] * block_width * param[1] * block_height;
            size += index_list[i][0].Length * index_list[i].Count;
        }
        byte size2 = (byte)(4 + Math.Abs(16 - size) % 16);
        byte len = (byte)output_file.Split('\\').Length;
        string file_name = (output_file.Split('\\')[len - 1]);
        byte[] data2 = new byte[size2 + len + (16 - len) % 16];
        byte[] data = new byte[64];  // header data
        float mipmaps = mipmaps_number;
        byte[] mipmap_float = BitConverter.GetBytes(mipmaps);
        for (int i = 0; i < size2; i++)
        {
            data2[i] = 0;
        }
        for (int i = 0; i < file_name.Length; i++)
        {
            data2[i + size2] = (byte)file_name[i];
        }
        for (int i = size2 + file_name.Length; i < data2.Length; i++)
        {
            data2[i] = 0;
        }
        data[0] = (byte)'T';
        data[1] = (byte)'E';
        data[2] = (byte)'X';
        data[3] = (byte)'0';  // file identifier
        data[4] = (byte)(size >> 24);
        data[5] = (byte)(size >> 16);
        data[6] = (byte)(size >> 8);
        data[7] = (byte)(size);  // file size
        data[8] = 0;
        data[9] = 0;
        data[10] = 0;
        data[11] = 3; // version
        data[12] = 0;
        data[13] = 0;
        data[14] = 0;
        data[15] = 0;  // offset to outer brres
        data[16] = 0;
        data[17] = 0;
        data[18] = 0;
        data[19] = 64;  // header size
        data[20] = (byte)((size + size2) >> 24);
        data[21] = (byte)((size + size2) >> 16);
        data[22] = (byte)((size + size2) >> 8);
        data[23] = (byte)(size + size2);  // name location
        data[24] = 0;
        data[25] = 0;
        data[26] = 0;
        if (has_palette)
        {
            data[27] = 1;  // image has a palette
        }
        else
        {
            data[27] = 0; // image don't have a palette
        }
        data[28] = (byte)(bitmap_width >> 8);  // unsigned short width
        data[29] = (byte)bitmap_width; // second byte of width
        data[30] = (byte)(bitmap_height >> 8);  // height
        data[31] = (byte)bitmap_height;
        data[32] = texture_format_int32[0];
        data[33] = texture_format_int32[1];
        data[34] = texture_format_int32[2];
        data[35] = texture_format_int32[3];
        data[36] = 0;
        data[37] = 0;
        data[38] = 0;
        data[39] = (byte)(mipmaps_number + 1);
        data[40] = 0;
        data[41] = 0;
        data[42] = 0;
        data[43] = 0;  // always zero
        data[44] = mipmap_float[0];
        data[45] = mipmap_float[1];
        data[46] = mipmap_float[2];
        data[47] = mipmap_float[3];
        for (int i = 48; i < 64; i++)  // undocumented bytes
        {
            data[i] = 0;
        }
        byte[] tex_data = new byte[size - 64];
        create_blocks(tex_data, settings, index_list);
        FileMode mode = System.IO.FileMode.CreateNew;
        if (System.IO.File.Exists(output_file + ".tex0"))
        {
            mode = System.IO.FileMode.Truncate;
            if (warn)
            {
                Console.WriteLine("Press enter to overwrite " + output_file + ".tex0");
                Console.ReadLine();
            }
        }
        using (System.IO.FileStream file = System.IO.File.Open(output_file + ".tex0", mode, System.IO.FileAccess.Write))
        {
            file.Write(data, 0, 64);
            file.Write(tex_data, 0, size - 64);
            file.Write(data2, 0, data2.Length);
            file.Close();
            if (!stfu)
                Console.WriteLine(output_file + ".tex0");
            // Console.ReadLine();
        }
    }

    static public byte[] create_blocks(byte[] tex_data, List<int[]> settings, List<List<byte[]>> index_list)
    {
        int count = 0;
        int height;
        int width;
        block_width = (sbyte)(block_width / format_ratio);
        if (texture_format_int32[3] == 6)  // RGBA32 has a f-cking byte order
        {
            for (int i = 0; i < settings.Count; i++)  // mipmaps
            {
                height = settings[i][1] * block_height - 1;
                width = 0;
                for (int j = settings[i][1]; j > 0; j--)  // number of height blocks  (remember that the last line was added first into index_list)
                {
                    for (int k = 0; k < settings[i][0]; k++)  // number of width blocks
                    {
                        for (int h = 0; h < 4; h++)  // height in the block
                        {
                            for (int w = 0; w < 8; w++)  // width in the block
                            {
                                tex_data[count] = index_list[i][height - h][width + w];  // adds the 8 ARAR ARAR formatted bytes for each line
                                count++;
                            }
                        }
                        for (int h = 0; h < 4; h++)  // height in the block
                        {
                            for (int w = 8; w < 16; w++)  // width in the block
                            {
                                tex_data[count] = index_list[i][height - h][width + w];  // adds the 8 GBGB GBGB formatted bytes for each line
                                count++;
                            }
                        }
                        width += block_width;
                    } // end of the loop to go through number of horizontal blocks
                    height -= block_height;
                    width = 0;
                }   // go through vertical blocks
            }      // go through mipmaps
        }
        else if (texture_format_int32[3] == 0xe) // yeah, I ordered CMPR by sub-blocks
        {
            for (int i = 0; i < settings.Count; i++)  // mipmaps
            {
                height = (settings[i][1] << 3) - 1;
                for (int j = height; j >= 0; j--)  // number of height blocks  (remember that the last line was added first into index_list)
                {
                    for (int w = 0; w < 8; w++)  // width in the sub-block
                    {
                        tex_data[count] = index_list[i][j][w];  // adds the 8 CMPR formatted bytes for each line
                        count++;
                    }

                    // end of the loop to go through number of horizontal blocks
                }   // go through vertical blocks
            }      // go through mipmaps
        }
        else
        {
            for (int i = 0; i < settings.Count; i++)  // mipmaps
            {
                height = settings[i][1] * block_height - 1;
                width = 0;
                for (int j = settings[i][1]; j > 0; j--)  // number of height blocks  (remember that the last line was added first into index_list)
                {
                    for (int k = 0; k < settings[i][0]; k++)  // number of width blocks
                    {
                        for (int h = 0; h < block_height; h++)  // height in the block
                        {
                            for (int w = 0; w < block_width; w++)  // width in the block
                            {
                                tex_data[count] = index_list[i][height - h][width + w];
                                count++;
                            }
                        }
                        width += block_width;
                    } // end of the loop to go through number of horizontal blocks
                    height -= block_height;
                    width = 0;
                }   // go through vertical blocks
            }      // go through mipmaps
        }
        return tex_data;
    }
}