import pyperclip
output = ""
booleans = ["bmd", "bti", "tex0", "tpl", "bmp", "png", "jpg", "jpeg", "gif", "ico", "tif", "tiff", "no_warning", "warn", "funky", "stfu", "safe_mode", "FORCE_ALPHA", "ask_exit", "bmp_32", "reverse", "random"]
for a in booleans:
    output += """        private void """ + a + """_Click(object sender, EventArgs e)
        {
            if (""" + a + """)
            {
                """ + a + """ = false;
                hover_checkbox(""" + a + """_ck);
            }
            else
            {
                """ + a + """ = true;
                selected_checkbox(""" + a + """_ck);
            }
        }
        private void """ + a + """_MouseEnter(object sender, EventArgs e)
        {
            if (""" + a + """)
                selected_checkbox(""" + a + """_ck);
            else
                hover_checkbox(""" + a + """_ck);
        }
        private void """ + a + """_MouseLeave(object sender, EventArgs e)
        {
            if (""" + a + """)
                checked_checkbox(""" + a + """_ck);
            else
                unchecked_checkbox(""" + a + """_ck);
        }
"""
encoding = ["i4", "i8", "ai4", "ai8", "rgb565", "rgb5a3", "rgba32", "", "ci4", "ci8", "ci14x2", "", "", "", "cmpr"]
for a in range(len(encoding)):
    if encoding[a] == "":
        continue
    output += """        private void """ + encoding[a].upper() + """_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            selected_encoding(""" + encoding[a] + """_ck);
            encoding = """ + str(a) + """; // """ + encoding[a].upper() + """
        }
        private void """ + encoding[a].upper() + """_MouseEnter(object sender, EventArgs e)
        {
            if (encoding == """ + str(a) + """)
                selected_encoding(""" + encoding[a] + """_ck);
            else
                hover_encoding(""" + encoding[a] + """_ck);
        }
        private void """ + encoding[a].upper() + """_MouseLeave(object sender, EventArgs e)
        {
            if (encoding == """ + str(a) + """)
                checked_encoding(""" + encoding[a] + """_ck);
            else
                unchecked_encoding(""" + encoding[a] + """_ck);
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
ck_name = ["alpha_ck_array", "algorithm_ck", "WrapS_ck", "WrapT_ck", "minification_ck", "magnification_ck"]
for i in range(len(view)):
    output += """        private void """ + view[i] + """_Click(object sender, EventArgs e)
        {
            if (""" + view[i] + """)
            {
                for (byte i = 0; i < """ + ck_name[i] + """.Count; i++)
                {
                    """ + ck_name[i] + """[i].Visible = false;
                }
                """ + view[i] + """ = false;
                Category_hover(""" + view[i] + """_ck);
            }
            else
            {
                for (byte i = 0; i < """ + ck_name[i] + """.Count; i++)
                {
                    """ + ck_name[i] + """[i].Visible = true;
                }
                """ + view[i] + """ = true;
                Category_selected(""" + view[i] + """_ck);
            }
        }
        private void """ + view[i] + """_MouseEnter(object sender, EventArgs e)
        {
            if (""" + view[i] + """)
                Category_selected(""" + view[i] + """_ck);
            else
                Category_hover(""" + view[i] + """_ck);
        }
        private void """ + view[i] + """_MouseLeave(object sender, EventArgs e)
        {
            if (""" + view[i] + """)
                Category_checked(""" + view[i] + """_ck);
            else
                Category_unchecked(""" + view[i] + """_ck);
        }
"""
pyperclip.copy(output)
input(output)