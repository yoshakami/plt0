import pyperclip
output = """"""
booleans = ["ask_exit", "bmp_32", "FORCE_ALPHA", "funky", "no_warning", "random", "reverse", "safe_mode", "stfu", "warn"]
typess = ["_ck", "_label", "_hitbox"]
sign = ["-", "+"]
direction = ["left", "right"]
for z in range(len(direction)):
    output += """
        private void Move_options_""" + direction[z] + """()
        {"""
    for a in booleans:
        for b in typess:
            output += """
            """ + a + b + ".Location = new Point(" + a + b + ".Location.X " + sign[z] + " 888, " + a + b + ".Location.Y);"
    output += """
        }"""
pyperclip.copy(output)