import pyperclip
output = ""
algorithm = ["Cie_601", "Cie_709", "Custom", "No_gradient"]
for a in range(len(algorithm)):
    output += """        private void """ + algorithm[a] + """_Click(object sender, EventArgs e)
        {
            unchecked_algorithm(algorithm_ck[algorithm]);
            checked_algorithm(""" + algorithm[a].lower() + """_ck);
            algorithm = """ + str(a) + """; // """ + algorithm[a] + """
        }
        private void """ + algorithm[a] + """_MouseEnter(object sender, EventArgs e)
        {
            if (algorithm != """ + str(a) + """)
                hover_algorithm(""" + algorithm[a].lower() + """_ck);
        }
        private void """ + algorithm[a] + """_MouseLeave(object sender, EventArgs e)
        {
            if (algorithm != """ + str(a) + """)
                unchecked_algorithm(""" + algorithm[a].lower() + """_ck);
        }
"""
alpha = ["No_alpha", "Alpha", "Mix"]
for b in range(len(alpha)):
    output += """        private void """ + alpha[b] + """_Click(object sender, EventArgs e)
        {
            unchecked_alpha(alpha_ck_array[alpha]);
            checked_alpha(""" + alpha[b].lower() + """_ck);
            alpha = """ + str(b) + """; // """ + alpha[b] + """
        }
        private void """ + alpha[b] + """_MouseEnter(object sender, EventArgs e)
        {
            if (alpha != """ + str(b) + """)
                hover_alpha(""" + alpha[b].lower() + """_ck);
        }
        private void """ + alpha[b] + """_MouseLeave(object sender, EventArgs e)
        {
            if (alpha != """ + str(b) + """)
                unchecked_alpha(""" + alpha[b].lower() + """_ck);
        }
"""
wrap = ["Clamp", "Repeat", "Mirror"]
for c in range(3):
    output += """        private void WrapS_""" + wrap[c] + """_Click(object sender, EventArgs e)
        {
            unchecked_WrapS(WrapS_ck[WrapS]);
            checked_WrapS(S""" + wrap[c].lower() + """_ck);
            WrapS = """ + str(c) + """; // """ + wrap[c] + """
        }
        private void WrapS_""" + wrap[c] + """_MouseEnter(object sender, EventArgs e)
        {
            if (WrapS != """ + str(c) + """)
                hover_WrapS(S""" + wrap[c].lower() + """_ck);
        }
        private void WrapS_""" + wrap[c] + """_MouseLeave(object sender, EventArgs e)
        {
            if (WrapS != """ + str(c) + """)
                unchecked_WrapS(S""" + wrap[c].lower() + """_ck);
        }
"""
for d in range(3):
    output += """        private void WrapT_""" + wrap[d] + """_Click(object sender, EventArgs e)
        {
            unchecked_WrapT(WrapT_ck[WrapT]);
            checked_WrapT(T""" + wrap[d].lower() + """_ck);
            WrapT = """ + str(d) + """; // """ + wrap[d] + """
        }
        private void WrapT_""" + wrap[d] + """_MouseEnter(object sender, EventArgs e)
        {
            if (WrapT != """ + str(d) + """)
                hover_WrapT(T""" + wrap[d].lower() + """_ck);
        }
        private void WrapT_""" + wrap[d] + """_MouseLeave(object sender, EventArgs e)
        {
            if (WrapT != """ + str(d) + """)
                unchecked_WrapT(T""" + wrap[d].lower() + """_ck);
        }
"""
filter = ["Nearest_Neighbour", "Linear", "NearestMipmapNearest", "NearestMipmapLinear", "LinearMipmapNearest", "LinearMipmapLinear"]
for e in range(6):
    output += """        private void Minification_""" + filter[e] + """_Click(object sender, EventArgs e)
        {
            unchecked_Minification(minification_ck[minification_filter]);
            checked_Minification(min_""" + filter[e].lower() + """_ck);
            minification_filter = """ + str(e) + """; // """ + filter[e] + """
        }
        private void Minification_""" + filter[e] + """_MouseEnter(object sender, EventArgs e)
        {
            if (minification_filter != """ + str(e) + """)
                hover_Minification(min_""" + filter[e].lower() + """_ck);
        }
        private void Minification_""" + filter[e] + """_MouseLeave(object sender, EventArgs e)
        {
            if (minification_filter != """ + str(e) + """)
                unchecked_Minification(min_""" + filter[e].lower() + """_ck);
        }
"""
for f in range(6):
    output += """        private void Magnification_""" + filter[f] + """_Click(object sender, EventArgs e)
        {
            unchecked_Magnification(magnification_ck[magnification_filter]);
            checked_Magnification(mag_""" + filter[f].lower() + """_ck);
            magnification_filter = """ + str(f) + """; // Mag_""" + filter[f] + """
        }
        private void Magnification_""" + filter[f] + """_MouseEnter(object sender, EventArgs e)
        {
            if (magnification_filter != """ + str(f) + """)
                hover_Magnification(mag_""" + filter[f].lower() + """_ck);
        }
        private void Magnification_""" + filter[f] + """_MouseLeave(object sender, EventArgs e)
        {
            if (magnification_filter != """ + str(f) + """)
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
            unchecked_""" + g + "(" + g.lower() + "_ck[" + g.lower() + """]);
            checked_""" + g + '_' + channel[h] + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
            """ + g.lower() + " = " + str(h) + "; // " + channel2[i] + " channel set to " + channel[h] + """
        }
        private void """ + g + '_' + channel[h] + """_MouseEnter(object sender, EventArgs e)
        {
            if (""" + g.lower() + " != " + str(h) + """)
                hover_""" + g + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
        }
        private void """ + g + '_' + channel[h] + """_MouseLeave(object sender, EventArgs e)
        {
            if (""" + g.lower() + " != " + str(h) + """)
                unchecked_""" + g + "(" + g.lower() + '_' + channel[h].lower() + """_ck);
        }
"""
pyperclip.copy(output)
input(output)