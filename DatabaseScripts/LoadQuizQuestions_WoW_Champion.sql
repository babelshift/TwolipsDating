declare @answers as dbo.AnswerType;

insert into @answers(Content, IsCorrect) values('Kalimdor', 0);
insert into @answers(Content, IsCorrect) values('Outland', 0);
insert into @answers(Content, IsCorrect) values('Twisted Nether', 0);
insert into @answers(Content, IsCorrect) values('Azeroth', 1);
exec dbo.InsertQuizQuestion 'What is the name of the main planet in the World of Warcraft universe?', 2, 7, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Kalimdor and Eastern Kingdoms', 1);
insert into @answers(Content, IsCorrect) values('Outland and Twisted Nether', 0);
insert into @answers(Content, IsCorrect) values('Tanaris and Silithus', 0);
insert into @answers(Content, IsCorrect) values('Draenor and Shattered Halls', 0);
exec dbo.InsertQuizQuestion 'What are the two main continents in the original game?', 2, 7, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Undercity', 0);
insert into @answers(Content, IsCorrect) values('Orgrimmar', 1);
insert into @answers(Content, IsCorrect) values('Thunder Bluff', 0);
insert into @answers(Content, IsCorrect) values('Gnomeregan', 0);
exec dbo.InsertQuizQuestion 'What is the name of the Orc city?', 2, 7, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('60', 1);
insert into @answers(Content, IsCorrect) values('70', 0);
insert into @answers(Content, IsCorrect) values('80', 0);
insert into @answers(Content, IsCorrect) values('90', 0);
exec dbo.InsertQuizQuestion 'What is the maximum level a character can reach in the original game?', 2, 7, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('45', 0);
insert into @answers(Content, IsCorrect) values('55', 1);
insert into @answers(Content, IsCorrect) values('65', 0);
insert into @answers(Content, IsCorrect) values('75', 0);
exec dbo.InsertQuizQuestion 'Death Knights start at what level when created?', 2, 7, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('The Burning Crusade', 0);
insert into @answers(Content, IsCorrect) values('Cataclysm', 0);
insert into @answers(Content, IsCorrect) values('Mists of Pandaria', 1);
insert into @answers(Content, IsCorrect) values('Warlords of Draenor', 0);
exec dbo.InsertQuizQuestion 'Monks were added in which expansion?', 2, 7, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Crossroads', 0)
insert into @answers(Content, IsCorrect) values('Westboro', 0);
insert into @answers(Content, IsCorrect) values('Goldshire', 1);
insert into @answers(Content, IsCorrect) values('Zangarmarsh', 0);
exec dbo.InsertQuizQuestion 'What is the name of the Human starting area?', 3, 7, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Deathwing', 0);
insert into @answers(Content, IsCorrect) values('Onyxia', 1);
insert into @answers(Content, IsCorrect) values('Azuregos', 0);
insert into @answers(Content, IsCorrect) values('Lord Kazzak', 0);
exec dbo.InsertQuizQuestion 'What was the name of the first dragon raid boss in the original game?', 3, 7, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Blackwing Lair', 0);
insert into @answers(Content, IsCorrect) values('Tempest Keep', 0);
insert into @answers(Content, IsCorrect) values('Molten Core', 1);
insert into @answers(Content, IsCorrect) values('Blackrock Depths', 0);
exec dbo.InsertQuizQuestion 'What was the name of the first full raid zone?', 3, 7, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Thrall', 0);
insert into @answers(Content, IsCorrect) values('Arthas', 0);
insert into @answers(Content, IsCorrect) values('Sylvanas', 1);
insert into @answers(Content, IsCorrect) values('Gorefiend', 0);
exec dbo.InsertQuizQuestion 'Who is the Forsaken''s leader?', 3, 7, @answers;