import { Component, ElementRef, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';
import { Employee } from '../../interfaces/employee-interface';
import { ConfirmationDialog } from '../../shared/confirmation-dialog/confirmation-dialog';
import { EmployeesService } from '../../services/employees.service';
import { List } from '../../interfaces/list-interface';
import { DepartmentsService } from '../../services/departments.service';
import { LookupsService } from '../../services/lookups.service';
import { LookupsMajorCodes } from '../../enums/major-codes';
import { ToastrService } from '@relynn/ngx-toastr';

@Component({
  imports: [CommonModule, ReactiveFormsModule, NgxPaginationModule, ConfirmationDialog],
  providers: [DatePipe],
  selector: 'app-employees',
  styleUrl: './employees.css',
  templateUrl: './employees.html',
})

export class Employees implements OnInit {

  @ViewChild('closeButton') closeButton: ElementRef | undefined;

  employees: Employee[] = [];

  employeeTableColumns: string[] = [
    '#',
    'Image',
    'Name',
    'Phone',
    'Birthdate',
    'Status',
    'Start Date',
    'Position',
    'Department',
    'Manager',
  ]

  departments: List[] = [];

  positions: List[] = [];

  managers: List[] = [];

  employeeStatusList: List[] = [
    { id: null, name: 'Select Status' },
    { id: true, name: 'Active' },
    { id: false, name: 'Inactive' }
  ]

  employeeForm: FormGroup = new FormGroup({
    Id: new FormControl(null),
    Name: new FormControl(null, [Validators.required]),
    Phone: new FormControl(null, [Validators.required]),
    Birthdate: new FormControl(null, [Validators.required]),
    StartDate: new FormControl(null, [Validators.required]),
    Department: new FormControl(null, [Validators.required]),
    Manager: new FormControl(null),
    Position: new FormControl(null, [Validators.required]),
    IsActive: new FormControl(true, [Validators.required]),
    Image: new FormControl(null)
  });

  searchFilterForm: FormGroup = new FormGroup({
    Name: new FormControl(null),
    PositionId: new FormControl(null),
    Status: new FormControl(null),
  })

  paginationConfig = {
    itemsPerPage: 5,
    currentPage: 1,
  }

  deleteDialogTitle: string = "Confirm Delete";
  deleteDialogBody: string = "Are you sure you want to delete this employee?";
  showConfirmationDialog: boolean = false;
  employeeIdToBeDeleted: number | null = null;

  constructor(private _datePipe: DatePipe,
    private _employeeService: EmployeesService,
    private _departmentsService: DepartmentsService,
    private cdr: ChangeDetectorRef,
    private _lookupsService: LookupsService,
    private _toastr: ToastrService) {

  }

  ngOnInit(): void {
    this.loadEmployees();
    this.loadPositionsList();
  }

  ngOnDestroy(): void {
    console.log("Employees Component Destroyed");
  }

  uploadImage(event: any) {
    this.employeeForm.patchValue({
      Image: event.target.files[0]
    });
  }

  loadEmployees() {

    let searchObj = {
      name: this.searchFilterForm.value.Name,
      positionId: this.searchFilterForm.value.PositionId,
      status: this.searchFilterForm.value.Status
    };
    this._employeeService.getAll(searchObj).subscribe({
      next: (res: any) => {
        this.employees = (res ?? []).map((x: any): Employee => ({
          id: x.id,
          name: x.name,
          phone: x.phone,
          birthdate: x.birthDate,
          isActive: x.isActive,
          startDate: x.startDate,
          positionId: x.positionId,
          positionName: x.positionName,
          departmentId: x.departmentId,
          departmentName: x.departmentName,
          managerId: x.managerId,
          managerName: x.managerName
        }));

        this.cdr.markForCheck();
      },
      error: err => {
        this.employees = [];
        this.cdr.markForCheck();
        console.error(err.error.message ?? err.error ?? "Unexpected Error");
      }
    });
  }

  loadManagersList(employeeId?: number) {
    this.managers = [
      { id: null, name: 'Select Manager' }
    ];

    this._employeeService.getManagers(employeeId).subscribe(
      {
        next: (res: any) => { // successful Request
          if (res?.length > 0) {
            res.forEach((x: any) => {
              let manager: List = {
                id: x.id,
                name: x.name
              }
              this.managers.push(manager);
            });
          }
        },
        error: err => { // Failed Request | 400, 500
          console.error(err.error.message ?? err.error ?? "Unexpected Error");
        }
      }
    )
  }

  loadDepartmentsList() {
    this._departmentsService.getDepartmentsList().subscribe({
      next: (res: any) => {
        this.departments = [
          { id: null, name: 'Select Department' },
          ...res
        ];

        this.cdr.detectChanges();
      },
      error: err => {
        console.error(err.error.message ?? err.error ?? "Unexpected Error");
      }
    });
  }

  loadPositionsList() {
    this.positions = [
      { id: null, name: 'Select Position' }
    ];

    this._lookupsService.getByMajorCode(LookupsMajorCodes.Position).subscribe({
      next: (res: any) => {
        if (res?.length > 0) {
          this.positions = this.positions.concat(
            res.map((x: any) => ({
              id: x.id,
              name: x.name
            } as List))
          )
        }
      },
      error: err => {
        console.error(err.error.message ?? err.error ?? "Unexpected Error");
      }
    });
  }

  OpenConfirmationDialog(empId: number) {
    this.employeeIdToBeDeleted = empId; // save employee id to be used later
    this.showConfirmationDialog = true; // show confirmation dialog
  }

  saveEmployee() {

    let employeeId = this.employeeForm.value.Id ?? 0;

    let newEmployee: Employee = {
      id: employeeId,
      name: this.employeeForm.value.Name,
      phone: this.employeeForm.value.Phone,
      birthdate: this.employeeForm.value.Birthdate,
      startDate: this.employeeForm.value.StartDate,
      isActive: this.employeeForm.value.IsActive,
      departmentId: this.employeeForm.value.Department,
      managerId: this.employeeForm.value.Manager,
      positionId: this.employeeForm.value.Position,
      image: this.employeeForm.value.Image
    }

    if (!this.employeeForm.value.Id) { // Add new employee
      this._employeeService.add(newEmployee).subscribe({
        next: (res: any) => {
          this.loadEmployees();
        },
        error: err => {
          console.error(err.error.message ?? err.error ?? "Unexpected Error");
        }
      });
    }
    else { // Edit existing employee
      this._employeeService.update(newEmployee).subscribe({
        next: (res: any) => {
          this.loadEmployees();
        },
        error: err => {
          console.error(err.error.message ?? err.error ?? "Unexpected Error");
        }
      });
    }
    this.closeButton?.nativeElement.click();
    this.clearEmployeeForm();
  }

  clearEmployeeForm() {
    this.employeeForm.reset({
      IsActive: true
    });
  }

  editEmployee(id: number) {
    this.loadSaveDialog(id);
    let employee = this.employees.find(e => e.id === id);
    if (employee != null) {
      this.employeeForm.patchValue({
        Id: employee?.id,
        Name: employee?.name,
        Phone: employee?.phone,
        StartDate: this._datePipe.transform(employee?.startDate, 'yyyy-MM-dd'),
        Birthdate: this._datePipe.transform(employee?.birthdate, 'yyyy-MM-dd'),
        Department: employee?.departmentId,
        Manager: employee?.managerId,
        Position: employee?.positionId,
        IsActive: employee?.isActive,
      });
    }
  }

  removeEmployee() {
    if (this.employeeIdToBeDeleted) {
      this._employeeService.delete(this.employeeIdToBeDeleted).subscribe({
        next: (res: any) => {
          this.loadEmployees();
        },
        error: err => {
          this._toastr.error(err.error.message ?? err.error ?? "Unexpected Error");
        }
      });
    }
  }

  changePage(pageNumber: number) {
    this.paginationConfig.currentPage = pageNumber;
  }

  confirmEmployeeDelete(isConfirmed: boolean) {
    // if (isConfirmed && this.employeeIdToDelete !== null) {
    //   this.employees = this.employees.filter(e => e.id !== this.employeeIdToDelete);
    // }
    // this.showConfirmationDialog = false;
    // this.employeeIdToBeDeleted = null;

    if (isConfirmed) {
      this.removeEmployee();
    }

    this.employeeIdToBeDeleted = null;
    this.showConfirmationDialog = false;
  }

  loadSaveDialog(employeeId?: number) {
    this.clearEmployeeForm();
    this.loadManagersList(employeeId);
    this.loadDepartmentsList();
    this.loadPositionsList();
  }

}