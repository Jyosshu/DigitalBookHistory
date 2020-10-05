BEGIN TRANSACTION;


--ALTER TABLE digital_item DROP CONSTRAINT fk_digital_item_kind;
--ALTER TABLE images DROP CONSTRAINT fk_image_digital_item;
--DROP TABLE IF EXISTS ratings, borrows, images, artist, digital_item, kind;
DROP TABLE IF EXISTS Ratings, Borrows, Images, Artist, DigitalItem, Kind;

-- Artist table
CREATE TABLE Artist
(
    ArtistId INTEGER IDENTITY,
    ArtistName VARCHAR(255) UNIQUE	
);

-- Media Kind table
CREATE TABLE Kind
(
    Id INTEGER PRIMARY KEY,
    Name VARCHAR(255) NULL,
    Enabled BIT NULL,
    Singular VARCHAR(30) NULL,
    Plural VARCHAR(30) NULL
);

-- Book table
CREATE TABLE DigitalItem
(
    Id INTEGER IDENTITY,
    TitleId BIGINT NULL,
    Title VARCHAR(255) NULL,
    KindId INTEGER NULL,
    ArtistName VARCHAR(255) NULL,
    Demo BIT NULL,
    Pa BIT NULL,
    Edited BIT NULL,    
    ArtKey VARCHAR(255) UNIQUE,
    CircId BIGINT NULL,
    FixedLayout BIT NULL,
    ReadAlong BIT NULL,
    CONSTRAINT fk_digital_item_kind FOREIGN KEY (KindId) REFERENCES Kind(Id)
);

CREATE TABLE Borrows
(
    Id INTEGER IDENTITY,
    TitleId BIGINT NULL,
    Borrowed BIGINT NULL,
    Returned BIGINT NULL,
);

CREATE TABLE Ratings
(
    Id INTEGER PRIMARY KEY,
    RatingSystemId INTEGER NULL, -- probably a rantingSystemId, (movieRatings, televisionRatings, comicRatings)
    Rating VARCHAR(10) NULL,
    Rank INTEGER NULL
);

-- images
CREATE TABLE Images
(
    Id INTEGER IDENTITY,
    AltText VARCHAR(255) NULL,
    ArtKey VARCHAR(255) NULL,
    RemoteUrl VARCHAR(255) NULL,
    CONSTRAINT fk_image_digital_item FOREIGN KEY (ArtKey) REFERENCES DigitalItem(ArtKey)
);


-- ROLLBACK;


-- COMMIT;