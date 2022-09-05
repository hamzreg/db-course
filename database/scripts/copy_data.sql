\copy "BonusCards"("Bonuses", "Phone")
from 'C:\Users\regin\bmstu\db-course\database\data\bonusCards.csv'
delimiter '|' csv;

\copy "Customers"("Name", "Surname", "BonusCardID") from 'C:\Users\regin\bmstu\db-course\database\data\customers.csv' delimiter '|' csv;

\copy "Purchases"("Price", "Status", "CustomerID")
from 'C:\Users\regin\bmstu\db-course\database\data\purchases.csv'
delimiter '|' csv;

\copy "Sales"("PurchasePrice", "SellingPrice", "Margin", "Costs", "Profit", "WineNumber", "Date", "PurchaseID", "SupplierWineID") 
from 'C:\Users\regin\bmstu\db-course\database\data\sales.csv' 
delimiter '|' csv;

\copy "SupplierWines"("SupplierID", "WineID", "Price", "Percent", "Rating") 
from 'C:\Users\regin\bmstu\db-course\database\data\supplierWines.csv' 
delimiter '|' csv;

\copy "Suppliers"("Name", "Country", "Experience", "License") 
from 'C:\Users\regin\bmstu\db-course\database\data\suppliers.csv' 
delimiter '|' csv;

\copy "Users"("RoleID", "Login", "Password", "Role") from 'C:\Users\regin\bmstu\db-course\database\data\users.csv' delimiter '|' csv;

\copy "Wines"("Kind", "Color", "Sugar", "Volume", "Alcohol", "Aging", "Number") 
from 'C:\Users\regin\bmstu\db-course\database\data\wines.csv' 
delimiter '|' csv;