import { Component, Input, OnInit } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { User } from 'libs/shared/data-access-user/src/lib/user.model';
import { Ad } from '../model/ad.model';
import { AdService } from '../services/ad.service';


@Component({
    standalone: true,
    imports: [CommonModule, FormsModule],
    selector: 'app-chat',
    templateUrl: './chat.component.html',
    styleUrls: ['./chat.component.css'],
})
export class ChatComponent implements OnInit {
    @Input() adId!: string;
    @Input() userConnected!: User;
    messages: any[] = [];
    newMessage = '';

    ad!: Ad;

    constructor(
        private chatService: ChatService,
        private adService: AdService,
        ) { }

    addChatMessage() {
        const chatData = {
            adId: this.adId,
            userId: this.userConnected.id,
            userEmail: this.userConnected.email,
            message: this.newMessage,
            timestamp: new Date(),

        };

        this.chatService.createChatMessage(chatData).subscribe(
            (response) => {
                console.log('ChatMessage created successfully:', response);
            },
            (error) => {
                console.error('Error creating ChatMessage:', error);
            }
        );
    }

    async ngOnInit() {
        console.log('User connected:', this.userConnected);

        this.adService.getAdById(this.adId).subscribe((ad) => {
            this.ad = ad;
        });
        
        this.chatService.getOldMessages(this.adId).subscribe((messages) => {
            this.messages = messages;
        });

        // Wait for connection and join chat before subscribing to messages
        await this.chatService.joinChat(this.adId);
        console.log('Joined chat for adId:', this.adId);
        
        this.chatService.getMessages().subscribe({
            next: (message) => {
                console.log('ChatComponent received message:', message);
                if (message.timestamp == null) {
                  message = { ...message, timestamp: new Date() };
                }
                this.messages.push(message);
                console.log('Total messages now:', this.messages.length);
            },
            error: (err) => {
                console.error('Error in getMessages subscription:', err);
            }
        });
    }

    sendMessage() {
        this.addChatMessage();
        this.chatService.sendMessage(this.adId, this.userConnected.id, this.userConnected.email, this.newMessage);
        this.newMessage = '';
    }

    isAdPoster(message: any) {
        return this.ad?.poster_id === message.userId;
    }

    isMessageAuthor(message: any) {
        return this.userConnected.id === message.userId;
    }

}
