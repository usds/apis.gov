# APIs.gov Open Product Planning

## Problem
*(The top 1-3 problems you want to solve.)*

Currently, there does not exist a complete and authoritative list of APIs created and maintained by the federal government. This hampers efficiency within government — people inside agencies often do not know what APIs have been created or are available to them, creating silos of information and services — and outside of it in both the public and private sectors. 

1. Create a web accessible list of known federal government APIs.
2. Create a way for individuals at agencies to add their APIs to the list.
3. Create a mechanism for individuals to flag APIs that should be removed from the list.

**_Specifically excluded from the initial effort…_**

Requiring agencies to create and/or maintain a machine-readable list of APIs on their domains. We recognize the high value of this, but acknowledge that this is the hardest part of the current problem to solve, as evidenced by our interactions with the team at Code.gov in their efforts to create and maintain a catalog of open source projects within the government. Making this a viable method for the ongoing maintenance of a government API directory would likely require mandates (with penalties for non-compliance)…and we only have 34 hours to complete our prototype.

## Solution
*(Outline your proposed solution for each problem.)*

We will creating a static site through Github Pages that will serve as an API registry for US federal government APIs and will allow users to find and view government APIs, register their own API, and flag zombie APIs for removal. Additions to the registry will be largely automated via scraping any existing hosted apis.json files from agency web sites as well as allowing form submissions. All submissions will be approved by a human reviewer.

## Key Metrics
*(How will you measure success?)*


## User Profiles
*(Who are you building this for? Who will your early adopters be?)*
Target audience and early adopters: 

	▪	Agency Engineers 
	▪	Agency Product folks or Business Owners 
	▪	Non-agency Engineers
	▪	Non-agency Product folks

It is to see how early adopters of this tool could come from across these groups of users. From within the government, the most likely early users are going to be those who have already developed APIs, and civic tech minded individuals and businesses in the private sector are regularly on the lookout for any evidence of progress toward transparency in gov tech.

## User Channels
*(How will you get new users?)*
  * Talk about within USDS
  * Get USDSers to talk about it at their own agencies, particularly with agency CIOs and CDOs (Chief Data Officers)
  * Blog and/or social media posts

## Resources Required
*(What do you need to build an MVP (Minimum Viable Product) — design, development, expertise, hardware requirements and other costs?)*

**For the prototype:** developers to make the things, GitHub access, 

***

**_For open sourcing purposes…_**
## Contributor Profiles
Contribution types and ideal contributors
*(What do your contributors look like? Be sure to include the different expertises outlined in Resources Required.)*

## Contributor Channels
*(How will you gain new contributors?)*
