# GitMonitor
Monitor and query changes across a collection of Git repositories

The introductory blog post can be found here - https://mikefourie.wordpress.com/2016/04/24/gitmonitor/


# Web Api

## Query Commits

**By Date Range**
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
   

## Search
