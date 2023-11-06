import { Component } from '@angular/core';
import { InsightsService } from '../services/insights.service';

@Component({
  selector: 'app-insights-container',
  templateUrl: './insights-container.component.html',
  styleUrls: ['./insights-container.component.css']
})
export class InsightsContainerComponent {

  constructor(public insightsService: InsightsService) {
    
    
  }
}
