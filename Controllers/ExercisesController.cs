using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using StudentExercisesAPI.Models;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        //Following code allows access to SQL database

        private readonly IConfiguration _config;

        public ExercisesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: api/<ExercisesController>
        [HttpGet]
        public async Task<IActionResult> Get(string includes)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string query = "SELECT Id, Name, Language FROM Exercises ";

                    if (includes == "students")
                    {
                        query = "SELECT e.Id AS ExerciseId, e.Name AS ExerciseName, e.Language, s.Id AS StudentId, s.FirstName, s.LastName, s.SlackHandle, se.ExerciseId, se.StudentId FROM Exercises e LEFT JOIN StudentsJoinExercises se ON se.ExerciseId = e.Id JOIN Students s ON se.StudentId = s.Id";

                        cmd.CommandText = query;
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<Exercise> exercises = new List<Exercise>();

                        while (reader.Read())
                        {
                            if (!exercises.Any(c => c.Id == reader.GetInt32(reader.GetOrdinal("ExerciseId"))))
                            {
                                Exercise exercise = new Exercise
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                    Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                    Language = reader.GetString(reader.GetOrdinal("Language"))
                                };

                                if (!reader.IsDBNull(reader.GetOrdinal("StudentId")))
                                {
                                    Student student = new Student
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                        SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"))
                                    };
                                    exercise.assignedStudents.Add(student);
                                }

                                exercises.Add(exercise);
                            }
                            else
                            {
                                Exercise exerciseAlreadyInTheList = exercises.FirstOrDefault(e => e.Id == reader.GetInt32(reader.GetOrdinal("ExerciseId")));
                                bool studentExists = !reader.IsDBNull(reader.GetOrdinal("StudentId"));
                                bool studentIsNotAlreadyInTheList = !exerciseAlreadyInTheList.assignedStudents.Any(s => s.Id == reader.GetInt32(reader.GetOrdinal("StudentId")));
                                if (studentExists && studentIsNotAlreadyInTheList)
                                {
                                    Student student = new Student
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                        SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"))
                                    };
                                    exerciseAlreadyInTheList.assignedStudents.Add(student);
                                }
                            }

                        }
                    }
                    else
                    {
                        cmd.CommandText = query;
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<Exercise> exercises = new List<Exercise>();

                        while (reader.Read())
                        {
                            Exercise exercise = new Exercise
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Language = reader.GetString(reader.GetOrdinal("Language"))
                            };

                            exercises.Add(exercise);
                        }

                        reader.Close();
                        return Ok(exercises);
                    }
                    
                }
            }
        }

        // GET api/<ExercisesController>/5
        [HttpGet("{id}", Name = "GetExercise")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, Name, Language
                        FROM Exercises
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Exercise exercise = null;

                    if (reader.Read())
                    {
                        exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Language = reader.GetString(reader.GetOrdinal("Language"))
                        };
                    }
                    reader.Close();

                    return Ok(exercise);
                }
            }
        }

        // POST api/<ExercisesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Exercise exercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Exercises (Name, Language)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name, @language)";
                    cmd.Parameters.Add(new SqlParameter("@name", exercise.Name));
                    cmd.Parameters.Add(new SqlParameter("@language", exercise.Language));

                    int newId = (int)cmd.ExecuteScalar();
                    exercise.Id = newId;
                    return CreatedAtRoute("GetExercise", new { id = newId }, exercise);
                }
            }
        }

        // PUT api/<ExercisesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Exercise exercise)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Exercises
                                            SET Name = @name,
                                                Language = @language
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@name", exercise.Name));
                        cmd.Parameters.Add(new SqlParameter("@language", exercise.Language));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ExerciseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/<ExercisesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Exercises WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ExerciseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        private bool ExerciseExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, Language
                        FROM Exercises
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}

