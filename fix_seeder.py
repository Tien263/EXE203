import os

filename = r'f:\FPTU\Ki8\EXE201\Exe_Demo_1\Exe_Demo_1\Exe_Demo\Data\DatabaseSeeder.cs'

with open(filename, 'r', encoding='utf-8') as f:
    content = f.read()

# Try to fix the mojibake. The text seems to be UTF-8 interpreted as CP1252 and then saved as UTF-8.
def fix_mojibake(text):
    try:
        # First, find substrings that look like mojibake and fix them
        # Let's just fix the whole text
        return text.encode('cp1252').decode('utf-8')
    except Exception as e:
        print("Error full fix:", e)
        # We can't do full fix because some parts might be valid UTF-8
        pass
    
    # Manual replacements for the known ones:
    replacements = {
        "SÁº¤Y GIÃ’N": "SẤY GIÒN",
        "XoÃ i Sáº¥y Giá»n": "Xoài Sấy Giòn",
        "MÃ­t Sáº¥y Giá»n": "Mít Sấy Giòn",
        "Chuá»‘i Sáº¥y Giá»n": "Chuối Sấy Giòn",
        "Khoai Lang Sáº¥y Giá»n": "Khoai Lang Sấy Giòn",
        "Tháº­p Cáº©m Sáº¥y Giá»n": "Thập Cẩm Sấy Giòn",
        "Tháº­p Cáº©m Sáº¥y Giá»n Mini": "Thập Cẩm Sấy Giòn Mini",
        "Sáº¤Y THÄ‚NG HOA": "SẤY THĂNG HOA",
        "DÃ¢u Sáº¥y ThÄƒng Hoa": "Dâu Sấy Thăng Hoa",
        "Sá»¯a Chua Sáº¥y ThÄƒng Hoa": "Sữa Chua Sấy Thăng Hoa",
        "Na Sáº¥y ThÄƒng Hoa": "Na Sấy Thăng Hoa",
        "Sáº§u RiÃªng Sáº¥y ThÄƒng Hoa": "Sầu Riêng Sấy Thăng Hoa",
        "NhÃ£n Sáº¥y ThÄƒng Hoa": "Nhãn Sấy Thăng Hoa",
        "Cam Sáº¥y ThÄƒng Hoa": "Cam Sấy Thăng Hoa",
        "Sáº¤Y DáºmultiO": "SẤY DẺO", # I typed this wrong above
        "Sáº¤Y Dáº»O": "SẤY DẺO",
        "XoÃ i Sáº¥y Dáº»o": "Xoài Sấy Dẻo",
        "Máº­n Sáº¥y Dáº»o": "Mận Sấy Dẻo",
        "Ä Ã o Sáº¥y Dáº»o": "Đào Sấy Dẻo",
        "DÆ°á»£u Sáº¥y Dáº»o": "Dâu Sấy Dẻo",
        "Há»“ng Sáº¥y Dáº»o": "Hồng Sấy Dẻo",
        "MÃ­t Sáº¥y Dáº»o": "Mít Sấy Dẻo",
        "XoÃ i sáº¥y giá»n thÆ¡m ngon, giá»¯ trá» n hÆ°Æ¡ng vá»‹ tá»± nhiÃªn.": "Xoài sấy giòn thơm ngon, giữ trọn hương vị tự nhiên.",
        "MÃ­t sáº¥y giá»n vÃ ng á»‘m, giÃ²n tan.": "Mít sấy giòn vàng óng, giòn tan.",
        "Chuá»‘i sáº¥y giá»n truyá» n thá»‘ng.": "Chuối sấy giòn truyền thống.",
        "Khoai lang sáº¥y giá»n tá»± nhiÃªn.": "Khoai lang sấy giòn tự nhiên.",
        "Tháº­p Cáº©m": "Thập Cẩm",
        "CÃ¡c loáº¡i cÅ© quáº£ sáº¥y giá»n": "Các loại củ quả sấy giòn",
        "TÃºi nhá»  tiá»‡n lá»£i": "Túi nhỏ tiện lợi",
        "GÃ³i": "Gói",
        "Há»™p": "Hộp",
        "DÃ¢u tÃ¢y sáº¥y thÄƒng hoa cao cáº¥p.": "Dâu tây sấy thăng hoa cao cấp.",
        "ViÃªn sá»¯a chua sáº¥y giÃ²n tan, bá»• dÆ°á»¡ng.": "Viên sữa chua sấy giòn tan, bổ dưỡng.",
        "Na sáº¥y thÄƒ hoa giá»¯ nguyÃªn cáº¥u trÃºc vÃ  dÆ°á»¡ng cháº¥t.": "Na sấy thăng hoa giữ nguyên cấu trúc và dưỡng chất.",
        "Sáº§u riÃªng sáº¥y thÄƒng hoa thÆ¡m ná»©c.": "Sầu riêng sấy thăng hoa thơm nức.",
        "CÆ¡i nhÃ£n sáº¥y thÄƒng hoa ngá» t thanh.": "Cùi nhãn sấy thăng hoa ngọt thanh.",
        "LÃ¡t cam sáº¥y thÄƒng hoa dÃ¹ng pha trÃ  hoáº·c Äƒn trá»±c tiáº¿p.": "Lát cam sấy thăng hoa dùng pha trà hoặc ăn trực tiếp.",
        "XoÃ i sáº¥y dáº»o chua ngá» t, dai ngon.": "Xoài sấy dẻo chua ngọt, dai ngon.",
        "Máº­n sáº¥y dáº»o khÃ´ng háº¡t.": "Mận sấy dẻo không hạt.",
        "Ä Ã o sáº¥y dáº»o thÆ¡m lÃ«ng.": "Đào sấy dẻo thơm lừng.",
        "DÃ¢u tÃ¢y sáº¥y dáº»o nguyÃªn trÃ¡i.": "Dâu tây sấy dẻo nguyên trái.",
        "Há»“ng treo giÃ³ sáº¥y dáº»o Ä Ã  Láº¡t.": "Hồng treo gió sấy dẻo Đà Lạt.",
        "MÃ­t sáº¥y dáº»o ngá» t lán.": "Mít sấy dẻo ngọt lịm.",
        "tháº­p cáº©m cÃ¡c loáº¡i cÅ© quáº£ sáº¥y giá»n.": "Thập cẩm các loại củ quả sấy giòn."
    }
    
    for k, v in replacements.items():
        text = text.replace(k, v)
        
    return text

new_content = fix_mojibake(content)

with open(filename, 'w', encoding='utf-8') as f:
    f.write(new_content)

print("Saved fixed database seeder.")
