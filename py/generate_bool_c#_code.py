import pyperclip
output = ""
name = ["bmd", "bti", "tex0", "tpl", "bmp", "png", "jpg", "jpeg", "gif", "ico", "tif", "tiff", "no_warning", "warn", "funky", "stfu", "safe_mode", "FORCE_ALPHA", "ask_exit", "bmp_32", "reverse", "random"]
for a in name:
    output += """        private void """ + a + """_Click(object sender, EventArgs e)
        {
            if (""" + a + """)
            {
                """ + a + """ = false;
                unchecked_checkbox(""" + a + """_ck);
            }
            else
            {
                """ + a + """ = true;
                checked_checkbox(""" + a + """_ck);
            }
        }

        private void """ + a + """_MouseEnter(object sender, EventArgs e)
        {
            if (!""" + a + """)
                hover_checkbox(""" + a + """_ck);
        }

        private void """ + a + """_MouseLeave(object sender, EventArgs e)
        {
            if (!""" + a + """)
            {
                unchecked_checkbox(""" + a + """_ck);
            }
        }"""
pyperclip.copy(output)
input(output)