using System.Collections.Generic;
using System.Linq;

class Fill_index_list_class
{
    Decode_texture_class _dec;
    public Fill_index_list_class(Decode_texture_class Decode_texture_class)
    {
        _dec = Decode_texture_class;
    }
    public List<List<byte[]>> Fill_index_list(byte[] data, int start, byte texture_format3, byte mipmaps_number, byte[] real_block_width_array, byte[] block_width_array, byte[] block_height_array, bool reverse)
    {
        int blocks_wide;
        int blocks_tall;
        int cursor = start;
        int count = 0;
        List<byte[]> index_list = new List<byte[]>();
        List<List<byte[]>> index_list_list = new List<List<byte[]>>();
        ushort[] canvas = { _dec.canvas_dim[0][0], _dec.canvas_dim[0][1], _dec.canvas_dim[0][2], _dec.canvas_dim[0][3] };
        switch (texture_format3)
        {
            case 6:  // RGBA32
                {
                    byte[] index = new byte[_dec.canvas_dim[0][2] * 4];
                    for (byte m = 0; m <= mipmaps_number; m++)
                    {
                        blocks_wide = _dec.canvas_dim[m][2] / 4;
                        blocks_tall = _dec.canvas_dim[m][3] / 4;
                        for (int t = 0; t < blocks_tall; t++)
                        {
                            for (int h = 0; h < 4; h++) // height of a block - the number of lines
                            {
                                for (int b = 0; b < blocks_wide; b++)
                                {
                                    for (int w = 0; w < 8; w++)
                                    {
                                        index[count] = data[cursor];
                                        cursor++;
                                        count++;
                                    }
                                    cursor += 24;
                                    for (int w = 0; w < 8; w++)
                                    {
                                        index[count] = data[cursor];
                                        cursor++;
                                        count++;
                                    }
                                    cursor += 24;
                                }
                                index_list.Add(index.ToArray());
                                count = 0;
                                // cursor -= (blocks_wide * block_height_array[texture_format3] * block_width_array[texture_format3]) - block_width_array[texture_format3];  // goes back to the left-most block
                                cursor -= (blocks_wide << 6) - 8;  // same but better
                            }  // 4 * 16 == 64 == 1 << 6
                            cursor += ((blocks_wide - 1) << 6) + 32;  // counters the last two operations beacause we've just filled a whole width of block.
                        }
                        if (!reverse)
                        {
                            index_list.Reverse();
                        }
                        index_list_list.Add(index_list.ToList());
                        if (m + 1 <= mipmaps_number)
                        {
                            canvas[0] >>= 1; // divides by 2
                            canvas[1] >>= 1; // divides by 2   - also YES 1 DIVIDED BY TWO IS ZERO
                            canvas[2] = (ushort)(canvas[0] + (4 - (canvas[0] % 4) % 4));
                            canvas[3] = (ushort)(canvas[1] + (4 - (canvas[1] % 4) % 4));
                            _dec.canvas_dim.Add(canvas.ToArray());
                            index_list.Clear();
                        }
                    }
                    break;
                }
            case 0xe:  // CMPR
                {
                    byte[] index = new byte[8];
                    for (byte m = 0; m <= mipmaps_number; m++)
                    {
                        blocks_wide = _dec.canvas_dim[m][2] / 8;
                        blocks_tall = _dec.canvas_dim[m][3] / 8;
                        for (int b = 0; b < (blocks_tall * blocks_wide) << 2; b++)
                        {
                            for (count = 0; count < 8; count++)
                            {
                                index[count] = data[cursor];
                                cursor++;
                            }
                            index_list.Add(index.ToArray());

                        }
                        if (!reverse)
                        {
                            index_list.Reverse();
                        }
                        index_list_list.Add(index_list.ToList());
                        if (m + 1 <= mipmaps_number)
                        {
                            canvas[0] >>= 1; // divides by 2
                            canvas[1] >>= 1; // divides by 2   - also YES 1 DIVIDED BY TWO IS ZERO
                            canvas[2] = (ushort)(canvas[0] + (8 - (canvas[0] % 8) % 8));
                            canvas[3] = (ushort)(canvas[1] + (8 - (canvas[1] % 8) % 8));
                            _dec.canvas_dim.Add(canvas.ToArray());
                            index_list.Clear();
                        }
                    }
                    break;
                }
            default:
                {
                    int len = _dec.canvas_dim[0][2];
                    switch (texture_format3)
                    {
                        case 0:  // I4
                        case 8: // CI4
                            {
                                len >>= 1;  // 4-bit per pixel
                                break;
                            }
                        /* 
                          case 9:  // CI8
                        case 1: // I8
                        case 2: // AI4
                            nothing happens
                       */
                        case 3:  // IA8
                        case 4:  // RGB565
                        case 5:  // RGB5A3
                        case 10: // CI14x2
                            {
                                len <<= 1; // 16bpp
                                break;
                            }
                        /* these are already treated in their own case
                        case 6:  // RGBA32
                        case 0xE:  // CMPR
                        */
                    }
                    byte[] index = new byte[len];
                    for (byte m = 0; m <= mipmaps_number; m++)
                    {
                        blocks_wide = _dec.canvas_dim[m][2] / real_block_width_array[texture_format3];
                        blocks_tall = _dec.canvas_dim[m][3] / block_height_array[texture_format3];
                        for (int t = 0; t < blocks_tall; t++)
                        {
                            for (byte h = 0; h < block_height_array[texture_format3]; h++)
                            {

                                for (int b = 0; b < blocks_wide; b++)
                                {
                                    for (byte w = 0; w < block_width_array[texture_format3]; w++)
                                    {
                                        index[count] = data[cursor];
                                        count++;
                                        cursor++;
                                    }
                                    cursor += (block_height_array[texture_format3] - 1) * block_width_array[texture_format3];  // goes to next right block to fill the line horizontally
                                }
                                index_list.Add(index.ToArray());
                                count = 0;
                                cursor -= (blocks_wide * block_height_array[texture_format3] * block_width_array[texture_format3]) - block_width_array[texture_format3];  // goes back to the left-most block
                            }
                            cursor += ((blocks_wide - 1) * block_height_array[texture_format3] * block_width_array[texture_format3]);  // counters the last two operations beacause we've just filled a whole width of block.
                        }
                        if (!reverse)
                        {
                            index_list.Reverse();
                        }
                        index_list_list.Add(index_list.ToList());
                        if (m + 1 <= mipmaps_number)
                        {
                            canvas[0] >>= 1; // divides by 2
                            canvas[1] >>= 1; // divides by 2   - also YES 1 DIVIDED BY TWO IS ZERO
                            canvas[2] = (ushort)(canvas[0] + ((real_block_width_array[texture_format3] - (canvas[0] % real_block_width_array[texture_format3])) % real_block_width_array[texture_format3]));
                            canvas[3] = (ushort)(canvas[1] + ((block_height_array[texture_format3] - (canvas[1] % block_height_array[texture_format3])) % block_height_array[texture_format3]));
                            _dec.canvas_dim.Add(canvas.ToArray());
                            index_list.Clear();
                        }
                    }
                    break;
                }
        }
        return index_list_list;
    }
}