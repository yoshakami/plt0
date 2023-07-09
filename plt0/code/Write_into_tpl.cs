using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Write_into_tpl_class
{
    static public string Write_into_tpl(List<List<byte[]>> index_list, byte[] colour_palette, byte[] texture_format_int32, byte[] palette_format_int32, byte[] real_block_width_array, byte[] block_height_array, byte[] add_depth, byte[] sub_depth, ushort bitmap_width, ushort bitmap_height, ushort colour_number, double format_ratio, string input_file2, string output_file, bool has_palette, bool overwrite, bool safe_mode, bool no_warning, bool warn, bool stfu, sbyte block_width, sbyte block_height, byte mipmaps_number, byte minificaction_filter, byte magnification_filter, byte WrapS, byte WrapT)
    {
        int size = 0; // fixed size at 1 image
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
        // long tpl_size = 32;
        int data_size;
        // byte header_size = 0x30;
        // byte size2 = (byte)(4 + Math.Abs(16 - size) % 16);
        // byte len = (byte)input_file2.Split('\\').Length;
        // string file_name = (input_file2.Split('\\')[len - 1]);
        // byte[] data2 = new byte[size2 + len + (16 - len) % 16];
        byte[] tpl_header = new byte[12];  // header data
        byte[] image_table = new byte[8];
        byte[] empty_array = new byte[0];
        byte[] image_header = new byte[0x24];
        List<byte[]> image_header_list = new List<byte[]>();
        byte[] palette_header = new byte[0x0C];
        List<byte[]> palette_header_list = new List<byte[]>();
        List<byte[]> image_data_list = new List<byte[]>();
        List<byte[]> palette_data_list = new List<byte[]>();
        int image_header_offset;
        int palette_header_offset;
        int c;
        if (has_palette)
        {
            palette_header[0] = (byte)(colour_number >> 8);
            palette_header[1] = (byte)(colour_number);  // number of colours
            palette_header[2] = 0;  // unpacked (0 I guess)
            palette_header[3] = 0;  // padding
            palette_header[4] = palette_format_int32[0];
            palette_header[5] = palette_format_int32[1];
            palette_header[6] = palette_format_int32[2]; // palette format
            palette_header[7] = palette_format_int32[3]; // palette format
            palette_header[8] = 0; // palette data address
            palette_header[9] = 0; // palette data address
            palette_header[10] = 0; // palette data address
            palette_header[11] = 0; // palette data address
            palette_header_list.Add(palette_header.ToArray());
            palette_data_list.Add(colour_palette); // this adds a pointer to the array. not duplicating the array itself haha
        }
        else
        {
            palette_header_list.Add(empty_array);
            palette_data_list.Add(empty_array);
        }
        // now's palette data, but it's already stored in colour_palette, so let's jump onto image header.
        image_header[0] = (byte)(bitmap_height >> 8);  // height
        image_header[1] = (byte)bitmap_height;  // yeah, I got tricked. height it written before the width
        image_header[2] = (byte)(bitmap_width >> 8);  // unsigned short width
        image_header[3] = (byte)bitmap_width; // second byte of width
        image_header[4] = texture_format_int32[0];
        image_header[5] = texture_format_int32[1];
        image_header[6] = texture_format_int32[2];
        image_header[7] = texture_format_int32[3];
        image_header[8] = 0; // image data offset
        image_header[9] = 0; // image data offset
        image_header[10] = 0; // image data offset
        image_header[11] = 0; // image data offset
        image_header[12] = 0;
        image_header[13] = 0;
        image_header[14] = 0;
        image_header[15] = WrapS;
        image_header[16] = 0;
        image_header[17] = 0;
        image_header[18] = 0;
        image_header[19] = WrapT;
        image_header[20] = 0;
        image_header[21] = 0;
        image_header[22] = 0;
        image_header[23] = minificaction_filter;
        image_header[24] = 0;
        image_header[25] = 0;
        image_header[26] = 0;
        image_header[27] = magnification_filter;
        image_header[28] = 0; // LODBias
        image_header[29] = 0; // LODBias
        image_header[30] = 0; // LODBias
        image_header[31] = 0; // LODBias
        image_header[32] = 0; // EdgeLODEnable
        image_header[33] = 0; // MinLOD
        image_header[34] = mipmaps_number; // MaxLOD
        image_header[35] = 0; // unpacked
        image_header_list.Add(image_header.ToArray());
        byte[] tex_data = new byte[size];
        // tex_data = Create_blocks_class.Create_blocks(tex_data, settings, index_list, block_width, block_height, format_ratio, texture_format_int32);
        Create_blocks_class.Create_blocks(tex_data, settings, index_list, block_width, block_height, format_ratio, texture_format_int32); // I'm surprised. I don't need to assign the array here.
        image_data_list.Add(tex_data);
        int image_number;
        using (System.IO.FileStream tpl = System.IO.File.Open(input_file2, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            tpl.Read(tpl_header, 0, 12);
            // tpl_size = tpl.Length;
            image_number = (tpl_header[4] << 24) | (tpl_header[5] << 16) | (tpl_header[6] << 8) | tpl_header[7];
            int image_table_start_offset = (tpl_header[8] << 24) | (tpl_header[9] << 16) | (tpl_header[10] << 8) | tpl_header[11];
            int image_data_offset;
            int palette_data_offset;
            int colour_number_x2;
            // byte[] image_table = new byte[image_number << 3];  // 8 bytes per image
            Array.Resize(ref image_table, (image_number + 1) << 3);
            tpl.Position = image_table_start_offset;
            tpl.Read(image_table, 0, image_table.Length);
            c = 0;
            ushort width;
            ushort height;
            ushort real_height;  // encoded height (multiple of block_height)
            ushort real_width;  // fake width (multiple of block_width * bit depth per pixel)
            // ushort[] dim = new ushort[4];  // = {height, width, canvas_height, canvas_width
            // List<ushort[]> dim_list = new List<ushort[]>();
            for (int a = 0; a < image_number; a++)
            {
                image_header_offset = (image_table[c] << 24) | (image_table[c + 1] << 16) | (image_table[c + 2] << 8) | image_table[c + 3];
                c += 4;
                palette_header_offset = (image_table[c] << 24) | (image_table[c + 1] << 16) | (image_table[c + 2] << 8) | image_table[c + 3];
                c += 4;
                tpl.Position = image_header_offset;
                tpl.Read(image_header, 0, 0x24);
                image_header_list.Add(image_header.ToArray());
                // calculate image data size so you can add data to image_data_list
                data_size = 0;
                for (byte b = 0; b <= image_header[0x22]; b++) // for each mipmap
                {
                    height = (ushort)((image_header[0] << 8) | image_header[1]);
                    width = (ushort)((image_header[2] << 8) | image_header[3]);
                    real_height = (ushort)(height + ((block_height_array[image_header[7]] - (height % block_height_array[image_header[7]])) % block_height_array[image_header[7]]));
                    real_width = (ushort)(width + ((real_block_width_array[image_header[7]] - (width % real_block_width_array[image_header[7]])) % real_block_width_array[image_header[7]]));
                    data_size += real_height * real_width;
                    data_size <<= add_depth[image_header[7]];
                    data_size >>= sub_depth[image_header[7]];
                }
                image_data_offset = (image_header[8] << 24) | (image_header[9] << 16) | (image_header[10] << 8) | image_header[11];
                tpl.Position = image_data_offset;
                byte[] image_data = new byte[data_size];
                tpl.Read(image_data, 0, data_size);
                image_data_list.Add(image_data);
                if (palette_header_offset == 0)
                {
                    palette_header_list.Add(empty_array);
                    palette_data_list.Add(empty_array);
                }
                else
                {
                    tpl.Position = palette_header_offset;
                    tpl.Read(palette_header, 0, 0x0C);
                    palette_header_list.Add(palette_header.ToArray());
                    colour_number_x2 = (palette_header[0] << 9) | (palette_header[1] << 1);
                    palette_data_offset = (palette_header[8] << 24) | (palette_header[9] << 16) | (palette_header[10] << 8) | palette_header[11];
                    tpl.Position = palette_data_offset;
                    byte[] palette_data = new byte[colour_number_x2 + ((16 - (colour_number_x2 % 16)) % 16)];
                    tpl.Read(palette_data, 0, colour_number_x2);
                    palette_data_list.Add(palette_data);
                }
            }

        }
        /* now's the real deal. I have filled all Lists, and I need to reorganize them.
        // I'll do it in this order because it was most likely the less padding as possible to add all headers first.
        // idk why but data is always padded to a multiple of 16 by all tools, even sdk
        tpl header -> image table -> palette headers -> image headers -> palettes data -> images data

        first, let's calculate where the data starts first.
        then, we'll edit all headers in order to change the pointer to data offset
        when adding palettes, check if its data size is a multiple of 16, else add padding
        */
        int data_start_offset = 0xC + image_table.Length + (image_header_list.Count * 0x24);
        for (int d = 0; d <= image_number; d++)
        {
            data_start_offset += palette_header_list[d].Length;
        }
        byte[] header_padding = new byte[(16 - (data_start_offset % 16)) % 16];
        data_start_offset += header_padding.Length;
        int offset = 0xC + image_table.Length;
        c = 0;
        for (int e = 1; e <= image_number; e++)
        {
            if (palette_header_list[e].Length == 0)
            {
                image_table[c + 4] = 0;
                image_table[c + 5] = 0;
                image_table[c + 6] = 0;
                image_table[c + 7] = 0;
            }
            else
            {
                image_table[c + 4] = (byte)(offset >> 24);
                image_table[c + 5] = (byte)(offset >> 16);
                image_table[c + 6] = (byte)(offset >> 8);
                image_table[c + 7] = (byte)(offset);
                offset += 0xC;
            }
            c += 8;
        }
        if (palette_header_list[0].Length == 0)  // unrolled loop
        {
            image_table[c + 4] = 0;
            image_table[c + 5] = 0;
            image_table[c + 6] = 0;
            image_table[c + 7] = 0;
        }
        else  // unrolled loop
        {
            image_table[c + 4] = (byte)(offset >> 24);
            image_table[c + 5] = (byte)(offset >> 16);
            image_table[c + 6] = (byte)(offset >> 8);
            image_table[c + 7] = (byte)(offset);
            offset += 0xC;
        }
        c = 0;
        int f;
        for (f = 1; f <= image_number + 1; f++)  // last loop is rolled in here
        {
            image_table[c] = (byte)(offset >> 24);
            image_table[c + 1] = (byte)(offset >> 16);
            image_table[c + 2] = (byte)(offset >> 8);
            image_table[c + 3] = (byte)(offset);
            offset += 0x24;
            c += 8;
        }
        offset = data_start_offset;
        for (f = 1; f <= image_number; f++)
        {
            if (palette_header_list[f].Length != 0)
            {
                palette_header_list[f][8] = (byte)(offset >> 24);
                palette_header_list[f][9] = (byte)(offset >> 16);
                palette_header_list[f][10] = (byte)(offset >> 8);
                palette_header_list[f][11] = (byte)(offset);
                offset += palette_data_list[f].Length;
            }
        }
        if (palette_header_list[0].Length != 0)  // unrolled loop
        {
            palette_header_list[0][8] = (byte)(offset >> 24);
            palette_header_list[0][9] = (byte)(offset >> 16);
            palette_header_list[0][10] = (byte)(offset >> 8);
            palette_header_list[0][11] = (byte)(offset);
            offset += palette_data_list[0].Length;
        }
        for (f = 1; f <= image_number; f++)
        {
            image_header_list[f][8] = (byte)(offset >> 24);
            image_header_list[f][9] = (byte)(offset >> 16);
            image_header_list[f][10] = (byte)(offset >> 8);
            image_header_list[f][11] = (byte)(offset);
            offset += image_data_list[f].Length;
        }
        image_header_list[0][8] = (byte)(offset >> 24);
        image_header_list[0][9] = (byte)(offset >> 16);
        image_header_list[0][10] = (byte)(offset >> 8);
        image_header_list[0][11] = (byte)(offset);
        image_number++;
        tpl_header[4] = (byte)(image_number >> 24);
        tpl_header[5] = (byte)(image_number >> 16);
        tpl_header[6] = (byte)(image_number >> 8);
        tpl_header[7] = (byte)(image_number);
        if (overwrite)
        {
            output_file = input_file2;
        }
        else
        {
            output_file += ".tpl";
        }
        FileMode mode = System.IO.FileMode.CreateNew;
        uint u = 0;
        bool done = false;
        while (!done)
        {
            try
            {
                if (System.IO.File.Exists(output_file))
                {
                    mode = System.IO.FileMode.Truncate;
                    if (warn)
                    {
                        Console.WriteLine("Press enter to overwrite " + output_file);
                        Console.ReadLine();
                    }
                }
                using (System.IO.FileStream file = System.IO.File.Open(output_file, mode, System.IO.FileAccess.Write))
                {
                    file.Write(tpl_header, 0, tpl_header.Length);
                    file.Write(image_table, 0, image_table.Length);
                    for (f = 1; f < image_number; f++)
                    {
                        file.Write(palette_header_list[f], 0, palette_header_list[f].Length);
                    }
                    file.Write(palette_header_list[0], 0, palette_header_list[0].Length);
                    for (f = 1; f < image_number; f++)
                    {
                        file.Write(image_header_list[f], 0, 0x24);
                    }
                    file.Write(image_header_list[0], 0, 0x24);
                    file.Write(header_padding, 0, header_padding.Length);
                    for (f = 1; f < image_number; f++)
                    {
                        file.Write(palette_data_list[f], 0, palette_data_list[f].Length);
                    }
                    file.Write(palette_data_list[0], 0, palette_data_list[0].Length);
                    for (f = 1; f < image_number; f++)
                    {
                        file.Write(image_data_list[f], 0, image_data_list[f].Length);
                    }
                    file.Write(image_data_list[0], 0, image_data_list[0].Length);
                    file.Close();
                    done = true;
                    if (!stfu)
                        Console.WriteLine(output_file);
                }
            }
            catch (Exception ex)
            {
                u += 1;
                if (ex.Message.Substring(0, 34) == "The process cannot access the file")  // because it is being used by another process
                {
                    if (u > 1)
                    {
                        output_file = output_file.Substring(output_file.Length - 6) + "-" + u + ".tpl";
                    }
                    else
                    {
                        output_file = output_file.Substring(output_file.Length - 4) + "-" + u + ".tpl";
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
                    return "cannot write " + output_file;
                }
            }
        }
        return "";
    }
}