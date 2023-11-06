import { Component } from '@angular/core';
import { InsightsService } from '../services/insights.service';

@Component({
  selector: 'app-summary',
  templateUrl: './summary.component.html',
  styleUrls: ['./summary.component.css']
})
export class SummaryComponent {
  
  constructor(public insightsService: InsightsService){

  }
}
