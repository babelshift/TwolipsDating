update dbo.questions set correctanswerid = null
where id in (50,51,52,53,54,55,56,57,58,59)
delete from dbo.answers where questionid in (50,51,52,53,54,55,56,57,58,59)
delete from dbo.quizquestions where question_id in (50,51,52,53,54,55,56,57,58,59)
delete from dbo.questions where id in (50,51,52,53,54,55,56,57,58,59)