export class Todo{
    id: string;
    title: string;
    description: string;
    isCompleted: boolean;
    dueDate: string;
    createdAt: string;
    updatedAt: string;
    userId: any;

    constructor(){
        this.id = '';
        this.title = '';
        this.description = '';
        this.isCompleted = false;
        this.dueDate = '';
        this.createdAt = '';
        this.updatedAt = '';
        this.userId = '';
    }
}