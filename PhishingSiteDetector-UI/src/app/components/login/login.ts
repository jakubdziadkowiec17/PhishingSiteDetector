import { NgClass, NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { LoginDTO } from '../../interfaces/login-dto';
import { AccountApiService } from '../../services/api/account-api-service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { MessageModule } from 'primeng/message';
import { FloatLabel } from 'primeng/floatlabel';
import { CardModule } from 'primeng/card';
import { AccountService } from '../../services/common/account-service';
import { finalize } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-login',
  imports: [NgIf, NgClass, ReactiveFormsModule, TranslateModule, InputTextModule, PasswordModule, ButtonModule, CardModule, MessageModule, FloatLabel],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent implements OnInit {
  loadingLogin = false;
  loginForm!: FormGroup;
  redirectUrl: string = '';

  constructor(private fb: FormBuilder, private accountApiService: AccountApiService, private accountService: AccountService, private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });

    this.route.queryParamMap.subscribe(params => {
      const redirectParam = params.get('redirect');
      this.redirectUrl = redirectParam ?? '';
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loadingLogin = true;
    const loginDTO: LoginDTO = this.loginForm.value;

    this.accountApiService.login(loginDTO).pipe(
      finalize(() => this.loadingLogin = false)
    ).subscribe({
      next: (tokens) => {
        this.accountService.setAccessToken(tokens.accessToken);
        this.accountService.setRefreshToken(tokens.refreshToken);
        this.router.navigateByUrl(this.redirectUrl);
      },
      error: () => {
        this.loginForm.reset();
      }
    });
  }
}