# Moving APIs.gov forward as a product

Are you interested in moving this effort forward and making our APIs.gov dream a reality? Here are some things to consider.

### Some Reading
Here are a couple of blog posts that talk about the basic problem we were trying to address when we started this whole thing.
1. <a href="https://civicunrest.com/2020/01/27/where-are-government-api-directories">Where are the Government API Directories?</a>
2. <a href="https://apievangelist.com/2017/07/27/state-of-apis-in-the-federal-government/">State of APIs in the Federal Government</a>

### User Research and Validation (please!)
While the hackathon team that explored this idea and gave birth to this prototype did a cursory round of user research with the people present in the room from other teams (you can find that <a href="https://github.com/usds/apis.gov/blob/master/product/User-Research.md">here</a>), in order to really address the problems we've identified and create a product that offers value, more research is needed. Some questions to consider:
  * Did we identify the right audiences?
  * What are users really looking for from an API directory/registry?
  * What do the workflows in various angencies look like to add an apis.json file to their sites?
  * Are the assumptions we made completely off-base?
  * So many more...
  
### Content Expansion
Consider whether this site should include general information about APIs and their utility in the gov tech space, rather than just be a simple directory or government APIs. It seems that the site could easily accomodate being the one-stop place for really learning about APIs and finding resources for both using and creating them. The source with the most information about this today is the <a href="https://digital.gov/2013/04/30/apis-in-government/">APIs in Government</a> page on digital.gov. That page is worth a look for anyone considering this effort.

Additionally, it would be worth considering if expanded inclusion of APIs from tribal, state and local governments would be worthwhile.

### Hurdles
We know from the history of Code.gov that one of their biggest hurdles to participation is getting federal agencies to create and maintain the code.json files from each site that they use to maintain their open source project lists. We have not confirmed it, but suspect that Data.gov is facing similar issues in their efforts to catalog and track open datasets from federal agencies. User research should start to surface if using this technical approach of harvesting data from a hosted JSON file is simply not viable. If that is found to be the case, then a new technical approach will be need to be created.

If the hosted JSON approach remains, significant product thought will need to be given toward how to get agencies to create and maintain those files. This will likely require significant outreach to "sell" the importance of doing this. Additionally, some kind of mandate may need to be worked on. We all know that most guidance memos go largely unheeded, since they typically do not include any penalties for non-compliance. The hackathon team that worked on this did not spend significant time trying to solve this problem. However, we did find one DoD Instruction memo that includes a line directing DoD and OSD Component Heads to "...submit public Web application APIs to be included on the DoD Developers Website
(https://www.defense.gov/developer/) and Data.gov’s Web API catalog at https://data.gov." That memo can be found at https://www.esd.whs.mil/Portals/54/Documents/DD/issuances/dodi/817001p.pdf and the referenced line is on page 8. While these kinds of directives seem to have the most success at DoD, it may be worth considering something similar inside of individual agencies or OMB -- though OMB guidance may the least effective approach.
