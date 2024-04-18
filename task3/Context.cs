using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace UniversityAdmissionApp
{
    public class UniversityDbContext : DbContext
    {
        public DbSet<Enrollee> Enrollees { get; set; }
        public DbSet<EnrolleeAchievement> EnrolleeAchievements { get; set; }
        public DbSet<EnrolleeSubject> EnrolleeSubjects { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<ProgramEnrollee> ProgramEnrollees { get; set; }
        public DbSet<ProgramSubject> ProgramSubjects { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=postgres;Database=task3");
        }
    }

    public class Enrollee
    {
        public int EnrolleeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        
        public ICollection<EnrolleeAchievement> Achievements { get; set; }
        public ICollection<ProgramEnrollee> Programs { get; set; }
        public ICollection<EnrolleeSubject> Subjects { get; set; }
    }

    public class ProgramEnrollee
    {
        public int ProgramEnrolleeId { get; set; }
        public int EnrolleeId { get; set; }
        public int ProgramId { get; set; }
    }

    public class Program
    {
        public int ProgramId { get; set; }
        public string NameProgram { get; set; }
        public int DepartmentId { get; set; }
        public int Plan { get; set; }

        public Department Department { get; set; }
        public ICollection<ProgramSubject> ProgramSubjects { get; set; }
        public ICollection<ProgramEnrollee> ProgramEnrollees { get; set; }
    }

    public class Department
    {
        public int DepartmentId { get; set; }
        public string NameDepartment { get; set; }

        public ICollection<Program> Programs { get; set; }
    }

    public class ProgramSubject
    {
        public int ProgramSubjectId { get; set; }
        public int ProgramId { get; set; }
        public int SubjectId { get; set; }
        public int MinResult { get; set; }

        public Program Program { get; set; }
        public Subject Subject { get; set; }
    }

    public class Subject
    {
        public int SubjectId { get; set; }
        public string NameSubject { get; set; }

        public ICollection<ProgramSubject> ProgramSubjects { get; set; }
    }

    public class EnrolleeSubject
    {
        public int EnrolleeSubjectId { get; set; }
        public int EnrolleeId { get; set; }
        public int SubjectId { get; set; }
        public int Result { get; set; }

        public Enrollee Enrollee { get; set; }
        public Subject Subject { get; set; }
    }
    
    public class EnrolleeAchievement
    {
        public int EnrolleeAchievementId { get; set; }
        public int EnrolleeId { get; set; }
        public int AchievementId { get; set; }

        public Enrollee Enrollee { get; set; }
        public Achievement Achievement { get; set; }
    }

    public class Achievement
    {
        public int AchievementId { get; set; }
        public string NameAchievement { get; set; }
        public int Bonus { get; set; }

        public ICollection<EnrolleeAchievement> EnrolleeAchievements { get; set; }
    }
}