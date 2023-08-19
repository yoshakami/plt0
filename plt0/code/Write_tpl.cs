using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Write_tpl_class
{
    static public string Write_tpl(List<List<byte[]>> index_list, byte[] colour_palette, byte[] texture_format_int32, byte[] palette_format_int32, ushort bitmap_width, ushort bitmap_height, ushort colour_number, double format_ratio, string output_file, bool has_palette, bool safe_mode, bool no_warning, bool warn, bool stfu, bool name_string, sbyte block_width, sbyte block_height, byte mipmaps_number, byte minificaction_filter, byte magnification_filter, byte WrapS, byte WrapT)
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
        byte data_size = 32;
        byte header_size = 0x30;
        byte size2 = (byte)(4 + Math.Abs(16 - size) % 16);
        byte len = (byte)output_file.Split('\\').Length;
        string file_name = (output_file.Split('\\')[len - 1]);
        byte[] data2 = new byte[size2 + len + (16 - len) % 16];
        byte[] data = new byte[32];  // header data
        byte[] header = new byte[0x30];  // image header data

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

        data[16] = 0;
        data[17] = 0;
        data[18] = 0;
        if (has_palette)
        {
            data[13] = (byte)((0x20 + colour_palette.Length) >> 16);
            data[14] = (byte)((0x20 + colour_palette.Length) >> 8);
            data[15] = (byte)(0x20 + colour_palette.Length);  // offset to image0 header, (byte) acts as %256

            data[19] = 20;  // offset to palette0 header
            data[20] = (byte)(colour_number >> 8);
            data[21] = (byte)(colour_number);  // number of colours
            data[22] = 0;  // unpacked (0 I guess)
            data[23] = 0;  // padding
            data[24] = palette_format_int32[0];
            data[25] = palette_format_int32[1];
            data[26] = palette_format_int32[2]; // palette format
            data[27] = palette_format_int32[3]; // palette format
            data[28] = 0;
            data[29] = 0;
            data[30] = 0;
            data[31] = 32; // palette data address
            header[9] = (byte)((0x50 + colour_palette.Length) >> 16);
            header[10] = (byte)((0x50 + colour_palette.Length) >> 8);
            header[11] = (byte)(0x50 + colour_palette.Length);
        }
        else
        {
            data[13] = 0;
            data[14] = 0;
            data[15] = 20;  // offset to image0 header, (byte) acts as %256

            data[19] = 0;
            data_size = 20;
            header[9] = 0;
            header[10] = 0;
            header[11] = 64; // texture data offset
            header_size = 0x2C; // just to align the texture data to a multiple of 16.
        }
        // now's palette data, but it's already stored in colour_palette, so let's jump onto image header.
        header[0] = (byte)(bitmap_height >> 8);  // height
        header[1] = (byte)bitmap_height;  // yeah, I got tricked. height it written before the width
        header[2] = (byte)(bitmap_width >> 8);  // unsigned short width
        header[3] = (byte)bitmap_width; // second byte of width
        header[4] = texture_format_int32[0];
        header[5] = texture_format_int32[1];
        header[6] = texture_format_int32[2];
        header[7] = texture_format_int32[3];
        header[8] = 0; // won't exceed the max number of colours in the colour table which is stored on 2 bytes + header size 0x20

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
        for (int i = 36; i < header_size; i++)  // nintendo doesn't do this on their tpl. I guess it's for a good reason
        {
            header[i] = 0;
        }
        byte[] tex_data = new byte[size - 0x60 - colour_palette.Length];
        // tex_data = Create_blocks_class.Create_blocks(tex_data, settings, index_list, block_width, block_height, format_ratio, texture_format_int32);
        Create_blocks_class.Create_blocks(tex_data, settings, index_list, block_width, block_height, format_ratio, texture_format_int32); // I'm surprised. I don't need to assign the array here.
        FileMode mode = System.IO.FileMode.CreateNew;
        uint u = 0;
        bool done = false;
        while (!done)
        {
            try
            {
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
                    file.Write(data, 0, data_size);
                    file.Write(colour_palette, 0, colour_palette.Length);  // can I write nothing? YES!!! :D
                    file.Write(header, 0, header_size);
                    file.Write(tex_data, 0, tex_data.Length);
                    if (name_string)
                        file.Write(data2, 0, data2.Length);
                    file.Close();
                    done = true;
                    if (!stfu)
                        Console.WriteLine(output_file + ".tpl");
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
                    return "cannot write " + output_file + "\n";
                }
            }
        }
        return "written " + output_file + ".tpl\n";
    }
}