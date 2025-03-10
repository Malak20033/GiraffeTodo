document.addEventListener("DOMContentLoaded", function () {
    const addTaskButton = document.getElementById("add-task-btn");
    const newTaskInput = document.getElementById("new-task");
    const todoListUl = document.getElementById("todo-list-ul");

    // Fetch current todos from the backend and display them
    async function fetchTodos() {
        const response = await fetch("/api/todos");
        const todos = await response.json();
        displayTodos(todos);
    }

    // Add a new todo to the backend and update the list
    async function addTodo() {
        const newTask = newTaskInput.value;
        if (!newTask) return;

        const todo = {
            Task: newTask,
            IsCompleted: false
        };

        await fetch("/api/todos", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(todo)
        });

        // Clear input and refresh the list
        newTaskInput.value = "";
        fetchTodos();
    }

    // Display todos in the list
    function displayTodos(todos) {
        todoListUl.innerHTML = ""; // Clear the list before adding items
        todos.forEach(todo => {
            const li = document.createElement("li");
            li.textContent = todo.Task;
            todoListUl.appendChild(li);
        });
    }

    addTaskButton.addEventListener("click", addTodo);
    fetchTodos();
});