import { Component, HostListener ,OnInit } from '@angular/core';
import { VideoService } from './services/video.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'fr-demo';
  isOptionOneLayout = true;

  public panelConfigs = [
    { title: 'Panel 1' },
    { title: 'Panel 2' },
    // Add more panel configurations as needed
  ];

  closePanel(index: number) {
    this.panelConfigs.splice(index, 1);
  }

  constructor(private videoService: VideoService) {
    this.videoService.stopAllDetectionProcesses();
  }

  ngOnInit() {
    window.onbeforeunload = () => {
      this.videoService.stopAllDetectionProcesses();
    };
  }

  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any): void {
    this.videoService.stopAllDetectionProcesses();
    $event.returnValue = true; // This line is optional and will show a confirmation dialog to the user
  }
  
  toggleLayout() {
    this.isOptionOneLayout = !this.isOptionOneLayout;
  }
}
