create type dbo.AnswerType as table
(
	Content varchar(255),
	IsCorrect bit
);
go