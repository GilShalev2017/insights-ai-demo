import { Component } from '@angular/core';

@Component({
  selector: 'app-main-menu',
  templateUrl: './main-menu.component.html',
  styleUrls: ['./main-menu.component.css']
})
export class MainMenuComponent {
  enrollFromImage() {
    // Implement "Enroll from Image(s)" action here
    console.log('Enroll from Image(s) clicked');
  }

  enrollFromDirectory() {
    // Implement "Enroll from Directory" action here
    console.log('Enroll from Directory clicked');
  }

  openSettings() {
    // Implement "Settings" action here
    console.log('Settings clicked');
  }

  openWatchList() {
    // Implement "Watch List" action here
    console.log('Watch List clicked');
  }

  clearWatchList() {
    // Implement "Clear Watch List" action here
    console.log('Clear Watch List clicked');
  }

  exportEvents() {
    // Implement "Export Events" action here
    console.log('Export Events clicked');
  }

  optimizeModels() {
    // Implement "Optimize Models" action here
    console.log('Optimize Models clicked');
  }

  aboutApp() {
    // Implement "About this app" action here
    console.log('About this app clicked');
  }

}
