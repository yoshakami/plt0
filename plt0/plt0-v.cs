using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
//using System.Drawing.Text;
//using System.Linq;

namespace plt0_gui
{
    public partial class plt0_gui : Form
    {
        static readonly string execPath = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");
        static readonly string execName = System.AppDomain.CurrentDomain.FriendlyName.Replace("\\", "/");
        static readonly string appdata = Environment.GetEnvironmentVariable("appdata").Replace("\\", "/");
        static string args;
        string[] lines = new string[255];
        string[] layout_name = { "All", "Auto", "Preview", "Paint" };
        byte[] block_width_array = { 8, 8, 8, 4, 4, 4, 4, 255, 8, 8, 4, 255, 255, 255, 8 }; // real one to calculate canvas size.
        //byte[] block_width_array = { 4, 8, 8, 8, 8, 8, 16, 255, 4, 8, 8, 255, 255, 255, 4 }; // altered to match bit-per pixel size.
        byte[] block_height_array = { 8, 4, 4, 4, 4, 4, 4, 255, 8, 4, 4, 255, 255, 255, 8 }; // 255 = unused image format
        byte[] block_depth_array = { 4, 8, 4, 8, 16, 16, 32, 255, 4, 8, 16, 255, 255, 255, 4 };  // for \z
        string[] encoding_array = { "i4", "i8", "ai4", "ai8", "rgb565", "rgb5a3", "rgba32", "", "ci4", "ci8", "ci14x2", "", "", "", "cmpr" };
        string[] wrap_array = { "Clamp", "Repeat", "Mirror", "Clamp" };
        string[] algorithm_array = { "", "cie709", "custom_rgba", "no gradient" };
        protected internal volatile string[] alpha_array = { "no alpha", "alpha", "mix" }; // imagine putting random keywords
        static readonly private protected string[] rgba_array = { "R", "G", "B", "A" }; // imagine knowing what the keywords do
        string input_file;
        string output_name = "";
        string input_file2;
        static char separator;
        static string font_name = "";
        static string font_colour = "";
        static string font_encoding = "";
        static string font_size = "";
        static string font_unit = "";
        string[] markdown = { font_name, font_colour, font_unit, font_encoding, font_size };
        //string backslash_j;
        //string backslash_n;
        byte GdiCharSet;
        byte run_count;
        //byte jump_line;
        float size_font;
        System.Drawing.GraphicsUnit unit_font;
        System.Windows.Forms.Padding cli_textbox_padding = new System.Windows.Forms.Padding(0, 12, 0, 20);
        System.Drawing.Point cli_textbox_location = new System.Drawing.Point(71, 1000);
        int y;
        byte byte_text;
        bool success;
        /* bool bold;
        bool italic;
        bool underline;
        bool strikeout;*/
        byte font_style;
        bool vertical;
        // banner_move
        int mouse_x;
        int mouse_y;
        bool mouse_down;
        // checkboxes
        bool bmd = false;
        bool bti = false;
        bool tex0 = false;
        bool tpl = false;
        bool bmp = false;
        bool png = false;
        bool ico = false;
        bool jpeg = false;
        bool jpg = false;
        bool gif = false;
        bool tif = false;
        bool tiff = false;
        // options
        bool ask_exit = false;
        bool FORCE_ALPHA = false;
        bool funky = false;
        bool bmp_32 = false;
        bool random = false;
        bool safe_mode = false;
        bool reverse = false;
        bool warn = false;
        bool stfu = false;
        bool no_warning = false;
        //view
        bool view_alpha = true;
        bool view_WrapS = true;
        bool view_WrapT = true;
        bool view_algorithm = true;
        bool view_min = true;
        bool view_mag = true;
        bool view_rgba = true;
        bool view_palette = true;
        bool view_cmpr = true;
        bool view_options = true;
        // radiobuttons
        byte encoding = 7;
        byte palette_enc = 3;
        byte WrapS = 3; // 0 = Clamp   1 = Repeat   2 = Mirror
        byte WrapT = 3; // 0 = Clamp   1 = Repeat   2 = Mirror
        byte algorithm = 4;  // 0 = CIE 601    1 = CIE 709     2 = custom RGBA     3 = Most Used Colours (No Gradient)
        byte alpha = 3;  // 0 = no alpha - 1 = alpha - 2 = mix 
        byte magnification_filter = 6;  // 0 = Nearest Neighbour   1 = Linear
        byte minification_filter = 6;  // 0 = Nearest Neighbour   1 = Linear
        byte r = 0;
        byte g = 1;
        byte b = 2;
        byte a = 3;
        // numbers
        byte cmpr_max = 16;  // number of colours that the program should take care in each 4x4 block - should always be set to 16 for better results.  // wimgt's cmpr encoding is better than mine. I gotta admit. 
        byte cmpr_min_alpha = 100; // byte cmpr_alpha_threshold = 100;
        byte diversity = 10;
        byte diversity2 = 0;
        byte mipmaps = 0;
        byte round3 = 15;
        byte round4 = 7;
        byte round5 = 3;
        byte round6 = 1;
        ushort num_colours = 0;
        byte layout;
        byte arrow;
        int number;
        int len;
        double percentage = 0;
        double percentage2 = 0;
        double custom_r = 1.0;
        double custom_g = 1.0;
        double custom_b = 1.0;
        double custom_a = 1.0;
        List<string> arg_array = new List<string>();
        List<Label> desc = new List<Label>(); // duplicates of the description label (used to have more than 1 font colour, style, family, and encoding too)
        List<PictureBox> encoding_ck = new List<PictureBox>();
        List<PictureBox> palette_ck = new List<PictureBox>();
        List<PictureBox> a_ck = new List<PictureBox>();
        List<PictureBox> b_ck = new List<PictureBox>();
        List<PictureBox> g_ck = new List<PictureBox>();
        List<PictureBox> r_ck = new List<PictureBox>();
        List<PictureBox> magnification_ck = new List<PictureBox>();
        List<PictureBox> minification_ck = new List<PictureBox>();
        List<PictureBox> WrapS_ck = new List<PictureBox>();
        List<PictureBox> WrapT_ck = new List<PictureBox>();
        List<PictureBox> alpha_ck_array = new List<PictureBox>();
        List<PictureBox> algorithm_ck = new List<PictureBox>();
        Image banner;
        Image background;
        Image white_box;
        Image light_blue_box;
        Image light_blue_check;
        Image check;
        Image pink_circle;
        Image pink_circle_on;
        Image violet_circle;
        Image violet_circle_on;
        Image white_circle;
        Image white_circle_on;
        Image green_circle;
        Image green_circle_on;
        Image light_blue_circle;
        Image light_blue_circle_on;
        Image blue_circle;
        Image blue_circle_on;
        Image light_red_circle;
        Image light_red_circle_on;
        Image red_circle;
        Image red_circle_on;
        Image yellow_circle;
        Image yellow_circle_on;
        Image orange_circle;
        Image orange_circle_on;
        Image fushia_circle;
        Image fushia_circle_on;
        Image cyan_circle;
        Image cyan_circle_on;
        Image cherry_circle;
        Image cherry_circle_on;
        Image purple_circle;
        Image purple_circle_on;
        Image chartreuse_circle;
        Image chartreuse_circle_on;
        Image a_on;
        Image a_off;
        Image a_hover;
        Image a_selected;
        Image b_on;
        Image b_off;
        Image b_hover;
        Image b_selected;
        Image g_on;
        Image g_off;
        Image g_hover;
        Image g_selected;
        Image r_on;
        Image r_off;
        Image r_hover;
        Image r_selected;
        Image surrounding;
        Image all_on;
        Image all_off;
        Image all_hover;
        Image all_selected;
        Image auto_on;
        Image auto_off;
        Image auto_hover;
        Image auto_selected;
        Image preview_on;
        Image preview_off;
        Image preview_hover;
        Image preview_selected;
        Image paint_on;
        Image paint_off;
        Image paint_hover;
        Image paint_selected;
        Image bottom_left_on;
        Image bottom_left_off;
        Image bottom_left_hover;
        Image bottom_left_selected;
        Image left_on;
        Image left_off;
        Image left_hover;
        Image left_selected;
        Image top_left_on;
        Image top_left_off;
        Image top_left_hover;
        Image top_left_selected;
        Image bottom_right_on;
        Image bottom_right_off;
        Image bottom_right_hover;
        Image bottom_right_selected;
        Image right_on;
        Image right_off;
        Image right_hover;
        Image right_selected;
        Image top_right_on;
        Image top_right_off;
        Image top_right_hover;
        Image top_right_selected;
        Image bottom_on;
        Image bottom_off;
        Image bottom_hover;
        Image bottom_selected;
        Image top_on;
        Image top_off;
        Image top_hover;
        Image top_selected;
        Image close;
        Image close_hover;
        Image maximized_on;
        Image maximized_off;
        Image maximized_hover;
        Image maximized_selected;
        Image minimized;
        Image minimized_hover;
        Image youtube;
        Image discord;
        Image github;
        Image youtube_hover;
        Image discord_hover;
        Image github_hover;
        Image run_on;
        Image run_off;
        Image run_hover;
        Image cli_textbox;
        Image cli_textbox_hover;
        Image version;
        Image version_hover;
        // I couldn't manage to get external fonts working. this needs to be specified within the app itself :/
        // static string fontname = "Segoe UI";
        Image input_file_image;
        // Font font_normal = new System.Drawing.Font(fontname, 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
        // Font new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
        public plt0_gui()
        {
            //if (System.IO.File.Exists(execPath + "images/a.ttf"))
            //{
            // AddFont(System.IO.File.ReadAllBytes(execPath + "images/font.ttf"));
            // fontname = Get_font_name();
            /*FNintendoP-NewRodin DB
             * using (PrivateFontCollection _privateFontCollection = new PrivateFontCollection())
            {
                _privateFontCollection.AddFontFile(execPath + "images/a.ttf");
                fontname = _privateFontCollection.Families[0].Name;
            }
            font_normal = new System.Drawing.Font(fontname, 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            /*PrivateFontCollection collection = new PrivateFontCollection();
            collection.AddFontFile("images/font.ttf");
            FontFamily fontFamily = new FontFamily(collection.Families[0].Name, collection);
            font_normal = new System.Drawing.Font(fontFamily, 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);*/
            //}
            InitializeComponent();
            //output_file_type_label.Text = fontname;
            //algorithm_label.Text = algorithm_label.Font.Name;
            //markdown.Add(font_name);
            //markdown.Add(font_colour);
            //markdown.Add(font_unit);
            //markdown.Add(font_encoding);
            //markdown.Add(font_size);
            encoding_ck.Add(i4_ck);
            encoding_ck.Add(i8_ck);
            encoding_ck.Add(ai4_ck);
            encoding_ck.Add(ai8_ck);
            encoding_ck.Add(rgb565_ck);
            encoding_ck.Add(rgb5a3_ck);
            encoding_ck.Add(rgba32_ck);
            encoding_ck.Add(i4_ck);  // nothing
            encoding_ck.Add(ci4_ck);
            encoding_ck.Add(ci8_ck);
            encoding_ck.Add(ci14x2_ck);
            encoding_ck.Add(i4_ck);  // nothing
            encoding_ck.Add(i4_ck);  // nothing
            encoding_ck.Add(i4_ck);  // nothing
            encoding_ck.Add(cmpr_ck);
            r_ck.Add(r_r_ck);
            r_ck.Add(r_g_ck);
            r_ck.Add(r_b_ck);
            r_ck.Add(r_a_ck);
            g_ck.Add(g_r_ck);
            g_ck.Add(g_g_ck);
            g_ck.Add(g_b_ck);
            g_ck.Add(g_a_ck);
            b_ck.Add(b_r_ck);
            b_ck.Add(b_g_ck);
            b_ck.Add(b_b_ck);
            b_ck.Add(b_a_ck);
            a_ck.Add(a_r_ck);
            a_ck.Add(a_g_ck);
            a_ck.Add(a_b_ck);
            a_ck.Add(a_a_ck);
            magnification_ck.Add(mag_nearest_neighbour_ck);
            magnification_ck.Add(mag_linear_ck);
            magnification_ck.Add(mag_nearestmipmapnearest_ck);
            magnification_ck.Add(mag_nearestmipmaplinear_ck);
            magnification_ck.Add(mag_linearmipmapnearest_ck);
            magnification_ck.Add(mag_linearmipmaplinear_ck);
            magnification_ck.Add(mag_linearmipmaplinear_ck); // nothing
            minification_ck.Add(min_nearest_neighbour_ck);
            minification_ck.Add(min_linear_ck);
            minification_ck.Add(min_nearestmipmapnearest_ck);
            minification_ck.Add(min_nearestmipmaplinear_ck);
            minification_ck.Add(min_linearmipmapnearest_ck);
            minification_ck.Add(min_linearmipmaplinear_ck);
            minification_ck.Add(min_linearmipmaplinear_ck); // nothing
            WrapS_ck.Add(Sclamp_ck);
            WrapS_ck.Add(Srepeat_ck);
            WrapS_ck.Add(Smirror_ck);
            WrapS_ck.Add(Smirror_ck);  // nothing
            WrapT_ck.Add(Tclamp_ck);
            WrapT_ck.Add(Trepeat_ck);
            WrapT_ck.Add(Tmirror_ck);
            WrapT_ck.Add(Tmirror_ck);  // nothing
            alpha_ck_array.Add(no_alpha_ck);
            alpha_ck_array.Add(alpha_ck);
            alpha_ck_array.Add(mix_ck);
            alpha_ck_array.Add(mix_ck);  // nothing
            algorithm_ck.Add(cie_601_ck);
            algorithm_ck.Add(cie_709_ck);
            algorithm_ck.Add(custom_ck);
            algorithm_ck.Add(no_gradient_ck);
            algorithm_ck.Add(cie_601_ck);  // nothing
            palette_ck.Add(palette_ai8_ck);
            palette_ck.Add(palette_rgb565_ck);
            palette_ck.Add(palette_rgb5a3_ck);
            palette_ck.Add(palette_ai8_ck);  // nothing
            desc.Add(description);
            desc.Add(desc2);
            desc.Add(desc3);
            desc.Add(desc4);
            desc.Add(desc5);
            desc.Add(desc6);
            desc.Add(desc7);
            desc.Add(desc8);
            desc.Add(desc9);
            Load_Images();
            banner_ck.BackgroundImage = banner;
            surrounding_ck.BackgroundImage = surrounding;
            banner_minus_ck.BackgroundImage = minimized;
            banner_x_ck.BackgroundImage = close;
            discord_ck.BackgroundImage = discord;
            github_ck.BackgroundImage = github;
            youtube_ck.BackgroundImage = youtube;
            version_ck.BackgroundImage = version;
            run_ck.BackgroundImage = run_off;
            cli_textbox_ck.BackgroundImage = cli_textbox;
            this.BackgroundImage = background;
            unchecked_checkbox(ask_exit_ck);
            unchecked_checkbox(FORCE_ALPHA_ck);
            unchecked_checkbox(jpeg_ck);
            unchecked_checkbox(jpg_ck);
            unchecked_checkbox(bti_ck);
            unchecked_checkbox(bmd_ck);
            unchecked_checkbox(ico_ck);
            unchecked_checkbox(bmp_ck);
            unchecked_checkbox(bmp_32_ck);
            unchecked_checkbox(stfu_ck);
            unchecked_checkbox(safe_mode_ck);
            unchecked_checkbox(warn_ck);
            unchecked_checkbox(tpl_ck);
            unchecked_checkbox(tiff_ck);
            unchecked_checkbox(tif_ck);
            unchecked_checkbox(tex0_ck);
            unchecked_checkbox(png_ck);
            unchecked_checkbox(random_ck);
            unchecked_checkbox(reverse_ck);
            unchecked_checkbox(funky_ck);
            unchecked_checkbox(no_warning_ck);
            unchecked_checkbox(gif_ck);
            unchecked_encoding(i4_ck);
            unchecked_encoding(i8_ck);
            unchecked_encoding(ai4_ck);
            unchecked_encoding(ai8_ck);
            unchecked_encoding(rgb565_ck);
            unchecked_encoding(rgb5a3_ck);
            unchecked_encoding(rgba32_ck);
            unchecked_encoding(ci4_ck);
            unchecked_encoding(ci8_ck);
            unchecked_encoding(ci14x2_ck);
            unchecked_encoding(cmpr_ck);
            unchecked_R(a_r_ck);
            unchecked_G(a_g_ck);
            unchecked_B(a_b_ck);
            checked_A(a_a_ck);
            unchecked_R(b_r_ck);
            unchecked_G(b_g_ck);
            checked_B(b_b_ck);
            unchecked_A(b_a_ck);
            unchecked_R(g_r_ck);
            checked_G(g_g_ck);
            unchecked_B(g_b_ck);
            unchecked_A(g_a_ck);
            checked_R(r_r_ck);
            unchecked_G(r_g_ck);
            unchecked_B(r_b_ck);
            unchecked_A(r_a_ck);
            unchecked_Minification(min_nearest_neighbour_ck);
            unchecked_Minification(min_linear_ck);
            unchecked_Minification(min_nearestmipmapnearest_ck);
            unchecked_Minification(min_nearestmipmaplinear_ck);
            unchecked_Minification(min_linearmipmapnearest_ck);
            unchecked_Minification(min_linearmipmaplinear_ck);
            unchecked_Magnification(mag_nearest_neighbour_ck);
            unchecked_Magnification(mag_linear_ck);
            unchecked_Magnification(mag_nearestmipmapnearest_ck);
            unchecked_Magnification(mag_nearestmipmaplinear_ck);
            unchecked_Magnification(mag_linearmipmapnearest_ck);
            unchecked_Magnification(mag_linearmipmaplinear_ck);
            unchecked_WrapT(Tclamp_ck);
            unchecked_WrapT(Trepeat_ck);
            unchecked_WrapT(Tmirror_ck);
            unchecked_WrapS(Sclamp_ck);
            unchecked_WrapS(Srepeat_ck);
            unchecked_WrapS(Smirror_ck);
            unchecked_alpha(no_alpha_ck);
            unchecked_alpha(alpha_ck);
            unchecked_alpha(mix_ck);
            unchecked_algorithm(cie_601_ck);
            unchecked_algorithm(cie_709_ck);
            unchecked_algorithm(custom_ck);
            unchecked_algorithm(no_gradient_ck);
            Category_checked(view_alpha_ck);
            Category_checked(view_algorithm_ck);
            Category_checked(view_WrapS_ck);
            Category_checked(view_WrapT_ck);
            Category_checked(view_min_ck);
            Category_checked(view_mag_ck);
            unchecked_palette(palette_ai8_ck);
            unchecked_palette(palette_rgb565_ck);
            unchecked_palette(palette_rgb5a3_ck);
            if (System.IO.File.Exists(execPath + "images/settings.txt"))
            {
                string version = "";
                lines = System.IO.File.ReadAllLines(execPath + "images/settings.txt");
                if (lines.Length > 0)
                {
                    version = lines[0].Substring(12);
                }

                switch (version)
                {
                    case "v1.0":
                        if (version == "v1.0" && lines.Length < 200)  // incorrect v1.0 config file
                        {
                            //  System.Diagnostics.Debug.WriteLine("some tetttttttttt23423423423423423ttttttttttttttttttttttt");
                            Console.WriteLine("plt0 v1.0 config file should have EXACTLY 200 lines");
                            Console.ReadLine();
                            Environment.Exit(1);
                        }
                        break; // in case it needs to get out of this switch
                               // sarcarm++;
                    default:
                        Console.WriteLine("incorrect config version. " + execPath + "images/settings.txt's First line isn't recognized by this tool");
                        Console.ReadLine();
                        Environment.Exit(2);
                        break; // idk what happens if you don't put a break on a case, but it won't compile otherwise

                }
                switch (lines[2].ToUpper())
                {
                    case "ALL":
                        layout = 0;
                        checked_All();
                        unchecked_Auto();
                        unchecked_Preview();
                        unchecked_Paint();
                        /*
                        View_alpha();
                        View_algorithm();
                        View_WrapS();
                        View_WrapT();
                        View_mag();
                        View_min();*/
                        break;
                    case "AUTO":
                        unchecked_All();
                        checked_Auto();
                        unchecked_Preview();
                        unchecked_Paint();
                        layout = 1;
                        break;
                    case "PREVIEW":
                        unchecked_All();
                        unchecked_Auto();
                        checked_Preview();
                        unchecked_Paint();
                        layout = 2;
                        //View_algorithm();
                        //View_alpha();
                        // view encoding and channel swap and some options
                        break;
                    case "PAINT":
                        unchecked_All();
                        unchecked_Auto();
                        unchecked_Preview();
                        checked_Paint();
                        layout = 3;
                        break;
                }
                switch (lines[4].ToUpper())
                {
                    case "MAXIMIZED":
                        this.WindowState = FormWindowState.Maximized;
                        banner_1_ck.BackgroundImage = bottom_left_off;
                        banner_2_ck.BackgroundImage = bottom_off;
                        banner_3_ck.BackgroundImage = bottom_right_off;
                        banner_4_ck.BackgroundImage = left_off;
                        banner_5_ck.BackgroundImage = maximized_on;
                        banner_6_ck.BackgroundImage = right_off;
                        banner_7_ck.BackgroundImage = top_left_off;
                        banner_8_ck.BackgroundImage = top_off;
                        banner_9_ck.BackgroundImage = top_right_off;
                        break;
                    case "NORMAL":
                        // default
                        banner_1_ck.BackgroundImage = bottom_left_off;
                        banner_2_ck.BackgroundImage = bottom_off;
                        banner_3_ck.BackgroundImage = bottom_right_off;
                        banner_4_ck.BackgroundImage = left_off;
                        banner_5_ck.BackgroundImage = maximized_off;
                        banner_6_ck.BackgroundImage = right_off;
                        banner_7_ck.BackgroundImage = top_left_off;
                        banner_8_ck.BackgroundImage = top_off;
                        banner_9_ck.BackgroundImage = top_right_off;
                        break;
                    case "LEFT":
                        arrow = 4;
                        banner_1_ck.BackgroundImage = bottom_left_off;
                        banner_2_ck.BackgroundImage = bottom_off;
                        banner_3_ck.BackgroundImage = bottom_right_off;
                        banner_4_ck.BackgroundImage = left_on;
                        banner_5_ck.BackgroundImage = maximized_off;
                        banner_6_ck.BackgroundImage = right_off;
                        banner_7_ck.BackgroundImage = top_left_off;
                        banner_8_ck.BackgroundImage = top_off;
                        banner_9_ck.BackgroundImage = top_right_off;
                        break;
                    case "TOP_LEFT":
                        arrow = 7;
                        banner_1_ck.BackgroundImage = bottom_left_off;
                        banner_2_ck.BackgroundImage = bottom_off;
                        banner_3_ck.BackgroundImage = bottom_right_off;
                        banner_4_ck.BackgroundImage = left_off;
                        banner_5_ck.BackgroundImage = maximized_off;
                        banner_6_ck.BackgroundImage = right_off;
                        banner_7_ck.BackgroundImage = top_left_on;
                        banner_8_ck.BackgroundImage = top_off;
                        banner_9_ck.BackgroundImage = top_right_off;
                        break;
                    case "TOP":
                        arrow = 8;
                        banner_1_ck.BackgroundImage = bottom_left_off;
                        banner_2_ck.BackgroundImage = bottom_off;
                        banner_3_ck.BackgroundImage = bottom_right_off;
                        banner_4_ck.BackgroundImage = left_off;
                        banner_5_ck.BackgroundImage = maximized_off;
                        banner_6_ck.BackgroundImage = right_off;
                        banner_7_ck.BackgroundImage = top_left_off;
                        banner_8_ck.BackgroundImage = top_on;
                        banner_9_ck.BackgroundImage = top_right_off;
                        break;
                    case "TOP_RIGHT":
                        arrow = 9;
                        banner_1_ck.BackgroundImage = bottom_left_off;
                        banner_2_ck.BackgroundImage = bottom_off;
                        banner_3_ck.BackgroundImage = bottom_right_off;
                        banner_4_ck.BackgroundImage = left_off;
                        banner_5_ck.BackgroundImage = maximized_off;
                        banner_6_ck.BackgroundImage = right_off;
                        banner_7_ck.BackgroundImage = top_left_off;
                        banner_8_ck.BackgroundImage = top_off;
                        banner_9_ck.BackgroundImage = top_right_on;
                        break;
                    case "RIGHT":
                        arrow = 6;
                        banner_1_ck.BackgroundImage = bottom_left_off;
                        banner_2_ck.BackgroundImage = bottom_off;
                        banner_3_ck.BackgroundImage = bottom_right_off;
                        banner_4_ck.BackgroundImage = left_off;
                        banner_5_ck.BackgroundImage = maximized_off;
                        banner_6_ck.BackgroundImage = right_on;
                        banner_7_ck.BackgroundImage = top_left_off;
                        banner_8_ck.BackgroundImage = top_off;
                        banner_9_ck.BackgroundImage = top_right_off;
                        break;
                    case "BOTTOM_RIGHT":
                        arrow = 3;
                        banner_1_ck.BackgroundImage = bottom_left_off;
                        banner_2_ck.BackgroundImage = bottom_off;
                        banner_3_ck.BackgroundImage = bottom_right_on;
                        banner_4_ck.BackgroundImage = left_off;
                        banner_5_ck.BackgroundImage = maximized_off;
                        banner_6_ck.BackgroundImage = right_off;
                        banner_7_ck.BackgroundImage = top_left_off;
                        banner_8_ck.BackgroundImage = top_off;
                        banner_9_ck.BackgroundImage = top_right_off;
                        break;
                    case "BOTTOM":
                        arrow = 2;
                        banner_1_ck.BackgroundImage = bottom_left_off;
                        banner_2_ck.BackgroundImage = bottom_on;
                        banner_3_ck.BackgroundImage = bottom_right_off;
                        banner_4_ck.BackgroundImage = left_off;
                        banner_5_ck.BackgroundImage = maximized_off;
                        banner_6_ck.BackgroundImage = right_off;
                        banner_7_ck.BackgroundImage = top_left_off;
                        banner_8_ck.BackgroundImage = top_off;
                        banner_9_ck.BackgroundImage = top_right_off;
                        break;
                    case "BOTTOM_LEFT":
                        arrow = 1;
                        banner_1_ck.BackgroundImage = bottom_left_on;
                        banner_2_ck.BackgroundImage = bottom_off;
                        banner_3_ck.BackgroundImage = bottom_right_off;
                        banner_4_ck.BackgroundImage = left_off;
                        banner_5_ck.BackgroundImage = maximized_off;
                        banner_6_ck.BackgroundImage = right_off;
                        banner_7_ck.BackgroundImage = top_left_off;
                        banner_8_ck.BackgroundImage = top_off;
                        banner_9_ck.BackgroundImage = top_right_off;
                        break;
                }
                input_file_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                input_file2_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                output_name_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                mipmaps_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                cmpr_max_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                cmpr_min_alpha_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                num_colours_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                round3_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                round4_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                round5_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                round6_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                diversity_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                diversity2_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                percentage_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                percentage2_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                custom_r_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                custom_g_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                custom_b_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);
                custom_a_txt.ForeColor = System.Drawing.Color.FromName(lines[6]);

                input_file_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                input_file2_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                output_name_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                mipmaps_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                cmpr_max_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                cmpr_min_alpha_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                num_colours_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                round3_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                round4_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                round5_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                round6_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                diversity_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                diversity2_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                percentage_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                percentage2_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                custom_r_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                custom_g_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                custom_b_txt.BackColor = System.Drawing.Color.FromName(lines[8]);
                custom_a_txt.BackColor = System.Drawing.Color.FromName(lines[8]);

                description_title.ForeColor = System.Drawing.Color.FromName(lines[10]);
                description.ForeColor = System.Drawing.Color.FromName(lines[10]);
            }
            else
            {
                try
                {
                    string[] new_lines = { "plt0 config v1.0", "Layout (change next line with one of these: \"All\", \"Auto\", \"Preview\", \"Paint\")", "All", "Window Location (\"Normal\", \"Maximized\", \"Bottom_left\", \"Left\", \"Top_left\", \"Top\", \"Top_right\", \"Right\", \"Bottom_right\", \"Bottom\")", "Maximized", "Textboxes text color (System.Drawing.Color)", "White", "Textboxes color", "Black", "Description color", "Cyan", "Font name (it must be installed on your system)", "default" };
                    Array.Resize(ref new_lines, 255);
                    for (byte i = 14; i < 255; i++)
                    {
                        new_lines[i - 1] = i.ToString();
                    }

                    System.IO.File.WriteAllLines(execPath + "images/settings.txt", new_lines);
                }
                catch
                {
                    // um, idk what to do here if the user doesn't let the app write a file.
                }
            }
            plt0.NativeMethods.FreeConsole();
        }

        /*private void Change_font_normal()
        {
            font_normal = new System.Drawing.Font(fontname, 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
        }
        private string Parse_Special_Markdown(int j, string[] text_array, byte i, byte arg)
        {
            if (j == -1)
                return text;
            markdown[arg] = "";
            separator = ' ';
            y = j;
            j += 3;
            if (text_array[i][j] == '"')
            {
                j++;
                separator = '"';
            }
            while (text_array[i][j] != separator)
            {
                markdown[arg] += text_array[i][j];
                j++;
            }
            j++;
            text_array[i] = text_array[i].Substring(0, y) + text_array[i].Substring(j);
            return text_array[i];
        }
        */
        // https://stackoverflow.com/questions/616584/how-do-i-get-the-name-of-the-current-executable-in-c
        private string Parse_Special_Markdown(int i, string text, byte arg)
        {
            markdown[arg] = "";
            if (i == -1)
                return text;
            separator = ' ';
            y = i;
            i += 3;
            if (text[i] == '"')
            {
                i++;
                separator = '"';
            }
            while (text[i] != separator)
            {
                markdown[arg] += text[i];
                i++;
            }
            i++;
            text = text.Substring(0, y) + text.Substring(i);
            return text;
        }
        private void Parse_Markdown(string txt)
        {
            // these are variables. easy to replace
            txt = txt.Replace("\\a", appdata).Replace("\\e", execName).Replace("\\h", this.Height.ToString()).Replace("\\l", layout_name[layout]).Replace("\\m", mipmaps.ToString()).Replace("\\n", "\n").Replace("\\o", output_name).Replace("\\p", execPath).Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\w", this.Width.ToString()).Replace("\\0", block_width_array[encoding].ToString()).Replace("\\y", block_height_array[encoding].ToString()).Replace("\\z", block_depth_array[encoding].ToString());
            if (input_file_image != null)
                txt = txt.Replace("\\d", input_file_image.PixelFormat.ToString());
            else
                txt = txt.Replace("\\d", "");
            // implement b, c, f, g, i, j, k, q, s, u, v, x
            string[] txt_label = txt.Split(new string[] { "\\j" }, StringSplitOptions.RemoveEmptyEntries);
            for (byte i = 0; i < (byte)txt_label.Length; i++)
            {
                font_colour = lines[10];  // default colour
                font_name = lines[12]; // default font name
                font_style = 0;
                font_unit = "point";
                size_font = 15F;
                font_encoding = "128";
                font_size = "15";
                byte.TryParse(lines[14], out GdiCharSet);
                desc[i].Visible = true;
                if (txt_label[i].Contains("\\b"))
                {
                    font_style += 1;
                }
                if (txt_label[i].Contains("\\i"))
                {
                    font_style += 2;
                }
                if (txt_label[i].Contains("\\k"))
                {
                    font_style += 4;
                }
                if (txt_label[i].Contains("\\u"))
                {
                    font_style += 8;
                }
                if (txt_label[i].Contains("\\v"))
                {
                    vertical = false;
                }
                txt_label[i] = Parse_Special_Markdown(txt_label[i].LastIndexOf("\\f"), txt_label[i], 0);
                txt_label[i] = Parse_Special_Markdown(txt_label[i].LastIndexOf("\\c"), txt_label[i], 1);
                txt_label[i] = Parse_Special_Markdown(txt_label[i].LastIndexOf("\\q"), txt_label[i], 2);
                txt_label[i] = Parse_Special_Markdown(txt_label[i].LastIndexOf("\\g"), txt_label[i], 3);
                txt_label[i] = Parse_Special_Markdown(txt_label[i].LastIndexOf("\\s"), txt_label[i], 4);
                if (markdown[0] != "")
                    font_name = markdown[0];
                if (markdown[1] != "")
                    font_colour = markdown[1];
                if (markdown[2] != "")
                    font_unit = markdown[2];
                if (markdown[3] != "")
                    font_encoding = markdown[3];
                if (markdown[4] != "")
                    font_size = markdown[4];
                if (font_encoding != "")
                    byte.TryParse(font_encoding, out GdiCharSet);
                if (font_size != "")
                    float.TryParse(font_size, out size_font);
                if (size_font == 0F)
                    size_font = 0.000001F;
                switch (font_unit.ToLower())
                {
                    case "world":
                        unit_font = GraphicsUnit.World;
                        break;
                    case "pixel":
                        unit_font = GraphicsUnit.Pixel;
                        break;
                    case "point":
                        unit_font = GraphicsUnit.Point;
                        break;
                    case "inch":
                        unit_font = GraphicsUnit.Inch;
                        break;
                    case "document":
                        unit_font = GraphicsUnit.Document;
                        break;
                    case "display":
                        unit_font = GraphicsUnit.Display;
                        break;
                    case "milimeter":
                        unit_font = GraphicsUnit.Display;
                        break;
                    default:  // I were to always suppose the setting file's markdown would never have errors so I haven't made any hard check. It's not worth securing everything since it's just visual and I'm doing this for free.
                        unit_font = GraphicsUnit.Point;
                        break;

                }
                // txt_label[i] = Parse_Special_Markdown(txt_label[i].LastIndexOf("\\j"), txt_label[i], backslash_j);  OH GOSH I AM IDIOT - that happens when you leave your code for one day lol
                y = 0;
                y = txt_label[i].IndexOf("\\x");
                while (y != -1)
                {
                    byte.TryParse(txt_label[i].Substring(y + 2, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out byte_text);
                    txt_label[i] = txt_label[i].Substring(0, y) + System.Text.Encoding.Unicode.GetString(new[] { byte_text }) + txt_label[i].Substring(y + 4);
                    y = txt_label[i].IndexOf("\\x");
                }
                txt_label[i] = txt_label[i].Replace("\\b", "").Replace("\\i", "").Replace("\\k", "").Replace("\\u", "").Replace("\\v", "");
                // desc[i].Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, ((System.Drawing.FontStyle)((((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) | System.Drawing.FontStyle.Underline) | System.Drawing.FontStyle.Strikeout))), System.Drawing.GraphicsUnit.World, ((byte)(0)), true);
                switch (font_style)
                {
                    case 0:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Regular, unit_font, GdiCharSet, vertical);
                        break;
                    case 1:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Bold, unit_font, GdiCharSet, vertical);
                        break;
                    case 2:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Italic, unit_font, GdiCharSet, vertical);
                        break;
                    case 3:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Bold | FontStyle.Italic, unit_font, GdiCharSet, vertical);
                        break;
                    case 4:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Strikeout, unit_font, GdiCharSet, vertical);
                        break;
                    case 5:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Strikeout | FontStyle.Bold, unit_font, GdiCharSet, vertical);
                        break;
                    case 6:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Strikeout | FontStyle.Italic, unit_font, GdiCharSet, vertical);
                        break;
                    case 7:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Strikeout | FontStyle.Italic | FontStyle.Bold, unit_font, GdiCharSet, vertical);
                        break;
                    case 8:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline, unit_font, GdiCharSet, vertical);
                        break;
                    case 9:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Bold, unit_font, GdiCharSet, vertical);
                        break;
                    case 10:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Italic, unit_font, GdiCharSet, vertical);
                        break;
                    case 11:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Bold | FontStyle.Italic, unit_font, GdiCharSet, vertical);
                        break;
                    case 12:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Strikeout, unit_font, GdiCharSet, vertical);
                        break;
                    case 13:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Strikeout | FontStyle.Bold, unit_font, GdiCharSet, vertical);
                        break;
                    case 14:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Strikeout | FontStyle.Italic, unit_font, GdiCharSet, vertical);
                        break;
                    case 15:
                        desc[i].Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Strikeout | FontStyle.Italic | FontStyle.Bold, unit_font, GdiCharSet, vertical);
                        break;
                }
                desc[i].ForeColor = Color.FromName(font_colour);
                desc[i].Text = txt_label[i];
            }
            for (byte i = (byte)txt_label.Length; i < 9; i++)
            {
                desc[i].Visible = false;
            }
        }
        private void Hide_description()
        {
            description.Text = "";
            desc2.Text = "";
            desc3.Text = "";
            desc4.Text = "";
            desc5.Text = "";
            desc6.Text = "";
            desc7.Text = "";
            desc8.Text = "";
            desc9.Text = "";
        }
        private void Parse_byte_text(TextBox txt, out byte output, byte max)
        {
            output = 254;
            if (txt.Text == "")
                return;
            success = false;
            len = txt.Text.Length;
            if (txt.Text[0] == '0' && len > 1)
            {
                txt.Text = txt.Text.Substring(1);
                return;
            }
            //if (ishexbyte(txt.Text.Substring(2, len - 2).ToLower()))
            if (len > 1)
            {
                if (txt.Text.Substring(0, 2) == "0x")
                {
                    if (len == 2)
                        return;
                    else
                        success = int.TryParse(txt.Text.Substring(2, len - 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out number);
                }
            }
            if (!success)
            {
                if (ishexchar(txt.Text[0]))
                    success = int.TryParse(txt.Text.Substring(0, len), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out number);
                else
                    success = int.TryParse(txt.Text, out number);
            }
            if (success)
            {
                if (number > max)
                {
                    output = max;
                    txt.Text = max.ToString();
                }
                else
                    output = (byte)number;
            }
            else
                txt.Text = txt.Text.Substring(0, len - 1);
        }
        private void Parse_ushort_text(TextBox txt, out ushort output, ushort max)
        {
            output = 0;
            if (txt.Text == "")
                return;
            success = false;
            len = txt.Text.Length;
            if (txt.Text[0] == '0' && len > 1)
            {
                txt.Text = txt.Text.Substring(1);
                return;
            }
            //if (ishexbyte(txt.Text.Substring(2, len - 2).ToLower()))
            if (len > 1)
            {
                if (txt.Text.Substring(0, 2) == "0x")
                {
                    if (len == 2)
                        return;
                    else
                        success = int.TryParse(txt.Text.Substring(2, len - 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out number);
                }
            }
            if (!success)
            {
                if (ishexchar(txt.Text[0]))
                    success = int.TryParse(txt.Text.Substring(0, len), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out number);
                else
                    success = int.TryParse(txt.Text, out number);
            }
            if (success)
            {
                if (number > max)
                {
                    output = max;
                    txt.Text = max.ToString();
                }
                else
                    output = (ushort)number;
            }
            else
                txt.Text = txt.Text.Substring(0, len - 1);
        }
        private void Parse_double_text(TextBox txt, out double output, double max)
        {
            output = 69420F;
            if (txt.Text == "")
                return;
            len = txt.Text.Length;
            if (txt.Text[0] == '0')
                if (len > 1)
                    if (txt.Text[1] != '.')
                    {
                        txt.Text = txt.Text.Substring(1);
                        return;
                    }
            success = false;
            //if (ishexbyte(txt.Text.Substring(2, len - 2).ToLower()))
            success = double.TryParse(txt.Text, out output);
            if (success)
            {
                if (output > max)
                {
                    output = max;
                    txt.Text = max.ToString();
                }
            }
            else
                txt.Text = txt.Text.Substring(0, len - 1);
        }
        private void Check_run()
        {
            if (bmd)
            {
                if (!File.Exists(input_file2))
                {
                    run_ck.BackgroundImage = run_off;
                    return;
                }
            }
            if (File.Exists(input_file) && encoding != 7)
            {
                if (bmd || bti || tex0 || tpl || bmp || png || jpeg || gif || ico || tif || tiff)
                    run_ck.BackgroundImage = run_on;
                else
                    run_ck.BackgroundImage = run_off;
            }
            else
                run_ck.BackgroundImage = run_off;
        }
        private void Organize_args()
        {
            arg_array.Clear();
            // args = "gui ";
            arg_array.Add("gui");
            args = "";
            if (File.Exists(input_file))
            {
                args += "\"" + input_file + "\" ";
                arg_array.Add(input_file);
            }
            if (File.Exists(input_file2))
            {
                args += "\"" + input_file2 + "\" ";
                arg_array.Add(input_file2);
            }
            if (output_name != "")
            {
                args += "\"" + output_name + "\" ";
                arg_array.Add(output_name);
            }
            if (encoding != 7)
            {
                args += encoding_array[encoding] + " ";
                arg_array.Add(encoding_array[encoding]);
            }
            if (palette_enc != 3)
            {
                args += encoding_array[palette_enc + 3] + " ";
                arg_array.Add(encoding_array[palette_enc + 3]);
            }
            if (WrapS != 3 || WrapT != 3)
            {
                args += "wrap " + wrap_array[WrapS] + " " + wrap_array[WrapT] + " ";
                arg_array.Add("wrap");
                arg_array.Add(wrap_array[WrapS]);
                arg_array.Add(wrap_array[WrapT]);
            }
            if (algorithm != 4 && algorithm != 0)
            {
                args += algorithm_array[algorithm] + " ";
                arg_array.Add(algorithm_array[algorithm]);
            }
            if (algorithm == 2)  // custom rgba
            {
                args += custom_r.ToString() + " " + custom_g.ToString() + " " + custom_b.ToString() + " " + custom_a.ToString() + " ";
                arg_array.Add(custom_r.ToString());
                arg_array.Add(custom_g.ToString());
                arg_array.Add(custom_b.ToString());
                arg_array.Add(custom_a.ToString());
            }
            if (alpha != 3)
            {
                args += alpha_array[alpha] + " ";
                arg_array.Add(alpha_array[alpha]);
            }
            if (r != 0 || g != 1 || b != 2 || a != 3)
            {
                args += rgba_array[r] + rgba_array[g] + rgba_array[b] + rgba_array[a] + " ";
                arg_array.Add(rgba_array[r] + rgba_array[g] + rgba_array[b] + rgba_array[a]);
            }
            if (minification_filter != 6)
            {
                args += "min " + minification_filter.ToString() + " ";
                arg_array.Add("min");
                arg_array.Add(minification_filter.ToString());
            }
            if (magnification_filter != 6)
            {
                args += "mag " + magnification_filter.ToString() + " ";
                arg_array.Add("mag");
                arg_array.Add(magnification_filter.ToString());
            }
            if (cmpr_max < 16)
            {
                args += "max " + cmpr_max.ToString() + " ";
                arg_array.Add("max");
                arg_array.Add(cmpr_max.ToString());
            }
            if (cmpr_min_alpha != 100)
            {
                args += "cmpr_min " + cmpr_min_alpha.ToString() + " ";
                arg_array.Add("cmpr_min");
                arg_array.Add(cmpr_min_alpha.ToString());
            }
            if (num_colours != 0)
            {
                args += "c " + num_colours.ToString() + " ";
                arg_array.Add("c");
                arg_array.Add(num_colours.ToString());
            }
            if (diversity != 10)
            {
                args += "d " + diversity.ToString() + " ";
                arg_array.Add("d");
                arg_array.Add(diversity.ToString());
            }
            if (diversity2 != 0)
            {
                args += "d2 " + diversity2.ToString() + " ";
                arg_array.Add("d2");
                arg_array.Add(diversity2.ToString());
            }
            if (mipmaps != 0)
            {
                args += "m " + mipmaps.ToString() + " ";
                arg_array.Add("m");
                arg_array.Add(mipmaps.ToString());
            }
            if (percentage != 0 && percentage < 128)
            {
                args += "p " + percentage.ToString() + " ";
                arg_array.Add("p");
                arg_array.Add(percentage.ToString());
            }
            if (percentage2 != 0 && percentage2 < 128)
            {
                args += "p2 " + percentage2.ToString() + " ";
                arg_array.Add("p2");
                arg_array.Add(percentage2.ToString());
            }
            if (round3 != 15 && round3 < 128)
            {
                args += "round3 " + round3.ToString() + " ";
                arg_array.Add("round3");
                arg_array.Add(round3.ToString());
            }
            if (round4 != 7 && round4 < 128)
            {
                args += "round4 " + round4.ToString() + " ";
                arg_array.Add("round4");
                arg_array.Add(round4.ToString());
            }
            if (round5 != 3 && round5 < 128)
            {
                args += "round5 " + round5.ToString() + " ";
                arg_array.Add("round5");
                arg_array.Add(round5.ToString());
            }
            if (round6 != 1 && round6 < 128)
            {
                args += "round6 " + round6.ToString() + " ";
                arg_array.Add("round6");
                arg_array.Add(round6.ToString());
            }
            if (bmd)
            {
                args += "bmd ";
                arg_array.Add("bmd");
            }
            if (bti)
            {
                args += "bti ";
                arg_array.Add("bti");
            }
            if (tex0)
            {
                args += "tex0 ";
                arg_array.Add("tex0");
            }
            if (tpl)
            {
                args += "tpl ";
                arg_array.Add("tpl");
            }
            if (bmp)
            {
                args += "bmp ";
                arg_array.Add("bmp");
            }
            if (png)
            {
                args += "png ";
                arg_array.Add("png");
            }
            if (jpg)
            {
                args += "jpg ";
                arg_array.Add("jpg");
            }
            if (jpeg)
            {
                args += "jpeg ";
                arg_array.Add("jpeg");
            }
            if (gif)
            {
                args += "gif ";
                arg_array.Add("gif");
            }
            if (ico)
            {
                args += "ico ";
                arg_array.Add("ico");
            }
            if (tif)
            {
                args += "tif ";
                arg_array.Add("tif");
            }
            if (tiff)
            {
                args += "tiff ";
                arg_array.Add("tiff");
            }
            if (ask_exit)
            {
                args += "ask_exit ";
                arg_array.Add("ask_exit");
            }
            if (bmp_32)
            {
                args += "bmp_32 ";
                arg_array.Add("bmp_32");
            }
            if (FORCE_ALPHA)
            {
                args += "FORCE_ALPHA ";
                arg_array.Add("FORCE_ALPHA");
            }
            if (funky)
            {
                args += "funky ";
                arg_array.Add("funky");
            }
            if (no_warning)
            {
                args += "no_warning ";
                arg_array.Add("no_warning");
            }
            if (random)
            {
                args += "random ";
                arg_array.Add("random");
            }
            if (reverse)
            {
                args += "reverse ";
                arg_array.Add("reverse");
            }
            if (safe_mode)
            {
                args += "safe_mode ";
                arg_array.Add("safe_mode");
            }
            if (stfu)
            {
                args += "stfu ";
                arg_array.Add("stfu");
            }
            if (warn)
            {
                args += "warn ";
                arg_array.Add("warn");
            }
            if (args.Length > 70)
            {
                cli_textbox_label.Location = cli_textbox_location;
                cli_textbox_label.Padding = cli_textbox_padding;
            }
            cli_textbox_label.Text = args;
        }
        private void plt0_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        private void plt0_DragDrop(object sender, DragEventArgs e)
        {

            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);  // gets all the files or folders name that were dragged in an array, but I'll only use the first
            if (file != null) // prevent crashes if it's for example a google chrome favourite that was dragged
            {
                bool isFolder = System.IO.File.GetAttributes(file[0]).HasFlag(System.IO.FileAttributes.Directory);
                if (isFolder)
                {
                    //byte len = (byte)file[0].Split('\\').Length;
                    //Output.Text += file[0].Split('\\')[len - 1] + " over Mario\n";  // file is actually a full path starting from a drive, but I won't clutter the display
                    //Replace_Character(file[0], "pc01_mario");  // file[0] is the folder name of the mod the user wants to be over mario
                }
                else
                {
                    //Output.Text += "only folders are accepted\n";
                    return;
                }
            }
            else
            {
                //Output.Text += e.Data.GetData(DataFormats.Text) + "\n";
            }
        }/*
        private bool ishexbyte(string txt)
        {
            if (txt.Length > 2)
                return false;
            for (byte i = 0; i < txt.Length; i++)
            {
                switch (txt[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                        // that's a correct hex char
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }
        private bool ishexdouble(string txt)
        {
            if (txt.Length > 8)
                return false;
            for (byte i = 0; i < txt.Length; i++)
            {
                switch (txt[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                        // that's a correct hex char
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }*/
        private bool ishexchar(char txt)
        {
            switch (txt)
            {
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    // that's a correct hex char
                    break;
                default:
                    return false;
            }

            return true;
        }
        private void unchecked_checkbox(PictureBox checkbox)
        {
            checkbox.BackgroundImage = white_box;
        }
        private void checked_checkbox(PictureBox checkbox)
        {
            checkbox.BackgroundImage = check;
        }
        private void hover_checkbox(PictureBox checkbox)
        {
            checkbox.BackgroundImage = light_blue_box;
        }
        private void selected_checkbox(PictureBox checkbox)
        {
            checkbox.BackgroundImage = light_blue_check;
        }
        // implementation of radio buttons
        private void unchecked_encoding(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = violet_circle;
        }
        private void checked_encoding(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = violet_circle_on;
        }
        private void hover_encoding(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle;
        }
        private void selected_encoding(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle_on;
        }
        private void unchecked_A(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = a_off;
        }
        private void checked_A(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = a_on;
        }
        private void hover_A(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = a_hover;
        }
        private void selected_A(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = a_selected;
        }
        private void unchecked_B(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = b_off;
        }
        private void checked_B(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = b_on;
        }
        private void hover_B(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = b_hover;
        }
        private void selected_B(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = b_selected;
        }
        private void unchecked_G(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = g_off;
        }
        private void checked_G(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = g_on;
        }
        private void hover_G(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = g_hover;
        }
        private void selected_G(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = g_selected;
        }
        private void unchecked_R(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = r_off;
        }
        private void checked_R(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = r_on;
        }
        private void hover_R(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = r_hover;
        }
        private void selected_R(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = r_selected;
        }
        private void unchecked_Magnification(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = red_circle;
        }
        private void checked_Magnification(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = red_circle_on;
        }
        private void hover_Magnification(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = light_red_circle;
        }
        private void selected_Magnification(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = light_red_circle_on;
        }
        private void unchecked_Minification(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = blue_circle;
        }
        private void checked_Minification(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = blue_circle_on;
        }
        private void hover_Minification(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = light_blue_circle;
        }
        private void selected_Minification(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = light_blue_circle_on;
        }
        private void unchecked_WrapT(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = green_circle;
        }
        private void checked_WrapT(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = green_circle_on;
        }
        private void hover_WrapT(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = white_circle;
        }
        private void selected_WrapT(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = white_circle_on;
        }
        private void unchecked_WrapS(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = orange_circle;
        }
        private void checked_WrapS(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = orange_circle_on;
        }
        private void hover_WrapS(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = yellow_circle;
        }
        private void selected_WrapS(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = yellow_circle_on;
        }
        private void unchecked_alpha(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = fushia_circle;
        }
        private void checked_alpha(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = fushia_circle_on;
        }
        private void hover_alpha(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = cyan_circle;
        }
        private void selected_alpha(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = cyan_circle_on;
        }
        private void unchecked_algorithm(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = cherry_circle;
        }
        private void checked_algorithm(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = cherry_circle_on;
        }
        private void hover_algorithm(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = purple_circle;
        }
        private void selected_algorithm(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = purple_circle_on;
        }
        private void Category_selected(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = light_blue_circle_on;
        }
        private void Category_hover(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = light_blue_circle;
        }
        private void Category_checked(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = white_circle_on;
        }
        private void Category_unchecked(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = white_circle;
        }
        private void unchecked_palette(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = chartreuse_circle;
        }
        private void checked_palette(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = chartreuse_circle_on;
        }
        private void hover_palette(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = cherry_circle;
        }
        private void selected_palette(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = cherry_circle_on;
        }
        private void View_alpha()
        {
            if (layout == 0)
                return;
            for (byte i = 0; i < alpha_ck_array.Count; i++)
            {
                alpha_ck_array[i].Visible = true;
            }
            no_alpha_hitbox.Visible = true;
            no_alpha_label.Visible = true;
            alpha_hitbox.Visible = true;
            alpha_label.Visible = true;
            alpha_title.Visible = true;
            mix_hitbox.Visible = true;
            mix_label.Visible = true;
            view_alpha = true;
        }
        private void Hide_alpha()
        {
            if (layout == 0)
                return;
            //System.Windows.Forms.FormWindowState previous = this.WindowState;
            //this.WindowState = FormWindowState.Minimized;
            for (byte i = 0; i < alpha_ck_array.Count; i++)
            {
                alpha_ck_array[i].Visible = false;
            }
            no_alpha_hitbox.Visible = false;
            alpha_hitbox.Visible = false;
            mix_hitbox.Visible = false;
            alpha_label.Visible = false;
            no_alpha_label.Visible = false;
            alpha_title.Visible = false;
            mix_label.Visible = false;
            view_alpha = false;
            //this.WindowState = FormWindowState.Normal;
        }
        private void View_algorithm(byte algorithm)
        {
            if (layout == 0)
                return;
            for (byte i = 0; i < algorithm_ck.Count; i++)
            {
                algorithm_ck[i].Visible = true;
            }
            cie_601_hitbox.Visible = true;
            cie_601_label.Visible = true;
            cie_709_hitbox.Visible = true;
            cie_709_label.Visible = true;
            custom_hitbox.Visible = true;
            custom_label.Visible = true;
            no_gradient_hitbox.Visible = true;
            no_gradient_label.Visible = true;
            algorithm_label.Visible = true;
            view_algorithm = true;
        }
        private void Hide_algorithm(byte algorithm)
        {
            if (layout == 0)
                return;
            for (byte i = 0; i < algorithm_ck.Count; i++)
            {
                algorithm_ck[i].Visible = false;
            }
            cie_601_hitbox.Visible = false;
            cie_709_hitbox.Visible = false;
            custom_hitbox.Visible = false;
            no_gradient_hitbox.Visible = false;
            no_gradient_label.Visible = false;
            algorithm_label.Visible = false;
            cie_601_label.Visible = false;
            cie_709_label.Visible = false;
            custom_label.Visible = false;
            view_algorithm = false;
        }
        private void View_options()
        {
            if (layout == 0)
                return;
            ask_exit_hitbox.Visible = true;
            warn_hitbox.Visible = true;
            no_warning_hitbox.Visible = true;
            stfu_hitbox.Visible = true;
            safe_mode_hitbox.Visible = true;
            ask_exit_label.Visible = true;
            warn_label.Visible = true;
            no_warning_label.Visible = true;
            stfu_label.Visible = true;
            safe_mode_label.Visible = true;
            ask_exit_ck.Visible = true;
            warn_ck.Visible = true;
            no_warning_ck.Visible = true;
            stfu_ck.Visible = true;
            safe_mode_ck.Visible = true;
            view_options = true;
        }
        private void Hide_options()
        {
            if (layout == 0)
                return;
            ask_exit_hitbox.Visible = false;
            warn_hitbox.Visible = false;
            no_warning_hitbox.Visible = false;
            stfu_hitbox.Visible = false;
            safe_mode_hitbox.Visible = false;
            ask_exit_label.Visible = false;
            warn_label.Visible = false;
            no_warning_label.Visible = false;
            stfu_label.Visible = false;
            safe_mode_label.Visible = false;
            ask_exit_ck.Visible = false;
            warn_ck.Visible = false;
            no_warning_ck.Visible = false;
            stfu_ck.Visible = false;
            safe_mode_ck.Visible = false;
            view_options = false;
        }
        private void View_WrapS()
        {
            if (layout == 0)
                return;
            for (byte i = 0; i < WrapS_ck.Count; i++)
            {
                WrapS_ck[i].Visible = true;
            }
            Sclamp_hitbox.Visible = true;
            Srepeat_hitbox.Visible = true;
            Smirror_hitbox.Visible = true;
            Sclamp_label.Visible = true;
            Srepeat_label.Visible = true;
            Smirror_label.Visible = true;
            WrapS_label.Visible = true;
            view_WrapS = true;
        }
        private void Hide_WrapS()
        {
            if (layout == 0)
                return;
            for (byte i = 0; i < WrapS_ck.Count; i++)
            {
                WrapS_ck[i].Visible = false;
            }
            Sclamp_hitbox.Visible = false;
            Srepeat_hitbox.Visible = false;
            Smirror_hitbox.Visible = false;
            Sclamp_label.Visible = false;
            Srepeat_label.Visible = false;
            Smirror_label.Visible = false;
            WrapS_label.Visible = false;
            view_WrapS = false;
        }
        private void View_WrapT()
        {
            if (layout == 0)
                return;
            for (byte i = 0; i < WrapT_ck.Count; i++)
            {
                WrapT_ck[i].Visible = true;
            }
            Tclamp_hitbox.Visible = true;
            Trepeat_hitbox.Visible = true;
            Tmirror_hitbox.Visible = true;
            Tclamp_label.Visible = true;
            Trepeat_label.Visible = true;
            Tmirror_label.Visible = true;
            WrapT_label.Visible = true;
            view_WrapT = true;
        }
        private void Hide_WrapT()
        {
            if (layout == 0)
                return;
            for (byte i = 0; i < WrapT_ck.Count; i++)
            {
                WrapT_ck[i].Visible = false;
            }
            Tclamp_hitbox.Visible = false;
            Trepeat_hitbox.Visible = false;
            Tmirror_hitbox.Visible = false;
            Tclamp_label.Visible = false;
            Trepeat_label.Visible = false;
            Tmirror_label.Visible = false;
            WrapT_label.Visible = false;
            view_WrapT = false;
        }
        private void View_min()
        {
            if (layout == 0)
                return;
            for (byte i = 0; i < minification_ck.Count; i++)
            {
                minification_ck[i].Visible = true;
            }
            min_linearmipmaplinear_hitbox.Visible = true;
            min_linearmipmapnearest_hitbox.Visible = true;
            min_linear_hitbox.Visible = true;
            min_nearestmipmaplinear_hitbox.Visible = true;
            min_nearestmipmapnearest_hitbox.Visible = true;
            min_nearest_neighbour_hitbox.Visible = true;
            min_linearmipmaplinear_label.Visible = true;
            min_linearmipmapnearest_label.Visible = true;
            min_linear_label.Visible = true;
            min_nearestmipmaplinear_label.Visible = true;
            min_nearestmipmapnearest_label.Visible = true;
            min_nearest_neighbour_label.Visible = true;
            minification_label.Visible = true;
            view_min = true;
        }
        private void Hide_min()
        {
            if (layout == 0)
                return;
            for (byte i = 0; i < minification_ck.Count; i++)
            {
                minification_ck[i].Visible = false;
            }
            min_linearmipmaplinear_hitbox.Visible = false;
            min_linearmipmapnearest_hitbox.Visible = false;
            min_linear_hitbox.Visible = false;
            min_nearestmipmaplinear_hitbox.Visible = false;
            min_nearestmipmapnearest_hitbox.Visible = false;
            min_nearest_neighbour_hitbox.Visible = false;
            min_linearmipmaplinear_label.Visible = false;
            min_linearmipmapnearest_label.Visible = false;
            min_linear_label.Visible = false;
            min_nearestmipmaplinear_label.Visible = false;
            min_nearestmipmapnearest_label.Visible = false;
            min_nearest_neighbour_label.Visible = false;
            minification_label.Visible = false;
            view_min = false;
        }
        private void View_mag()
        {
            if (layout == 0)
                return;
            for (byte i = 0; i < magnification_ck.Count; i++)
            {
                magnification_ck[i].Visible = true;
            }
            mag_linearmipmaplinear_hitbox.Visible = true;
            mag_linearmipmapnearest_hitbox.Visible = true;
            mag_linear_hitbox.Visible = true;
            mag_nearestmipmaplinear_hitbox.Visible = true;
            mag_nearestmipmapnearest_hitbox.Visible = true;
            mag_nearest_neighbour_hitbox.Visible = true;
            mag_linearmipmaplinear_label.Visible = true;
            mag_linearmipmapnearest_label.Visible = true;
            mag_linear_label.Visible = true;
            mag_nearestmipmaplinear_label.Visible = true;
            mag_nearestmipmapnearest_label.Visible = true;
            mag_nearest_neighbour_label.Visible = true;
            magnification_label.Visible = true;
            view_mag = true;
        }
        private void Hide_mag()
        {
            // this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            if (layout == 0)
                return;
            for (byte i = 0; i < magnification_ck.Count; i++)
            {
                magnification_ck[i].Visible = false;
            }
            mag_linearmipmaplinear_hitbox.Visible = false;
            mag_linearmipmapnearest_hitbox.Visible = false;
            mag_linear_hitbox.Visible = false;
            mag_nearestmipmaplinear_hitbox.Visible = false;
            mag_nearestmipmapnearest_hitbox.Visible = false;
            mag_nearest_neighbour_hitbox.Visible = false;
            mag_linearmipmaplinear_label.Visible = false;
            mag_linearmipmapnearest_label.Visible = false;
            mag_linear_label.Visible = false;
            mag_nearestmipmaplinear_label.Visible = false;
            mag_nearestmipmapnearest_label.Visible = false;
            mag_nearest_neighbour_label.Visible = false;
            magnification_label.Visible = false;
            view_mag = false;
        }
        private void View_cmpr()
        {
            if (layout == 0)
                return;
            no_gradient_ck.Visible = true;
            no_gradient_hitbox.Visible = true;
            no_gradient_label.Visible = true;
            cmpr_max_hitbox.Visible = true;
            cmpr_max_label.Visible = true;
            cmpr_max_txt.Visible = true;
            cmpr_min_alpha_hitbox.Visible = true;
            cmpr_min_alpha_label.Visible = true;
            cmpr_min_alpha_txt.Visible = true;
            diversity_hitbox.Visible = true;
            diversity_label.Visible = true;
            diversity_txt.Visible = true;
            diversity2_hitbox.Visible = true;
            diversity2_label.Visible = true;
            diversity2_txt.Visible = true;
            percentage2_hitbox.Visible = true;
            percentage2_label.Visible = true;
            percentage2_txt.Visible = true;
            percentage_hitbox.Visible = true;
            percentage_label.Visible = true;
            percentage_txt.Visible = true;
            round5_hitbox.Visible = true;
            round5_label.Visible = true;
            round5_txt.Visible = true;
            round6_hitbox.Visible = true;
            round6_label.Visible = true;
            round6_txt.Visible = true;
            view_cmpr = true;
        }
        private void Hide_cmpr()
        {
            if (layout == 0)
                return;
            no_gradient_ck.Visible = false;
            no_gradient_hitbox.Visible = false;
            no_gradient_label.Visible = false;
            cmpr_max_hitbox.Visible = false;
            cmpr_max_label.Visible = false;
            cmpr_max_txt.Visible = false;
            cmpr_min_alpha_hitbox.Visible = false;
            cmpr_min_alpha_label.Visible = false;
            cmpr_min_alpha_txt.Visible = false;
            diversity_hitbox.Visible = false;
            diversity_label.Visible = false;
            diversity_txt.Visible = false;
            diversity2_hitbox.Visible = false;
            diversity2_label.Visible = false;
            diversity2_txt.Visible = false;
            percentage2_hitbox.Visible = false;
            percentage2_label.Visible = false;
            percentage2_txt.Visible = false;
            percentage_hitbox.Visible = false;
            percentage_label.Visible = false;
            percentage_txt.Visible = false;
            round5_hitbox.Visible = false;
            round5_label.Visible = false;
            round5_txt.Visible = false;
            round6_hitbox.Visible = false;
            round6_label.Visible = false;
            round6_txt.Visible = false;
            view_cmpr = false;
        }
        private void View_palette()
        {
            if (layout == 0)
                return;
            palette_label.Visible = true;
            palette_ai8_hitbox.Visible = true;
            palette_ai8_label.Visible = true;
            palette_ai8_ck.Visible = true;
            palette_rgb565_ck.Visible = true;
            palette_rgb565_hitbox.Visible = true;
            palette_rgb565_label.Visible = true;
            palette_rgb5a3_ck.Visible = true;
            palette_rgb5a3_label.Visible = true;
            palette_rgb5a3_hitbox.Visible = true;
            num_colours_hitbox.Visible = true;
            num_colours_label.Visible = true;
            num_colours_txt.Visible = true;
            view_palette = true;
        }
        private void Hide_palette()
        {
            if (layout == 0)
                return;
            palette_label.Visible = false;
            palette_ai8_hitbox.Visible = false;
            palette_ai8_label.Visible = false;
            palette_ai8_ck.Visible = false;
            palette_rgb565_ck.Visible = false;
            palette_rgb565_hitbox.Visible = false;
            palette_rgb565_label.Visible = false;
            palette_rgb5a3_ck.Visible = false;
            palette_rgb5a3_label.Visible = false;
            palette_rgb5a3_hitbox.Visible = false;
            num_colours_hitbox.Visible = false;
            num_colours_label.Visible = false;
            num_colours_txt.Visible = false;
            view_palette = false;
        }
        private void View_rgba()
        {
            if (layout == 0)
                return;
            custom_rgba_label.Visible = true;
            custom_r_hitbox.Visible = true;
            custom_r_label.Visible = true;
            custom_r_txt.Visible = true;
            custom_g_label.Visible = true;
            custom_g_hitbox.Visible = true;
            custom_g_txt.Visible = true;
            custom_b_txt.Visible = true;
            custom_b_hitbox.Visible = true;
            custom_b_label.Visible = true;
            view_rgba = true;
        }
        private void Hide_rgba()
        {
            if (layout == 0)
                return;
            custom_rgba_label.Visible = false;
            custom_r_hitbox.Visible = false;
            custom_r_label.Visible = false;
            custom_r_txt.Visible = false;
            custom_g_label.Visible = false;
            custom_g_hitbox.Visible = false;
            custom_g_txt.Visible = false;
            custom_b_txt.Visible = false;
            custom_b_hitbox.Visible = false;
            custom_b_label.Visible = false;
            view_rgba = false;
        }
        private void View_i4()
        {
            if (layout == 0)
                return;
            cie_601_ck.Visible = true;
            cie_601_hitbox.Visible = true;
            cie_601_label.Visible = true;
            cie_709_ck.Visible = true;
            cie_709_hitbox.Visible = true;
            cie_709_label.Visible = true;
            round4_hitbox.Visible = false;
            round4_label.Visible = false;
            round4_txt.Visible = false;
            //view_rgba = true;
        }
        private void View_i8()
        {
            if (layout == 0)
                return;
            cie_601_ck.Visible = true;
            cie_601_hitbox.Visible = true;
            cie_601_label.Visible = true;
            cie_709_ck.Visible = true;
            cie_709_hitbox.Visible = true;
            cie_709_label.Visible = true;
            //view_rgba = true;
        }
        private void View_ai4()
        {
            if (layout == 0)
                return;
            cie_601_ck.Visible = true;
            cie_601_hitbox.Visible = true;
            cie_601_label.Visible = true;
            cie_709_ck.Visible = true;
            cie_709_hitbox.Visible = true;
            cie_709_label.Visible = true;
            round4_hitbox.Visible = false;
            round4_label.Visible = false;
            round4_txt.Visible = false;
            //view_rgba = true;
        }
        private void View_ai8()
        {
            if (layout == 0)
                return;
            cie_601_ck.Visible = true;
            cie_601_hitbox.Visible = true;
            cie_601_label.Visible = true;
            cie_709_ck.Visible = true;
            cie_709_hitbox.Visible = true;
            cie_709_label.Visible = true;
            //view_rgba = true;
        }
        private void View_rgb565()
        {
            if (layout == 0)
                return;
            round5_hitbox.Visible = false;
            round5_label.Visible = false;
            round5_txt.Visible = false;
            round6_hitbox.Visible = false;
            round6_label.Visible = false;
            round6_txt.Visible = false;
            //view_rgba = true;
        }
        private void View_rgb5a3()
        {
            if (layout == 0)
                return;
            View_alpha();
            round3_hitbox.Visible = false;
            round3_label.Visible = false;
            round3_txt.Visible = false;
            round4_hitbox.Visible = false;
            round4_label.Visible = false;
            round4_txt.Visible = false;
            round5_hitbox.Visible = false;
            round5_label.Visible = false;
            round5_txt.Visible = false;
            round6_hitbox.Visible = false;
            round6_label.Visible = false;
            round6_txt.Visible = false;
            //view_rgba = true;
        }
        private void View_rgba32()
        {
            //view_rgba = true;
        }
        private void View_ci4()
        {
            View_palette();
            //view_rgba = true;
        }
        private void View_ci8()
        {
            View_palette();
            //view_rgba = true;
        }
        private void View_ci14x2()
        {
            View_palette();
            //view_rgba = true;
        }
        private void Hide_encoding(byte encoding)
        {
            if (layout == 0)
                return;
            custom_rgba_label.Visible = false;
            custom_r_hitbox.Visible = false;
            custom_r_label.Visible = false;
            custom_r_txt.Visible = false;
            custom_g_label.Visible = false;
            custom_g_hitbox.Visible = false;
            custom_g_txt.Visible = false;
            custom_b_txt.Visible = false;
            custom_b_hitbox.Visible = false;
            custom_b_label.Visible = false;
            view_rgba = false;
        }

        /* public FontFamily GetFontFamilyByName(string name)
        {
            return _privateFontCollection.Families.FirstOrDefault(x => x.Name == name);
        } 
        public string Get_font_name()
        {
            return _privateFontCollection.Families[0].Name;
        }
        public void AddFont(byte[] fontBytes)
        {
            var handle = System.Runtime.InteropServices.GCHandle.Alloc(fontBytes, System.Runtime.InteropServices.GCHandleType.Pinned);
            IntPtr pointer = handle.AddrOfPinnedObject();
            try
            {
                _privateFontCollection.AddMemoryFont(pointer, fontBytes.Length);
            }
            finally
            {
                handle.Free();
            }
        } */

        // the whole code below is generated by something else than me typing on my keyboard in Visual Studio
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(plt0_gui));
            this.output_file_type_label = new System.Windows.Forms.Label();
            this.mandatory_settings_label = new System.Windows.Forms.Label();
            this.bmd_label = new System.Windows.Forms.Label();
            this.bmd_hitbox = new System.Windows.Forms.Label();
            this.bmd_ck = new System.Windows.Forms.PictureBox();
            this.bti_ck = new System.Windows.Forms.PictureBox();
            this.bti_label = new System.Windows.Forms.Label();
            this.bti_hitbox = new System.Windows.Forms.Label();
            this.tex0_ck = new System.Windows.Forms.PictureBox();
            this.tex0_label = new System.Windows.Forms.Label();
            this.tex0_hitbox = new System.Windows.Forms.Label();
            this.tpl_ck = new System.Windows.Forms.PictureBox();
            this.tpl_label = new System.Windows.Forms.Label();
            this.tpl_hitbox = new System.Windows.Forms.Label();
            this.bmp_ck = new System.Windows.Forms.PictureBox();
            this.bmp_label = new System.Windows.Forms.Label();
            this.bmp_hitbox = new System.Windows.Forms.Label();
            this.png_ck = new System.Windows.Forms.PictureBox();
            this.png_label = new System.Windows.Forms.Label();
            this.png_hitbox = new System.Windows.Forms.Label();
            this.jpg_ck = new System.Windows.Forms.PictureBox();
            this.jpg_label = new System.Windows.Forms.Label();
            this.jpg_hitbox = new System.Windows.Forms.Label();
            this.tiff_ck = new System.Windows.Forms.PictureBox();
            this.tiff_label = new System.Windows.Forms.Label();
            this.tiff_hitbox = new System.Windows.Forms.Label();
            this.tif_ck = new System.Windows.Forms.PictureBox();
            this.tif_label = new System.Windows.Forms.Label();
            this.tif_hitbox = new System.Windows.Forms.Label();
            this.ico_ck = new System.Windows.Forms.PictureBox();
            this.ico_label = new System.Windows.Forms.Label();
            this.ico_hitbox = new System.Windows.Forms.Label();
            this.gif_ck = new System.Windows.Forms.PictureBox();
            this.gib_label = new System.Windows.Forms.Label();
            this.gif_hitbox = new System.Windows.Forms.Label();
            this.jpeg_ck = new System.Windows.Forms.PictureBox();
            this.jpeg_label = new System.Windows.Forms.Label();
            this.jpeg_hitbox = new System.Windows.Forms.Label();
            this.options_label = new System.Windows.Forms.Label();
            this.warn_ck = new System.Windows.Forms.PictureBox();
            this.warn_label = new System.Windows.Forms.Label();
            this.stfu_ck = new System.Windows.Forms.PictureBox();
            this.stfu_label = new System.Windows.Forms.Label();
            this.safe_mode_ck = new System.Windows.Forms.PictureBox();
            this.safe_mode_label = new System.Windows.Forms.Label();
            this.reverse_ck = new System.Windows.Forms.PictureBox();
            this.reverse_label = new System.Windows.Forms.Label();
            this.random_ck = new System.Windows.Forms.PictureBox();
            this.random_label = new System.Windows.Forms.Label();
            this.no_warning_ck = new System.Windows.Forms.PictureBox();
            this.no_warning_label = new System.Windows.Forms.Label();
            this.funky_ck = new System.Windows.Forms.PictureBox();
            this.funky_label = new System.Windows.Forms.Label();
            this.FORCE_ALPHA_ck = new System.Windows.Forms.PictureBox();
            this.FORCE_ALPHA_label = new System.Windows.Forms.Label();
            this.bmp_32_ck = new System.Windows.Forms.PictureBox();
            this.bmp_32_label = new System.Windows.Forms.Label();
            this.ask_exit_ck = new System.Windows.Forms.PictureBox();
            this.ask_exit_label = new System.Windows.Forms.Label();
            this.ask_exit_hitbox = new System.Windows.Forms.Label();
            this.bmp_32_hitbox = new System.Windows.Forms.Label();
            this.FORCE_ALPHA_hitbox = new System.Windows.Forms.Label();
            this.funky_hitbox = new System.Windows.Forms.Label();
            this.no_warning_hitbox = new System.Windows.Forms.Label();
            this.random_hitbox = new System.Windows.Forms.Label();
            this.reverse_hitbox = new System.Windows.Forms.Label();
            this.safe_mode_hitbox = new System.Windows.Forms.Label();
            this.stfu_hitbox = new System.Windows.Forms.Label();
            this.warn_hitbox = new System.Windows.Forms.Label();
            this.cmpr_ck = new System.Windows.Forms.PictureBox();
            this.cmpr_label = new System.Windows.Forms.Label();
            this.cmpr_hitbox = new System.Windows.Forms.Label();
            this.ci14x2_ck = new System.Windows.Forms.PictureBox();
            this.ci14x2_label = new System.Windows.Forms.Label();
            this.ci14x2_hitbox = new System.Windows.Forms.Label();
            this.ci8_ck = new System.Windows.Forms.PictureBox();
            this.ci8_label = new System.Windows.Forms.Label();
            this.ci8_hitbox = new System.Windows.Forms.Label();
            this.ci4_ck = new System.Windows.Forms.PictureBox();
            this.ci4_label = new System.Windows.Forms.Label();
            this.ci4_hitbox = new System.Windows.Forms.Label();
            this.rgba32_ck = new System.Windows.Forms.PictureBox();
            this.rgba32_label = new System.Windows.Forms.Label();
            this.rgba32_hitbox = new System.Windows.Forms.Label();
            this.rgb5a3_ck = new System.Windows.Forms.PictureBox();
            this.rgb5a3_label = new System.Windows.Forms.Label();
            this.rgb5a3_hitbox = new System.Windows.Forms.Label();
            this.rgb565_ck = new System.Windows.Forms.PictureBox();
            this.rgb565_label = new System.Windows.Forms.Label();
            this.rgb565_hitbox = new System.Windows.Forms.Label();
            this.ai8_ck = new System.Windows.Forms.PictureBox();
            this.ai8_label = new System.Windows.Forms.Label();
            this.ai8_hitbox = new System.Windows.Forms.Label();
            this.ai4_ck = new System.Windows.Forms.PictureBox();
            this.ai4_label = new System.Windows.Forms.Label();
            this.ai4_hitbox = new System.Windows.Forms.Label();
            this.i8_ck = new System.Windows.Forms.PictureBox();
            this.i8_label = new System.Windows.Forms.Label();
            this.i8_hitbox = new System.Windows.Forms.Label();
            this.i4_ck = new System.Windows.Forms.PictureBox();
            this.i4_label = new System.Windows.Forms.Label();
            this.i4_hitbox = new System.Windows.Forms.Label();
            this.encoding_label = new System.Windows.Forms.Label();
            this.surrounding_ck = new System.Windows.Forms.PictureBox();
            this.no_gradient_ck = new System.Windows.Forms.PictureBox();
            this.no_gradient_label = new System.Windows.Forms.Label();
            this.no_gradient_hitbox = new System.Windows.Forms.Label();
            this.custom_ck = new System.Windows.Forms.PictureBox();
            this.custom_label = new System.Windows.Forms.Label();
            this.custom_hitbox = new System.Windows.Forms.Label();
            this.cie_709_ck = new System.Windows.Forms.PictureBox();
            this.cie_709_label = new System.Windows.Forms.Label();
            this.cie_709_hitbox = new System.Windows.Forms.Label();
            this.cie_601_ck = new System.Windows.Forms.PictureBox();
            this.cie_601_label = new System.Windows.Forms.Label();
            this.cie_601_hitbox = new System.Windows.Forms.Label();
            this.algorithm_label = new System.Windows.Forms.Label();
            this.mix_ck = new System.Windows.Forms.PictureBox();
            this.mix_label = new System.Windows.Forms.Label();
            this.mix_hitbox = new System.Windows.Forms.Label();
            this.alpha_ck = new System.Windows.Forms.PictureBox();
            this.alpha_label = new System.Windows.Forms.Label();
            this.alpha_hitbox = new System.Windows.Forms.Label();
            this.no_alpha_ck = new System.Windows.Forms.PictureBox();
            this.no_alpha_label = new System.Windows.Forms.Label();
            this.no_alpha_hitbox = new System.Windows.Forms.Label();
            this.alpha_title = new System.Windows.Forms.Label();
            this.Tmirror_ck = new System.Windows.Forms.PictureBox();
            this.Tmirror_label = new System.Windows.Forms.Label();
            this.Tmirror_hitbox = new System.Windows.Forms.Label();
            this.Trepeat_ck = new System.Windows.Forms.PictureBox();
            this.Trepeat_label = new System.Windows.Forms.Label();
            this.Trepeat_hitbox = new System.Windows.Forms.Label();
            this.Tclamp_ck = new System.Windows.Forms.PictureBox();
            this.Tclamp_label = new System.Windows.Forms.Label();
            this.Tclamp_hitbox = new System.Windows.Forms.Label();
            this.WrapT_label = new System.Windows.Forms.Label();
            this.Smirror_ck = new System.Windows.Forms.PictureBox();
            this.Smirror_label = new System.Windows.Forms.Label();
            this.Smirror_hitbox = new System.Windows.Forms.Label();
            this.Srepeat_ck = new System.Windows.Forms.PictureBox();
            this.Srepeat_label = new System.Windows.Forms.Label();
            this.Srepeat_hitbox = new System.Windows.Forms.Label();
            this.Sclamp_ck = new System.Windows.Forms.PictureBox();
            this.Sclamp_label = new System.Windows.Forms.Label();
            this.Sclamp_hitbox = new System.Windows.Forms.Label();
            this.WrapS_label = new System.Windows.Forms.Label();
            this.magnification_label = new System.Windows.Forms.Label();
            this.minification_label = new System.Windows.Forms.Label();
            this.min_linearmipmaplinear_ck = new System.Windows.Forms.PictureBox();
            this.min_linearmipmaplinear_label = new System.Windows.Forms.Label();
            this.min_linearmipmaplinear_hitbox = new System.Windows.Forms.Label();
            this.min_linearmipmapnearest_ck = new System.Windows.Forms.PictureBox();
            this.min_linearmipmapnearest_label = new System.Windows.Forms.Label();
            this.min_linearmipmapnearest_hitbox = new System.Windows.Forms.Label();
            this.min_nearestmipmaplinear_ck = new System.Windows.Forms.PictureBox();
            this.min_nearestmipmaplinear_label = new System.Windows.Forms.Label();
            this.min_nearestmipmaplinear_hitbox = new System.Windows.Forms.Label();
            this.min_nearestmipmapnearest_ck = new System.Windows.Forms.PictureBox();
            this.min_nearestmipmapnearest_label = new System.Windows.Forms.Label();
            this.min_nearestmipmapnearest_hitbox = new System.Windows.Forms.Label();
            this.min_linear_ck = new System.Windows.Forms.PictureBox();
            this.min_linear_label = new System.Windows.Forms.Label();
            this.min_linear_hitbox = new System.Windows.Forms.Label();
            this.min_nearest_neighbour_ck = new System.Windows.Forms.PictureBox();
            this.min_nearest_neighbour_label = new System.Windows.Forms.Label();
            this.min_nearest_neighbour_hitbox = new System.Windows.Forms.Label();
            this.mag_linearmipmaplinear_ck = new System.Windows.Forms.PictureBox();
            this.mag_linearmipmaplinear_label = new System.Windows.Forms.Label();
            this.mag_linearmipmaplinear_hitbox = new System.Windows.Forms.Label();
            this.mag_linearmipmapnearest_ck = new System.Windows.Forms.PictureBox();
            this.mag_linearmipmapnearest_label = new System.Windows.Forms.Label();
            this.mag_linearmipmapnearest_hitbox = new System.Windows.Forms.Label();
            this.mag_nearestmipmaplinear_ck = new System.Windows.Forms.PictureBox();
            this.mag_nearestmipmaplinear_label = new System.Windows.Forms.Label();
            this.mag_nearestmipmaplinear_hitbox = new System.Windows.Forms.Label();
            this.mag_nearestmipmapnearest_ck = new System.Windows.Forms.PictureBox();
            this.mag_nearestmipmapnearest_label = new System.Windows.Forms.Label();
            this.mag_nearestmipmapnearest_hitbox = new System.Windows.Forms.Label();
            this.mag_linear_ck = new System.Windows.Forms.PictureBox();
            this.mag_linear_label = new System.Windows.Forms.Label();
            this.mag_linear_hitbox = new System.Windows.Forms.Label();
            this.mag_nearest_neighbour_ck = new System.Windows.Forms.PictureBox();
            this.mag_nearest_neighbour_label = new System.Windows.Forms.Label();
            this.mag_nearest_neighbour_hitbox = new System.Windows.Forms.Label();
            this.r_r_ck = new System.Windows.Forms.PictureBox();
            this.r_g_ck = new System.Windows.Forms.PictureBox();
            this.g_r_ck = new System.Windows.Forms.PictureBox();
            this.g_g_ck = new System.Windows.Forms.PictureBox();
            this.a_g_ck = new System.Windows.Forms.PictureBox();
            this.a_r_ck = new System.Windows.Forms.PictureBox();
            this.b_g_ck = new System.Windows.Forms.PictureBox();
            this.b_r_ck = new System.Windows.Forms.PictureBox();
            this.g_a_ck = new System.Windows.Forms.PictureBox();
            this.g_b_ck = new System.Windows.Forms.PictureBox();
            this.r_a_ck = new System.Windows.Forms.PictureBox();
            this.r_b_ck = new System.Windows.Forms.PictureBox();
            this.a_a_ck = new System.Windows.Forms.PictureBox();
            this.a_b_ck = new System.Windows.Forms.PictureBox();
            this.b_a_ck = new System.Windows.Forms.PictureBox();
            this.b_b_ck = new System.Windows.Forms.PictureBox();
            this.colour_channels_label = new System.Windows.Forms.Label();
            this.r_r_ck_hitbox = new System.Windows.Forms.Label();
            this.g_r_ck_hitbox = new System.Windows.Forms.Label();
            this.a_a_ck_hitbox = new System.Windows.Forms.Label();
            this.r_g_ck_hitbox = new System.Windows.Forms.Label();
            this.r_b_ck_hitbox = new System.Windows.Forms.Label();
            this.r_a_ck_hitbox = new System.Windows.Forms.Label();
            this.g_g_ck_hitbox = new System.Windows.Forms.Label();
            this.g_b_ck_hitbox = new System.Windows.Forms.Label();
            this.g_a_ck_hitbox = new System.Windows.Forms.Label();
            this.b_r_ck_hitbox = new System.Windows.Forms.Label();
            this.b_g_ck_hitbox = new System.Windows.Forms.Label();
            this.b_b_ck_hitbox = new System.Windows.Forms.Label();
            this.b_a_ck_hitbox = new System.Windows.Forms.Label();
            this.a_r_ck_hitbox = new System.Windows.Forms.Label();
            this.a_g_ck_hitbox = new System.Windows.Forms.Label();
            this.a_b_ck_hitbox = new System.Windows.Forms.Label();
            this.view_alpha_ck = new System.Windows.Forms.PictureBox();
            this.view_alpha_label = new System.Windows.Forms.Label();
            this.view_alpha_hitbox = new System.Windows.Forms.Label();
            this.view_algorithm_ck = new System.Windows.Forms.PictureBox();
            this.view_algorithm_label = new System.Windows.Forms.Label();
            this.view_algorithm_hitbox = new System.Windows.Forms.Label();
            this.view_WrapS_ck = new System.Windows.Forms.PictureBox();
            this.view_WrapS_label = new System.Windows.Forms.Label();
            this.view_WrapS_hitbox = new System.Windows.Forms.Label();
            this.view_WrapT_ck = new System.Windows.Forms.PictureBox();
            this.view_WrapT_label = new System.Windows.Forms.Label();
            this.view_WrapT_hitbox = new System.Windows.Forms.Label();
            this.view_mag_ck = new System.Windows.Forms.PictureBox();
            this.view_mag_label = new System.Windows.Forms.Label();
            this.view_mag_hitbox = new System.Windows.Forms.Label();
            this.view_min_ck = new System.Windows.Forms.PictureBox();
            this.view_min_label = new System.Windows.Forms.Label();
            this.view_min_hitbox = new System.Windows.Forms.Label();
            this.banner_ck = new System.Windows.Forms.PictureBox();
            this.all_hitbox = new System.Windows.Forms.Label();
            this.preview_hitbox = new System.Windows.Forms.Label();
            this.paint_hitbox = new System.Windows.Forms.Label();
            this.auto_hitbox = new System.Windows.Forms.Label();
            this.all_ck = new System.Windows.Forms.PictureBox();
            this.preview_ck = new System.Windows.Forms.PictureBox();
            this.auto_ck = new System.Windows.Forms.PictureBox();
            this.paint_ck = new System.Windows.Forms.PictureBox();
            this.banner_x_ck = new System.Windows.Forms.PictureBox();
            this.banner_x_hitbox = new System.Windows.Forms.Label();
            this.banner_5_ck = new System.Windows.Forms.PictureBox();
            this.banner_5_hitbox = new System.Windows.Forms.Label();
            this.banner_minus_ck = new System.Windows.Forms.PictureBox();
            this.banner_minus_hitbox = new System.Windows.Forms.Label();
            this.banner_9_ck = new System.Windows.Forms.PictureBox();
            this.banner_9_hitbox = new System.Windows.Forms.Label();
            this.banner_8_ck = new System.Windows.Forms.PictureBox();
            this.banner_8_hitbox = new System.Windows.Forms.Label();
            this.banner_7_ck = new System.Windows.Forms.PictureBox();
            this.banner_7_hitbox = new System.Windows.Forms.Label();
            this.banner_6_ck = new System.Windows.Forms.PictureBox();
            this.banner_6_hitbox = new System.Windows.Forms.Label();
            this.banner_4_ck = new System.Windows.Forms.PictureBox();
            this.banner_4_hitbox = new System.Windows.Forms.Label();
            this.banner_3_ck = new System.Windows.Forms.PictureBox();
            this.banner_3_hitbox = new System.Windows.Forms.Label();
            this.banner_2_ck = new System.Windows.Forms.PictureBox();
            this.banner_2_hitbox = new System.Windows.Forms.Label();
            this.banner_1_ck = new System.Windows.Forms.PictureBox();
            this.banner_1_hitbox = new System.Windows.Forms.Label();
            this.cli_textbox_ck = new System.Windows.Forms.PictureBox();
            this.run_ck = new System.Windows.Forms.PictureBox();
            this.run_hitbox = new System.Windows.Forms.Label();
            this.cli_textbox_hitbox = new System.Windows.Forms.Label();
            this.input_file_txt = new System.Windows.Forms.TextBox();
            this.input_file_label = new System.Windows.Forms.Label();
            this.input_file2_label = new System.Windows.Forms.Label();
            this.input_file2_txt = new System.Windows.Forms.TextBox();
            this.mipmaps_label = new System.Windows.Forms.Label();
            this.mipmaps_txt = new System.Windows.Forms.TextBox();
            this.diversity_label = new System.Windows.Forms.Label();
            this.diversity_txt = new System.Windows.Forms.TextBox();
            this.diversity2_label = new System.Windows.Forms.Label();
            this.diversity2_txt = new System.Windows.Forms.TextBox();
            this.percentage_label = new System.Windows.Forms.Label();
            this.percentage_txt = new System.Windows.Forms.TextBox();
            this.percentage2_label = new System.Windows.Forms.Label();
            this.percentage2_txt = new System.Windows.Forms.TextBox();
            this.cmpr_max_label = new System.Windows.Forms.Label();
            this.cmpr_max_txt = new System.Windows.Forms.TextBox();
            this.output_name_label = new System.Windows.Forms.Label();
            this.output_name_txt = new System.Windows.Forms.TextBox();
            this.cmpr_min_alpha_label = new System.Windows.Forms.Label();
            this.cmpr_min_alpha_txt = new System.Windows.Forms.TextBox();
            this.num_colours_label = new System.Windows.Forms.Label();
            this.num_colours_txt = new System.Windows.Forms.TextBox();
            this.round3_label = new System.Windows.Forms.Label();
            this.round3_txt = new System.Windows.Forms.TextBox();
            this.round4_label = new System.Windows.Forms.Label();
            this.round4_txt = new System.Windows.Forms.TextBox();
            this.round5_label = new System.Windows.Forms.Label();
            this.round5_txt = new System.Windows.Forms.TextBox();
            this.round6_label = new System.Windows.Forms.Label();
            this.round6_txt = new System.Windows.Forms.TextBox();
            this.custom_a_label = new System.Windows.Forms.Label();
            this.custom_a_txt = new System.Windows.Forms.TextBox();
            this.custom_b_label = new System.Windows.Forms.Label();
            this.custom_b_txt = new System.Windows.Forms.TextBox();
            this.custom_g_label = new System.Windows.Forms.Label();
            this.custom_g_txt = new System.Windows.Forms.TextBox();
            this.custom_r_label = new System.Windows.Forms.Label();
            this.custom_r_txt = new System.Windows.Forms.TextBox();
            this.custom_rgba_label = new System.Windows.Forms.Label();
            this.description_title = new System.Windows.Forms.Label();
            this.description = new System.Windows.Forms.Label();
            this.description_surrounding = new System.Windows.Forms.PictureBox();
            this.palette_rgb5a3_ck = new System.Windows.Forms.PictureBox();
            this.palette_rgb5a3_label = new System.Windows.Forms.Label();
            this.palette_rgb5a3_hitbox = new System.Windows.Forms.Label();
            this.palette_rgb565_ck = new System.Windows.Forms.PictureBox();
            this.palette_rgb565_label = new System.Windows.Forms.Label();
            this.palette_rgb565_hitbox = new System.Windows.Forms.Label();
            this.palette_ai8_ck = new System.Windows.Forms.PictureBox();
            this.palette_ai8_label = new System.Windows.Forms.Label();
            this.palette_ai8_hitbox = new System.Windows.Forms.Label();
            this.palette_label = new System.Windows.Forms.Label();
            this.github_ck = new System.Windows.Forms.PictureBox();
            this.github_hitbox = new System.Windows.Forms.Label();
            this.youtube_ck = new System.Windows.Forms.PictureBox();
            this.youtube_hitbox = new System.Windows.Forms.Label();
            this.discord_ck = new System.Windows.Forms.PictureBox();
            this.discord_hitbox = new System.Windows.Forms.Label();
            this.input_file_hitbox = new System.Windows.Forms.Label();
            this.input_file2_hitbox = new System.Windows.Forms.Label();
            this.custom_r_hitbox = new System.Windows.Forms.Label();
            this.num_colours_hitbox = new System.Windows.Forms.Label();
            this.diversity_hitbox = new System.Windows.Forms.Label();
            this.round3_hitbox = new System.Windows.Forms.Label();
            this.cmpr_min_alpha_hitbox = new System.Windows.Forms.Label();
            this.cmpr_max_hitbox = new System.Windows.Forms.Label();
            this.mipmaps_hitbox = new System.Windows.Forms.Label();
            this.output_name_hitbox = new System.Windows.Forms.Label();
            this.round4_hitbox = new System.Windows.Forms.Label();
            this.round5_hitbox = new System.Windows.Forms.Label();
            this.round6_hitbox = new System.Windows.Forms.Label();
            this.diversity2_hitbox = new System.Windows.Forms.Label();
            this.percentage_hitbox = new System.Windows.Forms.Label();
            this.percentage2_hitbox = new System.Windows.Forms.Label();
            this.custom_g_hitbox = new System.Windows.Forms.Label();
            this.custom_b_hitbox = new System.Windows.Forms.Label();
            this.custom_a_hitbox = new System.Windows.Forms.Label();
            this.desc2 = new System.Windows.Forms.Label();
            this.desc3 = new System.Windows.Forms.Label();
            this.desc4 = new System.Windows.Forms.Label();
            this.desc5 = new System.Windows.Forms.Label();
            this.desc6 = new System.Windows.Forms.Label();
            this.desc7 = new System.Windows.Forms.Label();
            this.desc8 = new System.Windows.Forms.Label();
            this.desc9 = new System.Windows.Forms.Label();
            this.version_hitbox = new System.Windows.Forms.Label();
            this.output_label = new System.Windows.Forms.Label();
            this.version_ck = new System.Windows.Forms.PictureBox();
            this.cli_textbox_label = new System.Windows.Forms.Label();
            this.view_rgba_ck = new System.Windows.Forms.PictureBox();
            this.view_rgba_label = new System.Windows.Forms.Label();
            this.view_rgba_hitbox = new System.Windows.Forms.Label();
            this.view_palette_ck = new System.Windows.Forms.PictureBox();
            this.view_palette_label = new System.Windows.Forms.Label();
            this.view_palette_hitbox = new System.Windows.Forms.Label();
            this.view_cmpr_ck = new System.Windows.Forms.PictureBox();
            this.view_cmpr_label = new System.Windows.Forms.Label();
            this.view_cmpr_hitbox = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.view_options_ck = new System.Windows.Forms.PictureBox();
            this.view_options_label = new System.Windows.Forms.Label();
            this.view_options_hitbox = new System.Windows.Forms.Label();
            this.banner_move = new System.Windows.Forms.Label();
            this.banner_resize = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.bmd_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bti_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tex0_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpl_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bmp_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.png_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.jpg_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tiff_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tif_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ico_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gif_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.jpeg_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warn_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stfu_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.safe_mode_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reverse_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.random_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.no_warning_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.funky_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FORCE_ALPHA_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bmp_32_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ask_exit_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ci14x2_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ci8_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ci4_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgba32_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgb5a3_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgb565_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ai8_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ai4_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.i8_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.i4_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.surrounding_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.no_gradient_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.custom_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cie_709_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cie_601_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mix_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alpha_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.no_alpha_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tmirror_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Trepeat_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tclamp_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Smirror_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Srepeat_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Sclamp_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_linearmipmaplinear_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_linearmipmapnearest_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_nearestmipmaplinear_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_nearestmipmapnearest_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_linear_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_nearest_neighbour_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mag_linearmipmaplinear_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mag_linearmipmapnearest_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mag_nearestmipmaplinear_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mag_nearestmipmapnearest_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mag_linear_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mag_nearest_neighbour_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.r_r_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.r_g_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.g_r_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.g_g_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_g_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_r_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.b_g_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.b_r_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.g_a_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.g_b_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.r_a_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.r_b_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_a_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_b_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.b_a_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.b_b_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_alpha_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_algorithm_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_WrapS_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_WrapT_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_mag_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_min_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.all_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.auto_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paint_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_x_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_5_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_minus_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_9_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_8_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_7_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_6_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_4_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_3_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_2_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_1_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cli_textbox_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.run_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.description_surrounding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.palette_rgb5a3_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.palette_rgb565_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.palette_ai8_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.github_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.youtube_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.discord_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.version_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_rgba_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_palette_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_cmpr_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_options_ck)).BeginInit();
            this.SuspendLayout();
            // 
            // output_file_type_label
            // 
            this.output_file_type_label.AutoSize = true;
            this.output_file_type_label.BackColor = System.Drawing.Color.Transparent;
            this.output_file_type_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.output_file_type_label.ForeColor = System.Drawing.SystemColors.Control;
            this.output_file_type_label.Location = new System.Drawing.Point(36, 95);
            this.output_file_type_label.Margin = new System.Windows.Forms.Padding(0);
            this.output_file_type_label.Name = "output_file_type_label";
            this.output_file_type_label.Size = new System.Drawing.Size(194, 20);
            this.output_file_type_label.TabIndex = 0;
            this.output_file_type_label.Text = "Output file type";
            // 
            // mandatory_settings_label
            // 
            this.mandatory_settings_label.AutoSize = true;
            this.mandatory_settings_label.BackColor = System.Drawing.Color.Transparent;
            this.mandatory_settings_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mandatory_settings_label.ForeColor = System.Drawing.SystemColors.Control;
            this.mandatory_settings_label.Location = new System.Drawing.Point(119, 54);
            this.mandatory_settings_label.Name = "mandatory_settings_label";
            this.mandatory_settings_label.Size = new System.Drawing.Size(238, 20);
            this.mandatory_settings_label.TabIndex = 123;
            this.mandatory_settings_label.Text = "Mandatory Settings";
            // 
            // bmd_label
            // 
            this.bmd_label.AutoSize = true;
            this.bmd_label.BackColor = System.Drawing.Color.Transparent;
            this.bmd_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.bmd_label.ForeColor = System.Drawing.SystemColors.Window;
            this.bmd_label.Location = new System.Drawing.Point(108, 128);
            this.bmd_label.Margin = new System.Windows.Forms.Padding(0);
            this.bmd_label.Name = "bmd_label";
            this.bmd_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.bmd_label.Size = new System.Drawing.Size(57, 64);
            this.bmd_label.TabIndex = 124;
            this.bmd_label.Text = "bmd";
            this.bmd_label.Click += new System.EventHandler(this.bmd_Click);
            this.bmd_label.MouseEnter += new System.EventHandler(this.bmd_MouseEnter);
            this.bmd_label.MouseLeave += new System.EventHandler(this.bmd_MouseLeave);
            // 
            // bmd_hitbox
            // 
            this.bmd_hitbox.AutoSize = true;
            this.bmd_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.bmd_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.bmd_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.bmd_hitbox.Location = new System.Drawing.Point(37, 128);
            this.bmd_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.bmd_hitbox.Name = "bmd_hitbox";
            this.bmd_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.bmd_hitbox.Size = new System.Drawing.Size(128, 64);
            this.bmd_hitbox.TabIndex = 125;
            this.bmd_hitbox.Click += new System.EventHandler(this.bmd_Click);
            this.bmd_hitbox.MouseEnter += new System.EventHandler(this.bmd_MouseEnter);
            this.bmd_hitbox.MouseLeave += new System.EventHandler(this.bmd_MouseLeave);
            // 
            // bmd_ck
            // 
            this.bmd_ck.BackColor = System.Drawing.Color.Transparent;
            this.bmd_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bmd_ck.Enabled = false;
            this.bmd_ck.ErrorImage = null;
            this.bmd_ck.InitialImage = null;
            this.bmd_ck.Location = new System.Drawing.Point(40, 128);
            this.bmd_ck.Margin = new System.Windows.Forms.Padding(0);
            this.bmd_ck.Name = "bmd_ck";
            this.bmd_ck.Size = new System.Drawing.Size(64, 64);
            this.bmd_ck.TabIndex = 126;
            this.bmd_ck.TabStop = false;
            // 
            // bti_ck
            // 
            this.bti_ck.BackColor = System.Drawing.Color.Transparent;
            this.bti_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bti_ck.Enabled = false;
            this.bti_ck.ErrorImage = null;
            this.bti_ck.InitialImage = null;
            this.bti_ck.Location = new System.Drawing.Point(40, 192);
            this.bti_ck.Margin = new System.Windows.Forms.Padding(0);
            this.bti_ck.Name = "bti_ck";
            this.bti_ck.Size = new System.Drawing.Size(64, 64);
            this.bti_ck.TabIndex = 129;
            this.bti_ck.TabStop = false;
            // 
            // bti_label
            // 
            this.bti_label.AutoSize = true;
            this.bti_label.BackColor = System.Drawing.Color.Transparent;
            this.bti_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.bti_label.ForeColor = System.Drawing.SystemColors.Window;
            this.bti_label.Location = new System.Drawing.Point(108, 192);
            this.bti_label.Margin = new System.Windows.Forms.Padding(0);
            this.bti_label.Name = "bti_label";
            this.bti_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.bti_label.Size = new System.Drawing.Size(38, 64);
            this.bti_label.TabIndex = 127;
            this.bti_label.Text = "bti";
            this.bti_label.Click += new System.EventHandler(this.bti_Click);
            this.bti_label.MouseEnter += new System.EventHandler(this.bti_MouseEnter);
            this.bti_label.MouseLeave += new System.EventHandler(this.bti_MouseLeave);
            // 
            // bti_hitbox
            // 
            this.bti_hitbox.AutoSize = true;
            this.bti_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.bti_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.bti_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.bti_hitbox.Location = new System.Drawing.Point(37, 192);
            this.bti_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.bti_hitbox.Name = "bti_hitbox";
            this.bti_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.bti_hitbox.Size = new System.Drawing.Size(128, 64);
            this.bti_hitbox.TabIndex = 128;
            this.bti_hitbox.Click += new System.EventHandler(this.bti_Click);
            this.bti_hitbox.MouseEnter += new System.EventHandler(this.bti_MouseEnter);
            this.bti_hitbox.MouseLeave += new System.EventHandler(this.bti_MouseLeave);
            // 
            // tex0_ck
            // 
            this.tex0_ck.BackColor = System.Drawing.Color.Transparent;
            this.tex0_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tex0_ck.Enabled = false;
            this.tex0_ck.ErrorImage = null;
            this.tex0_ck.InitialImage = null;
            this.tex0_ck.Location = new System.Drawing.Point(40, 256);
            this.tex0_ck.Margin = new System.Windows.Forms.Padding(0);
            this.tex0_ck.Name = "tex0_ck";
            this.tex0_ck.Size = new System.Drawing.Size(64, 64);
            this.tex0_ck.TabIndex = 132;
            this.tex0_ck.TabStop = false;
            // 
            // tex0_label
            // 
            this.tex0_label.AutoSize = true;
            this.tex0_label.BackColor = System.Drawing.Color.Transparent;
            this.tex0_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.tex0_label.ForeColor = System.Drawing.SystemColors.Window;
            this.tex0_label.Location = new System.Drawing.Point(108, 257);
            this.tex0_label.Margin = new System.Windows.Forms.Padding(0);
            this.tex0_label.Name = "tex0_label";
            this.tex0_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.tex0_label.Size = new System.Drawing.Size(62, 64);
            this.tex0_label.TabIndex = 130;
            this.tex0_label.Text = "tex0";
            this.tex0_label.Click += new System.EventHandler(this.tex0_Click);
            this.tex0_label.MouseEnter += new System.EventHandler(this.tex0_MouseEnter);
            this.tex0_label.MouseLeave += new System.EventHandler(this.tex0_MouseLeave);
            // 
            // tex0_hitbox
            // 
            this.tex0_hitbox.AutoSize = true;
            this.tex0_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.tex0_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.tex0_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.tex0_hitbox.Location = new System.Drawing.Point(37, 256);
            this.tex0_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.tex0_hitbox.Name = "tex0_hitbox";
            this.tex0_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.tex0_hitbox.Size = new System.Drawing.Size(128, 64);
            this.tex0_hitbox.TabIndex = 131;
            this.tex0_hitbox.Click += new System.EventHandler(this.tex0_Click);
            this.tex0_hitbox.MouseEnter += new System.EventHandler(this.tex0_MouseEnter);
            this.tex0_hitbox.MouseLeave += new System.EventHandler(this.tex0_MouseLeave);
            // 
            // tpl_ck
            // 
            this.tpl_ck.BackColor = System.Drawing.Color.Transparent;
            this.tpl_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tpl_ck.Enabled = false;
            this.tpl_ck.ErrorImage = null;
            this.tpl_ck.InitialImage = null;
            this.tpl_ck.Location = new System.Drawing.Point(40, 320);
            this.tpl_ck.Margin = new System.Windows.Forms.Padding(0);
            this.tpl_ck.Name = "tpl_ck";
            this.tpl_ck.Size = new System.Drawing.Size(64, 64);
            this.tpl_ck.TabIndex = 135;
            this.tpl_ck.TabStop = false;
            // 
            // tpl_label
            // 
            this.tpl_label.AutoSize = true;
            this.tpl_label.BackColor = System.Drawing.Color.Transparent;
            this.tpl_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.tpl_label.ForeColor = System.Drawing.SystemColors.Window;
            this.tpl_label.Location = new System.Drawing.Point(108, 321);
            this.tpl_label.Margin = new System.Windows.Forms.Padding(0);
            this.tpl_label.Name = "tpl_label";
            this.tpl_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.tpl_label.Size = new System.Drawing.Size(39, 64);
            this.tpl_label.TabIndex = 133;
            this.tpl_label.Text = "tpl";
            this.tpl_label.Click += new System.EventHandler(this.tpl_Click);
            this.tpl_label.MouseEnter += new System.EventHandler(this.tpl_MouseEnter);
            this.tpl_label.MouseLeave += new System.EventHandler(this.tpl_MouseLeave);
            // 
            // tpl_hitbox
            // 
            this.tpl_hitbox.AutoSize = true;
            this.tpl_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.tpl_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.tpl_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.tpl_hitbox.Location = new System.Drawing.Point(37, 320);
            this.tpl_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.tpl_hitbox.Name = "tpl_hitbox";
            this.tpl_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.tpl_hitbox.Size = new System.Drawing.Size(128, 64);
            this.tpl_hitbox.TabIndex = 134;
            this.tpl_hitbox.Click += new System.EventHandler(this.tpl_Click);
            this.tpl_hitbox.MouseEnter += new System.EventHandler(this.tpl_MouseEnter);
            this.tpl_hitbox.MouseLeave += new System.EventHandler(this.tpl_MouseLeave);
            // 
            // bmp_ck
            // 
            this.bmp_ck.BackColor = System.Drawing.Color.Transparent;
            this.bmp_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bmp_ck.Enabled = false;
            this.bmp_ck.ErrorImage = null;
            this.bmp_ck.InitialImage = null;
            this.bmp_ck.Location = new System.Drawing.Point(40, 384);
            this.bmp_ck.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_ck.Name = "bmp_ck";
            this.bmp_ck.Size = new System.Drawing.Size(64, 64);
            this.bmp_ck.TabIndex = 138;
            this.bmp_ck.TabStop = false;
            // 
            // bmp_label
            // 
            this.bmp_label.AutoSize = true;
            this.bmp_label.BackColor = System.Drawing.Color.Transparent;
            this.bmp_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.bmp_label.ForeColor = System.Drawing.SystemColors.Window;
            this.bmp_label.Location = new System.Drawing.Point(108, 384);
            this.bmp_label.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_label.Name = "bmp_label";
            this.bmp_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.bmp_label.Size = new System.Drawing.Size(57, 64);
            this.bmp_label.TabIndex = 136;
            this.bmp_label.Text = "bmp";
            this.bmp_label.Click += new System.EventHandler(this.bmp_Click);
            this.bmp_label.MouseEnter += new System.EventHandler(this.bmp_MouseEnter);
            this.bmp_label.MouseLeave += new System.EventHandler(this.bmp_MouseLeave);
            // 
            // bmp_hitbox
            // 
            this.bmp_hitbox.AutoSize = true;
            this.bmp_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.bmp_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.bmp_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.bmp_hitbox.Location = new System.Drawing.Point(37, 384);
            this.bmp_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_hitbox.Name = "bmp_hitbox";
            this.bmp_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.bmp_hitbox.Size = new System.Drawing.Size(128, 64);
            this.bmp_hitbox.TabIndex = 137;
            this.bmp_hitbox.Click += new System.EventHandler(this.bmp_Click);
            this.bmp_hitbox.MouseEnter += new System.EventHandler(this.bmp_MouseEnter);
            this.bmp_hitbox.MouseLeave += new System.EventHandler(this.bmp_MouseLeave);
            // 
            // png_ck
            // 
            this.png_ck.BackColor = System.Drawing.Color.Transparent;
            this.png_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.png_ck.Enabled = false;
            this.png_ck.ErrorImage = null;
            this.png_ck.InitialImage = null;
            this.png_ck.Location = new System.Drawing.Point(40, 448);
            this.png_ck.Margin = new System.Windows.Forms.Padding(0);
            this.png_ck.Name = "png_ck";
            this.png_ck.Size = new System.Drawing.Size(64, 64);
            this.png_ck.TabIndex = 141;
            this.png_ck.TabStop = false;
            // 
            // png_label
            // 
            this.png_label.AutoSize = true;
            this.png_label.BackColor = System.Drawing.Color.Transparent;
            this.png_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.png_label.ForeColor = System.Drawing.SystemColors.Window;
            this.png_label.Location = new System.Drawing.Point(108, 449);
            this.png_label.Margin = new System.Windows.Forms.Padding(0);
            this.png_label.Name = "png_label";
            this.png_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.png_label.Size = new System.Drawing.Size(51, 64);
            this.png_label.TabIndex = 139;
            this.png_label.Text = "png";
            this.png_label.Click += new System.EventHandler(this.png_Click);
            this.png_label.MouseEnter += new System.EventHandler(this.png_MouseEnter);
            this.png_label.MouseLeave += new System.EventHandler(this.png_MouseLeave);
            // 
            // png_hitbox
            // 
            this.png_hitbox.AutoSize = true;
            this.png_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.png_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.png_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.png_hitbox.Location = new System.Drawing.Point(37, 448);
            this.png_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.png_hitbox.Name = "png_hitbox";
            this.png_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.png_hitbox.Size = new System.Drawing.Size(128, 64);
            this.png_hitbox.TabIndex = 140;
            this.png_hitbox.Click += new System.EventHandler(this.png_Click);
            this.png_hitbox.MouseEnter += new System.EventHandler(this.png_MouseEnter);
            this.png_hitbox.MouseLeave += new System.EventHandler(this.png_MouseLeave);
            // 
            // jpg_ck
            // 
            this.jpg_ck.BackColor = System.Drawing.Color.Transparent;
            this.jpg_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.jpg_ck.Enabled = false;
            this.jpg_ck.ErrorImage = null;
            this.jpg_ck.InitialImage = null;
            this.jpg_ck.Location = new System.Drawing.Point(40, 512);
            this.jpg_ck.Margin = new System.Windows.Forms.Padding(0);
            this.jpg_ck.Name = "jpg_ck";
            this.jpg_ck.Size = new System.Drawing.Size(64, 64);
            this.jpg_ck.TabIndex = 144;
            this.jpg_ck.TabStop = false;
            // 
            // jpg_label
            // 
            this.jpg_label.AutoSize = true;
            this.jpg_label.BackColor = System.Drawing.Color.Transparent;
            this.jpg_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.jpg_label.ForeColor = System.Drawing.SystemColors.Window;
            this.jpg_label.Location = new System.Drawing.Point(108, 513);
            this.jpg_label.Margin = new System.Windows.Forms.Padding(0);
            this.jpg_label.Name = "jpg_label";
            this.jpg_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.jpg_label.Size = new System.Drawing.Size(44, 64);
            this.jpg_label.TabIndex = 142;
            this.jpg_label.Text = "jpg";
            this.jpg_label.Click += new System.EventHandler(this.jpg_Click);
            this.jpg_label.MouseEnter += new System.EventHandler(this.jpg_MouseEnter);
            this.jpg_label.MouseLeave += new System.EventHandler(this.jpg_MouseLeave);
            // 
            // jpg_hitbox
            // 
            this.jpg_hitbox.AutoSize = true;
            this.jpg_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.jpg_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.jpg_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.jpg_hitbox.Location = new System.Drawing.Point(37, 512);
            this.jpg_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.jpg_hitbox.Name = "jpg_hitbox";
            this.jpg_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.jpg_hitbox.Size = new System.Drawing.Size(128, 64);
            this.jpg_hitbox.TabIndex = 143;
            this.jpg_hitbox.Click += new System.EventHandler(this.jpg_Click);
            this.jpg_hitbox.MouseEnter += new System.EventHandler(this.jpg_MouseEnter);
            this.jpg_hitbox.MouseLeave += new System.EventHandler(this.jpg_MouseLeave);
            // 
            // tiff_ck
            // 
            this.tiff_ck.BackColor = System.Drawing.Color.Transparent;
            this.tiff_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tiff_ck.Enabled = false;
            this.tiff_ck.ErrorImage = null;
            this.tiff_ck.InitialImage = null;
            this.tiff_ck.Location = new System.Drawing.Point(40, 833);
            this.tiff_ck.Margin = new System.Windows.Forms.Padding(0);
            this.tiff_ck.Name = "tiff_ck";
            this.tiff_ck.Size = new System.Drawing.Size(64, 64);
            this.tiff_ck.TabIndex = 159;
            this.tiff_ck.TabStop = false;
            // 
            // tiff_label
            // 
            this.tiff_label.AutoSize = true;
            this.tiff_label.BackColor = System.Drawing.Color.Transparent;
            this.tiff_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.tiff_label.ForeColor = System.Drawing.SystemColors.Window;
            this.tiff_label.Location = new System.Drawing.Point(108, 834);
            this.tiff_label.Margin = new System.Windows.Forms.Padding(0);
            this.tiff_label.Name = "tiff_label";
            this.tiff_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.tiff_label.Size = new System.Drawing.Size(42, 64);
            this.tiff_label.TabIndex = 157;
            this.tiff_label.Text = "tiff";
            this.tiff_label.Click += new System.EventHandler(this.tiff_Click);
            this.tiff_label.MouseEnter += new System.EventHandler(this.tiff_MouseEnter);
            this.tiff_label.MouseLeave += new System.EventHandler(this.tiff_MouseLeave);
            // 
            // tiff_hitbox
            // 
            this.tiff_hitbox.AutoSize = true;
            this.tiff_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.tiff_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.tiff_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.tiff_hitbox.Location = new System.Drawing.Point(37, 833);
            this.tiff_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.tiff_hitbox.Name = "tiff_hitbox";
            this.tiff_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.tiff_hitbox.Size = new System.Drawing.Size(128, 64);
            this.tiff_hitbox.TabIndex = 158;
            this.tiff_hitbox.Click += new System.EventHandler(this.tiff_Click);
            this.tiff_hitbox.MouseEnter += new System.EventHandler(this.tiff_MouseEnter);
            this.tiff_hitbox.MouseLeave += new System.EventHandler(this.tiff_MouseLeave);
            // 
            // tif_ck
            // 
            this.tif_ck.BackColor = System.Drawing.Color.Transparent;
            this.tif_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tif_ck.Enabled = false;
            this.tif_ck.ErrorImage = null;
            this.tif_ck.InitialImage = null;
            this.tif_ck.Location = new System.Drawing.Point(40, 769);
            this.tif_ck.Margin = new System.Windows.Forms.Padding(0);
            this.tif_ck.Name = "tif_ck";
            this.tif_ck.Size = new System.Drawing.Size(64, 64);
            this.tif_ck.TabIndex = 156;
            this.tif_ck.TabStop = false;
            // 
            // tif_label
            // 
            this.tif_label.AutoSize = true;
            this.tif_label.BackColor = System.Drawing.Color.Transparent;
            this.tif_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.tif_label.ForeColor = System.Drawing.SystemColors.Window;
            this.tif_label.Location = new System.Drawing.Point(108, 770);
            this.tif_label.Margin = new System.Windows.Forms.Padding(0);
            this.tif_label.Name = "tif_label";
            this.tif_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.tif_label.Size = new System.Drawing.Size(33, 64);
            this.tif_label.TabIndex = 154;
            this.tif_label.Text = "tif";
            this.tif_label.Click += new System.EventHandler(this.tif_Click);
            this.tif_label.MouseEnter += new System.EventHandler(this.tif_MouseEnter);
            this.tif_label.MouseLeave += new System.EventHandler(this.tif_MouseLeave);
            // 
            // tif_hitbox
            // 
            this.tif_hitbox.AutoSize = true;
            this.tif_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.tif_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.tif_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.tif_hitbox.Location = new System.Drawing.Point(37, 769);
            this.tif_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.tif_hitbox.Name = "tif_hitbox";
            this.tif_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.tif_hitbox.Size = new System.Drawing.Size(128, 64);
            this.tif_hitbox.TabIndex = 155;
            this.tif_hitbox.Click += new System.EventHandler(this.tif_Click);
            this.tif_hitbox.MouseEnter += new System.EventHandler(this.tif_MouseEnter);
            this.tif_hitbox.MouseLeave += new System.EventHandler(this.tif_MouseLeave);
            // 
            // ico_ck
            // 
            this.ico_ck.BackColor = System.Drawing.Color.Transparent;
            this.ico_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ico_ck.Enabled = false;
            this.ico_ck.ErrorImage = null;
            this.ico_ck.InitialImage = null;
            this.ico_ck.Location = new System.Drawing.Point(40, 705);
            this.ico_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ico_ck.Name = "ico_ck";
            this.ico_ck.Size = new System.Drawing.Size(64, 64);
            this.ico_ck.TabIndex = 153;
            this.ico_ck.TabStop = false;
            // 
            // ico_label
            // 
            this.ico_label.AutoSize = true;
            this.ico_label.BackColor = System.Drawing.Color.Transparent;
            this.ico_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ico_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ico_label.Location = new System.Drawing.Point(108, 705);
            this.ico_label.Margin = new System.Windows.Forms.Padding(0);
            this.ico_label.Name = "ico_label";
            this.ico_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.ico_label.Size = new System.Drawing.Size(42, 64);
            this.ico_label.TabIndex = 151;
            this.ico_label.Text = "ico";
            this.ico_label.Click += new System.EventHandler(this.ico_Click);
            this.ico_label.MouseEnter += new System.EventHandler(this.ico_MouseEnter);
            this.ico_label.MouseLeave += new System.EventHandler(this.ico_MouseLeave);
            // 
            // ico_hitbox
            // 
            this.ico_hitbox.AutoSize = true;
            this.ico_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.ico_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ico_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ico_hitbox.Location = new System.Drawing.Point(37, 705);
            this.ico_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ico_hitbox.Name = "ico_hitbox";
            this.ico_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.ico_hitbox.Size = new System.Drawing.Size(128, 64);
            this.ico_hitbox.TabIndex = 152;
            this.ico_hitbox.Click += new System.EventHandler(this.ico_Click);
            this.ico_hitbox.MouseEnter += new System.EventHandler(this.ico_MouseEnter);
            this.ico_hitbox.MouseLeave += new System.EventHandler(this.ico_MouseLeave);
            // 
            // gif_ck
            // 
            this.gif_ck.BackColor = System.Drawing.Color.Transparent;
            this.gif_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.gif_ck.Enabled = false;
            this.gif_ck.ErrorImage = null;
            this.gif_ck.InitialImage = null;
            this.gif_ck.Location = new System.Drawing.Point(40, 641);
            this.gif_ck.Margin = new System.Windows.Forms.Padding(0);
            this.gif_ck.Name = "gif_ck";
            this.gif_ck.Size = new System.Drawing.Size(64, 64);
            this.gif_ck.TabIndex = 150;
            this.gif_ck.TabStop = false;
            // 
            // gib_label
            // 
            this.gib_label.AutoSize = true;
            this.gib_label.BackColor = System.Drawing.Color.Transparent;
            this.gib_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.gib_label.ForeColor = System.Drawing.SystemColors.Window;
            this.gib_label.Location = new System.Drawing.Point(108, 642);
            this.gib_label.Margin = new System.Windows.Forms.Padding(0);
            this.gib_label.Name = "gib_label";
            this.gib_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.gib_label.Size = new System.Drawing.Size(37, 64);
            this.gib_label.TabIndex = 148;
            this.gib_label.Text = "gif";
            this.gib_label.Click += new System.EventHandler(this.gif_Click);
            this.gib_label.MouseEnter += new System.EventHandler(this.gif_MouseEnter);
            this.gib_label.MouseLeave += new System.EventHandler(this.gif_MouseLeave);
            // 
            // gif_hitbox
            // 
            this.gif_hitbox.AutoSize = true;
            this.gif_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.gif_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.gif_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.gif_hitbox.Location = new System.Drawing.Point(37, 641);
            this.gif_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.gif_hitbox.Name = "gif_hitbox";
            this.gif_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.gif_hitbox.Size = new System.Drawing.Size(128, 64);
            this.gif_hitbox.TabIndex = 149;
            this.gif_hitbox.Click += new System.EventHandler(this.gif_Click);
            this.gif_hitbox.MouseEnter += new System.EventHandler(this.gif_MouseEnter);
            this.gif_hitbox.MouseLeave += new System.EventHandler(this.gif_MouseLeave);
            // 
            // jpeg_ck
            // 
            this.jpeg_ck.BackColor = System.Drawing.Color.Transparent;
            this.jpeg_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.jpeg_ck.Enabled = false;
            this.jpeg_ck.ErrorImage = null;
            this.jpeg_ck.InitialImage = null;
            this.jpeg_ck.Location = new System.Drawing.Point(40, 577);
            this.jpeg_ck.Margin = new System.Windows.Forms.Padding(0);
            this.jpeg_ck.Name = "jpeg_ck";
            this.jpeg_ck.Size = new System.Drawing.Size(64, 64);
            this.jpeg_ck.TabIndex = 147;
            this.jpeg_ck.TabStop = false;
            // 
            // jpeg_label
            // 
            this.jpeg_label.AutoSize = true;
            this.jpeg_label.BackColor = System.Drawing.Color.Transparent;
            this.jpeg_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.jpeg_label.ForeColor = System.Drawing.SystemColors.Window;
            this.jpeg_label.Location = new System.Drawing.Point(108, 578);
            this.jpeg_label.Margin = new System.Windows.Forms.Padding(0);
            this.jpeg_label.Name = "jpeg_label";
            this.jpeg_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.jpeg_label.Size = new System.Drawing.Size(58, 64);
            this.jpeg_label.TabIndex = 145;
            this.jpeg_label.Text = "jpeg";
            this.jpeg_label.Click += new System.EventHandler(this.jpeg_Click);
            this.jpeg_label.MouseEnter += new System.EventHandler(this.jpeg_MouseEnter);
            this.jpeg_label.MouseLeave += new System.EventHandler(this.jpeg_MouseLeave);
            // 
            // jpeg_hitbox
            // 
            this.jpeg_hitbox.AutoSize = true;
            this.jpeg_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.jpeg_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.jpeg_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.jpeg_hitbox.Location = new System.Drawing.Point(37, 577);
            this.jpeg_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.jpeg_hitbox.Name = "jpeg_hitbox";
            this.jpeg_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.jpeg_hitbox.Size = new System.Drawing.Size(128, 64);
            this.jpeg_hitbox.TabIndex = 146;
            this.jpeg_hitbox.Click += new System.EventHandler(this.jpeg_Click);
            this.jpeg_hitbox.MouseEnter += new System.EventHandler(this.jpeg_MouseEnter);
            this.jpeg_hitbox.MouseLeave += new System.EventHandler(this.jpeg_MouseLeave);
            // 
            // options_label
            // 
            this.options_label.AutoSize = true;
            this.options_label.BackColor = System.Drawing.Color.Transparent;
            this.options_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.options_label.ForeColor = System.Drawing.SystemColors.Control;
            this.options_label.Location = new System.Drawing.Point(1674, 99);
            this.options_label.Name = "options_label";
            this.options_label.Size = new System.Drawing.Size(97, 20);
            this.options_label.TabIndex = 160;
            this.options_label.Text = "Options";
            // 
            // warn_ck
            // 
            this.warn_ck.BackColor = System.Drawing.Color.Transparent;
            this.warn_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.warn_ck.Enabled = false;
            this.warn_ck.ErrorImage = null;
            this.warn_ck.InitialImage = null;
            this.warn_ck.Location = new System.Drawing.Point(1647, 704);
            this.warn_ck.Margin = new System.Windows.Forms.Padding(0);
            this.warn_ck.Name = "warn_ck";
            this.warn_ck.Size = new System.Drawing.Size(64, 64);
            this.warn_ck.TabIndex = 190;
            this.warn_ck.TabStop = false;
            // 
            // warn_label
            // 
            this.warn_label.AutoSize = true;
            this.warn_label.BackColor = System.Drawing.Color.Transparent;
            this.warn_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.warn_label.ForeColor = System.Drawing.SystemColors.Window;
            this.warn_label.Location = new System.Drawing.Point(1716, 704);
            this.warn_label.Margin = new System.Windows.Forms.Padding(0);
            this.warn_label.Name = "warn_label";
            this.warn_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.warn_label.Size = new System.Drawing.Size(65, 64);
            this.warn_label.TabIndex = 188;
            this.warn_label.Text = "warn";
            this.warn_label.Click += new System.EventHandler(this.warn_Click);
            this.warn_label.MouseEnter += new System.EventHandler(this.warn_MouseEnter);
            this.warn_label.MouseLeave += new System.EventHandler(this.warn_MouseLeave);
            // 
            // stfu_ck
            // 
            this.stfu_ck.BackColor = System.Drawing.Color.Transparent;
            this.stfu_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.stfu_ck.Enabled = false;
            this.stfu_ck.ErrorImage = null;
            this.stfu_ck.InitialImage = null;
            this.stfu_ck.Location = new System.Drawing.Point(1647, 640);
            this.stfu_ck.Margin = new System.Windows.Forms.Padding(0);
            this.stfu_ck.Name = "stfu_ck";
            this.stfu_ck.Size = new System.Drawing.Size(64, 64);
            this.stfu_ck.TabIndex = 187;
            this.stfu_ck.TabStop = false;
            // 
            // stfu_label
            // 
            this.stfu_label.AutoSize = true;
            this.stfu_label.BackColor = System.Drawing.Color.Transparent;
            this.stfu_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.stfu_label.ForeColor = System.Drawing.SystemColors.Window;
            this.stfu_label.Location = new System.Drawing.Point(1716, 640);
            this.stfu_label.Margin = new System.Windows.Forms.Padding(0);
            this.stfu_label.Name = "stfu_label";
            this.stfu_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.stfu_label.Size = new System.Drawing.Size(55, 64);
            this.stfu_label.TabIndex = 185;
            this.stfu_label.Text = "stfu";
            this.stfu_label.Click += new System.EventHandler(this.stfu_Click);
            this.stfu_label.MouseEnter += new System.EventHandler(this.stfu_MouseEnter);
            this.stfu_label.MouseLeave += new System.EventHandler(this.stfu_MouseLeave);
            // 
            // safe_mode_ck
            // 
            this.safe_mode_ck.BackColor = System.Drawing.Color.Transparent;
            this.safe_mode_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.safe_mode_ck.Enabled = false;
            this.safe_mode_ck.ErrorImage = null;
            this.safe_mode_ck.InitialImage = null;
            this.safe_mode_ck.Location = new System.Drawing.Point(1647, 576);
            this.safe_mode_ck.Margin = new System.Windows.Forms.Padding(0);
            this.safe_mode_ck.Name = "safe_mode_ck";
            this.safe_mode_ck.Size = new System.Drawing.Size(64, 64);
            this.safe_mode_ck.TabIndex = 184;
            this.safe_mode_ck.TabStop = false;
            // 
            // safe_mode_label
            // 
            this.safe_mode_label.AutoSize = true;
            this.safe_mode_label.BackColor = System.Drawing.Color.Transparent;
            this.safe_mode_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.safe_mode_label.ForeColor = System.Drawing.SystemColors.Window;
            this.safe_mode_label.Location = new System.Drawing.Point(1716, 576);
            this.safe_mode_label.Margin = new System.Windows.Forms.Padding(0);
            this.safe_mode_label.Name = "safe_mode_label";
            this.safe_mode_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.safe_mode_label.Size = new System.Drawing.Size(131, 64);
            this.safe_mode_label.TabIndex = 182;
            this.safe_mode_label.Text = "safe mode";
            this.safe_mode_label.Click += new System.EventHandler(this.safe_mode_Click);
            this.safe_mode_label.MouseEnter += new System.EventHandler(this.safe_mode_MouseEnter);
            this.safe_mode_label.MouseLeave += new System.EventHandler(this.safe_mode_MouseLeave);
            // 
            // reverse_ck
            // 
            this.reverse_ck.BackColor = System.Drawing.Color.Transparent;
            this.reverse_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.reverse_ck.Enabled = false;
            this.reverse_ck.ErrorImage = null;
            this.reverse_ck.InitialImage = null;
            this.reverse_ck.Location = new System.Drawing.Point(1647, 512);
            this.reverse_ck.Margin = new System.Windows.Forms.Padding(0);
            this.reverse_ck.Name = "reverse_ck";
            this.reverse_ck.Size = new System.Drawing.Size(64, 64);
            this.reverse_ck.TabIndex = 181;
            this.reverse_ck.TabStop = false;
            // 
            // reverse_label
            // 
            this.reverse_label.AutoSize = true;
            this.reverse_label.BackColor = System.Drawing.Color.Transparent;
            this.reverse_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.reverse_label.ForeColor = System.Drawing.SystemColors.Window;
            this.reverse_label.Location = new System.Drawing.Point(1716, 512);
            this.reverse_label.Margin = new System.Windows.Forms.Padding(0);
            this.reverse_label.Name = "reverse_label";
            this.reverse_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.reverse_label.Size = new System.Drawing.Size(174, 64);
            this.reverse_label.TabIndex = 179;
            this.reverse_label.Text = "reverse image";
            this.reverse_label.Click += new System.EventHandler(this.reverse_Click);
            this.reverse_label.MouseEnter += new System.EventHandler(this.reverse_MouseEnter);
            this.reverse_label.MouseLeave += new System.EventHandler(this.reverse_MouseLeave);
            // 
            // random_ck
            // 
            this.random_ck.BackColor = System.Drawing.Color.Transparent;
            this.random_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.random_ck.Enabled = false;
            this.random_ck.ErrorImage = null;
            this.random_ck.InitialImage = null;
            this.random_ck.Location = new System.Drawing.Point(1647, 448);
            this.random_ck.Margin = new System.Windows.Forms.Padding(0);
            this.random_ck.Name = "random_ck";
            this.random_ck.Size = new System.Drawing.Size(64, 64);
            this.random_ck.TabIndex = 178;
            this.random_ck.TabStop = false;
            // 
            // random_label
            // 
            this.random_label.AutoSize = true;
            this.random_label.BackColor = System.Drawing.Color.Transparent;
            this.random_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.random_label.ForeColor = System.Drawing.SystemColors.Window;
            this.random_label.Location = new System.Drawing.Point(1716, 448);
            this.random_label.Margin = new System.Windows.Forms.Padding(0);
            this.random_label.Name = "random_label";
            this.random_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.random_label.Size = new System.Drawing.Size(187, 64);
            this.random_label.TabIndex = 176;
            this.random_label.Text = "random palette";
            this.random_label.Click += new System.EventHandler(this.random_Click);
            this.random_label.MouseEnter += new System.EventHandler(this.random_MouseEnter);
            this.random_label.MouseLeave += new System.EventHandler(this.random_MouseLeave);
            // 
            // no_warning_ck
            // 
            this.no_warning_ck.BackColor = System.Drawing.Color.Transparent;
            this.no_warning_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.no_warning_ck.Enabled = false;
            this.no_warning_ck.ErrorImage = null;
            this.no_warning_ck.InitialImage = null;
            this.no_warning_ck.Location = new System.Drawing.Point(1647, 384);
            this.no_warning_ck.Margin = new System.Windows.Forms.Padding(0);
            this.no_warning_ck.Name = "no_warning_ck";
            this.no_warning_ck.Size = new System.Drawing.Size(64, 64);
            this.no_warning_ck.TabIndex = 175;
            this.no_warning_ck.TabStop = false;
            // 
            // no_warning_label
            // 
            this.no_warning_label.AutoSize = true;
            this.no_warning_label.BackColor = System.Drawing.Color.Transparent;
            this.no_warning_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.no_warning_label.ForeColor = System.Drawing.SystemColors.Window;
            this.no_warning_label.Location = new System.Drawing.Point(1716, 384);
            this.no_warning_label.Margin = new System.Windows.Forms.Padding(0);
            this.no_warning_label.Name = "no_warning_label";
            this.no_warning_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.no_warning_label.Size = new System.Drawing.Size(136, 64);
            this.no_warning_label.TabIndex = 173;
            this.no_warning_label.Text = "no warning";
            this.no_warning_label.Click += new System.EventHandler(this.no_warning_Click);
            this.no_warning_label.MouseEnter += new System.EventHandler(this.no_warning_MouseEnter);
            this.no_warning_label.MouseLeave += new System.EventHandler(this.no_warning_MouseLeave);
            // 
            // funky_ck
            // 
            this.funky_ck.BackColor = System.Drawing.Color.Transparent;
            this.funky_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.funky_ck.Enabled = false;
            this.funky_ck.ErrorImage = null;
            this.funky_ck.InitialImage = null;
            this.funky_ck.Location = new System.Drawing.Point(1647, 320);
            this.funky_ck.Margin = new System.Windows.Forms.Padding(0);
            this.funky_ck.Name = "funky_ck";
            this.funky_ck.Size = new System.Drawing.Size(64, 64);
            this.funky_ck.TabIndex = 172;
            this.funky_ck.TabStop = false;
            // 
            // funky_label
            // 
            this.funky_label.AutoSize = true;
            this.funky_label.BackColor = System.Drawing.Color.Transparent;
            this.funky_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.funky_label.ForeColor = System.Drawing.SystemColors.Window;
            this.funky_label.Location = new System.Drawing.Point(1716, 320);
            this.funky_label.Margin = new System.Windows.Forms.Padding(0);
            this.funky_label.Name = "funky_label";
            this.funky_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.funky_label.Size = new System.Drawing.Size(72, 64);
            this.funky_label.TabIndex = 170;
            this.funky_label.Text = "funky";
            this.funky_label.Click += new System.EventHandler(this.funky_Click);
            this.funky_label.MouseEnter += new System.EventHandler(this.funky_MouseEnter);
            this.funky_label.MouseLeave += new System.EventHandler(this.funky_MouseLeave);
            // 
            // FORCE_ALPHA_ck
            // 
            this.FORCE_ALPHA_ck.BackColor = System.Drawing.Color.Transparent;
            this.FORCE_ALPHA_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.FORCE_ALPHA_ck.Enabled = false;
            this.FORCE_ALPHA_ck.ErrorImage = null;
            this.FORCE_ALPHA_ck.InitialImage = null;
            this.FORCE_ALPHA_ck.Location = new System.Drawing.Point(1647, 256);
            this.FORCE_ALPHA_ck.Margin = new System.Windows.Forms.Padding(0);
            this.FORCE_ALPHA_ck.Name = "FORCE_ALPHA_ck";
            this.FORCE_ALPHA_ck.Size = new System.Drawing.Size(64, 64);
            this.FORCE_ALPHA_ck.TabIndex = 169;
            this.FORCE_ALPHA_ck.TabStop = false;
            // 
            // FORCE_ALPHA_label
            // 
            this.FORCE_ALPHA_label.AutoSize = true;
            this.FORCE_ALPHA_label.BackColor = System.Drawing.Color.Transparent;
            this.FORCE_ALPHA_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.FORCE_ALPHA_label.ForeColor = System.Drawing.SystemColors.Window;
            this.FORCE_ALPHA_label.Location = new System.Drawing.Point(1716, 256);
            this.FORCE_ALPHA_label.Margin = new System.Windows.Forms.Padding(0);
            this.FORCE_ALPHA_label.Name = "FORCE_ALPHA_label";
            this.FORCE_ALPHA_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.FORCE_ALPHA_label.Size = new System.Drawing.Size(183, 64);
            this.FORCE_ALPHA_label.TabIndex = 167;
            this.FORCE_ALPHA_label.Text = "FORCE ALPHA";
            this.FORCE_ALPHA_label.Click += new System.EventHandler(this.FORCE_ALPHA_Click);
            this.FORCE_ALPHA_label.MouseEnter += new System.EventHandler(this.FORCE_ALPHA_MouseEnter);
            this.FORCE_ALPHA_label.MouseLeave += new System.EventHandler(this.FORCE_ALPHA_MouseLeave);
            // 
            // bmp_32_ck
            // 
            this.bmp_32_ck.BackColor = System.Drawing.Color.Transparent;
            this.bmp_32_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bmp_32_ck.Enabled = false;
            this.bmp_32_ck.ErrorImage = null;
            this.bmp_32_ck.InitialImage = null;
            this.bmp_32_ck.Location = new System.Drawing.Point(1647, 192);
            this.bmp_32_ck.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_32_ck.Name = "bmp_32_ck";
            this.bmp_32_ck.Size = new System.Drawing.Size(64, 64);
            this.bmp_32_ck.TabIndex = 166;
            this.bmp_32_ck.TabStop = false;
            // 
            // bmp_32_label
            // 
            this.bmp_32_label.AutoSize = true;
            this.bmp_32_label.BackColor = System.Drawing.Color.Transparent;
            this.bmp_32_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.bmp_32_label.ForeColor = System.Drawing.SystemColors.Window;
            this.bmp_32_label.Location = new System.Drawing.Point(1716, 192);
            this.bmp_32_label.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_32_label.Name = "bmp_32_label";
            this.bmp_32_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.bmp_32_label.Size = new System.Drawing.Size(137, 64);
            this.bmp_32_label.TabIndex = 164;
            this.bmp_32_label.Text = "32-bit bmp";
            this.bmp_32_label.Click += new System.EventHandler(this.bmp_32_Click);
            this.bmp_32_label.MouseEnter += new System.EventHandler(this.bmp_32_MouseEnter);
            this.bmp_32_label.MouseLeave += new System.EventHandler(this.bmp_32_MouseLeave);
            // 
            // ask_exit_ck
            // 
            this.ask_exit_ck.BackColor = System.Drawing.Color.Transparent;
            this.ask_exit_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ask_exit_ck.Enabled = false;
            this.ask_exit_ck.ErrorImage = null;
            this.ask_exit_ck.InitialImage = null;
            this.ask_exit_ck.Location = new System.Drawing.Point(1647, 128);
            this.ask_exit_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ask_exit_ck.Name = "ask_exit_ck";
            this.ask_exit_ck.Size = new System.Drawing.Size(64, 64);
            this.ask_exit_ck.TabIndex = 163;
            this.ask_exit_ck.TabStop = false;
            // 
            // ask_exit_label
            // 
            this.ask_exit_label.AutoSize = true;
            this.ask_exit_label.BackColor = System.Drawing.Color.Transparent;
            this.ask_exit_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ask_exit_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ask_exit_label.Location = new System.Drawing.Point(1716, 128);
            this.ask_exit_label.Margin = new System.Windows.Forms.Padding(0);
            this.ask_exit_label.Name = "ask_exit_label";
            this.ask_exit_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.ask_exit_label.Size = new System.Drawing.Size(101, 64);
            this.ask_exit_label.TabIndex = 161;
            this.ask_exit_label.Text = "ask exit";
            this.ask_exit_label.Click += new System.EventHandler(this.ask_exit_Click);
            this.ask_exit_label.MouseEnter += new System.EventHandler(this.ask_exit_MouseEnter);
            this.ask_exit_label.MouseLeave += new System.EventHandler(this.ask_exit_MouseLeave);
            // 
            // ask_exit_hitbox
            // 
            this.ask_exit_hitbox.AutoSize = true;
            this.ask_exit_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.ask_exit_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ask_exit_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ask_exit_hitbox.Location = new System.Drawing.Point(1640, 128);
            this.ask_exit_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ask_exit_hitbox.Name = "ask_exit_hitbox";
            this.ask_exit_hitbox.Padding = new System.Windows.Forms.Padding(280, 44, 0, 0);
            this.ask_exit_hitbox.Size = new System.Drawing.Size(280, 64);
            this.ask_exit_hitbox.TabIndex = 162;
            this.ask_exit_hitbox.Click += new System.EventHandler(this.ask_exit_Click);
            this.ask_exit_hitbox.MouseEnter += new System.EventHandler(this.ask_exit_MouseEnter);
            this.ask_exit_hitbox.MouseLeave += new System.EventHandler(this.ask_exit_MouseLeave);
            // 
            // bmp_32_hitbox
            // 
            this.bmp_32_hitbox.AutoSize = true;
            this.bmp_32_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.bmp_32_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.bmp_32_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.bmp_32_hitbox.Location = new System.Drawing.Point(1640, 192);
            this.bmp_32_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_32_hitbox.Name = "bmp_32_hitbox";
            this.bmp_32_hitbox.Padding = new System.Windows.Forms.Padding(280, 44, 0, 0);
            this.bmp_32_hitbox.Size = new System.Drawing.Size(280, 64);
            this.bmp_32_hitbox.TabIndex = 191;
            this.bmp_32_hitbox.Click += new System.EventHandler(this.bmp_32_Click);
            this.bmp_32_hitbox.MouseEnter += new System.EventHandler(this.bmp_32_MouseEnter);
            this.bmp_32_hitbox.MouseLeave += new System.EventHandler(this.bmp_32_MouseLeave);
            // 
            // FORCE_ALPHA_hitbox
            // 
            this.FORCE_ALPHA_hitbox.AutoSize = true;
            this.FORCE_ALPHA_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.FORCE_ALPHA_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.FORCE_ALPHA_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.FORCE_ALPHA_hitbox.Location = new System.Drawing.Point(1640, 256);
            this.FORCE_ALPHA_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.FORCE_ALPHA_hitbox.Name = "FORCE_ALPHA_hitbox";
            this.FORCE_ALPHA_hitbox.Padding = new System.Windows.Forms.Padding(280, 44, 0, 0);
            this.FORCE_ALPHA_hitbox.Size = new System.Drawing.Size(280, 64);
            this.FORCE_ALPHA_hitbox.TabIndex = 192;
            this.FORCE_ALPHA_hitbox.Click += new System.EventHandler(this.FORCE_ALPHA_Click);
            this.FORCE_ALPHA_hitbox.MouseEnter += new System.EventHandler(this.FORCE_ALPHA_MouseEnter);
            this.FORCE_ALPHA_hitbox.MouseLeave += new System.EventHandler(this.FORCE_ALPHA_MouseLeave);
            // 
            // funky_hitbox
            // 
            this.funky_hitbox.AutoSize = true;
            this.funky_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.funky_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.funky_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.funky_hitbox.Location = new System.Drawing.Point(1640, 320);
            this.funky_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.funky_hitbox.Name = "funky_hitbox";
            this.funky_hitbox.Padding = new System.Windows.Forms.Padding(280, 44, 0, 0);
            this.funky_hitbox.Size = new System.Drawing.Size(280, 64);
            this.funky_hitbox.TabIndex = 193;
            this.funky_hitbox.Click += new System.EventHandler(this.funky_Click);
            this.funky_hitbox.MouseEnter += new System.EventHandler(this.funky_MouseEnter);
            this.funky_hitbox.MouseLeave += new System.EventHandler(this.funky_MouseLeave);
            // 
            // no_warning_hitbox
            // 
            this.no_warning_hitbox.AutoSize = true;
            this.no_warning_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.no_warning_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.no_warning_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.no_warning_hitbox.Location = new System.Drawing.Point(1640, 384);
            this.no_warning_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.no_warning_hitbox.Name = "no_warning_hitbox";
            this.no_warning_hitbox.Padding = new System.Windows.Forms.Padding(280, 44, 0, 0);
            this.no_warning_hitbox.Size = new System.Drawing.Size(280, 64);
            this.no_warning_hitbox.TabIndex = 194;
            this.no_warning_hitbox.Click += new System.EventHandler(this.no_warning_Click);
            this.no_warning_hitbox.MouseEnter += new System.EventHandler(this.no_warning_MouseEnter);
            this.no_warning_hitbox.MouseLeave += new System.EventHandler(this.no_warning_MouseLeave);
            // 
            // random_hitbox
            // 
            this.random_hitbox.AutoSize = true;
            this.random_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.random_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.random_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.random_hitbox.Location = new System.Drawing.Point(1640, 448);
            this.random_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.random_hitbox.Name = "random_hitbox";
            this.random_hitbox.Padding = new System.Windows.Forms.Padding(300, 44, 0, 0);
            this.random_hitbox.Size = new System.Drawing.Size(300, 64);
            this.random_hitbox.TabIndex = 195;
            this.random_hitbox.Click += new System.EventHandler(this.random_Click);
            this.random_hitbox.MouseEnter += new System.EventHandler(this.random_MouseEnter);
            this.random_hitbox.MouseLeave += new System.EventHandler(this.random_MouseLeave);
            // 
            // reverse_hitbox
            // 
            this.reverse_hitbox.AutoSize = true;
            this.reverse_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.reverse_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.reverse_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.reverse_hitbox.Location = new System.Drawing.Point(1640, 512);
            this.reverse_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.reverse_hitbox.Name = "reverse_hitbox";
            this.reverse_hitbox.Padding = new System.Windows.Forms.Padding(280, 44, 0, 0);
            this.reverse_hitbox.Size = new System.Drawing.Size(280, 64);
            this.reverse_hitbox.TabIndex = 196;
            this.reverse_hitbox.Click += new System.EventHandler(this.reverse_Click);
            this.reverse_hitbox.MouseEnter += new System.EventHandler(this.reverse_MouseEnter);
            this.reverse_hitbox.MouseLeave += new System.EventHandler(this.reverse_MouseLeave);
            // 
            // safe_mode_hitbox
            // 
            this.safe_mode_hitbox.AutoSize = true;
            this.safe_mode_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.safe_mode_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.safe_mode_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.safe_mode_hitbox.Location = new System.Drawing.Point(1640, 576);
            this.safe_mode_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.safe_mode_hitbox.Name = "safe_mode_hitbox";
            this.safe_mode_hitbox.Padding = new System.Windows.Forms.Padding(280, 44, 0, 0);
            this.safe_mode_hitbox.Size = new System.Drawing.Size(280, 64);
            this.safe_mode_hitbox.TabIndex = 197;
            this.safe_mode_hitbox.Click += new System.EventHandler(this.safe_mode_Click);
            this.safe_mode_hitbox.MouseEnter += new System.EventHandler(this.safe_mode_MouseEnter);
            this.safe_mode_hitbox.MouseLeave += new System.EventHandler(this.safe_mode_MouseLeave);
            // 
            // stfu_hitbox
            // 
            this.stfu_hitbox.AutoSize = true;
            this.stfu_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.stfu_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.stfu_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.stfu_hitbox.Location = new System.Drawing.Point(1640, 640);
            this.stfu_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.stfu_hitbox.Name = "stfu_hitbox";
            this.stfu_hitbox.Padding = new System.Windows.Forms.Padding(280, 44, 0, 0);
            this.stfu_hitbox.Size = new System.Drawing.Size(280, 64);
            this.stfu_hitbox.TabIndex = 198;
            this.stfu_hitbox.Click += new System.EventHandler(this.stfu_Click);
            this.stfu_hitbox.MouseEnter += new System.EventHandler(this.stfu_MouseEnter);
            this.stfu_hitbox.MouseLeave += new System.EventHandler(this.stfu_MouseLeave);
            // 
            // warn_hitbox
            // 
            this.warn_hitbox.AutoSize = true;
            this.warn_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.warn_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.warn_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.warn_hitbox.Location = new System.Drawing.Point(1640, 704);
            this.warn_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.warn_hitbox.Name = "warn_hitbox";
            this.warn_hitbox.Padding = new System.Windows.Forms.Padding(280, 44, 0, 0);
            this.warn_hitbox.Size = new System.Drawing.Size(280, 64);
            this.warn_hitbox.TabIndex = 199;
            this.warn_hitbox.Click += new System.EventHandler(this.warn_Click);
            this.warn_hitbox.MouseEnter += new System.EventHandler(this.warn_MouseEnter);
            this.warn_hitbox.MouseLeave += new System.EventHandler(this.warn_MouseLeave);
            // 
            // cmpr_ck
            // 
            this.cmpr_ck.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmpr_ck.Enabled = false;
            this.cmpr_ck.ErrorImage = null;
            this.cmpr_ck.InitialImage = null;
            this.cmpr_ck.Location = new System.Drawing.Point(255, 769);
            this.cmpr_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_ck.Name = "cmpr_ck";
            this.cmpr_ck.Size = new System.Drawing.Size(64, 64);
            this.cmpr_ck.TabIndex = 233;
            this.cmpr_ck.TabStop = false;
            // 
            // cmpr_label
            // 
            this.cmpr_label.AutoSize = true;
            this.cmpr_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cmpr_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_label.Location = new System.Drawing.Point(323, 770);
            this.cmpr_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_label.Name = "cmpr_label";
            this.cmpr_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cmpr_label.Size = new System.Drawing.Size(79, 64);
            this.cmpr_label.TabIndex = 231;
            this.cmpr_label.Text = "CMPR";
            this.cmpr_label.Click += new System.EventHandler(this.CMPR_Click);
            this.cmpr_label.MouseEnter += new System.EventHandler(this.CMPR_MouseEnter);
            this.cmpr_label.MouseLeave += new System.EventHandler(this.CMPR_MouseLeave);
            // 
            // cmpr_hitbox
            // 
            this.cmpr_hitbox.AutoSize = true;
            this.cmpr_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cmpr_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_hitbox.Location = new System.Drawing.Point(252, 769);
            this.cmpr_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_hitbox.Name = "cmpr_hitbox";
            this.cmpr_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.cmpr_hitbox.Size = new System.Drawing.Size(190, 64);
            this.cmpr_hitbox.TabIndex = 232;
            this.cmpr_hitbox.Click += new System.EventHandler(this.CMPR_Click);
            this.cmpr_hitbox.MouseEnter += new System.EventHandler(this.CMPR_MouseEnter);
            this.cmpr_hitbox.MouseLeave += new System.EventHandler(this.CMPR_MouseLeave);
            // 
            // ci14x2_ck
            // 
            this.ci14x2_ck.BackColor = System.Drawing.Color.Transparent;
            this.ci14x2_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ci14x2_ck.Enabled = false;
            this.ci14x2_ck.ErrorImage = null;
            this.ci14x2_ck.InitialImage = null;
            this.ci14x2_ck.Location = new System.Drawing.Point(255, 705);
            this.ci14x2_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ci14x2_ck.Name = "ci14x2_ck";
            this.ci14x2_ck.Size = new System.Drawing.Size(64, 64);
            this.ci14x2_ck.TabIndex = 230;
            this.ci14x2_ck.TabStop = false;
            // 
            // ci14x2_label
            // 
            this.ci14x2_label.AutoSize = true;
            this.ci14x2_label.BackColor = System.Drawing.Color.Transparent;
            this.ci14x2_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ci14x2_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ci14x2_label.Location = new System.Drawing.Point(323, 705);
            this.ci14x2_label.Margin = new System.Windows.Forms.Padding(0);
            this.ci14x2_label.Name = "ci14x2_label";
            this.ci14x2_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.ci14x2_label.Size = new System.Drawing.Size(93, 64);
            this.ci14x2_label.TabIndex = 228;
            this.ci14x2_label.Text = "CI14x2";
            this.ci14x2_label.Click += new System.EventHandler(this.CI14X2_Click);
            this.ci14x2_label.MouseEnter += new System.EventHandler(this.CI14X2_MouseEnter);
            this.ci14x2_label.MouseLeave += new System.EventHandler(this.CI14X2_MouseLeave);
            // 
            // ci14x2_hitbox
            // 
            this.ci14x2_hitbox.AutoSize = true;
            this.ci14x2_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.ci14x2_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ci14x2_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ci14x2_hitbox.Location = new System.Drawing.Point(252, 705);
            this.ci14x2_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ci14x2_hitbox.Name = "ci14x2_hitbox";
            this.ci14x2_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.ci14x2_hitbox.Size = new System.Drawing.Size(190, 64);
            this.ci14x2_hitbox.TabIndex = 229;
            this.ci14x2_hitbox.Click += new System.EventHandler(this.CI14X2_Click);
            this.ci14x2_hitbox.MouseEnter += new System.EventHandler(this.CI14X2_MouseEnter);
            this.ci14x2_hitbox.MouseLeave += new System.EventHandler(this.CI14X2_MouseLeave);
            // 
            // ci8_ck
            // 
            this.ci8_ck.BackColor = System.Drawing.Color.Transparent;
            this.ci8_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ci8_ck.Enabled = false;
            this.ci8_ck.ErrorImage = null;
            this.ci8_ck.InitialImage = null;
            this.ci8_ck.Location = new System.Drawing.Point(255, 641);
            this.ci8_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ci8_ck.Name = "ci8_ck";
            this.ci8_ck.Size = new System.Drawing.Size(64, 64);
            this.ci8_ck.TabIndex = 227;
            this.ci8_ck.TabStop = false;
            // 
            // ci8_label
            // 
            this.ci8_label.AutoSize = true;
            this.ci8_label.BackColor = System.Drawing.Color.Transparent;
            this.ci8_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ci8_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ci8_label.Location = new System.Drawing.Point(323, 642);
            this.ci8_label.Margin = new System.Windows.Forms.Padding(0);
            this.ci8_label.Name = "ci8_label";
            this.ci8_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.ci8_label.Size = new System.Drawing.Size(48, 64);
            this.ci8_label.TabIndex = 225;
            this.ci8_label.Text = "CI8";
            this.ci8_label.Click += new System.EventHandler(this.CI8_Click);
            this.ci8_label.MouseEnter += new System.EventHandler(this.CI8_MouseEnter);
            this.ci8_label.MouseLeave += new System.EventHandler(this.CI8_MouseLeave);
            // 
            // ci8_hitbox
            // 
            this.ci8_hitbox.AutoSize = true;
            this.ci8_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.ci8_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ci8_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ci8_hitbox.Location = new System.Drawing.Point(252, 641);
            this.ci8_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ci8_hitbox.Name = "ci8_hitbox";
            this.ci8_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.ci8_hitbox.Size = new System.Drawing.Size(190, 64);
            this.ci8_hitbox.TabIndex = 226;
            this.ci8_hitbox.Click += new System.EventHandler(this.CI8_Click);
            this.ci8_hitbox.MouseEnter += new System.EventHandler(this.CI8_MouseEnter);
            this.ci8_hitbox.MouseLeave += new System.EventHandler(this.CI8_MouseLeave);
            // 
            // ci4_ck
            // 
            this.ci4_ck.BackColor = System.Drawing.Color.Transparent;
            this.ci4_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ci4_ck.Enabled = false;
            this.ci4_ck.ErrorImage = null;
            this.ci4_ck.InitialImage = null;
            this.ci4_ck.Location = new System.Drawing.Point(255, 577);
            this.ci4_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ci4_ck.Name = "ci4_ck";
            this.ci4_ck.Size = new System.Drawing.Size(64, 64);
            this.ci4_ck.TabIndex = 224;
            this.ci4_ck.TabStop = false;
            // 
            // ci4_label
            // 
            this.ci4_label.AutoSize = true;
            this.ci4_label.BackColor = System.Drawing.Color.Transparent;
            this.ci4_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ci4_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ci4_label.Location = new System.Drawing.Point(323, 578);
            this.ci4_label.Margin = new System.Windows.Forms.Padding(0);
            this.ci4_label.Name = "ci4_label";
            this.ci4_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.ci4_label.Size = new System.Drawing.Size(48, 64);
            this.ci4_label.TabIndex = 222;
            this.ci4_label.Text = "CI4";
            this.ci4_label.Click += new System.EventHandler(this.CI4_Click);
            this.ci4_label.MouseEnter += new System.EventHandler(this.CI4_MouseEnter);
            this.ci4_label.MouseLeave += new System.EventHandler(this.CI4_MouseLeave);
            // 
            // ci4_hitbox
            // 
            this.ci4_hitbox.AutoSize = true;
            this.ci4_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.ci4_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ci4_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ci4_hitbox.Location = new System.Drawing.Point(252, 577);
            this.ci4_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ci4_hitbox.Name = "ci4_hitbox";
            this.ci4_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.ci4_hitbox.Size = new System.Drawing.Size(190, 64);
            this.ci4_hitbox.TabIndex = 223;
            this.ci4_hitbox.Click += new System.EventHandler(this.CI4_Click);
            this.ci4_hitbox.MouseEnter += new System.EventHandler(this.CI4_MouseEnter);
            this.ci4_hitbox.MouseLeave += new System.EventHandler(this.CI4_MouseLeave);
            // 
            // rgba32_ck
            // 
            this.rgba32_ck.BackColor = System.Drawing.Color.Transparent;
            this.rgba32_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rgba32_ck.Enabled = false;
            this.rgba32_ck.ErrorImage = null;
            this.rgba32_ck.InitialImage = null;
            this.rgba32_ck.Location = new System.Drawing.Point(255, 512);
            this.rgba32_ck.Margin = new System.Windows.Forms.Padding(0);
            this.rgba32_ck.Name = "rgba32_ck";
            this.rgba32_ck.Size = new System.Drawing.Size(64, 64);
            this.rgba32_ck.TabIndex = 221;
            this.rgba32_ck.TabStop = false;
            // 
            // rgba32_label
            // 
            this.rgba32_label.AutoSize = true;
            this.rgba32_label.BackColor = System.Drawing.Color.Transparent;
            this.rgba32_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.rgba32_label.ForeColor = System.Drawing.SystemColors.Window;
            this.rgba32_label.Location = new System.Drawing.Point(323, 513);
            this.rgba32_label.Margin = new System.Windows.Forms.Padding(0);
            this.rgba32_label.Name = "rgba32_label";
            this.rgba32_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.rgba32_label.Size = new System.Drawing.Size(108, 64);
            this.rgba32_label.TabIndex = 219;
            this.rgba32_label.Text = "RGBA32";
            this.rgba32_label.Click += new System.EventHandler(this.RGBA32_Click);
            this.rgba32_label.MouseEnter += new System.EventHandler(this.RGBA32_MouseEnter);
            this.rgba32_label.MouseLeave += new System.EventHandler(this.RGBA32_MouseLeave);
            // 
            // rgba32_hitbox
            // 
            this.rgba32_hitbox.AutoSize = true;
            this.rgba32_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.rgba32_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.rgba32_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.rgba32_hitbox.Location = new System.Drawing.Point(252, 512);
            this.rgba32_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.rgba32_hitbox.Name = "rgba32_hitbox";
            this.rgba32_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.rgba32_hitbox.Size = new System.Drawing.Size(190, 64);
            this.rgba32_hitbox.TabIndex = 220;
            this.rgba32_hitbox.Click += new System.EventHandler(this.RGBA32_Click);
            this.rgba32_hitbox.MouseEnter += new System.EventHandler(this.RGBA32_MouseEnter);
            this.rgba32_hitbox.MouseLeave += new System.EventHandler(this.RGBA32_MouseLeave);
            // 
            // rgb5a3_ck
            // 
            this.rgb5a3_ck.BackColor = System.Drawing.Color.Transparent;
            this.rgb5a3_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rgb5a3_ck.Enabled = false;
            this.rgb5a3_ck.ErrorImage = null;
            this.rgb5a3_ck.InitialImage = null;
            this.rgb5a3_ck.Location = new System.Drawing.Point(255, 448);
            this.rgb5a3_ck.Margin = new System.Windows.Forms.Padding(0);
            this.rgb5a3_ck.Name = "rgb5a3_ck";
            this.rgb5a3_ck.Size = new System.Drawing.Size(64, 64);
            this.rgb5a3_ck.TabIndex = 218;
            this.rgb5a3_ck.TabStop = false;
            // 
            // rgb5a3_label
            // 
            this.rgb5a3_label.AutoSize = true;
            this.rgb5a3_label.BackColor = System.Drawing.Color.Transparent;
            this.rgb5a3_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.rgb5a3_label.ForeColor = System.Drawing.SystemColors.Window;
            this.rgb5a3_label.Location = new System.Drawing.Point(323, 449);
            this.rgb5a3_label.Margin = new System.Windows.Forms.Padding(0);
            this.rgb5a3_label.Name = "rgb5a3_label";
            this.rgb5a3_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.rgb5a3_label.Size = new System.Drawing.Size(108, 64);
            this.rgb5a3_label.TabIndex = 216;
            this.rgb5a3_label.Text = "RGB5A3";
            this.rgb5a3_label.Click += new System.EventHandler(this.RGB5A3_Click);
            this.rgb5a3_label.MouseEnter += new System.EventHandler(this.RGB5A3_MouseEnter);
            this.rgb5a3_label.MouseLeave += new System.EventHandler(this.RGB5A3_MouseLeave);
            // 
            // rgb5a3_hitbox
            // 
            this.rgb5a3_hitbox.AutoSize = true;
            this.rgb5a3_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.rgb5a3_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.rgb5a3_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.rgb5a3_hitbox.Location = new System.Drawing.Point(252, 448);
            this.rgb5a3_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.rgb5a3_hitbox.Name = "rgb5a3_hitbox";
            this.rgb5a3_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.rgb5a3_hitbox.Size = new System.Drawing.Size(190, 64);
            this.rgb5a3_hitbox.TabIndex = 217;
            this.rgb5a3_hitbox.Click += new System.EventHandler(this.RGB5A3_Click);
            this.rgb5a3_hitbox.MouseEnter += new System.EventHandler(this.RGB5A3_MouseEnter);
            this.rgb5a3_hitbox.MouseLeave += new System.EventHandler(this.RGB5A3_MouseLeave);
            // 
            // rgb565_ck
            // 
            this.rgb565_ck.BackColor = System.Drawing.Color.Transparent;
            this.rgb565_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rgb565_ck.Enabled = false;
            this.rgb565_ck.ErrorImage = null;
            this.rgb565_ck.InitialImage = null;
            this.rgb565_ck.Location = new System.Drawing.Point(255, 384);
            this.rgb565_ck.Margin = new System.Windows.Forms.Padding(0);
            this.rgb565_ck.Name = "rgb565_ck";
            this.rgb565_ck.Size = new System.Drawing.Size(64, 64);
            this.rgb565_ck.TabIndex = 215;
            this.rgb565_ck.TabStop = false;
            // 
            // rgb565_label
            // 
            this.rgb565_label.AutoSize = true;
            this.rgb565_label.BackColor = System.Drawing.Color.Transparent;
            this.rgb565_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.rgb565_label.ForeColor = System.Drawing.SystemColors.Window;
            this.rgb565_label.Location = new System.Drawing.Point(323, 384);
            this.rgb565_label.Margin = new System.Windows.Forms.Padding(0);
            this.rgb565_label.Name = "rgb565_label";
            this.rgb565_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.rgb565_label.Size = new System.Drawing.Size(107, 64);
            this.rgb565_label.TabIndex = 213;
            this.rgb565_label.Text = "RGB565";
            this.rgb565_label.Click += new System.EventHandler(this.RGB565_Click);
            this.rgb565_label.MouseEnter += new System.EventHandler(this.RGB565_MouseEnter);
            this.rgb565_label.MouseLeave += new System.EventHandler(this.RGB565_MouseLeave);
            // 
            // rgb565_hitbox
            // 
            this.rgb565_hitbox.AutoSize = true;
            this.rgb565_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.rgb565_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.rgb565_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.rgb565_hitbox.Location = new System.Drawing.Point(252, 384);
            this.rgb565_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.rgb565_hitbox.Name = "rgb565_hitbox";
            this.rgb565_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.rgb565_hitbox.Size = new System.Drawing.Size(190, 64);
            this.rgb565_hitbox.TabIndex = 214;
            this.rgb565_hitbox.Click += new System.EventHandler(this.RGB565_Click);
            this.rgb565_hitbox.MouseEnter += new System.EventHandler(this.RGB565_MouseEnter);
            this.rgb565_hitbox.MouseLeave += new System.EventHandler(this.RGB565_MouseLeave);
            // 
            // ai8_ck
            // 
            this.ai8_ck.BackColor = System.Drawing.Color.Transparent;
            this.ai8_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ai8_ck.Enabled = false;
            this.ai8_ck.ErrorImage = null;
            this.ai8_ck.InitialImage = null;
            this.ai8_ck.Location = new System.Drawing.Point(255, 320);
            this.ai8_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ai8_ck.Name = "ai8_ck";
            this.ai8_ck.Size = new System.Drawing.Size(64, 64);
            this.ai8_ck.TabIndex = 212;
            this.ai8_ck.TabStop = false;
            // 
            // ai8_label
            // 
            this.ai8_label.AutoSize = true;
            this.ai8_label.BackColor = System.Drawing.Color.Transparent;
            this.ai8_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ai8_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ai8_label.Location = new System.Drawing.Point(323, 321);
            this.ai8_label.Margin = new System.Windows.Forms.Padding(0);
            this.ai8_label.Name = "ai8_label";
            this.ai8_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.ai8_label.Size = new System.Drawing.Size(48, 64);
            this.ai8_label.TabIndex = 210;
            this.ai8_label.Text = "AI8";
            this.ai8_label.Click += new System.EventHandler(this.AI8_Click);
            this.ai8_label.MouseEnter += new System.EventHandler(this.AI8_MouseEnter);
            this.ai8_label.MouseLeave += new System.EventHandler(this.AI8_MouseLeave);
            // 
            // ai8_hitbox
            // 
            this.ai8_hitbox.AutoSize = true;
            this.ai8_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.ai8_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ai8_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ai8_hitbox.Location = new System.Drawing.Point(252, 320);
            this.ai8_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ai8_hitbox.Name = "ai8_hitbox";
            this.ai8_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.ai8_hitbox.Size = new System.Drawing.Size(190, 64);
            this.ai8_hitbox.TabIndex = 211;
            this.ai8_hitbox.Click += new System.EventHandler(this.AI8_Click);
            this.ai8_hitbox.MouseEnter += new System.EventHandler(this.AI8_MouseEnter);
            this.ai8_hitbox.MouseLeave += new System.EventHandler(this.AI8_MouseLeave);
            // 
            // ai4_ck
            // 
            this.ai4_ck.BackColor = System.Drawing.Color.Transparent;
            this.ai4_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ai4_ck.Enabled = false;
            this.ai4_ck.ErrorImage = null;
            this.ai4_ck.InitialImage = null;
            this.ai4_ck.Location = new System.Drawing.Point(255, 256);
            this.ai4_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ai4_ck.Name = "ai4_ck";
            this.ai4_ck.Size = new System.Drawing.Size(64, 64);
            this.ai4_ck.TabIndex = 209;
            this.ai4_ck.TabStop = false;
            // 
            // ai4_label
            // 
            this.ai4_label.AutoSize = true;
            this.ai4_label.BackColor = System.Drawing.Color.Transparent;
            this.ai4_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ai4_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ai4_label.Location = new System.Drawing.Point(323, 257);
            this.ai4_label.Margin = new System.Windows.Forms.Padding(0);
            this.ai4_label.Name = "ai4_label";
            this.ai4_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.ai4_label.Size = new System.Drawing.Size(48, 64);
            this.ai4_label.TabIndex = 207;
            this.ai4_label.Text = "AI4";
            this.ai4_label.Click += new System.EventHandler(this.AI4_Click);
            this.ai4_label.MouseEnter += new System.EventHandler(this.AI4_MouseEnter);
            this.ai4_label.MouseLeave += new System.EventHandler(this.AI4_MouseLeave);
            // 
            // ai4_hitbox
            // 
            this.ai4_hitbox.AutoSize = true;
            this.ai4_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.ai4_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ai4_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ai4_hitbox.Location = new System.Drawing.Point(252, 256);
            this.ai4_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ai4_hitbox.Name = "ai4_hitbox";
            this.ai4_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.ai4_hitbox.Size = new System.Drawing.Size(190, 64);
            this.ai4_hitbox.TabIndex = 208;
            this.ai4_hitbox.Click += new System.EventHandler(this.AI4_Click);
            this.ai4_hitbox.MouseEnter += new System.EventHandler(this.AI4_MouseEnter);
            this.ai4_hitbox.MouseLeave += new System.EventHandler(this.AI4_MouseLeave);
            // 
            // i8_ck
            // 
            this.i8_ck.BackColor = System.Drawing.Color.Transparent;
            this.i8_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.i8_ck.Enabled = false;
            this.i8_ck.ErrorImage = null;
            this.i8_ck.InitialImage = null;
            this.i8_ck.Location = new System.Drawing.Point(255, 192);
            this.i8_ck.Margin = new System.Windows.Forms.Padding(0);
            this.i8_ck.Name = "i8_ck";
            this.i8_ck.Size = new System.Drawing.Size(64, 64);
            this.i8_ck.TabIndex = 206;
            this.i8_ck.TabStop = false;
            // 
            // i8_label
            // 
            this.i8_label.AutoSize = true;
            this.i8_label.BackColor = System.Drawing.Color.Transparent;
            this.i8_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.i8_label.ForeColor = System.Drawing.SystemColors.Window;
            this.i8_label.Location = new System.Drawing.Point(323, 192);
            this.i8_label.Margin = new System.Windows.Forms.Padding(0);
            this.i8_label.Name = "i8_label";
            this.i8_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.i8_label.Size = new System.Drawing.Size(31, 64);
            this.i8_label.TabIndex = 204;
            this.i8_label.Text = "I8";
            this.i8_label.Click += new System.EventHandler(this.I8_Click);
            this.i8_label.MouseEnter += new System.EventHandler(this.I8_MouseEnter);
            this.i8_label.MouseLeave += new System.EventHandler(this.I8_MouseLeave);
            // 
            // i8_hitbox
            // 
            this.i8_hitbox.AutoSize = true;
            this.i8_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.i8_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.i8_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.i8_hitbox.Location = new System.Drawing.Point(252, 192);
            this.i8_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.i8_hitbox.Name = "i8_hitbox";
            this.i8_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.i8_hitbox.Size = new System.Drawing.Size(190, 64);
            this.i8_hitbox.TabIndex = 205;
            this.i8_hitbox.Click += new System.EventHandler(this.I8_Click);
            this.i8_hitbox.MouseEnter += new System.EventHandler(this.I8_MouseEnter);
            this.i8_hitbox.MouseLeave += new System.EventHandler(this.I8_MouseLeave);
            // 
            // i4_ck
            // 
            this.i4_ck.BackColor = System.Drawing.Color.Transparent;
            this.i4_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.i4_ck.Enabled = false;
            this.i4_ck.ErrorImage = null;
            this.i4_ck.InitialImage = null;
            this.i4_ck.Location = new System.Drawing.Point(255, 128);
            this.i4_ck.Margin = new System.Windows.Forms.Padding(0);
            this.i4_ck.Name = "i4_ck";
            this.i4_ck.Size = new System.Drawing.Size(64, 64);
            this.i4_ck.TabIndex = 203;
            this.i4_ck.TabStop = false;
            // 
            // i4_label
            // 
            this.i4_label.AutoSize = true;
            this.i4_label.BackColor = System.Drawing.Color.Transparent;
            this.i4_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.i4_label.ForeColor = System.Drawing.SystemColors.Window;
            this.i4_label.Location = new System.Drawing.Point(323, 128);
            this.i4_label.Margin = new System.Windows.Forms.Padding(0);
            this.i4_label.Name = "i4_label";
            this.i4_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.i4_label.Size = new System.Drawing.Size(31, 64);
            this.i4_label.TabIndex = 201;
            this.i4_label.Text = "I4";
            this.i4_label.Click += new System.EventHandler(this.I4_Click);
            this.i4_label.MouseEnter += new System.EventHandler(this.I4_MouseEnter);
            this.i4_label.MouseLeave += new System.EventHandler(this.I4_MouseLeave);
            // 
            // i4_hitbox
            // 
            this.i4_hitbox.AutoSize = true;
            this.i4_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.i4_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.i4_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.i4_hitbox.Location = new System.Drawing.Point(252, 128);
            this.i4_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.i4_hitbox.Name = "i4_hitbox";
            this.i4_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.i4_hitbox.Size = new System.Drawing.Size(190, 64);
            this.i4_hitbox.TabIndex = 202;
            this.i4_hitbox.Click += new System.EventHandler(this.I4_Click);
            this.i4_hitbox.MouseEnter += new System.EventHandler(this.I4_MouseEnter);
            this.i4_hitbox.MouseLeave += new System.EventHandler(this.I4_MouseLeave);
            // 
            // encoding_label
            // 
            this.encoding_label.AutoSize = true;
            this.encoding_label.BackColor = System.Drawing.Color.Transparent;
            this.encoding_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.encoding_label.ForeColor = System.Drawing.SystemColors.Control;
            this.encoding_label.Location = new System.Drawing.Point(278, 95);
            this.encoding_label.Name = "encoding_label";
            this.encoding_label.Size = new System.Drawing.Size(113, 20);
            this.encoding_label.TabIndex = 200;
            this.encoding_label.Text = "Encoding";
            // 
            // surrounding_ck
            // 
            this.surrounding_ck.BackColor = System.Drawing.Color.Transparent;
            this.surrounding_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.surrounding_ck.Enabled = false;
            this.surrounding_ck.ErrorImage = null;
            this.surrounding_ck.InitialImage = null;
            this.surrounding_ck.Location = new System.Drawing.Point(9, 65);
            this.surrounding_ck.Margin = new System.Windows.Forms.Padding(0);
            this.surrounding_ck.Name = "surrounding_ck";
            this.surrounding_ck.Size = new System.Drawing.Size(453, 840);
            this.surrounding_ck.TabIndex = 234;
            this.surrounding_ck.TabStop = false;
            // 
            // no_gradient_ck
            // 
            this.no_gradient_ck.BackColor = System.Drawing.Color.Transparent;
            this.no_gradient_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.no_gradient_ck.Enabled = false;
            this.no_gradient_ck.ErrorImage = null;
            this.no_gradient_ck.InitialImage = null;
            this.no_gradient_ck.Location = new System.Drawing.Point(503, 320);
            this.no_gradient_ck.Margin = new System.Windows.Forms.Padding(0);
            this.no_gradient_ck.Name = "no_gradient_ck";
            this.no_gradient_ck.Size = new System.Drawing.Size(64, 64);
            this.no_gradient_ck.TabIndex = 247;
            this.no_gradient_ck.TabStop = false;
            // 
            // no_gradient_label
            // 
            this.no_gradient_label.AutoSize = true;
            this.no_gradient_label.BackColor = System.Drawing.Color.Transparent;
            this.no_gradient_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.no_gradient_label.ForeColor = System.Drawing.SystemColors.Window;
            this.no_gradient_label.Location = new System.Drawing.Point(571, 320);
            this.no_gradient_label.Margin = new System.Windows.Forms.Padding(0);
            this.no_gradient_label.Name = "no_gradient_label";
            this.no_gradient_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.no_gradient_label.Size = new System.Drawing.Size(148, 64);
            this.no_gradient_label.TabIndex = 245;
            this.no_gradient_label.Text = "No Gradient";
            this.no_gradient_label.Click += new System.EventHandler(this.No_gradient_Click);
            this.no_gradient_label.MouseEnter += new System.EventHandler(this.No_gradient_MouseEnter);
            this.no_gradient_label.MouseLeave += new System.EventHandler(this.No_gradient_MouseLeave);
            // 
            // no_gradient_hitbox
            // 
            this.no_gradient_hitbox.AutoSize = true;
            this.no_gradient_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.no_gradient_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.no_gradient_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.no_gradient_hitbox.Location = new System.Drawing.Point(500, 320);
            this.no_gradient_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.no_gradient_hitbox.Name = "no_gradient_hitbox";
            this.no_gradient_hitbox.Padding = new System.Windows.Forms.Padding(250, 44, 0, 0);
            this.no_gradient_hitbox.Size = new System.Drawing.Size(250, 64);
            this.no_gradient_hitbox.TabIndex = 246;
            this.no_gradient_hitbox.Click += new System.EventHandler(this.No_gradient_Click);
            this.no_gradient_hitbox.MouseEnter += new System.EventHandler(this.No_gradient_MouseEnter);
            this.no_gradient_hitbox.MouseLeave += new System.EventHandler(this.No_gradient_MouseLeave);
            // 
            // custom_ck
            // 
            this.custom_ck.BackColor = System.Drawing.Color.Transparent;
            this.custom_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.custom_ck.Enabled = false;
            this.custom_ck.ErrorImage = null;
            this.custom_ck.InitialImage = null;
            this.custom_ck.Location = new System.Drawing.Point(503, 256);
            this.custom_ck.Margin = new System.Windows.Forms.Padding(0);
            this.custom_ck.Name = "custom_ck";
            this.custom_ck.Size = new System.Drawing.Size(64, 64);
            this.custom_ck.TabIndex = 244;
            this.custom_ck.TabStop = false;
            // 
            // custom_label
            // 
            this.custom_label.AutoSize = true;
            this.custom_label.BackColor = System.Drawing.Color.Transparent;
            this.custom_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.custom_label.ForeColor = System.Drawing.SystemColors.Window;
            this.custom_label.Location = new System.Drawing.Point(571, 256);
            this.custom_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_label.Name = "custom_label";
            this.custom_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.custom_label.Size = new System.Drawing.Size(174, 64);
            this.custom_label.TabIndex = 242;
            this.custom_label.Text = "Custom RGBA";
            this.custom_label.Click += new System.EventHandler(this.Custom_Click);
            this.custom_label.MouseEnter += new System.EventHandler(this.Custom_MouseEnter);
            this.custom_label.MouseLeave += new System.EventHandler(this.Custom_MouseLeave);
            // 
            // custom_hitbox
            // 
            this.custom_hitbox.AutoSize = true;
            this.custom_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.custom_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.custom_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_hitbox.Location = new System.Drawing.Point(500, 256);
            this.custom_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.custom_hitbox.Name = "custom_hitbox";
            this.custom_hitbox.Padding = new System.Windows.Forms.Padding(250, 44, 0, 0);
            this.custom_hitbox.Size = new System.Drawing.Size(250, 64);
            this.custom_hitbox.TabIndex = 243;
            this.custom_hitbox.Click += new System.EventHandler(this.Custom_Click);
            this.custom_hitbox.MouseEnter += new System.EventHandler(this.Custom_MouseEnter);
            this.custom_hitbox.MouseLeave += new System.EventHandler(this.Custom_MouseLeave);
            // 
            // cie_709_ck
            // 
            this.cie_709_ck.BackColor = System.Drawing.Color.Transparent;
            this.cie_709_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cie_709_ck.Enabled = false;
            this.cie_709_ck.ErrorImage = null;
            this.cie_709_ck.InitialImage = null;
            this.cie_709_ck.Location = new System.Drawing.Point(503, 192);
            this.cie_709_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cie_709_ck.Name = "cie_709_ck";
            this.cie_709_ck.Size = new System.Drawing.Size(64, 64);
            this.cie_709_ck.TabIndex = 241;
            this.cie_709_ck.TabStop = false;
            // 
            // cie_709_label
            // 
            this.cie_709_label.AutoSize = true;
            this.cie_709_label.BackColor = System.Drawing.Color.Transparent;
            this.cie_709_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cie_709_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cie_709_label.Location = new System.Drawing.Point(571, 192);
            this.cie_709_label.Margin = new System.Windows.Forms.Padding(0);
            this.cie_709_label.Name = "cie_709_label";
            this.cie_709_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cie_709_label.Size = new System.Drawing.Size(105, 64);
            this.cie_709_label.TabIndex = 239;
            this.cie_709_label.Text = "CIE 709";
            this.cie_709_label.Click += new System.EventHandler(this.Cie_709_Click);
            this.cie_709_label.MouseEnter += new System.EventHandler(this.Cie_709_MouseEnter);
            this.cie_709_label.MouseLeave += new System.EventHandler(this.Cie_709_MouseLeave);
            // 
            // cie_709_hitbox
            // 
            this.cie_709_hitbox.AutoSize = true;
            this.cie_709_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.cie_709_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cie_709_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.cie_709_hitbox.Location = new System.Drawing.Point(500, 192);
            this.cie_709_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.cie_709_hitbox.Name = "cie_709_hitbox";
            this.cie_709_hitbox.Padding = new System.Windows.Forms.Padding(250, 44, 0, 0);
            this.cie_709_hitbox.Size = new System.Drawing.Size(250, 64);
            this.cie_709_hitbox.TabIndex = 240;
            this.cie_709_hitbox.Click += new System.EventHandler(this.Cie_709_Click);
            this.cie_709_hitbox.MouseEnter += new System.EventHandler(this.Cie_709_MouseEnter);
            this.cie_709_hitbox.MouseLeave += new System.EventHandler(this.Cie_709_MouseLeave);
            // 
            // cie_601_ck
            // 
            this.cie_601_ck.BackColor = System.Drawing.Color.Transparent;
            this.cie_601_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cie_601_ck.Enabled = false;
            this.cie_601_ck.ErrorImage = null;
            this.cie_601_ck.InitialImage = null;
            this.cie_601_ck.Location = new System.Drawing.Point(503, 128);
            this.cie_601_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cie_601_ck.Name = "cie_601_ck";
            this.cie_601_ck.Size = new System.Drawing.Size(64, 64);
            this.cie_601_ck.TabIndex = 238;
            this.cie_601_ck.TabStop = false;
            // 
            // cie_601_label
            // 
            this.cie_601_label.AutoSize = true;
            this.cie_601_label.BackColor = System.Drawing.Color.Transparent;
            this.cie_601_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cie_601_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cie_601_label.Location = new System.Drawing.Point(571, 128);
            this.cie_601_label.Margin = new System.Windows.Forms.Padding(0);
            this.cie_601_label.Name = "cie_601_label";
            this.cie_601_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cie_601_label.Size = new System.Drawing.Size(105, 64);
            this.cie_601_label.TabIndex = 236;
            this.cie_601_label.Text = "CIE 601";
            this.cie_601_label.Click += new System.EventHandler(this.Cie_601_Click);
            this.cie_601_label.MouseEnter += new System.EventHandler(this.Cie_601_MouseEnter);
            this.cie_601_label.MouseLeave += new System.EventHandler(this.Cie_601_MouseLeave);
            // 
            // cie_601_hitbox
            // 
            this.cie_601_hitbox.AutoSize = true;
            this.cie_601_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.cie_601_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cie_601_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.cie_601_hitbox.Location = new System.Drawing.Point(500, 128);
            this.cie_601_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.cie_601_hitbox.Name = "cie_601_hitbox";
            this.cie_601_hitbox.Padding = new System.Windows.Forms.Padding(250, 44, 0, 0);
            this.cie_601_hitbox.Size = new System.Drawing.Size(250, 64);
            this.cie_601_hitbox.TabIndex = 237;
            this.cie_601_hitbox.Click += new System.EventHandler(this.Cie_601_Click);
            this.cie_601_hitbox.MouseEnter += new System.EventHandler(this.Cie_601_MouseEnter);
            this.cie_601_hitbox.MouseLeave += new System.EventHandler(this.Cie_601_MouseLeave);
            // 
            // algorithm_label
            // 
            this.algorithm_label.AutoSize = true;
            this.algorithm_label.BackColor = System.Drawing.Color.Transparent;
            this.algorithm_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.algorithm_label.ForeColor = System.Drawing.SystemColors.Control;
            this.algorithm_label.Location = new System.Drawing.Point(544, 95);
            this.algorithm_label.Name = "algorithm_label";
            this.algorithm_label.Size = new System.Drawing.Size(119, 20);
            this.algorithm_label.TabIndex = 235;
            this.algorithm_label.Text = "Algorithm";
            // 
            // mix_ck
            // 
            this.mix_ck.BackColor = System.Drawing.Color.Transparent;
            this.mix_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mix_ck.Enabled = false;
            this.mix_ck.ErrorImage = null;
            this.mix_ck.InitialImage = null;
            this.mix_ck.Location = new System.Drawing.Point(503, 557);
            this.mix_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mix_ck.Name = "mix_ck";
            this.mix_ck.Size = new System.Drawing.Size(64, 64);
            this.mix_ck.TabIndex = 257;
            this.mix_ck.TabStop = false;
            // 
            // mix_label
            // 
            this.mix_label.AutoSize = true;
            this.mix_label.BackColor = System.Drawing.Color.Transparent;
            this.mix_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mix_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mix_label.Location = new System.Drawing.Point(571, 557);
            this.mix_label.Margin = new System.Windows.Forms.Padding(0);
            this.mix_label.Name = "mix_label";
            this.mix_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mix_label.Size = new System.Drawing.Size(47, 64);
            this.mix_label.TabIndex = 255;
            this.mix_label.Text = "Mix";
            this.mix_label.Click += new System.EventHandler(this.Mix_Click);
            this.mix_label.MouseEnter += new System.EventHandler(this.Mix_MouseEnter);
            this.mix_label.MouseLeave += new System.EventHandler(this.Mix_MouseLeave);
            // 
            // mix_hitbox
            // 
            this.mix_hitbox.AutoSize = true;
            this.mix_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mix_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mix_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mix_hitbox.Location = new System.Drawing.Point(500, 557);
            this.mix_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mix_hitbox.Name = "mix_hitbox";
            this.mix_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.mix_hitbox.Size = new System.Drawing.Size(190, 64);
            this.mix_hitbox.TabIndex = 256;
            this.mix_hitbox.Click += new System.EventHandler(this.Mix_Click);
            this.mix_hitbox.MouseEnter += new System.EventHandler(this.Mix_MouseEnter);
            this.mix_hitbox.MouseLeave += new System.EventHandler(this.Mix_MouseLeave);
            // 
            // alpha_ck
            // 
            this.alpha_ck.BackColor = System.Drawing.Color.Transparent;
            this.alpha_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.alpha_ck.Enabled = false;
            this.alpha_ck.ErrorImage = null;
            this.alpha_ck.InitialImage = null;
            this.alpha_ck.Location = new System.Drawing.Point(503, 493);
            this.alpha_ck.Margin = new System.Windows.Forms.Padding(0);
            this.alpha_ck.Name = "alpha_ck";
            this.alpha_ck.Size = new System.Drawing.Size(64, 64);
            this.alpha_ck.TabIndex = 254;
            this.alpha_ck.TabStop = false;
            // 
            // alpha_label
            // 
            this.alpha_label.AutoSize = true;
            this.alpha_label.BackColor = System.Drawing.Color.Transparent;
            this.alpha_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.alpha_label.ForeColor = System.Drawing.SystemColors.Window;
            this.alpha_label.Location = new System.Drawing.Point(571, 493);
            this.alpha_label.Margin = new System.Windows.Forms.Padding(0);
            this.alpha_label.Name = "alpha_label";
            this.alpha_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.alpha_label.Size = new System.Drawing.Size(74, 64);
            this.alpha_label.TabIndex = 252;
            this.alpha_label.Text = "Alpha";
            this.alpha_label.Click += new System.EventHandler(this.Alpha_Click);
            this.alpha_label.MouseEnter += new System.EventHandler(this.Alpha_MouseEnter);
            this.alpha_label.MouseLeave += new System.EventHandler(this.Alpha_MouseLeave);
            // 
            // alpha_hitbox
            // 
            this.alpha_hitbox.AutoSize = true;
            this.alpha_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.alpha_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.alpha_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.alpha_hitbox.Location = new System.Drawing.Point(500, 493);
            this.alpha_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.alpha_hitbox.Name = "alpha_hitbox";
            this.alpha_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.alpha_hitbox.Size = new System.Drawing.Size(190, 64);
            this.alpha_hitbox.TabIndex = 253;
            this.alpha_hitbox.Click += new System.EventHandler(this.Alpha_Click);
            this.alpha_hitbox.MouseEnter += new System.EventHandler(this.Alpha_MouseEnter);
            this.alpha_hitbox.MouseLeave += new System.EventHandler(this.Alpha_MouseLeave);
            // 
            // no_alpha_ck
            // 
            this.no_alpha_ck.BackColor = System.Drawing.Color.Transparent;
            this.no_alpha_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.no_alpha_ck.Enabled = false;
            this.no_alpha_ck.ErrorImage = null;
            this.no_alpha_ck.InitialImage = null;
            this.no_alpha_ck.Location = new System.Drawing.Point(503, 429);
            this.no_alpha_ck.Margin = new System.Windows.Forms.Padding(0);
            this.no_alpha_ck.Name = "no_alpha_ck";
            this.no_alpha_ck.Size = new System.Drawing.Size(64, 64);
            this.no_alpha_ck.TabIndex = 251;
            this.no_alpha_ck.TabStop = false;
            // 
            // no_alpha_label
            // 
            this.no_alpha_label.AutoSize = true;
            this.no_alpha_label.BackColor = System.Drawing.Color.Transparent;
            this.no_alpha_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.no_alpha_label.ForeColor = System.Drawing.SystemColors.Window;
            this.no_alpha_label.Location = new System.Drawing.Point(571, 429);
            this.no_alpha_label.Margin = new System.Windows.Forms.Padding(0);
            this.no_alpha_label.Name = "no_alpha_label";
            this.no_alpha_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.no_alpha_label.Size = new System.Drawing.Size(115, 64);
            this.no_alpha_label.TabIndex = 249;
            this.no_alpha_label.Text = "No Alpha";
            this.no_alpha_label.Click += new System.EventHandler(this.No_alpha_Click);
            this.no_alpha_label.MouseEnter += new System.EventHandler(this.No_alpha_MouseEnter);
            this.no_alpha_label.MouseLeave += new System.EventHandler(this.No_alpha_MouseLeave);
            // 
            // no_alpha_hitbox
            // 
            this.no_alpha_hitbox.AutoSize = true;
            this.no_alpha_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.no_alpha_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.no_alpha_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.no_alpha_hitbox.Location = new System.Drawing.Point(500, 429);
            this.no_alpha_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.no_alpha_hitbox.Name = "no_alpha_hitbox";
            this.no_alpha_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.no_alpha_hitbox.Size = new System.Drawing.Size(190, 64);
            this.no_alpha_hitbox.TabIndex = 250;
            this.no_alpha_hitbox.Click += new System.EventHandler(this.No_alpha_Click);
            this.no_alpha_hitbox.MouseEnter += new System.EventHandler(this.No_alpha_MouseEnter);
            this.no_alpha_hitbox.MouseLeave += new System.EventHandler(this.No_alpha_MouseLeave);
            // 
            // alpha_title
            // 
            this.alpha_title.AutoSize = true;
            this.alpha_title.BackColor = System.Drawing.Color.Transparent;
            this.alpha_title.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.alpha_title.ForeColor = System.Drawing.SystemColors.Control;
            this.alpha_title.Location = new System.Drawing.Point(544, 397);
            this.alpha_title.Margin = new System.Windows.Forms.Padding(0);
            this.alpha_title.Name = "alpha_title";
            this.alpha_title.Size = new System.Drawing.Size(74, 20);
            this.alpha_title.TabIndex = 248;
            this.alpha_title.Text = "Alpha";
            // 
            // Tmirror_ck
            // 
            this.Tmirror_ck.BackColor = System.Drawing.Color.Transparent;
            this.Tmirror_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Tmirror_ck.Enabled = false;
            this.Tmirror_ck.ErrorImage = null;
            this.Tmirror_ck.InitialImage = null;
            this.Tmirror_ck.Location = new System.Drawing.Point(1440, 704);
            this.Tmirror_ck.Margin = new System.Windows.Forms.Padding(0);
            this.Tmirror_ck.Name = "Tmirror_ck";
            this.Tmirror_ck.Size = new System.Drawing.Size(64, 64);
            this.Tmirror_ck.TabIndex = 267;
            this.Tmirror_ck.TabStop = false;
            // 
            // Tmirror_label
            // 
            this.Tmirror_label.AutoSize = true;
            this.Tmirror_label.BackColor = System.Drawing.Color.Transparent;
            this.Tmirror_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Tmirror_label.ForeColor = System.Drawing.SystemColors.Window;
            this.Tmirror_label.Location = new System.Drawing.Point(1508, 705);
            this.Tmirror_label.Margin = new System.Windows.Forms.Padding(0);
            this.Tmirror_label.Name = "Tmirror_label";
            this.Tmirror_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Tmirror_label.Size = new System.Drawing.Size(78, 64);
            this.Tmirror_label.TabIndex = 265;
            this.Tmirror_label.Text = "Mirror";
            this.Tmirror_label.Click += new System.EventHandler(this.WrapT_Mirror_Click);
            this.Tmirror_label.MouseEnter += new System.EventHandler(this.WrapT_Mirror_MouseEnter);
            this.Tmirror_label.MouseLeave += new System.EventHandler(this.WrapT_Mirror_MouseLeave);
            // 
            // Tmirror_hitbox
            // 
            this.Tmirror_hitbox.AutoSize = true;
            this.Tmirror_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.Tmirror_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Tmirror_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.Tmirror_hitbox.Location = new System.Drawing.Point(1437, 704);
            this.Tmirror_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.Tmirror_hitbox.Name = "Tmirror_hitbox";
            this.Tmirror_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.Tmirror_hitbox.Size = new System.Drawing.Size(190, 64);
            this.Tmirror_hitbox.TabIndex = 266;
            this.Tmirror_hitbox.Click += new System.EventHandler(this.WrapT_Mirror_Click);
            this.Tmirror_hitbox.MouseEnter += new System.EventHandler(this.WrapT_Mirror_MouseEnter);
            this.Tmirror_hitbox.MouseLeave += new System.EventHandler(this.WrapT_Mirror_MouseLeave);
            // 
            // Trepeat_ck
            // 
            this.Trepeat_ck.BackColor = System.Drawing.Color.Transparent;
            this.Trepeat_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Trepeat_ck.Enabled = false;
            this.Trepeat_ck.ErrorImage = null;
            this.Trepeat_ck.InitialImage = null;
            this.Trepeat_ck.Location = new System.Drawing.Point(1440, 640);
            this.Trepeat_ck.Margin = new System.Windows.Forms.Padding(0);
            this.Trepeat_ck.Name = "Trepeat_ck";
            this.Trepeat_ck.Size = new System.Drawing.Size(64, 64);
            this.Trepeat_ck.TabIndex = 264;
            this.Trepeat_ck.TabStop = false;
            // 
            // Trepeat_label
            // 
            this.Trepeat_label.AutoSize = true;
            this.Trepeat_label.BackColor = System.Drawing.Color.Transparent;
            this.Trepeat_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Trepeat_label.ForeColor = System.Drawing.SystemColors.Window;
            this.Trepeat_label.Location = new System.Drawing.Point(1508, 640);
            this.Trepeat_label.Margin = new System.Windows.Forms.Padding(0);
            this.Trepeat_label.Name = "Trepeat_label";
            this.Trepeat_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Trepeat_label.Size = new System.Drawing.Size(92, 64);
            this.Trepeat_label.TabIndex = 262;
            this.Trepeat_label.Text = "Repeat";
            this.Trepeat_label.Click += new System.EventHandler(this.WrapT_Repeat_Click);
            this.Trepeat_label.MouseEnter += new System.EventHandler(this.WrapT_Repeat_MouseEnter);
            this.Trepeat_label.MouseLeave += new System.EventHandler(this.WrapT_Repeat_MouseLeave);
            // 
            // Trepeat_hitbox
            // 
            this.Trepeat_hitbox.AutoSize = true;
            this.Trepeat_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.Trepeat_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Trepeat_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.Trepeat_hitbox.Location = new System.Drawing.Point(1437, 640);
            this.Trepeat_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.Trepeat_hitbox.Name = "Trepeat_hitbox";
            this.Trepeat_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.Trepeat_hitbox.Size = new System.Drawing.Size(190, 64);
            this.Trepeat_hitbox.TabIndex = 263;
            this.Trepeat_hitbox.Click += new System.EventHandler(this.WrapT_Repeat_Click);
            this.Trepeat_hitbox.MouseEnter += new System.EventHandler(this.WrapT_Repeat_MouseEnter);
            this.Trepeat_hitbox.MouseLeave += new System.EventHandler(this.WrapT_Repeat_MouseLeave);
            // 
            // Tclamp_ck
            // 
            this.Tclamp_ck.BackColor = System.Drawing.Color.Transparent;
            this.Tclamp_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Tclamp_ck.Enabled = false;
            this.Tclamp_ck.ErrorImage = null;
            this.Tclamp_ck.InitialImage = null;
            this.Tclamp_ck.Location = new System.Drawing.Point(1440, 576);
            this.Tclamp_ck.Margin = new System.Windows.Forms.Padding(0);
            this.Tclamp_ck.Name = "Tclamp_ck";
            this.Tclamp_ck.Size = new System.Drawing.Size(64, 64);
            this.Tclamp_ck.TabIndex = 261;
            this.Tclamp_ck.TabStop = false;
            // 
            // Tclamp_label
            // 
            this.Tclamp_label.AutoSize = true;
            this.Tclamp_label.BackColor = System.Drawing.Color.Transparent;
            this.Tclamp_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Tclamp_label.ForeColor = System.Drawing.SystemColors.Window;
            this.Tclamp_label.Location = new System.Drawing.Point(1508, 576);
            this.Tclamp_label.Margin = new System.Windows.Forms.Padding(0);
            this.Tclamp_label.Name = "Tclamp_label";
            this.Tclamp_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Tclamp_label.Size = new System.Drawing.Size(80, 64);
            this.Tclamp_label.TabIndex = 259;
            this.Tclamp_label.Text = "Clamp";
            this.Tclamp_label.Click += new System.EventHandler(this.WrapT_Clamp_Click);
            this.Tclamp_label.MouseEnter += new System.EventHandler(this.WrapT_Clamp_MouseEnter);
            this.Tclamp_label.MouseLeave += new System.EventHandler(this.WrapT_Clamp_MouseLeave);
            // 
            // Tclamp_hitbox
            // 
            this.Tclamp_hitbox.AutoSize = true;
            this.Tclamp_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.Tclamp_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Tclamp_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.Tclamp_hitbox.Location = new System.Drawing.Point(1437, 576);
            this.Tclamp_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.Tclamp_hitbox.Name = "Tclamp_hitbox";
            this.Tclamp_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.Tclamp_hitbox.Size = new System.Drawing.Size(190, 64);
            this.Tclamp_hitbox.TabIndex = 260;
            this.Tclamp_hitbox.Click += new System.EventHandler(this.WrapT_Clamp_Click);
            this.Tclamp_hitbox.MouseEnter += new System.EventHandler(this.WrapT_Clamp_MouseEnter);
            this.Tclamp_hitbox.MouseLeave += new System.EventHandler(this.WrapT_Clamp_MouseLeave);
            // 
            // WrapT_label
            // 
            this.WrapT_label.AutoSize = true;
            this.WrapT_label.BackColor = System.Drawing.Color.Transparent;
            this.WrapT_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.WrapT_label.ForeColor = System.Drawing.SystemColors.Control;
            this.WrapT_label.Location = new System.Drawing.Point(1488, 546);
            this.WrapT_label.Name = "WrapT_label";
            this.WrapT_label.Size = new System.Drawing.Size(83, 20);
            this.WrapT_label.TabIndex = 258;
            this.WrapT_label.Text = "WrapT";
            // 
            // Smirror_ck
            // 
            this.Smirror_ck.BackColor = System.Drawing.Color.Transparent;
            this.Smirror_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Smirror_ck.Enabled = false;
            this.Smirror_ck.ErrorImage = null;
            this.Smirror_ck.InitialImage = null;
            this.Smirror_ck.Location = new System.Drawing.Point(1225, 704);
            this.Smirror_ck.Margin = new System.Windows.Forms.Padding(0);
            this.Smirror_ck.Name = "Smirror_ck";
            this.Smirror_ck.Size = new System.Drawing.Size(64, 64);
            this.Smirror_ck.TabIndex = 277;
            this.Smirror_ck.TabStop = false;
            // 
            // Smirror_label
            // 
            this.Smirror_label.AutoSize = true;
            this.Smirror_label.BackColor = System.Drawing.Color.Transparent;
            this.Smirror_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Smirror_label.ForeColor = System.Drawing.SystemColors.Window;
            this.Smirror_label.Location = new System.Drawing.Point(1293, 705);
            this.Smirror_label.Margin = new System.Windows.Forms.Padding(0);
            this.Smirror_label.Name = "Smirror_label";
            this.Smirror_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Smirror_label.Size = new System.Drawing.Size(78, 64);
            this.Smirror_label.TabIndex = 275;
            this.Smirror_label.Text = "Mirror";
            this.Smirror_label.Click += new System.EventHandler(this.WrapS_Mirror_Click);
            this.Smirror_label.MouseEnter += new System.EventHandler(this.WrapS_Mirror_MouseEnter);
            this.Smirror_label.MouseLeave += new System.EventHandler(this.WrapS_Mirror_MouseLeave);
            // 
            // Smirror_hitbox
            // 
            this.Smirror_hitbox.AutoSize = true;
            this.Smirror_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.Smirror_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Smirror_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.Smirror_hitbox.Location = new System.Drawing.Point(1222, 704);
            this.Smirror_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.Smirror_hitbox.Name = "Smirror_hitbox";
            this.Smirror_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.Smirror_hitbox.Size = new System.Drawing.Size(190, 64);
            this.Smirror_hitbox.TabIndex = 276;
            this.Smirror_hitbox.Click += new System.EventHandler(this.WrapS_Mirror_Click);
            this.Smirror_hitbox.MouseEnter += new System.EventHandler(this.WrapS_Mirror_MouseEnter);
            this.Smirror_hitbox.MouseLeave += new System.EventHandler(this.WrapS_Mirror_MouseLeave);
            // 
            // Srepeat_ck
            // 
            this.Srepeat_ck.BackColor = System.Drawing.Color.Transparent;
            this.Srepeat_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Srepeat_ck.Enabled = false;
            this.Srepeat_ck.ErrorImage = null;
            this.Srepeat_ck.InitialImage = null;
            this.Srepeat_ck.Location = new System.Drawing.Point(1225, 640);
            this.Srepeat_ck.Margin = new System.Windows.Forms.Padding(0);
            this.Srepeat_ck.Name = "Srepeat_ck";
            this.Srepeat_ck.Size = new System.Drawing.Size(64, 64);
            this.Srepeat_ck.TabIndex = 274;
            this.Srepeat_ck.TabStop = false;
            // 
            // Srepeat_label
            // 
            this.Srepeat_label.AutoSize = true;
            this.Srepeat_label.BackColor = System.Drawing.Color.Transparent;
            this.Srepeat_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Srepeat_label.ForeColor = System.Drawing.SystemColors.Window;
            this.Srepeat_label.Location = new System.Drawing.Point(1293, 640);
            this.Srepeat_label.Margin = new System.Windows.Forms.Padding(0);
            this.Srepeat_label.Name = "Srepeat_label";
            this.Srepeat_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Srepeat_label.Size = new System.Drawing.Size(92, 64);
            this.Srepeat_label.TabIndex = 272;
            this.Srepeat_label.Text = "Repeat";
            this.Srepeat_label.Click += new System.EventHandler(this.WrapS_Repeat_Click);
            this.Srepeat_label.MouseEnter += new System.EventHandler(this.WrapS_Repeat_MouseEnter);
            this.Srepeat_label.MouseLeave += new System.EventHandler(this.WrapS_Repeat_MouseLeave);
            // 
            // Srepeat_hitbox
            // 
            this.Srepeat_hitbox.AutoSize = true;
            this.Srepeat_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.Srepeat_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Srepeat_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.Srepeat_hitbox.Location = new System.Drawing.Point(1222, 640);
            this.Srepeat_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.Srepeat_hitbox.Name = "Srepeat_hitbox";
            this.Srepeat_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.Srepeat_hitbox.Size = new System.Drawing.Size(190, 64);
            this.Srepeat_hitbox.TabIndex = 273;
            this.Srepeat_hitbox.Click += new System.EventHandler(this.WrapS_Repeat_Click);
            this.Srepeat_hitbox.MouseEnter += new System.EventHandler(this.WrapS_Repeat_MouseEnter);
            this.Srepeat_hitbox.MouseLeave += new System.EventHandler(this.WrapS_Repeat_MouseLeave);
            // 
            // Sclamp_ck
            // 
            this.Sclamp_ck.BackColor = System.Drawing.Color.Transparent;
            this.Sclamp_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Sclamp_ck.Enabled = false;
            this.Sclamp_ck.ErrorImage = null;
            this.Sclamp_ck.InitialImage = null;
            this.Sclamp_ck.Location = new System.Drawing.Point(1225, 576);
            this.Sclamp_ck.Margin = new System.Windows.Forms.Padding(0);
            this.Sclamp_ck.Name = "Sclamp_ck";
            this.Sclamp_ck.Size = new System.Drawing.Size(64, 64);
            this.Sclamp_ck.TabIndex = 271;
            this.Sclamp_ck.TabStop = false;
            // 
            // Sclamp_label
            // 
            this.Sclamp_label.AutoSize = true;
            this.Sclamp_label.BackColor = System.Drawing.Color.Transparent;
            this.Sclamp_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Sclamp_label.ForeColor = System.Drawing.SystemColors.Window;
            this.Sclamp_label.Location = new System.Drawing.Point(1293, 576);
            this.Sclamp_label.Margin = new System.Windows.Forms.Padding(0);
            this.Sclamp_label.Name = "Sclamp_label";
            this.Sclamp_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Sclamp_label.Size = new System.Drawing.Size(80, 64);
            this.Sclamp_label.TabIndex = 269;
            this.Sclamp_label.Text = "Clamp";
            this.Sclamp_label.Click += new System.EventHandler(this.WrapS_Clamp_Click);
            this.Sclamp_label.MouseEnter += new System.EventHandler(this.WrapS_Clamp_MouseEnter);
            this.Sclamp_label.MouseLeave += new System.EventHandler(this.WrapS_Clamp_MouseLeave);
            // 
            // Sclamp_hitbox
            // 
            this.Sclamp_hitbox.AutoSize = true;
            this.Sclamp_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.Sclamp_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Sclamp_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.Sclamp_hitbox.Location = new System.Drawing.Point(1222, 576);
            this.Sclamp_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.Sclamp_hitbox.Name = "Sclamp_hitbox";
            this.Sclamp_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.Sclamp_hitbox.Size = new System.Drawing.Size(190, 64);
            this.Sclamp_hitbox.TabIndex = 270;
            this.Sclamp_hitbox.Click += new System.EventHandler(this.WrapS_Clamp_Click);
            this.Sclamp_hitbox.MouseEnter += new System.EventHandler(this.WrapS_Clamp_MouseEnter);
            this.Sclamp_hitbox.MouseLeave += new System.EventHandler(this.WrapS_Clamp_MouseLeave);
            // 
            // WrapS_label
            // 
            this.WrapS_label.AutoSize = true;
            this.WrapS_label.BackColor = System.Drawing.Color.Transparent;
            this.WrapS_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.WrapS_label.ForeColor = System.Drawing.SystemColors.Control;
            this.WrapS_label.Location = new System.Drawing.Point(1273, 546);
            this.WrapS_label.Name = "WrapS_label";
            this.WrapS_label.Size = new System.Drawing.Size(83, 20);
            this.WrapS_label.TabIndex = 268;
            this.WrapS_label.Text = "WrapS";
            // 
            // magnification_label
            // 
            this.magnification_label.AutoSize = true;
            this.magnification_label.BackColor = System.Drawing.Color.Transparent;
            this.magnification_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.magnification_label.ForeColor = System.Drawing.SystemColors.Control;
            this.magnification_label.Location = new System.Drawing.Point(1244, 97);
            this.magnification_label.Name = "magnification_label";
            this.magnification_label.Size = new System.Drawing.Size(225, 20);
            this.magnification_label.TabIndex = 291;
            this.magnification_label.Text = "Magnification filter";
            // 
            // minification_label
            // 
            this.minification_label.AutoSize = true;
            this.minification_label.BackColor = System.Drawing.Color.Transparent;
            this.minification_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.minification_label.ForeColor = System.Drawing.SystemColors.Control;
            this.minification_label.Location = new System.Drawing.Point(851, 97);
            this.minification_label.Name = "minification_label";
            this.minification_label.Size = new System.Drawing.Size(202, 20);
            this.minification_label.TabIndex = 278;
            this.minification_label.Text = "Minification filter";
            // 
            // min_linearmipmaplinear_ck
            // 
            this.min_linearmipmaplinear_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmaplinear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_linearmipmaplinear_ck.Enabled = false;
            this.min_linearmipmaplinear_ck.ErrorImage = null;
            this.min_linearmipmaplinear_ck.InitialImage = null;
            this.min_linearmipmaplinear_ck.Location = new System.Drawing.Point(825, 448);
            this.min_linearmipmaplinear_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmaplinear_ck.Name = "min_linearmipmaplinear_ck";
            this.min_linearmipmaplinear_ck.Size = new System.Drawing.Size(64, 64);
            this.min_linearmipmaplinear_ck.TabIndex = 324;
            this.min_linearmipmaplinear_ck.TabStop = false;
            // 
            // min_linearmipmaplinear_label
            // 
            this.min_linearmipmaplinear_label.AutoSize = true;
            this.min_linearmipmaplinear_label.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmaplinear_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_linearmipmaplinear_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_linearmipmaplinear_label.Location = new System.Drawing.Point(893, 448);
            this.min_linearmipmaplinear_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmaplinear_label.Name = "min_linearmipmaplinear_label";
            this.min_linearmipmaplinear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_linearmipmaplinear_label.Size = new System.Drawing.Size(240, 64);
            this.min_linearmipmaplinear_label.TabIndex = 322;
            this.min_linearmipmaplinear_label.Text = "LinearMipmapLinear";
            this.min_linearmipmaplinear_label.Click += new System.EventHandler(this.Minification_LinearMipmapLinear_Click);
            this.min_linearmipmaplinear_label.MouseEnter += new System.EventHandler(this.Minification_LinearMipmapLinear_MouseEnter);
            this.min_linearmipmaplinear_label.MouseLeave += new System.EventHandler(this.Minification_LinearMipmapLinear_MouseLeave);
            // 
            // min_linearmipmaplinear_hitbox
            // 
            this.min_linearmipmaplinear_hitbox.AutoSize = true;
            this.min_linearmipmaplinear_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmaplinear_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_linearmipmaplinear_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.min_linearmipmaplinear_hitbox.Location = new System.Drawing.Point(822, 448);
            this.min_linearmipmaplinear_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmaplinear_hitbox.Name = "min_linearmipmaplinear_hitbox";
            this.min_linearmipmaplinear_hitbox.Padding = new System.Windows.Forms.Padding(350, 44, 0, 0);
            this.min_linearmipmaplinear_hitbox.Size = new System.Drawing.Size(350, 64);
            this.min_linearmipmaplinear_hitbox.TabIndex = 323;
            this.min_linearmipmaplinear_hitbox.Click += new System.EventHandler(this.Minification_LinearMipmapLinear_Click);
            this.min_linearmipmaplinear_hitbox.MouseEnter += new System.EventHandler(this.Minification_LinearMipmapLinear_MouseEnter);
            this.min_linearmipmaplinear_hitbox.MouseLeave += new System.EventHandler(this.Minification_LinearMipmapLinear_MouseLeave);
            // 
            // min_linearmipmapnearest_ck
            // 
            this.min_linearmipmapnearest_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmapnearest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_linearmipmapnearest_ck.Enabled = false;
            this.min_linearmipmapnearest_ck.ErrorImage = null;
            this.min_linearmipmapnearest_ck.InitialImage = null;
            this.min_linearmipmapnearest_ck.Location = new System.Drawing.Point(825, 384);
            this.min_linearmipmapnearest_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmapnearest_ck.Name = "min_linearmipmapnearest_ck";
            this.min_linearmipmapnearest_ck.Size = new System.Drawing.Size(64, 64);
            this.min_linearmipmapnearest_ck.TabIndex = 321;
            this.min_linearmipmapnearest_ck.TabStop = false;
            // 
            // min_linearmipmapnearest_label
            // 
            this.min_linearmipmapnearest_label.AutoSize = true;
            this.min_linearmipmapnearest_label.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmapnearest_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_linearmipmapnearest_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_linearmipmapnearest_label.Location = new System.Drawing.Point(893, 385);
            this.min_linearmipmapnearest_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmapnearest_label.Name = "min_linearmipmapnearest_label";
            this.min_linearmipmapnearest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_linearmipmapnearest_label.Size = new System.Drawing.Size(280, 64);
            this.min_linearmipmapnearest_label.TabIndex = 319;
            this.min_linearmipmapnearest_label.Text = "LinearMipmapNearest  ";
            this.min_linearmipmapnearest_label.Click += new System.EventHandler(this.Minification_LinearMipmapNearest_Click);
            this.min_linearmipmapnearest_label.MouseEnter += new System.EventHandler(this.Minification_LinearMipmapNearest_MouseEnter);
            this.min_linearmipmapnearest_label.MouseLeave += new System.EventHandler(this.Minification_LinearMipmapNearest_MouseLeave);
            // 
            // min_linearmipmapnearest_hitbox
            // 
            this.min_linearmipmapnearest_hitbox.AutoSize = true;
            this.min_linearmipmapnearest_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmapnearest_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_linearmipmapnearest_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.min_linearmipmapnearest_hitbox.Location = new System.Drawing.Point(822, 384);
            this.min_linearmipmapnearest_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmapnearest_hitbox.Name = "min_linearmipmapnearest_hitbox";
            this.min_linearmipmapnearest_hitbox.Padding = new System.Windows.Forms.Padding(350, 44, 0, 0);
            this.min_linearmipmapnearest_hitbox.Size = new System.Drawing.Size(350, 64);
            this.min_linearmipmapnearest_hitbox.TabIndex = 320;
            this.min_linearmipmapnearest_hitbox.Click += new System.EventHandler(this.Minification_LinearMipmapNearest_Click);
            this.min_linearmipmapnearest_hitbox.MouseEnter += new System.EventHandler(this.Minification_LinearMipmapNearest_MouseEnter);
            this.min_linearmipmapnearest_hitbox.MouseLeave += new System.EventHandler(this.Minification_LinearMipmapNearest_MouseLeave);
            // 
            // min_nearestmipmaplinear_ck
            // 
            this.min_nearestmipmaplinear_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmaplinear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_nearestmipmaplinear_ck.Enabled = false;
            this.min_nearestmipmaplinear_ck.ErrorImage = null;
            this.min_nearestmipmaplinear_ck.InitialImage = null;
            this.min_nearestmipmaplinear_ck.Location = new System.Drawing.Point(825, 320);
            this.min_nearestmipmaplinear_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmaplinear_ck.Name = "min_nearestmipmaplinear_ck";
            this.min_nearestmipmaplinear_ck.Size = new System.Drawing.Size(64, 64);
            this.min_nearestmipmaplinear_ck.TabIndex = 318;
            this.min_nearestmipmaplinear_ck.TabStop = false;
            // 
            // min_nearestmipmaplinear_label
            // 
            this.min_nearestmipmaplinear_label.AutoSize = true;
            this.min_nearestmipmaplinear_label.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmaplinear_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_nearestmipmaplinear_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_nearestmipmaplinear_label.Location = new System.Drawing.Point(893, 321);
            this.min_nearestmipmaplinear_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmaplinear_label.Name = "min_nearestmipmaplinear_label";
            this.min_nearestmipmaplinear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_nearestmipmaplinear_label.Size = new System.Drawing.Size(280, 64);
            this.min_nearestmipmaplinear_label.TabIndex = 316;
            this.min_nearestmipmaplinear_label.Text = "NearestMipmapLinear  ";
            this.min_nearestmipmaplinear_label.Click += new System.EventHandler(this.Minification_NearestMipmapLinear_Click);
            this.min_nearestmipmaplinear_label.MouseEnter += new System.EventHandler(this.Minification_NearestMipmapLinear_MouseEnter);
            this.min_nearestmipmaplinear_label.MouseLeave += new System.EventHandler(this.Minification_NearestMipmapLinear_MouseLeave);
            // 
            // min_nearestmipmaplinear_hitbox
            // 
            this.min_nearestmipmaplinear_hitbox.AutoSize = true;
            this.min_nearestmipmaplinear_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmaplinear_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_nearestmipmaplinear_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.min_nearestmipmaplinear_hitbox.Location = new System.Drawing.Point(822, 320);
            this.min_nearestmipmaplinear_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmaplinear_hitbox.Name = "min_nearestmipmaplinear_hitbox";
            this.min_nearestmipmaplinear_hitbox.Padding = new System.Windows.Forms.Padding(350, 44, 0, 0);
            this.min_nearestmipmaplinear_hitbox.Size = new System.Drawing.Size(350, 64);
            this.min_nearestmipmaplinear_hitbox.TabIndex = 317;
            this.min_nearestmipmaplinear_hitbox.Click += new System.EventHandler(this.Minification_NearestMipmapLinear_Click);
            this.min_nearestmipmaplinear_hitbox.MouseEnter += new System.EventHandler(this.Minification_NearestMipmapLinear_MouseEnter);
            this.min_nearestmipmaplinear_hitbox.MouseLeave += new System.EventHandler(this.Minification_NearestMipmapLinear_MouseLeave);
            // 
            // min_nearestmipmapnearest_ck
            // 
            this.min_nearestmipmapnearest_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmapnearest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_nearestmipmapnearest_ck.Enabled = false;
            this.min_nearestmipmapnearest_ck.ErrorImage = null;
            this.min_nearestmipmapnearest_ck.InitialImage = null;
            this.min_nearestmipmapnearest_ck.Location = new System.Drawing.Point(825, 256);
            this.min_nearestmipmapnearest_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmapnearest_ck.Name = "min_nearestmipmapnearest_ck";
            this.min_nearestmipmapnearest_ck.Size = new System.Drawing.Size(64, 64);
            this.min_nearestmipmapnearest_ck.TabIndex = 315;
            this.min_nearestmipmapnearest_ck.TabStop = false;
            // 
            // min_nearestmipmapnearest_label
            // 
            this.min_nearestmipmapnearest_label.AutoSize = true;
            this.min_nearestmipmapnearest_label.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmapnearest_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_nearestmipmapnearest_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_nearestmipmapnearest_label.Location = new System.Drawing.Point(893, 257);
            this.min_nearestmipmapnearest_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmapnearest_label.Name = "min_nearestmipmapnearest_label";
            this.min_nearestmipmapnearest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_nearestmipmapnearest_label.Size = new System.Drawing.Size(290, 64);
            this.min_nearestmipmapnearest_label.TabIndex = 313;
            this.min_nearestmipmapnearest_label.Text = "NearestMipmapNearest ";
            this.min_nearestmipmapnearest_label.Click += new System.EventHandler(this.Minification_NearestMipmapNearest_Click);
            this.min_nearestmipmapnearest_label.MouseEnter += new System.EventHandler(this.Minification_NearestMipmapNearest_MouseEnter);
            this.min_nearestmipmapnearest_label.MouseLeave += new System.EventHandler(this.Minification_NearestMipmapNearest_MouseLeave);
            // 
            // min_nearestmipmapnearest_hitbox
            // 
            this.min_nearestmipmapnearest_hitbox.AutoSize = true;
            this.min_nearestmipmapnearest_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmapnearest_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_nearestmipmapnearest_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.min_nearestmipmapnearest_hitbox.Location = new System.Drawing.Point(822, 256);
            this.min_nearestmipmapnearest_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmapnearest_hitbox.Name = "min_nearestmipmapnearest_hitbox";
            this.min_nearestmipmapnearest_hitbox.Padding = new System.Windows.Forms.Padding(350, 44, 0, 0);
            this.min_nearestmipmapnearest_hitbox.Size = new System.Drawing.Size(350, 64);
            this.min_nearestmipmapnearest_hitbox.TabIndex = 314;
            this.min_nearestmipmapnearest_hitbox.Click += new System.EventHandler(this.Minification_NearestMipmapNearest_Click);
            this.min_nearestmipmapnearest_hitbox.MouseEnter += new System.EventHandler(this.Minification_NearestMipmapNearest_MouseEnter);
            this.min_nearestmipmapnearest_hitbox.MouseLeave += new System.EventHandler(this.Minification_NearestMipmapNearest_MouseLeave);
            // 
            // min_linear_ck
            // 
            this.min_linear_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_linear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_linear_ck.Enabled = false;
            this.min_linear_ck.ErrorImage = null;
            this.min_linear_ck.InitialImage = null;
            this.min_linear_ck.Location = new System.Drawing.Point(825, 192);
            this.min_linear_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_linear_ck.Name = "min_linear_ck";
            this.min_linear_ck.Size = new System.Drawing.Size(64, 64);
            this.min_linear_ck.TabIndex = 312;
            this.min_linear_ck.TabStop = false;
            // 
            // min_linear_label
            // 
            this.min_linear_label.AutoSize = true;
            this.min_linear_label.BackColor = System.Drawing.Color.Transparent;
            this.min_linear_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_linear_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_linear_label.Location = new System.Drawing.Point(893, 192);
            this.min_linear_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_linear_label.Name = "min_linear_label";
            this.min_linear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_linear_label.Size = new System.Drawing.Size(81, 64);
            this.min_linear_label.TabIndex = 310;
            this.min_linear_label.Text = "Linear";
            this.min_linear_label.Click += new System.EventHandler(this.Minification_Linear_Click);
            this.min_linear_label.MouseEnter += new System.EventHandler(this.Minification_Linear_MouseEnter);
            this.min_linear_label.MouseLeave += new System.EventHandler(this.Minification_Linear_MouseLeave);
            // 
            // min_linear_hitbox
            // 
            this.min_linear_hitbox.AutoSize = true;
            this.min_linear_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.min_linear_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_linear_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.min_linear_hitbox.Location = new System.Drawing.Point(822, 192);
            this.min_linear_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.min_linear_hitbox.Name = "min_linear_hitbox";
            this.min_linear_hitbox.Padding = new System.Windows.Forms.Padding(350, 44, 0, 0);
            this.min_linear_hitbox.Size = new System.Drawing.Size(350, 64);
            this.min_linear_hitbox.TabIndex = 311;
            this.min_linear_hitbox.Click += new System.EventHandler(this.Minification_Linear_Click);
            this.min_linear_hitbox.MouseEnter += new System.EventHandler(this.Minification_Linear_MouseEnter);
            this.min_linear_hitbox.MouseLeave += new System.EventHandler(this.Minification_Linear_MouseLeave);
            // 
            // min_nearest_neighbour_ck
            // 
            this.min_nearest_neighbour_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_nearest_neighbour_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_nearest_neighbour_ck.Enabled = false;
            this.min_nearest_neighbour_ck.ErrorImage = null;
            this.min_nearest_neighbour_ck.InitialImage = null;
            this.min_nearest_neighbour_ck.Location = new System.Drawing.Point(825, 128);
            this.min_nearest_neighbour_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearest_neighbour_ck.Name = "min_nearest_neighbour_ck";
            this.min_nearest_neighbour_ck.Size = new System.Drawing.Size(64, 64);
            this.min_nearest_neighbour_ck.TabIndex = 309;
            this.min_nearest_neighbour_ck.TabStop = false;
            // 
            // min_nearest_neighbour_label
            // 
            this.min_nearest_neighbour_label.AutoSize = true;
            this.min_nearest_neighbour_label.BackColor = System.Drawing.Color.Transparent;
            this.min_nearest_neighbour_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(128)), true);
            this.min_nearest_neighbour_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_nearest_neighbour_label.Location = new System.Drawing.Point(893, 128);
            this.min_nearest_neighbour_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearest_neighbour_label.Name = "min_nearest_neighbour_label";
            this.min_nearest_neighbour_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_nearest_neighbour_label.Size = new System.Drawing.Size(172, 59);
            this.min_nearest_neighbour_label.TabIndex = 307;
            this.min_nearest_neighbour_label.Text = "Nearest Neighbour";
            this.min_nearest_neighbour_label.Click += new System.EventHandler(this.Minification_Nearest_Neighbour_Click);
            this.min_nearest_neighbour_label.MouseEnter += new System.EventHandler(this.Minification_Nearest_Neighbour_MouseEnter);
            this.min_nearest_neighbour_label.MouseLeave += new System.EventHandler(this.Minification_Nearest_Neighbour_MouseLeave);
            // 
            // min_nearest_neighbour_hitbox
            // 
            this.min_nearest_neighbour_hitbox.AutoSize = true;
            this.min_nearest_neighbour_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.min_nearest_neighbour_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_nearest_neighbour_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.min_nearest_neighbour_hitbox.Location = new System.Drawing.Point(822, 128);
            this.min_nearest_neighbour_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearest_neighbour_hitbox.Name = "min_nearest_neighbour_hitbox";
            this.min_nearest_neighbour_hitbox.Padding = new System.Windows.Forms.Padding(350, 44, 0, 0);
            this.min_nearest_neighbour_hitbox.Size = new System.Drawing.Size(350, 64);
            this.min_nearest_neighbour_hitbox.TabIndex = 308;
            this.min_nearest_neighbour_hitbox.Click += new System.EventHandler(this.Minification_Nearest_Neighbour_Click);
            this.min_nearest_neighbour_hitbox.MouseEnter += new System.EventHandler(this.Minification_Nearest_Neighbour_MouseEnter);
            this.min_nearest_neighbour_hitbox.MouseLeave += new System.EventHandler(this.Minification_Nearest_Neighbour_MouseLeave);
            // 
            // mag_linearmipmaplinear_ck
            // 
            this.mag_linearmipmaplinear_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmaplinear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_linearmipmaplinear_ck.Enabled = false;
            this.mag_linearmipmaplinear_ck.ErrorImage = null;
            this.mag_linearmipmaplinear_ck.InitialImage = null;
            this.mag_linearmipmaplinear_ck.Location = new System.Drawing.Point(1225, 448);
            this.mag_linearmipmaplinear_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmaplinear_ck.Name = "mag_linearmipmaplinear_ck";
            this.mag_linearmipmaplinear_ck.Size = new System.Drawing.Size(64, 64);
            this.mag_linearmipmaplinear_ck.TabIndex = 342;
            this.mag_linearmipmaplinear_ck.TabStop = false;
            // 
            // mag_linearmipmaplinear_label
            // 
            this.mag_linearmipmaplinear_label.AutoSize = true;
            this.mag_linearmipmaplinear_label.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmaplinear_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_linearmipmaplinear_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mag_linearmipmaplinear_label.Location = new System.Drawing.Point(1293, 448);
            this.mag_linearmipmaplinear_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmaplinear_label.Name = "mag_linearmipmaplinear_label";
            this.mag_linearmipmaplinear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_linearmipmaplinear_label.Size = new System.Drawing.Size(240, 64);
            this.mag_linearmipmaplinear_label.TabIndex = 340;
            this.mag_linearmipmaplinear_label.Text = "LinearMipmapLinear";
            this.mag_linearmipmaplinear_label.Click += new System.EventHandler(this.Magnification_LinearMipmapLinear_Click);
            this.mag_linearmipmaplinear_label.MouseEnter += new System.EventHandler(this.Magnification_LinearMipmapLinear_MouseEnter);
            this.mag_linearmipmaplinear_label.MouseLeave += new System.EventHandler(this.Magnification_LinearMipmapLinear_MouseLeave);
            // 
            // mag_linearmipmaplinear_hitbox
            // 
            this.mag_linearmipmaplinear_hitbox.AutoSize = true;
            this.mag_linearmipmaplinear_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmaplinear_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_linearmipmaplinear_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mag_linearmipmaplinear_hitbox.Location = new System.Drawing.Point(1222, 448);
            this.mag_linearmipmaplinear_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmaplinear_hitbox.Name = "mag_linearmipmaplinear_hitbox";
            this.mag_linearmipmaplinear_hitbox.Padding = new System.Windows.Forms.Padding(350, 44, 0, 0);
            this.mag_linearmipmaplinear_hitbox.Size = new System.Drawing.Size(350, 64);
            this.mag_linearmipmaplinear_hitbox.TabIndex = 341;
            this.mag_linearmipmaplinear_hitbox.Click += new System.EventHandler(this.Magnification_LinearMipmapLinear_Click);
            this.mag_linearmipmaplinear_hitbox.MouseEnter += new System.EventHandler(this.Magnification_LinearMipmapLinear_MouseEnter);
            this.mag_linearmipmaplinear_hitbox.MouseLeave += new System.EventHandler(this.Magnification_LinearMipmapLinear_MouseLeave);
            // 
            // mag_linearmipmapnearest_ck
            // 
            this.mag_linearmipmapnearest_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmapnearest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_linearmipmapnearest_ck.Enabled = false;
            this.mag_linearmipmapnearest_ck.ErrorImage = null;
            this.mag_linearmipmapnearest_ck.InitialImage = null;
            this.mag_linearmipmapnearest_ck.Location = new System.Drawing.Point(1225, 384);
            this.mag_linearmipmapnearest_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmapnearest_ck.Name = "mag_linearmipmapnearest_ck";
            this.mag_linearmipmapnearest_ck.Size = new System.Drawing.Size(64, 64);
            this.mag_linearmipmapnearest_ck.TabIndex = 339;
            this.mag_linearmipmapnearest_ck.TabStop = false;
            // 
            // mag_linearmipmapnearest_label
            // 
            this.mag_linearmipmapnearest_label.AutoSize = true;
            this.mag_linearmipmapnearest_label.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmapnearest_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_linearmipmapnearest_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mag_linearmipmapnearest_label.Location = new System.Drawing.Point(1293, 385);
            this.mag_linearmipmapnearest_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmapnearest_label.Name = "mag_linearmipmapnearest_label";
            this.mag_linearmipmapnearest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_linearmipmapnearest_label.Size = new System.Drawing.Size(280, 64);
            this.mag_linearmipmapnearest_label.TabIndex = 337;
            this.mag_linearmipmapnearest_label.Text = "LinearMipmapNearest  ";
            this.mag_linearmipmapnearest_label.Click += new System.EventHandler(this.Magnification_LinearMipmapNearest_Click);
            this.mag_linearmipmapnearest_label.MouseEnter += new System.EventHandler(this.Magnification_LinearMipmapNearest_MouseEnter);
            this.mag_linearmipmapnearest_label.MouseLeave += new System.EventHandler(this.Magnification_LinearMipmapNearest_MouseLeave);
            // 
            // mag_linearmipmapnearest_hitbox
            // 
            this.mag_linearmipmapnearest_hitbox.AutoSize = true;
            this.mag_linearmipmapnearest_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmapnearest_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_linearmipmapnearest_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mag_linearmipmapnearest_hitbox.Location = new System.Drawing.Point(1222, 384);
            this.mag_linearmipmapnearest_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmapnearest_hitbox.Name = "mag_linearmipmapnearest_hitbox";
            this.mag_linearmipmapnearest_hitbox.Padding = new System.Windows.Forms.Padding(350, 44, 0, 0);
            this.mag_linearmipmapnearest_hitbox.Size = new System.Drawing.Size(350, 64);
            this.mag_linearmipmapnearest_hitbox.TabIndex = 338;
            this.mag_linearmipmapnearest_hitbox.Click += new System.EventHandler(this.Magnification_LinearMipmapNearest_Click);
            this.mag_linearmipmapnearest_hitbox.MouseEnter += new System.EventHandler(this.Magnification_LinearMipmapNearest_MouseEnter);
            this.mag_linearmipmapnearest_hitbox.MouseLeave += new System.EventHandler(this.Magnification_LinearMipmapNearest_MouseLeave);
            // 
            // mag_nearestmipmaplinear_ck
            // 
            this.mag_nearestmipmaplinear_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmaplinear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_nearestmipmaplinear_ck.Enabled = false;
            this.mag_nearestmipmaplinear_ck.ErrorImage = null;
            this.mag_nearestmipmaplinear_ck.InitialImage = null;
            this.mag_nearestmipmaplinear_ck.Location = new System.Drawing.Point(1225, 320);
            this.mag_nearestmipmaplinear_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmaplinear_ck.Name = "mag_nearestmipmaplinear_ck";
            this.mag_nearestmipmaplinear_ck.Size = new System.Drawing.Size(64, 64);
            this.mag_nearestmipmaplinear_ck.TabIndex = 336;
            this.mag_nearestmipmaplinear_ck.TabStop = false;
            // 
            // mag_nearestmipmaplinear_label
            // 
            this.mag_nearestmipmaplinear_label.AutoSize = true;
            this.mag_nearestmipmaplinear_label.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmaplinear_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_nearestmipmaplinear_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mag_nearestmipmaplinear_label.Location = new System.Drawing.Point(1293, 321);
            this.mag_nearestmipmaplinear_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmaplinear_label.Name = "mag_nearestmipmaplinear_label";
            this.mag_nearestmipmaplinear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_nearestmipmaplinear_label.Size = new System.Drawing.Size(280, 64);
            this.mag_nearestmipmaplinear_label.TabIndex = 334;
            this.mag_nearestmipmaplinear_label.Text = "NearestMipmapLinear  ";
            this.mag_nearestmipmaplinear_label.Click += new System.EventHandler(this.Magnification_NearestMipmapLinear_Click);
            this.mag_nearestmipmaplinear_label.MouseEnter += new System.EventHandler(this.Magnification_NearestMipmapLinear_MouseEnter);
            this.mag_nearestmipmaplinear_label.MouseLeave += new System.EventHandler(this.Magnification_NearestMipmapLinear_MouseLeave);
            // 
            // mag_nearestmipmaplinear_hitbox
            // 
            this.mag_nearestmipmaplinear_hitbox.AutoSize = true;
            this.mag_nearestmipmaplinear_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmaplinear_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_nearestmipmaplinear_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mag_nearestmipmaplinear_hitbox.Location = new System.Drawing.Point(1222, 320);
            this.mag_nearestmipmaplinear_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmaplinear_hitbox.Name = "mag_nearestmipmaplinear_hitbox";
            this.mag_nearestmipmaplinear_hitbox.Padding = new System.Windows.Forms.Padding(350, 44, 0, 0);
            this.mag_nearestmipmaplinear_hitbox.Size = new System.Drawing.Size(350, 64);
            this.mag_nearestmipmaplinear_hitbox.TabIndex = 335;
            this.mag_nearestmipmaplinear_hitbox.Click += new System.EventHandler(this.Magnification_NearestMipmapLinear_Click);
            this.mag_nearestmipmaplinear_hitbox.MouseEnter += new System.EventHandler(this.Magnification_NearestMipmapLinear_MouseEnter);
            this.mag_nearestmipmaplinear_hitbox.MouseLeave += new System.EventHandler(this.Magnification_NearestMipmapLinear_MouseLeave);
            // 
            // mag_nearestmipmapnearest_ck
            // 
            this.mag_nearestmipmapnearest_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmapnearest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_nearestmipmapnearest_ck.Enabled = false;
            this.mag_nearestmipmapnearest_ck.ErrorImage = null;
            this.mag_nearestmipmapnearest_ck.InitialImage = null;
            this.mag_nearestmipmapnearest_ck.Location = new System.Drawing.Point(1225, 256);
            this.mag_nearestmipmapnearest_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmapnearest_ck.Name = "mag_nearestmipmapnearest_ck";
            this.mag_nearestmipmapnearest_ck.Size = new System.Drawing.Size(64, 64);
            this.mag_nearestmipmapnearest_ck.TabIndex = 333;
            this.mag_nearestmipmapnearest_ck.TabStop = false;
            // 
            // mag_nearestmipmapnearest_label
            // 
            this.mag_nearestmipmapnearest_label.AutoSize = true;
            this.mag_nearestmipmapnearest_label.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmapnearest_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_nearestmipmapnearest_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mag_nearestmipmapnearest_label.Location = new System.Drawing.Point(1293, 257);
            this.mag_nearestmipmapnearest_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmapnearest_label.Name = "mag_nearestmipmapnearest_label";
            this.mag_nearestmipmapnearest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_nearestmipmapnearest_label.Size = new System.Drawing.Size(290, 64);
            this.mag_nearestmipmapnearest_label.TabIndex = 331;
            this.mag_nearestmipmapnearest_label.Text = "NearestMipmapNearest ";
            this.mag_nearestmipmapnearest_label.Click += new System.EventHandler(this.Magnification_NearestMipmapNearest_Click);
            this.mag_nearestmipmapnearest_label.MouseEnter += new System.EventHandler(this.Magnification_NearestMipmapNearest_MouseEnter);
            this.mag_nearestmipmapnearest_label.MouseLeave += new System.EventHandler(this.Magnification_NearestMipmapNearest_MouseLeave);
            // 
            // mag_nearestmipmapnearest_hitbox
            // 
            this.mag_nearestmipmapnearest_hitbox.AutoSize = true;
            this.mag_nearestmipmapnearest_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmapnearest_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_nearestmipmapnearest_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mag_nearestmipmapnearest_hitbox.Location = new System.Drawing.Point(1222, 256);
            this.mag_nearestmipmapnearest_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmapnearest_hitbox.Name = "mag_nearestmipmapnearest_hitbox";
            this.mag_nearestmipmapnearest_hitbox.Padding = new System.Windows.Forms.Padding(350, 44, 0, 0);
            this.mag_nearestmipmapnearest_hitbox.Size = new System.Drawing.Size(350, 64);
            this.mag_nearestmipmapnearest_hitbox.TabIndex = 332;
            this.mag_nearestmipmapnearest_hitbox.Click += new System.EventHandler(this.Magnification_NearestMipmapNearest_Click);
            this.mag_nearestmipmapnearest_hitbox.MouseEnter += new System.EventHandler(this.Magnification_NearestMipmapNearest_MouseEnter);
            this.mag_nearestmipmapnearest_hitbox.MouseLeave += new System.EventHandler(this.Magnification_NearestMipmapNearest_MouseLeave);
            // 
            // mag_linear_ck
            // 
            this.mag_linear_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_linear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_linear_ck.Enabled = false;
            this.mag_linear_ck.ErrorImage = null;
            this.mag_linear_ck.InitialImage = null;
            this.mag_linear_ck.Location = new System.Drawing.Point(1225, 192);
            this.mag_linear_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linear_ck.Name = "mag_linear_ck";
            this.mag_linear_ck.Size = new System.Drawing.Size(64, 64);
            this.mag_linear_ck.TabIndex = 330;
            this.mag_linear_ck.TabStop = false;
            // 
            // mag_linear_label
            // 
            this.mag_linear_label.AutoSize = true;
            this.mag_linear_label.BackColor = System.Drawing.Color.Transparent;
            this.mag_linear_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_linear_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mag_linear_label.Location = new System.Drawing.Point(1293, 192);
            this.mag_linear_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linear_label.Name = "mag_linear_label";
            this.mag_linear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_linear_label.Size = new System.Drawing.Size(81, 64);
            this.mag_linear_label.TabIndex = 328;
            this.mag_linear_label.Text = "Linear";
            this.mag_linear_label.Click += new System.EventHandler(this.Magnification_Linear_Click);
            this.mag_linear_label.MouseEnter += new System.EventHandler(this.Magnification_Linear_MouseEnter);
            this.mag_linear_label.MouseLeave += new System.EventHandler(this.Magnification_Linear_MouseLeave);
            // 
            // mag_linear_hitbox
            // 
            this.mag_linear_hitbox.AutoSize = true;
            this.mag_linear_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mag_linear_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_linear_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mag_linear_hitbox.Location = new System.Drawing.Point(1222, 192);
            this.mag_linear_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linear_hitbox.Name = "mag_linear_hitbox";
            this.mag_linear_hitbox.Padding = new System.Windows.Forms.Padding(350, 44, 0, 0);
            this.mag_linear_hitbox.Size = new System.Drawing.Size(350, 64);
            this.mag_linear_hitbox.TabIndex = 329;
            this.mag_linear_hitbox.Click += new System.EventHandler(this.Magnification_Linear_Click);
            this.mag_linear_hitbox.MouseEnter += new System.EventHandler(this.Magnification_Linear_MouseEnter);
            this.mag_linear_hitbox.MouseLeave += new System.EventHandler(this.Magnification_Linear_MouseLeave);
            // 
            // mag_nearest_neighbour_ck
            // 
            this.mag_nearest_neighbour_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearest_neighbour_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_nearest_neighbour_ck.Enabled = false;
            this.mag_nearest_neighbour_ck.ErrorImage = null;
            this.mag_nearest_neighbour_ck.InitialImage = null;
            this.mag_nearest_neighbour_ck.Location = new System.Drawing.Point(1225, 128);
            this.mag_nearest_neighbour_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearest_neighbour_ck.Name = "mag_nearest_neighbour_ck";
            this.mag_nearest_neighbour_ck.Size = new System.Drawing.Size(64, 64);
            this.mag_nearest_neighbour_ck.TabIndex = 327;
            this.mag_nearest_neighbour_ck.TabStop = false;
            // 
            // mag_nearest_neighbour_label
            // 
            this.mag_nearest_neighbour_label.AutoSize = true;
            this.mag_nearest_neighbour_label.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearest_neighbour_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_nearest_neighbour_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mag_nearest_neighbour_label.Location = new System.Drawing.Point(1293, 128);
            this.mag_nearest_neighbour_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearest_neighbour_label.Name = "mag_nearest_neighbour_label";
            this.mag_nearest_neighbour_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_nearest_neighbour_label.Size = new System.Drawing.Size(227, 64);
            this.mag_nearest_neighbour_label.TabIndex = 325;
            this.mag_nearest_neighbour_label.Text = "Nearest Neighbour";
            this.mag_nearest_neighbour_label.Click += new System.EventHandler(this.Magnification_Nearest_Neighbour_Click);
            this.mag_nearest_neighbour_label.MouseEnter += new System.EventHandler(this.Magnification_Nearest_Neighbour_MouseEnter);
            this.mag_nearest_neighbour_label.MouseLeave += new System.EventHandler(this.Magnification_Nearest_Neighbour_MouseLeave);
            // 
            // mag_nearest_neighbour_hitbox
            // 
            this.mag_nearest_neighbour_hitbox.AutoSize = true;
            this.mag_nearest_neighbour_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearest_neighbour_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_nearest_neighbour_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mag_nearest_neighbour_hitbox.Location = new System.Drawing.Point(1222, 128);
            this.mag_nearest_neighbour_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearest_neighbour_hitbox.Name = "mag_nearest_neighbour_hitbox";
            this.mag_nearest_neighbour_hitbox.Padding = new System.Windows.Forms.Padding(350, 44, 0, 0);
            this.mag_nearest_neighbour_hitbox.Size = new System.Drawing.Size(350, 64);
            this.mag_nearest_neighbour_hitbox.TabIndex = 326;
            this.mag_nearest_neighbour_hitbox.Click += new System.EventHandler(this.Magnification_Nearest_Neighbour_Click);
            this.mag_nearest_neighbour_hitbox.MouseEnter += new System.EventHandler(this.Magnification_Nearest_Neighbour_MouseEnter);
            this.mag_nearest_neighbour_hitbox.MouseLeave += new System.EventHandler(this.Magnification_Nearest_Neighbour_MouseLeave);
            // 
            // r_r_ck
            // 
            this.r_r_ck.BackColor = System.Drawing.Color.Transparent;
            this.r_r_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.r_r_ck.Enabled = false;
            this.r_r_ck.ErrorImage = null;
            this.r_r_ck.InitialImage = null;
            this.r_r_ck.Location = new System.Drawing.Point(1647, 813);
            this.r_r_ck.Margin = new System.Windows.Forms.Padding(0);
            this.r_r_ck.Name = "r_r_ck";
            this.r_r_ck.Size = new System.Drawing.Size(64, 64);
            this.r_r_ck.TabIndex = 343;
            this.r_r_ck.TabStop = false;
            // 
            // r_g_ck
            // 
            this.r_g_ck.BackColor = System.Drawing.Color.Transparent;
            this.r_g_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.r_g_ck.Enabled = false;
            this.r_g_ck.ErrorImage = null;
            this.r_g_ck.InitialImage = null;
            this.r_g_ck.Location = new System.Drawing.Point(1647, 877);
            this.r_g_ck.Margin = new System.Windows.Forms.Padding(0);
            this.r_g_ck.Name = "r_g_ck";
            this.r_g_ck.Size = new System.Drawing.Size(64, 64);
            this.r_g_ck.TabIndex = 344;
            this.r_g_ck.TabStop = false;
            // 
            // g_r_ck
            // 
            this.g_r_ck.BackColor = System.Drawing.Color.Transparent;
            this.g_r_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.g_r_ck.Enabled = false;
            this.g_r_ck.ErrorImage = null;
            this.g_r_ck.InitialImage = null;
            this.g_r_ck.Location = new System.Drawing.Point(1711, 813);
            this.g_r_ck.Margin = new System.Windows.Forms.Padding(0);
            this.g_r_ck.Name = "g_r_ck";
            this.g_r_ck.Size = new System.Drawing.Size(64, 64);
            this.g_r_ck.TabIndex = 345;
            this.g_r_ck.TabStop = false;
            // 
            // g_g_ck
            // 
            this.g_g_ck.BackColor = System.Drawing.Color.Transparent;
            this.g_g_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.g_g_ck.Enabled = false;
            this.g_g_ck.ErrorImage = null;
            this.g_g_ck.InitialImage = null;
            this.g_g_ck.Location = new System.Drawing.Point(1711, 877);
            this.g_g_ck.Margin = new System.Windows.Forms.Padding(0);
            this.g_g_ck.Name = "g_g_ck";
            this.g_g_ck.Size = new System.Drawing.Size(64, 64);
            this.g_g_ck.TabIndex = 346;
            this.g_g_ck.TabStop = false;
            // 
            // a_g_ck
            // 
            this.a_g_ck.BackColor = System.Drawing.Color.Transparent;
            this.a_g_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.a_g_ck.Enabled = false;
            this.a_g_ck.ErrorImage = null;
            this.a_g_ck.InitialImage = null;
            this.a_g_ck.Location = new System.Drawing.Point(1839, 877);
            this.a_g_ck.Margin = new System.Windows.Forms.Padding(0);
            this.a_g_ck.Name = "a_g_ck";
            this.a_g_ck.Size = new System.Drawing.Size(64, 64);
            this.a_g_ck.TabIndex = 350;
            this.a_g_ck.TabStop = false;
            // 
            // a_r_ck
            // 
            this.a_r_ck.BackColor = System.Drawing.Color.Transparent;
            this.a_r_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.a_r_ck.Enabled = false;
            this.a_r_ck.ErrorImage = null;
            this.a_r_ck.InitialImage = null;
            this.a_r_ck.Location = new System.Drawing.Point(1839, 813);
            this.a_r_ck.Margin = new System.Windows.Forms.Padding(0);
            this.a_r_ck.Name = "a_r_ck";
            this.a_r_ck.Size = new System.Drawing.Size(64, 64);
            this.a_r_ck.TabIndex = 349;
            this.a_r_ck.TabStop = false;
            // 
            // b_g_ck
            // 
            this.b_g_ck.BackColor = System.Drawing.Color.Transparent;
            this.b_g_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.b_g_ck.Enabled = false;
            this.b_g_ck.ErrorImage = null;
            this.b_g_ck.InitialImage = null;
            this.b_g_ck.Location = new System.Drawing.Point(1775, 877);
            this.b_g_ck.Margin = new System.Windows.Forms.Padding(0);
            this.b_g_ck.Name = "b_g_ck";
            this.b_g_ck.Size = new System.Drawing.Size(64, 64);
            this.b_g_ck.TabIndex = 348;
            this.b_g_ck.TabStop = false;
            // 
            // b_r_ck
            // 
            this.b_r_ck.BackColor = System.Drawing.Color.Transparent;
            this.b_r_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.b_r_ck.Enabled = false;
            this.b_r_ck.ErrorImage = null;
            this.b_r_ck.InitialImage = null;
            this.b_r_ck.Location = new System.Drawing.Point(1775, 813);
            this.b_r_ck.Margin = new System.Windows.Forms.Padding(0);
            this.b_r_ck.Name = "b_r_ck";
            this.b_r_ck.Size = new System.Drawing.Size(64, 64);
            this.b_r_ck.TabIndex = 347;
            this.b_r_ck.TabStop = false;
            // 
            // g_a_ck
            // 
            this.g_a_ck.BackColor = System.Drawing.Color.Transparent;
            this.g_a_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.g_a_ck.Enabled = false;
            this.g_a_ck.ErrorImage = null;
            this.g_a_ck.InitialImage = null;
            this.g_a_ck.Location = new System.Drawing.Point(1711, 1005);
            this.g_a_ck.Margin = new System.Windows.Forms.Padding(0);
            this.g_a_ck.Name = "g_a_ck";
            this.g_a_ck.Size = new System.Drawing.Size(64, 64);
            this.g_a_ck.TabIndex = 354;
            this.g_a_ck.TabStop = false;
            // 
            // g_b_ck
            // 
            this.g_b_ck.BackColor = System.Drawing.Color.Transparent;
            this.g_b_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.g_b_ck.Enabled = false;
            this.g_b_ck.ErrorImage = null;
            this.g_b_ck.InitialImage = null;
            this.g_b_ck.Location = new System.Drawing.Point(1711, 941);
            this.g_b_ck.Margin = new System.Windows.Forms.Padding(0);
            this.g_b_ck.Name = "g_b_ck";
            this.g_b_ck.Size = new System.Drawing.Size(64, 64);
            this.g_b_ck.TabIndex = 353;
            this.g_b_ck.TabStop = false;
            // 
            // r_a_ck
            // 
            this.r_a_ck.BackColor = System.Drawing.Color.Transparent;
            this.r_a_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.r_a_ck.Enabled = false;
            this.r_a_ck.ErrorImage = null;
            this.r_a_ck.InitialImage = null;
            this.r_a_ck.Location = new System.Drawing.Point(1647, 1005);
            this.r_a_ck.Margin = new System.Windows.Forms.Padding(0);
            this.r_a_ck.Name = "r_a_ck";
            this.r_a_ck.Size = new System.Drawing.Size(64, 64);
            this.r_a_ck.TabIndex = 352;
            this.r_a_ck.TabStop = false;
            // 
            // r_b_ck
            // 
            this.r_b_ck.BackColor = System.Drawing.Color.Transparent;
            this.r_b_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.r_b_ck.Enabled = false;
            this.r_b_ck.ErrorImage = null;
            this.r_b_ck.InitialImage = null;
            this.r_b_ck.Location = new System.Drawing.Point(1647, 941);
            this.r_b_ck.Margin = new System.Windows.Forms.Padding(0);
            this.r_b_ck.Name = "r_b_ck";
            this.r_b_ck.Size = new System.Drawing.Size(64, 64);
            this.r_b_ck.TabIndex = 351;
            this.r_b_ck.TabStop = false;
            // 
            // a_a_ck
            // 
            this.a_a_ck.BackColor = System.Drawing.Color.Transparent;
            this.a_a_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.a_a_ck.Enabled = false;
            this.a_a_ck.ErrorImage = null;
            this.a_a_ck.InitialImage = null;
            this.a_a_ck.Location = new System.Drawing.Point(1839, 1005);
            this.a_a_ck.Margin = new System.Windows.Forms.Padding(0);
            this.a_a_ck.Name = "a_a_ck";
            this.a_a_ck.Size = new System.Drawing.Size(64, 64);
            this.a_a_ck.TabIndex = 358;
            this.a_a_ck.TabStop = false;
            // 
            // a_b_ck
            // 
            this.a_b_ck.BackColor = System.Drawing.Color.Transparent;
            this.a_b_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.a_b_ck.Enabled = false;
            this.a_b_ck.ErrorImage = null;
            this.a_b_ck.InitialImage = null;
            this.a_b_ck.Location = new System.Drawing.Point(1839, 941);
            this.a_b_ck.Margin = new System.Windows.Forms.Padding(0);
            this.a_b_ck.Name = "a_b_ck";
            this.a_b_ck.Size = new System.Drawing.Size(64, 64);
            this.a_b_ck.TabIndex = 357;
            this.a_b_ck.TabStop = false;
            // 
            // b_a_ck
            // 
            this.b_a_ck.BackColor = System.Drawing.Color.Transparent;
            this.b_a_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.b_a_ck.Enabled = false;
            this.b_a_ck.ErrorImage = null;
            this.b_a_ck.InitialImage = null;
            this.b_a_ck.Location = new System.Drawing.Point(1775, 1005);
            this.b_a_ck.Margin = new System.Windows.Forms.Padding(0);
            this.b_a_ck.Name = "b_a_ck";
            this.b_a_ck.Size = new System.Drawing.Size(64, 64);
            this.b_a_ck.TabIndex = 356;
            this.b_a_ck.TabStop = false;
            // 
            // b_b_ck
            // 
            this.b_b_ck.BackColor = System.Drawing.Color.Transparent;
            this.b_b_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.b_b_ck.Enabled = false;
            this.b_b_ck.ErrorImage = null;
            this.b_b_ck.InitialImage = null;
            this.b_b_ck.Location = new System.Drawing.Point(1775, 941);
            this.b_b_ck.Margin = new System.Windows.Forms.Padding(0);
            this.b_b_ck.Name = "b_b_ck";
            this.b_b_ck.Size = new System.Drawing.Size(64, 64);
            this.b_b_ck.TabIndex = 355;
            this.b_b_ck.TabStop = false;
            // 
            // colour_channels_label
            // 
            this.colour_channels_label.AutoSize = true;
            this.colour_channels_label.BackColor = System.Drawing.Color.Transparent;
            this.colour_channels_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.colour_channels_label.ForeColor = System.Drawing.SystemColors.Control;
            this.colour_channels_label.Location = new System.Drawing.Point(1674, 778);
            this.colour_channels_label.Name = "colour_channels_label";
            this.colour_channels_label.Size = new System.Drawing.Size(200, 20);
            this.colour_channels_label.TabIndex = 359;
            this.colour_channels_label.Text = "Colour Channels";
            // 
            // r_r_ck_hitbox
            // 
            this.r_r_ck_hitbox.AutoSize = true;
            this.r_r_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.r_r_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.r_r_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.r_r_ck_hitbox.Location = new System.Drawing.Point(1647, 813);
            this.r_r_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.r_r_ck_hitbox.Name = "r_r_ck_hitbox";
            this.r_r_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.r_r_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.r_r_ck_hitbox.TabIndex = 360;
            this.r_r_ck_hitbox.Click += new System.EventHandler(this.R_R_Click);
            this.r_r_ck_hitbox.MouseEnter += new System.EventHandler(this.R_R_MouseEnter);
            this.r_r_ck_hitbox.MouseLeave += new System.EventHandler(this.R_R_MouseLeave);
            // 
            // g_r_ck_hitbox
            // 
            this.g_r_ck_hitbox.AutoSize = true;
            this.g_r_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.g_r_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.g_r_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.g_r_ck_hitbox.Location = new System.Drawing.Point(1711, 813);
            this.g_r_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.g_r_ck_hitbox.Name = "g_r_ck_hitbox";
            this.g_r_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.g_r_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.g_r_ck_hitbox.TabIndex = 361;
            this.g_r_ck_hitbox.Click += new System.EventHandler(this.G_R_Click);
            this.g_r_ck_hitbox.MouseEnter += new System.EventHandler(this.G_R_MouseEnter);
            this.g_r_ck_hitbox.MouseLeave += new System.EventHandler(this.G_R_MouseLeave);
            // 
            // a_a_ck_hitbox
            // 
            this.a_a_ck_hitbox.AutoSize = true;
            this.a_a_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.a_a_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.a_a_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.a_a_ck_hitbox.Location = new System.Drawing.Point(1839, 1007);
            this.a_a_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.a_a_ck_hitbox.Name = "a_a_ck_hitbox";
            this.a_a_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.a_a_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.a_a_ck_hitbox.TabIndex = 362;
            this.a_a_ck_hitbox.Click += new System.EventHandler(this.A_A_Click);
            this.a_a_ck_hitbox.MouseEnter += new System.EventHandler(this.A_A_MouseEnter);
            this.a_a_ck_hitbox.MouseLeave += new System.EventHandler(this.A_A_MouseLeave);
            // 
            // r_g_ck_hitbox
            // 
            this.r_g_ck_hitbox.AutoSize = true;
            this.r_g_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.r_g_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.r_g_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.r_g_ck_hitbox.Location = new System.Drawing.Point(1647, 877);
            this.r_g_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.r_g_ck_hitbox.Name = "r_g_ck_hitbox";
            this.r_g_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.r_g_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.r_g_ck_hitbox.TabIndex = 363;
            this.r_g_ck_hitbox.Click += new System.EventHandler(this.R_G_Click);
            this.r_g_ck_hitbox.MouseEnter += new System.EventHandler(this.R_G_MouseEnter);
            this.r_g_ck_hitbox.MouseLeave += new System.EventHandler(this.R_G_MouseLeave);
            // 
            // r_b_ck_hitbox
            // 
            this.r_b_ck_hitbox.AutoSize = true;
            this.r_b_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.r_b_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.r_b_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.r_b_ck_hitbox.Location = new System.Drawing.Point(1647, 941);
            this.r_b_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.r_b_ck_hitbox.Name = "r_b_ck_hitbox";
            this.r_b_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.r_b_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.r_b_ck_hitbox.TabIndex = 364;
            this.r_b_ck_hitbox.Click += new System.EventHandler(this.R_B_Click);
            this.r_b_ck_hitbox.MouseEnter += new System.EventHandler(this.R_B_MouseEnter);
            this.r_b_ck_hitbox.MouseLeave += new System.EventHandler(this.R_B_MouseLeave);
            // 
            // r_a_ck_hitbox
            // 
            this.r_a_ck_hitbox.AutoSize = true;
            this.r_a_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.r_a_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.r_a_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.r_a_ck_hitbox.Location = new System.Drawing.Point(1647, 1007);
            this.r_a_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.r_a_ck_hitbox.Name = "r_a_ck_hitbox";
            this.r_a_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.r_a_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.r_a_ck_hitbox.TabIndex = 365;
            this.r_a_ck_hitbox.Click += new System.EventHandler(this.R_A_Click);
            this.r_a_ck_hitbox.MouseEnter += new System.EventHandler(this.R_A_MouseEnter);
            this.r_a_ck_hitbox.MouseLeave += new System.EventHandler(this.R_A_MouseLeave);
            // 
            // g_g_ck_hitbox
            // 
            this.g_g_ck_hitbox.AutoSize = true;
            this.g_g_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.g_g_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.g_g_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.g_g_ck_hitbox.Location = new System.Drawing.Point(1711, 877);
            this.g_g_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.g_g_ck_hitbox.Name = "g_g_ck_hitbox";
            this.g_g_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.g_g_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.g_g_ck_hitbox.TabIndex = 366;
            this.g_g_ck_hitbox.Click += new System.EventHandler(this.G_G_Click);
            this.g_g_ck_hitbox.MouseEnter += new System.EventHandler(this.G_G_MouseEnter);
            this.g_g_ck_hitbox.MouseLeave += new System.EventHandler(this.G_G_MouseLeave);
            // 
            // g_b_ck_hitbox
            // 
            this.g_b_ck_hitbox.AutoSize = true;
            this.g_b_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.g_b_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.g_b_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.g_b_ck_hitbox.Location = new System.Drawing.Point(1711, 941);
            this.g_b_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.g_b_ck_hitbox.Name = "g_b_ck_hitbox";
            this.g_b_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.g_b_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.g_b_ck_hitbox.TabIndex = 367;
            this.g_b_ck_hitbox.Click += new System.EventHandler(this.G_B_Click);
            this.g_b_ck_hitbox.MouseEnter += new System.EventHandler(this.G_B_MouseEnter);
            this.g_b_ck_hitbox.MouseLeave += new System.EventHandler(this.G_B_MouseLeave);
            // 
            // g_a_ck_hitbox
            // 
            this.g_a_ck_hitbox.AutoSize = true;
            this.g_a_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.g_a_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.g_a_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.g_a_ck_hitbox.Location = new System.Drawing.Point(1711, 1005);
            this.g_a_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.g_a_ck_hitbox.Name = "g_a_ck_hitbox";
            this.g_a_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.g_a_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.g_a_ck_hitbox.TabIndex = 368;
            this.g_a_ck_hitbox.Click += new System.EventHandler(this.G_A_Click);
            this.g_a_ck_hitbox.MouseEnter += new System.EventHandler(this.G_A_MouseEnter);
            this.g_a_ck_hitbox.MouseLeave += new System.EventHandler(this.G_A_MouseLeave);
            // 
            // b_r_ck_hitbox
            // 
            this.b_r_ck_hitbox.AutoSize = true;
            this.b_r_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.b_r_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.b_r_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.b_r_ck_hitbox.Location = new System.Drawing.Point(1775, 813);
            this.b_r_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.b_r_ck_hitbox.Name = "b_r_ck_hitbox";
            this.b_r_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.b_r_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.b_r_ck_hitbox.TabIndex = 369;
            this.b_r_ck_hitbox.Click += new System.EventHandler(this.B_R_Click);
            this.b_r_ck_hitbox.MouseEnter += new System.EventHandler(this.B_R_MouseEnter);
            this.b_r_ck_hitbox.MouseLeave += new System.EventHandler(this.B_R_MouseLeave);
            // 
            // b_g_ck_hitbox
            // 
            this.b_g_ck_hitbox.AutoSize = true;
            this.b_g_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.b_g_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.b_g_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.b_g_ck_hitbox.Location = new System.Drawing.Point(1775, 877);
            this.b_g_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.b_g_ck_hitbox.Name = "b_g_ck_hitbox";
            this.b_g_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.b_g_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.b_g_ck_hitbox.TabIndex = 370;
            this.b_g_ck_hitbox.Click += new System.EventHandler(this.B_G_Click);
            this.b_g_ck_hitbox.MouseEnter += new System.EventHandler(this.B_G_MouseEnter);
            this.b_g_ck_hitbox.MouseLeave += new System.EventHandler(this.B_G_MouseLeave);
            // 
            // b_b_ck_hitbox
            // 
            this.b_b_ck_hitbox.AutoSize = true;
            this.b_b_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.b_b_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.b_b_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.b_b_ck_hitbox.Location = new System.Drawing.Point(1775, 941);
            this.b_b_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.b_b_ck_hitbox.Name = "b_b_ck_hitbox";
            this.b_b_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.b_b_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.b_b_ck_hitbox.TabIndex = 371;
            this.b_b_ck_hitbox.Click += new System.EventHandler(this.B_B_Click);
            this.b_b_ck_hitbox.MouseEnter += new System.EventHandler(this.B_B_MouseEnter);
            this.b_b_ck_hitbox.MouseLeave += new System.EventHandler(this.B_B_MouseLeave);
            // 
            // b_a_ck_hitbox
            // 
            this.b_a_ck_hitbox.AutoSize = true;
            this.b_a_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.b_a_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.b_a_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.b_a_ck_hitbox.Location = new System.Drawing.Point(1775, 1005);
            this.b_a_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.b_a_ck_hitbox.Name = "b_a_ck_hitbox";
            this.b_a_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.b_a_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.b_a_ck_hitbox.TabIndex = 372;
            this.b_a_ck_hitbox.Click += new System.EventHandler(this.B_A_Click);
            this.b_a_ck_hitbox.MouseEnter += new System.EventHandler(this.B_A_MouseEnter);
            this.b_a_ck_hitbox.MouseLeave += new System.EventHandler(this.B_A_MouseLeave);
            // 
            // a_r_ck_hitbox
            // 
            this.a_r_ck_hitbox.AutoSize = true;
            this.a_r_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.a_r_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.a_r_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.a_r_ck_hitbox.Location = new System.Drawing.Point(1839, 813);
            this.a_r_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.a_r_ck_hitbox.Name = "a_r_ck_hitbox";
            this.a_r_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.a_r_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.a_r_ck_hitbox.TabIndex = 373;
            this.a_r_ck_hitbox.Click += new System.EventHandler(this.A_R_Click);
            this.a_r_ck_hitbox.MouseEnter += new System.EventHandler(this.A_R_MouseEnter);
            this.a_r_ck_hitbox.MouseLeave += new System.EventHandler(this.A_R_MouseLeave);
            // 
            // a_g_ck_hitbox
            // 
            this.a_g_ck_hitbox.AutoSize = true;
            this.a_g_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.a_g_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.a_g_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.a_g_ck_hitbox.Location = new System.Drawing.Point(1839, 877);
            this.a_g_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.a_g_ck_hitbox.Name = "a_g_ck_hitbox";
            this.a_g_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.a_g_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.a_g_ck_hitbox.TabIndex = 374;
            this.a_g_ck_hitbox.Click += new System.EventHandler(this.A_G_Click);
            this.a_g_ck_hitbox.MouseEnter += new System.EventHandler(this.A_G_MouseEnter);
            this.a_g_ck_hitbox.MouseLeave += new System.EventHandler(this.A_G_MouseLeave);
            // 
            // a_b_ck_hitbox
            // 
            this.a_b_ck_hitbox.AutoSize = true;
            this.a_b_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.a_b_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.a_b_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.a_b_ck_hitbox.Location = new System.Drawing.Point(1839, 941);
            this.a_b_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.a_b_ck_hitbox.Name = "a_b_ck_hitbox";
            this.a_b_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 44, 0, 0);
            this.a_b_ck_hitbox.Size = new System.Drawing.Size(64, 64);
            this.a_b_ck_hitbox.TabIndex = 375;
            this.a_b_ck_hitbox.Click += new System.EventHandler(this.A_B_Click);
            this.a_b_ck_hitbox.MouseEnter += new System.EventHandler(this.A_B_MouseEnter);
            this.a_b_ck_hitbox.MouseLeave += new System.EventHandler(this.A_B_MouseLeave);
            // 
            // view_alpha_ck
            // 
            this.view_alpha_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_alpha_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_alpha_ck.Enabled = false;
            this.view_alpha_ck.ErrorImage = null;
            this.view_alpha_ck.InitialImage = null;
            this.view_alpha_ck.Location = new System.Drawing.Point(40, 1131);
            this.view_alpha_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_alpha_ck.Name = "view_alpha_ck";
            this.view_alpha_ck.Size = new System.Drawing.Size(64, 64);
            this.view_alpha_ck.TabIndex = 378;
            this.view_alpha_ck.TabStop = false;
            this.view_alpha_ck.Visible = false;
            // 
            // view_alpha_label
            // 
            this.view_alpha_label.AutoSize = true;
            this.view_alpha_label.BackColor = System.Drawing.Color.Transparent;
            this.view_alpha_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_alpha_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_alpha_label.Location = new System.Drawing.Point(108, 1131);
            this.view_alpha_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_alpha_label.Name = "view_alpha_label";
            this.view_alpha_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 0);
            this.view_alpha_label.Size = new System.Drawing.Size(74, 42);
            this.view_alpha_label.TabIndex = 376;
            this.view_alpha_label.Text = "Alpha";
            this.view_alpha_label.Visible = false;
            this.view_alpha_label.Click += new System.EventHandler(this.view_alpha_Click);
            this.view_alpha_label.MouseEnter += new System.EventHandler(this.view_alpha_MouseEnter);
            this.view_alpha_label.MouseLeave += new System.EventHandler(this.view_alpha_MouseLeave);
            // 
            // view_alpha_hitbox
            // 
            this.view_alpha_hitbox.AutoSize = true;
            this.view_alpha_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.view_alpha_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_alpha_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.view_alpha_hitbox.Location = new System.Drawing.Point(37, 1131);
            this.view_alpha_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.view_alpha_hitbox.Name = "view_alpha_hitbox";
            this.view_alpha_hitbox.Padding = new System.Windows.Forms.Padding(190, 35, 0, 0);
            this.view_alpha_hitbox.Size = new System.Drawing.Size(190, 55);
            this.view_alpha_hitbox.TabIndex = 377;
            this.view_alpha_hitbox.Visible = false;
            this.view_alpha_hitbox.Click += new System.EventHandler(this.view_alpha_Click);
            this.view_alpha_hitbox.MouseEnter += new System.EventHandler(this.view_alpha_MouseEnter);
            this.view_alpha_hitbox.MouseLeave += new System.EventHandler(this.view_alpha_MouseLeave);
            // 
            // view_algorithm_ck
            // 
            this.view_algorithm_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_algorithm_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_algorithm_ck.Enabled = false;
            this.view_algorithm_ck.ErrorImage = null;
            this.view_algorithm_ck.InitialImage = null;
            this.view_algorithm_ck.Location = new System.Drawing.Point(239, 1127);
            this.view_algorithm_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_algorithm_ck.Name = "view_algorithm_ck";
            this.view_algorithm_ck.Size = new System.Drawing.Size(64, 64);
            this.view_algorithm_ck.TabIndex = 381;
            this.view_algorithm_ck.TabStop = false;
            this.view_algorithm_ck.Visible = false;
            // 
            // view_algorithm_label
            // 
            this.view_algorithm_label.AutoSize = true;
            this.view_algorithm_label.BackColor = System.Drawing.Color.Transparent;
            this.view_algorithm_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_algorithm_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_algorithm_label.Location = new System.Drawing.Point(307, 1127);
            this.view_algorithm_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_algorithm_label.Name = "view_algorithm_label";
            this.view_algorithm_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_algorithm_label.Size = new System.Drawing.Size(119, 64);
            this.view_algorithm_label.TabIndex = 379;
            this.view_algorithm_label.Text = "Algorithm";
            this.view_algorithm_label.Visible = false;
            this.view_algorithm_label.Click += new System.EventHandler(this.view_algorithm_Click);
            this.view_algorithm_label.MouseEnter += new System.EventHandler(this.view_algorithm_MouseEnter);
            this.view_algorithm_label.MouseLeave += new System.EventHandler(this.view_algorithm_MouseLeave);
            // 
            // view_algorithm_hitbox
            // 
            this.view_algorithm_hitbox.AutoSize = true;
            this.view_algorithm_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.view_algorithm_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_algorithm_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.view_algorithm_hitbox.Location = new System.Drawing.Point(236, 1127);
            this.view_algorithm_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.view_algorithm_hitbox.Name = "view_algorithm_hitbox";
            this.view_algorithm_hitbox.Padding = new System.Windows.Forms.Padding(190, 35, 0, 0);
            this.view_algorithm_hitbox.Size = new System.Drawing.Size(190, 55);
            this.view_algorithm_hitbox.TabIndex = 380;
            this.view_algorithm_hitbox.Visible = false;
            this.view_algorithm_hitbox.Click += new System.EventHandler(this.view_algorithm_Click);
            this.view_algorithm_hitbox.MouseEnter += new System.EventHandler(this.view_algorithm_MouseEnter);
            this.view_algorithm_hitbox.MouseLeave += new System.EventHandler(this.view_algorithm_MouseLeave);
            // 
            // view_WrapS_ck
            // 
            this.view_WrapS_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_WrapS_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_WrapS_ck.Enabled = false;
            this.view_WrapS_ck.ErrorImage = null;
            this.view_WrapS_ck.InitialImage = null;
            this.view_WrapS_ck.Location = new System.Drawing.Point(40, 1180);
            this.view_WrapS_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_WrapS_ck.Name = "view_WrapS_ck";
            this.view_WrapS_ck.Size = new System.Drawing.Size(64, 64);
            this.view_WrapS_ck.TabIndex = 384;
            this.view_WrapS_ck.TabStop = false;
            this.view_WrapS_ck.Visible = false;
            // 
            // view_WrapS_label
            // 
            this.view_WrapS_label.AutoSize = true;
            this.view_WrapS_label.BackColor = System.Drawing.Color.Transparent;
            this.view_WrapS_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_WrapS_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_WrapS_label.Location = new System.Drawing.Point(108, 1180);
            this.view_WrapS_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_WrapS_label.Name = "view_WrapS_label";
            this.view_WrapS_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 0);
            this.view_WrapS_label.Size = new System.Drawing.Size(83, 42);
            this.view_WrapS_label.TabIndex = 382;
            this.view_WrapS_label.Text = "WrapS";
            this.view_WrapS_label.Visible = false;
            this.view_WrapS_label.Click += new System.EventHandler(this.view_WrapS_Click);
            this.view_WrapS_label.MouseEnter += new System.EventHandler(this.view_WrapS_MouseEnter);
            this.view_WrapS_label.MouseLeave += new System.EventHandler(this.view_WrapS_MouseLeave);
            // 
            // view_WrapS_hitbox
            // 
            this.view_WrapS_hitbox.AutoSize = true;
            this.view_WrapS_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.view_WrapS_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_WrapS_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.view_WrapS_hitbox.Location = new System.Drawing.Point(37, 1180);
            this.view_WrapS_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.view_WrapS_hitbox.Name = "view_WrapS_hitbox";
            this.view_WrapS_hitbox.Padding = new System.Windows.Forms.Padding(190, 35, 0, 0);
            this.view_WrapS_hitbox.Size = new System.Drawing.Size(190, 55);
            this.view_WrapS_hitbox.TabIndex = 383;
            this.view_WrapS_hitbox.Visible = false;
            this.view_WrapS_hitbox.Click += new System.EventHandler(this.view_WrapS_Click);
            this.view_WrapS_hitbox.MouseEnter += new System.EventHandler(this.view_WrapS_MouseEnter);
            this.view_WrapS_hitbox.MouseLeave += new System.EventHandler(this.view_WrapS_MouseLeave);
            // 
            // view_WrapT_ck
            // 
            this.view_WrapT_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_WrapT_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_WrapT_ck.Enabled = false;
            this.view_WrapT_ck.ErrorImage = null;
            this.view_WrapT_ck.InitialImage = null;
            this.view_WrapT_ck.Location = new System.Drawing.Point(239, 1180);
            this.view_WrapT_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_WrapT_ck.Name = "view_WrapT_ck";
            this.view_WrapT_ck.Size = new System.Drawing.Size(64, 64);
            this.view_WrapT_ck.TabIndex = 387;
            this.view_WrapT_ck.TabStop = false;
            this.view_WrapT_ck.Visible = false;
            // 
            // view_WrapT_label
            // 
            this.view_WrapT_label.AutoSize = true;
            this.view_WrapT_label.BackColor = System.Drawing.Color.Transparent;
            this.view_WrapT_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_WrapT_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_WrapT_label.Location = new System.Drawing.Point(307, 1180);
            this.view_WrapT_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_WrapT_label.Name = "view_WrapT_label";
            this.view_WrapT_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_WrapT_label.Size = new System.Drawing.Size(83, 64);
            this.view_WrapT_label.TabIndex = 385;
            this.view_WrapT_label.Text = "WrapT";
            this.view_WrapT_label.Visible = false;
            this.view_WrapT_label.Click += new System.EventHandler(this.view_WrapT_Click);
            this.view_WrapT_label.MouseEnter += new System.EventHandler(this.view_WrapT_MouseEnter);
            this.view_WrapT_label.MouseLeave += new System.EventHandler(this.view_WrapT_MouseLeave);
            // 
            // view_WrapT_hitbox
            // 
            this.view_WrapT_hitbox.AutoSize = true;
            this.view_WrapT_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.view_WrapT_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_WrapT_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.view_WrapT_hitbox.Location = new System.Drawing.Point(236, 1180);
            this.view_WrapT_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.view_WrapT_hitbox.Name = "view_WrapT_hitbox";
            this.view_WrapT_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.view_WrapT_hitbox.Size = new System.Drawing.Size(190, 64);
            this.view_WrapT_hitbox.TabIndex = 386;
            this.view_WrapT_hitbox.Visible = false;
            this.view_WrapT_hitbox.Click += new System.EventHandler(this.view_WrapT_Click);
            this.view_WrapT_hitbox.MouseEnter += new System.EventHandler(this.view_WrapT_MouseEnter);
            this.view_WrapT_hitbox.MouseLeave += new System.EventHandler(this.view_WrapT_MouseLeave);
            // 
            // view_mag_ck
            // 
            this.view_mag_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_mag_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_mag_ck.Enabled = false;
            this.view_mag_ck.ErrorImage = null;
            this.view_mag_ck.InitialImage = null;
            this.view_mag_ck.Location = new System.Drawing.Point(239, 1229);
            this.view_mag_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_mag_ck.Name = "view_mag_ck";
            this.view_mag_ck.Size = new System.Drawing.Size(64, 64);
            this.view_mag_ck.TabIndex = 393;
            this.view_mag_ck.TabStop = false;
            this.view_mag_ck.Visible = false;
            // 
            // view_mag_label
            // 
            this.view_mag_label.AutoSize = true;
            this.view_mag_label.BackColor = System.Drawing.Color.Transparent;
            this.view_mag_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_mag_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_mag_label.Location = new System.Drawing.Point(307, 1229);
            this.view_mag_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_mag_label.Name = "view_mag_label";
            this.view_mag_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_mag_label.Size = new System.Drawing.Size(121, 64);
            this.view_mag_label.TabIndex = 391;
            this.view_mag_label.Text = "Mag filter";
            this.view_mag_label.Visible = false;
            this.view_mag_label.Click += new System.EventHandler(this.view_mag_Click);
            this.view_mag_label.MouseEnter += new System.EventHandler(this.view_mag_MouseEnter);
            this.view_mag_label.MouseLeave += new System.EventHandler(this.view_mag_MouseLeave);
            // 
            // view_mag_hitbox
            // 
            this.view_mag_hitbox.AutoSize = true;
            this.view_mag_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.view_mag_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_mag_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.view_mag_hitbox.Location = new System.Drawing.Point(236, 1229);
            this.view_mag_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.view_mag_hitbox.Name = "view_mag_hitbox";
            this.view_mag_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.view_mag_hitbox.Size = new System.Drawing.Size(190, 64);
            this.view_mag_hitbox.TabIndex = 392;
            this.view_mag_hitbox.Visible = false;
            this.view_mag_hitbox.Click += new System.EventHandler(this.view_mag_Click);
            this.view_mag_hitbox.MouseEnter += new System.EventHandler(this.view_mag_MouseEnter);
            this.view_mag_hitbox.MouseLeave += new System.EventHandler(this.view_mag_MouseLeave);
            // 
            // view_min_ck
            // 
            this.view_min_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_min_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_min_ck.Enabled = false;
            this.view_min_ck.ErrorImage = null;
            this.view_min_ck.InitialImage = null;
            this.view_min_ck.Location = new System.Drawing.Point(40, 1229);
            this.view_min_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_min_ck.Name = "view_min_ck";
            this.view_min_ck.Size = new System.Drawing.Size(64, 64);
            this.view_min_ck.TabIndex = 390;
            this.view_min_ck.TabStop = false;
            this.view_min_ck.Visible = false;
            // 
            // view_min_label
            // 
            this.view_min_label.AutoSize = true;
            this.view_min_label.BackColor = System.Drawing.Color.Transparent;
            this.view_min_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_min_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_min_label.Location = new System.Drawing.Point(108, 1229);
            this.view_min_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_min_label.Name = "view_min_label";
            this.view_min_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_min_label.Size = new System.Drawing.Size(112, 64);
            this.view_min_label.TabIndex = 388;
            this.view_min_label.Text = "Min filter";
            this.view_min_label.Visible = false;
            this.view_min_label.Click += new System.EventHandler(this.view_min_Click);
            this.view_min_label.MouseEnter += new System.EventHandler(this.view_min_MouseEnter);
            this.view_min_label.MouseLeave += new System.EventHandler(this.view_min_MouseLeave);
            // 
            // view_min_hitbox
            // 
            this.view_min_hitbox.AutoSize = true;
            this.view_min_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.view_min_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_min_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.view_min_hitbox.Location = new System.Drawing.Point(37, 1229);
            this.view_min_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.view_min_hitbox.Name = "view_min_hitbox";
            this.view_min_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.view_min_hitbox.Size = new System.Drawing.Size(190, 64);
            this.view_min_hitbox.TabIndex = 389;
            this.view_min_hitbox.Visible = false;
            this.view_min_hitbox.Click += new System.EventHandler(this.view_min_Click);
            this.view_min_hitbox.MouseEnter += new System.EventHandler(this.view_min_MouseEnter);
            this.view_min_hitbox.MouseLeave += new System.EventHandler(this.view_min_MouseLeave);
            // 
            // banner_ck
            // 
            this.banner_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_ck.Enabled = false;
            this.banner_ck.ErrorImage = null;
            this.banner_ck.InitialImage = null;
            this.banner_ck.Location = new System.Drawing.Point(0, 0);
            this.banner_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_ck.Name = "banner_ck";
            this.banner_ck.Size = new System.Drawing.Size(1920, 32);
            this.banner_ck.TabIndex = 394;
            this.banner_ck.TabStop = false;
            // 
            // all_hitbox
            // 
            this.all_hitbox.AutoSize = true;
            this.all_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.all_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.all_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.all_hitbox.Location = new System.Drawing.Point(48, 0);
            this.all_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.all_hitbox.Name = "all_hitbox";
            this.all_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.all_hitbox.Size = new System.Drawing.Size(32, 32);
            this.all_hitbox.TabIndex = 395;
            this.all_hitbox.Click += new System.EventHandler(this.All_Click);
            this.all_hitbox.MouseEnter += new System.EventHandler(this.All_MouseEnter);
            this.all_hitbox.MouseLeave += new System.EventHandler(this.All_MouseLeave);
            // 
            // preview_hitbox
            // 
            this.preview_hitbox.AutoSize = true;
            this.preview_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.preview_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.preview_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.preview_hitbox.Location = new System.Drawing.Point(176, 0);
            this.preview_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.preview_hitbox.Name = "preview_hitbox";
            this.preview_hitbox.Padding = new System.Windows.Forms.Padding(96, 6, 0, 6);
            this.preview_hitbox.Size = new System.Drawing.Size(96, 32);
            this.preview_hitbox.TabIndex = 396;
            this.preview_hitbox.Click += new System.EventHandler(this.Preview_Click);
            this.preview_hitbox.MouseEnter += new System.EventHandler(this.Preview_MouseEnter);
            this.preview_hitbox.MouseLeave += new System.EventHandler(this.Preview_MouseLeave);
            // 
            // paint_hitbox
            // 
            this.paint_hitbox.AutoSize = true;
            this.paint_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.paint_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.paint_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.paint_hitbox.Location = new System.Drawing.Point(272, 0);
            this.paint_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.paint_hitbox.Name = "paint_hitbox";
            this.paint_hitbox.Padding = new System.Windows.Forms.Padding(96, 6, 0, 6);
            this.paint_hitbox.Size = new System.Drawing.Size(96, 32);
            this.paint_hitbox.TabIndex = 397;
            this.paint_hitbox.Click += new System.EventHandler(this.Paint_Click);
            this.paint_hitbox.MouseEnter += new System.EventHandler(this.Paint_MouseEnter);
            this.paint_hitbox.MouseLeave += new System.EventHandler(this.Paint_MouseLeave);
            // 
            // auto_hitbox
            // 
            this.auto_hitbox.AutoSize = true;
            this.auto_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.auto_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.auto_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.auto_hitbox.Location = new System.Drawing.Point(80, 0);
            this.auto_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.auto_hitbox.Name = "auto_hitbox";
            this.auto_hitbox.Padding = new System.Windows.Forms.Padding(96, 6, 0, 6);
            this.auto_hitbox.Size = new System.Drawing.Size(96, 32);
            this.auto_hitbox.TabIndex = 398;
            this.auto_hitbox.Click += new System.EventHandler(this.Auto_Click);
            this.auto_hitbox.MouseEnter += new System.EventHandler(this.Auto_MouseEnter);
            this.auto_hitbox.MouseLeave += new System.EventHandler(this.Auto_MouseLeave);
            // 
            // all_ck
            // 
            this.all_ck.BackColor = System.Drawing.Color.Transparent;
            this.all_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.all_ck.Enabled = false;
            this.all_ck.ErrorImage = null;
            this.all_ck.InitialImage = null;
            this.all_ck.Location = new System.Drawing.Point(48, 0);
            this.all_ck.Margin = new System.Windows.Forms.Padding(0);
            this.all_ck.Name = "all_ck";
            this.all_ck.Size = new System.Drawing.Size(32, 32);
            this.all_ck.TabIndex = 399;
            this.all_ck.TabStop = false;
            // 
            // preview_ck
            // 
            this.preview_ck.BackColor = System.Drawing.Color.Transparent;
            this.preview_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.preview_ck.Enabled = false;
            this.preview_ck.ErrorImage = null;
            this.preview_ck.InitialImage = null;
            this.preview_ck.Location = new System.Drawing.Point(176, 0);
            this.preview_ck.Margin = new System.Windows.Forms.Padding(0);
            this.preview_ck.Name = "preview_ck";
            this.preview_ck.Size = new System.Drawing.Size(96, 32);
            this.preview_ck.TabIndex = 400;
            this.preview_ck.TabStop = false;
            // 
            // auto_ck
            // 
            this.auto_ck.BackColor = System.Drawing.Color.Transparent;
            this.auto_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.auto_ck.Enabled = false;
            this.auto_ck.ErrorImage = null;
            this.auto_ck.InitialImage = null;
            this.auto_ck.Location = new System.Drawing.Point(80, 0);
            this.auto_ck.Margin = new System.Windows.Forms.Padding(0);
            this.auto_ck.Name = "auto_ck";
            this.auto_ck.Size = new System.Drawing.Size(96, 32);
            this.auto_ck.TabIndex = 401;
            this.auto_ck.TabStop = false;
            // 
            // paint_ck
            // 
            this.paint_ck.BackColor = System.Drawing.Color.Transparent;
            this.paint_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.paint_ck.Enabled = false;
            this.paint_ck.ErrorImage = null;
            this.paint_ck.InitialImage = null;
            this.paint_ck.Location = new System.Drawing.Point(272, 0);
            this.paint_ck.Margin = new System.Windows.Forms.Padding(0);
            this.paint_ck.Name = "paint_ck";
            this.paint_ck.Size = new System.Drawing.Size(96, 32);
            this.paint_ck.TabIndex = 402;
            this.paint_ck.TabStop = false;
            // 
            // banner_x_ck
            // 
            this.banner_x_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_x_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_x_ck.Enabled = false;
            this.banner_x_ck.ErrorImage = null;
            this.banner_x_ck.InitialImage = null;
            this.banner_x_ck.Location = new System.Drawing.Point(1888, 0);
            this.banner_x_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_x_ck.Name = "banner_x_ck";
            this.banner_x_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_x_ck.TabIndex = 404;
            this.banner_x_ck.TabStop = false;
            // 
            // banner_x_hitbox
            // 
            this.banner_x_hitbox.AutoSize = true;
            this.banner_x_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.banner_x_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_x_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_x_hitbox.Location = new System.Drawing.Point(1888, 0);
            this.banner_x_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.banner_x_hitbox.Name = "banner_x_hitbox";
            this.banner_x_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.banner_x_hitbox.Size = new System.Drawing.Size(32, 32);
            this.banner_x_hitbox.TabIndex = 403;
            this.banner_x_hitbox.Click += new System.EventHandler(this.Close_Click);
            this.banner_x_hitbox.MouseEnter += new System.EventHandler(this.Close_MouseEnter);
            this.banner_x_hitbox.MouseLeave += new System.EventHandler(this.Close_MouseLeave);
            // 
            // banner_5_ck
            // 
            this.banner_5_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_5_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_5_ck.Enabled = false;
            this.banner_5_ck.ErrorImage = null;
            this.banner_5_ck.InitialImage = null;
            this.banner_5_ck.Location = new System.Drawing.Point(1856, 0);
            this.banner_5_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_5_ck.Name = "banner_5_ck";
            this.banner_5_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_5_ck.TabIndex = 406;
            this.banner_5_ck.TabStop = false;
            // 
            // banner_5_hitbox
            // 
            this.banner_5_hitbox.AutoSize = true;
            this.banner_5_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.banner_5_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_5_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_5_hitbox.Location = new System.Drawing.Point(1856, 0);
            this.banner_5_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.banner_5_hitbox.Name = "banner_5_hitbox";
            this.banner_5_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.banner_5_hitbox.Size = new System.Drawing.Size(32, 32);
            this.banner_5_hitbox.TabIndex = 405;
            this.banner_5_hitbox.Click += new System.EventHandler(this.Maximized_Click);
            this.banner_5_hitbox.MouseEnter += new System.EventHandler(this.Maximized_MouseEnter);
            this.banner_5_hitbox.MouseLeave += new System.EventHandler(this.Maximized_MouseLeave);
            // 
            // banner_minus_ck
            // 
            this.banner_minus_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_minus_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_minus_ck.Enabled = false;
            this.banner_minus_ck.ErrorImage = null;
            this.banner_minus_ck.InitialImage = null;
            this.banner_minus_ck.Location = new System.Drawing.Point(1824, 0);
            this.banner_minus_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_minus_ck.Name = "banner_minus_ck";
            this.banner_minus_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_minus_ck.TabIndex = 408;
            this.banner_minus_ck.TabStop = false;
            // 
            // banner_minus_hitbox
            // 
            this.banner_minus_hitbox.AutoSize = true;
            this.banner_minus_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.banner_minus_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_minus_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_minus_hitbox.Location = new System.Drawing.Point(1824, 0);
            this.banner_minus_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.banner_minus_hitbox.Name = "banner_minus_hitbox";
            this.banner_minus_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.banner_minus_hitbox.Size = new System.Drawing.Size(32, 32);
            this.banner_minus_hitbox.TabIndex = 407;
            this.banner_minus_hitbox.Click += new System.EventHandler(this.Minimized_Click);
            this.banner_minus_hitbox.MouseEnter += new System.EventHandler(this.Minimized_MouseEnter);
            this.banner_minus_hitbox.MouseLeave += new System.EventHandler(this.Minimized_MouseLeave);
            // 
            // banner_9_ck
            // 
            this.banner_9_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_9_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_9_ck.Enabled = false;
            this.banner_9_ck.ErrorImage = null;
            this.banner_9_ck.InitialImage = null;
            this.banner_9_ck.Location = new System.Drawing.Point(480, 0);
            this.banner_9_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_9_ck.Name = "banner_9_ck";
            this.banner_9_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_9_ck.TabIndex = 412;
            this.banner_9_ck.TabStop = false;
            // 
            // banner_9_hitbox
            // 
            this.banner_9_hitbox.AutoSize = true;
            this.banner_9_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.banner_9_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_9_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_9_hitbox.Location = new System.Drawing.Point(480, 0);
            this.banner_9_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.banner_9_hitbox.Name = "banner_9_hitbox";
            this.banner_9_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.banner_9_hitbox.Size = new System.Drawing.Size(32, 32);
            this.banner_9_hitbox.TabIndex = 411;
            this.banner_9_hitbox.Click += new System.EventHandler(this.Top_right_Click);
            this.banner_9_hitbox.MouseEnter += new System.EventHandler(this.Top_right_MouseEnter);
            this.banner_9_hitbox.MouseLeave += new System.EventHandler(this.Top_right_MouseLeave);
            // 
            // banner_8_ck
            // 
            this.banner_8_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_8_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_8_ck.Enabled = false;
            this.banner_8_ck.ErrorImage = null;
            this.banner_8_ck.InitialImage = null;
            this.banner_8_ck.Location = new System.Drawing.Point(448, 0);
            this.banner_8_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_8_ck.Name = "banner_8_ck";
            this.banner_8_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_8_ck.TabIndex = 414;
            this.banner_8_ck.TabStop = false;
            // 
            // banner_8_hitbox
            // 
            this.banner_8_hitbox.AutoSize = true;
            this.banner_8_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.banner_8_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_8_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_8_hitbox.Location = new System.Drawing.Point(448, 0);
            this.banner_8_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.banner_8_hitbox.Name = "banner_8_hitbox";
            this.banner_8_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.banner_8_hitbox.Size = new System.Drawing.Size(32, 32);
            this.banner_8_hitbox.TabIndex = 413;
            this.banner_8_hitbox.Click += new System.EventHandler(this.Top_Click);
            this.banner_8_hitbox.MouseEnter += new System.EventHandler(this.Top_MouseEnter);
            this.banner_8_hitbox.MouseLeave += new System.EventHandler(this.Top_MouseLeave);
            // 
            // banner_7_ck
            // 
            this.banner_7_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_7_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_7_ck.Enabled = false;
            this.banner_7_ck.ErrorImage = null;
            this.banner_7_ck.InitialImage = null;
            this.banner_7_ck.Location = new System.Drawing.Point(416, 0);
            this.banner_7_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_7_ck.Name = "banner_7_ck";
            this.banner_7_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_7_ck.TabIndex = 416;
            this.banner_7_ck.TabStop = false;
            // 
            // banner_7_hitbox
            // 
            this.banner_7_hitbox.AutoSize = true;
            this.banner_7_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.banner_7_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_7_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_7_hitbox.Location = new System.Drawing.Point(416, 0);
            this.banner_7_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.banner_7_hitbox.Name = "banner_7_hitbox";
            this.banner_7_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.banner_7_hitbox.Size = new System.Drawing.Size(32, 32);
            this.banner_7_hitbox.TabIndex = 415;
            this.banner_7_hitbox.Click += new System.EventHandler(this.Top_left_Click);
            this.banner_7_hitbox.MouseEnter += new System.EventHandler(this.Top_left_MouseEnter);
            this.banner_7_hitbox.MouseLeave += new System.EventHandler(this.Top_left_MouseLeave);
            // 
            // banner_6_ck
            // 
            this.banner_6_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_6_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_6_ck.Enabled = false;
            this.banner_6_ck.ErrorImage = null;
            this.banner_6_ck.InitialImage = null;
            this.banner_6_ck.Location = new System.Drawing.Point(512, 0);
            this.banner_6_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_6_ck.Name = "banner_6_ck";
            this.banner_6_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_6_ck.TabIndex = 418;
            this.banner_6_ck.TabStop = false;
            // 
            // banner_6_hitbox
            // 
            this.banner_6_hitbox.AutoSize = true;
            this.banner_6_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.banner_6_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_6_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_6_hitbox.Location = new System.Drawing.Point(512, 0);
            this.banner_6_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.banner_6_hitbox.Name = "banner_6_hitbox";
            this.banner_6_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.banner_6_hitbox.Size = new System.Drawing.Size(32, 32);
            this.banner_6_hitbox.TabIndex = 417;
            this.banner_6_hitbox.Click += new System.EventHandler(this.Right_Click);
            this.banner_6_hitbox.MouseEnter += new System.EventHandler(this.Right_MouseEnter);
            this.banner_6_hitbox.MouseLeave += new System.EventHandler(this.Right_MouseLeave);
            // 
            // banner_4_ck
            // 
            this.banner_4_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_4_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_4_ck.Enabled = false;
            this.banner_4_ck.ErrorImage = null;
            this.banner_4_ck.InitialImage = null;
            this.banner_4_ck.Location = new System.Drawing.Point(384, 0);
            this.banner_4_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_4_ck.Name = "banner_4_ck";
            this.banner_4_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_4_ck.TabIndex = 420;
            this.banner_4_ck.TabStop = false;
            // 
            // banner_4_hitbox
            // 
            this.banner_4_hitbox.AutoSize = true;
            this.banner_4_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.banner_4_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_4_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_4_hitbox.Location = new System.Drawing.Point(384, 0);
            this.banner_4_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.banner_4_hitbox.Name = "banner_4_hitbox";
            this.banner_4_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.banner_4_hitbox.Size = new System.Drawing.Size(32, 32);
            this.banner_4_hitbox.TabIndex = 419;
            this.banner_4_hitbox.Click += new System.EventHandler(this.Left_Click);
            this.banner_4_hitbox.MouseEnter += new System.EventHandler(this.Left_MouseEnter);
            this.banner_4_hitbox.MouseLeave += new System.EventHandler(this.Left_MouseLeave);
            // 
            // banner_3_ck
            // 
            this.banner_3_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_3_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_3_ck.Enabled = false;
            this.banner_3_ck.ErrorImage = null;
            this.banner_3_ck.InitialImage = null;
            this.banner_3_ck.Location = new System.Drawing.Point(544, 0);
            this.banner_3_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_3_ck.Name = "banner_3_ck";
            this.banner_3_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_3_ck.TabIndex = 422;
            this.banner_3_ck.TabStop = false;
            // 
            // banner_3_hitbox
            // 
            this.banner_3_hitbox.AutoSize = true;
            this.banner_3_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.banner_3_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_3_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_3_hitbox.Location = new System.Drawing.Point(544, 0);
            this.banner_3_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.banner_3_hitbox.Name = "banner_3_hitbox";
            this.banner_3_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.banner_3_hitbox.Size = new System.Drawing.Size(32, 32);
            this.banner_3_hitbox.TabIndex = 421;
            this.banner_3_hitbox.Click += new System.EventHandler(this.Bottom_right_Click);
            this.banner_3_hitbox.MouseEnter += new System.EventHandler(this.Bottom_right_MouseEnter);
            this.banner_3_hitbox.MouseLeave += new System.EventHandler(this.Bottom_right_MouseLeave);
            // 
            // banner_2_ck
            // 
            this.banner_2_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_2_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_2_ck.Enabled = false;
            this.banner_2_ck.ErrorImage = null;
            this.banner_2_ck.InitialImage = null;
            this.banner_2_ck.Location = new System.Drawing.Point(576, 0);
            this.banner_2_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_2_ck.Name = "banner_2_ck";
            this.banner_2_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_2_ck.TabIndex = 424;
            this.banner_2_ck.TabStop = false;
            // 
            // banner_2_hitbox
            // 
            this.banner_2_hitbox.AutoSize = true;
            this.banner_2_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.banner_2_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_2_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_2_hitbox.Location = new System.Drawing.Point(576, 0);
            this.banner_2_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.banner_2_hitbox.Name = "banner_2_hitbox";
            this.banner_2_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.banner_2_hitbox.Size = new System.Drawing.Size(32, 32);
            this.banner_2_hitbox.TabIndex = 423;
            this.banner_2_hitbox.Click += new System.EventHandler(this.Bottom_Click);
            this.banner_2_hitbox.MouseEnter += new System.EventHandler(this.Bottom_MouseEnter);
            this.banner_2_hitbox.MouseLeave += new System.EventHandler(this.Bottom_MouseLeave);
            // 
            // banner_1_ck
            // 
            this.banner_1_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_1_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_1_ck.Enabled = false;
            this.banner_1_ck.ErrorImage = null;
            this.banner_1_ck.InitialImage = null;
            this.banner_1_ck.Location = new System.Drawing.Point(608, 0);
            this.banner_1_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_1_ck.Name = "banner_1_ck";
            this.banner_1_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_1_ck.TabIndex = 426;
            this.banner_1_ck.TabStop = false;
            // 
            // banner_1_hitbox
            // 
            this.banner_1_hitbox.AutoSize = true;
            this.banner_1_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.banner_1_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_1_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_1_hitbox.Location = new System.Drawing.Point(608, 0);
            this.banner_1_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.banner_1_hitbox.Name = "banner_1_hitbox";
            this.banner_1_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.banner_1_hitbox.Size = new System.Drawing.Size(32, 32);
            this.banner_1_hitbox.TabIndex = 425;
            this.banner_1_hitbox.Click += new System.EventHandler(this.Bottom_left_Click);
            this.banner_1_hitbox.MouseEnter += new System.EventHandler(this.Bottom_left_MouseEnter);
            this.banner_1_hitbox.MouseLeave += new System.EventHandler(this.Bottom_left_MouseLeave);
            // 
            // cli_textbox_ck
            // 
            this.cli_textbox_ck.BackColor = System.Drawing.Color.Transparent;
            this.cli_textbox_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cli_textbox_ck.Enabled = false;
            this.cli_textbox_ck.ErrorImage = null;
            this.cli_textbox_ck.InitialImage = null;
            this.cli_textbox_ck.Location = new System.Drawing.Point(9, 1005);
            this.cli_textbox_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cli_textbox_ck.MaximumSize = new System.Drawing.Size(1472, 64);
            this.cli_textbox_ck.MinimumSize = new System.Drawing.Size(1472, 64);
            this.cli_textbox_ck.Name = "cli_textbox_ck";
            this.cli_textbox_ck.Size = new System.Drawing.Size(1472, 64);
            this.cli_textbox_ck.TabIndex = 427;
            this.cli_textbox_ck.TabStop = false;
            // 
            // run_ck
            // 
            this.run_ck.BackColor = System.Drawing.Color.Transparent;
            this.run_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.run_ck.Enabled = false;
            this.run_ck.ErrorImage = null;
            this.run_ck.InitialImage = null;
            this.run_ck.Location = new System.Drawing.Point(1500, 1005);
            this.run_ck.Margin = new System.Windows.Forms.Padding(0);
            this.run_ck.Name = "run_ck";
            this.run_ck.Size = new System.Drawing.Size(128, 64);
            this.run_ck.TabIndex = 428;
            this.run_ck.TabStop = false;
            // 
            // run_hitbox
            // 
            this.run_hitbox.AutoSize = true;
            this.run_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.run_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.run_hitbox.ForeColor = System.Drawing.SystemColors.Window;
            this.run_hitbox.Location = new System.Drawing.Point(1500, 1005);
            this.run_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.run_hitbox.Name = "run_hitbox";
            this.run_hitbox.Padding = new System.Windows.Forms.Padding(128, 44, 0, 0);
            this.run_hitbox.Size = new System.Drawing.Size(128, 64);
            this.run_hitbox.TabIndex = 429;
            this.run_hitbox.Click += new System.EventHandler(this.Run_Click);
            this.run_hitbox.MouseEnter += new System.EventHandler(this.run_MouseEnter);
            this.run_hitbox.MouseLeave += new System.EventHandler(this.run_MouseLeave);
            // 
            // cli_textbox_hitbox
            // 
            this.cli_textbox_hitbox.AutoSize = true;
            this.cli_textbox_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.cli_textbox_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cli_textbox_hitbox.ForeColor = System.Drawing.SystemColors.Window;
            this.cli_textbox_hitbox.Location = new System.Drawing.Point(9, 1005);
            this.cli_textbox_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.cli_textbox_hitbox.Name = "cli_textbox_hitbox";
            this.cli_textbox_hitbox.Padding = new System.Windows.Forms.Padding(1472, 44, 0, 0);
            this.cli_textbox_hitbox.Size = new System.Drawing.Size(1472, 64);
            this.cli_textbox_hitbox.TabIndex = 430;
            this.cli_textbox_hitbox.MouseEnter += new System.EventHandler(this.cli_textbox_MouseEnter);
            this.cli_textbox_hitbox.MouseLeave += new System.EventHandler(this.cli_textbox_MouseLeave);
            // 
            // input_file_txt
            // 
            this.input_file_txt.BackColor = System.Drawing.Color.Black;
            this.input_file_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.input_file_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.input_file_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.input_file_txt.Location = new System.Drawing.Point(16, 972);
            this.input_file_txt.Margin = new System.Windows.Forms.Padding(0);
            this.input_file_txt.Name = "input_file_txt";
            this.input_file_txt.Size = new System.Drawing.Size(116, 21);
            this.input_file_txt.TabIndex = 0;
            this.input_file_txt.TextChanged += new System.EventHandler(this.input_file_TextChanged);
            this.input_file_txt.DoubleClick += new System.EventHandler(this.input_file_Click);
            this.input_file_txt.MouseEnter += new System.EventHandler(this.input_file_MouseEnter);
            this.input_file_txt.MouseLeave += new System.EventHandler(this.input_file_MouseLeave);
            // 
            // input_file_label
            // 
            this.input_file_label.AutoSize = true;
            this.input_file_label.BackColor = System.Drawing.Color.Transparent;
            this.input_file_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.input_file_label.ForeColor = System.Drawing.SystemColors.Control;
            this.input_file_label.Location = new System.Drawing.Point(21, 941);
            this.input_file_label.Margin = new System.Windows.Forms.Padding(0);
            this.input_file_label.Name = "input_file_label";
            this.input_file_label.Size = new System.Drawing.Size(111, 20);
            this.input_file_label.TabIndex = 432;
            this.input_file_label.Text = "Input file";
            this.input_file_label.DoubleClick += new System.EventHandler(this.input_file_Click);
            this.input_file_label.MouseEnter += new System.EventHandler(this.input_file_MouseEnter);
            this.input_file_label.MouseLeave += new System.EventHandler(this.input_file_MouseLeave);
            // 
            // input_file2_label
            // 
            this.input_file2_label.AutoSize = true;
            this.input_file2_label.BackColor = System.Drawing.Color.Transparent;
            this.input_file2_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.input_file2_label.ForeColor = System.Drawing.SystemColors.Control;
            this.input_file2_label.Location = new System.Drawing.Point(150, 941);
            this.input_file2_label.Margin = new System.Windows.Forms.Padding(0);
            this.input_file2_label.Name = "input_file2_label";
            this.input_file2_label.Size = new System.Drawing.Size(137, 20);
            this.input_file2_label.TabIndex = 434;
            this.input_file2_label.Text = "Input file 2";
            this.input_file2_label.DoubleClick += new System.EventHandler(this.input_file2_Click);
            this.input_file2_label.MouseEnter += new System.EventHandler(this.input_file2_MouseEnter);
            this.input_file2_label.MouseLeave += new System.EventHandler(this.input_file2_MouseLeave);
            // 
            // input_file2_txt
            // 
            this.input_file2_txt.BackColor = System.Drawing.Color.Black;
            this.input_file2_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.input_file2_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.input_file2_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.input_file2_txt.Location = new System.Drawing.Point(154, 972);
            this.input_file2_txt.Margin = new System.Windows.Forms.Padding(0);
            this.input_file2_txt.Name = "input_file2_txt";
            this.input_file2_txt.Size = new System.Drawing.Size(128, 21);
            this.input_file2_txt.TabIndex = 1;
            this.input_file2_txt.TextChanged += new System.EventHandler(this.input_file2_TextChanged);
            this.input_file2_txt.DoubleClick += new System.EventHandler(this.input_file2_Click);
            this.input_file2_txt.MouseEnter += new System.EventHandler(this.input_file2_MouseEnter);
            this.input_file2_txt.MouseLeave += new System.EventHandler(this.input_file2_MouseLeave);
            // 
            // mipmaps_label
            // 
            this.mipmaps_label.AutoSize = true;
            this.mipmaps_label.BackColor = System.Drawing.Color.Transparent;
            this.mipmaps_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mipmaps_label.ForeColor = System.Drawing.SystemColors.Control;
            this.mipmaps_label.Location = new System.Drawing.Point(499, 941);
            this.mipmaps_label.Margin = new System.Windows.Forms.Padding(0);
            this.mipmaps_label.Name = "mipmaps_label";
            this.mipmaps_label.Size = new System.Drawing.Size(109, 20);
            this.mipmaps_label.TabIndex = 436;
            this.mipmaps_label.Text = "mipmaps";
            this.mipmaps_label.MouseEnter += new System.EventHandler(this.mipmaps_MouseEnter);
            this.mipmaps_label.MouseLeave += new System.EventHandler(this.mipmaps_MouseLeave);
            // 
            // mipmaps_txt
            // 
            this.mipmaps_txt.BackColor = System.Drawing.Color.Black;
            this.mipmaps_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mipmaps_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mipmaps_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.mipmaps_txt.Location = new System.Drawing.Point(503, 972);
            this.mipmaps_txt.Margin = new System.Windows.Forms.Padding(0);
            this.mipmaps_txt.Name = "mipmaps_txt";
            this.mipmaps_txt.Size = new System.Drawing.Size(100, 21);
            this.mipmaps_txt.TabIndex = 3;
            this.mipmaps_txt.Text = "0";
            this.mipmaps_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mipmaps_txt.TextChanged += new System.EventHandler(this.mipmaps_TextChanged);
            this.mipmaps_txt.MouseEnter += new System.EventHandler(this.mipmaps_MouseEnter);
            this.mipmaps_txt.MouseLeave += new System.EventHandler(this.mipmaps_MouseLeave);
            // 
            // diversity_label
            // 
            this.diversity_label.AutoSize = true;
            this.diversity_label.BackColor = System.Drawing.Color.Transparent;
            this.diversity_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.diversity_label.ForeColor = System.Drawing.SystemColors.Control;
            this.diversity_label.Location = new System.Drawing.Point(498, 877);
            this.diversity_label.Margin = new System.Windows.Forms.Padding(0);
            this.diversity_label.Name = "diversity_label";
            this.diversity_label.Size = new System.Drawing.Size(106, 20);
            this.diversity_label.TabIndex = 438;
            this.diversity_label.Text = "diversity";
            this.diversity_label.MouseEnter += new System.EventHandler(this.diversity_MouseEnter);
            this.diversity_label.MouseLeave += new System.EventHandler(this.diversity_MouseLeave);
            // 
            // diversity_txt
            // 
            this.diversity_txt.BackColor = System.Drawing.Color.Black;
            this.diversity_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.diversity_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.diversity_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.diversity_txt.Location = new System.Drawing.Point(502, 908);
            this.diversity_txt.Margin = new System.Windows.Forms.Padding(0);
            this.diversity_txt.Name = "diversity_txt";
            this.diversity_txt.Size = new System.Drawing.Size(100, 21);
            this.diversity_txt.TabIndex = 7;
            this.diversity_txt.Text = "10";
            this.diversity_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.diversity_txt.TextChanged += new System.EventHandler(this.diversity_TextChanged);
            this.diversity_txt.MouseEnter += new System.EventHandler(this.diversity_MouseEnter);
            this.diversity_txt.MouseLeave += new System.EventHandler(this.diversity_MouseLeave);
            // 
            // diversity2_label
            // 
            this.diversity2_label.AutoSize = true;
            this.diversity2_label.BackColor = System.Drawing.Color.Transparent;
            this.diversity2_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.diversity2_label.ForeColor = System.Drawing.SystemColors.Control;
            this.diversity2_label.Location = new System.Drawing.Point(620, 877);
            this.diversity2_label.Margin = new System.Windows.Forms.Padding(0);
            this.diversity2_label.Name = "diversity2_label";
            this.diversity2_label.Size = new System.Drawing.Size(122, 20);
            this.diversity2_label.TabIndex = 440;
            this.diversity2_label.Text = "diversity2";
            this.diversity2_label.MouseEnter += new System.EventHandler(this.diversity2_MouseEnter);
            this.diversity2_label.MouseLeave += new System.EventHandler(this.diversity2_MouseLeave);
            // 
            // diversity2_txt
            // 
            this.diversity2_txt.BackColor = System.Drawing.Color.Black;
            this.diversity2_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.diversity2_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.diversity2_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.diversity2_txt.Location = new System.Drawing.Point(624, 908);
            this.diversity2_txt.Margin = new System.Windows.Forms.Padding(0);
            this.diversity2_txt.Name = "diversity2_txt";
            this.diversity2_txt.Size = new System.Drawing.Size(114, 21);
            this.diversity2_txt.TabIndex = 8;
            this.diversity2_txt.Text = "0";
            this.diversity2_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.diversity2_txt.TextChanged += new System.EventHandler(this.diversity2_TextChanged);
            this.diversity2_txt.MouseEnter += new System.EventHandler(this.diversity2_MouseEnter);
            this.diversity2_txt.MouseLeave += new System.EventHandler(this.diversity2_MouseLeave);
            // 
            // percentage_label
            // 
            this.percentage_label.AutoSize = true;
            this.percentage_label.BackColor = System.Drawing.Color.Transparent;
            this.percentage_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.percentage_label.ForeColor = System.Drawing.SystemColors.Control;
            this.percentage_label.Location = new System.Drawing.Point(753, 877);
            this.percentage_label.Margin = new System.Windows.Forms.Padding(0);
            this.percentage_label.Name = "percentage_label";
            this.percentage_label.Size = new System.Drawing.Size(141, 20);
            this.percentage_label.TabIndex = 442;
            this.percentage_label.Text = "percentage";
            this.percentage_label.MouseEnter += new System.EventHandler(this.percentage_MouseEnter);
            this.percentage_label.MouseLeave += new System.EventHandler(this.percentage_MouseLeave);
            // 
            // percentage_txt
            // 
            this.percentage_txt.BackColor = System.Drawing.Color.Black;
            this.percentage_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.percentage_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.percentage_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.percentage_txt.Location = new System.Drawing.Point(757, 908);
            this.percentage_txt.Margin = new System.Windows.Forms.Padding(0);
            this.percentage_txt.Name = "percentage_txt";
            this.percentage_txt.Size = new System.Drawing.Size(128, 21);
            this.percentage_txt.TabIndex = 9;
            this.percentage_txt.Text = "0%";
            this.percentage_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.percentage_txt.TextChanged += new System.EventHandler(this.percentage_TextChanged);
            this.percentage_txt.MouseEnter += new System.EventHandler(this.percentage_MouseEnter);
            this.percentage_txt.MouseLeave += new System.EventHandler(this.percentage_MouseLeave);
            // 
            // percentage2_label
            // 
            this.percentage2_label.AutoSize = true;
            this.percentage2_label.BackColor = System.Drawing.Color.Transparent;
            this.percentage2_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.percentage2_label.ForeColor = System.Drawing.SystemColors.Control;
            this.percentage2_label.Location = new System.Drawing.Point(900, 877);
            this.percentage2_label.Margin = new System.Windows.Forms.Padding(0);
            this.percentage2_label.Name = "percentage2_label";
            this.percentage2_label.Size = new System.Drawing.Size(157, 20);
            this.percentage2_label.TabIndex = 444;
            this.percentage2_label.Text = "percentage2";
            this.percentage2_label.MouseEnter += new System.EventHandler(this.percentage2_MouseEnter);
            this.percentage2_label.MouseLeave += new System.EventHandler(this.percentage2_MouseLeave);
            // 
            // percentage2_txt
            // 
            this.percentage2_txt.BackColor = System.Drawing.Color.Black;
            this.percentage2_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.percentage2_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.percentage2_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.percentage2_txt.Location = new System.Drawing.Point(904, 908);
            this.percentage2_txt.Margin = new System.Windows.Forms.Padding(0);
            this.percentage2_txt.Name = "percentage2_txt";
            this.percentage2_txt.Size = new System.Drawing.Size(143, 21);
            this.percentage2_txt.TabIndex = 10;
            this.percentage2_txt.Text = "0%";
            this.percentage2_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.percentage2_txt.TextChanged += new System.EventHandler(this.percentage2_TextChanged);
            this.percentage2_txt.MouseEnter += new System.EventHandler(this.percentage2_MouseEnter);
            this.percentage2_txt.MouseLeave += new System.EventHandler(this.percentage2_MouseLeave);
            // 
            // cmpr_max_label
            // 
            this.cmpr_max_label.AutoSize = true;
            this.cmpr_max_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_max_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cmpr_max_label.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_max_label.Location = new System.Drawing.Point(617, 941);
            this.cmpr_max_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_max_label.Name = "cmpr_max_label";
            this.cmpr_max_label.Size = new System.Drawing.Size(136, 20);
            this.cmpr_max_label.TabIndex = 446;
            this.cmpr_max_label.Text = "CMPR Max";
            this.cmpr_max_label.MouseEnter += new System.EventHandler(this.cmpr_max_MouseEnter);
            this.cmpr_max_label.MouseLeave += new System.EventHandler(this.cmpr_max_MouseLeave);
            // 
            // cmpr_max_txt
            // 
            this.cmpr_max_txt.BackColor = System.Drawing.Color.Black;
            this.cmpr_max_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cmpr_max_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmpr_max_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_max_txt.Location = new System.Drawing.Point(623, 972);
            this.cmpr_max_txt.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_max_txt.Name = "cmpr_max_txt";
            this.cmpr_max_txt.Size = new System.Drawing.Size(125, 21);
            this.cmpr_max_txt.TabIndex = 4;
            this.cmpr_max_txt.Text = "16";
            this.cmpr_max_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmpr_max_txt.TextChanged += new System.EventHandler(this.cmpr_max_TextChanged);
            this.cmpr_max_txt.MouseEnter += new System.EventHandler(this.cmpr_max_MouseEnter);
            this.cmpr_max_txt.MouseLeave += new System.EventHandler(this.cmpr_max_MouseLeave);
            // 
            // output_name_label
            // 
            this.output_name_label.AutoSize = true;
            this.output_name_label.BackColor = System.Drawing.Color.Transparent;
            this.output_name_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.output_name_label.ForeColor = System.Drawing.SystemColors.Control;
            this.output_name_label.Location = new System.Drawing.Point(308, 941);
            this.output_name_label.Margin = new System.Windows.Forms.Padding(0);
            this.output_name_label.Name = "output_name_label";
            this.output_name_label.Size = new System.Drawing.Size(161, 20);
            this.output_name_label.TabIndex = 448;
            this.output_name_label.Text = "Output name";
            this.output_name_label.MouseEnter += new System.EventHandler(this.output_name_MouseEnter);
            this.output_name_label.MouseLeave += new System.EventHandler(this.output_name_MouseLeave);
            // 
            // output_name_txt
            // 
            this.output_name_txt.BackColor = System.Drawing.Color.Black;
            this.output_name_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.output_name_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.output_name_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.output_name_txt.Location = new System.Drawing.Point(303, 972);
            this.output_name_txt.Margin = new System.Windows.Forms.Padding(0);
            this.output_name_txt.Name = "output_name_txt";
            this.output_name_txt.Size = new System.Drawing.Size(177, 21);
            this.output_name_txt.TabIndex = 2;
            this.output_name_txt.TextChanged += new System.EventHandler(this.output_name_TextChanged);
            this.output_name_txt.MouseEnter += new System.EventHandler(this.output_name_MouseEnter);
            this.output_name_txt.MouseLeave += new System.EventHandler(this.output_name_MouseLeave);
            // 
            // cmpr_min_alpha_label
            // 
            this.cmpr_min_alpha_label.AutoSize = true;
            this.cmpr_min_alpha_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_min_alpha_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cmpr_min_alpha_label.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_min_alpha_label.Location = new System.Drawing.Point(762, 941);
            this.cmpr_min_alpha_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_min_alpha_label.Name = "cmpr_min_alpha_label";
            this.cmpr_min_alpha_label.Size = new System.Drawing.Size(200, 20);
            this.cmpr_min_alpha_label.TabIndex = 450;
            this.cmpr_min_alpha_label.Text = "CMPR Min alpha";
            this.cmpr_min_alpha_label.MouseEnter += new System.EventHandler(this.cmpr_min_alpha_MouseEnter);
            this.cmpr_min_alpha_label.MouseLeave += new System.EventHandler(this.cmpr_min_alpha_MouseLeave);
            // 
            // cmpr_min_alpha_txt
            // 
            this.cmpr_min_alpha_txt.BackColor = System.Drawing.Color.Black;
            this.cmpr_min_alpha_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cmpr_min_alpha_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmpr_min_alpha_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_min_alpha_txt.Location = new System.Drawing.Point(766, 972);
            this.cmpr_min_alpha_txt.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_min_alpha_txt.Name = "cmpr_min_alpha_txt";
            this.cmpr_min_alpha_txt.Size = new System.Drawing.Size(192, 21);
            this.cmpr_min_alpha_txt.TabIndex = 5;
            this.cmpr_min_alpha_txt.Text = "100";
            this.cmpr_min_alpha_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmpr_min_alpha_txt.TextChanged += new System.EventHandler(this.cmpr_min_alpha_TextChanged);
            this.cmpr_min_alpha_txt.MouseEnter += new System.EventHandler(this.cmpr_min_alpha_MouseEnter);
            this.cmpr_min_alpha_txt.MouseLeave += new System.EventHandler(this.cmpr_min_alpha_MouseLeave);
            // 
            // num_colours_label
            // 
            this.num_colours_label.AutoSize = true;
            this.num_colours_label.BackColor = System.Drawing.Color.Transparent;
            this.num_colours_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.num_colours_label.ForeColor = System.Drawing.SystemColors.Control;
            this.num_colours_label.Location = new System.Drawing.Point(975, 941);
            this.num_colours_label.Margin = new System.Windows.Forms.Padding(0);
            this.num_colours_label.Name = "num_colours_label";
            this.num_colours_label.Size = new System.Drawing.Size(152, 20);
            this.num_colours_label.TabIndex = 452;
            this.num_colours_label.Text = "num colours";
            this.num_colours_label.MouseEnter += new System.EventHandler(this.num_colours_MouseEnter);
            this.num_colours_label.MouseLeave += new System.EventHandler(this.num_colours_MouseLeave);
            // 
            // num_colours_txt
            // 
            this.num_colours_txt.BackColor = System.Drawing.Color.Black;
            this.num_colours_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.num_colours_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.num_colours_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.num_colours_txt.Location = new System.Drawing.Point(979, 972);
            this.num_colours_txt.Margin = new System.Windows.Forms.Padding(0);
            this.num_colours_txt.Name = "num_colours_txt";
            this.num_colours_txt.Size = new System.Drawing.Size(141, 21);
            this.num_colours_txt.TabIndex = 6;
            this.num_colours_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_colours_txt.TextChanged += new System.EventHandler(this.num_colours_TextChanged);
            this.num_colours_txt.MouseEnter += new System.EventHandler(this.num_colours_MouseEnter);
            this.num_colours_txt.MouseLeave += new System.EventHandler(this.num_colours_MouseLeave);
            // 
            // round3_label
            // 
            this.round3_label.AutoSize = true;
            this.round3_label.BackColor = System.Drawing.Color.Transparent;
            this.round3_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.round3_label.ForeColor = System.Drawing.SystemColors.Control;
            this.round3_label.Location = new System.Drawing.Point(1157, 941);
            this.round3_label.Margin = new System.Windows.Forms.Padding(0);
            this.round3_label.Name = "round3_label";
            this.round3_label.Size = new System.Drawing.Size(91, 20);
            this.round3_label.TabIndex = 454;
            this.round3_label.Text = "round3";
            this.round3_label.MouseEnter += new System.EventHandler(this.round3_MouseEnter);
            this.round3_label.MouseLeave += new System.EventHandler(this.round3_MouseLeave);
            // 
            // round3_txt
            // 
            this.round3_txt.BackColor = System.Drawing.Color.Black;
            this.round3_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.round3_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.round3_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.round3_txt.Location = new System.Drawing.Point(1153, 972);
            this.round3_txt.Margin = new System.Windows.Forms.Padding(0);
            this.round3_txt.Name = "round3_txt";
            this.round3_txt.Size = new System.Drawing.Size(100, 21);
            this.round3_txt.TabIndex = 11;
            this.round3_txt.Text = "15";
            this.round3_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.round3_txt.TextChanged += new System.EventHandler(this.round3_TextChanged);
            this.round3_txt.MouseEnter += new System.EventHandler(this.round3_MouseEnter);
            this.round3_txt.MouseLeave += new System.EventHandler(this.round3_MouseLeave);
            // 
            // round4_label
            // 
            this.round4_label.AutoSize = true;
            this.round4_label.BackColor = System.Drawing.Color.Transparent;
            this.round4_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.round4_label.ForeColor = System.Drawing.SystemColors.Control;
            this.round4_label.Location = new System.Drawing.Point(1277, 941);
            this.round4_label.Margin = new System.Windows.Forms.Padding(0);
            this.round4_label.Name = "round4_label";
            this.round4_label.Size = new System.Drawing.Size(91, 20);
            this.round4_label.TabIndex = 456;
            this.round4_label.Text = "round4";
            this.round4_label.MouseEnter += new System.EventHandler(this.round4_MouseEnter);
            this.round4_label.MouseLeave += new System.EventHandler(this.round4_MouseLeave);
            // 
            // round4_txt
            // 
            this.round4_txt.BackColor = System.Drawing.Color.Black;
            this.round4_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.round4_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.round4_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.round4_txt.Location = new System.Drawing.Point(1273, 972);
            this.round4_txt.Margin = new System.Windows.Forms.Padding(0);
            this.round4_txt.Name = "round4_txt";
            this.round4_txt.Size = new System.Drawing.Size(100, 21);
            this.round4_txt.TabIndex = 12;
            this.round4_txt.Text = "7";
            this.round4_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.round4_txt.TextChanged += new System.EventHandler(this.round4_TextChanged);
            this.round4_txt.MouseEnter += new System.EventHandler(this.round4_MouseEnter);
            this.round4_txt.MouseLeave += new System.EventHandler(this.round4_MouseLeave);
            // 
            // round5_label
            // 
            this.round5_label.AutoSize = true;
            this.round5_label.BackColor = System.Drawing.Color.Transparent;
            this.round5_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.round5_label.ForeColor = System.Drawing.SystemColors.Control;
            this.round5_label.Location = new System.Drawing.Point(1395, 941);
            this.round5_label.Margin = new System.Windows.Forms.Padding(0);
            this.round5_label.Name = "round5_label";
            this.round5_label.Size = new System.Drawing.Size(91, 20);
            this.round5_label.TabIndex = 458;
            this.round5_label.Text = "round5";
            this.round5_label.MouseEnter += new System.EventHandler(this.round5_MouseEnter);
            this.round5_label.MouseLeave += new System.EventHandler(this.round5_MouseLeave);
            // 
            // round5_txt
            // 
            this.round5_txt.BackColor = System.Drawing.Color.Black;
            this.round5_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.round5_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.round5_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.round5_txt.Location = new System.Drawing.Point(1391, 972);
            this.round5_txt.Margin = new System.Windows.Forms.Padding(0);
            this.round5_txt.Name = "round5_txt";
            this.round5_txt.Size = new System.Drawing.Size(100, 21);
            this.round5_txt.TabIndex = 13;
            this.round5_txt.Text = "3";
            this.round5_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.round5_txt.TextChanged += new System.EventHandler(this.round5_TextChanged);
            this.round5_txt.MouseEnter += new System.EventHandler(this.round5_MouseEnter);
            this.round5_txt.MouseLeave += new System.EventHandler(this.round5_MouseLeave);
            // 
            // round6_label
            // 
            this.round6_label.AutoSize = true;
            this.round6_label.BackColor = System.Drawing.Color.Transparent;
            this.round6_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.round6_label.ForeColor = System.Drawing.SystemColors.Control;
            this.round6_label.Location = new System.Drawing.Point(1508, 941);
            this.round6_label.Margin = new System.Windows.Forms.Padding(0);
            this.round6_label.Name = "round6_label";
            this.round6_label.Size = new System.Drawing.Size(91, 20);
            this.round6_label.TabIndex = 460;
            this.round6_label.Text = "round6";
            this.round6_label.MouseEnter += new System.EventHandler(this.round6_MouseEnter);
            this.round6_label.MouseLeave += new System.EventHandler(this.round6_MouseLeave);
            // 
            // round6_txt
            // 
            this.round6_txt.BackColor = System.Drawing.Color.Black;
            this.round6_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.round6_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.round6_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.round6_txt.Location = new System.Drawing.Point(1504, 972);
            this.round6_txt.Margin = new System.Windows.Forms.Padding(0);
            this.round6_txt.Name = "round6_txt";
            this.round6_txt.Size = new System.Drawing.Size(100, 21);
            this.round6_txt.TabIndex = 14;
            this.round6_txt.Text = "1";
            this.round6_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.round6_txt.TextChanged += new System.EventHandler(this.round6_TextChanged);
            this.round6_txt.MouseEnter += new System.EventHandler(this.round6_MouseEnter);
            this.round6_txt.MouseLeave += new System.EventHandler(this.round6_MouseLeave);
            // 
            // custom_a_label
            // 
            this.custom_a_label.AutoSize = true;
            this.custom_a_label.BackColor = System.Drawing.Color.Transparent;
            this.custom_a_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.custom_a_label.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_a_label.Location = new System.Drawing.Point(1557, 877);
            this.custom_a_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_a_label.Name = "custom_a_label";
            this.custom_a_label.Size = new System.Drawing.Size(26, 20);
            this.custom_a_label.TabIndex = 468;
            this.custom_a_label.Text = "A";
            this.custom_a_label.MouseEnter += new System.EventHandler(this.custom_a_MouseEnter);
            this.custom_a_label.MouseLeave += new System.EventHandler(this.custom_a_MouseLeave);
            // 
            // custom_a_txt
            // 
            this.custom_a_txt.BackColor = System.Drawing.Color.Black;
            this.custom_a_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.custom_a_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.custom_a_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.custom_a_txt.Location = new System.Drawing.Point(1538, 908);
            this.custom_a_txt.Margin = new System.Windows.Forms.Padding(0);
            this.custom_a_txt.Name = "custom_a_txt";
            this.custom_a_txt.Size = new System.Drawing.Size(64, 21);
            this.custom_a_txt.TabIndex = 18;
            this.custom_a_txt.Text = "1.0";
            this.custom_a_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.custom_a_txt.TextChanged += new System.EventHandler(this.custom_a_TextChanged);
            this.custom_a_txt.MouseEnter += new System.EventHandler(this.custom_a_MouseEnter);
            this.custom_a_txt.MouseLeave += new System.EventHandler(this.custom_a_MouseLeave);
            // 
            // custom_b_label
            // 
            this.custom_b_label.AutoSize = true;
            this.custom_b_label.BackColor = System.Drawing.Color.Transparent;
            this.custom_b_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.custom_b_label.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_b_label.Location = new System.Drawing.Point(1479, 877);
            this.custom_b_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_b_label.Name = "custom_b_label";
            this.custom_b_label.Size = new System.Drawing.Size(25, 20);
            this.custom_b_label.TabIndex = 466;
            this.custom_b_label.Text = "B";
            this.custom_b_label.MouseEnter += new System.EventHandler(this.custom_b_MouseEnter);
            this.custom_b_label.MouseLeave += new System.EventHandler(this.custom_b_MouseLeave);
            // 
            // custom_b_txt
            // 
            this.custom_b_txt.BackColor = System.Drawing.Color.Black;
            this.custom_b_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.custom_b_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.custom_b_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.custom_b_txt.Location = new System.Drawing.Point(1457, 908);
            this.custom_b_txt.Margin = new System.Windows.Forms.Padding(0);
            this.custom_b_txt.Name = "custom_b_txt";
            this.custom_b_txt.Size = new System.Drawing.Size(64, 21);
            this.custom_b_txt.TabIndex = 17;
            this.custom_b_txt.Text = "1.0";
            this.custom_b_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.custom_b_txt.TextChanged += new System.EventHandler(this.custom_b_TextChanged);
            this.custom_b_txt.MouseEnter += new System.EventHandler(this.custom_b_MouseEnter);
            this.custom_b_txt.MouseLeave += new System.EventHandler(this.custom_b_MouseLeave);
            // 
            // custom_g_label
            // 
            this.custom_g_label.AutoSize = true;
            this.custom_g_label.BackColor = System.Drawing.Color.Transparent;
            this.custom_g_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.custom_g_label.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_g_label.Location = new System.Drawing.Point(1394, 877);
            this.custom_g_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_g_label.Name = "custom_g_label";
            this.custom_g_label.Size = new System.Drawing.Size(26, 20);
            this.custom_g_label.TabIndex = 464;
            this.custom_g_label.Text = "G";
            this.custom_g_label.MouseEnter += new System.EventHandler(this.custom_g_MouseEnter);
            this.custom_g_label.MouseLeave += new System.EventHandler(this.custom_g_MouseLeave);
            // 
            // custom_g_txt
            // 
            this.custom_g_txt.BackColor = System.Drawing.Color.Black;
            this.custom_g_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.custom_g_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.custom_g_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.custom_g_txt.Location = new System.Drawing.Point(1373, 908);
            this.custom_g_txt.Margin = new System.Windows.Forms.Padding(0);
            this.custom_g_txt.Name = "custom_g_txt";
            this.custom_g_txt.Size = new System.Drawing.Size(64, 21);
            this.custom_g_txt.TabIndex = 16;
            this.custom_g_txt.Text = "1.0";
            this.custom_g_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.custom_g_txt.TextChanged += new System.EventHandler(this.custom_g_TextChanged);
            this.custom_g_txt.MouseEnter += new System.EventHandler(this.custom_g_MouseEnter);
            this.custom_g_txt.MouseLeave += new System.EventHandler(this.custom_g_MouseLeave);
            // 
            // custom_r_label
            // 
            this.custom_r_label.AutoSize = true;
            this.custom_r_label.BackColor = System.Drawing.Color.Transparent;
            this.custom_r_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.custom_r_label.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_r_label.Location = new System.Drawing.Point(1312, 877);
            this.custom_r_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_r_label.Name = "custom_r_label";
            this.custom_r_label.Size = new System.Drawing.Size(26, 20);
            this.custom_r_label.TabIndex = 462;
            this.custom_r_label.Text = "R";
            this.custom_r_label.MouseEnter += new System.EventHandler(this.custom_r_MouseEnter);
            this.custom_r_label.MouseLeave += new System.EventHandler(this.custom_r_MouseLeave);
            // 
            // custom_r_txt
            // 
            this.custom_r_txt.BackColor = System.Drawing.Color.Black;
            this.custom_r_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.custom_r_txt.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.custom_r_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.custom_r_txt.Location = new System.Drawing.Point(1292, 908);
            this.custom_r_txt.Margin = new System.Windows.Forms.Padding(0);
            this.custom_r_txt.Name = "custom_r_txt";
            this.custom_r_txt.Size = new System.Drawing.Size(64, 21);
            this.custom_r_txt.TabIndex = 15;
            this.custom_r_txt.Text = "1.0";
            this.custom_r_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.custom_r_txt.TextChanged += new System.EventHandler(this.custom_r_TextChanged);
            this.custom_r_txt.MouseEnter += new System.EventHandler(this.custom_r_MouseEnter);
            this.custom_r_txt.MouseLeave += new System.EventHandler(this.custom_r_MouseLeave);
            // 
            // custom_rgba_label
            // 
            this.custom_rgba_label.AutoSize = true;
            this.custom_rgba_label.BackColor = System.Drawing.Color.Transparent;
            this.custom_rgba_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.custom_rgba_label.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_rgba_label.Location = new System.Drawing.Point(1095, 897);
            this.custom_rgba_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_rgba_label.Name = "custom_rgba_label";
            this.custom_rgba_label.Size = new System.Drawing.Size(174, 20);
            this.custom_rgba_label.TabIndex = 469;
            this.custom_rgba_label.Text = "Custom RGBA";
            // 
            // description_title
            // 
            this.description_title.AutoSize = true;
            this.description_title.BackColor = System.Drawing.Color.Transparent;
            this.description_title.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.description_title.ForeColor = System.Drawing.Color.Cyan;
            this.description_title.Location = new System.Drawing.Point(862, 512);
            this.description_title.Margin = new System.Windows.Forms.Padding(0);
            this.description_title.Name = "description_title";
            this.description_title.Padding = new System.Windows.Forms.Padding(0, 22, 0, 0);
            this.description_title.Size = new System.Drawing.Size(139, 42);
            this.description_title.TabIndex = 470;
            this.description_title.Text = "Description";
            // 
            // description
            // 
            this.description.AutoSize = true;
            this.description.BackColor = System.Drawing.Color.Transparent;
            this.description.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.description.ForeColor = System.Drawing.Color.Cyan;
            this.description.Location = new System.Drawing.Point(700, 576);
            this.description.Margin = new System.Windows.Forms.Padding(0);
            this.description.MaximumSize = new System.Drawing.Size(480, 280);
            this.description.MinimumSize = new System.Drawing.Size(480, 280);
            this.description.Name = "description";
            this.description.Padding = new System.Windows.Forms.Padding(0, 22, 0, 0);
            this.description.Size = new System.Drawing.Size(480, 280);
            this.description.TabIndex = 471;
            this.description.Text = "Point to something with your mouse!";
            this.description.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // description_surrounding
            // 
            this.description_surrounding.BackColor = System.Drawing.Color.Transparent;
            this.description_surrounding.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.description_surrounding.Enabled = false;
            this.description_surrounding.ErrorImage = null;
            this.description_surrounding.InitialImage = null;
            this.description_surrounding.Location = new System.Drawing.Point(681, 546);
            this.description_surrounding.Margin = new System.Windows.Forms.Padding(0);
            this.description_surrounding.Name = "description_surrounding";
            this.description_surrounding.Size = new System.Drawing.Size(512, 320);
            this.description_surrounding.TabIndex = 472;
            this.description_surrounding.TabStop = false;
            // 
            // palette_rgb5a3_ck
            // 
            this.palette_rgb5a3_ck.BackColor = System.Drawing.Color.Transparent;
            this.palette_rgb5a3_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.palette_rgb5a3_ck.Enabled = false;
            this.palette_rgb5a3_ck.ErrorImage = null;
            this.palette_rgb5a3_ck.InitialImage = null;
            this.palette_rgb5a3_ck.Location = new System.Drawing.Point(503, 800);
            this.palette_rgb5a3_ck.Margin = new System.Windows.Forms.Padding(0);
            this.palette_rgb5a3_ck.Name = "palette_rgb5a3_ck";
            this.palette_rgb5a3_ck.Size = new System.Drawing.Size(64, 64);
            this.palette_rgb5a3_ck.TabIndex = 482;
            this.palette_rgb5a3_ck.TabStop = false;
            // 
            // palette_rgb5a3_label
            // 
            this.palette_rgb5a3_label.AutoSize = true;
            this.palette_rgb5a3_label.BackColor = System.Drawing.Color.Transparent;
            this.palette_rgb5a3_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.palette_rgb5a3_label.ForeColor = System.Drawing.SystemColors.Window;
            this.palette_rgb5a3_label.Location = new System.Drawing.Point(571, 800);
            this.palette_rgb5a3_label.Margin = new System.Windows.Forms.Padding(0);
            this.palette_rgb5a3_label.Name = "palette_rgb5a3_label";
            this.palette_rgb5a3_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.palette_rgb5a3_label.Size = new System.Drawing.Size(108, 64);
            this.palette_rgb5a3_label.TabIndex = 480;
            this.palette_rgb5a3_label.Text = "RGB5A3";
            this.palette_rgb5a3_label.Click += new System.EventHandler(this.palette_RGB5A3_Click);
            this.palette_rgb5a3_label.MouseEnter += new System.EventHandler(this.palette_RGB5A3_MouseEnter);
            this.palette_rgb5a3_label.MouseLeave += new System.EventHandler(this.palette_RGB5A3_MouseLeave);
            // 
            // palette_rgb5a3_hitbox
            // 
            this.palette_rgb5a3_hitbox.AutoSize = true;
            this.palette_rgb5a3_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.palette_rgb5a3_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.palette_rgb5a3_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.palette_rgb5a3_hitbox.Location = new System.Drawing.Point(500, 800);
            this.palette_rgb5a3_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.palette_rgb5a3_hitbox.Name = "palette_rgb5a3_hitbox";
            this.palette_rgb5a3_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.palette_rgb5a3_hitbox.Size = new System.Drawing.Size(190, 64);
            this.palette_rgb5a3_hitbox.TabIndex = 481;
            this.palette_rgb5a3_hitbox.Click += new System.EventHandler(this.palette_RGB5A3_Click);
            this.palette_rgb5a3_hitbox.MouseEnter += new System.EventHandler(this.palette_RGB5A3_MouseEnter);
            this.palette_rgb5a3_hitbox.MouseLeave += new System.EventHandler(this.palette_RGB5A3_MouseLeave);
            // 
            // palette_rgb565_ck
            // 
            this.palette_rgb565_ck.BackColor = System.Drawing.Color.Transparent;
            this.palette_rgb565_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.palette_rgb565_ck.Enabled = false;
            this.palette_rgb565_ck.ErrorImage = null;
            this.palette_rgb565_ck.InitialImage = null;
            this.palette_rgb565_ck.Location = new System.Drawing.Point(503, 736);
            this.palette_rgb565_ck.Margin = new System.Windows.Forms.Padding(0);
            this.palette_rgb565_ck.Name = "palette_rgb565_ck";
            this.palette_rgb565_ck.Size = new System.Drawing.Size(64, 64);
            this.palette_rgb565_ck.TabIndex = 479;
            this.palette_rgb565_ck.TabStop = false;
            // 
            // palette_rgb565_label
            // 
            this.palette_rgb565_label.AutoSize = true;
            this.palette_rgb565_label.BackColor = System.Drawing.Color.Transparent;
            this.palette_rgb565_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.palette_rgb565_label.ForeColor = System.Drawing.SystemColors.Window;
            this.palette_rgb565_label.Location = new System.Drawing.Point(571, 736);
            this.palette_rgb565_label.Margin = new System.Windows.Forms.Padding(0);
            this.palette_rgb565_label.Name = "palette_rgb565_label";
            this.palette_rgb565_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.palette_rgb565_label.Size = new System.Drawing.Size(107, 64);
            this.palette_rgb565_label.TabIndex = 477;
            this.palette_rgb565_label.Text = "RGB565";
            this.palette_rgb565_label.Click += new System.EventHandler(this.palette_RGB565_Click);
            this.palette_rgb565_label.MouseEnter += new System.EventHandler(this.palette_RGB565_MouseEnter);
            this.palette_rgb565_label.MouseLeave += new System.EventHandler(this.palette_RGB565_MouseLeave);
            // 
            // palette_rgb565_hitbox
            // 
            this.palette_rgb565_hitbox.AutoSize = true;
            this.palette_rgb565_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.palette_rgb565_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.palette_rgb565_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.palette_rgb565_hitbox.Location = new System.Drawing.Point(500, 736);
            this.palette_rgb565_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.palette_rgb565_hitbox.Name = "palette_rgb565_hitbox";
            this.palette_rgb565_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.palette_rgb565_hitbox.Size = new System.Drawing.Size(190, 64);
            this.palette_rgb565_hitbox.TabIndex = 478;
            this.palette_rgb565_hitbox.Click += new System.EventHandler(this.palette_RGB565_Click);
            this.palette_rgb565_hitbox.MouseEnter += new System.EventHandler(this.palette_RGB565_MouseEnter);
            this.palette_rgb565_hitbox.MouseLeave += new System.EventHandler(this.palette_RGB565_MouseLeave);
            // 
            // palette_ai8_ck
            // 
            this.palette_ai8_ck.BackColor = System.Drawing.Color.Transparent;
            this.palette_ai8_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.palette_ai8_ck.Enabled = false;
            this.palette_ai8_ck.ErrorImage = null;
            this.palette_ai8_ck.InitialImage = null;
            this.palette_ai8_ck.Location = new System.Drawing.Point(503, 672);
            this.palette_ai8_ck.Margin = new System.Windows.Forms.Padding(0);
            this.palette_ai8_ck.Name = "palette_ai8_ck";
            this.palette_ai8_ck.Size = new System.Drawing.Size(64, 64);
            this.palette_ai8_ck.TabIndex = 476;
            this.palette_ai8_ck.TabStop = false;
            // 
            // palette_ai8_label
            // 
            this.palette_ai8_label.AutoSize = true;
            this.palette_ai8_label.BackColor = System.Drawing.Color.Transparent;
            this.palette_ai8_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.palette_ai8_label.ForeColor = System.Drawing.SystemColors.Window;
            this.palette_ai8_label.Location = new System.Drawing.Point(571, 672);
            this.palette_ai8_label.Margin = new System.Windows.Forms.Padding(0);
            this.palette_ai8_label.Name = "palette_ai8_label";
            this.palette_ai8_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.palette_ai8_label.Size = new System.Drawing.Size(48, 64);
            this.palette_ai8_label.TabIndex = 474;
            this.palette_ai8_label.Text = "AI8";
            this.palette_ai8_label.Click += new System.EventHandler(this.palette_AI8_Click);
            this.palette_ai8_label.MouseEnter += new System.EventHandler(this.palette_AI8_MouseEnter);
            this.palette_ai8_label.MouseLeave += new System.EventHandler(this.palette_AI8_MouseLeave);
            // 
            // palette_ai8_hitbox
            // 
            this.palette_ai8_hitbox.AutoSize = true;
            this.palette_ai8_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.palette_ai8_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.palette_ai8_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.palette_ai8_hitbox.Location = new System.Drawing.Point(500, 672);
            this.palette_ai8_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.palette_ai8_hitbox.Name = "palette_ai8_hitbox";
            this.palette_ai8_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.palette_ai8_hitbox.Size = new System.Drawing.Size(190, 64);
            this.palette_ai8_hitbox.TabIndex = 475;
            this.palette_ai8_hitbox.Click += new System.EventHandler(this.palette_AI8_Click);
            this.palette_ai8_hitbox.MouseEnter += new System.EventHandler(this.palette_AI8_MouseEnter);
            this.palette_ai8_hitbox.MouseLeave += new System.EventHandler(this.palette_AI8_MouseLeave);
            // 
            // palette_label
            // 
            this.palette_label.AutoSize = true;
            this.palette_label.BackColor = System.Drawing.Color.Transparent;
            this.palette_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.palette_label.ForeColor = System.Drawing.SystemColors.Control;
            this.palette_label.Location = new System.Drawing.Point(498, 642);
            this.palette_label.Margin = new System.Windows.Forms.Padding(0);
            this.palette_label.Name = "palette_label";
            this.palette_label.Size = new System.Drawing.Size(186, 20);
            this.palette_label.TabIndex = 473;
            this.palette_label.Text = "Palette Format";
            // 
            // github_ck
            // 
            this.github_ck.BackColor = System.Drawing.Color.Transparent;
            this.github_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.github_ck.Enabled = false;
            this.github_ck.ErrorImage = null;
            this.github_ck.InitialImage = null;
            this.github_ck.Location = new System.Drawing.Point(1711, 0);
            this.github_ck.Margin = new System.Windows.Forms.Padding(0);
            this.github_ck.Name = "github_ck";
            this.github_ck.Size = new System.Drawing.Size(32, 32);
            this.github_ck.TabIndex = 484;
            this.github_ck.TabStop = false;
            // 
            // github_hitbox
            // 
            this.github_hitbox.AutoSize = true;
            this.github_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.github_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.github_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.github_hitbox.Location = new System.Drawing.Point(1711, 0);
            this.github_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.github_hitbox.Name = "github_hitbox";
            this.github_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.github_hitbox.Size = new System.Drawing.Size(32, 32);
            this.github_hitbox.TabIndex = 483;
            this.github_hitbox.Click += new System.EventHandler(this.github_Click);
            this.github_hitbox.MouseEnter += new System.EventHandler(this.github_MouseEnter);
            this.github_hitbox.MouseLeave += new System.EventHandler(this.github_MouseLeave);
            // 
            // youtube_ck
            // 
            this.youtube_ck.BackColor = System.Drawing.Color.Transparent;
            this.youtube_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.youtube_ck.Enabled = false;
            this.youtube_ck.ErrorImage = null;
            this.youtube_ck.InitialImage = null;
            this.youtube_ck.Location = new System.Drawing.Point(1765, 0);
            this.youtube_ck.Margin = new System.Windows.Forms.Padding(0);
            this.youtube_ck.Name = "youtube_ck";
            this.youtube_ck.Size = new System.Drawing.Size(32, 32);
            this.youtube_ck.TabIndex = 486;
            this.youtube_ck.TabStop = false;
            // 
            // youtube_hitbox
            // 
            this.youtube_hitbox.AutoSize = true;
            this.youtube_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.youtube_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.youtube_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.youtube_hitbox.Location = new System.Drawing.Point(1765, 0);
            this.youtube_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.youtube_hitbox.Name = "youtube_hitbox";
            this.youtube_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.youtube_hitbox.Size = new System.Drawing.Size(32, 32);
            this.youtube_hitbox.TabIndex = 485;
            this.youtube_hitbox.Click += new System.EventHandler(this.youtube_Click);
            this.youtube_hitbox.MouseEnter += new System.EventHandler(this.youtube_MouseEnter);
            this.youtube_hitbox.MouseLeave += new System.EventHandler(this.youtube_MouseLeave);
            // 
            // discord_ck
            // 
            this.discord_ck.BackColor = System.Drawing.Color.Transparent;
            this.discord_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.discord_ck.Enabled = false;
            this.discord_ck.ErrorImage = null;
            this.discord_ck.InitialImage = null;
            this.discord_ck.Location = new System.Drawing.Point(1661, 0);
            this.discord_ck.Margin = new System.Windows.Forms.Padding(0);
            this.discord_ck.Name = "discord_ck";
            this.discord_ck.Size = new System.Drawing.Size(32, 32);
            this.discord_ck.TabIndex = 488;
            this.discord_ck.TabStop = false;
            // 
            // discord_hitbox
            // 
            this.discord_hitbox.AutoSize = true;
            this.discord_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.discord_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.discord_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.discord_hitbox.Location = new System.Drawing.Point(1661, 0);
            this.discord_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.discord_hitbox.Name = "discord_hitbox";
            this.discord_hitbox.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.discord_hitbox.Size = new System.Drawing.Size(32, 32);
            this.discord_hitbox.TabIndex = 487;
            this.discord_hitbox.Click += new System.EventHandler(this.discord_Click);
            this.discord_hitbox.MouseEnter += new System.EventHandler(this.discord_MouseEnter);
            this.discord_hitbox.MouseLeave += new System.EventHandler(this.discord_MouseLeave);
            // 
            // input_file_hitbox
            // 
            this.input_file_hitbox.AutoSize = true;
            this.input_file_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.input_file_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.input_file_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.input_file_hitbox.Location = new System.Drawing.Point(12, 929);
            this.input_file_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.input_file_hitbox.Name = "input_file_hitbox";
            this.input_file_hitbox.Padding = new System.Windows.Forms.Padding(122, 44, 0, 0);
            this.input_file_hitbox.Size = new System.Drawing.Size(122, 64);
            this.input_file_hitbox.TabIndex = 489;
            this.input_file_hitbox.DoubleClick += new System.EventHandler(this.input_file_Click);
            this.input_file_hitbox.MouseEnter += new System.EventHandler(this.input_file_MouseEnter);
            this.input_file_hitbox.MouseLeave += new System.EventHandler(this.input_file_MouseLeave);
            // 
            // input_file2_hitbox
            // 
            this.input_file2_hitbox.AutoSize = true;
            this.input_file2_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.input_file2_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.input_file2_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.input_file2_hitbox.Location = new System.Drawing.Point(150, 929);
            this.input_file2_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.input_file2_hitbox.Name = "input_file2_hitbox";
            this.input_file2_hitbox.Padding = new System.Windows.Forms.Padding(137, 44, 0, 0);
            this.input_file2_hitbox.Size = new System.Drawing.Size(137, 64);
            this.input_file2_hitbox.TabIndex = 490;
            this.input_file2_hitbox.DoubleClick += new System.EventHandler(this.input_file2_Click);
            this.input_file2_hitbox.MouseEnter += new System.EventHandler(this.input_file2_MouseEnter);
            this.input_file2_hitbox.MouseLeave += new System.EventHandler(this.input_file2_MouseLeave);
            // 
            // custom_r_hitbox
            // 
            this.custom_r_hitbox.AutoSize = true;
            this.custom_r_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.custom_r_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.custom_r_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_r_hitbox.Location = new System.Drawing.Point(1289, 877);
            this.custom_r_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.custom_r_hitbox.Name = "custom_r_hitbox";
            this.custom_r_hitbox.Padding = new System.Windows.Forms.Padding(70, 34, 0, 0);
            this.custom_r_hitbox.Size = new System.Drawing.Size(70, 54);
            this.custom_r_hitbox.TabIndex = 493;
            this.custom_r_hitbox.MouseEnter += new System.EventHandler(this.custom_r_MouseEnter);
            this.custom_r_hitbox.MouseLeave += new System.EventHandler(this.custom_r_MouseLeave);
            // 
            // num_colours_hitbox
            // 
            this.num_colours_hitbox.AutoSize = true;
            this.num_colours_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.num_colours_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.num_colours_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.num_colours_hitbox.Location = new System.Drawing.Point(975, 941);
            this.num_colours_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.num_colours_hitbox.Name = "num_colours_hitbox";
            this.num_colours_hitbox.Padding = new System.Windows.Forms.Padding(150, 34, 0, 0);
            this.num_colours_hitbox.Size = new System.Drawing.Size(150, 54);
            this.num_colours_hitbox.TabIndex = 497;
            this.num_colours_hitbox.MouseEnter += new System.EventHandler(this.num_colours_MouseEnter);
            this.num_colours_hitbox.MouseLeave += new System.EventHandler(this.num_colours_MouseLeave);
            // 
            // diversity_hitbox
            // 
            this.diversity_hitbox.AutoSize = true;
            this.diversity_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.diversity_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.diversity_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.diversity_hitbox.Location = new System.Drawing.Point(498, 878);
            this.diversity_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.diversity_hitbox.Name = "diversity_hitbox";
            this.diversity_hitbox.Padding = new System.Windows.Forms.Padding(107, 34, 0, 0);
            this.diversity_hitbox.Size = new System.Drawing.Size(107, 54);
            this.diversity_hitbox.TabIndex = 498;
            this.diversity_hitbox.MouseEnter += new System.EventHandler(this.diversity_MouseEnter);
            this.diversity_hitbox.MouseLeave += new System.EventHandler(this.diversity_MouseLeave);
            // 
            // round3_hitbox
            // 
            this.round3_hitbox.AutoSize = true;
            this.round3_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.round3_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.round3_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.round3_hitbox.Location = new System.Drawing.Point(1149, 941);
            this.round3_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.round3_hitbox.Name = "round3_hitbox";
            this.round3_hitbox.Padding = new System.Windows.Forms.Padding(107, 34, 0, 0);
            this.round3_hitbox.Size = new System.Drawing.Size(107, 54);
            this.round3_hitbox.TabIndex = 500;
            this.round3_hitbox.MouseEnter += new System.EventHandler(this.round3_MouseEnter);
            this.round3_hitbox.MouseLeave += new System.EventHandler(this.round3_MouseLeave);
            // 
            // cmpr_min_alpha_hitbox
            // 
            this.cmpr_min_alpha_hitbox.AutoSize = true;
            this.cmpr_min_alpha_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_min_alpha_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cmpr_min_alpha_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_min_alpha_hitbox.Location = new System.Drawing.Point(762, 941);
            this.cmpr_min_alpha_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_min_alpha_hitbox.Name = "cmpr_min_alpha_hitbox";
            this.cmpr_min_alpha_hitbox.Padding = new System.Windows.Forms.Padding(200, 34, 0, 0);
            this.cmpr_min_alpha_hitbox.Size = new System.Drawing.Size(200, 54);
            this.cmpr_min_alpha_hitbox.TabIndex = 501;
            this.cmpr_min_alpha_hitbox.MouseEnter += new System.EventHandler(this.cmpr_min_alpha_MouseEnter);
            this.cmpr_min_alpha_hitbox.MouseLeave += new System.EventHandler(this.cmpr_min_alpha_MouseLeave);
            // 
            // cmpr_max_hitbox
            // 
            this.cmpr_max_hitbox.AutoSize = true;
            this.cmpr_max_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_max_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cmpr_max_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_max_hitbox.Location = new System.Drawing.Point(617, 941);
            this.cmpr_max_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_max_hitbox.Name = "cmpr_max_hitbox";
            this.cmpr_max_hitbox.Padding = new System.Windows.Forms.Padding(137, 34, 0, 0);
            this.cmpr_max_hitbox.Size = new System.Drawing.Size(137, 54);
            this.cmpr_max_hitbox.TabIndex = 502;
            this.cmpr_max_hitbox.MouseEnter += new System.EventHandler(this.cmpr_max_MouseEnter);
            this.cmpr_max_hitbox.MouseLeave += new System.EventHandler(this.cmpr_max_MouseLeave);
            // 
            // mipmaps_hitbox
            // 
            this.mipmaps_hitbox.AutoSize = true;
            this.mipmaps_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mipmaps_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mipmaps_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mipmaps_hitbox.Location = new System.Drawing.Point(498, 941);
            this.mipmaps_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mipmaps_hitbox.Name = "mipmaps_hitbox";
            this.mipmaps_hitbox.Padding = new System.Windows.Forms.Padding(107, 34, 0, 0);
            this.mipmaps_hitbox.Size = new System.Drawing.Size(107, 54);
            this.mipmaps_hitbox.TabIndex = 503;
            this.mipmaps_hitbox.MouseEnter += new System.EventHandler(this.mipmaps_MouseEnter);
            this.mipmaps_hitbox.MouseLeave += new System.EventHandler(this.mipmaps_MouseLeave);
            // 
            // output_name_hitbox
            // 
            this.output_name_hitbox.AutoSize = true;
            this.output_name_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.output_name_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.output_name_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.output_name_hitbox.Location = new System.Drawing.Point(299, 929);
            this.output_name_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.output_name_hitbox.Name = "output_name_hitbox";
            this.output_name_hitbox.Padding = new System.Windows.Forms.Padding(185, 44, 0, 0);
            this.output_name_hitbox.Size = new System.Drawing.Size(185, 64);
            this.output_name_hitbox.TabIndex = 504;
            this.output_name_hitbox.MouseEnter += new System.EventHandler(this.output_name_MouseEnter);
            this.output_name_hitbox.MouseLeave += new System.EventHandler(this.output_name_MouseLeave);
            // 
            // round4_hitbox
            // 
            this.round4_hitbox.AutoSize = true;
            this.round4_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.round4_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.round4_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.round4_hitbox.Location = new System.Drawing.Point(1270, 941);
            this.round4_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.round4_hitbox.Name = "round4_hitbox";
            this.round4_hitbox.Padding = new System.Windows.Forms.Padding(107, 34, 0, 0);
            this.round4_hitbox.Size = new System.Drawing.Size(107, 54);
            this.round4_hitbox.TabIndex = 505;
            this.round4_hitbox.MouseEnter += new System.EventHandler(this.round4_MouseEnter);
            this.round4_hitbox.MouseLeave += new System.EventHandler(this.round4_MouseLeave);
            // 
            // round5_hitbox
            // 
            this.round5_hitbox.AutoSize = true;
            this.round5_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.round5_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.round5_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.round5_hitbox.Location = new System.Drawing.Point(1387, 941);
            this.round5_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.round5_hitbox.Name = "round5_hitbox";
            this.round5_hitbox.Padding = new System.Windows.Forms.Padding(107, 34, 0, 0);
            this.round5_hitbox.Size = new System.Drawing.Size(107, 54);
            this.round5_hitbox.TabIndex = 506;
            this.round5_hitbox.MouseEnter += new System.EventHandler(this.round5_MouseEnter);
            this.round5_hitbox.MouseLeave += new System.EventHandler(this.round5_MouseLeave);
            // 
            // round6_hitbox
            // 
            this.round6_hitbox.AutoSize = true;
            this.round6_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.round6_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.round6_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.round6_hitbox.Location = new System.Drawing.Point(1500, 941);
            this.round6_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.round6_hitbox.Name = "round6_hitbox";
            this.round6_hitbox.Padding = new System.Windows.Forms.Padding(107, 34, 0, 0);
            this.round6_hitbox.Size = new System.Drawing.Size(107, 54);
            this.round6_hitbox.TabIndex = 507;
            this.round6_hitbox.MouseEnter += new System.EventHandler(this.round6_MouseEnter);
            this.round6_hitbox.MouseLeave += new System.EventHandler(this.round6_MouseLeave);
            // 
            // diversity2_hitbox
            // 
            this.diversity2_hitbox.AutoSize = true;
            this.diversity2_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.diversity2_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.diversity2_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.diversity2_hitbox.Location = new System.Drawing.Point(619, 878);
            this.diversity2_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.diversity2_hitbox.Name = "diversity2_hitbox";
            this.diversity2_hitbox.Padding = new System.Windows.Forms.Padding(124, 34, 0, 0);
            this.diversity2_hitbox.Size = new System.Drawing.Size(124, 54);
            this.diversity2_hitbox.TabIndex = 508;
            this.diversity2_hitbox.MouseEnter += new System.EventHandler(this.diversity2_MouseEnter);
            this.diversity2_hitbox.MouseLeave += new System.EventHandler(this.diversity2_MouseLeave);
            // 
            // percentage_hitbox
            // 
            this.percentage_hitbox.AutoSize = true;
            this.percentage_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.percentage_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.percentage_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.percentage_hitbox.Location = new System.Drawing.Point(753, 878);
            this.percentage_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.percentage_hitbox.Name = "percentage_hitbox";
            this.percentage_hitbox.Padding = new System.Windows.Forms.Padding(138, 34, 0, 0);
            this.percentage_hitbox.Size = new System.Drawing.Size(138, 54);
            this.percentage_hitbox.TabIndex = 509;
            this.percentage_hitbox.MouseEnter += new System.EventHandler(this.percentage_MouseEnter);
            this.percentage_hitbox.MouseLeave += new System.EventHandler(this.percentage_MouseLeave);
            // 
            // percentage2_hitbox
            // 
            this.percentage2_hitbox.AutoSize = true;
            this.percentage2_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.percentage2_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.percentage2_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.percentage2_hitbox.Location = new System.Drawing.Point(900, 878);
            this.percentage2_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.percentage2_hitbox.Name = "percentage2_hitbox";
            this.percentage2_hitbox.Padding = new System.Windows.Forms.Padding(150, 34, 0, 0);
            this.percentage2_hitbox.Size = new System.Drawing.Size(150, 54);
            this.percentage2_hitbox.TabIndex = 510;
            this.percentage2_hitbox.MouseEnter += new System.EventHandler(this.percentage2_MouseEnter);
            this.percentage2_hitbox.MouseLeave += new System.EventHandler(this.percentage2_MouseLeave);
            // 
            // custom_g_hitbox
            // 
            this.custom_g_hitbox.AutoSize = true;
            this.custom_g_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.custom_g_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.custom_g_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_g_hitbox.Location = new System.Drawing.Point(1369, 877);
            this.custom_g_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.custom_g_hitbox.Name = "custom_g_hitbox";
            this.custom_g_hitbox.Padding = new System.Windows.Forms.Padding(70, 34, 0, 0);
            this.custom_g_hitbox.Size = new System.Drawing.Size(70, 54);
            this.custom_g_hitbox.TabIndex = 511;
            this.custom_g_hitbox.MouseEnter += new System.EventHandler(this.custom_g_MouseEnter);
            this.custom_g_hitbox.MouseLeave += new System.EventHandler(this.custom_g_MouseLeave);
            // 
            // custom_b_hitbox
            // 
            this.custom_b_hitbox.AutoSize = true;
            this.custom_b_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.custom_b_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.custom_b_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_b_hitbox.Location = new System.Drawing.Point(1453, 878);
            this.custom_b_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.custom_b_hitbox.Name = "custom_b_hitbox";
            this.custom_b_hitbox.Padding = new System.Windows.Forms.Padding(70, 34, 0, 0);
            this.custom_b_hitbox.Size = new System.Drawing.Size(70, 54);
            this.custom_b_hitbox.TabIndex = 512;
            this.custom_b_hitbox.MouseEnter += new System.EventHandler(this.custom_b_MouseEnter);
            this.custom_b_hitbox.MouseLeave += new System.EventHandler(this.custom_b_MouseLeave);
            // 
            // custom_a_hitbox
            // 
            this.custom_a_hitbox.AutoSize = true;
            this.custom_a_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.custom_a_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.custom_a_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_a_hitbox.Location = new System.Drawing.Point(1537, 878);
            this.custom_a_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.custom_a_hitbox.Name = "custom_a_hitbox";
            this.custom_a_hitbox.Padding = new System.Windows.Forms.Padding(70, 34, 0, 0);
            this.custom_a_hitbox.Size = new System.Drawing.Size(70, 54);
            this.custom_a_hitbox.TabIndex = 513;
            this.custom_a_hitbox.MouseEnter += new System.EventHandler(this.custom_a_MouseEnter);
            this.custom_a_hitbox.MouseLeave += new System.EventHandler(this.custom_a_MouseLeave);
            // 
            // desc2
            // 
            this.desc2.AutoSize = true;
            this.desc2.BackColor = System.Drawing.Color.Transparent;
            this.desc2.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.desc2.ForeColor = System.Drawing.Color.Cyan;
            this.desc2.Location = new System.Drawing.Point(700, 628);
            this.desc2.Margin = new System.Windows.Forms.Padding(0);
            this.desc2.MaximumSize = new System.Drawing.Size(480, 230);
            this.desc2.MinimumSize = new System.Drawing.Size(480, 230);
            this.desc2.Name = "desc2";
            this.desc2.Size = new System.Drawing.Size(480, 230);
            this.desc2.TabIndex = 533;
            this.desc2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.desc2.Visible = false;
            // 
            // desc3
            // 
            this.desc3.AutoSize = true;
            this.desc3.BackColor = System.Drawing.Color.Transparent;
            this.desc3.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.desc3.ForeColor = System.Drawing.Color.Cyan;
            this.desc3.Location = new System.Drawing.Point(700, 657);
            this.desc3.Margin = new System.Windows.Forms.Padding(0);
            this.desc3.MaximumSize = new System.Drawing.Size(480, 200);
            this.desc3.MinimumSize = new System.Drawing.Size(480, 200);
            this.desc3.Name = "desc3";
            this.desc3.Size = new System.Drawing.Size(480, 200);
            this.desc3.TabIndex = 534;
            this.desc3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.desc3.Visible = false;
            // 
            // desc4
            // 
            this.desc4.AutoSize = true;
            this.desc4.BackColor = System.Drawing.Color.Transparent;
            this.desc4.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.desc4.ForeColor = System.Drawing.Color.Cyan;
            this.desc4.Location = new System.Drawing.Point(700, 687);
            this.desc4.Margin = new System.Windows.Forms.Padding(0);
            this.desc4.MaximumSize = new System.Drawing.Size(480, 170);
            this.desc4.MinimumSize = new System.Drawing.Size(480, 170);
            this.desc4.Name = "desc4";
            this.desc4.Size = new System.Drawing.Size(480, 170);
            this.desc4.TabIndex = 535;
            this.desc4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // desc5
            // 
            this.desc5.AutoSize = true;
            this.desc5.BackColor = System.Drawing.Color.Transparent;
            this.desc5.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.desc5.ForeColor = System.Drawing.Color.Cyan;
            this.desc5.Location = new System.Drawing.Point(700, 717);
            this.desc5.Margin = new System.Windows.Forms.Padding(0);
            this.desc5.MaximumSize = new System.Drawing.Size(480, 140);
            this.desc5.MinimumSize = new System.Drawing.Size(480, 140);
            this.desc5.Name = "desc5";
            this.desc5.Size = new System.Drawing.Size(480, 140);
            this.desc5.TabIndex = 536;
            this.desc5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // desc6
            // 
            this.desc6.AutoSize = true;
            this.desc6.BackColor = System.Drawing.Color.Transparent;
            this.desc6.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.desc6.ForeColor = System.Drawing.Color.Cyan;
            this.desc6.Location = new System.Drawing.Point(700, 747);
            this.desc6.Margin = new System.Windows.Forms.Padding(0);
            this.desc6.MaximumSize = new System.Drawing.Size(480, 110);
            this.desc6.MinimumSize = new System.Drawing.Size(480, 110);
            this.desc6.Name = "desc6";
            this.desc6.Size = new System.Drawing.Size(480, 110);
            this.desc6.TabIndex = 537;
            this.desc6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // desc7
            // 
            this.desc7.AutoSize = true;
            this.desc7.BackColor = System.Drawing.Color.Transparent;
            this.desc7.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.desc7.ForeColor = System.Drawing.Color.Cyan;
            this.desc7.Location = new System.Drawing.Point(700, 778);
            this.desc7.Margin = new System.Windows.Forms.Padding(0);
            this.desc7.MaximumSize = new System.Drawing.Size(480, 80);
            this.desc7.MinimumSize = new System.Drawing.Size(480, 80);
            this.desc7.Name = "desc7";
            this.desc7.Size = new System.Drawing.Size(480, 80);
            this.desc7.TabIndex = 538;
            this.desc7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // desc8
            // 
            this.desc8.AutoSize = true;
            this.desc8.BackColor = System.Drawing.Color.Transparent;
            this.desc8.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.desc8.ForeColor = System.Drawing.Color.Cyan;
            this.desc8.Location = new System.Drawing.Point(700, 806);
            this.desc8.Margin = new System.Windows.Forms.Padding(0);
            this.desc8.MaximumSize = new System.Drawing.Size(480, 50);
            this.desc8.MinimumSize = new System.Drawing.Size(480, 50);
            this.desc8.Name = "desc8";
            this.desc8.Size = new System.Drawing.Size(480, 50);
            this.desc8.TabIndex = 539;
            this.desc8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // desc9
            // 
            this.desc9.AutoSize = true;
            this.desc9.BackColor = System.Drawing.Color.Transparent;
            this.desc9.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.desc9.ForeColor = System.Drawing.Color.Cyan;
            this.desc9.Location = new System.Drawing.Point(700, 834);
            this.desc9.Margin = new System.Windows.Forms.Padding(0);
            this.desc9.MaximumSize = new System.Drawing.Size(480, 25);
            this.desc9.MinimumSize = new System.Drawing.Size(480, 25);
            this.desc9.Name = "desc9";
            this.desc9.Size = new System.Drawing.Size(480, 25);
            this.desc9.TabIndex = 540;
            this.desc9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // version_hitbox
            // 
            this.version_hitbox.AutoSize = true;
            this.version_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.version_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.version_hitbox.ForeColor = System.Drawing.Color.Gray;
            this.version_hitbox.Location = new System.Drawing.Point(757, 0);
            this.version_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.version_hitbox.Name = "version_hitbox";
            this.version_hitbox.Padding = new System.Windows.Forms.Padding(64, 6, 0, 6);
            this.version_hitbox.Size = new System.Drawing.Size(64, 32);
            this.version_hitbox.TabIndex = 541;
            this.version_hitbox.MouseEnter += new System.EventHandler(this.version_MouseEnter);
            this.version_hitbox.MouseLeave += new System.EventHandler(this.version_MouseLeave);
            // 
            // output_label
            // 
            this.output_label.AutoSize = true;
            this.output_label.BackColor = System.Drawing.Color.Transparent;
            this.output_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.output_label.ForeColor = System.Drawing.Color.Cyan;
            this.output_label.Location = new System.Drawing.Point(1207, 770);
            this.output_label.Margin = new System.Windows.Forms.Padding(0);
            this.output_label.MaximumSize = new System.Drawing.Size(420, 100);
            this.output_label.MinimumSize = new System.Drawing.Size(420, 100);
            this.output_label.Name = "output_label";
            this.output_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 0);
            this.output_label.Size = new System.Drawing.Size(420, 100);
            this.output_label.TabIndex = 542;
            this.output_label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.output_label.MouseEnter += new System.EventHandler(this.Output_label_MouseEnter);
            this.output_label.MouseLeave += new System.EventHandler(this.Output_label_MouseLeave);
            // 
            // version_ck
            // 
            this.version_ck.BackColor = System.Drawing.Color.Transparent;
            this.version_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.version_ck.Enabled = false;
            this.version_ck.ErrorImage = null;
            this.version_ck.InitialImage = null;
            this.version_ck.Location = new System.Drawing.Point(757, 0);
            this.version_ck.Margin = new System.Windows.Forms.Padding(0);
            this.version_ck.Name = "version_ck";
            this.version_ck.Size = new System.Drawing.Size(64, 32);
            this.version_ck.TabIndex = 543;
            this.version_ck.TabStop = false;
            // 
            // cli_textbox_label
            // 
            this.cli_textbox_label.AutoSize = true;
            this.cli_textbox_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(20)))), ((int)(((byte)(0)))), ((int)(((byte)(49)))));
            this.cli_textbox_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cli_textbox_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cli_textbox_label.Location = new System.Drawing.Point(71, 1005);
            this.cli_textbox_label.Margin = new System.Windows.Forms.Padding(0);
            this.cli_textbox_label.MaximumSize = new System.Drawing.Size(1400, 128);
            this.cli_textbox_label.Name = "cli_textbox_label";
            this.cli_textbox_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cli_textbox_label.Size = new System.Drawing.Size(0, 64);
            this.cli_textbox_label.TabIndex = 544;
            // 
            // view_rgba_ck
            // 
            this.view_rgba_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_rgba_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_rgba_ck.Enabled = false;
            this.view_rgba_ck.ErrorImage = null;
            this.view_rgba_ck.InitialImage = null;
            this.view_rgba_ck.Location = new System.Drawing.Point(240, 1293);
            this.view_rgba_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_rgba_ck.Name = "view_rgba_ck";
            this.view_rgba_ck.Size = new System.Drawing.Size(64, 64);
            this.view_rgba_ck.TabIndex = 547;
            this.view_rgba_ck.TabStop = false;
            this.view_rgba_ck.Visible = false;
            // 
            // view_rgba_label
            // 
            this.view_rgba_label.AutoSize = true;
            this.view_rgba_label.BackColor = System.Drawing.Color.Transparent;
            this.view_rgba_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_rgba_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_rgba_label.Location = new System.Drawing.Point(308, 1293);
            this.view_rgba_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_rgba_label.Name = "view_rgba_label";
            this.view_rgba_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_rgba_label.Size = new System.Drawing.Size(174, 64);
            this.view_rgba_label.TabIndex = 545;
            this.view_rgba_label.Text = "Custom RGBA";
            this.view_rgba_label.Visible = false;
            this.view_rgba_label.Click += new System.EventHandler(this.view_rgba_Click);
            this.view_rgba_label.MouseEnter += new System.EventHandler(this.view_rgba_MouseEnter);
            this.view_rgba_label.MouseLeave += new System.EventHandler(this.view_rgba_MouseLeave);
            // 
            // view_rgba_hitbox
            // 
            this.view_rgba_hitbox.AutoSize = true;
            this.view_rgba_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.view_rgba_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_rgba_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.view_rgba_hitbox.Location = new System.Drawing.Point(237, 1293);
            this.view_rgba_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.view_rgba_hitbox.Name = "view_rgba_hitbox";
            this.view_rgba_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.view_rgba_hitbox.Size = new System.Drawing.Size(190, 64);
            this.view_rgba_hitbox.TabIndex = 546;
            this.view_rgba_hitbox.Visible = false;
            this.view_rgba_hitbox.Click += new System.EventHandler(this.view_rgba_Click);
            this.view_rgba_hitbox.MouseEnter += new System.EventHandler(this.view_rgba_MouseEnter);
            this.view_rgba_hitbox.MouseLeave += new System.EventHandler(this.view_rgba_MouseLeave);
            // 
            // view_palette_ck
            // 
            this.view_palette_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_palette_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_palette_ck.Enabled = false;
            this.view_palette_ck.ErrorImage = null;
            this.view_palette_ck.InitialImage = null;
            this.view_palette_ck.Location = new System.Drawing.Point(239, 1357);
            this.view_palette_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_palette_ck.Name = "view_palette_ck";
            this.view_palette_ck.Size = new System.Drawing.Size(64, 64);
            this.view_palette_ck.TabIndex = 550;
            this.view_palette_ck.TabStop = false;
            this.view_palette_ck.Visible = false;
            // 
            // view_palette_label
            // 
            this.view_palette_label.AutoSize = true;
            this.view_palette_label.BackColor = System.Drawing.Color.Transparent;
            this.view_palette_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_palette_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_palette_label.Location = new System.Drawing.Point(307, 1357);
            this.view_palette_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_palette_label.Name = "view_palette_label";
            this.view_palette_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_palette_label.Size = new System.Drawing.Size(93, 64);
            this.view_palette_label.TabIndex = 548;
            this.view_palette_label.Text = "Palette";
            this.view_palette_label.Visible = false;
            this.view_palette_label.Click += new System.EventHandler(this.view_palette_Click);
            this.view_palette_label.MouseEnter += new System.EventHandler(this.view_palette_MouseEnter);
            this.view_palette_label.MouseLeave += new System.EventHandler(this.view_palette_MouseLeave);
            // 
            // view_palette_hitbox
            // 
            this.view_palette_hitbox.AutoSize = true;
            this.view_palette_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.view_palette_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_palette_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.view_palette_hitbox.Location = new System.Drawing.Point(236, 1357);
            this.view_palette_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.view_palette_hitbox.Name = "view_palette_hitbox";
            this.view_palette_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.view_palette_hitbox.Size = new System.Drawing.Size(190, 64);
            this.view_palette_hitbox.TabIndex = 549;
            this.view_palette_hitbox.Visible = false;
            this.view_palette_hitbox.Click += new System.EventHandler(this.view_palette_Click);
            this.view_palette_hitbox.MouseEnter += new System.EventHandler(this.view_palette_MouseEnter);
            this.view_palette_hitbox.MouseLeave += new System.EventHandler(this.view_palette_MouseLeave);
            // 
            // view_cmpr_ck
            // 
            this.view_cmpr_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_cmpr_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_cmpr_ck.Enabled = false;
            this.view_cmpr_ck.ErrorImage = null;
            this.view_cmpr_ck.InitialImage = null;
            this.view_cmpr_ck.Location = new System.Drawing.Point(41, 1293);
            this.view_cmpr_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_cmpr_ck.Name = "view_cmpr_ck";
            this.view_cmpr_ck.Size = new System.Drawing.Size(64, 64);
            this.view_cmpr_ck.TabIndex = 583;
            this.view_cmpr_ck.TabStop = false;
            // 
            // view_cmpr_label
            // 
            this.view_cmpr_label.AutoSize = true;
            this.view_cmpr_label.BackColor = System.Drawing.Color.Transparent;
            this.view_cmpr_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_cmpr_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_cmpr_label.Location = new System.Drawing.Point(109, 1294);
            this.view_cmpr_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_cmpr_label.Name = "view_cmpr_label";
            this.view_cmpr_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_cmpr_label.Size = new System.Drawing.Size(79, 64);
            this.view_cmpr_label.TabIndex = 581;
            this.view_cmpr_label.Text = "CMPR";
            this.view_cmpr_label.Click += new System.EventHandler(this.view_cmpr_Click);
            this.view_cmpr_label.MouseEnter += new System.EventHandler(this.view_cmpr_MouseEnter);
            this.view_cmpr_label.MouseLeave += new System.EventHandler(this.view_cmpr_MouseLeave);
            // 
            // view_cmpr_hitbox
            // 
            this.view_cmpr_hitbox.AutoSize = true;
            this.view_cmpr_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.view_cmpr_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_cmpr_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.view_cmpr_hitbox.Location = new System.Drawing.Point(38, 1293);
            this.view_cmpr_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.view_cmpr_hitbox.Name = "view_cmpr_hitbox";
            this.view_cmpr_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.view_cmpr_hitbox.Size = new System.Drawing.Size(190, 64);
            this.view_cmpr_hitbox.TabIndex = 582;
            this.view_cmpr_hitbox.Click += new System.EventHandler(this.view_cmpr_Click);
            this.view_cmpr_hitbox.MouseEnter += new System.EventHandler(this.view_cmpr_MouseEnter);
            this.view_cmpr_hitbox.MouseLeave += new System.EventHandler(this.view_cmpr_MouseLeave);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox2.Enabled = false;
            this.pictureBox2.ErrorImage = null;
            this.pictureBox2.InitialImage = null;
            this.pictureBox2.Location = new System.Drawing.Point(1170, 1259);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(64, 64);
            this.pictureBox2.TabIndex = 580;
            this.pictureBox2.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label3.ForeColor = System.Drawing.SystemColors.Window;
            this.label3.Location = new System.Drawing.Point(1238, 1259);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.label3.Size = new System.Drawing.Size(93, 64);
            this.label3.TabIndex = 578;
            this.label3.Text = "CI14x2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label4.ForeColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(1167, 1259);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.label4.Size = new System.Drawing.Size(190, 64);
            this.label4.TabIndex = 579;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox3.Enabled = false;
            this.pictureBox3.ErrorImage = null;
            this.pictureBox3.InitialImage = null;
            this.pictureBox3.Location = new System.Drawing.Point(1170, 1195);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(64, 64);
            this.pictureBox3.TabIndex = 577;
            this.pictureBox3.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label5.ForeColor = System.Drawing.SystemColors.Window;
            this.label5.Location = new System.Drawing.Point(1238, 1196);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.label5.Size = new System.Drawing.Size(48, 64);
            this.label5.TabIndex = 575;
            this.label5.Text = "CI8";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(1167, 1195);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.label6.Size = new System.Drawing.Size(190, 64);
            this.label6.TabIndex = 576;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox4.Enabled = false;
            this.pictureBox4.ErrorImage = null;
            this.pictureBox4.InitialImage = null;
            this.pictureBox4.Location = new System.Drawing.Point(1170, 1131);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(64, 64);
            this.pictureBox4.TabIndex = 574;
            this.pictureBox4.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label7.ForeColor = System.Drawing.SystemColors.Window;
            this.label7.Location = new System.Drawing.Point(1238, 1132);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.label7.Size = new System.Drawing.Size(48, 64);
            this.label7.TabIndex = 572;
            this.label7.Text = "CI4";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label8.ForeColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(1167, 1131);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.label8.Size = new System.Drawing.Size(190, 64);
            this.label8.TabIndex = 573;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox5.Enabled = false;
            this.pictureBox5.ErrorImage = null;
            this.pictureBox5.InitialImage = null;
            this.pictureBox5.Location = new System.Drawing.Point(875, 1259);
            this.pictureBox5.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(64, 64);
            this.pictureBox5.TabIndex = 571;
            this.pictureBox5.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label9.ForeColor = System.Drawing.SystemColors.Window;
            this.label9.Location = new System.Drawing.Point(943, 1260);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.label9.Size = new System.Drawing.Size(108, 64);
            this.label9.TabIndex = 569;
            this.label9.Text = "RGBA32";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label10.ForeColor = System.Drawing.SystemColors.Control;
            this.label10.Location = new System.Drawing.Point(872, 1259);
            this.label10.Margin = new System.Windows.Forms.Padding(0);
            this.label10.Name = "label10";
            this.label10.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.label10.Size = new System.Drawing.Size(190, 64);
            this.label10.TabIndex = 570;
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox6.Enabled = false;
            this.pictureBox6.ErrorImage = null;
            this.pictureBox6.InitialImage = null;
            this.pictureBox6.Location = new System.Drawing.Point(875, 1195);
            this.pictureBox6.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(64, 64);
            this.pictureBox6.TabIndex = 568;
            this.pictureBox6.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label11.ForeColor = System.Drawing.SystemColors.Window;
            this.label11.Location = new System.Drawing.Point(943, 1196);
            this.label11.Margin = new System.Windows.Forms.Padding(0);
            this.label11.Name = "label11";
            this.label11.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.label11.Size = new System.Drawing.Size(108, 64);
            this.label11.TabIndex = 566;
            this.label11.Text = "RGB5A3";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label12.ForeColor = System.Drawing.SystemColors.Control;
            this.label12.Location = new System.Drawing.Point(872, 1195);
            this.label12.Margin = new System.Windows.Forms.Padding(0);
            this.label12.Name = "label12";
            this.label12.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.label12.Size = new System.Drawing.Size(190, 64);
            this.label12.TabIndex = 567;
            // 
            // pictureBox7
            // 
            this.pictureBox7.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox7.Enabled = false;
            this.pictureBox7.ErrorImage = null;
            this.pictureBox7.InitialImage = null;
            this.pictureBox7.Location = new System.Drawing.Point(875, 1131);
            this.pictureBox7.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(64, 64);
            this.pictureBox7.TabIndex = 565;
            this.pictureBox7.TabStop = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label13.ForeColor = System.Drawing.SystemColors.Window;
            this.label13.Location = new System.Drawing.Point(943, 1131);
            this.label13.Margin = new System.Windows.Forms.Padding(0);
            this.label13.Name = "label13";
            this.label13.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.label13.Size = new System.Drawing.Size(107, 64);
            this.label13.TabIndex = 563;
            this.label13.Text = "RGB565";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label14.ForeColor = System.Drawing.SystemColors.Control;
            this.label14.Location = new System.Drawing.Point(872, 1131);
            this.label14.Margin = new System.Windows.Forms.Padding(0);
            this.label14.Name = "label14";
            this.label14.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.label14.Size = new System.Drawing.Size(190, 64);
            this.label14.TabIndex = 564;
            // 
            // pictureBox8
            // 
            this.pictureBox8.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox8.Enabled = false;
            this.pictureBox8.ErrorImage = null;
            this.pictureBox8.InitialImage = null;
            this.pictureBox8.Location = new System.Drawing.Point(599, 1323);
            this.pictureBox8.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(64, 64);
            this.pictureBox8.TabIndex = 562;
            this.pictureBox8.TabStop = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.Transparent;
            this.label15.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label15.ForeColor = System.Drawing.SystemColors.Window;
            this.label15.Location = new System.Drawing.Point(667, 1324);
            this.label15.Margin = new System.Windows.Forms.Padding(0);
            this.label15.Name = "label15";
            this.label15.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.label15.Size = new System.Drawing.Size(48, 64);
            this.label15.TabIndex = 560;
            this.label15.Text = "AI8";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.Transparent;
            this.label16.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label16.ForeColor = System.Drawing.SystemColors.Control;
            this.label16.Location = new System.Drawing.Point(596, 1323);
            this.label16.Margin = new System.Windows.Forms.Padding(0);
            this.label16.Name = "label16";
            this.label16.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.label16.Size = new System.Drawing.Size(190, 64);
            this.label16.TabIndex = 561;
            // 
            // pictureBox9
            // 
            this.pictureBox9.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox9.Enabled = false;
            this.pictureBox9.ErrorImage = null;
            this.pictureBox9.InitialImage = null;
            this.pictureBox9.Location = new System.Drawing.Point(599, 1259);
            this.pictureBox9.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(64, 64);
            this.pictureBox9.TabIndex = 559;
            this.pictureBox9.TabStop = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.Transparent;
            this.label17.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label17.ForeColor = System.Drawing.SystemColors.Window;
            this.label17.Location = new System.Drawing.Point(667, 1260);
            this.label17.Margin = new System.Windows.Forms.Padding(0);
            this.label17.Name = "label17";
            this.label17.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.label17.Size = new System.Drawing.Size(48, 64);
            this.label17.TabIndex = 557;
            this.label17.Text = "AI4";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.Color.Transparent;
            this.label18.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label18.ForeColor = System.Drawing.SystemColors.Control;
            this.label18.Location = new System.Drawing.Point(596, 1259);
            this.label18.Margin = new System.Windows.Forms.Padding(0);
            this.label18.Name = "label18";
            this.label18.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.label18.Size = new System.Drawing.Size(190, 64);
            this.label18.TabIndex = 558;
            // 
            // pictureBox10
            // 
            this.pictureBox10.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox10.Enabled = false;
            this.pictureBox10.ErrorImage = null;
            this.pictureBox10.InitialImage = null;
            this.pictureBox10.Location = new System.Drawing.Point(599, 1195);
            this.pictureBox10.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(64, 64);
            this.pictureBox10.TabIndex = 556;
            this.pictureBox10.TabStop = false;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.BackColor = System.Drawing.Color.Transparent;
            this.label19.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label19.ForeColor = System.Drawing.SystemColors.Window;
            this.label19.Location = new System.Drawing.Point(667, 1195);
            this.label19.Margin = new System.Windows.Forms.Padding(0);
            this.label19.Name = "label19";
            this.label19.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.label19.Size = new System.Drawing.Size(31, 64);
            this.label19.TabIndex = 554;
            this.label19.Text = "I8";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label20.ForeColor = System.Drawing.SystemColors.Control;
            this.label20.Location = new System.Drawing.Point(596, 1195);
            this.label20.Margin = new System.Windows.Forms.Padding(0);
            this.label20.Name = "label20";
            this.label20.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.label20.Size = new System.Drawing.Size(190, 64);
            this.label20.TabIndex = 555;
            // 
            // pictureBox11
            // 
            this.pictureBox11.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox11.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox11.Enabled = false;
            this.pictureBox11.ErrorImage = null;
            this.pictureBox11.InitialImage = null;
            this.pictureBox11.Location = new System.Drawing.Point(599, 1131);
            this.pictureBox11.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.Size = new System.Drawing.Size(64, 64);
            this.pictureBox11.TabIndex = 553;
            this.pictureBox11.TabStop = false;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.BackColor = System.Drawing.Color.Transparent;
            this.label21.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label21.ForeColor = System.Drawing.SystemColors.Window;
            this.label21.Location = new System.Drawing.Point(667, 1131);
            this.label21.Margin = new System.Windows.Forms.Padding(0);
            this.label21.Name = "label21";
            this.label21.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.label21.Size = new System.Drawing.Size(31, 64);
            this.label21.TabIndex = 551;
            this.label21.Text = "I4";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.BackColor = System.Drawing.Color.Transparent;
            this.label22.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.label22.ForeColor = System.Drawing.SystemColors.Control;
            this.label22.Location = new System.Drawing.Point(596, 1131);
            this.label22.Margin = new System.Windows.Forms.Padding(0);
            this.label22.Name = "label22";
            this.label22.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.label22.Size = new System.Drawing.Size(190, 64);
            this.label22.TabIndex = 552;
            // 
            // view_options_ck
            // 
            this.view_options_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_options_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_options_ck.Enabled = false;
            this.view_options_ck.ErrorImage = null;
            this.view_options_ck.InitialImage = null;
            this.view_options_ck.Location = new System.Drawing.Point(40, 1357);
            this.view_options_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_options_ck.Name = "view_options_ck";
            this.view_options_ck.Size = new System.Drawing.Size(64, 64);
            this.view_options_ck.TabIndex = 586;
            this.view_options_ck.TabStop = false;
            // 
            // view_options_label
            // 
            this.view_options_label.AutoSize = true;
            this.view_options_label.BackColor = System.Drawing.Color.Transparent;
            this.view_options_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_options_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_options_label.Location = new System.Drawing.Point(108, 1358);
            this.view_options_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_options_label.Name = "view_options_label";
            this.view_options_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_options_label.Size = new System.Drawing.Size(97, 64);
            this.view_options_label.TabIndex = 584;
            this.view_options_label.Text = "Options";
            this.view_options_label.Click += new System.EventHandler(this.view_options_Click);
            this.view_options_label.MouseEnter += new System.EventHandler(this.view_options_MouseEnter);
            this.view_options_label.MouseLeave += new System.EventHandler(this.view_options_MouseLeave);
            // 
            // view_options_hitbox
            // 
            this.view_options_hitbox.AutoSize = true;
            this.view_options_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.view_options_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.view_options_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.view_options_hitbox.Location = new System.Drawing.Point(37, 1357);
            this.view_options_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.view_options_hitbox.Name = "view_options_hitbox";
            this.view_options_hitbox.Padding = new System.Windows.Forms.Padding(190, 44, 0, 0);
            this.view_options_hitbox.Size = new System.Drawing.Size(190, 64);
            this.view_options_hitbox.TabIndex = 585;
            this.view_options_hitbox.Click += new System.EventHandler(this.view_options_Click);
            this.view_options_hitbox.MouseEnter += new System.EventHandler(this.view_options_MouseEnter);
            this.view_options_hitbox.MouseLeave += new System.EventHandler(this.view_options_MouseLeave);
            // 
            // banner_move
            // 
            this.banner_move.AutoSize = true;
            this.banner_move.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(20)))), ((int)(((byte)(0)))), ((int)(((byte)(49)))));
            this.banner_move.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.banner_move.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_move.ForeColor = System.Drawing.Color.Gray;
            this.banner_move.Location = new System.Drawing.Point(840, 0);
            this.banner_move.Margin = new System.Windows.Forms.Padding(0);
            this.banner_move.Name = "banner_move";
            this.banner_move.Padding = new System.Windows.Forms.Padding(800, 6, 0, 6);
            this.banner_move.Size = new System.Drawing.Size(800, 32);
            this.banner_move.TabIndex = 587;
            this.banner_move.MouseDown += new System.Windows.Forms.MouseEventHandler(this.banner_move_MouseDown);
            this.banner_move.MouseEnter += new System.EventHandler(this.banner_move_MouseEnter);
            this.banner_move.MouseLeave += new System.EventHandler(this.banner_move_MouseLeave);
            this.banner_move.MouseMove += new System.Windows.Forms.MouseEventHandler(this.banner_move_MouseMove);
            this.banner_move.MouseUp += new System.Windows.Forms.MouseEventHandler(this.banner_move_MouseUp);
            // 
            // banner_resize
            // 
            this.banner_resize.AutoSize = true;
            this.banner_resize.BackColor = System.Drawing.Color.Transparent;
            this.banner_resize.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.banner_resize.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_resize.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_resize.Location = new System.Drawing.Point(-5, -5);
            this.banner_resize.Margin = new System.Windows.Forms.Padding(0);
            this.banner_resize.Name = "banner_resize";
            this.banner_resize.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.banner_resize.Size = new System.Drawing.Size(32, 32);
            this.banner_resize.TabIndex = 588;
            this.banner_resize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.banner_resize_MouseDown);
            this.banner_resize.MouseEnter += new System.EventHandler(this.banner_resize_MouseEnter);
            this.banner_resize.MouseLeave += new System.EventHandler(this.banner_resize_MouseLeave);
            this.banner_resize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.banner_resize_MouseMove);
            this.banner_resize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.banner_resize_MouseUp);
            // 
            // plt0_gui
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(72)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(2845, 1902);
            this.Controls.Add(this.banner_resize);
            this.Controls.Add(this.banner_move);
            this.Controls.Add(this.view_options_ck);
            this.Controls.Add(this.view_options_label);
            this.Controls.Add(this.view_options_hitbox);
            this.Controls.Add(this.view_cmpr_ck);
            this.Controls.Add(this.view_cmpr_label);
            this.Controls.Add(this.view_cmpr_hitbox);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.pictureBox6);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.pictureBox7);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.pictureBox8);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.pictureBox9);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.pictureBox10);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.pictureBox11);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.view_palette_ck);
            this.Controls.Add(this.view_palette_label);
            this.Controls.Add(this.view_palette_hitbox);
            this.Controls.Add(this.view_rgba_ck);
            this.Controls.Add(this.view_rgba_label);
            this.Controls.Add(this.view_rgba_hitbox);
            this.Controls.Add(this.cli_textbox_label);
            this.Controls.Add(this.version_ck);
            this.Controls.Add(this.output_label);
            this.Controls.Add(this.version_hitbox);
            this.Controls.Add(this.desc9);
            this.Controls.Add(this.desc8);
            this.Controls.Add(this.desc7);
            this.Controls.Add(this.desc6);
            this.Controls.Add(this.desc5);
            this.Controls.Add(this.desc4);
            this.Controls.Add(this.desc3);
            this.Controls.Add(this.discord_ck);
            this.Controls.Add(this.discord_hitbox);
            this.Controls.Add(this.youtube_ck);
            this.Controls.Add(this.youtube_hitbox);
            this.Controls.Add(this.github_ck);
            this.Controls.Add(this.github_hitbox);
            this.Controls.Add(this.palette_rgb5a3_ck);
            this.Controls.Add(this.palette_rgb5a3_label);
            this.Controls.Add(this.palette_rgb5a3_hitbox);
            this.Controls.Add(this.palette_rgb565_ck);
            this.Controls.Add(this.palette_rgb565_label);
            this.Controls.Add(this.palette_rgb565_hitbox);
            this.Controls.Add(this.palette_ai8_ck);
            this.Controls.Add(this.palette_ai8_label);
            this.Controls.Add(this.palette_ai8_hitbox);
            this.Controls.Add(this.palette_label);
            this.Controls.Add(this.description_title);
            this.Controls.Add(this.custom_rgba_label);
            this.Controls.Add(this.custom_a_label);
            this.Controls.Add(this.custom_a_txt);
            this.Controls.Add(this.custom_b_label);
            this.Controls.Add(this.custom_b_txt);
            this.Controls.Add(this.custom_g_label);
            this.Controls.Add(this.custom_g_txt);
            this.Controls.Add(this.custom_r_label);
            this.Controls.Add(this.custom_r_txt);
            this.Controls.Add(this.round6_label);
            this.Controls.Add(this.round6_txt);
            this.Controls.Add(this.round5_label);
            this.Controls.Add(this.round5_txt);
            this.Controls.Add(this.round4_label);
            this.Controls.Add(this.round4_txt);
            this.Controls.Add(this.round3_label);
            this.Controls.Add(this.round3_txt);
            this.Controls.Add(this.num_colours_label);
            this.Controls.Add(this.num_colours_txt);
            this.Controls.Add(this.cmpr_min_alpha_label);
            this.Controls.Add(this.cmpr_min_alpha_txt);
            this.Controls.Add(this.output_name_label);
            this.Controls.Add(this.output_name_txt);
            this.Controls.Add(this.cmpr_max_label);
            this.Controls.Add(this.cmpr_max_txt);
            this.Controls.Add(this.percentage2_label);
            this.Controls.Add(this.percentage2_txt);
            this.Controls.Add(this.percentage_label);
            this.Controls.Add(this.percentage_txt);
            this.Controls.Add(this.diversity2_label);
            this.Controls.Add(this.diversity2_txt);
            this.Controls.Add(this.diversity_label);
            this.Controls.Add(this.diversity_txt);
            this.Controls.Add(this.mipmaps_label);
            this.Controls.Add(this.mipmaps_txt);
            this.Controls.Add(this.input_file2_label);
            this.Controls.Add(this.input_file2_txt);
            this.Controls.Add(this.input_file_label);
            this.Controls.Add(this.input_file_txt);
            this.Controls.Add(this.run_ck);
            this.Controls.Add(this.cli_textbox_ck);
            this.Controls.Add(this.banner_1_ck);
            this.Controls.Add(this.banner_1_hitbox);
            this.Controls.Add(this.banner_2_ck);
            this.Controls.Add(this.banner_2_hitbox);
            this.Controls.Add(this.banner_3_ck);
            this.Controls.Add(this.banner_3_hitbox);
            this.Controls.Add(this.banner_4_ck);
            this.Controls.Add(this.banner_4_hitbox);
            this.Controls.Add(this.banner_6_ck);
            this.Controls.Add(this.banner_6_hitbox);
            this.Controls.Add(this.banner_7_ck);
            this.Controls.Add(this.banner_7_hitbox);
            this.Controls.Add(this.banner_8_ck);
            this.Controls.Add(this.banner_8_hitbox);
            this.Controls.Add(this.banner_9_ck);
            this.Controls.Add(this.banner_9_hitbox);
            this.Controls.Add(this.banner_minus_ck);
            this.Controls.Add(this.banner_minus_hitbox);
            this.Controls.Add(this.banner_5_ck);
            this.Controls.Add(this.banner_5_hitbox);
            this.Controls.Add(this.banner_x_ck);
            this.Controls.Add(this.banner_x_hitbox);
            this.Controls.Add(this.paint_ck);
            this.Controls.Add(this.auto_ck);
            this.Controls.Add(this.preview_ck);
            this.Controls.Add(this.all_ck);
            this.Controls.Add(this.auto_hitbox);
            this.Controls.Add(this.paint_hitbox);
            this.Controls.Add(this.preview_hitbox);
            this.Controls.Add(this.all_hitbox);
            this.Controls.Add(this.banner_ck);
            this.Controls.Add(this.view_mag_ck);
            this.Controls.Add(this.view_mag_label);
            this.Controls.Add(this.view_mag_hitbox);
            this.Controls.Add(this.view_min_ck);
            this.Controls.Add(this.view_WrapT_ck);
            this.Controls.Add(this.view_WrapT_label);
            this.Controls.Add(this.view_WrapT_hitbox);
            this.Controls.Add(this.view_WrapS_ck);
            this.Controls.Add(this.view_WrapS_label);
            this.Controls.Add(this.view_WrapS_hitbox);
            this.Controls.Add(this.view_algorithm_ck);
            this.Controls.Add(this.view_algorithm_label);
            this.Controls.Add(this.view_algorithm_hitbox);
            this.Controls.Add(this.view_alpha_ck);
            this.Controls.Add(this.view_alpha_label);
            this.Controls.Add(this.view_alpha_hitbox);
            this.Controls.Add(this.colour_channels_label);
            this.Controls.Add(this.a_a_ck);
            this.Controls.Add(this.a_b_ck);
            this.Controls.Add(this.b_a_ck);
            this.Controls.Add(this.b_b_ck);
            this.Controls.Add(this.g_a_ck);
            this.Controls.Add(this.g_b_ck);
            this.Controls.Add(this.r_a_ck);
            this.Controls.Add(this.r_b_ck);
            this.Controls.Add(this.a_g_ck);
            this.Controls.Add(this.a_r_ck);
            this.Controls.Add(this.b_g_ck);
            this.Controls.Add(this.b_r_ck);
            this.Controls.Add(this.g_g_ck);
            this.Controls.Add(this.g_r_ck);
            this.Controls.Add(this.r_g_ck);
            this.Controls.Add(this.r_r_ck);
            this.Controls.Add(this.mag_linearmipmaplinear_ck);
            this.Controls.Add(this.mag_linearmipmaplinear_label);
            this.Controls.Add(this.mag_linearmipmaplinear_hitbox);
            this.Controls.Add(this.mag_linearmipmapnearest_ck);
            this.Controls.Add(this.mag_linearmipmapnearest_label);
            this.Controls.Add(this.mag_linearmipmapnearest_hitbox);
            this.Controls.Add(this.mag_nearestmipmaplinear_ck);
            this.Controls.Add(this.mag_nearestmipmaplinear_label);
            this.Controls.Add(this.mag_nearestmipmaplinear_hitbox);
            this.Controls.Add(this.mag_nearestmipmapnearest_ck);
            this.Controls.Add(this.mag_nearestmipmapnearest_label);
            this.Controls.Add(this.mag_nearestmipmapnearest_hitbox);
            this.Controls.Add(this.mag_linear_ck);
            this.Controls.Add(this.mag_linear_label);
            this.Controls.Add(this.mag_linear_hitbox);
            this.Controls.Add(this.mag_nearest_neighbour_ck);
            this.Controls.Add(this.mag_nearest_neighbour_label);
            this.Controls.Add(this.mag_nearest_neighbour_hitbox);
            this.Controls.Add(this.min_linearmipmaplinear_ck);
            this.Controls.Add(this.min_linearmipmaplinear_label);
            this.Controls.Add(this.min_linearmipmaplinear_hitbox);
            this.Controls.Add(this.min_linearmipmapnearest_ck);
            this.Controls.Add(this.min_linearmipmapnearest_label);
            this.Controls.Add(this.min_linearmipmapnearest_hitbox);
            this.Controls.Add(this.min_nearestmipmaplinear_ck);
            this.Controls.Add(this.min_nearestmipmaplinear_label);
            this.Controls.Add(this.min_nearestmipmaplinear_hitbox);
            this.Controls.Add(this.min_nearestmipmapnearest_ck);
            this.Controls.Add(this.min_nearestmipmapnearest_label);
            this.Controls.Add(this.min_nearestmipmapnearest_hitbox);
            this.Controls.Add(this.min_linear_ck);
            this.Controls.Add(this.min_linear_label);
            this.Controls.Add(this.min_linear_hitbox);
            this.Controls.Add(this.min_nearest_neighbour_ck);
            this.Controls.Add(this.min_nearest_neighbour_label);
            this.Controls.Add(this.min_nearest_neighbour_hitbox);
            this.Controls.Add(this.magnification_label);
            this.Controls.Add(this.minification_label);
            this.Controls.Add(this.Smirror_ck);
            this.Controls.Add(this.Smirror_label);
            this.Controls.Add(this.Smirror_hitbox);
            this.Controls.Add(this.Srepeat_ck);
            this.Controls.Add(this.Srepeat_label);
            this.Controls.Add(this.Srepeat_hitbox);
            this.Controls.Add(this.Sclamp_ck);
            this.Controls.Add(this.Sclamp_label);
            this.Controls.Add(this.Sclamp_hitbox);
            this.Controls.Add(this.WrapS_label);
            this.Controls.Add(this.Tmirror_ck);
            this.Controls.Add(this.Tmirror_label);
            this.Controls.Add(this.Tmirror_hitbox);
            this.Controls.Add(this.Trepeat_ck);
            this.Controls.Add(this.Trepeat_label);
            this.Controls.Add(this.Trepeat_hitbox);
            this.Controls.Add(this.Tclamp_ck);
            this.Controls.Add(this.Tclamp_label);
            this.Controls.Add(this.Tclamp_hitbox);
            this.Controls.Add(this.WrapT_label);
            this.Controls.Add(this.mix_ck);
            this.Controls.Add(this.mix_label);
            this.Controls.Add(this.mix_hitbox);
            this.Controls.Add(this.alpha_ck);
            this.Controls.Add(this.alpha_label);
            this.Controls.Add(this.alpha_hitbox);
            this.Controls.Add(this.no_alpha_ck);
            this.Controls.Add(this.no_alpha_label);
            this.Controls.Add(this.no_alpha_hitbox);
            this.Controls.Add(this.alpha_title);
            this.Controls.Add(this.no_gradient_ck);
            this.Controls.Add(this.no_gradient_label);
            this.Controls.Add(this.no_gradient_hitbox);
            this.Controls.Add(this.custom_ck);
            this.Controls.Add(this.custom_label);
            this.Controls.Add(this.custom_hitbox);
            this.Controls.Add(this.cie_709_ck);
            this.Controls.Add(this.cie_709_label);
            this.Controls.Add(this.cie_709_hitbox);
            this.Controls.Add(this.cie_601_ck);
            this.Controls.Add(this.cie_601_label);
            this.Controls.Add(this.cie_601_hitbox);
            this.Controls.Add(this.algorithm_label);
            this.Controls.Add(this.cmpr_ck);
            this.Controls.Add(this.cmpr_label);
            this.Controls.Add(this.cmpr_hitbox);
            this.Controls.Add(this.ci14x2_ck);
            this.Controls.Add(this.ci14x2_label);
            this.Controls.Add(this.ci14x2_hitbox);
            this.Controls.Add(this.ci8_ck);
            this.Controls.Add(this.ci8_label);
            this.Controls.Add(this.ci8_hitbox);
            this.Controls.Add(this.ci4_ck);
            this.Controls.Add(this.ci4_label);
            this.Controls.Add(this.ci4_hitbox);
            this.Controls.Add(this.rgba32_ck);
            this.Controls.Add(this.rgba32_label);
            this.Controls.Add(this.rgba32_hitbox);
            this.Controls.Add(this.rgb5a3_ck);
            this.Controls.Add(this.rgb5a3_label);
            this.Controls.Add(this.rgb5a3_hitbox);
            this.Controls.Add(this.rgb565_ck);
            this.Controls.Add(this.rgb565_label);
            this.Controls.Add(this.rgb565_hitbox);
            this.Controls.Add(this.ai8_ck);
            this.Controls.Add(this.ai8_label);
            this.Controls.Add(this.ai8_hitbox);
            this.Controls.Add(this.ai4_ck);
            this.Controls.Add(this.ai4_label);
            this.Controls.Add(this.ai4_hitbox);
            this.Controls.Add(this.i8_ck);
            this.Controls.Add(this.i8_label);
            this.Controls.Add(this.i8_hitbox);
            this.Controls.Add(this.i4_ck);
            this.Controls.Add(this.i4_label);
            this.Controls.Add(this.i4_hitbox);
            this.Controls.Add(this.encoding_label);
            this.Controls.Add(this.warn_ck);
            this.Controls.Add(this.warn_label);
            this.Controls.Add(this.stfu_ck);
            this.Controls.Add(this.stfu_label);
            this.Controls.Add(this.safe_mode_ck);
            this.Controls.Add(this.safe_mode_label);
            this.Controls.Add(this.reverse_ck);
            this.Controls.Add(this.reverse_label);
            this.Controls.Add(this.random_ck);
            this.Controls.Add(this.random_label);
            this.Controls.Add(this.no_warning_ck);
            this.Controls.Add(this.no_warning_label);
            this.Controls.Add(this.funky_ck);
            this.Controls.Add(this.funky_label);
            this.Controls.Add(this.FORCE_ALPHA_ck);
            this.Controls.Add(this.FORCE_ALPHA_label);
            this.Controls.Add(this.bmp_32_ck);
            this.Controls.Add(this.bmp_32_label);
            this.Controls.Add(this.ask_exit_ck);
            this.Controls.Add(this.ask_exit_label);
            this.Controls.Add(this.ask_exit_hitbox);
            this.Controls.Add(this.options_label);
            this.Controls.Add(this.tiff_ck);
            this.Controls.Add(this.tiff_label);
            this.Controls.Add(this.tiff_hitbox);
            this.Controls.Add(this.tif_ck);
            this.Controls.Add(this.tif_label);
            this.Controls.Add(this.tif_hitbox);
            this.Controls.Add(this.ico_ck);
            this.Controls.Add(this.ico_label);
            this.Controls.Add(this.ico_hitbox);
            this.Controls.Add(this.gif_ck);
            this.Controls.Add(this.gib_label);
            this.Controls.Add(this.gif_hitbox);
            this.Controls.Add(this.jpeg_ck);
            this.Controls.Add(this.jpeg_label);
            this.Controls.Add(this.jpeg_hitbox);
            this.Controls.Add(this.jpg_ck);
            this.Controls.Add(this.jpg_label);
            this.Controls.Add(this.jpg_hitbox);
            this.Controls.Add(this.png_ck);
            this.Controls.Add(this.png_label);
            this.Controls.Add(this.png_hitbox);
            this.Controls.Add(this.bmp_ck);
            this.Controls.Add(this.bmp_label);
            this.Controls.Add(this.bmp_hitbox);
            this.Controls.Add(this.tpl_ck);
            this.Controls.Add(this.tpl_label);
            this.Controls.Add(this.tpl_hitbox);
            this.Controls.Add(this.tex0_ck);
            this.Controls.Add(this.tex0_label);
            this.Controls.Add(this.tex0_hitbox);
            this.Controls.Add(this.bti_ck);
            this.Controls.Add(this.bti_label);
            this.Controls.Add(this.bti_hitbox);
            this.Controls.Add(this.bmd_ck);
            this.Controls.Add(this.bmd_label);
            this.Controls.Add(this.bmd_hitbox);
            this.Controls.Add(this.mandatory_settings_label);
            this.Controls.Add(this.output_file_type_label);
            this.Controls.Add(this.bmp_32_hitbox);
            this.Controls.Add(this.FORCE_ALPHA_hitbox);
            this.Controls.Add(this.funky_hitbox);
            this.Controls.Add(this.no_warning_hitbox);
            this.Controls.Add(this.random_hitbox);
            this.Controls.Add(this.reverse_hitbox);
            this.Controls.Add(this.safe_mode_hitbox);
            this.Controls.Add(this.stfu_hitbox);
            this.Controls.Add(this.warn_hitbox);
            this.Controls.Add(this.surrounding_ck);
            this.Controls.Add(this.r_r_ck_hitbox);
            this.Controls.Add(this.g_r_ck_hitbox);
            this.Controls.Add(this.r_g_ck_hitbox);
            this.Controls.Add(this.r_a_ck_hitbox);
            this.Controls.Add(this.r_b_ck_hitbox);
            this.Controls.Add(this.g_g_ck_hitbox);
            this.Controls.Add(this.g_b_ck_hitbox);
            this.Controls.Add(this.g_a_ck_hitbox);
            this.Controls.Add(this.b_r_ck_hitbox);
            this.Controls.Add(this.b_g_ck_hitbox);
            this.Controls.Add(this.b_b_ck_hitbox);
            this.Controls.Add(this.b_a_ck_hitbox);
            this.Controls.Add(this.a_r_ck_hitbox);
            this.Controls.Add(this.a_g_ck_hitbox);
            this.Controls.Add(this.a_b_ck_hitbox);
            this.Controls.Add(this.a_a_ck_hitbox);
            this.Controls.Add(this.run_hitbox);
            this.Controls.Add(this.cli_textbox_hitbox);
            this.Controls.Add(this.input_file_hitbox);
            this.Controls.Add(this.input_file2_hitbox);
            this.Controls.Add(this.output_name_hitbox);
            this.Controls.Add(this.mipmaps_hitbox);
            this.Controls.Add(this.cmpr_max_hitbox);
            this.Controls.Add(this.cmpr_min_alpha_hitbox);
            this.Controls.Add(this.num_colours_hitbox);
            this.Controls.Add(this.round3_hitbox);
            this.Controls.Add(this.round4_hitbox);
            this.Controls.Add(this.round5_hitbox);
            this.Controls.Add(this.round6_hitbox);
            this.Controls.Add(this.diversity_hitbox);
            this.Controls.Add(this.diversity2_hitbox);
            this.Controls.Add(this.percentage_hitbox);
            this.Controls.Add(this.percentage2_hitbox);
            this.Controls.Add(this.view_min_label);
            this.Controls.Add(this.view_min_hitbox);
            this.Controls.Add(this.custom_g_hitbox);
            this.Controls.Add(this.custom_b_hitbox);
            this.Controls.Add(this.custom_a_hitbox);
            this.Controls.Add(this.custom_r_hitbox);
            this.Controls.Add(this.desc2);
            this.Controls.Add(this.description);
            this.Controls.Add(this.description_surrounding);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.MaximumSize = new System.Drawing.Size(99999, 99999);
            this.Name = "plt0_gui";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "PLT0 - Image Encoding tool";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.plt0_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.plt0_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.bmd_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bti_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tex0_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpl_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bmp_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.png_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.jpg_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tiff_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tif_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ico_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gif_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.jpeg_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warn_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stfu_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.safe_mode_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reverse_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.random_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.no_warning_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.funky_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FORCE_ALPHA_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bmp_32_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ask_exit_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ci14x2_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ci8_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ci4_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgba32_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgb5a3_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgb565_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ai8_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ai4_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.i8_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.i4_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.surrounding_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.no_gradient_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.custom_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cie_709_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cie_601_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mix_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alpha_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.no_alpha_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tmirror_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Trepeat_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tclamp_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Smirror_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Srepeat_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Sclamp_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_linearmipmaplinear_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_linearmipmapnearest_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_nearestmipmaplinear_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_nearestmipmapnearest_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_linear_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_nearest_neighbour_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mag_linearmipmaplinear_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mag_linearmipmapnearest_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mag_nearestmipmaplinear_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mag_nearestmipmapnearest_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mag_linear_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mag_nearest_neighbour_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.r_r_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.r_g_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.g_r_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.g_g_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_g_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_r_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.b_g_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.b_r_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.g_a_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.g_b_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.r_a_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.r_b_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_a_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_b_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.b_a_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.b_b_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_alpha_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_algorithm_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_WrapS_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_WrapT_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_mag_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_min_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.all_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.auto_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paint_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_x_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_5_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_minus_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_9_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_8_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_7_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_6_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_4_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_3_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_2_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_1_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cli_textbox_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.run_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.description_surrounding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.palette_rgb5a3_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.palette_rgb565_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.palette_ai8_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.github_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.youtube_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.discord_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.version_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_rgba_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_palette_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_cmpr_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_options_ck)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        // all the lines below 'till the end are the output of python scripts I made
        // I had to name all labels and pictureboxes through the [Design] tab manually though.
        // I also had to redirect the labels to each function below, which again, isn't pretty fast

        // generated checkbox behaviour code by the python script in the py folder
        private void Load_Images()
        {
            if (File.Exists(execPath + "images/banner.png"))
            {
                banner = Image.FromFile(execPath + "images/banner.png");
            }
            if (File.Exists(execPath + "images/background.png"))
            {
                background = Image.FromFile(execPath + "images/background.png");
            }
            if (File.Exists(execPath + "images/white_box.png"))
            {
                white_box = Image.FromFile(execPath + "images/white_box.png");
            }
            if (File.Exists(execPath + "images/light_blue_box.png"))
            {
                light_blue_box = Image.FromFile(execPath + "images/light_blue_box.png");
            }
            if (File.Exists(execPath + "images/light_blue_check.png"))
            {
                light_blue_check = Image.FromFile(execPath + "images/light_blue_check.png");
            }
            if (File.Exists(execPath + "images/check.png"))
            {
                check = Image.FromFile(execPath + "images/check.png");
            }
            if (File.Exists(execPath + "images/pink_circle.png"))
            {
                pink_circle = Image.FromFile(execPath + "images/pink_circle.png");
            }
            if (File.Exists(execPath + "images/pink_circle_on.png"))
            {
                pink_circle_on = Image.FromFile(execPath + "images/pink_circle_on.png");
            }
            if (File.Exists(execPath + "images/violet_circle.png"))
            {
                violet_circle = Image.FromFile(execPath + "images/violet_circle.png");
            }
            if (File.Exists(execPath + "images/violet_circle_on.png"))
            {
                violet_circle_on = Image.FromFile(execPath + "images/violet_circle_on.png");
            }
            if (File.Exists(execPath + "images/white_circle.png"))
            {
                white_circle = Image.FromFile(execPath + "images/white_circle.png");
            }
            if (File.Exists(execPath + "images/white_circle_on.png"))
            {
                white_circle_on = Image.FromFile(execPath + "images/white_circle_on.png");
            }
            if (File.Exists(execPath + "images/green_circle.png"))
            {
                green_circle = Image.FromFile(execPath + "images/green_circle.png");
            }
            if (File.Exists(execPath + "images/green_circle_on.png"))
            {
                green_circle_on = Image.FromFile(execPath + "images/green_circle_on.png");
            }
            if (File.Exists(execPath + "images/light_blue_circle.png"))
            {
                light_blue_circle = Image.FromFile(execPath + "images/light_blue_circle.png");
            }
            if (File.Exists(execPath + "images/light_blue_circle_on.png"))
            {
                light_blue_circle_on = Image.FromFile(execPath + "images/light_blue_circle_on.png");
            }
            if (File.Exists(execPath + "images/blue_circle.png"))
            {
                blue_circle = Image.FromFile(execPath + "images/blue_circle.png");
            }
            if (File.Exists(execPath + "images/blue_circle_on.png"))
            {
                blue_circle_on = Image.FromFile(execPath + "images/blue_circle_on.png");
            }
            if (File.Exists(execPath + "images/light_red_circle.png"))
            {
                light_red_circle = Image.FromFile(execPath + "images/light_red_circle.png");
            }
            if (File.Exists(execPath + "images/light_red_circle_on.png"))
            {
                light_red_circle_on = Image.FromFile(execPath + "images/light_red_circle_on.png");
            }
            if (File.Exists(execPath + "images/red_circle.png"))
            {
                red_circle = Image.FromFile(execPath + "images/red_circle.png");
            }
            if (File.Exists(execPath + "images/red_circle_on.png"))
            {
                red_circle_on = Image.FromFile(execPath + "images/red_circle_on.png");
            }
            if (File.Exists(execPath + "images/yellow_circle.png"))
            {
                yellow_circle = Image.FromFile(execPath + "images/yellow_circle.png");
            }
            if (File.Exists(execPath + "images/yellow_circle_on.png"))
            {
                yellow_circle_on = Image.FromFile(execPath + "images/yellow_circle_on.png");
            }
            if (File.Exists(execPath + "images/orange_circle.png"))
            {
                orange_circle = Image.FromFile(execPath + "images/orange_circle.png");
            }
            if (File.Exists(execPath + "images/orange_circle_on.png"))
            {
                orange_circle_on = Image.FromFile(execPath + "images/orange_circle_on.png");
            }
            if (File.Exists(execPath + "images/fushia_circle.png"))
            {
                fushia_circle = Image.FromFile(execPath + "images/fushia_circle.png");
            }
            if (File.Exists(execPath + "images/fushia_circle_on.png"))
            {
                fushia_circle_on = Image.FromFile(execPath + "images/fushia_circle_on.png");
            }
            if (File.Exists(execPath + "images/cyan_circle.png"))
            {
                cyan_circle = Image.FromFile(execPath + "images/cyan_circle.png");
            }
            if (File.Exists(execPath + "images/cyan_circle_on.png"))
            {
                cyan_circle_on = Image.FromFile(execPath + "images/cyan_circle_on.png");
            }
            if (File.Exists(execPath + "images/cherry_circle.png"))
            {
                cherry_circle = Image.FromFile(execPath + "images/cherry_circle.png");
            }
            if (File.Exists(execPath + "images/cherry_circle_on.png"))
            {
                cherry_circle_on = Image.FromFile(execPath + "images/cherry_circle_on.png");
            }
            if (File.Exists(execPath + "images/purple_circle.png"))
            {
                purple_circle = Image.FromFile(execPath + "images/purple_circle.png");
            }
            if (File.Exists(execPath + "images/purple_circle_on.png"))
            {
                purple_circle_on = Image.FromFile(execPath + "images/purple_circle_on.png");
            }
            if (File.Exists(execPath + "images/chartreuse_circle.png"))
            {
                chartreuse_circle = Image.FromFile(execPath + "images/chartreuse_circle.png");
            }
            if (File.Exists(execPath + "images/chartreuse_circle_on.png"))
            {
                chartreuse_circle_on = Image.FromFile(execPath + "images/chartreuse_circle_on.png");
            }
            if (File.Exists(execPath + "images/a_on.png"))
            {
                a_on = Image.FromFile(execPath + "images/a_on.png");
            }
            if (File.Exists(execPath + "images/a_off.png"))
            {
                a_off = Image.FromFile(execPath + "images/a_off.png");
            }
            if (File.Exists(execPath + "images/a_hover.png"))
            {
                a_hover = Image.FromFile(execPath + "images/a_hover.png");
            }
            if (File.Exists(execPath + "images/a_selected.png"))
            {
                a_selected = Image.FromFile(execPath + "images/a_selected.png");
            }
            if (File.Exists(execPath + "images/b_on.png"))
            {
                b_on = Image.FromFile(execPath + "images/b_on.png");
            }
            if (File.Exists(execPath + "images/b_off.png"))
            {
                b_off = Image.FromFile(execPath + "images/b_off.png");
            }
            if (File.Exists(execPath + "images/b_hover.png"))
            {
                b_hover = Image.FromFile(execPath + "images/b_hover.png");
            }
            if (File.Exists(execPath + "images/b_selected.png"))
            {
                b_selected = Image.FromFile(execPath + "images/b_selected.png");
            }
            if (File.Exists(execPath + "images/g_on.png"))
            {
                g_on = Image.FromFile(execPath + "images/g_on.png");
            }
            if (File.Exists(execPath + "images/g_off.png"))
            {
                g_off = Image.FromFile(execPath + "images/g_off.png");
            }
            if (File.Exists(execPath + "images/g_hover.png"))
            {
                g_hover = Image.FromFile(execPath + "images/g_hover.png");
            }
            if (File.Exists(execPath + "images/g_selected.png"))
            {
                g_selected = Image.FromFile(execPath + "images/g_selected.png");
            }
            if (File.Exists(execPath + "images/r_on.png"))
            {
                r_on = Image.FromFile(execPath + "images/r_on.png");
            }
            if (File.Exists(execPath + "images/r_off.png"))
            {
                r_off = Image.FromFile(execPath + "images/r_off.png");
            }
            if (File.Exists(execPath + "images/r_hover.png"))
            {
                r_hover = Image.FromFile(execPath + "images/r_hover.png");
            }
            if (File.Exists(execPath + "images/r_selected.png"))
            {
                r_selected = Image.FromFile(execPath + "images/r_selected.png");
            }
            if (File.Exists(execPath + "images/surrounding.png"))
            {
                surrounding = Image.FromFile(execPath + "images/surrounding.png");
            }
            if (File.Exists(execPath + "images/all_on.png"))
            {
                all_on = Image.FromFile(execPath + "images/all_on.png");
            }
            if (File.Exists(execPath + "images/all_off.png"))
            {
                all_off = Image.FromFile(execPath + "images/all_off.png");
            }
            if (File.Exists(execPath + "images/all_hover.png"))
            {
                all_hover = Image.FromFile(execPath + "images/all_hover.png");
            }
            if (File.Exists(execPath + "images/all_selected.png"))
            {
                all_selected = Image.FromFile(execPath + "images/all_selected.png");
            }
            if (File.Exists(execPath + "images/auto_on.png"))
            {
                auto_on = Image.FromFile(execPath + "images/auto_on.png");
            }
            if (File.Exists(execPath + "images/auto_off.png"))
            {
                auto_off = Image.FromFile(execPath + "images/auto_off.png");
            }
            if (File.Exists(execPath + "images/auto_hover.png"))
            {
                auto_hover = Image.FromFile(execPath + "images/auto_hover.png");
            }
            if (File.Exists(execPath + "images/auto_selected.png"))
            {
                auto_selected = Image.FromFile(execPath + "images/auto_selected.png");
            }
            if (File.Exists(execPath + "images/preview_on.png"))
            {
                preview_on = Image.FromFile(execPath + "images/preview_on.png");
            }
            if (File.Exists(execPath + "images/preview_off.png"))
            {
                preview_off = Image.FromFile(execPath + "images/preview_off.png");
            }
            if (File.Exists(execPath + "images/preview_hover.png"))
            {
                preview_hover = Image.FromFile(execPath + "images/preview_hover.png");
            }
            if (File.Exists(execPath + "images/preview_selected.png"))
            {
                preview_selected = Image.FromFile(execPath + "images/preview_selected.png");
            }
            if (File.Exists(execPath + "images/paint_on.png"))
            {
                paint_on = Image.FromFile(execPath + "images/paint_on.png");
            }
            if (File.Exists(execPath + "images/paint_off.png"))
            {
                paint_off = Image.FromFile(execPath + "images/paint_off.png");
            }
            if (File.Exists(execPath + "images/paint_hover.png"))
            {
                paint_hover = Image.FromFile(execPath + "images/paint_hover.png");
            }
            if (File.Exists(execPath + "images/paint_selected.png"))
            {
                paint_selected = Image.FromFile(execPath + "images/paint_selected.png");
            }
            if (File.Exists(execPath + "images/bottom_left_on.png"))
            {
                bottom_left_on = Image.FromFile(execPath + "images/bottom_left_on.png");
            }
            if (File.Exists(execPath + "images/bottom_left_off.png"))
            {
                bottom_left_off = Image.FromFile(execPath + "images/bottom_left_off.png");
            }
            if (File.Exists(execPath + "images/bottom_left_hover.png"))
            {
                bottom_left_hover = Image.FromFile(execPath + "images/bottom_left_hover.png");
            }
            if (File.Exists(execPath + "images/bottom_left_selected.png"))
            {
                bottom_left_selected = Image.FromFile(execPath + "images/bottom_left_selected.png");
            }
            if (File.Exists(execPath + "images/left_on.png"))
            {
                left_on = Image.FromFile(execPath + "images/left_on.png");
            }
            if (File.Exists(execPath + "images/left_off.png"))
            {
                left_off = Image.FromFile(execPath + "images/left_off.png");
            }
            if (File.Exists(execPath + "images/left_hover.png"))
            {
                left_hover = Image.FromFile(execPath + "images/left_hover.png");
            }
            if (File.Exists(execPath + "images/left_selected.png"))
            {
                left_selected = Image.FromFile(execPath + "images/left_selected.png");
            }
            if (File.Exists(execPath + "images/top_left_on.png"))
            {
                top_left_on = Image.FromFile(execPath + "images/top_left_on.png");
            }
            if (File.Exists(execPath + "images/top_left_off.png"))
            {
                top_left_off = Image.FromFile(execPath + "images/top_left_off.png");
            }
            if (File.Exists(execPath + "images/top_left_hover.png"))
            {
                top_left_hover = Image.FromFile(execPath + "images/top_left_hover.png");
            }
            if (File.Exists(execPath + "images/top_left_selected.png"))
            {
                top_left_selected = Image.FromFile(execPath + "images/top_left_selected.png");
            }
            if (File.Exists(execPath + "images/bottom_right_on.png"))
            {
                bottom_right_on = Image.FromFile(execPath + "images/bottom_right_on.png");
            }
            if (File.Exists(execPath + "images/bottom_right_off.png"))
            {
                bottom_right_off = Image.FromFile(execPath + "images/bottom_right_off.png");
            }
            if (File.Exists(execPath + "images/bottom_right_hover.png"))
            {
                bottom_right_hover = Image.FromFile(execPath + "images/bottom_right_hover.png");
            }
            if (File.Exists(execPath + "images/bottom_right_selected.png"))
            {
                bottom_right_selected = Image.FromFile(execPath + "images/bottom_right_selected.png");
            }
            if (File.Exists(execPath + "images/right_on.png"))
            {
                right_on = Image.FromFile(execPath + "images/right_on.png");
            }
            if (File.Exists(execPath + "images/right_off.png"))
            {
                right_off = Image.FromFile(execPath + "images/right_off.png");
            }
            if (File.Exists(execPath + "images/right_hover.png"))
            {
                right_hover = Image.FromFile(execPath + "images/right_hover.png");
            }
            if (File.Exists(execPath + "images/right_selected.png"))
            {
                right_selected = Image.FromFile(execPath + "images/right_selected.png");
            }
            if (File.Exists(execPath + "images/top_right_on.png"))
            {
                top_right_on = Image.FromFile(execPath + "images/top_right_on.png");
            }
            if (File.Exists(execPath + "images/top_right_off.png"))
            {
                top_right_off = Image.FromFile(execPath + "images/top_right_off.png");
            }
            if (File.Exists(execPath + "images/top_right_hover.png"))
            {
                top_right_hover = Image.FromFile(execPath + "images/top_right_hover.png");
            }
            if (File.Exists(execPath + "images/top_right_selected.png"))
            {
                top_right_selected = Image.FromFile(execPath + "images/top_right_selected.png");
            }
            if (File.Exists(execPath + "images/bottom_on.png"))
            {
                bottom_on = Image.FromFile(execPath + "images/bottom_on.png");
            }
            if (File.Exists(execPath + "images/bottom_off.png"))
            {
                bottom_off = Image.FromFile(execPath + "images/bottom_off.png");
            }
            if (File.Exists(execPath + "images/bottom_hover.png"))
            {
                bottom_hover = Image.FromFile(execPath + "images/bottom_hover.png");
            }
            if (File.Exists(execPath + "images/bottom_selected.png"))
            {
                bottom_selected = Image.FromFile(execPath + "images/bottom_selected.png");
            }
            if (File.Exists(execPath + "images/top_on.png"))
            {
                top_on = Image.FromFile(execPath + "images/top_on.png");
            }
            if (File.Exists(execPath + "images/top_off.png"))
            {
                top_off = Image.FromFile(execPath + "images/top_off.png");
            }
            if (File.Exists(execPath + "images/top_hover.png"))
            {
                top_hover = Image.FromFile(execPath + "images/top_hover.png");
            }
            if (File.Exists(execPath + "images/top_selected.png"))
            {
                top_selected = Image.FromFile(execPath + "images/top_selected.png");
            }
            if (File.Exists(execPath + "images/close.png"))
            {
                close = Image.FromFile(execPath + "images/close.png");
            }
            if (File.Exists(execPath + "images/close_hover.png"))
            {
                close_hover = Image.FromFile(execPath + "images/close_hover.png");
            }
            if (File.Exists(execPath + "images/maximized_on.png"))
            {
                maximized_on = Image.FromFile(execPath + "images/maximized_on.png");
            }
            if (File.Exists(execPath + "images/maximized_off.png"))
            {
                maximized_off = Image.FromFile(execPath + "images/maximized_off.png");
            }
            if (File.Exists(execPath + "images/maximized_hover.png"))
            {
                maximized_hover = Image.FromFile(execPath + "images/maximized_hover.png");
            }
            if (File.Exists(execPath + "images/maximized_selected.png"))
            {
                maximized_selected = Image.FromFile(execPath + "images/maximized_selected.png");
            }
            if (File.Exists(execPath + "images/minimized.png"))
            {
                minimized = Image.FromFile(execPath + "images/minimized.png");
            }
            if (File.Exists(execPath + "images/minimized_hover.png"))
            {
                minimized_hover = Image.FromFile(execPath + "images/minimized_hover.png");
            }
            if (File.Exists(execPath + "images/youtube.png"))
            {
                youtube = Image.FromFile(execPath + "images/youtube.png");
            }
            if (File.Exists(execPath + "images/discord.png"))
            {
                discord = Image.FromFile(execPath + "images/discord.png");
            }
            if (File.Exists(execPath + "images/github.png"))
            {
                github = Image.FromFile(execPath + "images/github.png");
            }
            if (File.Exists(execPath + "images/youtube_hover.png"))
            {
                youtube_hover = Image.FromFile(execPath + "images/youtube_hover.png");
            }
            if (File.Exists(execPath + "images/discord_hover.png"))
            {
                discord_hover = Image.FromFile(execPath + "images/discord_hover.png");
            }
            if (File.Exists(execPath + "images/github_hover.png"))
            {
                github_hover = Image.FromFile(execPath + "images/github_hover.png");
            }
            if (File.Exists(execPath + "images/run_on.png"))
            {
                run_on = Image.FromFile(execPath + "images/run_on.png");
            }
            if (File.Exists(execPath + "images/run_off.png"))
            {
                run_off = Image.FromFile(execPath + "images/run_off.png");
            }
            if (File.Exists(execPath + "images/run_hover.png"))
            {
                run_hover = Image.FromFile(execPath + "images/run_hover.png");
            }
            if (File.Exists(execPath + "images/cli_textbox.png"))
            {
                cli_textbox = Image.FromFile(execPath + "images/cli_textbox.png");
            }
            if (File.Exists(execPath + "images/cli_textbox_hover.png"))
            {
                cli_textbox_hover = Image.FromFile(execPath + "images/cli_textbox_hover.png");
            }
            if (File.Exists(execPath + "images/version.png"))
            {
                version = Image.FromFile(execPath + "images/version.png");
            }
            if (File.Exists(execPath + "images/version_hover.png"))
            {
                version_hover = Image.FromFile(execPath + "images/version_hover.png");
            }
        }
        private void bmd_Click(object sender, EventArgs e)
        {
            if (bmd)
            {
                bmd = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("bmd ", "");
                hover_checkbox(bmd_ck);
                View_WrapS();
                View_WrapT();
                View_min();
                View_mag();
            }
            else
            {
                bmd = true;
                cli_textbox_label.Text += "bmd ";
                selected_checkbox(bmd_ck);
                Hide_WrapS();
                Hide_WrapT();
                Hide_min();
                Hide_mag();
            }
            Check_run();
        }
        private void bmd_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[59]);
            if (bmd)
                selected_checkbox(bmd_ck);
            else
                hover_checkbox(bmd_ck);
        }
        private void bmd_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (bmd)
                checked_checkbox(bmd_ck);
            else
                unchecked_checkbox(bmd_ck);
        }
        private void bti_Click(object sender, EventArgs e)
        {
            if (bti)
            {
                bti = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("bti ", "");
                hover_checkbox(bti_ck);
            }
            else
            {
                bti = true;
                cli_textbox_label.Text += "bti ";
                selected_checkbox(bti_ck);
            }
            Check_run();
        }
        private void bti_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[60]);
            if (bti)
                selected_checkbox(bti_ck);
            else
                hover_checkbox(bti_ck);
        }
        private void bti_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (bti)
                checked_checkbox(bti_ck);
            else
                unchecked_checkbox(bti_ck);
        }
        private void tex0_Click(object sender, EventArgs e)
        {
            if (tex0)
            {
                tex0 = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("tex0 ", "");
                hover_checkbox(tex0_ck);
                View_WrapS();
                View_WrapT();
                View_min();
                View_mag();
            }
            else
            {
                tex0 = true;
                cli_textbox_label.Text += "tex0 ";
                selected_checkbox(tex0_ck);
                Hide_WrapS();
                Hide_WrapT();
                Hide_min();
                Hide_mag();
            }
            Check_run();
        }
        private void tex0_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[61]);
            if (tex0)
                selected_checkbox(tex0_ck);
            else
                hover_checkbox(tex0_ck);
        }
        private void tex0_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (tex0)
                checked_checkbox(tex0_ck);
            else
                unchecked_checkbox(tex0_ck);
        }
        private void tpl_Click(object sender, EventArgs e)
        {
            if (tpl)
            {
                tpl = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("tpl ", "");
                hover_checkbox(tpl_ck);
            }
            else
            {
                tpl = true;
                cli_textbox_label.Text += "tpl ";
                selected_checkbox(tpl_ck);
            }
            Check_run();
        }
        private void tpl_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[62]);
            if (tpl)
                selected_checkbox(tpl_ck);
            else
                hover_checkbox(tpl_ck);
        }
        private void tpl_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (tpl)
                checked_checkbox(tpl_ck);
            else
                unchecked_checkbox(tpl_ck);
        }
        private void bmp_Click(object sender, EventArgs e)
        {
            if (bmp)
            {
                bmp = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("bmp ", "");
                hover_checkbox(bmp_ck);
            }
            else
            {
                bmp = true;
                cli_textbox_label.Text += "bmp ";
                selected_checkbox(bmp_ck);
            }
            Check_run();
        }
        private void bmp_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[63]);
            if (bmp)
                selected_checkbox(bmp_ck);
            else
                hover_checkbox(bmp_ck);
        }
        private void bmp_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (bmp)
                checked_checkbox(bmp_ck);
            else
                unchecked_checkbox(bmp_ck);
        }
        private void png_Click(object sender, EventArgs e)
        {
            if (png)
            {
                png = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("png ", "");
                hover_checkbox(png_ck);
            }
            else
            {
                png = true;
                cli_textbox_label.Text += "png ";
                selected_checkbox(png_ck);
            }
            Check_run();
        }
        private void png_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[64]);
            if (png)
                selected_checkbox(png_ck);
            else
                hover_checkbox(png_ck);
        }
        private void png_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (png)
                checked_checkbox(png_ck);
            else
                unchecked_checkbox(png_ck);
        }
        private void jpg_Click(object sender, EventArgs e)
        {
            if (jpg)
            {
                jpg = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("jpg ", "");
                hover_checkbox(jpg_ck);
            }
            else
            {
                jpg = true;
                cli_textbox_label.Text += "jpg ";
                selected_checkbox(jpg_ck);
            }
            Check_run();
        }
        private void jpg_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[65]);
            if (jpg)
                selected_checkbox(jpg_ck);
            else
                hover_checkbox(jpg_ck);
        }
        private void jpg_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (jpg)
                checked_checkbox(jpg_ck);
            else
                unchecked_checkbox(jpg_ck);
        }
        private void jpeg_Click(object sender, EventArgs e)
        {
            if (jpeg)
            {
                jpeg = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("jpeg ", "");
                hover_checkbox(jpeg_ck);
            }
            else
            {
                jpeg = true;
                cli_textbox_label.Text += "jpeg ";
                selected_checkbox(jpeg_ck);
            }
            Check_run();
        }
        private void jpeg_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[66]);
            if (jpeg)
                selected_checkbox(jpeg_ck);
            else
                hover_checkbox(jpeg_ck);
        }
        private void jpeg_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (jpeg)
                checked_checkbox(jpeg_ck);
            else
                unchecked_checkbox(jpeg_ck);
        }
        private void gif_Click(object sender, EventArgs e)
        {
            if (gif)
            {
                gif = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("gif ", "");
                hover_checkbox(gif_ck);
            }
            else
            {
                gif = true;
                cli_textbox_label.Text += "gif ";
                selected_checkbox(gif_ck);
            }
            Check_run();
        }
        private void gif_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[67]);
            if (gif)
                selected_checkbox(gif_ck);
            else
                hover_checkbox(gif_ck);
        }
        private void gif_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (gif)
                checked_checkbox(gif_ck);
            else
                unchecked_checkbox(gif_ck);
        }
        private void ico_Click(object sender, EventArgs e)
        {
            if (ico)
            {
                ico = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("ico ", "");
                hover_checkbox(ico_ck);
            }
            else
            {
                ico = true;
                cli_textbox_label.Text += "ico ";
                selected_checkbox(ico_ck);
            }
            Check_run();
        }
        private void ico_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[68]);
            if (ico)
                selected_checkbox(ico_ck);
            else
                hover_checkbox(ico_ck);
        }
        private void ico_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (ico)
                checked_checkbox(ico_ck);
            else
                unchecked_checkbox(ico_ck);
        }
        private void tif_Click(object sender, EventArgs e)
        {
            if (tif)
            {
                tif = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("tif ", "");
                hover_checkbox(tif_ck);
            }
            else
            {
                tif = true;
                cli_textbox_label.Text += "tif ";
                selected_checkbox(tif_ck);
            }
            Check_run();
        }
        private void tif_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[69]);
            if (tif)
                selected_checkbox(tif_ck);
            else
                hover_checkbox(tif_ck);
        }
        private void tif_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (tif)
                checked_checkbox(tif_ck);
            else
                unchecked_checkbox(tif_ck);
        }
        private void tiff_Click(object sender, EventArgs e)
        {
            if (tiff)
            {
                tiff = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("tiff ", "");
                hover_checkbox(tiff_ck);
            }
            else
            {
                tiff = true;
                cli_textbox_label.Text += "tiff ";
                selected_checkbox(tiff_ck);
            }
        }
        private void tiff_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[70]);
            if (tiff)
                selected_checkbox(tiff_ck);
            else
                hover_checkbox(tiff_ck);
        }
        private void tiff_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (tiff)
                checked_checkbox(tiff_ck);
            else
                unchecked_checkbox(tiff_ck);
        }
        private void ask_exit_Click(object sender, EventArgs e)
        {
            if (ask_exit)
            {
                ask_exit = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("ask_exit ", "");
                hover_checkbox(ask_exit_ck);
            }
            else
            {
                ask_exit = true;
                cli_textbox_label.Text += "ask_exit ";
                selected_checkbox(ask_exit_ck);
            }
        }
        private void ask_exit_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[71]);
            if (ask_exit)
                selected_checkbox(ask_exit_ck);
            else
                hover_checkbox(ask_exit_ck);
        }
        private void ask_exit_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (ask_exit)
                checked_checkbox(ask_exit_ck);
            else
                unchecked_checkbox(ask_exit_ck);
        }
        private void bmp_32_Click(object sender, EventArgs e)
        {
            if (bmp_32)
            {
                bmp_32 = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("bmp_32 ", "");
                hover_checkbox(bmp_32_ck);
            }
            else
            {
                bmp_32 = true;
                cli_textbox_label.Text += "bmp_32 ";
                selected_checkbox(bmp_32_ck);
            }
        }
        private void bmp_32_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[72]);
            if (bmp_32)
                selected_checkbox(bmp_32_ck);
            else
                hover_checkbox(bmp_32_ck);
        }
        private void bmp_32_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (bmp_32)
                checked_checkbox(bmp_32_ck);
            else
                unchecked_checkbox(bmp_32_ck);
        }
        private void FORCE_ALPHA_Click(object sender, EventArgs e)
        {
            if (FORCE_ALPHA)
            {
                FORCE_ALPHA = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("FORCE_ALPHA ", "");
                hover_checkbox(FORCE_ALPHA_ck);
            }
            else
            {
                FORCE_ALPHA = true;
                cli_textbox_label.Text += "FORCE_ALPHA ";
                selected_checkbox(FORCE_ALPHA_ck);
            }
        }
        private void FORCE_ALPHA_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[73]);
            if (FORCE_ALPHA)
                selected_checkbox(FORCE_ALPHA_ck);
            else
                hover_checkbox(FORCE_ALPHA_ck);
        }
        private void FORCE_ALPHA_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (FORCE_ALPHA)
                checked_checkbox(FORCE_ALPHA_ck);
            else
                unchecked_checkbox(FORCE_ALPHA_ck);
        }
        private void funky_Click(object sender, EventArgs e)
        {
            if (funky)
            {
                funky = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("funky ", "");
                hover_checkbox(funky_ck);
            }
            else
            {
                funky = true;
                cli_textbox_label.Text += "funky ";
                selected_checkbox(funky_ck);
            }
        }
        private void funky_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[74]);
            if (funky)
                selected_checkbox(funky_ck);
            else
                hover_checkbox(funky_ck);
        }
        private void funky_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (funky)
                checked_checkbox(funky_ck);
            else
                unchecked_checkbox(funky_ck);
        }
        private void no_warning_Click(object sender, EventArgs e)
        {
            if (no_warning)
            {
                no_warning = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("no_warning ", "");
                hover_checkbox(no_warning_ck);
            }
            else
            {
                no_warning = true;
                cli_textbox_label.Text += "no_warning ";
                selected_checkbox(no_warning_ck);
            }
        }
        private void no_warning_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[75]);
            if (no_warning)
                selected_checkbox(no_warning_ck);
            else
                hover_checkbox(no_warning_ck);
        }
        private void no_warning_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (no_warning)
                checked_checkbox(no_warning_ck);
            else
                unchecked_checkbox(no_warning_ck);
        }
        private void random_Click(object sender, EventArgs e)
        {
            if (random)
            {
                random = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("random ", "");
                hover_checkbox(random_ck);
            }
            else
            {
                random = true;
                cli_textbox_label.Text += "random ";
                selected_checkbox(random_ck);
            }
        }
        private void random_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[76]);
            if (random)
                selected_checkbox(random_ck);
            else
                hover_checkbox(random_ck);
        }
        private void random_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (random)
                checked_checkbox(random_ck);
            else
                unchecked_checkbox(random_ck);
        }
        private void reverse_Click(object sender, EventArgs e)
        {
            if (reverse)
            {
                reverse = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("reverse ", "");
                hover_checkbox(reverse_ck);
            }
            else
            {
                reverse = true;
                cli_textbox_label.Text += "reverse ";
                selected_checkbox(reverse_ck);
            }
        }
        private void reverse_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[77]);
            if (reverse)
                selected_checkbox(reverse_ck);
            else
                hover_checkbox(reverse_ck);
        }
        private void reverse_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (reverse)
                checked_checkbox(reverse_ck);
            else
                unchecked_checkbox(reverse_ck);
        }
        private void safe_mode_Click(object sender, EventArgs e)
        {
            if (safe_mode)
            {
                safe_mode = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("safe_mode ", "");
                hover_checkbox(safe_mode_ck);
            }
            else
            {
                safe_mode = true;
                cli_textbox_label.Text += "safe_mode ";
                selected_checkbox(safe_mode_ck);
            }
        }
        private void safe_mode_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[78]);
            if (safe_mode)
                selected_checkbox(safe_mode_ck);
            else
                hover_checkbox(safe_mode_ck);
        }
        private void safe_mode_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (safe_mode)
                checked_checkbox(safe_mode_ck);
            else
                unchecked_checkbox(safe_mode_ck);
        }
        private void stfu_Click(object sender, EventArgs e)
        {
            if (stfu)
            {
                stfu = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("stfu ", "");
                hover_checkbox(stfu_ck);
            }
            else
            {
                stfu = true;
                cli_textbox_label.Text += "stfu ";
                selected_checkbox(stfu_ck);
            }
        }
        private void stfu_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[79]);
            if (stfu)
                selected_checkbox(stfu_ck);
            else
                hover_checkbox(stfu_ck);
        }
        private void stfu_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (stfu)
                checked_checkbox(stfu_ck);
            else
                unchecked_checkbox(stfu_ck);
        }
        private void warn_Click(object sender, EventArgs e)
        {
            if (warn)
            {
                warn = false;
                cli_textbox_label.Text = cli_textbox_label.Text.Replace("warn ", "");
                hover_checkbox(warn_ck);
            }
            else
            {
                warn = true;
                cli_textbox_label.Text += "warn ";
                selected_checkbox(warn_ck);
            }
        }
        private void warn_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[80]);
            if (warn)
                selected_checkbox(warn_ck);
            else
                hover_checkbox(warn_ck);
        }
        private void warn_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (warn)
                checked_checkbox(warn_ck);
            else
                unchecked_checkbox(warn_ck);
        }
        private void I4_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            Hide_encoding(encoding);
            selected_encoding(i4_ck);
            encoding = 0; // I4
            Check_run();
            View_i4();
            Organize_args();
        }
        private void I4_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[81]);
            if (encoding == 0)
                selected_encoding(i4_ck);
            else
                hover_encoding(i4_ck);
        }
        private void I4_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (encoding == 0)
                checked_encoding(i4_ck);
            else
                unchecked_encoding(i4_ck);
        }
        private void I8_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            Hide_encoding(encoding);
            selected_encoding(i8_ck);
            encoding = 1; // I8
            Check_run();
            View_i8();
            Organize_args();
        }
        private void I8_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[82]);
            if (encoding == 1)
                selected_encoding(i8_ck);
            else
                hover_encoding(i8_ck);
        }
        private void I8_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (encoding == 1)
                checked_encoding(i8_ck);
            else
                unchecked_encoding(i8_ck);
        }
        private void AI4_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            Hide_encoding(encoding);
            selected_encoding(ai4_ck);
            encoding = 2; // AI4
            Check_run();
            View_ai4();
            Organize_args();
        }
        private void AI4_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[83]);
            if (encoding == 2)
                selected_encoding(ai4_ck);
            else
                hover_encoding(ai4_ck);
        }
        private void AI4_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (encoding == 2)
                checked_encoding(ai4_ck);
            else
                unchecked_encoding(ai4_ck);
        }
        private void AI8_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            Hide_encoding(encoding);
            selected_encoding(ai8_ck);
            encoding = 3; // AI8
            Check_run();
            View_ai8();
            Organize_args();
        }
        private void AI8_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[84]);
            if (encoding == 3)
                selected_encoding(ai8_ck);
            else
                hover_encoding(ai8_ck);
        }
        private void AI8_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (encoding == 3)
                checked_encoding(ai8_ck);
            else
                unchecked_encoding(ai8_ck);
        }
        private void RGB565_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            Hide_encoding(encoding);
            selected_encoding(rgb565_ck);
            encoding = 4; // RGB565
            Check_run();
            View_rgb565();
            Organize_args();
        }
        private void RGB565_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[85]);
            if (encoding == 4)
                selected_encoding(rgb565_ck);
            else
                hover_encoding(rgb565_ck);
        }
        private void RGB565_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (encoding == 4)
                checked_encoding(rgb565_ck);
            else
                unchecked_encoding(rgb565_ck);
        }
        private void RGB5A3_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            Hide_encoding(encoding);
            selected_encoding(rgb5a3_ck);
            encoding = 5; // RGB5A3
            Check_run();
            View_rgb5a3();
            Organize_args();
        }
        private void RGB5A3_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[86]);
            if (encoding == 5)
                selected_encoding(rgb5a3_ck);
            else
                hover_encoding(rgb5a3_ck);
        }
        private void RGB5A3_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (encoding == 5)
                checked_encoding(rgb5a3_ck);
            else
                unchecked_encoding(rgb5a3_ck);
        }
        private void RGBA32_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            Hide_encoding(encoding);
            selected_encoding(rgba32_ck);
            encoding = 6; // RGBA32
            Check_run();
            View_rgba32();
            Organize_args();
        }
        private void RGBA32_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[87]);
            if (encoding == 6)
                selected_encoding(rgba32_ck);
            else
                hover_encoding(rgba32_ck);
        }
        private void RGBA32_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (encoding == 6)
                checked_encoding(rgba32_ck);
            else
                unchecked_encoding(rgba32_ck);
        }
        private void CI4_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            Hide_encoding(encoding);
            selected_encoding(ci4_ck);
            encoding = 8; // CI4
            Check_run();
            View_ci4();
            Organize_args();
        }
        private void CI4_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[88]);
            if (encoding == 8)
                selected_encoding(ci4_ck);
            else
                hover_encoding(ci4_ck);
        }
        private void CI4_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (encoding == 8)
                checked_encoding(ci4_ck);
            else
                unchecked_encoding(ci4_ck);
        }
        private void CI8_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            Hide_encoding(encoding);
            selected_encoding(ci8_ck);
            encoding = 9; // CI8
            Check_run();
            View_ci8();
            Organize_args();
        }
        private void CI8_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[89]);
            if (encoding == 9)
                selected_encoding(ci8_ck);
            else
                hover_encoding(ci8_ck);
        }
        private void CI8_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (encoding == 9)
                checked_encoding(ci8_ck);
            else
                unchecked_encoding(ci8_ck);
        }
        private void CI14X2_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            Hide_encoding(encoding);
            selected_encoding(ci14x2_ck);
            encoding = 10; // CI14X2
            Check_run();
            View_ci14x2();
            Organize_args();
        }
        private void CI14X2_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[90]);
            if (encoding == 10)
                selected_encoding(ci14x2_ck);
            else
                hover_encoding(ci14x2_ck);
        }
        private void CI14X2_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (encoding == 10)
                checked_encoding(ci14x2_ck);
            else
                unchecked_encoding(ci14x2_ck);
        }
        private void CMPR_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            Hide_encoding(encoding);
            selected_encoding(cmpr_ck);
            encoding = 14; // CMPR
            View_cmpr();
            Organize_args();
        }
        private void CMPR_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[91]);
            if (encoding == 14)
                selected_encoding(cmpr_ck);
            else
                hover_encoding(cmpr_ck);
        }
        private void CMPR_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (encoding == 14)
                checked_encoding(cmpr_ck);
            else
                unchecked_encoding(cmpr_ck);
        }
        private void Cie_601_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            Hide_algorithm(algorithm);
            selected_algorithm(cie_601_ck);
            algorithm = 0; // Cie_601
            Organize_args();
        }
        private void Cie_601_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[92]);
            if (algorithm == 0)
                selected_algorithm(cie_601_ck);
            else
                hover_algorithm(cie_601_ck);
        }
        private void Cie_601_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (algorithm == 0)
                checked_algorithm(cie_601_ck);
            else
                unchecked_algorithm(cie_601_ck);
        }
        private void Cie_709_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            Hide_algorithm(algorithm);
            selected_algorithm(cie_709_ck);
            algorithm = 1; // Cie_709
            Organize_args();
        }
        private void Cie_709_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[93]);
            if (algorithm == 1)
                selected_algorithm(cie_709_ck);
            else
                hover_algorithm(cie_709_ck);
        }
        private void Cie_709_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (algorithm == 1)
                checked_algorithm(cie_709_ck);
            else
                unchecked_algorithm(cie_709_ck);
        }
        private void Custom_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            Hide_algorithm(algorithm);
            selected_algorithm(custom_ck);
            algorithm = 2; // Custom
            Organize_args();
        }
        private void Custom_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[94]);
            if (algorithm == 2)
                selected_algorithm(custom_ck);
            else
                hover_algorithm(custom_ck);
        }
        private void Custom_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (algorithm == 2)
                checked_algorithm(custom_ck);
            else
                unchecked_algorithm(custom_ck);
        }
        private void No_gradient_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            Hide_algorithm(algorithm);
            selected_algorithm(no_gradient_ck);
            algorithm = 3; // No_gradient
            Organize_args();
        }
        private void No_gradient_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[95]);
            if (algorithm == 3)
                selected_algorithm(no_gradient_ck);
            else
                hover_algorithm(no_gradient_ck);
        }
        private void No_gradient_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (algorithm == 3)
                checked_algorithm(no_gradient_ck);
            else
                unchecked_algorithm(no_gradient_ck);
        }
        private void No_alpha_Click(object sender, EventArgs e)
        {
            unchecked_alpha(alpha_ck_array[alpha]);
            selected_alpha(no_alpha_ck);
            alpha = 0; // No_alpha
            Organize_args();
        }
        private void No_alpha_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[96]);
            if (alpha == 0)
                selected_alpha(no_alpha_ck);
            else
                hover_alpha(no_alpha_ck);
        }
        private void No_alpha_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (alpha == 0)
                checked_alpha(no_alpha_ck);
            else
                unchecked_alpha(no_alpha_ck);
        }
        private void Alpha_Click(object sender, EventArgs e)
        {
            unchecked_alpha(alpha_ck_array[alpha]);
            selected_alpha(alpha_ck);
            alpha = 1; // Alpha
            Organize_args();
        }
        private void Alpha_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[97]);
            if (alpha == 1)
                selected_alpha(alpha_ck);
            else
                hover_alpha(alpha_ck);
        }
        private void Alpha_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (alpha == 1)
                checked_alpha(alpha_ck);
            else
                unchecked_alpha(alpha_ck);
        }
        private void Mix_Click(object sender, EventArgs e)
        {
            unchecked_alpha(alpha_ck_array[alpha]);
            selected_alpha(mix_ck);
            alpha = 2; // Mix
            Organize_args();
        }
        private void Mix_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[98]);
            if (alpha == 2)
                selected_alpha(mix_ck);
            else
                hover_alpha(mix_ck);
        }
        private void Mix_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (alpha == 2)
                checked_alpha(mix_ck);
            else
                unchecked_alpha(mix_ck);
        }
        private void WrapS_Clamp_Click(object sender, EventArgs e)
        {
            unchecked_WrapS(WrapS_ck[WrapS]);
            selected_WrapS(Sclamp_ck);
            WrapS = 0; // Clamp
            Organize_args();
        }
        private void WrapS_Clamp_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[99]);
            if (WrapS == 0)
                selected_WrapS(Sclamp_ck);
            else
                hover_WrapS(Sclamp_ck);
        }
        private void WrapS_Clamp_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (WrapS == 0)
                checked_WrapS(Sclamp_ck);
            else
                unchecked_WrapS(Sclamp_ck);
        }
        private void WrapS_Repeat_Click(object sender, EventArgs e)
        {
            unchecked_WrapS(WrapS_ck[WrapS]);
            selected_WrapS(Srepeat_ck);
            WrapS = 1; // Repeat
            Organize_args();
        }
        private void WrapS_Repeat_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[100]);
            if (WrapS == 1)
                selected_WrapS(Srepeat_ck);
            else
                hover_WrapS(Srepeat_ck);
        }
        private void WrapS_Repeat_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (WrapS == 1)
                checked_WrapS(Srepeat_ck);
            else
                unchecked_WrapS(Srepeat_ck);
        }
        private void WrapS_Mirror_Click(object sender, EventArgs e)
        {
            unchecked_WrapS(WrapS_ck[WrapS]);
            selected_WrapS(Smirror_ck);
            WrapS = 2; // Mirror
            Organize_args();
        }
        private void WrapS_Mirror_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[101]);
            if (WrapS == 2)
                selected_WrapS(Smirror_ck);
            else
                hover_WrapS(Smirror_ck);
        }
        private void WrapS_Mirror_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (WrapS == 2)
                checked_WrapS(Smirror_ck);
            else
                unchecked_WrapS(Smirror_ck);
        }
        private void WrapT_Clamp_Click(object sender, EventArgs e)
        {
            unchecked_WrapT(WrapT_ck[WrapT]);
            selected_WrapT(Tclamp_ck);
            WrapT = 0; // Clamp
            Organize_args();
        }
        private void WrapT_Clamp_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[102]);
            if (WrapT == 0)
                selected_WrapT(Tclamp_ck);
            else
                hover_WrapT(Tclamp_ck);
        }
        private void WrapT_Clamp_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (WrapT == 0)
                checked_WrapT(Tclamp_ck);
            else
                unchecked_WrapT(Tclamp_ck);
        }
        private void WrapT_Repeat_Click(object sender, EventArgs e)
        {
            unchecked_WrapT(WrapT_ck[WrapT]);
            selected_WrapT(Trepeat_ck);
            WrapT = 1; // Repeat
            Organize_args();
        }
        private void WrapT_Repeat_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[103]);
            if (WrapT == 1)
                selected_WrapT(Trepeat_ck);
            else
                hover_WrapT(Trepeat_ck);
        }
        private void WrapT_Repeat_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (WrapT == 1)
                checked_WrapT(Trepeat_ck);
            else
                unchecked_WrapT(Trepeat_ck);
        }
        private void WrapT_Mirror_Click(object sender, EventArgs e)
        {
            unchecked_WrapT(WrapT_ck[WrapT]);
            selected_WrapT(Tmirror_ck);
            WrapT = 2; // Mirror
            Organize_args();
        }
        private void WrapT_Mirror_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[104]);
            if (WrapT == 2)
                selected_WrapT(Tmirror_ck);
            else
                hover_WrapT(Tmirror_ck);
        }
        private void WrapT_Mirror_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (WrapT == 2)
                checked_WrapT(Tmirror_ck);
            else
                unchecked_WrapT(Tmirror_ck);
        }
        private void Minification_Nearest_Neighbour_Click(object sender, EventArgs e)
        {
            unchecked_Minification(minification_ck[minification_filter]);
            selected_Minification(min_nearest_neighbour_ck);
            minification_filter = 0; // Nearest_Neighbour
            Organize_args();
        }
        private void Minification_Nearest_Neighbour_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[105]);
            if (minification_filter == 0)
                selected_Minification(min_nearest_neighbour_ck);
            else
                hover_Minification(min_nearest_neighbour_ck);
        }
        private void Minification_Nearest_Neighbour_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (minification_filter == 0)
                checked_Minification(min_nearest_neighbour_ck);
            else
                unchecked_Minification(min_nearest_neighbour_ck);
        }
        private void Minification_Linear_Click(object sender, EventArgs e)
        {
            unchecked_Minification(minification_ck[minification_filter]);
            selected_Minification(min_linear_ck);
            minification_filter = 1; // Linear
            Organize_args();
        }
        private void Minification_Linear_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[106]);
            if (minification_filter == 1)
                selected_Minification(min_linear_ck);
            else
                hover_Minification(min_linear_ck);
        }
        private void Minification_Linear_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (minification_filter == 1)
                checked_Minification(min_linear_ck);
            else
                unchecked_Minification(min_linear_ck);
        }
        private void Minification_NearestMipmapNearest_Click(object sender, EventArgs e)
        {
            unchecked_Minification(minification_ck[minification_filter]);
            selected_Minification(min_nearestmipmapnearest_ck);
            minification_filter = 2; // NearestMipmapNearest
            Organize_args();
        }
        private void Minification_NearestMipmapNearest_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[107]);
            if (minification_filter == 2)
                selected_Minification(min_nearestmipmapnearest_ck);
            else
                hover_Minification(min_nearestmipmapnearest_ck);
        }
        private void Minification_NearestMipmapNearest_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (minification_filter == 2)
                checked_Minification(min_nearestmipmapnearest_ck);
            else
                unchecked_Minification(min_nearestmipmapnearest_ck);
        }
        private void Minification_NearestMipmapLinear_Click(object sender, EventArgs e)
        {
            unchecked_Minification(minification_ck[minification_filter]);
            selected_Minification(min_nearestmipmaplinear_ck);
            minification_filter = 3; // NearestMipmapLinear
            Organize_args();
        }
        private void Minification_NearestMipmapLinear_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[108]);
            if (minification_filter == 3)
                selected_Minification(min_nearestmipmaplinear_ck);
            else
                hover_Minification(min_nearestmipmaplinear_ck);
        }
        private void Minification_NearestMipmapLinear_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (minification_filter == 3)
                checked_Minification(min_nearestmipmaplinear_ck);
            else
                unchecked_Minification(min_nearestmipmaplinear_ck);
        }
        private void Minification_LinearMipmapNearest_Click(object sender, EventArgs e)
        {
            unchecked_Minification(minification_ck[minification_filter]);
            selected_Minification(min_linearmipmapnearest_ck);
            minification_filter = 4; // LinearMipmapNearest
            Organize_args();
        }
        private void Minification_LinearMipmapNearest_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[109]);
            if (minification_filter == 4)
                selected_Minification(min_linearmipmapnearest_ck);
            else
                hover_Minification(min_linearmipmapnearest_ck);
        }
        private void Minification_LinearMipmapNearest_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (minification_filter == 4)
                checked_Minification(min_linearmipmapnearest_ck);
            else
                unchecked_Minification(min_linearmipmapnearest_ck);
        }
        private void Minification_LinearMipmapLinear_Click(object sender, EventArgs e)
        {
            unchecked_Minification(minification_ck[minification_filter]);
            selected_Minification(min_linearmipmaplinear_ck);
            minification_filter = 5; // LinearMipmapLinear
            Organize_args();
        }
        private void Minification_LinearMipmapLinear_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[110]);
            if (minification_filter == 5)
                selected_Minification(min_linearmipmaplinear_ck);
            else
                hover_Minification(min_linearmipmaplinear_ck);
        }
        private void Minification_LinearMipmapLinear_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (minification_filter == 5)
                checked_Minification(min_linearmipmaplinear_ck);
            else
                unchecked_Minification(min_linearmipmaplinear_ck);
        }
        private void Magnification_Nearest_Neighbour_Click(object sender, EventArgs e)
        {
            unchecked_Magnification(magnification_ck[magnification_filter]);
            selected_Magnification(mag_nearest_neighbour_ck);
            magnification_filter = 0; // Mag_Nearest_Neighbour
            Organize_args();
        }
        private void Magnification_Nearest_Neighbour_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[111]);
            if (magnification_filter == 0)
                selected_Magnification(mag_nearest_neighbour_ck);
            else
                hover_Magnification(mag_nearest_neighbour_ck);
        }
        private void Magnification_Nearest_Neighbour_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (magnification_filter == 0)
                checked_Magnification(mag_nearest_neighbour_ck);
            else
                unchecked_Magnification(mag_nearest_neighbour_ck);
        }
        private void Magnification_Linear_Click(object sender, EventArgs e)
        {
            unchecked_Magnification(magnification_ck[magnification_filter]);
            selected_Magnification(mag_linear_ck);
            magnification_filter = 1; // Mag_Linear
            Organize_args();
        }
        private void Magnification_Linear_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[112]);
            if (magnification_filter == 1)
                selected_Magnification(mag_linear_ck);
            else
                hover_Magnification(mag_linear_ck);
        }
        private void Magnification_Linear_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (magnification_filter == 1)
                checked_Magnification(mag_linear_ck);
            else
                unchecked_Magnification(mag_linear_ck);
        }
        private void Magnification_NearestMipmapNearest_Click(object sender, EventArgs e)
        {
            unchecked_Magnification(magnification_ck[magnification_filter]);
            selected_Magnification(mag_nearestmipmapnearest_ck);
            magnification_filter = 2; // Mag_NearestMipmapNearest
            Organize_args();
        }
        private void Magnification_NearestMipmapNearest_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[113]);
            if (magnification_filter == 2)
                selected_Magnification(mag_nearestmipmapnearest_ck);
            else
                hover_Magnification(mag_nearestmipmapnearest_ck);
        }
        private void Magnification_NearestMipmapNearest_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (magnification_filter == 2)
                checked_Magnification(mag_nearestmipmapnearest_ck);
            else
                unchecked_Magnification(mag_nearestmipmapnearest_ck);
        }
        private void Magnification_NearestMipmapLinear_Click(object sender, EventArgs e)
        {
            unchecked_Magnification(magnification_ck[magnification_filter]);
            selected_Magnification(mag_nearestmipmaplinear_ck);
            magnification_filter = 3; // Mag_NearestMipmapLinear
            Organize_args();
        }
        private void Magnification_NearestMipmapLinear_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[114]);
            if (magnification_filter == 3)
                selected_Magnification(mag_nearestmipmaplinear_ck);
            else
                hover_Magnification(mag_nearestmipmaplinear_ck);
        }
        private void Magnification_NearestMipmapLinear_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (magnification_filter == 3)
                checked_Magnification(mag_nearestmipmaplinear_ck);
            else
                unchecked_Magnification(mag_nearestmipmaplinear_ck);
        }
        private void Magnification_LinearMipmapNearest_Click(object sender, EventArgs e)
        {
            unchecked_Magnification(magnification_ck[magnification_filter]);
            selected_Magnification(mag_linearmipmapnearest_ck);
            magnification_filter = 4; // Mag_LinearMipmapNearest
            Organize_args();
        }
        private void Magnification_LinearMipmapNearest_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[115]);
            if (magnification_filter == 4)
                selected_Magnification(mag_linearmipmapnearest_ck);
            else
                hover_Magnification(mag_linearmipmapnearest_ck);
        }
        private void Magnification_LinearMipmapNearest_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (magnification_filter == 4)
                checked_Magnification(mag_linearmipmapnearest_ck);
            else
                unchecked_Magnification(mag_linearmipmapnearest_ck);
        }
        private void Magnification_LinearMipmapLinear_Click(object sender, EventArgs e)
        {
            unchecked_Magnification(magnification_ck[magnification_filter]);
            selected_Magnification(mag_linearmipmaplinear_ck);
            magnification_filter = 5; // Mag_LinearMipmapLinear
            Organize_args();
        }
        private void Magnification_LinearMipmapLinear_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[116]);
            if (magnification_filter == 5)
                selected_Magnification(mag_linearmipmaplinear_ck);
            else
                hover_Magnification(mag_linearmipmaplinear_ck);
        }
        private void Magnification_LinearMipmapLinear_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (magnification_filter == 5)
                checked_Magnification(mag_linearmipmaplinear_ck);
            else
                unchecked_Magnification(mag_linearmipmaplinear_ck);
        }
        private void R_R_Click(object sender, EventArgs e)
        {
            switch (r)
            {
                case 0:
                    unchecked_R(r_ck[r]);
                    break;
                case 1:
                    unchecked_G(r_ck[r]);
                    break;
                case 2:
                    unchecked_B(r_ck[r]);
                    break;
                case 3:
                    unchecked_A(r_ck[r]);
                    break;
            }
            selected_R(r_r_ck);
            r = 0; // Red channel set to R
            Organize_args();
        }
        private void R_R_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[117]);
            if (r == 0)
                selected_R(r_r_ck);
            else
                hover_R(r_r_ck);
        }
        private void R_R_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (r == 0)
                checked_R(r_r_ck);
            else
                unchecked_R(r_r_ck);
        }
        private void R_G_Click(object sender, EventArgs e)
        {
            switch (r)
            {
                case 0:
                    unchecked_R(r_ck[r]);
                    break;
                case 1:
                    unchecked_G(r_ck[r]);
                    break;
                case 2:
                    unchecked_B(r_ck[r]);
                    break;
                case 3:
                    unchecked_A(r_ck[r]);
                    break;
            }
            selected_G(r_g_ck);
            r = 1; // Red channel set to G
            Organize_args();
        }
        private void R_G_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[117]);
            if (r == 1)
                selected_G(r_g_ck);
            else
                hover_G(r_g_ck);
        }
        private void R_G_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (r == 1)
                checked_G(r_g_ck);
            else
                unchecked_G(r_g_ck);
        }
        private void R_B_Click(object sender, EventArgs e)
        {
            switch (r)
            {
                case 0:
                    unchecked_R(r_ck[r]);
                    break;
                case 1:
                    unchecked_G(r_ck[r]);
                    break;
                case 2:
                    unchecked_B(r_ck[r]);
                    break;
                case 3:
                    unchecked_A(r_ck[r]);
                    break;
            }
            selected_B(r_b_ck);
            r = 2; // Red channel set to B
            Organize_args();
        }
        private void R_B_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[117]);
            if (r == 2)
                selected_B(r_b_ck);
            else
                hover_B(r_b_ck);
        }
        private void R_B_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (r == 2)
                checked_B(r_b_ck);
            else
                unchecked_B(r_b_ck);
        }
        private void R_A_Click(object sender, EventArgs e)
        {
            switch (r)
            {
                case 0:
                    unchecked_R(r_ck[r]);
                    break;
                case 1:
                    unchecked_G(r_ck[r]);
                    break;
                case 2:
                    unchecked_B(r_ck[r]);
                    break;
                case 3:
                    unchecked_A(r_ck[r]);
                    break;
            }
            selected_A(r_a_ck);
            r = 3; // Red channel set to A
            Organize_args();
        }
        private void R_A_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[117]);
            if (r == 3)
                selected_A(r_a_ck);
            else
                hover_A(r_a_ck);
        }
        private void R_A_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (r == 3)
                checked_A(r_a_ck);
            else
                unchecked_A(r_a_ck);
        }
        private void G_R_Click(object sender, EventArgs e)
        {
            switch (g)
            {
                case 0:
                    unchecked_R(g_ck[g]);
                    break;
                case 1:
                    unchecked_G(g_ck[g]);
                    break;
                case 2:
                    unchecked_B(g_ck[g]);
                    break;
                case 3:
                    unchecked_A(g_ck[g]);
                    break;
            }
            selected_R(g_r_ck);
            g = 0; // Green channel set to R
            Organize_args();
        }
        private void G_R_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[118]);
            if (g == 0)
                selected_R(g_r_ck);
            else
                hover_R(g_r_ck);
        }
        private void G_R_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (g == 0)
                checked_R(g_r_ck);
            else
                unchecked_R(g_r_ck);
        }
        private void G_G_Click(object sender, EventArgs e)
        {
            switch (g)
            {
                case 0:
                    unchecked_R(g_ck[g]);
                    break;
                case 1:
                    unchecked_G(g_ck[g]);
                    break;
                case 2:
                    unchecked_B(g_ck[g]);
                    break;
                case 3:
                    unchecked_A(g_ck[g]);
                    break;
            }
            selected_G(g_g_ck);
            g = 1; // Green channel set to G
            Organize_args();
        }
        private void G_G_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[118]);
            if (g == 1)
                selected_G(g_g_ck);
            else
                hover_G(g_g_ck);
        }
        private void G_G_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (g == 1)
                checked_G(g_g_ck);
            else
                unchecked_G(g_g_ck);
        }
        private void G_B_Click(object sender, EventArgs e)
        {
            switch (g)
            {
                case 0:
                    unchecked_R(g_ck[g]);
                    break;
                case 1:
                    unchecked_G(g_ck[g]);
                    break;
                case 2:
                    unchecked_B(g_ck[g]);
                    break;
                case 3:
                    unchecked_A(g_ck[g]);
                    break;
            }
            selected_B(g_b_ck);
            g = 2; // Green channel set to B
            Organize_args();
        }
        private void G_B_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[118]);
            if (g == 2)
                selected_B(g_b_ck);
            else
                hover_B(g_b_ck);
        }
        private void G_B_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (g == 2)
                checked_B(g_b_ck);
            else
                unchecked_B(g_b_ck);
        }
        private void G_A_Click(object sender, EventArgs e)
        {
            switch (g)
            {
                case 0:
                    unchecked_R(g_ck[g]);
                    break;
                case 1:
                    unchecked_G(g_ck[g]);
                    break;
                case 2:
                    unchecked_B(g_ck[g]);
                    break;
                case 3:
                    unchecked_A(g_ck[g]);
                    break;
            }
            selected_A(g_a_ck);
            g = 3; // Green channel set to A
            Organize_args();
        }
        private void G_A_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[118]);
            if (g == 3)
                selected_A(g_a_ck);
            else
                hover_A(g_a_ck);
        }
        private void G_A_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (g == 3)
                checked_A(g_a_ck);
            else
                unchecked_A(g_a_ck);
        }
        private void B_R_Click(object sender, EventArgs e)
        {
            switch (b)
            {
                case 0:
                    unchecked_R(b_ck[b]);
                    break;
                case 1:
                    unchecked_G(b_ck[b]);
                    break;
                case 2:
                    unchecked_B(b_ck[b]);
                    break;
                case 3:
                    unchecked_A(b_ck[b]);
                    break;
            }
            selected_R(b_r_ck);
            b = 0; // Blue channel set to R
            Organize_args();
        }
        private void B_R_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[119]);
            if (b == 0)
                selected_R(b_r_ck);
            else
                hover_R(b_r_ck);
        }
        private void B_R_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (b == 0)
                checked_R(b_r_ck);
            else
                unchecked_R(b_r_ck);
        }
        private void B_G_Click(object sender, EventArgs e)
        {
            switch (b)
            {
                case 0:
                    unchecked_R(b_ck[b]);
                    break;
                case 1:
                    unchecked_G(b_ck[b]);
                    break;
                case 2:
                    unchecked_B(b_ck[b]);
                    break;
                case 3:
                    unchecked_A(b_ck[b]);
                    break;
            }
            selected_G(b_g_ck);
            b = 1; // Blue channel set to G
            Organize_args();
        }
        private void B_G_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[119]);
            if (b == 1)
                selected_G(b_g_ck);
            else
                hover_G(b_g_ck);
        }
        private void B_G_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (b == 1)
                checked_G(b_g_ck);
            else
                unchecked_G(b_g_ck);
        }
        private void B_B_Click(object sender, EventArgs e)
        {
            switch (b)
            {
                case 0:
                    unchecked_R(b_ck[b]);
                    break;
                case 1:
                    unchecked_G(b_ck[b]);
                    break;
                case 2:
                    unchecked_B(b_ck[b]);
                    break;
                case 3:
                    unchecked_A(b_ck[b]);
                    break;
            }
            selected_B(b_b_ck);
            b = 2; // Blue channel set to B
            Organize_args();
        }
        private void B_B_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[119]);
            if (b == 2)
                selected_B(b_b_ck);
            else
                hover_B(b_b_ck);
        }
        private void B_B_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (b == 2)
                checked_B(b_b_ck);
            else
                unchecked_B(b_b_ck);
        }
        private void B_A_Click(object sender, EventArgs e)
        {
            switch (b)
            {
                case 0:
                    unchecked_R(b_ck[b]);
                    break;
                case 1:
                    unchecked_G(b_ck[b]);
                    break;
                case 2:
                    unchecked_B(b_ck[b]);
                    break;
                case 3:
                    unchecked_A(b_ck[b]);
                    break;
            }
            selected_A(b_a_ck);
            b = 3; // Blue channel set to A
            Organize_args();
        }
        private void B_A_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[119]);
            if (b == 3)
                selected_A(b_a_ck);
            else
                hover_A(b_a_ck);
        }
        private void B_A_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (b == 3)
                checked_A(b_a_ck);
            else
                unchecked_A(b_a_ck);
        }
        private void A_R_Click(object sender, EventArgs e)
        {
            switch (a)
            {
                case 0:
                    unchecked_R(a_ck[a]);
                    break;
                case 1:
                    unchecked_G(a_ck[a]);
                    break;
                case 2:
                    unchecked_B(a_ck[a]);
                    break;
                case 3:
                    unchecked_A(a_ck[a]);
                    break;
            }
            selected_R(a_r_ck);
            a = 0; // Alpha channel set to R
            Organize_args();
        }
        private void A_R_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[120]);
            if (a == 0)
                selected_R(a_r_ck);
            else
                hover_R(a_r_ck);
        }
        private void A_R_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (a == 0)
                checked_R(a_r_ck);
            else
                unchecked_R(a_r_ck);
        }
        private void A_G_Click(object sender, EventArgs e)
        {
            switch (a)
            {
                case 0:
                    unchecked_R(a_ck[a]);
                    break;
                case 1:
                    unchecked_G(a_ck[a]);
                    break;
                case 2:
                    unchecked_B(a_ck[a]);
                    break;
                case 3:
                    unchecked_A(a_ck[a]);
                    break;
            }
            selected_G(a_g_ck);
            a = 1; // Alpha channel set to G
            Organize_args();
        }
        private void A_G_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[120]);
            if (a == 1)
                selected_G(a_g_ck);
            else
                hover_G(a_g_ck);
        }
        private void A_G_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (a == 1)
                checked_G(a_g_ck);
            else
                unchecked_G(a_g_ck);
        }
        private void A_B_Click(object sender, EventArgs e)
        {
            switch (a)
            {
                case 0:
                    unchecked_R(a_ck[a]);
                    break;
                case 1:
                    unchecked_G(a_ck[a]);
                    break;
                case 2:
                    unchecked_B(a_ck[a]);
                    break;
                case 3:
                    unchecked_A(a_ck[a]);
                    break;
            }
            selected_B(a_b_ck);
            a = 2; // Alpha channel set to B
            Organize_args();
        }
        private void A_B_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[120]);
            if (a == 2)
                selected_B(a_b_ck);
            else
                hover_B(a_b_ck);
        }
        private void A_B_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (a == 2)
                checked_B(a_b_ck);
            else
                unchecked_B(a_b_ck);
        }
        private void A_A_Click(object sender, EventArgs e)
        {
            switch (a)
            {
                case 0:
                    unchecked_R(a_ck[a]);
                    break;
                case 1:
                    unchecked_G(a_ck[a]);
                    break;
                case 2:
                    unchecked_B(a_ck[a]);
                    break;
                case 3:
                    unchecked_A(a_ck[a]);
                    break;
            }
            selected_A(a_a_ck);
            a = 3; // Alpha channel set to A
            Organize_args();
        }
        private void A_A_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[120]);
            if (a == 3)
                selected_A(a_a_ck);
            else
                hover_A(a_a_ck);
        }
        private void A_A_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (a == 3)
                checked_A(a_a_ck);
            else
                unchecked_A(a_a_ck);
        }
        private void All_Click(object sender, EventArgs e)
        {
            switch (layout)
            {
                case 0:
                    unchecked_All();
                    break;
                case 1:
                    unchecked_Auto();
                    break;
                case 2:
                    unchecked_Preview();
                    break;
                case 3:
                    unchecked_Paint();
                    break;
            }
            selected_All();
            layout = 0;
        }
        private void All_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[121]);
            if (layout == 0)
                selected_All();
            else
                hover_All();
        }
        private void All_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (layout == 0)
                checked_All();
            else
                unchecked_All();
        }
        private void checked_All()
        {
            all_ck.BackgroundImage = all_on;
        }
        private void unchecked_All()
        {
            all_ck.BackgroundImage = all_off;
        }
        private void hover_All()
        {
            all_ck.BackgroundImage = all_hover;
        }
        private void selected_All()
        {
            all_ck.BackgroundImage = all_selected;
        }
        private void Auto_Click(object sender, EventArgs e)
        {
            switch (layout)
            {
                case 0:
                    unchecked_All();
                    break;
                case 1:
                    unchecked_Auto();
                    break;
                case 2:
                    unchecked_Preview();
                    break;
                case 3:
                    unchecked_Paint();
                    break;
            }
            selected_Auto();
            layout = 1;
        }
        private void Auto_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[122]);
            if (layout == 1)
                selected_Auto();
            else
                hover_Auto();
        }
        private void Auto_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (layout == 1)
                checked_Auto();
            else
                unchecked_Auto();
        }
        private void checked_Auto()
        {
            auto_ck.BackgroundImage = auto_on;
        }
        private void unchecked_Auto()
        {
            auto_ck.BackgroundImage = auto_off;
        }
        private void hover_Auto()
        {
            auto_ck.BackgroundImage = auto_hover;
        }
        private void selected_Auto()
        {
            auto_ck.BackgroundImage = auto_selected;
        }
        private void Preview_Click(object sender, EventArgs e)
        {
            switch (layout)
            {
                case 0:
                    unchecked_All();
                    break;
                case 1:
                    unchecked_Auto();
                    break;
                case 2:
                    unchecked_Preview();
                    break;
                case 3:
                    unchecked_Paint();
                    break;
            }
            selected_Preview();
            layout = 2;
        }
        private void Preview_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[123]);
            if (layout == 2)
                selected_Preview();
            else
                hover_Preview();
        }
        private void Preview_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (layout == 2)
                checked_Preview();
            else
                unchecked_Preview();
        }
        private void checked_Preview()
        {
            preview_ck.BackgroundImage = preview_on;
        }
        private void unchecked_Preview()
        {
            preview_ck.BackgroundImage = preview_off;
        }
        private void hover_Preview()
        {
            preview_ck.BackgroundImage = preview_hover;
        }
        private void selected_Preview()
        {
            preview_ck.BackgroundImage = preview_selected;
        }
        private void Paint_Click(object sender, EventArgs e)
        {
            switch (layout)
            {
                case 0:
                    unchecked_All();
                    break;
                case 1:
                    unchecked_Auto();
                    break;
                case 2:
                    unchecked_Preview();
                    break;
                case 3:
                    unchecked_Paint();
                    break;
            }
            selected_Paint();
            layout = 3;
        }
        private void Paint_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[124]);
            if (layout == 3)
                selected_Paint();
            else
                hover_Paint();
        }
        private void Paint_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (layout == 3)
                checked_Paint();
            else
                unchecked_Paint();
        }
        private void checked_Paint()
        {
            paint_ck.BackgroundImage = paint_on;
        }
        private void unchecked_Paint()
        {
            paint_ck.BackgroundImage = paint_off;
        }
        private void hover_Paint()
        {
            paint_ck.BackgroundImage = paint_hover;
        }
        private void selected_Paint()
        {
            paint_ck.BackgroundImage = paint_selected;
        }
        private void Minimized_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void Minimized_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[125]);
            banner_minus_ck.BackgroundImage = minimized_hover;
        }
        private void Minimized_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            banner_minus_ck.BackgroundImage = minimized;
        }
        private void Maximized_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                banner_5_ck.BackgroundImage = maximized_hover;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                banner_5_ck.BackgroundImage = maximized_selected;
            }
        }
        private void Maximized_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[126]);
            if (this.WindowState == FormWindowState.Maximized)
                banner_5_ck.BackgroundImage = maximized_selected;
            else
                banner_5_ck.BackgroundImage = maximized_hover;
        }
        private void Maximized_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (this.WindowState == FormWindowState.Maximized)
                banner_5_ck.BackgroundImage = maximized_on;
            else
                banner_5_ck.BackgroundImage = maximized_off;
        }
        private void Close_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void Close_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[127]);
            banner_x_ck.BackgroundImage = close_hover;
        }
        private void Close_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            banner_x_ck.BackgroundImage = close;
        }
        private void Left_Click(object sender, EventArgs e)
        {
            switch (arrow)
            {
                case 4:
                    unchecked_Left();
                    break;
                case 7:
                    unchecked_Top_left();
                    break;
                case 8:
                    unchecked_Top();
                    break;
                case 9:
                    unchecked_Top_right();
                    break;
                case 6:
                    unchecked_Right();
                    break;
                case 3:
                    unchecked_Bottom_right();
                    break;
                case 2:
                    unchecked_Bottom();
                    break;
                case 1:
                    unchecked_Bottom_left();
                    break;
            }
            selected_Left();
            arrow = 4;
        }
        private void Left_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[128]);
            if (arrow == 4)
                selected_Left();
            else
                hover_Left();
        }
        private void Left_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 4)
                checked_Left();
            else
                unchecked_Left();
        }
        private void checked_Left()
        {
            banner_4_ck.BackgroundImage = left_on;
        }
        private void unchecked_Left()
        {
            banner_4_ck.BackgroundImage = left_off;
        }
        private void hover_Left()
        {
            banner_4_ck.BackgroundImage = left_hover;
        }
        private void selected_Left()
        {
            banner_4_ck.BackgroundImage = left_selected;
        }
        private void Top_left_Click(object sender, EventArgs e)
        {
            switch (arrow)
            {
                case 4:
                    unchecked_Left();
                    break;
                case 7:
                    unchecked_Top_left();
                    break;
                case 8:
                    unchecked_Top();
                    break;
                case 9:
                    unchecked_Top_right();
                    break;
                case 6:
                    unchecked_Right();
                    break;
                case 3:
                    unchecked_Bottom_right();
                    break;
                case 2:
                    unchecked_Bottom();
                    break;
                case 1:
                    unchecked_Bottom_left();
                    break;
            }
            selected_Top_left();
            arrow = 7;
        }
        private void Top_left_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[129]);
            if (arrow == 7)
                selected_Top_left();
            else
                hover_Top_left();
        }
        private void Top_left_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 7)
                checked_Top_left();
            else
                unchecked_Top_left();
        }
        private void checked_Top_left()
        {
            banner_7_ck.BackgroundImage = top_left_on;
        }
        private void unchecked_Top_left()
        {
            banner_7_ck.BackgroundImage = top_left_off;
        }
        private void hover_Top_left()
        {
            banner_7_ck.BackgroundImage = top_left_hover;
        }
        private void selected_Top_left()
        {
            banner_7_ck.BackgroundImage = top_left_selected;
        }
        private void Top_Click(object sender, EventArgs e)
        {
            switch (arrow)
            {
                case 4:
                    unchecked_Left();
                    break;
                case 7:
                    unchecked_Top_left();
                    break;
                case 8:
                    unchecked_Top();
                    break;
                case 9:
                    unchecked_Top_right();
                    break;
                case 6:
                    unchecked_Right();
                    break;
                case 3:
                    unchecked_Bottom_right();
                    break;
                case 2:
                    unchecked_Bottom();
                    break;
                case 1:
                    unchecked_Bottom_left();
                    break;
            }
            selected_Top();
            arrow = 8;
        }
        private void Top_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[130]);
            if (arrow == 8)
                selected_Top();
            else
                hover_Top();
        }
        private void Top_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 8)
                checked_Top();
            else
                unchecked_Top();
        }
        private void checked_Top()
        {
            banner_8_ck.BackgroundImage = top_on;
        }
        private void unchecked_Top()
        {
            banner_8_ck.BackgroundImage = top_off;
        }
        private void hover_Top()
        {
            banner_8_ck.BackgroundImage = top_hover;
        }
        private void selected_Top()
        {
            banner_8_ck.BackgroundImage = top_selected;
        }
        private void Top_right_Click(object sender, EventArgs e)
        {
            switch (arrow)
            {
                case 4:
                    unchecked_Left();
                    break;
                case 7:
                    unchecked_Top_left();
                    break;
                case 8:
                    unchecked_Top();
                    break;
                case 9:
                    unchecked_Top_right();
                    break;
                case 6:
                    unchecked_Right();
                    break;
                case 3:
                    unchecked_Bottom_right();
                    break;
                case 2:
                    unchecked_Bottom();
                    break;
                case 1:
                    unchecked_Bottom_left();
                    break;
            }
            selected_Top_right();
            arrow = 9;
        }
        private void Top_right_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[131]);
            if (arrow == 9)
                selected_Top_right();
            else
                hover_Top_right();
        }
        private void Top_right_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 9)
                checked_Top_right();
            else
                unchecked_Top_right();
        }
        private void checked_Top_right()
        {
            banner_9_ck.BackgroundImage = top_right_on;
        }
        private void unchecked_Top_right()
        {
            banner_9_ck.BackgroundImage = top_right_off;
        }
        private void hover_Top_right()
        {
            banner_9_ck.BackgroundImage = top_right_hover;
        }
        private void selected_Top_right()
        {
            banner_9_ck.BackgroundImage = top_right_selected;
        }
        private void Right_Click(object sender, EventArgs e)
        {
            switch (arrow)
            {
                case 4:
                    unchecked_Left();
                    break;
                case 7:
                    unchecked_Top_left();
                    break;
                case 8:
                    unchecked_Top();
                    break;
                case 9:
                    unchecked_Top_right();
                    break;
                case 6:
                    unchecked_Right();
                    break;
                case 3:
                    unchecked_Bottom_right();
                    break;
                case 2:
                    unchecked_Bottom();
                    break;
                case 1:
                    unchecked_Bottom_left();
                    break;
            }
            selected_Right();
            arrow = 6;
        }
        private void Right_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[132]);
            if (arrow == 6)
                selected_Right();
            else
                hover_Right();
        }
        private void Right_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 6)
                checked_Right();
            else
                unchecked_Right();
        }
        private void checked_Right()
        {
            banner_6_ck.BackgroundImage = right_on;
        }
        private void unchecked_Right()
        {
            banner_6_ck.BackgroundImage = right_off;
        }
        private void hover_Right()
        {
            banner_6_ck.BackgroundImage = right_hover;
        }
        private void selected_Right()
        {
            banner_6_ck.BackgroundImage = right_selected;
        }
        private void Bottom_right_Click(object sender, EventArgs e)
        {
            switch (arrow)
            {
                case 4:
                    unchecked_Left();
                    break;
                case 7:
                    unchecked_Top_left();
                    break;
                case 8:
                    unchecked_Top();
                    break;
                case 9:
                    unchecked_Top_right();
                    break;
                case 6:
                    unchecked_Right();
                    break;
                case 3:
                    unchecked_Bottom_right();
                    break;
                case 2:
                    unchecked_Bottom();
                    break;
                case 1:
                    unchecked_Bottom_left();
                    break;
            }
            selected_Bottom_right();
            arrow = 3;
        }
        private void Bottom_right_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[133]);
            if (arrow == 3)
                selected_Bottom_right();
            else
                hover_Bottom_right();
        }
        private void Bottom_right_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 3)
                checked_Bottom_right();
            else
                unchecked_Bottom_right();
        }
        private void checked_Bottom_right()
        {
            banner_3_ck.BackgroundImage = bottom_right_on;
        }
        private void unchecked_Bottom_right()
        {
            banner_3_ck.BackgroundImage = bottom_right_off;
        }
        private void hover_Bottom_right()
        {
            banner_3_ck.BackgroundImage = bottom_right_hover;
        }
        private void selected_Bottom_right()
        {
            banner_3_ck.BackgroundImage = bottom_right_selected;
        }
        private void Bottom_Click(object sender, EventArgs e)
        {
            switch (arrow)
            {
                case 4:
                    unchecked_Left();
                    break;
                case 7:
                    unchecked_Top_left();
                    break;
                case 8:
                    unchecked_Top();
                    break;
                case 9:
                    unchecked_Top_right();
                    break;
                case 6:
                    unchecked_Right();
                    break;
                case 3:
                    unchecked_Bottom_right();
                    break;
                case 2:
                    unchecked_Bottom();
                    break;
                case 1:
                    unchecked_Bottom_left();
                    break;
            }
            selected_Bottom();
            arrow = 2;
        }
        private void Bottom_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[134]);
            if (arrow == 2)
                selected_Bottom();
            else
                hover_Bottom();
        }
        private void Bottom_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 2)
                checked_Bottom();
            else
                unchecked_Bottom();
        }
        private void checked_Bottom()
        {
            banner_2_ck.BackgroundImage = bottom_on;
        }
        private void unchecked_Bottom()
        {
            banner_2_ck.BackgroundImage = bottom_off;
        }
        private void hover_Bottom()
        {
            banner_2_ck.BackgroundImage = bottom_hover;
        }
        private void selected_Bottom()
        {
            banner_2_ck.BackgroundImage = bottom_selected;
        }
        private void Bottom_left_Click(object sender, EventArgs e)
        {
            switch (arrow)
            {
                case 4:
                    unchecked_Left();
                    break;
                case 7:
                    unchecked_Top_left();
                    break;
                case 8:
                    unchecked_Top();
                    break;
                case 9:
                    unchecked_Top_right();
                    break;
                case 6:
                    unchecked_Right();
                    break;
                case 3:
                    unchecked_Bottom_right();
                    break;
                case 2:
                    unchecked_Bottom();
                    break;
                case 1:
                    unchecked_Bottom_left();
                    break;
            }
            selected_Bottom_left();
            arrow = 1;
        }
        private void Bottom_left_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[135]);
            if (arrow == 1)
                selected_Bottom_left();
            else
                hover_Bottom_left();
        }
        private void Bottom_left_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 1)
                checked_Bottom_left();
            else
                unchecked_Bottom_left();
        }
        private void checked_Bottom_left()
        {
            banner_1_ck.BackgroundImage = bottom_left_on;
        }
        private void unchecked_Bottom_left()
        {
            banner_1_ck.BackgroundImage = bottom_left_off;
        }
        private void hover_Bottom_left()
        {
            banner_1_ck.BackgroundImage = bottom_left_hover;
        }
        private void selected_Bottom_left()
        {
            banner_1_ck.BackgroundImage = bottom_left_selected;
        }
        private void input_file_Click(object sender, EventArgs e)
        {
            FileDialog dialog = new OpenFileDialog
            {
                Title = "Select a picture or a texture",
                Filter = "Picture|*.bmp;*.png;*.jfif;*.jpg;*.jpeg;*.jpg;*.ico;*.gif;*.tif;*.tiff;*.rle;*.dib|Texture|*.bti;*.tex0;*.tpl|All files (*.*)|*.*",
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                input_file_txt.Text = dialog.FileName;
                input_file = dialog.FileName;
                Check_run();
                Organize_args();
            }
        }
        private void input_file2_Click(object sender, EventArgs e)
        {
            FileDialog dialog = new OpenFileDialog
            {
                Title = "Select a palette, a bmd file, or a tpl file",
                Filter = "Palette|*.plt0;*.bmp|bmd or tpl|*.bmd;*tpl|All files (*.*)|*.*",
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                input_file2_txt.Text = dialog.FileName;
                input_file2 = dialog.FileName;
                Check_run();
                Organize_args();
            }
        }
        private void input_file_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[136]);
        }
        private void input_file_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void input_file_TextChanged(object sender, EventArgs e)
        {
            input_file = input_file_txt.Text;
            Check_run();
            Organize_args();
        }
        private void input_file2_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[137]);
        }
        private void input_file2_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void input_file2_TextChanged(object sender, EventArgs e)
        {
            input_file2 = input_file2_txt.Text;
            Check_run();
            Organize_args();
        }
        private void output_name_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[138]);
        }
        private void output_name_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void output_name_TextChanged(object sender, EventArgs e)
        {
            output_name = output_name_txt.Text;
            Check_run();
            Organize_args();
        }
        private void mipmaps_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[139]);
        }
        private void mipmaps_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void mipmaps_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(mipmaps_txt, out mipmaps, 255);
            Organize_args();
        }
        private void cmpr_max_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[140]);
        }
        private void cmpr_max_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_max_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(cmpr_max_txt, out cmpr_max, 16);
            Organize_args();
        }
        private void cmpr_min_alpha_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[141]);
        }
        private void cmpr_min_alpha_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_min_alpha_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(cmpr_min_alpha_txt, out cmpr_min_alpha, 255);
            Organize_args();
        }
        private void num_colours_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[142]);
        }
        private void num_colours_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void num_colours_TextChanged(object sender, EventArgs e)
        {
            Parse_ushort_text(num_colours_txt, out num_colours, 65535);
            Organize_args();
        }
        private void round3_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[143]);
        }
        private void round3_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void round3_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(round3_txt, out round3, 32);
            Organize_args();
        }
        private void round4_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[144]);
        }
        private void round4_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void round4_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(round4_txt, out round4, 16);
            Organize_args();
        }
        private void round5_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[145]);
        }
        private void round5_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void round5_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(round5_txt, out round5, 8);
            Organize_args();
        }
        private void round6_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[146]);
        }
        private void round6_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void round6_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(round6_txt, out round6, 4);
            Organize_args();
        }
        private void diversity_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[147]);
        }
        private void diversity_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void diversity_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(diversity_txt, out diversity, 255);
            Organize_args();
        }
        private void diversity2_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[148]);
        }
        private void diversity2_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void diversity2_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(diversity2_txt, out diversity2, 255);
            Organize_args();
        }
        private void percentage_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[149]);
        }
        private void percentage_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void percentage_TextChanged(object sender, EventArgs e)
        {
            Parse_double_text(percentage_txt, out percentage, 100.0);
            Organize_args();
        }
        private void percentage2_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[150]);
        }
        private void percentage2_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void percentage2_TextChanged(object sender, EventArgs e)
        {
            Parse_double_text(percentage2_txt, out percentage2, 100.0);
            Organize_args();
        }
        private void custom_r_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[151]);
        }
        private void custom_r_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void custom_r_TextChanged(object sender, EventArgs e)
        {
            Parse_double_text(custom_r_txt, out custom_r, 255.0);
            Organize_args();
        }
        private void custom_g_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[152]);
        }
        private void custom_g_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void custom_g_TextChanged(object sender, EventArgs e)
        {
            Parse_double_text(custom_g_txt, out custom_g, 255.0);
            Organize_args();
        }
        private void custom_b_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[153]);
        }
        private void custom_b_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void custom_b_TextChanged(object sender, EventArgs e)
        {
            Parse_double_text(custom_b_txt, out custom_b, 255.0);
            Organize_args();
        }
        private void custom_a_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[154]);
        }
        private void custom_a_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void custom_a_TextChanged(object sender, EventArgs e)
        {
            Parse_double_text(custom_a_txt, out custom_a, 255.0);
            Organize_args();
        }
        private void palette_AI8_Click(object sender, EventArgs e)
        {
            unchecked_palette(palette_ck[palette_enc]);
            Hide_encoding(palette_enc);
            selected_palette(palette_ai8_ck);
            palette_enc = 0;
            View_ai8();
            Organize_args();
        }
        private void palette_AI8_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[155]);
            if (palette_enc == 0)
                selected_palette(palette_ai8_ck);
            else
                hover_palette(palette_ai8_ck);
        }
        private void palette_AI8_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (palette_enc == 0)
                checked_palette(palette_ai8_ck);
            else
                unchecked_palette(palette_ai8_ck);
        }
        private void palette_RGB565_Click(object sender, EventArgs e)
        {
            unchecked_palette(palette_ck[palette_enc]);
            Hide_encoding(palette_enc);
            selected_palette(palette_rgb565_ck);
            palette_enc = 1;
            View_rgb565();
            Organize_args();
        }
        private void palette_RGB565_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[156]);
            if (palette_enc == 1)
                selected_palette(palette_rgb565_ck);
            else
                hover_palette(palette_rgb565_ck);
        }
        private void palette_RGB565_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (palette_enc == 1)
                checked_palette(palette_rgb565_ck);
            else
                unchecked_palette(palette_rgb565_ck);
        }
        private void palette_RGB5A3_Click(object sender, EventArgs e)
        {
            unchecked_palette(palette_ck[palette_enc]);
            Hide_encoding(palette_enc);
            selected_palette(palette_rgb5a3_ck);
            palette_enc = 2;
            View_rgb5a3();
            Organize_args();
        }
        private void palette_RGB5A3_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[157]);
            if (palette_enc == 2)
                selected_palette(palette_rgb5a3_ck);
            else
                hover_palette(palette_rgb5a3_ck);
        }
        private void palette_RGB5A3_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (palette_enc == 2)
                checked_palette(palette_rgb5a3_ck);
            else
                unchecked_palette(palette_rgb5a3_ck);
        }
        private void discord_Click(object sender, EventArgs e)
        {
            // launch webbrowser
            System.Diagnostics.Process.Start("https://discord.gg/4bpfqDJXnU");
        }
        private void discord_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[158]);
            discord_ck.BackgroundImage = discord_hover;
        }
        private void discord_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            discord_ck.BackgroundImage = discord;
        }
        private void github_Click(object sender, EventArgs e)
        {
            // launch webbrowser
            System.Diagnostics.Process.Start("https://github.com/yoshi2999/plt0/releases");
        }
        private void github_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[159]);
            github_ck.BackgroundImage = github_hover;
        }
        private void github_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            github_ck.BackgroundImage = github;
        }
        private void youtube_Click(object sender, EventArgs e)
        {
            // launch webbrowser
            System.Diagnostics.Process.Start("https://www.youtube.com/c/yoshytp");
        }
        private void youtube_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[160]);
            youtube_ck.BackgroundImage = youtube_hover;
        }
        private void youtube_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            youtube_ck.BackgroundImage = youtube;
        }
        private void version_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[161]);
            version_ck.BackgroundImage = version_hover;
        }
        private void version_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            version_ck.BackgroundImage = version;
        }
        private void cli_textbox_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[162]);
            cli_textbox_ck.BackgroundImage = cli_textbox_hover;
        }
        private void cli_textbox_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            cli_textbox_ck.BackgroundImage = cli_textbox;
        }
        private void run_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[163]);
            Check_run();
        }
        private void run_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            Check_run();
        }
        private void Output_label_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[164]);
        }
        private void Output_label_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void view_alpha_Click(object sender, EventArgs e)
        {
            if (view_alpha)
            {
                Hide_alpha();
                Category_hover(view_alpha_ck);
            }
            else
            {
                View_alpha();
                Category_selected(view_alpha_ck);
            }
        }
        private void view_alpha_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[165]);
            if (view_alpha)
                Category_selected(view_alpha_ck);
            else
                Category_hover(view_alpha_ck);
        }
        private void view_alpha_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (view_alpha)
                Category_checked(view_alpha_ck);
            else
                Category_unchecked(view_alpha_ck);
        }
        private void view_algorithm_Click(object sender, EventArgs e)
        {
            if (view_algorithm)
            {
                Hide_algorithm(algorithm);
                Category_hover(view_algorithm_ck);
            }
            else
            {
                View_algorithm(algorithm);
                Category_selected(view_algorithm_ck);
            }
        }
        private void view_algorithm_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[166]);
            if (view_algorithm)
                Category_selected(view_algorithm_ck);
            else
                Category_hover(view_algorithm_ck);
        }
        private void view_algorithm_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (view_algorithm)
                Category_checked(view_algorithm_ck);
            else
                Category_unchecked(view_algorithm_ck);
        }
        private void view_WrapS_Click(object sender, EventArgs e)
        {
            if (view_WrapS)
            {
                Hide_WrapS();
                Category_hover(view_WrapS_ck);
            }
            else
            {
                View_WrapS();
                Category_selected(view_WrapS_ck);
            }
        }
        private void view_WrapS_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[167]);
            if (view_WrapS)
                Category_selected(view_WrapS_ck);
            else
                Category_hover(view_WrapS_ck);
        }
        private void view_WrapS_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (view_WrapS)
                Category_checked(view_WrapS_ck);
            else
                Category_unchecked(view_WrapS_ck);
        }
        private void view_WrapT_Click(object sender, EventArgs e)
        {
            if (view_WrapT)
            {
                Hide_WrapT();
                Category_hover(view_WrapT_ck);
            }
            else
            {
                View_WrapT();
                Category_selected(view_WrapT_ck);
            }
        }
        private void view_WrapT_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[168]);
            if (view_WrapT)
                Category_selected(view_WrapT_ck);
            else
                Category_hover(view_WrapT_ck);
        }
        private void view_WrapT_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (view_WrapT)
                Category_checked(view_WrapT_ck);
            else
                Category_unchecked(view_WrapT_ck);
        }
        private void view_min_Click(object sender, EventArgs e)
        {
            if (view_min)
            {
                Hide_min();
                Category_hover(view_min_ck);
            }
            else
            {
                View_min();
                Category_selected(view_min_ck);
            }
        }
        private void view_min_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[169]);
            if (view_min)
                Category_selected(view_min_ck);
            else
                Category_hover(view_min_ck);
        }
        private void view_min_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (view_min)
                Category_checked(view_min_ck);
            else
                Category_unchecked(view_min_ck);
        }
        private void view_mag_Click(object sender, EventArgs e)
        {
            if (view_mag)
            {
                Hide_mag();
                Category_hover(view_mag_ck);
            }
            else
            {
                View_mag();
                Category_selected(view_mag_ck);
            }
        }
        private void view_mag_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[170]);
            if (view_mag)
                Category_selected(view_mag_ck);
            else
                Category_hover(view_mag_ck);
        }
        private void view_mag_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (view_mag)
                Category_checked(view_mag_ck);
            else
                Category_unchecked(view_mag_ck);
        }
        private void view_rgba_Click(object sender, EventArgs e)
        {

        }
        private void view_rgba_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[171]);
            if (view_rgba)
                Category_selected(view_rgba_ck);
            else
                Category_hover(view_rgba_ck);
        }
        private void view_rgba_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (view_rgba)
                Category_checked(view_rgba_ck);
            else
                Category_unchecked(view_rgba_ck);
        }
        private void view_palette_Click(object sender, EventArgs e)
        {
            if (view_palette)
            {
                Hide_palette();
                Category_hover(view_palette_ck);
            }
            else
            {
                View_palette();
                Category_selected(view_palette_ck);
            }
        }
        private void view_palette_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[172]);
            if (view_palette)
                Category_selected(view_palette_ck);
            else
                Category_hover(view_palette_ck);
        }
        private void view_palette_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (view_palette)
                Category_checked(view_palette_ck);
            else
                Category_unchecked(view_palette_ck);
        }
        private void view_cmpr_Click(object sender, EventArgs e)
        {
            if (view_cmpr)
            {
                Hide_cmpr();
                Category_hover(view_cmpr_ck);
            }
            else
            {
                View_cmpr();
                Category_selected(view_cmpr_ck);
            }
        }
        private void view_cmpr_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[173]);
            if (view_cmpr)
                Category_selected(view_cmpr_ck);
            else
                Category_hover(view_cmpr_ck);
        }
        private void view_cmpr_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (view_cmpr)
                Category_checked(view_cmpr_ck);
            else
                Category_unchecked(view_cmpr_ck);
        }
        private void view_options_Click(object sender, EventArgs e)
        {
            if (view_options)
            {
                Hide_options();
                Category_hover(view_options_ck);
            }
            else
            {
                View_options();
                Category_selected(view_options_ck);
            }
        }
        private void view_options_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[174]);
            if (view_options)
                Category_selected(view_options_ck);
            else
                Category_hover(view_options_ck);
        }
        private void view_options_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (view_options)
                Category_checked(view_options_ck);
            else
                Category_unchecked(view_options_ck);
        }
        private void Run_Click(object sender, EventArgs e)
        {
            run_count++;
            Parse_args_class cli = new Parse_args_class();
            cli.Parse_args(arg_array.ToArray());
            if (run_count < 2)
                output_label.Text = "Run " + run_count.ToString() + " time\n" + cli.Check_exit();
            else
                output_label.Text = "Run " + run_count.ToString() + " times\n" + cli.Check_exit();
        }
        private void banner_move_MouseEnter(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.SizeAll;
        }
        private void banner_move_MouseLeave(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }
        private void banner_move_MouseDown(object sender, MouseEventArgs e)
        {
            // e.Button;
            mouse_x = e.X;
            mouse_y = e.Y;
            mouse_down = true;
        }
        private void banner_move_MouseUp(object sender, MouseEventArgs e)
        {
            mouse_down = false;
        }
        private void banner_move_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouse_down)
            {
                this.Location = new Point(this.Location.X + e.X - mouse_x, this.Location.Y + e.Y - mouse_y);
            }
        }

        private void banner_resize_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_x = e.X;
            mouse_y = e.Y;
            mouse_down = true;
        }

        private void banner_resize_MouseUp(object sender, MouseEventArgs e)
        {
            mouse_down = false;
        }

        private void banner_resize_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouse_down)
            {
                this.Location = new Point(this.Location.X + e.X - mouse_x, this.Location.Y + e.Y - mouse_y);
                this.Size = new Size(this.Size.Width + mouse_x - e.X, this.Size.Height + mouse_y - e.Y);
            }
        }

        private void banner_resize_MouseEnter(object sender, EventArgs e)
        {

        }

        private void banner_resize_MouseLeave(object sender, EventArgs e)
        {

        }
    }
}
