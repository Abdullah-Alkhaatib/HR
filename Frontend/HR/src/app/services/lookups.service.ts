import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class LookupsService {

    apiUrl: string = `${environment.apiUrl}/Lookups`;

    constructor(private http: HttpClient) {

    }

    // Get By Major Code
    getByMajorCode(majorCode: number) {
        let params = new HttpParams();
        params = params.set("majorCode", majorCode);
        return this.http.get(this.apiUrl + '/GetByMajorCode', { params });
    }

}
