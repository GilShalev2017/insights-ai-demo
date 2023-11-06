import { Component, ViewChild, ElementRef, AfterViewInit, OnInit, ChangeDetectorRef, OnChanges, SimpleChanges } from '@angular/core';
import { InsightsService } from '../services/insights.service';


@Component({
  selector: 'app-azure-video-panel',
  templateUrl: './azure-video-panel.component.html',
  styleUrls: ['./azure-video-panel.component.css']
})
export class AzureVideoPanelComponent implements AfterViewInit, OnInit {
  @ViewChild('videoPlayer') videoPlayer!: ElementRef;

  
  currentVideoTime = 0;

  constructor(public insightsService: InsightsService) {}
    
  ngOnInit() {
    this.subscribeToJumpToFromLabel();
    this.subscribeToAttachedVideo();
  }

  ngAfterViewInit(): void {
    this.listenToPlayEvent(this.insightsService);
  }
  
  
  subscribeToAttachedVideo() {
    this.insightsService.attachNewVideo$.subscribe((videoFileName: string) => {
      this.videoPlayer.nativeElement.src = this.insightsService.blobUrl;
    });
  }

  subscribeToJumpToFromLabel() {
    this.insightsService.jumpToFromLabel$.subscribe((startInSeconds: number) => {
      this.videoPlayer.nativeElement.currentTime=startInSeconds;
    });
  }

  playPause() {
    const video = this.videoPlayer.nativeElement as HTMLVideoElement;
    if (video.paused) {
      video.play();
    } else {
      video.pause();
    }
  }

  jumpToTime(seconds: number) {
    const video = this.videoPlayer.nativeElement as HTMLVideoElement;
    video.currentTime = seconds;
  }

  listenToPlayEvent(insightsService : InsightsService) {
    this.videoPlayer.nativeElement.addEventListener('play',  () => {
      // This code will run when the video starts playing
      console.log('Video started playing');
      const video = this.videoPlayer.nativeElement as HTMLVideoElement;
      insightsService.setAzurePlayingVideo(video.currentTime);
    });
  }
}
