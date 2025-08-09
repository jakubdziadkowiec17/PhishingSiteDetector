import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule, NgIf } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { StepperModule } from 'primeng/stepper';
import { InputTextModule } from 'primeng/inputtext';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { TranslateModule } from '@ngx-translate/core';
import { UrlPredictionDTO } from '../../interfaces/url-prediction-dto';
import { UrlPredictionApiService } from '../../services/api/url-prediction-api-service';
import { UrlDTO } from '../../interfaces/url-dto';
import { MessageModule } from 'primeng/message';
import { FloatLabel } from 'primeng/floatlabel';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [NgIf,CommonModule, ReactiveFormsModule, CardModule, ButtonModule, StepperModule, InputTextModule, ToggleSwitchModule, TranslateModule, MessageModule, FloatLabel],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class HomeComponent implements OnInit {
  activeStep: number = 1;
  form!: FormGroup;
  predictionResult: UrlPredictionDTO | null = null;
  loadingPredict = false;

  constructor(private fb: FormBuilder, private urlPredictionApiService: UrlPredictionApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      url: ['', [Validators.required]],
      isRandomString: [false],
      hasDomainInSubdomain: [false],
      hasDomainInPath: [false],
      hasEmbeddedBrandName: [false],
    });
  }

  resetPrediction() {
    this.predictionResult = null;
  }

  goToStep2(): void {
    if (this.activeStep == 1 && this.form.get('url')?.invalid) {
      this.form.get('url')?.markAsTouched();
      return;
    }

    this.activeStep = 2;
  }
  
  goToStep3(): void {
    if (this.activeStep == 1 && this.form.get('url')?.invalid) {
      this.form.get('url')?.markAsTouched();
      return;
    }

    this.activeStep = 3;
  }

  predict(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const dto: UrlDTO = this.form.value;
    this.loadingPredict = true;
    this.urlPredictionApiService.predict(dto).subscribe({
      next: (res) => {
        this.predictionResult = res;
        this.loadingPredict = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.predictionResult = null;
        this.loadingPredict = false;
        this.cdr.detectChanges();
      }
    });
  }
}