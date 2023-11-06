import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { VideoService } from '../services/video.service';
import { VideoDetails } from '../models/video-details';
@Component({
  selector: 'app-select-video',
  templateUrl: './select-video.component.html',
  styleUrls: ['./select-video.component.css']
})
export class SelectVideoComponent {
  selectedFileName: string = '';
  videoDetails?: VideoDetails|undefined = undefined; // Replace with your video details model
  selectedVideoFile: File | undefined;
  formData!: FormData;
  constructor(public dialogRef: MatDialogRef<SelectVideoComponent>, private videoService : VideoService) {}

  browseForVideo() {
    // Implement file selection logic here (e.g., using a file input)
    // Update this.videoDetails with the selected video's details
    const fileInput = document.getElementById('fileInput');
    fileInput!.click();
  }

  handleFileSelection(event: any) {
    const file: File = event.target.files[0];

    this.formData = new FormData();
    this.formData.append('file', file, file.name);

    if (file) {
      this.selectedVideoFile = file;
      this.selectedFileName = file.name;

      this.videoService.uploadVideoFile(this.formData)
      .subscribe(
        response => {
          console.log('File uploaded successfully', response);

          this.videoService.getVideoDetails( this.selectedFileName)
          .subscribe(
            response => {
              let videoDetailsData = response as VideoDetails; 
              this.videoDetails = {
                displayName: this.selectedFileName, 
                duration: videoDetailsData.duration,
                resolution: videoDetailsData.resolution,
                framerate: videoDetailsData.framerate,
                size: file.size,
                codec: videoDetailsData.codec,
              };
              console.log('Video Details Accepted', response);
            },
            error => {
              // Handle any errors that occurred during the upload
              console.error('Video Details Accepting failed', error);
            }
          );
        },
        error => {
          // Handle any errors that occurred during the upload
          console.error('Error uploading file', error);
        }
      );

      // this.videoService.getVideoDetails( this.selectedFileName)
      // .subscribe(
      //   response => {
      //     let videoDetailsData = response as VideoDetails; 
      //     this.videoDetails = {
      //       displayName: this.selectedFileName, 
      //       duration: videoDetailsData.duration,
      //       resolution: videoDetailsData.resolution,
      //       framerate: videoDetailsData.framerate,
      //       size: file.size,
      //       codec: videoDetailsData.codec,
      //     };
      //     console.log('Video Details Accepted', response);
      //   },
      //   error => {
      //     // Handle any errors that occurred during the upload
      //     console.error('Video Details Accepting failed', error);
      //   }
      // );
    }
  }

  onOkClick() {
    // Implement OK button click logic here
    // You can pass selectedFileName and videoDetails back to the parent component
    this.dialogRef.close({ selectedFileName: this.selectedFileName, videoDetails: this.videoDetails, selectedFormData: this.formData });
  }

  onCancelClick() {
    this.dialogRef.close();
  }
}
