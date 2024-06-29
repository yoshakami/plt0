using System.Collections.Generic;
using System.Linq;

class Reverse_x_class
{
    static public List<byte[]> Reverse_x(ushort canvas_width, ushort canvas_height, List<byte[]> index_list, byte encoding)
    {
        List<byte[]> index_reversed = new List<byte[]>();
        byte[] index = new byte[index_list[0].Length];
        switch (encoding)
        {
            case 0: // I4
            case 8: // CI4
                for (int i = 0; i < index_list.Count; i++)
                {
                    for (int j = index_list[i].Length - 1, k = 0; j >= 0; j--, k++)
                    {
                        index[k] = (byte)(((index_list[i][j] & 15) << 4) + (index_list[i][j] >> 4));
                    }
                    index_list[i] = index.ToArray();
                }
                break;
            case 1:  // I8
            case 2:  // AI4
            case 9:  // CI8
                for (int i = 0; i < index_list.Count; i++)
                {
                    index_list[i] = index_list[i].Reverse().ToArray();
                }
                break;
            case 3:  // AI8
            case 4:  // RGB565
            case 5:  // RGB5A3
            case 10:  // CI14x2
                for (int i = 0; i < index_list.Count; i++)
                {
                    for (int j = index_list[i].Length - 1, k = 0; j >= 0; j -= 2, k += 2)
                    {
                        index[k] = index_list[i][j - 1];
                        index[k + 1] = index_list[i][j];
                    }
                    index_list[i] = index.ToArray();
                }
                break;
            case 6:  // RGBA8
                for (int i = 0; i < index_list.Count; i++)
                {
                    for (int j = index_list[i].Length - 1, k = 0; j >= 0; j -= 16, k += 16)
                    {
                        index[k + 6] = index_list[i][j - 15];  // A
                        index[k + 7] = index_list[i][j - 14];  // R
                        index[k + 4] = index_list[i][j - 13]; // A
                        index[k + 5] = index_list[i][j - 12]; // R
                        index[k + 2] = index_list[i][j - 11]; // A
                        index[k + 3] = index_list[i][j - 10]; // R
                        index[k] = index_list[i][j - 9]; // A
                        index[k + 1] = index_list[i][j - 8]; // R
                        index[k + 14] = index_list[i][j - 7];
                        index[k + 15] = index_list[i][j - 6];
                        index[k + 12] = index_list[i][j - 5];
                        index[k + 13] = index_list[i][j - 4];
                        index[k + 10] = index_list[i][j - 3];
                        index[k + 11] = index_list[i][j - 2];
                        index[k + 8] = index_list[i][j - 1];
                        index[k + 9] = index_list[i][j];
                    }
                    index_list[i] = index.ToArray();
                }
                break;
            case 14:  // CMPR
                int blocks_wide = canvas_width >> 2;
                int blocks_tall = canvas_height >> 3;
                //byte[][] index_reversed = new byte[blocks_wide][]; // I guess byte[][] sucks nowadays that List<byte[]> exists, can't get it to work
                int h = ((blocks_wide >> 1) - 1) << 2;
                for (int d = 0; d < blocks_tall; d++)
                {
                    for (int i = 0, e = 0; e < blocks_wide; i -= 4, e += 2)
                    {
                        index_reversed.Add(index_list[h + i + 1]);
                        index_reversed.Add(index_list[h + i]);
                        index_reversed.Add(index_list[h + i + 3]);
                        index_reversed.Add(index_list[h + i + 2]);
                    }

                    h += blocks_wide << 1;
                    // OMG I4M SO SMART IT WORKED
                    // it's so satisfying to update the most complicated encoding when it works
                }
                return index_reversed;
        }

        return index_list;
    }
}