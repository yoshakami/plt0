# plt0
Wii/GC texture encoder with palette support (png, jpeg, gif, bmp, tiff, tpl, tex0, plt0, bti)

this is a hybrid tool. launch it without specifying an input file or an encoding format and it'll launch the gui.
you can then view the corresponding cli arguments of your command in the gui itself.

![The app in the "All" Layout](../../../yoshakami.github.io/blob/main/plt0-all.png?raw=true)
![The app in the "Paint" Layout](../../../yoshakami.github.io/blob/main/plt0-paint.png?raw=true)

The app has 4 Layouts:
### All: used to display everything even if it has no impact when encoding
### Auto: only displays the options affecting the selected choices when encoding
### Preview: Previews Decoded textures or the encoded one with parameters set in the GUI
### Paint: CMPR Texture Editor
It's a special Paint application implementation where you have first to load a texture (tex0 or bti) that was encoded with CMPR, then Select a 4x4 block on the image, then painting on the 4x4 block on the left with all gaming mouse buttons. This process can be repeated by selecting another block
A colour can be picked on the image by holding Ctrl.

## vocabulary
*CMPR is both DXT1 without alpha and DXT1 with alpha. you can also call it BC1 if it's stored in a dds image. <br>
*cli = command line interface -> (usually cmd.exe or powershell.exe) a black window in Consolas fonts in which you can launch executables with parameters<br>
*gui = graphical user interface -> the visuals <br>


## future updates:
Change "Auto" to "Encode" and create a "Decode" Layout <br>
Add "Range Fit" and maybe other CMPR algorithms <br>
make a "How to use" image, and test everything (every option, every combination, find anything that breaks the program) <br>
add french description.txt <br>
add a "Palette" layout (adds the ability to change the colours of a CI4 palette through the gui) <br>
add an Image Viewer <br>
add WebP Support <br>

## known bugs
Minimum cmpr image size is 16x16 when encoding <br>
Bmd output is untested <br>
No TPL Support for Paint Layout

## version history
v0.1: Support for all palette formats

v0.2: Support input palette and mipmaps

v0.3: Inject into bmd files

v0.4: All image formats except cmpr

v0.5: CMPR (finding a decent DXT1 Compression algorithm was hard)

v0.6: Optimize the project by splitting it in more than 1 file

v0.7: Decode images

v0.8: TPL files options (add image to existing file)

v0.9: Decode TPL files

v1.0: GUI

v1.1: Auto-Scale on Launch and on each F12 press

v2.0: Revamped CMPR Algorithms

## future versions
v2.1: WebP Support, bugfixes, save settings, and enhance gui

v2.2: Decode Layout, and Palette Layout

v2.3: Fullscreen Image Viewer

v2.4: Native French Support

v3.0: Text Editor

## Flow Chart
![Flow Chart for developpers](../../../yoshakami.github.io/blob/main/plt0-chart.png?raw=true)
"Encode" corresponds to the "Encoding Format" file in the "Encode" folder.
Most functions are called in parse_args because I needed them to communicate with the GUI after they finish

## why this project
wimgt doesn't support textures with palette (or corrupts them), except tpl files. which prevents me from modding some wii games.

I tried to update it, it's a nightmare :')

## how did you make this
first, I made it on python, but noticed python is kinda slow*, so I rewritten it whole in C#. but Dictionnaries with multiple values can't be sorted like in python.<br>

I ended up making lists of ushort arrays and an IComparer.

0. I Installed "Visual Studio Community 2022", then choosen "Console Application", (this allows the program to be a hybrid gui/cli).
1. Parse arguments: I used a big switch case (not case sensitive) and some smart thinking about input/output files/palette
2. I used the native System.Drawing.Imaging.ImageFormat library to convert the input image to 32bit depth bmp, then I processed the raw stream data.
3. Convert all pixels to palette format
4. Build the colour table. (I created my own colour quantization algorithm)
5. Create colour index for all pixels, using my own algorithm
6. Build Output file (write header, then Convert all index rows to "Block" for Wii/GC, or reverse row order for bmp then convert it to other format if user asked so)
7. Test limit cases using files with annoying dimensions, unknown formats, or a combination of parameters that breaks step one

## Notes
if you want to change the source code of GUI layout through the [Design] window, you'll need a 4k screen, since Visual Studio automatically resizes the app bounds to your screen size (Naughy VS!) and the whole screen is packed to the gills with GUI elements

the whole GUI was made with only 4 elements : Label, PictureBox, PictureBoxWithInterpolationMode and TextBox.

the GUI opens if (input file = "")  or texture_format isn't specified<br>(which means that opening a texture with plt0.exe will decode it and output a bmp without showing the GUI)

the GUI won't open if the folder `plt0 content` is missing next to the exe, or if a config file is missing. Everything is bundled in the release.

I've spent the most time I have ever consumed for a decent CMPR Algorithm

the python files were either used to generate C# code (functions launched on label hover or on click ), or to analyze C# code, and even to extract data from the code.

* if you're skeptical about python being slow, run <a href="https://github.com/yoshakami/plt0/blob/master/py/plt0.py">plt0.py</a> from the py folder and you'll notice it takes 20 minutes to encode an image to CI8 AI8. (You'll need to change "input_file" at the line 48 first)

## Colour Quantization algorithm
First, create a Colour_table List of int array of 65536 entries with the number itself and the number of times it is used in the image.

(wii/gc colours are stored on 2 bytes) the usage number is set during step 3.

then that list is sorted by the usage number, and the most used values have their index added to the colour palette BUT

if it is too similar to another colour in the palette, it won't be added to the palette. (I decided to name this parameter "diversity" or "d" for short)

then the program will do a second loop with parameters "d2" if it couldn't find anymore colour with the diversity parameter.

there is also another paramter called "percentage" or "p" ("p2" for second loop) that will set the percentage limit of a colour (by its usage number) at which it will trigger second loop.

third loop sets p and d to zero and fills with most used colours not in the colour palette.

it doesn't beat median-cut if the input image has too much colours and output colour is 16. though diversity and percentage allow you to make it better than median-cut :P

I decided to not add dithering at all. it's stupid to destroy a texture with noise. also it adds up an useless colour for CI4 ^^

## colour index algorithm
for (each colour of the colour palette)

  Calculate the difference between the colour and the current pixel for Blue Channel. then add that up with Green Channel difference, and Red Channel, and alpha.
  
at the end of the loop, the colour with the least difference is used as the index.
