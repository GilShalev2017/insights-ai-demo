import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import * as signalR from '@aspnet/signalr';
import {SubjectDetailsWithNameAndScores} from '../models/SubjectDetailsWithNameAndScores';
@Injectable({
  providedIn: 'root'
})
export class WebSocketService {

  private hubConnection!: HubConnection;

  public HUB_URL = "https://localhost:5001/chatHub";
  //public HUB_URL = "http://34.229.229.19:5000/chatHub";

  constructor(private http: HttpClient) {

    this.hubConnection = new HubConnectionBuilder()
       .withUrl('https://localhost:5001/chatHub', {
      //.withUrl('https://34.229.229.19:5000/chatHub', {
        transport: signalR.HttpTransportType.WebSockets,
        skipNegotiation: true,
      })
      .build();

    const startConnection = () => {
      this.hubConnection
        .start()
        .then(() => console.log('Connection started'))
        .catch(err => {
          console.error('Error while starting connection: ' + err);
          // Handle the disconnection by restarting the connection
          setTimeout(startConnection, 1000); // Retry after 1 seconds (adjust as needed)
        });
    };

    // Add an event handler to monitor the connection state
    this.hubConnection.onclose(error => {
      if (error) {
        console.error(`Connection closed with error: ${error}`);
      } else {
        console.log('Connection closed');
      }

      // Restart the connection when it's closed
      startConnection();
    });

    // Start the connection initially
    startConnection();
  }

  sendMessage(message: string) {
    this.hubConnection.invoke('SendMessage', message);
  }

  receiveFaceDetectionMessage(): Observable<string> {
    return new Observable<string>(observer => {
      this.hubConnection.on('KnownFaceDetected', (data: string) => {
        observer.next(data);
      });
    });
  }

  receiveDetectionSummaryMessage(): Observable<SubjectDetailsWithNameAndScores[]> {
    return new Observable<SubjectDetailsWithNameAndScores[]>(observer => {
      this.hubConnection.on('DetectionSummary', (data: SubjectDetailsWithNameAndScores[]) => {
        observer.next(data);
      });
    });
  }
 
  receiveStatusMessage(): Observable<string> {
    return new Observable<string>(observer => {
      this.hubConnection.on('StatusMessage', (data: string) => {
        observer.next(data);
      });
    });
  }
}
