declare @answers as dbo.AnswerType;

delete from @answers;
insert into @answers(Content, IsCorrect) values('Celestial objects',1);
insert into @answers(Content, IsCorrect) values('Periodic elements',0);
insert into @answers(Content, IsCorrect) values('Human history',0);
insert into @answers(Content, IsCorrect) values('Rock formations',0);
exec dbo.InsertQuestion 'Astronomy is the study of what?',2,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('TRUE',1);
insert into @answers(Content, IsCorrect) values('FALSE',0);
insert into @answers(Content, IsCorrect) values('',0);
insert into @answers(Content, IsCorrect) values('',0);
exec dbo.InsertQuestion 'Amateur astronomers play an active role in discovering new objects in the universe. True or false?',1,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Study of rock formations',0);
insert into @answers(Content, IsCorrect) values('Study of prophecies',0);
insert into @answers(Content, IsCorrect) values('Religion of peace',0);
insert into @answers(Content, IsCorrect) values('Law of the stars',1);
exec dbo.InsertQuestion 'The word ''astronomy'' means what?',2,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Sun is the center of the universe',0);
insert into @answers(Content, IsCorrect) values('Earth is the center of the universe',1);
insert into @answers(Content, IsCorrect) values('Earth is the center of the solar system',0);
insert into @answers(Content, IsCorrect) values('The universe is expanding',0);
exec dbo.InsertQuestion 'What is the geocentric model of the universe?',5,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Babylonian',1);
insert into @answers(Content, IsCorrect) values('Sumerian',0);
insert into @answers(Content, IsCorrect) values('Greek',0);
insert into @answers(Content, IsCorrect) values('Ottomans',0);
exec dbo.InsertQuestion 'Which civilization discovered the concept of the lunar eclipse?',5,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Sun is the center of the universe',0);
insert into @answers(Content, IsCorrect) values('Earth is the center of the universe',0);
insert into @answers(Content, IsCorrect) values('Earth is the center of the solar system',0);
insert into @answers(Content, IsCorrect) values('Sun is the center of the solar system',1);
exec dbo.InsertQuestion 'What is the heliocentric model of the solar system?',5,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Scalpal',0);
insert into @answers(Content, IsCorrect) values('Screwdriver',0);
insert into @answers(Content, IsCorrect) values('Telescope',1);
insert into @answers(Content, IsCorrect) values('Computer',0);
exec dbo.InsertQuestion 'Galileo is most famous for using what astronomical instrument?',3,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Iron and Carbon',0);
insert into @answers(Content, IsCorrect) values('Hydrogen and Helium',1);
insert into @answers(Content, IsCorrect) values('Oxygen and Nitrogen',0);
insert into @answers(Content, IsCorrect) values('Xenon and Radon',0);
exec dbo.InsertQuestion 'At the center of stars, the process of thermonuclear fusion takes place between which two elements?',4,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Nebula',1);
insert into @answers(Content, IsCorrect) values('Black hole',0);
insert into @answers(Content, IsCorrect) values('Galaxy',0);
insert into @answers(Content, IsCorrect) values('Comet tail',0);
exec dbo.InsertQuestion 'In what celestial object are stars born?',3,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('White dwarf',1);
insert into @answers(Content, IsCorrect) values('Red giant',0);
exec dbo.InsertQuestion 'Which type of star is older, a white dwarf or a red giant?',4,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Group',0);
insert into @answers(Content, IsCorrect) values('Collection',0);
insert into @answers(Content, IsCorrect) values('Constellation',1);
insert into @answers(Content, IsCorrect) values('Party',0);
exec dbo.InsertQuestion 'A formation of stars in the observable sky from Earth are often called what?',3,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Alpha Centauri',1);
insert into @answers(Content, IsCorrect) values('Sirius',0);
insert into @answers(Content, IsCorrect) values('Betelgeuse',0);
insert into @answers(Content, IsCorrect) values('Aldeberon',0);
exec dbo.InsertQuestion 'Which star system is the closest to Earth?',5,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('TRUE',1);
insert into @answers(Content, IsCorrect) values('FALSE',0);
exec dbo.InsertQuestion 'Earth is the third planet from the Sun. True or false?',1,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('225',0);
insert into @answers(Content, IsCorrect) values('550',0);
insert into @answers(Content, IsCorrect) values('365',1);
insert into @answers(Content, IsCorrect) values('741',0);
exec dbo.InsertQuestion 'Approximately how many times per year does the Earth rotate on its own axis?',1,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('1',1);
insert into @answers(Content, IsCorrect) values('2',0);
insert into @answers(Content, IsCorrect) values('3',0);
insert into @answers(Content, IsCorrect) values('4',0);
exec dbo.InsertQuestion 'How many times per year does the Earth revolve around the Sun?',1,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('The moon''s gravity',1);
insert into @answers(Content, IsCorrect) values('The Earth''s magnetic field',0);
insert into @answers(Content, IsCorrect) values('Magic',0);
insert into @answers(Content, IsCorrect) values('Tectonic plate shifting',0);
exec dbo.InsertQuestion 'What causes the Earth''s tides?',2,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('30%',0);
insert into @answers(Content, IsCorrect) values('99%',1);
insert into @answers(Content, IsCorrect) values('95%',0);
insert into @answers(Content, IsCorrect) values('75%',0);
exec dbo.InsertQuestion 'It is estimated that what percent of all life on Earth has gone extinct?',4,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Rocks',1);
insert into @answers(Content, IsCorrect) values('Oceans',0);
insert into @answers(Content, IsCorrect) values('Insects',0);
insert into @answers(Content, IsCorrect) values('Lions',0);
exec dbo.InsertQuestion 'Geology is the study of what?',1,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Inner core',0);
insert into @answers(Content, IsCorrect) values('Outer core',0);
insert into @answers(Content, IsCorrect) values('Upper mantle',0);
insert into @answers(Content, IsCorrect) values('Crust',1);
exec dbo.InsertQuestion 'Which layer is the outermost layer of the Earth?',2,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Hurricanes',0);
insert into @answers(Content, IsCorrect) values('Tornados',0);
insert into @answers(Content, IsCorrect) values('Earthquakes',0);
insert into @answers(Content, IsCorrect) values('Volcanos',1);
exec dbo.InsertQuestion 'Islands are formed as the result of what natural action?',2,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Magnets',0);
insert into @answers(Content, IsCorrect) values('Tectonic plates',1);
insert into @answers(Content, IsCorrect) values('Clouds',0);
insert into @answers(Content, IsCorrect) values('People',0);
exec dbo.InsertQuestion 'Earthquakes are caused by the shifting of what?',2,1, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('TRUE',1);
insert into @answers(Content, IsCorrect) values('FALSE',0);
exec dbo.InsertQuestion 'Geology is important to the study of the history of the Earth. True or false?',1,1, @answers;

-- 8/14/2015

declare @answers as dbo.AnswerType;

delete from @answers;
insert into @answers(Content, IsCorrect) values('Karl Landsteiner',1);
insert into @answers(Content, IsCorrect) values('Wilhelm Wundt',0);
insert into @answers(Content, IsCorrect) values('Gustav Fechner',0);
insert into @answers(Content, IsCorrect) values('Ernst Weber',0);
exec dbo.InsertQuestion 'Who discovered the concept of blood types?',4,1, @answers;
