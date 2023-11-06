import { Component, ElementRef, OnDestroy, OnInit, ViewChild, AfterViewChecked } from '@angular/core';
import { InsightsService } from '../services/insights.service';
import { Transcript } from '../models/insights-respose';

@Component({
  selector: 'app-transcriptions',
  templateUrl: './transcriptions.component.html',
  styleUrls: ['./transcriptions.component.css']
})
export class TranscriptionsComponent implements OnDestroy, OnInit, AfterViewChecked {
  @ViewChild('transcriptContainer')
  private transcriptContainer!: ElementRef;
  private transcriptTimers: any[] = [];

  constructor(public insightsService: InsightsService) { }
  ngOnInit() {
    this.subscribeToPlayVideo();
  }
  ngAfterViewChecked() {
    // this.scrollToActiveTranscript();
  }
  findTranscriptIndexByTime(currentTimeInSeconds: number) {
    const transcripts = this.insightsService.insightsResponse.Transcripts;
    for (let index = 0; index < transcripts.length; index++) {
      const transcript = transcripts[index];
      if (currentTimeInSeconds >= +transcript.StartInSeconds && currentTimeInSeconds <= +transcript.EndInSeconds) {
        return index;
      }
    }
    return 0;
  }
  subscribeToPlayVideo() {
    this.insightsService.playAzureVideo$.subscribe((currentTimeInSeconds: number) => {
      this.setAllNonActive();
      var transcriptIndex = 0;
      transcriptIndex = this.findTranscriptIndexByTime(currentTimeInSeconds);
      this.startTranscriptTimers(transcriptIndex);
    });
  }
  startTranscriptTimers(transcriptIndex: number) {
    this.stopTranscriptTimers();
    this.setAllNonActive();
    const transcripts = this.insightsService.insightsResponse.Transcripts;
    if (transcripts == null)
      return;
    var timeOffsetOfSelectedTranscript = +transcripts[transcriptIndex].StartInSeconds;
    for (let i = transcriptIndex; i < transcripts.length; i++) {
      const transcript = transcripts[i];
      const delay = (+transcript.StartInSeconds - timeOffsetOfSelectedTranscript) * 1000;
      const timer = setTimeout(() => {
        this.setAllNonActive();
        transcript.isActive = true;
      }, delay);
      this.transcriptTimers.push(timer);
    }
  }
  stopTranscriptTimers() {
    this.transcriptTimers.forEach((timeout) => {
      clearTimeout(timeout);
    });
    this.transcriptTimers = [];
  }
  setAllNonActive() {
    const transcripts = this.insightsService.insightsResponse.Transcripts;
    transcripts.forEach(transcript => {
      transcript.isActive = false;
    });
  }
  ngOnDestroy() {
    this.stopTranscriptTimers();
  }
  formatTime(seconds: string): string {
    const totalSeconds = parseFloat(seconds);
    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const sec = Math.floor(totalSeconds % 60);
    return `${this.pad(hours)}:${this.pad(minutes)}:${this.pad(sec)}`;
  }
  private pad(val: number): string {
    return val < 10 ? `0${val}` : val.toString();
  }
  jumpToTimeFromLabel(transcript: Transcript) {
    const transcripts = this.insightsService.insightsResponse.Transcripts;
    var transcriptIndex = transcripts.findIndex(tr => tr.StartInSeconds === transcript.StartInSeconds)
    this.insightsService.jumpToFromLabel$.next(+transcript.StartInSeconds);
    this.startTranscriptTimers(transcriptIndex);
  }
  onSelectedLangauge($event: string) {
    this.insightsService.translateSTT($event);
  }
  
  text2Speech() {
    var synthesis = window.speechSynthesis;
    
    var utterance = new SpeechSynthesisUtterance(this.insightsService.insightsResponse.Summary);
    utterance.lang = 'en-US';
    
    var availableVoices = window.speechSynthesis.getVoices();
    var selectedVoice = availableVoices[0];
    utterance.voice = selectedVoice;
    synthesis.speak(utterance);

    // this.insightsService.insightsResponse.Transcripts.forEach(transcript => {
    //   var utterance = new SpeechSynthesisUtterance(transcript.Text);
    //   synthesis.speak(utterance);
    // });
    
  }
}

  // scrollToActiveTranscript(): void {
  //   // Get the active transcript element.
  //   const activeTranscriptElement = this.transcriptContainer.nativeElement.querySelector('.active');
  //   if(activeTranscriptElement == null)
  //      return;
  //   // Get the scroll offset of the transcript container.
  //   const scrollOffset = this.transcriptContainer.nativeElement.scrollTop;
  //   // Calculate the scroll position of the active transcript.
  //   const scrollPosition = activeTranscriptElement.getBoundingClientRect().top + scrollOffset;
  //   // Scroll the transcript container to the active transcript.
  //   this.transcriptContainer.nativeElement.scrollTo(0, scrollPosition);
  // }
