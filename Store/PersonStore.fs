namespace FundManager.Store

open System
open System.IO
open FundManager.Domain
open FundManager.Logging
open FundManager.Store

type IPersonStore =
    interface
       abstract member Add: Person -> Result<PersonId, string>
       abstract member Delete: Person -> Result<bool, string>
       abstract member Find: PersonId -> Result<Person, string>
       abstract member GetAll: unit -> Result<Person list, string>
       abstract member Update: Person -> Result<bool, string>
    end

type PersonStore (dbDirectory: DirectoryInfo, log: ILog) =
    let db = Common.getConfiguredLiteDB dbDirectory

    interface IPersonStore with
        member __.Add(person: Person): Result<PersonId, string> =
            db.GetCollection<Person>(Constants.CollectionNames.Person)
            |> Common.add log person
            |> Result.bind (fun x -> x |> int |> Ok)

        member __.Delete(person: Person): Result<bool, string> = 
            db.GetCollection<Person>(Constants.CollectionNames.Person)
            |> Common.delete log person.Id

        member __.Find(personId: PersonId): Result<Person, string> = 
            db.GetCollection<Person>(Constants.CollectionNames.Person)
            |> Common.find log personId

        member __.GetAll(): Result<Person list, string> = 
            db.GetCollection<Person>(Constants.CollectionNames.Person)
            |> Common.findAll log 

        member __.Update(person: Person): Result<bool, string> = 
            db.GetCollection<Person>(Constants.CollectionNames.Person) 
            |> Common.update log person

    interface IDisposable with 
        member __.Dispose() = 
            db.Dispose()

