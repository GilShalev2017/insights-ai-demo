import { Component } from '@angular/core';
import { InsightsService } from '../services/insights.service';

@Component({
  selector: 'app-key-points',
  templateUrl: './key-points.component.html',
  styleUrls: ['./key-points.component.css']
})
export class KeyPointsComponent {
  // public filteredKeyPointsList?: string[] = this.insightsService?.insightsResponse?.KeyPointsList?.filter(point => point.trim() !== '');
  constructor(public insightsService: InsightsService){

  }
}
