select * from dbo.questions q
left join dbo.tagquestions tq on tq.question_id = q.id
where tq.tag_tagid is null