[<RequireQualifiedAccess>]
module FundManager.Store.Common

    open FundManager.Logging
    open LiteDB
    open LiteDB.FSharp
    open System.IO

    let internal onError (log: ILog) (ex: exn) = 
        log.Error "Error message: %s" ex.Message
        log.Error "Stacktrace: \n%s" ex.StackTrace
        Error ex.Message

    let private configureMapper () = 
        let mapper = new FSharpBsonMapper()
        mapper

    let internal getConfiguredLiteDB (dbDirectory: DirectoryInfo) =
        let dbFilePath = System.IO.Path.Combine(dbDirectory.ToString(), Constants.DbFileName);
        new LiteDatabase(dbFilePath, configureMapper())

    let internal add (log: ILog) (entity: 'a) (collection: LiteCollection<'a>) =
        try
            log.Info "Inserting entity into collection '%s' \n'%A'" collection.Name entity
            collection.Insert(entity) |> Ok
        with 
        | ex -> onError log ex

    let internal delete (log: ILog) (entityId: int) (collection: LiteCollection<'a>) = 
        try 
            log.Info "Deleting entity with id '%d' from collection '%s'" entityId collection.Name
            collection.Delete(BsonValue((entityId |> int))) |> Ok
        with 
        | ex -> onError log ex

    let internal find (log: ILog) (entityId: int) (collection: LiteCollection<'a>) = 
        try 
            log.Info "Fetching entity with id '%d' in collection '%s'" entityId collection.Name
            collection.FindById(BsonValue((entityId |> int))) |> Ok
        with 
        | ex -> onError log ex

    let internal findAll (log: ILog) (collection: LiteCollection<'a>) = 
        try
            log.Info "Fetching all entities from collection '%s'" collection.Name
            collection.FindAll() |> List.ofSeq |> Ok
        with 
        | ex -> onError log ex
    
    let internal update (log: ILog) (entity: 'a) (collection: LiteCollection<'a>) =
        try 
            log.Info "Updating entity in collection '%s' \n'%A'" collection.Name entity
            collection.Update(entity) |> Ok
        with 
        | ex -> onError log ex