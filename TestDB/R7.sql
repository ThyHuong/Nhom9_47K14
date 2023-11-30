--Nhà cung cấp
--Mã NCC mới
create function fNewMaNCC()
returns varchar(8)
as
begin
	declare @NewMaNCC varchar(8), @maxMaNCC varchar(8), @num int
	set @maxMaNCC = (select MAX(MaNCC) from NHACUNGCAP)
	set @num = right(@maxMaNCC,len(@maxMaNCC)-3) + 1
	set @NewMaNCC = CONCAT('NCC',REPLICATE('0',len(@maxMaNCC) - 3 - len(@num)),@num)
	return @NewMaNCC
end
go

--Khách hàng
--Mã KH mới
create function fNewMaKH()
returns varchar(8)
as
begin
	declare @NewMaKH varchar(8), @maxMaKH varchar(8), @num int
	set @maxMaKH = (select MAX(MaKH) from KHACHHANG)
	set @num = right(@maxMaKH,len(@maxMaKH)-2) + 1
	set @NewMaKH = CONCAT('KH',REPLICATE('0',len(@maxMaKH) - 2 - len(@num)),@num)
	return @NewMaKH
end
go
select dbo.fNewMaKH()
go

--Hàng
--Mã hàng mới
create function fNewMaH()
returns varchar(8)
as
begin
	declare @NewMaH varchar(8), @maxMaH varchar(8), @num varchar(8)
	set @maxMaH = (select MAX(MaH) from HANG)
	set @num = right(@maxMaH,len(@maxMaH)-2)+1
	set @NewMaH = CONCAT('SP',REPLICATE('0',len(@maxMaH) - 2 - len(@num)),@num)
	return @NewMaH
end
go

--Hoá đơn
--Mã hoá đơn mới
create function fNewMaHD()
returns varchar(8)
as
begin
	declare @NewMaHD varchar(8), @maxMaHD varchar(8), @num varchar(8)
	set @maxMaHD = (select MAX(MaHD) from Ban)
	if @maxMaHD is null
	begin
		set @NewMaHD = 'HD000001'
	end
	else
	begin
		set @num = right(@maxMaHD,len(@maxMaHD)-2)+1
		set @NewMaHD = CONCAT('HD',REPLICATE('0',len(@maxMaHD) - 2 - len(@num)),@num)
	end
	return @NewMaHD
end
go
--Tạo mã nhập kho mới
create function fNewMaNhap()
returns varchar(10)
as
begin
	declare @NewMaNhap varchar(10), @maxMaNhap varchar(10), @num int
	set @maxMaNhap = (select MAX(MaNhap) from NHAP)
	if @maxMaNhap is null
	begin
		set @NewMaNhap = 'NH000001'
	end
	else
	begin
		set @num = right(@maxMaNhap,len(@maxMaNhap)-2)+1
		set @NewMaNhap = CONCAT('NH',REPLICATE('0',len(@maxMaNhap) - 2 - len(@num)),@num)
	end
	return @NewMaNhap
end
go

--Xoá NCC
create trigger tgDelNCC
on NhaCungCap instead of delete
as
begin
	declare @MaNCC varchar(10)
	select @MaNCC = MaNCC from deleted
	update NhaCungCap set DiaChi = 1 where MaNCC = @MaNCC
end
go
--Xoá hàng hoá
create trigger tgDelHang
on HANG instead of delete
as
begin
	declare @MaH varchar(10)
	select @MaH = MaH from deleted
	update HANG set SoLuong = -1 where MaH=@MaH
end
go
--Kiểm tra có MaHD trong Ban_CHITIET hay không
create function fCheckHD_CTHD (@MaHD varchar(10))
returns int
as
begin
	declare @ret int
	if exists (select * from Ban_CHITIET where MaHD = @MaHD)
	begin
		set @ret = 1
	end
	else
	begin
		set @ret = 0
	end
	return @ret
end
go
--Xoá hoá đơn
create trigger tgDelCTHD
on Ban_CHITIET after delete
as
begin
	declare @MaHD varchar(10), @MaH varchar(10), @SoLuong int
	select @MaHD=MaHD, @MaH=MaH, @SoLuong=SoLuong from deleted
	update HANG set SoLuong = SoLuong + @SoLuong where MaH = @MaH
	delete Ban where MaHD = @MaHD
end
go
--Kiểm tra có MaNhap trong NHAP_CHITIET hay không
create function fCheckNK_CTNK (@MaNhap varchar(10))
returns int
as
begin
	declare @ret int
	if exists (select * from NHAP_CHITIET where MaNhap = @MaNhap)
	begin
		set @ret = 1
	end
	else
	begin
		set @ret = 0
	end
	return @ret
end
go
--Xoá phiếu nhập kho
create trigger tgDelCTNK
on NHAP_CHITIET after delete
as
begin
	declare @MaNhap varchar(10), @MaH varchar(10), @SoLuong int
	select @MaNhap=MaNhap, @MaH=MaH, @SoLuong=SoLuong from deleted
	update HANG set SoLuong = SoLuong - @SoLuong where MaH = @MaH
	delete NHAP where MaNhap = @MaNhap
end
go

--Xoá khách hàng nếu có hoá đơn của KH, tự động đổi MaKH của hoá đơn thành Mã khách lẻ
create trigger tgDelKH
on Ban instead of delete
as
begin
	declare @MaKH varchar(10)
	select @MaKH=MaKH from deleted
	update Ban set MaKH='KH000000' where MaKH = @MaKH
	delete KHACHHANG where MaKH = @MaKH
end
go

--Kiểm tra có MaKH trong Ban hay không
create function fCheckKH_HD (@MaKH varchar(10))
returns int
as
begin
	declare @ret int
	if exists (select * from Ban where MaKH = @MaKH)
	begin
		set @ret = 1
	end
	else
	begin
		set @ret = 0
	end
	return @ret
end
go


--Kiểm tra đăng nhập 
create function fCheckLogin (@UserName varchar(100), @Pass varchar(100))
returns int
as
begin
	declare @result bit
	set @Pass = CONVERT(varbinary(32), HashBytes('MD5', @Pass), 2)

	if exists (select * from TAIKHOAN where UserName=@UserName and HASHPASSWORD=@Pass)
	begin
		--Đăng nhập đúng trả về kết quả = 1
		set @result = 1
	end
	else
	begin
		--Đăng nhập sai trả về kết quả = 0
		set @result = 0
	end
	set @result=1
	return @result
end
go

--Sau khi thêm mới bản ghi chi tiết hoá đơn thì tính tổng tiền và thanh toán, cập nhật số lượng hàng đã có
create trigger tgInsCTHD
on Ban_CHITIET after insert
as
begin
	declare @MaHD varchar(10), @MaH varchar(10), @SoLuong int, @TongTien numeric(12,0), @ThanhToan numeric(12,0)
	select @MaHD = MaHD, @MaH = MaH, @SoLuong = SoLuong from inserted
	set @TongTien = (select sum(ThanhTien) from Ban_CHITIET where MaHD = @MaHD)
	set @ThanhToan = @TongTien - (@TongTien * (select GiamGia from Ban where MaHD = @MaHD))

	update HANG set SoLuong = SoLuong - @SoLuong where MaH = @MaH
	update Ban set TongTien = @TongTien, ThanhToan = @ThanhToan where MaHD = @MaHD
end
go
--Sau khi thêm mới bản ghi chi tiết phiếu nhập kho thì tính tổng tiền
create trigger tgInsCTpNK
on NHAP_CHITIET after insert
as
begin
	declare @MaNhap varchar(10), @MaH varchar(10), @TongTien money
	select @MaNhap = MaNhap, @MaH = MaH, @SoLuong = SoLuong from inserted
	set @TongTien = (select sum(ThanhTien) from NHAP_CHITIET where MaNhap = @MaNhap)
	update NHAP 
	set TongTien = @TongTien, ThanhToan = @ThanhToan
	where MaNhap = @MaNhap
end