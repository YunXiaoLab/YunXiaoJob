import { useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { Alert, EmptyState } from '../components/ui/feedback'
import { Field, FormGrid, numberOrNull, textOrNull } from '../components/ui/form'
import { companyService } from '../features/companies/company.service'
import { errorMessage } from '../lib/api'

export function EmployerLandingPage() {
  const [form, setForm] = useState({
    companyName: '', website: '', address: '', industry: '', description: '', employeeCount: '',
    contactFullName: '', contactEmail: '', contactPhoneNumber: '',
  })
  const [done, setDone] = useState(false)
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  const set = (key: keyof typeof form) => (value: string) => setForm(current => ({ ...current, [key]: value }))

  const submit = async (event: FormEvent) => {
    event.preventDefault()
    setBusy(true)
    setError('')
    try {
      await companyService.submitRegistration({
        companyName: form.companyName.trim(),
        website: textOrNull(form.website),
        address: textOrNull(form.address),
        industry: textOrNull(form.industry),
        description: textOrNull(form.description),
        employeeCount: numberOrNull(form.employeeCount),
        contactFullName: form.contactFullName.trim(),
        contactEmail: form.contactEmail.trim(),
        contactPhoneNumber: textOrNull(form.contactPhoneNumber),
      })
      setDone(true)
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <>
      <section className="hero-section">
        <div>
          <p className="eyebrow">DÀNH CHO DOANH NGHIỆP</p>
          <h1>Tuyển dụng <em>có tổ chức</em> từ ngày đầu tiên.</h1>
          <p className="hero-copy">
            Gửi đăng ký doanh nghiệp, đội ngũ YunXiaoJob sẽ xét duyệt và tạo tài khoản chủ sở hữu cho bạn.
            Sau đó bạn có thể đăng tin, phân quyền HR/Recruiter và theo dõi toàn bộ pipeline ứng viên.
          </p>
          <div className="hero-stats">
            <span><b>1</b> lần đăng ký</span>
            <span><b>3</b> vai trò: Owner · HR · Recruiter</span>
            <span><b>8</b> trạng thái hồ sơ</span>
          </div>
          <p className="hero-copy">Đã có tài khoản doanh nghiệp? <Link className="text-link" to="/employer">Vào khu vực tuyển dụng →</Link></p>
        </div>
        <aside className="hero-card">
          <p>Quy trình xét duyệt</p>
          <b>3 bước</b>
          <span>1. Gửi đăng ký doanh nghiệp</span>
          <span>2. Quản trị viên xét duyệt hồ sơ</span>
          <span>3. Nhận email tài khoản chủ sở hữu</span>
          <small>Thông tin đăng nhập được gửi tới email liên hệ.</small>
        </aside>
      </section>

      <section className="section">
        <div className="section-heading">
          <div>
            <p className="eyebrow">ĐĂNG KÝ</p>
            <h2>Thông tin doanh nghiệp</h2>
          </div>
        </div>

        {done ? (
          <EmptyState
            title="Đã gửi đăng ký thành công"
            description="Chúng tôi sẽ gửi email tới địa chỉ liên hệ ngay khi hồ sơ được duyệt."
          >
            <Link className="button" to="/">Về trang chủ →</Link>
          </EmptyState>
        ) : (
          <form className="panel" onSubmit={submit}>
            <FormGrid>
              <Field label="Tên doanh nghiệp *">
                <input required value={form.companyName} onChange={event => set('companyName')(event.target.value)} />
              </Field>
              <Field label="Ngành nghề">
                <input value={form.industry} onChange={event => set('industry')(event.target.value)} placeholder="Công nghệ, Bán lẻ…" />
              </Field>
              <Field label="Website">
                <input value={form.website} onChange={event => set('website')(event.target.value)} placeholder="https://" />
              </Field>
              <Field label="Quy mô nhân sự">
                <input type="number" min={1} value={form.employeeCount} onChange={event => set('employeeCount')(event.target.value)} />
              </Field>
              <Field label="Địa chỉ" wide>
                <input value={form.address} onChange={event => set('address')(event.target.value)} />
              </Field>
              <Field label="Giới thiệu doanh nghiệp" wide>
                <textarea rows={4} value={form.description} onChange={event => set('description')(event.target.value)} />
              </Field>
              <Field label="Người liên hệ *">
                <input required value={form.contactFullName} onChange={event => set('contactFullName')(event.target.value)} />
              </Field>
              <Field label="Email liên hệ *" hint="Tài khoản chủ sở hữu sẽ được tạo với email này.">
                <input required type="email" value={form.contactEmail} onChange={event => set('contactEmail')(event.target.value)} />
              </Field>
              <Field label="Số điện thoại">
                <input value={form.contactPhoneNumber} onChange={event => set('contactPhoneNumber')(event.target.value)} />
              </Field>
            </FormGrid>
            <Alert tone="danger">{error}</Alert>
            <div className="form-actions">
              <button className="button" disabled={busy}>{busy ? 'Đang gửi…' : 'Gửi đăng ký →'}</button>
            </div>
          </form>
        )}
      </section>
    </>
  )
}
