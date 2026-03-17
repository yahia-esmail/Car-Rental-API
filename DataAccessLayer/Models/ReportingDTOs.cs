using Microsoft.Data.SqlClient.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Counts
    {
        public Counts(int users, int admins, int vehicles, int categories, int fuelTypes)
        {
            this.UsersCount = users;
            this.AdminsCount = admins;
            this.VehiclesCount = vehicles;
            this.CategoriesCount = categories;
            this.FuelTypesCount = fuelTypes;
        }
        public Counts()
        {
            
        }
        public int UsersCount { get; set; }
        public int AdminsCount { get; set; }
        public int VehiclesCount { get; set; }
        public int CategoriesCount { get; set; }
        public int FuelTypesCount { get; set; }
    }

}
