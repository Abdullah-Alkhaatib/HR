import { ChangeDetectorRef, Component, ElementRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Department } from '../../interfaces/department-interface';
import { NgxPaginationModule } from 'ngx-pagination';
import { DepartmentsService } from '../../services/departments.service';
import { List } from '../../interfaces/list-interface';
import { LookupsService } from '../../services/lookups.service';
import { LookupsMajorCodes } from '../../enums/major-codes';
import { ConfirmationDialog } from '../../shared/confirmation-dialog/confirmation-dialog';
import { ToastrService } from '@relynn/ngx-toastr';

@Component({
  imports: [ReactiveFormsModule, NgxPaginationModule, ConfirmationDialog],
  selector: 'app-departments',
  styleUrl: './departments.css',
  templateUrl: './departments.html',
})
export class Departments {

  @ViewChild('closeButton') closeButton: ElementRef | undefined;

  departmentsTableColumns: string[] = ['#', 'Name', 'Floor Number', 'Type', 'Description'];

  departments: Department[] = [];

  paginationConfig = { itemsPerPage: 5, currentPage: 1 };

  addButtonDisabled: boolean = false;

  departmentTypes: List[] = [];

  showConfirmationDialog: boolean = false;

  deleteDialogTitle: string = "Delete Confirmation";
  deleteDialogBody: string = "Are you sure you want to delete this department?"

  departmentIdToBeDeleted: number | null = null;

  searchFilterForm: FormGroup = new FormGroup({
    Name: new FormControl(null),
    FloorNumber: new FormControl(null),
  });

  departmentForm: FormGroup = new FormGroup({
    Id: new FormControl(null),
    Name: new FormControl(null, [Validators.required]),
    FloorNumber: new FormControl(null),
    Description: new FormControl(null),
    TypeId: new FormControl(null, [Validators.required]),
  });

  constructor(private _departmentsService: DepartmentsService,
    private _lookupsService: LookupsService,
    private cdr: ChangeDetectorRef,
    private _toastr: ToastrService
  ) {
  }

  ngOnInit() {
    this.loadDepartments();
    this.checkRole();
  }

  loadDepartments() {

    let searchObj = {
      departmentName: this.searchFilterForm.value.Name,
      floorNumber: this.searchFilterForm.value.FloorNumber
    };
    this._departmentsService.getAll(searchObj).subscribe({
      next: (res: any) => {
        this.departments = (res ?? []).map((x: any): Department => ({
          id: x.id,
          name: x.name,
          floorNumber: x.floorNumber,
          typeId: x.typeId,
          typeName: x.typeName,
          description: x.description
        }));

        this.cdr.markForCheck();
      },
      error: err => {
        this.departments = [];
        this.cdr.markForCheck();
        console.error(err.error.message ?? err.error ?? "Unexpected Error");
      }
    });
  }

  loadSaveDialog() {
    this.clearDepartmentForm();
    this.loadDepartmentTypes();
  }

  OpenConfirmationDialog(id: number) {
    this.departmentIdToBeDeleted = id;
    this.showConfirmationDialog = true;
  }

  editDepartment(id: number) {
    this.loadSaveDialog();
    let department = this.departments.find(d => d.id === id);
    if (department != null) {
      this.departmentForm.patchValue({
        Id: department?.id,
        Name: department?.name,
        FloorNumber: department?.floorNumber,
        TypeId: department?.typeId,
        Description: department?.description,
      });
    }
  }

  changePage(pageNumber: number) {
    this.paginationConfig.currentPage = pageNumber;
  }

  clearDepartmentForm() {
    this.departmentForm.reset();
  }

  loadDepartmentTypes() {
    this.departmentTypes = [
      { id: null, name: 'Select Type' }
    ];

    this._lookupsService.getByMajorCode(LookupsMajorCodes.DepartmentTypes).subscribe({
      next: (res: any) => {
        if (res?.length > 0) {
          this.departmentTypes = this.departmentTypes.concat(
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

  saveDepartment() {

    let departmentId = this.departmentForm.value.Id ?? 0;

    let newDept: Department = {
      id: departmentId,
      name: this.departmentForm.value.Name,
      floorNumber: this.departmentForm.value.FloorNumber,
      typeId: this.departmentForm.value.TypeId,
      description: this.departmentForm.value.Description
    }

    if (!this.departmentForm.value.Id) { // Add new employee
      this._departmentsService.add(newDept).subscribe({
        next: (res: any) => {
          this.loadDepartments();
        },
        error: err => {
          console.error(err.error.message ?? err.error ?? "Unexpected Error");
        }
      });
    }
    else { // Edit existing employee
      this._departmentsService.update(newDept).subscribe({
        next: (res: any) => {
          this.loadDepartments();
        },
        error: err => {
          console.error(err.error.message ?? err.error ?? "Unexpected Error");
        }
      });
    }
    this.closeButton?.nativeElement.click();
    this.clearDepartmentForm();
  }

  confirmDepartmentDelete(isConfirmed: boolean) {
    if (isConfirmed) {
      this.removeDepartment();
    }

    this.departmentIdToBeDeleted = null;
    this.showConfirmationDialog = false;
  }

  removeDepartment() {
    if (this.departmentIdToBeDeleted !== null) {
      this._departmentsService.delete(this.departmentIdToBeDeleted).subscribe({
        next: (res: any) => {
          this.loadDepartments();
          this._toastr.success("Department deleted successfully");
        },
        error: err => {
          this._toastr.error(err.error.message ?? err.error ?? "Unexpected Error");
        }
      });
    }
  }

  checkRole() {
    let role = localStorage.getItem('role');

    if (role?.toUpperCase() !== 'ADMIN' && role?.toUpperCase() !== 'HR') {
      this.addButtonDisabled = true;
    }
  }

}
