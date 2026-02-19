// add-ad.component.ts
import { Component, Input } from '@angular/core';
import { AdService } from '../services/ad.service';
import { FormsModule } from '@angular/forms';
import { User } from 'libs/shared/data-access-user/src/lib/user.model';
import { Router } from '@angular/router';
import { FormFieldComponent } from '../shared/form-field/form-field.component';

@Component({
  selector: 'app-add-ad',
  imports: [FormsModule, FormFieldComponent],
  templateUrl: './add-ad.component.html',
  styleUrl: './add-ad.component.css',
})
export class AddAdComponent {
  @Input() userConnected!: User;

  title: string = '';
  description: string = '';
  category: string = '';
  location: string = '';
  poster_id: string = ''; 

  constructor(
    private adService: AdService,
    private router : Router,
    ) {}

  addAd() {
    const adData = {
      title: this.title,
      description: this.description,
      category: this.category,
      location: this.location,
      poster_id: this.userConnected.id, 
    };

    this.adService.createAd(adData).subscribe(
      (response) => {
        console.log('Ad created successfully:', response);
        this.router.navigateByUrl('list-ad');
      },
      (error) => {
        console.error('Error creating ad:', error);
      }
    );
  }
}
