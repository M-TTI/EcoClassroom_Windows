using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EcoClassroom_Windows.Models
{
    public enum TransportMode
    {
        Voiture = 1,
        Covoiturage = 2,
        TransportCommun = 3,
        Velo = 4,
        APied = 5
    }

    public static class TransportModeHelper
    {
        public static string GetDescription(int mode)
        {
            return mode switch
            {
                1 => "Voiture",
                2 => "Covoiturage",
                3 => "Transport en commun",
                4 => "Vélo",
                5 => "À pied",
                _ => "Inconnu"
            };
        }

        public static Dictionary<int, string> GetAllModes()
        {
            return new Dictionary<int, string>
            {
                { 1, "Voiture" },
                { 2, "Covoiturage" },
                { 3, "Transport en commun" },
                { 4, "Vélo" },
                { 5, "À pied" }
            };
        }

        public static List<string> GetModesList()
        {
            return GetAllModes()
                .OrderByDescending(kvp => kvp.Key)
                .Select(kvp => $"{kvp.Key} - {kvp.Value}")
                .ToList();
        }
    }
}
