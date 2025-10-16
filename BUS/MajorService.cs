using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class MajorService
    {
        public List<Major> GetAllBYFaculty(int facultyID)
        {
            StudentcontextDB context = new StudentcontextDB();
            return context.Major.ToList();
        }
    }
}
