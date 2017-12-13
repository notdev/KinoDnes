using System.Collections.Generic;
using KinoDnesApi.Model;

namespace KinoDnesApi
{
    public interface IFileSystemShowTimes
    {
        IEnumerable<Cinema> Get();
        void Set(IEnumerable<Cinema> showtimes);
        int GetAgeHours();
    }
}