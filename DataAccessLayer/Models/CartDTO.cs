using DataAccessLayer.Models.Vehicle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class CartDTO
    {
        
        [JsonIgnore]
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public  VehicleSimpleDTO VehicleData{get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
    }
    
    public class CartUpdateDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        [DefaultValue(0)]
        public int UserId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        [DefaultValue(0)]
        public int VehicleId { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        [DefaultValue(1)]
        public int Quantity { get; set; }
    }
    



}
