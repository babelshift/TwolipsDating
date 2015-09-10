using System;
namespace TwolipsDating.Business
{
    public interface IBaseService
    {
        void Dispose();
        IMilestoneService MilestoneService { set; }
        IValidationDictionary ValidationDictionary { get; set; }
    }
}
