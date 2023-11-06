import { Component,OnInit } from '@angular/core';
import { VideoService } from '../services/video.service';
import { InsightsService } from '../services/insights.service';
import { VideoDetails } from '../models/video-details';

@Component({
  selector: 'app-video-details',
  templateUrl: './video-details.component.html',
  styleUrls: ['./video-details.component.css']
})
export class VideoDetailsComponent implements OnInit{
   createdAt: Date = new Date();
   createdBy: string = "Gil Shalev";
   public videoDetails?: VideoDetails;
  
   constructor(private videoService: VideoService, private insightsService: InsightsService) {}

   ngOnInit() {
     this.subscribeToAttachedVideo();
   }

   subscribeToAttachedVideo() {
    this.insightsService.attachNewVideo$.subscribe((videoFileName: string) => {
      this.videoDetails = this.videoService.videoDetailsByVideoName.get(videoFileName);
      const now = new Date();
      const date = now.toLocaleDateString();
      const time = now.toLocaleTimeString();
      if (this.videoDetails != null) {
        this.videoDetails.createdBy = "Gil Shalev";
        this.videoDetails.createdAt = "11/3/2023-12:44:38";//date + " - " + time;
        this.videoDetails.videoType = "Uploaded file";
        this.videoDetails.channel = "Youtube";
      }
    });
  }
}
