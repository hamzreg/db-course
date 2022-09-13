CREATE OR REPLACE PROCEDURE change_customer_bonus_card
(
    customer_id INTEGER, 
    bonus_card_id INTEGER
)
LANGUAGE plpgsql AS
$$
BEGIN
    UPDATE "Customers"
    SET "BonusCardID" = bonus_card_id
    WHERE "ID" = customer_id;
END
$$;