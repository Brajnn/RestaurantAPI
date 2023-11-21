using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RestaurantAPI.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapepr, ILogger<RestaurantService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapepr;
            _logger = logger;
        }
        public void Delete(int id)
        {
            _logger.LogError($"Restaurant with id:  {id} DELETE action invoked");
            var restaurant = _dbContext
            .Restaurants
            .FirstOrDefault(r => r.Id == id);

            if( restaurant is null) 
                throw new KeyNotFoundException("Resstaurant not found");
            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();


        }
        public void Update(int id, UpdateRestaurantDto dto)
        {
            var restaurant = _dbContext
            .Restaurants
            .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;
            
            _dbContext.SaveChanges();

            
        }
        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext
            .Restaurants
            .Include(r => r.Address)
            .Include(r => r.Dishes)
            .FirstOrDefault(r => r.Id == id);

            if (restaurant is null) throw new KeyNotFoundException("Resstaurant not found");

            var result = _mapper.Map<RestaurantDto>(restaurant);
            return result;
        }

        public PagedResult<RestaurantDto> GetAll(RestaurantQuery query)
        {
            var baseQuery = _dbContext
             .Restaurants
             .Include(r => r.Address)
             .Include(r => r.Dishes)
             .Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                      || r.Description.ToLower().Contains(query.SearchPhrase.ToLower())));
            if(!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelectors= new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    {nameof(Restaurant.Name), r=> r.Name},
                    {nameof(Restaurant.Description), r=> r.Description},
                    {nameof(Restaurant.Category), r=> r.Category},
                };
                var selectedColumn= columnsSelectors[query.SortBy];


                baseQuery=query.SortDirection==SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    :baseQuery.OrderByDescending(selectedColumn);
            }
            var restaurants = baseQuery
             .Skip(query.PageSize * (query.PageNumber -1))
             .Take(query.PageSize)
             .ToList();

            var totalItemsCount = baseQuery.Count();

            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result=new PagedResult<RestaurantDto>(restaurantsDtos,totalItemsCount,query.PageSize,query.PageNumber);

            return result;
        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;

        }
    }
}
