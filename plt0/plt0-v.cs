﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Linq;
/* note: pasting C# Guy.py code adds automatically new usings, messing up the whole code
 * here's the official list of usings. it should be the same as what's above. else copy and paste
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Linq;
*/
/* all the junk added so far
using System.Drawing.Drawing2D;
using System.Reflection;
using plt0;
using System.CodeDom.Compiler;
using System.Text;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;
using System.ComponentModel;
using static System.Net.WebRequestMethods;
using System.Drawing.Text;
using System.Linq; */

namespace plt0_gui
{
    public partial class plt0_gui : Form
    {
        private static readonly ImageConverter _imageConverter = new ImageConverter();
        static readonly string execPath = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");
        static readonly string execName = System.AppDomain.CurrentDomain.FriendlyName.Replace("\\", "/");
        static readonly string appdata = Environment.GetEnvironmentVariable("appdata").Replace("\\", "/");
        static string args;
        byte[] cmpr_preview;
        byte[] cmpr_preview_vanilla;
        byte[] colour_4 = { 0, 0, 0, 0 };
        // the 4x4 grid for the Paint Layout is a 4x4 bmp file
        byte[] cmpr_4x4 = { 66, 77, 186, 0, 0, 0, 121, 111, 115, 104, 122, 0, 0, 0, 108, 0, 0, 0, 4, 0, 0, 0, 4, 0, 0, 0, 1, 0, 32, 0, 3, 0, 0, 0, 64, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 0, 0, 255, 0, 0, 255, 0, 0, 0, 0, 0, 0, 255, 32, 110, 105, 87, 0, 104, 116, 116, 112, 115, 58, 47, 47, 100, 105, 115, 99, 111, 114, 100, 46, 103, 103, 47, 118, 57, 86, 112, 68, 90, 57, 0, 116, 104, 105, 115, 32, 105, 115, 32, 112, 97, 100, 100, 105, 110, 103, 32, 100, 97, 116, 97, 0, 0, 240, 255, 72, 242, 64, 255, 72, 66, 240, 255, 0, 255, 0, 255, 200, 254, 200, 255, 32, 57, 240, 255, 32, 249, 56, 255, 200, 206, 240, 255, 16, 16, 240, 255, 32, 241, 56, 255, 48, 49, 255, 255, 72, 250, 64, 255, 16, 255, 16, 255, 32, 57, 255, 255, 48, 241, 48, 255, 72, 66, 240, 255 };
        string[] cmpr_args = new string[] { "gui", "i", "i", execPath + "images/preview/1" };
        string[] settings = new string[255];
        string[] layout_name = { "All", "Auto", "Preview", "Paint" };
        string cmpr_colours_hex;
        byte[] cmpr_file;
        byte[] cmpr_colours_argb = new byte[16];
        byte[] cmpr_colour = new byte[4];
        byte[] cmpr_index = new byte[16];
        byte[] block_width_array = { 8, 8, 8, 4, 4, 4, 4, 255, 8, 8, 4, 255, 255, 255, 8 }; // real one to calculate canvas size.
        //byte[] block_width_array = { 4, 8, 8, 8, 8, 8, 16, 255, 4, 8, 8, 255, 255, 255, 4 }; // altered to match bit-per pixel size.
        byte[] block_height_array = { 8, 4, 4, 4, 4, 4, 4, 255, 8, 4, 4, 255, 255, 255, 8 }; // 255 = unused image format
        byte[] block_depth_array = { 4, 8, 4, 8, 16, 16, 32, 255, 4, 8, 16, 255, 255, 255, 4 };  // for \z
        string[] encoding_array = { "i4", "i8", "ai4", "ai8", "rgb565", "rgb5a3", "rgba32", "", "ci4", "ci8", "ci14x2", "", "", "", "cmpr" };
        string[] wrap_array = { "Clamp", "Repeat", "Mirror", "Clamp" };
        string[] algorithm_array = { "", "cie709", "custom_rgba", "gamma", "", "", "", "", "", "" };
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
        byte red;
        byte green;
        byte blue;
        byte red2;
        byte green2;
        byte blue2;
        byte alpha2;
        bool banner_global_move = false;
        bool cmpr_update_preview = true;
        bool sync_preview_is_on = false;
        bool the_program_is_loading_a_cmpr_block = false;
        bool cmpr_swap2_enabled = false;
        bool cmpr_hover = true;
        byte cmpr_index_i = 0;
        byte cmpr_hover_red = 255;
        byte cmpr_hover_green = 255;
        byte cmpr_hover_blue = 255;
        byte cmpr_hover_alpha = 0;
        /*byte cmpr_edit_red = 0;
        byte cmpr_edit_green = 0;
        byte cmpr_edit_blue = 0;
        byte cmpr_edit_alpha = 0;*/
        byte cmpr_selected_colour = 1;
        ushort cmpr_x_offscreen;
        ushort cmpr_y_offscreen;
        int cmpr_x;
        int cmpr_y;
        ushort colour1;
        ushort colour2;
        ushort colour3;
        ushort colour4;
        double mag_ratio;
        //byte offset;
        string seal;
        //byte jump_line;
        float size_font;
        System.Drawing.GraphicsUnit unit_font;
        System.Windows.Forms.Padding cli_textbox_padding = new System.Windows.Forms.Padding(0, 12, 0, 20);
        System.Drawing.Point cli_textbox_location = new System.Drawing.Point(71, 1000);
        int y;
        int cmpr_data_start_offset;
        int current_block;
        int loaded_block = -1;
        int previous_block = -1;
        int max_block;
        int cmpr_preview_start_offset;
        byte byte_text;
        bool success;
        /*bool Left_down = false;
        bool Right_down = false;
        bool Middle_down = false;
        bool XButton1_down = false;
        bool XButton2_down = false;
        /* bool bold;
        bool italic;
        bool underline;
        bool strikeout;*/
        byte font_style;
        bool vertical;
        bool preview_layout_is_enabled = false;
        bool preview_changed = false;
        bool upscale = true;
        bool auto_update = true;
        bool textchange = false;
        bool all_layout_is_enabled = true;
        bool cmpr_layout_is_in_place = false;
        bool cmpr_layout_is_enabled = false;
        // banner_move
        int mouse_x;
        int mouse_y;
        //bool mouse_down;
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
        bool reversex = false;
        bool reversey = false;
        bool warn = false;
        bool stfu = false;
        bool no_warning = false;
        bool name_string = false;
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
        byte algorithm = 9;  // 0 = CIE 601    1 = CIE 709     2 = custom RGBA     3 = Most Used Colours (No Gradient)
        // for cmpr : algorithm   0 = smart   1 = Range Fit   2 = Most Used/Furthest   3 = Darkest/Lightest   4 = No Gradient   5 = Wiimm (counterfeit)   6 = SuperBMD (counterfeit)   7 = Min/Max
        byte alpha = 3;  // 0 = no alpha - 1 = alpha - 2 = mix 
        byte magnification_filter = 6;  // 0 = Nearest Neighbour   1 = Linear
        byte minification_filter = 6;  // 0 = Nearest Neighbour   1 = Linear
        byte r = 0;
        byte g = 1;
        byte b = 2;
        byte a = 3;
        // numbers
        byte cmpr_max = 0;  // number of colours that the program should take care in each 4x4 block - should always be set to 16 for better results.  // wimgt's cmpr encoding is better than mine. I gotta admit. 
        byte cmpr_min_alpha = 100; // byte cmpr_alpha_threshold = 100;
        byte diversity = 10;
        byte diversity2 = 0;
        byte mipmaps = 0;
        byte round3 = 15;
        byte round4 = 7;
        byte round5 = 3;
        byte round6 = 1;
        ushort num_colours = 0;
        ushort block_x;
        ushort block_y;
        ushort selected_block_x;
        ushort selected_block_y;
        ushort blocks_wide;
        ushort blocks_tall;
        byte tooltip = 0;
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
        // List<Label> cmpr_grid = new List<Label>();
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
        Image gradient;
        Image surrounding;
        Image check;
        Image white_box;
        Image light_blue_box;
        Image light_blue_check;
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
        Image all_hover;
        Image all_off;
        Image all_on;
        Image all_selected;
        Image auto_hover;
        Image auto_off;
        Image auto_on;
        Image auto_selected;
        Image banner_global_move_hover;
        Image banner_global_move_off;
        Image banner_global_move_on;
        Image banner_global_move_selected;
        Image cli_textbox;
        Image cli_textbox_hover;
        Image close;
        Image close_hover;
        Image cmpr_save;
        Image cmpr_save_as;
        Image cmpr_save_as_hover;
        Image cmpr_save_hover;
        Image cmpr_swap2;
        Image cmpr_swap2_hover;
        Image cmpr_swap;
        Image cmpr_swap_hover;
        Image discord;
        Image discord_hover;
        Image github;
        Image github_hover;
        Image maximized_hover;
        Image maximized_off;
        Image maximized_on;
        Image maximized_selected;
        Image minimized;
        Image minimized_hover;
        Image paint_hover;
        Image paint_off;
        Image paint_on;
        Image paint_selected;
        Image preview_hover;
        Image preview_off;
        Image preview_on;
        Image preview_selected;
        Image run_hover;
        Image run_off;
        Image run_on;
        Image sync_preview_hover;
        Image sync_preview_off;
        Image sync_preview_on;
        Image sync_preview_selected;
        Image version;
        Image version_hover;
        Image youtube;
        Image youtube_hover;
        Image bottom_hover;
        Image bottom_left_hover;
        Image bottom_left_off;
        Image bottom_left_on;
        Image bottom_left_selected;
        Image bottom_off;
        Image bottom_on;
        Image bottom_right_hover;
        Image bottom_right_off;
        Image bottom_right_on;
        Image bottom_right_selected;
        Image bottom_selected;
        Image right_hover;
        Image right_off;
        Image right_on;
        Image right_selected;
        Image left_hover;
        Image left_off;
        Image left_on;
        Image left_selected;
        Image top_hover;
        Image top_left_hover;
        Image top_left_off;
        Image top_left_on;
        Image top_left_selected;
        Image top_off;
        Image top_on;
        Image top_right_hover;
        Image top_right_off;
        Image top_right_on;
        Image top_right_selected;
        Image top_selected;
        Image screen2_bottom_hover;
        Image screen2_bottom_left_hover;
        Image screen2_bottom_left_off;
        Image screen2_bottom_left_on;
        Image screen2_bottom_left_selected;
        Image screen2_bottom_off;
        Image screen2_bottom_on;
        Image screen2_bottom_right_hover;
        Image screen2_bottom_right_off;
        Image screen2_bottom_right_on;
        Image screen2_bottom_right_selected;
        Image screen2_bottom_selected;
        Image screen2_right_hover;
        Image screen2_right_off;
        Image screen2_right_on;
        Image screen2_right_selected;
        Image screen2_left_hover;
        Image screen2_left_off;
        Image screen2_left_on;
        Image screen2_left_selected;
        Image screen2_top_hover;
        Image screen2_top_left_hover;
        Image screen2_top_left_off;
        Image screen2_top_left_on;
        Image screen2_top_left_selected;
        Image screen2_top_off;
        Image screen2_top_on;
        Image screen2_top_right_hover;
        Image screen2_top_right_off;
        Image screen2_top_right_on;
        Image screen2_top_right_selected;
        Image screen2_arrow_1080p_hover;
        Image screen2_arrow_1080p_off;
        Image screen2_arrow_1080p_on;
        Image screen2_arrow_1080p_selected;
        Image arrow_1080p_hover;
        Image arrow_1080p_off;
        Image arrow_1080p_on;
        Image arrow_1080p_selected;
        Image screen2_top_selected;

        // I couldn't manage to get external fonts working. this needs to be specified within the app itself :/
        // static string fontname = "Segoe UI";
        // Image input_file_image;
        // Font font_normal = new System.Drawing.Font(fontname, 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
        // Font new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
        public plt0_gui()
        {
            plt0.NativeMethods.FreeConsole();
            InitializeComponent();
            this.Size = new Size(1920, 1080);
            InitializeForm();

            //below's some scrapped code to load custom fonts from a file, but it won't work unless the font itself is installed on the system, which makes the file useless
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
            // Make the GUI ignore the DPI setting
            //output_file_type_label.Text = fontname;
            //algorithm_label.Text = algorithm_label.Font.Name;
            //markdown.Add(font_name);
            //markdown.Add(font_colour);
            //markdown.Add(font_unit);
            //markdown.Add(font_encoding);
            //markdown.Add(font_size);
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
        private void Parse_Markdown(string txt, Label lab)
        {// these are variables. easy to replace
         //if (input_file_image != null)
         //    txt = txt.Replace("\\d", input_file_image.PixelFormat.ToString());
         //else
            txt = txt.Replace("\\d", "");
            // implement b, c, f, g, i, j, k, q, s, u, v, x
            font_colour = config[16];  // default colour
            font_name = config[6]; // default font name
            font_style = 0;
            font_unit = "pixel";
            size_font = 20F;
            font_size = "20";
            font_encoding = "128";
            byte.TryParse(config[8], out GdiCharSet);
            if (txt.Contains("\\b"))
            {
                font_style += 1;
            }
            if (txt.Contains("\\i"))
            {
                font_style += 2;
            }
            if (txt.Contains("\\k"))
            {
                font_style += 4;
            }
            if (txt.Contains("\\u"))
            {
                font_style += 8;
            }
            if (txt.Contains("\\v"))
            {
                vertical = false;
            }
            txt = Parse_Special_Markdown(txt.LastIndexOf("\\f"), txt, 0);  // fill markdown[0]
            txt = Parse_Special_Markdown(txt.LastIndexOf("\\c"), txt, 1);  // fill markdown[1]
            txt = Parse_Special_Markdown(txt.LastIndexOf("\\q"), txt, 2);  // fill markdown[2]
            txt = Parse_Special_Markdown(txt.LastIndexOf("\\g"), txt, 3);  // fill markdown[3]
            txt = Parse_Special_Markdown(txt.LastIndexOf("\\s"), txt, 4);  // fill markdown[4]
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
                    unit_font = GraphicsUnit.Pixel;
                    break;

            }
            // txt = Parse_Special_Markdown(txt.LastIndexOf("\\j"), txt, backslash_j);  OH GOSH I AM IDIOT - that happens when you leave your code for one day lol
            y = 0;
            y = txt.IndexOf("\\x");
            while (y != -1)
            {
                byte.TryParse(txt.Substring(y + 2, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out byte_text);
                txt = txt.Substring(0, y) + System.Text.Encoding.Unicode.GetString(new[] { byte_text }) + txt.Substring(y + 4);
                y = txt.IndexOf("\\x");
            }
            txt = txt.Replace("\\a", appdata).Replace("\\b", "").Replace("\\e", execName).Replace("\\h", this.Height.ToString()).Replace("\\i", "").Replace("\\k", "").Replace("\\l", layout_name[layout]).Replace("\\m", mipmaps.ToString()).Replace("\\n", "\n").Replace("\\o", output_name).Replace("\\p", execPath).Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\u", "").Replace("\\v", "").Replace("\\w", this.Width.ToString()).Replace("\\0", block_width_array[encoding].ToString()).Replace("\\y", block_height_array[encoding].ToString()).Replace("\\z", block_depth_array[encoding].ToString());
            // lab.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, ((System.Drawing.FontStyle)((((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) | System.Drawing.FontStyle.Underline) | System.Drawing.FontStyle.Strikeout))), System.Drawing.GraphicsUnit.World, ((byte)(0)), true);
            switch (font_style)
            {
                case 0:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Regular, unit_font, GdiCharSet, vertical);
                    break;
                case 1:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Bold, unit_font, GdiCharSet, vertical);
                    break;
                case 2:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Italic, unit_font, GdiCharSet, vertical);
                    break;
                case 3:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Bold | FontStyle.Italic, unit_font, GdiCharSet, vertical);
                    break;
                case 4:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Strikeout, unit_font, GdiCharSet, vertical);
                    break;
                case 5:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Strikeout | FontStyle.Bold, unit_font, GdiCharSet, vertical);
                    break;
                case 6:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Strikeout | FontStyle.Italic, unit_font, GdiCharSet, vertical);
                    break;
                case 7:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Strikeout | FontStyle.Italic | FontStyle.Bold, unit_font, GdiCharSet, vertical);
                    break;
                case 8:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline, unit_font, GdiCharSet, vertical);
                    break;
                case 9:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Bold, unit_font, GdiCharSet, vertical);
                    break;
                case 10:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Italic, unit_font, GdiCharSet, vertical);
                    break;
                case 11:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Bold | FontStyle.Italic, unit_font, GdiCharSet, vertical);
                    break;
                case 12:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Strikeout, unit_font, GdiCharSet, vertical);
                    break;
                case 13:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Strikeout | FontStyle.Bold, unit_font, GdiCharSet, vertical);
                    break;
                case 14:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Strikeout | FontStyle.Italic, unit_font, GdiCharSet, vertical);
                    break;
                case 15:
                    lab.Font = new System.Drawing.Font(font_name, size_font, FontStyle.Underline | FontStyle.Strikeout | FontStyle.Italic | FontStyle.Bold, unit_font, GdiCharSet, vertical);
                    break;
            }
            lab.ForeColor = Color.FromName(font_colour);
            lab.Text = txt;
        }
        private void Parse_Markdown(string txt)
        {
            // these are variables. easy to replace
            txt = txt.Replace("\\a", appdata).Replace("\\e", execName).Replace("\\h", this.Height.ToString()).Replace("\\l", layout_name[layout]).Replace("\\m", mipmaps.ToString()).Replace("\\n", "\n").Replace("\\o", output_name).Replace("\\p", execPath).Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\w", this.Width.ToString()).Replace("\\0", block_width_array[encoding].ToString()).Replace("\\y", block_height_array[encoding].ToString()).Replace("\\z", block_depth_array[encoding].ToString());
            //if (input_file_image != null)
            //    txt = txt.Replace("\\d", input_file_image.PixelFormat.ToString());
            //else
            txt = txt.Replace("\\d", "");
            // implement b, c, f, g, i, j, k, q, s, u, v, x
            string[] txt_label = txt.Split(new string[] { "\\j" }, StringSplitOptions.None);
            for (byte i = 0; i < (byte)txt_label.Length; i++)
            {
                font_colour = config[16];  // default colour
                font_name = config[6]; // default font name
                font_style = 0;
                font_unit = "pixel";
                size_font = 20F;
                font_encoding = "128";
                font_size = "20";
                byte.TryParse(config[8], out GdiCharSet);
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
                        unit_font = GraphicsUnit.Pixel;
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
        private bool Check_run()
        {
            if (bmd)
            {
                if (!File.Exists(input_file2))
                {
                    run_ck.BackgroundImage = run_off;
                    return false;
                }
            }
            if (File.Exists(input_file) && encoding != 7)
            {
                if (bmd || bti || tex0 || tpl || bmp || png || jpeg || gif || ico || tif || tiff)
                {
                    run_ck.BackgroundImage = run_on;
                    return true;
                }
                else
                {
                    run_ck.BackgroundImage = run_off;
                    return true;  // yup, technically you can preview the file even though output file type isn't inserted haha
                }
            }
            else
            {
                run_ck.BackgroundImage = run_off;
                return false;
            }
        }
        private void Preview(bool called_from_text, bool called_from_sync = false)
        {
            if (layout != 2)
                return;
            preview_changed = true;
            if (!sync_preview_is_on)
            {
                sync_preview_is_on = true;
                sync_preview_ck.BackgroundImage = sync_preview_on;
            }
            if (called_from_text && !textchange)
                return;
            if (!auto_update && !called_from_sync)
                return;
            if (Check_run())
            {
                int num = 1;
                while (File.Exists(execPath + "images/preview/" + num + ".bmp"))
                {
                    num++;
                }
                // foreach(string arg in arg_array)  // best to parse here rather than in Organize_args because this is only for layout 2
                for (byte i = 0; i < arg_array.Count; i++)
                {
                    switch (arg_array[i])
                    {
                        case "bmd":
                        case "bti":
                        case "tex0":
                        case "tpl":
                        case "bmp":
                        case "png":
                        case "jpg":
                        case "jpeg":
                        case "gif":
                        case "ico":
                        case "tif":
                        case "tiff":
                            arg_array.Remove(arg_array[i]);
                            break;
                    }
                }
                arg_array.Add("bmp");
                arg_array.Add(execPath + "images/preview/" + num + ".bmp");  // even if there's an output file in the args, the last one is th eoutput file :) that's how I made it
                Parse_args_class cli = new Parse_args_class();
                cli.Parse_args(arg_array.ToArray());
                input_file2 = cli.Check_exit();
                // PictureBoxWithInterpolationMode preview_ck2 = new PictureBoxWithInterpolationMode();
                if (upscale)
                    image_ck.SizeMode = PictureBoxSizeMode.Zoom;
                else
                    image_ck.SizeMode = PictureBoxSizeMode.CenterImage;
                //preview_ck2.Location = new Point(815, 96);
                //preview_ck2.Size = new Size(768, 768);
                //image_ck.InterpolationMode = InterpolationMode.NearestNeighbor;
                //if (image_ck.BackgroundImage != null)
                //    image_ck.BackgroundImage.Dispose();
                if (File.Exists(execPath + "images/preview/" + num + ".bmp"))
                {
                    image_ck.Image = Image.FromFile(execPath + "images/preview/" + num + ".bmp");
                    preview_changed = false;
                    sync_preview_ck.BackgroundImage = sync_preview_off;
                    sync_preview_is_on = false;
                }
                else
                    cli_textbox_label.Text = cli.Check_exit();
                //do something
            }
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
                arg_array.Add("i");
                arg_array.Add(input_file);
            }
            if (File.Exists(input_file2))
            {
                args += "\"" + input_file2 + "\" ";
                arg_array.Add("j");
                arg_array.Add(input_file2);
            }
            if (output_name != "")
            {
                args += "\"" + output_name + "\" ";
                arg_array.Add("o");
                arg_array.Add(output_name);
            }
            if (encoding != 7)
            {
                args += encoding_array[encoding] + " ";
                arg_array.Add(encoding_array[encoding]);
            }
            if (palette_enc != 3 && encoding > 7 && encoding != 14)
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
            if (encoding == 0xE)
            {
                switch (algorithm)
                {
                    case 1:
                        args += "Range fit " + " ";
                        arg_array.Add("range");
                        break;
                    case 2:
                        args += "most used furthest" + " ";
                        arg_array.Add("muf");
                        break;
                    case 3:
                        args += "dl " + " ";
                        arg_array.Add("dl");
                        break;
                    case 4:
                        args += "no gradient" + " ";
                        arg_array.Add("ng");
                        break;
                    case 5:
                        args += "Weemm " + " ";
                        arg_array.Add("weemm");
                        break;
                    case 6:
                        args += "SooperBMD " + " ";
                        arg_array.Add("SooperBMD");
                        break;
                    case 7:
                        args += "Min_Max " + " ";
                        arg_array.Add("min_max");
                        break;
                }
            }
            else
            {
                if (algorithm != 0)
                {
                    args += algorithm_array[algorithm] + " ";
                    arg_array.Add(algorithm_array[algorithm]);
                }
                if (algorithm == 2)  // custom rgba but not for cmpr
                {
                    args += custom_r.ToString() + " " + custom_g.ToString() + " " + custom_b.ToString() + " " + custom_a.ToString() + " ";
                    arg_array.Add(custom_r.ToString());
                    arg_array.Add(custom_g.ToString());
                    arg_array.Add(custom_b.ToString());
                    arg_array.Add(custom_a.ToString());
                }
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
            if (cmpr_max != 0)
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
                arg_array.Add("ask");
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
                arg_array.Add("nw");
            }
            if (random)
            {
                args += "random ";
                arg_array.Add("random");
            }
            if (reversex)
            {
                args += "reverse x ";
                arg_array.Add("x");
            }
            if (reversey)
            {
                args += "reverse y ";
                arg_array.Add("y");
            }
            if (safe_mode)
            {
                args += "safe_mode ";
                arg_array.Add("safe");
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
                if (!isFolder)  // that means it's a file.
                {
                    //byte len = (byte)file[0].Split('\\').Length;
                    input_file = file[0];
                    input_file_txt.Text = file[0]; //.Split('\\')[len - 1];// file is actually a full path starting from a drive, but I won't clutter the display
                }
                else
                {
                    output_label.Text = "only files are accepted\n";
                    return;
                }
            }
            else
            {
                output_label.Text = (string)e.Data.GetData(DataFormats.Text);
            }
        }
        private void plt0_gui_MouseEnter(object sender, EventArgs e)
        {
            if (banner_global_move)
                this.Cursor = Cursors.SizeAll;
        }
        private void plt0_gui_MouseLeave(object sender, EventArgs e)
        {
            if (banner_global_move)
                this.Cursor = Cursors.Default;
        }
        private bool ishex(char txt)
        {
            switch (txt)
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

            return true;
        }
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
        private void unchecked_tooltip(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = cherry_circle;
        }
        private void checked_tooltip(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = cherry_circle_on;
        }
        private void hover_tooltip(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = blue_circle;
        }
        private void selected_tooltip(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = blue_circle_on;
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
        private void Layout_All()
        {
            if (encoding == 14)
                View_cmpr(true);
            else
            {
                Hide_cmpr();
                View_alpha();
                round5_label.Visible = true;
                round5_txt.Visible = true;
                round6_label.Visible = true;
                round6_txt.Visible = true;
                cmpr_max_label.Visible = true;
                cmpr_max_txt.Visible = true;
                cmpr_min_alpha_label.Visible = true;
                cmpr_min_alpha_txt.Visible = true;
                darkest_lightest_ck.Visible = true;
                darkest_lightest_label.Visible = true;
            }
            layout = 1;
            View_algorithm(255);
            View_palette();
            View_mag();
            View_min();
            View_WrapS();
            View_WrapT();
            View_options();
            View_rgba(true);
            round3_label.Visible = true;
            round3_txt.Visible = true;
            round4_label.Visible = true;
            round4_txt.Visible = true;
            if (preview_layout_is_enabled)
                Disable_Preview_Layout();
            if (cmpr_layout_is_enabled)
                Disable_Paint_Layout();
            all_layout_is_enabled = true;
            options_label.Location = new Point(1674, 40);
            ask_exit_ck.Visible = true;
            ask_exit_label.Visible = true;
            layout = 0;

        }
        private void Layout_Auto()
        {
            layout = 1;
            cie_601_label.Text = "Default";
            Hide_options();
            Hide_cmpr();
            Hide_encoding(1);
            Hide_encoding(5);
            Hide_encoding(8);
            if (bti || tpl)
            {
                View_mag();
                View_min();
                View_WrapS();
                View_WrapT();
            }
            else
            {
                Hide_mag();
                Hide_min();
                Hide_WrapS();
                Hide_WrapT();
            }
            switch (encoding)
            {
                case 0:
                    View_i4();
                    break;
                case 1:
                    View_i8();
                    break;
                case 2:
                    View_ai4();
                    break;
                case 3:
                    View_ai8();
                    break;
                case 4:
                    View_rgb565();
                    break;
                case 5:
                    View_rgb5a3();
                    break;
                case 6:
                    View_rgba32();
                    break;
                case 8:
                    View_ci4();
                    break;
                case 9:
                    View_ci8();
                    break;
                case 10:
                    View_ci14x2();
                    break;
                case 14:
                    View_cmpr();
                    break;
            }
            if (algorithm == 2)
                Hide_algorithm(3);
            else
                Hide_algorithm(2);
            View_algorithm(algorithm);
            if (preview_layout_is_enabled)
                Disable_Preview_Layout();
            if (all_layout_is_enabled)
                Disable_All_Layout();
            if (cmpr_layout_is_enabled)
                Disable_Paint_Layout();
        }
        private void Layout_Preview()
        {
            if (encoding == 14)
                View_cmpr(true);
            else
            {
                Hide_cmpr();
                View_alpha();
                round5_label.Visible = true;
                round5_txt.Visible = true;
                round6_label.Visible = true;
                round6_txt.Visible = true;
                cmpr_max_label.Visible = true;
                cmpr_max_txt.Visible = true;
                cmpr_min_alpha_label.Visible = true;
                cmpr_min_alpha_txt.Visible = true;
                darkest_lightest_ck.Visible = true;
                darkest_lightest_label.Visible = true;
            }
            layout = 1;
            View_algorithm(255);
            View_palette();
            Hide_mag();
            Hide_min();
            Hide_WrapS();
            Hide_WrapT();
            View_options();
            View_rgba(true);
            round3_label.Visible = true;
            round3_txt.Visible = true;
            round4_label.Visible = true;
            round4_txt.Visible = true;
            if (!preview_layout_is_enabled)
                Enable_Preview_Layout();
            if (all_layout_is_enabled)
                Disable_All_Layout();
            if (cmpr_layout_is_enabled)
                Disable_Paint_Layout();
            auto_update_ck.Visible = true;
            auto_update_label.Visible = true;
            upscale_ck.Visible = true;
            upscale_label.Visible = true;
            textchange_ck.Visible = true;
            textchange_label.Visible = true;
            sync_preview_ck.Visible = true;
            image_ck.Visible = true;
            layout = 2;
        }
        private void Enable_Preview_Layout()
        {
            output_label.Visible = false;
            description_title.Visible = false;
            description_surrounding.Visible = false;
            for (byte i = 0; i < 9; i++)
            {
                desc[i].Location = new Point(desc[i].Location.X, desc[i].Location.Y + 600);
            }
            preview_layout_is_enabled = true;
        }
        private void Disable_Preview_Layout()
        {
            auto_update_ck.Visible = false;
            auto_update_label.Visible = false;
            upscale_ck.Visible = false;
            upscale_label.Visible = false;
            textchange_ck.Visible = false;
            textchange_label.Visible = false;
            sync_preview_ck.Visible = false;
            image_ck.Visible = false;

            output_label.Visible = true;
            description_title.Visible = true;
            description_surrounding.Visible = true;
            for (byte i = 0; i < 9; i++)
            {
                desc[i].Location = new Point(desc[i].Location.X, desc[i].Location.Y - 600);
            }
            preview_layout_is_enabled = false;
        }
        private void Disable_All_Layout()
        {
            ask_exit_ck.Visible = false;
            ask_exit_label.Visible = false;
            options_label.Location = new Point(1674, 96);
            all_layout_is_enabled = false;
        }
        private void Disable_Paint_Layout()
        {
            a_a_ck.Visible = true;
            a_b_ck.Visible = true;
            a_g_ck.Visible = true;
            a_r_ck.Visible = true;
            ai4_ck.Visible = true;
            ai4_label.Visible = true;
            ai8_ck.Visible = true;
            ai8_label.Visible = true;
            algorithm_label.Visible = true;
            b_a_ck.Visible = true;
            b_b_ck.Visible = true;
            b_g_ck.Visible = true;
            b_r_ck.Visible = true;
            bmd_ck.Visible = true;
            bmd_label.Visible = true;
            bmp_32_ck.Visible = true;
            bmp_32_label.Visible = true;
            bmp_ck.Visible = true;
            bmp_label.Visible = true;
            bti_ck.Visible = true;
            bti_label.Visible = true;
            ci14x2_ck.Visible = true;
            ci14x2_label.Visible = true;
            ci4_ck.Visible = true;
            ci4_label.Visible = true;
            ci8_ck.Visible = true;
            ci8_label.Visible = true;
            cie_601_ck.Visible = true;
            cie_601_label.Visible = true;
            cli_textbox_ck.Visible = true;
            cli_textbox_label.Visible = true;
            cmpr_block_paint_ck.Visible = false;
            cmpr_block_paint_label.Visible = false;
            cmpr_block_selection_ck.Visible = false;
            cmpr_block_selection_label.Visible = false;
            cmpr_c1.Visible = false;
            cmpr_c1_label.Visible = false;
            cmpr_c1_txt.Visible = false;
            cmpr_c2.Visible = false;
            cmpr_c2_label.Visible = false;
            cmpr_c2_txt.Visible = false;
            cmpr_c3.Visible = false;
            cmpr_c3_label.Visible = false;
            cmpr_c3_txt.Visible = false;
            cmpr_c4.Visible = false;
            cmpr_c4_label.Visible = false;
            cmpr_c4_txt.Visible = false;
            cmpr_ck.Visible = true;
            cmpr_grid_ck.Visible = false;
            cmpr_hover_ck.Visible = false;
            cmpr_hover_colour.Visible = false;
            cmpr_hover_colour_label.Visible = false;
            cmpr_hover_colour_txt.Visible = false;
            cmpr_hover_label.Visible = false;
            cmpr_label.Visible = true;
            cmpr_layout_is_enabled = false;
            cmpr_mouse1_label.Visible = false;
            cmpr_mouse2_label.Visible = false;
            cmpr_mouse3_label.Visible = false;
            cmpr_mouse4_label.Visible = false;
            cmpr_mouse5_label.Visible = false;
            cmpr_palette.Visible = false;
            cmpr_picture_tooltip_label.Visible = false;
            cmpr_preview_ck.Visible = false;
            cmpr_save_as_ck.Visible = false;
            cmpr_save_ck.Visible = false;
            cmpr_sel.Visible = false;
            cmpr_sel_label.Visible = false;
            cmpr_selected_block_label.Visible = false;
            cmpr_swap2_ck.Visible = false;
            cmpr_swap2_label.Visible = false;
            cmpr_swap_ck.Visible = false;
            cmpr_swap_label.Visible = false;
            cmpr_update_preview_ck.Visible = false;
            cmpr_update_preview_label.Visible = false;
            cmpr_warning.Visible = false;
            colour_channels_label.Visible = true;
            custom_ck.Visible = true;
            custom_label.Visible = true;
            encoding_label.Visible = true;
            funky_ck.Visible = true;
            funky_label.Visible = true;
            g_a_ck.Visible = true;
            g_b_ck.Visible = true;
            g_g_ck.Visible = true;
            g_r_ck.Visible = true;
            gif_ck.Visible = true;
            gif_label.Visible = true;
            i4_ck.Visible = true;
            i4_label.Visible = true;
            i8_ck.Visible = true;
            i8_label.Visible = true;
            ico_ck.Visible = true;
            ico_label.Visible = true;
            input_file2_label.Visible = true;
            input_file2_txt.Visible = true;
            jpeg_ck.Visible = true;
            jpeg_label.Visible = true;
            jpg_ck.Visible = true;
            jpg_label.Visible = true;
            mandatory_settings_label.Visible = true;
            name_string_ck.Visible = true;
            name_string_label.Visible = true;
            options_label.Visible = true;
            output_file_type_label.Visible = true;
            output_label.Visible = true;
            png_ck.Visible = true;
            png_label.Visible = true;
            r_a_ck.Visible = true;
            r_b_ck.Visible = true;
            r_g_ck.Visible = true;
            r_r_ck.Visible = true;
            random_ck.Visible = true;
            random_label.Visible = true;
            reversex_ck.Visible = true;
            reversex_label.Visible = true;
            reversey_ck.Visible = true;
            reversey_label.Visible = true;
            rgb565_ck.Visible = true;
            rgb565_label.Visible = true;
            rgb5a3_ck.Visible = true;
            rgb5a3_label.Visible = true;
            rgba32_ck.Visible = true;
            rgba32_label.Visible = true;
            run_ck.Visible = true;
            surrounding_ck.Visible = true;
            tex0_ck.Visible = true;
            tex0_label.Visible = true;
            tif_ck.Visible = true;
            tif_label.Visible = true;
            tiff_ck.Visible = true;
            tiff_label.Visible = true;
            tpl_ck.Visible = true;
            tpl_label.Visible = true;
            for (byte i = 0; i < 9; i++)
            {
                desc[i].Location = new Point(desc[i].Location.X + 300, desc[i].Location.Y - 100);
            }
            description_title.Location = new Point(description_title.Location.X + 300, description_title.Location.Y - 100);
            description_surrounding.Location = new Point(description_surrounding.Location.X + 300, description_surrounding.Location.Y - 100);
            input_file_label.Location = new Point(input_file_label.Location.X, input_file_label.Location.Y - 14);
            input_file_txt.Location = new Point(input_file_txt.Location.X, input_file_txt.Location.Y - 14);
            mipmaps_label.Location = new Point(mipmaps_label.Location.X + 180, mipmaps_label.Location.Y - 14);
            mipmaps_txt.Location = new Point(mipmaps_txt.Location.X + 180, mipmaps_txt.Location.Y - 14);
            output_name_label.Location = new Point(output_name_label.Location.X + 165, output_name_label.Location.Y - 14);
            output_name_txt.Location = new Point(output_name_txt.Location.X + 165, output_name_txt.Location.Y - 14);
        }
        private void Layout_Paint()
        {
            if (!cmpr_layout_is_in_place)
            {
                Put_that_damn_cmpr_layout_in_place();
                cmpr_palette.BackgroundImage = gradient;
                cmpr_swap_ck.BackgroundImage = cmpr_swap;
                cmpr_swap2_ck.BackgroundImage = cmpr_swap2;
                cmpr_save_ck.BackgroundImage = cmpr_save;
                cmpr_save_as_ck.BackgroundImage = cmpr_save_as;
                cmpr_layout_is_in_place = true;
                cmpr_c1.Location = new Point(cmpr_c1.Location.X - 1920, cmpr_c1.Location.Y);
                cmpr_c1_txt.Location = new Point(cmpr_c1_txt.Location.X - 1920, cmpr_c1_txt.Location.Y);
                cmpr_c1_label.Location = new Point(cmpr_c1_label.Location.X - 1920, cmpr_c1_label.Location.Y);
                cmpr_c2.Location = new Point(cmpr_c2.Location.X - 1920, cmpr_c2.Location.Y);
                cmpr_c2_txt.Location = new Point(cmpr_c2_txt.Location.X - 1920, cmpr_c2_txt.Location.Y);
                cmpr_c2_label.Location = new Point(cmpr_c2_label.Location.X - 1920, cmpr_c2_label.Location.Y);
                cmpr_c3.Location = new Point(cmpr_c3.Location.X - 1920, cmpr_c3.Location.Y);
                cmpr_c3_txt.Location = new Point(cmpr_c3_txt.Location.X - 1920, cmpr_c3_txt.Location.Y);
                cmpr_c3_label.Location = new Point(cmpr_c3_label.Location.X - 1920, cmpr_c3_label.Location.Y);
                cmpr_c4.Location = new Point(cmpr_c4.Location.X - 1920, cmpr_c4.Location.Y);
                cmpr_c4_txt.Location = new Point(cmpr_c4_txt.Location.X - 1920, cmpr_c4_txt.Location.Y);
                cmpr_c4_label.Location = new Point(cmpr_c4_label.Location.X - 1920, cmpr_c4_label.Location.Y);
                cmpr_sel.Location = new Point(cmpr_sel.Location.X - 1920, cmpr_sel.Location.Y);
                cmpr_sel_label.Location = new Point(cmpr_sel_label.Location.X - 1920, cmpr_sel_label.Location.Y);
                cmpr_swap_ck.Location = new Point(cmpr_swap_ck.Location.X - 1920, cmpr_swap_ck.Location.Y);
                cmpr_swap_label.Location = new Point(cmpr_swap_label.Location.X - 1920, cmpr_swap_label.Location.Y);
                cmpr_selected_block_label.Location = new Point(cmpr_selected_block_label.Location.X - 1920, cmpr_selected_block_label.Location.Y);
                cmpr_picture_tooltip_label.Location = new Point(cmpr_picture_tooltip_label.Location.X - 1920, cmpr_picture_tooltip_label.Location.Y);
                cmpr_block_selection_ck.Location = new Point(cmpr_block_selection_ck.Location.X - 1920, cmpr_block_selection_ck.Location.Y);
                cmpr_block_selection_label.Location = new Point(cmpr_block_selection_label.Location.X - 1920, cmpr_block_selection_label.Location.Y);
                cmpr_block_paint_ck.Location = new Point(cmpr_block_paint_ck.Location.X - 1920, cmpr_block_paint_ck.Location.Y);
                cmpr_block_paint_label.Location = new Point(cmpr_block_paint_label.Location.X - 1920, cmpr_block_paint_label.Location.Y);
                cmpr_save_ck.Location = new Point(cmpr_save_ck.Location.X - 1920, cmpr_save_ck.Location.Y);
                cmpr_save_as_ck.Location = new Point(cmpr_save_as_ck.Location.X - 1920, cmpr_save_as_ck.Location.Y);
                cmpr_warning.Location = new Point(cmpr_warning.Location.X - 1920, cmpr_warning.Location.Y);
                cmpr_mouse1_label.Location = new Point(cmpr_mouse1_label.Location.X - 1920, cmpr_mouse1_label.Location.Y);
                cmpr_mouse2_label.Location = new Point(cmpr_mouse2_label.Location.X - 1920, cmpr_mouse2_label.Location.Y);
                cmpr_mouse3_label.Location = new Point(cmpr_mouse3_label.Location.X - 1920, cmpr_mouse3_label.Location.Y);
                cmpr_mouse4_label.Location = new Point(cmpr_mouse4_label.Location.X - 1920, cmpr_mouse4_label.Location.Y);
                cmpr_mouse5_label.Location = new Point(cmpr_mouse5_label.Location.X - 1920, cmpr_mouse5_label.Location.Y);
                cmpr_grid_ck.Location = new Point(cmpr_grid_ck.Location.X - 1920, cmpr_grid_ck.Location.Y);
                cmpr_hover_colour.Location = new Point(cmpr_hover_colour.Location.X - 1920, cmpr_hover_colour.Location.Y);
                cmpr_hover_colour_label.Location = new Point(cmpr_hover_colour_label.Location.X - 1920, cmpr_hover_colour_label.Location.Y);
                cmpr_hover_colour_txt.Location = new Point(cmpr_hover_colour_txt.Location.X - 1920, cmpr_hover_colour_txt.Location.Y);
                cmpr_swap2_ck.Location = new Point(cmpr_swap2_ck.Location.X - 1920, cmpr_swap2_ck.Location.Y);
                cmpr_swap2_label.Location = new Point(cmpr_swap2_label.Location.X - 1920, cmpr_swap2_label.Location.Y);
                cmpr_palette.Location = new Point(cmpr_palette.Location.X - 1920, cmpr_palette.Location.Y);
                cmpr_hover_ck.Location = new Point(cmpr_hover_ck.Location.X - 1920, cmpr_hover_ck.Location.Y);
                cmpr_hover_label.Location = new Point(cmpr_hover_label.Location.X - 1920, cmpr_hover_label.Location.Y);
                cmpr_preview_ck.Location = new Point(cmpr_preview_ck.Location.X - 1920, cmpr_preview_ck.Location.Y);
                cmpr_update_preview_ck.Location = new Point(cmpr_update_preview_ck.Location.X - 1920, cmpr_update_preview_ck.Location.Y);
                cmpr_update_preview_label.Location = new Point(cmpr_update_preview_label.Location.X - 1920, cmpr_update_preview_label.Location.Y);
                cmpr_colours_argb[8] = 255;
            }
            if (!cmpr_layout_is_enabled)
            {
                layout = 1;
                if (all_layout_is_enabled)
                    Disable_All_Layout();
                if (preview_layout_is_enabled)
                    Disable_Preview_Layout();
                Hide_mag();
                Hide_min();
                Hide_cmpr();
                Hide_rgba();
                Hide_WrapS();
                Hide_WrapT();
                Hide_options();
                Hide_encoding(1);
                Hide_encoding(5);
                Hide_encoding(8);
                view_algorithm = false;
                a_a_ck.Visible = false;
                a_b_ck.Visible = false;
                a_g_ck.Visible = false;
                a_r_ck.Visible = false;
                ai4_ck.Visible = false;
                ai4_label.Visible = false;
                ai8_ck.Visible = false;
                ai8_label.Visible = false;
                algorithm_label.Visible = false;
                b_a_ck.Visible = false;
                b_b_ck.Visible = false;
                b_g_ck.Visible = false;
                b_r_ck.Visible = false;
                bmd_ck.Visible = false;
                bmd_label.Visible = false;
                bmp_32_ck.Visible = false;
                bmp_32_label.Visible = false;
                bmp_ck.Visible = false;
                bmp_label.Visible = false;
                bti_ck.Visible = false;
                bti_label.Visible = false;
                ci14x2_ck.Visible = false;
                ci14x2_label.Visible = false;
                ci4_ck.Visible = false;
                ci4_label.Visible = false;
                ci8_ck.Visible = false;
                ci8_label.Visible = false;
                cie_601_ck.Visible = false;
                cie_601_label.Visible = false;
                cli_textbox_ck.Visible = false;
                cli_textbox_label.Visible = false;
                cmpr_block_paint_ck.Visible = true;
                cmpr_block_paint_label.Visible = true;
                cmpr_block_selection_ck.Visible = true;
                cmpr_block_selection_label.Visible = true;
                cmpr_c1.Visible = true;
                cmpr_c1_label.Visible = true;
                cmpr_c1_txt.Visible = true;
                cmpr_c2.Visible = true;
                cmpr_c2_label.Visible = true;
                cmpr_c2_txt.Visible = true;
                cmpr_c3.Visible = true;
                cmpr_c3_label.Visible = true;
                cmpr_c3_txt.Visible = true;
                cmpr_c4.Visible = true;
                cmpr_c4_label.Visible = true;
                cmpr_c4_txt.Visible = true;
                cmpr_ck.Visible = false;
                cmpr_grid_ck.Visible = true;
                cmpr_hover_ck.Visible = true;
                cmpr_hover_colour.Visible = true;
                cmpr_hover_colour_label.Visible = true;
                cmpr_hover_colour_txt.Visible = true;
                cmpr_hover_label.Visible = true;
                cmpr_label.Visible = false;
                cmpr_layout_is_enabled = true;
                cmpr_mouse1_label.Visible = true;
                cmpr_mouse2_label.Visible = true;
                cmpr_mouse3_label.Visible = true;
                cmpr_mouse4_label.Visible = true;
                cmpr_mouse5_label.Visible = true;
                cmpr_palette.Visible = true;
                cmpr_picture_tooltip_label.Visible = true;
                cmpr_preview_ck.Visible = true;
                cmpr_save_as_ck.Visible = true;
                cmpr_save_ck.Visible = true;
                cmpr_sel.Visible = true;
                cmpr_sel_label.Visible = true;
                cmpr_selected_block_label.Visible = true;
                cmpr_swap2_ck.Visible = true;
                cmpr_swap2_label.Visible = true;
                cmpr_swap_ck.Visible = true;
                cmpr_swap_label.Visible = true;
                cmpr_update_preview_ck.Visible = true;
                cmpr_update_preview_label.Visible = true;
                cmpr_warning.Visible = true;
                colour_channels_label.Visible = false;
                custom_ck.Visible = false;
                custom_label.Visible = false;
                encoding_label.Visible = false;
                funky_ck.Visible = false;
                funky_label.Visible = false;
                g_a_ck.Visible = false;
                g_b_ck.Visible = false;
                g_g_ck.Visible = false;
                g_r_ck.Visible = false;
                gif_ck.Visible = false;
                gif_label.Visible = false;
                i4_ck.Visible = false;
                i4_label.Visible = false;
                i8_ck.Visible = false;
                i8_label.Visible = false;
                ico_ck.Visible = false;
                ico_label.Visible = false;
                input_file2_label.Visible = false;
                input_file2_txt.Visible = false;
                jpeg_ck.Visible = false;
                jpeg_label.Visible = false;
                jpg_ck.Visible = false;
                jpg_label.Visible = false;
                mandatory_settings_label.Visible = false;
                name_string_ck.Visible = false;
                name_string_label.Visible = false;
                options_label.Visible = false;
                output_file_type_label.Visible = false;
                output_label.Visible = false;
                png_ck.Visible = false;
                png_label.Visible = false;
                r_a_ck.Visible = false;
                r_b_ck.Visible = false;
                r_g_ck.Visible = false;
                r_r_ck.Visible = false;
                random_ck.Visible = false;
                random_label.Visible = false;
                reversex_ck.Visible = false;
                reversex_label.Visible = false;
                reversey_ck.Visible = false;
                reversey_label.Visible = false;
                rgb565_ck.Visible = false;
                rgb565_label.Visible = false;
                rgb5a3_ck.Visible = false;
                rgb5a3_label.Visible = false;
                rgba32_ck.Visible = false;
                rgba32_label.Visible = false;
                run_ck.Visible = false;
                surrounding_ck.Visible = false;
                tex0_ck.Visible = false;
                tex0_label.Visible = false;
                tif_ck.Visible = false;
                tif_label.Visible = false;
                tiff_ck.Visible = false;
                tiff_label.Visible = false;
                tpl_ck.Visible = false;
                tpl_label.Visible = false;
                for (byte i = 0; i < 9; i++)
                {
                    desc[i].Location = new Point(desc[i].Location.X - 300, desc[i].Location.Y + 100);
                }
                description_title.Location = new Point(description_title.Location.X - 300, description_title.Location.Y + 100);
                description_surrounding.Location = new Point(description_surrounding.Location.X - 300, description_surrounding.Location.Y + 100);
                input_file_label.Location = new Point(input_file_label.Location.X, input_file_label.Location.Y + 14);
                input_file_txt.Location = new Point(input_file_txt.Location.X, input_file_txt.Location.Y + 14);
                mipmaps_label.Location = new Point(mipmaps_label.Location.X - 180, mipmaps_label.Location.Y + 14);
                mipmaps_txt.Location = new Point(mipmaps_txt.Location.X - 180, mipmaps_txt.Location.Y + 14);
                output_name_label.Location = new Point(output_name_label.Location.X - 165, output_name_label.Location.Y + 14);
                output_name_txt.Location = new Point(output_name_txt.Location.X - 165, output_name_txt.Location.Y + 14);
                layout = 3;
            }
            Check_Paint();
        }
        private void View_alpha(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            for (byte i = 0; i < alpha_ck_array.Count; i++)
            {
                alpha_ck_array[i].Visible = true;
            }
            no_alpha_label.Visible = true;
            alpha_label.Visible = true;
            alpha_title.Visible = true;
            mix_label.Visible = true;
            view_alpha = true;
        }
        private void Hide_alpha(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            //System.Windows.Forms.FormWindowState previous = this.WindowState;
            //this.WindowState = FormWindowState.Minimized;
            for (byte i = 0; i < alpha_ck_array.Count; i++)
            {
                alpha_ck_array[i].Visible = false;
            }
            alpha_label.Visible = false;
            no_alpha_label.Visible = false;
            alpha_title.Visible = false;
            mix_label.Visible = false;
            view_alpha = false;  // this doesn't update for some reason
            //this.WindowState = FormWindowState.Normal;
        }
        private void View_algorithm(byte algorithm, bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            /*if (algorithm < 2)
            {
                for (byte i = 0; i < 2; i++)
                {
                    algorithm_ck[i].Visible = true;
                }
                cie_601_hitbox.Visible = true;
                cie_709_hitbox.Visible = true;
                cie_601_label.Visible = true;
                cie_709_label.Visible = true;
            }*/
            if (algorithm == 2)
            {
                View_rgba();
            }
            else if (algorithm == 3)
            {
                View_No_Gradient();
            }
            else if (algorithm == 255)
            {
                for (byte i = 0; i < 3; i++)
                {
                    algorithm_ck[i].Visible = true;
                }
                // no_gradient_label.Visible = true;
                algorithm_label.Visible = true;
                cie_601_label.Visible = true;
                cie_709_label.Visible = true;
                custom_label.Visible = true;
                view_algorithm = true;
            }
        }
        private void Hide_algorithm(byte algorithm, bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            if (algorithm == 2)
            {
                Hide_rgba();
            }
            else if (algorithm == 3)
            {
                cmpr_max_label.Visible = true;
                cmpr_max_txt.Visible = true;
                cmpr_min_alpha_label.Visible = true;
                cmpr_min_alpha_txt.Visible = true;
            }
            else if (algorithm == 255)
            {
                for (byte i = 0; i < algorithm_ck.Count; i++)
                {
                    algorithm_ck[i].Visible = false;
                }
                no_gradient_label.Visible = false;
                algorithm_label.Visible = false;
                cie_601_label.Visible = false;
                cie_709_label.Visible = false;
                custom_label.Visible = false;
                view_algorithm = false;
            }
        }
        private void View_options(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            warn_label.Visible = true;
            FORCE_ALPHA_label.Visible = true;
            no_warning_label.Visible = true;
            stfu_label.Visible = true;
            safe_mode_label.Visible = true;
            warn_ck.Visible = true;
            FORCE_ALPHA_ck.Visible = true;
            no_warning_ck.Visible = true;
            stfu_ck.Visible = true;
            safe_mode_ck.Visible = true;
            view_options = true;
        }
        private void Hide_options(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            warn_label.Visible = false;
            FORCE_ALPHA_label.Visible = false;
            no_warning_label.Visible = false;
            stfu_label.Visible = false;
            safe_mode_label.Visible = false;
            warn_ck.Visible = false;
            FORCE_ALPHA_ck.Visible = false;
            no_warning_ck.Visible = false;
            stfu_ck.Visible = false;
            safe_mode_ck.Visible = false;
            view_options = false;
        }
        private void View_WrapS(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            for (byte i = 0; i < WrapS_ck.Count; i++)
            {
                WrapS_ck[i].Visible = true;
            }
            Sclamp_label.Visible = true;
            Srepeat_label.Visible = true;
            Smirror_label.Visible = true;
            WrapS_label.Visible = true;
            view_WrapS = true;
        }
        private void Hide_WrapS(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            for (byte i = 0; i < WrapS_ck.Count; i++)
            {
                WrapS_ck[i].Visible = false;
            }
            Sclamp_label.Visible = false;
            Srepeat_label.Visible = false;
            Smirror_label.Visible = false;
            WrapS_label.Visible = false;
            view_WrapS = false;
        }
        private void View_WrapT(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            for (byte i = 0; i < WrapT_ck.Count; i++)
            {
                WrapT_ck[i].Visible = true;
            }
            Tclamp_label.Visible = true;
            Trepeat_label.Visible = true;
            Tmirror_label.Visible = true;
            WrapT_label.Visible = true;
            view_WrapT = true;
        }
        private void Hide_WrapT(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            for (byte i = 0; i < WrapT_ck.Count; i++)
            {
                WrapT_ck[i].Visible = false;
            }
            Tclamp_label.Visible = false;
            Trepeat_label.Visible = false;
            Tmirror_label.Visible = false;
            WrapT_label.Visible = false;
            view_WrapT = false;
        }
        private void View_min(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            for (byte i = 0; i < minification_ck.Count; i++)
            {
                minification_ck[i].Visible = true;
            }
            min_linearmipmaplinear_label.Visible = true;
            min_linearmipmapnearest_label.Visible = true;
            min_linear_label.Visible = true;
            min_nearestmipmaplinear_label.Visible = true;
            min_nearestmipmapnearest_label.Visible = true;
            min_nearest_neighbour_label.Visible = true;
            minification_label.Visible = true;
            view_min = true;
        }
        private void Hide_min(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            for (byte i = 0; i < minification_ck.Count; i++)
            {
                minification_ck[i].Visible = false;
            }
            min_linearmipmaplinear_label.Visible = false;
            min_linearmipmapnearest_label.Visible = false;
            min_linear_label.Visible = false;
            min_nearestmipmaplinear_label.Visible = false;
            min_nearestmipmapnearest_label.Visible = false;
            min_nearest_neighbour_label.Visible = false;
            minification_label.Visible = false;
            view_min = false;
        }
        private void View_mag(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            for (byte i = 0; i < magnification_ck.Count; i++)
            {
                magnification_ck[i].Visible = true;
            }
            mag_linearmipmaplinear_label.Visible = true;
            mag_linearmipmapnearest_label.Visible = true;
            mag_linear_label.Visible = true;
            mag_nearestmipmaplinear_label.Visible = true;
            mag_nearestmipmapnearest_label.Visible = true;
            mag_nearest_neighbour_label.Visible = true;
            magnification_label.Visible = true;
            view_mag = true;
        }
        private void Hide_mag(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            for (byte i = 0; i < magnification_ck.Count; i++)
            {
                magnification_ck[i].Visible = false;
            }
            mag_linearmipmaplinear_label.Visible = false;
            mag_linearmipmapnearest_label.Visible = false;
            mag_linear_label.Visible = false;
            mag_nearestmipmaplinear_label.Visible = false;
            mag_nearestmipmapnearest_label.Visible = false;
            mag_nearest_neighbour_label.Visible = false;
            magnification_label.Visible = false;
            view_mag = false;
        }
        private void View_diversity()
        {
            diversity_label.Visible = true;
            diversity_txt.Visible = true;
            diversity2_label.Visible = true;
            diversity2_txt.Visible = true;
            percentage2_label.Visible = true;
            percentage2_txt.Visible = true;
            percentage_label.Visible = true;
            percentage_txt.Visible = true;
        }
        private void Hide_diversity()
        {
            diversity_label.Visible = false;
            diversity_txt.Visible = false;
            diversity2_label.Visible = false;
            diversity2_txt.Visible = false;
            percentage2_label.Visible = false;
            percentage2_txt.Visible = false;
            percentage_label.Visible = false;
            percentage_txt.Visible = false;
        }
        private void View_cmpr(bool secret_mode = false)
        {
            cie_601_label.Text = "Default";
            cie_709_label.Text = "Range Fit";
            darkest_lightest_label.Text = "Darkest/Lightest";
            custom_label.Text = "Most Used/Furthest";
            Hide_alpha(true);
            cie_709_ck.Visible = true;
            cie_709_label.Visible = true;
            darkest_lightest_ck.Visible = true;
            darkest_lightest_label.Visible = true;
            weemm_ck.Visible = true;
            weemm_label.Visible = true;
            no_gradient_ck.Visible = true;
            no_gradient_label.Visible = true;
            sooperbmd_ck.Visible = true;
            sooperbmd_label.Visible = true;
            min_max_ck.Visible = true;
            min_max_label.Visible = true;
            //cie_601_label.Text = "Darkest/Lightest";
            //cie_601_label.Text = "No Gradient";
            //cie_601_label.Text = "Default";
            if (layout != 1 && !secret_mode)
                return;
            View_diversity();
            cmpr_max_label.Visible = true;
            cmpr_max_txt.Visible = true;
            cmpr_min_alpha_label.Visible = true;
            cmpr_min_alpha_txt.Visible = true;
            round5_label.Visible = true;
            round5_txt.Visible = true;
            round6_label.Visible = true;
            round6_txt.Visible = true;
            view_cmpr = true;
        }
        private void Hide_cmpr(bool secret_mode = false, bool just_change_list = false)
        {
            cie_709_label.Text = "CIE 709";
            custom_label.Text = "Custom RGBA";
            if (layout != 1)
                View_alpha(true);
            if (secret_mode)
            {
                cie_709_ck.Visible = false;
                cie_709_label.Visible = false;
            }
            weemm_ck.Visible = false;
            weemm_label.Visible = false;
            no_gradient_ck.Visible = false;
            no_gradient_label.Visible = false;
            sooperbmd_ck.Visible = false;
            sooperbmd_label.Visible = false;
            min_max_ck.Visible = false;
            min_max_label.Visible = false;
            if (just_change_list)
                return;
            if (layout != 1 && !secret_mode)
                return;
            Hide_diversity();
            darkest_lightest_ck.Visible = false;
            darkest_lightest_label.Visible = false;
            no_gradient_ck.Visible = false;
            no_gradient_label.Visible = false;
            cmpr_max_label.Visible = false;
            cmpr_max_txt.Visible = false;
            cmpr_min_alpha_label.Visible = false;
            cmpr_min_alpha_txt.Visible = false;
            round5_label.Visible = false;
            round5_txt.Visible = false;
            round6_label.Visible = false;
            round6_txt.Visible = false;
            view_cmpr = false;
        }
        private void View_palette(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            View_diversity();
            palette_label.Visible = true;
            palette_ai8_label.Visible = true;
            palette_ai8_ck.Visible = true;
            palette_rgb565_ck.Visible = true;
            palette_rgb565_label.Visible = true;
            palette_rgb5a3_ck.Visible = true;
            palette_rgb5a3_label.Visible = true;
            num_colours_label.Visible = true;
            num_colours_txt.Visible = true;
            view_palette = true;
        }
        private void Hide_palette(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            Hide_diversity();
            palette_label.Visible = false;
            palette_ai8_label.Visible = false;
            palette_ai8_ck.Visible = false;
            palette_rgb565_ck.Visible = false;
            palette_rgb565_label.Visible = false;
            palette_rgb5a3_ck.Visible = false;
            palette_rgb5a3_label.Visible = false;
            num_colours_label.Visible = false;
            num_colours_txt.Visible = false;
            view_palette = false;
        }
        private void View_No_Gradient()
        {
            if (layout != 1)
                return;
            cmpr_max_label.Visible = false;
            cmpr_max_txt.Visible = false;
            cmpr_min_alpha_label.Visible = false;
            cmpr_min_alpha_txt.Visible = false;
        }
        private void View_rgba(bool secret_mode = false)
        {
            if ((layout != 1 && !secret_mode) || (encoding == 14 && !secret_mode))
                return;
            custom_rgba_label.Visible = true;
            custom_r_label.Visible = true;
            custom_r_txt.Visible = true;
            custom_g_label.Visible = true;
            custom_g_txt.Visible = true;
            custom_b_txt.Visible = true;
            custom_b_label.Visible = true;
            custom_a_txt.Visible = true;
            custom_a_label.Visible = true;
            view_rgba = true;
        }
        private void Hide_rgba(bool secret_mode = false)
        {
            if (layout != 1 && !secret_mode)
                return;
            custom_rgba_label.Visible = false;
            custom_r_label.Visible = false;
            custom_r_txt.Visible = false;
            custom_g_label.Visible = false;
            custom_g_txt.Visible = false;
            custom_b_txt.Visible = false;
            custom_b_label.Visible = false;
            custom_a_txt.Visible = false;
            custom_a_label.Visible = false;
            view_rgba = false;
        }
        private void View_i4()
        {
            cie_601_label.Text = "CIE 601";
            darkest_lightest_label.Text = "Gamma";
            if (layout != 1)
                return;
            darkest_lightest_ck.Visible = true;
            darkest_lightest_label.Visible = true;
            cie_709_ck.Visible = true;
            cie_709_label.Visible = true;
            round4_label.Visible = true;
            round4_txt.Visible = true;
            //view_rgba = true;
        }
        private void View_i8()
        {
            cie_601_label.Text = "CIE 601";
            darkest_lightest_label.Text = "Gamma";
            if (layout != 1)
                return;
            darkest_lightest_ck.Visible = true;
            darkest_lightest_label.Visible = true;
            cie_709_ck.Visible = true;
            cie_709_label.Visible = true;
            //view_rgba = true;
        }
        private void View_ai4()
        {
            cie_601_label.Text = "CIE 601";
            darkest_lightest_label.Text = "Gamma";
            if (layout != 1)
                return;
            darkest_lightest_ck.Visible = true;
            darkest_lightest_label.Visible = true;
            cie_709_ck.Visible = true;
            cie_709_label.Visible = true;
            round4_label.Visible = true;
            round4_txt.Visible = true;
            //view_rgba = true;
        }
        private void View_ai8()
        {
            cie_601_label.Text = "CIE 601";
            darkest_lightest_label.Text = "Gamma";
            if (layout != 1)
                return;
            darkest_lightest_ck.Visible = true;
            darkest_lightest_label.Visible = true;
            cie_709_ck.Visible = true;
            cie_709_label.Visible = true;
            //view_rgba = true;
        }
        private void View_rgb565()
        {
            cie_601_label.Text = "Default";
            if (layout != 1)
                return;
            round5_label.Visible = true;
            round5_txt.Visible = true;
            round6_label.Visible = true;
            round6_txt.Visible = true;
            //view_rgba = true;
        }
        private void View_rgb5a3()
        {
            cie_601_label.Text = "Default";
            if (layout != 1)
                return;
            View_alpha();
            round3_label.Visible = true;
            round3_txt.Visible = true;
            round4_label.Visible = true;
            round4_txt.Visible = true;
            round5_label.Visible = true;
            round5_txt.Visible = true;
            round6_label.Visible = true;
            round6_txt.Visible = true;
            //view_rgba = true;
        }
        private void View_rgba32()
        {
            cie_601_label.Text = "Default";
            //view_rgba = true;
            if (layout != 1)
                return;
        }
        private void View_ci4()
        {
            cie_601_label.Text = "Default";
            if (layout != 1)
                return;
            View_palette();
            //view_rgba = true;
        }
        private void View_ci8()
        {
            cie_601_label.Text = "Default";
            if (layout != 1)
                return;
            View_palette();
            //view_rgba = true;
        }
        private void View_ci14x2()
        {
            cie_601_label.Text = "Default";
            if (layout != 1)
                return;
            View_palette();
            //view_rgba = true;
        }
        private void Hide_encoding(byte encoding)
        {
            if (layout != 1)
            {
                if (encoding == 14)
                    Hide_cmpr(false, true);
                return;
            }
            switch (encoding)
            {
                case 0:
                case 2:
                    cie_709_ck.Visible = false;
                    cie_709_label.Visible = false;
                    darkest_lightest_ck.Visible = false;
                    darkest_lightest_label.Visible = false;
                    round4_label.Visible = false;
                    round4_txt.Visible = false;
                    break;
                case 1:
                case 3:
                    cie_709_ck.Visible = false;
                    cie_709_label.Visible = false;
                    darkest_lightest_ck.Visible = false;
                    darkest_lightest_label.Visible = false;
                    break;
                case 4:
                    round5_label.Visible = false;
                    round5_txt.Visible = false;
                    round6_label.Visible = false;
                    round6_txt.Visible = false;
                    break;
                case 5:
                    Hide_alpha(false);
                    round3_label.Visible = false;
                    round3_txt.Visible = false;
                    round4_label.Visible = false;
                    round4_txt.Visible = false;
                    round5_label.Visible = false;
                    round5_txt.Visible = false;
                    round6_label.Visible = false;
                    round6_txt.Visible = false;
                    break;
                case 8:
                case 9:
                case 10:
                    Hide_palette();
                    break;
                case 14:
                    Hide_cmpr(true);
                    break;
            }
        }
        private void Easter_Egg()
        {
            if (!view_algorithm)
                Category_unchecked(view_algorithm_ck);
            if (!view_alpha)
                Category_unchecked(view_alpha_ck);
            if (!view_cmpr)
                Category_unchecked(view_cmpr_ck);
            if (!view_palette)
                Category_unchecked(view_palette_ck);
            if (!view_min)
                Category_unchecked(view_min_ck);
            if (!view_mag)
                Category_unchecked(view_mag_ck);
            if (!view_rgba)
                Category_unchecked(view_rgba_ck);
            if (!view_options)
                Category_unchecked(view_options_ck);
            if (!view_WrapS)
                Category_unchecked(view_WrapS_ck);
            if (!view_WrapT)
                Category_unchecked(view_WrapT_ck);
            view_algorithm_ck.Visible = true;
            view_alpha_ck.Visible = true;
            view_cmpr_ck.Visible = true;
            view_palette_ck.Visible = true;
            view_min_ck.Visible = true;
            view_mag_ck.Visible = true;
            view_rgba_ck.Visible = true;
            view_options_ck.Visible = true;
            view_WrapS_ck.Visible = true;
            view_WrapT_ck.Visible = true;
            view_algorithm_label.Visible = true;
            view_alpha_label.Visible = true;
            view_cmpr_label.Visible = true;
            view_palette_label.Visible = true;
            view_min_label.Visible = true;
            view_mag_label.Visible = true;
            view_rgba_label.Visible = true;
            view_options_label.Visible = true;
            view_WrapS_label.Visible = true;
            view_WrapT_label.Visible = true;
        }
        private void Uncheck_Arrow()
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
                case 5:
                    unchecked_Arrow_1080p();
                    break;
                case 10:
                    Maximized_Click(null, null);
                    break;
                case 14:
                    unchecked_Screen2_Left();
                    break;
                case 17:
                    unchecked_Screen2_Top_left();
                    break;
                case 18:
                    unchecked_Screen2_Top();
                    break;
                case 19:
                    unchecked_Screen2_Top_right();
                    break;
                case 16:
                    unchecked_Screen2_Right();
                    break;
                case 13:
                    unchecked_Screen2_Bottom_right();
                    break;
                case 12:
                    unchecked_Screen2_Bottom();
                    break;
                case 11:
                    unchecked_Screen2_Bottom_left();
                    break;
                case 15:
                    unchecked_Screen2_Arrow_1080p();
                    break;
            }
        }
        private void parse_rgb565(Label lab, TextBox txt, byte j, out ushort out_colour, ushort default_colour)
        {
            success = false;
            len = txt.Text.Length;
            for (byte i = 1; i < len; i++)
            {
                if (!ishex(txt.Text[i]))
                    txt.Text = txt.Text.Substring(0, i);
            }
            if (len > 0)
                if (!ishex(txt.Text[0]) && txt.Text[0] != '#')
                    txt.Text = "";
            if (len > 6)
            {
                if (txt.Text[0] != '#')
                    txt.Text = txt.Text.Substring(0, 6);
                else if (len > 7)
                    txt.Text = txt.Text.Substring(0, 7);

            }
            if (len < 3)
            {
                out_colour = default_colour;
                return;
            }
            if (txt.Text[0] == '#' && len > 3)
            {
                if (len == 4)
                {
                    byte.TryParse(txt.Text.Substring(1, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out red);
                    byte.TryParse(txt.Text.Substring(2, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out green);
                    byte.TryParse(txt.Text.Substring(3, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out blue);
                    red <<= 4;
                    green <<= 4;
                    blue <<= 4;
                    success = true;
                }
                else if (len == 7)
                {
                    byte.TryParse(txt.Text.Substring(1, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out red);
                    byte.TryParse(txt.Text.Substring(3, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out green);
                    byte.TryParse(txt.Text.Substring(5, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out blue);
                    if ((red & 7) != 0)
                    {
                        red &= 0xf8;
                        Warn_rgb565_colour_trim();
                    }
                    if ((green & 3) != 0)
                    {
                        green &= 0xfc;
                        Warn_rgb565_colour_trim();
                    }
                    if ((blue & 7) != 0)
                    {
                        blue &= 0xf8;
                        Warn_rgb565_colour_trim();
                    }
                    success = true;
                }
            }
            else
            {
                if (len == 3)
                {
                    byte.TryParse(txt.Text.Substring(0, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out red);
                    byte.TryParse(txt.Text.Substring(1, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out green);
                    byte.TryParse(txt.Text.Substring(2, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out blue);
                    red <<= 4;
                    green <<= 4;
                    blue <<= 4;
                    success = true;
                }
                else if (len == 6)
                {
                    byte.TryParse(txt.Text.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out red);
                    byte.TryParse(txt.Text.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out green);
                    byte.TryParse(txt.Text.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out blue);
                    if ((red & 7) != 0)
                    {
                        red &= 0xf8;
                        Warn_rgb565_colour_trim();
                    }
                    if ((green & 3) != 0)
                    {
                        green &= 0xfc;
                        Warn_rgb565_colour_trim();
                    }
                    if ((blue & 7) != 0)
                    {
                        blue &= 0xf8;
                        Warn_rgb565_colour_trim();
                    }
                    success = true;
                }
            }
            if (success)
            {
                cmpr_colour[j] = (byte)((red & 0xf8) + (green >> 5));
                cmpr_colour[j + 1] = (byte)(((green << 3) & 224) + (blue >> 3));
                lab.BackColor = Color.FromArgb(255, red, green, blue);
                cmpr_colours_argb[(j << 1)] = 255;
                cmpr_colours_argb[(j << 1) + 1] = red;
                cmpr_colours_argb[(j << 1) + 2] = green;
                cmpr_colours_argb[(j << 1) + 3] = blue;
                if (len > 5)
                {
                    cmpr_colours_hex = BitConverter.ToString(cmpr_colours_argb).Replace("-", string.Empty);
                    txt.Text = cmpr_colours_hex.Substring((j << 2) + 2, 6);
                }
                out_colour = (ushort)((cmpr_colour[j] << 8) | (cmpr_colour[j + 1]));
                if (the_program_is_loading_a_cmpr_block)
                    Update_Colours(false);
                else
                    Update_Colours(true);
            }
            else
                out_colour = default_colour;

        }
        private void parse_rgba_hover(Label lab, TextBox txt)
        {
            success = false;
            alpha2 = 255;
            len = txt.Text.Length;
            for (byte i = 1; i < len; i++)
            {
                if (!ishex(txt.Text[i]))
                    txt.Text = txt.Text.Substring(0, i);
            }
            if (len > 0)
                if (!ishex(txt.Text[0]) && txt.Text[0] != '#')
                    txt.Text = "";
            if (len > 8)
            {
                if (txt.Text[0] != '#')
                    txt.Text = txt.Text.Substring(0, 8);
                else if (len > 9)
                    txt.Text = txt.Text.Substring(0, 9);

            }
            if (len < 3)
            {
                return;
            }
            if (txt.Text[0] == '#' && len > 3)
            {
                if (len == 4)
                {
                    byte.TryParse(txt.Text.Substring(1, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out red);
                    byte.TryParse(txt.Text.Substring(2, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out green);
                    byte.TryParse(txt.Text.Substring(3, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out blue);
                    red <<= 4;
                    green <<= 4;
                    blue <<= 4;
                    success = true;
                }
                if (len == 5)
                {
                    byte.TryParse(txt.Text.Substring(1, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out red);
                    byte.TryParse(txt.Text.Substring(2, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out green);
                    byte.TryParse(txt.Text.Substring(3, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out blue);
                    byte.TryParse(txt.Text.Substring(4, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out alpha2);
                    red <<= 4;
                    green <<= 4;
                    blue <<= 4;
                    alpha2 <<= 4;
                    success = true;
                }
                else if (len == 7)
                {
                    byte.TryParse(txt.Text.Substring(1, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out red);
                    byte.TryParse(txt.Text.Substring(3, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out green);
                    byte.TryParse(txt.Text.Substring(5, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out blue);
                    success = true;
                }
                else if (len == 9)
                {
                    byte.TryParse(txt.Text.Substring(1, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out red);
                    byte.TryParse(txt.Text.Substring(3, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out green);
                    byte.TryParse(txt.Text.Substring(5, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out blue);
                    byte.TryParse(txt.Text.Substring(7, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out alpha2);
                    success = true;
                }
            }
            else
            {
                if (len == 3)
                {
                    byte.TryParse(txt.Text.Substring(0, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out red);
                    byte.TryParse(txt.Text.Substring(1, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out green);
                    byte.TryParse(txt.Text.Substring(2, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out blue);
                    red <<= 4;
                    green <<= 4;
                    blue <<= 4;
                    success = true;
                }
                if (len == 4)
                {
                    byte.TryParse(txt.Text.Substring(0, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out red);
                    byte.TryParse(txt.Text.Substring(1, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out green);
                    byte.TryParse(txt.Text.Substring(2, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out blue);
                    byte.TryParse(txt.Text.Substring(3, 1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out alpha2);
                    red <<= 4;
                    green <<= 4;
                    blue <<= 4;
                    alpha2 <<= 4;
                    success = true;
                }
                else if (len == 6)
                {
                    byte.TryParse(txt.Text.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out red);
                    byte.TryParse(txt.Text.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out green);
                    byte.TryParse(txt.Text.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out blue);
                    success = true;
                }
                else if (len == 8)
                {
                    byte.TryParse(txt.Text.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out red);
                    byte.TryParse(txt.Text.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out green);
                    byte.TryParse(txt.Text.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out blue);
                    byte.TryParse(txt.Text.Substring(6, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out alpha2);
                    success = true;
                }
            }
            if (success)
            {
                lab.BackColor = Color.FromArgb(alpha2, red, green, blue);
                colour_4[0] = red;
                colour_4[1] = green;
                colour_4[2] = blue;
                colour_4[3] = alpha2;
                cmpr_hover_red = red;
                cmpr_hover_green = green;
                cmpr_hover_blue = blue;
                cmpr_hover_alpha = alpha2;
                if (len > 7)
                {
                    cmpr_colours_hex = BitConverter.ToString(colour_4).Replace("-", string.Empty);
                    txt.Text = cmpr_colours_hex.Substring(0, 8);
                }
            }
        }
        private void Update_Colours(bool user_changed_the_colour_pusposefully = false)
        {
            if (loaded_block != -1 && user_changed_the_colour_pusposefully)
            {
                cmpr_file[cmpr_data_start_offset + (loaded_block << 3)] = (byte)(colour1 >> 8);
                cmpr_file[cmpr_data_start_offset + (loaded_block << 3) + 1] = (byte)(colour1);
                cmpr_file[cmpr_data_start_offset + (loaded_block << 3) + 2] = (byte)(colour2 >> 8);
                cmpr_file[cmpr_data_start_offset + (loaded_block << 3) + 3] = (byte)(colour2);
            }
            if (colour1 > colour2)
            {
                /*red = cmpr_colours_argb[1];
                green = cmpr_colours_argb[2];
                blue = cmpr_colours_argb[3];

                red2 = cmpr_colours_argb[5];
                green2 = cmpr_colours_argb[6];
                blue2 = cmpr_colours_argb[7];*/

                cmpr_colours_argb[9] = (byte)((cmpr_colours_argb[1] * 2 / 3) + (cmpr_colours_argb[5] / 3));
                cmpr_colours_argb[10] = (byte)((cmpr_colours_argb[2] * 2 / 3) + (cmpr_colours_argb[6] / 3));
                cmpr_colours_argb[11] = (byte)((cmpr_colours_argb[3] * 2 / 3) + (cmpr_colours_argb[7] / 3));
                //colour3 = (ushort)((((cmpr_colours_argb[9]) >> 3) << 11) + ((cmpr_colours_argb[10] >> 2) << 5) + (cmpr_colours_argb[11] >> 3)); // the RGB565 third colour
                cmpr_c3.BackColor = Color.FromArgb(255, cmpr_colours_argb[9], cmpr_colours_argb[10], cmpr_colours_argb[11]);
                cmpr_colours_argb[12] = 255;
                cmpr_colours_argb[13] = (byte)((cmpr_colours_argb[1] / 3) + (cmpr_colours_argb[5] * 2 / 3));
                cmpr_colours_argb[14] = (byte)((cmpr_colours_argb[2] / 3) + (cmpr_colours_argb[6] * 2 / 3));
                cmpr_colours_argb[15] = (byte)((cmpr_colours_argb[3] / 3) + (cmpr_colours_argb[7] * 2 / 3));
                //colour4 = (ushort)((((cmpr_colours_argb[13]) >> 3) << 11) + ((cmpr_colours_argb[14] >> 2) << 5) + (cmpr_colours_argb[15] >> 3)); // the RGB565 fourth colour
                cmpr_c4.BackColor = Color.FromArgb(255, cmpr_colours_argb[13], cmpr_colours_argb[14], cmpr_colours_argb[15]);
                cmpr_colours_hex = BitConverter.ToString(cmpr_colours_argb).Replace("-", string.Empty);
                cmpr_c3_txt.Text = cmpr_colours_hex.Substring(18, 6);
                cmpr_c4_txt.Text = cmpr_colours_hex.Substring(26, 6);
            }
            else
            {
                // of course, that's the exact opposite! - not quite lol
                cmpr_colours_argb[9] = (byte)((cmpr_colours_argb[1] / 2) + (cmpr_colours_argb[5] / 2));
                cmpr_colours_argb[10] = (byte)((cmpr_colours_argb[2] / 2) + (cmpr_colours_argb[6] / 2));
                cmpr_colours_argb[11] = (byte)((cmpr_colours_argb[3] / 2) + (cmpr_colours_argb[7] / 2));
                //colour3 = (ushort)((((cmpr_colours_argb[9]) >> 3) << 11) + ((cmpr_colours_argb[10] >> 2) << 5) + (cmpr_colours_argb[11] >> 3)); // the RGB565 third colour
                cmpr_c3.BackColor = Color.FromArgb(255, cmpr_colours_argb[9], cmpr_colours_argb[10], cmpr_colours_argb[11]);
                cmpr_colours_argb[12] = 0;
                cmpr_c4.BackColor = Color.FromArgb(0, 0, 0, 0); // fourth colour is fully transparent
                cmpr_colours_hex = BitConverter.ToString(cmpr_colours_argb).Replace("-", string.Empty);
                cmpr_c3_txt.Text = cmpr_colours_hex.Substring(18, 6);
                cmpr_c4_txt.Text = "";
                // last colour isn't in the palette, it's in _plt0.alpha_bitfield
            }
            cmpr_sel.BackColor = Color.FromArgb(cmpr_colours_argb[(cmpr_selected_colour << 2) - 4], cmpr_colours_argb[(cmpr_selected_colour << 2) - 3], cmpr_colours_argb[(cmpr_selected_colour << 2) - 2], cmpr_colours_argb[(cmpr_selected_colour << 2) - 1]);
            if (cmpr_swap2_enabled)
            {
                for (ushort x = 0; x < 256; x += 64)
                {
                    for (ushort y = 0; y < 256; y += 64)
                    {
                        cmpr_index_i = (byte)(cmpr_index[(x >> 6) + (y >> 4)] + 1);
                        if (cmpr_index_i == 1)
                            Paint_Pixel(x, y, 2, false);
                        else if (cmpr_index_i == 2)
                            Paint_Pixel(x, y, 1, false);
                        else
                            Paint_Pixel(x, y, cmpr_index_i, false);
                    }
                }
            }
            else
                for (ushort x = 0; x < 256; x += 64)
                    for (ushort y = 0; y < 256; y += 64)
                        Paint_Pixel(x, y, (byte)(cmpr_index[(x >> 6) + (y >> 4)] + 1), false);
            if (cmpr_update_preview)
                Preview_Paint();
        }
        private void Paint_Pixel(int x, int y, byte button, bool called_outside_update_colours = true)  // note: X and Y are height and width WITHIN the picturebox
        {
            x >>= 6;
            y >>= 6;
            if (x > 3 || y > 3 || x < 0 || y < 0)
                return;
            cmpr_4x4[170 + (x << 2) - (y << 4)] = cmpr_colours_argb[(button << 2) - 1];  // B
            cmpr_4x4[171 + (x << 2) - (y << 4)] = cmpr_colours_argb[(button << 2) - 2];  // G
            cmpr_4x4[172 + (x << 2) - (y << 4)] = cmpr_colours_argb[(button << 2) - 3];  // R
            cmpr_4x4[173 + (x << 2) - (y << 4)] = cmpr_colours_argb[(button << 2) - 4];  // A
            //cmpr_grid[index].BackColor = Color.FromArgb(cmpr_colours_argb[(button << 2) - 4], cmpr_colours_argb[(button << 2) - 3], cmpr_colours_argb[(button << 2) - 2], cmpr_colours_argb[(button << 2) - 1]);
            cmpr_index[x + (y << 2)] = (byte)(button - 1);
            cmpr_grid_ck.Image = GetImageFromByteArray(cmpr_4x4);
            if (cmpr_file == null || loaded_block == -1)  // the default value when no blocks are selected was making the program write into the header >_<
                return;
            cmpr_file[cmpr_data_start_offset + (loaded_block << 3) + 4 + y] &= (byte)(0xff ^ (3 << (6 - (x << 1)))); // voids the previous index
            cmpr_file[cmpr_data_start_offset + (loaded_block << 3) + 4 + y] += (byte)((button - 1) << (6 - (x << 1))); // replaces it with the new one
            // change that because the first byte of a bmp is at the last line :P
            // also X + Y doesn't work because 0, 1 = 1, 0 lol
            if (called_outside_update_colours)
                Preview_Paint();
        }
        /// <summary>
        /// Method that uses the ImageConverter object in .Net Framework to convert a byte array, 
        /// presumably containing a JPEG or PNG file image, into a Bitmap object, which can also be 
        /// used as an Image object.
        /// </summary>
        /// <param name="byteArray">byte array containing JPEG or PNG file image or similar</param>
        /// <returns>Bitmap object if it works, else exception is thrown</returns>
        public static Bitmap GetImageFromByteArray(byte[] byteArray)
        {
            Bitmap bm = (Bitmap)_imageConverter.ConvertFrom(byteArray);

            if (bm != null && (bm.HorizontalResolution != (int)bm.HorizontalResolution ||
                               bm.VerticalResolution != (int)bm.VerticalResolution))
            {
                // Correct a strange glitch that has been observed in the test program when converting 
                //  from a PNG file image created by CopyImageToByteArray() - the dpi value "drifts" 
                //  slightly away from the nominal integer value
                bm.SetResolution((int)(bm.HorizontalResolution + 0.5f),
                                 (int)(bm.VerticalResolution + 0.5f));
            }

            return bm;
        }
        private void Change_mipmap()
        {
            // you can deal directly with the "mipmaps" variable
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

        public void SetAllControlsFont(System.Windows.Forms.Control.ControlCollection ctrls)
        {
            foreach (Control c in ctrls)
            {
                if (c.Controls != null)
                {
                    SetAllControlsFont(c.Controls);
                }
                c.Font = new Font(config[6], c.Font.Size, c.Font.Style, c.Font.Unit, GdiCharSet);
                c.ForeColor = Color.FromName(config[10]);
            }
        }
        private void Load_settings()
        {
            banner_f11_ck.BackgroundImage = maximized_off;
            banner_1_ck.BackgroundImage = bottom_left_off;
            banner_2_ck.BackgroundImage = bottom_off;
            banner_3_ck.BackgroundImage = bottom_right_off;
            banner_4_ck.BackgroundImage = left_off;
            banner_5_ck.BackgroundImage = arrow_1080p_off;
            banner_6_ck.BackgroundImage = right_off;
            banner_7_ck.BackgroundImage = top_left_off;
            banner_8_ck.BackgroundImage = top_off;
            banner_9_ck.BackgroundImage = top_right_off;
            banner_11_ck.BackgroundImage = screen2_bottom_left_off;
            banner_12_ck.BackgroundImage = screen2_bottom_off;
            banner_13_ck.BackgroundImage = screen2_bottom_right_off;
            banner_14_ck.BackgroundImage = screen2_left_off;
            banner_15_ck.BackgroundImage = screen2_arrow_1080p_off;
            banner_16_ck.BackgroundImage = screen2_right_off;
            banner_17_ck.BackgroundImage = screen2_top_left_off;
            banner_18_ck.BackgroundImage = screen2_top_off;
            banner_19_ck.BackgroundImage = screen2_top_right_off;
            if (System.IO.File.Exists(execPath + "images/settings.txt"))
            {
                string version = "";
                settings = System.IO.File.ReadAllLines(execPath + "images/settings.txt");
                if (settings.Length > 0)
                {
                    version = config[0].Substring(12);
                }

                switch (version)
                {
                    case "v1.0":
                        if (version == "v1.0" && settings.Length < 200)  // incorrect v1.0 config file
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
                switch (config[2].ToUpper())
                {
                    case "ALL":
                        checked_All();
                        unchecked_Auto();
                        unchecked_Preview();
                        unchecked_Paint();
                        Layout_All();
                        break;
                    case "AUTO":
                        unchecked_All();
                        checked_Auto();
                        unchecked_Preview();
                        unchecked_Paint();
                        Layout_Auto();
                        break;
                    case "PREVIEW":
                        unchecked_All();
                        unchecked_Auto();
                        checked_Preview();
                        unchecked_Paint();
                        Layout_Preview();
                        break;
                    case "PAINT":
                        unchecked_All();
                        unchecked_Auto();
                        unchecked_Preview();
                        checked_Paint();
                        Layout_Paint();
                        break;
                }
                switch (config[4].ToUpper())
                {
                    case "MAXIMIZED":
                        this.WindowState = FormWindowState.Maximized;
                        banner_f11_ck.BackgroundImage = maximized_on;
                        arrow = 10;
                        break;
                    case "NORMAL":
                        arrow = 0;
                        // default
                        break;
                    case "LEFT":
                        arrow = 4;
                        banner_4_ck.BackgroundImage = left_on;
                        break;
                    case "TOP_LEFT":
                        arrow = 7;
                        banner_7_ck.BackgroundImage = top_left_on;
                        break;
                    case "TOP":
                        arrow = 8;
                        banner_8_ck.BackgroundImage = top_on;
                        break;
                    case "TOP_RIGHT":
                        arrow = 9;
                        banner_9_ck.BackgroundImage = top_right_on;
                        break;
                    case "RIGHT":
                        arrow = 6;
                        banner_6_ck.BackgroundImage = right_on;
                        break;
                    case "BOTTOM_RIGHT":
                        arrow = 3;
                        banner_3_ck.BackgroundImage = bottom_right_on;
                        break;
                    case "BOTTOM":
                        arrow = 2;
                        banner_2_ck.BackgroundImage = bottom_on;
                        break;
                    case "BOTTOM_LEFT":
                        arrow = 1;
                        banner_1_ck.BackgroundImage = bottom_left_on;
                        break;
                    case "1080P":
                        arrow = 5;
                        banner_5_ck.BackgroundImage = arrow_1080p_on;
                        break;
                    case "SCREEN2_LEFT":
                        arrow = 14;
                        banner_14_ck.BackgroundImage = screen2_left_on;
                        break;
                    case "SCREEN2_TOP_LEFT":
                        arrow = 17;
                        banner_17_ck.BackgroundImage = screen2_top_left_on;
                        break;
                    case "SCREEN2_TOP":
                        arrow = 18;
                        banner_18_ck.BackgroundImage = screen2_top_on;
                        break;
                    case "SCREEN2_TOP_RIGHT":
                        arrow = 19;
                        banner_19_ck.BackgroundImage = screen2_top_right_on;
                        break;
                    case "SCREEN2_RIGHT":
                        arrow = 16;
                        banner_16_ck.BackgroundImage = screen2_right_on;
                        break;
                    case "SCREEN2_BOTTOM_RIGHT":
                        arrow = 13;
                        banner_13_ck.BackgroundImage = screen2_bottom_right_on;
                        break;
                    case "SCREEN2_BOTTOM":
                        arrow = 12;
                        banner_12_ck.BackgroundImage = screen2_bottom_on;
                        break;
                    case "SCREEN2_BOTTOM_LEFT":
                        arrow = 11;
                        banner_11_ck.BackgroundImage = screen2_bottom_left_on;
                        break;
                    case "SCREEN2_1080P":
                        arrow = 15;
                        banner_15_ck.BackgroundImage = screen2_arrow_1080p_on;
                        break;
                }
                SetAllControlsFont((ControlCollection)this.Controls);
                byte.TryParse(config[8], out GdiCharSet);

                input_file_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                input_file2_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                output_name_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                mipmaps_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                cmpr_max_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                cmpr_min_alpha_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                num_colours_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                round3_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                round4_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                round5_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                round6_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                diversity_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                diversity2_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                percentage_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                percentage2_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                custom_r_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                custom_g_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                custom_b_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                custom_a_txt.ForeColor = System.Drawing.Color.FromName(config[12]);
                cmpr_edited_colour_txt.BackColor = System.Drawing.Color.FromName(config[12]);
                cmpr_hover_colour_txt.BackColor = System.Drawing.Color.FromName(config[12]);
                cmpr_c1_txt.BackColor = System.Drawing.Color.FromName(config[12]);
                cmpr_c2_txt.BackColor = System.Drawing.Color.FromName(config[12]);
                cmpr_c3_txt.BackColor = System.Drawing.Color.FromName(config[12]);
                cmpr_c4_txt.BackColor = System.Drawing.Color.FromName(config[12]);

                input_file_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                input_file2_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                output_name_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                mipmaps_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                cmpr_max_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                cmpr_min_alpha_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                num_colours_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                round3_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                round4_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                round5_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                round6_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                diversity_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                diversity2_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                percentage_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                percentage2_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                custom_r_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                custom_g_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                custom_b_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                custom_a_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                cmpr_edited_colour_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                cmpr_hover_colour_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                cmpr_c1_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                cmpr_c2_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                cmpr_c3_txt.BackColor = System.Drawing.Color.FromName(config[14]);
                cmpr_c4_txt.BackColor = System.Drawing.Color.FromName(config[14]);

                description_title.ForeColor = System.Drawing.Color.FromName(config[16]);
                description.ForeColor = System.Drawing.Color.FromName(config[16]);

                cmpr_hover_colour.BackColor = Color.FromName(config[18]);

                if (config[70].ToLower() == "true")
                {
                    checked_checkbox(banner_global_move_ck);
                }
                if (config[72].ToLower() == "true")
                {
                    checked_checkbox(textchange_ck);
                }
                if (config[74].ToLower() == "true")
                {
                    checked_checkbox(auto_update_ck);
                }
                if (config[76].ToLower() == "true")
                {
                    checked_checkbox(upscale_ck);
                }
                if (config[78].ToLower() == "true")
                {
                    checked_checkbox(cmpr_hover_ck);
                }
                if (config[80].ToLower() == "true")
                {
                    checked_checkbox(cmpr_update_preview_ck);
                }
                if (config[82].ToLower() == "true")
                {
                    checked_checkbox(bmd_ck);
                }
                if (config[84].ToLower() == "true")
                {
                    checked_checkbox(bti_ck);
                }
                if (config[86].ToLower() == "true")
                {
                    checked_checkbox(tex0_ck);
                }
                if (config[88].ToLower() == "true")
                {
                    checked_checkbox(tpl_ck);
                }
                if (config[90].ToLower() == "true")
                {
                    checked_checkbox(bmp_ck);
                }
                if (config[92].ToLower() == "true")
                {
                    checked_checkbox(png_ck);
                }
                if (config[94].ToLower() == "true")
                {
                    checked_checkbox(jpg_ck);
                }
                if (config[96].ToLower() == "true")
                {
                    checked_checkbox(jpeg_ck);
                }
                if (config[98].ToLower() == "true")
                {
                    checked_checkbox(gif_ck);
                }
                if (config[100].ToLower() == "true")
                {
                    checked_checkbox(ico_ck);
                }
                if (config[102].ToLower() == "true")
                {
                    checked_checkbox(tif_ck);
                }
                if (config[104].ToLower() == "true")
                {
                    checked_checkbox(tiff_ck);
                }
                if (config[106].ToLower() == "true")
                {
                    checked_checkbox(ask_exit_ck);
                }
                if (config[108].ToLower() == "true")
                {
                    checked_checkbox(bmp_32_ck);
                }
                if (config[110].ToLower() == "true")
                {
                    checked_checkbox(FORCE_ALPHA_ck);
                }
                if (config[112].ToLower() == "true")
                {
                    checked_checkbox(funky_ck);
                }
                if (config[114].ToLower() == "true")
                {
                    checked_checkbox(name_string_ck);
                }
                if (config[116].ToLower() == "true")
                {
                    checked_checkbox(random_ck);
                }
                if (config[118].ToLower() == "true")
                {
                    checked_checkbox(reversex_ck);
                }
                if (config[120].ToLower() == "true")
                {
                    checked_checkbox(reversey_ck);
                }
                if (config[122].ToLower() == "true")
                {
                    checked_checkbox(safe_mode_ck);
                }
                if (config[124].ToLower() == "true")
                {
                    checked_checkbox(stfu_ck);
                }
                if (config[126].ToLower() == "true")
                {
                    checked_checkbox(warn_ck);
                }
                Apply_Graphics();
            }
            else
            {
                try
                {
                    string[] new_lines = { "plt0 config v1.0", "Layout (change next line with one of these: \"All\", \"Auto\", \"Preview\", \"Paint\")", "All", "Window Location (\"Normal\", \"Maximized\", \"Bottom_left\", \"Left\", \"Top_left\", \"Top\", \"Top_right\", \"Right\", \"Bottom_right\", \"Bottom\")", "Maximized", "Textboxes text color (System.Drawing.Color)", "White", "Textboxes color", "Black", "Description color", "Cyan", "Font name (it must be installed on your system)", "Segoe UI", "Encoding number (internally named GdiCharSet, see https://docs.microsoft.com/en-us/dotnet/api/system.drawing.font.gdicharset?view=dotnet-plat-ext-6.0)", "128", "CMPR Hover Colour", "White", "CMPR Edited Colour", "Transparent" };
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
        }
        private void Fill_Lists()
        {
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
            algorithm_ck.Add(darkest_lightest_ck);
            algorithm_ck.Add(no_gradient_ck);
            algorithm_ck.Add(weemm_ck);
            algorithm_ck.Add(sooperbmd_ck);
            algorithm_ck.Add(min_max_ck);
            algorithm_ck.Add(cie_601_ck);  // nothing
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
        }
        private void Apply_Graphics()
        {
            //if ()
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
            unchecked_checkbox(reversex_ck);
            unchecked_checkbox(reversey_ck);
            unchecked_checkbox(funky_ck);
            unchecked_checkbox(no_warning_ck);
            unchecked_checkbox(gif_ck);
            unchecked_checkbox(textchange_ck);
            unchecked_checkbox(name_string_ck);
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
            checked_checkbox(cmpr_hover_ck);
            checked_checkbox(auto_update_ck);
            checked_checkbox(upscale_ck);
            checked_checkbox(cmpr_update_preview_ck);
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
            unchecked_algorithm(darkest_lightest_ck);
            unchecked_algorithm(no_gradient_ck);
            unchecked_algorithm(weemm_ck);
            unchecked_algorithm(sooperbmd_ck);
            unchecked_algorithm(min_max_ck);
            Category_checked(view_alpha_ck);
            Category_checked(view_algorithm_ck);
            Category_checked(view_WrapS_ck);
            Category_checked(view_WrapT_ck);
            Category_checked(view_min_ck);
            Category_checked(view_mag_ck);
            Category_checked(view_cmpr_ck);
            Category_checked(view_rgba_ck);
            Category_checked(view_options_ck);
            Category_checked(view_palette_ck);
            unchecked_palette(palette_ai8_ck);
            unchecked_palette(palette_rgb565_ck);
            unchecked_palette(palette_rgb5a3_ck);
        }
        private void InitializeForm(bool load_settings_dot_tee_ekks_tee = true, bool this_is_the_first_time_this_function_is_called = true)
        {
            Load_Images();
            if (this_is_the_first_time_this_function_is_called)
                Fill_Lists();
            if (load_settings_dot_tee_ekks_tee)
                Load_settings();
            // note:
            this.image_ck.Location = new System.Drawing.Point(815, 96);
            no_gradient_ck.Location = new System.Drawing.Point(500, 384);
            no_gradient_label.Location = new System.Drawing.Point(564, 384);
            weemm_ck.Location = new System.Drawing.Point(500, 448);
            weemm_label.Location = new System.Drawing.Point(564, 448);
            sooperbmd_ck.Location = new System.Drawing.Point(500, 512);
            sooperbmd_label.Location = new System.Drawing.Point(564, 512);
            min_max_ck.Location = new System.Drawing.Point(500, 576);
            min_max_label.Location = new System.Drawing.Point(564, 576);
            bool delete_preview = true;
            if (Directory.Exists(execPath + "images/preview"))
            {
                try
                {
                    string[] files = Directory.GetFiles(execPath + "images/preview");
                    for (int i = 0; i < files.Length; i++)
                        File.Delete(files[i]);
                    Directory.Delete(execPath + "images/preview");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Substring(0, 34) == "The process cannot access the file")  // because it is being used by another process
                    {
                        delete_preview = false;
                    }
                }
            }
            if (delete_preview)
                Directory.CreateDirectory(execPath + "images/preview");
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
            sync_preview_ck.BackgroundImage = sync_preview_off;
            banner_global_move_ck.BackgroundImage = banner_global_move_off;
            this.BackgroundImage = background;
            //
            // NativeMethods.AllocConsole();
        }
        // the whole code below is generated by something else than me typing on my keyboard in Visual Studio
        // actually not the whole code, but let's pretend I haven't typed 15k lines
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(plt0_gui));
            this.output_file_type_label = new System.Windows.Forms.Label();
            this.mandatory_settings_label = new System.Windows.Forms.Label();
            this.bmd_label = new System.Windows.Forms.Label();
            this.bti_ck = new System.Windows.Forms.PictureBox();
            this.bti_label = new System.Windows.Forms.Label();
            this.tex0_ck = new System.Windows.Forms.PictureBox();
            this.tex0_label = new System.Windows.Forms.Label();
            this.tpl_ck = new System.Windows.Forms.PictureBox();
            this.tpl_label = new System.Windows.Forms.Label();
            this.bmp_ck = new System.Windows.Forms.PictureBox();
            this.bmp_label = new System.Windows.Forms.Label();
            this.png_ck = new System.Windows.Forms.PictureBox();
            this.png_label = new System.Windows.Forms.Label();
            this.jpg_ck = new System.Windows.Forms.PictureBox();
            this.jpg_label = new System.Windows.Forms.Label();
            this.tiff_ck = new System.Windows.Forms.PictureBox();
            this.tiff_label = new System.Windows.Forms.Label();
            this.tif_ck = new System.Windows.Forms.PictureBox();
            this.tif_label = new System.Windows.Forms.Label();
            this.ico_ck = new System.Windows.Forms.PictureBox();
            this.ico_label = new System.Windows.Forms.Label();
            this.gif_ck = new System.Windows.Forms.PictureBox();
            this.gif_label = new System.Windows.Forms.Label();
            this.jpeg_ck = new System.Windows.Forms.PictureBox();
            this.jpeg_label = new System.Windows.Forms.Label();
            this.options_label = new System.Windows.Forms.Label();
            this.warn_ck = new System.Windows.Forms.PictureBox();
            this.warn_label = new System.Windows.Forms.Label();
            this.stfu_ck = new System.Windows.Forms.PictureBox();
            this.stfu_label = new System.Windows.Forms.Label();
            this.safe_mode_ck = new System.Windows.Forms.PictureBox();
            this.safe_mode_label = new System.Windows.Forms.Label();
            this.reversey_ck = new System.Windows.Forms.PictureBox();
            this.reversey_label = new System.Windows.Forms.Label();
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
            this.cmpr_ck = new System.Windows.Forms.PictureBox();
            this.cmpr_label = new System.Windows.Forms.Label();
            this.ci14x2_ck = new System.Windows.Forms.PictureBox();
            this.ci14x2_label = new System.Windows.Forms.Label();
            this.ci8_ck = new System.Windows.Forms.PictureBox();
            this.ci8_label = new System.Windows.Forms.Label();
            this.ci4_ck = new System.Windows.Forms.PictureBox();
            this.ci4_label = new System.Windows.Forms.Label();
            this.rgba32_ck = new System.Windows.Forms.PictureBox();
            this.rgba32_label = new System.Windows.Forms.Label();
            this.rgb5a3_ck = new System.Windows.Forms.PictureBox();
            this.rgb5a3_label = new System.Windows.Forms.Label();
            this.rgb565_ck = new System.Windows.Forms.PictureBox();
            this.rgb565_label = new System.Windows.Forms.Label();
            this.ai8_ck = new System.Windows.Forms.PictureBox();
            this.ai8_label = new System.Windows.Forms.Label();
            this.ai4_ck = new System.Windows.Forms.PictureBox();
            this.ai4_label = new System.Windows.Forms.Label();
            this.i8_ck = new System.Windows.Forms.PictureBox();
            this.i8_label = new System.Windows.Forms.Label();
            this.i4_ck = new System.Windows.Forms.PictureBox();
            this.i4_label = new System.Windows.Forms.Label();
            this.encoding_label = new System.Windows.Forms.Label();
            this.surrounding_ck = new System.Windows.Forms.PictureBox();
            this.no_gradient_ck = new System.Windows.Forms.PictureBox();
            this.no_gradient_label = new System.Windows.Forms.Label();
            this.custom_ck = new System.Windows.Forms.PictureBox();
            this.custom_label = new System.Windows.Forms.Label();
            this.cie_709_ck = new System.Windows.Forms.PictureBox();
            this.cie_709_label = new System.Windows.Forms.Label();
            this.cie_601_ck = new System.Windows.Forms.PictureBox();
            this.cie_601_label = new System.Windows.Forms.Label();
            this.algorithm_label = new System.Windows.Forms.Label();
            this.mix_ck = new System.Windows.Forms.PictureBox();
            this.mix_label = new System.Windows.Forms.Label();
            this.alpha_ck = new System.Windows.Forms.PictureBox();
            this.alpha_label = new System.Windows.Forms.Label();
            this.no_alpha_ck = new System.Windows.Forms.PictureBox();
            this.no_alpha_label = new System.Windows.Forms.Label();
            this.alpha_title = new System.Windows.Forms.Label();
            this.Tmirror_ck = new System.Windows.Forms.PictureBox();
            this.Tmirror_label = new System.Windows.Forms.Label();
            this.Trepeat_ck = new System.Windows.Forms.PictureBox();
            this.Trepeat_label = new System.Windows.Forms.Label();
            this.Tclamp_ck = new System.Windows.Forms.PictureBox();
            this.Tclamp_label = new System.Windows.Forms.Label();
            this.WrapT_label = new System.Windows.Forms.Label();
            this.Smirror_ck = new System.Windows.Forms.PictureBox();
            this.Smirror_label = new System.Windows.Forms.Label();
            this.Srepeat_ck = new System.Windows.Forms.PictureBox();
            this.Srepeat_label = new System.Windows.Forms.Label();
            this.Sclamp_ck = new System.Windows.Forms.PictureBox();
            this.Sclamp_label = new System.Windows.Forms.Label();
            this.WrapS_label = new System.Windows.Forms.Label();
            this.magnification_label = new System.Windows.Forms.Label();
            this.minification_label = new System.Windows.Forms.Label();
            this.min_linearmipmaplinear_ck = new System.Windows.Forms.PictureBox();
            this.min_linearmipmaplinear_label = new System.Windows.Forms.Label();
            this.min_linearmipmapnearest_ck = new System.Windows.Forms.PictureBox();
            this.min_linearmipmapnearest_label = new System.Windows.Forms.Label();
            this.min_nearestmipmaplinear_ck = new System.Windows.Forms.PictureBox();
            this.min_nearestmipmaplinear_label = new System.Windows.Forms.Label();
            this.min_nearestmipmapnearest_ck = new System.Windows.Forms.PictureBox();
            this.min_nearestmipmapnearest_label = new System.Windows.Forms.Label();
            this.min_linear_ck = new System.Windows.Forms.PictureBox();
            this.min_linear_label = new System.Windows.Forms.Label();
            this.min_nearest_neighbour_ck = new System.Windows.Forms.PictureBox();
            this.min_nearest_neighbour_label = new System.Windows.Forms.Label();
            this.mag_linearmipmaplinear_ck = new System.Windows.Forms.PictureBox();
            this.mag_linearmipmaplinear_label = new System.Windows.Forms.Label();
            this.mag_linearmipmapnearest_ck = new System.Windows.Forms.PictureBox();
            this.mag_linearmipmapnearest_label = new System.Windows.Forms.Label();
            this.mag_nearestmipmaplinear_ck = new System.Windows.Forms.PictureBox();
            this.mag_nearestmipmaplinear_label = new System.Windows.Forms.Label();
            this.mag_nearestmipmapnearest_ck = new System.Windows.Forms.PictureBox();
            this.mag_nearestmipmapnearest_label = new System.Windows.Forms.Label();
            this.mag_linear_ck = new System.Windows.Forms.PictureBox();
            this.mag_linear_label = new System.Windows.Forms.Label();
            this.mag_nearest_neighbour_ck = new System.Windows.Forms.PictureBox();
            this.mag_nearest_neighbour_label = new System.Windows.Forms.Label();
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
            this.view_alpha_ck = new System.Windows.Forms.PictureBox();
            this.view_alpha_label = new System.Windows.Forms.Label();
            this.view_algorithm_ck = new System.Windows.Forms.PictureBox();
            this.view_algorithm_label = new System.Windows.Forms.Label();
            this.view_WrapS_ck = new System.Windows.Forms.PictureBox();
            this.view_WrapS_label = new System.Windows.Forms.Label();
            this.view_WrapT_ck = new System.Windows.Forms.PictureBox();
            this.view_WrapT_label = new System.Windows.Forms.Label();
            this.view_mag_ck = new System.Windows.Forms.PictureBox();
            this.view_mag_label = new System.Windows.Forms.Label();
            this.view_min_ck = new System.Windows.Forms.PictureBox();
            this.view_min_label = new System.Windows.Forms.Label();
            this.banner_ck = new System.Windows.Forms.PictureBox();
            this.all_ck = new System.Windows.Forms.PictureBox();
            this.preview_ck = new System.Windows.Forms.PictureBox();
            this.auto_ck = new System.Windows.Forms.PictureBox();
            this.paint_ck = new System.Windows.Forms.PictureBox();
            this.banner_x_ck = new System.Windows.Forms.PictureBox();
            this.banner_f11_ck = new System.Windows.Forms.PictureBox();
            this.banner_minus_ck = new System.Windows.Forms.PictureBox();
            this.banner_9_ck = new System.Windows.Forms.PictureBox();
            this.banner_8_ck = new System.Windows.Forms.PictureBox();
            this.banner_7_ck = new System.Windows.Forms.PictureBox();
            this.banner_6_ck = new System.Windows.Forms.PictureBox();
            this.banner_4_ck = new System.Windows.Forms.PictureBox();
            this.banner_3_ck = new System.Windows.Forms.PictureBox();
            this.banner_2_ck = new System.Windows.Forms.PictureBox();
            this.banner_1_ck = new System.Windows.Forms.PictureBox();
            this.cli_textbox_ck = new System.Windows.Forms.PictureBox();
            this.run_ck = new System.Windows.Forms.PictureBox();
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
            this.palette_rgb565_ck = new System.Windows.Forms.PictureBox();
            this.palette_rgb565_label = new System.Windows.Forms.Label();
            this.palette_ai8_ck = new System.Windows.Forms.PictureBox();
            this.palette_ai8_label = new System.Windows.Forms.Label();
            this.palette_label = new System.Windows.Forms.Label();
            this.github_ck = new System.Windows.Forms.PictureBox();
            this.youtube_ck = new System.Windows.Forms.PictureBox();
            this.discord_ck = new System.Windows.Forms.PictureBox();
            this.desc2 = new System.Windows.Forms.Label();
            this.desc3 = new System.Windows.Forms.Label();
            this.desc4 = new System.Windows.Forms.Label();
            this.desc5 = new System.Windows.Forms.Label();
            this.desc6 = new System.Windows.Forms.Label();
            this.desc7 = new System.Windows.Forms.Label();
            this.desc8 = new System.Windows.Forms.Label();
            this.desc9 = new System.Windows.Forms.Label();
            this.output_label = new System.Windows.Forms.Label();
            this.version_ck = new System.Windows.Forms.PictureBox();
            this.cli_textbox_label = new System.Windows.Forms.Label();
            this.view_rgba_ck = new System.Windows.Forms.PictureBox();
            this.view_rgba_label = new System.Windows.Forms.Label();
            this.view_palette_ck = new System.Windows.Forms.PictureBox();
            this.view_palette_label = new System.Windows.Forms.Label();
            this.view_cmpr_ck = new System.Windows.Forms.PictureBox();
            this.view_cmpr_label = new System.Windows.Forms.Label();
            this.view_options_ck = new System.Windows.Forms.PictureBox();
            this.view_options_label = new System.Windows.Forms.Label();
            this.banner_resize = new System.Windows.Forms.Label();
            this.textchange_ck = new System.Windows.Forms.PictureBox();
            this.textchange_label = new System.Windows.Forms.Label();
            this.auto_update_ck = new System.Windows.Forms.PictureBox();
            this.auto_update_label = new System.Windows.Forms.Label();
            this.sync_preview_ck = new System.Windows.Forms.PictureBox();
            this.upscale_ck = new System.Windows.Forms.PictureBox();
            this.upscale_label = new System.Windows.Forms.Label();
            this.banner_move = new System.Windows.Forms.Label();
            this.preview4k_label = new System.Windows.Forms.Label();
            this.preview4k_ck = new System.Windows.Forms.PictureBox();
            this.reversex_ck = new System.Windows.Forms.PictureBox();
            this.reversex_label = new System.Windows.Forms.Label();
            this.cmpr_c1_label = new System.Windows.Forms.Label();
            this.cmpr_c1_txt = new System.Windows.Forms.TextBox();
            this.cmpr_c1 = new System.Windows.Forms.Label();
            this.cmpr_c2 = new System.Windows.Forms.Label();
            this.cmpr_c2_label = new System.Windows.Forms.Label();
            this.cmpr_c2_txt = new System.Windows.Forms.TextBox();
            this.cmpr_c3 = new System.Windows.Forms.Label();
            this.cmpr_c3_label = new System.Windows.Forms.Label();
            this.cmpr_c3_txt = new System.Windows.Forms.TextBox();
            this.cmpr_c4 = new System.Windows.Forms.Label();
            this.cmpr_c4_label = new System.Windows.Forms.Label();
            this.cmpr_c4_txt = new System.Windows.Forms.TextBox();
            this.cmpr_swap_ck = new System.Windows.Forms.PictureBox();
            this.cmpr_swap_label = new System.Windows.Forms.Label();
            this.cmpr_block_paint_ck = new System.Windows.Forms.PictureBox();
            this.cmpr_block_paint_label = new System.Windows.Forms.Label();
            this.cmpr_block_selection_ck = new System.Windows.Forms.PictureBox();
            this.cmpr_block_selection_label = new System.Windows.Forms.Label();
            this.cmpr_picture_tooltip_label = new System.Windows.Forms.Label();
            this.cmpr_selected_block_label = new System.Windows.Forms.Label();
            this.cmpr_save_ck = new System.Windows.Forms.PictureBox();
            this.cmpr_save_as_ck = new System.Windows.Forms.PictureBox();
            this.cmpr_warning = new System.Windows.Forms.Label();
            this.cmpr_sel_label = new System.Windows.Forms.Label();
            this.cmpr_mouse1_label = new System.Windows.Forms.Label();
            this.cmpr_mouse2_label = new System.Windows.Forms.Label();
            this.cmpr_mouse4_label = new System.Windows.Forms.Label();
            this.cmpr_mouse3_label = new System.Windows.Forms.Label();
            this.cmpr_mouse5_label = new System.Windows.Forms.Label();
            this.cmpr_sel = new System.Windows.Forms.Label();
            this.bmd_ck = new System.Windows.Forms.PictureBox();
            this.cmpr_hover_colour = new System.Windows.Forms.Label();
            this.cmpr_hover_colour_label = new System.Windows.Forms.Label();
            this.cmpr_hover_colour_txt = new System.Windows.Forms.TextBox();
            this.cmpr_edited_colour = new System.Windows.Forms.Label();
            this.cmpr_edited_colour_label = new System.Windows.Forms.Label();
            this.cmpr_edited_colour_txt = new System.Windows.Forms.TextBox();
            this.name_string_ck = new System.Windows.Forms.PictureBox();
            this.name_string_label = new System.Windows.Forms.Label();
            this.cmpr_swap2_ck = new System.Windows.Forms.PictureBox();
            this.cmpr_swap2_label = new System.Windows.Forms.Label();
            this.cmpr_hover_ck = new System.Windows.Forms.PictureBox();
            this.cmpr_hover_label = new System.Windows.Forms.Label();
            this.cmpr_update_preview_ck = new System.Windows.Forms.PictureBox();
            this.cmpr_update_preview_label = new System.Windows.Forms.Label();
            this.banner_global_move_ck = new System.Windows.Forms.PictureBox();
            this.banner_5_ck = new System.Windows.Forms.PictureBox();
            this.banner_11_ck = new System.Windows.Forms.PictureBox();
            this.banner_12_ck = new System.Windows.Forms.PictureBox();
            this.banner_13_ck = new System.Windows.Forms.PictureBox();
            this.banner_14_ck = new System.Windows.Forms.PictureBox();
            this.banner_16_ck = new System.Windows.Forms.PictureBox();
            this.banner_17_ck = new System.Windows.Forms.PictureBox();
            this.banner_18_ck = new System.Windows.Forms.PictureBox();
            this.banner_19_ck = new System.Windows.Forms.PictureBox();
            this.banner_15_ck = new System.Windows.Forms.PictureBox();
            this.sooperbmd_label = new System.Windows.Forms.Label();
            this.min_max_label = new System.Windows.Forms.Label();
            this.weemm_label = new System.Windows.Forms.Label();
            this.min_max_ck = new System.Windows.Forms.PictureBox();
            this.sooperbmd_ck = new System.Windows.Forms.PictureBox();
            this.weemm_ck = new System.Windows.Forms.PictureBox();
            this.darkest_lightest_label = new System.Windows.Forms.Label();
            this.darkest_lightest_ck = new System.Windows.Forms.PictureBox();
            this.cmpr_palette = new PictureBoxWithInterpolationMode();
            this.cmpr_grid_ck = new PictureBoxWithInterpolationMode();
            this.cmpr_preview_ck = new PictureBoxWithInterpolationMode();
            this.image_ck = new PictureBoxWithInterpolationMode();
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
            ((System.ComponentModel.ISupportInitialize)(this.reversey_ck)).BeginInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.banner_f11_ck)).BeginInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.view_options_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textchange_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.auto_update_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sync_preview_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upscale_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview4k_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reversex_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_swap_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_block_paint_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_block_selection_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_save_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_save_as_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bmd_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.name_string_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_swap2_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_hover_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_update_preview_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_global_move_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_5_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_11_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_12_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_13_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_14_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_16_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_17_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_18_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_19_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_15_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_max_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sooperbmd_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weemm_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.darkest_lightest_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_palette)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_grid_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_preview_ck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.image_ck)).BeginInit();
            this.SuspendLayout();
            // 
            // output_file_type_label
            // 
            this.output_file_type_label.AutoSize = true;
            this.output_file_type_label.BackColor = System.Drawing.Color.Transparent;
            this.output_file_type_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.output_file_type_label.ForeColor = System.Drawing.SystemColors.Control;
            this.output_file_type_label.Location = new System.Drawing.Point(36, 95);
            this.output_file_type_label.Margin = new System.Windows.Forms.Padding(0);
            this.output_file_type_label.Name = "output_file_type_label";
            this.output_file_type_label.Size = new System.Drawing.Size(168, 24);
            this.output_file_type_label.TabIndex = 0;
            this.output_file_type_label.Text = "Output file type";
            // 
            // mandatory_settings_label
            // 
            this.mandatory_settings_label.AutoSize = true;
            this.mandatory_settings_label.BackColor = System.Drawing.Color.Transparent;
            this.mandatory_settings_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.mandatory_settings_label.ForeColor = System.Drawing.SystemColors.Control;
            this.mandatory_settings_label.Location = new System.Drawing.Point(119, 54);
            this.mandatory_settings_label.Name = "mandatory_settings_label";
            this.mandatory_settings_label.Size = new System.Drawing.Size(209, 24);
            this.mandatory_settings_label.TabIndex = 123;
            this.mandatory_settings_label.Text = "Mandatory Settings";
            // 
            // bmd_label
            // 
            this.bmd_label.AutoSize = true;
            this.bmd_label.BackColor = System.Drawing.Color.Transparent;
            this.bmd_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.bmd_label.ForeColor = System.Drawing.SystemColors.Window;
            this.bmd_label.Location = new System.Drawing.Point(104, 128);
            this.bmd_label.Margin = new System.Windows.Forms.Padding(0);
            this.bmd_label.Name = "bmd_label";
            this.bmd_label.Padding = new System.Windows.Forms.Padding(0, 22, 20, 22);
            this.bmd_label.Size = new System.Drawing.Size(77, 64);
            this.bmd_label.TabIndex = 124;
            this.bmd_label.Text = "bmd";
            this.bmd_label.Click += new System.EventHandler(this.bmd_Click);
            this.bmd_label.MouseEnter += new System.EventHandler(this.bmd_MouseEnter);
            this.bmd_label.MouseLeave += new System.EventHandler(this.bmd_MouseLeave);
            // 
            // bti_ck
            // 
            this.bti_ck.BackColor = System.Drawing.Color.Transparent;
            this.bti_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bti_ck.ErrorImage = null;
            this.bti_ck.InitialImage = null;
            this.bti_ck.Location = new System.Drawing.Point(40, 192);
            this.bti_ck.Margin = new System.Windows.Forms.Padding(0);
            this.bti_ck.Name = "bti_ck";
            this.bti_ck.Size = new System.Drawing.Size(64, 64);
            this.bti_ck.TabIndex = 129;
            this.bti_ck.TabStop = false;
            this.bti_ck.Click += new System.EventHandler(this.bti_Click);
            this.bti_ck.MouseEnter += new System.EventHandler(this.bti_MouseEnter);
            this.bti_ck.MouseLeave += new System.EventHandler(this.bti_MouseLeave);
            // 
            // bti_label
            // 
            this.bti_label.AutoSize = true;
            this.bti_label.BackColor = System.Drawing.Color.Transparent;
            this.bti_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.bti_label.ForeColor = System.Drawing.SystemColors.Window;
            this.bti_label.Location = new System.Drawing.Point(104, 192);
            this.bti_label.Margin = new System.Windows.Forms.Padding(0);
            this.bti_label.Name = "bti_label";
            this.bti_label.Padding = new System.Windows.Forms.Padding(0, 22, 40, 22);
            this.bti_label.Size = new System.Drawing.Size(78, 68);
            this.bti_label.TabIndex = 127;
            this.bti_label.Text = "bti";
            this.bti_label.Click += new System.EventHandler(this.bti_Click);
            this.bti_label.MouseEnter += new System.EventHandler(this.bti_MouseEnter);
            this.bti_label.MouseLeave += new System.EventHandler(this.bti_MouseLeave);
            // 
            // tex0_ck
            // 
            this.tex0_ck.BackColor = System.Drawing.Color.Transparent;
            this.tex0_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tex0_ck.ErrorImage = null;
            this.tex0_ck.InitialImage = null;
            this.tex0_ck.Location = new System.Drawing.Point(40, 256);
            this.tex0_ck.Margin = new System.Windows.Forms.Padding(0);
            this.tex0_ck.Name = "tex0_ck";
            this.tex0_ck.Size = new System.Drawing.Size(64, 64);
            this.tex0_ck.TabIndex = 132;
            this.tex0_ck.TabStop = false;
            this.tex0_ck.Click += new System.EventHandler(this.tex0_Click);
            this.tex0_ck.MouseEnter += new System.EventHandler(this.tex0_MouseEnter);
            this.tex0_ck.MouseLeave += new System.EventHandler(this.tex0_MouseLeave);
            // 
            // tex0_label
            // 
            this.tex0_label.AutoSize = true;
            this.tex0_label.BackColor = System.Drawing.Color.Transparent;
            this.tex0_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tex0_label.ForeColor = System.Drawing.SystemColors.Window;
            this.tex0_label.Location = new System.Drawing.Point(104, 257);
            this.tex0_label.Margin = new System.Windows.Forms.Padding(0);
            this.tex0_label.Name = "tex0_label";
            this.tex0_label.Padding = new System.Windows.Forms.Padding(0, 22, 20, 22);
            this.tex0_label.Size = new System.Drawing.Size(74, 68);
            this.tex0_label.TabIndex = 130;
            this.tex0_label.Text = "tex0";
            this.tex0_label.Click += new System.EventHandler(this.tex0_Click);
            this.tex0_label.MouseEnter += new System.EventHandler(this.tex0_MouseEnter);
            this.tex0_label.MouseLeave += new System.EventHandler(this.tex0_MouseLeave);
            // 
            // tpl_ck
            // 
            this.tpl_ck.BackColor = System.Drawing.Color.Transparent;
            this.tpl_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tpl_ck.ErrorImage = null;
            this.tpl_ck.InitialImage = null;
            this.tpl_ck.Location = new System.Drawing.Point(40, 320);
            this.tpl_ck.Margin = new System.Windows.Forms.Padding(0);
            this.tpl_ck.Name = "tpl_ck";
            this.tpl_ck.Size = new System.Drawing.Size(64, 64);
            this.tpl_ck.TabIndex = 135;
            this.tpl_ck.TabStop = false;
            this.tpl_ck.Click += new System.EventHandler(this.tpl_Click);
            this.tpl_ck.MouseEnter += new System.EventHandler(this.tpl_MouseEnter);
            this.tpl_ck.MouseLeave += new System.EventHandler(this.tpl_MouseLeave);
            // 
            // tpl_label
            // 
            this.tpl_label.AutoSize = true;
            this.tpl_label.BackColor = System.Drawing.Color.Transparent;
            this.tpl_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tpl_label.ForeColor = System.Drawing.SystemColors.Window;
            this.tpl_label.Location = new System.Drawing.Point(104, 321);
            this.tpl_label.Margin = new System.Windows.Forms.Padding(0);
            this.tpl_label.Name = "tpl_label";
            this.tpl_label.Padding = new System.Windows.Forms.Padding(0, 22, 40, 22);
            this.tpl_label.Size = new System.Drawing.Size(78, 68);
            this.tpl_label.TabIndex = 133;
            this.tpl_label.Text = "tpl";
            this.tpl_label.Click += new System.EventHandler(this.tpl_Click);
            this.tpl_label.MouseEnter += new System.EventHandler(this.tpl_MouseEnter);
            this.tpl_label.MouseLeave += new System.EventHandler(this.tpl_MouseLeave);
            // 
            // bmp_ck
            // 
            this.bmp_ck.BackColor = System.Drawing.Color.Transparent;
            this.bmp_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bmp_ck.ErrorImage = null;
            this.bmp_ck.InitialImage = null;
            this.bmp_ck.Location = new System.Drawing.Point(40, 384);
            this.bmp_ck.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_ck.Name = "bmp_ck";
            this.bmp_ck.Size = new System.Drawing.Size(64, 64);
            this.bmp_ck.TabIndex = 138;
            this.bmp_ck.TabStop = false;
            this.bmp_ck.Click += new System.EventHandler(this.bmp_Click);
            this.bmp_ck.MouseEnter += new System.EventHandler(this.bmp_MouseEnter);
            this.bmp_ck.MouseLeave += new System.EventHandler(this.bmp_MouseLeave);
            // 
            // bmp_label
            // 
            this.bmp_label.AutoSize = true;
            this.bmp_label.BackColor = System.Drawing.Color.Transparent;
            this.bmp_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.bmp_label.ForeColor = System.Drawing.SystemColors.Window;
            this.bmp_label.Location = new System.Drawing.Point(104, 384);
            this.bmp_label.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_label.Name = "bmp_label";
            this.bmp_label.Padding = new System.Windows.Forms.Padding(0, 22, 30, 22);
            this.bmp_label.Size = new System.Drawing.Size(84, 68);
            this.bmp_label.TabIndex = 136;
            this.bmp_label.Text = "bmp";
            this.bmp_label.Click += new System.EventHandler(this.bmp_Click);
            this.bmp_label.MouseEnter += new System.EventHandler(this.bmp_MouseEnter);
            this.bmp_label.MouseLeave += new System.EventHandler(this.bmp_MouseLeave);
            // 
            // png_ck
            // 
            this.png_ck.BackColor = System.Drawing.Color.Transparent;
            this.png_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.png_ck.ErrorImage = null;
            this.png_ck.InitialImage = null;
            this.png_ck.Location = new System.Drawing.Point(40, 448);
            this.png_ck.Margin = new System.Windows.Forms.Padding(0);
            this.png_ck.Name = "png_ck";
            this.png_ck.Size = new System.Drawing.Size(64, 64);
            this.png_ck.TabIndex = 141;
            this.png_ck.TabStop = false;
            this.png_ck.Click += new System.EventHandler(this.png_Click);
            this.png_ck.MouseEnter += new System.EventHandler(this.png_MouseEnter);
            this.png_ck.MouseLeave += new System.EventHandler(this.png_MouseLeave);
            // 
            // png_label
            // 
            this.png_label.AutoSize = true;
            this.png_label.BackColor = System.Drawing.Color.Transparent;
            this.png_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.png_label.ForeColor = System.Drawing.SystemColors.Window;
            this.png_label.Location = new System.Drawing.Point(104, 449);
            this.png_label.Margin = new System.Windows.Forms.Padding(0);
            this.png_label.Name = "png_label";
            this.png_label.Padding = new System.Windows.Forms.Padding(0, 22, 30, 22);
            this.png_label.Size = new System.Drawing.Size(79, 68);
            this.png_label.TabIndex = 139;
            this.png_label.Text = "png";
            this.png_label.Click += new System.EventHandler(this.png_Click);
            this.png_label.MouseEnter += new System.EventHandler(this.png_MouseEnter);
            this.png_label.MouseLeave += new System.EventHandler(this.png_MouseLeave);
            // 
            // jpg_ck
            // 
            this.jpg_ck.BackColor = System.Drawing.Color.Transparent;
            this.jpg_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.jpg_ck.ErrorImage = null;
            this.jpg_ck.InitialImage = null;
            this.jpg_ck.Location = new System.Drawing.Point(40, 512);
            this.jpg_ck.Margin = new System.Windows.Forms.Padding(0);
            this.jpg_ck.Name = "jpg_ck";
            this.jpg_ck.Size = new System.Drawing.Size(64, 64);
            this.jpg_ck.TabIndex = 144;
            this.jpg_ck.TabStop = false;
            this.jpg_ck.Click += new System.EventHandler(this.jpg_Click);
            this.jpg_ck.MouseEnter += new System.EventHandler(this.jpg_MouseEnter);
            this.jpg_ck.MouseLeave += new System.EventHandler(this.jpg_MouseLeave);
            // 
            // jpg_label
            // 
            this.jpg_label.AutoSize = true;
            this.jpg_label.BackColor = System.Drawing.Color.Transparent;
            this.jpg_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.jpg_label.ForeColor = System.Drawing.SystemColors.Window;
            this.jpg_label.Location = new System.Drawing.Point(104, 513);
            this.jpg_label.Margin = new System.Windows.Forms.Padding(0);
            this.jpg_label.Name = "jpg_label";
            this.jpg_label.Padding = new System.Windows.Forms.Padding(0, 22, 30, 22);
            this.jpg_label.Size = new System.Drawing.Size(72, 68);
            this.jpg_label.TabIndex = 142;
            this.jpg_label.Text = "jpg";
            this.jpg_label.Click += new System.EventHandler(this.jpg_Click);
            this.jpg_label.MouseEnter += new System.EventHandler(this.jpg_MouseEnter);
            this.jpg_label.MouseLeave += new System.EventHandler(this.jpg_MouseLeave);
            // 
            // tiff_ck
            // 
            this.tiff_ck.BackColor = System.Drawing.Color.Transparent;
            this.tiff_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tiff_ck.ErrorImage = null;
            this.tiff_ck.InitialImage = null;
            this.tiff_ck.Location = new System.Drawing.Point(40, 833);
            this.tiff_ck.Margin = new System.Windows.Forms.Padding(0);
            this.tiff_ck.Name = "tiff_ck";
            this.tiff_ck.Size = new System.Drawing.Size(64, 64);
            this.tiff_ck.TabIndex = 159;
            this.tiff_ck.TabStop = false;
            this.tiff_ck.Click += new System.EventHandler(this.tiff_Click);
            this.tiff_ck.MouseEnter += new System.EventHandler(this.tiff_MouseEnter);
            this.tiff_ck.MouseLeave += new System.EventHandler(this.tiff_MouseLeave);
            // 
            // tiff_label
            // 
            this.tiff_label.AutoSize = true;
            this.tiff_label.BackColor = System.Drawing.Color.Transparent;
            this.tiff_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tiff_label.ForeColor = System.Drawing.SystemColors.Window;
            this.tiff_label.Location = new System.Drawing.Point(104, 834);
            this.tiff_label.Margin = new System.Windows.Forms.Padding(0);
            this.tiff_label.Name = "tiff_label";
            this.tiff_label.Padding = new System.Windows.Forms.Padding(0, 22, 30, 22);
            this.tiff_label.Size = new System.Drawing.Size(71, 68);
            this.tiff_label.TabIndex = 157;
            this.tiff_label.Text = "tiff";
            this.tiff_label.Click += new System.EventHandler(this.tiff_Click);
            this.tiff_label.MouseEnter += new System.EventHandler(this.tiff_MouseEnter);
            this.tiff_label.MouseLeave += new System.EventHandler(this.tiff_MouseLeave);
            // 
            // tif_ck
            // 
            this.tif_ck.BackColor = System.Drawing.Color.Transparent;
            this.tif_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tif_ck.ErrorImage = null;
            this.tif_ck.InitialImage = null;
            this.tif_ck.Location = new System.Drawing.Point(40, 769);
            this.tif_ck.Margin = new System.Windows.Forms.Padding(0);
            this.tif_ck.Name = "tif_ck";
            this.tif_ck.Size = new System.Drawing.Size(64, 64);
            this.tif_ck.TabIndex = 156;
            this.tif_ck.TabStop = false;
            this.tif_ck.Click += new System.EventHandler(this.tif_Click);
            this.tif_ck.MouseEnter += new System.EventHandler(this.tif_MouseEnter);
            this.tif_ck.MouseLeave += new System.EventHandler(this.tif_MouseLeave);
            // 
            // tif_label
            // 
            this.tif_label.AutoSize = true;
            this.tif_label.BackColor = System.Drawing.Color.Transparent;
            this.tif_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tif_label.ForeColor = System.Drawing.SystemColors.Window;
            this.tif_label.Location = new System.Drawing.Point(104, 770);
            this.tif_label.Margin = new System.Windows.Forms.Padding(0);
            this.tif_label.Name = "tif_label";
            this.tif_label.Padding = new System.Windows.Forms.Padding(0, 22, 40, 22);
            this.tif_label.Size = new System.Drawing.Size(74, 68);
            this.tif_label.TabIndex = 154;
            this.tif_label.Text = "tif";
            this.tif_label.Click += new System.EventHandler(this.tif_Click);
            this.tif_label.MouseEnter += new System.EventHandler(this.tif_MouseEnter);
            this.tif_label.MouseLeave += new System.EventHandler(this.tif_MouseLeave);
            // 
            // ico_ck
            // 
            this.ico_ck.BackColor = System.Drawing.Color.Transparent;
            this.ico_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ico_ck.ErrorImage = null;
            this.ico_ck.InitialImage = null;
            this.ico_ck.Location = new System.Drawing.Point(40, 705);
            this.ico_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ico_ck.Name = "ico_ck";
            this.ico_ck.Size = new System.Drawing.Size(64, 64);
            this.ico_ck.TabIndex = 153;
            this.ico_ck.TabStop = false;
            this.ico_ck.Click += new System.EventHandler(this.ico_Click);
            this.ico_ck.MouseEnter += new System.EventHandler(this.ico_MouseEnter);
            this.ico_ck.MouseLeave += new System.EventHandler(this.ico_MouseLeave);
            // 
            // ico_label
            // 
            this.ico_label.AutoSize = true;
            this.ico_label.BackColor = System.Drawing.Color.Transparent;
            this.ico_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ico_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ico_label.Location = new System.Drawing.Point(104, 705);
            this.ico_label.Margin = new System.Windows.Forms.Padding(0);
            this.ico_label.Name = "ico_label";
            this.ico_label.Padding = new System.Windows.Forms.Padding(0, 22, 40, 22);
            this.ico_label.Size = new System.Drawing.Size(80, 68);
            this.ico_label.TabIndex = 151;
            this.ico_label.Text = "ico";
            this.ico_label.Click += new System.EventHandler(this.ico_Click);
            this.ico_label.MouseEnter += new System.EventHandler(this.ico_MouseEnter);
            this.ico_label.MouseLeave += new System.EventHandler(this.ico_MouseLeave);
            // 
            // gif_ck
            // 
            this.gif_ck.BackColor = System.Drawing.Color.Transparent;
            this.gif_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.gif_ck.ErrorImage = null;
            this.gif_ck.InitialImage = null;
            this.gif_ck.Location = new System.Drawing.Point(40, 641);
            this.gif_ck.Margin = new System.Windows.Forms.Padding(0);
            this.gif_ck.Name = "gif_ck";
            this.gif_ck.Size = new System.Drawing.Size(64, 64);
            this.gif_ck.TabIndex = 150;
            this.gif_ck.TabStop = false;
            this.gif_ck.Click += new System.EventHandler(this.gif_Click);
            this.gif_ck.MouseEnter += new System.EventHandler(this.gif_MouseEnter);
            this.gif_ck.MouseLeave += new System.EventHandler(this.gif_MouseLeave);
            // 
            // gif_label
            // 
            this.gif_label.AutoSize = true;
            this.gif_label.BackColor = System.Drawing.Color.Transparent;
            this.gif_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.gif_label.ForeColor = System.Drawing.SystemColors.Window;
            this.gif_label.Location = new System.Drawing.Point(104, 642);
            this.gif_label.Margin = new System.Windows.Forms.Padding(0);
            this.gif_label.Name = "gif_label";
            this.gif_label.Padding = new System.Windows.Forms.Padding(0, 22, 40, 22);
            this.gif_label.Size = new System.Drawing.Size(78, 68);
            this.gif_label.TabIndex = 148;
            this.gif_label.Text = "gif";
            this.gif_label.Click += new System.EventHandler(this.gif_Click);
            this.gif_label.MouseEnter += new System.EventHandler(this.gif_MouseEnter);
            this.gif_label.MouseLeave += new System.EventHandler(this.gif_MouseLeave);
            // 
            // jpeg_ck
            // 
            this.jpeg_ck.BackColor = System.Drawing.Color.Transparent;
            this.jpeg_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.jpeg_ck.ErrorImage = null;
            this.jpeg_ck.InitialImage = null;
            this.jpeg_ck.Location = new System.Drawing.Point(40, 577);
            this.jpeg_ck.Margin = new System.Windows.Forms.Padding(0);
            this.jpeg_ck.Name = "jpeg_ck";
            this.jpeg_ck.Size = new System.Drawing.Size(64, 64);
            this.jpeg_ck.TabIndex = 147;
            this.jpeg_ck.TabStop = false;
            this.jpeg_ck.Click += new System.EventHandler(this.jpeg_Click);
            this.jpeg_ck.MouseEnter += new System.EventHandler(this.jpeg_MouseEnter);
            this.jpeg_ck.MouseLeave += new System.EventHandler(this.jpeg_MouseLeave);
            // 
            // jpeg_label
            // 
            this.jpeg_label.AutoSize = true;
            this.jpeg_label.BackColor = System.Drawing.Color.Transparent;
            this.jpeg_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.jpeg_label.ForeColor = System.Drawing.SystemColors.Window;
            this.jpeg_label.Location = new System.Drawing.Point(104, 578);
            this.jpeg_label.Margin = new System.Windows.Forms.Padding(0);
            this.jpeg_label.Name = "jpeg_label";
            this.jpeg_label.Padding = new System.Windows.Forms.Padding(0, 22, 20, 22);
            this.jpeg_label.Size = new System.Drawing.Size(74, 68);
            this.jpeg_label.TabIndex = 145;
            this.jpeg_label.Text = "jpeg";
            this.jpeg_label.Click += new System.EventHandler(this.jpeg_Click);
            this.jpeg_label.MouseEnter += new System.EventHandler(this.jpeg_MouseEnter);
            this.jpeg_label.MouseLeave += new System.EventHandler(this.jpeg_MouseLeave);
            // 
            // options_label
            // 
            this.options_label.AutoSize = true;
            this.options_label.BackColor = System.Drawing.Color.Transparent;
            this.options_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.options_label.ForeColor = System.Drawing.SystemColors.Control;
            this.options_label.Location = new System.Drawing.Point(1674, 96);
            this.options_label.Margin = new System.Windows.Forms.Padding(0);
            this.options_label.Name = "options_label";
            this.options_label.Size = new System.Drawing.Size(88, 24);
            this.options_label.TabIndex = 160;
            this.options_label.Text = "Options";
            // 
            // warn_ck
            // 
            this.warn_ck.BackColor = System.Drawing.Color.Transparent;
            this.warn_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.warn_ck.ErrorImage = null;
            this.warn_ck.InitialImage = null;
            this.warn_ck.Location = new System.Drawing.Point(1648, 704);
            this.warn_ck.Margin = new System.Windows.Forms.Padding(0);
            this.warn_ck.Name = "warn_ck";
            this.warn_ck.Size = new System.Drawing.Size(64, 64);
            this.warn_ck.TabIndex = 190;
            this.warn_ck.TabStop = false;
            this.warn_ck.Click += new System.EventHandler(this.warn_Click);
            this.warn_ck.MouseEnter += new System.EventHandler(this.warn_MouseEnter);
            this.warn_ck.MouseLeave += new System.EventHandler(this.warn_MouseLeave);
            // 
            // warn_label
            // 
            this.warn_label.AutoSize = true;
            this.warn_label.BackColor = System.Drawing.Color.Transparent;
            this.warn_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.warn_label.ForeColor = System.Drawing.SystemColors.Window;
            this.warn_label.Location = new System.Drawing.Point(1716, 704);
            this.warn_label.Margin = new System.Windows.Forms.Padding(0);
            this.warn_label.Name = "warn_label";
            this.warn_label.Padding = new System.Windows.Forms.Padding(0, 22, 120, 22);
            this.warn_label.Size = new System.Drawing.Size(181, 68);
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
            this.stfu_ck.ErrorImage = null;
            this.stfu_ck.InitialImage = null;
            this.stfu_ck.Location = new System.Drawing.Point(1648, 640);
            this.stfu_ck.Margin = new System.Windows.Forms.Padding(0);
            this.stfu_ck.Name = "stfu_ck";
            this.stfu_ck.Size = new System.Drawing.Size(64, 64);
            this.stfu_ck.TabIndex = 187;
            this.stfu_ck.TabStop = false;
            this.stfu_ck.Click += new System.EventHandler(this.stfu_Click);
            this.stfu_ck.MouseEnter += new System.EventHandler(this.stfu_MouseEnter);
            this.stfu_ck.MouseLeave += new System.EventHandler(this.stfu_MouseLeave);
            // 
            // stfu_label
            // 
            this.stfu_label.AutoSize = true;
            this.stfu_label.BackColor = System.Drawing.Color.Transparent;
            this.stfu_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.stfu_label.ForeColor = System.Drawing.SystemColors.Window;
            this.stfu_label.Location = new System.Drawing.Point(1716, 640);
            this.stfu_label.Margin = new System.Windows.Forms.Padding(0);
            this.stfu_label.Name = "stfu_label";
            this.stfu_label.Padding = new System.Windows.Forms.Padding(0, 22, 130, 22);
            this.stfu_label.Size = new System.Drawing.Size(181, 68);
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
            this.safe_mode_ck.ErrorImage = null;
            this.safe_mode_ck.InitialImage = null;
            this.safe_mode_ck.Location = new System.Drawing.Point(1648, 576);
            this.safe_mode_ck.Margin = new System.Windows.Forms.Padding(0);
            this.safe_mode_ck.Name = "safe_mode_ck";
            this.safe_mode_ck.Size = new System.Drawing.Size(64, 64);
            this.safe_mode_ck.TabIndex = 184;
            this.safe_mode_ck.TabStop = false;
            this.safe_mode_ck.Click += new System.EventHandler(this.safe_mode_Click);
            this.safe_mode_ck.MouseEnter += new System.EventHandler(this.safe_mode_MouseEnter);
            this.safe_mode_ck.MouseLeave += new System.EventHandler(this.safe_mode_MouseLeave);
            // 
            // safe_mode_label
            // 
            this.safe_mode_label.AutoSize = true;
            this.safe_mode_label.BackColor = System.Drawing.Color.Transparent;
            this.safe_mode_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.safe_mode_label.ForeColor = System.Drawing.SystemColors.Window;
            this.safe_mode_label.Location = new System.Drawing.Point(1716, 576);
            this.safe_mode_label.Margin = new System.Windows.Forms.Padding(0);
            this.safe_mode_label.Name = "safe_mode_label";
            this.safe_mode_label.Padding = new System.Windows.Forms.Padding(0, 22, 50, 22);
            this.safe_mode_label.Size = new System.Drawing.Size(162, 68);
            this.safe_mode_label.TabIndex = 182;
            this.safe_mode_label.Text = "safe mode";
            this.safe_mode_label.Click += new System.EventHandler(this.safe_mode_Click);
            this.safe_mode_label.MouseEnter += new System.EventHandler(this.safe_mode_MouseEnter);
            this.safe_mode_label.MouseLeave += new System.EventHandler(this.safe_mode_MouseLeave);
            // 
            // reversey_ck
            // 
            this.reversey_ck.BackColor = System.Drawing.Color.Transparent;
            this.reversey_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.reversey_ck.ErrorImage = null;
            this.reversey_ck.InitialImage = null;
            this.reversey_ck.Location = new System.Drawing.Point(1648, 512);
            this.reversey_ck.Margin = new System.Windows.Forms.Padding(0);
            this.reversey_ck.Name = "reversey_ck";
            this.reversey_ck.Size = new System.Drawing.Size(64, 64);
            this.reversey_ck.TabIndex = 181;
            this.reversey_ck.TabStop = false;
            this.reversey_ck.Click += new System.EventHandler(this.reversey_Click);
            this.reversey_ck.MouseEnter += new System.EventHandler(this.reversey_MouseEnter);
            this.reversey_ck.MouseLeave += new System.EventHandler(this.reversey_MouseLeave);
            // 
            // reversey_label
            // 
            this.reversey_label.AutoSize = true;
            this.reversey_label.BackColor = System.Drawing.Color.Transparent;
            this.reversey_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.reversey_label.ForeColor = System.Drawing.SystemColors.Window;
            this.reversey_label.Location = new System.Drawing.Point(1716, 512);
            this.reversey_label.Margin = new System.Windows.Forms.Padding(0);
            this.reversey_label.Name = "reversey_label";
            this.reversey_label.Padding = new System.Windows.Forms.Padding(0, 22, 10, 22);
            this.reversey_label.Size = new System.Drawing.Size(160, 68);
            this.reversey_label.TabIndex = 179;
            this.reversey_label.Text = "reverse y-axis";
            this.reversey_label.Click += new System.EventHandler(this.reversey_Click);
            this.reversey_label.MouseEnter += new System.EventHandler(this.reversey_MouseEnter);
            this.reversey_label.MouseLeave += new System.EventHandler(this.reversey_MouseLeave);
            // 
            // random_ck
            // 
            this.random_ck.BackColor = System.Drawing.Color.Transparent;
            this.random_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.random_ck.ErrorImage = null;
            this.random_ck.InitialImage = null;
            this.random_ck.Location = new System.Drawing.Point(1648, 384);
            this.random_ck.Margin = new System.Windows.Forms.Padding(0);
            this.random_ck.Name = "random_ck";
            this.random_ck.Size = new System.Drawing.Size(64, 64);
            this.random_ck.TabIndex = 178;
            this.random_ck.TabStop = false;
            this.random_ck.Click += new System.EventHandler(this.random_Click);
            this.random_ck.MouseEnter += new System.EventHandler(this.random_MouseEnter);
            this.random_ck.MouseLeave += new System.EventHandler(this.random_MouseLeave);
            // 
            // random_label
            // 
            this.random_label.AutoSize = true;
            this.random_label.BackColor = System.Drawing.Color.Transparent;
            this.random_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.random_label.ForeColor = System.Drawing.SystemColors.Window;
            this.random_label.Location = new System.Drawing.Point(1716, 384);
            this.random_label.Margin = new System.Windows.Forms.Padding(0);
            this.random_label.Name = "random_label";
            this.random_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.random_label.Size = new System.Drawing.Size(166, 68);
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
            this.no_warning_ck.ErrorImage = null;
            this.no_warning_ck.InitialImage = null;
            this.no_warning_ck.Location = new System.Drawing.Point(1648, 1131);
            this.no_warning_ck.Margin = new System.Windows.Forms.Padding(0);
            this.no_warning_ck.Name = "no_warning_ck";
            this.no_warning_ck.Size = new System.Drawing.Size(64, 64);
            this.no_warning_ck.TabIndex = 175;
            this.no_warning_ck.TabStop = false;
            this.no_warning_ck.Click += new System.EventHandler(this.no_warning_Click);
            this.no_warning_ck.MouseEnter += new System.EventHandler(this.no_warning_MouseEnter);
            this.no_warning_ck.MouseLeave += new System.EventHandler(this.no_warning_MouseLeave);
            // 
            // no_warning_label
            // 
            this.no_warning_label.AutoSize = true;
            this.no_warning_label.BackColor = System.Drawing.Color.Transparent;
            this.no_warning_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.no_warning_label.ForeColor = System.Drawing.SystemColors.Window;
            this.no_warning_label.Location = new System.Drawing.Point(1716, 1131);
            this.no_warning_label.Margin = new System.Windows.Forms.Padding(0);
            this.no_warning_label.Name = "no_warning_label";
            this.no_warning_label.Padding = new System.Windows.Forms.Padding(0, 22, 50, 22);
            this.no_warning_label.Size = new System.Drawing.Size(173, 68);
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
            this.funky_ck.ErrorImage = null;
            this.funky_ck.InitialImage = null;
            this.funky_ck.Location = new System.Drawing.Point(1648, 256);
            this.funky_ck.Margin = new System.Windows.Forms.Padding(0);
            this.funky_ck.Name = "funky_ck";
            this.funky_ck.Size = new System.Drawing.Size(64, 64);
            this.funky_ck.TabIndex = 172;
            this.funky_ck.TabStop = false;
            this.funky_ck.Click += new System.EventHandler(this.funky_Click);
            this.funky_ck.MouseEnter += new System.EventHandler(this.funky_MouseEnter);
            this.funky_ck.MouseLeave += new System.EventHandler(this.funky_MouseLeave);
            // 
            // funky_label
            // 
            this.funky_label.AutoSize = true;
            this.funky_label.BackColor = System.Drawing.Color.Transparent;
            this.funky_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.funky_label.ForeColor = System.Drawing.SystemColors.Window;
            this.funky_label.Location = new System.Drawing.Point(1716, 256);
            this.funky_label.Margin = new System.Windows.Forms.Padding(0);
            this.funky_label.Name = "funky_label";
            this.funky_label.Padding = new System.Windows.Forms.Padding(0, 22, 110, 22);
            this.funky_label.Size = new System.Drawing.Size(178, 68);
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
            this.FORCE_ALPHA_ck.ErrorImage = null;
            this.FORCE_ALPHA_ck.InitialImage = null;
            this.FORCE_ALPHA_ck.Location = new System.Drawing.Point(1648, 192);
            this.FORCE_ALPHA_ck.Margin = new System.Windows.Forms.Padding(0);
            this.FORCE_ALPHA_ck.Name = "FORCE_ALPHA_ck";
            this.FORCE_ALPHA_ck.Size = new System.Drawing.Size(64, 64);
            this.FORCE_ALPHA_ck.TabIndex = 169;
            this.FORCE_ALPHA_ck.TabStop = false;
            this.FORCE_ALPHA_ck.Click += new System.EventHandler(this.FORCE_ALPHA_Click);
            this.FORCE_ALPHA_ck.MouseEnter += new System.EventHandler(this.FORCE_ALPHA_MouseEnter);
            this.FORCE_ALPHA_ck.MouseLeave += new System.EventHandler(this.FORCE_ALPHA_MouseLeave);
            // 
            // FORCE_ALPHA_label
            // 
            this.FORCE_ALPHA_label.AutoSize = true;
            this.FORCE_ALPHA_label.BackColor = System.Drawing.Color.Transparent;
            this.FORCE_ALPHA_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FORCE_ALPHA_label.ForeColor = System.Drawing.SystemColors.Window;
            this.FORCE_ALPHA_label.Location = new System.Drawing.Point(1716, 192);
            this.FORCE_ALPHA_label.Margin = new System.Windows.Forms.Padding(0);
            this.FORCE_ALPHA_label.Name = "FORCE_ALPHA_label";
            this.FORCE_ALPHA_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.FORCE_ALPHA_label.Size = new System.Drawing.Size(147, 68);
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
            this.bmp_32_ck.ErrorImage = null;
            this.bmp_32_ck.InitialImage = null;
            this.bmp_32_ck.Location = new System.Drawing.Point(1648, 128);
            this.bmp_32_ck.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_32_ck.Name = "bmp_32_ck";
            this.bmp_32_ck.Size = new System.Drawing.Size(64, 64);
            this.bmp_32_ck.TabIndex = 166;
            this.bmp_32_ck.TabStop = false;
            this.bmp_32_ck.Click += new System.EventHandler(this.bmp_32_Click);
            this.bmp_32_ck.MouseEnter += new System.EventHandler(this.bmp_32_MouseEnter);
            this.bmp_32_ck.MouseLeave += new System.EventHandler(this.bmp_32_MouseLeave);
            // 
            // bmp_32_label
            // 
            this.bmp_32_label.AutoSize = true;
            this.bmp_32_label.BackColor = System.Drawing.Color.Transparent;
            this.bmp_32_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.bmp_32_label.ForeColor = System.Drawing.SystemColors.Window;
            this.bmp_32_label.Location = new System.Drawing.Point(1716, 128);
            this.bmp_32_label.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_32_label.Name = "bmp_32_label";
            this.bmp_32_label.Padding = new System.Windows.Forms.Padding(0, 22, 50, 22);
            this.bmp_32_label.Size = new System.Drawing.Size(169, 68);
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
            this.ask_exit_ck.ErrorImage = null;
            this.ask_exit_ck.InitialImage = null;
            this.ask_exit_ck.Location = new System.Drawing.Point(1648, 64);
            this.ask_exit_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ask_exit_ck.Name = "ask_exit_ck";
            this.ask_exit_ck.Size = new System.Drawing.Size(64, 64);
            this.ask_exit_ck.TabIndex = 163;
            this.ask_exit_ck.TabStop = false;
            this.ask_exit_ck.Visible = false;
            this.ask_exit_ck.Click += new System.EventHandler(this.ask_exit_Click);
            this.ask_exit_ck.MouseEnter += new System.EventHandler(this.ask_exit_MouseEnter);
            this.ask_exit_ck.MouseLeave += new System.EventHandler(this.ask_exit_MouseLeave);
            // 
            // ask_exit_label
            // 
            this.ask_exit_label.AutoSize = true;
            this.ask_exit_label.BackColor = System.Drawing.Color.Transparent;
            this.ask_exit_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ask_exit_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ask_exit_label.Location = new System.Drawing.Point(1716, 64);
            this.ask_exit_label.Margin = new System.Windows.Forms.Padding(0);
            this.ask_exit_label.Name = "ask_exit_label";
            this.ask_exit_label.Padding = new System.Windows.Forms.Padding(0, 22, 80, 22);
            this.ask_exit_label.Size = new System.Drawing.Size(167, 68);
            this.ask_exit_label.TabIndex = 161;
            this.ask_exit_label.Text = "ask exit";
            this.ask_exit_label.Visible = false;
            this.ask_exit_label.Click += new System.EventHandler(this.ask_exit_Click);
            this.ask_exit_label.MouseEnter += new System.EventHandler(this.ask_exit_MouseEnter);
            this.ask_exit_label.MouseLeave += new System.EventHandler(this.ask_exit_MouseLeave);
            // 
            // cmpr_ck
            // 
            this.cmpr_ck.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmpr_ck.ErrorImage = null;
            this.cmpr_ck.InitialImage = null;
            this.cmpr_ck.Location = new System.Drawing.Point(256, 769);
            this.cmpr_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_ck.Name = "cmpr_ck";
            this.cmpr_ck.Size = new System.Drawing.Size(64, 64);
            this.cmpr_ck.TabIndex = 233;
            this.cmpr_ck.TabStop = false;
            this.cmpr_ck.Click += new System.EventHandler(this.CMPR_Click);
            this.cmpr_ck.MouseEnter += new System.EventHandler(this.CMPR_MouseEnter);
            this.cmpr_ck.MouseLeave += new System.EventHandler(this.CMPR_MouseLeave);
            // 
            // cmpr_label
            // 
            this.cmpr_label.AutoSize = true;
            this.cmpr_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_label.Location = new System.Drawing.Point(320, 770);
            this.cmpr_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_label.Name = "cmpr_label";
            this.cmpr_label.Padding = new System.Windows.Forms.Padding(0, 22, 30, 22);
            this.cmpr_label.Size = new System.Drawing.Size(98, 68);
            this.cmpr_label.TabIndex = 231;
            this.cmpr_label.Text = "CMPR";
            this.cmpr_label.Click += new System.EventHandler(this.CMPR_Click);
            this.cmpr_label.MouseEnter += new System.EventHandler(this.CMPR_MouseEnter);
            this.cmpr_label.MouseLeave += new System.EventHandler(this.CMPR_MouseLeave);
            // 
            // ci14x2_ck
            // 
            this.ci14x2_ck.BackColor = System.Drawing.Color.Transparent;
            this.ci14x2_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ci14x2_ck.ErrorImage = null;
            this.ci14x2_ck.InitialImage = null;
            this.ci14x2_ck.Location = new System.Drawing.Point(256, 705);
            this.ci14x2_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ci14x2_ck.Name = "ci14x2_ck";
            this.ci14x2_ck.Size = new System.Drawing.Size(64, 64);
            this.ci14x2_ck.TabIndex = 230;
            this.ci14x2_ck.TabStop = false;
            this.ci14x2_ck.Click += new System.EventHandler(this.CI14X2_Click);
            this.ci14x2_ck.MouseEnter += new System.EventHandler(this.CI14X2_MouseEnter);
            this.ci14x2_ck.MouseLeave += new System.EventHandler(this.CI14X2_MouseLeave);
            // 
            // ci14x2_label
            // 
            this.ci14x2_label.AutoSize = true;
            this.ci14x2_label.BackColor = System.Drawing.Color.Transparent;
            this.ci14x2_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ci14x2_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ci14x2_label.Location = new System.Drawing.Point(320, 705);
            this.ci14x2_label.Margin = new System.Windows.Forms.Padding(0);
            this.ci14x2_label.Name = "ci14x2_label";
            this.ci14x2_label.Padding = new System.Windows.Forms.Padding(0, 22, 20, 22);
            this.ci14x2_label.Size = new System.Drawing.Size(97, 68);
            this.ci14x2_label.TabIndex = 228;
            this.ci14x2_label.Text = "CI14x2";
            this.ci14x2_label.Click += new System.EventHandler(this.CI14X2_Click);
            this.ci14x2_label.MouseEnter += new System.EventHandler(this.CI14X2_MouseEnter);
            this.ci14x2_label.MouseLeave += new System.EventHandler(this.CI14X2_MouseLeave);
            // 
            // ci8_ck
            // 
            this.ci8_ck.BackColor = System.Drawing.Color.Transparent;
            this.ci8_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ci8_ck.ErrorImage = null;
            this.ci8_ck.InitialImage = null;
            this.ci8_ck.Location = new System.Drawing.Point(256, 641);
            this.ci8_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ci8_ck.Name = "ci8_ck";
            this.ci8_ck.Size = new System.Drawing.Size(64, 64);
            this.ci8_ck.TabIndex = 227;
            this.ci8_ck.TabStop = false;
            this.ci8_ck.Click += new System.EventHandler(this.CI8_Click);
            this.ci8_ck.MouseEnter += new System.EventHandler(this.CI8_MouseEnter);
            this.ci8_ck.MouseLeave += new System.EventHandler(this.CI8_MouseLeave);
            // 
            // ci8_label
            // 
            this.ci8_label.AutoSize = true;
            this.ci8_label.BackColor = System.Drawing.Color.Transparent;
            this.ci8_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ci8_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ci8_label.Location = new System.Drawing.Point(320, 642);
            this.ci8_label.Margin = new System.Windows.Forms.Padding(0);
            this.ci8_label.Name = "ci8_label";
            this.ci8_label.Padding = new System.Windows.Forms.Padding(0, 22, 60, 22);
            this.ci8_label.Size = new System.Drawing.Size(102, 68);
            this.ci8_label.TabIndex = 225;
            this.ci8_label.Text = "CI8";
            this.ci8_label.Click += new System.EventHandler(this.CI8_Click);
            this.ci8_label.MouseEnter += new System.EventHandler(this.CI8_MouseEnter);
            this.ci8_label.MouseLeave += new System.EventHandler(this.CI8_MouseLeave);
            // 
            // ci4_ck
            // 
            this.ci4_ck.BackColor = System.Drawing.Color.Transparent;
            this.ci4_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ci4_ck.ErrorImage = null;
            this.ci4_ck.InitialImage = null;
            this.ci4_ck.Location = new System.Drawing.Point(256, 577);
            this.ci4_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ci4_ck.Name = "ci4_ck";
            this.ci4_ck.Size = new System.Drawing.Size(64, 64);
            this.ci4_ck.TabIndex = 224;
            this.ci4_ck.TabStop = false;
            this.ci4_ck.Click += new System.EventHandler(this.CI4_Click);
            this.ci4_ck.MouseEnter += new System.EventHandler(this.CI4_MouseEnter);
            this.ci4_ck.MouseLeave += new System.EventHandler(this.CI4_MouseLeave);
            // 
            // ci4_label
            // 
            this.ci4_label.AutoSize = true;
            this.ci4_label.BackColor = System.Drawing.Color.Transparent;
            this.ci4_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ci4_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ci4_label.Location = new System.Drawing.Point(320, 578);
            this.ci4_label.Margin = new System.Windows.Forms.Padding(0);
            this.ci4_label.Name = "ci4_label";
            this.ci4_label.Padding = new System.Windows.Forms.Padding(0, 22, 60, 22);
            this.ci4_label.Size = new System.Drawing.Size(102, 68);
            this.ci4_label.TabIndex = 222;
            this.ci4_label.Text = "CI4";
            this.ci4_label.Click += new System.EventHandler(this.CI4_Click);
            this.ci4_label.MouseEnter += new System.EventHandler(this.CI4_MouseEnter);
            this.ci4_label.MouseLeave += new System.EventHandler(this.CI4_MouseLeave);
            // 
            // rgba32_ck
            // 
            this.rgba32_ck.BackColor = System.Drawing.Color.Transparent;
            this.rgba32_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rgba32_ck.ErrorImage = null;
            this.rgba32_ck.InitialImage = null;
            this.rgba32_ck.Location = new System.Drawing.Point(256, 512);
            this.rgba32_ck.Margin = new System.Windows.Forms.Padding(0);
            this.rgba32_ck.Name = "rgba32_ck";
            this.rgba32_ck.Size = new System.Drawing.Size(64, 64);
            this.rgba32_ck.TabIndex = 221;
            this.rgba32_ck.TabStop = false;
            this.rgba32_ck.Click += new System.EventHandler(this.RGBA32_Click);
            this.rgba32_ck.MouseEnter += new System.EventHandler(this.RGBA32_MouseEnter);
            this.rgba32_ck.MouseLeave += new System.EventHandler(this.RGBA32_MouseLeave);
            // 
            // rgba32_label
            // 
            this.rgba32_label.AutoSize = true;
            this.rgba32_label.BackColor = System.Drawing.Color.Transparent;
            this.rgba32_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.rgba32_label.ForeColor = System.Drawing.SystemColors.Window;
            this.rgba32_label.Location = new System.Drawing.Point(320, 513);
            this.rgba32_label.Margin = new System.Windows.Forms.Padding(0);
            this.rgba32_label.Name = "rgba32_label";
            this.rgba32_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.rgba32_label.Size = new System.Drawing.Size(89, 68);
            this.rgba32_label.TabIndex = 219;
            this.rgba32_label.Text = "RGBA32";
            this.rgba32_label.Click += new System.EventHandler(this.RGBA32_Click);
            this.rgba32_label.MouseEnter += new System.EventHandler(this.RGBA32_MouseEnter);
            this.rgba32_label.MouseLeave += new System.EventHandler(this.RGBA32_MouseLeave);
            // 
            // rgb5a3_ck
            // 
            this.rgb5a3_ck.BackColor = System.Drawing.Color.Transparent;
            this.rgb5a3_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rgb5a3_ck.ErrorImage = null;
            this.rgb5a3_ck.InitialImage = null;
            this.rgb5a3_ck.Location = new System.Drawing.Point(256, 448);
            this.rgb5a3_ck.Margin = new System.Windows.Forms.Padding(0);
            this.rgb5a3_ck.Name = "rgb5a3_ck";
            this.rgb5a3_ck.Size = new System.Drawing.Size(64, 64);
            this.rgb5a3_ck.TabIndex = 218;
            this.rgb5a3_ck.TabStop = false;
            this.rgb5a3_ck.Click += new System.EventHandler(this.RGB5A3_Click);
            this.rgb5a3_ck.MouseEnter += new System.EventHandler(this.RGB5A3_MouseEnter);
            this.rgb5a3_ck.MouseLeave += new System.EventHandler(this.RGB5A3_MouseLeave);
            // 
            // rgb5a3_label
            // 
            this.rgb5a3_label.AutoSize = true;
            this.rgb5a3_label.BackColor = System.Drawing.Color.Transparent;
            this.rgb5a3_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.rgb5a3_label.ForeColor = System.Drawing.SystemColors.Window;
            this.rgb5a3_label.Location = new System.Drawing.Point(320, 449);
            this.rgb5a3_label.Margin = new System.Windows.Forms.Padding(0);
            this.rgb5a3_label.Name = "rgb5a3_label";
            this.rgb5a3_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.rgb5a3_label.Size = new System.Drawing.Size(89, 68);
            this.rgb5a3_label.TabIndex = 216;
            this.rgb5a3_label.Text = "RGB5A3";
            this.rgb5a3_label.Click += new System.EventHandler(this.RGB5A3_Click);
            this.rgb5a3_label.MouseEnter += new System.EventHandler(this.RGB5A3_MouseEnter);
            this.rgb5a3_label.MouseLeave += new System.EventHandler(this.RGB5A3_MouseLeave);
            // 
            // rgb565_ck
            // 
            this.rgb565_ck.BackColor = System.Drawing.Color.Transparent;
            this.rgb565_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rgb565_ck.ErrorImage = null;
            this.rgb565_ck.InitialImage = null;
            this.rgb565_ck.Location = new System.Drawing.Point(256, 384);
            this.rgb565_ck.Margin = new System.Windows.Forms.Padding(0);
            this.rgb565_ck.Name = "rgb565_ck";
            this.rgb565_ck.Size = new System.Drawing.Size(64, 64);
            this.rgb565_ck.TabIndex = 215;
            this.rgb565_ck.TabStop = false;
            this.rgb565_ck.Click += new System.EventHandler(this.RGB565_Click);
            this.rgb565_ck.MouseEnter += new System.EventHandler(this.RGB565_MouseEnter);
            this.rgb565_ck.MouseLeave += new System.EventHandler(this.RGB565_MouseLeave);
            // 
            // rgb565_label
            // 
            this.rgb565_label.AutoSize = true;
            this.rgb565_label.BackColor = System.Drawing.Color.Transparent;
            this.rgb565_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.rgb565_label.ForeColor = System.Drawing.SystemColors.Window;
            this.rgb565_label.Location = new System.Drawing.Point(320, 384);
            this.rgb565_label.Margin = new System.Windows.Forms.Padding(0);
            this.rgb565_label.Name = "rgb565_label";
            this.rgb565_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.rgb565_label.Size = new System.Drawing.Size(87, 68);
            this.rgb565_label.TabIndex = 213;
            this.rgb565_label.Text = "RGB565";
            this.rgb565_label.Click += new System.EventHandler(this.RGB565_Click);
            this.rgb565_label.MouseEnter += new System.EventHandler(this.RGB565_MouseEnter);
            this.rgb565_label.MouseLeave += new System.EventHandler(this.RGB565_MouseLeave);
            // 
            // ai8_ck
            // 
            this.ai8_ck.BackColor = System.Drawing.Color.Transparent;
            this.ai8_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ai8_ck.ErrorImage = null;
            this.ai8_ck.InitialImage = null;
            this.ai8_ck.Location = new System.Drawing.Point(256, 320);
            this.ai8_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ai8_ck.Name = "ai8_ck";
            this.ai8_ck.Size = new System.Drawing.Size(64, 64);
            this.ai8_ck.TabIndex = 212;
            this.ai8_ck.TabStop = false;
            this.ai8_ck.Click += new System.EventHandler(this.AI8_Click);
            this.ai8_ck.MouseEnter += new System.EventHandler(this.AI8_MouseEnter);
            this.ai8_ck.MouseLeave += new System.EventHandler(this.AI8_MouseLeave);
            // 
            // ai8_label
            // 
            this.ai8_label.AutoSize = true;
            this.ai8_label.BackColor = System.Drawing.Color.Transparent;
            this.ai8_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ai8_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ai8_label.Location = new System.Drawing.Point(320, 321);
            this.ai8_label.Margin = new System.Windows.Forms.Padding(0);
            this.ai8_label.Name = "ai8_label";
            this.ai8_label.Padding = new System.Windows.Forms.Padding(0, 22, 60, 22);
            this.ai8_label.Size = new System.Drawing.Size(102, 68);
            this.ai8_label.TabIndex = 210;
            this.ai8_label.Text = "AI8";
            this.ai8_label.Click += new System.EventHandler(this.AI8_Click);
            this.ai8_label.MouseEnter += new System.EventHandler(this.AI8_MouseEnter);
            this.ai8_label.MouseLeave += new System.EventHandler(this.AI8_MouseLeave);
            // 
            // ai4_ck
            // 
            this.ai4_ck.BackColor = System.Drawing.Color.Transparent;
            this.ai4_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ai4_ck.ErrorImage = null;
            this.ai4_ck.InitialImage = null;
            this.ai4_ck.Location = new System.Drawing.Point(256, 256);
            this.ai4_ck.Margin = new System.Windows.Forms.Padding(0);
            this.ai4_ck.Name = "ai4_ck";
            this.ai4_ck.Size = new System.Drawing.Size(64, 64);
            this.ai4_ck.TabIndex = 209;
            this.ai4_ck.TabStop = false;
            this.ai4_ck.Click += new System.EventHandler(this.AI4_Click);
            this.ai4_ck.MouseEnter += new System.EventHandler(this.AI4_MouseEnter);
            this.ai4_ck.MouseLeave += new System.EventHandler(this.AI4_MouseLeave);
            // 
            // ai4_label
            // 
            this.ai4_label.AutoSize = true;
            this.ai4_label.BackColor = System.Drawing.Color.Transparent;
            this.ai4_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ai4_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ai4_label.Location = new System.Drawing.Point(320, 257);
            this.ai4_label.Margin = new System.Windows.Forms.Padding(0);
            this.ai4_label.Name = "ai4_label";
            this.ai4_label.Padding = new System.Windows.Forms.Padding(0, 22, 60, 22);
            this.ai4_label.Size = new System.Drawing.Size(102, 68);
            this.ai4_label.TabIndex = 207;
            this.ai4_label.Text = "AI4";
            this.ai4_label.Click += new System.EventHandler(this.AI4_Click);
            this.ai4_label.MouseEnter += new System.EventHandler(this.AI4_MouseEnter);
            this.ai4_label.MouseLeave += new System.EventHandler(this.AI4_MouseLeave);
            // 
            // i8_ck
            // 
            this.i8_ck.BackColor = System.Drawing.Color.Transparent;
            this.i8_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.i8_ck.ErrorImage = null;
            this.i8_ck.InitialImage = null;
            this.i8_ck.Location = new System.Drawing.Point(256, 192);
            this.i8_ck.Margin = new System.Windows.Forms.Padding(0);
            this.i8_ck.Name = "i8_ck";
            this.i8_ck.Size = new System.Drawing.Size(64, 64);
            this.i8_ck.TabIndex = 206;
            this.i8_ck.TabStop = false;
            this.i8_ck.Click += new System.EventHandler(this.I8_Click);
            this.i8_ck.MouseEnter += new System.EventHandler(this.I8_MouseEnter);
            this.i8_ck.MouseLeave += new System.EventHandler(this.I8_MouseLeave);
            // 
            // i8_label
            // 
            this.i8_label.AutoSize = true;
            this.i8_label.BackColor = System.Drawing.Color.Transparent;
            this.i8_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.i8_label.ForeColor = System.Drawing.SystemColors.Window;
            this.i8_label.Location = new System.Drawing.Point(320, 192);
            this.i8_label.Margin = new System.Windows.Forms.Padding(0);
            this.i8_label.Name = "i8_label";
            this.i8_label.Padding = new System.Windows.Forms.Padding(0, 22, 80, 22);
            this.i8_label.Size = new System.Drawing.Size(108, 68);
            this.i8_label.TabIndex = 204;
            this.i8_label.Text = "I8";
            this.i8_label.Click += new System.EventHandler(this.I8_Click);
            this.i8_label.MouseEnter += new System.EventHandler(this.I8_MouseEnter);
            this.i8_label.MouseLeave += new System.EventHandler(this.I8_MouseLeave);
            // 
            // i4_ck
            // 
            this.i4_ck.BackColor = System.Drawing.Color.Transparent;
            this.i4_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.i4_ck.ErrorImage = null;
            this.i4_ck.InitialImage = null;
            this.i4_ck.Location = new System.Drawing.Point(256, 128);
            this.i4_ck.Margin = new System.Windows.Forms.Padding(0);
            this.i4_ck.Name = "i4_ck";
            this.i4_ck.Size = new System.Drawing.Size(64, 64);
            this.i4_ck.TabIndex = 203;
            this.i4_ck.TabStop = false;
            this.i4_ck.Click += new System.EventHandler(this.I4_Click);
            this.i4_ck.MouseEnter += new System.EventHandler(this.I4_MouseEnter);
            this.i4_ck.MouseLeave += new System.EventHandler(this.I4_MouseLeave);
            // 
            // i4_label
            // 
            this.i4_label.AutoSize = true;
            this.i4_label.BackColor = System.Drawing.Color.Transparent;
            this.i4_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.i4_label.ForeColor = System.Drawing.SystemColors.Window;
            this.i4_label.Location = new System.Drawing.Point(320, 128);
            this.i4_label.Margin = new System.Windows.Forms.Padding(0);
            this.i4_label.Name = "i4_label";
            this.i4_label.Padding = new System.Windows.Forms.Padding(0, 22, 80, 22);
            this.i4_label.Size = new System.Drawing.Size(108, 68);
            this.i4_label.TabIndex = 201;
            this.i4_label.Text = "I4";
            this.i4_label.Click += new System.EventHandler(this.I4_Click);
            this.i4_label.MouseEnter += new System.EventHandler(this.I4_MouseEnter);
            this.i4_label.MouseLeave += new System.EventHandler(this.I4_MouseLeave);
            // 
            // encoding_label
            // 
            this.encoding_label.AutoSize = true;
            this.encoding_label.BackColor = System.Drawing.Color.Transparent;
            this.encoding_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.encoding_label.ForeColor = System.Drawing.SystemColors.Control;
            this.encoding_label.Location = new System.Drawing.Point(278, 95);
            this.encoding_label.Name = "encoding_label";
            this.encoding_label.Size = new System.Drawing.Size(104, 24);
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
            this.no_gradient_ck.ErrorImage = null;
            this.no_gradient_ck.InitialImage = null;
            this.no_gradient_ck.Location = new System.Drawing.Point(500, 1104);
            this.no_gradient_ck.Margin = new System.Windows.Forms.Padding(0);
            this.no_gradient_ck.Name = "no_gradient_ck";
            this.no_gradient_ck.Size = new System.Drawing.Size(64, 64);
            this.no_gradient_ck.TabIndex = 247;
            this.no_gradient_ck.TabStop = false;
            this.no_gradient_ck.Visible = false;
            this.no_gradient_ck.Click += new System.EventHandler(this.No_gradient_Click);
            this.no_gradient_ck.MouseEnter += new System.EventHandler(this.No_gradient_MouseEnter);
            this.no_gradient_ck.MouseLeave += new System.EventHandler(this.No_gradient_MouseLeave);
            // 
            // no_gradient_label
            // 
            this.no_gradient_label.AutoSize = true;
            this.no_gradient_label.BackColor = System.Drawing.Color.Transparent;
            this.no_gradient_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.no_gradient_label.ForeColor = System.Drawing.SystemColors.Window;
            this.no_gradient_label.Location = new System.Drawing.Point(571, 1104);
            this.no_gradient_label.Margin = new System.Windows.Forms.Padding(0);
            this.no_gradient_label.Name = "no_gradient_label";
            this.no_gradient_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.no_gradient_label.Size = new System.Drawing.Size(132, 68);
            this.no_gradient_label.TabIndex = 245;
            this.no_gradient_label.Text = "No Gradient";
            this.no_gradient_label.Visible = false;
            this.no_gradient_label.Click += new System.EventHandler(this.No_gradient_Click);
            this.no_gradient_label.MouseEnter += new System.EventHandler(this.No_gradient_MouseEnter);
            this.no_gradient_label.MouseLeave += new System.EventHandler(this.No_gradient_MouseLeave);
            // 
            // custom_ck
            // 
            this.custom_ck.BackColor = System.Drawing.Color.Transparent;
            this.custom_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.custom_ck.ErrorImage = null;
            this.custom_ck.InitialImage = null;
            this.custom_ck.Location = new System.Drawing.Point(500, 256);
            this.custom_ck.Margin = new System.Windows.Forms.Padding(0);
            this.custom_ck.Name = "custom_ck";
            this.custom_ck.Size = new System.Drawing.Size(64, 64);
            this.custom_ck.TabIndex = 244;
            this.custom_ck.TabStop = false;
            this.custom_ck.Click += new System.EventHandler(this.Custom_Click);
            this.custom_ck.MouseEnter += new System.EventHandler(this.Custom_MouseEnter);
            this.custom_ck.MouseLeave += new System.EventHandler(this.Custom_MouseLeave);
            // 
            // custom_label
            // 
            this.custom_label.AutoSize = true;
            this.custom_label.BackColor = System.Drawing.Color.Transparent;
            this.custom_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.custom_label.ForeColor = System.Drawing.SystemColors.Window;
            this.custom_label.Location = new System.Drawing.Point(564, 256);
            this.custom_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_label.Name = "custom_label";
            this.custom_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.custom_label.Size = new System.Drawing.Size(146, 68);
            this.custom_label.TabIndex = 242;
            this.custom_label.Text = "Custom RGBA";
            this.custom_label.Click += new System.EventHandler(this.Custom_Click);
            this.custom_label.MouseEnter += new System.EventHandler(this.Custom_MouseEnter);
            this.custom_label.MouseLeave += new System.EventHandler(this.Custom_MouseLeave);
            // 
            // cie_709_ck
            // 
            this.cie_709_ck.BackColor = System.Drawing.Color.Transparent;
            this.cie_709_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cie_709_ck.ErrorImage = null;
            this.cie_709_ck.InitialImage = null;
            this.cie_709_ck.Location = new System.Drawing.Point(500, 192);
            this.cie_709_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cie_709_ck.Name = "cie_709_ck";
            this.cie_709_ck.Size = new System.Drawing.Size(64, 64);
            this.cie_709_ck.TabIndex = 241;
            this.cie_709_ck.TabStop = false;
            this.cie_709_ck.Click += new System.EventHandler(this.Cie_709_Click);
            this.cie_709_ck.MouseEnter += new System.EventHandler(this.Cie_709_MouseEnter);
            this.cie_709_ck.MouseLeave += new System.EventHandler(this.Cie_709_MouseLeave);
            // 
            // cie_709_label
            // 
            this.cie_709_label.AutoSize = true;
            this.cie_709_label.BackColor = System.Drawing.Color.Transparent;
            this.cie_709_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cie_709_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cie_709_label.Location = new System.Drawing.Point(564, 192);
            this.cie_709_label.Margin = new System.Windows.Forms.Padding(0);
            this.cie_709_label.Name = "cie_709_label";
            this.cie_709_label.Padding = new System.Windows.Forms.Padding(0, 22, 50, 22);
            this.cie_709_label.Size = new System.Drawing.Size(133, 68);
            this.cie_709_label.TabIndex = 239;
            this.cie_709_label.Text = "CIE 709";
            this.cie_709_label.Click += new System.EventHandler(this.Cie_709_Click);
            this.cie_709_label.MouseEnter += new System.EventHandler(this.Cie_709_MouseEnter);
            this.cie_709_label.MouseLeave += new System.EventHandler(this.Cie_709_MouseLeave);
            // 
            // cie_601_ck
            // 
            this.cie_601_ck.BackColor = System.Drawing.Color.Transparent;
            this.cie_601_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cie_601_ck.ErrorImage = null;
            this.cie_601_ck.InitialImage = null;
            this.cie_601_ck.Location = new System.Drawing.Point(500, 128);
            this.cie_601_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cie_601_ck.Name = "cie_601_ck";
            this.cie_601_ck.Size = new System.Drawing.Size(64, 64);
            this.cie_601_ck.TabIndex = 238;
            this.cie_601_ck.TabStop = false;
            this.cie_601_ck.Click += new System.EventHandler(this.Cie_601_Click);
            this.cie_601_ck.MouseEnter += new System.EventHandler(this.Cie_601_MouseEnter);
            this.cie_601_ck.MouseLeave += new System.EventHandler(this.Cie_601_MouseLeave);
            // 
            // cie_601_label
            // 
            this.cie_601_label.AutoSize = true;
            this.cie_601_label.BackColor = System.Drawing.Color.Transparent;
            this.cie_601_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cie_601_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cie_601_label.Location = new System.Drawing.Point(564, 128);
            this.cie_601_label.Margin = new System.Windows.Forms.Padding(0);
            this.cie_601_label.Name = "cie_601_label";
            this.cie_601_label.Padding = new System.Windows.Forms.Padding(0, 22, 50, 22);
            this.cie_601_label.Size = new System.Drawing.Size(133, 68);
            this.cie_601_label.TabIndex = 236;
            this.cie_601_label.Text = "CIE 601";
            this.cie_601_label.Click += new System.EventHandler(this.Cie_601_Click);
            this.cie_601_label.MouseEnter += new System.EventHandler(this.Cie_601_MouseEnter);
            this.cie_601_label.MouseLeave += new System.EventHandler(this.Cie_601_MouseLeave);
            // 
            // algorithm_label
            // 
            this.algorithm_label.AutoSize = true;
            this.algorithm_label.BackColor = System.Drawing.Color.Transparent;
            this.algorithm_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.algorithm_label.ForeColor = System.Drawing.SystemColors.Control;
            this.algorithm_label.Location = new System.Drawing.Point(544, 95);
            this.algorithm_label.Name = "algorithm_label";
            this.algorithm_label.Size = new System.Drawing.Size(111, 24);
            this.algorithm_label.TabIndex = 235;
            this.algorithm_label.Text = "Algorithm";
            // 
            // mix_ck
            // 
            this.mix_ck.BackColor = System.Drawing.Color.Transparent;
            this.mix_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mix_ck.ErrorImage = null;
            this.mix_ck.InitialImage = null;
            this.mix_ck.Location = new System.Drawing.Point(500, 558);
            this.mix_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mix_ck.Name = "mix_ck";
            this.mix_ck.Size = new System.Drawing.Size(64, 64);
            this.mix_ck.TabIndex = 257;
            this.mix_ck.TabStop = false;
            this.mix_ck.Click += new System.EventHandler(this.Mix_Click);
            this.mix_ck.MouseEnter += new System.EventHandler(this.Mix_MouseEnter);
            this.mix_ck.MouseLeave += new System.EventHandler(this.Mix_MouseLeave);
            // 
            // mix_label
            // 
            this.mix_label.AutoSize = true;
            this.mix_label.BackColor = System.Drawing.Color.Transparent;
            this.mix_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.mix_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mix_label.Location = new System.Drawing.Point(564, 558);
            this.mix_label.Margin = new System.Windows.Forms.Padding(0);
            this.mix_label.Name = "mix_label";
            this.mix_label.Padding = new System.Windows.Forms.Padding(0, 22, 60, 22);
            this.mix_label.Size = new System.Drawing.Size(105, 68);
            this.mix_label.TabIndex = 255;
            this.mix_label.Text = "Mix";
            this.mix_label.Click += new System.EventHandler(this.Mix_Click);
            this.mix_label.MouseEnter += new System.EventHandler(this.Mix_MouseEnter);
            this.mix_label.MouseLeave += new System.EventHandler(this.Mix_MouseLeave);
            // 
            // alpha_ck
            // 
            this.alpha_ck.BackColor = System.Drawing.Color.Transparent;
            this.alpha_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.alpha_ck.ErrorImage = null;
            this.alpha_ck.InitialImage = null;
            this.alpha_ck.Location = new System.Drawing.Point(500, 494);
            this.alpha_ck.Margin = new System.Windows.Forms.Padding(0);
            this.alpha_ck.Name = "alpha_ck";
            this.alpha_ck.Size = new System.Drawing.Size(64, 64);
            this.alpha_ck.TabIndex = 254;
            this.alpha_ck.TabStop = false;
            this.alpha_ck.Click += new System.EventHandler(this.Alpha_Click);
            this.alpha_ck.MouseEnter += new System.EventHandler(this.Alpha_MouseEnter);
            this.alpha_ck.MouseLeave += new System.EventHandler(this.Alpha_MouseLeave);
            // 
            // alpha_label
            // 
            this.alpha_label.AutoSize = true;
            this.alpha_label.BackColor = System.Drawing.Color.Transparent;
            this.alpha_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.alpha_label.ForeColor = System.Drawing.SystemColors.Window;
            this.alpha_label.Location = new System.Drawing.Point(564, 494);
            this.alpha_label.Margin = new System.Windows.Forms.Padding(0);
            this.alpha_label.Name = "alpha_label";
            this.alpha_label.Padding = new System.Windows.Forms.Padding(0, 22, 40, 22);
            this.alpha_label.Size = new System.Drawing.Size(108, 68);
            this.alpha_label.TabIndex = 252;
            this.alpha_label.Text = "Alpha";
            this.alpha_label.Click += new System.EventHandler(this.Alpha_Click);
            this.alpha_label.MouseEnter += new System.EventHandler(this.Alpha_MouseEnter);
            this.alpha_label.MouseLeave += new System.EventHandler(this.Alpha_MouseLeave);
            // 
            // no_alpha_ck
            // 
            this.no_alpha_ck.BackColor = System.Drawing.Color.Transparent;
            this.no_alpha_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.no_alpha_ck.ErrorImage = null;
            this.no_alpha_ck.InitialImage = null;
            this.no_alpha_ck.Location = new System.Drawing.Point(500, 430);
            this.no_alpha_ck.Margin = new System.Windows.Forms.Padding(0);
            this.no_alpha_ck.Name = "no_alpha_ck";
            this.no_alpha_ck.Size = new System.Drawing.Size(64, 64);
            this.no_alpha_ck.TabIndex = 251;
            this.no_alpha_ck.TabStop = false;
            this.no_alpha_ck.Click += new System.EventHandler(this.No_alpha_Click);
            this.no_alpha_ck.MouseEnter += new System.EventHandler(this.No_alpha_MouseEnter);
            this.no_alpha_ck.MouseLeave += new System.EventHandler(this.No_alpha_MouseLeave);
            // 
            // no_alpha_label
            // 
            this.no_alpha_label.AutoSize = true;
            this.no_alpha_label.BackColor = System.Drawing.Color.Transparent;
            this.no_alpha_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.no_alpha_label.ForeColor = System.Drawing.SystemColors.Window;
            this.no_alpha_label.Location = new System.Drawing.Point(564, 430);
            this.no_alpha_label.Margin = new System.Windows.Forms.Padding(0);
            this.no_alpha_label.Name = "no_alpha_label";
            this.no_alpha_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.no_alpha_label.Size = new System.Drawing.Size(100, 68);
            this.no_alpha_label.TabIndex = 249;
            this.no_alpha_label.Text = "No Alpha";
            this.no_alpha_label.Click += new System.EventHandler(this.No_alpha_Click);
            this.no_alpha_label.MouseEnter += new System.EventHandler(this.No_alpha_MouseEnter);
            this.no_alpha_label.MouseLeave += new System.EventHandler(this.No_alpha_MouseLeave);
            // 
            // alpha_title
            // 
            this.alpha_title.AutoSize = true;
            this.alpha_title.BackColor = System.Drawing.Color.Transparent;
            this.alpha_title.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.alpha_title.ForeColor = System.Drawing.SystemColors.Control;
            this.alpha_title.Location = new System.Drawing.Point(544, 397);
            this.alpha_title.Margin = new System.Windows.Forms.Padding(0);
            this.alpha_title.Name = "alpha_title";
            this.alpha_title.Size = new System.Drawing.Size(68, 24);
            this.alpha_title.TabIndex = 248;
            this.alpha_title.Text = "Alpha";
            // 
            // Tmirror_ck
            // 
            this.Tmirror_ck.BackColor = System.Drawing.Color.Transparent;
            this.Tmirror_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Tmirror_ck.ErrorImage = null;
            this.Tmirror_ck.InitialImage = null;
            this.Tmirror_ck.Location = new System.Drawing.Point(1440, 704);
            this.Tmirror_ck.Margin = new System.Windows.Forms.Padding(0);
            this.Tmirror_ck.Name = "Tmirror_ck";
            this.Tmirror_ck.Size = new System.Drawing.Size(64, 64);
            this.Tmirror_ck.TabIndex = 267;
            this.Tmirror_ck.TabStop = false;
            this.Tmirror_ck.Click += new System.EventHandler(this.WrapT_Mirror_Click);
            this.Tmirror_ck.MouseEnter += new System.EventHandler(this.WrapT_Mirror_MouseEnter);
            this.Tmirror_ck.MouseLeave += new System.EventHandler(this.WrapT_Mirror_MouseLeave);
            // 
            // Tmirror_label
            // 
            this.Tmirror_label.AutoSize = true;
            this.Tmirror_label.BackColor = System.Drawing.Color.Transparent;
            this.Tmirror_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Tmirror_label.ForeColor = System.Drawing.SystemColors.Window;
            this.Tmirror_label.Location = new System.Drawing.Point(1504, 704);
            this.Tmirror_label.Margin = new System.Windows.Forms.Padding(0);
            this.Tmirror_label.Name = "Tmirror_label";
            this.Tmirror_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Tmirror_label.Size = new System.Drawing.Size(76, 68);
            this.Tmirror_label.TabIndex = 265;
            this.Tmirror_label.Text = "Mirror";
            this.Tmirror_label.Click += new System.EventHandler(this.WrapT_Mirror_Click);
            this.Tmirror_label.MouseEnter += new System.EventHandler(this.WrapT_Mirror_MouseEnter);
            this.Tmirror_label.MouseLeave += new System.EventHandler(this.WrapT_Mirror_MouseLeave);
            // 
            // Trepeat_ck
            // 
            this.Trepeat_ck.BackColor = System.Drawing.Color.Transparent;
            this.Trepeat_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Trepeat_ck.ErrorImage = null;
            this.Trepeat_ck.InitialImage = null;
            this.Trepeat_ck.Location = new System.Drawing.Point(1440, 640);
            this.Trepeat_ck.Margin = new System.Windows.Forms.Padding(0);
            this.Trepeat_ck.Name = "Trepeat_ck";
            this.Trepeat_ck.Size = new System.Drawing.Size(64, 64);
            this.Trepeat_ck.TabIndex = 264;
            this.Trepeat_ck.TabStop = false;
            this.Trepeat_ck.Click += new System.EventHandler(this.WrapT_Repeat_Click);
            this.Trepeat_ck.MouseEnter += new System.EventHandler(this.WrapT_Repeat_MouseEnter);
            this.Trepeat_ck.MouseLeave += new System.EventHandler(this.WrapT_Repeat_MouseLeave);
            // 
            // Trepeat_label
            // 
            this.Trepeat_label.AutoSize = true;
            this.Trepeat_label.BackColor = System.Drawing.Color.Transparent;
            this.Trepeat_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Trepeat_label.ForeColor = System.Drawing.SystemColors.Window;
            this.Trepeat_label.Location = new System.Drawing.Point(1504, 640);
            this.Trepeat_label.Margin = new System.Windows.Forms.Padding(0);
            this.Trepeat_label.Name = "Trepeat_label";
            this.Trepeat_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Trepeat_label.Size = new System.Drawing.Size(80, 68);
            this.Trepeat_label.TabIndex = 262;
            this.Trepeat_label.Text = "Repeat";
            this.Trepeat_label.Click += new System.EventHandler(this.WrapT_Repeat_Click);
            this.Trepeat_label.MouseEnter += new System.EventHandler(this.WrapT_Repeat_MouseEnter);
            this.Trepeat_label.MouseLeave += new System.EventHandler(this.WrapT_Repeat_MouseLeave);
            // 
            // Tclamp_ck
            // 
            this.Tclamp_ck.BackColor = System.Drawing.Color.Transparent;
            this.Tclamp_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Tclamp_ck.ErrorImage = null;
            this.Tclamp_ck.InitialImage = null;
            this.Tclamp_ck.Location = new System.Drawing.Point(1440, 576);
            this.Tclamp_ck.Margin = new System.Windows.Forms.Padding(0);
            this.Tclamp_ck.Name = "Tclamp_ck";
            this.Tclamp_ck.Size = new System.Drawing.Size(64, 64);
            this.Tclamp_ck.TabIndex = 261;
            this.Tclamp_ck.TabStop = false;
            this.Tclamp_ck.Click += new System.EventHandler(this.WrapT_Clamp_Click);
            this.Tclamp_ck.MouseEnter += new System.EventHandler(this.WrapT_Clamp_MouseEnter);
            this.Tclamp_ck.MouseLeave += new System.EventHandler(this.WrapT_Clamp_MouseLeave);
            // 
            // Tclamp_label
            // 
            this.Tclamp_label.AutoSize = true;
            this.Tclamp_label.BackColor = System.Drawing.Color.Transparent;
            this.Tclamp_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Tclamp_label.ForeColor = System.Drawing.SystemColors.Window;
            this.Tclamp_label.Location = new System.Drawing.Point(1504, 576);
            this.Tclamp_label.Margin = new System.Windows.Forms.Padding(0);
            this.Tclamp_label.Name = "Tclamp_label";
            this.Tclamp_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Tclamp_label.Size = new System.Drawing.Size(73, 68);
            this.Tclamp_label.TabIndex = 259;
            this.Tclamp_label.Text = "Clamp";
            this.Tclamp_label.Click += new System.EventHandler(this.WrapT_Clamp_Click);
            this.Tclamp_label.MouseEnter += new System.EventHandler(this.WrapT_Clamp_MouseEnter);
            this.Tclamp_label.MouseLeave += new System.EventHandler(this.WrapT_Clamp_MouseLeave);
            // 
            // WrapT_label
            // 
            this.WrapT_label.AutoSize = true;
            this.WrapT_label.BackColor = System.Drawing.Color.Transparent;
            this.WrapT_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.WrapT_label.ForeColor = System.Drawing.SystemColors.Control;
            this.WrapT_label.Location = new System.Drawing.Point(1488, 546);
            this.WrapT_label.Name = "WrapT_label";
            this.WrapT_label.Size = new System.Drawing.Size(78, 24);
            this.WrapT_label.TabIndex = 258;
            this.WrapT_label.Text = "WrapT";
            // 
            // Smirror_ck
            // 
            this.Smirror_ck.BackColor = System.Drawing.Color.Transparent;
            this.Smirror_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Smirror_ck.ErrorImage = null;
            this.Smirror_ck.InitialImage = null;
            this.Smirror_ck.Location = new System.Drawing.Point(1224, 704);
            this.Smirror_ck.Margin = new System.Windows.Forms.Padding(0);
            this.Smirror_ck.Name = "Smirror_ck";
            this.Smirror_ck.Size = new System.Drawing.Size(64, 64);
            this.Smirror_ck.TabIndex = 277;
            this.Smirror_ck.TabStop = false;
            this.Smirror_ck.Click += new System.EventHandler(this.WrapS_Mirror_Click);
            this.Smirror_ck.MouseEnter += new System.EventHandler(this.WrapS_Mirror_MouseEnter);
            this.Smirror_ck.MouseLeave += new System.EventHandler(this.WrapS_Mirror_MouseLeave);
            // 
            // Smirror_label
            // 
            this.Smirror_label.AutoSize = true;
            this.Smirror_label.BackColor = System.Drawing.Color.Transparent;
            this.Smirror_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Smirror_label.ForeColor = System.Drawing.SystemColors.Window;
            this.Smirror_label.Location = new System.Drawing.Point(1288, 704);
            this.Smirror_label.Margin = new System.Windows.Forms.Padding(0);
            this.Smirror_label.Name = "Smirror_label";
            this.Smirror_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Smirror_label.Size = new System.Drawing.Size(76, 68);
            this.Smirror_label.TabIndex = 275;
            this.Smirror_label.Text = "Mirror";
            this.Smirror_label.Click += new System.EventHandler(this.WrapS_Mirror_Click);
            this.Smirror_label.MouseEnter += new System.EventHandler(this.WrapS_Mirror_MouseEnter);
            this.Smirror_label.MouseLeave += new System.EventHandler(this.WrapS_Mirror_MouseLeave);
            // 
            // Srepeat_ck
            // 
            this.Srepeat_ck.BackColor = System.Drawing.Color.Transparent;
            this.Srepeat_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Srepeat_ck.ErrorImage = null;
            this.Srepeat_ck.InitialImage = null;
            this.Srepeat_ck.Location = new System.Drawing.Point(1224, 640);
            this.Srepeat_ck.Margin = new System.Windows.Forms.Padding(0);
            this.Srepeat_ck.Name = "Srepeat_ck";
            this.Srepeat_ck.Size = new System.Drawing.Size(64, 64);
            this.Srepeat_ck.TabIndex = 274;
            this.Srepeat_ck.TabStop = false;
            this.Srepeat_ck.Click += new System.EventHandler(this.WrapS_Repeat_Click);
            this.Srepeat_ck.MouseEnter += new System.EventHandler(this.WrapS_Repeat_MouseEnter);
            this.Srepeat_ck.MouseLeave += new System.EventHandler(this.WrapS_Repeat_MouseLeave);
            // 
            // Srepeat_label
            // 
            this.Srepeat_label.AutoSize = true;
            this.Srepeat_label.BackColor = System.Drawing.Color.Transparent;
            this.Srepeat_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Srepeat_label.ForeColor = System.Drawing.SystemColors.Window;
            this.Srepeat_label.Location = new System.Drawing.Point(1288, 640);
            this.Srepeat_label.Margin = new System.Windows.Forms.Padding(0);
            this.Srepeat_label.Name = "Srepeat_label";
            this.Srepeat_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Srepeat_label.Size = new System.Drawing.Size(80, 68);
            this.Srepeat_label.TabIndex = 272;
            this.Srepeat_label.Text = "Repeat";
            this.Srepeat_label.Click += new System.EventHandler(this.WrapS_Repeat_Click);
            this.Srepeat_label.MouseEnter += new System.EventHandler(this.WrapS_Repeat_MouseEnter);
            this.Srepeat_label.MouseLeave += new System.EventHandler(this.WrapS_Repeat_MouseLeave);
            // 
            // Sclamp_ck
            // 
            this.Sclamp_ck.BackColor = System.Drawing.Color.Transparent;
            this.Sclamp_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Sclamp_ck.ErrorImage = null;
            this.Sclamp_ck.InitialImage = null;
            this.Sclamp_ck.Location = new System.Drawing.Point(1224, 576);
            this.Sclamp_ck.Margin = new System.Windows.Forms.Padding(0);
            this.Sclamp_ck.Name = "Sclamp_ck";
            this.Sclamp_ck.Size = new System.Drawing.Size(64, 64);
            this.Sclamp_ck.TabIndex = 271;
            this.Sclamp_ck.TabStop = false;
            this.Sclamp_ck.Click += new System.EventHandler(this.WrapS_Clamp_Click);
            this.Sclamp_ck.MouseEnter += new System.EventHandler(this.WrapS_Clamp_MouseEnter);
            this.Sclamp_ck.MouseLeave += new System.EventHandler(this.WrapS_Clamp_MouseLeave);
            // 
            // Sclamp_label
            // 
            this.Sclamp_label.AutoSize = true;
            this.Sclamp_label.BackColor = System.Drawing.Color.Transparent;
            this.Sclamp_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Sclamp_label.ForeColor = System.Drawing.SystemColors.Window;
            this.Sclamp_label.Location = new System.Drawing.Point(1288, 576);
            this.Sclamp_label.Margin = new System.Windows.Forms.Padding(0);
            this.Sclamp_label.Name = "Sclamp_label";
            this.Sclamp_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Sclamp_label.Size = new System.Drawing.Size(73, 68);
            this.Sclamp_label.TabIndex = 269;
            this.Sclamp_label.Text = "Clamp";
            this.Sclamp_label.Click += new System.EventHandler(this.WrapS_Clamp_Click);
            this.Sclamp_label.MouseEnter += new System.EventHandler(this.WrapS_Clamp_MouseEnter);
            this.Sclamp_label.MouseLeave += new System.EventHandler(this.WrapS_Clamp_MouseLeave);
            // 
            // WrapS_label
            // 
            this.WrapS_label.AutoSize = true;
            this.WrapS_label.BackColor = System.Drawing.Color.Transparent;
            this.WrapS_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.WrapS_label.ForeColor = System.Drawing.SystemColors.Control;
            this.WrapS_label.Location = new System.Drawing.Point(1273, 546);
            this.WrapS_label.Name = "WrapS_label";
            this.WrapS_label.Size = new System.Drawing.Size(77, 24);
            this.WrapS_label.TabIndex = 268;
            this.WrapS_label.Text = "WrapS";
            // 
            // magnification_label
            // 
            this.magnification_label.AutoSize = true;
            this.magnification_label.BackColor = System.Drawing.Color.Transparent;
            this.magnification_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.magnification_label.ForeColor = System.Drawing.SystemColors.Control;
            this.magnification_label.Location = new System.Drawing.Point(1244, 97);
            this.magnification_label.Name = "magnification_label";
            this.magnification_label.Size = new System.Drawing.Size(204, 24);
            this.magnification_label.TabIndex = 291;
            this.magnification_label.Text = "Magnification filter";
            // 
            // minification_label
            // 
            this.minification_label.AutoSize = true;
            this.minification_label.BackColor = System.Drawing.Color.Transparent;
            this.minification_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.minification_label.ForeColor = System.Drawing.SystemColors.Control;
            this.minification_label.Location = new System.Drawing.Point(851, 97);
            this.minification_label.Name = "minification_label";
            this.minification_label.Size = new System.Drawing.Size(185, 24);
            this.minification_label.TabIndex = 278;
            this.minification_label.Text = "Minification filter";
            // 
            // min_linearmipmaplinear_ck
            // 
            this.min_linearmipmaplinear_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmaplinear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_linearmipmaplinear_ck.ErrorImage = null;
            this.min_linearmipmaplinear_ck.InitialImage = null;
            this.min_linearmipmaplinear_ck.Location = new System.Drawing.Point(824, 448);
            this.min_linearmipmaplinear_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmaplinear_ck.Name = "min_linearmipmaplinear_ck";
            this.min_linearmipmaplinear_ck.Size = new System.Drawing.Size(64, 64);
            this.min_linearmipmaplinear_ck.TabIndex = 324;
            this.min_linearmipmaplinear_ck.TabStop = false;
            this.min_linearmipmaplinear_ck.Click += new System.EventHandler(this.Minification_LinearMipmapLinear_Click);
            this.min_linearmipmaplinear_ck.MouseEnter += new System.EventHandler(this.Minification_LinearMipmapLinear_MouseEnter);
            this.min_linearmipmaplinear_ck.MouseLeave += new System.EventHandler(this.Minification_LinearMipmapLinear_MouseLeave);
            // 
            // min_linearmipmaplinear_label
            // 
            this.min_linearmipmaplinear_label.AutoSize = true;
            this.min_linearmipmaplinear_label.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmaplinear_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.min_linearmipmaplinear_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_linearmipmaplinear_label.Location = new System.Drawing.Point(888, 448);
            this.min_linearmipmaplinear_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmaplinear_label.Name = "min_linearmipmaplinear_label";
            this.min_linearmipmaplinear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_linearmipmaplinear_label.Size = new System.Drawing.Size(216, 68);
            this.min_linearmipmaplinear_label.TabIndex = 322;
            this.min_linearmipmaplinear_label.Text = "LinearMipmapLinear";
            this.min_linearmipmaplinear_label.Click += new System.EventHandler(this.Minification_LinearMipmapLinear_Click);
            this.min_linearmipmaplinear_label.MouseEnter += new System.EventHandler(this.Minification_LinearMipmapLinear_MouseEnter);
            this.min_linearmipmaplinear_label.MouseLeave += new System.EventHandler(this.Minification_LinearMipmapLinear_MouseLeave);
            // 
            // min_linearmipmapnearest_ck
            // 
            this.min_linearmipmapnearest_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmapnearest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_linearmipmapnearest_ck.ErrorImage = null;
            this.min_linearmipmapnearest_ck.InitialImage = null;
            this.min_linearmipmapnearest_ck.Location = new System.Drawing.Point(824, 384);
            this.min_linearmipmapnearest_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmapnearest_ck.Name = "min_linearmipmapnearest_ck";
            this.min_linearmipmapnearest_ck.Size = new System.Drawing.Size(64, 64);
            this.min_linearmipmapnearest_ck.TabIndex = 321;
            this.min_linearmipmapnearest_ck.TabStop = false;
            this.min_linearmipmapnearest_ck.Click += new System.EventHandler(this.Minification_LinearMipmapNearest_Click);
            this.min_linearmipmapnearest_ck.MouseEnter += new System.EventHandler(this.Minification_LinearMipmapNearest_MouseEnter);
            this.min_linearmipmapnearest_ck.MouseLeave += new System.EventHandler(this.Minification_LinearMipmapNearest_MouseLeave);
            // 
            // min_linearmipmapnearest_label
            // 
            this.min_linearmipmapnearest_label.AutoSize = true;
            this.min_linearmipmapnearest_label.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmapnearest_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.min_linearmipmapnearest_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_linearmipmapnearest_label.Location = new System.Drawing.Point(888, 384);
            this.min_linearmipmapnearest_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmapnearest_label.Name = "min_linearmipmapnearest_label";
            this.min_linearmipmapnearest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_linearmipmapnearest_label.Size = new System.Drawing.Size(243, 68);
            this.min_linearmipmapnearest_label.TabIndex = 319;
            this.min_linearmipmapnearest_label.Text = "LinearMipmapNearest  ";
            this.min_linearmipmapnearest_label.Click += new System.EventHandler(this.Minification_LinearMipmapNearest_Click);
            this.min_linearmipmapnearest_label.MouseEnter += new System.EventHandler(this.Minification_LinearMipmapNearest_MouseEnter);
            this.min_linearmipmapnearest_label.MouseLeave += new System.EventHandler(this.Minification_LinearMipmapNearest_MouseLeave);
            // 
            // min_nearestmipmaplinear_ck
            // 
            this.min_nearestmipmaplinear_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmaplinear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_nearestmipmaplinear_ck.ErrorImage = null;
            this.min_nearestmipmaplinear_ck.InitialImage = null;
            this.min_nearestmipmaplinear_ck.Location = new System.Drawing.Point(824, 320);
            this.min_nearestmipmaplinear_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmaplinear_ck.Name = "min_nearestmipmaplinear_ck";
            this.min_nearestmipmaplinear_ck.Size = new System.Drawing.Size(64, 64);
            this.min_nearestmipmaplinear_ck.TabIndex = 318;
            this.min_nearestmipmaplinear_ck.TabStop = false;
            this.min_nearestmipmaplinear_ck.Click += new System.EventHandler(this.Minification_NearestMipmapLinear_Click);
            this.min_nearestmipmaplinear_ck.MouseEnter += new System.EventHandler(this.Minification_NearestMipmapLinear_MouseEnter);
            this.min_nearestmipmaplinear_ck.MouseLeave += new System.EventHandler(this.Minification_NearestMipmapLinear_MouseLeave);
            // 
            // min_nearestmipmaplinear_label
            // 
            this.min_nearestmipmaplinear_label.AutoSize = true;
            this.min_nearestmipmaplinear_label.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmaplinear_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.min_nearestmipmaplinear_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_nearestmipmaplinear_label.Location = new System.Drawing.Point(888, 320);
            this.min_nearestmipmaplinear_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmaplinear_label.Name = "min_nearestmipmaplinear_label";
            this.min_nearestmipmaplinear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_nearestmipmaplinear_label.Size = new System.Drawing.Size(243, 68);
            this.min_nearestmipmaplinear_label.TabIndex = 316;
            this.min_nearestmipmaplinear_label.Text = "NearestMipmapLinear  ";
            this.min_nearestmipmaplinear_label.Click += new System.EventHandler(this.Minification_NearestMipmapLinear_Click);
            this.min_nearestmipmaplinear_label.MouseEnter += new System.EventHandler(this.Minification_NearestMipmapLinear_MouseEnter);
            this.min_nearestmipmaplinear_label.MouseLeave += new System.EventHandler(this.Minification_NearestMipmapLinear_MouseLeave);
            // 
            // min_nearestmipmapnearest_ck
            // 
            this.min_nearestmipmapnearest_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmapnearest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_nearestmipmapnearest_ck.ErrorImage = null;
            this.min_nearestmipmapnearest_ck.InitialImage = null;
            this.min_nearestmipmapnearest_ck.Location = new System.Drawing.Point(824, 256);
            this.min_nearestmipmapnearest_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmapnearest_ck.Name = "min_nearestmipmapnearest_ck";
            this.min_nearestmipmapnearest_ck.Size = new System.Drawing.Size(64, 64);
            this.min_nearestmipmapnearest_ck.TabIndex = 315;
            this.min_nearestmipmapnearest_ck.TabStop = false;
            this.min_nearestmipmapnearest_ck.Click += new System.EventHandler(this.Minification_NearestMipmapNearest_Click);
            this.min_nearestmipmapnearest_ck.MouseEnter += new System.EventHandler(this.Minification_NearestMipmapNearest_MouseEnter);
            this.min_nearestmipmapnearest_ck.MouseLeave += new System.EventHandler(this.Minification_NearestMipmapNearest_MouseLeave);
            // 
            // min_nearestmipmapnearest_label
            // 
            this.min_nearestmipmapnearest_label.AutoSize = true;
            this.min_nearestmipmapnearest_label.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmapnearest_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.min_nearestmipmapnearest_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_nearestmipmapnearest_label.Location = new System.Drawing.Point(888, 256);
            this.min_nearestmipmapnearest_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmapnearest_label.Name = "min_nearestmipmapnearest_label";
            this.min_nearestmipmapnearest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_nearestmipmapnearest_label.Size = new System.Drawing.Size(255, 68);
            this.min_nearestmipmapnearest_label.TabIndex = 313;
            this.min_nearestmipmapnearest_label.Text = "NearestMipmapNearest ";
            this.min_nearestmipmapnearest_label.Click += new System.EventHandler(this.Minification_NearestMipmapNearest_Click);
            this.min_nearestmipmapnearest_label.MouseEnter += new System.EventHandler(this.Minification_NearestMipmapNearest_MouseEnter);
            this.min_nearestmipmapnearest_label.MouseLeave += new System.EventHandler(this.Minification_NearestMipmapNearest_MouseLeave);
            // 
            // min_linear_ck
            // 
            this.min_linear_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_linear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_linear_ck.ErrorImage = null;
            this.min_linear_ck.InitialImage = null;
            this.min_linear_ck.Location = new System.Drawing.Point(824, 192);
            this.min_linear_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_linear_ck.Name = "min_linear_ck";
            this.min_linear_ck.Size = new System.Drawing.Size(64, 64);
            this.min_linear_ck.TabIndex = 312;
            this.min_linear_ck.TabStop = false;
            this.min_linear_ck.Click += new System.EventHandler(this.Minification_Linear_Click);
            this.min_linear_ck.MouseEnter += new System.EventHandler(this.Minification_Linear_MouseEnter);
            this.min_linear_ck.MouseLeave += new System.EventHandler(this.Minification_Linear_MouseLeave);
            // 
            // min_linear_label
            // 
            this.min_linear_label.AutoSize = true;
            this.min_linear_label.BackColor = System.Drawing.Color.Transparent;
            this.min_linear_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.min_linear_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_linear_label.Location = new System.Drawing.Point(888, 192);
            this.min_linear_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_linear_label.Name = "min_linear_label";
            this.min_linear_label.Padding = new System.Windows.Forms.Padding(0, 22, 200, 22);
            this.min_linear_label.Size = new System.Drawing.Size(273, 68);
            this.min_linear_label.TabIndex = 310;
            this.min_linear_label.Text = "Linear";
            this.min_linear_label.Click += new System.EventHandler(this.Minification_Linear_Click);
            this.min_linear_label.MouseEnter += new System.EventHandler(this.Minification_Linear_MouseEnter);
            this.min_linear_label.MouseLeave += new System.EventHandler(this.Minification_Linear_MouseLeave);
            // 
            // min_nearest_neighbour_ck
            // 
            this.min_nearest_neighbour_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_nearest_neighbour_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_nearest_neighbour_ck.ErrorImage = null;
            this.min_nearest_neighbour_ck.InitialImage = null;
            this.min_nearest_neighbour_ck.Location = new System.Drawing.Point(824, 128);
            this.min_nearest_neighbour_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearest_neighbour_ck.Name = "min_nearest_neighbour_ck";
            this.min_nearest_neighbour_ck.Size = new System.Drawing.Size(64, 64);
            this.min_nearest_neighbour_ck.TabIndex = 309;
            this.min_nearest_neighbour_ck.TabStop = false;
            this.min_nearest_neighbour_ck.Click += new System.EventHandler(this.Minification_Nearest_Neighbour_Click);
            this.min_nearest_neighbour_ck.MouseEnter += new System.EventHandler(this.Minification_Nearest_Neighbour_MouseEnter);
            this.min_nearest_neighbour_ck.MouseLeave += new System.EventHandler(this.Minification_Nearest_Neighbour_MouseLeave);
            // 
            // min_nearest_neighbour_label
            // 
            this.min_nearest_neighbour_label.AutoSize = true;
            this.min_nearest_neighbour_label.BackColor = System.Drawing.Color.Transparent;
            this.min_nearest_neighbour_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.min_nearest_neighbour_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_nearest_neighbour_label.Location = new System.Drawing.Point(888, 128);
            this.min_nearest_neighbour_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearest_neighbour_label.Name = "min_nearest_neighbour_label";
            this.min_nearest_neighbour_label.Padding = new System.Windows.Forms.Padding(0, 22, 60, 22);
            this.min_nearest_neighbour_label.Size = new System.Drawing.Size(262, 68);
            this.min_nearest_neighbour_label.TabIndex = 307;
            this.min_nearest_neighbour_label.Text = "Nearest Neighbour";
            this.min_nearest_neighbour_label.Click += new System.EventHandler(this.Minification_Nearest_Neighbour_Click);
            this.min_nearest_neighbour_label.MouseEnter += new System.EventHandler(this.Minification_Nearest_Neighbour_MouseEnter);
            this.min_nearest_neighbour_label.MouseLeave += new System.EventHandler(this.Minification_Nearest_Neighbour_MouseLeave);
            // 
            // mag_linearmipmaplinear_ck
            // 
            this.mag_linearmipmaplinear_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmaplinear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_linearmipmaplinear_ck.ErrorImage = null;
            this.mag_linearmipmaplinear_ck.InitialImage = null;
            this.mag_linearmipmaplinear_ck.Location = new System.Drawing.Point(1224, 448);
            this.mag_linearmipmaplinear_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmaplinear_ck.Name = "mag_linearmipmaplinear_ck";
            this.mag_linearmipmaplinear_ck.Size = new System.Drawing.Size(64, 64);
            this.mag_linearmipmaplinear_ck.TabIndex = 342;
            this.mag_linearmipmaplinear_ck.TabStop = false;
            this.mag_linearmipmaplinear_ck.Click += new System.EventHandler(this.Magnification_LinearMipmapLinear_Click);
            this.mag_linearmipmaplinear_ck.MouseEnter += new System.EventHandler(this.Magnification_LinearMipmapLinear_MouseEnter);
            this.mag_linearmipmaplinear_ck.MouseLeave += new System.EventHandler(this.Magnification_LinearMipmapLinear_MouseLeave);
            // 
            // mag_linearmipmaplinear_label
            // 
            this.mag_linearmipmaplinear_label.AutoSize = true;
            this.mag_linearmipmaplinear_label.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmaplinear_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.mag_linearmipmaplinear_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mag_linearmipmaplinear_label.Location = new System.Drawing.Point(1288, 448);
            this.mag_linearmipmaplinear_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmaplinear_label.Name = "mag_linearmipmaplinear_label";
            this.mag_linearmipmaplinear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_linearmipmaplinear_label.Size = new System.Drawing.Size(216, 68);
            this.mag_linearmipmaplinear_label.TabIndex = 340;
            this.mag_linearmipmaplinear_label.Text = "LinearMipmapLinear";
            this.mag_linearmipmaplinear_label.Click += new System.EventHandler(this.Magnification_LinearMipmapLinear_Click);
            this.mag_linearmipmaplinear_label.MouseEnter += new System.EventHandler(this.Magnification_LinearMipmapLinear_MouseEnter);
            this.mag_linearmipmaplinear_label.MouseLeave += new System.EventHandler(this.Magnification_LinearMipmapLinear_MouseLeave);
            // 
            // mag_linearmipmapnearest_ck
            // 
            this.mag_linearmipmapnearest_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmapnearest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_linearmipmapnearest_ck.ErrorImage = null;
            this.mag_linearmipmapnearest_ck.InitialImage = null;
            this.mag_linearmipmapnearest_ck.Location = new System.Drawing.Point(1224, 384);
            this.mag_linearmipmapnearest_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmapnearest_ck.Name = "mag_linearmipmapnearest_ck";
            this.mag_linearmipmapnearest_ck.Size = new System.Drawing.Size(64, 64);
            this.mag_linearmipmapnearest_ck.TabIndex = 339;
            this.mag_linearmipmapnearest_ck.TabStop = false;
            this.mag_linearmipmapnearest_ck.Click += new System.EventHandler(this.Magnification_LinearMipmapNearest_Click);
            this.mag_linearmipmapnearest_ck.MouseEnter += new System.EventHandler(this.Magnification_LinearMipmapNearest_MouseEnter);
            this.mag_linearmipmapnearest_ck.MouseLeave += new System.EventHandler(this.Magnification_LinearMipmapNearest_MouseLeave);
            // 
            // mag_linearmipmapnearest_label
            // 
            this.mag_linearmipmapnearest_label.AutoSize = true;
            this.mag_linearmipmapnearest_label.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmapnearest_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.mag_linearmipmapnearest_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mag_linearmipmapnearest_label.Location = new System.Drawing.Point(1288, 384);
            this.mag_linearmipmapnearest_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmapnearest_label.Name = "mag_linearmipmapnearest_label";
            this.mag_linearmipmapnearest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_linearmipmapnearest_label.Size = new System.Drawing.Size(243, 68);
            this.mag_linearmipmapnearest_label.TabIndex = 337;
            this.mag_linearmipmapnearest_label.Text = "LinearMipmapNearest  ";
            this.mag_linearmipmapnearest_label.Click += new System.EventHandler(this.Magnification_LinearMipmapNearest_Click);
            this.mag_linearmipmapnearest_label.MouseEnter += new System.EventHandler(this.Magnification_LinearMipmapNearest_MouseEnter);
            this.mag_linearmipmapnearest_label.MouseLeave += new System.EventHandler(this.Magnification_LinearMipmapNearest_MouseLeave);
            // 
            // mag_nearestmipmaplinear_ck
            // 
            this.mag_nearestmipmaplinear_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmaplinear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_nearestmipmaplinear_ck.ErrorImage = null;
            this.mag_nearestmipmaplinear_ck.InitialImage = null;
            this.mag_nearestmipmaplinear_ck.Location = new System.Drawing.Point(1224, 320);
            this.mag_nearestmipmaplinear_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmaplinear_ck.Name = "mag_nearestmipmaplinear_ck";
            this.mag_nearestmipmaplinear_ck.Size = new System.Drawing.Size(64, 64);
            this.mag_nearestmipmaplinear_ck.TabIndex = 336;
            this.mag_nearestmipmaplinear_ck.TabStop = false;
            this.mag_nearestmipmaplinear_ck.Click += new System.EventHandler(this.Magnification_NearestMipmapLinear_Click);
            this.mag_nearestmipmaplinear_ck.MouseEnter += new System.EventHandler(this.Magnification_NearestMipmapLinear_MouseEnter);
            this.mag_nearestmipmaplinear_ck.MouseLeave += new System.EventHandler(this.Magnification_NearestMipmapLinear_MouseLeave);
            // 
            // mag_nearestmipmaplinear_label
            // 
            this.mag_nearestmipmaplinear_label.AutoSize = true;
            this.mag_nearestmipmaplinear_label.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmaplinear_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.mag_nearestmipmaplinear_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mag_nearestmipmaplinear_label.Location = new System.Drawing.Point(1288, 320);
            this.mag_nearestmipmaplinear_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmaplinear_label.Name = "mag_nearestmipmaplinear_label";
            this.mag_nearestmipmaplinear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_nearestmipmaplinear_label.Size = new System.Drawing.Size(243, 68);
            this.mag_nearestmipmaplinear_label.TabIndex = 334;
            this.mag_nearestmipmaplinear_label.Text = "NearestMipmapLinear  ";
            this.mag_nearestmipmaplinear_label.Click += new System.EventHandler(this.Magnification_NearestMipmapLinear_Click);
            this.mag_nearestmipmaplinear_label.MouseEnter += new System.EventHandler(this.Magnification_NearestMipmapLinear_MouseEnter);
            this.mag_nearestmipmaplinear_label.MouseLeave += new System.EventHandler(this.Magnification_NearestMipmapLinear_MouseLeave);
            // 
            // mag_nearestmipmapnearest_ck
            // 
            this.mag_nearestmipmapnearest_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmapnearest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_nearestmipmapnearest_ck.ErrorImage = null;
            this.mag_nearestmipmapnearest_ck.InitialImage = null;
            this.mag_nearestmipmapnearest_ck.Location = new System.Drawing.Point(1224, 256);
            this.mag_nearestmipmapnearest_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmapnearest_ck.Name = "mag_nearestmipmapnearest_ck";
            this.mag_nearestmipmapnearest_ck.Size = new System.Drawing.Size(64, 64);
            this.mag_nearestmipmapnearest_ck.TabIndex = 333;
            this.mag_nearestmipmapnearest_ck.TabStop = false;
            this.mag_nearestmipmapnearest_ck.Click += new System.EventHandler(this.Magnification_NearestMipmapNearest_Click);
            this.mag_nearestmipmapnearest_ck.MouseEnter += new System.EventHandler(this.Magnification_NearestMipmapNearest_MouseEnter);
            this.mag_nearestmipmapnearest_ck.MouseLeave += new System.EventHandler(this.Magnification_NearestMipmapNearest_MouseLeave);
            // 
            // mag_nearestmipmapnearest_label
            // 
            this.mag_nearestmipmapnearest_label.AutoSize = true;
            this.mag_nearestmipmapnearest_label.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmapnearest_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.mag_nearestmipmapnearest_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mag_nearestmipmapnearest_label.Location = new System.Drawing.Point(1288, 256);
            this.mag_nearestmipmapnearest_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmapnearest_label.Name = "mag_nearestmipmapnearest_label";
            this.mag_nearestmipmapnearest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_nearestmipmapnearest_label.Size = new System.Drawing.Size(255, 68);
            this.mag_nearestmipmapnearest_label.TabIndex = 331;
            this.mag_nearestmipmapnearest_label.Text = "NearestMipmapNearest ";
            this.mag_nearestmipmapnearest_label.Click += new System.EventHandler(this.Magnification_NearestMipmapNearest_Click);
            this.mag_nearestmipmapnearest_label.MouseEnter += new System.EventHandler(this.Magnification_NearestMipmapNearest_MouseEnter);
            this.mag_nearestmipmapnearest_label.MouseLeave += new System.EventHandler(this.Magnification_NearestMipmapNearest_MouseLeave);
            // 
            // mag_linear_ck
            // 
            this.mag_linear_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_linear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_linear_ck.ErrorImage = null;
            this.mag_linear_ck.InitialImage = null;
            this.mag_linear_ck.Location = new System.Drawing.Point(1224, 192);
            this.mag_linear_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linear_ck.Name = "mag_linear_ck";
            this.mag_linear_ck.Size = new System.Drawing.Size(64, 64);
            this.mag_linear_ck.TabIndex = 330;
            this.mag_linear_ck.TabStop = false;
            this.mag_linear_ck.Click += new System.EventHandler(this.Magnification_Linear_Click);
            this.mag_linear_ck.MouseEnter += new System.EventHandler(this.Magnification_Linear_MouseEnter);
            this.mag_linear_ck.MouseLeave += new System.EventHandler(this.Magnification_Linear_MouseLeave);
            // 
            // mag_linear_label
            // 
            this.mag_linear_label.AutoSize = true;
            this.mag_linear_label.BackColor = System.Drawing.Color.Transparent;
            this.mag_linear_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.mag_linear_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mag_linear_label.Location = new System.Drawing.Point(1288, 192);
            this.mag_linear_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linear_label.Name = "mag_linear_label";
            this.mag_linear_label.Padding = new System.Windows.Forms.Padding(0, 22, 200, 22);
            this.mag_linear_label.Size = new System.Drawing.Size(273, 68);
            this.mag_linear_label.TabIndex = 328;
            this.mag_linear_label.Text = "Linear";
            this.mag_linear_label.Click += new System.EventHandler(this.Magnification_Linear_Click);
            this.mag_linear_label.MouseEnter += new System.EventHandler(this.Magnification_Linear_MouseEnter);
            this.mag_linear_label.MouseLeave += new System.EventHandler(this.Magnification_Linear_MouseLeave);
            // 
            // mag_nearest_neighbour_ck
            // 
            this.mag_nearest_neighbour_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearest_neighbour_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_nearest_neighbour_ck.ErrorImage = null;
            this.mag_nearest_neighbour_ck.InitialImage = null;
            this.mag_nearest_neighbour_ck.Location = new System.Drawing.Point(1224, 128);
            this.mag_nearest_neighbour_ck.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearest_neighbour_ck.Name = "mag_nearest_neighbour_ck";
            this.mag_nearest_neighbour_ck.Size = new System.Drawing.Size(64, 64);
            this.mag_nearest_neighbour_ck.TabIndex = 327;
            this.mag_nearest_neighbour_ck.TabStop = false;
            this.mag_nearest_neighbour_ck.Click += new System.EventHandler(this.Magnification_Nearest_Neighbour_Click);
            this.mag_nearest_neighbour_ck.MouseEnter += new System.EventHandler(this.Magnification_Nearest_Neighbour_MouseEnter);
            this.mag_nearest_neighbour_ck.MouseLeave += new System.EventHandler(this.Magnification_Nearest_Neighbour_MouseLeave);
            // 
            // mag_nearest_neighbour_label
            // 
            this.mag_nearest_neighbour_label.AutoSize = true;
            this.mag_nearest_neighbour_label.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearest_neighbour_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.mag_nearest_neighbour_label.ForeColor = System.Drawing.SystemColors.Window;
            this.mag_nearest_neighbour_label.Location = new System.Drawing.Point(1288, 128);
            this.mag_nearest_neighbour_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearest_neighbour_label.Name = "mag_nearest_neighbour_label";
            this.mag_nearest_neighbour_label.Padding = new System.Windows.Forms.Padding(0, 22, 60, 22);
            this.mag_nearest_neighbour_label.Size = new System.Drawing.Size(262, 68);
            this.mag_nearest_neighbour_label.TabIndex = 325;
            this.mag_nearest_neighbour_label.Text = "Nearest Neighbour";
            this.mag_nearest_neighbour_label.Click += new System.EventHandler(this.Magnification_Nearest_Neighbour_Click);
            this.mag_nearest_neighbour_label.MouseEnter += new System.EventHandler(this.Magnification_Nearest_Neighbour_MouseEnter);
            this.mag_nearest_neighbour_label.MouseLeave += new System.EventHandler(this.Magnification_Nearest_Neighbour_MouseLeave);
            // 
            // r_r_ck
            // 
            this.r_r_ck.BackColor = System.Drawing.Color.Transparent;
            this.r_r_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.r_r_ck.ErrorImage = null;
            this.r_r_ck.InitialImage = null;
            this.r_r_ck.Location = new System.Drawing.Point(1648, 813);
            this.r_r_ck.Margin = new System.Windows.Forms.Padding(0);
            this.r_r_ck.Name = "r_r_ck";
            this.r_r_ck.Size = new System.Drawing.Size(64, 64);
            this.r_r_ck.TabIndex = 343;
            this.r_r_ck.TabStop = false;
            this.r_r_ck.Click += new System.EventHandler(this.R_R_Click);
            this.r_r_ck.MouseEnter += new System.EventHandler(this.R_R_MouseEnter);
            this.r_r_ck.MouseLeave += new System.EventHandler(this.R_R_MouseLeave);
            // 
            // r_g_ck
            // 
            this.r_g_ck.BackColor = System.Drawing.Color.Transparent;
            this.r_g_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.r_g_ck.ErrorImage = null;
            this.r_g_ck.InitialImage = null;
            this.r_g_ck.Location = new System.Drawing.Point(1648, 877);
            this.r_g_ck.Margin = new System.Windows.Forms.Padding(0);
            this.r_g_ck.Name = "r_g_ck";
            this.r_g_ck.Size = new System.Drawing.Size(64, 64);
            this.r_g_ck.TabIndex = 344;
            this.r_g_ck.TabStop = false;
            this.r_g_ck.Click += new System.EventHandler(this.R_G_Click);
            this.r_g_ck.MouseEnter += new System.EventHandler(this.R_G_MouseEnter);
            this.r_g_ck.MouseLeave += new System.EventHandler(this.R_G_MouseLeave);
            // 
            // g_r_ck
            // 
            this.g_r_ck.BackColor = System.Drawing.Color.Transparent;
            this.g_r_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.g_r_ck.ErrorImage = null;
            this.g_r_ck.InitialImage = null;
            this.g_r_ck.Location = new System.Drawing.Point(1712, 813);
            this.g_r_ck.Margin = new System.Windows.Forms.Padding(0);
            this.g_r_ck.Name = "g_r_ck";
            this.g_r_ck.Size = new System.Drawing.Size(64, 64);
            this.g_r_ck.TabIndex = 345;
            this.g_r_ck.TabStop = false;
            this.g_r_ck.Click += new System.EventHandler(this.G_R_Click);
            this.g_r_ck.MouseEnter += new System.EventHandler(this.G_R_MouseEnter);
            this.g_r_ck.MouseLeave += new System.EventHandler(this.G_R_MouseLeave);
            // 
            // g_g_ck
            // 
            this.g_g_ck.BackColor = System.Drawing.Color.Transparent;
            this.g_g_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.g_g_ck.ErrorImage = null;
            this.g_g_ck.InitialImage = null;
            this.g_g_ck.Location = new System.Drawing.Point(1712, 877);
            this.g_g_ck.Margin = new System.Windows.Forms.Padding(0);
            this.g_g_ck.Name = "g_g_ck";
            this.g_g_ck.Size = new System.Drawing.Size(64, 64);
            this.g_g_ck.TabIndex = 346;
            this.g_g_ck.TabStop = false;
            this.g_g_ck.Click += new System.EventHandler(this.G_G_Click);
            this.g_g_ck.MouseEnter += new System.EventHandler(this.G_G_MouseEnter);
            this.g_g_ck.MouseLeave += new System.EventHandler(this.G_G_MouseLeave);
            // 
            // a_g_ck
            // 
            this.a_g_ck.BackColor = System.Drawing.Color.Transparent;
            this.a_g_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.a_g_ck.ErrorImage = null;
            this.a_g_ck.InitialImage = null;
            this.a_g_ck.Location = new System.Drawing.Point(1840, 877);
            this.a_g_ck.Margin = new System.Windows.Forms.Padding(0);
            this.a_g_ck.Name = "a_g_ck";
            this.a_g_ck.Size = new System.Drawing.Size(64, 64);
            this.a_g_ck.TabIndex = 350;
            this.a_g_ck.TabStop = false;
            this.a_g_ck.Click += new System.EventHandler(this.A_G_Click);
            this.a_g_ck.MouseEnter += new System.EventHandler(this.A_G_MouseEnter);
            this.a_g_ck.MouseLeave += new System.EventHandler(this.A_G_MouseLeave);
            // 
            // a_r_ck
            // 
            this.a_r_ck.BackColor = System.Drawing.Color.Transparent;
            this.a_r_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.a_r_ck.ErrorImage = null;
            this.a_r_ck.InitialImage = null;
            this.a_r_ck.Location = new System.Drawing.Point(1840, 813);
            this.a_r_ck.Margin = new System.Windows.Forms.Padding(0);
            this.a_r_ck.Name = "a_r_ck";
            this.a_r_ck.Size = new System.Drawing.Size(64, 64);
            this.a_r_ck.TabIndex = 349;
            this.a_r_ck.TabStop = false;
            this.a_r_ck.Click += new System.EventHandler(this.A_R_Click);
            this.a_r_ck.MouseEnter += new System.EventHandler(this.A_R_MouseEnter);
            this.a_r_ck.MouseLeave += new System.EventHandler(this.A_R_MouseLeave);
            // 
            // b_g_ck
            // 
            this.b_g_ck.BackColor = System.Drawing.Color.Transparent;
            this.b_g_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.b_g_ck.ErrorImage = null;
            this.b_g_ck.InitialImage = null;
            this.b_g_ck.Location = new System.Drawing.Point(1776, 877);
            this.b_g_ck.Margin = new System.Windows.Forms.Padding(0);
            this.b_g_ck.Name = "b_g_ck";
            this.b_g_ck.Size = new System.Drawing.Size(64, 64);
            this.b_g_ck.TabIndex = 348;
            this.b_g_ck.TabStop = false;
            this.b_g_ck.Click += new System.EventHandler(this.B_G_Click);
            this.b_g_ck.MouseEnter += new System.EventHandler(this.B_G_MouseEnter);
            this.b_g_ck.MouseLeave += new System.EventHandler(this.B_G_MouseLeave);
            // 
            // b_r_ck
            // 
            this.b_r_ck.BackColor = System.Drawing.Color.Transparent;
            this.b_r_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.b_r_ck.ErrorImage = null;
            this.b_r_ck.InitialImage = null;
            this.b_r_ck.Location = new System.Drawing.Point(1776, 813);
            this.b_r_ck.Margin = new System.Windows.Forms.Padding(0);
            this.b_r_ck.Name = "b_r_ck";
            this.b_r_ck.Size = new System.Drawing.Size(64, 64);
            this.b_r_ck.TabIndex = 347;
            this.b_r_ck.TabStop = false;
            this.b_r_ck.Click += new System.EventHandler(this.B_R_Click);
            this.b_r_ck.MouseEnter += new System.EventHandler(this.B_R_MouseEnter);
            this.b_r_ck.MouseLeave += new System.EventHandler(this.B_R_MouseLeave);
            // 
            // g_a_ck
            // 
            this.g_a_ck.BackColor = System.Drawing.Color.Transparent;
            this.g_a_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.g_a_ck.ErrorImage = null;
            this.g_a_ck.InitialImage = null;
            this.g_a_ck.Location = new System.Drawing.Point(1712, 1005);
            this.g_a_ck.Margin = new System.Windows.Forms.Padding(0);
            this.g_a_ck.Name = "g_a_ck";
            this.g_a_ck.Size = new System.Drawing.Size(64, 64);
            this.g_a_ck.TabIndex = 354;
            this.g_a_ck.TabStop = false;
            this.g_a_ck.Click += new System.EventHandler(this.G_A_Click);
            this.g_a_ck.MouseEnter += new System.EventHandler(this.G_A_MouseEnter);
            this.g_a_ck.MouseLeave += new System.EventHandler(this.G_A_MouseLeave);
            // 
            // g_b_ck
            // 
            this.g_b_ck.BackColor = System.Drawing.Color.Transparent;
            this.g_b_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.g_b_ck.ErrorImage = null;
            this.g_b_ck.InitialImage = null;
            this.g_b_ck.Location = new System.Drawing.Point(1712, 941);
            this.g_b_ck.Margin = new System.Windows.Forms.Padding(0);
            this.g_b_ck.Name = "g_b_ck";
            this.g_b_ck.Size = new System.Drawing.Size(64, 64);
            this.g_b_ck.TabIndex = 353;
            this.g_b_ck.TabStop = false;
            this.g_b_ck.Click += new System.EventHandler(this.G_B_Click);
            this.g_b_ck.MouseEnter += new System.EventHandler(this.G_B_MouseEnter);
            this.g_b_ck.MouseLeave += new System.EventHandler(this.G_B_MouseLeave);
            // 
            // r_a_ck
            // 
            this.r_a_ck.BackColor = System.Drawing.Color.Transparent;
            this.r_a_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.r_a_ck.ErrorImage = null;
            this.r_a_ck.InitialImage = null;
            this.r_a_ck.Location = new System.Drawing.Point(1648, 1005);
            this.r_a_ck.Margin = new System.Windows.Forms.Padding(0);
            this.r_a_ck.Name = "r_a_ck";
            this.r_a_ck.Size = new System.Drawing.Size(64, 64);
            this.r_a_ck.TabIndex = 352;
            this.r_a_ck.TabStop = false;
            this.r_a_ck.Click += new System.EventHandler(this.R_A_Click);
            this.r_a_ck.MouseEnter += new System.EventHandler(this.R_A_MouseEnter);
            this.r_a_ck.MouseLeave += new System.EventHandler(this.R_A_MouseLeave);
            // 
            // r_b_ck
            // 
            this.r_b_ck.BackColor = System.Drawing.Color.Transparent;
            this.r_b_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.r_b_ck.ErrorImage = null;
            this.r_b_ck.InitialImage = null;
            this.r_b_ck.Location = new System.Drawing.Point(1648, 941);
            this.r_b_ck.Margin = new System.Windows.Forms.Padding(0);
            this.r_b_ck.Name = "r_b_ck";
            this.r_b_ck.Size = new System.Drawing.Size(64, 64);
            this.r_b_ck.TabIndex = 351;
            this.r_b_ck.TabStop = false;
            this.r_b_ck.Click += new System.EventHandler(this.R_B_Click);
            this.r_b_ck.MouseEnter += new System.EventHandler(this.R_B_MouseEnter);
            this.r_b_ck.MouseLeave += new System.EventHandler(this.R_B_MouseLeave);
            // 
            // a_a_ck
            // 
            this.a_a_ck.BackColor = System.Drawing.Color.Transparent;
            this.a_a_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.a_a_ck.ErrorImage = null;
            this.a_a_ck.InitialImage = null;
            this.a_a_ck.Location = new System.Drawing.Point(1840, 1005);
            this.a_a_ck.Margin = new System.Windows.Forms.Padding(0);
            this.a_a_ck.Name = "a_a_ck";
            this.a_a_ck.Size = new System.Drawing.Size(64, 64);
            this.a_a_ck.TabIndex = 358;
            this.a_a_ck.TabStop = false;
            this.a_a_ck.Click += new System.EventHandler(this.A_A_Click);
            this.a_a_ck.MouseEnter += new System.EventHandler(this.A_A_MouseEnter);
            this.a_a_ck.MouseLeave += new System.EventHandler(this.A_A_MouseLeave);
            // 
            // a_b_ck
            // 
            this.a_b_ck.BackColor = System.Drawing.Color.Transparent;
            this.a_b_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.a_b_ck.ErrorImage = null;
            this.a_b_ck.InitialImage = null;
            this.a_b_ck.Location = new System.Drawing.Point(1840, 941);
            this.a_b_ck.Margin = new System.Windows.Forms.Padding(0);
            this.a_b_ck.Name = "a_b_ck";
            this.a_b_ck.Size = new System.Drawing.Size(64, 64);
            this.a_b_ck.TabIndex = 357;
            this.a_b_ck.TabStop = false;
            this.a_b_ck.Click += new System.EventHandler(this.A_B_Click);
            this.a_b_ck.MouseEnter += new System.EventHandler(this.A_B_MouseEnter);
            this.a_b_ck.MouseLeave += new System.EventHandler(this.A_B_MouseLeave);
            // 
            // b_a_ck
            // 
            this.b_a_ck.BackColor = System.Drawing.Color.Transparent;
            this.b_a_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.b_a_ck.ErrorImage = null;
            this.b_a_ck.InitialImage = null;
            this.b_a_ck.Location = new System.Drawing.Point(1776, 1005);
            this.b_a_ck.Margin = new System.Windows.Forms.Padding(0);
            this.b_a_ck.Name = "b_a_ck";
            this.b_a_ck.Size = new System.Drawing.Size(64, 64);
            this.b_a_ck.TabIndex = 356;
            this.b_a_ck.TabStop = false;
            this.b_a_ck.Click += new System.EventHandler(this.B_A_Click);
            this.b_a_ck.MouseEnter += new System.EventHandler(this.B_A_MouseEnter);
            this.b_a_ck.MouseLeave += new System.EventHandler(this.B_A_MouseLeave);
            // 
            // b_b_ck
            // 
            this.b_b_ck.BackColor = System.Drawing.Color.Transparent;
            this.b_b_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.b_b_ck.ErrorImage = null;
            this.b_b_ck.InitialImage = null;
            this.b_b_ck.Location = new System.Drawing.Point(1776, 941);
            this.b_b_ck.Margin = new System.Windows.Forms.Padding(0);
            this.b_b_ck.Name = "b_b_ck";
            this.b_b_ck.Size = new System.Drawing.Size(64, 64);
            this.b_b_ck.TabIndex = 355;
            this.b_b_ck.TabStop = false;
            this.b_b_ck.Click += new System.EventHandler(this.B_B_Click);
            this.b_b_ck.MouseEnter += new System.EventHandler(this.B_B_MouseEnter);
            this.b_b_ck.MouseLeave += new System.EventHandler(this.B_B_MouseLeave);
            // 
            // colour_channels_label
            // 
            this.colour_channels_label.AutoSize = true;
            this.colour_channels_label.BackColor = System.Drawing.Color.Transparent;
            this.colour_channels_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.colour_channels_label.ForeColor = System.Drawing.SystemColors.Control;
            this.colour_channels_label.Location = new System.Drawing.Point(1674, 778);
            this.colour_channels_label.Name = "colour_channels_label";
            this.colour_channels_label.Size = new System.Drawing.Size(175, 24);
            this.colour_channels_label.TabIndex = 359;
            this.colour_channels_label.Text = "Colour Channels";
            // 
            // view_alpha_ck
            // 
            this.view_alpha_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_alpha_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_alpha_ck.ErrorImage = null;
            this.view_alpha_ck.InitialImage = null;
            this.view_alpha_ck.Location = new System.Drawing.Point(40, 1131);
            this.view_alpha_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_alpha_ck.Name = "view_alpha_ck";
            this.view_alpha_ck.Size = new System.Drawing.Size(64, 64);
            this.view_alpha_ck.TabIndex = 378;
            this.view_alpha_ck.TabStop = false;
            this.view_alpha_ck.Visible = false;
            this.view_alpha_ck.Click += new System.EventHandler(this.view_alpha_Click);
            this.view_alpha_ck.MouseEnter += new System.EventHandler(this.view_alpha_MouseEnter);
            this.view_alpha_ck.MouseLeave += new System.EventHandler(this.view_alpha_MouseLeave);
            // 
            // view_alpha_label
            // 
            this.view_alpha_label.AutoSize = true;
            this.view_alpha_label.BackColor = System.Drawing.Color.Transparent;
            this.view_alpha_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.view_alpha_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_alpha_label.Location = new System.Drawing.Point(104, 1131);
            this.view_alpha_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_alpha_label.Name = "view_alpha_label";
            this.view_alpha_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_alpha_label.Size = new System.Drawing.Size(68, 68);
            this.view_alpha_label.TabIndex = 376;
            this.view_alpha_label.Text = "Alpha";
            this.view_alpha_label.Visible = false;
            this.view_alpha_label.Click += new System.EventHandler(this.view_alpha_Click);
            this.view_alpha_label.MouseEnter += new System.EventHandler(this.view_alpha_MouseEnter);
            this.view_alpha_label.MouseLeave += new System.EventHandler(this.view_alpha_MouseLeave);
            // 
            // view_algorithm_ck
            // 
            this.view_algorithm_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_algorithm_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_algorithm_ck.ErrorImage = null;
            this.view_algorithm_ck.InitialImage = null;
            this.view_algorithm_ck.Location = new System.Drawing.Point(239, 1131);
            this.view_algorithm_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_algorithm_ck.Name = "view_algorithm_ck";
            this.view_algorithm_ck.Size = new System.Drawing.Size(64, 64);
            this.view_algorithm_ck.TabIndex = 381;
            this.view_algorithm_ck.TabStop = false;
            this.view_algorithm_ck.Visible = false;
            this.view_algorithm_ck.Click += new System.EventHandler(this.view_algorithm_Click);
            this.view_algorithm_ck.MouseEnter += new System.EventHandler(this.view_algorithm_MouseEnter);
            this.view_algorithm_ck.MouseLeave += new System.EventHandler(this.view_algorithm_MouseLeave);
            // 
            // view_algorithm_label
            // 
            this.view_algorithm_label.AutoSize = true;
            this.view_algorithm_label.BackColor = System.Drawing.Color.Transparent;
            this.view_algorithm_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.view_algorithm_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_algorithm_label.Location = new System.Drawing.Point(307, 1131);
            this.view_algorithm_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_algorithm_label.Name = "view_algorithm_label";
            this.view_algorithm_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_algorithm_label.Size = new System.Drawing.Size(111, 68);
            this.view_algorithm_label.TabIndex = 379;
            this.view_algorithm_label.Text = "Algorithm";
            this.view_algorithm_label.Visible = false;
            this.view_algorithm_label.Click += new System.EventHandler(this.view_algorithm_Click);
            this.view_algorithm_label.MouseEnter += new System.EventHandler(this.view_algorithm_MouseEnter);
            this.view_algorithm_label.MouseLeave += new System.EventHandler(this.view_algorithm_MouseLeave);
            // 
            // view_WrapS_ck
            // 
            this.view_WrapS_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_WrapS_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_WrapS_ck.ErrorImage = null;
            this.view_WrapS_ck.InitialImage = null;
            this.view_WrapS_ck.Location = new System.Drawing.Point(40, 1195);
            this.view_WrapS_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_WrapS_ck.Name = "view_WrapS_ck";
            this.view_WrapS_ck.Size = new System.Drawing.Size(64, 64);
            this.view_WrapS_ck.TabIndex = 384;
            this.view_WrapS_ck.TabStop = false;
            this.view_WrapS_ck.Visible = false;
            this.view_WrapS_ck.Click += new System.EventHandler(this.view_WrapS_Click);
            this.view_WrapS_ck.MouseEnter += new System.EventHandler(this.view_WrapS_MouseEnter);
            this.view_WrapS_ck.MouseLeave += new System.EventHandler(this.view_WrapS_MouseLeave);
            // 
            // view_WrapS_label
            // 
            this.view_WrapS_label.AutoSize = true;
            this.view_WrapS_label.BackColor = System.Drawing.Color.Transparent;
            this.view_WrapS_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.view_WrapS_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_WrapS_label.Location = new System.Drawing.Point(104, 1195);
            this.view_WrapS_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_WrapS_label.Name = "view_WrapS_label";
            this.view_WrapS_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_WrapS_label.Size = new System.Drawing.Size(77, 68);
            this.view_WrapS_label.TabIndex = 382;
            this.view_WrapS_label.Text = "WrapS";
            this.view_WrapS_label.Visible = false;
            this.view_WrapS_label.Click += new System.EventHandler(this.view_WrapS_Click);
            this.view_WrapS_label.MouseEnter += new System.EventHandler(this.view_WrapS_MouseEnter);
            this.view_WrapS_label.MouseLeave += new System.EventHandler(this.view_WrapS_MouseLeave);
            // 
            // view_WrapT_ck
            // 
            this.view_WrapT_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_WrapT_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_WrapT_ck.ErrorImage = null;
            this.view_WrapT_ck.InitialImage = null;
            this.view_WrapT_ck.Location = new System.Drawing.Point(239, 1195);
            this.view_WrapT_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_WrapT_ck.Name = "view_WrapT_ck";
            this.view_WrapT_ck.Size = new System.Drawing.Size(64, 64);
            this.view_WrapT_ck.TabIndex = 387;
            this.view_WrapT_ck.TabStop = false;
            this.view_WrapT_ck.Visible = false;
            this.view_WrapT_ck.Click += new System.EventHandler(this.view_WrapT_Click);
            this.view_WrapT_ck.MouseEnter += new System.EventHandler(this.view_WrapT_MouseEnter);
            this.view_WrapT_ck.MouseLeave += new System.EventHandler(this.view_WrapT_MouseLeave);
            // 
            // view_WrapT_label
            // 
            this.view_WrapT_label.AutoSize = true;
            this.view_WrapT_label.BackColor = System.Drawing.Color.Transparent;
            this.view_WrapT_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.view_WrapT_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_WrapT_label.Location = new System.Drawing.Point(307, 1195);
            this.view_WrapT_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_WrapT_label.Name = "view_WrapT_label";
            this.view_WrapT_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_WrapT_label.Size = new System.Drawing.Size(78, 68);
            this.view_WrapT_label.TabIndex = 385;
            this.view_WrapT_label.Text = "WrapT";
            this.view_WrapT_label.Visible = false;
            this.view_WrapT_label.Click += new System.EventHandler(this.view_WrapT_Click);
            this.view_WrapT_label.MouseEnter += new System.EventHandler(this.view_WrapT_MouseEnter);
            this.view_WrapT_label.MouseLeave += new System.EventHandler(this.view_WrapT_MouseLeave);
            // 
            // view_mag_ck
            // 
            this.view_mag_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_mag_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_mag_ck.ErrorImage = null;
            this.view_mag_ck.InitialImage = null;
            this.view_mag_ck.Location = new System.Drawing.Point(239, 1259);
            this.view_mag_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_mag_ck.Name = "view_mag_ck";
            this.view_mag_ck.Size = new System.Drawing.Size(64, 64);
            this.view_mag_ck.TabIndex = 393;
            this.view_mag_ck.TabStop = false;
            this.view_mag_ck.Visible = false;
            this.view_mag_ck.Click += new System.EventHandler(this.view_mag_Click);
            this.view_mag_ck.MouseEnter += new System.EventHandler(this.view_mag_MouseEnter);
            this.view_mag_ck.MouseLeave += new System.EventHandler(this.view_mag_MouseLeave);
            // 
            // view_mag_label
            // 
            this.view_mag_label.AutoSize = true;
            this.view_mag_label.BackColor = System.Drawing.Color.Transparent;
            this.view_mag_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.view_mag_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_mag_label.Location = new System.Drawing.Point(307, 1259);
            this.view_mag_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_mag_label.Name = "view_mag_label";
            this.view_mag_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_mag_label.Size = new System.Drawing.Size(108, 68);
            this.view_mag_label.TabIndex = 391;
            this.view_mag_label.Text = "Mag filter";
            this.view_mag_label.Visible = false;
            this.view_mag_label.Click += new System.EventHandler(this.view_mag_Click);
            this.view_mag_label.MouseEnter += new System.EventHandler(this.view_mag_MouseEnter);
            this.view_mag_label.MouseLeave += new System.EventHandler(this.view_mag_MouseLeave);
            // 
            // view_min_ck
            // 
            this.view_min_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_min_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_min_ck.ErrorImage = null;
            this.view_min_ck.InitialImage = null;
            this.view_min_ck.Location = new System.Drawing.Point(40, 1259);
            this.view_min_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_min_ck.Name = "view_min_ck";
            this.view_min_ck.Size = new System.Drawing.Size(64, 64);
            this.view_min_ck.TabIndex = 390;
            this.view_min_ck.TabStop = false;
            this.view_min_ck.Visible = false;
            this.view_min_ck.Click += new System.EventHandler(this.view_min_Click);
            this.view_min_ck.MouseEnter += new System.EventHandler(this.view_min_MouseEnter);
            this.view_min_ck.MouseLeave += new System.EventHandler(this.view_min_MouseLeave);
            // 
            // view_min_label
            // 
            this.view_min_label.AutoSize = true;
            this.view_min_label.BackColor = System.Drawing.Color.Transparent;
            this.view_min_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.view_min_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_min_label.Location = new System.Drawing.Point(104, 1259);
            this.view_min_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_min_label.Name = "view_min_label";
            this.view_min_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_min_label.Size = new System.Drawing.Size(102, 68);
            this.view_min_label.TabIndex = 388;
            this.view_min_label.Text = "Min filter";
            this.view_min_label.Visible = false;
            this.view_min_label.Click += new System.EventHandler(this.view_min_Click);
            this.view_min_label.MouseEnter += new System.EventHandler(this.view_min_MouseEnter);
            this.view_min_label.MouseLeave += new System.EventHandler(this.view_min_MouseLeave);
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
            // all_ck
            // 
            this.all_ck.BackColor = System.Drawing.Color.Transparent;
            this.all_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.all_ck.ErrorImage = null;
            this.all_ck.InitialImage = null;
            this.all_ck.Location = new System.Drawing.Point(48, 0);
            this.all_ck.Margin = new System.Windows.Forms.Padding(0);
            this.all_ck.Name = "all_ck";
            this.all_ck.Size = new System.Drawing.Size(32, 32);
            this.all_ck.TabIndex = 399;
            this.all_ck.TabStop = false;
            this.all_ck.Click += new System.EventHandler(this.All_Click);
            this.all_ck.MouseEnter += new System.EventHandler(this.All_MouseEnter);
            this.all_ck.MouseLeave += new System.EventHandler(this.All_MouseLeave);
            // 
            // preview_ck
            // 
            this.preview_ck.BackColor = System.Drawing.Color.Transparent;
            this.preview_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.preview_ck.ErrorImage = null;
            this.preview_ck.InitialImage = null;
            this.preview_ck.Location = new System.Drawing.Point(176, 0);
            this.preview_ck.Margin = new System.Windows.Forms.Padding(0);
            this.preview_ck.Name = "preview_ck";
            this.preview_ck.Size = new System.Drawing.Size(96, 32);
            this.preview_ck.TabIndex = 400;
            this.preview_ck.TabStop = false;
            this.preview_ck.Click += new System.EventHandler(this.Preview_Click);
            this.preview_ck.MouseEnter += new System.EventHandler(this.Preview_MouseEnter);
            this.preview_ck.MouseLeave += new System.EventHandler(this.Preview_MouseLeave);
            // 
            // auto_ck
            // 
            this.auto_ck.BackColor = System.Drawing.Color.Transparent;
            this.auto_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.auto_ck.ErrorImage = null;
            this.auto_ck.InitialImage = null;
            this.auto_ck.Location = new System.Drawing.Point(80, 0);
            this.auto_ck.Margin = new System.Windows.Forms.Padding(0);
            this.auto_ck.Name = "auto_ck";
            this.auto_ck.Size = new System.Drawing.Size(96, 32);
            this.auto_ck.TabIndex = 401;
            this.auto_ck.TabStop = false;
            this.auto_ck.Click += new System.EventHandler(this.Auto_Click);
            this.auto_ck.MouseEnter += new System.EventHandler(this.Auto_MouseEnter);
            this.auto_ck.MouseLeave += new System.EventHandler(this.Auto_MouseLeave);
            // 
            // paint_ck
            // 
            this.paint_ck.BackColor = System.Drawing.Color.Transparent;
            this.paint_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.paint_ck.ErrorImage = null;
            this.paint_ck.InitialImage = null;
            this.paint_ck.Location = new System.Drawing.Point(272, 0);
            this.paint_ck.Margin = new System.Windows.Forms.Padding(0);
            this.paint_ck.Name = "paint_ck";
            this.paint_ck.Size = new System.Drawing.Size(96, 32);
            this.paint_ck.TabIndex = 402;
            this.paint_ck.TabStop = false;
            this.paint_ck.Click += new System.EventHandler(this.Paint_Click);
            this.paint_ck.MouseEnter += new System.EventHandler(this.Paint_MouseEnter);
            this.paint_ck.MouseLeave += new System.EventHandler(this.Paint_MouseLeave);
            // 
            // banner_x_ck
            // 
            this.banner_x_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_x_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_x_ck.ErrorImage = null;
            this.banner_x_ck.InitialImage = null;
            this.banner_x_ck.Location = new System.Drawing.Point(1888, 0);
            this.banner_x_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_x_ck.Name = "banner_x_ck";
            this.banner_x_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_x_ck.TabIndex = 404;
            this.banner_x_ck.TabStop = false;
            this.banner_x_ck.Click += new System.EventHandler(this.Close_Click);
            this.banner_x_ck.MouseEnter += new System.EventHandler(this.Close_MouseEnter);
            this.banner_x_ck.MouseLeave += new System.EventHandler(this.Close_MouseLeave);
            // 
            // banner_f11_ck
            // 
            this.banner_f11_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_f11_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_f11_ck.ErrorImage = null;
            this.banner_f11_ck.InitialImage = null;
            this.banner_f11_ck.Location = new System.Drawing.Point(1856, 0);
            this.banner_f11_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_f11_ck.Name = "banner_f11_ck";
            this.banner_f11_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_f11_ck.TabIndex = 406;
            this.banner_f11_ck.TabStop = false;
            this.banner_f11_ck.Click += new System.EventHandler(this.Maximized_Click);
            this.banner_f11_ck.MouseEnter += new System.EventHandler(this.Maximized_MouseEnter);
            this.banner_f11_ck.MouseLeave += new System.EventHandler(this.Maximized_MouseLeave);
            // 
            // banner_minus_ck
            // 
            this.banner_minus_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_minus_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_minus_ck.ErrorImage = null;
            this.banner_minus_ck.InitialImage = null;
            this.banner_minus_ck.Location = new System.Drawing.Point(1824, 0);
            this.banner_minus_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_minus_ck.Name = "banner_minus_ck";
            this.banner_minus_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_minus_ck.TabIndex = 408;
            this.banner_minus_ck.TabStop = false;
            this.banner_minus_ck.Click += new System.EventHandler(this.Minimized_Click);
            this.banner_minus_ck.MouseEnter += new System.EventHandler(this.Minimized_MouseEnter);
            this.banner_minus_ck.MouseLeave += new System.EventHandler(this.Minimized_MouseLeave);
            // 
            // banner_9_ck
            // 
            this.banner_9_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_9_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_9_ck.ErrorImage = null;
            this.banner_9_ck.InitialImage = null;
            this.banner_9_ck.Location = new System.Drawing.Point(544, 0);
            this.banner_9_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_9_ck.Name = "banner_9_ck";
            this.banner_9_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_9_ck.TabIndex = 412;
            this.banner_9_ck.TabStop = false;
            this.banner_9_ck.Click += new System.EventHandler(this.Top_right_Click);
            this.banner_9_ck.MouseEnter += new System.EventHandler(this.Top_right_MouseEnter);
            this.banner_9_ck.MouseLeave += new System.EventHandler(this.Top_right_MouseLeave);
            // 
            // banner_8_ck
            // 
            this.banner_8_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_8_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_8_ck.ErrorImage = null;
            this.banner_8_ck.InitialImage = null;
            this.banner_8_ck.Location = new System.Drawing.Point(512, 0);
            this.banner_8_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_8_ck.Name = "banner_8_ck";
            this.banner_8_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_8_ck.TabIndex = 414;
            this.banner_8_ck.TabStop = false;
            this.banner_8_ck.Click += new System.EventHandler(this.Top_Click);
            this.banner_8_ck.MouseEnter += new System.EventHandler(this.Top_MouseEnter);
            this.banner_8_ck.MouseLeave += new System.EventHandler(this.Top_MouseLeave);
            // 
            // banner_7_ck
            // 
            this.banner_7_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_7_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_7_ck.ErrorImage = null;
            this.banner_7_ck.InitialImage = null;
            this.banner_7_ck.Location = new System.Drawing.Point(480, 0);
            this.banner_7_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_7_ck.Name = "banner_7_ck";
            this.banner_7_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_7_ck.TabIndex = 416;
            this.banner_7_ck.TabStop = false;
            this.banner_7_ck.Click += new System.EventHandler(this.Top_left_Click);
            this.banner_7_ck.MouseEnter += new System.EventHandler(this.Top_left_MouseEnter);
            this.banner_7_ck.MouseLeave += new System.EventHandler(this.Top_left_MouseLeave);
            // 
            // banner_6_ck
            // 
            this.banner_6_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_6_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_6_ck.ErrorImage = null;
            this.banner_6_ck.InitialImage = null;
            this.banner_6_ck.Location = new System.Drawing.Point(576, 0);
            this.banner_6_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_6_ck.Name = "banner_6_ck";
            this.banner_6_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_6_ck.TabIndex = 418;
            this.banner_6_ck.TabStop = false;
            this.banner_6_ck.Click += new System.EventHandler(this.Right_Click);
            this.banner_6_ck.MouseEnter += new System.EventHandler(this.Right_MouseEnter);
            this.banner_6_ck.MouseLeave += new System.EventHandler(this.Right_MouseLeave);
            // 
            // banner_4_ck
            // 
            this.banner_4_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_4_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_4_ck.ErrorImage = null;
            this.banner_4_ck.InitialImage = null;
            this.banner_4_ck.Location = new System.Drawing.Point(448, 0);
            this.banner_4_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_4_ck.Name = "banner_4_ck";
            this.banner_4_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_4_ck.TabIndex = 420;
            this.banner_4_ck.TabStop = false;
            this.banner_4_ck.Click += new System.EventHandler(this.Left_Click);
            this.banner_4_ck.MouseEnter += new System.EventHandler(this.Left_MouseEnter);
            this.banner_4_ck.MouseLeave += new System.EventHandler(this.Left_MouseLeave);
            // 
            // banner_3_ck
            // 
            this.banner_3_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_3_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_3_ck.ErrorImage = null;
            this.banner_3_ck.InitialImage = null;
            this.banner_3_ck.Location = new System.Drawing.Point(608, 0);
            this.banner_3_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_3_ck.Name = "banner_3_ck";
            this.banner_3_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_3_ck.TabIndex = 422;
            this.banner_3_ck.TabStop = false;
            this.banner_3_ck.Click += new System.EventHandler(this.Bottom_right_Click);
            this.banner_3_ck.MouseEnter += new System.EventHandler(this.Bottom_right_MouseEnter);
            this.banner_3_ck.MouseLeave += new System.EventHandler(this.Bottom_right_MouseLeave);
            // 
            // banner_2_ck
            // 
            this.banner_2_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_2_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_2_ck.ErrorImage = null;
            this.banner_2_ck.InitialImage = null;
            this.banner_2_ck.Location = new System.Drawing.Point(640, 0);
            this.banner_2_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_2_ck.Name = "banner_2_ck";
            this.banner_2_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_2_ck.TabIndex = 424;
            this.banner_2_ck.TabStop = false;
            this.banner_2_ck.Click += new System.EventHandler(this.Bottom_Click);
            this.banner_2_ck.MouseEnter += new System.EventHandler(this.Bottom_MouseEnter);
            this.banner_2_ck.MouseLeave += new System.EventHandler(this.Bottom_MouseLeave);
            // 
            // banner_1_ck
            // 
            this.banner_1_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_1_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_1_ck.ErrorImage = null;
            this.banner_1_ck.InitialImage = null;
            this.banner_1_ck.Location = new System.Drawing.Point(672, 0);
            this.banner_1_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_1_ck.Name = "banner_1_ck";
            this.banner_1_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_1_ck.TabIndex = 426;
            this.banner_1_ck.TabStop = false;
            this.banner_1_ck.Click += new System.EventHandler(this.Bottom_left_Click);
            this.banner_1_ck.MouseEnter += new System.EventHandler(this.Bottom_left_MouseEnter);
            this.banner_1_ck.MouseLeave += new System.EventHandler(this.Bottom_left_MouseLeave);
            // 
            // cli_textbox_ck
            // 
            this.cli_textbox_ck.BackColor = System.Drawing.Color.Transparent;
            this.cli_textbox_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
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
            this.cli_textbox_ck.MouseEnter += new System.EventHandler(this.cli_textbox_MouseEnter);
            this.cli_textbox_ck.MouseLeave += new System.EventHandler(this.cli_textbox_MouseLeave);
            // 
            // run_ck
            // 
            this.run_ck.BackColor = System.Drawing.Color.Transparent;
            this.run_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.run_ck.ErrorImage = null;
            this.run_ck.InitialImage = null;
            this.run_ck.Location = new System.Drawing.Point(1500, 1005);
            this.run_ck.Margin = new System.Windows.Forms.Padding(0);
            this.run_ck.Name = "run_ck";
            this.run_ck.Size = new System.Drawing.Size(128, 64);
            this.run_ck.TabIndex = 428;
            this.run_ck.TabStop = false;
            this.run_ck.Click += new System.EventHandler(this.Run_Click);
            this.run_ck.MouseEnter += new System.EventHandler(this.run_MouseEnter);
            this.run_ck.MouseLeave += new System.EventHandler(this.run_MouseLeave);
            // 
            // input_file_txt
            // 
            this.input_file_txt.BackColor = System.Drawing.Color.Black;
            this.input_file_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.input_file_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.input_file_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.input_file_txt.Location = new System.Drawing.Point(16, 972);
            this.input_file_txt.Margin = new System.Windows.Forms.Padding(0);
            this.input_file_txt.Name = "input_file_txt";
            this.input_file_txt.Size = new System.Drawing.Size(116, 24);
            this.input_file_txt.TabIndex = 0;
            this.input_file_txt.TextChanged += new System.EventHandler(this.input_file_TextChanged);
            this.input_file_txt.DoubleClick += new System.EventHandler(this.input_file_Click);
            this.input_file_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.input_file_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.input_file_txt.MouseEnter += new System.EventHandler(this.input_file_MouseEnter);
            this.input_file_txt.MouseLeave += new System.EventHandler(this.input_file_MouseLeave);
            // 
            // input_file_label
            // 
            this.input_file_label.AutoSize = true;
            this.input_file_label.BackColor = System.Drawing.Color.Transparent;
            this.input_file_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.input_file_label.ForeColor = System.Drawing.SystemColors.Control;
            this.input_file_label.Location = new System.Drawing.Point(21, 929);
            this.input_file_label.Margin = new System.Windows.Forms.Padding(0);
            this.input_file_label.Name = "input_file_label";
            this.input_file_label.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.input_file_label.Size = new System.Drawing.Size(100, 44);
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
            this.input_file2_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.input_file2_label.ForeColor = System.Drawing.SystemColors.Control;
            this.input_file2_label.Location = new System.Drawing.Point(150, 941);
            this.input_file2_label.Margin = new System.Windows.Forms.Padding(0);
            this.input_file2_label.Name = "input_file2_label";
            this.input_file2_label.Size = new System.Drawing.Size(117, 24);
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
            this.input_file2_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.input_file2_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.input_file2_txt.Location = new System.Drawing.Point(154, 972);
            this.input_file2_txt.Margin = new System.Windows.Forms.Padding(0);
            this.input_file2_txt.Name = "input_file2_txt";
            this.input_file2_txt.Size = new System.Drawing.Size(128, 24);
            this.input_file2_txt.TabIndex = 1;
            this.input_file2_txt.TextChanged += new System.EventHandler(this.input_file2_TextChanged);
            this.input_file2_txt.DoubleClick += new System.EventHandler(this.input_file2_Click);
            this.input_file2_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.input_file2_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.input_file2_txt.MouseEnter += new System.EventHandler(this.input_file2_MouseEnter);
            this.input_file2_txt.MouseLeave += new System.EventHandler(this.input_file2_MouseLeave);
            // 
            // mipmaps_label
            // 
            this.mipmaps_label.AutoSize = true;
            this.mipmaps_label.BackColor = System.Drawing.Color.Transparent;
            this.mipmaps_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.mipmaps_label.ForeColor = System.Drawing.SystemColors.Control;
            this.mipmaps_label.Location = new System.Drawing.Point(499, 941);
            this.mipmaps_label.Margin = new System.Windows.Forms.Padding(0);
            this.mipmaps_label.Name = "mipmaps_label";
            this.mipmaps_label.Size = new System.Drawing.Size(100, 24);
            this.mipmaps_label.TabIndex = 436;
            this.mipmaps_label.Text = "mipmaps";
            this.mipmaps_label.MouseEnter += new System.EventHandler(this.mipmaps_MouseEnter);
            this.mipmaps_label.MouseLeave += new System.EventHandler(this.mipmaps_MouseLeave);
            // 
            // mipmaps_txt
            // 
            this.mipmaps_txt.BackColor = System.Drawing.Color.Black;
            this.mipmaps_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mipmaps_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.mipmaps_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.mipmaps_txt.Location = new System.Drawing.Point(503, 972);
            this.mipmaps_txt.Margin = new System.Windows.Forms.Padding(0);
            this.mipmaps_txt.Name = "mipmaps_txt";
            this.mipmaps_txt.Size = new System.Drawing.Size(100, 24);
            this.mipmaps_txt.TabIndex = 3;
            this.mipmaps_txt.Text = "0";
            this.mipmaps_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mipmaps_txt.TextChanged += new System.EventHandler(this.mipmaps_TextChanged);
            this.mipmaps_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.mipmaps_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.mipmaps_txt.MouseEnter += new System.EventHandler(this.mipmaps_MouseEnter);
            this.mipmaps_txt.MouseLeave += new System.EventHandler(this.mipmaps_MouseLeave);
            // 
            // diversity_label
            // 
            this.diversity_label.AutoSize = true;
            this.diversity_label.BackColor = System.Drawing.Color.Transparent;
            this.diversity_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.diversity_label.ForeColor = System.Drawing.SystemColors.Control;
            this.diversity_label.Location = new System.Drawing.Point(498, 877);
            this.diversity_label.Margin = new System.Windows.Forms.Padding(0);
            this.diversity_label.Name = "diversity_label";
            this.diversity_label.Size = new System.Drawing.Size(98, 24);
            this.diversity_label.TabIndex = 438;
            this.diversity_label.Text = "diversity";
            this.diversity_label.MouseEnter += new System.EventHandler(this.diversity_MouseEnter);
            this.diversity_label.MouseLeave += new System.EventHandler(this.diversity_MouseLeave);
            // 
            // diversity_txt
            // 
            this.diversity_txt.BackColor = System.Drawing.Color.Black;
            this.diversity_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.diversity_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.diversity_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.diversity_txt.Location = new System.Drawing.Point(502, 908);
            this.diversity_txt.Margin = new System.Windows.Forms.Padding(0);
            this.diversity_txt.Name = "diversity_txt";
            this.diversity_txt.Size = new System.Drawing.Size(100, 24);
            this.diversity_txt.TabIndex = 11;
            this.diversity_txt.Text = "10";
            this.diversity_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.diversity_txt.TextChanged += new System.EventHandler(this.diversity_TextChanged);
            this.diversity_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.diversity_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.diversity_txt.MouseEnter += new System.EventHandler(this.diversity_MouseEnter);
            this.diversity_txt.MouseLeave += new System.EventHandler(this.diversity_MouseLeave);
            // 
            // diversity2_label
            // 
            this.diversity2_label.AutoSize = true;
            this.diversity2_label.BackColor = System.Drawing.Color.Transparent;
            this.diversity2_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.diversity2_label.ForeColor = System.Drawing.SystemColors.Control;
            this.diversity2_label.Location = new System.Drawing.Point(620, 877);
            this.diversity2_label.Margin = new System.Windows.Forms.Padding(0);
            this.diversity2_label.Name = "diversity2_label";
            this.diversity2_label.Size = new System.Drawing.Size(110, 24);
            this.diversity2_label.TabIndex = 440;
            this.diversity2_label.Text = "diversity2";
            this.diversity2_label.MouseEnter += new System.EventHandler(this.diversity2_MouseEnter);
            this.diversity2_label.MouseLeave += new System.EventHandler(this.diversity2_MouseLeave);
            // 
            // diversity2_txt
            // 
            this.diversity2_txt.BackColor = System.Drawing.Color.Black;
            this.diversity2_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.diversity2_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.diversity2_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.diversity2_txt.Location = new System.Drawing.Point(624, 908);
            this.diversity2_txt.Margin = new System.Windows.Forms.Padding(0);
            this.diversity2_txt.Name = "diversity2_txt";
            this.diversity2_txt.Size = new System.Drawing.Size(114, 24);
            this.diversity2_txt.TabIndex = 12;
            this.diversity2_txt.Text = "0";
            this.diversity2_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.diversity2_txt.TextChanged += new System.EventHandler(this.diversity2_TextChanged);
            this.diversity2_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.diversity2_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.diversity2_txt.MouseEnter += new System.EventHandler(this.diversity2_MouseEnter);
            this.diversity2_txt.MouseLeave += new System.EventHandler(this.diversity2_MouseLeave);
            // 
            // percentage_label
            // 
            this.percentage_label.AutoSize = true;
            this.percentage_label.BackColor = System.Drawing.Color.Transparent;
            this.percentage_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.percentage_label.ForeColor = System.Drawing.SystemColors.Control;
            this.percentage_label.Location = new System.Drawing.Point(753, 877);
            this.percentage_label.Margin = new System.Windows.Forms.Padding(0);
            this.percentage_label.Name = "percentage_label";
            this.percentage_label.Size = new System.Drawing.Size(128, 24);
            this.percentage_label.TabIndex = 442;
            this.percentage_label.Text = "percentage";
            this.percentage_label.MouseEnter += new System.EventHandler(this.percentage_MouseEnter);
            this.percentage_label.MouseLeave += new System.EventHandler(this.percentage_MouseLeave);
            // 
            // percentage_txt
            // 
            this.percentage_txt.BackColor = System.Drawing.Color.Black;
            this.percentage_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.percentage_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.percentage_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.percentage_txt.Location = new System.Drawing.Point(757, 908);
            this.percentage_txt.Margin = new System.Windows.Forms.Padding(0);
            this.percentage_txt.Name = "percentage_txt";
            this.percentage_txt.Size = new System.Drawing.Size(128, 24);
            this.percentage_txt.TabIndex = 13;
            this.percentage_txt.Text = "0%";
            this.percentage_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.percentage_txt.TextChanged += new System.EventHandler(this.percentage_TextChanged);
            this.percentage_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.percentage_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.percentage_txt.MouseEnter += new System.EventHandler(this.percentage_MouseEnter);
            this.percentage_txt.MouseLeave += new System.EventHandler(this.percentage_MouseLeave);
            // 
            // percentage2_label
            // 
            this.percentage2_label.AutoSize = true;
            this.percentage2_label.BackColor = System.Drawing.Color.Transparent;
            this.percentage2_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.percentage2_label.ForeColor = System.Drawing.SystemColors.Control;
            this.percentage2_label.Location = new System.Drawing.Point(900, 877);
            this.percentage2_label.Margin = new System.Windows.Forms.Padding(0);
            this.percentage2_label.Name = "percentage2_label";
            this.percentage2_label.Size = new System.Drawing.Size(140, 24);
            this.percentage2_label.TabIndex = 444;
            this.percentage2_label.Text = "percentage2";
            this.percentage2_label.MouseEnter += new System.EventHandler(this.percentage2_MouseEnter);
            this.percentage2_label.MouseLeave += new System.EventHandler(this.percentage2_MouseLeave);
            // 
            // percentage2_txt
            // 
            this.percentage2_txt.BackColor = System.Drawing.Color.Black;
            this.percentage2_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.percentage2_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.percentage2_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.percentage2_txt.Location = new System.Drawing.Point(904, 908);
            this.percentage2_txt.Margin = new System.Windows.Forms.Padding(0);
            this.percentage2_txt.Name = "percentage2_txt";
            this.percentage2_txt.Size = new System.Drawing.Size(143, 24);
            this.percentage2_txt.TabIndex = 14;
            this.percentage2_txt.Text = "0%";
            this.percentage2_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.percentage2_txt.TextChanged += new System.EventHandler(this.percentage2_TextChanged);
            this.percentage2_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.percentage2_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.percentage2_txt.MouseEnter += new System.EventHandler(this.percentage2_MouseEnter);
            this.percentage2_txt.MouseLeave += new System.EventHandler(this.percentage2_MouseLeave);
            // 
            // cmpr_max_label
            // 
            this.cmpr_max_label.AutoSize = true;
            this.cmpr_max_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_max_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_max_label.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_max_label.Location = new System.Drawing.Point(617, 941);
            this.cmpr_max_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_max_label.Name = "cmpr_max_label";
            this.cmpr_max_label.Size = new System.Drawing.Size(114, 24);
            this.cmpr_max_label.TabIndex = 446;
            this.cmpr_max_label.Text = "CMPR Max";
            this.cmpr_max_label.MouseEnter += new System.EventHandler(this.cmpr_max_MouseEnter);
            this.cmpr_max_label.MouseLeave += new System.EventHandler(this.cmpr_max_MouseLeave);
            // 
            // cmpr_max_txt
            // 
            this.cmpr_max_txt.BackColor = System.Drawing.Color.Black;
            this.cmpr_max_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cmpr_max_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_max_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_max_txt.Location = new System.Drawing.Point(623, 972);
            this.cmpr_max_txt.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_max_txt.Name = "cmpr_max_txt";
            this.cmpr_max_txt.Size = new System.Drawing.Size(125, 24);
            this.cmpr_max_txt.TabIndex = 4;
            this.cmpr_max_txt.Text = "0";
            this.cmpr_max_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmpr_max_txt.TextChanged += new System.EventHandler(this.cmpr_max_TextChanged);
            this.cmpr_max_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.cmpr_max_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.cmpr_max_txt.MouseEnter += new System.EventHandler(this.cmpr_max_MouseEnter);
            this.cmpr_max_txt.MouseLeave += new System.EventHandler(this.cmpr_max_MouseLeave);
            // 
            // output_name_label
            // 
            this.output_name_label.AutoSize = true;
            this.output_name_label.BackColor = System.Drawing.Color.Transparent;
            this.output_name_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.output_name_label.ForeColor = System.Drawing.SystemColors.Control;
            this.output_name_label.Location = new System.Drawing.Point(308, 941);
            this.output_name_label.Margin = new System.Windows.Forms.Padding(0);
            this.output_name_label.Name = "output_name_label";
            this.output_name_label.Size = new System.Drawing.Size(142, 24);
            this.output_name_label.TabIndex = 448;
            this.output_name_label.Text = "Output name";
            this.output_name_label.MouseEnter += new System.EventHandler(this.output_name_MouseEnter);
            this.output_name_label.MouseLeave += new System.EventHandler(this.output_name_MouseLeave);
            // 
            // output_name_txt
            // 
            this.output_name_txt.BackColor = System.Drawing.Color.Black;
            this.output_name_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.output_name_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.output_name_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.output_name_txt.Location = new System.Drawing.Point(303, 972);
            this.output_name_txt.Margin = new System.Windows.Forms.Padding(0);
            this.output_name_txt.Name = "output_name_txt";
            this.output_name_txt.Size = new System.Drawing.Size(177, 24);
            this.output_name_txt.TabIndex = 2;
            this.output_name_txt.TextChanged += new System.EventHandler(this.output_name_TextChanged);
            this.output_name_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.output_name_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.output_name_txt.MouseEnter += new System.EventHandler(this.output_name_MouseEnter);
            this.output_name_txt.MouseLeave += new System.EventHandler(this.output_name_MouseLeave);
            // 
            // cmpr_min_alpha_label
            // 
            this.cmpr_min_alpha_label.AutoSize = true;
            this.cmpr_min_alpha_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_min_alpha_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_min_alpha_label.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_min_alpha_label.Location = new System.Drawing.Point(762, 941);
            this.cmpr_min_alpha_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_min_alpha_label.Name = "cmpr_min_alpha_label";
            this.cmpr_min_alpha_label.Size = new System.Drawing.Size(171, 24);
            this.cmpr_min_alpha_label.TabIndex = 450;
            this.cmpr_min_alpha_label.Text = "CMPR Min alpha";
            this.cmpr_min_alpha_label.MouseEnter += new System.EventHandler(this.cmpr_min_alpha_MouseEnter);
            this.cmpr_min_alpha_label.MouseLeave += new System.EventHandler(this.cmpr_min_alpha_MouseLeave);
            // 
            // cmpr_min_alpha_txt
            // 
            this.cmpr_min_alpha_txt.BackColor = System.Drawing.Color.Black;
            this.cmpr_min_alpha_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cmpr_min_alpha_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_min_alpha_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_min_alpha_txt.Location = new System.Drawing.Point(766, 972);
            this.cmpr_min_alpha_txt.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_min_alpha_txt.Name = "cmpr_min_alpha_txt";
            this.cmpr_min_alpha_txt.Size = new System.Drawing.Size(192, 24);
            this.cmpr_min_alpha_txt.TabIndex = 5;
            this.cmpr_min_alpha_txt.Text = "100";
            this.cmpr_min_alpha_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmpr_min_alpha_txt.TextChanged += new System.EventHandler(this.cmpr_min_alpha_TextChanged);
            this.cmpr_min_alpha_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.cmpr_min_alpha_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.cmpr_min_alpha_txt.MouseEnter += new System.EventHandler(this.cmpr_min_alpha_MouseEnter);
            this.cmpr_min_alpha_txt.MouseLeave += new System.EventHandler(this.cmpr_min_alpha_MouseLeave);
            // 
            // num_colours_label
            // 
            this.num_colours_label.AutoSize = true;
            this.num_colours_label.BackColor = System.Drawing.Color.Transparent;
            this.num_colours_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.num_colours_label.ForeColor = System.Drawing.SystemColors.Control;
            this.num_colours_label.Location = new System.Drawing.Point(975, 941);
            this.num_colours_label.Margin = new System.Windows.Forms.Padding(0);
            this.num_colours_label.Name = "num_colours_label";
            this.num_colours_label.Size = new System.Drawing.Size(134, 24);
            this.num_colours_label.TabIndex = 452;
            this.num_colours_label.Text = "num colours";
            this.num_colours_label.MouseEnter += new System.EventHandler(this.num_colours_MouseEnter);
            this.num_colours_label.MouseLeave += new System.EventHandler(this.num_colours_MouseLeave);
            // 
            // num_colours_txt
            // 
            this.num_colours_txt.BackColor = System.Drawing.Color.Black;
            this.num_colours_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.num_colours_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.num_colours_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.num_colours_txt.Location = new System.Drawing.Point(979, 972);
            this.num_colours_txt.Margin = new System.Windows.Forms.Padding(0);
            this.num_colours_txt.Name = "num_colours_txt";
            this.num_colours_txt.Size = new System.Drawing.Size(141, 24);
            this.num_colours_txt.TabIndex = 6;
            this.num_colours_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_colours_txt.TextChanged += new System.EventHandler(this.num_colours_TextChanged);
            this.num_colours_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.num_colours_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.num_colours_txt.MouseEnter += new System.EventHandler(this.num_colours_MouseEnter);
            this.num_colours_txt.MouseLeave += new System.EventHandler(this.num_colours_MouseLeave);
            // 
            // round3_label
            // 
            this.round3_label.AutoSize = true;
            this.round3_label.BackColor = System.Drawing.Color.Transparent;
            this.round3_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.round3_label.ForeColor = System.Drawing.SystemColors.Control;
            this.round3_label.Location = new System.Drawing.Point(1157, 941);
            this.round3_label.Margin = new System.Windows.Forms.Padding(0);
            this.round3_label.Name = "round3_label";
            this.round3_label.Size = new System.Drawing.Size(83, 24);
            this.round3_label.TabIndex = 454;
            this.round3_label.Text = "round3";
            this.round3_label.MouseEnter += new System.EventHandler(this.round3_MouseEnter);
            this.round3_label.MouseLeave += new System.EventHandler(this.round3_MouseLeave);
            // 
            // round3_txt
            // 
            this.round3_txt.BackColor = System.Drawing.Color.Black;
            this.round3_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.round3_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.round3_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.round3_txt.Location = new System.Drawing.Point(1153, 972);
            this.round3_txt.Margin = new System.Windows.Forms.Padding(0);
            this.round3_txt.Name = "round3_txt";
            this.round3_txt.Size = new System.Drawing.Size(100, 24);
            this.round3_txt.TabIndex = 7;
            this.round3_txt.Text = "15";
            this.round3_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.round3_txt.TextChanged += new System.EventHandler(this.round3_TextChanged);
            this.round3_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.round3_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.round3_txt.MouseEnter += new System.EventHandler(this.round3_MouseEnter);
            this.round3_txt.MouseLeave += new System.EventHandler(this.round3_MouseLeave);
            // 
            // round4_label
            // 
            this.round4_label.AutoSize = true;
            this.round4_label.BackColor = System.Drawing.Color.Transparent;
            this.round4_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.round4_label.ForeColor = System.Drawing.SystemColors.Control;
            this.round4_label.Location = new System.Drawing.Point(1277, 941);
            this.round4_label.Margin = new System.Windows.Forms.Padding(0);
            this.round4_label.Name = "round4_label";
            this.round4_label.Size = new System.Drawing.Size(83, 24);
            this.round4_label.TabIndex = 456;
            this.round4_label.Text = "round4";
            this.round4_label.MouseEnter += new System.EventHandler(this.round4_MouseEnter);
            this.round4_label.MouseLeave += new System.EventHandler(this.round4_MouseLeave);
            // 
            // round4_txt
            // 
            this.round4_txt.BackColor = System.Drawing.Color.Black;
            this.round4_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.round4_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.round4_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.round4_txt.Location = new System.Drawing.Point(1273, 972);
            this.round4_txt.Margin = new System.Windows.Forms.Padding(0);
            this.round4_txt.Name = "round4_txt";
            this.round4_txt.Size = new System.Drawing.Size(100, 24);
            this.round4_txt.TabIndex = 8;
            this.round4_txt.Text = "7";
            this.round4_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.round4_txt.TextChanged += new System.EventHandler(this.round4_TextChanged);
            this.round4_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.round4_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.round4_txt.MouseEnter += new System.EventHandler(this.round4_MouseEnter);
            this.round4_txt.MouseLeave += new System.EventHandler(this.round4_MouseLeave);
            // 
            // round5_label
            // 
            this.round5_label.AutoSize = true;
            this.round5_label.BackColor = System.Drawing.Color.Transparent;
            this.round5_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.round5_label.ForeColor = System.Drawing.SystemColors.Control;
            this.round5_label.Location = new System.Drawing.Point(1395, 941);
            this.round5_label.Margin = new System.Windows.Forms.Padding(0);
            this.round5_label.Name = "round5_label";
            this.round5_label.Size = new System.Drawing.Size(83, 24);
            this.round5_label.TabIndex = 458;
            this.round5_label.Text = "round5";
            this.round5_label.MouseEnter += new System.EventHandler(this.round5_MouseEnter);
            this.round5_label.MouseLeave += new System.EventHandler(this.round5_MouseLeave);
            // 
            // round5_txt
            // 
            this.round5_txt.BackColor = System.Drawing.Color.Black;
            this.round5_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.round5_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.round5_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.round5_txt.Location = new System.Drawing.Point(1391, 972);
            this.round5_txt.Margin = new System.Windows.Forms.Padding(0);
            this.round5_txt.Name = "round5_txt";
            this.round5_txt.Size = new System.Drawing.Size(100, 24);
            this.round5_txt.TabIndex = 9;
            this.round5_txt.Text = "3";
            this.round5_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.round5_txt.TextChanged += new System.EventHandler(this.round5_TextChanged);
            this.round5_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.round5_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.round5_txt.MouseEnter += new System.EventHandler(this.round5_MouseEnter);
            this.round5_txt.MouseLeave += new System.EventHandler(this.round5_MouseLeave);
            // 
            // round6_label
            // 
            this.round6_label.AutoSize = true;
            this.round6_label.BackColor = System.Drawing.Color.Transparent;
            this.round6_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.round6_label.ForeColor = System.Drawing.SystemColors.Control;
            this.round6_label.Location = new System.Drawing.Point(1504, 941);
            this.round6_label.Margin = new System.Windows.Forms.Padding(0);
            this.round6_label.Name = "round6_label";
            this.round6_label.Size = new System.Drawing.Size(83, 24);
            this.round6_label.TabIndex = 460;
            this.round6_label.Text = "round6";
            this.round6_label.MouseEnter += new System.EventHandler(this.round6_MouseEnter);
            this.round6_label.MouseLeave += new System.EventHandler(this.round6_MouseLeave);
            // 
            // round6_txt
            // 
            this.round6_txt.BackColor = System.Drawing.Color.Black;
            this.round6_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.round6_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.round6_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.round6_txt.Location = new System.Drawing.Point(1504, 972);
            this.round6_txt.Margin = new System.Windows.Forms.Padding(0);
            this.round6_txt.Name = "round6_txt";
            this.round6_txt.Size = new System.Drawing.Size(100, 24);
            this.round6_txt.TabIndex = 10;
            this.round6_txt.Text = "1";
            this.round6_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.round6_txt.TextChanged += new System.EventHandler(this.round6_TextChanged);
            this.round6_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.round6_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.round6_txt.MouseEnter += new System.EventHandler(this.round6_MouseEnter);
            this.round6_txt.MouseLeave += new System.EventHandler(this.round6_MouseLeave);
            // 
            // custom_a_label
            // 
            this.custom_a_label.AutoSize = true;
            this.custom_a_label.BackColor = System.Drawing.Color.Transparent;
            this.custom_a_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.custom_a_label.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_a_label.Location = new System.Drawing.Point(1557, 877);
            this.custom_a_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_a_label.Name = "custom_a_label";
            this.custom_a_label.Size = new System.Drawing.Size(24, 24);
            this.custom_a_label.TabIndex = 468;
            this.custom_a_label.Text = "A";
            this.custom_a_label.MouseEnter += new System.EventHandler(this.custom_a_MouseEnter);
            this.custom_a_label.MouseLeave += new System.EventHandler(this.custom_a_MouseLeave);
            // 
            // custom_a_txt
            // 
            this.custom_a_txt.BackColor = System.Drawing.Color.Black;
            this.custom_a_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.custom_a_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.custom_a_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.custom_a_txt.Location = new System.Drawing.Point(1538, 908);
            this.custom_a_txt.Margin = new System.Windows.Forms.Padding(0);
            this.custom_a_txt.Name = "custom_a_txt";
            this.custom_a_txt.Size = new System.Drawing.Size(64, 24);
            this.custom_a_txt.TabIndex = 18;
            this.custom_a_txt.Text = "1.0";
            this.custom_a_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.custom_a_txt.TextChanged += new System.EventHandler(this.custom_a_TextChanged);
            this.custom_a_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.custom_a_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.custom_a_txt.MouseEnter += new System.EventHandler(this.custom_a_MouseEnter);
            this.custom_a_txt.MouseLeave += new System.EventHandler(this.custom_a_MouseLeave);
            // 
            // custom_b_label
            // 
            this.custom_b_label.AutoSize = true;
            this.custom_b_label.BackColor = System.Drawing.Color.Transparent;
            this.custom_b_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.custom_b_label.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_b_label.Location = new System.Drawing.Point(1479, 877);
            this.custom_b_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_b_label.Name = "custom_b_label";
            this.custom_b_label.Size = new System.Drawing.Size(23, 24);
            this.custom_b_label.TabIndex = 466;
            this.custom_b_label.Text = "B";
            this.custom_b_label.MouseEnter += new System.EventHandler(this.custom_b_MouseEnter);
            this.custom_b_label.MouseLeave += new System.EventHandler(this.custom_b_MouseLeave);
            // 
            // custom_b_txt
            // 
            this.custom_b_txt.BackColor = System.Drawing.Color.Black;
            this.custom_b_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.custom_b_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.custom_b_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.custom_b_txt.Location = new System.Drawing.Point(1457, 908);
            this.custom_b_txt.Margin = new System.Windows.Forms.Padding(0);
            this.custom_b_txt.Name = "custom_b_txt";
            this.custom_b_txt.Size = new System.Drawing.Size(64, 24);
            this.custom_b_txt.TabIndex = 17;
            this.custom_b_txt.Text = "1.0";
            this.custom_b_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.custom_b_txt.TextChanged += new System.EventHandler(this.custom_b_TextChanged);
            this.custom_b_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.custom_b_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.custom_b_txt.MouseEnter += new System.EventHandler(this.custom_b_MouseEnter);
            this.custom_b_txt.MouseLeave += new System.EventHandler(this.custom_b_MouseLeave);
            // 
            // custom_g_label
            // 
            this.custom_g_label.AutoSize = true;
            this.custom_g_label.BackColor = System.Drawing.Color.Transparent;
            this.custom_g_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.custom_g_label.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_g_label.Location = new System.Drawing.Point(1394, 877);
            this.custom_g_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_g_label.Name = "custom_g_label";
            this.custom_g_label.Size = new System.Drawing.Size(25, 24);
            this.custom_g_label.TabIndex = 464;
            this.custom_g_label.Text = "G";
            this.custom_g_label.MouseEnter += new System.EventHandler(this.custom_g_MouseEnter);
            this.custom_g_label.MouseLeave += new System.EventHandler(this.custom_g_MouseLeave);
            // 
            // custom_g_txt
            // 
            this.custom_g_txt.BackColor = System.Drawing.Color.Black;
            this.custom_g_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.custom_g_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.custom_g_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.custom_g_txt.Location = new System.Drawing.Point(1373, 908);
            this.custom_g_txt.Margin = new System.Windows.Forms.Padding(0);
            this.custom_g_txt.Name = "custom_g_txt";
            this.custom_g_txt.Size = new System.Drawing.Size(64, 24);
            this.custom_g_txt.TabIndex = 16;
            this.custom_g_txt.Text = "1.0";
            this.custom_g_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.custom_g_txt.TextChanged += new System.EventHandler(this.custom_g_TextChanged);
            this.custom_g_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.custom_g_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.custom_g_txt.MouseEnter += new System.EventHandler(this.custom_g_MouseEnter);
            this.custom_g_txt.MouseLeave += new System.EventHandler(this.custom_g_MouseLeave);
            // 
            // custom_r_label
            // 
            this.custom_r_label.AutoSize = true;
            this.custom_r_label.BackColor = System.Drawing.Color.Transparent;
            this.custom_r_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.custom_r_label.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_r_label.Location = new System.Drawing.Point(1312, 877);
            this.custom_r_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_r_label.Name = "custom_r_label";
            this.custom_r_label.Size = new System.Drawing.Size(23, 24);
            this.custom_r_label.TabIndex = 462;
            this.custom_r_label.Text = "R";
            this.custom_r_label.MouseEnter += new System.EventHandler(this.custom_r_MouseEnter);
            this.custom_r_label.MouseLeave += new System.EventHandler(this.custom_r_MouseLeave);
            // 
            // custom_r_txt
            // 
            this.custom_r_txt.BackColor = System.Drawing.Color.Black;
            this.custom_r_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.custom_r_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.custom_r_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.custom_r_txt.Location = new System.Drawing.Point(1292, 908);
            this.custom_r_txt.Margin = new System.Windows.Forms.Padding(0);
            this.custom_r_txt.Name = "custom_r_txt";
            this.custom_r_txt.Size = new System.Drawing.Size(64, 24);
            this.custom_r_txt.TabIndex = 15;
            this.custom_r_txt.Text = "1.0";
            this.custom_r_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.custom_r_txt.TextChanged += new System.EventHandler(this.custom_r_TextChanged);
            this.custom_r_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.custom_r_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.custom_r_txt.MouseEnter += new System.EventHandler(this.custom_r_MouseEnter);
            this.custom_r_txt.MouseLeave += new System.EventHandler(this.custom_r_MouseLeave);
            // 
            // custom_rgba_label
            // 
            this.custom_rgba_label.AutoSize = true;
            this.custom_rgba_label.BackColor = System.Drawing.Color.Transparent;
            this.custom_rgba_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.custom_rgba_label.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_rgba_label.Location = new System.Drawing.Point(1095, 897);
            this.custom_rgba_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_rgba_label.Name = "custom_rgba_label";
            this.custom_rgba_label.Size = new System.Drawing.Size(146, 24);
            this.custom_rgba_label.TabIndex = 469;
            this.custom_rgba_label.Text = "Custom RGBA";
            // 
            // description_title
            // 
            this.description_title.AutoSize = true;
            this.description_title.BackColor = System.Drawing.Color.Transparent;
            this.description_title.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.description_title.ForeColor = System.Drawing.Color.Cyan;
            this.description_title.Location = new System.Drawing.Point(862, 512);
            this.description_title.Margin = new System.Windows.Forms.Padding(0);
            this.description_title.Name = "description_title";
            this.description_title.Padding = new System.Windows.Forms.Padding(0, 22, 0, 0);
            this.description_title.Size = new System.Drawing.Size(127, 46);
            this.description_title.TabIndex = 470;
            this.description_title.Text = "Description";
            // 
            // description
            // 
            this.description.AutoSize = true;
            this.description.BackColor = System.Drawing.Color.Transparent;
            this.description.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
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
            this.palette_rgb5a3_ck.ErrorImage = null;
            this.palette_rgb5a3_ck.InitialImage = null;
            this.palette_rgb5a3_ck.Location = new System.Drawing.Point(500, 800);
            this.palette_rgb5a3_ck.Margin = new System.Windows.Forms.Padding(0);
            this.palette_rgb5a3_ck.Name = "palette_rgb5a3_ck";
            this.palette_rgb5a3_ck.Size = new System.Drawing.Size(64, 64);
            this.palette_rgb5a3_ck.TabIndex = 482;
            this.palette_rgb5a3_ck.TabStop = false;
            this.palette_rgb5a3_ck.Click += new System.EventHandler(this.palette_RGB5A3_Click);
            this.palette_rgb5a3_ck.MouseEnter += new System.EventHandler(this.palette_RGB5A3_MouseEnter);
            this.palette_rgb5a3_ck.MouseLeave += new System.EventHandler(this.palette_RGB5A3_MouseLeave);
            // 
            // palette_rgb5a3_label
            // 
            this.palette_rgb5a3_label.AutoSize = true;
            this.palette_rgb5a3_label.BackColor = System.Drawing.Color.Transparent;
            this.palette_rgb5a3_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.palette_rgb5a3_label.ForeColor = System.Drawing.SystemColors.Window;
            this.palette_rgb5a3_label.Location = new System.Drawing.Point(564, 800);
            this.palette_rgb5a3_label.Margin = new System.Windows.Forms.Padding(0);
            this.palette_rgb5a3_label.Name = "palette_rgb5a3_label";
            this.palette_rgb5a3_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.palette_rgb5a3_label.Size = new System.Drawing.Size(89, 68);
            this.palette_rgb5a3_label.TabIndex = 480;
            this.palette_rgb5a3_label.Text = "RGB5A3";
            this.palette_rgb5a3_label.Click += new System.EventHandler(this.palette_RGB5A3_Click);
            this.palette_rgb5a3_label.MouseEnter += new System.EventHandler(this.palette_RGB5A3_MouseEnter);
            this.palette_rgb5a3_label.MouseLeave += new System.EventHandler(this.palette_RGB5A3_MouseLeave);
            // 
            // palette_rgb565_ck
            // 
            this.palette_rgb565_ck.BackColor = System.Drawing.Color.Transparent;
            this.palette_rgb565_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.palette_rgb565_ck.ErrorImage = null;
            this.palette_rgb565_ck.InitialImage = null;
            this.palette_rgb565_ck.Location = new System.Drawing.Point(500, 736);
            this.palette_rgb565_ck.Margin = new System.Windows.Forms.Padding(0);
            this.palette_rgb565_ck.Name = "palette_rgb565_ck";
            this.palette_rgb565_ck.Size = new System.Drawing.Size(64, 64);
            this.palette_rgb565_ck.TabIndex = 479;
            this.palette_rgb565_ck.TabStop = false;
            this.palette_rgb565_ck.Click += new System.EventHandler(this.palette_RGB565_Click);
            this.palette_rgb565_ck.MouseEnter += new System.EventHandler(this.palette_RGB565_MouseEnter);
            this.palette_rgb565_ck.MouseLeave += new System.EventHandler(this.palette_RGB565_MouseLeave);
            // 
            // palette_rgb565_label
            // 
            this.palette_rgb565_label.AutoSize = true;
            this.palette_rgb565_label.BackColor = System.Drawing.Color.Transparent;
            this.palette_rgb565_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.palette_rgb565_label.ForeColor = System.Drawing.SystemColors.Window;
            this.palette_rgb565_label.Location = new System.Drawing.Point(564, 736);
            this.palette_rgb565_label.Margin = new System.Windows.Forms.Padding(0);
            this.palette_rgb565_label.Name = "palette_rgb565_label";
            this.palette_rgb565_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.palette_rgb565_label.Size = new System.Drawing.Size(87, 68);
            this.palette_rgb565_label.TabIndex = 477;
            this.palette_rgb565_label.Text = "RGB565";
            this.palette_rgb565_label.Click += new System.EventHandler(this.palette_RGB565_Click);
            this.palette_rgb565_label.MouseEnter += new System.EventHandler(this.palette_RGB565_MouseEnter);
            this.palette_rgb565_label.MouseLeave += new System.EventHandler(this.palette_RGB565_MouseLeave);
            // 
            // palette_ai8_ck
            // 
            this.palette_ai8_ck.BackColor = System.Drawing.Color.Transparent;
            this.palette_ai8_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.palette_ai8_ck.ErrorImage = null;
            this.palette_ai8_ck.InitialImage = null;
            this.palette_ai8_ck.Location = new System.Drawing.Point(500, 672);
            this.palette_ai8_ck.Margin = new System.Windows.Forms.Padding(0);
            this.palette_ai8_ck.Name = "palette_ai8_ck";
            this.palette_ai8_ck.Size = new System.Drawing.Size(64, 64);
            this.palette_ai8_ck.TabIndex = 476;
            this.palette_ai8_ck.TabStop = false;
            this.palette_ai8_ck.Click += new System.EventHandler(this.palette_AI8_Click);
            this.palette_ai8_ck.MouseEnter += new System.EventHandler(this.palette_AI8_MouseEnter);
            this.palette_ai8_ck.MouseLeave += new System.EventHandler(this.palette_AI8_MouseLeave);
            // 
            // palette_ai8_label
            // 
            this.palette_ai8_label.AutoSize = true;
            this.palette_ai8_label.BackColor = System.Drawing.Color.Transparent;
            this.palette_ai8_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.palette_ai8_label.ForeColor = System.Drawing.SystemColors.Window;
            this.palette_ai8_label.Location = new System.Drawing.Point(564, 672);
            this.palette_ai8_label.Margin = new System.Windows.Forms.Padding(0);
            this.palette_ai8_label.Name = "palette_ai8_label";
            this.palette_ai8_label.Padding = new System.Windows.Forms.Padding(0, 22, 60, 22);
            this.palette_ai8_label.Size = new System.Drawing.Size(102, 68);
            this.palette_ai8_label.TabIndex = 474;
            this.palette_ai8_label.Text = "AI8";
            this.palette_ai8_label.Click += new System.EventHandler(this.palette_AI8_Click);
            this.palette_ai8_label.MouseEnter += new System.EventHandler(this.palette_AI8_MouseEnter);
            this.palette_ai8_label.MouseLeave += new System.EventHandler(this.palette_AI8_MouseLeave);
            // 
            // palette_label
            // 
            this.palette_label.AutoSize = true;
            this.palette_label.BackColor = System.Drawing.Color.Transparent;
            this.palette_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.palette_label.ForeColor = System.Drawing.SystemColors.Control;
            this.palette_label.Location = new System.Drawing.Point(498, 642);
            this.palette_label.Margin = new System.Windows.Forms.Padding(0);
            this.palette_label.Name = "palette_label";
            this.palette_label.Size = new System.Drawing.Size(160, 24);
            this.palette_label.TabIndex = 473;
            this.palette_label.Text = "Palette Format";
            // 
            // github_ck
            // 
            this.github_ck.BackColor = System.Drawing.Color.Transparent;
            this.github_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.github_ck.ErrorImage = null;
            this.github_ck.InitialImage = null;
            this.github_ck.Location = new System.Drawing.Point(1712, 0);
            this.github_ck.Margin = new System.Windows.Forms.Padding(0);
            this.github_ck.Name = "github_ck";
            this.github_ck.Size = new System.Drawing.Size(32, 32);
            this.github_ck.TabIndex = 484;
            this.github_ck.TabStop = false;
            this.github_ck.Click += new System.EventHandler(this.github_Click);
            this.github_ck.MouseEnter += new System.EventHandler(this.github_MouseEnter);
            this.github_ck.MouseLeave += new System.EventHandler(this.github_MouseLeave);
            // 
            // youtube_ck
            // 
            this.youtube_ck.BackColor = System.Drawing.Color.Transparent;
            this.youtube_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.youtube_ck.ErrorImage = null;
            this.youtube_ck.InitialImage = null;
            this.youtube_ck.Location = new System.Drawing.Point(1765, 0);
            this.youtube_ck.Margin = new System.Windows.Forms.Padding(0);
            this.youtube_ck.Name = "youtube_ck";
            this.youtube_ck.Size = new System.Drawing.Size(32, 32);
            this.youtube_ck.TabIndex = 486;
            this.youtube_ck.TabStop = false;
            this.youtube_ck.Click += new System.EventHandler(this.youtube_Click);
            this.youtube_ck.MouseEnter += new System.EventHandler(this.youtube_MouseEnter);
            this.youtube_ck.MouseLeave += new System.EventHandler(this.youtube_MouseLeave);
            // 
            // discord_ck
            // 
            this.discord_ck.BackColor = System.Drawing.Color.Transparent;
            this.discord_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.discord_ck.ErrorImage = null;
            this.discord_ck.InitialImage = null;
            this.discord_ck.Location = new System.Drawing.Point(1661, 0);
            this.discord_ck.Margin = new System.Windows.Forms.Padding(0);
            this.discord_ck.Name = "discord_ck";
            this.discord_ck.Size = new System.Drawing.Size(32, 32);
            this.discord_ck.TabIndex = 488;
            this.discord_ck.TabStop = false;
            this.discord_ck.Click += new System.EventHandler(this.discord_Click);
            this.discord_ck.MouseEnter += new System.EventHandler(this.discord_MouseEnter);
            this.discord_ck.MouseLeave += new System.EventHandler(this.discord_MouseLeave);
            // 
            // desc2
            // 
            this.desc2.AutoSize = true;
            this.desc2.BackColor = System.Drawing.Color.Transparent;
            this.desc2.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
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
            this.desc3.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
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
            this.desc4.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
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
            this.desc5.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
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
            this.desc6.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
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
            this.desc7.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
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
            this.desc8.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
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
            this.desc9.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
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
            // output_label
            // 
            this.output_label.AutoSize = true;
            this.output_label.BackColor = System.Drawing.Color.Transparent;
            this.output_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
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
            this.version_ck.ErrorImage = null;
            this.version_ck.InitialImage = null;
            this.version_ck.Location = new System.Drawing.Point(1109, 0);
            this.version_ck.Margin = new System.Windows.Forms.Padding(0);
            this.version_ck.Name = "version_ck";
            this.version_ck.Size = new System.Drawing.Size(64, 32);
            this.version_ck.TabIndex = 543;
            this.version_ck.TabStop = false;
            this.version_ck.MouseEnter += new System.EventHandler(this.version_MouseEnter);
            this.version_ck.MouseLeave += new System.EventHandler(this.version_MouseLeave);
            // 
            // cli_textbox_label
            // 
            this.cli_textbox_label.AutoSize = true;
            this.cli_textbox_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(20)))), ((int)(((byte)(0)))), ((int)(((byte)(49)))));
            this.cli_textbox_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cli_textbox_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cli_textbox_label.Location = new System.Drawing.Point(71, 1005);
            this.cli_textbox_label.Margin = new System.Windows.Forms.Padding(0);
            this.cli_textbox_label.MaximumSize = new System.Drawing.Size(1400, 128);
            this.cli_textbox_label.Name = "cli_textbox_label";
            this.cli_textbox_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cli_textbox_label.Size = new System.Drawing.Size(0, 68);
            this.cli_textbox_label.TabIndex = 544;
            this.cli_textbox_label.MouseEnter += new System.EventHandler(this.cli_textbox_MouseEnter);
            this.cli_textbox_label.MouseLeave += new System.EventHandler(this.cli_textbox_MouseLeave);
            // 
            // view_rgba_ck
            // 
            this.view_rgba_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_rgba_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_rgba_ck.ErrorImage = null;
            this.view_rgba_ck.InitialImage = null;
            this.view_rgba_ck.Location = new System.Drawing.Point(240, 1323);
            this.view_rgba_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_rgba_ck.Name = "view_rgba_ck";
            this.view_rgba_ck.Size = new System.Drawing.Size(64, 64);
            this.view_rgba_ck.TabIndex = 547;
            this.view_rgba_ck.TabStop = false;
            this.view_rgba_ck.Visible = false;
            this.view_rgba_ck.Click += new System.EventHandler(this.view_rgba_Click);
            this.view_rgba_ck.MouseEnter += new System.EventHandler(this.view_rgba_MouseEnter);
            this.view_rgba_ck.MouseLeave += new System.EventHandler(this.view_rgba_MouseLeave);
            // 
            // view_rgba_label
            // 
            this.view_rgba_label.AutoSize = true;
            this.view_rgba_label.BackColor = System.Drawing.Color.Transparent;
            this.view_rgba_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.view_rgba_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_rgba_label.Location = new System.Drawing.Point(308, 1323);
            this.view_rgba_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_rgba_label.Name = "view_rgba_label";
            this.view_rgba_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_rgba_label.Size = new System.Drawing.Size(146, 68);
            this.view_rgba_label.TabIndex = 545;
            this.view_rgba_label.Text = "Custom RGBA";
            this.view_rgba_label.Visible = false;
            this.view_rgba_label.Click += new System.EventHandler(this.view_rgba_Click);
            this.view_rgba_label.MouseEnter += new System.EventHandler(this.view_rgba_MouseEnter);
            this.view_rgba_label.MouseLeave += new System.EventHandler(this.view_rgba_MouseLeave);
            // 
            // view_palette_ck
            // 
            this.view_palette_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_palette_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_palette_ck.ErrorImage = null;
            this.view_palette_ck.InitialImage = null;
            this.view_palette_ck.Location = new System.Drawing.Point(239, 1387);
            this.view_palette_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_palette_ck.Name = "view_palette_ck";
            this.view_palette_ck.Size = new System.Drawing.Size(64, 64);
            this.view_palette_ck.TabIndex = 550;
            this.view_palette_ck.TabStop = false;
            this.view_palette_ck.Visible = false;
            this.view_palette_ck.Click += new System.EventHandler(this.view_palette_Click);
            this.view_palette_ck.MouseEnter += new System.EventHandler(this.view_palette_MouseEnter);
            this.view_palette_ck.MouseLeave += new System.EventHandler(this.view_palette_MouseLeave);
            // 
            // view_palette_label
            // 
            this.view_palette_label.AutoSize = true;
            this.view_palette_label.BackColor = System.Drawing.Color.Transparent;
            this.view_palette_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.view_palette_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_palette_label.Location = new System.Drawing.Point(307, 1387);
            this.view_palette_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_palette_label.Name = "view_palette_label";
            this.view_palette_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_palette_label.Size = new System.Drawing.Size(83, 68);
            this.view_palette_label.TabIndex = 548;
            this.view_palette_label.Text = "Palette";
            this.view_palette_label.Visible = false;
            this.view_palette_label.Click += new System.EventHandler(this.view_palette_Click);
            this.view_palette_label.MouseEnter += new System.EventHandler(this.view_palette_MouseEnter);
            this.view_palette_label.MouseLeave += new System.EventHandler(this.view_palette_MouseLeave);
            // 
            // view_cmpr_ck
            // 
            this.view_cmpr_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_cmpr_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_cmpr_ck.ErrorImage = null;
            this.view_cmpr_ck.InitialImage = null;
            this.view_cmpr_ck.Location = new System.Drawing.Point(41, 1323);
            this.view_cmpr_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_cmpr_ck.Name = "view_cmpr_ck";
            this.view_cmpr_ck.Size = new System.Drawing.Size(64, 64);
            this.view_cmpr_ck.TabIndex = 583;
            this.view_cmpr_ck.TabStop = false;
            this.view_cmpr_ck.Visible = false;
            this.view_cmpr_ck.Click += new System.EventHandler(this.view_cmpr_Click);
            this.view_cmpr_ck.MouseEnter += new System.EventHandler(this.view_cmpr_MouseEnter);
            this.view_cmpr_ck.MouseLeave += new System.EventHandler(this.view_cmpr_MouseLeave);
            // 
            // view_cmpr_label
            // 
            this.view_cmpr_label.AutoSize = true;
            this.view_cmpr_label.BackColor = System.Drawing.Color.Transparent;
            this.view_cmpr_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.view_cmpr_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_cmpr_label.Location = new System.Drawing.Point(109, 1324);
            this.view_cmpr_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_cmpr_label.Name = "view_cmpr_label";
            this.view_cmpr_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_cmpr_label.Size = new System.Drawing.Size(68, 68);
            this.view_cmpr_label.TabIndex = 581;
            this.view_cmpr_label.Text = "CMPR";
            this.view_cmpr_label.Visible = false;
            this.view_cmpr_label.Click += new System.EventHandler(this.view_cmpr_Click);
            this.view_cmpr_label.MouseEnter += new System.EventHandler(this.view_cmpr_MouseEnter);
            this.view_cmpr_label.MouseLeave += new System.EventHandler(this.view_cmpr_MouseLeave);
            // 
            // view_options_ck
            // 
            this.view_options_ck.BackColor = System.Drawing.Color.Transparent;
            this.view_options_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.view_options_ck.ErrorImage = null;
            this.view_options_ck.InitialImage = null;
            this.view_options_ck.Location = new System.Drawing.Point(40, 1387);
            this.view_options_ck.Margin = new System.Windows.Forms.Padding(0);
            this.view_options_ck.Name = "view_options_ck";
            this.view_options_ck.Size = new System.Drawing.Size(64, 64);
            this.view_options_ck.TabIndex = 586;
            this.view_options_ck.TabStop = false;
            this.view_options_ck.Visible = false;
            this.view_options_ck.Click += new System.EventHandler(this.view_options_Click);
            this.view_options_ck.MouseEnter += new System.EventHandler(this.view_options_MouseEnter);
            this.view_options_ck.MouseLeave += new System.EventHandler(this.view_options_MouseLeave);
            // 
            // view_options_label
            // 
            this.view_options_label.AutoSize = true;
            this.view_options_label.BackColor = System.Drawing.Color.Transparent;
            this.view_options_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.view_options_label.ForeColor = System.Drawing.SystemColors.Window;
            this.view_options_label.Location = new System.Drawing.Point(104, 1388);
            this.view_options_label.Margin = new System.Windows.Forms.Padding(0);
            this.view_options_label.Name = "view_options_label";
            this.view_options_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.view_options_label.Size = new System.Drawing.Size(88, 68);
            this.view_options_label.TabIndex = 584;
            this.view_options_label.Text = "Options";
            this.view_options_label.Visible = false;
            this.view_options_label.Click += new System.EventHandler(this.view_options_Click);
            this.view_options_label.MouseEnter += new System.EventHandler(this.view_options_MouseEnter);
            this.view_options_label.MouseLeave += new System.EventHandler(this.view_options_MouseLeave);
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
            // 
            // textchange_ck
            // 
            this.textchange_ck.BackColor = System.Drawing.Color.Transparent;
            this.textchange_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.textchange_ck.ErrorImage = null;
            this.textchange_ck.InitialImage = null;
            this.textchange_ck.Location = new System.Drawing.Point(500, 32);
            this.textchange_ck.Margin = new System.Windows.Forms.Padding(0);
            this.textchange_ck.Name = "textchange_ck";
            this.textchange_ck.Size = new System.Drawing.Size(64, 64);
            this.textchange_ck.TabIndex = 590;
            this.textchange_ck.TabStop = false;
            this.textchange_ck.Visible = false;
            this.textchange_ck.Click += new System.EventHandler(this.textchange_Click);
            this.textchange_ck.MouseEnter += new System.EventHandler(this.textchange_MouseEnter);
            this.textchange_ck.MouseLeave += new System.EventHandler(this.textchange_MouseLeave);
            // 
            // textchange_label
            // 
            this.textchange_label.AutoSize = true;
            this.textchange_label.BackColor = System.Drawing.Color.Transparent;
            this.textchange_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.textchange_label.ForeColor = System.Drawing.SystemColors.Window;
            this.textchange_label.Location = new System.Drawing.Point(564, 32);
            this.textchange_label.Margin = new System.Windows.Forms.Padding(0);
            this.textchange_label.Name = "textchange_label";
            this.textchange_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.textchange_label.Size = new System.Drawing.Size(307, 68);
            this.textchange_label.TabIndex = 589;
            this.textchange_label.Text = "Update preview for textboxes";
            this.textchange_label.Visible = false;
            this.textchange_label.Click += new System.EventHandler(this.textchange_Click);
            this.textchange_label.MouseEnter += new System.EventHandler(this.textchange_MouseEnter);
            this.textchange_label.MouseLeave += new System.EventHandler(this.textchange_MouseLeave);
            // 
            // auto_update_ck
            // 
            this.auto_update_ck.BackColor = System.Drawing.Color.Transparent;
            this.auto_update_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.auto_update_ck.ErrorImage = null;
            this.auto_update_ck.InitialImage = null;
            this.auto_update_ck.Location = new System.Drawing.Point(968, 32);
            this.auto_update_ck.Margin = new System.Windows.Forms.Padding(0);
            this.auto_update_ck.Name = "auto_update_ck";
            this.auto_update_ck.Size = new System.Drawing.Size(64, 64);
            this.auto_update_ck.TabIndex = 593;
            this.auto_update_ck.TabStop = false;
            this.auto_update_ck.Visible = false;
            this.auto_update_ck.Click += new System.EventHandler(this.auto_update_Click);
            this.auto_update_ck.MouseEnter += new System.EventHandler(this.auto_update_MouseEnter);
            this.auto_update_ck.MouseLeave += new System.EventHandler(this.auto_update_MouseLeave);
            // 
            // auto_update_label
            // 
            this.auto_update_label.AutoSize = true;
            this.auto_update_label.BackColor = System.Drawing.Color.Transparent;
            this.auto_update_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.auto_update_label.ForeColor = System.Drawing.SystemColors.Window;
            this.auto_update_label.Location = new System.Drawing.Point(1032, 32);
            this.auto_update_label.Margin = new System.Windows.Forms.Padding(0);
            this.auto_update_label.Name = "auto_update_label";
            this.auto_update_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.auto_update_label.Size = new System.Drawing.Size(219, 68);
            this.auto_update_label.TabIndex = 592;
            this.auto_update_label.Text = "Auto update preview";
            this.auto_update_label.Visible = false;
            this.auto_update_label.Click += new System.EventHandler(this.auto_update_Click);
            this.auto_update_label.MouseEnter += new System.EventHandler(this.auto_update_MouseEnter);
            this.auto_update_label.MouseLeave += new System.EventHandler(this.auto_update_MouseLeave);
            // 
            // sync_preview_ck
            // 
            this.sync_preview_ck.BackColor = System.Drawing.Color.Transparent;
            this.sync_preview_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.sync_preview_ck.ErrorImage = null;
            this.sync_preview_ck.InitialImage = null;
            this.sync_preview_ck.Location = new System.Drawing.Point(1648, 40);
            this.sync_preview_ck.Margin = new System.Windows.Forms.Padding(0);
            this.sync_preview_ck.Name = "sync_preview_ck";
            this.sync_preview_ck.Size = new System.Drawing.Size(256, 48);
            this.sync_preview_ck.TabIndex = 596;
            this.sync_preview_ck.TabStop = false;
            this.sync_preview_ck.Visible = false;
            this.sync_preview_ck.Click += new System.EventHandler(this.sync_preview_Click);
            this.sync_preview_ck.MouseEnter += new System.EventHandler(this.sync_preview_MouseEnter);
            this.sync_preview_ck.MouseLeave += new System.EventHandler(this.sync_preview_MouseLeave);
            // 
            // upscale_ck
            // 
            this.upscale_ck.BackColor = System.Drawing.Color.Transparent;
            this.upscale_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.upscale_ck.ErrorImage = null;
            this.upscale_ck.InitialImage = null;
            this.upscale_ck.Location = new System.Drawing.Point(1320, 32);
            this.upscale_ck.Margin = new System.Windows.Forms.Padding(0);
            this.upscale_ck.Name = "upscale_ck";
            this.upscale_ck.Size = new System.Drawing.Size(64, 64);
            this.upscale_ck.TabIndex = 599;
            this.upscale_ck.TabStop = false;
            this.upscale_ck.Visible = false;
            this.upscale_ck.Click += new System.EventHandler(this.upscale_Click);
            this.upscale_ck.MouseEnter += new System.EventHandler(this.upscale_MouseEnter);
            this.upscale_ck.MouseLeave += new System.EventHandler(this.upscale_MouseLeave);
            // 
            // upscale_label
            // 
            this.upscale_label.AutoSize = true;
            this.upscale_label.BackColor = System.Drawing.Color.Transparent;
            this.upscale_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.upscale_label.ForeColor = System.Drawing.SystemColors.Window;
            this.upscale_label.Location = new System.Drawing.Point(1384, 32);
            this.upscale_label.Margin = new System.Windows.Forms.Padding(0);
            this.upscale_label.Name = "upscale_label";
            this.upscale_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.upscale_label.Size = new System.Drawing.Size(174, 68);
            this.upscale_label.TabIndex = 598;
            this.upscale_label.Text = "Upscale Preview";
            this.upscale_label.Visible = false;
            this.upscale_label.Click += new System.EventHandler(this.upscale_Click);
            this.upscale_label.MouseEnter += new System.EventHandler(this.upscale_MouseEnter);
            this.upscale_label.MouseLeave += new System.EventHandler(this.upscale_MouseLeave);
            // 
            // banner_move
            // 
            this.banner_move.AutoSize = true;
            this.banner_move.BackColor = System.Drawing.Color.Transparent;
            this.banner_move.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.banner_move.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.banner_move.ForeColor = System.Drawing.SystemColors.Control;
            this.banner_move.Location = new System.Drawing.Point(723, 0);
            this.banner_move.Margin = new System.Windows.Forms.Padding(0);
            this.banner_move.Name = "banner_move";
            this.banner_move.Padding = new System.Windows.Forms.Padding(880, 6, 0, 6);
            this.banner_move.Size = new System.Drawing.Size(880, 32);
            this.banner_move.TabIndex = 601;
            this.banner_move.MouseDown += new System.Windows.Forms.MouseEventHandler(this.banner_move_MouseDown);
            this.banner_move.MouseEnter += new System.EventHandler(this.banner_move_MouseEnter);
            this.banner_move.MouseLeave += new System.EventHandler(this.banner_move_MouseLeave);
            this.banner_move.MouseMove += new System.Windows.Forms.MouseEventHandler(this.banner_move_MouseMove);
            // 
            // preview4k_label
            // 
            this.preview4k_label.AutoSize = true;
            this.preview4k_label.BackColor = System.Drawing.Color.Transparent;
            this.preview4k_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.preview4k_label.ForeColor = System.Drawing.SystemColors.Window;
            this.preview4k_label.Location = new System.Drawing.Point(1160, 1378);
            this.preview4k_label.Margin = new System.Windows.Forms.Padding(0);
            this.preview4k_label.Name = "preview4k_label";
            this.preview4k_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.preview4k_label.Size = new System.Drawing.Size(849, 68);
            this.preview4k_label.TabIndex = 603;
            this.preview4k_label.Text = "4k screen Preview input file (because why not using that space on fullscreen mode" +
    ")";
            this.preview4k_label.Visible = false;
            // 
            // preview4k_ck
            // 
            this.preview4k_ck.BackColor = System.Drawing.Color.Transparent;
            this.preview4k_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.preview4k_ck.Enabled = false;
            this.preview4k_ck.ErrorImage = null;
            this.preview4k_ck.InitialImage = null;
            this.preview4k_ck.Location = new System.Drawing.Point(1091, 1378);
            this.preview4k_ck.Margin = new System.Windows.Forms.Padding(0);
            this.preview4k_ck.Name = "preview4k_ck";
            this.preview4k_ck.Size = new System.Drawing.Size(64, 64);
            this.preview4k_ck.TabIndex = 604;
            this.preview4k_ck.TabStop = false;
            // 
            // reversex_ck
            // 
            this.reversex_ck.BackColor = System.Drawing.Color.Transparent;
            this.reversex_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.reversex_ck.ErrorImage = null;
            this.reversex_ck.InitialImage = null;
            this.reversex_ck.Location = new System.Drawing.Point(1648, 448);
            this.reversex_ck.Margin = new System.Windows.Forms.Padding(0);
            this.reversex_ck.Name = "reversex_ck";
            this.reversex_ck.Size = new System.Drawing.Size(64, 64);
            this.reversex_ck.TabIndex = 607;
            this.reversex_ck.TabStop = false;
            this.reversex_ck.Click += new System.EventHandler(this.reversex_Click);
            this.reversex_ck.MouseEnter += new System.EventHandler(this.reversex_MouseEnter);
            this.reversex_ck.MouseLeave += new System.EventHandler(this.reversex_MouseLeave);
            // 
            // reversex_label
            // 
            this.reversex_label.AutoSize = true;
            this.reversex_label.BackColor = System.Drawing.Color.Transparent;
            this.reversex_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.reversex_label.ForeColor = System.Drawing.SystemColors.Window;
            this.reversex_label.Location = new System.Drawing.Point(1716, 448);
            this.reversex_label.Margin = new System.Windows.Forms.Padding(0);
            this.reversex_label.Name = "reversex_label";
            this.reversex_label.Padding = new System.Windows.Forms.Padding(0, 22, 10, 22);
            this.reversex_label.Size = new System.Drawing.Size(160, 68);
            this.reversex_label.TabIndex = 606;
            this.reversex_label.Text = "reverse x-axis";
            this.reversex_label.Click += new System.EventHandler(this.reversex_Click);
            this.reversex_label.MouseEnter += new System.EventHandler(this.reversex_MouseEnter);
            this.reversex_label.MouseLeave += new System.EventHandler(this.reversex_MouseLeave);
            // 
            // cmpr_c1_label
            // 
            this.cmpr_c1_label.AutoSize = true;
            this.cmpr_c1_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_c1_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_c1_label.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_c1_label.Location = new System.Drawing.Point(2060, 29);
            this.cmpr_c1_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_c1_label.Name = "cmpr_c1_label";
            this.cmpr_c1_label.Padding = new System.Windows.Forms.Padding(15, 15, 15, 10);
            this.cmpr_c1_label.Size = new System.Drawing.Size(124, 49);
            this.cmpr_c1_label.TabIndex = 610;
            this.cmpr_c1_label.Text = "Colour 1";
            this.cmpr_c1_label.MouseEnter += new System.EventHandler(this.cmpr_c1_MouseEnter);
            this.cmpr_c1_label.MouseLeave += new System.EventHandler(this.cmpr_c1_MouseLeave);
            // 
            // cmpr_c1_txt
            // 
            this.cmpr_c1_txt.BackColor = System.Drawing.Color.Black;
            this.cmpr_c1_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cmpr_c1_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_c1_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_c1_txt.Location = new System.Drawing.Point(2059, 73);
            this.cmpr_c1_txt.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_c1_txt.Name = "cmpr_c1_txt";
            this.cmpr_c1_txt.Size = new System.Drawing.Size(141, 24);
            this.cmpr_c1_txt.TabIndex = 19;
            this.cmpr_c1_txt.Text = "#000000";
            this.cmpr_c1_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmpr_c1_txt.TextChanged += new System.EventHandler(this.cmpr_c1_TextChanged);
            this.cmpr_c1_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.cmpr_c1_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.cmpr_c1_txt.MouseEnter += new System.EventHandler(this.cmpr_c1_MouseEnter);
            this.cmpr_c1_txt.MouseLeave += new System.EventHandler(this.cmpr_c1_MouseLeave);
            // 
            // cmpr_c1
            // 
            this.cmpr_c1.AutoSize = true;
            this.cmpr_c1.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_c1.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_c1.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_c1.Location = new System.Drawing.Point(1982, 32);
            this.cmpr_c1.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_c1.Name = "cmpr_c1";
            this.cmpr_c1.Padding = new System.Windows.Forms.Padding(64, 22, 0, 22);
            this.cmpr_c1.Size = new System.Drawing.Size(64, 68);
            this.cmpr_c1.TabIndex = 612;
            this.cmpr_c1.Click += new System.EventHandler(this.cmpr_c1_Click);
            this.cmpr_c1.MouseEnter += new System.EventHandler(this.cmpr_c1_MouseEnter);
            this.cmpr_c1.MouseLeave += new System.EventHandler(this.cmpr_c1_MouseLeave);
            // 
            // cmpr_c2
            // 
            this.cmpr_c2.AutoSize = true;
            this.cmpr_c2.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_c2.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_c2.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_c2.Location = new System.Drawing.Point(1982, 128);
            this.cmpr_c2.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_c2.Name = "cmpr_c2";
            this.cmpr_c2.Padding = new System.Windows.Forms.Padding(64, 22, 0, 22);
            this.cmpr_c2.Size = new System.Drawing.Size(64, 68);
            this.cmpr_c2.TabIndex = 616;
            this.cmpr_c2.Click += new System.EventHandler(this.cmpr_c2_Click);
            this.cmpr_c2.MouseEnter += new System.EventHandler(this.cmpr_c2_MouseEnter);
            this.cmpr_c2.MouseLeave += new System.EventHandler(this.cmpr_c2_MouseLeave);
            // 
            // cmpr_c2_label
            // 
            this.cmpr_c2_label.AutoSize = true;
            this.cmpr_c2_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_c2_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_c2_label.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_c2_label.Location = new System.Drawing.Point(2060, 124);
            this.cmpr_c2_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_c2_label.Name = "cmpr_c2_label";
            this.cmpr_c2_label.Padding = new System.Windows.Forms.Padding(15, 15, 15, 10);
            this.cmpr_c2_label.Size = new System.Drawing.Size(124, 49);
            this.cmpr_c2_label.TabIndex = 614;
            this.cmpr_c2_label.Text = "Colour 2";
            this.cmpr_c2_label.MouseEnter += new System.EventHandler(this.cmpr_c2_MouseEnter);
            this.cmpr_c2_label.MouseLeave += new System.EventHandler(this.cmpr_c2_MouseLeave);
            // 
            // cmpr_c2_txt
            // 
            this.cmpr_c2_txt.BackColor = System.Drawing.Color.Black;
            this.cmpr_c2_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cmpr_c2_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_c2_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_c2_txt.Location = new System.Drawing.Point(2059, 169);
            this.cmpr_c2_txt.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_c2_txt.Name = "cmpr_c2_txt";
            this.cmpr_c2_txt.Size = new System.Drawing.Size(141, 24);
            this.cmpr_c2_txt.TabIndex = 20;
            this.cmpr_c2_txt.Text = "#000000";
            this.cmpr_c2_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmpr_c2_txt.TextChanged += new System.EventHandler(this.cmpr_c2_TextChanged);
            this.cmpr_c2_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.cmpr_c2_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.cmpr_c2_txt.MouseEnter += new System.EventHandler(this.cmpr_c2_MouseEnter);
            this.cmpr_c2_txt.MouseLeave += new System.EventHandler(this.cmpr_c2_MouseLeave);
            // 
            // cmpr_c3
            // 
            this.cmpr_c3.AutoSize = true;
            this.cmpr_c3.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_c3.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_c3.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_c3.Location = new System.Drawing.Point(1982, 224);
            this.cmpr_c3.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_c3.Name = "cmpr_c3";
            this.cmpr_c3.Padding = new System.Windows.Forms.Padding(64, 22, 0, 22);
            this.cmpr_c3.Size = new System.Drawing.Size(64, 68);
            this.cmpr_c3.TabIndex = 620;
            this.cmpr_c3.Click += new System.EventHandler(this.cmpr_c3_Click);
            this.cmpr_c3.MouseEnter += new System.EventHandler(this.cmpr_c3_MouseEnter);
            this.cmpr_c3.MouseLeave += new System.EventHandler(this.cmpr_c3_MouseLeave);
            // 
            // cmpr_c3_label
            // 
            this.cmpr_c3_label.AutoSize = true;
            this.cmpr_c3_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_c3_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_c3_label.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_c3_label.Location = new System.Drawing.Point(2060, 222);
            this.cmpr_c3_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_c3_label.Name = "cmpr_c3_label";
            this.cmpr_c3_label.Padding = new System.Windows.Forms.Padding(15, 15, 15, 10);
            this.cmpr_c3_label.Size = new System.Drawing.Size(124, 49);
            this.cmpr_c3_label.TabIndex = 618;
            this.cmpr_c3_label.Text = "Colour 3";
            this.cmpr_c3_label.MouseEnter += new System.EventHandler(this.cmpr_c3_MouseEnter);
            this.cmpr_c3_label.MouseLeave += new System.EventHandler(this.cmpr_c3_MouseLeave);
            // 
            // cmpr_c3_txt
            // 
            this.cmpr_c3_txt.BackColor = System.Drawing.Color.Black;
            this.cmpr_c3_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cmpr_c3_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_c3_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_c3_txt.Location = new System.Drawing.Point(2059, 267);
            this.cmpr_c3_txt.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_c3_txt.Name = "cmpr_c3_txt";
            this.cmpr_c3_txt.Size = new System.Drawing.Size(141, 24);
            this.cmpr_c3_txt.TabIndex = 21;
            this.cmpr_c3_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmpr_c3_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.cmpr_c3_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.cmpr_c3_txt.MouseEnter += new System.EventHandler(this.cmpr_c3_MouseEnter);
            this.cmpr_c3_txt.MouseLeave += new System.EventHandler(this.cmpr_c3_MouseLeave);
            // 
            // cmpr_c4
            // 
            this.cmpr_c4.AutoSize = true;
            this.cmpr_c4.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_c4.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_c4.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_c4.Location = new System.Drawing.Point(1982, 320);
            this.cmpr_c4.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_c4.Name = "cmpr_c4";
            this.cmpr_c4.Padding = new System.Windows.Forms.Padding(64, 22, 0, 22);
            this.cmpr_c4.Size = new System.Drawing.Size(64, 68);
            this.cmpr_c4.TabIndex = 624;
            this.cmpr_c4.Click += new System.EventHandler(this.cmpr_c4_Click);
            this.cmpr_c4.MouseEnter += new System.EventHandler(this.cmpr_c4_MouseEnter);
            this.cmpr_c4.MouseLeave += new System.EventHandler(this.cmpr_c4_MouseLeave);
            // 
            // cmpr_c4_label
            // 
            this.cmpr_c4_label.AutoSize = true;
            this.cmpr_c4_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_c4_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_c4_label.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_c4_label.Location = new System.Drawing.Point(2060, 319);
            this.cmpr_c4_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_c4_label.Name = "cmpr_c4_label";
            this.cmpr_c4_label.Padding = new System.Windows.Forms.Padding(15, 15, 15, 10);
            this.cmpr_c4_label.Size = new System.Drawing.Size(124, 49);
            this.cmpr_c4_label.TabIndex = 622;
            this.cmpr_c4_label.Text = "Colour 4";
            this.cmpr_c4_label.MouseEnter += new System.EventHandler(this.cmpr_c4_MouseEnter);
            this.cmpr_c4_label.MouseLeave += new System.EventHandler(this.cmpr_c4_MouseLeave);
            // 
            // cmpr_c4_txt
            // 
            this.cmpr_c4_txt.BackColor = System.Drawing.Color.Black;
            this.cmpr_c4_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cmpr_c4_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_c4_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_c4_txt.Location = new System.Drawing.Point(2059, 364);
            this.cmpr_c4_txt.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_c4_txt.Name = "cmpr_c4_txt";
            this.cmpr_c4_txt.Size = new System.Drawing.Size(141, 24);
            this.cmpr_c4_txt.TabIndex = 22;
            this.cmpr_c4_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmpr_c4_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.cmpr_c4_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.cmpr_c4_txt.MouseEnter += new System.EventHandler(this.cmpr_c4_MouseEnter);
            this.cmpr_c4_txt.MouseLeave += new System.EventHandler(this.cmpr_c4_MouseLeave);
            // 
            // cmpr_swap_ck
            // 
            this.cmpr_swap_ck.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_swap_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmpr_swap_ck.ErrorImage = null;
            this.cmpr_swap_ck.InitialImage = null;
            this.cmpr_swap_ck.Location = new System.Drawing.Point(1986, 512);
            this.cmpr_swap_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_swap_ck.Name = "cmpr_swap_ck";
            this.cmpr_swap_ck.Size = new System.Drawing.Size(64, 64);
            this.cmpr_swap_ck.TabIndex = 626;
            this.cmpr_swap_ck.TabStop = false;
            this.cmpr_swap_ck.Click += new System.EventHandler(this.Swap_Colours_Click);
            this.cmpr_swap_ck.MouseEnter += new System.EventHandler(this.cmpr_swap_MouseEnter);
            this.cmpr_swap_ck.MouseLeave += new System.EventHandler(this.cmpr_swap_MouseLeave);
            // 
            // cmpr_swap_label
            // 
            this.cmpr_swap_label.AutoSize = true;
            this.cmpr_swap_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_swap_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_swap_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_swap_label.Location = new System.Drawing.Point(2050, 512);
            this.cmpr_swap_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_swap_label.Name = "cmpr_swap_label";
            this.cmpr_swap_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cmpr_swap_label.Size = new System.Drawing.Size(302, 68);
            this.cmpr_swap_label.TabIndex = 625;
            this.cmpr_swap_label.Text = "Swap Colours + change index";
            this.cmpr_swap_label.Click += new System.EventHandler(this.Swap_Colours_Click);
            this.cmpr_swap_label.MouseEnter += new System.EventHandler(this.cmpr_swap_MouseEnter);
            this.cmpr_swap_label.MouseLeave += new System.EventHandler(this.cmpr_swap_MouseLeave);
            // 
            // cmpr_block_paint_ck
            // 
            this.cmpr_block_paint_ck.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_block_paint_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmpr_block_paint_ck.ErrorImage = null;
            this.cmpr_block_paint_ck.InitialImage = null;
            this.cmpr_block_paint_ck.Location = new System.Drawing.Point(2532, 160);
            this.cmpr_block_paint_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_block_paint_ck.Name = "cmpr_block_paint_ck";
            this.cmpr_block_paint_ck.Size = new System.Drawing.Size(64, 64);
            this.cmpr_block_paint_ck.TabIndex = 665;
            this.cmpr_block_paint_ck.TabStop = false;
            this.cmpr_block_paint_ck.Visible = false;
            this.cmpr_block_paint_ck.Click += new System.EventHandler(this.cmpr_block_paint_Click);
            this.cmpr_block_paint_ck.MouseEnter += new System.EventHandler(this.cmpr_block_paint_MouseEnter);
            this.cmpr_block_paint_ck.MouseLeave += new System.EventHandler(this.cmpr_block_paint_MouseLeave);
            // 
            // cmpr_block_paint_label
            // 
            this.cmpr_block_paint_label.AutoSize = true;
            this.cmpr_block_paint_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_block_paint_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_block_paint_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_block_paint_label.Location = new System.Drawing.Point(2596, 160);
            this.cmpr_block_paint_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_block_paint_label.Name = "cmpr_block_paint_label";
            this.cmpr_block_paint_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cmpr_block_paint_label.Size = new System.Drawing.Size(147, 68);
            this.cmpr_block_paint_label.TabIndex = 663;
            this.cmpr_block_paint_label.Text = "Colour Picker";
            this.cmpr_block_paint_label.Visible = false;
            this.cmpr_block_paint_label.Click += new System.EventHandler(this.cmpr_block_paint_Click);
            this.cmpr_block_paint_label.MouseEnter += new System.EventHandler(this.cmpr_block_paint_MouseEnter);
            this.cmpr_block_paint_label.MouseLeave += new System.EventHandler(this.cmpr_block_paint_MouseLeave);
            // 
            // cmpr_block_selection_ck
            // 
            this.cmpr_block_selection_ck.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_block_selection_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmpr_block_selection_ck.ErrorImage = null;
            this.cmpr_block_selection_ck.InitialImage = null;
            this.cmpr_block_selection_ck.Location = new System.Drawing.Point(2532, 96);
            this.cmpr_block_selection_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_block_selection_ck.Name = "cmpr_block_selection_ck";
            this.cmpr_block_selection_ck.Size = new System.Drawing.Size(64, 64);
            this.cmpr_block_selection_ck.TabIndex = 662;
            this.cmpr_block_selection_ck.TabStop = false;
            this.cmpr_block_selection_ck.Visible = false;
            this.cmpr_block_selection_ck.Click += new System.EventHandler(this.cmpr_block_selection_Click);
            this.cmpr_block_selection_ck.MouseEnter += new System.EventHandler(this.cmpr_block_selection_MouseEnter);
            this.cmpr_block_selection_ck.MouseLeave += new System.EventHandler(this.cmpr_block_selection_MouseLeave);
            // 
            // cmpr_block_selection_label
            // 
            this.cmpr_block_selection_label.AutoSize = true;
            this.cmpr_block_selection_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_block_selection_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_block_selection_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_block_selection_label.Location = new System.Drawing.Point(2596, 96);
            this.cmpr_block_selection_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_block_selection_label.Name = "cmpr_block_selection_label";
            this.cmpr_block_selection_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cmpr_block_selection_label.Size = new System.Drawing.Size(133, 68);
            this.cmpr_block_selection_label.TabIndex = 660;
            this.cmpr_block_selection_label.Text = "Select Block";
            this.cmpr_block_selection_label.Visible = false;
            this.cmpr_block_selection_label.Click += new System.EventHandler(this.cmpr_block_selection_Click);
            this.cmpr_block_selection_label.MouseEnter += new System.EventHandler(this.cmpr_block_selection_MouseEnter);
            this.cmpr_block_selection_label.MouseLeave += new System.EventHandler(this.cmpr_block_selection_MouseLeave);
            // 
            // cmpr_picture_tooltip_label
            // 
            this.cmpr_picture_tooltip_label.AutoSize = true;
            this.cmpr_picture_tooltip_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_picture_tooltip_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_picture_tooltip_label.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_picture_tooltip_label.Location = new System.Drawing.Point(2565, 58);
            this.cmpr_picture_tooltip_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_picture_tooltip_label.Name = "cmpr_picture_tooltip_label";
            this.cmpr_picture_tooltip_label.Size = new System.Drawing.Size(157, 24);
            this.cmpr_picture_tooltip_label.TabIndex = 667;
            this.cmpr_picture_tooltip_label.Text = "Picture tooltip";
            // 
            // cmpr_selected_block_label
            // 
            this.cmpr_selected_block_label.AutoSize = true;
            this.cmpr_selected_block_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_selected_block_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_selected_block_label.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_selected_block_label.Location = new System.Drawing.Point(2029, 621);
            this.cmpr_selected_block_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_selected_block_label.Name = "cmpr_selected_block_label";
            this.cmpr_selected_block_label.Size = new System.Drawing.Size(158, 24);
            this.cmpr_selected_block_label.TabIndex = 668;
            this.cmpr_selected_block_label.Text = "Selected Block";
            // 
            // cmpr_save_ck
            // 
            this.cmpr_save_ck.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_save_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmpr_save_ck.ErrorImage = null;
            this.cmpr_save_ck.InitialImage = null;
            this.cmpr_save_ck.Location = new System.Drawing.Point(2351, 941);
            this.cmpr_save_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_save_ck.Name = "cmpr_save_ck";
            this.cmpr_save_ck.Size = new System.Drawing.Size(192, 64);
            this.cmpr_save_ck.TabIndex = 669;
            this.cmpr_save_ck.TabStop = false;
            this.cmpr_save_ck.Click += new System.EventHandler(this.cmpr_save_ck_Click);
            this.cmpr_save_ck.MouseEnter += new System.EventHandler(this.cmpr_save_MouseEnter);
            this.cmpr_save_ck.MouseLeave += new System.EventHandler(this.cmpr_save_MouseLeave);
            // 
            // cmpr_save_as_ck
            // 
            this.cmpr_save_as_ck.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_save_as_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmpr_save_as_ck.ErrorImage = null;
            this.cmpr_save_as_ck.InitialImage = null;
            this.cmpr_save_as_ck.Location = new System.Drawing.Point(2560, 941);
            this.cmpr_save_as_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_save_as_ck.Name = "cmpr_save_as_ck";
            this.cmpr_save_as_ck.Size = new System.Drawing.Size(256, 64);
            this.cmpr_save_as_ck.TabIndex = 671;
            this.cmpr_save_as_ck.TabStop = false;
            this.cmpr_save_as_ck.Click += new System.EventHandler(this.cmpr_save_as_ck_Click);
            this.cmpr_save_as_ck.MouseEnter += new System.EventHandler(this.cmpr_save_as_MouseEnter);
            this.cmpr_save_as_ck.MouseLeave += new System.EventHandler(this.cmpr_save_as_MouseLeave);
            // 
            // cmpr_warning
            // 
            this.cmpr_warning.AutoSize = true;
            this.cmpr_warning.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_warning.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_warning.ForeColor = System.Drawing.Color.Red;
            this.cmpr_warning.Location = new System.Drawing.Point(2999, 32);
            this.cmpr_warning.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_warning.MaximumSize = new System.Drawing.Size(0, 20);
            this.cmpr_warning.MinimumSize = new System.Drawing.Size(0, 20);
            this.cmpr_warning.Name = "cmpr_warning";
            this.cmpr_warning.Size = new System.Drawing.Size(316, 20);
            this.cmpr_warning.TabIndex = 673;
            this.cmpr_warning.Text = "Input file is not a cmpr texture";
            this.cmpr_warning.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cmpr_sel_label
            // 
            this.cmpr_sel_label.AutoSize = true;
            this.cmpr_sel_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_sel_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_sel_label.ForeColor = System.Drawing.Color.Wheat;
            this.cmpr_sel_label.Location = new System.Drawing.Point(1989, 419);
            this.cmpr_sel_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_sel_label.Name = "cmpr_sel_label";
            this.cmpr_sel_label.Padding = new System.Windows.Forms.Padding(0, 22, 20, 22);
            this.cmpr_sel_label.Size = new System.Drawing.Size(130, 68);
            this.cmpr_sel_label.TabIndex = 674;
            this.cmpr_sel_label.Text = "Selected :";
            this.cmpr_sel_label.MouseEnter += new System.EventHandler(this.cmpr_sel_MouseEnter);
            this.cmpr_sel_label.MouseLeave += new System.EventHandler(this.cmpr_sel_MouseLeave);
            // 
            // cmpr_mouse1_label
            // 
            this.cmpr_mouse1_label.AutoSize = true;
            this.cmpr_mouse1_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_mouse1_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_mouse1_label.ForeColor = System.Drawing.Color.Red;
            this.cmpr_mouse1_label.Location = new System.Drawing.Point(2213, 32);
            this.cmpr_mouse1_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_mouse1_label.Name = "cmpr_mouse1_label";
            this.cmpr_mouse1_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cmpr_mouse1_label.Size = new System.Drawing.Size(130, 68);
            this.cmpr_mouse1_label.TabIndex = 677;
            this.cmpr_mouse1_label.Text = "-> Left Click";
            this.cmpr_mouse1_label.MouseEnter += new System.EventHandler(this.cmpr_mouse1_MouseEnter);
            this.cmpr_mouse1_label.MouseLeave += new System.EventHandler(this.cmpr_mouse1_MouseLeave);
            // 
            // cmpr_mouse2_label
            // 
            this.cmpr_mouse2_label.AutoSize = true;
            this.cmpr_mouse2_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_mouse2_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_mouse2_label.ForeColor = System.Drawing.Color.Red;
            this.cmpr_mouse2_label.Location = new System.Drawing.Point(2213, 128);
            this.cmpr_mouse2_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_mouse2_label.Name = "cmpr_mouse2_label";
            this.cmpr_mouse2_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cmpr_mouse2_label.Size = new System.Drawing.Size(130, 68);
            this.cmpr_mouse2_label.TabIndex = 678;
            this.cmpr_mouse2_label.Text = "-> Left Click";
            this.cmpr_mouse2_label.MouseEnter += new System.EventHandler(this.cmpr_mouse2_MouseEnter);
            this.cmpr_mouse2_label.MouseLeave += new System.EventHandler(this.cmpr_mouse2_MouseLeave);
            // 
            // cmpr_mouse4_label
            // 
            this.cmpr_mouse4_label.AutoSize = true;
            this.cmpr_mouse4_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_mouse4_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_mouse4_label.ForeColor = System.Drawing.Color.Red;
            this.cmpr_mouse4_label.Location = new System.Drawing.Point(2213, 320);
            this.cmpr_mouse4_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_mouse4_label.Name = "cmpr_mouse4_label";
            this.cmpr_mouse4_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cmpr_mouse4_label.Size = new System.Drawing.Size(130, 68);
            this.cmpr_mouse4_label.TabIndex = 680;
            this.cmpr_mouse4_label.Text = "-> Left Click";
            this.cmpr_mouse4_label.MouseEnter += new System.EventHandler(this.cmpr_mouse4_MouseEnter);
            this.cmpr_mouse4_label.MouseLeave += new System.EventHandler(this.cmpr_mouse4_MouseLeave);
            // 
            // cmpr_mouse3_label
            // 
            this.cmpr_mouse3_label.AutoSize = true;
            this.cmpr_mouse3_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_mouse3_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_mouse3_label.ForeColor = System.Drawing.Color.Red;
            this.cmpr_mouse3_label.Location = new System.Drawing.Point(2213, 226);
            this.cmpr_mouse3_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_mouse3_label.Name = "cmpr_mouse3_label";
            this.cmpr_mouse3_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cmpr_mouse3_label.Size = new System.Drawing.Size(130, 68);
            this.cmpr_mouse3_label.TabIndex = 679;
            this.cmpr_mouse3_label.Text = "-> Left Click";
            this.cmpr_mouse3_label.MouseEnter += new System.EventHandler(this.cmpr_mouse3_MouseEnter);
            this.cmpr_mouse3_label.MouseLeave += new System.EventHandler(this.cmpr_mouse3_MouseLeave);
            // 
            // cmpr_mouse5_label
            // 
            this.cmpr_mouse5_label.AutoSize = true;
            this.cmpr_mouse5_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_mouse5_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_mouse5_label.ForeColor = System.Drawing.Color.Red;
            this.cmpr_mouse5_label.Location = new System.Drawing.Point(2213, 419);
            this.cmpr_mouse5_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_mouse5_label.Name = "cmpr_mouse5_label";
            this.cmpr_mouse5_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cmpr_mouse5_label.Size = new System.Drawing.Size(130, 68);
            this.cmpr_mouse5_label.TabIndex = 681;
            this.cmpr_mouse5_label.Text = "-> Left Click";
            this.cmpr_mouse5_label.MouseEnter += new System.EventHandler(this.cmpr_mouse5_MouseEnter);
            this.cmpr_mouse5_label.MouseLeave += new System.EventHandler(this.cmpr_mouse5_MouseLeave);
            // 
            // cmpr_sel
            // 
            this.cmpr_sel.AutoSize = true;
            this.cmpr_sel.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_sel.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_sel.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_sel.Location = new System.Drawing.Point(2136, 419);
            this.cmpr_sel.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_sel.Name = "cmpr_sel";
            this.cmpr_sel.Padding = new System.Windows.Forms.Padding(64, 22, 0, 22);
            this.cmpr_sel.Size = new System.Drawing.Size(64, 68);
            this.cmpr_sel.TabIndex = 682;
            this.cmpr_sel.MouseEnter += new System.EventHandler(this.cmpr_sel_MouseEnter);
            this.cmpr_sel.MouseLeave += new System.EventHandler(this.cmpr_sel_MouseLeave);
            // 
            // bmd_ck
            // 
            this.bmd_ck.BackColor = System.Drawing.Color.Transparent;
            this.bmd_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bmd_ck.ErrorImage = null;
            this.bmd_ck.InitialImage = null;
            this.bmd_ck.Location = new System.Drawing.Point(40, 128);
            this.bmd_ck.Margin = new System.Windows.Forms.Padding(0);
            this.bmd_ck.Name = "bmd_ck";
            this.bmd_ck.Size = new System.Drawing.Size(64, 64);
            this.bmd_ck.TabIndex = 126;
            this.bmd_ck.TabStop = false;
            this.bmd_ck.Click += new System.EventHandler(this.bmd_Click);
            this.bmd_ck.MouseEnter += new System.EventHandler(this.bmd_MouseEnter);
            this.bmd_ck.MouseLeave += new System.EventHandler(this.bmd_MouseLeave);
            // 
            // cmpr_hover_colour
            // 
            this.cmpr_hover_colour.AutoSize = true;
            this.cmpr_hover_colour.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_hover_colour.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_hover_colour.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_hover_colour.Location = new System.Drawing.Point(2564, 344);
            this.cmpr_hover_colour.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_hover_colour.Name = "cmpr_hover_colour";
            this.cmpr_hover_colour.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.cmpr_hover_colour.Size = new System.Drawing.Size(32, 36);
            this.cmpr_hover_colour.TabIndex = 686;
            // 
            // cmpr_hover_colour_label
            // 
            this.cmpr_hover_colour_label.AutoSize = true;
            this.cmpr_hover_colour_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_hover_colour_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_hover_colour_label.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_hover_colour_label.Location = new System.Drawing.Point(2596, 320);
            this.cmpr_hover_colour_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_hover_colour_label.Name = "cmpr_hover_colour_label";
            this.cmpr_hover_colour_label.Padding = new System.Windows.Forms.Padding(0, 15, 0, 10);
            this.cmpr_hover_colour_label.Size = new System.Drawing.Size(142, 49);
            this.cmpr_hover_colour_label.TabIndex = 685;
            this.cmpr_hover_colour_label.Text = "Hover Colour";
            // 
            // cmpr_hover_colour_txt
            // 
            this.cmpr_hover_colour_txt.BackColor = System.Drawing.Color.Black;
            this.cmpr_hover_colour_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cmpr_hover_colour_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_hover_colour_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_hover_colour_txt.Location = new System.Drawing.Point(2596, 365);
            this.cmpr_hover_colour_txt.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_hover_colour_txt.Name = "cmpr_hover_colour_txt";
            this.cmpr_hover_colour_txt.Size = new System.Drawing.Size(141, 24);
            this.cmpr_hover_colour_txt.TabIndex = 23;
            this.cmpr_hover_colour_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmpr_hover_colour_txt.TextChanged += new System.EventHandler(this.cmpr_hover_colour_TextChanged);
            this.cmpr_hover_colour_txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.cmpr_hover_colour_txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            // 
            // cmpr_edited_colour
            // 
            this.cmpr_edited_colour.AutoSize = true;
            this.cmpr_edited_colour.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_edited_colour.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_edited_colour.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_edited_colour.Location = new System.Drawing.Point(2565, 1325);
            this.cmpr_edited_colour.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_edited_colour.Name = "cmpr_edited_colour";
            this.cmpr_edited_colour.Padding = new System.Windows.Forms.Padding(32, 6, 0, 6);
            this.cmpr_edited_colour.Size = new System.Drawing.Size(32, 36);
            this.cmpr_edited_colour.TabIndex = 689;
            this.cmpr_edited_colour.Visible = false;
            // 
            // cmpr_edited_colour_label
            // 
            this.cmpr_edited_colour_label.AutoSize = true;
            this.cmpr_edited_colour_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_edited_colour_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_edited_colour_label.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_edited_colour_label.Location = new System.Drawing.Point(2592, 1295);
            this.cmpr_edited_colour_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_edited_colour_label.Name = "cmpr_edited_colour_label";
            this.cmpr_edited_colour_label.Padding = new System.Windows.Forms.Padding(0, 15, 0, 10);
            this.cmpr_edited_colour_label.Size = new System.Drawing.Size(147, 49);
            this.cmpr_edited_colour_label.TabIndex = 688;
            this.cmpr_edited_colour_label.Text = "Edited Colour";
            this.cmpr_edited_colour_label.Visible = false;
            // 
            // cmpr_edited_colour_txt
            // 
            this.cmpr_edited_colour_txt.BackColor = System.Drawing.Color.Black;
            this.cmpr_edited_colour_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cmpr_edited_colour_txt.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_edited_colour_txt.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_edited_colour_txt.Location = new System.Drawing.Point(2596, 1340);
            this.cmpr_edited_colour_txt.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_edited_colour_txt.Name = "cmpr_edited_colour_txt";
            this.cmpr_edited_colour_txt.Size = new System.Drawing.Size(141, 24);
            this.cmpr_edited_colour_txt.TabIndex = 687;
            this.cmpr_edited_colour_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmpr_edited_colour_txt.Visible = false;
            // 
            // name_string_ck
            // 
            this.name_string_ck.BackColor = System.Drawing.Color.Transparent;
            this.name_string_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.name_string_ck.ErrorImage = null;
            this.name_string_ck.InitialImage = null;
            this.name_string_ck.Location = new System.Drawing.Point(1648, 320);
            this.name_string_ck.Margin = new System.Windows.Forms.Padding(0);
            this.name_string_ck.Name = "name_string_ck";
            this.name_string_ck.Size = new System.Drawing.Size(64, 64);
            this.name_string_ck.TabIndex = 691;
            this.name_string_ck.TabStop = false;
            this.name_string_ck.Click += new System.EventHandler(this.name_string_Click);
            this.name_string_ck.MouseEnter += new System.EventHandler(this.name_string_MouseEnter);
            this.name_string_ck.MouseLeave += new System.EventHandler(this.name_string_MouseLeave);
            // 
            // name_string_label
            // 
            this.name_string_label.AutoSize = true;
            this.name_string_label.BackColor = System.Drawing.Color.Transparent;
            this.name_string_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.name_string_label.ForeColor = System.Drawing.SystemColors.Window;
            this.name_string_label.Location = new System.Drawing.Point(1716, 320);
            this.name_string_label.Margin = new System.Windows.Forms.Padding(0);
            this.name_string_label.Name = "name_string_label";
            this.name_string_label.Padding = new System.Windows.Forms.Padding(0, 22, 30, 22);
            this.name_string_label.Size = new System.Drawing.Size(161, 68);
            this.name_string_label.TabIndex = 690;
            this.name_string_label.Text = "name string";
            this.name_string_label.Click += new System.EventHandler(this.name_string_Click);
            this.name_string_label.MouseEnter += new System.EventHandler(this.name_string_MouseEnter);
            this.name_string_label.MouseLeave += new System.EventHandler(this.name_string_MouseLeave);
            // 
            // cmpr_swap2_ck
            // 
            this.cmpr_swap2_ck.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_swap2_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmpr_swap2_ck.ErrorImage = null;
            this.cmpr_swap2_ck.InitialImage = null;
            this.cmpr_swap2_ck.Location = new System.Drawing.Point(2532, 512);
            this.cmpr_swap2_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_swap2_ck.Name = "cmpr_swap2_ck";
            this.cmpr_swap2_ck.Size = new System.Drawing.Size(64, 64);
            this.cmpr_swap2_ck.TabIndex = 693;
            this.cmpr_swap2_ck.TabStop = false;
            this.cmpr_swap2_ck.Click += new System.EventHandler(this.cmpr_swap2_Click);
            this.cmpr_swap2_ck.MouseEnter += new System.EventHandler(this.cmpr_swap2_MouseEnter);
            this.cmpr_swap2_ck.MouseLeave += new System.EventHandler(this.cmpr_swap2_MouseLeave);
            // 
            // cmpr_swap2_label
            // 
            this.cmpr_swap2_label.AutoSize = true;
            this.cmpr_swap2_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_swap2_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_swap2_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_swap2_label.Location = new System.Drawing.Point(2596, 512);
            this.cmpr_swap2_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_swap2_label.Name = "cmpr_swap2_label";
            this.cmpr_swap2_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cmpr_swap2_label.Size = new System.Drawing.Size(145, 68);
            this.cmpr_swap2_label.TabIndex = 692;
            this.cmpr_swap2_label.Text = "Swap Colours";
            this.cmpr_swap2_label.Click += new System.EventHandler(this.cmpr_swap2_Click);
            this.cmpr_swap2_label.MouseEnter += new System.EventHandler(this.cmpr_swap2_MouseEnter);
            this.cmpr_swap2_label.MouseLeave += new System.EventHandler(this.cmpr_swap2_MouseLeave);
            // 
            // cmpr_hover_ck
            // 
            this.cmpr_hover_ck.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_hover_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmpr_hover_ck.ErrorImage = null;
            this.cmpr_hover_ck.InitialImage = null;
            this.cmpr_hover_ck.Location = new System.Drawing.Point(2532, 241);
            this.cmpr_hover_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_hover_ck.Name = "cmpr_hover_ck";
            this.cmpr_hover_ck.Size = new System.Drawing.Size(64, 64);
            this.cmpr_hover_ck.TabIndex = 696;
            this.cmpr_hover_ck.TabStop = false;
            this.cmpr_hover_ck.Click += new System.EventHandler(this.cmpr_hover_Click);
            this.cmpr_hover_ck.MouseEnter += new System.EventHandler(this.cmpr_hover_MouseEnter);
            this.cmpr_hover_ck.MouseLeave += new System.EventHandler(this.cmpr_hover_MouseLeave);
            // 
            // cmpr_hover_label
            // 
            this.cmpr_hover_label.AutoSize = true;
            this.cmpr_hover_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_hover_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_hover_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_hover_label.Location = new System.Drawing.Point(2596, 241);
            this.cmpr_hover_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_hover_label.Name = "cmpr_hover_label";
            this.cmpr_hover_label.Padding = new System.Windows.Forms.Padding(0, 22, 30, 22);
            this.cmpr_hover_label.Size = new System.Drawing.Size(172, 68);
            this.cmpr_hover_label.TabIndex = 695;
            this.cmpr_hover_label.Text = "Hover Colour";
            this.cmpr_hover_label.Click += new System.EventHandler(this.cmpr_hover_Click);
            this.cmpr_hover_label.MouseEnter += new System.EventHandler(this.cmpr_hover_MouseEnter);
            this.cmpr_hover_label.MouseLeave += new System.EventHandler(this.cmpr_hover_MouseLeave);
            // 
            // cmpr_update_preview_ck
            // 
            this.cmpr_update_preview_ck.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_update_preview_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmpr_update_preview_ck.ErrorImage = null;
            this.cmpr_update_preview_ck.InitialImage = null;
            this.cmpr_update_preview_ck.Location = new System.Drawing.Point(2532, 420);
            this.cmpr_update_preview_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_update_preview_ck.Name = "cmpr_update_preview_ck";
            this.cmpr_update_preview_ck.Size = new System.Drawing.Size(64, 64);
            this.cmpr_update_preview_ck.TabIndex = 698;
            this.cmpr_update_preview_ck.TabStop = false;
            this.cmpr_update_preview_ck.Click += new System.EventHandler(this.cmpr_update_preview_Click);
            this.cmpr_update_preview_ck.MouseEnter += new System.EventHandler(this.cmpr_update_preview_MouseEnter);
            this.cmpr_update_preview_ck.MouseLeave += new System.EventHandler(this.cmpr_update_preview_MouseLeave);
            // 
            // cmpr_update_preview_label
            // 
            this.cmpr_update_preview_label.AutoSize = true;
            this.cmpr_update_preview_label.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_update_preview_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cmpr_update_preview_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_update_preview_label.Location = new System.Drawing.Point(2596, 420);
            this.cmpr_update_preview_label.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_update_preview_label.Name = "cmpr_update_preview_label";
            this.cmpr_update_preview_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.cmpr_update_preview_label.Size = new System.Drawing.Size(168, 68);
            this.cmpr_update_preview_label.TabIndex = 697;
            this.cmpr_update_preview_label.Text = "Update Preview";
            this.cmpr_update_preview_label.Click += new System.EventHandler(this.cmpr_update_preview_Click);
            this.cmpr_update_preview_label.MouseEnter += new System.EventHandler(this.cmpr_update_preview_MouseEnter);
            this.cmpr_update_preview_label.MouseLeave += new System.EventHandler(this.cmpr_update_preview_MouseLeave);
            // 
            // banner_global_move_ck
            // 
            this.banner_global_move_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_global_move_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_global_move_ck.ErrorImage = null;
            this.banner_global_move_ck.InitialImage = null;
            this.banner_global_move_ck.Location = new System.Drawing.Point(396, 0);
            this.banner_global_move_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_global_move_ck.Name = "banner_global_move_ck";
            this.banner_global_move_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_global_move_ck.TabIndex = 699;
            this.banner_global_move_ck.TabStop = false;
            this.banner_global_move_ck.Click += new System.EventHandler(this.banner_global_move_Click);
            this.banner_global_move_ck.MouseEnter += new System.EventHandler(this.banner_global_move_MouseEnter);
            this.banner_global_move_ck.MouseLeave += new System.EventHandler(this.banner_global_move_MouseLeave);
            // 
            // banner_5_ck
            // 
            this.banner_5_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_5_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_5_ck.ErrorImage = null;
            this.banner_5_ck.InitialImage = null;
            this.banner_5_ck.Location = new System.Drawing.Point(704, 0);
            this.banner_5_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_5_ck.Name = "banner_5_ck";
            this.banner_5_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_5_ck.TabIndex = 700;
            this.banner_5_ck.TabStop = false;
            this.banner_5_ck.Click += new System.EventHandler(this.Arrow_1080p_Click);
            this.banner_5_ck.MouseEnter += new System.EventHandler(this.Arrow_1080p_MouseEnter);
            this.banner_5_ck.MouseLeave += new System.EventHandler(this.Arrow_1080p_MouseLeave);
            // 
            // banner_11_ck
            // 
            this.banner_11_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_11_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_11_ck.ErrorImage = null;
            this.banner_11_ck.InitialImage = null;
            this.banner_11_ck.Location = new System.Drawing.Point(960, 0);
            this.banner_11_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_11_ck.Name = "banner_11_ck";
            this.banner_11_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_11_ck.TabIndex = 708;
            this.banner_11_ck.TabStop = false;
            this.banner_11_ck.Click += new System.EventHandler(this.Screen2_Bottom_left_Click);
            this.banner_11_ck.MouseEnter += new System.EventHandler(this.Screen2_Bottom_left_MouseEnter);
            this.banner_11_ck.MouseLeave += new System.EventHandler(this.Screen2_Bottom_left_MouseLeave);
            // 
            // banner_12_ck
            // 
            this.banner_12_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_12_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_12_ck.ErrorImage = null;
            this.banner_12_ck.InitialImage = null;
            this.banner_12_ck.Location = new System.Drawing.Point(928, 0);
            this.banner_12_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_12_ck.Name = "banner_12_ck";
            this.banner_12_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_12_ck.TabIndex = 707;
            this.banner_12_ck.TabStop = false;
            this.banner_12_ck.Click += new System.EventHandler(this.Screen2_Bottom_Click);
            this.banner_12_ck.MouseEnter += new System.EventHandler(this.Screen2_Bottom_MouseEnter);
            this.banner_12_ck.MouseLeave += new System.EventHandler(this.Screen2_Bottom_MouseLeave);
            // 
            // banner_13_ck
            // 
            this.banner_13_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_13_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_13_ck.ErrorImage = null;
            this.banner_13_ck.InitialImage = null;
            this.banner_13_ck.Location = new System.Drawing.Point(896, 0);
            this.banner_13_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_13_ck.Name = "banner_13_ck";
            this.banner_13_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_13_ck.TabIndex = 706;
            this.banner_13_ck.TabStop = false;
            this.banner_13_ck.Click += new System.EventHandler(this.Screen2_Bottom_right_Click);
            this.banner_13_ck.MouseEnter += new System.EventHandler(this.Screen2_Bottom_right_MouseEnter);
            this.banner_13_ck.MouseLeave += new System.EventHandler(this.Screen2_Bottom_right_MouseLeave);
            // 
            // banner_14_ck
            // 
            this.banner_14_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_14_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_14_ck.ErrorImage = null;
            this.banner_14_ck.InitialImage = null;
            this.banner_14_ck.Location = new System.Drawing.Point(736, 0);
            this.banner_14_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_14_ck.Name = "banner_14_ck";
            this.banner_14_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_14_ck.TabIndex = 705;
            this.banner_14_ck.TabStop = false;
            this.banner_14_ck.Click += new System.EventHandler(this.Screen2_Left_Click);
            this.banner_14_ck.MouseEnter += new System.EventHandler(this.Screen2_Left_MouseEnter);
            this.banner_14_ck.MouseLeave += new System.EventHandler(this.Screen2_Left_MouseLeave);
            // 
            // banner_16_ck
            // 
            this.banner_16_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_16_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_16_ck.ErrorImage = null;
            this.banner_16_ck.InitialImage = null;
            this.banner_16_ck.Location = new System.Drawing.Point(864, 0);
            this.banner_16_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_16_ck.Name = "banner_16_ck";
            this.banner_16_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_16_ck.TabIndex = 704;
            this.banner_16_ck.TabStop = false;
            this.banner_16_ck.Click += new System.EventHandler(this.Screen2_Right_Click);
            this.banner_16_ck.MouseEnter += new System.EventHandler(this.Screen2_Right_MouseEnter);
            this.banner_16_ck.MouseLeave += new System.EventHandler(this.Screen2_Right_MouseLeave);
            // 
            // banner_17_ck
            // 
            this.banner_17_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_17_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_17_ck.ErrorImage = null;
            this.banner_17_ck.InitialImage = null;
            this.banner_17_ck.Location = new System.Drawing.Point(768, 0);
            this.banner_17_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_17_ck.Name = "banner_17_ck";
            this.banner_17_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_17_ck.TabIndex = 703;
            this.banner_17_ck.TabStop = false;
            this.banner_17_ck.Click += new System.EventHandler(this.Screen2_Top_left_Click);
            this.banner_17_ck.MouseEnter += new System.EventHandler(this.Screen2_Top_left_MouseEnter);
            this.banner_17_ck.MouseLeave += new System.EventHandler(this.Screen2_Top_left_MouseLeave);
            // 
            // banner_18_ck
            // 
            this.banner_18_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_18_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_18_ck.ErrorImage = null;
            this.banner_18_ck.InitialImage = null;
            this.banner_18_ck.Location = new System.Drawing.Point(800, 0);
            this.banner_18_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_18_ck.Name = "banner_18_ck";
            this.banner_18_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_18_ck.TabIndex = 702;
            this.banner_18_ck.TabStop = false;
            this.banner_18_ck.Click += new System.EventHandler(this.Screen2_Top_Click);
            this.banner_18_ck.MouseEnter += new System.EventHandler(this.Screen2_Top_MouseEnter);
            this.banner_18_ck.MouseLeave += new System.EventHandler(this.Screen2_Top_MouseLeave);
            // 
            // banner_19_ck
            // 
            this.banner_19_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_19_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_19_ck.ErrorImage = null;
            this.banner_19_ck.InitialImage = null;
            this.banner_19_ck.Location = new System.Drawing.Point(832, 0);
            this.banner_19_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_19_ck.Name = "banner_19_ck";
            this.banner_19_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_19_ck.TabIndex = 701;
            this.banner_19_ck.TabStop = false;
            this.banner_19_ck.Click += new System.EventHandler(this.Screen2_Top_right_Click);
            this.banner_19_ck.MouseEnter += new System.EventHandler(this.Screen2_Top_right_MouseEnter);
            this.banner_19_ck.MouseLeave += new System.EventHandler(this.Screen2_Top_right_MouseLeave);
            // 
            // banner_15_ck
            // 
            this.banner_15_ck.BackColor = System.Drawing.Color.Transparent;
            this.banner_15_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.banner_15_ck.ErrorImage = null;
            this.banner_15_ck.InitialImage = null;
            this.banner_15_ck.Location = new System.Drawing.Point(992, 0);
            this.banner_15_ck.Margin = new System.Windows.Forms.Padding(0);
            this.banner_15_ck.Name = "banner_15_ck";
            this.banner_15_ck.Size = new System.Drawing.Size(32, 32);
            this.banner_15_ck.TabIndex = 711;
            this.banner_15_ck.TabStop = false;
            this.banner_15_ck.Click += new System.EventHandler(this.Screen2_Arrow_1080p_Click);
            this.banner_15_ck.MouseEnter += new System.EventHandler(this.Screen2_Arrow_1080p_MouseEnter);
            this.banner_15_ck.MouseLeave += new System.EventHandler(this.Screen2_Arrow_1080p_MouseLeave);
            // 
            // sooperbmd_label
            // 
            this.sooperbmd_label.AutoSize = true;
            this.sooperbmd_label.BackColor = System.Drawing.Color.Transparent;
            this.sooperbmd_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.sooperbmd_label.ForeColor = System.Drawing.SystemColors.Window;
            this.sooperbmd_label.Location = new System.Drawing.Point(571, 1232);
            this.sooperbmd_label.Margin = new System.Windows.Forms.Padding(0);
            this.sooperbmd_label.Name = "sooperbmd_label";
            this.sooperbmd_label.Padding = new System.Windows.Forms.Padding(0, 22, 50, 22);
            this.sooperbmd_label.Size = new System.Drawing.Size(176, 68);
            this.sooperbmd_label.TabIndex = 714;
            this.sooperbmd_label.Text = "SooperBMD";
            this.sooperbmd_label.Visible = false;
            this.sooperbmd_label.Click += new System.EventHandler(this.SooperBMD_Click);
            this.sooperbmd_label.MouseEnter += new System.EventHandler(this.SooperBMD_MouseEnter);
            this.sooperbmd_label.MouseLeave += new System.EventHandler(this.SooperBMD_MouseLeave);
            // 
            // min_max_label
            // 
            this.min_max_label.AutoSize = true;
            this.min_max_label.BackColor = System.Drawing.Color.Transparent;
            this.min_max_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.min_max_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_max_label.Location = new System.Drawing.Point(571, 1296);
            this.min_max_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_max_label.Name = "min_max_label";
            this.min_max_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_max_label.Size = new System.Drawing.Size(98, 68);
            this.min_max_label.TabIndex = 716;
            this.min_max_label.Text = "Min/Max";
            this.min_max_label.Visible = false;
            this.min_max_label.Click += new System.EventHandler(this.Min_Max_Click);
            this.min_max_label.MouseEnter += new System.EventHandler(this.Min_Max_MouseEnter);
            this.min_max_label.MouseLeave += new System.EventHandler(this.Min_Max_MouseLeave);
            // 
            // weemm_label
            // 
            this.weemm_label.AutoSize = true;
            this.weemm_label.BackColor = System.Drawing.Color.Transparent;
            this.weemm_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.weemm_label.ForeColor = System.Drawing.SystemColors.Window;
            this.weemm_label.Location = new System.Drawing.Point(571, 1168);
            this.weemm_label.Margin = new System.Windows.Forms.Padding(0);
            this.weemm_label.Name = "weemm_label";
            this.weemm_label.Padding = new System.Windows.Forms.Padding(0, 22, 50, 22);
            this.weemm_label.Size = new System.Drawing.Size(139, 68);
            this.weemm_label.TabIndex = 712;
            this.weemm_label.Text = "Weemm";
            this.weemm_label.Visible = false;
            this.weemm_label.Click += new System.EventHandler(this.Weemm_Click);
            this.weemm_label.MouseEnter += new System.EventHandler(this.Weemm_MouseEnter);
            this.weemm_label.MouseLeave += new System.EventHandler(this.Weemm_MouseLeave);
            // 
            // min_max_ck
            // 
            this.min_max_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_max_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_max_ck.ErrorImage = null;
            this.min_max_ck.InitialImage = null;
            this.min_max_ck.Location = new System.Drawing.Point(503, 1296);
            this.min_max_ck.Margin = new System.Windows.Forms.Padding(0);
            this.min_max_ck.Name = "min_max_ck";
            this.min_max_ck.Size = new System.Drawing.Size(64, 64);
            this.min_max_ck.TabIndex = 717;
            this.min_max_ck.TabStop = false;
            this.min_max_ck.Visible = false;
            this.min_max_ck.Click += new System.EventHandler(this.Min_Max_Click);
            this.min_max_ck.MouseEnter += new System.EventHandler(this.Min_Max_MouseEnter);
            this.min_max_ck.MouseLeave += new System.EventHandler(this.Min_Max_MouseLeave);
            // 
            // sooperbmd_ck
            // 
            this.sooperbmd_ck.BackColor = System.Drawing.Color.Transparent;
            this.sooperbmd_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.sooperbmd_ck.ErrorImage = null;
            this.sooperbmd_ck.InitialImage = null;
            this.sooperbmd_ck.Location = new System.Drawing.Point(503, 1232);
            this.sooperbmd_ck.Margin = new System.Windows.Forms.Padding(0);
            this.sooperbmd_ck.Name = "sooperbmd_ck";
            this.sooperbmd_ck.Size = new System.Drawing.Size(64, 64);
            this.sooperbmd_ck.TabIndex = 715;
            this.sooperbmd_ck.TabStop = false;
            this.sooperbmd_ck.Visible = false;
            this.sooperbmd_ck.Click += new System.EventHandler(this.SooperBMD_Click);
            this.sooperbmd_ck.MouseEnter += new System.EventHandler(this.SooperBMD_MouseEnter);
            this.sooperbmd_ck.MouseLeave += new System.EventHandler(this.SooperBMD_MouseLeave);
            // 
            // weemm_ck
            // 
            this.weemm_ck.BackColor = System.Drawing.Color.Transparent;
            this.weemm_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.weemm_ck.ErrorImage = null;
            this.weemm_ck.InitialImage = null;
            this.weemm_ck.Location = new System.Drawing.Point(500, 1168);
            this.weemm_ck.Margin = new System.Windows.Forms.Padding(0);
            this.weemm_ck.Name = "weemm_ck";
            this.weemm_ck.Size = new System.Drawing.Size(64, 64);
            this.weemm_ck.TabIndex = 713;
            this.weemm_ck.TabStop = false;
            this.weemm_ck.Visible = false;
            this.weemm_ck.Click += new System.EventHandler(this.Weemm_Click);
            this.weemm_ck.MouseEnter += new System.EventHandler(this.Weemm_MouseEnter);
            this.weemm_ck.MouseLeave += new System.EventHandler(this.Weemm_MouseLeave);
            // 
            // darkest_lightest_label
            // 
            this.darkest_lightest_label.AutoSize = true;
            this.darkest_lightest_label.BackColor = System.Drawing.Color.Transparent;
            this.darkest_lightest_label.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.darkest_lightest_label.ForeColor = System.Drawing.SystemColors.Window;
            this.darkest_lightest_label.Location = new System.Drawing.Point(564, 320);
            this.darkest_lightest_label.Margin = new System.Windows.Forms.Padding(0);
            this.darkest_lightest_label.Name = "darkest_lightest_label";
            this.darkest_lightest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.darkest_lightest_label.Size = new System.Drawing.Size(85, 68);
            this.darkest_lightest_label.TabIndex = 718;
            this.darkest_lightest_label.Text = "Gamma";
            this.darkest_lightest_label.Visible = false;
            this.darkest_lightest_label.Click += new System.EventHandler(this.Darkest_Lightest_Click);
            this.darkest_lightest_label.MouseEnter += new System.EventHandler(this.Darkest_Lightest_MouseEnter);
            this.darkest_lightest_label.MouseLeave += new System.EventHandler(this.Darkest_Lightest_MouseLeave);
            // 
            // darkest_lightest_ck
            // 
            this.darkest_lightest_ck.BackColor = System.Drawing.Color.Transparent;
            this.darkest_lightest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.darkest_lightest_ck.ErrorImage = null;
            this.darkest_lightest_ck.InitialImage = null;
            this.darkest_lightest_ck.Location = new System.Drawing.Point(500, 320);
            this.darkest_lightest_ck.Margin = new System.Windows.Forms.Padding(0);
            this.darkest_lightest_ck.Name = "darkest_lightest_ck";
            this.darkest_lightest_ck.Size = new System.Drawing.Size(64, 64);
            this.darkest_lightest_ck.TabIndex = 719;
            this.darkest_lightest_ck.TabStop = false;
            this.darkest_lightest_ck.Visible = false;
            this.darkest_lightest_ck.Click += new System.EventHandler(this.Darkest_Lightest_Click);
            this.darkest_lightest_ck.MouseEnter += new System.EventHandler(this.Darkest_Lightest_MouseEnter);
            this.darkest_lightest_ck.MouseLeave += new System.EventHandler(this.Darkest_Lightest_MouseLeave);
            // 
            // cmpr_palette
            // 
            this.cmpr_palette.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_palette.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmpr_palette.ErrorImage = null;
            this.cmpr_palette.InitialImage = null;
            this.cmpr_palette.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.cmpr_palette.Location = new System.Drawing.Point(1920, 1016);
            this.cmpr_palette.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_palette.Name = "cmpr_palette";
            this.cmpr_palette.Size = new System.Drawing.Size(896, 64);
            this.cmpr_palette.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.cmpr_palette.TabIndex = 694;
            this.cmpr_palette.TabStop = false;
            this.cmpr_palette.Visible = false;
            this.cmpr_palette.MouseEnter += new System.EventHandler(this.cmpr_palette_MouseEnter);
            this.cmpr_palette.MouseLeave += new System.EventHandler(this.cmpr_palette_MouseLeave);
            // 
            // cmpr_grid_ck
            // 
            this.cmpr_grid_ck.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_grid_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.cmpr_grid_ck.ErrorImage = null;
            this.cmpr_grid_ck.InitialImage = null;
            this.cmpr_grid_ck.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.cmpr_grid_ck.Location = new System.Drawing.Point(1986, 661);
            this.cmpr_grid_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_grid_ck.MaximumSize = new System.Drawing.Size(256, 256);
            this.cmpr_grid_ck.MinimumSize = new System.Drawing.Size(256, 256);
            this.cmpr_grid_ck.Name = "cmpr_grid_ck";
            this.cmpr_grid_ck.Size = new System.Drawing.Size(256, 256);
            this.cmpr_grid_ck.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.cmpr_grid_ck.TabIndex = 683;
            this.cmpr_grid_ck.TabStop = false;
            this.cmpr_grid_ck.Visible = false;
            this.cmpr_grid_ck.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cmpr_grid_ck_MouseMove);
            this.cmpr_grid_ck.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cmpr_grid_ck_MouseMove);
            // 
            // cmpr_preview_ck
            // 
            this.cmpr_preview_ck.BackColor = System.Drawing.Color.Transparent;
            this.cmpr_preview_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.cmpr_preview_ck.ErrorImage = null;
            this.cmpr_preview_ck.InitialImage = null;
            this.cmpr_preview_ck.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.cmpr_preview_ck.Location = new System.Drawing.Point(2816, 56);
            this.cmpr_preview_ck.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_preview_ck.MaximumSize = new System.Drawing.Size(1024, 1024);
            this.cmpr_preview_ck.MinimumSize = new System.Drawing.Size(1024, 1024);
            this.cmpr_preview_ck.Name = "cmpr_preview_ck";
            this.cmpr_preview_ck.Size = new System.Drawing.Size(1024, 1024);
            this.cmpr_preview_ck.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.cmpr_preview_ck.TabIndex = 666;
            this.cmpr_preview_ck.TabStop = false;
            this.cmpr_preview_ck.Visible = false;
            this.cmpr_preview_ck.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cmpr_preview_ck_MouseDown);
            this.cmpr_preview_ck.MouseLeave += new System.EventHandler(this.cmpr_preview_ck_MouseLeave);
            this.cmpr_preview_ck.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cmpr_preview_ck_MouseMove);
            // 
            // image_ck
            // 
            this.image_ck.BackColor = System.Drawing.Color.Transparent;
            this.image_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.image_ck.Enabled = false;
            this.image_ck.ErrorImage = null;
            this.image_ck.InitialImage = null;
            this.image_ck.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.image_ck.Location = new System.Drawing.Point(815, 1596);
            this.image_ck.Margin = new System.Windows.Forms.Padding(0);
            this.image_ck.MaximumSize = new System.Drawing.Size(768, 768);
            this.image_ck.MinimumSize = new System.Drawing.Size(768, 768);
            this.image_ck.Name = "image_ck";
            this.image_ck.Size = new System.Drawing.Size(768, 768);
            this.image_ck.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.image_ck.TabIndex = 602;
            this.image_ck.TabStop = false;
            this.image_ck.Visible = false;
            // 
            // plt0_gui
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(72)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(3199, 1920);
            this.Controls.Add(this.darkest_lightest_label);
            this.Controls.Add(this.darkest_lightest_ck);
            this.Controls.Add(this.sooperbmd_label);
            this.Controls.Add(this.min_max_label);
            this.Controls.Add(this.weemm_label);
            this.Controls.Add(this.min_max_ck);
            this.Controls.Add(this.sooperbmd_ck);
            this.Controls.Add(this.weemm_ck);
            this.Controls.Add(this.banner_15_ck);
            this.Controls.Add(this.banner_11_ck);
            this.Controls.Add(this.banner_12_ck);
            this.Controls.Add(this.banner_13_ck);
            this.Controls.Add(this.banner_14_ck);
            this.Controls.Add(this.banner_16_ck);
            this.Controls.Add(this.banner_17_ck);
            this.Controls.Add(this.banner_18_ck);
            this.Controls.Add(this.banner_19_ck);
            this.Controls.Add(this.banner_5_ck);
            this.Controls.Add(this.banner_global_move_ck);
            this.Controls.Add(this.cmpr_update_preview_ck);
            this.Controls.Add(this.cmpr_update_preview_label);
            this.Controls.Add(this.cmpr_hover_ck);
            this.Controls.Add(this.cmpr_hover_label);
            this.Controls.Add(this.cmpr_palette);
            this.Controls.Add(this.cmpr_swap2_ck);
            this.Controls.Add(this.cmpr_swap2_label);
            this.Controls.Add(this.name_string_ck);
            this.Controls.Add(this.name_string_label);
            this.Controls.Add(this.mandatory_settings_label);
            this.Controls.Add(this.cmpr_edited_colour);
            this.Controls.Add(this.cmpr_edited_colour_label);
            this.Controls.Add(this.cmpr_edited_colour_txt);
            this.Controls.Add(this.cmpr_hover_colour);
            this.Controls.Add(this.cmpr_hover_colour_label);
            this.Controls.Add(this.cmpr_hover_colour_txt);
            this.Controls.Add(this.cmpr_grid_ck);
            this.Controls.Add(this.cmpr_sel);
            this.Controls.Add(this.cmpr_mouse5_label);
            this.Controls.Add(this.cmpr_mouse4_label);
            this.Controls.Add(this.cmpr_mouse3_label);
            this.Controls.Add(this.cmpr_mouse2_label);
            this.Controls.Add(this.cmpr_mouse1_label);
            this.Controls.Add(this.cmpr_sel_label);
            this.Controls.Add(this.cmpr_warning);
            this.Controls.Add(this.cmpr_save_as_ck);
            this.Controls.Add(this.cmpr_save_ck);
            this.Controls.Add(this.cmpr_selected_block_label);
            this.Controls.Add(this.cmpr_picture_tooltip_label);
            this.Controls.Add(this.cmpr_preview_ck);
            this.Controls.Add(this.cmpr_block_paint_ck);
            this.Controls.Add(this.cmpr_block_paint_label);
            this.Controls.Add(this.cmpr_block_selection_ck);
            this.Controls.Add(this.cmpr_block_selection_label);
            this.Controls.Add(this.cmpr_swap_ck);
            this.Controls.Add(this.cmpr_swap_label);
            this.Controls.Add(this.cmpr_c4);
            this.Controls.Add(this.cmpr_c4_label);
            this.Controls.Add(this.cmpr_c4_txt);
            this.Controls.Add(this.cmpr_c3);
            this.Controls.Add(this.cmpr_c3_label);
            this.Controls.Add(this.cmpr_c3_txt);
            this.Controls.Add(this.cmpr_c2);
            this.Controls.Add(this.cmpr_c2_label);
            this.Controls.Add(this.cmpr_c2_txt);
            this.Controls.Add(this.cmpr_c1);
            this.Controls.Add(this.cmpr_c1_label);
            this.Controls.Add(this.cmpr_c1_txt);
            this.Controls.Add(this.reversex_ck);
            this.Controls.Add(this.reversex_label);
            this.Controls.Add(this.preview4k_label);
            this.Controls.Add(this.preview4k_ck);
            this.Controls.Add(this.image_ck);
            this.Controls.Add(this.upscale_label);
            this.Controls.Add(this.auto_update_label);
            this.Controls.Add(this.textchange_label);
            this.Controls.Add(this.upscale_ck);
            this.Controls.Add(this.sync_preview_ck);
            this.Controls.Add(this.auto_update_ck);
            this.Controls.Add(this.textchange_ck);
            this.Controls.Add(this.palette_rgb565_label);
            this.Controls.Add(this.alpha_label);
            this.Controls.Add(this.ask_exit_ck);
            this.Controls.Add(this.ask_exit_label);
            this.Controls.Add(this.Tclamp_label);
            this.Controls.Add(this.Trepeat_label);
            this.Controls.Add(this.Tmirror_label);
            this.Controls.Add(this.Smirror_label);
            this.Controls.Add(this.Srepeat_label);
            this.Controls.Add(this.Sclamp_label);
            this.Controls.Add(this.mag_linearmipmaplinear_label);
            this.Controls.Add(this.mag_linearmipmapnearest_label);
            this.Controls.Add(this.mag_nearestmipmaplinear_label);
            this.Controls.Add(this.mag_nearestmipmapnearest_label);
            this.Controls.Add(this.mag_linear_label);
            this.Controls.Add(this.mag_nearest_neighbour_label);
            this.Controls.Add(this.min_nearest_neighbour_label);
            this.Controls.Add(this.min_linear_label);
            this.Controls.Add(this.min_nearestmipmapnearest_label);
            this.Controls.Add(this.min_nearestmipmaplinear_label);
            this.Controls.Add(this.min_linearmipmapnearest_label);
            this.Controls.Add(this.min_linearmipmaplinear_label);
            this.Controls.Add(this.cie_709_label);
            this.Controls.Add(this.custom_label);
            this.Controls.Add(this.no_gradient_label);
            this.Controls.Add(this.no_alpha_label);
            this.Controls.Add(this.mix_label);
            this.Controls.Add(this.palette_ai8_ck);
            this.Controls.Add(this.palette_ai8_label);
            this.Controls.Add(this.cie_601_label);
            this.Controls.Add(this.view_options_ck);
            this.Controls.Add(this.view_options_label);
            this.Controls.Add(this.view_cmpr_ck);
            this.Controls.Add(this.view_cmpr_label);
            this.Controls.Add(this.view_palette_ck);
            this.Controls.Add(this.view_palette_label);
            this.Controls.Add(this.view_rgba_ck);
            this.Controls.Add(this.view_rgba_label);
            this.Controls.Add(this.cli_textbox_label);
            this.Controls.Add(this.version_ck);
            this.Controls.Add(this.output_label);
            this.Controls.Add(this.desc9);
            this.Controls.Add(this.desc8);
            this.Controls.Add(this.desc7);
            this.Controls.Add(this.desc6);
            this.Controls.Add(this.desc5);
            this.Controls.Add(this.desc4);
            this.Controls.Add(this.desc3);
            this.Controls.Add(this.discord_ck);
            this.Controls.Add(this.youtube_ck);
            this.Controls.Add(this.github_ck);
            this.Controls.Add(this.palette_rgb5a3_ck);
            this.Controls.Add(this.palette_rgb5a3_label);
            this.Controls.Add(this.palette_rgb565_ck);
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
            this.Controls.Add(this.banner_2_ck);
            this.Controls.Add(this.banner_3_ck);
            this.Controls.Add(this.banner_4_ck);
            this.Controls.Add(this.banner_6_ck);
            this.Controls.Add(this.banner_7_ck);
            this.Controls.Add(this.banner_8_ck);
            this.Controls.Add(this.banner_9_ck);
            this.Controls.Add(this.banner_minus_ck);
            this.Controls.Add(this.banner_f11_ck);
            this.Controls.Add(this.banner_x_ck);
            this.Controls.Add(this.paint_ck);
            this.Controls.Add(this.auto_ck);
            this.Controls.Add(this.preview_ck);
            this.Controls.Add(this.all_ck);
            this.Controls.Add(this.banner_ck);
            this.Controls.Add(this.view_mag_ck);
            this.Controls.Add(this.view_mag_label);
            this.Controls.Add(this.view_min_ck);
            this.Controls.Add(this.view_WrapT_ck);
            this.Controls.Add(this.view_WrapT_label);
            this.Controls.Add(this.view_WrapS_ck);
            this.Controls.Add(this.view_WrapS_label);
            this.Controls.Add(this.view_algorithm_ck);
            this.Controls.Add(this.view_algorithm_label);
            this.Controls.Add(this.view_alpha_ck);
            this.Controls.Add(this.view_alpha_label);
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
            this.Controls.Add(this.mag_linearmipmapnearest_ck);
            this.Controls.Add(this.mag_nearestmipmaplinear_ck);
            this.Controls.Add(this.mag_nearestmipmapnearest_ck);
            this.Controls.Add(this.mag_linear_ck);
            this.Controls.Add(this.mag_nearest_neighbour_ck);
            this.Controls.Add(this.min_linearmipmaplinear_ck);
            this.Controls.Add(this.min_linearmipmapnearest_ck);
            this.Controls.Add(this.min_nearestmipmaplinear_ck);
            this.Controls.Add(this.min_nearestmipmapnearest_ck);
            this.Controls.Add(this.min_linear_ck);
            this.Controls.Add(this.min_nearest_neighbour_ck);
            this.Controls.Add(this.magnification_label);
            this.Controls.Add(this.minification_label);
            this.Controls.Add(this.Smirror_ck);
            this.Controls.Add(this.Srepeat_ck);
            this.Controls.Add(this.Sclamp_ck);
            this.Controls.Add(this.WrapS_label);
            this.Controls.Add(this.Tmirror_ck);
            this.Controls.Add(this.Trepeat_ck);
            this.Controls.Add(this.Tclamp_ck);
            this.Controls.Add(this.WrapT_label);
            this.Controls.Add(this.mix_ck);
            this.Controls.Add(this.alpha_ck);
            this.Controls.Add(this.no_alpha_ck);
            this.Controls.Add(this.alpha_title);
            this.Controls.Add(this.no_gradient_ck);
            this.Controls.Add(this.custom_ck);
            this.Controls.Add(this.cie_709_ck);
            this.Controls.Add(this.cie_601_ck);
            this.Controls.Add(this.algorithm_label);
            this.Controls.Add(this.cmpr_ck);
            this.Controls.Add(this.cmpr_label);
            this.Controls.Add(this.ci14x2_ck);
            this.Controls.Add(this.ci14x2_label);
            this.Controls.Add(this.ci8_ck);
            this.Controls.Add(this.ci8_label);
            this.Controls.Add(this.ci4_ck);
            this.Controls.Add(this.ci4_label);
            this.Controls.Add(this.rgba32_ck);
            this.Controls.Add(this.rgba32_label);
            this.Controls.Add(this.rgb5a3_ck);
            this.Controls.Add(this.rgb5a3_label);
            this.Controls.Add(this.rgb565_ck);
            this.Controls.Add(this.rgb565_label);
            this.Controls.Add(this.ai8_ck);
            this.Controls.Add(this.ai8_label);
            this.Controls.Add(this.ai4_ck);
            this.Controls.Add(this.ai4_label);
            this.Controls.Add(this.i8_ck);
            this.Controls.Add(this.i8_label);
            this.Controls.Add(this.i4_ck);
            this.Controls.Add(this.i4_label);
            this.Controls.Add(this.encoding_label);
            this.Controls.Add(this.warn_ck);
            this.Controls.Add(this.warn_label);
            this.Controls.Add(this.stfu_ck);
            this.Controls.Add(this.stfu_label);
            this.Controls.Add(this.safe_mode_ck);
            this.Controls.Add(this.safe_mode_label);
            this.Controls.Add(this.reversey_ck);
            this.Controls.Add(this.reversey_label);
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
            this.Controls.Add(this.options_label);
            this.Controls.Add(this.tiff_ck);
            this.Controls.Add(this.tiff_label);
            this.Controls.Add(this.tif_ck);
            this.Controls.Add(this.tif_label);
            this.Controls.Add(this.ico_ck);
            this.Controls.Add(this.ico_label);
            this.Controls.Add(this.gif_ck);
            this.Controls.Add(this.gif_label);
            this.Controls.Add(this.jpeg_ck);
            this.Controls.Add(this.jpeg_label);
            this.Controls.Add(this.jpg_ck);
            this.Controls.Add(this.jpg_label);
            this.Controls.Add(this.png_ck);
            this.Controls.Add(this.png_label);
            this.Controls.Add(this.bmp_ck);
            this.Controls.Add(this.bmp_label);
            this.Controls.Add(this.tpl_ck);
            this.Controls.Add(this.tpl_label);
            this.Controls.Add(this.tex0_ck);
            this.Controls.Add(this.tex0_label);
            this.Controls.Add(this.bti_ck);
            this.Controls.Add(this.bti_label);
            this.Controls.Add(this.bmd_ck);
            this.Controls.Add(this.bmd_label);
            this.Controls.Add(this.output_file_type_label);
            this.Controls.Add(this.view_min_label);
            this.Controls.Add(this.desc2);
            this.Controls.Add(this.description);
            this.Controls.Add(this.description_surrounding);
            this.Controls.Add(this.banner_resize);
            this.Controls.Add(this.banner_move);
            this.Controls.Add(this.surrounding_ck);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Ableton Sans Small", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.MaximumSize = new System.Drawing.Size(99999, 99999);
            this.Name = "plt0_gui";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "PLT0 - Image Encoding tool";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.plt0_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.plt0_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmpr_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.banner_global_move_MouseDown);
            this.MouseEnter += new System.EventHandler(this.plt0_gui_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.plt0_gui_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.banner_global_move_MouseMove);
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
            ((System.ComponentModel.ISupportInitialize)(this.reversey_ck)).EndInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.banner_f11_ck)).EndInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.view_options_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textchange_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.auto_update_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sync_preview_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upscale_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview4k_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reversex_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_swap_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_block_paint_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_block_selection_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_save_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_save_as_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bmd_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.name_string_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_swap2_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_hover_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_update_preview_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_global_move_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_5_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_11_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_12_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_13_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_14_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_16_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_17_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_18_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_19_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.banner_15_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_max_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sooperbmd_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weemm_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.darkest_lightest_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_palette)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_grid_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmpr_preview_ck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.image_ck)).EndInit();
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
            if (File.Exists(execPath + "images/gradient.png"))
            {
                gradient = Image.FromFile(execPath + "images/gradient.png");
            }
            if (File.Exists(execPath + "images/surrounding.png"))
            {
                surrounding = Image.FromFile(execPath + "images/surrounding.png");
            }
            if (File.Exists(execPath + "images/check.png"))
            {
                check = Image.FromFile(execPath + "images/check.png");
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
            if (File.Exists(execPath + "images/all_hover.png"))
            {
                all_hover = Image.FromFile(execPath + "images/all_hover.png");
            }
            if (File.Exists(execPath + "images/all_off.png"))
            {
                all_off = Image.FromFile(execPath + "images/all_off.png");
            }
            if (File.Exists(execPath + "images/all_on.png"))
            {
                all_on = Image.FromFile(execPath + "images/all_on.png");
            }
            if (File.Exists(execPath + "images/all_selected.png"))
            {
                all_selected = Image.FromFile(execPath + "images/all_selected.png");
            }
            if (File.Exists(execPath + "images/auto_hover.png"))
            {
                auto_hover = Image.FromFile(execPath + "images/auto_hover.png");
            }
            if (File.Exists(execPath + "images/auto_off.png"))
            {
                auto_off = Image.FromFile(execPath + "images/auto_off.png");
            }
            if (File.Exists(execPath + "images/auto_on.png"))
            {
                auto_on = Image.FromFile(execPath + "images/auto_on.png");
            }
            if (File.Exists(execPath + "images/auto_selected.png"))
            {
                auto_selected = Image.FromFile(execPath + "images/auto_selected.png");
            }
            if (File.Exists(execPath + "images/banner_global_move_hover.png"))
            {
                banner_global_move_hover = Image.FromFile(execPath + "images/banner_global_move_hover.png");
            }
            if (File.Exists(execPath + "images/banner_global_move_off.png"))
            {
                banner_global_move_off = Image.FromFile(execPath + "images/banner_global_move_off.png");
            }
            if (File.Exists(execPath + "images/banner_global_move_on.png"))
            {
                banner_global_move_on = Image.FromFile(execPath + "images/banner_global_move_on.png");
            }
            if (File.Exists(execPath + "images/banner_global_move_selected.png"))
            {
                banner_global_move_selected = Image.FromFile(execPath + "images/banner_global_move_selected.png");
            }
            if (File.Exists(execPath + "images/cli_textbox.png"))
            {
                cli_textbox = Image.FromFile(execPath + "images/cli_textbox.png");
            }
            if (File.Exists(execPath + "images/cli_textbox_hover.png"))
            {
                cli_textbox_hover = Image.FromFile(execPath + "images/cli_textbox_hover.png");
            }
            if (File.Exists(execPath + "images/close.png"))
            {
                close = Image.FromFile(execPath + "images/close.png");
            }
            if (File.Exists(execPath + "images/close_hover.png"))
            {
                close_hover = Image.FromFile(execPath + "images/close_hover.png");
            }
            if (File.Exists(execPath + "images/cmpr_save.png"))
            {
                cmpr_save = Image.FromFile(execPath + "images/cmpr_save.png");
            }
            if (File.Exists(execPath + "images/cmpr_save_as.png"))
            {
                cmpr_save_as = Image.FromFile(execPath + "images/cmpr_save_as.png");
            }
            if (File.Exists(execPath + "images/cmpr_save_as_hover.png"))
            {
                cmpr_save_as_hover = Image.FromFile(execPath + "images/cmpr_save_as_hover.png");
            }
            if (File.Exists(execPath + "images/cmpr_save_hover.png"))
            {
                cmpr_save_hover = Image.FromFile(execPath + "images/cmpr_save_hover.png");
            }
            if (File.Exists(execPath + "images/cmpr_swap2.png"))
            {
                cmpr_swap2 = Image.FromFile(execPath + "images/cmpr_swap2.png");
            }
            if (File.Exists(execPath + "images/cmpr_swap2_hover.png"))
            {
                cmpr_swap2_hover = Image.FromFile(execPath + "images/cmpr_swap2_hover.png");
            }
            if (File.Exists(execPath + "images/cmpr_swap.png"))
            {
                cmpr_swap = Image.FromFile(execPath + "images/cmpr_swap.png");
            }
            if (File.Exists(execPath + "images/cmpr_swap_hover.png"))
            {
                cmpr_swap_hover = Image.FromFile(execPath + "images/cmpr_swap_hover.png");
            }
            if (File.Exists(execPath + "images/discord.png"))
            {
                discord = Image.FromFile(execPath + "images/discord.png");
            }
            if (File.Exists(execPath + "images/discord_hover.png"))
            {
                discord_hover = Image.FromFile(execPath + "images/discord_hover.png");
            }
            if (File.Exists(execPath + "images/github.png"))
            {
                github = Image.FromFile(execPath + "images/github.png");
            }
            if (File.Exists(execPath + "images/github_hover.png"))
            {
                github_hover = Image.FromFile(execPath + "images/github_hover.png");
            }
            if (File.Exists(execPath + "images/maximized_hover.png"))
            {
                maximized_hover = Image.FromFile(execPath + "images/maximized_hover.png");
            }
            if (File.Exists(execPath + "images/maximized_off.png"))
            {
                maximized_off = Image.FromFile(execPath + "images/maximized_off.png");
            }
            if (File.Exists(execPath + "images/maximized_on.png"))
            {
                maximized_on = Image.FromFile(execPath + "images/maximized_on.png");
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
            if (File.Exists(execPath + "images/paint_hover.png"))
            {
                paint_hover = Image.FromFile(execPath + "images/paint_hover.png");
            }
            if (File.Exists(execPath + "images/paint_off.png"))
            {
                paint_off = Image.FromFile(execPath + "images/paint_off.png");
            }
            if (File.Exists(execPath + "images/paint_on.png"))
            {
                paint_on = Image.FromFile(execPath + "images/paint_on.png");
            }
            if (File.Exists(execPath + "images/paint_selected.png"))
            {
                paint_selected = Image.FromFile(execPath + "images/paint_selected.png");
            }
            if (File.Exists(execPath + "images/preview_hover.png"))
            {
                preview_hover = Image.FromFile(execPath + "images/preview_hover.png");
            }
            if (File.Exists(execPath + "images/preview_off.png"))
            {
                preview_off = Image.FromFile(execPath + "images/preview_off.png");
            }
            if (File.Exists(execPath + "images/preview_on.png"))
            {
                preview_on = Image.FromFile(execPath + "images/preview_on.png");
            }
            if (File.Exists(execPath + "images/preview_selected.png"))
            {
                preview_selected = Image.FromFile(execPath + "images/preview_selected.png");
            }
            if (File.Exists(execPath + "images/run_hover.png"))
            {
                run_hover = Image.FromFile(execPath + "images/run_hover.png");
            }
            if (File.Exists(execPath + "images/run_off.png"))
            {
                run_off = Image.FromFile(execPath + "images/run_off.png");
            }
            if (File.Exists(execPath + "images/run_on.png"))
            {
                run_on = Image.FromFile(execPath + "images/run_on.png");
            }
            if (File.Exists(execPath + "images/sync_preview_hover.png"))
            {
                sync_preview_hover = Image.FromFile(execPath + "images/sync_preview_hover.png");
            }
            if (File.Exists(execPath + "images/sync_preview_off.png"))
            {
                sync_preview_off = Image.FromFile(execPath + "images/sync_preview_off.png");
            }
            if (File.Exists(execPath + "images/sync_preview_on.png"))
            {
                sync_preview_on = Image.FromFile(execPath + "images/sync_preview_on.png");
            }
            if (File.Exists(execPath + "images/sync_preview_selected.png"))
            {
                sync_preview_selected = Image.FromFile(execPath + "images/sync_preview_selected.png");
            }
            if (File.Exists(execPath + "images/version.png"))
            {
                version = Image.FromFile(execPath + "images/version.png");
            }
            if (File.Exists(execPath + "images/version_hover.png"))
            {
                version_hover = Image.FromFile(execPath + "images/version_hover.png");
            }
            if (File.Exists(execPath + "images/youtube.png"))
            {
                youtube = Image.FromFile(execPath + "images/youtube.png");
            }
            if (File.Exists(execPath + "images/youtube_hover.png"))
            {
                youtube_hover = Image.FromFile(execPath + "images/youtube_hover.png");
            }
            if (File.Exists(execPath + "images/bottom_hover.png"))
            {
                bottom_hover = Image.FromFile(execPath + "images/bottom_hover.png");
            }
            if (File.Exists(execPath + "images/bottom_left_hover.png"))
            {
                bottom_left_hover = Image.FromFile(execPath + "images/bottom_left_hover.png");
            }
            if (File.Exists(execPath + "images/bottom_left_off.png"))
            {
                bottom_left_off = Image.FromFile(execPath + "images/bottom_left_off.png");
            }
            if (File.Exists(execPath + "images/bottom_left_on.png"))
            {
                bottom_left_on = Image.FromFile(execPath + "images/bottom_left_on.png");
            }
            if (File.Exists(execPath + "images/bottom_left_selected.png"))
            {
                bottom_left_selected = Image.FromFile(execPath + "images/bottom_left_selected.png");
            }
            if (File.Exists(execPath + "images/bottom_off.png"))
            {
                bottom_off = Image.FromFile(execPath + "images/bottom_off.png");
            }
            if (File.Exists(execPath + "images/bottom_on.png"))
            {
                bottom_on = Image.FromFile(execPath + "images/bottom_on.png");
            }
            if (File.Exists(execPath + "images/bottom_right_hover.png"))
            {
                bottom_right_hover = Image.FromFile(execPath + "images/bottom_right_hover.png");
            }
            if (File.Exists(execPath + "images/bottom_right_off.png"))
            {
                bottom_right_off = Image.FromFile(execPath + "images/bottom_right_off.png");
            }
            if (File.Exists(execPath + "images/bottom_right_on.png"))
            {
                bottom_right_on = Image.FromFile(execPath + "images/bottom_right_on.png");
            }
            if (File.Exists(execPath + "images/bottom_right_selected.png"))
            {
                bottom_right_selected = Image.FromFile(execPath + "images/bottom_right_selected.png");
            }
            if (File.Exists(execPath + "images/bottom_selected.png"))
            {
                bottom_selected = Image.FromFile(execPath + "images/bottom_selected.png");
            }
            if (File.Exists(execPath + "images/right_hover.png"))
            {
                right_hover = Image.FromFile(execPath + "images/right_hover.png");
            }
            if (File.Exists(execPath + "images/right_off.png"))
            {
                right_off = Image.FromFile(execPath + "images/right_off.png");
            }
            if (File.Exists(execPath + "images/right_on.png"))
            {
                right_on = Image.FromFile(execPath + "images/right_on.png");
            }
            if (File.Exists(execPath + "images/right_selected.png"))
            {
                right_selected = Image.FromFile(execPath + "images/right_selected.png");
            }
            if (File.Exists(execPath + "images/left_hover.png"))
            {
                left_hover = Image.FromFile(execPath + "images/left_hover.png");
            }
            if (File.Exists(execPath + "images/left_off.png"))
            {
                left_off = Image.FromFile(execPath + "images/left_off.png");
            }
            if (File.Exists(execPath + "images/left_on.png"))
            {
                left_on = Image.FromFile(execPath + "images/left_on.png");
            }
            if (File.Exists(execPath + "images/left_selected.png"))
            {
                left_selected = Image.FromFile(execPath + "images/left_selected.png");
            }
            if (File.Exists(execPath + "images/top_hover.png"))
            {
                top_hover = Image.FromFile(execPath + "images/top_hover.png");
            }
            if (File.Exists(execPath + "images/top_left_hover.png"))
            {
                top_left_hover = Image.FromFile(execPath + "images/top_left_hover.png");
            }
            if (File.Exists(execPath + "images/top_left_off.png"))
            {
                top_left_off = Image.FromFile(execPath + "images/top_left_off.png");
            }
            if (File.Exists(execPath + "images/top_left_on.png"))
            {
                top_left_on = Image.FromFile(execPath + "images/top_left_on.png");
            }
            if (File.Exists(execPath + "images/top_left_selected.png"))
            {
                top_left_selected = Image.FromFile(execPath + "images/top_left_selected.png");
            }
            if (File.Exists(execPath + "images/top_off.png"))
            {
                top_off = Image.FromFile(execPath + "images/top_off.png");
            }
            if (File.Exists(execPath + "images/top_on.png"))
            {
                top_on = Image.FromFile(execPath + "images/top_on.png");
            }
            if (File.Exists(execPath + "images/top_right_hover.png"))
            {
                top_right_hover = Image.FromFile(execPath + "images/top_right_hover.png");
            }
            if (File.Exists(execPath + "images/top_right_off.png"))
            {
                top_right_off = Image.FromFile(execPath + "images/top_right_off.png");
            }
            if (File.Exists(execPath + "images/top_right_on.png"))
            {
                top_right_on = Image.FromFile(execPath + "images/top_right_on.png");
            }
            if (File.Exists(execPath + "images/top_right_selected.png"))
            {
                top_right_selected = Image.FromFile(execPath + "images/top_right_selected.png");
            }
            if (File.Exists(execPath + "images/top_selected.png"))
            {
                top_selected = Image.FromFile(execPath + "images/top_selected.png");
            }
            if (File.Exists(execPath + "images/screen2_bottom_hover.png"))
            {
                screen2_bottom_hover = Image.FromFile(execPath + "images/screen2_bottom_hover.png");
            }
            if (File.Exists(execPath + "images/screen2_bottom_left_hover.png"))
            {
                screen2_bottom_left_hover = Image.FromFile(execPath + "images/screen2_bottom_left_hover.png");
            }
            if (File.Exists(execPath + "images/screen2_bottom_left_off.png"))
            {
                screen2_bottom_left_off = Image.FromFile(execPath + "images/screen2_bottom_left_off.png");
            }
            if (File.Exists(execPath + "images/screen2_bottom_left_on.png"))
            {
                screen2_bottom_left_on = Image.FromFile(execPath + "images/screen2_bottom_left_on.png");
            }
            if (File.Exists(execPath + "images/screen2_bottom_left_selected.png"))
            {
                screen2_bottom_left_selected = Image.FromFile(execPath + "images/screen2_bottom_left_selected.png");
            }
            if (File.Exists(execPath + "images/screen2_bottom_off.png"))
            {
                screen2_bottom_off = Image.FromFile(execPath + "images/screen2_bottom_off.png");
            }
            if (File.Exists(execPath + "images/screen2_bottom_on.png"))
            {
                screen2_bottom_on = Image.FromFile(execPath + "images/screen2_bottom_on.png");
            }
            if (File.Exists(execPath + "images/screen2_bottom_right_hover.png"))
            {
                screen2_bottom_right_hover = Image.FromFile(execPath + "images/screen2_bottom_right_hover.png");
            }
            if (File.Exists(execPath + "images/screen2_bottom_right_off.png"))
            {
                screen2_bottom_right_off = Image.FromFile(execPath + "images/screen2_bottom_right_off.png");
            }
            if (File.Exists(execPath + "images/screen2_bottom_right_on.png"))
            {
                screen2_bottom_right_on = Image.FromFile(execPath + "images/screen2_bottom_right_on.png");
            }
            if (File.Exists(execPath + "images/screen2_bottom_right_selected.png"))
            {
                screen2_bottom_right_selected = Image.FromFile(execPath + "images/screen2_bottom_right_selected.png");
            }
            if (File.Exists(execPath + "images/screen2_bottom_selected.png"))
            {
                screen2_bottom_selected = Image.FromFile(execPath + "images/screen2_bottom_selected.png");
            }
            if (File.Exists(execPath + "images/screen2_right_hover.png"))
            {
                screen2_right_hover = Image.FromFile(execPath + "images/screen2_right_hover.png");
            }
            if (File.Exists(execPath + "images/screen2_right_off.png"))
            {
                screen2_right_off = Image.FromFile(execPath + "images/screen2_right_off.png");
            }
            if (File.Exists(execPath + "images/screen2_right_on.png"))
            {
                screen2_right_on = Image.FromFile(execPath + "images/screen2_right_on.png");
            }
            if (File.Exists(execPath + "images/screen2_right_selected.png"))
            {
                screen2_right_selected = Image.FromFile(execPath + "images/screen2_right_selected.png");
            }
            if (File.Exists(execPath + "images/screen2_left_hover.png"))
            {
                screen2_left_hover = Image.FromFile(execPath + "images/screen2_left_hover.png");
            }
            if (File.Exists(execPath + "images/screen2_left_off.png"))
            {
                screen2_left_off = Image.FromFile(execPath + "images/screen2_left_off.png");
            }
            if (File.Exists(execPath + "images/screen2_left_on.png"))
            {
                screen2_left_on = Image.FromFile(execPath + "images/screen2_left_on.png");
            }
            if (File.Exists(execPath + "images/screen2_left_selected.png"))
            {
                screen2_left_selected = Image.FromFile(execPath + "images/screen2_left_selected.png");
            }
            if (File.Exists(execPath + "images/screen2_top_hover.png"))
            {
                screen2_top_hover = Image.FromFile(execPath + "images/screen2_top_hover.png");
            }
            if (File.Exists(execPath + "images/screen2_top_left_hover.png"))
            {
                screen2_top_left_hover = Image.FromFile(execPath + "images/screen2_top_left_hover.png");
            }
            if (File.Exists(execPath + "images/screen2_top_left_off.png"))
            {
                screen2_top_left_off = Image.FromFile(execPath + "images/screen2_top_left_off.png");
            }
            if (File.Exists(execPath + "images/screen2_top_left_on.png"))
            {
                screen2_top_left_on = Image.FromFile(execPath + "images/screen2_top_left_on.png");
            }
            if (File.Exists(execPath + "images/screen2_top_left_selected.png"))
            {
                screen2_top_left_selected = Image.FromFile(execPath + "images/screen2_top_left_selected.png");
            }
            if (File.Exists(execPath + "images/screen2_top_off.png"))
            {
                screen2_top_off = Image.FromFile(execPath + "images/screen2_top_off.png");
            }
            if (File.Exists(execPath + "images/screen2_top_on.png"))
            {
                screen2_top_on = Image.FromFile(execPath + "images/screen2_top_on.png");
            }
            if (File.Exists(execPath + "images/screen2_top_right_hover.png"))
            {
                screen2_top_right_hover = Image.FromFile(execPath + "images/screen2_top_right_hover.png");
            }
            if (File.Exists(execPath + "images/screen2_top_right_off.png"))
            {
                screen2_top_right_off = Image.FromFile(execPath + "images/screen2_top_right_off.png");
            }
            if (File.Exists(execPath + "images/screen2_top_right_on.png"))
            {
                screen2_top_right_on = Image.FromFile(execPath + "images/screen2_top_right_on.png");
            }
            if (File.Exists(execPath + "images/screen2_top_right_selected.png"))
            {
                screen2_top_right_selected = Image.FromFile(execPath + "images/screen2_top_right_selected.png");
            }
            if (File.Exists(execPath + "images/screen2_arrow_1080p_hover.png"))
            {
                screen2_arrow_1080p_hover = Image.FromFile(execPath + "images/screen2_arrow_1080p_hover.png");
            }
            if (File.Exists(execPath + "images/screen2_arrow_1080p_off.png"))
            {
                screen2_arrow_1080p_off = Image.FromFile(execPath + "images/screen2_arrow_1080p_off.png");
            }
            if (File.Exists(execPath + "images/screen2_arrow_1080p_on.png"))
            {
                screen2_arrow_1080p_on = Image.FromFile(execPath + "images/screen2_arrow_1080p_on.png");
            }
            if (File.Exists(execPath + "images/screen2_arrow_1080p_selected.png"))
            {
                screen2_arrow_1080p_selected = Image.FromFile(execPath + "images/screen2_arrow_1080p_selected.png");
            }
            if (File.Exists(execPath + "images/arrow_1080p_hover.png"))
            {
                arrow_1080p_hover = Image.FromFile(execPath + "images/arrow_1080p_hover.png");
            }
            if (File.Exists(execPath + "images/arrow_1080p_off.png"))
            {
                arrow_1080p_off = Image.FromFile(execPath + "images/arrow_1080p_off.png");
            }
            if (File.Exists(execPath + "images/arrow_1080p_on.png"))
            {
                arrow_1080p_on = Image.FromFile(execPath + "images/arrow_1080p_on.png");
            }
            if (File.Exists(execPath + "images/arrow_1080p_selected.png"))
            {
                arrow_1080p_selected = Image.FromFile(execPath + "images/arrow_1080p_selected.png");
            }
            if (File.Exists(execPath + "images/screen2_top_selected.png"))
            {
                screen2_top_selected = Image.FromFile(execPath + "images/screen2_top_selected.png");
            }
        }
        private void bmd_Click(object sender, EventArgs e)
        {
            if (bmd)
            {
                bmd = false;
                Organize_args();
                hover_checkbox(bmd_ck);
            }
            else
            {
                bmd = true;
                Organize_args();
                selected_checkbox(bmd_ck);
            }
            Check_run();
        }
        private void bmd_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[119]);
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
                Organize_args();
                hover_checkbox(bti_ck);
                Hide_WrapS();
                Hide_WrapT();
                Hide_min();
                Hide_mag();
            }
            else
            {
                bti = true;
                Organize_args();
                selected_checkbox(bti_ck);
                View_WrapS();
                View_WrapT();
                View_min();
                View_mag();
            }
            Check_run();
        }
        private void bti_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[120]);
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
                Organize_args();
                hover_checkbox(tex0_ck);
            }
            else
            {
                tex0 = true;
                Organize_args();
                selected_checkbox(tex0_ck);
            }
            Check_run();
        }
        private void tex0_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[121]);
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
                Organize_args();
                hover_checkbox(tpl_ck);
                Hide_WrapS();
                Hide_WrapT();
                Hide_min();
                Hide_mag();
            }
            else
            {
                tpl = true;
                Organize_args();
                selected_checkbox(tpl_ck);
                View_WrapS();
                View_WrapT();
                View_min();
                View_mag();
            }
            Check_run();
        }
        private void tpl_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[122]);
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
                Organize_args();
                hover_checkbox(bmp_ck);
            }
            else
            {
                bmp = true;
                Organize_args();
                selected_checkbox(bmp_ck);
            }
            Check_run();
        }
        private void bmp_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[123]);
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
                Organize_args();
                hover_checkbox(png_ck);
            }
            else
            {
                png = true;
                Organize_args();
                selected_checkbox(png_ck);
            }
            Check_run();
        }
        private void png_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[124]);
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
                Organize_args();
                hover_checkbox(jpg_ck);
            }
            else
            {
                jpg = true;
                Organize_args();
                selected_checkbox(jpg_ck);
            }
            Check_run();
        }
        private void jpg_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[125]);
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
                Organize_args();
                hover_checkbox(jpeg_ck);
            }
            else
            {
                jpeg = true;
                Organize_args();
                selected_checkbox(jpeg_ck);
            }
            Check_run();
        }
        private void jpeg_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[126]);
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
                Organize_args();
                hover_checkbox(gif_ck);
            }
            else
            {
                gif = true;
                Organize_args();
                selected_checkbox(gif_ck);
            }
            Check_run();
        }
        private void gif_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[127]);
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
                Organize_args();
                hover_checkbox(ico_ck);
            }
            else
            {
                ico = true;
                Organize_args();
                selected_checkbox(ico_ck);
            }
            Check_run();
        }
        private void ico_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[128]);
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
                Organize_args();
                hover_checkbox(tif_ck);
            }
            else
            {
                tif = true;
                Organize_args();
                selected_checkbox(tif_ck);
            }
            Check_run();
        }
        private void tif_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[129]);
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
                Organize_args();
                hover_checkbox(tiff_ck);
            }
            else
            {
                tiff = true;
                Organize_args();
                selected_checkbox(tiff_ck);
            }
            Check_run();
        }
        private void tiff_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[130]);
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
                Organize_args();
                hover_checkbox(ask_exit_ck);
            }
            else
            {
                ask_exit = true;
                Organize_args();
                selected_checkbox(ask_exit_ck);
            }
        }
        private void ask_exit_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[131]);
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
                Organize_args();
                hover_checkbox(bmp_32_ck);
            }
            else
            {
                bmp_32 = true;
                Organize_args();
                selected_checkbox(bmp_32_ck);
            }
        }
        private void bmp_32_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[132]);
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
                Organize_args();
                hover_checkbox(FORCE_ALPHA_ck);
                Preview(false);
            }
            else
            {
                FORCE_ALPHA = true;
                Organize_args();
                selected_checkbox(FORCE_ALPHA_ck);
                Preview(false);
            }
        }
        private void FORCE_ALPHA_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[133]);
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
                Organize_args();
                hover_checkbox(funky_ck);
                Preview(false);
            }
            else
            {
                funky = true;
                Organize_args();
                selected_checkbox(funky_ck);
                Preview(false);
            }
        }
        private void funky_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[134]);
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
        private void name_string_Click(object sender, EventArgs e)
        {
            if (name_string)
            {
                name_string = false;
                Organize_args();
                hover_checkbox(name_string_ck);
                Preview(false);
            }
            else
            {
                name_string = true;
                Organize_args();
                selected_checkbox(name_string_ck);
                Preview(false);
            }
        }
        private void name_string_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[135]);
            if (name_string)
                selected_checkbox(name_string_ck);
            else
                hover_checkbox(name_string_ck);
        }
        private void name_string_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (name_string)
                checked_checkbox(name_string_ck);
            else
                unchecked_checkbox(name_string_ck);
        }
        private void no_warning_Click(object sender, EventArgs e)
        {
            if (no_warning)
            {
                no_warning = false;
                Organize_args();
                hover_checkbox(no_warning_ck);
                Preview(false);
            }
            else
            {
                no_warning = true;
                Organize_args();
                selected_checkbox(no_warning_ck);
                Preview(false);
            }
        }
        private void no_warning_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[136]);
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
                Organize_args();
                hover_checkbox(random_ck);
                Preview(false);
            }
            else
            {
                random = true;
                Organize_args();
                selected_checkbox(random_ck);
                Preview(false);
            }
        }
        private void random_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[137]);
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
        private void reversex_Click(object sender, EventArgs e)
        {
            if (reversex)
            {
                reversex = false;
                Organize_args();
                hover_checkbox(reversex_ck);
                Preview(false);
            }
            else
            {
                reversex = true;
                Organize_args();
                selected_checkbox(reversex_ck);
                Preview(false);
            }
        }
        private void reversex_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[138]);
            if (reversex)
                selected_checkbox(reversex_ck);
            else
                hover_checkbox(reversex_ck);
        }
        private void reversex_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (reversex)
                checked_checkbox(reversex_ck);
            else
                unchecked_checkbox(reversex_ck);
        }
        private void reversey_Click(object sender, EventArgs e)
        {
            if (reversey)
            {
                reversey = false;
                Organize_args();
                hover_checkbox(reversey_ck);
                Preview(false);
            }
            else
            {
                reversey = true;
                Organize_args();
                selected_checkbox(reversey_ck);
                Preview(false);
            }
        }
        private void reversey_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[139]);
            if (reversey)
                selected_checkbox(reversey_ck);
            else
                hover_checkbox(reversey_ck);
        }
        private void reversey_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (reversey)
                checked_checkbox(reversey_ck);
            else
                unchecked_checkbox(reversey_ck);
        }
        private void safe_mode_Click(object sender, EventArgs e)
        {
            if (safe_mode)
            {
                safe_mode = false;
                Organize_args();
                hover_checkbox(safe_mode_ck);
            }
            else
            {
                safe_mode = true;
                Organize_args();
                selected_checkbox(safe_mode_ck);
            }
        }
        private void safe_mode_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[140]);
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
                Organize_args();
                hover_checkbox(stfu_ck);
            }
            else
            {
                stfu = true;
                Organize_args();
                selected_checkbox(stfu_ck);
            }
        }
        private void stfu_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[141]);
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
                Organize_args();
                hover_checkbox(warn_ck);
            }
            else
            {
                warn = true;
                Organize_args();
                selected_checkbox(warn_ck);
            }
        }
        private void warn_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[142]);
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
        private void cmpr_hover_Click(object sender, EventArgs e)
        {
            if (cmpr_hover)
            {
                cmpr_hover = false;
                hover_checkbox(cmpr_hover_ck);
                cmpr_hover_colour.Visible = false;
                cmpr_hover_colour_label.Visible = false;
                cmpr_hover_colour_txt.Visible = false;
            }
            else
            {
                cmpr_hover = true;
                selected_checkbox(cmpr_hover_ck);
                cmpr_hover_colour.Visible = true;
                cmpr_hover_colour_label.Visible = true;
                cmpr_hover_colour_txt.Visible = true;
            }
        }
        private void cmpr_hover_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[143]);
            if (cmpr_hover)
                selected_checkbox(cmpr_hover_ck);
            else
                hover_checkbox(cmpr_hover_ck);
        }
        private void cmpr_hover_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (cmpr_hover)
                checked_checkbox(cmpr_hover_ck);
            else
                unchecked_checkbox(cmpr_hover_ck);
        }
        private void cmpr_update_preview_Click(object sender, EventArgs e)
        {
            if (cmpr_update_preview)
            {
                cmpr_update_preview = false;
                hover_checkbox(cmpr_update_preview_ck);
            }
            else
            {
                cmpr_update_preview = true;
                selected_checkbox(cmpr_update_preview_ck);
                Preview_Paint();
            }
        }
        private void cmpr_update_preview_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[144]);
            if (cmpr_update_preview)
                selected_checkbox(cmpr_update_preview_ck);
            else
                hover_checkbox(cmpr_update_preview_ck);
        }
        private void cmpr_update_preview_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (cmpr_update_preview)
                checked_checkbox(cmpr_update_preview_ck);
            else
                unchecked_checkbox(cmpr_update_preview_ck);
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
            Preview(false);
        }
        private void I4_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[145]);
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
            Preview(false);
        }
        private void I8_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[146]);
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
            Preview(false);
        }
        private void AI4_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[147]);
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
            Preview(false);
        }
        private void AI8_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[148]);
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
            Preview(false);
        }
        private void RGB565_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[149]);
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
            Preview(false);
        }
        private void RGB5A3_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[150]);
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
            Preview(false);
        }
        private void RGBA32_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[151]);
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
            Preview(false);
        }
        private void CI4_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[152]);
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
            Preview(false);
        }
        private void CI8_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[153]);
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
            Preview(false);
        }
        private void CI14X2_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[154]);
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
            Preview(false);
        }
        private void CMPR_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[155]);
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
            Preview(false);
        }
        private void Cie_601_MouseEnter(object sender, EventArgs e)
        {

            if (encoding == 14)
                Parse_Markdown(config[156]);
            else
                Parse_Markdown(config[159]);
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
            Preview(false);
        }
        private void Cie_709_MouseEnter(object sender, EventArgs e)
        {

            if (encoding == 14)
                Parse_Markdown(config[157]);
            else
                Parse_Markdown(config[160]);
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
            View_rgba();
            Preview(false);
        }
        private void Custom_MouseEnter(object sender, EventArgs e)
        {

            if (encoding == 14)
                Parse_Markdown(config[158]);
            else
                Parse_Markdown(config[161]);
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
        private void Darkest_Lightest_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            Hide_algorithm(algorithm);
            selected_algorithm(darkest_lightest_ck);
            algorithm = 3; // Darkest_Lightest
            Organize_args();
            Preview(false);
        }
        private void Darkest_Lightest_MouseEnter(object sender, EventArgs e)
        {

            if (encoding == 14)
                Parse_Markdown(config[159]);
            else
                Parse_Markdown(config[162]);
            if (algorithm == 3)
                selected_algorithm(darkest_lightest_ck);
            else
                hover_algorithm(darkest_lightest_ck);
        }
        private void Darkest_Lightest_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (algorithm == 3)
                checked_algorithm(darkest_lightest_ck);
            else
                unchecked_algorithm(darkest_lightest_ck);
        }
        private void No_gradient_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            Hide_algorithm(algorithm);
            selected_algorithm(no_gradient_ck);
            algorithm = 4; // No_gradient
            Organize_args();
            View_No_Gradient();
            Preview(false);
        }
        private void No_gradient_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[163]);
            if (algorithm == 4)
                selected_algorithm(no_gradient_ck);
            else
                hover_algorithm(no_gradient_ck);
        }
        private void No_gradient_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (algorithm == 4)
                checked_algorithm(no_gradient_ck);
            else
                unchecked_algorithm(no_gradient_ck);
        }
        private void Weemm_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            Hide_algorithm(algorithm);
            selected_algorithm(weemm_ck);
            algorithm = 5; // Weemm
            Organize_args();
            Preview(false);
        }
        private void Weemm_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[164]);
            if (algorithm == 5)
                selected_algorithm(weemm_ck);
            else
                hover_algorithm(weemm_ck);
        }
        private void Weemm_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (algorithm == 5)
                checked_algorithm(weemm_ck);
            else
                unchecked_algorithm(weemm_ck);
        }
        private void SooperBMD_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            Hide_algorithm(algorithm);
            selected_algorithm(sooperbmd_ck);
            algorithm = 6; // SooperBMD
            Organize_args();
            Preview(false);
        }
        private void SooperBMD_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[165]);
            if (algorithm == 6)
                selected_algorithm(sooperbmd_ck);
            else
                hover_algorithm(sooperbmd_ck);
        }
        private void SooperBMD_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (algorithm == 6)
                checked_algorithm(sooperbmd_ck);
            else
                unchecked_algorithm(sooperbmd_ck);
        }
        private void Min_Max_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            Hide_algorithm(algorithm);
            selected_algorithm(min_max_ck);
            algorithm = 7; // Min_Max
            Organize_args();
            Preview(false);
        }
        private void Min_Max_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[166]);
            if (algorithm == 7)
                selected_algorithm(min_max_ck);
            else
                hover_algorithm(min_max_ck);
        }
        private void Min_Max_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (algorithm == 7)
                checked_algorithm(min_max_ck);
            else
                unchecked_algorithm(min_max_ck);
        }
        private void No_alpha_Click(object sender, EventArgs e)
        {
            unchecked_alpha(alpha_ck_array[alpha]);
            selected_alpha(no_alpha_ck);
            alpha = 0; // No_alpha
            Organize_args();
            Preview(false);
        }
        private void No_alpha_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[167]);
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
            Preview(false);
        }
        private void Alpha_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[168]);
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
            Preview(false);
        }
        private void Mix_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[169]);
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
            Parse_Markdown(config[170]);
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
            Parse_Markdown(config[171]);
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
            Parse_Markdown(config[172]);
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
            Parse_Markdown(config[173]);
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
            Parse_Markdown(config[174]);
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
            Parse_Markdown(config[175]);
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
            Parse_Markdown(config[176]);
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
            Parse_Markdown(config[177]);
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
            Parse_Markdown(config[178]);
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
            Parse_Markdown(config[179]);
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
            Parse_Markdown(config[180]);
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
            Parse_Markdown(config[181]);
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
            Parse_Markdown(config[182]);
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
            Parse_Markdown(config[183]);
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
            Parse_Markdown(config[184]);
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
            Parse_Markdown(config[185]);
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
            Parse_Markdown(config[186]);
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
            Parse_Markdown(config[187]);
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
            Preview(false);
        }
        private void R_R_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[188]);
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
            Preview(false);
        }
        private void R_G_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[188]);
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
            Preview(false);
        }
        private void R_B_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[188]);
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
            Preview(false);
        }
        private void R_A_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[188]);
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
            Preview(false);
        }
        private void G_R_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[189]);
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
            Preview(false);
        }
        private void G_G_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[189]);
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
            Preview(false);
        }
        private void G_B_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[189]);
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
            Preview(false);
        }
        private void G_A_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[189]);
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
            Preview(false);
        }
        private void B_R_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[190]);
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
            Preview(false);
        }
        private void B_G_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[190]);
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
            Preview(false);
        }
        private void B_B_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[190]);
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
            Preview(false);
        }
        private void B_A_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[190]);
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
            Preview(false);
        }
        private void A_R_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[191]);
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
            Preview(false);
        }
        private void A_G_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[191]);
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
            Preview(false);
        }
        private void A_B_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[191]);
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
            Preview(false);
        }
        private void A_A_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[191]);
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
            Layout_All();
        }
        private void All_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[192]);
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
            Layout_Auto();
        }
        private void Auto_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[193]);
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
            Layout_Preview();
        }
        private void Preview_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[194]);
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
            Layout_Paint();
        }
        private void Paint_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[195]);
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
        private void banner_global_move_Click(object sender, EventArgs e)
        {
            if (banner_global_move)
            {
                banner_global_move = false;
                banner_global_move_ck.BackgroundImage = banner_global_move_hover;
            }
            else
            {
                banner_global_move = true;
                banner_global_move_ck.BackgroundImage = banner_global_move_selected;
            }
        }
        private void Minimized_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void Minimized_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[196]);
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
                banner_f11_ck.BackgroundImage = maximized_hover;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                banner_f11_ck.BackgroundImage = maximized_selected;
            }
        }
        private void Maximized_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[197]);
            if (this.WindowState == FormWindowState.Maximized)
                banner_f11_ck.BackgroundImage = maximized_selected;
            else
                banner_f11_ck.BackgroundImage = maximized_hover;
        }
        private void Maximized_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (this.WindowState == FormWindowState.Maximized)
                banner_f11_ck.BackgroundImage = maximized_on;
            else
                banner_f11_ck.BackgroundImage = maximized_off;
        }
        private void Close_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void Close_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[198]);
            banner_x_ck.BackgroundImage = close_hover;
        }
        private void Close_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            banner_x_ck.BackgroundImage = close;
        }
        private void Left_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Left();
            arrow = 4;
        }
        private void Left_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[199]);
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
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X == 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height);
                    this.Location = Screen.AllScreens[i].Bounds.Location;
                }
            }
            banner_4_ck.BackgroundImage = left_selected;
        }
        private void Top_left_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Top_left();
            arrow = 7;
        }
        private void Top_left_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[200]);
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
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X == 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height / 2);
                    this.Location = Screen.AllScreens[i].Bounds.Location;
                }
            }
            banner_7_ck.BackgroundImage = top_left_selected;
        }
        private void Top_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Top();
            arrow = 8;
        }
        private void Top_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[201]);
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
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X == 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width, Screen.AllScreens[i].Bounds.Height / 2);
                    this.Location = Screen.AllScreens[i].Bounds.Location;
                }
            }
            banner_8_ck.BackgroundImage = top_selected;
        }
        private void Top_right_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Top_right();
            arrow = 9;
        }
        private void Top_right_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[202]);
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
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X == 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height / 2);
                    this.Location = new Point(Screen.AllScreens[i].Bounds.X + Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Y);
                }
            }
            banner_9_ck.BackgroundImage = top_right_selected;
        }
        private void Right_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Right();
            arrow = 6;
        }
        private void Right_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[203]);
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
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X == 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height);
                    this.Location = new Point(Screen.AllScreens[i].Bounds.X + Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Y);
                }
            }
            banner_6_ck.BackgroundImage = right_selected;
        }
        private void Bottom_right_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Bottom_right();
            arrow = 3;
        }
        private void Bottom_right_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[204]);
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
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X == 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height / 2);
                    this.Location = new Point(Screen.AllScreens[i].Bounds.X + Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Y + Screen.AllScreens[i].Bounds.Height / 2);
                }
            }
            banner_3_ck.BackgroundImage = bottom_right_selected;
        }
        private void Bottom_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Bottom();
            arrow = 2;
        }
        private void Bottom_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[205]);
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
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X == 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width, Screen.AllScreens[i].Bounds.Height / 2);
                    this.Location = new Point(Screen.AllScreens[i].Bounds.X, Screen.AllScreens[i].Bounds.Y + Screen.AllScreens[i].Bounds.Height / 2);
                }
            }
            banner_2_ck.BackgroundImage = bottom_selected;
        }
        private void Bottom_left_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Bottom_left();
            arrow = 1;
        }
        private void Bottom_left_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[206]);
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
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X == 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height / 2);
                    this.Location = new Point(Screen.AllScreens[i].Bounds.X, Screen.AllScreens[i].Bounds.Y + Screen.AllScreens[i].Bounds.Height / 2);
                }
            }
            banner_1_ck.BackgroundImage = bottom_left_selected;
        }
        private void Arrow_1080p_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Arrow_1080p();
            arrow = 5;
        }
        private void Arrow_1080p_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[207]);
            if (arrow == 5)
                selected_Arrow_1080p();
            else
                hover_Arrow_1080p();
        }
        private void Arrow_1080p_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 5)
                checked_Arrow_1080p();
            else
                unchecked_Arrow_1080p();
        }
        private void checked_Arrow_1080p()
        {
            banner_5_ck.BackgroundImage = arrow_1080p_on;
        }
        private void unchecked_Arrow_1080p()
        {
            banner_5_ck.BackgroundImage = arrow_1080p_off;
        }
        private void hover_Arrow_1080p()
        {
            banner_5_ck.BackgroundImage = arrow_1080p_hover;
        }
        private void selected_Arrow_1080p()
        {
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X == 0)
                {
                    this.Size = new Size(1920, 1080);
                    this.Location = Screen.AllScreens[i].Bounds.Location;
                }
            }
            banner_5_ck.BackgroundImage = arrow_1080p_selected;
        }
        private void Screen2_Left_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Screen2_Left();
            arrow = 14;
        }
        private void Screen2_Left_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[208]);
            if (arrow == 14)
                selected_Screen2_Left();
            else
                hover_Screen2_Left();
        }
        private void Screen2_Left_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 14)
                checked_Screen2_Left();
            else
                unchecked_Screen2_Left();
        }
        private void checked_Screen2_Left()
        {
            banner_14_ck.BackgroundImage = screen2_left_on;
        }
        private void unchecked_Screen2_Left()
        {
            banner_14_ck.BackgroundImage = screen2_left_off;
        }
        private void hover_Screen2_Left()
        {
            banner_14_ck.BackgroundImage = screen2_left_hover;
        }
        private void selected_Screen2_Left()
        {
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X != 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height);
                    this.Location = Screen.AllScreens[i].Bounds.Location;
                }
            }
            banner_14_ck.BackgroundImage = screen2_left_selected;
        }
        private void Screen2_Top_left_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Screen2_Top_left();
            arrow = 17;
        }
        private void Screen2_Top_left_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[209]);
            if (arrow == 17)
                selected_Screen2_Top_left();
            else
                hover_Screen2_Top_left();
        }
        private void Screen2_Top_left_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 17)
                checked_Screen2_Top_left();
            else
                unchecked_Screen2_Top_left();
        }
        private void checked_Screen2_Top_left()
        {
            banner_17_ck.BackgroundImage = screen2_top_left_on;
        }
        private void unchecked_Screen2_Top_left()
        {
            banner_17_ck.BackgroundImage = screen2_top_left_off;
        }
        private void hover_Screen2_Top_left()
        {
            banner_17_ck.BackgroundImage = screen2_top_left_hover;
        }
        private void selected_Screen2_Top_left()
        {
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X != 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height / 2);
                    this.Location = Screen.AllScreens[i].Bounds.Location;
                }
            }
            banner_17_ck.BackgroundImage = screen2_top_left_selected;
        }
        private void Screen2_Top_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Screen2_Top();
            arrow = 18;
        }
        private void Screen2_Top_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[210]);
            if (arrow == 18)
                selected_Screen2_Top();
            else
                hover_Screen2_Top();
        }
        private void Screen2_Top_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 18)
                checked_Screen2_Top();
            else
                unchecked_Screen2_Top();
        }
        private void checked_Screen2_Top()
        {
            banner_18_ck.BackgroundImage = screen2_top_on;
        }
        private void unchecked_Screen2_Top()
        {
            banner_18_ck.BackgroundImage = screen2_top_off;
        }
        private void hover_Screen2_Top()
        {
            banner_18_ck.BackgroundImage = screen2_top_hover;
        }
        private void selected_Screen2_Top()
        {
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X != 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width, Screen.AllScreens[i].Bounds.Height / 2);
                    this.Location = Screen.AllScreens[i].Bounds.Location;
                }
            }
            banner_18_ck.BackgroundImage = screen2_top_selected;
        }
        private void Screen2_Top_right_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Screen2_Top_right();
            arrow = 19;
        }
        private void Screen2_Top_right_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[211]);
            if (arrow == 19)
                selected_Screen2_Top_right();
            else
                hover_Screen2_Top_right();
        }
        private void Screen2_Top_right_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 19)
                checked_Screen2_Top_right();
            else
                unchecked_Screen2_Top_right();
        }
        private void checked_Screen2_Top_right()
        {
            banner_19_ck.BackgroundImage = screen2_top_right_on;
        }
        private void unchecked_Screen2_Top_right()
        {
            banner_19_ck.BackgroundImage = screen2_top_right_off;
        }
        private void hover_Screen2_Top_right()
        {
            banner_19_ck.BackgroundImage = screen2_top_right_hover;
        }
        private void selected_Screen2_Top_right()
        {
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X != 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height / 2);
                    this.Location = new Point(Screen.AllScreens[i].Bounds.X + Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Y);
                }
            }
            banner_19_ck.BackgroundImage = screen2_top_right_selected;
        }
        private void Screen2_Right_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Screen2_Right();
            arrow = 16;
        }
        private void Screen2_Right_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[212]);
            if (arrow == 16)
                selected_Screen2_Right();
            else
                hover_Screen2_Right();
        }
        private void Screen2_Right_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 16)
                checked_Screen2_Right();
            else
                unchecked_Screen2_Right();
        }
        private void checked_Screen2_Right()
        {
            banner_16_ck.BackgroundImage = screen2_right_on;
        }
        private void unchecked_Screen2_Right()
        {
            banner_16_ck.BackgroundImage = screen2_right_off;
        }
        private void hover_Screen2_Right()
        {
            banner_16_ck.BackgroundImage = screen2_right_hover;
        }
        private void selected_Screen2_Right()
        {
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X != 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height);
                    this.Location = new Point(Screen.AllScreens[i].Bounds.X + Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Y);
                }
            }
            banner_16_ck.BackgroundImage = screen2_right_selected;
        }
        private void Screen2_Bottom_right_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Screen2_Bottom_right();
            arrow = 13;
        }
        private void Screen2_Bottom_right_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[213]);
            if (arrow == 13)
                selected_Screen2_Bottom_right();
            else
                hover_Screen2_Bottom_right();
        }
        private void Screen2_Bottom_right_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 13)
                checked_Screen2_Bottom_right();
            else
                unchecked_Screen2_Bottom_right();
        }
        private void checked_Screen2_Bottom_right()
        {
            banner_13_ck.BackgroundImage = screen2_bottom_right_on;
        }
        private void unchecked_Screen2_Bottom_right()
        {
            banner_13_ck.BackgroundImage = screen2_bottom_right_off;
        }
        private void hover_Screen2_Bottom_right()
        {
            banner_13_ck.BackgroundImage = screen2_bottom_right_hover;
        }
        private void selected_Screen2_Bottom_right()
        {
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X != 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height / 2);
                    this.Location = new Point(Screen.AllScreens[i].Bounds.X + Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Y + Screen.AllScreens[i].Bounds.Height / 2);
                }
            }
            banner_13_ck.BackgroundImage = screen2_bottom_right_selected;
        }
        private void Screen2_Bottom_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Screen2_Bottom();
            arrow = 12;
        }
        private void Screen2_Bottom_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[214]);
            if (arrow == 12)
                selected_Screen2_Bottom();
            else
                hover_Screen2_Bottom();
        }
        private void Screen2_Bottom_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 12)
                checked_Screen2_Bottom();
            else
                unchecked_Screen2_Bottom();
        }
        private void checked_Screen2_Bottom()
        {
            banner_12_ck.BackgroundImage = screen2_bottom_on;
        }
        private void unchecked_Screen2_Bottom()
        {
            banner_12_ck.BackgroundImage = screen2_bottom_off;
        }
        private void hover_Screen2_Bottom()
        {
            banner_12_ck.BackgroundImage = screen2_bottom_hover;
        }
        private void selected_Screen2_Bottom()
        {
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X != 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width, Screen.AllScreens[i].Bounds.Height / 2);
                    this.Location = new Point(Screen.AllScreens[i].Bounds.X, Screen.AllScreens[i].Bounds.Y + Screen.AllScreens[i].Bounds.Height / 2);
                }
            }
            banner_12_ck.BackgroundImage = screen2_bottom_selected;
        }
        private void Screen2_Bottom_left_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Screen2_Bottom_left();
            arrow = 11;
        }
        private void Screen2_Bottom_left_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[215]);
            if (arrow == 11)
                selected_Screen2_Bottom_left();
            else
                hover_Screen2_Bottom_left();
        }
        private void Screen2_Bottom_left_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 11)
                checked_Screen2_Bottom_left();
            else
                unchecked_Screen2_Bottom_left();
        }
        private void checked_Screen2_Bottom_left()
        {
            banner_11_ck.BackgroundImage = screen2_bottom_left_on;
        }
        private void unchecked_Screen2_Bottom_left()
        {
            banner_11_ck.BackgroundImage = screen2_bottom_left_off;
        }
        private void hover_Screen2_Bottom_left()
        {
            banner_11_ck.BackgroundImage = screen2_bottom_left_hover;
        }
        private void selected_Screen2_Bottom_left()
        {
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X != 0)
                {
                    this.Size = new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height / 2);
                    this.Location = new Point(Screen.AllScreens[i].Bounds.X, Screen.AllScreens[i].Bounds.Y + Screen.AllScreens[i].Bounds.Height / 2);
                }
            }
            banner_11_ck.BackgroundImage = screen2_bottom_left_selected;
        }
        private void Screen2_Arrow_1080p_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_Screen2_Arrow_1080p();
            arrow = 15;
        }
        private void Screen2_Arrow_1080p_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[216]);
            if (arrow == 15)
                selected_Screen2_Arrow_1080p();
            else
                hover_Screen2_Arrow_1080p();
        }
        private void Screen2_Arrow_1080p_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == 15)
                checked_Screen2_Arrow_1080p();
            else
                unchecked_Screen2_Arrow_1080p();
        }
        private void checked_Screen2_Arrow_1080p()
        {
            banner_15_ck.BackgroundImage = screen2_arrow_1080p_on;
        }
        private void unchecked_Screen2_Arrow_1080p()
        {
            banner_15_ck.BackgroundImage = screen2_arrow_1080p_off;
        }
        private void hover_Screen2_Arrow_1080p()
        {
            banner_15_ck.BackgroundImage = screen2_arrow_1080p_hover;
        }
        private void selected_Screen2_Arrow_1080p()
        {
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X != 0)
                {
                    this.Size = new Size(1920, 1080);
                    this.Location = Screen.AllScreens[i].Bounds.Location;
                }
            }
            banner_15_ck.BackgroundImage = screen2_arrow_1080p_selected;
        }
        private void input_file_Click(object sender, EventArgs e)
        {
            FileDialog dialog = new OpenFileDialog
            {
                Title = "Select a picture or a texture",
                Filter = "Picture|*.bmp;*.png;*.jfif;*.jpg;*.jpeg;*.jpg;*.ico;*.gif;*.tif;*.tiff;*.rle;*.dib|Texture|*.bti;*.tex0;*.tpl|All files (*.*)|*.*",
                RestoreDirectory = true
            };
            if (layout == 3)
            {
                dialog.Title = "Select a CMPR texture";
                dialog.Filter = "Texture|*.bti;*.tex0;*.tpl|All files (*.*)|*.*";
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                input_file_txt.Text = dialog.FileName;
                input_file = dialog.FileName;
                Check_run();
                Check_Paint();
                Organize_args();
            }
        }
        private void input_file_MouseEnter(object sender, EventArgs e)
        {
            if (layout == 3)
                Parse_Markdown(config[217]);
            else
                Parse_Markdown(config[218]);
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
            Preview(true);
            Check_Paint();
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
        private void input_file2_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[219]);
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
            Preview(true);
        }
        private void output_name_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Output file name",
                Filter = "All files (*.*)|*.*|Texture|*.bti;*.tex0;*.tpl|Picture|*.bmp;*.png;*.jfif;*.jpg;*.jpeg;*.jpg;*.ico;*.gif;*.tif;*.tiff;*.rle;*.dib",
                RestoreDirectory = true,
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                output_name_txt.Text = saveFileDialog.FileName;
            }
        }
        private void output_name_MouseEnter(object sender, EventArgs e)
        {
            if (layout == 3)
                Parse_Markdown(config[220]);
            else
                Parse_Markdown(config[221]);
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
            Preview(true);
        }
        private void mipmaps_MouseEnter(object sender, EventArgs e)
        {
            if (layout == 3)
                Parse_Markdown(config[222]);
            else
                Parse_Markdown(config[223]);
        }
        private void mipmaps_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void mipmaps_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(mipmaps_txt, out mipmaps, 255);
            Organize_args();
            Preview(true);
            Change_mipmap();
        }
        private void cmpr_max_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[224]);
        }
        private void cmpr_max_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_max_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(cmpr_max_txt, out cmpr_max, 16);
            Organize_args();
            Preview(true);
        }
        private void cmpr_min_alpha_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[225]);
        }
        private void cmpr_min_alpha_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_min_alpha_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(cmpr_min_alpha_txt, out cmpr_min_alpha, 255);
            Organize_args();
            Preview(true);
        }
        private void num_colours_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[226]);
        }
        private void num_colours_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void num_colours_TextChanged(object sender, EventArgs e)
        {
            Parse_ushort_text(num_colours_txt, out num_colours, 65535);
            Organize_args();
            Preview(true);
        }
        private void round3_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[227]);
        }
        private void round3_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void round3_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(round3_txt, out round3, 32);
            Organize_args();
            Preview(true);
        }
        private void round4_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[228]);
        }
        private void round4_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void round4_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(round4_txt, out round4, 16);
            Organize_args();
            Preview(true);
        }
        private void round5_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[229]);
        }
        private void round5_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void round5_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(round5_txt, out round5, 8);
            Organize_args();
            Preview(true);
        }
        private void round6_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[230]);
        }
        private void round6_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void round6_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(round6_txt, out round6, 4);
            Organize_args();
            Preview(true);
        }
        private void diversity_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[231]);
        }
        private void diversity_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void diversity_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(diversity_txt, out diversity, 255);
            Organize_args();
            Preview(true);
        }
        private void diversity2_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[232]);
        }
        private void diversity2_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void diversity2_TextChanged(object sender, EventArgs e)
        {
            Parse_byte_text(diversity2_txt, out diversity2, 255);
            Organize_args();
            Preview(true);
        }
        private void percentage_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[233]);
        }
        private void percentage_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void percentage_TextChanged(object sender, EventArgs e)
        {
            Parse_double_text(percentage_txt, out percentage, 100.0);
            Organize_args();
            Preview(true);
        }
        private void percentage2_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[234]);
        }
        private void percentage2_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void percentage2_TextChanged(object sender, EventArgs e)
        {
            Parse_double_text(percentage2_txt, out percentage2, 100.0);
            Organize_args();
            Preview(true);
        }
        private void custom_r_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[235]);
        }
        private void custom_r_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void custom_r_TextChanged(object sender, EventArgs e)
        {
            Parse_double_text(custom_r_txt, out custom_r, 255.0);
            Organize_args();
            Preview(true);
        }
        private void custom_g_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[236]);
        }
        private void custom_g_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void custom_g_TextChanged(object sender, EventArgs e)
        {
            Parse_double_text(custom_g_txt, out custom_g, 255.0);
            Organize_args();
            Preview(true);
        }
        private void custom_b_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[237]);
        }
        private void custom_b_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void custom_b_TextChanged(object sender, EventArgs e)
        {
            Parse_double_text(custom_b_txt, out custom_b, 255.0);
            Organize_args();
            Preview(true);
        }
        private void custom_a_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[238]);
        }
        private void custom_a_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void custom_a_TextChanged(object sender, EventArgs e)
        {
            Parse_double_text(custom_a_txt, out custom_a, 255.0);
            Organize_args();
            Preview(true);
        }
        private void palette_AI8_Click(object sender, EventArgs e)
        {
            unchecked_palette(palette_ck[palette_enc]);
            Hide_encoding((byte)(palette_enc + 3));
            selected_palette(palette_ai8_ck);
            palette_enc = 0;
            View_ai8();
            Organize_args();
            Preview(false);
        }
        private void palette_AI8_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[239]);
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
            Hide_encoding((byte)(palette_enc + 3));
            selected_palette(palette_rgb565_ck);
            palette_enc = 1;
            View_rgb565();
            Organize_args();
            Preview(false);
        }
        private void palette_RGB565_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[240]);
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
            Hide_encoding((byte)(palette_enc + 3));
            selected_palette(palette_rgb5a3_ck);
            palette_enc = 2;
            View_rgb5a3();
            Organize_args();
            Preview(false);
        }
        private void palette_RGB5A3_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[241]);
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
            Parse_Markdown(config[242]);
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
            Parse_Markdown(config[243]);
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
            Parse_Markdown(config[244]);
            youtube_ck.BackgroundImage = youtube_hover;
        }
        private void youtube_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            youtube_ck.BackgroundImage = youtube;
        }
        private void view_alpha_Click(object sender, EventArgs e)
        {
            if (view_alpha)
            {
                Hide_alpha(true);
                Category_hover(view_alpha_ck);
            }
            else
            {
                View_alpha(true);
                Category_selected(view_alpha_ck);
            }
        }
        private void view_alpha_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[245]);
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
                Hide_algorithm(255, true);
                Category_hover(view_algorithm_ck);
            }
            else
            {
                View_algorithm(255, true);
                Category_selected(view_algorithm_ck);
            }
        }
        private void view_algorithm_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[246]);
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
                Hide_WrapS(true);
                Category_hover(view_WrapS_ck);
            }
            else
            {
                View_WrapS(true);
                Category_selected(view_WrapS_ck);
            }
        }
        private void view_WrapS_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[247]);
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
                Hide_WrapT(true);
                Category_hover(view_WrapT_ck);
            }
            else
            {
                View_WrapT(true);
                Category_selected(view_WrapT_ck);
            }
        }
        private void view_WrapT_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[248]);
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
                Hide_min(true);
                Category_hover(view_min_ck);
            }
            else
            {
                View_min(true);
                Category_selected(view_min_ck);
            }
        }
        private void view_min_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[249]);
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
                Hide_mag(true);
                Category_hover(view_mag_ck);
            }
            else
            {
                View_mag(true);
                Category_selected(view_mag_ck);
            }
        }
        private void view_mag_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[250]);
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
            if (view_rgba)
            {
                Hide_rgba(true);
                Category_hover(view_rgba_ck);
            }
            else
            {
                View_rgba(true);
                Category_selected(view_rgba_ck);
            }
        }
        private void view_rgba_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[251]);
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
                Hide_palette(true);
                Category_hover(view_palette_ck);
            }
            else
            {
                View_palette(true);
                Category_selected(view_palette_ck);
            }
        }
        private void view_palette_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[252]);
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
                Hide_cmpr(true);
                Category_hover(view_cmpr_ck);
            }
            else
            {
                View_cmpr(true);
                Category_selected(view_cmpr_ck);
            }
        }
        private void view_cmpr_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[253]);
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
                Hide_options(true);
                Category_hover(view_options_ck);
            }
            else
            {
                View_options(true);
                Category_selected(view_options_ck);
            }
        }
        private void view_options_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[254]);
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
        private void version_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[255]);
            version_ck.BackgroundImage = version_hover;
        }
        private void version_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            version_ck.BackgroundImage = version;
        }
        private void cli_textbox_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[256]);
            cli_textbox_ck.BackgroundImage = cli_textbox_hover;
            this.cli_textbox_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(4)))), ((int)(((byte)(0)))));
        }
        private void cli_textbox_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            cli_textbox_ck.BackgroundImage = cli_textbox;
            this.cli_textbox_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(20)))), ((int)(((byte)(0)))), ((int)(((byte)(49)))));
        }
        private void run_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[257]);
            run_ck.BackgroundImage = run_hover;
        }
        private void run_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            Check_run();
        }
        private void Output_label_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[258]);
        }
        private void Output_label_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void banner_global_move_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[259]);
            if (banner_global_move)
                banner_global_move_ck.BackgroundImage = banner_global_move_selected;
            else
                banner_global_move_ck.BackgroundImage = banner_global_move_hover;
        }
        private void banner_global_move_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (banner_global_move)
                banner_global_move_ck.BackgroundImage = banner_global_move_on;
            else
                banner_global_move_ck.BackgroundImage = banner_global_move_off;
        }
        private void banner_move_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[260]);
        }
        private void banner_move_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void banner_resize_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[261]);
        }
        private void banner_resize_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void sync_preview_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[262]);
            if (!preview_changed)
                sync_preview_ck.BackgroundImage = sync_preview_hover;
            else
                sync_preview_ck.BackgroundImage = sync_preview_selected;
        }
        private void sync_preview_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (!preview_changed)
                sync_preview_ck.BackgroundImage = sync_preview_off;
            else
                sync_preview_ck.BackgroundImage = sync_preview_on;
        }
        private void cmpr_save_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[263]);
            cmpr_save_ck.BackgroundImage = cmpr_save_hover;
        }
        private void cmpr_save_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            cmpr_save_ck.BackgroundImage = cmpr_save;
        }
        private void cmpr_save_as_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[264]);
            cmpr_save_as_ck.BackgroundImage = cmpr_save_as_hover;
        }
        private void cmpr_save_as_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            cmpr_save_as_ck.BackgroundImage = cmpr_save_as;
        }
        private void cmpr_swap_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[265]);
            cmpr_swap_ck.BackgroundImage = cmpr_swap_hover;
        }
        private void cmpr_swap_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            cmpr_swap_ck.BackgroundImage = cmpr_swap;
        }
        private void cmpr_swap2_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[266]);
            cmpr_swap2_ck.BackgroundImage = cmpr_swap2_hover;
        }
        private void cmpr_swap2_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            cmpr_swap2_ck.BackgroundImage = cmpr_swap2;
        }
        private void cmpr_palette_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[267]);
        }
        private void cmpr_palette_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_mouse1_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[268]);
        }
        private void cmpr_mouse1_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_mouse2_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[269]);
        }
        private void cmpr_mouse2_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_mouse3_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[270]);
        }
        private void cmpr_mouse3_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_mouse4_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[271]);
        }
        private void cmpr_mouse4_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_mouse5_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[272]);
        }
        private void cmpr_mouse5_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_sel_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[273]);
        }
        private void cmpr_sel_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_c1_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[274]);
        }
        private void cmpr_c1_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_c2_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[275]);
        }
        private void cmpr_c2_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_c3_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[276]);
        }
        private void cmpr_c3_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_c4_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[277]);
        }
        private void cmpr_c4_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
        }
        private void cmpr_c1_Click(object sender, EventArgs e)
        {
            cmpr_selected_colour = 1;
            cmpr_sel.BackColor = System.Drawing.Color.FromArgb(cmpr_colours_argb[0], cmpr_colours_argb[1], cmpr_colours_argb[2], cmpr_colours_argb[3]);
        }
        private void cmpr_c2_Click(object sender, EventArgs e)
        {
            cmpr_selected_colour = 2;
            cmpr_sel.BackColor = System.Drawing.Color.FromArgb(cmpr_colours_argb[4], cmpr_colours_argb[5], cmpr_colours_argb[6], cmpr_colours_argb[7]);
        }
        private void cmpr_c3_Click(object sender, EventArgs e)
        {
            cmpr_selected_colour = 3;
            cmpr_sel.BackColor = System.Drawing.Color.FromArgb(cmpr_colours_argb[8], cmpr_colours_argb[9], cmpr_colours_argb[10], cmpr_colours_argb[11]);
        }
        private void cmpr_c4_Click(object sender, EventArgs e)
        {
            cmpr_selected_colour = 4;
            cmpr_sel.BackColor = System.Drawing.Color.FromArgb(cmpr_colours_argb[12], cmpr_colours_argb[13], cmpr_colours_argb[14], cmpr_colours_argb[15]);
        }
        private void banner_global_move_MouseDown(object sender, MouseEventArgs e)
        {
            // e.Button;
            mouse_x = e.X;
            mouse_y = e.Y;
        }
        private void banner_global_move_MouseMove(object sender, MouseEventArgs e)
        {
            if (banner_global_move && e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mouse_x, this.Location.Y + e.Y - mouse_y);
            }
        }
        private void banner_move_MouseDown(object sender, MouseEventArgs e)
        {
            // e.Button;
            mouse_x = e.X;
            mouse_y = e.Y;
        }
        private void banner_move_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mouse_x, this.Location.Y + e.Y - mouse_y);
            }
        }
        private void banner_resize_MouseDown(object sender, MouseEventArgs e)
        {
            // e.Button;
            mouse_x = e.X;
            mouse_y = e.Y;
        }
        private void banner_resize_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mouse_x, this.Location.Y + e.Y - mouse_y);
                this.Size = new Size(this.Size.Width + mouse_x - e.X, this.Size.Height + mouse_y - e.Y);
            }
        }
        private void textchange_Click(object sender, EventArgs e)
        {
            if (textchange)
            {
                textchange = false;
                hover_checkbox(textchange_ck);
            }
            else
            {
                textchange = true;
                selected_checkbox(textchange_ck);
            }
        }
        private void textchange_MouseEnter(object sender, EventArgs e)
        {
            if (textchange)
                selected_checkbox(textchange_ck);
            else
                hover_checkbox(textchange_ck);
        }
        private void textchange_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (textchange)
                checked_checkbox(textchange_ck);
            else
                unchecked_checkbox(textchange_ck);
        }
        private void auto_update_Click(object sender, EventArgs e)
        {
            if (auto_update)
            {
                auto_update = false;
                hover_checkbox(auto_update_ck);
            }
            else
            {
                auto_update = true;
                selected_checkbox(auto_update_ck);
            }
        }
        private void auto_update_MouseEnter(object sender, EventArgs e)
        {
            if (auto_update)
                selected_checkbox(auto_update_ck);
            else
                hover_checkbox(auto_update_ck);
        }
        private void auto_update_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (auto_update)
                checked_checkbox(auto_update_ck);
            else
                unchecked_checkbox(auto_update_ck);
        }
        private void upscale_Click(object sender, EventArgs e)
        {
            if (upscale)
            {
                upscale = false;
                hover_checkbox(upscale_ck);
            }
            else
            {
                upscale = true;
                selected_checkbox(upscale_ck);
            }
        }
        private void upscale_MouseEnter(object sender, EventArgs e)
        {
            if (upscale)
                selected_checkbox(upscale_ck);
            else
                hover_checkbox(upscale_ck);
        }
        private void upscale_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (upscale)
                checked_checkbox(upscale_ck);
            else
                unchecked_checkbox(upscale_ck);
        }
        private void cmpr_block_selection_Click(object sender, EventArgs e)
        {
            unchecked_tooltip(cmpr_block_paint_ck);
            selected_tooltip(cmpr_block_selection_ck);
            tooltip = 0; // cmpr_block_selection
        }
        private void cmpr_block_selection_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[278]);
            if (tooltip == 0)
                selected_tooltip(cmpr_block_selection_ck);
            else
                hover_tooltip(cmpr_block_selection_ck);
        }
        private void cmpr_block_selection_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (tooltip == 0)
                checked_tooltip(cmpr_block_selection_ck);
            else
                unchecked_tooltip(cmpr_block_selection_ck);
        }
        private void cmpr_block_paint_Click(object sender, EventArgs e)
        {
            unchecked_tooltip(cmpr_block_selection_ck);
            selected_tooltip(cmpr_block_paint_ck);
            tooltip = 1; // cmpr_block_paint
        }
        private void cmpr_block_paint_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(config[279]);
            if (tooltip == 1)
                selected_tooltip(cmpr_block_paint_ck);
            else
                hover_tooltip(cmpr_block_paint_ck);
        }
        private void cmpr_block_paint_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (tooltip == 1)
                checked_tooltip(cmpr_block_paint_ck);
            else
                unchecked_tooltip(cmpr_block_paint_ck);
        }
        private void Warn_rgb565_colour_trim()
        {
            Parse_Markdown(config[280], cmpr_warning);
        }
        private void Put_that_damn_cmpr_layout_in_place()
        {
            Check_Paint();
            Parse_Markdown(config[281], cmpr_mouse1_label);
            Parse_Markdown(config[282], cmpr_mouse2_label);
            Parse_Markdown(config[283], cmpr_mouse3_label);
            Parse_Markdown(config[284], cmpr_mouse4_label);
            Parse_Markdown(config[285], cmpr_mouse5_label);
            checked_tooltip(cmpr_block_selection_ck);
            unchecked_tooltip(cmpr_block_paint_ck);
        }
        private void Check_Paint()
        {
            if (layout != 3)
                return;
            if (File.Exists(input_file))
            {
                using (FileStream fs = File.OpenRead(input_file))
                {
                    Array.Resize(ref cmpr_file, (int)fs.Length);  // with this, 2GB is the max size for a texture. if it was an unsigned int, the limit would be 4GB
                    fs.Read(cmpr_file, 0, (int)fs.Length);
                }
                if (cmpr_file[0] == 0 && cmpr_file[1] == 32 && cmpr_file[2] == 0xaf && cmpr_file[3] == 48)
                    return; // not implemented yet.
                else if (cmpr_file[0] == 84 && cmpr_file[1] == 69 && cmpr_file[2] == 88 && cmpr_file[3] == 48)
                    cmpr_data_start_offset = 0x40;
                else if (cmpr_file[0] < 15 && cmpr_file[6] < 3 && cmpr_file[7] < 3)
                    cmpr_data_start_offset = (cmpr_file[0x1C] << 24) | (cmpr_file[0x1D] << 16) | (cmpr_file[0x1E] << 8) | cmpr_file[0x1F];  // usually 0x20

                int num = 1;
                while (File.Exists(execPath + "images/preview/" + num + ".bmp"))
                {
                    num++;
                }
                cmpr_args[2] = input_file;
                cmpr_args[3] = (execPath + "images/preview/" + num + ".bmp");  // even if there's an output file in the args, the last one is the output file :) that's how I made it
                Parse_args_class cli = new Parse_args_class();
                cli.Parse_args(cmpr_args);
                if (cli.texture_format != 0xE)
                {
                    Parse_Markdown(config[286], cmpr_warning);
                    return;
                }
                if (File.Exists(execPath + "images/preview/" + num + ".bmp"))
                {
                    previous_block = -1;
                    loaded_block = -1;
                    using (FileStream fs = File.OpenRead(execPath + "images/preview/" + num + ".bmp"))
                    {
                        Array.Resize(ref cmpr_preview, (int)fs.Length);  // with this, 2GB is the max size for a texture. if it was an unsigned int, the limit would be 4GB
                        fs.Read(cmpr_preview, 0, (int)fs.Length);
                    }
                    cmpr_preview_ck.Image = GetImageFromByteArray(cmpr_preview);
                    cmpr_preview_vanilla = cmpr_preview.ToArray();
                    if (cmpr_preview_ck.Image.Height > cmpr_preview_ck.Image.Width)
                    {
                        cmpr_y_offscreen = 0;
                        mag_ratio = 1 + (double)(1024 - cmpr_preview_ck.Image.Height) / (double)cmpr_preview_ck.Image.Height;
                        cmpr_x_offscreen = (ushort)((ushort)(1024 - (cmpr_preview_ck.Image.Width * mag_ratio)) >> 1);  // a fix for non-squared images
                    }
                    else
                    {
                        cmpr_x_offscreen = 0;
                        mag_ratio = 1 + (double)(1024 - cmpr_preview_ck.Image.Width) / (double)cmpr_preview_ck.Image.Width;
                        cmpr_y_offscreen = (ushort)((ushort)(1024 - (cmpr_preview_ck.Image.Height * mag_ratio)) >> 1);  // a fix for non-squared images
                    }
                    blocks_wide = (ushort)(cmpr_preview_ck.Image.Width >> 2);
                    blocks_tall = (ushort)(cmpr_preview_ck.Image.Height >> 2);
                    max_block = blocks_wide * blocks_tall - 1;  // minus one because the first block is zero
                    cmpr_preview_start_offset = (cmpr_preview.Length - (cmpr_preview_ck.Image.Width << 2));
                    cmpr_warning.Text = "";
                }
                else
                    cmpr_warning.Text = cli.Check_exit();
            }
            else
            {
                Parse_Markdown(config[286], cmpr_warning);
            }
        }
        private void cmpr_c2_TextChanged(object sender, EventArgs e)
        {
            parse_rgb565(cmpr_c2, cmpr_c2_txt, 2, out colour2, colour2);
        }

        private void cmpr_c1_TextChanged(object sender, EventArgs e)
        {
            parse_rgb565(cmpr_c1, cmpr_c1_txt, 0, out colour1, colour1);
        }
        private void Swap_Colours_Click(object sender, EventArgs e)
        {
            seal = cmpr_c1_txt.Text;
            cmpr_c1_txt.Text = cmpr_c2_txt.Text;
            cmpr_c2_txt.Text = seal;
        }
        private void cmpr_swap2_Click(object sender, EventArgs e)
        {
            seal = cmpr_c1_txt.Text;
            cmpr_c1_txt.Text = cmpr_c2_txt.Text;
            cmpr_swap2_enabled = true;
            cmpr_c2_txt.Text = seal;
            cmpr_swap2_enabled = false;
        }
        private void cmpr_grid_ck_MouseMove(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Paint_Pixel(e.X, e.Y, cmpr_selected_colour);
                    break;
                case MouseButtons.Middle:
                    Paint_Pixel(e.X, e.Y, 1);
                    break;
                case MouseButtons.Right:
                    Paint_Pixel(e.X, e.Y, 2);
                    break;
                case MouseButtons.XButton2:
                    Paint_Pixel(e.X, e.Y, 3);
                    break;
                case MouseButtons.XButton1:
                    Paint_Pixel(e.X, e.Y, 4);
                    break;
            }
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
        private void sync_preview_Click(object sender, EventArgs e)
        {
            Preview(false, true);
        }
        private void Get_Pixel_Colour(MouseButtons e)
        {
            cmpr_x &= 3;
            cmpr_y &= 3;

            switch (e)
            {
                case MouseButtons.Left:
                    Change_Colour_From_Pixel(cmpr_selected_colour);
                    break;
                case MouseButtons.Middle:
                    Change_Colour_From_Pixel(1);
                    break;
                case MouseButtons.Right:
                    Change_Colour_From_Pixel(2);
                    break;
                case MouseButtons.XButton2:
                    Change_Colour_From_Pixel(3);
                    break;
                case MouseButtons.XButton1:
                    Change_Colour_From_Pixel(4);
                    break;
            }
        }
        private void Change_Colour_From_Pixel(byte cmpr_colour_index)
        {
            if (cmpr_colour_index > 2)
            {
                Parse_Markdown(config[287], cmpr_warning);
            }
            cmpr_index_i = (byte)((cmpr_file[cmpr_data_start_offset + (current_block << 3) + 4 + cmpr_y] >> (6 - (cmpr_x << 1))) & 3);
            if (cmpr_index_i < 2)
            {
                cmpr_colours_argb[(cmpr_colour_index << 2) - 1] = (byte)(cmpr_file[cmpr_data_start_offset + (current_block << 3) + (cmpr_index_i << 1) + 1] << 3); // blue
                cmpr_colours_argb[(cmpr_colour_index << 2) - 2] = (byte)(((cmpr_file[cmpr_data_start_offset + (current_block << 3) + (cmpr_index_i << 1)] << 5) | (cmpr_file[cmpr_data_start_offset + (current_block << 3) + (cmpr_index_i << 1) + 1] >> 3)) & 0xfc);  // green
                cmpr_colours_argb[(cmpr_colour_index << 2) - 3] = (byte)(cmpr_file[cmpr_data_start_offset + (current_block << 3) + (cmpr_index_i << 1)] & 0xf8);  // red
            }
            else
            {
                colour3 = (ushort)((cmpr_file[cmpr_data_start_offset + (current_block << 3)] << 8) | cmpr_file[cmpr_data_start_offset + (current_block << 3) + 1]);
                colour4 = (ushort)((cmpr_file[cmpr_data_start_offset + (current_block << 3) + 2] << 8) | cmpr_file[cmpr_data_start_offset + (current_block << 3) + 3]);

                red = (byte)((colour3 >> 8) & 248);
                green = (byte)((((colour3 >> 8) & 7) << 5) + (((byte)colour3 >> 3) & 28));
                blue = (byte)(((byte)colour3 << 3) & 248);

                red2 = (byte)((colour4 >> 8) & 248);
                green2 = (byte)((((colour4 >> 8) & 7) << 5) + (((byte)colour4 >> 3) & 28));
                blue2 = (byte)(((byte)colour4 << 3) & 248);

                if (colour1 > colour2)
                {
                    if (cmpr_index_i == 2)
                    {
                        cmpr_colours_argb[(cmpr_colour_index << 2) - 3] = (byte)((red * 2 / 3) + (red2 / 3));
                        cmpr_colours_argb[(cmpr_colour_index << 2) - 2] = (byte)((green * 2 / 3) + (green2 / 3));
                        cmpr_colours_argb[(cmpr_colour_index << 2) - 1] = (byte)((blue * 2 / 3) + (blue2 / 3));
                    }
                    else
                    {
                        cmpr_colours_argb[(cmpr_colour_index << 2) - 3] = (byte)((red / 3) + (red2 * 2 / 3));
                        cmpr_colours_argb[(cmpr_colour_index << 2) - 2] = (byte)((green / 3) + (green2 * 2 / 3));
                        cmpr_colours_argb[(cmpr_colour_index << 2) - 1] = (byte)((blue / 3) + (blue2 * 2 / 3));
                    }
                }
                else
                {
                    // of course, that's the exact opposite! - not quite lol
                    if (cmpr_index_i == 2)
                    {
                        cmpr_colours_argb[(cmpr_colour_index << 2) + 1] = (byte)((red * 2 / 3) + (red2 / 3));
                        cmpr_colours_argb[(cmpr_colour_index << 2) + 2] = (byte)((green * 2 / 3) + (green2 / 3));
                        cmpr_colours_argb[(cmpr_colour_index << 2) + 3] = (byte)((blue * 2 / 3) + (blue2 / 3));
                    }
                    else
                    {
                        Parse_Markdown(config[288], cmpr_warning);
                    }
                }
            }
            cmpr_colours_hex = BitConverter.ToString(cmpr_colours_argb).Replace("-", string.Empty);
            if (cmpr_colour_index == 1)
                cmpr_c1_txt.Text = cmpr_colours_hex.Substring(2, 6);
            else
                cmpr_c2_txt.Text = cmpr_colours_hex.Substring(10, 6);

        }
        private void cmpr_preview_ck_MouseMove(object sender, MouseEventArgs e)
        {
            if (cmpr_preview == null)
                return;
            cmpr_x = (int)((e.X - cmpr_x_offscreen) / mag_ratio);
            cmpr_y = (int)((e.Y - cmpr_y_offscreen) / mag_ratio);
            if (cmpr_x < 0 || cmpr_y < 0)
            {
                cmpr_preview_ck_MouseLeave(null, null);
                return;
            }
            block_x = (ushort)(cmpr_x >> 2);
            block_y = (ushort)(cmpr_y >> 2);
            if (block_y % 2 == 1)
            {
                current_block = (block_y * blocks_wide) + 2 + (((block_x >> 1) << 2) + (block_x % 2));
                current_block -= blocks_wide;
            }
            else
                current_block = (block_y * blocks_wide) + (((block_x >> 1) << 2) + (block_x % 2));
            if (tooltip == 1)
            {
                if (e.Button != MouseButtons.None)
                    Get_Pixel_Colour(e.Button);
                return;
            }
            // block_y = (ushort)(block_y * blocks_wide);
            //current_block = block_x + block_y;
            if (e.Button == MouseButtons.Left)
            {
                Load_cmpr_block();
                return;
            }
            if (!cmpr_hover)
                return;
            if (current_block == previous_block)
                return;
            if (current_block > max_block)
            {
                cmpr_preview_ck_MouseLeave(null, null);
                return;
            }
            previous_block = current_block;
            cmpr_preview = cmpr_preview_vanilla.ToArray();
            cmpr_preview[cmpr_preview_start_offset + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 1 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 2 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 3 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset + 4 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 5 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 6 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 7 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset + 8 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 9 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 10 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 11 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset + 12 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 13 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 14 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 15 + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 1 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 2 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 3 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset + 4 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 5 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 6 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 7 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset + 8 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 9 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 10 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 11 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset + 12 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 13 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 14 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 15 - (cmpr_preview_ck.Image.Width << 2) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 1 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 2 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 3 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset + 4 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 5 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 6 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 7 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset + 8 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 9 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 10 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 11 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset + 12 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 13 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 14 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 15 - (cmpr_preview_ck.Image.Width << 3) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 1 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 2 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 3 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset + 4 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 5 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 6 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 7 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset + 8 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 9 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 10 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 11 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview[cmpr_preview_start_offset + 12 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_blue;
            cmpr_preview[cmpr_preview_start_offset + 13 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_green;
            cmpr_preview[cmpr_preview_start_offset + 14 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_red;
            cmpr_preview[cmpr_preview_start_offset + 15 - (cmpr_preview_ck.Image.Width * 12) + (block_x << 4) - ((block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_hover_alpha;
            cmpr_preview_ck.Image = GetImageFromByteArray(cmpr_preview);
        }
        private void Preview_Paint()
        {
            if (!cmpr_update_preview || cmpr_preview_vanilla == null)
                return;
            cmpr_preview_vanilla[cmpr_preview_start_offset + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[0] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 1 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[0] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 2 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[0] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 3 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[0] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 4 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[1] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 5 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[1] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 6 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[1] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 7 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[1] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 8 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[2] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 9 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[2] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 10 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[2] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 11 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[2] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 12 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[3] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 13 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[3] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 14 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[3] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 15 + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[3] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[4] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 1 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[4] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 2 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[4] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 3 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[4] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 4 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[5] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 5 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[5] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 6 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[5] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 7 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[5] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 8 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[6] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 9 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[6] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 10 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[6] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 11 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[6] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 12 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[7] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 13 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[7] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 14 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[7] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 15 - (cmpr_preview_ck.Image.Width << 2) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[7] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[8] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 1 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[8] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 2 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[8] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 3 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[8] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 4 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[9] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 5 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[9] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 6 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[9] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 7 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[9] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 8 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[10] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 9 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[10] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 10 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[10] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 11 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[10] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 12 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[11] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 13 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[11] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 14 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[11] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 15 - (cmpr_preview_ck.Image.Width << 3) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[11] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[12] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 1 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[12] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 2 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[12] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 3 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[12] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 4 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[13] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 5 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[13] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 6 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[13] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 7 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[13] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 8 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[14] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 9 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[14] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 10 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[14] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 11 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[14] << 2)];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 12 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[15] << 2) + 3];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 13 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[15] << 2) + 2];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 14 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[15] << 2) + 1];
            cmpr_preview_vanilla[cmpr_preview_start_offset + 15 - (cmpr_preview_ck.Image.Width * 12) + (selected_block_x << 4) - ((selected_block_y * cmpr_preview_ck.Image.Width) << 4)] = cmpr_colours_argb[(cmpr_index[15] << 2)];
            cmpr_preview_ck.Image = GetImageFromByteArray(cmpr_preview_vanilla);
        }
        private void cmpr_hover_colour_TextChanged(object sender, EventArgs e)
        {
            parse_rgba_hover(cmpr_hover_colour, cmpr_hover_colour_txt);
        }

        private void cmpr_preview_ck_MouseLeave(object sender, EventArgs e)
        {
            if (cmpr_preview_vanilla != null && !cmpr_hover)
                cmpr_preview_ck.Image = GetImageFromByteArray(cmpr_preview_vanilla);
            previous_block = -1;
        }

        private void cmpr_preview_ck_MouseDown(object sender, MouseEventArgs e)
        {
            cmpr_preview_ck_MouseMove(sender, e);
        }
        private void Load_cmpr_block()
        {
            if (current_block > max_block)
                return; // you idiot thought I would allow that!!!!!
            the_program_is_loading_a_cmpr_block = true;
            loaded_block = current_block;
            selected_block_x = block_x;
            selected_block_y = block_y;
            cmpr_colours_argb[7] = (byte)(cmpr_file[cmpr_data_start_offset + (current_block << 3) + 3] << 3); // blue
            cmpr_colours_argb[6] = (byte)(((cmpr_file[cmpr_data_start_offset + (current_block << 3) + 2] << 5) | (cmpr_file[cmpr_data_start_offset + (current_block << 3) + 3] >> 3)) & 0xfc);  // green
            cmpr_colours_argb[5] = (byte)(cmpr_file[cmpr_data_start_offset + (current_block << 3) + 2] & 0xf8);  // red
            cmpr_colours_argb[3] = (byte)(cmpr_file[cmpr_data_start_offset + (current_block << 3) + 1] << 3); // blue
            cmpr_colours_argb[2] = (byte)(((cmpr_file[cmpr_data_start_offset + (current_block << 3)] << 5) | (cmpr_file[cmpr_data_start_offset + (current_block << 3) + 1] >> 3)) & 0xfc);  // green
            cmpr_colours_argb[1] = (byte)(cmpr_file[cmpr_data_start_offset + (current_block << 3)] & 0xf8);  // red
            // colour2 = (ushort)((cmpr_file[cmpr_data_start_offset + (current_block << 3) + 2] << 8) | cmpr_file[cmpr_data_start_offset + (current_block << 3) + 3]);  
            // ^ edit: no need to assign this AAAAA since parse_rgb565 does litteraly all the job by changing the textbox.Text
            // edit2: need to assign this AAAAA since Update_colours change them in the cmpr file and is called as soon as you edit ONE BYTE of cmpr_c1.txt
            // edit3: no. you don't need it.
            for (byte i = 0; i < 4; i++)
            {
                for (byte j = 0; j < 4; j++)
                {
                    cmpr_index[(i << 2) + j] = (byte)((cmpr_file[cmpr_data_start_offset + (current_block << 3) + 4 + i] >> (6 - (j << 1))) & 3);
                }
            }
            cmpr_colours_hex = BitConverter.ToString(cmpr_colours_argb).Replace("-", string.Empty);
            cmpr_c1_txt.Text = cmpr_colours_hex.Substring(2, 6);
            cmpr_c2_txt.Text = cmpr_colours_hex.Substring(10, 6);
            the_program_is_loading_a_cmpr_block = false;
        }
        private void Save_CMPR_Texture(string cmpr_filename)
        {
            try
            {
                using (FileStream fs = new FileStream(cmpr_filename, FileMode.Create))
                {
                    fs.Write(cmpr_file, 0, cmpr_file.Length);
                }
                Parse_Markdown(config[289], description);
            }
            catch (Exception ex)
            {
                description.Text = ex.Message;
                if (ex.Message.Substring(0, 34) == "The process cannot access the file")  // because it is being used by another process
                {
                    Parse_Markdown(config[290], description);
                }
            }
        }
        private void cmpr_save_ck_Click(object sender, EventArgs e)
        {
            Save_CMPR_Texture(output_name);
        }

        private void cmpr_save_as_ck_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save CMPR Texture",
                Filter = "All files (*.*)|*.*|Texture|*.bti;*.tex0;*.tpl",
                RestoreDirectory = true,
                //OverwritePrompt = true,
                //DereferenceLinks = true,
                //ValidateNames = true
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                output_name = saveFileDialog.FileName;
                Save_CMPR_Texture(saveFileDialog.FileName);
            }
        }
        private void cmpr_KeyDown(object sender, KeyEventArgs e)
        {

            cmpr_warning.Text = $"KeyDown code: {e.KeyCode}, value: {e.KeyValue}, modifiers: {e.Modifiers}";
            if (e.KeyCode == Keys.F1)
                All_Click(null, null);
            else if (e.KeyCode == Keys.F2)
                Auto_Click(null, null);
            else if (e.KeyCode == Keys.F3)
                Preview_Click(null, null);
            else if (e.KeyCode == Keys.F4)
                Paint_Click(null, null);
            else if (e.KeyCode == Keys.F8)  // the dev key to reload all graphics
                InitializeForm(false, false);
            else if (e.KeyCode == Keys.F9)  // the dev key to reload settings.txt and all graphics
                InitializeForm(true, false);
            else if (e.KeyCode == Keys.F10)
                Minimized_Click(null, null);
            else if (e.KeyCode == Keys.F11)
                Maximized_Click(null, null);
            else if (e.KeyCode == Keys.F12)  // the dev key to reload settings.txt
                Load_settings();
            else if (e.KeyCode == Keys.Clear)
                Easter_Egg();  // God Luck finding this key :P
            else if (e.KeyCode == Keys.Escape)
                Environment.Exit(0);
            else if (e.Control && e.KeyCode == Keys.Left)
                Left_Click(null, null);
            else if (e.Control && e.KeyCode == Keys.Right)
                Right_Click(null, null);
            else if (e.Control && e.KeyCode == Keys.Up)
                Top_Click(null, null);
            else if (e.Control && e.KeyCode == Keys.Down)
                Bottom_Click(null, null);
            else if (e.Alt && e.Control && (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1))
                Screen2_Bottom_left_Click(null, null);
            else if (e.Alt && e.Control && (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2))
                Screen2_Bottom_Click(null, null);
            else if (e.Alt && e.Control && (e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3))
                Screen2_Bottom_right_Click(null, null);
            else if (e.Alt && e.Control && (e.KeyCode == Keys.D4 || e.KeyCode == Keys.NumPad4))
                Screen2_Left_Click(null, null);
            else if (e.Alt && e.Control && (e.KeyCode == Keys.D5 || e.KeyCode == Keys.NumPad5))
                Screen2_Arrow_1080p_Click(null, null);
            else if (e.Alt && e.Control && (e.KeyCode == Keys.D6 || e.KeyCode == Keys.NumPad6))
                Screen2_Right_Click(null, null);
            else if (e.Alt && e.Control && (e.KeyCode == Keys.D7 || e.KeyCode == Keys.NumPad7))
                Screen2_Top_left_Click(null, null);
            else if (e.Alt && e.Control && (e.KeyCode == Keys.D8 || e.KeyCode == Keys.NumPad8))
                Screen2_Top_Click(null, null);
            else if (e.Alt && e.Control && (e.KeyCode == Keys.D9 || e.KeyCode == Keys.NumPad9))
                Screen2_Top_right_Click(null, null);
            else if (e.Alt && e.KeyCode == Keys.Left)
                Screen2_Left_Click(null, null);
            else if (e.Alt && e.KeyCode == Keys.Right)
                Screen2_Right_Click(null, null);
            else if (e.Alt && e.KeyCode == Keys.Up)
                Screen2_Top_Click(null, null);
            else if (e.Alt && e.KeyCode == Keys.Down)
                Screen2_Bottom_Click(null, null);
            else if (e.Alt && (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1))
                Bottom_left_Click(null, null);
            else if (e.Alt && (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2))
                Bottom_Click(null, null);
            else if (e.Alt && (e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3))
                Bottom_right_Click(null, null);
            else if (e.Alt && (e.KeyCode == Keys.D4 || e.KeyCode == Keys.NumPad4))
                Left_Click(null, null);
            else if (e.Alt && (e.KeyCode == Keys.D5 || e.KeyCode == Keys.NumPad5))
                Arrow_1080p_Click(null, null);
            else if (e.Alt && (e.KeyCode == Keys.D6 || e.KeyCode == Keys.NumPad6))
                Right_Click(null, null);
            else if (e.Alt && (e.KeyCode == Keys.D7 || e.KeyCode == Keys.NumPad7))
                Top_left_Click(null, null);
            else if (e.Alt && (e.KeyCode == Keys.D8 || e.KeyCode == Keys.NumPad8))
                Top_Click(null, null);
            else if (e.Alt && (e.KeyCode == Keys.D9 || e.KeyCode == Keys.NumPad9))
                Top_right_Click(null, null);
            if (layout != 3) // not paint
            {
                if (e.Control && e.KeyCode == Keys.R)
                    Run_Click(null, null);
                return;
            }
            if (e.KeyCode == Keys.ControlKey)
                cmpr_block_paint_Click(null, null);
            if (e.KeyCode == Keys.Enter)
                cmpr_hover_Click(null, null);
            if (e.Control && e.Shift && e.KeyCode == Keys.S)
                cmpr_save_as_ck_Click(null, null);
            else if (e.Control && e.KeyCode == Keys.S)
                cmpr_save_ck_Click(null, null);
            else if (e.Control && e.KeyCode == Keys.U)
                Preview_Paint();
            else if (e.Control && (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1))
                cmpr_c1_Click(null, null);
            else if (e.Control && (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2))
                cmpr_c2_Click(null, null);
            else if (e.Control && (e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3))
                cmpr_c3_Click(null, null);
            else if (e.Control && (e.KeyCode == Keys.D4 || e.KeyCode == Keys.NumPad4))
                cmpr_c4_Click(null, null);
        }
        private void cmpr_KeyUp(object sender, KeyEventArgs e)
        {
            if (layout != 3) // not paint
                return;
            if (e.KeyCode == Keys.ControlKey)
                cmpr_block_selection_Click(null, null);
        }
    }
}

