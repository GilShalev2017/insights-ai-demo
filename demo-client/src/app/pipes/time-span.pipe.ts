import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeSpan'
})
export class TimeSpanPipe implements PipeTransform {

  transform(value: string | undefined): string {
    if (value == undefined)
      return "";

    const timeSpan = new Date(value);

    // Extract hours, minutes, and seconds
    const hours = timeSpan.getUTCHours();
    const minutes = timeSpan.getUTCMinutes();
    const seconds = timeSpan.getUTCSeconds();

    // Format the time as hh:mm:ss
    return (
      this.padZero(hours) + ':' +
      this.padZero(minutes) + ':' +
      this.padZero(seconds)
    );
  }

  private padZero(value: number): string {
    return value < 10 ? '0' + value : value.toString();
  }
}
