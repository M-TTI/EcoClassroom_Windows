using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoClassroom_Windows.Data;
using EcoClassroom_Windows.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoClassroom_Windows.Services
{
    public class StudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetStudentsByUserIdAsync(int userId)
        {
            return await _context.Students
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
        }

        public async Task<bool> AddStudentAsync(int userId, string firstname, string lastname)
        {
            try
            {
                var student = new Student
                {
                    FirstName = firstname,
                    LastName = lastname,
                    UserId = userId,
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteStudentAsync(int studentId, int userId)
        {
            try
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Id == studentId && s.UserId == userId);

                if (student == null)
                    return false; // Étudiant non trouvé ou n'appartient pas à l'utilisateur

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();


                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
