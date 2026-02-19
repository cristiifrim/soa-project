import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, switchMap, tap, throwError } from 'rxjs';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { User } from './user.model';


@Injectable({ providedIn: 'root' })
export class UserService {
  private isUserLoggedIn = new BehaviorSubject(false);
  isUserLoggedIn$ = this.isUserLoggedIn.asObservable();
  
  constructor(private http: HttpClient) {
    const userId = localStorage.getItem('user_id');
    if (userId) {
      this.isUserLoggedIn.next(true);
    }
  }


  signup(email: string, password: string) {
    const url = `http://localhost:3000/users/signup`;

    return this.http.post<any>(url, { email, password }).pipe(
      tap((response) => {
        if (response?.user_id) {
          localStorage.removeItem("user_id");
          localStorage.setItem("user_id", response.user_id);
        }
      }),
      switchMap((response) =>
        this.authentificationAPI(email, password).pipe(
          tap((authRes: any) => {
            localStorage.removeItem("token_api");
            localStorage.setItem("token_api", authRes.access_token);
            this.isUserLoggedIn.next(true);
          }),
          map(() => response)
        )
      ),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 400) console.error('Register Failed:', error);
        else console.error('Fail while register:', error);
        return throwError(() => error);
      })
    );
  }

  login(email: string, password: string) {
    const url = `http://localhost:3000/users/login`;

    return this.http.post<any>(url, { email, password }).pipe(
      tap((response) => {
        if (response?.user_id) {
          localStorage.removeItem("user_id");
          localStorage.setItem("user_id", response.user_id);
        }
      }),
      switchMap((response) =>
        this.authentificationAPI(email, password).pipe(
          tap((authRes: any) => {
            localStorage.removeItem("token_api");
            localStorage.setItem("token_api", authRes.access_token);
            this.isUserLoggedIn.next(true);
          }),
          map(() => response)
        )
      ),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) console.error('Authentication failed:', error);
        else console.error('Error while connecting:', error);
        return throwError(() => error);
      })
    );
  }

  logout() {
    localStorage.removeItem("user_id");
    localStorage.removeItem("token_api");
    this.isUserLoggedIn.next(false);
  }

  authentificationAPI(username: string, password: string) {
    const url = `http://localhost:3000/auth/login`;

    return this.http.post(url, { username, password })
      .pipe(
        catchError((error: HttpErrorResponse) => {
          if (error.status === 401) {
            console.error('Authentication failed:', error);
          } else {
            console.error('Error while connecting:', error);
          }

          return throwError(error);
        })
      );
  }

  getUserById(userId: string) {
    const url = `http://localhost:3000/users/${userId}`;

    return this.http.get<any>(url).pipe(
      map((response) => {
        const user = new User(
          userId,
          response.email,
          response.password
        );
        return user;
      }),
      tap((user) => {
        console.log('User fetched:', user);
      }),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          console.error('User not found:', error);
        } else {
          console.error('Error while fetching user:', error);
        }
        return throwError(error);  
      })
    );
  }

  getAllUsers(token: string) {
    const url = `http://localhost:3000/users/all`;

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`  
    });

    return this.http.get<any[]>(url, { headers }).pipe(
      tap((users) => {
        console.log('Users fetched:', users);
      }),
      catchError((error: HttpErrorResponse) => {
        console.error('Error while fetching users:', error);
        return throwError(error);  
      })
    );
  }

}

