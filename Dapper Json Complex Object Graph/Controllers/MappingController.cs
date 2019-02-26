using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using ErikTheCoder.Sandbox.Dapper.Contract;
using Microsoft.AspNetCore.Mvc;


namespace ErikTheCoder.Sandbox.Dapper.Service.Controllers
{
    public class MappingController : Controller, IMappingService
    {
        private readonly IAppSettings _appSettings;


        public MappingController(IAppSettings AppSettings)
        {
            _appSettings = AppSettings;
        }


        [HttpGet("/mapping/getopenservicecalls")]
        public async Task<GetOpenServiceCallsResponse> GetOpenServiceCallsAsync()
        {
            GetOpenServiceCallsResponse response = new GetOpenServiceCallsResponse
            {
                ServiceCalls = new ServiceCalls(),
                Customers = new Customers(),
                Technicians = new Technicians()
            };
            // Each row in SQL result represents a unique service call but may contain duplicate customers and technicians.
            const string sql = @"select sc.Id, sc.Scheduled, sc.[Open], c.Id, c.Name, c.Address, c.City, c.State, c.ZipCode, t.Id, t.Name
                from ServiceCalls sc
                inner join Customers c on sc.CustomerId = c.Id
                inner join Technicians t on sc.TechnicianId = t.Id
                where sc.[Open] = 1";
            using (SqlConnection connection = new SqlConnection(_appSettings.Database))
            {
                await connection.OpenAsync();
                await connection.QueryAsync<ServiceCall, Customer, Technician, ServiceCall>(sql, (ServiceCall, Customer, Technician) =>
                {
                    // This lambda method runs once per row in SQL result.
                    response.ServiceCalls.Add(ServiceCall);
                    // Reference existing customers and technicians or add to collection if first time encountered.
                    if (!response.Customers.TryGetValue(Customer.Id, out Customer customer))
                    {
                        // Customer mapped by Dapper not in collection.
                        customer = Customer;
                        response.Customers.Add(customer);
                    }
                    if (!response.Technicians.TryGetValue(Technician.Id, out Technician technician))
                    {
                        // Technician mapped by Dapper not in collection.
                        technician = Technician;
                        response.Technicians.Add(technician);
                    }
                    // Configure relationships among objects.
                    ServiceCall.Customer = customer;
                    ServiceCall.Technician = technician;
                    customer.ServiceCalls.Add(ServiceCall);
                    if (!customer.Technicians.Contains(technician.Id))
                    {
                        // Technician not found in customer's collection.
                        customer.Technicians.Add(technician);
                    }
                    technician.ServiceCalls.Add(ServiceCall);
                    if (!technician.Customers.Contains(customer.Id))
                    {
                        // Customer not found in technician's collection.
                        technician.Customers.Add(customer);
                    }
                    return null;
                });
            }
            return response;
        }
    }
}
