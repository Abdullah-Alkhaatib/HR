import { Injectable, } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Vacation } from '../interfaces/vacation-interface';

@Injectable({
    providedIn: 'root'
})
export class VacationService {

    apiUrl: string = 'https://localhost:44370/api/Vacations';

    constructor(private _http: HttpClient) {
    }

    getAll(searchObj: any) {

        let params = new HttpParams();
        params = params.set("VacationTypeId", searchObj.vacationTypeId ?? "");
        params = params.set("EmployeeId", searchObj.employeeId ?? "");

        return this._http.get(this.apiUrl + "/GetAll", { params });
    }

    add(vacation: Vacation) {

        return this._http.post(this.apiUrl + "/Add", vacation);
    }

    update(vacation: Vacation) {

        return this._http.put(this.apiUrl + "/Update", vacation);
    }

    delete(id: number) {
        let params = new HttpParams();
        params = params.set("id", id);
        return this._http.delete(this.apiUrl + "/Delete", { params });
    }

}
