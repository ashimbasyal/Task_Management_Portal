import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmationService, MessageService } from 'primeng/api';

interface SprintTask {
  id: number;
  title: string;
  priority: string;
  status: string;
  assignee: string;
  sprintName: string;
  startDate: string;
  endDate: string;
  remarks: string;
  department: string;
}

@Component({
  selector: 'app-sprint',
  standalone: true,
  imports: [
    CommonModule, FormsModule, TableModule, ButtonModule, DialogModule,
    SelectModule, InputTextModule, TextareaModule, TagModule,
    ToastModule, ConfirmDialogModule, TooltipModule,
  ],
  providers: [MessageService, ConfirmationService],
  template: `
    <p-toast position="bottom-right" key="br"></p-toast>
    <p-confirmDialog [style]="{ width: '450px' }"></p-confirmDialog>

    <div class="page-header">
      <h2>Sprint Management</h2>
      <div class="header-actions">
        <p-select [options]="sprintNames" placeholder="All Sprints" [(ngModel)]="selectedSprint" (onChange)="applyFilter()" appendTo="body" styleClass="sprint-select"></p-select>
      </div>
    </div>

    <p-table [value]="filteredTasks()" [paginator]="true" [rows]="10" [loading]="loading()"
      styleClass="p-datatable-striped" [tableStyle]="{ 'min-width': '75rem' }">
      <ng-template pTemplate="header">
        <tr>
          <th>Title</th><th>Priority</th><th>Status</th><th>Assignee</th><th>Sprint</th><th>Start Date</th><th>End Date</th><th>Actions</th>
        </tr>
      </ng-template>
      <ng-template pTemplate="body" let-task>
        <tr>
          <td>{{ task.title }}</td>
          <td><p-tag [value]="task.priority" [severity]="task.priority === 'High' ? 'danger' : task.priority === 'Medium' ? 'warn' : 'success'"></p-tag></td>
          <td>{{ task.status }}</td>
          <td>{{ task.assignee }}</td>
          <td>{{ task.sprintName }}</td>
          <td>{{ task.startDate }}</td>
          <td>{{ task.endDate }}</td>
          <td>
            <button pButton icon="pi pi-pencil" class="p-button-rounded p-button-text" (click)="editTask(task)" pTooltip="Edit"></button>
            <button pButton icon="pi pi-trash" class="p-button-rounded p-button-text p-button-danger" (click)="confirmDelete(task)" pTooltip="Delete"></button>
          </td>
        </tr>
      </ng-template>
      <ng-template pTemplate="emptymessage">
        <tr><td colspan="8" style="text-align:center;padding:2rem;">No sprint tasks found</td></tr>
      </ng-template>
    </p-table>

    <p-dialog header="Edit Sprint Task" [modal]="true" [(visible)]="dialogVisible" [style]="{ width: '520px' }">
      <div class="form">
        <div class="field">
          <label>Title <span class="req">*</span></label>
          <input pInputText [(ngModel)]="editForm.title" class="w-full" />
        </div>
        <div class="field-row">
          <div class="field">
            <label>Priority</label>
            <p-select [options]="priorityOptions" [(ngModel)]="editForm.priority" placeholder="Select" appendTo="body" styleClass="w-full"></p-select>
          </div>
          <div class="field">
            <label>Status</label>
            <p-select [options]="statusOptions" [(ngModel)]="editForm.status" placeholder="Select" appendTo="body" styleClass="w-full"></p-select>
          </div>
        </div>
        <div class="field-row">
          <div class="field">
            <label>Assignee</label>
            <p-select [options]="assigneeOptions" [(ngModel)]="editForm.assignee" placeholder="Select" appendTo="body" styleClass="w-full"></p-select>
          </div>
          <div class="field">
            <label>Sprint</label>
            <p-select [options]="sprintNames" [(ngModel)]="editForm.sprintName" placeholder="Select" appendTo="body" styleClass="w-full"></p-select>
          </div>
        </div>
        <div class="field-row">
          <div class="field">
            <label>Start Date</label>
            <input pInputText [(ngModel)]="editForm.startDate" class="w-full" placeholder="DD/MM/YYYY" />
          </div>
          <div class="field">
            <label>End Date</label>
            <input pInputText [(ngModel)]="editForm.endDate" class="w-full" placeholder="DD/MM/YYYY" />
          </div>
        </div>
        <div class="field">
          <label>Remarks</label>
          <textarea pTextarea [(ngModel)]="editForm.remarks" rows="2" class="w-full"></textarea>
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
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  tasks: SprintTask[] = [
    { id: 1, title: 'Implement login page', priority: 'High', status: 'In Progress', assignee: 'Alice', sprintName: 'Sprint 1', startDate: '01/05/2026', endDate: '15/05/2026', remarks: '', department: 'Engineering' },
    { id: 2, title: 'Build task dashboard', priority: 'Medium', status: 'Completed', assignee: 'Bob', sprintName: 'Sprint 1', startDate: '01/05/2026', endDate: '15/05/2026', remarks: 'Delivered on time', department: 'Engineering' },
    { id: 3, title: 'Excel upload feature', priority: 'Low', status: 'Open', assignee: 'Charlie', sprintName: 'Sprint 2', startDate: '16/05/2026', endDate: '30/05/2026', remarks: '', department: 'QA' },
    { id: 4, title: 'Notification module', priority: 'High', status: 'Open', assignee: 'Alice', sprintName: 'Sprint 2', startDate: '16/05/2026', endDate: '30/05/2026', remarks: '', department: 'Engineering' },
    { id: 5, title: 'API integration tests', priority: 'Medium', status: 'Completed', assignee: 'Bob', sprintName: 'Sprint 1', startDate: '01/05/2026', endDate: '15/05/2026', remarks: '', department: 'QA' },
  ];

  filteredTasks = signal<SprintTask[]>([]);
  loading = signal(false);
  saving = signal(false);
  dialogVisible = false;
  selectedSprint: string | null = null;

  sprintNames = ['Sprint 1', 'Sprint 2'];
  priorityOptions = ['High', 'Medium', 'Low'];
  statusOptions = ['Open', 'In Progress', 'Completed', 'On Hold'];
  assigneeOptions = ['Alice', 'Bob', 'Charlie'];

  editForm: Partial<SprintTask> = {};

  constructor() {
    this.applyFilter();
  }

  editTask(task: SprintTask) {
    this.editForm = { ...task };
    this.dialogVisible = true;
  }

  saveTask() {
    if (!this.editForm.title) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Title is required', key: 'br' });
      return;
    }
    this.saving.set(true);
    const idx = this.tasks.findIndex(t => t.id === this.editForm.id);
    if (idx >= 0) {
      this.tasks[idx] = { ...this.editForm as SprintTask };
      this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'Sprint task updated', key: 'br' });
    }
    this.applyFilter();
    this.dialogVisible = false;
    this.saving.set(false);
  }

  confirmDelete(task: SprintTask) {
    this.confirmationService.confirm({
      message: `Delete sprint task "${task.title}"?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.tasks = this.tasks.filter(t => t.id !== task.id);
        this.applyFilter();
        this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Sprint task removed', key: 'br' });
      },
    });
  }

  applyFilter() {
    this.loading.set(true);
    this.filteredTasks.set(
      this.selectedSprint
        ? this.tasks.filter(t => t.sprintName === this.selectedSprint)
        : this.tasks
    );
    this.loading.set(false);
  }
}
