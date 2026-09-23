import { useState } from 'react'
import { NavLink, Outlet } from 'react-router-dom'
import { Alert, Badge, CompanyStatusBadge, Loading } from '../../components/ui/feedback'
import { Field, FormGrid, numberOrNull, textOrNull } from '../../components/ui/form'
import { useToast } from '../../components/ui/ToastProvider'
import { useCompanies } from '../../features/companies/CompanyProvider'
import { companyService } from '../../features/companies/company.service'
import { errorMessage } from '../../lib/api'
import { CompanyStatus, companyMemberRoleLabels } from '../../types/api'

export function EmployerLayout() {
  const { memberships, active, loading, error, select, reload } = useCompanies()

  if (loading) return <section className="page section"><Loading /></section>

  if (!active) {
    return (
      <section className="page section">
        <p className="eyebrow">NHÀ TUYỂN DỤNG</p>
        <h1>Tạo không gian tuyển dụng</h1>
        <Alert tone="danger">{error}</Alert>
        <p className="lead">
          Bạn chưa thuộc doanh nghiệp nào. Tạo hồ sơ doanh nghiệp bên dưới — hồ sơ cần được quản trị viên duyệt
          trước khi đăng tin tuyển dụng.
        </p>
        <CreateCompanyForm onCreated={reload} />
      </section>
    )
  }

  return (
    <section className="page section">
      <div className="workspace-head">
        <div>
          <p className="eyebrow">NHÀ TUYỂN DỤNG</p>
          <h1>{active.company.name}</h1>
          <div className="job-meta">
            <CompanyStatusBadge status={active.company.status} />
            <Badge tone="info">{companyMemberRoleLabels[active.role]}</Badge>
            {active.company.industry ? <span>{active.company.industry}</span> : null}
          </div>
        </div>
        {memberships.length > 1 ? (
          <Field label="Doanh nghiệp đang quản lý">
            <select value={active.company.id} onChange={event => select(event.target.value)}>
              {memberships.map(membership => (
                <option key={membership.company.id} value={membership.company.id}>{membership.company.name}</option>
              ))}
            </select>
          </Field>
        ) : null}
      </div>

      {active.company.status !== CompanyStatus.Active ? (
        <Alert tone="warning">
          Doanh nghiệp đang ở trạng thái “{active.company.status === CompanyStatus.PendingVerification ? 'chờ duyệt' : 'không hoạt động'}”.
          Một số thao tác như đăng tin hoặc tạo tài khoản nhân sự sẽ bị từ chối cho tới khi được duyệt.
        </Alert>
      ) : null}

      <nav className="sub-nav">
        <NavLink end to="/employer">Tổng quan</NavLink>
        <NavLink to="/employer/jobs">Tin tuyển dụng</NavLink>
        <NavLink to="/employer/company">Doanh nghiệp & nhân sự</NavLink>
      </nav>

      {/* Remount the child page when the active company changes so its form state is rebuilt. */}
      <div key={active.company.id}><Outlet /></div>
    </section>
  )
}

function CreateCompanyForm({ onCreated }: { onCreated: () => Promise<void> }) {
  const { notify } = useToast()
  const [form, setForm] = useState({ name: '', industry: '', website: '', logoUrl: '', address: '', employeeCount: '', description: '' })
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  const set = (key: keyof typeof form) => (value: string) => setForm(current => ({ ...current, [key]: value }))

  const submit = async () => {
    setBusy(true)
    setError('')
    try {
      await companyService.create({
        name: form.name.trim(),
        description: textOrNull(form.description),
        website: textOrNull(form.website),
        logoUrl: textOrNull(form.logoUrl),
        address: textOrNull(form.address),
        industry: textOrNull(form.industry),
        employeeCount: numberOrNull(form.employeeCount),
      })
      notify('Đã tạo doanh nghiệp, đang chờ quản trị viên duyệt.')
      await onCreated()
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="panel">
      <FormGrid>
        <Field label="Tên doanh nghiệp *">
          <input value={form.name} onChange={event => set('name')(event.target.value)} />
        </Field>
        <Field label="Ngành nghề">
          <input value={form.industry} onChange={event => set('industry')(event.target.value)} />
        </Field>
        <Field label="Website">
          <input value={form.website} onChange={event => set('website')(event.target.value)} placeholder="https://" />
        </Field>
        <Field label="Logo (URL)">
          <input value={form.logoUrl} onChange={event => set('logoUrl')(event.target.value)} placeholder="https://" />
        </Field>
        <Field label="Địa chỉ">
          <input value={form.address} onChange={event => set('address')(event.target.value)} />
        </Field>
        <Field label="Quy mô nhân sự">
          <input type="number" min={1} value={form.employeeCount} onChange={event => set('employeeCount')(event.target.value)} />
        </Field>
        <Field label="Giới thiệu" wide>
          <textarea rows={4} value={form.description} onChange={event => set('description')(event.target.value)} />
        </Field>
      </FormGrid>
      <Alert tone="danger">{error}</Alert>
      <div className="form-actions">
        <button type="button" className="button" disabled={busy || !form.name.trim()} onClick={submit}>
          {busy ? 'Đang tạo…' : 'Tạo doanh nghiệp →'}
        </button>
      </div>
    </div>
  )
}
