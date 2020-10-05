 BEGIN TRANSACTION;


-- *** Kind ***
INSERT INTO Kind (Id,Name,Enabled,Singular,Plural) VALUES (5,'EBOOK',0,'ebook','Ebooks');
INSERT INTO Kind (Id,Name,Enabled,Singular,Plural) VALUES (6,'MUSIC',1,'music','Music');
INSERT INTO Kind (Id,Name,Enabled,Singular,Plural) VALUES (7,'MOVIE',1,'movie','Movies');
INSERT INTO Kind (Id,Name,Enabled,Singular,Plural) VALUES (8,'AUDIOBOOK',0,'audiobook','Audibooks');
INSERT INTO Kind (Id,Name,Enabled,Singular,Plural) VALUES (9,'TELEVISION',1,'television','Television');
INSERT INTO Kind (Id,Name,Enabled,Singular,Plural) VALUES (10,'COMIC',1,'comic','Comics');

-- *** Ratings ***
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (1,1,'G',1);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (2,1,'PG',3);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (3,1,'PG13',4);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (4,1,'R',5);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (5,1,'NC17',7);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (6,2,'TVY',1);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (7,2,'TVY7',2);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (8,2,'TVG',1);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (9,2,'TVPG',3);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (10,2,'TV14',5);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (11,2,'TVMA',6);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (20,1,'NR',3);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (21,1,'UNK',5);
INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES (22,1,'UR',6);
-- INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES ();
-- INSERT INTO Ratings (Id,RatingSystemId,Rating,Rank) VALUES ();



-- COMMIT;
