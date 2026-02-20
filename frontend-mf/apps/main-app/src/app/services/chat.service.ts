import { Injectable, NgZone } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Observable, map, Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ChatMessage } from '../model/chatMessage.model';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private connection: HubConnection;
  private connectionPromise: Promise<void>;
  /** Single shared stream for new messages so we only register one SignalR listener (avoids doubled messages). */
  private newMessages$ = new Subject<any>();

  constructor(
    private http: HttpClient,
    private ngZone: NgZone,
  ) {
    // Verify SignalR is available (from CDN)
    if (typeof window !== 'undefined' && (window as any).signalR) {
      console.log('SignalR CDN loaded successfully');
    } else {
      console.error('SignalR CDN not loaded! Check index.html script tag.');
    }

    this.connection = new HubConnectionBuilder()
      .withUrl('http://localhost:3000/hubs/chat')
      .withAutomaticReconnect()
      .build();

    // Store the connection promise and register a single NewMessage listener
    this.connectionPromise = this.connection.start()
      .then(() => {
        console.log('SignalR chat connection started, state:', this.connection.state);
        this.connection.on('NewMessage', (data: any) => {
          this.ngZone.run(() => this.newMessages$.next(data));
        });
      })
      .catch(err => {
        console.error('SignalR chat connection error:', err);
      });
  }

  private async ensureConnected(): Promise<void> {
    const state = this.connection.state;
    console.log('Connection state before ensureConnected:', state);
    
    if (state === 'Disconnected') {
      console.log('Starting SignalR connection...');
      await this.connection.start();
      console.log('Connection started, new state:', this.connection.state);
    }
    
    // Wait for connection to be fully established (max 5 seconds)
    let attempts = 0;
    while (this.connection.state === 'Connecting' && attempts < 50) {
      await new Promise(resolve => setTimeout(resolve, 100));
      attempts++;
    }
    
    if (this.connection.state !== 'Connected') {
      console.warn('Connection not in Connected state:', this.connection.state);
    } else {
      console.log('Connection confirmed Connected');
    }
  }

  async joinChat(adId: string) {
    await this.ensureConnected();
    this.connection.invoke('JoinChat', adId)
      .catch(err => console.error('JoinChat error:', err));
  }

  async sendMessage(adId: string, userId: string, userEmail: string, message: string) {
    await this.ensureConnected();
    console.log('Invoking SendMessage:', { adId, userId, userEmail, message });
    this.connection.invoke('SendMessage', adId, userId, userEmail, message)
      .catch(err => console.error('SendMessage error:', err));
    console.log('FRONTEND - Message envoyé');
  }

  getMessages(): Observable<any> {
    return this.newMessages$.asObservable();
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
