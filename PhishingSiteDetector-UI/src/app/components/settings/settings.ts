import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AccountApiService } from '../../services/api/account-api-service';
import { finalize } from 'rxjs';
import { NgIf, NgClass } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MessageModule } from 'primeng/message';
import { FloatLabel } from 'primeng/floatlabel';
import { TabsModule } from 'primeng/tabs';
import { TranslateModule } from '@ngx-translate/core';
import { NotificationService } from '../../services/common/notification-service';
import { AccountService } from '../../services/common/account-service';
import { PasswordModule } from 'primeng/password';

@Component({
  selector: 'app-settings',
  imports: [NgIf, NgClass, ReactiveFormsModule, TranslateModule, InputTextModule, ButtonModule, CardModule, MessageModule, FloatLabel, TabsModule, PasswordModule],
  templateUrl: './settings.html',
  styleUrl: './settings.css'
})
export class SettingsComponent implements OnInit {
  editAccountForm!: FormGroup;
  resetPasswordForm!: FormGroup;
  loadingEdit = false;
  loadingReset = false;

  constructor(private fb: FormBuilder, private accountApiService: AccountApiService, private accountService: AccountService, private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.editAccountForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(30)]],
      lastName: ['', [Validators.required, Validators.maxLength(30)]],
      email: ['', [Validators.required, Validators.email]],
      dateOfBirth: ['', Validators.required],
      address: ['', [Validators.required, Validators.maxLength(100)]],
      areaCode: [null, [Validators.required, Validators.min(1), Validators.max(9999)]],
      phoneNumber: [null, [Validators.required, Validators.min(100000000), Validators.max(999999999)]]
    });

    this.resetPasswordForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', Validators.required],
      confirmNewPassword: ['', Validators.required]
    });

    this.accountApiService.getAccount().subscribe({
      next: (account) => {
        this.editAccountForm.patchValue(account);

        const current = this.accountService.account;
        if (current) {
          const updatedAccount = { ...current, firstName: account.firstName };
          this.accountService.setAccount(updatedAccount);
        }
      }
    });
  }

  editAccount(): void {
    if (this.editAccountForm.invalid) {
      this.editAccountForm.markAllAsTouched();
      return;
    }

    this.loadingEdit = true;
    this.accountApiService.editAccount(this.editAccountForm.value).pipe(
      finalize(() => this.loadingEdit = false)
    ).subscribe({
      next: (response) => {
        this.notificationService.showSuccessToast(response.message);

        const currentAccount = this.accountService.account;
        if (currentAccount) {
          const updatedAccount = { ...currentAccount, firstName: this.editAccountForm.value.firstName };
          this.accountService.setAccount(updatedAccount);
        }
      }
    });
  }

  resetPassword(): void {
    if (this.resetPasswordForm.invalid) {
      this.resetPasswordForm.markAllAsTouched();
      return;
    }

    this.loadingReset = true;
    this.accountApiService.resetPassword(this.resetPasswordForm.value).pipe(
      finalize(() => this.loadingReset = false)
    ).subscribe({
      next: (response) => {
        this.notificationService.showSuccessToast(response.message);
        this.resetPasswordForm.reset();
      }
    });
  }
}