export interface Employee {
    id: number;
    name: string;
    positionId?: number;
    positionName?: string;
    birthdate?: Date;
    isActive: boolean;
    startDate: Date;
    phone?: string;
    managerId?: number | null;
    managerName?: string | null;
    departmentId?: number;
    departmentName?: string;
    image?: any;
}