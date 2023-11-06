import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { InsightsResponse } from '../models/insights-respose';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InsightsService {

  private insightsBaseUrl = 'https://localhost:5001/api/insights';
  blobUrl: string | undefined;
  
  playAzureVideo$: BehaviorSubject<number> = new BehaviorSubject(0);
  jumpToFromLabel$: BehaviorSubject<number> = new BehaviorSubject(0);
  
  public attachNewVideo$: BehaviorSubject<string> = new BehaviorSubject("");

  public isTranslating = false;

  public insightsResponse: InsightsResponse = {
    Transcripts: [],
    OriginalTranscripts: [],
    Summary: '',
    KeyPoints: '',
    ActionItems: '',
    Sentiment: '',
    KeyPointsList: [],
    ActionItemsList:[]
  };

  constructor(private http: HttpClient) { }

  runInsights(attachedVideoFileName: string, blobUrl: string) {
    this.blobUrl = blobUrl;

    this.attachNewVideo$.next(attachedVideoFileName);

    this.isTranslating = true;
    return this.http.post(this.insightsBaseUrl,{videoFileName:attachedVideoFileName}).subscribe((response:any) => {
      this.isTranslating = false;
      this.insightsResponse = response;
      this.insightsResponse.OriginalTranscripts = response.Transcripts;
      this.insightsResponse.KeyPointsList = this.styleStringWithPoints( this.insightsResponse.KeyPoints)
      this.insightsResponse.KeyPointsList = this.insightsResponse.KeyPointsList.filter(item => item !== "");
      this.insightsResponse.ActionItemsList = this.styleStringWithPoints( this.insightsResponse.ActionItems)
      this.insightsResponse.ActionItemsList = this.insightsResponse.ActionItemsList.filter(item => item !== "");
    });
  }

  styleStringWithPoints(string: string): string []{
    // Split the string into an array of strings, one for each point.
    const points = string.split(/\d+\./);  
    return points;
  }

  setAzurePlayingVideo(currentTimeInSeconds : number)//videoName: string, videoDetails :VideoDetails) 
  {
    this.playAzureVideo$.next(currentTimeInSeconds);
  }

  translateSTT(language: string)
  {
    this.isTranslating = true;
    var translateUrl = this.insightsBaseUrl + "/translate";
    return this.http.post(translateUrl,{Transcripts:this.insightsResponse.OriginalTranscripts,TargetLanguage:language}).subscribe((response:any) => {
        console.log(response);
        this.insightsResponse.Transcripts = response.Transcripts;

        this.isTranslating = false;
      });
  }
}
