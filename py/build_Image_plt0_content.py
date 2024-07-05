import pyperclip
# the goal of this code is to be able to easily change variable or function names in the designer, and generate them in bulk. also adds code to load images declared 
scrapped = "" # for scrapped functions
output = """        private void Load_Images()
        {"""
x = 0  # settings start line minus two 
with open("C:\\C#\\plt0\\plt0\\plt0-v.cs", "r") as cs:
    text = cs.read()
    text = text.splitlines()
w = 0
for a in ["", "arrows/", "banner/", "circles/", "graphics/", "rgba/"]:
    while (text[w][:14] != "        Image "):
        w += 1
    while (text[w][:14] == "        Image "):
        output += """
                if (File.Exists(execPath + "plt0 content/""" + a  + text[w][14:-1] + """.png"))
                {
                    """ + text[w][14:-1] + """ = Image.FromFile(execPath + "plt0 content/""" + a + text[w][14:-1] + """.png");
                }"""
        w += 1

output += """
        }"""
w = -1
pyperclip.copy(output)
print(output)