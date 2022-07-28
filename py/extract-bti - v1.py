import os

tex = 0
colourenc = ['I4', 'I8', 'IA4', 'IA8', 'RGB565', 'RGB5A3', 'RGBA8', 0, 'CI4', 'CI8', 'CI14x2', 0, 0, 0, 'CMPR']
block_widths = [8, 8, 8, 4, 4, 4, 4, 0, 8, 8, 4, 0, 0, 0, 8]
block_heights = [8, 4, 4, 4, 4, 4, 4, 0, 8, 4, 4, 0, 0, 0, 8]
for file in os.listdir('./'):
    if os.path.splitext(file)[-1] == '.bmd':
        with open(file, 'rb') as bmd:
            """size = os.path.getsize(file)
            i = 16
            bmd.seek(size - i + 2)
            while bmd.read(2) != b'\xff\xff':
                i += 16
                bmd.seek(size - i + 2)
            bmd.seek(size - i)
            byte = bmd.read(2)
            string_count = (byte[0] << 8) + byte[1]  # unsigned short
            bmd.seek(size - i + (string_count * 4))  # don't ask me why on SuperBMD the hash is a ushort then it writes two 0 while on vanilla bmd only 1 zero is written, and sometimes it's even a 1...
            names = bmd.read()
            if b'\x00' not in names:
                input(f"HOW THE HELL IS NOT \\x00 IN THE STRING TABLE FOR {file}\nskipping...")
                continue
            names = names.split(b'\x00')[:-1] # don't take b'This is padding data to '
            print(names)"""
            nextsection = 0x20
            while bmd.read(3) != b"TEX":
                bmd.seek(nextsection + 4)
                byte = bmd.read(4)
                if len(byte) != 4:
                    input(f"reached end of {file}. no TEX section there?")
                    continue
                print(byte)
                nextsection += (byte[0] << 24) + (byte[1] << 16) + (byte[2] << 8) + byte[3]  # 4 bytes integer
                bmd.seek(nextsection)
            if bmd.read(1) != b'1':
                print(file, nextsection)
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
                input(f"HOW THE HELL IS NOT \\x00 IN THE STRING TABLE FOR {file}\nskipping...")
                continue
            names = names.split(b'\x00')[:-1]  # don't take b'This is padding data to '
            print(names)

            # extracts the bti files
            pos = 0  # current position to add to the bti data start
            if bti_length != string_count:
                input(f"WHAT THE FUCK !? {string_count} string for {bti_length} bti !!?")
            bmd.seek(nextsection + 0x20)
            for i in range(bti_length):
                headers.append(bmd.read(32))  # reads all bti headers
            print(headers)
            for i in range(bti_length):
                block_data_size = 32
                if (headers[i][0]) == 6:  # RGBA32 has a block width of 64 bytes
                    block_data_size = 64
                block_width = block_widths[headers[i][0]]
                block_height = block_heights[headers[i][0]]
                width = headers[i][2] << 8 + headers[i][3]
                height = headers[i][4] << 8 + headers[i][5]
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
                tex += 1
                pos += 0x20
                bti_data_start = (headers[i][28] << 24) + (headers[i][29] << 16) + (headers[i][30] << 8) + headers[i][31]  # 4 bytes integer
                bmd.seek(nextsection + pos + bti_data_start)
                """if i == bti_length - 1:
                    bti_data_end = string_pool_table_start_offset  # the last bti data ends there
                else:
                    bti_data_end = (headers[i + 1][28] << 24) + (headers[i + 1][29] << 16) + (headers[i + 1][30] << 8) + headers[i + 1][31]  # 4 bytes integer
                with open(os.path.splitext(file)[0] + "/" + str(names[i])[2:-1] + ".bti", "wb") as bti:
                    bti.write(bmd.read(bti_data_end + 0x20 - bti_data_start))"""
                with open(os.path.splitext(file)[0] + "/" + str(names[i])[2:-1] + ".bti", "wb") as bti:
                    bti.write(bmd.read(image_data_size))
input(f"dumped {tex} bti files!")
