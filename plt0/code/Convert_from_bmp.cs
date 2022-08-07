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
    public static bool Convert_from_bmp(System.Drawing.Bitmap imageIn, int current_mipmap, string output_file, ushort canvas_width, ushort canvas_height, bool png, bool tif, bool tiff, bool jpg, bool jpeg, bool gif, bool ico, bool no_warning, bool warn, bool stfu)
    {
        byte last_value_index = 0;
        string end = ".bmp";
        bool success = true;
        using (var ms = new MemoryStream())
        {
            while (success)
            {
                if (png && last_value_index < 1)
                {
                    imageIn.Save(ms, ImageFormat.Png);
                    end = ".png";
                    last_value_index = 1;
                }
                else if (tif && last_value_index < 2)
                {
                    imageIn.Save(ms, ImageFormat.Tiff);
                    end = ".tif";
                    last_value_index = 2;
                }
                else if (tiff && last_value_index < 3)
                {
                    imageIn.Save(ms, ImageFormat.Tiff);
                    end = ".tiff";
                    last_value_index = 3;
                }
                else if (jpg && last_value_index < 4)
                {
                    imageIn.Save(ms, ImageFormat.Jpeg);
                    end = ".jpg";
                    last_value_index = 4;
                }
                else if (jpeg && last_value_index < 5)
                {
                    imageIn.Save(ms, ImageFormat.Jpeg);
                    end = ".jpeg";
                    last_value_index = 5;
                }
                else if (gif && last_value_index < 6)
                {
                    imageIn.Save(ms, ImageFormat.Gif);
                    end = ".gif";
                    last_value_index = 6;
                }
                else if (ico && last_value_index < 7)
                {
                    if ((canvas_width >> current_mipmap) <= 256 && (canvas_height >> current_mipmap) <= 256)
                    {
                        if (!no_warning)
                            Console.WriteLine("max dimensions for a .ico file are 256x256");
                    }
                    else
                    {
                        imageIn.Save(ms, ImageFormat.Icon);
                        end = ".ico";
                        last_value_index = 7;
                    }
                }
                else
                {
                    return false;  // prevents the function from being called infinitely :P
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
        return true;
    }
}