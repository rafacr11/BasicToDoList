using System.Collections.Generic;

namespace BasicToDoList.Models.ViewModels
{
    public class TodoViewModel
    {
        public List<ToDoItem> TodoList { get; set; }
        public ToDoItem ToDo { get; set; }

    }
}
