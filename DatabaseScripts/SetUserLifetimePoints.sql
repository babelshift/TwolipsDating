update dbo.AspNetUsers
set LifetimePoints = isnull((
		select
		sum(stl.PointPrice * stl.ItemCount) as LifetimePoints
		from dbo.StoreTransactionLogs stl
		where stl.UserId = dbo.AspNetUsers.id
		group by UserId
), 0)

update dbo.AspNetUsers
set LifetimePoints = LifetimePoints + CurrentPoints