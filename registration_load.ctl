LOAD DATA
INFILE 'C:\Users\anush\OneDrive\Documents\Aviation_Safety_System\new_reg_mod.csv'
INTO TABLE REGISTRATION
FIELDS TERMINATED BY ',' optionally enclosed by '"'
(
		registration,
		admin_id,
		aircraft_id,
		operator,
		registration_id    
)
