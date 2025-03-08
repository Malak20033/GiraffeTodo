namespace Models

module Todo = 
    type TodoItem = {
        Id: int
        Task: string
        IsCompleted: bool
    }

    let createTodoItem id task =
        { Id = id; Task = task; IsCompleted = false }

    let completeTodoItem todo =
        { todo with IsCompleted = true }