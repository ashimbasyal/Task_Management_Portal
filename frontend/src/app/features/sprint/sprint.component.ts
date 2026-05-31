import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { DatePickerModule } from 'primeng/datepicker';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmationService, MessageService } from 'primeng/api';
import { SprintTaskService, SprintTaskDto } from '../../core/services/sprint-task.service';
import { SprintStatusTriggerService, SprintStatusTriggerDto } from '../../core/services/sprint-status-trigger.service';
import { StatusService, StatusDto } from '../../core/services/status.service';
import { UserService, UserDto } from '../../core/services/user.service';

@Component({
  selector: 'app-sprint',
  standalone: true,
  imports: [
    CommonModule, FormsModule, TableModule, ButtonModule, DialogModule,
    SelectModule, InputTextModule, TextareaModule, DatePickerModule, TagModule,
    ToastModule, ConfirmDialogModule, TooltipModule,
  ],
  providers: [MessageService, ConfirmationService],
  template: `
    <p-toast position="bottom-right" key="br"></p-toast>
    <p-confirmDialog [style]="{ width: '450px' }"></p-confirmDialog>

    <div class="page-header">
      <h2>Sprint Management</h2>
      <div class="header-actions">
        <p-select [options]="sprintNameOpts()" placeholder="All Sprints" [(ngModel)]="selectedSprint" (onChange)="applyFilter()" appendTo="body" styleClass="sprint-select" [showClear]="true"></p-select>
        <p-select [options]="triggerOpts()" placeholder="Sprint Status" [(ngModel)]="selectedTriggerId" appendTo="body" styleClass="trigger-select" optionLabel="name" optionValue="id" [showClear]="true"></p-select>
      </div>
    </div>

    <div class="sprint-status-bar" *ngIf="selectedTriggerId">
      <span class="status-indicator" [class.active]="selectedTriggerName !== 'Closed'" [class.closed]="selectedTriggerName === 'Closed'">
        Sprint Status: <strong>{{ selectedTriggerName }}</strong>
      </span>
    </div>

    <p-table [value]="filteredTasks()" [paginator]="true" [rows]="10" [loading]="loading()"
      styleClass="p-datatable-striped" [tableStyle]="{ 'min-width': '85rem' }">
      <ng-template pTemplate="header">
        <tr>
          <th>SN</th><th>Title</th><th>Priority</th><th>Status</th><th>Assignee</th><th>Sprint</th><th>Start Date</th><th>End Date</th><th>Actions</th>
        </tr>
      </ng-template>
      <ng-template pTemplate="body" let-task>
        <tr>
          <td>{{ task.backlogTaskSN }}</td>
          <td>{{ task.backlogTaskTitle }}</td>
          <td><p-tag [value]="task.priorityName" [severity]="task.priorityName === 'High' ? 'danger' : task.priorityName === 'Medium' ? 'warn' : 'success'"></p-tag></td>
          <td>{{ task.statusName }}</td>
          <td>{{ task.assigneeName }}</td>
          <td>{{ task.sprintName }}</td>
          <td>{{ task.startDate | date:'dd/MM/yyyy' }}</td>
          <td>{{ task.endDate | date:'dd/MM/yyyy' }}</td>
          <td>
            <button pButton icon="pi pi-pencil" class="p-button-rounded p-button-text" (click)="editTask(task)" pTooltip="Edit"></button>
            <button pButton icon="pi pi-trash" class="p-button-rounded p-button-text p-button-danger" (click)="confirmDelete(task)" pTooltip="Delete"></button>
          </td>
        </tr>
      </ng-template>
      <ng-template pTemplate="emptymessage">
        <tr><td colspan="9" style="text-align:center;padding:2rem;">No sprint tasks found</td></tr>
      </ng-template>
    </p-table>

    <p-dialog header="Edit Sprint Task" [modal]="true" [(visible)]="dialogVisible" [style]="{ width: '520px' }">
      <div class="form" *ngIf="editTaskData">
        <div class="field">
          <label>Title</label>
          <input pInputText [ngModel]="editTaskData.backlogTaskTitle" class="w-full" disabled />
        </div>
        <div class="field-row">
          <div class="field">
            <label>Status</label>
            <p-select [options]="statusOpts()" [(ngModel)]="editTaskData.statusId" placeholder="Select" appendTo="body" styleClass="w-full" optionLabel="name" optionValue="id"></p-select>
          </div>
          <div class="field">
            <label>Assignee</label>
            <p-select [options]="assigneeOpts()" [(ngModel)]="editTaskData.assigneeId" placeholder="Select" appendTo="body" styleClass="w-full" optionLabel="label" optionValue="value"></p-select>
          </div>
        </div>
        <div class="field-row">
          <div class="field">
            <label>Start Date</label>
            <p-datepicker [(ngModel)]="editTaskData.startDate" [minDate]="today" [keepInvalid]="true" dateFormat="dd/mm/yy" styleClass="w-full" appendTo="body"></p-datepicker>
          </div>
          <div class="field">
            <label>End Date</label>
            <p-datepicker [(ngModel)]="editTaskData.endDate" [minDate]="today" [keepInvalid]="true" dateFormat="dd/mm/yy" styleClass="w-full" appendTo="body"></p-datepicker>
          </div>
        </div>
        <div class="field">
          <label>Sprint Name</label>
          <input pInputText [(ngModel)]="editTaskData.sprintName" class="w-full" />
        </div>
        <div class="field">
          <label>Remarks</label>
          <textarea pTextarea [(ngModel)]="editTaskData.remarks" rows="2" class="w-full"></textarea>
        </div>
      </div>
      <ng-template pTemplate="footer">
        <button pButton label="Cancel" class="p-button-text" (click)="dialogVisible = false"></button>
        <button pButton label="Save" [loading]="saving()" (click)="saveTask()"></button>
      </ng-template>
    </p-dialog>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
    .page-header h2 { margin: 0; font-size: 1.5rem; color: #1e293b; }
    .header-actions { display: flex; gap: 0.5rem; }
    :host ::ng-deep .sprint-select .p-select { min-width: 180px; }
    :host ::ng-deep .trigger-select .p-select { min-width: 160px; }
    .sprint-status-bar { margin-bottom: 0.75rem; }
    .status-indicator {
      display: inline-flex; align-items: center; gap: 0.4rem;
      font-size: 0.85rem; padding: 0.3rem 0.75rem; border-radius: 999px;
      background: #f0fdf4; color: #16a34a; border: 1px solid #bbf7d0;
    }
    .status-indicator.closed { background: #fef2f2; color: #dc2626; border-color: #fecaca; }
    .form { display: flex; flex-direction: column; gap: 0.75rem; }
    .field-row { display: flex; gap: 0.75rem; }
    .field-row .field { flex: 1; }
    .field { display: flex; flex-direction: column; gap: 0.35rem; }
    .field label { font-size: 0.85rem; font-weight: 600; color: #374151; }
    .req { color: #ef4444; }
    .w-full { width: 100%; }
  `]
})
export class SprintComponent {
  private sprintTaskService = inject(SprintTaskService);
  private sprintStatusTriggerService = inject(SprintStatusTriggerService);
  private statusService = inject(StatusService);
  private userService = inject(UserService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  allTasks: SprintTaskDto[] = [];
  filteredTasks = signal<SprintTaskDto[]>([]);
  loading = signal(false);
  saving = signal(false);
  dialogVisible = false;
  selectedSprint: string | null = null;
  selectedTriggerId: number | null = null;

  statusOpts = signal<StatusDto[]>([]);
  assigneeOpts = signal<{ label: string; value: string }[]>([]);
  sprintNameOpts = signal<{ label: string; value: string }[]>([]);
  triggerOpts = signal<SprintStatusTriggerDto[]>([]);

  today = new Date();

  editTaskData: {
    id?: number;
    backlogTaskTitle?: string | null;
    sprintName?: string | null;
    startDate: Date | null;
    endDate: Date | null;
    remarks?: string | null;
    assigneeId?: string | null;
    statusId?: number | null;
  } = { startDate: null, endDate: null };

  constructor() {
    this.loadReferenceData();
    this.loadTasks();
    this.loadTriggers();
  }

  private loadReferenceData() {
    this.statusService.getAll().subscribe(d => this.statusOpts.set(d));
    this.userService.getAll().subscribe({
      next: users => this.assigneeOpts.set(users.map(u => ({ label: `${u.fullName} (${u.email})`, value: u.id }))),
      error: () => this.assigneeOpts.set([]),
    });
  }

  private loadTriggers() {
    this.sprintStatusTriggerService.getAll().subscribe({
      next: triggers => this.triggerOpts.set(triggers),
    });
  }

  private loadTasks() {
    this.loading.set(true);
    this.sprintTaskService.getAll().subscribe({
      next: data => {
        this.allTasks = data;
        const names = [...new Set(data.map(t => t.sprintName).filter(Boolean))] as string[];
        this.sprintNameOpts.set(names.map(n => ({ label: n, value: n })));
        this.applyFilter();
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  editTask(task: SprintTaskDto) {
    this.editTaskData = {
      id: task.id,
      backlogTaskTitle: task.backlogTaskTitle,
      sprintName: task.sprintName,
      startDate: task.startDate ? new Date(task.startDate) : null,
      endDate: task.endDate ? new Date(task.endDate) : null,
      remarks: task.remarks,
      assigneeId: task.assigneeId,
      statusId: task.statusId,
    };
    this.dialogVisible = true;
  }

  saveTask() {
    if (!this.editTaskData.id) return;
    this.saving.set(true);
    const d = this.editTaskData.startDate;
    const ed = this.editTaskData.endDate;
    this.sprintTaskService.update(this.editTaskData.id, {
      sprintName: this.editTaskData.sprintName ?? null,
      startDate: d ? `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}T00:00:00.000Z` : null,
      endDate: ed ? `${ed.getFullYear()}-${String(ed.getMonth() + 1).padStart(2, '0')}-${String(ed.getDate()).padStart(2, '0')}T00:00:00.000Z` : null,
      remarks: this.editTaskData.remarks ?? null,
      assigneeId: this.editTaskData.assigneeId ?? null,
      statusId: this.editTaskData.statusId ?? null,
    }).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'Sprint task updated', key: 'br' });
        this.dialogVisible = false;
        this.saving.set(false);
        this.loadTasks();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update sprint task', key: 'br' });
        this.saving.set(false);
      },
    });
  }

  confirmDelete(task: SprintTaskDto) {
    this.confirmationService.confirm({
      message: `Delete sprint task "${task.backlogTaskTitle}"?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.sprintTaskService.delete(task.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Sprint task removed', key: 'br' });
            this.loadTasks();
          },
          error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete', key: 'br' }),
        });
      },
    });
  }

  get selectedTriggerName(): string {
    const found = this.triggerOpts().find(t => t.id === this.selectedTriggerId);
    return found?.name ?? '';
  }

  applyFilter() {
    this.filteredTasks.set(
      this.selectedSprint
        ? this.allTasks.filter(t => t.sprintName === this.selectedSprint)
        : this.allTasks
    );
  }
}
