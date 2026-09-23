import { useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { JobCard } from '../components/jobs/JobCard'
import { EnumSelect, Field } from '../components/ui/form'
import { Alert, EmptyState, Loading } from '../components/ui/feedback'
import { jobService } from '../features/jobs/job.service'
import { useAsync } from '../lib/useAsync'
import { employmentTypeLabels, workplaceTypeLabels, type EmploymentType, type WorkplaceType } from '../types/api'

export function JobsPage() {
  const [params, setParams] = useSearchParams()
  const keyword = params.get('keyword') ?? ''
  const location = params.get('location') ?? ''
  const employmentType = params.get('employmentType') === null ? '' : Number(params.get('employmentType')) as EmploymentType
  const workplaceType = params.get('workplaceType') === null ? '' : Number(params.get('workplaceType')) as WorkplaceType

  const [keywordDraft, setKeywordDraft] = useState(keyword)
  const [locationDraft, setLocationDraft] = useState(location)

  const jobs = useAsync(
    () => jobService.search({ keyword, location, employmentType, workplaceType }),
    [keyword, location, employmentType, workplaceType],
  )

  const update = (changes: Record<string, string>) => {
    const next = new URLSearchParams(params)
    for (const [key, value] of Object.entries(changes)) {
      if (value === '') next.delete(key)
      else next.set(key, value)
    }
    setParams(next)
  }

  return (
    <section className="page section">
      <p className="eyebrow">KHÁM PHÁ CƠ HỘI</p>
      <h1>Tìm công việc tiếp theo của bạn</h1>

      <div className="filter-bar">
        <input
          value={keywordDraft}
          onChange={event => setKeywordDraft(event.target.value)}
          onKeyDown={event => { if (event.key === 'Enter') update({ keyword: keywordDraft.trim() }) }}
          placeholder="Tìm theo vị trí hoặc mô tả công việc"
        />
        <input
          value={locationDraft}
          onChange={event => setLocationDraft(event.target.value)}
          onKeyDown={event => { if (event.key === 'Enter') update({ location: locationDraft.trim() }) }}
          placeholder="Tỉnh / Thành phố"
        />
        <button
          type="button"
          className="button"
          onClick={() => update({ keyword: keywordDraft.trim(), location: locationDraft.trim() })}
        >
          Tìm kiếm
        </button>
      </div>

      <div className="content-grid">
        <aside className="filters">
          <b>Bộ lọc</b>
          <Field label="Hình thức làm việc">
            <EnumSelect
              labels={employmentTypeLabels}
              value={employmentType}
              emptyLabel="Tất cả"
              onChange={value => update({ employmentType: value === '' ? '' : String(value) })}
            />
          </Field>
          <Field label="Nơi làm việc">
            <EnumSelect
              labels={workplaceTypeLabels}
              value={workplaceType}
              emptyLabel="Tất cả"
              onChange={value => update({ workplaceType: value === '' ? '' : String(value) })}
            />
          </Field>
          <button
            type="button"
            className="outline-button"
            onClick={() => { setKeywordDraft(''); setLocationDraft(''); setParams(new URLSearchParams()) }}
          >
            Xoá bộ lọc
          </button>
        </aside>

        <div>
          {jobs.loading ? <Loading /> : null}
          <Alert tone="danger">{jobs.error}</Alert>
          {jobs.data ? <p className="result-count">Có {jobs.data.length} việc làm phù hợp</p> : null}
          {jobs.data?.length === 0
            ? <EmptyState title="Không tìm thấy việc làm phù hợp" description="Thử bỏ bớt bộ lọc hoặc dùng từ khoá khác." />
            : <div className="job-list">{jobs.data?.map(job => <JobCard key={job.id} job={job} />)}</div>}
        </div>
      </div>
    </section>
  )
}
