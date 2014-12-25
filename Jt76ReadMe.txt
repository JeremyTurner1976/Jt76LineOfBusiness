OVERALL NOTES: 

Web.config is linked from the ui, if you bring in another UI just link the new config
hold alt and click drag the new config into Projects that expect a config file. 
Tests still need their own app.config

You can delete and reseed any db if the web.config points to a Server location, 
the db is created to match the models, with equal constraints

Ninject
No Logic in constructors, try not to use new, instead create singletons of non data
two ways to inject [Inject] propname {get;set;}, and by calling it in the constructor with a private declaration

Global.asax Application_error to catch all errors
-At this point the only C# try catch logic wanted is when logging, or wanting to skip a minor/expected exception

JS testing - Chutzpah is installed at the solution level, 
	--all other references will show in the nuget package as installed



