update dbo.questions set correctanswerid = null;
delete from dbo.AnsweredQuestions;
delete from dbo.answers;
delete from dbo.quizquestions;
delete from dbo.TagQuestions;
delete from dbo.QuestionViolations;
delete from dbo.questions;