namespace Hackathon.services.impl;

using System.Collections.Generic;
using System.Linq;

public class HRDirectorService: IHRDirectorService
{
    public double CalculateHarmonicity(List<int> satisfactionPoints)
    {
        double harmonicSum = satisfactionPoints.Sum(x => 1.0 / x);
        int count = satisfactionPoints.Count;
        double harmonicMean = count / harmonicSum;
        
        return harmonicMean;
    }
}
