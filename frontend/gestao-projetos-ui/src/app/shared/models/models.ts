export interface LoginResponse {
  token: string;
  username: string;
  role: string;
}

export interface UserResponse {
  id: number;
  username: string;
  email: string;
  role: string;
}

export interface ProjectResponse {
  id: number;
  name: string;
  description: string;
  isActive: boolean;
  createdAt: string;
}

export interface ProjectRequest {
  name: string;
  description: string;
  isActive: boolean;
}

export interface IssueResponse {
  id: number;
  projectId: number;
  projectName: string;
  title: string;
  description: string;
  issueType: 'Requirement' | 'Bug' | 'Improvement';
  implementationType: 'Angular' | 'CSharp' | 'SSIS' | 'Razor' | 'SqlServer' | 'Others';
  requestedBy: string;
  assignedToUserId?: number;
  assignedToUsername?: string;
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  status: 'Backlog' | 'Pending' | 'InAnalysis' | 'InDevelopment' | 'InTesting' | 'Done' | 'Cancelled';
  startDate?: string;
  endDate?: string;
  deadline?: string;
  createdAt: string;
  updatedAt: string;
}

export interface IssueRequest {
  projectId: number;
  title: string;
  description: string;
  issueType: 'Requirement' | 'Bug' | 'Improvement';
  implementationType: 'Angular' | 'CSharp' | 'SSIS' | 'Razor' | 'SqlServer' | 'Others';
  requestedBy: string;
  assignedToUserId?: number;
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  status: 'Backlog' | 'Pending' | 'InAnalysis' | 'InDevelopment' | 'InTesting' | 'Done' | 'Cancelled';
  startDate?: string;
  endDate?: string;
  deadline?: string;
}

export interface CommentResponse {
  id: number;
  issueId: number;
  userId: number;
  username: string;
  content: string;
  createdAt: string;
}

export interface AttachmentResponse {
  id: number;
  issueId: number;
  fileName: string;
  filePath: string;
  uploadedBy: string;
  uploadedAt: string;
}

export interface AuditLogResponse {
  id: number;
  issueId: number;
  userId: number;
  username: string;
  fieldChanged: string;
  oldValue?: string;
  newValue?: string;
  changedAt: string;
}

export interface TimeLogResponse {
  id: number;
  issueId: number;
  issueTitle: string;
  userId: number;
  username: string;
  loggedDate: string;
  hoursSpent: number;
  workDescription: string;
}

export interface TimeLogRequest {
  issueId: number;
  loggedDate: string;
  hoursSpent: number;
  workDescription: string;
}

export interface DashboardSummary {
  totalCompleted: number;
  totalInDevelopment: number;
  totalPending: number;
  totalCritical: number;
}

export interface DeveloperWorkload {
  developerId: number;
  developerName: string;
  issues: IssueResponse[];
}

export interface IssueDetailResponse {
  id: number;
  projectId: number;
  projectName: string;
  title: string;
  description: string;
  issueType: 'Requirement' | 'Bug' | 'Improvement';
  implementationType: 'Angular' | 'CSharp' | 'SSIS' | 'Razor' | 'SqlServer' | 'Others';
  requestedBy: string;
  assignedToUserId?: number;
  assignedToUsername?: string;
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  status: 'Backlog' | 'Pending' | 'InAnalysis' | 'InDevelopment' | 'InTesting' | 'Done' | 'Cancelled';
  startDate?: string;
  endDate?: string;
  deadline?: string;
  createdAt: string;
  updatedAt: string;
  totalHoursLogged: number;
  timeLogs: TimeLogResponse[];
  comments: CommentResponse[];
  attachments: AttachmentResponse[];
  auditLogs: AuditLogResponse[];
}
