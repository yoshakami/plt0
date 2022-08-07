public byte[] Convert_to_bmp(System.Drawing.Bitmap imageIn)
    {
        if (warn)
            Console.WriteLine(imageIn.PixelFormat.ToString());
        if (!FORCE_ALPHA)
        {
            switch (imageIn.PixelFormat.ToString())
            {
                case "Format32bppRgb":
                case "Format24bppRgb":
                case "Format1bppIndexed":
                case "Format4bppIndexed":
                case "Format8bppIndexed":
                    {
                        alpha = 0; // I don't care if the user set alpha to 1 or 2. the input image has no alpha. I won't let him trick my tool unless he uses the parameter "FORCE ALPHA"
                        break;
                    }
                    // case "Format32bppArgb"
            }
        }
        bitmap_width = (ushort)imageIn.Width;
        bitmap_height = (ushort)imageIn.Height;
        pixel_count = bitmap_width * bitmap_height;
        canvas_width = (ushort)(bitmap_width + ((block_width - (bitmap_width % block_width)) % block_width));
        canvas_height = (ushort)(bitmap_height + ((block_height - (bitmap_height % block_height)) % block_height));
        var bmp = new Bitmap(canvas_width, canvas_height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);  // makes it 32 bit in depth
        using (var gr = Graphics.FromImage(bmp))
            gr.DrawImage(imageIn, new Rectangle(0, 0, canvas_width, canvas_height));
        using (var ms = new MemoryStream())
        {
            bmp.Save(ms, ImageFormat.Bmp);
            /*
            FileMode mode = System.IO.FileMode.CreateNew;
            if (System.IO.File.Exists(output_file + ".bmp"))
            {
                mode = System.IO.FileMode.Truncate;
                if (warn)
                {
                    Console.WriteLine("Press enter to overwrite " + output_file + ".bmp");
                    Console.ReadLine();
                }
            }
            using (System.IO.FileStream file = System.IO.File.Open(output_file + ".bmp", mode, System.IO.FileAccess.Write))
            {
                file.Write(ms.ToArray(), 0, (int)ms.Length);
                file.Close();
                Console.WriteLine(output_file + ".bmp");
            }
            alright. Format16bppRgb565 is compressed only
            */

            return ms.ToArray();
        }
    }