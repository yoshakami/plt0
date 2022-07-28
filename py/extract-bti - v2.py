import os

tex = 0
colourenc = ['I4', 'I8', 'IA4', 'IA8', 'RGB565', 'RGB5A3', 'RGBA8', 0, 'CI4', 'CI8', 'CI14x2', 0, 0, 0, 'CMPR']
block_widths = [8, 8, 8, 4, 4, 4, 4, 0, 8, 8, 4, 0, 0, 0, 8]
block_heights = [8, 4, 4, 4, 4, 4, 4, 0, 8, 4, 4, 0, 0, 0, 8]
for file in os.listdir('./'):
    if os.path.splitext(file)[-1] == '.bmd':
        print(f"dumping bti files from {file}...")
        with open(file, 'rb') as bmd:
            nextsection = 0x20
            while bmd.read(3) != b"TEX":
                bmd.seek(nextsection + 4)
                byte = bmd.read(4)
                if len(byte) != 4:
                    input(f"reached end of {file}. no TEX section there?")  # never happened in MKDD
                    continue
                nextsection += (byte[0] << 24) + (byte[1] << 16) + (byte[2] << 8) + byte[3]  # 4 bytes integer
                bmd.seek(nextsection)
            if bmd.read(1) != b'1':
                print(file, nextsection)  # never happened in MKDD
            # here, nextsection is the offset to the TEX section, which contains all bti headers then all bti data I guess
            if not os.path.exists(os.path.splitext(file)[0]):
                os.mkdir(os.path.splitext(file)[0])
            headers = []  # contains all bti headers
            bmd.seek(nextsection + 8)
            byte = bmd.read(2)
            bti_length = (byte[0] << 8) + byte[1]  # unsigned short
            bmd.seek(nextsection + 0x10)
            byte = bmd.read(4)
            string_pool_table_start_offset = nextsection + (byte[0] << 24) + (byte[1] << 16) + (byte[2] << 8) + byte[3]  # 4 bytes integer

            # get the names from the string pool table
            bmd.seek(string_pool_table_start_offset)
            byte = bmd.read(2)
            string_count = (byte[0] << 8) + byte[1]  # unsigned short
            bmd.seek(string_pool_table_start_offset + 4 + (string_count * 4))  # don't ask me why on SuperBMD the hash is a ushort then it writes two 0 while on vanilla bmd only 1 zero is written, and sometimes it's even a 1...
            names = bmd.read()
            if b'\x00' not in names:
                # input(f"HOW THE HELL IS NOT \\x00 IN THE STRING TABLE FOR {file}\nskipping...")
                print("no bti file found.\nskipping...")
                continue
            names = names.split(b'\x00')[:-1]  # don't take b'This is padding data to '
            # extracts the bti files
            pos = 0  # current position to add to the bti data start
            if bti_length != string_count:
                input(f"WHAT THE FUCK !? {string_count} string for {bti_length} bti !!?")  # never happened in MKDD
            bmd.seek(nextsection + 0x20)
            for i in range(bti_length):
                headers.append(bmd.read(32))  # reads all bti headers
            print(f"{bti_length} textures found")
            for i in range(bti_length):
                colour_number = (headers[i][10] << 8) + headers[i][11]
                if colour_number != 0:  # or headers[i][8] == 1 works too haha
                    palette = True
                else:
                    palette = False
                block_data_size = 32
                if (headers[i][0]) == 6:  # RGBA32 has a block width of 64 bytes
                    block_data_size = 64
                block_width = block_widths[headers[i][0]]
                block_height = block_heights[headers[i][0]]
                width = (headers[i][2] << 8) + headers[i][3]
                height = (headers[i][4] << 8) + headers[i][5]
                blocks_wide = (width + (block_width - 1)) // block_width
                blocks_tall = (height + (block_height - 1)) // block_height
                image_data_size = blocks_wide * blocks_tall * block_data_size
                remaining_mipmaps = headers[i][0x18] - 1
                curr_mipmap_size = image_data_size
                while remaining_mipmaps > 0:
                    # Each mipmap is a quarter the size of the last (half the width and half the height).
                    curr_mipmap_size = curr_mipmap_size // 4
                    image_data_size += curr_mipmap_size
                    remaining_mipmaps -= 1
                if image_data_size == 0:
                    input(f"block_width = {block_width}, block_height = {block_height}, width = {width}, height = {height}")
                tex += 1
                pos += 0x20
                bti_data_start = (headers[i][28] << 24) + (headers[i][29] << 16) + (headers[i][30] << 8) + headers[i][31]  # 4 bytes integer
                bmd.seek(nextsection + pos + bti_data_start)
                # I have to change the header else the files can't be opened.
                if palette:
                    bti_palette_start = (headers[i][12] << 24) + (headers[i][13] << 16) + (headers[i][14] << 8) + headers[i][15]  # 4 bytes integer
                    palette_data = bti.read(colour_number * 2)  # the size of the colour palette is 2 bytes per color no matter the format.
                    with open(os.path.splitext(file)[0] + "/" + str(names[i])[2:-1] + ".bti", "wb") as bti:
                        bti.write(headers[i][:12] + b'\x00\x00\x00\x20' + headers[i][12:28] + bytes(chr(32 + (len(palette_data)) >> 24), 'latin-1') + bytes(chr(32 + ((len(palette_data) >> 16) % 256)), 'latin-1') + bytes(chr(32 + ((len(palette_data) >> 8) % 256)), 'latin-1') +  + bytes(chr(32 + (len(palette_data) % 256)), 'latin-1') + palette_data + bmd.read(image_data_size))
                else:
                    with open(os.path.splitext(file)[0] + "/" + str(names[i])[2:-1] + ".bti", "wb") as bti:
                        bti.write(headers[i][:28] + b'\x00\x00\x00\x20' + bmd.read(image_data_size))
input(f"dumped {tex} bti files!")
