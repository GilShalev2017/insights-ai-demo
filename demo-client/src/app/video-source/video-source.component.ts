import { Component, Input } from '@angular/core';
import {MatIconModule} from '@angular/material/icon';
import {MatDividerModule} from '@angular/material/divider';
import {MatButtonModule} from '@angular/material/button';
import { VideoService } from '../services/video.service';
import { HttpErrorResponse } from '@angular/common/http';
import { VideoSource } from '../models/video-source';

@Component({
  selector: 'app-video-source',
  templateUrl: './video-source.component.html',
  styleUrls: ['./video-source.component.css']
})
export class VideoSourceComponent {
  @Input() videoSource!: VideoSource;
  
  isPlaying: boolean = false;

  constructor(private videoService: VideoService) {}

  play() {
    this.videoService.setPlayingVideo(this.videoSource.title, this.videoSource.videoDetails);
    this.isPlaying = true;
  }

  stop() {
    this.videoService.stopPlayingVideo(this.videoSource.title);
    this.isPlaying = false;
  }

  hide() {
    
  }

  replay() {
    
  }
}