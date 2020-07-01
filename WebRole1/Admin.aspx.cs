using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebRole1.Models;

namespace WebRole1
{
    public partial class Admin : Page
    {

        private DbHelper dbHelper;

        protected void Page_Load(object sender, EventArgs e)
        {
            dbHelper = new DbHelper();
            string role = getUserRole();

            RedirectThePage();

            // this will disable all the buttons to any user that is not an admin
            if (role != "Administrator")
            {
                Label1.Text = "You have to be logged as admin to use this page";
                GetAirlineButton.Enabled = false;
                GetAirportButton.Enabled = false;
                GetCarRentalButton.Enabled = false;
                GetRouteButton.Enabled = false;

                AddAirlineButton.Enabled = false;
                AddAirportButton.Enabled = false;
                AddCarRentalButton.Enabled = false;
                AddRouteButton.Enabled = false;

                ActivateButton.Enabled = false;
                DeactivateButton.Enabled = false;

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

        // to deactivate a user
        protected async void DeactivateButton_Click(object sender, EventArgs e)
        {
            if (User.Identity.GetUserId() == ddlActiveList.SelectedItem.Value)
            {
                ActivateResults.Text = "you cannot deactivate yourself";
            }
            else
            {
                await dbHelper.UpdateLockoutStatus(ddlActiveList.SelectedItem.Value, false);
                ActivateResults.Text = ddlActiveList.SelectedItem.Text + " is Deactivated";

                // to update the dropdownlists
                ddlActiveList.DataBind();
                sdcDeactivate.DataBind();
                ddlDeactiveList.DataBind();
                sdcDeactivate.DataBind();
            }

        }

        //to activate a user
        protected async void ActivateButton_Click(object sender, EventArgs e)
        {
            await dbHelper.UpdateLockoutStatus(ddlDeactiveList.SelectedItem.Value, true);
            ActivateResults.Text = ddlDeactiveList.SelectedItem.Text + " is activated";

            // to update the dropdownlists
            ddlDeactiveList.DataBind();
            sdcDeactivate.DataBind();
            ddlActiveList.DataBind();
            sdcDeactivate.DataBind();
        }


        // to get a route from the database and populate the fields with the fetched data
        protected async void GetRoute_Click(object sender, EventArgs e)
        {
            Route route = await dbHelper.GetRoute(ddlRoutes.SelectedItem.Text);

            // to check that it's not an empty object
            if (route != null)
            {

                RouteFlightNrTextBox.Text = route.FlightNumber;
                ddlRouteCarrier.Text = route.Carrier;
                ddlRouteDepCode.Text = route.Departure;
                ddlRouteArrCode.Text = route.Arrival;

                RouteFlightNrTextBox.Enabled = false;
                EditRouteButton.Enabled = true;

                AddRouteResults.Text = "Route is fetched";
            }
            else
            {
                AddRouteResults.Text = "CANNOT BE FETCHED";

            }

        }

        // to get an airport from the database and populate the fields with the fetched data
        protected async void GetAirport_Click(object sender, EventArgs e)
        {
            Airport airport = await dbHelper.GetAirport(ddlAirports.SelectedItem.Text);

            // to check that it's not an empty object
            if (airport != null)
            {

                AirportCodeTextBox.Text = airport.Code;
                AirportCityTextBox.Text = airport.City;
                AirportLatitudeTextBox.Text = airport.Latitude.ToString();
                AirportLongitudeTextBox.Text = airport.Longitude.ToString();

                AirportCodeTextBox.Enabled = false;
                EditAirportButton.Enabled = true;

                AddAirportResult.Text = "Airport is fetched";
            }
            else
            {
                AddAirportResult.Text = "CANNOT BE FETCHED";

            }
        }

        // to get an airline from the database and populate the fields with the fetched data
        protected async void GetAirline_Click(object sender, EventArgs e)
        {
            Airline airline = await dbHelper.GetAirline(ddlAirlines.SelectedItem.Text);

            // to check that it's not an empty object
            if (airline != null)
            {
                AirlineCodeTextBox.Text = airline.Code;
                AirlineNameTextBox.Text = airline.Name;

                AirlineCodeTextBox.Enabled = false;
                EditAirlineButton.Enabled = true;

                AddAirlineResult.Text = "Airline is fetched";
            }
            else
            {
                AddAirlineResult.Text = "CANNOT BE FETCHED";

            }
        }

        // to get a car rental from the database and populate the fields with the fetched data
        // PK cannot be changed.
        protected async void GetCarRental_Click(object sender, EventArgs e)
        {
            CarRental carRental = await dbHelper.GetCarRental(ddlCarRentals.SelectedItem.Text);

            // to check that it's not an empty object
            if (carRental != null)
            {
                CarRentalCodeTextBox.Text = carRental.Code;
                CarRentalNameTextBox.Text = carRental.Name;

                CarRentalCodeTextBox.Enabled = false;
                EditCarRentalButton.Enabled = true;

                AddCRResults.Text = "CarRental is fetched";
            }
            else
            {
                AddCRResults.Text = "CANNOT BE FETCHED";

            }

        }

        // to edit the values of the route. 
        // PK cannot be changed.
        //FKs can be changed to different ones from the exiting FKs but you cannot edit it.
        protected async void EditRoute_Click(object sender, EventArgs e)
        {
            // to make sure the fields are not empty and that the arrival and departure are not the same
            if (!string.IsNullOrWhiteSpace(ddlRouteCarrier.SelectedItem.Text) &&
                !string.IsNullOrWhiteSpace(ddlRouteArrCode.SelectedItem.Text) &&
                !string.IsNullOrWhiteSpace(ddlRouteDepCode.SelectedItem.Text) &&
                (ddlRouteArrCode.SelectedItem.Text != ddlRouteDepCode.SelectedItem.Text))
            {
                Route route = new Route()
                {
                    FlightNumber = RouteFlightNrTextBox.Text,
                    Carrier = ddlRouteCarrier.SelectedItem.Text,
                    Departure = ddlRouteDepCode.SelectedItem.Text,
                    Arrival = ddlRouteArrCode.SelectedItem.Text

                };

                await dbHelper.UpdateRoute(route);

                AddRouteResults.Text = "Updated";

                RouteFlightNrTextBox.Text = string.Empty;
                ddlRouteCarrier.SelectedItem.Text = string.Empty;

                RouteFlightNrTextBox.Enabled = true;
                EditRouteButton.Enabled = false;

                //to update the dropdownlists
                ddlRouteDepCode.DataBind();
                ddlRouteArrCode.DataBind();
                RoutesdcAirports.DataBind();
                ddlRouteCarrier.DataBind();
                RoutesdcAirlines.DataBind();

            }
            else
            {
                AddRouteResults.Text = "Make sure to fill the fields!";

            }

        }

        // to edit the values of the airport. 
        // PK cannot be changed.
        protected async void EditAirport_Click(object sender, EventArgs e)
        {
            // to make sure the fields are not empty
            if (!string.IsNullOrWhiteSpace(AirportCityTextBox.Text) &&
               !string.IsNullOrWhiteSpace(AirportLatitudeTextBox.Text) && !string.IsNullOrWhiteSpace(AirportLongitudeTextBox.Text))
            {
                //this will create an airport object
                Airport airport = new Airport()
                {
                    Code = AirportCodeTextBox.Text,
                    City = AirportCityTextBox.Text,
                    Latitude = double.Parse(AirportLatitudeTextBox.Text),
                    Longitude = double.Parse(AirportLongitudeTextBox.Text)
                };

                await dbHelper.UpdateAirport(airport);

                AddAirportResult.Text = "Updated";
                AirportCodeTextBox.Text = string.Empty;
                AirportCityTextBox.Text = string.Empty;
                AirportLatitudeTextBox.Text = string.Empty;
                AirportLongitudeTextBox.Text = string.Empty;

                AirportCodeTextBox.Enabled = true;
                EditAirportButton.Enabled = false;
            }
            else
            {
                AddAirportResult.Text = "Make sure to fill the fields!";

            }
        }

        // to edit the values of the airline. 
        // PK cannot be changed.
        protected async void EditAirline_Click(object sender, EventArgs e)
        {
            // to make sure the field is not empty
            if (!string.IsNullOrWhiteSpace(AirlineNameTextBox.Text))
            {
                Airline airline = new Airline()
                {
                    Code = AirlineCodeTextBox.Text,
                    Name = AirlineNameTextBox.Text

                };
                await dbHelper.UpdateAirline(airline);

                AddAirlineResult.Text = "Updated";
                AirlineCodeTextBox.Text = string.Empty;
                AirlineNameTextBox.Text = string.Empty;

                AirlineCodeTextBox.Enabled = true;
                EditAirlineButton.Enabled = false;
            }
            else
            {
                AddAirlineResult.Text = "Make sure to fill the fields!";
            }
        }

        // to edit the car rentals
        protected async void EditCarRental_Click(object sender, EventArgs e)
        {
            // to make sure the field is not empty
            if (!string.IsNullOrWhiteSpace(CarRentalNameTextBox.Text))
            {
                CarRental carRental = new CarRental()
                {
                    Code = CarRentalCodeTextBox.Text,
                    Name = CarRentalNameTextBox.Text
                };

                await dbHelper.UpdateCarRental(carRental);

                AddCRResults.Text = "Updated";
                CarRentalCodeTextBox.Text = string.Empty;
                CarRentalNameTextBox.Text = string.Empty;

                CarRentalCodeTextBox.Enabled = true;
                EditCarRentalButton.Enabled = false;

            }
            else
            {
                AddCRResults.Text = "Make sure to fill the fields!";
            }

        }

        protected async void AddRoute_Click(object sender, EventArgs e)
        {
            // to make sure the fields are not empty
            if (!string.IsNullOrWhiteSpace(RouteFlightNrTextBox.Text) && !string.IsNullOrWhiteSpace(ddlRouteCarrier.SelectedItem.Text) &&
                !string.IsNullOrWhiteSpace(ddlRouteArrCode.SelectedItem.Text) && !string.IsNullOrWhiteSpace(ddlRouteDepCode.SelectedItem.Text))
            {
                Route route = new Route()
                {
                    FlightNumber = RouteFlightNrTextBox.Text,
                    Carrier = ddlRouteCarrier.SelectedItem.Text,
                    Departure = ddlRouteDepCode.SelectedItem.Text,
                    Arrival = ddlRouteArrCode.SelectedItem.Text

                };

                await dbHelper.AddRoute(route);

                AddRouteResults.Text = "Added";

                RouteFlightNrTextBox.Text = string.Empty;
                ddlRouteCarrier.SelectedItem.Text = string.Empty;



                //to update the dropdownlists
                sdcRoutes.DataBind();
                ddlRoutes.DataBind();
                ddlRouteDepCode.DataBind();
                ddlRouteArrCode.DataBind();
                RoutesdcAirports.DataBind();
                ddlRouteCarrier.DataBind();
                RoutesdcAirlines.DataBind();
            }
            else
            {
                AddRouteResults.Text = "Make sure to fill the fields!";

            }
        }

        protected async void AddAirport_Click(object sender, EventArgs e)
        {
            // to make sure the fields are not empty
            if (!string.IsNullOrWhiteSpace(AirportCodeTextBox.Text) && !string.IsNullOrWhiteSpace(AirportCityTextBox.Text) &&
               !string.IsNullOrWhiteSpace(AirportLatitudeTextBox.Text) && !string.IsNullOrWhiteSpace(AirportLongitudeTextBox.Text))
            {
                //this will create an airport object
                Airport airport = new Airport()
                {
                    Code = AirportCodeTextBox.Text,
                    City = AirportCityTextBox.Text,
                    Latitude = double.Parse(AirportLatitudeTextBox.Text),
                    Longitude = double.Parse(AirportLongitudeTextBox.Text)
                };

                await dbHelper.AddAirport(airport);

                AddAirportResult.Text = "Added";
                AirportCodeTextBox.Text = string.Empty;
                AirportCityTextBox.Text = string.Empty;
                AirportLatitudeTextBox.Text = string.Empty;
                AirportLongitudeTextBox.Text = string.Empty;

                // to update the dropdownlists
                ddlAirports.DataBind();
                sdcAirports.DataBind();
                ddlRouteDepCode.DataBind();
                ddlRouteArrCode.DataBind();
                RoutesdcAirports.DataBind();

            }
            else
            {
                AddAirportResult.Text = "Make sure to fill the fields!";

            }

        }

        protected async void AddAirline_Click(object sender, EventArgs e)
        {

            // to make sure the fields are not empty
            if (!string.IsNullOrWhiteSpace(AirlineCodeTextBox.Text) && !string.IsNullOrWhiteSpace(AirlineNameTextBox.Text))
            {
                Airline airline = new Airline()
                {
                    Code = AirlineCodeTextBox.Text,
                    Name = AirlineNameTextBox.Text

                };
                await dbHelper.AddAirline(airline);

                AddAirlineResult.Text = "Added";
                AirlineCodeTextBox.Text = string.Empty;
                AirlineNameTextBox.Text = string.Empty;

                //to update the dropdownlists
                ddlAirlines.DataBind();
                sdcAirlines.DataBind();
                ddlRouteCarrier.DataBind();
                RoutesdcAirlines.DataBind();


            }
            else
            {
                AddAirlineResult.Text = "Make sure to fill the fields!";
            }
        }
        protected async void AddCarRental_Click(object sender, EventArgs e)
        {
            // to make sure the fields are not empty
            if (!string.IsNullOrWhiteSpace(CarRentalCodeTextBox.Text) && !string.IsNullOrWhiteSpace(CarRentalNameTextBox.Text))
            {
                CarRental carRental = new CarRental()
                {
                    Code = CarRentalCodeTextBox.Text,
                    Name = CarRentalNameTextBox.Text

                };
                await dbHelper.AddCarRental(carRental);

                AddCRResults.Text = "Added";
                CarRentalCodeTextBox.Text = string.Empty;
                CarRentalNameTextBox.Text = string.Empty;

                //to update the dropdownlists
                ddlCarRentals.DataBind();
                sdcCarRentals.DataBind();
            }
            else
            {
                AddCRResults.Text = "Make sure to fill the fields!";
            }
        }


    }
}