namespace FundManager.Store

open FundManager.Domain
open FundManager.Logging
open LiteDB
open LiteDB.FSharp
open System.IO

type DbStore (dbDirectory: DirectoryInfo, log: ILog) =
    let onError ex = Common.onError log ex

    let configureMapper () = 
        let mapper = new FSharpBsonMapper()
        mapper

    let db = new LiteDatabase(dbDirectory.ToString() + Constants.DbFileName, configureMapper())

    let funds (db: LiteDatabase) = db.GetCollection<Fund.Entity>("funds")
    let people (db: LiteDatabase) = db.GetCollection<Person.Entity>("people")
    let transactions (db: LiteDatabase) = db.GetCollection<Transaction.Entity>("transactions")
    
    let add (entity: 'a) (collection: LiteCollection<'a>) =
        try
            log.Info "Inserting entity into collection '%s' \n'%A'" collection.Name entity
            collection.Insert(entity) |> Ok
        with 
        | ex -> onError ex
    
    let update (entity: 'a) (collection: LiteCollection<'a>) =
        try 
            log.Info "Updating entity in collection '%s' \n'%A'" collection.Name entity
            collection.Update(entity) |> Ok
        with 
        | ex -> onError ex

    let find (entityId: int) (collection: LiteCollection<'a>) = 
        try 
            log.Info "Fetching entity with id '%d' in collection '%s'" entityId collection.Name
            collection.FindById(BsonValue((entityId |> int))) |> Ok
        with 
        | ex -> onError ex

    let delete (entityId: int) (collection: LiteCollection<'a>) = 
        try 
            log.Info "Deleting entity with id '%d' from collection '%s'" entityId collection.Name
            collection.Delete(BsonValue((entityId |> int))) |> Ok
        with 
        | ex -> onError ex

    let findAll (collection: LiteCollection<'a>) = 
        try
            log.Info "Fetching all entities from collection '%s'" collection.Name
            collection.FindAll() |> List.ofSeq |> Ok
        with 
        | ex -> onError ex

        
    (* exposed methods here *)

    member __.Add(entity: Entity) = 
        match entity with 
        | Fund value            -> db |> funds |> add value
        | Person value          -> db |> people |> add value
        | Transaction value     -> db |> transactions |> add value

    member __.Get(entityId: EntityId) =
        match entityId with 
        | FundId id             -> db |> funds |> (find id >> Result.map Entity.Fund)
        | PersonId id           -> db |> people |> (find id >> Result.map Entity.Person)
        | TransactionId id      -> db |> transactions |> (find id >> Result.map Entity.Transaction)

    member __.Update(entity: Entity) = 
        match entity with 
        | Fund value            -> db |> funds |> update value
        | Person value          -> db |> people |> update value
        | Transaction value     -> db |> transactions |> update value

    member __.Delete(entityId: EntityId) =
        match entityId with 
        | FundId id             -> db |> funds |> delete id
        | PersonId id           -> db |> people |> delete id
        | TransactionId id      -> db |> transactions |> delete id

    member __.GetAll(entityType: EntityType) =
        match entityType with
        | FundType              -> db |> funds |> findAll |> Result.map (List.map Entity.Fund)
        | PersonType            -> db |> people |> findAll |> Result.map (List.map Entity.Person)
        | TransactionType       -> db |> transactions |> findAll |> Result.map (List.map Entity.Transaction)

    