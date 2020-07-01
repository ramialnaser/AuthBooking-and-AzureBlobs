using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1.Models
{
    public class Flight
    {

        // a model class to represent the flight object

        public Flight() { }

        public string PassportNumber { get; set; }
        public string PassengerName { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureDate { get; set; }

        public string PassengerType { get; set; }
        public int Price { get; set; }
    }
}