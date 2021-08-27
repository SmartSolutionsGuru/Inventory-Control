INSERT INTO ChartOfAccount(AccountCategory,AccountSubCategory,AccountHeading,AccountNumber,[Description],IsActive,CreatedAt,CreatedBy)
					VALUES('Assets','Liquid Assets','Liquid Assets',1000,'Liquid Assets',1,GETDATE(),'admin'),
						('Assets','Liquid Assets','Cash In Hand',1010,'Cash In Hand',1,GETDATE(),'admin'),
						('Assets','Liquid Assets','Current Assets',1020,'Current Assets',1,GETDATE(),'admin'),
						('Assets','Liquid Assets','Bank',1030,'Bank',1,GETDATE(),'admin'),
						('Assets','Liquid Assets','Petty Cash',1040,'Petty Cash',1,GETDATE(),'admin'),
						('Assets','Liquid Assets','Accounts Receivable',1050,'Accounts Receivable Or DR',1,GETDATE(),'admin'),
						('Assets','Liquid Assets','Any Other Assets',1060,'Accounts Receivable Or DR',1,GETDATE(),'admin'),

						('Assets','Fixed Assts','Fixed Assets',1500,'Fixed Assets',1,GETDATE(),'admin'),
						('Assets','Fixed Assts','Office/Shop Equipments',1510,'Fixed Assets',1,GETDATE(),'admin'),
						('Assets','Fixed Assts','Motor Vehicale',1520,'Fixed Assets',1,GETDATE(),'admin'),

						('Liabalities','Liabalities','Liabalities',2000,'Liabalities',1,GETDATE(),'admin'),
						('Liabalities','Liabalities','Current Liabalities',2110,'Current Liabalities',1,GETDATE(),'admin'),
						('Liabalities','Liabalities','Credit Card',2120,'Credit Card Liabalities',1,GETDATE(),'admin'),
						('Liabalities','Liabalities','Accounts Payable',2130,'Accounts Payable Or Creditors',1,GETDATE(),'admin'),
						('Liabalities','Liabalities','Long term Liabalities',2200,'Long term Liabalities',1,GETDATE(),'admin'),
						('Liabalities','Liabalities','Bank Loan',2210,'Liabalities',1,GETDATE(),'admin'),
						('Liabalities','Liabalities','Vehical Loan',2220,'Liabalities',1,GETDATE(),'admin'),
						('Liabalities','Liabalities','Any Other',2250,'Liabalities',1,GETDATE(),'admin'),

						('Equity','Equity','Equity',3000,'Equity',1,GETDATE(),'admin'),
						('Equity','Capital','Capital',3110,'Equity',1,GETDATE(),'admin'),
						('Equity','Drawing','Drawing',3120,'Equity',1,GETDATE(),'admin'),

						('Revenew','Revenew','Revenew',5000,'Revenew',1,GETDATE(),'admin'),
						('Revenew','Sale','Sale',5310,'Sale',1,GETDATE(),'admin'),
						('Revenew','Sale Return','Sale Return',4210,'Sale Return',1,GETDATE(),'admin'),
						('Revenew','Labour','Labour',4300,'Labour',1,GETDATE(),'admin'),
						('Revenew','Materials','Materials',4400,'Materials',1,GETDATE(),'admin'),
						('Revenew','Bank Intrest','Bank Intrest',4530,'Bank Intrest',1,GETDATE(),'admin'),
						('Revenew','Commisions','Commisions',4550,'Commisions',1,GETDATE(),'admin'),
						('Revenew','Sale Discount','Sale Discount',4900,'Sale Discount',1,GETDATE(),'admin'),

						('Cost Of Goods Sold','Purchase','Purchase',5000,'Purchase',1,GETDATE(),'admin'),
						('Cost Of Goods Sold','Raw Material Purchase','Raw Material Purchase',5310,'Raw Material Purchase',1,GETDATE(),'admin'),
						('Cost Of Goods Sold','Commision','Commision',5510,'Commision',1,GETDATE(),'admin'),
						('Cost Of Goods Sold','Freight','Feright',5610,'Feright',1,GETDATE(),'admin'),
						('Cost Of Goods Sold','Purchase Return','Purchase Return',5710,'Purchase Return',1,GETDATE(),'admin'),
						('Cost Of Goods Sold','Purchase Discount','Purchase Discount',5810,'Purchase Discount',1,GETDATE(),'admin'),

						('Expenses','Expenses','Expenses',6000,'Expenses',1,GETDATE(),'admin'),
						('Expenses','Cost of Goods Sold','Cost of Goods Sold',6100,'Cost of Goods Sold',1,GETDATE(),'admin'),
						('Expenses','Advertising','Advertising',6110,'Advertising',1,GETDATE(),'admin'),
						('Expenses','Bank Fees','Bank Fees',6120,'Bank Fees',1,GETDATE(),'admin'),
						('Expenses','Genral Expenses','Genral Expenses',6130,'Genral Expenses',1,GETDATE(),'admin'),
						('Expenses','Internet','Internet',6140,'Internet',1,GETDATE(),'admin'),
						('Expenses','Stationery','Stationery',6150,'Stationery',1,GETDATE(),'admin'),
						('Expenses','Subscription','Subscription',6160,'Subscription',1,GETDATE(),'admin'),
						('Expenses','Telephone','Telephone',6170,'Telephone',1,GETDATE(),'admin'),
						('Expenses','Other Expenses','Other Expenses',6180,'Other Expenses',1,GETDATE(),'admin'),
						('Expenses','Personal Expenses','Personal Expenses',6190,'Personal Expenses',1,GETDATE(),'admin');



