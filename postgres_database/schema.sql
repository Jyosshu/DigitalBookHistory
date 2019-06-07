BEGIN TRANSACTION;


-- Artist table
CREATE TABLE artist
(
    artistId INTEGER NULL,
    artistName VARCHAR(255) NULL
);

-- Media Kind table
CREATE TABLE media_kind
(
    --id SERIAL PRIMARY KEY,
    kindId INTEGER PRIMARY KEY,
    kind VARCHAR(255)
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
    CONSTRAINT fk_digital_item_media_kind FOREIGN KEY (kindId) REFERENCES media_kind(kindId)
);



-- ROLLBACK;


COMMIT;