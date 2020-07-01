﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebRole1.Models;

namespace WebRole1
{
    public class DbHelper
    {
        // this class will handle the communications between the database and the controllers using the model classes as objects.

        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=lab4db;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private readonly string connectionString2 = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=aspnet-WebRole1-20200609040122;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // to get an airport from the database
        public async Task<Airport> GetAirport(string code)
        {
            Airport airport = new Airport();

            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM dbo.Airports WHERE Code = @Code;";
                    cmd.Parameters.AddWithValue("@Code", code);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            airport.Code = reader.GetString(0);
                            airport.City = reader.GetString(1);
                            airport.Latitude = reader.GetDouble(2);
                            airport.Longitude = reader.GetDouble(3);
                        }
                    }
                }
            }
            return airport;
        }

        // to get a list of airports from the database
        public async Task<List<Airport>> GetAirports()
        {
            List<Airport> airports = new List<Airport>();

            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM dbo.Airports;";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            airports.Add(new Airport()
                            {
                                Code = reader.GetString(0),
                                City = reader.GetString(1),
                                Latitude = reader.GetDouble(2),
                                Longitude = reader.GetDouble(3)
                            });
                        }
                    }
                }
            }

            return airports;
        }

        // to add an airport to the database
        public async Task AddAirport(Airport airport)
        {
            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO dbo.Airports(Code, City, Latitude, Longitude) VALUES(@Code, @City, @Latitude, @Longitude);";
                    cmd.Parameters.AddWithValue("@Code", airport.Code);
                    cmd.Parameters.AddWithValue("@City", airport.City);
                    cmd.Parameters.AddWithValue("@Latitude", airport.Latitude);
                    cmd.Parameters.AddWithValue("@Longitude", airport.Longitude);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // to update an airport in the database
        public async Task UpdateAirport(Airport airport)
        {
            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "UPDATE dbo.Airports SET  City = @City, Latitude = @Latitude, Longitude = @Longitude WHERE Code = @Code;";
                    cmd.Parameters.AddWithValue("@City", airport.City);
                    cmd.Parameters.AddWithValue("@Latitude", airport.Latitude);
                    cmd.Parameters.AddWithValue("@Longitude", airport.Longitude);
                    cmd.Parameters.AddWithValue("@Code", airport.Code);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // to get an airline from the database
        public async Task<Airline> GetAirline(string code)
        {
            Airline airline = new Airline();

            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM dbo.Airlines WHERE Code = @Code;";
                    cmd.Parameters.AddWithValue("@Code", code);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            airline.Code = reader.GetString(0);
                            airline.Name = reader.GetString(1);

                        }
                    }
                }
            }
            return airline;
        }


        // to get list of airlines
        public async Task<List<Airline>> GetAirlines()
        {
            List<Airline> airlines = new List<Airline>();

            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM dbo.Airlines;";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            airlines.Add(new Airline()
                            {
                                Code = reader.GetString(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return airlines;
        }

        // to update an airline in the database
        public async Task UpdateAirline(Airline airline)
        {
            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "UPDATE dbo.Airlines SET   Name = @Name WHERE Code = @Code;";
                    cmd.Parameters.AddWithValue("@Name", airline.Name);
                    cmd.Parameters.AddWithValue("@Code", airline.Code);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // to add an airline
        public async Task AddAirline(Airline airline)
        {
            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO dbo.Airlines(Code, Name) VALUES(@Code, @Name);";
                    cmd.Parameters.AddWithValue("@Code", airline.Code);
                    cmd.Parameters.AddWithValue("@Name", airline.Name);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // to get a route from the database
        public async Task<Route> GetRoute(string flightNumber)
        {
            Route route = new Route();

            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM dbo.Routes WHERE FlightNumber = @FlightNumber;";
                    cmd.Parameters.AddWithValue("@FlightNumber", flightNumber);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            route.FlightNumber = reader.GetString(0);
                            route.Carrier = reader.GetString(1);
                            route.Departure = reader.GetString(2);
                            route.Arrival = reader.GetString(3);
                        }
                    }
                }
            }
            return route;
        }


        // to get list of routes
        public async Task<List<Route>> GetRoutes()
        {
            List<Route> routes = new List<Route>();

            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM dbo.Routes;";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            routes.Add(new Route()
                            {
                                FlightNumber = reader.GetString(0),
                                Carrier = reader.GetString(1),
                                Departure = reader.GetString(2),
                                Arrival = reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return routes;
        }

        // to add a route
        public async Task AddRoute(Route route)
        {
            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO dbo.Routes(FlightNumber, Carrier, Departure, Arrival) VALUES(@FlightNumber, @Carrier, @Departure, @Arrival);";
                    cmd.Parameters.AddWithValue("@FlightNumber", route.FlightNumber);
                    cmd.Parameters.AddWithValue("@Carrier", route.Carrier);
                    cmd.Parameters.AddWithValue("@Departure", route.Departure);
                    cmd.Parameters.AddWithValue("@Arrival", route.Arrival);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // to update a route in the database
        public async Task UpdateRoute(Route route)
        {
            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "UPDATE dbo.Routes SET  Carrier = @Carrier, Departure = @Departure, Arrival = @Arrival WHERE FlightNumber = @FlightNumber;";
                    cmd.Parameters.AddWithValue("@Carrier", route.Carrier);
                    cmd.Parameters.AddWithValue("@Departure", route.Departure);
                    cmd.Parameters.AddWithValue("@Arrival", route.Arrival);
                    cmd.Parameters.AddWithValue("@FlightNumber", route.FlightNumber);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // to get an airport from the database
        public async Task<CarRental> GetCarRental(string code)
        {
            CarRental carRental = new CarRental();

            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM dbo.CarRentals WHERE Code = @Code;";
                    cmd.Parameters.AddWithValue("@Code", code);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            carRental.Code = reader.GetString(0);
                            carRental.Name = reader.GetString(1);

                        }
                    }
                }
            }
            return carRental;
        }

        // to get list of car rentals
        public async Task<List<CarRental>> GetCarRentals()
        {
            List<CarRental> carRentals = new List<CarRental>();

            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM dbo.CarRentals;";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            carRentals.Add(new CarRental()
                            {
                                Code = reader.GetString(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return carRentals;
        }

        // to add a car rental
        public async Task AddCarRental(CarRental carRental)
        {
            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO dbo.CarRentals(Code, Name) VALUES(@Code, @Name);";
                    cmd.Parameters.AddWithValue("@Code", carRental.Code);
                    cmd.Parameters.AddWithValue("@Name", carRental.Name);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // to update a car rental in the database
        public async Task UpdateCarRental(CarRental carRental)
        {
            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "UPDATE dbo.CarRentals SET   Name = @Name WHERE Code = @Code;";
                    cmd.Parameters.AddWithValue("@Name", carRental.Name);
                    cmd.Parameters.AddWithValue("@Code", carRental.Code);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        // to add a flight
        public async Task AddFlight(Flight flight)
        {
            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO dbo.Flights(PassengerName, PassportNumber, FlightNumber, DepartureDate, Price, PassengerType) VALUES(@PassengerName, @PassportNumber, @FlightNumber, @DepartureDate, @Price, @PassengerType);";
                    cmd.Parameters.AddWithValue("@PassengerName", flight.PassengerName);
                    cmd.Parameters.AddWithValue("@PassportNumber", flight.PassportNumber);
                    cmd.Parameters.AddWithValue("@FlightNumber", flight.FlightNumber);
                    cmd.Parameters.AddWithValue("@DepartureDate", flight.DepartureDate);
                    cmd.Parameters.AddWithValue("@Price", flight.Price);
                    cmd.Parameters.AddWithValue("@PassengerType", flight.PassengerType);


                    await cmd.ExecuteNonQueryAsync();

                }
            }
        }

        // to add a CarBooking
        public async Task AddCarBooking(CarBooking carBooking)
        {
            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO dbo.CarBookings(CarRentalCode, NumberOfSeats, DriverAge, FuelType, Price, CarModel) VALUES(@CarRentalCode, @NumberOfSeats, @DriverAge, @FuelType, @Price, @CarModel);";
                    cmd.Parameters.AddWithValue("@CarRentalCode", carBooking.CarRentalCode);
                    cmd.Parameters.AddWithValue("@NumberOfSeats", carBooking.NumberOfSeats);
                    cmd.Parameters.AddWithValue("@DriverAge", carBooking.DriverAge);
                    cmd.Parameters.AddWithValue("@FuelType", carBooking.FuelType);
                    cmd.Parameters.AddWithValue("@Price", carBooking.Price);
                    cmd.Parameters.AddWithValue("@CarModel", carBooking.CarModel);


                    await cmd.ExecuteNonQueryAsync();

                }
            }
        }


        // to calculate the price of the trip according to longitude and latitude of from destination and the to destination
        // then checks the type of the passenger and according to that it adds the discounts
        //0.2525 is supposed to be hardcoded baserate for all trips.
        public int FlightPriceCalc(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude, string type)
        {
            double price = 0;
            GeoCoordinate fromCord = new GeoCoordinate(fromLatitude, fromLongitude);
            GeoCoordinate toCord = new GeoCoordinate(toLatitude, toLongitude);

            double distance = fromCord.GetDistanceTo(toCord);

            if (type == "Infant")
            {
                price = 0.2525 * distance * 0.1;
            }
            if (type == "Child")
            {
                price = 0.2525 * distance * 0.67;

            }
            if (type == "Adult")
            {
                price = 0.2525 * distance;

            }
            if (type == "Senior")
            {
                price = 0.2525 * distance * 0.75;

            }
            return (int)price;

        }

        // apply the calculations and returns the price for this service.
        public double CarRentalCalculation(CarBooking carBooking)
        {
            double priceA = 0.0, priceB = 0.0, price = 0.0;
            if (carBooking.DriverAge < 45)
            {
                priceB = (50 - carBooking.DriverAge) * 200;

            }
            else
            {
                priceB = (carBooking.DriverAge - 40) * 225;

            }

            if (carBooking.NumberOfSeats <= 5)
            {
                priceA = (carBooking.CarModel - 2010) * 600;

            }
            else
            {
                priceA = (carBooking.CarModel - 2010) * 900;

            }
            price = priceA + priceB;


            if (carBooking.FuelType == "Diesel")
            {
                price = price + (price * 0.2);
            }

            return Math.Round(price, 2);
        }


        // to get the lockoutStatus of a user
        public async Task<bool> GetlockoutStatus(string id)
        {
            bool isEnabled =false;

            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString2))
            {
                //opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT LockoutEnabled FROM dbo.AspNetUsers WHERE Id = @Id;";
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            isEnabled = reader.GetBoolean(0);

                        }
                    }
                }
            }
            return isEnabled;
        }

        // to update the lockoutStatus of a user
        public async Task UpdateLockoutStatus(string id, bool isEnabled)
        {
            // using with resources to establish the connection and disposes it after it is done with the task
            using (SqlConnection connection = new SqlConnection(connectionString2))
            {
                // opens connection
                await connection.OpenAsync();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "UPDATE dbo.AspNetUsers  SET LockoutEnabled = @LockoutEnabled WHERE Id = @Id;";
                    cmd.Parameters.AddWithValue("@LockoutEnabled", isEnabled);
                    cmd.Parameters.AddWithValue("@Id", id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

    }
}