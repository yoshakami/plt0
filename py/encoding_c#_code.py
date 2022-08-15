import pyperclip
output = ""
name = ["i4", "i8", "ai4", "ai8", "rgb565", "rgb5a3", "rgba32", "", "ci4", "ci8", "ci14x2", "", "", "", "cmpr"]
for a in range(len(name)):
    if name[a] == "":
        continue
    output += """        private void """ + name[a].upper() + """_Click(object sender, EventArgs e)
        {
            unchecked_encoding(encoding_ck[encoding]);
            checked_encoding(""" + name[a] + """_ck);
            encoding = """ + str(a) + """; // """ + name[a].upper() + """
        }
        private void """ + name[a].upper() + """_MouseEnter(object sender, EventArgs e)
        {
            if (encoding != """ + str(a) + """)
                hover_encoding(""" + name[a] + """_ck);
        }
        private void """ + name[a].upper() + """_MouseLeave(object sender, EventArgs e)
        {
            if (encoding != """ + str(a) + """)
                unchecked_encoding(""" + name[a] + """_ck);
        }
"""
pyperclip.copy(output)
input(output)