import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class LookupsService { 

    apiUrl: string = 'https://localhost:44370/api/Lookups';

    constructor(private http: HttpClient){

    }

    // Get By Major Code
    getByMajorCode(majorCode: number){
        let params = new HttpParams();
        params = params.set("majorCode", majorCode);
        return this.http.get(this.apiUrl + '/GetByMajorCode', { params } );
    }

}
