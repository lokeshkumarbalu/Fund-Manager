namespace FundManager.Store

open System
open System.IO
open FundManager.Domain
open FundManager.Logging
open FundManager.Store

type IFundStore =
   abstract member Add: Fund -> Result<FundId, string>
   abstract member Delete: Fund -> Result<bool, string>
   abstract member Find: FundId -> Result<Fund, string>
   abstract member GetAll: unit -> Result<Fund list, string>
   abstract member Update: Fund -> Result<bool, string>

type FundStore (dbDirectory: DirectoryInfo, log: ILog) =
    let db = Common.getConfiguredLiteDB dbDirectory

    interface IFundStore with
        member __.Add(fund: Fund): Result<FundId, string> =
            db.GetCollection<Fund>(Constants.CollectionNames.Fund)
            |> Common.add log fund
            |> Result.bind (fun x -> x |> int |> Ok)

        member __.Delete(fund: Fund): Result<bool, string> = 
            db.GetCollection<Fund>(Constants.CollectionNames.Fund)
            |> Common.delete log fund.Id

        member __.Find(fundId: FundId): Result<Fund, string> = 
            db.GetCollection<Fund>(Constants.CollectionNames.Fund)
            |> Common.find log fundId

        member __.GetAll(): Result<Fund list, string> = 
            db.GetCollection<Fund>(Constants.CollectionNames.Fund)
            |> Common.findAll log 

        member __.Update(fund: Fund): Result<bool, string> = 
            db.GetCollection<Fund>(Constants.CollectionNames.Fund) 
            |> Common.update log fund

    interface IDisposable with 
        member __.Dispose() = 
            db.Dispose()

