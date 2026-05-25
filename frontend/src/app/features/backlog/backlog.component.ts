import { Component, inject, signal, afterNextRender } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { FileUploadModule } from 'primeng/fileupload';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmationService, MessageService } from 'primeng/api';

interface BacklogItem {
  sn: number;
  title: string;
  description: string;
  requestedBy: string;
  gitlabLink: string;
  priority: string;
  remarks: string;
  status: string;
  department: string;
}

@Component({
  selector: 'app-backlog',
  standalone: true,
  imports: [
    CommonModule, FormsModule, TableModule, ButtonModule, DialogModule,
    SelectModule, InputTextModule, TextareaModule, FileUploadModule, TagModule,
    ToastModule, ConfirmDialogModule, TooltipModule,
  ],
  providers: [MessageService, ConfirmationService],
  template: `
    <p-toast position="bottom-right" key="br"></p-toast>
    <p-confirmDialog [style]="{ width: '450px' }"></p-confirmDialog>

    <div class="page-header">
      <h2>Backlog Management</h2>
        @if (ready()) {
        <div class="header-actions">
        <p-button label="Excel Upload" icon="pi pi-upload" severity="secondary" (onClick)="uploadVisible = true" styleClass="p-button-outlined"></p-button>
        <p-button label="Create Backlog" icon="pi pi-plus" (onClick)="showCreate()"></p-button>
      </div>
      }
    </div>

    <div class="filters">
      <p-select [options]="priorityOptions" placeholder="Priority" [(ngModel)]="filterPriority" (onChange)="applyFilters()" appendTo="body" styleClass="filter-select"></p-select>
      <p-select [options]="statusOptions" placeholder="Status" [(ngModel)]="filterStatus" (onChange)="applyFilters()" appendTo="body" styleClass="filter-select"></p-select>
      <p-select [options]="deptOptions" placeholder="Department" [(ngModel)]="filterDept" (onChange)="applyFilters()" appendTo="body" styleClass="filter-select"></p-select>
    </div>

    <p-table [value]="filteredItems()" [paginator]="true" [rows]="10" [loading]="loading()"
      styleClass="p-datatable-striped" [tableStyle]="{ 'min-width': '70rem' }">
      <ng-template pTemplate="header">
        <tr>
          <th>SN</th><th>Title</th><th>Priority</th><th>Status</th><th>Department</th><th>Requested By</th><th>Actions</th>
        </tr>
      </ng-template>
      <ng-template pTemplate="body" let-item>
        <tr>
          <td>{{ item.sn }}</td>
          <td>{{ item.title }}</td>
          <td><p-tag [value]="item.priority" [severity]="item.priority === 'High' ? 'danger' : item.priority === 'Medium' ? 'warn' : 'success'"></p-tag></td>
          <td>{{ item.status }}</td>
          <td>{{ item.department }}</td>
          <td>{{ item.requestedBy }}</td>
          <td>
            <button pButton icon="pi pi-pencil" class="p-button-rounded p-button-text" (click)="editItem(item)" pTooltip="Edit"></button>
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
            <p-select [options]="priorityOptions" [(ngModel)]="formItem.priority" placeholder="Select" appendTo="body" styleClass="w-full"></p-select>
          </div>
          <div class="field">
            <label>Department <span class="req">*</span></label>
            <p-select [options]="deptOptions" [(ngModel)]="formItem.department" placeholder="Select" appendTo="body" styleClass="w-full"></p-select>
          </div>
        </div>
        <div class="field-row">
          <div class="field">
            <label>Status</label>
            <p-select [options]="statusOptions" [(ngModel)]="formItem.status" placeholder="Select" appendTo="body" styleClass="w-full"></p-select>
          </div>
          <div class="field">
            <label>Requested By</label>
            <input pInputText [(ngModel)]="formItem.requestedBy" class="w-full" />
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

    <p-dialog header="Excel Upload" [modal]="true" [(visible)]="uploadVisible" [style]="{ width: '450px' }">
      <p-fileUpload mode="basic" chooseLabel="Choose Excel File" accept=".xlsx,.xls" (onSelect)="onUpload($event)"></p-fileUpload>
      <p style="margin-top:1rem;color:#64748b;font-size:0.9rem;">Upload an Excel file with backlog entries. The file should match the system fields.</p>
      <ng-template pTemplate="footer">
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
  `]
})
export class BacklogComponent {
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  ready = signal(false);

  items: BacklogItem[] = [
    { sn: 1, title: 'User authentication flow', description: 'Implement JWT login flow', requestedBy: 'Admin', gitlabLink: 'https://gitlab.com/proj/1', priority: 'High', remarks: '', status: 'Open', department: 'Engineering' },
    { sn: 2, title: 'Dashboard charts', description: 'Build dynamic charts', requestedBy: 'Admin', gitlabLink: 'https://gitlab.com/proj/2', priority: 'Medium', remarks: '', status: 'In Progress', department: 'Engineering' },
    { sn: 3, title: 'Excel export feature', description: 'Allow exporting reports', requestedBy: 'Officer', gitlabLink: 'https://gitlab.com/proj/3', priority: 'Low', remarks: '', status: 'Open', department: 'QA' },
    { sn: 4, title: 'Sprint planning board', description: 'Drag and drop sprint board', requestedBy: 'Admin', gitlabLink: 'https://gitlab.com/proj/4', priority: 'High', remarks: '', status: 'Completed', department: 'Engineering' },
    { sn: 5, title: 'Notification system', description: 'Email and in-app notifications', requestedBy: 'Developer', gitlabLink: 'https://gitlab.com/proj/5', priority: 'Medium', remarks: '', status: 'On Hold', department: 'Engineering' },
  ];

  filteredItems = signal<BacklogItem[]>([]);
  loading = signal(false);
  saving = signal(false);
  dialogVisible = false;
  uploadVisible = false;
  filterPriority: string | null = null;
  filterStatus: string | null = null;
  filterDept: string | null = null;

  priorityOptions = ['High', 'Medium', 'Low'];
  statusOptions = ['Open', 'In Progress', 'Completed', 'On Hold'];
  deptOptions = ['Engineering', 'QA', 'Support'];

  formItem: Partial<BacklogItem> = {};
  private nextSn = 6;

  constructor() {
    afterNextRender(() => this.ready.set(true));
    this.applyFilters();
  }

  showCreate() {
    this.formItem = {};
    this.dialogVisible = true;
  }

  editItem(item: BacklogItem) {
    this.formItem = { ...item };
    this.dialogVisible = true;
  }

  saveItem() {
    if (!this.formItem.title || !this.formItem.priority || !this.formItem.department) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Title, Priority, and Department are required', key: 'br' });
      return;
    }
    this.saving.set(true);
    const idx = this.items.findIndex(i => i.sn === this.formItem.sn);
    if (idx >= 0) {
      this.items[idx] = { ...this.formItem as BacklogItem };
      this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'Backlog item updated', key: 'br' });
    } else {
      this.formItem.sn = this.nextSn++;
      this.items.push({ ...this.formItem as BacklogItem });
      this.messageService.add({ severity: 'success', summary: 'Created', detail: 'Backlog item created', key: 'br' });
    }
    this.applyFilters();
    this.dialogVisible = false;
    this.saving.set(false);
  }

  confirmDelete(item: BacklogItem) {
    this.confirmationService.confirm({
      message: `Delete backlog item "${item.title}"?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.items = this.items.filter(i => i.sn !== item.sn);
        this.applyFilters();
        this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Backlog item deleted', key: 'br' });
      },
    });
  }

  applyFilters() {
    this.loading.set(true);
    this.filteredItems.set(this.items.filter(i =>
      (!this.filterPriority || i.priority === this.filterPriority) &&
      (!this.filterStatus || i.status === this.filterStatus) &&
      (!this.filterDept || i.department === this.filterDept)
    ));
    this.loading.set(false);
  }

  onUpload(event: any) {
    this.messageService.add({ severity: 'info', summary: 'Uploaded', detail: `File "${event.files?.[0]?.name}" selected`, key: 'br' });
    this.uploadVisible = false;
  }
}
