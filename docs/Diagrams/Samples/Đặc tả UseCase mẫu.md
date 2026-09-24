

|Tên|Start Registration|
|-|-|
|Mô tả chung|Cho phép người dùng khởi tạo quá trình đăng ký tài khoản. Hệ thống kiểm tra thông tin đăng ký, tạo một yêu cầu đăng ký tạm thời, sinh mã OTP và gửi mã xác thực đến email của người dùng.|
|Tác nhân|Guest (người dùng chưa đăng nhập)|
|Điều kiện tiền kích hoạt|Người dùng chưa có tài khoản trong hệ thống; email chưa có một yêu cầu đăng ký đang chờ xác nhận; thông tin đăng ký được cung cấp đầy đủ.|
|Điều kiện hậu kích hoạt|Một bản ghi PendingRegistration được tạo và lưu vào cơ sở dữ liệu với thời hạn 5 phút. OTP được sinh và lưu dưới dạng giá trị băm kèm salt. Mã OTP được gửi đến email của người dùng.|
|Luồng chính|1. Người dùng gửi yêu cầu đăng ký gồm email, họ tên, username và mật khẩu.<br />2. Hệ thống chuẩn hóa email bằng cách loại bỏ khoảng trắng và chuyển về chữ thường.<br />3. Hệ thống kiểm tra email đã tồn tại trong tài khoản hay chưa.<br />4. Hệ thống kiểm tra email có yêu cầu đăng ký đang chờ hay không.<br />5. Hệ thống sinh OTP gồm 6 chữ số và tạo giá trị băm kèm salt.<br />6. Hệ thống băm mật khẩu và tạo bản ghi PendingRegistration với thời gian hết hạn 5 phút.<br />7. Hệ thống lưu yêu cầu đăng ký vào cơ sở dữ liệu.<br />8. Hệ thống gửi OTP đến email của người dùng.<br />|
|Luồng thay thế|A1. Email đã tồn tại, hệ thống kết thúc ca sử dụng và trả về lỗi.<br />A2. Đã tồn tại yêu cầu đăng ký, hệ thống từ chối yêu cầu mới.<br />A3. Gửi email thất bại, hệ thống không hoàn tất được việc gửi OTP và yêu cầu đăng ký vẫn tồn tại để có thể xử lý theo cơ chế gửi lại OTP.|
|Mô tả bổ sung|OTP không được lưu trực tiếp mà được lưu dưới dạng hash và salt <br />OTP có hiệu lực trong 5 phút.|



