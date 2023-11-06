import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { VideoService } from '../services/video.service';
import { HttpErrorResponse } from '@angular/common/http';
import { WebSocketService } from '../services/web-socket.service';
import { InsightsService } from '../services/insights.service';

@Component({
  selector: 'app-video-cell',
  templateUrl: './video-cell.component.html',
  styleUrls: ['./video-cell.component.css']
})
export class VideoCellComponent {
  @ViewChild('videoElement') videoElement!: ElementRef<HTMLVideoElement>;
  isPlaying: boolean = false;
  isVideoPlaying = false;
  public attachedVideoFileName: string = "";
  showOverlay = false;
  statusMessage = "";

  constructor(private videoService: VideoService, private webSocketService: WebSocketService, private insightsService: InsightsService) {
    this.webSocketService.receiveStatusMessage().subscribe(onmessage => {
      if (this.attachedVideoFileName != "" && onmessage.includes(this.attachedVideoFileName)) {
        if(!onmessage.includes("STOPPED")) {
          setTimeout(() => {
            this.showOverlay = false;
          }, 5000); // 5 seconds 
        }
        console.log(`VideoCellComponent received ${onmessage}`);
        this.statusMessage = onmessage;
        this.showOverlay = true;
      }
    });
  }

  playVideo() {
    this.videoService.fetchVideoBlob(this.attachedVideoFileName).subscribe({
      next: (blob) => {
        const blobUrl = URL.createObjectURL(blob);
        this.videoElement.nativeElement.src = blobUrl;
        this.insightsService.runInsights(this.attachedVideoFileName, blobUrl);
        if (this.videoElement.nativeElement.src) {
          this.videoElement.nativeElement.play();
          this.isPlaying = true;
        }
      },
      error: (err: HttpErrorResponse) => console.log(err),
    });

    this.videoService.detectFaces(this.attachedVideoFileName, "")
      .subscribe((response) => {
        console.log(response);
      });
    
    // this.insightsService.runInsights(this.attachedVideoFileName);
    
    }

  jumpTo(time: number) {
    if (!this.videoElement || !this.videoElement.nativeElement)
      return;
    this.videoElement.nativeElement.currentTime=time;
  }
  
  stopVideo() {
    if (!this.videoElement || !this.videoElement.nativeElement)
      return;
    this.videoElement.nativeElement.pause();
    this.isPlaying = false;

    this.videoService.stopDetection(this.attachedVideoFileName)
      .subscribe((response) => {
        console.log(response);
      });
  }

}
