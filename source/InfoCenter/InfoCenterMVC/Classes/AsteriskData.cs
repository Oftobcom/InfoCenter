using InfoCenterMVC.Models;
using System.Collections.Generic;
using DataLibrary.DataAccess;
using DataLibrary.Models;

namespace InfoCenterMVC.Classes
{
    public class AsteriskData
    {
        public List<MvcAsteriskCallModel> GetAsteriskCallsInfo()
        {
            //throw new NotImplementedException();
            List<MvcAsteriskCallModel> calls = new List<MvcAsteriskCallModel>();
            DataProcessor db = new DataProcessor();
            List<AsteriskCallModel> data = db.GetAsteriskCallsInfo();

            foreach (AsteriskCallModel item in data)
            {
                calls.Add(new MvcAsteriskCallModel
                {
                    Id = item.Id,
                    Caller = item.Caller,
                    Callee = item.Callee,
                    Date_Time = item.Date_Time
                });
            }

            return calls;
        }

    }
}