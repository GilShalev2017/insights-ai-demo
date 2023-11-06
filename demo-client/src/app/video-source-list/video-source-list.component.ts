import { Component } from '@angular/core';
import { VideoService } from '../services/video.service';

@Component({
  selector: 'app-video-source-list',
  templateUrl: './video-source-list.component.html',
  styleUrls: ['./video-source-list.component.css']
})
export class VideoSourceListComponent {
  selectedVideoIndex: number | null = null;
  constructor(public videoService: VideoService) {}
  selectVideo(index: number) {
    this.selectedVideoIndex = index;
  }
}
