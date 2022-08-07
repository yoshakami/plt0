using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Write_bti_class
{
    static public void Write_bti(List<List<byte[]>> index_list, byte[] colour_palette, byte[] texture_format_int32, byte[] palette_format_int32, byte[] block_width_array, byte[] block_height_array, ushort bitmap_width, ushort bitmap_height, ushort colour_number, double format_ratio, string input_fil, string input_file2, string output_file, bool bmd_file, bool no_warning, bool has_palette, bool warn, bool stfu, sbyte block_width, sbyte block_height, byte mipmaps_number, byte minificaction_filter, byte magnification_filter, byte WrapS, byte WrapT, byte alpha)
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
        data[0] = texture_format_int32[3];  // image format, pretty straightforward it isn't an int lol
        data[1] = alpha;
        data[2] = (byte)(bitmap_width >> 8);  // unsigned short width
        data[3] = (byte)bitmap_width; // second byte of width
        data[4] = (byte)(bitmap_height >> 8);  // height
        data[5] = (byte)bitmap_height;
        data[6] = WrapS; // sideways wrap
        data[7] = WrapT; // vertical wrap
        if (has_palette)
        {
            data[8] = 1; // well, 1 means palette enabled, and the whole purpose of this tool is to make images with palette LMAO
            data[9] = palette_format_int32[3]; // pretty straightforward again
            data[10] = (byte)(colour_number >> 8);
            data[11] = (byte)(colour_number);  // number of colours
            data[12] = 0;
            data[13] = 0;
            data[14] = 0;
            data[15] = 32; // palette data address
        }
        else
        {

            data[8] = 0;  // well, I've changed my mind, I wanna make it better than wimgt 
            data[9] = 0; // pretty straightforward again
            data[10] = 0;
            data[11] = 0;  // number of colours
            data[12] = 0;
            data[13] = 0;
            data[14] = 0;
            data[15] = 0; // palette data address
        }
        if (mipmaps_number > 0)
        {
            data[16] = 1;
        }
        else
        {
            data[16] = 0;
        }
        data[17] = 0;  // EdgeLOD (bool)
        data[18] = 0;  // BiasClamp (bool)
        data[19] = 0;  // MaxAniso (byte)
        data[20] = minificaction_filter;
        data[21] = magnification_filter;
        data[22] = 0; // MinLOD
        data[23] = (byte)(mipmaps_number << 3);  // MaxLOD   << 3 is faster than * 8
        data[24] = (byte)(mipmaps_number + 1);  // number of images
        data[25] = 0x69; // my signature XDDDD, I couldn't figure out what this setting does, but it doesn't affect the gameplay at all, it looks like it is used as a version number.
        data[26] = 0; // how do I calculate LODBIAS
        data[27] = 0; // how do I calculate LODBIAS
        data[28] = 0;  // won't exceed the max number of colours in the colour table which is stored on 2 bytes + header size 0x20
        data[29] = (byte)((0x20 + colour_palette.Length) >> 16); // >> 16 is better than / 65536
        data[30] = (byte)((0x20 + colour_palette.Length) >> 8);  // >> 8 is better than / 256
        data[31] = (byte)(0x20 + colour_palette.Length);  // offset to image0 header, (byte) acts as %256
                                                          // now's palette data, but it's already stored in colour_palette, so let's jump onto image data.
        byte[] tex_data = new byte[size - 0x20 - colour_palette.Length];
        Create_blocks_class.Create_blocks(tex_data, settings, index_list, block_width, block_height, format_ratio, texture_format_int32);

        // finished to get everything of the bti
        // now we'll check if that bti needs to be injected in a bmd
        // I made extract-bti.py first, so I'm using it as a template
        // 0x14/20 bytes long
        /*
        TEX1 Header description
        0x00
        char[4] Magic_TEX1; //'TEX1'
        0x04
        int ChunkSize; //Total bytes of chunk
        0x08
        short TextureCount; //Number of textures
        0x0A
        short Padding; (Usually 0xFFFF)
        0x0C
        int TextureHeaderOffset; // (always 0x20) TextureCount bti image headers are stored here. Relative to TEX1Header start.
        0x10
        int StringTableOffset; //Stores one filename for each texture. Relative to TEX1Header start.
        */
        if (bmd_file)
        {
            if (has_palette)
            {
                Console.WriteLine("Sorry man, I have not met any bmd file with a palette amongst all games I've reviewed so far. I'll be glad to add this feature if you find some files I could look into\n for now you can't use CI4, CI8 and CI14x2 in a bmd\n");
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
                return;
            }
            using (System.IO.FileStream file = System.IO.File.Open(input_file2, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                byte[] j3d = new byte[file.Length];
                file.Read(j3d, 0, j3d.Length);
                file.Close();
                int y = 0x20; // next section offset
                while (!(j3d[y] == 'T' && j3d[y + 1] == 'E' && j3d[y + 2] == 'X' && j3d[y + 3] == '1'))  // section identifier
                {
                    if (y + 9 > j3d.Length)
                    {
                        if (!no_warning)
                            Console.WriteLine("Couldn't find the TEX1 Section. This bmd file is corrupted. the program will exit");
                        return;
                    }
                    y += (j3d[y + 4] << 24) + (j3d[y + 5] << 16) + (j3d[y + 6] << 8) + j3d[y + 7];  // size of current section
                }
                // now that we found TEX1 section address, we can find string pool table address, then get all texture names
                int x = y + (j3d[y + 16] << 24) + (j3d[y + 17] << 16) + (j3d[y + 18] << 8) + j3d[y + 19];  // string pool table start offset
                int string_pool_table_start_offset = x;
                ushort string_count = (ushort)((j3d[x] << 8) + j3d[x + 1]);
                int string_count_5 = string_count << 5;
                if (string_count == 0)  // ARE YOU KIDDING ME
                {
                    if (!no_warning)
                        Console.WriteLine("I could have sworn you were smart enough to take a bmd with bti textures in it, but this one contains none.");
                    return;
                }
                string name = "";
                x += 4 + (string_count << 2); // 4 is the string pool table header size
                                              // string count is multiplied by 4 because after the header there is a ushort hash and a ushort offset for each string
                bool success = false;
                ushort[] score = new ushort[string_count];
                int f = 0;
                int z = 0;
                int w = 0;
                ushort current_string = 0;
                ushort best_score = 0;  // length of the biggest substring in common
                ushort current = 0;
                List<string> name_list = new List<string>();
                while (current_string < string_count) // used to compare each texture name with the input file
                {
                    while (j3d[x] != 0)  // 0 is supposed to be the byte separating strings. never seen another value in mkdd
                    {
                        name += j3d[x];
                        x++;
                    }
                    name_list.Add(name);
                    if (name == input_fil)
                    {
                        success = true;
                        break;
                    }
                    else  // use an algorithm to determine the MOST SIMILAR texture by file name
                    {
                        for (z = 0; z < name.Length - 1; z++)
                        {
                            for (w = 0; w < input_fil.Length; w++)
                            {
                                if (name[z] == input_fil[w] && name[z + 1] == input_fil[w])
                                {
                                    break;
                                }
                            }  // used to find the start of the similar substring
                               // no, don't say 1 byte length files won't work here. They're supposed to fall in the first if
                            for (f = z, current = 0; f < input_fil.Length && f < name.Length; f++, current++)
                            {
                                if (input_fil[w] != name[f])
                                {
                                    break;
                                }
                            }
                            // get the length of the common substring 
                            if (current > best_score)
                            {
                                best_score = current;
                            }
                        }
                        score[current_string] = best_score;
                    }
                    current_string++;
                }
                if (!success)  // get the most fitting texture and ask for confirmation
                {
                    best_score = 0;
                    f = 0;
                    for (z = 0; z < score.Length; z++)
                    {
                        if (score[z] < best_score)
                        {
                            best_score = score[z];
                            f = z;
                        }
                    }
                    name = name_list[f];  // most probable fitting texture by the file name
                    if (warn)
                    {
                        Console.WriteLine("File names doesn't exactly match. Press Enter to replace " + name + " inside " + input_file2);
                        Console.ReadLine();
                    }
                }
                List<int> data_offset_array = new List<int>();
                List<int> data_size_array = new List<int>();
                int blocks_wide;
                int blocks_tall;
                int curr_mipmap_size;
                // f = bti header of the image to replace >> 5 (divided by 32, and relative to y)
                // y = TEX1 section start offset
                // z = current bti header offset (relative to y)
                // 
                for (z = 0; z < string_count_5; z += 32)
                {
                    if ((z >> 5) == f)
                    {
                        data[12] = (byte)(((string_count_5 - z)) << 24);
                        data[13] = (byte)(((string_count_5 - z)) << 16);
                        data[14] = (byte)(((string_count_5 - z)) << 8);
                        data[15] = (byte)(((string_count_5 - z)));
                        x = y + (z + 1 * 0x20);
                    }
                    int data_offset = (j3d[y + z + 0x3C] << 24) + (j3d[y + z + 0x3D] << 16) + (j3d[y + z + 0x3E] << 8) + (j3d[y + z + 0x3F]);
                    data_offset_array.Add(data_offset);
                    blocks_wide = (j3d[y + 0x22 + z] + j3d[y + 0x23 + z] + block_width_array[j3d[y + 0x20 + z]]) / block_width_array[j3d[y + 0x20 + z]];
                    blocks_tall = (j3d[y + 0x24 + z] + j3d[y + 0x25 + z] + block_width_array[j3d[y + 0x20 + z]]) / block_height_array[j3d[y + 0x20 + z]];
                    int data_size = (blocks_wide * blocks_tall);
                    if (j3d[y + 0x20 + z] == 6)
                    {
                        data_size <<= 6;
                    }
                    else
                    {
                        data_size <<= 5;
                    }
                    curr_mipmap_size = data_size;
                    for (int i = 1; i < j3d[y + 0x38 + z]; i++)
                    {
                        curr_mipmap_size >>= 2;
                        data_size += curr_mipmap_size;
                    }
                    data_size_array.Add(data_size);
                    /*blocks_wide = (self.width + (self.block_width-1)) // self.block_width
                      blocks_tall = (self.height + (self.block_height-1)) // self.block_height
                      image_data_size = blocks_wide*blocks_tall*self.block_data_size
                      remaining_mipmaps = self.mipmap_count-1
                      curr_mipmap_size = image_data_size
                      while remaining_mipmaps > 0:
                      # Each mipmap is a quarter the size of the last (half the width and half the height).
                          curr_mipmap_size = curr_mipmap_size//4
                          image_data_size += curr_mipmap_size
                          remaining_mipmaps -= 1*/

                    // fix palette_offset inside bmd which is the offset to the end of all bti headers
                    j3d[y + 0x20 + (0x20 * z) + 12] = (byte)(((string_count_5 - z)) << 24);
                    j3d[y + 0x20 + (0x20 * z) + 13] = (byte)(((string_count_5 - z)) << 16);
                    j3d[y + 0x20 + (0x20 * z) + 14] = (byte)(((string_count_5 - z)) << 8);
                    j3d[y + 0x20 + (0x20 * z) + 15] = (byte)(((string_count_5 - z)));
                }
                // now let's remove the image data that we're replacing if it's unused
                // no, I won't check for unused texture data apart from this one. these kind of things were generated by other softwares!
                int start = y + string_count_5 + 0x20;  // bti headers end offset
                int middle = data_offset_array[f] + ((string_count - f) << 5); // relative to start
                int start_2 = y + string_count_5 + data_offset_array[f + 1] - ((string_count - f + 1) << 5);
                for (z = 0; z < string_count_5; z += 32)
                {
                    if (data_offset_array[z >> 5] < middle)
                    {
                        if ((data_offset_array[z >> 5] + data_size_array[z >> 5]) > middle)
                        {
                            middle = (data_offset_array[z >> 5] + data_size_array[z >> 5]);
                            if (middle > start_2)
                            {
                                start_2 = middle;
                            }
                        }
                    }
                }
                FileMode mode = System.IO.FileMode.CreateNew;
                if (System.IO.File.Exists(output_file + ".bmd"))
                {
                    mode = System.IO.FileMode.Truncate;
                    if (warn)
                    {
                        Console.WriteLine("Press enter to overwrite " + output_file + ".bmd");
                        Console.ReadLine();
                    }
                }
                using (System.IO.FileStream ofile = System.IO.File.Open(output_file + ".bmd", mode, System.IO.FileAccess.Write))
                {
                    ofile.Write(j3d, 0, y);
                    // write texture headers
                    if (f != 0)
                    {
                        ofile.Write(j3d, y, f << 5);
                    }
                    ofile.Write(data, 0, 32);
                    if (f != string_count - 1)
                    {
                        ofile.Write(j3d, x, (string_count - f - 1) << 5);
                    }
                    if (middle == start_2)
                    {
                        ofile.Write(j3d, start, j3d.Length - start);
                    }
                    else
                    {
                        ofile.Write(j3d, start, middle);
                        ofile.Write(j3d, start_2, j3d.Length - start_2);
                    }
                    ofile.Close();
                    if (!stfu)
                        Console.WriteLine(output_file + ".bmd");
                }
            }
        }
        else  // single bti
        {
            FileMode mode = System.IO.FileMode.CreateNew;
            if (System.IO.File.Exists(output_file + ".bti"))
            {
                mode = System.IO.FileMode.Truncate;
                if (warn)
                {
                    Console.WriteLine("Press enter to overwrite " + output_file + ".bti");
                    Console.ReadLine();
                }
            }
            using (System.IO.FileStream file = System.IO.File.Open(output_file + ".bti", mode, System.IO.FileAccess.Write))
            {
                file.Write(data, 0, 32);
                file.Write(colour_palette, 0, colour_palette.Length);
                file.Write(tex_data, 0, tex_data.Length);
                file.Write(data2, 0, data2.Length);
                file.Close();
                if (!stfu)
                    Console.WriteLine(output_file + ".bti");
            }
        }
    }

}