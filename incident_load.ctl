LOAD DATA
	INFILE 'C:\Users\anush\OneDrive\Documents\Aviation_Safety_System\incident.csv'
	INTO TABLE INCIDENT
	FIELDS TERMINATED BY ',' optionally enclosed by '"'
	(
		admin_id,accident_id,accd_date DATE 'MM-DD-YYYY',location,operator_id,Fatalities,registration_id,cat,Country,aircraft_id

	)
