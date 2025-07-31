import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  constructor(private messageService: MessageService, private translateService: TranslateService) {}

  showSuccessToast(detailKey: string = ''): void {
    const summary = this.translateService.instant('SUCCESS.SUMMARY');
    const detail = detailKey ? this.translateService.instant(`SUCCESS.${detailKey}`) : '';
    this.messageService.add({ severity: 'success', summary: summary, detail: detail, life: 5000 });
  }

  showErrorToast(detailKey: string = ''): void {
    const summary = this.translateService.instant('ERROR.SUMMARY');
    const detail = detailKey ? this.translateService.instant(`ERROR.${detailKey}`) : '';
    this.messageService.add({ severity: 'error', summary: summary, detail: detail, life: 5000 });
  }
}