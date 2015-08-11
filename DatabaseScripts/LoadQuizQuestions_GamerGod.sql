declare @answers as dbo.AnswerType;

insert into @answers(Content, IsCorrect) values('MMO', 0);
insert into @answers(Content, IsCorrect) values('FPS', 0);
insert into @answers(Content, IsCorrect) values('MOBA', 1);
insert into @answers(Content, IsCorrect) values('Adventure', 0);
exec dbo.InsertQuizQuestion 'To which genre do the games Dota 2 and League of Legends belong?', 4, 2, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Druid', 0);
insert into @answers(Content, IsCorrect) values('Rogue', 0);
insert into @answers(Content, IsCorrect) values('Hunter', 1);
insert into @answers(Content, IsCorrect) values('Warrior', 0);
exec dbo.InsertQuizQuestion 'In World of Warcraft, which class allows the player to tame animals?', 3, 2, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Xbox One', 0);
insert into @answers(Content, IsCorrect) values('Xbox 360', 1);
insert into @answers(Content, IsCorrect) values('Playstation', 0);
insert into @answers(Content, IsCorrect) values('Virtual Boy', 0);
exec dbo.InsertQuizQuestion 'Which console did Microsoft release after the original Xbox?', 2, 2, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Super Nintendo', 0);
insert into @answers(Content, IsCorrect) values('Wii', 1);
insert into @answers(Content, IsCorrect) values('Wii U', 0);
insert into @answers(Content, IsCorrect) values('Virtual Boy', 0);
exec dbo.InsertQuizQuestion 'Which console did Nintendo release after the Gamecube?', 2, 2, @answers;