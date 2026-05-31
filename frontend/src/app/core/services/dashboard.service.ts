import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface DashboardFilter {
  sprintName?: string;
  priorityId?: number;
  statusId?: number;
  departmentId?: number;
}

export interface ChartItem {
  label: string;
  count: number;
  pct: number;
  color: string;
}

export interface DepartmentItem {
  name: string;
  tasks: number;
  pct: number;
  color: string;
}

export interface AssignedUserItem {
  userName: string;
  taskCount: number;
}

export interface SprintItem {
  sprintName: string;
  taskCount: number;
  completedCount: number;
}

export interface DashboardData {
  totalTasks: number;
  inProgress: number;
  completed: number;
  activeSprints: number;
  pendingTasks: number;
  priorityDistribution: ChartItem[];
  statusDistribution: ChartItem[];
  departmentDistribution: DepartmentItem[];
  assignedUserCounts: AssignedUserItem[];
  sprintDistribution: SprintItem[];
}

interface BackendSprintTaskCount {
  sprintName: string;
  taskCount: number;
}

interface BackendStatusDistribution {
  statusName: string;
  count: number;
}

interface BackendPriorityDistribution {
  priorityName: string;
  count: number;
}

interface BackendDepartmentDistribution {
  departmentName: string;
  count: number;
}

interface BackendUserTaskCount {
  userId: string;
  userName: string;
  taskCount: number;
}

interface BackendDashboardResponse {
  totalTasks: number;
  inProgressTasks: number;
  completedTasks: number;
  pendingTasks: number;
  sprintWiseTaskCounts: BackendSprintTaskCount[];
  statusWiseDistribution: BackendStatusDistribution[];
  priorityWiseDistribution: BackendPriorityDistribution[];
  departmentWiseDistribution: BackendDepartmentDistribution[];
  assignedUserTaskCounts: BackendUserTaskCount[];
}

const PRIORITY_COLORS: Record<string, string> = {
  High: '#ef4444',
  Medium: '#f59e0b',
  Low: '#3b82f6',
};

const STATUS_COLORS: Record<string, string> = {
  Open: '#94a3b8',
  'In Progress': '#f59e0b',
  Completed: '#22c55e',
  'On Hold': '#ef4444',
  Pending: '#a855f7',
};

const DEPT_COLORS = ['#3b82f6', '#8b5cf6', '#ec4899', '#14b8a6', '#f97316'];

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/dashboard`;

  get(filter?: DashboardFilter) {
    let params = new HttpParams();
    if (filter) {
      if (filter.sprintName) params = params.set('SprintName', filter.sprintName);
      if (filter.priorityId) params = params.set('PriorityId', filter.priorityId);
      if (filter.statusId) params = params.set('StatusId', filter.statusId);
      if (filter.departmentId) params = params.set('DepartmentId', filter.departmentId);
    }
    return this.http.get<any>(this.API, { params }).pipe(
      map(res => this.toDashboardData(res.data as BackendDashboardResponse))
    );
  }

  filterDashboard(filter: DashboardFilter) {
    return this.http.post<any>(`${this.API}/filter`, filter).pipe(
      map(res => this.toDashboardData(res.data as BackendDashboardResponse))
    );
  }

  private toDashboardData(d: BackendDashboardResponse): DashboardData {
    const total = d.totalTasks || 0;

    const priorityTotal = d.priorityWiseDistribution?.reduce((s, i) => s + i.count, 0) || 0;
    const priorityDistribution: ChartItem[] = (d.priorityWiseDistribution || []).map(i => ({
      label: i.priorityName,
      count: i.count,
      pct: priorityTotal > 0 ? Math.round(i.count / priorityTotal * 100) : 0,
      color: PRIORITY_COLORS[i.priorityName] || '#94a3b8',
    }));

    const statusTotal = d.statusWiseDistribution?.reduce((s, i) => s + i.count, 0) || 0;
    const statusDistribution: ChartItem[] = (d.statusWiseDistribution || []).map(i => ({
      label: i.statusName,
      count: i.count,
      pct: statusTotal > 0 ? Math.round(i.count / statusTotal * 100) : 0,
      color: STATUS_COLORS[i.statusName] || '#94a3b8',
    }));

    const deptTotal = d.departmentWiseDistribution?.reduce((s, i) => s + i.count, 0) || 0;
    const departmentDistribution: DepartmentItem[] = (d.departmentWiseDistribution || []).map((i, idx) => ({
      name: i.departmentName,
      tasks: i.count,
      pct: deptTotal > 0 ? Math.round(i.count / deptTotal * 100) : 0,
      color: DEPT_COLORS[idx % DEPT_COLORS.length],
    }));

    const sprintDistribution: SprintItem[] = (d.sprintWiseTaskCounts || []).map(i => ({
      sprintName: i.sprintName,
      taskCount: i.taskCount,
      completedCount: 0,
    }));

    return {
      totalTasks: total,
      inProgress: d.inProgressTasks || 0,
      completed: d.completedTasks || 0,
      activeSprints: d.sprintWiseTaskCounts?.length || 0,
      pendingTasks: d.pendingTasks || 0,
      priorityDistribution,
      statusDistribution,
      departmentDistribution,
      assignedUserCounts: (d.assignedUserTaskCounts || []).map(i => ({
        userName: i.userName || i.userId,
        taskCount: i.taskCount,
      })),
      sprintDistribution,
    };
  }
}
