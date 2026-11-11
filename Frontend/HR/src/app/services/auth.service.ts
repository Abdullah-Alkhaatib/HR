import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    apiUrl: string = `${environment.apiUrl}/Auth`;

    constructor(private http: HttpClient) { }

    login(loginForm: any) {
        return this.http.post(this.apiUrl + '/Login', loginForm);
    }

}
