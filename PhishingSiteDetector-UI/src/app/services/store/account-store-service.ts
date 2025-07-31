import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AccountDataDTO } from '../../interfaces/account-data-dto';
import { AccountApiService } from '../api/account-api-service';
import { AccountService } from '../common/account-service';

@Injectable({ providedIn: 'root' })
export class AccountStoreService {
  private accountSubject = new BehaviorSubject<AccountDataDTO | null>(null);
  public account$: Observable<AccountDataDTO | null> = this.accountSubject.asObservable();

  constructor(private accountApi: AccountApiService, private accountService: AccountService) {}

  loadUser(): void {
    this.accountApi.getAccountData().subscribe({
      next: (data) => {
        this.accountSubject.next(data);
      },
      error: () => this.accountSubject.next(null)
    });
  }

  get currentAccount(): AccountDataDTO | null {
    return this.accountSubject.value;
  }
}