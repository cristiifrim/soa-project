import { Component, Input, inject } from '@angular/core';
import { Router } from '@angular/router';
import { User } from 'libs/shared/data-access-user/src/lib/user.model';
import { UserService } from '@frontend-mf/data-access-user';
import { NotificationsComponent } from '../notification/notification.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NotificationService } from '../services/notification.service';

@Component({
  selector: 'ng-mf-top-bar',
  imports: [CommonModule, RouterModule, NotificationsComponent],
  templateUrl: './top-bar.component.html',
  styleUrl: './top-bar.component.css'
})
export class TopBarComponent {
  private router = inject(Router);
  private userService = inject(UserService);
  @Input() userConnected?: User | null;

  showNotifications: boolean = false;
  unreadCount = 0;

  constructor(private notificationService: NotificationService) {}

  ngOnInit() {
    this.notificationService.getUnreadCount().subscribe((count) => {
      this.unreadCount = count;
    });
  }

  isUserConnected() {
    return this.userConnected != null || localStorage.getItem('user_id') !== null;
  }

  logout() {
    this.userService.logout();  
    this.router.navigateByUrl('login');  
  }

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
    if (this.showNotifications) {
      this.notificationService.markAsRead();
    }
  }

}
