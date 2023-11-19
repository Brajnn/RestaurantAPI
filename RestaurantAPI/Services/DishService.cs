using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace RestaurantAPI.Services
{

    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _context;
        private readonly IMapper _mapper;

        public DishService(RestaurantDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public int Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);

            // Ręczne mapowanie z CreateDishDto na Dish
            var dishEntity = new Dish
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                RestaurantId = restaurantId
            };

            _context.Dishes.Add(dishEntity);
            _context.SaveChanges();

            return dishEntity.Id;
        }
        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dish = _context.Dishes.FirstOrDefault(r => r.Id == dishId);
            if(dish is null || dish.RestaurantId!=restaurantId)
            {
                throw new NotFoundException("Dish not found");
            }
            var dishDto = _mapper.Map<DishDto>(dish);
            return dishDto;
        }
        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dishDtos= _mapper.Map<List<DishDto>>(restaurant.Dishes);
            return dishDtos;
            
        }
        public void RemoveAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            _context.RemoveRange(restaurant.Dishes);
            _context.SaveChanges();
        }
        public void Remove(int restaurantId, int dishId)
        {
            var restaurant=GetAll(restaurantId);
            var dish=_context.Dishes.FirstOrDefault(r=>r.Id == dishId);
            _context.Remove(dish);
            _context.SaveChanges();
        }

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _context
                .Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == restaurantId);
            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");
            return restaurant;

        }
    }
}
