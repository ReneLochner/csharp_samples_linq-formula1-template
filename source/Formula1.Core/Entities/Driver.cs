using Formula1.Core.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Formula1.Core.Entities
{
    public class Driver : ICompetitor
    {
        // public int Number { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //public DateTime Birthday { get; set; }
        public string Nationality { get; set; }

        public string Name => ToString();

        public override string ToString()
        {
            return $"{LastName} {FirstName}";
        }
    }
}
