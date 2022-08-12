class Fill_index_list_class
{
    static public List<List<byte[]>> Fill_index_list(byte[] data, int start, byte texture_format3, byte mipmaps_number, List<ushort[]> canvas_dim, byte[] real_block_width_array, byte[] block_width_array, byte[] block_height_array)
    {
        int blocks_wide;
        int blocks_tall;
        int cursor = start;
        int count = 0;
        ushort[] canvas = { canvas_dim[0][0], canvas_dim[0][1], canvas_dim[0][2], canvas_dim[0][3] };
        switch (texture_format3)
        {
            case 6:  // RGBA32
                {
                    for (byte m = 0; m <= mipmaps_number; m++)
                    {
                        blocks_wide = (canvas_dim[m][2] + 4) / 4;
                        blocks_tall = (canvas_dim[m][3] + 4) / 4;
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
                                index_list.Add(index);
                                cursor -= (blocks_wide * 4) + 8  // goes back to the left-most block
                            }
                            cursor += (blocks_wide * 4);  // cancels last operation because a whole width of blocks has been processed.
                        }
                        index_list_list.Add(index_list);
                        if (m + 1 <= mipmaps_number)
                        {
                            canvas[0] >>= 1; // divides by 2
                            canvas[1] >>= 1; // divides by 2   - also YES 1 DIVIDED BY TWO IS ZERO
                            canvas[2] = (ushort)(canvas[0] + (4 - (canvas[0] % 4) % 4));
                            canvas[3] = (ushort)(canvas[1] + (4 - (canvas[1] % 4) % 4));
                            canvas_dim.Add(canvas.ToArray());
                        }
                    }
                    break;
                }
            case 0xe:  // CMPR
                {
                    for (byte m = 0; m <= mipmaps_number; m++)
                    {
                        blocks_wide = (canvas_dim[m][2] + 8) / 8;
                        blocks_tall = (canvas_dim[m][3] + 8) / 8;
                        for (int b = 0; b < blocks_tall * blocks_wide; b++)
                        {
                            for (int w = 0; w < 8; w++)
                            {
                                index[count] = data[cursor];
                                cursor++;
                                count++;
                            }

                        }
                        index_list_list.Add(index_list);
                        if (m + 1 <= mipmaps_number)
                        {
                            canvas[0] >>= 1; // divides by 2
                            canvas[1] >>= 1; // divides by 2   - also YES 1 DIVIDED BY TWO IS ZERO
                            canvas[2] = (ushort)(canvas[0] + (8 - (canvas[0] % 8) % 8));
                            canvas[3] = (ushort)(canvas[1] + (8 - (canvas[1] % 8) % 8));
                            canvas_dim.Add(canvas.ToArray());
                        }
                    }
                    break;
                }
            default:
                {
                    for (byte m = 0; m <= mipmaps_number; m++)
                    {
                        blocks_wide = (canvas_dim[m][2] + block_width_array[texture_format3]) / block_width_array[texture_format3];
                        blocks_tall = (canvas_dim[m][3] + block_width_array[texture_format3]) / block_height_array[texture_format3];
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
                                    cursor += (block_height_array[texture_format3] - 1) * block_width_array[texture_format3]  // goes to next right block to fill the line horizontally
                                }
                                index_list.Add(index);
                                cursor -= (blocks_wide * block_height_array[texture_format3]) + block_width_array[texture_format3]  // goes back to the left-most block
                            }
                            cursor = cursor - ((block_height_array[texture_format3] - 1) * block_width_array[texture_format3]) + ((blocks_wide * block_height_array[texture_format3]))  // counters the last two operations beacause we've just filled a whole width of block.
                        }
                        index_list_list.Add(index_list);
                        if (m + 1 <= mipmaps_number)
                        {
                            canvas[0] >>= 1; // divides by 2
                            canvas[1] >>= 1; // divides by 2   - also YES 1 DIVIDED BY TWO IS ZERO
                            canvas[2] = (ushort)(canvas[0] + ((real_block_width_array[texture_format_int32[3]] - (canvas[0] % real_block_width_array[texture_format_int32[3]])) % real_block_width_array[texture_format_int32[3]]));
                            canvas[3] = (ushort)(canvas[1] + ((block_height_array[texture_format_int32[3]] - (canvas[1] % block_height_array[texture_format_int32[3]])) % block_height_array[texture_format_int32[3]]));
                            canvas_dim.Add(canvas.ToArray());
                        }
                    }
                    break;
                }
        }
    }
}