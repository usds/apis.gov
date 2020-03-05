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

`dune build`

It should at this point generate an executable. For the sake of simplicity, call it like: 
`./_build/default/Catalog.exe`

PS: if you'd like to do development, please: 
`opam install merlin`
