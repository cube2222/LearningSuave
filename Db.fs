module LearningSuave.Db
open System.Collections.Generic
type Person = {
    Id: int
    Name: string
    Age: int
    Email: string
}

module Db =
    let private peopleStorage = Dictionary<int, Person>();
    let getPeople() =
        peopleStorage.Values :> seq<Person>
    let createPerson person =
        let newId = peopleStorage.Count
        let newPerson = {person with Id = newId}
        peopleStorage.Add(newId, newPerson)
        newId
    let updatePerson person =
        if peopleStorage.ContainsKey person.Id
        then 
            peopleStorage.[person.Id] <- person
            Some person.Id
        else None
    let getPerson id =
        if peopleStorage.ContainsKey id
        then
            Some peopleStorage.[id]
        else None
    let deletePerson id =
        if peopleStorage.Remove id
        then Some id
        else None