using System;

namespace PopulationDensity;

public class Country
{
    public string Name { get; init;} = ""; //init makes the data immutable  and the "" to get rid of non nullable warnings
    public string Region { get; init;} = "";
    public string SubRegion { get; init;} = ""; 
    public int Population { get; init;}
    public double Area {get; init;}

    public double CalculateDensity()
    {
        // cant divide by 0 so we would just return density as zero no negatives either
        if (Area <= 0) 
        {
            return 0;
        }
        // if good data calculate density
        else 
        {
            return Population / Area;
        }
    }

    public string CleanString()
    {
        // Commas mess with csv format so replace
        string CleanName = Name.Replace(",", ";");
        double Density = CalculateDensity(); // get density
        // format string thats ready for wCSV
        return $"{CleanName},{Region},{SubRegion},{Population},{Area},{Density}"; 
    }
}