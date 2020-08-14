using System;
using System.Collections.Generic;
using System.Text;

namespace StudentExercisesAPI.Models
{
    public class Student : Person
    {
        public int Id { get; set; }
        public int Grade { get; set; }

        //Create List here when Student will be assigned many Exercises
        public List<Exercise> AssignedExercises { get; set; } = new List<Exercise>();

    }
}
