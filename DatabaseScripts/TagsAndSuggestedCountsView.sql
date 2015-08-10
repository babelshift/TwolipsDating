CREATE VIEW [dbo].[TagsAndSuggestedCountsView] AS 
	select 
		ts.tagid,
		t.name,
		t.description,
		count(ts.tagid) as SuggestedCount
	from 
		dbo.tags as t
		left join dbo.tagsuggestions as ts on ts.tagid = t.tagid
		left join dbo.aspnetusers as u on u.id = ts.suggestinguserid
		left join dbo.profiles as p on p.id = ts.profileid
		left join dbo.aspnetusers u2 on u2.id = p.applicationuser_id
	where 
		(u.isactive = 1 or u.isactive is null)
		and (u2.isactive = 1 or u2.isactive is null)
	group by 
		t.name, t.description, ts.tagid
GO


