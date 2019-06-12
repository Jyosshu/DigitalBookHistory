-- BEGIN TRANSACTION;


-- *** kind ***
INSERT INTO kind (id,name,enabled,singular,plural) VALUES (5,'EBOOK',0,'ebook','Ebooks');
INSERT INTO kind (id,name,enabled,singular,plural) VALUES (6,'MUSIC',1,'music','Music');
INSERT INTO kind (id,name,enabled,singular,plural) VALUES (7,'MOVIE',1,'movie','Movies');
INSERT INTO kind (id,name,enabled,singular,plural) VALUES (8,'AUDIOBOOK',0,'audiobook','Audibooks');
INSERT INTO kind (id,name,enabled,singular,plural) VALUES (9,'TELEVISION',1,'television','Television');
INSERT INTO kind (id,name,enabled,singular,plural) VALUES (10,'COMIC',1,'comic','Comics');

-- *** ratings ***
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (1,1,'G',1);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (2,1,'PG',3);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (3,1,'PG13',4);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (4,1,'R',5);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (5,1,'NC17',7);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (6,2,'TVY',1);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (7,2,'TVY7',2);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (8,2,'TVG',1);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (9,2,'TVPG',3);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (10,2,'TV14',5);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (11,2,'TVMA',6);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (20,1,'NR',3);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (21,1,'UNK',5);
INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES (22,1,'UR',6);
-- INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES ();
-- INSERT INTO ratings (id,ratingSystemId,rating,rank) VALUES ();



-- COMMIT;
