import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonModule,
    InputTextModule,
    PasswordModule,
    ToastModule,
    RouterLink,
  ],
  template: `
    <div class="wrapper">
      <div class="container">
        <p-toast position="bottom-right" key="br"></p-toast>
        <div class="form-container">
          <div class="img-wrapper">
            <h3 style="font-weight:700;color:#495057;margin:0;letter-spacing:1px;">TMP</h3>
          </div>
          <div class="heading-text">
            <h4 class="font-bold">Create Account</h4>
            <h5 class="mt-2">Register to get started</h5>
          </div>
          <div class="form-wrap">
            <form [formGroup]="registerForm" (keydown.enter)="onSubmit()">
              <div class="form-group">
                <label for="email">Email</label>
                <input type="email" class="form-control" placeholder="Your Email" formControlName="email" />
                <small class="error" *ngIf="submitted() && registerForm.get('email')?.errors?.['required']">⚠️ Email is required</small>
                <small class="error" *ngIf="submitted() && registerForm.get('email')?.errors?.['email']">⚠️ Invalid email</small>
              </div>
              <div class="form-group">
                <label for="password">Password</label>
                <input type="password" class="form-control" placeholder="Create Password" formControlName="password" />
                <small class="error" *ngIf="submitted() && registerForm.get('password')?.errors?.['required']">⚠️ Password is required</small>
                <small class="error" *ngIf="submitted() && registerForm.get('password')?.errors?.['minlength']">⚠️ Min 8 characters</small>
              </div>
              <div class="form-group">
                <label for="confirmPassword">Confirm Password</label>
                <input type="password" class="form-control" placeholder="Confirm Password" formControlName="confirmPassword" />
                <small class="error" *ngIf="submitted() && registerForm.errors?.['mismatch']">⚠️ Passwords do not match</small>
              </div>
              <div class="btn-wrapper mt-6">
                <button pButton pRipple type="button" label="Register" class="p-button w-full button-primary-imark" [loading]="loading()" [disabled]="loading()" (click)="onSubmit()"></button>
              </div>
            </form>
            <div style="text-align:center;margin-top:1rem;">
              <a routerLink="/login" style="color:#2563eb;text-decoration:none;font-size:0.9rem;">Already have an account? Login</a>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .font-bold { font-weight: 700; }
    .mt-2 { margin-top: 0.5rem; }
    .mt-6 { margin-top: 1.5rem; }
    .w-full { width: 100%; }
    .wrapper {
      height: 100vh; display: flex; justify-content: center; align-items: center; background-color: #F8F8F8;
    }
    .container { width: 100%; max-width: 440px; }
    .form-container {
      background: #fff; border-radius: 16px; padding: 32px 16px; box-shadow: 0 20px 60px rgba(0,0,0,0.18);
    }
    .img-wrapper { text-align: center; margin-bottom: 0.5rem; }
    .heading-text { padding: 16px; margin-top: 8px; text-align: center; }
    .heading-text h5 { font-size: 16px; margin-bottom: 0; font-weight: 500; color: #495057; }
    .heading-text h4 { font-size: 18px; font-weight: 600; margin-bottom: 0; color: #495057; }
    .form-wrap { padding: 16px; }
    .form-group { margin-bottom: 12px; }
    .form-group label { display: block; font-size: 16px; font-weight: 500; margin-bottom: 4px; color: #374151; }
    .form-control {
      width: 100%; padding: 0.975rem 0.75rem; border: 1.5px solid #d1d5db; border-radius: 8px;
      font-size: 0.95rem; outline: none; transition: border-color 0.2s; box-sizing: border-box;
    }
    .form-control:focus { border-color: #2563eb; box-shadow: 0 0 0 3px rgba(37,99,235,0.15); }
    .error { display: block; margin-top: 0.25rem; font-size: 0.78rem; color: #e11d48; }
    .button-primary-imark {
      width: 100%; border-radius: 8px !important; padding: 0.7rem 1rem !important;
      font-size: 0.95rem !important; font-weight: 600 !important;
      background: #2563eb !important; border-color: #2563eb !important;
    }
    .button-primary-imark:hover:not(:disabled) { background: #1d4ed8 !important; border-color: #1d4ed8 !important; }
  `]
})
export class RegisterComponent implements OnInit {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private messageService = inject(MessageService);

  registerForm!: FormGroup;
  submitted = signal(false);
  loading = signal(false);

  ngOnInit() {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required],
    }, { validators: this.passwordsMatch });
  }

  passwordsMatch(g: FormGroup) {
    return g.get('password')?.value === g.get('confirmPassword')?.value ? null : { mismatch: true };
  }

  onSubmit() {
    this.submitted.set(true);
    if (this.registerForm.invalid) return;

    this.loading.set(true);
    const { email, password } = this.registerForm.value;
    this.auth.register(email, password).subscribe({
      next: () => {
        this.loading.set(false);
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Account created. Please login.', key: 'br' });
        setTimeout(() => this.router.navigate(['/login']), 1000);
      },
      error: () => this.loading.set(false),
    });
  }
}
