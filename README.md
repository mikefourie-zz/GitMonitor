# GitMonitor
Monitor and query changes across a collection of Git repositories

The introductory blog post can be found here - https://mikefourie.wordpress.com/2016/04/24/gitmonitor/


# Web Api
**Query commits by date range**
----
  Returns commits between the provided dates (inclusive).

* **URL**
  /api/commits/[startdate]/[enddate]

* **Method:**
  `GET`
  
*  **URL Params**
   **Required:**
   `startdate=[datetime]`

* **Sample Calls**
  * Commits between 1 Jan 2015 and today: ```/api/commits/1 Jan 2015```
  * Commits between 1 Jan 2016 and 30 Jan 2016 today: ```/api/commits/1 Jan 2016/30 Jan 2016```
   

**Find a commit**
----
  Returns commits between the provided dates (inclusive).

* **URL**
  /api/search/[path]/[commit]

* **Method:**
  `GET`
  
*  **URL Params**
   **Required:**
 
   `commit=[sha1]`

* **Sample Calls**
  * Find commit in default path: ```/api/search/a3er4w```
  * Find commit in work path: ```/api/search/work/a3er4w```
  * Find commit in all paths: ```/api/search/*/a3er4w```
