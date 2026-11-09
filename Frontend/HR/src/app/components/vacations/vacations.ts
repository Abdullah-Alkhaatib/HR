import { ChangeDetectorRef, Component, ElementRef, ViewChild } from '@angular/core';
import { ConfirmationDialog } from '../../shared/confirmation-dialog/confirmation-dialog';
import { NgxPaginationModule } from 'ngx-pagination';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';
import { List } from '../../interfaces/list-interface';
import { LookupsService } from '../../services/lookups.service';
import { EmployeesService } from '../../services/employees.service';
import { ToastrService } from '@relynn/ngx-toastr';
import { LookupsMajorCodes } from '../../enums/major-codes';
import { Vacation } from '../../interfaces/vacation-interface';
import { VacationService } from '../../services/vacation.service';

@Component({
  imports: [CommonModule, ReactiveFormsModule, NgxPaginationModule, ConfirmationDialog],
  providers: [DatePipe],
  selector: 'app-vacations',
  styleUrl: './vacations.css',
  templateUrl: './vacations.html',
})
export class Vacations {

  @ViewChild('closeButton') closeButton: ElementRef | undefined;

  vacationsTableColumns: string[] = ['#', 'Employee Name', 'Start Date', 'End Date', 'Type', 'Creation Date', 'Notes'];

  vacations: Vacation[] = [];

  paginationConfig = { itemsPerPage: 5, currentPage: 1 };

  addButtonDisabled: boolean = false;

  vacationTypesList: List[] = [];
  employeesList: List[] = [];

  showConfirmationDialog: boolean = false;

  deleteDialogTitle: string = "Delete Confirmation";
  deleteDialogBody: string = "Are you sure you want to delete this vacation?"

  vacationIdToBeDeleted: number | null = null;

  searchFilterForm: FormGroup = new FormGroup({
    TypeId: new FormControl(null),
    EmployeeId: new FormControl(null),
  });

  vacationForm: FormGroup = new FormGroup({
    Id: new FormControl(null),
    StartDate: new FormControl(null, [Validators.required]),
    EndDate: new FormControl(null, [Validators.required]),
    EmployeeId: new FormControl(null, [Validators.required]),
    TypeId: new FormControl(null, [Validators.required]),
    Notes: new FormControl(null),
  });

  constructor(private _vacationsService: VacationService,
    private _employeesService: EmployeesService,
    private _lookupsService: LookupsService,
    private _datePipe: DatePipe,
    private cdr: ChangeDetectorRef,
    private _toastr: ToastrService
  ) {
  }

  ngOnInit() {
    this.loadVacations();
    this.loadEmployeesList();
    this.loadVacationTypes();
  }

  loadVacations() {
    let searchObj = {
      vacationTypeId: this.searchFilterForm.value.TypeId,
      employeeId: this.searchFilterForm.value.EmployeeId
    };
    this._vacationsService.getAll(searchObj).subscribe({
      next: (res: any) => {
        this.vacations = (res ?? []).map((x: any): Vacation => ({
          id: x.id,
          startDate: x.startDate,
          endDate: x.endDate,
          creationDate: x.creationDate,
          typeId: x.typeId,
          typeName: x.typeName,
          employeeId: x.employeeId,
          employeeName: x.employeeName,
          notes: x.notes
        }));

        this.cdr.markForCheck();
      },
      error: err => {
        this.vacations = [];
        this.cdr.markForCheck();
        console.error(err.error.message ?? err.error ?? "Unexpected Error");
      }
    });
  }

  loadSaveDialog() {
    this.clearVacationForm();
    this.loadVacationTypes();
    this.loadEmployeesList();
  }

  OpenConfirmationDialog(id: number) {
    this.vacationIdToBeDeleted = id;
    this.showConfirmationDialog = true;
  }

  editVacation(id: number) {
    this.loadSaveDialog();
    let vacation = this.vacations.find(d => d.id === id);
    if (vacation != null) {
      this.vacationForm.patchValue({
        Id: vacation?.id,
        EmployeeId: vacation?.employeeId,
        TypeId: vacation?.typeId,
        StartDate: this._datePipe.transform(vacation?.startDate, 'yyyy-MM-dd'),
        EndDate: this._datePipe.transform(vacation?.endDate, 'yyyy-MM-dd'),
        Notes: vacation?.notes,
      });
    }
  }

  changePage(pageNumber: number) {
    this.paginationConfig.currentPage = pageNumber;
  }

  clearVacationForm() {
    this.vacationForm.reset();
  }

  loadVacationTypes() {
    this.vacationTypesList = [
      { id: null, name: 'Select Type' }
    ];

    this._lookupsService.getByMajorCode(LookupsMajorCodes.VacationTypes).subscribe({
      next: (res: any) => {
        if (res?.length > 0) {
          this.vacationTypesList = this.vacationTypesList.concat(
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
    })
  }

  loadEmployeesList() {
    this.employeesList = [
      { id: null, name: "Select Employees" }
    ];

    let role = localStorage.getItem("role");
    let currentUserOnly = false;
    if (role?.toUpperCase() != "ADMIN" && role?.toUpperCase() != "HR") {
      currentUserOnly = true;
    }
    this._employeesService.getEmployeesList(currentUserOnly).subscribe({
      next: (res: any) => {
        if (res?.length > 0) {
          this.employeesList = this.employeesList.concat(
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
    })
  }

  saveVacation() {

    let departmentId = this.vacationForm.value.Id ?? 0;

    let newDept: Vacation = {
      id: departmentId,
      startDate: this.vacationForm.value.StartDate,
      endDate: this.vacationForm.value.EndDate,
      typeId: this.vacationForm.value.TypeId,
      employeeId: this.vacationForm.value.EmployeeId,
      notes: this.vacationForm.value.Notes
    }

    if (!this.vacationForm.value.Id) { // Add new Vacation
      this._vacationsService.add(newDept).subscribe({
        next: (res: any) => {
          this.loadVacations();
        },
        error: err => {
          console.error(err.error.message ?? err.error ?? "Unexpected Error");
        }
      });
    }
    else { // Edit existing employee
      this._vacationsService.update(newDept).subscribe({
        next: (res: any) => {
          this.loadVacations();
        },
        error: err => {
          console.error(err.error.message ?? err.error ?? "Unexpected Error");
        }
      });
    }
    this.closeButton?.nativeElement.click();
    this.clearVacationForm();
  }

  confirmVacationDelete(isConfirmed: boolean) {
    if (isConfirmed) {
      this.removeVacation();
    }

    this.vacationIdToBeDeleted = null;
    this.showConfirmationDialog = false;
  }

  removeVacation() {
    if (this.vacationIdToBeDeleted !== null) {
      this._vacationsService.delete(this.vacationIdToBeDeleted).subscribe({
        next: (res: any) => {
          this.loadVacations();
          this._toastr.success("Vacation deleted successfully");
        },
        error: err => {
          this._toastr.error(err.error.message ?? err.error ?? "Unexpected Error");
        }
      });
    }
  }

}
