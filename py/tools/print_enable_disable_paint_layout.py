import pyperclip
txt = """                cmpr_c1.Visible = true;
                cmpr_c1_label.Location = new Point(cmpr_c1_label.Location.X - 1920, cmpr_c1_label.Location.Y);
                cmpr_c1_hitbox.Location = new Point(cmpr_c1_hitbox.Location.X - 1920, cmpr_c1_hitbox.Location.Y);
                cmpr_c2.Location = new Point(cmpr_c2.Location.X - 1920, cmpr_c2.Location.Y);
                cmpr_c2_label.Location = new Point(cmpr_c2_label.Location.X - 1920, cmpr_c2_label.Location.Y);
                cmpr_c2_hitbox.Location = new Point(cmpr_c2_hitbox.Location.X - 1920, cmpr_c2_hitbox.Location.Y);
                cmpr_c3.Location = new Point(cmpr_c3.Location.X - 1920, cmpr_c3.Location.Y);
                cmpr_c3_label.Location = new Point(cmpr_c3_label.Location.X - 1920, cmpr_c3_label.Location.Y);
                cmpr_c3_hitbox.Location = new Point(cmpr_c3_hitbox.Location.X - 1920, cmpr_c3_hitbox.Location.Y);
                cmpr_c4.Location = new Point(cmpr_c4.Location.X - 1920, cmpr_c4.Location.Y);
                cmpr_c4_label.Location = new Point(cmpr_c4_label.Location.X - 1920, cmpr_c4_label.Location.Y);
                cmpr_c4_hitbox.Location = new Point(cmpr_c4_hitbox.Location.X - 1920, cmpr_c4_hitbox.Location.Y);
                cmpr_sel.Location = new Point(cmpr_sel.Location.X - 1920, cmpr_sel.Location.Y);
                cmpr_sel_label.Location = new Point(cmpr_sel_label.Location.X - 1920, cmpr_sel_label.Location.Y);
                cmpr_sel_hitbox.Location = new Point(cmpr_sel_hitbox.Location.X - 1920, cmpr_sel_hitbox.Location.Y);
                cmpr_swap_ck.Location = new Point(cmpr_swap_ck.Location.X - 1920, cmpr_swap_ck.Location.Y);
                cmpr_swap_label.Location = new Point(cmpr_swap_label.Location.X - 1920, cmpr_swap_label.Location.Y);
                cmpr_swap_hitbox.Location = new Point(cmpr_swap_hitbox.Location.X - 1920, cmpr_swap_hitbox.Location.Y);
                cmpr_selected_block_label.Location = new Point(cmpr_selected_block_label.Location.X - 1920, cmpr_selected_block_label.Location.Y);
                cmpr_picture_tooltip_label.Location = new Point(cmpr_picture_tooltip_label.Location.X - 1920, cmpr_picture_tooltip_label.Location.Y);
                cmpr_block1.Location = new Point(cmpr_block1.Location.X - 1920, cmpr_block1.Location.Y);
                cmpr_block2.Location = new Point(cmpr_block2.Location.X - 1920, cmpr_block2.Location.Y);
                cmpr_block3.Location = new Point(cmpr_block3.Location.X - 1920, cmpr_block3.Location.Y);
                cmpr_block4.Location = new Point(cmpr_block4.Location.X - 1920, cmpr_block4.Location.Y);
                cmpr_block5.Location = new Point(cmpr_block5.Location.X - 1920, cmpr_block5.Location.Y);
                cmpr_block6.Location = new Point(cmpr_block6.Location.X - 1920, cmpr_block6.Location.Y);
                cmpr_block7.Location = new Point(cmpr_block7.Location.X - 1920, cmpr_block7.Location.Y);
                cmpr_block8.Location = new Point(cmpr_block8.Location.X - 1920, cmpr_block8.Location.Y);
                cmpr_block9.Location = new Point(cmpr_block9.Location.X - 1920, cmpr_block9.Location.Y);
                cmpr_blockA.Location = new Point(cmpr_blockA.Location.X - 1920, cmpr_blockA.Location.Y);
                cmpr_blockB.Location = new Point(cmpr_blockB.Location.X - 1920, cmpr_blockB.Location.Y);
                cmpr_blockC.Location = new Point(cmpr_blockC.Location.X - 1920, cmpr_blockC.Location.Y);
                cmpr_blockD.Location = new Point(cmpr_blockD.Location.X - 1920, cmpr_blockD.Location.Y);
                cmpr_blockE.Location = new Point(cmpr_blockE.Location.X - 1920, cmpr_blockE.Location.Y);
                cmpr_blockF.Location = new Point(cmpr_blockF.Location.X - 1920, cmpr_blockF.Location.Y);
                cmpr_blockG.Location = new Point(cmpr_blockG.Location.X - 1920, cmpr_blockG.Location.Y);
                cmpr_block_selection_ck.Location = new Point(cmpr_block_selection_ck.Location.X - 1920, cmpr_block_selection_ck.Location.Y);
                cmpr_block_selection_label.Location = new Point(cmpr_block_selection_label.Location.X - 1920, cmpr_block_selection_label.Location.Y);
                cmpr_block_selection_hitbox.Location = new Point(cmpr_block_selection_hitbox.Location.X - 1920, cmpr_block_selection_hitbox.Location.Y);
                cmpr_block_paint_ck.Location = new Point(cmpr_block_paint_ck.Location.X - 1920, cmpr_block_paint_ck.Location.Y);
                cmpr_block_paint_label.Location = new Point(cmpr_block_paint_label.Location.X - 1920, cmpr_block_paint_label.Location.Y);
                cmpr_block_paint_hitbox.Location = new Point(cmpr_block_paint_hitbox.Location.X - 1920, cmpr_block_paint_hitbox.Location.Y);
                cmpr_save_ck.Location = new Point(cmpr_save_ck.Location.X - 1920, cmpr_save_ck.Location.Y);
                cmpr_save_as_ck.Location = new Point(cmpr_save_as_ck.Location.X - 1920, cmpr_save_as_ck.Location.Y);
                cmpr_save_hitbox.Location = new Point(cmpr_save_hitbox.Location.X - 1920, cmpr_save_hitbox.Location.Y);
                cmpr_save_as_hitbox.Location = new Point(cmpr_save_as_hitbox.Location.X - 1920, cmpr_save_as_hitbox.Location.Y);
                cmpr_warning_label.Location = new Point(cmpr_warning_label.Location.X - 1920, cmpr_warning_label.Location.Y);"""
txt = txt.splitlines()
output = ""
for line in txt:
    output += line.split('.')[0] + '.Visible = true;\n'

for line in txt:
    output += line.split('.')[0] + '.Visible = false;\n'
pyperclip.copy(output)
print(output)
print("Generated " + str(output.count('\n')) + " lines of C# code")
input("Press enter to exit...")