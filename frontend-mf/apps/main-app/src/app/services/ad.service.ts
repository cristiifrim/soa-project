// ad.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Ad } from '../model/ad.model';

@Injectable({
  providedIn: 'root',
})
export class AdService {
  private apiUrl = 'http://localhost:3000/ads/create';

  constructor(private http: HttpClient) {}

  createAd(adData: any): Observable<any> {
    console.log('adData:', adData);
    
    return this.http.post<any>(this.apiUrl, adData);
  }

  getAllAds(token: string): Observable<Ad[]> {
    const url = `http://localhost:3000/ads/all`;

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`  
    });

    return this.http.get<any[]>(url, { headers }).pipe(
      map((response) => {
        return response.map(ad => new Ad(
          ad._id,
          ad.title,
          ad.description,
          ad.poster_id,
          ad.location,
          ad.category,
          ad.created_at,
          ad.updated_at
        ));
      })
    );
  }

  getAdById(id: string): Observable<Ad> {
    const url = `http://localhost:3000/ads/${id}`;

    return this.http.get<any>(url).pipe(
      map((ad) => new Ad(
        String(ad._id),
        ad.title,
        ad.description,
        ad.poster_id,
        ad.location,
        ad.category,
        ad.created_at,
        ad.updated_at
      ))
    );
  }

}
