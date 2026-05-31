import { Component, ChangeDetectorRef, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, takeUntil } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { SelectModule } from 'primeng/select';
import { AuditLogService, AuditLogDto } from '../../core/services/audit-log.service';
import { DashboardService, DashboardData, DashboardFilter } from '../../core/services/dashboard.service';
import { PriorityService, PriorityDto } from '../../core/services/priority.service';
import { StatusService, StatusDto } from '../../core/services/status.service';
import { DepartmentService, DepartmentDto } from '../../core/services/department.service';
import { SprintTaskService, SprintTaskDto } from '../../core/services/sprint-task.service';
import { relativeTime } from '../../shared/utils/relative-time';

interface SelectOption {
  label: string;
  value: any;
}

interface StatCard {
  label: string;
  value: number;
  icon: string;
  color: string;
  pct: number;
}

interface Activity {
  text: string;
  time: string;
  type: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, SelectModule],
  template: `
    <div class="dashboard" [class.loading]="dashboardLoading">
      @if (dashboardLoading) {
      <div class="dashboard-loader">
        <div class="loader-spinner"></div>
        <span>Updating dashboard...</span>
      </div>
      }
      <div class="welcome-section">
        <div>
          <h2 class="welcome-title">Welcome back, {{ fullName }}</h2>
          <p class="welcome-sub">Here's what's happening with your tasks today.</p>
        </div>
        <div class="role-badge">{{ role }}</div>
      </div>

      <div class="filter-bar">
        <p-select [options]="sprintOptions" placeholder="All Sprints" [(ngModel)]="filter.sprintName" (onChange)="applyFilter()" [showClear]="true" optionLabel="label" optionValue="value" styleClass="filter-select"></p-select>
        <p-select [options]="priorityOptions" placeholder="All Priorities" [(ngModel)]="filter.priorityId" (onChange)="applyFilter()" [showClear]="true" optionLabel="label" optionValue="value" styleClass="filter-select"></p-select>
        <p-select [options]="statusOptions" placeholder="All Statuses" [(ngModel)]="filter.statusId" (onChange)="applyFilter()" [showClear]="true" optionLabel="label" optionValue="value" styleClass="filter-select"></p-select>
        <p-select [options]="deptOptions" placeholder="All Departments" [(ngModel)]="filter.departmentId" (onChange)="applyFilter()" [showClear]="true" optionLabel="label" optionValue="value" styleClass="filter-select"></p-select>
      </div>

      <div class="stats-grid">
        <div class="stat-card" *ngFor="let card of stats">
          <div class="stat-icon" [style.background]="card.color + '12'" [style.color]="card.color">
            <i [class]="card.icon"></i>
          </div>
          <div class="stat-body">
            <span class="stat-value">{{ card.value }}</span>
            <span class="stat-label">{{ card.label }}</span>
            <div class="stat-bar">
              <div class="stat-bar-fill" [style.width.%]="card.pct" [style.background]="card.color"></div>
            </div>
          </div>
        </div>
      </div>

      <div class="grid-2col">
        <div class="card">
          <div class="card-header">
            <h3>Priority Distribution</h3>
            <span class="card-badge">{{ totalTasks }} tasks</span>
          </div>
          <div class="priority-chart">
            <div class="priority-group" *ngFor="let item of priorityData">
              <div class="priority-row">
                <span class="priority-badge" [style.background]="item.color + '18'" [style.color]="item.color">{{ item.label || 'Unknown' }}</span>
                <span class="priority-value">{{ item.count }}</span>
              </div>
              <div class="priority-track">
                <div class="priority-fill" [style.width.%]="item.pct" [style.background]="'linear-gradient(90deg, ' + item.color + ', ' + item.color + '88)'"></div>
                <span class="priority-pct">{{ item.pct }}%</span>
              </div>
            </div>
            <div *ngIf="priorityData.length === 0" class="empty-state">No priority data</div>
          </div>
        </div>

        <div class="card">
          <div class="card-header">
            <h3>Status Overview</h3>
            <span class="card-badge">{{ totalTasks }} tasks</span>
          </div>
          <div class="status-grid">
            <div class="status-card-item" *ngFor="let item of statusData" [style.borderLeftColor]="item.color">
              <div class="status-card-top">
                <span class="status-card-count">{{ item.count }}</span>
                <span class="status-card-dot" [style.background]="item.color"></span>
              </div>
              <span class="status-card-label">{{ item.label }}</span>
              <div class="status-card-track">
                <div class="status-card-fill" [style.width.%]="item.pct" [style.background]="item.color"></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="grid-2col">
        <div class="card">
          <div class="card-header">
            <h3>Department Overview</h3>
          </div>
          <div class="dept-chart">
            <div class="dept-group" *ngFor="let dept of deptData">
              <div class="dept-row">
                <div class="dept-info">
                  <span class="dept-dot" [style.background]="dept.color"></span>
                  <span class="dept-name">{{ dept.name || 'Unknown' }}</span>
                </div>
                <span class="dept-count">{{ dept.tasks }} tasks</span>
              </div>
              <div class="dept-track">
                <div class="dept-fill" [style.width.%]="dept.pct" [style.background]="'linear-gradient(90deg, ' + dept.color + ', ' + dept.color + '66)'"></div>
              </div>
            </div>
            <div *ngIf="deptData.length === 0" class="empty-state">No department data</div>
          </div>
        </div>

        <div class="card">
          <div class="card-header">
            <h3>Assigned Users</h3>
          </div>
          <div class="user-chart">
            @for (user of assignedUsers; track user.userName) {
            <div class="user-row">
              <div class="user-avatar">{{ user.userName.charAt(0).toUpperCase() }}</div>
              <span class="user-name">{{ user.userName }}</span>
              <span class="user-count-badge">{{ user.taskCount }}</span>
            </div>
            } @empty {
            <div class="empty-state">No assigned tasks</div>
            }
          </div>
        </div>
      </div>

      <div class="grid-2col">
        <div class="card">
          <div class="card-header">
            <h3>Sprint Distribution</h3>
          </div>
          <div class="sprint-chart">
            @for (sprint of sprintData; track sprint.sprintName) {
            <div class="sprint-card-item">
              <div class="sprint-card-header">
                <span class="sprint-name">{{ sprint.sprintName }}</span>
                <span class="sprint-total">{{ sprint.taskCount }} tasks</span>
              </div>
              <div class="sprint-progress">
                <div class="sprint-progress-track">
                  <div class="sprint-progress-fill" [style.width.%]="sprint.taskCount > 0 ? (sprint.completedCount / sprint.taskCount * 100) : 0" style="background: linear-gradient(90deg, #22c55e, #4ade80)"></div>
                </div>
                <span class="sprint-pct">{{ sprint.pct }}% completed</span>
              </div>
            </div>
            } @empty {
            <div class="empty-state">No sprints yet</div>
            }
          </div>
        </div>

        <div class="card">
          <div class="card-header">
            <h3>Recent Activity</h3>
          </div>
          <div class="activity-list">
            @for (act of activities; track act.text + act.time) {
            <div class="activity-item">
              <div class="activity-dot" [class.added]="act.type==='added'" [class.updated]="act.type==='updated'" [class.completed]="act.type==='completed'" [class.login]="act.type==='login'"></div>
              <div class="activity-body">
                <span class="activity-text">{{ act.text }}</span>
                <span class="activity-time">{{ act.time }}</span>
              </div>
            </div>
            } @empty {
            <div class="activity-empty">No recent activity</div>
            }
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard { }
    .welcome-section { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1.5rem; }
    .welcome-title { margin: 0; font-size: 1.35rem; font-weight: 700; color: #1e293b; }
    .welcome-sub { margin: 0.25rem 0 0; font-size: 0.9rem; color: #94a3b8; }
    .role-badge {
      font-size: 0.75rem; font-weight: 600; color: #2563eb; background: #eff6ff;
      padding: 0.3rem 0.75rem; border-radius: 999px; text-transform: uppercase;
    }
    .filter-bar { display: flex; gap: 0.75rem; margin-bottom: 1rem; flex-wrap: wrap; }
    :host ::ng-deep .filter-select .p-select { min-width: 160px; }
    .stats-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 1rem; margin-bottom: 1.5rem; }
    .stat-card {
      display: flex; align-items: center; gap: 1rem;
      background: #fff; padding: 1.25rem; border-radius: 12px; box-shadow: 0 1px 3px rgba(0,0,0,0.06);
      transition: transform 0.2s, box-shadow 0.2s;
    }
    .stat-card:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(0,0,0,0.08); }
    .stat-icon {
      width: 48px; height: 48px; border-radius: 12px;
      display: flex; align-items: center; justify-content: center; font-size: 1.25rem;
      flex-shrink: 0;
    }
    .stat-body { flex: 1; min-width: 0; }
    .stat-value { font-size: 1.5rem; font-weight: 700; color: #1e293b; line-height: 1.2; }
    .stat-label { font-size: 0.8rem; color: #94a3b8; display: block; margin-bottom: 0.5rem; }
    .stat-bar { height: 4px; background: #f1f5f9; border-radius: 2px; overflow: hidden; }
    .stat-bar-fill { height: 100%; border-radius: 2px; transition: width 0.4s; }
    .grid-2col { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-bottom: 1rem; }
    .card { background: #fff; border-radius: 12px; padding: 1.5rem; box-shadow: 0 1px 3px rgba(0,0,0,0.06); transition: box-shadow 0.2s; }
    .card:hover { box-shadow: 0 4px 16px rgba(0,0,0,0.08); }
    .card-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.25rem; }
    .card-header h3 { margin: 0; font-size: 0.95rem; font-weight: 600; color: #1e293b; }
    .card-badge { font-size: 0.75rem; color: #94a3b8; background: #f8fafc; padding: 0.2rem 0.6rem; border-radius: 999px; }

    .priority-chart { display: flex; flex-direction: column; gap: 1rem; min-height: 80px; }
    .priority-group { display: flex; flex-direction: column; gap: 0.35rem; }
    .priority-row { display: flex; justify-content: space-between; align-items: center; }
    .priority-badge { font-size: 0.8rem; font-weight: 600; padding: 0.15rem 0.6rem; border-radius: 999px; min-width: 50px; text-align: center; }
    .priority-value { font-size: 1rem; font-weight: 700; color: #1e293b; }
    .priority-track { position: relative; height: 10px; background: #f1f5f9; border-radius: 5px; overflow: hidden; }
    .priority-fill { height: 100%; border-radius: 5px; transition: width 0.6s ease; }
    .priority-pct { position: absolute; right: 6px; top: 50%; transform: translateY(-50%); font-size: 0.65rem; font-weight: 700; color: #475569; }

    .status-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 0.75rem; }
    .status-card-item {
      background: #fafbfc; border-left: 3px solid; border-radius: 8px; padding: 0.75rem;
      display: flex; flex-direction: column; gap: 0.5rem;
    }
    .status-card-top { display: flex; justify-content: space-between; align-items: center; }
    .status-card-count { font-size: 1.25rem; font-weight: 700; color: #1e293b; }
    .status-card-dot { width: 8px; height: 8px; border-radius: 50%; }
    .status-card-label { font-size: 0.78rem; color: #64748b; font-weight: 500; }
    .status-card-track { height: 4px; background: #e2e8f0; border-radius: 2px; overflow: hidden; }
    .status-card-fill { height: 100%; border-radius: 2px; transition: width 0.5s; }

    .dept-chart { display: flex; flex-direction: column; gap: 1rem; }
    .dept-group { display: flex; flex-direction: column; gap: 0.35rem; }
    .dept-row { display: flex; justify-content: space-between; align-items: center; }
    .dept-info { display: flex; align-items: center; gap: 0.5rem; }
    .dept-dot { width: 10px; height: 10px; border-radius: 50%; flex-shrink: 0; }
    .dept-name { font-size: 0.85rem; font-weight: 500; color: #334155; }
    .dept-count { font-size: 0.78rem; color: #64748b; }
    .dept-track { height: 8px; background: #f1f5f9; border-radius: 4px; overflow: hidden; }
    .dept-fill { height: 100%; border-radius: 4px; transition: width 0.6s ease; }

    .user-chart { display: flex; flex-direction: column; gap: 0.5rem; }
    .user-row {
      display: flex; align-items: center; gap: 0.75rem;
      padding: 0.5rem 0; border-bottom: 1px solid #f1f5f9;
    }
    .user-row:last-child { border-bottom: none; }
    .user-avatar {
      width: 32px; height: 32px; border-radius: 50%; background: #eff6ff; color: #2563eb;
      display: flex; align-items: center; justify-content: center;
      font-size: 0.8rem; font-weight: 700; flex-shrink: 0;
    }
    .user-name { flex: 1; font-size: 0.85rem; color: #334155; }
    .user-count-badge {
      background: #eff6ff; color: #2563eb; font-weight: 600;
      padding: 0.15rem 0.6rem; border-radius: 999px; font-size: 0.78rem;
    }
    .empty-state { padding: 1.5rem; text-align: center; color: #94a3b8; font-size: 0.9rem; }
    .dashboard.loading { opacity: 0.6; pointer-events: none; position: relative; }
    .dashboard-loader {
      position: absolute; inset: 0; display: flex; flex-direction: column;
      align-items: center; justify-content: center; gap: 0.75rem;
      background: rgba(255,255,255,0.5); z-index: 10; border-radius: 12px;
    }
    .loader-spinner {
      width: 32px; height: 32px; border: 3px solid #e2e8f0;
      border-top-color: #3b82f6; border-radius: 50%;
      animation: spin 0.7s linear infinite;
    }
    @keyframes spin { to { transform: rotate(360deg); } }
    .dashboard-loader span { font-size: 0.85rem; color: #64748b; font-weight: 500; }

    .sprint-chart { display: flex; flex-direction: column; gap: 0.75rem; }
    .sprint-card-item {
      background: #fafbfc; border-radius: 8px; padding: 0.85rem;
      display: flex; flex-direction: column; gap: 0.5rem;
    }
    .sprint-card-header { display: flex; justify-content: space-between; align-items: center; }
    .sprint-name { font-size: 0.85rem; font-weight: 600; color: #1e293b; }
    .sprint-total { font-size: 0.75rem; color: #64748b; }
    .sprint-progress { display: flex; align-items: center; gap: 0.75rem; }
    .sprint-progress-track { flex: 1; height: 6px; background: #e2e8f0; border-radius: 3px; overflow: hidden; }
    .sprint-progress-fill { height: 100%; border-radius: 3px; transition: width 0.6s ease; }
    .sprint-pct { font-size: 0.75rem; font-weight: 600; color: #22c55e; white-space: nowrap; }

    .activity-list { display: flex; flex-direction: column; gap: 1rem; }
    .activity-item { display: flex; gap: 0.75rem; }
    .activity-dot { width: 8px; height: 8px; border-radius: 50%; margin-top: 5px; flex-shrink: 0; background: #e2e8f0; }
    .activity-dot.added { background: #22c55e; box-shadow: 0 0 4px #22c55e66; }
    .activity-dot.updated { background: #3b82f6; box-shadow: 0 0 4px #3b82f666; }
    .activity-dot.completed { background: #8b5cf6; box-shadow: 0 0 4px #8b5cf666; }
    .activity-dot.login { background: #14b8a6; box-shadow: 0 0 4px #14b8a666; }
    .activity-body { display: flex; flex-direction: column; gap: 0.15rem; }
    .activity-text { font-size: 0.85rem; color: #334155; }
    .activity-time { font-size: 0.75rem; color: #94a3b8; }
    .activity-empty { padding: 1.5rem; text-align: center; color: #94a3b8; font-size: 0.9rem; }
  `]
})
export class DashboardComponent implements OnInit, OnDestroy {
  private auth = inject(AuthService);
  private dashboardService = inject(DashboardService);
  private auditLogService = inject(AuditLogService);
  private priorityService = inject(PriorityService);
  private statusService = inject(StatusService);
  private departmentService = inject(DepartmentService);
  private sprintTaskService = inject(SprintTaskService);
  private cdr = inject(ChangeDetectorRef);
  private filterSubject = new Subject<DashboardFilter>();
  private destroyRef = new Subject<void>();

  fullName = this.auth.getFullName();
  role = this.auth.getRole();

  dashboardLoading = false;
  stats: StatCard[] = [];
  priorityData: { label: string; count: number; pct: number; color: string }[] = [];
  statusData: { label: string; count: number; pct: number; color: string }[] = [];
  deptData: { name: string; tasks: number; pct: number; color: string }[] = [];
  assignedUsers: { userName: string; taskCount: number }[] = [];
  sprintData: { sprintName: string; taskCount: number; completedCount: number; pct: number }[] = [];
  activities: Activity[] = [];
  totalTasks = 0;

  filter: DashboardFilter = {};

  sprintOptions: SelectOption[] = [];
  priorityOptions: SelectOption[] = [];
  statusOptions: SelectOption[] = [];
  deptOptions: SelectOption[] = [];

  ngOnInit() {
    this.filterSubject.pipe(
      debounceTime(1000),
      takeUntil(this.destroyRef)
    ).subscribe(() => {
      this.loadDashboard();
    });
    this.loadFilterOptions();
    this.loadDashboard();
    this.loadRecentActivity();
  }

  ngOnDestroy() {
    this.destroyRef.next();
    this.destroyRef.complete();
    this.filterSubject.complete();
  }

  applyFilter() {
    this.filterSubject.next(this.filter);
  }

  private loadFilterOptions() {
    this.priorityService.getAll().subscribe({
      next: (items: PriorityDto[]) => {
        this.priorityOptions = items.map(p => ({ label: p.name, value: p.id }));
        this.cdr.markForCheck();
      },
    });

    this.statusService.getAll().subscribe({
      next: (items: StatusDto[]) => {
        this.statusOptions = items.map(s => ({ label: s.name, value: s.id }));
        this.cdr.markForCheck();
      },
    });

    this.departmentService.getAll().subscribe({
      next: (items: DepartmentDto[]) => {
        this.deptOptions = items.map(d => ({ label: d.name, value: d.id }));
        this.cdr.markForCheck();
      },
    });

    this.sprintTaskService.getAll().subscribe({
      next: (tasks: SprintTaskDto[]) => {
        const names = [...new Set(tasks.map(t => t.sprintName).filter((n): n is string => !!n))];
        this.sprintOptions = names.map(name => ({ label: name, value: name }));
        this.cdr.markForCheck();
      },
    });
  }

  private loadDashboard() {
    this.dashboardLoading = true;
    const hasFilter = !!(this.filter.sprintName || this.filter.priorityId || this.filter.statusId || this.filter.departmentId);
    const source$ = hasFilter
      ? this.dashboardService.filterDashboard(this.filter)
      : this.dashboardService.get(this.filter);
    source$.subscribe({
      next: (data: DashboardData) => {
        this.stats = [
          { label: 'Total Tasks', value: data.totalTasks, icon: 'pi pi-list', color: '#3b82f6', pct: 100 },
          { label: 'In Progress', value: data.inProgress, icon: 'pi pi-clock', color: '#f59e0b', pct: data.totalTasks > 0 ? Math.round(data.inProgress / data.totalTasks * 100) : 0 },
          { label: 'Completed', value: data.completed, icon: 'pi pi-check-circle', color: '#22c55e', pct: data.totalTasks > 0 ? Math.round(data.completed / data.totalTasks * 100) : 0 },
          { label: 'Active Sprints', value: data.activeSprints, icon: 'pi pi-flag', color: '#8b5cf6', pct: 100 },
        ];
        this.priorityData = data.priorityDistribution;
        this.statusData = data.statusDistribution;
        this.deptData = data.departmentDistribution;
        this.assignedUsers = data.assignedUserCounts;
        this.sprintData = data.sprintDistribution.map(s => ({
          ...s,
          pct: s.taskCount > 0 ? Math.round(s.completedCount / s.taskCount * 100) : 0
        }));
        this.totalTasks = data.totalTasks;
        this.dashboardLoading = false;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error('[Dashboard] load failed:', err);
        this.dashboardLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  private loadRecentActivity() {
    this.auditLogService.getAll({ pageSize: 10 }).subscribe({
      next: data => {
        try {
          this.activities = data.items.map(l => this.toActivity(l));
        } catch (e) {
          console.error('[Dashboard] map failed', e);
        }
        this.cdr.detectChanges();
      },
      error: err => console.error('Recent activity load failed:', err),
    });
  }

  private toActivity(log: AuditLogDto): Activity {
    try {
      const type = log.action === 'CREATE' ? 'added'
        : log.action === 'DELETE' ? 'completed'
        : log.action === 'LOGIN' ? 'login'
        : 'updated';

      if (log.action === 'LOGIN' || (log.tableName === 'AppUser' && log.action === 'UPDATE')) {
        const name = this.extractName(log.newValues ?? log.oldValues);
        return {
          text: `${name || log.changedBy || 'User'} logged in`,
          time: relativeTime(log.changedAt),
          type,
        };
      }

      const name = this.extractName(log.newValues ?? log.oldValues);
      const label = name ? `"${name}"` : `#${log.recordId ?? ''}`;
      return {
        text: `${log.tableName} ${label} ${log.action.toLowerCase()}d`,
        time: relativeTime(log.changedAt),
        type,
      };
    } catch (e) {
      console.error('[Dashboard] toActivity error', e, log);
      return { text: 'Unknown activity', time: '', type: 'updated' };
    }
  }

  private extractName(json: string | null): string | null {
    if (!json) return null;
    try {
      const obj = JSON.parse(json);
      return obj?.FullName || obj?.fullName || obj?.Name || obj?.name
        || obj?.Title || obj?.title || obj?.UserName || obj?.userName
        || obj?.Email || obj?.email || null;
    } catch {
      return null;
    }
  }
}
