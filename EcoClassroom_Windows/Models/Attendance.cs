using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoClassroom_Windows.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        public DateTime Date { get; set; }

        // Indique si l'élève était présent
        public bool IsPresent { get; set; }

        // 1 = Voiture, 2 = Covoiturage, 3 = Transport commun, 4 = Vélo, 5 = À pied
        [Range(1, 5)]
        public int? TransportMode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
