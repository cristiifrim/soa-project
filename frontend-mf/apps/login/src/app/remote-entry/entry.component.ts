import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '@frontend-mf/data-access-user';
import { inject } from '@angular/core';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  selector: 'ng-mf-login-entry',
  templateUrl: './entry.component.html',
  styleUrls: ['./entry.component.css'],
})
export class RemoteEntryComponent {
  private userService = inject(UserService);
  private router = inject(Router);

  email = '';
  password = '';
  isLoggedIn$ = this.userService.isUserLoggedIn$;
  isRegisterMode = false;

  errorMessage: string | null = null;
  loading = false;

  toggleMode() {
    this.isRegisterMode = !this.isRegisterMode;
    this.errorMessage = null;
  }

  submit() {
    this.errorMessage = null;
    this.loading = true;

    if (this.isRegisterMode) {
      this.userService.signup(this.email, this.password).subscribe({
        next: () => {
          this.loading = false;
          this.router.navigateByUrl('/list-ad');
        },
        error: (err) => {
          this.loading = false;
          this.errorMessage = this.getErrorMessage(err, 'Registration failed. Please try again.');
        },
      });
    } else {
      this.userService.login(this.email, this.password).subscribe({
        next: () => {
          this.loading = false;
          this.router.navigateByUrl('/list-ad');
        },
        error: (err) => {
          this.loading = false;
          this.errorMessage = this.getErrorMessage(err, 'Login failed. Please check your email and password.');
        },
      });
    }
  }

  private getErrorMessage(err: { error?: { message?: string | string[] }; message?: string }, fallback: string): string {
    const m = err.error?.message;
    if (Array.isArray(m) && m.length) return m[0];
    if (typeof m === 'string') return m;
    return err.message || fallback;
  }
}
