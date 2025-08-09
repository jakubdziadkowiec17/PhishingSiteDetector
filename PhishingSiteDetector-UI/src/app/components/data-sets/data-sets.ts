import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DataSetItemDTO } from '../../interfaces/data-set-item-dto';
import { DataSetApiService } from '../../services/api/data-set-api-service';
import { TableModule } from 'primeng/table';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { DialogModule } from 'primeng/dialog';
import { FileUploadModule } from 'primeng/fileupload';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/common/notification-service';
import { ResponseDTO } from '../../interfaces/response-dto';
import { finalize, debounceTime, distinctUntilChanged, tap, Subject } from 'rxjs';
import { CardModule } from 'primeng/card';
import { TranslateModule } from '@ngx-translate/core';
import { InputIconModule } from 'primeng/inputicon';
import { IconFieldModule } from 'primeng/iconfield';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ActivatedRoute, Router } from '@angular/router';
import { DataSetSortField } from '../../constants/data-set-sort-field';
import { SortOrder } from '../../constants/sort-order';

@Component({
  selector: 'app-data-sets',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, TableModule, InputIconModule, IconFieldModule, FloatLabelModule, ToggleSwitchModule, DialogModule, FileUploadModule, TranslateModule, CardModule, ButtonModule, InputTextModule, ConfirmDialogModule, TooltipModule],
  templateUrl: './data-sets.html',
  styleUrl: './data-sets.css',
  providers: [ConfirmationService, MessageService]
})
export class DataSetsComponent implements OnInit {
  dataSets: DataSetItemDTO[] = [];
  count = 0;
  loadingDataSets = false;
  searchText = '';
  pageNumber = 1;
  pageSize = 5;
  sortField: string = DataSetSortField.Id;
  sortOrder: number = SortOrder.Desc;
  dataSetSortField = DataSetSortField
  uploadForm!: FormGroup;
  selectedFile?: File;
  uploadDialogVisible = false;
  deleteDialogVisible = false;
  dataSetToDeleteId?: number;

  private searchText$ = new Subject<string>();

  constructor(private dataSetApiService: DataSetApiService, private fb: FormBuilder, private notificationService: NotificationService, private cdr: ChangeDetectorRef, private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    this.uploadForm = this.fb.group({file: [null, Validators.required], isActiveDataSet: [false]});

    this.route.queryParamMap.subscribe(params => {
      this.searchText = params.get('search') ?? this.searchText;
      this.pageNumber = params.has('page') ? +params.get('page')! : this.pageNumber;
      this.sortField = params.get('sortField') ?? this.sortField;
      this.sortOrder = params.has('sortOrder') ? +params.get('sortOrder')! : this.sortOrder;

      this.getDataSets();
    });

    this.searchText$.pipe(
      debounceTime(800),
      distinctUntilChanged(),
      tap(() => {
        this.pageNumber = 1;
        this.updateUrl()
      })
    ).subscribe();
  }

  private updateUrl(): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        search: this.searchText || null,
        page: this.pageNumber || null,
        sortField: this.sortField || null,
        sortOrder: this.sortOrder || null
      },
      queryParamsHandling: 'merge'
    });
  }

  getDataSets(): void {
    this.loadingDataSets = true;
    this.dataSetApiService.getDataSets(this.searchText, this.pageNumber, this.pageSize, this.sortField, this.sortOrder).pipe(
      finalize(() => {
        this.loadingDataSets = false;
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: (result) => {
        this.dataSets = result.items;
        this.count = result.count;
      }
    });
  }

  onSearch(): void {
    this.searchText$.next(this.searchText);
  }

  onPage(event: any): void {
    this.pageNumber = event.first / event.rows + 1;
    this.pageSize = event.rows;
    this.updateUrl();
  }

  onSort(event: any): void {
    this.sortField = event.field;
    this.sortOrder = event.order;
    this.pageNumber = 1;
    this.updateUrl();
  }

  updateActivityForDataSet(dataSet: DataSetItemDTO): void {
    const newStatus = !dataSet.isActiveDataSet;
    this.dataSetApiService.updateActivityForDataSet(dataSet.id, { isActiveDataSet: newStatus }).subscribe({
      next: (response: ResponseDTO) => {
        dataSet.isActiveDataSet = newStatus;
        this.notificationService.showSuccessToast(response.message);
        this.getDataSets();
      }
    });
  }

  downloadDataSet(dataSet: DataSetItemDTO): void {
    this.dataSetApiService.downloadDataSet(dataSet.id).subscribe(response => {
      const contentDisposition = response.headers.get('Content-Disposition') || '';
      let filename = dataSet.name;
      const matches = /filename="?([^"]+)"?/.exec(contentDisposition);
      if (matches && matches[1]) { filename = matches[1]; }
      const blob = response.body;
      if (blob) {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        a.click();
        window.URL.revokeObjectURL(url);
      }
    });
  }

  showDeleteDialog(dataSetId: number) {
    this.dataSetToDeleteId = dataSetId;
    this.deleteDialogVisible = true;
  }

  hideDeleteDialog(): void {
    this.dataSetToDeleteId = undefined;
    this.deleteDialogVisible = false;
  }

  deleteDataSetConfirmed() {
    if (this.dataSetToDeleteId != null) {
      this.deleteDataSet(this.dataSetToDeleteId);
    }
    this.deleteDialogVisible = false;
  }

  deleteDataSet(id: number): void {
    this.dataSetApiService.deleteDataSet(id).subscribe({
      next: (response: ResponseDTO) => {
        this.notificationService.showSuccessToast(response.message);
        this.getDataSets();
      }
    });
  }

  showUploadDialog(): void {
    this.uploadForm.reset({ isActiveDataSet: false });
    this.selectedFile = undefined;
    this.uploadDialogVisible = true;
  }

  hideUploadDialog(): void {
    this.uploadForm.reset();
    this.selectedFile = undefined;
    this.uploadDialogVisible = false
  }

  onFileSelected(event: any): void {
    if (event.files && event.files.length > 0) {
      this.selectedFile = event.files[0];
      this.uploadForm.get('file')?.setValue(this.selectedFile);
    }
  }

  uploadDataSet(): void {
    if (!this.selectedFile || this.uploadForm.invalid) { return; }
    const formData = new FormData();
    formData.append('file', this.selectedFile);
    formData.append('isActiveDataSet', this.uploadForm.get('isActiveDataSet')?.value);
    this.dataSetApiService.uploadDataSet(formData).subscribe({
      next: (response: ResponseDTO) => {
        this.notificationService.showSuccessToast(response.message);
        this.uploadDialogVisible = false;
        this.getDataSets();
      }
    });
  }
}