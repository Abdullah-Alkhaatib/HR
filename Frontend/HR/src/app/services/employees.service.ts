import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Employee } from '../interfaces/employee-interface';

@Injectable({
    providedIn: 'root'
})
export class EmployeesService {

    apiUrl: string = 'https://localhost:44370/api/Employees';

    constructor(private http: HttpClient){

    }

    // Get all employees
    getAll(searchObj: any){
        let params = new HttpParams();
        params = params.set("PositionId", searchObj.positionId ?? "");
        params = params.set("EmployeeName", searchObj.name ?? "");
        params = params.set("IsActive", searchObj.status ?? "");

        return this.http.get(this.apiUrl + "/GetAll", { params } );
    }

    getManagers(employeeId?: number){
        let params = new HttpParams();
        params = params.set("EmployeeId", employeeId ?? ""); // employeeId !=null ? employeeId: ""
        return this.http.get(this.apiUrl + '/GetManagersList', { params } );
    }

    // add
    add(employee: Employee){
        return this.http.post(this.apiUrl + '/Add', employee);
    }

    // update
    update(employee: Employee){
        return this.http.put(this.apiUrl + '/Update', employee);
    }

    // delete
    delete(id: number){
        let params = new HttpParams();
        params = params.set("id", id);
        return this.http.delete(this.apiUrl + '/Delete', { params } );
    }
}
