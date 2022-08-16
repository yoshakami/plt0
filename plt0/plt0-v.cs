using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Linq;

namespace plt0_gui
{
    public partial class plt0_gui : Form
    {
        public string execPath = AppDomain.CurrentDomain.BaseDirectory;

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
        // radiobuttons
        byte encoding = 7;
        byte WrapS = 1; // 0 = Clamp   1 = Repeat   2 = Mirror
        byte WrapT = 1; // 0 = Clamp   1 = Repeat   2 = Mirror
        byte algorithm = 0;  // 0 = CIE 601    1 = CIE 709     2 = custom RGBA     3 = Most Used Colours (No Gradient)
        byte alpha = 9;  // 0 = no alpha - 1 = alpha - 2 = mix 
        byte magnification_filter = 1;  // 0 = Nearest Neighbour   1 = Linear
        byte minification_filter = 1;  // 0 = Nearest Neighbour   1 = Linear
        byte r = 0;
        byte g = 1;
        byte b = 2;
        byte a = 3;
        // numbers
        byte cmpr_max = 16;  // number of colours that the program should take care in each 4x4 block - should always be set to 16 for better results.  // wimgt's cmpr encoding is better than mine. I gotta admit. 
        byte cmpr_alpha_threshold = 100;
        byte diversity = 10;
        byte diversity2 = 0;
        byte mipmaps_number = 0;
        byte round3 = 16;
        byte round4 = 8;
        byte round5 = 4;
        byte round6 = 2;
        byte color;
        List<PictureBox> encoding_ck = new List<PictureBox>();
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
        Image white_box;
        Image light_blue_box;
        Image light_blue_check;
        Image check;
        Image pink_circle;
        Image pink_circle_on;
        Image violet_circle;
        Image violet_circle_on;
        Image chartreuse_circle;
        Image chartreuse_circle_on;
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
        // I couldn't manage to get external fonts working. this needs to be specified within the app itself :/
        // static string fontname = "Segoe UI";
        // Font font_normal = new System.Drawing.Font(fontname, 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
        // Font new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
        public plt0_gui()
        {
            //if (System.IO.File.Exists(execPath + "images/a.ttf"))
            //{
                // AddFont(System.IO.File.ReadAllBytes(execPath + "images/font.ttf"));
                // fontname = Get_font_name();
                /*using (PrivateFontCollection _privateFontCollection = new PrivateFontCollection())
                {
                    _privateFontCollection.AddFontFile(execPath + "images/a.ttf");
                    fontname = _privateFontCollection.Families[0].Name;
                }
                font_normal = new System.Drawing.Font(fontname, 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
                /*PrivateFontCollection collection = new PrivateFontCollection();
                collection.AddFontFile("images/font.ttf");
                FontFamily fontFamily = new FontFamily(collection.Families[0].Name, collection);
                font_normal = new System.Drawing.Font(NintendoP-NewRodin DB, 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);*/
            //}
            InitializeComponent();
            //output_file_type_label.Text = fontname;
            //algorithm_label.Text = algorithm_label.Font.Name;
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
            a_ck.Add(a_a_ck);
            a_ck.Add(a_b_ck);
            a_ck.Add(a_g_ck);
            a_ck.Add(a_r_ck);
            b_ck.Add(b_a_ck);
            b_ck.Add(b_b_ck);
            b_ck.Add(b_g_ck);
            b_ck.Add(b_r_ck);
            g_ck.Add(g_a_ck);
            g_ck.Add(g_b_ck);
            g_ck.Add(g_g_ck);
            g_ck.Add(g_r_ck);
            r_ck.Add(r_a_ck);
            r_ck.Add(r_b_ck);
            r_ck.Add(r_g_ck);
            r_ck.Add(r_r_ck);
            magnification_ck.Add(mag_nearest_neighbour_ck);
            magnification_ck.Add(mag_linear_ck);
            magnification_ck.Add(mag_nearestmipmapnearest_ck);
            magnification_ck.Add(mag_nearestmipmaplinear_ck);
            magnification_ck.Add(mag_linearmipmapnearest_ck);
            magnification_ck.Add(mag_linearmipmaplinear_ck);
            minification_ck.Add(min_nearest_neighbour_ck);
            minification_ck.Add(min_linear_ck);
            minification_ck.Add(min_nearestmipmapnearest_ck);
            minification_ck.Add(min_nearestmipmaplinear_ck);
            minification_ck.Add(min_linearmipmapnearest_ck);
            minification_ck.Add(min_linearmipmaplinear_ck);
            WrapS_ck.Add(Sclamp_ck);
            WrapS_ck.Add(Srepeat_ck);
            WrapS_ck.Add(Smirror_ck);
            WrapT_ck.Add(Tclamp_ck);
            WrapT_ck.Add(Trepeat_ck);
            WrapT_ck.Add(Tmirror_ck);
            alpha_ck_array.Add(no_alpha_ck);
            alpha_ck_array.Add(alpha_ck);
            alpha_ck_array.Add(mix_ck);
            algorithm_ck.Add(cie_601_ck);
            algorithm_ck.Add(cie_709_ck);
            algorithm_ck.Add(custom_ck);
            algorithm_ck.Add(no_gradient_ck);
            if (System.IO.File.Exists(execPath + "images/background.png"))
            {
                this.BackgroundImage = System.Drawing.Image.FromFile(execPath + "images/background.png");
            }
            if (System.IO.File.Exists(execPath + "images/surrounding.png"))
            {
                surrounding_ck.BackgroundImage = System.Drawing.Image.FromFile(execPath + "images/surrounding.png");
            }
            if (System.IO.File.Exists(execPath + "images/white_box.png"))
            {
                white_box = System.Drawing.Image.FromFile(execPath + "images/white_box.png");
            }
            if (System.IO.File.Exists(execPath + "images/check.png"))
            {
                check = System.Drawing.Image.FromFile(execPath + "images/check.png");
            }
            if (System.IO.File.Exists(execPath + "images/light_blue_box.png"))
            {
                light_blue_box = System.Drawing.Image.FromFile(execPath + "images/light_blue_box.png");
            }
            if (System.IO.File.Exists(execPath + "images/light_blue_check.png"))
            {
                light_blue_check = System.Drawing.Image.FromFile(execPath + "images/light_blue_check.png");
            }
            if (System.IO.File.Exists(execPath + "images/pink_circle.png"))
            {
                pink_circle = System.Drawing.Image.FromFile(execPath + "images/pink_circle.png");
            }
            if (System.IO.File.Exists(execPath + "images/pink_circle_on.png"))
            {
                pink_circle_on = System.Drawing.Image.FromFile(execPath + "images/pink_circle_on.png");
            }
            if (System.IO.File.Exists(execPath + "images/violet_circle.png"))
            {
                violet_circle = System.Drawing.Image.FromFile(execPath + "images/violet_circle.png");
            }
            if (System.IO.File.Exists(execPath + "images/violet_circle_on.png"))
            {
                violet_circle_on = System.Drawing.Image.FromFile(execPath + "images/violet_circle_on.png");
            }
            if (System.IO.File.Exists(execPath + "images/chartreuse_circle.png"))
            {
                chartreuse_circle = System.Drawing.Image.FromFile(execPath + "images/chartreuse_circle.png");
            }
            if (System.IO.File.Exists(execPath + "images/chartreuse_circle_on.png"))
            {
                chartreuse_circle_on = System.Drawing.Image.FromFile(execPath + "images/chartreuse_circle_on.png");
            }
            if (System.IO.File.Exists(execPath + "images/green_circle.png"))
            {
                green_circle = System.Drawing.Image.FromFile(execPath + "images/green_circle.png");
            }
            if (System.IO.File.Exists(execPath + "images/green_circle_on.png"))
            {
                green_circle_on = System.Drawing.Image.FromFile(execPath + "images/green_circle_on.png");
            }
            if (System.IO.File.Exists(execPath + "images/light_blue_circle.png"))
            {
                light_blue_circle = System.Drawing.Image.FromFile(execPath + "images/light_blue_circle.png");
            }
            if (System.IO.File.Exists(execPath + "images/light_blue_circle_on.png"))
            {
                light_blue_circle_on = System.Drawing.Image.FromFile(execPath + "images/light_blue_circle_on.png");
            }
            if (System.IO.File.Exists(execPath + "images/blue_circle.png"))
            {
                blue_circle = System.Drawing.Image.FromFile(execPath + "images/blue_circle.png");
            }
            if (System.IO.File.Exists(execPath + "images/blue_circle_on.png"))
            {
                blue_circle_on = System.Drawing.Image.FromFile(execPath + "images/blue_circle_on.png");
            }
            if (System.IO.File.Exists(execPath + "images/light_red_circle.png"))
            {
                light_red_circle = System.Drawing.Image.FromFile(execPath + "images/light_red_circle.png");
            }
            if (System.IO.File.Exists(execPath + "images/light_red_circle_on.png"))
            {
                light_red_circle_on = System.Drawing.Image.FromFile(execPath + "images/light_red_circle_on.png");
            }
            if (System.IO.File.Exists(execPath + "images/red_circle.png"))
            {
                red_circle = System.Drawing.Image.FromFile(execPath + "images/red_circle.png");
            }
            if (System.IO.File.Exists(execPath + "images/red_circle_on.png"))
            {
                red_circle_on = System.Drawing.Image.FromFile(execPath + "images/red_circle_on.png");
            }
            if (System.IO.File.Exists(execPath + "images/yellow_circle.png"))
            {
                yellow_circle = System.Drawing.Image.FromFile(execPath + "images/yellow_circle.png");
            }
            if (System.IO.File.Exists(execPath + "images/yellow_circle_on.png"))
            {
                yellow_circle_on = System.Drawing.Image.FromFile(execPath + "images/yellow_circle_on.png");
            }
            if (System.IO.File.Exists(execPath + "images/orange_circle.png"))
            {
                orange_circle = System.Drawing.Image.FromFile(execPath + "images/orange_circle.png");
            }
            if (System.IO.File.Exists(execPath + "images/orange_circle_on.png"))
            {
                orange_circle_on = System.Drawing.Image.FromFile(execPath + "images/orange_circle_on.png");
            }
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
            unchecked_A(a_r_ck);
            unchecked_A(a_g_ck);
            unchecked_A(a_b_ck);
            checked_A(a_a_ck);
            unchecked_B(b_r_ck);
            unchecked_B(b_g_ck);
            checked_B(b_b_ck);
            unchecked_B(b_a_ck);
            unchecked_G(g_r_ck);
            checked_G(g_g_ck);
            unchecked_G(g_b_ck);
            unchecked_G(g_a_ck);
            checked_R(r_r_ck);
            unchecked_R(r_g_ck);
            unchecked_R(r_b_ck);
            unchecked_R(r_a_ck);
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
        }
        /*private void Change_font_normal()
        {
            font_normal = new System.Drawing.Font(fontname, 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
        }*/
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
            radiobutton.BackgroundImage = violet_circle;
        }
        private void checked_A(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = violet_circle_on;
        }
        private void hover_A(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle;
        }
        private void selected_A(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle_on;
        }
        private void unchecked_B(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = violet_circle;
        }
        private void checked_B(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = violet_circle_on;
        }
        private void hover_B(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle;
        }
        private void selected_B(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle_on;
        }
        private void unchecked_G(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = violet_circle;
        }
        private void checked_G(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = violet_circle_on;
        }
        private void hover_G(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle;
        }
        private void selected_G(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle_on;
        }
        private void unchecked_R(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = violet_circle;
        }
        private void checked_R(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = violet_circle_on;
        }
        private void hover_R(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle;
        }
        private void selected_R(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle_on;
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
            radiobutton.BackgroundImage = chartreuse_circle;
        }
        private void selected_WrapT(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = chartreuse_circle_on;
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
            radiobutton.BackgroundImage = violet_circle;
        }
        private void checked_alpha(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = violet_circle_on;
        }
        private void hover_alpha(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle;
        }
        private void selected_alpha(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle_on;
        }
        private void unchecked_algorithm(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = violet_circle;
        }
        private void checked_algorithm(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = violet_circle_on;
        }
        private void hover_algorithm(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle;
        }
        private void selected_algorithm(PictureBox radiobutton)
        {
            radiobutton.BackgroundImage = pink_circle_on;
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
            this.SuspendLayout();
            // 
            // output_file_type_label
            // 
            this.output_file_type_label.AutoSize = true;
            this.output_file_type_label.BackColor = System.Drawing.Color.Transparent;
            this.output_file_type_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.output_file_type_label.ForeColor = System.Drawing.SystemColors.Control;
            this.output_file_type_label.Location = new System.Drawing.Point(16, 84);
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
            this.mandatory_settings_label.Location = new System.Drawing.Point(115, 48);
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
            this.bmd_label.Location = new System.Drawing.Point(109, 114);
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
            this.bmd_hitbox.Location = new System.Drawing.Point(38, 114);
            this.bmd_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.bmd_hitbox.Name = "bmd_hitbox";
            this.bmd_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
            this.bmd_hitbox.Size = new System.Drawing.Size(128, 68);
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
            this.bmd_ck.Location = new System.Drawing.Point(41, 114);
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
            this.bti_ck.Location = new System.Drawing.Point(41, 178);
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
            this.bti_label.Location = new System.Drawing.Point(109, 178);
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
            this.bti_hitbox.Location = new System.Drawing.Point(38, 178);
            this.bti_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.bti_hitbox.Name = "bti_hitbox";
            this.bti_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
            this.bti_hitbox.Size = new System.Drawing.Size(128, 68);
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
            this.tex0_ck.Location = new System.Drawing.Point(41, 242);
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
            this.tex0_label.Location = new System.Drawing.Point(109, 243);
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
            this.tex0_hitbox.Location = new System.Drawing.Point(38, 242);
            this.tex0_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.tex0_hitbox.Name = "tex0_hitbox";
            this.tex0_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
            this.tex0_hitbox.Size = new System.Drawing.Size(128, 68);
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
            this.tpl_ck.Location = new System.Drawing.Point(41, 306);
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
            this.tpl_label.Location = new System.Drawing.Point(109, 307);
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
            this.tpl_hitbox.Location = new System.Drawing.Point(38, 306);
            this.tpl_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.tpl_hitbox.Name = "tpl_hitbox";
            this.tpl_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
            this.tpl_hitbox.Size = new System.Drawing.Size(128, 68);
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
            this.bmp_ck.Location = new System.Drawing.Point(41, 370);
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
            this.bmp_label.Location = new System.Drawing.Point(109, 370);
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
            this.bmp_hitbox.Location = new System.Drawing.Point(38, 370);
            this.bmp_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_hitbox.Name = "bmp_hitbox";
            this.bmp_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
            this.bmp_hitbox.Size = new System.Drawing.Size(128, 68);
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
            this.png_ck.Location = new System.Drawing.Point(41, 434);
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
            this.png_label.Location = new System.Drawing.Point(109, 435);
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
            this.png_hitbox.Location = new System.Drawing.Point(38, 434);
            this.png_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.png_hitbox.Name = "png_hitbox";
            this.png_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
            this.png_hitbox.Size = new System.Drawing.Size(128, 68);
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
            this.jpg_ck.Location = new System.Drawing.Point(41, 498);
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
            this.jpg_label.Location = new System.Drawing.Point(109, 499);
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
            this.jpg_hitbox.Location = new System.Drawing.Point(38, 498);
            this.jpg_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.jpg_hitbox.Name = "jpg_hitbox";
            this.jpg_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
            this.jpg_hitbox.Size = new System.Drawing.Size(128, 68);
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
            this.tiff_ck.Location = new System.Drawing.Point(41, 819);
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
            this.tiff_label.Location = new System.Drawing.Point(109, 820);
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
            this.tiff_hitbox.Location = new System.Drawing.Point(38, 819);
            this.tiff_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.tiff_hitbox.Name = "tiff_hitbox";
            this.tiff_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
            this.tiff_hitbox.Size = new System.Drawing.Size(128, 68);
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
            this.tif_ck.Location = new System.Drawing.Point(41, 755);
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
            this.tif_label.Location = new System.Drawing.Point(109, 756);
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
            this.tif_hitbox.Location = new System.Drawing.Point(38, 755);
            this.tif_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.tif_hitbox.Name = "tif_hitbox";
            this.tif_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
            this.tif_hitbox.Size = new System.Drawing.Size(128, 68);
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
            this.ico_ck.Location = new System.Drawing.Point(41, 691);
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
            this.ico_label.Location = new System.Drawing.Point(109, 691);
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
            this.ico_hitbox.Location = new System.Drawing.Point(38, 691);
            this.ico_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ico_hitbox.Name = "ico_hitbox";
            this.ico_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
            this.ico_hitbox.Size = new System.Drawing.Size(128, 68);
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
            this.gif_ck.Location = new System.Drawing.Point(41, 627);
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
            this.gib_label.Location = new System.Drawing.Point(109, 628);
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
            this.gif_hitbox.Location = new System.Drawing.Point(38, 627);
            this.gif_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.gif_hitbox.Name = "gif_hitbox";
            this.gif_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
            this.gif_hitbox.Size = new System.Drawing.Size(128, 68);
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
            this.jpeg_ck.Location = new System.Drawing.Point(41, 563);
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
            this.jpeg_label.Location = new System.Drawing.Point(109, 564);
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
            this.jpeg_hitbox.Location = new System.Drawing.Point(38, 563);
            this.jpeg_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.jpeg_hitbox.Name = "jpeg_hitbox";
            this.jpeg_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
            this.jpeg_hitbox.Size = new System.Drawing.Size(128, 68);
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
            this.options_label.Location = new System.Drawing.Point(1306, 86);
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
            this.warn_ck.Location = new System.Drawing.Point(1279, 692);
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
            this.warn_label.Location = new System.Drawing.Point(1348, 693);
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
            this.stfu_ck.Location = new System.Drawing.Point(1279, 628);
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
            this.stfu_label.Location = new System.Drawing.Point(1348, 630);
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
            this.safe_mode_ck.Location = new System.Drawing.Point(1279, 564);
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
            this.safe_mode_label.Location = new System.Drawing.Point(1348, 566);
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
            this.reverse_ck.Location = new System.Drawing.Point(1279, 499);
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
            this.reverse_label.Location = new System.Drawing.Point(1348, 501);
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
            this.random_ck.Location = new System.Drawing.Point(1279, 435);
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
            this.random_label.Location = new System.Drawing.Point(1348, 437);
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
            this.no_warning_ck.Location = new System.Drawing.Point(1279, 371);
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
            this.no_warning_label.Location = new System.Drawing.Point(1348, 372);
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
            this.funky_ck.Location = new System.Drawing.Point(1279, 307);
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
            this.funky_label.Location = new System.Drawing.Point(1348, 309);
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
            this.FORCE_ALPHA_ck.Location = new System.Drawing.Point(1279, 243);
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
            this.FORCE_ALPHA_label.Location = new System.Drawing.Point(1348, 245);
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
            this.bmp_32_ck.Location = new System.Drawing.Point(1279, 179);
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
            this.bmp_32_label.Location = new System.Drawing.Point(1348, 180);
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
            this.ask_exit_ck.Location = new System.Drawing.Point(1279, 115);
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
            this.ask_exit_label.Location = new System.Drawing.Point(1348, 116);
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
            this.ask_exit_hitbox.Location = new System.Drawing.Point(1281, 116);
            this.ask_exit_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ask_exit_hitbox.Name = "ask_exit_hitbox";
            this.ask_exit_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.ask_exit_hitbox.Size = new System.Drawing.Size(250, 68);
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
            this.bmp_32_hitbox.Location = new System.Drawing.Point(1281, 179);
            this.bmp_32_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_32_hitbox.Name = "bmp_32_hitbox";
            this.bmp_32_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.bmp_32_hitbox.Size = new System.Drawing.Size(250, 68);
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
            this.FORCE_ALPHA_hitbox.Location = new System.Drawing.Point(1281, 243);
            this.FORCE_ALPHA_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.FORCE_ALPHA_hitbox.Name = "FORCE_ALPHA_hitbox";
            this.FORCE_ALPHA_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.FORCE_ALPHA_hitbox.Size = new System.Drawing.Size(250, 68);
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
            this.funky_hitbox.Location = new System.Drawing.Point(1281, 307);
            this.funky_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.funky_hitbox.Name = "funky_hitbox";
            this.funky_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.funky_hitbox.Size = new System.Drawing.Size(250, 68);
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
            this.no_warning_hitbox.Location = new System.Drawing.Point(1281, 372);
            this.no_warning_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.no_warning_hitbox.Name = "no_warning_hitbox";
            this.no_warning_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.no_warning_hitbox.Size = new System.Drawing.Size(250, 68);
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
            this.random_hitbox.Location = new System.Drawing.Point(1281, 435);
            this.random_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.random_hitbox.Name = "random_hitbox";
            this.random_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.random_hitbox.Size = new System.Drawing.Size(250, 68);
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
            this.reverse_hitbox.Location = new System.Drawing.Point(1281, 499);
            this.reverse_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.reverse_hitbox.Name = "reverse_hitbox";
            this.reverse_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.reverse_hitbox.Size = new System.Drawing.Size(250, 68);
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
            this.safe_mode_hitbox.Location = new System.Drawing.Point(1281, 566);
            this.safe_mode_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.safe_mode_hitbox.Name = "safe_mode_hitbox";
            this.safe_mode_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.safe_mode_hitbox.Size = new System.Drawing.Size(250, 68);
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
            this.stfu_hitbox.Location = new System.Drawing.Point(1281, 629);
            this.stfu_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.stfu_hitbox.Name = "stfu_hitbox";
            this.stfu_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.stfu_hitbox.Size = new System.Drawing.Size(250, 68);
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
            this.warn_hitbox.Location = new System.Drawing.Point(1281, 692);
            this.warn_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.warn_hitbox.Name = "warn_hitbox";
            this.warn_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.warn_hitbox.Size = new System.Drawing.Size(250, 68);
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
            this.cmpr_ck.Location = new System.Drawing.Point(241, 755);
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
            this.cmpr_label.Location = new System.Drawing.Point(309, 756);
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
            this.cmpr_hitbox.Location = new System.Drawing.Point(238, 755);
            this.cmpr_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_hitbox.Name = "cmpr_hitbox";
            this.cmpr_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.cmpr_hitbox.Size = new System.Drawing.Size(190, 68);
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
            this.ci14x2_ck.Location = new System.Drawing.Point(241, 691);
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
            this.ci14x2_label.Location = new System.Drawing.Point(309, 691);
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
            this.ci14x2_hitbox.Location = new System.Drawing.Point(238, 691);
            this.ci14x2_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ci14x2_hitbox.Name = "ci14x2_hitbox";
            this.ci14x2_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.ci14x2_hitbox.Size = new System.Drawing.Size(190, 68);
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
            this.ci8_ck.Location = new System.Drawing.Point(241, 627);
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
            this.ci8_label.Location = new System.Drawing.Point(309, 628);
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
            this.ci8_hitbox.Location = new System.Drawing.Point(238, 627);
            this.ci8_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ci8_hitbox.Name = "ci8_hitbox";
            this.ci8_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.ci8_hitbox.Size = new System.Drawing.Size(190, 68);
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
            this.ci4_ck.Location = new System.Drawing.Point(241, 563);
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
            this.ci4_label.Location = new System.Drawing.Point(309, 564);
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
            this.ci4_hitbox.Location = new System.Drawing.Point(238, 563);
            this.ci4_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ci4_hitbox.Name = "ci4_hitbox";
            this.ci4_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.ci4_hitbox.Size = new System.Drawing.Size(190, 68);
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
            this.rgba32_ck.Location = new System.Drawing.Point(241, 498);
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
            this.rgba32_label.Location = new System.Drawing.Point(309, 499);
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
            this.rgba32_hitbox.Location = new System.Drawing.Point(238, 498);
            this.rgba32_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.rgba32_hitbox.Name = "rgba32_hitbox";
            this.rgba32_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.rgba32_hitbox.Size = new System.Drawing.Size(190, 68);
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
            this.rgb5a3_ck.Location = new System.Drawing.Point(241, 434);
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
            this.rgb5a3_label.Location = new System.Drawing.Point(309, 435);
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
            this.rgb5a3_hitbox.Location = new System.Drawing.Point(238, 434);
            this.rgb5a3_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.rgb5a3_hitbox.Name = "rgb5a3_hitbox";
            this.rgb5a3_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.rgb5a3_hitbox.Size = new System.Drawing.Size(190, 68);
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
            this.rgb565_ck.Location = new System.Drawing.Point(241, 370);
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
            this.rgb565_label.Location = new System.Drawing.Point(309, 370);
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
            this.rgb565_hitbox.Location = new System.Drawing.Point(238, 370);
            this.rgb565_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.rgb565_hitbox.Name = "rgb565_hitbox";
            this.rgb565_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.rgb565_hitbox.Size = new System.Drawing.Size(190, 68);
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
            this.ai8_ck.Location = new System.Drawing.Point(241, 306);
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
            this.ai8_label.Location = new System.Drawing.Point(309, 307);
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
            this.ai8_hitbox.Location = new System.Drawing.Point(238, 306);
            this.ai8_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ai8_hitbox.Name = "ai8_hitbox";
            this.ai8_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.ai8_hitbox.Size = new System.Drawing.Size(190, 68);
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
            this.ai4_ck.Location = new System.Drawing.Point(241, 242);
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
            this.ai4_label.Location = new System.Drawing.Point(309, 243);
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
            this.ai4_hitbox.Location = new System.Drawing.Point(238, 242);
            this.ai4_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ai4_hitbox.Name = "ai4_hitbox";
            this.ai4_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.ai4_hitbox.Size = new System.Drawing.Size(190, 68);
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
            this.i8_ck.Location = new System.Drawing.Point(241, 178);
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
            this.i8_label.Location = new System.Drawing.Point(309, 178);
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
            this.i8_hitbox.Location = new System.Drawing.Point(238, 178);
            this.i8_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.i8_hitbox.Name = "i8_hitbox";
            this.i8_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.i8_hitbox.Size = new System.Drawing.Size(190, 68);
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
            this.i4_ck.Location = new System.Drawing.Point(241, 114);
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
            this.i4_label.Location = new System.Drawing.Point(309, 114);
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
            this.i4_hitbox.Location = new System.Drawing.Point(238, 114);
            this.i4_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.i4_hitbox.Name = "i4_hitbox";
            this.i4_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.i4_hitbox.Size = new System.Drawing.Size(190, 68);
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
            this.encoding_label.Location = new System.Drawing.Point(289, 84);
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
            this.surrounding_ck.Location = new System.Drawing.Point(10, 58);
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
            this.no_gradient_ck.Location = new System.Drawing.Point(493, 306);
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
            this.no_gradient_label.Location = new System.Drawing.Point(561, 307);
            this.no_gradient_label.Margin = new System.Windows.Forms.Padding(0);
            this.no_gradient_label.Name = "no_gradient_label";
            this.no_gradient_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.no_gradient_label.Size = new System.Drawing.Size(148, 64);
            this.no_gradient_label.TabIndex = 245;
            this.no_gradient_label.Text = "No Gradient";
            // 
            // no_gradient_hitbox
            // 
            this.no_gradient_hitbox.AutoSize = true;
            this.no_gradient_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.no_gradient_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.no_gradient_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.no_gradient_hitbox.Location = new System.Drawing.Point(490, 306);
            this.no_gradient_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.no_gradient_hitbox.Name = "no_gradient_hitbox";
            this.no_gradient_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.no_gradient_hitbox.Size = new System.Drawing.Size(250, 68);
            this.no_gradient_hitbox.TabIndex = 246;
            // 
            // custom_ck
            // 
            this.custom_ck.BackColor = System.Drawing.Color.Transparent;
            this.custom_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.custom_ck.Enabled = false;
            this.custom_ck.ErrorImage = null;
            this.custom_ck.InitialImage = null;
            this.custom_ck.Location = new System.Drawing.Point(493, 242);
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
            this.custom_label.Location = new System.Drawing.Point(561, 243);
            this.custom_label.Margin = new System.Windows.Forms.Padding(0);
            this.custom_label.Name = "custom_label";
            this.custom_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.custom_label.Size = new System.Drawing.Size(174, 64);
            this.custom_label.TabIndex = 242;
            this.custom_label.Text = "Custom RGBA";
            // 
            // custom_hitbox
            // 
            this.custom_hitbox.AutoSize = true;
            this.custom_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.custom_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.custom_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.custom_hitbox.Location = new System.Drawing.Point(490, 242);
            this.custom_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.custom_hitbox.Name = "custom_hitbox";
            this.custom_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.custom_hitbox.Size = new System.Drawing.Size(250, 68);
            this.custom_hitbox.TabIndex = 243;
            // 
            // cie_709_ck
            // 
            this.cie_709_ck.BackColor = System.Drawing.Color.Transparent;
            this.cie_709_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cie_709_ck.Enabled = false;
            this.cie_709_ck.ErrorImage = null;
            this.cie_709_ck.InitialImage = null;
            this.cie_709_ck.Location = new System.Drawing.Point(493, 178);
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
            this.cie_709_label.Location = new System.Drawing.Point(561, 178);
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
            this.cie_709_hitbox.Location = new System.Drawing.Point(490, 178);
            this.cie_709_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.cie_709_hitbox.Name = "cie_709_hitbox";
            this.cie_709_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.cie_709_hitbox.Size = new System.Drawing.Size(250, 68);
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
            this.cie_601_ck.Location = new System.Drawing.Point(493, 114);
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
            this.cie_601_label.Location = new System.Drawing.Point(561, 114);
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
            this.cie_601_hitbox.Location = new System.Drawing.Point(490, 114);
            this.cie_601_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.cie_601_hitbox.Name = "cie_601_hitbox";
            this.cie_601_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.cie_601_hitbox.Size = new System.Drawing.Size(250, 68);
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
            this.algorithm_label.Location = new System.Drawing.Point(541, 84);
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
            this.mix_ck.Location = new System.Drawing.Point(493, 562);
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
            this.mix_label.Location = new System.Drawing.Point(561, 563);
            this.mix_label.Margin = new System.Windows.Forms.Padding(0);
            this.mix_label.Name = "mix_label";
            this.mix_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mix_label.Size = new System.Drawing.Size(47, 64);
            this.mix_label.TabIndex = 255;
            this.mix_label.Text = "Mix";
            // 
            // mix_hitbox
            // 
            this.mix_hitbox.AutoSize = true;
            this.mix_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mix_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mix_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mix_hitbox.Location = new System.Drawing.Point(490, 562);
            this.mix_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mix_hitbox.Name = "mix_hitbox";
            this.mix_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.mix_hitbox.Size = new System.Drawing.Size(190, 68);
            this.mix_hitbox.TabIndex = 256;
            // 
            // alpha_ck
            // 
            this.alpha_ck.BackColor = System.Drawing.Color.Transparent;
            this.alpha_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.alpha_ck.Enabled = false;
            this.alpha_ck.ErrorImage = null;
            this.alpha_ck.InitialImage = null;
            this.alpha_ck.Location = new System.Drawing.Point(493, 498);
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
            this.alpha_label.Location = new System.Drawing.Point(561, 498);
            this.alpha_label.Margin = new System.Windows.Forms.Padding(0);
            this.alpha_label.Name = "alpha_label";
            this.alpha_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.alpha_label.Size = new System.Drawing.Size(74, 64);
            this.alpha_label.TabIndex = 252;
            this.alpha_label.Text = "Alpha";
            // 
            // alpha_hitbox
            // 
            this.alpha_hitbox.AutoSize = true;
            this.alpha_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.alpha_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.alpha_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.alpha_hitbox.Location = new System.Drawing.Point(490, 498);
            this.alpha_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.alpha_hitbox.Name = "alpha_hitbox";
            this.alpha_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.alpha_hitbox.Size = new System.Drawing.Size(190, 68);
            this.alpha_hitbox.TabIndex = 253;
            // 
            // no_alpha_ck
            // 
            this.no_alpha_ck.BackColor = System.Drawing.Color.Transparent;
            this.no_alpha_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.no_alpha_ck.Enabled = false;
            this.no_alpha_ck.ErrorImage = null;
            this.no_alpha_ck.InitialImage = null;
            this.no_alpha_ck.Location = new System.Drawing.Point(493, 434);
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
            this.no_alpha_label.Location = new System.Drawing.Point(561, 434);
            this.no_alpha_label.Margin = new System.Windows.Forms.Padding(0);
            this.no_alpha_label.Name = "no_alpha_label";
            this.no_alpha_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.no_alpha_label.Size = new System.Drawing.Size(115, 64);
            this.no_alpha_label.TabIndex = 249;
            this.no_alpha_label.Text = "No Alpha";
            // 
            // no_alpha_hitbox
            // 
            this.no_alpha_hitbox.AutoSize = true;
            this.no_alpha_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.no_alpha_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.no_alpha_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.no_alpha_hitbox.Location = new System.Drawing.Point(490, 434);
            this.no_alpha_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.no_alpha_hitbox.Name = "no_alpha_hitbox";
            this.no_alpha_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.no_alpha_hitbox.Size = new System.Drawing.Size(190, 68);
            this.no_alpha_hitbox.TabIndex = 250;
            // 
            // alpha_title
            // 
            this.alpha_title.AutoSize = true;
            this.alpha_title.BackColor = System.Drawing.Color.Transparent;
            this.alpha_title.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.alpha_title.ForeColor = System.Drawing.SystemColors.Control;
            this.alpha_title.Location = new System.Drawing.Point(541, 404);
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
            this.Tmirror_ck.Location = new System.Drawing.Point(873, 563);
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
            this.Tmirror_label.Location = new System.Drawing.Point(941, 564);
            this.Tmirror_label.Margin = new System.Windows.Forms.Padding(0);
            this.Tmirror_label.Name = "Tmirror_label";
            this.Tmirror_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Tmirror_label.Size = new System.Drawing.Size(78, 64);
            this.Tmirror_label.TabIndex = 265;
            this.Tmirror_label.Text = "Mirror";
            // 
            // Tmirror_hitbox
            // 
            this.Tmirror_hitbox.AutoSize = true;
            this.Tmirror_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.Tmirror_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Tmirror_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.Tmirror_hitbox.Location = new System.Drawing.Point(870, 563);
            this.Tmirror_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.Tmirror_hitbox.Name = "Tmirror_hitbox";
            this.Tmirror_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.Tmirror_hitbox.Size = new System.Drawing.Size(190, 68);
            this.Tmirror_hitbox.TabIndex = 266;
            // 
            // Trepeat_ck
            // 
            this.Trepeat_ck.BackColor = System.Drawing.Color.Transparent;
            this.Trepeat_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Trepeat_ck.Enabled = false;
            this.Trepeat_ck.ErrorImage = null;
            this.Trepeat_ck.InitialImage = null;
            this.Trepeat_ck.Location = new System.Drawing.Point(873, 499);
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
            this.Trepeat_label.Location = new System.Drawing.Point(941, 499);
            this.Trepeat_label.Margin = new System.Windows.Forms.Padding(0);
            this.Trepeat_label.Name = "Trepeat_label";
            this.Trepeat_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Trepeat_label.Size = new System.Drawing.Size(92, 64);
            this.Trepeat_label.TabIndex = 262;
            this.Trepeat_label.Text = "Repeat";
            // 
            // Trepeat_hitbox
            // 
            this.Trepeat_hitbox.AutoSize = true;
            this.Trepeat_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.Trepeat_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Trepeat_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.Trepeat_hitbox.Location = new System.Drawing.Point(870, 499);
            this.Trepeat_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.Trepeat_hitbox.Name = "Trepeat_hitbox";
            this.Trepeat_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.Trepeat_hitbox.Size = new System.Drawing.Size(190, 68);
            this.Trepeat_hitbox.TabIndex = 263;
            // 
            // Tclamp_ck
            // 
            this.Tclamp_ck.BackColor = System.Drawing.Color.Transparent;
            this.Tclamp_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Tclamp_ck.Enabled = false;
            this.Tclamp_ck.ErrorImage = null;
            this.Tclamp_ck.InitialImage = null;
            this.Tclamp_ck.Location = new System.Drawing.Point(873, 435);
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
            this.Tclamp_label.Location = new System.Drawing.Point(941, 435);
            this.Tclamp_label.Margin = new System.Windows.Forms.Padding(0);
            this.Tclamp_label.Name = "Tclamp_label";
            this.Tclamp_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Tclamp_label.Size = new System.Drawing.Size(80, 64);
            this.Tclamp_label.TabIndex = 259;
            this.Tclamp_label.Text = "Clamp";
            // 
            // Tclamp_hitbox
            // 
            this.Tclamp_hitbox.AutoSize = true;
            this.Tclamp_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.Tclamp_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Tclamp_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.Tclamp_hitbox.Location = new System.Drawing.Point(870, 435);
            this.Tclamp_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.Tclamp_hitbox.Name = "Tclamp_hitbox";
            this.Tclamp_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.Tclamp_hitbox.Size = new System.Drawing.Size(190, 68);
            this.Tclamp_hitbox.TabIndex = 260;
            // 
            // WrapT_label
            // 
            this.WrapT_label.AutoSize = true;
            this.WrapT_label.BackColor = System.Drawing.Color.Transparent;
            this.WrapT_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.WrapT_label.ForeColor = System.Drawing.SystemColors.Control;
            this.WrapT_label.Location = new System.Drawing.Point(921, 405);
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
            this.Smirror_ck.Location = new System.Drawing.Point(873, 243);
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
            this.Smirror_label.Location = new System.Drawing.Point(941, 244);
            this.Smirror_label.Margin = new System.Windows.Forms.Padding(0);
            this.Smirror_label.Name = "Smirror_label";
            this.Smirror_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Smirror_label.Size = new System.Drawing.Size(78, 64);
            this.Smirror_label.TabIndex = 275;
            this.Smirror_label.Text = "Mirror";
            // 
            // Smirror_hitbox
            // 
            this.Smirror_hitbox.AutoSize = true;
            this.Smirror_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.Smirror_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Smirror_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.Smirror_hitbox.Location = new System.Drawing.Point(870, 243);
            this.Smirror_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.Smirror_hitbox.Name = "Smirror_hitbox";
            this.Smirror_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.Smirror_hitbox.Size = new System.Drawing.Size(190, 68);
            this.Smirror_hitbox.TabIndex = 276;
            // 
            // Srepeat_ck
            // 
            this.Srepeat_ck.BackColor = System.Drawing.Color.Transparent;
            this.Srepeat_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Srepeat_ck.Enabled = false;
            this.Srepeat_ck.ErrorImage = null;
            this.Srepeat_ck.InitialImage = null;
            this.Srepeat_ck.Location = new System.Drawing.Point(873, 179);
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
            this.Srepeat_label.Location = new System.Drawing.Point(941, 179);
            this.Srepeat_label.Margin = new System.Windows.Forms.Padding(0);
            this.Srepeat_label.Name = "Srepeat_label";
            this.Srepeat_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Srepeat_label.Size = new System.Drawing.Size(92, 64);
            this.Srepeat_label.TabIndex = 272;
            this.Srepeat_label.Text = "Repeat";
            // 
            // Srepeat_hitbox
            // 
            this.Srepeat_hitbox.AutoSize = true;
            this.Srepeat_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.Srepeat_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Srepeat_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.Srepeat_hitbox.Location = new System.Drawing.Point(870, 179);
            this.Srepeat_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.Srepeat_hitbox.Name = "Srepeat_hitbox";
            this.Srepeat_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.Srepeat_hitbox.Size = new System.Drawing.Size(190, 68);
            this.Srepeat_hitbox.TabIndex = 273;
            // 
            // Sclamp_ck
            // 
            this.Sclamp_ck.BackColor = System.Drawing.Color.Transparent;
            this.Sclamp_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Sclamp_ck.Enabled = false;
            this.Sclamp_ck.ErrorImage = null;
            this.Sclamp_ck.InitialImage = null;
            this.Sclamp_ck.Location = new System.Drawing.Point(873, 115);
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
            this.Sclamp_label.Location = new System.Drawing.Point(941, 115);
            this.Sclamp_label.Margin = new System.Windows.Forms.Padding(0);
            this.Sclamp_label.Name = "Sclamp_label";
            this.Sclamp_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.Sclamp_label.Size = new System.Drawing.Size(80, 64);
            this.Sclamp_label.TabIndex = 269;
            this.Sclamp_label.Text = "Clamp";
            // 
            // Sclamp_hitbox
            // 
            this.Sclamp_hitbox.AutoSize = true;
            this.Sclamp_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.Sclamp_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.Sclamp_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.Sclamp_hitbox.Location = new System.Drawing.Point(870, 115);
            this.Sclamp_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.Sclamp_hitbox.Name = "Sclamp_hitbox";
            this.Sclamp_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
            this.Sclamp_hitbox.Size = new System.Drawing.Size(190, 68);
            this.Sclamp_hitbox.TabIndex = 270;
            // 
            // WrapS_label
            // 
            this.WrapS_label.AutoSize = true;
            this.WrapS_label.BackColor = System.Drawing.Color.Transparent;
            this.WrapS_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.WrapS_label.ForeColor = System.Drawing.SystemColors.Control;
            this.WrapS_label.Location = new System.Drawing.Point(921, 85);
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
            this.magnification_label.Location = new System.Drawing.Point(892, 661);
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
            this.minification_label.Location = new System.Drawing.Point(519, 661);
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
            this.min_linearmipmaplinear_ck.Location = new System.Drawing.Point(493, 1007);
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
            this.min_linearmipmaplinear_label.Location = new System.Drawing.Point(561, 1008);
            this.min_linearmipmaplinear_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmaplinear_label.Name = "min_linearmipmaplinear_label";
            this.min_linearmipmaplinear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_linearmipmaplinear_label.Size = new System.Drawing.Size(240, 64);
            this.min_linearmipmaplinear_label.TabIndex = 322;
            this.min_linearmipmaplinear_label.Text = "LinearMipmapLinear";
            // 
            // min_linearmipmaplinear_hitbox
            // 
            this.min_linearmipmaplinear_hitbox.AutoSize = true;
            this.min_linearmipmaplinear_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmaplinear_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_linearmipmaplinear_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.min_linearmipmaplinear_hitbox.Location = new System.Drawing.Point(490, 1007);
            this.min_linearmipmaplinear_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmaplinear_hitbox.Name = "min_linearmipmaplinear_hitbox";
            this.min_linearmipmaplinear_hitbox.Padding = new System.Windows.Forms.Padding(350, 48, 0, 0);
            this.min_linearmipmaplinear_hitbox.Size = new System.Drawing.Size(350, 68);
            this.min_linearmipmaplinear_hitbox.TabIndex = 323;
            // 
            // min_linearmipmapnearest_ck
            // 
            this.min_linearmipmapnearest_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmapnearest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_linearmipmapnearest_ck.Enabled = false;
            this.min_linearmipmapnearest_ck.ErrorImage = null;
            this.min_linearmipmapnearest_ck.InitialImage = null;
            this.min_linearmipmapnearest_ck.Location = new System.Drawing.Point(493, 948);
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
            this.min_linearmipmapnearest_label.Location = new System.Drawing.Point(561, 949);
            this.min_linearmipmapnearest_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmapnearest_label.Name = "min_linearmipmapnearest_label";
            this.min_linearmipmapnearest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_linearmipmapnearest_label.Size = new System.Drawing.Size(280, 64);
            this.min_linearmipmapnearest_label.TabIndex = 319;
            this.min_linearmipmapnearest_label.Text = "LinearMipmapNearest  ";
            // 
            // min_linearmipmapnearest_hitbox
            // 
            this.min_linearmipmapnearest_hitbox.AutoSize = true;
            this.min_linearmipmapnearest_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.min_linearmipmapnearest_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_linearmipmapnearest_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.min_linearmipmapnearest_hitbox.Location = new System.Drawing.Point(490, 948);
            this.min_linearmipmapnearest_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.min_linearmipmapnearest_hitbox.Name = "min_linearmipmapnearest_hitbox";
            this.min_linearmipmapnearest_hitbox.Padding = new System.Windows.Forms.Padding(350, 48, 0, 0);
            this.min_linearmipmapnearest_hitbox.Size = new System.Drawing.Size(350, 68);
            this.min_linearmipmapnearest_hitbox.TabIndex = 320;
            // 
            // min_nearestmipmaplinear_ck
            // 
            this.min_nearestmipmaplinear_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmaplinear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_nearestmipmaplinear_ck.Enabled = false;
            this.min_nearestmipmaplinear_ck.ErrorImage = null;
            this.min_nearestmipmaplinear_ck.InitialImage = null;
            this.min_nearestmipmaplinear_ck.Location = new System.Drawing.Point(493, 884);
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
            this.min_nearestmipmaplinear_label.Location = new System.Drawing.Point(561, 885);
            this.min_nearestmipmaplinear_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmaplinear_label.Name = "min_nearestmipmaplinear_label";
            this.min_nearestmipmaplinear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_nearestmipmaplinear_label.Size = new System.Drawing.Size(280, 64);
            this.min_nearestmipmaplinear_label.TabIndex = 316;
            this.min_nearestmipmaplinear_label.Text = "NearestMipmapLinear  ";
            // 
            // min_nearestmipmaplinear_hitbox
            // 
            this.min_nearestmipmaplinear_hitbox.AutoSize = true;
            this.min_nearestmipmaplinear_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmaplinear_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_nearestmipmaplinear_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.min_nearestmipmaplinear_hitbox.Location = new System.Drawing.Point(490, 884);
            this.min_nearestmipmaplinear_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmaplinear_hitbox.Name = "min_nearestmipmaplinear_hitbox";
            this.min_nearestmipmaplinear_hitbox.Padding = new System.Windows.Forms.Padding(350, 48, 0, 0);
            this.min_nearestmipmaplinear_hitbox.Size = new System.Drawing.Size(350, 68);
            this.min_nearestmipmaplinear_hitbox.TabIndex = 317;
            // 
            // min_nearestmipmapnearest_ck
            // 
            this.min_nearestmipmapnearest_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmapnearest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_nearestmipmapnearest_ck.Enabled = false;
            this.min_nearestmipmapnearest_ck.ErrorImage = null;
            this.min_nearestmipmapnearest_ck.InitialImage = null;
            this.min_nearestmipmapnearest_ck.Location = new System.Drawing.Point(493, 820);
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
            this.min_nearestmipmapnearest_label.Location = new System.Drawing.Point(561, 821);
            this.min_nearestmipmapnearest_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmapnearest_label.Name = "min_nearestmipmapnearest_label";
            this.min_nearestmipmapnearest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_nearestmipmapnearest_label.Size = new System.Drawing.Size(290, 64);
            this.min_nearestmipmapnearest_label.TabIndex = 313;
            this.min_nearestmipmapnearest_label.Text = "NearestMipmapNearest ";
            // 
            // min_nearestmipmapnearest_hitbox
            // 
            this.min_nearestmipmapnearest_hitbox.AutoSize = true;
            this.min_nearestmipmapnearest_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.min_nearestmipmapnearest_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_nearestmipmapnearest_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.min_nearestmipmapnearest_hitbox.Location = new System.Drawing.Point(490, 820);
            this.min_nearestmipmapnearest_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearestmipmapnearest_hitbox.Name = "min_nearestmipmapnearest_hitbox";
            this.min_nearestmipmapnearest_hitbox.Padding = new System.Windows.Forms.Padding(350, 48, 0, 0);
            this.min_nearestmipmapnearest_hitbox.Size = new System.Drawing.Size(350, 68);
            this.min_nearestmipmapnearest_hitbox.TabIndex = 314;
            // 
            // min_linear_ck
            // 
            this.min_linear_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_linear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_linear_ck.Enabled = false;
            this.min_linear_ck.ErrorImage = null;
            this.min_linear_ck.InitialImage = null;
            this.min_linear_ck.Location = new System.Drawing.Point(493, 756);
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
            this.min_linear_label.Location = new System.Drawing.Point(561, 756);
            this.min_linear_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_linear_label.Name = "min_linear_label";
            this.min_linear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_linear_label.Size = new System.Drawing.Size(81, 64);
            this.min_linear_label.TabIndex = 310;
            this.min_linear_label.Text = "Linear";
            // 
            // min_linear_hitbox
            // 
            this.min_linear_hitbox.AutoSize = true;
            this.min_linear_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.min_linear_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_linear_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.min_linear_hitbox.Location = new System.Drawing.Point(490, 756);
            this.min_linear_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.min_linear_hitbox.Name = "min_linear_hitbox";
            this.min_linear_hitbox.Padding = new System.Windows.Forms.Padding(350, 48, 0, 0);
            this.min_linear_hitbox.Size = new System.Drawing.Size(350, 68);
            this.min_linear_hitbox.TabIndex = 311;
            // 
            // min_nearest_neighbour_ck
            // 
            this.min_nearest_neighbour_ck.BackColor = System.Drawing.Color.Transparent;
            this.min_nearest_neighbour_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.min_nearest_neighbour_ck.Enabled = false;
            this.min_nearest_neighbour_ck.ErrorImage = null;
            this.min_nearest_neighbour_ck.InitialImage = null;
            this.min_nearest_neighbour_ck.Location = new System.Drawing.Point(493, 692);
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
            this.min_nearest_neighbour_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_nearest_neighbour_label.ForeColor = System.Drawing.SystemColors.Window;
            this.min_nearest_neighbour_label.Location = new System.Drawing.Point(561, 692);
            this.min_nearest_neighbour_label.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearest_neighbour_label.Name = "min_nearest_neighbour_label";
            this.min_nearest_neighbour_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.min_nearest_neighbour_label.Size = new System.Drawing.Size(227, 64);
            this.min_nearest_neighbour_label.TabIndex = 307;
            this.min_nearest_neighbour_label.Text = "Nearest Neighbour";
            // 
            // min_nearest_neighbour_hitbox
            // 
            this.min_nearest_neighbour_hitbox.AutoSize = true;
            this.min_nearest_neighbour_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.min_nearest_neighbour_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.min_nearest_neighbour_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.min_nearest_neighbour_hitbox.Location = new System.Drawing.Point(490, 692);
            this.min_nearest_neighbour_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.min_nearest_neighbour_hitbox.Name = "min_nearest_neighbour_hitbox";
            this.min_nearest_neighbour_hitbox.Padding = new System.Windows.Forms.Padding(350, 48, 0, 0);
            this.min_nearest_neighbour_hitbox.Size = new System.Drawing.Size(350, 68);
            this.min_nearest_neighbour_hitbox.TabIndex = 308;
            // 
            // mag_linearmipmaplinear_ck
            // 
            this.mag_linearmipmaplinear_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmaplinear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_linearmipmaplinear_ck.Enabled = false;
            this.mag_linearmipmaplinear_ck.ErrorImage = null;
            this.mag_linearmipmaplinear_ck.InitialImage = null;
            this.mag_linearmipmaplinear_ck.Location = new System.Drawing.Point(873, 1007);
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
            this.mag_linearmipmaplinear_label.Location = new System.Drawing.Point(941, 1008);
            this.mag_linearmipmaplinear_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmaplinear_label.Name = "mag_linearmipmaplinear_label";
            this.mag_linearmipmaplinear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_linearmipmaplinear_label.Size = new System.Drawing.Size(240, 64);
            this.mag_linearmipmaplinear_label.TabIndex = 340;
            this.mag_linearmipmaplinear_label.Text = "LinearMipmapLinear";
            // 
            // mag_linearmipmaplinear_hitbox
            // 
            this.mag_linearmipmaplinear_hitbox.AutoSize = true;
            this.mag_linearmipmaplinear_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmaplinear_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_linearmipmaplinear_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mag_linearmipmaplinear_hitbox.Location = new System.Drawing.Point(870, 1007);
            this.mag_linearmipmaplinear_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmaplinear_hitbox.Name = "mag_linearmipmaplinear_hitbox";
            this.mag_linearmipmaplinear_hitbox.Padding = new System.Windows.Forms.Padding(350, 48, 0, 0);
            this.mag_linearmipmaplinear_hitbox.Size = new System.Drawing.Size(350, 68);
            this.mag_linearmipmaplinear_hitbox.TabIndex = 341;
            // 
            // mag_linearmipmapnearest_ck
            // 
            this.mag_linearmipmapnearest_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmapnearest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_linearmipmapnearest_ck.Enabled = false;
            this.mag_linearmipmapnearest_ck.ErrorImage = null;
            this.mag_linearmipmapnearest_ck.InitialImage = null;
            this.mag_linearmipmapnearest_ck.Location = new System.Drawing.Point(873, 948);
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
            this.mag_linearmipmapnearest_label.Location = new System.Drawing.Point(941, 949);
            this.mag_linearmipmapnearest_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmapnearest_label.Name = "mag_linearmipmapnearest_label";
            this.mag_linearmipmapnearest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_linearmipmapnearest_label.Size = new System.Drawing.Size(280, 64);
            this.mag_linearmipmapnearest_label.TabIndex = 337;
            this.mag_linearmipmapnearest_label.Text = "LinearMipmapNearest  ";
            // 
            // mag_linearmipmapnearest_hitbox
            // 
            this.mag_linearmipmapnearest_hitbox.AutoSize = true;
            this.mag_linearmipmapnearest_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mag_linearmipmapnearest_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_linearmipmapnearest_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mag_linearmipmapnearest_hitbox.Location = new System.Drawing.Point(870, 948);
            this.mag_linearmipmapnearest_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linearmipmapnearest_hitbox.Name = "mag_linearmipmapnearest_hitbox";
            this.mag_linearmipmapnearest_hitbox.Padding = new System.Windows.Forms.Padding(350, 48, 0, 0);
            this.mag_linearmipmapnearest_hitbox.Size = new System.Drawing.Size(350, 68);
            this.mag_linearmipmapnearest_hitbox.TabIndex = 338;
            // 
            // mag_nearestmipmaplinear_ck
            // 
            this.mag_nearestmipmaplinear_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmaplinear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_nearestmipmaplinear_ck.Enabled = false;
            this.mag_nearestmipmaplinear_ck.ErrorImage = null;
            this.mag_nearestmipmaplinear_ck.InitialImage = null;
            this.mag_nearestmipmaplinear_ck.Location = new System.Drawing.Point(873, 884);
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
            this.mag_nearestmipmaplinear_label.Location = new System.Drawing.Point(941, 885);
            this.mag_nearestmipmaplinear_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmaplinear_label.Name = "mag_nearestmipmaplinear_label";
            this.mag_nearestmipmaplinear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_nearestmipmaplinear_label.Size = new System.Drawing.Size(280, 64);
            this.mag_nearestmipmaplinear_label.TabIndex = 334;
            this.mag_nearestmipmaplinear_label.Text = "NearestMipmapLinear  ";
            // 
            // mag_nearestmipmaplinear_hitbox
            // 
            this.mag_nearestmipmaplinear_hitbox.AutoSize = true;
            this.mag_nearestmipmaplinear_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmaplinear_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_nearestmipmaplinear_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mag_nearestmipmaplinear_hitbox.Location = new System.Drawing.Point(870, 884);
            this.mag_nearestmipmaplinear_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmaplinear_hitbox.Name = "mag_nearestmipmaplinear_hitbox";
            this.mag_nearestmipmaplinear_hitbox.Padding = new System.Windows.Forms.Padding(350, 48, 0, 0);
            this.mag_nearestmipmaplinear_hitbox.Size = new System.Drawing.Size(350, 68);
            this.mag_nearestmipmaplinear_hitbox.TabIndex = 335;
            // 
            // mag_nearestmipmapnearest_ck
            // 
            this.mag_nearestmipmapnearest_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmapnearest_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_nearestmipmapnearest_ck.Enabled = false;
            this.mag_nearestmipmapnearest_ck.ErrorImage = null;
            this.mag_nearestmipmapnearest_ck.InitialImage = null;
            this.mag_nearestmipmapnearest_ck.Location = new System.Drawing.Point(873, 820);
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
            this.mag_nearestmipmapnearest_label.Location = new System.Drawing.Point(941, 821);
            this.mag_nearestmipmapnearest_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmapnearest_label.Name = "mag_nearestmipmapnearest_label";
            this.mag_nearestmipmapnearest_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_nearestmipmapnearest_label.Size = new System.Drawing.Size(290, 64);
            this.mag_nearestmipmapnearest_label.TabIndex = 331;
            this.mag_nearestmipmapnearest_label.Text = "NearestMipmapNearest ";
            // 
            // mag_nearestmipmapnearest_hitbox
            // 
            this.mag_nearestmipmapnearest_hitbox.AutoSize = true;
            this.mag_nearestmipmapnearest_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearestmipmapnearest_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_nearestmipmapnearest_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mag_nearestmipmapnearest_hitbox.Location = new System.Drawing.Point(870, 820);
            this.mag_nearestmipmapnearest_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearestmipmapnearest_hitbox.Name = "mag_nearestmipmapnearest_hitbox";
            this.mag_nearestmipmapnearest_hitbox.Padding = new System.Windows.Forms.Padding(350, 48, 0, 0);
            this.mag_nearestmipmapnearest_hitbox.Size = new System.Drawing.Size(350, 68);
            this.mag_nearestmipmapnearest_hitbox.TabIndex = 332;
            // 
            // mag_linear_ck
            // 
            this.mag_linear_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_linear_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_linear_ck.Enabled = false;
            this.mag_linear_ck.ErrorImage = null;
            this.mag_linear_ck.InitialImage = null;
            this.mag_linear_ck.Location = new System.Drawing.Point(873, 756);
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
            this.mag_linear_label.Location = new System.Drawing.Point(941, 756);
            this.mag_linear_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linear_label.Name = "mag_linear_label";
            this.mag_linear_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_linear_label.Size = new System.Drawing.Size(81, 64);
            this.mag_linear_label.TabIndex = 328;
            this.mag_linear_label.Text = "Linear";
            // 
            // mag_linear_hitbox
            // 
            this.mag_linear_hitbox.AutoSize = true;
            this.mag_linear_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mag_linear_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_linear_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mag_linear_hitbox.Location = new System.Drawing.Point(870, 756);
            this.mag_linear_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mag_linear_hitbox.Name = "mag_linear_hitbox";
            this.mag_linear_hitbox.Padding = new System.Windows.Forms.Padding(350, 48, 0, 0);
            this.mag_linear_hitbox.Size = new System.Drawing.Size(350, 68);
            this.mag_linear_hitbox.TabIndex = 329;
            // 
            // mag_nearest_neighbour_ck
            // 
            this.mag_nearest_neighbour_ck.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearest_neighbour_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mag_nearest_neighbour_ck.Enabled = false;
            this.mag_nearest_neighbour_ck.ErrorImage = null;
            this.mag_nearest_neighbour_ck.InitialImage = null;
            this.mag_nearest_neighbour_ck.Location = new System.Drawing.Point(873, 692);
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
            this.mag_nearest_neighbour_label.Location = new System.Drawing.Point(941, 692);
            this.mag_nearest_neighbour_label.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearest_neighbour_label.Name = "mag_nearest_neighbour_label";
            this.mag_nearest_neighbour_label.Padding = new System.Windows.Forms.Padding(0, 22, 0, 22);
            this.mag_nearest_neighbour_label.Size = new System.Drawing.Size(227, 64);
            this.mag_nearest_neighbour_label.TabIndex = 325;
            this.mag_nearest_neighbour_label.Text = "Nearest Neighbour";
            // 
            // mag_nearest_neighbour_hitbox
            // 
            this.mag_nearest_neighbour_hitbox.AutoSize = true;
            this.mag_nearest_neighbour_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.mag_nearest_neighbour_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.mag_nearest_neighbour_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.mag_nearest_neighbour_hitbox.Location = new System.Drawing.Point(870, 692);
            this.mag_nearest_neighbour_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.mag_nearest_neighbour_hitbox.Name = "mag_nearest_neighbour_hitbox";
            this.mag_nearest_neighbour_hitbox.Padding = new System.Windows.Forms.Padding(350, 48, 0, 0);
            this.mag_nearest_neighbour_hitbox.Size = new System.Drawing.Size(350, 68);
            this.mag_nearest_neighbour_hitbox.TabIndex = 326;
            // 
            // r_r_ck
            // 
            this.r_r_ck.BackColor = System.Drawing.Color.Transparent;
            this.r_r_ck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.r_r_ck.Enabled = false;
            this.r_r_ck.ErrorImage = null;
            this.r_r_ck.InitialImage = null;
            this.r_r_ck.Location = new System.Drawing.Point(1279, 819);
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
            this.r_g_ck.Location = new System.Drawing.Point(1279, 883);
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
            this.g_r_ck.Location = new System.Drawing.Point(1343, 819);
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
            this.g_g_ck.Location = new System.Drawing.Point(1343, 883);
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
            this.a_g_ck.Location = new System.Drawing.Point(1471, 883);
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
            this.a_r_ck.Location = new System.Drawing.Point(1471, 819);
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
            this.b_g_ck.Location = new System.Drawing.Point(1407, 883);
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
            this.b_r_ck.Location = new System.Drawing.Point(1407, 819);
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
            this.g_a_ck.Location = new System.Drawing.Point(1343, 1010);
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
            this.g_b_ck.Location = new System.Drawing.Point(1343, 946);
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
            this.r_a_ck.Location = new System.Drawing.Point(1279, 1010);
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
            this.r_b_ck.Location = new System.Drawing.Point(1279, 946);
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
            this.a_a_ck.Location = new System.Drawing.Point(1471, 1010);
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
            this.a_b_ck.Location = new System.Drawing.Point(1471, 946);
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
            this.b_a_ck.Location = new System.Drawing.Point(1407, 1010);
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
            this.b_b_ck.Location = new System.Drawing.Point(1407, 946);
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
            this.colour_channels_label.Location = new System.Drawing.Point(1306, 783);
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
            this.r_r_ck_hitbox.Location = new System.Drawing.Point(1279, 819);
            this.r_r_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.r_r_ck_hitbox.Name = "r_r_ck_hitbox";
            this.r_r_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.r_r_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.r_r_ck_hitbox.TabIndex = 360;
            // 
            // g_r_ck_hitbox
            // 
            this.g_r_ck_hitbox.AutoSize = true;
            this.g_r_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.g_r_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.g_r_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.g_r_ck_hitbox.Location = new System.Drawing.Point(1343, 819);
            this.g_r_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.g_r_ck_hitbox.Name = "g_r_ck_hitbox";
            this.g_r_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.g_r_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.g_r_ck_hitbox.TabIndex = 361;
            // 
            // a_a_ck_hitbox
            // 
            this.a_a_ck_hitbox.AutoSize = true;
            this.a_a_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.a_a_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.a_a_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.a_a_ck_hitbox.Location = new System.Drawing.Point(1471, 1012);
            this.a_a_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.a_a_ck_hitbox.Name = "a_a_ck_hitbox";
            this.a_a_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.a_a_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.a_a_ck_hitbox.TabIndex = 362;
            // 
            // r_g_ck_hitbox
            // 
            this.r_g_ck_hitbox.AutoSize = true;
            this.r_g_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.r_g_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.r_g_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.r_g_ck_hitbox.Location = new System.Drawing.Point(1279, 882);
            this.r_g_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.r_g_ck_hitbox.Name = "r_g_ck_hitbox";
            this.r_g_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.r_g_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.r_g_ck_hitbox.TabIndex = 363;
            // 
            // r_b_ck_hitbox
            // 
            this.r_b_ck_hitbox.AutoSize = true;
            this.r_b_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.r_b_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.r_b_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.r_b_ck_hitbox.Location = new System.Drawing.Point(1279, 946);
            this.r_b_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.r_b_ck_hitbox.Name = "r_b_ck_hitbox";
            this.r_b_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.r_b_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.r_b_ck_hitbox.TabIndex = 364;
            // 
            // r_a_ck_hitbox
            // 
            this.r_a_ck_hitbox.AutoSize = true;
            this.r_a_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.r_a_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.r_a_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.r_a_ck_hitbox.Location = new System.Drawing.Point(1279, 1012);
            this.r_a_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.r_a_ck_hitbox.Name = "r_a_ck_hitbox";
            this.r_a_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.r_a_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.r_a_ck_hitbox.TabIndex = 365;
            // 
            // g_g_ck_hitbox
            // 
            this.g_g_ck_hitbox.AutoSize = true;
            this.g_g_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.g_g_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.g_g_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.g_g_ck_hitbox.Location = new System.Drawing.Point(1343, 882);
            this.g_g_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.g_g_ck_hitbox.Name = "g_g_ck_hitbox";
            this.g_g_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.g_g_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.g_g_ck_hitbox.TabIndex = 366;
            // 
            // g_b_ck_hitbox
            // 
            this.g_b_ck_hitbox.AutoSize = true;
            this.g_b_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.g_b_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.g_b_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.g_b_ck_hitbox.Location = new System.Drawing.Point(1343, 946);
            this.g_b_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.g_b_ck_hitbox.Name = "g_b_ck_hitbox";
            this.g_b_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.g_b_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.g_b_ck_hitbox.TabIndex = 367;
            // 
            // g_a_ck_hitbox
            // 
            this.g_a_ck_hitbox.AutoSize = true;
            this.g_a_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.g_a_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.g_a_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.g_a_ck_hitbox.Location = new System.Drawing.Point(1343, 1010);
            this.g_a_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.g_a_ck_hitbox.Name = "g_a_ck_hitbox";
            this.g_a_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.g_a_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.g_a_ck_hitbox.TabIndex = 368;
            // 
            // b_r_ck_hitbox
            // 
            this.b_r_ck_hitbox.AutoSize = true;
            this.b_r_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.b_r_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.b_r_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.b_r_ck_hitbox.Location = new System.Drawing.Point(1407, 821);
            this.b_r_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.b_r_ck_hitbox.Name = "b_r_ck_hitbox";
            this.b_r_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.b_r_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.b_r_ck_hitbox.TabIndex = 369;
            // 
            // b_g_ck_hitbox
            // 
            this.b_g_ck_hitbox.AutoSize = true;
            this.b_g_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.b_g_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.b_g_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.b_g_ck_hitbox.Location = new System.Drawing.Point(1407, 882);
            this.b_g_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.b_g_ck_hitbox.Name = "b_g_ck_hitbox";
            this.b_g_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.b_g_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.b_g_ck_hitbox.TabIndex = 370;
            // 
            // b_b_ck_hitbox
            // 
            this.b_b_ck_hitbox.AutoSize = true;
            this.b_b_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.b_b_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.b_b_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.b_b_ck_hitbox.Location = new System.Drawing.Point(1407, 946);
            this.b_b_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.b_b_ck_hitbox.Name = "b_b_ck_hitbox";
            this.b_b_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.b_b_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.b_b_ck_hitbox.TabIndex = 371;
            // 
            // b_a_ck_hitbox
            // 
            this.b_a_ck_hitbox.AutoSize = true;
            this.b_a_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.b_a_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.b_a_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.b_a_ck_hitbox.Location = new System.Drawing.Point(1407, 1010);
            this.b_a_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.b_a_ck_hitbox.Name = "b_a_ck_hitbox";
            this.b_a_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.b_a_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.b_a_ck_hitbox.TabIndex = 372;
            // 
            // a_r_ck_hitbox
            // 
            this.a_r_ck_hitbox.AutoSize = true;
            this.a_r_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.a_r_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.a_r_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.a_r_ck_hitbox.Location = new System.Drawing.Point(1471, 820);
            this.a_r_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.a_r_ck_hitbox.Name = "a_r_ck_hitbox";
            this.a_r_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.a_r_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.a_r_ck_hitbox.TabIndex = 373;
            // 
            // a_g_ck_hitbox
            // 
            this.a_g_ck_hitbox.AutoSize = true;
            this.a_g_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.a_g_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.a_g_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.a_g_ck_hitbox.Location = new System.Drawing.Point(1471, 882);
            this.a_g_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.a_g_ck_hitbox.Name = "a_g_ck_hitbox";
            this.a_g_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.a_g_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.a_g_ck_hitbox.TabIndex = 374;
            // 
            // a_b_ck_hitbox
            // 
            this.a_b_ck_hitbox.AutoSize = true;
            this.a_b_ck_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.a_b_ck_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.a_b_ck_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.a_b_ck_hitbox.Location = new System.Drawing.Point(1471, 946);
            this.a_b_ck_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.a_b_ck_hitbox.Name = "a_b_ck_hitbox";
            this.a_b_ck_hitbox.Padding = new System.Windows.Forms.Padding(64, 48, 0, 0);
            this.a_b_ck_hitbox.Size = new System.Drawing.Size(64, 68);
            this.a_b_ck_hitbox.TabIndex = 375;
            // 
            // plt0_gui
            // 
            this.AllowDrop = true;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1920, 1061);
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
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.MaximumSize = new System.Drawing.Size(1920, 1080);
            this.Name = "plt0_gui";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        // all the lines below 'till the end are the output of python scripts I made
        // I had to name all labels and pictureboxes through the [Design] tab manually though.
        // I also had to redirect the labels to each function below, which again, isn't pretty fast

        // generated checkbox behaviour code by the python script in the py folder

        private void bmd_Click(object sender, EventArgs e)
        {
            if (bmd)
            {
                bmd = false;
                hover_checkbox(bmd_ck);
            }
            else
            {
                bmd = true;
                selected_checkbox(bmd_ck);
            }
        }
        private void bmd_MouseEnter(object sender, EventArgs e)
        {
            if (bmd)
                selected_checkbox(bmd_ck);
            else
                hover_checkbox(bmd_ck);
        }
        private void bmd_MouseLeave(object sender, EventArgs e)
        {
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
                hover_checkbox(bti_ck);
            }
            else
            {
                bti = true;
                selected_checkbox(bti_ck);
            }
        }
        private void bti_MouseEnter(object sender, EventArgs e)
        {
            if (bti)
                selected_checkbox(bti_ck);
            else
                hover_checkbox(bti_ck);
        }
        private void bti_MouseLeave(object sender, EventArgs e)
        {
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
                hover_checkbox(tex0_ck);
            }
            else
            {
                tex0 = true;
                selected_checkbox(tex0_ck);
            }
        }
        private void tex0_MouseEnter(object sender, EventArgs e)
        {
            if (tex0)
                selected_checkbox(tex0_ck);
            else
                hover_checkbox(tex0_ck);
        }
        private void tex0_MouseLeave(object sender, EventArgs e)
        {
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
                hover_checkbox(tpl_ck);
            }
            else
            {
                tpl = true;
                selected_checkbox(tpl_ck);
            }
        }
        private void tpl_MouseEnter(object sender, EventArgs e)
        {
            if (tpl)
                selected_checkbox(tpl_ck);
            else
                hover_checkbox(tpl_ck);
        }
        private void tpl_MouseLeave(object sender, EventArgs e)
        {
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
                hover_checkbox(bmp_ck);
            }
            else
            {
                bmp = true;
                selected_checkbox(bmp_ck);
            }
        }
        private void bmp_MouseEnter(object sender, EventArgs e)
        {
            if (bmp)
                selected_checkbox(bmp_ck);
            else
                hover_checkbox(bmp_ck);
        }
        private void bmp_MouseLeave(object sender, EventArgs e)
        {
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
                hover_checkbox(png_ck);
            }
            else
            {
                png = true;
                selected_checkbox(png_ck);
            }
        }
        private void png_MouseEnter(object sender, EventArgs e)
        {
            if (png)
                selected_checkbox(png_ck);
            else
                hover_checkbox(png_ck);
        }
        private void png_MouseLeave(object sender, EventArgs e)
        {
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
                hover_checkbox(jpg_ck);
            }
            else
            {
                jpg = true;
                selected_checkbox(jpg_ck);
            }
        }
        private void jpg_MouseEnter(object sender, EventArgs e)
        {
            if (jpg)
                selected_checkbox(jpg_ck);
            else
                hover_checkbox(jpg_ck);
        }
        private void jpg_MouseLeave(object sender, EventArgs e)
        {
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
                hover_checkbox(jpeg_ck);
            }
            else
            {
                jpeg = true;
                selected_checkbox(jpeg_ck);
            }
        }
        private void jpeg_MouseEnter(object sender, EventArgs e)
        {
            if (jpeg)
                selected_checkbox(jpeg_ck);
            else
                hover_checkbox(jpeg_ck);
        }
        private void jpeg_MouseLeave(object sender, EventArgs e)
        {
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
                hover_checkbox(gif_ck);
            }
            else
            {
                gif = true;
                selected_checkbox(gif_ck);
            }
        }
        private void gif_MouseEnter(object sender, EventArgs e)
        {
            if (gif)
                selected_checkbox(gif_ck);
            else
                hover_checkbox(gif_ck);
        }
        private void gif_MouseLeave(object sender, EventArgs e)
        {
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
                hover_checkbox(ico_ck);
            }
            else
            {
                ico = true;
                selected_checkbox(ico_ck);
            }
        }
        private void ico_MouseEnter(object sender, EventArgs e)
        {
            if (ico)
                selected_checkbox(ico_ck);
            else
                hover_checkbox(ico_ck);
        }
        private void ico_MouseLeave(object sender, EventArgs e)
        {
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
                hover_checkbox(tif_ck);
            }
            else
            {
                tif = true;
                selected_checkbox(tif_ck);
            }
        }
        private void tif_MouseEnter(object sender, EventArgs e)
        {
            if (tif)
                selected_checkbox(tif_ck);
            else
                hover_checkbox(tif_ck);
        }
        private void tif_MouseLeave(object sender, EventArgs e)
        {
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
                hover_checkbox(tiff_ck);
            }
            else
            {
                tiff = true;
                selected_checkbox(tiff_ck);
            }
        }
        private void tiff_MouseEnter(object sender, EventArgs e)
        {
            if (tiff)
                selected_checkbox(tiff_ck);
            else
                hover_checkbox(tiff_ck);
        }
        private void tiff_MouseLeave(object sender, EventArgs e)
        {
            if (tiff)
                checked_checkbox(tiff_ck);
            else
                unchecked_checkbox(tiff_ck);
        }
        private void no_warning_Click(object sender, EventArgs e)
        {
            if (no_warning)
            {
                no_warning = false;
                hover_checkbox(no_warning_ck);
            }
            else
            {
                no_warning = true;
                selected_checkbox(no_warning_ck);
            }
        }
        private void no_warning_MouseEnter(object sender, EventArgs e)
        {
            if (no_warning)
                selected_checkbox(no_warning_ck);
            else
                hover_checkbox(no_warning_ck);
        }
        private void no_warning_MouseLeave(object sender, EventArgs e)
        {
            if (no_warning)
                checked_checkbox(no_warning_ck);
            else
                unchecked_checkbox(no_warning_ck);
        }
        private void warn_Click(object sender, EventArgs e)
        {
            if (warn)
            {
                warn = false;
                hover_checkbox(warn_ck);
            }
            else
            {
                warn = true;
                selected_checkbox(warn_ck);
            }
        }
        private void warn_MouseEnter(object sender, EventArgs e)
        {
            if (warn)
                selected_checkbox(warn_ck);
            else
                hover_checkbox(warn_ck);
        }
        private void warn_MouseLeave(object sender, EventArgs e)
        {
            if (warn)
                checked_checkbox(warn_ck);
            else
                unchecked_checkbox(warn_ck);
        }
        private void funky_Click(object sender, EventArgs e)
        {
            if (funky)
            {
                funky = false;
                hover_checkbox(funky_ck);
            }
            else
            {
                funky = true;
                selected_checkbox(funky_ck);
            }
        }
        private void funky_MouseEnter(object sender, EventArgs e)
        {
            if (funky)
                selected_checkbox(funky_ck);
            else
                hover_checkbox(funky_ck);
        }
        private void funky_MouseLeave(object sender, EventArgs e)
        {
            if (funky)
                checked_checkbox(funky_ck);
            else
                unchecked_checkbox(funky_ck);
        }
        private void stfu_Click(object sender, EventArgs e)
        {
            if (stfu)
            {
                stfu = false;
                hover_checkbox(stfu_ck);
            }
            else
            {
                stfu = true;
                selected_checkbox(stfu_ck);
            }
        }
        private void stfu_MouseEnter(object sender, EventArgs e)
        {
            if (stfu)
                selected_checkbox(stfu_ck);
            else
                hover_checkbox(stfu_ck);
        }
        private void stfu_MouseLeave(object sender, EventArgs e)
        {
            if (stfu)
                checked_checkbox(stfu_ck);
            else
                unchecked_checkbox(stfu_ck);
        }
        private void safe_mode_Click(object sender, EventArgs e)
        {
            if (safe_mode)
            {
                safe_mode = false;
                hover_checkbox(safe_mode_ck);
            }
            else
            {
                safe_mode = true;
                selected_checkbox(safe_mode_ck);
            }
        }
        private void safe_mode_MouseEnter(object sender, EventArgs e)
        {
            if (safe_mode)
                selected_checkbox(safe_mode_ck);
            else
                hover_checkbox(safe_mode_ck);
        }
        private void safe_mode_MouseLeave(object sender, EventArgs e)
        {
            if (safe_mode)
                checked_checkbox(safe_mode_ck);
            else
                unchecked_checkbox(safe_mode_ck);
        }
        private void FORCE_ALPHA_Click(object sender, EventArgs e)
        {
            if (FORCE_ALPHA)
            {
                FORCE_ALPHA = false;
                hover_checkbox(FORCE_ALPHA_ck);
            }
            else
            {
                FORCE_ALPHA = true;
                selected_checkbox(FORCE_ALPHA_ck);
            }
        }
        private void FORCE_ALPHA_MouseEnter(object sender, EventArgs e)
        {
            if (FORCE_ALPHA)
                selected_checkbox(FORCE_ALPHA_ck);
            else
                hover_checkbox(FORCE_ALPHA_ck);
        }
        private void FORCE_ALPHA_MouseLeave(object sender, EventArgs e)
        {
            if (FORCE_ALPHA)
                checked_checkbox(FORCE_ALPHA_ck);
            else
                unchecked_checkbox(FORCE_ALPHA_ck);
        }
        private void ask_exit_Click(object sender, EventArgs e)
        {
            if (ask_exit)
            {
                ask_exit = false;
                hover_checkbox(ask_exit_ck);
            }
            else
            {
                ask_exit = true;
                selected_checkbox(ask_exit_ck);
            }
        }
        private void ask_exit_MouseEnter(object sender, EventArgs e)
        {
            if (ask_exit)
                selected_checkbox(ask_exit_ck);
            else
                hover_checkbox(ask_exit_ck);
        }
        private void ask_exit_MouseLeave(object sender, EventArgs e)
        {
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
                hover_checkbox(bmp_32_ck);
            }
            else
            {
                bmp_32 = true;
                selected_checkbox(bmp_32_ck);
            }
        }
        private void bmp_32_MouseEnter(object sender, EventArgs e)
        {
            if (bmp_32)
                selected_checkbox(bmp_32_ck);
            else
                hover_checkbox(bmp_32_ck);
        }
        private void bmp_32_MouseLeave(object sender, EventArgs e)
        {
            if (bmp_32)
                checked_checkbox(bmp_32_ck);
            else
                unchecked_checkbox(bmp_32_ck);
        }
        private void reverse_Click(object sender, EventArgs e)
        {
            if (reverse)
            {
                reverse = false;
                hover_checkbox(reverse_ck);
            }
            else
            {
                reverse = true;
                selected_checkbox(reverse_ck);
            }
        }
        private void reverse_MouseEnter(object sender, EventArgs e)
        {
            if (reverse)
                selected_checkbox(reverse_ck);
            else
                hover_checkbox(reverse_ck);
        }
        private void reverse_MouseLeave(object sender, EventArgs e)
        {
            if (reverse)
                checked_checkbox(reverse_ck);
            else
                unchecked_checkbox(reverse_ck);
        }
        private void random_Click(object sender, EventArgs e)
        {
            if (random)
            {
                random = false;
                hover_checkbox(random_ck);
            }
            else
            {
                random = true;
                selected_checkbox(random_ck);
            }
        }
        private void random_MouseEnter(object sender, EventArgs e)
        {
            if (random)
                selected_checkbox(random_ck);
            else
                hover_checkbox(random_ck);
        }
        private void random_MouseLeave(object sender, EventArgs e)
        {
            if (random)
                checked_checkbox(random_ck);
            else
                unchecked_checkbox(random_ck);
        }
        private void I4_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(i4_ck);
            encoding = 0; // I4
        }
        private void I4_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == 0)
                selected_encoding(i4_ck);
            else
                hover_encoding(i4_ck);
        }
        private void I4_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == 0)
                checked_encoding(i4_ck);
            else
                unchecked_encoding(i4_ck);
        }
        private void I8_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(i8_ck);
            encoding = 1; // I8
        }
        private void I8_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == 1)
                selected_encoding(i8_ck);
            else
                hover_encoding(i8_ck);
        }
        private void I8_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == 1)
                checked_encoding(i8_ck);
            else
                unchecked_encoding(i8_ck);
        }
        private void AI4_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(ai4_ck);
            encoding = 2; // AI4
        }
        private void AI4_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == 2)
                selected_encoding(ai4_ck);
            else
                hover_encoding(ai4_ck);
        }
        private void AI4_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == 2)
                checked_encoding(ai4_ck);
            else
                unchecked_encoding(ai4_ck);
        }
        private void AI8_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(ai8_ck);
            encoding = 3; // AI8
        }
        private void AI8_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == 3)
                selected_encoding(ai8_ck);
            else
                hover_encoding(ai8_ck);
        }
        private void AI8_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == 3)
                checked_encoding(ai8_ck);
            else
                unchecked_encoding(ai8_ck);
        }
        private void RGB565_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(rgb565_ck);
            encoding = 4; // RGB565
        }
        private void RGB565_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == 4)
                selected_encoding(rgb565_ck);
            else
                hover_encoding(rgb565_ck);
        }
        private void RGB565_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == 4)
                checked_encoding(rgb565_ck);
            else
                unchecked_encoding(rgb565_ck);
        }
        private void RGB5A3_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(rgb5a3_ck);
            encoding = 5; // RGB5A3
        }
        private void RGB5A3_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == 5)
                selected_encoding(rgb5a3_ck);
            else
                hover_encoding(rgb5a3_ck);
        }
        private void RGB5A3_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == 5)
                checked_encoding(rgb5a3_ck);
            else
                unchecked_encoding(rgb5a3_ck);
        }
        private void RGBA32_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(rgba32_ck);
            encoding = 6; // RGBA32
        }
        private void RGBA32_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == 6)
                selected_encoding(rgba32_ck);
            else
                hover_encoding(rgba32_ck);
        }
        private void RGBA32_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == 6)
                checked_encoding(rgba32_ck);
            else
                unchecked_encoding(rgba32_ck);
        }
        private void CI4_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(ci4_ck);
            encoding = 8; // CI4
        }
        private void CI4_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == 8)
                selected_encoding(ci4_ck);
            else
                hover_encoding(ci4_ck);
        }
        private void CI4_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == 8)
                checked_encoding(ci4_ck);
            else
                unchecked_encoding(ci4_ck);
        }
        private void CI8_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(ci8_ck);
            encoding = 9; // CI8
        }
        private void CI8_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == 9)
                selected_encoding(ci8_ck);
            else
                hover_encoding(ci8_ck);
        }
        private void CI8_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == 9)
                checked_encoding(ci8_ck);
            else
                unchecked_encoding(ci8_ck);
        }
        private void CI14X2_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(ci14x2_ck);
            encoding = 10; // CI14X2
        }
        private void CI14X2_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == 10)
                selected_encoding(ci14x2_ck);
            else
                hover_encoding(ci14x2_ck);
        }
        private void CI14X2_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == 10)
                checked_encoding(ci14x2_ck);
            else
                unchecked_encoding(ci14x2_ck);
        }
        private void CMPR_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(cmpr_ck);
            encoding = 14; // CMPR
        }
        private void CMPR_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == 14)
                selected_encoding(cmpr_ck);
            else
                hover_encoding(cmpr_ck);
        }
        private void CMPR_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == 14)
                checked_encoding(cmpr_ck);
            else
                unchecked_encoding(cmpr_ck);
        }
        private void Cie_601_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            selected_algorithm(cie_601_ck);
            algorithm = 0; // Cie_601
        }
        private void Cie_601_MouseEnter(object sender, EventArgs e)
        {
            if (algorithm == 0)
                selected_algorithm(cie_601_ck);
            else
                hover_algorithm(cie_601_ck);
        }
        private void Cie_601_MouseLeave(object sender, EventArgs e)
        {
            if (algorithm == 0)
                checked_algorithm(cie_601_ck);
            else
                unchecked_algorithm(cie_601_ck);
        }
        private void Cie_709_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            selected_algorithm(cie_709_ck);
            algorithm = 1; // Cie_709
        }
        private void Cie_709_MouseEnter(object sender, EventArgs e)
        {
            if (algorithm == 1)
                selected_algorithm(cie_709_ck);
            else
                hover_algorithm(cie_709_ck);
        }
        private void Cie_709_MouseLeave(object sender, EventArgs e)
        {
            if (algorithm == 1)
                checked_algorithm(cie_709_ck);
            else
                unchecked_algorithm(cie_709_ck);
        }
        private void Custom_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            selected_algorithm(custom_ck);
            algorithm = 2; // Custom
        }
        private void Custom_MouseEnter(object sender, EventArgs e)
        {
            if (algorithm == 2)
                selected_algorithm(custom_ck);
            else
                hover_algorithm(custom_ck);
        }
        private void Custom_MouseLeave(object sender, EventArgs e)
        {
            if (algorithm == 2)
                checked_algorithm(custom_ck);
            else
                unchecked_algorithm(custom_ck);
        }
        private void No_gradient_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            selected_algorithm(no_gradient_ck);
            algorithm = 3; // No_gradient
        }
        private void No_gradient_MouseEnter(object sender, EventArgs e)
        {
            if (algorithm == 3)
                selected_algorithm(no_gradient_ck);
            else
                hover_algorithm(no_gradient_ck);
        }
        private void No_gradient_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void No_alpha_MouseEnter(object sender, EventArgs e)
        {
            if (alpha == 0)
                selected_alpha(no_alpha_ck);
            else
                hover_alpha(no_alpha_ck);
        }
        private void No_alpha_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Alpha_MouseEnter(object sender, EventArgs e)
        {
            if (alpha == 1)
                selected_alpha(alpha_ck);
            else
                hover_alpha(alpha_ck);
        }
        private void Alpha_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Mix_MouseEnter(object sender, EventArgs e)
        {
            if (alpha == 2)
                selected_alpha(mix_ck);
            else
                hover_alpha(mix_ck);
        }
        private void Mix_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void WrapS_Clamp_MouseEnter(object sender, EventArgs e)
        {
            if (WrapS == 0)
                selected_WrapS(Sclamp_ck);
            else
                hover_WrapS(Sclamp_ck);
        }
        private void WrapS_Clamp_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void WrapS_Repeat_MouseEnter(object sender, EventArgs e)
        {
            if (WrapS == 1)
                selected_WrapS(Srepeat_ck);
            else
                hover_WrapS(Srepeat_ck);
        }
        private void WrapS_Repeat_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void WrapS_Mirror_MouseEnter(object sender, EventArgs e)
        {
            if (WrapS == 2)
                selected_WrapS(Smirror_ck);
            else
                hover_WrapS(Smirror_ck);
        }
        private void WrapS_Mirror_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void WrapT_Clamp_MouseEnter(object sender, EventArgs e)
        {
            if (WrapT == 0)
                selected_WrapT(Tclamp_ck);
            else
                hover_WrapT(Tclamp_ck);
        }
        private void WrapT_Clamp_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void WrapT_Repeat_MouseEnter(object sender, EventArgs e)
        {
            if (WrapT == 1)
                selected_WrapT(Trepeat_ck);
            else
                hover_WrapT(Trepeat_ck);
        }
        private void WrapT_Repeat_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void WrapT_Mirror_MouseEnter(object sender, EventArgs e)
        {
            if (WrapT == 2)
                selected_WrapT(Tmirror_ck);
            else
                hover_WrapT(Tmirror_ck);
        }
        private void WrapT_Mirror_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Minification_Nearest_Neighbour_MouseEnter(object sender, EventArgs e)
        {
            if (minification_filter == 0)
                selected_Minification(min_nearest_neighbour_ck);
            else
                hover_Minification(min_nearest_neighbour_ck);
        }
        private void Minification_Nearest_Neighbour_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Minification_Linear_MouseEnter(object sender, EventArgs e)
        {
            if (minification_filter == 1)
                selected_Minification(min_linear_ck);
            else
                hover_Minification(min_linear_ck);
        }
        private void Minification_Linear_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Minification_NearestMipmapNearest_MouseEnter(object sender, EventArgs e)
        {
            if (minification_filter == 2)
                selected_Minification(min_nearestmipmapnearest_ck);
            else
                hover_Minification(min_nearestmipmapnearest_ck);
        }
        private void Minification_NearestMipmapNearest_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Minification_NearestMipmapLinear_MouseEnter(object sender, EventArgs e)
        {
            if (minification_filter == 3)
                selected_Minification(min_nearestmipmaplinear_ck);
            else
                hover_Minification(min_nearestmipmaplinear_ck);
        }
        private void Minification_NearestMipmapLinear_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Minification_LinearMipmapNearest_MouseEnter(object sender, EventArgs e)
        {
            if (minification_filter == 4)
                selected_Minification(min_linearmipmapnearest_ck);
            else
                hover_Minification(min_linearmipmapnearest_ck);
        }
        private void Minification_LinearMipmapNearest_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Minification_LinearMipmapLinear_MouseEnter(object sender, EventArgs e)
        {
            if (minification_filter == 5)
                selected_Minification(min_linearmipmaplinear_ck);
            else
                hover_Minification(min_linearmipmaplinear_ck);
        }
        private void Minification_LinearMipmapLinear_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Magnification_Nearest_Neighbour_MouseEnter(object sender, EventArgs e)
        {
            if (magnification_filter == 0)
                selected_Magnification(mag_nearest_neighbour_ck);
            else
                hover_Magnification(mag_nearest_neighbour_ck);
        }
        private void Magnification_Nearest_Neighbour_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Magnification_Linear_MouseEnter(object sender, EventArgs e)
        {
            if (magnification_filter == 1)
                selected_Magnification(mag_linear_ck);
            else
                hover_Magnification(mag_linear_ck);
        }
        private void Magnification_Linear_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Magnification_NearestMipmapNearest_MouseEnter(object sender, EventArgs e)
        {
            if (magnification_filter == 2)
                selected_Magnification(mag_nearestmipmapnearest_ck);
            else
                hover_Magnification(mag_nearestmipmapnearest_ck);
        }
        private void Magnification_NearestMipmapNearest_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Magnification_NearestMipmapLinear_MouseEnter(object sender, EventArgs e)
        {
            if (magnification_filter == 3)
                selected_Magnification(mag_nearestmipmaplinear_ck);
            else
                hover_Magnification(mag_nearestmipmaplinear_ck);
        }
        private void Magnification_NearestMipmapLinear_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Magnification_LinearMipmapNearest_MouseEnter(object sender, EventArgs e)
        {
            if (magnification_filter == 4)
                selected_Magnification(mag_linearmipmapnearest_ck);
            else
                hover_Magnification(mag_linearmipmapnearest_ck);
        }
        private void Magnification_LinearMipmapNearest_MouseLeave(object sender, EventArgs e)
        {
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
        }
        private void Magnification_LinearMipmapLinear_MouseEnter(object sender, EventArgs e)
        {
            if (magnification_filter == 5)
                selected_Magnification(mag_linearmipmaplinear_ck);
            else
                hover_Magnification(mag_linearmipmaplinear_ck);
        }
        private void Magnification_LinearMipmapLinear_MouseLeave(object sender, EventArgs e)
        {
            if (magnification_filter == 5)
                checked_Magnification(mag_linearmipmaplinear_ck);
            else
                unchecked_Magnification(mag_linearmipmaplinear_ck);
        }
        private void R_R_Click(object sender, EventArgs e)
        {
            unchecked_R(r_ck[r]);
            selected_R(r_r_ck);
            r = 0; // Red channel set to R
        }
        private void R_R_MouseEnter(object sender, EventArgs e)
        {
            if (r == 0)
                selected_R(r_r_ck);
            else
                hover_R(r_r_ck);
        }
        private void R_R_MouseLeave(object sender, EventArgs e)
        {
            if (r == 0)
                checked_R(r_r_ck);
            else
                unchecked_R(r_r_ck);
        }
        private void R_G_Click(object sender, EventArgs e)
        {
            unchecked_R(r_ck[r]);
            selected_R(r_g_ck);
            r = 1; // Red channel set to G
        }
        private void R_G_MouseEnter(object sender, EventArgs e)
        {
            if (r == 1)
                selected_R(r_g_ck);
            else
                hover_R(r_g_ck);
        }
        private void R_G_MouseLeave(object sender, EventArgs e)
        {
            if (r == 1)
                checked_R(r_g_ck);
            else
                unchecked_R(r_g_ck);
        }
        private void R_B_Click(object sender, EventArgs e)
        {
            unchecked_R(r_ck[r]);
            selected_R(r_b_ck);
            r = 2; // Red channel set to B
        }
        private void R_B_MouseEnter(object sender, EventArgs e)
        {
            if (r == 2)
                selected_R(r_b_ck);
            else
                hover_R(r_b_ck);
        }
        private void R_B_MouseLeave(object sender, EventArgs e)
        {
            if (r == 2)
                checked_R(r_b_ck);
            else
                unchecked_R(r_b_ck);
        }
        private void R_A_Click(object sender, EventArgs e)
        {
            unchecked_R(r_ck[r]);
            selected_R(r_a_ck);
            r = 3; // Red channel set to A
        }
        private void R_A_MouseEnter(object sender, EventArgs e)
        {
            if (r == 3)
                selected_R(r_a_ck);
            else
                hover_R(r_a_ck);
        }
        private void R_A_MouseLeave(object sender, EventArgs e)
        {
            if (r == 3)
                checked_R(r_a_ck);
            else
                unchecked_R(r_a_ck);
        }
        private void G_R_Click(object sender, EventArgs e)
        {
            unchecked_G(g_ck[g]);
            selected_G(g_r_ck);
            g = 0; // Green channel set to R
        }
        private void G_R_MouseEnter(object sender, EventArgs e)
        {
            if (g == 0)
                selected_G(g_r_ck);
            else
                hover_G(g_r_ck);
        }
        private void G_R_MouseLeave(object sender, EventArgs e)
        {
            if (g == 0)
                checked_G(g_r_ck);
            else
                unchecked_G(g_r_ck);
        }
        private void G_G_Click(object sender, EventArgs e)
        {
            unchecked_G(g_ck[g]);
            selected_G(g_g_ck);
            g = 1; // Green channel set to G
        }
        private void G_G_MouseEnter(object sender, EventArgs e)
        {
            if (g == 1)
                selected_G(g_g_ck);
            else
                hover_G(g_g_ck);
        }
        private void G_G_MouseLeave(object sender, EventArgs e)
        {
            if (g == 1)
                checked_G(g_g_ck);
            else
                unchecked_G(g_g_ck);
        }
        private void G_B_Click(object sender, EventArgs e)
        {
            unchecked_G(g_ck[g]);
            selected_G(g_b_ck);
            g = 2; // Green channel set to B
        }
        private void G_B_MouseEnter(object sender, EventArgs e)
        {
            if (g == 2)
                selected_G(g_b_ck);
            else
                hover_G(g_b_ck);
        }
        private void G_B_MouseLeave(object sender, EventArgs e)
        {
            if (g == 2)
                checked_G(g_b_ck);
            else
                unchecked_G(g_b_ck);
        }
        private void G_A_Click(object sender, EventArgs e)
        {
            unchecked_G(g_ck[g]);
            selected_G(g_a_ck);
            g = 3; // Green channel set to A
        }
        private void G_A_MouseEnter(object sender, EventArgs e)
        {
            if (g == 3)
                selected_G(g_a_ck);
            else
                hover_G(g_a_ck);
        }
        private void G_A_MouseLeave(object sender, EventArgs e)
        {
            if (g == 3)
                checked_G(g_a_ck);
            else
                unchecked_G(g_a_ck);
        }
        private void B_R_Click(object sender, EventArgs e)
        {
            unchecked_B(b_ck[b]);
            selected_B(b_r_ck);
            b = 0; // Blue channel set to R
        }
        private void B_R_MouseEnter(object sender, EventArgs e)
        {
            if (b == 0)
                selected_B(b_r_ck);
            else
                hover_B(b_r_ck);
        }
        private void B_R_MouseLeave(object sender, EventArgs e)
        {
            if (b == 0)
                checked_B(b_r_ck);
            else
                unchecked_B(b_r_ck);
        }
        private void B_G_Click(object sender, EventArgs e)
        {
            unchecked_B(b_ck[b]);
            selected_B(b_g_ck);
            b = 1; // Blue channel set to G
        }
        private void B_G_MouseEnter(object sender, EventArgs e)
        {
            if (b == 1)
                selected_B(b_g_ck);
            else
                hover_B(b_g_ck);
        }
        private void B_G_MouseLeave(object sender, EventArgs e)
        {
            if (b == 1)
                checked_B(b_g_ck);
            else
                unchecked_B(b_g_ck);
        }
        private void B_B_Click(object sender, EventArgs e)
        {
            unchecked_B(b_ck[b]);
            selected_B(b_b_ck);
            b = 2; // Blue channel set to B
        }
        private void B_B_MouseEnter(object sender, EventArgs e)
        {
            if (b == 2)
                selected_B(b_b_ck);
            else
                hover_B(b_b_ck);
        }
        private void B_B_MouseLeave(object sender, EventArgs e)
        {
            if (b == 2)
                checked_B(b_b_ck);
            else
                unchecked_B(b_b_ck);
        }
        private void B_A_Click(object sender, EventArgs e)
        {
            unchecked_B(b_ck[b]);
            selected_B(b_a_ck);
            b = 3; // Blue channel set to A
        }
        private void B_A_MouseEnter(object sender, EventArgs e)
        {
            if (b == 3)
                selected_B(b_a_ck);
            else
                hover_B(b_a_ck);
        }
        private void B_A_MouseLeave(object sender, EventArgs e)
        {
            if (b == 3)
                checked_B(b_a_ck);
            else
                unchecked_B(b_a_ck);
        }
        private void A_R_Click(object sender, EventArgs e)
        {
            unchecked_A(a_ck[a]);
            selected_A(a_r_ck);
            a = 0; // Alpha channel set to R
        }
        private void A_R_MouseEnter(object sender, EventArgs e)
        {
            if (a == 0)
                selected_A(a_r_ck);
            else
                hover_A(a_r_ck);
        }
        private void A_R_MouseLeave(object sender, EventArgs e)
        {
            if (a == 0)
                checked_A(a_r_ck);
            else
                unchecked_A(a_r_ck);
        }
        private void A_G_Click(object sender, EventArgs e)
        {
            unchecked_A(a_ck[a]);
            selected_A(a_g_ck);
            a = 1; // Alpha channel set to G
        }
        private void A_G_MouseEnter(object sender, EventArgs e)
        {
            if (a == 1)
                selected_A(a_g_ck);
            else
                hover_A(a_g_ck);
        }
        private void A_G_MouseLeave(object sender, EventArgs e)
        {
            if (a == 1)
                checked_A(a_g_ck);
            else
                unchecked_A(a_g_ck);
        }
        private void A_B_Click(object sender, EventArgs e)
        {
            unchecked_A(a_ck[a]);
            selected_A(a_b_ck);
            a = 2; // Alpha channel set to B
        }
        private void A_B_MouseEnter(object sender, EventArgs e)
        {
            if (a == 2)
                selected_A(a_b_ck);
            else
                hover_A(a_b_ck);
        }
        private void A_B_MouseLeave(object sender, EventArgs e)
        {
            if (a == 2)
                checked_A(a_b_ck);
            else
                unchecked_A(a_b_ck);
        }
        private void A_A_Click(object sender, EventArgs e)
        {
            unchecked_A(a_ck[a]);
            selected_A(a_a_ck);
            a = 3; // Alpha channel set to A
        }
        private void A_A_MouseEnter(object sender, EventArgs e)
        {
            if (a == 3)
                selected_A(a_a_ck);
            else
                hover_A(a_a_ck);
        }
        private void A_A_MouseLeave(object sender, EventArgs e)
        {
            if (a == 3)
                checked_A(a_a_ck);
            else
                unchecked_A(a_a_ck);
        }
    }
}
