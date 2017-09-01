using System.Collections.Generic;
using KinoDnes.Models;

namespace KinoDnes.DataProvider
{
    public interface IDataProvider
    {
        IEnumerable<Cinema> GetAllCinemas();
    }
}