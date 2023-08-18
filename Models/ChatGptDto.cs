using System.Collections.Generic;

namespace openai;
public class EmployeesFilterDto
{
	public string? reason { get; set; }
	public List<string> candidates { get; set; }
}

