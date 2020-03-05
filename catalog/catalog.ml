open Lwt
open Cohttp_lwt_unix

let list_unique l = 
  let starting_state = Hashtbl.create (List.length l) in
  let rec aux acc rest state =  
    match rest with 
    | x::xs -> 
      begin
        try begin
          let _ = Hashtbl.find state x in
          aux acc xs state
        end with Not_found -> begin
          Hashtbl.add state x Int32.one;
          aux (x::acc) xs state
        end
      end
    | [] -> acc
  in aux [] l starting_state

module Validator = struct 

  (** TODO: Implement Validation of API Config Record *)
  let validate config = print_endline config

end

module Catalog = struct

  exception CouldNotReachDotGovRegistry of string

  let dotgovs = "https://raw.githubusercontent.com/GSA/data/master/dotgov-domains/current-full.csv"

 (** TODO: Change to /api.json *)
 let api_config_file = "/robots.txt"

  let build_registry () = 
    let registry : Uri.t list Lwt.t=
      Client.get (Uri.of_string dotgovs) >>= fun (_, resp_body) -> 
        resp_body |> Cohttp_lwt.Body.to_string >|= fun body ->
          let csv_in = Csv.of_string body in
          Csv.fold_left ~f:(fun acc row -> 
            let domain_name = List.hd row in
            (Uri.of_string ("http://" ^ domain_name ^ api_config_file)::acc)
            ) ~init:[] csv_in
          |> list_unique (* Validates that we only hit a give .gov once  *)
    in Lwt.bind registry (fun config_uris -> 
      Lwt_list.iter_p (fun uri ->
          Client.get uri >>= fun (_, resp_body) ->
            resp_body |> Cohttp_lwt.Body.to_string >|= fun body ->
            Validator.validate body
      ) config_uris)
end

let () = Lwt_main.run @@ Catalog.build_registry ()
