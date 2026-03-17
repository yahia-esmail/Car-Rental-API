using DataAccessLayer;
using DataAccessLayer.Models;
using DataAccessLayer.Models.Vehicle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Category
    {
        public enum eMode { AddNew = 0 , Update = 1 }
        public eMode Mode;


        public Category(CategoryDTO DTO)
        {
            this.ID = DTO.ID;
            this.Name = DTO.Name;
            this.ImagePath = DTO.ImagePath;
            this.Mode = eMode.Update;
        }
        
        public Category(string name)
        {
            this.Name = name;

            this.Mode = eMode.AddNew;
        }

        public CategoryDTO CategoryDTO
        {
            get 
            {
                return new CategoryDTO(this.ID, this.Name,this.ImagePath);
            } 
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }


        private async Task<bool> _AddNew()
        {
            this.ID =  await VehicleCategoriesData.AddNewCategory(this.CategoryDTO);

            return this.ID != -1;
        }
        
        private async Task<bool> _Update()
        {
            return await VehicleCategoriesData.UpdateCategory(this.CategoryDTO);
        }

        public async Task<bool>Save()
        {
            switch(Mode)
            {
                case eMode.AddNew:
                    if(await _AddNew())
                    {
                        this.Mode = eMode.Update;
                        return true;
                    }
                    else
                        return false;

                case eMode.Update:
                    return await _Update();

                
            }

            return false;
        }



        public static async Task<Category?> Find(int id)
        {
            var DTO = await VehicleCategoriesData.GetCategoryById(id);

            return DTO != null?
                new Category(DTO):null;
        }


        public static async Task<IEnumerable<CategoryDTO>> GetAllCategories()
        {
            return (await VehicleCategoriesData.GetAllCategories());
        }
        public static async Task<IEnumerable<VehicleReadDTO>> GetAllVehicles(int categoryId)
        {
            return await VehicleCategoriesData.GetVehicles(categoryId);
        }
        public static async Task<bool> Delete(int id)
        {
            return (await VehicleCategoriesData.DeleteCategory(id));
        }

        public async Task<bool> Delete()
        {
            return await Delete(this.ID);
        }

        public async Task<bool> UpdateImage(string imageUrl)
        {
            if (await VehicleCategoriesData.UpdateImage(this.ID, imageUrl))
            {
                this.ImagePath = imageUrl;
                return true;
            }
            else
                return false;
        }



    }


}
