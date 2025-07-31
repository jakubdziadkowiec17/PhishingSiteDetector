import { Component } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { StepperModule } from 'primeng/stepper';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/common/notification-service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-home',
  imports: [CardModule, ButtonModule, TranslateModule, StepperModule, ButtonModule, InputTextModule, CommonModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class HomeComponent {

  constructor(private notificationService: NotificationService) {}

  activeStep: number = 1;

  showSuccess() {
    this.notificationService.showSuccessToast('PASSWORD_RESET');
  }

  showError() {
    this.notificationService.showErrorToast('YOUR_SESSION_HAS_EXPIRED');
  }
}
