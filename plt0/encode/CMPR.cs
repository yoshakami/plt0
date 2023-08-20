using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

/* hmm, well. let's be honest. this is the harderest encoding to write, and the most efficient one
* I'll be directly storing sub-blocks here because the rgb565 values can't be added like that

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
II II II II   II II II II  - 16 pixels


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
    ushort diff_min = 0xffff;
    ushort diff = 0;
    bool not_similar;
    bool weemm_algorithm = false;
    byte c;
    sbyte h;
    sbyte w;
    ushort z = 0;
    int j = 0;
    int x = 0;
    int y = 0;
    byte i = 0;
    ushort current_diff;
    ushort[] Colour_pixel = { 1, 0 };  // No Gradient
    ushort width = 0;
    ushort[] Colour_array = { 1, 0, 0 };  // default value of Colour_List
    ushort diff_max;
    HashSet<Tuple<byte, byte>> encounteredPairs = new HashSet<Tuple<byte, byte>>();
    public void CMPR(List<byte[]> index_list, byte[] bmp_image_ref)
    {
        bmp_image = bmp_image_ref; // this should reference bmp_image to bmp_image_ref, not copy it.
        switch (_plt0.algorithm)
        {
            default: // default: used to re-encode - Darkest/Lightest
                switch (_plt0.distance)
                {
                    default:  // Luminance
                        for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                        {
                            if (!Load_Block_CIE_709())
                                continue;
                            // now let's take the darkest and the brightest colour
                            diff_min = 0xffff;
                            diff_max = 0;
                            for (i = 0; i < 16; i++)
                            {
                                if (((alpha_bitfield >> i) & 1) == 1)
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
                            Process_Indexes_CIE_709();
                            index_list.Add(index.ToArray());
                        }
                        break;
                    case 1:  // RGB - Manhattan
                        for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                        {
                            if (!Load_Block_RGB())
                                continue;
                            // now let's take the darkest and the brightest colour
                            diff_min = 0xffff;
                            diff_max = 0;
                            for (i = 0; i < 16; i++)
                            {
                                if (((alpha_bitfield >> i) & 1) == 1)
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
                            Process_Indexes_RGB();
                            index_list.Add(index.ToArray());
                        }
                        break;
                    case 2:  // Euclidian - 2-way Quadratic
                        for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                        {
                            if (!Load_Block_Euclidian())
                                continue;
                            // now let's take the darkest and the brightest colour
                            diff_min = 0xffff;
                            diff_max = 0;
                            for (i = 0; i < 16; i++)
                            {
                                if (((alpha_bitfield >> i) & 1) == 1)
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
                            Process_Indexes_Euclidian();
                            index_list.Add(index.ToArray());
                        }
                        break;
                    case 3:  // Infinite - Tchebichev
                        for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                        {
                            if (!Load_Block_RGB())  // this block needs something on colour_list[i][2]
                                continue;
                            // now let's take the darkest and the brightest colour
                            diff_min = 0xffff;
                            diff_max = 0;
                            for (i = 0; i < 16; i++)
                            {
                                if (((alpha_bitfield >> i) & 1) == 1)
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
                            Process_Indexes_Infinite();
                            index_list.Add(index.ToArray());
                        }
                        break;
                    case 4:  // Delta E (CIEDE2000) - Lab Colour Space
                        for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                        {
                            if (!Load_Block_RGB())
                                continue;
                            // now let's take the darkest and the brightest colour
                            diff_min = 0xffff;
                            diff_max = 0;
                            for (i = 0; i < 16; i++)
                            {
                                if (((alpha_bitfield >> i) & 1) == 1)
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
                            Process_Indexes_Delta_E();
                            index_list.Add(index.ToArray());
                        }
                        break;
                }
                break;// default
            case 1: // Range Fit -   //https://stackoverflow.com/questions/24747643/3d-linear-regression
                // first, process the red_array, then do the same thing for green and blue.
                byte[] covariance = new byte[256];
                short[] red_array = new short[16];
                short sum = 0;
                short average = 0;
                bool skipped;
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block())
                        continue;
                    skipped = false;
                    diff_max = 16; // the number of colours in the array - used to remove transparent pixels from the regression calculation
                    // now let's make the "principal axis", by using 3d linear regression
                    // fill red_array values
                    for (i = 0; i < 16; i++)
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
                    for (i = 0; i < 15; i++)  // discard skipped pixels
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
                    Process_Indexes_RGB();
                    index_list.Add(index.ToArray());
                }
                break; // Range Fit
            case 2: // hidden: SuperBMD
                // SuperBMD is calculating the distance between a pixel and his next neighbour in the 4x4 block, and the couple with the max distance is chosen as the two colours
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block_RGB())
                        continue;
                    // now let's take the darkest and the brightest colour but only by going through neighbours (that's SuperBMD algorithm)
                    diff_max = 0;
                    for (i = 0; i < 15; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 0)
                        {
                            current_diff = (ushort)Math.Abs(Colour_list[i][2] - Colour_list[i + 1][2]);  // substracts RGB sums
                            if (current_diff > diff_max)
                            {
                                diff_max = current_diff;
                                diff_max_index = i;  // diff_max_index  =  second colour
                                diff_min_index = (byte)(i + 1); // diff_min_index = first colour
                            }
                        }
                    }
                    Organize_Colours();
                    Process_Indexes_RGB();
                    index_list.Add(index.ToArray());
                }
                break; // Cluster Fit
            case 3: // Wiimm
                    // he's storing every colour of the block in a byte[4] inside a sum_t structure named sum
                    // then he's making the interpolated colours for each colour of the 4x4 block and test
                    // which combination is the best one by iterating over the 16 pixels and using calc_distance
                    //Wiimm_Algorithm(_plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16, _plt0.bmp_filesize, index_list); 

                List<byte[]> index_list_2 = new List<byte[]>();
                List<byte[]> index_list_3 = new List<byte[]>();
                List<byte[]> index_list_4 = new List<byte[]>();
                List<byte[]> index_list_5 = new List<byte[]>();
                List<byte[]> index_list_6 = new List<byte[]>();
                List<byte[]> index_list_7 = new List<byte[]>();
                List<byte[]> index_list_8 = new List<byte[]>();
                int split1 = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16 + ((_plt0.canvas_height >> 6) * (_plt0.canvas_width << 5));
                int split2 = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16 + ((_plt0.canvas_height >> 5) * (_plt0.canvas_width << 5));
                int split3 = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16 + (((_plt0.canvas_height >> 5) + (_plt0.canvas_height >> 6)) * (_plt0.canvas_width << 5));
                int split4 = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16 + ((_plt0.canvas_height >> 4) * (_plt0.canvas_width << 5));
                int split5 = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16 + (((_plt0.canvas_height >> 4) + (_plt0.canvas_height >> 6)) * (_plt0.canvas_width << 5));
                int split6 = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16 + (((_plt0.canvas_height >> 4) + (_plt0.canvas_height >> 5)) * (_plt0.canvas_width << 5));
                int split7 = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16 + (((_plt0.canvas_height >> 4) + (_plt0.canvas_height >> 5) + (_plt0.canvas_height >> 6)) * (_plt0.canvas_width << 5));
                /*Wiimm_Algorithm(_plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16, (uint)split1, index_list);
                Wiimm_Algorithm(split1, (uint)split2, index_list_2);
                Wiimm_Algorithm(split2, (uint)split3, index_list_3);
                Wiimm_Algorithm(split3, _plt0.bmp_filesize, index_list_4);*/
                List<Task> tasks;
                switch (_plt0.distance)
                {
                    default:  // Luminance
                        tasks = new List<Task>
                        {
                            Task.Run(() => Wiimm_Algorithm_CIE_709(_plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16, (uint)split1, index_list)),
                            Task.Run(() => Wiimm_Algorithm_CIE_709(split1, (uint)split2, index_list_2)),
                            Task.Run(() => Wiimm_Algorithm_CIE_709(split2, (uint)split3, index_list_3)),
                            Task.Run(() => Wiimm_Algorithm_CIE_709(split3, (uint)split4, index_list_4)),
                            Task.Run(() => Wiimm_Algorithm_CIE_709(split4, (uint)split5, index_list_5)),
                            Task.Run(() => Wiimm_Algorithm_CIE_709(split5, (uint)split6, index_list_6)),
                            Task.Run(() => Wiimm_Algorithm_CIE_709(split6, (uint)split7, index_list_7)),
                            Task.Run(() => Wiimm_Algorithm_CIE_709(split7, _plt0.bmp_filesize, index_list_8))
                        };
                        break;
                    case 1:  // RGB - Manhattan
                        tasks = new List<Task>
                        {
                            Task.Run(() => Wiimm_Algorithm_RGB(_plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16, (uint)split1, index_list)),
                            Task.Run(() => Wiimm_Algorithm_RGB(split1, (uint)split2, index_list_2)),
                            Task.Run(() => Wiimm_Algorithm_RGB(split2, (uint)split3, index_list_3)),
                            Task.Run(() => Wiimm_Algorithm_RGB(split3, (uint)split4, index_list_4)),
                            Task.Run(() => Wiimm_Algorithm_RGB(split4, (uint)split5, index_list_5)),
                            Task.Run(() => Wiimm_Algorithm_RGB(split5, (uint)split6, index_list_6)),
                            Task.Run(() => Wiimm_Algorithm_RGB(split6, (uint)split7, index_list_7)),
                            Task.Run(() => Wiimm_Algorithm_RGB(split7, _plt0.bmp_filesize, index_list_8))
                        };
                        break;
                    case 2:  // Euclidian - 2-way Quadratic
                        tasks = new List<Task>
                        {
                            Task.Run(() => Wiimm_Algorithm_Euclidian(_plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16, (uint)split1, index_list)),
                            Task.Run(() => Wiimm_Algorithm_Euclidian(split1, (uint)split2, index_list_2)),
                            Task.Run(() => Wiimm_Algorithm_Euclidian(split2, (uint)split3, index_list_3)),
                            Task.Run(() => Wiimm_Algorithm_Euclidian(split3, (uint)split4, index_list_4)),
                            Task.Run(() => Wiimm_Algorithm_Euclidian(split4, (uint)split5, index_list_5)),
                            Task.Run(() => Wiimm_Algorithm_Euclidian(split5, (uint)split6, index_list_6)),
                            Task.Run(() => Wiimm_Algorithm_Euclidian(split6, (uint)split7, index_list_7)),
                            Task.Run(() => Wiimm_Algorithm_Euclidian(split7, _plt0.bmp_filesize, index_list_8))
                        };
                        break;
                    case 3:  // Infinite - Tchebichev
                        tasks = new List<Task>
                        {
                            Task.Run(() => Wiimm_Algorithm_Infinite(_plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16, (uint)split1, index_list)),
                            Task.Run(() => Wiimm_Algorithm_Infinite(split1, (uint)split2, index_list_2)),
                            Task.Run(() => Wiimm_Algorithm_Infinite(split2, (uint)split3, index_list_3)),
                            Task.Run(() => Wiimm_Algorithm_Infinite(split3, (uint)split4, index_list_4)),
                            Task.Run(() => Wiimm_Algorithm_Infinite(split4, (uint)split5, index_list_5)),
                            Task.Run(() => Wiimm_Algorithm_Infinite(split5, (uint)split6, index_list_6)),
                            Task.Run(() => Wiimm_Algorithm_Infinite(split6, (uint)split7, index_list_7)),
                            Task.Run(() => Wiimm_Algorithm_Infinite(split7, _plt0.bmp_filesize, index_list_8))
                        };
                        break;
                    case 4:  // Delta E (CIEDE2000) - Lab Colour Space
                        tasks = new List<Task>
                        {
                            Task.Run(() => Wiimm_Algorithm_Delta_E(_plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16, (uint)split1, index_list)),
                            Task.Run(() => Wiimm_Algorithm_Delta_E(split1, (uint)split2, index_list_2)),
                            Task.Run(() => Wiimm_Algorithm_Delta_E(split2, (uint)split3, index_list_3)),
                            Task.Run(() => Wiimm_Algorithm_Delta_E(split3, (uint)split4, index_list_4)),
                            Task.Run(() => Wiimm_Algorithm_Delta_E(split4, (uint)split5, index_list_5)),
                            Task.Run(() => Wiimm_Algorithm_Delta_E(split5, (uint)split6, index_list_6)),
                            Task.Run(() => Wiimm_Algorithm_Delta_E(split6, (uint)split7, index_list_7)),
                            Task.Run(() => Wiimm_Algorithm_Delta_E(split7, _plt0.bmp_filesize, index_list_8))
                        };
                        break;
                }
                Task.WhenAll(tasks).Wait(); // Wait for tasks to complete;

                index_list.AddRange(index_list_2);
                index_list.AddRange(index_list_3);
                index_list.AddRange(index_list_4);
                index_list.AddRange(index_list_5);
                index_list.AddRange(index_list_6);
                index_list.AddRange(index_list_7);
                index_list.AddRange(index_list_8);
                break; // Wiimm V2
            case 4: // hidden: SuperBMD
                    // SuperBMD is calculating the distance between a pixel and his next neighbour in the 4x4 block, and the couple with the max distance is chosen as the two colours
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block_RGB())
                        continue;
                    // now let's take the darkest and the brightest colour but only by going through neighbours (that's SuperBMD algorithm)
                    diff_max = 0;
                    for (i = 0; i < 15; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 0)
                        {
                            current_diff = (ushort)Math.Abs(Colour_list[i][2] - Colour_list[i + 1][2]);  // substracts RGB sums
                            if (current_diff > diff_max)
                            {
                                diff_max = current_diff;
                                diff_max_index = i;  // diff_max_index  =  second colour
                                diff_min_index = (byte)(i + 1); // diff_min_index = first colour
                            }
                        }
                    }
                    Organize_Colours();
                    Process_Indexes_RGB();
                    index_list.Add(index.ToArray());
                }
                break; // Average
            case 5:  // CI2 - most used colours with _plt0.diversity - no gradient - similar - looks pixelated
                {
                    for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                    {
                        if (!Load_Block_RGB())
                            continue;
                        // now let's just try to take the most two used colours and use _plt0.diversity I guess
                        // implementing my own way to find most used colours
                        // let's count the number of exact same colours in Colour_list
                        for (i = 0; i < 16; i++)  // useless to set it to 16 because of the condition k > i.
                        {
                            for (c = (byte)(i + 1); c < 16; c++)
                            {
                                if (Colour_list[c][1] == Colour_list[i][1] && ((alpha_bitfield >> c) & 1) == 0 && ((alpha_bitfield >> i) & 1) == 0)  // k > i prevents colours occurences from being added twice.
                                {
                                    Colour_list[c][2]++;
                                    Colour_list[i][2] = 0; // should set it to zero.
                                }
                            }
                        }
                        Colour_list.Sort(new UshortArrayComparer());  // sorts the table by the most used colour first
                        c = 0;
                        for (i = 0; i < 16 && c < 2; i++)  // build the colour table with the two most used colours and _plt0.diversity
                        {
                            not_similar = true;
                            if (Colour_list[i][2] / 16 < _plt0.percentage / 100)
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

                            for (i = 0; i < 16 && c < 2; i++)
                            {
                                not_similar = true;
                                if (Colour_list[i][2] / 16 < _plt0.percentage2 / 100)
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
                                for (i = 0; i < 16 && c < 2; i++)
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
                        Process_Indexes_RGB();
                        index_list.Add(index.ToArray());
                    }
                }
                break; // CI2
            case 6: // hidden: SuperBMD
                    // SuperBMD is calculating the distance between a pixel and his next neighbour in the 4x4 block, and the couple with the max distance is chosen as the two colours
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block_RGB())
                        continue;
                    // now let's take the darkest and the brightest colour but only by going through neighbours (that's SuperBMD algorithm)
                    diff_max = 0;
                    for (i = 0; i < 15; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 0)
                        {
                            current_diff = (ushort)Math.Abs(Colour_list[i][2] - Colour_list[i + 1][2]);  // substracts RGB sums
                            if (current_diff > diff_max)
                            {
                                diff_max = current_diff;
                                diff_max_index = i;  // diff_max_index  =  second colour
                                diff_min_index = (byte)(i + 1); // diff_min_index = first colour
                            }
                        }
                    }
                    Organize_Colours();
                    Process_Indexes_RGB();
                    index_list.Add(index.ToArray());
                }
                break; // SuperBMD
            case 7:  // hidden: Min/Max
                     // take the colour composed of the darkest R, G and B, and the other composed of the highest R, G, and B
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block())
                        continue;
                    diff_min_index = red_max = green_max = blue_max = 0;
                    diff_max_index = 1;
                    red_min = green_min = blue_min = 255;
                    for (i = 0; i < 16; i++)
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
                    Process_Indexes_RGB();
                    index_list.Add(index.ToArray());
                }
                break; // Min/Max
            case 8: // hidden: Most Used/Furthest
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block())
                        continue;
                    Colour_rgb565 = Colour_list.ToList();
                    // implementing my own way to find most used colours
                    // let's count the number of exact same colours in Colour_list
                    for (i = 0; i < 16; i++)  // useless to set it to 16 because of the condition k > i.
                    {
                        for (c = (byte)(i + 1); c < 16; c++)
                        {
                            if (Colour_list[c][1] == Colour_list[i][1] && ((alpha_bitfield >> c) & 1) == 0 && ((alpha_bitfield >> i) & 1) == 0)  // k > i prevents colours occurences from being added twice.
                            {
                                Colour_list[c][2]++;
                                Colour_list[i][2] = 0; // should set it to zero.
                            }
                        }
                    }
                    Colour_list.Sort(new UshortArrayComparer());  // sorts the table by the most used colour first
                    diff_min_index = 0;
                    diff_min = 0xffff;
                    for (i = 0; i < 15; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 1)
                            continue;
                        this.diff = (ushort)(rgb565[Colour_list[diff_min_index][2]] + rgb565[Colour_list[diff_min_index][2] + 1] + rgb565[Colour_list[diff_min_index][2] + 2]);
                        if (this.diff < diff_min)
                        {
                            diff_min = this.diff;
                            diff_min_index = i;
                        }
                        if (Colour_list[i][2] != Colour_list[i + 1][2])
                            break;
                    }
                    // now let's take the colour the furthest away from it
                    diff_max = 0;
                    for (i = 0; i < 16; i++)
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
                    Process_Indexes_RGB();
                    index_list.Add(index.ToArray());
                }
                break; // Most Used/Furthest
            case 9: // hidden: darkest/lightest
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (!Load_Block_RGB())
                        continue;
                    // now let's take the darkest and the brightest colour
                    diff_min = 0xffff;
                    diff_max = 0;
                    for (i = 0; i < 16; i++)
                    {
                        if (((alpha_bitfield >> i) & 1) == 1)
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
                    Process_Indexes_RGB();
                    index_list.Add(index.ToArray());
                }
                break; // Darkest/Lightest
            case 10: // hidden: Brute Force
                     // test litteraly every couple of ushort rgb565 colours. thus making 65536² combinations (equals the int max size, more than 4 billion combinations)
                     // test which combination is the best one by iterating over all couples
                     // there is no way to use it unless you modify this project and optimize this algorithm
                     // it rather is a code easter egg left for comparison purpose
                for (y = _plt0.pixel_data_start_offset + (_plt0.canvas_width << 2) - 16; y < _plt0.bmp_filesize; y += 4)
                {
                    if (_plt0.alpha > 0 && bmp_image[y + _plt0.rgba_channel[3]] < _plt0.cmpr_alpha_threshold)  // no colour
                    {
                        alpha_bitfield += (ushort)(1 << (j + (z * 4)));
                    }
                    else  // opaque / no alpha / colours
                    {
                        red = bmp_image[y + _plt0.rgba_channel[0]];
                        green = bmp_image[y + _plt0.rgba_channel[1]];
                        blue = bmp_image[y + _plt0.rgba_channel[2]];
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
                        Colour_array[0] = (ushort)((z << 4) + (j << 2)); // link to rgb565 array
                        rgb565[Colour_array[0]] = (byte)(red & 0xf8);
                        rgb565[Colour_array[0] + 1] = (byte)(green & 0xfc);
                        rgb565[Colour_array[0] + 2] = (byte)(blue & 0xf8);
                        pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                        Colour_array[1] = pixel;
                    }
                    Colour_list.Add(Colour_array.ToArray());
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
                        y += (_plt0.canvas_width << 2) - 16;
                        x = 0;
                    }
                    else if (x == 2)
                    {
                        // the cursor warped horizontally to the first block in width 4 lines above
                        // IT WARPS NORMALLY LIKE IT4S THE PIXEL AFTER
                        y += 16;  // this is right in the mirror and mirrorred mode
                    }
                    else if (x == 4)
                    {
                        // minus 8 lines + point to next block
                        y -= (_plt0.canvas_width << 5) + 16;
                        x = 0;
                    }
                    else
                    {
                        y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
                    }
                    if (alpha_bitfield == 0xffff)  // save computation time I guess
                    {
                        index_list.Add(full_alpha_index); // this byte won't be changed so no need to copy it
                        Colour_list.Clear();
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
                            for (i = 0; i < 16; i++)
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
                Process_Indexes_RGB();
                index_list.Add(index.ToArray());
                break;// Brute Force
        }
    }

    private ushort Find_Nearest_Colour(byte p) // position in the 4x4 block from 0 to 15
    {
        ushort diff_min0 = 65535;
        ushort diff0;
        for (byte e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
        { // calculate difference between each separate colour channel and store the sum
            diff0 = (ushort)(Math.Abs(palette_rgba[e << 2] - rgb565[(p << 2)]) + Math.Abs(palette_rgba[(e << 2) + 1] - rgb565[(p << 2) + 1]) + Math.Abs(palette_rgba[(e << 2) + 2] - rgb565[(p << 2) + 2]));
            if (diff0 < diff_min0)
                diff_min0 = diff0;
        }
        return diff_min0;
    }
    private bool Load_Block()  // Colour list [i][0] == index to rgb565 array   [i][1] == the colour itself
    {
        if (_plt0.alpha > 0 && bmp_image[y + _plt0.rgba_channel[3]] < _plt0.cmpr_alpha_threshold)  // no colour
        {
            alpha_bitfield += (ushort)(1 << (j + (z * 4)));
        }
        else  // opaque / no alpha / colours
        {
            red = bmp_image[y + _plt0.rgba_channel[0]];
            green = bmp_image[y + _plt0.rgba_channel[1]];
            blue = bmp_image[y + _plt0.rgba_channel[2]];
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
            Colour_array[0] = (ushort)((z << 4) + (j << 2)); // link to rgb565 array
            rgb565[Colour_array[0]] = (byte)(red & 0xf8);
            rgb565[Colour_array[0] + 1] = (byte)(green & 0xfc);
            rgb565[Colour_array[0] + 2] = (byte)(blue & 0xf8);
            pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
            Colour_array[1] = pixel;
        }
        Colour_list.Add(Colour_array.ToArray());
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
            y += (_plt0.canvas_width << 2) - 16;
            x = 0;
        }
        else if (x == 2)
        {
            y += 16;  // the cursor warped horizontally to the first block in width 4 lines above
        }
        else if (x == 4)
        {
            y -= (_plt0.canvas_width << 5) + 16;  // minus 8 lines + point to next block
            x = 0;
        }
        else
        {
            y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
        }
        return true;
    }
    private bool Load_Block_CIE_709()  // Colour list [i][0] == index to rgb565 array   [i][1] == the colour itself  [i][2] == red + green + blue in black and white
    {
        if (_plt0.alpha > 0 && bmp_image[y + _plt0.rgba_channel[3]] < _plt0.cmpr_alpha_threshold)  // no colour
        {
            alpha_bitfield += (ushort)(1 << (j + (z * 4)));
        }
        else  // opaque / no alpha / colours
        {
            red = bmp_image[y + _plt0.rgba_channel[0]];
            green = bmp_image[y + _plt0.rgba_channel[1]];
            blue = bmp_image[y + _plt0.rgba_channel[2]];
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
            Colour_array[0] = (ushort)((z << 4) + (j << 2)); // link to rgb565 array
            rgb565[Colour_array[0]] = (byte)(red & 0xf8);
            rgb565[Colour_array[0] + 1] = (byte)(green & 0xfc);
            rgb565[Colour_array[0] + 2] = (byte)(blue & 0xf8);
            pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
            Colour_array[1] = pixel;
            Colour_array[2] = (byte)(red * 0.0721 + green * 0.7154 + blue * 0.2125);  // CIE 709 recreated the perfect re-encoding
        }
        Colour_list.Add(Colour_array.ToArray());
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
            y += 16;  // the cursor warped horizontally to the first block in width 4 lines above
                      // this is right in the mirror and mirrorred mode
        }
        else if (x == 4)
        {
            y -= (_plt0.canvas_width << 5) + 16;  // minus 8 lines + point to next block
            x = 0;
        }
        else
        {
            y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
        }
        return true;
    }
    private bool Load_Block_RGB()  // Colour list [i][0] == index to rgb565 array   [i][1] == the colour itself  [i][2] == red + green + blue
    {
        if (_plt0.alpha > 0 && bmp_image[y + _plt0.rgba_channel[3]] < _plt0.cmpr_alpha_threshold)  // no colour
        {
            alpha_bitfield += (ushort)(1 << (j + (z * 4)));
        }
        else  // opaque / no alpha / colours
        {
            red = bmp_image[y + _plt0.rgba_channel[0]];
            green = bmp_image[y + _plt0.rgba_channel[1]];
            blue = bmp_image[y + _plt0.rgba_channel[2]];
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
            Colour_array[0] = (ushort)((z << 4) + (j << 2)); // link to rgb565 array
            rgb565[Colour_array[0]] = (byte)(red & 0xf8);
            rgb565[Colour_array[0] + 1] = (byte)(green & 0xfc);
            rgb565[Colour_array[0] + 2] = (byte)(blue & 0xf8);
            pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
            Colour_array[1] = pixel;
            Colour_array[2] = (ushort)(red + green + blue); // best way to find darkest colour :D
        }
        Colour_list.Add(Colour_array.ToArray());
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
            y += (_plt0.canvas_width << 2) - 16;
            x = 0;
        }
        else if (x == 2)
        {
            y += 16;  // the cursor warped horizontally to the first block in width 4 lines above
                      // this is right in the mirror and mirrorred mode
        }
        else if (x == 4)
        {
            y -= (_plt0.canvas_width << 5) + 16;  // minus 8 lines + point to next block
            x = 0;
        }
        else
        {
            y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
        }
        return true;
    }
    private bool Load_Block_Euclidian()  // Colour list [i][0] == index to rgb565 array   [i][1] == the colour itself  [i][2] == Math.Sqrt(red² + green² + blue²)
    {
        if (_plt0.alpha > 0 && bmp_image[y + _plt0.rgba_channel[3]] < _plt0.cmpr_alpha_threshold)  // no colour
        {
            alpha_bitfield += (ushort)(1 << (j + (z * 4)));
        }
        else  // opaque / no alpha / colours
        {
            red = bmp_image[y + _plt0.rgba_channel[0]];
            green = bmp_image[y + _plt0.rgba_channel[1]];
            blue = bmp_image[y + _plt0.rgba_channel[2]];
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
            Colour_array[0] = (ushort)((z << 4) + (j << 2)); // link to rgb565 array
            rgb565[Colour_array[0]] = (byte)(red & 0xf8);
            rgb565[Colour_array[0] + 1] = (byte)(green & 0xfc);
            rgb565[Colour_array[0] + 2] = (byte)(blue & 0xf8);
            pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
            Colour_array[1] = pixel;
            Colour_array[2] = (ushort)((red * red + green * green + blue * blue) >> 2); // best way to find darkest colour :D
        }
        Colour_list.Add(Colour_array.ToArray());
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
            y += (_plt0.canvas_width << 2) - 16;
            x = 0;
        }
        else if (x == 2)
        {
            // the cursor warped horizontally to the first block in width 4 lines above
            y += 16;  // this is right in the mirror and mirrorred mode
        }
        else if (x == 4)
        {
            y -= (_plt0.canvas_width << 5) + 16; // minus 8 lines + point to next block
            x = 0;
        }
        else
        {
            y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
        }
        return true;
    }

    // There's no Load_Block_Infinite nor Delta_E since they're used for differences between a pair of two colours, no more.

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
                palette_rgba[0] = rgb565[Colour_list[diff_max_index][0]]; // red
                palette_rgba[1] = rgb565[Colour_list[diff_max_index][0] + 1]; // green
                palette_rgba[2] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue
                palette_rgba[4] = rgb565[Colour_list[diff_min_index][0]]; // red2
                palette_rgba[5] = rgb565[Colour_list[diff_min_index][0] + 1]; // green2
                palette_rgba[6] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue2
            }
            else
            {
                index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                index[1] = (byte)(Colour_list[diff_min_index][1]);
                index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                index[3] = (byte)(Colour_list[diff_max_index][1]);
                palette_rgba[0] = rgb565[Colour_list[diff_min_index][0]]; // red
                palette_rgba[1] = rgb565[Colour_list[diff_min_index][0] + 1]; // green
                palette_rgba[2] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue
                palette_rgba[4] = rgb565[Colour_list[diff_max_index][0]]; // red2
                palette_rgba[5] = rgb565[Colour_list[diff_max_index][0] + 1]; // green2
                palette_rgba[6] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue2
            }
            palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
            palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
            palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1); // the RGB565 third colour
            palette_length = 3;

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
                palette_rgba[0] = rgb565[Colour_list[diff_min_index][0]]; // red
                palette_rgba[1] = rgb565[Colour_list[diff_min_index][0] + 1]; // green
                palette_rgba[2] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue
                palette_rgba[4] = rgb565[Colour_list[diff_max_index][0]]; // red2
                palette_rgba[5] = rgb565[Colour_list[diff_max_index][0] + 1]; // green2
                palette_rgba[6] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue2
            }
            else
            {
                index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                index[1] = (byte)(Colour_list[diff_max_index][1]);
                index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                index[3] = (byte)(Colour_list[diff_min_index][1]);
                palette_rgba[0] = rgb565[Colour_list[diff_max_index][0]]; // red
                palette_rgba[1] = rgb565[Colour_list[diff_max_index][0] + 1]; // green
                palette_rgba[2] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue
                palette_rgba[4] = rgb565[Colour_list[diff_min_index][0]]; // red2
                palette_rgba[5] = rgb565[Colour_list[diff_min_index][0] + 1]; // green2
                palette_rgba[6] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue2
            }

            palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
            palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
            palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3)); // the RGB565 third colour

            palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
            palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
            palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3)); // the RGB565 fourth colour

            palette_length = 4;
        }
    }
    private void Process_Indexes_CIE_709()
    {
        // time to get the "linear interpolation to add third and fourth colour
        // CI2 if that's a name
        for (i = 4; i < 8; i++)
        {
            index[i] = 0;
        }
        if (_plt0.reverse_x)
        {
            for (h = 3; h >= 0; h--)
            {
                for (w = 3; w >= 0; w--)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (w << 1));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) * 0.0721 + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) * 0.7154 + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]) * 0.2125);
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
            for (h = 0; h < 4; h++)
            {
                for (w = 0; w < 4; w++)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[4 + h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) * 0.0721 + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) * 0.7154 + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]) * 0.2125);
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = i;
                        }
                    }
                    index[4 + h] += (byte)(diff_min_index << (6 - (w << 1)));
                }
            }
        }
        else
        {
            for (h = 3; h >= 0; h--)
            {
                for (w = 0; w < 4; w++)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) * 0.0721 + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) * 0.7154 + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]) * 0.2125);
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = i;
                        }
                    }
                    index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                }
            }
        }
        // index is overwritten each time
        // the lists need to be cleaned
        Colour_list.Clear();
        alpha_bitfield = 0;
    }
    private void Process_Indexes_RGB()
    {
        // time to get the "linear interpolation to add third and fourth colour
        // CI2 if that's a name
        for (i = 4; i < 8; i++)
        {
            index[i] = 0;
        }
        if (_plt0.reverse_x)
        {
            for (h = 3; h >= 0; h--)
            {
                for (w = 3; w >= 0; w--)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (w << 1));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
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
            for (h = 0; h < 4; h++)
            {
                for (w = 0; w < 4; w++)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[4 + h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]));
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = i;
                        }
                    }
                    index[4 + h] += (byte)(diff_min_index << (6 - (w << 1)));
                }
            }
        }
        else
        {
            for (h = 3; h >= 0; h--)
            {
                for (w = 0; w < 4; w++)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]));
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = i;
                        }
                    }
                    index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                }
            }
        }
        // index is overwritten each time
        // the lists need to be cleaned
        Colour_list.Clear();
        alpha_bitfield = 0;
    }
    private void Process_Indexes_Euclidian()
    {
        // time to get the "linear interpolation to add third and fourth colour
        // CI2 if that's a name
        for (i = 4; i < 8; i++)
        {
            index[i] = 0;
        }
        if (_plt0.reverse_x)
        {
            for (h = 3; h >= 0; h--)
            {
                for (w = 3; w >= 0; w--)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (w << 1));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff = (ushort)((int)(Math.Pow(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)], 2) + Math.Pow(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1], 2) + Math.Pow(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2], 2)) >> 2);
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
            for (h = 0; h < 4; h++)
            {
                for (w = 0; w < 4; w++)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[4 + h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff = (ushort)((int)(Math.Pow(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)], 2) + Math.Pow(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1], 2) + Math.Pow(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2], 2)) >> 2);
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = i;
                        }
                    }
                    index[4 + h] += (byte)(diff_min_index << (6 - (w << 1)));
                }
            }
        }
        else
        {
            for (h = 3; h >= 0; h--)
            {
                for (w = 0; w < 4; w++)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff = (ushort)((int)(Math.Pow(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)], 2) + Math.Pow(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1], 2) + Math.Pow(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2], 2)) >> 2);
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = i;
                        }
                    }
                    index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                }
            }
        }
        // index is overwritten each time
        // the lists need to be cleaned
        Colour_list.Clear();
        alpha_bitfield = 0;
    }
    private void Process_Indexes_Infinite()
    {
        ushort diff_red;
        ushort diff_green;
        ushort diff_blue;
        // time to get the "linear interpolation to add third and fourth colour
        // CI2 if that's a name
        for (i = 4; i < 8; i++)
        {
            index[i] = 0;
        }
        if (_plt0.reverse_x)
        {
            for (h = 3; h >= 0; h--)
            {
                for (w = 3; w >= 0; w--)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (w << 1));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff_red = (ushort)Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]);
                        diff_green = (ushort)Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]);
                        diff_blue = (ushort)Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]);
                        if (diff_red > diff_green)  // takes the biggest value between red, green, and blue
                        {
                            if (diff_red > diff_blue)
                            {
                                diff = diff_red;
                            }
                            else
                            {
                                diff = diff_blue;
                            }
                        }
                        else
                        {
                            if (diff_green > diff_blue)
                            {
                                diff = diff_green;
                            }
                            else
                            {
                                diff = diff_blue;
                            }
                        }
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
            for (h = 0; h < 4; h++)
            {
                for (w = 0; w < 4; w++)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[4 + h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff_red = (ushort)Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]);
                        diff_green = (ushort)Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]);
                        diff_blue = (ushort)Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]);
                        if (diff_red > diff_green)  // takes the biggest value between red, green, and blue
                        {
                            if (diff_red > diff_blue)
                            {
                                diff = diff_red;
                            }
                            else
                            {
                                diff = diff_blue;
                            }
                        }
                        else
                        {
                            if (diff_green > diff_blue)
                            {
                                diff = diff_green;
                            }
                            else
                            {
                                diff = diff_blue;
                            }
                        }
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
            for (h = 3; h >= 0; h--)
            {
                for (w = 0; w < 4; w++)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 0xffff;
                    // diff_min_index = w;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        diff_red = (ushort)Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]);
                        diff_green = (ushort)Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]);
                        diff_blue = (ushort)Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]);
                        if (diff_red > diff_green)  // takes the biggest value between red, green, and blue
                        {
                            if (diff_red > diff_blue)
                            {
                                diff = diff_red;
                            }
                            else
                            {
                                diff = diff_blue;
                            }
                        }
                        else
                        {
                            if (diff_green > diff_blue)
                            {
                                diff = diff_green;
                            }
                            else
                            {
                                diff = diff_blue;
                            }
                        }
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
    private void Process_Indexes_Delta_E()
    {
        double diff;
        double diff_min;
        byte[] rgb1 = { 0, 0, 0 };
        byte[] rgb2 = { 0, 0, 0 };
        // time to get the "linear interpolation to add third and fourth colour
        // CI2 if that's a name
        for (i = 4; i < 8; i++)
        {
            index[i] = 0;
        }
        if (_plt0.reverse_x)
        {
            for (h = 3; h >= 0; h--)
            {
                for (w = 3; w >= 0; w--)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (w << 1));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        rgb1[0] = palette_rgba[i << 2];
                        rgb2[0] = rgb565[(h << 4) + (w << 2)];
                        rgb1[1] = palette_rgba[(i << 2) + 1];
                        rgb2[1] = rgb565[(h << 4) + (w << 2) + 1];
                        rgb1[2] = palette_rgba[(i << 2) + 2];
                        rgb2[2] = rgb565[(h << 4) + (w << 2) + 2];
                        diff = CalculateCIEDE2000(rgb1, rgb2);
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
            for (h = 0; h < 4; h++)
            {
                for (w = 0; w < 4; w++)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[4 + h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        rgb1[0] = palette_rgba[i << 2];
                        rgb2[0] = rgb565[(h << 4) + (w << 2)];
                        rgb1[1] = palette_rgba[(i << 2) + 1];
                        rgb2[1] = rgb565[(h << 4) + (w << 2) + 1];
                        rgb1[2] = palette_rgba[(i << 2) + 2];
                        rgb2[2] = rgb565[(h << 4) + (w << 2) + 2];
                        diff = CalculateCIEDE2000(rgb1, rgb2);
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = i;
                        }
                    }
                    index[4 + h] += (byte)(diff_min_index << (6 - (w << 1)));
                }
            }
        }
        else
        {
            for (h = 3; h >= 0; h--)
            {
                for (w = 0; w < 4; w++)  // index_size = number of pixels
                {
                    if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                    {
                        index[7 - h] += (byte)(3 << (6 - (w << 1)));
                        continue;
                    }
                    diff_min = 0xffff;
                    for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                    { // calculate difference between each separate colour channel and store the sum
                        rgb1[0] = palette_rgba[i << 2];
                        rgb2[0] = rgb565[(h << 4) + (w << 2)];
                        rgb1[1] = palette_rgba[(i << 2) + 1];
                        rgb2[1] = rgb565[(h << 4) + (w << 2) + 1];
                        rgb1[2] = palette_rgba[(i << 2) + 2];
                        rgb2[2] = rgb565[(h << 4) + (w << 2) + 2];
                        diff = CalculateCIEDE2000(rgb1, rgb2);
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = i;
                        }
                    }
                    index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                }
            }
        }
        // index is overwritten each time
        // the lists need to be cleaned
        Colour_list.Clear();
        alpha_bitfield = 0;
    }



    private void Wiimm_Algorithm_CIE_709(int startpoint, uint endpoint, List<byte[]> index_list)
    {
        bool block_has_alpha = false;
        byte palette_length;
        int j = 0;
        int x = 0;
        int y = 0;
        byte i;
        byte e;
        byte c;
        byte d;
        sbyte w;
        sbyte h;
        ushort z = 0;
        ushort width = 0;
        ushort diff_min0 = 65535;
        ushort diff0;
        ushort diff_min;
        ushort diff;
        byte diff_min_index = 0;
        byte diff_max_index = 0;
        byte red;
        byte green;
        byte blue;
        ushort alpha_bitfield = 0;
        ushort pixel;
        byte[] index = new byte[8];  // sub block length
        ushort[] Colour_array = { 1, 0, 0 };  // default value of Colour_List
        List<ushort[]> Colour_list = new List<ushort[]>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
        HashSet<Tuple<ushort, ushort>> encounteredPairs = new HashSet<Tuple<ushort, ushort>>();
        byte[] palette_rgba = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        byte[] rgb565 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  // 64 rgbx bytes, x is unused
        for (y = startpoint; y < endpoint; y += 4)
        {
            if (_plt0.alpha > 0 && bmp_image[y + _plt0.rgba_channel[3]] < _plt0.cmpr_alpha_threshold)  // no colour
            {
                alpha_bitfield += (ushort)(1 << (j + (z * 4)));
            }
            else  // opaque / no alpha / colours
            {
                red = bmp_image[y + _plt0.rgba_channel[0]];
                green = bmp_image[y + _plt0.rgba_channel[1]];
                blue = bmp_image[y + _plt0.rgba_channel[2]];
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
                Colour_array[0] = (ushort)((z << 4) + (j << 2)); // link to rgb565 array
                rgb565[Colour_array[0]] = (byte)(red & 0xf8);
                rgb565[Colour_array[0] + 1] = (byte)(green & 0xfc);
                rgb565[Colour_array[0] + 2] = (byte)(blue & 0xf8);
                pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                Colour_array[1] = pixel;
            }
            Colour_list.Add(Colour_array.ToArray());
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
                y += (_plt0.canvas_width << 2) - 16;
                x = 0;
            }
            else if (x == 2)
            {
                y += 16; // the cursor warped horizontally to the first block in width 4 lines above
            }
            else if (x == 4)
            {
                y -= (_plt0.canvas_width << 5) + 16; // minus 8 lines + point to next block
                x = 0;
            }
            else
            {
                y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
            }
            if (alpha_bitfield == 0xffff)  // save computation time I guess
            {
                index_list.Add(full_alpha_index); // this byte won't be changed so no need to copy it
                Colour_list.Clear();
                alpha_bitfield = 0;
                continue;
            }
            // now let's take the colour couple with the best result inside the 4x4 block
            diff_min = 0xffff;
            if (alpha_bitfield != 0)  // put the biggest ushort in second place - there are transparent pixels in this block
            {
                block_has_alpha = true;
                palette_length = 3;
                for (c = 0; c < 16; c++)  // useless to set it to 16 because of the condition c > i.
                {
                    for (d = (byte)(c + 1); d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2
                                                                // alpha padding
                        palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                        palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                        palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
                        // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 3 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                diff0 = (ushort)(Math.Abs(palette_rgba[e << 2] - rgb565[(i << 2)]) * 0.0721 + Math.Abs(palette_rgba[(e << 2) + 1] - rgb565[(i << 2) + 1]) * 0.7154 + Math.Abs(palette_rgba[(e << 2) + 2] - rgb565[(i << 2) + 2]) * 0.2125);
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                        }
                    }
                }
            }
            else
            {
                palette_length = 4;
                for (c = 0; c < 16; c++)  // useless to set it to 16 because of the condition c > i.
                {
                    for (d = (byte)(c + 1); d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2

                        palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                        palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                        palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3));

                        palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                        palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                        palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3));
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 4 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                diff0 = (ushort)(Math.Abs(palette_rgba[e << 2] - rgb565[(i << 2)]) * 0.0721 + Math.Abs(palette_rgba[(e << 2) + 1] - rgb565[(i << 2) + 1]) * 0.7154 + Math.Abs(palette_rgba[(e << 2) + 2] - rgb565[(i << 2) + 2]) * 0.2125);
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                            block_has_alpha = false;
                        }
                    }
                }
                palette_length = 3;
                for (c = 0; c < 16; c++)
                {
                    for (d = (byte)(c + 1); d < 16; d++)
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2

                        palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                        palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                        palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
                        // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 3 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                diff0 = (ushort)(Math.Abs(palette_rgba[e << 2] - rgb565[(i << 2)]) * 0.0721 + Math.Abs(palette_rgba[(e << 2) + 1] - rgb565[(i << 2) + 1]) * 0.7154 + Math.Abs(palette_rgba[(e << 2) + 2] - rgb565[(i << 2) + 2]) * 0.2125);
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                            block_has_alpha = true;
                        }
                    }
                }
            }
            if (block_has_alpha)  // put the biggest ushort in second place
            {
                palette_length = 3;
                if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the second spot
                {
                    index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_max_index][1]);
                    index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_min_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_max_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_max_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_min_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_min_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue2
                }
                else
                {
                    index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_min_index][1]);
                    index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_max_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_min_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_min_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_max_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_max_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue2
                }
                palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);

                // last colour isn't in the palette, it's in _plt0.alpha_bitfield


            }
            else  // put biggest ushort in first place
            {
                palette_length = 4;
                // of course, that's the exact opposite!
                if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the first spot
                {
                    index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_min_index][1]);
                    index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_max_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_min_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_min_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_max_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_max_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue2
                }
                else
                {
                    index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_max_index][1]);
                    index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_min_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_max_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_max_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_min_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_min_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue2
                }

                palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3)); // the RGB565 third colour

                palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3)); // the RGB565 fourth colour
            }
            // time to get the "linear interpolation to add third and fourth colour
            // CI2 if that's a name
            for (i = 4; i < 8; i++)
            {
                index[i] = 0;
            }
            if (_plt0.reverse_x)
            {
                for (h = 3; h >= 0; h--)
                {
                    for (w = 3; w >= 0; w--)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[7 - h] += (byte)(3 << (w << 1));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) * 0.0721 + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) * 0.7154 + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]) * 0.2125);
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
                for (h = 0; h < 4; h++)
                {
                    for (w = 0; w < 4; w++)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[4 + h] += (byte)(3 << (6 - (w << 1)));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) * 0.0721 + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) * 0.7154 + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]) * 0.2125);
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                        index[4 + h] += (byte)(diff_min_index << (6 - (w << 1)));
                    }
                }
            }
            else
            {
                for (h = 3; h >= 0; h--)
                {
                    for (w = 0; w < 4; w++)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[7 - h] += (byte)(3 << (6 - (w << 1)));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) * 0.0721 + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) * 0.7154 + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]) * 0.2125);
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                        index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                    }
                }
            }
            // index is overwritten each time
            // the lists need to be cleaned
            Colour_list.Clear();
            encounteredPairs.Clear();
            alpha_bitfield = 0;
            index_list.Add(index.ToArray());
        }
    }
    private void Wiimm_Algorithm_RGB(int startpoint, uint endpoint, List<byte[]> index_list)
    {
        bool block_has_alpha = false;
        byte palette_length;
        int j = 0;
        int x = 0;
        int y = 0;
        byte i;
        byte e;
        byte c;
        byte d;
        sbyte w;
        sbyte h;
        ushort z = 0;
        ushort width = 0;
        ushort diff_min0 = 65535;
        ushort diff0;
        ushort diff_min;
        ushort diff;
        byte diff_min_index = 0;
        byte diff_max_index = 0;
        byte red;
        byte green;
        byte blue;
        ushort alpha_bitfield = 0;
        ushort pixel;
        byte[] index = new byte[8];  // sub block length
        ushort[] Colour_array = { 1, 0, 0 };  // default value of Colour_List
        List<ushort[]> Colour_list = new List<ushort[]>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
        HashSet<Tuple<ushort, ushort>> encounteredPairs = new HashSet<Tuple<ushort, ushort>>();
        byte[] palette_rgba = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        byte[] rgb565 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  // 64 rgbx bytes, x is unused
        for (y = startpoint; y < endpoint; y += 4)
        {
            if (_plt0.alpha > 0 && bmp_image[y + _plt0.rgba_channel[3]] < _plt0.cmpr_alpha_threshold)  // no colour
            {
                alpha_bitfield += (ushort)(1 << (j + (z * 4)));
            }
            else  // opaque / no alpha / colours
            {
                red = bmp_image[y + _plt0.rgba_channel[0]];
                green = bmp_image[y + _plt0.rgba_channel[1]];
                blue = bmp_image[y + _plt0.rgba_channel[2]];
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
                Colour_array[0] = (ushort)((z << 4) + (j << 2)); // link to rgb565 array
                rgb565[Colour_array[0]] = (byte)(red & 0xf8);
                rgb565[Colour_array[0] + 1] = (byte)(green & 0xfc);
                rgb565[Colour_array[0] + 2] = (byte)(blue & 0xf8);
                pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                Colour_array[1] = pixel;
            }
            Colour_list.Add(Colour_array.ToArray());
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
                y += (_plt0.canvas_width << 2) - 16;
                x = 0;
            }
            else if (x == 2)
            {
                y += 16; // the cursor warped horizontally to the first block in width 4 lines above
            }
            else if (x == 4)
            {
                y -= (_plt0.canvas_width << 5) + 16; // minus 8 lines + point to next block
                x = 0;
            }
            else
            {
                y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
            }
            if (alpha_bitfield == 0xffff)  // save computation time I guess
            {
                index_list.Add(full_alpha_index); // this byte won't be changed so no need to copy it
                Colour_list.Clear();
                alpha_bitfield = 0;
                continue;
            }
            // now let's take the colour couple with the best result inside the 4x4 block
            diff_min = 0xffff;
            if (alpha_bitfield != 0)  // put the biggest ushort in second place - there are transparent pixels in this block
            {
                block_has_alpha = true;
                palette_length = 3;
                for (c = 0; c < 16; c++)  // useless to set it to 16 because of the condition c > i.
                {
                    for (d = (byte)(c + 1); d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2
                                                                // alpha padding
                        palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                        palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                        palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
                        // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 3 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                diff0 = (ushort)(Math.Abs(palette_rgba[e << 2] - rgb565[(i << 2)]) + Math.Abs(palette_rgba[(e << 2) + 1] - rgb565[(i << 2) + 1]) + Math.Abs(palette_rgba[(e << 2) + 2] - rgb565[(i << 2) + 2]));
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                        }
                    }
                }
            }
            else
            {
                palette_length = 4;
                for (c = 0; c < 16; c++)  // useless to set it to 16 because of the condition c > i.
                {
                    for (d = (byte)(c + 1); d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2

                        palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                        palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                        palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3));

                        palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                        palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                        palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3));
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 4 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                diff0 = (ushort)(Math.Abs(palette_rgba[e << 2] - rgb565[(i << 2)]) + Math.Abs(palette_rgba[(e << 2) + 1] - rgb565[(i << 2) + 1]) + Math.Abs(palette_rgba[(e << 2) + 2] - rgb565[(i << 2) + 2]));
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                            block_has_alpha = false;
                        }
                    }
                }
                palette_length = 3;
                for (c = 0; c < 16; c++)
                {
                    for (d = (byte)(c + 1); d < 16; d++)
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2

                        palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                        palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                        palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
                        // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 3 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                diff0 = (ushort)(Math.Abs(palette_rgba[e << 2] - rgb565[(i << 2)]) + Math.Abs(palette_rgba[(e << 2) + 1] - rgb565[(i << 2) + 1]) + Math.Abs(palette_rgba[(e << 2) + 2] - rgb565[(i << 2) + 2]));
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                            block_has_alpha = true;
                        }
                    }
                }
            }
            if (block_has_alpha)  // put the biggest ushort in second place
            {
                palette_length = 3;
                if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the second spot
                {
                    index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_max_index][1]);
                    index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_min_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_max_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_max_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_min_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_min_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue2
                }
                else
                {
                    index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_min_index][1]);
                    index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_max_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_min_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_min_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_max_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_max_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue2
                }
                palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);

                // last colour isn't in the palette, it's in _plt0.alpha_bitfield


            }
            else  // put biggest ushort in first place
            {
                palette_length = 4;
                // of course, that's the exact opposite!
                if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the first spot
                {
                    index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_min_index][1]);
                    index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_max_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_min_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_min_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_max_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_max_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue2
                }
                else
                {
                    index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_max_index][1]);
                    index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_min_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_max_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_max_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_min_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_min_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue2
                }

                palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3)); // the RGB565 third colour

                palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3)); // the RGB565 fourth colour
            }
            // time to get the "linear interpolation to add third and fourth colour
            // CI2 if that's a name
            for (i = 4; i < 8; i++)
            {
                index[i] = 0;
            }
            if (_plt0.reverse_x)
            {
                for (h = 3; h >= 0; h--)
                {
                    for (w = 3; w >= 0; w--)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[7 - h] += (byte)(3 << (w << 1));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
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
                for (h = 0; h < 4; h++)
                {
                    for (w = 0; w < 4; w++)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[4 + h] += (byte)(3 << (6 - (w << 1)));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]));
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                        index[4 + h] += (byte)(diff_min_index << (6 - (w << 1)));
                    }
                }
            }
            else
            {
                for (h = 3; h >= 0; h--)
                {
                    for (w = 0; w < 4; w++)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[7 - h] += (byte)(3 << (6 - (w << 1)));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            diff = (ushort)(Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]) + Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]) + Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]));
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                        index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                    }
                }
            }
            // index is overwritten each time
            // the lists need to be cleaned
            Colour_list.Clear();
            encounteredPairs.Clear();
            alpha_bitfield = 0;
            index_list.Add(index.ToArray());
        }
    }
    private void Wiimm_Algorithm_Euclidian(int startpoint, uint endpoint, List<byte[]> index_list)
    {
        bool block_has_alpha = false;
        byte palette_length;
        int j = 0;
        int x = 0;
        int y = 0;
        byte i;
        byte e;
        byte c;
        byte d;
        sbyte w;
        sbyte h;
        ushort z = 0;
        ushort width = 0;
        ushort diff_min0 = 65535;
        ushort diff0;
        ushort diff_min;
        ushort diff;
        byte diff_min_index = 0;
        byte diff_max_index = 0;
        byte red;
        byte green;
        byte blue;
        ushort alpha_bitfield = 0;
        ushort pixel;
        byte[] index = new byte[8];  // sub block length
        ushort[] Colour_array = { 1, 0, 0 };  // default value of Colour_List
        List<ushort[]> Colour_list = new List<ushort[]>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
        HashSet<Tuple<ushort, ushort>> encounteredPairs = new HashSet<Tuple<ushort, ushort>>();
        byte[] palette_rgba = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        byte[] rgb565 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  // 64 rgbx bytes, x is unused
        for (y = startpoint; y < endpoint; y += 4)
        {
            if (_plt0.alpha > 0 && bmp_image[y + _plt0.rgba_channel[3]] < _plt0.cmpr_alpha_threshold)  // no colour
            {
                alpha_bitfield += (ushort)(1 << (j + (z * 4)));
            }
            else  // opaque / no alpha / colours
            {
                red = bmp_image[y + _plt0.rgba_channel[0]];
                green = bmp_image[y + _plt0.rgba_channel[1]];
                blue = bmp_image[y + _plt0.rgba_channel[2]];
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
                Colour_array[0] = (ushort)((z << 4) + (j << 2)); // link to rgb565 array
                rgb565[Colour_array[0]] = (byte)(red & 0xf8);
                rgb565[Colour_array[0] + 1] = (byte)(green & 0xfc);
                rgb565[Colour_array[0] + 2] = (byte)(blue & 0xf8);
                pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                Colour_array[1] = pixel;
            }
            Colour_list.Add(Colour_array.ToArray());
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
                y += (_plt0.canvas_width << 2) - 16;
                x = 0;
            }
            else if (x == 2)
            {
                y += 16; // the cursor warped horizontally to the first block in width 4 lines above
            }
            else if (x == 4)
            {
                y -= (_plt0.canvas_width << 5) + 16; // minus 8 lines + point to next block
                x = 0;
            }
            else
            {
                y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
            }
            if (alpha_bitfield == 0xffff)  // save computation time I guess
            {
                index_list.Add(full_alpha_index); // this byte won't be changed so no need to copy it
                Colour_list.Clear();
                alpha_bitfield = 0;
                continue;
            }
            // now let's take the colour couple with the best result inside the 4x4 block
            diff_min = 0xffff;
            if (alpha_bitfield != 0)  // put the biggest ushort in second place - there are transparent pixels in this block
            {
                block_has_alpha = true;
                palette_length = 3;
                for (c = 0; c < 16; c++)  // useless to set it to 16 because of the condition c > i.
                {
                    for (d = (byte)(c + 1); d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2
                                                                // alpha padding
                        palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                        palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                        palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
                        // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 3 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                diff0 = (ushort)((int)(Math.Pow(palette_rgba[e << 2] - rgb565[(i << 2)], 2) + Math.Pow(palette_rgba[(e << 2) + 1] - rgb565[(i << 2) + 1], 2) + Math.Pow(palette_rgba[(e << 2) + 2] - rgb565[(i << 2) + 2], 2)) >> 2);
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                        }
                    }
                }
            }
            else
            {
                palette_length = 4;
                for (c = 0; c < 16; c++)  // useless to set it to 16 because of the condition c > i.
                {
                    for (d = (byte)(c + 1); d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2

                        palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                        palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                        palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3));

                        palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                        palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                        palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3));
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 4 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                diff0 = (ushort)((int)(Math.Pow(palette_rgba[e << 2] - rgb565[(i << 2)], 2) + Math.Pow(palette_rgba[(e << 2) + 1] - rgb565[(i << 2) + 1], 2) + Math.Pow(palette_rgba[(e << 2) + 2] - rgb565[(i << 2) + 2], 2)) >> 2);
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                            block_has_alpha = false;
                        }
                    }
                }
                palette_length = 3;
                for (c = 0; c < 16; c++)
                {
                    for (d = (byte)(c + 1); d < 16; d++)
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2

                        palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                        palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                        palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
                        // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 3 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                diff0 = (ushort)((int)(Math.Pow(palette_rgba[e << 2] - rgb565[(i << 2)], 2) + Math.Pow(palette_rgba[(e << 2) + 1] - rgb565[(i << 2) + 1], 2) + Math.Pow(palette_rgba[(e << 2) + 2] - rgb565[(i << 2) + 2], 2)) >> 2);
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                            block_has_alpha = true;
                        }
                    }
                }
            }
            if (block_has_alpha)  // put the biggest ushort in second place
            {
                palette_length = 3;
                if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the second spot
                {
                    index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_max_index][1]);
                    index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_min_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_max_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_max_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_min_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_min_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue2
                }
                else
                {
                    index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_min_index][1]);
                    index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_max_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_min_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_min_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_max_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_max_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue2
                }
                palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);

                // last colour isn't in the palette, it's in _plt0.alpha_bitfield


            }
            else  // put biggest ushort in first place
            {
                palette_length = 4;
                // of course, that's the exact opposite!
                if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the first spot
                {
                    index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_min_index][1]);
                    index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_max_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_min_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_min_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_max_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_max_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue2
                }
                else
                {
                    index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_max_index][1]);
                    index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_min_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_max_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_max_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_min_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_min_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue2
                }

                palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3)); // the RGB565 third colour

                palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3)); // the RGB565 fourth colour
            }
            // time to get the "linear interpolation to add third and fourth colour
            // CI2 if that's a name
            for (i = 4; i < 8; i++)
            {
                index[i] = 0;
            }
            if (_plt0.reverse_x)
            {
                for (h = 3; h >= 0; h--)
                {
                    for (w = 3; w >= 0; w--)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[7 - h] += (byte)(3 << (w << 1));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            diff = (ushort)((int)(Math.Pow(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)], 2) + Math.Pow(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1], 2) + Math.Pow(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2], 2)) >> 2);
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
                for (h = 0; h < 4; h++)
                {
                    for (w = 0; w < 4; w++)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[4 + h] += (byte)(3 << (6 - (w << 1)));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            diff = (ushort)((int)(Math.Pow(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)], 2) + Math.Pow(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1], 2) + Math.Pow(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2], 2)) >> 2);
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                        index[4 + h] += (byte)(diff_min_index << (6 - (w << 1)));
                    }
                }
            }
            else
            {
                for (h = 3; h >= 0; h--)
                {
                    for (w = 0; w < 4; w++)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[7 - h] += (byte)(3 << (6 - (w << 1)));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            diff = (ushort)((int)(Math.Pow(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)], 2) + Math.Pow(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1], 2) + Math.Pow(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2], 2)) >> 2);
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                        index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                    }
                }
            }
            // index is overwritten each time
            // the lists need to be cleaned
            Colour_list.Clear();
            encounteredPairs.Clear();
            alpha_bitfield = 0;
            index_list.Add(index.ToArray());
        }
    }
    private void Wiimm_Algorithm_Infinite(int startpoint, uint endpoint, List<byte[]> index_list)
    {
        bool block_has_alpha = false;
        byte palette_length;
        int j = 0;
        int x = 0;
        int y = 0;
        byte i;
        byte e;
        byte c;
        byte d;
        sbyte w;
        sbyte h;
        ushort z = 0;
        ushort width = 0;
        ushort diff_min0 = 65535;
        ushort diff0;
        ushort diff_min;
        ushort diff;
        ushort diff_red;
        ushort diff_green;
        ushort diff_blue;
        byte diff_min_index = 0;
        byte diff_max_index = 0;
        byte red;
        byte green;
        byte blue;
        ushort alpha_bitfield = 0;
        ushort pixel;
        byte[] index = new byte[8];  // sub block length
        ushort[] Colour_array = { 1, 0, 0 };  // default value of Colour_List
        List<ushort[]> Colour_list = new List<ushort[]>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
        HashSet<Tuple<ushort, ushort>> encounteredPairs = new HashSet<Tuple<ushort, ushort>>();
        byte[] palette_rgba = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        byte[] rgb565 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  // 64 rgbx bytes, x is unused
        for (y = startpoint; y < endpoint; y += 4)
        {
            if (_plt0.alpha > 0 && bmp_image[y + _plt0.rgba_channel[3]] < _plt0.cmpr_alpha_threshold)  // no colour
            {
                alpha_bitfield += (ushort)(1 << (j + (z * 4)));
            }
            else  // opaque / no alpha / colours
            {
                red = bmp_image[y + _plt0.rgba_channel[0]];
                green = bmp_image[y + _plt0.rgba_channel[1]];
                blue = bmp_image[y + _plt0.rgba_channel[2]];
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
                Colour_array[0] = (ushort)((z << 4) + (j << 2)); // link to rgb565 array
                rgb565[Colour_array[0]] = (byte)(red & 0xf8);
                rgb565[Colour_array[0] + 1] = (byte)(green & 0xfc);
                rgb565[Colour_array[0] + 2] = (byte)(blue & 0xf8);
                pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                Colour_array[1] = pixel;
            }
            Colour_list.Add(Colour_array.ToArray());
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
                y += (_plt0.canvas_width << 2) - 16;
                x = 0;
            }
            else if (x == 2)
            {
                y += 16; // the cursor warped horizontally to the first block in width 4 lines above
            }
            else if (x == 4)
            {
                y -= (_plt0.canvas_width << 5) + 16; // minus 8 lines + point to next block
                x = 0;
            }
            else
            {
                y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
            }
            if (alpha_bitfield == 0xffff)  // save computation time I guess
            {
                index_list.Add(full_alpha_index); // this byte won't be changed so no need to copy it
                Colour_list.Clear();
                alpha_bitfield = 0;
                continue;
            }
            // now let's take the colour couple with the best result inside the 4x4 block
            diff_min = 0xffff;
            if (alpha_bitfield != 0)  // put the biggest ushort in second place - there are transparent pixels in this block
            {
                block_has_alpha = true;
                palette_length = 3;
                for (c = 0; c < 16; c++)  // useless to set it to 16 because of the condition c > i.
                {
                    for (d = (byte)(c + 1); d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2
                                                                // alpha padding
                        palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                        palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                        palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
                        // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 3 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                diff_red = (ushort)Math.Abs(palette_rgba[e << 2] - rgb565[(i << 2)]);
                                diff_green = (ushort)Math.Abs(palette_rgba[(e << 2) + 1] - rgb565[(i << 2) + 1]);
                                diff_blue = (ushort)Math.Abs(palette_rgba[(e << 2) + 2] - rgb565[(i << 2) + 2]);
                                if (diff_red > diff_green)  // takes the biggest value between red, green, and blue
                                {
                                    if (diff_red > diff_blue)
                                    {
                                        diff0 = diff_red;
                                    }
                                    else
                                    {
                                        diff0 = diff_blue;
                                    }
                                }
                                else
                                {
                                    if (diff_green > diff_blue)
                                    {
                                        diff0 = diff_green;
                                    }
                                    else
                                    {
                                        diff0 = diff_blue;
                                    }
                                }
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                        }
                    }
                }
            }
            else
            {
                palette_length = 4;
                for (c = 0; c < 16; c++)  // useless to set it to 16 because of the condition c > i.
                {
                    for (d = (byte)(c + 1); d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2

                        palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                        palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                        palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3));

                        palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                        palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                        palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3));
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 4 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                diff_red = (ushort)Math.Abs(palette_rgba[e << 2] - rgb565[(i << 2)]);
                                diff_green = (ushort)Math.Abs(palette_rgba[(e << 2) + 1] - rgb565[(i << 2) + 1]);
                                diff_blue = (ushort)Math.Abs(palette_rgba[(e << 2) + 2] - rgb565[(i << 2) + 2]);
                                if (diff_red > diff_green)  // takes the biggest value between red, green, and blue
                                {
                                    if (diff_red > diff_blue)
                                    {
                                        diff0 = diff_red;
                                    }
                                    else
                                    {
                                        diff0 = diff_blue;
                                    }
                                }
                                else
                                {
                                    if (diff_green > diff_blue)
                                    {
                                        diff0 = diff_green;
                                    }
                                    else
                                    {
                                        diff0 = diff_blue;
                                    }
                                }
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                            block_has_alpha = false;
                        }
                    }
                }
                palette_length = 3;
                for (c = 0; c < 16; c++)
                {
                    for (d = (byte)(c + 1); d < 16; d++)
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2

                        palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                        palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                        palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
                        // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 3 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                diff_red = (ushort)Math.Abs(palette_rgba[e << 2] - rgb565[(i << 2)]);
                                diff_green = (ushort)Math.Abs(palette_rgba[(e << 2) + 1] - rgb565[(i << 2) + 1]);
                                diff_blue = (ushort)Math.Abs(palette_rgba[(e << 2) + 2] - rgb565[(i << 2) + 2]);
                                if (diff_red > diff_green)  // takes the biggest value between red, green, and blue
                                {
                                    if (diff_red > diff_blue)
                                    {
                                        diff0 = diff_red;
                                    }
                                    else
                                    {
                                        diff0 = diff_blue;
                                    }
                                }
                                else
                                {
                                    if (diff_green > diff_blue)
                                    {
                                        diff0 = diff_green;
                                    }
                                    else
                                    {
                                        diff0 = diff_blue;
                                    }
                                }
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                            block_has_alpha = true;
                        }
                    }
                }
            }
            if (block_has_alpha)  // put the biggest ushort in second place
            {
                palette_length = 3;
                if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the second spot
                {
                    index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_max_index][1]);
                    index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_min_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_max_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_max_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_min_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_min_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue2
                }
                else
                {
                    index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_min_index][1]);
                    index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_max_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_min_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_min_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_max_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_max_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue2
                }
                palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);

                // last colour isn't in the palette, it's in _plt0.alpha_bitfield


            }
            else  // put biggest ushort in first place
            {
                palette_length = 4;
                // of course, that's the exact opposite!
                if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the first spot
                {
                    index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_min_index][1]);
                    index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_max_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_min_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_min_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_max_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_max_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue2
                }
                else
                {
                    index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_max_index][1]);
                    index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_min_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_max_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_max_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_min_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_min_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue2
                }

                palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3)); // the RGB565 third colour

                palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3)); // the RGB565 fourth colour
            }
            // time to get the "linear interpolation to add third and fourth colour
            // CI2 if that's a name
            for (i = 4; i < 8; i++)
            {
                index[i] = 0;
            }
            if (_plt0.reverse_x)
            {
                for (h = 3; h >= 0; h--)
                {
                    for (w = 3; w >= 0; w--)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[7 - h] += (byte)(3 << (w << 1));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            diff_red = (ushort)Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]);
                            diff_green = (ushort)Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]);
                            diff_blue = (ushort)Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]);
                            if (diff_red > diff_green)  // takes the biggest value between red, green, and blue
                            {
                                if (diff_red > diff_blue)
                                {
                                    diff = diff_red;
                                }
                                else
                                {
                                    diff = diff_blue;
                                }
                            }
                            else
                            {
                                if (diff_green > diff_blue)
                                {
                                    diff = diff_green;
                                }
                                else
                                {
                                    diff = diff_blue;
                                }
                            }
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
                for (h = 0; h < 4; h++)
                {
                    for (w = 0; w < 4; w++)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[4 + h] += (byte)(3 << (6 - (w << 1)));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            diff_red = (ushort)Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]);
                            diff_green = (ushort)Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]);
                            diff_blue = (ushort)Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]);
                            if (diff_red > diff_green)  // takes the biggest value between red, green, and blue
                            {
                                if (diff_red > diff_blue)
                                {
                                    diff = diff_red;
                                }
                                else
                                {
                                    diff = diff_blue;
                                }
                            }
                            else
                            {
                                if (diff_green > diff_blue)
                                {
                                    diff = diff_green;
                                }
                                else
                                {
                                    diff = diff_blue;
                                }
                            }
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                        index[4 + h] += (byte)(diff_min_index << (6 - (w << 1)));
                    }
                }
            }
            else
            {
                for (h = 3; h >= 0; h--)
                {
                    for (w = 0; w < 4; w++)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[7 - h] += (byte)(3 << (6 - (w << 1)));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            diff_red = (ushort)Math.Abs(palette_rgba[i << 2] - rgb565[(h << 4) + (w << 2)]);
                            diff_green = (ushort)Math.Abs(palette_rgba[(i << 2) + 1] - rgb565[(h << 4) + (w << 2) + 1]);
                            diff_blue = (ushort)Math.Abs(palette_rgba[(i << 2) + 2] - rgb565[(h << 4) + (w << 2) + 2]);
                            if (diff_red > diff_green)  // takes the biggest value between red, green, and blue
                            {
                                if (diff_red > diff_blue)
                                {
                                    diff = diff_red;
                                }
                                else
                                {
                                    diff = diff_blue;
                                }
                            }
                            else
                            {
                                if (diff_green > diff_blue)
                                {
                                    diff = diff_green;
                                }
                                else
                                {
                                    diff = diff_blue;
                                }
                            }
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                        index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                    }
                }
            }
            // index is overwritten each time
            // the lists need to be cleaned
            Colour_list.Clear();
            encounteredPairs.Clear();
            alpha_bitfield = 0;
            index_list.Add(index.ToArray());
        }
    }
    private void Wiimm_Algorithm_Delta_E(int startpoint, uint endpoint, List<byte[]> index_list)
    {
        bool block_has_alpha = false;
        byte palette_length;
        int j = 0;
        int x = 0;
        int y = 0;
        byte i;
        byte e;
        byte c;
        byte d;
        sbyte w;
        sbyte h;
        ushort z = 0;
        ushort width = 0;
        double diff_min0 = 65535;
        double diff0;
        double diff_min;
        double diff;
        byte diff_min_index = 0;
        byte diff_max_index = 0;
        byte red;
        byte green;
        byte blue;
        byte[] rgb1 = { 0, 0, 0 };
        byte[] rgb2 = { 0, 0, 0 };
        ushort alpha_bitfield = 0;
        ushort pixel;
        byte[] index = new byte[8];  // sub block length
        ushort[] Colour_array = { 1, 0, 0 };  // default value of Colour_List
        List<ushort[]> Colour_list = new List<ushort[]>();  // a list of every 2 bytes pixel transformed to correspond to the current colour format
        HashSet<Tuple<ushort, ushort>> encounteredPairs = new HashSet<Tuple<ushort, ushort>>();
        byte[] palette_rgba = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        byte[] rgb565 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  // 64 rgbx bytes, x is unused
        for (y = startpoint; y < endpoint; y += 4)
        {
            if (_plt0.alpha > 0 && bmp_image[y + _plt0.rgba_channel[3]] < _plt0.cmpr_alpha_threshold)  // no colour
            {
                alpha_bitfield += (ushort)(1 << (j + (z * 4)));
            }
            else  // opaque / no alpha / colours
            {
                red = bmp_image[y + _plt0.rgba_channel[0]];
                green = bmp_image[y + _plt0.rgba_channel[1]];
                blue = bmp_image[y + _plt0.rgba_channel[2]];
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
                Colour_array[0] = (ushort)((z << 4) + (j << 2)); // link to rgb565 array
                rgb565[Colour_array[0]] = (byte)(red & 0xf8);
                rgb565[Colour_array[0] + 1] = (byte)(green & 0xfc);
                rgb565[Colour_array[0] + 2] = (byte)(blue & 0xf8);
                pixel = (ushort)(((red >> 3) << 11) + ((green >> 2) << 5) + (blue >> 3)); // the RGB565 colour
                Colour_array[1] = pixel;
            }
            Colour_list.Add(Colour_array.ToArray());
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
                y += (_plt0.canvas_width << 2) - 16;
                x = 0;
            }
            else if (x == 2)
            {
                y += 16; // the cursor warped horizontally to the first block in width 4 lines above
            }
            else if (x == 4)
            {
                y -= (_plt0.canvas_width << 5) + 16; // minus 8 lines + point to next block
                x = 0;
            }
            else
            {
                y -= ((_plt0.canvas_width << 4)) + 16;  // substract 4 lines and goes one block to the left
            }
            if (alpha_bitfield == 0xffff)  // save computation time I guess
            {
                index_list.Add(full_alpha_index); // this byte won't be changed so no need to copy it
                Colour_list.Clear();
                alpha_bitfield = 0;
                continue;
            }
            // now let's take the colour couple with the best result inside the 4x4 block
            diff_min = 0xffff;
            if (alpha_bitfield != 0)  // put the biggest ushort in second place - there are transparent pixels in this block
            {
                block_has_alpha = true;
                palette_length = 3;
                for (c = 0; c < 16; c++)  // useless to set it to 16 because of the condition c > i.
                {
                    for (d = (byte)(c + 1); d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2
                                                                // alpha padding
                        palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                        palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                        palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
                        // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 3 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                rgb1[0] = palette_rgba[e << 2];
                                rgb2[0] = rgb565[(i << 2)];
                                rgb1[1] = palette_rgba[(e << 2) + 1];
                                rgb2[1] = rgb565[(i << 2) + 1];
                                rgb1[2] = palette_rgba[(e << 2) + 2];
                                rgb2[2] = rgb565[(i << 2) + 2];
                                diff0 = CalculateCIEDE2000(rgb1, rgb2);
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                        }
                    }
                }
            }
            else
            {
                palette_length = 4;
                for (c = 0; c < 16; c++)  // useless to set it to 16 because of the condition c > i.
                {
                    for (d = (byte)(c + 1); d < 16; d++)  // useless to set it to 16 because of the condition d > i.
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2

                        palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                        palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                        palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3));

                        palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                        palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                        palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3));
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 4 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                rgb1[0] = palette_rgba[e << 2];
                                rgb2[0] = rgb565[(i << 2)];
                                rgb1[1] = palette_rgba[(e << 2) + 1];
                                rgb2[1] = rgb565[(i << 2) + 1];
                                rgb1[2] = palette_rgba[(e << 2) + 2];
                                rgb2[2] = rgb565[(i << 2) + 2];
                                diff0 = CalculateCIEDE2000(rgb1, rgb2);
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                            block_has_alpha = false;
                        }
                    }
                }
                palette_length = 3;
                for (c = 0; c < 16; c++)
                {
                    for (d = (byte)(c + 1); d < 16; d++)
                    {
                        if (((alpha_bitfield >> c) & 1) == 1 || ((alpha_bitfield >> d) & 1) == 1)
                            continue;
                        Tuple<ushort, ushort> bytePair = Tuple.Create(Colour_list[c][1], Colour_list[d][1]); // Create a tuple to represent the byte pair

                        // Check if the byte pair has been encountered before
                        if (encounteredPairs.Contains(bytePair))
                        {
                            // Console.WriteLine("This byte pair has been encountered before.");
                            continue;
                        }
                        else
                        {
                            encounteredPairs.Add(bytePair);
                            // Console.WriteLine("This byte pair is encountered for the first time.");
                        }
                        index[0] = (byte)(Colour_list[c][1] >> 8);
                        index[1] = (byte)(Colour_list[c][1]);
                        index[2] = (byte)(Colour_list[d][1] >> 8);
                        index[3] = (byte)(Colour_list[d][1]);
                        palette_rgba[0] = rgb565[(c << 2)];  // red
                        palette_rgba[1] = rgb565[(c << 2) + 1]; // green
                        palette_rgba[2] = rgb565[(c << 2) + 2]; // blue
                                                                // alpha padding
                        palette_rgba[4] = rgb565[(d << 2)]; // red2
                        palette_rgba[5] = rgb565[(d << 2) + 1]; // green2
                        palette_rgba[6] = rgb565[(d << 2) + 2]; // blue2

                        palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                        palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                        palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);
                        // last colour isn't in the palette, it's in _plt0.alpha_bitfield
                        diff = 0;  // still a short because the theoritical max value is 12240 (which is 255 * 3 * 16)

                        for (i = 0; i < 16; i++)
                        {
                            if (((alpha_bitfield >> i) & 1) == 1)
                            {
                                continue;  // this pixel will be transparent no matter what, it won't add any difference 
                            }
                            diff_min0 = 0xffff;
                            // palette length = 3 here
                            for (e = 0; e < palette_length; e++)  // process the colour palette to find the closest colour corresponding to the current pixel
                            { // calculate difference between each separate colour channel and store the sum
                                rgb1[0] = palette_rgba[e << 2];
                                rgb2[0] = rgb565[(i << 2)];
                                rgb1[1] = palette_rgba[(e << 2) + 1];
                                rgb2[1] = rgb565[(i << 2) + 1];
                                rgb1[2] = palette_rgba[(e << 2) + 2];
                                rgb2[2] = rgb565[(i << 2) + 2];
                                diff0 = CalculateCIEDE2000(rgb1, rgb2);
                                if (diff0 < diff_min0)
                                {
                                    diff_min0 = diff0;
                                }
                            }
                            diff += diff_min0;
                        }
                        if (diff < diff_min)
                        {
                            diff_min = diff;
                            diff_min_index = c;
                            diff_max_index = d;
                            block_has_alpha = true;
                        }
                    }
                }
            }
            if (block_has_alpha)  // put the biggest ushort in second place
            {
                palette_length = 3;
                if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the second spot
                {
                    index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_max_index][1]);
                    index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_min_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_max_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_max_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_min_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_min_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue2
                }
                else
                {
                    index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_min_index][1]);
                    index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_max_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_min_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_min_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_max_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_max_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue2
                }
                palette_rgba[8] = (byte)((palette_rgba[0] + palette_rgba[4]) >> 1);
                palette_rgba[9] = (byte)((palette_rgba[1] + palette_rgba[5]) >> 1);
                palette_rgba[10] = (byte)((palette_rgba[2] + palette_rgba[6]) >> 1);

                // last colour isn't in the palette, it's in _plt0.alpha_bitfield


            }
            else  // put biggest ushort in first place
            {
                palette_length = 4;
                // of course, that's the exact opposite!
                if (Colour_list[diff_min_index][1] > Colour_list[diff_max_index][1])  // put diff_min at the first spot
                {
                    index[0] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_min_index][1]);
                    index[2] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_max_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_min_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_min_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_max_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_max_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue2
                }
                else
                {
                    index[0] = (byte)(Colour_list[diff_max_index][1] >> 8);
                    index[1] = (byte)(Colour_list[diff_max_index][1]);
                    index[2] = (byte)(Colour_list[diff_min_index][1] >> 8);
                    index[3] = (byte)(Colour_list[diff_min_index][1]);
                    palette_rgba[0] = rgb565[Colour_list[diff_max_index][0]]; // red
                    palette_rgba[1] = rgb565[Colour_list[diff_max_index][0] + 1]; // green
                    palette_rgba[2] = rgb565[Colour_list[diff_max_index][0] + 2]; // blue
                    palette_rgba[4] = rgb565[Colour_list[diff_min_index][0]]; // red2
                    palette_rgba[5] = rgb565[Colour_list[diff_min_index][0] + 1]; // green2
                    palette_rgba[6] = rgb565[Colour_list[diff_min_index][0] + 2]; // blue2
                }

                palette_rgba[8] = (byte)(((palette_rgba[0] << 1) / 3) + (palette_rgba[4] / 3));
                palette_rgba[9] = (byte)(((palette_rgba[1] << 1) / 3) + (palette_rgba[5] / 3));
                palette_rgba[10] = (byte)(((palette_rgba[2] << 1) / 3) + (palette_rgba[6] / 3)); // the RGB565 third colour

                palette_rgba[12] = (byte)((palette_rgba[0] / 3) + ((palette_rgba[4] << 1) / 3));
                palette_rgba[13] = (byte)((palette_rgba[1] / 3) + ((palette_rgba[5] << 1) / 3));
                palette_rgba[14] = (byte)((palette_rgba[2] / 3) + ((palette_rgba[6] << 1) / 3)); // the RGB565 fourth colour
            }
            // time to get the "linear interpolation to add third and fourth colour
            // CI2 if that's a name
            for (i = 4; i < 8; i++)
            {
                index[i] = 0;
            }
            if (_plt0.reverse_x)
            {
                for (h = 3; h >= 0; h--)
                {
                    for (w = 3; w >= 0; w--)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[7 - h] += (byte)(3 << (w << 1));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            rgb1[0] = palette_rgba[i << 2];
                            rgb2[0] = rgb565[(h << 4) + (w << 2)];
                            rgb1[1] = palette_rgba[(i << 2) + 1];
                            rgb2[1] = rgb565[(h << 4) + (w << 2) + 1];
                            rgb1[2] = palette_rgba[(i << 2) + 2];
                            rgb2[2] = rgb565[(h << 4) + (w << 2) + 2];
                            diff = CalculateCIEDE2000(rgb1, rgb2);
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
                for (h = 0; h < 4; h++)
                {
                    for (w = 0; w < 4; w++)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[4 + h] += (byte)(3 << (6 - (w << 1)));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            rgb1[0] = palette_rgba[i << 2];
                            rgb2[0] = rgb565[(h << 4) + (w << 2)];
                            rgb1[1] = palette_rgba[(i << 2) + 1];
                            rgb2[1] = rgb565[(h << 4) + (w << 2) + 1];
                            rgb1[2] = palette_rgba[(i << 2) + 2];
                            rgb2[2] = rgb565[(h << 4) + (w << 2) + 2];
                            diff = CalculateCIEDE2000(rgb1, rgb2);
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                        index[4 + h] += (byte)(diff_min_index << (6 - (w << 1)));
                    }
                }
            }
            else
            {
                for (h = 3; h >= 0; h--)
                {
                    for (w = 0; w < 4; w++)  // index_size = number of pixels
                    {
                        if (((alpha_bitfield >> (h << 2) + w) & 1) == 1)
                        {
                            index[7 - h] += (byte)(3 << (6 - (w << 1)));
                            continue;
                        }
                        diff_min = 0xffff;
                        for (i = 0; i < palette_length; i++)  // process the colour palette to find the closest colour corresponding to the current pixel
                        { // calculate difference between each separate colour channel and store the sum
                            rgb1[0] = palette_rgba[i << 2];
                            rgb2[0] = rgb565[(h << 4) + (w << 2)];
                            rgb1[1] = palette_rgba[(i << 2) + 1];
                            rgb2[1] = rgb565[(h << 4) + (w << 2) + 1];
                            rgb1[2] = palette_rgba[(i << 2) + 2];
                            rgb2[2] = rgb565[(h << 4) + (w << 2) + 2];
                            diff = CalculateCIEDE2000(rgb1, rgb2);
                            if (diff < diff_min)
                            {
                                diff_min = diff;
                                diff_min_index = i;
                            }
                        }
                        index[7 - h] += (byte)(diff_min_index << (6 - (w << 1)));
                    }
                }
            }
            // index is overwritten each time
            // the lists need to be cleaned
            Colour_list.Clear();
            encounteredPairs.Clear();
            alpha_bitfield = 0;
            index_list.Add(index.ToArray());
        }
    }

    public static double CalculateCIEDE2000(byte[] rgb1, byte[] rgb2)
    {
        double[] lab1 = RGBToLab(rgb1);
        double[] lab2 = RGBToLab(rgb2);

        double deltaE = CalculateCIEDE2000(lab1, lab2);

        return deltaE;
    }

    public static double[] RGBToLab(byte[] rgb)
    {
        double[] xyz = RGBToXYZ(rgb);
        double[] lab = XYZToLab(xyz);

        return lab;
    }

    public static double[] RGBToXYZ(byte[] rgb)
    {
        double[] linearRGB = new double[3];
        for (int i = 0; i < 3; i++)
        {
            linearRGB[i] = rgb[i] / 255.0;
            if (linearRGB[i] <= 0.04045)
                linearRGB[i] = linearRGB[i] / 12.92;
            else
                linearRGB[i] = Math.Pow((linearRGB[i] + 0.055) / 1.055, 2.4);
        }

        double[] xyz = new double[3];
        xyz[0] = linearRGB[0] * 0.4124564 + linearRGB[1] * 0.3575761 + linearRGB[2] * 0.1804375;
        xyz[1] = linearRGB[0] * 0.2126729 + linearRGB[1] * 0.7151522 + linearRGB[2] * 0.0721750;
        xyz[2] = linearRGB[0] * 0.0193339 + linearRGB[1] * 0.1191920 + linearRGB[2] * 0.9503041;

        return xyz;
    }

    public static double[] XYZToLab(double[] xyz)
    {
        double[] normalizedXYZ = new double[3];
        for (int i = 0; i < 3; i++)
        {
            normalizedXYZ[i] = xyz[i] / XYZReference[i];
        }

        double[] lab = new double[3];
        for (int i = 0; i < 3; i++)
        {
            if (normalizedXYZ[i] > Epsilon)
                lab[i] = 116 * Math.Pow(normalizedXYZ[i], 1.0 / 3.0) - 16;
            else
                lab[i] = Kappa * normalizedXYZ[i];
        }

        return lab;
    }

    public static double CalculateCIEDE2000(double[] lab1, double[] lab2)
    {
        double L1 = lab1[0];
        double a1 = lab1[1];
        double b1 = lab1[2];

        double L2 = lab2[0];
        double a2 = lab2[1];
        double b2 = lab2[2];

        double deltaL = L2 - L1;
        double C1 = Math.Sqrt(a1 * a1 + b1 * b1);
        double C2 = Math.Sqrt(a2 * a2 + b2 * b2);
        double deltaC = C2 - C1;
        double deltaA = a2 - a1;
        double deltaB = b2 - b1;

        double deltaH = Math.Sqrt(deltaA * deltaA + deltaB * deltaB - deltaC * deltaC);

        double SL = 1.0;
        double SC = 1.0 + K1 * C1;
        double SH = 1.0 + K2 * C1;

        double deltaTheta = Math.Atan2(b2, a2) - Math.Atan2(b1, a1);
        if (deltaTheta > Math.PI)
            deltaTheta -= 2 * Math.PI;
        else if (deltaTheta < -Math.PI)
            deltaTheta += 2 * Math.PI;

        double deltaE = Math.Sqrt(
            Math.Pow(deltaL / (SL * Kl), 2) +
            Math.Pow(deltaC / (SC * Kc), 2) +
            Math.Pow(deltaH / (SH * Kh), 2) +
            (Rt * (deltaC / (SC * Kc)) * (deltaH / (SH * Kh))));

        return deltaE;
    }

    // Constants for CIEDE2000 calculation
    private const double Epsilon = 0.008856;
    private const double Kappa = 903.3;
    private static readonly double[] XYZReference = { 95.047, 100.000, 108.883 };
    private const double Kl = 1.0;
    private const double Kc = 1.0;
    private const double Kh = 1.0;
    private const double K1 = 0.045;
    private const double K2 = 0.015;
    private const double Rt = 0.016;
}
