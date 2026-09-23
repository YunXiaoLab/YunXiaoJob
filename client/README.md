# YunXiaoJob client

Giao diện web (React 19 + Vite + React Router) cho API YunXiaoJob. Không dùng thư viện UI ngoài — toàn bộ
giao diện nằm trong `src/index.css`.

## Chạy dự án

```bash
npm install
cp .env.example .env   # đổi VITE_API_BASE_URL nếu API không chạy ở http://localhost:5018
npm run dev            # http://localhost:5173
npm run build          # tsc -b && vite build
npm run lint
```

API phải cho phép origin của client (`ClientUrl` trong `src/YunXiaoJob.API/appsettings.json`).

## Cấu trúc

```text
src/
  types/api.ts        # bản sao TypeScript của DTO + enum của BE (enum được serialize dạng số)
  lib/api.ts          # fetch wrapper: gắn access token, tự refresh khi 401, bóc ApiResponse
  lib/useAsync.ts     # hook nạp dữ liệu (data/error/loading/reload)
  lib/format.ts       # định dạng ngày, lương, chuyển đổi input
  features/<miền>/    # service gọi API + context (auth, công ty đang chọn)
  components/ui/      # Field, EnumSelect, Alert, Badge, Modal, Toast, SectionCard…
  pages/              # trang công khai, ứng viên, nhà tuyển dụng, quản trị
```

## Bản đồ màn hình → endpoint

| Màn hình | Endpoint |
| --- | --- |
| `/` , `/jobs` | `GET /api/job-postings` |
| `/jobs/:id` | `GET /api/job-postings/{id}`, `POST /api/applications` |
| `/companies/:id` | `GET /api/companies/{id}` |
| `/login`, `/register` | `POST /api/auth/login`, `register`, `register/verify`, `register/resend`, `refresh` |
| `/forgot-password`, `/reset-password` | `POST /api/auth/forgot-password`, `reset-password` |
| `/for-employers` | `POST /api/company-registrations` |
| `/candidate` | `GET /api/candidate/profile`, `GET /api/candidate/applications` |
| `/candidate/profile` | `PUT /api/candidate/profile`, `POST /api/candidate/resumes`, `PUT .../educations`, `.../experiences`, `.../skills` |
| `/candidate/applications` | `GET /api/applications/{id}`, `POST /api/applications/{id}/withdraw`, `POST /api/applications/offers/{id}/respond` |
| `/employer` | `GET /api/companies/mine`, `GET /api/dashboard/company/{id}`, `POST /api/companies` |
| `/employer/jobs` | `GET /api/job-postings/company/{id}`, `POST`/`PUT /api/job-postings`, `POST /api/job-postings/{id}/submit` |
| `/employer/jobs/:jobId/applications` | `GET /api/applications/job-posting/{jobId}`, `PUT .../status`, `PUT .../recruiter`, `POST .../interviews`, `POST .../offers`, `POST .../interviews/{id}/complete` |
| `/employer/company` | `PUT /api/companies/{id}`, `GET`/`POST /api/companies/{id}/members`, `POST /api/companies/{id}/staff` |
| `/admin` | `GET /api/dashboard/platform`, `/api/admin/users`, `/api/admin/companies`, `/api/admin/job-postings`, `/api/admin/company-registrations` và các endpoint duyệt |
| `/notifications` | `GET /api/notifications`, `PUT /api/notifications/{id}/read` |
