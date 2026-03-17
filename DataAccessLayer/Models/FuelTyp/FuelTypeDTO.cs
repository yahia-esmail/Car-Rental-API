using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.FuelTyp
{
    public class FuelTypeDTO
    {
        public FuelTypeDTO(int id, string fuelType)
        {
            this.Id = id;
            this.FuelType = fuelType;
        }
        public int Id { get; set; }
        public string FuelType { get; set; }
    }

    public class CreateFuelTypeDTO
    {
        public CreateFuelTypeDTO(string fuelType)
        {
            this.FuelType = fuelType;
        }
        public string FuelType { get; set; }
    }

}
