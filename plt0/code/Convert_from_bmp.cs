using System;
using System.Drawing.Imaging;
using System.IO;

class Convert_from_bmp_class
{
    /// <summary>
    /// Convert an image to specified format.
    /// </summary>
    /// <param name="imageIn">The image to convert.</param>
    /// <param name="current_mipmap">The actual numnber of mipmap (used to add .mm1 at the end).</param>
    /// <returns>The converted image written in a free-to-edit file.</returns>
    public static void Convert_from_bmp(System.Drawing.Bitmap imageIn, int current_mipmap, string output_file, ushort canvas_width, ushort canvas_height, bool png, bool tif, bool tiff, bool jpg, bool jpeg, bool gif, bool ico, bool no_warning, bool warn, bool stfu)
    {
        string end;
        using (MemoryStream ms = new MemoryStream())
        {
            for (byte i = 1; i < 8; i++)  // I'm curious how you would have implemented this without redundency
            {
                if (png && i == 1)
                {
                    imageIn.Save(ms, ImageFormat.Png);
                    end = ".png";
                }
                else if (tif && i == 2)
                {
                    imageIn.Save(ms, ImageFormat.Tiff);
                    end = ".tif";
                }
                else if (tiff && i == 3)
                {
                    imageIn.Save(ms, ImageFormat.Tiff);
                    end = ".tiff";
                }
                else if (jpg && i == 4)
                {
                    imageIn.Save(ms, ImageFormat.Jpeg);
                    end = ".jpg";
                }
                else if (jpeg && i == 5)
                {
                    imageIn.Save(ms, ImageFormat.Jpeg);
                    end = ".jpeg";
                }
                else if (gif && i == 6)
                {
                    imageIn.Save(ms, ImageFormat.Gif);
                    end = ".gif";
                }
                else if (ico && i == 7)
                {
                    if ((canvas_width >> current_mipmap) > 256 || (canvas_height >> current_mipmap) > 256)
                    {
                        if (!no_warning)
                            Console.WriteLine("max dimensions for a .ico file are 256x256");
                        continue;
                    }
                    else
                    {
                        imageIn.Save(ms, ImageFormat.Icon);
                        end = ".ico";
                    }
                }
                else
                {
                    continue;
                }
                if (current_mipmap != 0)
                {
                    end = ".mm" + current_mipmap + end;
                }
                FileMode mode = System.IO.FileMode.CreateNew;
                if (System.IO.File.Exists(output_file + end))
                {
                    mode = System.IO.FileMode.Truncate;
                    if (warn)
                    {
                        Console.WriteLine("Press enter to overwrite " + output_file + end);
                        Console.ReadLine();
                    }
                }
                using (System.IO.FileStream file = System.IO.File.Open(output_file + end, mode, System.IO.FileAccess.Write))
                {
                    file.Write(ms.ToArray(), 0, (int)ms.Length);
                    file.Close();
                    if (!stfu)
                        Console.WriteLine(output_file + end);
                }
            }
        }
    }
}