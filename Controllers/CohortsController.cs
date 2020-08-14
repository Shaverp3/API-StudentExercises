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
using Microsoft.AspNetCore.Mvc.Formatters;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CohortsController : ControllerBase
    {
        //Following code allows access to SQL database

        private readonly IConfiguration _config;

        public CohortsController(IConfiguration config)
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

        // GET: api/<CohortsController>
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT c.Id, c.Name, s.id AS StudentID, s.FirstName AS StudentFirstName, s.LastName AS StudentLastName, s.SlackHandle AS StudentSlackHandle, s.CohortId AS StudentCohortID, i.id AS InstructorID, i.FirstName AS InstructorFirstName, i.LastName AS InstructorLastName,i.SlackHandle AS InstructorSlackHandle, i.CohortId AS InstructorCohortId FROM Cohorts c LEFT JOIN Students s ON  c.Id = s.CohortId LEFT JOIN Instructors i ON c.Id = i.CohortId";
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            List<Cohort> cohortList = new List<Cohort>();

        //            while (reader.Read())
        //            {
        //                Cohort currentCohortInLoop = new Cohort
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    Name = reader.GetString(reader.GetOrdinal("Name"))
        //                };

        //                Student currentStudentInLoop = null;
        //                if (!reader.IsDBNull(reader.GetOrdinal("StudentID")))
        //                {
        //                    currentStudentInLoop = new Student
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("StudentID")),
        //                        FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
        //                        LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
        //                        SlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
        //                        CohortId = reader.GetInt32(reader.GetOrdinal("StudentCohortId"))
        //                    };
        //                }

        //                Instructor currentInstructorInLoop = null;
        //                if (!reader.IsDBNull(reader.GetOrdinal("InstructorID")))
        //                {
        //                    currentInstructorInLoop = new Instructor
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("InstructorID")),
        //                        FirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
        //                        LastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
        //                        SlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
        //                        CohortId = reader.GetInt32(reader.GetOrdinal("InstructorCohortId"))

        //                    };
        //                }
        //                // Check if cohort is already in the list
        //                // If it's NOT in the list, we can add it
        //                // If it is in the list, do something else

        //                if (!cohortList.Any(c => c.Id == currentCohortInLoop.Id))
        //                {
        //                    currentCohortInLoop.Students.Add(currentStudentInLoop);
        //                    currentCohortInLoop.Instructors.Add(currentInstructorInLoop);

        //                    cohortList.Add(currentCohortInLoop);
        //                }
        //                else
        //                {
        //                    //don't add the cohort to the list
        //                    Cohort cohortAlreadyInList = cohortList.Find(c => c.Id == currentCohortInLoop.Id);

        //                    bool instructorExists = currentInstructorInLoop != null;
        //                    bool instructorIsNotInList = !currentCohortInLoop.Instructors.Any(i => i.Id == currentInstructorInLoop.Id);

        //                    //Check and see if the instructor is already in the cohort's instructors list
        //                    //If the instructor is NOT in the list, add them
        //                    if (instructorExists && instructorIsNotInList)
        //                    {
        //                        cohortAlreadyInList.Instructors.Add(currentInstructorInLoop);
        //                    }

        //                    //check and see if the student is already in the cohort's Students List
        //                    //if NOT, add them
        //                    if (!reader.IsDBNull(reader.GetOrdinal("StudentID")) && !cohortList.FirstOrDefault(c => c.Id == reader.GetInt32(reader.GetOrdinal("StudentCohortId"))).Students.Any(s => s.Id == reader.GetInt32(reader.GetOrdinal("StudentID"))))
        //                    {
        //                        cohortList.FirstOrDefault(c => c.Id == currentStudentInLoop.CohortId).Students.Add(currentStudentInLoop);

        //                    }
        //                };
        //            }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.Name, i.Id AS 'Instructor Id', i.FirstName AS 'Instructor FirstName', i.LastName AS 'Instructor LastName', i.SlackHandle AS 'Instructor SlackHandle', i.Specialty, i.CohortId AS 'Instructor CohortId', s.Id AS 'Student Id', s.FirstName, s.LastName, s.SlackHandle, i.CohortId FROM Cohorts c
                      LEFT JOIN Instructors i on c.Id = i.CohortId
                      LEFT JOIN Students s on c.Id = s.CohortId";

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {

                        if (!cohorts.Any(c => c.Id == reader.GetInt32(reader.GetOrdinal("Id"))))
                        {
                            Cohort cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("Student Id")))
                            {
                                Student student = new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Student Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))

                                };
                                cohort.Students.Add(student);
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("Instructor Id")))
                            {
                                Instructor instructor = new Instructor
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Instructor Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("Instructor FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("Instructor LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("Instructor SlackHandle")),
                                    Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("Instructor CohortId")),

                                };

                                cohort.Instructors.Add(instructor);
                            }

                            cohorts.Add(cohort);
                        }
                        else
                        {
                            Cohort cohortAlreadyInTheList = cohorts.FirstOrDefault(c => c.Id == reader.GetInt32(reader.GetOrdinal("CohortId")));
                            bool studentExists = !reader.IsDBNull(reader.GetOrdinal("Student Id"));
                            bool studentIsNotAlreadyInTheList = !cohortAlreadyInTheList.Students.Any(s => s.Id == reader.GetInt32(reader.GetOrdinal("Student Id")));
                            if (studentExists && studentIsNotAlreadyInTheList)
                            {
                                Student student = new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Student Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))

                                };
                                cohortAlreadyInTheList.Students.Add(student);
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("Instructor Id")) && !cohorts.FirstOrDefault(c => c.Id == reader.GetInt32(reader.GetOrdinal("Instructor CohortId"))).Instructors.Any(s => s.Id == reader.GetInt32(reader.GetOrdinal("Instructor Id"))))
                            {
                                Instructor instructor = new Instructor
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Instructor Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("Instructor FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("Instructor LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("Instructor SlackHandle")),
                                    Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("Instructor CohortId")),

                                };
                                cohorts.FirstOrDefault(c => c.Id == instructor.CohortId).Instructors.Add(instructor);
                            }

                        }
                    }

                    reader.Close();
                    return Ok(cohorts);

                }
            }
        }

        // GET api/<CohortsController>/5
        [HttpGet("{id}", Name = "GetCohort")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, Name
                        FROM Cohorts
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort cohort = null;

                    if (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }
                    reader.Close();

                    return Ok(cohort);
                }
            }
        }

        // POST api/<CohortsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cohort cohort)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Cohorts (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";
                    cmd.Parameters.Add(new SqlParameter("@name", cohort.Name));

                    int newId = (int)cmd.ExecuteScalar();
                    cohort.Id = newId;
                    return CreatedAtRoute("GetCohort", new { id = newId }, cohort);
                }
            }
        }

        // PUT api/<CohortsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Cohort cohort)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Cohorts
                                            SET Name = @name
                                                WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@name", cohort.Name));
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
                if (!CohortExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/<CohortsController>/5
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
                        cmd.CommandText = @"DELETE FROM Cohorts WHERE Id = @id";
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
                if (!CohortExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        private bool CohortExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name
                        FROM Cohorts
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
