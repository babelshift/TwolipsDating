--drop procedure dbo.InsertQuestion
create procedure dbo.InsertQuestion
@content varchar(255),
@answers dbo.AnswerType readonly
as
	declare @latestQuestionId int
	declare @correctAnswerContent varchar(255)
	declare @correctAnswerId int
	
	-- insert the question
	insert into dbo.Questions(Content, Points, QuestionTypeId)
	values(@content, 3, 1);

	-- get the question's id
	select @latestQuestionId = SCOPE_IDENTITY();

	-- insert the answers
	insert into dbo.Answers(Content, QuestionId)
	select Content, @latestQuestionId
	from @answers;

	-- get the correct answer content (this will fail if there is more than one correct answer
	select @correctAnswerContent = Content
	from @answers
	where IsCorrect = 1;

	-- get the correct answer id based on this question id and the correct answer content
	select @correctAnswerId = Id
	from dbo.Answers
	where Content = @correctAnswerContent
	and QuestionId = @latestQuestionId;

	update dbo.Questions
	set CorrectAnswerId = @correctAnswerId
	where Id = @latestQuestionId;
go

