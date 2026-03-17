using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Vehicle
{
    public class VehicleCreateDTO
    {
        public VehicleCreateDTO( string make, string model, int year, int mileage,
            int fuelTypeID, string plateNumber,int vehicleCategoryID, decimal rentalPricePerDay,
            bool isAvailableForRent, string imagePath,string features)
        {
            Make = make;
            Model = model;
            Year = year;
            Mileage = mileage;
            FuelTypeID = fuelTypeID;
            PlateNumber = plateNumber;
            VehicleCategoryID = vehicleCategoryID;
            RentalPricePerDay = rentalPricePerDay;
            IsAvailableForRent = isAvailableForRent;
            ImagePath = imagePath;
            Features = features;
        }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Mileage { get; set; }
        public int FuelTypeID { get; set; }
        public string PlateNumber { get; set; }
        public int VehicleCategoryID { get; set; }
        public decimal RentalPricePerDay { get; set; }
        public bool IsAvailableForRent { get; set; }
        public string ImagePath { get; set; }
        public string Features { get; set; } 

    }
    
    public class VehicleCreateFromUserDTO
    {
        [Required]
        public string Make { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        [DefaultValue(2010)]
        public int Year { get; set; }
        [Required]
        public int Mileage { get; set; }
        [Required]
        public int FuelTypeID { get; set; }
        [Required]
        [MinLength(4)]
        public string PlateNumber { get; set; }
        [Required]
        public int VehicleCategoryID { get; set; }
        [Required]
        public decimal RentalPricePerDay { get; set; }
        [Required]
        [DefaultValue(true)]
        public bool IsAvailableForRent { get; set; }
        [Required]
        public IFormFile VehicleImage{ get; set; }
        [MaxLength(300)]
        public string? Features { get; set; } 

    }
    
    public class VehicleUpdateDTO
    {
        public VehicleUpdateDTO(int id , string make, string model, int year, int mileage,
            int fuelTypeID, string plateNumber,int vehicleCategoryID, decimal rentalPricePerDay,
            bool isAvailableForRent, string imagePath,string features)
        {
            Id = id;
            Make = make;
            Model = model;
            Year = year;
            Mileage = mileage;
            FuelTypeID = fuelTypeID;
            PlateNumber = plateNumber;
            VehicleCategoryID = vehicleCategoryID;
            RentalPricePerDay = rentalPricePerDay;
            IsAvailableForRent = isAvailableForRent;
            ImagePath = imagePath;
            Features = features;
        }

        public VehicleUpdateDTO() // because the above constructor make front-end to enter id and image path to make instance from this class.
        {
            
        }
        [JsonIgnore]
        public int Id { get; set; }
        [Required]
        public string Make { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Mileage { get; set; }
        [Required]
        public int FuelTypeID { get; set; }
        [Required]
        public string PlateNumber { get; set; }
        [Required]
        public int VehicleCategoryID { get; set; }
        [Required]
        public decimal RentalPricePerDay { get; set; }
        [Required]
        public bool IsAvailableForRent { get; set; }
        [JsonIgnore]
        public string? ImagePath { get; set; }
        public string? Features { get; set; }

    }
    
    // with GetVehicle, GetAllVehicles (show info)
    public class VehicleReadDTO
    {
        public VehicleReadDTO(int vehicleID, string make, string model, int year, int mileage,
            string fuelType, string plateNumber,string categoryName, decimal rentalPricePerDay,
            bool isAvailableForRent, string imagePath,string features)
        {
            VehicleID = vehicleID;
            Make = make;
            Model = model; 
            Year = year;
            Mileage = mileage;
            FuelType = fuelType;
            PlateNumber = plateNumber;
            CategoryName = categoryName;
            RentalPricePerDay = rentalPricePerDay;
            IsAvailableForRent = isAvailableForRent;
            ImagePath = imagePath;
            Features = features;
        }
        public int VehicleID { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Mileage { get; set; }
        public string FuelType { get; set; }
        public string PlateNumber { get; set; }
        public string CategoryName { get; set; }
        public decimal RentalPricePerDay { get; set; }
        public bool IsAvailableForRent { get; set; }
        public string ImagePath { get; set; }
        public string Features { get; set; }

    }

    public class VehicleSimpleDTO
    {
        [JsonIgnore]
        public int VehicleID { get; set; }
        [JsonIgnore]
        public string Make { get; set; }
        [JsonIgnore]
        public string Model { get; set; }
        [JsonIgnore]
        public int Year { get; set; }
        public string Info
        {
            get { return $"{Make}, {Model}, {Year}"; }
        }

        public string? ImagePath { get; set; }
    }










}
