using System;
using System.Collections.Generic;

class Decode_texture_class
{
    public List<ushort[]> canvas_dim = new List<ushort[]>();
    public byte Decode_texture(string input_file, string palette_file, string output_file, byte[] real_block_width_array, byte[] block_width_array, byte[] block_height_array, bool tex0, bool tpl, byte[] colour_palette, byte palette_format3, bool bmp_32, bool funky, bool reverse_x, bool reverse_y, bool warn, bool stfu, bool no_warning, bool safe_mode, bool png, bool gif, bool jpeg, bool jpg, bool ico, bool tiff, bool tif, byte mipmaps_number)
    {
        ushort colour_number = 0;
        int colour_number_x2 = 0;
        int colour_number_x4 = 0;
        // byte[] index declared in each if below
        ushort[] canvas = { 0, 0, 0, 0 };
        byte[] texture_format_int32 = { 0, 0, 0, 7 };
        byte[] palette_format_int32 = { 0, 0, 0, 0 };
        byte[] data = new byte[0];
        bool has_palette = false;
        byte alpha = 1;
        int data_start_offset = 0;
        int palette_start_offset;
        // fill index_list the same way write_bmp handles it
        if (colour_palette == null && tex0 && palette_file != "")
        {
            has_palette = true;
            using (System.IO.FileStream file_2 = System.IO.File.Open(palette_file, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                byte[] id = new byte[0x20];
                file_2.Read(id, 0, 0x20);
                if (id[0] == 80 && id[1] == 76 && id[2] == 84 && id[3] == 48)  // PLT0
                {
                    //byte[] data2 = new byte[26];
                    //byte[0x20] data;  // what is "on the stack" synthax in C#
                    //file_2.Read(data2, 0, 26);
                    palette_format_int32[3] = id[0x1B];
                    colour_number = (ushort)((id[0x1C] << 8) | id[0x1D]);
                    colour_number_x2 = colour_number << 1;
                    colour_number_x4 = colour_number << 2;
                    Array.Resize(ref colour_palette, colour_number_x2);
                    // file_2.Read(colour_palette, 0x40, colour_number_x2); // check  ""&&&$$$^^ù!:;;,
                    file_2.Position = 0x40;
                    file_2.Read(colour_palette, 0x40, colour_number_x2);
                    // user_palette = true;
                }
            }
        }
        else if (palette_file != "")
        {
            has_palette = true;
            colour_number = (ushort)(colour_palette.Length >> 1);
            colour_number_x4 = colour_palette.Length << 1;
            palette_format_int32[3] = palette_format3;
        }
        using (System.IO.FileStream file = System.IO.File.Open(input_file, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            Array.Resize(ref data, (int)file.Length);
            file.Read(data, 0, (int)file.Length);
        }
        if (tex0)
        {
            texture_format_int32[3] = data[0x23];
            if (mipmaps_number > data[0x27] - 1)
            {
                mipmaps_number = (byte)(data[0x27] - 1);
                if (!no_warning)
                    Console.WriteLine("number of images set as input are higher than in the file to decode.\nThis program will output the maximum number of mipmaps possible.");
            }
            canvas[0] = (ushort)((data[0x1c] << 8) + data[0x1d]);
            canvas[1] = (ushort)((data[0x1e] << 8) + data[0x1f]);
            data_start_offset = (data[0x10] << 24) | (data[0x11] << 16) | (data[0x12] << 8) | data[0x13];
        }
        else if (tpl)  // will decode ALL images
        {
            int image_number = (data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7];
            int image_table_offset = (data[8] << 24) | (data[9] << 16) | (data[10] << 8) | data[11];
            int cursor = image_table_offset;
            int[] image_headers_offset = new int[image_number];
            int[] palette_headers_offset = new int[image_number];
            for (int a = 0; a < image_number; a++)
            {
                image_headers_offset[a] = (data[cursor] << 24) | (data[cursor + 1] << 16) | (data[cursor + 2] << 8) | data[cursor + 3];
                cursor += 4;
                palette_headers_offset[a] = (data[cursor] << 24) | (data[cursor + 1] << 16) | (data[cursor + 2] << 8) | data[cursor + 3];
                cursor += 4;
            }
            for (int b = 0; b < image_number; b++)
            {
                has_palette = false;
                colour_number = 0;
                colour_number_x2 = 0;
                colour_number_x4 = 0;
                if (b == 1)
                {
                    output_file += b.ToString();
                }
                if (b > 1)
                {
                    output_file = output_file.Substring(0, output_file.Length - 1) + b.ToString();
                }
                canvas_dim.Clear();
                if (palette_headers_offset[b] != 0)
                {
                    has_palette = true;
                    palette_format_int32[3] = data[palette_headers_offset[b] + 7];
                    colour_number = (ushort)((data[palette_headers_offset[b]] << 8) + data[palette_headers_offset[b] + 1]);
                    colour_number_x2 = colour_number << 1;
                    colour_number_x4 = colour_number << 2;
                    palette_start_offset = (data[palette_headers_offset[b] + 8] << 24) | (data[palette_headers_offset[b] + 9] << 16) | (data[palette_headers_offset[b] + 10] << 8) | data[palette_headers_offset[b] + 11];
                    Array.Resize(ref colour_palette, colour_number_x2);
                    Array.Copy(data, palette_start_offset, colour_palette, 0, colour_number_x2);
                }
                 /*if (mipmaps_number > data[image_headers_offset[b] + 0x22] || b != 0)
                {
                    mipmaps_number = (byte)(data[image_headers_offset[b] + 0x22]);
                    if (!no_warning)
                        Console.WriteLine("number of images set as input are higher than in the file to decode.\nThis program will output the maximum number of mipmaps possible.");
                }*/
                texture_format_int32[3] = data[image_headers_offset[b] + 7];
                data_start_offset = (data[image_headers_offset[b] + 8] << 24) | (data[image_headers_offset[b] + 9] << 16) | (data[image_headers_offset[b] + 10] << 8) | data[image_headers_offset[b] + 11];

                canvas[0] = (ushort)((data[image_headers_offset[b] + 2] << 8) + data[image_headers_offset[b] + 3]);
                canvas[1] = (ushort)((data[image_headers_offset[b]] << 8) + data[image_headers_offset[b] + 1]);
                // fill canvas_dim first array
                canvas[2] = (ushort)(canvas[0] + ((real_block_width_array[texture_format_int32[3]] - (canvas[0] % real_block_width_array[texture_format_int32[3]])) % real_block_width_array[texture_format_int32[3]]));
                canvas[3] = (ushort)(canvas[1] + ((block_height_array[texture_format_int32[3]] - (canvas[1] % block_height_array[texture_format_int32[3]])) % block_height_array[texture_format_int32[3]]));
                canvas_dim.Add(canvas);
                // call fill_index_list
                Fill_index_list_class g = new Fill_index_list_class(this);
                object tpl_picture = g.Fill_index_list(data, data_start_offset, texture_format_int32[3], data[image_headers_offset[b] + 0x22], real_block_width_array, block_width_array, block_height_array, reverse_x, reverse_y);
                Write_bmp_class.Write_bmp((List<List<byte[]>>)tpl_picture, canvas_dim, colour_palette, texture_format_int32, palette_format_int32, colour_number, output_file, bmp_32, funky, has_palette, warn, stfu, no_warning, safe_mode, png, gif, jpeg, jpg, ico, tiff, tif, data[image_headers_offset[b] + 0x22], alpha, colour_number_x2, colour_number_x4);
            }
            return texture_format_int32[3];
        }
        else  // bti
        {
            texture_format_int32[3] = data[0];
            if (mipmaps_number > data[0x18] - 1)
            {
                mipmaps_number = (byte)(data[0x18] - 1);
                if (!no_warning)
                    Console.WriteLine("number of images set as input are higher than in the file to decode.\nThis program will output the maximum number of mipmaps possible.");
            }
            canvas[0] = (ushort)((data[2] << 8) + data[3]);
            canvas[1] = (ushort)((data[4] << 8) + data[5]);
            if ((data[8] == 1 || data[0x0a] != 0 || data[0x0b] != 0) && data[9] < 3 && data[0] > 7)  // the image has a palette - made so even bad images encoded with an idiot tool setting data[0x08] to zero would be decoded here
            {
                has_palette = true;
                palette_format_int32[3] = data[9];
                colour_number = (ushort)((data[0x0a] << 8) + data[0x0b]);
                colour_number_x2 = colour_number << 1;
                colour_number_x4 = colour_number << 2;
                palette_start_offset = (data[0x0C] << 24) | (data[0x0D] << 16) | (data[0x0E] << 8) | data[0x0F];
                Array.Resize(ref colour_palette, colour_number_x2);
                Array.Copy(data, palette_start_offset, colour_palette, 0, colour_number_x2);
            }
            data_start_offset = (data[0x1C] << 24) | (data[0x1D] << 16) | (data[0x1E] << 8) | data[0x1F];
        }
        // fill canvas_dim first array
        canvas[2] = (ushort)(canvas[0] + ((real_block_width_array[texture_format_int32[3]] - (canvas[0] % real_block_width_array[texture_format_int32[3]])) % real_block_width_array[texture_format_int32[3]]));
        canvas[3] = (ushort)(canvas[1] + ((block_height_array[texture_format_int32[3]] - (canvas[1] % block_height_array[texture_format_int32[3]])) % block_height_array[texture_format_int32[3]]));
        canvas_dim.Add(canvas);
        // call fill_index_list
        Fill_index_list_class f = new Fill_index_list_class(this);
        object picture = f.Fill_index_list(data, data_start_offset, texture_format_int32[3], mipmaps_number, real_block_width_array, block_width_array, block_height_array, reverse_x, reverse_y);
        Write_bmp_class.Write_bmp((List<List<byte[]>>)picture, canvas_dim, colour_palette, texture_format_int32, palette_format_int32, colour_number, output_file, bmp_32, funky, has_palette, warn, stfu, no_warning, safe_mode, png, gif, jpeg, jpg, ico, tiff, tif, mipmaps_number, 9, colour_number_x2, colour_number_x4);
        return texture_format_int32[3];
    }
}