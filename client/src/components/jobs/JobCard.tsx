import { Link } from 'react-router-dom'
import { formatRelative, formatSalaryRange, initials } from '../../lib/format'
import { employmentTypeLabels, workplaceTypeLabels, type JobPostingSummary } from '../../types/api'

export function JobCard({ job }: { job: JobPostingSummary }) {
  return (
    <article className="job-card">
      {job.companyLogoUrl
        ? <img className="company-mark" src={job.companyLogoUrl} alt={job.companyName} />
        : <div className="company-mark">{initials(job.companyName || job.title)}</div>}
      <div className="job-card-content">
        <div className="job-card-heading">
          <div>
            <Link to={`/jobs/${job.id}`}><h3>{job.title}</h3></Link>
            <p>
              {job.companyId
                ? <Link to={`/companies/${job.companyId}`}>{job.companyName || 'Doanh nghiệp'}</Link>
                : job.companyName}
            </p>
          </div>
        </div>
        <strong>{formatSalaryRange(job.minSalary, job.maxSalary, job.currency)}</strong>
        <div className="job-meta">
          <span>⌖ {job.location}</span>
          <span>{workplaceTypeLabels[job.workplaceType]}</span>
          <span>{employmentTypeLabels[job.employmentType]}</span>
          <span>{formatRelative(job.publishedAtUtc)}</span>
        </div>
        <div className="tags">{job.skills.slice(0, 6).map(skill => <span key={skill}>{skill}</span>)}</div>
      </div>
    </article>
  )
}
