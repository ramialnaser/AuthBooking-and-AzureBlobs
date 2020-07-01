using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebRole1.Models;

namespace WebRole1
{
    public partial class _Default : Page
    {

        private DbHelper dbHelper;

        protected void Page_Load(object sender, EventArgs e)
        {

            dbHelper = new DbHelper();
            string role = getUserRole();

            RedirectThePage();

            // this will disable all the buttons to any user that is not a customer
            if (role != "Customer")
            {
                Label1.Text = "You have to be logged as a customer to use this page";
                BookFlightButton.Enabled = false;
                BookCarRentalButton.Enabled = false;
                CreatePayment.Enabled = false;


            }

        }

        // this method will check the lockedoutenable status of the logged user
        // and if it is true the user is redirected to the lockout page.          
        protected async void RedirectThePage()
        {
            if (User.Identity.GetUserId() != null)
            {
                bool isEnabled = await dbHelper.GetlockoutStatus(User.Identity.GetUserId());

                if (!isEnabled)
                {
                    Response.Redirect("/Account/Lockout");
                }
            }

        }

        // to get the role of the logged user
        protected string getUserRole()
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            if (User.Identity.GetUserId() == null) return null;
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var roles = roleManager.Roles.ToList();
            foreach (IdentityRole role in roles)
            {
                string roleName = role.Name;
                if (User.IsInRole(roleName))
                {
                    return role.Name;
                }
            }
            return null;
        }

        // to make a flight booking
        protected async void BookFlightButton_Click(object sender, EventArgs e)
        {
            // to make sure the fields are not empty
            if (!string.IsNullOrWhiteSpace(PassportTextBox.Text) &&
               !string.IsNullOrWhiteSpace(PassengerNameTextBox.Text) && !string.IsNullOrWhiteSpace(DepartureDateTextBox.Text))
            {
                //this will create an a flight booking object
                Flight flight = new Flight()
                {
                    PassportNumber = PassportTextBox.Text,
                    PassengerName = PassengerNameTextBox.Text,
                    FlightNumber = ddlRoutes.SelectedItem.Text,
                    DepartureDate = DepartureDateTextBox.Text,
                    PassengerType = ddlPassengerType.SelectedItem.Text
                };
                // to get the needed values that needed for the flight price calculation.
                List<Route> routes = await dbHelper.GetRoutes();
                List<Airport> airports = await dbHelper.GetAirports();

                Route route = routes.Find(a => a.FlightNumber == flight.FlightNumber);
                Airport fromAirport = airports.Find(a => a.Code == route.Departure);
                Airport toAirport = airports.Find(a => a.Code == route.Arrival);

                flight.Price = dbHelper.FlightPriceCalc(fromAirport.Latitude, fromAirport.Longitude, toAirport.Latitude, toAirport.Longitude, flight.PassengerType);


                FlightPriceTextBox.Text = flight.Price.ToString();
                TotalPrice();

                FlightResults.Text = "Added to cart!";

                PassportTextBox.Enabled = false;
                PassengerNameTextBox.Enabled = false;
                ddlRoutes.Enabled = false;
                DepartureDateTextBox.Enabled = false;
                ddlPassengerType.Enabled = false;


                BookFlightButton.Enabled = false;
                CreatePayment.Enabled = true;

            }
            else
            {
                FlightResults.Text = "Make sure to fill the fields!";

            }

        }

        //to make a car rental
        protected void BookCarRentalButton_Click(object sender, EventArgs e)
        {
            // to make sure the fields are not empty
            if (!string.IsNullOrWhiteSpace(CarModelTextBox.Text) &&
               !string.IsNullOrWhiteSpace(DriverAgeTextBox.Text))
            {
                //this will create an a flight booking object
                CarBooking carBooking = new CarBooking()
                {
                    CarRentalCode = ddlCarRentals.SelectedItem.Text,
                    CarModel = int.Parse(CarModelTextBox.Text),
                    NumberOfSeats = int.Parse(ddlNumberOfSeats.SelectedItem.Text),
                    FuelType = ddlFuelType.SelectedItem.Text,
                };

                carBooking.Price = (int)dbHelper.CarRentalCalculation(carBooking);

                CarBookingPrice.Text = carBooking.Price.ToString();

                TotalPrice();

                CarBookingResults.Text = "Added to cart!";

                ddlCarRentals.Enabled = false;
                CarModelTextBox.Enabled = false;
                ddlNumberOfSeats.Enabled = false;
                DriverAgeTextBox.Enabled = false;
                ddlFuelType.Enabled = false;



                BookCarRentalButton.Enabled = false;
                CreatePayment.Enabled = true;
            }
            else
            {
                CarBookingResults.Text = "Make sure to fill the fields!";

            }
        }

        // to make payment and add it to the database
        protected async void CreatePayment_Click(object sender, EventArgs e)
        {
            if (!PassportTextBox.Enabled)
            {
                //this will create an a flight booking object
                Flight flight = new Flight()
                {
                    PassportNumber = PassportTextBox.Text,
                    PassengerName = PassengerNameTextBox.Text,
                    FlightNumber = ddlRoutes.SelectedItem.Text,
                    DepartureDate = DepartureDateTextBox.Text,
                    PassengerType = ddlPassengerType.SelectedItem.Text
                };
                // to get the needed values that needed for the flight price calculation.
                List<Route> routes = await dbHelper.GetRoutes();
                List<Airport> airports = await dbHelper.GetAirports();

                Route route = routes.Find(a => a.FlightNumber == flight.FlightNumber);
                Airport fromAirport = airports.Find(a => a.Code == route.Departure);
                Airport toAirport = airports.Find(a => a.Code == route.Arrival);

                flight.Price = dbHelper.FlightPriceCalc(fromAirport.Latitude, fromAirport.Longitude, toAirport.Latitude, toAirport.Longitude, flight.PassengerType);


                await dbHelper.AddFlight(flight);

                PassportTextBox.Enabled = true;
                PassengerNameTextBox.Enabled = true;
                ddlRoutes.Enabled = true;
                DepartureDateTextBox.Enabled = true;
                ddlPassengerType.Enabled = true;


                PassportTextBox.Text = string.Empty;
                PassengerNameTextBox.Text = string.Empty;
                DepartureDateTextBox.Text = string.Empty;

                PaymentResults.Text += " Added flight.    ";
            }

            if (!ddlCarRentals.Enabled)
            {
                //this will create an a flight booking object
                CarBooking carBooking = new CarBooking()
                {
                    CarRentalCode = ddlCarRentals.SelectedItem.Text,
                    CarModel = int.Parse(CarModelTextBox.Text),
                    NumberOfSeats = int.Parse(ddlNumberOfSeats.SelectedItem.Text),
                    FuelType = ddlFuelType.SelectedItem.Text,
                };

                carBooking.Price = (int)dbHelper.CarRentalCalculation(carBooking);

                await dbHelper.AddCarBooking(carBooking);

                ddlCarRentals.Enabled = true;
                CarModelTextBox.Enabled = true;
                ddlNumberOfSeats.Enabled = true;
                DriverAgeTextBox.Enabled = true;
                ddlFuelType.Enabled = true;

                CarModelTextBox.Text = string.Empty;
                DriverAgeTextBox.Text = string.Empty;

                PaymentResults.Text += " Added car booking.    ";

            }
            FlightPriceTextBox.Text = string.Empty;
            CarBookingPrice.Text = string.Empty;
            TotalPriceTextBox.Text = string.Empty;

            BookCarRentalButton.Enabled = true;
            BookFlightButton.Enabled = true;

        }

        // to calculate the totalPrice and display it for the user for to accept.
        private void TotalPrice()
        {
            int flightPrice = 0;
            int carBookingPrice = 0;
            if (!string.IsNullOrEmpty(FlightPriceTextBox.Text))
            {
                flightPrice = int.Parse(FlightPriceTextBox.Text);
            }

            if (!string.IsNullOrEmpty(CarBookingPrice.Text))
            {
                carBookingPrice = int.Parse(CarBookingPrice.Text);
            }
            

            int totalPrice = flightPrice + carBookingPrice;


            TotalPriceTextBox.Text = totalPrice.ToString();

        }
    }
}