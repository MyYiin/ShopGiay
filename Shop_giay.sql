Use Master
GO
    IF EXISTS(SELECT name FROM sys.databases WHERE name='shop_giay' )
	Begin
	ALTER DATABASE shop_giay SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE shop_giay
	end
GO
    CREATE DATABASE shop_giay
GO


USE shop_giay;
-- 1. Bảng CỬA HÀNG (Thông tin cửa hàng)
CREATE TABLE CUAHANG(
	MaCH int primary key identity(1,1),
	Ten nvarchar(100) not null,
	DienThoai varchar(20),
	DiaChi nvarchar(100)
)
GO

-- 2. Bảng LOẠI GIÀY 
CREATE TABLE LOAIGIAY(
	MaLG int primary key identity(1,1),
	Ten nvarchar(100) not null 
)
GO

-- 3. Bảng THƯƠNG HIỆU
CREATE TABLE THUONGHIEU(
	MaTH int primary key identity(1,1),
	Ten nvarchar(100) not null UNIQUE
)
GO
-- 4. Bảng KÍCH CỠ (Cần để các biến thể có thể tham chiếu)
CREATE TABLE KICHCO(
	MaKC int primary key identity(1,1),
	GiaTriKC float not null UNIQUE -- Ví dụ: 38.0, 38.5, 39.0
)
GO

-- 5. Bảng MÀU SẮC (Cần để các biến thể có thể tham chiếu)
CREATE TABLE MAUSAC(
	MaMS int primary key identity(1,1),
	Ten nvarchar(50) not null,
	MaHex varchar(7) -- Mã màu Hex, ví dụ: #FFFFFF
)
GO
-- 6. Bảng GIAY: Thông tin chung về mẫu giày
CREATE TABLE MATHANG(
	MaMh int primary key identity(1,1),
	Ten nvarchar(100) not null,
	MoTa nvarchar(1000),
	GiaGoc int default 0,
	GiaBan int default 0, -- Giá cơ bản của mẫu giày
	MaLG int not null foreign key(MaLG) references LOAIGIAY(MaLG),
	MaTH int not null foreign key(MaTH) references THUONGHIEU(MaTH),
	HinhAnh varchar(255), -- Hình ảnh đại diện cho mẫu giày
	LuotXem int default 0,
	LuotMua int default 0
)
GO
-- 7. Bảng BIẾN THỂ GIÀY (Tồn kho chi tiết theo Size x Màu)
CREATE TABLE TONKHO (
    MaK int primary key identity(1,1),
    MaMh int not null foreign key(MaMh) references MATHANG(MaMh) ON DELETE CASCADE,
    MaMS int not null foreign key(MaMS) references MAUSAC(MaMS),
    MaKC int not null foreign key(MaKC) references KICHCO(MaKC),
    SKU varchar(50) UNIQUE, -- Mã sản phẩm SKU (Ví dụ: NKPG-W-40)
    SoLuongTonKho smallint default 0,
	GiaGocBT int default 0,
    GiaBanBT int default 0, -- Giá bán cụ thể cho biến thể này (có thể khác GiaBan trong GIAY)
	UNIQUE (MaMh, MaMS, MaKC) -- Đảm bảo không có 2 biến thể trùng Màu và Size cho cùng 1 mẫu giày
)
GO


-- =================================================================
-- 2. KHÁCH HÀNG & NHÂN VIÊN
-- =================================================================
--8. CHỨC VỤ
CREATE TABLE CHUCVU(
	MaCV int primary key identity(1,1),
	Ten nvarchar(100) not null,
	HeSo float default 1.0
) 
GO
--9. NHÂN VIÊN
CREATE TABLE NHANVIEN(
	MaNV int primary key identity(1,1),
	Ten nvarchar(100) not null,
	MaCV int not null foreign key(MaCV) references CHUCVU(MaCV),
	DienThoai varchar(20),
	Email varchar(50),
	MatKhau varchar(255)	
) 
GO
--10. KHÁCH HÀNG
CREATE TABLE KHACHHANG(
	MaKH int primary key identity(1,1),
	Ten nvarchar(100) not null,
	DienThoai varchar(20)not null,
	Email varchar(50)not null,
	MatKhau varchar(500)
)
GO

CREATE TABLE DIACHI(
	MaDC int primary key identity(1,1),
	MaKH int not null foreign key(MaKH) references KHACHHANG(MaKH),
	DiaChi nvarchar(100) not null,
	PhuongXa nvarchar(50) ,
	QuanHuyen nvarchar(50),
	Tinh nvarchar(50) ,
	MacDinh int default 1
)
GO

-- =================================================================
-- 3. CÁC BẢNG QUẢN LÝ ĐƠN HÀNG VÀ GIAO DỊCH (ORDERS)
-- =================================================================

CREATE TABLE HOADON(
	MaHD int primary key identity(1,1),
	Ngay datetime default getdate(),
	TongTien int default 0,
	MaMh int not null foreign key(MaMh) references MATHANG(MaMh),
	MaKH int not null foreign key(MaKH) references KHACHHANG(MaKH),
	TrangThai int default 0 -- 0: Chờ xác nhận, 1: Đã xác nhận, 2: Đang giao, 3: Hoàn thành, 4: Hủy
)
GO

CREATE TABLE CTHOADON (
    MaCTHD int primary key identity(1,1),
    MaHD int not null foreign key(MaHD)references HOADON(MaHD),
    MaK int not null foreign key(MaK)references TONKHO(MaK),
	MaMS int not null foreign key(MaMS) references MAUSAC(MaMS),
    MaKC int not null foreign key(MaKC) references KICHCO(MaKC),
    DonGia int default 0, -- Giá bán tại thời điểm lập hóa đơn
    SoLuong smallint default 1,
    ThanhTien int
);

-- =================================================================
-- 4. BẢNG BỔ SUNG: ĐÁNH GIÁ SẢN PHẨM
-- =================================================================
CREATE TABLE DANHGIA(
    MaDG int primary key identity(1,1),
    MaMh int not null foreign key(MaMh) references MATHANG(MaMh),
    MaKH int not null foreign key(MaKH) references KHACHHANG(MaKH),
    Diem int not null CHECK (Diem >= 1 AND Diem <= 5), -- Điểm đánh giá từ 1 đến 5
    NoiDung text,
    NgayDG datetime default getdate(),
	UNIQUE (MaKH, MaMh) -- Mỗi khách hàng chỉ đánh giá 1 lần cho 1 mẫu giày
);
GO

-- 1. DỮ LIỆU CUAHANG
INSERT INTO CUAHANG (Ten, DienThoai, DiaChi) VALUES
(N'Cửa Hàng Giày Việt An Giang', '0901234567', N'Bình Thạnh, Châu Thành, An Giang ');

-- 2. DỮ LIỆU LOAIGIAY(Sử dụng phân loại chức năng)
INSERT INTO LOAIGIAY (Ten) VALUES
(N'Giày Thể Thao (Sneakers)'),     --1
(N'Giày Chạy Bộ Chuyên Dụng'),      --2
(N'Giày Tây / Giày Công Sở'),       --3
(N'Giày Lười (Loafer & Slip-on)'), --4
(N'Boots & Bốt Cổ Cao');        --5
 

-- 3. DỮ LIỆU THUONGHIEU
INSERT INTO THUONGHIEU (Ten) VALUES
(N'Nike'),   --1      
(N'Adidas'),       --2
(N'Vans'),         --3
(N'Pedro'),     --4
(N'Fila'),        --5
(N'Urban Revivo'),      --6
(N'Skechers'),        --7
(N'Antoni Fernando'),   --8
(N'DSQUARED2'),          --9
(N'Vascara');  --10
-- 4. DỮ LIỆU KICHCO 
INSERT INTO KICHCO (GiaTriKC) VALUES
(36.0), (37.0), (38.0), (39.0), (40.0), (41.0), (42.0), (43.0), (44.0);

-- 5. DỮ LIỆU MAUSAC
INSERT INTO MAUSAC (Ten, MaHex) VALUES
(N'Đen', '#000000'),          -- MaMS = 1
(N'Trắng', '#FFFFFF'),        -- MaMS = 2
(N'Xám', '#808080'),          -- MaMS = 3 (Xám Tro)
(N'Đỏ', '#FF0000'),           -- MaMS = 4 (Đỏ Tươi)
(N'Xanh Navy', '#000080'),    -- MaMS = 5 (Màu Xanh Đậm Phổ Biến)
(N'Nâu', '#964B00'),          -- MaMS = 6
(N'Xanh Tím', '#4B0082'),     -- MaMS = 7 (Hoặc Xanh Than Tím)
(N'Xanh Đen', '#1A1A33'),     -- MaMS = 8 (Tông màu rất tối, gần như đen)
(N'Xám Đen', '#36454F'),      -- MaMS = 9 (Màu than chì)
(N'Nâu Đỏ', '#800020'),       -- MaMS = 10 (Màu Đỏ đô/Burgundy)
(N'Đỏ Đô', '#8B0000');        -- MaMS = 11 (Màu Đỏ trầm)

-- 6. MẪU GIÀY THỂ THAO (MaG = 1)
INSERT INTO MATHANG (Ten, MoTa, GiaGoc, GiaBan, MaLG, MaTH, HinhAnh) VALUES
(N'Nike Air Zoom Pegasus 40', N'Giày chạy bộ/thể thao đa năng, đệm Zoom êm ái, phù hợp chạy hàng ngày.', 2800000, 2490000, 1, 1, 'ttnike01_white.jpg'),
(N'Giày Thể Thao Nike Renew 40',N'Giày Thể Thao Nike Renew Run 2 Black Photo Blue CU3504-002 Màu Đen Size 40.',2700000,1350000,1,1,'ttnike02_den.jpg'),
(N'Nike Tanjun Sneakers Black ',N'Nike Tanjun Sneakers Black with Rose Gold Swoosh Size 8.',563000,474000,1,1,'ttnike03_den.jpg'),
(N'Giày Buộc Dây CLIMACOOL ',N'Sản phẩm này không được hưởng bất kỳ giảm giá khuyến mại và ưu đãi nào.',4400000,4200000,2,2,'cbadidas01_w.jpg'),
(N'Giày Ultraboost 5 ',N'Đôi giày chạy bộ BOOST có sử dụng 20% chất liệu tái chế.',5000000,4900000,2,2,'cbadidas02_xanhden.jpg'),
(N'Giày Chạy Bộ Ultraboost 5 H.Koumori',N'NGiày chạy đàn hồi, được tạo ra trong sự hợp tác với Hermanos Koumori.',5100000,4900000,2,2,'cbadidas03_xamden.jpg'),
(N'VANS SK8-HI VINTAGE POP MARSHMALLOW TURT LEDOVE',N'Chất liệu 100% Leather phù hợp cho cả nam và nữ',2200000,1980000,1,3,'vans01_w.jpg'),
(N'VANS SK8 LOW BLACK',N'Chất liệu Suede, Canvas, Rubber. Phù hợp cho cả nam và nữ.',1750000,1575000,1,3,'vans02_b.jpg'),
(N'Giày lười nam mũi vuông Edge Leather',N'Với thiết kế tinh tế, đôi giày Edge Leather chinh phục mọi ánh nhìn bằng sự kết hợp hài hòa giữa đường may cổ điển và chi tiết kim loại hiện đại,tạo nên điểm nhấn vừa mang đậm nét truyền thống vừa thể hiện phong cách đương đại.',3000000,2599000,4,4,'glpedro01_b.jpg'),
(N'Giày chạy bộ unisex Eletrico',N'Thiết kế streamline hiện đại, tối ưu cho hoạt động chạy bộ và luyện tập',2000000,1999000,2,5,'cbfila01_xtim.jpg'),
(N'Giày pickleball unisex Volley Burst 2',N'Thiết kế tối giản, hiện đại với tông trắng sang trọng. Thích hợp dùng trong các dịp: Chơi pickleball, tập luyện thể thao, hoạt động ngoài trời,...',3700000,3599000,1,5,'ttfila02_w.jpg'),
(N'Giày sneakers nam cổ thấp Contrast Chunky',N'Chất liệu: Artificial material, synthetic leather. Lớp lót: Textile. Thích hợp dùng trong các dịp: Đi chơi, đi làm,...',1400000,1399000,1,6,'tturban01_nau.jpg'),
(N'Giày sneakers nam cổ thấp mũi tròn',N'Chất liệu: Artificial material, synthetic leather. Lớp lót: Textile. Thoáng khí: Có lớp lót thoáng khí...',1400000,1399000,1,6,'tturban02_do.jpg'),
(N'Giày slip on nam Performance Max Cushioning',N'Chất liệu: Textile.Thích hợp dùng trong các dịp: Hoạt động ngoài trời, đi làm, đi chơi,....',2700000,2590000,4,7,'glskechers01_w.jpg'),
(N'Giày Da Công Sở Nam Da Bò Cao Cấp Antoni Fernando MS-AF3505',N'Mẫu giày loafer như một biểu tượng của sự lịch lãm và phong cách chuyên nghiệp trong môi trường làm việc hiện đại. Được thiết kế thanh lịch kết hợp cùng chất liệu da bò thuộc tự nhiên cao cấp..',2200000,1850000,3,8,'gtantoni01_den.jpg'),
(N'Giày Da Công Sở Nam Da Bò Cao Cấp Antoni Fernando MS-AF3503 (-10%)',N'Lót talon: Da bò cao cấp hút mồ hôi, chống hôi. Đế: cao su cao cấp, độ chống bào mòn cao. Bảo hành sản phẩm 12 tháng.',1800000,1620000,3,8,'gtantoni02_den.jpg'),
(N'GIÀY LOAFER NAM CÔNG SỞ ĐẾ PHÍP Antoni Fernando AF30725',N'Sản phẩm được chế tác từ 100% da bò cao cấp, mang lại cảm giác mềm mại, êm ái, ôm trọn đôi chân bạn. ',1900000,1850000,3,8,'gdantoni03_do.jpg'),
(N'VANS CLASSIC SLIP-ON BLACK/WHITE',N'Dành cho những bạn bận bịu không thích thắt dây giày, phối màu đơn giản dễ phối đồ cùng với chất liệu dễ custom khiến đôi giày trở thành một trong những sự lựa chọn hàng đầu. ',1600000,1450000,4,3,'glvans03_den.jpg'),
(N'Giày boots nam cổ cao mũi tròn',N'Chất liệu: Textile, Synthetic leather. Lớp lót: Textile. Thoáng khí: Có lớp lót thoáng khí. Thích hợp dùng trong các dịp: Đi làm, đi chơi,...',1750000,1699000,5,6,'bootsnurban03_nau.jpg'),
(N'Giày boots nam cổ cao thắt dây Toronto',N'Thiết kế tinh tế kết hợp cùng chất liệu cao cấp được tối ưu hóa nhằm đạt hiệu suất tối đa là những điểm nhấn nổi bật của đôi giày boots Toronto, giúp chúng trở thành lựa chọn lý tưởng cho những hoạt động đi bộ trong mùa đông lạnh giá.',19900000,13900000,5,9,'bootsdsquared201_naudo.jpg'),
(N'Giày boots nam cổ cao thắt dây Canadian',N'Đôi giày boots Canadian của thương hiệu Dsquared2 là sự kết hợp hoàn hảo giữa phong cách mạnh mẽ và tinh tế. Với thiết kế cổ cao ôm sát mắt cá chân mang lại cảm giác vững chãi và an toàn khi di chuyển, đôi giày được chế tác từ chất liệu da cao cấp cùng đường chỉ khâu tinh tế ở viền đế giày và các đường rãnh sâu mang đến khả năng chống trượt, phù hợp cho nhiều loại địa hình. ',33000000,23100000,5,9,'bootsdsquared202_den.jpg'),
(N'Giày boots nam cổ thấp mũi tròn Hiking',N'Đôi giày Hiking là một món phụ kiện thời trang hoàn hảo, mang đến sự kết hợp giữa phong cách hiện đại và tính năng tiện dụng. Với thiết kế tinh tế, đôi giày được chế tác từ chất liệu cao cấp, kết hợp cùng đế giày chắc chắn mang lại sự ổn định và an toàn khi di chuyển trên nhiều bề mặt khác nhau.',26500000,18600000,5,9,'bootsdsquared203_nau.jpg');



-- DỮ LIỆU BIENTHE_GIAY (Tồn kho chi tiết)
-- Biến thể cho Nike Pegasus 40 (MaG=1)
-- 7. Bảng BIẾN THỂ GIÀY (Tồn kho chi tiết theo Size x Màu)
-- DỮ LIỆU BIENTHE_GIAY
-- MaKC: (3=38.0), (4=39.0), (5=40.0), (6=41.0), (7=42.0), (8=43.0), (9=44.0)
-- MaMS: (1=Đen), (2=Trắng), (6=Nâu), (8=Xanh Đen), (9=Xám Đen), (7=Xanh Tím), (4=Đỏ), (10=Nâu Đỏ)

INSERT INTO TONKHO (MaMh, MaMS, MaKC, SKU, GiaGocBT, GiaBanBT, SoLuongTonKho) VALUES

-- 1. Nike Air Zoom Pegasus 40 (MaG=1) - Giá 2490000
(1, 1, 5, 'NKPG40-B-40', 2800000, 2490000, 15), -- Đen, Size 40
(1, 1, 6, 'NKPG40-B-41', 2800000, 2490000, 10), -- Đen, Size 41
(1, 2, 4, 'NKPG40-W-39', 2800000, 2490000, 12), -- Trắng, Size 39
(1, 2, 5, 'NKPG40-W-40', 2800000, 2490000, 8),  -- Trắng, Size 40

-- 2. Giày Thể Thao Nike Renew 40 (MaG=2) - Giá 1350000 (Màu Đen)
(2, 1, 5, 'NKRN40-B-40', 2700000, 1350000, 20), -- Đen, Size 40
(2, 1, 6, 'NKRN40-B-41', 2700000, 1350000, 15), -- Đen, Size 41

-- 3. Nike Tanjun Sneakers Black (MaG=3) - Giá 474000 (Đen)
(3, 1, 3, 'NKTAN-B-38', 563000, 474000, 10), -- Đen, Size 38
(3, 1, 4, 'NKTAN-B-39', 563000, 474000, 18), -- Đen, Size 39
(3, 1, 5, 'NKTAN-B-40', 563000, 474000, 14), -- Đen, Size 40

-- 4. Giày Buộc Dây CLIMACOOL (MaG=4) - Giá 4200000 (Trắng - w.jpg)
(4, 2, 7, 'ADCL-W-42', 4400000, 4200000, 9),  -- Trắng, Size 42
(4, 2, 8, 'ADCL-W-43', 4400000, 4200000, 11), -- Trắng, Size 43

-- 5. Giày Ultraboost 5 (MaG=5) - Giá 4900000 (Xanh Đen - xanhden.jpg)
(5, 8, 6, 'ADUB5-XD-41', 5000000, 4900000, 13), -- Xanh Đen, Size 41
(5, 8, 7, 'ADUB5-XD-42', 5000000, 4900000, 7),  -- Xanh Đen, Size 42

-- 6. Giày Chạy Bộ Ultraboost 5 H.Koumori (MaG=6) - Giá 4900000 (Xám Đen - xamden.jpg)
(6, 9, 5, 'ADUB5-XAM-40', 5100000, 4900000, 6), -- Xám Đen, Size 40
(6, 9, 6, 'ADUB5-XAM-41', 5100000, 4900000, 10), -- Xám Đen, Size 41

-- 7. VANS SK8-HI VINTAGE POP (MaG=7) - Giá 1980000 (Trắng - w.jpg)
(7, 2, 3, 'VSK8-W-38', 2200000, 1980000, 11), -- Trắng, Size 38
(7, 2, 4, 'VSK8-W-39', 2200000, 1980000, 15), -- Trắng, Size 39

-- 8. VANS SK8 LOW BLACK (MaG=8) - Giá 1575000 (Đen - b.jpg)
(8, 1, 4, 'VSK8L-B-39', 1750000, 1575000, 17), -- Đen, Size 39
(8, 1, 5, 'VSK8L-B-40', 1750000, 1575000, 12), -- Đen, Size 40

-- 9. Giày lười nam mũi vuông Edge Leather (MaG=9) - Giá 2599000 (Đen - b.jpg)
(9, 1, 6, 'GLPEDRO-B-41', 3000000, 2599000, 8), -- Đen, Size 41
(9, 1, 7, 'GLPEDRO-B-42', 3000000, 2599000, 5), -- Đen, Size 42

-- 10. Giày chạy bộ unisex Eletrico (MaG=10) - Giá 1999000 (Xanh Tím - xtim.jpg)
(10, 7, 5, 'CBFILA-XT-40', 2000000, 1999000, 10), -- Xanh Tím, Size 40
(10, 7, 6, 'CBFILA-XT-41', 2000000, 1999000, 10), -- Xanh Tím, Size 41

-- 11. Giày pickleball unisex Volley Burst 2 (MaG=11) - Giá 3599000 (Trắng - w.jpg)
(11, 2, 4, 'TTFILA-W-39', 3700000, 3599000, 15), -- Trắng, Size 39
(11, 2, 5, 'TTFILA-W-40', 3700000, 3599000, 10), -- Trắng, Size 40

-- 12. Giày sneakers nam cổ thấp Contrast Chunky (MaG=12) - Giá 1399000 (Nâu - nau.jpg)
(12, 6, 6, 'TTURB-N-41', 1400000, 1399000, 18), -- Nâu, Size 41
(12, 6, 7, 'TTURB-N-42', 1400000, 1399000, 14), -- Nâu, Size 42

-- 13. Giày sneakers nam cổ thấp mũi tròn (MaG=13) - Giá 1399000 (Đỏ - do.jpg)
(13, 4, 5, 'TTURB-D-40', 1400000, 1399000, 10), -- Đỏ, Size 40
(13, 4, 6, 'TTURB-D-41', 1400000, 1399000, 8),  -- Đỏ, Size 41

-- 14. Giày slip on nam Performance Max Cushioning (MaG=14) - Giá 2590000 (Trắng - w.jpg)
(14, 2, 6, 'GLSKE-W-41', 2700000, 2590000, 16), -- Trắng, Size 41
(14, 2, 7, 'GLSKE-W-42', 2700000, 2590000, 11), -- Trắng, Size 42

-- 15. Giày Da Công Sở Nam Da Bò Cao Cấp Antoni Fernando MS-AF3505 (MaG=15) - Giá 1850000 (Đen - den.jpg)
(15, 1, 6, 'GTAF-B-41', 2200000, 1850000, 9), -- Đen, Size 41
(15, 1, 7, 'GTAF-B-42', 2200000, 1850000, 7), -- Đen, Size 42

-- 16. Giày Da Công Sở Nam Da Bò Cao Cấp Antoni Fernando MS-AF3503 (MaG=16) - Giá 1620000 (Đen - den.jpg)
(16, 1, 5, 'GTAF2-B-40', 1800000, 1620000, 12), -- Đen, Size 40
(16, 1, 6, 'GTAF2-B-41', 1800000, 1620000, 10), -- Đen, Size 41

-- 17. GIÀY LOAFER NAM CÔNG SỞ ĐẾ PHÍP Antoni Fernando AF30725 (MaG=17) - Giá 1850000 (Đỏ - do.jpg)
(17, 4, 6, 'GLAF3-D-41', 1900000, 1850000, 5), -- Đỏ, Size 41
(17, 4, 7, 'GLAF3-D-42', 1900000, 1850000, 3), -- Đỏ, Size 42

-- 18. VANS CLASSIC SLIP-ON BLACK/WHITE (MaG=18) - Giá 1450000 (Đen - den.jpg)
(18, 1, 3, 'GLVS-B-38', 1600000, 1450000, 15), -- Đen, Size 38
(18, 1, 4, 'GLVS-B-39', 1600000, 1450000, 18), -- Đen, Size 39

-- 19. Giày boots nam cổ cao mũi tròn (MaG=19) - Giá 1699000 (Nâu - nau.jpg)
(19, 6, 5, 'BOOTURB-N-40', 1750000, 1699000, 10), -- Nâu, Size 40
(19, 6, 6, 'BOOTURB-N-41', 1750000, 1699000, 8),  -- Nâu, Size 41

-- 20. Giày boots nam cổ cao thắt dây Toronto (MaG=20) - Giá 13900000 (Nâu Đỏ - naudo.jpg)
(20, 10, 7, 'BOOTDSQ-ND-42', 19900000, 13900000, 4), -- Nâu Đỏ, Size 42
(20, 10, 8, 'BOOTDSQ-ND-43', 19900000, 13900000, 3), -- Nâu Đỏ, Size 43

-- 21. Giày boots nam cổ cao thắt dây Canadian (MaG=21) - Giá 23100000 (Đen - den.jpg)
(21, 1, 7, 'BOOTDSQ2-B-42', 33000000, 23100000, 2), -- Đen, Size 42
(21, 1, 8, 'BOOTDSQ2-B-43', 33000000, 23100000, 1), -- Đen, Size 43

-- 22. Giày boots nam cổ thấp mũi tròn Hiking (MaG=22) - Giá 18600000 (Nâu - nau.jpg)
(22, 6, 6, 'BOOTDSQ3-N-41', 26500000, 18600000, 5), -- Nâu, Size 41
(22, 6, 7, 'BOOTDSQ3-N-42', 26500000, 18600000, 4); -- Nâu, Size 42
-- =================================================================
-- III. DỮ LIỆU KHÁCH HÀNG VÀ ĐƠN HÀNG
-- =================================================================
-- 8. DỮ LIỆU CHUCVU
INSERT INTO CHUCVU (Ten, HeSo) VALUES
(N'Quản lý', 2.0),
(N'Nhân viên bán hàng', 1.2),
(N'Nhân viên kho', 1.0);
--9.NHÂN VIÊN
INSERT INTO NHANVIEN(Ten,MaCV,DienThoai,Email,MatKhau) VALUES
(N'Mỹ Duyên',1,'0123456789','duyen789@gmail.com','pass123'),
(N'Dương Thị Mỹ Thuận',2,'0231023548','thuan548@gmail.com','pass456'),
(N'Trần Huỳnh Sơn',3,'0863254125','son125@gmail.com','pass123'),
(N'Lê Ngọc Thanh',2,'0463254871','thanh871@gmail.com','pass456');

-- 10. DỮ LIỆU KHACHHANG (Mật khẩu nên được Hash, ở đây là ví dụ)
INSERT INTO KHACHHANG (Ten, DienThoai, Email, MatKhau) VALUES
(N'Lê Văn A', '0912345678', 'levana@email.com', 'pass123'),  -- MaKH = 1
(N'Trần Thị B', '0987654321', 'tranthib@email.com', 'pass456'); -- MaKH = 2

-- 11. DỮ LIỆU DIACHI
INSERT INTO DIACHI (MaKH, DiaChi, PhuongXa, QuanHuyen, Tinh, MacDinh) VALUES
(1, N'Bình Chánh', N'TP.LongXuyên', N'TP. Long Xuyên', N'An Giang', 1),
(2, N'Bình Thạnh', N'Bình Hòa', N'Châu Thành', N'An Giang', 1);

-- 12. DỮ LIỆU HOADON & CTHOADON 
---------------------------------------------------------------------

-- HÓA ĐƠN 1 (MaHD = 1) - Khách 1 (Nguyễn Văn Khách)
-- Mua MaBT=1 (2,490,000) và MaBT=13 (1,980,000).
-- TongTien = 2,490,000 + 1,980,000 = 4,470,000
--INSERT INTO HOADON (MaKH, Ngay, TongTien, TrangThai) VALUES
--(1, GETDATE(), 4470000, 3); -- Trạng thái: 3 (Hoàn thành)

-- Chi tiết cho Hóa đơn 1 (MaHD=1)
-- Item 1: Nike Pegasus Đen Size 40 (MaBT=1)
--INSERT INTO CTHOADON (MaHD, MaBT, DonGia, SoLuong, ThanhTien) VALUES
--(1, 1, 2490000, 1, 2490000);
-- Item 2: VANS SK8-HI Trắng Size 39 (MaBT=13)
--INSERT INTO CTHOADON (MaHD, MaBT, DonGia, SoLuong, ThanhTien) VALUES
--(1, 13, 1980000, 1, 1980000);


-- HÓA ĐƠN 2 (MaHD = 2) - Khách 2 (Trần Thị B)
-- Mua 2 đôi Giày Da Công Sở Antoni Fernando MS-AF3505 (MaBT=14)
-- TongTien = 2 x 1,850,000 = 3,700,000
--INSERT INTO HOADON (MaKH, Ngay, TongTien, TrangThai) VALUES(2, DATEADD(DAY, -5, GETDATE()), 3700000, 1); -- Trạng thái: 1 (Đã xác nhận)

-- Chi tiết cho Hóa đơn 2 (MaHD=2)
-- Item 1: Giày Da Công Sở Đen Size 41 (MaBT=14)
--INSERT INTO CTHOADON (MaHD, MaBT, DonGia, SoLuong, ThanhTien) VALUES(2, 14, 1850000, 2, 3700000);


-- HÓA ĐƠN 3 (MaHD = 3) - Khách 1 (Nguyễn Văn Khách)
-- Mua 1 đôi Giày boots nam cổ cao thắt dây Canadian (MaBT=16)
-- TongTien = 1 x 23,100,000 = 23,100,000
--INSERT INTO HOADON (MaKH, Ngay, TongTien, TrangThai) VALUES (1, GETDATE(), 23100000, 0); -- Trạng thái: 0 (Chờ xác nhận)

-- Chi tiết cho Hóa đơn 3 (MaHD=3)
-- Item 1: Giày boots Canadian Đen Size 42 (MaBT=16)
--INSERT INTO CTHOADON (MaHD, MaBT, DonGia, SoLuong, ThanhTien) VALUES(3, 16, 23100000, 1, 23100000);


-- =================================================================
-- IV. DỮ LIỆU ĐÁNH GIÁ (REVIEWS)
-- =================================================================
--14. Khach đánh giá
-- Khách A (MaKH=1) đánh giá Nike Pegasus 40 (MaG=1)
--INSERT INTO DANHGIA (MaG, MaKH, Diem, NoiDung) VALUES
--(1, 1, 5, N'Giày rất nhẹ và êm, chạy bộ cực kỳ thoải mái. Thiết kế đẹp và năng động.');

-- Khách B (MaKH=2) đánh giá Oxford Da Bò (MaG=2)
--INSERT INTO DANHGIA (MaG, MaKH, Diem, NoiDung) VALUES
--(2, 2, 4, N'Giày da chất lượng tốt, form chuẩn. Giao hàng hơi lâu một chút.');
--GO

