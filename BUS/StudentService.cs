using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class StudentService
    {
        public List<Student> GetAll()
        {
            StudentcontextDB context = new StudentcontextDB();
            return context.Student.ToList();
        }

        public List<Student> GetAllHasNoMajor()
        {
            StudentcontextDB context = new StudentcontextDB();
            return context.Student.Where(p => p.MajorID == null).ToList();
        }

        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            StudentcontextDB context = new StudentcontextDB();
            return context.Student.Where(p => p.MajorID == null && p.FacultyID == facultyID).ToList();
        }

        public Student FindById(string studentId)
        {
            StudentcontextDB context = new StudentcontextDB();
            return context.Student.FirstOrDefault(p => p.StudentID == studentId);
        }

        public void InsertUpdate(Student s)
        {
            StudentcontextDB context = new StudentcontextDB();
            context.Student.AddOrUpdate(s);
            context.SaveChanges();
        }
    }
}
