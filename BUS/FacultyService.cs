﻿using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class FacultyService
    {
        public List<Faculty> GetAll()
        {
            StudentcontextDB context = new StudentcontextDB();
            return context.Faculty.ToList();
        }
    }
}
