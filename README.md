# BÁO CÁO BÀI TẬP / ĐỒ ÁN

## THÔNG TIN SINH VIÊN
- **Họ và tên:** Trần Nhật Đông
- **Mã số sinh viên:** [24810320177]
- **Lớp:** D19QTANM1
- **Tên môn học:** Lập trình C# / Windows Forms
- **Tên bài tập:** Bài 4.6, 4.7, 4.8: BindingSource & xử lý File, khung ứng dụng MDI, lập trình bất đồng bộ & ProgressBar

---

## KẾT QUẢ THỰC HÀNH

### Bài tập 4.6: Quản lý dữ liệu với BindingSource & xử lý File (Avatar / CSV)

#### 1. Ảnh màn hình Giao diện chính
DataGridView hiển thị danh sách sinh viên; cụm nhập liệu (Mã SV, Họ tên, Tuổi, Giới tính, Avatar) liên kết 2 chiều với dòng đang chọn qua BindingSource.

![Giao diện chính](./screenshots/bai4_6_main_ui.png)

#### 2. Ảnh màn hình Chức năng thực thi / Kết quả
Thêm sinh viên mới SV006, nhập thông tin và gán ảnh đại diện: lưới và PictureBox tự đồng bộ theo dòng đang chọn.

![Thực thi chức năng](./screenshots/bai4_6_execution_result.png)

#### 3. Ảnh màn hình Kiểm tra lỗi (Validation)
Họ tên chứa chữ số, bấm Cập nhật thì chương trình báo lỗi.

![Kiểm tra lỗi](./screenshots/bai4_6_validation_error.png)

---

### Bài tập 4.7: Thiết kế khung ứng dụng MDI & quản lý đa cửa sổ

#### 1. Ảnh màn hình Giao diện chính
FormMain là MDI Parent với MenuStrip, ToolStrip, StatusStrip; hai Form con Quản lý Sinh viên và Quản lý Sản phẩm sắp xếp kiểu Cascade.

![Giao diện chính](./screenshots/bai4_7_main_ui.png)

#### 2. Ảnh màn hình Chức năng thực thi / Kết quả
Thêm sinh viên trên Form con: Form con truyền thông điệp "Thêm thành công..." lên StatusStrip của Form cha.

![Thực thi chức năng](./screenshots/bai4_7_execution_result.png)

ContextMenuStrip trên DataGridView của Form con (Xem chi tiết, Xóa dòng, Làm mới dữ liệu):

![Menu ngữ cảnh](./screenshots/bai4_7_context_menu.png)

#### 3. Ảnh màn hình Kiểm tra lỗi (Validation)
Mở lại Form Quản lý Sinh viên khi Form đã mở: chương trình không mở lặp mà chỉ kích hoạt Form cũ và báo trên StatusStrip.

![Kiểm tra lỗi](./screenshots/bai4_7_validation_error.png)

---

### Bài tập 4.8: Lập trình bất đồng bộ (async/await) & tiến trình ProgressBar

#### 1. Ảnh màn hình Giao diện chính
Giao diện sau khi nạp xong danh sách sinh viên.

![Giao diện chính](./screenshots/bai4_8_main_ui.png)

#### 2. Ảnh màn hình Chức năng thực thi / Kết quả
Đang nạp dữ liệu bất đồng bộ: ProgressBar và nhãn phần trăm trên Form con, đồng thời phần trăm hiển thị trên StatusStrip của Form cha.

![Thực thi chức năng](./screenshots/bai4_8_execution_result.png)

#### 3. Ảnh màn hình Kiểm tra lỗi (Validation)
Trong lúc nạp, nút "Nạp dữ liệu" bị khóa để tránh kích hoạt trùng (xem ảnh ở mục 2). Khi tiến trình kết thúc, chương trình mở lại nút và hiển thị thông báo hoàn tất.

![Kiểm tra lỗi](./screenshots/bai4_8_validation_error.png)
