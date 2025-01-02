import { Roomie } from './roomie';

export interface Task {
  taskId: number;
  taskName: string;
  note: string;
  dayOfWeek: string;
  isCompleted: boolean;
  taskOrder: number;
  loginId: number;
  roomies: Roomie[]; // For internal mapping
  assignedRoomies?: string[]; // Backend field for roomie names
}