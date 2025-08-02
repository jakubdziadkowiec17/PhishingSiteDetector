import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AccountDataDTO } from '../../interfaces/account-data-dto';
import { AccountApiService } from '../api/account-api-service';
import { SessionService } from '../common/session-service';

@Injectable({ providedIn: 'root' })
export class AccountStoreService {
  private accountSubject = new BehaviorSubject<AccountDataDTO | null>(null);
  public account$: Observable<AccountDataDTO | null> = this.accountSubject.asObservable();

  constructor(private accountApi: AccountApiService) {}

  get account(): AccountDataDTO | null {
    return this.accountSubject.value;
  }

  setAccount(account: AccountDataDTO): void {
    this.accountSubject.next(account);
  }

  loadUser(): void {
    this.accountApi.getAccountData().subscribe({
      next: (data) => {
        this.accountSubject.next(data);
      },
      error: () => this.accountSubject.next(null)
    });
  }
}