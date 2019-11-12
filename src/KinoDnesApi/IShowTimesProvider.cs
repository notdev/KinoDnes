using System.Collections.Generic;
using KinoDnesApi.Model;

namespace KinoDnesApi
{
    public interface IShowTimesProvider
    {
        IEnumerable<Cinema> Get();
        void Set(IEnumerable<Cinema> cinemas);
        int GetAgeHours();
    }
}