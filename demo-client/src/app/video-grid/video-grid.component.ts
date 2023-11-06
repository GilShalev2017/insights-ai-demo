import { Component, ViewChildren, QueryList, AfterViewInit } from '@angular/core';
import { VideoService } from '../services/video.service';
import { VideoCellComponent } from '../video-cell/video-cell.component';

export interface Tile {
  color: string;
  cols: number;
  rows: number;
  text: string;
}

@Component({
  selector: 'app-video-grid',
  templateUrl: './video-grid.component.html',
  styleUrls: ['./video-grid.component.css']
})
export class VideoGridComponent implements AfterViewInit {

  @ViewChildren(VideoCellComponent)
  videoCells!: QueryList<VideoCellComponent>;

  playingVideosCount: number = 0;
  videoDictionary: { [key: string]: VideoCellComponent } = {};

  constructor(private videoService: VideoService) { }

  ngAfterViewInit() {

    this.subscribeToPlayVideo();
    this.subscribeToStopVideo();
    this.subscribeToJumpToVideo();
  }

  subscribeToJumpToVideo() {
    this.videoService.jumpTo$.subscribe((timeInSeconds: number) => {
      const videoName = "G7.mp4";
      if(this.videoDictionary[videoName]){
        this.videoDictionary[videoName].jumpTo(timeInSeconds);
      }
    });
  }
  subscribeToStopVideo() {
    this.videoService.stopVideo$.subscribe((videoName: string) => {
      if(this.videoDictionary[videoName]){
        this.videoDictionary[videoName].stopVideo();
      }
    });
  }

  subscribeToPlayVideo() {
    this.videoService.playVideo$.subscribe((videoName: string) => {
      if (videoName == "")
        return;

      if(this.videoDictionary[videoName]){
        this.videoDictionary[videoName].playVideo();
        return;
      }

      if (this.playingVideosCount < this.videoService.maxProcessing) {
        this.playingVideosCount++;
      }
      if (this.playingVideosCount <= this.videoService.maxProcessing) {
        const videoCell = this.videoCells.get(this.playingVideosCount - 1);
        if (videoCell) {
          videoCell.attachedVideoFileName = videoName;
          videoCell.playVideo();
          this.videoDictionary[videoName]=videoCell;
        }
      }
    });
  }
}


// tiles: Tile[] = [
//   {text: 'One', cols: 3, rows: 1, color: 'lightblue'},
//   {text: 'Two', cols: 1, rows: 2, color: 'lightgreen'},
//   {text: 'Three', cols: 1, rows: 1, color: 'lightpink'},
//   {text: 'Four', cols: 2, rows: 1, color: '#DDBDF1'},
// ];
