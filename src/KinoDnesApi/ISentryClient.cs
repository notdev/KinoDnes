using System;
using System.Threading.Tasks;

namespace KinoDnesApi
{
    public interface ISentryClient
    {
        Task CaptureException(Exception ex);
    }
}