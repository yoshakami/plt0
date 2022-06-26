import os, pyperclip
output = 'Console.WriteLine('
with open("parameters.txt", 'r') as txt:
    a = txt.read()
lines = a.splitlines()
for line in lines:
    param = line.split(' ')[1].split(';')[0]
    output += f'"\n{param}=" + {param} + '
output = output[:-2] + ');'
pyperclip.copy(output)
input(output)