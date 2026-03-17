using DataAccessLayer;
using DataAccessLayer.Models.FuelTyp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class FuelType
    {
        public enum eMode { AddNew = 0, Update = 1}
        public eMode Mode = eMode.AddNew;

        public FuelTypeDTO FT_DTO 
        { 
            get
            { 
                return new FuelTypeDTO(this.Id, this.Type); 
            } 
        }
        public FuelType(FuelTypeDTO FT_DTO)
        {
            this.Id = FT_DTO.Id;
            this.Type = FT_DTO.FuelType;

            this.Mode = eMode.Update;
        }
        
        public FuelType(CreateFuelTypeDTO FT_DTO)
        {
            this.Type = FT_DTO.FuelType;

            this.Mode = eMode.AddNew;
        }


        public int Id { get; set; } 
        public string Type { get; set; }

        private async Task<bool> _AddNewFT()
        {
            this.Id = await FuelTypeData.AddNewFuelType(this.Type);

            return this.Id != -1;
        }

        private async Task<bool> _UpdateFT()
        {
            return await FuelTypeData.UpdateFuelType(FT_DTO);
        }


        public async Task<bool> Save()
        {
            switch(Mode)
            {
                case eMode.AddNew:
                    if (await _AddNewFT())
                    {
                        this.Mode = eMode.Update;
                        return true;
                    }
                    else
                        return false;

                case eMode.Update:
                    return await _UpdateFT();
            }

            return false;
        }
        public static async Task<List<FuelTypeDTO>> GetAllFuelTypes()
        {
            return await FuelTypeData.GetAllFuelTypes();
        }

        public static async Task<FuelType> Find(int id)
        {
            var FT_DTO = await FuelTypeData.GetFuelType(id);

            if (FT_DTO == null)
                return null;

            return new FuelType(FT_DTO);
        }

        public async Task<bool> DeleteFuelType()
        {
            return await FuelTypeData.DeleteFuelType(this.Id);
        }



    }
}
