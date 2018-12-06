import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  // @Output() getMemberPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMain: Photo;
  photoUrl: string;

  constructor(private authService: AuthService, private userService: UserService,
      private alertify: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader( ) {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024 // 10 mb image
    });
    // tell uploader that file should be without credentials
    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };

    // to show picture in menu soon after uploading 
    this.uploader.onSuccessItem = (item, response, status, Headers) => {
      if (response) {
        const res: Photo = JSON.parse(response); // convert string (response) into json object as Photo is..
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          isMain: res.isMain,
          description: res.description
        };
        // now add photo to photos array
        this.photos.push(photo);
      }
    };

  }

  setMainPhoto (photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      // set the currently selected photo as enabled
      // use array filter method, it returns array as output
      this.currentMain = this.photos.filter(p => p.isMain === true)[0];
      this.currentMain.isMain = false;
      photo.isMain = true;
      // change profile photo by sending it to parent member
      // this.getMemberPhotoChange.emit(photo.url);
      this.authService.changeMemberPhoto(photo.url); // using photoUrl subjectbehavior observable
      // update the user in local storage so that it persist upon refreshing page
      this.authService.currentUser.photoUrl = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser)); // overwrite localstorage user
    }, error => {
      this.alertify.error(error);
    });
  }

  deletePhoto (id: number) {
    this.alertify.confirm('Are you sure you want to delete', () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
        /*remove image from array*/
        this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
        this.alertify.success('Photo has been deleted');
      }, error => {
        this.alertify.error('Failed to delete photo');
      });
    });
  }

}
