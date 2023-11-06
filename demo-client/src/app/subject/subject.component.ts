import { Component, Input } from '@angular/core';
import { SubjectPerson } from '../models/subject-person';
import { VideoService } from '../services/video.service';

@Component({
  selector: 'app-subject',
  templateUrl: './subject.component.html',
  styleUrls: ['./subject.component.css']
})
export class SubjectComponent {
  @Input() data!: SubjectPerson;

  timelineData: { timeRangeAndScoreArray: Array<{ from: Date, to: Date }> } = {
    timeRangeAndScoreArray: [
      { from: new Date(2023, 10, 5, 9, 0, 0), to: new Date(2023, 10, 5, 10, 0, 0) },
      { from: new Date(2023, 10, 5, 11, 0, 0), to: new Date(2023, 10, 5, 12, 0, 0) },
    ]
  };

  constructor(private videoService: VideoService) {}
  
  jumpTo(timeString: string) {
    const timePart = timeString.split(" ")[1];
    const [hours, minutes, seconds] = timePart.split(':');
    // Convert each part to an integer.
    const hoursInt = parseInt(hours);
    const minutesInt = parseInt(minutes);
    const secondsInt = parseInt(seconds);
    // Calculate the total number of seconds in the time.
    const totalSeconds = (hoursInt * 3600) + (minutesInt * 60) + secondsInt;
    this.videoService.jumpVideo(secondsInt);//totalSeconds);
  }
}
