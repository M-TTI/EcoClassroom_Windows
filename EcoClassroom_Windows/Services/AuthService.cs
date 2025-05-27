using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoClassroom_Windows.Models;
using EcoClassroom_Windows.Data;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;


namespace EcoClassroom_Windows.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var hashedPassword = HashPassword(password);
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hashedPassword);
        }

        public async Task<bool> RegisterAsync(string email, string password, string firstName, string lastName)
        {
            try
            {
                // Vérifier si l'utilisateur existe déjà
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);
                    
                if (existingUser != null)
                    return false;
                
                var user = new User
                {
                    Email = email,
                    PasswordHash = HashPassword(password),
                    FirstName = firstName,
                    LastName = lastName
                };
                
                _context.Users.Add(user);
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
