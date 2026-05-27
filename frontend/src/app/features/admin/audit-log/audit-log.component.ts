import { Component, inject, NgZone, ChangeDetectorRef, afterNextRender } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { AuditLogService, AuditLogDto } from '../../../core/services/audit-log.service';
import { relativeTime } from '../../../shared/utils/relative-time';

@Component({
  selector: 'app-audit-log',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    DialogModule,
    TooltipModule,
  ],
  template: `
    <div class="page-header">
      <h2>Audit Log</h2>
      <p-button label="Refresh" icon="pi pi-refresh" (onClick)="loadLogs()"></p-button>
    </div>

    <p-table [value]="logs" [paginator]="true" [rows]="20" [loading]="loading"
      styleClass="p-datatable-striped" [tableStyle]="{ 'min-width': '60rem' }"
      sortField="changedAt" [sortOrder]="-1">
      <ng-template pTemplate="header">
        <tr>
          <th pSortableColumn="changedAt">Date <p-sortIcon field="changedAt"></p-sortIcon></th>
          <th pSortableColumn="tableName">Table <p-sortIcon field="tableName"></p-sortIcon></th>
          <th pSortableColumn="action">Action <p-sortIcon field="action"></p-sortIcon></th>
          <th>Record ID</th>
          <th pSortableColumn="changedBy">Changed By <p-sortIcon field="changedBy"></p-sortIcon></th>
          <th>Details</th>
        </tr>
      </ng-template>
      <ng-template pTemplate="body" let-log>
        <tr>
          <td>{{ relativeTime(log.changedAt) }}</td>
          <td>{{ log.tableName }}</td>
          <td>
            <span class="action-badge" [class.create]="log.action === 'CREATE'"
              [class.update]="log.action === 'UPDATE'" [class.delete]="log.action === 'DELETE'">
              {{ log.action }}
            </span>
          </td>
          <td>{{ log.recordId ?? '-' }}</td>
          <td>{{ log.changedBy || '-' }}</td>
          <td>
            <button pButton icon="pi pi-eye" class="p-button-rounded p-button-text"
              (click)="showDetails(log)" pTooltip="View details"></button>
          </td>
        </tr>
      </ng-template>
      <ng-template pTemplate="emptymessage">
        <tr>
          <td colspan="6" style="text-align:center;padding:2rem;">No audit logs found</td>
        </tr>
      </ng-template>
    </p-table>

    <p-dialog header="Audit Log Details" [modal]="true" [(visible)]="dialogVisible"
      [style]="{ width: '650px' }">
      <ng-container *ngIf="selectedLog">
      <div class="detail-row">
        <span class="detail-label">Table:</span>
        <span class="detail-value">{{ selectedLog.tableName }}</span>
      </div>
      <div class="detail-row">
        <span class="detail-label">Action:</span>
        <span class="detail-value">{{ selectedLog.action }}</span>
      </div>
      <div class="detail-row">
        <span class="detail-label">Record ID:</span>
        <span class="detail-value">{{ selectedLog.recordId ?? '-' }}</span>
      </div>
      <div class="detail-row">
        <span class="detail-label">Changed By:</span>
        <span class="detail-value">{{ selectedLog.changedBy || '-' }}</span>
      </div>
      <div class="detail-row">
        <span class="detail-label">Changed At:</span>
        <span class="detail-value">{{ selectedLog.changedAt | date:'medium' }}</span>
      </div>
      <div class="detail-section" *ngIf="selectedLog.oldValues">
        <h4>Old Values</h4>
        <pre class="json-display">{{ formatJson(selectedLog.oldValues) }}</pre>
      </div>
      <div class="detail-section" *ngIf="selectedLog.newValues">
        <h4>New Values</h4>
        <pre class="json-display">{{ formatJson(selectedLog.newValues) }}</pre>
      </div>
      </ng-container>
    </p-dialog>
  `,
  styles: [`
    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
    }
    .page-header h2 {
      margin: 0;
      font-size: 1.5rem;
      color: #1e293b;
    }
    .action-badge {
      display: inline-block;
      padding: 0.2rem 0.6rem;
      border-radius: 4px;
      font-size: 0.8rem;
      font-weight: 600;
    }
    .action-badge.create { background: #d1fae5; color: #059669; }
    .action-badge.update { background: #dbeafe; color: #1d4ed8; }
    .action-badge.delete { background: #fee2e2; color: #dc2626; }
    .detail-row {
      display: flex;
      gap: 0.5rem;
      margin-bottom: 0.5rem;
      font-size: 0.9rem;
    }
    .detail-label {
      font-weight: 600;
      color: #475569;
      min-width: 100px;
    }
    .detail-value {
      color: #1e293b;
    }
    .detail-section {
      margin-top: 1rem;
    }
    .detail-section h4 {
      margin: 0 0 0.5rem;
      font-size: 0.95rem;
      color: #334155;
    }
    .json-display {
      background: #f1f5f9;
      padding: 0.75rem;
      border-radius: 6px;
      font-size: 0.78rem;
      max-height: 250px;
      overflow: auto;
      white-space: pre-wrap;
      word-break: break-all;
    }
  `]
})
export class AuditLogComponent {
  private auditLogService = inject(AuditLogService);
  private zone = inject(NgZone);
  private cdr = inject(ChangeDetectorRef);

  logs: AuditLogDto[] = [];
  loading = true;
  dialogVisible = false;
  selectedLog: AuditLogDto | null = null;

  constructor() {
    afterNextRender(() => this.zone.runOutsideAngular(() => this.loadLogs()));
  }

  loadLogs() {
    this.auditLogService.getAll().subscribe({
      next: data => {
        this.logs = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error('[AuditLog] error', err);
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  relativeTime = relativeTime;

  showDetails(log: AuditLogDto) {
    this.selectedLog = log;
    this.dialogVisible = true;
  }

  formatJson(json: string): string {
    try {
      return JSON.stringify(JSON.parse(json), null, 2);
    } catch {
      return json;
    }
  }
}
