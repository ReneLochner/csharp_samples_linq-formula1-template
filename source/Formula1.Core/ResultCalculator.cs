using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula1.Core.Entities;

namespace Formula1.Core
{
    public class ResultCalculator
    {
        /// <summary>
        /// Berechnet aus den Ergebnissen den aktuellen WM-Stand für die Fahrer
        /// </summary>
        /// <returns>DTO für die Fahrerergebnisse</returns>
        public static IEnumerable<TotalResultDto<Driver>> GetDriverWmTable()
        {
            IEnumerable<Result> drivers = ImportController.LoadResultsFromXmlIntoCollections();
            return drivers
                .GroupBy(s => s.Driver)
                .Select(t => new TotalResultDto<Driver>()
                {
                    Competitor = t.Key,
                    Points = t.Sum(p => p.Points)
                })
                .OrderByDescending(d => d.Points)
                .ThenBy(l => l.Competitor.LastName);
        }

        /// <summary>
        /// Berechnet aus den Ergebnissen den aktuellen WM-Stand für die Teams
        /// </summary>
        /// <returns>DTO für die Teamergebnisse</returns>
        public static IEnumerable<TotalResultDto<Team>> GetTeamWmTable()
        {
            IEnumerable<Result> team = ImportController.LoadResultsFromXmlIntoCollections();

            return team
                .GroupBy(t => t.Team)
                .Select(s => new TotalResultDto<Team>()
                {
                    Competitor = s.Key,
                    Points = s.Sum(p => p.Points)
                })
                .OrderByDescending(o => o.Points)
                .ThenBy(c => c.Competitor);
        }
    }
}



