using System.Collections.Generic;
using KinoDnesApi.Model;

namespace KinoDnesApi.DataProviders
{
    public interface ICsfdDataProvider
    {
        IEnumerable<Cinema> GetAllShowTimes();
    }
}