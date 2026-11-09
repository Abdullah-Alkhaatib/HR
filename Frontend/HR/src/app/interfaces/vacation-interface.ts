export interface Vacation {
    id: number;
    startDate: Date;
    endDate: Date;
    typeId: number;
    typeName?: string;
    employeeId: number;
    employeeName?: string;
    notes?: string;
    creationDate?: Date;
}