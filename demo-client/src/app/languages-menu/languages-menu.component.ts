import { Component , EventEmitter, Output} from '@angular/core';
import { InsightsService } from '../services/insights.service';
@Component({
  selector: 'app-languages-menu',
  templateUrl: './languages-menu.component.html',
  styleUrls: ['./languages-menu.component.css']
})
export class LanguagesMenuComponent {
  selectedLanguage: string = '';
  @Output() selectedLangaugeEvent = new EventEmitter<string>();
  selectLanguage(language: string) {
    this.selectedLanguage = language;
    this.selectedLangaugeEvent.emit(language);
  }
}
