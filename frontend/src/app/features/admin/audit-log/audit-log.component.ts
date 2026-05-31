import { Component, inject, NgZone, ChangeDetectorRef, afterNextRender } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { AuditLogService, AuditLogDto, AuditLogFilter } from '../../../core/services/audit-log.service';
import { relativeTime } from '../../../shared/utils/relative-time';

@Component({
  selector: 'app-audit-log',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    DialogModule,
    TooltipModule,
    SelectModule,
    DatePickerModule,
    InputTextModule,
  ],
  template: `
    <div class="page-header">
      <h2>Audit Log</h2>
      <p-button label="Refresh" icon="pi pi-refresh" (onClick)="loadLogs()"></p-button>
    </div>

    <div class="filter-bar">
      <span class="p-input-icon-left">
        <i class="pi pi-search"></i>
        <input pInputText type="text" placeholder="Table name" [(ngModel)]="filter.tableName" (input)="onFilterChange()" />
      </span>
      <p-select [options]="actionOptions" [(ngModel)]="filter.action" placeholder="Action" (onChange)="onFilterChange()"
        [showClear]="true" styleClass="filter-dropdown"></p-select>
      <p-datepicker [(ngModel)]="filterFrom" placeholder="From date" dateFormat="yy-mm-dd" (onSelect)="onFilterChange()"
        [showClear]="true" styleClass="filter-calendar"></p-datepicker>
      <p-datepicker [(ngModel)]="filterTo" placeholder="To date" dateFormat="yy-mm-dd" (onSelect)="onFilterChange()"
        [showClear]="true" styleClass="filter-calendar"></p-datepicker>
    </div>

    <p-table [value]="logs" [paginator]="true" [rows]="pageSize" [loading]="loading"
      [totalRecords]="totalCount" [lazy]="true" (onPage)="onPageChange($event)"
      styleClass="p-datatable-striped" [tableStyle]="{ 'min-width': '50rem' }"
      sortField="changedAt" [sortOrder]="-1">
      <ng-template pTemplate="header">
        <tr>
          <th pSortableColumn="changedAt">Date <p-sortIcon field="changedAt"></p-sortIcon></th>
          <th pSortableColumn="action">Action <p-sortIcon field="action"></p-sortIcon></th>
          <th>Table</th>
          <th pSortableColumn="changedBy">Changed By <p-sortIcon field="changedBy"></p-sortIcon></th>
          <th>Details</th>
        </tr>
      </ng-template>
      <ng-template pTemplate="body" let-log>
        <tr>
          <td>{{ relativeTime(log.changedAt) }}</td>
          <td>
            <span class="action-badge" [class.create]="log.action === 'CREATE'"
              [class.update]="log.action === 'UPDATE'" [class.delete]="log.action === 'DELETE'"
              [class.login]="log.action === 'LOGIN'">
              {{ log.action }}
            </span>
          </td>
          <td>{{ log.tableName }}</td>
          <td>{{ log.changedBy || '-' }}</td>
          <td>
            <button pButton icon="pi pi-eye" class="p-button-rounded p-button-text"
              (click)="showDetails(log)" pTooltip="View details"></button>
          </td>
        </tr>
      </ng-template>
      <ng-template pTemplate="emptymessage">
        <tr>
          <td colspan="5" style="text-align:center;padding:2rem;">No audit logs found</td>
        </tr>
      </ng-template>
    </p-table>

    <p-dialog header="Audit Log Details" [modal]="true" [(visible)]="dialogVisible"
      [style]="{ width: '650px' }">
      <ng-container *ngIf="selectedLog">
      <div class="detail-row">
        <span class="detail-label">Action:</span>
        <span class="detail-value">{{ selectedLog.action }}</span>
      </div>
      <div class="detail-row">
        <span class="detail-label">Table:</span>
        <span class="detail-value">{{ selectedLog.tableName }}</span>
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
    .filter-bar {
      display: flex;
      gap: 0.75rem;
      margin-bottom: 1rem;
      align-items: center;
      flex-wrap: wrap;
    }
    .filter-dropdown { width: 150px; }
    .filter-calendar { width: 180px; }
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
    .action-badge.login { background: #f0fdf4; color: #16a34a; }
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
  totalCount = 0;
  pageSize = 20;

  filter: AuditLogFilter = {};
  filterFrom?: Date;
  filterTo?: Date;

  actionOptions = [
    { label: 'CREATE', value: 'CREATE' },
    { label: 'UPDATE', value: 'UPDATE' },
    { label: 'DELETE', value: 'DELETE' },
    { label: 'LOGIN', value: 'LOGIN' },
  ];

  constructor() {
    afterNextRender(() => this.zone.runOutsideAngular(() => this.loadLogs()));
  }

  loadLogs() {
    const f: AuditLogFilter = { ...this.filter, pageSize: this.pageSize };
    if (this.filterFrom) f.from = this.filterFrom.toISOString();
    if (this.filterTo) {
      const end = new Date(this.filterTo);
      end.setHours(23, 59, 59, 999);
      f.to = end.toISOString();
    }

    this.auditLogService.getAll(f).subscribe({
      next: data => {
        this.logs = data.items;
        this.totalCount = data.totalCount;
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

  onFilterChange() {
    this.filter.page = 1;
    this.loadLogs();
  }

  onPageChange(event: any) {
    this.filter.page = (event.first / event.rows) + 1;
    this.pageSize = event.rows;
    this.loadLogs();
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
