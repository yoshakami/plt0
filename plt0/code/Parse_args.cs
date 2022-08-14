// this file also contains the check_exit function
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

class Parse_args_class
{
    static bool ask_exit = false;
    bool add = false;
    bool bmd = false;
    bool bmd_file = false;
    bool bmp = false;
    bool bti = false;
    bool exit = false;
    public bool fill_height = false;
    public bool fill_width = false;
    public bool FORCE_ALPHA = false;
    bool funky = false;
    bool gif = false;
    bool grey = true;
    public bool has_palette = false;
    bool ico = false;
    bool jpeg = false;
    bool jpg = false;
    bool png = false;
    bool bmp_32 = false;
    bool random_palette = false;
    bool safe_mode = false;
    bool success = false;
    bool tif = false;
    bool tiff = false;
    bool tpl = false;
    bool tex0 = false;
    bool reverse = false;
    bool overwrite = false;
    bool file2_conflict = false;
    public bool user_palette = false;
    public bool warn = false;
    public bool stfu = false;
    public bool no_warning = false;
    public byte cmpr_max = 16;  // number of colours that the program should take care in each 4x4 block - should always be set to 16 for better results.  // wimgt's cmpr encoding is better than mine. I gotta admit. 
    byte WrapS = 1; // 0 = Clamp   1 = Repeat   2 = Mirror
    byte WrapT = 1; // 0 = Clamp   1 = Repeat   2 = Mirror
    public byte algorithm = 0;  // 0 = CIE 601    1 = CIE 709     2 = custom RGBA     3 = Most Used Colours (No Gradient)
    public byte alpha = 9;  // 0 = no alpha - 1 = alpha - 2 = mix 
    byte color;
    public byte cmpr_alpha_threshold = 100;
    public byte diversity = 10;
    public byte diversity2 = 0;
    byte magnification_filter = 1;  // 0 = Nearest Neighbour   1 = Linear
    byte minificaction_filter = 1;  // 0 = Nearest Neighbour   1 = Linear
    byte mipmaps_number = 0;
    public byte round3 = 16;
    public byte round4 = 8;
    public byte round5 = 4;
    public byte round6 = 2;
    //byte r = 2;
    //byte g = 1;
    //byte b = 0;
    //byte a = 3;
    public byte[] colour_palette;
    public byte[] rgba_channel = { 2, 1, 0, 3 };
    public byte[] palette_format_int32 = { 0, 0, 0, 9 };  // 0 = AI8   1 = RGB565  2 = RGB5A3
    public byte[] texture_format_int32 = { 0, 0, 0, 7 };  // 8 = CI4   9 = CI8    10 = CI14x2
    byte[] real_block_width_array = { 8, 8, 8, 4, 4, 4, 4, 255, 8, 8, 4, 255, 255, 255, 8 }; // real one to calculate canvas size.
    byte[] block_width_array = { 4, 8, 8, 8, 8, 8, 16, 255, 4, 8, 8, 255, 255, 255, 4 }; // altered to match bit-per pixel size.
    byte[] block_height_array = { 8, 4, 4, 4, 4, 4, 4, 255, 8, 4, 4, 255, 255, 255, 8 }; // 255 = unused image format
    byte[] add_depth = { 0, 0, 0, 1, 1, 1, 2, 255, 0, 0, 1, 255, 255, 255, 0 };  // yet another method to calculate canvas size
    byte[] sub_depth = { 1, 0, 0, 0, 0, 0, 0, 255, 1, 0, 0, 255, 255, 255, 1 };  // just >> that number and you have the bit depth.
    double format_ratio = 1;  // bit par pixel * format_ratio = 8   -  though I still have in mind that a multiplication takes longer than moving bits around
    public double percentage = 0;
    public double percentage2 = 0;
    public double[] custom_rgba = { 1, 1, 1, 1 };
    public int colour_number_x2;
    int colour_number_x4;
    public int fill_palette_start_offset = 0;
    int pass = 0;
    public int pixel_count;
    int y = 0;
    public sbyte block_height = -1;
    public sbyte block_width = -1;
    string input_file = "";
    string input_fil = "";
    string input_ext = "";
    string input_file2 = "";
    string output_file = "";
    string swap = "";
    public ushort bitmap_height;
    public ushort bitmap_width;
    ushort colour_number = 0;
    ushort max_colours = 0;
    ushort z;
    public ushort canvas_width;
    public ushort canvas_height;
    List<byte> BGRA = new List<byte>();
    List<ushort[]> canvas_dim = new List<ushort[]>();
    public void Parse_args()
    {
        string[] args = Environment.GetCommandLineArgs();
        for (ushort i = 1; i < args.Length; i++)
        {
            if (pass > 0)
            {
                pass--;
                continue;
            }
            for (int j = 0; j < 10;)
            {
                if (args[i][j] == '-')
                {
                    args[i] = args[i].Substring(1, args[i].Length - 1);
                }
                else
                {
                    break;
                }
            }  // who had the stupid idea to add -- before each argument. I'm removing them all lol
            switch (args[i].ToUpper())  // this needs to be sorted sooner or later. it's becoming really big with all the options lol
            {
                case "STFU":
                case "SHUT":
                    {
                        stfu = true;
                        break;
                    }
                case "IA8":
                case "AI8":
                    {
                        palette_format_int32[3] = 0;  // will be set to texture format if texture format is unset
                        break;
                    }
                case "RGB565":
                case "RGB":
                    {
                        palette_format_int32[3] = 1;
                        break;
                    }
                case "RGB5A3":
                case "RGBA":
                    {
                        palette_format_int32[3] = 2;
                        break;
                    }
                case "I4":
                    {
                        texture_format_int32[3] = 0;
                        block_width = 8;
                        block_height = 8;
                        format_ratio = 2;
                        has_palette = false;
                        break;
                    }
                case "I8":
                    {
                        texture_format_int32[3] = 1;
                        block_width = 8;
                        block_height = 4;
                        format_ratio = 1;
                        has_palette = false;
                        break;
                    }
                case "IA4":
                case "AI4":
                    {
                        texture_format_int32[3] = 2;
                        block_width = 8;
                        block_height = 4;
                        format_ratio = 1;
                        has_palette = false;
                        break;
                    }
                case "FORCE_ALPHA":
                    {
                        FORCE_ALPHA = true;
                        break;
                    }
                case "FORCE":
                    {
                        FORCE_ALPHA = true;
                        if (args[i + 1].ToUpper() == "ALPHA")
                        {
                            pass = 1;
                        }
                        break;
                    }
                case "RGBA32":
                case "RGBA8":
                case "RGBA64":
                    {
                        texture_format_int32[3] = 6;
                        block_width = 4;
                        block_height = 4;
                        format_ratio = 0.25;
                        has_palette = false;
                        break;
                    }
                case "CMPR":
                    {
                        texture_format_int32[3] = 0xE;
                        block_width = 8;
                        block_height = 8;
                        format_ratio = 2;
                        has_palette = false;
                        break;
                    }
                case "C4":
                case "CI4":
                    {
                        max_colours = 16;
                        block_width = 8;
                        block_height = 8;
                        format_ratio = 2;
                        texture_format_int32[3] = 8;
                        has_palette = true;
                        break;
                    }
                case "C8":
                case "CI8":
                    {
                        max_colours = 256;
                        block_width = 8;
                        block_height = 4;
                        texture_format_int32[3] = 9;
                        has_palette = true;
                        break;
                    }
                case "CI14X2":
                case "C14X2":
                    {
                        max_colours = 16385;  // the user can still bypass this number by manually specifiying the number of colours
                                              // max_colours = 65535;
                        block_width = 4;
                        block_height = 4;
                        texture_format_int32[3] = 10;
                        format_ratio = 0.5;
                        has_palette = true;
                        break;
                    }
                case "32-BIT":
                case "32_BIT":
                case "32BPP":
                case "32BIT":
                case "BMP32":
                case "BMP_32":
                case "BMP-32":
                    {
                        bmp_32 = true;
                        break;
                    }
                case "G2":
                    {
                        algorithm = 1;
                        break;
                    }
                case "CC":
                    {
                        if (args.Length > i + 5)
                        {
                            algorithm = 2;
                            double.TryParse(args[i + 1], out custom_rgba[0]);
                            double.TryParse(args[i + 2], out custom_rgba[1]);
                            double.TryParse(args[i + 3], out custom_rgba[2]);
                            success = double.TryParse(args[i + 4], out custom_rgba[3]);
                            if (success)
                            {
                                pass = 4;
                            }
                        }
                        break;
                    }
                case "ROUND3":
                    {
                        if (args.Length < i + 2)
                        {
                            break;
                        }
                        success = byte.TryParse(args[i + 1], out round3);
                        if (success)
                        {
                            pass = 1;
                            if (round3 > 31 && !no_warning)
                            {
                                Console.WriteLine("um, so you would like to round up the 3rd bit if the value of the truncated bytes is greater than their max value, which means always round down (a.k.a BrawlCrate method), sure but be aware that the accuracy of the image is lowered");
                            }
                        }
                        break;
                    }
                case "ROUND4":
                    {
                        if (args.Length < i + 2)
                        {
                            break;
                        }
                        success = byte.TryParse(args[i + 1], out round4);
                        if (success)
                        {
                            pass = 1;
                            if (round4 > 15 && !no_warning)
                            {
                                Console.WriteLine("um, so you would like to round up the 4th bit if the value of the truncated bytes is greater than their max value, which means always round down (a.k.a BrawlCrate method), sure but be aware that the accuracy of the image is lowered");
                            }
                        }
                        break;
                    }
                case "ROUND5":
                    {
                        if (args.Length < i + 2)
                        {
                            break;
                        }
                        success = byte.TryParse(args[i + 1], out round5);
                        if (success)
                        {
                            pass = 1;
                            if (round5 > 7 && !no_warning)
                            {
                                Console.WriteLine("um, so you would like to round up the 5th bit if the value of the truncated bytes is greater than their max value, which means always round down (a.k.a BrawlCrate method), sure but be aware that the accuracy of the image is lowered");
                            }
                        }
                        break;
                    }
                case "ROUND6":
                    {
                        if (args.Length < i + 2)
                        {
                            break;
                        }
                        success = byte.TryParse(args[i + 1], out round6);
                        if (success)
                        {
                            pass = 1;
                            if (round6 > 3 && !no_warning)
                            {
                                Console.WriteLine("um, so you would like to round up the 6th bit if the value of the truncated bytes is greater than their max value, which means always round down (a.k.a BrawlCrate method), sure but be aware that the accuracy of the image is lowered");
                            }
                        }
                        break;
                    }
                case "REVERSE":
                    {
                        reverse = true;
                        break;
                    }
                case "FUNKY":
                    {
                        funky = true;
                        break;
                    }
                case "ALPHA":
                case "1BIT":
                case "1-BIT":
                    {
                        if (args.Length == i + 1)
                        {
                            alpha = 1;
                        }
                        else
                        {
                            success = byte.TryParse(args[i + 1], out alpha);
                            if (success)
                            {
                                pass = 1;
                            }
                            else
                            {
                                alpha = 1;
                            }
                        }
                        break;
                    }
                case "BMP":
                    {
                        bmp = true;
                        break;
                    }
                case "BMD":
                    {
                        bmd = true;
                        break;
                    }
                case "BTI":
                    {
                        bti = true;
                        break;
                    }
                case "PNG":
                    {
                        png = true;
                        break;
                    }
                case "TIF":
                    {
                        tif = true;
                        break;
                    }
                case "TIFF":
                    {
                        tiff = true;
                        break;
                    }
                case "ICO":
                    {
                        ico = true;
                        break;
                    }
                case "JPG":
                    {
                        jpg = true;
                        break;
                    }
                case "JPEG":
                    {
                        jpeg = true;
                        break;
                    }
                case "GIF":
                    {
                        gif = true;
                        break;
                    }
                case "C":
                    {
                        if (args.Length > i + 1)
                        {
                            success = ushort.TryParse(args[i + 1], out colour_number); // colour_number is now a number 

                            if (success)
                            {
                                colour_number_x2 = colour_number << 1;
                                colour_number_x4 = colour_number << 2;
                                pass = 1;
                            }
                        }
                        break;
                    }
                case "D":
                case "DIVERSITY":
                    {
                        // default_diversity = false;
                        if (args.Length > i + 1)
                        {
                            success = byte.TryParse(args[i + 1], out diversity); // diversity is now a number 

                            if (success)
                            {
                                pass = 1;
                            }
                        }
                        break;
                    }
                case "D2":
                case "DIVERSITY2":
                    {
                        // default_diversity2 = false;
                        if (args.Length > i + 1)
                        {
                            success = byte.TryParse(args[i + 1], out diversity2); // diversity2 is now a number 
                            if (success)
                            {
                                pass = 1;
                            }
                        }
                        break;
                    }
                case "M":
                case "N-MIPMAPS":
                case "N-MM":
                    {
                        if (args.Length > i + 1)
                        {
                            success = byte.TryParse(args[i + 1], out mipmaps_number); // mipmaps_number is now a number 
                            if (success)
                            {
                                pass = 1;
                            }
                        }
                        break;
                    }
                case "MIN":
                    {

                        if (args.Length < i + 2)
                        {
                            break;
                        }
                        pass = 1;
                        switch (args[i + 1].ToLower())
                        {
                            case "NN":
                            case "NEAREST":
                            case "0":
                                {
                                    minificaction_filter = 0;
                                    break;
                                }
                            case "LINEAR":
                            case "1":
                                {
                                    minificaction_filter = 1;
                                    break;
                                }
                            case "nearestmipmapnearest":
                            case "2":

                                {
                                    minificaction_filter = 2;
                                    break;
                                }
                            case "nearestmipmaplinear":
                            case "3":
                                {
                                    minificaction_filter = 3;
                                    break;
                                }
                            case "linearmipmapnearest":
                            case "4":
                                {
                                    minificaction_filter = 4;
                                    break;
                                }
                            case "linearmipmaplinear":
                            case "5":
                                {
                                    minificaction_filter = 5;
                                    break;
                                }
                            default:
                                {
                                    pass = 0;
                                    break;
                                }
                        }
                        break;
                    }
                case "MAG":
                    {
                        if (args.Length < i + 2)
                        {
                            break;
                        }
                        pass = 1;
                        switch (args[i + 1].ToLower())
                        {
                            case "NN":
                            case "NEAREST":
                            case "0":
                                {
                                    magnification_filter = 0;
                                    break;
                                }
                            case "LINEAR":
                            case "1":
                                {
                                    magnification_filter = 1;
                                    break;
                                }
                            case "nearestmipmapnearest":
                            case "2":

                                {
                                    magnification_filter = 2;
                                    break;
                                }
                            case "nearestmipmaplinear":
                            case "3":
                                {
                                    magnification_filter = 3;
                                    break;
                                }
                            case "linearmipmapnearest":
                            case "4":
                                {
                                    magnification_filter = 4;
                                    break;
                                }
                            case "linearmipmaplinear":
                            case "5":
                                {
                                    magnification_filter = 5;
                                    break;
                                }
                            default:
                                {
                                    pass = 0;
                                    break;
                                }
                        }
                        break;
                    }
                case "MAX":
                case "CMPR_MAX":
                case "CMPR-MAX":
                    {

                        if (args.Length < i + 2)
                        {
                            break;
                        }
                        success = byte.TryParse(args[i + 1], out cmpr_max);
                        if (success)
                        {
                            pass = 1;
                            if (cmpr_max > 16)
                            {
                                Console.WriteLine("nah man, a 4x4 block can only have up to 16 colours :') I already had a hard time dealing with 541x301 pictures so don't push mamá in the ortigas");
                                cmpr_max = 16;
                            }
                        }
                        break;
                    }
                case "MIX":
                case "MIXED":
                case "BOTH":
                case "8-BIT":
                case "8BIT":
                    {
                        alpha = 2;
                        break;
                    }
                case "NN":
                case "NEAREST":
                case "NEIGHBOUR": // bri'ish
                case "NEIGHBOR": // ricain
                    {
                        minificaction_filter = 0;
                        magnification_filter = 0;
                        break;
                    }
                case "LINEAR":
                    {
                        minificaction_filter = 1;
                        magnification_filter = 1;
                        break;
                    }
                case "NEARESTMIPMAPNEAREST":
                    {
                        minificaction_filter = 2;
                        magnification_filter = 2;
                        break;
                    }
                case "NEARESTMIPMAPLINEAR":
                    {
                        minificaction_filter = 3;
                        magnification_filter = 3;
                        break;
                    }
                case "LINEARMIPMAPNEAREST":
                    {
                        minificaction_filter = 4;
                        magnification_filter = 4;
                        break;
                    }
                case "LINEARMIPMAPLINEAR":
                    {
                        minificaction_filter = 5;
                        magnification_filter = 5;
                        break;
                    }
                case "NW":
                case "NOWARNING":
                case "NO WARNING":
                case "NO_WARNING":
                case "NO-WARNING":
                    {
                        no_warning = true;
                        break;
                    }
                case "NA":
                case "NOALPHA":
                case "NO ALPHA":
                case "NO_ALPHA":
                case "NO-ALPHA":
                    {
                        alpha = 0;
                        break;
                    }
                case "NG":
                case "NOGRADIENT":
                case "NO GRADIENT":
                case "NO_GRADIENT":
                case "NO-GRADIENT":
                case "SIMILAR":
                    {
                        algorithm = 3;
                        break;
                    }
                case "NO":
                    {
                        if (args.Length < i + 2)
                        {
                            break;
                        }
                        if (args[i + 1].ToUpper() == "ALPHA")
                        {
                            alpha = 0;
                            pass = 1;
                        }
                        if (args[i + 1].ToUpper() == "GRADIENT")
                        {
                            algorithm = 3;
                            pass = 1;
                        }
                        if (args[i + 1].ToUpper() == "WARNING")
                        {
                            no_warning = true;
                            pass = 1;
                        }
                        /*
                        if (args[i + 1].ToUpper() == "BOOBS")
                        {
                            sadness = 255;
                            pass = 1;
                        } */
                        break;
                    }
                case "PAL":
                    {
                        z = i;
                        pass = -1;
                        while (z < args.Length)
                        {
                            z++;
                            pass++;
                            if (args[z][0] == '#' && args[z].Length > 6)  // #RRGGBB
                            {
                                byte.TryParse(args[z].Substring(5, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                BGRA.Add(color);
                                byte.TryParse(args[z].Substring(3, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                BGRA.Add(color);
                                byte.TryParse(args[z].Substring(1, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                BGRA.Add(color);  // for god's sake, 0 is the hashtag, NOT A F-CKING INDEX START
                                if (args[z].Length > 8)  // #RRGGBBAA
                                {
                                    byte.TryParse(args[z].Substring(7, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                    BGRA.Add(color);
                                }
                                else
                                {
                                    BGRA.Add(0xff); // no alpha
                                }

                            }
                            else if (args[z][0] == '#' && args[z].Length > 3)  // #RGB
                            {
                                byte.TryParse(args[z].Substring(3, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                BGRA.Add((byte)(color << 4));
                                byte.TryParse(args[z].Substring(2, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                BGRA.Add((byte)(color << 4));
                                byte.TryParse(args[z].Substring(1, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                BGRA.Add((byte)(color << 4));
                                if (args[z].Length > 4)  // #RGBA
                                {
                                    byte.TryParse(args[z].Substring(4, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out color);
                                    BGRA.Add((byte)(color << 4));
                                }
                                else
                                {
                                    BGRA.Add(0xff); // no alpha
                                }
                            }
                            else  // no # means there are other arguments
                            {
                                break;
                            }
                        }
                        break;
                    }
                case "P":
                case "PERCENTAGE":
                    {
                        if (args.Length < i + 2)
                        {
                            break;
                        }
                        success = double.TryParse(args[i + 1], out percentage);  // percentage is now a double

                        if (success)
                        {
                            pass = 1;
                        }
                        break;
                    }
                case "P2":
                case "PERCENTAGE2":
                    {
                        if (args.Length < i + 2)
                        {
                            break;
                        }
                        success = double.TryParse(args[i + 1], out percentage2);  // percentage2 is now a double
                        if (success)
                        {
                            pass = 1;
                        }
                        break;
                    }
                case "RANDOM":
                case "RAND":
                    {
                        random_palette = true;
                        user_palette = true;  // palette doesn't need some complex indexing to be created
                        break;
                    }
                case "CMPR_ALPHA_THRESHOLD":
                case "CMPR_ALPHA_TRESHOLD":
                case "CMPR_ALPHA_TRESOLD":
                case "THRESHOLD":
                case "TRESHOLD": // common typo I do a lot
                case "TRESOLD":
                    {
                        if (args.Length > i + 1)
                        {
                            success = byte.TryParse(args[i + 1], out cmpr_alpha_threshold);
                            if (success)
                            {
                                pass = 1;
                            }
                        }
                        break;
                    }
                case "TPL":
                    {
                        tpl = true;
                        break;
                    }
                case "TEX0":
                    {
                        tex0 = true;
                        break;
                    }
                case "WARN":
                case "W":
                case "VERBOSE":
                    {
                        warn = true;
                        break;
                    }
                case "EXIT":
                case "ASK":
                    {
                        ask_exit = true;
                        break;
                    }
                case "NOERROR":
                case "NO-ERROR":
                case "NOERR":
                case "NO-ERR":
                case "SAFE":
                    {
                        safe_mode = true;
                        break;
                    }
                case "WRAP":
                    {
                        if (args.Length < i + 3)
                        {
                            break;
                        }
                        pass = 2;
                        switch (args[i + 1].ToUpper().Substring(0, 1))
                        {
                            case "C": // clamp
                                {
                                    WrapS = 0;
                                    break;
                                }
                            case "R": // repeat
                                {
                                    WrapS = 1;
                                    break;
                                }
                            case "M": // mirror
                                {
                                    WrapS = 2;
                                    break;
                                }
                            default:
                                {
                                    pass = 0;
                                    break;
                                }
                        }
                        switch (args[i + 2].ToUpper().Substring(0, 1))
                        {
                            case "C": // clamp
                                {
                                    WrapT = 0;
                                    break;
                                }
                            case "R": // repeat
                                {
                                    WrapT = 1;
                                    break;
                                }
                            case "M": // mirror
                                {
                                    WrapT = 2;
                                    break;
                                }
                            default:
                                {
                                    pass = 0;
                                    break;
                                }
                        }
                        break;
                    }

                default:
                    {
                        if (System.IO.File.Exists(args[i]) && input_file == "")
                        {
                            input_file = args[i];
                            input_fil = args[i].Substring(0, args[i].Length - args[i].Split('.')[args[i].Split('.').Length - 1].Length - 1);  // removes the text after the extension dot.
                            input_ext = args[i].Substring(args[i].Length - args[i].Split('.')[args[i].Split('.').Length - 1].Length - 1, args[i].Length - input_fil.Length);  // removes the text before the extension dot.
                        }
                        else if (System.IO.File.Exists(swap) && input_file2 == "")  // swap out args[i] and output file.
                        {
                            input_file2 = swap;
                            if (args[i].Contains(".") && args[i].Length > 1)
                            {
                                output_file = args[i].Substring(0, args[i].Length - args[i].Split('.')[args[i].Split('.').Length - 1].Length - 1);  // same as the os.path.splitext(args[i])[0] function in python
                            }
                            else
                            {
                                output_file = args[i];
                            }

                        }
                        else
                        {
                            if (args[i].Length == 3)
                            {
                                success = true;
                                for (byte c = 0; c < 3; c++)
                                {
                                    if (args[i + c].ToUpper() != "R" && args[i + c].ToUpper() != "G" && args[i + c].ToUpper() != "B")
                                    {
                                        success = false;
                                        break;
                                    }
                                }
                                if (success)
                                {
                                    for (byte c = 0; c < 3; c++)
                                    {
                                        switch (args[i + c].ToUpper())
                                        {
                                            case "R":
                                                {
                                                    rgba_channel[c] = 2;
                                                    break;
                                                }
                                            case "G":
                                                {
                                                    rgba_channel[c] = 1;
                                                    break;
                                                }
                                            case "B":
                                                {
                                                    rgba_channel[c] = 0;
                                                    break;
                                                }
                                        }
                                    }
                                }
                            }
                            else if (args[i].Length == 4)
                            {
                                success = true;
                                for (byte c = 0; c < 4; c++)
                                {
                                    if (args[i + c].ToUpper() != "R" && args[i + c].ToUpper() != "G" && args[i + c].ToUpper() != "B" && args[i + c].ToUpper() != "A")
                                    {
                                        success = false;
                                        break;
                                    }
                                }
                                if (success)
                                {
                                    for (byte c = 0; c < 4; c++)
                                    {
                                        switch (args[i + c].ToUpper())
                                        {
                                            case "R":
                                                {
                                                    rgba_channel[c] = 2;
                                                    break;
                                                }
                                            case "G":
                                                {
                                                    rgba_channel[c] = 1;
                                                    break;
                                                }
                                            case "B":
                                                {
                                                    rgba_channel[c] = 0;
                                                    break;
                                                }
                                            case "A":
                                                {
                                                    rgba_channel[c] = 3;
                                                    break;
                                                }
                                        }
                                    }
                                }
                            }
                            if (args[i].Contains(".") && args[i].Length > 1)
                            {
                                output_file = args[i].Substring(0, args[i].Length - args[i].Split('.')[args[i].Split('.').Length - 1].Length - 1);  // removes the text after the extension dot.
                                swap = args[i];
                            }
                            else
                            {
                                output_file = args[i];
                                swap = args[i];
                            }
                        }
                        break;
                    }
            }

        }
        if (input_file == "")
        {
            if (!no_warning)
                Console.WriteLine("no input file specified\nusage: PLT0 <file> <tex0|tpl|bti|bmd|bmp|png|gif|jpeg|ico|tiff> <Encoding Format> [Palette Format] [pal custom palette|bmd file|plt0 file|bmp file|png file|jpeg file|tiff file|ico file|gif file|rle file] [dest file name without extension] [alpha|no alpha|mix] [c colour number] [d diversity] [d2 diversity2] [m mipmaps] [p percentage] [p2 percentage2] [g2|no_gradient|cc 0.7 0.369 0.4 1.0] [warn|w] [exit|ask] [safe|noerror] [random] [min nearest neighbour] [mag linear] [Wrap Clamp Clamp] [funky] [round3] [round4] [round5] [round6] [force alpha]\nthis is the usage format for parameters : [optional] <mandatory> \n\nthe order of the parameters doesn't matter, but the image to encode must be put before the second input file (bmd/image used as palette)\nAvailable Encoding Formats: C4, CI4, C8, CI8, CI14x2, C14x2, default = CI8\nAvailable Palette Formats : IA8 (Black and White), RGB565 (RGB), RGB5A3 (RGBA), default = (auto)\n\nif the palette chosen is RGB5A3, you can force the image to have no alpha for all colours (so the RGB values will be stored on 5 bits each), or force all colours to use an alpha channel, by default it's set on 'mix'\n\ndashes are not needed before each parameter, it will still work if you add some as you please lol, this cli parsing function should be unbreakable\nthe number of colours is stored on 2 bytes, but the index bit length of the colour encoding format limits the max number of colours to these:\nCI4 : from 0 to 16\nCI8 : from 0 to 256\nCI14x2 : from 0 to 16384\n\nbmd = inject directly the bti file in a bmd (if the bti filename isn't in the bmd or you haven't entered a bmd second input file, this will notice you of the failure).\n\npal #RRGGBB #RRGGBB #RRGGBB ... is used to directly input your colour palette to a tex0 so it'll encode it with this palette. (example creating a 2 colour palette:plt0 pal #000000 #FFFFFF)\nif you input two existing images, the second one will be taken as the input palette.\n\nd | diversity = a minimum amount of difference for at least one colour channel in the colour palette\n(prevents a palette from having colours close to each other) default : 16 colours -> diversity = 60 | 256 colours -> diversity = 20\nvalue between 0 and 255, if the program heaven't filled the colour table with this parameter on, it'll fill it again with diversity2, then it'll let you know and fill it with the most used colours\n\n d2 | diversity2 = the diversity parameter for the second loop that fills the colour table if it's not full after the first loop.\n\n-m | --n-mm | m | --n-mipmaps = mipmaps number (number of duplicates versions of lower resolution of the input image stored in the output texture) default = 0\n\n p | percentage a double (64 bit float) between zero and 100. Alongside the diversity setting, if a colour counts for less than p % of the whole number of pixels, then it won't be added to the palette. default = 0 (0%)\n-p2 | p2 | percentage2 = the minimum percentage for the second loop that fills the colour table. default = 0 (0%)\nadd w or warn to make the program ask to you before overwriting the output file or tell you which parameters the program used.\n\nexit or ask is used to always make the tool make you press enter before exit.\nsafe or noerror is used to make the program never throw execution error code (useful for using subsystem.check_output in python without crash)\nrandom = random colour palette\n\ntype g2 to use grayscale recommendation CIE 709, the default one used is the recommendation CIE 601.\ntype cc then your float RGBA factors to multiply every pixel of the input image by this factor\n the order of parameters doesn't matter, though you need to add a number after commands like 'm', 'c' or 'd',\nmin or mag filters are used to choose the algorithm for downscaling/upscaling textures in a tpl file : (nearest neighbour, linear, NearestMipmapNearest, NearestMipmapLinear, LinearMipmapNearest, LinearMipmapLinear) you can just type nn or nearest instead of nearest neighbour or the filter number.\nwrap X Y is the syntax to define the wrap mode for both horizontal and vertical within the same option. X and Y can have these values (clamp, repeat, mirror) or just type the first letter. \nplease note that if a parameter corresponds to nothing described above and doesn't exists, it'll be taken as the output file name.\n\nRound3 : threshold for 3-bit values (alpha in RGB5A3). every value of the trimmed bits above this threshold will round up the last bit. Default: 16.\nRound4 : Default: 8 (it's the middle between 0 and 16)\nRound5 : Default: 4 (brawlcrate uses to put 0 everywhere)\nRound6 : Default: 2 (wimgt has every default value here +1 eg here it would be 3)\n\nFORCE ALPHA: by default only 32 bits RGBA Images will have alpha enabled automatically or not (if the image has transparent pixels) on RGBA32 and RGB5A3 and CMPR textures.\n1-bit, 4-bit, 8-bit and 24-bit depth images won't have alpha enabled unless you FORCE ALPHA (which will likely result in a fully transparent image)\nthreshold / cmpr_alpha_threshold : every pixel that has an alpha below this value will be fully transparent (only for cmpr format, value between 0 and 255). default = 100\nfunky : gives funky results if your output image is a bmp/png/gif etc. because it paste the raw encoded format in the bmp who's supposed to have a GBRA byte array 🤣🤣🤣\n\n Examples: (using wimgt synthax can work as seen below)\nplt0 rosalina.png -d 0 -x ci8 rgb5a3 --n-mipmaps 5 -w -c 256 (output name will be '-x.plt0' and '-x.tex0', as this is not an existing option)\nplt0 tpl ci4 rosalina.jpeg AI8 c 4 d 16 g2 m 1 warn texture.tex0");
            return;
        }
        if (System.IO.File.Exists(swap) && input_file2 == "")
        {
            input_file2 = swap;
            file2_conflict = true;
        }
        if (output_file == "")
        {
            overwrite = true;
            output_file = input_file.Substring(0, input_file.Length - input_file.Split('.')[input_file.Split('.').Length - 1].Length - 1);
        }
        if (colour_number > max_colours && max_colours == 16)
        {
            if (!no_warning)
                Console.WriteLine("CI4 can only supports up to 16 colours as each pixel index is stored on 4 bits");
            return;
        }
        if (colour_number > max_colours && max_colours == 256)
        {
            if (!no_warning)
                Console.WriteLine("CI8 can only supports up to 256 colours as each pixel index is stored on 8 bits");
            return;
        }
        if (colour_number > 65535 && max_colours == 16385)
        {
            if (!no_warning)
                Console.WriteLine("Colour number is stored on 2 bytes for CI14x2");
            return;
        }
        if (minificaction_filter == 0 && mipmaps_number > 0)
        {
            minificaction_filter = 2; // Nearest becomes "NearestMipmapNearest", if you don't want that then why the heck are you adding mipmaps to your file if you're not going to use them
        }
        if (minificaction_filter == 1 && mipmaps_number > 0)
        {
            minificaction_filter = 5; // Linear becomes "LinearMipmapLinear"
        }
        if (magnification_filter == 0 && mipmaps_number > 0)
        {
            magnification_filter = 2;
        }
        if (magnification_filter == 1 && mipmaps_number > 0)
        {
            magnification_filter = 5;
        }
        if (texture_format_int32[3] == 7 && palette_format_int32[3] != 9)  // if user has chosen to convert to AI8, RGB565, or RGB5A3
        {
            texture_format_int32[3] = (byte)(palette_format_int32[3] + 3);
            block_width = 4;
            block_height = 4;
            format_ratio = 0.5;
            has_palette = false;
        }
        /* if (max_colours == 0 && texture_format_int32[3] == 7 && palette_format_int32[3] != 9)  // if user haven't chosen a texture format
        {
            max_colours = 256;  // let's set CI8 as default
            block_width = 8;
            block_height = 4;
            texture_format_int32[3] = 9;
            has_palette = true;
        }*/
        if (colour_number == 0)
        {
            colour_number = max_colours;
            colour_number_x2 = colour_number << 1;
            colour_number_x4 = colour_number << 2;
        }
        //try
        //{
        byte[] id = new byte[9];
        using (System.IO.FileStream file = System.IO.File.Open(input_file, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            file.Read(id, 0, 8);
            file.Position = 0x23;
            file.Read(id, 8, 1);
        }
        Decode_texture_class dec = new Decode_texture_class();
        // msm tools handles these. the user can also make scripts to launch the program for every file he wants.
        //case "bres":
        //case .arc file from wii games
        //case bmd
        //case bti ??? how do I write that lol

        if (id[0] == 84 && id[1] == 69 && id[2] == 88 && id[3] == 48) // TEX0
        {
            if (id[8] == 8 || id[8] == 9 || id[8] == 10)  // CI4, CI8, CI14x2
            {
                if (input_file2 == "")
                {
                    // get the palette file from the same directory or exit
                    if (System.IO.File.Exists(input_fil + ".plt0"))
                    {
                        dec.Decode_texture(input_file, input_fil + ".plt0", output_file, real_block_width_array, block_width_array, block_height_array, true, false, colour_palette, 0, bmp_32, funky, reverse, warn, stfu, no_warning, safe_mode, png, gif, jpeg, jpg, ico, tiff, tif, mipmaps_number);
                    }
                    else
                    {
                        if (!no_warning)
                            Console.WriteLine("your input texture missing plt0 file. it should be named " + input_fil + ".plt0");
                    }
                }
                else
                {
                    // check if second file is the palette or exit
                    byte[] data = new byte[0x20];
                    using (System.IO.FileStream file_2 = System.IO.File.Open(input_file2, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        file_2.Read(id, 0, 4);
                        file_2.Read(data, 0, 0x20);
                        colour_number = (ushort)((data[0x1C] << 8) + data[0x1D]);
                        colour_number_x2 = colour_number << 1;
                        Array.Resize(ref colour_palette, colour_number_x2);
                        file_2.Position = 0x40;
                        file_2.Read(colour_palette, 0, colour_number_x2);
                    }
                    if (id[0] == 80 && id[1] == 76 && id[2] == 84 && id[3] == 48)  // PLT0
                    {
                        //byte[0x20] data;  // what is "on the stack" synthax in C#
                        // colour_number_x4 = colour_number << 2;
                        // palette_format_int32[3] = data[0x1B];
                        //file_2.Read(colour_palette, 0x40, colour_number_x2);  // check if this is right. second parameter should always be ZERO
                        // user_palette = true;
                        dec.Decode_texture(input_file, input_file2, output_file, real_block_width_array, block_width_array, block_height_array, true, false, colour_palette, data[0x1B], bmp_32, funky, reverse, warn, stfu, no_warning, safe_mode, png, gif, jpeg, jpg, ico, tiff, tif, mipmaps_number);
                    }
                    // get the palette file from the same directory or exit
                    else if (System.IO.File.Exists(input_fil + ".plt0"))
                    {
                        dec.Decode_texture(input_file, input_fil + ".plt0", output_file, real_block_width_array, block_width_array, block_height_array, true, false, colour_palette, 0, bmp_32, funky, reverse, warn, stfu, no_warning, safe_mode, png, gif, jpeg, jpg, ico, tiff, tif, mipmaps_number);
                    }
                    else
                    {
                        if (!no_warning)
                            Console.WriteLine("your input texture missing plt0 file. it should be named " + input_fil + ".plt0");
                    }

                }
            }
            else
            {
                // decode the image
                dec.Decode_texture(input_file, "", output_file, real_block_width_array, block_width_array, block_height_array, true, false, colour_palette, 0, bmp_32, funky, reverse, warn, stfu, no_warning, safe_mode, png, gif, jpeg, jpg, ico, tiff, tif, mipmaps_number);
            }
            return;
        }
        else if (id[0] == 0 && id[1] == 32 && id[2] == 0xaf && id[3] == 48)  // tpl file header
        {
            dec.Decode_texture(input_file, "", output_file, real_block_width_array, block_width_array, block_height_array, false, true, colour_palette, 0, bmp_32, funky, reverse, warn, stfu, no_warning, safe_mode, png, gif, jpeg, jpg, ico, tiff, tif, mipmaps_number);
            return;
        }
        else if (id[0] < 15 && id[6] < 3 && id[7] < 3)  // rough bti check
        {
            dec.Decode_texture(input_file, "", output_file, real_block_width_array, block_width_array, block_height_array, false, false, colour_palette, 0, bmp_32, funky, reverse, warn, stfu, no_warning, safe_mode, png, gif, jpeg, jpg, ico, tiff, tif, mipmaps_number);
            return;
        }


        if (texture_format_int32[3] == 7 && palette_format_int32[3] == 9)
        {
            if (!no_warning)
                Console.WriteLine("add a texture encoding format as argument.\n\nList of available formats: \nI4      (black and white 4 bit shade of gray)\nI8      (1 black and white byte per pixel)\nAI4     (4-bit alpha then 4-bit I4)\nAI8     (1 byte alpha and 1 byte I8)\nRGB565  (best colour encoding, 5-bit red, 6-bit green, and 5-bit blue)\nRGB5A3  (rgb555 if pixel doesn't have alpha, and 3-bit alpha + rgb444 if pixel have alpha)\nRGBA8   (lossless encoding, biggest one though, 4 bytes per pixel)\nCI4   * (uses a colour palette of max 16 colours)\nCI8   * (uses a colour palette of max 256 colours)\nCI14x2 *(uses a colour palette of max 65536 colours) - untested in-game\nCMPR  (4-bit depth, max 2 colours per 4x4 image chunk + 2 software interpolated ones) - wimgt encoding for this format is pretty decent, you should check it out\n\n* you can force a palette format to be set for these textures format\nPalette formats: AI8, RGB565, RGB5A3");
            return;
            // need to change this for decoding
        }
        Convert_to_bmp_class _bmp = new Convert_to_bmp_class(this);
        byte[] bmp_image = _bmp.Convert_to_bmp((Bitmap)Bitmap.FromFile(input_file));

        /* if (colour_number > max_colours && max_colours == 16385)
    {
        Console.WriteLine("CI14x2 can only supports up to 16385 colours as each pixel index is stored on 14 bits");
        stay = false;
    }*/

        if (bmp_image[0x15] != 0 || bmp_image[0x14] != 0 || bmp_image[0x19] != 0 || bmp_image[0x18] != 0)
        {
            if (!no_warning)
                Console.WriteLine("Textures Dimensions are too high for a TEX0. Maximum dimensions are the values of a 2 bytes integer (65535 x 65535)");
            return;
        }
        /***** BMP File Process *****/
        // process the bmp file
        int bmp_filesize = bmp_image[2] | bmp_image[3] << 8 | bmp_image[4] << 16 | bmp_image[5] << 24;
        int pixel_data_start_offset = bmp_image[10] | bmp_image[11] << 8 | bmp_image[12] << 16 | bmp_image[13] << 24;
        // bitmap_width = (ushort)(bmp_image[0x13] << 8 | bmp_image[0x12]);
        // bitmap_height = (ushort)(bmp_image[0x17] << 8 | bmp_image[0x16]);
        if ((palette_format_int32[3] == 9 && texture_format_int32[3] > 7 && texture_format_int32[3] < 11) || (texture_format_int32[3] == 5 && alpha == 9)) // if a colour palette hasn't been selected by the user, this program will set it automatically to the most fitting one
        {                                                                                                                                                   // or if alpha hasn't been selected by the user
                                                                                                                                                            //y = 0;
            for (int i = 0; i < bitmap_width; i += 4)  // first row - or last one because it's a bmp lol
            {
                //y += bmp_image[pixel_data_start_offset + i + 3];
                if (bmp_image[pixel_data_start_offset + i + 3] != 0xff && alpha == 9)
                {
                    alpha = 2;
                }
                if (bmp_image[pixel_data_start_offset + i] != bmp_image[pixel_data_start_offset + i + 1] || bmp_image[pixel_data_start_offset + i] == bmp_image[pixel_data_start_offset + i + 2])
                {
                    grey = false;
                }
                if (alpha < 3 && !grey && y != 0)
                {
                    break;
                }
            }
            for (int i = bitmap_width * (bitmap_height >> 1); i % bitmap_width != 0; i += 4)  // middle row
            {
                //y += bmp_image[pixel_data_start_offset + i + 3];
                if (bmp_image[pixel_data_start_offset + i + 3] != 0xff && alpha == 9)
                {
                    alpha = 2;
                }
                if (bmp_image[pixel_data_start_offset + i] != bmp_image[pixel_data_start_offset + i + 1] || bmp_image[pixel_data_start_offset + i] == bmp_image[pixel_data_start_offset + i + 2])
                {
                    grey = false;
                }
                if (alpha < 3 && !grey && y != 0)
                {
                    break;
                }
            }
            //if (y == 0)
            //{
            //not_32bits;
            //}
            if (grey)
            {
                palette_format_int32[3] = 0;  // AI8
            }
            else if (alpha != 9)
            {
                palette_format_int32[3] = 2;  // RGB5A3
            }
            else
            {
                palette_format_int32[3] = 1;  // RGB565
            }
        }
        // I hope there could be a keyboard shortcut to format every tabs :')
        // I'm too lazy to do that manually, + that's just visual lol
        //edit: It's Ctrl + E
        ushort[] mipmap_dimensions = { bitmap_width, bitmap_height, canvas_width, canvas_height };

        canvas_dim.Add(mipmap_dimensions.ToArray());
        Array.Resize(ref colour_palette, colour_number_x2);
        if (BGRA.Count != 0)
        {
            byte[] colors = BGRA.ToArray();
            if (colors.Length > colour_number_x4)
            {
                if (!no_warning)
                    Console.WriteLine((int)(colors.Length / 4) + " colours sent in the palette but colour number is set to " + colour_number + " trimming the palette to " + colour_number);
                Array.Resize(ref colors, colour_number_x4);
            }
            colour_palette = Fill_palette_class.Fill_palette(colors, 0, colors.Length, colour_palette, rgba_channel, custom_rgba, palette_format_int32, algorithm, alpha, round3, round4, round5, round6);
            fill_palette_start_offset = colors.Length >> 1; // divides by two because PLT0 is two bytes per colour
            user_palette = true;
        }
        if (random_palette) // fill the palette randomly :')
        {
            Random rnd = new Random();
            rnd.NextBytes(colour_palette);
        }
        try  // try to see what the second file is
        {
            if (input_file2 != "")
            {
                using (System.IO.FileStream file2 = System.IO.File.Open(input_file2, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    success = false;
                    byte[] id2 = new byte[8];
                    file2.Read(id2, 0, 8);
                    //if (id2.ToString() == "J3D2bmd3") // id2.ToString == "System.Byte[]"
                    if (id2[0] == 74 && id2[1] == 51 && id2[2] == 68 && id2[3] == 50 && id2[4] == 98 && id2[5] == 109 && id2[6] == 100 && id2[7] == 51)
                    {
                        bmd_file = true;
                        success = true;
                    }
                    //else if (id2.ToString().Substring(0, 4) == "PLT0")
                    else if (id2[0] == 80 && id2[1] == 76 && id2[2] == 84 && id2[3] == 48)
                    {
                        success = true;
                        byte[] data = new byte[0x20];
                        file2.Read(data, 0, 0x20);
                        colour_number = (ushort)((data[0x1C] << 8) + data[0x1D]);
                        colour_number_x2 = colour_number << 1;
                        colour_number_x4 = colour_number << 2;
                        palette_format_int32[3] = data[0x1B];
                        // file2.Read(colour_palette, 0x40, colour_number_x2); // check if this is right
                        file2.Position = 0x40;
                        file2.Read(colour_palette, 0, colour_number_x2);
                        user_palette = true;
                    }
                    else if (id2[0] == 0 && id2[1] == 32 && id2[2] == 0xaf && id2[3] == 48)  // TPL
                    {
                        // add the encoded image into this file
                        add = true;
                        success = true;
                    }
                }
                if (!success)
                {
                    byte[] bmp_palette = _bmp.Convert_to_bmp((Bitmap)Bitmap.FromFile(input_file2));  // will fail if this isn't a supported image
                    user_palette = true;
                    int array_size = bmp_palette[2] | bmp_palette[3] << 8 | bmp_palette[4] << 16 | bmp_palette[5] << 24;
                    int pixel_start_offset = bmp_palette[10] | bmp_palette[11] << 8 | bmp_palette[12] << 16 | bmp_palette[13] << 24;
                    ushort bitmap_w = (ushort)(bmp_palette[0x13] << 8 | bmp_palette[0x12]);
                    ushort bitmap_h = (ushort)(bmp_palette[0x17] << 8 | bmp_palette[0x16]);
                    ushort pixel = (ushort)(bitmap_w * bitmap_h);
                    if (pixel != colour_number)
                    {
                        if (!no_warning)
                            Console.WriteLine("Second image input has " + pixel + " pixels while there are " + colour_number + " max colours in the palette.");
                        return;
                    }
                    Fill_palette_class.Fill_palette(bmp_palette, pixel_start_offset, array_size, colour_palette, rgba_channel, custom_rgba, palette_format_int32, algorithm, alpha, round3, round4, round5, round6);
                }

            }
        }
        catch (Exception ex)
        {
            if (file2_conflict)
            {
                // the arg parser purposefully made a mistake lol, this happens if the output file already exists and input_file2 is empty
                // let's just pretend the program ran fine and forget about this 
            }
            else if (ex.Message == "Out of memory." && ex.Source == "System.Drawing")
            {
                if (!no_warning)
                    Console.WriteLine("Second image input format not supported (convert it to jpeg or png)");
            }
            else if (safe_mode)
            {
                if (!no_warning)
                    Console.WriteLine("error on " + input_file2 + "\nsafe mode is enabled, this program will exit silently");
            }
            else
            {
                throw ex;
            }
            return;
        }
        Create_plt0_class _plt0 = new Create_plt0_class(this);
        List<List<byte[]>> index_list = new List<List<byte[]>>();
        object v = _plt0.Create_plt0(bmp_image, bmp_filesize, pixel_data_start_offset);
        index_list.Add((List<byte[]>)v);
        if (warn)
        {
            if (!no_warning)
                Console.WriteLine("v-- bool --v\nbmd=" + bmd + " bmd_file=" + bmd_file + " bmp=" + bmp + " bti=" + bti + " fill_height=" + fill_height + " fill_width=" + fill_width + " gif=" + gif + " grey=" + grey + " ico=" + ico + " jpeg=" + jpeg + " jpg=" + jpg + " png=" + png + " success=" + success + " tif=" + tif + " tiff=" + tiff + " tpl=" + tpl + " user_palette=" + user_palette + " warn=" + warn + "\n\nv-- byte --v\nWrapS=" + WrapS + " WrapT=" + WrapT + " algorithm=" + algorithm + " alpha=" + alpha + " color=" + color + " diversity=" + diversity + " diversity2=" + diversity2 + " magnification_filter=" + magnification_filter + " minificaction_filter=" + minificaction_filter + " mipmaps_number=" + mipmaps_number + "\n\nv-- byte[] --v\ncolour_palette=" + colour_palette + " palette_format_int32=" + palette_format_int32 + " texture_format_int32=" + texture_format_int32 + "\n\nv-- double --v\nformat_ratio=" + format_ratio + " percentage=" + percentage + " percentage2=" + percentage2 + "\n\nv-- float[] --v\ncustom_rgba=" + custom_rgba + "\n\nv-- int --v\ncolour_number_x2=" + colour_number_x2 + " colour_number_x4=" + colour_number_x4 + " pass=" + pass + " pixel_count=" + pixel_count + "\n\nv-- signed byte --v\nblock_height=" + block_height + " block_width=" + block_width + "\n\nv-- string --v\ninput_file=" + input_file + " input_file2=" + input_file2 + " output_file=" + output_file + " swap=" + swap + "\n\nv-- unsigned short --v\nbitmap_height=" + bitmap_height + " bitmap_width=" + bitmap_width + " colour_number=" + colour_number + " max_colours=" + max_colours + " z=" + z + "\n\nv-- List<byte> --v\nBGRA=" + BGRA);
        }
        for (z = 1; z <= mipmaps_number; z++)
        {
            if (warn)
            {
                if (!no_warning)
                    Console.WriteLine("processing mipmap " + z);
            }
            if (System.IO.File.Exists(input_fil + ".mm" + z + input_ext))  // image with mipmap: input.png -> input.mm1.png -> input.mm2.png
            {
                byte[] bmp_mipmap = _bmp.Convert_to_bmp((Bitmap)Bitmap.FromFile(input_fil + ".mm" + z + input_ext));
                if (bmp_mipmap[0x15] != 0 || bmp_mipmap[0x14] != 0 || bmp_mipmap[0x19] != 0 || bmp_mipmap[0x18] != 0)
                {
                    if (!no_warning)
                        Console.WriteLine("Textures Dimensions are too high for a TEX0. Maximum dimensions are the values of a 2 bytes integer (65535 x 65535)");
                    return;
                }
                /***** BMP File Process *****/
                // process the bmp file
                int bmp_size = bmp_mipmap[2] | bmp_mipmap[3] << 8 | bmp_mipmap[4] << 16 | bmp_mipmap[5] << 24;
                int pixel_start_offset = bmp_mipmap[10] | bmp_mipmap[11] << 8 | bmp_mipmap[12] << 16 | bmp_mipmap[13] << 24;
                //bitmap_width = (ushort)(bmp_mipmap[0x13] << 8 | bmp_mipmap[0x12]);
                //bitmap_height = (ushort)(bmp_mipmap[0x17] << 8 | bmp_mipmap[0x16]);
                //pixel_count = bitmap_width * bitmap_height;
                user_palette = true; // won't edit palette with mipmaps
                object w = _plt0.Create_plt0(bmp_mipmap, bmp_size, pixel_start_offset);
                index_list.Add((List<byte[]>)w);
            }
            else
            {
                bitmap_width >>= 1; // divides by 2
                bitmap_height >>= 1; // divides by 2   - also YES 1 DIVIDED BY TWO IS ZERO
                                     // note: depending on the number of mipmaps, this will make unused block space with images that are not power of two because sooner or later width or height won't be a multiple or 4 or 8
                if (bitmap_width == 0 || bitmap_height == 0)
                {
                    if (!no_warning)
                        Console.WriteLine("Too many mipmaps. " + (z - 1) + " is the maximum for this file");
                    exit = true;
                    return;
                }
                byte[] bmp_mipmap = _bmp.Convert_to_bmp(ResizeImage_class.ResizeImage((Bitmap)Bitmap.FromFile(input_file), bitmap_width, bitmap_height));


                if (bmp_mipmap[0x15] != 0 || bmp_mipmap[0x14] != 0 || bmp_mipmap[0x19] != 0 || bmp_mipmap[0x18] != 0)
                {
                    if (!no_warning)
                        Console.WriteLine("Textures Dimensions are too high for a TEX0. Maximum dimensions are the values of a 2 bytes integer (65535 x 65535)");
                    return;
                }
                /***** BMP File Process *****/
                // process the bmp file
                int bmp_size = bmp_mipmap[2] | bmp_mipmap[3] << 8 | bmp_mipmap[4] << 16 | bmp_mipmap[5] << 24;
                int pixel_start_offset = bmp_mipmap[10] | bmp_mipmap[11] << 8 | bmp_mipmap[12] << 16 | bmp_mipmap[13] << 24;
                //bitmap_width = (ushort)(bmp_mipmap[0x13] << 8 | bmp_mipmap[0x12]);
                //bitmap_height = (ushort)(bmp_mipmap[0x17] << 8 | bmp_mipmap[0x16]);
                //pixel_count = bitmap_width * bitmap_height;
                user_palette = true; // won't edit palette with mipmaps
                object w = _plt0.Create_plt0(bmp_mipmap, bmp_size, pixel_start_offset);
                index_list.Add((List<byte[]>)w);
            }
            mipmap_dimensions[2] = canvas_width;
            mipmap_dimensions[3] = canvas_height;
            canvas_dim.Add(mipmap_dimensions.ToArray());
        }
        if (exit)
        {
            return;
        }
        bitmap_width = canvas_dim[0][0];
        bitmap_height = canvas_dim[0][1];
        canvas_width = canvas_dim[0][2];
        canvas_height = canvas_dim[0][3];
        if (bti || bmd)
        {
            if (bmd && !bmd_file)
            {
                if (!no_warning)
                    Console.WriteLine("specified bmd output but no bmd file given");
                return;
            }
            Write_bti_class.Write_bti(index_list, colour_palette, texture_format_int32, palette_format_int32, block_width_array, block_height_array, bitmap_width, bitmap_height, colour_number, format_ratio, input_fil, input_file2, output_file, bmd_file, has_palette, safe_mode, no_warning, warn, stfu, block_width, block_height, mipmaps_number, minificaction_filter, magnification_filter, WrapS, WrapT, alpha);
        }
        if (tpl)
        {
            if (add)
            {
                Write_into_tpl_class.Write_into_tpl(index_list, colour_palette, texture_format_int32, palette_format_int32, real_block_width_array, block_height_array, add_depth, sub_depth, bitmap_width, bitmap_height, colour_number, format_ratio, input_file2, output_file, has_palette, overwrite, safe_mode, no_warning, warn, stfu, block_width, block_height, mipmaps_number, minificaction_filter, magnification_filter, WrapS, WrapT);
            }
            else
            {
                Write_tpl_class.Write_tpl(index_list, colour_palette, texture_format_int32, palette_format_int32, bitmap_width, bitmap_height, colour_number, format_ratio, output_file, has_palette, safe_mode, no_warning, warn, stfu, block_width, block_height, mipmaps_number, minificaction_filter, magnification_filter, WrapS, WrapT);
            }
        }
        if (bmp || png || tif || tiff || ico || jpg || jpeg || gif)  // tell me if there's another format available through some extensions I'll add it
        {
            Write_bmp_class.Write_bmp(index_list, canvas_dim, colour_palette, texture_format_int32, palette_format_int32, colour_number, output_file, bmp_32, funky, has_palette, warn, stfu, no_warning, safe_mode, png, gif, jpeg, jpg, ico, tiff, tif, mipmaps_number, alpha, colour_number_x2, colour_number_x4);
        }
        if (tex0)
        {
            if (has_palette)
            {
                Write_plt0_class.Write_plt0(colour_palette, palette_format_int32, colour_number, output_file, safe_mode, no_warning, warn, stfu);
            }
            Write_tex0_class.Write_tex0(index_list, texture_format_int32, bitmap_width, bitmap_height, format_ratio, output_file, has_palette, safe_mode, no_warning, warn, stfu, block_width, block_height, mipmaps_number);
        }

        /* catch (Exception ex)  // remove this when debugging else it'll tell you every error were at this line lol
        {
            if (ex.Message == "Out of memory." && ex.Source == "System.Drawing")
                Console.WriteLine("Image input format not supported (convert it to jpeg or png)");
            else if (safe_mode)
            {
                Console.WriteLine("error on " + input_file + "\nsafe mode is enabled, this program will exit silently");
            }
            else
            {
                Console.WriteLine("\n\nPlease report this error on github https://github.com/yoshi2999/plt0/issues or to yosh#0304 on discord. Preferably with the file you used and the full command causing this.\n\nThe command you used is : \n" + args.ToString());
                throw ex;
            }
            return;
        }*/

    }
    public void Check_exit()
    {
        if (ask_exit)
        {
            Console.WriteLine("\nPress enter to exit...");
            Console.ReadLine();
        }
    }
}