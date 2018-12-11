import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
// member-messages component (child) requires recipientId which we can get from member-detail component (parent)
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService, private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages () {
    const currentUserId = +this.authService.decodedToken.nameid; // + for type conversion
    // use pipe for those messages that are marked as 'Unread'
    // tap allows us to do something before subscription
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
      .pipe(
        tap ( messages => {
          for (let i = 0; i < messages.length; i++) {
            // by default messages are unread so only read messages will be in thread
            if (messages[i].isRead === false && messages[i].recipientId === currentUserId) {
              this.userService.markAsRead(currentUserId, messages[i].id);
            }
          }
        })
      )
      .subscribe( messages => {
          this.messages = messages;
      }, error => {
        this.alertify.error(error);
      });
  }

  sendMessage () {
    this.newMessage.recipientId = this.recipientId; // we get it from parent component
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe((message: Message) => {
        // we need to add message in response into messages array to display on thread
        // using unshift to display it on top of array
        this.messages.unshift(message);
        this.newMessage.content = ''; // reset the content
      }, error => {
        this.alertify.error(error);
      });
  }

}
