using RestaurantAPI.Models;
using System.Collections.Generic;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dto);
        public List<DishDto> GetAll(int restaurantId);
        public DishDto GetById(int restaurantId, int dishId);
        void RemoveAll(int restaurantId);
        public void Remove(int restaurantId, int dishId);
    }
}
