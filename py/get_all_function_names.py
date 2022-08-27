import pyperclip
# the goal of this code is to be able to easily change variable or function names in the designer, and generate them in bulk. also adds code to load images declared 
output = """"""
with open("X:\\yoshi\\3D Objects\\C#\\plt0\\plt0\\plt0-v.cs", "r") as cs:
    text = cs.read()
    text = text.splitlines()
for w in range(len(text)):
    if (text[w][:15] == "        private"):
        output += text[w].split("private")[1].split("(")[0].split(" ")[-1] + "\n"
pyperclip.copy(output)
print(output)
print("Generated " + str(output.count('\n')) + " lines of C# code")
input("Press enter to exit...")