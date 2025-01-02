  export interface Task {
    taskId: number;           // Maps to TaskID in the database
    taskName: string;         // Maps to TaskName
    assignedRoomie?: number | null; // Changed from string to number | null
    note?: string;             // Task details or notes
    isCompleted: boolean;     // Task completion status
    dayOfWeek: string;        // Day the task is scheduled for
    taskOrder: number;        // Order of the task within the day
    LoginID: number;
  }
