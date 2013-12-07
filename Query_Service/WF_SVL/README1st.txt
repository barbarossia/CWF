Before running this solution, you have to ensure that you have SQL Server 2008 installed and that the workflow 
persistance sql scripts have been run.

1) Install SQL Server 2008
2) In Microsoft SQL Server Management Studio
	a) Create a database for persisting Workflows (i.e. Persistance)
	b) Select Option to use the Database
	c) Select option to create a new query
	d) Select File->Open->File from the main menu
		browse to c:\Windows\Microsoft.Net\Framework\v4.xxxx\SQL\EN\SqlWorkflowInstanceStoreSchema
	e) Run Script
	f) Select option to create a new query
	g) Select File->Open->File from the main menu
		browse to c:\Windows\Microsoft.Net\Framework\v4.xxxx\SQL\EN\SqlWorkflowInstanceStoreLogic
	h) Run Script
3) In Visual Studio
	a) Open the web.config file in the WF_SVL project
	b) Change DataSource= to your machine name (i.e. v-evsmit1)
	c) Change Catalog= to the database name you created above (i.e. Persistance)
	d) Build Solution
	e) Start WF_SFL in debug mode
	f) Start ConsoleAppXboxLiveMultiStep in debug mode