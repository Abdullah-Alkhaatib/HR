import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class DepartmentsService { 

    apiUrl: string = 'https://localhost:44370/api/Departments';

    constructor(private http: HttpClient){

    }

    // Get Departments List
    getDepartmentsList(){
        return this.http.get(this.apiUrl + '/GetDepartmentsList');
    }

}
