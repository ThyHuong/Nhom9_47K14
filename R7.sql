--Nhà cung cấp
--Mã NCC mới
create function fNewMaNCC()
returns varchar(8)
as
begin
	declare @NewMaNCC varchar(8), @maxMaNCC varchar(8), @num int
	set @maxMaNCC = (select MAX(MaNCC) from NhaCungCap)
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
	set @maxMaHD = (select MAX(MaHD) from BAN)
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
returns varchar(8)
as
begin
	declare @NewMaNhap varchar(8), @maxMaNhap varchar(8), @num int
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
on NHACUNGCAP instead of delete
as
begin
	declare @MaNCC varchar(8)
	select @MaNCC = MaNCC from deleted
	update NhaCungCap set TenNCC = 'deleted' where MaNCC = @MaNCC
end
go
--Xoá hàng hoá
create trigger tgDelHang
on HANG instead of delete
as
begin
	declare @MaH varchar(8)
	select @MaH = MaH from deleted
	update HANG set TenHang = 'deleted' where MaH=@MaH
end
go

--Xoá khách hàng
create trigger tgDelKH
on KHACHHANG instead of delete
as
begin
	declare @MaKH varchar(8)
	select @MaKH=MaKH from deleted
	update KHACHHANG
	set TenKH='deleted', SDT='0000000000'
	where MaKH = @MaKH
end
go

--Kiểm tra đăng nhập 
create function fCheckLogin (@UserName varchar(100), @Pass varchar(100))
returns int
as
begin
	declare @result int
	set @Pass = CONVERT(varchar(10), HashBytes('MD5', @Pass), 1)

	if exists (select * from TAIKHOAN where TenTK=@UserName and MatKhau=@Pass)
	begin
		--Đăng nhập đúng trả về kết quả = 1
		set @result = 1
	end
	else
	begin
		--Đăng nhập sai trả về kết quả = 0
		set @result = 0
	end
	return @result
end
go

--Sau khi thêm mới, xóa bản ghi chi tiết hoá đơn thì tính tổng tiền
create trigger tgInsCTHD
on BAN_CHITIET for insert
as
begin
	declare @MaHD varchar(8), @TongTien numeric(15,0)
	select @MaHD = MaHD from inserted
	set @TongTien = (select sum(ThanhTien) from BAN_CHITIET where MaHD = @MaHD)
	set @TongTien = @TongTien - (@TongTien * (select GiamGia from BAN where MaHD = @MaHD)/100)

	update BAN set TongTien = @TongTien where MaHD = @MaHD
end
go

create trigger tgdelCTHD
on BAN_CHITIET for delete
as
begin
	declare @MaHD varchar(8), @TongTien numeric(15,0)
	select @MaHD = MaHD from deleted
	set @TongTien = (select sum(ThanhTien) from BAN_CHITIET where MaHD = @MaHD)
	set @TongTien = @TongTien - (@TongTien * (select GiamGia from BAN where MaHD = @MaHD)/100)
	if @TongTien is null
	begin
		set @TongTien=0;
	end
	update BAN set TongTien = @TongTien where MaHD = @MaHD
end
go

--Sau khi thêm mới, xóa bản ghi chi tiết phiếu nhập kho thì tính tổng tiền
create trigger tgInsCTpNK
on NHAP_CHITIET for insert
as
begin
	declare @MaNhap varchar(8), @TongTien numeric(15,0)
	select @MaNhap = MaNhap from inserted
	set @TongTien = (select sum(ThanhTien) from NHAP_CHITIET where MaNhap = @MaNhap)
	set @TongTien = @TongTien - (@TongTien * (select GiamGia from NHAP where MaNhap = @MaNhap)/100)
	
	update NHAP set TongTien = @TongTien where MaNhap = @MaNhap
end
go

create trigger tgdelCTpNK
on NHAP_CHITIET for delete
as
begin
	declare @MaNhap varchar(8), @TongTien numeric(15,0)
	select @MaNhap = MaNhap from deleted
	set @TongTien = (select sum(ThanhTien) from NHAP_CHITIET where MaNhap = @MaNhap)
	set @TongTien = @TongTien - (@TongTien * (select GiamGia from NHAP where MaNhap = @MaNhap)/100)
	if @TongTien is null
	begin
		set @TongTien = 0
	end

	update NHAP set TongTien = @TongTien where MaNhap = @MaNhap
end
go
