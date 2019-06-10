BEGIN TRANSACTION;


-- Artist table
CREATE TABLE artist
(
    artistId INTEGER NULL,
    artistName VARCHAR(255) NULL
);

-- Media Kind table
CREATE TABLE kind
(
    id INTEGER PRIMARY KEY,
    name VARCHAR(255) NULL,
    enabled BOOLEAN NULL,
    singular VARCHAR(30) NULL,
    plural VARCHAR(30) NULL
);

-- Book table
CREATE TABLE digital_item
(
    id SERIAL PRIMARY KEY,
    titleId BIGINT NULL,
    title VARCHAR(255) NULL,
    kindId BIGINT NULL,
    artistId INTEGER NULL,
    --artistName VARCHAR(255) NULL,
    demo BOOLEAN NULL,
    pa BOOLEAN NULL,
    edited BOOLEAN NULL,    
    artKey VARCHAR(255) NULL,
    borrowed BIGINT NULL,
    returned BIGINT NULL,
    circId BIGINT NULL,
    fixedLayout BOOLEAN NULL,
    readAlong BOOLEAN NULL,
    CONSTRAINT fk_digital_item_media_kind FOREIGN KEY (kindId) REFERENCES media_kind(id),
    CONSTRAINT fk_digital_item_artist FOREIGN KEY (artistId) REFERENCES artist(artistId)
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
    id SERIAL PRIMARY KEY,
    altText VARCHAR(255) NULL,
    artKey VARCHAR(255) NULL,
    remoteUrl VARCHAR(255) NULL,
    localUrl VARCHAR(255) NULL,
    CONSTRAINT fk_image_digital_item FOREIGN KEY (artKey) REFERENCES digital_item(artKey)
);


-- ROLLBACK;


COMMIT;