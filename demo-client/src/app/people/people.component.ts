import { Component, OnInit } from '@angular/core';
import { Face } from '../models/people';
import { PeopleService } from '../services/people.service';

@Component({
  selector: 'app-people',
  templateUrl: './people.component.html',
  styleUrls: ['./people.component.css']
})
export class PeopleComponent implements OnInit {

  numPeopleDetected = 0;
  detectedFaces: Face[] = [];
  selectedFace: Face | null = null;

  constructor(private peopleService: PeopleService) { }

  ngOnInit() {
    this.peopleService.detectedFaces$.subscribe(detectedFaces => {
      this.detectedFaces = detectedFaces;
      this.numPeopleDetected = detectedFaces.length;
    });

    this.peopleService.selectedFace$.subscribe(selectedFace => {
      this.selectedFace = selectedFace;
      this.unselectAllOthers();
      if(this.selectedFace?.isSelected)
        this.selectedFace!.isSelected = true;
    });
  }

  unselectAllOthers() {
   this.detectedFaces.forEach(face => {
      face.isSelected = false;
    });
  }
  selectFace(face: Face) {
    this.peopleService.selectFace(face);
  }

  jumpToTime(time: number) {
    // TODO: Implement this method to jump to the specified time in the video.
  }
}
