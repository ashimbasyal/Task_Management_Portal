import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface DashboardFilter {
  sprint?: string;
  priority?: string;
  status?: string;
  department?: string;
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
  priorityDistribution: ChartItem[];
  statusDistribution: ChartItem[];
  departmentDistribution: DepartmentItem[];
  assignedUserCounts: AssignedUserItem[];
  sprintDistribution: SprintItem[];
  pendingTasks: number;
  completedTasks: number;
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/dashboard`;

  get(filter?: DashboardFilter) {
    let params = new HttpParams();
    if (filter) {
      if (filter.sprint) params = params.set('sprint', filter.sprint);
      if (filter.priority) params = params.set('priority', filter.priority);
      if (filter.status) params = params.set('status', filter.status);
      if (filter.department) params = params.set('department', filter.department);
    }
    return this.http.get<DashboardData>(this.API, { params });
  }
}
