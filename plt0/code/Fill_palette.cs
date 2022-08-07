class Fill_palette_class
{
    public static byte[] Fill_palette(byte[] BGRA_array, int pixel_start_offset, int array_size, byte[] colour_palette, byte[] rgba_channel, double[] custom_rgba, byte[] palette_format_int32, byte algorithm, byte alpha, byte round3, byte round4, byte round5, byte round6)
    {
        byte red, green, blue, a;
        switch (palette_format_int32[3])  // fills the colour table
        {
            case 0: // AI8
                {
                    switch (algorithm)
                    {
                        case 0: // cie_601
                            {
                                for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
                                {
                                    colour_palette[j] = (BGRA_array[i + rgba_channel[3]]);  // alpha value
                                    if (BGRA_array[i + rgba_channel[3]] != 0)
                                    {
                                        colour_palette[j + 1] = (byte)(BGRA_array[i + rgba_channel[2]] * 0.114 + BGRA_array[i + rgba_channel[1]] * 0.587 + BGRA_array[i + rgba_channel[0]] * 0.299);
                                    }
                                }
                                break;
                            }
                        case 1: // cie_709
                            {
                                for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
                                {
                                    colour_palette[j] = (BGRA_array[i + rgba_channel[3]]);  // alpha value
                                    if (BGRA_array[i + rgba_channel[3]] != 0)
                                    {
                                        colour_palette[j + 1] = (byte)(BGRA_array[i + rgba_channel[2]] * 0.0721 + BGRA_array[i + rgba_channel[1]] * 0.7154 + BGRA_array[i + rgba_channel[0]] * 0.2125);
                                    }
                                }
                                break;
                            }
                        case 2:  // custom
                            {
                                for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)  // process every pixel to fit the AAAA AAAA  CCCC CCCC  profile
                                {
                                    colour_palette[j] = (BGRA_array[i + rgba_channel[3]]);  // alpha value
                                    if (BGRA_array[i + rgba_channel[3]] != 0)
                                    {
                                        colour_palette[j + 1] = (byte)(BGRA_array[i + rgba_channel[2]] * custom_rgba[2] + BGRA_array[i + rgba_channel[1]] * custom_rgba[1] + BGRA_array[i + rgba_channel[0]] * custom_rgba[0]);
                                    }
                                }
                                break;
                            }
                    }
                    break;
                }

            case 1:  // RGB565
                {

                    switch (algorithm)
                    {
                        case 2:  // custom  RRRR RGGG GGGB BBBB
                            {
                                for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                {
                                    red = (byte)(BGRA_array[i + rgba_channel[0]] * custom_rgba[0]);
                                    green = (byte)(BGRA_array[i + rgba_channel[1]] * custom_rgba[1]);
                                    blue = (byte)(BGRA_array[i + rgba_channel[2]] * custom_rgba[2]);
                                    if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                    {
                                        red += 8;
                                    }
                                    if ((green & round6) == round6 && green < 252)  // 6-bit max value on a trimmed byte
                                    {
                                        green += 4;
                                    }
                                    if ((blue & 7) > round5 && blue < 248)
                                    {
                                        blue += 8;
                                    }
                                    // pixel = (ushort)((((byte)(red) >> 3) << 11) + (((byte)(green) >> 2) << 5) + (byte)(blue) >> 3);
                                    colour_palette[j] = (byte)((red & 0xf8) | (green >> 5));
                                    colour_palette[j + 1] = (byte)(((green << 3) & 0xe0) | (blue >> 3));
                                }
                                break;
                            }
                        default: // RRRR RGGG GGGB BBBB
                            {
                                for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                {
                                    red = BGRA_array[i + rgba_channel[0]];
                                    green = BGRA_array[i + rgba_channel[1]];
                                    blue = BGRA_array[i + rgba_channel[2]];
                                    if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                    {
                                        red += 8;
                                    }
                                    if ((green & round6) == round6 && green < 252)  // 6-bit max value on a trimmed byte
                                    {
                                        green += 4;
                                    }
                                    if ((blue & 7) > round5 && blue < 248)
                                    {
                                        blue += 8;
                                    }
                                    // pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3));
                                    colour_palette[j] = (byte)((red & 0xf8) | (green >> 5));
                                    colour_palette[j + 1] = (byte)(((green << 3) & 0xe0) | (blue >> 3));
                                }
                                break;
                            }
                    }
                    break;
                }
            case 2:  // RGB5A3
                {
                    switch (algorithm)
                    {
                        case 2:  // custom
                            {
                                if (alpha == 1)  // 0AAA RRRR GGGG BBBB
                                {
                                    for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                    {
                                        a = (byte)(BGRA_array[i + rgba_channel[3]] * custom_rgba[3]);
                                        red = (byte)(BGRA_array[i + rgba_channel[0]] * custom_rgba[0]);
                                        green = (byte)(BGRA_array[i + rgba_channel[1]] * custom_rgba[1]);
                                        blue = (byte)(BGRA_array[i + rgba_channel[2]] * custom_rgba[2]);
                                        if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                        {
                                            a += 32;
                                        }
                                        if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                        {
                                            red += 16;
                                        }
                                        if ((green & 15) > round4 && green < 240)
                                        {
                                            green += 16;
                                        }
                                        if ((blue & 15) > round4 && blue < 240)
                                        {
                                            blue += 16;
                                        }
                                        // pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                        colour_palette[j] = (byte)(((a >> 1) & 0x70) | (red >> 4));
                                        colour_palette[j + 1] = (byte)((green & 0xf0) | (blue >> 4));
                                    }
                                }
                                else if (alpha == 0)  // 1RRR RRGG GGGB BBBB
                                {
                                    for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                    {
                                        red = (byte)(BGRA_array[i + rgba_channel[0]] * custom_rgba[0]);
                                        green = (byte)(BGRA_array[i + rgba_channel[1]] * custom_rgba[1]);
                                        blue = (byte)(BGRA_array[i + rgba_channel[2]] * custom_rgba[2]);
                                        if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                        {
                                            red += 8;
                                        }
                                        if ((green & 7) > round5 && green < 248)
                                        {
                                            green += 8;
                                        }
                                        if ((blue & 7) > round5 && blue < 248)
                                        {
                                            blue += 8;
                                        }
                                        // pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                        colour_palette[j] = (byte)(0x80 | ((red >> 1) & 0x7C) | (green >> 6));
                                        colour_palette[j + 1] = (byte)(((green & 0x38) << 2) | (blue >> 3));
                                    }
                                }
                                else  // check for each colour if alpha trimmed to 3 bits is 255
                                {
                                    for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                    {
                                        a = (byte)(BGRA_array[i + rgba_channel[3]] * custom_rgba[3]);
                                        red = (byte)(BGRA_array[i + rgba_channel[0]] * custom_rgba[0]);
                                        green = (byte)(BGRA_array[i + rgba_channel[1]] * custom_rgba[1]);
                                        blue = (byte)(BGRA_array[i + rgba_channel[2]] * custom_rgba[2]);
                                        if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                        {
                                            a += 32;
                                        }
                                        if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                        {
                                            red += 16;
                                        }
                                        if ((green & 15) > round4 && green < 240)
                                        {
                                            green += 16;
                                        }
                                        if ((blue & 15) > round4 && blue < 240)
                                        {
                                            blue += 16;
                                        }
                                        if (a > 223)  // no alpha
                                        {
                                            colour_palette[j] = (byte)(0x80 | ((red >> 1) & 0x7C) | (green >> 6));
                                            colour_palette[j + 1] = (byte)(((green & 0x38) << 2) | (blue >> 3));
                                        }
                                        else
                                        {
                                            colour_palette[j] = (byte)(((a >> 1) & 0x70) | (red >> 4));
                                            colour_palette[j + 1] = (byte)((green & 0xf0) | (blue >> 4));
                                        }
                                    }
                                }
                                break;
                            }
                        default:
                            {
                                if (alpha == 1)  // 0AAA RRRR GGGG BBBB
                                {
                                    for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                    {
                                        a = BGRA_array[i + rgba_channel[3]];
                                        red = BGRA_array[i + rgba_channel[0]];
                                        green = BGRA_array[i + rgba_channel[1]];
                                        blue = BGRA_array[i + rgba_channel[2]];
                                        if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                        {
                                            a += 32;
                                        }
                                        if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                        {
                                            red += 16;
                                        }
                                        if ((green & 15) > round4 && green < 240)
                                        {
                                            green += 16;
                                        }
                                        if ((blue & 15) > round4 && blue < 240)
                                        {
                                            blue += 16;
                                        }
                                        // pixel = (ushort)(((a >> 5) << 12) + ((red >> 4) << 8) + ((green >> 4) << 4) + (blue >> 4));
                                        colour_palette[j] = (byte)(((a >> 1) & 0x70) | (red >> 4));
                                        colour_palette[j + 1] = (byte)((green & 0xf0) | (blue >> 4));
                                    }
                                }
                                else if (alpha == 0)  // 1RRR RRGG GGGB BBBB
                                {
                                    for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                    {
                                        red = BGRA_array[i + rgba_channel[0]];
                                        green = BGRA_array[i + rgba_channel[1]];
                                        blue = BGRA_array[i + rgba_channel[2]];
                                        if ((red & 7) > round5 && red < 248)  // 5-bit max value on a trimmed byte
                                        {
                                            red += 8;
                                        }
                                        if ((green & 7) > round5 && green < 248)
                                        {
                                            green += 8;
                                        }
                                        if ((blue & 7) > round5 && blue < 248)
                                        {
                                            blue += 8;
                                        }
                                        // pixel = (ushort)((1 << 15) + ((red >> 3) << 10) + ((green >> 3) << 5) + (blue >> 3));
                                        colour_palette[j] = (byte)(0x80 | ((red >> 1) & 0x7C) | (green >> 6));
                                        colour_palette[j + 1] = (byte)(((green & 0x38) << 2) | (blue >> 3));
                                    }
                                }
                                else  // mix up alpha and no alpha
                                {
                                    for (int i = pixel_start_offset, j = 0; i < array_size; i += 4, j += 2)
                                    {
                                        a = BGRA_array[i + rgba_channel[3]];
                                        red = BGRA_array[i + rgba_channel[0]];
                                        green = BGRA_array[i + rgba_channel[1]];
                                        blue = BGRA_array[i + rgba_channel[2]];
                                        if ((a & 31) > round3 && a < 224)  // 3-bit max value on a trimmed byte
                                        {
                                            a += 32;
                                        }
                                        if ((red & 15) > round4 && red < 240)  // 4-bit max value on a trimmed byte
                                        {
                                            red += 16;
                                        }
                                        if ((green & 15) > round4 && green < 240)
                                        {
                                            green += 16;
                                        }
                                        if ((blue & 15) > round4 && blue < 240)
                                        {
                                            blue += 16;
                                        }
                                        if (a > 223)  // no alpha
                                        {
                                            colour_palette[j] = (byte)(0x80 | ((red >> 1) & 0x7C) | (green >> 6));
                                            colour_palette[j + 1] = (byte)(((green & 0x38) << 2) | (blue >> 3));
                                        }
                                        else
                                        {
                                            colour_palette[j] = (byte)(((a >> 1) & 0x70) | (red >> 4));
                                            colour_palette[j + 1] = (byte)((green & 0xf0) | (blue >> 4));
                                        }
                                    }
                                }
                                break;
                            }
                    }
                    break;
                }
        }
        return colour_palette;
    }
}