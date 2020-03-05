# Notes

We know people aren’t going to maintain an apis.json file at least for awhile, so building tools or processes around this doesn’t make sense yet. So, we will begin by making a static site with a web form so that people can submit info about their APIs; then that dtaa will be accessible in a standardized format (apis.json) and searchable from the site.

## Implementation details

Info needed for each API:
https://github.com/apis-json/api-json/blob/5d3e77683884f46a6077bb3d49dc5d2d1035bfd5/spec/apisjson.txt#L185
  * Name [Mandatory]: name of the API
  * Description [Mandatory]: human readable text description of the API.
  * Image [Optional]: URL of an image which can be used as an "icon" for the API if displayed by a
        search engine.
  * humanUrl: Web URL corresponding to human readable information about the API.
  * baseUrl: Web URL corresponding to the root URL of the API or primary endpoint.
  * Version [Optional]: String representing the version number of the API this description refers to.
  * Tags: (collection) [Optional]: this is a list of descriptive strings which identify the API.
as an array.
  * properties (collection):
    - type: please see reserved keywords below.
    - url or value.
  * contact (collection)
    - [Person or Organization - see below]
    
We'd also like:
- auth info 

## Similar projects
### US
*Code.gov*
Problems: 
* Incentivizing maintaining these files
* Validating json files

### World
*UK API catalogue*
https://alphagov.github.io/api-catalogue/#uk-government-apis

*New Zealand*


*Data.gov*
Mandated to have data.json files at root of agency websites

## Standards

* APIs.json - https://github.com/apis-json/api-json
* /.well-known - https://en.wikipedia.org/wiki/List_of_/.well-known/_services_offered_by_webservers 

