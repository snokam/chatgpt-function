using System;
using Sanity.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snokam.Sanity;
using System.Linq;
using Newtonsoft.Json;
namespace openai;
using Sanity.Linq.CommonTypes;

public static class SanityService
{
    private static SanityOptions options = new()
    {
        ApiVersion = "v1",
        ProjectId = Environment.GetEnvironmentVariable("SANITY_PROJECT"),
        Dataset = Environment.GetEnvironmentVariable("SANITY_DATASET"),
        Token = Environment.GetEnvironmentVariable("SANITY_TOKEN")
    };

    private static readonly SanityDataContext context = new(options);

    public static async Task<List<SanityEmployee>> GetEmployeesWithoutActiveCustomerContract()
	{
		DateTime currentDate = DateTime.Today;
		var employees = await context.DocumentSet<SanityEmployee>()
            .Include(employee => employee.CustomerContracts)
			.ToListAsync();

        employees = employees
			 .Where(employee => !employee.Id.Contains("draft"))
			 .Where(employee => {
				 SanityReference<SanityCustomerContract?> activeContract = employee.CustomerContracts?.Find(contract => contract?.Value?.StartDate <= currentDate && contract?.Value?.EndDate >= currentDate);
				 // || (activeContract?.Value != null && employee.CustomerContracts?.Find(contact => contact.Value?.EndDate > activeContract.Value.EndDate) == null);
				 return activeContract?.Value == null;
			 }) 
			.ToList();

        return employees;
    }
}
