select t.Name as TechnicanName, count(*) as ServiceCalls
from Technicians t
inner join ServiceCalls sc on sc.TechnicianId = t.Id
where sc.[Open] = 1
group by t.Name
order by count(*) desc

select t.Name as TechnicianName, count(distinct sc.CustomerId) as Customers
from Technicians t
inner join ServiceCalls sc on sc.TechnicianId = t.Id
where sc.[Open] = 1
group by t.Name
order by count(distinct sc.CustomerId) desc


select c.Name as CustomerName, count(*) as ServiceCalls
from Customers c
inner join ServiceCalls sc on sc.CustomerId = c.Id
where sc.[Open] = 1
group by c.Name
order by count(*) desc

select c.Name as CustomerName, count(distinct sc.TechnicianId) as Technicians
from Customers c
inner join ServiceCalls sc on sc.CustomerId = c.Id
where sc.[Open] = 1
group by c.Name
order by count(distinct sc.TechnicianId) desc
