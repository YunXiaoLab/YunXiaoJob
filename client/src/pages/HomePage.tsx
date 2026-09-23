import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { JobCard } from '../components/jobs/JobCard'
import { Alert, EmptyState, Loading } from '../components/ui/feedback'
import { jobService } from '../features/jobs/job.service'
import { useAsync } from '../lib/useAsync'

export function HomePage() {
  const jobs = useAsync(() => jobService.search(), [])
  const [keyword, setKeyword] = useState('')
  const [location, setLocation] = useState('')
  const navigate = useNavigate()
  const featured = jobs.data?.slice(0, 6) ?? []

  const search = () => {
    const params = new URLSearchParams()
    if (keyword.trim()) params.set('keyword', keyword.trim())
    if (location.trim()) params.set('location', location.trim())
    navigate(`/jobs?${params.toString()}`)
  }

  return (
    <>
      <section className="hero-section">
        <div>
          <p className="eyebrow">TÌM ĐÚNG VIỆC · TUYỂN ĐÚNG NGƯỜI</p>
          <h1>Cơ hội tốt bắt đầu từ một <em>kết nối đúng.</em></h1>
          <p className="hero-copy">
            Khám phá công việc phù hợp hoặc xây dựng đội ngũ của bạn trên một nền tảng tuyển dụng đơn giản, minh bạch.
          </p>
          <div className="search-panel">
            <input
              value={keyword}
              onChange={event => setKeyword(event.target.value)}
              onKeyDown={event => { if (event.key === 'Enter') search() }}
              placeholder="Vị trí, kỹ năng hoặc tên công ty"
            />
            <input
              value={location}
              onChange={event => setLocation(event.target.value)}
              onKeyDown={event => { if (event.key === 'Enter') search() }}
              placeholder="Tỉnh / Thành phố"
            />
            <button type="button" className="button" onClick={search}>Tìm việc ngay →</button>
          </div>
          <div className="hero-stats">
            <span><b>{jobs.data?.length ?? 0}</b> việc làm đang tuyển</span>
            <span><b>{new Set(jobs.data?.map(job => job.companyId)).size}</b> doanh nghiệp</span>
            <span><b>100%</b> hồ sơ được theo dõi</span>
          </div>
        </div>
        <aside className="hero-card">
          <p>Việc làm mới nhất</p>
          <b>{featured[0]?.title ?? 'Đang cập nhật'}</b>
          <span>{featured[0] ? `${featured[0].companyName} · ${featured[0].location}` : 'Hãy quay lại sau ít phút'}</span>
          <div className="mini-chart"><i /><i /><i /><i /><i /><i /><i /></div>
          <small>Ứng tuyển nhanh với hồ sơ đã lưu</small>
        </aside>
      </section>

      <section className="section">
        <div className="section-heading">
          <div>
            <p className="eyebrow">CƠ HỘI MỚI</p>
            <h2>Việc làm nổi bật</h2>
          </div>
          <Link className="text-link" to="/jobs">Xem tất cả →</Link>
        </div>
        {jobs.loading ? <Loading /> : null}
        <Alert tone="danger">{jobs.error}</Alert>
        {!jobs.loading && featured.length === 0 && !jobs.error
          ? <EmptyState title="Chưa có tin tuyển dụng nào được đăng" description="Các tin tuyển dụng đã duyệt sẽ xuất hiện tại đây." />
          : <div className="job-grid">{featured.map(job => <JobCard key={job.id} job={job} />)}</div>}
      </section>

      <section className="dark-cta">
        <p className="eyebrow">DÀNH CHO DOANH NGHIỆP</p>
        <h2>Tuyển dụng có tổ chức.<br /><em>Không bỏ lỡ ứng viên tốt.</em></h2>
        <p>Đăng tin, quản lý pipeline và theo dõi hiệu quả tuyển dụng trong cùng một nơi.</p>
        <Link className="button" to="/for-employers">Bắt đầu tuyển dụng →</Link>
      </section>
    </>
  )
}
