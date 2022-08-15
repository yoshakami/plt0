using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace plt0_gui
{
    public partial class plt0_gui : Form
    {
        public string execPath = AppDomain.CurrentDomain.BaseDirectory;
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
        byte encoding = 7;
        byte cmpr_max = 16;  // number of colours that the program should take care in each 4x4 block - should always be set to 16 for better results.  // wimgt's cmpr encoding is better than mine. I gotta admit. 
        byte WrapS = 1; // 0 = Clamp   1 = Repeat   2 = Mirror
        byte WrapT = 1; // 0 = Clamp   1 = Repeat   2 = Mirror
        byte algorithm = 0;  // 0 = CIE 601    1 = CIE 709     2 = custom RGBA     3 = Most Used Colours (No Gradient)
        byte alpha = 9;  // 0 = no alpha - 1 = alpha - 2 = mix 
        byte color;
        byte cmpr_alpha_threshold = 100;
        byte diversity = 10;
        byte diversity2 = 0;
        byte magnification_filter = 1;  // 0 = Nearest Neighbour   1 = Linear
        byte minificaction_filter = 1;  // 0 = Nearest Neighbour   1 = Linear
        byte mipmaps_number = 0;
        byte round3 = 16;
        byte round4 = 8;
        byte round5 = 4;
        byte round6 = 2;

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
        List<PictureBox> encoding_ck = new List<PictureBox>();
        public plt0_gui()
        {
            InitializeComponent();
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
            if (System.IO.File.Exists(execPath + "images/background.png"))
            {
                this.BackgroundImage = System.Drawing.Image.FromFile(execPath + "images/background.png");
            }
            if (System.IO.File.Exists(execPath + "images/surrounding.png"))
            {
                surrounding_ck.BackgroundImage = System.Drawing.Image.FromFile(execPath + "images/surrounding.png");
            }

        }

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
            this.SuspendLayout();
            // 
            // output_file_type_label
            // 
            this.output_file_type_label.AutoSize = true;
            this.output_file_type_label.BackColor = System.Drawing.Color.Transparent;
            this.output_file_type_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.output_file_type_label.ForeColor = System.Drawing.SystemColors.Control;
            this.output_file_type_label.Location = new System.Drawing.Point(24, 56);
            this.output_file_type_label.Name = "output_file_type_label";
            this.output_file_type_label.Size = new System.Drawing.Size(194, 20);
            this.output_file_type_label.TabIndex = 0;
            this.output_file_type_label.Text = "Output file type";
            // 
            // mandatory_settings_label
            // 
            this.mandatory_settings_label.AutoSize = true;
            this.mandatory_settings_label.BackColor = System.Drawing.Color.Transparent;
            this.mandatory_settings_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mandatory_settings_label.ForeColor = System.Drawing.SystemColors.Control;
            this.mandatory_settings_label.Location = new System.Drawing.Point(123, 20);
            this.mandatory_settings_label.Name = "mandatory_settings_label";
            this.mandatory_settings_label.Size = new System.Drawing.Size(238, 20);
            this.mandatory_settings_label.TabIndex = 123;
            this.mandatory_settings_label.Text = "Mandatory Settings";
            // 
            // bmd_label
            // 
            this.bmd_label.AutoSize = true;
            this.bmd_label.BackColor = System.Drawing.Color.Transparent;
            this.bmd_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.bmd_label.ForeColor = System.Drawing.SystemColors.Window;
            this.bmd_label.Location = new System.Drawing.Point(117, 86);
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
            this.bmd_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.bmd_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.bmd_hitbox.Location = new System.Drawing.Point(46, 86);
            this.bmd_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.bmd_hitbox.Name = "bmd_hitbox";
            this.bmd_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
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
            this.bmd_ck.Location = new System.Drawing.Point(49, 86);
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
            this.bti_ck.Location = new System.Drawing.Point(49, 150);
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
            this.bti_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.bti_label.ForeColor = System.Drawing.SystemColors.Window;
            this.bti_label.Location = new System.Drawing.Point(117, 150);
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
            this.bti_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.bti_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.bti_hitbox.Location = new System.Drawing.Point(46, 150);
            this.bti_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.bti_hitbox.Name = "bti_hitbox";
            this.bti_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
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
            this.tex0_ck.Location = new System.Drawing.Point(49, 214);
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
            this.tex0_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.tex0_label.ForeColor = System.Drawing.SystemColors.Window;
            this.tex0_label.Location = new System.Drawing.Point(117, 215);
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
            this.tex0_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tex0_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.tex0_hitbox.Location = new System.Drawing.Point(46, 214);
            this.tex0_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.tex0_hitbox.Name = "tex0_hitbox";
            this.tex0_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
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
            this.tpl_ck.Location = new System.Drawing.Point(49, 278);
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
            this.tpl_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.tpl_label.ForeColor = System.Drawing.SystemColors.Window;
            this.tpl_label.Location = new System.Drawing.Point(117, 279);
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
            this.tpl_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tpl_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.tpl_hitbox.Location = new System.Drawing.Point(46, 278);
            this.tpl_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.tpl_hitbox.Name = "tpl_hitbox";
            this.tpl_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
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
            this.bmp_ck.Location = new System.Drawing.Point(49, 342);
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
            this.bmp_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.bmp_label.ForeColor = System.Drawing.SystemColors.Window;
            this.bmp_label.Location = new System.Drawing.Point(117, 342);
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
            this.bmp_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.bmp_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.bmp_hitbox.Location = new System.Drawing.Point(46, 342);
            this.bmp_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_hitbox.Name = "bmp_hitbox";
            this.bmp_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
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
            this.png_ck.Location = new System.Drawing.Point(49, 406);
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
            this.png_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.png_label.ForeColor = System.Drawing.SystemColors.Window;
            this.png_label.Location = new System.Drawing.Point(117, 407);
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
            this.png_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.png_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.png_hitbox.Location = new System.Drawing.Point(46, 406);
            this.png_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.png_hitbox.Name = "png_hitbox";
            this.png_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
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
            this.jpg_ck.Location = new System.Drawing.Point(49, 470);
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
            this.jpg_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.jpg_label.ForeColor = System.Drawing.SystemColors.Window;
            this.jpg_label.Location = new System.Drawing.Point(117, 471);
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
            this.jpg_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.jpg_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.jpg_hitbox.Location = new System.Drawing.Point(46, 470);
            this.jpg_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.jpg_hitbox.Name = "jpg_hitbox";
            this.jpg_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
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
            this.tiff_ck.Location = new System.Drawing.Point(49, 791);
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
            this.tiff_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.tiff_label.ForeColor = System.Drawing.SystemColors.Window;
            this.tiff_label.Location = new System.Drawing.Point(117, 792);
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
            this.tiff_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tiff_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.tiff_hitbox.Location = new System.Drawing.Point(46, 791);
            this.tiff_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.tiff_hitbox.Name = "tiff_hitbox";
            this.tiff_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
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
            this.tif_ck.Location = new System.Drawing.Point(49, 727);
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
            this.tif_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.tif_label.ForeColor = System.Drawing.SystemColors.Window;
            this.tif_label.Location = new System.Drawing.Point(117, 728);
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
            this.tif_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tif_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.tif_hitbox.Location = new System.Drawing.Point(46, 727);
            this.tif_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.tif_hitbox.Name = "tif_hitbox";
            this.tif_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
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
            this.ico_ck.Location = new System.Drawing.Point(49, 663);
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
            this.ico_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ico_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ico_label.Location = new System.Drawing.Point(117, 663);
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
            this.ico_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ico_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ico_hitbox.Location = new System.Drawing.Point(46, 663);
            this.ico_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ico_hitbox.Name = "ico_hitbox";
            this.ico_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
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
            this.gif_ck.Location = new System.Drawing.Point(49, 599);
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
            this.gib_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.gib_label.ForeColor = System.Drawing.SystemColors.Window;
            this.gib_label.Location = new System.Drawing.Point(117, 600);
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
            this.gif_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.gif_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.gif_hitbox.Location = new System.Drawing.Point(46, 599);
            this.gif_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.gif_hitbox.Name = "gif_hitbox";
            this.gif_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
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
            this.jpeg_ck.Location = new System.Drawing.Point(49, 535);
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
            this.jpeg_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.jpeg_label.ForeColor = System.Drawing.SystemColors.Window;
            this.jpeg_label.Location = new System.Drawing.Point(117, 536);
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
            this.jpeg_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.jpeg_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.jpeg_hitbox.Location = new System.Drawing.Point(46, 535);
            this.jpeg_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.jpeg_hitbox.Name = "jpeg_hitbox";
            this.jpeg_hitbox.Padding = new System.Windows.Forms.Padding(128, 48, 0, 0);
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
            this.options_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.options_label.ForeColor = System.Drawing.SystemColors.Control;
            this.options_label.Location = new System.Drawing.Point(1057, 56);
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
            this.warn_ck.Location = new System.Drawing.Point(1061, 663);
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
            this.warn_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.warn_label.ForeColor = System.Drawing.SystemColors.Window;
            this.warn_label.Location = new System.Drawing.Point(1130, 664);
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
            this.stfu_ck.Location = new System.Drawing.Point(1061, 599);
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
            this.stfu_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.stfu_label.ForeColor = System.Drawing.SystemColors.Window;
            this.stfu_label.Location = new System.Drawing.Point(1130, 601);
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
            this.safe_mode_ck.Location = new System.Drawing.Point(1061, 535);
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
            this.safe_mode_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.safe_mode_label.ForeColor = System.Drawing.SystemColors.Window;
            this.safe_mode_label.Location = new System.Drawing.Point(1130, 537);
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
            this.reverse_ck.Location = new System.Drawing.Point(1061, 470);
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
            this.reverse_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.reverse_label.ForeColor = System.Drawing.SystemColors.Window;
            this.reverse_label.Location = new System.Drawing.Point(1130, 472);
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
            this.random_ck.Location = new System.Drawing.Point(1061, 406);
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
            this.random_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.random_label.ForeColor = System.Drawing.SystemColors.Window;
            this.random_label.Location = new System.Drawing.Point(1130, 408);
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
            this.no_warning_ck.Location = new System.Drawing.Point(1061, 342);
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
            this.no_warning_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.no_warning_label.ForeColor = System.Drawing.SystemColors.Window;
            this.no_warning_label.Location = new System.Drawing.Point(1130, 343);
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
            this.funky_ck.Location = new System.Drawing.Point(1061, 278);
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
            this.funky_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.funky_label.ForeColor = System.Drawing.SystemColors.Window;
            this.funky_label.Location = new System.Drawing.Point(1130, 280);
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
            this.FORCE_ALPHA_ck.Location = new System.Drawing.Point(1061, 214);
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
            this.FORCE_ALPHA_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.FORCE_ALPHA_label.ForeColor = System.Drawing.SystemColors.Window;
            this.FORCE_ALPHA_label.Location = new System.Drawing.Point(1130, 216);
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
            this.bmp_32_ck.Location = new System.Drawing.Point(1061, 150);
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
            this.bmp_32_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.bmp_32_label.ForeColor = System.Drawing.SystemColors.Window;
            this.bmp_32_label.Location = new System.Drawing.Point(1130, 151);
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
            this.ask_exit_ck.Location = new System.Drawing.Point(1061, 86);
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
            this.ask_exit_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ask_exit_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ask_exit_label.Location = new System.Drawing.Point(1130, 87);
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
            this.ask_exit_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ask_exit_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ask_exit_hitbox.Location = new System.Drawing.Point(1063, 87);
            this.ask_exit_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ask_exit_hitbox.Name = "ask_exit_hitbox";
            this.ask_exit_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.ask_exit_hitbox.Size = new System.Drawing.Size(250, 64);
            this.ask_exit_hitbox.TabIndex = 162;
            this.ask_exit_hitbox.Click += new System.EventHandler(this.ask_exit_Click);
            this.ask_exit_hitbox.MouseEnter += new System.EventHandler(this.ask_exit_MouseEnter);
            this.ask_exit_hitbox.MouseLeave += new System.EventHandler(this.ask_exit_MouseLeave);
            // 
            // bmp_32_hitbox
            // 
            this.bmp_32_hitbox.AutoSize = true;
            this.bmp_32_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.bmp_32_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.bmp_32_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.bmp_32_hitbox.Location = new System.Drawing.Point(1063, 150);
            this.bmp_32_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.bmp_32_hitbox.Name = "bmp_32_hitbox";
            this.bmp_32_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.bmp_32_hitbox.Size = new System.Drawing.Size(250, 64);
            this.bmp_32_hitbox.TabIndex = 191;
            this.bmp_32_hitbox.Click += new System.EventHandler(this.bmp_32_Click);
            this.bmp_32_hitbox.MouseEnter += new System.EventHandler(this.bmp_32_MouseEnter);
            this.bmp_32_hitbox.MouseLeave += new System.EventHandler(this.bmp_32_MouseLeave);
            // 
            // FORCE_ALPHA_hitbox
            // 
            this.FORCE_ALPHA_hitbox.AutoSize = true;
            this.FORCE_ALPHA_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.FORCE_ALPHA_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FORCE_ALPHA_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.FORCE_ALPHA_hitbox.Location = new System.Drawing.Point(1063, 214);
            this.FORCE_ALPHA_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.FORCE_ALPHA_hitbox.Name = "FORCE_ALPHA_hitbox";
            this.FORCE_ALPHA_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.FORCE_ALPHA_hitbox.Size = new System.Drawing.Size(250, 64);
            this.FORCE_ALPHA_hitbox.TabIndex = 192;
            this.FORCE_ALPHA_hitbox.Click += new System.EventHandler(this.FORCE_ALPHA_Click);
            this.FORCE_ALPHA_hitbox.MouseEnter += new System.EventHandler(this.FORCE_ALPHA_MouseEnter);
            this.FORCE_ALPHA_hitbox.MouseLeave += new System.EventHandler(this.FORCE_ALPHA_MouseLeave);
            // 
            // funky_hitbox
            // 
            this.funky_hitbox.AutoSize = true;
            this.funky_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.funky_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.funky_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.funky_hitbox.Location = new System.Drawing.Point(1063, 278);
            this.funky_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.funky_hitbox.Name = "funky_hitbox";
            this.funky_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.funky_hitbox.Size = new System.Drawing.Size(250, 64);
            this.funky_hitbox.TabIndex = 193;
            this.funky_hitbox.Click += new System.EventHandler(this.funky_Click);
            this.funky_hitbox.MouseEnter += new System.EventHandler(this.funky_MouseEnter);
            this.funky_hitbox.MouseLeave += new System.EventHandler(this.funky_MouseLeave);
            // 
            // no_warning_hitbox
            // 
            this.no_warning_hitbox.AutoSize = true;
            this.no_warning_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.no_warning_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.no_warning_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.no_warning_hitbox.Location = new System.Drawing.Point(1063, 343);
            this.no_warning_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.no_warning_hitbox.Name = "no_warning_hitbox";
            this.no_warning_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.no_warning_hitbox.Size = new System.Drawing.Size(250, 64);
            this.no_warning_hitbox.TabIndex = 194;
            this.no_warning_hitbox.Click += new System.EventHandler(this.no_warning_Click);
            this.no_warning_hitbox.MouseEnter += new System.EventHandler(this.no_warning_MouseEnter);
            this.no_warning_hitbox.MouseLeave += new System.EventHandler(this.no_warning_MouseLeave);
            // 
            // random_hitbox
            // 
            this.random_hitbox.AutoSize = true;
            this.random_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.random_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.random_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.random_hitbox.Location = new System.Drawing.Point(1063, 406);
            this.random_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.random_hitbox.Name = "random_hitbox";
            this.random_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.random_hitbox.Size = new System.Drawing.Size(250, 64);
            this.random_hitbox.TabIndex = 195;
            this.random_hitbox.Click += new System.EventHandler(this.random_Click);
            this.random_hitbox.MouseEnter += new System.EventHandler(this.random_MouseEnter);
            this.random_hitbox.MouseLeave += new System.EventHandler(this.random_MouseLeave);
            // 
            // reverse_hitbox
            // 
            this.reverse_hitbox.AutoSize = true;
            this.reverse_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.reverse_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.reverse_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.reverse_hitbox.Location = new System.Drawing.Point(1063, 470);
            this.reverse_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.reverse_hitbox.Name = "reverse_hitbox";
            this.reverse_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.reverse_hitbox.Size = new System.Drawing.Size(250, 64);
            this.reverse_hitbox.TabIndex = 196;
            this.reverse_hitbox.Click += new System.EventHandler(this.reverse_Click);
            this.reverse_hitbox.MouseEnter += new System.EventHandler(this.reverse_MouseEnter);
            this.reverse_hitbox.MouseLeave += new System.EventHandler(this.reverse_MouseLeave);
            // 
            // safe_mode_hitbox
            // 
            this.safe_mode_hitbox.AutoSize = true;
            this.safe_mode_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.safe_mode_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.safe_mode_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.safe_mode_hitbox.Location = new System.Drawing.Point(1063, 537);
            this.safe_mode_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.safe_mode_hitbox.Name = "safe_mode_hitbox";
            this.safe_mode_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.safe_mode_hitbox.Size = new System.Drawing.Size(250, 64);
            this.safe_mode_hitbox.TabIndex = 197;
            this.safe_mode_hitbox.Click += new System.EventHandler(this.safe_mode_Click);
            this.safe_mode_hitbox.MouseEnter += new System.EventHandler(this.safe_mode_MouseEnter);
            this.safe_mode_hitbox.MouseLeave += new System.EventHandler(this.safe_mode_MouseLeave);
            // 
            // stfu_hitbox
            // 
            this.stfu_hitbox.AutoSize = true;
            this.stfu_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.stfu_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.stfu_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.stfu_hitbox.Location = new System.Drawing.Point(1063, 600);
            this.stfu_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.stfu_hitbox.Name = "stfu_hitbox";
            this.stfu_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.stfu_hitbox.Size = new System.Drawing.Size(250, 64);
            this.stfu_hitbox.TabIndex = 198;
            this.stfu_hitbox.Click += new System.EventHandler(this.stfu_Click);
            this.stfu_hitbox.MouseEnter += new System.EventHandler(this.stfu_MouseEnter);
            this.stfu_hitbox.MouseLeave += new System.EventHandler(this.stfu_MouseLeave);
            // 
            // warn_hitbox
            // 
            this.warn_hitbox.AutoSize = true;
            this.warn_hitbox.BackColor = System.Drawing.Color.Transparent;
            this.warn_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.warn_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.warn_hitbox.Location = new System.Drawing.Point(1063, 663);
            this.warn_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.warn_hitbox.Name = "warn_hitbox";
            this.warn_hitbox.Padding = new System.Windows.Forms.Padding(250, 48, 0, 0);
            this.warn_hitbox.Size = new System.Drawing.Size(250, 64);
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
            this.cmpr_ck.Location = new System.Drawing.Point(249, 727);
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
            this.cmpr_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.cmpr_label.ForeColor = System.Drawing.SystemColors.Window;
            this.cmpr_label.Location = new System.Drawing.Point(317, 728);
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
            this.cmpr_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmpr_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.cmpr_hitbox.Location = new System.Drawing.Point(246, 727);
            this.cmpr_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.cmpr_hitbox.Name = "cmpr_hitbox";
            this.cmpr_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
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
            this.ci14x2_ck.Location = new System.Drawing.Point(249, 663);
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
            this.ci14x2_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ci14x2_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ci14x2_label.Location = new System.Drawing.Point(317, 663);
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
            this.ci14x2_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ci14x2_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ci14x2_hitbox.Location = new System.Drawing.Point(246, 663);
            this.ci14x2_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ci14x2_hitbox.Name = "ci14x2_hitbox";
            this.ci14x2_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
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
            this.ci8_ck.Location = new System.Drawing.Point(249, 599);
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
            this.ci8_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ci8_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ci8_label.Location = new System.Drawing.Point(317, 600);
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
            this.ci8_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ci8_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ci8_hitbox.Location = new System.Drawing.Point(246, 599);
            this.ci8_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ci8_hitbox.Name = "ci8_hitbox";
            this.ci8_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
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
            this.ci4_ck.Location = new System.Drawing.Point(249, 535);
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
            this.ci4_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ci4_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ci4_label.Location = new System.Drawing.Point(317, 536);
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
            this.ci4_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ci4_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ci4_hitbox.Location = new System.Drawing.Point(246, 535);
            this.ci4_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ci4_hitbox.Name = "ci4_hitbox";
            this.ci4_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
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
            this.rgba32_ck.Location = new System.Drawing.Point(249, 470);
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
            this.rgba32_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.rgba32_label.ForeColor = System.Drawing.SystemColors.Window;
            this.rgba32_label.Location = new System.Drawing.Point(317, 471);
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
            this.rgba32_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rgba32_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.rgba32_hitbox.Location = new System.Drawing.Point(246, 470);
            this.rgba32_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.rgba32_hitbox.Name = "rgba32_hitbox";
            this.rgba32_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
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
            this.rgb5a3_ck.Location = new System.Drawing.Point(249, 406);
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
            this.rgb5a3_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.rgb5a3_label.ForeColor = System.Drawing.SystemColors.Window;
            this.rgb5a3_label.Location = new System.Drawing.Point(317, 407);
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
            this.rgb5a3_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rgb5a3_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.rgb5a3_hitbox.Location = new System.Drawing.Point(246, 406);
            this.rgb5a3_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.rgb5a3_hitbox.Name = "rgb5a3_hitbox";
            this.rgb5a3_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
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
            this.rgb565_ck.Location = new System.Drawing.Point(249, 342);
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
            this.rgb565_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.rgb565_label.ForeColor = System.Drawing.SystemColors.Window;
            this.rgb565_label.Location = new System.Drawing.Point(317, 342);
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
            this.rgb565_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rgb565_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.rgb565_hitbox.Location = new System.Drawing.Point(246, 342);
            this.rgb565_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.rgb565_hitbox.Name = "rgb565_hitbox";
            this.rgb565_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
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
            this.ai8_ck.Location = new System.Drawing.Point(249, 278);
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
            this.ai8_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ai8_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ai8_label.Location = new System.Drawing.Point(317, 279);
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
            this.ai8_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ai8_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ai8_hitbox.Location = new System.Drawing.Point(246, 278);
            this.ai8_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ai8_hitbox.Name = "ai8_hitbox";
            this.ai8_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
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
            this.ai4_ck.Location = new System.Drawing.Point(249, 214);
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
            this.ai4_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.ai4_label.ForeColor = System.Drawing.SystemColors.Window;
            this.ai4_label.Location = new System.Drawing.Point(317, 215);
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
            this.ai4_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ai4_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.ai4_hitbox.Location = new System.Drawing.Point(246, 214);
            this.ai4_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.ai4_hitbox.Name = "ai4_hitbox";
            this.ai4_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
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
            this.i8_ck.Location = new System.Drawing.Point(249, 150);
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
            this.i8_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.i8_label.ForeColor = System.Drawing.SystemColors.Window;
            this.i8_label.Location = new System.Drawing.Point(317, 150);
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
            this.i8_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.i8_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.i8_hitbox.Location = new System.Drawing.Point(246, 150);
            this.i8_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.i8_hitbox.Name = "i8_hitbox";
            this.i8_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
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
            this.i4_ck.Location = new System.Drawing.Point(249, 86);
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
            this.i4_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)), true);
            this.i4_label.ForeColor = System.Drawing.SystemColors.Window;
            this.i4_label.Location = new System.Drawing.Point(317, 86);
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
            this.i4_hitbox.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.i4_hitbox.ForeColor = System.Drawing.SystemColors.Control;
            this.i4_hitbox.Location = new System.Drawing.Point(246, 86);
            this.i4_hitbox.Margin = new System.Windows.Forms.Padding(0);
            this.i4_hitbox.Name = "i4_hitbox";
            this.i4_hitbox.Padding = new System.Windows.Forms.Padding(190, 48, 0, 0);
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
            this.encoding_label.Font = new System.Drawing.Font("NintendoP-NewRodin DB", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.encoding_label.ForeColor = System.Drawing.SystemColors.Control;
            this.encoding_label.Location = new System.Drawing.Point(297, 56);
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
            this.surrounding_ck.Location = new System.Drawing.Point(18, 30);
            this.surrounding_ck.Margin = new System.Windows.Forms.Padding(0);
            this.surrounding_ck.Name = "surrounding_ck";
            this.surrounding_ck.Size = new System.Drawing.Size(453, 840);
            this.surrounding_ck.TabIndex = 234;
            this.surrounding_ck.TabStop = false;
            // 
            // plt0_gui
            // 
            this.AllowDrop = true;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1323, 1041);
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1920, 1080);
            this.Name = "plt0_gui";
            this.Text = "PLT0 - Image Encoding tool";
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
            this.ResumeLayout(false);
            this.PerformLayout();

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
            if (System.IO.File.Exists(execPath + "images/white_box.png"))
            {
                checkbox.BackgroundImage = System.Drawing.Image.FromFile(execPath + "images/white_box.png");
            }
        }
        private void checked_checkbox(PictureBox checkbox)
        {
            if (System.IO.File.Exists(execPath + "images/check.png"))
            {
                checkbox.BackgroundImage = System.Drawing.Image.FromFile(execPath + "images/check.png");
            }
        }
        private void hover_checkbox(PictureBox checkbox)
        {
            if (System.IO.File.Exists(execPath + "images/green_box.png"))
            {
                checkbox.BackgroundImage = System.Drawing.Image.FromFile(execPath + "images/green_box.png");
            }
        }

        private void plt0_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }



        // generated checkbox behaviour code by the python script in the py folder
        private void bmd_Click(object sender, EventArgs e)
        {
            if (bmd)
            {
                bmd = false;
                unchecked_checkbox(bmd_ck);
            }
            else
            {
                bmd = true;
                checked_checkbox(bmd_ck);
            }
        }
        private void bmd_MouseEnter(object sender, EventArgs e)
        {
            if (!bmd)
                hover_checkbox(bmd_ck);
        }
        private void bmd_MouseLeave(object sender, EventArgs e)
        {
            if (!bmd)
                unchecked_checkbox(bmd_ck);
        }
        private void bti_Click(object sender, EventArgs e)
        {
            if (bti)
            {
                bti = false;
                unchecked_checkbox(bti_ck);
            }
            else
            {
                bti = true;
                checked_checkbox(bti_ck);
            }
        }
        private void bti_MouseEnter(object sender, EventArgs e)
        {
            if (!bti)
                hover_checkbox(bti_ck);
        }
        private void bti_MouseLeave(object sender, EventArgs e)
        {
            if (!bti)
                unchecked_checkbox(bti_ck);
        }
        private void tex0_Click(object sender, EventArgs e)
        {
            if (tex0)
            {
                tex0 = false;
                unchecked_checkbox(tex0_ck);
            }
            else
            {
                tex0 = true;
                checked_checkbox(tex0_ck);
            }
        }
        private void tex0_MouseEnter(object sender, EventArgs e)
        {
            if (!tex0)
                hover_checkbox(tex0_ck);
        }
        private void tex0_MouseLeave(object sender, EventArgs e)
        {
            if (!tex0)
                unchecked_checkbox(tex0_ck);
        }
        private void tpl_Click(object sender, EventArgs e)
        {
            if (tpl)
            {
                tpl = false;
                unchecked_checkbox(tpl_ck);
            }
            else
            {
                tpl = true;
                checked_checkbox(tpl_ck);
            }
        }
        private void tpl_MouseEnter(object sender, EventArgs e)
        {
            if (!tpl)
                hover_checkbox(tpl_ck);
        }
        private void tpl_MouseLeave(object sender, EventArgs e)
        {
            if (!tpl)
                unchecked_checkbox(tpl_ck);
        }
        private void bmp_Click(object sender, EventArgs e)
        {
            if (bmp)
            {
                bmp = false;
                unchecked_checkbox(bmp_ck);
            }
            else
            {
                bmp = true;
                checked_checkbox(bmp_ck);
            }
        }
        private void bmp_MouseEnter(object sender, EventArgs e)
        {
            if (!bmp)
                hover_checkbox(bmp_ck);
        }
        private void bmp_MouseLeave(object sender, EventArgs e)
        {
            if (!bmp)
                unchecked_checkbox(bmp_ck);
        }
        private void png_Click(object sender, EventArgs e)
        {
            if (png)
            {
                png = false;
                unchecked_checkbox(png_ck);
            }
            else
            {
                png = true;
                checked_checkbox(png_ck);
            }
        }
        private void png_MouseEnter(object sender, EventArgs e)
        {
            if (!png)
                hover_checkbox(png_ck);
        }
        private void png_MouseLeave(object sender, EventArgs e)
        {
            if (!png)
                unchecked_checkbox(png_ck);
        }
        private void jpg_Click(object sender, EventArgs e)
        {
            if (jpg)
            {
                jpg = false;
                unchecked_checkbox(jpg_ck);
            }
            else
            {
                jpg = true;
                checked_checkbox(jpg_ck);
            }
        }
        private void jpg_MouseEnter(object sender, EventArgs e)
        {
            if (!jpg)
                hover_checkbox(jpg_ck);
        }
        private void jpg_MouseLeave(object sender, EventArgs e)
        {
            if (!jpg)
                unchecked_checkbox(jpg_ck);
        }
        private void jpeg_Click(object sender, EventArgs e)
        {
            if (jpeg)
            {
                jpeg = false;
                unchecked_checkbox(jpeg_ck);
            }
            else
            {
                jpeg = true;
                checked_checkbox(jpeg_ck);
            }
        }
        private void jpeg_MouseEnter(object sender, EventArgs e)
        {
            if (!jpeg)
                hover_checkbox(jpeg_ck);
        }
        private void jpeg_MouseLeave(object sender, EventArgs e)
        {
            if (!jpeg)
                unchecked_checkbox(jpeg_ck);
        }
        private void gif_Click(object sender, EventArgs e)
        {
            if (gif)
            {
                gif = false;
                unchecked_checkbox(gif_ck);
            }
            else
            {
                gif = true;
                checked_checkbox(gif_ck);
            }
        }
        private void gif_MouseEnter(object sender, EventArgs e)
        {
            if (!gif)
                hover_checkbox(gif_ck);
        }
        private void gif_MouseLeave(object sender, EventArgs e)
        {
            if (!gif)
                unchecked_checkbox(gif_ck);
        }
        private void ico_Click(object sender, EventArgs e)
        {
            if (ico)
            {
                ico = false;
                unchecked_checkbox(ico_ck);
            }
            else
            {
                ico = true;
                checked_checkbox(ico_ck);
            }
        }
        private void ico_MouseEnter(object sender, EventArgs e)
        {
            if (!ico)
                hover_checkbox(ico_ck);
        }
        private void ico_MouseLeave(object sender, EventArgs e)
        {
            if (!ico)
                unchecked_checkbox(ico_ck);
        }
        private void tif_Click(object sender, EventArgs e)
        {
            if (tif)
            {
                tif = false;
                unchecked_checkbox(tif_ck);
            }
            else
            {
                tif = true;
                checked_checkbox(tif_ck);
            }
        }
        private void tif_MouseEnter(object sender, EventArgs e)
        {
            if (!tif)
                hover_checkbox(tif_ck);
        }
        private void tif_MouseLeave(object sender, EventArgs e)
        {
            if (!tif)
                unchecked_checkbox(tif_ck);
        }
        private void tiff_Click(object sender, EventArgs e)
        {
            if (tiff)
            {
                tiff = false;
                unchecked_checkbox(tiff_ck);
            }
            else
            {
                tiff = true;
                checked_checkbox(tiff_ck);
            }
        }
        private void tiff_MouseEnter(object sender, EventArgs e)
        {
            if (!tiff)
                hover_checkbox(tiff_ck);
        }
        private void tiff_MouseLeave(object sender, EventArgs e)
        {
            if (!tiff)
                unchecked_checkbox(tiff_ck);
        }
        private void no_warning_Click(object sender, EventArgs e)
        {
            if (no_warning)
            {
                no_warning = false;
                unchecked_checkbox(no_warning_ck);
            }
            else
            {
                no_warning = true;
                checked_checkbox(no_warning_ck);
            }
        }
        private void no_warning_MouseEnter(object sender, EventArgs e)
        {
            if (!no_warning)
                hover_checkbox(no_warning_ck);
        }
        private void no_warning_MouseLeave(object sender, EventArgs e)
        {
            if (!no_warning)
                unchecked_checkbox(no_warning_ck);
        }
        private void warn_Click(object sender, EventArgs e)
        {
            if (warn)
            {
                warn = false;
                unchecked_checkbox(warn_ck);
            }
            else
            {
                warn = true;
                checked_checkbox(warn_ck);
            }
        }
        private void warn_MouseEnter(object sender, EventArgs e)
        {
            if (!warn)
                hover_checkbox(warn_ck);
        }
        private void warn_MouseLeave(object sender, EventArgs e)
        {
            if (!warn)
                unchecked_checkbox(warn_ck);
        }
        private void funky_Click(object sender, EventArgs e)
        {
            if (funky)
            {
                funky = false;
                unchecked_checkbox(funky_ck);
            }
            else
            {
                funky = true;
                checked_checkbox(funky_ck);
            }
        }
        private void funky_MouseEnter(object sender, EventArgs e)
        {
            if (!funky)
                hover_checkbox(funky_ck);
        }
        private void funky_MouseLeave(object sender, EventArgs e)
        {
            if (!funky)
                unchecked_checkbox(funky_ck);
        }
        private void stfu_Click(object sender, EventArgs e)
        {
            if (stfu)
            {
                stfu = false;
                unchecked_checkbox(stfu_ck);
            }
            else
            {
                stfu = true;
                checked_checkbox(stfu_ck);
            }
        }
        private void stfu_MouseEnter(object sender, EventArgs e)
        {
            if (!stfu)
                hover_checkbox(stfu_ck);
        }
        private void stfu_MouseLeave(object sender, EventArgs e)
        {
            if (!stfu)
                unchecked_checkbox(stfu_ck);
        }
        private void safe_mode_Click(object sender, EventArgs e)
        {
            if (safe_mode)
            {
                safe_mode = false;
                unchecked_checkbox(safe_mode_ck);
            }
            else
            {
                safe_mode = true;
                checked_checkbox(safe_mode_ck);
            }
        }
        private void safe_mode_MouseEnter(object sender, EventArgs e)
        {
            if (!safe_mode)
                hover_checkbox(safe_mode_ck);
        }
        private void safe_mode_MouseLeave(object sender, EventArgs e)
        {
            if (!safe_mode)
                unchecked_checkbox(safe_mode_ck);
        }
        private void FORCE_ALPHA_Click(object sender, EventArgs e)
        {
            if (FORCE_ALPHA)
            {
                FORCE_ALPHA = false;
                unchecked_checkbox(FORCE_ALPHA_ck);
            }
            else
            {
                FORCE_ALPHA = true;
                checked_checkbox(FORCE_ALPHA_ck);
            }
        }
        private void FORCE_ALPHA_MouseEnter(object sender, EventArgs e)
        {
            if (!FORCE_ALPHA)
                hover_checkbox(FORCE_ALPHA_ck);
        }
        private void FORCE_ALPHA_MouseLeave(object sender, EventArgs e)
        {
            if (!FORCE_ALPHA)
                unchecked_checkbox(FORCE_ALPHA_ck);
        }
        private void ask_exit_Click(object sender, EventArgs e)
        {
            if (ask_exit)
            {
                ask_exit = false;
                unchecked_checkbox(ask_exit_ck);
            }
            else
            {
                ask_exit = true;
                checked_checkbox(ask_exit_ck);
            }
        }
        private void ask_exit_MouseEnter(object sender, EventArgs e)
        {
            if (!ask_exit)
                hover_checkbox(ask_exit_ck);
        }
        private void ask_exit_MouseLeave(object sender, EventArgs e)
        {
            if (!ask_exit)
                unchecked_checkbox(ask_exit_ck);
        }
        private void bmp_32_Click(object sender, EventArgs e)
        {
            if (bmp_32)
            {
                bmp_32 = false;
                unchecked_checkbox(bmp_32_ck);
            }
            else
            {
                bmp_32 = true;
                checked_checkbox(bmp_32_ck);
            }
        }
        private void bmp_32_MouseEnter(object sender, EventArgs e)
        {
            if (!bmp_32)
                hover_checkbox(bmp_32_ck);
        }
        private void bmp_32_MouseLeave(object sender, EventArgs e)
        {
            if (!bmp_32)
                unchecked_checkbox(bmp_32_ck);
        }
        private void reverse_Click(object sender, EventArgs e)
        {
            if (reverse)
            {
                reverse = false;
                unchecked_checkbox(reverse_ck);
            }
            else
            {
                reverse = true;
                checked_checkbox(reverse_ck);
            }
        }
        private void reverse_MouseEnter(object sender, EventArgs e)
        {
            if (!reverse)
                hover_checkbox(reverse_ck);
        }
        private void reverse_MouseLeave(object sender, EventArgs e)
        {
            if (!reverse)
                unchecked_checkbox(reverse_ck);
        }
        private void random_Click(object sender, EventArgs e)
        {
            if (random)
            {
                random = false;
                unchecked_checkbox(random_ck);
            }
            else
            {
                random = true;
                checked_checkbox(random_ck);
            }
        }
        private void random_MouseEnter(object sender, EventArgs e)
        {
            if (!random)
                hover_checkbox(random_ck);
        }
        private void random_MouseLeave(object sender, EventArgs e)
        {
            if (!random)
                unchecked_checkbox(random_ck);
        }




        // implementation of radio buttons
        private void unchecked_encoding(PictureBox radiobutton)
        {
            if (System.IO.File.Exists(execPath + "images/violet_circle.png"))
            {
                radiobutton.BackgroundImage = System.Drawing.Image.FromFile(execPath + "images/violet_circle.png");
            }
        }
        private void checked_encoding(PictureBox radiobutton)
        {
            if (System.IO.File.Exists(execPath + "images/violet_circle_on.png"))
            {
                radiobutton.BackgroundImage = System.Drawing.Image.FromFile(execPath + "images/violet_circle_on.png");
            }
        }
        private void hover_encoding(PictureBox radiobutton)
        {
            if (System.IO.File.Exists(execPath + "images/pink_circle.png"))
            {
                radiobutton.BackgroundImage = System.Drawing.Image.FromFile(execPath + "images/pink_circle.png");
            }
        }

        // code generated by the python script
        private void I4_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            checked_encoding(i4_ck);
            encoding = 0; // I4
        }
        private void I4_MouseEnter(object sender, EventArgs e)
        {
            if (encoding != 0)
                hover_encoding(i4_ck);
        }
        private void I4_MouseLeave(object sender, EventArgs e)
        {
            if (encoding != 0)
                unchecked_encoding(i4_ck);
        }
        private void I8_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            checked_encoding(i8_ck);
            encoding = 1; // I8
        }
        private void I8_MouseEnter(object sender, EventArgs e)
        {
            if (encoding != 1)
                hover_encoding(i8_ck);
        }
        private void I8_MouseLeave(object sender, EventArgs e)
        {
            if (encoding != 1)
                unchecked_encoding(i8_ck);
        }
        private void AI4_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            checked_encoding(ai4_ck);
            encoding = 2; // AI4
        }
        private void AI4_MouseEnter(object sender, EventArgs e)
        {
            if (encoding != 2)
                hover_encoding(ai4_ck);
        }
        private void AI4_MouseLeave(object sender, EventArgs e)
        {
            if (encoding != 2)
                unchecked_encoding(ai4_ck);
        }
        private void AI8_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            checked_encoding(ai8_ck);
            encoding = 3; // AI8
        }
        private void AI8_MouseEnter(object sender, EventArgs e)
        {
            if (encoding != 3)
                hover_encoding(ai8_ck);
        }
        private void AI8_MouseLeave(object sender, EventArgs e)
        {
            if (encoding != 3)
                unchecked_encoding(ai8_ck);
        }
        private void RGB565_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            checked_encoding(rgb565_ck);
            encoding = 4; // RGB565
        }
        private void RGB565_MouseEnter(object sender, EventArgs e)
        {
            if (encoding != 4)
                hover_encoding(rgb565_ck);
        }
        private void RGB565_MouseLeave(object sender, EventArgs e)
        {
            if (encoding != 4)
                unchecked_encoding(rgb565_ck);
        }
        private void RGB5A3_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            checked_encoding(rgb5a3_ck);
            encoding = 5; // RGB5A3
        }
        private void RGB5A3_MouseEnter(object sender, EventArgs e)
        {
            if (encoding != 5)
                hover_encoding(rgb5a3_ck);
        }
        private void RGB5A3_MouseLeave(object sender, EventArgs e)
        {
            if (encoding != 5)
                unchecked_encoding(rgb5a3_ck);
        }
        private void RGBA32_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            checked_encoding(rgba32_ck);
            encoding = 6; // RGBA32
        }
        private void RGBA32_MouseEnter(object sender, EventArgs e)
        {
            if (encoding != 6)
                hover_encoding(rgba32_ck);
        }
        private void RGBA32_MouseLeave(object sender, EventArgs e)
        {
            if (encoding != 6)
                unchecked_encoding(rgba32_ck);
        }
        private void CI4_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            checked_encoding(ci4_ck);
            encoding = 8; // CI4
        }
        private void CI4_MouseEnter(object sender, EventArgs e)
        {
            if (encoding != 8)
                hover_encoding(ci4_ck);
        }
        private void CI4_MouseLeave(object sender, EventArgs e)
        {
            if (encoding != 8)
                unchecked_encoding(ci4_ck);
        }
        private void CI8_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            checked_encoding(ci8_ck);
            encoding = 9; // CI8
        }
        private void CI8_MouseEnter(object sender, EventArgs e)
        {
            if (encoding != 9)
                hover_encoding(ci8_ck);
        }
        private void CI8_MouseLeave(object sender, EventArgs e)
        {
            if (encoding != 9)
                unchecked_encoding(ci8_ck);
        }
        private void CI14X2_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            checked_encoding(ci14x2_ck);
            encoding = 10; // CI14X2
        }
        private void CI14X2_MouseEnter(object sender, EventArgs e)
        {
            if (encoding != 10)
                hover_encoding(ci14x2_ck);
        }
        private void CI14X2_MouseLeave(object sender, EventArgs e)
        {
            if (encoding != 10)
                unchecked_encoding(ci14x2_ck);
        }
        private void CMPR_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            checked_encoding(cmpr_ck);
            encoding = 14; // CMPR
        }
        private void CMPR_MouseEnter(object sender, EventArgs e)
        {
            if (encoding != 14)
                hover_encoding(cmpr_ck);
        }
        private void CMPR_MouseLeave(object sender, EventArgs e)
        {
            if (encoding != 14)
                unchecked_encoding(cmpr_ck);
        }
    }
}
