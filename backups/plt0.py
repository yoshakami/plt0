import os
import sys
import struct
import subprocess
from collections import OrderedDict
from operator import itemgetter

"""PLT0 (File Format) Made by me
```
Offset Size  Description
0x00    4    The magic "PLT0" to identify the sub file. (50 4C 54 30)
0x04    4    Length of the sub file. (internal name/ other string at the end not counted)
0x08    4    Sub file version number. ( 00 00 00 03 else crash)
0x0C    4    Offset to outer BRRES File (negative value) -> (00 00 00 00).
0x10    4    header size. (00 00 00 40)
0x14    4    String offset to the name of this sub file. relative to PLT0 start (that means if it's 0 it will point to 'PLT0'. one byte before the string is its length, so it would read the byte before 'PLT0' if the pointer was 0 )

0x18    4    Pixel Format. (0 = IA8, 1 = RGB565, 2 = RGB5A3)
0x1C    2    Number of colors (can't be set to more than 256 in BrawlCrate 🤔, usually 01 00)
0x22    34    Padding (all 00)

v-- PLT0 Data --v
0x40   -    Data Starts
each colour is 2 bytes in length (A = Alpha, R = Red, G = Green, B = Blue)

for the palette RGB5A3, it's AAAA RRRR   GGGG BBBB  (colour values can only be multiple of 16)
A: 15 R:255 G:3 B:227 is     0000 1111   0000 1101
A: 0  R:255 G:0 B:221 is what Brawlcrate encodes (such a bad encoding to be honest 15 is nearest from 16 than from 0)

for the palette IA8, it's  AAAA AAAA   CCCC CCCC (Alpha and Colour can be any value between 0 and 255)
A: 15 R:127 G:127 B:127 is 0000 0111   0111 1111

for the palette RGB565, it's RRRR RGGG   GGGB BBBB (alpha always 255, and yes there are 6 G lol)
R: 200 G:60 B:69 would be    1100 0001   1110 1000
R: 197 G:61 B:66 is displayed by BrawlCrate and R:192 G:60 B:64 is what's really encoded (by Brawlcrate). it looks like Brawlcrate is faulty lol
```"""
use_pil = False
use_magick = False
input_is_already_bmp = False
user_input_is_correct = True
ImageMagick_path = "magick"
palette_formats = ['IA8', 'AI8', 'RGB565', 'RGB5A3', '']
texture_encoding_formats = ['', 'C4', 'CI4', 'C8', 'CI8', 'CI14X2', 'C14X2']
selected_palette_format = 'AI8'
selected_texture_format = 'CI8'
colour_number = 256
output_file = ''
input_file = r"X:\root\Mario Party 9 [SSQP01]\DATA\files\common\bdresult\option2.brres_DECOMP\tex0\rosalina.png"
if input_file == '':
    print("""no input file specified
    usage: PLT0.py [Encoding Format] [Palette Format] [colour number] [p|m] <file> [dest]
    this is the usage format for parameters : [optional] <mandatory>
    p : use Pillow site-package to convert the input image to bmp
    m : use ImageMagick to convert the input image to bmp
    available palette formats : 'IA8', 'RGB565', 'RGB5A3', '' (auto)
    Available Encoding Formats: '', 'C4', 'CI4', 'C8', 'CI8', 'CI14x2', 'C14x2'
    the number of colours is stored on 2 bytes, but the index bit length of the colour encoding format limits the max number of colours to these:
    CI4 : from 0 to 16
    CI8 : from 0 to 256
    CI14x2 : from 0 to 16384""")

# if selected_palette_format == '':
# #if(check_if_image_has_alpha(input_file)):
#    selected_palette_format = 'RGB5A3'

if output_file == '':
    output_file = os.path.splitext(input_file)[0]
    i = 0
    while os.path.exists(output_file + '.plt0') or os.path.exists(output_file + '.tex0'):
        i += 1
        output_file = f"{os.path.splitext(input_file)[0]}-{i}"

if colour_number > 16 and selected_texture_format in ["CI4", "C4"]:
    user_input_is_correct = False
    print("CI4 can only supports up to 16 colours as each pixel index is stored on 4 bits")

if colour_number > 256 and selected_texture_format in ["CI8", "C8"]:
    user_input_is_correct = False
    print("CI4 can only supports up to 16 colours as each pixel index is stored on 4 bits")

if selected_texture_format == '':
    if colour_number > 256:
        selected_texture_format = "CI14X2"
        print(f'encoding in {selected_texture_format}')
    elif colour_number > 16:
        selected_texture_format = "CI8"
        print(f'encoding in {selected_texture_format}')
    elif colour_number != -1:  # -1 is the default value of this variable I arbitrarily put
        selected_texture_format = "CI4"
        print(f'encoding in {selected_texture_format}')

if not use_pil and not use_magick:
    with open(input_file, 'rb') as check_bmp:
        if check_bmp.read(2) == b'BM':
            input_is_already_bmp = True
    try:
        from PIL import Image, UnidentifiedImageError

        use_pil = True
    except ModuleNotFoundError:
        out = subprocess.run(["magick"], stdout=subprocess.PIPE, stderr=subprocess.STDOUT).stdout
        use_magick = True
        # out = "```\n" + (str(out)[2:-3].replace('\\n', '\n').replace('\\r', '\r').replace('\\t', '\t'))
        # print(out)
    except FileNotFoundError:
        for folder in os.listdir('C:/Program Files'):
            if folder.startswith("ImageMagick"):
                ImageMagick_path = f"C:\\Program Files\\{folder}\\magick.exe"
        if ImageMagick_path == "magick":
            print("no image converter, either install Pillow with 'pip install Pillow' or install ImageMagick on your computer")


def hex_float(number):
    number = number.replace(',', '.')  # replaces coma with dots
    num = b''
    w = hex(struct.unpack('<I', struct.pack('<f', float(number)))[0])[2:]
    # add zeros to always make the value length to 8
    # w = '0' * (8-len(w)) + w
    w = w.zfill(8)
    for octet in range(0, 8, 2):  # transform for example "3f800000" to b'\x3f\x80\x00\x00'
        num += bytes(chr(int(w[octet:(octet + 2)], 16)), 'latin-1')
    return num


def write_TEX0(index_data, output_name, width, height, texture_encoding_int32):
    data = b'TEX0'
    data_length = len(index_data)
    data += bytes(chr(((0x40 + data_length) & 0xff000000) >> 24), 'latin-1') + bytes(chr(((0x40 + data_length) & 0xff0000) >> 16), 'latin-1') + bytes(chr(((0x40 + data_length) & 0xff00) >> 8), 'latin-1') + bytes(chr((0x40 + data_length) & 0xff), 'latin-1')
    data += b'\x00\x00\x00\x03\x00\x00\x00\x00\x00\x00\x00@'
    data += bytes(chr(((0x44 + data_length) & 0xff000000) >> 24), 'latin-1') + bytes(chr(((0x44 + data_length) & 0xff0000) >> 16), 'latin-1') + bytes(chr(((0x44 + data_length) & 0xff00) >> 8), 'latin-1') + bytes(chr((0x44 + data_length) & 0xff), 'latin-1')
    data += b'\x00\x00\x00\x01'  # image has a palette
    data += bytes(chr((width & 0xff00) >> 8), 'latin-1') + bytes(chr(width & 0xff), 'latin-1')  # width stored on 2 bytes
    data += bytes(chr((height & 0xff00) >> 8), 'latin-1') + bytes(chr(height & 0xff), 'latin-1')  # height stored on 2 bytes
    data += texture_encoding_int32
    data += b'\x00\x00\x00\x01'  # no mipmaps yet
    data += b'\x00' * 4  # padding
    # data += hex_float(mipmaps)
    data += b'\x00' * 20  # padding
    data += index_data
    data += b'\x00' * (abs(16 - len(data)) % 16) + b'\x00\x00\x00' + bytes(chr(len(output_name)), 'latin-1') + bytes(output_name, 'latin-1') + b'\x00' * (abs(16 - len(data)) % 16)
    with open(f'{output_name}.tex0', 'wb') as tex0:
        tex0.write(data)


def write_PLT0(colour, colour_palette, output_name, pixel_format):
    data = b'PLT0'
    data += bytes(chr(((0x40 + colour) & 0xff000000) >> 24), 'latin-1') + bytes(chr(((0x40 + colour) & 0xff0000) >> 16), 'latin-1') + bytes(chr(((0x40 + colour) & 0xff00) >> 8), 'latin-1') + bytes(chr((0x40 + colour) & 0xff), 'latin-1')
    data += b'\x00\x00\x00\x03\x00\x00\x00\x00\x00\x00\x00@'
    data += bytes(chr(((0x44 + colour) & 0xff000000) >> 24), 'latin-1') + bytes(chr(((0x44 + colour) & 0xff0000) >> 16), 'latin-1') + bytes(chr(((0x44 + colour) & 0xff00) >> 8), 'latin-1') + bytes(chr((0x44 + colour) & 0xff), 'latin-1')
    data += pixel_format
    data += bytes(chr((colour & 0xff00) >> 8), 'latin-1') + bytes(chr(colour & 0xff), 'latin-1')  # number of colours
    data += b'\x00' * 0x22  # padding
    data += colour_palette
    while len(data) < 0x40 + colour:  # fills in the colour palette if it's not full
        data += b'\x00\x00'
    data += b'\x00' * (abs(16 - len(data)) % 16) + b'\x00\x00\x00' + bytes(chr(len(output_name)), 'latin-1') + bytes(output_name, 'latin-1') + b'\x00' * (abs(16 - len(data)) % 16)
    with open(f'{output_name}.plt0', 'wb') as plt0:
        plt0.write(data)



def create_PLT0_IA8(raw_pixel_array_input, output_name, colour, width, height, texture_encoding_int32):
    # for the palette IA8, it's  AAAA AAAA   CCCC CCCC (Alpha and Colour can be any value between 0 and 255)
    # A: 15 R:127 G:127 B:127 is 0000 0111   0111 1111
    colour_table = b''
    colours_duplicate = b''
    colours_unique = {}
    colour_index = []
    print('sorting colours...')
    for i in range(0, len(raw_pixel_array_input), 4):
        if i % 100000 == 0:
            positionStr = f'\rprocessing pixels... {int((i / len(raw_pixel_array_input)) * 100)}%'
            print(positionStr, end='')
        #    print(f'processed {i//4} pixels out of {width*height}')
        grey = bytes(chr(int(raw_pixel_array_input[i] * 0.299 + raw_pixel_array_input[i + 1] * 0.587 + raw_pixel_array_input[i + 2] * 0.114)), 'latin-1')
        # https://stackoverflow.com/questions/18710560/how-to-convert-colors-to-grayscale-in-java-with-just-the-java-io-library
        alpha = raw_pixel_array_input[i + 3:i + 4]
        if alpha == b'\x00':
            grey = b'\x00'
        colour_data = alpha + grey
        colours_duplicate += colour_data
        if colour_data in colours_unique.keys():
            colours_unique[colour_data] += 1
        else:
            colours_unique[colour_data] = 0
    positionStr = f'\rprocessing pixels... 100%'
    print(positionStr, end='')
    colours_unique = OrderedDict(sorted(colours_unique.items(), key=itemgetter(1), reverse=True))
    i = 0
    for most_used_colour_value in colours_unique.keys():
        colour_table += most_used_colour_value
        i += 1
        if i > colour - 1:
            break
    print('\ncreating indexes...')
    # print(colour_table)
    i = 0
    for n in range(height):
        index_row = []
        for o in range(width):
            diff_min_index = 0
            diff_min = 500
            for j in range(0, len(colour_table), 2):
                difference = abs(colours_duplicate[i] - colour_table[j]) + abs(colours_duplicate[i+1] - colour_table[j+1])
                if difference == 0:
                    diff_min_index = j // 2
                    break
                if difference < diff_min:
                    diff_min = difference
                    diff_min_index = j // 2
            index_row.append(diff_min_index)
            i += 2
        colour_index.append(index_row)
    print(f'colour table has a length of {len(colour_table)}')
    print('writing PLT0...')
    write_PLT0(colour, colour_table, output_name, b'\x00' * 4)
    print('writing TEX0...')
    create_TEX0(colour_index, output_name, width, height, 8, 4, texture_encoding_int32)


def create_PLT0_RGB565(raw_pixel_array_input, output_name, colour, width, height, texture_encoding_int32):
    pass


def create_PLT0_RGB5A3(raw_pixel_bytes_input, output_name, colour, width, height, create_tex0_function):
    pass


def create_PLT0_auto(raw_pixel_bytes_input, output_name, colour, width, height, create_tex0_function):
    pass


def create_TEX0(pixel_index_int_row, output_name, width, height, block_width, block_height, texture_encoding_int32):  # 8x8  4-bit
    index_data = b''
    w_blocks = width / block_width
    h_blocks = height / block_height
    if int(w_blocks) != w_blocks:  # there must be another block if the dimensions don't fit the current number of block / if the number is a decimal value
        w_blocks += 1
    if int(h_blocks) != h_blocks:
        h_blocks += 1
    w_blocks = int(w_blocks)
    h_blocks = int(h_blocks)
    for j in range(h_blocks):  # moves vertically
        for k in range(w_blocks):  # moves horizontally
            for l in range(block_height):
                for m in range(block_width):
                    index_data += bytes(chr(pixel_index_int_row[(block_height * j) + l][(block_width * k) + m]), 'latin-1')
    write_TEX0(index_data, output_name, width, height, texture_encoding_int32)


def create_TEX0_CI8(raw_pixel_bytes_input, output_name, width, height):  # 8x4  8-bit
    pass


def create_TEX0_CI14x2(raw_pixel_bytes_input, output_name, width, height):  # 4x4  16-bit
    pass


def process_bmp(func_to_launch_after_this_function, bmp_input_file, output_file_name, colour, texture_colour_format_int32):  # assuming the bmp has no compression
    with open(bmp_input_file, 'rb') as bmp:
        bmp.seek(2)
        byte = bmp.read(4)
        bmp_filesize = (byte[3] << 24) + (byte[2] << 16) + (byte[1] << 8) + byte[0]  # 4 bytes integer in little-endian
        bmp.seek(10)
        byte = bmp.read(4)
        pixel_data_start_offset = (byte[3] << 24) + (byte[2] << 16) + (byte[1] << 8) + byte[0]  # 4 bytes integer in little-endian
        bmp.seek(0x12)
        byte = bmp.read(4)
        bitmap_width = (byte[3] << 24) + (byte[2] << 16) + (byte[1] << 8) + byte[0]  # 4 bytes integer in little-endian
        byte = bmp.read(4)
        bitmap_height = (byte[3] << 24) + (byte[2] << 16) + (byte[1] << 8) + byte[0]  # 4 bytes integer in little-endian
        bmp.seek(0x1C)
        depth = bmp.read(1)[0]
        sorted_pixel_bytes = b''
        pixel_data_size = bmp_filesize - pixel_data_start_offset
        bmp.seek(pixel_data_start_offset)
        pixel_data = bmp.read(pixel_data_size)
        if depth == 32:  # 4 bytes B G R A per pixel, no row padding needed
            for i in range(0, pixel_data_size, 4):
                sorted_pixel_bytes += (pixel_data[i + 2])  # R
                sorted_pixel_bytes += (pixel_data[i + 1])  # G
                sorted_pixel_bytes += (pixel_data[i])  # B
                sorted_pixel_bytes += (pixel_data[i + 3])  # A
        elif depth == 24:  # 3 bytes B G R per pixel (A is always 255), row padding of 4 bytes needed
            i = j = 0
            row_length = (bitmap_width * 3)  # the row length in bytes
            row_padding = abs(4 - row_length) % 4
            while i < pixel_data_size:
                if j + row_padding == row_length:
                    i += row_padding
                    j = 0
                    continue
                sorted_pixel_bytes += (pixel_data[i + 2:i + 3])  # R
                sorted_pixel_bytes += (pixel_data[i + 1:i + 2])  # G
                sorted_pixel_bytes += (pixel_data[i:i + 1])  # B
                sorted_pixel_bytes += (b'\xff')  # A
                j += 3
                i += 3
        elif depth == 16:  # 2 bytes AAAA BBBB   GGGG RRRR  per pixel (each channel has a value multiple of 16), row padding of 4 bytes needed
            i = j = 0
            row_length = (bitmap_width * 3)  # the row length in bytes
            row_padding = abs(4 - row_length) % 4
            while i < pixel_data_size:
                if j + row_padding == row_length:
                    i += row_padding
                    j = 0
                    continue
                sorted_pixel_bytes += (bytes(chr((pixel_data[i + 1] & 0x0f) << 4)), 'latin-1')  # R 4 bits
                sorted_pixel_bytes += (bytes(chr(pixel_data[i + 1] & 0xf0)), 'latin-1')  # G 4 bits
                sorted_pixel_bytes += (bytes(chr(pixel_data[i] & 0x0f << 4)), 'latin-1')  # B 4 bits
                sorted_pixel_bytes += (bytes(chr(pixel_data[i] & 0xf0)), 'latin-1')  # A 4 bits
                j += 2
                i += 2
        elif depth == 8:  # need to read the colour table
            bmp.seek(0x2E)
            byte = bmp.read(4)
            number_of_colours_in_the_palette = (byte[3] << 24) + (byte[2] << 16) + (byte[1] << 8) + byte[0]  # 4 bytes integer in little-endian
            colour_table_size = (number_of_colours_in_the_palette * 4)
            colour_table_start_offset = pixel_data_start_offset - colour_table_size
            bmp.seek(colour_table_start_offset)
            colour_table = bmp.read(colour_table_size)
            for i in range(pixel_data_size):
                sorted_pixel_bytes += bytes(chr((colour_table[(pixel_data[i] * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[(pixel_data[i] * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[(pixel_data[i] * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[(pixel_data[i] * 4) + 3])), 'latin-1')  # A
        elif depth == 4:  # need to read the colour table
            bmp.seek(0x2E)
            byte = bmp.read(4)
            number_of_colours_in_the_palette = (byte[3] << 24) + (byte[2] << 16) + (byte[1] << 8) + byte[0]  # 4 bytes integer in little-endian
            colour_table_size = (number_of_colours_in_the_palette * 4)
            colour_table_start_offset = pixel_data_start_offset - colour_table_size
            bmp.seek(colour_table_start_offset)
            colour_table = bmp.read(colour_table_size)
            for i in range(pixel_data_size):  # each byte has a 4 bit pixel refering to the colour index in the colour table. as a for can't advance lower than 1, I'm adding two of them at the same time
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0xf0) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0xf0) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0xf0) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0xf0) * 4) + 3])), 'latin-1')  # A

                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x0f) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x0f) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x0f) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x0f) * 4) + 3])), 'latin-1')  # A
        elif depth == 2:  # need to read the colour table
            bmp.seek(0x2E)
            byte = bmp.read(4)
            number_of_colours_in_the_palette = (byte[3] << 24) + (byte[2] << 16) + (byte[1] << 8) + byte[0]  # 4 bytes integer in little-endian
            colour_table_size = (number_of_colours_in_the_palette * 4)
            colour_table_start_offset = pixel_data_start_offset - colour_table_size
            bmp.seek(colour_table_start_offset)
            colour_table = bmp.read(colour_table_size)
            for i in range(pixel_data_size):  # each byte has a 4 bit pixel refering to the colour index in the colour table. as a for can't advance lower than 1, I'm adding two of them at the same time
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x03) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x03) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x03) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x03) * 4) + 3])), 'latin-1')  # A

                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x0C) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x0C) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x0C) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x0C) * 4) + 3])), 'latin-1')  # A

                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x30) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x30) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x30) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x30) * 4) + 3])), 'latin-1')  # A

                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0xC0) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0xC0) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0xC0) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0xC0) * 4) + 3])), 'latin-1')  # A

        elif depth == 1:  # need to read the colour table
            bmp.seek(0x2E)
            byte = bmp.read(4)
            number_of_colours_in_the_palette = (byte[3] << 24) + (byte[2] << 16) + (byte[1] << 8) + byte[0]  # 4 bytes integer in little-endian
            colour_table_size = (number_of_colours_in_the_palette * 4)
            colour_table_start_offset = pixel_data_start_offset - colour_table_size
            bmp.seek(colour_table_start_offset)
            colour_table = bmp.read(colour_table_size)
            for i in range(pixel_data_size):  # each byte has a 4 bit pixel refering to the colour index in the colour table. as a for can't advance lower than 1, I'm adding two of them at the same time
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x01) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x01) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x01) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x01) * 4) + 3])), 'latin-1')  # A

                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x02) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x02) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x02) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x02) * 4) + 3])), 'latin-1')  # A

                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x04) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x04) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x04) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x04) * 4) + 3])), 'latin-1')  # A

                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x08) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x08) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x08) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x08) * 4) + 3])), 'latin-1')  # A

                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x10) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x10) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x10) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x10) * 4) + 3])), 'latin-1')  # A

                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x20) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x20) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x20) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x20) * 4) + 3])), 'latin-1')  # A

                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x40) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x40) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x40) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x40) * 4) + 3])), 'latin-1')  # A

                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x80) * 4) + 2])), 'latin-1')  # R
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x80) * 4) + 1])), 'latin-1')  # G
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x80) * 4)])), 'latin-1')  # B
                sorted_pixel_bytes += bytes(chr((colour_table[((pixel_data[i] & 0x80) * 4) + 3])), 'latin-1')  # A
        func_to_launch_after_this_function(sorted_pixel_bytes, output_file_name, colour, bitmap_width, bitmap_height, texture_colour_format_int32)


create_palette_function = (create_PLT0_IA8, create_PLT0_IA8, create_PLT0_RGB565, create_PLT0_RGB5A3, create_PLT0_auto)
# create_texture_function = (create_TEX0_CI4, create_TEX0_CI4, create_TEX0_CI4, create_TEX0_CI8, create_TEX0_CI8, create_TEX0_CI14x2, create_TEX0_CI14x2)
list_of_textures_formats_int32 = (b'\x00\x00\x00\x08', b'\x00\x00\x00\x08', b'\x00\x00\x00\x08', b'\x00\x00\x00\x09', b'\x00\x00\x00\x09', b'\x00\x00\x00\x0A', b'\x00\x00\x00\x0A')

bmp_output_filename = os.path.splitext(input_file)[0] + '.bmp'
i = 0
while os.path.exists(bmp_output_filename):
    i += 1
    bmp_output_filename = f"{os.path.splitext(input_file)[0]}-{i}.bmp"
if input_is_already_bmp:
    bmp_output_filename = input_file
# texture_function = ''
texture_format_int32 = b'\x00\x00\x00\x09'
for i in range(len(texture_encoding_formats)):
    if selected_texture_format == texture_encoding_formats[i]:
        # texture_function = create_texture_function[i]
        texture_format_int32 = list_of_textures_formats_int32[i]
if user_input_is_correct:
    for i in range(len(palette_formats)):
        if selected_palette_format == palette_formats[i]:
            if use_pil and not input_is_already_bmp:
                try:
                    palette = Image
                    pic = Image.open(input_file)
                    picture_width = pic.width
                    picture_height = pic.height
                    # pic = pic.convert(mode='LA', colors=colour_number)
                    # pic = pic.quantize(colors=colour_number)  #, palette=palette)
                    pixel_array = pic.tobytes()  # each pixel one after another is a 4 bytes RGBA
                    # colour_palette = pic.getpalette()
                    # print(pixel_array, colour_palette, pic.palette, sep='\n')
                    # pic.save(bmp_output_filename + '.png')
                    pic.close()
                    create_palette_function[i](pixel_array, output_file, colour_number, picture_width, picture_height, texture_format_int32)
                except UnidentifiedImageError:
                    print("PIL can't convert the input file to bmp, try using ImageMagick")
            if use_magick and not input_is_already_bmp:
                out = subprocess.run([ImageMagick_path, input_file, bmp_output_filename], stdout=subprocess.PIPE, stderr=subprocess.STDOUT).stdout
            if os.path.exists(bmp_output_filename) and not use_pil:
                process_bmp(create_palette_function[i], bmp_output_filename, output_file, colour_number, texture_format_int32)
print(colour_number, selected_palette_format, selected_texture_format)
