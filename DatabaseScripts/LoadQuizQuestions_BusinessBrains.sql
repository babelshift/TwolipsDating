declare @answers as dbo.AnswerType;

insert into @answers(Content, IsCorrect) values('Last level cache', 0);
insert into @answers(Content, IsCorrect) values('Logical link control', 0);
insert into @answers(Content, IsCorrect) values('Limited liability company', 1);
insert into @answers(Content, IsCorrect) values('Landlocked country', 0);
exec dbo.InsertQuizQuestion 'What does LLC stand for in business terms?', 2, 4, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Stock exchange', 1);
insert into @answers(Content, IsCorrect) values('Stock tradership', 0);
insert into @answers(Content, IsCorrect) values('Auction house', 0);
insert into @answers(Content, IsCorrect) values('Market place', 0);
exec dbo.InsertQuizQuestion 'Stock brokers and traders can buy or sell stocks on what?', 2, 4, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('A specific person or enterprise with full control over supply of a commodity', 1);
insert into @answers(Content, IsCorrect) values('An extremely rich city or village', 0);
insert into @answers(Content, IsCorrect) values('An upset CEO', 0);
insert into @answers(Content, IsCorrect) values('A method by which investors can increase their investment returns', 0);
exec dbo.InsertQuizQuestion 'Aside from being a popular board game, what is a monopoly?', 3, 4, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Microsoft', 0);
insert into @answers(Content, IsCorrect) values('Lumber Liquidators', 0);
insert into @answers(Content, IsCorrect) values('Caterpillar', 0);
insert into @answers(Content, IsCorrect) values('Enron', 1);
exec dbo.InsertQuizQuestion 'Which company was found to be involved in systemic accounting fraud in 2001?', 3, 4, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('A tax on imports or exports', 1);
insert into @answers(Content, IsCorrect) values('A tax on local sales', 0);
insert into @answers(Content, IsCorrect) values('A term to describe a successful business', 0);
insert into @answers(Content, IsCorrect) values('A technique by which investors can swap shares', 0);
exec dbo.InsertQuizQuestion 'What is a tariff?', 4, 4, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Throwing three strikes in a row in baseball', 0);
insert into @answers(Content, IsCorrect) values('Ending a strike by paying workers higher wages', 1);
insert into @answers(Content, IsCorrect) values('Forceful replacement of striking workers', 0);
insert into @answers(Content, IsCorrect) values('Locking out workers from a business', 0);
exec dbo.InsertQuizQuestion 'The controversial act of strikebreaking involves what?', 4, 4, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Direct deposit', 1);
insert into @answers(Content, IsCorrect) values('Direct payment', 0);
insert into @answers(Content, IsCorrect) values('Direct wages', 0);
insert into @answers(Content, IsCorrect) values('Direct transaction', 0);
exec dbo.InsertQuizQuestion 'Which term is used when a worker receives salary payments directly to a bank account?', 2, 4, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Accounting', 0);
insert into @answers(Content, IsCorrect) values('Engineering', 0);
insert into @answers(Content, IsCorrect) values('Human resources', 1);
insert into @answers(Content, IsCorrect) values('Executives', 0);
exec dbo.InsertQuizQuestion 'Which business department is most commonly in charge of handling worker disputes?', 1, 4, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Industrial revolution', 0);
insert into @answers(Content, IsCorrect) values('Civil war', 0);
insert into @answers(Content, IsCorrect) values('Gilded age', 1);
insert into @answers(Content, IsCorrect) values('Civil rights', 0);
exec dbo.InsertQuizQuestion 'In United States history, the 1870s to 1900 saw both a large economic and poverty increase. What was this period called?', 3, 4, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Chief Executive Officer', 1);
insert into @answers(Content, IsCorrect) values('Chief Efficiency Officer', 0);
insert into @answers(Content, IsCorrect) values('Counter Effects Online', 1);
insert into @answers(Content, IsCorrect) values('Cross Element Order', 0);
exec dbo.InsertQuizQuestion 'What does CEO stand for?', 1, 4, @answers;