import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';

interface MasterConfig {
  type: string;
  values: string[];
}

@Component({
  selector: 'app-master-data',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ButtonModule, DialogModule, InputTextModule,
    SelectModule, TableModule, ConfirmDialogModule, ToastModule,
  ],
  providers: [ConfirmationService, MessageService],
  template: `
    <p-toast position="bottom-right" key="br"></p-toast>
    <p-confirmDialog [style]="{ width: '450px' }"></p-confirmDialog>

    <div class="page">
      <div class="page-header">
        <h2>Master Data Configuration</h2>
      </div>

      <div class="config-grid">
        <div class="config-card" *ngFor="let config of configs">
          <div class="config-header">
            <h3>{{ config.type }}</h3>
            <button pButton icon="pi pi-plus" class="p-button-rounded p-button-text" (click)="openAdd(config)"></button>
          </div>
          <ul class="value-list">
            <li *ngFor="let val of config.values">
              <span>{{ val }}</span>
              <button pButton icon="pi pi-trash" class="p-button-rounded p-button-text p-button-danger" (click)="confirmDelete(config, val)"></button>
            </li>
          </ul>
        </div>
      </div>
    </div>

    <p-dialog header="Add Value" [modal]="true" [(visible)]="dialogVisible" [style]="{ width: '400px' }">
      <div class="field">
        <label>Value</label>
        <input pInputText [(ngModel)]="newValue" class="w-full" placeholder="Enter value" />
      </div>
      <ng-template pTemplate="footer">
        <button pButton label="Cancel" class="p-button-text" (click)="dialogVisible = false"></button>
        <button pButton label="Add" [disabled]="!newValue.trim()" (click)="addValue()"></button>
      </ng-template>
    </p-dialog>
  `,
  styles: [`
    .page { max-width: 1200px; }
    .page-header { margin-bottom: 1.5rem; }
    .page-header h2 { margin: 0; font-size: 1.5rem; color: #1e293b; }
    .config-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 1rem; }
    .config-card { background: #fff; border-radius: 12px; padding: 1.25rem; box-shadow: 0 1px 3px rgba(0,0,0,0.08); }
    .config-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 0.75rem; }
    .config-header h3 { margin: 0; font-size: 1rem; color: #1e293b; }
    .value-list { list-style: none; padding: 0; margin: 0; display: flex; flex-direction: column; gap: 0.25rem; }
    .value-list li {
      display: flex; justify-content: space-between; align-items: center;
      padding: 0.5rem 0.75rem; border-radius: 6px; background: #f8fafc;
      font-size: 0.9rem; color: #334155;
    }
    .field { display: flex; flex-direction: column; gap: 0.35rem; }
    .field label { font-size: 0.85rem; font-weight: 600; color: #374151; }
    .w-full { width: 100%; }
  `]
})
export class MasterDataComponent {
  private confirmationService = inject(ConfirmationService);
  private messageService = inject(MessageService);

  configs: MasterConfig[] = [
    { type: 'Status', values: ['Open', 'In Progress', 'Completed', 'On Hold', 'Closed'] },
    { type: 'Priority', values: ['High', 'Medium', 'Low'] },
    { type: 'Department', values: ['Engineering', 'Quality Assurance', 'Support', 'Human Resources'] },
    { type: 'Sprint Status Trigger', values: ['Completed', 'Closed'] },
  ];

  dialogVisible = false;
  newValue = '';
  activeConfig: MasterConfig | null = null;

  openAdd(config: MasterConfig) {
    this.activeConfig = config;
    this.newValue = '';
    this.dialogVisible = true;
  }

  addValue() {
    if (!this.activeConfig || !this.newValue.trim()) return;
    this.activeConfig.values.push(this.newValue.trim());
    this.messageService.add({ severity: 'success', summary: 'Added', detail: 'Value added successfully', key: 'br' });
    this.dialogVisible = false;
  }

  confirmDelete(config: MasterConfig, value: string) {
    this.confirmationService.confirm({
      message: `Delete "${value}" from ${config.type}?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        config.values = config.values.filter(v => v !== value);
        this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Value removed', key: 'br' });
      },
    });
  }
}
