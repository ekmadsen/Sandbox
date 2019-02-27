select sc.Id, sc.Scheduled, sc.[Open], c.Id, c.Name, c.Address, c.City, c.State, c.ZipCode, t.Id, t.Name
from ServiceCalls sc
inner join Customers c on sc.CustomerId = c.Id
inner join Technicians t on sc.TechnicianId = t.Id
where sc.[Open] = 1
order by sc.Id asc