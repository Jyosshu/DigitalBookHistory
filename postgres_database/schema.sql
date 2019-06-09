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
    name VARCHAR(255),
    enable BOOLEAN,
    singular VARCHAR(30) NULL,
    plural VARCHAR(30) NULL
);

-- Book table
CREATE TABLE digital_item
(
    id SERIAL PRIMARY KEY,
    titleId INTEGER NULL,
    title VARCHAR(255) NULL,
    kindId INTEGER NULL,
    artistName VARCHAR(255) NULL,
    artKey VARCHAR(255) NULL,
    borrowed INTEGER NULL,
    returned INTEGER NULL,
    circId INTEGER NULL,
    CONSTRAINT fk_digital_item_media_kind FOREIGN KEY (kindId) REFERENCES media_kind(id)
);

CREATE TABLE ratings
(
    id INTEGER PRIMARY KEY,
    ratingSystemId INTEGER NULL, -- probably a rantingSystemId, (movieRatings, televisionRatings, comicRatings)
    rating VARCHAR(10) NULL,
    rank INTEGER NULL
);

-- ROLLBACK;


COMMIT;