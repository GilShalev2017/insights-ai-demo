import { Component } from '@angular/core';
import { InsightsService } from '../services/insights.service';

@Component({
  selector: 'app-sentiment',
  templateUrl: './sentiment.component.html',
  styleUrls: ['./sentiment.component.css']
})
export class SentimentComponent {
  constructor(public insightsService: InsightsService){

  }
}
