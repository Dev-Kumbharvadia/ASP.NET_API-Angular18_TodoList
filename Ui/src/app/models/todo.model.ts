export interface TodoItem {
    id: string; // Unique identifier for the todo item
    title: string; // Title of the todo item
    description?: string | null; // Detailed description of the todo item (optional)
    isCompleted: boolean; // Indicates whether the task is completed
    dueDate?: Date | null; // The due date for the task (optional)
    createdAt: Date; // Timestamp of when the task was created
    updatedAt: Date; // Timestamp of when the task was last updated
    userId?: number | null; // (Optional) Foreign key to link to a user
}
