# Task Manager Console App

This is a simple console-based task manager implemented in C#.
The GUI is currently in Polish, and there are plans to implement language customization for users in the future.

## Features

- Add, remove, and update tasks.
- Display tasks with optional sorting by deadline or priority.
- Save and load tasks to/from a JSON file.

## Usage

1. Run the program.
2. Choose options:
   - Add a task
   - Remove a task
   - Update task details
   - Display tasks with sorting options
   - Exit and save changes
3. Follow the prompts to interact with the task manager.

## Command Line Arguments

- Use `-d` to display tasks sorted by deadline.
- Use `-w` to display tasks sorted by priority.

## Technologies Used

- C#
- .NET
- JSON Serialization

The self-contained executable file can be found in `\bin\Release\net6.0\publish\win-x64\TaskManager.exe`
