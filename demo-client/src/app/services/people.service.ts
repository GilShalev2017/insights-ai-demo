import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Face } from '../models/people';
import { WebSocketService } from './web-socket.service';

@Injectable({
  providedIn: 'root'
})
export class PeopleService {

  public faces: Face[] = [];
  private detectedFacesSubject = new BehaviorSubject<Face[]>([]);
  public detectedFaces$ = this.detectedFacesSubject.asObservable();

  private selectedFaceSubject = new BehaviorSubject<Face | null>(null);
  public selectedFace$ = this.selectedFaceSubject.asObservable();

  constructor(private webSocketService: WebSocketService) { 
    this.webSocketService.receiveFaceDetectionMessage().subscribe(onmessage => {
      const splits = onmessage.split('-');
      const img = splits[0];
      const score = splits[1];
      const name = img.substring(0, img.indexOf("."));

      const imgUrl = `assets/images/${img}`;
      this.faces.push({
        imageUrl: imgUrl,
        name: name,
        notches: []
      });
      this.updateDetectedFaces(this.faces);
    });
  }

  public updateDetectedFaces(detectedFaces: Face[]) {
    this.detectedFacesSubject.next(detectedFaces);
  }

  public selectFace(face: Face) {
    this.selectedFaceSubject.next(face);
  }
}
