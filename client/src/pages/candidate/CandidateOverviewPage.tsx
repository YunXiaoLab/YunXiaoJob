import { Link } from 'react-router-dom'
import { Alert, ApplicationBadge, EmptyState, Loading, SectionCard, StatTile } from '../../components/ui/feedback'
import { useAuth } from '../../features/auth/AuthProvider'
import { candidateService } from '../../features/candidate/candidate.service'
import { formatDate } from '../../lib/format'
import { useAsync } from '../../lib/useAsync'
import { ApplicationStatus } from '../../types/api'

export function CandidateOverviewPage() {
  const { user } = useAuth()
  const profile = useAsync(() => candidateService.profile(), [])
  const applications = useAsync(() => candidateService.applications(), [])

  if (profile.loading || applications.loading) return <section className="page section"><Loading /></section>

  if (profile.error || !profile.data) {
    return (
      <section className="page section">
        <p className="eyebrow">TÀI KHOẢN</p>
        <h1>Xin chào, {user?.fullName}</h1>
        <EmptyState
          title="Tài khoản này không có hồ sơ ứng viên"
          description="Tài khoản doanh nghiệp được tạo bởi chủ sở hữu công ty nên không có hồ sơ ứng viên."
        >
          <Link className="button" to="/employer">Tới khu vực tuyển dụng →</Link>
        </EmptyState>
      </section>
    )
  }

  const value = profile.data
  const items = applications.data ?? []
  const closed: number[] = [ApplicationStatus.Hired, ApplicationStatus.Rejected, ApplicationStatus.Withdrawn]
  const active = items.filter(item => !closed.includes(item.application.status)).length
  const missing = [
    value.headline ? null : 'tiêu đề hồ sơ',
    value.resumes.length > 0 ? null : 'CV',
    value.skills.length > 0 ? null : 'kỹ năng',
    value.experiences.length > 0 ? null : 'kinh nghiệm',
  ].filter(Boolean) as string[]

  return (
    <section className="page section">
      <p className="eyebrow">ỨNG VIÊN</p>
      <h1>Xin chào, {user?.fullName}</h1>

      <div className="stat-row">
        <StatTile label="Đơn ứng tuyển" value={items.length} />
        <StatTile label="Đang theo dõi" value={active} hint="Chưa có kết quả cuối cùng" />
        <StatTile label="CV đã tải lên" value={value.resumes.length} />
        <StatTile label="Kỹ năng" value={value.skills.length} />
      </div>

      {missing.length > 0 ? (
        <Alert tone="warning">
          Hồ sơ của bạn còn thiếu: {missing.join(', ')}. <Link className="text-link" to="/candidate/profile">Hoàn thiện ngay →</Link>
        </Alert>
      ) : null}

      <div className="stack">
        <SectionCard
          title="Đơn ứng tuyển gần đây"
          actions={<Link className="text-link" to="/candidate/applications">Xem tất cả →</Link>}
        >
          {items.length === 0 ? (
            <EmptyState title="Chưa có đơn ứng tuyển nào" description="Bắt đầu tìm việc để nhận phản hồi từ nhà tuyển dụng.">
              <Link className="button" to="/jobs">Tìm việc làm →</Link>
            </EmptyState>
          ) : (
            <ul className="list">
              {items.slice(0, 5).map(item => (
                <li key={item.application.id}>
                  <div>
                    <b>{item.job?.title ?? 'Tin đã gỡ'}</b>
                    <small>{item.job?.companyName} · nộp ngày {formatDate(item.application.submittedAtUtc)}</small>
                  </div>
                  <ApplicationBadge status={item.application.status} />
                </li>
              ))}
            </ul>
          )}
        </SectionCard>

        <SectionCard title="Hồ sơ của bạn" actions={<Link className="text-link" to="/candidate/profile">Chỉnh sửa →</Link>}>
          <p className="lead">{value.headline ?? 'Chưa có tiêu đề hồ sơ.'}</p>
          <p className="prewrap">{value.summary ?? 'Chưa có giới thiệu bản thân.'}</p>
          <div className="tags">{value.skills.map(skill => <span key={skill.id}>{skill.name}</span>)}</div>
        </SectionCard>
      </div>
    </section>
  )
}
