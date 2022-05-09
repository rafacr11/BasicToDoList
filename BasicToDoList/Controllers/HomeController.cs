using BasicToDoList.Models;
using BasicToDoList.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BasicToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var todoListViewModel = GetAllTodos();
            return View(todoListViewModel);
        }


        public RedirectResult Update (ToDoItem todo)
        {
            using (SqliteConnection con = new("Data Source=db.db"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"UPDATE todo SET name = '{todo.Name}' WHERE Id = '{todo.Id}'";
                    try
                    {
                        tableCmd.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

            }

            return Redirect("https://localhost:44368");
        }
        [HttpGet]
        public JsonResult PopulateForm (int id)
        {
            var todo = GetById(id);
            return Json(todo);
        }

        internal ToDoItem GetById(int id)
        {
            ToDoItem todo = new();
            
            using (SqliteConnection con = new("Data Source=db.db"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"SELECT * FROM todo WHERE Id = '{id}'";

                    using (var reader = tableCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            todo.Id = reader.GetInt32(0);
                            todo.Name = reader.GetString(1);
                        }
                        else
                        {
                            return todo;
                        }
                    }
                }
            }
            return todo;

        }

        [HttpPost]
        public JsonResult Delete (int id)
        {
            using (SqliteConnection con = new("Data Source=db.db"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"DELETE FROM todo WHERE Id ='{id}'";
                    tableCmd.ExecuteNonQuery();
                }

            }

            return Json(new {});

        }

        internal TodoViewModel GetAllTodos()
        {
            List<ToDoItem> toDoList = new();
            using (SqliteConnection con = new("Data Source=db.db"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = "SELECT * FROM todo";

                    using(var reader = tableCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                toDoList.Add(
                                    new ToDoItem
                                    {
                                        Id = reader.GetInt32(0),
                                        Name =  reader.GetString(1)
                                    });
                            }
                        }
                        else
                        {
                            return new TodoViewModel
                            {
                                TodoList = toDoList
                            };
                        }
                    };
                }
            }

            return new TodoViewModel
            {
                TodoList = toDoList
            };

        }

        public RedirectResult Insert(ToDoItem todo)
        {
            using (SqliteConnection con = new("Data Source=db.db"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"INSERT INTO todo (name) VALUES ('{todo.Name}')";

                    try
                    {
                        tableCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                return Redirect("https://localhost:44368");
            }
        }
       
    }
}
