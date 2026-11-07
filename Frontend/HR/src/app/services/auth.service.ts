import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    apiUrl: string = 'https://localhost:44370/api/Auth';

    constructor(private http: HttpClient) { }

    login(loginForm: any) {
        return this.http.post(this.apiUrl + '/Login', loginForm);
    }

}
