using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CRUD_NETC_ORE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace CRUD_NETC_ORE.Controllers
{
    public class HomeController : Controller
    {
        public IConfiguration Configuration { get; }
        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IActionResult Index()
        {
            List<Table> tablesList = new List<Table>();
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //sql Data Reader
                connection.Open();
                string sql = "SELECT * FROM [TABLE] ";
                SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while(dataReader.Read())
                    {
                        Table table = new Table();
                        table.Id = Convert.ToInt32(dataReader["Id"]);
                        table.Name = Convert.ToString(dataReader["Name"]);
                        table.Skills = Convert.ToString(dataReader["Skills"]);
                        table.TotalStudents = Convert.ToInt32(dataReader["TotalStudents"]);
                        table.Salary = Convert.ToDecimal(dataReader["salary"]);
                        table.AddedOn = Convert.ToDateTime(dataReader["AddedOn"]);
                        tablesList.Add(table);
                    }
                }
                connection.Close();
            }
                
            return View(tablesList);
        }
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Create_Post(Table table)
        {
            if (!ModelState.IsValid)
            {
                string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = $"Insert Into [Table] (Name, Skills, TotalStudents, Salary) Values ('{table.Name}', '{table.Skills}','{table.TotalStudents}','{table.Salary}')";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    return RedirectToAction("Index");
                }
            }
            else
                return View();
        }

        public IActionResult Update(int id)
        {
            string ConnectionString = Configuration["ConnectionStrings:DefaultConnection"];
            Table table = new Table();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sql = $"SELECT * FROM [TABLE] WHERE ID = '{id}'";
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while(dataReader.Read())
                    {
                        table.Id = Convert.ToInt32(dataReader["Id"]);
                        table.Name = Convert.ToString(dataReader["Name"]);
                        table.Salary = Convert.ToDecimal(dataReader["Salary"]);
                        table.Skills = Convert.ToString(dataReader["Skills"]);
                        table.TotalStudents = Convert.ToInt32(dataReader["TotalStudents"]);
                        table.AddedOn = Convert.ToDateTime(dataReader["AddedOn"]);
                    }
                }
                connection.Close();
            }
            return View(table);

        }

        [HttpPost]
        [ActionName("Update")]
        public IActionResult Update_Post(Table table)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE [TABLE] SET NAME ='{table.Name}', SKILLS = '{table.Skills}', TotalStudents = '{table.TotalStudents}', Salary='{table.Salary}' Where Id='{table.Id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Delete from [table] where ID='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch(SqlException)
                    {
                        
                    }
                    connection.Close();
                }
            }
            return RedirectToAction("Index");

        }
    }
}