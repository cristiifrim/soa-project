import { Injectable, NgZone } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Observable, BehaviorSubject, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private connection: HubConnection;
  private notificationsList: any[] = [];
  /** Unread count for the badge; cleared when user opens the panel. */
  private unreadCount$ = new BehaviorSubject<number>(0);
  /** All received notifications (for the dropdown list). */
  readonly notifications$ = new BehaviorSubject<any[]>([]);
  private newNotification$ = new Subject<any>();

  constructor(private ngZone: NgZone) {
    this.connection = new HubConnectionBuilder()
      .withUrl('http://localhost:3000/hubs/notifications')
      .withAutomaticReconnect()
      .build();

    this.connection.start().then(() => {
      this.connection.on('NewNotification', (data: any) => {
        this.ngZone.run(() => {
          this.notificationsList = [...this.notificationsList, data];
          this.notifications$.next(this.notificationsList);
          this.unreadCount$.next(this.unreadCount$.value + 1);
          this.newNotification$.next(data);
        });
      });
    }).catch(err => console.error('SignalR notifications connection error:', err));
  }

  listenForNotifications(): Observable<any> {
    return this.newNotification$.asObservable();
  }

  getUnreadCount(): Observable<number> {
    return this.unreadCount$.asObservable();
  }

  /** Call when user opens the notifications panel so the badge resets. */
  markAsRead(): void {
    this.unreadCount$.next(0);
  }
}
