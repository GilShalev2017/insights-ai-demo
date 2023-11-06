import { Component } from '@angular/core';
import { InsightsService } from '../services/insights.service';

@Component({
  selector: 'app-action-items',
  templateUrl: './action-items.component.html',
  styleUrls: ['./action-items.component.css']
})
export class ActionItemsComponent {
  constructor(public insightsService: InsightsService){

  }
}
