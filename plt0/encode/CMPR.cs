using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

/* hmm, well. let's be honest. this is the harderest encoding to write, and the most efficient one
* I'll be directly storing sub-blocks here because the rgb565 values can't be added like that lol 

This is the DXT1 Compression BUT it has contitionnal alpha: in a nutshell it's this
if (colour1 > colour2)
{
    colour3 = colour1 * 2/3 + colour2 * 1/3;
    colour4 = colour1 * 1/3 + colour2 * 2/3;
}
else
{
    colour3 = colour1 * 1/2 + colour2 * 1/2;
    colour4 = fully transparent;
}

each block is 4 sub blocks
this is a sub-block structure. with 4x4 pixel and 2 rgb565 colours
RRRR  RGGG    GGGB  BBBB
RRRR  RGGG    GGGB  BBBB
II II II II   II II II II  - 2 bit index per pixel
II II II II   II II II II
II II II II   II II II II
II II II II   II II II II


how I will store each block: (1 is stored first)
16  15  12  11
14  13  10   9
8   7   4   3
6   5   2   1

how blocks are supposed to be stored:
1  2   5  6
3  4   7  8
9  10  13 14
11 12  15 16

the reason for this choice is conveniency, considering that a bmp file starts by the last line
*/
class CMPR_class
{
    Parse_args_class _plt0;
    public CMPR_class(Parse_args_class Parse_args_class)
    {
        _plt0 = Parse_args_class;
    }
    ushort alpha_bitfield = 0;
    List<ushort[]> Colour_list = new List<ushort[]>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
    List<ushort> colour_palette = new List<ushort>();
    List<ushort> Colour_rgb565 = new List<ushort>();  // won't be sorted
    byte red;
    byte green;
    byte blue;
    byte red2;
    byte green2;
    byte blue2;
    ushort pixel;
    byte[] index = new byte[8];  // sub block length
    static readonly byte[] full_alpha_index = { 0, 0, 0, 0, 255, 255, 255, 255 };
    byte[] rgb565 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  // 64 rgbx bytes, x is unused
    byte[] bmp_image;
    byte diff_min_index = 0;
    byte diff_max_index = 0;
    short diff_min = 500;
    short diff = 0;
    bool not_similar;
    byte c;
    ushort z = 0;
    int j = 0;
    int x = 0;
    int y = 0;
    ushort current_diff;
    ushort[] Colour_pixel = { 1, 0 };  // case 3
    ushort width = 0;
    ushort[] Colour_array = { 1, 0, 0 };  // default
    ushort diff_max;
    public void CMPR(List<byte[]> index_list, byte[] bmp_image_ref)
    {
        bmp_image = bmp_image_ref; // this should reference bmp_image to bmp_image_ref, not copy it.
        // byte[] Colour_count = {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };  // 16 pixels, because a block is 4x4
        // byte f;
        // int[] total_diff = {0, 0, 0};  // total_diff, e, f
        // List<int[]> diff_array = new List<int[]>();
        // List<byte> ef = new List<byte>();
        //byte swap;
        //byte swap2;
        // bool use__plt0.alpha = false;
        // bool done = false;
        switch (_plt0.algorithm)
        {
            case 1: // Wiimm
                // he's storing every colour of the block in a byte[4] inside a sum_t structure named sum
                // then he's making the interpolated colours for each colour of the 4x4 block and test
                // which combination is the best one by iterating over the 16 pixels and using calc_distance
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    red = bmp_image[y + _plt0.rgba_channel[0]];
                    green = bmp_image[y + _plt0.rgba_channel[1]];
                    blue = bmp_image[y + _plt0.rgba_channel[2]];
                    if (_plt0.alpha > 0 && bmp_image[y + _plt0.rgba_channel[3]] < _plt0.cmpr_alpha_threshold)
                    {
                        alpha_bitfield += (ushort)(1 << (j + (z * 4)));
                    }
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
                    rgb565[(z << 4) + (j << 2)] = red;
                    rgb565[(z << 4) + (j << 2) + 1] = green;
                    rgb565[(z << 4) + (j << 2) + 2] = blue;
                    // Colour_pixel[0] = // the number of occurences, though it stays to 1 so that's not really a problem lol
                    pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                    Colour_array[1] = pixel;
                    Colour_array[2] = (ushort)(red + green + blue); // best way to find darkest colour :D
                    Colour_list.Add(Colour_array.ToArray());
                    Colour_rgb565.Add(pixel);
                    j++;
                    if (j != 4)
                    {
                        continue;
                    }
                    j = 0;
                    z++;
                    y += (_plt0.canvas_width << 2) - 16; // returns to the start of the next line  - bitmap width << 2 because it's a 32-bit BGRA bmp file
                    if (z != 4)
                    {
                        continue;  // Still within the same 4x4 block
                    }
                    x++;
                    z = 0;
                    width += 2;  // triggered 4 times per block
                    if (width == _plt0.canvas_width)
                    {
                        width = 0;
                        // y -= (_plt0.bitmap_width << 1) - 16;  // this has been driving me nuts
                        y += (_plt0.canvas_width << 2) - 16;
                        x = 0;
                    }
                    else if (x == 2)
                    {
                        // y += (_plt0.bitmap_width << 4) - 4; // adds 4 lines and put the cursor back to the first block in width (I hope)
                        // y += 16; // hmm, it looks like the cursor warped horizontally to the first block in width 4 lines above
                        // EDIT: YA DEFINITELY NEED TO CANCEL THE Y OPERATION ABOVE, IT WARPS NORMALLY LIKE IT4S THE PIXEL AFTER
                        //y -= (_plt0.bitmap_width << 2) - 16;  // this has been driving me nuts
                        y += 16;  // I can't believe this is right in the mirror and mirrorred mode lol
                                  // edit: you just need to add 32 everywhere
                    }
                    else if (x == 4)
                    {
                        //y -= (_plt0.bitmap_width << 5) - 16; // minus 8 lines + point to next block
                        y -= (_plt0.canvas_width << 5) + 16;
                        x = 0;
                    }
                    else
                    {
                        /* y -= (_plt0.bitmap_width << 4) - 16; // on retire 4 lignes et on passe le 1er block héhé
                         substract 4 lines and jumps over the first block */
                        y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
                    }
                    if (alpha_bitfield == 0xffff)  // save computation time I guess
                    {
                        index_list.Add(full_alpha_index); // this byte won't be changed so no need to copy it
                        Colour_list.Clear();
                        colour_palette.Clear();
                        Colour_rgb565.Clear();
                        alpha_bitfield = 0;
                        continue;
                    }
                    // now let's take the colour couple with the best result inside the 4x4 block
                    diff_min = 0x7fff;
                    if (alpha_bitfield != 0)  // put the biggest ushort in second place
                    {
                        for (byte c = 0; c < 15; c++)  // useless to set it to 16 because of the condition c > i.
                        {
                            for (byte d = 0; d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                            {
                                colour_palette.Clear();
                                index[0] = (byte)(Colour_list[c][1] >> 8);
                                index[1] = (byte)(Colour_list[c][1]);
                                index[2] = (byte)(Colour_list[d][1] >> 8);
                                index[3] = (byte)(Colour_list[d][1]);
                                colour_palette.Add(Colour_list[c][1]);
                                colour_palette.Add(Colour_list[c][1]);
                                red = (byte)(rgb565[(c << 2)] + rgb565[(d << 2)]) / 2);
                                green = (byte)(((((index[0] & 7) << 5) + ((index[1] >> 3) & 28)) + (((index[2] & 7) << 5) + ((index[3] >> 3) & 28))) / 2);
                                blue = (byte)((((index[1] << 3) & 248) + ((index[3] << 3) & 248)) / 2);
                                colour_palette.Add((ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)));  // the RGB565 third colour
                                                                                                                       // last colour isn't in the palette, it's in _plt0.alpha_bitfield

                                diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)
                                for (byte i = 0; i < 16; i++)
                                {
                                    diff += Find_Nearest_Colour();
                                }
                            }
                        }
                    }
                    for (byte i = 0; i < 16; i++)  // useless to set it to 16 because of the condition k > i.
                    {

                        // put biggest ushort in first place
                        {
                            // of course, that's the exact opposite!
                            if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the first spot
                            {
                                index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                                index[1] = (byte)(Colour_list[diff_min_index][1]);
                                index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                                index[3] = (byte)(Colour_list[diff_max_index][1]);
                                colour_palette.Add(Colour_list[diff_min_index][1]);
                                colour_palette.Add(Colour_list[diff_max_index][1]);
                            }
                            else
                            {
                                index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                                index[1] = (byte)(Colour_list[diff_max_index][1]);
                                index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                                index[3] = (byte)(Colour_list[diff_min_index][1]);
                                colour_palette.Add(Colour_list[diff_max_index][1]);
                                colour_palette.Add(Colour_list[diff_min_index][1]);
                            }

                            red = (byte)(index[0] & 248);
                            green = (byte)(((index[0] & 7) << 5) + ((index[1] >> 3) & 28));
                            blue = (byte)((index[1] << 3) & 248);

                            red2 = (byte)(index[2] & 248);
                            green2 = (byte)(((index[2] & 7) << 5) + ((index[3] >> 3) & 28));
                            blue2 = (byte)((index[3] << 3) & 248);

                            pixel = (ushort)(((((red * 2 / 3) + (red2 / 3)) >> 3) << 11) + ((((green * 2 / 3) + (green2 / 3)) >> 2) << 5) + (((blue * 2 / 3) + (blue2 / 3)) >> 3));
                            colour_palette.Add(pixel);  // the RGB565 third colour
                            pixel = (ushort)(((((red / 3) + (red2 * 2 / 3)) >> 3) << 11) + ((((green / 3) + (green2 * 2 / 3)) >> 2) << 5) + (((blue / 3) + (blue2 * 2 / 3)) >> 3));
                            colour_palette.Add(pixel);  // the RGB565 fourth colour
                        }
                    }
                    Colour_list.Sort(new UshortArrayComparer());  // sorts the table by the most used colour first
                                                                  //now let's take the darkest and the brightest colour from the cmpr_max most used ones
                    diff_min = 1024;
                    diff_max = 0;
                    for (byte i = 0; i < _plt0.cmpr_max && Colour_list[i][0] != 0; i++)
                    {
                        if (Colour_list[i][2] < diff_min)
                        {
                            diff_min = (short)(Colour_list[i][2]);
                            diff_min_index = i;
                        }
                        if (Colour_list[i][2] > diff_max)
                        {
                            diff_max = Colour_list[i][2];
                            diff_max_index = i;
                        }
                    }
                    Organize_Colours_And_Process_Indexes();
                    index_list.Add(index.ToArray());
                }
                break;
            case 2: // SuperBMD
                // SuperBMD is calculating the distance between a pixel and his next neighbour in the 4x4 block, and the couple with the max distance is chosen as the two colours
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block())
                        continue;
                    // now let's take the darkest and the brightest colour but only by going through neighbours (that's SuperBMD algorithm)
                    diff_max = 0;
                    for (byte i = 0; i < 15; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 0)
                        {
                            current_diff = (ushort)Math.Abs(Colour_list[i][2] - Colour_list[i + 1][2]);
                            if (current_diff > diff_max)
                            {
                                diff_max = current_diff;
                                diff_max_index = i;  // diff_max_index  =  second colour
                                diff_min_index = (byte)(i + 1); // diff_min_index = first colour
                            }
                        }
                    }
                    Organize_Colours_And_Process_Indexes();
                    index_list.Add(index.ToArray());
                }
                break;
            case 3:  // most used colours with _plt0.diversity - no gradient - similar - looks pixelated
                {
                    for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                    {
                        if (!Load_Block())
                            continue;
                        // now let's just try to take the most two used colours and use _plt0.diversity I guess
                        // implementing my own way to find most used colours
                        // let's count the number of exact same colours in Colour_list
                        for (int i = 0; i < 15; i++)  // useless to set it to 16 because of the condition k > i.
                        {
                            for (int k = 0; k < 16; k++)
                            {
                                if (k == i)
                                {
                                    continue;
                                }
                                if (Colour_list[k][1] == Colour_list[i][1] && k > i && ((alpha_bitfield >> k) & 1) == 0 && ((alpha_bitfield >> i) & 1) == 0)  // k > i prevents colours occurences from being added twice.
                                {
                                    Colour_list[k][0]++;
                                    Colour_list[i][0] = 0; // should set it to zero.
                                }
                            }
                        }
                        Colour_list.Sort(new UshortArrayComparer());  // sorts the table by the most used colour first
                        c = 0;
                        for (int i = 0; i < 16 && c < 4; i++)  // build the colour table with the two most used colours and _plt0.diversity
                        {
                            not_similar = true;
                            if (Colour_list[i][0] / 16 < _plt0.percentage / 100)
                            {
                                // break;  // THIS IS BREAKING THE LOOP
                                continue;
                            }
                            if (c == 2)  // checks for _plt0.diversity before adding the second colour ^^
                            {
                                if (Math.Abs((index[0] & 248) - ((Colour_list[i][1] >> 8) & 248)) < _plt0.diversity && Math.Abs(((index[0] & 7) << 5) + ((index[1] >> 3) & 28) - ((Colour_list[i][1] >> 3) & 252)) < _plt0.diversity && Math.Abs(((index[1] << 3) & 248) - (Colour_list[i][1] << 3) & 248) < _plt0.diversity)
                                {
                                    not_similar = false;
                                    // break;  // EGAD YOU4VE BROKEN THE LOOP
                                    continue;
                                }
                            }
                            if (not_similar)
                            {
                                index[c] = (byte)(Colour_list[i][1] >> 8);  // adds the RRRR RGGG value
                                index[c + 1] = (byte)(Colour_list[i][1]);  // adds the GGGB BBBB value
                                c += 2;
                            }
                        }
                        if (c < 4) // if the colour palette is not full
                        {
                            // Console.WriteLine("The colour palette was not full, starting second loop...\n");

                            for (int i = 0; i < 16 && c < 4; i++)
                            {
                                not_similar = true;
                                if (Colour_list[i][0] / 16 < _plt0.percentage2 / 100)
                                {
                                    continue;
                                }
                                if (c == 2)  // checks for _plt0.diversity before adding the second colour ^^
                                {
                                    if (Math.Abs((index[0] & 248) - ((Colour_list[i][1] >> 8) & 248)) < _plt0.diversity2 && Math.Abs(((index[0] & 7) << 5) + ((index[1] >> 3) & 28) - ((Colour_list[i][1] >> 3) & 252)) < _plt0.diversity2 && Math.Abs(((index[1] << 3) & 248) - (Colour_list[i][1] << 3) & 248) < _plt0.diversity2)
                                    {
                                        not_similar = false;
                                        continue;
                                    }
                                }
                                if (not_similar)
                                {
                                    index[c] = (byte)(Colour_list[i][1] >> 8);  // adds the RRRR RGGG value
                                    index[c + 1] = (byte)(Colour_list[i][1]);  // adds the GGGB BBBB value
                                    c += 2;
                                }
                            }
                            if (c < 4) // if the colour palette is still not full
                            {
                                // Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                for (int i = 0; i < 16 && c < 4; i++)
                                {
                                    not_similar = true;
                                    if (c == 2)
                                    {
                                        if ((index[0] == (byte)(Colour_list[i][1] >> 8)) && index[1] == (byte)(Colour_list[i][1]))
                                        {
                                            not_similar = false;
                                            continue;
                                        }
                                    }
                                    if (not_similar)
                                    {
                                        index[c] = (byte)(Colour_list[i][1] >> 8);  // adds the RRRR RGGG value
                                        index[c + 1] = (byte)(Colour_list[i][1]);  // adds the GGGB BBBB value
                                        c += 2;
                                    }
                                }
                            }
                        }
                        Organize_Colours_And_Process_Indexes();
                        index_list.Add(index.ToArray());
                    }
                }
                break;

            case 4: // darkest/lightest
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block())
                        continue;
                    // now let's take the darkest and the brightest colour
                    diff_min = 1024;
                    diff_max = 0;
                    for (byte i = 0; i < 16; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 0 || Colour_list[i][0] <= _plt0.cmpr_max)
                            continue;
                        if (Colour_list[i][2] < diff_min)
                        {
                            diff_min = (short)(Colour_list[i][2]);
                            diff_min_index = i;  // darkest colour index of Colour_RGB565
                        }
                        if (Colour_list[i][2] > diff_max)
                        {
                            diff_max = Colour_list[i][2];
                            diff_max_index = i;  // brightest colour index of Colour_RGB565
                        }
                    }
                    Organize_Colours_And_Process_Indexes();
                    index_list.Add(index.ToArray());
                }
                break;
        }
    }
    /*
     * t = (pixel_posN - pixel_pos1) / (pixel_pos2 - pixel_pos1)
       pixelN_red = (t-1)*pixel1_red + (t)*pixel2_red
       same for blue + green*/
    private int Find_Nearest_Colour()
    {

    }
    private bool Load_Block()
    {
        red = bmp_image[y + _plt0.rgba_channel[0]];
        green = bmp_image[y + _plt0.rgba_channel[1]];
        blue = bmp_image[y + _plt0.rgba_channel[2]];
        if (_plt0.alpha > 0 && bmp_image[y + _plt0.rgba_channel[3]] < _plt0.cmpr_alpha_threshold)
        {
            alpha_bitfield += (ushort)(1 << (j + (z * 4)));
        }
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
        // Colour_pixel[0] = // the number of occurences, though it stays to 1 so that's not really a problem lol
        pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
        Colour_array[1] = pixel;
        Colour_array[2] = (ushort)(red + green + blue); // best way to find darkest colour :D
        Colour_list.Add(Colour_array.ToArray());
        Colour_rgb565.Add(pixel);
        j++;
        if (j != 4)
        {
            return false;
        }
        j = 0;
        z++;
        y += (_plt0.canvas_width << 2) - 16; // returns to the start of the next line  - bitmap width << 2 because it's a 32-bit BGRA bmp file
        if (z != 4)
        {
            return false;  // Still within the same 4x4 block
        }
        x++;
        z = 0;
        width += 2;  // triggered 4 times per block
        if (width == _plt0.canvas_width)
        {
            width = 0;
            // y -= (_plt0.bitmap_width << 1) - 16;  // this has been driving me nuts
            y += (_plt0.canvas_width << 2) - 16;
            x = 0;
        }
        else if (x == 2)
        {
            // y += (_plt0.bitmap_width << 4) - 4; // adds 4 lines and put the cursor back to the first block in width (I hope)
            // y += 16; // hmm, it looks like the cursor warped horizontally to the first block in width 4 lines above
            // EDIT: YA DEFINITELY NEED TO CANCEL THE Y OPERATION ABOVE, IT WARPS NORMALLY LIKE IT4S THE PIXEL AFTER
            //y -= (_plt0.bitmap_width << 2) - 16;  // this has been driving me nuts
            y += 16;  // I can't believe this is right in the mirror and mirrorred mode lol
                      // edit: you just need to add 32 everywhere
        }
        else if (x == 4)
        {
            //y -= (_plt0.bitmap_width << 5) - 16; // minus 8 lines + point to next block
            y -= (_plt0.canvas_width << 5) + 16;
            x = 0;
        }
        else
        {
            /* y -= (_plt0.bitmap_width << 4) - 16; // on retire 4 lignes et on passe le 1er block héhé
             substract 4 lines and jumps over the first block */
            y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
        }
        return true;
    }
    private void Organize_Colours_And_Process_Indexes()
    {
        if (alpha_bitfield != 0)  // put the biggest ushort in second place
        {
            if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the second spot
            {
                index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                index[1] = (byte)(Colour_list[diff_max_index][1]);
                index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                index[3] = (byte)(Colour_list[diff_min_index][1]);
                colour_palette.Add(Colour_list[diff_max_index][1]);
                colour_palette.Add(Colour_list[diff_min_index][1]);
            }
            else
            {
                index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                index[1] = (byte)(Colour_list[diff_min_index][1]);
                index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                index[3] = (byte)(Colour_list[diff_max_index][1]);
                colour_palette.Add(Colour_list[diff_min_index][1]);
                colour_palette.Add(Colour_list[diff_max_index][1]);
            }
            red = (byte)(((index[0] & 248) + (index[2] & 248)) / 2);
            green = (byte)(((((index[0] & 7) << 5) + ((index[1] >> 3) & 28)) + (((index[2] & 7) << 5) + ((index[3] >> 3) & 28))) / 2);
            blue = (byte)((((index[1] << 3) & 248) + ((index[3] << 3) & 248)) / 2);
            colour_palette.Add((ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)));  // the RGB565 third colour
                                                                                                   // last colour isn't in the palette, it's in _plt0.alpha_bitfield
        }
        else  // put biggest ushort in first place
        {
            // of course, that's the exact opposite!
            if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the first spot
            {
                index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                index[1] = (byte)(Colour_list[diff_min_index][1]);
                index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                index[3] = (byte)(Colour_list[diff_max_index][1]);
                colour_palette.Add(Colour_list[diff_min_index][1]);
                colour_palette.Add(Colour_list[diff_max_index][1]);
            }
            else
            {
                index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                index[1] = (byte)(Colour_list[diff_max_index][1]);
                index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                index[3] = (byte)(Colour_list[diff_min_index][1]);
                colour_palette.Add(Colour_list[diff_max_index][1]);
                colour_palette.Add(Colour_list[diff_min_index][1]);
            }

            red = (byte)(index[0] & 248);
            green = (byte)(((index[0] & 7) << 5) + ((index[1] >> 3) & 28));
            blue = (byte)((index[1] << 3) & 248);

            red2 = (byte)(index[2] & 248);
            green2 = (byte)(((index[2] & 7) << 5) + ((index[3] >> 3) & 28));
            blue2 = (byte)((index[3] << 3) & 248);

            pixel = (ushort)(((((red * 2 / 3) + (red2 / 3)) >> 3) << 11) + ((((green * 2 / 3) + (green2 / 3)) >> 2) << 5) + (((blue * 2 / 3) + (blue2 / 3)) >> 3));
            colour_palette.Add(pixel);  // the RGB565 third colour
            pixel = (ushort)(((((red / 3) + (red2 * 2 / 3)) >> 3) << 11) + ((((green / 3) + (green2 * 2 / 3)) >> 2) << 5) + (((blue / 3) + (blue2 * 2 / 3)) >> 3));
            colour_palette.Add(pixel);  // the RGB565 fourth colour
        }
        // time to get the "linear interpolation to add third and fourth colour
        // CI2 if that's a name lol
        for (byte i = 4; i < 8; i++)
        {
            index[i] = 0;
        }
        if (_plt0.reverse_x)
        {
            for (sbyte h = 3; h >= 0; h--)
            {
                for (sbyte w = 3; w >= 0; w--)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h * 4) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (w << 1));
                        continue;
                    }
                    diff_min = 500;
                    // diff_min_index = w;
                    for (byte i = 0; i < colour_palette.Count; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    {
                        if (colour_palette[i] == Colour_rgb565[(h * 4) + w])  // if it's the exact same colour
                        {
                            diff_min_index = i;  // index is stored on 1 byte, while each colour is stored on 2 bytes
                            break;
                        }
                        else  // calculate difference between each separate colour channel and store the sum
                        {
                            diff = (short)(Math.Abs(((colour_palette[i] >> 8) & 248) - ((Colour_rgb565[(h * 4) + w] >> 8) & 248)) + Math.Abs(((colour_palette[i] >> 3) & 252) - ((Colour_rgb565[(h * 4) + w] >> 3) & 252)) + Math.Abs(((colour_palette[i] << 3) & 248) - ((Colour_rgb565[(h * 4) + w] << 3) & 248)));
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                    }
                    index[7 - h] += (byte)(diff_min_index << (w << 1));
                }
            }
            if (_plt0.reverse_y)  // swap
            {
                red = index[7];
                green = index[6];  // it's not the green colour. It's rather a seal
                index[7] = index[4];
                index[6] = index[5];
                index[5] = green;
                index[4] = red;
            }
        }
        else if (_plt0.reverse_y)
        {
            //for (sbyte h = 3; h >= 0; h--)
            for (byte h = 0; h < 4; h++)
            {
                for (byte w = 0; w < 4; w++)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h * 4) + w) & 1) == 1)
                    {
                        index[4 + h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 500;
                    // diff_min_index = w;
                    for (byte i = 0; i < colour_palette.Count; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    {
                        if (colour_palette[i] == Colour_rgb565[(h * 4) + w])  // if it's the exact same colour
                        {
                            diff_min_index = i;  // index is stored on 1 byte, while each colour is stored on 2 bytes
                            break;
                        }
                        else  // calculate difference between each separate colour channel and store the sum
                        {
                            diff = (short)(Math.Abs(((colour_palette[i] >> 8) & 248) - ((Colour_rgb565[(h * 4) + w] >> 8) & 248)) + Math.Abs(((colour_palette[i] >> 3) & 252) - ((Colour_rgb565[(h * 4) + w] >> 3) & 252)) + Math.Abs(((colour_palette[i] << 3) & 248) - ((Colour_rgb565[(h * 4) + w] << 3) & 248)));
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                    }
                    index[4 + h] += (byte)(diff_min_index << (6 - (w << 1)));
                    // Console.WriteLine(index[4 + h]);
                }
            }
        }
        else
        {
            for (sbyte h = 3; h >= 0; h--)
            //for (byte h = 0; h < 4; h++)
            {
                for (byte w = 0; w < 4; w++)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h * 4) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 500;
                    // diff_min_index = w;
                    for (byte i = 0; i < colour_palette.Count; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    {
                        if (colour_palette[i] == Colour_rgb565[(h * 4) + w])  // if it's the exact same colour
                        {
                            diff_min_index = i;  // index is stored on 1 byte, while each colour is stored on 2 bytes
                            break;
                        }
                        else  // calculate difference between each separate colour channel and store the sum
                        {
                            diff = (short)(Math.Abs(((colour_palette[i] >> 8) & 248) - ((Colour_rgb565[(h * 4) + w] >> 8) & 248)) + Math.Abs(((colour_palette[i] >> 3) & 252) - ((Colour_rgb565[(h * 4) + w] >> 3) & 252)) + Math.Abs(((colour_palette[i] << 3) & 248) - ((Colour_rgb565[(h * 4) + w] << 3) & 248)));
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                    }
                    index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                    // Console.WriteLine(index[4 + h]);
                }
            }
        }
        // index is overwritten each time
        // the lists need to be cleaned
        Colour_list.Clear();
        colour_palette.Clear();
        Colour_rgb565.Clear();
        alpha_bitfield = 0;
    }
}