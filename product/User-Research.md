# User Research

## Info about APIs
At the 2020 USDS hackathon, we spoke with 20 USDSers (engineers, product managers, designers, and procurement specialists) about what kind of high-level info about APIs they'd want from an API discovery tool like an API registry.

Info | Popularity (1-5) | User type | Notes
--- | --- | --- | ---
Access (open, must apply, internal, etc) | 5 | All | At least just saying if this is open or not would be valuable. For procurmeent/product folks, this info can help get stakeholder buy-in. 
Keywords/tags/categories | 5 | All |
Who uses it? | 4 | All | For PMs and procurement, this info might help you understand if this API is relevant for you. They want to know what agencies use this. One engineer suggested that you could go ask other consumers about the developer experience and how good the API is. PMs also want to know how it's working for other people. 
Auth | 4 | Engineers | Maybe overlaps with "Access" info
Pricing models | 4 | All | Maybe overlaps with "Access" info
Format | 4 | Engineers | This could be response format like JSON/XML/Protobuff or architectural style like REST/SOAP/GraphQL. Some users disagreed with this, saying that format isn't necessary because they will use an API if it's relevant regardless of format
Sensitivity/compliance of data | 4 | All | Is this PII? Has it been anonymized? Is it fully open? Is it compliant with HIPAA, FERS, CJIS, etc, rules?
How do I use this? | 4 | Engineers | Lots of overlap with access, auth, sandbox, etc
Freshness of data or updates | 3 | Engineers |
Sandbox / How can I play with this? | 3 | Engineers |
Maintenance Status | 3 | Engineers | Is this API under active maintenance? Is it in beta/pilot? Is it deprecated?
Uptime/Availability Status | 3 | Engineers | This could be a link to the API's status page or SLAs or an uptime rating 
How do other people use this? | 2 | PMs, Procurement | This helps them understand why they would want to use this. Would be great to see projects/products. How's it working for those people?
Who maintains this? | 2 | Engineers, PMs | PMs want a point of contact and agency info.
Data schema / structure | 2 | Engineers |
Link to source code repo | 1 | Engineers |
Rate limiting | 1 | Engineers |
List of keys available in data | 1 | Engineers | Might help you understand the data available, although you might need a data dictionary to really understand keys. Maybe have example of key/value pair.
Licenses on data | 1 | Engineers |
What tech is compatible with this API? | 1 | PMs |
Where is the data coming from? | 1 | PMs, procurement |


## API registry product features
For an API registry product, some feature requests we got:

Feature | Popularity (1-5) | User type | Notes
--- | --- | --- | --- 
How are the APIs connected? | | | Do some APIs share data elements that I can join on? Are some APIs built off of other APIs? How are the APIs connected and how can I utilize those connections in my own product? Which APIs can I use the same API key for?
What APIs are trending or popular? | | |
"If you like this API, you might like..." recommendations | | | This might help people find APIs that are connected.
Community can rate or comment on quality of API (e.g. data quality) | | |
Dark mode | | Engineers |
VI  mode with hotkeys | | Engineers |
How much data is available in an API and how representative of the entire population is it? | | | Not sure how you'd determine the latter.
