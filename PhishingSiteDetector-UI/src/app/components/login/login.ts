import { NgClass } from '@angular/common';
import { Component } from '@angular/core';
import { LoginDTO } from '../../interfaces/login-dto';
import { AccountApiService } from '../../services/api/account-api-service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { MessageModule } from 'primeng/message';
import { CardModule } from 'primeng/card';
import { AccountService } from '../../services/common/account-service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-login',
  imports: [NgClass, ReactiveFormsModule, InputTextModule, PasswordModule, ButtonModule, CardModule, MessageModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  loading = false;
  loginError = false;
  loginForm!: FormGroup;
  redirectUrl: string = '';

  constructor(private fb: FormBuilder, private accountApiService: AccountApiService, private accountService: AccountService, private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });

    const redirectParam = this.route.snapshot.queryParamMap.get('redirect');
    if (redirectParam) this.redirectUrl = redirectParam;
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.loading = true;
    this.loginError = false;
    const loginDTO: LoginDTO = this.loginForm.value;

    this.accountApiService.login(loginDTO).pipe(
      finalize(() => this.loading = false)
    ).subscribe({
      next: (tokens) => {
        this.accountService.setAccessToken(tokens.accessToken);
        this.accountService.setRefreshToken(tokens.refreshToken);
        this.router.navigateByUrl(this.redirectUrl);
      },
      error: () => {
        this.loginError = true;
      }
    });
  }
}