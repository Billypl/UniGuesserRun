using Microsoft.EntityFrameworkCore;
using PartyGame.Entities;

namespace PartyGame
{
    public class Seeder
    {
        private readonly PlacesDbContext _dbContext;
    
        public Seeder(PlacesDbContext dbContext )
        {
            _dbContext = dbContext;
        }


        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Places.Any())
                {
                    var places = GetPlaces();
                    _dbContext.Places.AddRange( places );
                    _dbContext.SaveChanges();
                }
            }
        }


        private IEnumerable<Place> GetPlaces()
        {
            var places = new List<Place>()
            {
                new Place
                {
                    Id = 1,
                    Name = "Wydział Elektroniki, Telekomunikacji i Informatyki",
                    Description = "Wydział Elektroniki, Telekomunikacji i Informatyki (ETI) Politechniki Gdańskiej " +
                                  "(PG) jest jednym z czołowych wydziałów uczelni, oferującym kształcenie na kierunkach ",
                    Coordinates = new Coordinates { Latitude = 54.37170, Longitude = 18.61242 },
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/4/41/FOT_1171-ETI.jpg"
                },
                new Place
                {
                    Id = 2,
                    Name = "Gmach Główny",
                    Description = "Zabytkowy budynek w Gdańsku. Mieści się we Wrzeszczu przy ul. Narutowicza 11/12. ",
                    Coordinates = new Coordinates { Latitude = 54.371437068881185, Longitude = 18.619219721970538 },
                    ImageUrl =
                        "https://upload.wikimedia.org/wikipedia/commons/thumb/7/76/Politechnika_gdanska_2012.tif/lossy-page1-800px-Politechnika_gdanska_2012.tif.jpg"
                },
                new Place
                {
                    Id = 3,
                    Name = "Centrum Sportu Akademickiego",
                    Description =
                        "Centralnym obiektem jest zbudowany w 1962 roku kompleks Akademickiego Ośrodka Sportowego PG.",
                    Coordinates = new Coordinates { Latitude = 54.3693814546564, Longitude = 18.6313515818974 },
                    ImageUrl = "https://pg.edu.pl/files/csa/styles/large/public/2021-07/DSC_0564.jpg?itok=bQFL7JLl"
                }
            };

            return places;
        }
    }   
}
