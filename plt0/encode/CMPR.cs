using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
    // List<ushort> colour_palette = new List<ushort>();
    List<ushort[]> Colour_rgb565 = new List<ushort[]>();  // won't be sorted
    byte red;
    byte green;
    byte blue;
    byte red2;
    byte green2;
    byte blue2;
    byte red_min;
    byte green_min;
    byte blue_min;
    byte red_max;
    byte green_max;
    byte blue_max;
    byte palette_length;
    ushort pixel;
    byte[] index = new byte[8];  // sub block length
    static readonly byte[] full_alpha_index = { 0, 0, 0, 0, 255, 255, 255, 255 };
    byte[] rgb565 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  // 64 rgbx bytes, x is unused
    byte[] bmp_image;
    byte[] palette_rgba = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    byte diff_min_index = 0;
    byte diff_max_index = 0;
    ushort diff_min = 1024;
    ushort diff = 0;
    ushort diff_min0;
    ushort diff0;
    bool not_similar;
    bool weemm_algorithm = false;
    byte c;
    ushort z = 0;
    int j = 0;
    int x = 0;
    int y = 0;
    ushort current_diff;
    ushort[] Colour_pixel = { 1, 0 };  // No Gradient
    ushort width = 0;
    ushort[] Colour_array = { 1, 0, 0 };  // default value of Colour_List
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
            case 0: // default: used to re-encode
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block_CIE_709())
                        continue;
                    // now let's take the darkest and the brightest colour
                    diff_min = 1024;
                    diff_max = 0;
                    for (byte i = 0; i < 16; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 1 || Colour_list[i][0] <= _plt0.cmpr_max)
                            continue;
                        if (Colour_list[i][2] < diff_min)
                        {
                            diff_min = Colour_list[i][2];
                            diff_min_index = i;  // darkest colour index of Colour_RGB565
                        }
                        if (Colour_list[i][2] > diff_max)
                        {
                            diff_max = Colour_list[i][2];
                            diff_max_index = i;  // brightest colour index of Colour_RGB565
                        }
                    }
                    Organize_Colours();
                    Process_Indexes();
                    index_list.Add(index.ToArray());
                }
                break;
            case 1: // Range Fit -   //https://stackoverflow.com/questions/24747643/3d-linear-regression
                // first, process the red_array, then do the same thing for green and blue.
                byte[] covariance = new byte[256];
                short[] red_array = new short[16];
                short sum = 0;
                short average = 0;
                bool skipped;
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block_rgb565())
                        continue;
                    skipped = false;
                    diff_max = 16; // the number of colours in the array - used to remove transparent pixels from the regression calculation
                    // now let's make the "principal axis", by using 3d linear regression
                    // fill red_array values
                    for (byte i = 0; i < 16; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 1 || Colour_list[i][0] <= _plt0.cmpr_max)
                        {
                            diff_max -= 1;
                            red_array[i] = 0x7fff;
                            continue;
                        }
                        red_array[i] = rgb565[i << 2];
                        sum += rgb565[i << 2];
                    }
                    // calculate average
                    if (diff_max == 16)
                        average = (short)(sum >> 4); // divides by 16 to calculate the average
                    else
                        average = (short)(sum / diff_max); // average with skipped pixels
                    diff = 0;
                    for (byte i = 0; i < 15; i++)  // discard skipped pixels
                    {
                        if (red_array[i] == 0x7fff)
                        {
                            diff += 1;
                            skipped = true;
                            for (; red_array[diff] == 0x7fff; diff++)
                            { }
                        }
                        if (skipped)
                            red_array[i] = red_array[i + diff];
                    }
                    // Normalize ( this means calculate the distance to the average)
                    for (byte i = 0; i < diff_max; i++)
                    {
                        red_array[i] -= average;
                    }
                    // Calculate to covariance matrix
                    // Sigma = (1 / 16) * X^T * X     X^T = transposed X matrix
                    for (byte u = 0; u < diff_max; u++)
                    {
                        for (byte v = 0; v < diff_max; v++)
                        {
                            covariance[(u << 4) + v] = (byte)(red_array[u] * red_array[v]);  // yup, that's a matrix product
                        }
                    }
                    // covariance[255] = (byte)(red_array[diff_max] * red_array[diff_max]);
                    // Singular Value Decomposition.
                    // Factorizes the matrix a into two unitary matrices U and Vh, and a 1 - D array s of singular values(real, non - negative) such that
                    // a == U @ S @ Vh, where S is a suitably shaped matrix of zeros with main diagonal s.
                    //MathNet.Numerics.LinearAlgebra svd = new MathNet.Numerics.Linearalgebra();
                    //svd.Factorization.DenseSvd(covariance);
                    Organize_Colours();
                    Process_Indexes();
                    index_list.Add(index.ToArray());
                }
                break;
            case 2: // Most Used/Furthest
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block_rgb565())
                        continue;
                    Colour_rgb565 = Colour_list.ToList();
                    // implementing my own way to find most used colours
                    // let's count the number of exact same colours in Colour_list
                    for (int i = 0; i < 16; i++)  // useless to set it to 16 because of the condition k > i.
                    {
                        for (int k = (byte)(i + 1); k < 16; k++)
                        {
                            if (Colour_list[k][1] == Colour_list[i][1] && ((alpha_bitfield >> k) & 1) == 0 && ((alpha_bitfield >> i) & 1) == 0)  // k > i prevents colours occurences from being added twice.
                            {
                                Colour_list[k][0]++;
                                Colour_list[i][0] = 0; // should set it to zero.
                            }
                        }
                    }
                    Colour_list.Sort(new UshortArrayComparer());  // sorts the table by the most used colour first
                    diff_min_index = 0;
                    diff_min = 1024;
                    for (byte i = 0; i < 15; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 1)
                            continue;
                        this.diff = (ushort)(rgb565[Colour_list[diff_min_index][2]] + rgb565[Colour_list[diff_min_index][2] + 1] + rgb565[Colour_list[diff_min_index][2] + 2]);
                        if (this.diff < diff_min)
                        {
                            diff_min = this.diff;
                            diff_min_index = i;
                        }
                        if (Colour_list[i][0] != Colour_list[i + 1][0])
                            break;
                    }
                    // now let's take the colour the furthest away from it
                    diff_max = 0;
                    for (byte i = 0; i < 16; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 1 || Colour_rgb565[i][0] <= _plt0.cmpr_max)
                            continue;
                        this.diff = (ushort)(Math.Abs(rgb565[Colour_list[diff_min_index][2]] - rgb565[i << 2]) + Math.Abs(rgb565[Colour_list[diff_min_index][2] + 1] - rgb565[(i << 2) + 1]) + Math.Abs(rgb565[Colour_list[diff_min_index][2] + 2] - rgb565[(i << 2) + 2]));
                        if (this.diff > diff_max)
                        {
                            diff_max = this.diff;
                            diff_max_index = (byte)(i << 2);  // brightest colour index of Colour_RGB565
                        }
                    }
                    Colour_list[15][1] = (ushort)(rgb565[diff_max_index] << 8 | rgb565[diff_max_index + 1] << 3 | rgb565[diff_max_index + 2] >> 3);
                    diff_max_index = 15;
                    /* to me, the two lines above are faster than this loop.
                     * for (byte i = 0; i < 16; i++)
                    {
                        if (diff_max_index == Colour_list[i][2])
                        {

                            diff_max_index = i;
                            break;
                        }
                    }*/
                    Organize_Colours();
                    Process_Indexes();
                    index_list.Add(index.ToArray());
                }
                break;
            case 3: // darkest/lightest
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block_Darkest())
                        continue;
                    // now let's take the darkest and the brightest colour
                    diff_min = 1024;
                    diff_max = 0;
                    for (byte i = 0; i < 16; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 1 || Colour_list[i][0] <= _plt0.cmpr_max)
                            continue;
                        if (Colour_list[i][2] < diff_min)
                        {
                            diff_min = Colour_list[i][2];
                            diff_min_index = i;  // darkest colour index of Colour_RGB565
                        }
                        if (Colour_list[i][2] > diff_max)
                        {
                            diff_max = Colour_list[i][2];
                            diff_max_index = i;  // brightest colour index of Colour_RGB565
                        }
                    }
                    Organize_Colours();
                    Process_Indexes();
                    index_list.Add(index.ToArray());
                }
                break;
            case 4:  // most used colours with _plt0.diversity - no gradient - similar - looks pixelated
                {
                    for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                    {
                        if (!Load_Block_Darkest())
                            continue;
                        // now let's just try to take the most two used colours and use _plt0.diversity I guess
                        // implementing my own way to find most used colours
                        // let's count the number of exact same colours in Colour_list
                        for (int i = 0; i < 16; i++)  // useless to set it to 16 because of the condition k > i.
                        {
                            for (int k = (byte)(i + 1); k < 16; k++)
                            {
                                if (Colour_list[k][1] == Colour_list[i][1] && ((alpha_bitfield >> k) & 1) == 0 && ((alpha_bitfield >> i) & 1) == 0)  // k > i prevents colours occurences from being added twice.
                                {
                                    Colour_list[k][0]++;
                                    Colour_list[i][0] = 0; // should set it to zero.
                                }
                            }
                        }
                        Colour_list.Sort(new UshortArrayComparer());  // sorts the table by the most used colour first
                        c = 0;
                        for (byte i = 0; i < 16 && c < 2; i++)  // build the colour table with the two most used colours and _plt0.diversity
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
                                diff_min_index = i;  // adds the value
                                c += 1;
                            }
                        }
                        if (c < 4) // if the colour palette is not full
                        {
                            // Console.WriteLine("The colour palette was not full, starting second loop...\n");

                            for (byte i = 0; i < 16 && c < 2; i++)
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
                                    diff_min_index = i;  // adds the value
                                    c += 1;
                                }
                            }
                            if (c < 4) // if the colour palette is still not full
                            {
                                // Console.WriteLine("The colour palette is not full, this program will fill it with the most used colours\n");
                                for (byte i = 0; i < 16 && c < 2; i++)
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
                                        diff_min_index = i;  // adds the value
                                        c += 1;
                                    }
                                }
                            }
                        }
                        Organize_Colours();
                        Process_Indexes();
                        index_list.Add(index.ToArray());
                    }
                }
                break;
            case 5: // Wiimm
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
                    rgb565[(z << 4) + (j << 2)] = (byte)(red & 0xf8);
                    rgb565[(z << 4) + (j << 2) + 1] = (byte)(green & 0xfc);
                    rgb565[(z << 4) + (j << 2) + 2] = (byte)(blue & 0xf8);
                    // Colour_pixel[0] = // the number of occurences, though it stays to 1 so that's not really a problem lol
                    pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                    Colour_array[1] = pixel;
                    // it's not used here
                    // Colour_array[2] = (ushort)(red + green + blue); // best way to find darkest colour :D
                    Colour_list.Add(Colour_array.ToArray());
                    // Colour_rgb565.Add(pixel);
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
                        //colour_palette.Clear();
                        // Colour_rgb565.Clear();
                        alpha_bitfield = 0;
                        continue;
                    }
                    // now let's take the colour couple with the best result inside the 4x4 block
                    diff_min = 0xffff;
                    if (alpha_bitfield != 0)  // put the biggest ushort in second place
                    {
                        for (byte c = 0; c < 16; c++)  // useless to set it to 16 because of the condition c > i.
                        {
                            for (byte d = (byte)(c + 1); d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                            {
                                if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                                    continue;
                                //colour_palette.Clear();
                                index[0] = (byte)(Colour_list[c][1] >> 8);
                                index[1] = (byte)(Colour_list[c][1]);
                                index[2] = (byte)(Colour_list[d][1] >> 8);
                                index[3] = (byte)(Colour_list[d][1]);
                                palette_rgba[0] = (byte)(index[0] & 248);
                                palette_rgba[1] = (byte)(((index[0] & 7) << 5) + ((index[1] >> 3) & 28));
                                palette_rgba[2] = (byte)((index[1] << 3) & 248);

                                palette_rgba[4] = (byte)(index[2] & 248);
                                palette_rgba[5] = (byte)(((index[2] & 7) << 5) + ((index[3] >> 3) & 28));
                                palette_rgba[6] = (byte)((index[3] << 3) & 248);

                                palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                                palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                                palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);

                                palette_length = 3;
                                //colour_palette.Add(Colour_list[c][1]);
                                //colour_palette.Add(Colour_list[d][1]);
                                //red = (byte)((rgb565[c << 2] + rgb565[d << 2]) / 2);
                                //green = (byte)((rgb565[(c << 2) + 1] + rgb565[(d << 2) + 1]) / 2);
                                //blue = (byte)((rgb565[(c << 2) + 2] + rgb565[(d << 2) + 2]) / 2);
                                //colour_palette.Add((ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)));  // the RGB565 third colour
                                // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                                this.diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)
                                for (byte i = 0; i < 16; i++)
                                {
                                    this.diff += Find_Nearest_Colour(i);
                                }
                                if (this.diff < diff_min)
                                {
                                    diff_min = this.diff;
                                    diff_min_index = c;
                                    diff_max_index = d;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (byte c = 0; c < 16; c++)  // useless to set it to 16 because of the condition c > i.
                        {
                            for (byte d = (byte)(c + 1); d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                            {
                                if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                                    continue;
                                index[0] = (byte)(Colour_list[c][1] >> 8);
                                index[1] = (byte)(Colour_list[c][1]);
                                index[2] = (byte)(Colour_list[d][1] >> 8);
                                index[3] = (byte)(Colour_list[d][1]);
                                palette_rgba[0] = (byte)(index[0] & 248);
                                palette_rgba[1] = (byte)(((index[0] & 7) << 5) + ((index[1] >> 3) & 28));
                                palette_rgba[2] = (byte)((index[1] << 3) & 248);

                                palette_rgba[4] = (byte)(index[2] & 248);
                                palette_rgba[5] = (byte)(((index[2] & 7) << 5) + ((index[3] >> 3) & 28));
                                palette_rgba[6] = (byte)((index[3] << 3) & 248);

                                palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                                palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                                palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3));

                                palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                                palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                                palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3));
                                palette_length = 4;
                                this.diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)
                                for (byte i = 0; i < 16; i++)
                                {
                                    this.diff += Find_Nearest_Colour(i);
                                }
                                if (this.diff < diff_min)
                                {
                                    diff_min = this.diff;
                                    diff_min_index = c;
                                    diff_max_index = d;
                                    weemm_algorithm = false;
                                }
                            }
                        }
                        for (byte c = 0; c < 16; c++)
                        {
                            for (byte d = (byte)(c + 1); d < 16; d++)
                            {
                                if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                                    continue;
                                index[0] = (byte)(Colour_list[c][1] >> 8);
                                index[1] = (byte)(Colour_list[c][1]);
                                index[2] = (byte)(Colour_list[d][1] >> 8);
                                index[3] = (byte)(Colour_list[d][1]);
                                palette_rgba[0] = (byte)(index[0] & 248);
                                palette_rgba[1] = (byte)(((index[0] & 7) << 5) + ((index[1] >> 3) & 28));
                                palette_rgba[2] = (byte)((index[1] << 3) & 248);

                                palette_rgba[4] = (byte)(index[2] & 248);
                                palette_rgba[5] = (byte)(((index[2] & 7) << 5) + ((index[3] >> 3) & 28));
                                palette_rgba[6] = (byte)((index[3] << 3) & 248);

                                palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                                palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                                palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);

                                palette_length = 3;
                                // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                                this.diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)
                                for (byte i = 0; i < 16; i++)
                                {
                                    this.diff += Find_Nearest_Colour(i);
                                }
                                if (this.diff < diff_min)
                                {
                                    diff_min = this.diff;
                                    diff_min_index = c;
                                    diff_max_index = d;
                                    weemm_algorithm = true;
                                }
                            }
                        }
                    }
                    Organize_Colours();
                    Process_Indexes();
                    index_list.Add(index.ToArray());
                }
                break;
            case 6: // SuperBMD
                // SuperBMD is calculating the distance between a pixel and his next neighbour in the 4x4 block, and the couple with the max distance is chosen as the two colours
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block_Darkest())
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
                    Organize_Colours();
                    Process_Indexes();
                    index_list.Add(index.ToArray());
                }
                break;
            case 7:  // Min/Max
                // take the colour composed of the darkest R, G and B, and the other composed of the highest R, G, and B
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block_rgb565())
                        continue;
                    diff_min_index = red_max = green_max = blue_max = 0;
                    diff_max_index = 1;
                    red_min = green_min = blue_min = 255;
                    for (byte i = 0; i < 16; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 1 || Colour_list[i][0] <= _plt0.cmpr_max)
                            continue;
                        //red = (byte)Math.Abs(rgb565[Colour_list[0][2]] - rgb565[i << 2]);
                        //green = (byte)Math.Abs(rgb565[Colour_list[0][2] + 1] - rgb565[(i << 2) + 1]);
                        //blue = (byte)Math.Abs(rgb565[Colour_list[0][2] + 2] - rgb565[(i << 2) + 2]);
                        if (rgb565[Colour_list[i][2]] > red_max)
                        {
                            red_max = rgb565[Colour_list[i][2]];  // brightest red value of Colour_RGB565
                        }
                        if (rgb565[Colour_list[i][2] + 1] > green_max)
                        {
                            green_max = rgb565[Colour_list[i][2] + 1];  // brightest green value of Colour_RGB565
                        }
                        if (rgb565[Colour_list[i][2] + 2] > blue_max)
                        {
                            blue_max = rgb565[Colour_list[i][2] + 2];  // brightest blue value of Colour_RGB565
                        }
                        if (rgb565[Colour_list[i][2]] < red_min)
                        {
                            red_min = rgb565[Colour_list[i][2]];  // Darkest red value of Colour_RGB565
                        }
                        if (rgb565[Colour_list[i][2] + 1] < green_min)
                        {
                            green_min = rgb565[Colour_list[i][2] + 1];  // Darkest green value of Colour_RGB565
                        }
                        if (rgb565[Colour_list[i][2] + 2] < blue_min)
                        {
                            blue_min = rgb565[Colour_list[i][2] + 2];  // Darkest blue value of Colour_RGB565
                        }
                    }
                    Colour_list[0][1] = (ushort)(red_min << 8 | green_min << 3 | blue_min >> 3);
                    Colour_list[1][1] = (ushort)(red_max << 8 | green_max << 3 | blue_max >> 3);
                    Organize_Colours();
                    Process_Indexes();
                    index_list.Add(index.ToArray());
                }
                break;

            case 8: // Brute Force
                // test litteraly every couple of ushort rgb565 colours. thus making 65536² combinations (equals the int max size, more than 4 billion combinations)
                // test which combination is the best one by iterating over all couples
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
                    rgb565[(z << 4) + (j << 2)] = (byte)(red & 0xf8);
                    rgb565[(z << 4) + (j << 2) + 1] = (byte)(green & 0xfc);
                    rgb565[(z << 4) + (j << 2) + 2] = (byte)(blue & 0xf8);
                    // Colour_pixel[0] = // the number of occurences, though it stays to 1 so that's not really a problem lol
                    pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                    Colour_array[1] = pixel;
                    // it's not used here
                    // Colour_array[2] = (ushort)(red + green + blue); // best way to find darkest colour :D
                    Colour_list.Add(Colour_array.ToArray());
                    // Colour_rgb565.Add(pixel);
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
                        //colour_palette.Clear();
                        // Colour_rgb565.Clear();
                        alpha_bitfield = 0;
                        continue;
                    }
                    // now let's take the colour couple with the best result inside the 4x4 block
                    diff_min = 0xffff;

                    for (int c = 0; c < 65536; c++)
                    {
                        for (int d = 0; d < 65536; d++)
                        {

                            index[0] = (byte)(c >> 8);
                            index[1] = (byte)(c);
                            index[2] = (byte)(d >> 8);
                            index[3] = (byte)(d);

                            palette_rgba[0] = (byte)(index[0] & 248);
                            palette_rgba[1] = (byte)(((index[0] & 7) << 5) + ((index[1] >> 3) & 28));
                            palette_rgba[2] = (byte)((index[1] << 3) & 248);

                            palette_rgba[4] = (byte)(index[2] & 248);
                            palette_rgba[5] = (byte)(((index[2] & 7) << 5) + ((index[3] >> 3) & 28));
                            palette_rgba[6] = (byte)((index[3] << 3) & 248);
                            if (c > d)  // colour
                            {

                                palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                                palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                                palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3));

                                palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                                palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                                palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3));
                                palette_length = 4;
                            }
                            else  // alpha
                            {

                                palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                                palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                                palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
                                palette_length = 3;
                            }
                            this.diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)
                            for (byte i = 0; i < 16; i++)
                            {
                                this.diff += Find_Nearest_Colour(i);
                            }
                            if (this.diff < diff_min)
                            {
                                diff_min = this.diff;
                                Colour_list[0][1] = (ushort)c;
                                Colour_list[1][1] = (ushort)d;
                                weemm_algorithm = false;
                            }
                        }
                    }

                }
                index[0] = (byte)(Colour_list[0][1] >> 8);
                index[1] = (byte)(Colour_list[0][1]);
                index[2] = (byte)(Colour_list[1][1] >> 8);
                index[3] = (byte)(Colour_list[1][1]);
                if (Colour_list[0][1] > Colour_list[1][1])  // colour
                {

                    palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                    palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                    palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3));

                    palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                    palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                    palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3));
                    palette_length = 4;
                }
                else  // alpha
                {

                    palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                    palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                    palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
                    palette_length = 3;
                }
                Process_Indexes();
                index_list.Add(index.ToArray());
                break;
        }
    }

    /*
     * t = (pixel_posN - pixel_pos1) / (pixel_pos2 - pixel_pos1)
       pixelN_red = (t-1)*pixel1_red + (t)*pixel2_red
       same for blue + green*/
    private ushort Find_Nearest_Colour(byte p) // position in the 4x4 block from 0 to 15
    {
        diff_min0 = 65535;
        for (byte e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
        { // calculate difference between each separate colour channel and store the sum
            diff0 = (ushort)(Math.Abs(palette_rgba[e << 2] - rgb565[(p << 2)]) + Math.Abs(palette_rgba[(e << 2) + 1] - rgb565[(p << 2) + 1]) + Math.Abs(palette_rgba[(e << 2) + 2] - rgb565[(p << 2) + 2]));
            if (diff0 < diff_min0)
                diff_min0 = diff0;
        }
        return diff_min0;
    }
    private bool Load_Block_rgb565()  // Colour list [i][0] == number of occurences   [i][1] == the colour itself   [i][2] == byte index to rgb565 array
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
        rgb565[(z << 4) + (j << 2)] = (byte)(red & 0xf8);
        rgb565[(z << 4) + (j << 2) + 1] = (byte)(green & 0xfc);
        rgb565[(z << 4) + (j << 2) + 2] = (byte)(blue & 0xf8);
        // Colour_pixel[0] = // the number of occurences, though it stays to 1 so that's not really a problem lol
        pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
        Colour_array[1] = pixel;
        // it's not used here
        Colour_array[2] = (ushort)((z << 4) + (j << 2)); // link to rgb565 array
        Colour_list.Add(Colour_array.ToArray());
        // Colour_rgb565.Add(pixel);
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
    private bool Load_Block_CIE_709()  // Colour list [i][0] == number of occurences   [i][1] == the colour itself  [i][2] == red + green + blue
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
        rgb565[(z << 4) + (j << 2)] = (byte)(red & 0xf8);
        rgb565[(z << 4) + (j << 2) + 1] = (byte)(green & 0xfc);
        rgb565[(z << 4) + (j << 2) + 2] = (byte)(blue & 0xf8);
        pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
        Colour_array[1] = pixel;
        //Colour_array[2] = (byte)(red * 0.114 + green * 0.587 + blue * 0.299);  // grey colour trimmed to 4 bit - CIE 601 produces a bad result
        Colour_array[2] = (byte)(red * 0.0721 + green * 0.7154 + blue * 0.2125);  // CIE 709 recreated the perfect re-encoding
        Colour_list.Add(Colour_array.ToArray());
        // Colour_rgb565.Add(pixel);
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
    private bool Load_Block_Darkest()  // Colour list [i][0] == number of occurences   [i][1] == the colour itself  [i][2] == red + green + blue
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
        rgb565[(z << 4) + (j << 2)] = (byte)(red & 0xf8);
        rgb565[(z << 4) + (j << 2) + 1] = (byte)(green & 0xfc);
        rgb565[(z << 4) + (j << 2) + 2] = (byte)(blue & 0xf8);
        pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
        Colour_array[1] = pixel;
        Colour_array[2] = (ushort)(red + green + blue); // best way to find darkest colour :D
        Colour_list.Add(Colour_array.ToArray());
        // Colour_rgb565.Add(pixel);
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
    private void Organize_Colours()
    {
        if (alpha_bitfield != 0 || weemm_algorithm)  // put the biggest ushort in second place
        {
            if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the second spot
            {
                index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                index[1] = (byte)(Colour_list[diff_max_index][1]);
                index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                index[3] = (byte)(Colour_list[diff_min_index][1]);
            }
            else
            {
                index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                index[1] = (byte)(Colour_list[diff_min_index][1]);
                index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                index[3] = (byte)(Colour_list[diff_max_index][1]);
            }
            palette_rgba[0] = (byte)(index[0] & 248);
            palette_rgba[1] = (byte)(((index[0] & 7) << 5) + ((index[1] >> 3) & 28));
            palette_rgba[2] = (byte)((index[1] << 3) & 248);

            palette_rgba[4] = (byte)(index[2] & 248);
            palette_rgba[5] = (byte)(((index[2] & 7) << 5) + ((index[3] >> 3) & 28));
            palette_rgba[6] = (byte)((index[3] << 3) & 248);

            palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
            palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
            palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
            palette_length = 3;

            // colour_palette.Add((ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)));  // the RGB565 third colour
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
            }
            else
            {
                index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                index[1] = (byte)(Colour_list[diff_max_index][1]);
                index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                index[3] = (byte)(Colour_list[diff_min_index][1]);
            }

            palette_rgba[0] = (byte)(index[0] & 248);
            palette_rgba[1] = (byte)(((index[0] & 7) << 5) + ((index[1] >> 3) & 28));
            palette_rgba[2] = (byte)((index[1] << 3) & 248);

            palette_rgba[4] = (byte)(index[2] & 248);
            palette_rgba[5] = (byte)(((index[2] & 7) << 5) + ((index[3] >> 3) & 28));
            palette_rgba[6] = (byte)((index[3] << 3) & 248);

            palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
            palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
            palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3));

            palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
            palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
            palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3));

            palette_length = 4;
            //pixel = (ushort)(((((palette_rgba[0] * 2 / 3) + (palette_rgba[4] / 3)) >> 3) << 11) + ((((palette_rgba[1] * 2 / 3) + (palette_rgba[5] / 3)) >> 2) << 5) + (((palette_rgba[2] * 2 / 3) + (palette_rgba[6] / 3)) >> 3));
            //colour_palette.Add(pixel);  // the RGB565 third colour
            //pixel = (ushort)(((((palette_rgba[0] / 3) + (palette_rgba[4] * 2 / 3)) >> 3) << 11) + ((((palette_rgba[1] / 3) + (palette_rgba[5] * 2 / 3)) >> 2) << 5) + (((palette_rgba[2] / 3) + (palette_rgba[6] * 2 / 3)) >> 3));
            //colour_palette.Add(pixel);  // the RGB565 fourth colour
        }
    }
    private void Process_Indexes()
    {
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
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (w << 1));
                        continue;
                    }
                    diff_min = 1024;
                    // diff_min_index = w;
                    for (byte i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]));
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = i;
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
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[4 + h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 1024;
                    // diff_min_index = w;
                    for (byte i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]));
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = i;
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
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 1024;
                    // diff_min_index = w;
                    for (byte i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]));
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = i;
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
        //colour_palette.Clear();
        // Colour_rgb565.Clear();
        alpha_bitfield = 0;
    }
}