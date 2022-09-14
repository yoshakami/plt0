
checkbox = ["banner_global_move", "textchange", "auto_update", "upscale"]
import pyperclip
output = ""
booleans = ["bmd", "bti", "tex0", "tpl", "bmp", "png", "jpg", "jpeg", "gif", "ico", "tif", "tiff", "ask_exit", "bmp_32", "FORCE_ALPHA", "funky", "name_string", "random", "reversex", "reversey", "safe_mode", "stfu", "warn"]
big_array = checkbox + ["cmpr_hover", "cmpr_update_preview"] + booleans
w = 70
for a in big_array:
    w += 2
    output += """
            if (config[""" + str(w) + """].ToLower() == "true")
            {
                checked_checkbox(""" + a + """_ck);
                """ + a + """ = true;
            }
            else
            {
                unchecked_checkbox(""" + a + """_ck);
            }"""
pyperclip.copy(output)