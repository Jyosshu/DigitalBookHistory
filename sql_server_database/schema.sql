BEGIN TRANSACTION;


--ALTER TABLE digital_item DROP CONSTRAINT fk_digital_item_kind;
--ALTER TABLE images DROP CONSTRAINT fk_image_digital_item;

DROP TABLE IF EXISTS ratings, digital_item, images, kind, artist;

-- Artist table
CREATE TABLE artist
(
    artistId INTEGER PRIMARY KEY,
    artistName VARCHAR(255) UNIQUE	
);

-- Media Kind table
CREATE TABLE kind
(
    id INTEGER PRIMARY KEY,
    name VARCHAR(255) NULL,
    enabled BIT NULL,
    singular VARCHAR(30) NULL,
    plural VARCHAR(30) NULL
);

-- Book table
CREATE TABLE digital_item
(
    id INTEGER PRIMARY KEY,
    titleId BIGINT NULL,
    title VARCHAR(255) NULL,
    kindId INTEGER NULL,
    artistName VARCHAR(255) NULL,
    demo BIT NULL,
    pa BIT NULL,
    edited BIT NULL,    
    artKey VARCHAR(255) UNIQUE,
    borrowed BIGINT NULL,
    returned BIGINT NULL,
    circId BIGINT NULL,
    fixedLayout BIT NULL,
    readAlong BIT NULL,
    CONSTRAINT fk_digital_item_kind FOREIGN KEY (kindId) REFERENCES kind(id)
);

CREATE TABLE ratings
(
    id INTEGER PRIMARY KEY,
    ratingSystemId INTEGER NULL, -- probably a rantingSystemId, (movieRatings, televisionRatings, comicRatings)
    rating VARCHAR(10) NULL,
    rank INTEGER NULL
);

-- images
CREATE TABLE images
(
    id INTEGER IDENTITY,
    altText VARCHAR(255) NULL,
    artKey VARCHAR(255) NULL,
    remoteUrl VARCHAR(255) NULL,
    localUrl VARCHAR(255) NULL,
    CONSTRAINT fk_image_digital_item FOREIGN KEY (artKey) REFERENCES digital_item(artKey)
);


-- ROLLBACK;


-- COMMIT;