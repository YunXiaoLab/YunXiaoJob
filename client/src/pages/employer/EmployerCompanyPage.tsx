import { useState } from 'react'
import { Alert, Badge, Loading, SectionCard } from '../../components/ui/feedback'
import { EnumSelect, Field, FormGrid, asInput, numberOrNull, textOrNull } from '../../components/ui/form'
import { useToast } from '../../components/ui/ToastProvider'
import { useCompanies } from '../../features/companies/CompanyProvider'
import { companyService } from '../../features/companies/company.service'
import { errorMessage } from '../../lib/api'
import { useAsync } from '../../lib/useAsync'
import { CompanyMemberRole, companyMemberRoleLabels } from '../../types/api'

export function EmployerCompanyPage() {
  const { active, reload } = useCompanies()
  const companyId = active?.company.id ?? ''
  const members = useAsync(() => companyService.members(companyId), [companyId], Boolean(companyId))
  const { notify } = useToast()
  const isOwner = active?.role === CompanyMemberRole.Owner

  const [form, setForm] = useState({
    name: active?.company.name ?? '',
    industry: asInput(active?.company.industry),
    website: asInput(active?.company.website),
    logoUrl: asInput(active?.company.logoUrl),
    address: asInput(active?.company.address),
    employeeCount: asInput(active?.company.employeeCount),
    description: asInput(active?.company.description),
  })
  const [profileError, setProfileError] = useState('')
  const [savingProfile, setSavingProfile] = useState(false)

  const [staff, setStaff] = useState({ email: '', fullName: '', phoneNumber: '', role: CompanyMemberRole.HR as CompanyMemberRole })
  const [staffError, setStaffError] = useState('')
  const [creatingStaff, setCreatingStaff] = useState(false)

  const [memberUserId, setMemberUserId] = useState('')
  const [memberRole, setMemberRole] = useState<CompanyMemberRole>(CompanyMemberRole.Recruiter)
  const [memberError, setMemberError] = useState('')
  const [addingMember, setAddingMember] = useState(false)

  if (!active) return <Loading />

  const saveProfile = async () => {
    setSavingProfile(true)
    setProfileError('')
    try {
      await companyService.update(companyId, {
        name: form.name.trim(),
        description: textOrNull(form.description),
        website: textOrNull(form.website),
        logoUrl: textOrNull(form.logoUrl),
        address: textOrNull(form.address),
        industry: textOrNull(form.industry),
        employeeCount: numberOrNull(form.employeeCount),
      })
      notify('Đã cập nhật thông tin doanh nghiệp.')
      await reload()
    } catch (reason) {
      setProfileError(errorMessage(reason))
    } finally {
      setSavingProfile(false)
    }
  }

  const createStaff = async () => {
    setCreatingStaff(true)
    setStaffError('')
    try {
      const created = await companyService.createStaff(companyId, {
        email: staff.email.trim(),
        fullName: staff.fullName.trim(),
        phoneNumber: textOrNull(staff.phoneNumber),
        role: staff.role,
      })
      notify(`Đã tạo tài khoản ${created.email}, mật khẩu tạm được gửi qua email.`)
      setStaff({ email: '', fullName: '', phoneNumber: '', role: CompanyMemberRole.HR })
      members.reload()
    } catch (reason) {
      setStaffError(errorMessage(reason))
    } finally {
      setCreatingStaff(false)
    }
  }

  const addMember = async () => {
    setAddingMember(true)
    setMemberError('')
    try {
      await companyService.addMember(companyId, memberUserId.trim(), memberRole)
      notify('Đã thêm thành viên vào doanh nghiệp.')
      setMemberUserId('')
      members.reload()
    } catch (reason) {
      setMemberError(errorMessage(reason))
    } finally {
      setAddingMember(false)
    }
  }

  return (
    <div className="stack">
      <SectionCard title="Thông tin doanh nghiệp" description="Thông tin này hiển thị công khai trên trang doanh nghiệp.">
        <FormGrid>
          <Field label="Tên doanh nghiệp *">
            <input value={form.name} onChange={event => setForm({ ...form, name: event.target.value })} />
          </Field>
          <Field label="Ngành nghề">
            <input value={form.industry} onChange={event => setForm({ ...form, industry: event.target.value })} />
          </Field>
          <Field label="Website">
            <input value={form.website} onChange={event => setForm({ ...form, website: event.target.value })} placeholder="https://" />
          </Field>
          <Field label="Logo (URL)">
            <input value={form.logoUrl} onChange={event => setForm({ ...form, logoUrl: event.target.value })} placeholder="https://" />
          </Field>
          <Field label="Địa chỉ">
            <input value={form.address} onChange={event => setForm({ ...form, address: event.target.value })} />
          </Field>
          <Field label="Quy mô nhân sự">
            <input type="number" min={1} value={form.employeeCount} onChange={event => setForm({ ...form, employeeCount: event.target.value })} />
          </Field>
          <Field label="Giới thiệu" wide>
            <textarea rows={4} value={form.description} onChange={event => setForm({ ...form, description: event.target.value })} />
          </Field>
        </FormGrid>
        <Alert tone="danger">{profileError}</Alert>
        <div className="form-actions">
          <button type="button" className="button" disabled={savingProfile || !form.name.trim()} onClick={saveProfile}>
            {savingProfile ? 'Đang lưu…' : 'Lưu thông tin'}
          </button>
        </div>
      </SectionCard>

      <SectionCard title="Nhân sự tuyển dụng" description="Owner quản lý toàn quyền, HR xử lý hồ sơ, Recruiter phụ trách hồ sơ được giao.">
        {members.loading ? <Loading /> : null}
        <Alert tone="danger">{members.error}</Alert>
        {members.data && members.data.length > 0 ? (
          <div className="table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Họ tên</th>
                  <th>Email</th>
                  <th>Vai trò</th>
                  <th>Trạng thái</th>
                </tr>
              </thead>
              <tbody>
                {members.data.map(member => (
                  <tr key={member.id}>
                    <td>{member.fullName}</td>
                    <td>{member.email}</td>
                    <td>{companyMemberRoleLabels[member.role]}</td>
                    <td>
                      {member.isActive
                        ? <Badge tone="success">Đang hoạt động</Badge>
                        : <Badge tone="danger">Ngừng hoạt động</Badge>}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : <p className="muted">Chưa có thành viên nào.</p>}
      </SectionCard>

      {isOwner ? (
        <>
          <SectionCard title="Tạo tài khoản nhân sự" description="Hệ thống gửi mật khẩu tạm thời tới email của nhân sự. Doanh nghiệp phải ở trạng thái đang hoạt động.">
            <FormGrid>
              <Field label="Họ tên *">
                <input value={staff.fullName} onChange={event => setStaff({ ...staff, fullName: event.target.value })} />
              </Field>
              <Field label="Email *">
                <input type="email" value={staff.email} onChange={event => setStaff({ ...staff, email: event.target.value })} />
              </Field>
              <Field label="Số điện thoại">
                <input value={staff.phoneNumber} onChange={event => setStaff({ ...staff, phoneNumber: event.target.value })} />
              </Field>
              <Field label="Vai trò">
                <EnumSelect
                  labels={{ [CompanyMemberRole.HR]: companyMemberRoleLabels[CompanyMemberRole.HR], [CompanyMemberRole.Recruiter]: companyMemberRoleLabels[CompanyMemberRole.Recruiter] }}
                  value={staff.role}
                  onChange={value => setStaff({ ...staff, role: (value === '' ? CompanyMemberRole.HR : value) as CompanyMemberRole })}
                />
              </Field>
            </FormGrid>
            <Alert tone="danger">{staffError}</Alert>
            <div className="form-actions">
              <button
                type="button"
                className="button"
                disabled={creatingStaff || !staff.email.trim() || !staff.fullName.trim()}
                onClick={createStaff}
              >
                {creatingStaff ? 'Đang tạo…' : 'Tạo tài khoản'}
              </button>
            </div>
          </SectionCard>

          <SectionCard title="Thêm thành viên có sẵn" description="Dùng khi người đó đã có tài khoản YunXiaoJob.">
            <FormGrid>
              <Field label="Mã người dùng (User Id)" hint="Quản trị viên có thể tra cứu mã này trong danh sách người dùng.">
                <input value={memberUserId} onChange={event => setMemberUserId(event.target.value)} placeholder="00000000-0000-0000-0000-000000000000" />
              </Field>
              <Field label="Vai trò">
                <EnumSelect
                  labels={companyMemberRoleLabels}
                  value={memberRole}
                  onChange={value => setMemberRole((value === '' ? CompanyMemberRole.Recruiter : value) as CompanyMemberRole)}
                />
              </Field>
            </FormGrid>
            <Alert tone="danger">{memberError}</Alert>
            <div className="form-actions">
              <button type="button" className="button" disabled={addingMember || !memberUserId.trim()} onClick={addMember}>
                {addingMember ? 'Đang thêm…' : 'Thêm thành viên'}
              </button>
            </div>
          </SectionCard>
        </>
      ) : null}
    </div>
  )
}
