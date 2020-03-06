open Js_of_ocaml

module Validator = struct 

  (** TODO FIX OPTIONAL VALUES TO BE NON EMPTY STRINGS *)
  type entity = {
    (** String Value corresponding to the Full Nname name of the individual / organization. *)
    fullname: string; 
    (** String Value corresponding to the email address of the individual / organization *)
    email: string;
    (** String Value corresponding to a web page about the individual individual / organization *)
    url: string;
    (** String Value representing the name of the organization associated with the cCard. *)
    org: string;
    (** String Value corresponding to the physical address of the individual / organization. *)
    address: string;
    (** String Value corresponding to the phone number including country code of the individual / organization. *)
    telephone: string;
    (** String Value corresponding to the twitter username of the individual / organization (convention do
          not use the "@" symbol). Note - these are X- since this is the way they are / would be in vCard. *)
    x_twitter: string;
    (** String Value corresponding to the github username of the individual / organization. Note - these are X- since this is the way they are / would be in vCard. *)
    x_github: string;
    (** URL corresponding to an image which could be used to represent the individual / organization. *)
    photo: string;
    (** URL pointing to a vCard Objective RFC6350 *)
    v_card: string;
  } [@@ deriving yojson]  [@@deriving show]

  (** TODO FIX OPTIONAL VALUES TO BE NON EMPTY STRINGS *)
  type apiconfig = {
    (** name of the API *)
    name : string;
    (** human readable text description of the API. *)
    description: string;
    (** URL of an image which can be used as an "icon" for the API if displayed by a
              search engine. *)
    image: string;
    (** Web URL corresponding to human readable information about the API. *)
    human_url: string;
    (** Web URL corresponding to the root URL of the API or primary endpoint. *)
    base_url: string;
    (** String representing the version number of the API this description refers to. *)
    version: string;
    (** this is a list of descriptive strings which identify the API.
      as an array. *)
    tags: string array;
    properties: (string * string) array;
    (**   * Maintainers (collection)    [VCARD]
        * [Person or Organization ...] *)
    maintainers: entity;
    (** 
    * Include (collection) [Optional]
        * Name [Mandatory]: name of the APIs.json file referenced.
        * Url [Mandatory]: Web URL of the file. *)
    _include: string array [@key "include"];
  } [@@deriving yojson] [@@deriving show]

  let to_api_record (json: Yojson.Safe.t) =
    apiconfig_of_yojson json

  let from_api_record (api_record: apiconfig) =
    apiconfig_to_yojson api_record

  (** TODO FIX THIS *)
  let is_valid_optional_url uri =
    begin
    if uri = ""
    then true
    else
      begin try 
        Uri.of_string uri |> ignore;
        true
      with _ -> false end end

  (** TODO FIX THIS *)
  let is_valid_required_url uri =
    begin try 
      if uri = ""
      then false
      else begin
      Uri.of_string uri |> ignore;
      true end
    with _ -> false end

  let is_valid_optional_email _ = true
  let is_valid_optional_telephone _ = true

  let is_valid_entity =
    function
      {        
        url = u;
        email = em;
        telephone = t;
        x_github = github;
        x_twitter = twitter;
        photo = pic;
        v_card = vc;
        _
      } -> 
      List.fold_left (&&) true      
      [ 
        is_valid_optional_url u;
        is_valid_optional_email em;
        is_valid_optional_telephone t;
        is_valid_optional_url github;
        is_valid_optional_url twitter;
        is_valid_optional_url pic;
        is_valid_optional_url vc;
      ]

  let is_valid_api record =
    match record with
    | {
      image = i;
      human_url = h_url;
      maintainers = ent;
      _
      } ->
      List.fold_left (&&) true      
      [ 
        is_valid_optional_url i;
        is_valid_required_url h_url;
        is_valid_entity ent;
      ]

end

let _ =
  Js.export "BrowserValidator" 
    (object%js
       method is_valid_api jsonstring =
        Js.string jsonstring
        |> Js.to_string
        |> Yojson.Safe.from_string
        |> Validator.to_api_record
        |> function 
          | Ok apiconfig -> 
            begin Validator.is_valid_api apiconfig
            |> function 
            | true -> Js._true
            | false -> Js._false end
          | Error err -> failwith err

      end)


