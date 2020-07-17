using DocketManagerSample.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocketManagerSample
{
    public class TestProducts
    {
        public static List<ProductModel> Products => new List<ProductModel>() {
                new ProductModel
                {
                    Id = "1",
                    Name = "Cat 1",
                    Image = "http://lorempixel.com/640/480/cats/1/",
                    Type = "cat"
                },
                new ProductModel
                {
                    Id = "2",
                    Name = "City",
                    Image = "http://lorempixel.com/640/480/city/1/",
                    Type = "city"
                },
                new ProductModel
                {
                    Id = "3",
                    Name = "Food",
                    Image = "http://lorempixel.com/640/480/food/1/",
                    Type = "food"
                },
                new ProductModel
                {
                    Id = "4",
                    Name = "Cat 2",
                    Image = "http://lorempixel.com/640/480/cats/2/",
                    Type = "cat"
                },
                new ProductModel
                {
                    Id = "5",
                    Name = "City 2",
                    Image = "http://lorempixel.com/640/480/city/2/",
                    Type = "city"
                },
                new ProductModel
                {
                    Id = "6",
                    Name = "Food 2",
                    Image = "http://lorempixel.com/640/480/food/2/",
                    Type = "food"
                },

                new ProductModel
                {
                    Id = "7",
                    Name = "Cat 3",
                    Image = "http://lorempixel.com/640/480/cats/3/",
                    Type = "cat"
                },
                new ProductModel
                {
                    Id = "8",
                    Name = "City 3",
                    Image = "http://lorempixel.com/640/480/city/3/",
                    Type = "city"
                },
                new ProductModel
                {
                    Id = "9",
                    Name = "Food 3",
                    Image = "http://lorempixel.com/640/480/food/3/",
                    Type = "food"
                }
        };
           
    }
}
