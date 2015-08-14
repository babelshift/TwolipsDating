declare @answers as dbo.AnswerType;

insert into @answers(Content, IsCorrect) values('1948', 0);
insert into @answers(Content, IsCorrect) values('1954', 1);
insert into @answers(Content, IsCorrect) values('1961', 0);
insert into @answers(Content, IsCorrect) values('1970', 0);
exec dbo.InsertQuizQuestion 'What year was the first Godzilla movie released?', 2, 9, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('A feeling of exploiting World War II devastation', 1);
insert into @answers(Content, IsCorrect) values('Cheap effects and poor acting', 0);
insert into @answers(Content, IsCorrect) values('Non-sensical plot', 0);
insert into @answers(Content, IsCorrect) values('Monster was too scary', 0);
exec dbo.InsertQuizQuestion 'Why did the original Godzilla movie receive mixed and negative reviews in Japan?', 4, 9, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Stick motion', 0);
insert into @answers(Content, IsCorrect) values('Clay rolling', 0);
insert into @answers(Content, IsCorrect) values('Egg and barrel', 0);
insert into @answers(Content, IsCorrect) values('Stop motion', 1);
exec dbo.InsertQuizQuestion 'What is the name of a technique used to make objects appear to move on their own?', 3, 9, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Swamp creature', 0);
insert into @answers(Content, IsCorrect) values('Vampire', 1);
insert into @answers(Content, IsCorrect) values('Ghost', 0);
insert into @answers(Content, IsCorrect) values('Dragon', 0);
exec dbo.InsertQuizQuestion 'Nosferatu is a film about what type of mythical creature?', 2, 9, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Spear of Destiny', 0);
insert into @answers(Content, IsCorrect) values('Golden Fleece', 1);
insert into @answers(Content, IsCorrect) values('Flaming Torch', 0);
insert into @answers(Content, IsCorrect) values('Leaves of Wisdom', 0);
exec dbo.InsertQuizQuestion 'In Jason and the Argonauts, what object was Jason searching for?', 5, 9, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('2001: A Space Odyssey', 0);
insert into @answers(Content, IsCorrect) values('Moon', 0);
insert into @answers(Content, IsCorrect) values('Gravity', 0);
insert into @answers(Content, IsCorrect) values('Alien', 1);
exec dbo.InsertQuizQuestion 'The tagline, "In space, no one can hear you scream" belong to which movie?', 4, 9, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('It wants to kill her', 0);
insert into @answers(Content, IsCorrect) values('It wants to learn more about her', 0);
insert into @answers(Content, IsCorrect) values('It fell in love with her', 1);
insert into @answers(Content, IsCorrect) values('It was tired of being bullied', 0);
exec dbo.InsertQuizQuestion 'Why does the creature in "The Creature from the Black Lagoon" capture a woman?', 4, 9, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('2', 0);
insert into @answers(Content, IsCorrect) values('3', 0);
insert into @answers(Content, IsCorrect) values('4', 0);
insert into @answers(Content, IsCorrect) values('5', 1);
exec dbo.InsertQuizQuestion 'What is the maximum party size?', 1, 9, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('2', 0);
insert into @answers(Content, IsCorrect) values('4', 0);
insert into @answers(Content, IsCorrect) values('6', 0);
insert into @answers(Content, IsCorrect) values('8', 1);
exec dbo.InsertQuizQuestion 'How many races were included in the original game?', 2, 9, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('3', 0);
insert into @answers(Content, IsCorrect) values('6', 0);
insert into @answers(Content, IsCorrect) values('9', 1);
insert into @answers(Content, IsCorrect) values('12', 0);
exec dbo.InsertQuizQuestion 'How many classes were included in the original game?', 2, 9, @answers;