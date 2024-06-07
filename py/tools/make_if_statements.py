import pyperclip
output = ""
booleans = ["bmd", "bti", "tex0", "tpl", "bmp", "png", "jpg", "jpeg", "gif", "ico", "tif", "tiff", "ask_exit", "bmp_32", "FORCE_ALPHA", "funky", "no_warning", "random", "reverse", "safe_mode", "stfu", "warn"]
for a in booleans:
    output += """
            if (""" + a + """)
            {
                args += \"""" + a + """ ";
                arg_array.Add(\"""" + a + """");
            }"""
pyperclip.copy(output)