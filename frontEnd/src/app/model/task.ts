import { Roomie } from './roomie';

export interface Task {
  taskId: number;
  taskName: string;
  note: string;
  isCompleted: boolean;
  dayOfWeek: string;
  taskOrder: number;
  loginId: number;
  roomies: Roomie[]; // Add this property to match backend
}