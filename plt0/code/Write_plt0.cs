using System;
using System.IO;

class Write_plt0_class
{
    static public string Write_plt0(byte[] colour_palette, byte[] palette_format_int32, ushort colour_number, string output_file, bool safe_mode, bool no_warning, bool warn, bool stfu, bool name_string)
    {
        int size = 0x40 + colour_palette.Length;
        byte size2 = (byte)(4 + Math.Abs(16 - size) % 16);
        byte len = (byte)output_file.Split('\\').Length;
        string file_name = (output_file.Split('\\')[len - 1]);
        byte[] data = new byte[64];  // header data
        byte[] data2 = new byte[size2 + len + ((16 - len) % 16)];
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
        data[0] = (byte)'P';
        data[1] = (byte)'L';
        data[2] = (byte)'T';
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
        data[15] = 0; // offset to outer brres
        data[16] = 0;
        data[17] = 0;
        data[18] = 0;
        data[19] = 64; // header size
        data[20] = (byte)((size + size2) >> 24);
        data[21] = (byte)((size + size2) >> 16);
        data[22] = (byte)((size + size2) >> 8);
        data[23] = (byte)(size + size2);  // name location
        data[24] = palette_format_int32[0];
        data[25] = palette_format_int32[1];
        data[26] = palette_format_int32[2];
        data[27] = palette_format_int32[3];
        data[28] = (byte)(colour_number >> 8);
        data[29] = (byte)colour_number;
        for (int i = 30; i < 64; i++)
        {
            data[i] = 0;
        }
        FileMode mode = System.IO.FileMode.CreateNew;
        uint u = 0;
        bool done = false;
        while (!done)
        {
            try
            {
                if (System.IO.File.Exists(output_file + ".plt0"))
                {
                    mode = System.IO.FileMode.Truncate;
                    if (warn)
                    {
                        Console.WriteLine("Press enter to overwrite " + output_file + ".plt0");
                        Console.ReadLine();
                    }
                }
                using (System.IO.FileStream file = System.IO.File.Open(output_file + ".plt0", mode, System.IO.FileAccess.Write))
                {
                    file.Write(data, 0, 64);
                    file.Write(colour_palette, 0, colour_palette.Length);
                    if (name_string)
                        file.Write(data2, 0, data2.Length);
                    file.Close();
                    done = true;
                    if (!stfu)
                        Console.WriteLine(output_file + ".plt0");
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
        return "written " + output_file + ".plt0\n";
    }
}