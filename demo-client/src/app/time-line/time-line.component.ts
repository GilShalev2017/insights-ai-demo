import { Component, Input, OnInit } from '@angular/core';
import { VideoService } from '../services/video.service';
import { TimeRangeAndScore2 } from '../models/SubjectDetailsWithNameAndScores';

@Component({
  selector: 'app-time-line',
  templateUrl: './time-line.component.html',
  styleUrls: ['./time-line.component.css']
})
export class TimeLineComponent {
  @Input() data: { timeRangeAndScoreArray: Array<{ from: Date, to: Date }> } = { timeRangeAndScoreArray: [] };
  
  @Input() data2 : TimeRangeAndScore2[]=[];

  constructor(private videoService: VideoService) { }

  calculateNotchPosition(tr: TimeRangeAndScore2): number {
   
    // Calculate the position of the notch based on the slider value and total time range
    const totalRange = 100; // Adjust this value as needed
    // const position = (tr.from.getHours() * 60 + tr.from.getMinutes() + tr.from.getSeconds()) / (tr.duration!) * totalRange;
    const position = (/*tr.from.getMinutes() * 60*/ + tr.from.getSeconds()) / (tr.duration!) * totalRange;
     return position;
  }
  
  calculateTimeRangeWidth(tr: TimeRangeAndScore2): number {
    
    // Convert the Date objects to timestamps (milliseconds)
    const fromTimestamp = tr.from.getTime();
    const toTimestamp = tr.to.getTime();
    // Calculate the width of the time range bar
    const totalRange = 100; // Adjust this value as needed
    const width = ((toTimestamp - fromTimestamp) / (tr.duration!* 1000)) * totalRange; // Convert milliseconds to days
    return width;
  }

  // calculateNotchPosition(date: Date | undefined): number {
  //   if(!date)
  //     return -1;
  //   // Calculate the position of the notch based on the slider value and total time range
  //   const totalRange = 100; // Adjust this value as needed
  //   const position = (date.getHours() * 60 + date.getMinutes()) / (24 * 60) * totalRange;
  //    return position;
  // }
  
  // calculateTimeRangeWidth(from: Date| undefined, to: Date| undefined): number {
  //   if(!from || !to)
  //     return -1;
  //   // Convert the Date objects to timestamps (milliseconds)
  //   const fromTimestamp = from.getTime();
  //   const toTimestamp = to.getTime();
  //   // Calculate the width of the time range bar
  //   const totalRange = 100; // Adjust this value as needed
  //   const width = ((toTimestamp - fromTimestamp) / (24 * 60 * 60 * 1000)) * totalRange * 1000; // Convert milliseconds to days
  //   return width;
  // }

  jumpTo(date: Date) {
    // const timePart = timeString.split(" ")[1];
    // const [hours, minutes, seconds] = date;
    // Convert each part to an integer.
    // const hoursInt = parseInt(hours);
    // const minutesInt = parseInt(minutes);
    // const secondsInt = parseInt(seconds);
    // // Calculate the total number of seconds in the time.
    // const totalSeconds = (hoursInt * 3600) + (minutesInt * 60) + secondsInt;
    this.videoService.jumpVideo(date.getSeconds());//totalSeconds);
  }
}
