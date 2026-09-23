import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Alert, EmptyState, Loading, SectionCard } from '../../components/ui/feedback'
import { Checkbox, EnumSelect, Field, FormGrid, asInput, numberOrNull, textOrNull } from '../../components/ui/form'
import { useToast } from '../../components/ui/ToastProvider'
import { candidateService } from '../../features/candidate/candidate.service'
import { errorMessage } from '../../lib/api'
import { useAsync } from '../../lib/useAsync'
import {
  EducationLevel, Gender, SkillProficiency, educationLevelLabels, employmentTypeLabels, genderLabels,
  skillProficiencyLabels, type CandidateProfile, type EmploymentType,
} from '../../types/api'

type EducationDraft = {
  schoolName: string; major: string; level: EducationLevel; startYear: string; endYear: string
  gpa: string; gpaScale: string; isCurrent: boolean; description: string
}
type ExperienceDraft = {
  companyName: string; jobTitle: string; employmentType: EmploymentType | ''; location: string
  startDate: string; endDate: string; isCurrent: boolean; description: string
}
type SkillDraft = { name: string; proficiency: SkillProficiency; yearsOfExperience: string; lastUsedYear: string }

const emptyEducation: EducationDraft = {
  schoolName: '', major: '', level: EducationLevel.Bachelor, startYear: '', endYear: '',
  gpa: '', gpaScale: '', isCurrent: false, description: '',
}
const emptyExperience: ExperienceDraft = {
  companyName: '', jobTitle: '', employmentType: '', location: '',
  startDate: '', endDate: '', isCurrent: false, description: '',
}
const emptySkill: SkillDraft = { name: '', proficiency: SkillProficiency.Intermediate, yearsOfExperience: '', lastUsedYear: '' }

export function CandidateProfilePage() {
  const profile = useAsync(() => candidateService.profile(), [])
  const { notify } = useToast()

  if (profile.loading) return <section className="page section"><Loading /></section>
  if (profile.error || !profile.data) {
    return (
      <section className="page section">
        <EmptyState
          title="Chưa có hồ sơ ứng viên"
          description={`${profile.error ?? ''} Tài khoản doanh nghiệp không có hồ sơ ứng viên.`}
        >
          <Link className="button" to="/employer">Tới khu vực tuyển dụng →</Link>
        </EmptyState>
      </section>
    )
  }

  return (
    <section className="page section">
      <p className="eyebrow">HỒ SƠ ỨNG VIÊN</p>
      <h1>Hoàn thiện hồ sơ của bạn</h1>
      <p className="lead">Hồ sơ đầy đủ giúp nhà tuyển dụng đánh giá nhanh và chính xác hơn.</p>

      <div className="stack">
        <BasicSection profile={profile.data} onSaved={value => { profile.setData(value); notify('Đã lưu thông tin cá nhân.') }} />
        <ResumeSection profile={profile.data} onChanged={() => { profile.reload(); notify('Đã thêm CV.') }} />
        <EducationSection profile={profile.data} onSaved={() => { profile.reload(); notify('Đã lưu học vấn.') }} />
        <ExperienceSection profile={profile.data} onSaved={() => { profile.reload(); notify('Đã lưu kinh nghiệm.') }} />
        <SkillSection profile={profile.data} onSaved={() => { profile.reload(); notify('Đã lưu kỹ năng.') }} />
      </div>
    </section>
  )
}

function BasicSection({ profile, onSaved }: { profile: CandidateProfile; onSaved: (value: CandidateProfile) => void }) {
  const [form, setForm] = useState({
    headline: asInput(profile.headline), summary: asInput(profile.summary), location: asInput(profile.location),
    yearsOfExperience: asInput(profile.yearsOfExperience), isSearchable: profile.isSearchable,
    avatarUrl: asInput(profile.avatarUrl), dateOfBirth: asInput(profile.dateOfBirth), gender: profile.gender as Gender,
    expectedMinSalary: asInput(profile.expectedMinSalary), expectedMaxSalary: asInput(profile.expectedMaxSalary),
    expectedSalaryCurrency: profile.expectedSalaryCurrency ?? 'VND',
    linkedInUrl: asInput(profile.linkedInUrl), gitHubUrl: asInput(profile.gitHubUrl), portfolioUrl: asInput(profile.portfolioUrl),
  })
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  const set = <K extends keyof typeof form>(key: K, value: (typeof form)[K]) => setForm(current => ({ ...current, [key]: value }))

  const save = async () => {
    setBusy(true)
    setError('')
    try {
      onSaved(await candidateService.updateProfile({
        headline: textOrNull(form.headline),
        summary: textOrNull(form.summary),
        location: textOrNull(form.location),
        yearsOfExperience: numberOrNull(form.yearsOfExperience),
        isSearchable: form.isSearchable,
        avatarUrl: textOrNull(form.avatarUrl),
        dateOfBirth: textOrNull(form.dateOfBirth),
        gender: form.gender,
        expectedMinSalary: numberOrNull(form.expectedMinSalary),
        expectedMaxSalary: numberOrNull(form.expectedMaxSalary),
        expectedSalaryCurrency: textOrNull(form.expectedSalaryCurrency),
        linkedInUrl: textOrNull(form.linkedInUrl),
        gitHubUrl: textOrNull(form.gitHubUrl),
        portfolioUrl: textOrNull(form.portfolioUrl),
      }))
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <SectionCard title="Thông tin cá nhân" description="Phần đầu tiên nhà tuyển dụng nhìn thấy.">
      <FormGrid>
        <Field label="Tiêu đề hồ sơ" wide>
          <input value={form.headline} onChange={event => set('headline', event.target.value)} placeholder="Frontend Developer · 3 năm kinh nghiệm" />
        </Field>
        <Field label="Giới thiệu bản thân" wide>
          <textarea rows={4} value={form.summary} onChange={event => set('summary', event.target.value)} />
        </Field>
        <Field label="Nơi sinh sống">
          <input value={form.location} onChange={event => set('location', event.target.value)} />
        </Field>
        <Field label="Số năm kinh nghiệm">
          <input type="number" min={0} value={form.yearsOfExperience} onChange={event => set('yearsOfExperience', event.target.value)} />
        </Field>
        <Field label="Ngày sinh">
          <input type="date" value={form.dateOfBirth} onChange={event => set('dateOfBirth', event.target.value)} />
        </Field>
        <Field label="Giới tính">
          <EnumSelect labels={genderLabels} value={form.gender} onChange={value => set('gender', (value === '' ? Gender.Unspecified : value) as Gender)} />
        </Field>
        <Field label="Lương mong muốn từ">
          <input type="number" min={0} value={form.expectedMinSalary} onChange={event => set('expectedMinSalary', event.target.value)} />
        </Field>
        <Field label="Lương mong muốn đến">
          <input type="number" min={0} value={form.expectedMaxSalary} onChange={event => set('expectedMaxSalary', event.target.value)} />
        </Field>
        <Field label="Đơn vị tiền tệ">
          <input value={form.expectedSalaryCurrency} onChange={event => set('expectedSalaryCurrency', event.target.value)} placeholder="VND" />
        </Field>
        <Field label="Ảnh đại diện (URL)">
          <input value={form.avatarUrl} onChange={event => set('avatarUrl', event.target.value)} placeholder="https://" />
        </Field>
        <Field label="LinkedIn">
          <input value={form.linkedInUrl} onChange={event => set('linkedInUrl', event.target.value)} placeholder="https://" />
        </Field>
        <Field label="GitHub">
          <input value={form.gitHubUrl} onChange={event => set('gitHubUrl', event.target.value)} placeholder="https://" />
        </Field>
        <Field label="Portfolio">
          <input value={form.portfolioUrl} onChange={event => set('portfolioUrl', event.target.value)} placeholder="https://" />
        </Field>
      </FormGrid>
      <Checkbox
        label="Cho phép nhà tuyển dụng tìm thấy hồ sơ của tôi"
        checked={form.isSearchable}
        onChange={value => set('isSearchable', value)}
      />
      <Alert tone="danger">{error}</Alert>
      <div className="form-actions">
        <button type="button" className="button" disabled={busy} onClick={save}>{busy ? 'Đang lưu…' : 'Lưu thông tin'}</button>
      </div>
    </SectionCard>
  )
}

function ResumeSection({ profile, onChanged }: { profile: CandidateProfile; onChanged: () => void }) {
  const [name, setName] = useState('')
  const [fileUrl, setFileUrl] = useState('')
  const [isDefault, setIsDefault] = useState(profile.resumes.length === 0)
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const add = async () => {
    setBusy(true)
    setError('')
    try {
      await candidateService.addResume(name.trim(), fileUrl.trim(), isDefault)
      setName('')
      setFileUrl('')
      onChanged()
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <SectionCard title="CV của bạn" description="Bạn cần ít nhất một CV để ứng tuyển.">
      {profile.resumes.length === 0
        ? <p className="muted">Chưa có CV nào.</p>
        : (
          <ul className="list">
            {profile.resumes.map(resume => (
              <li key={resume.id}>
                <div>
                  <b>{resume.name}</b>
                  {resume.isDefault ? <small> · mặc định</small> : null}
                </div>
                <a className="text-link" href={resume.fileUrl} target="_blank" rel="noreferrer">Mở CV ↗</a>
              </li>
            ))}
          </ul>
        )}
      <FormGrid>
        <Field label="Tên CV">
          <input value={name} onChange={event => setName(event.target.value)} placeholder="CV Frontend 2026" />
        </Field>
        <Field label="Đường dẫn tệp CV" hint="Dán liên kết PDF đã tải lên Drive, Dropbox…">
          <input value={fileUrl} onChange={event => setFileUrl(event.target.value)} placeholder="https://" />
        </Field>
      </FormGrid>
      <Checkbox label="Đặt làm CV mặc định" checked={isDefault} onChange={setIsDefault} />
      <Alert tone="danger">{error}</Alert>
      <div className="form-actions">
        <button type="button" className="button" disabled={busy || !name.trim() || !fileUrl.trim()} onClick={add}>
          {busy ? 'Đang thêm…' : 'Thêm CV'}
        </button>
      </div>
    </SectionCard>
  )
}

function EducationSection({ profile, onSaved }: { profile: CandidateProfile; onSaved: () => void }) {
  const [rows, setRows] = useState<EducationDraft[]>([])
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    setRows(profile.educations.map(item => ({
      schoolName: item.schoolName, major: asInput(item.major), level: item.level,
      startYear: asInput(item.startYear), endYear: asInput(item.endYear), gpa: asInput(item.gpa),
      gpaScale: asInput(item.gpaScale), isCurrent: item.isCurrent, description: asInput(item.description),
    })))
  }, [profile.educations])

  const update = (index: number, changes: Partial<EducationDraft>) =>
    setRows(current => current.map((row, position) => (position === index ? { ...row, ...changes } : row)))

  const save = async () => {
    setBusy(true)
    setError('')
    try {
      await candidateService.saveEducations(rows.map(row => ({
        schoolName: row.schoolName.trim(),
        major: textOrNull(row.major),
        level: row.level,
        startYear: numberOrNull(row.startYear),
        endYear: numberOrNull(row.endYear),
        gpa: numberOrNull(row.gpa),
        gpaScale: numberOrNull(row.gpaScale),
        isCurrent: row.isCurrent,
        description: textOrNull(row.description),
      })))
      onSaved()
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <SectionCard
      title="Học vấn"
      description="Danh sách được lưu thay thế toàn bộ, hãy giữ lại những mục bạn muốn hiển thị."
      actions={<button type="button" className="outline-button" onClick={() => setRows(current => [...current, { ...emptyEducation }])}>+ Thêm mục</button>}
    >
      {rows.length === 0 ? <p className="muted">Chưa có thông tin học vấn.</p> : null}
      {rows.map((row, index) => (
        <div className="repeat-row" key={index}>
          <FormGrid>
            <Field label="Trường / cơ sở đào tạo">
              <input value={row.schoolName} onChange={event => update(index, { schoolName: event.target.value })} />
            </Field>
            <Field label="Chuyên ngành">
              <input value={row.major} onChange={event => update(index, { major: event.target.value })} />
            </Field>
            <Field label="Bậc học">
              <EnumSelect labels={educationLevelLabels} value={row.level} onChange={value => update(index, { level: (value === '' ? EducationLevel.Other : value) as EducationLevel })} />
            </Field>
            <Field label="Năm bắt đầu">
              <input type="number" value={row.startYear} onChange={event => update(index, { startYear: event.target.value })} />
            </Field>
            <Field label="Năm kết thúc">
              <input type="number" value={row.endYear} onChange={event => update(index, { endYear: event.target.value })} />
            </Field>
            <Field label="GPA">
              <input type="number" step="0.01" value={row.gpa} onChange={event => update(index, { gpa: event.target.value })} />
            </Field>
            <Field label="Thang điểm">
              <input type="number" step="0.1" value={row.gpaScale} onChange={event => update(index, { gpaScale: event.target.value })} placeholder="4" />
            </Field>
            <Field label="Mô tả" wide>
              <textarea rows={2} value={row.description} onChange={event => update(index, { description: event.target.value })} />
            </Field>
          </FormGrid>
          <div className="repeat-actions">
            <Checkbox label="Đang theo học" checked={row.isCurrent} onChange={value => update(index, { isCurrent: value })} />
            <button type="button" className="link-danger" onClick={() => setRows(current => current.filter((_, position) => position !== index))}>Xoá mục</button>
          </div>
        </div>
      ))}
      <Alert tone="danger">{error}</Alert>
      <div className="form-actions">
        <button type="button" className="button" disabled={busy} onClick={save}>{busy ? 'Đang lưu…' : 'Lưu học vấn'}</button>
      </div>
    </SectionCard>
  )
}

function ExperienceSection({ profile, onSaved }: { profile: CandidateProfile; onSaved: () => void }) {
  const [rows, setRows] = useState<ExperienceDraft[]>([])
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    setRows(profile.experiences.map(item => ({
      companyName: item.companyName, jobTitle: item.jobTitle,
      employmentType: item.employmentType === null ? '' : item.employmentType,
      location: asInput(item.location), startDate: asInput(item.startDate), endDate: asInput(item.endDate),
      isCurrent: item.isCurrent, description: asInput(item.description),
    })))
  }, [profile.experiences])

  const update = (index: number, changes: Partial<ExperienceDraft>) =>
    setRows(current => current.map((row, position) => (position === index ? { ...row, ...changes } : row)))

  const save = async () => {
    setBusy(true)
    setError('')
    try {
      await candidateService.saveExperiences(rows.map(row => ({
        companyName: row.companyName.trim(),
        jobTitle: row.jobTitle.trim(),
        employmentType: row.employmentType === '' ? null : row.employmentType,
        location: textOrNull(row.location),
        startDate: row.startDate,
        endDate: row.isCurrent ? null : textOrNull(row.endDate),
        isCurrent: row.isCurrent,
        description: textOrNull(row.description),
      })))
      onSaved()
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <SectionCard
      title="Kinh nghiệm làm việc"
      actions={<button type="button" className="outline-button" onClick={() => setRows(current => [...current, { ...emptyExperience }])}>+ Thêm mục</button>}
    >
      {rows.length === 0 ? <p className="muted">Chưa có kinh nghiệm nào.</p> : null}
      {rows.map((row, index) => (
        <div className="repeat-row" key={index}>
          <FormGrid>
            <Field label="Công ty">
              <input value={row.companyName} onChange={event => update(index, { companyName: event.target.value })} />
            </Field>
            <Field label="Vị trí">
              <input value={row.jobTitle} onChange={event => update(index, { jobTitle: event.target.value })} />
            </Field>
            <Field label="Hình thức">
              <EnumSelect labels={employmentTypeLabels} value={row.employmentType} emptyLabel="Không rõ" onChange={value => update(index, { employmentType: value as EmploymentType | '' })} />
            </Field>
            <Field label="Địa điểm">
              <input value={row.location} onChange={event => update(index, { location: event.target.value })} />
            </Field>
            <Field label="Bắt đầu">
              <input type="date" value={row.startDate} onChange={event => update(index, { startDate: event.target.value })} />
            </Field>
            <Field label="Kết thúc">
              <input type="date" value={row.endDate} disabled={row.isCurrent} onChange={event => update(index, { endDate: event.target.value })} />
            </Field>
            <Field label="Mô tả công việc" wide>
              <textarea rows={3} value={row.description} onChange={event => update(index, { description: event.target.value })} />
            </Field>
          </FormGrid>
          <div className="repeat-actions">
            <Checkbox label="Đang làm việc tại đây" checked={row.isCurrent} onChange={value => update(index, { isCurrent: value })} />
            <button type="button" className="link-danger" onClick={() => setRows(current => current.filter((_, position) => position !== index))}>Xoá mục</button>
          </div>
        </div>
      ))}
      <Alert tone="danger">{error}</Alert>
      <div className="form-actions">
        <button type="button" className="button" disabled={busy} onClick={save}>{busy ? 'Đang lưu…' : 'Lưu kinh nghiệm'}</button>
      </div>
    </SectionCard>
  )
}

function SkillSection({ profile, onSaved }: { profile: CandidateProfile; onSaved: () => void }) {
  const [rows, setRows] = useState<SkillDraft[]>([])
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    setRows(profile.skills.map(item => ({
      name: item.name, proficiency: item.proficiency,
      yearsOfExperience: asInput(item.yearsOfExperience), lastUsedYear: asInput(item.lastUsedYear),
    })))
  }, [profile.skills])

  const update = (index: number, changes: Partial<SkillDraft>) =>
    setRows(current => current.map((row, position) => (position === index ? { ...row, ...changes } : row)))

  const save = async () => {
    setBusy(true)
    setError('')
    try {
      await candidateService.saveSkills(rows.map(row => ({
        name: row.name.trim(),
        proficiency: row.proficiency,
        yearsOfExperience: numberOrNull(row.yearsOfExperience),
        lastUsedYear: numberOrNull(row.lastUsedYear),
      })))
      onSaved()
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <SectionCard
      title="Kỹ năng"
      actions={<button type="button" className="outline-button" onClick={() => setRows(current => [...current, { ...emptySkill }])}>+ Thêm kỹ năng</button>}
    >
      {rows.length === 0 ? <p className="muted">Chưa có kỹ năng nào.</p> : null}
      {rows.map((row, index) => (
        <div className="repeat-row" key={index}>
          <FormGrid columns={4}>
            <Field label="Tên kỹ năng">
              <input value={row.name} onChange={event => update(index, { name: event.target.value })} placeholder="React" />
            </Field>
            <Field label="Mức độ">
              <EnumSelect labels={skillProficiencyLabels} value={row.proficiency} onChange={value => update(index, { proficiency: (value === '' ? SkillProficiency.Beginner : value) as SkillProficiency })} />
            </Field>
            <Field label="Số năm">
              <input type="number" min={0} value={row.yearsOfExperience} onChange={event => update(index, { yearsOfExperience: event.target.value })} />
            </Field>
            <Field label="Dùng gần nhất (năm)">
              <input type="number" value={row.lastUsedYear} onChange={event => update(index, { lastUsedYear: event.target.value })} />
            </Field>
          </FormGrid>
          <div className="repeat-actions">
            <button type="button" className="link-danger" onClick={() => setRows(current => current.filter((_, position) => position !== index))}>Xoá kỹ năng</button>
          </div>
        </div>
      ))}
      <Alert tone="danger">{error}</Alert>
      <div className="form-actions">
        <button type="button" className="button" disabled={busy} onClick={save}>{busy ? 'Đang lưu…' : 'Lưu kỹ năng'}</button>
      </div>
    </SectionCard>
  )
}
