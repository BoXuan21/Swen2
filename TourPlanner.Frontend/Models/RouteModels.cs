using System;

namespace TourPlanner.Frontend.Models
{
    public class Coordinates
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public class GeocodingResult
    {
        public string Lat { get; set; }
        public string Lon { get; set; }
        public string DisplayName { get; set; }
    }

    public class RouteResult
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
    }

    public class RouteResponse
    {
        public RouteFeature[] Features { get; set; }
    }

    public class RouteFeature
    {
        public RouteGeometry Geometry { get; set; }
        public RouteProperties Properties { get; set; }
    }

    public class RouteGeometry
    {
        public double[][] Coordinates { get; set; }
    }

    public class RouteProperties
    {
        public RouteSegment[] Segments { get; set; }
    }

    public class RouteSegment
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
    }
} 