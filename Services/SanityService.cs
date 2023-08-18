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

    public static async Task<List<SanityEmployee>> GetEmployeesWithoutProject()
	{
		DateTime currentDate = DateTime.Today;
		var employees = await context.DocumentSet<SanityEmployee>()
            .Include(employee => employee.CustomerContracts)
			.ToListAsync();

		employees = employees
			  .Where(employee => !employee.Id.Contains("draft"))
			  .Where(employee => {
				  List<SanityCustomerContract> customerContracts = employee.CustomerContracts != null ? employee.CustomerContracts.Select(contact => contact.Value).ToList() : new List<SanityCustomerContract>();
				  SanityCustomerContract activeContract = customerContracts?.Find(contract => contract?.StartDate <= currentDate && contract?.EndDate >= currentDate);
				  Boolean hasUpcomingProject = customerContracts.Where(contact => contact?.StartDate > activeContract?.EndDate).ToList().Count() > 0;
				  Console.WriteLine(employee?.Name);
				  Console.WriteLine(activeContract?.EndDate);
				  return (!hasUpcomingProject && (activeContract == null || currentDate >= activeContract.EndDate.AddDays(-60)));
			  })
			 .ToList();

		return employees;
    }
}