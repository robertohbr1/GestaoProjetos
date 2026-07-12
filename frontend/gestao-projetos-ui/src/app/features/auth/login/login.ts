import { Component, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    MatSelectModule
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class LoginComponent {
  loginForm: FormGroup;
  registerForm: FormGroup;
  isRegister = signal(false);
  isLoading = signal(false);

  roles = [
    { value: 'Administrator', label: 'Administrador' },
    { value: 'Developer', label: 'Desenvolvedor' },
    { value: 'Collaborator', label: 'Colaborador' }
  ];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      role: ['Collaborator', Validators.required]
    });
  }

  toggleMode(): void {
    this.isRegister.update(v => !v);
  }

  onSubmitLogin(): void {
    if (this.loginForm.invalid) return;

    this.isLoading.set(true);
    const { username, password } = this.loginForm.value;

    this.authService.login(username, password).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.snackBar.open('Login efetuado com sucesso!', 'Fechar', { duration: 3000 });
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.snackBar.open(err || 'Erro ao realizar login.', 'Fechar', { duration: 3000 });
      }
    });
  }

  onSubmitRegister(): void {
    if (this.registerForm.invalid) return;

    this.isLoading.set(true);
    const { username, email, password, role } = this.registerForm.value;

    this.authService.register(username, email, password, role).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.snackBar.open('Usuário registrado! Faça login.', 'Fechar', { duration: 4000 });
        this.isRegister.set(false);
        this.loginForm.patchValue({ username });
      },
      error: (err) => {
        this.isLoading.set(false);
        this.snackBar.open(err || 'Erro ao registrar usuário.', 'Fechar', { duration: 3000 });
      }
    });
  }
}
