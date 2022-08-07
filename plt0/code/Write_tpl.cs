using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Write_tpl_class
{
    static public void Write_tpl(List<List<byte[]>> index_list, byte[] colour_palette, byte[] texture_format_int32, byte[] palette_format_int32, ushort bitmap_width, ushort bitmap_height, ushort colour_number, double format_ratio, string output_file, bool has_palette, bool warn, bool stfu, sbyte block_width, sbyte block_height,byte mipmaps_number, byte minificaction_filter, byte magnification_filter, byte WrapS, byte WrapT)
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
        Create_blocks_class.Create_blocks(tex_data, settings, index_list, block_width, block_height, format_ratio, texture_format_int32);
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

}