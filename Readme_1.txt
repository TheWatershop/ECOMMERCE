Installation Notes:
To be opened by visual studio 2012, will ask to install razer so do so.
Web.config needs to be changed to point at current database, this will always be different coming from a local machine to here. can be located:nop\web\app_data\settings.txt
New template does not exist in this version, so to install follow these steps:
Open in VS
Build
navigate to the bin folder, find all the .dll inside, copy it to the same folder using iis manager, and clicking browse to on the website
paste the .dll,s from the solution bin folder to the sites bin folder.
Restart iis through iis manager, alternatively open elevated cmd and type in iisreset and press enter
Any cshtml changes need to be done inside the site folder, open the cshtml with notepad and make the changes.

Alternatively, since most of the changes exist in the solution, install the new template into the solution, and publish it over the current site.