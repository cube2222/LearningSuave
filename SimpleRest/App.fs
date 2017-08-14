open SimpleRest.Api
open SimpleRest.Db
open Suave.Web
open Suave.Successful

[<EntryPoint>]
let main argv =
    let peopleWebPart = rest "people" {
        GetAll = Db.getPeople
        Create = Db.createPerson
        Update = Db.updatePerson
        Get = Db.getPerson
        Delete = Db.deletePerson
    }
    startWebServer defaultConfig peopleWebPart
    0