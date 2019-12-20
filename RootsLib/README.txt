
1. Go into roots magic and synch a new copy of dnamatchs
2. copy .rmgc(usual location is C:\Users\george\Documents\) file into sqliteimporter folder 
3. go to command prompt  
   go to sqliteimporter folder (this is contained within this solution)	
   run sqlite3 roots.rmgc < import.txt
4. run dropdata.sql script on roots db
5. run generated import.sql on roots db (it can take some time about 12 minutes last time i ran it)

-- in c# --
7. find ancestors of _persons 
8. record against persons
9. import persons into dnaged persons table under 10000
