using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniGuesser.Domain.ValueObjects;

namespace UniGuesser.Application.Models.GeolocationGameModels
{
    class GetDistanceFromPlaceDto
    {
        public required string Id;
        public required Coordinates ActualPosition { get; set; }
    }
}
