import pyperclip
# the goal of this code is to be able to easily change variable or function names in the designer, and generate them in bulk. also adds code to load images declared 
scrapped = "" # for scrapped functions
output = """        private void Load_Images()
        {"""
x = 118  # settings start line minus two
with open("X:\\yoshi\\3D Objects\\C#\\plt0\\plt0\\plt0-v.cs", "r") as cs:
    text = cs.read()
    text = text.splitlines()
w = 0
while (text[w][:14] != "        Image "):
    w += 1
while (text[w][:14] == "        Image "):
    output += """
            if (File.Exists(execPath + "images/""" + text[w][14:-1] + """.png"))
            {
                """ + text[w][14:-1] + """ = Image.FromFile(execPath + "images/""" + text[w][14:-1] + """.png");
            }"""
    w += 1
output += """
        }"""
w = -1
booleans = ["bmd", "bti", "tex0", "tpl", "bmp", "png", "jpg", "jpeg", "gif", "ico", "tif", "tiff", "ask_exit", "bmp_32", "FORCE_ALPHA", "funky", "name_string", "no_warning", "random", "reversex", "reversey", "safe_mode", "stfu", "warn", "cmpr_hover", "cmpr_update_preview"]
organize_args = ["""
                Organize_args();"""] * 24 + ['', '']
check_run = ["\n            Check_run();"] * 12 + [""] * 30
layout_auto = ["", "\n            View_WrapS();\n            View_WrapT();\n            View_min();\n            View_mag();"] * 2 + [""] * 10 + ["\n            Preview(false);"] * 7 + ["", "", "", """
            cmpr_hover_colour.Visible = true;
            cmpr_hover_colour_label.Visible = true;
            cmpr_hover_colour_txt.Visible = true;""", "\n            Preview_Paint();"]
layout_auto2 = ["", "\n            Hide_WrapS();\n            Hide_WrapT();\n            Hide_min();\n            Hide_mag();"] * 2 + [""] * 10 + ["\n            Preview(false);"] * 7+ ["", "", "", """
            cmpr_hover_colour.Visible = false;
            cmpr_hover_colour_label.Visible = false;
            cmpr_hover_colour_txt.Visible = false;""", ""]
for y in booleans:
    x += 1
    w += 1
    output += """
        private void """ + y + """_Click(object sender, EventArgs e)
        {
            if (""" + y + """)
            {
                """ + y + " = false;" + organize_args[w] + """
                hover_checkbox(""" + y + "_ck);" + layout_auto2[w] + """
            }
            else
            {
                """ + y + " = true;" + organize_args[w] + """
                selected_checkbox(""" + y + "_ck);" + layout_auto[w] + """
            }""" + check_run[w] + """
        }
        private void """ + y + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (""" + y + """)
                selected_checkbox(""" + y + """_ck);
            else
                hover_checkbox(""" + y + """_ck);
        }
        private void """ + y + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (""" + y + """)
                checked_checkbox(""" + y + """_ck);
            else
                unchecked_checkbox(""" + y + """_ck);
        }"""
encoding = ["i4", "i8", "ai4", "ai8", "rgb565", "rgb5a3", "rgba32", "", "ci4", "ci8", "ci14x2", "", "", "", "cmpr"]
check_run[7] = check_run[11] = check_run[12] = check_run[13] = ""
for z in range(len(encoding)):
    if encoding[z] == "":
        continue
    x += 1
    output += """
        private void """ + encoding[z].upper() + """_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            Hide_encoding(encoding);
            selected_encoding(""" + encoding[z] + """_ck);
            encoding = """ + str(z) + """; // """ + encoding[z].upper() + check_run[z] + """
            View_""" + encoding[z] + """();
            Organize_args();
            Preview(false);
        }
        private void """ + encoding[z].upper() + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (encoding == """ + str(z) + """)
                selected_encoding(""" + encoding[z] + """_ck);
            else
                hover_encoding(""" + encoding[z] + """_ck);
        }
        private void """ + encoding[z].upper() + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (encoding == """ + str(z) + """)
                checked_encoding(""" + encoding[z] + """_ck);
            else
                unchecked_encoding(""" + encoding[z] + """_ck);
        }"""
x += 1
algorithm = ["Cie_601", "Cie_709", "Custom", "Darkest_Lightest", "No_gradient", "Weemm", "SooperBMD", "Min_Max"]
layout2 = ["","", "\n            View_rgba();", "", "\n            View_No_Gradient();", "", "", "", ""]
rgba = ["""
            if (encoding == 14)
                Parse_Markdown(lines[""" + str(x) + """]);
            else
                """, """
            if (encoding == 14)
                Parse_Markdown(lines[""" + str(x + 1) + """]);
            else
                """, """
            if (encoding == 14)
                Parse_Markdown(lines[""" + str(x + 2) + """]);
            else
                """, """
            if (encoding == 14)
                Parse_Markdown(lines[""" + str(x + 3) + """]);
            else
                """, "", "", "", "", "", "", ""]
x += 3
for a in range(len(algorithm)):
    x += 1
    output += """
        private void """ + algorithm[a] + """_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            Hide_algorithm(algorithm);
            selected_algorithm(""" + algorithm[a].lower() + """_ck);
            algorithm = """ + str(a) + """; // """ + algorithm[a] + """
            Organize_args();""" + layout2[a] + """
            Preview(false);
        }
        private void """ + algorithm[a] + """_MouseEnter(object sender, EventArgs e)
        {
            """ + rgba[a] + """Parse_Markdown(lines[""" + str(x) + """]);
            if (algorithm == """ + str(a) + """)
                selected_algorithm(""" + algorithm[a].lower() + """_ck);
            else
                hover_algorithm(""" + algorithm[a].lower() + """_ck);
        }
        private void """ + algorithm[a] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (algorithm == """ + str(a) + """)
                checked_algorithm(""" + algorithm[a].lower() + """_ck);
            else
                unchecked_algorithm(""" + algorithm[a].lower() + """_ck);
        }"""
alpha = ["No_alpha", "Alpha", "Mix"]
for b in range(len(alpha)):
    x += 1
    output += """
        private void """ + alpha[b] + """_Click(object sender, EventArgs e)
        {
            unchecked_alpha(alpha_ck_array[alpha]);
            selected_alpha(""" + alpha[b].lower() + """_ck);
            alpha = """ + str(b) + """; // """ + alpha[b] + """
            Organize_args();
            Preview(false);
        }
        private void """ + alpha[b] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (alpha == """ + str(b) + """)
                selected_alpha(""" + alpha[b].lower() + """_ck);
            else
                hover_alpha(""" + alpha[b].lower() + """_ck);
        }
        private void """ + alpha[b] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (alpha == """ + str(b) + """)
                checked_alpha(""" + alpha[b].lower() + """_ck);
            else
                unchecked_alpha(""" + alpha[b].lower() + """_ck);
        }"""
wrap = ["Clamp", "Repeat", "Mirror"]
for c in range(3):
    x += 1
    output += """
        private void WrapS_""" + wrap[c] + """_Click(object sender, EventArgs e)
        {
            unchecked_WrapS(WrapS_ck[WrapS]);
            selected_WrapS(S""" + wrap[c].lower() + """_ck);
            WrapS = """ + str(c) + """; // """ + wrap[c] + """
            Organize_args();
        }
        private void WrapS_""" + wrap[c] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (WrapS == """ + str(c) + """)
                selected_WrapS(S""" + wrap[c].lower() + """_ck);
            else
                hover_WrapS(S""" + wrap[c].lower() + """_ck);
        }
        private void WrapS_""" + wrap[c] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (WrapS == """ + str(c) + """)
                checked_WrapS(S""" + wrap[c].lower() + """_ck);
            else
                unchecked_WrapS(S""" + wrap[c].lower() + """_ck);
        }"""
for d in range(3):
    x += 1
    output += """
        private void WrapT_""" + wrap[d] + """_Click(object sender, EventArgs e)
        {
            unchecked_WrapT(WrapT_ck[WrapT]);
            selected_WrapT(T""" + wrap[d].lower() + """_ck);
            WrapT = """ + str(d) + """; // """ + wrap[d] + """
            Organize_args();
        }
        private void WrapT_""" + wrap[d] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (WrapT == """ + str(d) + """)
                selected_WrapT(T""" + wrap[d].lower() + """_ck);
            else
                hover_WrapT(T""" + wrap[d].lower() + """_ck);
        }
        private void WrapT_""" + wrap[d] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (WrapT == """ + str(d) + """)
                checked_WrapT(T""" + wrap[d].lower() + """_ck);
            else
                unchecked_WrapT(T""" + wrap[d].lower() + """_ck);
        }"""
filter = ["Nearest_Neighbour", "Linear", "NearestMipmapNearest", "NearestMipmapLinear", "LinearMipmapNearest", "LinearMipmapLinear"]
for e in range(6):
    x += 1
    output += """
        private void Minification_""" + filter[e] + """_Click(object sender, EventArgs e)
        {
            unchecked_Minification(minification_ck[minification_filter]);
            selected_Minification(min_""" + filter[e].lower() + """_ck);
            minification_filter = """ + str(e) + """; // """ + filter[e] + """
            Organize_args();
        }
        private void Minification_""" + filter[e] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (minification_filter == """ + str(e) + """)
                selected_Minification(min_""" + filter[e].lower() + """_ck);
            else
                hover_Minification(min_""" + filter[e].lower() + """_ck);
        }
        private void Minification_""" + filter[e] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (minification_filter == """ + str(e) + """)
                checked_Minification(min_""" + filter[e].lower() + """_ck);
            else
                unchecked_Minification(min_""" + filter[e].lower() + """_ck);
        }"""
for f in range(6):
    x += 1
    output += """
        private void Magnification_""" + filter[f] + """_Click(object sender, EventArgs e)
        {
            unchecked_Magnification(magnification_ck[magnification_filter]);
            selected_Magnification(mag_""" + filter[f].lower() + """_ck);
            magnification_filter = """ + str(f) + """; // Mag_""" + filter[f] + """
            Organize_args();
        }
        private void Magnification_""" + filter[f] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (magnification_filter == """ + str(f) + """)
                selected_Magnification(mag_""" + filter[f].lower() + """_ck);
            else
                hover_Magnification(mag_""" + filter[f].lower() + """_ck);
        }
        private void Magnification_""" + filter[f] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (magnification_filter == """ + str(f) + """)
                checked_Magnification(mag_""" + filter[f].lower() + """_ck);
            else
                unchecked_Magnification(mag_""" + filter[f].lower() + """_ck);
        }"""
channel = ["R", "G", "B", "A"]
channel2 = ["Red", "Green", "Blue", "Alpha"]
i = -1
for g in channel:  # this looks unreadable because it's packed up instead of pasting 4 times the h loop
    i += 1
    x += 1
    for h in range(4):
        output += """
        private void """ + g + '_' + channel[h] + """_Click(object sender, EventArgs e)
        {
            switch (""" + g.lower() + """)
            {
                case 0:
                    unchecked_R(""" + g.lower() + "_ck[" + g.lower() + """]);
                    break;
                case 1:
                    unchecked_G(""" + g.lower() + "_ck[" + g.lower() + """]);
                    break;
                case 2:
                    unchecked_B(""" + g.lower() + "_ck[" + g.lower() + """]);
                    break;
                case 3:
                    unchecked_A(""" + g.lower() + "_ck[" + g.lower() + """]);
                    break;
            }
            selected_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
            """ + g.lower() + " = " + str(h) + "; // " + channel2[i] + " channel set to " + channel[h] + """
            Organize_args();
            Preview(false);
        }
        private void """ + g + '_' + channel[h] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (""" + g.lower() + " == " + str(h) + """)
                selected_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
            else
                hover_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
        }
        private void """ + g + '_' + channel[h] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (""" + g.lower() + " == " + str(h) + """)
                checked_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
            else
                unchecked_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
        }"""
lyt = ["all", "auto", "preview", "paint"]
Lyt = ["All", "Auto", "Preview", "Paint"]
for k in range(len(lyt)):
    x += 1
    output += """
        private void """ + Lyt[k] + """_Click(object sender, EventArgs e)
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
            selected_""" + Lyt[k] + """();
            Layout_""" + Lyt[k] + """();
        }
        private void """ + Lyt[k] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (layout == """ + str(k) + """)
                selected_""" + Lyt[k] + """();
            else
                hover_""" + Lyt[k] + """();
        }
        private void """ + Lyt[k] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (layout == """ + str(k) + """)
                checked_""" + Lyt[k] + """();
            else
                unchecked_""" + Lyt[k] + """();
        }
        private void checked_""" + Lyt[k] + """()
        {
            """ + lyt[k] + "_ck.BackgroundImage = " + lyt[k] + """_on;
        }
        private void unchecked_""" + Lyt[k] + """()
        {
            """ + lyt[k] + "_ck.BackgroundImage = " + lyt[k] + """_off;
        }
        private void hover_""" + Lyt[k] + """()
        {
            """ + lyt[k] + "_ck.BackgroundImage = " + lyt[k] + """_hover;
        }
        private void selected_""" + Lyt[k] + """()
        {
            """ + lyt[k] + "_ck.BackgroundImage = " + lyt[k] + """_selected;
        }"""
output += """
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
        }"""
banner_common = ["Minimized", "Maximized", "Close"]
banner_short = ["minus", "f11", "x"]
line3 = ["this.WindowState = FormWindowState.Minimized", "this.WindowState = FormWindowState.Maximized", "Environment.Exit(0)"]
for l in range(len(banner_common)):
    x += 1
    if (l == 1):
        output += """
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
            Parse_Markdown(lines[""" + str(x) + """]);
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
        }"""
        continue
    output += """
        private void """ + banner_common[l] + """_Click(object sender, EventArgs e)
        {
            """ + line3[l] + """;
        }
        private void """ + banner_common[l] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            banner_""" + banner_short[l] + """_ck.BackgroundImage = """ + banner_common[l].lower() + """_hover;
        }
        private void """ + banner_common[l] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            banner_""" + banner_short[l] + """_ck.BackgroundImage = """ + banner_common[l].lower() + """;
        }"""
banner_long = ["Left", "Top_left", "Top", "Top_right", "Right", "Bottom_right", "Bottom", "Bottom_left", "Arrow_1080p", "Screen2_Left", "Screen2_Top_left", "Screen2_Top", "Screen2_Top_right", "Screen2_Right", "Screen2_Bottom_right", "Screen2_Bottom", "Screen2_Bottom_left", "Screen2_Arrow_1080p"]
banner = ["4", "7", "8", "9", "6", "3", "2", "1", "5", "14", "17", "18", "19", "16", "13", "12", "11", "15"]  # it's faster to look at the numpad arrows
location = ["Screen.AllScreens[i].Bounds.Location"] * 3 + [
"new Point(Screen.AllScreens[i].Bounds.X + Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Y)",   # Top Right
"new Point(Screen.AllScreens[i].Bounds.X + Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Y)",   # Right
"new Point(Screen.AllScreens[i].Bounds.X + Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Y + Screen.AllScreens[i].Bounds.Height / 2)",  # Bottom Right
"new Point(Screen.AllScreens[i].Bounds.X, Screen.AllScreens[i].Bounds.Y + Screen.AllScreens[i].Bounds.Height / 2)",  # Bottom
"new Point(Screen.AllScreens[i].Bounds.X, Screen.AllScreens[i].Bounds.Y + Screen.AllScreens[i].Bounds.Height / 2)"] + [  # Bottom Left
"Screen.AllScreens[i].Bounds.Location"] * 4 + [ # Arrow 1080p
"new Point(Screen.AllScreens[i].Bounds.X + Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Y)",   # Top Right
"new Point(Screen.AllScreens[i].Bounds.X + Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Y)",   # Right
"new Point(Screen.AllScreens[i].Bounds.X + Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Y + Screen.AllScreens[i].Bounds.Height / 2)",  # Bottom Right
"new Point(Screen.AllScreens[i].Bounds.X, Screen.AllScreens[i].Bounds.Y + Screen.AllScreens[i].Bounds.Height / 2)",  # Bottom
"new Point(Screen.AllScreens[i].Bounds.X, Screen.AllScreens[i].Bounds.Y + Screen.AllScreens[i].Bounds.Height / 2)",  # Bottom Left
"Screen.AllScreens[i].Bounds.Location"]  # Screen2_Arrow_1080p
da_sign = ['='] * 9 + ['!'] * 9
size = ["new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height)",
"new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height / 2)",
"new Size(Screen.AllScreens[i].Bounds.Width, Screen.AllScreens[i].Bounds.Height / 2)",
"new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height / 2)"] * 2 + [
"new Size(1920, 1080)"] + ["new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height)",
"new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height / 2)",
"new Size(Screen.AllScreens[i].Bounds.Width, Screen.AllScreens[i].Bounds.Height / 2)",
"new Size(Screen.AllScreens[i].Bounds.Width / 2, Screen.AllScreens[i].Bounds.Height / 2)"] * 2 + [
"new Size(1920, 1080)"]


#banner_long = ["Left", "Top_left", "Top", "Top_right", "Right", "Bottom_right", "Bottom", "Bottom_left"]
#banner = ["1", "2", "3", "4", "5", "6", "7", "8", "9"]
for m in range(len(banner)):
    x += 1
    #if banner[m] == "":
    #    continue
    output += """
        private void """ + banner_long[m] + """_Click(object sender, EventArgs e)
        {
            Uncheck_Arrow();
            selected_""" + banner_long[m] + """();
            arrow = """ + banner[m] + """;
        }
        private void """ + banner_long[m] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (arrow == """ + banner[m] + """)
                selected_""" + banner_long[m] + """();
            else
                hover_""" + banner_long[m] + """();
        }
        private void """ + banner_long[m] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (arrow == """ + banner[m] + """)
                checked_""" + banner_long[m] + """();
            else
                unchecked_""" + banner_long[m] + """();
        }
        private void checked_""" + banner_long[m] + """()
        {
            banner_""" + banner[m] + "_ck.BackgroundImage = " + banner_long[m].lower() + """_on;
        }
        private void unchecked_""" + banner_long[m] + """()
        {
            banner_""" + banner[m] + "_ck.BackgroundImage = " + banner_long[m].lower() + """_off;
        }
        private void hover_""" + banner_long[m] + """()
        {
            banner_""" + banner[m] + "_ck.BackgroundImage = " + banner_long[m].lower() + """_hover;
        }
        private void selected_""" + banner_long[m] + """()
        {
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.X """ + da_sign[m] + """= 0)
                {
                    this.Size = """ + size[m] + """;
                    this.Location = """ + location[m] + """;
                }
            }
            banner_""" + banner[m] + "_ck.BackgroundImage = " + banner_long[m].lower() + """_selected;
        }"""
x += 1
output += """
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
                Parse_Markdown(lines[""" + str(x) + """]);
            else
                Parse_Markdown(lines[""" + str(x + 1) + """]);
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
            Parse_Markdown(lines[""" + str(x + 2) + """]);
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
                Parse_Markdown(lines[""" + str(x + 3) + """]);
            else
                Parse_Markdown(lines[""" + str(x + 4) + """]);
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
                Parse_Markdown(lines[""" + str(x + 5) + """]);
            else
                Parse_Markdown(lines[""" + str(x + 6) + """]);
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
        }"""
x += 6
"""for p in range(3):
    x += 1
    w += 1"""
p = 0
textbox = ["input_file_txt",
"input_file2_txt",
"output_name_txt",
"mipmaps_txt",
"cmpr_max_txt",
"cmpr_min_alpha_txt",
"num_colours_txt",
"round3_txt",
"round4_txt",
"round5_txt",
"round6_txt",
"diversity_txt",
"diversity2_txt",
"percentage_txt",
"percentage2_txt",
"custom_r_txt",
"custom_g_txt",
"custom_b_txt",
"custom_a_txt"]
var_type = ["", "", "",
"byte", "byte", "byte", "ushort", "byte", "byte", "byte", "byte",
 "byte", "byte", "double", "double", "double", "double", "double", "double"]
max_value = [0,0,0,
255, 16, 255, 65535, 32, 16, 8, 4,
255, 255, 100.0, 100.0, 255.0, 255.0, 255.0, 255.0]
check_paint = ["\n                Check_Paint();", "", "", "\n                Change_mipmap();"] + [""] * 20
name = ["Picture", "Palette"]
name2 = ["Texture", "bmd or tpl"]
filter = ["*.bmp;*.png;*.jfif;*.jpg;*.jpeg;*.jpg;*.ico;*.gif;*.tif;*.tiff;*.rle;*.dib", "*.plt0;*.bmp"]
filter2 = ["*.bti;*.tex0;*.tpl", "*.bmd;*tpl"]
file_title = ["Select a picture or a texture", "Select a palette, a bmd file, or a tpl file"]
w = 7
scrapped += """
        private void """ + textbox[p][:-4] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
        }
        private void """ + textbox[p][:-4] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();                     
        }
        private void """ + textbox[p][:-4] + """_TextChanged(object sender, EventArgs e)
        {
            """ + textbox[p][:-4] + """ = """ + textbox[p] + """.Text;""" + check_run[w] + """
            Organize_args();
            Preview(true);""" + check_paint[p] + """
        }"""
for o in range(4, len(textbox)):
    x += 1
    output += """
        private void """ + textbox[o][:-4] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
        }
        private void """ + textbox[o][:-4] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();                     
        }
        private void """ + textbox[o][:-4] + """_TextChanged(object sender, EventArgs e)
        {
            Parse_""" + var_type[o] + "_text(" + textbox[o] + ", out " + textbox[o][:-4] + ", " + str(max_value[o]) + """);
            Organize_args();
            Preview(true);""" + check_paint[o] + """
        }"""
palette_enc = ["AI8", "RGB565", "RGB5A3"]
for q in range(len(palette_enc)):
    x += 1
    output += """
        private void palette_""" + palette_enc[q] + """_Click(object sender, EventArgs e)
        {
            unchecked_palette(palette_ck[palette_enc]);
            Hide_encoding((byte)(palette_enc + 3));
            selected_palette(palette_""" + palette_enc[q].lower() + """_ck);
            palette_enc = """ + str(q) + """;
            View_""" + palette_enc[q].lower() + """();
            Organize_args();
            Preview(false);
        }
        private void palette_""" + palette_enc[q] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (palette_enc == """ + str(q) + """)
                selected_palette(palette_""" + palette_enc[q].lower() + """_ck);
            else
                hover_palette(palette_""" + palette_enc[q].lower() + """_ck);
        }
        private void palette_""" + palette_enc[q] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (palette_enc == """ + str(q) + """)
                checked_palette(palette_""" + palette_enc[q].lower() + """_ck);
            else
                unchecked_palette(palette_""" + palette_enc[q].lower() + """_ck);
        }"""
banner_icon = ["discord", "github", "youtube"]
link = ["https://discord.gg/4bpfqDJXnU", "https://github.com/yoshi2999/plt0/releases", "https://www.youtube.com/c/yoshytp"]
for r in range(3):
    x += 1
    output += """
        private void """ + banner_icon[r] + """_Click(object sender, EventArgs e)
        {
            // launch webbrowser
            System.Diagnostics.Process.Start(\"""" + link[r] + """");
        }
        private void """ + banner_icon[r] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            """ + banner_icon[r] + "_ck.BackgroundImage = " + banner_icon[r] + """_hover;
        }
        private void """ + banner_icon[r] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            """ + banner_icon[r] + "_ck.BackgroundImage = " + banner_icon[r] + """;
        }"""
view = ["view_alpha", "view_algorithm", "view_WrapS", "view_WrapT", "view_min", "view_mag", "view_rgba", "view_palette", "view_cmpr", "view_options"]
# ck_name = ["alpha_ck_array", "algorithm_ck", "WrapS_ck", "WrapT_ck", "minification_ck", "magnification_ck"]
for j in range(len(view)):
    hide = [view[j].split('_')[1] + "(true);", view[j].split('_')[1] + "(255, true);"] + [view[j].split('_')[1] + "(true);"] * 9
    x += 1
    output += """
        private void """ + view[j] + """_Click(object sender, EventArgs e)
        {
            if (""" + view[j] + """)
            {
                Hide_""" + hide[j]  + """
                Category_hover(""" + view[j] + """_ck);
            }
            else
            {
                View_""" + hide[j] + """
                Category_selected(""" + view[j] + """_ck);
            }
        }
        private void """ + view[j] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (""" + view[j] + """)
                Category_selected(""" + view[j] + """_ck);
            else
                Category_hover(""" + view[j] + """_ck);
        }
        private void """ + view[j] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (""" + view[j] + """)
                Category_checked(""" + view[j] + """_ck);
            else
                Category_unchecked(""" + view[j] + """_ck);
        }"""
text_icon = ["version", "cli_textbox", "run", "Output_label", "banner_global_move", "banner_move", "banner_resize", "sync_preview", "cmpr_save", "cmpr_save_as", "cmpr_swap", "cmpr_swap2", "cmpr_palette", "cmpr_mouse1", "cmpr_mouse2", "cmpr_mouse3", "cmpr_mouse4", "cmpr_mouse5", "cmpr_sel", "cmpr_c1", "cmpr_c2", "cmpr_c3", "cmpr_c4"]
for s in range(len(text_icon)):
    line2 = [
"\n            " + text_icon[s] + "_ck.BackgroundImage = " + text_icon[s] + "_hover;",
"\n            " + text_icon[s] + "_ck.BackgroundImage = " + text_icon[s] + "_hover;\n            this.cli_textbox_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(4)))), ((int)(((byte)(0)))));",
"\n            " + text_icon[s] + "_ck.BackgroundImage = " + text_icon[s] + "_hover;",
"",
"""
            if (banner_global_move)
                banner_global_move_ck.BackgroundImage = banner_global_move_selected;
            else
                banner_global_move_ck.BackgroundImage = banner_global_move_hover;""",
"",
"",
"\n            if (!preview_changed)\n                " + text_icon[s] + "_ck.BackgroundImage = " + text_icon[s] + "_hover;\n            else\n                " + text_icon[s] + "_ck.BackgroundImage = " + text_icon[s] + "_selected;"] + [
"\n            " + text_icon[s] + "_ck.BackgroundImage = " + text_icon[s] + "_hover;"] * 4 + [""] * 15
    line3 = [
"\n            " + text_icon[s] + "_ck.BackgroundImage = " + text_icon[s] + ";",
"\n            " + text_icon[s] + "_ck.BackgroundImage = " + text_icon[s] + ";\n            this.cli_textbox_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(20)))), ((int)(((byte)(0)))), ((int)(((byte)(49)))));",
"\n            Check_run();",
"",
"""
            if (banner_global_move)
                banner_global_move_ck.BackgroundImage = banner_global_move_on;
            else
                banner_global_move_ck.BackgroundImage = banner_global_move_off;""",
"",
"",
"\n            if (!preview_changed)\n                " + text_icon[s] + "_ck.BackgroundImage = " + text_icon[s] + "_off;\n            else\n                " + text_icon[s] + "_ck.BackgroundImage = " + text_icon[s] + "_on;"] + [
"\n            " + text_icon[s] + "_ck.BackgroundImage = " + text_icon[s] + ";"] * 4 + [""] * 15
    x += 1
    output += """
        private void """ + text_icon[s] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);""" + line2[s] + """
        }
        private void """ + text_icon[s] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();""" + line3[s] + """
        }"""
for u in range(len(text_icon) - 4, len(text_icon)):
    output += """
        private void """ + text_icon[u] + """_Click(object sender, EventArgs e)
        {
            cmpr_selected_colour = """ + str(u - len(text_icon) + 5) + """;
            cmpr_sel.BackColor = System.Drawing.Color.FromArgb(cmpr_colours_argb[""" + str(((u - len(text_icon) + 5) << 2) - 4) + "], cmpr_colours_argb[" + str(((u - len(text_icon) + 5) << 2) - 3) + "], cmpr_colours_argb[" + str(((u - len(text_icon) + 5) << 2) - 2) + "], cmpr_colours_argb[" + str(((u - len(text_icon) + 5) << 2) - 1) + """]);
        }"""
line4 = [''] * 6 + ["\n                this.Size = new Size(this.Size.Width + mouse_x - e.X, this.Size.Height + mouse_y - e.Y);"]
line5 = [''] * 5 + ["\n                this.Size = new Size(this.Size.Width + mouse_x - e.X, this.Size.Height + mouse_y - e.Y);", '']
line6 = [''] * 4 + ["banner_global_move && e.Button == MouseButtons.Left"] + ["e.Button == MouseButtons.Left"] * 2
for t in range(4, 7):
    output += """
        private void """ + text_icon[t] + """_MouseDown(object sender, MouseEventArgs e)
        {
            // e.Button;
            mouse_x = e.X;
            mouse_y = e.Y;
        }
        private void """ + text_icon[t] + """_MouseMove(object sender, MouseEventArgs e)
        {
            if (""" + line6[t] + """)
            {
                this.Location = new Point(this.Location.X + e.X - mouse_x, this.Location.Y + e.Y - mouse_y);""" + line4[t] + """
            }
        }"""
checkbox = ["textchange", "auto_update", "upscale"]
for u in checkbox:
    output += """
        private void """ + u + """_Click(object sender, EventArgs e)
        {
            if (""" + u + """)
            {
                """ + u + """ = false;
                hover_checkbox(""" + u + """_ck);
            }
            else
            {
                """ + u + """ = true;
                selected_checkbox(""" + u + """_ck);
            }
        }
        private void """ + u + """_MouseEnter(object sender, EventArgs e)
        {
            if (""" + u + """)
                selected_checkbox(""" + u + """_ck);
            else
                hover_checkbox(""" + u + """_ck);
        }
        private void """ + u + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (""" + u + """)
                checked_checkbox(""" + u + """_ck);
            else
                unchecked_checkbox(""" + u + """_ck);
        }"""
tooltip = ["cmpr_block_selection", "cmpr_block_paint", "cmpr_block_selection"]
for v in range(2):
    x += 1
    output += """
        private void """ + tooltip[v] + """_Click(object sender, EventArgs e)
        {
            unchecked_tooltip(""" + tooltip[v + 1] + """_ck);
            selected_tooltip(""" + tooltip[v] + """_ck);
            tooltip = """ + str(v) + """; // """ + tooltip[v] + """
        }
        private void """ + tooltip[v] + """_MouseEnter(object sender, EventArgs e)
        {
            Parse_Markdown(lines[""" + str(x) + """]);
            if (tooltip == """ + str(v) + """)
                selected_tooltip(""" + tooltip[v] + """_ck);
            else
                hover_tooltip(""" + tooltip[v] + """_ck);
        }
        private void """ + tooltip[v] + """_MouseLeave(object sender, EventArgs e)
        {
            Hide_description();
            if (tooltip == """ + str(v) + """)
                checked_tooltip(""" + tooltip[v] + """_ck);
            else
                unchecked_tooltip(""" + tooltip[v] + """_ck);
        }"""
bool_value = ["true", "false"]
func_name = ["Down", "Up"]
w = 0
#for w in range(2):
scrapped = """
        private void Block_Mouse""" + func_name[w] + """(object sender, MouseEventArgs e)
        {
            // back button = XButton1
            // forward button = XButton2
            // mouse wheel = Middle
            // Left click = Left
            // Right click = Right
            switch (e.Button.ToString())
            {
                case "XButton1":
                    XButton1_down = """ + bool_value[w] + """;
                    break;
                case "XButton2":
                    XButton2_down = """ + bool_value[w] + """;
                    break;
                case "Middle":
                    Middle_down = """ + bool_value[w] + """;
                    break;
                case "Left":
                    Left_down = """ + bool_value[w] + """;
                    break;
                case "Right":
                    Right_down = """ + bool_value[w] + """;
                    break;
            }
        }"""
x += 1
output += """
        private void Warn_rgb565_colour_trim()
        {
            Parse_Markdown(lines[""" + str(x) + """], cmpr_warning);
        }
        private void Put_that_damn_cmpr_layout_in_place()
        {
            Check_Paint();"""
for y in range(5):
    x += 1
    output += """
            Parse_Markdown(lines[""" + str(x) + "], cmpr_mouse" + str(y + 1) + "_label);"
output += """
            checked_tooltip(cmpr_block_selection_ck);
            unchecked_tooltip(cmpr_block_paint_ck);
        }"""
#block_name = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G"]
#for z in range(16):
#    x += 1
#    output += """
#        private void cmpr_block""" + block_name[z] + """_MouseClick(object sender, MouseEventArgs e)
#        {
#            switch (e.Button.ToString())
#            {
#                case "Left":
#                    Paint_Pixel(""" + str(z) + """, cmpr_selected_colour);
#                    break;
#                case "Middle":
#                    Paint_Pixel(""" + str(z) + """, 1);
#                    break;
#                case "Right":
#                    Paint_Pixel(""" + str(z) + """, 2);
#                    break;
#                case "XButton2":
#                    Paint_Pixel(""" + str(z) + """, 3);
#                    break;
#                case "XButton1":
#                    Paint_Pixel(""" + str(z) + """, 4);
#                    break;
#            }
#        }
#        private void cmpr_block""" + block_name[z] + """_MouseEnter(object sender, EventArgs e)
#        {
#            Parse_Markdown(lines[""" + str(x) + """]);
#        }
#        private void cmpr_block""" + block_name[z] + """_MouseLeave(object sender, EventArgs e)
#        {
#            Hide_description();
#        }"""
output += """
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
                    Parse_Markdown(lines[""" + str(x + 1) + """], cmpr_warning);
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
                Parse_Markdown(lines[""" + str(x + 1) + """], cmpr_warning);
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
                output_label.Text = "Run " + run_count.ToString() + " time\\n" + cli.Check_exit();
            else
                output_label.Text = "Run " + run_count.ToString() + " times\\n" + cli.Check_exit();
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
                Parse_Markdown(lines[""" + str(x + 2) + """], cmpr_warning);
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
                        Parse_Markdown(lines[""" + str(x + 3) + """], cmpr_warning);
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
                Parse_Markdown(lines[""" + str(x + 4) + """], description);
            }
            catch (Exception ex)
            {
                description.Text = ex.Message;
                if (ex.Message.Substring(0, 34) == "The process cannot access the file")  // because it is being used by another process
                {
                    Parse_Markdown(lines[""" + str(x + 5) + """], description);
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

"""
pyperclip.copy(output)
print(output)
print("Generated " + str(output.count('\n')) + " lines of C# code")
input("Press enter to exit...")