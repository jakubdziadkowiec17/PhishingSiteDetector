import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  constructor(private messageService: MessageService, private translateService: TranslateService) {}

  showSuccessToast(successKey: string): void {
    var summary = this.translateService.instant('SUCCESS.SUMMARY');
    var key = `SUCCESS.${successKey}`;
    
    var detail = this.translateService.instant(key);
    if (detail === key) {
      detail = this.translateService.instant('SUCCESS.OPERATION_COMPLETED_SUCCESSFULLY');
    }

    this.messageService.add({ severity: 'success', summary: summary, detail: detail, life: 5000 });
  }

  showErrorToast(errorKey: string): void {
    var summary = this.translateService.instant('ERROR.SUMMARY');
    var key = `ERROR.${errorKey}`;
    
    var detail = this.translateService.instant(key);
    if (detail === key) {
      detail = this.translateService.instant('ERROR.AN_UNKNOWN_ERROR_OCCURED');
    }

    this.messageService.add({ severity: 'error', summary: summary, detail: detail, life: 5000 });
  }
}