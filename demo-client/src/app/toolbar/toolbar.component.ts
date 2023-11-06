import { Component } from '@angular/core';
import { VideoService } from '../services/video.service';
import { MatDialog } from '@angular/material/dialog';
import { SelectVideoComponent } from '../select-video/select-video.component';
import { HttpErrorResponse, HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.css']
})
export class ToolbarComponent {

  constructor(public videoService: VideoService, private dialog: MatDialog) {}

  addVideoSource(){

    const dialogRef = this.dialog.open(SelectVideoComponent, {
      width: '400px', // Adjust the width as needed
      data: {} // You can pass initial data if required
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        console.log('Selected File Name:', result.selectedFileName);
        console.log('Video Details:', result.videoDetails);
        const formatDetails = `${result.videoDetails.codec}, ${result.videoDetails.resolution}, ${result.videoDetails.framerate}fps`;
        this.videoService.addVideoSource(
        {
            title: result.selectedFileName,
            formatDetails: formatDetails,
            showHideLabel: true,
            videoDetails: result.videoDetails
        });

        // this.videoService.uploadVideoFile(result.selectedFormData)
        // .subscribe(
        //   response => {
        //     console.log('File uploaded successfully', response);
        //   },
        //   error => {
        //     // Handle any errors that occurred during the upload
        //     console.error('Error uploading file', error);
        //   }
        // );
      }
    });
  }

  hideSources() {

  }
  remove() {

  }
  enrollFromImage(){

  }
  enrollFromDirectory(){
    
  }

  handleFolderSelection(event: any) {
    // Handle the selected folder
    const filesToUpload:File[] = event.target.files; // Use the first selected folder (ignore multiple selections)
    
    const formData = new FormData();
    Array.from(filesToUpload).map((file, index) => {
      return formData.append('file' + index, file, file.name);
    });

    this.videoService.uploadFaceFiles(formData).subscribe(
      (data) => {
        // Handle the data from the API response
        console.log(data);
        console.log('Files uploaded successfully', data);
      },
      (error) => {
        // Handle any errors
        console.error(error);
        console.error('Error uploading files', error);
      }
    );    
  }

  uploadFolderFiles() {

  }
}