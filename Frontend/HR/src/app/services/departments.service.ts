import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Department } from '../interfaces/department-interface';

@Injectable({
    providedIn: 'root'
})
export class DepartmentsService {

    apiUrl: string = 'https://localhost:44370/api/Departments';

    constructor(private http: HttpClient) {

    }

    // Get all Departments
    getAll(searchObj: any) {
        let params = new HttpParams();

        if (searchObj.departmentName != null) {
            params = params.set("DepartmentName", searchObj.departmentName);
        }

        if (searchObj.floorNumber != null) {
            params = params.set("FloorNumber", searchObj.floorNumber);
        }

        return this.http.get(this.apiUrl + '/GetAll', { params });
    }

    // add
    add(department: Department) {
        return this.http.post(this.apiUrl + '/Add', department);
    }

    // Get Departments List
    getDepartmentsList() {
        return this.http.get(this.apiUrl + '/GetDepartmentsList');
    }

    // update
    update(department: Department) {
        return this.http.put(this.apiUrl + '/Update', department);
    }

    // delete
    delete(id: number) {
        let params = new HttpParams();
        params = params.set("id", id);

        return this.http.delete(this.apiUrl + '/Delete', { params });
    }

}
