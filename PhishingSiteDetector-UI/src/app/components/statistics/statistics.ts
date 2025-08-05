import { Component, OnInit, OnDestroy, ChangeDetectorRef, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { finalize, Subscription } from 'rxjs';
import { CardModule } from 'primeng/card';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SiteLogApiService } from '../../services/api/site-log-api-service';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { CommonModule } from '@angular/common';
import { ChartDTO } from '../../interfaces/chart-dto';
import { TooltipModule } from 'primeng/tooltip';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.html',
  styleUrl: './statistics.css',
  imports: [CommonModule, TranslateModule, CardModule, NgxChartsModule, TooltipModule, ProgressSpinnerModule]
})
export class StatisticsComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('chartContainer') chartContainer!: ElementRef;
  chartData: { name: string, value: number }[] = [];
  rawData: ChartDTO[] = [];
  langChangeSub?: Subscription;
  currentYear = new Date().getFullYear();
  view: [number, number] = [700, 400];
  loadingStatistics = false;

  constructor(private siteLogApiService: SiteLogApiService, private translate: TranslateService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.updateChartSize();
    window.addEventListener('resize', this.updateChartSize.bind(this));

    this.loadData(this.currentYear);

    this.langChangeSub = this.translate.onLangChange.subscribe(() => {
      this.updateChartData();
    });
  }

  ngAfterViewInit(): void {
    this.updateChartSize();
    this.cdr.detectChanges();
    window.addEventListener('resize', this.updateChartSize.bind(this));
  }

  ngOnDestroy(): void {
    window.removeEventListener('resize', this.updateChartSize.bind(this));
    this.langChangeSub?.unsubscribe();
  }

  updateChartSize(): void {
    if (this.chartContainer && this.chartContainer.nativeElement) {
      const width = this.chartContainer.nativeElement.offsetWidth;
      this.view = [width, 400];
    }
  }

  loadData(year: number): void {
    this.loadingStatistics = true;
    this.siteLogApiService.getSiteLogs(year).pipe(
      finalize(() => {
        this.loadingStatistics = false;
        this.updateChartSize();
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: (data) => {
        this.rawData = data;
        this.updateChartData();
        this.cdr.detectChanges();
      },
      error: () => {
        this.chartData = [];
      }
    });
  }

  updateChartData(): void {
    this.chartData = this.rawData.map(item => ({
      name: this.translate.instant(`STATISTICS.MONTHS.${item.month}`),
      value: item.count
    }));
  }

  previousYear(): void {
    this.currentYear--;
    this.loadData(this.currentYear);
  }

  nextYear(): void {
    this.currentYear++;
    this.loadData(this.currentYear);
  }
}