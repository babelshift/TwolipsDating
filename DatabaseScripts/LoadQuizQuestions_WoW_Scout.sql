declare @answers as dbo.AnswerType;

insert into @answers(Content, IsCorrect) values('MMO', 1);
insert into @answers(Content, IsCorrect) values('FPS', 0);
insert into @answers(Content, IsCorrect) values('MOBA', 0);
insert into @answers(Content, IsCorrect) values('Strategy', 0);
exec dbo.InsertQuizQuestion 'To what game genre does World of Warcraft belong?', 1, 6, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Magic', 0);
insert into @answers(Content, IsCorrect) values('Ranged', 0);
insert into @answers(Content, IsCorrect) values('Melee', 1);
insert into @answers(Content, IsCorrect) values('Flight', 0);
exec dbo.InsertQuizQuestion 'What type of combat does a Warrior engage in?', 1, 6, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Mists of Pandaria', 0);
insert into @answers(Content, IsCorrect) values('The Burning Crusade', 1);
insert into @answers(Content, IsCorrect) values('Cataclysm', 0);
insert into @answers(Content, IsCorrect) values('Wrath of the Lich King', 0);
exec dbo.InsertQuizQuestion 'What is the name of the first World of Warcraft expansion?', 2, 6, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Quest giver', 1);
insert into @answers(Content, IsCorrect) values('Hostile target', 0);
insert into @answers(Content, IsCorrect) values('Friend', 0);
insert into @answers(Content, IsCorrect) values('Waiting for invite', 0);
exec dbo.InsertQuizQuestion 'What does an exclamation mark over a person''s head indicate?', 1, 6, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Paladin', 0);
insert into @answers(Content, IsCorrect) values('Mage', 0);
insert into @answers(Content, IsCorrect) values('Priest', 1);
insert into @answers(Content, IsCorrect) values('Rogue', 0);
exec dbo.InsertQuizQuestion 'Which class uses holy powers to heal and shadow powers to deal damage?', 1, 6, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Infinity Ward', 0);
insert into @answers(Content, IsCorrect) values('Valve Software', 0);
insert into @answers(Content, IsCorrect) values('Interplay', 0);
insert into @answers(Content, IsCorrect) values('Blizzard Entertainment', 1);
exec dbo.InsertQuizQuestion 'Which company created World of Warcraft?', 1, 6, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('TRUE', 1);
insert into @answers(Content, IsCorrect) values('FALSE', 0);
exec dbo.InsertQuizQuestion 'An internet connection is required to play. True or false?', 1, 6, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('2', 0);
insert into @answers(Content, IsCorrect) values('3', 0);
insert into @answers(Content, IsCorrect) values('4', 0);
insert into @answers(Content, IsCorrect) values('5', 1);
exec dbo.InsertQuizQuestion 'What is the maximum party size?', 1, 6, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('2', 0);
insert into @answers(Content, IsCorrect) values('4', 0);
insert into @answers(Content, IsCorrect) values('6', 0);
insert into @answers(Content, IsCorrect) values('8', 1);
exec dbo.InsertQuizQuestion 'How many races were included in the original game?', 2, 6, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('3', 0);
insert into @answers(Content, IsCorrect) values('6', 0);
insert into @answers(Content, IsCorrect) values('9', 1);
insert into @answers(Content, IsCorrect) values('12', 0);
exec dbo.InsertQuizQuestion 'How many classes were included in the original game?', 2, 6, @answers;