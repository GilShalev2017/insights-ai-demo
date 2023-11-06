import { Component, ElementRef, ViewChild, AfterViewChecked } from '@angular/core';
import { SubjectPerson } from '../models/subject-person';
import { WebSocketService } from '../services/web-socket.service';
import { VideoService } from '../services/video.service';

@Component({
  selector: 'app-subject-list',
  templateUrl: './subject-list.component.html',
  styleUrls: ['./subject-list.component.css']
})
export class SubjectListComponent implements AfterViewChecked {
  @ViewChild('subjectListContainer')
  private subjectListContainer!: ElementRef;
  subjects: SubjectPerson[] = [];
  public autoScroll: boolean = true;

  ngAfterViewChecked() {
    if (this.autoScroll)
      this.scrollToBottom();
  }
  scrollToBottom(): void {
    try {
      this.subjectListContainer.nativeElement.scrollTop = this.subjectListContainer.nativeElement.scrollHeight;
    } catch (err) {
      console.error('Error scrolling to bottom:', err);
    }
  }
  rotate() {

  }
  clearSubjects() {
    this.subjects = [];
  }
  constructor(private webSocketService: WebSocketService,private videoService: VideoService) {
    this.webSocketService.receiveFaceDetectionMessage().subscribe(onmessage => {
      console.log(`SubjectListComponent received ${onmessage}`);
      const splits = onmessage.split('-');
      const img = splits[0];
      const score = splits[1];
      const name = img.substring(0, img.indexOf("."));
      this.subjects.push({
        time: "22:38:01",
        status: "Disappeared",
        image1: `assets/images/${img}`,
        subjectIdentified: true,
        identifiedPersonName: `${name}`,
        score: `${score}`
      },);
    });

    this.webSocketService.receiveDetectionSummaryMessage()
      .subscribe(data => {
        for (const propX in data) {
          const value = data[propX];
          const name = data[propX].name;
          const nameNoPostfix = name!.substring(0, name!.indexOf("."));
          const timeRangeAndScores = data[propX].timeRangeAndScores;
          const subjectPerson = this.findSubjectWithIdentifiedPersonName(nameNoPostfix!);
          if (subjectPerson) {
            subjectPerson.timeRangeAndScoreArray = timeRangeAndScores;
            subjectPerson.timeRangeAndScoreArray2 = [];
            timeRangeAndScores?.forEach(dt => {
              const videoDetails = this.videoService.videoDetailsByVideoName.get(dt.videoName!);
              subjectPerson.videoDetails = videoDetails;
              const duration = this.durationToSeconds(videoDetails!.duration!);
              subjectPerson.timeRangeAndScoreArray2?.push({from: new Date(dt.from!), to: new Date(dt.to!), duration: duration});
            })
          }
        }
      });
  }

  durationToSeconds(duration: string): number {
    // Split the duration string into components
    const parts = duration.split(':');
    // Extract hours, minutes, and seconds
    const hours = parseInt(parts[0], 10) || 0;
    const minutes = parseInt(parts[1], 10) || 0;
    // Split the seconds into integer and fractional parts
    const secondsParts = parts[2].split('.');
    const seconds = parseInt(secondsParts[0], 10) || 0;
    const milliseconds = parseFloat(`0.${secondsParts[1] || '0'}`) * 1000;
    // Calculate the total duration in seconds
    const totalSeconds = hours * 3600 + minutes * 60 + seconds + milliseconds / 1000;
    return totalSeconds;
  }
  
  findSubjectWithIdentifiedPersonName(identifiedPersonName: string): SubjectPerson | null {
    for (const subject of this.subjects) {
      if (subject.identifiedPersonName === identifiedPersonName) {
        return subject;
      }
    }
    return null;
  }
}