import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { Menubar } from 'primeng/menubar';

@Component({
  selector: 'app-footer',
  imports: [Menubar, TranslateModule],
  templateUrl: './footer.html',
  styleUrl: './footer.css'
})
export class Footer {
  currentYear: number = new Date().getFullYear();
}
