select 
q.Id,
q.Content,
q.Points,
qt.Name
from dbo.Questions as q
inner join dbo.QuestionTypes as qt on qt.Id = q.QuestionTypeId
