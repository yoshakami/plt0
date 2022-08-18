# plt0
Wii/GC texture encoder with palette support (png, jpeg, gif, bmp, tiff, tpl, tex0, plt0, bti)

v0.1: support for all palette formats

v0.2: support input palette and mipmaps

v0.3: inject into bmd files

v0.4: all image formats except cmpr

v0.5: CMPR (finding a decent DXT1 Compression algorithm was hard)

v0.6: Optimize the project by splitting it in more than 1 file

v0.7: decode images

v0.8: TPL files options (add image to existing file)

v0.9: decode TPL files

v1.0: GUI

v1.1 : make a "How to use" image, and test everything (every option, every combination, find anything that breaks the program)

v1.2 : add french zettings.txt

currently developping v1.0

## why this project
wimgt doesn't support textures with palette (or corrupts them), except tpl files. which makes his whole tool a bit annoying

I tried to update it, it's a nightmare :')

## how did you make this
first, I made it on python, but noticed python is kinda slow, so I rewritten it whole in C#. but Dictionnaries with multiple values can't be sorted like in python.

I ended up making lists of ushort arrays and an IComparer.

0. I Installed "Visual Studio Community 2022", then choosen "Windows Application", then changed the code itself to make it a CLI (for now).
1. Parse arguments: I used a big switch case (not case sensitive) and some smart thinking about input/output files/palette
2. I used the native System.Drawing.Imaging.ImageFormat library to convert the input image to 32bit depth bmp, then I processed the raw stream data.
3. Convert all pixels to palette format
4. Build the colour table. (I created my own colour quantization algorithm)
5. Create colour index for all pixels, using my own algorithm
6. Build Output file (write header, then Convert all index rows to "Block" for Wii/GC, or reverse row order for bmp then convert it to other format if user asked so)
7. Test limit cases using files with annoying dimensions, unknown formats, or a combination of parameters that breaks step one

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
