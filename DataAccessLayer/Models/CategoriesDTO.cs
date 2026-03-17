using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class CategoryDTO
    {
        
        public CategoryDTO(int ID, string CategoryName,string ImagePath)
        {
            this.ID = ID;
            this.Name = CategoryName;
            this.ImagePath = ImagePath;
        }
        public int ID { get; set; }
        public string Name { get; set; } 
        public string ImagePath { get; set; }

    }
    
    public class CreateCategoryDTO
    {
        [Length(4,50)]
        [Required]
        public string Name { get; set; }
        [Required]
        public IFormFile CategoryImage{ get; set; }

    }
    
    public class UpdateCategoryDTO
    {
        [Length(4,50)]
        [Required]
        public string Name { get; set; }
    }

}
