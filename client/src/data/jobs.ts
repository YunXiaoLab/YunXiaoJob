export type Job = { id: string; title: string; company: string; location: string; salary: string; mode: string; tags: string[]; posted: string; description: string }

export const jobs: Job[] = [
  { id: '1', title: 'Frontend Developer (React)', company: 'GreenTech Vietnam', location: 'Hà Nội', salary: '18 – 28 triệu', mode: 'Hybrid', tags: ['React', 'TypeScript', 'CSS'], posted: 'Hôm nay', description: 'Xây dựng trải nghiệm tìm việc mượt mà, dễ tiếp cận cho hàng nghìn ứng viên.' },
  { id: '2', title: 'Chuyên viên Tuyển dụng', company: 'Mori Coffee', location: 'Hồ Chí Minh', salary: '12 – 18 triệu', mode: 'On-site', tags: ['Recruitment', 'ATS'], posted: '1 ngày trước', description: 'Đồng hành cùng các đội ngũ để tìm và phát triển nhân sự phù hợp.' },
  { id: '3', title: 'Product Designer', company: 'Nami Studio', location: 'Đà Nẵng', salary: '20 – 35 triệu', mode: 'Remote', tags: ['Figma', 'UX Research'], posted: '2 ngày trước', description: 'Thiết kế sản phẩm số có tác động tích cực đến hành trình nghề nghiệp.' },
]
