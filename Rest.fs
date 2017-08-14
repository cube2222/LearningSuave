module LearningSuave.Api

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Successful
open Suave.WebPart
open Suave.Filters
open Suave.RequestErrors

let (>=>) = compose

[<AutoOpen>]
module Rest =
    type RestResource<'a> = {
        GetAll: unit -> 'a seq
        Create: 'a -> int
        Update: 'a -> int option
        Get: int -> 'a option
        Delete: int -> int option
    }

    let JSON v =
        let settings = JsonSerializerSettings()
        settings.ContractResolver <- CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, settings)
        |> OK
        >=> Writers.setMimeType "application/json;	charset=utf-8"

    let fromJSON<'a> json =
        JsonConvert.DeserializeObject<'a> json

    let getResourceFromRequest<'a> request =
        let getString rawForm =
            System.Text.Encoding.UTF8.GetString rawForm
        request.rawForm |> getString |> fromJSON<'a>

    let notFound = NOT_FOUND "Resource not found"

    let handleResource error = function
        | Some r -> r |> JSON
        | _ -> error

    let rest resourceName resource =
        let resourcePath = "/" + resourceName
        let resourcePathById = 
            let path = resourcePath + "/%d"
            PrintfFormat<int -> string,unit,string,string,int>(path)

        let getAll = warbler (fun _ -> resource.GetAll() |> JSON)
        let create = request (getResourceFromRequest >> resource.Create >> JSON)
        let update = request (getResourceFromRequest >> resource.Update >> handleResource notFound)
        let get = resource.Get >> handleResource notFound
        let delete = resource.Delete >> handleResource notFound
        
        choose [
            path resourcePath >=> choose [
                GET >=> getAll
                POST >=> create
                PUT >=> update
            ]
            GET >=> pathScan resourcePathById get
            DELETE >=> pathScan resourcePathById delete            
        ]
        
