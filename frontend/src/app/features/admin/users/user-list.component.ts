import { Component, inject, OnInit, signal, afterNextRender } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { FormsModule } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { UserService, UserDto, CreateUserRequest } from '../../../core/services/user.service';
import { DepartmentService, DepartmentDto } from '../../../core/services/department.service';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    TableModule,
    ButtonModule,
    DialogModule,
    SelectModule,
    InputTextModule,
    PasswordModule,
    ToggleSwitchModule,
    ToastModule,
    ConfirmDialogModule,
    TooltipModule,
  ],
  providers: [MessageService, ConfirmationService],
  template: `
    <p-toast position="bottom-right" key="br"></p-toast>
    <p-confirmDialog [style]="{ width: '450px' }"></p-confirmDialog>

    <div class="page-header">
      <h2>User Management</h2>
      @if (ready()) {
      <p-button label="Create User" icon="pi pi-plus" (onClick)="showCreateDialog()"></p-button>
      }
    </div>

    <p-table [value]="users()" [paginator]="true" [rows]="10" [loading]="loading()"
      styleClass="p-datatable-striped" [tableStyle]="{ 'min-width': '50rem' }">
      <ng-template pTemplate="header">
        <tr>
          <th>Full Name</th>
          <th>Email</th>
          <th>Role</th>
          <th>Department</th>
          <th>View All</th>
          <th>Actions</th>
        </tr>
      </ng-template>
      <ng-template pTemplate="body" let-user>
        <tr>
          <td>{{ user.fullName }}</td>
          <td>{{ user.email }}</td>
          <td><span class="role-tag" [class.admin]="user.role === 1" [class.dev]="user.role === 2" [class.officer]="user.role === 3">{{ roleLabel(user.role) }}</span></td>
          <td>{{ user.departmentName || '-' }}</td>
          <td>
            <span [pTooltip]="user.role === 3 ? 'Allow viewing tasks across all departments' : 'Only Officer users can toggle this permission'" tooltipPosition="top">
              <p-toggleswitch [ngModel]="user.canViewAllDepartments" (onChange)="togglePermission(user, $event.checked)" [disabled]="user.role !== 3" />
            </span>
          </td>
          <td>
            <button pButton icon="pi pi-trash" class="p-button-rounded p-button-text p-button-danger" (click)="confirmDelete(user)" [disabled]="user.id === currentUserId" [pTooltip]="user.id === currentUserId ? 'Cannot delete yourself' : ''"></button>
          </td>
        </tr>
      </ng-template>
      <ng-template pTemplate="emptymessage">
        <tr>
          <td colspan="6" style="text-align:center;padding:2rem;">No users found</td>
        </tr>
      </ng-template>
    </p-table>

    <p-dialog header="Create User" [modal]="true" [(visible)]="dialogVisible" [style]="{ width: '450px' }">
      <form [formGroup]="userForm">
        <div class="field">
          <label for="fullName">Full Name</label>
          <input id="fullName" pInputText formControlName="fullName" class="w-full" />
          <small class="error" *ngIf="submitted() && userForm.get('fullName')?.errors?.['required']">Required</small>
        </div>
        <div class="field">
          <label for="email">Email</label>
          <input id="email" pInputText formControlName="email" class="w-full" />
          <small class="error" *ngIf="submitted() && userForm.get('email')?.errors?.['required']">Required</small>
          <small class="error" *ngIf="submitted() && userForm.get('email')?.errors?.['email']">Invalid email</small>
        </div>
        <div class="field">
          <label for="password">Password</label>
          <p-password id="password" formControlName="password" [feedback]="true" [toggleMask]="true" styleClass="w-full" inputStyleClass="w-full"></p-password>
          <small class="error" *ngIf="submitted() && userForm.get('password')?.errors?.['required']">Required</small>
          <small class="error" *ngIf="submitted() && userForm.get('password')?.errors?.['minlength']">Min 8 characters</small>
        </div>
        <div class="field">
          <label for="role">Role</label>
          <p-select id="role" [options]="roles" formControlName="role" optionLabel="label" optionValue="value" placeholder="Select Role" styleClass="w-full" appendTo="body"></p-select>
          <small class="error" *ngIf="submitted() && userForm.get('role')?.errors?.['required']">Required</small>
        </div>
        <div class="field" *ngIf="userForm.get('role')?.value === 3">
          <label for="department">Department</label>
          <p-select id="department" [options]="departments()" formControlName="departmentId" optionLabel="name" optionValue="id" placeholder="Select Department" styleClass="w-full" appendTo="body"></p-select>
          <small class="error" *ngIf="submitted() && userForm.get('departmentId')?.errors?.['required']">Required for Officer</small>
        </div>
      </form>
      <ng-template pTemplate="footer">
        <button pButton label="Cancel" class="p-button-text" (click)="dialogVisible = false"></button>
        <button pButton label="Create" [loading]="saving()" (click)="onCreate()"></button>
      </ng-template>
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
    .role-tag {
      display: inline-block;
      padding: 0.2rem 0.6rem;
      border-radius: 4px;
      font-size: 0.8rem;
      font-weight: 600;
    }
    .role-tag.admin { background: #dbeafe; color: #1d4ed8; }
    .role-tag.dev { background: #d1fae5; color: #059669; }
    .role-tag.officer { background: #fef3c7; color: #d97706; }
    .field {
      margin-bottom: 1rem;
    }
    .field label {
      display: block;
      font-size: 0.85rem;
      font-weight: 600;
      color: #374151;
      margin-bottom: 0.35rem;
    }
    .field .error {
      display: block;
      margin-top: 0.25rem;
      font-size: 0.78rem;
      color: #e11d48;
    }
  `]
})
export class UserListComponent implements OnInit {
  private userService = inject(UserService);
  private departmentService = inject(DepartmentService);
  private fb = inject(FormBuilder);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private auth = inject(AuthService);

  currentUserId = this.auth.getUserId();

  ready = signal(false);

  constructor() {
    afterNextRender(() => this.ready.set(true));
  }

  users = signal<UserDto[]>([]);
  departments = signal<DepartmentDto[]>([]);
  loading = signal(false);
  saving = signal(false);
  submitted = signal(false);
  dialogVisible = false;

  roles = [
    { label: 'Admin', value: 1 },
    { label: 'Developer', value: 2 },
    { label: 'Officer', value: 3 },
  ];

  userForm: FormGroup = this.fb.group({
    fullName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    role: [null, Validators.required],
    departmentId: [null],
  });

  ngOnInit() {
    this.loadUsers();
    this.departmentService.getAll().subscribe(deps => this.departments.set(deps));
  }

  roleLabel(role: number): string {
    return this.roles.find(r => r.value === role)?.label || '';
  }

  loadUsers() {
    this.loading.set(true);
    this.userService.getAll().subscribe({
      next: data => { this.users.set(data); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  showCreateDialog() {
    this.userForm.reset();
    this.submitted.set(false);
    this.dialogVisible = true;
  }

  onCreate() {
    this.submitted.set(true);
    if (this.userForm.invalid) return;

    this.saving.set(true);
    const data: CreateUserRequest = {
      fullName: this.userForm.value.fullName,
      email: this.userForm.value.email,
      password: this.userForm.value.password,
      role: this.userForm.value.role,
      departmentId: this.userForm.value.role === 3 ? this.userForm.value.departmentId : null,
    };

    this.userService.create(data).subscribe({
      next: () => {
        this.saving.set(false);
        this.dialogVisible = false;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'User created', key: 'br' });
        this.loadUsers();
      },
      error: () => this.saving.set(false),
    });
  }

  togglePermission(user: UserDto, canViewAll: boolean) {
    const previous = user.canViewAllDepartments;
    user.canViewAllDepartments = canViewAll;
    this.userService.updatePermission(user.id, { canViewAllDepartments: canViewAll }).subscribe({
      next: updated => {
        this.users.update(list => list.map(u => u.id === updated.id ? updated : u));
        this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'Permission updated', key: 'br' });
      },
      error: () => {
        user.canViewAllDepartments = previous;
        this.users.update(list => [...list]);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update permission', key: 'br' });
      },
    });
  }

  confirmDelete(user: UserDto) {
    this.confirmationService.confirm({
      message: `Delete user "${user.fullName}"?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.userService.delete(user.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'User deleted', key: 'br' });
            this.loadUsers();
          },
        });
      },
    });
  }
}
