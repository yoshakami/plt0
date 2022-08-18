import pyperclip
output = """        private void Load_Images()
        {"""
x = 48
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
booleans = ["bmd", "bti", "tex0", "tpl", "bmp", "png", "jpg", "jpeg", "gif", "ico", "tif", "tiff", "ask_exit", "bmp_32", "FORCE_ALPHA", "funky", "no_warning", "random", "reverse", "safe_mode", "stfu", "warn"]
for y in booleans:
    x += 1
    output += """
        private void """ + y + """_Click(object sender, EventArgs e)
        {
            if (""" + y + """)
            {
                """ + y + """ = false;
                hover_checkbox(""" + y + """_ck);
            }
            else
            {
                """ + y + """ = true;
                selected_checkbox(""" + y + """_ck);
            }
        }
        private void """ + y + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (""" + y + """)
                selected_checkbox(""" + y + """_ck);
            else
                hover_checkbox(""" + y + """_ck);
        }
        private void """ + y + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
            if (""" + y + """)
                checked_checkbox(""" + y + """_ck);
            else
                unchecked_checkbox(""" + y + """_ck);
        }"""
encoding = ["i4", "i8", "ai4", "ai8", "rgb565", "rgb5a3", "rgba32", "", "ci4", "ci8", "ci14x2", "", "", "", "cmpr"]
for z in range(len(encoding)):
    if encoding[z] == "":
        continue
    x += 1
    output += """
        private void """ + encoding[z].upper() + """_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(""" + encoding[z] + """_ck);
            encoding = """ + str(z) + """; // """ + encoding[z].upper() + """
        }
        private void """ + encoding[z].upper() + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (encoding == """ + str(z) + """)
                selected_encoding(""" + encoding[z] + """_ck);
            else
                hover_encoding(""" + encoding[z] + """_ck);
        }
        private void """ + encoding[z].upper() + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
            if (encoding == """ + str(z) + """)
                checked_encoding(""" + encoding[z] + """_ck);
            else
                unchecked_encoding(""" + encoding[z] + """_ck);
        }"""
algorithm = ["Cie_601", "Cie_709", "Custom", "No_gradient"]
for a in range(len(algorithm)):
    x += 1
    output += """
        private void """ + algorithm[a] + """_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            selected_algorithm(""" + algorithm[a].lower() + """_ck);
            algorithm = """ + str(a) + """; // """ + algorithm[a] + """
        }
        private void """ + algorithm[a] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (algorithm == """ + str(a) + """)
                selected_algorithm(""" + algorithm[a].lower() + """_ck);
            else
                hover_algorithm(""" + algorithm[a].lower() + """_ck);
        }
        private void """ + algorithm[a] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
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
        }
        private void """ + alpha[b] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (alpha == """ + str(b) + """)
                selected_alpha(""" + alpha[b].lower() + """_ck);
            else
                hover_alpha(""" + alpha[b].lower() + """_ck);
        }
        private void """ + alpha[b] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
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
        }
        private void WrapS_""" + wrap[c] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (WrapS == """ + str(c) + """)
                selected_WrapS(S""" + wrap[c].lower() + """_ck);
            else
                hover_WrapS(S""" + wrap[c].lower() + """_ck);
        }
        private void WrapS_""" + wrap[c] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
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
        }
        private void WrapT_""" + wrap[d] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (WrapT == """ + str(d) + """)
                selected_WrapT(T""" + wrap[d].lower() + """_ck);
            else
                hover_WrapT(T""" + wrap[d].lower() + """_ck);
        }
        private void WrapT_""" + wrap[d] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
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
        }
        private void Minification_""" + filter[e] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (minification_filter == """ + str(e) + """)
                selected_Minification(min_""" + filter[e].lower() + """_ck);
            else
                hover_Minification(min_""" + filter[e].lower() + """_ck);
        }
        private void Minification_""" + filter[e] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
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
        }
        private void Magnification_""" + filter[f] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (magnification_filter == """ + str(f) + """)
                selected_Magnification(mag_""" + filter[f].lower() + """_ck);
            else
                hover_Magnification(mag_""" + filter[f].lower() + """_ck);
        }
        private void Magnification_""" + filter[f] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
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
        }
        private void """ + g + '_' + channel[h] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (""" + g.lower() + " == " + str(h) + """)
                selected_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
            else
                hover_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
        }
        private void """ + g + '_' + channel[h] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
            if (""" + g.lower() + " == " + str(h) + """)
                checked_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
            else
                unchecked_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
        }"""
view = ["view_alpha", "view_algorithm", "view_WrapS", "view_WrapT", "view_min", "view_mag"]
# ck_name = ["alpha_ck_array", "algorithm_ck", "WrapS_ck", "WrapT_ck", "minification_ck", "magnification_ck"]
for j in range(len(view)):
    x += 1
    output += """
        private void """ + view[j] + """_Click(object sender, EventArgs e)
        {
            if (""" + view[j] + """)
            {
                Hide_""" + view[j].split('_')[1] + """();
                Category_hover(""" + view[j] + """_ck);
            }
            else
            {
                View_""" + view[j].split('_')[1] + """();
                Category_selected(""" + view[j] + """_ck);
            }
        }
        private void """ + view[j] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (""" + view[j] + """)
                Category_selected(""" + view[j] + """_ck);
            else
                Category_hover(""" + view[j] + """_ck);
        }
        private void """ + view[j] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
            if (""" + view[j] + """)
                Category_checked(""" + view[j] + """_ck);
            else
                Category_unchecked(""" + view[j] + """_ck);
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
            layout = """ + str(k) + """;
        }
        private void """ + Lyt[k] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (layout == """ + str(k) + """)
                selected_""" + Lyt[k] + """();
            else
                hover_""" + Lyt[k] + """();
        }
        private void """ + Lyt[k] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
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
banner_common = ["Minimized", "Maximized", "Close"]
banner_short = ["minus", "5", "x"]
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
            description.Text = lines[""" + str(x) + """];
            if (this.WindowState == FormWindowState.Maximized)
                banner_5_ck.BackgroundImage = maximized_selected;
            else
                banner_5_ck.BackgroundImage = maximized_hover;
        }
        private void Maximized_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
            if (this.WindowState == FormWindowState.Maximized)
                banner_5_ck.BackgroundImage = maximized_on;
            else
                banner_5_ck.BackgroundImage = maximized_off;
        }"""
        continue
    output += """
        private void """ + banner_common[l] + """_Click(object sender, EventArgs e)
        {
            """ + line3[l] + """;
        }
        private void """ + banner_common[l] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            banner_""" + banner_short[l] + """_ck.BackgroundImage = """ + banner_common[l].lower() + """_hover;
        }
        private void """ + banner_common[l] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
            banner_""" + banner_short[l] + """_ck.BackgroundImage = """ + banner_common[l].lower() + """;
        }"""
banner_long = ["Left", "Top_left", "Top", "Top_right", "Right", "Bottom_right", "Bottom", "Bottom_left"]
banner = ["4", "7", "8", "9", "6", "3", "2", "1"]  # it's faster to look at the numpad arrows
#banner_long = ["Left", "Top_left", "Top", "Top_right", "Right", "Bottom_right", "Bottom", "Bottom_left"]
#banner = ["1", "2", "3", "4", "5", "6", "7", "8", "9"]
for m in range(len(banner)):
    x += 1
    #if banner[m] == "":
    #    continue
    output += """
        private void """ + banner_long[m] + """_Click(object sender, EventArgs e)
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
            selected_""" + banner_long[m] + """();
            arrow = """ + banner[m] + """;
        }
        private void """ + banner_long[m] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (arrow == """ + banner[m] + """)
                selected_""" + banner_long[m] + """();
            else
                hover_""" + banner_long[m] + """();
        }
        private void """ + banner_long[m] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
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
            banner_""" + banner[m] + "_ck.BackgroundImage = " + banner_long[m].lower() + """_selected;
        }"""
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
"byte", "byte", "byte", "byte", "byte", "byte", "byte", "byte",
 "byte", "byte", "double", "double", "double", "double", "double", "double"]
name = ["Picture", "Palette"]
name2 = ["Texture", "bmd or tpl"]
filter = ["*.bmp;*.png;*.jfif;*.jpg;*.jpeg;*.jpg;*.ico;*.gif;*.tif;*.tiff;*.rle;*.dib", "*.plt0;*.bmp"]
filter2 = ["*.bti;*.tex0;*.tpl", "*.bmd;*tpl"]
file_title = ["Select a picture or a texture", "Select a palette, a bmd file, or a tpl file"]
for n in range(2):
    output += """
        private void """ + textbox[n][:-4] + """_Click(object sender, EventArgs e)
        {
            FileDialog dialog = new OpenFileDialog
            {
                Title = \"""" + file_title[n] + """",
                Filter = \"""" + name[n] + '|' + filter[n] + '|' + name2[n] + '|' + filter2[n] + """|All files (*.*)|*.*",
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                """ + textbox[n] + """.Text = dialog.FileName;
                """ + textbox[n][:-4] + """ = dialog.FileName;
            }
        }"""
for p in range(3):
    x += 1
    output += """
        private void """ + textbox[p][:-4] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
        }
        private void """ + textbox[p][:-4] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";                     
        }
        private void """ + textbox[p][:-4] + """_TextChanged(object sender, EventArgs e)
        {
            """ + textbox[p][:-4] + """ = """ + textbox[p] + """.Text;
        }"""
for o in range(3, len(textbox)):
    x += 1
    output += """
        private void """ + textbox[o][:-4] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
        }
        private void """ + textbox[o][:-4] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";                     
        }
        private void """ + textbox[o][:-4] + """_TextChanged(object sender, EventArgs e)
        {
            len = """ + textbox[o] + """.Text.Length;
            if (""" + textbox[o] + """.Text.Substring(0, 2) == "0x" || ishex""" + var_type[o] + "(" + textbox[o] + """.Text.ToLower()))
            {
                success = """ + var_type[o] + ".TryParse(" + textbox[o] + ".Text.Substring(2, len - 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out " + textbox[o][:-4] + """);
                if (!success)
                    """ + textbox[o] + ".Text = " + textbox[o] + """.Text.Substring(0, len - 1);
            }
            else
            {
                success = """ + var_type[o] + ".TryParse(" + textbox[o] + ".Text, out " + textbox[o][:-4] + """);
                if (!success)
                {
                    """ + textbox[o] + ".Text = " + textbox[o] + """.Text.Substring(0, len - 1);
                }
            }
        }"""
palette_enc = ["AI8", "RGB565", "RGB5A3"]
for q in range(len(palette_enc)):
    x += 1
    output += """
        private void palette_""" + palette_enc[q] + """_Click(object sender, EventArgs e)
        {
            unchecked_palette_enc(palette_ck[palette_enc]);
            palette_enc = """ + str(q) + """;
        }
        private void palette_""" + palette_enc[q] + """_MouseEnter(object sender, EventArgs e)
        {
            description.Text = lines[""" + str(x) + """];
            if (palette_enc == """ + str[q] + """)
                selected_palette(""" + palette_enc[q] + """)();
            else
                hover_palette(""" + palette_enc[q] + """)();
            palette_""" + palette_enc[q] + """_ck.BackgroundImage = """ + palette_enc[l].lower() + """_hover;
        }
        private void palette_""" + palette_enc[q] + """_MouseLeave(object sender, EventArgs e)
        {
            description.Text = "";
            if (palette_enc == """ + str[q] + """)
                checked_""" + palette_enc[q] + """();
            else
                unchecked_""" + palette_enc[q] + """();
        }"""
output += """
    }
}
"""
pyperclip.copy(output)
input(output)