import { Component, inject, signal, afterNextRender, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { DatePickerModule } from 'primeng/datepicker';
import { FileUploadModule, FileUpload } from 'primeng/fileupload';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmationService, MessageService } from 'primeng/api';
import { BacklogService, BacklogItem } from '../../core/services/backlog.service';
import { SprintTaskService } from '../../core/services/sprint-task.service';
import { PriorityService, PriorityDto } from '../../core/services/priority.service';
import { StatusService, StatusDto } from '../../core/services/status.service';
import { DepartmentService, DepartmentDto } from '../../core/services/department.service';
import { UserService, UserDto } from '../../core/services/user.service';

interface BacklogForm {
  id?: number;
  title: string;
  description: string;
  requestedBy: string;
  gitlabLink: string;
  remarks: string;
  priorityId: number | null;
  statusId: number | null;
  departmentId: number | null;
}

interface SprintForm {
  sprintName: string;
  assigneeId: string | null;
  startDate: Date | null;
  endDate: Date | null;
  remarks: string;
}

@Component({
  selector: 'app-backlog',
  standalone: true,
  imports: [
    CommonModule, FormsModule, TableModule, ButtonModule, DialogModule,
    SelectModule, InputTextModule, TextareaModule, DatePickerModule, FileUploadModule, TagModule,
    ToastModule, ConfirmDialogModule, TooltipModule,
  ],
  providers: [MessageService, ConfirmationService],
  template: `
    <p-toast position="bottom-right" key="br"></p-toast>
    <p-confirmDialog [style]="{ width: '450px' }"></p-confirmDialog>

    <div class="page-header">
      <h2>Backlog Management</h2>
      <div class="header-actions">
        <p-button label="Sample" icon="pi pi-download" severity="secondary" (onClick)="downloadSample()" styleClass="p-button-outlined"></p-button>
        <p-button label="Excel Upload" icon="pi pi-upload" severity="secondary" (onClick)="uploadVisible = true" styleClass="p-button-outlined"></p-button>
        <p-button label="Create Backlog" icon="pi pi-plus" (onClick)="showCreate()"></p-button>
      </div>
    </div>

    <div class="filters">
      <p-select [options]="priorityOpts()" placeholder="Priority" [(ngModel)]="filterPriorityId" (onChange)="loadItems()" appendTo="body" styleClass="filter-select" optionLabel="name" optionValue="id" [showClear]="true"></p-select>
      <p-select [options]="statusOpts()" placeholder="Status" [(ngModel)]="filterStatusId" (onChange)="loadItems()" appendTo="body" styleClass="filter-select" optionLabel="name" optionValue="id" [showClear]="true"></p-select>
      <p-select [options]="deptOpts()" placeholder="Department" [(ngModel)]="filterDeptId" (onChange)="loadItems()" appendTo="body" styleClass="filter-select" optionLabel="name" optionValue="id" [showClear]="true"></p-select>
    </div>

    <p-table [value]="filteredItems()" [paginator]="true" [rows]="10" [loading]="loading()"
      styleClass="p-datatable-striped" [tableStyle]="{ 'min-width': '85rem' }">
      <ng-template pTemplate="header">
        <tr>
          <th>SN</th><th>Title</th><th>Priority</th><th>Status</th><th>Department</th><th>Requested By</th><th>Actions</th>
        </tr>
      </ng-template>
      <ng-template pTemplate="body" let-item>
        <tr>
          <td>{{ item.sn }}</td>
          <td>{{ item.title }}</td>
          <td><p-tag [value]="item.priorityName" [severity]="item.priorityName === 'High' ? 'danger' : item.priorityName === 'Medium' ? 'warn' : 'success'"></p-tag></td>
          <td>{{ item.statusName }}</td>
          <td>{{ item.departmentName }}</td>
          <td>{{ item.requestedBy }}</td>
          <td>
            <button pButton icon="pi pi-pencil" class="p-button-rounded p-button-text" (click)="editItem(item)" pTooltip="Edit"></button>
            <button pButton icon="pi pi-send" class="p-button-rounded p-button-text p-button-info" (click)="showMoveToSprint(item)" [disabled]="item.isMovedToSprint" pTooltip="Move to Sprint"></button>
            <button pButton icon="pi pi-trash" class="p-button-rounded p-button-text p-button-danger" (click)="confirmDelete(item)" pTooltip="Delete"></button>
          </td>
        </tr>
      </ng-template>
      <ng-template pTemplate="emptymessage">
        <tr><td colspan="7" style="text-align:center;padding:2rem;">No backlog items found</td></tr>
      </ng-template>
    </p-table>

    <p-dialog header="Backlog Item" [modal]="true" [(visible)]="dialogVisible" [style]="{ width: '520px' }">
      <div class="form">
        <div class="field">
          <label>Title <span class="req">*</span></label>
          <input pInputText [(ngModel)]="formItem.title" class="w-full" />
        </div>
        <div class="field">
          <label>Description</label>
          <textarea pTextarea [(ngModel)]="formItem.description" rows="3" class="w-full"></textarea>
        </div>
        <div class="field-row">
          <div class="field">
            <label>Priority <span class="req">*</span></label>
            <p-select [options]="priorityOpts()" [(ngModel)]="formItem.priorityId" placeholder="Select" appendTo="body" styleClass="w-full" optionLabel="name" optionValue="id"></p-select>
          </div>
          <div class="field">
            <label>Department <span class="req">*</span></label>
            <p-select [options]="deptOpts()" [(ngModel)]="formItem.departmentId" placeholder="Select" appendTo="body" styleClass="w-full" optionLabel="name" optionValue="id"></p-select>
          </div>
        </div>
        <div class="field-row">
          <div class="field">
            <label>Status</label>
            <p-select [options]="statusOpts()" [(ngModel)]="formItem.statusId" placeholder="Select" appendTo="body" styleClass="w-full" optionLabel="name" optionValue="id"></p-select>
          </div>
          <div class="field assignee-field">
            <label>Requested By</label>
            <div class="assignee-wrapper">
              <input pInputText [(ngModel)]="requestedByInput" (input)="onRequestedByInput()" (focus)="onRequestedByFocus()" (blur)="onRequestedByBlur()" class="w-full" placeholder="Type @ to search users" autocomplete="off" />
              <div *ngIf="showRequestedByOverlay" class="assignee-overlay">
                <div *ngFor="let user of filteredRequestedByUsers()" class="assignee-option" (mousedown)="selectRequestedBy(user)">
                  <span>{{ user.fullName }}</span>
                  <small>{{ user.email }}</small>
                </div>
                <div *ngIf="filteredRequestedByUsers().length === 0" class="assignee-option no-results">No users found</div>
              </div>
            </div>
          </div>
        </div>
        <div class="field">
          <label>GitLab Link</label>
          <input pInputText [(ngModel)]="formItem.gitlabLink" class="w-full" />
        </div>
        <div class="field">
          <label>Remarks</label>
          <textarea pTextarea [(ngModel)]="formItem.remarks" rows="2" class="w-full"></textarea>
        </div>
      </div>
      <ng-template pTemplate="footer">
        <button pButton label="Cancel" class="p-button-text" (click)="dialogVisible = false"></button>
        <button pButton label="Save" [loading]="saving()" (click)="saveItem()"></button>
      </ng-template>
    </p-dialog>

    <p-dialog header="Move to Sprint" [modal]="true" [(visible)]="sprintDialogVisible" [style]="{ width: '500px' }">
      <div class="form" *ngIf="selectedItem">
        <p class="move-info">Moving: <strong>{{ selectedItem.title }}</strong></p>
        <div class="field">
          <label>Sprint Name <span class="req">*</span></label>
          <input pInputText [(ngModel)]="sprintForm.sprintName" class="w-full" placeholder="e.g. Sprint 3" />
        </div>
        <div class="field assignee-field">
          <label>Assignee</label>
          <div class="assignee-wrapper">
            <input pInputText [(ngModel)]="assigneeInput" (input)="onAssigneeInput()" (focus)="onAssigneeFocus()" (blur)="onAssigneeBlur()" class="w-full" placeholder="Type @ to search users" autocomplete="off" />
            <div *ngIf="showAssigneeOverlay" class="assignee-overlay">
              <div *ngFor="let user of filteredAssigneeUsers()" class="assignee-option" (mousedown)="selectAssignee(user)">
                <span>{{ user.fullName }}</span>
                <small>{{ user.email }}</small>
              </div>
              <div *ngIf="filteredAssigneeUsers().length === 0" class="assignee-option no-results">No users found</div>
            </div>
          </div>
        </div>
        <div class="field-row">
          <div class="field">
            <label>Start Date</label>
            <p-datepicker [(ngModel)]="sprintForm.startDate" [minDate]="today" dateFormat="dd/mm/yy" styleClass="w-full" appendTo="body"></p-datepicker>
          </div>
          <div class="field">
            <label>End Date</label>
            <p-datepicker [(ngModel)]="sprintForm.endDate" [minDate]="today" dateFormat="dd/mm/yy" styleClass="w-full" appendTo="body"></p-datepicker>
          </div>
        </div>
        <div class="field">
          <label>Remarks</label>
          <textarea pTextarea [(ngModel)]="sprintForm.remarks" rows="2" class="w-full"></textarea>
        </div>
      </div>
      <ng-template pTemplate="footer">
        <button pButton label="Cancel" class="p-button-text" (click)="sprintDialogVisible = false"></button>
        <button pButton label="Move to Sprint" icon="pi pi-send" [loading]="movingToSprint()" (click)="moveToSprint()"></button>
      </ng-template>
    </p-dialog>

    <p-dialog header="Excel Upload" [modal]="true" [(visible)]="uploadVisible" [style]="{ width: '450px' }" (onShow)="onUploadDialogShow()">
      <p-fileUpload #fileUpload mode="basic" chooseLabel="Choose Excel File" accept=".xlsx,.xls" (onSelect)="onUpload($event)" [auto]="false"></p-fileUpload>
      <p style="margin-top:1rem;color:#64748b;font-size:0.9rem;">Upload an Excel file with backlog entries.</p>
      <ng-template pTemplate="footer">
        <button pButton label="Upload" [loading]="uploading()" (click)="confirmUpload()"></button>
        <button pButton label="Close" class="p-button-text" (click)="uploadVisible = false"></button>
      </ng-template>
    </p-dialog>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
    .page-header h2 { margin: 0; font-size: 1.5rem; color: #1e293b; }
    .header-actions { display: flex; gap: 0.5rem; }
    .filters { display: flex; gap: 0.75rem; margin-bottom: 1rem; }
    :host ::ng-deep .filter-select .p-select { min-width: 160px; }
    .form { display: flex; flex-direction: column; gap: 0.75rem; }
    .field-row { display: flex; gap: 0.75rem; }
    .field-row .field { flex: 1; }
    .field { display: flex; flex-direction: column; gap: 0.35rem; }
    .field label { font-size: 0.85rem; font-weight: 600; color: #374151; }
    .req { color: #ef4444; }
    .w-full { width: 100%; }
    .move-info { margin: 0 0 0.5rem; font-size: 0.9rem; color: #475569; }
    .assignee-field { position: relative; }
    .assignee-wrapper { position: relative; }
    .assignee-overlay {
      position: absolute;
      top: 100%;
      left: 0;
      right: 0;
      background: #fff;
      border: 1px solid #d1d5db;
      border-radius: 6px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
      max-height: 200px;
      overflow-y: auto;
      z-index: 1000;
    }
    .assignee-option {
      padding: 0.5rem 0.75rem;
      cursor: pointer;
    }
    .assignee-option:hover { background: #f1f5f9; }
    .assignee-option small { display: block; font-size: 0.75rem; color: #6b7280; }
    .assignee-option.no-results { cursor: default; color: #9ca3af; font-style: italic; }
  `]
})
export class BacklogComponent {
  private backlogService = inject(BacklogService);
  private sprintTaskService = inject(SprintTaskService);
  private priorityService = inject(PriorityService);
  private statusService = inject(StatusService);
  private deptService = inject(DepartmentService);
  private userService = inject(UserService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  filteredItems = signal<BacklogItem[]>([]);
  loading = signal(false);
  saving = signal(false);
  movingToSprint = signal(false);
  uploading = signal(false);
  dialogVisible = false;
  sprintDialogVisible = false;
  uploadVisible = false;
  selectedFile: File | null = null;
  @ViewChild('fileUpload') fileUpload!: FileUpload;

  filterPriorityId: number | null = null;
  filterStatusId: number | null = null;
  filterDeptId: number | null = null;

  priorityOpts = signal<PriorityDto[]>([]);
  statusOpts = signal<StatusDto[]>([]);
  deptOpts = signal<DepartmentDto[]>([]);
  assigneeOpts = signal<{ label: string; value: string }[]>([]);

  today = new Date();
  formItem: Partial<BacklogForm> = {};
  selectedItem: BacklogItem | null = null;
  sprintForm: SprintForm = { sprintName: '', assigneeId: null, startDate: null, endDate: null, remarks: '' };

  private toUTCDateString(date: Date | null): string | null {
    if (!date) return null;
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}T00:00:00.000Z`;
  }
  assigneeInput = '';
  showAssigneeOverlay = false;
  requestedByInput = '';
  showRequestedByOverlay = false;
  allUsers: UserDto[] = [];

  constructor() {
    afterNextRender(() => {
      this.loadReferenceData();
      this.loadItems();
    });
  }

  private loadReferenceData() {
    this.priorityService.getAll().subscribe(d => this.priorityOpts.set(d));
    this.statusService.getAll().subscribe(d => this.statusOpts.set(d));
    this.deptService.getAll().subscribe(d => this.deptOpts.set(d));
    this.userService.getAll().subscribe({
      next: users => {
        this.allUsers = users;
        this.assigneeOpts.set(users.map(u => ({ label: `${u.fullName} (${u.email})`, value: u.id })));
      },
      error: () => this.assigneeOpts.set([]),
    });
  }

  loadItems() {
    this.loading.set(true);
    this.backlogService.getAll(this.filterPriorityId, this.filterStatusId, this.filterDeptId).subscribe({
      next: data => {
        this.filteredItems.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  showCreate() {
    this.formItem = {};
    this.requestedByInput = '';
    this.showRequestedByOverlay = false;
    this.dialogVisible = true;
  }

  editItem(item: BacklogItem) {
    this.formItem = {
      id: item.id,
      title: item.title,
      description: item.description ?? '',
      requestedBy: item.requestedBy,
      gitlabLink: item.gitLabLink ?? '',
      remarks: item.remarks ?? '',
      priorityId: item.priorityId,
      statusId: item.statusId,
      departmentId: item.departmentId,
    };
    this.requestedByInput = item.requestedBy || '';
    this.showRequestedByOverlay = false;
    this.dialogVisible = true;
  }

  saveItem() {
    if (!this.formItem.title || !this.formItem.priorityId || !this.formItem.departmentId) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Title, Priority, and Department are required', key: 'br' });
      return;
    }
    this.saving.set(true);
    if (this.formItem.id) {
      this.backlogService.update(this.formItem.id, {
        title: this.formItem.title,
        description: this.formItem.description || null,
        requestedBy: this.formItem.requestedBy || 'Unknown',
        gitLabLink: this.formItem.gitlabLink || null,
        remarks: this.formItem.remarks || null,
        priorityId: this.formItem.priorityId,
        statusId: this.formItem.statusId ?? null,
        departmentId: this.formItem.departmentId,
      }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'Backlog item updated', key: 'br' });
          this.dialogVisible = false;
          this.saving.set(false);
          this.loadItems();
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update', key: 'br' });
          this.saving.set(false);
        },
      });
    } else {
      this.backlogService.create({
        title: this.formItem.title,
        description: this.formItem.description || null,
        requestedBy: this.formItem.requestedBy || 'Unknown',
        gitLabLink: this.formItem.gitlabLink || null,
        remarks: this.formItem.remarks || null,
        priorityId: this.formItem.priorityId,
        statusId: this.formItem.statusId ?? null,
        departmentId: this.formItem.departmentId,
      }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Created', detail: 'Backlog item created', key: 'br' });
          this.dialogVisible = false;
          this.saving.set(false);
          this.loadItems();
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to create', key: 'br' });
          this.saving.set(false);
        },
      });
    }
  }

  confirmDelete(item: BacklogItem) {
    this.confirmationService.confirm({
      message: `Delete backlog item "${item.title}"?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.backlogService.delete(item.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Backlog item deleted', key: 'br' });
            this.loadItems();
          },
          error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete', key: 'br' }),
        });
      },
    });
  }

  showMoveToSprint(item: BacklogItem) {
    this.selectedItem = item;
    this.sprintForm = { sprintName: '', assigneeId: null, startDate: null, endDate: null, remarks: '' };
    this.assigneeInput = '';
    this.showAssigneeOverlay = false;
    this.sprintDialogVisible = true;
  }

  onAssigneeInput() {
    const hasAt = this.assigneeInput.includes('@');
    if (hasAt && !this.showAssigneeOverlay) {
      this.showAssigneeOverlay = true;
    } else if (!hasAt) {
      this.showAssigneeOverlay = false;
    }
  }

  onAssigneeFocus() {
    if (this.assigneeInput.includes('@')) {
      this.showAssigneeOverlay = true;
    }
  }

  onAssigneeBlur() {
    setTimeout(() => {
      this.showAssigneeOverlay = false;
    }, 200);
  }

  selectAssignee(user: UserDto) {
    this.assigneeInput = user.fullName;
    this.sprintForm.assigneeId = user.id;
    this.showAssigneeOverlay = false;
  }

  onRequestedByInput() {
    const hasAt = this.requestedByInput.includes('@');
    if (hasAt && !this.showRequestedByOverlay) {
      this.showRequestedByOverlay = true;
    } else if (!hasAt) {
      this.showRequestedByOverlay = false;
    }
  }

  onRequestedByFocus() {
    if (this.requestedByInput.includes('@')) {
      this.showRequestedByOverlay = true;
    }
  }

  onRequestedByBlur() {
    setTimeout(() => {
      this.showRequestedByOverlay = false;
    }, 200);
  }

  selectRequestedBy(user: UserDto) {
    this.requestedByInput = user.fullName;
    this.formItem.requestedBy = user.fullName;
    this.showRequestedByOverlay = false;
  }

  filteredRequestedByUsers(): UserDto[] {
    if (!this.showRequestedByOverlay) return [];
    const atIndex = this.requestedByInput.lastIndexOf('@');
    if (atIndex === -1) return [];
    const query = this.requestedByInput.slice(atIndex + 1).toLowerCase();
    if (!query) return this.allUsers;
    return this.allUsers.filter(u =>
      u.fullName.toLowerCase().includes(query) ||
      u.email.toLowerCase().includes(query)
    );
  }

  filteredAssigneeUsers(): UserDto[] {
    if (!this.showAssigneeOverlay) return [];
    const atIndex = this.assigneeInput.lastIndexOf('@');
    if (atIndex === -1) return [];
    const query = this.assigneeInput.slice(atIndex + 1).toLowerCase();
    if (!query) return this.allUsers;
    return this.allUsers.filter(u =>
      u.fullName.toLowerCase().includes(query) ||
      u.email.toLowerCase().includes(query)
    );
  }

  moveToSprint() {
    if (!this.selectedItem || !this.sprintForm.sprintName) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Sprint name is required', key: 'br' });
      return;
    }
    this.movingToSprint.set(true);
    this.sprintTaskService.create({
      backlogTaskId: this.selectedItem.id,
      sprintName: this.sprintForm.sprintName,
      assigneeId: this.sprintForm.assigneeId,
      startDate: this.toUTCDateString(this.sprintForm.startDate),
      endDate: this.toUTCDateString(this.sprintForm.endDate),
      remarks: this.sprintForm.remarks || null,
    }).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Moved', detail: 'Task moved to sprint', key: 'br' });
        this.sprintDialogVisible = false;
        this.movingToSprint.set(false);
        this.loadItems();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to move to sprint', key: 'br' });
        this.movingToSprint.set(false);
      },
    });
  }

  downloadSample() {
    this.backlogService.downloadSample().subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'backlog_sample.xlsx';
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  onUpload(event: any) {
    this.selectedFile = event.files?.[0] || null;
    if (this.selectedFile) {
      this.messageService.add({ severity: 'info', summary: 'File selected', detail: this.selectedFile.name, key: 'br' });
    }
  }

  onUploadDialogShow() {
    this.selectedFile = null;
    this.fileUpload?.clear();
  }

  confirmUpload() {
    if (!this.selectedFile) {
      this.messageService.add({ severity: 'warn', summary: 'No file', detail: 'Please select an Excel file first', key: 'br' });
      return;
    }
    this.uploading.set(true);
    this.backlogService.bulkUpload(this.selectedFile).subscribe({
      next: (res) => {
        const skipped = res?.data?.skipped ?? 0;
        const inserted = res?.data?.inserted ?? 0;
        const detail = res?.message || `Inserted: ${inserted}, Skipped: ${skipped}`;
        if (skipped > 0) {
          this.messageService.add({ severity: 'warn', summary: 'Uploaded with warnings', detail: `${detail} — ${skipped} row(s) skipped (duplicate or invalid)`, key: 'br' });
        } else {
          this.messageService.add({ severity: 'success', summary: 'Uploaded', detail, key: 'br' });
        }
        this.uploadVisible = false;
        this.uploading.set(false);
        this.selectedFile = null;
        this.fileUpload?.clear();
        this.loadItems();
      },
      error: (err) => {
        const detail = err.error?.message || 'Bulk upload failed';
        this.messageService.add({ severity: 'error', summary: 'Upload Failed', detail, key: 'br' });
        this.uploading.set(false);
      },
    });
  }
}
