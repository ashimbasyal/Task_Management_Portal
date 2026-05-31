import { Component, inject, signal } from '@angular/core';
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
import { MasterDataService, MasterDataEntry } from '../../core/services/master-data.service';

interface MasterConfig {
  type: number;
  label: string;
  values: MasterDataEntry[];
}

const MASTER_TYPES = [
  { type: 1, label: 'Status' },
  { type: 2, label: 'Priority' },
  { type: 3, label: 'Sprint Status Trigger' },
  { type: 4, label: 'Assignee' },
];

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
        <div class="config-card" *ngFor="let config of configs()">
          <div class="config-header">
            <h3>{{ config.label }}</h3>
            <button pButton icon="pi pi-plus" class="p-button-rounded p-button-text" (click)="openAdd(config)"></button>
          </div>
          <ul class="value-list">
            <li *ngFor="let entry of config.values">
              <span>{{ entry.value }}</span>
              <div>
                <button pButton icon="pi pi-pencil" class="p-button-rounded p-button-text" (click)="openEdit(config, entry)"></button>
                <button pButton icon="pi pi-trash" class="p-button-rounded p-button-text p-button-danger" (click)="confirmDelete(config, entry)"></button>
              </div>
            </li>
            <li *ngIf="config.values.length === 0" class="empty">No entries</li>
          </ul>
        </div>
      </div>
    </div>

    <p-dialog [header]="editingEntry ? 'Edit Value' : 'Add Value'" [modal]="true" [(visible)]="dialogVisible" [style]="{ width: '400px' }">
      <div class="field">
        <label>Value</label>
        <input pInputText [(ngModel)]="newValue" class="w-full" placeholder="Enter value" />
      </div>
      <ng-template pTemplate="footer">
        <button pButton label="Cancel" class="p-button-text" (click)="dialogVisible = false"></button>
        <button pButton [label]="editingEntry ? 'Update' : 'Add'" [disabled]="!newValue.trim()" [loading]="saving()" (click)="saveValue()"></button>
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
    .value-list li.empty { justify-content: center; color: #94a3b8; font-style: italic; }
    .field { display: flex; flex-direction: column; gap: 0.35rem; }
    .field label { font-size: 0.85rem; font-weight: 600; color: #374151; }
    .w-full { width: 100%; }
  `]
})
export class MasterDataComponent {
  private masterDataService = inject(MasterDataService);
  private confirmationService = inject(ConfirmationService);
  private messageService = inject(MessageService);

  configs = signal<MasterConfig[]>([]);
  dialogVisible = false;
  saving = signal(false);
  newValue = '';
  activeConfig: MasterConfig | null = null;
  editingEntry: MasterDataEntry | null = null;

  constructor() {
    this.loadAll();
  }

  private loadAll() {
    const configs: MasterConfig[] = MASTER_TYPES.map(t => ({ ...t, values: [] }));
    this.configs.set(configs);
    for (const cfg of configs) {
      this.masterDataService.getByType(cfg.type).subscribe(entries => {
        cfg.values = entries;
        this.configs.set([...configs]);
      });
    }
  }

  openAdd(config: MasterConfig) {
    this.activeConfig = config;
    this.editingEntry = null;
    this.newValue = '';
    this.dialogVisible = true;
  }

  openEdit(config: MasterConfig, entry: MasterDataEntry) {
    this.activeConfig = config;
    this.editingEntry = entry;
    this.newValue = entry.value;
    this.dialogVisible = true;
  }

  saveValue() {
    if (!this.activeConfig || !this.newValue.trim()) return;
    this.saving.set(true);
    if (this.editingEntry) {
      this.masterDataService.update(this.editingEntry.id, {
        value: this.newValue.trim(),
        displayOrder: this.editingEntry.displayOrder,
      }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'Value updated', key: 'br' });
          this.dialogVisible = false;
          this.saving.set(false);
          this.loadAll();
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update value', key: 'br' });
          this.saving.set(false);
        },
      });
    } else {
      this.masterDataService.create({
        type: this.activeConfig.type,
        value: this.newValue.trim(),
        displayOrder: this.activeConfig.values.length + 1,
      }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Added', detail: 'Value added', key: 'br' });
          this.dialogVisible = false;
          this.saving.set(false);
          this.loadAll();
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to add value', key: 'br' });
          this.saving.set(false);
        },
      });
    }
  }

  confirmDelete(config: MasterConfig, entry: MasterDataEntry) {
    this.confirmationService.confirm({
      message: `Delete "${entry.value}" from ${config.label}?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.masterDataService.delete(entry.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Value removed', key: 'br' });
            this.loadAll();
          },
          error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete', key: 'br' }),
        });
      },
    });
  }
}
