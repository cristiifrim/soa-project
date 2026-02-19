import { Injectable } from '@angular/core';
import { io } from 'socket.io-client';
import { Observable, map } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ChatMessage } from '../model/chatMessage.model';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private socket = io('http://localhost:3000');

  constructor(private http: HttpClient) {}

  joinChat(adId: string) {
    this.socket.emit('joinChat', adId);
  }

  sendMessage(adId: string, userId: string, userEmail: string, message: string) {
    console.log('Emitting message:', { adId, userId, userEmail, message }); 
    this.socket.emit('sendMessage', { adId, userId, userEmail, message });
    console.log('FRONTEND - Message envoyé');
  }


  getMessages(): Observable<any> {
    return new Observable((observer) => {
      this.socket.on('newMessage', (data) => {
        observer.next(data);
      });
    });
  }

  getOldMessages(adId: string): Observable<any> {
    const url = `http://localhost:3000/chat/${adId}`;

    return this.http.get<any[]>(url).pipe(
      map((response) => {
        return response.map(chatMessage => new ChatMessage(
          chatMessage.adId,
          chatMessage.userId,
          chatMessage.userEmail,
          chatMessage.message,
          chatMessage.timestamp,
        ));
      })
    );
  }

  createChatMessage(adData: any): Observable<any> {
    let apiUrl = 'http://localhost:3000/chat/create';
    console.log('adData:', adData);
    
    return this.http.post<any>(apiUrl, adData);
  }
}
