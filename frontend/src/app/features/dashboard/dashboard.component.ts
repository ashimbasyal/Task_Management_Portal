import { Component, ChangeDetectorRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';
import { AuditLogService, AuditLogDto } from '../../core/services/audit-log.service';
import { relativeTime } from '../../shared/utils/relative-time';

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
  imports: [CommonModule],
  template: `
    <div class="dashboard">
      <div class="welcome-section">
        <div>
          <h2 class="welcome-title">Welcome back, {{ fullName }}</h2>
          <p class="welcome-sub">Here's what's happening with your tasks today.</p>
        </div>
        <div class="role-badge">{{ role }}</div>
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
          <div class="bar-chart">
            <div class="bar-group" *ngFor="let item of priorityData">
              <div class="bar-row">
                <span class="bar-label">{{ item.label }}</span>
                <span class="bar-value">{{ item.count }}</span>
              </div>
              <div class="bar-track">
                <div class="bar-fill" [style.width.%]="item.pct" [style.background]="item.color"></div>
              </div>
            </div>
          </div>
        </div>

        <div class="card">
          <div class="card-header">
            <h3>Status Overview</h3>
            <span class="card-badge">{{ totalTasks }} tasks</span>
          </div>
          <div class="status-list">
            <div class="status-item" *ngFor="let item of statusData">
              <span class="status-dot" [style.background]="item.color"></span>
              <span class="status-label">{{ item.label }}</span>
              <span class="status-count">{{ item.count }}</span>
              <div class="status-track">
                <div class="status-fill" [style.width.%]="item.pct" [style.background]="item.color"></div>
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
          <table class="dept-table">
            <thead>
              <tr><th>Department</th><th>Tasks</th><th>Progress</th></tr>
            </thead>
            <tbody>
              <tr *ngFor="let dept of deptData">
                <td>{{ dept.name }}</td>
                <td>{{ dept.tasks }}</td>
                <td><div class="dept-bar"><div class="dept-bar-fill" [style.width.%]="dept.pct" [style.background]="dept.color"></div></div></td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="card">
          <div class="card-header">
            <h3>Recent Activity</h3>
          </div>
          <div class="activity-list">
            @for (act of activities; track act.text + act.time) {
            <div class="activity-item">
              <div class="activity-dot" [class.added]="act.type==='added'" [class.updated]="act.type==='updated'" [class.completed]="act.type==='completed'"></div>
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
    .stats-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 1rem; margin-bottom: 1.5rem; }
    .stat-card {
      display: flex; align-items: center; gap: 1rem;
      background: #fff; padding: 1.25rem; border-radius: 12px; box-shadow: 0 1px 3px rgba(0,0,0,0.06);
    }
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
    .card { background: #fff; border-radius: 12px; padding: 1.5rem; box-shadow: 0 1px 3px rgba(0,0,0,0.06); }
    .card-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.25rem; }
    .card-header h3 { margin: 0; font-size: 0.95rem; font-weight: 600; color: #1e293b; }
    .card-badge { font-size: 0.75rem; color: #94a3b8; }
    .bar-chart { display: flex; flex-direction: column; gap: 1rem; }
    .bar-group { display: flex; flex-direction: column; gap: 0.35rem; }
    .bar-row { display: flex; justify-content: space-between; }
    .bar-label { font-size: 0.85rem; color: #475569; }
    .bar-value { font-size: 0.85rem; font-weight: 600; color: #1e293b; }
    .bar-track { height: 8px; background: #f1f5f9; border-radius: 4px; overflow: hidden; }
    .bar-fill { height: 100%; border-radius: 4px; transition: width 0.5s; }
    .status-list { display: flex; flex-direction: column; gap: 1rem; }
    .status-item { display: flex; align-items: center; gap: 0.75rem; }
    .status-dot { width: 10px; height: 10px; border-radius: 50%; flex-shrink: 0; }
    .status-label { flex: 1; font-size: 0.85rem; color: #475569; }
    .status-count { font-size: 0.85rem; font-weight: 600; color: #1e293b; min-width: 24px; text-align: right; }
    .status-track { width: 80px; height: 6px; background: #f1f5f9; border-radius: 3px; overflow: hidden; }
    .status-fill { height: 100%; border-radius: 3px; transition: width 0.5s; }
    .dept-table { width: 100%; border-collapse: collapse; }
    .dept-table th { text-align: left; font-size: 0.75rem; font-weight: 600; color: #94a3b8; text-transform: uppercase; padding-bottom: 0.5rem; }
    .dept-table td { padding: 0.5rem 0; font-size: 0.85rem; color: #475569; border-bottom: 1px solid #f1f5f9; }
    .dept-table tr:last-child td { border-bottom: none; }
    .dept-bar { height: 6px; background: #f1f5f9; border-radius: 3px; overflow: hidden; width: 100px; }
    .dept-bar-fill { height: 100%; border-radius: 3px; }
    .activity-list { display: flex; flex-direction: column; gap: 1rem; }
    .activity-item { display: flex; gap: 0.75rem; }
    .activity-dot { width: 8px; height: 8px; border-radius: 50%; margin-top: 5px; flex-shrink: 0; background: #e2e8f0; }
    .activity-dot.added { background: #22c55e; }
    .activity-dot.updated { background: #3b82f6; }
    .activity-dot.completed { background: #8b5cf6; }
    .activity-body { display: flex; flex-direction: column; gap: 0.15rem; }
    .activity-text { font-size: 0.85rem; color: #334155; }
    .activity-time { font-size: 0.75rem; color: #94a3b8; }
    .activity-empty { padding: 1.5rem; text-align: center; color: #94a3b8; font-size: 0.9rem; }
  `]
})
export class DashboardComponent implements OnInit {
  private auth = inject(AuthService);
  private auditLogService = inject(AuditLogService);
  private cdr = inject(ChangeDetectorRef);
  fullName = this.auth.getFullName();
  role = this.auth.getRole();

  stats: StatCard[] = [];
  priorityData = [
    { label: 'High', count: 12, pct: 100, color: '#ef4444' },
    { label: 'Medium', count: 18, pct: 75, color: '#f59e0b' },
    { label: 'Low', count: 8, pct: 40, color: '#22c55e' },
  ];
  statusData = [
    { label: 'Open', count: 15, pct: 40, color: '#3b82f6' },
    { label: 'In Progress', count: 8, pct: 21, color: '#f59e0b' },
    { label: 'Completed', count: 20, pct: 53, color: '#22c55e' },
    { label: 'On Hold', count: 5, pct: 13, color: '#ef4444' },
  ];
  deptData = [
    { name: 'Engineering', tasks: 22, pct: 75, color: '#3b82f6' },
    { name: 'Quality Assurance', tasks: 10, pct: 45, color: '#8b5cf6' },
    { name: 'Support', tasks: 6, pct: 30, color: '#f59e0b' },
  ];
  activities: Activity[] = [];
  totalTasks = 38;

  ngOnInit() {
    this.stats = [
      { label: 'Total Tasks', value: 38, icon: 'pi pi-list', color: '#3b82f6', pct: 100 },
      { label: 'In Progress', value: 8, icon: 'pi pi-clock', color: '#f59e0b', pct: 21 },
      { label: 'Completed', value: 20, icon: 'pi pi-check-circle', color: '#22c55e', pct: 53 },
      { label: 'Active Sprints', value: 2, icon: 'pi pi-flag', color: '#8b5cf6', pct: 100 },
    ];
    this.loadRecentActivity();
  }

  private loadRecentActivity() {
    this.auditLogService.getAll().subscribe({
      next: data => {
        try {
          this.activities = data.slice(0, 10).map(l => this.toActivity(l));
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
        : 'updated';
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
