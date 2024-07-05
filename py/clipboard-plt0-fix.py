import os
import sys
import json
import time
import pyperclip
from googletrans import Translator

previous = ""
# Read the JSON file
def print_in_red(text):
    print("\033[91m" + text + "\033[0m")

current = pyperclip.paste()
    
translator = Translator()

already_translated = {}
"desc[i].Location = new Point((int)((desc[i].Location.X + 300) * width_ratio), (int)((desc[i].Location.Y - 100) * height_ratio));"
def fix_bug_in_yosh_scharp_code(text):
    out = ""
    for line in text.splitlines():
        done = False
        i = 0
        j = 0
        count = 0
        while not done:
            try:
                if "width_ratio" in line or "height_ratio" in line:
                    line = line.replace("(", "").replace(")", "").replace(" ", "").replace("(", "").replace(",", "").replace(";", "").replace("newPoint", "new Point")
                    part = line.split("int")
                    opp = part[i].split("+")
                    if len(opp) > 1:
                        symbol = "+"
                    else:
                        opp = part[i].split("-")
                        symbol = "-"
                    print(part, opp)
                    if "width_ratio" in line and "new Po" in line:
                        out += f"{part[0]}int((int)({opp[0]} {symbol} ({opp[1]})), "
                        j += 2
                    elif "width_ratio" in line:
                        out += f"(int)({opp[0]} {symbol} ({opp[1]})), "
                        j += 1
                    if "height_ratio" in line:
                        opp = part[j].split("+")
                        if len(opp) > 1:
                            symbol = "+"
                        else:
                            opp = part[j].split("-")
                            symbol = "-"
                        out += f"(int)({opp[0]} {symbol} ({opp[1]})));"
                else:
                    out += line
                out += "\n"
                done = True
            except IndexError:
                if count > 50:
                    out += line + "\n"
                if count % 2 == 0:
                    i += 1
                else:
                    j += 1
                count += 1
    
        
    return out

while 1:
    while current == previous:
        time.sleep(0.5)
        current = pyperclip.paste()
    current = previous = fix_bug_in_yosh_scharp_code(current)
    pyperclip.copy(current)
    time.sleep(1)