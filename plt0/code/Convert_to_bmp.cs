using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// same here. as the parse_args class's variables are being edited, I have to use dependancy injection.
class Convert_to_bmp_class
{
    Parse_args_class _plt0;
    public Convert_to_bmp_class(Parse_args_class Parse_args_class)
    {
        _plt0 = Parse_args_class;
    }
    public byte[] Convert_to_bmp(System.Drawing.Bitmap imageIn, bool linux = false)
    {
        if (_plt0.warn)
            Console.WriteLine(imageIn.PixelFormat.ToString());
        if (!_plt0.FORCE_ALPHA)
        {
            switch (imageIn.PixelFormat.ToString())
            {
                case "Format32bppRgb":
                case "Format24bppRgb":
                case "Format1bppIndexed":
                case "Format4bppIndexed":
                case "Format8bppIndexed":
                    {
                        _plt0.alpha = 0; // I don't care if the user set alpha to 1 or 2. the input image has no alpha. I won't let him trick my tool unless he uses the parameter "FORCE ALPHA"
                        break;
                    }
                    // case "Format32bppArgb"
            }
        }
        _plt0.bitmap_width = (ushort)imageIn.Width;
        _plt0.bitmap_height = (ushort)imageIn.Height;
        _plt0.pixel_count = _plt0.bitmap_width * _plt0.bitmap_height;
        _plt0.canvas_width = (ushort)(_plt0.bitmap_width + ((_plt0.block_width - (_plt0.bitmap_width % _plt0.block_width)) % _plt0.block_width));
        _plt0.canvas_height = (ushort)(_plt0.bitmap_height + ((_plt0.block_height - (_plt0.bitmap_height % _plt0.block_height)) % _plt0.block_height));
        if (linux)
        {
            Console.WriteLine(imageIn.Width + "x" + imageIn.Height + "\ncanvas: " + _plt0.canvas_width + "x" + _plt0.canvas_height);
            return ImageToByteArray(imageIn);
        }
        else
        {
            var bmp = new Bitmap(_plt0.canvas_width, _plt0.canvas_height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);  // makes it 32 bit in depth
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(imageIn, new Rectangle(0, 0, _plt0.canvas_width, _plt0.canvas_height));
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }
    }
    public byte[] ImageToByteArray(System.Drawing.Image imageIn)
    {
        using (var ms = new MemoryStream())
        {
            imageIn.Save(ms, imageIn.RawFormat);
            return ms.ToArray();
        }
    }
}