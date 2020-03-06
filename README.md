# APIs.gov

APIs.gov is a static site that serves as an API registry for US government APIs. 

On this website, you will be able to find and view govenrment APIs and register your own API. API data will be stored in machine-readable format (as an APIs.json file). Additions to the registry will be largely automated (via scraping hosted apis.json files from agency websites and via form submissions) but always approved by a human reviewer.

# Generating the Catalog: 

First, you'll need some prerequisites: 

`brew install opam` (The OCaml Package manager & ecosystem)

Then once that is finished (be sure to follow the homebrew instructions for OPAM after installing): 

`opam install dune`

Then `cd` into `/catalog` and run:

`dune external-lib-deps --missing .`

(at this point follow the command line for all `opam install`s)

`dune build`

It should at this point generate an executable. For the sake of simplicity, call it like: 
`./_build/default/Catalog.exe`

PS: if you'd like to do development, please: 
`opam install merlin`


# Validating the ApiRecords: 

First, you'll need some prerequisites: 

`brew install opam` (The OCaml Package manager & ecosystem)

Then once that is finished (be sure to follow the homebrew instructions for OPAM after installing): 

`opam install dune`

Then `cd` into `/validation` and run:

`dune external-lib-deps --missing .`

(at this point follow the command line for all `opam install`s)

PS: if you'd like to do development, please: 
`opam install merlin`

`dune build`

It should at this point generate a Javascript file. For the sake of simplicity, call it like: 
` node ./_build/default/validation.bc.js`

Or play with it in the REPL to invoke functions, like: 

`cp ./_build/default/validation.bc.js ./_build/default/validation.js`
`node`
...then inside node:
```javascript
const ValidatorModule = require("./_build/default/validation");
```
```javascript
ValidatorModule.BrowserValidator.is_valid('{"name":"foo", "description":"some foo", "human_url":"http://foo.human.com", "base_url":"http://foo.com", "image": "", "version": "", "tags": [], "properties": [], "maintainers": { "fullname":"", "email":"", "url":"", "org":"", "address":"", "telephone":"","x_twitter":"", "x_github":"", "photo":"", "v_card":""}, "include": []}');
```
## API Record JSON format: 

```json
{
	"name": "foo",
	"description": "some foo",
	"url": "http://foo.com",
	"human_url": "http://foo.human.com",
	"base_url": "http://foo.com",
	"image": "",
	"version": "",
	"tags": [],
	"properties": [],
	"maintainers": {
		"fullname": "",
		"email": "",
		"url": "",
		"org": "",
		"address": "",
		"telephone": "",
		"x_twitter": "",
		"x_github": "",
		"photo": "",
		"v_card": ""
	},
	"include": []
}
```
* Optional values are stringly typed, by empty string. Yes, this is trash and yes, I feel bad about it.