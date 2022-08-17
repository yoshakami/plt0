import pyperclip
output = ""
booleans = ["bmd", "bti", "tex0", "tpl", "bmp", "png", "jpg", "jpeg", "gif", "ico", "tif", "tiff", "no_warning", "warn", "funky", "stfu", "safe_mode", "FORCE_ALPHA", "ask_exit", "bmp_32", "reverse", "random"]
for y in booleans:
    output += """        private void """ + y + """_Click(object sender, EventArgs e)
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
            if (""" + y + """)
                selected_checkbox(""" + y + """_ck);
            else
                hover_checkbox(""" + y + """_ck);
        }
        private void """ + y + """_MouseLeave(object sender, EventArgs e)
        {
            if (""" + y + """)
                checked_checkbox(""" + y + """_ck);
            else
                unchecked_checkbox(""" + y + """_ck);
        }
"""
encoding = ["i4", "i8", "ai4", "ai8", "rgb565", "rgb5a3", "rgba32", "", "ci4", "ci8", "ci14x2", "", "", "", "cmpr"]
for z in range(len(encoding)):
    if encoding[z] == "":
        continue
    output += """        private void """ + encoding[z].upper() + """_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(""" + encoding[z] + """_ck);
            encoding = """ + str(z) + """; // """ + encoding[z].upper() + """
        }
        private void """ + encoding[z].upper() + """_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == """ + str(z) + """)
                selected_encoding(""" + encoding[z] + """_ck);
            else
                hover_encoding(""" + encoding[z] + """_ck);
        }
        private void """ + encoding[z].upper() + """_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == """ + str(z) + """)
                checked_encoding(""" + encoding[z] + """_ck);
            else
                unchecked_encoding(""" + encoding[z] + """_ck);
        }
"""
algorithm = ["Cie_601", "Cie_709", "Custom", "No_gradient"]
for a in range(len(algorithm)):
    output += """        private void """ + algorithm[a] + """_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            selected_algorithm(""" + algorithm[a].lower() + """_ck);
            algorithm = """ + str(a) + """; // """ + algorithm[a] + """
        }
        private void """ + algorithm[a] + """_MouseEnter(object sender, EventArgs e)
        {
            if (algorithm == """ + str(a) + """)
                selected_algorithm(""" + algorithm[a].lower() + """_ck);
            else
                hover_algorithm(""" + algorithm[a].lower() + """_ck);
        }
        private void """ + algorithm[a] + """_MouseLeave(object sender, EventArgs e)
        {
            if (algorithm == """ + str(a) + """)
                checked_algorithm(""" + algorithm[a].lower() + """_ck);
            else
                unchecked_algorithm(""" + algorithm[a].lower() + """_ck);
        }
"""
alpha = ["No_alpha", "Alpha", "Mix"]
for b in range(len(alpha)):
    output += """        private void """ + alpha[b] + """_Click(object sender, EventArgs e)
        {
            unchecked_alpha(alpha_ck_array[alpha]);
            selected_alpha(""" + alpha[b].lower() + """_ck);
            alpha = """ + str(b) + """; // """ + alpha[b] + """
        }
        private void """ + alpha[b] + """_MouseEnter(object sender, EventArgs e)
        {
            if (alpha == """ + str(b) + """)
                selected_alpha(""" + alpha[b].lower() + """_ck);
            else
                hover_alpha(""" + alpha[b].lower() + """_ck);
        }
        private void """ + alpha[b] + """_MouseLeave(object sender, EventArgs e)
        {
            if (alpha == """ + str(b) + """)
                checked_alpha(""" + alpha[b].lower() + """_ck);
            else
                unchecked_alpha(""" + alpha[b].lower() + """_ck);
        }
"""
wrap = ["Clamp", "Repeat", "Mirror"]
for c in range(3):
    output += """        private void WrapS_""" + wrap[c] + """_Click(object sender, EventArgs e)
        {
            unchecked_WrapS(WrapS_ck[WrapS]);
            selected_WrapS(S""" + wrap[c].lower() + """_ck);
            WrapS = """ + str(c) + """; // """ + wrap[c] + """
        }
        private void WrapS_""" + wrap[c] + """_MouseEnter(object sender, EventArgs e)
        {
            if (WrapS == """ + str(c) + """)
                selected_WrapS(S""" + wrap[c].lower() + """_ck);
            else
                hover_WrapS(S""" + wrap[c].lower() + """_ck);
        }
        private void WrapS_""" + wrap[c] + """_MouseLeave(object sender, EventArgs e)
        {
            if (WrapS == """ + str(c) + """)
                checked_WrapS(S""" + wrap[c].lower() + """_ck);
            else
                unchecked_WrapS(S""" + wrap[c].lower() + """_ck);
        }
"""
for d in range(3):
    output += """        private void WrapT_""" + wrap[d] + """_Click(object sender, EventArgs e)
        {
            unchecked_WrapT(WrapT_ck[WrapT]);
            selected_WrapT(T""" + wrap[d].lower() + """_ck);
            WrapT = """ + str(d) + """; // """ + wrap[d] + """
        }
        private void WrapT_""" + wrap[d] + """_MouseEnter(object sender, EventArgs e)
        {
            if (WrapT == """ + str(d) + """)
                selected_WrapT(T""" + wrap[d].lower() + """_ck);
            else
                hover_WrapT(T""" + wrap[d].lower() + """_ck);
        }
        private void WrapT_""" + wrap[d] + """_MouseLeave(object sender, EventArgs e)
        {
            if (WrapT == """ + str(d) + """)
                checked_WrapT(T""" + wrap[d].lower() + """_ck);
            else
                unchecked_WrapT(T""" + wrap[d].lower() + """_ck);
        }
"""
filter = ["Nearest_Neighbour", "Linear", "NearestMipmapNearest", "NearestMipmapLinear", "LinearMipmapNearest", "LinearMipmapLinear"]
for e in range(6):
    output += """        private void Minification_""" + filter[e] + """_Click(object sender, EventArgs e)
        {
            unchecked_Minification(minification_ck[minification_filter]);
            selected_Minification(min_""" + filter[e].lower() + """_ck);
            minification_filter = """ + str(e) + """; // """ + filter[e] + """
        }
        private void Minification_""" + filter[e] + """_MouseEnter(object sender, EventArgs e)
        {
            if (minification_filter == """ + str(e) + """)
                selected_Minification(min_""" + filter[e].lower() + """_ck);
            else
                hover_Minification(min_""" + filter[e].lower() + """_ck);
        }
        private void Minification_""" + filter[e] + """_MouseLeave(object sender, EventArgs e)
        {
            if (minification_filter == """ + str(e) + """)
                checked_Minification(min_""" + filter[e].lower() + """_ck);
            else
                unchecked_Minification(min_""" + filter[e].lower() + """_ck);
        }
"""
for f in range(6):
    output += """        private void Magnification_""" + filter[f] + """_Click(object sender, EventArgs e)
        {
            unchecked_Magnification(magnification_ck[magnification_filter]);
            selected_Magnification(mag_""" + filter[f].lower() + """_ck);
            magnification_filter = """ + str(f) + """; // Mag_""" + filter[f] + """
        }
        private void Magnification_""" + filter[f] + """_MouseEnter(object sender, EventArgs e)
        {
            if (magnification_filter == """ + str(f) + """)
                selected_Magnification(mag_""" + filter[f].lower() + """_ck);
            else
                hover_Magnification(mag_""" + filter[f].lower() + """_ck);
        }
        private void Magnification_""" + filter[f] + """_MouseLeave(object sender, EventArgs e)
        {
            if (magnification_filter == """ + str(f) + """)
                checked_Magnification(mag_""" + filter[f].lower() + """_ck);
            else
                unchecked_Magnification(mag_""" + filter[f].lower() + """_ck);
        }
"""
channel = ["R", "G", "B", "A"]
channel2 = ["Red", "Green", "Blue", "Alpha"]
i = -1
for g in channel:  # this looks unreadable because it's packed up instead of pasting 4 times the h loop
    i += 1
    for h in range(4):
        output += """        private void """ + g + '_' + channel[h] + """_Click(object sender, EventArgs e)
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
            if (""" + g.lower() + " == " + str(h) + """)
                selected_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
            else
                hover_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
        }
        private void """ + g + '_' + channel[h] + """_MouseLeave(object sender, EventArgs e)
        {
            if (""" + g.lower() + " == " + str(h) + """)
                checked_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
            else
                unchecked_""" + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
        }
"""
view = ["view_alpha", "view_algorithm", "view_WrapS", "view_WrapT", "view_min", "view_mag"]
# ck_name = ["alpha_ck_array", "algorithm_ck", "WrapS_ck", "WrapT_ck", "minification_ck", "magnification_ck"]
for j in range(len(view)):
    output += """        private void """ + view[j] + """_Click(object sender, EventArgs e)
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
            if (""" + view[j] + """)
                Category_selected(""" + view[j] + """_ck);
            else
                Category_hover(""" + view[j] + """_ck);
        }
        private void """ + view[j] + """_MouseLeave(object sender, EventArgs e)
        {
            if (""" + view[j] + """)
                Category_checked(""" + view[j] + """_ck);
            else
                Category_unchecked(""" + view[j] + """_ck);
        }
"""
lyt = ["all", "auto", "preview", "paint"]
Lyt = ["All", "Auto", "Preview", "Paint"]
for k in range(len(lyt)):
    output += """        private void """ + Lyt[k] + """_Click(object sender, EventArgs e)
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
            if (layout == """ + str(k) + """)
                selected_""" + Lyt[k] + """();
            else
                hover_""" + Lyt[k] + """();
        }
        private void """ + Lyt[k] + """_MouseLeave(object sender, EventArgs e)
        {
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
        }
"""
banner_common = ["Minimized", "Maximized", "Close"]
banner_short = ["minus", "5", "x"]
for l in range(len(banner_common)):
    output += """        private void """ + banner_common[l] + """_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.""" + banner_common[l] + """;
        }
        private void """ + banner_common[l] + """_MouseEnter(object sender, EventArgs e)
        {
            banner_""" + banner_short[l] + """_ck.BackgroundImage = """ + banner_common[l].lower() + """_hover;
        }
        private void """ + banner_common[l] + """_MouseLeave(object sender, EventArgs e)
        {
            banner_""" + banner_short[l] + """_ck.BackgroundImage = """ + banner_common[l].lower() + """;
        }
"""
banner_long = ["Left", "Top_left", "Top", "Top_right", "Right", "Bottom_right", "Bottom", "Bottom_left"]
banner = ["4", "7", "8", "9", "6", "3", "2", "1"]  # it's faster to look at the numpad arrows
#banner_long = ["Left", "Top_left", "Top", "Top_right", "Right", "Bottom_right", "Bottom", "Bottom_left"]
#banner = ["1", "2", "3", "4", "5", "6", "7", "8", "9"]
for m in range(len(banner)):
    #if banner[m] == "":
    #    continue
    output += """        private void """ + banner_long[m] + """_Click(object sender, EventArgs e)
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
            if (arrow == """ + banner[m] + """)
                selected_""" + banner_long[m] + """();
            else
                hover_""" + banner_long[m] + """();
        }
        private void """ + banner_long[m] + """_MouseLeave(object sender, EventArgs e)
        {
            if (arrow == """ + banner[m] + """)
                checked_""" + banner_long[m] + """();
            else
                unchecked_""" + banner_long[m] + """();
        }
        private void checked_""" + banner_long[m] + """()
        {
            banner_""" + banner[m] + "_ck.BackgroundImage = " + banner_long[m] + """_on;
        }
        private void unchecked_""" + banner_long[m] + """()
        {
            banner_""" + banner[m] + "_ck.BackgroundImage = " + banner_long[m] + """_off;
        }
        private void hover_""" + banner_long[m] + """()
        {
            banner_""" + banner[m] + "_ck.BackgroundImage = " + banner_long[m] + """_hover;
        }
        private void selected_""" + banner_long[m] + """()
        {
            banner_""" + banner[m] + "_ck.BackgroundImage = " + banner_long[m] + """_selected;
        }
"""
pyperclip.copy(output)
input(output)