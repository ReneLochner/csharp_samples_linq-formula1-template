using Formula1.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Utils;

namespace Formula1.Core
{
    /// <summary>
    /// Daten sind in XML-Dateien gespeichert und werden per Linq2XML
    /// in die Collections geladen.
    /// </summary>
    public static class ImportController
    {
        private static List<Driver> drivers;
        private static List<Team> teams;
        private static List<Result> results;
        private static List<Race> races;

        /// <summary>
        /// Daten der Rennen werden aus der
        /// XML-Datei ausgelesen und in die Races-Collection gespeichert.
        /// Grund: Races werden nicht aus den Results geladen, weil sonst die
        /// Rennen in der Zukunft fehlen
        /// </summary>
        public static IEnumerable<Race> LoadRacesFromRacesXml()
        {
            races = new List<Race>();
            string racesPath = MyFile.GetFullNameInApplicationTree("Races.xml");
            XElement xElement = XDocument.Load(racesPath).Root;

            if(xElement != null)
            {
                races = xElement.Elements("Race")
                    .Select(race => new Race
                    {
                        Number = (int)race.Attribute("round"),
                        Date = (DateTime)race.Element("Date"),
                        Country = race.Element("Circuit")
                                    ?.Element("Location")
                                    ?.Element("Country")?.Value,
                        City = race.Element("Circuit")
                                    ?.Element("Location")
                                    ?.Element("Locality")?.Value
                    })
                    .ToList();
            }

            return races;
        }

        /// <summary>
        /// Aus den Results werden alle Collections, außer Races gefüllt.
        /// Races wird extra behandelt, um auch Rennen ohne Results zu verwalten
        /// </summary>
        public static IEnumerable<Result> LoadResultsFromXmlIntoCollections()
        {
            results = new List<Result>();
            string racesPath = MyFile.GetFullNameInApplicationTree("Results.xml");
            XElement xElement = XDocument.Load(racesPath).Root;

            if (xElement != null)
            {
                results = xElement.Elements("Race").Elements("ResultsList").Elements("Result")
                    .Select(results => new Result
                    {
                        Race = GetRace(results),
                        Driver = GetDriver(results),
                        Team = GetTeam(results),
                        Position = (int)results.Attribute("position"),
                        Points = (int)results.Attribute("points"),
                    })
                    .ToList();
            }

            return results;
        }

        private static Race GetRace(XElement result)
        {
            IEnumerable<Race> races = LoadRacesFromRacesXml();
            int raceNumber = (int)result.Parent?.Parent?.Attribute("round");

            return races.Single(races => races.Number == raceNumber);
        }

        private static Driver GetDriver(XElement result)
        {
            drivers = new List<Driver>();
            Driver driver = new Driver()
            {
                FirstName = (string)result?.Element("Driver")?.Element("GivenName"),
                LastName = (string)result?.Element("Driver")?.Element("FamilyName"),
                Nationality = (string)result?.Element("Driver")?.Element("Nationality")
            };

            Driver inside = drivers.SingleOrDefault(s => s.ToString().Equals(drivers.ToString()));
            if (inside == default(Driver))
            {
                drivers.Add(driver);
                inside = driver;
            }

            return inside;
        }

        private static Team GetTeam(XElement result)
        {
            teams = new List<Team>();

            string teamName = (string)result.Element("Constructor").Element("Name");
            string teamNationality = (string)result.Element("Constructor").Element("Nationality");
            Team inside = teams.SingleOrDefault(s => s.Name.Equals(teamName));

            if (inside == default(Team))
            {
                inside = new Team()
                {
                    Name = teamName,
                    Nationality = teamNationality
                };
                teams.Add(inside);

            }

            return inside;
        }
    }
}