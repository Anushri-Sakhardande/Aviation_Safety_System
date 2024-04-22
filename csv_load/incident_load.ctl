LOAD DATA
INFILE 'C:\Users\anush\OneDrive\Documents\Aviation_Safety_System\new_incid_mod.csv'
INTO TABLE INCIDENT
FIELDS TERMINATED BY ',' optionally enclosed by '"'
(
accident_id,
admin_id,
accd_date DATE 'MM-DD-YYYY',
location,Fatalities,
registration_id,
cat,
Country,
aircraft_id
)
