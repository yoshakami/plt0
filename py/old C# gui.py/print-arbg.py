import pyperclip
rgba = 'RGBA'
output = ""
for i in range(4):
    for j in range(4):
        for k in range(4):
            for l in range(4):
                print(f'{rgba[i]}{rgba[j]}{rgba[k]}{rgba[l]}')
                output += f'case "{rgba[i]}{rgba[j]}{rgba[k]}{rgba[l]}":\n' + '{\nr = ' + f'{i};\ng = {j};\nb = {k};\na = {l};\nbreak;\n' + '}\n'

for i in range(3):
    for j in range(3):
        for k in range(3):
            print(f'{rgba[i]}{rgba[j]}{rgba[k]}')
            output += f'case "{rgba[i]}{rgba[j]}{rgba[k]}":\n' + '{\nr = ' + f'{i};\ng = {j};\nb = {k};\nbreak;\n' + '}\n'

pyperclip.copy(output)
input("Done! press enter to exit...")