using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;


namespace ErikTheCoder.Sandbox.Dapper
{
    public static class Program
    {
        private const string _database = "Data Source=localhost;Initial Catalog=Dapper;Integrated Security=True";
        private const int _technicianCount = 500;
        private const int _customerCount = 10_000;
        private const int _dayCount = 365;
        private const int _serviceCallsPerTechPerDayMin = 4;
        private const int _serviceCallsPerTechPerDayMax = 8;
        private const int _maxCustomersPerTechPerDay = 8;
        private const int _percentPriorDayCallsOpen = 15;
        private const int _nameLength = 10;
        private static readonly char[] _alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly char[] _digits = "0123456789".ToCharArray();
        private static readonly Random _random = new Random();


        public static void Main()
        {
            using (SqlConnection connection = new SqlConnection(_database))
            {
                connection.Open();
                List<int> technicianIds = CreateTechnicians(connection);
                List<int> customerIds = CreateCustomers(connection);
                CreateServiceCalls(connection, technicianIds, customerIds);
            }
        }

        
        private static List<int> CreateTechnicians(SqlConnection Connection)
        {
            List<int> technicianIds = new List<int>();
            string sql;
            SqlCommand command;
            for (int index = 0; index < _technicianCount; index++)
            {
                string name = GetRandomName(_nameLength);
                sql = $"insert into technicians (name) values ('{name}')";
                using (command = new SqlCommand(sql, Connection)) { command.ExecuteNonQuery(); }
            }
            sql = "select id from technicians";
            command = new SqlCommand(sql, Connection);
            using (SqlDataReader reader = command.ExecuteReader()) { while (reader.Read()) { technicianIds.Add(reader.GetInt32(0)); } }
            return technicianIds;
        }



        private static List<int> CreateCustomers(SqlConnection Connection)
        {
            List<int> customerIds = new List<int>();
            string sql;
            SqlCommand command;
            for (int index = 0; index < _customerCount; index++)
            {
                string name = GetRandomName(_nameLength);
                string address = $"{GetRandomDigits(4)} {GetRandomName(_nameLength)}";
                string city = GetRandomName(_nameLength);
                string state = GetRandomName(2);
                string zipCode = GetRandomDigits(5);
                sql = $"insert into customers (name, address, city, state, zipcode) values ('{name}', '{address}', '{city}', '{state.ToUpper()}', '{zipCode}')";
                using (command = new SqlCommand(sql, Connection)) { command.ExecuteNonQuery(); }
            }
            sql = "select id from customers";
            command = new SqlCommand(sql, Connection);
            using (SqlDataReader reader = command.ExecuteReader()) { while (reader.Read()) { customerIds.Add(reader.GetInt32(0)); } }
            return customerIds;
        }


        private static void CreateServiceCalls(SqlConnection Connection, IReadOnlyCollection<int> TechnicianIds, IReadOnlyList<int> CustomerIds)
        {
            for (int day = 0; day < _dayCount; day++)
            {
                DateTime scheduled = DateTime.Now - TimeSpan.FromDays(day);
                bool open = (day == 0) || ((day == 1) && (_random.Next(1, 101) <= _percentPriorDayCallsOpen));
                foreach (int technicianId in TechnicianIds)
                {
                    int customerCount = _random.Next(1, _maxCustomersPerTechPerDay + 1);
                    List<int> customerIds = GetRandomCustomerIds(CustomerIds, customerCount);
                    int serviceCallCount = _random.Next(_serviceCallsPerTechPerDayMin, _serviceCallsPerTechPerDayMax + 1);
                    int customerIndex = 0;
                    for (int serviceCallIndex = 0; serviceCallIndex < serviceCallCount; serviceCallIndex++)
                    {
                        int customerId = customerIds[customerIndex];
                        string sql = $"insert into servicecalls (customerid, technicianid, scheduled, [open]) values ({customerId}, {technicianId}, '{scheduled}', {(open ? 1 : 0)})";
                        using (SqlCommand command = new SqlCommand(sql, Connection)) { command.ExecuteNonQuery(); }
                        customerIndex++;
                        if (customerIndex == customerCount) customerIndex = 0;
                    }
                }
            }
        }


        private static List<int> GetRandomCustomerIds(IReadOnlyList<int> CustomerIds, int Count)
        {
            List<int> customerIds = new List<int>();
            do
            {
                int customerId = CustomerIds[_random.Next(0, CustomerIds.Count)];
                // Don't add the same customer more than once.
                if (!customerIds.Contains(customerId)) customerIds.Add(customerId);
            } while (customerIds.Count < Count);
            return customerIds;
        }


        private static string GetRandomName(int Length)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(char.ToUpper(_alphabet[_random.Next(0, _alphabet.Length)]));
            while (stringBuilder.Length < Length) stringBuilder.Append(_alphabet[_random.Next(0, _alphabet.Length)]);
            return stringBuilder.ToString();
        }


        private static string GetRandomDigits(int Length)
        {
            StringBuilder stringBuilder = new StringBuilder();
            while (stringBuilder.Length < Length) stringBuilder.Append(_digits[_random.Next(0, _digits.Length)]);
            return stringBuilder.ToString();
        }
    }
}
