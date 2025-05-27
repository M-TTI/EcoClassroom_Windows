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
    public class AttendanceService
    {
        private readonly AppDbContext _context;
        public AttendanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Attendance>> GetAttendancesByDateAsync(int userId, DateTime date)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.Student.UserId == userId && a.Date.Date == date.Date)
                .ToListAsync();
        }

        public async Task<bool> MarkStudentPresentAsync(int studentId, DateTime date, int transportMode)
        {
            try
            {
                var existingAttendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.StudentId == studentId && a.Date.Date == date.Date);

                if (existingAttendance != null)
                {
                    existingAttendance.IsPresent = true;
                    existingAttendance.TransportMode = transportMode;
                }
                else
                {
                    var attendance = new Attendance
                    {
                        StudentId = studentId,
                        Date = date,
                        IsPresent = true,
                        TransportMode = transportMode
                    };
                    _context.Attendances.Add(attendance);
                }

                await _context.SaveChangesAsync();


                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkStudentAbsentAsync(int stduentId, DateTime date)
        {
            try
            {
                var existingAttendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.StudentId == stduentId && a.Date.Date == date.Date);

                if (existingAttendance != null)
                {
                    existingAttendance.IsPresent = false;
                    existingAttendance.TransportMode = null; // Aucun transport
                }
                else
                {
                    var attendance = new Attendance
                    {
                        StudentId = stduentId,
                        Date = date,
                        IsPresent = false,
                        TransportMode = null // Aucun transport
                    };
                    _context.Attendances.Add(attendance);
                }

                await _context.SaveChangesAsync();


                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<double> GetWeeklyAverageAsync(int userId, DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(6);

            var attendances = await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.Student.UserId == userId &&
                            a.Date >= weekStart &&
                            a.Date <= weekEnd &&
                            a.IsPresent &&
                            a.TransportMode.HasValue)
                .ToListAsync();

            if (!attendances.Any())
                return 0;

            var totalScore = attendances.Sum(a => a.TransportMode.Value);


            return (double)totalScore / attendances.Count;
        }

        public async Task<WeeklyStats> GetWeeklyStatsAsync(int userId, DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(6);
            
            var students = await _context.Students
                .Where(s => s.UserId == userId)
                .ToListAsync();

            var attendances = await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.Student.UserId == userId &&
                            a.Date >= weekStart &&
                            a.Date <= weekEnd)
                .ToListAsync();

            var weeklyStats = new WeeklyStats
            {
                TotalStudents = students.Count,
                TotalPresences = attendances.Count(a => a.IsPresent),
                TotalAbsences = attendances.Count(a => !a.IsPresent),
                AverageTransportScore = attendances.Where(a => a.IsPresent && a.TransportMode.HasValue)
                    .Select(a => a.TransportMode.Value)
                    .DefaultIfEmpty(0)
                    .Average(),
                TransportModeBreakdown = attendances
                    .Where(a => a.IsPresent && a.TransportMode.HasValue)
                    .GroupBy(a => a.TransportMode.Value)
                    .ToDictionary(g => g.Key, g => g.Count())
            };


            return weeklyStats;
        }
    }

    public class WeeklyStats
    {
        public int TotalStudents { get; set; }
        public int TotalPresences { get; set; }
        public int TotalAbsences { get; set; }
        public double AverageTransportScore { get; set; }
        public Dictionary<int, int> TransportModeBreakdown { get; set; } = new();
    }
}
