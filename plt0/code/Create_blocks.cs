using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Create_blocks_class
{
    static public byte[] Create_blocks(byte[] tex_data, List<int[]> settings, List<List<byte[]>> index_list, sbyte block_width, sbyte block_height, double format_ratio, byte[] texture_format_int32)
    {
        int count = 0;
        int height;
        int width;
        //Console.WriteLine(block_width);
        block_width = (sbyte)(block_width / format_ratio);  // cool thing: changing this value will only be effective in the current function. not in the one it was called from
        //Console.WriteLine(block_width);
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