import pyperclip
# the goal of this code is to be able to easily change variable or function names in the designer, and generate them in bulk. also adds code to load images declared 
output = """"""
with open("X:\\yoshi\\3D Objects\\C#\\plt0\\plt0\\code\\Parse_args.cs", "r") as cs:
    text = cs.read()
    text = text.splitlines()
args = []
for w in range(len(text)):
    if (text[w][:22] == "                case \"" and text[w - 1][:22] != "                case \""):
        args.append(text[w].split('"')[1] + '$' + str(w))
args.sort()
for i in range(len(args)):
    w = int(args[i].split('$')[1])
    while text[w] != "                    }": #text[w] != "                        break;" and text[w] != "                    break;":
        if (text[w] == "                    {"):
            w += 1
            continue
        output += text[w] + "\n"
        w += 1
    
pyperclip.copy(output)
print(output)
print("Generated " + str(output.count('\n')) + " lines of C# code")
input("Press enter to exit...")