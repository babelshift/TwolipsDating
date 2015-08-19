declare @answers as dbo.AnswerType;

insert into @answers(Content, IsCorrect) values('Red, Green, and Blue', 0);
insert into @answers(Content, IsCorrect) values('Red, Yellow, and Blue', 1);
insert into @answers(Content, IsCorrect) values('Green, Yellow, and Blue', 0);
insert into @answers(Content, IsCorrect) values('Black, White, and Orange', 0);
exec dbo.InsertQuizQuestion 'In traditional color theory, what are the three primary colors?', 2, 3, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Ballet', 1);
insert into @answers(Content, IsCorrect) values('Modern', 0);
insert into @answers(Content, IsCorrect) values('Square', 0);
insert into @answers(Content, IsCorrect) values('Ballroom', 0);
exec dbo.InsertQuizQuestion 'A tutu is traditionally worn during which type of dance performance?', 1, 3, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Drawing', 0);
insert into @answers(Content, IsCorrect) values('Photography', 0);
insert into @answers(Content, IsCorrect) values('Sculpting', 1);
insert into @answers(Content, IsCorrect) values('Architecture', 0);
exec dbo.InsertQuizQuestion 'Michelangelo''s David belongs to which branch of visual arts?', 2, 3, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Dutch Republic', 1);
insert into @answers(Content, IsCorrect) values('Kingdom of France', 0);
insert into @answers(Content, IsCorrect) values('Kingdom of Denmark', 0);
insert into @answers(Content, IsCorrect) values('Venice', 0);
exec dbo.InsertQuizQuestion 'In what country was Rembrandt van Rijn born?', 5, 3, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Baroque', 0);
insert into @answers(Content, IsCorrect) values('Expressionism', 0);
insert into @answers(Content, IsCorrect) values('Cubism', 0);
insert into @answers(Content, IsCorrect) values('Impressionism', 1);
exec dbo.InsertQuizQuestion 'Oscar-Claude Monet was a founder of which movement?', 5, 3, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Bass and Boss', 0);
insert into @answers(Content, IsCorrect) values('Bass and Treble', 1);
insert into @answers(Content, IsCorrect) values('Treble and Vibe', 0);
insert into @answers(Content, IsCorrect) values('Pitch and Beat', 0);
exec dbo.InsertQuizQuestion 'What are the names of the two clefs in modern music staff notation?', 2, 3, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Brass', 0);
insert into @answers(Content, IsCorrect) values('String', 0);
insert into @answers(Content, IsCorrect) values('Woodwind', 0);
insert into @answers(Content, IsCorrect) values('Steel', 1);
exec dbo.InsertQuizQuestion 'Which of the following is not recognized as a common family of instruments?', 2, 3, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Troop', 0);
insert into @answers(Content, IsCorrect) values('Band', 0);
insert into @answers(Content, IsCorrect) values('Chorus', 1);
insert into @answers(Content, IsCorrect) values('Team', 0);
exec dbo.InsertQuizQuestion 'A body of singers who perform together as a group is called what?', 3, 3, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Kiln', 0);
insert into @answers(Content, IsCorrect) values('Ceramic glaze', 1);
insert into @answers(Content, IsCorrect) values('Terracotta', 0);
insert into @answers(Content, IsCorrect) values('Burnishing', 0);
exec dbo.InsertQuizQuestion 'What is a popular layer of protection as well as decoration given to ceramic bodies?', 5, 3, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Romania', 0);
insert into @answers(Content, IsCorrect) values('Bulgaria', 0);
insert into @answers(Content, IsCorrect) values('Czech Republic', 1);
insert into @answers(Content, IsCorrect) values('Hungary', 0);
exec dbo.InsertQuizQuestion 'The earliest known ceramic objects were discovered in what modern day country?', 4, 3, @answers;