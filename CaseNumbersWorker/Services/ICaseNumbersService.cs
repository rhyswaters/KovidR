using System;
using System.Threading.Tasks;

namespace CaseNumbersWorker.Services
{
    public interface ICaseNumbersService
    {
        Task PublishCaseNumbers();
    }
}
