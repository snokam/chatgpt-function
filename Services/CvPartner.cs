using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace openai
{
    public class CvPartnerService
    {
        static HttpClient client = new HttpClient();
        static string baseUrl = Environment.GetEnvironmentVariable("CV_PARTNER_BASE_URL");
        static string token = Environment.GetEnvironmentVariable("CV_PARTNER_TOKEN");

        static CvPartnerService(){
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Token token=\"{token}\"");
        }

		public static async Task<List<CvPartnerUser>> GetEmployees()
		{
			HttpResponseMessage response = await client.GetAsync($"{baseUrl}/v1/users");
			List<CvPartnerUser> users = await response.Content.ReadAsAsync<List<CvPartnerUser>>();
			return users;
		}

		public static CvPartnerUser? GetUser(string? email, List<CvPartnerUser> users) {
            return users.Find(u => u.email == email);
        }

        public static async Task<Cv> GetCv(string? email, List<CvPartnerUser> users) {
            CvPartnerUser? user = GetUser(email, users);
            Cv cv = new Cv();
            try{
                HttpResponseMessage response = await client.GetAsync($"{baseUrl}/v3/cvs/{user?.user_id}/{user?.default_cv_id}");
                cv = await response.Content.ReadAsAsync<Cv>();
                return cv;
            }
            catch (Exception error)
            {
                Console.WriteLine($"Failed to fetch cv for {user?.email}, error ${JsonConvert.SerializeObject(error)}");
                return cv;
            }
        }

        public class CvPartnerUser {
            public string? user_id { get; set; }
            public string? email { get; set; }
            public string? default_cv_id { get; set; }
        }

        public class Cv {
            public string? _id { get; set; }
            public List<Qualification>? key_qualifications { get; set; }
        }

        public class Qualification {
            public string? _id { get; set; }
            public Locale? long_description { get; set; }
        }

        public class Locale {
            public string? no { get; set; }
        }
    }
}

