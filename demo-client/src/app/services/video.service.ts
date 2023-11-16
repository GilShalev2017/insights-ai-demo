import { Injectable } from '@angular/core';
import { VideoSource } from '../models/video-source';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { VideoDetails } from '../models/video-details';
import { InsightsService } from './insights.service';

@Injectable({
  providedIn: 'root'
})
export class VideoService {
  public videoSources: VideoSource[] = [];
  private playingVideoNames: string[] = [];
  private videoBaseUrl = 'https://localhost:5001/api/mediaFiles';
  private facesBaseUrl = 'https://localhost:5001/api/faceFiles';
  private frBaseUrl    = 'https://localhost:5001/api/frameRecognition';

  // private videoBaseUrl = 'http://34.229.229.19:5000/api/MediaFiles';
  // private facesBaseUrl = 'http://34.229.229.19:5000/api/faceFiles';
  // private frBaseUrl    = 'http://34.229.229.19:5000/api/frameRecognition';

  maxProcessing:number = 4;
  
  playVideo$: BehaviorSubject<string> = new BehaviorSubject("");
  stopVideo$: BehaviorSubject<string> = new BehaviorSubject("");
  jumpTo$: BehaviorSubject<number> = new BehaviorSubject(0);
  videoDetailsByVideoName: Map<string, VideoDetails> = new Map<string, VideoDetails>();

  constructor(private http: HttpClient) {}

  uploadVideoFile(formdata: FormData) {
    return this.http.post(this.videoBaseUrl, formdata,{ responseType: 'text' });
  }
  
  uploadFaceFiles(formData: FormData) {
    return this.http.post(this.facesBaseUrl, formData,{ responseType: 'text' });
  }

  fetchVideoBlob(videoName: string) {
    return this.http.get(`${this.videoBaseUrl}/${videoName}`, {
      responseType: 'blob',
    });
  }

  addVideoSource(source: VideoSource) {
    this.videoSources.push(source);
  }

  setPlayingVideo(videoName: string, videoDetails :VideoDetails) {
    this.videoDetailsByVideoName.set(videoName, videoDetails);
    this.playVideo$.next(videoName);
  }

  stopPlayingVideo(videoName: string) {
    this.stopVideo$.next(videoName);
  }
  
  jumpVideo(time: number) {
    this.jumpTo$.next(time);
  }

  detectFaces(videoFileName: string, facesFolder: string) {
    return this.http.post(this.frBaseUrl, {videoFileName:videoFileName},{ responseType: 'text' });
  }

  stopDetection(videoFileName: string) {
    return this.http.put(this.frBaseUrl, {videoFileName:videoFileName},{ responseType: 'text' });
  }

  stopAllDetectionProcesses() {
    return this.http.get(this.frBaseUrl, { responseType: 'text' });
  }

  getVideoDetails(videoFileName: string) {
    const url = `${this.frBaseUrl}/${videoFileName}`;
    return this.http.get(url);
  }
}

// getThumbnails = () => {
//   return this.http.get(this.videoBaseUrl);
// };
