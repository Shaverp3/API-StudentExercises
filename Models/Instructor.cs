using System;
using System.Collections.Generic;
using System.Text;

namespace StudentExercisesAPI.Models
{
    public class Instructor : Person
    {
        public int Id { get; set; }
        public string Specialty { get; set; }


        //public List<Exercise> AssignedExercises { get; set; } = new List<Exercise>();


        //Method that accepts a student and an exercise to the student's list of assigned exercises
        public void AssignExercise(Student assignedStudent, Exercise exerciseToAssign)
        {
            assignedStudent.AssignedExercises.Add(exerciseToAssign);
            exerciseToAssign.assignedStudents.Add(assignedStudent);
        }
    }
}
