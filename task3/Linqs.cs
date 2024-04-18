using System;
using System.Linq;
using Npgsql;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;



namespace UniversityAdmissionApp
{
    class Linqs
    {
        static void Main(string[] args)
        {
            // string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=task3";
            
            using (var dbContext = new UniversityDbContext())
            {
                // TestDataGenerator.GenerateTestData(dbContext);
                
                // ex1(dbContext, "Program 3");
                // ex2(dbContext, "Subject 2");
                // ex3(dbContext);
                // ex4(dbContext, 50);
                // ex5(dbContext);
                // ex6(dbContext);
                // ex7(dbContext);
                // ex8(dbContext, new List<string> { "Subject 1", "Subject 2" });
                // ex9(dbContext);
                // ex10(dbContext);
            }
            
        }

        static void ex1(UniversityDbContext dbContext, string programName)
        {
            var enrolleesForProgram = from enrollee in dbContext.Enrollees
                join programEnrollee in dbContext.ProgramEnrollees on enrollee.EnrolleeId equals programEnrollee.EnrolleeId
                join program in dbContext.Programs on programEnrollee.ProgramId equals program.ProgramId
                where program.NameProgram == programName
                select enrollee;
            
            foreach (var i in enrolleesForProgram)
            {
                Console.WriteLine($"Фамилия, Имя, Отчество: {i.LastName} {i.FirstName} {i.MiddleName}");
            }
        }
        
        static void ex2(UniversityDbContext dbContext, string subjectName)
        {
            var programsForSubject = from programSubject in dbContext.ProgramSubjects
                join subject in dbContext.Subjects on programSubject.SubjectId equals subject.SubjectId
                where subject.NameSubject == subjectName
                select programSubject.Program;
            
            foreach (var i in programsForSubject)
            {
                Console.WriteLine($"Программа: {i.NameProgram}");
            }
        }
        
        static void ex3(UniversityDbContext dbContext)
        {
            var examStats = from enrolleeSubject in dbContext.EnrolleeSubjects
                join subject in dbContext.Subjects on enrolleeSubject.SubjectId equals subject.SubjectId 
                group new { enrolleeSubject, subject } by new { enrolleeSubject.SubjectId, subject.NameSubject } into g
                select new
                {
                    NameSubject = g.Key.NameSubject,
                    MinScore = g.Min(es => es.enrolleeSubject.Result),
                    MaxScore = g.Max(es => es.enrolleeSubject.Result),
                    Count = g.Count()
                };
            
            foreach (var i in examStats)
            {
                Console.WriteLine($"Предмет: {i.NameSubject}\nМинимальное количество баллов: {i.MinScore}\nМаксимальное количество баллов: {i.MaxScore}\nКоличество абитуриентов: {i.MinScore}\n");
            }
        }
        
        static void ex4(UniversityDbContext dbContext, double minValue)
        {
            var programsAboveMinValue = from programSubject in dbContext.ProgramSubjects
                group programSubject by programSubject.ProgramId into g
                where g.Min(ps => ps.MinResult) > minValue
                select g.FirstOrDefault().Program;
            
            foreach (var i in programsAboveMinValue)
            {
                Console.WriteLine($"Программа: {i.NameProgram}");
            }
        }
        
        static void ex5(UniversityDbContext dbContext)
        {
            var maxPlan = dbContext.Programs.Max(p => p.Plan);
            var programsWithMaxPlan = dbContext.Programs.Where(p => p.Plan == maxPlan);
            
            foreach (var i in programsWithMaxPlan)
            {
                Console.WriteLine($"Программа: {i.NameProgram} ({i.Plan})");
            }
        }
        
        static void ex6(UniversityDbContext dbContext)
        {
            var additionalScores = from enrolleeAchievement in dbContext.EnrolleeAchievements 
                join enrollee in dbContext.Enrollees on enrolleeAchievement.EnrolleeId equals enrollee.EnrolleeId 
                group enrolleeAchievement by new { enrolleeAchievement.EnrolleeId, enrollee.FirstName, enrollee.MiddleName } into g
                select new
                {
                    EnrolleeId = g.Key,
                    EnrolleeFirstName = g.Key.FirstName,
                    EnrolleeMiddleName = g.Key.MiddleName,
                    TotalBonus = g.Sum(ea => ea.Achievement.Bonus)
                };
            
            foreach (var i in additionalScores)
            {
                Console.WriteLine($"Абитуриент: {i.EnrolleeFirstName} {i.EnrolleeMiddleName} - {i.TotalBonus}");
            }
        }
        
        static void ex7(UniversityDbContext dbContext)
        {
            var competition = from programEnrollee in dbContext.ProgramEnrollees
                join program in dbContext.Programs on programEnrollee.ProgramId equals program.ProgramId 

                group programEnrollee by new { programEnrollee.ProgramId, program.NameProgram } into g
                select new
                {
                    ProgramId = g.Key,
                    NameProgram = g.Key.NameProgram,
                    ApplicantsCount = g.Count()
                };
            
            foreach (var i in competition)
            {
                Console.WriteLine($"Конкурс на программу: {i.NameProgram} - {i.ApplicantsCount}");
            }
        }
        
        static void ex8(UniversityDbContext dbContext, List<string> requiredSubjects)
        {
            var programsWithRequiredSubjects = from programSubject in dbContext.ProgramSubjects
                group programSubject by programSubject.ProgramId into g
                where g.Count(ps => requiredSubjects.Contains(ps.Subject.NameSubject)) == requiredSubjects.Count
                select g.FirstOrDefault().Program;
            
            foreach (var i in programsWithRequiredSubjects)
            {
                Console.WriteLine($"Программа: {i.NameProgram}");
            }
        }
        
        static void ex9(UniversityDbContext dbContext) 
        {
            var scoresPerEnrolleePerProgram = from enrolleeSubject in dbContext.EnrolleeSubjects
                join enrollee in dbContext.Enrollees on enrolleeSubject.EnrolleeId equals enrollee.EnrolleeId 
                join programEnrollee in dbContext.ProgramEnrollees on enrolleeSubject.EnrolleeId equals programEnrollee.EnrolleeId 
                join program in dbContext.Programs on programEnrollee.ProgramId equals program.ProgramId 
                group new { enrolleeSubject, enrollee, program } by new { enrolleeSubject.EnrolleeId, program.ProgramId } into g
                select new
                {
                    FirstName = g.FirstOrDefault().enrollee.FirstName,
                    MiddleName = g.FirstOrDefault().enrollee.MiddleName,
                    ProgramName = g.FirstOrDefault().program.NameProgram,
                    TotalScore = g.Sum(es => es.enrolleeSubject.Result)
                };
            
            foreach (var i in scoresPerEnrolleePerProgram)
            {

                Console.WriteLine($"Абитуриент: {i.FirstName} {i.MiddleName}\nПрограмма: {i.ProgramName}\nКоличество баллов: {i.TotalScore}\n");
            }
        }
        
        static void ex10(UniversityDbContext dbContext) 
        {
            var unadmittedEnrollees = from enrollee in dbContext.Enrollees
                join enrolleeSubject in dbContext.EnrolleeSubjects on enrollee.EnrolleeId equals enrolleeSubject.EnrolleeId
                join programEnrollee in dbContext.ProgramEnrollees on enrollee.EnrolleeId equals programEnrollee.EnrolleeId
                join programSubject in dbContext.ProgramSubjects on programEnrollee.ProgramId equals programSubject.ProgramId 
                where enrolleeSubject.Result < programSubject.MinResult
                select new { enrollee, enrolleeSubject.Result, programSubject.MinResult };
            
            foreach (var i in unadmittedEnrollees)
            {
                Console.WriteLine($"Абитуриент: {i.enrollee.FirstName} {i.enrollee.MiddleName} {i.enrollee.LastName} {i.Result} ({i.MinResult})");
            }
        }
    }
    
     public static class TestDataGenerator
    {
        private static readonly Random Random = new Random();

        public static void GenerateTestData(UniversityDbContext context)
        {
            GenerateDepartments(context);
            GenerateSubjects(context);
            GeneratePrograms(context);
            GenerateAchievements(context);
            GenerateEnrollees(context);
            GenerateEnrolleeAchievements(context);
            GenerateEnrolleeSubjects(context);
            GenerateProgramEnrollees(context);
            GenerateProgramSubjects(context);
        }

        private static void GenerateDepartments(UniversityDbContext context)
        {
            var departments = new List<Department>
            {
                new Department { NameDepartment = "Department 1" },
                new Department { NameDepartment = "Department 2" },
                new Department { NameDepartment = "Department 3" },
            };
            context.Departments.AddRange(departments);
            context.SaveChanges();
        }

        private static void GenerateSubjects(UniversityDbContext context)
        {
            var subjects = new List<Subject>
            {
                new Subject { NameSubject = "Subject 1" },
                new Subject { NameSubject = "Subject 2" },
                new Subject { NameSubject = "Subject 3" },
            };
            context.Subjects.AddRange(subjects);
            context.SaveChanges();
        }

        private static void GeneratePrograms(UniversityDbContext context)
        {
            var departments = context.Departments.ToList();
            var programs = new List<Program>
            {
                new Program { NameProgram = "Program 1", Plan = 5, DepartmentId = departments[0].DepartmentId },
                new Program { NameProgram = "Program 2", Plan = 3, DepartmentId = departments[1].DepartmentId },
                new Program { NameProgram = "Program 3", Plan = 2, DepartmentId = departments[2].DepartmentId },
            };
            context.Programs.AddRange(programs);
            context.SaveChanges();
        }

        private static void GenerateAchievements(UniversityDbContext context)
        {
            var achievements = new List<Achievement>
            {
                new Achievement { NameAchievement = "Achievement 1", Bonus = 5 },
                new Achievement { NameAchievement = "Achievement 2", Bonus = 10 },
            };
            context.Achievements.AddRange(achievements);
            context.SaveChanges();
        }

        private static void GenerateEnrollees(UniversityDbContext context)
        {
            var enrollees = new List<Enrollee>
            {
                new Enrollee { FirstName = "Дмитрий", LastName = "Коперников", MiddleName = "Викторович" },
                new Enrollee { FirstName = "Иван", LastName = "Иванов", MiddleName = "Иванович" },
                new Enrollee { FirstName = "Пётр", LastName = "Петров", MiddleName = "Петрович" },
                new Enrollee { FirstName = "Чен", LastName = "Ким", MiddleName = "Ин" },
                new Enrollee { FirstName = "Александр", LastName = "Иванов", MiddleName = "Павлович" },
            };
            context.Enrollees.AddRange(enrollees);
            context.SaveChanges();
        }

        private static void GenerateEnrolleeAchievements(UniversityDbContext context)
        {
            var enrollees = context.Enrollees.ToList();
            var achievements = context.Achievements.ToList();

            foreach (var enrollee in enrollees)
            {
                var enrolleeAchievements = new List<EnrolleeAchievement>();
                foreach (var achievement in achievements)
                {
                    // Вероятность наличия достижения у абитуриента
                    if (Random.Next(0, 2) == 1)
                    {
                        enrolleeAchievements.Add(new EnrolleeAchievement
                        {
                            EnrolleeId = enrollee.EnrolleeId,
                            AchievementId = achievement.AchievementId
                        });
                    }
                }
                context.EnrolleeAchievements.AddRange(enrolleeAchievements);
            }
            context.SaveChanges();
        }

        private static void GenerateEnrolleeSubjects(UniversityDbContext context)
        {
            var enrollees = context.Enrollees.ToList();
            var subjects = context.Subjects.ToList();

            foreach (var enrollee in enrollees)
            {
                var enrolleeSubjects = new List<EnrolleeSubject>();
                foreach (var subject in subjects)
                {
                    // Вероятность сдачи предмета абитуриентом
                    if (Random.Next(0, 2) == 1)
                    {
                        enrolleeSubjects.Add(new EnrolleeSubject
                        {
                            EnrolleeId = enrollee.EnrolleeId,
                            SubjectId = subject.SubjectId,
                            Result = Random.Next(50, 100) // Пример случайного результата от 50 до 100
                        });
                    }
                }
                context.EnrolleeSubjects.AddRange(enrolleeSubjects);
            }
            context.SaveChanges();
        }

        private static void GenerateProgramEnrollees(UniversityDbContext context)
        {
            var programs = context.Programs.ToList();
            var enrollees = context.Enrollees.ToList();

            foreach (var program in programs)
            {
                var programEnrollees = enrollees.OrderBy(e => Guid.NewGuid()).Take(Random.Next(1, 10)).ToList(); // Случайный выбор абитуриентов для программы
                foreach (var enrollee in programEnrollees)
                {
                    context.ProgramEnrollees.Add(new ProgramEnrollee { ProgramId = program.ProgramId, EnrolleeId = enrollee.EnrolleeId });
                }
            }
            context.SaveChanges();
        }

        private static void GenerateProgramSubjects(UniversityDbContext context)
        {
            var programs = context.Programs.ToList();
            var subjects = context.Subjects.ToList();

            foreach (var program in programs)
            {
                var programSubjects = new List<ProgramSubject>();
                foreach (var subject in subjects)
                {
                    // Вероятность наличия предмета в программе
                    if (Random.Next(0, 2) == 1)
                    {
                        programSubjects.Add(new ProgramSubject
                        {
                            ProgramId = program.ProgramId,
                            SubjectId = subject.SubjectId,
                            MinResult = Random.Next(50, 100) // Пример случайного минимального результата от 50 до 100
                        });
                    }
                }
                context.ProgramSubjects.AddRange(programSubjects);
            }
            context.SaveChanges();
        }
    }
     
}
