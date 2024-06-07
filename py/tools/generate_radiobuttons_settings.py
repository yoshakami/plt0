encoding = ["i4", "i8", "ai4", "ai8", "rgb565", "rgb5a3", "rgba32", "", "ci4", "ci8", "ci14x2", "", "", "", "cmpr"]
algorithm = ["Cie_601", "Cie_709", "Custom", "Darkest_Lightest", "No_gradient", "Weemm", "SooperBMD", "Min_Max"]
alpha = ["No_alpha", "Alpha", "Mix"]
wrap = ["Clamp", "Repeat", "Mirror"]
filter = ["Nearest_Neighbour", "Linear", "NearestMipmapNearest", "NearestMipmapLinear", "LinearMipmapNearest", "LinearMipmapLinear"]
palette_enc = ["AI8", "RGB565", "RGB5A3"]
import pyperclip

w = 20
output = """switch (config[""" + str(w) + """].ToLower())
{"""
for a in range(len(encoding)):
    output += """
    case \"""" + encoding[a].lower() + """":
        checked_encoding(""" + encoding[a].lower() + """_ck);
        encoding = """ + str(a) + """;
        break;"""
w += 2
output += """}
switch (config[""" + str(w) + """].ToLower())
{"""
for a in range(len(algorithm)):
    output += """
    case \"""" + algorithm[a].lower() + """":
        checked_algorithm(""" + algorithm[a].lower() + """_ck);
        algorithm = """ + str(a) + """;
        break;"""
w += 2
output += """}
switch (config[""" + str(w) + """].ToLower())
{"""
for a in range(len(alpha)):
    output += """
    case \"""" + alpha[a].lower() + """":
        checked_alpha(""" + alpha[a].lower() + """_ck);
        alpha = """ + str(a) + """;
        break;"""
w += 2
output += """}
switch (config[""" + str(w) + """].ToLower())
{"""
for a in range(len(palette_enc)):
    output += """
    case \"""" + palette_enc[a].lower() + """":
        checked_palette(""" + palette_enc[a].lower() + """_ck);
        palette_enc = """ + str(a) + """;
        break;"""
w += 2
output += """}
switch (config[""" + str(w) + """].ToLower())
{"""
for a in range(len(filter)):
    output += """
    case \"""" + filter[a].lower() + """":
        checked_Minification(""" + filter[a].lower() + """_ck);
        minification_filter = """ + str(a) + """;
        break;"""
w += 2
output += """}
switch (config[""" + str(w) + """].ToLower())
{"""
for a in range(len(filter)):
    output += """
    case \"""" + filter[a].lower() + """":
        checked_Magnification(""" + filter[a].lower() + """_ck);
        magnification_filter = """ + str(a) + """;
        break;"""
w += 2
output += """}
switch (config[""" + str(w) + """].ToLower())
{"""
for a in range(len(wrap)):
    output += """
    case \"""" + wrap[a].lower() + """":
        checked_WrapS(S""" + wrap[a].lower() + """_ck);
        WrapS = """ + str(a) + """;
        break;"""
w += 2
output += """}
switch (config[""" + str(w) + """].ToLower())
{"""
for a in range(len(wrap)):
    output += """
    case \"""" + wrap[a].lower() + """":
        checked_WrapT(T""" + wrap[a].lower() + """_ck);
        WrapT = """ + str(a) + """;
        break;"""
w += 2
output += """}"""

pyperclip.copy(output)
