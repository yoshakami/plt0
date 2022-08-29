using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Write_tex0_class
{

    //[MethodImpl(MethodImplOptions.NoOptimization)]
    /// <summary>
    /// writes a TEX0 file from parameters of plt0_class and the list given in argument
    /// </summary>
    /// <param name="index_list">a List of mipmaps, first one being the highest quality one. <br/>
    /// each mipmap contains a list of every row of the image (starting by the bottom one). <br/>
    /// each row is actually a byte array of every pixel encoded in a specific format.</param>
    /// <returns>nothing. but writes the file into the output name given in CLI argument</returns>
    static public void Write_tex0(List<List<byte[]>> index_list, byte[] texture_format_int32, ushort bitmap_width, ushort bitmap_height, double format_ratio, string output_file, bool has_palette, bool safe_mode, bool no_warning, bool warn, bool stfu, bool name_string, sbyte block_width, sbyte block_height, byte mipmaps_number)  // index_list contains all mipmaps.
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
        if (name_string)
        {
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
        // tex_data = Create_blocks_class.Create_blocks(tex_data, settings, index_list, block_width, block_height, format_ratio, texture_format_int32);
        Create_blocks_class.Create_blocks(tex_data, settings, index_list, block_width, block_height, format_ratio, texture_format_int32); // WTF, tex_data values are changed in this function
        FileMode mode = System.IO.FileMode.CreateNew;
        uint u = 0;
        bool done = false;
        while (!done)
        {
            try
            {
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
                    if (name_string)
                        file.Write(data2, 0, data2.Length);
                    file.Close();
                    done = true;
                    if (!stfu)
                        Console.WriteLine(output_file + ".tex0");
                    // Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                u += 1;
                if (ex.Message.Substring(0, 34) == "The process cannot access the file")  // because it is being used by another process
                {
                    if (u > 1)
                    {
                        output_file = output_file.Substring(output_file.Length - 2) + "-" + u;
                    }
                    else
                    {
                        output_file += "-" + u;
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
    }
}