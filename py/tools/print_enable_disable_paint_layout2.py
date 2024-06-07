import pyperclip
txt = """                cmpr_mouse1_label.Location = new Point(cmpr_mouse1_label.Location.X - 1920, cmpr_mouse1_label.Location.Y);
                cmpr_mouse2_label.Location = new Point(cmpr_mouse2_label.Location.X - 1920, cmpr_mouse2_label.Location.Y);
                cmpr_mouse3_label.Location = new Point(cmpr_mouse3_label.Location.X - 1920, cmpr_mouse3_label.Location.Y);
                cmpr_mouse4_label.Location = new Point(cmpr_mouse4_label.Location.X - 1920, cmpr_mouse4_label.Location.Y);
                cmpr_mouse5_label.Location = new Point(cmpr_mouse5_label.Location.X - 1920, cmpr_mouse5_label.Location.Y);
                cmpr_c1_txt.Location = new Point(cmpr_c1_txt.Location.X - 1920, cmpr_c1_txt.Location.Y);
                cmpr_c2_txt.Location = new Point(cmpr_c2_txt.Location.X - 1920, cmpr_c2_txt.Location.Y);
                cmpr_c3_txt.Location = new Point(cmpr_c3_txt.Location.X - 1920, cmpr_c3_txt.Location.Y);
                cmpr_c4_txt.Location = new Point(cmpr_c4_txt.Location.X - 1920, cmpr_c4_txt.Location.Y);"""
txt = txt.splitlines()
output = ""
for line in txt:
    output += line.split('.')[0] + '.Visible = true;\n'

for line in txt:
    output += line.split('.')[0] + '.Visible = false;\n'
pyperclip.copy(output)
print(output)
print("Generated " + str(output.count('\n')) + " lines of C# code")
input("Press enter to exit...")